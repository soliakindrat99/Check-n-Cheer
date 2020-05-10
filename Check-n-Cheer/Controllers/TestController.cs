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
        private IOptionRepository _optionRepo;
        private ITaskResultRepository _taskResultRepo;
        private ITestResultRepository _testResultRepo;
        private IOptionResultRepository _optionResultRepo;

        public TestController(ILogger<TestController> logger,
            IUserRepository repo,
            ITestRepository testRepo,
            ITaskRepository taskRepo,
            IOptionRepository optionRepo,
            ITestResultRepository testResultRepo,
            ITaskResultRepository taskResultRepo,
            IOptionResultRepository optionResultRepo)
        {
            _logger = logger;
            _userRepo = repo;
            _testRepo = testRepo;
            _taskRepo = taskRepo;
            _testResultRepo = testResultRepo;
            _taskResultRepo = taskResultRepo;
            _optionRepo = optionRepo;
            _optionResultRepo = optionResultRepo;

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
            
            if(newTest.Name!="")
            {
                var teacher = _userRepo.GetUser(newTest.TeacherId);
                var test = new Test(Guid.NewGuid(), newTest.Name, teacher);
                test.Tasks = new List<Task>();
             
                _testRepo.AddTest(test);
                return RedirectToAction("ManageTasks", new { testId = test.Id });
            }
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
                ViewData["TestID"] = testId;
                return View(test.Tasks);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public IActionResult SaveTask(string id,string condition,double mark)
        {
            _logger.LogInformation("POST Test/SaveTask");
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
                
                var task= _taskRepo.GetTask(Guid.Parse(id));
                if (task != null) 
                {
                    task.Name = condition;
                    task.Mark = mark;
                    _taskRepo.UpdateTask(Guid.Parse(id), task);
                    return RedirectToAction("ManageTasks", new { testId = task.Test.Id });
                }
                
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
            ViewData["UserRole"] = user.Role;
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
                    if (test != null && test.Teacher.Id == user.Id)
                    {
                        tests.Add(test);
                    }       
                }
                ViewData["LoggedIn"] = "true";
                return View(tests);
            }
            if (user != null && user.Role == "Student")
            {
                List<Test> tests;
                if (id == null)
                {
                    tests = _testRepo.GetTests();
                }
                else
                {
                    tests = new List<Test>();
                    // TODO: two tests with equal names different teachers
                    Test test = _testRepo.GetByName(id);
                    if (test != null)
                    {
                        tests.Add(test);
                    }
                }
                ViewData["LoggedIn"] = "true";
                return View(tests);
            }
            if (user != null)
            {
                _logger.LogInformation("User is not teacher or student!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult AddTask(Guid id, string condition,double mark)
        {
            var test = _testRepo.GetTest(id);
            var task = new Task()
            {
                Id = Guid.NewGuid(),
                Name = condition,
                Mark=mark,
                TaskNumber = test.Tasks.Count + 1,
                Test = test
            };
            _taskRepo.AddTask(task);
            test.Tasks.Add(task);
            _testRepo.UpdateTest(id, test);

            return RedirectToAction("ManageTasks", "Test", new {testId = id });
        }

        [HttpPost]
        public ActionResult RemoveTask(Guid id)
        {
            var task = _taskRepo.GetTask(id);
            if(task != null)
            {
                _taskRepo.RemoveTask(id);
                return RedirectToAction("ManageTasks", new { testId = task.Test.Id });
            }
            return RedirectToAction( "Error" );
        }

        [HttpPost]
        public ActionResult RemoveTest(Guid id)
        {
            var test = _testRepo.GetTest(id);
            if (test != null)
            {
                _testRepo.RemoveTest(id);
                return RedirectToAction("TestHistory");
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        public ActionResult PassTest(Guid testId)
        {
            _logger.LogInformation("GET Test/PassTest");
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
            var test = _testRepo.GetTest(testId);
            if (test != null && user != null && user.Role == "Student")
            {
                var testResult = new TestResult(user, test);
                _testResultRepo.AddTestResult(testResult);
                foreach (var task in test.Tasks)
                {
                    var taskResult = new TaskResult(testResult, task);
                    _taskResultRepo.AddTaskResult(taskResult);
                    task.Options.ForEach(x => _optionResultRepo.AddOptionResult(new OptionResult(taskResult, x, false)));
                }
                _userRepo.SetCurrentTest(user.Id, testResult.Id);
                return RedirectToAction("CurrentTest", new { testResultId = testResult.Id });
            }
            if (user != null)
            {
                _logger.LogInformation("User is not student!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        public ActionResult CurrentTest(Guid testResultId)
        {
            var testResult = _testResultRepo.GetTestResult(testResultId);
            if (testResult != null && testResult.Test != null)
            {
                ViewData["TestName"] = testResult.Test.Name;
                ViewData["TestResultId"] = testResult.Id;
                return View(testResult.TaskResults);
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult SaveAnswer(Guid id, List<Guid> answers)
        {

            var taskResult = _taskResultRepo.GetTaskResult(id);
            if (taskResult != null)
            {
                var totalCount = taskResult.Task.Options.Count;
                var goalCount = taskResult.Task.Options.Count(x => x.IsCorrect);
                var actualTotalCount = answers.Count;
                double penaltyMultiplier = 1.0;
                if(actualTotalCount == totalCount)
                {
                    penaltyMultiplier = 0;
                }
                if(actualTotalCount > goalCount)
                {
                    penaltyMultiplier = 0.5;
                }
                var actualCount = answers.Count(x => _optionRepo.GetOption(x).IsCorrect);
                var result = ((double)actualCount / (double)goalCount)*penaltyMultiplier;
                taskResult.Percent = result;
                _taskResultRepo.UpdateTaskResult(taskResult.Id, taskResult);
                foreach (var option in taskResult.Task.Options)
                {
                    var optionResult = _optionResultRepo.GetOptionResult(taskResult.Id, option.Id);
                    if(optionResult != null)
                    {
                        optionResult.IsChecked = answers.Exists(x => x == option.Id);
                        _optionResultRepo.UpdateOptionResult(optionResult.Id, optionResult);
                    } 
                    else
                    {
                        optionResult = new OptionResult(taskResult, option, answers.Exists(x => x == option.Id));
                        _optionResultRepo.AddOptionResult(optionResult);
                    }
                }
                return RedirectToAction("CurrentTest", new { testResultId = taskResult.TestResult.Id });
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        public ActionResult CompleteTest(Guid testResultId)
        {
            _logger.LogInformation(Convert.ToString(testResultId));
            var testResult = _testResultRepo.GetTestResult(testResultId);
            if (testResult != null)
            {
                var finalMark = 0.0;
                foreach (var result in testResult.TaskResults)
                {
                    finalMark += result.Percent * result.Task.Mark;
                }
                ViewData["FinalMark"] = finalMark;
                return View(testResult.TaskResults);
            }
            return RedirectToAction("Error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
