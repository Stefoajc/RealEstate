using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class ImagesRepository:GenericRepository<RealEstateDbContext,Images>,IImagesRepository
    {
        public ImagesRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}