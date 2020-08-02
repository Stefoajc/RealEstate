using System.Collections.Generic;
using System.Web;
using RealEstate.ViewModels.CustomDataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class ImageViewModels
    {
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
    }

    public class ImageEditViewModel
    {
        public bool IsForSlider { get; set; }
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
    }

    public class ImageCreateViewModel
    {
        [EnsureOneItem]
        public List<HttpPostedFileBase> ImageFiles { get; set; }
        public string ForeignKey { get; set; } // could be PropertyImageID,SightImageID
    }

    public class UserImageViewModel
    {
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
    }

    public class ImageFileSystemDTO
    {
        public string ImageRelPath { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
    
}
