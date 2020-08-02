using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Reports
{
    public class Reports
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string PathToReport { get; set; }

        public int TotalViews { get; set; }
        public byte TotalCalls { get; set; }
        public byte TotalInspections { get; set; }
        public byte TotalOffers { get; set; }

        // What is the result of the taken actions
        public string ActionsConclusion { get; set; }

        // For what changes the client will be asked
        public bool IsPriceChangeIssued { get; set; }
        public bool IsMarketingChangeIssued { get; set; }
        public string ChangeArguments { get; set; }
        //------------------------------------------

        public virtual ISet<WebPlatformViews> WebPlatformViews { get; set; } = new HashSet<WebPlatformViews>();
        public virtual ISet<PromotionMedia> PromotionMediae { get; set; } = new HashSet<PromotionMedia>();
        public virtual ISet<Partners> ColleaguesPartners { get; set; } = new HashSet<Partners>();

        public virtual ISet<CustomPromotionMedia> CustomPromotionMediae { get; set; } =
            new HashSet<CustomPromotionMedia>();

        public virtual ISet<CustomRecommendedActions> CustomRecommendedActions { get; set; } =
            new HashSet<CustomRecommendedActions>();

        public virtual ISet<Offers> Offers { get; set; } = new HashSet<Offers>();

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual Properties Property { get; set; }

        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual ApplicationUser Agent { get; set; }
    }
}