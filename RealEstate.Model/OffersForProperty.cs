using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class OffersForProperty
    {
        [Key]
        public int Id { get; set; }
        public decimal Offer { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual Properties Property { get; set; }
    }
}