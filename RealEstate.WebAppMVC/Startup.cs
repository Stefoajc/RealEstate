using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RealEstate.WebAppMVC.Startup))]
namespace RealEstate.WebAppMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
