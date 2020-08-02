using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    //this Entity represents the renting information for example
    //Hotel has 12 rooms with 3 beds
    public class RentalsInfo : PropertiesBase
    {
        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual Properties Property { get; set; }

        //How many units ot this type the Property has
        public int UnitCount { get; set; }                
        
    }
}
