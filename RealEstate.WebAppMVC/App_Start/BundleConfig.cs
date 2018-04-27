using System.Web;
using System.Web.Optimization;

namespace RealEstate.WebAppMVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/js/jquery/jquery.min.js",
                        "~/js/jquery/jquery.gmap.js",
                        "~/js/jquery/jquery.flexslider-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/js/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modelstate-errors-handling").Include(
                        "~/js/modelstate-error-handling.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/js/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/js/bootstrap/bootstrap.min.js",
                      "~/js/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/css/bootstrap/bootstrap.css",
                      "~/css/css.css"));

            //bundles.Add(new ScriptBundle("~/bundles/owl")
            //    .Include("~/js/owl/owl.carousel.js"));

            //bundles.Add(new StyleBundle("~/Content/owl")
            //    .Include("~/css/owl/owl.carousel.css",
            //        "~/css/owl/ow.theme.default.css",
            //        "~/css/owl/ow.theme.green.css"));

            bundles.Add(new ScriptBundle("~/bundles/owl")
                .Include("~/js/owl/old/owl.carousel.js"));

            bundles.Add(new StyleBundle("~/Content/owl")
                .Include("~/css/owl/old/owl.carousel.css",
                    "~/css/owl/old/owl.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapDatePicker")
                .Include("~/js/bootstrap-datepicker.min.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrapDatePicker")
                .Include("~/css/bootstrapDateTimePicker/bootstrap-datepicker.min.css",
                "~/css/bootstrapDateTimePicker/bootstrap-datepicker3.min.css"));
        }
    }
}
