using System;
using Xunit;
using Moq;
using Check_n_Cheer.Controllers;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;
using Check_n_Cheer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;

namespace Check_n_Cheer_XUnitTestProject
{
    public class TestControllerTests
    {
        
        
        private readonly TestController _controller;

        private readonly ILogger<TestController> _logger;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITestRepository> _mockTestRepo;
        private readonly Mock<ITaskRepository> _mockTaskRepo;
        private readonly Mock<IOptionRepository> _mockOptionRepo;
        private readonly Mock<ITaskResultRepository> _mockTaskResultRepo;
        private readonly Mock<ITestResultRepository> _mockTestResultRepo;
        private readonly Mock<IOptionResultRepository> _mockOptionResultRepo;

        public TestControllerTests()
        {
            _logger = Mock.Of<ILogger<TestController>>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockTestRepo = new Mock<ITestRepository>();
            _mockTaskRepo = new Mock<ITaskRepository>();
            _mockOptionRepo = new Mock<IOptionRepository>();
            _mockTaskResultRepo = new Mock<ITaskResultRepository>();
            _mockTestResultRepo = new Mock<ITestResultRepository>();
            _mockOptionResultRepo = new Mock<IOptionResultRepository>();

            _controller = new TestController(_logger, _mockUserRepo.Object, _mockTestRepo.Object, _mockTaskRepo.Object,
                _mockOptionRepo.Object,
                _mockTestResultRepo.Object,
                 _mockTaskResultRepo.Object,
                _mockOptionResultRepo.Object);
            }
        private string GetCookieValueFromResponse(HttpResponse response, string cookieName)
        {
            foreach (var headers in response.Headers.Values)
                foreach (var header in headers)
                    if (header.StartsWith($"{cookieName}="))
                    {
                        var p1 = header.IndexOf('=');
                        var p2 = header.IndexOf(';');
                        return header.Substring(p1 + 1, p2 - p1 - 1);
                    }
            return null;
        }
        //CreateTest_UserIsNotLogined_RedirectToError
        //CreateTest_UserIsLogined_DoesNotExist_RedirectToError
        //CreateTest_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError
        //CreateTest_UserIsLogined_Exist_UserIsTeacher_ReturnsView
        [Fact]
        public void Get_CreateTest_UserIsNotLogined_RedirectToError() 
        {
            var result = _controller.CreateTest();
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Get_CreateTest_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var result = _controller.CreateTest();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Get_CreateTest_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var result = _controller.CreateTest();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Get_CreateTest_UserIsTeacher_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            var result = _controller.CreateTest();
            var redirect = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Post_CreateTest_UserIsNotLogined_RedirectToError()
        {
            var test = new CreateTestDTO
            {
                Name = "Tets",
                TaskCount = 0,
                TeacherId = Guid.NewGuid()
            };
            var result = _controller.CreateTest(test);
            Assert.NotNull(result);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_CreateTest_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var test = new CreateTestDTO
            {
                Name = "Tets",
                TaskCount = 0,
                TeacherId = Guid.NewGuid()
            };
            var result = _controller.CreateTest(test);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_CreateTest_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var test = new CreateTestDTO
            {
                Name = "Tets",
                TaskCount = 0,
                TeacherId = Guid.NewGuid()
            };
            var result = _controller.CreateTest(test);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_CreateTest_UserIsTeacher_TestNameIsEmpty_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            var test = new CreateTestDTO
            {
                Name = "",
                TaskCount = 0,
                TeacherId = Guid.NewGuid()
            };
            var result = _controller.CreateTest(test);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_CreateTest_UserIsTeacher_TestNameIsValid_RedirectToManageTasks()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            var test = new CreateTestDTO
            {
                Name = "sks",
                TaskCount = 0,
                TeacherId = id
            };
            var result = _controller.CreateTest(test);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageTasks", redirect.ActionName);
        }

        [Fact]
        public void ManageTasks_UserIsNotLogined_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var result = _controller.ManageTasks(id);
            Assert.NotNull(result);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void ManageTasks_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            Guid testId = Guid.NewGuid();
            var result = _controller.ManageTasks(testId);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void ManageTasks_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            Guid testId = Guid.NewGuid();
            var result = _controller.ManageTasks(testId);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void ManageTasks_UserIsTeacher_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            Guid testId = Guid.NewGuid();
            _mockTestRepo.Setup(repo => repo.GetTest(testId))
                .Returns(new Test()
                { 
                    Id = testId,
                    Name = "sks",
                    Teacher = null,
                    Tasks = new List<Task> 
                    { 
                        new Task()
                        {
                            Id = Guid.NewGuid(),
                            Test = null,
                            Name = "sts",
                            TaskNumber = 2,
                            Mark = 1,
                            Options = null,
                            Results = null

                        }
                    },
                    Results = null

                });
            var result = _controller.ManageTasks(testId);
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Task>>(view.Model);
            Assert.Single(model);
        }

        [Fact]
        public void Post_SaveTask_UserIsNotLogined_RedirectToError()
        {
            var result = _controller.SaveTask(Guid.NewGuid().ToString(), "cond", 0.0);
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Post_SaveTask_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var result = _controller.SaveTask(Guid.NewGuid().ToString(), "cond", 0.0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_SaveTask_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var result = _controller.SaveTask(Guid.NewGuid().ToString(), "cond", 0.0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_SaveTask_UserIsTeacher_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            Guid taskId = Guid.NewGuid();
            _mockTaskRepo.Setup(repo => repo.GetTask(taskId))
              .Returns(new Task()
              {
                  Id = taskId,
                  Test = new Test()
                  {
                      Id = Guid.NewGuid(),
                      Name = "sks",
                      Teacher = null,
                      Tasks = null,
                      Results = null

                  },
                  Name = "sts",
                  TaskNumber = 2,
                  Mark = 1,
                  Options = null,
                  Results = null

              });

            var result = _controller.SaveTask(taskId.ToString(), "cond", 0.0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageTasks", redirect.ActionName);
        }

        [Fact]
        public void Post_AddTask_UserIsNotLogined_RedirectToError()
        {
            var result = _controller.AddTask(Guid.NewGuid(), "cond", 0.0);
            Assert.NotNull(result);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_AddTask_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var result = _controller.AddTask(Guid.NewGuid(), "cond", 0.0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_AddTask_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var result = _controller.AddTask(Guid.NewGuid(), "cond", 0.0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_AddTask_UserIsTeacher_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            Guid testId = Guid.NewGuid();
            _mockTestRepo.Setup(repo => repo.GetTest(testId))
              .Returns(new Test()
              {
                  Id = testId,
                  Name = "sks",
                  Teacher = null,
                  Tasks = new List<Task>(),
                  Results = null

              });

            var result = _controller.AddTask(testId, "cond", 0.0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageTasks", redirect.ActionName);
        }

        [Fact]
        public void SubmitTest_UserIsNotLogined_RedirectToError()
        {
            var result = _controller.SubmitTest(Guid.NewGuid());
            Assert.NotNull(result);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void SubmitTest_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var result = _controller.SubmitTest(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void SubmitTest_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var result = _controller.SubmitTest(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void SubmitTest_UserIsTeacher_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            Guid testId = Guid.NewGuid();
            _mockTestRepo.Setup(repo => repo.GetTest(testId))
              .Returns(new Test()
              {
                  Id = testId,
                  Name = "sks",
                  Teacher = null,
                  Tasks = new List<Task>(),
                  Results = null

              });

            var result = _controller.SubmitTest(testId);
            var redirect = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Post_RemoveTest_UserIsNotLogined_RedirectToError()
        {
            var result = _controller.RemoveTest(Guid.NewGuid());
            Assert.NotNull(result);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_RemoveTest_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var result = _controller.RemoveTest(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_RemoveTest_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var result = _controller.RemoveTest(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_RemoveTest_UserIsTeacher_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            Guid testId = Guid.NewGuid();
            _mockTestRepo.Setup(repo => repo.GetTest(testId))
              .Returns(new Test()
              {
                  Id = testId,
                  Name = "sks",
                  Teacher = null,
                  Tasks = new List<Task>(),
                  Results = null

              });

            var result = _controller.RemoveTest(testId);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("TestHistory", redirect.ActionName);
        }

        [Fact]
        public void Post_RemoveTask_UserIsNotLogined_RedirectToError()
        {
            var result = _controller.RemoveTask(Guid.NewGuid());
            Assert.NotNull(result);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_RemoveTask_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var result = _controller.RemoveTask(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_RemoveTask_UserIsLogined_Exist_UserIsNotTeacher_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var result = _controller.RemoveTask(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void Post_RemoveTask_UserIsTeacher_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Teacher"
              });

            Guid taskId = Guid.NewGuid();
            _mockTaskRepo.Setup(repo => repo.GetTask(taskId))
              .Returns(new Task()
              {
                  Id = taskId,
                  Test = new Test()
                  {
                      Id = Guid.NewGuid(),
                      Name = "sks",
                      Teacher = null,
                      Tasks = null,
                      Results = null

                  },
                  Name = "sts",
                  TaskNumber = 2,
                  Mark = 1,
                  Options = null,
                  Results = null

              });

            var result = _controller.RemoveTask(taskId);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageTasks", redirect.ActionName);
        }

        [Fact]
        public void PassTest_UserIsNotLogined_RedirectToError()
        {
            var result = _controller.PassTest(Guid.NewGuid());
            Assert.NotNull(result);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void PassTest_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var result = _controller.PassTest(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void PassTest_UserIsLogined_Exist_UserIsNotStudent_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var result = _controller.PassTest(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void PassTest_UserIsStudent_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Student"
              });

            Guid testId = Guid.NewGuid();
            _mockTestRepo.Setup(repo => repo.GetTest(testId))
              .Returns(new Test()
              {
                  Id = testId,
                  Name = "sks",
                  Teacher = null,
                  Tasks = new List<Task>(),
                  Results = null

              });

            var result = _controller.PassTest(testId);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("CurrentTest", redirect.ActionName);
        }

        [Fact]
        public void CurrentTest_UserIsNotLogined_RedirectToError()
        {
            var result = _controller.CurrentTest(Guid.NewGuid());
            Assert.NotNull(result);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void CurrentTest_UserIsLogined_DoesNotExist_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(null as User);

            var result = _controller.CurrentTest(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void CurrentTest_UserIsLogined_Exist_UserIsNotStudent_RedirectToError()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Admin"
              });

            var result = _controller.CurrentTest(Guid.NewGuid());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }

        [Fact]
        public void CurrentTest_UserIsStudent_ReturnsView()
        {
            Guid id = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", id.ToString()).ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockUserRepo.Setup(repo => repo.GetUser(id))
              .Returns(new User()
              {
                  Id = id,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Student"
              });

            Guid testResultId = Guid.NewGuid();
            _mockTestResultRepo.Setup(repo => repo.GetTestResult(testResultId))
              .Returns(new TestResult()
              {
                  Id = testResultId,
                  Test = new Test()
                  {
                      Id = Guid.NewGuid(),
                      Name = "sks",
                      Teacher = null,
                      Tasks = null,
                      Results = null

                  },
                  Student = null,
                  TaskResults = new List<TaskResult>(),

              });

            var result = _controller.CurrentTest(testResultId);
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<TaskResult>>(view.Model);
            Assert.Empty(model);
        }
    }
}