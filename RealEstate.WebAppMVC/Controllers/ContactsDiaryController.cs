using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.WebAppMVC.Controllers
{
    [Authorize(Roles = "Agent, Administrator")]
    public class ContactsDiaryController : Controller
    {
        private readonly ContactsDiaryServices _contactsDiaryServices;
        private readonly PropertiesServices _propertyManager;
        private readonly CityServices _cityManager;
        private readonly AgentServices _agentServices;

        public ContactsDiaryController(ContactsDiaryServices contactsDiaryServices, PropertiesServices propertyManager, CityServices cityManager, AgentServices agentServices)
        {
            _contactsDiaryServices = contactsDiaryServices;
            _propertyManager = propertyManager;
            _cityManager = cityManager;
            _agentServices = agentServices;
        }

        // GET: ContactsDiary
        [HttpGet]
        public async Task<ActionResult> Index(int? cityId = null, string cityDistrict = null
            , int? propertyTypeId = null
            , string phoneNumber = null, int? dealTypeId = null
            , int? contactedPersonTypeId = null
            , int? negotiationStateId = null)
        {
            //Filling dropdowns
            ViewBag.PropertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);
            ViewBag.DealTypes = await _contactsDiaryServices.ListDealTypes();
            ViewBag.NegotiationStages = await _contactsDiaryServices.ListNegotiationStages();
            ViewBag.PersonTypes = await _contactsDiaryServices.ListPersonType();
            ViewBag.Agents = await _agentServices.GetAgentsForDropDown();
            //---------

            var records = await _contactsDiaryServices.List(cityId: cityId, propertyTypeId: propertyTypeId
                , phoneNumber: phoneNumber, dealTypeId: dealTypeId
                , contactedPersonTypeId: contactedPersonTypeId
                , cityDistrict: cityDistrict, negotiationStateId: negotiationStateId);

            return View(records);
        }


        // GET: ContactsDiary
        [HttpGet]
        public async Task<ActionResult> MyContacts(int? cityId = null, string cityDistrict = null
            , int? propertyTypeId = null
            , string phoneNumber = null, int? dealTypeId = null
            , int? contactedPersonTypeId = null
            , int? negotiationStateId = null)
        {
            //Fillig selects
            ViewBag.PropertyTypes = await _propertyManager.GetPropertyTypes();
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);
            ViewBag.DealTypes = await _contactsDiaryServices.ListDealTypes();
            ViewBag.NegotiationStages = await _contactsDiaryServices.ListNegotiationStages();
            ViewBag.PersonTypes = await _contactsDiaryServices.ListPersonType();
            ViewBag.Agents = await _agentServices.GetAgentsForDropDown();
            //---------

            var agentId = User.Identity.GetUserId();
            var records = await _contactsDiaryServices.List(cityId: cityId, propertyTypeId: propertyTypeId
                , phoneNumber: phoneNumber, dealTypeId: dealTypeId
                , agentId: agentId, contactedPersonTypeId: contactedPersonTypeId
                , cityDistrict: cityDistrict, negotiationStateId: negotiationStateId);

            return View("Index", records);
        }


        [HttpPost]
        public async Task<ActionResult> GetRecordsJson(int? cityId = null, string cityDistrict = null
            , int? propertyTypeId = null
            , string phoneNumber = null, int? dealTypeId = null
            , int? contactedPersonTypeId = null
            , int? negotiationStateId = null)
        {
            var agentId = User.Identity.GetUserId();
            var records = await _contactsDiaryServices.List(cityId: cityId, propertyTypeId: propertyTypeId
                , phoneNumber: phoneNumber, dealTypeId: dealTypeId
                , agentId: agentId, contactedPersonTypeId: contactedPersonTypeId
                , cityDistrict: cityDistrict, negotiationStateId: negotiationStateId);

            return Json(records);
        }

        [HttpGet]
        public async Task<ActionResult> Get(int? id)
        {
            if (id == null)
            {
                throw new ContentNotFoundException();
            }
            var record = await _contactsDiaryServices.Get((int)id);

            return Json(record);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var agentId = User.Identity.GetUserId();
            var record = await _contactsDiaryServices.CreateRecord(model, agentId);

            return Json(record);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var agentId = User.Identity.GetUserId();
            var record = await _contactsDiaryServices.EditRecord(model, agentId);

            return Json(record);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _contactsDiaryServices.DeleteRecord(id, User.Identity.GetUserId());
            return Json("STATUS_OK");
        }
    }
}