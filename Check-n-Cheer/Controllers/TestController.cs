using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Check_n_Cheer.Models;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.DTO;
using System.Collections.Generic;

namespace Check_n_Cheer.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private IUserRepository _userRepo;
        private ITestRepository _testRepo;
        private ITaskRepository _taskRepo;

        public TestController(ILogger<TestController> logger, IUserRepository repo, ITestRepository testRepo, ITaskRepository taskRepo)
        {
            _logger = logger;
            _userRepo = repo;
            _testRepo = testRepo;
            _taskRepo = taskRepo;
        }

        public string Get(string key)
        {
            if (Request == null)
                return null;
            return Request.Cookies[key];
        }
        public void Set(string key, string value, int expireTime = 60)
        {
            if (Response == null)
                return;
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMinutes(expireTime);
            Response.Cookies.Append(key, value, option);
        }
        public void Remove(string key)
        {
            Response.Cookies.Delete(key);
        }

        [HttpGet]
        public IActionResult CreateTest()
        {
            _logger.LogInformation("GET Test/CreateTest");
            User user = null;
            if (Get("user") != null)
            {
                Guid id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user != null && user.Role == "Teacher")
            {
                ViewData["LoggedIn"] = "true";
                ViewData["Id"] = user.Id;
                return View();
            }
            else if(user != null)
            {
                _logger.LogInformation("User is not Teacher!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public IActionResult CreateTest(CreateTestDTO newTest)
        {
            _logger.LogInformation("GET Test/CreateTest");
            Guid id = Guid.Parse(Get("user"));
            User user = _userRepo.GetUser(id);
            if (user == null || user.Role != "Teacher")
            {
                ViewData["LoggedIn"] = "false";
                return RedirectToAction("Error");
            }
            
            if(newTest.Name!="" && newTest.TaskCount>0)
            { 
                var test = new Test(Guid.NewGuid(), newTest.Name, newTest.TeacherId);
                test.Tasks = new List<Task>();
                for (int i = 0; i < newTest.TaskCount; i++)
                {
                    var task = new Task()
                    {
                        Id = Guid.NewGuid(),
                        TaskNumber = i + 1,
                        Test = test
                    };
                    test.Tasks.Add(task);
                }
                _testRepo.AddTest(test);
                return RedirectToAction("ManageTasks", new { testId = test.Id });
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        public IActionResult Open(Guid testId)
        {
            _logger.LogInformation("GET Test/Open");
            User user = null;
            if (Get("user") != null)
            {
                Guid id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user != null && user.Role == "Teacher")
            {
                ViewData["LoggedIn"] = "true";
                var test = _testRepo.GetTest(testId);
                return View(test);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpGet]
        public IActionResult ManageTasks(Guid testId)
        {
            _logger.LogInformation("GET Test/CreateTest");
            User user = null;
            if (Get("user") != null)
            {
                Guid id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user != null && user.Role == "Teacher")
            {
                var test = _testRepo.GetTest(testId);
                test.Tasks = test.Tasks.OrderBy(x => x.TaskNumber).ToList();
                ViewData["LoggedIn"] = "true";
                return View(test.Tasks);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public IActionResult RenameTask(string id,string condition)
        {
            _logger.LogInformation("POST Test/RenameTask");
            User user = null;
            if (Get("user") != null)
            {
                Guid user_id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(user_id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user != null && user.Role == "Teacher")
            {
                _taskRepo.RenameTask(Guid.Parse(id), condition);                
                var task= _taskRepo.GetTask(Guid.Parse(id));
                return RedirectToAction("ManageTasks", new { testId = task.Test.Id });
            }
            if (user != null)
            {
                _logger.LogInformation("User is not teacher!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpGet]
        public IActionResult TestHistory(string? id)
        {
            _logger.LogInformation("GET User/TestHistory");
            User user = null;
            if (Get("user") != null)
            {
                Guid user_id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(user_id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user != null && user.Role == "Teacher")
            {
                List<Test> tests;
                if (id == null)
                {
                    tests = _testRepo.GetTests(user.Id);
                }
                else
                {
                    tests =new List<Test> ();
                    // TODO: two tests with equal names different teachers
                    Test test = _testRepo.GetByName(id);
                    if (test != null && test.TeacherId == user.Id)
                    {
                        tests.Add(test);
                    }       
                }
                ViewData["LoggedIn"] = "true";
                return View(tests);
            }
            if (user != null)
            {
                _logger.LogInformation("User is not admin!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
