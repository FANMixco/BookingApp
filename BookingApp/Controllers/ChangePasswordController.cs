using BookingApp.DB.Classes;
using BookingApp.DB.Classes.DB;
using BookingApp.Filters.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BookingApp.Controllers
{
    [Authorize(Roles.CUSTOMER, Roles.ADMIN)]
    public class ChangePasswordController : Controller
    {
        public IActionResult Index()
        {
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
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "ChangePassword");
                }

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

                    //MailLibrary.MailNotifications.SendEmail(user.Email, "Your password was updated.", "Password updated");
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