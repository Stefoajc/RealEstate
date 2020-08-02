using System;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject;
using RealEstate.Model.SmsSubSystem;

namespace RealEstate.WebAppMVC.Controllers
{
    public class SmsController : Controller
    {
        
        private Services.NotificationSmsService SmsManager { get; set; }

        [Inject]
        public SmsController(Services.NotificationSmsService smsService)
        {
            SmsManager = smsService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult AddSmsDeliveryStatus(SmsDeliveryStatuses smsDeliveryStatus)
        {
            try
            {
                SmsManager.AddSmsDeliveryStatus(smsDeliveryStatus);
                return Content(DLR_Status.DLR_OK.ToString(), "text/plain");
            }
            catch (Exception)
            {
                return Content(DLR_Status.DLR_FAILED.ToString(), "text/plain");
            }
        }

        [AllowAnonymous]
        public async Task<string> SendSms(string contactInfo, string messageBody)
        {
            if (!Request.IsLocal)
            {
                throw new SecurityException("Not Allowed for outside usage");
            }
            var messageBodyBin = Encoding.Default.GetBytes(messageBody);
            messageBody = Encoding.UTF8.GetString(messageBodyBin);
            await SmsManager.SendSmsAsync(contactInfo, messageBody);
            return "";
        }

        enum DLR_Status
        {
            DLR_OK = 1,
            DLR_FAILED = 2,
        }
    }
}