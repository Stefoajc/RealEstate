using System.IO;

namespace RealEstate.Services
{
    public static class PathManager
    {
        public static string CreateUserProfileImagePath(string userName, string imageName)
        {
            imageName = Path.GetFileName(imageName);
            return Path.Combine("\\Resources", "Users", userName, "Profile", imageName);
        }

        public static string CreateUserPropertyImagePath(string userName, string imageName)
        {
            imageName = Path.GetFileName(imageName);
            return Path.Combine("\\Resources", "Users", userName, "Properties", imageName);
        }

        public static string CreateSightImagePath(string sightName, string imageName)
        {
            imageName = Path.GetFileName(imageName);
            return Path.Combine("\\Resources", "Sights", sightName, imageName);
        }

        public static string CreateCityImagePath(string cityName, string imageName)
        {
            imageName = Path.GetFileName(imageName);
            return Path.Combine("\\Resources", "Cities", cityName, imageName);
        }
    }
}
