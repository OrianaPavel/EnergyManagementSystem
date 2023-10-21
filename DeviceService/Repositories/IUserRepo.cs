using System.Collections.Generic;
using DeviceService.Entities;

namespace DeviceService.Repositories
{
    public interface IUserRepo
    {
        void CreateUser(User user);
        User? GetUserById(int id);
        //void UpdateUser(User user);
        void DeleteUser(User user);
        bool SaveChanges();
    }
}
