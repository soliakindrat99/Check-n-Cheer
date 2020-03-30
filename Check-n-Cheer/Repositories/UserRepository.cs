using System.Linq;
using Check_n_Cheer.Models;
using Check_n_Cheer.Interfaces;

namespace Check_n_Cheer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context)
        {
            _context = context;
        }
        public User GetUser(string email)
        {
            User user = _context.Users.SingleOrDefault(e => e.Email == email);
            return user;
        }
        public User GetUser(int id)
        {
            User user = _context.Users.SingleOrDefault(e => e.Id == id);
            return user;
        }
        public void RegisterUser(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }
        public User[] GetUsers()
        {
            var users =_context.Users.ToList();
            return users.ToArray();
        }
    }
}
