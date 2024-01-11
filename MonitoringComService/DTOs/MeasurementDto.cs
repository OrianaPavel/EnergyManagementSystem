using System.ComponentModel.DataAnnotations;

namespace MonitoringComService.DTOs
{
    public class MeasurementDto
    {

        [Required]
        public long Timestamp { get; set; }
        [Required]
        public int MeasurementValue { get; set; }

        [Required]
        public string DeviceId { get; set; } = String.Empty;
    }
}