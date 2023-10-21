using System.Collections.Generic;
using DeviceService.Entities;

namespace DeviceService.Repositories
{
    public interface IDeviceRepo
    {
        
        void CreateDevice(Device device);
        IEnumerable<Device> GetDevicesByUserId(int userId);
        Device? GetDeviceById(int id);
        void UpdateDevice(Device device);
        void DeleteDevice(Device device);
        bool SaveChanges();
    }
}
