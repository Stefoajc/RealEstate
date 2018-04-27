using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class LikesRepository:GenericRepository<RealEstateDbContext,Likes>,ILikesRepository
    {
        public LikesRepository(RealEstateDbContext db) : base(db)
        {
        }


    }
}