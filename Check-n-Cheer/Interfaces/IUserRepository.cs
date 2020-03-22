using Check_n_Cheer.Models;

namespace Check_n_Cheer.Interfaces
{
    public interface IUserRepository
    {
        User GetUser(string email);
        User GetUser(int id);
        void RegisterUser(User user);
    };
}
