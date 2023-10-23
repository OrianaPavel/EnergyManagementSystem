using System.Collections.Generic;
using DeviceService.Entities;

namespace DeviceService.Repositories
{
    public interface IDeviceRepo
    {
        
        void CreateDevice(ref Device device);
        IEnumerable<Device> GetDevicesByUserId(int userId);
        IEnumerable<Device> GetAllDevices();
        Device? GetDeviceById(int id);
        void UpdateDevice(Device device);
        void DeleteDevice(Device device);
        bool SaveChanges();
    }
}
