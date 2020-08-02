using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services.Forum
{
    public class TagServices : BaseService
    {
        public TagServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        {
        }

        public async Task<List<string>> ListPopularTagNames(int? count = 20)
        {
            IQueryable<Tags> popularTags = unitOfWork.TagsRepository.GetAll()
                .OrderByDescending(t => t.Posts.Count);

            if (count != null)
            {
                popularTags = popularTags.Take((int)count);
            }
                
            return await popularTags
                .Select(t => t.Name)
                .ToListAsync(); ;
        }

        public async Task<List<string>> List(Expression<Func<Tags,bool>> filter = null)
        {
            var allTags = unitOfWork.TagsRepository.GetAll();

            if (filter != null)
            {
                allTags = allTags.Where(filter);
            }

            return await allTags.Select(t => t.Name).ToListAsync();
        }

        public async Task Create(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                throw  new ArgumentException("Tag name should not be empty!");

            Tags tagToCreate = new Tags { Name = tag };

            unitOfWork.TagsRepository.Add(tagToCreate);
            await unitOfWork.SaveAsync();
        }

        public async Task Delete(string tag)
        {
            Tags tagToRemove = await unitOfWork.TagsRepository.GetAll()
                .FirstOrDefaultAsync(t => t.Name == tag);

            unitOfWork.TagsRepository.Delete(tagToRemove);
            await unitOfWork.SaveAsync();
        }
    }
}
