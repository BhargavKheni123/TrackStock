using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace eTurnsWeb.Controllers
{
    public class InventoryDashboardController : eTurnsControllerBase
    {
        //
        // GET: /InventoryDashboard/

        public ActionResult ItemMasterListAjax(QuickListJQueryDataTableParamModel param)
        {
            DashboardDAL obj = new DashboardDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            DateTime FromDate = DateTime.Parse(Request["FromDate"].ToString());
            DateTime ToDate = DateTime.Parse(Request["ToDate"].ToString());
            string Criteria = Request["Criteria"];
            bool IsRequestFromDashboard = false;
            string selectedSupplier = "";
            string selectedCategory = "";
            string strSupplierOptions = "";
            string strCategoryOptions = "";

            if (Request["SelectedSupplier"] != null)
            {
                selectedSupplier = Request["SelectedSupplier"].ToString();
                IsRequestFromDashboard = true;
            }

            if (Request["SelectedCategory"] != null)
            {
                selectedCategory = Request["SelectedCategory"].ToString();
                IsRequestFromDashboard = true;
            }

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + FromDate.ToString() + "#" + ToDate.ToString() + "#" + Criteria;
            Session.Add("InventorySearchCriteria", SearchCteriaCombined);
            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            List<DashboardItem> DataFromDBAll = new List<DashboardItem>();
            List<DashboardItem> DataFromDB = new List<DashboardItem>();
            IEnumerable<DashboardItem> supplierList;
            IEnumerable<DashboardItem> categoryList;

            if (IsRequestFromDashboard == true)
            {
                DataFromDBAll = obj.GetPagedItemsForDashboardInventory(out supplierList, out categoryList, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, true, Criteria, false, selectedSupplier, selectedCategory).ToList();

                if (DataFromDBAll != null && DataFromDBAll.Count > 0)
                {
                    DataFromDB = DataFromDBAll;
                }
                else
                    DataFromDB = new List<DashboardItem>();

                //-------------------Prepare DDL Data-------------------
                //
                if (supplierList != null && supplierList.Any())
                {
                    List<string> lstSupplier = supplierList.Select(x => "<option value='" + x.SupplierID + "'>" + x.SupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
                    strSupplierOptions = string.Join("", lstSupplier);
                }

                if (categoryList != null && categoryList.Any())
                {
                    List<string> lstCategory = categoryList.Select(x => "<option value='" + x.CategoryID + "'>" + x.CategoryName + "(" + x.TotalRecords + ")" + "</option>").ToList();
                    strCategoryOptions = string.Join("", lstCategory);
                }
            }
            else
            {
                DataFromDB = obj.GetItemsForItemDashboardDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, true, null, null, 0, false, Criteria, false).ToList();
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB,
                SupplierOptions = strSupplierOptions,
                CategoryOptions = strCategoryOptions,
                SelectedSupplier = selectedSupplier,
                SelectedCategory = selectedCategory,
                SearchTerm = param.sSearch,
                StartIndex = param.iDisplayStart
            },
            JsonRequestBehavior.AllowGet);

        }

        public Int32 GetItemStatusCount(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string FromDate, string ToDate, string Criteria = "Critical")
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            DateTime DFromDate = DateTime.Parse(FromDate.ToString());
            DateTime DToDate = DateTime.Parse(ToDate.ToString());

            return obj.GetItemStatusCount(RoomID, CompanyID, DFromDate, DToDate, Criteria);
        }

        public Int32 GetItemStatusCountMoving(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string FromDate, string ToDate, string Criteria = "Critical")
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            DateTime DFromDate = DateTime.Parse(FromDate.ToString());
            DateTime DToDate = DateTime.Parse(ToDate.ToString());

            return 0;//return obj.GetItemStatusCountSlowMoving(RoomID, CompanyID, IsArchived, IsDeleted, DFromDate, DToDate, true, Criteria);
        }
        public Int32 GetCartItemOrderCount()
        {
            CartItemDAL obj = new CartItemDAL(SessionHelper.EnterPriseDBName);
            return obj.GetCartOrderCount(SessionHelper.CompanyID, SessionHelper.RoomID);
        }
        public Int32 GetCartItemTransferCount()
        {
            CartItemDAL obj = new CartItemDAL(SessionHelper.EnterPriseDBName);
            return obj.GetCartTransferCount(SessionHelper.CompanyID, SessionHelper.RoomID);
        }
        public Int32 GetRequisitionStatusCount(string Status)
        {
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<CommonDTO> objlst = objCommon.GetTabStatusCount("RequisitionMaster", "RequisitionStatus", SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserSupplierIds, "false").ToList();

            if ((objlst.FindIndex(t => t.Text == Status) >= 0))
            {
                return (objlst.Where(t => t.Text == Status).SingleOrDefault()).Count;// "Unsubmitted").SingleOrDefault()).Count;
            }

            return 0;
        }

        public long GetOrderStatusCount(int Status)
        {
            OrderMasterDAL orderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            long StatusCount = 0;
            StatusCount = orderMasterDAL.GetOrderCountByOrderStatus(SessionHelper.RoomID, SessionHelper.CompanyID, (int)OrderType.Order);
            return StatusCount;
        }
        public Int32 GetItemCount()
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);

            return (objInventoryCountDAL.GetInventoryItemCount());
        }
        public Int32 GetTransferStatusCount(int Status)
        {
            //TransferMasterDAL obj = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<TransferMasterDTO> lsttransfer = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            int StatusCount = 0;
            //StatusCount = lsttransfer.Where(t => t.TransferStatus == Status).Count();
            return StatusCount;
        }

        public ActionResult RequisitionMasterListAjax(JQueryDataTableParamModel param)
        {
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"];
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            DateTime FromDate = DateTime.Parse(Request["FromDate"].ToString());
            DateTime ToDate = DateTime.Parse(Request["ToDate"].ToString());
            string Criteria = Request["Criteria"];

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + FromDate.ToString() + "#" + ToDate.ToString() + "#" + Criteria;
            Session.Add("ConsumeSearchCriteria", SearchCteriaCombined);
            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            List<RequisitionMasterDTO> lstRequisition = obj.GetPagedRequisitionsForDashboard(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds, true, Criteria, "Requisition", SessionHelper.UserID, ChartHelper.GetReqStatus(), ChartHelper.GetReqTypes(), true);
            var DataFromDB = lstRequisition;

            DataFromDB.ForEach(x =>
            {
                x.RequiredDateStr = x.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public Int32 GetProjectSpendCount(string status)
        {

            ProjectMasterDAL objProject = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            return objProject.GetProjectMaster(SessionHelper.RoomID, SessionHelper.CompanyID, null, null, status, 0).Count();
        }

        public ActionResult ProjectMasterListAjax(JQueryDataTableParamModel param)
        {
            ProjectMasterDAL objProject = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            DateTime FromDate = DateTime.Parse(Request["FromDate"].ToString());
            DateTime ToDate = DateTime.Parse(Request["ToDate"].ToString());
            string Status = Request["Status"].ToString();
            Int32 StatusValue = 0;
            Int32.TryParse(Request["StatusValue"].ToString(), out StatusValue);

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + FromDate.ToString() + "#" + ToDate.ToString() + "#" + Status + "#" + StatusValue.ToString();
            Session.Add("ProjectSearchCriteria", SearchCteriaCombined);
            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            string WidgetType = string.Empty;
            if (!string.IsNullOrWhiteSpace(SearchCteriaCombined.Split('#')[4]))
            {
                WidgetType = SearchCteriaCombined.Split('#')[4];
                switch (WidgetType)
                {
                    case "Project Amount Exceeds":
                        WidgetType = "ProjectAmount";
                        break;
                    case "Item Quantity Exceeds":
                        WidgetType = "ItemQuantity";
                        break;
                    case "Item Amount Exceeds":
                        WidgetType = "ItemAmount";
                        break;
                    default:
                        break;
                }
            }
            int TotalRecordCount = 0;

            var DataFromDB = objProject.GetProjectMastersDashboard(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, WidgetType, true, SessionHelper.UserID);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyConfig(Int64 CompanyID, string Status)
        {
            string statusvalue = "";
            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            if (Status == "Project Amount Exceeds")
                statusvalue = obj.GetCompanyConfig(CompanyID).ProjectAmountExceed == null ? "0" : obj.GetCompanyConfig(CompanyID).ProjectAmountExceed.ToString();
            else if (Status == "Item Quantity Exceeds")
                statusvalue = obj.GetCompanyConfig(CompanyID).ProjectItemQuantitExceed == null ? "0" : obj.GetCompanyConfig(CompanyID).ProjectItemQuantitExceed.ToString();
            else if (Status == "Item Amount Exceeds")
                statusvalue = obj.GetCompanyConfig(CompanyID).ProjectItemAmountExceed == null ? "0" : obj.GetCompanyConfig(CompanyID).ProjectItemAmountExceed.ToString();
            else if (Status == "ScheduleDays")
                statusvalue = obj.GetCompanyConfig(CompanyID).ScheduleDaysBefore == null ? "0" : obj.GetCompanyConfig(CompanyID).ScheduleDaysBefore.ToString();

            return Json(new { StatusValue = statusvalue }, JsonRequestBehavior.AllowGet);
        }
        public string GetCompanyInventoryConfig(Int64 CompanyID, string Status)
        {
            string statusvalue = "";
            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            if (Status == "NOBackDays")
                statusvalue = obj.GetCompanyConfig(CompanyID).NOBackDays.ToString();
            else if (Status == "NODaysAve")
                statusvalue = obj.GetCompanyConfig(CompanyID).NODaysAve.ToString();
            else if (Status == "NOTimes")
                statusvalue = obj.GetCompanyConfig(CompanyID).NOTimes.ToString();
            else if (Status == "MaxPer")
                statusvalue = obj.GetCompanyConfig(CompanyID).MaxPer.ToString();
            else if (Status == "MinPer")
                statusvalue = obj.GetCompanyConfig(CompanyID).MinPer.ToString();

            return statusvalue == "" ? "0" : statusvalue;
        }


        /// <summary>
        /// OrderMasterListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult OrderMasterListAjax(JQueryDataTableParamModel param)
        {
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            DateTime FromDate = DateTime.Parse(Request["FromDate"].ToString());
            DateTime ToDate = DateTime.Parse(Request["ToDate"].ToString());
            string OrderStatus = Request["OrderStatus"].ToString();
            bool IsRequestFromDashboard = false;
            string selectedSupplier = "";
            string strSupplierOptions = "";

            if (Request["SelectedSupplier"] != null)
            {
                selectedSupplier = Request["SelectedSupplier"].ToString();
                IsRequestFromDashboard = true;
            }

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + FromDate.ToString() + "#" + ToDate.ToString() + "#" + OrderStatus;
            Session.Add("OrderSearchCriteria", SearchCteriaCombined);
            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            // set the default column sorting here, if first time then required to set 

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
            string TabtoPass = "[^]1";
            if (OrderStatus == "Unsubmitted")
            {
                TabtoPass = "[^]1";
            }
            else if (OrderStatus == "Submitted")
            {
                TabtoPass = "[^]2";
            }
            else if (OrderStatus == "Approved")
            {
                TabtoPass = "[^]3";
            }
            if(string.IsNullOrWhiteSpace(param.sSearch))
            {
                param.sSearch = TabtoPass;
            }

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            List<OrderMasterDTO> lstOrderListAll = new List<OrderMasterDTO>();
            List<OrderMasterDTO> lstOrderList = new List<OrderMasterDTO>();
            OrderMasterDAL controller = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<OrderMasterDTO> supplierList;

            if (IsRequestFromDashboard == true)
            {
                lstOrderListAll = controller.GetOrderMasterPagedDataNormalForDashboard(out supplierList, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, OrderType.Order, selectedSupplier, 1, SessionHelper.UserSupplierIds).ToList();

                if (lstOrderListAll != null && lstOrderListAll.Count > 0)
                {
                    lstOrderList = lstOrderListAll;
                }
                else
                    lstOrderList = new List<OrderMasterDTO>();

                //-------------------Prepare DDL Data-------------------
                //
                if (supplierList != null && supplierList.Any())
                {
                    List<string> lstSupplier = supplierList.Select(x => "<option value='" + x.Supplier + "'>" + x.SupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
                    strSupplierOptions = string.Join("", lstSupplier);
                }
            }
            else
            {
                lstOrderList = controller.GetOrderMasterPagedDataNormalForDashboard(out supplierList, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, OrderType.Order, selectedSupplier, 1, SessionHelper.UserSupplierIds).ToList();
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstOrderList,
                SupplierOptions = strSupplierOptions,
                SelectedSupplier = selectedSupplier,
                SearchTerm = param.sSearch,
                StartIndex = param.iDisplayStart
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemOrderMasterListAjax(JQueryDataTableParamModel param)
        {
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            string sortDirection = Request["sSortDir_0"];
            string spendType = Convert.ToString(Request["SpendType"]);
            bool IsRequestFromDashboard = false;
            string selectedSupplier = "";
            string strSupplierOptions = "";

            if (Request["SelectedSupplier"] != null)
            {
                selectedSupplier = Request["SelectedSupplier"].ToString();
                IsRequestFromDashboard = true;
            }

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "OrderCost";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            List<DashboardBottomAndTopSpendDTO> lstOrderListAll = new List<DashboardBottomAndTopSpendDTO>();
            List<DashboardBottomAndTopSpendDTO> lstOrderList = new List<DashboardBottomAndTopSpendDTO>();
            OrderMasterDAL controller = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<DashboardBottomAndTopSpendDTO> SupplierList;

            if (IsRequestFromDashboard == true)
            {
                lstOrderListAll = controller.GetDashboardTopAndBottomSpendForGrid(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, spendType, selectedSupplier, out SupplierList, SessionHelper.UserSupplierIds).ToList();

                if (lstOrderListAll != null && lstOrderListAll.Count > 0)
                {
                    lstOrderList = lstOrderListAll;
                }
                else
                    lstOrderList = new List<DashboardBottomAndTopSpendDTO>();

                //-------------------Prepare DDL Data-------------------
                if (SupplierList != null && SupplierList.Any())
                {
                    List<string> lstSupplier = SupplierList.Select(x => "<option value='" + x.SupplierID + "'>" + x.SupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
                    strSupplierOptions = string.Join("", lstSupplier);
                }
            }
            else
            {
                lstOrderList = controller.GetDashboardTopAndBottomSpendForGrid(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, "BotttomSpend", string.Empty, out SupplierList, SessionHelper.UserSupplierIds).ToList();
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstOrderList,
                SupplierOptions = strSupplierOptions,
                SelectedSupplier = selectedSupplier,
                SearchTerm = param.sSearch,
                StartIndex = param.iDisplayStart
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransferListAjax(JQueryDataTableParamModel param)
        {
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            DateTime FromDate = DateTime.Parse(Request["FromDate"].ToString());
            DateTime ToDate = DateTime.Parse(Request["ToDate"].ToString());
            string TransferStatus = Request["TransferStatus"].ToString();

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + FromDate.ToString() + "#" + ToDate.ToString() + "#" + TransferStatus;
            Session.Add("TransferSearchCriteria", SearchCteriaCombined);
            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            TransferMasterDAL controller = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<TransferMasterDTO> lstTransferList = controller.GetPagedTransferDataForDashboard(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), TransferStatus);
            var DataFromDB = lstTransferList;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssetsMaintenanceListAjax(JQueryDataTableParamModel param)
        {
            AssetMasterDAL assetMasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"];
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            int Days = 0;
            Int32.TryParse(Request["Days"].ToString(), out Days);

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + Days.ToString();
            Session.Add("AssetSearchCriteria", SearchCteriaCombined);
            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            var DataFromDB = assetMasterDAL.GetAssetMaintenceForDashboard(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult AssetsListAjax(JQueryDataTableParamModel param)
        {

            AssetMasterDAL obj = new AssetMasterDAL(SessionHelper.EnterPriseDBName);

            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"];

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


            Int32 Days = 0;
            Int32.TryParse(Request["Days"].ToString(), out Days);


            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + Days.ToString();
            Session.Add("AssetSearchCriteria", SearchCteriaCombined);
            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedAssetMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone).ToList();

            //lstToolsMaintenance = lstToolsMaintenance.Where(t => t.AssetGUID.HasValue).ToList();

            //var DataFromDB = lstToolsMaintenance;
            //TotalRecordCount = lstToolsMaintenance.Count();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult ToolsMaintenanceListAjax(JQueryDataTableParamModel param)
        {
            ToolsMaintenanceDAL toolsMaintenanceDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            Int32 Days = 0;
            Int32.TryParse(Request["Days"].ToString(), out Days);

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + Days.ToString();
            Session.Add("ToolsSearchCriteria", SearchCteriaCombined);
            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            var DataFromDB = toolsMaintenanceDAL.GetToolMaintenceForDashboard(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        //public ActionResult ToolsListAjax(JQueryDataTableParamModel param)
        //{

        //    ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);

        //    int PageIndex = Convert.ToInt32(param.sEcho);
        //    int PageSize = param.iDisplayLength;
        //    var sortDirection = Request["sSortDir_0"];
        //    var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    string sortColumnName = string.Empty;
        //    string sDirection = string.Empty;
        //    int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
        //    sortColumnName = Request["SortingField"].ToString();

        //    bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
        //    bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


        //    Int32 Days = 0;
        //    Int32.TryParse(Request["Days"].ToString(), out Days);


        //    /////////////////// SESSION VALUE FOR CHART ////////////////////////////
        //    string SearchCteriaCombined = IsArchived.ToString() + "#" + IsDeleted.ToString() + "#" + Days.ToString();
        //    Session.Add("ToolsSearchCriteria", SearchCteriaCombined);
        //    /////////////////// SESSION VALUE FOR CHART ////////////////////////////


        //    if (sortColumnName == "0" || sortColumnName == "undefined")
        //        sortColumnName = "ID";

        //    if (sortDirection == "asc")
        //        sortColumnName = sortColumnName + " asc";
        //    else
        //        sortColumnName = sortColumnName + " desc";

        //    string searchQuery = string.Empty;

        //    int TotalRecordCount = 0;
        //    var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat)).ToList();

        //    //lstToolsMaintenance = lstToolsMaintenance.Where(t => t.ToolGUID.HasValue).ToList();

        //    //var DataFromDB = lstToolsMaintenance;
        //    //TotalRecordCount = lstToolsMaintenance.Count();

        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount,
        //        aaData = DataFromDB
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}
        public string ItemQuantityChart(Int64 ItemID, string ItemGUID)
        {
            return RenderRazorPartialViewToString("ItemQuantityChart");
        }
        public string RenderRazorPartialViewToString(string viewName)
        {
            //ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult ItemMasterQuantityListAjax(JQueryDataTableParamModel param)
        {
            //ItemMasterController obj = new ItemMasterController();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            //LoadTestEntities entity = new LoadTestEntities();
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


            int FromYear = Int32.Parse(Request["FromYear"].ToString());
            int ToYear = Int32.Parse(Request["ToYear"].ToString());
            int FromMonth = Int32.Parse(Request["FromMonth"].ToString());
            int ToMonth = Int32.Parse(Request["ToMonth"].ToString());

            int NOBackDays = 0;
            Int32.TryParse(Request["NOBackDays"].ToString(), out NOBackDays);
            int NODaysAve = 0;
            Int32.TryParse(Request["NODaysAve"].ToString(), out NODaysAve);
            decimal NOTimes = 0;
            decimal.TryParse(Request["NOTimes"].ToString(), out NOTimes);
            int MinPer = 0;
            Int32.TryParse(Request["MinPer"].ToString(), out MinPer);
            int MaxPer = 0;
            Int32.TryParse(Request["MaxPer"].ToString(), out MaxPer);
            int AutoMinPer = 0;
            Int32.TryParse(Request["AutoMinPer"].ToString(), out AutoMinPer);
            int AutoMaxPer = 0;
            Int32.TryParse(Request["AutoMaxPer"].ToString(), out AutoMaxPer);
            bool IsItemLevelMinMax = true;
            bool.TryParse(Request["IsItemLevelMinMax"].ToString(), out IsItemLevelMinMax);

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            List<InventoryDashboardDTO> DataFromDB = new List<InventoryDashboardDTO>();

            if (Session["ItemQuantityGrid"] == null || Convert.ToString(Session["ItemQuantityGrid"]) == "")
            {
                //Session["ItemQuantityGridCount"] = 0;

                DataFromDB = new List<InventoryDashboardDTO>();//obj.GetAllRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromYear, ToYear, FromMonth, ToMonth, NOBackDays, NODaysAve, NOTimes, MinPer, MaxPer, AutoMinPer, AutoMaxPer, IsItemLevelMinMax).ToList();
                Session["ItemQuantityGrid"] = DataFromDB;
                Session["ItemQuantityGridCount"] = TotalRecordCount;
            }
            else
            {
                DataFromDB = (List<InventoryDashboardDTO>)Session["ItemQuantityGrid"];

            }
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = Session["ItemQuantityGridCount"],
                iTotalDisplayRecords = Session["ItemQuantityGridCount"], //filteredCompanies.Count(),
                aaData = (List<InventoryDashboardDTO>)Session["ItemQuantityGrid"]
            }, JsonRequestBehavior.AllowGet);

            //var DataFromDB = obj.GetAllRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromYear, ToYear, FromMonth, ToMonth, NOBackDays, NODaysAve, NOTimes, MinPer, MaxPer, AutoMinPer, AutoMaxPer);
            //return Json(new
            //{
            //    sEcho = param.sEcho,
            //    iTotalRecords = TotalRecordCount,
            //    iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
            //    aaData = DataFromDB
            //},
            //            JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetAllRecordsWithoutPaging(string RoomID, string CompanyID, string FromYear, string ToYear, string FromMonth, string ToMonth, string NOBackDays, string NODaysAve, string NOTimes, string MinPer, string MaxPer, string AutoMinPer, string AutoMaxPer, bool IsItemLevelMinMax)
        {
            Int64 iRoomID = 0;
            Int64.TryParse(RoomID.ToString(), out iRoomID);
            Int64 iCompanyID = 0;
            Int64.TryParse(CompanyID.ToString(), out iCompanyID);

            int iFromYear = Int32.Parse(FromYear.ToString());
            int iToYear = Int32.Parse(ToYear.ToString());
            int iFromMonth = Int32.Parse(FromMonth.ToString());
            int iToMonth = Int32.Parse(ToMonth.ToString());

            int iNOBackDays = 0;
            Int32.TryParse(NOBackDays.ToString(), out iNOBackDays);
            int iNODaysAve = 0;
            Int32.TryParse(NODaysAve.ToString(), out iNODaysAve);
            decimal iNOTimes = 0;
            decimal.TryParse(NOTimes.ToString(), out iNOTimes);
            int iMinPer = 0;
            Int32.TryParse(MinPer.ToString(), out iMinPer);
            int iMaxPer = 0;
            Int32.TryParse(MaxPer.ToString(), out iMaxPer);
            int iAutoMinPer = 0;
            Int32.TryParse(AutoMinPer.ToString(), out iAutoMinPer);
            int iAutoMaxPer = 0;
            Int32.TryParse(AutoMaxPer.ToString(), out iAutoMaxPer);


            string strAllCount = "";
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            List<InventoryDashboardDTO> lst = new List<InventoryDashboardDTO>();
            //bool IsItemLevelMinMax = true;
            //bool.TryParse(Request["IsItemLevelMinMax"].ToString(), out IsItemLevelMinMax);
            lst = new List<InventoryDashboardDTO>(); //obj.GetAllRecordsWithoutPaging(iRoomID, iCompanyID, iFromYear, iToYear, iFromMonth, iToMonth, iNOBackDays, iNODaysAve, iNOTimes, iMinPer, iMaxPer, iAutoMinPer, iAutoMaxPer, IsItemLevelMinMax).ToList();
            string MinRedCount = lst.Where(c => c.MinAnalysis == "Red").Count().ToString();
            string MinYellowCount = lst.Where(c => c.MinAnalysis == "Yellow").Count().ToString();
            string MinGreenCount = lst.Where(c => c.MinAnalysis == "Green").Count().ToString();


            string MaxRedCount = lst.Where(c => c.MaxAnalysis == "Red").Count().ToString();
            string MaxYellowCount = lst.Where(c => c.MaxAnalysis == "Yellow").Count().ToString();
            string MaxGreenCount = lst.Where(c => c.MaxAnalysis == "Green").Count().ToString();

            strAllCount = MinRedCount + "#" + MinYellowCount + "#" + MinGreenCount + "$$" + MaxRedCount + "#" + MaxYellowCount + "#" + MaxGreenCount;
            return Json(new { StatusValue = strAllCount }, JsonRequestBehavior.AllowGet);
            //return strAllCount;
        }

        [HttpPost]
        public JsonResult SaveCalculatedMinMax(string para = "")
        {
            string message = "";
            string status = "";
            Session["ItemQuantityGrid"] = "";
            Session["ItemQuantityGridCount"] = 0;
            JavaScriptSerializer s = new JavaScriptSerializer();
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            try
            {
                //List<ItemMasterDTO> CurrentBlankBinList = new List<ItemMasterDTO>();
                ItemMasterDTO[] LstitemMaster = s.Deserialize<ItemMasterDTO[]>(para);
                if (LstitemMaster != null && LstitemMaster.Length > 0)
                {
                    foreach (ItemMasterDTO item in LstitemMaster)
                    {
                        ItemMasterDAL objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                        ItemMasterDTO objDTO = new ItemMasterDTO();
                        objDTO = objItem.GetItemWithoutJoins(null, item.GUID);
                        objDTO.MinimumQuantity = item.MinimumQuantity;
                        objDTO.MaximumQuantity = item.MaximumQuantity;
                        objDTO.WhatWhereAction = "Dashboard";
                        objItem.Edit(objDTO, SessionUserId,enterpriseId);
                    }
                }

            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                // resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BlankSession(string para = "")
        {
            string message = "";
            Session["ItemQuantityGrid"] = "";
            Session["ItemQuantityGridCount"] = 0;
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateInventoryQuantity(string MinOrMax, string GUID, string Automin, string CalMin, string Automax, string CalMax, string MinPer, string MaxPer)
        {
            string message = "";
            List<InventoryDashboardDTO> lst = (List<InventoryDashboardDTO>)Session["ItemQuantityGrid"];

            InventoryDashboardDTO obj = new InventoryDashboardDTO();
            obj = lst.Where(c => c.GUID.ToString() == GUID).SingleOrDefault();
            decimal dMinPer = 0;
            decimal.TryParse(MinPer, out dMinPer);

            decimal dMaxPer = 0;
            decimal.TryParse(MaxPer, out dMaxPer);

            if (MinOrMax == "AutoCurrentMin")
            {
                decimal dCalMin = 0;
                decimal.TryParse(CalMin, out dCalMin);

                double dAutomin = 0;
                double.TryParse(Automin, out dAutomin);

                obj.AutoCurrentMin = dAutomin;

                decimal MinAutoPer = Convert.ToDecimal(((dCalMin - Convert.ToDecimal(dAutomin)) / Convert.ToDecimal(dAutomin)) * 100);
                obj.AutoMinPercentage = MinAutoPer < 0 ? Math.Round((MinAutoPer * (-1)), 2) : Math.Round(MinAutoPer, 2);

                obj.MinAnalysis = (obj.AutoMinPercentage < dMinPer) ? "Green" : (obj.AutoMinPercentage > dMaxPer) ? "Red" : "Yellow";
            }
            else if (MinOrMax == "AutoCurrentMax")
            {
                decimal dCalMax = 0;
                decimal.TryParse(CalMax, out dCalMax);

                decimal dAutomax = 0;
                decimal.TryParse(Automax, out dAutomax);

                obj.AutoCurrentMax = dAutomax;

                decimal MaxAutoPer = Convert.ToDecimal(((dCalMax - Convert.ToDecimal(dAutomax)) / Convert.ToDecimal(dAutomax)) * 100);

                obj.AutoMaxPercentage = MaxAutoPer < 0 ? Math.Round((MaxAutoPer * (-1)), 2) : Math.Round(MaxAutoPer, 2);

                obj.MaxAnalysis = (obj.AutoMaxPercentage < dMinPer) ? "Green" : (obj.AutoMaxPercentage > dMaxPer) ? "Red" : "Yellow";
            }
            List<InventoryDashboardDTO> lstnew = new List<InventoryDashboardDTO>();

            foreach (InventoryDashboardDTO item in lst)
            {
                if (item.GUID.ToString() == GUID)
                {
                    lstnew.Add(obj);
                }
                else
                {
                    lstnew.Add(item);
                }
            }
            Session["ItemQuantityGrid"] = lstnew;
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
            //BinMasterController obj = new BinMasterController();
            //BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            // return "";
        }

        public JsonResult GetHeaderCount(string RoomID, string CompanyID)
        {
            string message = "";


            Int64 iRoomID = 0;
            Int64.TryParse(RoomID.ToString(), out iRoomID);
            Int64 iCompanyID = 0;
            Int64.TryParse(CompanyID.ToString(), out iCompanyID);

            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            InventoryDashboardDTO objDTO = new InventoryDashboardDTO();
            //objDTO = obj.GetHeaderCount(iRoomID, iCompanyID);
            Int32 iMTDCount = 0;// obj.GetMTDCount(iRoomID, iCompanyID);
            Int32 iYTDCount = 0;//obj.GetYTDCount(iRoomID, iCompanyID);

            message = "$ " + objDTO.InventoryValue + "##" + objDTO.Turns.ToString() + "##" + iMTDCount.ToString() + "##" + iYTDCount.ToString(); // InventoryValue , Turns, MTD, YTD



            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InventoryCountMasterListAjax(JQueryDataTableParamModel param)
        {
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"];

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = string.Empty;

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;

            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<InventoryCountDTO> lstInventoryCountDTO = new List<InventoryCountDTO>();

            lstInventoryCountDTO = objInventoryCountDAL.GetPagedInventoryCountForDashboard(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat));
            var DataFromDB = lstInventoryCountDTO;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CartItemMasterListAjax(JQueryDataTableParamModel param)
        {
            int PageIndex = Convert.ToInt32(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"];
            string CartType = Request["CartType"].ToString();
            bool IsRequestFromDashboard = false;
            string _SelectedSupplier = "";
            string _SelectedCategory = "";
            string strSupplierOptions = "";
            string strCategoryOptions = "";

            if (Request["SelectedSupplier"] != null && Request["SelectedCategory"] != null)
            {
                _SelectedSupplier = Request["SelectedSupplier"].ToString();
                _SelectedCategory = Request["SelectedCategory"].ToString();
                IsRequestFromDashboard = true;
            }

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////
            string SearchCteriaCombined = string.Empty;

            /////////////////// SESSION VALUE FOR CHART ////////////////////////////

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            List<CartChartDTO> lstCartItemDTOAll = new List<CartChartDTO>();
            List<CartChartDTO> lstCartItemDTO = new List<CartChartDTO>();
            IEnumerable<CartChartDTO> SupplierList = null;
            IEnumerable<CartChartDTO> CategoryList = null;

            if (IsRequestFromDashboard == true)
            {
                lstCartItemDTOAll = objCartItemDAL.GetCartItemsForDashboard(out SupplierList, out CategoryList, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, CartType, SessionHelper.UserSupplierIds, _SelectedSupplier, _SelectedCategory);

                if (lstCartItemDTOAll != null && lstCartItemDTOAll.Count > 0)
                {
                    lstCartItemDTO = lstCartItemDTOAll;
                }
                else
                    lstCartItemDTO = new List<CartChartDTO>();

                //-------------------Prepare DDL Data-------------------
                //
                if (SupplierList != null && SupplierList.Any())
                {
                    List<string> lstSupplier = SupplierList.Select(x => "<option value='" + x.SupplierId + "'>" + x.SupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
                    strSupplierOptions = string.Join("", lstSupplier);
                }

                if (CategoryList != null && CategoryList.Any())
                {
                    List<string> lstCategory = CategoryList.Select(x => "<option value='" + x.CategoryId + "'>" + x.CategoryName + "(" + x.TotalRecords + ")" + "</option>").ToList();
                    strCategoryOptions = string.Join("", lstCategory);
                }
            }
            else
            {
                lstCartItemDTOAll = objCartItemDAL.GetCartItemsForDashboard(out SupplierList, out CategoryList, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, CartType, SessionHelper.UserSupplierIds, _SelectedSupplier, _SelectedCategory);
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstCartItemDTO,
                SupplierOptions = strSupplierOptions,
                CategoryOptions = strCategoryOptions,
                SelectedSupplier = _SelectedSupplier,
                SelectedCategory = _SelectedCategory,
                SearchTerm = param.sSearch,
                StartIndex = param.iDisplayStart
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectRedCountForDashboard()
        {
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            List<ProjectSpendItemsDTO> lstCounts = objDashboardDAL.GetProjectsForDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, 10, true, "none");
            long projectExceedCount = 0;
            long itemQuantityExceedCount = 0;
            long itemAmountExceedCount = 0;

            if (lstCounts != null && lstCounts.Count > 0)
            {
                if (lstCounts.Any(t => t.ProjectSpendName == "ItemAmount"))
                {
                    itemAmountExceedCount = lstCounts.First(t => t.ProjectSpendName == "ItemAmount").RownumberPS;
                }
                if (lstCounts.Any(t => t.ProjectSpendName == "ItemQuantity"))
                {
                    itemQuantityExceedCount = lstCounts.First(t => t.ProjectSpendName == "ItemQuantity").RownumberPS;
                }
                if (lstCounts.Any(t => t.ProjectSpendName == "ProjectAmount"))
                {
                    projectExceedCount = lstCounts.First(t => t.ProjectSpendName == "ProjectAmount").RownumberPS;
                }
            }

            return Json(new
            {
                projectExceedCount = projectExceedCount,
                itemQuantityExceedCount = itemQuantityExceedCount,
                itemAmountExceedCount = itemAmountExceedCount
            }, JsonRequestBehavior.AllowGet);
        }
    }

}
