﻿using System.Security.Principal;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class AddressServices:BaseService
    {
        public AddressServices(IUnitOfWork unitOfWork,  ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        {
        }

        public Addresses AddAddress(CreateAddressViewModel addressToAdd)
        {
            return new Addresses()
            {
                CityId = addressToAdd.CityId,
                FullAddress = addressToAdd.FullAddress,
                Coordinates = new Coordinates()
                {
                    Latitude = addressToAdd.Latitude,
                    Longtitude = addressToAdd.Longitude
                }
            };
        }

    }
}
