using System.Collections.Generic;
using DeviceService.Entities;

namespace DeviceService.Repositories
{
    public interface IUserRepo
    {
<<<<<<< Updated upstream
        void CreateUser(User user);
=======
        User CreateUser(User user);
>>>>>>> Stashed changes
        User? GetUserById(int id);
        //void UpdateUser(User user);
        void DeleteUser(User user);
        bool SaveChanges();
    }
}
