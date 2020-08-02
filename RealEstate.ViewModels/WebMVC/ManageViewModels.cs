using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using RealEstate.Data;

namespace RealEstate.ViewModels.WebMVC
{
    public class IndexViewModel
    {
        public string Id { get; set; }
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "Въведете парола!")]
        [StringLength(100, ErrorMessage = "Паролата трябва да е поне {2} символа.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Старата паролата е задължителна!")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Новата парола е задължителна!")]
        [StringLength(100, ErrorMessage = "Паролата трябва да е между {0} и {2} символа.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Въведете телефонен номер!")]
        [Phone]
        public string Number { get; set; }
    }

    public class AddEmailAddressViewModel
    {
        [Required(ErrorMessage = "Въведете Емейл адрес!")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ChangeUserNameViewModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително!")]
        [UniqueUsername(ErrorMessage = "Потребителското име е заето!")]
        public string UserName { get; set; }
    }

    public class UniqueUsernameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var dbContext = new RealEstateDbContext();
            var userName = (string)value;
            return !dbContext.Users.Any(u => u.UserName == userName);
        }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Въведете код!")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}