using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Payments
{
    /// <summary>
    /// Encoded stores base64 coded string off all 
    /// </summary>
    public class ePaySignedViewModel
    {
        /// <summary>
        /// Encoded stores base64 coded string off all properties in the ePayQueryViewModel ("MIN=" + MIN + "\nINVOICE=" + INVOICE ..)
        /// separated from each other it /n !
        /// </summary>
        public string Encoded { get; set; }
        /// <summary>
        /// Hash-based message authentication code 
        /// of the Encoded string
        /// </summary>
        public string CheckSum { get; set; }
    }

    public class ePayHttpRequestViewModel
    {
        public string PAGE { get; set; }
        public string ENCODED { get; set; }
        public string CHECKSUM { get; set; }
        public string URL_OK { get; set; }
        public string URL_CANCEL { get; set; }
    }

    public class ePayQueryViewModel
    {
        /// <summary>
        /// ePayClientKey
        /// </summary>
        [Required(ErrorMessage = "Въведете клиентски номер")]
        public string MIN { get; set; }
        /// <summary>
        /// Invoice of the transaction
        /// </summary>
        [Required(ErrorMessage = "Въведете номер на фактура")]
        public string Invoice { get; set; }
        /// <summary>
        /// Transaction Amount
        /// </summary>
        [Required(ErrorMessage = "Въведете сума")]
        public string Amount { get; set; }
        /// <summary>
        /// Transaction Currency (ex. BGN,USD,EUR) the default value for epay.bg is BGN
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// How long the payment will be allowed
        /// </summary>
        [Required(ErrorMessage = "Въведете дата на която заявката изтича")]
        public string ExpirationDate { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
    }

    public class ePayRecieveViewModel
    {
        /// <summary>
        /// Long as string
        /// </summary>
        public int Invoice { get; set; }
        /// <summary>
        /// Can be PAID/DENIED/EXPIRED
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// DateTime as string
        /// </summary>
        public string PayDate { get; set; }
        /// <summary>
        /// long as string
        /// </summary>
        public string Stan { get; set; }
        /// <summary>
        /// string
        /// </summary>
        public string BankCode { get; set; }
    }

    public class PaymentMethodInfoViewModel
    {
        /// <summary>
        /// Name of Payer
        /// USED FOR EASYPAY (REQUIRED)
        /// </summary>        
        public string PayerName { get; set; }

        /// <summary>
        /// Payer identifiation number should be in format for EGN (10 digits) / LNC (9 digits) / 
        /// USED FOR EASYPAY (NOT REQUIRED)
        /// </summary>
        public string PayerId { get; set; }

        /// <summary>
        /// Payer Identification Number Type Could be one of three (EGN|LNC|BULSTAT)
        /// USED FOR EASYPAY (NOT REQUIRED)
        /// </summary>
        public string PayerIdType { get; set; }

        /// <summary>
        /// SHOULD BE ONE OF (EASYPAY|EPAYPAYMENT|DIRECTCREDITCARD|FREETRANSACTION|DEPOSITSLIP)
        /// </summary>
        [Required(ErrorMessage = "Изберете метод на плащане")]
        public string PaymentMethod { get; set; }
    }

    public class PayedItemViewModel
    {
        /// <summary>
        /// Payed item code to fetch the info from the database about the descripion and the amount
        /// </summary>
        public string PayedItemCode { get; set; }
        /// <summary>
        /// The Item To Be Payed ID
        /// </summary>
        public string PayedItemId { get; set; }
    }


    public class PaymentPreCreateViewModel
    {
        /// <summary>
        /// The Item To Be Payed ID
        /// </summary>
        public string PayedItemId { get; set; }

        /// <summary>
        /// Name of Payer
        /// USED FOR EASYPAY
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// Payer identifiation number should be in format for EGN (10 digits) / LNC (9 digits) / 
        /// USED FOR EASYPAY
        /// </summary>
        public string PayerId { get; set; }

        /// <summary>
        /// Payer Identification Number Type Could be one of three (EGN|LNC|BULSTAT)
        /// </summary>
        public string PayerIdType { get; set; }

        /// <summary>
        /// SHOULD BE ONE OF (EASYPAY|EPAYPAYMENT|DIRECTCREDITCARD|FREETRANSACTION|DEPOSITSLIP)
        /// </summary>
        [Required(ErrorMessage = "Изберете метод на плащане")]
        public string PaymentMethod { get; set; }
    }


    public class PaymentCreateViewModel
    {
        public decimal Amount { get; set; }
        public string PaymentDescription { get; set; }

        public PaymentMethodInfoViewModel PaymentInfo { get; set; }
        public PayedItemViewModel PayedItem { get; set; }
    }
}