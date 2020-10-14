using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Model.ContactDiary;
using RealEstate.Model.Notifications;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class ContactsDiaryServices
    {
        private readonly RealEstateDbContext dbContext;
        private readonly INotificationCreator notificationCreator;

        public ContactsDiaryServices(RealEstateDbContext dbContext, INotificationCreator notificationCreator)
        {
            this.dbContext = dbContext;
            this.notificationCreator = notificationCreator;
        }

        public async Task<List<RecordListViewModel>> List(int? cityId = null, int? propertyTypeId = null
            , string phoneNumber = null, int? dealTypeId = null
            , int? contactedPersonTypeId = null, string cityDistrict = null
            , int? negotiationStateId = null, string agentId = null)
        {
            var recordsQuery = dbContext.ContactsDiary
                .Include(r => r.Agent)
                .Include(r => r.NegotiationState)
                .Include(r => r.ContactedPersonType)
                .Include(r => r.DealType)
                .Include(r => r.PropertyType);

            recordsQuery = cityId != null ? recordsQuery.Where(r => r.CityId == cityId) : recordsQuery;
            recordsQuery = propertyTypeId != null
                ? recordsQuery.Where(r => r.PropertyTypeId == propertyTypeId)
                : recordsQuery;
            recordsQuery = phoneNumber != null
                ? recordsQuery.Where(r => r.PhoneNumber == phoneNumber)
                : recordsQuery;
            recordsQuery = dealTypeId != null
                ? recordsQuery.Where(r => r.DealTypeId == dealTypeId)
                : recordsQuery;
            recordsQuery = contactedPersonTypeId != null
                ? recordsQuery.Where(r => r.ContactedPersonTypeId == contactedPersonTypeId)
                : recordsQuery;
            recordsQuery = cityDistrict != null
                ? recordsQuery.Where(r => r.CityDistrict == cityDistrict)
                : recordsQuery;
            recordsQuery = negotiationStateId != null
                ? recordsQuery.Where(r => r.NegotiationStateId == negotiationStateId)
                : recordsQuery;
            recordsQuery = agentId != null
                ? recordsQuery.Where(r => r.AgentId == agentId)
                : recordsQuery;

            return await recordsQuery
                .Select(r => new RecordListViewModel
            {
                Id = r.Id,
                CreatedOn = r.CreatedOn,
                PhoneNumber = r.PhoneNumber,
                Name = r.Name,
                CityId = r.CityId,
                CityName = r.City.CityName,
                CityDistrict = r.CityDistrict,
                Address = r.Address,
                PropertySource = r.PropertySource,
                AdditionalDescription = r.AdditionalDescription,
                ContactedPersonTypeId = r.ContactedPersonTypeId,
                ContactedPersonType = r.ContactedPersonType.ContactedPersonType,
                DealTypeId = r.DealTypeId,
                DealType = r.DealType.DealType,
                PropertyTypeId = r.PropertyTypeId,
                PropertyType = r.PropertyType.PropertyTypeName,
                NegotiationStateId = r.NegotiationStateId,
                NegotiationState = r.NegotiationState.State,
                RecordColor = r.NegotiationState.Color,
                AgentId = r.AgentId,
                AgentName = r.Agent.FirstName + " " + r.Agent.LastName
            })
                .ToListAsync();
        }

        public async Task<ContactsDiary> Get(int id)
        {
            var record = await dbContext.ContactsDiary
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new ContentNotFoundException("Не е намерен записът!");

            return record;
        }

        public async Task<RecordListViewModel> CreateRecord(CreateRecordViewModel model, string agentId)
        {
            if (!await IsAgentExisting(agentId))
            {
                throw new ArgumentException("Агентът, не е намерен!");
            }

            var record = new ContactsDiary
            {
                Address = model.Address,
                AdditionalDescription = model.AdditionalDescription,
                AgentId = agentId,
                CityDistrict = model.CityDistrict,
                CityId = model.CityId,
                ContactedPersonTypeId = model.ContactedPersonTypeId,
                DealTypeId = model.DealTypeId,
                Name = model.Name,
                NegotiationStateId = model.NegotiationStateId,
                PhoneNumber = model.PhoneNumber,
                PropertySource = model.PropertySource,
                PropertyTypeId = model.PropertyTypeId
            };

            dbContext.ContactsDiary.Add(record);
            await dbContext.SaveChangesAsync();        

            var createdClient = await dbContext.ContactsDiary
                .Include(r => r.Agent)
                .Include(r => r.NegotiationState)
                .Include(r => r.ContactedPersonType)
                .Include(r => r.DealType)
                .Include(r => r.PropertyType)
                .Where(c => c.Id == record.Id).Select(r => new RecordListViewModel
                {
                    Id = r.Id,
                    CreatedOn = r.CreatedOn,
                    PhoneNumber = r.PhoneNumber,
                    Name = r.Name,
                    CityId = r.CityId,
                    CityName = r.City.CityName,
                    CityDistrict = r.CityDistrict,
                    Address = r.Address,
                    PropertySource = r.PropertySource,
                    AdditionalDescription = r.AdditionalDescription,
                    ContactedPersonTypeId = r.ContactedPersonTypeId,
                    ContactedPersonType = r.ContactedPersonType.ContactedPersonType,
                    DealTypeId = r.DealTypeId,
                    DealType = r.DealType.DealType,
                    PropertyTypeId = r.PropertyTypeId,
                    PropertyType = r.PropertyType.PropertyTypeName,
                    NegotiationStateId = r.NegotiationStateId,
                    NegotiationState = r.NegotiationState.State,
                    RecordColor = r.NegotiationState.Color,
                    AgentId = r.AgentId,
                    AgentName = r.Agent.FirstName + " " + r.Agent.LastName
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен записът!");

            #region Notifications

            var notificationToCreate = new NotificationCreateViewModel
            {
                NotificationTypeId = (int)NotificationType.Contact,
                NotificationPicture = "",
                NotificationLink = "/contactsdiary/index?contactId=" + record.Id,
                NotificationText = createdClient.AgentName + " добави контакт: " + createdClient.ContactedPersonType + " - тел:" + createdClient.PhoneNumber
            };

            await notificationCreator.CreateGlobalNotification(notificationToCreate, agentId);

            #endregion

            return createdClient;
        }

        public async Task<RecordListViewModel> EditRecord(EditRecordViewModel model, string agentId)
        {
            if (!await IsAgentExisting(agentId))
            {
                throw new ArgumentException("Агентът, не е намерен!");
            }

            var record = await dbContext.ContactsDiary
                .FirstOrDefaultAsync(c => c.Id == model.Id)
                ?? throw new ContentNotFoundException("Не е намерен записът!");

            record.PhoneNumber = model.PhoneNumber;
            record.Name = model.Name;
            record.Address = model.Address;
            record.CityDistrict = model.CityDistrict;
            record.CityId = model.CityId;
            record.AdditionalDescription = model.AdditionalDescription;
            record.ContactedPersonTypeId = model.ContactedPersonTypeId;
            record.DealTypeId = model.DealTypeId;
            record.PropertySource = model.PropertySource;
            record.PropertyTypeId = model.PropertyTypeId;
            record.NegotiationStateId = model.NegotiationStateId;

            await dbContext.SaveChangesAsync();

            return await dbContext.ContactsDiary
                .Include(r => r.Agent)
                .Include(r => r.NegotiationState)
                .Include(r => r.ContactedPersonType)
                .Include(r => r.DealType)
                .Include(r => r.PropertyType)
                .Where(c => c.Id == record.Id).Select(r => new RecordListViewModel
                {
                    Id = r.Id,
                    CreatedOn = r.CreatedOn,
                    PhoneNumber = r.PhoneNumber,
                    Name = r.Name,
                    CityId = r.CityId,
                    CityName = r.City.CityName,
                    CityDistrict = r.CityDistrict,
                    Address = r.Address,
                    PropertySource = r.PropertySource,
                    AdditionalDescription = r.AdditionalDescription,
                    ContactedPersonTypeId = r.ContactedPersonTypeId,
                    ContactedPersonType = r.ContactedPersonType.ContactedPersonType,
                    DealTypeId = r.DealTypeId,
                    DealType = r.DealType.DealType,
                    PropertyTypeId = r.PropertyTypeId,
                    PropertyType = r.PropertyType.PropertyTypeName,
                    NegotiationStateId = r.NegotiationStateId,
                    NegotiationState = r.NegotiationState.State,
                    RecordColor = r.NegotiationState.Color,
                    AgentId = r.AgentId,
                    AgentName = r.Agent.FirstName + " " + r.Agent.LastName
                })
                .FirstOrDefaultAsync();
        }

        public async Task DeleteRecord(int recordId, string agentId)
        {
            if (!await IsAgentExisting(agentId))
            {
                throw new ArgumentException("Агентът, не е намерен!");
            }

            var record = await dbContext.ContactsDiary
                .FirstOrDefaultAsync(c => c.Id == recordId)
                ?? throw new ContentNotFoundException("Не е намерен записът!");

            if (record.AgentId != agentId)
            {
                throw new NotAuthorizedUserException("Не сте оторизиран за това действие");
            }

            dbContext.ContactsDiary.Remove(record);
            await dbContext.SaveChangesAsync();
        }


        #region DealTypes

        public Task<List<DealTypeListViewModel>> ListDealTypes()
        {
            return dbContext.DealTypes
                .Select(d => new DealTypeListViewModel
                {
                    DealTypeId = d.Id,
                    DealType = d.DealType
                })
                .ToListAsync();
        }

        #endregion

        #region NegotiationStages

        public async Task<List<NegotiationStageListViewModel>> ListNegotiationStages()
        {
            return await dbContext.NegotiationStates
                .Select(n => new NegotiationStageListViewModel
                {
                    NegotiationStageId = n.Id,
                    NegotiationStage = n.State
                })
                .ToListAsync();
        }

        #endregion

        #region PersonType

        public Task<List<PersonTypeListViewModel>> ListPersonType()
        {
            return dbContext.ContactedPersonTypes
                .Select(p => new PersonTypeListViewModel
                {
                    PersonTypeId = p.Id,
                    PersonType = p.ContactedPersonType
                })
                .ToListAsync();
        }

        #endregion

        #region Helpers

        private Task<bool> IsAgentExisting(string agentId) => dbContext.Users.AnyAsync(u => u is AgentUsers && u.Id == agentId);

        #endregion
    }
}