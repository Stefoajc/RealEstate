using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class SearchTrackingRepository:GenericRepository<RealEstateDbContext, SearchParamsTracking>,ISearchTrackingRepository
    {
        public SearchTrackingRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}
