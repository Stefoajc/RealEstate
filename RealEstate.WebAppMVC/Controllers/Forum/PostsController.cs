using System;
using System.Collections.Generic;
using System.Linq;
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

    public class PostsController : Controller
    {
        private readonly ForumCategoryServices _forumCategriesManager;
        private readonly PostServices _postsManager;
        private readonly CommentServices _commentsManager;
        private readonly TagServices _tagsManager;
        private readonly ThemeServices _themesManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public PostsController(ForumCategoryServices forumCategoryServices, PostServices postServices, CommentServices commentServices, TagServices tagServices, ThemeServices themeServices)
        {
            _forumCategriesManager = forumCategoryServices;
            _postsManager = postServices;
            _commentsManager = commentServices;
            _tagsManager = tagServices;
            _themesManager = themeServices;
        }

        //
        // GET: Posts/Details/id
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.ForumCategories = await _forumCategriesManager.ListCategoriesForLinks();
            ViewBag.Themes = await _themesManager.ListThemesForLinks();
            ViewBag.PopularPosts = await _postsManager.ListMostPopular();
            ViewBag.LatestComments = await _commentsManager.ListLatest();
            ViewBag.FlickrPhotos = new ImageCollectionViewModel
            {
                CollectionName = "Flickr снимки",
                ImageUrls = new List<string>
                {
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQcu__MdyeKEmtW-Ki5Bg12YYPJXN5rxQjYewBsc46LYhR3K-Xj",
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQcu__MdyeKEmtW-Ki5Bg12YYPJXN5rxQjYewBsc46LYhR3K-Xj",
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQcu__MdyeKEmtW-Ki5Bg12YYPJXN5rxQjYewBsc46LYhR3K-Xj",
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQcu__MdyeKEmtW-Ki5Bg12YYPJXN5rxQjYewBsc46LYhR3K-Xj",
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQcu__MdyeKEmtW-Ki5Bg12YYPJXN5rxQjYewBsc46LYhR3K-Xj",
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQcu__MdyeKEmtW-Ki5Bg12YYPJXN5rxQjYewBsc46LYhR3K-Xj"
                }

            };
            ViewBag.Tags = await _tagsManager.ListPopularTagNames();
            ViewBag.RelatedPosts = await _postsManager.GetRelatedPosts((int)id, 2);
            ViewBag.HeighestScoredPosts = (await _postsManager.GetHeighestScored()).Take(3).ToList();

            await _postsManager.IncreaseViews((int) id);

            var post = await _postsManager.Get((int)id);
            return View(post);
        }

        //
        // GET: /Posts/Create
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Create(int? themeId = null)
        {
            ViewBag.Tags = await _tagsManager.ListPopularTagNames();
            ViewBag.Themes = await _themesManager.List();
            ViewBag.SelectedThemeId = themeId;
            return View();
        }

        //
        // POST: Posts/Create
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(PostCreateViewModel model)
        {
            _logger.Info("Creating Post! Params: " + model.ToJson());

            if(!ModelState.IsValid)
            {
                _logger.Error("Creating Post Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }
            if(!await _themesManager.Exists(model.ThemeId))
            {
                ModelState.AddModelError("ThemeId", "Темата не съществува!");
                _logger.Error("Creating Post Form Invalid! Errors: " + ModelState.ToJson());

                return Json(ModelState.ToDictionary());
            }

            try
            {
                var post = await _postsManager.Create(model, User.Identity.GetUserId());
                _logger.Info("Post created successfully!");

                return Json(post);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Post Creationg Failed!");
                throw;
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var postToEdit = await _postsManager.Get((int)id);

            return View(postToEdit);
        }

        //
        // POST: Posts/Edit
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit(PostEditViewModel model)
        {
            _logger.Info("Editing Post! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Editing Post Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                var post = await _postsManager.Edit(model, User.Identity.GetUserId());
                _logger.Info("Editing Post Successfully!");

                return Json(post);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Editing Post Failed!");
                throw;
            }

        }


        //
        // POST: Posts/Delete
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.Info("Deleting Post! Id: " + id);

            try
            {
                await _postsManager.Delete(id, User.Identity.GetUserId());
                _logger.Info("Deleting Post Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Deleting Post Failed!");
                throw;
            }

        }

    }
}