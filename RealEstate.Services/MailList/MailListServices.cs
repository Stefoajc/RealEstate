using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using RealEstate.Data;
using RealEstate.Model.MailList;
using RealEstate.Services.Interfaces;

namespace RealEstate.Services.MailList
{
    public class MailListServices
    {
        private RealEstateDbContext realEstateDbContext;
        private IEmailService emailsManager;
        private string createUnsubscribeLink(string emailId) 
            => "</br></br> <p style=\"margin: 0px; padding: 0px; color:#8a8a8a;font-size:14px;line-height:18px;\">Ако искаш да изключиш всички известия, <a href=\" " + ConfigurationManager.AppSettings["Domain"] + "maillist/removemailfromlist/" + emailId + " \" style=\"color:#1d66e5;text-decoration:none;\">кликни тук</a></p>";

        [Inject]
        public MailListServices(RealEstateDbContext realEstateDbContext, IEmailService emailsManager)
        {
            this.realEstateDbContext = realEstateDbContext;
            this.emailsManager = emailsManager;
        }        

        public async Task Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email) || realEstateDbContext.EmailList.Any(e => e.EmailAddress == email))
                return;

            realEstateDbContext.EmailList.Add(new EmailList { EmailAddress = email });
            await realEstateDbContext.SaveChangesAsync();
        }

        public async Task Unsubscribe(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var emailToRemove = await realEstateDbContext.EmailList.FirstOrDefaultAsync(e => e.EmailId == id);

            if(emailToRemove != null)
            {
                realEstateDbContext.EmailList.Remove(emailToRemove);
                await realEstateDbContext.SaveChangesAsync();
            }
        }

        public async Task SendBroadcastMail(string subject, string body)
        {
            var mailList = await realEstateDbContext.EmailList.ToListAsync();

            foreach (var email in mailList)
            {
                await emailsManager.SendHtmlEmailAsync(email.EmailAddress, subject, body + createUnsubscribeLink(email.EmailId));
            }
        }
    }


    public class EmailDTO
    {
        [Required(ErrorMessage = "Въведете е-мейл")]
        [EmailAddress(ErrorMessage = "Въведете правилен е-мейл")]
        public string email { get; set; }
    }
}
