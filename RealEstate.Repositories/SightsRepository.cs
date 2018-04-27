using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class SightsRepository:GenericRepository<RealEstateDbContext,Sights>,ISightsRepository
    {
        public SightsRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}