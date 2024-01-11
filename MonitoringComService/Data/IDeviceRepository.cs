using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonitoringComService.Entities;

namespace MonitoringComService.Data
{
    public interface IDeviceRepository
    {
        Task<Device?> CreateDeviceAsync(Device device);
        Task<Device?> GetDeviceByIdAsync(int deviceId);
        //Task<IEnumerable<Device>> GetAllDevicesAsync();
        Task UpdateDeviceAsync(Device device);
        Task DeleteDeviceAsync(int deviceId);
    }
}
