using RealEstate.Data;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Forum.Interfaces;

namespace RealEstate.Repositories.Forum
{
    public class TagsRepo : GenericRepository<RealEstateDbContext, Tags>, ITagsRepository
    {
        public TagsRepo(RealEstateDbContext db) : base(db)
        {
        }
    }
}