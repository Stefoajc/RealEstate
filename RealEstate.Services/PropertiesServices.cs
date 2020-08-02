using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using RealEstate.Model;
using RealEstate.Model.Notifications;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.Services.Helpers;
using RealEstate.Services.Interfaces;
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
        private readonly ExtraServices _extrasManager;
        private readonly AppointmentServices _appointmentsManager;
        private readonly INotificationCreator _notificationCreator;


        public PropertiesServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr
            , CityServices cityServices, ImageServices imageServices
            , AddressServices addressServices, RentalInfoServices rentalInfoServices
            , ExtraServices extraServices, AppointmentServices appointmentsManager
            , INotificationCreator notificationCreator) : base(unitOfWork, userMgr)
        {
            _imagesManager = imageServices;
            _addressManager = addressServices;
            _rentalsInfoManager = rentalInfoServices;
            _propertyExtrasManager = extraServices;
            _appointmentsManager = appointmentsManager;
            _notificationCreator = notificationCreator;
            _citiesManager = cityServices;
            _extrasManager = extraServices;
        }

        public async Task<ManageProperties> PropertiesManage(int id, string userId)
        {
            //Get aggregated properties
            var properties = await (await ListPropertiesAggregated(isForRent: true)).Properties
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    PropertyId = p.Id,
                    p.PropertyName,
                    p.Agent.AgentId,
                    p.Owner.OwnerId
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен имотът!");

            if (properties.AgentId != userId && properties.OwnerId != userId &&
                await userManager.IsInRoleAsync(userId, "Admin"))
            {
                throw new NotAuthorizedUserException("Не сте оторизиран за това действие!");
            }

            return new ManageProperties
            {
                PropertyId = properties.PropertyId,
                PropertyName = properties.PropertyName,
            };
        }

        /// <summary>
        /// Service for adding a full featured Property
        /// </summary>
        /// <param name="model">Full featured property</param>
        public async Task<int> AddProperty(CreatePropertyViewModel model, string userId)
        {
            Properties property = new Properties
            {
                IsActive = true,
                CreatedOn = DateTime.Now,
                PropertyName = model.PropertyName,
                UnitTypeId = model.PropertyTypeId,
                AdditionalDescription = model.AdditionalDescription,
                AreaInSquareMeters = model.Area,
                PropertySeasonId = model.PropertySeasonId,
                AgentId = userId,
                OwnerId = model.OwnerId,
                SellingPrice = model.SellingPrice,
                RentalPrice = model.RentalPrice,
                RentalPricePeriodId = model.RentalPricePeriodId,
                Views = 0,
                //Add Address
                Address = _addressManager.AddAddress(model.Address),
                // Rental Info Handle HERE !
                Rentals = new HashSet<RentalsInfo>(MapRentalInfo(model.RentalsInfo)),
                //Add extras to the property
                Extras =
                    new HashSet<Extras>(_propertyExtrasManager
                        .GetPropertyExtras(model.PropertyExtrasCheckBoxes
                            .Where(p => p.IsChecked)
                            .Select(p => p.ExtraId)
                            .ToList())),
                Attributes = new HashSet<KeyValuePairs>(model.Attributes
                    .Select(a => AttributesResolvers.AttributesResolver(a.Key, a.Value)))
            };
            
            //Add images to the property
            var agentName = await userManager.Users.Where(u => u.Id == userId).Select(u => u.UserName).FirstOrDefaultAsync();
            var regularImages = _imagesManager.CreatePropertyImagesSet(model.ImageFiles, agentName, false);
            var sliderImages = _imagesManager.CreatePropertyImagesSet(model.ImageFilesForSlider, agentName, true);
            property.Images = new HashSet<PropertyImages>(regularImages.Union(sliderImages));

            //Add property to the database
            unitOfWork.PropertiesBaseRepository.Add(property);
            await unitOfWork.SaveAsync();


            // this forces the Slider with properties to refresh on next query
            _isPropertiesForSliderChanged = true;
            //

            #region Notifications

            var creatorName = await unitOfWork.UsersRepository
                .Where(u => u.Id == property.AgentId)
                .Select(a => a.FirstName + " " + a.LastName)
                .FirstOrDefaultAsync();

            var notificationToCreate = new NotificationCreateViewModel
            {
                NotificationTypeId = (int)NotificationType.Property,
                NotificationPicture = property.Images.Select(i => i.ImagePath).FirstOrDefault(),
                NotificationLink = "/properties/details?id=" + property.Id + "&isrentsearching=" + (property.SellingPrice != null),
                NotificationText = creatorName + " добави имот: " + property.PropertyName
            };

            await _notificationCreator.CreateGlobalNotification(notificationToCreate, property.AgentId);

            #endregion

            return property.Id;
        }

        /// <summary>
        /// Get property for edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<EditPropertyViewModel> GetPropertyForEdit(int id)
        {
            var propToEdit = (await unitOfWork.PropertiesRepository
                .GetAll()
                .OfType<Properties>()
                .Where(p => p.Id == id)
                .Include(p => p.Rentals)
                .Include(p => p.Images)
                .Include(p => p.Extras)
                .Include(p => p.Attributes)
                .ToListAsync())
                .Select(p => new EditPropertyViewModel
                {
                    PropertyId = p.Id,
                    AdditionalDescription = p.AdditionalDescription,
                    PropertyName = p.PropertyName,
                    PropertySeasonId = p.PropertySeasonId,
                    PropertyTypeId = p.UnitTypeId,
                    Area = p.AreaInSquareMeters,
                    SellingPrice = p.SellingPrice,
                    RentalPrice = p.RentalPrice,
                    RentalPeriodId = p.RentalPricePeriodId,
                    OwnerId = p.OwnerId,
                    Address = new CreateAddressViewModel
                    {
                        CityId = p.Address.CityId,
                        FullAddress = p.Address.FullAddress,
                        Latitude = p.Address.Coordinates.Latitude,
                        Longitude = p.Address.Coordinates.Longtitude
                    },
                    Images = p.Images.Select(i => new ImageEditViewModel
                    {
                        ImagePath = i.ImagePath,
                        ImageId = i.ImageId,
                        IsForSlider = i.ImageRatio > 2.0f
                    }).ToList(),
                    PropertyExtrasCheckBoxes = p.Extras.Select(pe => new ExtraCheckBoxViewModel
                    {
                        ExtraId = pe.ExtraId,
                        ExtraName = pe.ExtraName,
                        IsChecked = true
                    }).ToList(),
                    Attributes = p.Attributes.Select(a => new AttributesKeyValueViewModel
                    {
                        Key = a.Key,
                        Value = a.Value
                    }).ToList()
                })
                .FirstOrDefault() ?? throw new ContentNotFoundException("Не е намерен имота!");

            var propertyExtras = _propertyExtrasManager.GetExtras("PropertyExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList();
            foreach (var extra in propertyExtras)
            {
                if (propToEdit.PropertyExtrasCheckBoxes.Any(pr => pr.ExtraId == extra.ExtraId))
                {
                    extra.IsChecked = true;
                }
            }

            propToEdit.PropertyExtrasCheckBoxes = propertyExtras;

            return propToEdit;
        }

        /// <summary>
        /// Edit full property 
        /// </summary>
        /// <param name="model"></param>
        public async Task EditProperty(EditPropertyViewModel model)
        {
            var propertyToEdit = unitOfWork.PropertiesBaseRepository
                .GetAll().OfType<Properties>()
                .Include(p => p.Rentals)
                .FirstOrDefault(p => p.Id == model.PropertyId) ?? throw new ContentNotFoundException();

            propertyToEdit.SellingPrice = model.SellingPrice;
            propertyToEdit.RentalPrice = model.RentalPrice;
            propertyToEdit.RentalPricePeriodId = model.RentalPeriodId;
            propertyToEdit.AreaInSquareMeters = model.Area;
            propertyToEdit.AdditionalDescription = model.AdditionalDescription;
            propertyToEdit.PropertyName = model.PropertyName;
            propertyToEdit.PropertySeasonId = model.PropertySeasonId;
            propertyToEdit.UnitTypeId = model.PropertyTypeId;
            propertyToEdit.OwnerId = model.OwnerId;

            propertyToEdit.Extras.Clear();
            //Add extras to the property
            propertyToEdit.Extras = new HashSet<Extras>(
                _propertyExtrasManager.GetPropertyExtras(model.PropertyExtrasCheckBoxes.Where(p => p.IsChecked)
                    .Select(p => p.ExtraId).ToList()));

            //Edit Address
            var newAddress = _addressManager.AddAddress(model.Address);
            propertyToEdit.Address.CityId = newAddress.CityId;
            propertyToEdit.Address.FullAddress = newAddress.FullAddress;
            propertyToEdit.Address.Coordinates.Latitude = newAddress.Coordinates.Latitude;
            propertyToEdit.Address.Coordinates.Longtitude = newAddress.Coordinates.Longtitude;


            //Mark property as modified
            unitOfWork.PropertiesBaseRepository.Edit(propertyToEdit);
            //Commit changes
            await unitOfWork.SaveAsync();
        }


        /// <summary>
        /// Change property state to 0-Available/1-Sold/2-Rented
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task ChangePropertyStatus(int propertyId, string status)
        {
            var result = Enum.TryParse(status, out PropertyState propertyState);
            if (!result)
            {
                throw new ArgumentException();
            }

            var property = await unitOfWork.PropertiesBaseRepository
                .Where(p => p.Id == propertyId)
                .FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            property.PropertyState = propertyState;
            unitOfWork.PropertiesBaseRepository.Edit(property);
            await unitOfWork.SaveAsync();
        }


        /// <summary>
        /// Remove property From DataBase
        /// </summary>
        /// <param name="id">Id of property</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task DeleteProperty(int id, string userId)
        {
            if (await unitOfWork.PropertiesRepository.GetAll().AnyAsync(p => p.Id == id))
            {
                await Delete(id, userId);
            }
            else if (await unitOfWork.RentalsRepository.GetAll().AnyAsync(p => p.Id == id))
            {
                await _rentalsInfoManager.Delete(id, userId);
            }
            else
            {
                throw new ContentNotFoundException("Не е намерен имотът, който искате да изтриете!");
            }

            // this forces the Slider with properties to refresh on next query
            _isPropertiesForSliderChanged = true;
            //
        }

        private async Task Delete(int id, string userId)
        {
            var propertyToDelete = await unitOfWork.PropertiesRepository
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен имотът, който искате да изтриете!");

            if (propertyToDelete.OwnerId != userId && propertyToDelete.AgentId != userId
                && !await userManager.IsInRoleAsync(userId, Enum.GetName(typeof(Role), Role.Administrator))
                && !await userManager.IsInRoleAsync(userId, Enum.GetName(typeof(Role), Role.Maintenance)))
            {
                throw new NotAuthorizedException("Нямате права за изтриване на имота!");
            }

            propertyToDelete.Extras.Clear();
            propertyToDelete.Attributes.Clear();

            //Remove rentals from the property
            foreach (var rentals in propertyToDelete.Rentals)
            {
                await _rentalsInfoManager.Delete(rentals.Id, userId);
            }

            unitOfWork.PropertiesRepository.Delete(propertyToDelete);
            await unitOfWork.SaveAsync();

        }

        /// <summary>
        /// Get Full Details about Property
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public async Task<DetailsPropertyViewModel> Details(int propertyId)
        {
            DetailsPropertyViewModel propertyModel;

            if (await unitOfWork.PropertiesRepository.GetAll()
                .AnyAsync(p => p.Id == propertyId))
            {
                var property = unitOfWork.PropertiesRepository
                    .Where(p => p.Id == propertyId)
                    .Include(p => p.Address)
                    .Include(p => p.Address.City)
                    .Include(p => p.Address.City.Country)
                    .Include(p => p.Address.Coordinates)
                    .Include(p => p.Attributes)
                    .Include(p => p.RentalHirePeriodType)
                    .Include(p => p.Extras)
                    .Include(p => p.Images)
                    .Include(p => p.Rentals)
                    .Include(p => p.Rentals.Select(r => r.Extras))
                    .Include(p => p.Rentals.Select(r => r.Attributes))
                    .Include(p => p.Rentals.Select(r => r.RentalHirePeriodType))
                    .Include(p => p.Agent)
                    .Include(p => p.Owner)
                    .Include(p => p.Reviews)
                    .Include(p => p.Reviews.Select(r => r.User))
                    .Include(p => p.Reviews.Select(r => r.User.Images))
                    .Include(p => p.Rentals.Select(r => r.RentalHirePeriodType))
                    .FirstOrDefault();

                propertyModel = Mapper.Map<DetailsPropertyViewModel>(property);
            }
            else if (await unitOfWork.RentalsRepository.GetAll()
                .AnyAsync(p => p.Id == propertyId))
            {
                propertyModel = await _rentalsInfoManager.GetRentalDetails(propertyId);
            }
            else
            {
                throw new ContentNotFoundException("Имотът не е намерен");
            }

            return propertyModel;
        }


        public void GetMostLikedProperties(int? count = null)
        {
            var properties = unitOfWork.PropertiesRepository.GetAll();
            if (count != null)
            {
                properties = properties.OrderBy(p => p.PropertyLikes.Count).Take((int)count);
            }
            // Return property VM
        }

        public void GetMostReviewedProperties(int? count = null)
        {
            var properties = unitOfWork.PropertiesRepository.GetAll();
            if (count != null)
            {
                properties = properties.OrderBy(p => p.Reviews.Count).Take((int)count);
            }
            // Return Property VM
        }

        private static bool _isPropertiesForSliderChanged = true;
        private static List<PropertySliderViewModel> _propertiesForSliderCache;
        public async Task<List<PropertySliderViewModel>> GetPropertiesForMainSlider(int propertiesCount)
        {
            if (_isPropertiesForSliderChanged || _propertiesForSliderCache == null)
            {
                var properties = (await (await ListPropertiesAggregated(orderBy: "Descending", sortBy: "Date"))
                .Properties
                .Select(p => new
                {
                    p.Id,
                    p.AdditionalDescription,
                    p.Address,
                    p.IsRental,
                    p.SellingPrice,
                    p.RentalPrice,
                    p.RentalHirePeriodType,
                    p.PropertyState
                })
                .ToListAsync());

                var propertiesForSlider = properties
                    .Select(p => new PropertySliderViewModel
                    {
                        PropertyId = p.Id,
                        AdditionalDescription = p.AdditionalDescription,
                        PropertyState = (int)p.PropertyState,
                        FullAddress = p.Address.City.Country.CountryNameBG + " " + p.Address.City.CityName + " " + p.Address.FullAddress,
                        PriceDescription = p.IsRental || p.SellingPrice == null ?
                        ((p.RentalPrice ?? 0M).ToString("0")) + "лв. " + p.RentalHirePeriodType.PeriodName.ToLower() :
                        ((p.SellingPrice ?? 0M).ToString("0") + "лв.")
                    })
                    .ToList();

                var propertiesIds = propertiesForSlider.Select(r => r.PropertyId).ToList();
                var imagePaths = await unitOfWork.ImagesRepository.GetAll().OfType<PropertyImages>()
                    .Where(i => propertiesIds.Any(p => p == i.PropertyId) && i.ImageRatio > 2.0F)
                    .GroupBy(i => new { i.PropertyId, i.ImagePath })
                    .Select(i => new { i.Key.PropertyId, i.Key.ImagePath })
                    .ToListAsync();

                foreach (var property in propertiesForSlider)
                {
                    property.ImagePath = imagePaths
                        .Where(i => i.PropertyId == property.PropertyId)
                        .Select(i => i.ImagePath)
                        .FirstOrDefault();

                    var imagePhysicalPath =
                        Path.Combine(System.Web.HttpRuntime.AppDomainAppPath.TrimEnd('\\', '/'), property.ImagePath.TrimStart('\\', '/'));

                    if (!File.Exists(imagePhysicalPath))
                    {
                        property.ImagePath = FileHelpers.AppendToFileName(property.ImagePath, "_Slider");
                    }
                }

                propertiesForSlider = propertiesForSlider.Where(p => !string.IsNullOrEmpty(p.ImagePath)).Take(propertiesCount).ToList();

                _propertiesForSliderCache = propertiesForSlider;
                _isPropertiesForSliderChanged = false;
            }

            return _propertiesForSliderCache;
        }

        public async Task<List<PropertyInfoDTO>> GetHighestRatedProperties(int count)
        {
            var prop = (await ListPropertiesByStatusAsPropertyInfo())
                .OrderBy(p => p.ReviewsAverage)
                .Take(count)
                .ToList();

            //Figure out the end Use Average on ReviewsScore

            return prop;
        }

        public async Task<PropertiesInfoViewModel> GetUserProperties(string userId)
        {
            var user = await userManager.FindByIdAsync(userId) ?? throw new ContentNotFoundException("Потребителят не е намерен!");

            var properties = await ListPropertiesAggregated();

            if (await userManager.IsInRoleAsync(userId, AgentRole))
            {
                properties.Properties = properties.Properties.Where(p => p.Agent.AgentId == userId);
            }
            else if (await userManager.IsInRoleAsync(userId, OwnerRole))
            {
                properties.Properties = properties.Properties.Where(p => p.Owner.OwnerId == userId);
            }
            else
            {
                throw new ContentNotFoundException("Влезте като собственик или агент!");
            }

            return new PropertiesInfoViewModel
            {
                Properties = await properties.Properties
                    .Select(property => new PropertyInfoViewModel
                    {
                        PropertyId = property.Id,
                        PropertyName = property.PropertyName,
                        PropertyState = (int)property.PropertyState,
                        PropertyType = property.UnitType.PropertyTypeName,
                        Status = (property.IsRental || property.SellingPrice == null) && property.RentalHirePeriodType != null ? property.RentalHirePeriodType.PeriodName : "Продажна",
                        FullAddress = property.Address.City.Country.CountryNameBG + ", " + property.Address.City.CityName + " " + property.Address.FullAddress,
                        Price = property.IsRental || property.SellingPrice == null ? property.RentalPrice ?? 0.0M : property.SellingPrice ?? 0.0M,
                        ImagePath = property.ImagePath,
                        Info = property.AdditionalDescription,
                        ReviewsAverage = property.ReviewsAverage,
                        IsRentalProperty = property.IsRental,
                        IsPartlyRented = property.IsPartlyRented,
                        IsRentable = property.RentalPrice != null && !property.IsRental, // property has rental price and is not rental
                        CreatedOn = property.CreatedOn,
                        CityName = property.Address.City.CityName,
                        Views = property.Views,
                        AreaInSquareMeters = property.AreaInSquareMeters ?? 0,
                        BottomRight = property.UnitType.PropertyTypeName,
                        BottomLeft = property.AreaInSquareMeters != null ? "<li><strong>Площ: </strong>" + property.AreaInSquareMeters + "<sup>m2</sup></li>" : ""
                    })
                    .ToListAsync(),
                Count = properties.Count
            };
        }

        public async Task<List<PropertyMapViewModel>> GetPropertiesForMap(int? propertyType = null, int? cityId = null, int? distanceFromCity = null, string agentId = null, bool? isForRent = null, bool? isForShortPeriod = null)
        {
            IQueryable<PropertyInfoDTO> properties =
               (await ListPropertiesAggregated(isForRent: isForRent, isShortPeriodRent: isForShortPeriod,
                    cityId: cityId, distanceFromCity: distanceFromCity,
                    propertyTypeId: propertyType))
                .Properties;

            properties = properties.Where(p => p.Address.Coordinates != null);

            var resultTest = await properties
                .Select(property => new
                {
                    PropertyId = property.Id,
                    PropertyName = property.PropertyName,
                    FullAddress = property.Address.City.Country.CountryNameBG + ", " + property.Address.City.CityName + " " + property.Address.FullAddress,
                    Price = isForRent == true || property.IsRental ? property.RentalPrice ?? 0 : property.SellingPrice ?? 0,
                    Latitude = property.Address.Coordinates.Latitude,
                    Longitude = property.Address.Coordinates.Longtitude,
                    PropertyType = property.UnitType.PropertyTypeName,
                    HasRentals = property.IsPartlyRented,
                    Period = (isForRent == true || property.IsRental) && property.RentalHirePeriodType  != null ? property.RentalHirePeriodType.PeriodName : "Продажна",
                    ParentPropertyId = property.ParentPropertyId
                })
                .ToListAsync();

            var propertiesIds = resultTest.Select(r => r.PropertyId).ToList();
            var imagePaths = await unitOfWork.ImagesRepository.GetAll().OfType<PropertyImages>()
                .Where(i => propertiesIds.Any(p => p == i.PropertyId)).Select(i => new { i.PropertyId, i.ImagePath }).ToListAsync();

            var resultForMap = resultTest.Select(property => new PropertyMapViewModel
            {
                PropertyId = property.PropertyId,
                PropertyName = property.PropertyName,
                FullAddress = property.FullAddress,
                Price = property.Price,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                PropertyType = property.PropertyType,
                HasRentals = property.HasRentals,
                Period = property.Period,
                ImagePaths = imagePaths
                    .Where(i => i.PropertyId == property.PropertyId || i.PropertyId == property.ParentPropertyId)
                    .Select(i => i.ImagePath).ToList()
            });

            var result = properties.Select(p => new PropertyMapViewModel
            {
                PropertyId = p.Id,
                PropertyName = p.PropertyName,
                FullAddress = p.Address.City.Country.CountryNameBG + ", " + p.Address.City.CityName + " " + p.Address.FullAddress,
                Price = isForRent == null || isForRent == false ? p.SellingPrice ?? 0 : p.RentalPrice ?? 0,
                Latitude = p.Address.Coordinates.Latitude,
                Longitude = p.Address.Coordinates.Longtitude,
                PropertyType = p.UnitType.PropertyTypeName,
                HasRentals = p.IsPartlyRented,
                Period = p.RentalHirePeriodType != null ? p.RentalHirePeriodType.PeriodName : null
            });

            return resultForMap.ToList();
        }

        public async Task<List<PropertyBriefInfoViewModel>> GetRelatedPropertiesBriefInfo(bool? isForRent = null, int? propertyType = null, int? cityId = null)
        {
            var properties = (await ListPropertiesAggregated(isForRent: isForRent,
                    propertyTypeId: propertyType,
                    cityId: cityId))
                .Properties.Where(p => p.ImagePath != null);


            return properties.Take(5)
                .Select(p => new PropertyBriefInfoViewModel
                {
                    PropertyId = p.Id,
                    PropertyName = p.PropertyName,
                    PropertyType = p.UnitType.PropertyTypeName,
                    ImagePath = p.ImagePath,
                    PropertyState = (int)p.PropertyState,
                    FullAddress = p.Address.City.CityName + " " + p.Address.FullAddress
                })
                .ToList();
        }

        public async Task<List<PropertiesRelatedViewModel>> GetRelatedProperties(int relatedToPropertyId, bool isForRent = false)
        {
            var propertyRelatedTo = await unitOfWork.PropertiesBaseRepository
                .Where(p => p.Id == relatedToPropertyId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException();

            int? cityId = propertyRelatedTo is Properties ? ((Properties)propertyRelatedTo).Address.CityId : ((RentalsInfo)propertyRelatedTo).Property.Address.CityId;
            int? areaFrom = propertyRelatedTo.AreaInSquareMeters - 50;
            int? areaTo = propertyRelatedTo.AreaInSquareMeters + 50;
            int? propertyTypeId = propertyRelatedTo.UnitTypeId;


            var properties = (await ListPropertiesByStatusAsPropertyInfo(
                    isForRent,
                    cityId: cityId,
                    areaFrom: areaFrom, areaTo: areaTo,
                    propertyTypeId: propertyTypeId))
                .Where(p => p.Id != relatedToPropertyId)
                .Take(6);

            return await properties
                .Select(property => new PropertiesRelatedViewModel
                {
                    PropertyId = property.Id,
                    PropertyName = property.PropertyName,
                    Status = isForRent == false || property.RentalHirePeriodType == null ? "Продажна" : property.RentalHirePeriodType.PeriodName,
                    FullAddress = property.Address.City.Country.CountryNameBG + ", " + property.Address.City.CityName + " " + property.Address.FullAddress,
                    Price = isForRent == false ? property.SellingPrice ?? 0.0M : property.RentalPrice ?? 0.0M,
                    ImagePath = property.ImagePath,
                    BottomRight = property.UnitType.PropertyTypeName,
                    BottomLeft = property.AreaInSquareMeters != null ? "<li><strong>Площ: </strong>" + property.AreaInSquareMeters + "<sup>m2</sup></li>" : ""
                })
                .ToListAsync();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="priceTo"></param>
        /// <param name="propertyType">0 - Hotel / 1 - House / 2 - Motel / 3 - Station / 4 - Office Building / 5 - Villa / 6 - Parking / 7 - Garage / 8-</param>
        /// <param name="cityId">Filter by city</param>
        /// <param name="distanceFromCity"></param>
        /// <param name="areaFrom"></param>
        /// <param name="areaTo"></param>
        /// <param name="extras">Extras of the property</param>
        /// <param name="pageCount">Page number</param>
        /// <param name="pageSize">Size of Page</param>
        /// <param name="sortBy">City / Area / PropertyName / Date</param>
        /// <param name="orderBy">Ascending / Descending / None</param>
        /// <param name="isShortPeriodRent"></param>
        /// <param name="priceFrom"></param>
        /// <param name="isForRent"></param>
        /// <param name="agentId"></param>
        /// <param name="from">Filters if isForRent is true</param>
        /// <param name="to">Filters if isForRent is true</param>
        /// <returns></returns>
        public async Task<PropertiesInfoViewModel> GetProperties(
            bool? isForRent = null, bool? isShortPeriodRent = null,
            string agentId = null,
            DateTime? from = null, DateTime? to = null,
            int? priceFrom = null, int? priceTo = null,
            int? propertyType = null,
            int? areaFrom = null, int? areaTo = null,
            int? cityId = null, int? distanceFromCity = null,
            List<int> extras = null,
            int? pageCount = 1, int? pageSize = null,
            string sortBy = "", string orderBy = "")
        {
            return await ListProperties(
                isForRent: isForRent, isShortPeriodRent: isShortPeriodRent,
                agentId: agentId,
                cityId: cityId, distanceFromCity: distanceFromCity,
                areaFrom: areaFrom, areaTo: areaTo,
                propertyTypeId: propertyType,
                priceFrom: priceFrom, priceTo: priceTo, // depending on the diapason this leads to Rental/Sell search 
                from: from, to: to, // if those are set then we search for dayly rented properties                 
                extras: extras,
                pageCount: pageCount, pageSize: pageSize,
                sortBy: sortBy, orderBy: orderBy);
        }

        public async Task<PropertiesInfoViewModel> GetAgentProperties(string agentId, bool? isForRent = null, int? page = 1, int? pageSize = 6)
        {
            return await ListProperties(isForRent, agentId: agentId, pageCount: page, pageSize: pageSize);
        }

        public async Task<PropertiesHomePageAggregatedViewModel> GetPropertiesForHomePage()
        {
            var propertiesForSell = await ListProperties(isForRent:false);
            var propertiesForRent = await ListProperties(isForRent: true);

            return new PropertiesHomePageAggregatedViewModel
            {
                PropertiesForSell = propertiesForSell.Properties.Take(6).ToList(),
                HousesForSell = propertiesForSell.Properties.Where(p => p.PropertyType == "Къща").Take(6).ToList(),
                OfficesForSell = propertiesForSell.Properties.Where(p => p.PropertyType == "Офис").Take(6).ToList(),
                ApartmentsForSell = propertiesForSell.Properties.Where(p => p.PropertyType == "Апартамент").Take(6).ToList(),
                PropertiesForRent = propertiesForRent.Properties.Take(6).ToList(),
                HousesForRent = propertiesForRent.Properties.Where(p => p.PropertyType == "Къща").Take(6).ToList(),
                OfficesForRent = propertiesForRent.Properties.Where(p => p.PropertyType == "Офис").Take(6).ToList(),
                ApartmentsForRent = propertiesForRent.Properties.Where(p => p.PropertyType == "Апартамент").Take(6).ToList()
            };
        }

        public async Task<PropertiesInfoViewModel> GetPropertiesByType(bool isForRent = false, string propertyType = null)
        {
            var propertyTypeId = (await unitOfWork.PropertiesRepository.ListPropertyTypesAsync())
                .FirstOrDefault(p => p.PropertyTypeName == propertyType)
                ?.PropertyTypeId;

            var properties = await ListProperties(isForRent, propertyTypeId: propertyTypeId);

            return properties;
        }

        public async Task<int> GetPropertiesCount(int? propertyType = null, int? cityId = null)
        {
            var properties = unitOfWork.PropertiesRepository.GetAll();

            if (propertyType != null)
            {
                properties = properties.Where(p => p.UnitTypeId == propertyType);
            }
            if (cityId != null)
            {
                properties = properties.Where(p => p.Address.CityId == cityId);
            }

            return await properties.CountAsync();
        }

        public async Task<int> GetAgentPropertiesCount(string userId)
        {
            var properties = unitOfWork.PropertiesRepository.GetAll();

            if (!string.IsNullOrEmpty(userId))
            {
                properties = properties.Where(p => p.AgentId == userId);
            }

            return await properties.CountAsync();
        }

        /// <summary>
        /// For Agent details when client try to make appointment
        /// </summary>
        public async Task<List<PropertyDropDownViewModel>> GetPropertiesForDropDown(string agentId)
        {
            var properties = await ListPropertiesAggregated(agentId: agentId);


            return await properties.Properties.Select(p => new PropertyDropDownViewModel
            {
                PropertyId = p.Id,
                PropertyName = p.PropertyName + "(" + p.UnitType.PropertyTypeName + "-" + (p.IsRental ? "наемане" : "продаване") + ")" + " - " + p.Address.City.Country.CountryNameBG + ", " + p.Address.City.CityName + " " + p.Address.FullAddress,
            }).ToListAsync();
        }

        #region PropertyTypes

        private static List<DropDownPropertyTypesViewModel> _propertyTypesForDropDown;

        /// <summary>
        /// Get All Property Types (House,Motel,Hotel) // Cached in _propertyTypesForDropDown
        /// </summary>
        /// <returns></returns>
        public async Task<List<DropDownPropertyTypesViewModel>> GetPropertyTypes()
        {
            return _propertyTypesForDropDown ?? (_propertyTypesForDropDown =
                       (await unitOfWork.PropertiesRepository.ListPropertyTypesAsync())
                       .Select(pt => new DropDownPropertyTypesViewModel
                       {
                           PropertyTypeId = pt.PropertyTypeId,
                           PropertyTypeName = pt.PropertyTypeName
                       }).ToList());
        }


        #region PropertyTypes for layout


        private static IDictionary<int, string> _propertyTypes;

        private static IDictionary<int, string> PropertyTypes
        {
            get
            {
                if (_propertyTypes == null)
                {
                    var unitOfWork = (IUnitOfWork)DependencyResolver.Current.GetService(typeof(IUnitOfWork));

                    _propertyTypes = unitOfWork.PropertiesRepository.ListPropertyTypes()
                        .Select(pt => new { pt.PropertyTypeId, pt.PropertyTypeName })
                        .ToDictionary(pt => pt.PropertyTypeId, pt => pt.PropertyTypeName);
                }

                return _propertyTypes;
            }
        }

        public static string GetPropertyType(int propertyTypeId)
        {
            return PropertyTypes[propertyTypeId];
        }

        public static int GetPropertyTypeId(string propertyType)
        {
            return PropertyTypes
                .Where(pt => pt.Value.ToUpper() == propertyType.ToUpper())
                .Select(pt => pt.Key)
                .FirstOrDefault();
        }

        #endregion


        #endregion

        #region Image Helpers

        public async Task<List<ImageViewModels>> AddImages(ImageCreateViewModel images, string userId, bool isForSlider = false)
        {
            int propertyIdToAddImageTo = Int32.Parse(images.ForeignKey);
            var property = await unitOfWork.PropertiesRepository
                .Where(p => p.Id == propertyIdToAddImageTo)
                .FirstOrDefaultAsync();

            if (property == null)
            {
                throw new ContentNotFoundException("Не е намерен имота на който искате да добавите изображение!");
            }

            string currentlyLoggedUserId = userId;
            if (property.AgentId != currentlyLoggedUserId && property.OwnerId != currentlyLoggedUserId 
                && !await userManager.IsInRoleAsync(currentlyLoggedUserId, Enum.GetName(typeof(Role), Role.Administrator))
                && !await userManager.IsInRoleAsync(currentlyLoggedUserId, Enum.GetName(typeof(Role), Role.Maintenance)))
            {
                throw new NotAuthorizedUserException("Нямате права да добавяте изображение за този имот");
            }

            int propertyId = int.Parse(images.ForeignKey);
            var callerName = await unitOfWork.PropertiesRepository
                .Where(p => p.Id == propertyId).Select(p => p.Owner.UserName)
                .FirstOrDefaultAsync();

            //If property not found
            if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("Не е намерен имотът!");

            List<ImageFileSystemDTO> imagesForFileSystem = new List<ImageFileSystemDTO>();
            //Used for result generating
            List<Images> resultCollection = new List<Images>();

            foreach (var imageToAdd in images.ImageFiles)
            {
                Images imagePreDbAdd = new PropertyImages
                {
                    ImageType = imageToAdd.ContentType,
                    ImagePath = PathManager.CreateUserPropertyImagePath(callerName, imageToAdd.FileName, isForSlider:isForSlider),
                    PropertyId = int.Parse(images.ForeignKey),
                    ImageRatio = isForSlider ? (float)2200 / 800 : (float)870 / 580
                };

                unitOfWork.ImagesRepository.Add(imagePreDbAdd);

                resultCollection.Add(imagePreDbAdd);

                imagesForFileSystem.Add(new ImageFileSystemDTO
                {
                    ImageRelPath = imagePreDbAdd.ImagePath,
                    ImageFile = imageToAdd
                });
            }
            await unitOfWork.SaveAsync();

            _imagesManager.SaveToFileSystem(imagesForFileSystem, isForSlider);


            return resultCollection
                .Select(i => new ImageViewModels
                {
                    ImageId = i.ImageId,
                    ImagePath = i.ImagePath
                }).ToList();
        }

        public async Task RemoveImage(int id, string userId)
        {
            var image = await unitOfWork.ImagesRepository.GetAll()
                .Where(i => i.ImageId == id)
                .OfType<PropertyImages>()
                .Include(i => i.Property)
                .FirstOrDefaultAsync();

            if (image == null)
            {
                throw new ContentNotFoundException("Не е намерено изображението, което искате да изтриите!");
            }

            if (image.Property.AgentId != userId && image.Property.OwnerId != userId
                && !await userManager.IsInRoleAsync(userId, Enum.GetName(typeof(Role), Role.Administrator))
                && !await userManager.IsInRoleAsync(userId, Enum.GetName(typeof(Role), Role.Maintenance)))
            {
                throw new NotAuthorizedUserException("Нямате права да добавяте изображение за този имот");
            }

            await _imagesManager.DeleteImage(id, userId);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get Seasons when the property is supposed to work most( AllYear,Summer,Winter)
        /// </summary>
        /// <returns></returns>
        public async Task<List<DropDownPropertySeasonsViewModel>> GetPropertySeasons()
        {
            return (await unitOfWork.PropertiesRepository.ListPropertySeasons())
                .Select(ps => new DropDownPropertySeasonsViewModel()
                {
                    PropertySeasonId = ps.PropertySeasonId,
                    PropertySeasonName = ps.PropertySeasonName
                }).ToList();
        }

        /// <summary>
        /// Increase the property Views by one
        /// </summary>
        /// <param name="propertyId"></param>
        public async Task IncrementViews(int propertyId)
        {
            var property = await unitOfWork.PropertiesBaseRepository
                .Where(p => p.Id == propertyId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Property not found!");

            property.Views++;
            unitOfWork.PropertiesBaseRepository.Edit(property);
            await unitOfWork.SaveAsync();
        }


        private ISet<RentalsInfo> MapRentalInfo(List<AddRentalInfoToPropertyViewModel> rentalInfoCreateModels)
        {
            ISet<RentalsInfo> rentals = new HashSet<RentalsInfo>();
            foreach (var rentalInfoCreateModel in rentalInfoCreateModels)
            {
                var areaInSquareFt = rentalInfoCreateModel.Attributes
                    .Where(a => a.Key == AttributesResolvers.AreaInSquareMetersAttribute).Select(a => a.Value)
                    .FirstOrDefault();

                int? areaFinal = int.TryParse(areaInSquareFt, out var tempArea) ? tempArea : (int?)null;

                var rentalInfoToAdd = new RentalsInfo
                {
                    CreatedOn = DateTime.Now,
                    UnitTypeId = rentalInfoCreateModel.UnitTypeId,
                    AdditionalDescription = rentalInfoCreateModel.AdditionalInfo,
                    UnitCount = rentalInfoCreateModel.UnitsCount,
                    RentalPrice = rentalInfoCreateModel.RentalPrice,
                    RentalPricePeriodId = rentalInfoCreateModel.RentalPricePeriodId,
                    AreaInSquareMeters = areaFinal,
                    Attributes = new HashSet<KeyValuePairs>(rentalInfoCreateModel.Attributes.Select(a =>
                        new KeyValuePairs
                        {
                            Key = a.Key,
                            Value = a.Value
                        }).ToList()),

                    Extras = new HashSet<Extras>(_extrasManager.GetRentalExtras(rentalInfoCreateModel
                        .RentalExtras.Where(e => e.IsChecked).Select(e => e.ExtraId).ToList()))
                };

                rentals.Add(rentalInfoToAdd);
            }

            return rentals;
        }

        #endregion

        #region PropertyReviews

        public async Task AddPropertyReview(PropertyReviewCreateViewModel model, string userId)
        {
            var property = await unitOfWork.PropertiesBaseRepository
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == model.PropertyId) ?? throw new ContentNotFoundException("Имотът не е намерен!");

            if (!await userManager.IsInRoleAsync(userId, ClientRole))
            {
                throw new NotAuthorizedException("Само клиентите могат да дават оценка!");
            }

            if (!await _appointmentsManager.HasPassedApprovedAppointmentForProperty(userId, model.PropertyId))
            {
                throw new NotAuthorizedException("Само клиентите който имат записана среща, която е минала имат правото да дават оценка!");
            }

            var review = property.Reviews.FirstOrDefault(r => r.UserId == userId && r.PropertyId == model.PropertyId);
            //If the user hasnt made review Add new review
            if (review == null)
            {
                property.Reviews.Add(new PropertyReviews
                {
                    PropertyId = model.PropertyId,
                    ReviewScore = model.Score,
                    ReviewText = model.ReviewText,
                    UserId = userId
                });
            }
            //if the user has made review for this property edit it
            else
            {
                review.ReviewScore = model.Score;
                review.ReviewText = model.ReviewText;
            }


            unitOfWork.PropertiesBaseRepository.Edit(property);
            await unitOfWork.SaveAsync();
        }

        #endregion



        #region Filtering Helpers

        //IsForRent=null {All properties plus dublicates of these with rental and sell price}
        //IsForRent=true {All properties with rentPrice and Rentals}
        //IsForRent=false {Only properties with sell price}
        private async Task<PropertiesInfoViewModel> ListProperties(
            bool? isForRent = null, bool? isShortPeriodRent = null,
            DateTime? from = null, DateTime? to = null,
            string agentId = null,
            int? cityId = null, int? distanceFromCity = null,
            int? areaFrom = null, int? areaTo = null,
            int? propertyTypeId = null,
            int? priceFrom = null, int? priceTo = null,
            List<int> extras = null,
            int? pageCount = 1, int? pageSize = null,
            string sortBy = "", string orderBy = "")
        {
            var properties = await ListPropertiesAggregated(
                isForRent: isForRent, isShortPeriodRent: isShortPeriodRent,
                agentId: agentId, ownerId: null,
                cityId: cityId, distanceFromCity: distanceFromCity,
                areaFrom: areaFrom, areaTo: areaTo,
                propertyTypeId: propertyTypeId,
                priceFrom: priceFrom, priceTo: priceTo,
                from: from, to: to,
                extras: extras,
                pageCount: pageCount, pageSize: pageSize,
                sortBy: sortBy, orderBy: orderBy);

            return new PropertiesInfoViewModel
            {
                Properties = await properties.Properties
                .Select(property => new PropertyInfoViewModel
                {
                    PropertyId = property.Id,
                    PropertyName = property.PropertyName,
                    PropertyState = (int)property.PropertyState,
                    PropertyType = property.UnitType.PropertyTypeName,
                    Status = isForRent == true || property.IsRental || property.SellingPrice == null && property.RentalHirePeriodType != null ? property.RentalHirePeriodType.PeriodName : "Продажна",
                    FullAddress = property.Address.City.Country.CountryNameBG + ", " + property.Address.City.CityName + " " + property.Address.FullAddress,
                    Price = isForRent == true || property.IsRental || property.SellingPrice == null ? property.RentalPrice ?? 0.0M : property.SellingPrice ?? 0.0M,
                    ImagePath = property.ImagePath,
                    Info = property.AdditionalDescription,
                    ReviewsAverage = property.ReviewsAverage,
                    IsRentalProperty = property.IsRental,
                    IsPartlyRented = property.IsPartlyRented,
                    IsRentable = property.RentalPrice != null && !property.IsRental, // property has rental price and is not rental
                    IsGreen = property.IsGreen,
                    CreatedOn = property.CreatedOn,
                    CityName = property.Address.City.CityName,
                    Views = property.Views,
                    AreaInSquareMeters = property.AreaInSquareMeters ?? 0,
                    BottomRight = property.UnitType.PropertyTypeName,
                    BottomLeft = property.AreaInSquareMeters != null ? "<li><strong>Площ: </strong>" + property.AreaInSquareMeters + "<sup>m2</sup></li>" : ""
                })
                .ToListAsync(),
                Count = properties.Count
            };

        }


        private async Task<PropertiesInfoDTO> ListPropertiesAggregated(
            bool? isForRent = null, bool? isShortPeriodRent = null,
            string agentId = null, string ownerId = null,
            int? cityId = null, int? distanceFromCity = null,
            int? areaFrom = null, int? areaTo = null,
            int? propertyTypeId = null,
            int? priceFrom = null, int? priceTo = null,
            DateTime? from = null, DateTime? to = null,
            List<int> extras = null,
            int? pageCount = 1, int? pageSize = null,
            string sortBy = "", string orderBy = "")
        {
            IQueryable<PropertyInfoDTO> properties;

            //if isShortPeriodRent is set to something isForRent is set to true
            isForRent = isShortPeriodRent != null ? true : isForRent;

            //Aggregate all available properties rentals / sell
            //only rentals if rentalPricePeriodId != null
            if (isForRent == null)
            {
                //Add Properties which have rentalPrice
                properties = await ListPropertiesByStatusAsPropertyInfo(
                        isForRent: true, isShortPeriodRent: isShortPeriodRent,
                        agentId: agentId, ownerId: ownerId,
                        cityId: cityId, distanceFromCity: distanceFromCity,
                        areaFrom: areaFrom, areaTo: areaTo,
                        propertyTypeId: propertyTypeId,
                        priceFrom: priceFrom, priceTo: priceTo,
                        from: from, to: to,
                        extras: extras);

                //Add RentalInfoes
                properties = properties.Union(
                    await _rentalsInfoManager.GetRentalsAsPropertyInfoDTO(
                        isShortPeriodRent: isShortPeriodRent,
                        agentId: agentId, ownerId: ownerId,
                        to: to, from: from,
                        priceFrom: priceFrom, priceTo: priceTo,
                        propertyTypeId: propertyTypeId,
                        areaFrom: areaFrom, areaTo: areaTo,
                        extras: extras,
                        cityId: cityId, distanceFromCity: distanceFromCity)
                );


                if (isShortPeriodRent == null)
                {
                    //Only evaluated when isRental and rentalPricePeriodId are null
                    properties = properties.Union(
                        await ListPropertiesByStatusAsPropertyInfo(
                            isForRent: false, isShortPeriodRent: null,
                            agentId: agentId, ownerId: ownerId,
                            cityId: cityId, distanceFromCity: distanceFromCity,
                            areaFrom: areaFrom, areaTo: areaTo,
                            propertyTypeId: propertyTypeId,
                            priceFrom: priceFrom, priceTo: priceTo,
                            from: from, to: to,
                            extras: extras));
                }

            }
            else
            {
                properties = await ListPropertiesByStatusAsPropertyInfo(
                    isForRent: isForRent, isShortPeriodRent: isShortPeriodRent,
                    agentId: agentId, ownerId: ownerId,
                    cityId: cityId, distanceFromCity: distanceFromCity,
                    areaFrom: areaFrom, areaTo: areaTo,
                    propertyTypeId: propertyTypeId,
                    priceFrom: priceFrom, priceTo: priceTo,
                    from: from, to: to,
                    extras: extras);

                if (isForRent == true)
                {
                    var rentals = await _rentalsInfoManager.GetRentalsAsPropertyInfoDTO(
                        isShortPeriodRent: isShortPeriodRent,
                        agentId: agentId, ownerId: ownerId,
                        to: to, from: from,
                        priceFrom: priceFrom, priceTo: priceTo,
                        propertyTypeId: propertyTypeId,
                        areaFrom: areaFrom, areaTo: areaTo,
                        extras: extras,
                        cityId: cityId, distanceFromCity: distanceFromCity);

                    properties = properties.Union(rentals);
                }
            }

            var propertiesCount = await properties.CountAsync();
            properties = OrderPropertyInfo(properties, isForRent ?? false, sortBy, orderBy);
            //Page the resultset
            if (pageSize != null && pageCount != null)
            {
                properties = properties.Paging((int)pageCount, (int)pageSize);
            }


            return new PropertiesInfoDTO
            {
                Properties = properties,
                Count = propertiesCount
            };
        }


        //FINAL
        private async Task<IQueryable<PropertyInfoDTO>> ListPropertiesByStatusAsPropertyInfo(
            bool? isForRent = false, bool? isShortPeriodRent = null,
            string agentId = null, string ownerId = null,
            int? cityId = null, int? distanceFromCity = null,
            int? areaFrom = null, int? areaTo = null,
            int? propertyTypeId = null,
            int? priceFrom = null, int? priceTo = null,
            DateTime? from = null, DateTime? to = null,
            List<int> extras = null)
        {
            var properties = await ListPropertiesByStatus(
                isForRent: isForRent, isShorPeriodRent: isShortPeriodRent,
                agentId: agentId, ownerId: ownerId,
                cityId: cityId, distanceFromCity: distanceFromCity,
                areaFrom: areaFrom, areaTo: areaTo,
                propertyTypeId: propertyTypeId,
                priceFrom: priceFrom, priceTo: priceTo,
                from: from, to: to,
                extras: extras);

            return properties
                .Select(property => new PropertyInfoDTO
                {
                    Id = property.Id,
                    PropertyName = property.PropertyName,
                    SellingPrice = property.SellingPrice,
                    RentalPrice = property.RentalPrice,
                    AreaInSquareMeters = property.AreaInSquareMeters,
                    AdditionalDescription = property.AdditionalDescription,
                    CreatedOn = property.CreatedOn,
                    Views = property.Views,
                    IsActive = property.IsActive,
                    PropertyState = property.PropertyState,
                    IsRental = false,
                    PropertyLikesCount = property.PropertyLikes.Count,
                    ImagePath = property.Images.Select(i => i.ImagePath).FirstOrDefault(),
                    ReviewsAverage = property.Reviews.Average(p => p.ReviewScore),
                    IsPartlyRented = property.Rentals.Any(),
                    ParentPropertyId = null,
                    IsGreen = property.Extras.Any(e => e.ExtraName.Contains("зелено")),

                    RentalHirePeriodType = property.RentalPricePeriodId != null ? new RentalHirePeriodTypesInfoDTO
                    {
                        Id = property.RentalHirePeriodType.Id,
                        PeriodName = property.RentalHirePeriodType.PeriodName,
                        IsTimePeriodSearchable = property.RentalHirePeriodType.IsTimePeriodSearchable,
                    } : null,
                    UnitType = new PropertyTypeInfoDTO
                    {
                        PropertyTypeId = property.UnitType.PropertyTypeId,
                        PropertyTypeName = property.UnitType.PropertyTypeName
                    },
                    Address = new AddressInfoDTO
                    {
                        FullAddress = property.Address.FullAddress,
                        Coordinates = new CoordinatesInfoDTO
                        {
                            Latitude = property.Address.Coordinates.Latitude,
                            Longtitude = property.Address.Coordinates.Longtitude
                        },
                        City = new CityInfoDTO
                        {
                            CityId = property.Address.City.CityId,
                            CityName = property.Address.City.CityName,
                            PostalCode = property.Address.City.PostalCode,
                            PhoneCode = property.Address.City.PhoneCode,
                            CityCode = property.Address.City.CityCode,
                            Latitude = property.Address.City.Latitude,
                            Longitude = property.Address.City.Longitude,
                            Country = new CountryInfoDTO
                            {
                                CountryNameBG = property.Address.City.Country.CountryNameBG,
                                CountryPhoneCode = property.Address.City.Country.CountryPhoneCode
                            }
                        }
                    },
                    PropertySeason = new PropertySeasonInfoDTO
                    {
                        PropertySeasonId = property.PropertySeason.PropertySeasonId,
                        PropertySeasonName = property.PropertySeason.PropertySeasonName
                    },
                    Owner = new OwnerUserInfoDTO
                    {
                        OwnerId = property.Owner.Id,
                        FullName = property.Owner.FirstName + " " + property.Owner.LastName,
                        Email = property.Owner.Email,
                        ImagePath = property.Owner.Images.Select(i => i.ImagePath).FirstOrDefault(),
                    },
                    Agent = new AgentUsersInfoDTO
                    {
                        AgentId = property.Agent.Id,
                        FullName = property.Agent.FirstName + " " + property.Agent.LastName,
                        AdditionalDescription = property.Agent.AdditionalDescription,
                        PhoneNumber = property.Agent.PhoneNumber,
                        Email = property.Agent.Email,
                        ImagePath = property.Agent.Images.Select(i => i.ImagePath).FirstOrDefault()
                    }
                });
        }

        //FINAL
        //Rental/Sell specific filtering done here!
        //All filtering done till here
        private async Task<IQueryable<Properties>> ListPropertiesByStatus(
            bool? isForRent = null, bool? isShorPeriodRent = null,
            string agentId = null, string ownerId = null,
            int? cityId = null, int? distanceFromCity = null,
            int? areaFrom = null, int? areaTo = null,
            int? propertyTypeId = null,
            int? priceFrom = null, int? priceTo = null,
            DateTime? from = null, DateTime? to = null,
            List<int> extras = null,
            string sortBy = "", string orderBy = "")
        {
            var properties = await GetProperties(propertyType: propertyTypeId,
                cityId: cityId, distanceFromCity: distanceFromCity,
                sortBy: sortBy, orderBy: orderBy,
                agentId: agentId, ownerId: ownerId,
                areaFrom: areaFrom, areaTo: areaTo,
                extras: extras);

            if (isForRent == true)
            {
                properties = RentFilters(properties, from, to, priceFrom, priceTo, isShorPeriodRent);
            }
            if (isForRent == false)
            {
                properties = SellFilters(properties, priceFrom, priceTo);
            }

            return properties;
        }

        private IQueryable<PropertyInfoDTO> OrderPropertyInfo(IQueryable<PropertyInfoDTO> properties, bool isForRental = false, string sortBy = "", string orderBy = "")
        {
            //Ordering Asc/Desc/Null
            if (orderBy == "Ascending")
            {
                switch (sortBy)
                {
                    case "City":
                        properties = properties.OrderBy(p => p.Address.City.CityName);
                        break;
                    case "Area":
                        properties = properties.OrderBy(p => p.AreaInSquareMeters);
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
                        properties = properties.OrderBy(p => p.ReviewsAverage);
                        break;
                    case "Price":
                        properties = isForRental ? properties.OrderBy(p => p.RentalPrice) : properties.OrderBy(p => p.SellingPrice);
                        break;
                    default:
                        properties = properties.OrderBy(p => p.Id);
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
                        properties = properties.OrderByDescending(p => p.AreaInSquareMeters);
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
                        properties = properties.OrderByDescending(p => p.ReviewsAverage);
                        break;
                    case "Price":
                        properties = isForRental ? properties.OrderByDescending(p => p.RentalPrice) : properties.OrderByDescending(p => p.SellingPrice);
                        break;
                    default:
                        properties = properties.OrderByDescending(p => p.Id);
                        break;
                }
            }
            else
            {
                properties = properties.OrderBy(p => p.Id);
            }

            return properties;
        }

        private IQueryable<Properties> SellFilters(IQueryable<Properties> properties, int? priceFrom = null, int? priceTo = null)
        {
            properties = properties.Where(p => p.SellingPrice != null);

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

            return properties;
        }

        private IQueryable<Properties> RentFilters(IQueryable<Properties> properties,
            DateTime? from = null, DateTime? to = null,
            int? priceFrom = null, int? priceTo = null,
            bool? isShortPeriodRent = null)
        {
            properties = properties.Where(p => p.RentalPrice != null);

            // Filter for price range
            properties = priceFrom == null ? properties : properties.Where(p => p.RentalPrice > priceFrom);
            properties = priceTo == null ? properties : properties.Where(p => p.RentalPrice < priceTo);


            //Only one day stays rentals are filtered
            properties = isShortPeriodRent == null
                ? properties
                : properties.Where(p => p.RentalHirePeriodType.IsTimePeriodSearchable == isShortPeriodRent);

            //This line should filter by start and end date
            if (from != null && to != null)
            {
                properties = properties.Where(p => p.RentalHirePeriodType.IsTimePeriodSearchable);

                properties = properties
                    .Join(unitOfWork.RentalsRepository.GetAll(),
                        p => p.Id,
                        r => r.PropertyId,
                        (p, r) => new { Property = p, Rentals = r }
                    )
                    .Where(p => !p.Rentals.Reservations.Any(r => r.From >= from && r.To <= to))
                    .Select(p => p.Property);
            }

            return properties;
        }

        /// <summary>
        /// Base filtering functionality
        /// </summary>
        /// <param name="distanceFromCity"></param>
        /// <param name="sortBy">City / Area / PropertyName / Date</param>
        /// <param name="orderBy">Ascending / Descending / None</param>
        /// <param name="propertyType"></param>
        /// <param name="cityId"></param>
        /// <param name="agentId"></param>
        /// <param name="ownerId"></param>
        /// <param name="areaFrom"></param>
        /// <param name="areaTo"></param>
        /// <param name="isActive"></param>
        /// <param name="extras"></param>
        /// <returns></returns>
        private async Task<IQueryable<Properties>> GetProperties(int? propertyType = null, int? cityId = null, int? distanceFromCity = null,
            string sortBy = "", string orderBy = "",
            string agentId = null, string ownerId = null,
            int? areaFrom = null, int? areaTo = null,
            List<int> extras = null , bool? isActive = true)
        {
            var properties = unitOfWork.PropertiesRepository
                .Include(p => p.Attributes,
                    p => p.Extras,
                    p => p.Rentals,
                    p => p.RentalHirePeriodType,
                    p => p.UnitType,
                    p => p.Reservations,
                    p => p.Reviews,
                    p => p.Images,
                    p => p.Reviews,
                    p => p.Address,
                    p => p.Address.Coordinates,
                    p => p.Address.City,
                    p => p.Address.City.Country);

            properties = isActive == null ? properties : properties.Where(p => p.IsActive == isActive);

            properties = propertyType == null ? properties : properties.Where(p => p.UnitType.PropertyTypeId == propertyType);

            properties = cityId == null ? properties : properties.Where(p => p.Address.City.CityId == cityId);

            properties = extras == null
                ? properties
                : properties.Where(p => p.Extras.Any(e => extras.Any(ex => ex == e.ExtraId)));

            if (distanceFromCity != null && cityId != null)
            {
                double kmInDegrees = (double)distanceFromCity / 111.12;
                CoordinatesViewModel city = await _citiesManager.GetCoords((int)cityId);
                var cityLatitudeUpper = city.Latitude + kmInDegrees;
                var cityLatitudeLower = city.Latitude - kmInDegrees;
                var cityLongitudeUpper = city.Longitude + kmInDegrees;
                var cityLongitudeLower = city.Longitude - kmInDegrees;

                //Parse is not supported by linq to entities, also not every address has coordinates !
                properties = properties.Where(p =>
                    p.Address.Coordinates.Latitude <= cityLatitudeUpper &&
                    p.Address.Coordinates.Latitude >= cityLatitudeLower &&
                    p.Address.Coordinates.Longtitude <= cityLongitudeUpper &&
                    p.Address.Coordinates.Longtitude >= cityLongitudeLower);
            }

            //Filter by area
            properties = areaFrom == null ? properties : properties.Where(p => p.AreaInSquareMeters > areaFrom);
            properties = areaTo == null ? properties : properties.Where(p => p.AreaInSquareMeters < areaTo);

            //Ordering Asc/Desc/Null
            if (orderBy == "Ascending")
            {
                switch (sortBy)
                {
                    case "City":
                        properties = properties.OrderBy(p => p.Address.City.CityName);
                        break;
                    case "Area":
                        properties = properties.OrderBy(p => p.AreaInSquareMeters);
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
                    case "Price":
                        properties = properties.OrderBy(p => p.SellingPrice);
                        break;
                    default:
                        properties = properties.OrderBy(p => p.Id);
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
                        properties = properties.OrderByDescending(p => p.AreaInSquareMeters);
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
                    case "Price":
                        properties = properties.OrderByDescending(p => p.SellingPrice);
                        break;
                    default:
                        properties = properties.OrderBy(p => p.Id);
                        break;
                }
            }

            //Filtering on agent
            properties = string.IsNullOrEmpty(agentId) ? properties : properties.Where(p => p.AgentId == agentId);
            //Filtering on owner
            properties = string.IsNullOrEmpty(ownerId) ? properties : properties.Where(p => p.OwnerId == ownerId);

            return properties;
        }

        #endregion
    }
}
