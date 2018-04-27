using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using RealEstate.Services.Interfaces;

namespace RealEstate.Services
{
    public class GmailMailService : IEmailService
    {
        private readonly string _emailDomain = ConfigurationManager.AppSettings["GmailDomain"];
        private readonly int _emailServerPort = int.Parse(ConfigurationManager.AppSettings["GmailSmtpPort"]);
        private readonly string _emailUserName = ConfigurationManager.AppSettings["GmailUserName"];
        private readonly string _emailPassword = ConfigurationManager.AppSettings["GmailPassword"];

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailDomain, _emailServerPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(_emailUserName, _emailPassword);

                using (MailMessage message = new MailMessage(_emailUserName, to, subject, body))
                {
                    message.IsBodyHtml = isBodyHtml;
                    await smtpClient.SendMailAsync(message);
                }

            }
        }
    }

    public class AbvMailService : IEmailService
    {
        private readonly string _emailDomain = ConfigurationManager.AppSettings["AbvDomain"];
        private readonly int _emailServerPort = int.Parse(ConfigurationManager.AppSettings["AbvSmtpPort"]);
        private readonly string _emailUserName = ConfigurationManager.AppSettings["AbvUserName"];
        private readonly string _emailPassword = ConfigurationManager.AppSettings["AbvPassword"];

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailDomain, _emailServerPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(_emailUserName, _emailPassword);
                smtpClient.Timeout = 2000;

                using (MailMessage message = new MailMessage(_emailUserName, to, subject, body))
                {
                    message.IsBodyHtml = isBodyHtml;
                    
                    await smtpClient.SendMailAsync(message);
                }

            }
        }
    }
}
