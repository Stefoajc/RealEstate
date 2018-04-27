using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class AddRentalPriceForPeriodViewModel
    {
        public string Period { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal Price { get; set; }
    }

    public class EditRentalPriceForPeriodForPropertyViewModel
    {
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal Price { get; set; }
    }

    public class CreateRentalPriceForPeriodViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal Price { get; set; }

        [Required]
        public int RentalId { get; set; }
    }
}