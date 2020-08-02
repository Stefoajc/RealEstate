using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model
{
    public class RentalHirePeriodsTypes
    {
        [Key]
        public int Id { get; set; }

        public string PeriodName { get; set; }
        //Is it gonna be shown in the Date to Date search
        //Only dayly, PerPersonDayly will be shown
        public bool IsTimePeriodSearchable { get; set; }

        public virtual ISet<PropertiesBase> Properties { get; set; } = new HashSet<PropertiesBase>();
    }
}
