using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services
{
    public class LikeServices:BaseService
    {
        public LikeServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }


        public void LikeCity(int cityId)
        {
            if (!UnitOfWork.CitiesRepository.FindBy(c => c.CityId == cityId).Any())
            {
                throw new ArgumentException("City doesnt exist");
            }

            CityLikes like = new CityLikes
            {
                UserId = User.Identity.GetUserId(),
                CityId = cityId,
                LikedOn = DateTime.Now
            };

            UnitOfWork.LikesRepository.Add(like);
            UnitOfWork.Save();
        }

        public void LikeProperty(int propertyId)
        {
            if (!UnitOfWork.PropertiesRepository.FindBy(p => p.PropertyId == propertyId).Any())
            {
                throw new ArgumentException("Property doesnt exist");
            }

            PropertyLikes like = new PropertyLikes
            {
                UserId = User.Identity.GetUserId(),
                PropertyId = propertyId,
                LikedOn = DateTime.Now
            };

            UnitOfWork.LikesRepository.Add(like);
            UnitOfWork.Save();
        }

        /// <summary>
        /// Delete the like from the city for the current User
        /// </summary>
        /// <param name="cityId"></param>
        public void DisLikeCity(int cityId)
        {
            var userId = User.Identity.GetUserId();

            var likeToRemove = UnitOfWork.LikesRepository.GetAll()
                .OfType<CityLikes>().FirstOrDefault(l => l.CityId == cityId && l.UserId == userId);

            if (likeToRemove != null)
            {
                UnitOfWork.LikesRepository.Delete(likeToRemove);
                UnitOfWork.Save();
            }
        }

        /// <summary>
        /// Delete the like from the property for the current User
        /// </summary>
        /// <param name="propertyId"></param>
        public void DisLikeProperty(int propertyId)
        {
            var userId = User.Identity.GetUserId();

            var likeToRemove = UnitOfWork.LikesRepository.GetAll()
                .OfType<PropertyLikes>().FirstOrDefault(l => l.PropertyId == propertyId && l.UserId == userId);

            if (likeToRemove != null)
            {
                UnitOfWork.LikesRepository.Delete(likeToRemove);
                UnitOfWork.Save();
            }
        }
    }
}