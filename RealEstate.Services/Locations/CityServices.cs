using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class CityServices : BaseService
    {
        //This is used to cache the cities
        private static int _lastQueriedCountryId;
        private static List<DropDownCitiesViewModel> _lastQueriedCities;

        public CityServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        { }

        //This func result is cached
        public async Task<List<DropDownCitiesViewModel>> GetCitiesForDropDown(int countryId)
        {
            if (_lastQueriedCountryId != countryId)
            {
                _lastQueriedCities = await unitOfWork.CitiesRepository
                    .Where(c => c.CountryId == countryId)
                    .Select(c => new DropDownCitiesViewModel
                    {
                        CityId = c.CityId,
                        CityName = c.CityName + (c.PostalCode != "NULL" ? "," + c.PostalCode : "")
                    })
                    .ToListAsync();

                _lastQueriedCountryId = countryId;
            }

            return _lastQueriedCities;
        }


        public List<DropDownCountriesViewModel> GetCountriesForDropDown()
        {
            return unitOfWork.CitiesRepository
                .GetAllCountries()
                .Select(c => new DropDownCountriesViewModel
                {
                    CountryId = c.CountryId,
                    CountryName = c.CountryNameBG
                }).ToList();
        }


        public async Task SeedAllCoords()
        {
            List<Cities> cities = await unitOfWork.CitiesRepository.GetAll().ToListAsync();

            foreach (var city in cities)
            {
                if (string.IsNullOrEmpty(city.Latitude) || string.IsNullOrEmpty(city.Longitude))
                {
                    try
                    {
                        var coords = await GetCoords(city.PostalCode + "+" + city.CityName + ",България");

                        city.Latitude = coords.Item1.ToString(new CultureInfo("en-US"));
                        city.Longitude = coords.Item2.ToString(new CultureInfo("en-US"));

                        unitOfWork.CitiesRepository.Edit(city);
                        await unitOfWork.SaveAsync();
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
        }

       

        public async Task<CoordinatesViewModel> GetCoords(int cityId)
        {
            Cities city = await unitOfWork.CitiesRepository.Where(c => c.CityId == cityId).FirstOrDefaultAsync() ??
                          throw new ArgumentException("City not found!");

            if (string.IsNullOrEmpty(city.Latitude) || string.IsNullOrEmpty(city.Longitude))
            {
                var coords = await GetCoords(city.CityName + "+" + city.PostalCode);

                city.Latitude = coords.Item1.ToString(new CultureInfo("en-US"));
                city.Longitude = coords.Item2.ToString(new CultureInfo("en-US"));

                unitOfWork.CitiesRepository.Edit(city);
                await unitOfWork.SaveAsync();
            }
            return new CoordinatesViewModel
            {
                Longitude = double.Parse(city.Longitude, NumberStyles.Any, new CultureInfo("bg-BG")),
                Latitude = double.Parse(city.Latitude, NumberStyles.Any, new CultureInfo("bg-BG"))
            };
        }

        private async Task<Tuple<string, string>> GetCoords(string place)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client
                    .GetAsync($"https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyAdd8_yPWxOIPQ17YLlygDndq9GTDHFbnM&address={place}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    dynamic jsonObject = serializer.Deserialize<dynamic>(result);

                    foreach (var jsonResult in jsonObject["results"])
                    {
                        if (((string)jsonResult["formatted_address"]).Contains("Bulgaria"))
                        {
                            var lat = jsonResult["geometry"]["location"]["lat"].ToString(); // result is string
                            var lng = jsonResult["geometry"]["location"]["lng"].ToString(); // result is string

                            return Tuple.Create(lat, lng);
                        }
                    }
                }

                throw new ArgumentException("Не се намери компютъра!");
            }
        }

        public async Task DeleteDuplicate()
        {
            var cities = await unitOfWork.CitiesRepository.GetAll().ToListAsync();
            var citiesToDelete = new List<Cities>();

            foreach (var city in cities)
            {
                var cityToDelete = cities.FirstOrDefault(c =>
                    c.CityId > city.CityId && c.CityName == city.CityName && c.PostalCode == city.PostalCode &&
                    c.CityCode == city.CityCode);

                if (cityToDelete != null)
                {
                    citiesToDelete.Add(cityToDelete);
                }
            }

            foreach (var cityToDelete in citiesToDelete)
            {
                unitOfWork.CitiesRepository.Delete(cityToDelete);
            }

            await unitOfWork.SaveAsync();
        }
    }
}