using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services.Forum;
using RealEstate.ViewModels.WebMVC.Forum;
using RealEstate.WebAppMVC.Helpers;

namespace RealEstate.WebAppMVC.Controllers.Forum
{
    [Authorize(Roles = "Administrator,Agent")]
    public class ThemesController : Controller
    {
        public readonly ThemeServices themesManager;
        public readonly ForumCategoryServices forumCategoriesManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public ThemesController(ThemeServices themeService, ForumCategoryServices forumCategoryService)
        {
            themesManager = themeService;
            forumCategoriesManager = forumCategoryService;
        }

        //
        // GET: /Themes/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.Categories = await forumCategoriesManager.ListCategoriesForLinks();
            return View();
        }

        /// <param name="model">{ Name:string, CategoryId:int}</param>
        /// <returns></returns>
        //
        // POST: Themes/Create
        [HttpPost]
        public async Task<ActionResult> Create(ThemeCreateViewModel model)
        {
            _logger.Info("Creating Theme! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Creating Theme Form Invalid! Errors: " + ModelState.ToJson());

                ViewBag.Categories = await forumCategoriesManager.ListCategoriesForLinks();
                return View(model);
            }
            if (!await forumCategoriesManager.Exists(model.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Категорията не съществува!");
                _logger.Error("Creating Theme Form Invalid! Errors: " + ModelState.ToJson());

                ViewBag.Categories = await forumCategoriesManager.ListCategoriesForLinks();
                return View(model);
            }
            try
            {
                await themesManager.Create(model, User.Identity.GetUserId());
                _logger.Info("Creating Theme Successfully!");

                return RedirectToAction("Index", "Forum");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Creating Theme Failed!");
                throw;
            }

        }

        /// <param name="model">{ ThemeId:int, Name:string, CategoryId:int}</param>
        /// <returns></returns>
        //
        // POST: Themes/Edit
        [HttpPost]
        public async Task<ActionResult> Edit(ThemeEditViewModel model)
        {
            _logger.Info("Editing Theme! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Editing Theme Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }
            if (!await forumCategoriesManager.Exists(model.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Категорията не съществува!");
                _logger.Error("Editing Theme Form Invalid! Errors: " + ModelState.ToJson());

                return Json(ModelState.ToDictionary());
            }

            try
            {
                await themesManager.Edit(model, User.Identity.GetUserId());
                _logger.Info("Editing Theme Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Editing Theme Failed!");
                throw;
            }
        }

        //
        // POST: Themes/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {
            _logger.Info("Deleting Theme With Id: " + id);

            if (id == null)
            {
                _logger.Warn("Theme To Delete Not Found!");
                return HttpNotFound();
            }

            try
            {
                await themesManager.Delete((int)id, User.Identity.GetUserId());
                _logger.Info("Deleting Theme Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Deleting Theme Failed!");
                throw;
            }
        }
    }
}