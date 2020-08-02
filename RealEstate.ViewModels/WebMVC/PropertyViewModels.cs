using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        public int? RentalPricePeriodId { get; set; }

        [AllowHtml, HtmlValidation(ErrorMessage = "Опитвате се да въведете опасно съдържание!")]
        public string AdditionalDescription { get; set; }

        public int? PropertySeasonId { get; set; }
        [Required(ErrorMessage = "Типът на имота е задължително поле")]
        public int PropertyTypeId { get; set; }
        /// <summary>
        /// Represents the Rental Information
        /// </summary>
        /// <example>a property has  {5 3bed rooms with additional information for them with collection of extras and prices for each period (like month) // 3 2bed rooms with some info} </example>
        public List<AddRentalInfoToPropertyViewModel> RentalsInfo { get; set; } = new List<AddRentalInfoToPropertyViewModel>();
        [EnsureOneItem(ErrorMessage = "Снимките на имота са задължителни")]
        [FileType("JPG,JPEG,PNG")]
        public List<HttpPostedFileBase> ImageFiles { get; set; } = new List<HttpPostedFileBase>();
        [EnsureOneItem(ErrorMessage = "Снимките на имота са задължителни")]
        [FileType("JPG,JPEG,PNG")]
        public List<HttpPostedFileBase> ImageFilesForSlider { get; set; } = new List<HttpPostedFileBase>();
        public List<ExtraCheckBoxViewModel> PropertyExtrasCheckBoxes { get; set; } = new List<ExtraCheckBoxViewModel>();
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();

        [Required]
        public string OwnerId { get; set; }
    }

    public class EditPropertyViewModel
    {
        public int PropertyId { get; set; }
        [Required(ErrorMessage = "Името на имота е задължително поле")]
        public string PropertyName { get; set; }
        [AllowHtml]
        public string AdditionalDescription { get; set; }
        public int? PropertySeasonId { get; set; }
        public int PropertyTypeId { get; set; }
        public int? Area { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? RentalPrice { get; set; }
        public int? RentalPeriodId { get; set; }
        public string OwnerId { get; set; }
        [Required(ErrorMessage = "Адресът е задължително поле")]
        public CreateAddressViewModel Address { get; set; }

        public List<ImageEditViewModel> Images { get; set; } = new List<ImageEditViewModel>();
        public List<ExtraCheckBoxViewModel> PropertyExtrasCheckBoxes { get; set; } = new List<ExtraCheckBoxViewModel>();
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();
    }

    public class DetailsPropertyViewModel
    {
        //if client is searching rentals show them
        //rental info of the Property
        public bool IsRentSearch { get; set; }

        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string AdditionalDescription { get; set; }
        public int? AreaInSquareMeters { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? RentalPrice { get; set; }
        public string RentalPeriod { get; set; }

        public string BedroomsCount { get; set; }
        public string BathroomsCount { get; set; }
        public string RoomsCount { get; set; }
        public string VideoUrl { get; set; }

        public long Views { get; set; }
        public long LikesCount { get; set; }
        public int PropertyState { get; set; }

        public DetailsAddressViewModel Address { get; set; }

        public string PropertySeason { get; set; }
        public string PropertyType { get; set; }

        public ReviewsStarsPartialViewModel ReviewsInfo { get; set; }
        public List<ReviewListViewModel> ClientReviews { get; set; } = new List<ReviewListViewModel>();
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Extras { get; set; } = new List<string>();
        public List<PropertyInfoViewModel> Rentals { get; set; } = new List<PropertyInfoViewModel>();
        public PropertyInfoViewModel PropertyParent { get; set; } = null;
        public List<AttributesKeyValueViewModel> Attributes { get; set; } = new List<AttributesKeyValueViewModel>();

        public TeamUserListViewModel TeamUser { get; set; }
        public OwnerViewModel Owner { get; set; }
    }

    public class ChangePropertyStatusViewModel
    {
        public int PropertyId { get; set; }
        public string State { get; set; }
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
        public string PriceDescription { get; set; }
        public string FullAddress { get; set; }
        public string AdditionalDescription { get; set; }
        public string ImagePath { get; set; }
        public int PropertyState { get; set; }
    }


    public class PropertyBriefInfoViewModel
    {
        public int PropertyId { get; set; }
        public string FullAddress { get; set; }
        public string PropertyType { get; set; }
        public string ImagePath { get; set; }
        public string PropertyName { get; set; }
        public int PropertyState { get; set; }
    }

    public class PropertiesRelatedViewModel
    {
        public int PropertyId { get; set; }
        public string FullAddress { get; set; }
        public string PropertyName { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
        public string BottomLeft { get; set; }
        public string BottomRight { get; set; }
    }

    public class PropertiesInfoViewModel
    {
        public List<PropertyInfoViewModel> Properties { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// This model is used in the home page for loading speed boost
    /// </summary>
    public class PropertiesHomePageAggregatedViewModel
    {
        public List<PropertyInfoViewModel> PropertiesForSell { get; set; }
        public List<PropertyInfoViewModel> HousesForSell { get; set; }
        public List<PropertyInfoViewModel> OfficesForSell { get; set; }
        public List<PropertyInfoViewModel> ApartmentsForSell { get; set; }

        public List<PropertyInfoViewModel> PropertiesForRent { get; set; }
        public List<PropertyInfoViewModel> HousesForRent { get; set; }
        public List<PropertyInfoViewModel> OfficesForRent { get; set; }
        public List<PropertyInfoViewModel> ApartmentsForRent { get; set; }
    }

    public class PropertiesInfoDTO
    {
        public IQueryable<PropertyInfoDTO> Properties { get; set; }
        public int Count { get; set; }
    }

    public class PropertyInfoViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; } // Where the property will be sorted Office/House
        public long Views { get; set; }
        public string Status { get; set; } // Status of property
        public int PropertyState { get; set; } // State of the property 0-Available/ 1-Sold / 2-Rented
        public decimal Price { get; set; } // Selling price
        public string ImagePath { get; set; } // Image
        public string Info { get; set; } // Most important info
        public string CityName { get; set; }
        public string FullAddress { get; set; } // Full Address of the Property
        public int AreaInSquareMeters { get; set; } // AreaInSquareFt in кв.м
        public string BottomLeft { get; set; }
        public string BottomRight { get; set; }
        public bool IsPartlyRented { get; set; }
        //For showing the details of the Rental not the property
        public bool IsRentalProperty { get; set; }

        //For Properties which also have rentalPrice
        public bool IsRentable { get; set; }

        public double? ReviewsAverage { get; set; }
        public DateTime CreatedOn { get; set; }

        public bool IsGreen { get; set; }
    }

    public class PropertyDropDownViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
    }


    public class PropertyMapViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public List<string> ImagePaths { get; set; }
        public string FullAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal Price { get; set; }
        public string Period { get; set; }
        public bool HasRentals { get; set; }
    }



    public class AttributesKeyValueViewModel
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }


    public class ManageProperties
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
    }

}
