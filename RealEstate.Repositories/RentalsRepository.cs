using System.Linq;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class RentalsRepository:GenericRepository<RealEstateDbContext,RentalsInfo>,IRentalsRepository
    {
        public RentalsRepository(RealEstateDbContext db) : base(db)
        {
        }


        public IQueryable<UnitTypes> GetRentalTypes()
        {
            return Context.UnitTypes.AsQueryable();
        }
    }
}
