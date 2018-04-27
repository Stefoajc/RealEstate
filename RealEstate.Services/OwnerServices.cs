using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class OwnerServices : BaseService
    {

        private UserServices UsersManager { get; set; }

        public OwnerServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr,UserServices usersServices) : base(unitOfWork, user, userMgr)
        {
            UsersManager = usersServices;
        }


        public List<AgentListViewModel> ListOwners()
        {
            var owners = UsersManager.GetUsersInRole("Owner")
                .OfType<OwnerUsers>()
                .Include(o => o.Properties)
                .Include(o => o.Images)
                .Include(o => o.Reviews)
                .Select(o => new AgentListViewModel
                {
                    ImagePath = o.Images.Select(i => i.ImagePath).FirstOrDefault(),
                    Email = o.Email,
                    AgentId = o.Id,
                    PhoneNumber = o.PhoneNumber,
                    Description = o.Description,
                    FullName = o.FirstName + " " + o.LastName,
                    OfficePhone = "359876717000"
                }).ToList();

            return owners;
        }

        public ApplicationUser GetOwner(string ownerId)
        {
            var owner = UnitOfWork.UsersRepository.FindBy(u => u.Id == ownerId).OfType<OwnerUsers>().FirstOrDefault() ?? throw new UserNotFoundException();

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

            var ownerUser = await UserManager.CreateAsync(owner, ownerModel.Password);
            await UserManager.AddToRoleAsync(owner.Id, "Owner");
        }

    }
}
