using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using RealEstate.Model;
using RealEstate.Model.Notifications;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class OwnerServices : BaseService
    {

        private readonly UserServices usersManager;
        private readonly INotificationCreator notificationCreator;

        public OwnerServices(IUnitOfWork unitOfWork
            , ApplicationUserManager userMgr
            , UserServices usersServices
            , INotificationCreator notificationCreator) 
            : base(unitOfWork, userMgr)
        {
            usersManager = usersServices;
            this.notificationCreator = notificationCreator;
        }

        /// <summary>
        /// List all owners with which the Agent is working
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //public async Task<List<dynamic>> ListAgentOwners(string userId)
        //{
        //    throw new NotImplementedException();
        //} 

        public List<TeamUserListViewModel> ListOwners()
        {
            var owners = usersManager.GetUsersInRole(OwnerRole)
                .OfType<OwnerUsers>()
                .Include(o => o.Properties)
                .Include(o => o.Images)
                .Include(o => o.Reviews)
                .Select(o => new TeamUserListViewModel
                {
                    ImagePath = o.Images.Select(i => i.ImagePath).FirstOrDefault(),
                    Email = o.Email,
                    AgentId = o.Id,
                    PhoneNumber = o.PhoneNumber,
                    AdditionalDescription = o.AdditionalDescription,
                    FullName = o.FirstName + " " + o.LastName,
                    OfficePhone = "359876717000"
                }).ToList();

            return owners;
        }

        public async Task<ApplicationUser> GetOwner(string ownerId)
        {
            var owner = await unitOfWork.UsersRepository
                .Where(u => u.Id == ownerId)
                .OfType<OwnerUsers>()
                .FirstOrDefaultAsync() 
                ?? throw new UserNotFoundException("Не е намерен собственикът, който търсите!");

            return owner;
        }

        public async Task<ApplicationUser> GetOwner(int propertyId)
        {
            var owner = await unitOfWork.PropertiesRepository
                .Include(p => p.Owner)
                .Where(u => u.Id == propertyId)
                .Select(p => p.Owner)
                .FirstOrDefaultAsync() 
                ?? throw new UserNotFoundException("Не е намерен собственикът, който търсите!");

            return owner;
        }

        public async Task Create(RegisterViewModel ownerModel)
        {
            var owner = new OwnerUsers
            {
                Email = ownerModel.Email,
                UserName = ownerModel.UserName,
                EmailConfirmed = true
            };

            var ownerUser = await userManager.CreateAsync(owner, ownerModel.Password);
            await userManager.AddToRoleAsync(owner.Id, OwnerRole);
        }



        public async Task NotifyForBirthdays()
        {
            var agentsToBeNotifiedForOwnersBirthday = await unitOfWork.PropertiesRepository
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .Where(p => p.Owner.CreatedOn.Year == DateTime.Now.Year
                            && p.Owner.CreatedOn.Month == DateTime.Now.Month
                            && p.Owner.CreatedOn.Day == DateTime.Now.Day)
                .Select(p => new
                {
                    AgentId = p.AgentId,
                    OwnerFullName = p.Owner.FirstName + " " + p.Owner.LastName,
                    OwnerPhoneNumber = p.Owner.PhoneNumber
                })
                .GroupBy(a => new { a.AgentId, a.OwnerFullName, a.OwnerPhoneNumber })
                .ToListAsync();

            foreach (var agent in agentsToBeNotifiedForOwnersBirthday)
            {
                await notificationCreator.CreateIndividualNotification(new NotificationCreateViewModel
                {
                    NotificationTypeId = (int)NotificationType.Birthday,
                    NotificationLink = null, //TODO: Create View with all owners with which the agent is working
                    NotificationText = $"{agent.Key.OwnerFullName} телефон: {agent.Key.OwnerPhoneNumber} има рожден днес" +
                                       ", поздравете го за да му повдигнете духа :)"
                }
                , agent.Key.AgentId);
            }
        }
    }
}
