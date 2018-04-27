using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult GetCitiesInCountry(int countryId)
        {
            return Json(_cityManager.GetCitiesForDropDown(countryId),JsonRequestBehavior.AllowGet);
        }
    }
}