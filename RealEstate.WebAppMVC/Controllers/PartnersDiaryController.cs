using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.ViewModels.WebMVC.Reports;

namespace RealEstate.WebAppMVC.Controllers
{
    [Authorize(Roles = "Agent,Administrator")]
    public class PartnersDiaryController : Controller
    {
        private readonly PartnerServices _partnersManager;
        private readonly CityServices _cityManager;
        private readonly AgentServices _agentServices;

        public PartnersDiaryController(PartnerServices partnersManager, CityServices cityManager, AgentServices agentServices)
        {
            _partnersManager = partnersManager;
            _cityManager = cityManager;
            _agentServices = agentServices;
        }

        // GET: ColleaguesDiary
        public async Task<ActionResult> Index()
        {
            //Dropdowns Init
            ViewBag.Cities = await _cityManager.GetCitiesForDropDown(40);
            ViewBag.Agents = await _agentServices.GetAgentsForDropDown();
            ViewBag.PartnerTypes = await _partnersManager.ListTypes();
            //

            var currUserId = User.Identity.GetUserId();
            var partners = await _partnersManager.ListAsync(currUserId);

            return View(partners);
        }

        public async Task<ActionResult> MyPartners()
        {
            var currUserId = User.Identity.GetUserId();
            var partners = await _partnersManager.ListAsync(currUserId, createdById: currUserId);

            return View("Index", partners);
        }

        [HttpPost]
        public async Task<ActionResult> Create(PartnersCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var partnerCreated = await _partnersManager.CreateAsync(model, User.Identity.GetUserId());

            return Json(partnerCreated);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBroker(PartnersCreateViewModel model)
        {
            ModelState.Remove(nameof(model.PartnerTypeId));
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            model.PartnerTypeId = (await _partnersManager.GetPartnerType("брокер")).Id;

            var partnerCreated = await _partnersManager.CreateAsync(model, User.Identity.GetUserId());

            return Json(partnerCreated);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(PartnersEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var partnerEdited = await _partnersManager.EditAsync(model, User.Identity.GetUserId());

            return Json(partnerEdited);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _partnersManager.DeleteAsync(id, User.Identity.GetUserId());
                return Json("STATUS_OK");
            }
            catch (Exception)
            {
                return Json("STATUS_ERR");
            }
        }
    }
}