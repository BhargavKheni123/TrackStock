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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace eTurnsWeb.Controllers
{
    public partial class InventoryController : eTurnsControllerBase
    {
        #region [Inventory count List]


        public ActionResult InventoryCountList()
        {
            return View();
        }

        public ActionResult InventoryCountListAjax(JQueryDataTableParamModel param)
        {
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

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<InventoryCountDTO> DataFromDB = objInventoryCountDAL.GetPagedRecordsICListFromDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            var result = from u in DataFromDB
                         select new
                         {
                             CompanyId = u.CompanyId,
                             RoomId = u.RoomId,
                             Created = u.Created,
                             CreatedBy = u.CreatedBy,
                             CreatedByName = u.CreatedByName,
                             GUID = u.GUID,
                             ID = u.ID,
                             CountName = u.CountName,

                             CountItemDescription = u.CountItemDescription,
                             CountType = u.CountType,
                             Year = u.Year,
                             CountDate = u.CountDate,
                             CountDateDisplay = FnCommon.ConvertDateByTimeZone(u.CountDate, false, true),
                             //CountDateDisplay = u.CountDateDisplay,
                             CountCompletionDate = u.CountCompletionDate,
                             IsAutomatedCompletion = u.IsAutomatedCompletion,
                             CompleteCauseCountGUID = u.CompleteCauseCountGUID,
                             CountStatus = u.CountStatus,
                             IsApplied = u.IsApplied,
                             IsClosed = u.IsClosed,
                             IsArchived = u.IsArchived,
                             IsDeleted = u.IsDeleted,
                             LastUpdatedBy = u.LastUpdatedBy,
                             UDF1 = u.UDF1,
                             UDF2 = u.UDF2,
                             UDF3 = u.UDF3,
                             UDF4 = u.UDF4,
                             UDF5 = u.UDF5,
                             Updated = u.Updated,
                             UpdatedByName = u.UpdatedByName,
                             TotalItemsWithinCount = u.TotalItemsWithinCount,
                             CreatedDate = FnCommon.ConvertDateByTimeZone(u.Created, true, false),//CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = FnCommon.ConvertDateByTimeZone(u.Updated, true, false),//CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             ReceivedOnDate = FnCommon.ConvertDateByTimeZone(u.ReceivedOn, true, false),//CommonUtility.ConvertDateByTimeZone(u.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOnDateWeb = FnCommon.ConvertDateByTimeZone(u.ReceivedOnWeb, true, false),// CommonUtility.ConvertDateByTimeZone(u.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             // strCountDate = FnCommon.ConvertDateByTimeZone(u.CountDate, false, true),
                             ReleaseNumber = u.ReleaseNumber
                         };
            ViewBag.TotalRecordCount = TotalRecordCount;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount
            ,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CloseCountItems(string ids, string guids)
        {
            string message = string.Empty, status = string.Empty;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            try
            {
                objInventoryCountDAL.CloseCountItems(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            catch (Exception)
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            return Json(new { Message = message, Status = status });
        }
        [HttpPost]
        public JsonResult UnclosedInventoryCount(string ids)
        {
            string message = string.Empty, status = string.Empty;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            try
            {
                objInventoryCountDAL.UnclosedInventoryCount(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            catch (Exception)
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            return Json(new { Message = message, Status = status });
        }

        #endregion

        #region [Inventory count Management]


        public ActionResult InventoryCountEdit(Guid? IcGuid)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = null;
            if (IcGuid.HasValue)
            {
                bool IsArchived = false;
                if (Request["IsArchived"] != null && !string.IsNullOrEmpty(Request["IsArchived"].ToString()))
                    IsArchived = bool.Parse(Request["IsArchived"].ToString());

                objInventoryCountDTO = IsArchived ? objInventoryCountDAL.GetArchivedCountByGUIDFull(IcGuid.GetValueOrDefault(Guid.Empty)) 
                                                  : objInventoryCountDAL.GetInventoryCountByGUId(IcGuid ?? Guid.NewGuid(), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objInventoryCountDTO != null)
                {
                    objInventoryCountDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objInventoryCountDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    objInventoryCountDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objInventoryCountDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    objInventoryCountDTO.ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(objInventoryCountDTO.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                }
                //objInventoryCountDTO.CountDateDisplay = objInventoryCountDTO.CountDate.ToString();
                objInventoryCountDTO.CountDateDisplay = objInventoryCountDTO.CountDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                if (objInventoryCountDTO == null)
                {
                    objInventoryCountDTO = new InventoryCountDTO();
                }
            }
            else
            {
                objInventoryCountDTO = new InventoryCountDTO();
            }
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("InventoryCount");
            ViewBag.CountTypeBag = GetCountTypeOptions(objInventoryCountDTO.CountType);
            //ViewBag.ProjectBag = GetProjects(objInventoryCountDTO.CountType);
            SetProjectViewBag(objInventoryCountDTO.ProjectSpendGUID ?? Guid.Empty);
            ViewBag.PullOrderNumberBag = GetPullOrderNumberlist();

            ViewData["UDF1"] = objInventoryCountDTO.UDF1;
            ViewData["UDF2"] = objInventoryCountDTO.UDF2;
            ViewData["UDF3"] = objInventoryCountDTO.UDF3;
            ViewData["UDF4"] = objInventoryCountDTO.UDF4;
            ViewData["UDF5"] = objInventoryCountDTO.UDF5;
            objInventoryCountDTO.IsOnlyFromItemUI = true;
            objInventoryCountDTO.IsApplyAllDisable = objInventoryCountDAL.CheckLineItems(SessionHelper.RoomID, SessionHelper.CompanyID, IcGuid ?? Guid.Empty);
            return PartialView("_InventoryCountDetails", objInventoryCountDTO);
        }

        public ActionResult InventoryCountCreate()
        {
            InventoryCountDAL inventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            //long AutoCountNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID("nextcountnumber", SessionHelper.RoomID, SessionHelper.CompanyID);
            string AutoCountNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("nextcountnumber", SessionHelper.RoomID, SessionHelper.CompanyID);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

            // for Get room level setting for CountType

            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            RoomDTO objRoomInfo = new RoomDTO();
            //objRoomInfo = objRoomDal.GetRoomByIDPlain(RoomID);
            string columnList = "ID,RoomName,DefaultCountType";
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objRoomInfo = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");


            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            objInventoryCountDTO.CompanyId = SessionHelper.CompanyID;
            objInventoryCountDTO.CompleteCauseCountGUID = null;
            objInventoryCountDTO.CountCompletionDate = null;

            objInventoryCountDTO.CountDate = datetimetoConsider;
            if (objInventoryCountDTO.CountDate != null)
                objInventoryCountDTO.CountDateDisplay = objInventoryCountDTO.CountDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);

            //objInventoryCountDTO.CountDate = DateTimeUtility.DateTimeNow;
            objInventoryCountDTO.CountItemDescription = string.Empty;
            //objInventoryCountDTO.CountName = "#Count" + AutoCountNumber;
            objInventoryCountDTO.CountName = AutoCountNumber.ToString();
            objInventoryCountDTO.CountStatus = Convert.ToString(InventoryCountStatus.Open);
            objInventoryCountDTO.CountType = Convert.ToChar(InventoryCountType.Adjustment).ToString();
            if (objRoomInfo != null)
                objInventoryCountDTO.CountType = objRoomInfo.DefaultCountType;

            objInventoryCountDTO.Created = DateTimeUtility.DateTimeNow;
            objInventoryCountDTO.CreatedBy = SessionHelper.UserID;
            objInventoryCountDTO.CreatedByName = SessionHelper.UserName;
            objInventoryCountDTO.GUID = Guid.NewGuid();
            objInventoryCountDTO.ID = 0;
            objInventoryCountDTO.IsApplied = false;
            objInventoryCountDTO.IsArchived = false;
            objInventoryCountDTO.IsAutomatedCompletion = false;
            objInventoryCountDTO.IsClosed = false;
            objInventoryCountDTO.IsDeleted = false;
            objInventoryCountDTO.LastUpdatedBy = SessionHelper.UserID;
            objInventoryCountDTO.RoomId = SessionHelper.RoomID;
            objInventoryCountDTO.RoomName = SessionHelper.RoomName;
            objInventoryCountDTO.TotalItemsWithinCount = 0;
            objInventoryCountDTO.Updated = DateTimeUtility.DateTimeNow;
            objInventoryCountDTO.UpdatedByName = SessionHelper.UserName;
            objInventoryCountDTO.Year = Convert.ToInt16(DateTimeUtility.DateTimeNow.Year);
            objInventoryCountDTO.IsOnlyFromItemUI = true;

            var releaseNumber = inventoryCountDAL.GetInventoryCountReleaseNumber(0, objInventoryCountDTO.CountName, SessionHelper.CompanyID, SessionHelper.RoomID);
            objInventoryCountDTO.ReleaseNumber = Convert.ToString(releaseNumber);

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("InventoryCount");
            ViewBag.CountTypeBag = GetCountTypeOptions(objInventoryCountDTO.CountType);
            // ViewBag.ProjectBag = GetProjects(objInventoryCountDTO.CountType);
            SetProjectViewBag(Guid.Empty);
            ViewBag.PullOrderNumberBag = GetPullOrderNumberlist();

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_InventoryCountDetails", objInventoryCountDTO);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveInventoryCount(InventoryCountDTO objInventoryCountDTO)
        {
            string message = "";
            string status = "";
            try
            {
                InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
                //wi-1379 remove duplicate validation and allow count can be duplicate
                //if (objInventoryCountDAL.IsNameCountNameExist(objInventoryCountDTO.ID, objInventoryCountDTO.CountName, SessionHelper.RoomID, SessionHelper.CompanyID))
                //{
                //    message = string.Format(ResMessage.DuplicateMessage, ResInventoryCount.CountName, objInventoryCountDTO.CountName);
                //    status = "duplicate";
                //}
                //else
                //{


                if (objInventoryCountDTO.ID > 0)
                {
                    InventoryCountDTO objInventoryCountExistingDTO = objInventoryCountDAL.GetInventoryCountByGUId(objInventoryCountDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objInventoryCountDTO != null)
                    {
                        if (objInventoryCountDTO.CountType != objInventoryCountExistingDTO.CountType)
                        {
                            var TotalAppliedCount = 0;
                            TotalAppliedCount = objInventoryCountDAL.GetAppliedCountDetailscnt(SessionHelper.RoomID, SessionHelper.CompanyID, objInventoryCountDTO.GUID);
                            if (TotalAppliedCount > 0)
                            {
                                message = string.Format(ResInventoryCount.CannotChangeCountType);
                                status = "CannotChangeCountType";
                            }
                        }
                    }
                }
                if (String.IsNullOrWhiteSpace(message) && String.IsNullOrWhiteSpace(status))
                {
                    //var releaseNumber = objInventoryCountDAL.GetInventoryCountReleaseNumber(objInventoryCountDTO.ID, objInventoryCountDTO.CountName, SessionHelper.CompanyID, SessionHelper.RoomID);
                    //objInventoryCountDTO.ReleaseNumber = Convert.ToString(releaseNumber);
                    bool IsCountCreate = false;
                    bool IsCountUpdate = false;
                    if (objInventoryCountDTO.ID < 1)
                    {
                        IsCountCreate = true;
                        objInventoryCountDTO.CountDate = DateTime.ParseExact(objInventoryCountDTO.CountDateDisplay, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                        objInventoryCountDTO.CompanyId = SessionHelper.CompanyID;
                        objInventoryCountDTO.CompleteCauseCountGUID = null;
                        objInventoryCountDTO.CountCompletionDate = null;
                        objInventoryCountDTO.CountStatus = Convert.ToChar(InventoryCountStatus.Open).ToString();
                        //objInventoryCountDTO.CountType = Convert.ToChar(InventoryCountType.Manual).ToString();
                        objInventoryCountDTO.Created = DateTimeUtility.DateTimeNow;
                        objInventoryCountDTO.CreatedBy = SessionHelper.UserID;
                        objInventoryCountDTO.CreatedByName = SessionHelper.UserName;
                        objInventoryCountDTO.GUID = Guid.NewGuid();
                        objInventoryCountDTO.ID = 0;
                        objInventoryCountDTO.IsApplied = false;
                        objInventoryCountDTO.IsArchived = false;
                        objInventoryCountDTO.IsAutomatedCompletion = false;
                        objInventoryCountDTO.IsClosed = false;
                        objInventoryCountDTO.IsDeleted = false;
                        objInventoryCountDTO.LastUpdatedBy = SessionHelper.UserID;
                        objInventoryCountDTO.RoomId = SessionHelper.RoomID;
                        objInventoryCountDTO.RoomName = SessionHelper.RoomName;
                        objInventoryCountDTO.TotalItemsWithinCount = 0;
                        objInventoryCountDTO.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDTO.UpdatedByName = SessionHelper.UserName;
                        objInventoryCountDTO.Year = Convert.ToInt16(DateTimeUtility.DateTimeNow.Year);
                        objInventoryCountDTO.AddedFrom = "Web";
                        objInventoryCountDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objInventoryCountDTO.EditedFrom = "Web";
                        objInventoryCountDTO.ReceivedOn = DateTimeUtility.DateTimeNow;

                    }
                    else
                    {
                        IsCountUpdate = true;
                        objInventoryCountDTO.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDTO.LastUpdatedBy = SessionHelper.UserID;
                        if (objInventoryCountDTO.IsOnlyFromItemUI)
                        {
                            objInventoryCountDTO.EditedFrom = "Web";
                            objInventoryCountDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    if ((objInventoryCountDTO.CountType ?? string.Empty).ToLower() == "a")
                    {
                        objInventoryCountDTO.ProjectSpendGUID = null;
                        objInventoryCountDTO.PullOrderNumber = null;

                    }
                    else
                    {
                        ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                        ProjectMasterDTO DefaultProjectSpend = objProjectMasterDAL.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                        if (DefaultProjectSpend != null && DefaultProjectSpend.GUID != Guid.Empty)
                        {
                            objInventoryCountDTO.ProjectSpendGUID = DefaultProjectSpend.GUID;
                        }
                        PullController pullController = new PullController();
                        PullPOMasterDTO PullPOMaster = new PullPOMasterDTO();
                        PullPOMaster.PullOrderNumber = objInventoryCountDTO.PullOrderNumber;
                        PullPOMaster.IsActive = true;
                        pullController.PullPOSave(PullPOMaster);
                    }
                    objInventoryCountDTO = objInventoryCountDAL.SaveInventoryCount(objInventoryCountDTO);
                    if (objInventoryCountDTO.ID > 0)
                    {
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        //SendMailForApplyCount(objInventoryCountDTO);

                        #region WI-6496 - Scheduled Immediate Inventory Count report not sending
                        string dataGUIDs = "<DataGuids>" + objInventoryCountDTO.GUID + "</DataGuids>";
                        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                        NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);

                        if (IsCountCreate)
                        {
                            string eventName = "OCNTC";
                            List<NotificationDTO> lstOCNTCNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                            if (lstOCNTCNotification != null && lstOCNTCNotification.Count > 0)
                            {
                                objNotificationDAL.SendMailForImmediate(lstOCNTCNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                            }
                        }
                        if (IsCountUpdate)
                        {
                            string eventName = "OCNTE";
                            List<NotificationDTO> lstOCNTENotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                            if (lstOCNTENotification != null && lstOCNTENotification.Count > 0)
                            {
                                objNotificationDAL.SendMailForImmediate(lstOCNTENotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "fail";
            }

            Session["IsInsert"] = "True";
            return Json(new { Message = message, Status = status, UpdatedDTO = objInventoryCountDTO });
        }

        public string DeleteInventoryCountRecords(string ids)
        {
            try
            {
                //MaterialStagingDetailAPIController objMaterialStagingDetailAPIController = new MaterialStagingDetailAPIController();
                InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
                objInventoryCountDAL.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                return "ok";
            }
            catch (Exception)
            {

                return "fail";
            }

        }

        #endregion

        #region [Inventory count line items]

        public ActionResult GetCountLineItems(JQueryDataTableParamModel param)
        {
            Guid ICGUID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["IcGuid"]), out ICGUID);
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
            sortColumnName = Request["SortingField"].ToString().Trim();
            bool IsArchived = false;

            if (Request["IsArchived"] != null && !string.IsNullOrEmpty(Request["IsArchived"].ToString()))
                IsArchived = bool.Parse(Request["IsArchived"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID";
            }
            else
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName += " asc";
            else
                sortColumnName += " desc";

            int TotalRecordCount = 0;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            bool IsDeleted = false;
            List<InventoryCountDetailDTO> DataFromDB = new List<InventoryCountDetailDTO>();
            DataFromDB = objInventoryCountDAL.GetPagedCountLineItems(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ICGUID.ToString(), SessionHelper.UserSupplierIds);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemMasterModelIC(Int64 ParentId)
        {
            ViewBag.InventoryCountHeader = new InventoryCountDAL(SessionHelper.EnterPriseDBName).GetInventoryCountById(ParentId, SessionHelper.RoomID, SessionHelper.CompanyID);
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Inventory/AddItemToDetailTable/",
                ModelHeader = eTurns.DTO.ResInventoryCount.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/Inventory/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/Inventory/GetItemsModelMethodForCount/",
                CallingFromPageName = "ICNT",
                PerentID = ParentId.ToString()
            };

            return PartialView("ItemMasterModelIC", obj);
        }

        public ActionResult LoadItemMasterModelICNew(Int64 ParentId, string ParentGuid)
        {
            ViewBag.InventoryCountHeader = new InventoryCountDAL(SessionHelper.EnterPriseDBName).GetInventoryCountById(ParentId, SessionHelper.RoomID, SessionHelper.CompanyID);
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Inventory/AddItemToDetailTable/",
                ModelHeader = eTurns.DTO.ResInventoryCount.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/Inventory/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/Inventory/GetItemsModelMethodForCountNew/",
                CallingFromPageName = "ICNT",
                PerentID = ParentId.ToString(),
                PerentGUID = ParentGuid
            };

            return PartialView("ItemMasterModel", obj);
        }
        public ActionResult GetItemsModelMethodForCount(QuickListJQueryDataTableParamModel param)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            bool ShowStagingLocation = false;
            long CurentParentID = 0;
            string BinId_ItemGuid = string.Empty;
            if (Request["ShowStagingLocation"] != null)
            {
                if (Request["ShowStagingLocation"].ToString() == "1")
                {
                    ShowStagingLocation = false;
                }
                if (Request["ShowStagingLocation"].ToString() == "2")
                {
                    ShowStagingLocation = true;
                }
            }

            string CountGUID = string.Empty;
            if (Request["ParentID"] != null)
            {
                long.TryParse(Convert.ToString(Request["ParentID"]), out CurentParentID);
                InventoryCountDTO objInv = new InventoryCountDTO();
                objInv = objInventoryCountDAL.GetInventoryCountById(CurentParentID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objInv != null)
                {
                    CountGUID = objInv.GUID.ToString();
                }
                //List<InventoryCountDetailDTO> lstLineitems = objInventoryCountDAL.GetAllLineItemsWithinCount(CurentParentID);
                //if (lstLineitems != null && lstLineitems.Any())
                //{
                //    BinId_ItemGuid = string.Join(",", lstLineitems.Select(t => t.BinID + "_" + t.ItemGUID).ToArray());
                //}
            }
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
            //string CountGUID = Convert.ToString(Request["CountGUID"]);

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            if (sortColumnName.Trim() == "ItemUDF1")
                sortColumnName = "UDF1";
            if (sortColumnName.Trim() == "ItemUDF2")
                sortColumnName = "UDF2";
            if (sortColumnName.Trim() == "ItemUDF3")
                sortColumnName = "UDF3";
            if (sortColumnName.Trim() == "ItemUDF4")
                sortColumnName = "UDF4";
            if (sortColumnName.Trim() == "ItemUDF5")
                sortColumnName = "UDF5";
            if (sortColumnName.Contains("Description"))
                sortColumnName = sortColumnName.Replace("Description", "ItemDescription");

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            Int64 ICountId = 0;
            Int64.TryParse(Request["ParentID"], out ICountId);

            Int64 BinID = 0;
            Int64.TryParse(Request["BinID"], out BinID);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<ItemMasterDTO> DataFromDB = objInventoryCountDAL.GetPagedItemLocationsForCount(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, BinID, ShowStagingLocation, BinId_ItemGuid, SessionHelper.UserSupplierIds, CountGUID, RoomDateFormat, CurrentTimeZone).ToList();
            DataFromDB.ToList().ForEach(t =>
            {
                t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            });
            var jsonResult = Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult GetItemsModelMethodForCountNew(QuickListJQueryDataTableParamModel param)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            long CurentParentID = 0;
            string BinId_ItemGuid = string.Empty;
            InventoryCountDTO objInv = new InventoryCountDTO();
            string CountGUID = string.Empty;

            if (Request["ParentID"] != null)
            {
                long.TryParse(Convert.ToString(Request["ParentID"]), out CurentParentID);

                objInv = objInventoryCountDAL.GetInventoryCountById(CurentParentID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objInv != null)
                {
                    CountGUID = objInv.GUID.ToString();
                }
            }

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

            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            if (Request["sSearch_0"] != null && !string.IsNullOrEmpty(Request["sSearch_0"]))
            {
                param.sSearch = Request["sSearch_0"].Trim(',');
            }

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            string modelPopupFor = "inventorycount";
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetPagedItemsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, true, true, true, SessionHelper.UserID, modelPopupFor, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true).ToList();
            var jsonResult = Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult GetItemsModelMethod(QuickListJQueryDataTableParamModel param)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            bool ShowStagingLocation = false;
            long CurentParentID = 0;
            string BinId_ItemGuid = string.Empty;
            if (Request["ShowStagingLocation"] != null)
            {
                if (Request["ShowStagingLocation"].ToString() == "1")
                {
                    ShowStagingLocation = false;
                }
                if (Request["ShowStagingLocation"].ToString() == "2")
                {
                    ShowStagingLocation = true;
                }
            }

            if (Request["ParentID"] != null)
            {
                long.TryParse(Convert.ToString(Request["ParentID"]), out CurentParentID);
                List<InventoryCountDetailDTO> lstLineitems = objInventoryCountDAL.GetAllLineItemsWithinCount(CurentParentID);

                if (lstLineitems != null && lstLineitems.Any())
                {
                    BinId_ItemGuid = string.Join(",", lstLineitems.Select(t => t.BinID + "_" + t.ItemGUID).ToArray());
                }
            }

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

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            if (sortColumnName.Trim() == "ItemUDF1")
                sortColumnName = "UDF1";
            if (sortColumnName.Trim() == "ItemUDF2")
                sortColumnName = "UDF2";
            if (sortColumnName.Trim() == "ItemUDF3")
                sortColumnName = "UDF3";
            if (sortColumnName.Trim() == "ItemUDF4")
                sortColumnName = "UDF4";
            if (sortColumnName.Trim() == "ItemUDF5")
                sortColumnName = "UDF5";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            Int64 ICountId = 0;
            Int64.TryParse(Request["ParentID"], out ICountId);
            Int64 BinID = 0;
            Int64.TryParse(Request["BinID"], out BinID);
            var DataFromDB = objInventoryCountDAL.GetPagedItemLocations(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, BinID, ShowStagingLocation, BinId_ItemGuid, SessionHelper.UserSupplierIds).ToList();
            DataFromDB.ToList().ForEach(t =>
            {
                t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            });

            return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddItemToDetailTable(string para)
        {
            string message = "";
            string status = "";
            int TotalItems = 0;
            int SuccessItems = 0;
            JavaScriptSerializer s = new JavaScriptSerializer();
            InventoryCountDetailDTO[] ICNTDetails = s.Deserialize<InventoryCountDetailDTO[]>(para);

            //------------------------------------------------------------------------------
            //

            if (ICNTDetails != null && ICNTDetails.Count() > 0)
            {
                TotalItems = ICNTDetails.Count();
                foreach (InventoryCountDetailDTO item in ICNTDetails)
                {
                    item.RoomId = SessionHelper.RoomID;
                    item.CreatedBy = SessionHelper.UserID;
                    item.LastUpdatedBy = SessionHelper.UserID;
                    if (!string.IsNullOrWhiteSpace(item.BinNumber))
                    {
                        BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        BinMasterDTO objBin = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID, item.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                        item.BinID = objBin.ID;
                    }
                    item.CompanyId = SessionHelper.CompanyID;

                    Guid CountGUID = Guid.Empty;
                    Guid CountDetailGUID = Guid.Empty;
                    string messageInner = "";
                    string statusInner = "";
                    AddItemToCountAndReturnDetailGUID(item.InventoryCountGUID, item.ItemGUID, item.BinID, item.CountConsignedQuantity, item.CountCustomerOwnedQuantity, item.IsStagingLocationCount, null, null, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, item.CountLineItemDescription, item.ItemType, true, item.ProjectSpendGUID, out CountGUID, out CountDetailGUID, out messageInner, out statusInner);
                    if (statusInner == "ok")
                    {
                        SuccessItems++;
                    }
                }

                if (TotalItems == SuccessItems)
                {
                    message = ResCommon.AllItemsAddedSuccessfully;
                    status = "success";
                }
                else
                {
                    message = string.Format(ResCommon.ItemsAddedSuccessfully,SuccessItems.ToString());
                    status = "success";
                }
            }
            else
            {
                message = ResCommon.NoItemFound;
                status = "fail";
            }

            //message = "Item added";
            //status = "success";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddItemToCount(Guid? CountGuid, Guid? ItemGUID, long? BinId, double? ConsignedQuantity, double? CustomerOwnedQuantity, bool? IsStagingLocationCount, double? ItemLocationCoQty, double? ItemLocationConQty, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string CountLineItemDescription, int? ItemType, Guid? ProjectSpendGUID)
        {
            Guid CountGUID = Guid.Empty;
            Guid CountDetailGUID = Guid.Empty;
            string message = "";
            string status = "";

            AddItemToCountAndReturnDetailGUID(CountGuid, ItemGUID, BinId, ConsignedQuantity, CustomerOwnedQuantity, IsStagingLocationCount, ItemLocationCoQty, ItemLocationConQty, UDF1, UDF2, UDF3, UDF4, UDF5, CountLineItemDescription, ItemType, true, ProjectSpendGUID, out CountGUID, out CountDetailGUID, out message, out status);
            CheckUnappliedLineItem(CountGUID);

            return Json(new { Message = message, Status = status });
        }

        public bool AddItemToCountAndReturnDetailGUID(Guid? CountGuid, Guid? ItemGUID, long? BinId, double? ConsignedQuantity, double? CustomerOwnedQuantity, bool? IsStagingLocationCount, double? ItemLocationCoQty, double? ItemLocationConQty, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string CountLineItemDescription, int? ItemType, bool SaveCountLineItemDetail, Guid? ProjectSpendGUID, out Guid CountGUID, out Guid CountDetailGUID, out string message, out string status, List<CountLineItemDetailDTO> lstCountLineItemDetail = null)
        {
            message = string.Empty;
            status = string.Empty;
            CountGUID = Guid.Empty;
            CountDetailGUID = Guid.Empty;

            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            objInventoryCountDTO = objInventoryCountDAL.GetInventoryCountByGUId(CountGuid ?? Guid.NewGuid(), SessionHelper.RoomID, SessionHelper.CompanyID);
            List<InventoryCountDetailDTO> lstInventoryCountDetail = objInventoryCountDAL.GetAllLineItemsWithinCount(objInventoryCountDTO.ID);

            CountGUID = objInventoryCountDTO.GUID;

            if ((ItemType ?? 0) == 1 || (ItemType ?? 0) == 3)
            {

                if (lstInventoryCountDetail != null && lstInventoryCountDetail.Where(x => x.ItemGUID == ItemGUID && x.BinID == BinId && x.IsApplied == false).Count() <= 0)
                {
                    //List<ItemLocationDetail> lstItemLocations = new List<ItemLocationDetail>();


                    InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();

                    BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinId ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, BinId ?? 0,null,null).FirstOrDefault();
                    objInventoryCountDetailDTO.BinGUID = objBinMasterDTO.GUID;
                    objInventoryCountDetailDTO.BinID = objBinMasterDTO.ID;
                    objInventoryCountDetailDTO.BinNumber = objBinMasterDTO.BinNumber;
                    objInventoryCountDetailDTO.CompanyId = SessionHelper.CompanyID;
                    objInventoryCountDetailDTO.ConsignedQuantity = ConsignedQuantity;
                    objInventoryCountDetailDTO.CountConsignedQuantity = ConsignedQuantity;
                    objInventoryCountDetailDTO.CountCustomerOwnedQuantity = CustomerOwnedQuantity;
                    objInventoryCountDetailDTO.CountDate = objInventoryCountDTO.CountDate;
                    objInventoryCountDetailDTO.CountItemStatus = string.Empty;
                    objInventoryCountDetailDTO.CountLineItemDescription = HttpUtility.UrlDecode(CountLineItemDescription);
                    objInventoryCountDetailDTO.CountName = objInventoryCountDTO.CountName;
                    objInventoryCountDetailDTO.CountQuantity = 0;
                    objInventoryCountDetailDTO.CountStatus = objInventoryCountDTO.CountStatus;
                    objInventoryCountDetailDTO.CountType = objInventoryCountDTO.CountType;
                    objInventoryCountDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objInventoryCountDetailDTO.CreatedBy = SessionHelper.UserID;
                    objInventoryCountDetailDTO.CreatedByName = SessionHelper.UserName;
                    objInventoryCountDetailDTO.CustomerOwnedQuantity = ItemLocationCoQty ?? 0;
                    objInventoryCountDetailDTO.GUID = Guid.NewGuid();
                    objInventoryCountDetailDTO.ID = 0;
                    objInventoryCountDetailDTO.InventoryCountGUID = objInventoryCountDTO.GUID;
                    objInventoryCountDetailDTO.IsStagingLocationCount = IsStagingLocationCount ?? false;
                    objInventoryCountDetailDTO.IsApplied = false;
                    objInventoryCountDetailDTO.IsArchived = false;
                    objInventoryCountDetailDTO.IsClosed = false;
                    objInventoryCountDetailDTO.IsDeleted = false;
                    objInventoryCountDetailDTO.ItemGUID = ItemGUID ?? Guid.NewGuid();
                    objInventoryCountDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                    objInventoryCountDetailDTO.RoomId = SessionHelper.RoomID;
                    objInventoryCountDetailDTO.RoomName = SessionHelper.RoomName;
                    objInventoryCountDetailDTO.UDF1 = HttpUtility.UrlDecode(UDF1) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF1);
                    objInventoryCountDetailDTO.UDF2 = HttpUtility.UrlDecode(UDF2) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF2);
                    objInventoryCountDetailDTO.UDF3 = HttpUtility.UrlDecode(UDF3) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF3);
                    objInventoryCountDetailDTO.UDF4 = HttpUtility.UrlDecode(UDF4) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF4);
                    objInventoryCountDetailDTO.UDF5 = HttpUtility.UrlDecode(UDF5) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF5);
                    objInventoryCountDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objInventoryCountDetailDTO.UpdatedByName = SessionHelper.UserName;
                    objInventoryCountDetailDTO.AddedFrom = "Web";
                    objInventoryCountDetailDTO.EditedFrom = "Web";
                    objInventoryCountDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objInventoryCountDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objInventoryCountDetailDTO.IsOnlyFromItemUI = true;
                    objInventoryCountDetailDTO.ProjectSpendGUID = objInventoryCountDTO.ProjectSpendGUID;
                    try
                    {
                        objInventoryCountDAL.SaveInventoryCountLineItem(objInventoryCountDetailDTO, SaveCountLineItemDetail, lstCountLineItemDetail);
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        CountDetailGUID = objInventoryCountDetailDTO.GUID;
                    }
                    catch
                    {
                        message = ResMessage.SaveErrorMsg;
                        status = "fail";
                        CountDetailGUID = Guid.Empty;
                    }
                }
            }
            if ((ItemType ?? 0) == 2)
            {
                string AlreadyAddedItems = "";
                QuickListDAL objQuickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                List<QuickListDetailDTO> lstQlitems = objQuickListDAL.DB_GetQLLineItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null, null, (ItemGUID ?? Guid.Empty).ToString(), SessionHelper.UserSupplierIds);

                if (lstQlitems != null && lstQlitems.Count > 0)
                {
                    InventoryCountDetailDTO objInventoryCountDetailDTO = null;
                    foreach (var item in lstQlitems)
                    {
                        if (item.SerialNumberTracking != true && item.LotNumberTracking != true && item.DateCodeTracking != true)
                        {
                            if (lstInventoryCountDetail != null && lstInventoryCountDetail.Where(x => x.ItemGUID == item.ItemGUID && x.BinID == BinId).Count() <= 0)
                            {
                                if ((item.ItemGUID ?? Guid.Empty) != Guid.Empty)
                                {
                                    objInventoryCountDetailDTO = new InventoryCountDetailDTO();
                                    BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(item.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    //BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, item.BinID ?? 0,null,null).FirstOrDefault();
                                    if (objBinMasterDTO != null && objBinMasterDTO.ID > 0)
                                    {
                                        objInventoryCountDetailDTO.BinGUID = objBinMasterDTO.GUID;
                                        objInventoryCountDetailDTO.BinID = objBinMasterDTO.ID;
                                        objInventoryCountDetailDTO.BinNumber = objBinMasterDTO.BinNumber;
                                    }
                                    else
                                    {
                                        objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(item.DefaultLocation ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                                        //                                        objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, item.DefaultLocation ?? 0, null,null).FirstOrDefault();
                                        objInventoryCountDetailDTO.BinGUID = objBinMasterDTO.GUID;
                                        objInventoryCountDetailDTO.BinID = objBinMasterDTO.ID;
                                        objInventoryCountDetailDTO.BinNumber = objBinMasterDTO.BinNumber;
                                    }
                                    Guid? GetExistingGuid = Guid.Empty;
                                    if (objBinMasterDTO != null)
                                    {
                                        objInventoryCountDAL.GetCountDetailGUIDByItemGUIDBinID(objInventoryCountDTO.GUID, item.ItemGUID ?? Guid.Empty, objBinMasterDTO.ID, out GetExistingGuid);
                                    }
                                    if (GetExistingGuid == Guid.Empty)
                                    {

                                        ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                                        ItemLocationQTYDTO objItemlocationQty = objItemLocationDetailsDAL.GetItemQtyByLocation(objInventoryCountDetailDTO.BinID, item.ItemGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                                        objInventoryCountDetailDTO.CompanyId = SessionHelper.CompanyID;

                                        //if (objItemlocationQty != null)
                                        //{
                                        //    objInventoryCountDetailDTO.ConsignedQuantity = objItemlocationQty.ConsignedQuantity;
                                        //    objInventoryCountDetailDTO.CustomerOwnedQuantity = objItemlocationQty.CustomerOwnedQuantity;
                                        //}
                                        //
                                        //if (item.Consignment)
                                        //{
                                        //    objInventoryCountDetailDTO.CountConsignedQuantity = item.Quantity * CustomerOwnedQuantity;
                                        //}
                                        //else
                                        //{
                                        //    objInventoryCountDetailDTO.CountCustomerOwnedQuantity = item.Quantity * CustomerOwnedQuantity;
                                        //}

                                        if (item.QuickListType == 3)
                                        {
                                            objInventoryCountDetailDTO.CountConsignedQuantity = item.ConsignedQuantity * CustomerOwnedQuantity;
                                            objInventoryCountDetailDTO.CountCustomerOwnedQuantity = item.Quantity * CustomerOwnedQuantity;
                                        }
                                        else
                                        {
                                            objInventoryCountDetailDTO.CountCustomerOwnedQuantity = item.Quantity * CustomerOwnedQuantity;
                                        }

                                        objInventoryCountDetailDTO.CountDate = objInventoryCountDTO.CountDate;
                                        objInventoryCountDetailDTO.CountItemStatus = string.Empty;
                                        objInventoryCountDetailDTO.CountLineItemDescription = HttpUtility.UrlDecode(CountLineItemDescription);
                                        objInventoryCountDetailDTO.CountName = objInventoryCountDTO.CountName;
                                        objInventoryCountDetailDTO.CountQuantity = 0;
                                        objInventoryCountDetailDTO.CountStatus = objInventoryCountDTO.CountStatus;
                                        objInventoryCountDetailDTO.CountType = objInventoryCountDTO.CountType;
                                        objInventoryCountDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                        objInventoryCountDetailDTO.CreatedBy = SessionHelper.UserID;
                                        objInventoryCountDetailDTO.CreatedByName = SessionHelper.UserName;
                                        objInventoryCountDetailDTO.CustomerOwnedQuantity = ItemLocationCoQty ?? 0;
                                        objInventoryCountDetailDTO.GUID = Guid.NewGuid();
                                        objInventoryCountDetailDTO.ID = 0;
                                        objInventoryCountDetailDTO.InventoryCountGUID = objInventoryCountDTO.GUID;
                                        objInventoryCountDetailDTO.IsStagingLocationCount = IsStagingLocationCount ?? false;
                                        objInventoryCountDetailDTO.IsApplied = false;
                                        objInventoryCountDetailDTO.IsArchived = false;
                                        objInventoryCountDetailDTO.IsClosed = false;
                                        objInventoryCountDetailDTO.IsDeleted = false;
                                        objInventoryCountDetailDTO.ItemGUID = item.ItemGUID ?? Guid.Empty;
                                        objInventoryCountDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                                        objInventoryCountDetailDTO.RoomId = SessionHelper.RoomID;
                                        objInventoryCountDetailDTO.RoomName = SessionHelper.RoomName;
                                        objInventoryCountDetailDTO.UDF1 = HttpUtility.UrlDecode(UDF1) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF1);
                                        objInventoryCountDetailDTO.UDF2 = HttpUtility.UrlDecode(UDF2) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF2);
                                        objInventoryCountDetailDTO.UDF3 = HttpUtility.UrlDecode(UDF3) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF3);
                                        objInventoryCountDetailDTO.UDF4 = HttpUtility.UrlDecode(UDF4) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF4);
                                        objInventoryCountDetailDTO.UDF5 = HttpUtility.UrlDecode(UDF5) == "undefined" ? string.Empty : HttpUtility.UrlDecode(UDF5);
                                        objInventoryCountDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objInventoryCountDetailDTO.UpdatedByName = SessionHelper.UserName;
                                        objInventoryCountDetailDTO.AddedFrom = "Web";
                                        objInventoryCountDetailDTO.EditedFrom = "Web";
                                        objInventoryCountDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objInventoryCountDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objInventoryCountDetailDTO.IsOnlyFromItemUI = true;

                                        try
                                        {
                                            objInventoryCountDAL.SaveInventoryCountLineItem(objInventoryCountDetailDTO, SaveCountLineItemDetail);
                                            message = ResMessage.SaveMessage;
                                            status = "ok";
                                            CountDetailGUID = objInventoryCountDetailDTO.GUID;
                                        }
                                        catch
                                        {
                                            message = ResMessage.SaveErrorMsg;
                                            status = "fail";
                                            CountDetailGUID = Guid.Empty;
                                        }
                                    }
                                    else
                                    {
                                        if(string.IsNullOrWhiteSpace(AlreadyAddedItems))
                                        {
                                            AlreadyAddedItems = item.ItemNumber + ":" + objBinMasterDTO.BinNumber;
                                        }
                                        else
                                        {
                                            AlreadyAddedItems = AlreadyAddedItems + "," + item.ItemNumber + ":" + objBinMasterDTO.BinNumber;
                                        }
                                    }    
                                }
                            }
                            else
                            {
                                status = "ok";
                                message = ResMessage.SaveMessage;
                            }
                        }
                        else
                        {
                            status = "ok";
                            message = ResMessage.SaveMessage;
                        }
                    }
                    if(!string.IsNullOrWhiteSpace(AlreadyAddedItems))
                    {
                        message = string.Format(ResInventoryCountDetail.QLItemAlreadyAddedToCount, AlreadyAddedItems);
                        status = "fail";
                        CountDetailGUID = Guid.Empty;
                    }
                }
            }

            if (status == "ok")
                return true;
            else
                return false;
        }

        [HttpPost]
        public JsonResult AddQLToCount(Guid? CountGuid, Guid? QLGUID, double? Quantity, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string CountLineItemDescription, long? ItemType)
        {
            string message = string.Empty;
            string status = string.Empty;
            try
            {
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            catch
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            return Json(new { Message = message, Status = status });
        }

        [HttpPost]
        public JsonResult IcLineItemsSave(List<InventoryCountDetailDTO> lstLineItems)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            string message = string.Empty, status = string.Empty;
            try
            {
                objInventoryCountDAL.SaveCountLineItems(lstLineItems, SessionHelper.UserID);
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            catch
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            return Json(new { Message = message, Status = status });
        }

        [HttpPost]
        public JsonResult ApplyCountOnLineItemsNew(List<InventoryCountDetailDTO> lstLineItems)
        {
         
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            var resp = objInventoryCountDAL.ApplyCountOnLineItemsNew(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID
                , lstLineItems, SessionHelper.RoomDateFormat,true,SessionHelper.EnterPriceID);

            return Json(new { Message = resp.Message, Status = resp.Status, CurrentObj = resp.CurrentObj, IsCountClosed = resp.IsCountClosed });

            //InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            //InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();
            //ItemMasterDTO objItemDTO;
            //ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //double CustomerOwnedQty = 0;
            //bool IsValidInvalidQuantity = true;
            //double ConsignedOwnedQty = 0;
            //string message = string.Empty, status = string.Empty;
            //int InvalidQuantity = 0;
            //int TotalItems = 0;
            //bool _IsCountClosed = false;

            //// RoomDTO objRoomDTO = new RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
            //string columnList = "ID,RoomName,IsIgnoreCreditRule";
            //CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            //long SessionUserId = SessionHelper.UserID;
            //if (lstLineItems != null && lstLineItems.Count > 0)
            //{
            //    try
            //    {
            //        foreach (InventoryCountDetailDTO InventoryCount in lstLineItems)
            //        {
            //            if (InventoryCount.SerialNumberTracking == false && InventoryCount.LotNumberTracking == false
            //                && InventoryCount.CountCustomerOwnedQuantity == null
            //                && (InventoryCount.Consignment == true && InventoryCount.CountConsignedQuantity == null))
            //            {
            //                return Json(new { Message = "Please provide valid value for customer owned quantity Or consigned quantity", Status = "fail", IsCountClosed = false });
            //                //if (InventoryCount.CountCustomerOwnedQuantity == null)
            //                //{
            //                //    return Json(new { Message = "Please provide valid value for customer owned quantity", Status = "fail", IsCountClosed = false });
            //                //}
            //                //else if (InventoryCount.Consignment == true && InventoryCount.CountConsignedQuantity == null)
            //                //{
            //                //    return Json(new { Message = "Please provide valid value for consigned quantity", Status = "fail", IsCountClosed = false });
            //                //}
            //            }
            //        }

            //        ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            //        foreach (InventoryCountDetailDTO InventoryCount in lstLineItems)
            //        {
            //            objItemDTO = objItemDAL.GetItemWithoutJoins(null, InventoryCount.ItemGUID);
            //            if (InventoryCount.SerialNumberTracking == true)
            //            {
            //                AddMissingLotSerial(InventoryCount.CountDetailGUID, InventoryCount.ItemGUID);
            //            }

            //            IsValidInvalidQuantity = true;
            //            objInventoryCountDTO = objInventoryCountDAL.GetInventoryCountByLIGUId(InventoryCount.CountDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            //            List<CountLineItemDetailDTO> lstCountLineItemDetail = objInventoryCountDAL.GetLotDetailForCountByCountDetailGUID((InventoryCount.GUID != null && InventoryCount.GUID != Guid.Empty ? InventoryCount.GUID : InventoryCount.CountDetailGUID),
            //                                                                                                                              InventoryCount.ItemGUID, SessionHelper.RoomDateFormat, SessionHelper.CompanyID, SessionHelper.RoomID);

            //            bool isValidConsignedCredit = false;
            //            bool isValidCusOwnedCredit = false;
            //            if (objInventoryCountDTO.CountType.ToLower().Equals("m"))
            //            {
            //                foreach (CountLineItemDetailDTO objCountLineItemDetailDTO in lstCountLineItemDetail)
            //                {
            //                    bool IsStagginLocation = false;
            //                    BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objCountLineItemDetailDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
            //                    if (objLocDTO != null)
            //                    {
            //                        IsStagginLocation = objLocDTO.IsStagingLocation;
            //                    }

            //                    //isValidConsignedCredit = false;
            //                    //isValidCusOwnedCredit = false;
            //                    bool isnotsufficientpull = false;
            //                    double? CusOwnedDifference = 0;
            //                    double? ConsignedDifference = 0;

            //                    double? countedCusOwned = 0;
            //                    double? countedConsigned = 0;
            //                    if (IsStagginLocation)
            //                    {
            //                        IEnumerable<MaterialStagingPullDetailDTO> lstMSPullDetail = null;
            //                        lstMSPullDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetCountDifferenceforValidatMSPull(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.RoomID.GetValueOrDefault(0), objCountLineItemDetailDTO.CompanyID.GetValueOrDefault(0), objCountLineItemDetailDTO.BinID.GetValueOrDefault(0));
            //                        if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        else if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber))
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber)).ToList();
            //                        }
            //                        else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber))
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber)).ToList();
            //                        }
            //                        else if (objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        if (lstMSPullDetail != null && lstMSPullDetail.Count() > 0)
            //                        {
            //                            countedCusOwned = lstMSPullDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
            //                            countedConsigned = lstMSPullDetail.Sum(x => x.ConsignedQuantity) ?? 0;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetail = null;
            //                        lstItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetCountDifferenceforValidatPull(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.RoomID.GetValueOrDefault(0), objCountLineItemDetailDTO.CompanyID.GetValueOrDefault(0), objCountLineItemDetailDTO.BinID.GetValueOrDefault(0));
            //                        if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        else if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber))
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber)).ToList();
            //                        }
            //                        else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber))
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber)).ToList();
            //                        }
            //                        else if (objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
            //                        {
            //                            countedCusOwned = lstItemLocationDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
            //                            countedConsigned = lstItemLocationDetail.Sum(x => x.ConsignedQuantity) ?? 0;
            //                        }
            //                    }
            //                    CusOwnedDifference = (objCountLineItemDetailDTO.CountCustomerOwnedQuantity - countedCusOwned);
            //                    ConsignedDifference = (objCountLineItemDetailDTO.CountConsignedQuantity - countedConsigned);

            //                    if (CusOwnedDifference > 0 || ConsignedDifference > 0)
            //                    {

            //                        double TotalConsignedPoolQuantity = 0;
            //                        double TotalCusOwnedPoolQuantity = 0;
            //                        string _pullAction = "pull";
            //                        IEnumerable<PullDetailsDTO> lstPullDetailDTO = null;
            //                        PullDetailsDAL pullDetailDAL = new PullDetailsDAL(SessionHelper.EnterPriseDBName);
            //                        if (IsStagginLocation)
            //                        {
            //                            _pullAction = "ms pull";
            //                        }
            //                        else
            //                        {
            //                            _pullAction = "pull";
            //                        }

            //                        PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);

            //                        if (InventoryCount.SerialNumberTracking)
            //                        {
            //                            lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuidAndSerialNo_CreditHistoryForCountNormal(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), _pullAction, RoomID, CompanyID, objCountLineItemDetailDTO.SerialNumber);
            //                            // if (objRoomDTO.IsIgnoreCreditRule)
            //                            // {
            //                            bool IsSerailAvailableForCredit = objPullDetails.ValidateSerialNumberForCreditCount(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.SerialNumber, SessionHelper.CompanyID, SessionHelper.RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
            //                            if (IsSerailAvailableForCredit == false)
            //                            {
            //                                return Json(new { Message = "Credit transaction is already done for selected Serial Number " + objCountLineItemDetailDTO.SerialNumber, Status = "fail", IsCountClosed = false });
            //                            }
            //                          // }
            //                       }
            //                       else if (InventoryCount.LotNumberTracking)
            //                       {
            //                            lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuidAndLotNo_CreditHistoryForCountNormal(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), _pullAction, RoomID, CompanyID, objCountLineItemDetailDTO.LotNumber);
            //                           //if (objRoomDTO.IsIgnoreCreditRule)
            //                           //{
            //                               if (InventoryCount.LotNumberTracking && InventoryCount.DateCodeTracking)
            //                               {
            //                                   DateTime ExpirationDate;
            //                                   if (!string.IsNullOrWhiteSpace(Convert.ToString(objCountLineItemDetailDTO.ExpirationDate)))
            //                                   {
            //                                       ExpirationDate = Convert.ToDateTime(objCountLineItemDetailDTO.ExpirationDate.Value.ToString("MM/dd/yyyy"));
            //                                       bool IsLotAvailableForCredit = objPullDetails.ValidateLotDateCodeForCredit(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.LotNumber, ExpirationDate, SessionHelper.CompanyID, SessionHelper.RoomID);

            //                                       if (!IsLotAvailableForCredit)
            //                                       {
            //                                           return Json(new { Message = "Credit transaction is for selected Lot Number " + objCountLineItemDetailDTO.LotNumber + " and ExpirationDate " + ExpirationDate.ToString(Convert.ToString(SessionHelper.RoomDateFormat)) + " is available.You can not use other lot", Status = "fail", IsCountClosed = false });
            //                                       }
            //                                   }
            //                               }
            //                           //}
            //                       }

            //                        if (!InventoryCount.SerialNumberTracking && !InventoryCount.LotNumberTracking)
            //                        {
            //                            lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid_CreditHistoryForCountNormal(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), _pullAction, RoomID, CompanyID);
            //                        }

            //                        if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
            //                        {
            //                            if (ConsignedDifference > 0)
            //                            {
            //                                TotalConsignedPoolQuantity = lstPullDetailDTO.Where(x => (x.ConsignedQuantity ?? 0) > 0).Sum(x => x.PoolQuantity ?? 0);
            //                                if (TotalConsignedPoolQuantity >= (ConsignedDifference))
            //                                {
            //                                    isValidConsignedCredit = true;
            //                                }
            //                                else
            //                                {
            //                                    isValidConsignedCredit = false;
            //                                    isnotsufficientpull = true;
            //                                }
            //                            }
            //                            if (CusOwnedDifference > 0)
            //                            {
            //                                TotalCusOwnedPoolQuantity = lstPullDetailDTO.Where(x => (x.CustomerOwnedQuantity ?? 0) > 0).Sum(x => x.PoolQuantity ?? 0);
            //                                if (TotalCusOwnedPoolQuantity >= (CusOwnedDifference))
            //                                {
            //                                    isValidCusOwnedCredit = true;
            //                                }
            //                                else
            //                                {
            //                                    isValidCusOwnedCredit = false;
            //                                    isnotsufficientpull = true;
            //                                }
            //                            }
            //                            if (isnotsufficientpull)
            //                            {
            //                                if (!objRoomDTO.IsIgnoreCreditRule)
            //                                {
            //                                    return Json(new { Message = "Can not apply count because enough pull is not available for credit count.", Status = "fail", IsCountClosed = false });
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (!objRoomDTO.IsIgnoreCreditRule)
            //                            {
            //                                return Json(new { Message = "Can not apply count because enough pull is not available for credit count.", Status = "fail", IsCountClosed = false });
            //                            }
            //                        }

            //                    }
            //                }
            //            }
            //            else if (objInventoryCountDTO.CountType.ToLower().Equals("a"))
            //            {
            //                foreach (CountLineItemDetailDTO objCountLineItemDetailDTO in lstCountLineItemDetail)
            //                {
            //                    bool IsStagginLocation = false;
            //                    BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objCountLineItemDetailDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
            //                    if (objLocDTO != null)
            //                    {
            //                        IsStagginLocation = objLocDTO.IsStagingLocation;
            //                    }

            //                    double? CusOwnedDifference = 0;
            //                    double? ConsignedDifference = 0;

            //                    double? countedCusOwned = 0;
            //                    double? countedConsigned = 0;

            //                    if (IsStagginLocation)
            //                    {
            //                        IEnumerable<MaterialStagingPullDetailDTO> lstMSPullDetail = null;
            //                        lstMSPullDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetCountDifferenceforValidatMSPull(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.RoomID.GetValueOrDefault(0), objCountLineItemDetailDTO.CompanyID.GetValueOrDefault(0), objCountLineItemDetailDTO.BinID.GetValueOrDefault(0));
            //                        if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        else if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber))
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber)).ToList();
            //                        }
            //                        else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber))
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber)).ToList();
            //                        }
            //                        else if (objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstMSPullDetail = lstMSPullDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        if (lstMSPullDetail != null && lstMSPullDetail.Count() > 0)
            //                        {
            //                            countedCusOwned = lstMSPullDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
            //                            countedConsigned = lstMSPullDetail.Sum(x => x.ConsignedQuantity) ?? 0;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetail = null;
            //                        lstItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetCountDifferenceforValidatPull(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.RoomID.GetValueOrDefault(0), objCountLineItemDetailDTO.CompanyID.GetValueOrDefault(0), objCountLineItemDetailDTO.BinID.GetValueOrDefault(0));
            //                        if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber) && objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber) && x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        else if (InventoryCount.SerialNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.SerialNumber))
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.SerialNumber.Equals(objCountLineItemDetailDTO.SerialNumber)).ToList();
            //                        }
            //                        else if (InventoryCount.LotNumberTracking && !string.IsNullOrEmpty(objCountLineItemDetailDTO.LotNumber))
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.LotNumber.Equals(objCountLineItemDetailDTO.LotNumber)).ToList();
            //                        }
            //                        else if (objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) && objCountLineItemDetailDTO.ExpirationDate != null && objCountLineItemDetailDTO.ExpirationDate != DateTime.MinValue)
            //                        {
            //                            lstItemLocationDetail = lstItemLocationDetail.Where(x => x.ExpirationDate.Value.ToShortDateString().Equals(objCountLineItemDetailDTO.ExpirationDate.Value.ToShortDateString())).ToList();
            //                        }
            //                        if (lstItemLocationDetail != null && lstItemLocationDetail.Count() > 0)
            //                        {
            //                            countedCusOwned = lstItemLocationDetail.Sum(x => x.CustomerOwnedQuantity) ?? 0;
            //                            countedConsigned = lstItemLocationDetail.Sum(x => x.ConsignedQuantity) ?? 0;
            //                        }
            //                    }

            //                    CusOwnedDifference = (objCountLineItemDetailDTO.CountCustomerOwnedQuantity - countedCusOwned);
            //                    ConsignedDifference = (objCountLineItemDetailDTO.CountConsignedQuantity - countedConsigned);

            //                    if (CusOwnedDifference > 0 || ConsignedDifference > 0)
            //                    {
            //                        if (InventoryCount.SerialNumberTracking)
            //                        {
            //                            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            //                            //bool IsSerailAvailableForCredit = objPullDetails.ValidateSerialNumberForCredit(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.SerialNumber, SessionHelper.CompanyID, SessionHelper.RoomID);
            //                            bool IsSerailAvailableForCredit = objPullDetails.ValidateSerialNumberForCreditCount(objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objCountLineItemDetailDTO.SerialNumber, SessionHelper.CompanyID, SessionHelper.RoomID, ConsignedDifference.GetValueOrDefault(0), CusOwnedDifference.GetValueOrDefault(0));
            //                            if (IsSerailAvailableForCredit == false)
            //                            {
            //                                return Json(new { Message = "Credit transaction is already done for selected Serial Number " + objCountLineItemDetailDTO.SerialNumber, Status = "fail", IsCountClosed = false });
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            objInventoryCountDetailDTO = objInventoryCountDAL.BeforeApplyAction(InventoryCount, SessionHelper.UserID, lstCountLineItemDetail);
            //            if (lstCountLineItemDetail != null || lstCountLineItemDetail.Count > 0)
            //            {
            //                CustomerOwnedQty = lstCountLineItemDetail.Sum(x => (x.CountCustomerOwnedQuantity == null ? 0 : x.CountCustomerOwnedQuantity.Value));
            //                ConsignedOwnedQty = lstCountLineItemDetail.Sum(x => (x.CountConsignedQuantity == null ? 0 : x.CountConsignedQuantity.Value));
            //                TotalItems++;
            //                if (((objInventoryCountDetailDTO.CountCustomerOwnedQuantity == null || objInventoryCountDetailDTO.CountCustomerOwnedQuantity == (-0.000000001)) ? 0 : objInventoryCountDetailDTO.CountCustomerOwnedQuantity.Value) != (CustomerOwnedQty == (-0.000000001) ? 0 : CustomerOwnedQty)
            //                    || ((objInventoryCountDetailDTO.CountConsignedQuantity == null || objInventoryCountDetailDTO.CountConsignedQuantity == (-0.000000001)) ? 0 : objInventoryCountDetailDTO.CountConsignedQuantity.Value) != (ConsignedOwnedQty == (-0.000000001) ? 0 : ConsignedOwnedQty))
            //                {
            //                    if (InventoryCount.SerialNumberTracking == true || InventoryCount.LotNumberTracking == true)
            //                    {
            //                        IsValidInvalidQuantity = false;
            //                        InvalidQuantity++;
            //                    }
            //                    else
            //                    {
            //                        CustomerOwnedQty = ((objInventoryCountDetailDTO.CountCustomerOwnedQuantity == null || objInventoryCountDetailDTO.CountCustomerOwnedQuantity == (-0.000000001)) ? 0 : objInventoryCountDetailDTO.CountCustomerOwnedQuantity.Value);
            //                        ConsignedOwnedQty = ((objInventoryCountDetailDTO.CountConsignedQuantity == null || objInventoryCountDetailDTO.CountConsignedQuantity == (-0.000000001)) ? 0 : objInventoryCountDetailDTO.CountConsignedQuantity.Value);
            //                        objInventoryCountDAL.UpdateCountLineItemDetailQty(InventoryCount.CountDetailGUID, objInventoryCountDetailDTO.CountCustomerOwnedQuantity, objInventoryCountDetailDTO.CountConsignedQuantity, objInventoryCountDetailDTO.CusOwnedDifference, objInventoryCountDetailDTO.ConsignedDifference);
            //                    }
            //                }

            //                if (IsValidInvalidQuantity == true)
            //                {
            //                    List<ItemLocationDetailsDTO> lstProperRecords = new List<ItemLocationDetailsDTO>();
            //                    List<MaterialStagingPullDetailDTO> lstStgProperRecords = new List<MaterialStagingPullDetailDTO>();
            //                    CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            //                    ItemLocationDetailsDTO objItemLocationDetailsDTO;
            //                    MaterialStagingPullDetailDTO objMaterialStgPullDetailsDTO;
            //                    bool isProperRecordsAvail = false;

            //                    foreach (CountLineItemDetailDTO objCountLineItemDetailDTO in lstCountLineItemDetail)
            //                    {
            //                        if (objCountLineItemDetailDTO.IsStagingLocationCount.GetValueOrDefault(false) == true)
            //                        {
            //                            objMaterialStgPullDetailsDTO = new MaterialStagingPullDetailDTO();
            //                            objMaterialStgPullDetailsDTO.ItemGUID = objCountLineItemDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty);
            //                            objMaterialStgPullDetailsDTO.StagingBinId = objCountLineItemDetailDTO.BinID.GetValueOrDefault(0);

            //                            objMaterialStgPullDetailsDTO.ConsignedQuantity = objCountLineItemDetailDTO.CountConsignedQuantity;
            //                            objMaterialStgPullDetailsDTO.CustomerOwnedQuantity = objCountLineItemDetailDTO.CountCustomerOwnedQuantity;
            //                            if (objMaterialStgPullDetailsDTO.SerialNumberTracking == true || objMaterialStgPullDetailsDTO.LotNumberTracking == true)
            //                            {
            //                                if (objMaterialStgPullDetailsDTO.ConsignedQuantity < 0)
            //                                    objMaterialStgPullDetailsDTO.ConsignedQuantity = 0;

            //                                if (objMaterialStgPullDetailsDTO.CustomerOwnedQuantity < 0)
            //                                    objMaterialStgPullDetailsDTO.CustomerOwnedQuantity = 0;
            //                            }

            //                            objMaterialStgPullDetailsDTO.UDF1 = HttpUtility.UrlDecode(InventoryCount.UDF1);
            //                            objMaterialStgPullDetailsDTO.UDF2 = HttpUtility.UrlDecode(InventoryCount.UDF2);
            //                            objMaterialStgPullDetailsDTO.UDF3 = HttpUtility.UrlDecode(InventoryCount.UDF3);
            //                            objMaterialStgPullDetailsDTO.UDF4 = HttpUtility.UrlDecode(InventoryCount.UDF4);
            //                            objMaterialStgPullDetailsDTO.UDF5 = HttpUtility.UrlDecode(InventoryCount.UDF5);
            //                            objMaterialStgPullDetailsDTO.ItemNumber = objCountLineItemDetailDTO.ItemNumber;
            //                            objMaterialStgPullDetailsDTO.BinNumber = objCountLineItemDetailDTO.BinNumber;
            //                            objMaterialStgPullDetailsDTO.Expiration = objCountLineItemDetailDTO.Expiration;
            //                            objMaterialStgPullDetailsDTO.Received = objCountLineItemDetailDTO.Received;
            //                            objMaterialStgPullDetailsDTO.LotNumber = (!string.IsNullOrWhiteSpace(objCountLineItemDetailDTO.LotNumber)) ? objCountLineItemDetailDTO.LotNumber.Trim() : string.Empty;
            //                            objMaterialStgPullDetailsDTO.SerialNumber = (!string.IsNullOrWhiteSpace(objCountLineItemDetailDTO.SerialNumber)) ? objCountLineItemDetailDTO.SerialNumber.Trim() : string.Empty;
            //                            lstStgProperRecords.Add(objMaterialStgPullDetailsDTO);
            //                        }
            //                        else
            //                        {
            //                            objItemLocationDetailsDTO = new ItemLocationDetailsDTO();
            //                            objItemLocationDetailsDTO.ItemGUID = objCountLineItemDetailDTO.ItemGUID;
            //                            objItemLocationDetailsDTO.BinID = objCountLineItemDetailDTO.BinID;

            //                            objItemLocationDetailsDTO.ConsignedQuantity = objCountLineItemDetailDTO.CountConsignedQuantity;
            //                            objItemLocationDetailsDTO.CustomerOwnedQuantity = objCountLineItemDetailDTO.CountCustomerOwnedQuantity;
            //                            if (objCountLineItemDetailDTO.SerialNumberTracking == true || objCountLineItemDetailDTO.LotNumberTracking == true)
            //                            {
            //                                if (objItemLocationDetailsDTO.ConsignedQuantity < 0)
            //                                    objItemLocationDetailsDTO.ConsignedQuantity = 0;

            //                                if (objItemLocationDetailsDTO.CustomerOwnedQuantity < 0)
            //                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = 0;
            //                            }

            //                            objItemLocationDetailsDTO.UDF1 = HttpUtility.UrlDecode(InventoryCount.UDF1);
            //                            objItemLocationDetailsDTO.UDF2 = HttpUtility.UrlDecode(InventoryCount.UDF2);
            //                            objItemLocationDetailsDTO.UDF3 = HttpUtility.UrlDecode(InventoryCount.UDF3);
            //                            objItemLocationDetailsDTO.UDF4 = HttpUtility.UrlDecode(InventoryCount.UDF4);
            //                            objItemLocationDetailsDTO.UDF5 = HttpUtility.UrlDecode(InventoryCount.UDF5);
            //                            objItemLocationDetailsDTO.ItemNumber = objCountLineItemDetailDTO.ItemNumber;
            //                            objItemLocationDetailsDTO.BinNumber = objCountLineItemDetailDTO.BinNumber;
            //                            objItemLocationDetailsDTO.Expiration = objCountLineItemDetailDTO.Expiration;
            //                            objItemLocationDetailsDTO.Received = objCountLineItemDetailDTO.Received;
            //                            objItemLocationDetailsDTO.LotNumber = (!string.IsNullOrWhiteSpace(objCountLineItemDetailDTO.LotNumber)) ? objCountLineItemDetailDTO.LotNumber.Trim() : string.Empty;
            //                            objItemLocationDetailsDTO.SerialNumber = (!string.IsNullOrWhiteSpace(objCountLineItemDetailDTO.SerialNumber)) ? objCountLineItemDetailDTO.SerialNumber.Trim() : string.Empty;
            //                            lstProperRecords.Add(objItemLocationDetailsDTO);
            //                        }

            //                        if (objCountLineItemDetailDTO.LotNumberTracking.GetValueOrDefault(false) == true || objCountLineItemDetailDTO.SerialNumberTracking.GetValueOrDefault(false) == true || objCountLineItemDetailDTO.DateCodeTracking.GetValueOrDefault(false) == true)
            //                            UpdateCountLineItemOnApply(objCountLineItemDetailDTO, lstCountLineItemDetail);
            //                    }


            //                    if (lstStgProperRecords != null && lstStgProperRecords.Count > 0)
            //                    {
            //                        DataTable dtItemLocations = GetTableFromStgList(lstStgProperRecords);
            //                        objItemLocationDetailsDAL.ApplyStageCountLineitem(dtItemLocations, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);

            //                        if (objInventoryCountDTO.CountType.ToLower().Equals("m"))
            //                        {
            //                            if (objRoomDTO.IsIgnoreCreditRule)
            //                            {
            //                                if (objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
            //                                    new PullMasterDAL(SessionHelper.EnterPriseDBName).InsertintoMSCreditHistoryForCount(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID, "MS Pull Credit");
            //                            }
            //                            else
            //                            {
            //                                if ((isValidConsignedCredit || isValidCusOwnedCredit) && objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
            //                                    new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).InsertMSCreditHistoryForCount(RoomID, CompanyID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if ((isValidConsignedCredit || isValidCusOwnedCredit) && objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
            //                                new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).InsertMSCreditHistoryForCount(RoomID, CompanyID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
            //                        }

            //                        isProperRecordsAvail = true;
            //                    }


            //                    if (lstProperRecords != null && lstProperRecords.Count > 0)
            //                    {
            //                        DataTable dtItemLocations = GetTableFromList(lstProperRecords);
            //                        objItemLocationDetailsDAL.ApplyCountLineitem(dtItemLocations, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
            //                        if (objInventoryCountDTO.CountType.ToLower().Equals("m"))
            //                        {
            //                            if (objRoomDTO.IsIgnoreCreditRule)
            //                            {
            //                                if (objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
            //                                    new PullMasterDAL(SessionHelper.EnterPriseDBName).InsertintoCreditHistoryForCount(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID, "Credit");
            //                            }
            //                            else
            //                            {
            //                                if ((isValidConsignedCredit || isValidCusOwnedCredit) && objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
            //                                    new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).InsertCreditHistoryForCount(RoomID, CompanyID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if ((isValidConsignedCredit || isValidCusOwnedCredit) && objInventoryCountDTO.GUID != Guid.Empty && objInventoryCountDetailDTO.GUID != Guid.Empty)
            //                                new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).InsertCreditHistoryForCount(RoomID, CompanyID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
            //                        }

            //                        lstProperRecords.ForEach(t =>
            //                        {
            //                            //objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, SessionHelper.UserID, "Web", "Inventorycontroller>> ApplyOnCountLineItem");
            //                            objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, SessionHelper.UserID, "Web", "Inventory >> Apply Count", SessionUserId);
            //                        });

            //                        if (objItemDTO != null && objItemDTO.ItemType == 3 && objItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
            //                        {
            //                            new KitDetailDAL(SessionHelper.EnterPriseDBName).UpdateQtyToMeedDemand(InventoryCount.ItemGUID, SessionHelper.UserID, SessionUserId);
            //                        }

            //                        //if (InventoryCount.ItemLotSerialType != "LOT_SERIAL")
            //                        //{
            //                        //    ApplyOnCountLineItem(objInventoryCountDetailDTO);
            //                        //}

            //                        isProperRecordsAvail = true;

            //                    }

            //                    if (isProperRecordsAvail)
            //                    {
            //                        PostApplyAction(objInventoryCountDetailDTO);
            //                    }
            //                }
            //            }
            //        }

            //        if (InvalidQuantity == 0)
            //        {
            //            /* closed count comment for WI-3311 */
            //            //if (objItemLocationDetailsDAL.CloseCountIfAllApplied(lstLineItems[0].InventoryCountGUID, SessionHelper.UserID, "Web"))
            //            //{
            //            //    _IsCountClosed = true;
            //            //}
            //            return Json(new { Message = ResMessage.SaveMessage, Status = "ok", IsCountClosed = _IsCountClosed });
            //        }
            //        else if (TotalItems == InvalidQuantity)
            //        {
            //            return Json(new { Message = "Can not apply count because total quantities are not matching with detail quantities", Status = "fail", IsCountClosed = false });
            //        }
            //        else
            //        {
            //            return Json(new { Message = "Can not apply count for some items because total quantities are not matching with detail quantities", Status = "fail", IsCountClosed = false });
            //        }
            //    }
            //    catch
            //    {
            //        return Json(new { Message = ResMessage.SaveErrorMsg, Status = "fail", IsCountClosed = false });
            //    }
            //}
            //else
            //{
            //    message = ResMessage.SaveErrorMsg;
            //    status = "fail";
            //    return Json(new { Message = message, Status = status, CurrentObj = lstLineItems.First(), IsCountClosed = false });
            //}
        }

        [HttpPost]
        public JsonResult ApplyCountOnLineItems(List<InventoryCountDetailDTO> lstLineItems)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();
            string message = string.Empty, status = string.Empty;
            if (lstLineItems != null && lstLineItems.Count > 0)
            {
                try
                {
                    objInventoryCountDetailDTO = objInventoryCountDAL.BeforeApplyAction(lstLineItems.First(), SessionHelper.UserID);


                    message = ResMessage.SaveMessage;

                    if (objInventoryCountDetailDTO.TotalDifference < 0)
                    {
                        status = "pull";
                    }
                    else if (objInventoryCountDetailDTO.TotalDifference > 0)
                    {
                        if (objInventoryCountDetailDTO.IsStagingLocationCount)
                        {
                            status = "creditms";
                        }
                        else
                        {
                            status = "credit";
                        }
                    }
                    else
                    {
                        status = "ok";
                    }
                }
                catch (Exception)
                {
                    message = ResMessage.SaveErrorMsg;
                    status = "fail";
                }
                return Json(new { Message = message, Status = status, CurrentObj = objInventoryCountDetailDTO });
            }
            else
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
                return Json(new { Message = message, Status = status, CurrentObj = lstLineItems.First() });
            }
        }

        [HttpPost]
        public JsonResult DeleteCountLineItems(string ids, string guids, Guid? CountGUId)
        {
            string message = string.Empty, status = string.Empty;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            try
            {
                objInventoryCountDAL.DeleteCountLineItems(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            catch (Exception)
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            return Json(new { Message = message, Status = status });
        }

        public JsonResult UpdateCountCalculations(long? ID, double? CountConsignedQty, double? CountCutOwnedQty)
        {
            InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();
            try
            {
                InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
                return Json(objInventoryCountDetailDTO);
            }
            catch (Exception)
            {
                return Json(objInventoryCountDetailDTO);
            }
        }

        [HttpPost]
        public JsonResult PostApplyAction(InventoryCountDetailDTO objInventoryCountDetailDTO)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            objInventoryCountDAL.PostApplyOnSignleLineItem(objInventoryCountDetailDTO, SessionHelper.UserID);
            return Json(null);
        }

        [HttpPost]
        public JsonResult FromLocApplyActionForCount(Guid? DtlGUID)
        {

            // Guid CountDtlGUID = DtlGUID;// new Guid(DtlGUID);
            InventoryCountDetailDAL objDtlDal = new InventoryCountDetailDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDetailDTO objInv = new InventoryCountDetailDTO();
            objInv = objDtlDal.GetInventoryDetailByGUIDPlain(DtlGUID ?? Guid.NewGuid(), SessionHelper.RoomID, SessionHelper.CompanyID);
            InventoryCountDAL objInvDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            objInvDAL.PostApplyOnSignleLineItem(objInv, SessionHelper.UserID);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult AddAllItemToCount(List<InventoryCountDetailDTO> lstLineItems)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            InventoryCountDetailDTO objInventoryCountDetailDTO;
            QuickListDAL objQuickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
            string message = string.Empty;
            string status = string.Empty;
            try
            {
                foreach (var item in lstLineItems)
                {
                    objInventoryCountDetailDTO = new InventoryCountDetailDTO();
                    objInventoryCountDTO = objInventoryCountDAL.GetInventoryCountByGUId(item.InventoryCountGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (item.ItemType != 2)
                    {
                        BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(item.BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        //BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, item.BinID,null,null).FirstOrDefault();
                        if (objBinMasterDTO != null)
                        {
                            objInventoryCountDetailDTO.BinGUID = objBinMasterDTO.GUID;
                            objInventoryCountDetailDTO.BinID = objBinMasterDTO.ID;
                            objInventoryCountDetailDTO.BinNumber = objBinMasterDTO.BinNumber;
                        }
                    }
                    objInventoryCountDetailDTO.CompanyId = SessionHelper.CompanyID;
                    objInventoryCountDetailDTO.ConsignedQuantity = item.ConsignedQuantity ?? 0;

                    //objInventoryCountDetailDTO.CountConsignedQuantity = item.ConsignedQuantity;
                    //objInventoryCountDetailDTO.CountCustomerOwnedQuantity = item.CustomerOwnedQuantity;
                    if (item.ItemType == 1 || item.ItemType == 3)
                    {
                        objInventoryCountDetailDTO.CountConsignedQuantity = item.ConsignedQuantity;
                        objInventoryCountDetailDTO.CountCustomerOwnedQuantity = item.CustomerOwnedQuantity;
                        objInventoryCountDetailDTO.CountDate = objInventoryCountDTO.CountDate;
                        objInventoryCountDetailDTO.CountItemStatus = string.Empty;
                        objInventoryCountDetailDTO.CountLineItemDescription = HttpUtility.UrlDecode(item.CountLineItemDescription);
                        objInventoryCountDetailDTO.CountName = objInventoryCountDTO.CountName;
                        objInventoryCountDetailDTO.CountQuantity = 0;
                        objInventoryCountDetailDTO.CountStatus = objInventoryCountDTO.CountStatus;
                        objInventoryCountDetailDTO.CountType = objInventoryCountDTO.CountType;
                        objInventoryCountDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetailDTO.CreatedBy = SessionHelper.UserID;
                        objInventoryCountDetailDTO.CreatedByName = SessionHelper.UserName;
                        objInventoryCountDetailDTO.CustomerOwnedQuantity = 0;
                        objInventoryCountDetailDTO.GUID = Guid.NewGuid();
                        objInventoryCountDetailDTO.ID = 0;
                        objInventoryCountDetailDTO.InventoryCountGUID = objInventoryCountDTO.GUID;
                        objInventoryCountDetailDTO.IsStagingLocationCount = item.IsStagingLocationCount;
                        objInventoryCountDetailDTO.IsApplied = false;
                        objInventoryCountDetailDTO.IsArchived = false;
                        objInventoryCountDetailDTO.IsClosed = false;
                        objInventoryCountDetailDTO.IsDeleted = false;
                        objInventoryCountDetailDTO.ItemGUID = item.ItemGUID;
                        objInventoryCountDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                        objInventoryCountDetailDTO.RoomId = SessionHelper.RoomID;
                        objInventoryCountDetailDTO.RoomName = SessionHelper.RoomName;
                        objInventoryCountDetailDTO.UDF1 = HttpUtility.UrlDecode(item.UDF1);
                        objInventoryCountDetailDTO.UDF2 = HttpUtility.UrlDecode(item.UDF2);
                        objInventoryCountDetailDTO.UDF3 = HttpUtility.UrlDecode(item.UDF3);
                        objInventoryCountDetailDTO.UDF4 = HttpUtility.UrlDecode(item.UDF4);
                        objInventoryCountDetailDTO.UDF5 = HttpUtility.UrlDecode(item.UDF5);
                        objInventoryCountDetailDTO.IsOnlyFromItemUI = true;
                        objInventoryCountDetailDTO.AddedFrom = "Web";
                        objInventoryCountDetailDTO.EditedFrom = "Web";
                        objInventoryCountDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                        objInventoryCountDetailDTO.UpdatedByName = SessionHelper.UserName;
                        objInventoryCountDetailDTO.ProjectSpendGUID = item.ProjectSpendGUID;
                        objInventoryCountDAL.SaveInventoryCountLineItem(objInventoryCountDetailDTO);
                        message = ResMessage.SaveMessage;
                    }
                    else if (item.ItemType == 2)
                    {
                        List<QuickListDetailDTO> lstQlitems = objQuickListDAL.DB_GetQLLineItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null, null, (item.ItemGUID).ToString(), SessionHelper.UserSupplierIds);

                        foreach (QuickListDetailDTO item1 in lstQlitems)
                        {
                            if (item1 != null)
                            {

                                if (item1.QuickListType == 3)
                                {
                                    objInventoryCountDetailDTO.CountConsignedQuantity = item1.ConsignedQuantity * item.CustomerOwnedQuantity;
                                    objInventoryCountDetailDTO.CountCustomerOwnedQuantity = item1.Quantity * item.CustomerOwnedQuantity;
                                }
                                else
                                {
                                    objInventoryCountDetailDTO.CountCustomerOwnedQuantity = item1.Quantity * item.CustomerOwnedQuantity;
                                }
                            }
                            else
                            {
                                objInventoryCountDetailDTO.CountConsignedQuantity = item.ConsignedQuantity;
                                objInventoryCountDetailDTO.CountCustomerOwnedQuantity = item.CustomerOwnedQuantity;
                            }
                            BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(item1.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, item1.BinID ?? 0,null,null).FirstOrDefault();
                            if (objBinMasterDTO != null)
                            {
                                objInventoryCountDetailDTO.BinGUID = objBinMasterDTO.GUID;
                                objInventoryCountDetailDTO.BinID = objBinMasterDTO.ID;
                                objInventoryCountDetailDTO.BinNumber = objBinMasterDTO.BinNumber;
                            }
                            Guid? GetExistingGuid = Guid.Empty;
                            if (objBinMasterDTO != null)
                            {
                                objInventoryCountDAL.GetCountDetailGUIDByItemGUIDBinID(objInventoryCountDTO.GUID, item1.ItemGUID ?? Guid.Empty, objBinMasterDTO.ID, out GetExistingGuid);
                            }
                            if (GetExistingGuid == Guid.Empty)
                            {
                                objInventoryCountDetailDTO.CountDate = objInventoryCountDTO.CountDate;
                                objInventoryCountDetailDTO.CountItemStatus = string.Empty;
                                objInventoryCountDetailDTO.CountLineItemDescription = HttpUtility.UrlDecode(item.CountLineItemDescription);
                                objInventoryCountDetailDTO.CountName = objInventoryCountDTO.CountName;
                                objInventoryCountDetailDTO.CountQuantity = 0;
                                objInventoryCountDetailDTO.CountStatus = objInventoryCountDTO.CountStatus;
                                objInventoryCountDetailDTO.CountType = objInventoryCountDTO.CountType;
                                objInventoryCountDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                objInventoryCountDetailDTO.CreatedBy = SessionHelper.UserID;
                                objInventoryCountDetailDTO.CreatedByName = SessionHelper.UserName;
                                objInventoryCountDetailDTO.CustomerOwnedQuantity = 0;
                                objInventoryCountDetailDTO.GUID = Guid.NewGuid();
                                objInventoryCountDetailDTO.ID = 0;
                                objInventoryCountDetailDTO.InventoryCountGUID = objInventoryCountDTO.GUID;
                                objInventoryCountDetailDTO.IsStagingLocationCount = item.IsStagingLocationCount;
                                objInventoryCountDetailDTO.IsApplied = false;
                                objInventoryCountDetailDTO.IsArchived = false;
                                objInventoryCountDetailDTO.IsClosed = false;
                                objInventoryCountDetailDTO.IsDeleted = false;
                                objInventoryCountDetailDTO.ItemGUID = item1.ItemGUID ?? Guid.Empty;
                                objInventoryCountDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                                objInventoryCountDetailDTO.RoomId = SessionHelper.RoomID;
                                objInventoryCountDetailDTO.RoomName = SessionHelper.RoomName;
                                objInventoryCountDetailDTO.UDF1 = HttpUtility.UrlDecode(item.UDF1);
                                objInventoryCountDetailDTO.UDF2 = HttpUtility.UrlDecode(item.UDF2);
                                objInventoryCountDetailDTO.UDF3 = HttpUtility.UrlDecode(item.UDF3);
                                objInventoryCountDetailDTO.UDF4 = HttpUtility.UrlDecode(item.UDF4);
                                objInventoryCountDetailDTO.UDF5 = HttpUtility.UrlDecode(item.UDF5);
                                objInventoryCountDetailDTO.IsOnlyFromItemUI = true;
                                objInventoryCountDetailDTO.AddedFrom = "Web";
                                objInventoryCountDetailDTO.EditedFrom = "Web";
                                objInventoryCountDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objInventoryCountDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objInventoryCountDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                objInventoryCountDetailDTO.UpdatedByName = SessionHelper.UserName;
                                objInventoryCountDetailDTO.ProjectSpendGUID = item.ProjectSpendGUID;
                                objInventoryCountDAL.SaveInventoryCountLineItem(objInventoryCountDetailDTO);
                            }
                            message = ResMessage.SaveMessage;
                        }
                    }

                    CheckUnappliedLineItem(objInventoryCountDTO.GUID);
                    status = "ok";
                }
            }
            catch
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }

            return Json(new { Message = message, Status = status });
        }

        public PartialViewResult GetCountLineItemspartial(Guid? CountGUID)
        {
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            objInventoryCountDTO = objInventoryCountDAL.GetInventoryCountByGUId(CountGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
            objInventoryCountDTO.IsApplyAllDisable = objInventoryCountDAL.CheckLineItems(SessionHelper.RoomID, SessionHelper.CompanyID, CountGUID ?? Guid.Empty);
            return PartialView("CountLineItemsNew", objInventoryCountDTO);
        }

        [HttpPost]
        public ActionResult ApplyCountOnLineItemsSelected(List<InventoryCountDetailDTO> lstLineItems)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();
            string message = string.Empty, status = string.Empty;
            if (lstLineItems != null && lstLineItems.Count > 0)
            {
                foreach (var item in lstLineItems)
                {
                    try
                    {
                        objInventoryCountDetailDTO = objInventoryCountDAL.BeforeApplyAction(lstLineItems.First(), SessionHelper.UserID);



                        message = ResMessage.SaveMessage;

                        if (objInventoryCountDetailDTO.TotalDifference < 0)
                        {
                            status = "pull";
                        }
                        else if (objInventoryCountDetailDTO.TotalDifference > 0)
                        {
                            if (objInventoryCountDetailDTO.IsStagingLocationCount)
                            {
                                status = "creditms";
                            }
                            else
                            {
                                status = "credit";
                            }
                        }
                        else
                        {
                            status = "ok";
                        }
                    }
                    catch
                    {
                        message = ResMessage.SaveErrorMsg;
                        status = "fail";
                    }
                }
                return Json(new { Message = message, Status = status, CurrentObj = objInventoryCountDetailDTO });
            }
            else
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
                return Json(new { Message = message, Status = status, CurrentObj = lstLineItems.First() });
            }
        }



        [HttpPost]
        public ActionResult OpenApplyCountOnLineItemsPopup(List<InventoryCountDetailDTO> lstLineItems)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);

            foreach (InventoryCountDetailDTO InventoryCount in lstLineItems)
            {
                InventoryCountDetailDTO objInventoryCountDetailDTO = objInventoryCountDAL.BeforeApplyAction(InventoryCount, SessionHelper.UserID);

                ItemMasterDAL objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                objItemMaster.EditDate(InventoryCount.ItemGUID, "EditCountedDate");
            }

            return PartialView("CountApplyLotSrSelection", lstLineItems);
        }

        public ActionResult LotSrSelectionForCountPull(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            long BinID = 0;
            double CustomerOwnedPullQTY = 0;
            double ConsignedPullQTY = 0;
            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["CustomerOwnedPullQTY"]), out CustomerOwnedPullQTY);
            double.TryParse(Convert.ToString(Request["ConsignedPullQTY"]), out ConsignedPullQTY);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ItemMasterDTO oItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID);

            int TotalRecordCount = 0;
            InventoryCountDAL oInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, string> dicSerialLots = new Dictionary<string, string>();
            CultureInfo RoomCulture = SessionHelper.RoomCulture;
            if (arrIds.Count() > 0)
            {
                string[] arrSerialLots = new string[arrIds.Count()];
                for (int i = 0; i < arrIds.Count(); i++)
                {
                    string[] arrItem = arrIds[i].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 1)
                    {
                        arrSerialLots[i] = arrItem[0];
                        dicSerialLots.Add(arrItem[0], arrItem[1]);
                    }
                }

                lstLotSrs = oInventoryCountDAL.GetItemLocationsWithLotSerialsForCountPull(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, CustomerOwnedPullQTY, ConsignedPullQTY, "0", string.Empty, RoomCulture);
                retlstLotSrs = lstLotSrs.Where(t => ((arrSerialLots.Contains(t.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    || (arrSerialLots.Contains(t.Expiration) && oItem.DateCodeTracking)
                    || (arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).ToList();

                if (!IsDeleteRowMode)
                {
                    if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.BinNumber = string.Empty;
                        retlstLotSrs.Add(oLotSr);
                    }
                    else
                        retlstLotSrs = retlstLotSrs.Union(lstLotSrs.Where(t =>
                            ((!arrSerialLots.Contains(t.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                        || (!arrSerialLots.Contains(t.Expiration) && oItem.DateCodeTracking)
                        || (!arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).Take(1)).ToList();
                }
            }
            else
            {
                if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                {
                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.LotOrSerailNumber = string.Empty;
                    oLotSr.Expiration = string.Empty;
                    oLotSr.BinNumber = string.Empty;
                    retlstLotSrs.Add(oLotSr);
                }
                else
                    retlstLotSrs = oInventoryCountDAL.GetItemLocationsWithLotSerialsForCountPull(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, CustomerOwnedPullQTY, ConsignedPullQTY, "1", string.Empty, RoomCulture);
            }

            foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
            {
                if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                {
                    string value = dicSerialLots[item.LotOrSerailNumber];
                    string[] arrItem = value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 1)
                    {
                        item.CustomerOwnedTobePulled = Convert.ToDouble(arrItem[0]);
                        item.ConsignedTobePulled = Convert.ToDouble(arrItem[1]);
                    }

                    CustomerOwnedPullQTY -= item.CustomerOwnedTobePulled;
                    ConsignedPullQTY -= item.ConsignedTobePulled;
                }
                else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && oItem.DateCodeTracking)
                {
                    string value = dicSerialLots[item.Expiration];
                    string[] arrItem = value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 1)
                    {
                        item.CustomerOwnedTobePulled = Convert.ToDouble(arrItem[0]);
                        item.ConsignedTobePulled = Convert.ToDouble(arrItem[1]);
                    }

                    CustomerOwnedPullQTY -= item.CustomerOwnedTobePulled;
                    ConsignedPullQTY -= item.ConsignedTobePulled;
                }
                else if (dicSerialLots.ContainsKey(item.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking)
                {
                    string value = dicSerialLots[item.BinNumber];
                    string[] arrItem = value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 1)
                    {
                        item.CustomerOwnedTobePulled = Convert.ToDouble(arrItem[0]);
                        item.ConsignedTobePulled = Convert.ToDouble(arrItem[1]);
                    }

                    CustomerOwnedPullQTY -= item.CustomerOwnedTobePulled;
                    ConsignedPullQTY -= item.ConsignedTobePulled;
                }
                else
                {
                    if (item.CustomerOwnedTobePulled <= CustomerOwnedPullQTY)
                    {
                        CustomerOwnedPullQTY -= item.CustomerOwnedTobePulled;
                    }
                    else if (CustomerOwnedPullQTY >= 0)
                    {
                        item.CustomerOwnedTobePulled = CustomerOwnedPullQTY;
                        CustomerOwnedPullQTY = 0;
                    }
                    else
                        item.CustomerOwnedTobePulled = 0;

                    if (item.ConsignedTobePulled <= ConsignedPullQTY)
                    {
                        ConsignedPullQTY -= item.ConsignedTobePulled;
                    }
                    else if (ConsignedPullQTY >= 0)
                    {
                        item.ConsignedTobePulled = ConsignedPullQTY;
                        ConsignedPullQTY = 0;
                    }
                    else
                        item.ConsignedTobePulled = 0;
                }
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLotOrSerailNumberList(int maxRows, string name_startsWith, Guid? ItemGuid, long BinID)
        {
            CultureInfo RoomCulture = SessionHelper.RoomCulture;
            InventoryCountDAL objPullDetails = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForCountPull(ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, 0, "0", String.Empty, RoomCulture);

            var lstLotSr =
                objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith)).Select(x => new { x.LotOrSerailNumber }).Distinct();

            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSerialLotNumberForPullCount(Guid? ItemGuid, string SerialOrLotNumber, long BinID)
        {
            CultureInfo RoomCulture = SessionHelper.RoomCulture;
            InventoryCountDAL objPullDetails = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            ItemLocationLotSerialDTO objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForCountPull(ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, 0, "0", SerialOrLotNumber, RoomCulture).FirstOrDefault();

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = new ItemLocationLotSerialDTO();
                objItemLocationLotSerialDTO.BinID = BinID;
                objItemLocationLotSerialDTO.ID = BinID;
                objItemLocationLotSerialDTO.ItemGUID = ItemGuid;
                objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                objItemLocationLotSerialDTO.Expiration = string.Empty;
                objItemLocationLotSerialDTO.BinNumber = string.Empty;
            }
            return Json(objItemLocationLotSerialDTO);
        }

        public ActionResult LotSrSelectionForCountCredit(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            long BinID = 0;
            double CustomerOwnedCreditQTY = 0;
            double ConsignedCreditQTY = 0;
            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["CustomerOwnedCreditQTY"]), out CustomerOwnedCreditQTY);
            double.TryParse(Convert.ToString(Request["ConsignedCreditQTY"]), out ConsignedCreditQTY);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            string BinNumber = Convert.ToString(Request["BinNumber"]);

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ItemMasterDTO oItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID);

            int TotalRecordCount = 0;
            InventoryCountDAL oInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, string> dicSerialLots = new Dictionary<string, string>();
            if (arrIds.Count() > 0)
            {
                string[] arrSerialLots = new string[arrIds.Count()];
                for (int i = 0; i < arrIds.Count(); i++)
                {
                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    string[] arrItem = arrIds[i].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 1)
                    {
                        arrSerialLots[i] = arrItem[0];
                        dicSerialLots.Add(arrItem[0], arrItem[1]);

                        if (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                            oLotSr.LotOrSerailNumber = arrItem[0];
                        else
                            oLotSr.LotOrSerailNumber = string.Empty;
                        if (oItem.DateCodeTracking)
                            oLotSr.Expiration = arrItem[0];
                        else
                            oLotSr.Expiration = string.Empty;


                        string[] arrQtys = arrItem[1].Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrQtys.Length > 1)
                        {
                            oLotSr.CustomerOwnedQuantity = Convert.ToDouble(arrQtys[0]);
                            oLotSr.ConsignedQuantity = Convert.ToDouble(arrQtys[1]);
                        }
                    }
                    else
                    {
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;

                        string[] arrQtys = arrItem[0].Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrQtys.Length > 1)
                        {
                            oLotSr.CustomerOwnedQuantity = Convert.ToDouble(arrQtys[0]);
                            oLotSr.ConsignedQuantity = Convert.ToDouble(arrQtys[1]);
                        }
                    }

                    if (oItem.SerialNumberTracking)
                    {
                        oLotSr.ConsignedQuantity = 1;
                        oLotSr.CustomerOwnedQuantity = 1;
                    }

                    oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.IsCreditPull = false;
                    oLotSr.BinNumber = BinNumber;
                    oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                    oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                    oLotSr.LotNumberTracking = oItem.LotNumberTracking;

                    //Received = !il.ReceivedDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true), (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"]).ToString("MM/dd/yyyy"),
                    //ReceivedDate = il.ReceivedDate,
                    //IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,

                    retlstLotSrs.Add(oLotSr);
                }

                if (!IsDeleteRowMode)
                {
                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.IsCreditPull = false;
                    oLotSr.BinNumber = BinNumber;
                    oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                    oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                    oLotSr.LotNumberTracking = oItem.LotNumberTracking;

                    if (oItem.SerialNumberTracking)
                    {
                        oLotSr.ConsignedQuantity = 1;
                        oLotSr.CustomerOwnedQuantity = 1;
                    }
                    else
                    {
                        oLotSr.ConsignedQuantity = 0;
                        oLotSr.CustomerOwnedQuantity = 0;
                    }
                    //Received = !il.ReceivedDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true), (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"]).ToString("MM/dd/yyyy"),
                    //ReceivedDate = il.ReceivedDate,
                    //IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,

                    oLotSr.LotOrSerailNumber = string.Empty;
                    oLotSr.Expiration = string.Empty;

                    retlstLotSrs.Add(oLotSr);
                }
            }
            else
            {
                if (oItem.SerialNumberTracking)
                {
                    double maxQty = ConsignedCreditQTY > CustomerOwnedCreditQTY ? ConsignedCreditQTY : CustomerOwnedCreditQTY;

                    for (double i = 1; i <= maxQty; i++)
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.IsCreditPull = false;
                        oLotSr.BinNumber = BinNumber;
                        oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                        oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                        oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.ConsignedQuantity = 1;
                        oLotSr.CustomerOwnedQuantity = 1;

                        //Received = !il.ReceivedDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true), (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"]).ToString("MM/dd/yyyy"),
                        //ReceivedDate = il.ReceivedDate,
                        //IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,

                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;

                        retlstLotSrs.Add(oLotSr);
                    }
                }
                else
                {
                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.IsCreditPull = false;
                    oLotSr.BinNumber = BinNumber;
                    oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                    oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                    oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                    oLotSr.ConsignedQuantity = ConsignedCreditQTY;
                    oLotSr.CustomerOwnedQuantity = CustomerOwnedCreditQTY;

                    //Received = !il.ReceivedDate.HasValue ? "" : Convert.ToDateTime(FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true), (CultureInfo)System.Web.HttpContext.Current.Session["RoomCulture"]).ToString("MM/dd/yyyy"),
                    //ReceivedDate = il.ReceivedDate,
                    //IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,

                    oLotSr.LotOrSerailNumber = string.Empty;
                    oLotSr.Expiration = string.Empty;

                    retlstLotSrs.Add(oLotSr);
                }
            }

            foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
            {
                if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                {
                    string value = dicSerialLots[item.LotOrSerailNumber];
                    string[] arrItem = value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 1)
                    {
                        item.CustomerOwnedQuantity = Convert.ToDouble(arrItem[0]);
                        item.ConsignedQuantity = Convert.ToDouble(arrItem[1]);
                    }

                    CustomerOwnedCreditQTY -= item.CustomerOwnedQuantity.Value;
                    ConsignedCreditQTY -= item.ConsignedQuantity.Value;
                }
                else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && oItem.DateCodeTracking)
                {
                    string value = dicSerialLots[item.Expiration];
                    string[] arrItem = value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 1)
                    {
                        item.CustomerOwnedQuantity = Convert.ToDouble(arrItem[0]);
                        item.ConsignedQuantity = Convert.ToDouble(arrItem[1]);
                    }

                    CustomerOwnedCreditQTY -= item.CustomerOwnedQuantity.Value;
                    ConsignedCreditQTY -= item.ConsignedQuantity.Value;
                }
                else if (dicSerialLots.ContainsKey(item.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking)
                {
                    string value = dicSerialLots[item.BinNumber];
                    string[] arrItem = value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 1)
                    {
                        item.CustomerOwnedQuantity = Convert.ToDouble(arrItem[0]);
                        item.ConsignedQuantity = Convert.ToDouble(arrItem[1]);
                    }

                    CustomerOwnedCreditQTY -= item.CustomerOwnedQuantity.Value;
                    ConsignedCreditQTY -= item.ConsignedQuantity.Value;
                }
                else
                {
                    if (item.CustomerOwnedQuantity <= CustomerOwnedCreditQTY)
                    {
                        CustomerOwnedCreditQTY -= item.CustomerOwnedQuantity.Value;
                    }
                    else if (CustomerOwnedCreditQTY >= 0)
                    {
                        item.CustomerOwnedQuantity = CustomerOwnedCreditQTY;
                        CustomerOwnedCreditQTY = 0;
                    }
                    else
                        item.CustomerOwnedQuantity = 0;

                    if (item.ConsignedQuantity <= ConsignedCreditQTY)
                    {
                        ConsignedCreditQTY -= item.ConsignedQuantity.Value;
                    }
                    else if (ConsignedCreditQTY >= 0)
                    {
                        item.ConsignedQuantity = ConsignedCreditQTY;
                        ConsignedCreditQTY = 0;
                    }
                    else
                        item.ConsignedQuantity = 0;
                }
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [Cycle Count Setup]
        [AjaxOrChildActionOnlyAttribute]
        public ActionResult InventoryCountSetUp()
        {
            CycleCountSettingDAL objCycleCountSettingDAL = new CycleCountSettingDAL(SessionHelper.EnterPriseDBName);
            CycleCountSettingDTO objCycleCountSettingDTO = objCycleCountSettingDAL.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID);
            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            RoomDTO objRoomInfo = new RoomDTO();
            // objRoomInfo = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            string columnList = "ID,RoomName,DefaultCountType";
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objRoomInfo = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (objCycleCountSettingDTO == null)
            {
                objCycleCountSettingDTO = new CycleCountSettingDTO();
                objCycleCountSettingDTO.YearStartDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 1, 1);
                objCycleCountSettingDTO.YearEndDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 12, 31);


                objCycleCountSettingDTO.YearStartDay = 1;
                objCycleCountSettingDTO.YearStartMonth = 1;
                objCycleCountSettingDTO.YearEndDay = 31;
                objCycleCountSettingDTO.YearEndMonth = 12;
                objCycleCountSettingDTO.RecurrringDays = 1;
                objCycleCountSettingDTO.CycleCountTimestr = "00:01";
                objCycleCountSettingDTO.CountFrequencyType = 1;
                objCycleCountSettingDTO.IsActive = true;
                objCycleCountSettingDTO.RecurringType = 1;
                objCycleCountSettingDTO.MissedItemsEmailTimestr = "20:00";
                objCycleCountSettingDTO.MissedItemEmailPriorHours = 3;

            }
            else
            {
                objCycleCountSettingDTO.CycleCountTimestr = objCycleCountSettingDTO.CycleCountTime.ToString(@"hh\:mm");
                objCycleCountSettingDTO.MissedItemsEmailTimestr = objCycleCountSettingDTO.MissedItemsEmailTime.ToString(@"hh\:mm");
                objCycleCountSettingDTO.YearStartDay = objCycleCountSettingDTO.YearStartDate.Day;
                objCycleCountSettingDTO.YearStartMonth = objCycleCountSettingDTO.YearStartDate.Month;
                objCycleCountSettingDTO.YearEndDay = objCycleCountSettingDTO.YearEndDate.Day;
                objCycleCountSettingDTO.YearEndMonth = objCycleCountSettingDTO.YearEndDate.Month;
                //if (objCycleCountSettingDTO.NextRunDate != null)
                //{
                //    objCycleCountSettingDTO.NextRunDatestr = FnCommon.ConvertDateByTimeZone(objCycleCountSettingDTO.NextRunDate, true);
                //    DateTime dt = Convert.ToDateTime(objCycleCountSettingDTO.NextRunDatestr);
                //    dt = dt.Date.AddMinutes(1);
                //    objCycleCountSettingDTO.NextRunDatestr = dt.ToString();
                //}

                //objCycleCountSettingDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objCycleCountSettingDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                //objCycleCountSettingDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objCycleCountSettingDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }
            objCycleCountSettingDTO.YearEndDateStr = objCycleCountSettingDTO.YearEndDate.ToString(SessionHelper.RoomDateFormat);
            objCycleCountSettingDTO.YearStartDateStr = objCycleCountSettingDTO.YearStartDate.ToString(SessionHelper.RoomDateFormat);
            objCycleCountSettingDTO.lstClassifications = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName).GetInventoryClassificationByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID, false).ToList();

            if (string.IsNullOrWhiteSpace(objCycleCountSettingDTO.CountType))
            {
                objCycleCountSettingDTO.CountType = Convert.ToChar(InventoryCountType.Adjustment).ToString();
                if (objRoomInfo != null)
                    objCycleCountSettingDTO.CountType = objRoomInfo.DefaultCountType;
            }
            ViewBag.CountTypeBag = GetCountTypeOptions(objCycleCountSettingDTO.CountType);

            CycleCountSetUpDTO objCycleCountSetUpDTO = new CycleCountSetUpDTO();
            List<CycleCountSetUpDTO> lstClass = new List<CycleCountSetUpDTO>();
            lstClass = objCycleCountSettingDAL.GetCycleCountSetUpByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID);
            objCycleCountSettingDTO.lstCycleCountSetup = lstClass;
            return PartialView("_InventoryCountSetUp", objCycleCountSettingDTO);
        }


        public ActionResult InventoryCountSetUp1()
        {
            CycleCountSettingDAL objCycleCountSettingDAL = new CycleCountSettingDAL(SessionHelper.EnterPriseDBName);
            CycleCountSettingDTO objCycleCountSettingDTO = objCycleCountSettingDAL.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objCycleCountSettingDTO == null)
            {
                objCycleCountSettingDTO = new CycleCountSettingDTO();
                objCycleCountSettingDTO.YearStartDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 1, 1);
                objCycleCountSettingDTO.YearEndDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 12, 31);
            }
            else
            {
                objCycleCountSettingDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objCycleCountSettingDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objCycleCountSettingDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objCycleCountSettingDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }
            objCycleCountSettingDTO.YearEndDateStr = objCycleCountSettingDTO.YearEndDate.ToString(SessionHelper.RoomDateFormat);
            objCycleCountSettingDTO.YearStartDateStr = objCycleCountSettingDTO.YearStartDate.ToString(SessionHelper.RoomDateFormat);
            objCycleCountSettingDTO.lstClassifications = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName).GetInventoryClassificationByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID, false).ToList();
            return PartialView("_InventoryCountSetUp", objCycleCountSettingDTO);
        }

        [HttpPost]
        public ActionResult SaveInventoryCountSettings(CycleCountSettingDTO objCycleCountSettingDTO)
        {
            string message = "";
            string status = "";
            try
            {
                if (string.IsNullOrWhiteSpace(objCycleCountSettingDTO.CycleCountTimestr))
                {
                    objCycleCountSettingDTO.CycleCountTimestr = "00:01";
                }
                objCycleCountSettingDTO.CycleCountTime = Convert.ToDateTime(objCycleCountSettingDTO.CycleCountTimestr).TimeOfDay;
                DateTime YearStartDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 1, 1);
                DateTime YearEndDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 12, 31);


                DateTime.TryParseExact(objCycleCountSettingDTO.YearStartDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out YearStartDate);
                DateTime.TryParseExact(objCycleCountSettingDTO.YearEndDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out YearEndDate);
                if (YearStartDate != DateTime.MinValue)
                {
                    objCycleCountSettingDTO.YearStartDate = YearStartDate;
                }
                else
                {
                    objCycleCountSettingDTO.YearStartDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 1, 1);
                }
                if (YearEndDate != DateTime.MinValue)
                {
                    objCycleCountSettingDTO.YearEndDate = YearEndDate;
                }
                else
                {
                    objCycleCountSettingDTO.YearEndDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 12, 31);
                }

                CycleCountSettingDAL objCycleCountSettingDAL = new CycleCountSettingDAL(SessionHelper.EnterPriseDBName);

                if (objCycleCountSettingDTO.ID < 1)
                {
                    objCycleCountSettingDTO.CompanyId = SessionHelper.CompanyID;
                    objCycleCountSettingDTO.Created = DateTimeUtility.DateTimeNow;
                    objCycleCountSettingDTO.CreatedBy = SessionHelper.UserID;
                    objCycleCountSettingDTO.CreatedByName = SessionHelper.UserName;
                    objCycleCountSettingDTO.GUID = Guid.NewGuid();
                    objCycleCountSettingDTO.ID = 0;
                    objCycleCountSettingDTO.IsArchived = false;
                    objCycleCountSettingDTO.IsDeleted = false;
                    objCycleCountSettingDTO.LastUpdatedBy = SessionHelper.UserID;
                    objCycleCountSettingDTO.RoomId = SessionHelper.RoomID;
                    objCycleCountSettingDTO.RoomName = SessionHelper.RoomName;
                    objCycleCountSettingDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCycleCountSettingDTO.UpdatedByName = SessionHelper.UserName;
                }
                else
                {
                    objCycleCountSettingDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCycleCountSettingDTO.LastUpdatedBy = SessionHelper.UserID;
                }
                objCycleCountSettingDTO = objCycleCountSettingDAL.SaveInventoryCountSettings(objCycleCountSettingDTO);
                if (objCycleCountSettingDTO.ID > 0)
                {
                    message = ResMessage.SaveMessage;
                    status = "ok";
                }
                else
                {
                    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                    status = "fail";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "fail";
            }
            return Json(new { Message = message, Status = status, UpdatedDTO = objCycleCountSettingDTO });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveInventoryCountSetUp(CycleCountSettingDTO objCycleCountSettingDTO)
        {
            string message = "";
            string status = "";
            try
            {
                if (string.IsNullOrWhiteSpace(objCycleCountSettingDTO.CycleCountTimestr))
                {
                    objCycleCountSettingDTO.CycleCountTimestr = "00:01";
                }
                objCycleCountSettingDTO.CycleCountTime = Convert.ToDateTime(objCycleCountSettingDTO.CycleCountTimestr).TimeOfDay;


                if (string.IsNullOrWhiteSpace(objCycleCountSettingDTO.MissedItemsEmailTimestr))
                {
                    objCycleCountSettingDTO.MissedItemsEmailTimestr = "20:00";
                }
                objCycleCountSettingDTO.MissedItemsEmailTime = Convert.ToDateTime(objCycleCountSettingDTO.MissedItemsEmailTimestr).TimeOfDay;

                DateTime YearStartDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 1, 1);
                DateTime YearEndDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 12, 31);


                DateTime.TryParseExact(objCycleCountSettingDTO.YearStartDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out YearStartDate);
                DateTime.TryParseExact(objCycleCountSettingDTO.YearEndDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out YearEndDate);
                if (YearStartDate != DateTime.MinValue)
                {
                    objCycleCountSettingDTO.YearStartDate = YearStartDate;
                }
                else
                {
                    objCycleCountSettingDTO.YearStartDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 1, 1);
                }
                if (YearEndDate != DateTime.MinValue)
                {
                    objCycleCountSettingDTO.YearEndDate = YearEndDate;
                }
                else
                {
                    objCycleCountSettingDTO.YearEndDate = new DateTime(DateTimeUtility.DateTimeNow.Year, 12, 31);
                }
                if (objCycleCountSettingDTO.YearEndDate < objCycleCountSettingDTO.YearStartDate)
                {
                    message = ResCommon.EndDateShouldGreaterThanStartDate;
                    status = "fail";
                }
                if (status != "fail")
                {
                    if ((objCycleCountSettingDTO.YearEndDate - objCycleCountSettingDTO.YearStartDate).TotalDays < 364 || (objCycleCountSettingDTO.YearEndDate - objCycleCountSettingDTO.YearStartDate).TotalDays > 365)
                    {
                        message = ResInventoryCount.YearShouldHave365Days;
                        status = "fail";
                    }
                }
                if (status != "fail")
                {
                    CycleCountSettingDAL objCycleCountSettingDAL = new CycleCountSettingDAL(SessionHelper.EnterPriseDBName);

                    if (objCycleCountSettingDTO.ID < 1)
                    {
                        objCycleCountSettingDTO.CompanyId = SessionHelper.CompanyID;
                        objCycleCountSettingDTO.Created = DateTimeUtility.DateTimeNow;
                        objCycleCountSettingDTO.CreatedBy = SessionHelper.UserID;
                        objCycleCountSettingDTO.CreatedByName = SessionHelper.UserName;
                        objCycleCountSettingDTO.GUID = Guid.NewGuid();
                        objCycleCountSettingDTO.ID = 0;
                        objCycleCountSettingDTO.IsArchived = false;
                        objCycleCountSettingDTO.IsDeleted = false;
                        objCycleCountSettingDTO.LastUpdatedBy = SessionHelper.UserID;
                        objCycleCountSettingDTO.RoomId = SessionHelper.RoomID;
                        objCycleCountSettingDTO.RoomName = SessionHelper.RoomName;
                        objCycleCountSettingDTO.Updated = DateTimeUtility.DateTimeNow;
                        objCycleCountSettingDTO.UpdatedByName = SessionHelper.UserName;
                    }
                    else
                    {
                        objCycleCountSettingDTO.Updated = DateTimeUtility.DateTimeNow;
                        objCycleCountSettingDTO.LastUpdatedBy = SessionHelper.UserID;
                    }
                    objCycleCountSettingDTO = objCycleCountSettingDAL.SaveInventoryCountSettings(objCycleCountSettingDTO);
                    CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.InvalidateCache();
                    if (objCycleCountSettingDTO.ID > 0)
                    {
                        message = ResMessage.SaveMessage;
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "fail";
            }



            return Json(new { Message = message, Status = status });
        }

        public ActionResult GenerateAutomatedCount(string countdate)
        {
            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                if (!string.IsNullOrWhiteSpace(countdate))
                {
                    string GenCount = "EXEC [dbo].[AutomatedCycleCount] '" + countdate + "'";
                    context.Database.ExecuteSqlCommand(GenCount);
                }
                else
                {
                    string GenCount = "EXEC [dbo].[AutomatedCycleCount] '" + DateTimeUtility.DateTimeNow.Date.ToString() + "'";
                    context.Database.ExecuteSqlCommand(GenCount);
                }

            }
            return RedirectToAction("InventoryCountList", "Inventory");
        }

        #endregion

        #region [Inventory count history]

        public ActionResult InventoryCountListHistory(Guid? GUID)
        {
            ViewBag.SelectedGUID = GUID ?? Guid.Empty;
            return PartialView("InventoryCountListHistory");
        }

        public ActionResult InventoryCountHistoryView(long? ID)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = null;
            if (ID.HasValue)
            {
                objInventoryCountDTO = objInventoryCountDAL.GetHistoryRecord(ID ?? 0);
                if (objInventoryCountDTO == null)
                {
                    objInventoryCountDTO = new InventoryCountDTO();
                }
            }
            else
            {
                objInventoryCountDTO = new InventoryCountDTO();
            }
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("InventoryCount");

            ViewData["UDF1"] = objInventoryCountDTO.UDF1;
            ViewData["UDF2"] = objInventoryCountDTO.UDF2;
            ViewData["UDF3"] = objInventoryCountDTO.UDF3;
            ViewData["UDF4"] = objInventoryCountDTO.UDF4;
            ViewData["UDF5"] = objInventoryCountDTO.UDF5;
            return PartialView("_InventoryCountDetails_History", objInventoryCountDTO);
        }

        public PartialViewResult GetCountLineItemsHistory(long? HistoryID)
        {
            InventoryCountDTO objDTO = new InventoryCountDTO();
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            objDTO = objInventoryCountDAL.GetHistoryRecord(HistoryID ?? 0);
            if (objDTO != null)
                objDTO.CountLineItemsList = objInventoryCountDAL.GetHistoryRecordbyCountGUID(objDTO.GUID).ToList();

            return PartialView("CountLineItems_History", objDTO);
        }

        #endregion

        #region SendMailForApplyCount
        [HttpPost]
        public void SendMailForApplyCount(List<InventoryCountDetailDTO> lstInventoryCountDetails)
        {
            if (lstInventoryCountDetails != null && lstInventoryCountDetails.Count > 0)
            {
                InventoryCountDAL inventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
                inventoryCountDAL.SendMailForApplyCount(lstInventoryCountDetails
                                    , SessionHelper.EnterPriseDBName,SessionHelper.EnterPriceID
                                    , SessionHelper.CompanyName, SessionHelper.CompanyID
                                    , SessionHelper.RoomName, SessionHelper.RoomID
                                    , SessionHelper.UserName);
            }
        }

        //private string GetMailBodyForCountApply(List<InventoryCountDetailDTO> LstCountDtl, NotificationDTO objNotification)
        //{

        //    string htmlTabl = string.Empty;
        //    htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0"" cellspacing=""0"">
        //                    <thead>
        //                        <tr role=""row"">
        //                            <th  style=""width: 10%; text-align: left;"">
        //                                " + ResItemMaster.ItemNumber + @"
        //                            </th>
        //                            <th  style=""width: 10%; text-align: left;"">
        //                                " + ResInventoryCountDetail.BinNumber + @"
        //                            </th>
        //                            <th  style=""width: 10%; text-align: left;"">
        //                                " + ResInventoryCountDetail.ConsignedDifference + @"
        //                            </th>
        //                            <th  style=""width: 10%; text-align: left;"">
        //                                " + ResInventoryCountDetail.CusOwnedDifference + @"
        //                            </th>
        //                            <th  style=""width: 10%; text-align: left;"">
        //                                " + ResInventoryCountDetail.IsApplied + @"
        //                            </th>
        //                            <th  style=""width: 10%; text-align: left;"">
        //                                " + ResInventoryCountDetail.AppliedDate + @"
        //                            </th>
        //                        </tr>
        //                    </thead>
        //                    <tbody>
        //                    ##TRS##
        //                    </tbody>
        //                </table>";

        //    string trs = "";
        //    //List<InventoryCountDetailDTO> LstCountDtl = new List<InventoryCountDetailDTO>();
        //    //InventoryCountDAL objCountDal = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
        //    //LstCountDtl = objCountDal.GetAllLineItemsWithinCount(objInventoryCount.ID);

        //    if (LstCountDtl != null && LstCountDtl.Count() > 0)
        //    {
        //        string QtyFormat = "N";
        //        string dateFormate = "MM/dd/yyyy";


        //        if (objNotification.objeTurnsRegionInfo != null && objNotification.objeTurnsRegionInfo.NumberDecimalDigits > 0)
        //            QtyFormat += objNotification.objeTurnsRegionInfo.NumberDecimalDigits;

        //        if (objNotification.objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objNotification.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(objNotification.objeTurnsRegionInfo.ShortTimePattern))
        //            dateFormate = objNotification.objeTurnsRegionInfo.ShortDatePattern + " " + objNotification.objeTurnsRegionInfo.ShortTimePattern;

        //        int cntrow = 1;
        //        foreach (var item in LstCountDtl)
        //        {
        //            string RowStyle = string.Empty;
        //            if (cntrow % 2 == 0)
        //            {
        //                RowStyle = AlterNativeRowStyle;
        //            }
        //            trs += @"<tr " + RowStyle + @" >
        //                       <td>
        //                           " + item.ItemNumber + @"
        //                       </td>
        //                       <td>
        //                           " + (string.IsNullOrEmpty(item.BinNumber) ? "&nbsp;" : item.BinNumber) + @"
        //                       </td>
        //                       <td>
        //                           " + (item.ConsignedDifference.HasValue ? item.ConsignedDifference.Value.ToString(QtyFormat) : "NA") + @"
        //                       </td>
        //                       <td>
        //                           " + (item.CusOwnedDifference.HasValue ? item.CusOwnedDifference.Value.ToString(QtyFormat) : "NA") + @"
        //                       </td>
        //                        <td>
        //                           " + (item.IsApplied ? "True" : "False") + @"
        //                       </td>
        //                       <td>
        //                            " + (item.AppliedDate.GetValueOrDefault(DateTime.MinValue).ToString(dateFormate)) + @"
        //                       </td>
        //                     </tr>";
        //            cntrow += 1;
        //        }
        //    }
        //    else
        //    {
        //        //if Not Data Found 
        //        string Str = "No Data found";
        //        string RowStyle = string.Empty;
        //        trs += @"<tr " + RowStyle + @" >
        //                <td colspan=7>
        //                    " + Str + @"
        //                </td>
        //           </tr>";
        //    }

        //    htmlTabl = htmlTabl.Replace("##TRS##", trs);
        //    return htmlTabl;
        //}

        #endregion
        public void UnApplieIDbyID(Int64 Id, bool IsApplied)
        {
            InventoryCountDetailDAL objInventoryCountDetailDAL = new InventoryCountDetailDAL(SessionHelper.EnterPriseDBName);
            objInventoryCountDetailDAL.IsAppliedInventoryDetailById(Id, IsApplied);
        }
        public JsonResult CheckUnappliedLineItem(Guid? CountLineItem)
        {
            InventoryCountDAL objInventoryCountDetailDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            bool Result = objInventoryCountDetailDAL.CheckLineItems(SessionHelper.RoomID, SessionHelper.CompanyID, CountLineItem ?? Guid.Empty);
            if (Result)
            {
                objInventoryCountDetailDAL.SetAppliedFalse(SessionHelper.RoomID, SessionHelper.CompanyID, CountLineItem ?? Guid.Empty, false);
            }
            return Json(new { Message = "ok", Result = Result });
        }


        [HttpPost]
        public ActionResult ApplyOnCountLineItem(InventoryCountDetailDTO lstLineItems)
        {
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDTO objInventoryCountDTO = new InventoryCountDTO();
            long SessionUserId = SessionHelper.UserID;
            InventoryCountDetailDTO objInventoryCountDetailDTO = new InventoryCountDetailDTO();
            objInventoryCountDTO = objInventoryCountDAL.GetInventoryCountByLIGUId(lstLineItems.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            objInventoryCountDetailDTO = objInventoryCountDAL.BeforeApplyAction(lstLineItems, SessionHelper.UserID);

            List<ItemLocationDetailsDTO> lstProperRecords = new List<ItemLocationDetailsDTO>();
            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDTO objItemLocationDetailsDTO = new ItemLocationDetailsDTO();
            objItemLocationDetailsDTO.ItemGUID = lstLineItems.ItemGUID;
            objItemLocationDetailsDTO.BinID = lstLineItems.BinID;
            objItemLocationDetailsDTO.ConsignedQuantity = lstLineItems.CountConsignedQuantity;
            objItemLocationDetailsDTO.CustomerOwnedQuantity = lstLineItems.CountCustomerOwnedQuantity;
            objItemLocationDetailsDTO.UDF1 = lstLineItems.UDF1;
            objItemLocationDetailsDTO.UDF2 = lstLineItems.UDF2;
            objItemLocationDetailsDTO.UDF3 = lstLineItems.UDF3;
            objItemLocationDetailsDTO.UDF4 = lstLineItems.UDF4;
            objItemLocationDetailsDTO.UDF5 = lstLineItems.UDF5;
            lstProperRecords.Add(objItemLocationDetailsDTO);
            if (lstProperRecords != null && lstProperRecords.Count > 0)
            {
                DataTable dtItemLocations = GetTableFromList(lstProperRecords);
                objItemLocationDetailsDAL.ApplyCountLineitem(dtItemLocations, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objInventoryCountDTO.GUID, objInventoryCountDetailDTO.GUID);
                lstProperRecords.ForEach(t =>
                {
                    //objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, SessionHelper.UserID, "Web", "Inventorycontroller>> ApplyOnCountLineItem");
                    objCartItemDAL.AutoCartUpdateByCode(t.ItemGUID ?? Guid.Empty, SessionHelper.UserID, "Web", "Inventory >> Apply Count", SessionUserId);
                });
            }
            return Json(new { Message = ResMessage.SaveMessage, Status = "ok", CurrentObj = objInventoryCountDetailDTO });


        }

        private DataTable GetTableFromList(List<ItemLocationDetailsDTO> lstItemLocs)
        {
            //RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = DateTime.UtcNow; //objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            DataTable ReturnDT = new DataTable("ItemLocationParam");
            try
            {
                DataColumn[] arrColumns = new DataColumn[]            {
                new DataColumn() { AllowDBNull=true,ColumnName="ItemGUID",DataType=typeof(Guid)},
                new DataColumn() { AllowDBNull=true,ColumnName="ItemNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinID",DataType=typeof(Int64)},
                new DataColumn() { AllowDBNull=true,ColumnName="BinNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="Expiration",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="Received",DataType=typeof(DateTime)},
                new DataColumn() { AllowDBNull=true,ColumnName="LotNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="SerialNumber",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ConsignedQuantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="CustomerOwnedQuantity",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(float)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF1",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF2",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF3",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF4",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="UDF5",DataType=typeof(String)},
                new DataColumn() { AllowDBNull=true,ColumnName="ProjectSpend",DataType=typeof(String)}
            };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstItemLocs != null && lstItemLocs.Count > 0)
                {
                    foreach (var item in lstItemLocs)
                    {
                        DateTime tempDT = DateTime.Now;
                        DateTime.TryParseExact(item.Expiration, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out tempDT);
                        DataRow row = ReturnDT.NewRow();
                        row["ItemGUID"] = item.ItemGUID;
                        row["ItemNumber"] = item.ItemNumber;
                        row["BinID"] = (item.BinID ?? 0) > 0 ? (object)item.BinID : DBNull.Value;
                        row["BinNumber"] = item.BinNumber;
                        row["Expiration"] = tempDT != DateTime.MinValue ? (object)tempDT : DBNull.Value;
                        row["Received"] = datetimetoConsider;
                        row["LotNumber"] = item.LotNumber;
                        row["SerialNumber"] = item.SerialNumber;
                        row["ConsignedQuantity"] = item.ConsignedQuantity.HasValue ? (object)item.ConsignedQuantity.Value : DBNull.Value;
                        row["CustomerOwnedQuantity"] = item.CustomerOwnedQuantity.HasValue ? (object)item.CustomerOwnedQuantity.Value : DBNull.Value;
                        row["ReceiptCost"] = item.Cost.HasValue ? (object)item.Cost.Value : DBNull.Value;
                        row["UDF1"] = item.UDF1;
                        row["UDF2"] = item.UDF2;
                        row["UDF3"] = item.UDF3;
                        row["UDF4"] = item.UDF4;
                        row["UDF5"] = item.UDF5;
                        row["ProjectSpend"] = string.Empty;
                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }


        //private DataTable GetTableFromStgList(List<MaterialStagingPullDetailDTO> lstItemLocs)
        //{
        //    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
        //    DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
        //    DataTable ReturnDT = new DataTable("ItemLocationParam");
        //    try
        //    {
        //        DataColumn[] arrColumns = new DataColumn[]            {
        //        new DataColumn() { AllowDBNull=true,ColumnName="ItemGUID",DataType=typeof(Guid)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="ItemNumber",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="BinID",DataType=typeof(Int64)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="BinNumber",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="Expiration",DataType=typeof(DateTime)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="Received",DataType=typeof(DateTime)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="LotNumber",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="SerialNumber",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="ConsignedQuantity",DataType=typeof(float)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="CustomerOwnedQuantity",DataType=typeof(float)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="ReceiptCost",DataType=typeof(float)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="UDF1",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="UDF2",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="UDF3",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="UDF4",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="UDF5",DataType=typeof(String)},
        //        new DataColumn() { AllowDBNull=true,ColumnName="ProjectSpend",DataType=typeof(String)}
        //    };
        //        ReturnDT.Columns.AddRange(arrColumns);

        //        if (lstItemLocs != null && lstItemLocs.Count > 0)
        //        {
        //            foreach (var item in lstItemLocs)
        //            {
        //                DateTime tempDT = DateTime.Now;
        //                DateTime.TryParseExact(item.Expiration, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out tempDT);
        //                DataRow row = ReturnDT.NewRow();
        //                row["ItemGUID"] = item.ItemGUID;
        //                row["ItemNumber"] = item.ItemNumber;
        //                row["BinID"] = (item.StagingBinId > 0) ? (object)item.StagingBinId : DBNull.Value;
        //                row["BinNumber"] = item.BinNumber;
        //                row["Expiration"] = tempDT != DateTime.MinValue ? (object)tempDT : DBNull.Value;
        //                row["Received"] = datetimetoConsider;
        //                row["LotNumber"] = item.LotNumber;
        //                row["SerialNumber"] = item.SerialNumber;
        //                row["ConsignedQuantity"] = item.ConsignedQuantity.HasValue ? (object)item.ConsignedQuantity.Value : DBNull.Value;
        //                row["CustomerOwnedQuantity"] = item.CustomerOwnedQuantity.HasValue ? (object)item.CustomerOwnedQuantity.Value : DBNull.Value;
        //                row["ReceiptCost"] = item.Cost.HasValue ? (object)item.Cost.Value : DBNull.Value;
        //                row["UDF1"] = item.UDF1;
        //                row["UDF2"] = item.UDF2;
        //                row["UDF3"] = item.UDF3;
        //                row["UDF4"] = item.UDF4;
        //                row["UDF5"] = item.UDF5;
        //                row["ProjectSpend"] = string.Empty;
        //                ReturnDT.Rows.Add(row);
        //            }
        //        }

        //        return ReturnDT;
        //    }
        //    catch
        //    {
        //        return ReturnDT;
        //    }
        //}


        public JsonResult GetDaysInMonth(int Month, int year)
        {
            return Json(DateTime.DaysInMonth(year, Month));
        }

        [HttpPost]
        public ActionResult LoadLotDetailPopupForCount(Guid? ItemGUID, Int64? BinId, Guid? CountDetailGUID, string Descriotion, bool IsStage)
        {
            ViewBag.ItemGUID = ItemGUID;
            ViewBag.BinId = BinId;
            ViewBag.CountDetailGUID = CountDetailGUID;
            ViewBag.Descriotion = Descriotion;
            ViewBag.IsStage = IsStage;
            return PartialView("LotDetailPopupForCount");
        }

        public ActionResult LoadLotDetailPopupForCountAjax(Guid ItemGUID, Int64 BinId, Guid? CountDetailGUID, bool IsStage)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<CountLineItemDetailDTO> lstCountLineItemDetail = null;

            if (CountDetailGUID == null || CountDetailGUID == Guid.Empty)
                lstCountLineItemDetail = objInventoryCountDAL.GetLotDetailForCount(ItemGUID, BinId, SessionHelper.RoomDateFormat, SessionHelper.CompanyID, SessionHelper.RoomID, IsStage);
            else
                lstCountLineItemDetail = objInventoryCountDAL.GetLotDetailForCountByCountDetailGUID(CountDetailGUID.Value, ItemGUID, SessionHelper.RoomDateFormat, SessionHelper.CompanyID, SessionHelper.RoomID);
            lstCountLineItemDetail.ForEach(x =>
            {
                x.Expiration = x.ExpirationDate.HasValue ? x.ExpirationDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
                x.IsStagingLocationCount = IsStage;
            });

            return Json(lstCountLineItemDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddUpdateLotDetailPopupForCount(List<CountLineItemDetailDTO> lstCountLineItemDetail)
        {
            CommonDAL cmnDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<CountLineItemDetailDTO> lstCountLineItemDetailTmp = new List<CountLineItemDetailDTO>();
            string ErrorMessage = "";
            int ValidItemCount = 0;
            string ReturnMessage = "";
            string ReturnStatus = "";
            Guid? CountDetailGUID = null;
            Guid ItemGUID = Guid.Empty;
            long BinId = 0;
            bool IsAdd = false;
            bool IsUpdate = false;

            if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0)
            {
                //--------------------------------------------------
                //
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                List<CountLineItemDetailDTO> locations = objBinMasterDAL.GetAllItemLocationsByItemId(lstCountLineItemDetail[0].ItemGUID.Value, SessionHelper.CompanyID, SessionHelper.RoomID, lstCountLineItemDetail[0].IsStagingLocationCount.GetValueOrDefault(false));

                //---------------VALIDATE COUNT LINE ITEM LIST AND CHECK FOR ADD OR UPDATE---------------
                //
                foreach (CountLineItemDetailDTO objCountLineItemDetailDTO in lstCountLineItemDetail)
                {
                    if (!ValidateCountLineItemDetail(objCountLineItemDetailDTO, locations, out ErrorMessage))
                    {
                        objCountLineItemDetailDTO.IsValidObject = false;
                    }
                    else
                    {
                        objCountLineItemDetailDTO.IsValidObject = true;

                        //------------------------------------------------------------------------
                        //
                        objCountLineItemDetailDTO.BinNumber = objCountLineItemDetailDTO.BinNumber.Trim();

                        if (objCountLineItemDetailDTO.DateCodeTracking == true)
                        {
                            objCountLineItemDetailDTO.Expiration = objCountLineItemDetailDTO.ExpirationDate.Value.ToString(SessionHelper.RoomDateFormat);

                            if ((objCountLineItemDetailDTO.SerialNumberTracking == true || objCountLineItemDetailDTO.LotNumberTracking == true)
                                    && objCountLineItemDetailDTO.LotSerialNumber.Contains('-') && objCountLineItemDetailDTO.LotSerialNumber.Contains('/'))
                            {
                                objCountLineItemDetailDTO.LotSerialNumber = objCountLineItemDetailDTO.LotSerialNumber.Substring(0, objCountLineItemDetailDTO.LotSerialNumber.LastIndexOf('-'));
                            }
                        }
                        else
                            objCountLineItemDetailDTO.Expiration = "";

                        if (objCountLineItemDetailDTO.SerialNumberTracking == true)
                        {
                            objCountLineItemDetailDTO.SerialNumber = objCountLineItemDetailDTO.LotSerialNumber;
                            objCountLineItemDetailDTO.LotNumber = "";
                        }
                        else if (objCountLineItemDetailDTO.LotNumberTracking == true)
                        {
                            objCountLineItemDetailDTO.SerialNumber = "";
                            objCountLineItemDetailDTO.LotNumber = objCountLineItemDetailDTO.LotSerialNumber;
                        }

                        //-----------------------INSERT NEW BIN IF REQUIRED-----------------------
                        //


                        BinMasterDTO objBinDTO = objBinMasterDAL.GetItemBinPlain(objCountLineItemDetailDTO.ItemGUID.Value, objCountLineItemDetailDTO.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objCountLineItemDetailDTO.IsStagingLocationCount.GetValueOrDefault(false));
                        if (objBinDTO != null && objBinDTO.ID > 0)
                        {
                            objCountLineItemDetailDTO.BinID = objBinDTO.ID;
                        }

                        //-------------------------SET QUANTITY-------------------------
                        //
                        if (objCountLineItemDetailDTO.Consignment == true && objCountLineItemDetailDTO.IsConsigned == true)
                        {
                            objCountLineItemDetailDTO.CountCustomerOwnedQuantity = (-0.000000001);
                            objCountLineItemDetailDTO.CountConsignedQuantity = (objCountLineItemDetailDTO.CountConsignedQuantity != null ? objCountLineItemDetailDTO.CountConsignedQuantity : (-0.000000001));
                        }
                        else if (objCountLineItemDetailDTO.Consignment == true && objCountLineItemDetailDTO.IsConsigned == false)
                        {
                            objCountLineItemDetailDTO.CountCustomerOwnedQuantity = (objCountLineItemDetailDTO.CountConsignedQuantity != null ? objCountLineItemDetailDTO.CountConsignedQuantity : (-0.000000001));
                            objCountLineItemDetailDTO.CountConsignedQuantity = (-0.000000001);
                        }
                        else
                        {
                            objCountLineItemDetailDTO.CountCustomerOwnedQuantity = objCountLineItemDetailDTO.CountConsignedQuantity;
                            objCountLineItemDetailDTO.CountConsignedQuantity = (-0.000000001);
                        }

                        //if (objCountLineItemDetailDTO.SerialNumberTracking == true)
                        //{
                        //    if (objCountLineItemDetailDTO.Consignment == true && objCountLineItemDetailDTO.IsConsigned == true)
                        //    {
                        //        objCountLineItemDetailDTO.CountCustomerOwnedQuantity = (-0.000000001);
                        //        objCountLineItemDetailDTO.CountConsignedQuantity = (objCountLineItemDetailDTO.CountConsignedQuantity != null ? objCountLineItemDetailDTO.CountConsignedQuantity : (-0.000000001));
                        //    }
                        //    else if (objCountLineItemDetailDTO.Consignment == true && objCountLineItemDetailDTO.IsConsigned == false)
                        //    {
                        //        objCountLineItemDetailDTO.CountCustomerOwnedQuantity = (objCountLineItemDetailDTO.CountConsignedQuantity != null ? objCountLineItemDetailDTO.CountConsignedQuantity : (-0.000000001));
                        //        objCountLineItemDetailDTO.CountConsignedQuantity = (-0.000000001);
                        //    }
                        //    else
                        //    {
                        //        objCountLineItemDetailDTO.CountCustomerOwnedQuantity = objCountLineItemDetailDTO.CountConsignedQuantity;
                        //        objCountLineItemDetailDTO.CountConsignedQuantity = (-0.000000001);
                        //    }
                        //}
                        //else
                        //{
                        //    objCountLineItemDetailDTO.CountCustomerOwnedQuantity = (objCountLineItemDetailDTO.CountCustomerOwnedQuantity != null ? objCountLineItemDetailDTO.CountCustomerOwnedQuantity : (-0.000000001));
                        //    if (objCountLineItemDetailDTO.Consignment == true)
                        //        objCountLineItemDetailDTO.CountConsignedQuantity = (objCountLineItemDetailDTO.CountConsignedQuantity != null ? objCountLineItemDetailDTO.CountConsignedQuantity : (-0.000000001));
                        //}

                        //-------------------------CHECK FOR ADD OR EDIT-------------------------
                        //
                        if (objInventoryCountDAL.GetCountDetailGUIDByItemGUIDBinID(objCountLineItemDetailDTO.CountGUID.Value, objCountLineItemDetailDTO.ItemGUID.Value, objCountLineItemDetailDTO.BinID.Value, out CountDetailGUID))
                        {
                            if (CountDetailGUID != null && CountDetailGUID != Guid.Empty)
                            {
                                objCountLineItemDetailDTO.CountDetailGUID = CountDetailGUID;
                                objCountLineItemDetailDTO.IsAdd = false;
                                objCountLineItemDetailDTO.IsUpdate = true;
                                ValidItemCount = ValidItemCount + 1;
                            }
                            else
                            {
                                objCountLineItemDetailDTO.CountDetailGUID = null;
                                objCountLineItemDetailDTO.IsAdd = true;
                                objCountLineItemDetailDTO.IsUpdate = false;
                                ValidItemCount = ValidItemCount + 1;
                            }
                        }
                        else
                        {
                            objCountLineItemDetailDTO.IsAdd = false;
                            objCountLineItemDetailDTO.IsUpdate = false;
                        }
                    }
                }

                lstCountLineItemDetail = (from A in lstCountLineItemDetail where A.IsValidObject == true orderby A.ItemGUID, A.BinID select A).ToList();

                //-------------------------------------------------------------------
                //
                if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0 && ValidItemCount > 0)
                {
                    //--------------------Check For Duplicate Lot--------------------
                    //
                    if (lstCountLineItemDetail[0].SerialNumberTracking == true)
                    {
                        //if(lstCountLineItemDetail.Select(x => new { SerialNumber = x.SerialNumber.ToLower().Trim() }).GroupBy(x => x.SerialNumber).Select(x => new { SerialNumber = x.Key, Cnt = x.Count() }).Where(x => x.Cnt > 1).Count() > 0)
                        //{
                        //    ReturnMessage = "Multiple entries with same Serial number is not allowed.";
                        //    ReturnStatus = "fail";
                        //    return Json(new { Message = ReturnMessage, Status = ReturnStatus });
                        //}

                        //var lstLotGroup = lstCountLineItemDetail
                        //                        .GroupBy(x => new { x.ItemGUID, x.BinID, x.LotSerialNumber })
                        //                            .Select(g => new { ItemGUID = g.Key.ItemGUID, BinID = g.Key.BinID, LotSerialNumber = g.Key.LotSerialNumber, Count = g.Count() });
                        var lstLotGroup = lstCountLineItemDetail
                                            .Select(x => new { ItemGUID = x.ItemGUID, LotSerialNumber = x.LotSerialNumber.Trim().ToLower() })
                                                .GroupBy(x => new { x.ItemGUID, x.LotSerialNumber })
                                                    .Select(g => new { ItemGUID = g.Key.ItemGUID, LotSerialNumber = g.Key.LotSerialNumber, Count = g.Count() });

                        if (lstLotGroup.Where(x => x.Count > 1).Count() > 0)
                        {
                            ReturnMessage = ResInventoryCount.SameSerialMultipleEntryNotAllowed;
                            ReturnStatus = "fail";
                            return Json(new { Message = ReturnMessage, Status = ReturnStatus });
                        }
                        List<string[]> lstCountLineItemDetailExisting = objInventoryCountDAL.GetCountLineItemListUsingItemGuid((lstCountLineItemDetail[0].ItemGUID ?? Guid.Empty), SessionHelper.RoomDateFormat, (lstCountLineItemDetail[0].BinID ?? 0));
                        if (lstCountLineItemDetailExisting != null && lstCountLineItemDetailExisting.Count() > 0)
                        {
                            lstCountLineItemDetailExisting = lstCountLineItemDetailExisting.Where(t1 => lstCountLineItemDetail.Any(t2 => t2.SerialNumber.ToLower().Trim() == t1[0].Split(new string[1] { "###" }, StringSplitOptions.RemoveEmptyEntries)[0].ToLower().Trim() && t2.BinID != Convert.ToInt64(t1[0].Trim().Split(new string[1] { "###" }, StringSplitOptions.RemoveEmptyEntries)[1]))).ToList();
                            if (lstCountLineItemDetailExisting != null && lstCountLineItemDetailExisting.Count() > 0)
                            {
                                foreach (string[] arrStr in lstCountLineItemDetailExisting)
                                {
                                    if (arrStr[1].Trim() == "")
                                    {
                                        ReturnMessage = ReturnMessage + (ReturnMessage == "" ? "" : ", ") +  string.Format(ResInventoryCount.ItemContainsQtyOnSerial, arrStr[0].Split(new string[1] { "###" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                                    }
                                    else
                                    {
                                        ReturnMessage = ReturnMessage + (ReturnMessage == "" ? "" : ", ") + string.Format(ResInventoryCount.ItemContainsQtyOnSerialInCount, arrStr[0].Split(new string[1] { "###" }, StringSplitOptions.RemoveEmptyEntries)[0], arrStr[1].Split('~')[0], arrStr[1].Split('~')[1]);
                                    }
                                }
                                ReturnStatus = "fail";
                                return Json(new { Message = ReturnMessage, Status = ReturnStatus });
                            }
                        }
                    }
                    else if (lstCountLineItemDetail[0].LotNumberTracking == true)
                    {
                        if (lstCountLineItemDetail[0].DateCodeTracking == true)
                        {


                            var lstLotGroup = lstCountLineItemDetail
                                                .GroupBy(x => new { x.ItemGUID, x.BinID, x.LotSerialNumber, x.ExpirationDate })
                                                    .Select(g => new { ItemGUID = g.Key.ItemGUID, BinID = g.Key.BinID, LotSerialNumber = g.Key.LotSerialNumber, Count = g.Count() });

                            if (lstLotGroup.Where(x => x.Count > 1).Count() > 0)
                            {
                                ReturnMessage = ResInventoryCount.SameLotMultipleEntryNotAllowed;
                                ReturnStatus = "fail";
                                return Json(new { Message = ReturnMessage, Status = ReturnStatus });
                            }
                            else
                            {
                                string innserMsg = "";
                                bool isError = false;
                                foreach (var item in lstCountLineItemDetail)
                                {
                                    string Expiration = item.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy");
                                    string msg = cmnDAL.CheckDuplicateLotAndExpiration(item.LotNumber, Expiration, item.ExpirationDate.GetValueOrDefault(DateTime.MinValue), 0, SessionHelper.RoomID, SessionHelper.CompanyID, item.ItemGUID.GetValueOrDefault(Guid.Empty),SessionHelper.UserID,SessionHelper.EnterPriceID);
                                    if (string.IsNullOrWhiteSpace(msg) || (msg ?? string.Empty).ToLower() == "ok")
                                    {
                                        var isExists = lstCountLineItemDetail.Where(x => x.ExpirationDate != item.ExpirationDate && (x.LotNumber ?? string.Empty).ToLower() == (item.LotNumber ?? string.Empty).ToLower()).FirstOrDefault();
                                        if (isExists != null)
                                        {
                                            isError = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        innserMsg = msg;
                                        isError = true;
                                        break;
                                    }

                                }
                                if (isError)
                                {
                                    if (string.IsNullOrWhiteSpace(innserMsg))
                                    {
                                        ReturnMessage = ResInventoryCount.SameLotDifferentExpirationNotAllowed;
                                        ReturnStatus = "fail";
                                        return Json(new { Message = ReturnMessage, Status = ReturnStatus });
                                    }
                                    else
                                    {
                                        ReturnMessage = innserMsg;
                                        ReturnStatus = "fail";
                                        return Json(new { Message = ReturnMessage, Status = ReturnStatus });
                                    }
                                }
                            }
                        }
                        else
                        {
                            var lstLotGroup = lstCountLineItemDetail
                                                .GroupBy(x => new { x.ItemGUID, x.BinID, x.LotSerialNumber })
                                                    .Select(g => new { ItemGUID = g.Key.ItemGUID, BinID = g.Key.BinID, LotSerialNumber = g.Key.LotSerialNumber, Count = g.Count() });

                            if (lstLotGroup.Where(x => x.Count > 1).Count() > 0)
                            {
                                ReturnMessage = ResInventoryCount.SameLotMultipleEntryNotAllowed;
                                ReturnStatus = "fail";
                                return Json(new { Message = ReturnMessage, Status = ReturnStatus });
                            }
                        }
                    }

                    //-------------------------------------------------------------------
                    //
                    foreach (CountLineItemDetailDTO objCountLineItemDetailDTO in lstCountLineItemDetail)
                    {
                        if (ItemGUID != Guid.Empty && BinId != 0 && lstCountLineItemDetailTmp.Count > 0 &&
                            (objCountLineItemDetailDTO.ItemGUID != ItemGUID || objCountLineItemDetailDTO.BinID != BinId))
                        {
                            if (IsAdd == true)
                                SaveLotDetailPopupForCountAdd(lstCountLineItemDetailTmp);
                            else if (IsUpdate == true)
                                SaveLotDetailPopupForCountUpdate(lstCountLineItemDetailTmp);

                            lstCountLineItemDetailTmp = new List<CountLineItemDetailDTO>();
                        }

                        lstCountLineItemDetailTmp.Add(objCountLineItemDetailDTO);
                        IsAdd = objCountLineItemDetailDTO.IsAdd.Value;
                        IsUpdate = objCountLineItemDetailDTO.IsUpdate.Value;
                        ItemGUID = objCountLineItemDetailDTO.ItemGUID.Value;
                        BinId = objCountLineItemDetailDTO.BinID.Value;
                    }

                    if (lstCountLineItemDetailTmp.Count > 0)
                    {
                        if (IsAdd == true)
                            SaveLotDetailPopupForCountAdd(lstCountLineItemDetailTmp);
                        else if (IsUpdate == true)
                            SaveLotDetailPopupForCountUpdate(lstCountLineItemDetailTmp);
                    }

                    //-------------------------------------------------------------------
                    //
                    ReturnMessage = ValidItemCount.ToString() + " " + ResMessage.SaveMessage;
                    ReturnStatus = "ok";
                }
                else
                {
                    ReturnMessage = ResCommon.NoRecordUpdated;
                    ReturnStatus = "fail";
                    return Json(new { Message = ReturnMessage, Status = ReturnStatus });
                }
            }
            else
            {
                ReturnMessage = ResCommon.NoRecordFound;
                ReturnStatus = "fail";
            }

            return Json(new { Message = ReturnMessage, Status = ReturnStatus });
        }

        public bool SaveLotDetailPopupForCountAdd(List<CountLineItemDetailDTO> lstCountLineItemDetail)
        {
            Guid CountGUID = Guid.Empty;
            Guid CountDetailGUID = Guid.Empty;
            string message = "";
            string status = "";
            double? ConsignedQuantity = 0;
            double? CustomerOwnedQuantity = 0;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);

            if (lstCountLineItemDetail != null && lstCountLineItemDetail.Count > 0)
            {
                //-------------------------------------------------------------------
                //
                ConsignedQuantity = (from A in lstCountLineItemDetail select (A.CountConsignedQuantity < 0 ? 0 : A.CountConsignedQuantity)).Sum();
                CustomerOwnedQuantity = (from A in lstCountLineItemDetail select (A.CountCustomerOwnedQuantity < 0 ? 0 : A.CountCustomerOwnedQuantity)).Sum();

                if (ConsignedQuantity < 0)
                    ConsignedQuantity = (-0.000000001);

                if (CustomerOwnedQuantity < 0)
                    CustomerOwnedQuantity = (-0.000000001);

                //-------------------------------------------------------------------
                //
                if (AddItemToCountAndReturnDetailGUID(lstCountLineItemDetail[0].CountGUID, lstCountLineItemDetail[0].ItemGUID, lstCountLineItemDetail[0].BinID,
                                                      ConsignedQuantity, CustomerOwnedQuantity, lstCountLineItemDetail[0].IsStagingLocationCount,
                                                      null, null, null, null, null, null, null, lstCountLineItemDetail[0].ItemDescription, lstCountLineItemDetail[0].ItemType, false, lstCountLineItemDetail[0].ProjectSpendGUID,
                                                      out CountGUID, out CountDetailGUID, out message, out status, lstCountLineItemDetail))
                {
                    foreach (CountLineItemDetailDTO objCountLineItemDetail in lstCountLineItemDetail)
                    {
                        objCountLineItemDetail.CountDetailGUID = CountDetailGUID;
                        objCountLineItemDetail.GUID = Guid.NewGuid();
                        objCountLineItemDetail.CompanyID = SessionHelper.CompanyID;
                        objCountLineItemDetail.RoomID = SessionHelper.RoomID;
                        objCountLineItemDetail.Created = DateTime.UtcNow;
                        objCountLineItemDetail.CreatedBy = SessionHelper.UserID;
                        objCountLineItemDetail.Updated = DateTime.UtcNow;
                        objCountLineItemDetail.LastUpdatedBy = SessionHelper.UserID;
                        objCountLineItemDetail.IsDeleted = false;
                        objCountLineItemDetail.IsArchived = false;
                        objCountLineItemDetail.AddedFrom = "Web";
                        objCountLineItemDetail.ReceivedOn = DateTime.UtcNow;
                        objCountLineItemDetail.ReceivedOnWeb = DateTime.UtcNow;
                        objInventoryCountDAL.AddCountLineItemDetail(objCountLineItemDetail);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        

        

        public bool ValidateCountLineItemDetail(CountLineItemDetailDTO objCountLineItemDetail, List<CountLineItemDetailDTO> locations, out string Message)
        {
            Message = "";

            if (objCountLineItemDetail.ItemGUID == null || objCountLineItemDetail.ItemGUID == Guid.Empty)
            {
                Message = string .Format(ResCommon.MsgMissing, ResLayout.Item);
                return false;
            }

            if (String.IsNullOrEmpty(objCountLineItemDetail.BinNumber))
            {
                Message = string.Format(ResCommon.MsgMissing, ResInventoryCountDetail.BinNumber);
                return false;
            }

            if (objCountLineItemDetail.SerialNumberTracking == true || objCountLineItemDetail.LotNumberTracking == true)
            {
                if (String.IsNullOrEmpty(objCountLineItemDetail.LotSerialNumber))
                {
                    var lotSerial = ResPullMaster.LotNumber +"/" + ResPullMaster.SerialNumber;
                    Message = string.Format(ResCommon.MsgMissing, lotSerial);
                    return false;
                }
            }

            if ((objCountLineItemDetail.DateCodeTracking != null && objCountLineItemDetail.DateCodeTracking == true) && objCountLineItemDetail.ExpirationDate == null)
            {
                Message = string.Format(ResCommon.MsgMissing, ResItemLocationDetails.ExpirationDate);
                return false;
            }

            if (objCountLineItemDetail.CountConsignedQuantity != null)
            {
                if (objCountLineItemDetail.CountConsignedQuantity < 0)
                {
                    Message = ResCommon.QtyCannotLessThanZeroForSerial;
                    return false;
                }
                else if (objCountLineItemDetail.SerialNumberTracking == true && objCountLineItemDetail.CountConsignedQuantity > 1)
                {
                    Message = ResCommon.QtyCannotGreaterThanOneForSerial;
                    return false;
                }
            }

            //if (objCountLineItemDetail.SerialNumberTracking == true)
            //{
            //    if (objCountLineItemDetail.CountConsignedQuantity != null)
            //    {
            //        if (objCountLineItemDetail.CountConsignedQuantity > 1)
            //        {
            //            Message = "Quantity can not be greater than 1 for serial number tracking";
            //            return false;
            //        }
            //        else if (objCountLineItemDetail.CountConsignedQuantity < 0)
            //        {
            //            Message = "Quantity can not be less than 0 for serial number tracking";
            //            return false;
            //        }
            //    }
            //}
            //else
            //{
            //    if (objCountLineItemDetail.CountCustomerOwnedQuantity < 0)
            //    {
            //        Message = "Customer owned quantity can not be less than 0";
            //        return false;
            //    }

            //    if (objCountLineItemDetail.Consignment == true && objCountLineItemDetail.CountConsignedQuantity < 0)
            //    {
            //        Message = "Consigned quantity can not be less than 0";
            //        return false;
            //    }
            //}

            //if (objCountLineItemDetail.SerialNumberTracking == true)
            //{
            //    if (objCountLineItemDetail.CountConsignedQuantity == null || objCountLineItemDetail.CountConsignedQuantity < 0)
            //    {
            //        Message = "Quantity is missing";
            //        return false;
            //    }
            //    else if (objCountLineItemDetail.CountConsignedQuantity > 1)
            //    {
            //        Message = "Quantity can not be greater then 1 for serial number tracking";
            //        return false;
            //    }
            //}
            //else
            //{
            //    if (objCountLineItemDetail.LotNumberTracking == true && objCountLineItemDetail.CountCustomerOwnedQuantity <= 0)
            //    {
            //        if (locations.Where(x => x.BinNumber.Trim().ToUpper() == objCountLineItemDetail.BinNumber.Trim().ToUpper()
            //                                    && x.LotSerialNumber.Trim().ToUpper() == objCountLineItemDetail.LotSerialNumber.Trim().ToUpper()).Count() <= 0)
            //        {
            //            Message = "Customer owned quantity is missing";
            //            return false;
            //        }
            //    }
            //    else if (objCountLineItemDetail.CountCustomerOwnedQuantity == null || objCountLineItemDetail.CountCustomerOwnedQuantity <= 0)
            //    {
            //        Message = "Customer owned quantity is missing";
            //        return false;
            //    }

            //    if (objCountLineItemDetail.Consignment == true)
            //    {
            //        if (objCountLineItemDetail.LotNumberTracking == true && objCountLineItemDetail.CountConsignedQuantity <= 0)
            //        {
            //            if (locations.Where(x => x.BinNumber.Trim().ToUpper() == objCountLineItemDetail.BinNumber.Trim().ToUpper()
            //                                        && x.LotSerialNumber.Trim().ToUpper() == objCountLineItemDetail.LotSerialNumber.Trim().ToUpper()).Count() <= 0)
            //            {
            //                Message = "Consigned quantity is missing";
            //                return false;
            //            }
            //        }
            //        else if ((objCountLineItemDetail.CountConsignedQuantity == null || objCountLineItemDetail.CountConsignedQuantity <= 0))
            //        {
            //            Message = "Consigned quantity is missing";
            //            return false;
            //        }
            //    }
            //}

            return true;
        }

        public ActionResult GetCountLineItemDetailList(Guid InventoryCountGUID, bool LotNumberTracking, bool SerialNumberTracking, bool DateCodeTracking, bool IsApplied, string BinNumber, string Description, bool IsStage)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            List<CountLineItemDetailDTO> lstCountLineItemDetail = objInventoryCountDAL.GetCountLineItemDetailList(InventoryCountGUID, SessionHelper.RoomDateFormat, LotNumberTracking, SerialNumberTracking, DateCodeTracking, IsApplied, BinNumber, Description, IsStage);
            if (lstCountLineItemDetail == null || lstCountLineItemDetail.Count <= 0)
            {
                InventoryCountDetailDTO objInventoryCountDetailDTO = objInventoryCountDAL.GetInventoryCountdtlByGUId(InventoryCountGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                lstCountLineItemDetail = new List<CountLineItemDetailDTO>();
                CountLineItemDetailDTO objCountLineItemDetailDTO = new CountLineItemDetailDTO();
                objCountLineItemDetailDTO.ItemGUID = objInventoryCountDetailDTO.ItemGUID;
                objCountLineItemDetailDTO.BinID = objInventoryCountDetailDTO.BinID;
                objCountLineItemDetailDTO.CountDetailGUID = objInventoryCountDetailDTO.GUID;
                objCountLineItemDetailDTO.BinNumber = "";
                objCountLineItemDetailDTO.Comment = "";
                objCountLineItemDetailDTO.LotSerialNumber = "";
                objCountLineItemDetailDTO.CountCustomerOwnedQuantity = 0;
                objCountLineItemDetailDTO.CountConsignedQuantity = 0;

                objCountLineItemDetailDTO.CusOwnedDifference = 0;
                objCountLineItemDetailDTO.ConsignedDifference = 0;

                objCountLineItemDetailDTO.GUID = Guid.Empty;
                objCountLineItemDetailDTO.IsStagingLocationCount = IsStage;
                lstCountLineItemDetail.Add(objCountLineItemDetailDTO);
            }
            return PartialView("_CountLineItemDetailList", lstCountLineItemDetail);
        }

        public JsonResult DeleteLineItemCountByGUID(Guid CountLineItemGuid)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            if (objInventoryCountDAL.DeleteCountLineItemDetail(CountLineItemGuid))
            {
                objInventoryCountDAL.UpdateCountInInventoryCountDetailsByCountLineItemGuid(CountLineItemGuid, SessionHelper.UserID, SessionHelper.CompanyID);
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" });
            }
            else
            {
                return Json(new { Message = ResCommon.SomeErrorInDeleteRecord, Status = "fail" }); 
            }
        }

        public void UpdateCountLineItemOnApply(CountLineItemDetailDTO objCountLineItemDetail, List<CountLineItemDetailDTO> lstCountLineItemDetail)
        {
            InventoryCountDAL countDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            countDAL.UpdateCountLineItemOnApply(objCountLineItemDetail, lstCountLineItemDetail, SessionHelper.UserID, SessionHelper.RoomDateFormat);
                
        }

        public bool AddMissingLotSerial(Guid CountDetailGUID, Guid ItemGUID, bool IsStagingLocation = false)
        {
            InventoryCountDAL countDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            return countDAL.AddMissingLotSerial( CountDetailGUID, ItemGUID,SessionHelper.RoomDateFormat, SessionHelper.UserID,SessionHelper.CompanyID,SessionHelper.RoomID,IsStagingLocation);
        }

        public bool SaveLotDetailPopupForCountUpdate(List<CountLineItemDetailDTO> lstCountLineItemDetail
            ) {
            InventoryCountDAL countDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            return countDAL.SaveLotDetailPopupForCountUpdate(lstCountLineItemDetail, SessionHelper.RoomDateFormat,SessionHelper.UserID,
                SessionHelper.CompanyID, SessionHelper.RoomID               
                );
        }

    }
}
