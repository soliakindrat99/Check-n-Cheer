using System;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Interfaces
{
    public interface IUserRepository
    {
        User GetUser(string email);
        User GetUser(Guid id);
        void RegisterUser(User user);
        User[] GetUsers();
        void SetUserRole(Guid id, string role);
        void RemoveUser(Guid id);
    };
}
