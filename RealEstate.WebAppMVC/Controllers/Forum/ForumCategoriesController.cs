using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Extentions;
using RealEstate.Services.Forum;
using RealEstate.ViewModels.WebMVC.Forum;

namespace RealEstate.WebAppMVC.Controllers.Forum
{
    public class ForumCategoriesController : Controller
    {
        private readonly ForumCategoryServices forumCategriesManager;

        [Inject]
        public ForumCategoriesController(ForumCategoryServices forumCategoryServices)
        {
            forumCategriesManager = forumCategoryServices;
        }

        // GET: ForumCategories
        public ActionResult Index()
        {
            throw new NotImplementedException();
        }

        //
        // GET: ForumCategories/List
        [HttpGet]
        public async Task<ActionResult> ListNames() => Json(await forumCategriesManager.ListCategoriesForLinks());


        //
        // GET: ForumCategories/Create
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public  async Task<ActionResult> Create()
        {
            ViewBag.ParentCategory = await forumCategriesManager.ListCategoriesForLinks();
            return View();
        }

        /// <param name="model">{ Name:string, Info:string, IsOpenForThemes:bool, ForumCategoryParentId:int? }</param>
        /// <returns>{ ForumCategoryId:int, ForumCategoryParentId:int?, Name:string, Info:string, IsOpenForThemes:bool }</returns>
        //
        // POST: ForumCategories/Create
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> Create(ForumCategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentCategory = await forumCategriesManager.ListCategoriesForLinks();
                return View(model);
            }

            ForumCategoryDetailViewModel category = await forumCategriesManager.Create(model, User.Identity.GetUserId());

            return RedirectToAction("Index", "Forum");
        }


        /// <param name="model">{ Name:string, Info:string, IsOpenForThemes:bool }</param>
        /// <returns>{ ForumCategoryId:int, ForumCategoryParentId:int?, Name:string, Info:string, IsOpenForThemes:bool }</returns>
        //
        // POST: ForumCategories/Edit
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> Edit(ForumCategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            ForumCategoryDetailViewModel category = await forumCategriesManager.Edit(model, User.Identity.GetUserId());

            return Json(category);
        }

        //
        // POST: ForumCategories/Delete
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> Close(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            await forumCategriesManager.Close((int) id, User.Identity.GetUserId());

            return Json("STATUS_OK");
        }

        //
        // POST: ForumCategories/Delete
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> Open(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            await forumCategriesManager.Open((int)id, User.Identity.GetUserId());

            return Json("STATUS_OK");
        }
    }
}