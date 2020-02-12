using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Classes;
using BookingApp.Classes.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class ChangePasswordController : Controller
    {
        public IActionResult Index()
        {
            var role = _httpContextAccessor.HttpContext.Session.GetInt32("role");

            if (role == null)
            {
                return RedirectToAction("Index", "Home", new { error = "NoLogin" });
            }

            return View();
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangePasswordController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(string oldPassword, string newPassword, string rPassword)
        {
            if (newPassword != rPassword)
            {
                return RedirectToAction("Index", "ChangePassword", new { error = "WrongPasswords" });
            }

            var role = _httpContextAccessor.HttpContext.Session.GetInt32("role");

            if (role == null)
            {
                return RedirectToAction("Index", "Home", new { error = "NoLogin" });
            }

            using var db = new BookingContext();

            var username = _httpContextAccessor.HttpContext.Session.GetString("user");

            var user = db.Users.FirstOrDefault(x => x.Username == username);

            if (Encryption.Encrypt(oldPassword).Equals(user.Password))
            {
                user.Password = Encryption.Encrypt(newPassword);
                db.Update(user);
                db.SaveChanges();
                return RedirectToAction("Index", "ChangePassword", new { message = "Updated" });
            }
            else
            {
                return RedirectToAction("Index", "ChangePassword", new { error = "WrongOld" });
            }
        }
    }
}