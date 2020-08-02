using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Ninject;
using RealEstate.Model.Payment;
using RealEstate.Repositories;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC.Payments;

namespace RealEstate.Services.Payments
{
    public class PayedItemServices : BaseService
    {
        [Inject]
        public PayedItemServices(IUnitOfWork unitOfWork, ApplicationUserManager userManager) : base(unitOfWork, userManager)
        {
        }

        public async Task<PayedItemsViewModel> Get(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Невалиден код на плащаният елемент");
            }

            var payedItem = await unitOfWork.PayedItemsRepository
                .Where(p => p.Code == code)
                .Select(p => new PayedItemsViewModel
                {
                    Amount = p.Amount,
                    Description = p.Description
                })
                .FirstOrDefaultAsync();

            return payedItem;
        }

        public async Task<PayedItemsMeta> GetPayedItemInfo(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Невалиден код на плащаният елемент");
            }

            var payedItem = await unitOfWork.PayedItemsRepository
                .Where(p => p.Code == code)
                .FirstOrDefaultAsync();

            return payedItem;
        }

    }
}