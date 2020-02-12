using System;
using System.Linq;
using BookingApp.Classes.DB;
using BookingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private int? Role { get; set; }

        private static int UserID { get; set; }

        public BookingController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult LogOut()
        {
            _httpContextAccessor.HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Reserve(int id)
        {
            try
            {
                using var db = new BookingContext();

                if (db.ReservedBook.Count(x => x.BookId == id && x.UserId == UserID) < 2)
                {
                    db.Add(new ReservedBook() { BookId = id, UserId = UserID, ReservedDate = DateTime.Now });
                    db.SaveChanges();
                    return RedirectToAction("Index", "Booking", new { message = "reserved" });
                }

                return RedirectToAction("Index", "Booking", new { message = "tooMany" });
            }
            catch
            {
                return RedirectToAction("Index", "Booking", new { message = "error" });
            }
        }

        public IActionResult Cancel(int id)
        {
            try
            {
                using var db = new BookingContext();

                db.Remove(new ReservedBook() { ReservedBookId = id });
                db.SaveChanges();

                return RedirectToAction("Index", "Booking", new { reservation = "canceled" });
            }
            catch
            {
                return RedirectToAction("Index", "Booking", new { reservation = "error" });
            }
        }

        public IActionResult Index()
        {
            Role = _httpContextAccessor.HttpContext.Session.GetInt32("role");

            if (Role == null)
            {
                return RedirectToAction("Index", "Home", new { error = "NoLogin" });
            }
            else if (Role != 1)
            {
                return RedirectToAction("Index", "Home", new { error = "WrongRole" });
            }

            using var db = new BookingContext();

            var booksAvailable = new ReservationsModel();

            foreach (var book in db.Books)
            {
                var total = book.Total;

                var totalCurrentBook = db.ReservedBook.Count(x => x.BookId == book.BookId);

                booksAvailable.AvailableBooks.Add(new AvailableBooksModel() { Book = book.Name, Available = total - totalCurrentBook, BookId = book.BookId });
            }

            UserID = db.Users.FirstOrDefault(x => x.Username == _httpContextAccessor.HttpContext.Session.GetString("user")).UserId;

            foreach (var reservations in db.ReservedBook.Where(x=>x.UserId == UserID))
            {
                var user = db.Users.FirstOrDefault(x => x.UserId == reservations.UserId).Username;

                var book = db.Books.FirstOrDefault(x => x.BookId == reservations.BookId).Name;

                booksAvailable.ReservatedBooks.Add(new ReservatedBooksModel() { Book = book, ReservationId = reservations.ReservedBookId, User = user, Date = reservations.ReservedDate.ToString("yyyy-MM-dd hh:mm") });
            }

            return View(booksAvailable);
        }
    }
}