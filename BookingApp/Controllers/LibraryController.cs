using System.Linq;
using BookingApp.Classes.DB;
using BookingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class LibraryController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private int? Role { get; set; }

        public LibraryController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult LogOut()
        {
            _httpContextAccessor.HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
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

            using var db = new BookingContext();

            var booksAvailable = new AvailableBooksViewModel();

            foreach (var book in db.Books)
            {
                var total = book.Total;

                var totalCurrentBook = db.ReservedBook.Count(x => x.BookId == book.BookId);

                booksAvailable.AvailableBooks.Add(new AvailableBooksModel() { Book = book.Name, Available = total - totalCurrentBook, BookId = book.BookId, Total = book.Total });
            }

            foreach (var reservations in db.ReservedBook)
            {
                var user = db.Users.FirstOrDefault(x => x.UserId == reservations.UserId).Username;

                var book = db.Books.FirstOrDefault(x => x.BookId == reservations.BookId).Name;

                booksAvailable.ReservedBooks.Add(new ReservatedBooksModel() { Book = book, ReservationId = reservations.ReservedBookId, User = user, Date = reservations.ReservedDate.ToString("yyyy-MM-dd hh:mm") });
            }

            foreach (var users in db.Users)
            {
                booksAvailable.Users.Add(new UsersModel { Username = users.Username, Role = users.Role == 0 ? "Admin" : "Reserver", UserId = users.UserId, Registered = users.Registered.ToString("yyyy-MM-dd") });
            }

            return View(booksAvailable);
        }

        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _httpContextAccessor.HttpContext.Session.GetString("user");
                using var db = new BookingContext();

                if (Role == 0 && db.Users.Count(x => x.Role == 0) == 1)
                {
                    return RedirectToAction("Index", "Library", new { user = "oneAdmin" });
                }
                else if (id == db.Users.FirstOrDefault(x => x.Username == user).UserId)
                {
                    return RedirectToAction("Index", "Library", new { error = "sameUser" });
                }
                else if (db.ReservedBook.Count(x => x.UserId == id) == 0)
                {
                    db.Remove(new Users() { UserId = id });
                    db.SaveChanges();

                    return RedirectToAction("Index", "Library", new { user = "deleted" });
                }
                return RedirectToAction("Index", "Library", new { book = "reserved" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { user = "error" });
            }
        }

        public IActionResult DeleteBook(int id)
        {
            try
            {
                using var db = new BookingContext();

                if (db.ReservedBook.Count(x => x.BookId == id) == 0)
                {
                    db.Remove(new Books() { BookId = id });
                    db.SaveChanges();

                    return RedirectToAction("Index", "Library", new { book = "deleted" });
                }
                return RedirectToAction("Index", "Library", new { book = "reserved" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { book = "error" });
            }
        }

        public IActionResult CancelReservation(int id)
        {
            try
            {
                using var db = new BookingContext();

                db.Remove(new ReservedBook() { ReservedBookId = id });
                db.SaveChanges();

                return RedirectToAction("Index", "Library", new { reservation = "canceled" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { reservation = "error" });
            }
        }
    }
}
