using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Reports
{
    public class Offers
    {
        public int Id { get; set; }
        public decimal Offer { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public virtual Reports Report { get; set; }
    }
}
