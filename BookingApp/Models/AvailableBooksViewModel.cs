using System.Collections.Generic;

namespace BookingApp.Models
{
    public class AvailableBooksViewModel
    {
        public List<AvailableBooksModel> AvailableBooks = new List<AvailableBooksModel>();
        public List<ReservatedBooksModel> ReservedBooks = new List<ReservatedBooksModel>();
        public List<UsersModel> Users = new List<UsersModel>();
    }

    public class AvailableBooksModel
    {
        public int BookId { get; set; }
        public string Book { get; set; }
        public int Available { get; set; }
        public int Total { get; set; }
    }

    public class ReservatedBooksModel
    {
        public int ReservationId { get; set; }
        public string User { get; set; }
        public string Book { get; set; }
        public string Date { get; set; }
    }

    public class UsersModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Registered { get; set; }
    }
}
