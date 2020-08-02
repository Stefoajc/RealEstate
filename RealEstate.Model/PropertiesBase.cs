using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class PropertiesBase
    {
        [Key]
        public int Id { get; set; }
        
        //Type of property (Hotel,House...)
        [ForeignKey("UnitType")]
        public int UnitTypeId { get; set; }
        public virtual PropertyTypes UnitType { get; set; }

        public decimal? RentalPrice { get; set; }
        public string AdditionalDescription { get; set; }

        public int? AreaInSquareMeters { get; set; }

        //Set to false when the property is not receiving guests
        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public PropertyState PropertyState { get; set; } = PropertyState.Available;

        //How many time the Property has been viewed
        public long Views { get; set; } = 0;

        [ForeignKey("RentalHirePeriodType")]
        public int? RentalPricePeriodId { get; set; }
        public virtual RentalHirePeriodsTypes RentalHirePeriodType { get; set; }

        public virtual ISet<PropertyLikes> PropertyLikes { get; set; } = new HashSet<PropertyLikes>();
        public virtual ISet<PropertyReviews> Reviews { get; set; } = new HashSet<PropertyReviews>();
        public virtual ISet<Reservations> Reservations { get; set; } = new HashSet<Reservations>();
        public virtual ISet<KeyValuePairs> Attributes { get; set; } = new HashSet<KeyValuePairs>();
        public virtual ISet<Extras> Extras { get; set; } = new HashSet<Extras>();
        public virtual ISet<Appointments> Appointments { get; set; } = new HashSet<Appointments>();

        public virtual ISet<OffersForProperty> Offers { get; set; } = new HashSet<OffersForProperty>();
    }


    public enum PropertyState
    {
        Available,
        Sold,
        Rented
    }
}
