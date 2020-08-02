using System;
using System.Linq;
using Ninject;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services
{
    public class OwnerRegisterCodeServices : BaseService
    {
        [Inject]
        public OwnerRegisterCodeServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) 
            : base(unitOfWork, userMgr) {}

        public string AddOwnerRegisterCode()
        {
            var code = new OwnerRegisterCodes() { OwnerRegisterCode = Guid.NewGuid().ToString() };
            unitOfWork.OwnerRegisterCodesRepository.Add(code);
            unitOfWork.Save();

            return code.OwnerRegisterCode;
        }

        public bool CheckOwnerRegisterCode(string code)
        {
            var ownerRegCode = unitOfWork.OwnerRegisterCodesRepository
                .Where(c => c.OwnerRegisterCode == code)
                .FirstOrDefault();

            if (ownerRegCode != null)
            {
                unitOfWork.OwnerRegisterCodesRepository.Delete(ownerRegCode);
                unitOfWork.Save();
                return true;
            }

            return false;
        }
    }
}