using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.ViewModels.WebMVC;
using RealEstate.WebAppMVC.Helpers;

namespace RealEstate.WebAppMVC.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly ImageServices _imageServices;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ImageServices imageServices)
        {
            _imageServices = imageServices;
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public ApplicationSignInManager SignInManager { get; set; }
        public ApplicationUserManager UserManager { get; set; }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Паролата ви беше сменена."
                : message == ManageMessageId.SetPasswordSuccess ? "Паролата ви е създадена."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Двуфакторна автентикация е включена."
                : message == ManageMessageId.Error ? "Грешка."
                : message == ManageMessageId.AddPhoneSuccess ? "Телефонният номер беше добавен."
                : message == ManageMessageId.RemovePhoneSuccess ? "Телефонният номер беше изтрит."
                : message == ManageMessageId.ChangeUserNameSuccess ? "Потребителското име е сменено."
                : message == ManageMessageId.EmailChangeSuccess ? "Вашият е-мейл адрес сменен, влезте в предоставената електронна поща и потвърдете."
                : "";

            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId) ?? throw new ContentNotFoundException("Този потребител не е намерен, може да е изтрит или блокиран!");

            var model = new IndexViewModel
            {
                Id = user.Id,
                HasPassword = HasPassword(),
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                UserName = user.UserName,
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }



        #region SocialMediaAccount Managing

        //
        //GET: /Manage/SocialMediaManagement
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> SocialMediaManagement()
        {
            var socialMediaAccounts = await UserManager.GetSocialMediaAccounts(User.Identity.GetUserId());
            return View(socialMediaAccounts);
        }

        //POST /Manage/AddUpdateSocialMediaAccount
        [HttpPost]
        public async Task<ActionResult> AddUpdateSocialMediaAccount(SocialMediaAccountViewModel socialMediaAccountViewModel)
        {
            _logger.Info("Adding Social Media Account! Params: " + socialMediaAccountViewModel.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Adding Social Media Account Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                var socialMediaAccount =
                    await UserManager.AddSocialMediaAccount(socialMediaAccountViewModel, User.Identity.GetUserId());
                _logger.Info("Adding Social Media Account Successfully!");

                return Json(socialMediaAccount);
            }
            catch (UniquenessException e)
            {
                ModelState.AddModelError("SocialMedia", e.Message);
                _logger.Info("Adding Social Media Account Failed! Errors: " + ModelState.ToJson());

                return Json(ModelState.ToDictionary());
            }
        }

        //
        //POST /Manage/DeleteSocialMediaAccount
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> DeleteSocialMediaAccount([Required(ErrorMessage = "Изберете социална медия!")]int socialMediaId)
        {
            _logger.Info("Deleting Social Media Account! Id: " + socialMediaId);

            try
            {
                await UserManager.DeleteSocialMediaAccount(socialMediaId, User.Identity.GetUserId());
                _logger.Info("Deleting Social Media Account Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Deleting Social Media Account Failed!");
                throw;
            }
        }

        #endregion

        #region ChangeEmail

        //
        //GET: /Manage/AddEmailAddress
        public ActionResult AddEmailAddress()
        {
            var userId = User.Identity.GetUserId();
            var userEmail = UserManager.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefault();
            return View(new AddEmailAddressViewModel { Email = userEmail });
        }

        //
        //POST: /Manage/AddEmailAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEmailAddress(AddEmailAddressViewModel model)
        {
            _logger.Info("Adding Email Address! Params: " + model.ToJson());

            var userId = User.Identity.GetUserId();

            if (UserManager.Users.Any(u => u.Id != userId && u.Email == model.Email) && !string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Друг потребител използва този е-мейл адрес!");
            }

            if (!ModelState.IsValid)
            {
                _logger.Error("Adding Email Address Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }
            // Generate the token and send it
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            if (UserManager.EmailService != null)
            {
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = User.Identity.GetUserId(), code = token }, Request?.Url?.Scheme);
                var message = new IdentityMessage
                {
                    Destination = model.Email,
                    Subject = "Потвърди Емейл адрес",
                    Body = "Моля, потвърдете вашият е-мейл адрес като кликнете <a href=\"" + callbackUrl + "\">тук</a>"
                };
                await UserManager.EmailService.SendAsync(message);

                var user = await UserManager.FindByIdAsync(userId);
                user.Email = model.Email;
                user.EmailConfirmed = User.IsInRole("TeamMember");


                await UserManager.UpdateAsync(user);
            }
            return Json("STATUS_OK");
        }

        #endregion

        #region Edit Personal Info

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyPersonalData()
        {
            var userId = User.Identity.GetUserId();

            return Json(await UserManager.GetPersonalData(userId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// /Manage/EditPersonalData
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> EditPersonalData(EditUserPersonalInfoViewModel model)
        {
            _logger.Info("Editing Personal Data! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Editing Personal Data Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                await UserManager.EditPersonalData(model, User.Identity.GetUserId());
                _logger.Info("Editing Personal Data Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Editing Personal Data Failed!");
                throw;
            }
        }

        #endregion

        #region ChangeUserName

        //
        //GET: /Manage/ChangeUsername
        public ActionResult ChangeUsername()
        {
            return View(new ChangeUserNameViewModel
            {
                UserName = User.Identity.GetUserName()
            });
        }

        //
        //POST: /Manage/ChangeUsername
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUsername(ChangeUserNameViewModel model)
        {
            _logger.Info("Changing Username! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Changing Username Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.UserName = model.UserName;

                await UserManager.UpdateAsync(user);
                _logger.Info("Changing Username Successfully!");

                return Json(user.UserName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Changing Username Failed!");
                throw;
            }
        }

        #endregion


        //TODO: Make it possible
        #region Two-factor Authentication

        ////
        //// POST: /Manage/EnableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EnableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, false, false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        ////
        //// POST: /Manage/DisableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DisableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, false, false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        #endregion

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            _logger.Info("Adding Phonenumber! Params: " + model.ToJson());

            var userId = User.Identity.GetUserId();

            if (UserManager.Users.Any(u => u.Id != userId && u.PhoneNumber == model.Number) && !string.IsNullOrEmpty(model.Number))
            {
                ModelState.AddModelError(nameof(model.Number), "Друг потребител използва този телефонен номер!");
            }

            if (!ModelState.IsValid)
            {
                _logger.Error("Changing Phonenumber Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            //Uncomment this if the Clients has to verify his phonenumber
            //if (User.IsInRole("Client"))
            //{
            //    return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
            //}

            try
            {
                var teamUser = await UserManager.FindByIdAsync(userId);
                teamUser.PhoneNumber = model.Number;
                teamUser.PhoneNumberConfirmed = User.IsInRole("TeamMember");

                await UserManager.UpdateAsync(teamUser);
                _logger.Info("Adding Phonenumber Successfully!");

                //refresh claims
                await SignInManager.SignInAsync(teamUser, false, false);

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Adding Phonenumber Failed!");
                throw;
            }


        }

        //
        // GET: /Manage/VerifyPhoneNumber        
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            //Send Sms to Verify phone number
            if (UserManager.SmsService != null && !string.IsNullOrEmpty(phoneNumber))
            {
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
                await UserManager.SendSmsAsync(User.Identity.GetUserId(), code);
            }

            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, false, false);
                }
                return Json("STATUS_OK");
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Грешка при валидиране на номерът или кодът!");
            return Json(ModelState.ToDictionary());
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            _logger.Info("Removing Phone Number!");

            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, false, false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            _logger.Info("Changing Password! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Changing Password Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                _logger.Info("Changing Password Successfully!");

                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, false, false);
                }
                return Json("STATUS_OK");
            }

            AddErrors(result);
            _logger.Error("Changing Password Failed! Errors: " + ModelState.ToJson());

            return Json(ModelState.ToDictionary());
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/Images
        public async Task<ActionResult> Images()
        {
            var logedUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var images = logedUser.Images
                .Select(i => new UserImageViewModel
                {
                    ImageId = i.ImageId,
                    ImagePath = i.ImagePath
                }).ToList();


            return View(images);
        }

        //
        // POST: /Manage/RemoveImage
        [HttpPost]
        public async Task<ActionResult> RemoveImage(int id)
        {
            _logger.Info("Removing Image! ImageId: " + id);
            try
            {
                await _imageServices.DeleteImage(id, User.Identity.GetUserId());
                _logger.Info("Removing Image Successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Removing Image Failed!");
                throw;
            }

        }

        //
        // POST: /Manage/AddImage
        [HttpPost]
        public async Task<ActionResult> AddImage(HttpPostedFileBase image)
        {
            _logger.Info("Adding Image! Params: " + image.ToJson());

            try
            {
                var imageResult = (await _imageServices.CreateUserImages(new List<HttpPostedFileBase> { image }, User.Identity.GetUserId())).FirstOrDefault();


                if (imageResult != null)
                {
                    _logger.Info("Adding Image Successfully!");
                    return Json(new UserImageViewModel { ImageId = imageResult.ImageId, ImagePath = imageResult.ImagePath });
                }
                else
                {
                    throw new OperationCanceledException("Проблем, моля опитайте отново!");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Adding Image Failed!"); ;
                throw;
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return user?.PasswordHash != null;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return user?.PhoneNumber != null;
        }

        public enum ManageMessageId
        {
            ChangeUserNameSuccess,
            AddPhoneSuccess,
            EmailChangeSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}