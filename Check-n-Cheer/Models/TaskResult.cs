using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Models
{
    public class TaskResult
    {
        public Guid Id { get; set; }
        public TestResult TestResult { get; set; }
        public Task Task { get; set; }
        public List<OptionResult> OptionResults { get; set; }
        public double Percent { get; set; } = 0;
        public TaskResult() { }
        public TaskResult(TestResult testResult, Task task)
        {
            Id = Guid.NewGuid();
            TestResult = testResult;
            Task = task;
        }
    }
}
