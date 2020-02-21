using System.Net;
using System.Net.Mail;

namespace BookingApp.MailLibrary
{
    public static class MailNotifications
    {
        const string HOST = "smtp.host.com";
        const string PERSONAL_EMAIL = "your-email@host.com";
        const string PASSWORD = "your-password";
        const int PORT = 0;

        public static void SendEmail(string email, string subject, string body)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(PERSONAL_EMAIL, PASSWORD);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(PERSONAL_EMAIL),
                    Subject = subject,
                    Body = body
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(email));
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = PORT,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = HOST,
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
