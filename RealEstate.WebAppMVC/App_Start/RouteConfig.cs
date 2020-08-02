using System.Web.Mvc;
using System.Web.Routing;
using Canonicalize;

namespace RealEstate.WebAppMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.Canonicalize().Www().Lowercase().NoTrailingSlash();
            routes.AppendTrailingSlash = false;
            routes.LowercaseUrls = true;

            routes.RouteExistingFiles = true;
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
