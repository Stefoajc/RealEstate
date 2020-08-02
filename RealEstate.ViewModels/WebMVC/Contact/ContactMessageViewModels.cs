using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Contact
{
    public class ContactMessageViewModel
    {
        [Required(ErrorMessage = "Въведете име!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Въведете е-поща!")]
        [EmailAddress(ErrorMessage = "Въведете правилна е-поща!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Въведете заглавие!")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Въведете телефонен номер!")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Въведете съобщение!")]
        public string Message { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Трябва да се съгласите със общите условие!")]
        public bool AreTermsAgreed { get; set; }
    }
}
