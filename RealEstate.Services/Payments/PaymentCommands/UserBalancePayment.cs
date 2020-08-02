using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Model;
using RealEstate.Model.Payment;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services.Payments.PaymentCommands
{
    internal class UserBalancePayment : IPaymentCommand
    {
        public ApplicationUserManager UserManager { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }

        public UserBalancePayment(ApplicationUserManager userManager, IUnitOfWork unitOfWork)
        {
            UserManager = userManager;
            UnitOfWork = unitOfWork;
        }

        //
        public Task Execute(string payedItemId, PayedItemsMeta itemMeta)
        {
            return Task.CompletedTask;
        }
    }
}