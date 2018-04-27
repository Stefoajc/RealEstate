using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ninject;
using RealEstate.Model;
using RealEstate.Services;
using RealEstate.ViewModels.WebMVC;

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

        [Inject]
        public HomeController(PropertiesServices propertyServices, UserServices userServices, ClientServices clientServices, RentalInfoServices rentalInfoServices, ReservationServices reservationServices, ReviewServices reviewServices)
        {
            _propertyManager = propertyServices;
            RentalsManager = rentalInfoServices;
            ReservationsManager = reservationServices;
            ReviewsManager = reviewServices;
            _userServices = userServices;
            _clientsManager = clientServices;
        }

        public ActionResult Index()
        {
            //IEmailService mailService = new GmailMailService();
            //await mailService.SendEmailAsync("stefoajc@abv.bg", "Subject", "<h1>Body<h1>", true);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.HappyClient = _clientsManager.GetHappyClients();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Activity()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Agents = _userServices.GetTopTwoAgents();
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Team()
        {
            ViewBag.FeaturedAgents = _userServices.GetTopTwoAgents();
            ViewBag.Agents = _userServices.GetAgents();
            ViewBag.Maintenance = "";
            ViewBag.Marketers = "";

            return View();
        }

        public ActionResult GetRentalsForProperty(int propertyId,DateTime from, DateTime to)
        {

            return Json(RentalsManager.GetFreeRentalsByBedsCount(propertyId, from, to), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReservationsCount(int propertyId, DateTime from, DateTime to)
        {
            return Json(ReservationsManager.GetReservationsCount(propertyId, from, to));
        }


        public ActionResult ListReviews(int propertyId)
        {
            return View(ReviewsManager.ListPropertyReviews(propertyId));
        }

        public ActionResult CreateReservations()
        {
            var reservation1 = new CreateReservationViewModel
            {
                RentalId = 1017,
                From = new DateTime(2018,3,6),
                To = new DateTime(2018,3,8)
            };

            var reservation2 = new CreateReservationViewModel
            {
                RentalId = 1017,
                From = new DateTime(2018, 3, 7),
                To = new DateTime(2018, 3, 10)
            };

            ReservationsManager.CreateReservation(reservation1);
            ReservationsManager.CreateReservation(reservation2);

            return Json("STATUS_OK", JsonRequestBehavior.AllowGet);
        }



        public async Task<ContentResult> GetCityCoords()
        {
            string cityName = "Пловдив";
            using (HttpClient client = new HttpClient())
            {
                var response =
                    await client.GetAsync(
                        "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyCRNibRrD3DnsD2EoqbagZhLwnf82GxraU&address=" + cityName);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    dynamic jsonObject = serializer.Deserialize<dynamic>(result);

                    dynamic x = jsonObject["results"][0]["geometry"]["location"]; // result is Dictionary<string,object> user with fields name, teamname, email and players with their values
                    var lat = jsonObject["results"][0]["geometry"]["location"]["lat"]; // result is asdf
                    var lng = jsonObject["results"][0]["geometry"]["location"]["lng"]; // result is object[] players with its values
                    return Content(result);
                }

                return Content("Not Found");
            }

            
        }
    }
}