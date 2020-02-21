﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApp.Classes.DB
{
    public class Users
    {
        [Key]
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int Role { get; set; }
        [Required]
        public DateTime Registered { get; set; }
    }

    public class Books
    {
        [Key]
        [Required]
        public int BookId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Total { get; set; }
        [Required]
        public string Author { get; set; }
        public int? PublicationYear { get; set; }
        [Required]
        public DateTime Registered { get; set; }
    }

    public class ReservedBook
    {
        [Key]
        [Required]
        public int ReservedBookId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BookId { get; set; }
        public int? BooksCopiesId { get; set; }
        [Required]
        public DateTime ReservedDate { get; set; }
        public DateTime? CollectedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
    }

    public class BooksCopies
    {
        [Key]
        [Required]
        public int BooksCopiesId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public string Barcode { get; set; }
        [Required]
        public DateTime Registered { get; set; }
    }
}
