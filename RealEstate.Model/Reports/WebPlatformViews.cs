using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Reports
{
    public class WebPlatformViews
    {
        [Key]
        public int Id { get; set; }
        
        public int Views { get; set; }

        [ForeignKey("WebPlatform")]
        public int WebPlatformId { get; set; }
        public virtual WebPlatforms WebPlatform { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public virtual Reports Report { get; set; }
    }
}