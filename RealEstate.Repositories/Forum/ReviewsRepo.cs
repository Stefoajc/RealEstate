using RealEstate.Data;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Forum.Interfaces;

namespace RealEstate.Repositories.Forum
{
    public class ReviewsRepo : GenericRepository<RealEstateDbContext, ForumReviews>, IReviewsRepository
    {
        public ReviewsRepo(RealEstateDbContext db) : base(db)
        {
        }
    }
}