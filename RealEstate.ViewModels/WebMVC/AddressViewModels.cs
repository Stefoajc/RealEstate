using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class AddressViewModels
    {
        
    }

    public class CreateAddressViewModel
    {
        [Required(ErrorMessage = "Градът е задължително поле")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Адресът е задължително поле")]
        public string FullAddress { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DetailsAddressViewModel
    {
        public string FullAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class CoordinatesViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}