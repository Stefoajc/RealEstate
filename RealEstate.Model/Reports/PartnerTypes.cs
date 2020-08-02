using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.Reports
{
    public class PartnerTypes
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }

        public string TypeColor { get; set; }

        public virtual List<Partners> Partners { get; set; }
    }
}
