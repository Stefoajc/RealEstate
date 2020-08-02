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


        public IQueryable<PropertyTypes> GetRentalTypes()
        {
            return Context.PropertyTypes.Where(p => p.IsPropertyOnly == false).AsQueryable();
        }

        public IQueryable<RentalHirePeriodsTypes> GetRentalPeriodTypes()
        {
            return Context.RentalHirePeriodsTypes.AsQueryable();
        }
    }
}
