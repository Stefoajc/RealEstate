using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using AutoMapper;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class UserServices : BaseService
    {

        public UserServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }

        public IQueryable<ApplicationUser> GetUsersInRole(string role)
        {
            var roleId = UnitOfWork.UsersRepository.GetRoleId(role);
            return UnitOfWork.UsersRepository.FindBy(u => u.Roles.Any(r => r.RoleId == roleId));
        }


        public List<AgentListViewModel> GetTopTwoAgents()
        {
            var agents = GetTopAgents()
                .Take(2)
                .ToList()
                .Select(a => Mapper.Map<AgentListViewModel>(a));

            return agents.ToList();

        }

        public List<AgentListViewModel> GetAgents()
        {
            var agents = GetUsersInRole("Agent")
                .OfType<AgentUsers>()
                .ToList()
                .Select(a => Mapper.Map<AgentListViewModel>(a));

            return agents.ToList();
        }

        public AgentListViewModel GetAgent(string agentId)
        {
            var agent = GetUsersInRole("Agent")
                .OfType<AgentUsers>()
                .FirstOrDefault(u => u.Id == agentId);
            
            return Mapper.Map<AgentListViewModel>(agent);
        }

        private IQueryable<AgentUsers> GetTopAgents()
        {
            return GetUsersInRole("Agent")
                .OfType<AgentUsers>()
                .OrderBy(a => a.Reviews.Max(r => r.ReviewScore));
        }

































        public static List<AgentListViewModel> MockedAgents = new List<AgentListViewModel>
        {
            new AgentListViewModel
            {
                AgentId = "dajhugy329rf",
                FullName = "John Smith",
                Description =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin rutrum nisi eu ante mattis.",
                PhoneNumber = "1-800-666-8888",
                OfficePhone = "1-800-666-8888",
                Email = "JohnSmith@gmail.com",
                ImagePath = "/Images/agents/agent-1.jpg"
            },
            new AgentListViewModel
            {
                AgentId = "adsfsg3greb",
                FullName = "Andrew MCCarthy",
                Description =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin rutrum nisi eu ante mattis.",
                PhoneNumber = "1-800-666-8888",
                OfficePhone = "1-800-666-8888",
                Email = "MCCarthy@gmail.com",
                ImagePath = "/Images/agents/agent-1.jpg"
            }
        };
    }
}