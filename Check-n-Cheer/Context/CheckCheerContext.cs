using Microsoft.EntityFrameworkCore;

namespace Check_n_Cheer.Models
{
    public class CheckCheerContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public CheckCheerContext(DbContextOptions<CheckCheerContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
