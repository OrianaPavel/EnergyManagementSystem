using System.ComponentModel.DataAnnotations;

namespace UserDevice.Dtos
{
    public class DeviceCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = String.Empty;

        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = String.Empty;

        [Required]
        public int MaximumHourlyEnergyConsumption { get; set; }

        [Required]
        public string UserId { get; set; } = String.Empty;
    }
}
