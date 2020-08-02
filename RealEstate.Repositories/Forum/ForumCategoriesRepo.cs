using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Forum.Interfaces;

namespace RealEstate.Repositories.Forum
{
    public class ForumCategoriesRepo:GenericRepository<RealEstateDbContext,ForumCategories>,IForumCategoriesRepository
    {
        public ForumCategoriesRepo(RealEstateDbContext db) : base(db)
        {
        }
    }
}
