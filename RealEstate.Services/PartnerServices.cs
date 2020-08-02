using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Model.Reports;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC.Reports;

namespace RealEstate.Services
{
    public class PartnerServices
    {
        private readonly RealEstateDbContext dbContext;
        private readonly ApplicationUserManager userManager;
        private readonly int timeWindowForChangesInMinutes = int.Parse(ConfigurationManager.AppSettings["TimeWindowForChangesInMinutes"]);

        public PartnerServices(RealEstateDbContext dbContext, ApplicationUserManager userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<List<PartnersListViewModel>> ListAsync(string issuerAgentId, string createdById = null)
        {
            var partners = dbContext.Partners
                .Include(c => c.City)
                .Include(c => c.Agent)
                .Include(c => c.PartnerType);

            partners = string.IsNullOrEmpty(createdById)
                ? partners
                : partners.Where(c => c.AgentIdCreator == createdById);

            var colleaguesProjection = await partners
                .Select(c => new PartnersListViewModel
                {
                    Id = c.Id,
                    CreatedOn = c.CreatedOn,
                    PartnerName = c.PartnerName,
                    CityName = c.City.CityName,
                    CityId = c.CityId,
                    PartnerCompanyName = c.PartnerCompanyName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    SocialMediaAccount = c.SocialMediaAccount,
                    PartnerTypeId = c.PartnerTypeId,
                    PartnerTypeName = c.PartnerType.Type,
                    AdditionalInformation = c.AdditionalInformation,
                    AgentCreatorId = c.AgentIdCreator,
                    AgentCreatorName = c.Agent.FirstName + " " + c.Agent.LastName
                })
                .ToListAsync();

            var isAdmin = await userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Administrator));
            var isMaintenance = await userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Maintenance));

            foreach (var partner in colleaguesProjection)
            {
                var isOwner = partner.AgentCreatorId == issuerAgentId;
                var isInTimeForChange = partner.CreatedOn > DateTime.Now.AddMinutes(-timeWindowForChangesInMinutes);

                partner.IsAllowedToEdit = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
                partner.IsAllowedToDelete = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
            }

            return colleaguesProjection;
        }

        public async Task<List<PartnersDropdownViewModel>> ListBrokersForDropdown()
        {
            var brokerType = await dbContext.PartnerTypes
                .Where(pt => pt.Type.ToUpper() == "БРОКЕР")
                .FirstOrDefaultAsync();

            var brokers = dbContext.Partners
                .Where(p => p.PartnerTypeId == brokerType.Id)
                .Select(b => new PartnersDropdownViewModel
                {
                    Id = b.Id,
                    BrokerInfo = b.PartnerName + (b.PartnerCompanyName != null ? " от " + b.PartnerCompanyName : "" )
                })
                .ToListAsync();

            return await brokers;
        }

        public async Task<PartnersListViewModel> Get(string issuerAgentId, int id)
        {
            var collegues = dbContext.Partners
                .Include(c => c.City)
                .Include(c => c.Agent)
                .Where(c => c.Id == id);

            var partnerProjection = await collegues
                .Select(c => new PartnersListViewModel
                {
                    Id = c.Id,
                    CreatedOn = c.CreatedOn,
                    PartnerName = c.PartnerName,
                    CityName = c.City.CityName,
                    CityId = c.CityId,
                    PartnerCompanyName = c.PartnerCompanyName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    SocialMediaAccount = c.SocialMediaAccount,
                    PartnerTypeId = c.PartnerTypeId,
                    PartnerTypeName = c.PartnerType.Type,
                    AdditionalInformation = c.AdditionalInformation,
                    AgentCreatorId = c.AgentIdCreator,
                    AgentCreatorName = c.Agent.FirstName + " " + c.Agent.LastName
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен партньорът!");

            var isAdmin = await userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Administrator));
            var isMaintenance = await userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Maintenance));
            var isOwner = partnerProjection.AgentCreatorId == issuerAgentId;
            var isInTimeForChange = partnerProjection.CreatedOn > DateTime.Now.AddMinutes(-timeWindowForChangesInMinutes);

            partnerProjection.IsAllowedToEdit = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
            partnerProjection.IsAllowedToDelete = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;

            return partnerProjection;
        }

        public async Task<bool> Exist(string phoneNumber)
        {
            return await dbContext.Partners.AnyAsync(c => c.PhoneNumber == phoneNumber);
        }

        public async Task<PartnersListViewModel> CreateAsync(PartnersCreateViewModel model, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът, който създава Партньори!");
            }

            if (!await IsAdminOrAgent(agentId))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи и брокери имат право да създават Партньори !");
            }

            if (await Exist(model.PhoneNumber))
            {
                throw new ArgumentException("Съществува партньор с този телефонен номер!");
            }

            if (!await IsPartnerTypeExisting(model.PartnerTypeId))
            {
                throw new ContentNotFoundException("Не съществува типът Партньор, който сте избрали!");
            }

            Partners colleague = new Partners
            {
                PartnerName = model.PartnerName,
                PartnerCompanyName = model.PartnerCompanyName,
                CityId = model.CityId,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                SocialMediaAccount = model.SocialMediaAccount,
                AdditionalInformation = model.AdditionalInformation,
                AgentIdCreator = agentId,
                PartnerTypeId = model.PartnerTypeId
            };

            dbContext.Partners.Add(colleague);
            await dbContext.SaveChangesAsync();

            return await Get(agentId, colleague.Id);
        }

        public async Task<PartnersListViewModel> EditAsync(PartnersEditViewModel model, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът, който редактира информацията за Партньор!");
            }

            if (!await IsAdminOrAgent(agentId))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи и брокери имат право да редактират информацията за Партньори !");
            }

            if (dbContext.Partners.Any(p => p.PhoneNumber == model.PhoneNumber && p.Id != model.Id))
            {
                throw new ArgumentException("Съществува партньор с този телефонен номер!");
            }

            var colleagueToEdit = await dbContext.Partners
                .FirstOrDefaultAsync(c => c.Id == model.Id) 
                ?? throw new ContentNotFoundException("Не е намеренa информацията за партньорът, който искате да редактирате.");

            if (colleagueToEdit.AgentIdCreator != agentId && !await IsUserAdministrator(agentId))
            {
                throw new NotAuthorizedUserException("Нямате право да редактирате Партньор добавен от друг Агент!");
            }

            if (!await IsPartnerTypeExisting(model.PartnerTypeId))
            {
                throw new ContentNotFoundException("Не съществува типът Партньор, който сте избрали!");
            }

            dbContext.Partners.Attach(colleagueToEdit);
            colleagueToEdit.PartnerCompanyName = model.PartnerCompanyName;
            colleagueToEdit.PartnerName = model.PartnerName;
            colleagueToEdit.PhoneNumber = model.PhoneNumber;
            colleagueToEdit.Email = model.Email;
            colleagueToEdit.CityId = model.CityId;
            colleagueToEdit.SocialMediaAccount = model.SocialMediaAccount;
            colleagueToEdit.AdditionalInformation = model.AdditionalInformation;
            colleagueToEdit.PartnerTypeId = model.PartnerTypeId;
            await dbContext.SaveChangesAsync();

            return await Get(agentId, model.Id);
        }

        public async Task DeleteAsync(int id, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът, който изтрива информацията за Партньор!");
            }

            var colleagueToDelete = await dbContext.Partners
                .FirstOrDefaultAsync(c => c.Id == id) 
                ?? throw new ContentNotFoundException("Не е намеренa информацията за партньорът, който искате да редактирате.");

            if (colleagueToDelete.AgentIdCreator != agentId && !await IsUserAdministrator(agentId))
            {
                throw new NotAuthorizedUserException("Нямате право да изтриете информация за Парньор добавен от друг Агент!");
            }

            dbContext.Partners.Remove(colleagueToDelete);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<PartnerTypesListViewModel>> ListTypes()
        {
            return await dbContext.PartnerTypes
                .Select(pt => new PartnerTypesListViewModel
                {
                    Id = pt.Id,
                    Type = pt.Type
                })
                .ToListAsync();
        }

        public async Task<PartnerTypesListViewModel> GetPartnerType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Не е въведен търсеният тип партньор!");
            }

            var partnerType = await dbContext.PartnerTypes
                .Where(pt => pt.Type.ToUpper() == type.ToUpper())
                .Select(pt => new PartnerTypesListViewModel
                {
                    Id = pt.Id,
                    Type = pt.Type
                })
                .FirstOrDefaultAsync();

            return partnerType;
        }

        #region Validations

        private Task<bool> IsPartnerTypeExisting(int partnerTypeId) => dbContext.PartnerTypes.AnyAsync(pt => pt.Id == partnerTypeId);

        private Task<bool> IsUserInRole(string agentId, Role role) => userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), role));

        private Task<bool> IsUserAdministrator(string agentId) => IsUserInRole(agentId, Role.Administrator);

        private async Task<bool> IsAdminOrAgent(string agentId) => await IsUserInRole(agentId, Role.Administrator) || await IsUserInRole(agentId, Role.Agent);

        #endregion
    }
}