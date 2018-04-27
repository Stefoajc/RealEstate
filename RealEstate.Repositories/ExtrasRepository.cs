using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class ExtrasRepository:GenericRepository<RealEstateDbContext,Extras>,IExtrasRepository
    {
        public ExtrasRepository(RealEstateDbContext db) : base(db)
        {
        }


    }
}