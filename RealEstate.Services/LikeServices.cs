using System;
using System.Linq;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Services
{
    public class LikeServices:BaseService
    {
        public LikeServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) 
            : base(unitOfWork, userMgr){}


        public void LikeCity(int cityId, string userId)
        {
            if (!IsCityExisting(cityId))
            {
                throw new ArgumentException("City doesnt exist");
            }

            CityLikes like = new CityLikes
            {
                UserId = userId,
                CityId = cityId,
                LikedOn = DateTime.Now
            };

            unitOfWork.LikesRepository.Add(like);
            unitOfWork.Save();
        }        

        public void LikeProperty(int propertyId, string userId)
        {
            if (!IsPropertyExisting(propertyId))
            {
                throw new ArgumentException("Property doesnt exist");
            }

            PropertyLikes like = new PropertyLikes
            {
                UserId = userId,
                PropertyId = propertyId,
                LikedOn = DateTime.Now
            };

            unitOfWork.LikesRepository.Add(like);
            unitOfWork.Save();
        }        

        /// <summary>
        /// Delete the like from the city for the current User
        /// </summary>
        /// <param name="cityId"></param>
        public void DislikeCity(int cityId, string userId)
        {
            var likeToRemove = unitOfWork.LikesRepository.GetAll()
                .OfType<CityLikes>()
                .FirstOrDefault(l => l.CityId == cityId && l.UserId == userId);

            if (likeToRemove != null)
            {
                unitOfWork.LikesRepository.Delete(likeToRemove);
                unitOfWork.Save();
            }
        }

        /// <summary>
        /// Delete the like from the property for the current User
        /// </summary>
        /// <param name="propertyId"></param>
        public void DislikeProperty(int propertyId, string userId)
        {
            var likeToRemove = unitOfWork.LikesRepository.GetAll()
                .OfType<PropertyLikes>()
                .FirstOrDefault(l => l.PropertyId == propertyId && l.UserId == userId);

            if (likeToRemove != null)
            {
                unitOfWork.LikesRepository.Delete(likeToRemove);
                unitOfWork.Save();
            }
        }



        #region Validations

        private bool IsCityExisting(int cityId) => unitOfWork.CitiesRepository.GetAll().Any(c => c.CityId == cityId);
        private bool IsPropertyExisting(int propertyId) => unitOfWork.PropertiesRepository.GetAll().Any(p => p.Id == propertyId);

        #endregion
    }
}