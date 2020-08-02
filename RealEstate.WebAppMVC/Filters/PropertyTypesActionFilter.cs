using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using RealEstate.Repositories;

namespace RealEstate.WebAppMVC.Filters
{
    public class PropertyTypesActionFilter : ActionFilterAttribute
    {
        public override async void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //if (filterContext.Controller.ViewBag.PropertyTypes == null)
            //{
            //    var unitOfWork = (UnitOfWork)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(UnitOfWork));

            //    var propertyTypes = (await unitOfWork.PropertiesRepository.ListPropertyTypes())
            //        .Select(pt => new { pt.PropertyTypeId, pt.PropertyTypeName })
            //        .ToList();

            //    filterContext.Controller.ViewBag.HouseTypeId = propertyTypes
            //        .Where(pt => pt.PropertyTypeName == "Къща")
            //        .Select(pt => pt.PropertyTypeId)
            //        .FirstOrDefault();

            //    filterContext.Controller.ViewBag.HouseTypeId = propertyTypes
            //        .Where(pt => pt.PropertyTypeName == "Апарта")
            //        .Select(pt => pt.PropertyTypeId)
            //        .FirstOrDefault();
            //}
        }
    }
}