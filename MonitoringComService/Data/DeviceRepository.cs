using System;
using System.Threading.Tasks;
using MonitoringComService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MonitoringComService.Data
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly AppDbContext _context;

        private readonly ILogger<DeviceRepository> _logger;

        public DeviceRepository(AppDbContext context, ILogger<DeviceRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Device?> CreateDeviceAsync(Device device)
        {
            if (device == null)
            {
                _logger.LogWarning("In CreateDevice method cannot insert new record in DB because device object is null");
                return null;
            }

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<Device?> GetDeviceByIdAsync(int deviceId)
        {
            return await _context.Devices
                                 .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        }

        // Uncomment and implement if needed
        // public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        // {
        //     return await _context.Devices.ToListAsync();
        // }

        public async Task UpdateDeviceAsync(Device device)
        {
            _context.Devices.Update(device);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeviceAsync(int deviceId)
        {
            var device = await _context.Devices
                                       .FirstOrDefaultAsync(d => d.DeviceId == deviceId);

            if (device == null)
            {
                _logger.LogWarning("In DeleteDevice method cannot delete record in DB because user object is null");
                return;
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

        }
    }
}
