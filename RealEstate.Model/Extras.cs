using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    /// <summary>
    /// Contains Extras for rentals like IndividualBathroom,individual Kitchen , Balcony
    /// </summary>
    public class Extras
    {
        public Extras()
        {
            
        }
        public Extras(string extraName)
        {
            ExtraName = extraName;
        }

        [Key]
        public int ExtraId { get; set; }

        public string ExtraName { get; set; }
    }

    public class RentalExtras : Extras
    {
        public RentalExtras()
        {
            
        }
        public RentalExtras(string extraName) : base(extraName)
        {
        }

        public virtual ISet<RentalsInfo> Rentals { get; set; } = new HashSet<RentalsInfo>();
    }

    public class PropertyExtras : Extras
    {
        public PropertyExtras()
        {
            
        }
        public PropertyExtras(string extraName) : base(extraName)
        {
        }

        public virtual ISet<Properties> Properties { get; set; }
    }
}
