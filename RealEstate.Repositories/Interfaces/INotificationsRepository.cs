using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Model.Notifications;

namespace RealEstate.Repositories.Interfaces
{
    public interface INotificationsRepository:IGenericRepository<Notifications>
    {
        IQueryable<NotificationTypes> ListTypes();
        Task<bool> ExistType(int typeId);
    }
}