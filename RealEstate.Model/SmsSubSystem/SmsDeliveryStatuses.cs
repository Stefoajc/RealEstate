using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.SmsSubSystem
{
    public class SmsDeliveryStatuses
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// sms/viber
        /// </summary>
        [Required]
        [StringLength(10)]
        public string CHANNEL { get; set; }

        /// <summary>
        /// Unique Service ID
        /// </summary>
        public int SID { get; set; }

        /// <summary>
        /// Delivery status flag
        /// 1-Delivered to handset
        /// 2-Not Delivered to handset (delivery failed)
        /// 8-Delivered to SMSC(Accepted from Mobile operators SMS Center for delivery)
        /// 16-Not accepted (Rejected for delivery) from SMSC
        /// 9001 Delivered 
        /// 9002 Seen 
        /// 9010 Not delivered (delivery failed) 
        /// 9020 Delivered to Viber (Accepted from Viber for delivery) 
        /// 9021 Not accepted (Rejected for delivery) from Viber
        /// </summary>
        public int MESSAGE_STATUS { get; set; }

        /// <summary>
        /// The MSISDN of the recipient of the
        /// message(E.164 format).
        /// The Phone number of the Recipient
        /// </summary>
        public long TO { get; set; }

        /// <summary>
        /// Sender source address.
        /// Can be a short
        /// code or an alphanumerical originator.
        /// </summary>
        public string FROM { get; set; }

        /// <summary>
        /// The SMSC time of the DLR status in unix
        /// timestamp format
        /// </summary>
        public long TS { get; set; }

    }
}
