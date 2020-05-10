using System;
using Microsoft.EntityFrameworkCore;

namespace Check_n_Cheer.Models
{
    public class CheckCheerContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<TaskResult> TaskResults { get; set; }
        public DbSet<OptionResult> OptionResults { get; set; }
        public CheckCheerContext(DbContextOptions<CheckCheerContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
            new User[]
            {
                new User { Id=Guid.NewGuid(), Email="admin@checkandcheer.com", Password = Crypto.Hash("admin"), Role="Admin"},
                new User { Id=Guid.NewGuid(), Email="teacher@checkandcheer.com", Password = Crypto.Hash("teacher"), Role="Teacher"},
                new User { Id=Guid.NewGuid(), Email="student@checkandcheer.com", Password = Crypto.Hash("student"), Role="Student"}
            });

            modelBuilder.Entity<Test>()
                .HasOne(p => p.Teacher)
                .WithMany(t => t.Tests)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Task>()
                .HasOne(p => p.Test)
                .WithMany(t => t.Tasks)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Option>()
                .HasOne(p => p.Task)
                .WithMany(t => t.Options)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TestResult>()
                .HasOne(p => p.Student)
                .WithMany(t => t.TestResults)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaskResult>()
                .HasOne(p => p.TestResult)
                .WithMany(t => t.TaskResults)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OptionResult>()
                .HasOne(p => p.TaskResult)
                .WithMany(t => t.OptionResults)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
