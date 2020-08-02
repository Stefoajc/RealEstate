using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Model;

namespace RealEstate.ViewModels.WebMVC
{
    public class RentalInfoViewModels
    {
    }

    public class CreateRentalInfoViewModel
    {
        [Required(ErrorMessage = "Типът на помещенията е задължителен")]
        public int UnitTypeId { get; set; }
        [Required(ErrorMessage = "Броят на помещенията е задължителен")]
        public int UnitsCount { get; set; }
        [Required(ErrorMessage = "Наемната цена е задължителенa")]
        public decimal RentalPrice { get; set; }
        [Required(ErrorMessage = "Периодът на наема е задължителен")]
        public int RentalPricePeriodId { get; set; }
        public string AdditionalInfo { get; set; }

        [Required(ErrorMessage = "Не е избран имот на ,който това е част!")]
        public int PropertyId { get; set; }
        public List<ExtraCheckBoxViewModel> RentalExtras { get; set; } = new List<ExtraCheckBoxViewModel>();
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();
    }

    public class AddRentalInfoToPropertyViewModel
    {
        [Required(ErrorMessage = "Типът на помещенията е задължителен")]
        public int UnitTypeId { get; set; }
        [Required(ErrorMessage = "Броят на помещенията е задължителен")]
        public int UnitsCount { get; set; }
        [Required(ErrorMessage = "Наемната цена е задължителенa")]
        public decimal RentalPrice { get; set; }
        [Required(ErrorMessage = "Периодът на наема е задължителен")]
        public int RentalPricePeriodId { get; set; }
        public string AdditionalInfo { get; set; }

        public List<ExtraCheckBoxViewModel> RentalExtras { get; set; } = new List<ExtraCheckBoxViewModel>();
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();
    }

    public class EditRentalInfoForPropertyViewModel
    {
        public int? RentalInfoId { get; set; }
        [Required(ErrorMessage = "Типът на помещенията е задължителен")]
        public int UnitTypeId { get; set; }
        [Required(ErrorMessage = "Броят на помещенията е задължителен")]
        public int UnitCount { get; set; }
        [Required(ErrorMessage = "Наемната цена е задължителенa")]
        public decimal RentalPrice { get; set; }
        [Required(ErrorMessage = "Периодът на наема е задължителен")]
        public int RentalPricePeriodId { get; set; }
        public string AdditionalInfo { get; set; }

        public List<ExtraCheckBoxViewModel> RentalExtras { get; set; } = new List<ExtraCheckBoxViewModel>();
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();
    }

    public class EditRentalInfoViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public int? RentalInfoId { get; set; }
        [Required(ErrorMessage = "Типът на помещенията е задължителен")]
        public int UnitTypeId { get; set; }
        [Required(ErrorMessage = "Броят на помещенията е задължителен")]
        public int UnitCount { get; set; }
        [Required(ErrorMessage = "Наемната цена е задължителенa")]
        public decimal RentalPrice { get; set; }
        [Required(ErrorMessage = "Периодът на наема е задължителен")]
        public int RentalPricePeriodId { get; set; }
        public string AdditionalInfo { get; set; }

        public List<ExtraCheckBoxViewModel> RentalExtras { get; set; } = new List<ExtraCheckBoxViewModel>();
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();
    }

    public class RentalInfoDetails
    {
        public string UnitType { get; set; }
        public int UnitCount { get; set; }
        public decimal Price { get; set; }
        public string PricePeriodType { get; set; }
        public string AdditionalInfo { get; set; }

        public List<AttributesKeyValueViewModel> RentalAttributes { get; set; }
    }

    public class RentalInfoBedsRoomsAvailableViewModel
    {
        public int BedsCount { get; set; }
        public int AvailableRooms { get; set; }
    }
}