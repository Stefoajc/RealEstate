using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class ClientServices:BaseService
    {
        public ClientServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }

        public List<ClientListViewModel> GetHappyClients()
        {
            var happyClients = UserManager.Users.OfType<ClientUsers>()
                //.Where(c => c.Reviews.Average(r => r.ReviewScore) > 4)
                .ToList()
                .Select(c => Mapper.Map<ClientListViewModel>(c))
                .ToList();

            return happyClients;
        }

    }
}
