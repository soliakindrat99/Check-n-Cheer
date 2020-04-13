using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;
using Check_n_Cheer.DTO;
using Microsoft.Extensions.Logging;

namespace Check_n_Cheer.Controllers
{
    public class OptionController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private IUserRepository _userRepository;
        private ITaskRepository _taskRepository;
        private IOptionRepository _optionRepository;
        public OptionController(ILogger<TestController> logger,IUserRepository userRepository, ITaskRepository taskRepository, IOptionRepository optionRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _optionRepository = optionRepository;
        }
        public string Get(string key)
        {
            if (Request == null)
                return null;
            return Request.Cookies[key];
        }
        public ActionResult Index()
        {
            _logger.LogInformation("GET Option/Index");
            return View();
        }

        [HttpGet]
        public ActionResult Create(Guid taskId, Guid testId)
        {
            _logger.LogInformation("GET Option/Create");
            var option = new CreateOptionDTO { TaskId = taskId, TestId = testId };
            int id = int.Parse(Get("user"));
            User user = _userRepository.GetUser(id);
            if (user != null && user.Role == "Teacher")
            {
                ViewData["LoggedIn"] = "true";
                return View(option);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        // POST: Option/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateOptionDTO newOption)
        {
            _logger.LogInformation("POST Option/Create");
            try
            {
                var task = _taskRepository.GetTask(newOption.TaskId);
                var option = new Option()
                {
                    Id = Guid.NewGuid(),
                    IsCorrect = newOption.IsCorrect,
                    Name = newOption.Name,
                    Task = task
                };
                _optionRepository.AddOption(option);
                return RedirectToAction("CreateTask", "Test", new { testId = newOption.TestId});
            }
            catch
            {
                return View();
            }
        }

        // GET: Option/Delete/5
        public ActionResult Delete(Guid id)
        {
            _logger.LogInformation("GET Option/Delete");
            return View();
        }

        [HttpGet]
        public ActionResult GetAllForTask(Guid taskId)
        {
            _logger.LogInformation("GET Option/GetAllForTask");
            var options = _optionRepository.GetOptions().Where(x => x.Task.Id == taskId);
            return View(options);
        }


    }
}