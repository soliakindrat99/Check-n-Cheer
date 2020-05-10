using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Models
{
    public class Task
    { 
        public Guid Id { get; set; }
        public Test Test { get; set; }
        public string Name { get; set; }
        public int TaskNumber { get; set; }
        public double Mark { get; set; }
        public List<Option> Options { get; set; }
        public List<TaskResult> Results { get; set; }
    }
}
