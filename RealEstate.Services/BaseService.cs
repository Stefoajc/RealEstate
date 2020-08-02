using Ninject;

namespace RealEstate.Services
{
    public class BaseService
    {
        protected readonly Repositories.Interfaces.IUnitOfWork unitOfWork;
        protected readonly ApplicationUserManager userManager;

        protected static readonly string AdminRole = "Administartor";
        protected static readonly string ClientRole = "Client";
        protected static readonly string AgentRole = "Agent";
        protected static readonly string MaintenanceRole = "Maintenance";
        protected static readonly string OwnerRole = "Owner";
        protected static readonly string MarketerRole = "Marketer";
        protected static readonly string TeamUserRole = "TeamMember";

        [Inject]
        public BaseService(Repositories.Interfaces.IUnitOfWork unitOfWork, ApplicationUserManager userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }
    }
}
