using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class Likes
    {
        [Key]
        public long LikeId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ClientUsers User { get; set; }

        public DateTime LikedOn { get; set; }
    }

    public class CityLikes : Likes
    {
        public int CityId { get; set; }

        public virtual ISet<Cities> Cities { get; set; } = new HashSet<Cities>();
    }

    public class PropertyLikes : Likes
    {
        public int PropertyId { get; set; }

        public virtual ISet<PropertiesBase> Properties { get; set; } = new HashSet<PropertiesBase>();
    }

}
