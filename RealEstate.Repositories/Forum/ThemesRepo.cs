using RealEstate.Data;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Forum.Interfaces;

namespace RealEstate.Repositories.Forum
{
    public class ThemesRepo : GenericRepository<RealEstateDbContext, Themes>, IThemesRepository
    {
        public ThemesRepo(RealEstateDbContext db) : base(db)
        {
        }
    }
}