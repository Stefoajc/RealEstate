using System;
using System.Linq;
using System.Security.Principal;
using Ninject;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services
{
    public class OwnerRegisterCodeServices : BaseService
    {
        [Inject]
        public OwnerRegisterCodeServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }

        public string AddOwnerRegisterCode()
        {
            var code = new OwnerRegisterCodes() { OwnerRegisterCode = Guid.NewGuid().ToString()};
            UnitOfWork.OwnerRegisterCodesRepository.Add(code);
            UnitOfWork.Save();

            return code.OwnerRegisterCode;
        }

        public bool CheckOwnerRegisterCode(string code)
        {
            var ownerRegCode = UnitOfWork.OwnerRegisterCodesRepository.FindBy(c => c.OwnerRegisterCode == code)
                .FirstOrDefault();
            if (ownerRegCode != null)
            {
                UnitOfWork.OwnerRegisterCodesRepository.Delete(ownerRegCode);
                UnitOfWork.Save();
                return true;
            }

            return false;
        }
    }
}