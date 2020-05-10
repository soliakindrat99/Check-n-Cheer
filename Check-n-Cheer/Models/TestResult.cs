using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Models
{
    public class TestResult
    {
        public Guid Id { get; set; }
        public User Student { get; set; }
        public Test Test { get; set; }
        public List<TaskResult> TaskResults { get; set; }
        public TestResult() { }
        public TestResult(User student, Test test)
        {
            Id = Guid.NewGuid();
            Student = student;
            Test = test;
        }
    }
}
