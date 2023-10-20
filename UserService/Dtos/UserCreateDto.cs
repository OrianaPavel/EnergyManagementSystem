using System.ComponentModel.DataAnnotations;
using UserService.Entities;

namespace UserService.Dtos
{
    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public Role UserRole { get; set; } = Role.User;
    }
}