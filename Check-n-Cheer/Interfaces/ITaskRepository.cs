using System;
using System.Collections.Generic;
using System.Linq;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Interfaces
{
    public interface ITaskRepository
    {
        void AddTask(Task task);
        Task GetTask(Guid id);
        List<Task> GetTasks();
        List<Task> GetTasks(Guid testId);
        void UpdateTask(Guid id, Task updatedTask);
        void RemoveTask(Guid id);
    }
}
