using Xunit;
using Moq;
using Check_n_Cheer.Controllers;
using Check_n_Cheer.Interfaces;
using Check_n_Cheer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;


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
        string GetCookieValueFromResponse(HttpResponse response, string cookieName)
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
        public void IfNotLoggedIn_GetSignInAction_ReturnsView()
        {
            var result = _controller.SignIn();
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void IfNotLoggedIn_GetSignUpAction_ReturnsView()
        {
            var result = _controller.SignUp();
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void IfLoggedIn_GetSignInAction_ReturnsRedirect()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = _controller.SignIn();

            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
        }
        [Fact]
        public void IfLoggedIn_GetSignUpAction_ReturnsRedirect()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = _controller.SignUp();
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
        }
        [Fact]
        public void PostSignUpAction_RegisterUser_ReturnsView()
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
        public void PostSignInAction_SignInUser_ReturnsView()
        {
            _mockRepo.Setup(repo => repo.GetUser("test@test.com"))
                .Returns(new User() {
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
            var result = _controller.SignIn(testUser);
            var viewResult = Assert.IsType<ViewResult>(result);
            var user = Assert.IsType<User>(viewResult.Model);
            Assert.Equal(testUser.Email, user.Email);
            Assert.Equal(testUser.Password, user.Password);
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
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
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
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
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
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
