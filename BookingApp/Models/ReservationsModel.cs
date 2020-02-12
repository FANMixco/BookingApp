using System.Collections.Generic;

namespace BookingApp.Models
{
    public class ReservationsModel
    {
        public List<ReservatedBooksModel> ReservatedBooks = new List<ReservatedBooksModel>();
        public List<AvailableBooksModel> AvailableBooks = new List<AvailableBooksModel>();
    }
}
