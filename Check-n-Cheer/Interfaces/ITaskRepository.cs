using System;
using System.Collections.Generic;
using System.Linq;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Interfaces
{
    public interface ITaskRepository
    {
        Task GetTask(Guid id);
        List<Task> GetTasks();
        void UpdateTask(Guid id, Task updatedTask);
        void RemoveTask(Guid id);
        void AddTask(Task task);

        void RenameTask(Guid id, string condition);
    }
}
