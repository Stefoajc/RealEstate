using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foolproof;
using RealEstate.ViewModels.CustomDataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class SearchRentalViewModel
    {
        [PropertyDependOn("to", ErrorMessage = "Трябва дa изберете Начална дата")]
        public DateTime? from { get; set; } = null;

        [DateTimeBiggerThan("from", ErrorMessage = "Крайната дата трябва да е по-голяма от началната")]
        [PropertyDependOn("from", ErrorMessage = "Трябва дa изберете Крайна дата")]
        public DateTime? to { get; set; } = null;

        public int? rentPriceFrom { get; set; } = null;

        [IntBiggerThan("rentPriceFrom", ErrorMessage = "Крайната цената трябва да е по голяма от началната")]
        public int? rentPriceTo { get; set; } = null;

        public int? rentPropertyType { get; set; } = null;

        [PropertyDependOn("rentDistanceFromCity", ErrorMessage = "Не е избран град")]
        public int? rentCityId { get; set; } = null;

        public int? rentDistanceFromCity { get; set; } = null;
        public int pageCount { get; set; } = 1;
        public int pageSize { get; set; } = 6;
        public string sortBy { get; set; } = "PropertyName";
        public string orderBy { get; set; } = "Descending";
        public string searchType { get; set; } = "SearchRental";
    }

    public class SearchSellViewModel
    {
        public int? priceFrom { get; set; } = null;

        [IntBiggerThan("priceFrom", ErrorMessage = "Крайната цената трябва да е по голяма от началната")]
        public int? priceTo { get; set; } = null;

        public int? propertyType { get; set; } = null;

        [PropertyDependOn("distanceFromCity", ErrorMessage = "Не е избран град")]
        public int? cityId { get; set; } = null;

        public int? distanceFromCity { get; set; } = null;
        public int pageCount { get; set; } = 1;
        public int pageSize { get; set; } = 6;
        public string sortBy { get; set; } = "PropertyName";
        public string orderBy { get; set; } = "Descending";
        public string searchType { get; set; } = "SearchSell";
    }

    public class SearchFullViewModel
    {
        public SearchRentalViewModel SearchRental { get; set; } = new SearchRentalViewModel();
        public SearchSellViewModel SearchSell { get; set; } = new SearchSellViewModel();
    }

}
