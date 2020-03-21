using System;
using Xunit;
using Check_n_Cheer.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Microsoft.Extensions.Logging;
using Moq;

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
    }
}
