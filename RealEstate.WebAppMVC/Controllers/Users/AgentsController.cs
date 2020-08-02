using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.ViewModels.WebMVC;
using RealEstate.WebAppMVC.Helpers;
using ReviewServices = RealEstate.Services.ReviewServices;

namespace RealEstate.WebAppMVC.Controllers
{
    public class AgentsController : Controller
    {
        private UserServices UsersManager { get; set; }
        private PropertiesServices PropertiesManager { get; set; }
        private ReviewServices ReviewsManager { get; set; }
        private AgentServices AgentsManager { get; set; }
        private AppointmentServices AppointmentsManager { get; set; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        [Inject]
        public AgentsController(UserServices userServices, PropertiesServices propertiesServices, ReviewServices reviewsManager, AgentServices agentsManager, AppointmentServices appointmentsManager)
        {
            UsersManager = userServices;
            PropertiesManager = propertiesServices;
            ReviewsManager = reviewsManager;
            AgentsManager = agentsManager;
            AppointmentsManager = appointmentsManager;
        }

        // GET: Agents
        public async Task<ActionResult> Index()
        {

            ViewBag.FeaturedAgents = await UsersManager.GetTopTwoAgents();

            var agents = await UsersManager.GetAgents();

            return View(agents);
        }

        [HttpGet]
        public async Task<ActionResult> Details(string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                return HttpNotFound();
            }

            ViewBag.Agents = await UsersManager.GetTopTwoAgents();

            //Is the user eligable to rate the agent
            ViewBag.IsAllowedToRateAgent =
                await AppointmentsManager.HasPassedApprovedAppointmentWithAgent(User.Identity.GetUserId(), agentId);

            TeamUserListViewModel agent = await UsersManager.GetAgent(agentId);

            return View(agent);
        }


        #region Reviews       

        //GET: /Agents/GetReviews
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetReviews(string agentId)
        {
            try
            {
                var reviews = await AgentsManager.GetReviews(agentId);

                return Json(reviews, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new List<ReviewListViewModel>(), JsonRequestBehavior.AllowGet);
            }
        }

        //POST: /Agents/RateAgent
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RateAgent(AgentReviewCreateViewModel model)
        {
            _logger.Info("Rate Agent! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Rate Agent Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                var userId = User.Identity.GetUserId();
                await AgentsManager.CreateReview(model, userId);
                _logger.Info("Rate Agent Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Rate Agent Failed!");
                throw;
            }

        }

        //POST: /Agents/DeleteAgent
        [Authorize(Roles = "Client,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteReview(int? reviewId)
        {
            _logger.Info("Deleting Review! Id: " + reviewId);

            if (reviewId == null)
            {
                _logger.Warn("Deleted Review! Not Found!");
                return HttpNotFound();
            }

            try
            {
                await AgentsManager.DeleteReview((int)reviewId, User.Identity.GetUserId());
                _logger.Info("Delete Review Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Deleting Review Failed!");
                throw;
            }

        }

        #endregion
    }
}