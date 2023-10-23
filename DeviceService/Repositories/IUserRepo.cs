using System.Collections.Generic;
using DeviceService.Entities;

namespace DeviceService.Repositories
{
    public interface IUserRepo
    {
        User? CreateUser(User user);
        User? GetUserById(int id);
        //void UpdateUser(User user);
        void DeleteUser(int userId);
        bool SaveChanges();
    }
}
