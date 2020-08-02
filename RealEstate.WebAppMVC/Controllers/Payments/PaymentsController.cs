using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.Services.Helpers;
using RealEstate.Services.Payments;
using RealEstate.ViewModels.WebMVC.Payments;

namespace RealEstate.WebAppMVC.Controllers.Payments
{
    [AllowAnonymous]
    public class PaymentsController : Controller
    {
        [Inject]
        public InvoiceServices InvoicesManager { get; set; }
        [Inject]
        public PaymentServices PaymentsManager { get; set; }

        /// <summary>
        /// CallBack API waiting for ePay to send information about the status of the payment
        /// Notes: Should be POST ! Should be allowed for NonRegistered Users !
        /// TODO: Filter so only ePay can access this API
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        // POST: Payments/PaymentAccept
        [HttpPost, AllowAnonymous]
        public async Task<ContentResult> PaymentAccept(string encoded, string checksum)
        {
            //Log
            System.IO.File.AppendAllText(@"D:\RealEstate\RealEstate.WebAppMVC\PaymentsBeforeNotification\Log.txt", "Encoded: " + encoded + " Checksum: " + checksum);
            //

            var infoStatus = "";
            var ePayQueryDataAsString = Encoding.UTF8.GetString(Convert.FromBase64String(encoded)); //ex. INVOICE=00050:STATUS=PAID:PAY_TIME=20171204163636:STAN=058030:BCODE=058030
            var ePayQueryData = ePayQueryDataAsString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var ePayTransaction in ePayQueryData)
            {
                if (checksum != ePayHelpers.HMAC_SHA1_Encoding(encoded))
                {
                    return Content($"INVOICE={ePayQueryDataAsString[1]}:STATUS=ERR\n"); //"ERR=Not valid CHECKSUM\n", "plain/text"
                }
                if (RegexExtentions.preg_match(@"^INVOICE=(\d+):STATUS=(PAID|DENIED|EXPIRED)(:PAY_TIME=(\d+):STAN=(\d+):BCODE=([0-9a-zA-Z]+))?$", ePayTransaction, out var ePayQueryDataProperties))
                {
                    ePayRecieveViewModel paymentData = new ePayRecieveViewModel()
                    {
                        Invoice = int.Parse(ePayQueryDataProperties[1]),
                        Status = ePayQueryDataProperties[2],
                        PayDate = ePayQueryDataProperties[4],
                        Stan = ePayQueryDataProperties[5],
                        BankCode = ePayQueryDataProperties[6]
                    };

                    switch (paymentData.Status)
                    {
                        case "PAID":
                            try  // if invoice with this id is not found
                            {
                                await InvoicesManager.UpdateInvoiceOnNotification(paymentData);
                                infoStatus = $"INVOICE={paymentData.Invoice}:STATUS=OK\n";
                                await PaymentsManager.AcceptPayment(paymentData.Invoice);
                            }
                            catch (ContentNotFoundException)
                            {
                                infoStatus = $"INVOICE={paymentData.Invoice}:STATUS=NO\n";
                            }
                            catch (Exception)
                            {
                                infoStatus = $"INVOICE={paymentData.Invoice}:STATUS=ERR\n";
                            }
                            //Increase the balance of the user
                            break;
                        case "DENIED":
                        case "EXPIRED":
                            try // if invoice with this id is not found
                            {
                                await InvoicesManager.UpdateInvoiceOnNotification(paymentData);
                                infoStatus = $"INVOICE={paymentData.Invoice}:STATUS=OK\n";
                            }
                            catch (ContentNotFoundException)
                            {
                                infoStatus = $"INVOICE={paymentData.Invoice}:STATUS=NO\n";
                            }
                            break;
                        default:
                            infoStatus = $"INVOICE={paymentData.Invoice}:STATUS=ERR\n";
                            break;
                    }

                }
                else
                {
                    infoStatus = "The RegularExpression Does not Match the entry";
                }
            }
            return Content(infoStatus, "plain/text");
        }

    }
}