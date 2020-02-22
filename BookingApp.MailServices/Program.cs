using System;
using System.Linq;
using BookingApp.DB.Classes.DB;
using BookingApp.MailLibrary;

namespace BookingApp.MailServices
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "reminder")
            {
                SendReminder();
            }
        }

        private static void SendReminder()
        {
            var db = new BookingContext();

            foreach (var reservedBook in db.ReservedBook.Where(x => x.ReturnDate != null && x.ReturnedDate == null))
            {
                var email = db.Users.FirstOrDefault(x => x.UserId == reservedBook.UserId).Email;

                if (reservedBook.ReturnDate.Value.Subtract(DateTime.Now).TotalDays < 0)
                {
                    MailNotifications.SendEmail(email, "Please return the book. Your time has expired.", "WARNING: OVERTIME!");
                }
                else if (reservedBook.ReturnDate.Value.Subtract(DateTime.Now).TotalDays < db.Settings.FirstOrDefault().MaxTime)
                {
                    MailNotifications.SendEmail(email, $"You have to return your book on: {reservedBook.ReturnDate}.", "Library reminder");
                }
            }
        }
    }
}
