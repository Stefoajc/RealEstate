using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class Addresses
    {
        [Key]
        public int AddressId { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual Cities City { get; set; }
        //Long and lat for Geolocation
        [ForeignKey("Coordinates")]
        public int? CoordinatesId { get; set; }
        public virtual Coordinates Coordinates { get; set; }

        public string FullAddress { get; set; }
    }
}
