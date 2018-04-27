using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class SellingInfo
    {
        [Key]
        public int PropertySellingInfoId { get; set; }

        public decimal SellingPrice { get; set; }

        public int BedroomsCount { get; set; }
        public int BathroomsCount { get; set; }
        public string AdditionalInfo { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual Properties Property { get; set; }
    }
}