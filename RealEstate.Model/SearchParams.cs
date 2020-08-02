using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class SearchParams
    {
        [Key]
        public int ParamId { get; set; }

        public string ParamName { get; set; }
        public string ParamValue { get; set; }


        [ForeignKey("SearchParamsTracking")]
        public int SearchParamsCollectionId { get; set; }
        public virtual SearchParamsTracking SearchParamsTracking { get; set; }

    }
}
