using System.Diagnostics;
using System.Linq;
using BookingApp.DB.Classes;
using BookingApp.DB.Classes.DB;
using BookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookingApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        private readonly DBInfoSettings _settings;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private static bool HasStarted = false;

        public HomeController(IHttpContextAccessor httpContextAccessor, IOptions<DBInfoSettings> settings/*, ILogger<HomeController> logger*/)
        {
            //_logger = logger;

            _httpContextAccessor = httpContextAccessor;

            _settings = settings.Value;

            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Cookies["DBVersion"]) && !HasStarted)
            {
                using var db = new BookingContext();
                db.DefaultData();

                _httpContextAccessor.HttpContext.Response.Cookies.Append("DBVersion", _settings.Version);
            }
            HasStarted = true;
        }

        public IActionResult Index()
        {
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
