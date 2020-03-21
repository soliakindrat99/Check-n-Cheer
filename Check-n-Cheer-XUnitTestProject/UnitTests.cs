using System;
using Xunit;
using Check_n_Cheer.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Net.Http.Headers;

namespace Check_n_Cheer_XUnitTestProject
{
    public class UnitTests
    {
        [Fact]
        public void IndexPageNotNull()
        {
            var logger = Mock.Of<ILogger<HomeController>>();
            var home = new HomeController(logger);
            var result = home.Index() as ViewResult;
            Assert.NotNull(result);
        }
        [Fact]
        public void IsNotLoggedInFirst()
        {
            var logger = Mock.Of<ILogger<HomeController>>();
            var httpContext = new DefaultHttpContext();
            var home = new HomeController(logger);
            home.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var result = home.Index() as ViewResult;
            Assert.Equal("false",result.ViewData["LoggedIn"]);
        }
        [Fact]
        public void LoggedInWhenCookieFieldIsSet()
        {
            var logger = Mock.Of<ILogger<HomeController>>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Cookie", new CookieHeaderValue("user", "100").ToString());
            var home = new HomeController(logger);
            home.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var result = home.Index() as ViewResult;
            Assert.Equal("true", result.ViewData["LoggedIn"]);
        }
    }
}
