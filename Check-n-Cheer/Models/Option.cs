using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Models
{
    public class Option
    {
        public Guid Id { get; set; }
        public Task Task { get; set; }
        public string Name { get; set; }
        public bool IsCorrect { get; set; }
    }
}
