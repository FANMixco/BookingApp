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
                RedirectToAction("Index", "Library", new { error = "WrongBook" });
            }
            return View();
        }
    }
}
