using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.ContactDiary
{
    public class ContactsDiary
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CityDistrict { get; set; }
        public string Address { get; set; }

        //Facebook, Farming, OLX ...
        public string PropertySource { get; set; }        
        
        public string AdditionalDescription { get; set; }


        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual Cities City { get; set; }

        [ForeignKey("ContactedPersonType")]
        public int? ContactedPersonTypeId { get; set; }
        public virtual ContactedPersonTypes ContactedPersonType { get; set; }

        [ForeignKey("DealType")]
        public int? DealTypeId { get; set; }
        public virtual DealTypes DealType { get; set; }

        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual AgentUsers Agent { get; set; }

        [ForeignKey("PropertyType")]
        public int? PropertyTypeId { get; set; }
        public virtual PropertyTypes PropertyType { get; set; }

        [ForeignKey("NegotiationState")]
        public int? NegotiationStateId { get; set; }
        public virtual NegotiationStates NegotiationState { get; set; }
    }
}