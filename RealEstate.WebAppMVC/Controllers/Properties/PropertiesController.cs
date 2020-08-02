using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC;
using RealEstate.WebAppMVC.Helpers;
using RealEstate.WebAppMVC.Helpers.DataAnnotations;

namespace RealEstate.WebAppMVC.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private const string All = "Всички";
        private readonly PropertiesServices _propertyManager;
        private readonly CityServices _cityManager;
        private readonly RentalInfoServices _rentalInfoManager;
        private readonly ExtraServices _extrasManager;
        private readonly UserServices _userServices;
        private readonly ClientServices _clientsManager;
        private readonly ReviewServices _reviewsManager;
        private readonly ImageServices _imagesManager;
        private ApplicationUserManager _userManager;
        private readonly AppointmentServices _appointmentsManager;
        private readonly SearchTrackingServices _searchTrackingManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();


        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        [Inject]
        public PropertiesController(ExtraServices extraServices, ImageServices imageServices, PropertiesServices propertyServices, ClientServices clientServices, CityServices cityServices, RentalInfoServices rentalInfoServices, UserServices userServices, AppointmentServices appointmentsManager, ReviewServices reviewsManager, SearchTrackingServices searchTrackingManager)
        {
            _propertyManager = propertyServices;
            _cityManager = cityServices;
            _rentalInfoManager = rentalInfoServices;
            _extrasManager = extraServices;
            _userServices = userServices;
            _appointmentsManager = appointmentsManager;
            _reviewsManager = reviewsManager;
            _searchTrackingManager = searchTrackingManager;
            _clientsManager = clientServices;
            _imagesManager = imageServices;
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetPropertiesDropDown(string agentId)
        {
            var properties = await _propertyManager.GetPropertiesForDropDown(agentId: agentId);

            return Json(properties, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.Authorize(Roles = "Agent,Owner")]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> MyProperties()
        {
            var userId = User.Identity.GetUserId();
            var properties = await _propertyManager.GetUserProperties(userId);

            return View(properties);
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Index(SearchAllViewModel searchParams)
        {
            //Setup Advanced Search
            var propertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.PropertyTypes = propertyTypes;
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = await _propertyManager.GetRelatedPropertiesBriefInfo(isForRent: searchParams.IsForRent, propertyType: searchParams.PropertyType, cityId: searchParams.CityId);
            //Setup Advanced Search

            //Setup filters
            ViewBag.PropertiesTitle = searchParams.PropertyType == null ? All : propertyTypes
                                                                                    .Where(pt => pt.PropertyTypeId == (int)searchParams.PropertyType)
                                                                                    .Select(pt => pt.PropertyTypeName)
                                                                                    .FirstOrDefault();
            ViewBag.SearchParams = searchParams;
            //---- Setup End -----

            //Save Search Params
            if (User.Identity.IsAuthenticated)
            {
                await _searchTrackingManager.AddSearchParameters(User.Identity.GetUserId(), searchParams);
            }
            //Save Search Params

            var properties = await _propertyManager.GetProperties(
                isForRent: searchParams.IsForRent, isShortPeriodRent: searchParams.IsShortPeriodRent,
                priceFrom: searchParams.PriceFrom, priceTo: searchParams.PriceTo,
                propertyType: searchParams.PropertyType,
                cityId: searchParams.CityId, distanceFromCity: searchParams.DistanceFromCity,
                areaFrom: searchParams.AreaFrom, areaTo: searchParams.AreaTo,
                extras: searchParams.Extras,
                pageCount: searchParams.PageCount, pageSize: searchParams.PageSize,
                sortBy: searchParams.SortBy, orderBy: searchParams.OrderBy);

            //Pagination
            var propCount = properties.Count;
            ViewBag.LastPage = (propCount % searchParams.PageSize) == 0 ? propCount / searchParams.PageSize : propCount / searchParams.PageSize + 1;
            //---------

            if (searchParams.ViewType == "ListPropertiesFullWidth")
            {
                return View($"~/Views/Properties/{searchParams.ViewType}.cshtml", properties.Properties);
            }
            else
            {
                return View("~/Views/Properties/ListProperties.cshtml", properties.Properties);
            }
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Map(SearchMapViewModel searchParams)
        {
            //Setup filters
            ViewBag.SearchParams = searchParams;

            if (searchParams.CityId != null)
            {
                ViewBag.City = await _cityManager.GetCoords((int)searchParams.CityId);
            }

            //Setup Advanced Search
            var propertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.PropertyTypes = propertyTypes;
            ViewBag.PropertiesTitle = searchParams.PropertyType == null ? All : propertyTypes
                                                                                    .Where(pt => pt.PropertyTypeId == (int)searchParams.PropertyType)
                                                                                    .Select(pt => pt.PropertyTypeName)
                                                                                    .FirstOrDefault();

            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);
            //Advanced Search End
            ViewBag.FeaturedProperties = await _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: searchParams.PropertyType, cityId: searchParams.CityId);


            //Save Search Params
            if (User.Identity.IsAuthenticated)
            {
                await _searchTrackingManager.AddSearchParameters(User.Identity.GetUserId(), searchParams);
            }
            //Save Search Params

            var properties = await _propertyManager.GetPropertiesForMap(searchParams.PropertyType, searchParams.CityId, searchParams.DistanceFromCity, isForRent: searchParams.IsForRent, isForShortPeriod: searchParams.IsShortPeriodRent);
            return View(properties);
        }


        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetAgentProperties([Required(ErrorMessage = "Изберете агент!")]string agentId, int pageCount = 1, int pageSize = 6)
        {
            //Setup
            ViewBag.pageCount = pageCount;
            ViewBag.pageSize = pageSize;
            ViewBag.agentId = agentId;
            //---- Setup End -----

            var properties = await _propertyManager.GetAgentProperties(agentId: agentId, page: pageCount, pageSize: pageSize);

            return Json(properties, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Recommended(int? propertyType = null, int? cityId = null, int pageCount = 1, int pageSize = 6,
            string sortBy = "PropertyName", string orderBy = "Descending")
        {
            //Setup filters
            ViewBag.PropertiesTitle = propertyType == null ? All : PropertiesServices.GetPropertyType((int)propertyType);
            ViewBag.propertyType = propertyType;
            ViewBag.cityId = cityId;
            ViewBag.pageCount = pageCount;
            ViewBag.pageSize = pageSize;
            ViewBag.sortBy = sortBy;
            ViewBag.orderBy = orderBy;
            //---- Setup End -----

            //Setup Advanced Search
            ViewBag.PropertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = await _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: propertyType);
            var pagedProperties = await _propertyManager.GetProperties(isForRent: false
                , priceFrom: null, priceTo: null
                , propertyType: propertyType
                , cityId: cityId, distanceFromCity: null
                , areaFrom: null, areaTo: null
                , pageCount: pageCount, pageSize: pageSize
                , sortBy: sortBy, orderBy: orderBy);

            //LastPage
            var propCount = pagedProperties.Count;
            ViewBag.LastPage = propCount % pageSize == 0 ? (propCount / pageSize) : (propCount / pageSize) + 1;

            return View(pagedProperties.Properties);
        }

        /// <summary>
        /// Search View
        /// </summary>
        /// <returns></returns>
        //GET: Properties/Search
        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Search()
        {
            //Setup Advanced Search
            ViewBag.PropertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = await _propertyManager.GetRelatedPropertiesBriefInfo();

            return View();
        }


        // GET: Properties/Details
        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Details(int? id, bool isRentSearching = false)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            try
            {
                // Advanced search
                ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);
                ViewBag.PropertyTypes = await _propertyManager.GetPropertyTypes();
                //Our Agents
                ViewBag.Agents = await _userServices.GetTopTwoAgents();
                //Related Properties
                ViewBag.RelatedProperties = await _propertyManager.GetRelatedProperties((int)id, isRentSearching);
                //Clients aside
                ViewBag.HappyClient = await _reviewsManager.ListClientReviews();
                //

                //Is the user eligable to rate the agent
                ViewBag.IsAllowedToRateProperty = User.Identity.IsAuthenticated &&
                    await _appointmentsManager.HasPassedApprovedAppointmentForProperty(User.Identity.GetUserId(), (int)id);

                DetailsPropertyViewModel property = await _propertyManager.Details((int)id);
                property.IsRentSearch = isRentSearching;

                if (!User.Identity.IsAuthenticated || User.IsInRole("Client"))
                {
                    //Increase views if user is not auth or is Client
                    await _propertyManager.IncrementViews((int)id);
                }

                return View(property);
            }
            catch (ContentNotFoundException ex)
            {
                return HttpNotFound(ex.Message);
            }
        }

        // GET: Properties/Create        
        [AuthorizeUser(Roles = "Administrator,Agent")]
        public async Task<ActionResult> Create(int? propertyType = null)
        {
            //Set the viewBag with all needed information to create properties (and edit)
            await PropertiesCreateViewBagSetup(propertyType);

            //Property Extras populated here
            CreatePropertyViewModel property = new CreatePropertyViewModel
            {
                PropertyExtrasCheckBoxes = _extrasManager.GetExtras("PropertyExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList()
            };

            return View(property);
        }


        // POST: Properties/Create
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = "Administrator,Agent")]
        public async Task<ActionResult> Create(CreatePropertyViewModel model)
        {
            _logger.Info("Creating property! Params: " + model.ToJson());

            if (model.RentalPrice != null && model.RentalPricePeriodId == null ||
                model.RentalPrice == null && model.RentalPricePeriodId != null)
            {
                ModelState.AddModelError("RentalPrice", "И двете полета трябва да са попълнени");
            }
            if (!ModelState.IsValid)
            {
                _logger.Error("Creating Property! Errors: " + ModelState.ToJson());

                //Set the viewBag with all needed information to create properties (and edit)
                await PropertiesCreateViewBagSetup(model.PropertyTypeId);
                model.PropertyExtrasCheckBoxes = _extrasManager.GetExtras("PropertyExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList();
                ViewBag.Title = "Създай Имот";

                return Json(ModelState.ToDictionary());
            }

            try
            {
                var propertyId = await _propertyManager.AddProperty(model, User.Identity.GetUserId());
                _logger.Info("Creating Property Successfully!");

                return Json(propertyId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Creating Property Failed!");
                throw;
            }

        }

        //
        //GET: /Properties/EditPropertyInfo/
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Authorize(Roles = "Administrator,Agent")]
        public async Task<ActionResult> EditPropertyInfo(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            try
            {
                EditPropertyViewModel property = await _propertyManager.GetPropertyForEdit((int)id);
                //Set the viewBag with all needed information to create properties (and edit)
                await PropertiesCreateViewBagSetup(property.PropertyTypeId);

                return View(property);
            }
            catch (ContentNotFoundException ex)
            {
                return HttpNotFound(ex.Message);
            }
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = "Administrator,Agent")]
        public async Task<ActionResult> EditPropertyInfo(EditPropertyViewModel model)
        {
            _logger.Info("Editing Property! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Editing Property Form Invalid! Errors: " + model.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                await _propertyManager.EditProperty(model);
                _logger.Info("Editing Property Successfully!");

                return Json("STATUS_OK");
            }
            catch (ContentNotFoundException ex)
            {
                _logger.Error(ex, "Editing Property Failed!");
                return HttpNotFound(ex.Message);
            }
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = "Administrator,Agent")]
        public async Task<ActionResult> ChangePropertyStatus(ChangePropertyStatusViewModel model)
        {
            _logger.Info("Change Property Status! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Change Property Status Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                await _propertyManager.ChangePropertyStatus(model.PropertyId, model.State);
                _logger.Info("Change Property Status Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Change Property Status Failed!");
                throw;
            }
        }

        //
        //POST Properties/AddImages
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddImages(ImageCreateViewModel model, bool isForSlider = false)
        {
            _logger.Info("Adding Image To Property! Params:" + (new { model, isForSlider }).ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Info("Adding Image To Property Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                var images = await _propertyManager.AddImages(model, User.Identity.GetUserId(), isForSlider);
                _logger.Info("Adding Image To Property Successfully!");

                return Json(images);
            }
            catch (ContentNotFoundException ex)
            {
                _logger.Error(ex, "Adding Image To Property Failed");
                return HttpNotFound(ex.Message);
            }
        }

        //
        //POST: Properties/DeleteImage
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteImage(int? id)
        {
            _logger.Info("Deleting Image! Id: " + id);

            if (id == null)
            {
                _logger.Error("Deleted Image not Found!");
                return Json("STATUS_ERR");
            }

            try
            {
                await _propertyManager.RemoveImage((int)id, User.Identity.GetUserId());
                _logger.Info("Deleting Image Successfully!");

                return Json("STATUS_OK");
            }
            catch (ContentNotFoundException ex)
            {
                _logger.Error(ex, "Deleting Image Failed!");
                return HttpNotFound(ex.Message);
            }
        }

        // POST: Properties/Delete/5
        [System.Web.Mvc.HttpPost, System.Web.Mvc.ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            try
            {
                await _propertyManager.DeleteProperty((int)id, User.Identity.GetUserId());
                return Json("STATUS_OK");
            }
            catch (ContentNotFoundException ex)
            {
                return HttpNotFound(ex.Message);
            }
        }


        #region PropertyReviews

        //
        // POST: /Properties/CreateReview
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateReview(PropertyReviewCreateViewModel review)
        {
            _logger.Info("Creating Property Review! Params: " + review.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Creating Property Review Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                await _propertyManager.AddPropertyReview(review, User.Identity.GetUserId());
                _logger.Info("Creating Property Review Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Creating Property Review Failed!");
                throw;
            }
        }

        #endregion




        private async Task PropertiesCreateViewBagSetup(int? propertyType)
        {
            ViewBag.CountryId = new SelectList(_cityManager.GetCountriesForDropDown(), "CountryId", "CountryName");
            ViewBag.PropertySeasonId = new SelectList((await _propertyManager.GetPropertySeasons()), "PropertySeasonId", "PropertySeasonName");
            ViewBag.PropertyTypeId = new SelectList((await _propertyManager.GetPropertyTypes()), "PropertyTypeId", "PropertyTypeName", propertyType);
            ViewBag.UnitTypeId = new SelectList(_rentalInfoManager.GetRentalTypesForDropDown(), "UnitTypeId", "RentalTypeName");
            ViewBag.OwnerId = new SelectList(_userServices.GetUsersInRole("Owner").AsEnumerable().Select(Mapper.Map<UsersIdInfoViewModel>).ToList(), "Id", "Info");
            ViewBag.RentalPeriodId = _rentalInfoManager.GetRentalPeriods();
            ViewBag.RentalExtras = _extrasManager.GetExtras("RentalExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList();

            //Property Additional Information
            ViewBag.Title = "Създай " + (propertyType != null ? PropertiesServices.GetPropertyType((int)propertyType) : "Имот");
        }
    }
}
