using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using BookingApp.DB.Classes.DB;

namespace BookingApp.MailLibrary
{
    public static class MailNotifications
    {
        private const string PASS_KEY = "!emJ(?w)Sx_5S-3L";

        public static void SendEmail(string email, string subject, string body)
        {
            try
            {
                using var db = new BookingContext();

                var settings = db.Settings.FirstOrDefault();

                if (string.IsNullOrEmpty(settings.Email))
                {
                    throw new Exception("Empty email");
                }

                // Credentials
                var credentials = new NetworkCredential(settings.Email, DB.EncryptionMails.DecryptString(PASS_KEY, settings.PasswordHost));
                // Mail message
                MailMessage mail = new()
                {
                    From = new MailAddress(settings.Email),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(new MailAddress(email));
                // Smtp client
                SmtpClient client = new()
                {
                    Port = settings.Port.Value,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = settings.MailHost,
                    EnableSsl = true,
                    Credentials = credentials
                };
                client.Send(mail);
            }
            catch
            {

            }
        }
    }
}
