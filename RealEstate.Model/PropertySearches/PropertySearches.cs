using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.PropertySearches
{
    public class PropertySearches
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public decimal? PriceFrom { get; set; }
        public decimal PriceTo { get; set; }
        public double? AreaInSquareMeters { get; set; }
        public bool IsRentSearch { get; set; }
        public string AdditionalInformation { get; set; }

        public int Views { get; set; } = 0;

        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual Cities City { get; set; }

        public string Areas { get; set; }

        public virtual ISet<PropertyTypes> UnitTypes { get; set; } = new HashSet<PropertyTypes>();

        public virtual PersonSearcher PersonSearcher { get; set; }

        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual AgentUsers Agent { get; set; }
    }
}