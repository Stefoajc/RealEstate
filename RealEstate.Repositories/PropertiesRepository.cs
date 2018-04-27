using System.Collections.Generic;
using System.Linq;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class PropertiesRepository:GenericRepository<RealEstateDbContext,Properties>,IPropertiesRepository
    {
        public PropertiesRepository(RealEstateDbContext db) : base(db)
        {
        }

        public List<PropertyTypes> ListPropertyTypes()
        {
            return Context.PropertyTypes.ToList();
        }

        public List<PropertySeason> ListPropertySeasons()
        {
            return Context.PropertyRentPeriods.ToList();
        }
    }
}