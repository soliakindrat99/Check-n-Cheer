﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.DTO
{
    public class CreateTestDTO
    {
        public string Name { get; set; }
        public int TaskCount { get; set; }
        public Guid TeacherId { get; set; }
    }
}
