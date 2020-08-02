using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.WebAppMVC.Controllers.Properties
{
    public class PropertySearchesController : Controller
    {
        private readonly PropertySearchServices _propertySearchServices;
        private readonly CityServices _cityManager;
        private readonly PropertiesServices _propertyManager;
        private readonly UserServices _userServices;
        private readonly ReviewServices _reviewsManager;

        public PropertySearchesController(PropertySearchServices propertySearchServices, PropertiesServices propertyManager, CityServices cityManager, UserServices userServices, ReviewServices reviewsManager)
        {
            _propertySearchServices = propertySearchServices;
            _propertyManager = propertyManager;
            _cityManager = cityManager;
            _userServices = userServices;
            _reviewsManager = reviewsManager;
        }

        // GET: PropertySearches
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Index(int? pageNumber = 0, int pageSize = 6, int? cityId = null, int? propertyTypeId = null, bool? onlyRentalSearches = null)
        {
            #region Dropdowns

            var propertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.PropertyTypes = propertyTypes;
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);

            #endregion

            #region Current Filters

            ViewBag.pageNumber = pageNumber;
            ViewBag.paegSize = pageSize;
            ViewBag.cityId = cityId;
            ViewBag.propertyTypeId = propertyTypeId;
            ViewBag.onlyRentalSearches = onlyRentalSearches;

            #endregion

            #region Paging

            var searchesCount = await _propertySearchServices.CountAsync(onlyRentalSearches);
            ViewBag.PagesCount = (int)Math.Ceiling((double)searchesCount / (double)pageSize);

            #endregion

            var propertySearches = await _propertySearchServices.ListAsync(pageSize, pageNumber, cityId, propertyTypeId);

            return View(propertySearches);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            //Sidebar
            //Clients aside
            ViewBag.HappyClient = await _reviewsManager.ListClientReviews();

            var detailedPropertySearch = await _propertySearchServices.GetDetailsAsync(id);
            return View(detailedPropertySearch);
        }

        [Authorize(Roles = "Administrator,Agent")]
        [HttpGet]
        public async Task<ActionResult> MyPropertySearches()
        {
            var userId = User.Identity.GetUserId();
            var myPropertySearches = await _propertySearchServices.ListAsync(agentId: userId);

            return View(myPropertySearches);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Agent,Maintenance")]
        public async Task<ActionResult> Create()
        {            
            ViewBag.PropertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);

            return View();
        }

        [Authorize(Roles = "Administrator,Agent,Maintenance")]
        [HttpPost]
        public async Task<ActionResult> Create(PropertySearchCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var createdPropertySearch = await _propertySearchServices.CreateAsync(model, User.Identity.GetUserId());

            return Json(createdPropertySearch);
        }

        [Authorize(Roles = "Administrator,Agent,Maintenance")]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.PropertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);

            var propertySearchToEdit = await _propertySearchServices.GetEditAsync(id);

            return View(propertySearchToEdit);
        }

        [Authorize(Roles = "Administrator,Agent,Maintenance")]
        [HttpPost]
        public async Task<ActionResult> Edit(PropertySearchEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var editedPropertySearch = await _propertySearchServices.EditAsync(model, User.Identity.GetUserId());

            return Json(editedPropertySearch);
        }

        [Authorize(Roles = "Administrator,Agent,Maintenance")]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            await _propertySearchServices.DeleteAsync(id, User.Identity.GetUserId());

            return Json("STATUS_OK");
        }
    }
}