using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.WebAppMVC.Controllers
{
    [Authorize]
    public class RentalInfoesController : Controller
    {
        private readonly RentalInfoServices _rentalInfoManager;
        private readonly ExtraServices _extrasManager;
        private readonly CityServices _cityManager;
        private readonly PropertiesServices _propertyManager;
        private readonly ClientServices _clientsManager;
        private readonly UserServices _userServices;


        [Inject]
        public RentalInfoesController(RentalInfoServices rentalInfoServices, ExtraServices extraServices, CityServices cityManager, PropertiesServices propertyManager, ClientServices clientsManager, UserServices userServices)
        {
            _rentalInfoManager = rentalInfoServices;
            _extrasManager = extraServices;
            _cityManager = cityManager;
            _propertyManager = propertyManager;
            _clientsManager = clientsManager;
            _userServices = userServices;
        }

        // GET: RentalInfoes
        public ActionResult Index()
        {

            throw new NotImplementedException();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Agent,Administrator,Maintenance")]
        public async Task<ActionResult> Create(CreateRentalInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var resultRental = await _rentalInfoManager.CreateRentalInfo(model, User.Identity.GetUserId());

            return Json(resultRental);
        }


        //
        //GET: /RentalInfoes/Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.UnitTypeId = _rentalInfoManager.GetRentalTypesForDropDown();
            ViewBag.RentalPeriodId = _rentalInfoManager.GetRentalPeriods();
            ViewBag.RentalExtras = _extrasManager
                .GetExtras("RentalExtras")
                .Select(Mapper.Map<ExtraCheckBoxViewModel>)
                .ToList();
            ViewBag.PropertyId = id;
            try
            {
                var rental = await _rentalInfoManager.GetRentalForEdit((int) id);
                return View(rental);
            }
            catch (ContentNotFoundException ex)
            {
                return HttpNotFound(ex.Message);
            }
        }


        //
        //GET: /RentalInfoes/Edit
        [HttpGet]
        public ActionResult EditPropertyRentals(int? propertyId)
        {
            if (propertyId == null)
            {
                return HttpNotFound();
            }

            try
            {
                ViewBag.UnitTypeId = _rentalInfoManager.GetRentalTypesForDropDown();
                ViewBag.RentalPeriodId = _rentalInfoManager.GetRentalPeriods();
                ViewBag.RentalExtras = _extrasManager
                    .GetExtras("RentalExtras")
                    .Select(Mapper.Map<ExtraCheckBoxViewModel>)
                    .ToList();
                ViewBag.PropertyId = propertyId;

                var propertyRentals = _rentalInfoManager.GetRentalInfoes((int)propertyId);
                return View(propertyRentals);
            }
            catch (ContentNotFoundException ex)
            {
                return HttpNotFound(ex.Message);
            }
        }


        //
        //POST: /RentalInfoes/Edit
        [Authorize(Roles = "Agent,Owner,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditRentalInfoForPropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            try
            {
                await _rentalInfoManager.EditRentalInfo(model, User.Identity.GetUserId());
                return Json("STATUS_OK");
            }
            catch (ContentNotFoundException ex)
            {
                return HttpNotFound(ex.Message);
            }
        }


        //
        // POST: RentalInfoes/Delete
        [Authorize(Roles = "Agent,Owner,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? rentalInfoId)
        {
            if (rentalInfoId == null)
            {
                return HttpNotFound();
            }

            try
            {
                await _rentalInfoManager.Delete((int)rentalInfoId, User.Identity.GetUserId());
                return Json("STATUS_OK");
            }
            catch (ContentNotFoundException ex)
            {
                return HttpNotFound(ex.Message);
            }
        }
    }
}