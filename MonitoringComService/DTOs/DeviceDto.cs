using System.ComponentModel.DataAnnotations;

namespace MonitoringComService.DTOs
{
    public class DeviceDto
    {
        [Required]
        public string DeviceId { get; set; } = String.Empty;
        [Required]
        public string UserId { get; set; } = String.Empty;
        [Required]
        public int MaxHourlyConsumption { get; set; }
    }
}