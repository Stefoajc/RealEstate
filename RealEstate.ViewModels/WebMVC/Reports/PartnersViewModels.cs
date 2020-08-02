using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RealEstate.ViewModels.WebMVC.Reports
{
    public class PartnersListViewModel
    {
        public int Id { get; set; }
        public string PartnerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string SocialMediaAccount { get; set; }
        public string PartnerCompanyName { get; set; }
        public string AdditionalInformation { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CityId { get; set; }
        public string CityName { get; set; }

        public int PartnerTypeId { get; set; }
        public string PartnerTypeName { get; set; }

        public string AgentCreatorId { get; set; }
        public string AgentCreatorName { get; set; }

        public bool IsAllowedToDelete { get; set; }
        public bool IsAllowedToEdit { get; set; }
    }

    public class PartnersDropdownViewModel
    {
        public int Id { get; set; }
        public string BrokerInfo { get; set; }
    }

    public class PartnersCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Въведете име на брокера!")]
        public string PartnerName { get; set; }
        public string PartnerCompanyName { get; set; }
        [Required(ErrorMessage = "Въведете телефонен номер на брокера!")]
        [Phone(ErrorMessage = "Невалиден телефонен номер!")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string SocialMediaAccount { get; set; }

        public string AdditionalInformation { get; set; }

        [Required(ErrorMessage = "Изберете тип на партньорът!")]
        public int PartnerTypeId { get; set; }
        [Required(ErrorMessage = "Въведете град, в които работи брокера!")]
        public int CityId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbContext = new Data.RealEstateDbContext();

            if (dbContext.Partners.Any(p => p.PhoneNumber == PhoneNumber))
            {
                yield return new ValidationResult("Има партньор с този телефонен номер!", new[] { "PhoneNumber" });
            }

        }
    }

    public class PartnersEditViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Въведете име на брокера!")]
        public string PartnerName { get; set; }
        public string PartnerCompanyName { get; set; }
        [Required(ErrorMessage = "Въведете телефонен номер на брокера!")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string SocialMediaAccount { get; set; }


        [Required(ErrorMessage = "Изберете тип на партньорът!")]
        public int PartnerTypeId { get; set; }
        [Required(ErrorMessage = "Въведете град, в които работи брокера!")]
        public int CityId { get; set; }

        public string AdditionalInformation { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbContext = new Data.RealEstateDbContext();

            if (dbContext.Partners.Any(p => p.PhoneNumber == PhoneNumber && p.Id != Id))
            {
                yield return new ValidationResult("Има партньор с този телефонен номер!", new[] { "PhoneNumber" });
            }

        }
    }


    public class PartnerTypesListViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}
