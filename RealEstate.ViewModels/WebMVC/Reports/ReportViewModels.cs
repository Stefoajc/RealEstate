using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Reports
{
    public class ReportCreateViewModel : IValidatableObject
    {
        public ICollection<int> PromotionMediaUsedIds { get; set; } = new List<int>();
        public ICollection<string> CustomPromotionMediae { get; set; } = new List<string>();
        public ICollection<WebPlatformViewCreateViewModel> WebPlatformViews { get; set; } = new List<WebPlatformViewCreateViewModel>();

        public ICollection<int> ColleguesIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Въведете брой преглеждания")]
        public int TotalViews { get; set; }
        [Required(ErrorMessage = "Въведете брой обаждания")]
        public byte TotalCalls { get; set; }
        [Required(ErrorMessage = "Въведете брой огледи")]
        public byte TotalInspections { get; set; }
        [Required(ErrorMessage = "Въведете брой оферти")]
        public byte TotalOffers { get; set; }
        public ICollection<decimal> Offers { get; set; } = new List<decimal>();

        // What is the result of the taken actions
        [Required(ErrorMessage = "Изводите са задължителни!")]
        [StringLength(int.MaxValue, MinimumLength = 20, ErrorMessage = "Изводът трябва да е поне 20 символа!")]
        public string ActionsConclusion { get; set; }

        // For what changes the client will be asked
        public bool IsPriceChangeIssued { get; set; }
        public bool IsMarketingChangeIssued { get; set; }
        public ICollection<string> CustomRecommendedActions { get; set; } = new List<string>();

        [Required(ErrorMessage = "Аргументите са задължителни!")]
        [StringLength(int.MaxValue, MinimumLength = 20, ErrorMessage = "Аргументацията трябва да е поне 20 символа!")]
        public string ChangeArguments { get; set; }
        //------------------------------------------

        public bool SendToOwnersViaEmail { get; set; }

        public int PropertyId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Offers.Count > TotalOffers)
            {
                yield return new ValidationResult("Дали сте повече офертни цени отколкото сте записали в Общо оферти!", new[] { "TotalOffers" });
            }
        }
    }





    public class ReportFileDownloadViewModel
    {
        public string ReportName { get; set; }
        public byte[] ReportData { get; set; }
    }


    public class ReportToBeWritenViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string OwnerName { get; set; }
    }





    #region Report Email template

    public class ReportTemplateViewModel
    {
        public List<PromotionMediaForEmail> PromotionMediae { get; set; } = new List<PromotionMediaForEmail>();
        public List<string> CustomPromotionMediae { get; set; } = new List<string>();
        public List<WebPlatformViewEmailViewModel> WebPlatformViews { get; set; } = new List<WebPlatformViewEmailViewModel>();
        public List<string> PartnersSharedWith { get; set; } = new List<string>();

        public int TotalViews { get; set; }
        public byte TotalCalls { get; set; }
        public byte TotalInspections { get; set; }
        public byte TotalOffers { get; set; }
        public List<decimal> Offers { get; set; } = new List<decimal>();

        public string ActionsConclusion { get; set; }

        public bool IsPriceChangeIssued { get; set; }
        public bool IsMarketingChangeIssued { get; set; }
        public List<string> CustomRecommendedActions { get; set; } = new List<string>();
        public string ChangeArguments { get; set; }

        public DateTime CreatedOn { get; set; }
        public string AgentCreator { get; set; }
        public string LinkToProperty { get; set; }
    }


    public class PromotionMediaForEmail
    {
        public int Id { get; set; }
        public string MediaType { get; set; }

        public bool IsChecked { get; set; }
    }

    #endregion


}