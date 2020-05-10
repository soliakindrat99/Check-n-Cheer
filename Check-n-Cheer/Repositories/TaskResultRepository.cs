using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Repositories
{
    public class TaskResultRepository: ITaskResultRepository
    {
        private readonly CheckCheerContext _context;
        public TaskResultRepository(CheckCheerContext context)
        {
            _context = context;
        }
        public void AddTaskResult(TaskResult result)
        {
            _context.Add(result);
            _context.SaveChanges();
        }
        public TaskResult GetTaskResult(Guid id)
        {
            var result = _context.TaskResults
                .Include(x => x.TestResult)
                .Include(x => x.Task).ThenInclude(x => x.Options)
                .Include(x => x.OptionResults).ThenInclude(x => x.Option)
                .FirstOrDefault(x => x.Id == id);
            return result;
        }
        public List<TaskResult> GetTaskResults() 
        {
            var taskResults = _context.TaskResults.ToList();
            return taskResults;
        }
        public List<TaskResult> GetTaskResults(Guid testResultId) 
        {
            var taskResults = _context.TaskResults
                .Include(x => x.Task).ThenInclude(x => x.Test)
                .Where(x => x.Task.Test.Id == testResultId)
                .ToList();
            return taskResults;
        }
        public void UpdateTaskResult(Guid id, TaskResult updatedResult)
        {
            var result = _context.TaskResults.FirstOrDefault(u => u.Id == id);
            result.Percent = updatedResult.Percent;
            _context.SaveChanges();
        }
        public void RemoveTaskResult(Guid id)
        {
            var result = _context.TaskResults.FirstOrDefault(u => u.Id == id);
            _context.TaskResults.Remove(result);
            _context.SaveChanges();
        }
    }
}
