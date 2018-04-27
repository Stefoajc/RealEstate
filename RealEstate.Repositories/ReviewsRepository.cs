using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class ReviewsRepository:GenericRepository<RealEstateDbContext,Reviews>,IReviewsRepository
    {
        public ReviewsRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}
