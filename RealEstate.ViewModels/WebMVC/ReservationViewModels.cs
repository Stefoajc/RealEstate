using System;
using System.ComponentModel.DataAnnotations;
using Foolproof;
using RealEstate.Model;
using RealEstate.ViewModels.CustomDataAnnotations;
using RealEstate.ViewModels.WebMVC.Payments;

namespace RealEstate.ViewModels.WebMVC
{
    public class ReservationViewModels
    {

    }

    public class CreateReservationViewModel
    {
        [Required(ErrorMessage = "Начална дата на резервацията е задължителна!")]
        [BiggerThanNow(ErrorMessage = "Началната дата трябва да е по-голяма от днес!")]
        public DateTime? From { get; set; }
        [Required(ErrorMessage = "Крайна дата на резервацията е задължителна!")]
        [GreaterThan("From", ErrorMessage = "Крайната дата трябва да е по-голяма от началната")]
        public DateTime? To { get; set; }
        public string AdditionalDescription { get; set; }
        [Required(ErrorMessage = "Изберете имот, който искате да резервирате!")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Празен Recaptcha код!")]
        [ValidateRecaptcha(ErrorMessage = "Невалиден Recaptcha код!")]
        public string reCaptcha { get; set; }
    }

    public class CreateReservationForNonRegisteredUserViewModel
    {
        [Required(ErrorMessage = "Начална дата на резервацията е задължителна!")]
        [BiggerThanNow(ErrorMessage = "Началната дата трябва да е по-голяма от днес!")]
        public DateTime? From { get; set; }
        [Required(ErrorMessage = "Крайна дата на резервацията е задължителна!")]
        [GreaterThan("From", ErrorMessage = "Крайната дата трябва да е по-голяма от началната")]
        public DateTime? To { get; set; }
        public string AdditionalDescription { get; set; }
        [Required(ErrorMessage = "Изберете имот, който искате да резервирате!")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Въведете име!")]
        public string ClientName { get; set; }
        [Required(ErrorMessage = "Въведете телефон за връзка!")]
        public string ClientPhoneNumber { get; set; }
        public string ClientEmail { get; set; }

        [Required(ErrorMessage = "Празен Recaptcha код!")]
        [ValidateRecaptcha(ErrorMessage = "Невалиден Recaptcha код!")]
        public string reCaptcha { get; set; }
    }

    public class CreateReservationDTO
    {
        [BiggerThanNow(ErrorMessage = "Началната дата трябва да е по-голяма от днес!")]
        public DateTime From { get; set; }
        [GreaterThan("From", ErrorMessage = "Крайната дата трябва да е по-голяма от началната")]
        public DateTime To { get; set; }
        public string AdditionalDescription { get; set; }
        [Required(ErrorMessage = "Изберете имот, който искате да резервирате!")]
        public int PropertyId { get; set; }

        //For Registered User
        public string UserId { get; set; }

        //For NonReg Users
        public NonRegisteredUserCreateDTO NonRegisteredUser { get; set; }
    }

    public class EditReservationViewModel
    {
        public int ReservationId { get; set; }
        [BiggerThanNow]
        public DateTime From { get; set; }
        [GreaterThan("From")]
        public DateTime To { get; set; }
        public int RentalId { get; set; }
    }

    public class ListReservationViewModel
    {
        public int ReservationId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string PaymentStatus { get; set; }
        public decimal CaparoPrice { get; set; }
        public decimal FullPrice { get; set; }

        public PropertyInfoViewModel Property { get; set; }
    }

    public class ListPropertyReservationsViewModel
    {
        public int? PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string RentalType { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class ReservationDetailsViewModel
    {
        public int? PropertyId { get; set; }
        public string PropertyImage { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string ReservationStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal CaparoParice { get; set; }
        public decimal Price { get; set; }
    }


    public class BiggerThanNowAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime from = Convert.ToDateTime(value);

            return from >= DateTime.Now;
        }
    }


    public class ReservationInfoDTO
    {
        public int ReservationId { get; set; }
        public string ClientUserId { get; set; }
        public int? PropertyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ApproveStatus ApproveStatus { get; set; }
        public decimal CaparoPrice { get; set; }
        public decimal FullPrice { get; set; }
    }


    public class ReservationPaymentViewModel
    {
        public int Id { get; set; }
        public bool IsCaparoPayed { get; set; }
        public PaymentMethodInfoViewModel PaymentInfo { get; set; }
        public string UrlOk { get; set; }
        public string UrlCancel { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Трябва да се съгласите с условията за да продължите!")]
        public bool AreTermsAgreed { get; set; }
    }
}