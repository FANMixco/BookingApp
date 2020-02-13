using BookingApp.Classes;
using BookingApp.Classes.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BookingApp.Controllers
{
    public class ChangePasswordController : Controller
    {
        public IActionResult Index()
        {
            var role = _httpContextAccessor.HttpContext.Session.GetInt32("role");

            if (role == null)
            {
                _httpContextAccessor.HttpContext.Session.Clear();
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
            try
            {
                if (newPassword != rPassword)
                {
                    return RedirectToAction("Index", "ChangePassword", new { error = "wrongPasswords" });
                }

                using var db = new BookingContext();

                var username = _httpContextAccessor.HttpContext.Session.GetString("user");

                var user = db.Users.FirstOrDefault(x => x.Username == username);

                if (Encryption.Encrypt(oldPassword).Equals(user.Password))
                {
                    user.Password = Encryption.Encrypt(newPassword);
                    db.Update(user);
                    db.SaveChanges();
                    return RedirectToAction("Index", "ChangePassword", new { msg = "updated" });
                }
                else
                {
                    return RedirectToAction("Index", "ChangePassword", new { error = "wrongOld" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "ChangePassword", new { error = "error" });
            }
        }
    }
}