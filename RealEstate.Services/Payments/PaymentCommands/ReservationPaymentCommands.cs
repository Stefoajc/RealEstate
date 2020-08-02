using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using RealEstate.Model;
using RealEstate.Model.Payment;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;

namespace RealEstate.Services.Payments.PaymentCommands
{
    public class CaparoPaymentCommand : IPaymentCommand
    {
        private ApplicationUserManager UserManager { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }

        public CaparoPaymentCommand(ApplicationUserManager userManager, IUnitOfWork unitOfWork)
        {
            UserManager = userManager;
            UnitOfWork = unitOfWork;
        }

        public async Task Execute(string payedItemId, PayedItemsMeta itemMeta)
        {
            int reservationId = int.Parse(payedItemId);
            var reservation = await UnitOfWork.ReservationsRepository
                .Where(r => r.ReservationId == reservationId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена резервацията!");

            reservation.PaymentStatus = PaymentStatus.CaparoPayed;
            await UnitOfWork.SaveAsync();
        }
    }

    public class FullPaymentCommand : IPaymentCommand
    {
        private ApplicationUserManager UserManager { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }

        public FullPaymentCommand(ApplicationUserManager userManager, IUnitOfWork unitOfWork)
        {
            UserManager = userManager;
            UnitOfWork = unitOfWork;
        }

        public async Task Execute(string payedItemId, PayedItemsMeta itemMeta)
        {
            int reservationId = int.Parse(payedItemId);
            var reservation = await UnitOfWork.ReservationsRepository
                                  .Where(r => r.ReservationId == reservationId)
                                  .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена резервацията!");

            reservation.PaymentStatus = PaymentStatus.FullPayed;
            await UnitOfWork.SaveAsync();
        }
    }
}
