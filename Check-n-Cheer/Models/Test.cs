using System;
using System.Collections.Generic;
using System.Linq;

namespace Check_n_Cheer.Models
{
    public class Test
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public User Teacher { get; set; }
        public List<Task> Tasks{ get; set; }
        public List<TestResult> Results { get; set; }
        public Test() { }
        public Test(Guid id, string name, User teacher)
        {
            Id = id;
            Name = name;
            Teacher = teacher;
        }

        public void SetTask(List<Task> tasks)
        {
            Tasks = tasks;
        }
    }
}
