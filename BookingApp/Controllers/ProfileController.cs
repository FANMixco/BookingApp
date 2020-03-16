using System.Linq;
using BookingApp.DB.Classes.DB;
using BookingApp.Filters.Authorization;
using BookingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Authorize(Roles.CUSTOMER, Roles.ADMIN)]
    public class ProfileController : Controller
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View(new ProfileModel
            {
                Email = new BookingContext().Users.FirstOrDefault(x => x.Username == _httpContextAccessor.HttpContext.Session.GetString("user")).Email
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(string email)
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

                var db = new BookingContext();

                var isEmailInvalid = db.Users.Count(x => x.Email == email) > 0;

                var user = db.Users.FirstOrDefault(x => x.Username == _httpContextAccessor.HttpContext.Session.GetString("user"));

                if (isEmailInvalid && !user.Email.Equals(email))
                {
                    return RedirectToAction("Index", "Profile", new { error = "registered" });
                }

                user.Email = email;
                db.Update(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Profile", new { msg = "updated" });
            }
            catch
            {
                return RedirectToAction("Index", "Profile", new { error = "error" });
            }
        }
    }
}