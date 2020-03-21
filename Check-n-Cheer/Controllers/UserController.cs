using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private UserContext db;

        public UserController(ILogger<UserController> logger, UserContext context)
        {
            _logger = logger;
            db = context;
        }

        public string Get(string key)
        {
            if (Request == null)
                return null;
            return Request.Cookies[key];
        }
        public void Set(string key, string value, int expireTime = 1)
        {
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
        public async Task<IActionResult> SignUp(User formData)
        {
            if(formData != null)
            {
                db.Users.Add(formData);
                await db.SaveChangesAsync();
                return RedirectToAction("SignIn");
            }
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
        public async Task<IActionResult> SignIn(User formData)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == formData.Email);
            if(user != null && user.Password == formData.Password)
            {
                Set("user", Convert.ToString(user.Id));
                return RedirectToAction("Profile");
            }
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            int id = int.Parse(Get("user"));
            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user != null)
            {
                ViewData["LoggedIn"] = "true";
                return View(user);
            }
            ViewData["LoggedIn"] = "false";
            return RedirectToAction("Error");
        }

        public async Task<IActionResult> Logout()
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
