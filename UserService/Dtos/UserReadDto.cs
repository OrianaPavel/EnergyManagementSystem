using UserService.Entities;

namespace UserService.Dtos
{
    public class UserReadDto
    {
        public int Id { get; set; }  

        public string Hashid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public Role UserRole { get; set; }
    }
}