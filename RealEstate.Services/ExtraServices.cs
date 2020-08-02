using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services
{
    public class ExtraServices:BaseService
    {
        public ExtraServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr)
            : base(unitOfWork, userMgr) {}

        public List<Extras> GetRentalExtras(List<int> rentalExtrasToAdd)
        {
            List<Extras> rentalExtras = unitOfWork.ExtrasRepository
                .GetAll()
                .OfType<Extras>()
                .Where(re => rentalExtrasToAdd.Any(r => r == re.ExtraId) && !(re is PropertyExtras))
                .ToList();

            return rentalExtras;
        }

        public List<Extras> GetPropertyExtras(List<int> propertyExtrasToAdd)
        {
            List<Extras> propertyExtras = unitOfWork.ExtrasRepository
                .GetAll()
                .OfType<Extras>()
                .Where(re => propertyExtrasToAdd.Any(r => r == re.ExtraId) && !(re is RentalExtras))
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
            List<Extras> extras = new List<Extras>();
            switch (extraType)
            {
                case "PropertyExtras":
                    extras = unitOfWork.ExtrasRepository.Where(e => !(e is RentalExtras)).ToList();
                    break;
                case "RentalExtras":
                    extras = unitOfWork.ExtrasRepository.Where(e => !(e is PropertyExtras)).ToList();
                    break;
                    default: throw new ArgumentException("No such extra");
            }

            return extras;
        }

        #region Extras for layout

        private static IDictionary<int, string> extras;

        private static IDictionary<int, string> Extras
        {
            get
            {
                if (extras == null)
                {
                    var unitOfWork = (IUnitOfWork)DependencyResolver.Current.GetService(typeof(IUnitOfWork));

                    extras = unitOfWork.ExtrasRepository.GetAll()
                        .Select(e => new { e.ExtraId, e.ExtraName })
                        .ToDictionary(e => e.ExtraId, e => e.ExtraName);
                }

                return extras;
            }
        }

        public static string GetExtraName(int extraId)
        {
            return Extras[extraId];
        }

        public static int GetExtraId(string extra)
        {
            return Extras
                .Where(pt => pt.Value.ToUpper().Contains(extra.ToUpper()))
                .Select(pt => pt.Key)
                .FirstOrDefault();
        }

        #endregion
    }
}