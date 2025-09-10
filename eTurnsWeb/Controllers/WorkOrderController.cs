using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using eTurns.DTO.Utils;
using System.Web;
using System.Security.Claims;
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
            //// Session["RoomAllWO"] = new WorkOrderDAL(SessionHelper.EnterPriseDBName).GetAllWorkOrders(SessionHelper.RoomID, SessionHelper.CompanyID, new string[] { "WorkOrder", "Reqn", "Maint" });
            //return View();
            return View("WorkOrderListNew");
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

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;
            string WOTypes = "WorkOrder','Reqn','Maint";
            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, true, WOTypes, GetWOStatus(), Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone).ToList();

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
            WOStatus.Rows.Add("Close", ResWorkOrder.Closed);
            return WOStatus;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveWONew(WorkOrderModel objModel)
        {
            var objDTO = objModel.WorkOrderDTO;
            objDTO.UDF1 = objModel.UDF1;
            objDTO.UDF2 = objModel.UDF2;
            objDTO.UDF3 = objModel.UDF3;
            objDTO.UDF4 = objModel.UDF4;
            objDTO.UDF5 = objModel.UDF5;

            return SaveWO(objDTO);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveWO(WorkOrderDTO objDTO)
        {
            var ignoreProperty = new List<string>() { };
            bool isFromMaintanance = Request.UrlReferrer == null ? false : Request.UrlReferrer.LocalPath.Contains("/Assets/Maintenance");
            if (isFromMaintanance)
            {
                ignoreProperty.Add("SupplierId");
                ignoreProperty.Add("SupplierAccountGuid");
            }

            var valResult = DTOCommonUtils.ValidateDTO<WorkOrderDTO>(objDTO, ControllerContext, ignoreProperty);

            if (valResult.HasErrors())
            {
                return Json(new { Message = ResMessage.InvalidModel, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

            //if (!ModelState.IsValid)
            //{
            //    return Json(new { Message = ResMessage.InvalidModel, Status = "fail" }, JsonRequestBehavior.AllowGet);
            //}

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

            bool isWorkorderEditWithCloseStatus = false;

            if (objDTO.ID == 0)
            {
                //-------------------------Check Work Order Duplication-------------------------
                //
                string strOK = "";
                // RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);

                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string columnList = "ID,RoomName,IsAllowWorkOrdersDuplicate";
                RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

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
                        //IEnumerable<RequisitionMasterDTO> RequisitionList;
                        RequisitionMasterDTO requistion = new RequisitionMasterDTO();
                        //RequisitionList = objrequ.GetAllRecords(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.IsArchived ?? false, objDTO.IsDeleted ?? false);
                        //requistion = RequisitionList.Where(r => r.WorkorderGUID == objDTO.GUID).FirstOrDefault();
                        requistion = objrequ.GetRequisitionsByWOPlain(objDTO.GUID, objDTO.Room ?? 0, objDTO.CompanyID ?? 0, null).OrderByDescending(t => t.ID).FirstOrDefault();
                        if (requistion != null)
                        {
                            CustomerMasterDAL objcust = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                            CustomerMasterDTO customer = new CustomerMasterDTO();
                            customer = objcust.GetCustomerByGUID(objDTO.CustomerGUID ?? Guid.Empty);
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
                    catch
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
                //  RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string columnList = "ID,RoomName,IsAllowWorkOrdersDuplicate";
                RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

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
                        WorkOrderDTO workorderBeforeEdit=null;
                        
                        if (objDTO.WOStatus == "Close")
                        {
                            workorderBeforeEdit= obj.GetWorkOrderByIDPlain(objDTO.ID);
                        }
                        
                        objDTO.ReleaseNumber = obj.GenerateAndGetReleaseNumber(objDTO.WOName, objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        objDTO.WhatWhereAction = "Work Order";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.EditedFrom = "Web";
                        obj.Edit(objDTO);
                        if (objDTO.WOStatus == "Close" && (objDTO.MaintenanceGUID ?? Guid.Empty) != Guid.Empty)
                        {
                            ToolsMaintenanceDAL objToolsMaintenanceDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
                            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                            objToolsMaintenanceDAL.CloseMaintenanceOnWOClose(objDTO.GUID, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.RoomDateFormat, CurrentTimeZone);

                        }
                        message = ResMessage.SaveMessage;
                        status = "ok";

                        if (objDTO.WOStatus == "Close" && workorderBeforeEdit != null && workorderBeforeEdit.WOStatus != "Close")
                        {
                            isWorkorderEditWithCloseStatus = true;
                        }

                        RequisitionMasterDAL objrequ = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                        //IEnumerable<RequisitionMasterDTO> RequisitionList;
                        RequisitionMasterDTO requistion = new RequisitionMasterDTO();
                        //RequisitionList = objrequ.GetAllRecords(objDTO.Room ?? 0, objDTO.CompanyID ?? 0, objDTO.IsArchived ?? false, objDTO.IsDeleted ?? false);
                        //requistion = RequisitionList.Where(r => r.WorkorderGUID == objDTO.GUID).FirstOrDefault();
                        requistion = objrequ.GetRequisitionsByWOPlain(objDTO.GUID, objDTO.Room ?? 0, objDTO.CompanyID ?? 0, null).OrderByDescending(t => t.ID).FirstOrDefault();
                        if (requistion != null)
                        {
                            CustomerMasterDAL objcust = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                            CustomerMasterDTO customer = new CustomerMasterDTO();
                            customer = objcust.GetCustomerByGUID(objDTO.CustomerGUID ?? Guid.Empty);
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


            if ((Session["WOEvent"] != null && !string.IsNullOrWhiteSpace(Convert.ToString(Session["WOEvent"]))) || isWorkorderEditWithCloseStatus)
            {

                if (status == "ok")
                {
                    try
                    {
                        string workOrderGUIDs = "<DataGuids>" + Convert.ToString(objDTO.GUID) + "</DataGuids>";
                        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                        NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);

                        if (Session["WOEvent"] != null && !string.IsNullOrWhiteSpace(Convert.ToString(Session["WOEvent"])) && !string.IsNullOrEmpty(Convert.ToString(Session["WOEvent"])))
                        {
                            string eventName = Convert.ToString(Session["WOEvent"]);
                            List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                            if (lstNotification != null && lstNotification.Count > 0)
                            {
                                objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, workOrderGUIDs);
                                Session["WOEvent"] = "";
                            }
                        }

                        if (isWorkorderEditWithCloseStatus)
                        {
                            string eventName = "OWOCL";
                            List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                            if (lstNotification != null && lstNotification.Count > 0)
                            {
                                objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, workOrderGUIDs);
                                Session["WOEvent"] = "";
                            }
                        }                       
                    }
                    catch (Exception ex)
                    {
                        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);

                    }
                }
            }


            Session["IsInsert"] = "True";

            return Json(new { Message = message, Status = status, ID = objDTO.ID, GUID = objDTO.GUID }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WOCreate()
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDAL workorderDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderModel workOrderModel = new WorkOrderModel();

            //string nextWONo = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
            //string nextWONo = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID);
            Int64 DefualtRoomSupplier = 0;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            //RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);

            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,DefaultSupplierID";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

            DefualtRoomSupplier = ROOMDTO.DefaultSupplierID.GetValueOrDefault(0);

            objAutoNumber = objAutoSeqDAL.GetNextWorkOrderNumber(RoomID, CompanyID,SessionHelper.EnterPriceID);

            string nextWONo = objAutoNumber.OrderNumber;
            if (nextWONo != null && (!string.IsNullOrEmpty(nextWONo)))
            {
                nextWONo = nextWONo.Length > 22 ? nextWONo.Substring(0, 22) : nextWONo;
            }

            int ReleaseNo = 1;

            if (!string.IsNullOrWhiteSpace(nextWONo))
            {
                //var objOrderList = workorderDAL.GetAllRecords(RoomID, CompanyID, false, false);
                var objOrderList = workorderDAL.GetWorkOrdersByNamePlain(nextWONo, RoomID, CompanyID);
                if (objOrderList != null)
                {
                    IEnumerable<string> lstReleaseNo = objOrderList.Select(x => x.ReleaseNumber);
                    //IEnumerable<string> lstReleaseNo = objOrderList.Where(x => x.WOName.Equals(nextWONo)).Select(x => x.ReleaseNumber);
                    if (lstReleaseNo != null && lstReleaseNo.Count() > 0)
                        ReleaseNo = lstReleaseNo.Max(x => int.Parse(x ?? "0")) + 1;
                }
            }

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
                ReleaseNumber = ReleaseNo.ToString(),
                WOStatus = "Open",
                SupplierId = DefualtRoomSupplier,
            };
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
            {
                var strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
                var supplier = objSupDAL.GetSupplierByIDsPlain(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);

                if (supplier != null && supplier.Any())
                {
                    lstSupplier.AddRange(supplier);
                }
            }
            else
            {
                lstSupplier = objSupDAL.GetSupplierByRoomPlain(RoomID, CompanyID, false).OrderBy(x => x.SupplierName).ToList();
            }

            lstSupplier.Insert(0, null);
            //ViewBag.SupplierList = lstSupplier;
            workOrderModel.SupplierList = lstSupplier;

            objDTO.SupplierAccountGuid = Guid.Empty;
            SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(Convert.ToInt64(objDTO.SupplierId), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            workOrderModel.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(Convert.ToInt64(objDTO.SupplierId), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("WorkOrder");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();
            workOrderModel.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> technicianlist = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            technicianlist.ForEach(t =>
            {
                if (!string.IsNullOrEmpty(t.Technician))
                {
                    t.Technician = Convert.ToString(t.TechnicianCode + " --- " + t.Technician);
                }
                else
                {
                    t.Technician = Convert.ToString(t.TechnicianCode);
                }
            });

            //ViewBag.TechnicianBAG = technicianlist.OrderBy(t => t.TechnicianCode).ToList();
            workOrderModel.TechnicianBAG = technicianlist.OrderBy(t => t.TechnicianCode).ToList();
            //GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            workOrderModel.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
            workOrderModel.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            //JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.JobTypeBAG = objJobTypeApi.GetJobTypeByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);


            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.Open, Value = "Open" });
            //ViewBag.WOStatusBag = ItemType;
            workOrderModel.WOStatusBag = ItemType;
            //ViewBag.WOTypeBag = GetWOType();



            Session["WOEvent"] = "OWOC";

            workOrderModel.WorkOrderDTO = objDTO;

            return PartialView("_CreateWorkOrderNew", workOrderModel);
        }


        [NonAction]
        private List<CommonDTO> GetWOType()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeWorkorder });
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.Requisition });
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.ToolService });
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.AssetService });
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
            WorkOrderModel workOrderModel = new WorkOrderModel();
            Session["RoomAllWO"] = null;
            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
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
            WorkOrderDTO objDTO = new WorkOrderDTO();

            if (IsArchived)
            {
                objDTO = obj.GetWorkOrdersByGUIDFullJoinsArchieve(Guid.Parse(WorkOrderGUID));
                objDTO.IsArchived = true;
            }
            else
                objDTO = obj.GetWorkOrdersByGUIDFullJoins(Guid.Parse(WorkOrderGUID));

            objDTO.RequisitionNumber = (objDTO.RequisitionNumber ?? string.Empty).Trim(',');
            objDTO.ProjectSpendName = (objDTO.ProjectSpendName ?? string.Empty).Trim(',');
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
            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();
            workOrderModel.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();
            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> technicianlist = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            technicianlist.ForEach(t =>
            {
                if (!string.IsNullOrEmpty(t.Technician))
                {
                    t.Technician = Convert.ToString(t.TechnicianCode + " --- " + t.Technician);
                }
                else
                {
                    t.Technician = Convert.ToString(t.TechnicianCode);
                }
            });

            //ViewBag.TechnicianBAG = technicianlist;
            workOrderModel.TechnicianBAG = technicianlist;
            //GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            workOrderModel.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
            workOrderModel.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            //JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.JobTypeBAG = objJobTypeApi.GetJobTypeByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);

            //ViewBag.WOStatusBag = GetWOStaus();
            workOrderModel.WOStatusBag = GetWOStaus();
            //ViewBag.WOTypeBag = GetWOType();


            SupplierMasterDAL objSupDAL = null;
            objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
            {
                var strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
                var suppliers = objSupDAL.GetSupplierByIDsPlain(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);

                if (suppliers != null && suppliers.Any())
                {
                    lstSupplier.AddRange(suppliers);
                }
            }
            else
            {
                lstSupplier = objSupDAL.GetSupplierByRoomPlain(RoomID, CompanyID, false).OrderBy(x => x.SupplierName).ToList();
            }

            lstSupplier.Insert(0, null);
            //ViewBag.SupplierList = lstSupplier;
            workOrderModel.SupplierList = lstSupplier;

            SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(objDTO.SupplierId.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
            workOrderModel.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(objDTO.SupplierId.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);

            if (string.IsNullOrWhiteSpace(Convert.ToString(Session["WOEvent"])))
            {
                Session["WOEvent"] = "OWOE";
            }
            workOrderModel.WorkOrderDTO = objDTO;
            return PartialView("_CreateWorkOrderNew", workOrderModel);
        }

        public JsonResult DeleteWOMasterRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.WorkOrder.ToString(), true, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                
                #region schedule immediate mail sending on delete workorder
                try
                {
                    if (!string.IsNullOrEmpty(ids) && !string.IsNullOrWhiteSpace(ids))
                    {
                        string isDeleted = "<IsDeleted>true</IsDeleted>";
                        string workOrderGUIDs = "<DataGuids>" + ids + "</DataGuids>" + isDeleted;                        
                        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                        NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                        string eventName = "OWOD";
                        List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                        if (lstNotification != null && lstNotification.Count > 0)
                        {
                            objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, workOrderGUIDs);
                        }
                    }
                }
                catch { }

                #endregion

                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadToolsOnModel(string ParentGuid)
        {
            Guid WOGUID = Guid.Empty;
            Guid.TryParse(ParentGuid, out WOGUID);
            WorkOrderDTO objDTO = new WorkOrderDAL(SessionHelper.EnterPriseDBName).GetWorkOrderByGUIDPlain(WOGUID);

            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/WorkOrder/AddToolToDetailTable/",
                PerentID = objDTO.ID.ToString(),
                PerentGUID = ParentGuid,
                ModelHeader = "Tools",
                AjaxURLAddMultipleItemToSession = "~/WorkOrder/AddToolToDetailTable/",
                CallingFromPageName = "WO",
                ReqRequiredDate = SessionHelper.RoomDateFormat != null ? DateTimeUtility.DateTimeNow.ToString(SessionHelper.RoomDateFormat) : DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy"),
            };

            return PartialView("ToolsModel", obj);
        }

        public ActionResult LoadItemMasterModel(string ParentId, string ParentGuid)
        {
            ItemMasterDAL objPullNew = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            bool IsAllowConsignedCredit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);
            Guid mntsGUID = Guid.Empty;

            if (!String.IsNullOrEmpty(Convert.ToString(Request.QueryString["mntsGUID"])))
            {
                Guid.TryParse(Convert.ToString(Request.QueryString["mntsGUID"]), out mntsGUID);
                ViewBag.mntsGUID = mntsGUID;
            }

            if (!String.IsNullOrEmpty(Convert.ToString(Request.QueryString["mntsGUID"])) && !String.IsNullOrEmpty(Convert.ToString(Request.QueryString["firsttime"])))
            {
                ViewBag.FirstTimePopup = Convert.ToString(Request.QueryString["firsttime"]);
            }

            string Itempopupfor = "pull";
            string itemGUIDs = string.Empty;
            List<ToolsSchedulerDetailsDTO> lstitems = new List<ToolsSchedulerDetailsDTO>();

            if (mntsGUID != Guid.Empty && Convert.ToString(Request.QueryString["firsttime"]) == "yes")
            {
                Itempopupfor = "maint";
                ToolsMaintenanceDAL objToolsMaintenanceDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
                lstitems = objToolsMaintenanceDAL.GetSchedulerItems(mntsGUID);

                if (lstitems != null)
                {
                    itemGUIDs = string.Join(",", lstitems.Select(t => t.ItemGUID).Where(t => t != null).ToArray());
                }
            }

            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            Session["PullItemList"] = objPullNew.GetPulledItemsForModel(0,
                                                          int.MaxValue,
                                                          out TotalRecordCount,
                                                          string.Empty,
                                                          "Id desc",
                                                          SessionHelper.RoomID,
                                                          SessionHelper.CompanyID,
                                                          false,
                                                          false,

                                                          SessionHelper.UserSupplierIds,

                                                          true, IsAllowConsignedCredit, true, SessionHelper.UserID, Itempopupfor, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone,
                                                          true, null, itemGUIDs, Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)
                                                          ).ToList();

            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,IsProjectSpendMandatory,AllowPullBeyondAvailableQty";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");


            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/QuickList/AddItemToSession/",
                ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToSessionMultiple/",
                AjaxURLToFillItemGrid = "~/Pull/GetItemsModelMethod/",
                IsProjectSpendMandatoryInRoom = ROOMDTO.IsProjectSpendMandatory,
                AllowPullBeyondAvailableQty = ROOMDTO.AllowPullBeyondAvailableQty,
                CallingFromPageName = "WorkOrder"
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
            WorkOrderDTO objDTO = obj.GetWorkOrderByGUIDPlain(WOGUID);
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
                }
                catch (Exception)
                {
                    message = ResMessage.SaveErrorMsg;
                    status = "fail";
                }
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddToolToDetailTable(string para)
        {
            string message = ResToolMaster.ToolCheckOutSuccessfully; 
            string status = "ok";
            JavaScriptSerializer s = new JavaScriptSerializer();
            List<WorkOrderLineItemsDTO> QLDetails = s.Deserialize<List<WorkOrderLineItemsDTO>>(para);
            List<string> ErrorMessage = new List<string>();
            if (QLDetails != null && QLDetails.Count > 0)
            {
                List<ToolMasterDTO> objToolList = new List<ToolMasterDTO>();
                List<TechnicianMasterDTO> objTechnicianList = new List<TechnicianMasterDTO>();
                string ToolGUIDs = string.Join(",", QLDetails.Where(x => (x.ToolGUID ?? Guid.Empty) != Guid.Empty).Select(t => t.ToolGUID.GetValueOrDefault(Guid.Empty)).Distinct().ToArray());
                objToolList = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDsNormal(ToolGUIDs);

                string TechnicianGUIDs = string.Join(",", QLDetails.Where(x => (x.TechnicianGUID ?? Guid.Empty) != Guid.Empty).Select(t => t.TechnicianGUID.GetValueOrDefault(Guid.Empty)).Distinct().ToArray());
                objTechnicianList = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName).GetTechnicianByGUIDsPlain(TechnicianGUIDs, SessionHelper.RoomID, SessionHelper.CompanyID);

                foreach (var item in QLDetails)
                {
                    AssetsController toolController = new AssetsController();
                    TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                    string Technicain = "";
                    ToolMasterDTO toolDTO = objToolList.Where(x => x.GUID == item.ToolGUID.GetValueOrDefault(Guid.Empty)).FirstOrDefault();

                    if (item.TechnicianGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        TechnicianMasterDTO objTechnicianMasterDTO = objTechnicianList.Where(x => x.GUID == item.TechnicianGUID.GetValueOrDefault(Guid.Empty)).FirstOrDefault();
                        Technicain = objTechnicianMasterDTO.TechnicianCode;

                        if (!string.IsNullOrEmpty(objTechnicianMasterDTO.Technician))
                        {
                            Technicain = Technicain + " --- " + objTechnicianMasterDTO.Technician;
                        }

                        status = toolController.CheckOutCheckIn("co", (int)item.PulledQuantity, false, item.ToolGUID.GetValueOrDefault(Guid.Empty),
                                                  0, 0, 0, item.ToolCheckoutUDF1, item.ToolCheckoutUDF2, item.ToolCheckoutUDF3,
                                                  item.ToolCheckoutUDF4, item.ToolCheckoutUDF5, "", true, Technicain, null, item.WorkOrderGUID);
                        if (status == "ok")
                            ErrorMessage.Add(toolDTO.ToolName + "-" + toolDTO.Serial + ": " + ResCommon.Success); 
                        else
                        {
                            ErrorMessage.Add(toolDTO.ToolName + "-" + toolDTO.Serial + ": " + ResCommon.Fail); 
                            message = status;
                            status = "fail";

                        }
                        if (item.WorkOrderGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
                            objWOLDAL.UpdateWOItemAndTotalCost(item.WorkOrderGUID.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                        }
                    }
                    else
                    {
                        ErrorMessage.Add(toolDTO.ToolName + "-" + toolDTO.Serial + ": " + ResCommon.Fail);
                        status = "fail";
                        message = ResToolMaster.ReqTechnician; 
                    }

                }
            }

            return Json(new { Message = message, Status = status, StatusMessage = ErrorMessage }, JsonRequestBehavior.AllowGet);
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

            message = ResAssetMaster.MsgItemQtyUpdated;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadWOItems(Guid? WorkOrderGUID)
        {
            ViewBag.WorkOrderGUID = (WorkOrderGUID ?? Guid.Empty).ToString();
            if (Request["mntsGUID"] != null)
            {
                ViewBag.mntsGUID = Convert.ToString(Request["mntsGUID"]);
            }

            WorkOrderDAL objDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO = new WorkOrderDTO();
            objDTO = objDAL.GetWorkOrdersByGUIDFullJoins(WorkOrderGUID ?? Guid.Empty);
            if (objDTO == null)
            {
                objDTO = objDAL.GetWorkOrdersByGUIDFullJoinsArchieve(WorkOrderGUID ?? Guid.Empty);
                if(objDTO!=null)
                objDTO.IsArchived = true;
            }
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
            //PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);

            //GridState gridState = null;
            //using (MasterBAL masterBAL = new MasterBAL())
            //{
            //    gridState = masterBAL.GetGridStateObj("WorkOrderDetails");
            //}

            //var ReqDetaiData = obj.GetAllWorkOrderRecords(Convert.ToInt64(SessionHelper.RoomID), Convert.ToInt64(SessionHelper.CompanyID), (WorkOrderGUID ?? Guid.Empty).ToString()).ToList();
            //obj.Dispose();

            return PartialView("_CreateWOItemsNew");
        }
        
        public ActionResult CreateWOItemsAjax(JQueryDataTableParamModel param)
        {
            //WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            Guid WorkOrderGUID = new Guid(Request.Params["WOIGUID"]); //new Guid("5c4e6859-7a62-499e-b0e1-ed5035eead41");
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

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
            {
                sortColumnName = "ID desc";
            }

            string searchQuery = param.sSearch;
            //string WOTypes = "WorkOrder','Reqn','Maint";
            long TotalRecordCount = 0;

            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            var ReqDetaiData = obj.GetAllWorkOrderRecordsWithPaging(Convert.ToInt64(SessionHelper.RoomID), Convert.ToInt64(SessionHelper.CompanyID)
                , (WorkOrderGUID).ToString(), param.iDisplayStart, PageSize, sortColumnName, searchQuery,IsArchived, out TotalRecordCount).ToList();

            obj.Dispose();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = ReqDetaiData
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadWOItemsReq(Guid? WorkOrderGUID)
        {
            ViewBag.WorkOrderGUID = (WorkOrderGUID ?? Guid.Empty).ToString();

            WorkOrderDAL objDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO = objDAL.GetWorkOrdersByGUIDFullJoins(WorkOrderGUID ?? Guid.Empty);

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
            //var ReqDetaiData = obj.GetWOPulls(Guid.Parse(WorkOrderGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
            var ReqDetaiData = obj.GetAllWorkOrderRecords(Convert.ToInt64(SessionHelper.RoomID), Convert.ToInt64(SessionHelper.CompanyID), (WorkOrderGUID ?? Guid.Empty).ToString()).ToList();


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
            var DataFromDB = obj.GetPullsByWorkOrderDetailGUIDNormal(Guid.Parse(WODetailGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
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

        public string WOToolCheckouts(Guid WOGUID, Guid ToolGUID, Guid ToolCheckoutGUID)
        {
            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            //var objModel = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.WorkOrderGuid == WOGUID).ToList();
            var objModel = objDAL.GetTCIOHsByWOGUIDFull(WOGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.ToolGUID = ToolGUID;
            ViewBag.RequisitionDetailGUID = Guid.Empty;
            ViewBag.RequisitionDetailHistoryID = 0;
            ViewBag.WorkorderGUID = WOGUID;
            ViewBag.ToolCheckoutGUID = ToolCheckoutGUID;
            return RenderRazorViewToString("_WOCheckoutTools", objModel);
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

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID";
            }
            else
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            PullMasterDAL objPullDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            //var DataFromDB = objPullDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, sortColumnName).Where(x => x.WorkOrderDetailGUID.ToString() == Request["WODetailGUID"].ToString());
            var DataFromDB = objPullDAL.GetPullsByWorkOrderDetailGUIDNormal(Guid.Parse(Request["WODetailGUID"].ToString()), SessionHelper.RoomID, SessionHelper.CompanyID);

            ToolCheckInOutHistoryDAL objTCDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO objToolDTO = null;
            //var objModel = objTCDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.WorkOrderGuid == Guid.Parse(Request["WODetailGUID"].ToString()));
            var objModel = objTCDAL.GetTCIOHsByWOGUIDFull(Guid.Parse(Request["WODetailGUID"].ToString()), SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objModel != null && objModel.Count() > 0)
            {

                List<ToolMasterDTO> objToolList = new List<ToolMasterDTO>();
                string ToolGUIDs = string.Join(",", objModel.Where(x => (x.ToolGUID ?? Guid.Empty) != Guid.Empty).Select(t => t.ToolGUID.GetValueOrDefault(Guid.Empty)).Distinct().ToArray());
                objToolList = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDsNormal(ToolGUIDs);


                foreach (var item in objModel)
                {
                    objToolDTO = objToolList.Where(x => x.GUID == item.ToolGUID.GetValueOrDefault(Guid.Empty)).FirstOrDefault();

                    DataFromDB.Add(new PullMasterViewDTO()
                    {
                        ToolName = objToolDTO.ToolName,
                        ActionType = "Checkout",
                        PoolQuantity = item.CheckedOutQTY.GetValueOrDefault(0),
                        ID = item.ID,
                        ItemNumber = "",
                        Created = item.Created,
                        CreatedDate = item.CreatedDate,
                        Updated = item.Updated,
                        UpdatedDate = item.UpdatedDate,
                        AddedFrom = item.AddedFrom,
                        EditedFrom = item.EditedFrom,
                        ReceivedOn = item.ReceivedOn,
                        ReceivedOnWeb = item.ReceivedOnWeb,
                        ReceivedOnDate = item.ReceivedOnDate,
                        ReceivedOnWebDate = item.ReceivedOnDateWeb,
                    });
                }
            }

            DataFromDB.Sort(sortColumnName);
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

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID";
            }
            else
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
            //var DataFromDB = objPullDAL.GetWOPulls(Guid.Parse(Request["WODetailGUID"].ToString()), SessionHelper.RoomID, SessionHelper.CompanyID);
            var DataFromDB = objPullDAL.GetPullDetailHistoryByPullGUIDNormal(Request["WODetailGUID"].ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);


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
            var DataFromDB = obj.GetPullsByWorkOrderDetailGUIDNormal(Guid.Parse(WorkOrderGUID), SessionHelper.RoomID, SessionHelper.CompanyID);

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
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(PageName, SessionHelper.RoomID, SessionHelper.CompanyID);

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
                             //UpdatedByName = c.UpdatedByName,
                             //CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                         };
            return result;
        }

        public ActionResult GetWorkOrderFiles(Guid WorkOrderGuid)
        {
                WorkOrderImageDetailDAL objWorkOrderImageDetailDAL = new WorkOrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                List<WorkOrderImageDetailDTO> listWorkOrderImageDetail = objWorkOrderImageDetailDAL.GetWorkOrderImageDetailByWOGUID(WorkOrderGuid, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                Dictionary<string, Guid> retData = new Dictionary<string, Guid>();

                if (listWorkOrderImageDetail != null && listWorkOrderImageDetail.Any())
                {
                    foreach (WorkOrderImageDetailDTO woimg in listWorkOrderImageDetail.Where(e => !string.IsNullOrEmpty(e.WOImageName) && !string.IsNullOrWhiteSpace(e.WOImageName)))
                    {
                        if (!retData.ContainsKey(woimg.WOImageName))
                        {
                            retData.Add(woimg.WOImageName, woimg.GUID);
                        }
                    }
                }
                
                return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
            
        }

        public void DeleteExistingFiles(string FileId, Guid WorkOrderGuid)
        {
            try
            {
                WorkOrderImageDetailDAL objWorkOrderImageDetailDAL = new WorkOrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                objWorkOrderImageDetailDAL.DeleteRecords(FileId, SessionHelper.UserID, WorkOrderGuid);
            }
            catch
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
            WorkOrderDTO objDTO = obj.GetWorkOrderHistoryByHistoryIdFull(ID);

            ViewBag.UDFs = GetUDFDataPageWise("WorkOrder");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.TechnicianBAG = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetJobTypeByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            return PartialView("_CreateWorkOrder_History", objDTO);
        }


        public ActionResult WorkOrderHistoryViewFromMaintenance(Guid? GUID)
        {

            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO1 = obj.GetWorkOrderByGUIDPlain(GUID ?? Guid.Empty);
            WorkOrderDTO objDTO = obj.GetHistoryRecordForMaintenance(objDTO1.ID);

            ViewBag.UDFs = GetUDFDataPageWise("WorkOrder");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.TechnicianBAG = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetJobTypeByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            return PartialView("_CreateWorkOrder_History", objDTO);
        }

        // Commented below as dont find any ref , amit t 6-apr-20
        //public ActionResult WorkOrderLineItemsDetails(Guid? GUID)
        //{
        //    WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
        //    WorkOrderDTO objDTO = obj.GetWorkOrdersByGUIDFullJoins(GUID ?? Guid.Empty);

        //    ViewBag.UDFs = GetUDFDataPageWise("WorkOrder");
        //    foreach (var i in ViewBag.UDFs)
        //    {
        //        string _UDFColumnName = (string)i.UDFColumnName;
        //        ViewData[_UDFColumnName] = i.UDFDefaultValue;
        //    }

        //    CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
        //    ViewBag.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer).ToList();

        //    TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
        //    ViewBag.TechnicianBAG = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

        //    GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
        //    ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

        //    AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
        //    ViewBag.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

        //    ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
        //    ViewBag.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

        //    JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
        //    ViewBag.JobTypeBAG = objJobTypeApi.GetJobTypeByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);

        //    ViewBag.WOStatusBag = GetWOStaus();
        //    ViewBag.WOTypeBag = GetWOType();

        //    ViewBag.ViewOnly = true;

        //    objDTO.IsHistory = true;

        //    return PartialView("_CreateWorkOrder", objDTO);
        //}

        public ActionResult WorkOrderLineItemsDetailsForReq(Guid? GUID)
        {
            WorkOrderDAL obj = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDTO objDTO = obj.GetWorkOrdersByGUIDFullJoins(GUID ?? Guid.Empty);

            ViewBag.UDFs = GetUDFDataPageWise("WorkOrder");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer).ToList();

            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> technicianlist = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            technicianlist.ForEach(t =>
            {
                if (!string.IsNullOrEmpty(t.Technician))
                {
                    t.Technician = Convert.ToString(t.TechnicianCode + " --- " + t.Technician);
                }
                else
                {
                    t.Technician = Convert.ToString(t.TechnicianCode);
                }
            });
            ViewBag.TechnicianBAG = technicianlist.OrderBy(t => t.TechnicianCode).ToList();

            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            ToolMasterDAL objToolApi = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetJobTypeByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            ViewBag.ViewOnly = true;

            objDTO.IsHistory = true;
            if (!string.IsNullOrWhiteSpace(objDTO.ProjectSpendName))
                objDTO.ProjectSpendName = objDTO.ProjectSpendName.TrimStart(',');
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
            objDTO = new WorkOrderDAL(SessionHelper.EnterPriseDBName).GetWorkOrderHistoryByHistoryIdFull(historyID);
            if (objDTO != null)
            {
                objDTO.WorkOrderListItem = new PullMasterDAL(SessionHelper.EnterPriseDBName).GetPullHistoryByWorkOrderDetailGUIDFull(Guid.Parse(WOHistoryGUID)).ToList();
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
                List<WorkOrderImageDetailDTO> listWorkOrderImageDetail = objWorkOrderImageDetailDAL.GetWorkOrderImageDetailByWOGUID(WOGuids, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                //string WorkOrderFilePath = System.Configuration.ConfigurationManager.AppSettings["WorkOrderFilePath"].ToString();
                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(Server.MapPath("/SiteSettings.xml"));
                string WorkOrderFilePath = SiteSettingHelper.WorkOrderFilePaths;  //Settinfile.Element("WorkOrderFilePaths").Value;
                WorkOrderFilePath = WorkOrderFilePath.Replace("~", string.Empty);
                //WorkOrderDTO woDTO = objWODAL.GetRecord(WOGuids.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                Guid WOGUID = Guid.Empty;
                WorkOrderDTO woDTO = new WorkOrderDTO();
                string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath, "");
                baseURL = SessionHelper.CurrentDomainURL;
                string WorkOrderPath = baseURL + WorkOrderFilePath + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/" + woDTO.ID;
                if (Guid.TryParse(WOGuids.ToString(), out WOGUID))
                {
                    woDTO = objWODAL.GetWorkOrderByGUIDPlain(WOGUID);
                    WorkOrderPath = baseURL + WorkOrderFilePath + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/" + woDTO.ID;
                }

                foreach (WorkOrderImageDetailDTO woimg in listWorkOrderImageDetail)
                {
                    retData.Add(WorkOrderPath + "/" + woimg.WOImageName);
                }
            }

            //return File(data, "application/csv", dtoModuleDetail.ResourceModuleName + "_" + dtoModuleDetail.PageName + "_MobileRes_" + DateTimeUtility.DateTimeNow.ToString("yyyyMMddHHmmss") + ".csv");
            return Json(new { Message = "Sucess", Status = true, ReturnFiles = retData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ClearSession()
        {
            Session["RoomAllWO"] = null;
            return Json(true);
        }

        [HttpGet]
        public JsonResult BlankSession()
        {
            Session["IsInsert"] = "";
            Session["WOEvent"] = "";
            return Json(new { Status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CloseWorkOrders(string ids, string Guids)
        {
            string message = string.Empty, status = string.Empty;
            WorkOrderDAL workOrderDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);

            try
            {
                if (!string.IsNullOrEmpty(ids) && !string.IsNullOrWhiteSpace(ids))
                {
                    workOrderDAL.CloseWorkorderByIds(ids, SessionHelper.UserID);

                    try
                    {
                        if (!string.IsNullOrEmpty(Guids) && !string.IsNullOrWhiteSpace(Guids))
                        {
                            string workOrderGUIDs = "<DataGuids>" + Guids + "</DataGuids>";
                            string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                            string eventName = "OWOCL";
                            List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                            if (lstNotification != null && lstNotification.Count > 0)
                            {
                                objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, workOrderGUIDs);
                            }
                        }
                    }
                    catch { }                    
                }                    

                status = "ok";
                message = ResMessage.WorkordersClosedSuccessfully;
            }
            catch (Exception ex)
            {
                status = "fail";
                message = "";
            }            

            return Json(new { Message = message, Status = status });
        }

        public void DeleteWorkorderSignature(long Id, string FileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrWhiteSpace(FileName))
                {
                    long CompanyID = SessionHelper.CompanyID;
                    string UNCPathRoot = "~/Uploads/WorkOrderSignature/" + Convert.ToString(CompanyID);
                    string folderPath = System.Web.HttpContext.Current.Server.MapPath(UNCPathRoot);

                    if (Directory.Exists(folderPath))
                    {
                        string filePath = folderPath + "/" + FileName;
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }

                    WorkOrderDAL workorderDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                    workorderDAL.RemoveWOSignatureById(Id,SessionHelper.UserID);
                }               
            }
            catch
            {

            }
        }

        [HttpPost]
        public JsonResult DeleteWorkorderTool(string ToolCheckInOutHistoryIds,Guid? WOGUID)
        { 
            try
            {
                if (!string.IsNullOrEmpty(ToolCheckInOutHistoryIds) && !string.IsNullOrWhiteSpace(ToolCheckInOutHistoryIds))
                {
                    bool isAllowToolOrdering = SessionHelper.AllowToolOrdering;
                    ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
                    ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
                    ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    var workOrderLineItemsDAL = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
                    string[] CheckInCheckOutGuids = ToolCheckInOutHistoryIds.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var checkInCheckOutGuid in CheckInCheckOutGuids)
                    {
                        if (checkInCheckOutGuid != "" && Guid.Parse(checkInCheckOutGuid) != Guid.Empty)
                        {
                            ToolCheckInOutHistoryDTO checkoutData = objCICODAL.GetTCIOHByGUIDPlain(Guid.Parse(checkInCheckOutGuid), SessionHelper.RoomID, SessionHelper.CompanyID);

                            if (checkoutData != null && checkoutData.ID > 0)
                            {
                                var objToolDTO = isAllowToolOrdering ? objToolDAL.GetToolByGUIDFull(checkoutData.ToolGUID.GetValueOrDefault(Guid.Empty))
                                                                                    : objToolDAL.GetToolByGUIDPlain(checkoutData.ToolGUID.GetValueOrDefault(Guid.Empty));

                                if (objToolDTO != null && objToolDTO.ID > 0 && !objToolDTO.IsDeleted.GetValueOrDefault(false))
                                {
                                    ToolCheckInHistoryDTO objCIDTO = new ToolCheckInHistoryDTO();
                                    objCIDTO.CompanyID = SessionHelper.CompanyID;
                                    objCIDTO.Created = DateTimeUtility.DateTimeNow;
                                    objCIDTO.CreatedBy = SessionHelper.UserID;
                                    objCIDTO.CreatedByName = SessionHelper.UserName;
                                    objCIDTO.IsArchived = false;
                                    objCIDTO.IsDeleted = false;
                                    objCIDTO.LastUpdatedBy = SessionHelper.UserID;
                                    objCIDTO.Room = SessionHelper.RoomID;
                                    objCIDTO.RoomName = SessionHelper.RoomName;
                                    objCIDTO.SerialNumber = objToolDTO.Serial;
                                    objCIDTO.CheckInCheckOutGUID = checkoutData.GUID;
                                    objCIDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objCIDTO.UpdatedByName = SessionHelper.UserName;

                                    objToolDTO.CheckedOutQTY = objToolDTO.CheckedOutQTY.GetValueOrDefault(0) - checkoutData.CheckedOutQTY.GetValueOrDefault(0);
                                    objToolDTO.CheckedOutMQTY = objToolDTO.CheckedOutMQTY.GetValueOrDefault(0) - checkoutData.CheckedOutMQTY.GetValueOrDefault(0);
                                    objCIDTO.CheckedOutQTY = checkoutData.CheckedOutQTY.GetValueOrDefault(0);
                                    objCIDTO.CheckedOutMQTY = checkoutData.CheckedOutMQTY.GetValueOrDefault(0);

                                    objCIDTO.CheckInDate = DateTimeUtility.DateTimeNow;
                                    objCIDTO.CheckOutStatus = "Check In";
                                    objCIDTO.IsOnlyFromItemUI = true;
                                    objCIDTO.AddedFrom = "Web";
                                    objCIDTO.EditedFrom = "Web";
                                    objCIDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objCIDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objCIDTO.TechnicianGuid = checkoutData.TechnicianGuid.GetValueOrDefault(Guid.Empty);
                                        
                                    if (isAllowToolOrdering)
                                    {
                                        objCIDTO.SerialNumber = checkoutData.SerialNumber;
                                    }
                                        
                                    long objCOID = objCIDAL.Insert(objCIDTO);
                                    ToolCheckInHistoryDTO objToolCheckInHistoryDTO = objCIDAL.GetToolCheckInByIDPlain(objCOID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    objToolDTO.IsOnlyFromItemUI = true;
                                    objToolDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objToolDTO.EditedFrom = "Web";

                                    objToolDAL.Edit(objToolDTO);

                                    if (isAllowToolOrdering)
                                    {
                                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                        objToolAssetQuantityDetailDTO.ToolGUID = checkoutData.ToolGUID;
                                        objToolAssetQuantityDetailDTO.AssetGUID = null;
                                        objToolAssetQuantityDetailDTO.ToolBinID = objToolDTO.ToolLocationDetailsID;
                                        objToolAssetQuantityDetailDTO.Quantity = checkoutData.CheckedOutQTY.GetValueOrDefault(0) + checkoutData.CheckedOutMQTY.GetValueOrDefault(0);
                                        objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                                        objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                                        objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.WhatWhereAction = "Workordercontroller>>DeleteWorkorderTool";
                                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolDTO.Quantity;
                                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web.";
                                        objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                                        objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                                        objToolAssetQuantityDetailDTO.IsArchived = false;
                                        objToolAssetQuantityDetailDTO.SerialNumber = checkoutData.SerialNumber;
                                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                                        objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, CheckoutGUID: objToolCheckInHistoryDTO.CheckInCheckOutGUID, CheckinGUID: objToolCheckInHistoryDTO.GUID, ReferalAction: "Check In", SerialNumber: checkoutData.SerialNumber);
                                    }
                                        
                                    if (objToolDTO != null && objToolDTO.IsGroupOfItems.GetValueOrDefault(0) == 0 && objCIDTO != null && objCIDTO.ID > 0)
                                        MaintCreateForNoOfCheckoutAtCheckIn(objToolDTO, objCIDTO);

                                    checkoutData.CheckedOutMQTYCurrent = checkoutData.CheckedOutMQTY.GetValueOrDefault(0);
                                    checkoutData.CheckedOutQTYCurrent = checkoutData.CheckedOutQTY.GetValueOrDefault(0);
                                    checkoutData.IsOnlyFromItemUI = true;
                                    checkoutData.EditedFrom = "Web";
                                    checkoutData.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    checkoutData.WorkOrderGuid = null;
                                    objCICODAL.Edit(checkoutData);
                                }
                            }
                        }
                    }
                    workOrderLineItemsDAL.UpdateWOItemAndTotalCost(WOGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                }
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            
        }

        private void MaintCreateForNoOfCheckoutAtCheckIn(ToolMasterDTO objToolDTO, ToolCheckInHistoryDTO objCIDTO)
        {
            AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
            ToolsSchedulerDTO objToolsSchedulerDTO1 = null;
            List<ToolsSchedulerMappingDTO> lstToolsSchedulerMappingDTO = objAssetMasterDAL.GetSchedulerMappingRecordforTool_SchedularGUID(objToolDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID, false, false);
            ToolsMaintenanceDAL objToolsMainDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);

            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

            foreach (var ToolsSchedulerMappingDTO in lstToolsSchedulerMappingDTO)
            {
                objToolsSchedulerDTO1 = ToolsSchedulerDAL.GetToolsSchedulerByGuidPlain(ToolsSchedulerMappingDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty));
                if (objToolsSchedulerDTO1 != null && objToolsSchedulerDTO1.SchedulerType == (int)MaintenanceScheduleType.CheckOuts)
                {
                    List<ToolCheckInOutHistoryDTO> lstToolCheckInOutHistoryDTO = null;
                    //lstToolCheckInOutHistoryDTO = objCICODAL.GetRecordByTool(objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                    lstToolCheckInOutHistoryDTO = objCICODAL.GetTCIOHsByToolGUIDWithToolInfo(objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (lstToolCheckInOutHistoryDTO.Count() >= objToolsSchedulerDTO1.CheckOuts) // Entery in Maintenance
                    {
                        ToolsMaintenanceDTO objToolsMainDTO = objToolsMainDAL.GetToolsMaintenanceSchedulerMappingPlain(null, objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, objToolsSchedulerDTO1.GUID, ToolsSchedulerMappingDTO.GUID);
                        if (objToolsMainDTO == null) // First Entery in ToolsMaintenance
                        {
                            lstToolCheckInOutHistoryDTO = lstToolCheckInOutHistoryDTO.Where(x => x.GUID != objCIDTO.CheckInCheckOutGUID).ToList();
                            double CalcScheduleDay = CalculateToolCheckoutDays(lstToolCheckInOutHistoryDTO, true);
                            ToolsMaintenanceDTO objTMDTO = new ToolsMaintenanceDTO();
                            objTMDTO.ToolGUID = objToolDTO.GUID;
                            objTMDTO.ScheduleDate = datetimetoConsider.Date.AddDays(CalcScheduleDay);
                            objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                            objTMDTO.MaintenanceName = objToolsSchedulerDTO1.SchedulerName;
                            objTMDTO.AssetGUID = null;
                            objTMDTO.CompanyID = SessionHelper.CompanyID;
                            objTMDTO.Created = objCIDTO.Created.Value.AddSeconds(-1); //DateTimeUtility.DateTimeNow;
                            objTMDTO.CreatedBy = SessionHelper.UserID;
                            objTMDTO.GUID = Guid.NewGuid();
                            objTMDTO.IsArchived = false;
                            objTMDTO.IsDeleted = false;
                            objTMDTO.LastMaintenanceDate = null;
                            objTMDTO.LastMeasurementValue = null;
                            objTMDTO.MaintenanceDate = datetimetoConsider.Date.AddDays(CalcScheduleDay);
                            objTMDTO.LastUpdatedBy = SessionHelper.UserID;
                            objTMDTO.MaintenanceType = MaintenanceType.UnScheduled.ToString();
                            objTMDTO.MappingGUID = ToolsSchedulerMappingDTO.GUID;
                            objTMDTO.RequisitionGUID = null;
                            objTMDTO.Room = SessionHelper.RoomID;
                            objTMDTO.ScheduleFor = objToolsSchedulerDTO1.ScheduleFor;
                            objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                            objTMDTO.SchedulerType = (byte)objToolsSchedulerDTO1.SchedulerType;
                            objTMDTO.Status = MaintenanceStatus.Open.ToString();
                            objTMDTO.TrackngMeasurement = objToolsSchedulerDTO1.SchedulerType;
                            objTMDTO.UDF1 = null;
                            objTMDTO.UDF2 = null;
                            objTMDTO.UDF3 = null;
                            objTMDTO.UDF4 = null;
                            objTMDTO.UDF5 = null;
                            objTMDTO.Updated = DateTimeUtility.DateTimeNow;
                            objTMDTO.WorkorderGUID = null;
                            objToolsMainDAL.Insert(objTMDTO);
                        }
                        else
                        {
                            lstToolCheckInOutHistoryDTO = lstToolCheckInOutHistoryDTO.Where(x => x.GUID != objCIDTO.CheckInCheckOutGUID && x.Created > objToolsMainDTO.Created).ToList();
                            if (lstToolCheckInOutHistoryDTO.Count() >= objToolsSchedulerDTO1.CheckOuts)
                            {
                                double CalcScheduleDay = CalculateToolCheckoutDays(lstToolCheckInOutHistoryDTO, true);
                                ToolsMaintenanceDTO objTMDTO = new ToolsMaintenanceDTO();
                                objTMDTO.ToolGUID = objToolDTO.GUID;
                                objTMDTO.ScheduleDate = objToolsMainDTO.ScheduleDate.Value.AddDays(CalcScheduleDay);
                                objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                                objTMDTO.MaintenanceName = objToolsSchedulerDTO1.SchedulerName;
                                objTMDTO.AssetGUID = null;
                                objTMDTO.CompanyID = SessionHelper.CompanyID;
                                objTMDTO.Created = objCIDTO.Created.Value.AddSeconds(-1); //DateTimeUtility.DateTimeNow;
                                objTMDTO.CreatedBy = SessionHelper.UserID;
                                objTMDTO.GUID = Guid.NewGuid();
                                objTMDTO.IsArchived = false;
                                objTMDTO.IsDeleted = false;
                                objTMDTO.LastMaintenanceDate = null;
                                objTMDTO.LastMeasurementValue = null;
                                objTMDTO.MaintenanceDate = objToolsMainDTO.MaintenanceDate.Value.AddDays(CalcScheduleDay);
                                objTMDTO.LastUpdatedBy = SessionHelper.UserID;
                                objTMDTO.MaintenanceType = MaintenanceType.UnScheduled.ToString();
                                objTMDTO.MappingGUID = ToolsSchedulerMappingDTO.GUID;
                                objTMDTO.RequisitionGUID = null;
                                objTMDTO.Room = SessionHelper.RoomID;
                                objTMDTO.ScheduleFor = objToolsSchedulerDTO1.ScheduleFor;
                                objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                                objTMDTO.SchedulerType = (byte)objToolsSchedulerDTO1.SchedulerType;
                                objTMDTO.Status = MaintenanceStatus.Open.ToString();
                                objTMDTO.TrackngMeasurement = objToolsSchedulerDTO1.SchedulerType;
                                objTMDTO.UDF1 = null;
                                objTMDTO.UDF2 = null;
                                objTMDTO.UDF3 = null;
                                objTMDTO.UDF4 = null;
                                objTMDTO.UDF5 = null;
                                objTMDTO.Updated = DateTimeUtility.DateTimeNow;
                                objTMDTO.WorkorderGUID = null;
                                objToolsMainDAL.Insert(objTMDTO);
                            }

                        }
                    }
                }
            }
        }

        private double CalculateToolCheckoutDays(List<ToolCheckInOutHistoryDTO> lstToolCheckInOutHistoryDTO, bool IsFirst)
        {
            double DayDiffer = 0;
            if (IsFirst)
            {
                ToolCheckInOutHistoryDTO objDTO = lstToolCheckInOutHistoryDTO.OrderBy(x => x.Created).FirstOrDefault();
                DateTime DtFirst = (DateTime)objDTO.CheckOutDate;

                objDTO = lstToolCheckInOutHistoryDTO.OrderBy(x => x.Created).LastOrDefault();
                DateTime DtLast = (DateTime)objDTO.CheckOutDate;

                DayDiffer = (DtLast - DtFirst).TotalDays;
            }

            return Math.Ceiling(DayDiffer);
        }

       

    }
}

