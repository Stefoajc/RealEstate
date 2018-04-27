using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    /// <summary>
    /// The properties which contains the rentals (the rooms, apartements)
    /// Represent the base class of Hotels,Houses,Motels etc..
    /// </summary>
    public class Properties
    {
        public Properties()
        {
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int PropertyId { get; set; }
        [ForeignKey("Address")]
        public int AddressId { get; set; }
        public virtual Addresses Address { get; set; }
        //When is the property expected to be engaged the most
        [ForeignKey("PropertySeason")]
        public int? PropertySeasonId { get; set; }
        public virtual PropertySeason PropertySeason { get; set; }

        [ForeignKey("PropertyType")]
        public int PropertyTypeId { get; set; }
        public virtual PropertyTypes PropertyType { get; set; }

        [ForeignKey("Owner")]
        public string OwnerId { get; set; }
        public virtual OwnerUsers Owner { get; set; }

        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual AgentUsers Agent { get; set; }

        public int? AreaInSquareFt { get; set; }
        [Required]
        public string PropertyName { get; set; }
        public string AdditionalDescription { get; set; }
        public DateTime CreatedOn { get; set; }

        //How many time the Property has been viewed
        public long Views { get; set; }

        //Set to false when the property is not receiving guests
        public bool IsActive { get; set; }

        public decimal? SellingPrice { get; set; }
        public decimal? RentalPrice { get; set; }
        public RentalPeriod RentPricePeriod { get; set; }


        public virtual ISet<PropertyLikes> PropertyLikes { get; set; } = new HashSet<PropertyLikes>();
        public virtual ISet<PropertyReviews> Reviews { get; set; } = new HashSet<PropertyReviews>();
        public virtual ISet<PropertyImages> Images { get; set; } = new HashSet<PropertyImages>();

        public virtual ISet<PropertyExtras> PropertyExtras { get; set; } = new HashSet<PropertyExtras>();

        // if its property with rentals in it use this
        public virtual ISet<RentalsInfo> Rentals { get; set; } = new HashSet<RentalsInfo>();

        public virtual ISet<KeyValuePairs> PropertyAttributes { get; set; } = new HashSet<KeyValuePairs>();
        public virtual ISet<Reservations> Reservations { get; set; } = new HashSet<Reservations>();
    }
}
