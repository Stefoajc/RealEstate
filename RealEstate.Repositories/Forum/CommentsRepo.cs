using RealEstate.Data;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Forum.Interfaces;

namespace RealEstate.Repositories.Forum
{
    public class CommentsRepo : GenericRepository<RealEstateDbContext, Comments>, ICommentsRepository
    {
        public CommentsRepo(RealEstateDbContext db) : base(db)
        {
        }
    }
}