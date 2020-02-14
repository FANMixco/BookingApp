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
        const string HOST = "smtp.host.com";
        const string PERSONAL_EMAIL = "your-email@host.com";
        const string PASSWORD = "your-password";
        const string SUBJECT = "Reservation canceled";
        const string BODY = "Your reservation of {0} has been canceled.";
        const int PORT = 0;

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
                var total = book.Total;

                var totalCurrentBook = db.ReservedBook.Count(x => x.BookId == book.BookId);

                booksAvailable.AvailableBooks.Add(new AvailableBooksModel() { Book = book.Name, Available = total - totalCurrentBook, BookId = book.BookId, Total = book.Total, Author = book.Author, PublicationYear = book.PublicationYear });
            }

            foreach (var reservations in db.ReservedBook)
            {
                var user = db.Users.FirstOrDefault(x => x.UserId == reservations.UserId).Username;

                var book = db.Books.FirstOrDefault(x => x.BookId == reservations.BookId);

                booksAvailable.ReservedBooks.Add(new ReservedBooksModel() {
                    Book = book.Name,
                    Author = book.Author,
                    ReservationId = reservations.ReservedBookId,
                    User = user,
                    Date = reservations.ReservedDate.ToString("yyyy-MM-dd hh:mm"),
                    CollectedDate = reservations.CollectedDate?.ToString("yyyy-MM-dd hh:mm"),
                    ReturnDate = reservations.ReturnDate?.ToString("yyyy-MM-dd hh:mm"),
                    ReturnedDate = reservations.ReturnedDate?.ToString("yyyy-MM-dd hh:mm")
                });
            }

            foreach (var users in db.Users)
            {
                booksAvailable.Users.Add(new UsersModel { Username = users.Username, Role = users.Role == 0 ? "Admin" : "Reserver", UserId = users.UserId, Registered = users.Registered.ToString("yyyy-MM-dd"), Email = users.Email });
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

        public IActionResult CancelReservation(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                /*//Uncomment it to send emails
                var reservedBook = db.ReservedBook.FirstOrDefault(x => x.ReservedBookId == id);
                var email = db.Users.FirstOrDefault(x => x.UserId == reservedBook.UserId).Email;
                var book = db.Books.FirstOrDefault(x => x.BookId == reservedBook.BookId).Name;
                SendCancelationEmail(email, book);*/

                db.Remove(new ReservedBook() { ReservedBookId = id });
                db.SaveChanges();

                return RedirectToAction("Index", "Library", new { msg = "reservationCanceled" });
            }
            catch
            {
                return RedirectToAction("Index", "Library", new { error = "error" });
            }
        }

        private void SendCancelationEmail(string email, string book)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(PERSONAL_EMAIL, PASSWORD);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(PERSONAL_EMAIL),
                    Subject = SUBJECT,
                    Body = string.Format(BODY, book)
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(email));
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = PORT,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = HOST,
                    EnableSsl = true,
                    Credentials = credentials
                };
                client.Send(mail);
            }
            catch
            {

            }
        }
    }
}
