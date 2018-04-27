using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class ReviewServices : BaseService
    {
        public ReviewServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }

        /// <summary>
        /// Create Review Via Factory
        /// </summary>
        /// <param name="reviewModel"></param>
        /// <param name="reviewType">Type of review to be created</param>
        public void CreateReview(CreateReviewViewModel reviewModel, ReviewTypes reviewType)
        {
            Reviews reviewToAdd;

            switch (reviewType)
            {
                case ReviewTypes.Property:
                    reviewToAdd = new PropertyReviews
                    {
                        ReviewScore = reviewModel.ReviewScore,
                        ReviewText = reviewModel.ReviewText,
                        PropertyId = (int)reviewModel.ReviewForeignKey,
                        UserId = User.Identity.GetUserId()
                    };
                    break;
                case ReviewTypes.Sight:
                    reviewToAdd = new SightReviews()
                    {
                        ReviewScore = reviewModel.ReviewScore,
                        ReviewText = reviewModel.ReviewText,
                        SightId = (int)reviewModel.ReviewForeignKey,
                        UserId = User.Identity.GetUserId()
                    };
                    break;
                case ReviewTypes.City:
                    reviewToAdd = new CityReviews()
                    {
                        ReviewScore = reviewModel.ReviewScore,
                        ReviewText = reviewModel.ReviewText,
                        CityId = (int)reviewModel.ReviewForeignKey,
                        UserId = User.Identity.GetUserId()
                    };
                    break;
                case ReviewTypes.Owner:
                    reviewToAdd = new OwnerReviews()
                    {
                        ReviewScore = reviewModel.ReviewScore,
                        ReviewText = reviewModel.ReviewText,
                        OwnerId = (string)reviewModel.ReviewForeignKey,
                        UserId = User.Identity.GetUserId()
                    };
                    break;
                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }

            UnitOfWork.ReviewsRepository.Add(reviewToAdd);
            UnitOfWork.Save();
        }

        /// <summary>
        /// Delete review by id check as well if the curr user is owner of the review
        /// </summary>
        /// <param name="id">review id</param>
        public void DeleteReview(int id)
        {
            var userId = User.Identity.GetUserId();
            var reviewToDelete = UnitOfWork.ReviewsRepository.FindBy(r => r.ReviewId == id && r.UserId == userId).FirstOrDefault();

            if (reviewToDelete == null)
            {
                throw new ArgumentException("No such review");
            }

            UnitOfWork.ReviewsRepository.Delete(reviewToDelete);
            UnitOfWork.Save();
        }


        /// <summary>
        /// Edit the Review (the base class)
        /// </summary>
        /// <param name="model"></param>
        public void EditReview(EditReviewViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var reviewToEdit = UnitOfWork.ReviewsRepository
                .FindBy(r => r.ReviewId == model.ReviewId && r.UserId == userId).FirstOrDefault();

            if (reviewToEdit == null)
            {
                throw new ArgumentException("No such review");
            }

            reviewToEdit.ReviewScore = model.ReviewScore;
            reviewToEdit.ReviewText = model.ReviewText;

            UnitOfWork.ReviewsRepository.Edit(reviewToEdit);
            UnitOfWork.Save();
        }

        /// <summary>
        /// List Property Reviews
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public List<ListReviewViewModel> ListPropertyReviews(int propertyId)
        {
            var propertyReviews = UnitOfWork.PropertiesRepository
                .Include(p => p.Reviews, p => p.Reviews.Select(r => r.User), p => p.Reviews.Select(r => r.User.Images))
                .Where(p => p.PropertyId == propertyId)
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
            var sightReviews = UnitOfWork.SightsRepository
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
            var sightReviews = UnitOfWork.CitiesRepository
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
            var ownerReviews = UnitOfWork.UsersRepository.GetAll().OfType<OwnerUsers>()
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
            var ownerReviews = UnitOfWork.UsersRepository.GetAll().OfType<AgentUsers>()
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

            return ownerReviews;
        }

        /// <summary>
        /// Get all property reviews of client
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ListUserReviewsViewModel> GetClientPropertyReviews(string userId)
        {
            var reviews = UnitOfWork.ReviewsRepository
                .GetAll()
                .OfType<PropertyReviews>()
                .Where(r => r.UserId == userId)
                .Select(r => new ListUserReviewsViewModel
                {
                    PropertyId = r.PropertyId,
                    PropertyName = r.Property.PropertyName,
                    PropertyImage = r.Property.Images.Select(i => i.ImagePath).FirstOrDefault(),
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText
                })
                .ToList();

            return reviews;
        }
    }

}