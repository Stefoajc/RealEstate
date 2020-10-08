using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services.MailList;
using RealEstate.WebAppMVC.Helpers;

namespace RealEstate.WebAppMVC.Controllers.MailList
{
    public class MailListController : Controller
    {
        [Inject]
        private MailListServices mailListManager;

        public MailListController(MailListServices mailListManager)
        {
            this.mailListManager = mailListManager;
        }

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // POST: /MailList/Subscribe
        [HttpPost]
        public async Task<ActionResult> Subscribe(EmailDTO model)
        {
            _logger.Info("User posted subscription Form: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("User posted Invalid Form: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                await mailListManager.Subscribe(model.email);
                _logger.Info("User posted subscription Successfully!");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Subscribing Failed!");
            }

            return Json("STATUS_OK");
        }

        // Delete: /MailList/AddMailToList
        [HttpDelete]
        public async Task<ActionResult> RemoveMailFromList([Required(ErrorMessage = "Въведете е-мейл")] string id)
        {
            _logger.Info("User requested Mail removal from the Mail List! EmailId:" + id);

            if (!ModelState.IsValid)
            {
                _logger.Error("User remove email request form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                await mailListManager.Unsubscribe(id);
                _logger.Info("User Mail removed from the Email list!");

                return View();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Removing user's mail failed!");
                throw;
            }
        }
    }
}