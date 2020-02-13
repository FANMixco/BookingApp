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
                return RedirectToAction("Index", "Home", new { error = "noLogin" });
            }
            else if (Role != 0)
            {
                _httpContextAccessor.HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home", new { error = "wrongRole" });
            }

            return View(new UserCreateModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(string username, string email, string password, string SelectedRole)
        {
            try
            {
                using var db = new BookingContext();

                if (db.Users.Count(x => x.Username == username) == 0)
                {
                    _ = db.Add(new Users() { Username = username, Email = email, Registered = DateTime.Now, Role = int.Parse(SelectedRole), Password = Encryption.Encrypt(password) });
                    _ = db.SaveChanges();

                    return RedirectToAction("Index", "User", new { msg = "created" });
                }
                else
                {
                    return RedirectToAction("Index", "User", new { error = "exist" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "User", new { error = "error" });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Update(int id, string username, string email, string password, string SelectedRole)
        {
            try
            {
                using var db = new BookingContext();

                var user = db.Users.FirstOrDefault(x => x.UserId == id);
                if (!string.IsNullOrEmpty(password))
                {
                    user.Password = Encryption.Encrypt(password);
                }
                user.Username = username;
                user.Email = email;
                user.Role = int.Parse(SelectedRole);
                db.Update(user);
                db.SaveChanges();

                return RedirectToAction("Update", "User", new { msg = "updated" });
            }
            catch
            {
                return RedirectToAction("Update", "User", new { error = "error" });
            }
        }

        public IActionResult Update(int id)
        {
            Role = _httpContextAccessor.HttpContext.Session.GetInt32("role");

            if (Role == null)
            {
                return RedirectToAction("Index", "Home", new { error = "noLogin" });
            }
            else if (Role != 0)
            {
                _httpContextAccessor.HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home", new { error = "wrongRole" });
            }

            UserUpdateModel model = new UserUpdateModel();
            using var db = new BookingContext();

            var users = db.Users.FirstOrDefault(x => x.UserId == id);

            if (users != null)
            {
                model.ID = id;
                model.Username = users.Username;
                model.SelectedRole = users.Role;
                model.Email = users.Email;
            }
            else
            {
                RedirectToAction("Index", "Library", new { error = "wrongUser" });
            }

            return View(model);
        }
    }
}
