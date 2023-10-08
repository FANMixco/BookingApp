using System.Collections.Generic;
using System.Linq;
using BookingApp.DB.Classes.DB;
using BookingApp.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Authorize(Roles.ADMIN)]
    public class BookController : Controller
    {
        public BookController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(string book, string author, string year, IList<string> barcodes)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Library");
            }

            switch (Insert(book, author, year, barcodes))
            {
                case true:
                    return RedirectToAction("Index", "Book", new { msg = "added" });
                case false:
                    return RedirectToAction("Index", "Book", new { error = "wrongName" });
                case null:
                    return RedirectToAction("Index", "Library", new { error = "error" });
            }
        }

        public bool? Insert(string book, string author, string year, IList<string> barcodes)
        {
            try
            {
                using var db = new BookingContext();

                if (!db.Books.Any(x => x.Name == book) && !string.IsNullOrEmpty(book) && !string.IsNullOrEmpty(author))
                {
                    int? yearVal = null;

                    if (!string.IsNullOrEmpty(year))
                    {
                        yearVal = int.Parse(year);
                    }

                    db.Add(new Books() { Name = book, Author = author, PublicationYear = yearVal });

                    db.SaveChanges();

                    var lastID = db.Books.OrderByDescending(x => x.BookId).FirstOrDefault().BookId;
                    InsertBarcode(barcodes, lastID);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return null;
            }
        }

        private static void InsertBarcode(IList<string> barcodes, int bookId)
        {
            foreach (var barcode in barcodes)
            {
                using var db2 = new BookingContext();
                db2.Add(new BooksCopies() { Barcode = barcode, BookId = bookId, Registered = System.DateTime.Now });
                db2.SaveChanges();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Update(int id, string book, string author, string year, IList<string> barcodes)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Library");
                }

                using var db = new BookingContext();

                var bookTotal = db.Books.Count(x => x.Name == book);
                var isRenamed = bookTotal == 0;
                var isOldName = bookTotal == 1;

                var totalReserved = db.ReservedBook.Count(x => x.BookId == id);

                var copiesMakeLogic = (db.BooksCopies.Count(x => x.BookId == id) - totalReserved) >= 0;

                //bookTotal == 0 is a new name
                if ((isOldName || isRenamed) && copiesMakeLogic)
                {
                    int? yearVal = null;

                    if (!string.IsNullOrEmpty(year))
                    {
                        yearVal = int.Parse(year);
                    }

                    UpdateBook(id, book, author, yearVal, barcodes, db);
                    return RedirectToAction("Update", "Book", new { id, msg = "updated" });
                }
                else if (!copiesMakeLogic)
                {
                    return RedirectToAction("Update", "Book", new { id, error = "lessBooks" });
                }
                else if (bookTotal > 1)
                {
                    return RedirectToAction("Update", "Book", new { id, error = "wrongName" });
                }
                else
                {
                    return RedirectToAction("Index", "Library", new { id, error = "wrongBook" });
                }
            }
            catch
            {
                return RedirectToAction("Update", "Home", new { id, error = "error" });
            }
        }

        //Can be rented could be a solution to the point of a stolen book
        private static void UpdateBook(int id, string book, string author, int? year, IList<string> barcodes, BookingContext db)
        {
            var bookData = db.Books.FirstOrDefault(x => x.BookId == id);
            bookData.Name = book;
            bookData.Author = author;
            bookData.PublicationYear = year;

            db.Update(bookData);
            db.SaveChanges();

            var currentBarcodes = db.BooksCopies.Where(x => x.BookId == id).Select(x => x.Barcode).ToList();

            var newBarcodes = barcodes.Except(currentBarcodes).ToList();

            InsertBarcode(newBarcodes, id);

            var removedBarcodes = currentBarcodes.Except(barcodes).ToList();

            foreach (var barcode in removedBarcodes)
            {
                var booksCopiesId = db.BooksCopies.FirstOrDefault(x => x.Barcode == barcode).BooksCopiesId;
                if (!db.ReservedBook.Any(x => x.BooksCopiesId == booksCopiesId))
                {
                    using var db2 = new BookingContext();
                    db2.Remove(new BooksCopies { BooksCopiesId = booksCopiesId });
                    db2.SaveChanges();
                }
            }
        }

        public IActionResult Update(int id)
        {
            using var db = new BookingContext();
            var book = db.Books.FirstOrDefault(x => x.BookId == id);
            if (book != null)
            {
                var barcodes = (from bookCopies in db.BooksCopies where bookCopies.BookId == id orderby bookCopies.BooksCopiesId select new { id = bookCopies.BooksCopiesId, barcode = bookCopies.Barcode }).ToDictionary(item => item.id, item => item.barcode);

                return View(new Models.BookUpdateModel() {
                    ID = id,
                    Book = book.Name,
                    Author = book.Author,
                    Year = book.PublicationYear,
                    Barcodes = barcodes,
                    Total = barcodes.Count
                });
            }
            else
            {
                return RedirectToAction("Index", "Library", new { error = "wrongBook" });
            }
        }
    }
}
