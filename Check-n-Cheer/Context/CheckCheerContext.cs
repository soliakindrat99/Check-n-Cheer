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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .HasOne(p => p.Test)
                .WithMany(t => t.Tasks)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Option>()
                .HasOne(p => p.Task)
                .WithMany(t => t.Options)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
