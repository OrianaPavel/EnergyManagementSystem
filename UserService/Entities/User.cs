using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Entities
{
    [Table("dm_user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  

        [Required]
        [Column("username", TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("password", TypeName = "varchar(100)")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Column("role")]
        public Role UserRole { get; set; }
    }

}