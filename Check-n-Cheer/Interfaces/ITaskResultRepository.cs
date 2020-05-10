using System;
using System.Collections.Generic;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Interfaces
{
    public interface ITaskResultRepository
    {
        void AddTaskResult(TaskResult result);
        TaskResult GetTaskResult(Guid id);
        List<TaskResult> GetTaskResults();
        List<TaskResult> GetTaskResults(Guid testResultId);
        void UpdateTaskResult(Guid id, TaskResult updatedResult);
        void RemoveTaskResult(Guid id);

        
    }
}
