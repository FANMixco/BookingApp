using System.Collections.Generic;

namespace BookingApp.Models
{
    public class BookUpdateModel
    {
        public string Book { get; set; }
        public int? Year { get; set; }
        public string Author { get; set; }
        public int ID { get; set; }
        public Dictionary<int, string> Barcodes { get; set; }
        public int Total { get; set; }
    }
}
