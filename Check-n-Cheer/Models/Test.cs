using System;
using System.Collections.Generic;
using System.Linq;

namespace Check_n_Cheer.Models
{
    public class Test
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid TeacherId { get; set; }
        public List<Task> Tasks{ get; set; }

        private Test() { }
        public Test(Guid id, string name, Guid teacherId)
        {
            Id = id;
            Name = name;
            TeacherId = teacherId;
        }

        public void SetTask(List<Task> tasks)
        {
            Tasks = tasks;
        }
    }
}
