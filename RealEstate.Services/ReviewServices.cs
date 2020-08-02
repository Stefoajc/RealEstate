using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class ReviewServices : BaseService
    {
        public ReviewServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        {
        }

        /// <summary>
        /// Create Review Via Factory
        /// </summary>
        /// <param name="reviewModel"></param>
        /// <param name="userId"></param>
        /// <param name="reviewType">Type of review to be created</param>
        public async Task CreateReview(CreateReviewViewModel reviewModel, string userId, ReviewTypes reviewType)
        {
            if (!await IsUserExisting(userId))
            {
                throw new ContentNotFoundException("Потребителят който се въвели не съществува!");
            }

            Reviews reviewToAdd = await ReviewsCreateFactory(reviewModel, userId, reviewType);

            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Delete review by id check as well if the curr user is owner of the review
        /// </summary>
        /// <param name="id">review id</param>
        /// <param name="userId"></param>
        public async Task DeleteReview(int id, string userId)
        {
            var review = await unitOfWork.ReviewsRepository.GetAll()
                .FirstOrDefaultAsync(r => r.ReviewId == id)
                ?? throw new ContentNotFoundException("Не е намерено ревюто!"); ;

            if (!await IsOwnerOrAdministratorAsync(userId, review))
            {
                throw new NotAuthorizedUserException("Не сте оторизиран да извършвате това действие");
            }

            unitOfWork.ReviewsRepository.Delete(review);
            await unitOfWork.SaveAsync();
        }




        /// <summary>
        /// Edit the Review (the base class)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        public async Task EditReview(EditReviewViewModel model, string userId)
        {
            var review = await unitOfWork.ReviewsRepository.GetAll()
                .FirstOrDefaultAsync(r => r.ReviewId == model.ReviewId)
                ?? throw new ContentNotFoundException("Не е намерено ревюто!");

            if (!await IsOwnerOrAdministratorAsync(userId, review))
            {
                throw new NotAuthorizedUserException("Не сте оторизиран да извършвате това действие");
            }

            review.ReviewScore = model.ReviewScore;
            review.ReviewText = model.ReviewText;

            unitOfWork.ReviewsRepository.Edit(review);
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// List Property Reviews
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public List<ListReviewViewModel> ListPropertyReviews(int propertyId)
        {
            var propertyReviews = unitOfWork.PropertiesRepository
                .Include(p => p.Reviews, p => p.Reviews.Select(r => r.User), p => p.Reviews.Select(r => r.User.Images))
                .Where(p => p.Id == propertyId)
                .Select(p => p.Reviews.Select(r => new ListReviewViewModel
                {
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText,
                    ReviewUserFullname = r.User.FirstName + " " + r.User.LastName,
                    UserProfileImagePath = r.User.Images.Select(i => i.ImagePath).FirstOrDefault()
                })
                .ToList())
                .FirstOrDefault();

            return propertyReviews;
        }

        public List<ListReviewViewModel> ListSightReviews(int sightId)
        {
            var sightReviews = unitOfWork.SightsRepository
                .Include(p => p.Reviews, p => p.Reviews.Select(r => r.User), p => p.Reviews.Select(r => r.User.Images))
                .Where(p => p.SightId == sightId)
                .Select(p => p.Reviews.Select(r => new ListReviewViewModel
                {
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText,
                    ReviewUserFullname = r.User.FirstName + " " + r.User.LastName,
                    UserProfileImagePath = r.User.Images.Select(i => i.ImagePath).FirstOrDefault()
                })
                .ToList())
                .FirstOrDefault();

            return sightReviews;
        }

        public List<ListReviewViewModel> ListCityReviews(int cityId)
        {
            var sightReviews = unitOfWork.CitiesRepository
                .Include(p => p.Reviews, p => p.Reviews.Select(r => r.User), p => p.Reviews.Select(r => r.User.Images))
                .Where(p => p.CityId == cityId)
                .Select(p => p.Reviews.Select(r => new ListReviewViewModel
                {
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText,
                    ReviewUserFullname = r.User.FirstName + " " + r.User.LastName,
                    UserProfileImagePath = r.User.Images.Select(i => i.ImagePath).FirstOrDefault()
                })
                .ToList())
                .FirstOrDefault();

            return sightReviews;
        }

        public List<ListReviewViewModel> ListOwnerReviews(string ownerId)
        {
            var ownerReviews = unitOfWork.UsersRepository.GetAll().OfType<OwnerUsers>()
                .Where(p => p.Id == ownerId)
                .Select(p => p.Reviews.Select(r => new ListReviewViewModel
                {
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText,
                    ReviewUserFullname = r.User.FirstName + " " + r.User.LastName,
                    UserProfileImagePath = r.User.Images.Select(i => i.ImagePath).FirstOrDefault()
                })
                .ToList())
                .FirstOrDefault();

            return ownerReviews;
        }

        public List<ListReviewViewModel> ListAgentReviews(string agentId)
        {
            var agentReviews = unitOfWork.UsersRepository.GetAll().OfType<AgentUsers>()
                .Where(p => p.Id == agentId)
                .Select(p => p.Reviews.Select(r => new ListReviewViewModel
                {
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText,
                    ReviewUserFullname = r.User.FirstName + " " + r.User.LastName,
                    UserProfileImagePath = r.User.Images.Select(i => i.ImagePath).FirstOrDefault()
                })
                .ToList())
                .FirstOrDefault();

            return agentReviews;
        }

        public async Task<List<ListReviewViewModel>> ListClientReviews()
        {
            var reviews = await unitOfWork.ReviewsRepository
                .Include(r => r.User)
                .Where(r => r is AgentReviews || r is PropertyReviews)
                .Select(r => new ListReviewViewModel
                {
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText,
                    ReviewUserFullname = r.User.FirstName + " " + r.User.LastName,
                    UserProfileImagePath = r.User.Images.Select(i => i.ImagePath).FirstOrDefault()
                })
                .ToListAsync();

            return reviews;
        }

        /// <summary>
        /// Get all property reviews of client
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ListUserReviewsViewModel> GetClientPropertyReviews(string userId)
        {
            var reviews = unitOfWork.ReviewsRepository
                .GetAll()
                .OfType<PropertyReviews>()
                .Where(r => r.UserId == userId)
                .Select(r => new ListUserReviewsViewModel
                {
                    PropertyId = r.PropertyId,
                    //PropertyName = r.Property.PropertyName,
                    //PropertyImage = r.Property.Images.Select(i => i.ImagePath).FirstOrDefault(),
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText
                })
                .ToList();

            return reviews;
        }




        #region Helpers

        /// <summary>
        /// Create Review if the user has made review to this entity it updates the review
        /// </summary>
        /// <param name="review"></param>
        /// <param name="userId"></param>
        /// <param name="reviewType"></param>
        /// <returns></returns>
        private async Task<Reviews> ReviewsCreateFactory(CreateReviewViewModel review, string userId, ReviewTypes reviewType)
        {
            Reviews reviewResult;
            bool isExisting = false;
            switch (reviewType)
            {
                case ReviewTypes.Property:
                    if (!await unitOfWork.PropertiesRepository.GetAll().AnyAsync(p => p.Id == (int)review.ReviewForeignKey))
                    {
                        throw new ContentNotFoundException("Имотът не е намерен!");
                    }
                    var propertyId = (int)review.ReviewForeignKey;
                    reviewResult = await unitOfWork.ReviewsRepository.GetAll()
                        .OfType<PropertyReviews>()
                        .FirstOrDefaultAsync(r => r.PropertyId == propertyId && r.UserId == userId);
                    if (reviewResult != null)
                    {
                        isExisting = true;
                    }
                    else
                    {
                        reviewResult = new PropertyReviews { PropertyId = propertyId };
                    }
                    break;
                case ReviewTypes.Sight:
                    if (!await unitOfWork.SightsRepository.GetAll().AnyAsync(p => p.SightId == (int)review.ReviewForeignKey))
                    {
                        throw new ContentNotFoundException("Забележителността не е намерен!");
                    }
                    var sightId = (int)review.ReviewForeignKey;
                    reviewResult = await unitOfWork.ReviewsRepository.GetAll()
                        .OfType<SightReviews>()
                        .FirstOrDefaultAsync(r => r.SightId == sightId && r.UserId == userId);
                    if (reviewResult != null)
                    {
                        isExisting = true;
                    }
                    else
                    {
                        reviewResult = new SightReviews() { ReviewId = (int)review.ReviewForeignKey };
                    }
                    break;
                case ReviewTypes.City:
                    if (!await unitOfWork.CitiesRepository.GetAll().AnyAsync(p => p.CityId == (int)review.ReviewForeignKey))
                    {
                        throw new ContentNotFoundException("Градът не е намерен!");
                    }
                    var cityId = (int)review.ReviewForeignKey;
                    reviewResult = await unitOfWork.ReviewsRepository.GetAll()
                        .OfType<CityReviews>()
                        .FirstOrDefaultAsync(r => r.CityId == cityId && r.UserId == userId);
                    if (reviewResult != null)
                    {
                        isExisting = true;
                    }
                    else
                    {
                        reviewResult = new CityReviews() { CityId = (int)review.ReviewForeignKey };
                    }
                    break;
                case ReviewTypes.Owner:
                    if (!await userManager.Users.AnyAsync(p => p.Id == (string)review.ReviewForeignKey))
                    {
                        throw new ContentNotFoundException("Собственикът не е намерен!");
                    }
                    if (!await userManager.IsInRoleAsync((string)review.ReviewForeignKey, OwnerRole))
                    {
                        throw new ContentNotFoundException("Потребителят не е собственик на имот!");
                    }
                    var ownerId = (string)review.ReviewForeignKey;
                    reviewResult = await unitOfWork.ReviewsRepository.GetAll()
                        .OfType<OwnerReviews>()
                        .FirstOrDefaultAsync(r => r.OwnerId == ownerId && r.UserId == userId);
                    if (reviewResult != null)
                    {
                        isExisting = true;
                    }
                    else
                    {
                        reviewResult = new OwnerReviews() { OwnerId = (string)review.ReviewForeignKey };
                    }
                    break;
                case ReviewTypes.Agent:
                    if (!await userManager.Users.AnyAsync(p => p.Id == (string)review.ReviewForeignKey))
                    {
                        throw new ContentNotFoundException("Собственикът не е намерен!");
                    }
                    if (!await userManager.IsInRoleAsync((string)review.ReviewForeignKey, OwnerRole))
                    {
                        throw new ContentNotFoundException("Потребителят не е собственик на имот!");
                    }
                    var agentId = (string)review.ReviewForeignKey;
                    reviewResult = await unitOfWork.ReviewsRepository.GetAll()
                        .OfType<AgentReviews>()
                        .FirstOrDefaultAsync(r => r.AgentId == agentId && r.UserId == userId);
                    if (reviewResult != null)
                    {
                        isExisting = true;
                    }
                    else
                    {
                        reviewResult = new AgentReviews() { AgentId = (string)review.ReviewForeignKey };
                    }
                    break;
                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            reviewResult.ReviewScore = review.ReviewScore;
            reviewResult.ReviewText = review.ReviewText;
            reviewResult.UserId = userId;

            if (isExisting)
            {
                unitOfWork.ReviewsRepository.Edit(reviewResult);
            }
            else
            {
                unitOfWork.ReviewsRepository.Add(reviewResult);
            }

            return reviewResult;
        }

        private Task<bool> IsUserExisting(string userId) => userManager.Users.AnyAsync(u => u.Id == userId);

        private async Task<bool> IsOwnerOrAdministratorAsync(string userId, Reviews review) => IsOwnerOfTheReview(userId, review) || await IsAdministratorAsync(userId);

        private Task<bool> IsAdministratorAsync(string userId) => userManager.IsInRoleAsync(userId, "Administrator");

        private static bool IsOwnerOfTheReview(string userId, Reviews reviewToDelete) => reviewToDelete.UserId == userId;

        #endregion
    }

}