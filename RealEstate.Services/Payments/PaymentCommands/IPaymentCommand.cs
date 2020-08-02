using System.Threading.Tasks;
using RealEstate.Model.Payment;

namespace RealEstate.Services.Payments.PaymentCommands
{
    public interface IPaymentCommand
    {
        Task Execute(string payedItemId, PayedItemsMeta itemMeta);
    }
}
