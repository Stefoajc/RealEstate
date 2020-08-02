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
        private readonly PropertiesServices _propertyManager;
        private RentalInfoServices RentalsManager { get; set; }
        private ReservationServices ReservationsManager { get; set; }
        private ReviewServices ReviewsManager { get; set; }
        private readonly UserServices _userServices;
        private readonly ClientServices _clientsManager;
        private PostServices PostsManager { get; }
        private CommentServices CommentsManager { get; }
        private ContactMessageServices ContactMessagesManager { get; }
        private CityServices CitiesManager { get; }
        private readonly ReportServices _reportServices;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public HomeController(PropertiesServices propertyServices, UserServices userServices, ClientServices clientServices, RentalInfoServices rentalInfoServices, ReservationServices reservationServices, ReviewServices reviewServices, PostServices postsManager, CommentServices commentsManager, ContactMessageServices contactMessagesManager, CityServices citiesManager, ReportServices reportServices)
        {
            _propertyManager = propertyServices;
            RentalsManager = rentalInfoServices;
            ReservationsManager = reservationServices;
            ReviewsManager = reviewServices;
            PostsManager = postsManager;
            CommentsManager = commentsManager;
            ContactMessagesManager = contactMessagesManager;
            CitiesManager = citiesManager;
            _reportServices = reportServices;
            _userServices = userServices;
            _clientsManager = clientServices;

        }

        [AllowAnonymous]
        //GET: Properties
        public async Task<ActionResult> Index()
        {
            ViewBag.SliderProperties = await _propertyManager.GetPropertiesForMainSlider(3);

            ViewBag.Cities = await CitiesManager.GetCitiesForDropDown(40);
            ViewBag.PropertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.RelatedProperties = await _propertyManager.GetRelatedPropertiesBriefInfo();
            ViewBag.HappyClient = await ReviewsManager.ListClientReviews();

            var propertiesAggregated = await _propertyManager.GetPropertiesForHomePage();

            ViewBag.PropertiesAll = propertiesAggregated.PropertiesForSell;
            ViewBag.Houses = propertiesAggregated.HousesForSell;
            ViewBag.Offices = propertiesAggregated.OfficesForSell;
            ViewBag.Apartments = propertiesAggregated.ApartmentsForSell;

            ViewBag.PropertiesRentedAll = propertiesAggregated.PropertiesForRent;
            ViewBag.HousesRented = propertiesAggregated.HousesForRent;
            ViewBag.OfficesRented = propertiesAggregated.OfficesForRent;
            ViewBag.ApartmentsRented = propertiesAggregated.ApartmentsForRent;

            //Agents List
            ViewBag.Agents = await _userServices.GetTopTwoAgents();
            return View();
        }

        public async Task<ActionResult> About()
        {
            ViewBag.HappyClient = await ReviewsManager.ListClientReviews();
            ViewBag.TeamMembers = _userServices.GetTeamMembers();

            return View();
        }

        //public ActionResult Activity()
        //{
        //    return View();
        //}

        public async Task<ActionResult> Contact()
        {
            ViewBag.Agents = await _userServices.GetTopTwoAgents();
            ViewBag.PopularPosts = await PostsManager.ListMostPopular();
            ViewBag.LatestComments = await CommentsManager.ListLatest();

            return View();
        }

        public async Task<ActionResult> FAQ()
        {
            ViewBag.PopularPosts = await PostsManager.ListMostPopular();
            ViewBag.LatestComments = await CommentsManager.ListLatest();

            return View();
        }

        public async Task<ActionResult> Team()
        {
            ViewBag.FeaturedAgents = await _userServices.GetTopTwoAgents();
            ViewBag.Agents = await _userServices.GetAgents();
            ViewBag.Maintenance = await _userServices.GetMaintenance();
            ViewBag.Marketers = await _userServices.GetMarketers();

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
                await ContactMessagesManager.AddContactMessage(model);
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
            return Json(ReservationsManager.GetReservationsCount(propertyId, from, to));
        }


        public ActionResult ListReviews(int propertyId)
        {
            return View(ReviewsManager.ListPropertyReviews(propertyId));
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


        public async Task<ActionResult> TestEmailTemplate()
        {
            var templateModel = new ReportTemplateViewModel
            {
                AgentCreator = "Stef Stef",
                CreatedOn = DateTime.Now,
                ActionsConclusion = "Lorem ipsum dolor sit amet",
                ChangeArguments = "Lorem ipsum dolor sit amet",
                IsMarketingChangeIssued = true,
                IsPriceChangeIssued = true,
                LinkToProperty = "https://www.sproperties.net",
                TotalViews = 123,
                TotalCalls = 4,
                TotalInspections = 2,
                TotalOffers = 0,
                CustomRecommendedActions = new List<string>() { "Рекоменд" },
                Offers = new List<decimal>() { 123M },
                PartnersSharedWith = new List<string>() { "Иван Иванов", "Петър петров" },
                PromotionMediae = new List<PromotionMediaForEmail>()
                {
                    new PromotionMediaForEmail
                    {
                        Id = 1,
                        IsChecked = true,
                        MediaType = "Уеб платформи"
                    },
                    new PromotionMediaForEmail
                    {
                        Id = 2,
                        IsChecked = false,
                        MediaType = "Флаери"
                    },
                    new PromotionMediaForEmail
                    {
                        Id = 3,
                        IsChecked = false,
                        MediaType = "Билборд"
                    },
                    new PromotionMediaForEmail
                    {
                        Id = 4,
                        IsChecked = true,
                        MediaType = " Споделяне с контакти "
                    },
                    new PromotionMediaForEmail
                    {
                        Id = 5,
                        IsChecked = true,
                        MediaType = "Транспаранти"
                    },
                    new PromotionMediaForEmail
                    {
                        Id = 6,
                        IsChecked = true,
                        MediaType = " Емейл маркетинг "
                    },
                    new PromotionMediaForEmail
                    {
                        Id = 7,
                        IsChecked = false,
                        MediaType = " Социални мрежи "
                    },
                    new PromotionMediaForEmail
                    {
                        Id = 8,
                        IsChecked = true,
                        MediaType = "Споделяне с Брокери"
                    }
                },
                WebPlatformViews = new List<WebPlatformViewEmailViewModel>
                {
                    new WebPlatformViewEmailViewModel
                    {
                        PlatformName = "olx.bg",
                        Views = 122
                    },
                    new WebPlatformViewEmailViewModel
                    {
                        PlatformName = "bazar.bg",
                        Views = 322
                    }
                },
                CustomPromotionMediae = new List<string> { "One Media" }
            };

            templateModel = await _reportServices.CreateTemplateViewModelAsync(2);

            return View(templateModel);
        }

    }
}