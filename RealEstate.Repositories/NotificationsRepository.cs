using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class NotificationsRepository:GenericRepository<RealEstateDbContext,Notifications>,INotificationsRepository
    {
        public NotificationsRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}
