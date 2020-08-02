using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject;
using NLog;
using RealEstate.Services;
using RealEstate.Services.Forum;
using RealEstate.ViewModels.WebMVC.Forum;

namespace RealEstate.WebAppMVC.Controllers.Forum
{
    public class ForumController : Controller
    {
        private readonly ForumCategoryServices _forumCategriesManager;
        private readonly PostServices _postsManager;
        private readonly CommentServices _commentsManager;
        private readonly TagServices _tagsManager;
        private readonly ThemeServices _themesManager;
        private readonly ApplicationUserManager _userManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();


        [Inject]
        public ForumController(ForumCategoryServices forumCategoryServices, PostServices postServices, CommentServices commentServices, TagServices tagServices, ThemeServices themeServices, ApplicationUserManager userManager)
        {
            _forumCategriesManager = forumCategoryServices;
            _postsManager = postServices;
            _commentsManager = commentServices;
            _tagsManager = tagServices;
            _themesManager = themeServices;
            _userManager = userManager;
        }

        // GET: Forum
        public async Task<ActionResult> Index(int page = 1, int pageSize = 6, int? categoryId = null, int? themeId = null, string tagName = null, string keyWord = null, string userId = null, ForumViewTypes viewType = ForumViewTypes.OneColumnWithBar)
        {
            //Persist the parameters for the pagination
            ViewBag.categoryId = categoryId;
            ViewBag.themeId = themeId;
            ViewBag.tagName = tagName;
            ViewBag.userId = userId;
            ViewBag.keyword = keyWord;
            ViewBag.viewType = viewType;
            ViewBag.Heading = 
                themeId != null 
                ? "Тема: " + (await _themesManager.Get((int)themeId)).Name 
                : categoryId != null 
                    ? "Категория: " + (await _forumCategriesManager.Get((int)categoryId)).Name 
                    : tagName != null 
                        ? "Постове с Таг: " + tagName 
                        : !string.IsNullOrEmpty(userId) 
                            ? "Постове на " + await _userManager.Users.Where(u => u.Id == userId).Select(u => u.UserName).FirstOrDefaultAsync()
                            : "Форум";
            //

            var posts = await _postsManager.List(page, pageSize, categoryId, themeId, tagName, keyWord, userId);
            //Pagination
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            var postsCount = await _postsManager.GetPostsCount();
            ViewBag.LastPage = (postsCount % pageSize) == 0 ? postsCount / pageSize : postsCount / pageSize + 1;
            //---------

            //SideBar 
            if (viewType != ForumViewTypes.TwoColumns)
            {
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
            }

            switch (viewType)
            {
                case ForumViewTypes.OneColumnWithBar:
                    return View(posts);
                case ForumViewTypes.TwoColumns:
                    return View("~/Views/Forum/IndexTwoColumns.cshtml", posts);
                case ForumViewTypes.TwoColumnsWithBar:
                    return View("~/Views/Forum/IndexTwoColumnsSideBar.cshtml", posts);
                default:
                    return View(posts);
            }
        }



        //
        //GET: Forum/GetUserPosts
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetUserPosts(string userId, int page = 1, int pageSize = 6)
        {
            var posts = await _postsManager.List(userId: userId,page: page,pageSize: pageSize);

            return Json(posts, JsonRequestBehavior.AllowGet);
        }
    }


    public enum ForumViewTypes
    {
        OneColumnWithBar,
        TwoColumns,
        TwoColumnsWithBar
    }
}