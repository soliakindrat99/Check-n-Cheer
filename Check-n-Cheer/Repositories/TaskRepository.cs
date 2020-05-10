using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly CheckCheerContext _context;
        public TaskRepository(CheckCheerContext context)
        {
            _context = context;
        }

        public void AddTask(Task task)
        {
            _context.Add(task);
            _context.SaveChanges();
        }
        public Task GetTask(Guid id)
        {
            var task = _context.Tasks.Include(x => x.Options).Include(x => x.Test).FirstOrDefault(u => u.Id == id);
            return task;
        }
        public List<Task> GetTasks()
        {
            var tests = _context.Tasks.Include(x => x.Options).ToList();
            return tests;
        }
        public List<Task> GetTasks(Guid testId)
        {
            var test = _context.Tests.Include(x => x.Tasks)
                .ThenInclude(x => x.Options)
                .FirstOrDefault(x => x.Id == testId);
            return test.Tasks;
        }
        public void UpdateTask(Guid id, Task updatedTask)
        {
            var task = _context.Tasks.FirstOrDefault(u => u.Id == id);
            task.Name = updatedTask.Name;
            task.Mark = updatedTask.Mark;
            _context.SaveChanges();
        }
        public void RemoveTask(Guid id)
        {
            var task = _context.Tasks.FirstOrDefault(u => u.Id == id);
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }

    }
}
