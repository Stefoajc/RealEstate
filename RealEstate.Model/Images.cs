using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class Images
    {
        [Key]
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
        public string ImageType { get; set; }
        // Width / Height
        public float ImageRatio { get; set; }
    }

    public class UserImages:Images
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class PropertyImages : Images
    {
        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual Properties Property { get; set; }
    }

    public class CityImages : Images
    {
        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual Cities City { get; set; }
    }

    public class SightImages : Images
    {
        [ForeignKey("Sight")]
        public int SightId { get; set; }
        public virtual Sights Sight { get; set; }
    }
}
