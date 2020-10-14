using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Ninject;
using Ninject.Web.Common.WebHost;
using RealEstate.Services;
using RealEstate.Services.AutoMapper;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Forum.AutoMapper;
using RealEstate.Services.Holidays;
using RealEstate.Services.Reports;
using RealEstate.WebAppMVC.AutoMapper;
using RealEstate.WebAppMVC.Controllers;
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
            Application[APPLICATION_CACHE] = HttpContext.Current.Cache;
            //If after 6AM trigger on next day at 6 if before 6AM trigger at 6AM (same day) and continue afterwards with 1 day cycle (every day at 6AM)
            var daylyActionsStart = DateTime.Now.Hour > 6
                ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0).AddDays(1)
                : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0);
            RegisterDaylyMorningCacheEntry(daylyActionsStart);

            //This ensures that the Actions will be triggered on whole hour (0 minutes)
            var hourlyActionsStart = DateTime.Now.Minute != 0
                ? DateTime.Now.AddHours(1).AddMinutes(-DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second)
                : DateTime.Now;

            RegisterHourlyCacheEntry(hourlyActionsStart);

            RegisterTwoMinuteCacheEntry();

            MvcHandler.DisableMvcResponseHeader = true;
            BundleTable.EnableOptimizations = true;

            //AutoMapper init
            Mapper.Initialize(x =>
            {
                AutoMapperServiceConfiguration.ConfigAction.Invoke(x);
                AutoMapperControllerConfiguration.ConfigAction.Invoke(x);
                AutoMapperForumConfiguration.ConfigAction.Invoke(x);
            });

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }



        protected void Application_Error(Object sender, EventArgs e)
        {
            // See http://stackoverflow.com/questions/13905164/how-to-make-custom-error-pages-work-in-asp-net-mvc-4
            // for additional context on use of this technique

            var exception = Server.GetLastError();
            if (exception != null)
            {
                Response.Clear();
                Server.ClearError();
                var routeData = new RouteData();
                routeData.Values["controller"] = "Errors";
                routeData.Values["action"] = "Index";
                routeData.Values["exception"] = exception;
                Response.StatusCode = 500;

                if (exception is HttpException)
                {
                    routeData.Values["action"] = "NotFound";
                }
                else if (exception is ContentNotFoundException || exception is UserNotFoundException)
                {
                    routeData.Values["action"] = "NotFound";
                }
                else if (exception is NotAuthorizedException)
                {
                    routeData.Values["action"] = "NotAuthorized";
                }
                else
                {
                    
                }
                
                routeData.Values["errorMessage"] = exception.Message;
                routeData.Values["errorStackTrace"] = Uri.EscapeDataString(exception.StackTrace);

                Response.TrySkipIisCustomErrors = true;
                IController errorsController = new ErrorsController();
                var rc = new RequestContext(new HttpContextWrapper(Context), routeData);

                /* This will run specific action without redirecting */
                errorsController.Execute(rc);

            }

            // This will invoke our error page, passing the exception message via querystring parameter
            // Note that we chose to use Server.TransferRequest, which is only supported in IIS 7 and above.
            // As an alternative, Response.Redirect could be used instead.
            // Server.Transfer does not work (see https://support.microsoft.com/en-us/kb/320439 )

            //if (exception is HttpException)
            //    {
            //        Server.TransferRequest("/Errors/NotFound", false);
            //    }
            //    else if (exception is ContentNotFoundException || exception is UserNotFoundException)
            //    {
            //        Server.TransferRequest("/Errors/NotFound?errorMessage=" + exception.Message + "&errorStackTrace=" + Uri.EscapeDataString(exception.StackTrace), false);
            //    }
            //    else if (exception is NotAuthorizedException)
            //    {
            //        Server.TransferRequest("/Errors/NotAuthorized?errorMessage=" + exception.Message + "&errorStackTrace=" + Uri.EscapeDataString(exception.StackTrace), false);
            //    }
            //    else if (exception is Exception)
            //    {
            //        Server.TransferRequest("/Errors/Index?errorMessage=" + exception.Message + "&errorStackTrace=" + Uri.EscapeDataString(exception.StackTrace), false);
            //    }

            //    // This is to stop a problem where we were seeing "gibberish" in the
            //    // chrome and firefox browsers
            //    HttpApplication app = sender as HttpApplication;
            //    app.Response.Filter = null;

        }




        private const string APPLICATION_CACHE = "ApplicationCache";

        #region Dayly Jobs

        private const string DAYLY_CACHE_ENTRY_KEY = "DaylyServiceCacheEntry";

        private void RegisterDaylyMorningCacheEntry(DateTime? expirationDate = null)
        {
            Cache daylyCache = (Cache)Application[APPLICATION_CACHE];
            if (daylyCache[DAYLY_CACHE_ENTRY_KEY] != null) return;
            if (expirationDate == null)
            {
                daylyCache.Add(DAYLY_CACHE_ENTRY_KEY, DAYLY_CACHE_ENTRY_KEY, null,
                    Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1), CacheItemPriority.Normal,
                    new CacheItemRemovedCallback(DaylyCacheItemRemoved));
            }
            else
            {
                daylyCache.Add(DAYLY_CACHE_ENTRY_KEY, DAYLY_CACHE_ENTRY_KEY, null,
                    (DateTime)expirationDate, Cache.NoSlidingExpiration , CacheItemPriority.Normal,
                    new CacheItemRemovedCallback(DaylyCacheItemRemoved));
            }
        }

        private void DaylyCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            //Run the actions
            SpawnDaylyServiceActions();
            //Re-enter the actions
            RegisterDaylyMorningCacheEntry();
        }

        //Spawn thread to run the action
        private void SpawnDaylyServiceActions()
        {
            Task.Run(async () => await TriggerDaylyActions());
        }

        private async Task TriggerDaylyActions()
        {
            var reportServices = (ReportServices)DependencyResolver.Current.GetService(typeof(ReportServices));
            await reportServices.NotifyForReports();

            var holidayServices = (HolidayServices)DependencyResolver.Current.GetService(typeof(HolidayServices));
            await holidayServices.NotifyForHolidays();

            var appointmentServices = (AppointmentServices)DependencyResolver.Current.GetService(typeof(AppointmentServices));
            await appointmentServices.NotifyForAppointments();

            var ownerServices = (OwnerServices)DependencyResolver.Current.GetService(typeof(OwnerServices));
            await ownerServices.NotifyForBirthdays();

        }

        #endregion
        
        #region Hourly Jobs

        private const string HOURLY_CACHE_ENTRY_KEY = "HourlyServiceCacheEntry";

        private void RegisterHourlyCacheEntry(DateTime? expirationDate = null)
        {
            Cache hourlyCache = (Cache)Application[APPLICATION_CACHE];
            if (hourlyCache[HOURLY_CACHE_ENTRY_KEY] != null) return;

            if (expirationDate == null)
            {
                hourlyCache.Add(HOURLY_CACHE_ENTRY_KEY, HOURLY_CACHE_ENTRY_KEY, null,
                    Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1), CacheItemPriority.Normal,
                    new CacheItemRemovedCallback(HourlyCacheItemRemoved));
            }
            else
            {
                hourlyCache.Add(HOURLY_CACHE_ENTRY_KEY, HOURLY_CACHE_ENTRY_KEY, null,
                    (DateTime)expirationDate, Cache.NoSlidingExpiration, CacheItemPriority.Normal,
                    new CacheItemRemovedCallback(HourlyCacheItemRemoved));
            }
        }

        private void HourlyCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            //Run the actions
            SpawnHourlyServiceActions();
            //Re-enter the actions
            RegisterHourlyCacheEntry();
        }

        //Spawn thread to run the action
        private void SpawnHourlyServiceActions()
        {
            Task.Run(async () => await TriggerHourlyActions());
        }

        private async Task TriggerHourlyActions()
        {
            //var mailSender = new NoReplyMailService();
            //await mailSender.SendEmailAsync("stefoajc@abv.bg", "Test Test", "Test Test", false);

        }

        #endregion

        #region Two Minute Jobs

        private const string TWO_MINUTE_CACHE_ENTRY_KEY = "TwoMinuteServiceCacheEntry";

        private void RegisterTwoMinuteCacheEntry()
        {
            Cache hourlyCache = (Cache)Application[APPLICATION_CACHE];
            if (hourlyCache[TWO_MINUTE_CACHE_ENTRY_KEY] != null) return;
            hourlyCache.Add(TWO_MINUTE_CACHE_ENTRY_KEY, TWO_MINUTE_CACHE_ENTRY_KEY, null,
                Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(2), CacheItemPriority.Normal,
                new CacheItemRemovedCallback(TwoMinuteCacheItemRemoved));
        }

        private void TwoMinuteCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            //Run the actions
            SpawnTwoMinuteServiceActions();
            //Re-enter the actions
            RegisterHourlyCacheEntry();
        }

        //Spawn thread to run the action
        private void SpawnTwoMinuteServiceActions()
        {
            Task.Run(async () => await TriggerTwoMinuteActions());
        }

        private async Task TriggerTwoMinuteActions()
        {
            
        }

        #endregion
    }
}
