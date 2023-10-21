using System.Linq;
using DeviceService.Entities;
using Microsoft.EntityFrameworkCore;
using UserService.Data;

namespace DeviceService.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepo> _logger;

        public UserRepo(AppDbContext context, ILogger<UserRepo> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void CreateUser(User user)
        {
            if (user == null)
            {
                
                _logger.LogWarning("In CreateUser method cannot insert new record in DB because user object is null");
                return;
            }

            _context.Users.Add(user);
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }


        public void DeleteUser(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("In DeleteUser method cannot insert new record in DB because user object is null");
                return;
            }

            _context.Users.Remove(user);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}