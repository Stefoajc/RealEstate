using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstate.Model;

namespace RealEstate.ViewModels.WebMVC
{
    public class PropertyInfoFlatDTO
    {
        //------------------------
        public int Id { get; set; }

        //Type of property (Hotel,House...)
        public PropertyTypeInfoDTO UnitType { get; set; }

        public decimal? RentalPrice { get; set; }
        public string AdditionalDescription { get; set; }

        public int? AreaInSquareMeters { get; set; }

        public DateTime CreatedOn { get; set; }

        //How many time the Property has been viewed
        public long Views { get; set; } = 0;

        public RentalHirePeriodTypesInfoDTO RentalHirePeriodType { get; set; }

        public int? PropertyLikesCount { get; set; }
        public List<PropertyReviewInfoDTO> Reviews { get; set; } = new List<PropertyReviewInfoDTO>();
        public List<AttributesInfoDTO> Attributes { get; set; } = new List<AttributesInfoDTO>();


        //------------------------
        public AddressInfoDTO Address { get; set; }
        //When is the property expected to be engaged the most

        public PropertySeasonInfoDTO PropertySeason { get; set; }

        public OwnerUserInfoDTO Owner { get; set; }

        public AgentUsersInfoDTO Agent { get; set; }

        public string PropertyName { get; set; }

        //Set to false when the property is not receiving guests
        public bool IsActive { get; set; }

        public decimal? SellingPrice { get; set; }

        public List<PropertyImagesInfoDTO> Images { get; set; } = new List<PropertyImagesInfoDTO>();

        public List<PropertyExtrasInfoDTO> PropertyExtras { get; set; } = new List<PropertyExtrasInfoDTO>();

        // if its property with rentals in it use this
        public List<PropertyInfoDTO> PropertyRentals { get; set; } = new List<PropertyInfoDTO>();
    }


    public class PropertyInfoDTO
    {
        public PropertyInfoDTO()
        {

        }

        public PropertyInfoDTO(Properties property)
        {
            Id = property.Id;
            PropertyName = property.PropertyName;
            SellingPrice = property.SellingPrice;
            RentalPrice = property.RentalPrice;
            RentalHirePeriodType = new RentalHirePeriodTypesInfoDTO(property.RentalHirePeriodType);
            AreaInSquareMeters = property.AreaInSquareMeters;
            AdditionalDescription = property.AdditionalDescription;
            CreatedOn = property.CreatedOn;
            Views = property.Views;
            PropertyLikesCount = property.PropertyLikes.Count;
            UnitType = new PropertyTypeInfoDTO(property.UnitType);
            Address = new AddressInfoDTO(property.Address);
            PropertySeason = new PropertySeasonInfoDTO(property.PropertySeason);
            Owner = new OwnerUserInfoDTO(property.Owner);
            Agent = new AgentUsersInfoDTO(property.Agent);
            IsActive = property.IsActive;
            //PropertyRentals = property.Rentals.Select(r => new PropertyInfoDTO(r)).ToList();
        }

        public PropertyInfoDTO(RentalsInfo property)
        {
            Id = property.Id;
            PropertyName = property.Property.PropertyName;
            SellingPrice = property.Property.SellingPrice;
            RentalPrice = property.RentalPrice;
            RentalHirePeriodType = new RentalHirePeriodTypesInfoDTO(property.RentalHirePeriodType);
            AreaInSquareMeters = property.AreaInSquareMeters;
            AdditionalDescription = property.AdditionalDescription;
            CreatedOn = property.CreatedOn;
            Views = property.Views;
            PropertyLikesCount = property.PropertyLikes.Count;
            UnitType = new PropertyTypeInfoDTO(property.UnitType);
            Address = new AddressInfoDTO(property.Property.Address);
            PropertySeason = new PropertySeasonInfoDTO(property.Property.PropertySeason);
            Owner = new OwnerUserInfoDTO(property.Property.Owner);
            Agent = new AgentUsersInfoDTO(property.Property.Agent);
            IsActive = property.Property.IsActive;
            //PropertyRentals = new List<PropertyInfoDTO>();
        }

        //------------------------
        public int Id { get; set; }
        public string PropertyName { get; set; }
        //Set to false when the property is not receiving guests
        public bool IsActive { get; set; }
        //State of Property Available/Sold/Rented
        public PropertyState PropertyState { get; set; }
        //True for rentals / null for properties
        public bool IsRental { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? RentalPrice { get; set; }
        public string AdditionalDescription { get; set; }
        public int? AreaInSquareMeters { get; set; }
        public DateTime CreatedOn { get; set; }
        //How many time the Property has been viewed
        public long Views { get; set; } = 0;
        public int? PropertyLikesCount { get; set; }
        public string ImagePath { get; set; }
        public double? ReviewsAverage { get; set; }
        public int? ParentPropertyId { get; set; }

        public bool IsGreen { get; set; }

        public bool IsPartlyRented { get; set; }
        //Type of property (Hotel,House...)
        public PropertyTypeInfoDTO UnitType { get; set; }
        public RentalHirePeriodTypesInfoDTO RentalHirePeriodType { get; set; }
        public AddressInfoDTO Address { get; set; }        
        public PropertySeasonInfoDTO PropertySeason { get; set; }        
        public OwnerUserInfoDTO Owner { get; set; }
        public AgentUsersInfoDTO Agent { get; set; }
    }

    public class PropertyTypeInfoDTO
    {
        public PropertyTypeInfoDTO()
        {

        }

        public PropertyTypeInfoDTO(PropertyTypes propType)
        {
            PropertyTypeId = propType.PropertyTypeId;
            PropertyTypeName = propType.PropertyTypeName;
        }

        public int PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; }
    }

    public class RentalHirePeriodTypesInfoDTO
    {
        public RentalHirePeriodTypesInfoDTO()
        {

        }

        public RentalHirePeriodTypesInfoDTO(RentalHirePeriodsTypes hirePeriod)
        {
            Id = hirePeriod.Id;
            PeriodName = hirePeriod.PeriodName;
            IsTimePeriodSearchable = hirePeriod.IsTimePeriodSearchable;
        }

        public int? Id { get; set; }

        public string PeriodName { get; set; }
        //Is it gonna be shown in the Date to Date search
        //Only dayly, PerPersonDayly will be shown
        public bool IsTimePeriodSearchable { get; set; }
    }

    public class PropertyReviewInfoDTO
    {
        public PropertyReviewInfoDTO()
        {

        }

        public PropertyReviewInfoDTO(PropertyReviews review)
        {
            ReviewScore = review.ReviewScore;
            ReviewText = review.ReviewText;
        }

        public int? ReviewScore { get; set; }
        public string ReviewText { get; set; }
    }

    public class AttributesInfoDTO
    {
        public AttributesInfoDTO()
        {

        }

        public AttributesInfoDTO(KeyValuePairs pair)
        {
            Key = pair.Key;
            Value = pair.Value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class AddressInfoDTO
    {
        public AddressInfoDTO()
        {

        }

        public AddressInfoDTO(Addresses address)
        {
            FullAddress = address.FullAddress;
            Coordinates = new CoordinatesInfoDTO(address.Coordinates);
            City = new CityInfoDTO(address.City);
        }

        public string FullAddress { get; set; }

        public CoordinatesInfoDTO Coordinates { get; set; }
        public CityInfoDTO City { get; set; }

    }

    public class CityInfoDTO
    {
        public CityInfoDTO()
        {

        }

        public CityInfoDTO(Cities city)
        {
            CityId = city.CityId;
            CityName = city.CityName;
            PostalCode = city.PostalCode;
            PhoneCode = city.PhoneCode;
            CityCode = city.CityCode;
            Latitude = city.Latitude;
            Longitude = city.Longitude;
            Country = new CountryInfoDTO(city.Country);
        }

        public int CityId { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string PhoneCode { get; set; }
        public string CityCode { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public CountryInfoDTO Country { get; set; }
    }

    public class CountryInfoDTO
    {
        public CountryInfoDTO()
        {

        }

        public CountryInfoDTO(Countries country)
        {
            CountryNameBG = country.CountryNameBG;
            CountryPhoneCode = country.CountryPhoneCode;
        }
        public string CountryNameBG { get; set; }
        public int CountryPhoneCode { get; set; }
    }

    public class CoordinatesInfoDTO
    {
        public CoordinatesInfoDTO()
        {

        }

        public CoordinatesInfoDTO(Coordinates coords)
        {
            Latitude = coords.Latitude;
            Longtitude = coords.Longtitude;
        }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
    }

    public class PropertySeasonInfoDTO
    {
        public PropertySeasonInfoDTO()
        {

        }

        public PropertySeasonInfoDTO(PropertySeason season)
        {
            PropertySeasonId = season.PropertySeasonId;
            PropertySeasonName = season.PropertySeasonName;
        }

        public int? PropertySeasonId { get; set; }
        public string PropertySeasonName { get; set; }
    }

    public class OwnerUserInfoDTO
    {
        public OwnerUserInfoDTO()
        {

        }

        public OwnerUserInfoDTO(OwnerUsers owner)
        {
            OwnerId = owner.Id;
            FullName = owner.FirstName + " " + owner.LastName;
            Email = owner.Email;
            ImagePath = owner.Images.Select(i => i.ImagePath).FirstOrDefault();
        }

        public string OwnerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
    }

    public class AgentUsersInfoDTO
    {
        public AgentUsersInfoDTO()
        {

        }

        public AgentUsersInfoDTO(AgentUsers agent)
        {
            AgentId = agent.Id;
            FullName = agent.FirstName + " " + agent.LastName;
            AdditionalDescription = agent.AdditionalDescription;
            PhoneNumber = agent.PhoneNumber;
            Email = agent.Email;
            ImagePath = agent.Images.Select(i => i.ImagePath).FirstOrDefault();
        }

        public string AgentId { get; set; }
        public string FullName { get; set; }
        public string AdditionalDescription { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }       
    }

    public class SocialMediaAccountInfoDTO
    {
        public SocialMediaAccountInfoDTO()
        {

        }

        public SocialMediaAccountInfoDTO(SocialMediaAccounts socialMediaAccount)
        {
            SocialMedia = socialMediaAccount.SocialMedia;
            SocialMediaAccount = socialMediaAccount.SocialMediaAccount;
        }
        public string SocialMedia { get; set; }
        public string SocialMediaAccount { get; set; }
    }

    public class PropertyImagesInfoDTO
    {
        public PropertyImagesInfoDTO()
        {

        }

        public PropertyImagesInfoDTO(Images image)
        {
            ImagePath = image.ImagePath;
            ImageRatio = image.ImageRatio;
        }

        public string ImagePath { get; set; }
        // Width / Height
        public float ImageRatio { get; set; }
    }

    public class PropertyExtrasInfoDTO
    {
        public PropertyExtrasInfoDTO()
        {

        }

        public PropertyExtrasInfoDTO(Extras extra)
        {
            ExtraName = extra.ExtraName;
        }

        public string ExtraName { get; set; }
    }
}
