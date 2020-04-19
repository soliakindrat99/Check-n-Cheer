using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.DTO
{
    public class CreateOptionDTO
    {
        public Guid TaskId { get; set; }
        public Guid TestId { get; set; }
        public string Name { get; set; }
        public bool IsCorrect { get; set; }
    }
}
