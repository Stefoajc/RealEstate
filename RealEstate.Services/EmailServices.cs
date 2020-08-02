using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using RealEstate.Services.Interfaces;

namespace RealEstate.Services
{
    public class GmailMailService : IEmailService
    {
        private readonly string emailDomain = ConfigurationManager.AppSettings["GmailDomain"];
        private readonly int emailServerPort = int.Parse(ConfigurationManager.AppSettings["GmailSmtpPort"]);
        private readonly string emailUserName = ConfigurationManager.AppSettings["GmailUserName"];
        private readonly string emailPassword = ConfigurationManager.AppSettings["GmailPassword"];

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            using (SmtpClient smtpClient = new SmtpClient(emailDomain, emailServerPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(emailUserName, emailPassword);

                using (MailMessage message = new MailMessage(emailUserName, to, subject, body))
                {
                    message.IsBodyHtml = isBodyHtml;
                    await smtpClient.SendMailAsync(message);
                }

            }
        }
    }

    public class AbvMailService : IEmailService
    {
        private readonly string emailDomain = ConfigurationManager.AppSettings["AbvDomain"];
        private readonly int emailServerPort = int.Parse(ConfigurationManager.AppSettings["AbvSmtpPort"]);
        private readonly string emailUserName = ConfigurationManager.AppSettings["AbvUserName"];
        private readonly string emailPassword = ConfigurationManager.AppSettings["AbvPassword"];

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            using (SmtpClient smtpClient = new SmtpClient(emailDomain, emailServerPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(emailUserName, emailPassword);
                smtpClient.Timeout = 2000;

                using (MailMessage message = new MailMessage(emailUserName, to, subject, body))
                {
                    message.IsBodyHtml = isBodyHtml;
                    
                    await smtpClient.SendMailAsync(message);
                }

            }
        }
    }


    public class NoReplyMailService : IEmailService
    {
        private readonly string emailDomain = ConfigurationManager.AppSettings["CompanySmtpHost"];
        private readonly int emailServerPort = int.Parse(ConfigurationManager.AppSettings["CompanySmtpPort"]);
        private readonly string emailUserName = ConfigurationManager.AppSettings["No-ReplyEmail"];
        private readonly string emailPassword = ConfigurationManager.AppSettings["CompanyMailsPassword"];

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            using (SmtpClient smtpClient = new SmtpClient(emailDomain, emailServerPort))
            {
                smtpClient.EnableSsl = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(emailUserName, emailPassword);

                using (MailMessage message = new MailMessage(emailUserName, to, subject, body))
                {
                    message.IsBodyHtml = isBodyHtml;
                    await smtpClient.SendMailAsync(message);
                }

            }
        }
    }

    public class OfficeMailService : IEmailService
    {
        private readonly string emailDomain = ConfigurationManager.AppSettings["CompanySmtpHost"];
        private readonly int emailServerPort = int.Parse(ConfigurationManager.AppSettings["CompanySmtpPort"]);
        private readonly string emailUserName = ConfigurationManager.AppSettings["OfficeEmail"];
        private readonly string emailPassword = ConfigurationManager.AppSettings["CompanyMailsPassword"];

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            using (SmtpClient smtpClient = new SmtpClient(emailDomain, emailServerPort))
            {
                smtpClient.EnableSsl = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(emailUserName, emailPassword);

                using (MailMessage message = new MailMessage(emailUserName, to, subject, body))
                {
                    message.IsBodyHtml = isBodyHtml;
                    await smtpClient.SendMailAsync(message);
                }

            }
        }
    }
}
