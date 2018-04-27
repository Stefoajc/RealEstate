using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class AddressesRepository:GenericRepository<RealEstateDbContext, Addresses>,IAddressesRepository
    {
        public AddressesRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}
