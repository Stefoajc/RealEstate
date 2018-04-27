using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace RealEstate.Services
{
    public class BaseService
    {
        internal readonly Repositories.Interfaces.IUnitOfWork UnitOfWork;
        internal readonly IPrincipal User;
        internal readonly ApplicationUserManager UserManager;

        [Inject]
        public BaseService(RealEstate.Repositories.Interfaces.IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr)
        {
            this.UnitOfWork = unitOfWork;
            this.User = user;
            this.UserManager = userMgr;
        }
    }
}
