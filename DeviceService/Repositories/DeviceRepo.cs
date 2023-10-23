using System.Collections.Generic;
using System.Linq;
using DeviceService.Entities;
using Microsoft.EntityFrameworkCore;
using UserService.Data;

namespace DeviceService.Repositories
{
    public class DeviceRepo : IDeviceRepo
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DeviceRepo> _logger;

        public DeviceRepo(AppDbContext context, ILogger<DeviceRepo> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void CreateDevice( ref Device device)
        {
            if (device == null)
            {
                _logger.LogWarning("In CreateDevice method cannot insert new record in DB because device object is null");
                return;
            }

            _context.Devices.Add(device);
        }

        public IEnumerable<Device> GetDevicesByUserId(int userId)
        {
            return _context.Devices
                .Where(d => d.UserId == userId)
                .ToList();
        }

        public IEnumerable<Device> GetAllDevices()
        {
            return _context.Devices.ToList();
        }
        public Device? GetDeviceById(int id)
        {
            return _context.Devices
                .FirstOrDefault(d => d.Id == id);
        }

        // Update a device
        public void UpdateDevice(Device device)
        {
            _context.Devices.Update(device);
            _context.SaveChanges();
        }

        public void DeleteDevice(Device device)
        {
            if (device == null)
            {
                _logger.LogWarning("In DeleteDevice method cannot insert new record in DB because user object is null");
                return;
            }

            _context.Devices.Remove(device);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
