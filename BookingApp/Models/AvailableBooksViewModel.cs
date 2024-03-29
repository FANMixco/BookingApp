﻿using System.Collections.Generic;

namespace BookingApp.Models
{
    public class AvailableBooksViewModel
    {
        public List<AvailableBooksModel> AvailableBooks = new();
        public List<ReservedBooksModel> ReservedBooks = new();
        public List<UsersModel> Users = new();
    }

    public class AvailableBooksModel
    {
        public int BookId { get; set; }
        public string Book { get; set; }
        public string Author { get; set; }
        public int? PublicationYear { get; set; }
        public int Available { get; set; }
        public int Total { get; set; }
        public string Copies { get; set; }
    }

    public class ReservedBooksModel
    {
        public int ReservationId { get; set; }
        public string User { get; set; }
        public string Book { get; set; }
        public string Barcode { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string CollectedDate { get; set; }
        public string ReturnDate { get; set; }
        public string ReturnedDate { get; set; }
    }

    public class UsersModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Registered { get; set; }
    }
}
