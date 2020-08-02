using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.PropertySearches
{
    public class PersonSearcher
    {
        [Key,ForeignKey("PropertySearch")]
        public int Id { get; set; }
        public virtual PropertySearches PropertySearch { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AdditionalInformation { get; set; }
    }
}