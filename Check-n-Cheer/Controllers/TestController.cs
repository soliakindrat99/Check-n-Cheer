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
            //CreateTest_UserIsNotLogined_RedirectToError
            //CreateTest_UserIsLogined_DoesNotExist_RedirectToError
            //CreateTest_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError
            //CreateTest_UserIsLogined_Exist_UserIsTeacher_ReturnsView


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
            User user = null;
            if (Get("user") != null)
            {
                Guid id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(id);
            }
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
                ViewData["TestId"] = testId;
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

        [HttpPost]
        public ActionResult AddTask(Guid id, string condition,double mark)
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
                var test = _testRepo.GetTest(id);
                var task = new Task()
                {
                    Id = Guid.NewGuid(),
                    Name = condition,
                    Mark = mark,
                    TaskNumber = test.Tasks.Count + 1,
                    Test = test
                };
                _taskRepo.AddTask(task);
                test.Tasks.Add(task);
                _testRepo.UpdateTest(id, test);

                return RedirectToAction("ManageTasks", "Test", new { testId = id });
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
        public IActionResult SubmitTest(Guid testId)
        {
            _logger.LogInformation("GET Test/SubmitTest");
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
                ViewData["LoggedIn"] = "true";
                ViewData["TestId"] = testId;
                return View(test.Tasks);
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
                    tests = new List<Test>();
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

        [HttpPost]
        public ActionResult RemoveTask(Guid id)
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
                var task = _taskRepo.GetTask(id);
                if (task != null)
                {
                    _taskRepo.RemoveTask(id);
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

        [HttpPost]
        public ActionResult RemoveTest(Guid id)
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
                var test = _testRepo.GetTest(id);
                if (test != null)
                {
                    _testRepo.RemoveTest(id);
                    return RedirectToAction("TestHistory");
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
            
            if (user != null && user.Role == "Student")
            {
                var test = _testRepo.GetTest(testId);
                if (test != null)
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
            if(user != null && user.Role == "Student")
            {
                var testResult = _testResultRepo.GetTestResult(testResultId);
                if (testResult != null && testResult.Test != null)
                {
                    ViewData["TestName"] = testResult.Test.Name;
                    ViewData["TestResultId"] = testResult.Id;
                    ViewData["LoggedIn"] = "true";
                    return View(testResult.TaskResults);
                }
            }
            if (user != null)
            {
                _logger.LogInformation("User is not student!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult SaveAnswer(Guid id, List<Guid> answers)
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
            if (user != null && user.Role == "Student")
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
        public ActionResult TestResult(Guid testResultId)
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
            if (user != null && (user.Role == "Student" || user.Role == "Teacher"))
            {
                var testResult = _testResultRepo.GetTestResult(testResultId);
                if (testResult != null)
                {
                    var finalMark = 0.0;
                    var possibleMark = 0.0;
                    foreach (var result in testResult.TaskResults)
                    {
                        possibleMark += result.Task.Mark;
                        finalMark += result.Percent * result.Task.Mark;
                    }
                    ViewData["PossibleMark"] = possibleMark;
                    ViewData["FinalMark"] = finalMark;
                    ViewData["LoggedIn"] = "true";
                    return View(testResult.TaskResults);
                }
            }
            if (user != null)
            {
                _logger.LogInformation("User is not student or teacher!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpGet]
        public ActionResult Statistics()
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
            if (user != null && user.Role == "Teacher")
            {
                var tests = _testRepo.GetTests(user.Id);
                if (tests != null)
                {
                    var testResults = new List<TestResult>();
                    foreach (var test in tests)
                    {
                        var results = _testResultRepo.GetTestResultsForTest(test.Id);
                        testResults.AddRange(results);
                    }
                    ViewData["UserRole"] = "Teacher";
                    ViewData["LoggedIn"] = "true";
                    return View(testResults);
                }
            }
            if (user != null && user.Role == "Student")
            {
                var tests = _testRepo.GetTests(user.Id);
                if (tests != null)
                {
                    var testResults = _testResultRepo.GetTestResultsForStudent(user.Id);
                    ViewData["UserRole"] = "Student";
                    ViewData["LoggedIn"] = "true";
                    return View(testResults);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
            Guid id = Guid.Parse(Get("user"));
            User user = _userRepo.GetUser(id);
            if (user != null && user.Role == "Teacher")
            {
                ViewData["LoggedIn"] = "true";
                return View(option);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult CreateOption(Guid id, string optionName, string? optionIsCorrect)
        {
            _logger.LogInformation("POST Option/Create");
            bool correct = (optionIsCorrect != null);
            var task = _taskRepo.GetTask(id);
            var option = new Option()
            {
                Id = Guid.NewGuid(),
                IsCorrect = correct,
                Name = optionName,
                Task = task
            };
            _optionRepo.AddOption(option);
            return RedirectToAction("ManageOptions", new { taskId = id });

        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            _logger.LogInformation("GET Option/Delete");
            return View();
        }

        [HttpGet]
        public ActionResult ManageOptions(Guid taskId)
        {
            _logger.LogInformation("GET Option/ManageOptions");
            var options = _taskRepo.GetTask(taskId);
            ViewData["TestId"] = options.Test.Id;
            ViewData["TaskId"] = taskId;
            ViewData["LoggedIn"] = "true";
            return View(options.Options);
        }

        [HttpPost]
        public ActionResult ChangeOption(Guid id, string optionName, string? optionIsCorrect)
        {
            Option option = _optionRepo.GetOption(id);

            bool correct = (optionIsCorrect != null);
            if (option != null)
            {
                if (optionName != option.Name || option.IsCorrect != correct)
                {
                    option.Name = optionName;
                    option.IsCorrect = correct;
                    _optionRepo.UpdateOption(id, option);
                }
                return RedirectToAction("ManageOptions", new { taskId = option.Task.Id });
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult DeleteOption(Guid id)
        {
            Option option = _optionRepo.GetOption(id);


            if (option != null)
            {
                _optionRepo.RemoveOption(id);
                return RedirectToAction("ManageOptions", new { taskId = option.Task.Id });
            }
            return RedirectToAction("Error");
        }

        
    }
}
