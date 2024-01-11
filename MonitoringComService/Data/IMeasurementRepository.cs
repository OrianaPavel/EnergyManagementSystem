using System.Collections.Generic;
using System.Threading.Tasks;
using MonitoringComService.Entities;

namespace MonitoringComService.Data
{
    public interface IMeasurementRepository
    {
        Task<Measurement?> CreateMeasurementAsync(Measurement measurement);
        Task<Measurement?> GetMeasurementByIdAsync(int measurementId);
        Task<IEnumerable<Measurement>> GetMeasurementsByDeviceIdAsync(int deviceId);
        Task<IEnumerable<Measurement>> GetMeasurementsByDeviceIdAndDateAsync(int deviceId, DateTime date);
        Task UpdateMeasurementAsync(Measurement measurement);
        Task DeleteMeasurementAsync(int measurementId);
        Task<int> GetTotalEnergyAndMaxByHour(long unixTimeSeconds, int deviceId);
    }
}
