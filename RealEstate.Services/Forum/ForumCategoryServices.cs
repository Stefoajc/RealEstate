using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC.Forum;

namespace RealEstate.Services.Forum
{
    public class ForumCategoryServices : BaseService
    {
        public ForumCategoryServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr) {}

        public async Task<List<ForumCategoriesLinkViewModel>> ListCategoriesForLinks()
        {
            var forumCategories = await unitOfWork.ForumCategoriesRepository.GetAll()
                .Select(c =>new ForumCategoriesLinkViewModel
                {
                    ForumCategoryId = c.ForumCategoryId,
                    Name = c.Name,
                    PostCount = c.Themes.SelectMany(t => t.Posts).Count()
                })
                .ToListAsync();

            return forumCategories;
        }

        public async Task<List<ForumCategoryDetailViewModel>> List()
        {
            List<ForumCategories> forumCategories = await unitOfWork.ForumCategoriesRepository.GetAll().ToListAsync();

            return forumCategories.Select(Mapper.Map<ForumCategoryDetailViewModel>).ToList();
        }

        public async Task<List<ForumCategoryDetailViewModel>> GetSubCategories(int parentCategoryId)
        {
            if(!await Exists(parentCategoryId))
            {
                throw new ContentNotFoundException("Не е намерена категорията!");
            }

            var forumCategories = await unitOfWork.ForumCategoriesRepository
                .Where(fc => fc.ForumCategoryParentId == parentCategoryId)
                .ToListAsync();

            return forumCategories.Select(Mapper.Map<ForumCategoryDetailViewModel>).ToList();
        }

        public async Task<ForumCategoryDetailViewModel> Get(int categoryId)
        {
            var forumCategory = await unitOfWork.ForumCategoriesRepository
                .Where(fc => fc.ForumCategoryId == categoryId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена категорията!");


            return Mapper.Map<ForumCategoryDetailViewModel>(forumCategory);
        }

        public async Task<bool> Exists(int categoryId)
        {
            return await unitOfWork.ForumCategoriesRepository.GetAll().AnyAsync(fc => fc.ForumCategoryId == categoryId);
        }

        public async Task<ForumCategoryDetailViewModel> Create(ForumCategoryCreateViewModel model, string creatorId)
        {
            if (!await userManager.IsInRoleAsync(creatorId, "Administrator"))
            {
                throw new NotAuthorizedUserException();
            }

            if (model.ForumCategoryParentId != null)
            {
                if (!await Exists((int)model.ForumCategoryParentId))
                {
                    throw new ContentNotFoundException("Не е намерена категорията!");
                }
            }

            var forumCategory = Mapper.Map<ForumCategories>(model, opts => opts.Items["CreatorId"] = creatorId);

            unitOfWork.ForumCategoriesRepository.Add(forumCategory);
            await unitOfWork.SaveAsync();

            return Mapper.Map<ForumCategoryDetailViewModel>(forumCategory);
        }

        public async Task<ForumCategoryDetailViewModel> Edit(ForumCategoryEditViewModel model, string editorId)
        {
            if (!await userManager.IsInRoleAsync(editorId, "Administrator"))
            {
                throw new NotAuthorizedUserException();
            }

            var forumCategoryToEdit = await unitOfWork.ForumCategoriesRepository
                .Where(fc => fc.ForumCategoryId == model.ForumCategoryId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена категорията");

            Mapper.Map(model, forumCategoryToEdit);

            unitOfWork.ForumCategoriesRepository.Edit(forumCategoryToEdit);
            await unitOfWork.SaveAsync();

            return Mapper.Map<ForumCategoryDetailViewModel>(forumCategoryToEdit);
        }

        public async Task Close(int categoryId, string closerId)
        {
            await SetIsClosed(categoryId, closerId, true);
        }

        public async Task Open(int categoryId, string openerId)
        {
            await SetIsClosed(categoryId, openerId, false);
        }

        private async Task SetIsClosed(int categoryId, string closerId, bool isClosed)
        {
            if (!await userManager.IsInRoleAsync(closerId, "Administrator"))
            {
                throw new NotAuthorizedUserException();
            }

            var forumCategoryToDelete = await unitOfWork.ForumCategoriesRepository
                .Where(fc => fc.ForumCategoryId == categoryId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена категорията");

            forumCategoryToDelete.IsClosed = isClosed;

            unitOfWork.ForumCategoriesRepository.Edit(forumCategoryToDelete);
            await unitOfWork.SaveAsync();
        }


    }
}