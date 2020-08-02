using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using RealEstate.Repositories;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.Services.Helpers;
using RealEstate.Services.Interfaces;
using RealEstate.Services.Payments.PaymentCommands;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.Payments;

namespace RealEstate.Services.Payments
{
    public class PaymentServices : BaseService
    {
        private InvoiceServices InvoicesManager { get; }
        private PayedItemServices PayedItemsManager { get; }

        private readonly Dictionary<PaymentMethods, string> _paymentMethodsStrings = new Dictionary<PaymentMethods, string>()
        {
            { PaymentMethods.DirectCreaditDebitCard , "credit_paydirect" },
            { PaymentMethods.EPayWebTrader , "paylogin" },
            { PaymentMethods.FreeTransaction , "paylogin" },
            { PaymentMethods.DepositSlip, "paylogin" }
        };

        private readonly Dictionary<string, string> _validationKeyValues = new Dictionary<string, string>();

        [Inject]
        public PaymentServices( IUnitOfWork unitOfWork, ApplicationUserManager userManager
            , InvoiceServices invoiceServices, PayedItemServices payedItemServices) : base(unitOfWork, userManager)
        {
            InvoicesManager = invoiceServices;
            PayedItemsManager = payedItemServices;
        }

        public async Task<string> PaymentFacade(PaymentMethodInfoViewModel paymentInfo, PayedItemViewModel payedItem, decimal? amount = null, string urlOk = null, string urlCancel = null)
        {
            if (string.IsNullOrEmpty(payedItem.PayedItemId))
            {
                throw new ArgumentException("Идентификаторът на платения елемент е задължителен!");
            }
            var payedItemInfo = await PayedItemsManager.Get(payedItem.PayedItemCode) ?? throw new ArgumentException("Кодът не съществува!");

            if (payedItemInfo.Amount == 0.0M && amount != null)
            {
                payedItemInfo.Amount = (decimal) amount;
            }
            else if(payedItemInfo.Amount == 0.0M && amount == null)
            {
                throw new ArgumentException("Цената на продуктът/услугата не е определена! Моля свържете се с екипът ни за корекция!");
            }

            ePayQueryViewModel ePayInfo = await InvoicesManager.CreateInvoiceForSending(payedItemInfo.Amount, payedItem.PayedItemId, payedItemInfo.Description, payedItem.PayedItemCode);


            // Payment Logic
            if (paymentInfo.PaymentMethod.ToUpper() == "EASYPAY")
            {
                if (!IsEasyPayModelValid(paymentInfo, payedItem))
                {
                    throw new ServicesValidationException();
                }
                var easyPayCode = await GetEasyPayCode(ePayInfo, paymentInfo, payedItem);

                return easyPayCode;
            }
            else
            {
                if (!IsEPayModelValid(paymentInfo, payedItem))
                {
                    throw new ServicesValidationException();
                }
                if (string.IsNullOrEmpty(urlOk) || string.IsNullOrEmpty(urlCancel))
                {
                    throw new ServicesValidationException();
                }

                return EPayFacade(ePayInfo, paymentInfo.PaymentMethod, urlOk, urlCancel);
            }
        }

        public async Task AcceptPayment(int invoiceId)
        {
            var invoice = await unitOfWork.InvoicesRepository
                .Include(i => i.PayedItemInfo)
                .Where(i => i.InvoiceNumber == invoiceId && i.Status == null)
                .OrderByDescending(i => i.CreatedOn.Year)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Фактурата не е намерена!");

            var paymentAcceptFactory = new PaymentCommandsFactory(unitOfWork, userManager);
            var paymentAction = paymentAcceptFactory.Create(invoice.PayedItemInfo.PaymentActionHandle);

            await paymentAction.Execute(invoice.PolymorphicFkId, invoice.PayedItemInfo);
        }


        #region PaymentFacadeMethods

        /// <summary>
        /// Redirect to Payment provider with the all needed parameters (Expecting validated data)
        /// </summary>
        /// <param name="ePayInfo"></param>
        /// <param name="paymentMethod">method of payment supported methods: 
        /// ePayPayment | DirectCreditCard | FreeTransaction | DepositSlip
        /// </param>
        /// <param name="urlOk"></param>
        /// <param name="urlCancel"></param>
        /// <returns></returns>
        private string EPayFacade(ePayQueryViewModel ePayInfo, string paymentMethod, string urlOk, string urlCancel)
        {
            List<KeyValuePair<string, string>> httpParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("URL_OK", urlOk),
                new KeyValuePair<string, string>("URL_CANCEL", urlCancel)
            };
            switch (paymentMethod.ToUpper())
            {
                case "EPAYPAYMENT":
                    httpParameters.AddRange(EPayPayment(ePayInfo));
                    break;
                case "DIRECTCREDITCARD":
                    httpParameters.AddRange(EPayDirectCreditCard(ePayInfo));
                    break;
                case "FREETRANSACTION":
                    httpParameters.AddRange(FreeTransaction(ePayInfo));
                    break;
                case "DEPOSITSLIP":
                    httpParameters.AddRange(DepositSlip(ePayInfo));
                    break;
                default:
                    throw new NotSupportedException("Методът на плащане не се поддържа!");
            }

            // This line REDIRECTS to target page with the passed parameters
            return WebExtentions.CreatePaymentForm(httpParameters, ConfigurationManager.AppSettings["ePayUrl"]);
        }

        /// <summary>
        /// Web Trader from ePay Documentation
        /// </summary>
        /// <param name="ePayInfo"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> EPayPayment(ePayQueryViewModel ePayInfo)
        {
            string ePayInfoString = "MIN=" + ePayInfo.MIN + "\nINVOICE=" + ePayInfo.Invoice + "\nAMOUNT=" + ePayInfo.Amount + "\nEXP_TIME=" + ePayInfo.ExpirationDate + "\nDESCR=" + ePayInfo.Description + "\nENCODING=utf-8";
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(ePayInfoString));
            string checkSum = ePayHelpers.HMAC_SHA1_Encoding(encoded);
            string page = _paymentMethodsStrings[PaymentMethods.EPayWebTrader];

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("PAGE", page),
                new KeyValuePair<string, string>("CHECKSUM", checkSum),
                new KeyValuePair<string, string>("ENCODED", encoded)
            };
        }

        /// <summary>
        /// Direct credit card payment with ePay
        /// </summary>
        /// <param name="ePayInfo"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> EPayDirectCreditCard(ePayQueryViewModel ePayInfo)
        {
            string ePayInfoString = "MIN=" + ePayInfo.MIN + "\nINVOICE=" + ePayInfo.Invoice + "\nAMOUNT=" + ePayInfo.Amount + "\nEXP_TIME=" + ePayInfo.ExpirationDate + "\nDESCR=" + ePayInfo.Description + "\nENCODING=utf-8";
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(ePayInfoString));
            string checkSum = ePayHelpers.HMAC_SHA1_Encoding(encoded);
            string page = _paymentMethodsStrings[PaymentMethods.DirectCreaditDebitCard];

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("PAGE", page),
                new KeyValuePair<string, string>("CHECKSUM", checkSum),
                new KeyValuePair<string, string>("ENCODED", encoded),
                new KeyValuePair<string, string>("LANG", "bg")
            };
        }

        /// <summary>
        /// Free Transaction The Payed amount goes to the receiver ePay micro account
        /// </summary>
        /// <param name="ePayInfo"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> FreeTransaction(ePayQueryViewModel ePayInfo)
        {
            string page = _paymentMethodsStrings[PaymentMethods.FreeTransaction];

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("PAGE", page),
                new KeyValuePair<string, string>("MIN", ePayInfo.MIN),
                new KeyValuePair<string, string>("INVOICE", ePayInfo.Invoice),
                new KeyValuePair<string, string>("TOTAL", ePayInfo.Amount),
                new KeyValuePair<string, string>("DESCR", ePayInfo.Description),
                new KeyValuePair<string, string>("ENCODING","utf-8")
            };
        }

        /// <summary>
        /// The Payed amount goes to the receiver bank account (which is specified in his ePay account)
        /// </summary>
        /// <param name="ePayInfo"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> DepositSlip(ePayQueryViewModel ePayInfo)
        {
            string page = _paymentMethodsStrings[PaymentMethods.DepositSlip];
            string merchant = ConfigurationManager.AppSettings["CompanyName"];
            string iban = ConfigurationManager.AppSettings["IBAN"];
            string bic = ConfigurationManager.AppSettings["BIC"];
            string amount = ePayInfo.Amount;

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("PAGE", page),
                new KeyValuePair<string, string>("MERCHANT", merchant),
                new KeyValuePair<string, string>("IBAN", iban),
                new KeyValuePair<string, string>("BIC", bic),
                new KeyValuePair<string, string>("TOTAL", decimal.Parse(amount).ToString("F"))
            };
        }


        /// <summary>
        /// Expecting valid data !
        /// </summary>
        /// <param name="ePayInfo"></param>
        /// <param name="paymentInfo"></param>
        /// <param name="payedItem"></param>
        /// <returns></returns>
        private async Task<string> GetEasyPayCode(ePayQueryViewModel ePayInfo, PaymentMethodInfoViewModel paymentInfo, PayedItemViewModel payedItem)
        {

            //"MIN=D555355354\nINVOICE=123456\nAMOUNT=20.20\nEXP_TIME=01.08.2020\nDESCR=Test\nMERCHANT=eTemida\nIBAN=BG80BNBG96611020345678\nBIC=TTBBBG22\nPSTATEMENT=123456\nSTATEMENT=Плащане за обява\nOBLIG_PERSON=Стефан Манев\nEGN=8707221342\nDOC_NO=123456\nDOC_DATE=123\nDATE_BEGIN=123\nDATE_END=123";
            string easyPayRequestParameters =
                "MIN=" + ePayInfo.MIN + "\n" +                      // Required
                "INVOICE=" + ePayInfo.Invoice + "\n" +              // Required
                "AMOUNT=" + ePayInfo.Amount + "\n" +                // Required
                "EXP_TIME=" + ePayInfo.ExpirationDate + "\n" +      // Required
                "DESCR=" + ePayInfo.Description + "\n" +            // Optional
                "MERCHANT=" + ConfigurationManager.AppSettings["CompanyName"] + "\n" + // Required
                "IBAN=" + ConfigurationManager.AppSettings["IBAN"] + "\n" +            // Required
                "BIC=" + ConfigurationManager.AppSettings["BIC"] + "\n" +              // Required
                "OBLIG_PERSON=" + paymentInfo.PayerName + "\n" +                       // Required
                                                                                       //paymentInfo.PayerIdType + "=" + paymentInfo.PayerId + "\n" +           // Required
                "ENCODING=utf-8";

            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(easyPayRequestParameters));
            string checkSum = ePayHelpers.HMAC_SHA1_Encoding(encoded);

            string resultParameters = "ENCODED=" + encoded + "&CHECKSUM=" + checkSum;


            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(ConfigurationManager.AppSettings["EasyPayUrl"] + "?" + resultParameters);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string easyPayResult = await response.Content.ReadAsStringAsync();
                    string easyPayCode = easyPayResult.Trim('\n').Split('=')[1]; // [0] is IDN / [1] is the Code

                    return easyPayCode;
                }

                throw new ArgumentException("Сървърът на EasyPay не отговаря опитайте пак по-късно!");
            }
        }
        #endregion

        #region Payment Models Validation

        private bool IsEPayModelValid(PaymentMethodInfoViewModel paymentInfo, PayedItemViewModel payedItem)
        {
            if (paymentInfo == null || payedItem == null)
                return false;

            bool isPayedItemValid = !string.IsNullOrEmpty(payedItem.PayedItemId);
            if (!isPayedItemValid)
            {
                _validationKeyValues.Add("PayedItem.PayedItemId", "Полето не трябва да бъде празно");
            }
            return isPayedItemValid;
        }

        private bool IsEasyPayModelValid(PaymentMethodInfoViewModel paymentInfo, PayedItemViewModel payedItem)
        {
            if (paymentInfo == null || payedItem == null)
                return false;

            bool isPayedItemValid = !string.IsNullOrEmpty(payedItem.PayedItemId);
            if (!isPayedItemValid)
            {
                _validationKeyValues.Add("PayedItem.PayedItemId", "Полето не трябва да бъде празно");
            }

            bool isPayerNameValid = !string.IsNullOrEmpty(paymentInfo.PayerName) && paymentInfo.PayerName?.Length < 26;
            if (!isPayerNameValid)
            {
                _validationKeyValues.Add("PaymentInfo.PayerName", "Полето не трябва да бъде празно, нито да е по дълго от 26 символа");
            }

            return isPayedItemValid && isPayerNameValid;
        }

        #endregion

        #region Payment Contact information

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payedItemCode"></param>
        /// <param name="payableId"></param>
        /// <param name="contactInfoType">PhoneNumber | Email</param>
        /// <returns></returns>
        private async Task<string> GetContactInformation(string payedItemCode, string payableId, string contactInfoType = "PhoneNumber")
        {
            var payedItemType = await unitOfWork.PayedItemsRepository
                .Where(p => p.Code == payedItemCode)
                .Select(p => p.PayedItemType)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен продуктът/услугата, която искате да платите");

            ContactInfoViewModel contactInfo;

            switch (payedItemType.ToUpper())
            {
                ////Uncomment when start using advertisements
                //case "Advertisements":
                //    int adId = int.Parse(payableId);
                //    contactInfo = (await UnitOfWork.AdvertisementsRepository
                //        .Include(a => a.NonRegisteredUser)
                //        .Where(a => a.AdvertisementID == adId)
                //        .Select(a => new ContactInfoViewModel
                //        {
                //            Email = a.NonRegisteredUser.Email,
                //            PhoneNumber = a.NonRegisteredUser.PhoneNumber
                //        })
                //        .FirstOrDefaultAsync()) ?? throw new ContentNotFoundException("Обяватa не е намерена!");
                //    break;

                case "RESERVATION":
                    var reservationId = int.Parse(payableId);
                    contactInfo = await unitOfWork.ReservationsRepository
                        .Include(r => r.NonRegisteredUser, r => r.ClientUser)
                        .Where(r => r.ReservationId == reservationId)
                        .Select(r => new ContactInfoViewModel
                        {
                            Email = r.ClientUserId == null ? r.NonRegisteredUser.ClientEmail : r.ClientUser.Email,
                            PhoneNumber = r.ClientUserId == null ? r.NonRegisteredUser.ClientPhoneNumber : r.ClientUser.PhoneNumber
                        })
                        .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Потребителя не е намерен!");
                    break;

                default:
                    throw new ArgumentException("Не съществува такъв платен елемент!");
            }

            if (contactInfo == null)
            {
                throw new ArgumentException("Обяватa няма телефонен номер");
            }

            return contactInfoType == "PhoneNumber" ? contactInfo.PhoneNumber : contactInfo.Email;
        }

        #endregion
    }

    public enum PaymentMethods
    {
        FreeTransaction, // Платената сума от клиента постъпва по Микросметка на получателя. Необходимо е клиента и получателя на плащането да са регистрирани в ePay.bg
        EPayWebTrader, // Плащане през сайта на ePay като е нужен е вход от клиента
        DirectCreaditDebitCard, // Директно плащане с кредитна/дебитна карта
        DepositSlip //Вносна бележка
    }
}