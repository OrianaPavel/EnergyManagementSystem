using UserService.Entities;

namespace UserService.Repositories
{
    public interface IUserRepo
    {
        bool SaveChanges();
        public void AddUser(ref User user);
        public User? GetUserByUsername(string username);
        public User? GetUserById(int userId);
        public IEnumerable<User> GetAllUsers();
        public void UpdateUser(User user);
        public void DeleteUser(User user);

    }
}