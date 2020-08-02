using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Reports
{
    public class CustomPromotionMedia
    {
        [Key]
        public int Id { get; set; }
        public string CustomMedia { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public virtual Reports Report { get; set; }
    }
}