using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using RealEstate.Model;
using RealEstate.ViewModels.CustomDataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{

    public class CreatePropertyViewModel
    {
        [Required(ErrorMessage = "Адресът е задължително поле")]
        public CreateAddressViewModel Address { get; set; }
        [Required(ErrorMessage = "Името на имота е задължително поле")]
        public string PropertyName { get; set; }
        public int? Area { get; set; }

        public decimal? SellingPrice { get; set; }
        public decimal? RentalPrice { get; set; }
        public RentalPeriod? RentPricePeriod { get; set; }

        public string AdditionalDescription { get; set; }

        public int? PropertySeasonId { get; set; }
        [Required(ErrorMessage = "Типът на имота е задължително поле")]
        public int PropertyTypeId { get; set; }
        /// <summary>
        /// Represents the Rental Information
        /// </summary>
        /// <example>a property has  {5 3bed rooms with additional information for them with collection of extras and prices for each period (like month) // 3 2bed rooms with some info} </example>
        public List<CreateRentalInfoViewModel> RentalsInfo { get; set; } = new List<CreateRentalInfoViewModel>();
        [EnsureOneItem(ErrorMessage = "Снимките на имота са задължителни")]
        public List<HttpPostedFileBase> ImageFiles { get; set; } = new List<HttpPostedFileBase>();
        [EnsureOneItem(ErrorMessage = "Снимките на имота са задължителни")]
        public List<HttpPostedFileBase> ImageFilesForSlider { get; set; } = new List<HttpPostedFileBase>();
        public List<ExtraCheckBoxViewModel> PropertyExtrasCheckBoxes { get; set; } = new List<ExtraCheckBoxViewModel>();
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();

        [Required]
        public string OwnerId { get; set; }
    }

    public class EditPropertyViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string AdditionalDescription { get; set; }
        public int? PropertySeasonId { get; set; }

        public List<ImageViewModels> Images { get; set; } = new List<ImageViewModels>();
        public List<EditRentalInfoForPropertyViewModel> RentalsInfo = new List<EditRentalInfoForPropertyViewModel>();
        public List<ExtraCheckBoxViewModel> PropertyExtrasCheckBoxes { get; set; } = new List<ExtraCheckBoxViewModel>();
    }

    public class DetailsPropertyViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string AdditionalDescription { get; set; }
        public int? AreaInSquareFt { get; set; }
        public decimal? SellingPrice { get; set; }
        public string BedroomsCount { get; set; }
        public string BathroomsCount { get; set; }
        public string RoomCount { get; set; }

        public long Views { get; set; }
        public long LikesCount { get; set; }


        public DetailsAddressViewModel Address { get; set; }

        public string PropertySeason { get; set; }
        public string PropertyType { get; set; }

        public List<object> Reviews { get; set; }
        public List<string> Images { get; set; }
        public List<object> Extras { get; set; }
        public List<RentalInfoDetails> Rentals { get; set; }
        public List<AttributesKeyValueViewModel> Attributes { get; set; }

        public AgentListViewModel Agent { get; set; }
    }



    public class ListPropertyViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string AdditionalDescription { get; set; }

        public long Views { get; set; }
        public long LikesCount { get; set; }
        public double ReviewScoreAvg { get; set; }
        public int ReviewCount { get; set; }

        public string CityName { get; set; }
        public string FullAddress { get; set; }

        public string PropertySeason { get; set; }
        public string PropertyType { get; set; }
    }


    public class DropDownPropertySeasonsViewModel
    {
        public int PropertySeasonId { get; set; }
        public string PropertySeasonName { get; set; }
    }

    public class DropDownPropertyTypesViewModel
    {
        public int PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; }
    }


    public class PropertySliderViewModel
    {
        public int PropertyId { get; set; }
        public decimal SellingPrice { get; set; }
        public string FullAddress { get; set; }
        public string AdditionalDescription { get; set; }
        public string ImagePath { get; set; }
    }


    public class PropertyBriefInfoViewModel
    {
        public int PropertyId { get; set; }
        public string FullAddress { get; set; }
        public string ImagePath { get; set; }
        public string PropertyName { get; set; }
    }

    public class PropertiesInfoViewModel
    {
        public List<PropertyInfoViewModel> Properties { get; set; }
        public int Count { get; set; }
    }

    public class PropertyInfoViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; } // Where the property will be sorted Office/House
        public long Views { get; set; }
        public string Status { get; set; } // Status of property
        public decimal Price { get; set; } // Selling price
        public string ImagePath { get; set; } // Image
        public string Info { get; set; } // Most important info
        public string FullAddress { get; set; } // Full Address of the Property
        public int AreaInSquareFt { get; set; } // AreaInSquareFt in кв.м
        public string BottomLeft { get; set; }
        public string BottomRight { get; set; }
    }

    public class PropertyForRentInfoViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; } // Where the property will be sorted Office/House
        public long Views { get; set; }
        public string Status { get; set; } // Status of property
        public decimal Price { get; set; } // Selling price
        public string ImagePath { get; set; } // Image
        public string Info { get; set; } // Most important info
        public string FullAddress { get; set; } // Full Address of the Property
        public int? RoomsCount { get; set; }
        public string BottomLeft { get; set; }
        public string BottomRight { get; set; }
    }

    public class PropertyFullWidthViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string Info { get; set; }
        public string FullAddress { get; set; }
        public string ImagePath { get; set; }
        public float Area { get; set; }
        public int BedroomsCount { get; set; }
        public int BathroomsCount { get; set; }
        public int Price { get; set; }
    }



    public class AttributesKeyValueViewModel
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
