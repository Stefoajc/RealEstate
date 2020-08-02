using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using RealEstate.Data;
using RealEstate.ViewModels.CustomDataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class AcountViewModels
    {
    }

    #region DataAnnotations

    public class IsUniqueEmail : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var email = (string) value;
            var dbContext = new RealEstateDbContext();

            return !dbContext.Users.Any(u => u.Email == email);
        }
    }

    public class IsUniqueUsername : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var username = (string)value;
            var dbContext = new RealEstateDbContext();

            return !dbContext.Users.Any(u => u.UserName == username);
        }
    }
    #endregion

    #region WebAppMVC_ViewModels

    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Въведе емейл адрес!")]
        [EmailAddress(ErrorMessage = "Въведете валиден емейл адрес!")]
        [IsUniqueEmail(ErrorMessage = "Емейл адресът вече се използва.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Въведете потребителско име!")]
        [IsUniqueUsername(ErrorMessage = "Това потребителско име вече е заето!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Въведете парола!")]
        [StringLength(100, ErrorMessage = "Паролата трябва да е поне {2} символа", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }
        
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Въведете емейл!")]
        [EmailAddress(ErrorMessage = "Невалиден формат на емейл адресът.")]
        [IsUniqueEmail(ErrorMessage = "Емейл адресът вече се използва.")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Въведете потребителско име!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Въведете парола!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Празен Recaptcha код!")]
        [ValidateRecaptcha(ErrorMessage = "Невалиден Recaptcha код!")]
        public string reCaptcha { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Въведете потребителско име!")]
        [IsUniqueUsername(ErrorMessage = "Това потребителско име вече е заето!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Въведете е-мейл!")]
        [EmailAddress(ErrorMessage = "Невалиден формат на емейл адресът.")]
        [IsUniqueEmail(ErrorMessage = "Емейл адресът вече се използва.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Въведете парола!")]
        [StringLength(100, ErrorMessage = "Паролата трябва да е поне {2} символа", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Празен Recaptcha код!")]
        [ValidateRecaptcha(ErrorMessage = "Невалиден Recaptcha код!")]
        public string reCaptcha { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Трябва да се съгласите със общите условия, за да създадете акаунт.")]
        public bool IsTermsAgreed { get; set; }

        public bool IsSubscribed { get; set; }
        // Gender type
    }


    public class RegisterAgent : RegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class RegisterOwnerViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "Въведете регистрационен код")]
        public string RegisterCode { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Емейлът е задължителен за въвеждане!")]
        [EmailAddress(ErrorMessage = "Невалиден формат на емейл адресът.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Паролата трябва да е поне {2} символа.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Паролите не съвпадат!")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Емейлът е задължителен за въвеждане!")]
        [EmailAddress(ErrorMessage = "Невалиден формат на емейл адресът.")]
        [IsUniqueEmail(ErrorMessage = "Емейл адресът вече се използва.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    #endregion


    #region Agents

    public class TeamUserListViewModel
    {
        public string AgentId { get; set; }
        public string FullName { get; set; }
        public ReviewsStarsPartialViewModel ReviewsInfo { get; set; }
        public string AdditionalDescription { get; set; }
        public string PhoneNumber { get; set; }
        public string OfficePhone { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }

        public List<SocialMediaAccountViewModel> SocialMediaAccounts { get; set; } = new List<SocialMediaAccountViewModel>();
    }

    #endregion


    #region Clients

    public class ClientListViewModel
    {
        public string ImagePath { get; set; }
        public string FullName { get; set; }
        public string AdditionalInformation { get; set; }
    }

    #endregion


    #region Owners

    public class OwnerCreateViewModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително.")]
        [IsUniqueUsername(ErrorMessage = "Това потребителско име вече е заето!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Паролата е задължителна за въвеждане.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Паролите не съвпадат!")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Името е задължително за въвеждане.")]
        public string FirstName { get; set; }
        //[Required(ErrorMessage = "Фамилията е задължителна за въвеждане.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Телефонът е задължителен за въвеждане.")]
        [Phone(ErrorMessage = "Невалиден телефонен номер!")]
        public string PhoneNumber { get; set; }
        //[Required(ErrorMessage = "Емейл адресът е задължителен за въвеждане.")]
        [EmailAddress(ErrorMessage = "Невалиден формат на емейл адресът.")]
        [IsUniqueEmail(ErrorMessage = "Емейл адресът вече се използва.")]
        public string Email { get; set; }

        public DateTime? BirthDate { get; set; }
    }

    public class OwnerViewModel
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
    #endregion

    public class UsersIdInfoViewModel
    {
        public string Id { get; set; }
        public string Info { get; set; }
    }

    public class RolesViewModel
    {
        public string Name { get; set; }
        public string Language { get; set; }
    }


    public class TeamMemberViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string ImageSrc { get; set; }
        public string AdditionalDescription { get; set; }
        public List<SocialMediaAccountViewModel> SocialMediaAccounts { get; set; } = new List<SocialMediaAccountViewModel>();
        public string FacebookAccount { get; set; }
        public string TwitterAccount { get; set; }
        public string SkypeAccount { get; set; }
    }

    public class SocialMediaAccountViewModel
    {
        [Required(ErrorMessage = "Изберете социална медия!")]
        public string SocialMedia { get; set; }
        [Required(ErrorMessage = "Въведете акаунт!")]        
        public string SocialMediaAccount { get; set; }
    }

    public class SocialMediaAccountFullViewModel
    {
        public int Id { get; set; }
        public string SocialMedia { get; set; }
        public string SocialMediaAccount { get; set; }
    }


    public class ContactInfoViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

}
