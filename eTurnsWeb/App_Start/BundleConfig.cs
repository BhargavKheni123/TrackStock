using System.Web.Optimization;

namespace eTurnsWeb
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {


            bundles.Add(new ScriptBundle("~/bundles/Scripts").Include(
                        "~/Scripts/json2.js",
                        "~/Scripts/jquery-1.7.1.js",
                        "~/Scripts/knockout-3.2.0.js",
                        "~/Scripts/knockout.mapping-latest.js",
                        //"~/Scripts/knockout.validation-2.0.0.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery-ui-1.8.20.js",
                        "~/Scripts/jquery.ui.touch-punch.js",
                        "~/Scripts/jquery.dataTables.js",
                        "~/Scripts/jquery.jeditable.js",
                        "~/Scripts/jquery.dataTables.editable.js",
                        //"~/Scripts/ColReorder.js",
                        "~/Scripts/ColReorderWithResize.js",
                        "~/Scripts/ColVis.js",
                        "~/Scripts/autoNumeric.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/wrappers.js",
                        "~/Scripts/PageScripts/NarrowSearchSave.js",
                        "~/Scripts/Common.js",
                        "~/Scripts/CustumValidations.js",
                        "~/Scripts/jquery.rotate.js",
                        "~/Scripts/css_browser_selector.js",
                        "~/Scripts/simplemodal.js",
                        "~/Scripts/TableTools.js",
                        "~/Scripts/ZeroClipboard.js",
                        "~/Scripts/MultiSelect/jquery.multiselect.js",
                        "~/Scripts/MultiSelect/prettify.js",
                        "~/Scripts/MultiSelect/jquery.multiselect.filter.js",
                        "~/Scripts/jquery.textchange.js",
                        "~/Scripts/jquery.idletimer.js",
                        "~/Scripts/jquery.idletimeout.js",
                        "~/Scripts/easytimer.js",
                        //"~/Scripts/jquery.signalR-2.0.0",
                        //"~/signalr/hubs",
                        "~/Scripts/stringExtensions.js",
                        "~/Scripts/knockoutSessionTimeout.js",
                        "~/Scripts/BarcodeReader.js"
                        //"~/Scripts/modernizr-2.6.2.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/ScriptsNG").Include(
                        "~/ScriptNG/angular.js",
                        "~/ScriptNG/angular-aria.js",
                        "~/ScriptNG/angular-route.js",
                        "~/ScriptNG/angular-sanitize.js",
                        "~/ScriptNG/angular-touch.js",
                        "~/ScriptNG/AngularUI/ui-router.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/pullMasterList")
                .Include("~/Scripts/Authorization.js")
                .Include("~/Scripts/DynemicDataTable.js")
                .Include("~/Scripts/TabCommon.js")
                .Include("~/Scripts/PageScripts/PullMasterList.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/toolMasterList").Include(
                "~/Scripts/TabCommon.js",
                "~/Scripts/Authorization.js",
                "~/Scripts/PageScripts/ToolPage.js",
                "~/Scripts/PageScripts/ToolList.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/AlertDashboard")
                .Include("~/Scripts/jquery.slidepanel.js")
                .Include("~/Scripts/Dashboard/simple-slider.js")
                .Include("~/Scripts/Dashboard/interface.js")
                .Include("~/Scripts/dataTables.scroller.js")
                .Include("~/Scripts/Dashboard/Dashboard.js")
                );

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/reset.css",
                "~/Content/site.css",
                "~/Content/dataTables/ColVis.css",
                "~/Content/dataTables/ColVisAlt.css",
               //"~/Content/dataTables/demo_page.css",
               //"~/Content/dataTables/demo_table.css",
               "~/Content/dataTables/demo_table_jui.css",
                "~/Content/simplemodal.css",
                "~/Content/themes/smoothness/jquery-ui-1.7.2.custom.css",
                "~/Content/TableTools_JUI.css",
                "~/Content/MultiSelect/jquery.multiselect.css",
                "~/Content/MultiSelect/prettify.css",
                "~/Content/MultiSelect/jquery.multiselect.filter.css"

                //  "~/Content/jquery-ui.css"
                ));

            bundles.Add(new StyleBundle("~/Content/cssBS").Include(
                "~/Content/reset.css",
                "~/Content/siteBS.css",
                "~/Content/dataTables/ColVis.css",
                "~/Content/dataTables/ColVisAlt.css",
               //"~/Content/dataTables/demo_page.css",
               //"~/Content/dataTables/demo_table.css",
               "~/Content/dataTables/demo_table_juiBS.css",
                "~/Content/simplemodal.css",
                "~/Content/themes/smoothness/jquery-ui-1.7.2.customBS.css",
                "~/Content/TableTools_JUI.css",
                "~/Content/MultiSelect/jquery.multiselect.css",
                "~/Content/MultiSelect/prettify.css",
                "~/Content/MultiSelect/jquery.multiselect.filter.css"

                //  "~/Content/jquery-ui.css"
                ));

            bundles.Add(new StyleBundle("~/Content/AlertDashboardCss").Include(
                "~/Content/simple-slider-volume.css",
                "~/Content/simple-slider.css",
                "~/Content/Dashboard/blue.css",
                "~/Content/dataTables.scroller.css",
               "~/Content/Dashboard/jquery.ui.theme.css",
                "~/Content/MultiSelect/jquery.multiselect.css",
                "~/Content/MultiSelect/jquery.multiselect.filter.css",
                "~/Content/jquery.slidepanel.css"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/PopupScripts").Include(                        
            //            "~/Scripts/jquery.validate.js",
            //            "~/Scripts/jquery.validate.unobtrusive.js",
            //            "~/Scripts/jquery.unobtrusive-ajax.js"
            //            ));

            //bundles.Add(new ScriptBundle("~/bundles/MultiSelect").Include(
            //           "~/Scripts/MultiSelect/jquery.multiselect.js",
            //           "~/Scripts/MultiSelect/prettify.js",
            //           "~/Scripts/MultiSelect/jquery.multiselect.filter.js"
            //           ));

            //bundles.Add(new StyleBundle("~/Content/MultiSelect").Include(
            //   "~/Content/MultiSelect/jquery.multiselect.css",
            //      "~/Content/MultiSelect/prettify.css",
            //      "~/Content/MultiSelect/jquery.multiselect.filter.css"
            //   ));

            bundles.Add(new ScriptBundle("~/bundles/receiveList").Include(
                "~/Scripts/Receive.js",
                "~/Scripts/TabCommon.js",
                "~/Scripts/PageScripts/ReceiveList.js",
                "~/Scripts/PageScripts/ReceiveItemDetail.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/workOrderList").Include(
                "~/Scripts/TabCommon.js",
                "~/Scripts/Authorization.js",
                "~/Scripts/PageScripts/WorkOrder/WorkOrderList.js",
                "~/Scripts/PageScripts/WorkOrder/Wo_CreateWorkOrder.js",
                "~/Scripts/PageScripts/WorkOrder/CreateWOItem.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/newConsumePull").Include(
                //"~/Scripts/TabCommon.js",
                "~/Scripts/PageScripts/PullCommon.js",
                "~/Scripts/Authorization.js",
                "~/Scripts/CreditPull.js",
                "~/Scripts/PageScripts/MSCreditPull.js",
                "~/Scripts/PageScripts/Pull/NewConsumePull.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                "~/ScriptNG/Angular/Module/AppMain.js",
                "~/ScriptNG/Angular/Service/HtmlHelperSvc.js",
                "~/Scripts/webkitdragdrop.js",
                "~/Scripts/DynemicGirdFill.js",
                "~/Scripts/DynemicMultiselect.js",
                //"~/Scripts/moment-with-locales.min.js",
                "~/Scripts/Navigate/ays-beforeunload-shim.js",
                "~/Scripts/Navigate/jquery.are-you-sure.js",
                //"~/Scripts/PageScripts/LayoutScripts.js",
                "~/Scripts/TabCommon.js",
                //"~/Scripts/jquery.signalR-1.1.4.js",
                "~/Scripts/jquery.signalR-2.4.2.js",
                "~/Scripts/PageScripts/eTurnsSignalRHelper.js"
                ));


        }
    }
}