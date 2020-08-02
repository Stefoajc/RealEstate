using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    /// <summary>
    /// The properties which contains the rentals (the rooms, apartements)
    /// Represent the base class of Hotels,Houses,Motels etc..
    /// </summary>
    public class Properties : PropertiesBase
    {
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public virtual Addresses Address { get; set; }
        //When is the property expected to be engaged the most
        [ForeignKey("PropertySeason")]
        public int? PropertySeasonId { get; set; }
        public virtual PropertySeason PropertySeason { get; set; }

        [ForeignKey("Owner")]
        public string OwnerId { get; set; }
        public virtual OwnerUsers Owner { get; set; }

        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual AgentUsers Agent { get; set; }

        [Required]
        public string PropertyName { get; set; }

        public decimal? SellingPrice { get; set; }


        public virtual ISet<PropertyImages> Images { get; set; } = new HashSet<PropertyImages>();

        // if its property with rentals in it use this
        public virtual ISet<RentalsInfo> Rentals { get; set; } = new HashSet<RentalsInfo>();  
        
        public virtual ISet<Reports.Reports> Reports { get; set; } = new HashSet<Reports.Reports>();
    }
}
