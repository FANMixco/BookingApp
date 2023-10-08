using System;
using System.Linq;
using BookingApp.DB.Classes;
using BookingApp.DB.Classes.DB;
using BookingApp.Filters.Authorization;
using BookingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Authorize(Roles.ADMIN)]
    public class UserController : Controller
    {
        public UserController()
        {

        }

        public IActionResult Index()
        {
            return View(new UserCreateModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(string username, string email, string password, string SelectedRole)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                if (db.Users.Any(x => x.Email == email))
                {
                    return RedirectToAction("Update", "User", new { error = "sameEmail" });
                }
                else if (!db.Users.Any(x => x.Username == username))
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
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                var user = db.Users.FirstOrDefault(x => x.UserId == id);

                if (user.Email != email && db.Users.Count(x => x.Email == email) > 0)
                {
                    return RedirectToAction("Update", "User", new { error = "sameEmail" });
                }

                //var passUpdated = false;
                if (!string.IsNullOrEmpty(password))
                {
                    user.Password = Encryption.Encrypt(password);
                    //passUpdated = true;
                }
                user.Username = username;
                user.Email = email;
                user.Role = int.Parse(SelectedRole);
                db.Update(user);
                db.SaveChanges();

                /*if (passUpdated)
                {
                    MailLibrary.MailNotifications.SendEmail(user.Email, $"Your password was updated. New password is: ${password}. If it was not requested, please contact the administration by phone.", "Password updated");
                }*/

                return RedirectToAction("Update", "User", new { msg = "updated" });
            }
            catch
            {
                return RedirectToAction("Update", "User", new { error = "error" });
            }
        }

        public IActionResult Update(int id)
        {
            using var db = new BookingContext();

            var users = db.Users.FirstOrDefault(x => x.UserId == id);

            if (users != null)
            {
                return base.View(new UserUpdateModel
                {
                    ID = id,
                    Username = users.Username,
                    SelectedRole = users.Role,
                    Email = users.Email
                });
            }
            else
            {
                return RedirectToAction("Index", "Library", new { error = "wrongUser" });
            }
        }
    }
}
