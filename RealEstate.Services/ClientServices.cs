using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class ClientServices:BaseService
    {
        public ClientServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        {
        }

        public List<ClientListViewModel> GetHappyClients()
        {
            var happyClients = userManager.Users.OfType<ClientUsers>()
                //.Where(c => c.Reviews.Average(r => r.ReviewScore) > 4)
                .ToList()
                .Select(Mapper.Map<ClientListViewModel>)
                .ToList();

            return happyClients;
        }

    }
}
