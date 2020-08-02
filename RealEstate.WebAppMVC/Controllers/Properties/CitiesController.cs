using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ninject;
using RealEstate.Services;

namespace RealEstate.WebAppMVC.Controllers
{
    public class CitiesController : Controller
    {
        private readonly CityServices _cityManager;

        [Inject]
        public CitiesController(CityServices cityServices)
        {
            _cityManager = cityServices;
        }
        // GET: Cities/GetCitiesInCountry?countryId=<int>
        public async Task<ActionResult> GetCitiesInCountry(int countryId)
        {
            return Json(await _cityManager.GetCitiesForDropDown(countryId), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetCoords(int cityId)
        {
            return Json(await _cityManager.GetCoords(cityId), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> SeedCityCoords()
        {
            await _cityManager.SeedAllCoords();

            return Json("STATUS_OK");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteDublicates()
        {
            await _cityManager.DeleteDuplicate();
            return Json("STATUS_OK");
        }
    }
}