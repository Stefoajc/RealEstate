using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Repositories
{
    public class PropertiesRepository : GenericRepository<RealEstateDbContext, Properties>, IPropertiesRepository
    {
        public PropertiesRepository(RealEstateDbContext db) : base(db)
        {
        }

        public async Task<List<PropertyTypes>> ListPropertyTypesAsync()
        {
            return await Context.PropertyTypes.ToListAsync();
        }

        public List<PropertyTypes> ListPropertyTypes()
        {
            return Context.PropertyTypes.ToList();
        }

        public async Task<List<PropertySeason>> ListPropertySeasons()
        {
            return await Context.PropertyRentPeriods.ToListAsync();
        }


        public IQueryable<PropertyInfoDTO> GetAll(bool excludeRentals)
        {
            var properties = Context.Properties
                .Include(r => r.Rentals)
                .Include(r => r.Address)
                .Include(r => r.Address.City)
                .Include(r => r.Address.City.Country)
                .Include(p => p.Address.Coordinates)
                .Include(r => r.Reviews)
                .Include(r => r.Extras)
                .Include(r => r.Owner)
                .Include(r => r.Agent)
                .Include(r => r.Images)
                .Include(r => r.Reservations)
                .Include(r => r.Attributes)
                .Include(r => r.PropertySeason)
                .Include(r => r.Appointments)
                .Include(r => r.UnitType)
                .Include(r => r.RentalHirePeriodType)
                .Include(r => r.PropertyLikes)
                .Select(property => new
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
                    IsRental = false,
                    PropertyLikesCount = property.PropertyLikes.Count,
                    MainImagePath = property.Images.Select(i => i.ImagePath).FirstOrDefault(),
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


            if (!excludeRentals)
            {
                var rentals = Context.RentalsInfo
                    .Include(r => r.Property)
                    .Include(r => r.Property.Address)
                    .Include(r => r.Property.Address.City)
                    .Include(r => r.Property.Address.City.Country)
                    .Include(r => r.Property.Address.Coordinates)
                    .Include(r => r.Reviews)
                    .Include(r => r.Extras)
                    .Include(r => r.Property.Owner)
                    .Include(r => r.Property.Owner.Images)
                    .Include(r => r.Property.Agent)
                    .Include(r => r.Property.Agent.Images)
                    .Include(r => r.Property.Agent.SocialMediaAccounts)
                    .Include(r => r.Property.Images)
                    .Include(r => r.Reservations)
                    .Include(r => r.Attributes)
                    .Include(r => r.Property.PropertySeason)
                    .Include(r => r.Appointments)
                    .Include(r => r.UnitType)
                    .Include(r => r.RentalHirePeriodType)
                    .Include(r => r.PropertyLikes)
                    .Select(property => new
                    {
                        Id = property.Id,
                        PropertyName = property.Property.PropertyName,
                        SellingPrice = property.Property.SellingPrice,
                        RentalPrice = property.RentalPrice,
                        AreaInSquareMeters = property.AreaInSquareMeters,
                        AdditionalDescription = property.AdditionalDescription,
                        CreatedOn = property.CreatedOn,
                        Views = property.Views,
                        IsActive = property.Property.IsActive,
                        IsRental = true,
                        PropertyLikesCount = property.PropertyLikes.Count,
                        MainImagePath = property.Property.Images.Select(i => i.ImagePath).FirstOrDefault(),
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
                            ImagePath = property.Property.Agent.Images.Select(i => i.ImagePath).FirstOrDefault()
                        }
                    });

                properties = properties.Concat(rentals);


            }

            var result = properties.Select(property => new PropertyInfoDTO
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
                PropertyLikesCount = property.PropertyLikesCount,
                ImagePath = property.MainImagePath,
                RentalHirePeriodType = property.RentalHirePeriodType,
                UnitType = property.UnitType,
                Address = property.Address,
                PropertySeason = property.PropertySeason,
                Owner = property.Owner,
                Agent = property.Agent,
                IsRental = property.IsRental,
            });

            var test = result.ToList();

            return result;
        }



        //public IQueryable<PropertyInfoViewModel> GetPropertyInfo(bool excludeRentals)
        //{
        //    var result = Context.Properties
        //        .Include(r => r.Rentals)
        //        .Include(r => r.Address)
        //        .Include(r => r.Address.City)
        //        .Include(r => r.Address.City.Country)
        //        .Include(p => p.Address.Coordinates)
        //        .Include(r => r.Reviews)
        //        .Include(r => r.PropertyExtras)
        //        .Include(r => r.Owner)
        //        .Include(r => r.Agent)
        //        .Include(r => r.Images)
        //        .Include(r => r.Reservations)
        //        .Include(r => r.Attributes)
        //        .Include(r => r.PropertySeason)
        //        .Include(r => r.Appointments)
        //        .Include(r => r.UnitType)
        //        .Include(r => r.RentalHirePeriodType)
        //        .Include(r => r.PropertyLikes)
        //        .Select(property => new PropertyInfoViewModel
        //        {
        //            PropertyId = property.Id,
        //            PropertyName = property.PropertyName,
        //            ImagePath = property.Images.Select(i => i.ImagePath).FirstOrDefault(),
        //            CreatedOn = 
        //        })
        //}
    }
}