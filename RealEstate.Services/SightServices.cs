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
        public SightServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }


        public void CreateSight(SightCreateViewModel model)
        {
            Sights sight = new Sights()
            {
                SightName = model.SightName,
                SightInfo = model.SightInfo,
                CityId = model.CityId
            };

            if ((model.Latitude > -90.0D && model.Latitude < 90.0D) && // Latitude range -90 to 90
                (model.Longitude > -180.0D && model.Longitude < 180.0D)) // Longitude range -180 to 180
            {
                Coordinates sightCoords = new Coordinates()
                {
                    Latitude = model.Latitude,
                    Longtitude = model.Longitude
                };

                sight.Coordinates = sightCoords;
            }

            UnitOfWork.SightsRepository.Add(sight);
            UnitOfWork.Save();
        }


        public SightEditViewModel GetSightForEdition(int sightId)
        {
            SightEditViewModel sight = UnitOfWork.SightsRepository
                .FindBy(s => s.SightId == sightId)
                .Select(s => new SightEditViewModel
                {
                    SightId = s.SightId,
                    CityId = (int)s.CityId,
                    Longitude =  s.Coordinates.Latitude,
                    Latitude = s.Coordinates.Longtitude,
                    SightInfo = s.SightInfo,
                    SightName = s.SightName,
                })
                .FirstOrDefault();

            if (sight != null)
            {
                return sight;
            }
            else
            {
                throw new ArgumentException("Sight not Found!");
            }
        }


        public void EditSight(SightEditViewModel model)
        {
            Sights sight = UnitOfWork.SightsRepository
                .FindBy(s => s.SightId == model.SightId)
                .Include(c => c.Coordinates)
                .FirstOrDefault();

            if (sight == null)
            {
                throw new ArgumentException("Sight not found!");
            }

            if ((model.Latitude > -90.0D && model.Latitude < 90.0D) && // Latitude range -90 to 90
                (model.Longitude > -180.0D && model.Longitude < 180.0D)) // Longitude range -180 to 180
            {
                if (sight.Coordinates != null) // Editing existing Coords
                {
                    sight.Coordinates.Latitude = model.Latitude;
                    sight.Coordinates.Longtitude = model.Longitude;
                }
                else // Adding coords if not set previously
                {
                    Coordinates sightCoords = new Coordinates()
                    {
                        Latitude = model.Latitude,
                        Longtitude = model.Longitude
                    };

                    sight.Coordinates = sightCoords;
                }
            }
            else // Delete existing coords
            {
                sight.Coordinates = null;
            }

            UnitOfWork.SightsRepository.Edit(sight);
            UnitOfWork.Save();
        }
    }
}