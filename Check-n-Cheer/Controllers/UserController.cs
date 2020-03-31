using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Check_n_Cheer.Models;
using Check_n_Cheer.Interfaces;

namespace Check_n_Cheer.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IUserRepository _repo;

        public UserController(ILogger<UserController> logger, IUserRepository repo)
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
        public IActionResult SignUp()
        {
            if (Get("user") == null){
                ViewData["LoggedIn"] = "false";
                return View();
            }
            ViewData["LoggedIn"] = "true";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult SignUp(User formData)
        {
            if(formData != null)
            {
                _repo.RegisterUser(formData);
                var view = View("Thanks", formData);
                view.ViewData["LoggedIn"] = "false";
                view.ViewData["Type"] = "signing up";
                return view;
            }
            ViewData["LoggedIn"] = "false";
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            if (Get("user") == null)
            {
                ViewData["LoggedIn"] = "false";
                return View();
            }
            ViewData["LoggedIn"] = "true";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult SignIn(User formData)
        {
            User user = _repo.GetUser(formData.Email);
            if(user != null && user.Password == formData.Password)
            {
                Set("user", Convert.ToString(user.Id));
                var view = View("Thanks", user);
                view.ViewData["LoggedIn"] = "true";
                view.ViewData["Type"] = "signing in";
                return view;
            }
            ViewData["LoggedIn"] = "false";
            return View();
        }

        public IActionResult Profile()
        {
            int id = int.Parse(Get("user"));
            User user = _repo.GetUser(id);
            if (user.Role == "Admin")
            {
                return RedirectToAction("AdminProfile");
            }
            if (user != null)
            {
                ViewData["LoggedIn"] = "true";
                return View(user);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        public IActionResult AdminProfile()
        {
            int id = int.Parse(Get("user"));
            User user = _repo.GetUser(id);
            if (user!=null && user.Role == "Admin")
            {
                User[] users = _repo.GetUsers();
                ViewData["LoggedIn"] = "true";
                return View(users);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public IActionResult ChangeToStudent(string id)
        {
            _repo.SetUserRole(int.Parse(id), "Student");
            return RedirectToAction("AdminProfile");
        }

        [HttpPost]
        public IActionResult ChangeToTeacher(string id)
        {
            _repo.SetUserRole(int.Parse(id), "Teacher");
            return RedirectToAction("AdminProfile");
        }

        [HttpPost]
        public IActionResult RemoveUser(string id)
        {
            _repo.RemoveUser(int.Parse(id));
            return RedirectToAction("AdminProfile");
        }

        public IActionResult Logout()
        {
            Remove("user");
            return RedirectToAction("SignIn", "User");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
