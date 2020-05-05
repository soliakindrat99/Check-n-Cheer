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
        public string Condition { get; set; }
        public int TaskNumber { get; set; }
        public double Mark { get; set; }
        public IEnumerable<Option> Options { get; set; }
    }
}
