using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Check_n_Cheer.Models;
using Check_n_Cheer.Interfaces;

namespace Check_n_Cheer.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private IUserRepository _repo;

        public TestController(ILogger<TestController> logger, IUserRepository repo)
        {
            _logger = logger;
            _repo = repo;
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
            int id = int.Parse(Get("user"));
            User user = _repo.GetUser(id);
            if (user != null && user.Role == "Teacher")
            {
                ViewData["LoggedIn"] = "true";
                return View();
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
