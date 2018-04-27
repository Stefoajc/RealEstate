using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class UnitTypes
    {
        private UnitTypes(UnitType @enum)
        {
            RentalTypeId = (int)@enum;
            RentalTypeName = @enum.ToString();
        }

        protected UnitTypes() { }

        [Key, EnumDataType(typeof(UnitType)), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RentalTypeId { get; set; }

        [Required, MaxLength(50)]
        public string RentalTypeName { get; set; }

        public virtual ISet<RentalsInfo> RentalsInfo { get; set; } = new HashSet<RentalsInfo>();

        public virtual ISet<SellingInfo> SellingsInfo { get; set; } = new HashSet<SellingInfo>();

        public static implicit operator UnitTypes(UnitType @enum) => new UnitTypes(@enum);

        public static implicit operator UnitType(UnitTypes unitType) => (UnitType)unitType.RentalTypeId;
    }

    public enum UnitType
    {
        Апартамент,//Apartement
        Стая,//Room
        Студио,//Studio
        Етаж,//Flat
        Офис,//Office
        Гараж,//Garage
        Магазин,//Shop
        Друго
    }
}
