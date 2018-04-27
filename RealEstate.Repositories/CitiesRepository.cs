using System.Linq;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class CitiesRepository:GenericRepository<RealEstateDbContext,Cities>,ICitiesRepository
    {
        public CitiesRepository(RealEstateDbContext db) : base(db)
        {
        }


        public IQueryable<Countries> GetAllCountries()
        {
            return Context.Countries.AsQueryable();
        }
    }
}