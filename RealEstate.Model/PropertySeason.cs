using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class PropertySeason
    {

        private PropertySeason(RentPeriod @enum)
        {
            PropertySeasonId = (int)@enum;
            PropertySeasonName = @enum.ToString();
        }

        protected PropertySeason() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PropertySeasonId { get; set; }

        [Required, MaxLength(30)]
        public string PropertySeasonName { get; set; }

        public virtual ISet<Properties> Properties { get; set; } = new HashSet<Properties>();

        public static implicit operator PropertySeason(RentPeriod @enum) => new PropertySeason(@enum);

        public static implicit operator RentPeriod(PropertySeason period) => (RentPeriod)period.PropertySeasonId;
    }


    public enum RentPeriod
    {
        Целогодишно,//AllYear,
        Лято,//Summer,
        Зима,//Winter,
        Пролет,//Spring,
        Есен,//Autumn,
        Празници//Holidays
    }
}
