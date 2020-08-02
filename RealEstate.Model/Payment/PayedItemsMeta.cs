using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.Payment
{
    public class PayedItemsMeta
    {
        [StringLength(200)]
        [Key]
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public string PayedItemType { get; set; }
        public string PayedItemValue { get; set; }

        public string PaymentActionHandle { get; set; }
    }
}
