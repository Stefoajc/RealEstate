using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Model;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC.Forum;

namespace RealEstate.Services.Forum
{
    public class ThemeServices : BaseService
    {
        public ThemeServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        {
        }

        [Inject]
        public ForumCategoryServices ForumCategoriesManager { get; set; }

        public async Task<List<ThemeLinkViewModel>> ListThemesForLinks()
        {
            var themes = await unitOfWork.ThemesRepository.GetAll()
                .OrderByDescending(t => t.Posts.Count)
                .Select(t => new ThemeLinkViewModel
                {
                    ThemeId = t.ThemeId,
                    Name = t.Name,
                    PostCount = t.Posts.Count
                })
                .ToListAsync();

            return themes;
        }

        public async Task<ThemeDetailsViewModel> Get(int id)
        {
            if (!await Exists(id))
            {
                throw new ContentNotFoundException("Не е намерена категорията!");
            }

            Themes theme = await unitOfWork.ThemesRepository
                .Include(t => t.Category)
                .Where(t => t.ThemeId == id)
                .FirstOrDefaultAsync();

            return Mapper.Map<ThemeDetailsViewModel>(theme);
        }

        public async Task<List<ThemeDetailsViewModel>> GetByCategory(int categoryId)
        {
            if (!await ForumCategoriesManager.Exists(categoryId))
            {
                throw new ContentNotFoundException("Не е намерена категорията!");
            }

            List<Themes> themes = await unitOfWork.ThemesRepository
                .Include(t => t.Category)
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync();

            return themes.Select(Mapper.Map<ThemeDetailsViewModel>).ToList();
        }

        public async Task<List<ThemeDetailsViewModel>> ListPopular(int count = 20)
        {
            var themes = await unitOfWork.ThemesRepository.GetAll()
                .Include(t => t.Category)
                .OrderByDescending(t => t.Posts.Count)
                .Take(count)
                .ToListAsync();

            return themes.Select(Mapper.Map<ThemeDetailsViewModel>).ToList();
        }

        public async Task<List<ThemeDetailsViewModel>> List()
        {
            var themes = await unitOfWork.ThemesRepository.GetAll().Include(t => t.Category).ToListAsync();

            return themes.Select(Mapper.Map<ThemeDetailsViewModel>).ToList();
        }

        public async Task<bool> Exists(int id)
        {
            return await unitOfWork.ThemesRepository.GetAll().AnyAsync(t => t.ThemeId == id);
        }

        public async Task<ThemeDetailsViewModel> Create(ThemeCreateViewModel model, string agentId)
        {
            if (!(await userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                  || await userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))))
            {
                throw new NotAuthorizedUserException("Не сте оторизирани да изпълнявате тези действия");
            }

            var category = await unitOfWork.ForumCategoriesRepository.Where(fc => fc.ForumCategoryId == model.CategoryId).FirstOrDefaultAsync();
            if (category == null)
            {
                throw new ContentNotFoundException("Не е намерена категорията!");
            }
            if(!category.IsOpenForThemes)
            {
                throw new ArgumentException("Категорията не е отворена за добавяне на теми!");
            }

            var theme = Mapper.Map<Themes>(model, opt => opt.Items.Add("UserId", agentId));
            unitOfWork.ThemesRepository.Add(theme);
            await unitOfWork.SaveAsync();

            return Mapper.Map<ThemeDetailsViewModel>(theme);
        }

        public async Task<ThemeDetailsViewModel> Edit(ThemeEditViewModel model, string userId)
        {

            if (!await userManager.IsInRoleAsync(userId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Не сте оторизирани да изпълнявате тези действия");
            }
            if (!await Exists(model.ThemeId))
            {
                throw new ArgumentException("Не е намерена темата която искате да редактирате.");
            }
            var category = await unitOfWork.ForumCategoriesRepository.Where(fc => fc.ForumCategoryId == model.CategoryId).FirstOrDefaultAsync();
            if (category == null)
            {
                throw new ContentNotFoundException("Не е намерена категорията!");
            }
            if (!category.IsOpenForThemes)
            {
                throw new ArgumentException("Категорията не е отворена за добавяне на теми!");
            }

            var themeToEdit = await unitOfWork.ThemesRepository.Include(t => t.Category).Where(t => t.ThemeId == model.ThemeId).FirstOrDefaultAsync();
            Mapper.Map(model, themeToEdit);
            unitOfWork.ThemesRepository.Edit(themeToEdit);
            await unitOfWork.SaveAsync();

            return Mapper.Map<ThemeDetailsViewModel>(themeToEdit);
        }

        public async Task Delete(int id, string userId)
        {
            if (!await userManager.IsInRoleAsync(userId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Не сте оторизирани да изпълнявате тези действия");
            }
            var themeToDelete = await unitOfWork.ThemesRepository.Where(t => t.ThemeId == id).FirstOrDefaultAsync();
            unitOfWork.ThemesRepository.Delete(themeToDelete);
            await unitOfWork.SaveAsync();
        }


    }
}