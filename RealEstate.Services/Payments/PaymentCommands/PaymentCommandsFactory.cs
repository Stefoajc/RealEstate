using System;
using System.Reflection;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services.Payments.PaymentCommands
{
    public class PaymentCommandsFactory
    {
        private const string BaseNamespace = @"RealEstate.Services.Payments.PaymentCommands.";
        private IUnitOfWork UnitOfWork { get; set; }
        private ApplicationUserManager UserManager { get; set; }

        public PaymentCommandsFactory(IUnitOfWork unitOfWork, ApplicationUserManager userManager)
        {
            UnitOfWork = unitOfWork;
            UserManager = userManager;
        }

        public IPaymentCommand Create(string paymentCommandName)
        {
            paymentCommandName = BaseNamespace + paymentCommandName;
            Type type = Assembly.GetExecutingAssembly().GetType(paymentCommandName);
            object obj = Activator.CreateInstance(type, UserManager, UnitOfWork);
            return obj as IPaymentCommand;
        }
    }
}