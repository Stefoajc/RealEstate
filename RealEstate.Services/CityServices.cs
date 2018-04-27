using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Script.Serialization;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class CityServices : BaseService
    {
        public CityServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork,
            user, userMgr)
        {
        }

        public List<DropDownCitiesViewModel> GetCitiesForDropDown(int countryId)
        {
            return UnitOfWork.CitiesRepository
                .FindBy(c => c.CountryId == countryId)
                .Select(c => new DropDownCitiesViewModel
                {
                    CityId = c.CityId,
                    CityName = c.CityName
                }).ToList();
        }


        public List<DropDownCountriesViewModel> GetCountiesForDropDown()
        {
            return UnitOfWork.CitiesRepository
                .GetAllCountries()
                .Select(c => new DropDownCountriesViewModel
                {
                    CountryId = c.CountryId,
                    CountryName = c.CountryNameBG
                }).ToList();
        }

        public CoordinatesViewModel GetCoords(int cityId)
        {
            Cities city = UnitOfWork.CitiesRepository.FindBy(c => c.CityId == cityId).FirstOrDefault() ??
                          throw new ArgumentException("City not found!");


            if (string.IsNullOrEmpty(city.Latitude) || string.IsNullOrEmpty(city.Longitude))
            {
                using (HttpClient client = new HttpClient())
                {
                    var response =
                        client.GetAsync(
                                "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCRNibRrD3DnsD2EoqbagZhLwnf82GxraU&address=" +
                                city.CityName)
                            .Result;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        dynamic jsonObject = serializer.Deserialize<dynamic>(result);

                        dynamic
                            x = jsonObject["results"][0]["geometry"][
                                "location"]; // result is Dictionary<string,object> user with fields name, teamname, email and players with their values
                        var lat = jsonObject["results"][0]["geometry"]["location"]["lat"]; // result is asdf
                        var lng = jsonObject["results"][0]["geometry"]["location"][
                            "lng"]; // result is object[] players with its values

                        city.Latitude = lat.ToString(new CultureInfo("en-US"));
                        city.Longitude = lng.ToString(new CultureInfo("en-US"));

                        UnitOfWork.CitiesRepository.Edit(city);
                        UnitOfWork.Save();

                    }
                }
            }
            return new CoordinatesViewModel
            {
                Longitude = city.Longitude,
                Latitude = city.Latitude
            };
        }
    }
}