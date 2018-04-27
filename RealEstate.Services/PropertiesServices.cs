using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using AutoMapper;
using Microsoft.AspNet.Identity;
using RealEstate.Extentions;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    /// <summary>
    /// Property services used from the owner to add property and its attributes
    /// </summary>
    public class PropertiesServices : BaseService
    {
        private const string BedroomCount = "BedroomCount";
        private const string BathroomCount = "BathroomCount";
        private const string RoomCount = "RoomCount";

        private readonly ImageServices _imagesManager;
        private readonly AddressServices _addressManager;
        private readonly RentalInfoServices _rentalsInfoManager;
        private readonly ExtraServices _propertyExtrasManager;
        private readonly CityServices _citiesManager;

        public PropertiesServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr, CityServices cityServices, ImageServices imageServices, AddressServices addressServices, RentalInfoServices rentalInfoServices, ExtraServices extraServices) : base(unitOfWork, user, userMgr)
        {
            _imagesManager = imageServices;
            _addressManager = addressServices;
            _rentalsInfoManager = rentalInfoServices;
            _propertyExtrasManager = extraServices;
            _citiesManager = cityServices;
        }

        /// <summary>
        /// Service for adding a full featured Property
        /// </summary>
        /// <param name="model">Full featured property</param>
        public void AddProperty(CreatePropertyViewModel model)
        {
            Properties property = new Properties()
            {
                IsActive = true,
                CreatedOn = DateTime.Now,
                PropertyName = model.PropertyName,
                AdditionalDescription = model.AdditionalDescription,
                PropertySeasonId = model.PropertySeasonId,
                AgentId = User.Identity.GetUserId(),
                OwnerId = model.OwnerId,
                SellingPrice = model.SellingPrice,
                RentalPrice = model.RentalPrice,
                RentPricePeriod = (RentalPeriod)model.RentPricePeriod,
                Views = 0
            };

            //Add extras to the property
            property.PropertyExtras = new HashSet<PropertyExtras>(_propertyExtrasManager.CreatePropertyExtras(model.PropertyExtrasCheckBoxes.Where(p => p.IsChecked).Select(p => p.ExtraId).ToList()));

            //Add images to the property
            var regularImages = _imagesManager.CreatePropertyImages(model.ImageFiles);
            var sliderImages = new HashSet<PropertyImages>(_imagesManager.CreatePropertyImages(model.ImageFilesForSlider, true));
            property.Images = new HashSet<PropertyImages>(regularImages.Union(sliderImages));

            //Add Address
            property.Address = _addressManager.AddAddress(model.Address);

            // Rental Info Handle HERE !
            property.Rentals = new HashSet<RentalsInfo>(_rentalsInfoManager.AddRentalInfo(model.RentalsInfo));

            //Add property to the database
            UnitOfWork.PropertiesRepository.Add(property);
            UnitOfWork.Save();
        }

        /// <summary>
        /// Get property for edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EditPropertyViewModel GetProperty(int id)
        {
            var propToEdit = UnitOfWork.PropertiesRepository
                .FindBy(p => p.PropertyId == id)
                .Include(p => p.Rentals)
                .Include(p => p.Images)
                .Include(p => p.PropertyExtras)
                .Select(p => new EditPropertyViewModel
                {
                    PropertyId = p.PropertyId,
                    AdditionalDescription = p.AdditionalDescription,
                    PropertyName = p.PropertyName,
                    PropertySeasonId = p.PropertySeasonId,
                    Images = p.Images.Select(i => new ImageViewModels
                    {
                        ImagePath = i.ImagePath,
                        ImageId = i.ImageId
                    }).ToList(),
                    PropertyExtrasCheckBoxes = p.PropertyExtras.Select(pe => new ExtraCheckBoxViewModel
                    {
                        ExtraId = pe.ExtraId,
                        ExtraName = pe.ExtraName,
                        IsChecked = true
                    }).ToList(),
                    RentalsInfo = p.Rentals.Select(r => new EditRentalInfoForPropertyViewModel
                    {
                        RentalInfoId = r.RentalId,
                        AdditionalInfo = r.AdditionalInfo,
                        RentalTypeId = (int)r.UnitTypeId,

                        RentalExtrasIds = r.RentalExtras.Select(re => re.ExtraId).ToList(),
                        RentalPrices = new AddRentalPriceForPeriodViewModel
                        {
                            Price = r.RentalPrice
                        }
                    })
                    .ToList()
                })
                .FirstOrDefault();

            return propToEdit;
        }

        /// <summary>
        /// Edit full property 
        /// </summary>
        /// <param name="model"></param>
        public void EditProperty(EditPropertyViewModel model)
        {
            var propertyToEdit = UnitOfWork.PropertiesRepository.GetAll()
                .Include(p => p.Rentals)
                .FirstOrDefault(p => p.PropertyId == model.PropertyId);

            if (propertyToEdit == null)
            {
                throw new ArgumentException("Not Found");
            }

            propertyToEdit.AdditionalDescription = model.AdditionalDescription;
            propertyToEdit.PropertyName = model.PropertyName;
            propertyToEdit.PropertySeasonId = model.PropertySeasonId;

            propertyToEdit.PropertyExtras.Clear();
            //Add extras to the property
            propertyToEdit.PropertyExtras = new HashSet<PropertyExtras>(
                _propertyExtrasManager.CreatePropertyExtras(model.PropertyExtrasCheckBoxes.Where(p => p.IsChecked)
                    .Select(p => p.ExtraId).ToList()));


            //Get all rentalsInfo which are deleted from the property 
            //(the ids of the rentalInfos in the model are the new ones the rest of the are deleted)
            var rentalsInfoToRemove =
                propertyToEdit.Rentals.Where(r => model.RentalsInfo.Any(ri => ri.RentalInfoId != r.RentalId)); // test before use

            //Deleting the rentalInfos which no longer exist in the collection
            _rentalsInfoManager.DeleteRentalInfo(rentalsInfoToRemove);

            //Adding All new rentals information
            var newRentalInfo = model.RentalsInfo.Where(ri => ri.RentalInfoId == null);
            propertyToEdit.Rentals = new HashSet<RentalsInfo>(_rentalsInfoManager.AddRentalInfo(newRentalInfo.Select(ri =>
               new CreateRentalInfoViewModel()
               {
                   AdditionalInfo = ri.AdditionalInfo,
                   UnitTypeId = ri.RentalTypeId,
                   UnitsCount = ri.RoomsCount
               }).ToList()));

            //Edit the existing rentalInformation
            _rentalsInfoManager.EditRentalInfo(model.RentalsInfo.Where(ri => ri.RentalInfoId != null));

            //Mark property as modified
            UnitOfWork.PropertiesRepository.Edit(propertyToEdit);
            //Commit changes
            UnitOfWork.Save();
        }

        public string DeleteProperty(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var propertyToDelete = UnitOfWork.PropertiesRepository.FindBy(p => p.PropertyId == id && p.Owner.Id == userId).FirstOrDefault();
                UnitOfWork.PropertiesRepository.Delete(propertyToDelete);
                UnitOfWork.Save();
            }
            catch (Exception)
            {
                return "STATUS_ERR";
            }
            return "STATUS_OK";
        }

        public DetailsPropertyViewModel Details(int propertyId)
        {
            var property = UnitOfWork.PropertiesRepository
                .FindBy(p => p.PropertyId == propertyId)
                .Include(p => p.Address)
                .Include(p => p.Address.City)
                .Include(p => p.Address.City.Country)
                .Include(p => p.Address.Coordinates)
                .Include(p => p.Images)
                .Include(p => p.Rentals)
                .Include(p => p.Agent)
                .FirstOrDefault();

            if (property == null)
            {
                throw new ContentNotFoundException("Property not found");
            }

            var propertyModel = new DetailsPropertyViewModel
            {
                PropertyId = property.PropertyId,
                PropertyName = property.PropertyName,
                AdditionalDescription = property.AdditionalDescription,
                PropertyType = property.PropertyType?.PropertyTypeName,
                SellingPrice = property.SellingPrice,
                BedroomsCount =
                    property.PropertyAttributes?.Where(a => a.Key == BedroomCount).Select(a => a.Value)
                        .FirstOrDefault(), // Calculate bedrooms
                BathroomsCount =
                    property.PropertyAttributes?.Where(a => a.Key == BathroomCount).Select(a => a.Value)
                        .FirstOrDefault(), // Calculate bathrooms
                RoomCount = property.PropertyAttributes?.Where(a => a.Key == RoomCount).Select(a => a.Value).FirstOrDefault(),
                AreaInSquareFt = property.AreaInSquareFt,
                Views = property.Views,
                LikesCount = property.PropertyLikes.Count,
                Address = new DetailsAddressViewModel
                {
                    FullAddress = property.Address.City.Country.CountryNameBG + ", " + property.Address.City.CityName + ", " +
                              property.Address.FullAddress,
                    Latitude = property.Address.Coordinates?.Latitude,
                    Longitude = property.Address.Coordinates?.Longtitude
                },
                PropertySeason = property.PropertySeason?.PropertySeasonName,
                Images = property.Images?.Where(i => Math.Abs(i.ImageRatio - 1.5F) < 0.1).Select(i => i.ImagePath).ToList(),
                Attributes = property.PropertyAttributes?.Select(Mapper.Map<AttributesKeyValueViewModel>).ToList(),
                Rentals = property.Rentals?.Select(r => new RentalInfoDetails
                {
                    UnitType = Enum.GetName(typeof(UnitType), r.UnitTypeId),
                    UnitCount = r.UnitCount,
                    Price = r.RentalPrice,
                    PricePeriodType = Enum.GetName(typeof(RentalPeriod), r.RentPricePeriod),
                    AdditionalInfo = r.AdditionalInfo,
                    RentalAttributes = r.RentalAttributes?.Select(Mapper.Map<AttributesKeyValueViewModel>).ToList()
                })
                    .ToList(),
                Agent = Mapper.Map<AgentListViewModel>(property.Agent)
            };

            //**********************************************
            // map to ViewModel and return
            //**********************************************

            return propertyModel;
        }


        public List<Properties> ListProperties(bool? isForSale = null, int? cityId = null, int? distanceFromCity = null,
            int? areaInSqrtMeters = null, int? propertyTypeId = null, int? priceFrom = null, int? priceTo = null)
        {
            var properties = UnitOfWork.PropertiesRepository.GetAll();

            properties = isForSale == null ? properties : properties.Where(p => p.SellingPrice != null);
            properties = cityId == null ? properties : properties.Where(p => p.Address.CityId == cityId);

            if (distanceFromCity != null)
            {
                double kmInDegrees = 111.12 / (double)distanceFromCity;
                Cities city = UnitOfWork.CitiesRepository.GetAll().FirstOrDefault(c => c.CityId == cityId);
                var cityLatitudeUpper = double.Parse(city.Latitude) + kmInDegrees;
                var cityLatitudeLower = double.Parse(city.Latitude) - kmInDegrees;
                var cityLongitudeUpper = double.Parse(city.Longitude) + kmInDegrees;
                var cityLongitudeLower = double.Parse(city.Longitude) - kmInDegrees;

                properties = properties.Where(p =>
                p.Address.Coordinates.Latitude <= cityLatitudeUpper &&
                p.Address.Coordinates.Latitude >= cityLatitudeLower &&
                p.Address.Coordinates.Longtitude <= cityLongitudeUpper &&
                p.Address.Coordinates.Longtitude >= cityLongitudeLower);
            }

            properties = isForSale == null ? properties : properties.Where(p => p.AreaInSquareFt <= areaInSqrtMeters);
            properties = isForSale == null ? properties : properties.Where(p => p.PropertyTypeId == propertyTypeId);

            if (isForSale == true)
            {
                properties = isForSale == null ? properties : properties.Where(p => p.SellingPrice >= priceFrom && p.SellingPrice <= priceTo);
            }

            return properties.ToList();
        }

        /// <summary>
        /// Owner's properties
        /// </summary>
        /// <param name="ownerId"></param>
        public void OwnerProperties(string ownerId)
        {
            UnitOfWork.PropertiesRepository.FindBy(p => p.Owner.Id == ownerId);

            //**********************************************
            //Map to VM and return 
            //**********************************************

        }


        /// <summary>
        /// Properties filtered by City, from, to, people count
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="peopleCount"></param>
        public void ListPropertiesBy(int cityId, DateTime from, DateTime to, int peopleCount)
        {
            var properties = UnitOfWork.PropertiesRepository.FindBy(p => p.Address.CityId == cityId)
                .Join(UnitOfWork.RentalsRepository.GetAll(),
                    p => p.PropertyId,
                    r => r.PropertyId,
                    (p, r) => new { Property = p, Rentals = r }
                )
                .Where(p => !p.Rentals.Reservations.Any(r => r.From >= from && r.To <= to))
                .Select(p => p.Property);


            //***********************************
            // return Collection of the properties which has rentals with peopleCount beds in that city for the period)
            //***********************************
        }

        public void ListProperties(List<int> cities, List<int> propertyTypes, List<int> propertySeasons,
            List<int> propertyExtras)
        {
            var properties = UnitOfWork.PropertiesRepository.GetAll();
            if (cities.Any())
            {
                properties = properties.Where(p => cities.Any(c => p.Address.CityId == c));
            }
            if (propertyTypes.Any())
            {
                properties = properties.Where(p => propertyTypes.Any(pt => p.PropertyTypeId == pt));
            }
            if (propertySeasons.Any())
            {
                properties = properties.Where(p => propertySeasons.Any(ps => p.PropertySeasonId == ps));
            }
            if (propertyExtras.Any())
            {
                properties = properties.Where(p =>
                    propertyExtras.Any(pe => p.PropertyExtras.Any(ppe => ppe.ExtraId == pe)));
            }

            //**********************************************
            // return Collection of some Property View Model
            //**********************************************
        }


        public void GetMostLikedProperties(int? count = null)
        {
            var properties = UnitOfWork.PropertiesRepository.GetAll();
            if (count != null)
            {
                properties = properties.OrderBy(p => p.PropertyLikes.Count).Take((int)count);
            }
            // Return property VM
        }


        public void GetMostReviewedProperties(int? count = null)
        {
            var properties = UnitOfWork.PropertiesRepository.GetAll();
            if (count != null)
            {
                properties = properties.OrderBy(p => p.Reviews.Count).Take((int)count);
            }
            // Return Property VM
        }

        public List<Properties> GetHighestRatedProperties(int count)
        {
            var prop = UnitOfWork.PropertiesRepository.GetAll()
                .Include(p => p.Reviews)
                .OrderBy(p => p.Reviews.Average(r => r.ReviewScore))
                .Take(count)
                .Include(p => p.Address)
                .Include(p => p.Address.City)
                .Include(p => p.Address.City.Country)
                .ToList();

            //Figure out the end Use Average on ReviewsScore

            return prop;
        }

        /// <summary>
        /// Get Seasons when the property is supposed to work most( AllYear,Summer,Winter)
        /// </summary>
        /// <returns></returns>
        public List<DropDownPropertySeasonsViewModel> GetPropertySeasons()
        {
            return UnitOfWork.PropertiesRepository.ListPropertySeasons().Select(ps => new DropDownPropertySeasonsViewModel()
            {
                PropertySeasonId = ps.PropertySeasonId,
                PropertySeasonName = ps.PropertySeasonName
            }).ToList();
        }


        /// <summary>
        /// Get All Property Types (House,Motel,Hotel)
        /// </summary>
        /// <returns></returns>
        public List<DropDownPropertyTypesViewModel> GetPropertyTypes()
        {
            return UnitOfWork.PropertiesRepository.ListPropertyTypes().Select(pt => new DropDownPropertyTypesViewModel()
            {
                PropertyTypeId = pt.PropertyTypeId,
                PropertyTypeName = pt.PropertyTypeName
            }).ToList();
        }

        public List<PropertyBriefInfoViewModel> GetRelatedPropertiesBriefInfo(int? propertyType = null, int? cityId = null)
        {
            var properties = UnitOfWork.PropertiesRepository.GetAll()
                .Include(p => p.Address)
                .Include(p => p.Address.City)
                .Include(p => p.Address.City.Country)
                .Include(p => p.Images);


            properties = propertyType == null ? properties : properties.Where(p => p.PropertyTypeId == propertyType);
            properties = cityId == null ? properties : properties.Where(p => p.Address.CityId == cityId);


            return properties.Take(5)
                .AsEnumerable()
                .Select(Mapper.Map<PropertyBriefInfoViewModel>)
                .ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="priceTo"></param>
        /// <param name="propertyType">0 - Hotel / 1 - House / 2 - Motel / 3 - Station / 4 - Office Building / 5 - Villa / 6 - Parking / 7 - Garage / 8-</param>
        /// <param name="cityId">Filter by city</param>
        /// <param name="distanceFromCity"></param>
        /// <param name="pageCount">Page number</param>
        /// <param name="pageSize">Size of Page</param>
        /// <param name="sortBy">City / Area / PropertyName / Date</param>
        /// <param name="orderBy">Ascending / Descending / None</param>
        /// <param name="priceFrom"></param>
        /// <returns></returns>
        public PropertiesInfoViewModel GetPropertiesForSell(int? priceFrom = null, int? priceTo = null, int? propertyType = null, int? cityId = null, int? distanceFromCity = null, int? pageCount = 1, int? pageSize = 6,
            string sortBy = "", string orderBy = "")
        {
            var properties = GetProperties(propertyType, cityId, distanceFromCity, sortBy, orderBy);


            if (priceFrom != null && priceTo != null)
            {
                properties = properties.Where(p => p.SellingPrice >= priceFrom && p.SellingPrice <= priceTo);
            }
            else if (priceFrom != null)
            {
                properties = properties.Where(p => p.SellingPrice >= priceFrom);
            }
            else if (priceTo != null)
            {
                properties = properties.Where(p => p.SellingPrice <= priceTo);
            }

            var propertiesCount = properties.Count();

            //Page the resultset
            if (pageSize != null && pageCount != null)
            {
                properties = properties.Paging((int)pageCount, (int)pageSize);
            }

            return new PropertiesInfoViewModel
            {
                Properties = properties
                    .ToList()
                    .Select(Mapper.Map<PropertyInfoViewModel>)
                    .ToList(),
                Count = propertiesCount
            };
        }


        public PropertiesInfoViewModel GetPropertyForRent(DateTime? from = null, DateTime? to = null, int? priceFrom = null, int? priceTo = null, int? propertyType = null, int? cityId = null, int? distanceFromCity = null, int? pageCount = 1,
            int? pageSize = null,
            string sortBy = "", string orderBy = "")
        {
            var properties = GetProperties(propertyType, cityId, distanceFromCity, sortBy, orderBy);


            properties = properties.Where(p => p.RentalPrice != null || p.Rentals.Any());

            // Filter for price range
            if (priceFrom != null && priceTo != null)
            {
                //Property rental price is in range or some of property Rentals is in the price range
                Expression<Func<Properties, bool>> priceRange = p =>
                    (p.RentalPrice >= priceFrom &&
                     p.RentalPrice <= priceTo) ||
                    (p.Rentals.Any(r =>
                        r.RentalPrice >= priceFrom &&
                        r.RentalPrice <= priceTo));

                properties = properties.Where(priceRange);
            }
            else if (priceFrom != null) // For higher than
            {
                properties = properties.Where(p => p.RentalPrice >= priceFrom || p.Rentals.Any(r => r.RentalPrice >= priceFrom));
            }
            else if (priceTo != null) // For lower than
            {
                properties = properties.Where(p => p.RentalPrice <= priceTo || p.Rentals.Any(r => r.RentalPrice <= priceTo));
            }


            //This line should filter by start and end date
            if (from != null && to != null)
            {
                properties = properties
                    .Join(UnitOfWork.RentalsRepository.GetAll(),
                        p => p.PropertyId,
                        r => r.PropertyId,
                        (p, r) => new { Property = p, Rentals = r }
                    )
                    .Where(p => !p.Rentals.Reservations.Any(r => r.From >= from && r.To <= to))
                    .Select(p => p.Property);
            }

            var propertiesCount = properties.Count();

            //Page the resultset
            if (pageSize != null && pageCount != null)
            {
                properties = properties.Paging((int)pageCount, (int)pageSize);
            }

            return new PropertiesInfoViewModel
            {
                Properties = properties
                    .ToList()
                    .Select(Mapper.Map<PropertyInfoViewModel>)
                    .ToList(),
                Count = propertiesCount
            };
        }





        /// <summary>
        /// Increase the property Views by one
        /// </summary>
        /// <param name="propertyId"></param>
        public void IncrementViews(int propertyId)
        {
            var property = UnitOfWork.PropertiesRepository.FindBy(p => p.PropertyId == propertyId).FirstOrDefault() ?? throw new ContentNotFoundException("Property not found!");
            property.Views++;
            UnitOfWork.PropertiesRepository.Edit(property);
            UnitOfWork.Save();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="distanceFromCity"></param>
        /// <param name="sortBy">City / Area / PropertyName / Date</param>
        /// <param name="orderBy">Ascending / Descending / None</param>
        /// <param name="isForRent"></param>
        /// <param name="priceTo"></param>
        /// <param name="propertyType"></param>
        /// <param name="cityId"></param>
        /// <param name="priceFrom"></param>
        /// <returns></returns>
        private IQueryable<Properties> GetProperties(int? propertyType = null, int? cityId = null, int? distanceFromCity = null,
            string sortBy = "", string orderBy = "")
        {
            var properties = UnitOfWork.PropertiesRepository
                .Include(p => p.Address)
                .Include(p => p.Address.City)
                .Include(p => p.Address.City.Country)
                .Include(p => p.Images);



            if (propertyType != null)
            {
                properties = properties.Where(p => p.PropertyTypeId == propertyType);
            }
            if (cityId != null)
            {
                properties = properties.Where(p => p.Address.CityId == cityId);
            }

            if (distanceFromCity != null && cityId != null)
            {
                double kmInDegrees = (double)distanceFromCity / 111.12;
                CoordinatesViewModel city = _citiesManager.GetCoords((int)cityId);
                var cityLatitudeUpper = double.Parse(city.Latitude, new CultureInfo("en-US")) + kmInDegrees;
                var cityLatitudeLower = double.Parse(city.Latitude, new CultureInfo("en-US")) - kmInDegrees;
                var cityLongitudeUpper = double.Parse(city.Longitude, new CultureInfo("en-US")) + kmInDegrees;
                var cityLongitudeLower = double.Parse(city.Longitude, new CultureInfo("en-US")) - kmInDegrees;

                //Parse in not supported by linq to entities, also not every address has coordinates !
                properties = properties.Where(p =>
                    p.Address.Coordinates.Latitude <= cityLatitudeUpper &&
                    p.Address.Coordinates.Latitude >= cityLatitudeLower &&
                    p.Address.Coordinates.Longtitude <= cityLongitudeUpper &&
                    p.Address.Coordinates.Longtitude >= cityLongitudeLower);
            }

            if (orderBy == "Ascending")
            {
                switch (sortBy)
                {
                    case "City":
                        properties = properties.OrderBy(p => p.Address.City.CityName);
                        break;
                    case "Area":
                        properties = properties.OrderBy(p => p.AreaInSquareFt);
                        break;
                    case "PropertyName":
                        properties = properties.OrderBy(p => p.PropertyName);
                        break;
                    case "Date":
                        properties = properties.OrderBy(p => p.CreatedOn);
                        break;
                    case "Views":
                        properties = properties.OrderBy(p => p.Views);
                        break;
                    case "ReviewScore":
                        properties = properties.OrderBy(p => p.Reviews.Average(r => r.ReviewScore));
                        break;
                    default:
                        properties = properties.OrderBy(p => p.PropertyId);
                        break;
                }
            }
            else if (orderBy == "Descending")
            {
                switch (sortBy)
                {
                    case "City":
                        properties = properties.OrderByDescending(p => p.Address.City.CityName);
                        break;
                    case "Area":
                        properties = properties.OrderByDescending(p => p.AreaInSquareFt);
                        break;
                    case "PropertyName":
                        properties = properties.OrderByDescending(p => p.PropertyName);
                        break;
                    case "Date":
                        properties = properties.OrderByDescending(p => p.CreatedOn);
                        break;
                    case "Views":
                        properties = properties.OrderByDescending(p => p.Views);
                        break;
                    case "ReviewScore":
                        properties = properties.OrderByDescending(p => p.Reviews.Average(r => r.ReviewScore));
                        break;
                    default:
                        properties = properties.OrderBy(p => p.PropertyId);
                        break;
                }
            }
            else
            {
                properties = properties.OrderBy(p => p.PropertyId);
            }


            return properties;
        }

        public int GetPropertiesCount(int? propertyType = null, int? cityId = null)
        {
            var properties = UnitOfWork.PropertiesRepository.GetAll();

            if (propertyType != null)
            {
                properties = properties.Where(p => p.PropertyTypeId == propertyType);
            }
            if (cityId != null)
            {
                properties = properties.Where(p => p.Address.CityId == cityId);
            }

            return properties.Count();
        }

        public int GetAgentPropertiesCount(string userId)
        {
            var properties = UnitOfWork.PropertiesRepository.GetAll();

            if (!string.IsNullOrEmpty(userId))
            {
                properties = properties.Where(p => p.AgentId == userId);
            }

            return properties.Count();
        }

    }
}
