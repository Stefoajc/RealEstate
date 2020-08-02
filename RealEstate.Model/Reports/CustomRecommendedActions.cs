using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Reports
{
    public class CustomRecommendedActions
    {
        [Key]
        public int Id { get; set; }
        public string RecommendedAction { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public virtual Reports Report { get; set; }
    }
}