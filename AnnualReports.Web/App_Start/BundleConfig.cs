using System.Web.Optimization;

namespace AnnualReports.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                       // AngularJs Core modules/services/directives.
                       "~/Scripts/angular.js",
                       "~/Scripts/angular-sanitize.js",
                       "~/Scripts/angular-animate.js",
                       "~/Scripts/angular-touch.js",
                       "~/Scripts/ng-plugins/angular-eonasdan-datetimepicker.js",
                       "~/Scripts/ng-plugins/select.js",
                       "~/Scripts/ng-plugins/angular-block-ui.js",
                       "~/Scripts/ui-grid.js"
                       ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                        "~/Scripts/moment.js",
                        "~/Scripts/moment-with-locales.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        //"~/Scripts/loadingoverlay.js",
                        //"~/Scripts/toastr.js",
                        "~/Scripts/select2.js",
                        "~/Scripts/app/common.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //"~/Content/toastr.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/select.css",
                      "~/Content/angular-block-ui.css",
                      "~/Content/ui-grid.css",
                      "~/Content/site.css"));
        }
    }
}