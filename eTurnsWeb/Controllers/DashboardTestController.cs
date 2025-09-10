using eTurns.DTO;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public class DashboardTestController : eTurnsControllerBase
    {
        //
        // GET: /DashboardTest/
        public ActionResult Index()
        {
            if (SessionHelper.EnterPriceID > 0)
            {
                var objdal = new DashboardWidgetDAL();
                var result = objdal.GetUserWidget(SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID,
                                                  SessionHelper.EnterPriceID, 1);
                if (result != null && !string.IsNullOrEmpty(result.WidgetOrder))
                {
                    var widgetorder = result.WidgetOrder;
                    string[] arr = widgetorder.Split(':');
                    if (arr.Length > 0)
                    {
                        List<string> leftids = arr[0].Split(',').ToList();
                        List<string> rightids = arr[1].Split(',').ToList();
                        ViewBag.LeftWidget = leftids;
                        ViewBag.RightWidget = rightids;
                    }
                }
                else
                {
                    string left = SessionHelper.ParentModuleList.Master + "," +
                                         SessionHelper.ParentModuleList.Kits + "," +
                                         SessionHelper.ParentModuleList.Consume + "," +
                                         SessionHelper.ParentModuleList.Reports;
                    string right = SessionHelper.ParentModuleList.Authentication + "," +
                                           SessionHelper.ParentModuleList.Inventory + "," +
                                           SessionHelper.ParentModuleList.Assets + "," +
                                           SessionHelper.ParentModuleList.Receive + "," +
                                           SessionHelper.ParentModuleList.Replenish + "," +
                                           SessionHelper.ParentModuleList.Configuration;

                    List<string> leftids = left.Split(',').ToList();
                    List<string> rightids = right.Split(',').ToList();
                    ViewBag.LeftWidget = leftids;
                    ViewBag.RightWidget = rightids;
                }
            }

            return View();
        }
        [HttpPost]
        public void SaveDashboardWidget(string leftwidget, string rightwidget)
        {
            var objdal = new DashboardWidgetDAL();
            string widgetorder = string.Empty;
            if (!string.IsNullOrEmpty(leftwidget) && !string.IsNullOrEmpty(rightwidget))
            {
                widgetorder = leftwidget + ":" + rightwidget;
            }
            else if (string.IsNullOrEmpty(leftwidget) && !string.IsNullOrEmpty(rightwidget))
            {
                widgetorder = ":" + rightwidget;
            }
            else if (!string.IsNullOrEmpty(leftwidget) && string.IsNullOrEmpty(rightwidget))
            {
                widgetorder = leftwidget + ":";
            }
            objdal.SaveUserWidget(SessionHelper.UserID, widgetorder, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, 1);


        }
        public ActionResult AuthenticationDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Authentication));
            ViewBag.AuthenticationList = obj;
            return PartialView("AuthenticationDb");
        }
        public ActionResult MasterDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Master));
            ViewBag.MasterList = obj;
            return PartialView("MasterDb");
        }
        public ActionResult AssetsDb()
        {
            return PartialView("AssetsDb");
        }
        public ActionResult ReportsDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Reports));
            ViewBag.ReportsList = obj;
            return PartialView("ReportsDb");
        }
        public ActionResult InventoryDb()
        {
            return PartialView("InventoryDb");
        }
        public ActionResult ConsumeDb()
        {
            return PartialView("ConsumeDb");
        }
        public ActionResult ReplenishDb()
        {
            return PartialView("ReplenishDb");
        }
        public ActionResult ReceiveDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Receive));
            ViewBag.ReceiveList = obj;
            return PartialView("ReceiveDb");
        }
        public ActionResult KitsDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Kits));
            ViewBag.KitsList = obj;
            return PartialView("KitsDb");
        }
        public ActionResult ConfigurationDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Configuration));
            ViewBag.ConfigurationList = obj;

            return PartialView("ConfigurationDb");
        }
        public ActionResult CategoryCostDb()
        {
            return PartialView("CategoryCostDb");
        }
        private List<UserRoleModuleDetailsDTO> GetModulePermission(int itemid)
        {
            var lstPermission = SessionHelper.RoomPermissions.Find(element => element.EnterpriseId == SessionHelper.EnterPriceID && element.CompanyId == SessionHelper.CompanyID && element.RoomID == SessionHelper.RoomID);

            List<UserRoleModuleDetailsDTO> returval = (from m in lstPermission.PermissionList
                                                       where m.ParentID == itemid
                                                       select m).OrderBy(c => c.ModuleName).ToList();
            if (returval.Any())
                returval = returval.Where(c => c.ModuleID != (Int64)SessionHelper.ModuleList.ResetAutoNumbers && c.ModuleID != (Int64)SessionHelper.ModuleList.Synch && c.ModuleID != (Int64)SessionHelper.ModuleList.Barcode).ToList();
            return returval;
        }
    }
}
