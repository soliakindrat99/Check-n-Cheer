using System;
using System.Collections.Generic;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Interfaces
{
    public interface IUserRepository
    {
        void RegisterUser(User user);
        User GetUser(string email);
        User GetUser(Guid id);
        List<User> GetUsers();
        void SetUserRole(Guid id, string role);
        void SetCurrentTest(Guid id, Guid testId);
        void RemoveUser(Guid id);
    };
}
