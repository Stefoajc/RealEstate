using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class PropertiesBaseRepository : GenericRepository<RealEstateDbContext, PropertiesBase>, IPropertiesBaseRepository
    {
        public PropertiesBaseRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}
