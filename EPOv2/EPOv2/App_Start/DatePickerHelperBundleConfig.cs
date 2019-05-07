using System.Web.Optimization;

//[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(EPOv2.App_Start.DatePickerHelperBundleConfig), "RegisterBundles")]

namespace EPOv2.App_Start
{
	public class DatePickerHelperBundleConfig
	{
		public static void RegisterBundles()
		{
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
            "~/Scripts/bootstrap-datepicker.js",
            "~/Scripts/locales/bootstrap-datepicker.*"));

            BundleTable.Bundles.Add(new StyleBundle("~/Content/datepicker").Include(
            "~/Content/bootstrap-datepicker.css"));
		}
	}
}