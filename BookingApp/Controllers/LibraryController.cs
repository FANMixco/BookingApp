using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using BookingApp.Classes.DB;
using BookingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class LibraryController : Controller
    {
        const int RETURN_DAYS = 2;
        const string SUBJECT_CANCEL = "Reservation canceled";
        const string BODY_CANCEL = "Your reservation of {0} has been canceled.";

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
                return RedirectToAction("Index", "Home", new { error = "noLogin" });
            }
            else if (Role != 0)
            {
                _httpContextAccessor.HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home", new { error = "wrongRole" });
            }

            using var db = new BookingContext();

            var booksAvailable = new AvailableBooksViewModel();

            foreach (var book in db.Books)
            {
                var total = db.BooksCopies.Count(x => x.BookId == book.BookId);

                var totalCurrentBook = db.ReservedBook.Count(x => x.BookId == book.BookId && x.ReturnedDate == null);

                booksAvailable.AvailableBooks.Add(new AvailableBooksModel()
                {
                    Book = book.Name,
                    Available = total - totalCurrentBook,
                    BookId = book.BookId,
                    Total = book.Total,
                    Author = book.Author,
                    PublicationYear = book.PublicationYear
                });
            }

            foreach (var reservations in db.ReservedBook)
            {
                var user = db.Users.FirstOrDefault(x => x.UserId == reservations.UserId).Username;

                var book = db.Books.FirstOrDefault(x => x.BookId == reservations.BookId);

                string barcode = "";

                if (reservations.BooksCopiesId != null)
                {
                    barcode = db.BooksCopies.FirstOrDefault(x => x.BooksCopiesId == reservations.BooksCopiesId).Barcode;
                }

                booksAvailable.ReservedBooks.Add(new ReservedBooksModel()
                {
                    Book = book.Name,
                    Author = book.Author,
                    ReservationId = reservations.ReservedBookId,
                    User = user,
                    Date = reservations.ReservedDate.ToString("yyyy-MM-dd hh:mm"),
                    CollectedDate = reservations.CollectedDate?.ToString("yyyy-MM-dd hh:mm"),
                    ReturnDate = reservations.ReturnDate?.ToString("yyyy-MM-dd hh:mm"),
                    ReturnedDate = reservations.ReturnedDate?.ToString("yyyy-MM-dd hh:mm"),
                    Barcode = barcode
                });
            }

            foreach (var users in db.Users)
            {
                booksAvailable.Users.Add(new UsersModel
                {
                    Username = users.Username,
                    Role = users.Role == 0 ? "Admin" : "Reserver",
                    UserId = users.UserId,
                    Registered = users.Registered.ToString("yyyy-MM-dd"),
                    Email = users.Email
                });
            }

            return View(booksAvailable);
        }

        public IActionResult DeleteUser(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                var user = _httpContextAccessor.HttpContext.Session.GetString("user");
                using var db = new BookingContext();

                if (Role == 0 && db.Users.Count(x => x.Role == 0) == 1)
                {
                    return RedirectToAction("Index", "Library", new { error = "oneAdmin" });
                }
                else if (id == db.Users.FirstOrDefault(x => x.Username == user).UserId)
                {
                    return RedirectToAction("Index", "Library", new { error = "sameUser" });
                }
                else if (db.ReservedBook.Count(x => x.UserId == id) == 0)
                {
                    db.Remove(new Users() { UserId = id });
                    db.SaveChanges();

                    return RedirectToAction("Index", "Library", new { msg = "userDeleted" });
                }
                return RedirectToAction("Index", "Library", new { error = "reserved" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { error = "error" });
            }
        }

        public IActionResult DeleteBook(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                if (db.ReservedBook.Count(x => x.BookId == id) == 0)
                {
                    db.Remove(new Books() { BookId = id });
                    db.SaveChanges();

                    return RedirectToAction("Index", "Library", new { msg = "bookDeleted" });
                }
                return RedirectToAction("Index", "Library", new { error = "reserved" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { error = "error" });
            }
        }

        public IActionResult CollectBook(int id, string barcode)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                var currentBC = db.BooksCopies.FirstOrDefault(x => x.BookId == id && x.Barcode == barcode);

                if (currentBC == null)
                {
                    return RedirectToAction("Index", "Library", new { error = "wrongBarcode" });
                }

                if (db.ReservedBook.Count(x => x.BooksCopiesId == currentBC.BooksCopiesId && x.ReturnedDate == null) > 0)
                {
                    return RedirectToAction("Index", "Library", new { error = "reservedBook" });
                }

                var reservedBook = db.ReservedBook.FirstOrDefault(x => x.ReservedBookId == id);

                reservedBook.BooksCopiesId = currentBC.BooksCopiesId;

                reservedBook.CollectedDate = DateTime.Now;

                reservedBook.ReturnDate = DateTime.Now.AddDays(RETURN_DAYS);

                db.Update(reservedBook);

                db.SaveChanges();

                return RedirectToAction("Index", "Library", new { msg = "collected" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { error = "error" });
            }
        }

        public IActionResult ReturnedBook(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                var reservedBook = db.ReservedBook.FirstOrDefault(x => x.ReservedBookId == id);

                reservedBook.ReturnedDate = DateTime.Now;

                db.Update(reservedBook);

                db.SaveChanges();

                return RedirectToAction("Index", "Library", new { msg = "returned" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { error = "error" });
            }
        }

        public IActionResult CancelReservation(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                //Uncomment it to send emails
                //var reservedBook = db.ReservedBook.FirstOrDefault(x => x.ReservedBookId == id);
                //var email = db.Users.FirstOrDefault(x => x.UserId == reservedBook.UserId).Email;
                //var book = db.Books.FirstOrDefault(x => x.BookId == reservedBook.BookId).Name;
                //MailLibrary.MailNotifications.SendEmail(email, SUBJECT_CANCEL, string.Format(BODY_CANCEL, book));

                db.Remove(new ReservedBook() { ReservedBookId = id });
                db.SaveChanges();

                return RedirectToAction("Index", "Library", new { msg = "reservationCanceled" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { error = "error" });
            }
        }
    }
}
