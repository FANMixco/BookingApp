using System;
using System.Diagnostics;
using System.Linq;
using BookingApp.DB.Classes;
using BookingApp.DB.Classes.DB;
using BookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

namespace BookingApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IHttpContextAccessor httpContextAccessor/*, ILogger<HomeController> logger*/)
        {
            //_logger = logger;

            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            using var db = new BookingContext();

            if (db.Users.Count() == 0)
            {
                return RedirectToAction("FirstTime", "Home");
            }

            var role = _httpContextAccessor.HttpContext.Session.GetInt32("role");
            if (role != null)
            {
                if (role == 0)
                {
                    return RedirectToAction("Index", "Library");
                }
                else
                {
                    return RedirectToAction("Index", "Booking");
                }
            }

            return View();
        }

        public IActionResult FirstTime()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult FirstTime(string userName, string adminEmail, string adminPassword, string repeatPassword, string port, string max, string email, string host, string password)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("FirstTime", "Home");
                }

                if (adminPassword != repeatPassword)
                {
                    return RedirectToAction("FirstTime", "Home", new { msg = "wrongPasswords" });
                }

                if (!string.IsNullOrEmpty(email) && (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(port)))
                {
                    return RedirectToAction("FirstTime", "Home", new { error = "wrongEmailSettings" });
                }

                using var db = new BookingContext();

                var user = new Users()
                {
                    Username = userName,
                    Email = adminEmail,
                    Password = Encryption.Encrypt(adminPassword),
                    Role = 0
                };
                db.Add(user);
                db.SaveChanges();

                int? intPort = int.TryParse(port, out var tempVal) ? tempVal : (int?)null;

                var settings = new Settings
                {
                    Port = intPort,
                    Email = email,
                    MaxTime = int.Parse(max),
                    MailHost = host
                };

                if (!string.IsNullOrEmpty(password))
                {
                    settings.PasswordHost = DB.EncryptionMails.EncryptString(SettingsController.PASS_KEY, password);
                }
                db.Add(settings);
                db.SaveChanges();

                return RedirectToAction("Index", "Home", new { msg = "userCreated" });
            }
            catch
            {
                return RedirectToAction("FirstTime", "Home", new { error = "error" });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error500()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            using var db = new BookingContext();

            var user = db.Users.FirstOrDefault(x => x.Username == username && x.Password == Encryption.Encrypt(password));

            if (user != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("JWToken", TokenProvider.LoginUser(user));

                _httpContextAccessor.HttpContext.Session.SetString("user", username);
                _httpContextAccessor.HttpContext.Session.SetInt32("role", user.Role);

                if (user.Role == 0)
                {
                    return RedirectToAction("Index", "Library");
                }
                else
                {
                    return RedirectToAction("Index", "Booking");
                }
            }

            return RedirectToAction("Index", "Home", new { error = "password" });
        }
    }
}
