using System;
using System.Collections.Generic;
using System.Linq;

namespace Check_n_Cheer.Models
{
    public class Test
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TeacherName { get; set; }
        public List<Task> Tasks{ get; set; }

        private Test() { }
        public Test(Guid id, string name, string teacherName)
        {
            Id = id;
            Name = name;
            TeacherName = teacherName;
        }

        public void SetTask(List<Task> tasks)
        {
            Tasks = tasks;
        }
    }
}
