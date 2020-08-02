using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Model;
using RealEstate.Model.Notifications;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.Services.Helpers;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class RentalInfoServices : BaseService
    {
        private readonly ExtraServices _extrasManager;
        private readonly CityServices _citiesManager;
        private readonly NotificationServices _notificationCreator;

        public RentalInfoServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr, ExtraServices extrasManager
            , CityServices cityServices, NotificationServices notificationCreator) : base(unitOfWork, userMgr)
        {
            _extrasManager = extrasManager;
            _citiesManager = cityServices;
            _notificationCreator = notificationCreator;
        }

        public async Task<List<PropertyInfoViewModel>> GetRentalInfoesForList(int propertyId)
        {
            var rentals = await unitOfWork.RentalsRepository
                .Include(p => p.Property.Images, p => p.Property, p => p.Property.Address, p => p.Property.Address.City,
                    p => p.Property.Address.City.Country)
                .Where(p => p.PropertyId == propertyId)
                .ToListAsync();

            if (!rentals.Any())
            {
                throw new ContentNotFoundException("Имотът не е намерен");
            }

            return rentals.Select(Mapper.Map<PropertyInfoViewModel>).ToList();
        }

        private async Task<IQueryable<RentalsInfo>> GetRentals(bool? isShortPeriodRent,
            string agentId = null, string ownerId = null,
            DateTime? from = null, DateTime? to = null,
            int? priceFrom = null, int? priceTo = null,
            int? propertyTypeId = null,
            int? areaFrom = null, int? areaTo = null,
            int? cityId = null, int? distanceFromCity = null,
            List<int> extras = null,
            string sortBy = "", string orderBy = "")
        {
            var rentals = unitOfWork.RentalsRepository
               .Include(p => p.Attributes,
                   p => p.Extras,
                   p => p.Property,
                   p => p.Reservations,
                   p => p.Reviews,
                   p => p.UnitType,
                   p => p.Property.Images,
                   p => p.Property.Reviews,
                   p => p.Property.Address,
                   p => p.Property.Address.Coordinates,
                   p => p.Property.Address.City,
                   p => p.Property.Address.City.Country);

            rentals = string.IsNullOrEmpty(ownerId) ? rentals : rentals.Where(p => p.Property.OwnerId == ownerId);
            rentals = string.IsNullOrEmpty(agentId) ? rentals : rentals.Where(p => p.Property.AgentId == agentId);
            rentals = cityId == null ? rentals : rentals.Where(p => p.Property.Address.CityId == cityId);
            rentals = propertyTypeId == null ?  rentals : rentals.Where(r => r.UnitTypeId == propertyTypeId);
            rentals = extras == null 
                ? rentals 
                : rentals.Where(p => p.Extras.Any(e => extras.Any(ex => ex == e.ExtraId)));

            if (distanceFromCity != null && cityId != null)
            {
                double kmInDegrees = (double)distanceFromCity / 111.12;
                CoordinatesViewModel city = await _citiesManager.GetCoords((int)cityId);
                var cityLatitudeUpper = city.Latitude + kmInDegrees;
                var cityLatitudeLower = city.Latitude - kmInDegrees;
                var cityLongitudeUpper = city.Longitude + kmInDegrees;
                var cityLongitudeLower = city.Longitude - kmInDegrees;

                //Parse is not supported by linq to entities, also not every address has coordinates !
                rentals = rentals.Where(p =>
                    p.Property.Address.Coordinates.Latitude <= cityLatitudeUpper &&
                    p.Property.Address.Coordinates.Latitude >= cityLatitudeLower &&
                    p.Property.Address.Coordinates.Longtitude <= cityLongitudeUpper &&
                    p.Property.Address.Coordinates.Longtitude >= cityLongitudeLower);
            }

            //filter by price
            rentals = priceFrom != null ? rentals.Where(r => r.RentalPrice >= priceFrom) : rentals;
            rentals = priceTo != null ? rentals.Where(r => r.RentalPrice <= priceTo) : rentals;


            //This line should filter by start and end date
            if (from != null && to != null)
            {
                rentals = rentals
                    .Where(r => !r.Reservations.Any(res => res.From >= from && res.To <= to));
            }

            //Filter by area
            rentals = areaFrom == null ? rentals : rentals.Where(r => r.AreaInSquareMeters > areaFrom);
            rentals = areaTo == null ? rentals : rentals.Where(r => r.AreaInSquareMeters < areaTo);

            //Filter by rentPeriod
            rentals = isShortPeriodRent == null ? rentals : rentals.Where(r => r.RentalHirePeriodType.IsTimePeriodSearchable == isShortPeriodRent);

            //Ordering Asc/Desc/Null
            if (orderBy == "Ascending")
            {
                switch (sortBy)
                {
                    case "City":
                        rentals = rentals.OrderBy(p => p.Property.Address.City.CityName);
                        break;
                    case "Area":
                        rentals = rentals.OrderBy(p => p.AreaInSquareMeters);
                        break;
                    case "PropertyName":
                        rentals = rentals.OrderBy(p => p.Property.PropertyName);
                        break;
                    case "Date":
                        rentals = rentals.OrderBy(p => p.Property.CreatedOn);
                        break;
                    case "Views":
                        rentals = rentals.OrderBy(p => p.Property.Views);
                        break;
                    case "ReviewScore":
                        rentals = rentals.OrderBy(p => p.Property.Reviews.Average(r => r.ReviewScore));
                        break;
                    default:
                        rentals = rentals.OrderBy(p => p.Id);
                        break;
                }
            }
            else if (orderBy == "Descending")
            {
                switch (sortBy)
                {
                    case "City":
                        rentals = rentals.OrderByDescending(p => p.Property.Address.City.CityName);
                        break;
                    case "Area":
                        rentals = rentals.OrderByDescending(p => p.AreaInSquareMeters);
                        break;
                    case "PropertyName":
                        rentals = rentals.OrderByDescending(p => p.Property.PropertyName);
                        break;
                    case "Date":
                        rentals = rentals.OrderByDescending(p => p.Property.CreatedOn);
                        break;
                    case "Views":
                        rentals = rentals.OrderByDescending(p => p.Property.Views);
                        break;
                    case "ReviewScore":
                        rentals = rentals.OrderByDescending(p => p.Property.Reviews.Average(r => r.ReviewScore));
                        break;
                    default:
                        rentals = rentals.OrderBy(p => p.PropertyId);
                        break;
                }
            }
            else
            {
                rentals = rentals.OrderBy(p => p.Id);
            }

            return rentals;
        }


        public async Task<IQueryable<PropertyInfoDTO>> GetRentalsAsPropertyInfoDTO(bool? isShortPeriodRent = null,
            string agentId = null, string ownerId = null,
            DateTime? from = null, DateTime? to = null,
            int? priceFrom = null, int? priceTo = null,
            int? propertyTypeId = null,
            int? areaFrom = null, int? areaTo = null,
            List<int> extras = null,
            int? cityId = null, int? distanceFromCity = null)
        {
            var rentals = await GetRentals(isShortPeriodRent,
                agentId, ownerId,
                to, from, priceFrom, priceTo,
                propertyTypeId, areaFrom, areaTo,
                cityId, distanceFromCity,
                extras);

            return rentals.Select(property => new PropertyInfoDTO
            {
                Id = property.Id,
                PropertyName = property.Property.PropertyName,
                SellingPrice = null,
                RentalPrice = property.RentalPrice,
                AreaInSquareMeters = property.AreaInSquareMeters,
                AdditionalDescription = property.AdditionalDescription,
                CreatedOn = property.CreatedOn,
                Views = property.Views,
                IsActive = property.Property.IsActive,
                PropertyState = property.PropertyState,
                IsRental = true,
                PropertyLikesCount = property.PropertyLikes.Count,
                ImagePath = property.Property.Images.Select(i => i.ImagePath).FirstOrDefault(),
                ReviewsAverage = property.Reviews.Average(r => r.ReviewScore),
                IsPartlyRented = false,
                ParentPropertyId = property.PropertyId,
                IsGreen = property.Extras.Any(/*e => e.ExtraName.Contains("зелено")*/),
                RentalHirePeriodType = new RentalHirePeriodTypesInfoDTO
                {
                    Id = property.RentalHirePeriodType.Id,
                    PeriodName = property.RentalHirePeriodType.PeriodName,
                    IsTimePeriodSearchable = property.RentalHirePeriodType.IsTimePeriodSearchable,
                },
                UnitType = new PropertyTypeInfoDTO
                {
                    PropertyTypeId = property.UnitType.PropertyTypeId,
                    PropertyTypeName = property.UnitType.PropertyTypeName
                },
                Address = new AddressInfoDTO
                {
                    FullAddress = property.Property.Address.FullAddress,
                    Coordinates = new CoordinatesInfoDTO
                    {
                        Latitude = property.Property.Address.Coordinates.Latitude,
                        Longtitude = property.Property.Address.Coordinates.Longtitude
                    },
                    City = new CityInfoDTO
                    {
                        CityId = property.Property.Address.City.CityId,
                        CityName = property.Property.Address.City.CityName,
                        PostalCode = property.Property.Address.City.PostalCode,
                        PhoneCode = property.Property.Address.City.PhoneCode,
                        CityCode = property.Property.Address.City.CityCode,
                        Latitude = property.Property.Address.City.Latitude,
                        Longitude = property.Property.Address.City.Longitude,
                        Country = new CountryInfoDTO
                        {
                            CountryNameBG = property.Property.Address.City.Country.CountryNameBG,
                            CountryPhoneCode = property.Property.Address.City.Country.CountryPhoneCode
                        }
                    }
                },
                PropertySeason = new PropertySeasonInfoDTO
                {
                    PropertySeasonId = property.Property.PropertySeason.PropertySeasonId,
                    PropertySeasonName = property.Property.PropertySeason.PropertySeasonName
                },
                Owner = new OwnerUserInfoDTO
                {
                    OwnerId = property.Property.Owner.Id,
                    FullName = property.Property.Owner.FirstName + " " + property.Property.Owner.LastName,
                    Email = property.Property.Owner.Email,
                    ImagePath = property.Property.Owner.Images.Select(i => i.ImagePath).FirstOrDefault(),
                },
                Agent = new AgentUsersInfoDTO
                {
                    AgentId = property.Property.Agent.Id,
                    FullName = property.Property.Agent.FirstName + " " + property.Property.Agent.LastName,
                    AdditionalDescription = property.Property.Agent.AdditionalDescription,
                    PhoneNumber = property.Property.Agent.PhoneNumber,
                    Email = property.Property.Agent.Email,
                    ImagePath = property.Property.Agent.Images.Select(i => i.ImagePath).FirstOrDefault(),
                },
                ////TODO: MUST BE REMOVED AND QUERIED SEPARATELY
                //Reservations = property.Reservations
                //    .Select(r => new ReservationInfoDTO
                //    {
                //        PropertyId = r.PropertyId,
                //        CreatedOn = r.CreatedOn,
                //        To = r.To,
                //        From = r.From,
                //        ReservationId = r.ReservationId,
                //        PaymentStatus = r.PaymentStatus,
                //        ApproveStatus = r.ApproveStatus,
                //        ClientUserId = r.ClientUserId,
                //        CaparoPrice = r.CaparoPrice,
                //        FullPrice = r.FullPrice
                //    }).ToList()
            });
        }

        //Work well
        public async Task<EditRentalInfoForPropertyViewModel> CreateRentalInfo(CreateRentalInfoViewModel rentalInfoToAddInProperty, string loggedUserId)
        {
            if (string.IsNullOrEmpty(loggedUserId)) throw new ArgumentNullException(nameof(loggedUserId));

            var property = await unitOfWork.PropertiesRepository
                .Include(p => p.Images)
                .Where(p => p.Id == rentalInfoToAddInProperty.PropertyId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Имотът не е намерен!");

            if (property.OwnerId != loggedUserId && property.AgentId != loggedUserId && !await userManager.IsInRoleAsync(loggedUserId, "Administrator"))
            {
                throw new NotAuthorizedException("Нямате право да създавате части в този имот!");
            }

            var areaInSquareFt = rentalInfoToAddInProperty.Attributes
                .Where(a => a.Key == AttributesResolvers.AreaInSquareMetersAttribute)
                .Select(a => a.Value)
                .FirstOrDefault();

            int? areaFinal = int.TryParse(areaInSquareFt, out var tempArea) ? tempArea : (int?)null;

            RentalsInfo rental = new RentalsInfo
            {
                PropertyId = rentalInfoToAddInProperty.PropertyId,
                RentalPrice = rentalInfoToAddInProperty.RentalPrice,
                RentalPricePeriodId = rentalInfoToAddInProperty.RentalPricePeriodId,
                UnitCount = rentalInfoToAddInProperty.UnitsCount,
                UnitTypeId = rentalInfoToAddInProperty.UnitTypeId,
                AdditionalDescription = rentalInfoToAddInProperty.AdditionalInfo,
                AreaInSquareMeters = areaFinal,
                Attributes = new HashSet<KeyValuePairs>(rentalInfoToAddInProperty.Attributes
                .Select(a => AttributesResolvers.AttributesResolver(a.Key, a.Value))
                .ToList()),
                Extras = new HashSet<Extras>(_extrasManager.GetRentalExtras(rentalInfoToAddInProperty
                    .RentalExtras
                    .Where(e => e.IsChecked)
                    .Select(e => e.ExtraId)
                    .ToList()))
            };

            unitOfWork.RentalsRepository.Add(rental);
            await unitOfWork.SaveAsync();

            #region Notifications

            var creatorName = await unitOfWork.UsersRepository
                .Where(u => u.Id == property.AgentId)
                .Select(a => a.FirstName + " " + a.LastName)
                .FirstOrDefaultAsync();

            var notificationToCreate = new NotificationCreateViewModel
            {
                NotificationTypeId = (int)NotificationType.Property,
                NotificationPicture = property.Images.Select(i => i.ImagePath).FirstOrDefault(),
                NotificationLink = "/properties/details?id=" + rental.Id + "&isrentsearching=True" ,
                NotificationText = creatorName + " добави имот: " + property.PropertyName
            };

            await _notificationCreator.CreateGlobalNotification(notificationToCreate, property.AgentId);

            #endregion

            return new EditRentalInfoForPropertyViewModel
            {
                UnitCount = rental.UnitCount,
                UnitTypeId = rental.UnitTypeId,
                // ReSharper disable once PossibleInvalidOperationException
                // Rental Create ViewModel guarantee for value here
                RentalPrice = (decimal)rental.RentalPrice,
                RentalInfoId = rental.Id,
                // ReSharper disable once PossibleInvalidOperationException
                RentalPricePeriodId = (int)rental.RentalPricePeriodId,
                AdditionalInfo = rental.AdditionalDescription,
                Attributes = rental.Attributes.Select(ra => new AttributesKeyValueViewModel
                {
                    Key = ra.Key,
                    Value = ra.Value
                }).ToList(),
                RentalExtras = rental.Extras.Select(re => new ExtraCheckBoxViewModel
                {
                    ExtraId = re.ExtraId,
                    ExtraName = re.ExtraName,
                    IsChecked = true
                }).ToList()
            };
        }

        //Work Well
        public async Task EditRentalInfo(EditRentalInfoForPropertyViewModel rentalInfoEditModel, string loggedUserId)
        {
            if (loggedUserId == null) throw new ArgumentNullException(nameof(loggedUserId));

            var rentalInfoToEdit = unitOfWork.RentalsRepository
                .Where(r => r.Id == rentalInfoEditModel.RentalInfoId)
                .FirstOrDefault() ?? throw new ContentNotFoundException("Не е намерен имотът!");


            if (rentalInfoToEdit.Property.OwnerId != loggedUserId && rentalInfoToEdit.Property.AgentId != loggedUserId
                 && !await userManager.IsInRoleAsync(loggedUserId, "Administrator"))
            {
                throw new NotAuthorizedException("Нямате право да редактирате този имот!");
            }

            rentalInfoToEdit.UnitTypeId = rentalInfoEditModel.UnitTypeId;
            rentalInfoToEdit.AdditionalDescription = rentalInfoEditModel.AdditionalInfo;
            rentalInfoToEdit.RentalPrice = rentalInfoEditModel.RentalPrice;
            rentalInfoToEdit.UnitCount = rentalInfoEditModel.UnitCount;
            rentalInfoToEdit.RentalPricePeriodId = rentalInfoEditModel.RentalPricePeriodId;

            //Add New Extras
            rentalInfoToEdit.Extras.Clear();
            var extrasToBeAdded = _extrasManager.GetRentalExtras(rentalInfoEditModel.RentalExtras
                .Where(r => r.IsChecked)
                .Select(r => r.ExtraId)
                .ToList());
            rentalInfoToEdit.Extras = new HashSet<Extras>(extrasToBeAdded);

            rentalInfoToEdit.Attributes.Clear();
            rentalInfoToEdit.Attributes = new HashSet<KeyValuePairs>(rentalInfoEditModel.Attributes
                .Select(a => new KeyValuePairs
                {
                    Key = a.Key,
                    Value = a.Value
                })
                .ToList());

            unitOfWork.RentalsRepository.Edit(rentalInfoToEdit);
            unitOfWork.Save();

        }

        //Work Well
        public async Task Delete(int rentalInfoId, string loggedUserId)
        {
            var rentalInfo = await unitOfWork.RentalsRepository
                .Include(r => r.Property, r => r.Attributes)
                .FirstOrDefaultAsync(r => r.Id == rentalInfoId) ?? throw new ContentNotFoundException("Имотът не е намерен!");

            if (rentalInfo.Property.OwnerId != loggedUserId
                && rentalInfo.Property.AgentId != loggedUserId
                && !await userManager.IsInRoleAsync(loggedUserId, "Administrator"))
            {
                throw new NotAuthorizedException("Нямате право да изтриете този имот!");
            }

            //Delete all rental attributes because attributes are part of triangular relationship between properties and rentalInfoes
            rentalInfo.Attributes.Clear();
            unitOfWork.RentalsRepository.Edit(rentalInfo);
            await unitOfWork.SaveAsync();


            unitOfWork.RentalsRepository.Delete(rentalInfo);
            await unitOfWork.SaveAsync();
        }


        public async Task<EditRentalInfoViewModel> GetRentalForEdit(int rentalId)
        {
            var rental = await unitOfWork.RentalsRepository
                .Include(r => r.Attributes, r => r.Extras, r => r.Property)
                .Where(r => r.Id == rentalId)
                .Select(r => new EditRentalInfoViewModel
                {
                    PropertyId = r.PropertyId,
                    PropertyName = r.Property.PropertyName,
                    UnitCount = r.UnitCount,
                    UnitTypeId = r.UnitTypeId,
                    RentalPrice = (decimal)r.RentalPrice,
                    RentalInfoId = r.Id,
                    RentalPricePeriodId = (int)r.RentalPricePeriodId,
                    AdditionalInfo = r.AdditionalDescription,
                    Attributes = r.Attributes.Select(ra => new AttributesKeyValueViewModel
                    {
                        Key = ra.Key,
                        Value = ra.Value
                    }).ToList(),
                    RentalExtras = r.Extras.Select(re => new ExtraCheckBoxViewModel
                    {
                        ExtraId = re.ExtraId,
                        ExtraName = re.ExtraName,
                        IsChecked = true
                    }).ToList()
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен имотът!");

            return rental;
        }

        public List<EditRentalInfoForPropertyViewModel> GetRentalInfoes(int propertyId)
        {
            if (!unitOfWork.PropertiesBaseRepository.GetAll()
                .OfType<Properties>()
                .Any(p => p.Id == propertyId))
            {
                throw new ContentNotFoundException("Не е намерен имотът!");
            }

            var rentals = unitOfWork.RentalsRepository
                .Include(r => r.Attributes, r => r.Extras)
                .Where(r => r.PropertyId == propertyId)
                .Select(r => new EditRentalInfoForPropertyViewModel
                {
                    UnitCount = r.UnitCount,
                    UnitTypeId = r.UnitTypeId,
                    RentalPrice = (decimal)r.RentalPrice,
                    RentalInfoId = r.Id,
                    RentalPricePeriodId = (int)r.RentalPricePeriodId,
                    AdditionalInfo = r.AdditionalDescription,
                    Attributes = r.Attributes.Select(ra => new AttributesKeyValueViewModel
                    {
                        Key = ra.Key,
                        Value = ra.Value
                    }).ToList(),
                    RentalExtras = r.Extras.Select(re => new ExtraCheckBoxViewModel
                    {
                        ExtraId = re.ExtraId,
                        ExtraName = re.ExtraName,
                        IsChecked = true
                    }).ToList()
                })
                .ToList();



            return rentals;
        }



        public async Task<DetailsPropertyViewModel> GetRentalDetails(int rentailId)
        {
            var rental = await unitOfWork.PropertiesBaseRepository.GetAll()
                .OfType<RentalsInfo>()
                .Where(p => p.Id == rentailId)
                .Include(p => p.RentalHirePeriodType)
                .Include(p => p.Property.Address)
                .Include(p => p.Property.Address.City)
                .Include(p => p.Property.Address.City.Country)
                .Include(p => p.Property.Address.Coordinates)
                .Include(p => p.Property.Images)
                .Include(p => p.Property.Agent)
                .Include(p => p.Property.Reviews)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен имотът отдавам под наем!");

            var rentalModel = Mapper.Map<DetailsPropertyViewModel>(rental);

            return rentalModel;
        }


        #region Misc functionality connected to rentals

        public List<RentalTypeDropDownViewModel> GetRentalTypesForDropDown()
        {
            return unitOfWork.RentalsRepository
                .GetRentalTypes()
                .Select(rt => new RentalTypeDropDownViewModel
                {
                    UnitTypeId = rt.PropertyTypeId,
                    RentalTypeName = rt.PropertyTypeName
                }).ToList();
        }

        public List<RentalPeriodDropDownViewModel> GetRentalPeriods()
        {
            var rentalPeriodList = unitOfWork.RentalsRepository.GetRentalPeriodTypes()
                .Select(r =>
                    new RentalPeriodDropDownViewModel
                    {
                        Period = r.PeriodName,
                        PeriodId = r.Id
                    })
                .ToList();

            return rentalPeriodList;
        }

        #endregion

    }
}