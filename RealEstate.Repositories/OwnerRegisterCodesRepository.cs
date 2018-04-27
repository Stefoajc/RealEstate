using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class OwnerRegisterCodesRepository:GenericRepository<RealEstateDbContext,OwnerRegisterCodes>,IOwnerRegisterCodesRepository
    {
        public OwnerRegisterCodesRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}