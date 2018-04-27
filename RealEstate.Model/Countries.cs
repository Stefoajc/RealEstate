using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model
{
    public class Countries
    {
        public Countries()
        {
        }


        [Key]
        public int CountryId { get; set; }
        public string CountryNameBG { get; set; }
        public string CountryNameEN { get; set; }
        
        /// <summary>
        /// This property represents the abbraviation of the County name (ex. GB for Great Britain , BG for Bulgaria)
        /// </summary>
        [StringLength(3)]
        public string IsoCode { get; set; }

        /// <summary>
        /// This property represents the starting of the phone number (ex. Bulgaria-359 , Russia - 7)
        /// </summary>
        public int CountryPhoneCode { get; set; }

        public virtual ISet<Cities> Cities { get; set; } = new HashSet<Cities>();
    }
}
