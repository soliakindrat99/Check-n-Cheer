using Xunit;
using Moq;
using Check_n_Cheer.Controllers;
using Check_n_Cheer.Interfaces;
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
    }
}
