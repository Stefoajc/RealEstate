using System;
using System.Reflection;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services.Payments.PaymentNotificationCommands
{
    public class PaymentNotificationCommandsFactory
    {
        private const string BaseNamespace = @"RealEstate.Services.Payments.PaymentNotificationCommands.";
        private const string CommandSufix = @"";
        private IUnitOfWork UnitOfWork { get; set; }
        private ApplicationUserManager UserManager { get; set; }

        public PaymentNotificationCommandsFactory(IUnitOfWork unitOfWork, ApplicationUserManager userManager)
        {
            UnitOfWork = unitOfWork;
            UserManager = userManager;
        }

        public IPaymentNotification Create(string payedItemName)
        {
            payedItemName = BaseNamespace + payedItemName + "NotificationCommand";
            Type type = Assembly.GetExecutingAssembly().GetType(payedItemName);
            object obj = Activator.CreateInstance(type, UnitOfWork);
            return obj as IPaymentNotification;
        }

    }
}
