using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class KeyValuePairs
    {
        [Key]
        public int Id { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        [ForeignKey("Property")]
        public int? PropertyId { get; set; }
        public virtual Properties Property { get; set; }

        [ForeignKey("Rental")]
        public int? RentalInfoId { get; set; }
        public virtual RentalsInfo Rental { get; set; }
    }
}
