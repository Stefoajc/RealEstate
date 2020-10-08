using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using RealEstate.Data;
using RealEstate.Model.Contact;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC.Contact;

namespace RealEstate.Services.Contact
{
    public class ContactMessageServices
    {
        [Inject]
        public RealEstateDbContext RealEstateDbContext { get; set; }
        [Inject]
        public IEmailService EmailsManager { get; set; }

        public async Task AddContactMessage(ContactMessageViewModel messageTo)
        {
            ContactMessages message = new ContactMessages
            {
                Name = messageTo.Name,
                Email = messageTo.Email,
                Title = messageTo.Subject,
                PhoneNumber = messageTo.PhoneNumber,
                Message = messageTo.Message
            };

            RealEstateDbContext.ContactMessages.Add(message);
            await RealEstateDbContext.SaveChangesAsync();

            await EmailsManager.SendHtmlEmailAsync(message.Email, "Съобщение изпратено до sProperties","Благодаря за обратната връзка, която ни пратихте. Оценяваме времето, което отделихте. Поздрави, sProperties.", true);

        }

        public async Task<List<ContactMessageViewModel>> ListMessages()
        {
            var messages = await RealEstateDbContext.ContactMessages
                .OrderByDescending(m => m.CreatedOn)
                .Select(c => new ContactMessageViewModel
            {
                    Name = c.Name,
                    Email = c.Email,
                    Subject = c.Title,
                    PhoneNumber = c.PhoneNumber,
                    Message = c.Message
            })
            .ToListAsync();

            return messages;
        }
    }
}
