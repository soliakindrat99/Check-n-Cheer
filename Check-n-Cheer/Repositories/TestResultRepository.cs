using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Repositories
{
    public class TestResultRepository : ITestResultRepository
    {
        private readonly CheckCheerContext _context;
        public TestResultRepository(CheckCheerContext context)
        {
            _context = context;
        }
        public void AddTestResult(TestResult result)
        {
            _context.Add(result);
            _context.SaveChanges();
        }
        public TestResult GetTestResult(Guid id)
        {
            var result = _context.TestResults
                .Include(x => x.Student)
                .Include(x => x.Test)
                .Include(x => x.TaskResults).ThenInclude(x => x.Task)
                .Include(x => x.TaskResults).ThenInclude(x => x.OptionResults).ThenInclude(x => x.Option)
                .FirstOrDefault(x => x.Id == id);
            return result;
        }
        public List<TestResult> GetTestResults() 
        {
            var tests = _context.TestResults
                .Include(x => x.Student)
                .Include(x => x.Test).ThenInclude(x => x.Tasks)
                .Include(x => x.TaskResults).ThenInclude(x => x.Task)
                .ToList();
            return tests;
        }
        public List<TestResult> GetTestResultsForStudent(Guid studentId)
        {
            var tests = _context.TestResults
                .Include(x => x.Student)
                .Include(x => x.Test).ThenInclude(x => x.Tasks)
                .Include(x => x.TaskResults).ThenInclude(x => x.Task)
                .Where(x => x.Student.Id == studentId)
                .ToList();
            return tests;
        }
        public List<TestResult> GetTestResultsForTest(Guid testId)
        {
            var tests = _context.TestResults
                .Include(x => x.Student)
                .Include(x => x.Test).ThenInclude(x => x.Tasks)
                .Include(x => x.TaskResults).ThenInclude(x => x.Task)
                .Where(x => x.Test.Id == testId)
                .ToList();
            return tests;
        }
        public void RemoveTestResult(Guid id)
        {
            var result = _context.TestResults.FirstOrDefault(x => x.Id == id);
            _context.TestResults.Remove(result);
            _context.SaveChanges();
        }
    }
}
