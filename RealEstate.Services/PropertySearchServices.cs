using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Model.Notifications;
using RealEstate.Model.PropertySearches;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class PropertySearchServices
    {
        private readonly RealEstateDbContext _dbContext;
        private readonly INotificationCreator _notificationCreator;
        private readonly ApplicationUserManager _userManager;

        public PropertySearchServices(RealEstateDbContext dbContext, INotificationCreator notificationCreator, ApplicationUserManager userManager)
        {
            _dbContext = dbContext;
            _notificationCreator = notificationCreator;
            _userManager = userManager;
        }

        public async Task<List<PropertySearchListViewModel>> ListAsync(int? pageSize = null, int? pageNumber = null, int? cityId = null, int? unitTypeId = null, bool? onlyRentalSearches = null, string agentId = null)
        {
            var propertySearchesQuery = _dbContext.PropertySearches
                .Include(p => p.PersonSearcher)
                .Include(p => p.City)
                .Include(p => p.UnitTypes)
                .Include(p => p.Agent);

            propertySearchesQuery = cityId != null
                ? propertySearchesQuery.Where(ps => ps.CityId == cityId)
                : propertySearchesQuery;
            propertySearchesQuery = unitTypeId != null
                ? propertySearchesQuery.Where(ps => ps.UnitTypes.Any(ut => ut.PropertyTypeId == unitTypeId))
                : propertySearchesQuery;
            propertySearchesQuery = onlyRentalSearches != null
                ? propertySearchesQuery.Where(ps => ps.IsRentSearch == onlyRentalSearches)
                : propertySearchesQuery;
            propertySearchesQuery = !string.IsNullOrEmpty(agentId)
                ? propertySearchesQuery.Where(ps => ps.AgentId == agentId)
                : propertySearchesQuery;

            propertySearchesQuery = propertySearchesQuery.OrderByDescending(ps => ps.CreatedOn);

            if (pageSize != null && pageNumber != null)
            {
                propertySearchesQuery = propertySearchesQuery
                    .Skip((int) pageSize * (int) pageNumber)
                    .Take((int)pageSize);
            }

            return await propertySearchesQuery
                .Select(ps => new PropertySearchListViewModel
                {
                    Id = ps.Id,
                    CreatedOn = ps.CreatedOn,
                    Views = ps.Views,
                    PriceFrom = ps.PriceFrom,
                    PriceTo = ps.PriceTo,
                    CityId = ps.CityId,
                    CityName = ps.City.CityName,
                    IsRentSearch = ps.IsRentSearch,
                    AreaInSquareMeters = ps.AreaInSquareMeters,
                    AdditionalInformation = ps.AdditionalInformation,
                    Areas = ps.Areas,
                    UnitTypes = ps.UnitTypes.Select(u => u.PropertyTypeName).ToList(),
                    PersonSearcher = new PersonSearcherListViewModel
                    {
                        Id = ps.PersonSearcher.Id,
                        Name = ps.PersonSearcher.Name,
                        PhoneNumber = ps.PersonSearcher.PhoneNumber,
                        Email = ps.PersonSearcher.Email,
                        AdditionalInformation = ps.PersonSearcher.AdditionalInformation
                    }
                })
                .ToListAsync();
        }

        public async Task<PropertySearchListViewModel> GetAsync(int id)
        {
            var propertySearch = _dbContext.PropertySearches
                .Include(p => p.PersonSearcher)
                .Include(p => p.City)
                .Include(p => p.UnitTypes);

            return await propertySearch
                .Where(ps => ps.Id == id)
                .Select(ps => new PropertySearchListViewModel
                {
                    Id = ps.Id,
                    CreatedOn = ps.CreatedOn,
                    Views = ps.Views,
                    PriceFrom = ps.PriceFrom,
                    PriceTo = ps.PriceTo,
                    CityId = ps.CityId,
                    CityName = ps.City.CityName,
                    IsRentSearch = ps.IsRentSearch,
                    AreaInSquareMeters = ps.AreaInSquareMeters,
                    AdditionalInformation = ps.AdditionalInformation,
                    Areas = ps.Areas,
                    UnitTypes = ps.UnitTypes.Select(u => u.PropertyTypeName).ToList(),
                    PersonSearcher = new PersonSearcherListViewModel
                    {
                        Id = ps.PersonSearcher.Id,
                        Name = ps.PersonSearcher.Name,
                        PhoneNumber = ps.PersonSearcher.PhoneNumber,
                        Email = ps.PersonSearcher.Email,
                        AdditionalInformation = ps.PersonSearcher.AdditionalInformation
                    }
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PropertySearchDetailViewModel> GetDetailsAsync(int id)
        {
            var propertySearch = _dbContext.PropertySearches
                .Include(p => p.PersonSearcher)
                .Include(p => p.City)
                .Include(p => p.UnitTypes)
                .Include(p => p.Agent)
                .Include(p => p.Agent.Images);

            var propertySearchDetails = await propertySearch
                .Where(ps => ps.Id == id)
                .Select(ps => new PropertySearchDetailViewModel
                {
                    Id = ps.Id,
                    CreatedOn = ps.CreatedOn,
                    Views = ps.Views,
                    PriceFrom = ps.PriceFrom,
                    PriceTo = ps.PriceTo,
                    CityId = ps.CityId,
                    CityName = ps.City.CityName,
                    IsRentSearch = ps.IsRentSearch,
                    AreaInSquareMeters = ps.AreaInSquareMeters,
                    AdditionalInformation = ps.AdditionalInformation,
                    Areas = ps.Areas,
                    UnitTypes = ps.UnitTypes.Select(u => u.PropertyTypeName).ToList(),
                    PersonSearcher = new PersonSearcherListViewModel
                    {
                        Id = ps.PersonSearcher.Id,
                        Name = ps.PersonSearcher.Name,
                        PhoneNumber = ps.PersonSearcher.PhoneNumber,
                        Email = ps.PersonSearcher.Email,
                        AdditionalInformation = ps.PersonSearcher.AdditionalInformation
                    },
                    TeamUser = new TeamUserListViewModel
                    {
                        ImagePath = ps.Agent.Images.Select(i => i.ImagePath).FirstOrDefault(),
                        AgentId = ps.AgentId,
                        Email = ps.Agent.Email,
                        PhoneNumber = ps.Agent.PhoneNumber,
                        FullName = ps.Agent.FirstName + " " + ps.Agent.LastName,
                        AdditionalDescription = ps.Agent.LastName
                    }
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерено търсенето, което сте избрали!");

            propertySearchDetails.TeamUser.OfficePhone = ConfigurationManager.AppSettings["OfficePhone"];
            return propertySearchDetails;
        }

        public async Task<PropertySearchListViewModel> CreateAsync(PropertySearchCreateViewModel model, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е установен брокерът задаващ въпроса!");
            }

            if (!(await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                  || await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи и брокери имат достъп !");
            }

            PropertySearches propertySearch = new PropertySearches
            {
                CityId = model.CityId,
                AdditionalInformation = model.AdditionalInformation,
                AgentId = agentId,
                AreaInSquareMeters = model.AreaInSquareMeters,
                Areas = model.Areas,
                IsRentSearch = model.IsRentSearch,
                PriceFrom = model.PriceFrom,
                PriceTo = model.PriceTo,
                UnitTypes = new HashSet<PropertyTypes>(await _dbContext.PropertyTypes
                    .Where(pt => model.UnitTypeIds.Any(ut => pt.PropertyTypeId == ut))
                    .ToListAsync()),
                PersonSearcher = new PersonSearcher
                {
                    Name = model.PersonSearcher.Name,
                    PhoneNumber = model.PersonSearcher.PhoneNumber,
                    Email = model.PersonSearcher.Email,
                    AdditionalInformation = model.PersonSearcher.AdditionalInformation
                }
            };

            _dbContext.PropertySearches.Add(propertySearch);
            await _dbContext.SaveChangesAsync();

            #region Create notification

            var creatorName = await _dbContext.Users
                .Where(u => u.Id == propertySearch.AgentId)
                .Select(a => a.FirstName + " " + a.LastName)
                .FirstOrDefaultAsync();

            var notificationToCreate = new NotificationCreateViewModel
            {
                NotificationTypeId = (int)NotificationType.Property,
                NotificationLink = "/propertysearches/details?id=" + propertySearch.Id ,
                NotificationText = creatorName + " добави търсене на имот с цена " + propertySearch.PriceFrom + "лв." + " - " + propertySearch.PriceTo + "лв."
            };

            await _notificationCreator.CreateGlobalNotification(notificationToCreate, propertySearch.AgentId);

            #endregion

            return await GetAsync(propertySearch.Id);
        }

        public async Task<PropertySearchListViewModel> EditAsync(PropertySearchEditViewModel model, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е установен брокерът задаващ въпроса!");
            }

            var propertySearchToEdit = await _dbContext.PropertySearches
                .Include(p => p.PersonSearcher)
                .FirstOrDefaultAsync(p => p.Id == model.Id)
                ?? throw new ContentNotFoundException("Не е намерено търсенето, което искате да редактирате");

            if (propertySearchToEdit.AgentId != agentId &&
                !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Нямате право да изтривате чуждо търсене!");
            }

            _dbContext.PropertySearches.Attach(propertySearchToEdit);
            propertySearchToEdit.CityId = model.CityId;
            propertySearchToEdit.AdditionalInformation = model.AdditionalInformation;
            propertySearchToEdit.AreaInSquareMeters = model.AreaInSquareMeters;
            propertySearchToEdit.Areas = model.Areas;
            propertySearchToEdit.IsRentSearch = model.IsRentSearch;
            propertySearchToEdit.PriceFrom = model.PriceFrom;
            propertySearchToEdit.PriceTo = model.PriceTo;

            var unitTypeIds = model.UnitTypes.Select(u => u.PropertyTypeId);
            propertySearchToEdit.UnitTypes = new HashSet<PropertyTypes>(await _dbContext.PropertyTypes
                .Where(pt => unitTypeIds.Any(ut => pt.PropertyTypeId == ut))
                .ToListAsync());

            propertySearchToEdit.PersonSearcher.Name = model.PersonSearcher.Name;
            propertySearchToEdit.PersonSearcher.PhoneNumber = model.PersonSearcher.PhoneNumber;
            propertySearchToEdit.PersonSearcher.Email = model.PersonSearcher.Email;
            propertySearchToEdit.PersonSearcher.AdditionalInformation = model.PersonSearcher.AdditionalInformation;
            await _dbContext.SaveChangesAsync();

            return await GetAsync(propertySearchToEdit.Id);
        }

        public async Task DeleteAsync(int id, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е установен брокерът задаващ въпроса!");
            }

            var searchToDelete = await _dbContext.PropertySearches
                .Include(ps => ps.PersonSearcher)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new ContentNotFoundException("Не е намарено търсенето на имот, което искате да изтриете!");

            if (searchToDelete.AgentId != agentId &&
                !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Нямате право да изтривате чуждо търсене!");
            }

            _dbContext.PropertySearches.Remove(searchToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync(bool? onlyRentalSearches)
        {
            var countQuery = _dbContext.PropertySearches.AsQueryable();

            countQuery = onlyRentalSearches != null
                ? countQuery.Where(ps => ps.IsRentSearch == onlyRentalSearches)
                : countQuery;

            return await countQuery.CountAsync();
        }

        public async Task<PropertySearchEditViewModel> GetEditAsync(int id)
        {
            var propertySearch = _dbContext.PropertySearches
                .Include(p => p.PersonSearcher)
                .Include(p => p.City)
                .Include(p => p.UnitTypes)
                .Include(p => p.Agent)
                .Include(p => p.Agent.Images);

            var propertySearchEditViewModel = await propertySearch
                .Where(ps => ps.Id == id)
                .Select(ps => new PropertySearchEditViewModel
                {
                    Id = ps.Id,
                    PriceFrom = ps.PriceFrom,
                    PriceTo = ps.PriceTo,
                    CityId = ps.CityId,
                    IsRentSearch = ps.IsRentSearch,
                    AreaInSquareMeters = ps.AreaInSquareMeters,
                    AdditionalInformation = ps.AdditionalInformation,
                    Areas = ps.Areas,
                    UnitTypes = ps.UnitTypes.Select(u => new DropDownPropertyTypesViewModel
                    {
                        PropertyTypeName = u.PropertyTypeName,
                        PropertyTypeId = u.PropertyTypeId
                    })
                    .ToList(),
                    PersonSearcher = new PersonSearcherCreateViewModel
                    {
                        Name = ps.PersonSearcher.Name,
                        PhoneNumber = ps.PersonSearcher.PhoneNumber,
                        Email = ps.PersonSearcher.Email,
                        AdditionalInformation = ps.PersonSearcher.AdditionalInformation
                    }
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерено търсенето, което сте избрали!");
            
            return propertySearchEditViewModel;
        }
    }
}