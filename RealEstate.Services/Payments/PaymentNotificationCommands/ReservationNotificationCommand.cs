using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services.Payments.PaymentNotificationCommands
{
    //public class ReservationNotificationCommand : IPaymentNotification
    //{
    //    private IUnitOfWork UnitOfWork { get; set; }

    //    public ReservationNotificationCommand(IUnitOfWork unitOfWork)
    //    {
    //        UnitOfWork = unitOfWork;
    //    }

    //    private async Task<ContactInfoViewModel> GetReservationCreatorContactInformation(int reservationId)
    //    {
    //        ContactInfoViewModel contactInfo = await UnitOfWork.ReservationsRepository
    //                          .Include(r => r.NonRegisteredUser, r => r.ClientUser)
    //                          .Where(r => r.ReservationId == reservationId)
    //                          .Select(r => new ContactInfoViewModel
    //                          {
    //                              Email = r.ClientUserId == null ? r.NonRegisteredUser.ClientEmail : r.ClientUser.Email,
    //                              PhoneNumber = r.ClientUserId == null ? r.NonRegisteredUser.ClientPhoneNumber : r.ClientUser.PhoneNumber
    //                          })
    //                          .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Потребителя не е намерен!");

    //        return contactInfo;
    //    }

    //    public async Task SendEmail(string payableId, string easyPay)
    //    {
    //        var reservationCreatorContactInformation = await GetReservationCreatorContactInformation(int.Parse(payableId));
    //        SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["NoReplyDomain"], int.Parse(ConfigurationManager.AppSettings["NoReplySmtpPort"]));
    //        smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["NoReplyUserName"], ConfigurationManager.AppSettings["NoReplyPassword"]);

    //        var mailMessage = new MailMessage(ConfigurationManager.AppSettings["NoReplyUserName"],
    //                reservationCreatorContactInformation.Email,
    //            "Код за плащане чрез EasyPay",
    //            $"<p>Здравейте,</p><p>Вашият номер за плащане в EasyPay е: {easyPayCode}</p><p><br></p><p>За да извършите плащането моля посетете някой от <a href=\"https://www.easypay.bg/site/?p=offices\" target=\"_blank\" rel=\"noopener\">офисите на EasyPay</a> и заплатете по посоченият по по-горе код.</p><p><br/></p><p>Поздрави,<br>екипът на&nbsp;<strong>еТемида.</strong></p><hr><p>!&nbsp; Това е автоматично генериран е-мейл от системата на&nbsp;<strong>еТемида.<br></strong>&nbsp; &nbsp; &nbsp;Моля не отговаряйте и не изпращайте други съобщения на този е-мейл.</p>")
    //        { IsBodyHtml = true };

    //        await smtpClient.SendMailAsync(,)
    //    }

    //    public async Task SendSms(string payableId)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
