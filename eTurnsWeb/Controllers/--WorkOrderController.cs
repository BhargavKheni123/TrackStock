using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTurnsWeb.Helper;
using eTurns.DTO;
using eTurns.DAL;
using eTurnsWeb.Controllers.WebAPI;
using System.IO;
using System.Text;
using System.Web.UI;
using eTurns.DTO.Resources;
using System.Net.Http;
using System.Web.UI.WebControls;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurnsWeb.Models;
using System.Web.Script.Serialization;
using System.Data;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class WorkOrderController : eTurnsControllerBase
    {
        UDFController objUDFDAL = new UDFController();

        // GET: /WorkOrder/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WorkOrderList()
        {
            Session["RoomAllWO"] = new WorkOrderDAL(SessionHelper.EnterPriseDBName).GetAllWorkOrders(SessionHelper.RoomID, SessionHelper.CompanyID, new string[] { "WorkOrder", "Reqn" });
            return View();
        }

        public ActionResult WOMasterListAjax(JQueryDataTableParamModel param)
        {
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);

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

            bool IsArchived = bool.Parse(Request["IsArchived"]);
            bool IsDeleted = bool.Parse(Request["IsDeleted"]);


            // set the default column sorting here, if first time then required to set 
            //if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID asc";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";


            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            string WOTypes = "WorkOrder','Reqn";
            int TotalRecordCount = 0;


            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierID, true, WOTypes, GetWOStatus(), Convert.ToString(SessionHelper.RoomDateFormat)).ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);

        }
        public DataTable GetWOStatus()
        {
            DataTable WOStatus = new DataTable();
            DataColumn Dt = new DataColumn("StatusName", typeof(string));
            Dt.MaxLength = 255;
            WOStatus.Columns.Add(Dt);
            Dt = new DataColumn("StatusValue", typeof(string));
            Dt.MaxLength = 255;
            WOStatus.Columns.Add(Dt);

            WOStatus.Rows.Add("Open", ResWorkOrder.Open);
            WOStatus.Rows.Add("Close", ResWorkOrder.Close);
            return WOStatus;
        }
        [HttpPost]
        public JsonResult SaveWO(WorkOrderDTO objDTO)
        {
            if (!ModelState.IsValid)
                return Json(new { Message = ResMessage.InvalidModel, Status = "fail" }, JsonRequestBehavior.AllowGet);

            string message = "";
            string status = "";
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objC = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            if ((objDTO.AssetGUID ?? Guid.Empty) == Guid.Empty && (objDTO.ToolGUID ?? Guid.Empty) == Guid.Empty)
            {
                objDTO.WOType = "WorkOrder";
            }
            else
            {
                objDTO.WOType = "Maint";
            }


            if (objDTO.ID == 0)
            {
                //-------------------------Check Work Order Duplication-------------------------
                //
                string strOK = "";
                RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByID(eTurnsWeb.Helper.SessionHelper.RoomID);
                if (roomDTO.IsAllowWorkOrdersDuplicate != true)
                {
                    strOK = objC.DuplicateCheck(objDTO.WOName, "add", objDTO.ID, "WorkOrder", "WOName", SessionHelper.RoomID, SessionHelper.CompanyID);
                }

                //------------------------------------------------------------------------------
                //
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResWorkOrder.WOName, objDTO.WOName);
                    status = "duplicate";
                }
                else
                {
                    try
                    {
                        objDTO.ReleaseNumber = obj.GenerateAndGetReleaseNumber(objDTO.WOName, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.WhatWhereAction = "Work Order";
                        obj.Insert(objDTO);
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        RequisitionMasterDAL objrequ = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<RequisitionMasterDTO> RequisitionList;
                        RequisitionMasterDTO requistion = new RequisitionMasterDTO();
                        RequisitionList = objrequ.GetAllRecords(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.IsArchived ?? false, objDTO.IsDeleted ?? false);
                        requistion = RequisitionList.Where(r => r.WorkorderGUID == objDTO.GUID).FirstOrDefault();
                        if (requistion != null)
                        {
                            CustomerMasterDAL objcust = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                            CustomerMasterDTO customer = new CustomerMasterDTO();
                            customer = objcust.GetRecordByGUID(objDTO.CustomerGUID ?? Guid.Empty, objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.IsArchived ?? false, objDTO.IsDeleted ?? false);
                            if (customer != null)
                            {
                                requistion.CustomerID = customer.ID;
                                requistion.Customer = customer.Customer;
                                requistion.CustomerGUID = objDTO.CustomerGUID;
                                objrequ.Edit(requistion);
                            }
                            if (objDTO.CustomerGUID == null)
                            {
                                requistion.CustomerID = null;
                                requistion.Customer = null;
                                requistion.CustomerGUID = null;
                                objrequ.Edit(requistion);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        message = ResMessage.SaveErrorMsg;
                        status = "fail";
                    }
                }
            }
            else
            {
                //-------------------------Check Work Order Duplication-------------------------
                //
                string strOK = "";
                RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByID(eTurnsWeb.Helper.SessionHelper.RoomID);
                if (roomDTO.IsAllowWorkOrdersDuplicate != true)
                {
                    strOK = objC.DuplicateCheck(objDTO.WOName, "edit", objDTO.ID, "WorkOrder", "WOName", SessionHelper.RoomID, SessionHelper.CompanyID);
                }

                //------------------------------------------------------------------------------
                //
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResWorkOrder.WOName, objDTO.WOName);
                    status = "duplicate";
                }
                else
                {
                    try
                    {
                        objDTO.ReleaseNumber = obj.GenerateAndGetReleaseNumber(objDTO.WOName, objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        objDTO.WhatWhereAction = "Work Order";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.EditedFrom = "Web";
                        obj.Edit(objDTO);
                        if (objDTO.WOStatus == "Close" && (objDTO.MaintenanceGUID ?? Guid.Empty) != Guid.Empty)
                        {
                            ToolsMaintenanceDAL objToolsMaintenanceDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);

                            objToolsMaintenanceDAL.CloseMaintenanceOnWOClose(objDTO.GUID, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.RoomDateFormat);

                        }
                        message = ResMessage.SaveMessage;
                        status = "ok";

                        RequisitionMasterDAL objrequ = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<RequisitionMasterDTO> RequisitionList;
                        RequisitionMasterDTO requistion = new RequisitionMasterDTO();
                        RequisitionList = objrequ.GetAllRecords(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.IsArchived ?? false, objDTO.IsDeleted ?? false);
                        requistion = RequisitionList.Where(r => r.WorkorderGUID == objDTO.GUID).FirstOrDefault();
                        if (requistion != null)
                        {
                            CustomerMasterDAL objcust = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                            CustomerMasterDTO customer = new CustomerMasterDTO();
                            customer = objcust.GetRecordByGUID(objDTO.CustomerGUID ?? Guid.Empty, objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.IsArchived ?? false, objDTO.IsDeleted ?? false);
                            if (customer != null)
                            {
                                requistion.CustomerID = customer.ID;
                                requistion.Customer = customer.Customer;
                                requistion.CustomerGUID = objDTO.CustomerGUID;
                                objrequ.Edit(requistion);
                            }
                            if (objDTO.CustomerGUID == null)
                            {
                                requistion.CustomerID = null;
                                requistion.Customer = null;
                                requistion.CustomerGUID = null;
                                objrequ.Edit(requistion);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        message = ResMessage.SaveErrorMsg + " " + Convert.ToString(ex);
                        status = "fail";
                    }
                }
            }
            Session["IsInsert"] = "True";

            return Json(new { Message = message, Status = status, ID = objDTO.ID, GUID = objDTO.GUID }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WOCreate()
        {
            //string nextWONo = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
            string nextWONo = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID);
            Int64 DefualtRoomSupplier = 0;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);

            List<SupplierMasterDTO> lstSupplier = null;
            DefualtRoomSupplier = objRoomDAL.GetRoomByID(RoomID).DefaultSupplierID.GetValueOrDefault(0);
            WorkOrderDTO objDTO = new WorkOrderDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                IsArchived = false,
                IsDeleted = false,
                //WOName = "#W" + nextWONo,
                WOName = nextWONo,
                WOStatus = "Open",
                SupplierId = DefualtRoomSupplier,
            };
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            lstSupplier = objSupDAL.GetAllRecords(RoomID, CompanyID, false, false, false).OrderBy(x => x.SupplierName).ToList();
            if (SessionHelper.UserSupplierID > 0)
                lstSupplier = lstSupplier.Where(x => x.ID == SessionHelper.UserSupplierID).OrderBy(x => x.SupplierName).ToList();
            lstSupplier.Insert(0, null);

            ViewBag.SupplierList = lstSupplier;

            objDTO.SupplierAccountGuid = Guid.Empty;
            SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(Convert.ToInt64(objDTO.SupplierId), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            //objDTO.IsRecordEditable = IsRecordEditable(objDTO);

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("WorkOrder");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.Customer.Trim()).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> technicianlist = objTechMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            technicianlist.ForEach(t => t.Technician = Convert.ToString(t.TechnicianCode + "-" + t.Technician));
            ViewBag.TechnicianBAG = technicianlist;

            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetRoomWiseAssetMaster(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetCachedDataNew(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.Open, Value = "Open" });

            ViewBag.WOStatusBag = ItemType;

            ViewBag.WOTypeBag = GetWOType();

            return PartialView("_CreateWorkOrder", objDTO);
        }

        [NonAction]
        private List<CommonDTO> GetWOType()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = "Work Order" });
            ItemType.Add(new CommonDTO() { Text = "Requisition" });
            ItemType.Add(new CommonDTO() { Text = "Tool Service" });
            ItemType.Add(new CommonDTO() { Text = "Asset Service" });
            return ItemType;
        }
        [NonAction]
        private List<CommonDTO> GetWOStaus()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.Open, Value = "Open" });
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.Close, Value = "Close" });
            return ItemType;
        }

        public ActionResult WOEdit(string WorkOrderGUID)
        {
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            Guid mntsGUID = Guid.Empty;

            List<SupplierMasterDTO> lstSupplier = null;
            if (Request["mntsGUID"] != null)
            {
                Guid.TryParse(Request["mntsGUID"].ToString(), out mntsGUID);
            }

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            bool IsHitory = false;
            if (Request["IsHistory"] != null && !string.IsNullOrEmpty(Request["IsHistory"].ToString()))
                IsHitory = bool.Parse(Request["IsHistory"].ToString());



            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            //RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);

            WorkOrderDTO objDTO = obj.GetWoByGUId(Guid.Parse(WorkOrderGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
            string RequiNumber = string.Empty;
            IEnumerable<WorkOrderDTO> lstobjDTO = obj.GetRequistionByWO(Guid.Parse(WorkOrderGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
            foreach (WorkOrderDTO wo in lstobjDTO)
            {
                if (!string.IsNullOrWhiteSpace(RequiNumber))
                    RequiNumber += "," + wo.RequisitionNumber;
                else
                    RequiNumber = wo.RequisitionNumber;
            }
            objDTO.RequisitionNumber = RequiNumber;
            objDTO.MaintenanceGUID = mntsGUID;
            if (IsHitory)
                objDTO.IsHistory = true;

            if (IsDeleted || IsArchived || objDTO.WOStatus == "Close")
            {
                ViewBag.ViewOnly = true;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("WorkOrder");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            //CustomerMasterController objCustApi = new CustomerMasterController();
            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.Customer.Trim()).ToList();

            //TechnicianController objTechMasterApi = new TechnicianController();
            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> technicianlist = objTechMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            technicianlist.ForEach(t => t.Technician = Convert.ToString(t.TechnicianCode + "-" + t.Technician));
            ViewBag.TechnicianBAG = technicianlist;

            //GXPRConsignedJobController objGXPRMasterApi = new GXPRConsignedJobController();
            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetRoomWiseAssetMaster(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetCachedDataNew(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            //List<SupplierMasterDTO> lstSupplier = null;
            SupplierMasterDAL objSupDAL = null;
            objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            lstSupplier = objSupDAL.GetAllRecords(RoomID, CompanyID, false, false, false).OrderBy(x => x.SupplierName).ToList();

            if (SessionHelper.UserSupplierID > 0)
                lstSupplier = lstSupplier.Where(x => x.ID == SessionHelper.UserSupplierID).ToList();
            //if (objDTO != null && (objDTO.SupplierId ?? 0) > 0)
            //{
            //    SupplierMasterDTO oSupplier = objSupDAL.GetRecord(objDTO.SupplierId, RoomID, CompanyID, false);
            //}



            lstSupplier.Insert(0, null);
            ViewBag.SupplierList = lstSupplier;
            //if (objDTO != null && (objDTO.SupplierId ?? 0) > 0)
            {
                SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
                ViewBag.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(objDTO.SupplierId.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
            }
            return PartialView("_CreateWorkOrder", objDTO);
        }

        public JsonResult DeleteWOMasterRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                //WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                //response=obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.WorkOrder.ToString(), true, SessionHelper.UserID);
                //return "ok";
                eTurns.DAL.CacheHelper<IEnumerable<WorkOrderDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadItemMasterModel(string ParentId, string ParentGuid)
        {
            //ItemModelPerameter obj = new ItemModelPerameter()
            //{
            //    AjaxURLAddItemToSession = "~/WorkOrder/AddItemToDetailTable/",
            //    PerentID = ParentId,
            //    PerentGUID = ParentGuid,
            //    ModelHeader = eTurns.DTO.ResWorkOrder.ModelHeader,
            //    AjaxURLAddMultipleItemToSession = "~/WorkOrder/AddItemToDetailTable/",
            //    AjaxURLToFillItemGrid = "~/WorkOrder/GetItemsModelMethod/",
            //    CallingFromPageName = "WO"
            //};
            ItemMasterDAL objPullNew = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            bool IsAllowConsignedCredit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);
            Session["PullItemList"] = objPullNew.GetPulledItemsForModel(0,
                                                            1000,
                                                            out TotalRecordCount,
                                                            string.Empty,
                                                            "Id desc",
                                                            SessionHelper.RoomID,
                                                            SessionHelper.CompanyID,
                                                            false,
                                                            false,
                                                            SessionHelper.UserSupplierID,
                                                            true, IsAllowConsignedCredit, true, SessionHelper.UserID, "pull", Convert.ToString(SessionHelper.RoomDateFormat), true, null, null, Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)
                                                            ).ToList();

            if (!String.IsNullOrEmpty(Convert.ToString(Request.QueryString["mntsGUID"])))
            {
                Guid mntsGUID = Guid.Empty;
                Guid.TryParse(Convert.ToString(Request.QueryString["mntsGUID"]), out mntsGUID);
                ViewBag.mntsGUID = mntsGUID;
            }

            if (!String.IsNullOrEmpty(Convert.ToString(Request.QueryString["mntsGUID"])) && !String.IsNullOrEmpty(Convert.ToString(Request.QueryString["firsttime"])))
            {
                ViewBag.FirstTimePopup = Convert.ToString(Request.QueryString["firsttime"]);
            }
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/QuickList/AddItemToSession/",
                ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToSessionMultiple/",
                AjaxURLToFillItemGrid = "~/Pull/GetItemsModelMethod/",
                IsProjectSpendMandatoryInRoom = new RoomDAL(SessionHelper.EnterPriseDBName).GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).IsProjectSpendMandatory,
            };
            TempData["WorkOrderGUID"] = ParentGuid;
           
            return PartialView("~/Views/Pull/_NewPull.cshtml", obj);
        }

        //public ActionResult GetItemsModelMethod(JQueryDataTableParamModel param)
        //{
        //    ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

        //    int PageIndex = int.Parse(param.sEcho);
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


        //    // set the default column sorting here, if first time then required to set 
        //    if (sortColumnName == "0" || sortColumnName == "undefined")
        //        sortColumnName = "ID";

        //    if (sortDirection == "asc")
        //        sortColumnName = sortColumnName + " asc";
        //    else
        //        sortColumnName = sortColumnName + " desc";

        //    string searchQuery = string.Empty;

        //    int TotalRecordCount = 0;
        //    List<WorkOrderLineItemsDTO> objQLItems = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName).GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.WorkOrderGUID == Guid.Parse(Request["ParentGUID"].ToString())).ToList();
        //    string ItemsIDs = "";
        //    if (objQLItems != null && objQLItems.Count > 0)
        //    {
        //        foreach (var item in objQLItems)
        //        {
        //            if (!string.IsNullOrEmpty(ItemsIDs))
        //                ItemsIDs += ",";

        //            ItemsIDs += item.ItemGUID.ToString();
        //        }
        //    }
        //    // .Where(x=>x.ItemType != 4); , as Labour Type item not required in this module
        //    var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsIDs,"labor");

        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount,
        //        aaData = DataFromDB
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}

        public JsonResult UncloseWorkOrder(Guid WOGUID)
        {
            string message = string.Empty;
            string status = string.Empty;

            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            //WorkOrderDTO objDTO = obj.GetWoByGUId(Guid.Parse(WOGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
            WorkOrderDTO objDTO = obj.GetWoByGUId(WOGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objDTO != null)
            {
                objDTO.WOStatus = "Open";
                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                try
                {
                    objDTO.WhatWhereAction = "Work Order";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.EditedFrom = "Web";
                    obj.Edit(objDTO);
                    message = ResMessage.SaveMessage;
                    status = "ok";
                    eTurns.DAL.CacheHelper<IEnumerable<WorkOrderDTO>>.InvalidateCache();
                }
                catch (Exception)
                {
                    message = ResMessage.SaveErrorMsg;
                    status = "fail";
                }
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddItemToDetailTable(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            WorkOrderLineItemsDTO[] QLDetails = s.Deserialize<WorkOrderLineItemsDTO[]>(para);

            WorkOrderLineItemsDAL objApi = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDAL ObjItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            foreach (WorkOrderLineItemsDTO item in QLDetails)
            {
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;

                if (item.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    ItemMasterDTO ObjItemDTO = ObjItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                    if (ObjItemDTO != null)
                        item.ItemType = ObjItemDTO.ItemType;
                }


                //if (Guid.Parse(item.GUID.ToString()) != Guid.Empty)
                //{
                //    if (item.GUID == item.ItemGUID)
                //        objApi.Edit(item);
                //}
                //else
                //{
                //    List<WorkOrderLineItemsDTO> tempDTO = objApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.WorkOrderGUID == item.WorkOrderGUID && x.ItemGUID == item.ItemGUID).ToList();
                //    if (tempDTO == null || tempDTO.Count == 0)
                //        objApi.Insert(item);
                //}
            }

            message = "Item's Quantity/Amount updated successfully.";
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadWOItems(string WorkOrderGUID)
        {
            ViewBag.WorkOrderGUID = WorkOrderGUID;
            if (Request["mntsGUID"] != null)
            {
                ViewBag.mntsGUID = Convert.ToString(Request["mntsGUID"]);
            }

            WorkOrderDAL objDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO = objDAL.GetRecord(WorkOrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.WorkOrderDTO = objDTO;
            bool IsHitory = false;
            if (Request["IsHistory"] != null && !string.IsNullOrEmpty(Request["IsHistory"].ToString()))
                IsHitory = bool.Parse(Request["IsHistory"].ToString());

            if (objDTO.IsDeleted.GetValueOrDefault(false) || objDTO.IsArchived.GetValueOrDefault(false))
            {
                ViewBag.ViewOnly = true;
            }
            if (IsHitory)
                ViewBag.ViewOnly = true;
            if (objDTO.WOStatus == "Close")
                ViewBag.ViewOnly = true;

            //WorkOrderLineItemsDAL obj = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            var ReqDetaiData = obj.GetAllWorkOrderRecords(Convert.ToInt64(SessionHelper.RoomID), Convert.ToInt64(SessionHelper.CompanyID), WorkOrderGUID).ToList();

            return PartialView("_CreateWOItems", ReqDetaiData);
        }

        public ActionResult LoadWOItemsReq(string WorkOrderGUID)
        {
            ViewBag.WorkOrderGUID = WorkOrderGUID;

            WorkOrderDAL objDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO = objDAL.GetRecord(WorkOrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

            bool IsHitory = false;
            if (Request["IsHistory"] != null && !string.IsNullOrEmpty(Request["IsHistory"].ToString()))
                IsHitory = bool.Parse(Request["IsHistory"].ToString());

            if (objDTO.IsDeleted.GetValueOrDefault(false) || objDTO.IsArchived.GetValueOrDefault(false))
            {
                ViewBag.ViewOnly = true;
            }
            if (IsHitory)
                ViewBag.ViewOnly = true;
            if (objDTO.WOStatus == "Close")
                ViewBag.ViewOnly = true;

            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            //var ReqDetaiData = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.WorkOrderDetailGUID.ToString() == WorkOrderGUID).ToList();
            var ReqDetaiData = obj.GetWOPulls(Guid.Parse(WorkOrderGUID), SessionHelper.RoomID, SessionHelper.CompanyID);


            return PartialView("_CreateWOItemsReq", ReqDetaiData);
        }

        //public string WOItemsDelete(string ids)
        //{
        //    try
        //    {
        //        WorkOrderLineItemsDAL obj = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
        //        obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
        //        return "ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        public string WOPulls(string WODetailGUID)
        {
            // used guid as we are passing WODetailGUID = Pull GUID ... 
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            //var DataFromDB = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.GUID.ToString() == WODetailGUID);
            var DataFromDB = obj.GetWOPulls(Guid.Parse(WODetailGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                ViewBag.WODetailGUID = WODetailGUID;
                ViewBag.WODetailGUIDUnique = WODetailGUID + DateTimeUtility.DateTimeNow.Ticks.ToString();
            }
            else
            {
                ViewBag.WODetailGUID = WODetailGUID;
                ViewBag.WODetailGUIDUnique = WODetailGUID + DateTimeUtility.DateTimeNow.Ticks.ToString();
            }
            ViewBag.IsDetail = false;
            return RenderRazorViewToString("_WOPulls", DataFromDB);
        }

        public ActionResult PullMasterListByWOAjax(JQueryDataTableParamModel param)
        {
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);

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

            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            PullMasterDAL objPullDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            //var DataFromDB = objPullDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, sortColumnName).Where(x => x.WorkOrderDetailGUID.ToString() == Request["WODetailGUID"].ToString());
            var DataFromDB = objPullDAL.GetWOPulls(Guid.Parse(Request["WODetailGUID"].ToString()), SessionHelper.RoomID, SessionHelper.CompanyID);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);

        }

        public ActionResult PullMasterListByWOAjaxByPullID(JQueryDataTableParamModel param)
        {
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);

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

            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            // used guid as we are passing WODetailGUID = Pull GUID ... 
            int TotalRecordCount = 0;
            PullMasterDAL objPullDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            //var DataFromDB = objPullDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.GUID.ToString() == Request["WODetailGUID"].ToString());
            var DataFromDB = objPullDAL.GetWOPulls(Guid.Parse(Request["WODetailGUID"].ToString()), SessionHelper.RoomID, SessionHelper.CompanyID);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);

        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public string WODetails(string WorkOrderGUID)
        {

            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            //var DataFromDB = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.WorkOrderDetailGUID.ToString() == WorkOrderGUID);
            var DataFromDB = obj.GetWOPulls(Guid.Parse(WorkOrderGUID), SessionHelper.RoomID, SessionHelper.CompanyID);

            if (DataFromDB != null && DataFromDB.Count() > 0)
            {
                ViewBag.WODetailGUID = WorkOrderGUID;
                ViewBag.WODetailGUIDUnique = WorkOrderGUID + DateTimeUtility.DateTimeNow.Ticks.ToString();
            }
            else
            {
                ViewBag.WODetailGUID = WorkOrderGUID;
                ViewBag.WODetailGUIDUnique = WorkOrderGUID + DateTimeUtility.DateTimeNow.Ticks.ToString();
            }
            ViewBag.IsDetail = true;
            return RenderRazorViewToString("_WOPulls", DataFromDB);
        }

        //public ActionResult ItemWODetailsAjax(JQueryDataTableParamModel param)
        //{
        //    string WorkOrderGUID = Request["ItemID"].ToString();

        //    ///////////// requried when paging needs in this method /////////////////
        //    int PageIndex = int.Parse(param.sEcho);
        //    int PageSize = param.iDisplayLength;
        //    var sortDirection = Request["sSortDir_0"];
        //    var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    string sortColumnName = string.Empty;
        //    int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
        //    sortColumnName = Request["SortingField"].ToString();

        //    bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
        //    bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


        //    // set the default column sorting here, if first time then required to set 
        //    if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
        //        sortColumnName = "ID";

        //    if (sortDirection == "asc")
        //        sortColumnName = sortColumnName + " asc";
        //    else
        //        sortColumnName = sortColumnName + " desc";

        //    ///////////// requried when paging needs in this method /////////////////

        //    ViewBag.WorkOrderGUID = WorkOrderGUID;
        //    WorkOrderLineItemsDAL objAPI = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
        //    int TotalRecordCount = 0;
        //    var objModel = objAPI.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, WorkOrderGUID);

        //    var Result = new List<WorkOrderLineItemsDTO>();

        //    Result = (from x in objModel
        //              group x by new { x.ItemNumber }
        //                  into grp
        //                  select new WorkOrderLineItemsDTO
        //                  {
        //                      ItemNumber = grp.Key.ItemNumber,
        //                      Quantity = grp.Sum(x => x.Quantity),
        //                      PulledQuantity = grp.Sum(x => x.PulledQuantity),
        //                  }).ToList();

        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
        //        aaData = Result
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}

        private object GetUDFDataPageWise(string PageName)
        {
            //UDFApiController objUDFApiController = new UDFApiController();
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, PageName, SessionHelper.RoomID);

            var result = from c in DataFromDB
                         select new UDFDTO
                         {
                             ID = c.ID,
                             CompanyID = c.CompanyID,
                             Room = c.Room,
                             UDFTableName = c.UDFTableName,
                             UDFColumnName = c.UDFColumnName,
                             UDFDefaultValue = c.UDFDefaultValue,
                             UDFOptionsCSV = c.UDFOptionsCSV,
                             UDFControlType = c.UDFControlType,
                             UDFIsRequired = c.UDFIsRequired,
                             UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                         };
            return result;
        }
        public ActionResult GetWorkOrderFiles(Guid WorkOrderGuid)
        {
            // try
            {
                WorkOrderImageDetailDAL objWorkOrderImageDetailDAL = new WorkOrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                List<WorkOrderImageDetailDTO> listWorkOrderImageDetail = objWorkOrderImageDetailDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, WorkOrderGuid).ToList();
                Dictionary<string, Guid> retData = new Dictionary<string, Guid>();
                foreach (WorkOrderImageDetailDTO woimg in listWorkOrderImageDetail)
                {
                    retData.Add(woimg.WOImageName, woimg.GUID);
                }
                return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
            }
            //catch(Exception ex)
            //{
            //   // return Json(new { DDData = null }, JsonRequestBehavior.AllowGet);
            //}
        }
        public void DeleteExistingFiles(string FileId, Guid WorkOrderGuid)
        {
            try
            {
                WorkOrderImageDetailDAL objWorkOrderImageDetailDAL = new WorkOrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                objWorkOrderImageDetailDAL.DeleteRecords(FileId, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, WorkOrderGuid);
            }
            catch (Exception ex)
            {

            }
        }

        #region WORK Order History
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkOrderHistoryView(Int64 ID)
        {

            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO = obj.GetHistoryRecord(ID);

            ViewBag.UDFs = GetUDFDataPageWise("WorkOrder");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.Customer).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.TechnicianBAG = objTechMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetRoomWiseAssetMaster(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetCachedDataNew(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            return PartialView("_CreateWorkOrder_History", objDTO);
        }


        public ActionResult WorkOrderHistoryViewFromMaintenance(string GUID)
        {

            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO1 = obj.GetRecord(GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            WorkOrderDTO objDTO = obj.GetHistoryRecordForMaintenance(objDTO1.ID);

            ViewBag.UDFs = GetUDFDataPageWise("WorkOrder");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.Customer).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.TechnicianBAG = objTechMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetRoomWiseAssetMaster(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetCachedDataNew(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            return PartialView("_CreateWorkOrder_History", objDTO);
        }

        public ActionResult WorkOrderLineItemsDetails(string GUID)
        {
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO = obj.GetRecord(GUID, SessionHelper.RoomID, SessionHelper.CompanyID);

            ViewBag.UDFs = GetUDFDataPageWise("WorkOrder");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.Customer).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.TechnicianBAG = objTechMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetRoomWiseAssetMaster(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetCachedDataNew(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            ViewBag.ViewOnly = true;

            objDTO.IsHistory = true;

            return PartialView("_CreateWorkOrder", objDTO);
        }

        public ActionResult WorkOrderLineItemsDetailsForReq(string GUID)
        {
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO = obj.GetRecord(GUID, SessionHelper.RoomID, SessionHelper.CompanyID);

            ViewBag.UDFs = GetUDFDataPageWise("WorkOrder");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.Customer).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.TechnicianBAG = objTechMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetRoomWiseAssetMaster(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetCachedDataNew(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            ViewBag.ViewOnly = true;

            objDTO.IsHistory = true;

            return PartialView("_CreateWorkOrderReq", objDTO);
        }

        /// <summary>
        /// LoadOrderLineItemsHistory
        /// </summary>
        /// <param name="historyID"></param>
        /// <returns></returns>
        public ActionResult LoadWorkOrderLineItemsHistory(Int64 historyID, string WOHistoryGUID)
        {
            WorkOrderDTO objDTO = null;
            objDTO = new WorkOrderDAL(SessionHelper.EnterPriseDBName).GetHistoryRecord(historyID);
            if (objDTO != null)
            {
                objDTO.WorkOrderListItem = new PullMasterDAL(SessionHelper.EnterPriseDBName).GetHistoryRecordWORecords(Guid.Parse(WOHistoryGUID)).ToList();
            }
            return PartialView("_CreateWOItems_History", objDTO);
        }

        #endregion


        public ActionResult DownloadWorkOrderDocument(List<Guid> lstWOGuids)
        {
            List<string> retData = new List<string>();
            foreach (var WOGuids in lstWOGuids)
            {
                WorkOrderImageDetailDAL objWorkOrderImageDetailDAL = new WorkOrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                WorkOrderDAL objWODAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                List<WorkOrderImageDetailDTO> listWorkOrderImageDetail = objWorkOrderImageDetailDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, WOGuids).ToList();
                string WorkOrderFilePath = System.Configuration.ConfigurationManager.AppSettings["WorkOrderFilePath"].ToString();
                WorkOrderDTO woDTO = objWODAL.GetRecord(WOGuids.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                string WorkOrderPath = HttpContext.Request.Url.AbsoluteUri.Replace(HttpContext.Request.RawUrl, "") + "/Uploads/" + WorkOrderFilePath + "/" + SessionHelper.CompanyID + "/" + woDTO.ID;

                foreach (WorkOrderImageDetailDTO woimg in listWorkOrderImageDetail)
                {
                    retData.Add(WorkOrderPath + "/" + woimg.WOImageName);
                }
            }

            //return File(data, "application/csv", dtoModuleDetail.ResourceModuleName + "_" + dtoModuleDetail.PageName + "_MobileRes_" + DateTimeUtility.DateTimeNow.ToString("yyyyMMddHHmmss") + ".csv");
            return Json(new { Message = "Sucess", Status = true, ReturnFiles = retData }, JsonRequestBehavior.AllowGet);
        }
    }
}

