using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Check_n_Cheer.Models;
using Microsoft.AspNetCore.Mvc;
using Check_n_Cheer.Interfaces;
using Microsoft.Extensions.Logging;

namespace Check_n_Cheer.Controllers
{
    public class TaskController :Controller
    {
        private readonly ILogger<TestController> _logger;
        private IUserRepository _repo;
        private ITestRepository _testRepo;
        private ITaskRepository _taskRepo;

        public TaskController(ILogger<TestController> logger, IUserRepository repo, ITestRepository testRepo, ITaskRepository taskRepo)
        {
            _logger = logger;
            _repo = repo;
            _testRepo = testRepo;
            _taskRepo = taskRepo;
        }

        [HttpPost]
        public ActionResult UpdateTasks(List<Task> tasks)
        {
            _logger.LogInformation("POST Task/UpdateTasks");
            foreach (var task in tasks)
            {
                _taskRepo.UpdateTask(task.Id, task);
            }
            var test = _testRepo.GetTests().FirstOrDefault(x => x.Tasks.Any(x => x.Id == tasks.First().Id));
            return RedirectToAction("Open", "Test", new { testId = test.Id });
        }

        [HttpGet]
        public ActionResult UpdateTask(Guid taskId, Guid testId, string condition)
        {
            _logger.LogInformation("GET Task/UpdateTasks");
            var task = _taskRepo.GetTask(taskId);
            task.Condition = condition;
            _taskRepo.UpdateTask(taskId, task);
            return RedirectToAction("ManageTask", "Test", new { testId });
        }

        [HttpGet]
        public ActionResult AddTask(Guid testId)
        {
            var test = _testRepo.GetTest(testId);
            var task = new Task()
            {
                Id = Guid.NewGuid(),
                TaskNumber = test.Tasks.Count + 1,
                Test = test
            };
            _taskRepo.AddTask(task);
            test.Tasks.Add(task);
            _testRepo.UpdateTest(testId, test);

            return RedirectToAction("ManageTasks", "Test", new { testId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
