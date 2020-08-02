#define DEVELOPEMENT

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Services.Interfaces;

namespace RealEstate.Services
{
    public class IdentitySmsService : IIdentityMessageService
    {
        private ISmsService SmsManager;

        public IdentitySmsService(ISmsService smsService) 
        {
            SmsManager = smsService;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            await SmsManager.SendSmsAsync(message.Subject, message.Subject);
        }
    }


    public class IdentityEmailService : IIdentityMessageService
    {
        public IEmailService EmailManager;

        public IdentityEmailService(IEmailService emailService)
        {
            EmailManager = emailService;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            await EmailManager.SendEmailAsync(message.Destination, message.Subject, message.Body, true);
        }
    }
}
