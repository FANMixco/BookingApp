using System.Linq;
using BookingApp.DB.Classes.DB;
using BookingApp.Filters.Authorization;
using BookingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Authorize(Roles.ADMIN)]
    public class SettingsController : Controller
    {
        private const string PASS_KEY = "!emJ(?w)Sx_5S-3L";
        readonly IHttpContextAccessor _httpContextAccessor;

        public SettingsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            using var db = new BookingContext();

            var settings = db.Settings.FirstOrDefault();

            return View(new SettingsModel
            {
                Port = settings.Port.ToString(),
                MaxTime = settings.MaxTime,
                Email = settings.Email,
                Host = settings.MailHost
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(string port, string max, string email, string host, string password)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (_httpContextAccessor.HttpContext.Session.GetInt32("role") == 0)
                    {
                        return RedirectToAction("Index", "Library");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Booking");
                    }
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
