using System;
using System.Threading.Tasks;
using MonitoringComService.Data;
using MonitoringComService.DTOs;
using MonitoringComService.Entities;
using AutoMapper;
using HashidsNet;
using System.Text.Json;


namespace MonitoringComService.Services
{

    public class DeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceService> _logger;

        private readonly IHashids _hashids;

        public DeviceService(IDeviceRepository deviceRepository, IMapper mapper, ILogger<DeviceService> logger, IHashids hashids)
        {
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
        }

        public async Task<DeviceDto> CreateDeviceAsync(DeviceDto deviceDto)
        {
            var device = _mapper.Map<Device>(deviceDto);
            await _deviceRepository.CreateDeviceAsync(device);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto?> GetDeviceByIdAsync(string hashedDeviceId)
        {
            int deviceId = getRawID(hashedDeviceId);
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                _logger.LogWarning($"Device with ID {hashedDeviceId} was not found.");
                return null;
            }
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task UpdateDeviceAsync(string hashedDeviceId, DeviceDto deviceDto)
        {
            int deviceId = getRawID(hashedDeviceId);
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);

            if (device == null)
            {
                _logger.LogWarning($"Device with ID {hashedDeviceId} was not found during update.");
                return;
            }
            _mapper.Map(deviceDto, device);
            await _deviceRepository.UpdateDeviceAsync(device);

        }

        public async Task ProcessCreateDeviceMessage(string message)
        {
            try
            {
                var deviceDto = JsonSerializer.Deserialize<DeviceDto>(message);
                await CreateDeviceAsync(deviceDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing create device message");
            }
        }

        public async Task ProcessUpdateDeviceMessage(string message)
        {
            try
            {
                var updateInfo = JsonSerializer.Deserialize<(string hashedDeviceId, DeviceDto deviceDto)>(message);
                await UpdateDeviceAsync(updateInfo.hashedDeviceId, updateInfo.deviceDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing update device message");
            }
        }

        public async Task ProcessDeleteDeviceMessage(string message)
        {
            try
            {
                await DeleteDeviceAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing delete device message");
            }
        }

        public async Task DeleteDeviceAsync(string hashedDeviceId)
        {
            int deviceId = getRawID(hashedDeviceId);
            await _deviceRepository.DeleteDeviceAsync(deviceId);
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