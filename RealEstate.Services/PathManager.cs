using System.IO;

namespace RealEstate.Services
{
    public static class PathManager
    {
        public static string CreateUserProfileImagePath(string userName, string imageName, string imageExtention = null)
        {
            var imageNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
            var fileExtention = imageExtention ?? Path.GetExtension(imageName);

            return Path.Combine("\\Resources", "Users", userName, "Profile", imageNameWithoutExtension + fileExtention);
        }

        public static string CreateUserPropertyImagePath(string userName, string imageName, string imageExtention = null, bool isForSlider = false)
        {
            var imageNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
            imageNameWithoutExtension = isForSlider ? imageNameWithoutExtension + "_Slider" : imageNameWithoutExtension;
            var fileExtention = imageExtention ?? Path.GetExtension(imageName);
            return Path.Combine("\\Resources", "Users", userName, "Properties", imageNameWithoutExtension + fileExtention);
        }

        public static string CreateSightImagePath(string sightName, string imageName, string imageExtention = null)
        {
            var imageNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
            var fileExtention = imageExtention ?? Path.GetExtension(imageName);
            return Path.Combine("\\Resources", "Sights", sightName, imageNameWithoutExtension + fileExtention);
        }

        public static string CreateCityImagePath(string cityName, string imageName, string imageExtention = null)
        {
            var imageNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
            var fileExtention = imageExtention ?? Path.GetExtension(imageName);
            return Path.Combine("\\Resources", "Cities", cityName, imageNameWithoutExtension + fileExtention);
        }

    }
}
