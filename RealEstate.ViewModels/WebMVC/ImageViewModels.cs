using System.Web;

namespace RealEstate.ViewModels.WebMVC
{
    public class ImageViewModels
    {
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
    }

    public class ImageCreateViewModel
    {
        public HttpPostedFileBase ImageFile { get; set; }
        public object ForeignKey { get; set; } // could be PropertyImageID,SightImageID
    }
}
