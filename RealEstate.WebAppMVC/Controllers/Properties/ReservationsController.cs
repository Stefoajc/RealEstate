using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.Payments;
using RealEstate.ViewModels.WebMVC;
using RealEstate.WebAppMVC.Helpers;

namespace RealEstate.WebAppMVC.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private ReservationServices ReservationsManager { get; set; }
        private PaymentServices PaymentsManager { get; set; }
        private ApplicationUserManager UserManager { get; set; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public ReservationsController(ReservationServices reservationServices, ApplicationUserManager userManager)
        {
            ReservationsManager = reservationServices;
            UserManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetReservations([Required(ErrorMessage = "Изберете имот!")]int propertyId)
        {
            var reservations = await ReservationsManager.ListReservations(propertyId);

            return Json(reservations, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetOwnReservations()
        {
            var userId = User.Identity.GetUserId();
            List<ListReservationViewModel> reservations = await ReservationsManager.ListUserReservations(userId);

            return View("MyReservations", reservations);

            //return Json(reservations, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateReservationViewModel model)
        {
            _logger.Info("Creating Reservation! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Creating Reservation Form Invalid! Errors:" + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            //if (await UserManager.IsPhoneNumberConfirmedAsync(User.Identity.GetUserId()) 
            //    && await UserManager.HasPhoneNumberAsync(User.Identity.GetUserId()))
            //{
            //    return Json("Въведете и потвърдете телефонен номер тогава опитайте отново!");
            //}

            try
            {
                CreateReservationDTO reservation = Mapper.Map<CreateReservationViewModel, CreateReservationDTO>(model, o => o.Items["UserId"] = User.Identity.GetUserId());
                await ReservationsManager.CreateReservation(reservation);
                _logger.Info("Creating Reservation Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Creating Reservation Failed!");
                throw;
            }

        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateForUnregistered(CreateReservationForNonRegisteredUserViewModel model)
        {
            _logger.Info("Creating Reservation! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Creating Reservation Form Invalid! Errors:" + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                CreateReservationDTO reservation = Mapper.Map<CreateReservationForNonRegisteredUserViewModel, CreateReservationDTO>(model);
                await ReservationsManager.CreateReservation(reservation);
                _logger.Info("Creating Reservation Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Creating Reservation Failed!");
                throw;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? reservationId)
        {
            _logger.Info("Deleting Reservation! ReservationId: " + reservationId);

            if (reservationId == 0 || reservationId == null)
            {
                _logger.Warn("Deleting Reservation Not Found!");
                return HttpNotFound();
            }
            try
            {
                await ReservationsManager.DeleteReservation((int)reservationId, User.Identity.GetUserId());
                _logger.Info("Deleting Reservation Successfully!");

                return Json("STATUS_OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Deleting Reservation Failed!");
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? reservationId)
        {
            if (reservationId == 0 || reservationId == null)
            {
                return HttpNotFound();
            }

            var reservation = await ReservationsManager.GetReservation((int)reservationId, User.Identity.GetUserId());

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PayReservation(ReservationPaymentViewModel model)
        {
            _logger.Info("Paying Reservation! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Paying Reservation Form Invalid! Errors:" + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
                //throw new ArgumentException("Проблем при плащане на резервацията. Опитайте отново, ако не стане свържете се с екипът ни!");
            }
            if (model.PaymentInfo.PaymentMethod.ToUpper() == "EASYPAY" && (string.IsNullOrEmpty(model.PaymentInfo.PayerName) || model.PaymentInfo.PayerName.Length > 26))
            {
                ModelState.AddModelError(nameof(model.PaymentInfo.PayerName), "Името е задължително и не трябва да превишава 26 символа");
                _logger.Error("Paying Reservation Form Invalid! Errors:" + ModelState.ToJson());

                return Json(ModelState.ToDictionary());
            }

            try
            {
                //Either EasyPayCode or redirect Form which have to be submited
                var paymentRedirectForm = await ReservationsManager.PayReservation(model.Id, model.IsCaparoPayed, model.PaymentInfo, model.UrlOk, model.UrlCancel);
                _logger.Info("Paying Reservation - Redirecting to External Payment provider!");

                return Content(paymentRedirectForm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Paying Reservation Failed!");
                throw;
            }
        }

    }
}