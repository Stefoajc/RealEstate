using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Payment
{
    public class Invoices
    {
        public Invoices()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid InvoiceID { get; set; } = Guid.NewGuid();

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        
        public int InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? PayDate { get; set; }
        public int? Stan { get; set; }
        public string BankCode { get; set; }

        [ForeignKey("PayedItemInfo")]
        public string PaymentCode { get; set; }
        public PayedItemsMeta PayedItemInfo { get; set; }

        #region ForeignKeys
        [Required]
        [StringLength(128)] // Because User Id is 128 chars
        [Index("IX_IdType", 1)]
        public string PolymorphicFkId { get; set; }
        #endregion
    }
}
