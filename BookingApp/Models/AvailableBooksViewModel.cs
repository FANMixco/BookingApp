using System.Collections.Generic;

namespace BookingApp.Models
{
    public class AvailableBooksViewModel
    {
        public List<AvailableBooksModel> AvailableBooks = new List<AvailableBooksModel>();
        public List<ReservatedBooksModel> ReservedBooks = new List<ReservatedBooksModel>();
    }

    public class AvailableBooksModel
    {
        public int BookId { get; set; }
        public string Book { get; set; }
        public int Available { get; set; }
    }

    public class ReservatedBooksModel
    {
        public int ReservationId { get; set; }
        public string User { get; set; }
        public string Book { get; set; }
        public string Date { get; set; }
    }
}
