using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class SightServices : BaseService
    {
        public SightServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        {
        }


        public void CreateSight(SightCreateViewModel model)
        {
            Sights sight = new Sights
            {
                SightName = model.SightName,
                SightInfo = model.SightInfo,
                CityId = model.CityId
            };

            if (AreCoordinatesValid(model.Latitude, model.Longitude))
            {
                sight.Coordinates = new Coordinates
                {
                    Latitude = model.Latitude,
                    Longtitude = model.Longitude
                };
            }

            unitOfWork.SightsRepository.Add(sight);
            unitOfWork.Save();
        }


        public SightEditViewModel GetSightForEdition(int sightId)
        {
            SightEditViewModel sight = unitOfWork.SightsRepository.GetAll()
                .Select(s => new SightEditViewModel
                {
                    SightId = s.SightId,
                    CityId = (int)s.CityId,
                    Longitude = s.Coordinates.Latitude,
                    Latitude = s.Coordinates.Longtitude,
                    SightInfo = s.SightInfo,
                    SightName = s.SightName,
                })
                .FirstOrDefault(s => s.SightId == sightId)
                ?? throw new ArgumentException("Забележителността не е намерена!");

            return sight;
        }


        public void EditSight(SightEditViewModel model)
        {
            Sights sight = unitOfWork.SightsRepository.GetAll()
                .Include(c => c.Coordinates)
                .FirstOrDefault(s => s.SightId == model.SightId)
                ?? throw new ArgumentException("Забележителността не е намерена!");

            sight.Coordinates = AreCoordinatesValid(model.Latitude, model.Longitude)
                ? new Coordinates{ Latitude = model.Latitude, Longtitude = model.Longitude }
                : null;

            unitOfWork.SightsRepository.Edit(sight);
            unitOfWork.Save();
        }


        #region Private methods

        private static bool AreCoordinatesValid(double latitude, double longitude)
            => (latitude > -90.0D && latitude < 90.0D) && // Latitude range -90 to 90
                    (longitude > -180.0D && longitude < 180.0D); // Longitude range -180 to 180

        #endregion
    }
}