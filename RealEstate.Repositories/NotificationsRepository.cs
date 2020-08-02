using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model.Notifications;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class NotificationsRepository:GenericRepository<RealEstateDbContext,Notifications>,INotificationsRepository
    {
        public NotificationsRepository(RealEstateDbContext db) : base(db)
        {
        }


        public IQueryable<NotificationTypes> ListTypes()
        {
            return Context.NotificationTypes;
        }

        public async Task<bool> ExistType(int typeId)
        {
            return await Context.NotificationTypes
                .AnyAsync(nt => nt.Id == typeId);
        }
    }
}
