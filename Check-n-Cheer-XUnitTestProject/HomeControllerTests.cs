using System;
using Xunit;
using Moq;
using Check_n_Cheer.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Check_n_Cheer_XUnitTestProject
{
    public class HomeControllerTests
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeController _controller;
        public HomeControllerTests()
        {
            _logger = Mock.Of<ILogger<HomeController>>();
            _controller = new HomeController(_logger);
        }
        [Fact]
        public void IndexPageReturnsView()
        {
            var result = _controller.Index();
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void IsNotLoggedInFirst()
        {
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var result = _controller.Index() as ViewResult;
            Assert.Equal("false", result.ViewData["LoggedIn"]);
        }
        [Fact]
        public void LoggedInWhenCookieFieldIsSet()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var result = _controller.Index() as ViewResult;
            Assert.Equal("true", result.ViewData["LoggedIn"]);
        }
    }
}
