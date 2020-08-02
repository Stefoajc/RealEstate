using System;
using System.Collections.Generic;
using System.Linq;
using RealEstate.ViewModels.CustomDataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class SearchAllViewModel
    {
        public SearchAllViewModel() { }

        public SearchAllViewModel(SearchMapViewModel model, string viewType = "ListProperties")
        {
            this.IsForRent = model.IsForRent;
            this.To = model.To;
            this.From = model.From;
            this.IsShortPeriodRent = model.IsShortPeriodRent;
            this.PropertyType = model.PropertyType;
            this.AreaFrom = model.AreaFrom;
            this.AreaTo = model.AreaTo;
            this.PriceFrom = model.PriceFrom;
            this.PriceTo = model.PriceTo;
            this.CityId = model.CityId;
            this.DistanceFromCity = model.DistanceFromCity;
            this.Extras = model.Extras;

            this.PageCount = 1;
            this.PageSize = 6;
            this.SortBy = "PropertyName";
            this.OrderBy = "Descending";

            this.ViewType = viewType;
        }

        public SearchAllViewModel(SearchAllViewModel model, 
            bool? isForRent = null, bool? isShortPeriodRent = null,
            DateTime? from = null, DateTime? to = null,
            int? priceFrom = null, int? priceTo = null,            
            int? propertyType = null,
            int? areaFrom = null, int? areaTo = null,
            int? cityId = null, int? distanceFromCity = null,
            List<int> extras = null,
            int? pageCount = 1, int? pageSize = null,
            string sortBy = "", string orderBy = "",
            string viewType = "")
        {
            this.IsForRent = isForRent ?? model.IsForRent;

            this.From = from ?? model.From;
            this.To = to ?? model.To;
            this.IsShortPeriodRent = isShortPeriodRent ?? model.IsShortPeriodRent;

            this.PropertyType = propertyType == -1 ? null : propertyType ?? model.PropertyType;
            this.AreaFrom = areaFrom == -1 ? null : areaFrom ?? model.AreaFrom;
            this.AreaTo = areaTo == -1 ? null : areaTo ?? model.AreaTo;
            this.PriceFrom = priceFrom == -1 ? null : priceFrom ?? model.PriceFrom;
            this.PriceTo = priceTo == -1 ? null : priceTo ?? model.PriceTo;
            this.CityId = cityId == -1 ? null : cityId ?? model.CityId;
            this.DistanceFromCity = distanceFromCity == -1 ? null : distanceFromCity ?? model.DistanceFromCity;
            this.PageCount = pageCount ?? model.PageCount;
            this.PageSize = pageSize ?? model.PageSize;
            this.SortBy = !string.IsNullOrEmpty(sortBy) ? sortBy : model.SortBy;
            this.OrderBy = !string.IsNullOrEmpty(orderBy) ? orderBy : model.OrderBy;
            this.ViewType = !string.IsNullOrEmpty(viewType) ? viewType : model.ViewType;

            this.Extras = extras ?? model.Extras;
        }

        public bool? IsForRent { get; set; }
        public bool? IsShortPeriodRent { get; set; }

        //Depend on IsForRent=True
        [PropertyDependOn("To", ErrorMessage = "Трябва дa изберете Начална дата")]
        public DateTime? From { get; set; } = null;
        [DateTimeBiggerThan("From", ErrorMessage = "Крайната дата трябва да е по-голяма от началната")]
        [PropertyDependOn("From", ErrorMessage = "Трябва дa изберете Крайна дата")]
        public DateTime? To { get; set; } = null;



        public int? AreaFrom { get; set; } = null;
        [IntBiggerThan("AreaFrom", ErrorMessage = "Крайната площ трябва да е по голяма от началната")]
        public int? AreaTo { get; set; } = null;

        public int? PriceFrom { get; set; } = null;
        [IntBiggerThan("PriceFrom", ErrorMessage = "Крайната цената трябва да е по голяма от началната")]
        public int? PriceTo { get; set; } = null;

        public int? PropertyType { get; set; } = null;

        [PropertyDependOn("DistanceFromCity", ErrorMessage = "Не е избран град")]
        public int? CityId { get; set; } = null;
        public int? DistanceFromCity { get; set; } = null;

        public List<int> Extras { get; set; } = null;

        public int PageCount { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public string SortBy { get; set; } = "PropertyName";
        public string OrderBy { get; set; } = "Descending";

        public string ViewType { get; set; } = "ListProperties";

    }

    public class SearchMapViewModel
    {
        public SearchMapViewModel() { }

        public SearchMapViewModel(int? propertyType = null,
            DateTime? from = null, DateTime? to = null,
            bool? isShortPeriodRent = null,
            int? areaFrom = null, int? areaTo = null,
            int? priceFrom = null, int? priceTo = null,
            int? cityId = null, int? distanceFromCity = null)
        {
            this.PropertyType = propertyType;
            this.AreaFrom = areaFrom;
            this.AreaTo = areaTo;
            this.PriceFrom = priceFrom;
            this.PriceTo = priceTo;
            this.CityId = cityId;
            this.DistanceFromCity = distanceFromCity;
            this.IsForRent = false;
            this.From = from;
            this.To = to;
            this.IsShortPeriodRent = isShortPeriodRent;
        }

        public SearchMapViewModel(SearchAllViewModel model)
        {
            this.PropertyType = model.PropertyType;
            this.AreaFrom = model.AreaFrom;
            this.AreaTo = model.AreaTo;
            this.PriceFrom = model.PriceFrom;
            this.PriceTo = model.PriceTo;
            this.CityId = model.CityId;
            this.DistanceFromCity = model.DistanceFromCity;
            this.IsForRent = model.IsForRent;
            this.From = model.From;
            this.To = model.To;
            this.IsShortPeriodRent = model.IsShortPeriodRent;
            this.Extras = model.Extras;
        }

        public SearchMapViewModel(SearchMapViewModel model, int? propertyType = null,
            DateTime? from = null, DateTime? to = null,
            int? rentalPricePeriodId = null,
            int? areaFrom = null, int? areaTo = null,
            int? priceFrom = null, int? priceTo = null,
            int? cityId = null, int? distanceFromCity = null,
            List<int> extras = null)
        {
            this.PropertyType = propertyType == -1 ? null : propertyType ?? model.PropertyType;
            this.AreaFrom = areaFrom == -1 ? null : areaFrom ?? model.AreaFrom;
            this.AreaTo = areaTo == -1 ? null : areaTo ?? model.AreaTo;
            this.PriceFrom = priceFrom == -1 ? null : priceFrom ?? model.PriceFrom;
            this.PriceTo = priceTo == -1 ? null : priceTo ?? model.PriceTo;
            this.CityId = cityId == -1 ? null : cityId ?? model.CityId;
            this.DistanceFromCity = distanceFromCity == -1 ? null : distanceFromCity ?? model.DistanceFromCity;
            this.IsForRent = model.IsForRent;
            this.From = from ?? model.From;
            this.To = to ?? model.To;
            this.IsShortPeriodRent = IsShortPeriodRent ?? model.IsShortPeriodRent;

            this.Extras = extras ?? model.Extras;
        }

        public bool? IsForRent { get; set; } = false;
        public bool? IsShortPeriodRent { get; set; }

        //Depend on IsForRent=True
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }


        public int? AreaFrom { get; set; } = null;
        [IntBiggerThan("AreaFrom", ErrorMessage = "Крайната площ трябва да е по голяма от началната")]
        public int? AreaTo { get; set; } = null;

        public int? PriceFrom { get; set; } = null;
        [IntBiggerThan("PriceFrom", ErrorMessage = "Крайната цена трябва да е по голяма от началната")]
        public int? PriceTo { get; set; } = null;

        public int? PropertyType { get; set; } = null;

        [PropertyDependOn("DistanceFromCity", ErrorMessage = "Не е избран град")]
        public int? CityId { get; set; } = null;
        public int? DistanceFromCity { get; set; } = null;

        public List<int> Extras { get; set; } = null;
    }    
}
