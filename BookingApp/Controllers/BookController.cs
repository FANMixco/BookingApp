using System.Linq;
using BookingApp.Classes.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int? Role { get; set; }

        public BookController(IHttpContextAccessor httpContextAccessor)
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
            return View();
        }

        [HttpPost]
        public IActionResult Index(string book, string copies)
        {
            using var db = new BookingContext();

            if (db.Books.Count(x => x.Name == book) == 0)
            {
                db.Add(new Books() { Name = book, Total = int.Parse(copies) });
                db.SaveChanges();
                return RedirectToAction("Index", "Book", new { message = "Added" });
            }
            else
            {
                return RedirectToAction("Index", "Book", new { error = "WrongName" });
            }
        }

        [HttpPost]
        public IActionResult Update(int id, string book, string copies)
        {
            using var db = new BookingContext();

            var bookTotal = db.Books.Count(x => x.Name == book);
            var isOldName = bookTotal == 1;

            var totalReserved = db.ReservedBook.Count(x => x.BookId == id);

            var makesCopiesLogic = (int.Parse(copies) - totalReserved) >= 0;

            //bookTotal == 0 is a new name
            if ((isOldName || bookTotal == 0) && makesCopiesLogic)
            {
                UpdateBook(id, book, copies, db);
                return RedirectToAction("Update", "Book", new { id, message = "Updated" });
            }
            else if (!makesCopiesLogic)
            {
                return RedirectToAction("Update", "Book", new { id, error = "LessBooks" });
            }
            else if (bookTotal > 1)
            {
                return RedirectToAction("Update", "Book", new { id, error = "WrongName" });
            }
            else
            {
                return RedirectToAction("Update", "Home", new { id, error = "WrongRole" });
            }
        }

        private static void UpdateBook(int id, string book, string copies, BookingContext db)
        {
            var bookData = db.Books.FirstOrDefault(x => x.BookId == id);
            bookData.Name = book;
            bookData.Total = int.Parse(copies);

            db.Update(bookData);
            db.SaveChanges();
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
            using var db = new BookingContext();
            var book = db.Books.FirstOrDefault(x => x.BookId == id);
            if (book != null)
            {
                var model = new Models.BookUpdateModel() { ID = id, Book = book.Name, Copies = book.Total };
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Library", new { error = "WrongBook" });
            }
        }
    }
}
