using System;
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

        
    }
}