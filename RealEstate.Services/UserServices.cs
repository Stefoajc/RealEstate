using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Ninject;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class UserServices : BaseService
    {
        [Inject]
        public UserServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        {
        }

        public IQueryable<ApplicationUser> GetUsersInRole(string role)
        {
            var roleId = unitOfWork.UsersRepository.GetRoleId(role);
            return unitOfWork.UsersRepository.Where(u => u.Roles.Any(r => r.RoleId == roleId));
        }


        public async Task<List<TeamUserListViewModel>> GetTopTwoAgents()
        {
            var agents = (await GetTopAgents()
                .Take(2)
                .ToListAsync())
                .Select(Mapper.Map<TeamUserListViewModel>);

            return agents.ToList();

        }

        public async Task<List<TeamUserListViewModel>> GetAgents()
        {
            var agents = (await GetUsersInRole(AgentRole)
                .OfType<AgentUsers>()
                .ToListAsync())
                .Select(Mapper.Map<TeamUserListViewModel>);

            return agents.ToList();
        }

        public async Task<TeamUserListViewModel> GetAgent(string agentId)
        {
            var agent = await GetUsersInRole(AgentRole)
                .OfType<AgentUsers>()
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(u => u.Id == agentId) ?? throw new ContentNotFoundException("Не е намерен брокерът!");

            var details = Mapper.Map<TeamUserListViewModel>(agent);
            details.ReviewsInfo = new ReviewsStarsPartialViewModel
            {
                AverageScore = agent.Reviews.Average(a => a.ReviewScore),
                ReviewsCount = agent.Reviews.Count()
            };

            return details;
        }

        private IQueryable<ApplicationUser> GetTopAgents()
        {
            return GetUsersInRole(AgentRole)
                .OfType<AgentUsers>()
                .OrderBy(a => a.Reviews.Max(r => r.ReviewScore));
        }


        public async Task<List<TeamUserListViewModel>> GetMarketers()
        {
            var marketers = (await GetUsersInRole(MarketerRole).ToListAsync())
                .Select(Mapper.Map<TeamUserListViewModel>)
                .ToList();

            return marketers;
        }

        public async Task<List<TeamUserListViewModel>> GetMaintenance()
        {
            var maintenance = (await GetUsersInRole(MaintenanceRole).ToListAsync())
                .Select(Mapper.Map<TeamUserListViewModel>)
                .ToList();

            return maintenance;
        }


        public bool DoesRoleExist(string role)
        {
            var roles = Enum.GetNames(typeof(Role));

            return roles.Any(r => r == role);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language">en|bg</param>
        /// <returns></returns>
        public string[] GetRoles(string language = "en")
        {
            List<RolesViewModel> roles = new List<RolesViewModel>
            {
                new RolesViewModel{ Name = AgentRole, Language = "en"},
                new RolesViewModel{ Name = "Агент/Брокер", Language = "bg"},
                new RolesViewModel{ Name = ClientRole, Language = "en"},
                new RolesViewModel{ Name = "Клиент", Language = "bg"},
                new RolesViewModel{ Name = MaintenanceRole, Language = "en"},
                new RolesViewModel{ Name = "Поддръжка", Language = "bg"},
                new RolesViewModel{ Name = MarketerRole, Language = "en"},
                new RolesViewModel{ Name = "Маркетолог", Language = "bg"},
                new RolesViewModel{ Name = OwnerRole, Language = "en"},
                new RolesViewModel{ Name = "Собственик на имот", Language = "bg"},
                new RolesViewModel{ Name = AdminRole, Language = "en"},
                new RolesViewModel{ Name = "Администратор", Language = "bg"}
            };

            return roles.Where(r => r.Language == language).Select(r => r.Name).ToArray();
        }

        public List<TeamMemberViewModel> GetTeamMembers()
        {
            var roleId = unitOfWork.UsersRepository.GetRoleId(AgentRole);
            var users = userManager.Users
                .Include(u => u.SocialMediaAccounts)
                .Where(u => u.Roles.Any(r => r.RoleId == roleId))
                .ToList()
                .Select(Mapper.Map<TeamMemberViewModel>)
                .ToList();

            return users;
        }


        public async Task<bool> RegisterTeamUser(RegisterViewModel model, string role)
        {
            ApplicationUser teamUser;
            if (role == AgentRole)
            {
                teamUser = new AgentUsers
                {
                    UserName = model.UserName.Trim(' '),
                    Email = model.Email,
                    CreatedOn = DateTime.Now
                };
            }
            else
            {
                teamUser = new ApplicationUser
                {
                    UserName = model.UserName.Trim(' '),
                    Email = model.Email,
                    CreatedOn = DateTime.Now
                };
            }
            var result = await userManager.CreateAsync(teamUser, model.Password);

            if (result.Succeeded)
            {
                var currentUser = await userManager.FindByNameAsync(teamUser.UserName);
                await userManager.AddToRolesAsync(currentUser.Id, TeamUserRole , role);

                return true;
            }

            return false;
        }



        public async Task AddSocialMediaAccounts(SocialMediaAccountViewModel socialMediaAccountViewModel, string userId)
        {
            await userManager.AddUpdateClaim(new Claim(socialMediaAccountViewModel.SocialMedia, socialMediaAccountViewModel.SocialMediaAccount), userId);
        }


        public async Task<int> CreateNonRegisteredUser(NonRegisteredUserCreateDTO userModel)
        {
            if (string.IsNullOrEmpty(userModel.ClientName))
            {
                throw new ArgumentException("ClientName not provided!");
            }
            if (string.IsNullOrEmpty(userModel.ClientEmail) && string.IsNullOrEmpty(userModel.ClientPhoneNumber))
            {
                throw new ArgumentException("ClientEmail nor ClientPhoneNumber provided!");
            }

            NonRegisteredAppointmentUsers appointmentUser = new NonRegisteredAppointmentUsers
            {
                ClientName = userModel.ClientName,
                ClientEmail = userModel.ClientEmail,
                ClientPhoneNumber = userModel.ClientPhoneNumber
            };

            unitOfWork.UsersRepository.AddNonRegisteredUser(appointmentUser);
            await unitOfWork.SaveAsync();
            return appointmentUser.Id;
        }

        
        public static List<TeamUserListViewModel> MockedAgents = new List<TeamUserListViewModel>
        {
            new TeamUserListViewModel
            {
                AgentId = "dajhugy329rf",
                FullName = "John Smith",
                AdditionalDescription =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin rutrum nisi eu ante mattis.",
                PhoneNumber = "1-800-666-8888",
                OfficePhone = "1-800-666-8888",
                Email = "JohnSmith@gmail.com",
                ImagePath = "/Images/agents/agent-1.jpg"
            },
            new TeamUserListViewModel
            {
                AgentId = "adsfsg3greb",
                FullName = "Andrew MCCarthy",
                AdditionalDescription =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin rutrum nisi eu ante mattis.",
                PhoneNumber = "1-800-666-8888",
                OfficePhone = "1-800-666-8888",
                Email = "MCCarthy@gmail.com",
                ImagePath = "/Images/agents/agent-1.jpg"
            }
        };
    }
}