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
        [Required(ErrorMessage = "Наемната цена е задължителен")]
        public decimal RentalPrice { get; set; }
        [Required(ErrorMessage = "Периодът на наема е задължителен")]
        public RentalPeriod RentalPricePeriod { get; set; }
        public string AdditionalInfo { get; set; }

        public List<ExtraCheckBoxViewModel> RentalExtras { get; set; } = new List<ExtraCheckBoxViewModel>();
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();
    }

    public class EditRentalInfoForPropertyViewModel
    {
        public int? RentalInfoId { get; set; }
        public int RentalTypeId { get; set; }
        public int RoomsCount { get; set; }
        public byte BedsCount { get; set; }
        public string AdditionalInfo { get; set; }

        public List<int> RentalExtrasIds = new List<int>();
        public AddRentalPriceForPeriodViewModel RentalPrices;
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