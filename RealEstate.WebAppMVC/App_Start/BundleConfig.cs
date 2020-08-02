using System.Web;
using System.Web.Optimization;

namespace RealEstate.WebAppMVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            bundles.Add(new ScriptBundle("~/bundles/shared")
                .Include(
                "~/Scripts/jquery-3.3.1.min.js",
                "~/Scripts/jquery.gmap.js",
                "~/Scripts/jquery.flexslider-min.js",
                "~/Scripts/modernizr-*",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/respond.min.js",
                "~/Scripts/alertify/alertify.min.js",
                "~/Scripts/theme/owlDefaultOptions.js",
                "~/Scripts/owl/old/owl.carousel.js",
                "~/Scripts/moment-with-locales.min.js",
                "~/Scripts/dateTimePicker/jquery.datetimepicker.full.min.js",
                "~/Scripts/modelstate-error-handling.js",
                "~/Scripts/UnselectableTags.js",
                "~/Scripts/extentions/extentions.js",
                "~/Scripts/paymentMethods.js",
                "~/Scripts/cookies/cookies-managing.js",
                "~/Scripts/unpkg/weakmap-polyfill.min.js",
                "~/Scripts/unpkg/formdata.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js",
                        "~/Scripts/jquery.gmap.js",
                        "~/Scripts/jquery.flexslider-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new StyleBundle("~/Content/alertify").Include(
                "~/Content/alertify/alertify.min.css",
                "~/Content/alertify/themes/semantic.min.css"));

            //bundles.Add(new ScriptBundle("~/bundles/owl")
            //    .Include("~/js/owl/owl.carousel.js"));

            //bundles.Add(new StyleBundle("~/Content/owl")
            //    .Include("~/css/owl/owl.carousel.css",
            //        "~/css/owl/ow.theme.default.css",
            //        "~/css/owl/ow.theme.green.css"));

            bundles.Add(new StyleBundle("~/Content/owl")
                .Include("~/Content/owl/old/owl.carousel.css",
                    "~/Content/owl/old/owl.theme.css"));


            bundles.Add(new StyleBundle("~/Content/shared")
                .Include("~/Content/bootstrap.min.css",
                    "~/Content/owl/old/owl.carousel.css",
                    "~/Content/owl/old/owl.theme.css",
                    "~/Content/alertify/alertify.min.css",
                    "~/Content/alertify/themes/semantic.min.css",
                    "~/Content/shared-compressed.css",
                    "~/Content/dateTimePicker/jquery.datetimepicker.min.css")
                .Include("~/Content/css/font-awesome.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/chosen/chosen.css", new CssRewriteUrlTransform())
                .Include(
                    "~/Content/theme/theme-all-compressed.css",
                    "~/Content/smartMenus/jquery.smartmenus.bootstrap.css"));
        }
    }
}
