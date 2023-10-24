using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DeviceService.Entities;

namespace DeviceService.Entities
{
    [Table("dm_device")]
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("description", TypeName = "varchar(255)")]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("address", TypeName = "varchar(255)")]
        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Column("max_hourly_energy_consumption", TypeName = "int")]
        public double MaximumHourlyEnergyConsumption { get; set; }

        [Required]
        [ForeignKey("User")]
        //[Column("user_id")]
        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
