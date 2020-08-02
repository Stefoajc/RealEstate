using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.Reports
{
    /// <summary>
    /// Media for promotion (Web Platforms / Flaers / Bilboard / ..)
    /// </summary>
    public class PromotionMedia
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Media { get; set; }

        public virtual ISet<Reports> Reports { get; set; } = new HashSet<Reports>();
    }
}