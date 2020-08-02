using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Ninject;
using RealEstate.Model;
using RealEstate.Model.Forum;
using RealEstate.Model.Notifications;
using RealEstate.Repositories;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.Services.Helpers;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.Forum;

namespace RealEstate.Services.Forum
{
    public class PostServices : BaseService
    {
        [Inject]
        public ThemeServices ThemesManager { get; set; }
        private readonly INotificationCreator _notificationCreator;

        public PostServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr, INotificationCreator notificationCreator) : base(unitOfWork, userMgr)
        {
            _notificationCreator = notificationCreator;
        }

        public async Task<List<PostSideViewModel>> ListMostPopular()
        {
            var posts = await unitOfWork.PostsRepository
                .GetAll()
                .Include(p => p.PostCreator)
                .Include(p => p.Images)
                .OrderByDescending(p => p.Views)
                .Take(5)
                .ToListAsync();

            return posts.Select(p => Mapper.Map<PostSideViewModel>(p))
            .ToList();
        }
        

        public async Task<List<PostForumIndexViewModel>> List(int page = 1, int pageSize = 6, int? categoryId = null, int? themeId = null, string tagName = null, string keyword = null, string userId = null)
        {
            var posts = unitOfWork.PostsRepository.GetAll()
                .Include(p => p.PostCreator)
                .Include(p => p.Images);

            if (categoryId != null)
            {
                posts = posts.Where(p => p.Theme.CategoryId == categoryId);
            }
            if (themeId != null)
            {
                posts = posts.Where(p => p.ThemeId == themeId);
            }
            if (!string.IsNullOrEmpty(tagName))
            {
                posts = posts.Where(p => p.Tags.Any(t => t.Name == tagName));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                posts = posts.Where(p => p.Title.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(userId))
            {
                posts = posts.Where(p => p.CreatorId == userId);
            }


            return (await posts.OrderByDescending(p => p.CreatedOn)
                .Paging(page, pageSize)
                .ToListAsync())
                .Select(Mapper.Map<PostForumIndexViewModel>)
                .ToList();
        }

        public async Task<PostDetailViewModel> Get(int id)
        {
            Posts post = await unitOfWork.PostsRepository.Where(p => p.PostId == id)
                .Include(p => p.PostCreator)
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .Include(p => p.PostReviews)
                .FirstOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("Постът не е намерен");
            }

            return Mapper.Map<PostDetailViewModel>(post);
        }

        /// <summary>
        /// Get most related to the post posts
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postsCount"></param>
        /// <returns></returns>
        public async Task<List<PostDetailViewModel>> GetRelatedPosts(int id, int postsCount = int.MaxValue)
        {
            Posts post = await unitOfWork.PostsRepository
                .Where(p => p.PostId == id)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException();

            List<Posts> relatedPosts = await unitOfWork.PostsRepository
                .Include(p => p.PostCreator)
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .Include(p => p.PostReviews)
                .Include(p => p.Tags)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();

            if (relatedPosts.Count > postsCount)
            {
                //Filter by creator
                var resultPosts = relatedPosts.Where(p => p.CreatorId == post.CreatorId).ToList();
                if (resultPosts.Count > postsCount)
                {
                    relatedPosts = resultPosts;
                    //Filter by theme
                    resultPosts = resultPosts.Where(p => p.ThemeId == post.ThemeId).ToList();
                    if (resultPosts.Count > postsCount)
                    {
                        relatedPosts = resultPosts;
                        //Filter by tag
                        resultPosts = resultPosts.Where(p => p.Tags.Any(t => post.Tags.Any(pt => pt.TagId == t.TagId))).ToList();
                        if (resultPosts.Count > postsCount)
                        {
                            relatedPosts = resultPosts;
                        }
                    }
                }
            }


            return relatedPosts
                .Select(Mapper.Map<PostDetailViewModel>)
                .Take(postsCount)
                .ToList();
        }

        public async Task<List<PostDetailViewModel>> GetHeighestScored()
        {
            List<Posts> relatedPosts = await unitOfWork.PostsRepository
                .GetAll()
                .OrderBy(p => p.PostReviews.Sum(r => r.Score))
                .ToListAsync();

            return relatedPosts
                .Select(Mapper.Map<PostDetailViewModel>)
                .ToList();
        }


        public async Task<int> GetPostsCount()
        {
            return await unitOfWork.PostsRepository.GetAll().CountAsync();
        }

        public async Task<List<PostDetailViewModel>> GetByTheme(int themeId)
        {
            var posts = await unitOfWork.PostsRepository
                .Where(p => p.ThemeId == themeId)
                .Include(p => p.PostCreator)
                .Include(p => p.Images)
                .ToListAsync();

            return posts.Select(p => Mapper.Map<PostDetailViewModel>(p)).ToList();
        }



        /// <summary>
        /// Get posts for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<PostDetailViewModel>> UserPosts(string userId)
        {
            if (!await userManager.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ArgumentException("Потребителят не е намерен!");
            }

            var userPosts = await unitOfWork.PostsRepository
                .Where(c => c.CreatorId == userId)
                .Include(p => p.PostCreator)
                .Include(p => p.Images)
                .ToListAsync();

            return userPosts.Select(c => Mapper.Map<PostDetailViewModel>(c)).ToList();
        }

        /// <summary>
        /// Create post
        /// </summary>
        /// <param name="model"></param>
        /// <param name="postCreatorId"></param>
        /// <returns></returns>
        public async Task<PostDetailViewModel> Create(PostCreateViewModel model, string postCreatorId)
        {
            //Does the user exist in the db
            if (!await userManager.Users.AnyAsync(u => u.Id == postCreatorId))
            {
                throw new ArgumentException("Не е намерен потребителят с който сте логнати!");
            }

            if (!await ThemesManager.Exists(model.ThemeId))
            {
                throw new ArgumentException("Не е намерена темата в която искате да създадете пост!");
            }

            var postToCreate = Mapper.Map<Posts>(model, opts => opts.Items.Add("UserId", postCreatorId));

            postToCreate.Tags = new HashSet<Tags>(model.Tags.Select(t => new Tags { Name = t }).ToList());

            unitOfWork.PostsRepository.Add(postToCreate);
            await unitOfWork.SaveAsync();

            var postRelativeDirPath = Path.Combine(ConfigurationManager.AppSettings["BlogPostFolderRelativePath"], postToCreate.PostId.ToString());
            var postPhysicalDirPath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), postRelativeDirPath.TrimStart('\\'));
            Directory.CreateDirectory(postPhysicalDirPath);
            try
            {
                foreach (HttpPostedFileBase image in model.ImageFiles)
                {
                    if (image == null) continue;

                    var imagePath = Path.Combine(postPhysicalDirPath, image.FileName);
                    ImageHelpers.SaveImage(image, imagePath);
                    ImageHelpers.SaveAsWebP(image, imagePath);

                    var imageRelPath = Path.Combine(postRelativeDirPath, image.FileName);
                    postToCreate.Images.Add(new PostImages { ImagePath = imageRelPath });
                }

                //This edit is cuz, we first add the property and use its id for the images Directory
                unitOfWork.PostsRepository.Edit(postToCreate);
                await unitOfWork.SaveAsync();

                var post = Mapper.Map<PostDetailViewModel>(postToCreate);

                #region Notifications

                var notificationToCreate = new NotificationCreateViewModel
                {
                    NotificationTypeId = (int)NotificationType.Post,
                    NotificationPicture = post.ImageUrls.Any() ? post.ImageUrls[0] : "",
                    NotificationLink = "/posts/details?id=" + post.PostId,
                    NotificationText = post.AuthorName + " публикува пост: " + post.Title
                };

                await _notificationCreator.CreateGlobalNotification(notificationToCreate, postCreatorId);

                #endregion

                return post;
            }
            catch (Exception e)
            {
                //if something wrong delete the post from db and filesystem
                unitOfWork.PostsRepository.Delete(postToCreate);
                await unitOfWork.SaveAsync();
                Directory.Delete(postPhysicalDirPath, true);
                throw;
            }
        }

        public async Task<PostDetailViewModel> Edit(PostEditViewModel model, string postEditorId)
        {
            //Does the user exists in the Db
            if (!await userManager.Users.AnyAsync(u => u.Id == postEditorId))
            {
                throw new ArgumentException("Не е намерен потребителят с който сте логнати!");
            }

            var postToEdit = await unitOfWork.PostsRepository.GetAll()
                .Where(p => p.PostId == model.PostId)
                .FirstOrDefaultAsync() ?? throw new ArgumentException("Постът, който искате да редактирате не съществува.");

            //User is not owning the comment neither is Administrator
            if (postToEdit.CreatorId != postEditorId && !await userManager.IsInRoleAsync(postEditorId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Постът, който искате да редактирате не е създаден от вас!");
            }

            Mapper.Map(model, postToEdit);
            unitOfWork.PostsRepository.Edit(postToEdit);
            await unitOfWork.SaveAsync();

            return Mapper.Map<PostDetailViewModel>(postToEdit);
        }

        public async Task IncreaseViews(int id)
        {
            var postToEdit = await unitOfWork.PostsRepository
                .GetAll()
                .Where(p => p.PostId == id)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Постът, който искате да редактирате не съществува.");


            postToEdit.Views++;
            unitOfWork.PostsRepository.Edit(postToEdit);
            await unitOfWork.SaveAsync();
        }

        public async Task Delete(int id, string deletingUserId)
        {
            //Does the user exists in the Db
            if (!await userManager.Users.AnyAsync(u => u.Id == deletingUserId))
            {
                throw new ArgumentException("Не е намерен потребителят с който сте логнати!");
            }
            var postToDelete = await unitOfWork.PostsRepository.GetAll().Where(p => p.PostId == id).FirstOrDefaultAsync();
            if (postToDelete == null)
            {
                throw new ArgumentException("Постът, който искате да изтриете не съществува.");
            }
            //User is not owning the comment neither is Administrator
            if (postToDelete.CreatorId != deletingUserId || !await userManager.IsInRoleAsync(deletingUserId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Постът, който искате да изтриете не е създаден от вас!");
            }

            unitOfWork.PostsRepository.Delete(postToDelete);
            await unitOfWork.SaveAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await unitOfWork.PostsRepository.GetAll().AnyAsync(p => p.PostId == id);
        }

    }
}