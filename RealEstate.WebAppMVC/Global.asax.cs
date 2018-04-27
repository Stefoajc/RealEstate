using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Ninject;
using Ninject.Web.Common.WebHost;
using RealEstate.Services.AutoMapper;
using RealEstate.WebAppMVC.AutoMapper;
using RealEstate.WebAppMVC.Ninject;

namespace RealEstate.WebAppMVC
{
    public class MvcApplication : NinjectHttpApplication
    {

        protected override IKernel CreateKernel()
        {
            return new StandardKernel(new NinjectBindings());
        }

        protected override void OnApplicationStarted()
        {
            //AutoMapper init
            Mapper.Initialize(x =>
            {
                AutoMapperServiceConfiguration.ConfigAction.Invoke(x);
                AutoMapperControllerConfiguration.ConfigAction.Invoke(x);
            });

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
