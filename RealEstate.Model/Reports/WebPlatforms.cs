using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.Reports
{
    public class WebPlatforms
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string WebPlatform { get; set; }


        public virtual ISet<WebPlatformViews> WebPlatformViews { get; set; } = new HashSet<WebPlatformViews>();
    }
}