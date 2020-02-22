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
            switch (args[0])
            {
                case "reminder":
                    SendReminder();
                    break;
                case "oldReservation":
                    OldReservations();
                    break;
            }
        }

        private static void OldReservations()
        {
            var db = new BookingContext();

            var maxTime = db.Settings.FirstOrDefault().MaxTime;

            foreach (var reservedBook in db.ReservedBook.Where(x => DateTime.Now > x.ReservedDate.AddDays(maxTime)))
            {
                var email = db.Users.FirstOrDefault(x => x.UserId == reservedBook.UserId).Email;

                MailNotifications.SendEmail(email, "Your reservation was auto-cancelled because the book was not collected on time.", "Reservation cancelled");

                db.Remove(new ReservedBook() { ReservedBookId = reservedBook.ReservedBookId });
                db.SaveChanges();
            }
        }

        private static void SendReminder()
        {
            var db = new BookingContext();

            var maxTime = db.Settings.FirstOrDefault().MaxTime;

            foreach (var reservedBook in db.ReservedBook.Where(x => x.ReturnDate != null && x.ReturnedDate == null))
            {
                var email = db.Users.FirstOrDefault(x => x.UserId == reservedBook.UserId).Email;

                if (reservedBook.ReturnDate.Value.Subtract(DateTime.Now).TotalDays < 0)
                {
                    MailNotifications.SendEmail(email, "Please return the book. Your time has expired.", "WARNING: OVERTIME!");
                }
                else if (reservedBook.ReturnDate.Value.Subtract(DateTime.Now).TotalDays < maxTime)
                {
                    MailNotifications.SendEmail(email, $"You have to return your book on: {reservedBook.ReturnDate}.", "Library reminder");
                }
            }
        }
    }
}
