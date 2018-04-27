using System.Data.Entity;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Ninject.Modules;
using Ninject.Web.Common;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services;
using RealEstate.Services.Interfaces;

namespace RealEstate.WebAppMVC.Ninject
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<OwnerRegisterCodeServices>().ToSelf().InRequestScope();
            Bind<OwnerServices>().ToSelf().InRequestScope();
            Bind<AddressServices>().ToSelf().InRequestScope();
            Bind<ExtraServices>().ToSelf().InRequestScope();
            Bind<RentalInfoServices>().ToSelf().InRequestScope();
            Bind<ImageServices>().ToSelf().InRequestScope();
            Bind<PropertiesServices>().ToSelf().InRequestScope();
            Bind<IEmailService>().To<GmailMailService>().InRequestScope();
            Bind<CityServices>().ToSelf().InRequestScope();

            Bind<DbContext>().To<RealEstateDbContext>().InRequestScope();
            Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
            Bind<IPrincipal>()
                .ToMethod(context => HttpContext.Current.User)
                .InRequestScope();

            Bind<IAuthenticationManager>().ToMethod(
                c =>
                    HttpContext.Current.GetOwinContext().Authentication).InRequestScope();

            Bind(typeof(IUserStore<ApplicationUser>)).To(typeof(UserStore<ApplicationUser>)).InRequestScope();
            Bind(typeof(UserManager<ApplicationUser>)).ToSelf().InRequestScope();

        }
    }
}