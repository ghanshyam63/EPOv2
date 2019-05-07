using System.Web.Optimization;

namespace EPOv2
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/bootbox.min.js",        
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                 "~/Scripts/additional-methods.js",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap.js",
                     "~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-switch").Include(
                    "~/Scripts/bootstrap-switch/bootstrap-switch.js",
                    "~/Scripts/bootstrap-switch/main.js",
                    "~/Scripts/bootstrap-switch/highlight.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/docs.min.css",
                      "~/Content/site.css", 
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/bootstrap-datapicker.css",
                      "~/Content/bootstrap-datapicker3.css",
                      "~/Content/bootstrap-switch.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/themes/base/*.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
