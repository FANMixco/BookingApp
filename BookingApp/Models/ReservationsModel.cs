using System.Collections.Generic;

namespace BookingApp.Models
{
    public class ReservationsModel
    {
        public List<ReservedBooksModel> ReservedBooks = new List<ReservedBooksModel>();
        public List<AvailableBooksModel> AvailableBooks = new List<AvailableBooksModel>();
    }
}
