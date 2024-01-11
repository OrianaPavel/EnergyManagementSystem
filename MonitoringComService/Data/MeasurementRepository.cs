using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MonitoringComService.Entities;

namespace MonitoringComService.Data
{
    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MeasurementRepository> _logger;

        public MeasurementRepository(AppDbContext context, ILogger<MeasurementRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Measurement?> CreateMeasurementAsync(Measurement measurement)
        {
            if (measurement == null)
            {
                _logger.LogWarning("In CreateDevice method cannot insert new record in DB because device object is null");
                return null;
            }
            _context.Measurements.Add(measurement);
            await _context.SaveChangesAsync();
            return measurement;
        }

        public async Task<Measurement?> GetMeasurementByIdAsync(int measurementId)
        {
            return await _context.Measurements
                                 .FirstOrDefaultAsync(m => m.MeasurementId == measurementId);
        }

        public async Task<IEnumerable<Measurement>> GetMeasurementsByDeviceIdAsync(int deviceId)
        {
            return await _context.Measurements
                                 .Where(m => m.DeviceId == deviceId)
                                 .ToListAsync();
        }


        public async Task<IEnumerable<Measurement>> GetMeasurementsByDeviceIdAndDateAsync(int deviceId, DateTime date)
        {
            /*
            return await _context.Measurements
                                 .Where(m => m.DeviceId == deviceId &&
                                             DateTimeOffset.FromUnixTimeMilliseconds(m.Timestamp).Date == date.Date)
                                 .OrderBy(m => m.Timestamp)
                                 .ToListAsync();
            */
            var startOfDay = new DateTimeOffset(date.Date).ToUnixTimeSeconds();
            _logger.LogInformation($"Processing query with startOfDay {startOfDay}");
            var endOfDay = new DateTimeOffset(date.Date.AddDays(1)).ToUnixTimeSeconds();
            _logger.LogInformation($"Processing query with endOfDay {endOfDay}");
            var measurements = await _context.Measurements
                .Where(m => m.DeviceId == deviceId && m.Timestamp >= startOfDay && m.Timestamp < endOfDay)
                .ToListAsync();
            // Log the response
            _logger.LogInformation($"Retrieved {measurements.Count} measurements for device {deviceId} on date {date.Date}");

            // Optionally log the details of each measurement
            foreach (var measurement in measurements)
            {
                _logger.LogInformation($"MeasurementId: {measurement.MeasurementId}, Timestamp: {measurement.Timestamp}, Value: {measurement.MeasurementValue}");
            }

            return measurements;

        }
        public async Task<int> GetTotalEnergyAndMaxByHour(long unixTimeInSeconds, int deviceId)
        {
            //DateTime timestamp = DateTime.UnixEpoch.AddSeconds(unixTimeInSeconds);
            DateTime timestamp = DateTime.UnixEpoch.AddSeconds(unixTimeInSeconds).ToUniversalTime();
            timestamp = timestamp.AddHours(2);
            var startOfHour = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, 0, 0);
            var endOfHour = startOfHour.AddHours(1);

            long startOfHourInSeconds = new DateTimeOffset(startOfHour).ToUnixTimeSeconds();
            long endOfHourInSeconds = new DateTimeOffset(endOfHour).ToUnixTimeSeconds();

            _logger.LogInformation("StartOfHour == " + startOfHour + " endOfHour == " + endOfHour);
            _logger.LogInformation("StartOfHour in seconds == " + startOfHourInSeconds + " endOfHour in seconds == " + endOfHourInSeconds);

            var measurements = await _context.Measurements
                                             .Include(m => m.Device)
                                             .Where(m => m.DeviceId == deviceId &&
                                                         m.Timestamp >= startOfHourInSeconds &&
                                                         m.Timestamp < endOfHourInSeconds)
                                             .ToListAsync();

            int totalEnergy = measurements.Sum(m => m.MeasurementValue);

            return totalEnergy;
        }


        public async Task UpdateMeasurementAsync(Measurement measurement)
        {
            _context.Measurements.Update(measurement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMeasurementAsync(int measurementId)
        {
            var measurement = await _context.Measurements
                                            .FirstOrDefaultAsync(m => m.MeasurementId == measurementId);

            if (measurement == null)
            {
                _logger.LogWarning("In DeleteMeasurement method cannot delete record in DB because user object is null");
                return;
            }

            _context.Measurements.Remove(measurement);
            await _context.SaveChangesAsync();

        }
    }
}
