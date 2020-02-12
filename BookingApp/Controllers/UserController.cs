using System;
using System.Linq;
using BookingApp.Classes;
using BookingApp.Classes.DB;
using BookingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int? Role { get; set; }

        public UserController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            Role = _httpContextAccessor.HttpContext.Session.GetInt32("role");

            if (Role == null)
            {
                return RedirectToAction("Index", "Home", new { error = "NoLogin" });
            }
            else if (Role != 0)
            {
                return RedirectToAction("Index", "Home", new { error = "WrongRole" });
            }

            return View(new UserCreateModel());
        }

        [HttpPost]
        public IActionResult Index(string username, string password, string SelectedRole)
        {
            using var db = new BookingContext();

            if (db.Users.Count(x => x.Username == username) == 0)
            {
                try
                {
                    _ = db.Add(new Users() { Username = username, Registered = DateTime.Now, Role = int.Parse(SelectedRole), Password = Encryption.Encrypt(password) });
                    _ = db.SaveChanges();

                    return RedirectToAction("Index", "User", new { user = "Created" });
                }
                catch
                {
                    return RedirectToAction("Index", "User", new { user = "Error" });
                }
            }
            else
            {
                return RedirectToAction("Index", "User", new { user = "Exist" });
            }
        }

        public IActionResult Update(int id)
        {
            Role = _httpContextAccessor.HttpContext.Session.GetInt32("role");

            if (Role == null)
            {
                return RedirectToAction("Index", "Home", new { error = "NoLogin" });
            }
            else if (Role != 0)
            {
                return RedirectToAction("Index", "Home", new { error = "WrongRole" });
            }

            UserUpdateModel model = new UserUpdateModel();
            using var db = new BookingContext();

            var users = db.Users.FirstOrDefault(x => x.UserId == id);

            if (users != null)
            {
                model.ID = id;
                model.Username = users.Username;
                model.SelectedRole = users.Role;
            }
            else
            {
                RedirectToAction("Index", "Library", new { error = "WrongUser" });
            }

            return View(model);
        }
    }
}
