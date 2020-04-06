using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly CheckCheerContext _context;
        public TestRepository(CheckCheerContext context)
        {
            _context = context;
        }
        
        public Test GetTest(Guid id)
        {
            var test = _context.Tests.Include(x => x.Tasks).ThenInclude(x => x.Options).FirstOrDefault(u => u.Id == id);
            return test;
        }

        public Test GetByName(string name)
        {
            var test = _context.Tests.FirstOrDefault(u => u.Name == name);
            return test;
        }

        public List<Test> GetTests()
        {
            var tests = _context.Tests.Include(x => x.Tasks).ThenInclude(x => x.Options).ToList();
            return tests;
        }

        public void UpdateTest(Guid id, Test updatedTest)
        {
            var test = _context.Tests.FirstOrDefault(u => u.Id == id);
            test.Name = updatedTest.Name;
            test.TeacherName = updatedTest.TeacherName;
            test.Tasks = updatedTest.Tasks;
            _context.SaveChanges();
        }

        public void RemoveTest(Guid id)
        {
            var test = _context.Tests.FirstOrDefault(u => u.Id == id);
            _context.Tests.Remove(test);
            _context.SaveChanges();
        }
        public void AddTest(Test test)
        {
            _context.Add(test);
            _context.SaveChanges();
        }
    }
}
