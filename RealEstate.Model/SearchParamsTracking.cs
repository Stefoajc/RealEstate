using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class SearchParamsTracking
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public virtual ISet<SearchParams> SearchParams { get; set; } = new HashSet<SearchParams>();
    }
}
