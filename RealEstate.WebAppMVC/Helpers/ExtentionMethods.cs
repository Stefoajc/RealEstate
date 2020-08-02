using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using System.Web.UI;
using Newtonsoft.Json;

namespace RealEstate.WebAppMVC.Helpers
{
    public class ExtentionMethods
    {
    }

    public static class ViewExtensions
    {
        public static string RenderToString(this PartialViewResult partialView)
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
            }

            var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();

            var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, controllerName);

            var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }
    }

    public static class ViewHelper
    {
        public static string RenderViewToString(this Controller controller, string viewName, object model)
        {
            using (var writer = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                controller.ViewData.Model = model;
                var viewCxt = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, writer);
                viewCxt.View.Render(viewCxt, writer);
                return writer.ToString();
            }
        }
    }

    public static class AjaxHelperExtensions
    {
        public static MvcHtmlString ActionLinkWithCollectionModel(this AjaxHelper ajaxHelper, string linkText, string actionName, object model, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            var rv = new RouteValueDictionary();

            foreach (var property in model.GetType().GetProperties())
            {
                if (typeof(ICollection).IsAssignableFrom(property.PropertyType))
                {
                    var s = ((IEnumerable<object>)property.GetValue(model));
                    if (s != null && s.Any())
                    {
                        var values = s.Select(p => p.ToString()).Where(p => !string.IsNullOrEmpty(p)).ToList();
                        for (var i = 0; i < values.Count(); i++)
                            rv.Add(string.Concat(property.Name, "[", i, "]"), values[i]);
                    }
                }
                else
                {
                    var value = property.GetGetMethod().Invoke(model, null) == null ? "" : property.GetGetMethod().Invoke(model, null).ToString();
                    if (!string.IsNullOrEmpty(value))
                        rv.Add(property.Name, value);
                }
            }
            return System.Web.Mvc.Ajax.AjaxExtensions.ActionLink(ajaxHelper, linkText, actionName, rv, ajaxOptions, htmlAttributes);
        }

        public static string HrefWithList(this UrlHelper helper, string action, string controller, object routeData)
        {
            string href = helper.Action(action, controller);

            if (routeData != null)
            {
                RouteValueDictionary rv = new RouteValueDictionary(routeData);
                List<string> urlParameters = new List<string>();
                foreach (var key in rv.Keys)
                {
                    object value = rv[key];
                    if (value is IEnumerable && !(value is string))
                    {
                        int i = 0;
                        foreach (object val in (IEnumerable)value)
                        {
                            urlParameters.Add(string.Format("{0}[{2}]={1}", key, val, i));
                            ++i;
                        }
                    }else if (value is DateTime)
                    {
                        urlParameters.Add(string.Format("{0}={1}", key, ((DateTime)value).ToString("dd.MM.yyyy")));
                    }
                    else if (value != null)
                    {
                        urlParameters.Add(string.Format("{0}={1}", key, value));
                    }
                }
                string paramString = string.Join("&", urlParameters.ToArray()); // ToArray not needed in 4.0
                if (!string.IsNullOrEmpty(paramString))
                {
                    href += "?" + paramString;
                }
            }

            return href;
        }

        public static string ActionLinkWithList(this HtmlHelper helper, string text, string action, string controller, object routeData, object htmlAttributes)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);


            string href = urlHelper.Action(action, controller);

            if (routeData != null)
            {
                RouteValueDictionary rv = new RouteValueDictionary(routeData);
                List<string> urlParameters = new List<string>();
                foreach (var key in rv.Keys)
                {
                    object value = rv[key];
                    if (value is IEnumerable && !(value is string))
                    {
                        int i = 0;
                        foreach (object val in (IEnumerable)value)
                        {
                            urlParameters.Add(string.Format("{0}[{2}]={1}", key, val, i));
                            ++i;
                        }
                    }
                    else if (value != null)
                    {
                        urlParameters.Add(string.Format("{0}={1}", key, value));
                    }
                }
                string paramString = string.Join("&", urlParameters.ToArray()); // ToArray not needed in 4.0
                if (!string.IsNullOrEmpty(paramString))
                {
                    href += "?" + paramString;
                }
            }

            TagBuilder builder = new TagBuilder("a");
            builder.Attributes.Add("href", href);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            builder.SetInnerText(text);
            return builder.ToString(TagRenderMode.Normal);
        }
    }

    public static class ObjectExtensions
    {
        /// <summary>
        /// The string representation of null.
        /// </summary>
        private static readonly string Null = "null";

        /// <summary>
        /// The string representation of exception.
        /// </summary>
        private static readonly string Exception = "Exception";

        /// <summary>
        /// To json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The Json of any object.</returns>
        public static string ToJson(this object value)
        {
            if (value == null) return Null;

            try
            {
                string json = JsonConvert.SerializeObject(value);
                return json;
            }
            catch (Exception exception)
            {
                //log exception but dont throw one
                return Exception;
            }
        }
    }
}