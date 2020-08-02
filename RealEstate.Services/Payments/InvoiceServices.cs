using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Ninject;
using RealEstate.Model.Payment;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC.Payments;

namespace RealEstate.Services.Payments
{
    public class InvoiceServices : BaseService
    {
        [Inject]
        public InvoiceServices(IUnitOfWork unitOfWork, ApplicationUserManager userManager) : base(unitOfWork, userManager)
        {
        }

        public async Task<ePayQueryViewModel> CreateInvoiceForSending(decimal amount, string polimorphicId, string description, string payedItemCode, string currency = "BGN")
        {
            if (amount == 0 || polimorphicId == null)
            {
                throw new ArgumentException();
            }

            //Keeping track of invoice numbers for a year
            var largestInvoiceNumberForThisYear = await unitOfWork.InvoicesRepository
                .Where(i => i.CreatedOn.Year == DateTime.Now.Year)
                .MaxAsync(i => (int?)i.InvoiceNumber) ?? 0;

            Invoices invoice = new Invoices
            {
                Amount = amount,
                Currency = currency,
                ExpirationDate = DateTime.Now.AddDays(3),
                Description = description,
                PolymorphicFkId = polimorphicId,
                PaymentCode = payedItemCode,
                InvoiceNumber = largestInvoiceNumberForThisYear + 1
            };

            unitOfWork.InvoicesRepository.Add(invoice);
            await unitOfWork.SaveAsync();

            return new ePayQueryViewModel
            {
                Amount = invoice.Amount.ToString(CultureInfo.InvariantCulture),
                Currency = invoice.Currency,
                ExpirationDate = invoice.ExpirationDate?.ToString("dd.MM.yyyy"),
                Description = invoice.Description,
                Invoice = invoice.InvoiceNumber.ToString("D" + 5),
                MIN = ConfigurationManager.AppSettings["ePayClientKey"]
            };
        }

        public async Task UpdateInvoiceOnNotification(ePayRecieveViewModel ePayReceive)
        {
            //int invoiceForUpdateId = int.Parse(ePayReceive.Invoice);
            Invoices invoiceToUpdate = await unitOfWork.InvoicesRepository
                .Where(i => i.InvoiceNumber == ePayReceive.Invoice && i.Status == null)
                .OrderByDescending(i => i.CreatedOn.Year)
                .FirstOrDefaultAsync();

            if (invoiceToUpdate == null)
            {
                throw new ContentNotFoundException();
            }
            invoiceToUpdate.Status = ePayReceive.Status;
            invoiceToUpdate.Stan = string.IsNullOrEmpty(ePayReceive.Stan) ? null : (int?)int.Parse(ePayReceive.Stan);
            invoiceToUpdate.PayDate = string.IsNullOrEmpty(ePayReceive.PayDate) ? null : (DateTime?)DateTime.ParseExact(ePayReceive.PayDate, "yyyyMMddHHmmss", null);
            invoiceToUpdate.BankCode = string.IsNullOrEmpty(ePayReceive.BankCode) ? null : ePayReceive.BankCode;

            unitOfWork.InvoicesRepository.Edit(invoiceToUpdate);
            await unitOfWork.SaveAsync();

        }
    }
}