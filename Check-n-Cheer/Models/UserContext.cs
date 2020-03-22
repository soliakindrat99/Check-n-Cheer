using Microsoft.EntityFrameworkCore;

namespace Check_n_Cheer.Models
{
    public class UserContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
