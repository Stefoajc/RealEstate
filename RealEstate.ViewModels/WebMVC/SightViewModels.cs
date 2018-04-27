namespace RealEstate.ViewModels.WebMVC
{
    public class SightViewModels
    {
        
    }


    public class SightCreateViewModel
    {
        public int CityId { get; set; }

        //Coordinates
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //Sight Info
        public string SightName { get; set; }
        public string SightInfo { get; set; }
    }


    public class SightEditViewModel
    {
        public int SightId { get; set; }
        public int CityId { get; set; }

        //Coordinates
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //Sight Info
        public string SightName { get; set; }
        public string SightInfo { get; set; }
    }
}