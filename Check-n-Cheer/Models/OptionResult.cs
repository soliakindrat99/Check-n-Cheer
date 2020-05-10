using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Models
{
    public class OptionResult
    {
        public Guid Id { get; set; }
        public Option Option { get; set; }
        public TaskResult TaskResult { get; set; }
        public bool IsChecked { get; set; }
        public OptionResult() { }
        public OptionResult(TaskResult taskResult, Option option, bool isChecked)
        {
            Id = Guid.NewGuid();
            TaskResult = taskResult;
            Option = option;
            IsChecked = isChecked;
        }
    }
}
