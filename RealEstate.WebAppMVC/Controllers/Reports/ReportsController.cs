using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.Reports;
using RealEstate.ViewModels.WebMVC.Reports;

namespace RealEstate.WebAppMVC.Controllers.Reports
{
    [Authorize(Roles = "Agent,Administrator,Maintenance")]
    public class ReportsController : Controller
    {

        private readonly PromotionMediaServices _promotionMediaServices;
        private readonly WebPlatformServices _webPlatformsManager;
        private readonly PartnerServices _partnerServices;
        private readonly OwnerServices _ownersManager;
        private readonly ReportServices _reportServices;
        private readonly CityServices _cityServices;

        public ReportsController(WebPlatformServices webPlatformsManager
            , PromotionMediaServices promotionMediaServices, PartnerServices partnerServices
            , ReportServices reportServices, OwnerServices ownersManager, CityServices cityServices)
        {
            _webPlatformsManager = webPlatformsManager;
            _promotionMediaServices = promotionMediaServices;
            _partnerServices = partnerServices;
            _reportServices = reportServices;
            _ownersManager = ownersManager;
            _cityServices = cityServices;
        }

        // GET
        [HttpGet]
        public async Task<ActionResult> Create(int propertyId)
        {
            ViewBag.Platforms = await _webPlatformsManager.ListAsync();
            ViewBag.PromotionMediae = await _promotionMediaServices.ListAsKeyValue();
            ViewBag.Brokers = await _partnerServices.ListBrokersForDropdown();
            ViewBag.Cities = await _cityServices.GetCitiesForDropDown(40);

            ViewBag.Owner = await _ownersManager.GetOwner(propertyId);

            return View(propertyId);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ReportCreateViewModel report)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            await _reportServices.CreateReport(report, User.Identity.GetUserId());
            return Json("STATUS_OK");
        }
    }
}