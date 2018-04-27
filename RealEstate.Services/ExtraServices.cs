using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services
{
    public class ExtraServices:BaseService
    {
        public ExtraServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }

        public List<RentalExtras> AddRentalExtras(List<int> rentalExtrasToAdd)
        {
            List<RentalExtras> rentalExtras = UnitOfWork.ExtrasRepository
                .GetAll()
                .OfType<RentalExtras>()
                .Where(re => rentalExtrasToAdd.Any(r => r == re.ExtraId))
                .ToList();

            return rentalExtras;
        }

        public List<PropertyExtras> CreatePropertyExtras(List<int> propertyExtrasToAdd)
        {
            List<PropertyExtras> propertyExtras = UnitOfWork.ExtrasRepository
                .GetAll()
                .OfType<PropertyExtras>()
                .Where(re => propertyExtrasToAdd.Any(r => r == re.ExtraId))
                .ToList();

            return propertyExtras;
        }


        /// <summary>
        /// extraType is one of PropertyExtras or RentalExtras
        /// </summary>
        /// <param name="extraType"></param>
        /// <returns></returns>
        public List<Extras> GetExtras(string extraType)
        {

            List<Extras> extras;
            switch (extraType)
            {
                case "PropertyExtras":
                    extras = UnitOfWork.ExtrasRepository.FindBy(e => e is PropertyExtras).ToList();
                    break;
                case "RentalExtras":
                    extras = UnitOfWork.ExtrasRepository.FindBy(e => e is RentalExtras).ToList();
                    break;
                    default: throw new ArgumentException("No such extra");
            }

            return extras;
        }
    }
}