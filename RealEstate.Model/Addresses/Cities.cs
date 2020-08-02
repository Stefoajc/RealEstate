using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class Cities
    {

        [Key]
        public int CityId { get; set; }
        [ForeignKey("Country")]
        public int CountryId { get; set; }

        [StringLength(85)]
        public string CityName { get; set; }
        [StringLength(10)]
        public string PostalCode { get; set; }
        [StringLength(50)]
        public string PhoneCode { get; set; }
        [StringLength(20)]
        public string CityCode { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public virtual Countries Country { get; set; }
        public virtual ISet<Addresses> Addresses { get; set; } = new HashSet<Addresses>();
        public virtual ISet<CityReviews> Reviews { get; set; } = new HashSet<CityReviews>();
        public virtual ISet<CityLikes> CityLikes { get; set; } = new HashSet<CityLikes>();
    }
}
