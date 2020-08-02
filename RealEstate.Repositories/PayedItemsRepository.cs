using RealEstate.Data;
using RealEstate.Model.Payment;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class PayedItemsRepository : GenericRepository<RealEstateDbContext, PayedItemsMeta>, IPayedItemsRepository
    {
        public PayedItemsRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}