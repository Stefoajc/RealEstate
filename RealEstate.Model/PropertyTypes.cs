using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class PropertyTypes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PropertyTypeId { get; set; }

        [Required, MaxLength(50)]
        public string PropertyTypeName { get; set; }

        public bool IsPropertyOnly { get; set; }

        public virtual ISet<PropertiesBase> Properties { get; set; } = new HashSet<PropertiesBase>();
        public virtual ISet<PropertySearches.PropertySearches> PropertySearches { get; set; } = new HashSet<PropertySearches.PropertySearches>();
    }


    public enum PropertyType
    {
        Хотел,//Hotel
        Къща,//House
        Мотел,//Motel
        Станция,//Station
        [Display(Name = "Офис сграда")]
        ОфисСграда,//OfficeBuilding
        Офис,
        Вила,//Vila
        Етаж,
        Паркинг,//Parking
        ПаркоМясто,
        Гараж,//Garage
        Земя,//Land
        Магазин,
        [Display(Name = "Търговски Център")]
        ТърговскиЦентър,//AppartmentBuilding
        Сграда,
        Склад,
        Стаи,
        Друго,//Other
    }
}
