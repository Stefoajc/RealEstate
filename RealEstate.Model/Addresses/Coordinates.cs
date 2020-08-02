using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model
{
    public class Coordinates
    {
        [Key]
        public int CoordinatesId { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }

        //[ForeignKey("Address")]
        //public int? AddressId { get; set; }
        //[ForeignKey("Sight")]
        //public int? SightId { get; set; }

        //public virtual Addresses Address { get; set; }
        //public virtual Sights Sight { get; set; }
    }
}
