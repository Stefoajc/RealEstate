using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Ninject;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC.Forum;

namespace RealEstate.Services.Forum
{
    public class ReviewServices : BaseService
    {
        [Inject]
        public PostServices PostsManager { get; set; }
        [Inject]
        public CommentServices CommentsManager { get; set; }

        public ReviewServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr) {}


        public async Task<ForumReviews> Create(ForumReviewCreateViewModel model, ReviewType reviewType, string reviewingUserId)
        {
            if(!await userManager.Users.AnyAsync(u => u.Id == reviewingUserId))
            {
                throw new ArgumentException("Не е намерен потребителят с който сте логнати!");
            }

            ForumReviews reviewToBeCreated;
            //review created by this user for this item which has to be deleted
            ForumReviews reviewAlreadyCreatedByThisUser;
            switch (reviewType)
            {
                case ReviewType.Post:
                    if(!await PostsManager.Exists(model.ReviewedItemId))
                    {
                        throw new ArgumentException("Постът, на който искате да направите ревю не съществува!");
                    }

                    reviewAlreadyCreatedByThisUser = await unitOfWork.ForumReviewsRepository.GetAll()
                        .OfType<PostReviews>()
                        .Where(r => r.UserId == reviewingUserId && r.PostId == model.ReviewedItemId)
                        .FirstOrDefaultAsync();

                    reviewToBeCreated  = Mapper.Map<PostReviews>(model, opts => opts.Items.Add("UserId", reviewingUserId));
                    break;
                case ReviewType.Comment:
                    if (!await CommentsManager.Exists(model.ReviewedItemId))
                    {
                        throw new ArgumentException("Коментарът, на който искате да направите ревю не съществува!");
                    }
                    reviewAlreadyCreatedByThisUser = await unitOfWork.ForumReviewsRepository.GetAll()
                        .OfType<PostReviews>()
                        .Where(r => r.UserId == reviewingUserId && r.PostId == model.ReviewedItemId)
                        .FirstOrDefaultAsync();

                    reviewToBeCreated = Mapper.Map<CommentReviews>(model, opts => opts.Items.Add("UserId", reviewingUserId));
                    break;
                default:
                    throw new ArgumentException("Не се поддържат Ревюта от избраният от вас тип!");
            }

            if (reviewAlreadyCreatedByThisUser != null)
            {
                unitOfWork.ForumReviewsRepository.Delete(reviewAlreadyCreatedByThisUser);
            }
            unitOfWork.ForumReviewsRepository.Add(reviewToBeCreated);
            await unitOfWork.SaveAsync();

            return reviewToBeCreated;
        }


        public async Task<ForumReviews> Edit(ForumReviewEditViewModel model, ReviewType reviewType, string editingUserId)
        {
            if (!await userManager.Users.AnyAsync(u => u.Id == editingUserId))
            {
                throw new ArgumentException("Не е намерен потребителят с който сте логнати!");
            }

            var reviewToBeEdited = await unitOfWork.ForumReviewsRepository.Where(r => r.ReviewId == model.ReviewId).FirstOrDefaultAsync() ?? throw new ArgumentException("Not Found");

            if(reviewToBeEdited.UserId != editingUserId)
            {
                throw new NotAuthorizedUserException("Ревюто което искате да редактирате не е ваше!");
            }

            reviewToBeEdited.Score = model.Score;
            reviewToBeEdited.ReviewText = model.ReviewText;

            unitOfWork.ForumReviewsRepository.Edit(reviewToBeEdited);
            await unitOfWork.SaveAsync();

            return reviewToBeEdited;

        }

        public async Task Delete(int reviewId, string deletingUserId)
        {
            if (!await userManager.Users.AnyAsync(u => u.Id == deletingUserId))
            {
                throw new ArgumentException("Не е намерен потребителят с който сте логнати!");
            }

            var reviewToBeDeleted = await unitOfWork.ForumReviewsRepository.Where(r => r.ReviewId == reviewId).FirstOrDefaultAsync() ?? throw new ArgumentException();

            if (reviewToBeDeleted.UserId != deletingUserId && !await userManager.IsInRoleAsync(deletingUserId, "Administrator"))
            {
                throw new NotAuthorizedUserException("Ревюто което искате да изтриете не е ваше!");
            }

            unitOfWork.ForumReviewsRepository.Delete(reviewToBeDeleted);
            await unitOfWork.SaveAsync();
        }


    }


    public enum ReviewType
    {
        Post,
        Comment
    }
}