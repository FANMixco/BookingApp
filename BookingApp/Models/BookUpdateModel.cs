﻿namespace BookingApp.Models
{
    public class BookUpdateModel
    {
        public string Book { get; set; }
        public int Copies { get; set; }
        public int? Year { get; set; }
        public string Author { get; set; }
        public int ID { get; set; }
    }
}
