using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public sealed class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Register two factor authentication providers. This application uses Phone and Emails as a 
            // step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            this.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Вашият код за сигурност е: {0}"
            });
            this.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Код за сигурност",
                BodyFormat = "Вашият код за сигурност е: {0}"
            });

            this.SmsService = new IdentitySmsService((ISmsService)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(ISmsService)));
            this.EmailService = new IdentityEmailService((IEmailService)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IEmailService)));

            var provider = new DpapiDataProtectionProvider("sProperties");
            this.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("RealEstate Token"));
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<RealEstateDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("RealEstate Token"));
            }
            return manager;
        }


        public async Task<Claim> AddUpdateClaim(Claim claim, string userId)
        {
            var claimToEdit = (await GetClaimsAsync(userId)).FirstOrDefault(c => c.Type == claim.Type);

            if (claimToEdit != null)
            {
                await RemoveClaimAsync(userId, claimToEdit);
            }

            await AddClaimAsync(userId, claim);

            return claim;
        }


        public async Task<SocialMediaAccountFullViewModel> AddSocialMediaAccount(SocialMediaAccountViewModel account,
            string userId)
        {
            var user = await FindByIdAsync(userId) ??
                       throw new ContentNotFoundException(
                           "Не е намерен потребителят на, който искате да добавите социална медия!");

            if (user.SocialMediaAccounts.Any(s => s.SocialMedia == account.SocialMedia))
            {
                throw new UniquenessException($"Вече имате въведен акаунт в {account.SocialMedia}");
            }

            var socialMediaAccount = new SocialMediaAccounts
            {
                SocialMedia = account.SocialMedia,
                SocialMediaAccount = account.SocialMediaAccount
            };

            user.SocialMediaAccounts.Add(socialMediaAccount);
            await UpdateAsync(user);

            return new SocialMediaAccountFullViewModel
            {
                Id = socialMediaAccount.Id,
                SocialMedia = socialMediaAccount.SocialMedia,
                SocialMediaAccount = socialMediaAccount.SocialMediaAccount
            };
        }

        public async Task DeleteSocialMediaAccount(int socialMediaId, string userId)
        {
            var user = await FindByIdAsync(userId) ??
                       throw new ContentNotFoundException(
                           "Не е намерен потребителят на, който искате да добавите социална медия!");

            var socialMedia = user.SocialMediaAccounts.FirstOrDefault(s => s.Id == socialMediaId);
            user.SocialMediaAccounts.Remove(socialMedia);
            await UpdateAsync(user);
        }

        public async Task<List<SocialMediaAccountFullViewModel>> GetSocialMediaAccounts(string userId)
        {
            var user = await FindByIdAsync(userId) ??
                       throw new ContentNotFoundException(
                           "Не е намерен потребителят на, който искате да добавите социална медия!");

            return user.SocialMediaAccounts.Select(s => new SocialMediaAccountFullViewModel
            {
                Id = s.Id,
                SocialMedia = s.SocialMedia,
                SocialMediaAccount = s.SocialMediaAccount
            }).ToList();
        }

        public async Task<EditUserPersonalInfoViewModel> GetPersonalData(string userId)
        {
            var userPersonalData = await Users
                .Where(u => u.Id == userId)
                .Select(u => new EditUserPersonalInfoViewModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    AdditionalInformation = u.AdditionalDescription
                }).FirstOrDefaultAsync();

            return userPersonalData;
        }

        public async Task<bool> EditPersonalData(EditUserPersonalInfoViewModel personalInfo, string userId)
        {
            var userToEdit = await FindByIdAsync(userId) ?? throw new ContentNotFoundException("Потребителят е изтрит или блокиран!");

            userToEdit.FirstName = personalInfo.FirstName;
            userToEdit.LastName = personalInfo.LastName;
            userToEdit.AdditionalDescription = personalInfo.AdditionalInformation;

            var result = await UpdateAsync(userToEdit);

            return result.Succeeded;
        }

        public async Task UpdateLastLoginByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Не въведен потребителски идентификационен номер!");
            }
            var user = await FindByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await UpdateAsync(user);
        }

        public async Task UpdateLastLoginByUserNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Не въведен потребителски идентификационен номер!");
            }
            var user = await FindByNameAsync(userName);
            user.LastActive = DateTime.Now;
            await UpdateAsync(user);
        }

        public async Task<bool> IsFirstLoginByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Не въведен потребителски идентификационен номер!");
            }
            var lastActive = await Users
                .Where(u => u.Id == userId)
                .Select(u => u.LastName)
                .FirstOrDefaultAsync();

            return lastActive == null;
        }


        public bool IsFirstLoginByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Не въведен потребителски идентификационен номер!");
            }
            var lastActive = Users
                .Where(u => u.UserName == userName)
                .Select(u => u.LastName)
                .FirstOrDefault();

            return lastActive == null;
        }

        public async Task<bool> IsFirstLoginByUserNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Не въведен потребителски идентификационен номер!");
            }
            var lastActive = await Users
                .Where(u => u.UserName == userName)
                .Select(u => u.LastName)
                .FirstOrDefaultAsync();

            return lastActive == null;
        }


        public bool HasPhoneNumber(string userId)
        {
            return Users.Where(u => u.Id == userId).Select(u => u.PhoneNumber).FirstOrDefault() != null;
        }

        public async Task<bool> HasPhoneNumberAsync(string userId)
        {
            return await Users.Where(u => u.Id == userId).Select(u => u.PhoneNumber).FirstOrDefaultAsync() != null;
        }
    }
}
