using System;
using System.Collections.Generic;
using System.Linq;
using Check_n_Cheer.Models;
using Microsoft.AspNetCore.Mvc;
using Check_n_Cheer.Interfaces;

namespace Check_n_Cheer.Controllers
{
    public class TaskController :Controller
    {
        private IUserRepository _repo;
        private ITestRepository _testRepo;
        private ITaskRepository _taskRepo;

        public TaskController(IUserRepository repo, ITestRepository testRepo, ITaskRepository taskRepo)
        {
            _repo = repo;
            _testRepo = testRepo;
            _taskRepo = taskRepo;
        }

        [HttpPost]
        public ActionResult UpdateTasks(List<Task> tasks)
        {
            foreach(var task in tasks)
            {
                _taskRepo.UpdateTask(task.Id, task);
            }
            var test = _testRepo.GetTests().FirstOrDefault(x => x.Tasks.Any(x => x.Id == tasks.First().Id));
            return RedirectToAction("Open", "Test", new { testId = test.Id });
        }

        [HttpGet]
        public ActionResult UpdateTask(Guid taskId, Guid testId, string condition)
        {
            var task = _taskRepo.GetTask(taskId);
            task.Condition = condition;
            _taskRepo.UpdateTask(taskId, task);
            return RedirectToAction("CreateTask", "Test", new { testId = testId });
        }

    }
}
