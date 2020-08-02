using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public virtual ISet<PropertiesBase> Properties { get; set; } = new HashSet<PropertiesBase>();


        //public string ExtraType { get; set; } // Could be Rental/Property/Null(when for both)
    }

    public class RentalExtras : Extras
    {
        public RentalExtras() { }
        public RentalExtras(string extraName) : base(extraName) { }
    }

    public class PropertyExtras : Extras
    {
        public PropertyExtras() { }
        public PropertyExtras(string extraName) : base(extraName) { }
    }
}
