using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class AgentServices : BaseService
    {
        private AppointmentServices appointmentsManager;

        [Inject]
        public AgentServices(IUnitOfWork unitOfWork
            , ApplicationUserManager userManager
            , AppointmentServices appointmentsManager) 
            : base(unitOfWork, userManager)
        {
            this.appointmentsManager = appointmentsManager;
        }


        public async Task<List<UserIdNameViewModel>> GetAgentsForDropDown()
        {
            return await userManager.Users.Where(u => u is AgentUsers)
                .Select(a => new UserIdNameViewModel
                {
                    Id = a.Id,
                    Name = a.FirstName + " " + a.LastName
                })
                .ToListAsync();
        }


        #region Reviews

        public async Task<List<ReviewListViewModel>> GetReviews(string agentId, int page = 1, int pageSize = 6)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е въведен агент, за който взимате ревюта!");
            }

            var reviewsQuery = unitOfWork.ReviewsRepository.GetAll()
                .OfType<AgentReviews>()
                .Where(r => r.AgentId == agentId);

            return await reviewsQuery.OrderBy(r => r.CreatedOn)
                .Paging(page, pageSize)
                .Select(r => new ReviewListViewModel
                {
                    ReviewScore = r.ReviewScore,
                    ReviewText = r.ReviewText,
                    CreatedOn = r.CreatedOn,
                    ClientReviewer = new ClientReviewListViewModel
                    {
                        Name = r.User.FirstName != null || r.User.LastName != null ? r.User.FirstName + " " + r.User.LastName : r.User.UserName,
                        ImagePath = r.User.Images.Select(i => i.ImagePath).FirstOrDefault() ?? "/Resources/no-image-person.png"
                    }
                })
                .ToListAsync();
        }


        public async Task CreateReview(AgentReviewCreateViewModel review, string reviewerId)
        {
            if (string.IsNullOrEmpty(reviewerId))
            {
                throw new ArgumentException("Не е въведен потребител, който дава ревюто!");
            }
            if (await userManager.Users.AnyAsync(u => u.Id == reviewerId))
            {
                throw new ContentNotFoundException("Не е намерен потребителят, който дава ревюто!");
            }

            if (string.IsNullOrEmpty(review.AgentId))
            {
                throw new ArgumentException("Не е въведен агентът, на който давате ревюто!");
            }


            var agent = await userManager.Users
                .OfType<AgentUsers>()
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(u => u.Id == review.AgentId) 
                ?? throw new ContentNotFoundException("Не е намерен агентът, на който давате ревюто!");

            var reviewToEdit = agent.Reviews.FirstOrDefault(r => r.UserId == reviewerId);
            if (reviewToEdit == null)
            {
                if (!await userManager.IsInRoleAsync(reviewerId, ClientRole))
                {
                    throw new NotAuthorizedException("Само клиентите могат да дават оценка!");
                }
                if (!await appointmentsManager.HasPassedApprovedAppointmentWithAgent(reviewerId, agent.Id))
                {
                    throw new NotAuthorizedException("Само клиентите който имат записана среща, която е минала имат правото да дават оценка!");
                }

                reviewToEdit = new AgentReviews
                {
                    AgentId = review.AgentId,
                    ReviewScore = review.ReviewScore,
                    ReviewText = review.ReviewText,
                    UserId = reviewerId
                };

                unitOfWork.ReviewsRepository.Add(reviewToEdit);
            }
            else
            {
                reviewToEdit.ReviewScore = review.ReviewScore;
                reviewToEdit.ReviewText = review.ReviewText;

                unitOfWork.ReviewsRepository.Edit(reviewToEdit);
            }

            await unitOfWork.SaveAsync();
        }

        public async Task DeleteReview(int id, string ownerId)
        {
            if (string.IsNullOrEmpty(ownerId))
            {
                throw new ArgumentException("Не е въведен потребител, изтриващ ревюто!");
            }

            var agentReview = await unitOfWork.ReviewsRepository
                .Where(r => r.ReviewId == id)
                .FirstOrDefaultAsync() 
                ?? throw new ContentNotFoundException("Не е намерено ревюто, което искате бъде изтрито!");

            if (agentReview.UserId == ownerId && await userManager.IsInRoleAsync(ownerId, "Administrator"))
            {
                throw new NotAuthorizedException("Нямате право да изтриете ревюто!");
            }

            unitOfWork.ReviewsRepository.Delete(agentReview);
            await unitOfWork.SaveAsync();
        }

        #endregion




    }
}
