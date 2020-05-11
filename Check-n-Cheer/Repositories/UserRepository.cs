using System;
using System.Linq;
using Check_n_Cheer.Models;
using Check_n_Cheer.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Check_n_Cheer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CheckCheerContext _context;
        public UserRepository(CheckCheerContext context)
        {
            _context = context;
        }
        public void RegisterUser(User user)
        {
            user.SetPassword(user.Password);
            _context.Add(user);
            _context.SaveChanges();
        }
        public User GetUser(string email)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == email);
            return user;
        }
        public User GetUser(Guid id)
        {
            User user = _context.Users.SingleOrDefault(u => u.Id == id);
            return user;
        }
        public List<User> GetUsers()
        {
            var users =_context.Users.ToList();
            return users;
        }
        public void SetUserRole(Guid id, string role)
        {
            User user = _context.Users.SingleOrDefault(u => u.Id == id);
            user.Role = role;
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public void SetCurrentTest(Guid id, Guid testId)
        {
            User user = _context.Users.SingleOrDefault(u => u.Id == id);
            user.CurrentTestId = testId;
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public void RemoveUser(Guid id)
        {
            User user = new User { Id = id };
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
