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
        [Inject]
        public RealEstateDbContext RealEstateDbContext { get; set; }
        [Inject]
        public IEmailService EmailsManager { get; set; }

        public async Task Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException();
            if (!RealEstateDbContext.EmailList.Any(e => e.EmailAddress == email))
            {
                RealEstateDbContext.EmailList.Add(new EmailList { EmailAddress = email });
                await RealEstateDbContext.SaveChangesAsync();
            }
        }

        public async Task UnSubscribe(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException();

            var emailToRemove = await RealEstateDbContext.EmailList.Where(e => e.EmailId == id).FirstOrDefaultAsync();
            RealEstateDbContext.EmailList.Remove(emailToRemove);
            await RealEstateDbContext.SaveChangesAsync();
        }

        public async Task SendBroadcastMail(string subject, string body)
        {
            var mailList = await RealEstateDbContext.EmailList.ToListAsync();


            foreach (var email in mailList)
            {
                //Unsubscribe 
                body = body +
                       "</br></br> <p style=\"margin: 0px; padding: 0px; color:#8a8a8a;font-size:14px;line-height:18px;\">Ако искаш да изключиш всички известия, <a href=\" " + ConfigurationManager.AppSettings["Domain"] + "maillist/removemailfromlist/" + email.EmailId + " \" style=\"color:#1d66e5;text-decoration:none;\">кликни тук</a></p>";

                await EmailsManager.SendEmailAsync(email.EmailAddress, subject, body, true);
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
