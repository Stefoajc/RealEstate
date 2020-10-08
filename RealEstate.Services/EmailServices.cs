using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using RealEstate.Services.Interfaces;

namespace RealEstate.Services
{
    public class GmailMailService : BaseEmailService
    {
        public GmailMailService()
            : base(ConfigurationManager.AppSettings["GmailDomain"],
                  int.Parse(ConfigurationManager.AppSettings["GmailSmtpPort"]),
                  ConfigurationManager.AppSettings["GmailUserName"],
                  ConfigurationManager.AppSettings["GmailPassword"])
        { }
    }

    public class AbvMailService : BaseEmailService
    {
        public AbvMailService()
            : base(ConfigurationManager.AppSettings["AbvDomain"],
                  int.Parse(ConfigurationManager.AppSettings["AbvSmtpPort"]),
                  ConfigurationManager.AppSettings["AbvUserName"],
                  ConfigurationManager.AppSettings["AbvPassword"])
        { }
    }


    public class NoReplyMailService : BaseEmailService
    {
        public NoReplyMailService()
        : base(ConfigurationManager.AppSettings["CompanySmtpHost"],
              int.Parse(ConfigurationManager.AppSettings["CompanySmtpPort"]),
              ConfigurationManager.AppSettings["ReplyEmail"],
              ConfigurationManager.AppSettings["CompanyMailsPassword"])
        { }
    }

    public class OfficeMailService : BaseEmailService
    {
        public OfficeMailService()
        : base(ConfigurationManager.AppSettings["CompanySmtpHost"],
              int.Parse(ConfigurationManager.AppSettings["CompanySmtpPort"]),
              ConfigurationManager.AppSettings["OfficeEmail"],
              ConfigurationManager.AppSettings["CompanyMailsPassword"])
        { }
    }

    public abstract class BaseEmailService : IEmailService
    {
        private readonly string emailDomain;
        private readonly int emailServerPort;
        private readonly string emailUserName;
        private readonly string emailPassword;

        public BaseEmailService(string emailDomain, int emailServerPort, string emailUserName, string emailPassword)
        {
            this.emailDomain = emailDomain;
            this.emailServerPort = emailServerPort;
            this.emailUserName = emailUserName;
            this.emailPassword = emailPassword;
        }

        public Task SendHtmlEmailAsync(string to, string subject, string body)
            => SendEmailAsync(to, subject, body, isBodyHtml: true);
        public Task SendPlainTextEmailAsync(string to, string subject, string body)
             => SendEmailAsync(to, subject, body, isBodyHtml: false);

        private async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml, bool enableSsl = false)
        {
            using (SmtpClient smtpClient = new SmtpClient(emailDomain, emailServerPort))
            {
                smtpClient.EnableSsl = enableSsl;
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
