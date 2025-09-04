using System.Net;
using System.Net.Mail;

namespace FlowForge.Services
{
    public static class EmailService
    {
        public static void SendEmail(string to, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.office365.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("ashley.d@pixelogicmedia.com", "Passw0rdAdm1n=-098765");

                var mail = new MailMessage("your_email@domain.com", to, subject, body);
                client.Send(mail);
            }
        }
    }
}
