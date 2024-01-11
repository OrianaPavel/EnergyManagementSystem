using System.Collections.Generic;
using System.Threading.Tasks;
using MonitoringComService.Data;
using MonitoringComService.DTOs;
using MonitoringComService.Entities;
using AutoMapper;
using HashidsNet;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace MonitoringComService.Services
{

    public class MeasurementService
    {
        private readonly IHubContext<SocketHub> _hubContext;
        private readonly IMeasurementRepository _measurementRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MeasurementService> _logger;
        private readonly IHashids _hashids;

        public MeasurementService(IMeasurementRepository measurementRepository, IMapper mapper, ILogger<MeasurementService> logger, IHashids hashids,IHubContext<SocketHub> hubContext, IDeviceRepository deviceRepository)
        {
            _measurementRepository = measurementRepository ?? throw new ArgumentNullException(nameof(measurementRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));

        }

        public async Task<MeasurementDto> CreateMeasurementAsync(MeasurementDto measurementDto)
        {
            var measurement = _mapper.Map<Measurement>(measurementDto);
            await _measurementRepository.CreateMeasurementAsync(measurement);
            await CheckAndHandleEnergyThreshold(measurement);
            return _mapper.Map<MeasurementDto>(measurement);
        }

        public async Task ProcessMeasurementData(string message)
        {
            try
            {
                var measurement = JsonSerializer.Deserialize<Measurement>(message);
                if (measurement != null)
                {
                    // Log the deserialized MeasurementDto
                    _logger.LogInformation("Deserialized MeasurementDto: {@MeasurementDto}", measurement);
                    //await CreateMeasurementAsync(measurement);
                    var createdMeasurement = await _measurementRepository.CreateMeasurementAsync(measurement);
                    await CheckAndHandleEnergyThreshold(createdMeasurement);
                }
                else
                {
                    _logger.LogWarning("Deserialized MeasurementDto is null for message: {Message}", message);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error for message: {Message}", message);
            }
        }

        public async Task<MeasurementDto?> GetMeasurementByIdAsync(string hashedMeasurementId)
        {
            int measurementId = getRawID(hashedMeasurementId);
            var measurement = await _measurementRepository.GetMeasurementByIdAsync(measurementId);

            if (measurement == null)
            {
                _logger.LogWarning($"Measurement with ID {measurementId} was not found.");
                return null;
            }

            return _mapper.Map<MeasurementDto>(measurement);
        }

        public async Task<IEnumerable<MeasurementDto>> GetMeasurementsByDeviceIdAsync(string hashedDeviceId)
        {
            int deviceId = getRawID(hashedDeviceId);
            var measurements = await _measurementRepository.GetMeasurementsByDeviceIdAsync(deviceId);
            if (!measurements.Any())
            {
                _logger.LogWarning($"No measurements found for deviceID {deviceId}.");
            }
            return _mapper.Map<IEnumerable<MeasurementDto>>(measurements);
        }

        public async Task<Dictionary<int, int>> GetHourlyMeasurementsByDeviceAndDateAsync(string hashedDeviceId, DateTime date)
        {
            int deviceId = getRawID(hashedDeviceId);
            var measurements = await _measurementRepository.GetMeasurementsByDeviceIdAndDateAsync(deviceId, date);

            var hourlyConsumption = InitializeHourlyConsumptionDictionary();

            for (int hour = 0; hour < 24; hour++)
            {
                var startOfHour = new DateTimeOffset(date.Date).AddHours(hour).ToUnixTimeSeconds();
                var endOfHour = new DateTimeOffset(date.Date).AddHours(hour + 1).ToUnixTimeSeconds() - 1;

                var measurementsOfHour = measurements.Where(m => m.Timestamp >= startOfHour && m.Timestamp < endOfHour).ToList();

                if (measurementsOfHour.Any())
                {
                    hourlyConsumption[hour] = measurementsOfHour.Sum(m => m.MeasurementValue);
                    /* Diferenta intre ultima si prima citire din ora ? caz particular cand exista o singura citire in ora respectiva
                    // Get the first and last measurement of the hour
                    var firstMeasurementOfHour = measurementsOfHour.First();
                    var lastMeasurementOfHour = measurementsOfHour.Last();

                    // Calculate the difference in measurements to find the consumption for the hour
                    hourlyConsumption[hour] = lastMeasurementOfHour.MeasurementValue - firstMeasurementOfHour.MeasurementValue;
                    */
                }
            }

            return hourlyConsumption;
        }

        private Dictionary<int, int> InitializeHourlyConsumptionDictionary()
        {
            var hourlyConsumption = new Dictionary<int, int>();
            for (int i = 0; i < 24; i++)
            {
                hourlyConsumption[i] = 0;
            }
            return hourlyConsumption;
        }


        public async Task UpdateMeasurementAsync(string hashedMeasurementId, MeasurementDto measurementDto)
        {
            int measurementId = getRawID(hashedMeasurementId);
            var measurement = await _measurementRepository.GetMeasurementByIdAsync(measurementId);
            if (measurement == null)
            {
                _logger.LogWarning($"Measurement with ID {measurementId} was not found during update.");
                return;
            }
            _mapper.Map(measurementDto, measurement);
            await _measurementRepository.UpdateMeasurementAsync(measurement);

            await CheckAndHandleEnergyThreshold(measurement);
        }
        private async Task CheckAndHandleEnergyThreshold(Measurement measurement)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(measurement.DeviceId);
            var totalEnergy = await _measurementRepository.GetTotalEnergyAndMaxByHour(measurement.Timestamp, measurement.DeviceId);
            var maxHourlyConsumption = device.MaxHourlyConsumption;
            _logger.LogInformation("MaxHourlyConsumption == " + maxHourlyConsumption + "TotalEnergy == " + totalEnergy);
            if (totalEnergy > maxHourlyConsumption)
            {
                string message = "Energy consumption threshold exceeded for " + _hashids.Encode(measurement.DeviceId) + "!";
                _logger.LogInformation("---->" + message);
                //await _webSocketHandler.SendMessageAsync(_hashids.Encode(device.UserId), message);
                await _hubContext.Clients.Group(_hashids.Encode(device.UserId)).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task DeleteMeasurementAsync(string hashedMeasurementId)
        {
            int measurementId = getRawID(hashedMeasurementId);
            await _measurementRepository.DeleteMeasurementAsync(measurementId);
        }

        private int getRawID(string hashedId)
        {
            int[] decoded = _hashids.Decode(hashedId);
            if (decoded.Length == 0)
            {
                throw new ArgumentException("Invalid ID.");
            }
            return decoded[0];
        }
    }
}