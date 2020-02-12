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

                booksAvailable.AvailableBooks.Add(new AvailableBooksModel() { Book = book.Name, Available = total - totalCurrentBook, BookId = book.BookId });
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
            return View();
        }

        public IActionResult DeleteBook(int id)
        {
            return View();
        }

        public IActionResult CancelReservation(int id)
        {
            return View();
        }
    }
}
