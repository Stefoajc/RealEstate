using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class ContactsDiaryViewModels
    {

    }

    public class RecordListViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string CityDistrict { get; set; }
        public string Address { get; set; }
        public string PropertySource { get; set; }
        public string AdditionalDescription { get; set; }
        public int? ContactedPersonTypeId { get; set; }
        public string ContactedPersonType { get; set; }
        public int? DealTypeId { get; set; }
        public string DealType { get; set; }
        public int? PropertyTypeId { get; set; }
        public string PropertyType { get; set; }
        public int? NegotiationStateId { get; set; }
        public string NegotiationState { get; set; }
        public string RecordColor { get; set; }
        public string AgentId { get; set; }
        public string AgentName { get; set; }
    }

    public class CreateRecordViewModel
    {
        [StringLength(20)]
        [Required(ErrorMessage = "Телефонният номер е задължителен!")]
        [RegularExpression(@"(\+)*((\d)+( )*)*$", ErrorMessage = "Невалиден телефонен номер!")]
        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "Градът е задължителен!")]
        public int CityId { get; set; }
        public string CityDistrict { get; set; }
        public string Address { get; set; }

        //Facebook, Farming, OLX ...
        public string PropertySource { get; set; }
        public string AdditionalDescription { get; set; }
        public int? ContactedPersonTypeId { get; set; }
        public int? DealTypeId { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? NegotiationStateId { get; set; }
    }

    public class EditRecordViewModel
    {
        public int Id { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "Телефонният номер е задължителен!")]
        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        public int CityId { get; set; }
        public string CityDistrict { get; set; }
        public string Address { get; set; }

        //Facebook, Farming, OLX ...
        public string PropertySource { get; set; }
        public string AdditionalDescription { get; set; }
        public int? ContactedPersonTypeId { get; set; }
        public int? DealTypeId { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? NegotiationStateId { get; set; }
    }


    public class DealTypeListViewModel
    {
        public int DealTypeId { get; set; }
        public string DealType { get; set; }
    }


    public class NegotiationStageListViewModel 
    {
        public int NegotiationStageId { get; set; }
        public string NegotiationStage { get; set; }
    }

    public class PersonTypeListViewModel
    {
        public int PersonTypeId { get; set; }
        public string PersonType { get; set; }
    }
}