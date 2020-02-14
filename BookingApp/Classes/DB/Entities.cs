using System;
namespace BookingApp.Classes.DB
{
    public class Users
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public DateTime Registered { get; set; }
    }

    public class Books
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
        public string Author { get; set; }
        public int? PublicationYear { get; set; }
        public DateTime Registered { get; set; }
    }

    public class ReservedBook
    {
        public int ReservedBookId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime ReservedDate { get; set; }
        public DateTime? CollectedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
    }
}
