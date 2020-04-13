using Xunit;
using Moq;
using Check_n_Cheer.Controllers;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;


namespace Check_n_Cheer_XUnitTestProject
{
    public class UserControllerTests
    {
        private readonly ILogger<UserController> _logger;
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserController _controller;
        public UserControllerTests()
        {
            _logger = Mock.Of<ILogger<UserController>>();
            _mockRepo = new Mock<IUserRepository>();
            _controller = new UserController(_logger, _mockRepo.Object);
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

        [Fact]
        public void GetSignInAction_UserNotLoggedIn_ReturnsView()
        {
            var result = _controller.SignIn();
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void GetSignUpAction_UserNotLoggedIn_ReturnsView()
        {
            var result = _controller.SignUp();
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void GetSignInAction_UserLoggedIn_ReturnsRedirect()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = _controller.SignIn();
          
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public void GetSignUpAction_UserLoggedIn_ReturnsRedirect()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = _controller.SignUp();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public void PostSignUpAction_RegisteredSuccessfully_ReturnsView()
        {
            var testUser = new User
            {
                Id = 100,
                Email = "test@test.com",
                Password = "test"
            };
            var result = _controller.SignUp(testUser);
            var viewResult = Assert.IsType<ViewResult>(result);
            var user = Assert.IsType<User>(viewResult.Model);
            Assert.Equal(testUser.Password, user.Password);
        }
        [Fact]
        public void PostSignInAction_SignedSuccessfully_ReturnsView()
        {
            _mockRepo.Setup(repo => repo.GetUser("test@test.com"))
                .Returns(new User() {
                    Id = 100,
                    Email = "test@test.com",
                    Password = "G08OmFGXGZjnMgeFRMlrNsPQHO33yqMyNZ1vHYNWcBQ="
                });
            var testUser = new User
            {
                Id = 100,
                Email = "test@test.com",
                Password = "test1"
            };
            var result = _controller.SignIn(testUser);
            var viewResult = Assert.IsType<ViewResult>(result);
            var user = Assert.IsType<User>(viewResult.Model);
            Assert.Equal(testUser.Email, user.Email);
            Assert.Equal(Crypto.Hash(testUser.Password), user.Password);
        }
        [Fact]
        public void PostSignUpAction_UserExists_ReturnsRedirect()
        {
            _mockRepo.Setup(repo => repo.GetUser("test@test.com"))
                .Returns(new User()
                {
                    Id = 100,
                    Email = "test@test.com",
                    Password = "test"
                });
            var testUser = new User
            {
                Id = 100,
                Email = "test@test.com",
                Password = "test"
            };
            var result = _controller.SignUp(testUser);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SignUp", redirect.ActionName);
        }
        [Fact]
        public void PostSignInAction_WrongUser_ReturnsRedirect()
        {
            _mockRepo.Setup(repo => repo.GetUser("test@test.com"))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test"
               });
            var testUser = new User
            {
                Id = 100,
                Email = "test@test.com",
                Password = "wrong"
            };
            var result = _controller.SignIn(testUser);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SignIn", redirect.ActionName);
        }
        [Fact]
        public void LogoutAction_RemoveUserFromCookies_ReturnsRedirect()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var result = _controller.Logout();
            Assert.Equal("", GetCookieValueFromResponse(_controller.HttpContext.Response,"user"));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SignIn", redirect.ActionName);
        }

        [Fact]
        public void ProfileAction_UserIsStudent_ReturnsView()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Student"
               });
            var result = _controller.Profile();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void ProfileAction_UserIsAdmin_ReturnsRedirectToAdmin()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Admin"
               });
            var result = _controller.Profile();
            
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminProfile", redirect.ActionName);
        }
        [Fact]
        public void ProfileAction_UserIsNotLoginned_ReturnsRedirectToError()
        {
            
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Student"
               });
            var result = _controller.Profile();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void ProfileAction_UserIsNotRegistered_ReturnsRedirectToError()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            _mockRepo.Setup(repo => repo.GetUser(100))
              .Returns(null as User);
            var result = _controller.Profile();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void AdminProfileAction_UserIsStudent_ReturnsRedirectToError() 
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Student"
               });

            var result = _controller.AdminProfile(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void AdminProfileAction_UserIsNotLoginned_ReturnsRedirectToError()
        {
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Student"
               });
          
            var result = _controller.AdminProfile(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void AdminProfileAction_UserIsNotRegistered_ReturnsRedirectToError()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockRepo.Setup(repo => repo.GetUser(100))
              .Returns(null as User);

            var result = _controller.AdminProfile(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void AdminProfileAction_UserIsAdmin_IdIsNull_ReturnsView_ModelLengthIs3() 
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Admin"
               });

            _mockRepo.Setup(repo => repo.GetUsers())
               .Returns(new User[] { new User(){
                   Id = 100,
                   Email = "test1@test.com",
                   Password = "test",
                   Role = "Student"
               }, new User(){
                   Id = 100,
                   Email = "test2@test.com",
                   Password = "test",
                   Role = "Student"
               }, new User(){
                   Id = 100,
                   Email = "test3@test.com",
                   Password = "test",
                   Role = "Student"
               }
               }
               );

            var result = _controller.AdminProfile(null);
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<User[]>(view.Model) ;
            Assert.Equal(3, model.Length);
        }
        [Fact]
        public void AdminProfileAction_UserIsAdmin_IdNotNull_ReturnsView_ModelLengthIs1()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Admin"
               });

            _mockRepo.Setup(repo => repo.GetUser("test1@test.com"))
               .Returns(new User(){
                   Id = 100,
                   Email = "test1@test.com",
                   Password = "test",
                   Role = "Student"
               } );

            var result = _controller.AdminProfile("test1@test.com");
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<User[]>(view.Model);
            Assert.Single(model);
        }

        [Fact]
        public void ChangeToTeacherAction_UserIsStudent_ReturnsRedirectToError() 
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Student"
               });

            var result = _controller.ChangeToTeacher("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void ChangeToTeacherAction_UserIsNotLoginned_ReturnsRedirectToError()
        {
            _mockRepo.Setup(repo => repo.GetUser(100))
              .Returns(new User()
              {
                  Id = 100,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Student"
              });

            var result = _controller.ChangeToTeacher("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void ChangeToTeacherAction_UserIsNotRegistered_ReturnsRedirectToError()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockRepo.Setup(repo => repo.GetUser(100))
              .Returns(null as User);

            var result = _controller.ChangeToTeacher("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void ChangeToTeacherAction_UserIsAdmin_ReturnsRedirectToAdmin()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Admin"
               });


            var result = _controller.ChangeToTeacher("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminProfile", redirect.ActionName);
        }

        [Fact]
        public void ChangeToStudentAction_UserIsStudent_ReturnsRedirectToError()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Student"
               });

            var result = _controller.ChangeToStudent("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void ChangeToStudentAction_UserIsNotLoginned_ReturnsRedirectToError()
        {
            _mockRepo.Setup(repo => repo.GetUser(100))
              .Returns(new User()
              {
                  Id = 100,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Student"
              });

            var result = _controller.ChangeToStudent("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void ChangeToStudentAction_UserIsNotRegistered_ReturnsRedirectToError()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockRepo.Setup(repo => repo.GetUser(100))
              .Returns(null as User);

            var result = _controller.ChangeToStudent("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void ChangeToStudentAction_UserIsAdmin_ReturnsRedirectToAdmin()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Admin"
               });


            var result = _controller.ChangeToStudent("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminProfile", redirect.ActionName);
        }


        [Fact]
        public void RemoveUserAction_UserIsStudent_ReturnsRedirectToError()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Student"
               });

            var result = _controller.RemoveUser("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void RemoveUserAction_UserIsNotLoginned_ReturnsRedirectToError()
        {
            _mockRepo.Setup(repo => repo.GetUser(100))
              .Returns(new User()
              {
                  Id = 100,
                  Email = "test@test.com",
                  Password = "test",
                  Role = "Student"
              });

            var result = _controller.RemoveUser("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void RemoveUserAction_UserIsNotRegistered_ReturnsRedirectToError()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockRepo.Setup(repo => repo.GetUser(100))
              .Returns(null as User);

            var result = _controller.RemoveUser("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirect.ActionName);
        }
        [Fact]
        public void RemoveUserAction_UserIsAdmin_ReturnsRedirectToAdmin()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _mockRepo.Setup(repo => repo.GetUser(100))
               .Returns(new User()
               {
                   Id = 100,
                   Email = "test@test.com",
                   Password = "test",
                   Role = "Admin"
               });


            var result = _controller.RemoveUser("100");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminProfile", redirect.ActionName);
        }
    }
}