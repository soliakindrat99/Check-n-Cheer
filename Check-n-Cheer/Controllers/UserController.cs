using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Check_n_Cheer.Models;
using Check_n_Cheer.Interfaces;
using System.Collections.Generic;

namespace Check_n_Cheer.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IUserRepository _userRepo;
        private ITestRepository _testRepo;

        public UserController(ILogger<UserController> logger, IUserRepository userRepo,ITestRepository testRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
            _testRepo = testRepo;
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
            _logger.LogInformation("GET User/SignUp");
            if (Get("user") == null){
                ViewData["LoggedIn"] = "false";
                
                return View();
            }
            _logger.LogInformation("User is logged in!");
            ViewData["LoggedIn"] = "true";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult SignUp(User formData)
        {
            _logger.LogInformation("POST User/SignUp");
            User user = null;
            if (formData != null)
            {
                user = _userRepo.GetUser(formData.Email);
            }
            else
            {
                _logger.LogInformation("Form data is empty!");
            }

            if (user == null)
            {
                formData.Id = Guid.NewGuid();
                _userRepo.RegisterUser(formData);
                var view = View("Thanks", formData);
                view.ViewData["LoggedIn"] = "false";
                view.ViewData["Type"] = "signing up";
                return view;
            }
            else if(formData!=null)
            {
                _logger.LogInformation("User has already registered!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("SignUp");
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            _logger.LogInformation("GET User/SignIn");
            if (Get("user") == null)
            {
                ViewData["LoggedIn"] = "false";
                return View();
            }
            _logger.LogInformation("User is logged in!");
            ViewData["LoggedIn"] = "true";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult SignIn(User formData)
        {
            _logger.LogInformation("POST User/SignIn");
            User user = _userRepo.GetUser(formData.Email);
            if(user != null && user.CheckPassword(formData.Password))
            {
                Set("user", Convert.ToString(user.Id));
                var view = View("Thanks", user);
                view.ViewData["LoggedIn"] = "true";
                view.ViewData["Type"] = "signing in";
                return view;
            }
            if (user == null)
            {
                _logger.LogInformation("User not found!");
            }
            else if (!user.CheckPassword(formData.Password))
            {
                _logger.LogInformation("Wrong password!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("SignIn");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            _logger.LogInformation("GET User/Logout");
            Remove("user");
            return RedirectToAction("SignIn");
        }
        [HttpGet]
        public IActionResult Profile()
        {
            _logger.LogInformation("GET User/Profile");
            User user = null;
            if (Get("user") != null)
            {
                Guid id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            
            if (user != null)
            {
                if (user.Role == "Admin")
                {
                    return RedirectToAction("AdminProfile");
                }
                ViewData["LoggedIn"] = "true";
                return View(user);
            }
            if(Get("user") != null)
            {
                _logger.LogInformation("User authorised but not exist!");
            }

            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        [HttpGet]
        public IActionResult AdminProfile(string? id)
        {
            _logger.LogInformation("GET User/AdminProfile");
            User user = null;
            if (Get("user") != null)
            {
                Guid user_id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(user_id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user!=null && user.Role == "Admin")
            {
                List<User> users;
                if (id == null)
                {
                    users = _userRepo.GetUsers();
                }
                else
                {
                    users = new List<User> { _userRepo.GetUser(id)};
                }
                ViewData["LoggedIn"] = "true";
                return View(users);
            }
            if(user != null )
            {
                _logger.LogInformation("User is not admin!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }
        [HttpPost]
        public IActionResult ChangeToStudent(string id)
        {
            _logger.LogInformation("POST User/ChangeToStudent");
            User user = null;
            if (Get("user") != null)
            {
                Guid user_id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(user_id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user != null && user.Role == "Admin")
            {
                _userRepo.SetUserRole(Guid.Parse(id), "Student");
                return RedirectToAction("AdminProfile");
            }
            if (user != null)
            {
                _logger.LogInformation("User is not admin!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }
        [HttpPost]
        public IActionResult ChangeToTeacher(string id)
        {
            _logger.LogInformation("POST User/ChangeToTeacher");
            User user = null;
            if (Get("user") != null)
            {
                Guid user_id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(user_id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user != null && user.Role == "Admin")
            {
                _userRepo.SetUserRole(Guid.Parse(id), "Teacher");
                return RedirectToAction("AdminProfile");
            }
            if (user != null)
            {
                _logger.LogInformation("User is not admin!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }
        [HttpPost]
        public IActionResult RemoveUser(string id)
        {
            _logger.LogInformation("POST User/RemoveUser");
            User user = null;
            if (Get("user") != null)
            {
                Guid user_id = Guid.Parse(Get("user"));
                user = _userRepo.GetUser(user_id);
            }
            else
            {
                _logger.LogInformation("User is not logged!");
            }
            if (user != null && user.Role == "Admin")
            {
                _userRepo.RemoveUser(Guid.Parse(id));
                return RedirectToAction("AdminProfile");
            }
            if (user != null)
            {
                _logger.LogInformation("User is not admin!");
            }
            else
            {
                _logger.LogInformation("User authorised but not exist!");
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
