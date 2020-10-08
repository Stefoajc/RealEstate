using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Model;
using RealEstate.Services;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.Services.MailList;
using RealEstate.ViewModels.WebMVC;
using RealEstate.WebAppMVC.Helpers;

namespace RealEstate.WebAppMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private OwnerRegisterCodeServices OwnerRegisterCodeManager { get; }
        private IEmailService EmailManager { get; }
        private UserServices UserServices { get; }
        private MailListServices MailListManager { get; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public AccountController(IEmailService emailService, OwnerRegisterCodeServices ownerRegisterCodeServices, UserServices userServices, MailListServices mailListManager)
        {
            EmailManager = emailService;
            OwnerRegisterCodeManager = ownerRegisterCodeServices;
            UserServices = userServices;
            MailListManager = mailListManager;
        }


        [Inject]
        public ApplicationSignInManager SignInManager { get; set; }
        [Inject]
        public ApplicationUserManager UserManager { get; set; }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string confirmation)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            _logger.Info("User is Logging in! Params: " + new { model.UserName, model.RememberMe, model.reCaptcha }.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("User logging form Invalid! Errors: " + ModelState.ToJson());
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                case SignInStatus.RequiresVerification:
                    _logger.Info("User logged in Successfully!");

                    if (await UserManager.IsFirstLoginByUserNameAsync(model.UserName))
                    {
                        //Redirect to setup account
                        await UserManager.UpdateLastLoginByUserNameAsync(model.UserName);
                        return RedirectToAction("Index", "Manage");
                    }

                    await UserManager.UpdateLastLoginByUserNameAsync(model.UserName);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                case SignInStatus.Failure:
                    ModelState.AddModelError(string.Empty, "Потребителското име или паролата са неверни!");
                    return View();
                default:
                    ModelState.AddModelError("", "Невалиден логин опит. Опитайте по-късно!");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("/Views/Errors/Index.cshtml");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                    throw new NotImplementedException("Failure");
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            _logger.Info("User submitted registration Form! Params: " + model.ToJson());

            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ClientUsers { UserName = model.UserName.Trim(' '), Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        _logger.Info("User registered Successfully!");

                        //Send notification to office mail
                        await EmailNotifications.RegiteredUserNotification(user.UserName, user.Email, user.PhoneNumber);

                        //Subscribe him if checked
                        if (model.IsSubscribed)
                        {
                            await MailListManager.Subscribe(model.Email);
                        }

                        var currentUser = await UserManager.FindByNameAsync(user.UserName);
                        await UserManager.AddToRoleAsync(currentUser.Id, "Client");
                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        if (Request.Url != null)
                        {
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code },
                                Request.Url.Scheme);
                            await EmailNotifications.NoReplyMailService.SendHtmlEmailAsync(user.Email,
                                "Потвърдете акаунтът си в sProperties",
                                "За да потвърдите регистрацията си натиснете <a href=\"" + callbackUrl + "\">ТУК</a>");
                        }

                        return RedirectToAction("Login", "Account", new { confirmation = "email" });
                    }
                    AddErrorsToModelState(result);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Something went wrong during the registration!");
                }
            }

            _logger.Error("User registration form Invalid! Errors: " + ModelState.ToJson());

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Maintenance")]
        public ActionResult RegisterAgent()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RegisterAgent(RegisterAgent model)
        {
            _logger.Info("Registering Agent Request! Params: " + model.ToJson());

            ModelState.Remove(nameof(model.IsTermsAgreed));
            ModelState.Remove(nameof(model.reCaptcha));

            if (!ModelState.IsValid)
            {
                _logger.Error("Registering Agent Form Invalid! Errors:" + ModelState.ToJson());

                return View(model);
            }

            var role = "Agent";


            var result = await UserServices.RegisterTeamUser(model, role);

            if (result)
            {
                _logger.Info("Registration of Agent Successfully!");
                return RedirectToAction("Index", "Home");
            }


            _logger.Error("Registration of Agent Failed due to Service Error!");
            ModelState.AddModelError("", "Грешка при създаване на потребител!");
            return View(model);
        }

        //GET: Account/RegisterAgent
        [Authorize(Roles = "Administrator")]
        public ActionResult RegisterTeamUser()
        {
            ViewBag.Roles = UserServices.GetRoles("bg");
            return View();
        }

        [Authorize(Roles = "Administrator"), HttpPost]
        public async Task<ActionResult> RegisterTeamUser(RegisterAgent model, string role)
        {

            if (!UserServices.DoesRoleExist(role))
            {
                return HttpNotFound("The Role does not exist");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = UserServices.GetRoles("bg");
                return View(model);
            }

            var result = await UserServices.RegisterTeamUser(model, role);

            if (result)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Error Creating User");
            return View(model);
        }

        //GET: Account/RegisterOwner
        [AllowAnonymous]
        public ActionResult RegisterOwner()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> RegisterOwner(RegisterOwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Checking the register code in the db and delete the code if exists
                if (OwnerRegisterCodeManager.CheckOwnerRegisterCode(model.RegisterCode))
                {
                    var owner = new OwnerUsers { UserName = model.UserName.Trim(' '), Email = model.Email };
                    var result = await UserManager.CreateAsync(owner, model.Password);
                    if (result.Succeeded)
                    {
                        //Send notification to office mail
                        await EmailNotifications.RegiteredUserNotification(owner.UserName, owner.Email, owner.PhoneNumber);

                        var currentUser = await UserManager.FindByNameAsync(owner.UserName);
                        await UserManager.AddToRoleAsync(currentUser.Id, "Owner");
                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(owner.Id);
                        if (Request.Url != null)
                        {
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = owner.Id, code }, Request.Url.Scheme);
                            await EmailNotifications.NoReplyMailService.SendHtmlEmailAsync(owner.Email, "Потвърдете акаунтът си в sProperties", "За да потвърдите регистрацията си натиснете <a href=\"" + callbackUrl + "\">ТУК</a>");
                        }

                        return RedirectToAction("Login", "Account", new { confirm = "email" });
                    }
                    AddErrorsToModelState(result);
                }
                else
                {
                    ModelState.AddModelError("RegisterCode", "Invalid Register Code");
                    return View();
                }
            }

            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "/Views/Errors/Index.cshtml");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    throw new UserNotFoundException("Потребител с този емейл не съществува в системата");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await EmailNotifications.NoReplyMailService.SendHtmlEmailAsync(user.Id, "Забравена парола в sproperties.net", "Моля променете паролата си като кликнете <a href=\"" + callbackUrl + "\">ТУК</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("/Views/Errors/Index.cshtml") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrorsToModelState(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            _logger.Info("User Issued External Login!");
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("/Views/Errors/Index.cshtml");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("/Views/Errors/Index.cshtml");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            _logger.Info("User Returned From provider!");

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                _logger.Error("User Information Could Not be Fetched From External login!");
                return RedirectToAction("Login");
            }

            _logger.Info("User trying to log in with " + loginInfo.Login.LoginProvider);
            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, false);
            switch (result)
            {
                case SignInStatus.Success:
                    _logger.Info("User has account in the system and is logged in successfully!");

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    _logger.Error("User account is locked out!");

                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    _logger.Info("User account requires verification Redirect to Account/SendCode!");
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                default:

                    _logger.Info("User does not have account in the system and will be promped to create one with external information!");

                    //Twitter does not provide Email by default we have to make separate request
                    if (loginInfo.Login.LoginProvider.ToLower() == "twitter")
                    {
                        string accessToken = loginInfo.ExternalIdentity.Claims.Where(x => x.Type == "urn:twitter:access_token").Select(x => x.Value).FirstOrDefault();
                        string accessSecret = loginInfo.ExternalIdentity.Claims.Where(x => x.Type == "urn:twitter:access_secret").Select(x => x.Value).FirstOrDefault();
                        TwitterDto response = await TwitterHelpers.TwitterLogin(accessToken, accessSecret, ConfigurationManager.AppSettings["TwitterClientID"], ConfigurationManager.AppSettings["TwitterClientSecret"]);
                        // by now response.email should possess the email value you need
                        loginInfo.Email = response.Email;
                    }

                    if (!string.IsNullOrEmpty(loginInfo.Email))
                    {
                        var userWithThisEmail = await UserManager.FindByEmailAsync(loginInfo.Email);
                        //When there is user with this email in the system
                        if (userWithThisEmail != null)
                        {
                            await SignInManager.SignInAsync(userWithThisEmail, false, false);
                            return RedirectToLocal(returnUrl);
                        }
                    }

                    // Commented for now, because it can be changed in the SocialMedia and be used to log
                    // in someone elses account which is not acceptable
                    #region Find and log by username 

                    //if (!string.IsNullOrEmpty(loginInfo.DefaultUserName))
                    //{
                    //    var userWithThisUsername = await UserManager.FindByNameAsync(loginInfo.DefaultUserName);
                    //    //When there is user with this userName in the system
                    //    if (userWithThisUsername != null)
                    //    {
                    //        await SignInManager.SignInAsync(userWithThisUsername, false, false);
                    //        return RedirectToLocal(returnUrl);
                    //    }
                    //}

                    #endregion


                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel
                        {
                            UserName = loginInfo.DefaultUserName,
                            Email = loginInfo.Email
                        });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.Info("User is authenticated and will be redirected to Manage/Index");
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ClientUsers { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.Info("User registered successfully with external login!");
                    //Send notification to office mail
                    await EmailNotifications.RegiteredUserNotification(user.UserName, user.Email, user.PhoneNumber);

                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrorsToModelState(result);
            }

            _logger.Error("Problem in external login Form! Errors: " + ModelState.ToJson());

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _logger.Info("User is logging out!");
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //
        // GET: /Account/CreateOwnerCode
        [HttpGet]
        [Authorize(Roles = "Maintenance")]
        public ActionResult CreateOwnerCode()
        {
            var code = OwnerRegisterCodeManager.AddOwnerRegisterCode();
            return Json(code, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserManager != null)
                {
                    UserManager.Dispose();
                    UserManager = null;
                }

                if (SignInManager != null)
                {
                    SignInManager.Dispose();
                    SignInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri, string userId = null)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            private string LoginProvider { get; }
            private string RedirectUri { get; }
            private string UserId { get; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        [Authorize(Roles = "Agent, Administrator, Maintenance")]
        [HttpPost]
        public async Task<ActionResult> CreateOwner(OwnerCreateViewModel model)
        {
            _logger.Info("Creating Owner Account! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Creating Owner Account Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            var owner = new OwnerUsers
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                PhoneNumberConfirmed = true,
                BirthDate = model.BirthDate
            };

            var result = await UserManager.CreateAsync(owner, model.Password);
            if (result.Succeeded)
            {
                _logger.Info("Owner Created Successfully!");
                //Send notification to office mail
                await EmailNotifications.RegiteredUserNotification(owner.UserName, owner.Email, owner.PhoneNumber);

                await UserManager.AddToRoleAsync(owner.Id, "Owner");

                return Json(new UsersIdInfoViewModel
                {
                    Id = owner.Id,
                    Info = "Име: " + owner.FirstName + " " + (owner.LastName ?? "") + ", Телефон: " + owner.PhoneNumber
                });
            }
            else
            {
                AddErrorsToModelState(result);
                _logger.Info("Owner Creation Failed! Errors: " + ModelState.ToJson());

                return Json(ModelState.ToDictionary());
            }
        }

    }
}