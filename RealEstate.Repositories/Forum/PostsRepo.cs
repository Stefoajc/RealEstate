using RealEstate.Data;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Forum.Interfaces;

namespace RealEstate.Repositories.Forum
{
    public class PostsRepo : GenericRepository<RealEstateDbContext, Posts>, IPostsRepository
    {
        public PostsRepo(RealEstateDbContext db) : base(db)
        {
        }
    }
}