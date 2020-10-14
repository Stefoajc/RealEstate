using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.Contact;
using RealEstate.Services.Forum;
using RealEstate.Services.Reports;
using RealEstate.ViewModels.WebMVC.Contact;
using RealEstate.ViewModels.WebMVC.Reports;
using RealEstate.WebAppMVC.Helpers;
using ReviewServices = RealEstate.Services.ReviewServices;

namespace RealEstate.WebAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly PropertiesServices propertyManager;
        private readonly ReservationServices reservationsManager;
        private readonly ReviewServices reviewsManager;
        private readonly UserServices userServices;
        private readonly PostServices postsManager;
        private readonly CommentServices commentsManager;
        private readonly ContactMessageServices contactMessagesManager;
        private readonly CityServices citiesManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public HomeController(PropertiesServices propertyServices, 
            UserServices userServices, 
            ReservationServices reservationServices, 
            ReviewServices reviewServices, 
            PostServices postsManager, 
            CommentServices commentsManager, 
            ContactMessageServices contactMessagesManager, 
            CityServices citiesManager)
        {
            propertyManager = propertyServices;
            reservationsManager = reservationServices;
            reviewsManager = reviewServices;
            this.postsManager = postsManager;
            this.commentsManager = commentsManager;
            this.contactMessagesManager = contactMessagesManager;
            this.citiesManager = citiesManager;
            this.userServices = userServices;

        }

        [AllowAnonymous]
        //GET: Properties
        public async Task<ActionResult> Index()
        {
            ViewBag.SliderProperties = await propertyManager.GetPropertiesForMainSlider(3);

            ViewBag.Cities = await citiesManager.GetCitiesForDropDown(40);
            ViewBag.PropertyTypes = await propertyManager.GetPropertyTypes();
            ViewBag.RelatedProperties = await propertyManager.GetRelatedPropertiesBriefInfo();
            ViewBag.HappyClient = await reviewsManager.ListClientReviews();

            var propertiesAggregated = await propertyManager.GetPropertiesForHomePage();

            ViewBag.PropertiesAll = propertiesAggregated.PropertiesForSell;
            ViewBag.Houses = propertiesAggregated.HousesForSell;
            ViewBag.Offices = propertiesAggregated.OfficesForSell;
            ViewBag.Apartments = propertiesAggregated.ApartmentsForSell;

            ViewBag.PropertiesRentedAll = propertiesAggregated.PropertiesForRent;
            ViewBag.HousesRented = propertiesAggregated.HousesForRent;
            ViewBag.OfficesRented = propertiesAggregated.OfficesForRent;
            ViewBag.ApartmentsRented = propertiesAggregated.ApartmentsForRent;

            //Agents List
            ViewBag.Agents = await userServices.GetTopTwoAgents();
            return View();
        }

        public async Task<ActionResult> About()
        {
            ViewBag.HappyClient = await reviewsManager.ListClientReviews();
            ViewBag.TeamMembers = userServices.GetTeamMembers();

            return View();
        }

        public async Task<ActionResult> Contact()
        {
            ViewBag.Agents = await userServices.GetTopTwoAgents();
            ViewBag.PopularPosts = await postsManager.ListMostPopular();
            ViewBag.LatestComments = await commentsManager.ListLatest();

            return View();
        }

        public async Task<ActionResult> FAQ()
        {
            ViewBag.PopularPosts = await postsManager.ListMostPopular();
            ViewBag.LatestComments = await commentsManager.ListLatest();

            return View();
        }

        public async Task<ActionResult> Team()
        {
            ViewBag.FeaturedAgents = await userServices.GetTopTwoAgents();
            ViewBag.Agents = await userServices.GetAgents();
            ViewBag.Maintenance = await userServices.GetMaintenance();
            ViewBag.Marketers = await userServices.GetMarketers();

            return View();
        }

        #region Misc

        public ActionResult PriceBuilding()
        {
            return View();
        }

        public ActionResult Marketing()
        {
            //Not Implemented
            return View("/Views/Errors/HttpNotFound.cshtml", new ErrorsController.ErrorMessageViewModel(null, null));
        }

        public ActionResult Loans()
        {
            return View();
        }

        public ActionResult CookiesPolicy()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }

        public ActionResult PersonalInfoSecurity()
        {
            return View();
        }

        public ActionResult CommunityRules()
        {
            return View();
        }

        #endregion


        [HttpPost]
        public async Task<ActionResult> SendMessage(ContactMessageViewModel model)
        {
            _logger.Log(LogLevel.Info, "User posted contact form: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Log(LogLevel.Error, "User contact form is Invalid! Errors: " + ModelState.ToJson());

                return Json(ModelState.ToDictionary());
            }

            try
            {
                await contactMessagesManager.AddContactMessage(model);
                _logger.Log(LogLevel.Info, "Contact form saved succesfully !");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Problem submitting contact form!");
                throw;
            }
        }

        public ActionResult GetReservationsCount(int propertyId, DateTime from, DateTime to)
        {
            return Json(reservationsManager.GetReservationsCount(propertyId, from, to));
        }


        public ActionResult ListReviews(int propertyId)
        {
            return View(reviewsManager.ListPropertyReviews(propertyId));
        }

        public async Task<ActionResult> SendSms()
        {
            NotificationSmsService smsService = new NotificationSmsService();
            try
            {
                await smsService.SendSmsAsync("359876717000", "Test");
                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
    }
}