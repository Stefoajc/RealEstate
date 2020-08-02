using System.Threading.Tasks;

namespace RealEstate.Services.Payments.PaymentNotificationCommands
{
    public interface IPaymentNotification
    {
        Task SendEmail(string payableId);
        Task SendSms(string payableId);
    }
}