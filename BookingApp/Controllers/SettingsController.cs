using System.Linq;
using BookingApp.DB.Classes.DB;
using BookingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static int? Role { get; set; }

        private const string PASS_KEY = "!emJ(?w)Sx_5S-3L";

        public SettingsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            Role = _httpContextAccessor.HttpContext.Session.GetInt32("role");

            if (Role == null)
            {
                return RedirectToAction("Index", "Home", new { error = "noLogin" });
            }
            else if (Role != 0)
            {
                return RedirectToAction("Index", "Home", new { error = "wrongRole" });
            }

            var model = new SettingsModel();

            using var db = new BookingContext();

            var settings = db.Settings.FirstOrDefault();

            model.Port = settings.Port.ToString();
            model.MaxTime = settings.MaxTime;
            model.Email = settings.Email;
            model.Host = settings.MailHost;

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(string port, string max, string email, string host, string password)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                var settings = db.Settings.FirstOrDefault();

                settings.Port = int.Parse(port);
                settings.Email = email;
                settings.MaxTime = int.Parse(max);
                settings.MailHost = host;

                if (!string.IsNullOrEmpty(password))
                {
                    settings.PasswordHost = DB.EncryptionMails.EncryptString(PASS_KEY, password);
                }
                db.Update(settings);
                db.SaveChanges();

                return RedirectToAction("Index", "User", new { msg = "updated" });
            }
            catch
            {
                return RedirectToAction("Index", "User", new { error = "error" });
            }
        }
    }
}
