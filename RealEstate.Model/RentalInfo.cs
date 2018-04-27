using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    //this Entity represents the renting information for example
    //Hotel has 12 rooms with 3 beds
    public class RentalsInfo
    {
        [Key]
        public int RentalId { get; set; }
        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual Properties Property { get; set; }
        //UnitType like Apartment,Room,Studio,Mesonet,Bungalo
        [ForeignKey("UnitType")]
        public int UnitTypeId { get; set; }
        public virtual UnitTypes UnitType { get; set; }
        //How many units ot this type the Property has
        public int UnitCount { get; set; }

        public decimal RentalPrice { get; set; }
        public RentalPeriod RentPricePeriod { get; set; }
        public string AdditionalInfo { get; set; }

        public virtual ISet<RentalExtras> RentalExtras { get; set; } = new HashSet<RentalExtras>();
        public virtual ISet<Reservations> Reservations { get; set; } = new HashSet<Reservations>();
        public virtual ISet<KeyValuePairs> RentalAttributes { get; set; } = new HashSet<KeyValuePairs>();
    }


    public enum RentalPeriod
    {
        Dayly,
        PerBedDaily,
        PerPersonDaily,
        Weekly,
        Monthly,
        Yearly
    }
}
