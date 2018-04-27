using System;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace RealEstate.ViewModels.WebMVC
{
    public class ReservationViewModels
    {
        
    }

    public class CreateReservationViewModel
    {
        [BiggerThanNow]
        public DateTime From { get; set; }
        [GreaterThan("From")]
        public DateTime To { get; set; }
        public int RentalId { get; set; }
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
        public string PropertyName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }



    public class BiggerThanNowAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime from = Convert.ToDateTime(value);

            return from >= DateTime.Now;
        }
    }
}