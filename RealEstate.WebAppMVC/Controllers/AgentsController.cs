using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Ninject;
using RealEstate.Services;

namespace RealEstate.WebAppMVC.Controllers
{
    public class AgentsController : Controller
    {
        private UserServices UsersManager { get; set; }
        private PropertiesServices PropertiesManager { get; set; }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        [Inject]
        public AgentsController(UserServices userServices, PropertiesServices propertiesServices)
        {
            UsersManager = userServices;
            PropertiesManager = propertiesServices;
        }

        // GET: Agents
        public ActionResult Index()
        {

            ViewBag.FeaturedAgents = UsersManager.GetTopTwoAgents();

            var agents = UsersManager.GetAgents();

            return View(agents);
        }

        [HttpGet]
        public ActionResult Details(string agentId, int pageCount = 1, int pageSize = 6)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                return HttpNotFound();
            }
            //Setup
            ViewBag.pageCount = pageCount;
            ViewBag.pageSize = pageSize;
            ViewBag.agentId = agentId;
            //LastPage
            var propCount = PropertiesManager.GetAgentPropertiesCount(agentId);
            ViewBag.LastPage = propCount / pageSize == 0 ? (propCount / pageSize) - 1 : (propCount / pageSize);
            //---- Setup End -----

            var agent = UsersManager.GetAgent(agentId);

            ViewBag.AgentProperties = PropertiesManager.GetPropertiesForSell(null, pageCount, pageSize);

            return View(agent);
        }
    }
}