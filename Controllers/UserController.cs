using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [HttpGet]
        public ViewResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(User user)
        {
            if(user != null)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                await Authenticate(user.Email);
                return View("Profile", user);
            }
            return View();
        }

        [HttpGet]
        public ViewResult SignIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(User formData)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == formData.Email);
            if(user != null && user.Password == formData.Password)
            {
                await Authenticate(user.Email);
                return View("Profile", user);
            }
            return View();
        }
        [Authorize]
        public IActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["Authenticated"] = "true";
            }
            else
                ViewData["Authenticated"] = "false";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn", "User");
        }

        private async Task Authenticate(string email)
        {
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email)
            };
            
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
