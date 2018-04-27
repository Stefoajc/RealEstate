using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class Sights
    {
        [Key]
        public int SightId { get; set; }
        [ForeignKey("City")]
        public int? CityId { get; set; }
        public virtual Cities City { get; set; }
        [ForeignKey("Coordinates")]
        public int? CoordinatesId { get; set; }
        public virtual Coordinates Coordinates { get; set; }

        public string SightName { get; set; }
        public string SightInfo { get; set; }

        public virtual ISet<SightReviews> Reviews { get; set; } = new HashSet<SightReviews>();
    }
}
