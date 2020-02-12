using System;
namespace BookingApp.Classes.DB
{
    public class Users
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public DateTime Registered { get; set; }
    }

    public class Books
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
        public DateTime Registered { get; set; }
    }

    public class ReservedBook
    {
        public int ReservedBookId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime ReservedDate { get; set; }
    }
}
