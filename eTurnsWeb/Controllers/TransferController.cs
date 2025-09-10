using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using eTurns.DTO.Utils;
using System.Configuration;

namespace eTurnsWeb.Controllers
{

    [AuthorizeHelper]
    public class TransferController : eTurnsControllerBase
    {

        UDFController objUDFDAL = new UDFController();
        bool IsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        bool IsUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
        bool IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
        bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TransferSubmit, eTurnsWeb.Helper.SessionHelper.PermissionType.Submit);
        bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TransferApproval, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
        bool IsChangeOrder = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ChangeTransfer, eTurnsWeb.Helper.SessionHelper.PermissionType.ChangeOrder);


        #region Transfer Master

        /// <summary>
        /// TransferList
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferList()
        {
            return View();
        }

        /// <summary>
        /// KitMasterListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public JsonResult TransferListAjax(JQueryDataTableParamModel param)
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
            {
                sortColumnName = "ID desc";
            }


            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            string MainFilter = "";
            if (Convert.ToString(Session["MainFilter"]).Trim().ToLower() == "true")
            {
                MainFilter = "true";
            }

            TransferMasterDAL transferDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<TransferMasterDTO> DataFromDB = transferDAL.GetPagedTransfers(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(Session["RoomDateFormat"]), Convert.ToString(Session["CurrentTimeZone"]), MainFilter);

            DataFromDB.ToList().ForEach(x => x.IsAbleToDelete = IsRecordDeleteable(x));

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// _TransferCreate
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _TransferCreate()
        {
            return PartialView();
        }

        /// <summary>
        /// CreateKit
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateTransferIn()
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
            //string autoNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextTransferNo", SessionHelper.RoomID, SessionHelper.CompanyID);
            objAutoNumber = objAutoSeqDAL.GetNextTransferNumber(SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.EnterPriceID);

            string autoNumber = objAutoNumber.OrderNumber;
            if (autoNumber != null && (!string.IsNullOrEmpty(autoNumber)))
            {
                autoNumber = autoNumber.Length > 22 ? autoNumber.Substring(0, 22) : autoNumber;
            }
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);

            TransferMasterDTO objDTO = new TransferMasterDTO()
            {
                RequireDate = datetimetoConsider,
                TransferNumber = autoNumber,
                TransferStatus = (int)TransferStatus.UnSubmitted,
                RequestType = (int)RequestType.In,
                RequestingRoomID = SessionHelper.RoomID,
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                RoomID = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
            };

            objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
            objDTO.RequiredDateString = objDTO.RequireDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("TransferMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ViewBag.TransferStatusList = GetTransferStatusList(objDTO, "create");
            RoomDAL roomCtrl = new RoomDAL(SessionHelper.EnterPriseDBName);
            //List<RoomDTO> roomDTOList = roomCtrl.GetAllRecords(SessionHelper.CompanyID, false, false).Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
            IEnumerable<RoomDTO> rooms = roomCtrl.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderBy(x => x.RoomName).ToList();
            RoomDTO currentRoom = rooms.FirstOrDefault(x => x.ID == SessionHelper.RoomID);
            if (currentRoom != null && currentRoom.ReplineshmentRoom.GetValueOrDefault(0) > 0)
                objDTO.ReplenishingRoomID = currentRoom.ReplineshmentRoom.GetValueOrDefault(0);

            List<RoomDTO> roomDTOList = rooms.Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
            ViewBag.ReplenishingRoom = roomDTOList;

            //ViewBag.StagginLocations = GetStagginLocation();

            return PartialView("_TransferCreate", objDTO);
        }

        /// <summary>
        /// CreateKit
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateTransferOut()
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
            //string autoNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextTransferNo", SessionHelper.RoomID, SessionHelper.CompanyID);
            objAutoNumber = objAutoSeqDAL.GetNextTransferNumber(SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.EnterPriceID);

            string autoNumber = objAutoNumber.OrderNumber;
            if (autoNumber != null && (!string.IsNullOrEmpty(autoNumber)))
            {
                autoNumber = autoNumber.Length > 22 ? autoNumber.Substring(0, 22) : autoNumber;
            }
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);

            TransferMasterDTO objDTO = new TransferMasterDTO()
            {
                RequireDate = datetimetoConsider,
                TransferNumber = autoNumber,
                TransferStatus = (int)TransferStatus.UnSubmitted,
                RequestType = (int)RequestType.Out,
                RequestingRoomID = SessionHelper.RoomID,

                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,

                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                RoomID = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,

            };
            objDTO.RequiredDateString = objDTO.RequireDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("TransferMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ViewBag.TransferStatusList = GetTransferStatusList(objDTO, "create");

            RoomDAL roomCtrl = new RoomDAL(SessionHelper.EnterPriseDBName);
            //List<RoomDTO> roomDTOList = roomCtrl.GetAllRecords(SessionHelper.CompanyID, false, false).Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
            //ViewBag.ReplenishingRoom = roomDTOList;
            IEnumerable<RoomDTO> rooms = roomCtrl.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderBy(x => x.RoomName).ToList();
            RoomDTO currentRoom = rooms.FirstOrDefault(x => x.ID == SessionHelper.RoomID);
            if (currentRoom != null && currentRoom.ReplineshmentRoom.GetValueOrDefault(0) > 0)
                objDTO.ReplenishingRoomID = currentRoom.ReplineshmentRoom.GetValueOrDefault(0);

            List<RoomDTO> roomDTOList = rooms.Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
            ViewBag.ReplenishingRoom = roomDTOList;

            //ViewBag.StagginLocations = null; //GetStagginLocation();
            return PartialView("_TransferCreate", objDTO);

        }


        /// <summary>
        /// CreateKit
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferEdit(Int64 ID)
        {
            TransferMasterDTO objDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferByIdFull(ID);
            objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
            if (objDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.IsRecordNotEditable = true;
                if (objDTO.TransferStatus < (int)TransferStatus.Transmitted)
                    objDTO.IsOnlyStatusUpdate = true;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("TransferMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            if (objDTO.IsDeleted.GetValueOrDefault(false) || objDTO.IsArchived.GetValueOrDefault(false))
            {
                ViewBag.TransferStatusList = GetTransferStatusList(objDTO, "");
                objDTO.IsRecordNotEditable = true;
                objDTO.IsOnlyStatusUpdate = false;
                objDTO.IsAbleToDelete = true;
            }
            else if (objDTO.IsRecordNotEditable && objDTO.TransferStatus >= (int)TransferStatus.Transmitted)
            {
                ViewBag.TransferStatusList = GetTransferStatusList(objDTO, "");
            }
            else
            {
                ViewBag.TransferStatusList = GetTransferStatusList(objDTO, "edit");
            }

            RoomDAL roomCtrl = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<RoomDTO> roomDTOList = roomCtrl.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
            ViewBag.ReplenishingRoom = roomDTOList;
            objDTO.RequiredDateString = objDTO.RequireDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            // ViewBag.StagginLocations = GetStagginLocation();

            return PartialView("_TransferCreate", objDTO);
        }

        /// <summary>
        /// Save Transfer
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        /// 
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveTransfer(TransferMasterDTO objDTO)
        {

            var ignoreProperty = new List<string>() { };
            var valResult = DTOCommonUtils.ValidateDTO<TransferMasterDTO>(objDTO, ControllerContext, ignoreProperty);


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
            TransferMasterDAL obj = null;
            CommonDAL objCDAL = null;
            //List<TransferDetailDTO> lstOrdDetailDTO = null;
            TransferDetailDAL transferDetailDAL = new TransferDetailDAL(SessionHelper.EnterPriseDBName);

            try
            {
                obj = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                if (string.IsNullOrEmpty(objDTO.TransferNumber))
                {
                    message = string.Format(ResMessage.Required, ResTransfer.TransferNumber);
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                else if (objDTO.TransferStatus == (int)TransferStatus.Rejected)
                {
                    if (string.IsNullOrEmpty(objDTO.RejectionReason))
                    {
                        message = string.Format(ResMessage.Required, ResTransfer.RejectionReason); 
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                    objDTO.TransferStatus = (int)TransferStatus.UnSubmitted;

                }

                string errorMsg = "";
                bool isUdfValid = objCDAL.CheckUDFIsRequired("TransferMaster", objDTO.UDF1, objDTO.UDF2, objDTO.UDF3, objDTO.UDF4, objDTO.UDF5, out errorMsg, SessionHelper.CompanyID, SessionHelper.RoomID,SessionHelper.EnterPriceID,SessionHelper.UserID, "");
                if (isUdfValid)
                {
                    status = "fail";
                    return Json(new { Message = errorMsg, Status = status }, JsonRequestBehavior.AllowGet);
                }

                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.RequireDate = DateTime.ParseExact(objDTO.RequiredDateString, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                objDTO.RoomID = SessionHelper.RoomID;
                objDTO.StagingID = objCDAL.GetOrInsertMaterialStagingIDByName(objDTO.StagingName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID);
                objDTO.RequestingRoomID = SessionHelper.RoomID;

                if (objDTO.ID == 0)
                {
                    string strOK = "";
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResTransfer.TransferNumber, objDTO.TransferNumber);
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.GUID = Guid.NewGuid();
                        long ReturnVal = obj.Insert(objDTO,false);
                        if (ReturnVal > 0)
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
                else
                {
                    string strOK = "";
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResOrder.OrderNumber, objDTO.TransferNumber);
                        status = "duplicate";
                    }
                    else
                    {

                        var transferLineItemCount = transferDetailDAL.GetTransferDetailRecordCount(objDTO.GUID);
                        var transferBeforeUpdate = obj.GetTransferByIdPlain(objDTO.ID);
                        if ((objDTO.TransferStatus >= (int)TransferStatus.Submitted && (transferLineItemCount <= 0)) && !(transferBeforeUpdate.TransferStatus == (int)TransferStatus.UnSubmitted && objDTO.TransferStatus == (int)TransferStatus.Closed) )
                        {                            
                            message = string.Format(ResTransfer.TransferMustHaveMinimumOneLineItem);
                            status = "duplicate";
                        }
                        else
                        {
                            if (IsApprove)
                            {
                                if ((objDTO.TransferStatus == (int)TransferStatus.Submitted || objDTO.TransferStatus == (int)TransferStatus.Approved) && objDTO.RequestType == (int)RequestType.In)
                                    objDTO.TransferStatus = (int)TransferStatus.Transmitted;

                                if (objDTO.TransferStatus == (int)TransferStatus.Submitted && objDTO.RequestType == (int)RequestType.Out)
                                    objDTO.TransferStatus = (int)TransferStatus.Approved;
                            }

                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objDTO.EditedFrom = "Web";
                            var isTransferStatusClosed = (objDTO.TransferStatus == (int)TransferStatus.Closed);
                            var returnIntransitItemsResult = new ReturnIntransitItemsResult();

                            if (isTransferStatusClosed)
                            {
                                //Below code is to return intransit items to replenish room on transfer close.
                                returnIntransitItemsResult = obj.ReturnInTransitItemsToReplenishRoomOnCloseTransfer(objDTO.GUID, SessionHelper.UserID);
                            }

                            
                            bool ReturnVal = isTransferStatusClosed ? (returnIntransitItemsResult.ReturnValue ? obj.Edit(objDTO) : false) : obj.Edit(objDTO);
                            long SessionUserId = SessionHelper.UserID;
                            
                            if (ReturnVal)
                            {
                                if (objDTO.TransferStatus == (int)TransferStatus.Submitted)
                                {
                                    SendMailToApprovalAuthority(objDTO);
                                }
                                else if (objDTO.TransferStatus == (int)TransferStatus.Transmitted && objDTO.RequestType == (int)RequestType.In)
                                {
                                    TransferMasterDTO ReplinishTransfer = obj.GetTransferByRefTransferGuidPlain(objDTO.ReplenishingRoomID, objDTO.CompanyID, objDTO.GUID);
                                    var tmpsupplierIds = new List<long>();
                                    List<TransferDetailDTO> TransferItems = transferDetailDAL.GetTransferDetailNormal(ReplinishTransfer.GUID, ReplinishTransfer.RoomID, ReplinishTransfer.CompanyID, tmpsupplierIds);
                                    ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                    foreach (var item in TransferItems)
                                    {
                                        ItemMasterDTO itemDTO = ItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                                        itemDTO.LastUpdatedBy = SessionHelper.UserID;
                                        itemDTO.Updated = DateTimeUtility.DateTimeNow;
                                        itemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itemDTO.EditedFrom = "Web";
                                        ItemDAL.Edit(itemDTO, SessionUserId,SessionHelper.EnterPriceID);
                                    }

                                    SendMailForApprovedOrRejected(objDTO, "Approved");
                                }
                                else if (objDTO.TransferStatus == (int)TransferStatus.Closed || objDTO.TransferStatus == (int)TransferStatus.Rejected)
                                {
                                    TransferInOutQtyDetailDAL objTransferInOutQtyDetailDAL = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
                                    var tmpsupplierIds = new List<long>();
                                    List<TransferDetailDTO> TransferItems = transferDetailDAL.GetTransferDetailNormal(objDTO.GUID, objDTO.RoomID, objDTO.CompanyID, tmpsupplierIds);
                                    foreach (var item in TransferItems)
                                    {
                                        objTransferInOutQtyDetailDAL.UpdateOnTransferQuantityByTransferDetailGUID(item.GUID, SessionHelper.UserID, true, SessionUserId);
                                    }
                                }

                                if (transferBeforeUpdate != null && ((objDTO.RequestType == (int)RequestType.In && transferBeforeUpdate.TransferStatus <= (int)TransferStatus.Submitted
                                    && (objDTO.TransferStatus > (int)TransferStatus.Submitted && objDTO.TransferStatus < (int)TransferStatus.Closed))
                                    || (objDTO.RequestType == (int)RequestType.Out && transferBeforeUpdate.TransferStatus != (int)TransferStatus.Approved &&
                                    transferBeforeUpdate.TransferStatus != (int)TransferStatus.Transmitted && (objDTO.TransferStatus == (int)TransferStatus.Transmitted || objDTO.TransferStatus == (int)TransferStatus.Approved))))

                                {
                                    try
                                    {
                                        string eventName = objDTO.RequestType == (int)RequestType.In ? CommonUtility.OnTransferInApproveEventName 
                                                                                                     : CommonUtility.OnTransferOutApproveEventName;
                                        
                                            string transferGUIDs = "<DataGuids>" + Convert.ToString(objDTO.GUID) + "</DataGuids>";
                                            string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                                            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                                            List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                                            if (lstNotification != null && lstNotification.Count > 0)
                                            {
                                                objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, transferGUIDs);
                                            }
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                                    }
                                }
                            }
                            else if (objDTO.TransferStatus == (int)TransferStatus.Submitted)
                            {
                                SendMailToApprovalAuthority(objDTO);
                            }

                            if (ReturnVal)
                            {
                                message = ResMessage.SaveMessage;
                                status = "ok";
                            }
                            else
                            {
                                if (isTransferStatusClosed && (!returnIntransitItemsResult.ReturnValue))
                                {
                                    message = string.Format(returnIntransitItemsResult.ReturnMessage, HttpStatusCode.ExpectationFailed);
                                    status = "fail";
                                }
                                else
                                {
                                    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                                    status = "fail";
                                }

                            }
                        }
                    }
                }

                Session["IsInsert"] = "True";

                return Json(new { Message = message, Status = status, objModel = objDTO }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message.ToString(), Status = "fail", objModel = objDTO }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                obj = null;
                objCDAL = null;
                //lstOrdDetailDTO = null;
            }
        }

        public JsonResult SendMailForTransferApproved(string TransferGuid)
        {
                try
                {
                    if (!string.IsNullOrEmpty(TransferGuid) && !string.IsNullOrWhiteSpace(TransferGuid))
                    {
                        string eventName = CommonUtility.OnTransferOutApproveEventName;
                        string transferGUIDs = "<DataGuids>" + TransferGuid + "</DataGuids>";
                        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                        NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                        List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                        if (lstNotification != null && lstNotification.Count > 0)
                        {
                            objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, transferGUIDs);
                        }

                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteTransferMasterRecords(string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    long SessionUserId = SessionHelper.UserID;
                    TransferMasterDAL obj = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                    obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionUserId,SessionHelper.EnterPriceID);
                    return "ok";
                }
                else
                {
                    return "notdelete";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public JsonResult UpdateTransferToClose(Int64 TransferID)
        {
            TransferMasterDAL obj = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            TransferMasterDTO objDTO = obj.GetTransferByIdNormal(TransferID);
            objDTO.TransferStatus = (int)TransferStatus.Closed;

            return SaveTransfer(objDTO);
        }

        #endregion

        #region Private Methods

        private List<RoomDTO> GetReplinishingRoom()
        {
            //RoomController roomCtrl = new RoomController();
            //RoomDAL roomCtrl = new RoomDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO roomDTO = roomCtrl.GetRoomByIDPlain(SessionHelper.RoomID);

            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,ReplineshmentRoom";
            RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            List<RoomDTO> listReplinishRoom = new List<RoomDTO>();

            if (roomDTO.ReplineshmentRoom.GetValueOrDefault(0) > 0)
            {
                string columnRList = "ID,RoomName,ReplineshmentRoom";
                RoomDTO ReplinishRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnRList, "Room", "ID = " + roomDTO.ReplineshmentRoom.ToString() + "", "");
                // RoomDTO ReplinishRoom = roomCtrl.GetRoomByIDPlain(roomDTO.ReplineshmentRoom.GetValueOrDefault(0));
                listReplinishRoom.Add(ReplinishRoom);
            }
            //listReplinishRoom.Insert(0, new RoomDTO());
            return listReplinishRoom;
        }



        private List<SelectListItem> GetTransferStatusList(TransferMasterDTO objDTO, string Mode)
        {
            int CurrentStatus = objDTO.TransferStatus;

            List<SelectListItem> returnList = new List<SelectListItem>();

            if (Mode.ToLower() == "create")
            {
                returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(TransferStatus.UnSubmitted.ToString()), Value = ((int)TransferStatus.UnSubmitted).ToString() });
            }
            else if (Mode.ToLower() == "edit")
            {
                foreach (var item in Enum.GetValues(typeof(TransferStatus)))
                {
                    string itemText = item.ToString();
                    int itemValue = (int)(Enum.Parse(typeof(TransferStatus), itemText));
                    if (((itemValue <= (int)TransferStatus.Approved && objDTO.RequestType == (int)RequestType.In)
                        || (itemValue <= (int)TransferStatus.Approved && objDTO.RequestType == (int)RequestType.Out)) && itemValue >= CurrentStatus)
                    {
                        if (returnList.FindIndex(x => int.Parse(x.Value) == itemValue) < 0)
                            returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(item.ToString()), Value = itemValue.ToString() });
                    }
                    else if (CurrentStatus == (int)TransferStatus.Transmitted && IsChangeOrder && !objDTO.TransferIsInReceive)
                    {
                        if (returnList.FindIndex(x => int.Parse(x.Value) == (int)TransferStatus.UnSubmitted) < 0)
                        {
                            returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(TransferStatus.Transmitted.ToString()), Value = ((int)TransferStatus.Transmitted).ToString() });
                            break;
                        }
                    }
                }

                if (objDTO.TransferStatus != (int)TransferStatus.Approved && returnList.FindIndex(x => x.Value == ((int)TransferStatus.Approved).ToString()) >= 0 && !IsApprove)
                {
                    returnList.RemoveAt(returnList.FindIndex(x => x.Value == ((int)TransferStatus.Approved).ToString()));
                }

                if (objDTO.TransferStatus != (int)TransferStatus.Submitted && returnList.FindIndex(x => x.Value == ((int)TransferStatus.Submitted).ToString()) >= 0 && !IsSubmit)
                {
                    returnList.RemoveAt(returnList.FindIndex(x => x.Value == ((int)TransferStatus.Submitted).ToString()));
                }


                if (returnList.FindIndex(x => x.Value == ((int)TransferStatus.Approved).ToString()) >= 0)
                {
                    //if (returnList.FindIndex(x => int.Parse(x.Value) == (int)TransferStatus.Rejected) < 0)
                    //    returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(TransferStatus.Rejected.ToString()), Value = ((int)TransferStatus.Rejected).ToString() });

                    if (objDTO.TransferStatus == (int)TransferStatus.Approved && returnList.FindIndex(x => int.Parse(x.Value) == (int)TransferStatus.Transmitted) < 0)
                        returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(TransferStatus.Transmitted.ToString()), Value = ((int)TransferStatus.Transmitted).ToString() });
                }

                if (returnList.FindIndex(x => int.Parse(x.Value) == (int)TransferStatus.Closed) < 0)
                {
                    if (CurrentStatus != (int)TransferStatus.Closed && returnList.FindIndex(x => int.Parse(x.Value) == CurrentStatus) < 0)
                    {
                        string statusName = Enum.GetName(typeof(TransferStatus), CurrentStatus);
                        returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(statusName), Value = (objDTO.TransferStatus).ToString() });
                    }

                    returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(TransferStatus.Closed.ToString()), Value = ((int)TransferStatus.Closed).ToString() });
                }


            }
            else
            {
                foreach (var item in Enum.GetValues(typeof(TransferStatus)))
                {
                    string itemText = item.ToString();
                    int itemValue = (int)(Enum.Parse(typeof(TransferStatus), itemText));
                    returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(item.ToString()), Value = itemValue.ToString() });
                }
            }


            return returnList;
        }

        /// <summary>
        /// IsRecordNotEditable
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        private bool IsRecordNotEditable(TransferMasterDTO objDTO)
        {
            bool isNotEditable = false;

            if (!(IsInsert || IsUpdate || IsDelete || IsSubmit || IsApprove || IsChangeOrder))
            {
                isNotEditable = true;
                return isNotEditable;
            }

            if (objDTO.ID <= 0 && !IsInsert)
            {
                isNotEditable = true;
                return isNotEditable;
            }

            if (IsChangeOrder && !objDTO.TransferIsInReceive && objDTO.TransferStatus == (int)TransferStatus.Transmitted)
            {
                isNotEditable = false;
            }
            else if (IsUpdate || IsSubmit || IsApprove || Convert.ToString(Session["IsInsert"]) == "True" || IsInsert)
            {
                if (IsApprove || IsSubmit)
                    objDTO.IsOnlyStatusUpdate = true;
                else
                    objDTO.IsOnlyStatusUpdate = false;


                if (objDTO.TransferStatus < (int)TransferStatus.Submitted)
                {
                    if (Convert.ToString(Session["IsInsert"]) == "")
                    {
                        if (objDTO.ID > 0 && !IsUpdate)// Edit mode with View only 
                        {
                            isNotEditable = true;
                        }
                    }
                    else if (!IsUpdate && Convert.ToString(Session["IsInsert"]) != "True")
                        isNotEditable = true;
                }
                else if (objDTO.TransferStatus >= (int)TransferStatus.Submitted)
                {
                    if (objDTO.TransferStatus == (int)TransferStatus.Submitted)
                    {
                        if (IsSubmit && !IsApprove)
                            isNotEditable = true;
                        else if (!IsApprove)
                            isNotEditable = true;
                        else if (IsApprove && !IsUpdate)
                        {
                            objDTO.IsOnlyStatusUpdate = true;
                            isNotEditable = true;
                        }
                    }
                    else if (objDTO.TransferStatus > (int)TransferStatus.Submitted)
                    {
                        isNotEditable = true;
                        objDTO.IsOnlyStatusUpdate = false;
                        if (objDTO.TransferStatus == (int)TransferStatus.Approved && IsApprove)
                        {
                            objDTO.IsOnlyStatusUpdate = true;
                        }
                        else if (objDTO.TransferStatus == (int)TransferStatus.Transmitted && IsChangeOrder)
                            isNotEditable = false;
                    }
                }
            }

            return isNotEditable;
        }

        /// <summary>
        /// IsRecordNotEditable
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        private bool IsRecordDeleteable(TransferMasterDTO objDTO)
        {
            bool IsDeleteable = false;

            if (objDTO.TransferStatus < (int)TransferStatus.Approved && IsDelete)
            {
                IsDeleteable = true;
            }



            return IsDeleteable;
        }

        /// <summary>
        /// GetUDFDataPageWise
        /// </summary>
        /// <param name="PageName"></param>
        /// <returns></returns>
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

        private TransferMasterDTO GetNewTransferMasterDTOForOutRequest(TransferMasterDTO objDTO)
        {
            TransferMasterDTO objMasterDTO = null;

            //TransferMasterController TMController = new TransferMasterController();
            TransferMasterDAL TMController = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            TransferMasterDTO objAlreadyTransferDTO = TMController.GetTransferByRefTransferGuidPlain(objDTO.ReplenishingRoomID, objDTO.CompanyID, objDTO.GUID);
            Int64 id = 0;
            if (objAlreadyTransferDTO == null)
            {

                objMasterDTO = new TransferMasterDTO()
                {
                    ID = id,
                    Action = string.Empty,
                    Comment = objDTO.Comment,
                    CompanyID = SessionHelper.CompanyID,
                    Created = DateTimeUtility.DateTimeNow,
                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    GUID = Guid.Empty,
                    HistoryID = 0,
                    IsAbleToDelete = false,
                    IsArchived = false,
                    IsDeleted = false,
                    IsHistory = false,
                    IsOnlyStatusUpdate = false,
                    IsRecordNotEditable = false,
                    LastUpdatedBy = SessionHelper.UserID,
                    RefTransferGUID = objDTO.GUID,
                    RefTransferNumber = objDTO.TransferNumber,
                    TransferNumber = objDTO.TransferNumber,
                    RejectionReason = string.Empty,
                    ReplenishingRoomID = objDTO.RequestingRoomID,
                    ReplenishingRoomName = String.Empty,
                    RequestingRoomID = objDTO.ReplenishingRoomID,
                    RequestingRoomName = string.Empty,
                    RequestType = (int)RequestType.In,
                    RequestTypeName = "In",
                    RequireDate = objDTO.RequireDate,
                    RoomID = objDTO.ReplenishingRoomID,
                    RoomName = String.Empty,
                    StagingID = 0,
                    StagingName = string.Empty,
                    TransferDetailList = null,
                    TransferIsInReceive = false,
                    TransferStatus = objDTO.TransferStatus,
                    TransferStatusName = string.Empty,
                    UDF1 = string.Empty,
                    UDF2 = string.Empty,
                    UDF3 = string.Empty,
                    UDF4 = string.Empty,
                    UDF5 = string.Empty,
                    Updated = DateTimeUtility.DateTimeNow,
                    UpdatedByName = SessionHelper.UserName
                };

                return objMasterDTO;
            }
            else
            {
                objAlreadyTransferDTO.Comment = objDTO.Comment;
                objAlreadyTransferDTO.Updated = DateTimeUtility.DateTimeNow;
                objAlreadyTransferDTO.LastUpdatedBy = SessionHelper.UserID;
                objAlreadyTransferDTO.RequireDate = objDTO.RequireDate;
                return objAlreadyTransferDTO;
            }
        }

        private TransferMasterDTO GetNewTransferMasterDTOForInRequest(TransferMasterDTO objDTO)
        {
            TransferMasterDTO objMasterDTO = null;
            //TransferMasterController TMController = new TransferMasterController();
            TransferMasterDAL TMController = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            TransferMasterDTO objAlreadyTransferDTO = TMController.GetTransferByRefTransferGuidPlain(objDTO.ReplenishingRoomID, objDTO.CompanyID, objDTO.GUID);

            if (objAlreadyTransferDTO == null)
            {
                objMasterDTO = new TransferMasterDTO()
                {
                    ID = 0,
                    Action = string.Empty,
                    Comment = objDTO.Comment,
                    CompanyID = SessionHelper.CompanyID,
                    Created = DateTimeUtility.DateTimeNow,
                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    GUID = Guid.Empty,
                    HistoryID = 0,
                    IsAbleToDelete = false,
                    IsArchived = false,
                    IsDeleted = false,
                    IsHistory = false,
                    IsOnlyStatusUpdate = false,
                    IsRecordNotEditable = false,
                    LastUpdatedBy = SessionHelper.UserID,
                    RefTransferGUID = objDTO.GUID,
                    RefTransferNumber = objDTO.TransferNumber,
                    TransferNumber = objDTO.TransferNumber,
                    RejectionReason = string.Empty,
                    ReplenishingRoomID = objDTO.RequestingRoomID,
                    ReplenishingRoomName = String.Empty,
                    RequestingRoomID = objDTO.ReplenishingRoomID,
                    RequestingRoomName = string.Empty,
                    RequestType = (int)RequestType.Out,
                    RequestTypeName = "Out",
                    RequireDate = objDTO.RequireDate,
                    RoomID = objDTO.ReplenishingRoomID,
                    RoomName = String.Empty,
                    StagingID = 0,
                    StagingName = string.Empty,
                    TransferDetailList = null,
                    TransferIsInReceive = false,
                    TransferStatus = (int)TransferStatus.UnSubmitted,
                    TransferStatusName = string.Empty,
                    UDF1 = string.Empty,
                    UDF2 = string.Empty,
                    UDF3 = string.Empty,
                    UDF4 = string.Empty,
                    UDF5 = string.Empty,
                    Updated = DateTimeUtility.DateTimeNow,
                    UpdatedByName = SessionHelper.UserName
                };

                return objMasterDTO;
            }
            else
            {
                objAlreadyTransferDTO.Comment = objDTO.Comment;
                objAlreadyTransferDTO.Updated = DateTimeUtility.DateTimeNow;
                objAlreadyTransferDTO.LastUpdatedBy = SessionHelper.UserID;
                objAlreadyTransferDTO.RequireDate = objDTO.RequireDate;
                return objAlreadyTransferDTO;
            }
        }

        private List<TransferDetailDTO> GetNewTransferDetailDTOListForOutRequest(TransferMasterDTO objDTO, List<TransferDetailDTO> objDetailDTO)
        {
            List<TransferDetailDTO> lstNewDetailDTO = new List<TransferDetailDTO>();

            //TransferDetailController transerDetailController = new TransferDetailController();
            TransferDetailDAL transerDetailController = new TransferDetailDAL(SessionHelper.EnterPriseDBName);

            List<TransferDetailDTO> lstAlreadyTransferDetailList = transerDetailController.GetTransferDetailNormal(objDTO.GUID, objDTO.RoomID, objDTO.CompanyID, SessionHelper.UserSupplierIds);
            if (lstAlreadyTransferDetailList != null && lstAlreadyTransferDetailList.Count > 0)
            {
                //IEnumerable<ItemMasterDTO> lstItems = new ItemMasterController().GetAllRecord(objDTO.ReplenishingRoomID, SessionHelper.CompanyID);
                //IEnumerable<ItemMasterDTO> lstItems = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(objDTO.ReplenishingRoomID, SessionHelper.CompanyID);
                foreach (var item in lstAlreadyTransferDetailList)
                {
                    //ItemMasterDTO itm = lstItems.Where(x => x.ItemNumber == item.ItemDetail.ItemNumber).SingleOrDefault();
                    //TransferDetailDTO DetailDTO = objDetailDTO.SingleOrDefault(x => x.ItemGUID == itm.GUID);
                    TransferDetailDTO DetailDTO = objDetailDTO.SingleOrDefault(x => x.ItemNumber == item.ItemNumber);

                    item.RequestedQuantity = DetailDTO.RequestedQuantity;
                    item.RequiredDate = DetailDTO.RequiredDate;
                    item.IsDeleted = DetailDTO.IsDeleted;
                    item.IsArchived = DetailDTO.IsArchived;
                    item.LastUpdated = DateTimeUtility.DateTimeNow;
                    item.LastUpdatedBy = SessionHelper.UserID;
                    lstNewDetailDTO.Add(item);

                }

                return lstNewDetailDTO;
            }
            else
            {
                //IEnumerable<ItemMasterDTO> lstItems = new ItemMasterController().GetAllRecord(objDTO.RoomID, SessionHelper.CompanyID);
                IEnumerable<ItemMasterDTO> lstItems = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetAllItemsPlain(objDTO.RoomID, SessionHelper.CompanyID);
                foreach (var item in objDetailDTO)
                {
                    //ItemMasterDTO itm = lstItems.Where(x => x.ItemNumber == item.ItemDetail.ItemNumber).SingleOrDefault();
                    ItemMasterDTO itm = lstItems.Where(x => x.ItemNumber == item.ItemNumber).SingleOrDefault();
                    TransferDetailDTO objDDTO = new TransferDetailDTO()
                    {
                        Action = string.Empty,
                        Bin = 0,
                        BinName = string.Empty,
                        CompanyID = SessionHelper.CompanyID,
                        Created = DateTimeUtility.DateTimeNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = string.Empty,
                        FulFillQuantity = 0,
                        GUID = Guid.Empty,
                        HistoryID = 0,
                        ID = 0,
                        IntransitQuantity = 0,
                        IsArchived = false,
                        IsDeleted = false,
                        IsHistory = false,
                        //ItemDetail = null,
                        ItemGUID = itm.GUID,
                        LastUpdated = DateTimeUtility.DateTimeNow,
                        LastUpdatedBy = SessionHelper.UserID,
                        ReceivedQuantity = 0,
                        RequestedQuantity = item.RequestedQuantity,
                        RequiredDate = item.RequiredDate,
                        Room = objDTO.RoomID,
                        RoomName = string.Empty,
                        ShippedQuantity = 0,
                        TransferGUID = objDTO.GUID,
                        UpdatedByName = string.Empty

                    };
                    lstNewDetailDTO.Add(objDDTO);
                }

                return lstNewDetailDTO;
            }
        }

        private List<TransferDetailDTO> GetNewTransferDetailDTOListForInRequest(TransferMasterDTO objDTO, List<TransferDetailDTO> objDetailDTO)
        {
            List<TransferDetailDTO> lstNewDetailDTO = new List<TransferDetailDTO>();
            //TransferDetailController transerDetailController = new TransferDetailController();
            TransferDetailDAL transerDetailController = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            List<TransferDetailDTO> lstAlreadyTransferDetailList = transerDetailController.GetTransferDetailNormal(objDTO.GUID, objDTO.RoomID, objDTO.CompanyID, SessionHelper.UserSupplierIds);
            if (lstAlreadyTransferDetailList != null && lstAlreadyTransferDetailList.Count > 0)
            {
                //IEnumerable<ItemMasterDTO> lstItems = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(objDTO.ReplenishingRoomID, SessionHelper.CompanyID);
                foreach (var item in lstAlreadyTransferDetailList)
                {

                    //TransferDetailDTO DetailDTO = objDetailDTO.SingleOrDefault(x => x.ItemDetail.ItemNumber == item.ItemDetail.ItemNumber);
                    TransferDetailDTO DetailDTO = objDetailDTO.SingleOrDefault(x => x.ItemNumber == item.ItemNumber);

                    item.RequestedQuantity = DetailDTO.RequestedQuantity;
                    item.RequiredDate = DetailDTO.RequiredDate;
                    item.IsDeleted = DetailDTO.IsDeleted;
                    item.IsArchived = DetailDTO.IsArchived;
                    item.LastUpdated = DateTimeUtility.DateTimeNow;
                    item.LastUpdatedBy = SessionHelper.UserID;
                    lstNewDetailDTO.Add(item);

                }

                return lstNewDetailDTO;
            }
            else
            {
                //IEnumerable<ItemMasterDTO> lstItems = new ItemMasterController().GetAllRecord(objDTO.RoomID, SessionHelper.CompanyID);
                IEnumerable<ItemMasterDTO> lstItems = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetAllItemsPlain(objDTO.RoomID, SessionHelper.CompanyID);
                foreach (var item in objDetailDTO)
                {
                    ItemMasterDTO itm = lstItems.Where(x => x.ItemNumber == item.ItemNumber).SingleOrDefault();
                    TransferDetailDTO objDDTO = new TransferDetailDTO()
                    {
                        Action = string.Empty,
                        Bin = 0,
                        BinName = string.Empty,
                        CompanyID = SessionHelper.CompanyID,
                        Created = DateTimeUtility.DateTimeNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = string.Empty,
                        FulFillQuantity = 0,
                        GUID = Guid.Empty,
                        HistoryID = 0,
                        ID = 0,
                        IntransitQuantity = 0,
                        IsArchived = false,
                        IsDeleted = false,
                        IsHistory = false,
                        //ItemDetail = null,
                        ItemGUID = itm.GUID,
                        LastUpdated = DateTimeUtility.DateTimeNow,
                        LastUpdatedBy = SessionHelper.UserID,
                        ReceivedQuantity = 0,
                        RequestedQuantity = item.RequestedQuantity,
                        RequiredDate = item.RequiredDate,
                        Room = objDTO.RoomID,
                        RoomName = string.Empty,
                        ShippedQuantity = 0,
                        TransferGUID = objDTO.GUID,
                        UpdatedByName = string.Empty

                    };
                    lstNewDetailDTO.Add(objDDTO);
                }

                return lstNewDetailDTO;
            }
        }

        //private static void AddExternalUser(List<UserMasterDTO> objUsers, string EmailTemplate)
        //{

        //    EmailUserConfigurationDAL objExternalUser = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
        //    IEnumerable<EmailUserConfigurationDTO> lstExternalUser = objExternalUser.GetAllExternalUserRecords(EmailTemplate, true, SessionHelper.RoomID, SessionHelper.CompanyID);
        //    if (lstExternalUser != null)
        //    {
        //        foreach (EmailUserConfigurationDTO item in lstExternalUser)
        //        {
        //            UserMasterDTO objExtUser = new UserMasterDTO();
        //            objExtUser.Email = item.Email;
        //            objUsers.Add(objExtUser);
        //        }
        //    }
        //}

        /// <summary>
        /// Send Mail to Approval authority for Approve Order
        /// </summary>
        /// <param name="strToEmailAddress"></param>
        /// <param name="transferDTO"></param>
        public void SendMailToApprovalAuthority(TransferMasterDTO transferDTO)
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            eTurnsWeb.Helper.AlertMail objAlertMail = new Helper.AlertMail();
            //eTurnsUtility objUtils = null;
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            //eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
            NotificationDAL objNotificationDAL = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            try
            {

                objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.TransferApproval, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

                lstNotifications.ForEach(t =>
                {
                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                if (lstNotificationsImidiate.Count > 0)
                {
                    lstNotificationsImidiate.ForEach(t =>
                    {
                        string StrSubject = string.Empty;
                        string strToAddress = t.EmailAddress;
                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            //string strCCAddress = "";
                            MessageBody = new StringBuilder();
                            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                            }
                            if (objEmailTemplateDetailDTO != null)
                            {
                                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                StrSubject = objEmailTemplateDetailDTO.MailSubject;
                            }
                            else
                            {
                                return;
                            }

                            MessageBody.Replace("@@ORDERNO@@", transferDTO.TransferNumber);
                            MessageBody.Replace("@@TRANSFERNO@@", transferDTO.TransferNumber);
                            MessageBody.Replace("@@TABLE@@", GetMailBody(transferDTO));

                            MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                            MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                            MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);

                            string replacePart = string.Empty;
                            if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                            {
                                replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                            }
                            else if (Request == null)
                            {
                                replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                            }
                            else
                            {
                                string urlPart = Request.Url.ToString();
                                replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                            }

                            Dictionary<string, string> Params = new Dictionary<string, string>();
                            Params.Add("DataGuids", transferDTO.GUID.ToString());
                            lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);

                            if (!string.IsNullOrWhiteSpace(strToAddress))
                            {
                                List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (EmailAddrs != null && EmailAddrs.Count > 0)
                                {
                                    foreach (var emailitem in EmailAddrs)
                                    {
                                        // string strdata = transferDTO.ID + "^" + transferDTO.RoomID + "^" + transferDTO.CompanyID + "^" + arrUsers[0].ID + "^" + SessionHelper.EnterPriceID.ToString() + "^" + transferDTO.LastUpdatedBy;
                                        string strdata = transferDTO.ID + "^" + transferDTO.RoomID + "^" + transferDTO.CompanyID + "^" + SessionHelper.EnterPriceID.ToString() + "^" + transferDTO.LastUpdatedBy + "^" + emailitem;
                                        string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
                                        string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");
                                        MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/TransferStatus?eKey=" + approvalURLData + "'>'" + ResCommon.Approve + "'</a> &nbsp;&nbsp;<a href='" + replacePart + "/EmailLink/TransferStatus?eKey=" + rejectURLData + "'>'" + ResCommon.Reject + "'</a>");
                                        objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), emailitem, t, objEnterpriseDTO);
                                    }
                                }
                            }
                        }
                    });
                }


            }
            finally
            {
                //objUtils = null;
                //objEmailDAL = null;
                MessageBody = null;
                objNotificationDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }

        public void SendMailForApprovedOrRejected(TransferMasterDTO objTransfer, string AprvRejString)
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            eTurnsWeb.Helper.AlertMail objAlertMail = new Helper.AlertMail();
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            NotificationDAL objNotificationDAL = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            try
            {

                objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.TransferApproveReject, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

                lstNotifications.ForEach(t =>
                {
                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                if (lstNotificationsImidiate.Count > 0)
                {
                    lstNotificationsImidiate.ForEach(t =>
                    {
                        string StrSubject = string.Empty;
                        string strToAddress = t.EmailAddress;
                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            //string strCCAddress = "";
                            MessageBody = new StringBuilder();
                            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                            {
                                objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                            }
                            if (objEmailTemplateDetailDTO != null)
                            {
                                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                StrSubject = objEmailTemplateDetailDTO.MailSubject;
                            }
                            else
                            {
                                return;
                            }

                            MessageBody.Replace("@@APRVREJ@@", AprvRejString);
                            MessageBody.Replace("@@TRANSFERNO@@", objTransfer.TransferNumber);
                            MessageBody.Replace("@@ORDERNO@@", objTransfer.TransferNumber);

                            MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                            MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                            MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);

                            string replacePart = string.Empty;
                            if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                            {
                                replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                            }
                            else if (Request == null)
                            {
                                replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                            }
                            else
                            {
                                string urlPart = Request.Url.ToString();
                                replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                            }

                            Dictionary<string, string> Params = new Dictionary<string, string>();
                            Params.Add("DataGuids", objTransfer.GUID.ToString());
                            lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                            objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                        }

                    });
                }


            }
            finally
            {
                //objUtils = null;
                //objEmailDAL = null;
                MessageBody = null;
                objNotificationDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }


        //private void SendMailForApproval(List<UserMasterDTO> lstUserMasterDTO, TransferMasterDTO objOrder)
        //{
        //    StringBuilder MessageBody = null;
        //    EmailTemplateDAL objEmailTemplateDAL = null;
        //    EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
        //    //eTurnsUtility objUtils = null;
        //    eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
        //    try
        //    {
        //        string StrSubject = "Transfer Approval Request";
        //        string strToAddress = CommonUtility.GetEmailToAddress(lstUserMasterDTO, "TransferApproval");
        //        string strCCAddress = "";

        //        if (!string.IsNullOrEmpty(strToAddress))
        //        {
        //            MessageBody = new StringBuilder();
        //            objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
        //            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
        //            objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("TransferApproval", ResourceHelper.CurrentCult.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
        //            if (objEmailTemplateDetailDTO != null)
        //            {
        //                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
        //                StrSubject = objEmailTemplateDetailDTO.MailSubject;
        //            }
        //            else
        //            {
        //                return;
        //            }
        //            //---------------------------------------------
        //            MessageBody.Replace("@@ORDERNO@@", objOrder.TransferNumber);
        //            MessageBody.Replace("@@TABLE@@", GetMailBody(objOrder));
        //            MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
        //            MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
        //            MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
        //            //MessageBody.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));
        //            string urlPart = Request.Url.ToString();
        //            string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];

        //            string strdata = objOrder.ID + "^" + objOrder.RoomID + "^" + objOrder.CompanyID + "^" + lstUserMasterDTO[0].ID + "^" + SessionHelper.EnterPriceID.ToString() + "^" + objOrder.LastUpdatedBy;
        //            string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
        //            string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");
        //            MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/TransferStatus?eKey=" + approvalURLData + "'>Approve</a> &nbsp;&nbsp;<a href='" + replacePart + "/EmailLink/TransferStatus?eKey=" + rejectURLData + "'>Reject</a>");
        //            //objUtils = new eTurnsUtility();
        //            //objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
        //            objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
        //            objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, null, "Web => Transfer => SendMailForApproval");
        //        }
        //    }
        //    finally
        //    {
        //        MessageBody = null;
        //        objEmailTemplateDAL = null;
        //        objEmailTemplateDetailDTO = null;
        //        //objUtils = null;
        //        objEmailDAL = null;
        //    }
        //}
        private StringBuilder EmailTemplateReplaceURL(string EmailTemplateName)
        {
            StringBuilder MessageBody = new StringBuilder();
            if (System.IO.File.Exists(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName))
            {
                MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName));
            }

            string urlPart = Request.Url.ToString();
            string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];

            //MessageBody = MessageBody.Replace("../CommonImages/logo.gif", replacePart + "CommonImages/logo.gif");
            if (Request.ApplicationPath != "/")
                MessageBody = MessageBody.Replace("src=\"", "src=\"" + replacePart + Request.ApplicationPath);
            else
                MessageBody = MessageBody.Replace("src=\"", "src=\"" + replacePart);
            return MessageBody;
        }
        public string GetMailBody(TransferMasterDTO obj)
        {
            string mailBody = "";

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + ResTransfer.TransferNumber + @": </label>
                                    <label style=""font-weight: bold;"">
                                        " + obj.TransferNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResTransfer.Comment + @": </label>
                                    <label>
                                        " + obj.Comment + @"</label>
                                </td>
                            </tr>                           
                        </table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + ResTransfer.RequireDate + @": </label>
                                    <label>
                                        " + obj.RequireDate.ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture) + @"</label>
                                </td>
                            </tr>                            
                            <tr>
                                <td>
                                    <label>
                                        " + ResTransfer.TransferStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(TransferStatus), obj.TransferStatus.ToString()).ToString() + @"</label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.ItemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResOrder.Bin + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResOrder.RequestedQuantity + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResOrder.RequiredDate + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
            ##TRS##
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
            ";
            string trs = "";
            if (obj.TransferDetailList == null || obj.TransferDetailList.Count <= 0)
            {
                TransferDetailDAL objTransferDetailDAL = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
                obj.TransferDetailList = objTransferDetailDAL.GetTransferDetailNormal(obj.GUID, obj.RoomID, obj.CompanyID, SessionHelper.UserSupplierIds);
            }

            if (obj.TransferDetailList != null && obj.TransferDetailList.Count > 0)
            {

                foreach (var item in obj.TransferDetailList)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.Bin != null && item.Bin > 0)
                    {
                        //binname = new BinMasterController().GetRecord(Int64.Parse(Convert.ToString(item.Bin)), SessionHelper.RoomID, SessionHelper.CompanyID, false, false).BinNumber;
                        binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(item.Bin)), SessionHelper.RoomID, SessionHelper.CompanyID).BinNumber;
                        //binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.Bin)),null,null).FirstOrDefault().BinNumber;
                    }
                    //if (item.RequestedQuantity != null)
                    ReqQty = item.RequestedQuantity.ToString();

                    if (item.RequiredDate != null)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);
                    //ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.CompanyConfig.DateFormatCSharp);


                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                        <td>
                            " + binname + @"
                        </td>
                        <td>
                            " + ReqQty + @"
                        </td>
                        <td>
                            " + ReqDate + @"
                        </td>
                    </tr>";
                }
            }
            else
            {
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           There is no item for this order
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }
        #endregion

        #region Transfer Detail

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult LoadTransferLineItems(Int64 TransferID)
        {
            TransferMasterDTO objDTO = null;
            IEnumerable<TransferDetailDTO> lstTransferDetails = null;
            ItemMasterDAL itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            if (TransferID <= 0)
                objDTO = new TransferMasterDTO() { ID = TransferID, TransferDetailList = new List<TransferDetailDTO>() };
            else
            {
                objDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferByIdNormal(TransferID);
                //IEnumerable<TransferDetailDTO> lstTransferDetails =   new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.TransferGUID == objDTO.GUID);
                lstTransferDetails = new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetTransferedRecord(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds);
                if (lstTransferDetails != null && lstTransferDetails.Count() > 0)
                {
                    objDTO.TransferDetailList = lstTransferDetails.ToList();
                    objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
                    if (objDTO.IsDeleted.GetValueOrDefault(false) || objDTO.IsArchived.GetValueOrDefault(false))
                    {
                        objDTO.IsRecordNotEditable = true;
                        objDTO.IsOnlyStatusUpdate = false;
                        objDTO.IsAbleToDelete = true;
                    }

                    if (objDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        objDTO.IsRecordNotEditable = true;
                        objDTO.IsAbleToDelete = false;
                        if (objDTO.TransferStatus < (int)TransferStatus.Transmitted)
                            objDTO.IsOnlyStatusUpdate = true;
                    }
                }
            }
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            string ItemGUIDs = string.Empty;
            if (lstTransferDetails != null && lstTransferDetails.Count() > 0)
            {
                ItemGUIDs = string.Join(",", lstTransferDetails.Select(t => t.ItemGUID).ToArray());
            }
            if (!string.IsNullOrWhiteSpace(ItemGUIDs))
            {
                lstBins = objBinMasterDAL.GetInventoryAndStagingBinsByItemGUIDs(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUIDs);
            }
            if (objDTO.StagingID.GetValueOrDefault(0) <= 0)
            {
                //lstBins = new BinMasterController().GetAllRecord(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.IsStagingLocation == false).ToList();
                lstBins = lstBins.Where(x => x.IsStagingLocation == false).ToList();
                //lstBins = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, false).ToList();//.Where(x => x.IsStagingLocation == false).ToList();
            }
            else
            {
                lstBins = lstBins.Where(x => x.IsStagingLocation == true).ToList();
                //lstBins = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, true).ToList();
            }

            lstBins.Insert(0, new BinMasterDTO());
            ViewBag.BinList = lstBins;

            if (lstTransferDetails != null && lstTransferDetails.Count() > 0)
            {
                var isFetchMaxQtyBinForItem = (objDTO.RequestType == (int)RequestType.Out &&  (objDTO.TransferStatus != (int)TransferStatus.Closed));
                var transferStatus = objDTO.TransferStatus;
                foreach (var item in lstTransferDetails)
                {
                    if (isFetchMaxQtyBinForItem && (transferStatus < (int)TransferStatus.TransmittedIncomplete || (transferStatus >= (int)TransferStatus.TransmittedIncomplete 
                        && item.FulFillQuantity.GetValueOrDefault(0) < item.ApprovedQuantity.GetValueOrDefault(0) )))
                    {
                            item.Bin = itemMasterDAL.GetMaxQtyBinIdByItemGuid(item.ItemGUID);
                            item.BinName = lstBins.Any(x => x.ID == item.Bin) ? lstBins.FirstOrDefault(x => x.ID == item.Bin).BinNumber : string.Empty;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.BinName) && item.Bin.GetValueOrDefault(0) <= 0)
                        {
                            item.Bin = item.DefaultLocation;
                            item.BinName = lstBins.Any(x => x.ID == item.DefaultLocation) ? lstBins.FirstOrDefault(x => x.ID == item.DefaultLocation).BinNumber : string.Empty;
                        }
                    }
                    
                }
            }


            return PartialView("_TransferLineItems", objDTO);
        }

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult LoadItemMasterModel(Int64 ParentId)
        {
            TransferMasterDTO objDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferByIdPlain(ParentId);
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Transfer/AddItemToDetailTable/",
                PerentID = ParentId.ToString(),
                PerentGUID = objDTO.GUID.ToString(),
                ModelHeader = eTurns.DTO.ResProjectMaster.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/Transfer/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/Transfer/GetItemsModelMethod/",
                CallingFromPageName = "TRF",
                OrdStagingID = objDTO.StagingID.ToString(),
                OrdRequeredDate = objDTO.RequireDate.ToString("MM/dd/yyyy"),
                TransferRequestType = (RequestType)objDTO.RequestType,
            };

            //------------------------------------Save ItemList In Session For Narrow Search Start------------------------------------
            //
            ItemMasterDAL objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            Int64 TRFID = 0;
            Int64.TryParse(Request["ParentID"], out TRFID);
            TransferMasterDTO objMasterDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferByIdPlain(TRFID);
            List<TransferDetailDTO> lstItems = new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetTransferedRecord(objMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds);
            string ItemsIDs = "";
            string ExclueBinMasterGUIDs = "";

            if (lstItems != null && lstItems.Count > 0)
            {
                ExclueBinMasterGUIDs = String.Join(",", lstItems.Select(x => (x.Bin == null ? "" : x.Bin.Value.ToString())));
            }
            // commented for WI-4854: Allow adding the same item to a transfer more than once
            //if (objMasterDTO.RequestType == (int)RequestType.Out && lstItems != null && lstItems.Count > 0)
            //    ItemsIDs = string.Join(",", lstItems.Select(x => x.ItemGUID.ToString()).ToArray());

            string stagingGuid = "";
            if (objMasterDTO.StagingID.GetValueOrDefault(0) > 0)
                stagingGuid = objMasterDTO.StagingID.GetValueOrDefault(0).ToString();

            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = objItemMaster.GetItemsToAddTransferDetail_Paging(0, int.MaxValue, out TotalRecordCount, "", "ItemNumber Asc", SessionHelper.RoomID,
                SessionHelper.CompanyID, objMasterDTO.ReplenishingRoomID, objMasterDTO.RequestType, SessionHelper.UserSupplierIds, SessionHelper.UserID, ItemsIDs, ExclueBinMasterGUIDs, stagingGuid, RoomDateFormat, CurrentTimeZone);


            Session["TransferItemMasterList"] = DataFromDB.ToList();

            return PartialView("ItemMasterModel", obj);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetItemsModelMethod(QuickListJQueryDataTableParamModel param)
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
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

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            Int64 TRFID = 0;
            Int64.TryParse(Request["ParentID"], out TRFID);
            TransferMasterDTO objMasterDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferByIdPlain(TRFID);
            List<TransferDetailDTO> lstItems = new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetTransferedRecord(objMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds);
            string ItemsIDs = "";
            string ExclueBinMasterGUIDs = "";

            if (lstItems != null && lstItems.Count > 0)
            {
                ExclueBinMasterGUIDs = String.Join(",", lstItems.Select(x => (x.Bin == null ? "" : x.Bin.Value.ToString())));
            }
            string stagingGuid = "";
            if (objMasterDTO.StagingID.GetValueOrDefault(0) > 0)
                stagingGuid = objMasterDTO.StagingID.GetValueOrDefault(0).ToString();
            // commented for WI-4854: Allow adding the same item to a transfer more than once
            //if (objMasterDTO.RequestType == (int)RequestType.Out && lstItems != null && lstItems.Count > 0)
            //    ItemsIDs = string.Join(",", lstItems.Select(x => x.ItemGUID.ToString()).ToArray());

            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetItemsToAddTransferDetail_Paging(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID,
              SessionHelper.CompanyID, objMasterDTO.ReplenishingRoomID, objMasterDTO.RequestType, SessionHelper.UserSupplierIds, SessionHelper.UserID, ItemsIDs, ExclueBinMasterGUIDs, stagingGuid, RoomDateFormat, CurrentTimeZone);

            foreach (var item in DataFromDB)
            {
                List<DTOForAutoComplete> locations = GetBinsOfItemByOrderId(lstItems, string.Empty, string.Empty, item.GUID.ToString(), false, false);
                item.BinAutoComplete = new List<BinAutoComplete>();
                foreach (var binlist in locations)
                {
                    BinAutoComplete bin = new BinAutoComplete();
                    bin.ID = binlist.ID;
                    bin.BinNumber = binlist.Key;
                    item.BinAutoComplete.Add(bin);
                }
            }

            return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBinsOfItemByTransferId(string StagingName, string NameStartWith, string ItemGUID, bool QtyRequired = false, bool OnlyHaveQty = false, Int64 OrderId = 0, bool? IsLoadMoreLocations = false)
        {
            List<string> dtoList = new List<string>();
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();
            TransferMasterDAL transferMasterDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            TransferDetailDAL transferDetailDAL = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            TransferMasterDTO transferMasterDTO = null;
            bool IsStagingLocations = false;
            if (OrderId > 0)
            {
                transferMasterDTO = transferMasterDAL.GetTransferByIdPlain(OrderId);
                if (transferMasterDTO != null && transferMasterDTO.StagingID.GetValueOrDefault(0) > 0)
                    IsStagingLocations = true;
            }

            if (QtyRequired == true && !IsLoadMoreLocations.GetValueOrDefault(false))
            {
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => x.IsStagingLocation == IsStagingLocations && x.BinNumber != "[|EmptyStagingBin|]").OrderBy(x => x.BinNumber);
                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, IsStagingLocations, string.Empty,null,false).OrderBy(x => x.BinNumber);

                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.BinNumber).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                    foreach (var item in objBinDTOList)
                    {
                        ItemLocationQTYDAL objLocationQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                        ItemLocationQTYDTO objLocatQtyDTO = objLocationQtyDAL.GetRecordByBinItem(Guid.Parse(ItemGUID), item.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")";
                            objAutoDTO.Value = item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")";
                            objAutoDTO.ID = item.ID;

                            locations.Add(objAutoDTO);
                        }
                        else
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                            {
                                Key = item.BinNumber + " (0)",
                                Value = item.BinNumber + " (0)",
                                ID = item.ID,
                                GUID = item.GUID,
                            };
                            locations.Add(objAutoDTO);
                        }
                        locations.Add(new DTOForAutoComplete() { GUID = item.GUID });
                    }

                }
            }
            else if (QtyRequired == false)
            {
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => x.IsStagingLocation == IsStagingLocations && x.BinNumber != "[|EmptyStagingBin|]").OrderBy(x => x.BinNumber);
                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, IsStagingLocations, string.Empty, null, false).OrderBy(x => x.BinNumber);

                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.BinNumber).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                    foreach (var item in objBinDTOList)
                    {
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.Key = item.BinNumber;
                        objAutoDTO.Value = item.BinNumber;
                        objAutoDTO.ID = item.ID;
                        objAutoDTO.ItemGuid = item.ItemGUID;
                        locations.Add(objAutoDTO);
                    }
                }
            }

            if (IsLoadMoreLocations.HasValue && !QtyRequired)
            {
                if (IsLoadMoreLocations.Value)
                {
                    locations = new List<DTOForAutoComplete>();
                    //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), IsStagingLocations).Where(x => x.BinNumber != "[|EmptyStagingBin|]").OrderBy(x => x.BinNumber);
                    IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), IsStagingLocations, string.Empty, null, false).OrderBy(x => x.BinNumber);

                    if (objBinDTOList != null && objBinDTOList.Count() > 0)
                    {
                        foreach (var item in objBinDTOList)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item.BinNumber;
                            objAutoDTO.Value = item.BinNumber;
                            objAutoDTO.ID = item.ID;
                            objAutoDTO.ItemGuid = item.ItemGUID;
                            locations.Add(objAutoDTO);
                        }
                    }
                }
                else
                {
                    DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                    objAutoDTO.Key = ResBin.MoreLocations;
                    objAutoDTO.Value = ResBin.MoreLocations;
                    locations.Add(objAutoDTO);
                }
            }


            if (transferMasterDTO != null)
            {
                List<TransferDetailDTO> lstDetails = transferDetailDAL.GetTransferedRecord(transferMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds);
                if (lstDetails != null && lstDetails.Count > 0)
                {
                    foreach (TransferDetailDTO o in lstDetails)
                    {
                        if (o.ItemGUID.ToString() == ItemGUID)
                            locations.RemoveAll(a => a.ID == o.Bin);

                    }
                }
            }

            return Json(locations, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public List<DTOForAutoComplete> GetBinsOfItemByOrderId(List<TransferDetailDTO> lstDetails, string StagingName, string NameStartWith, string ItemGUID, bool QtyRequired = false, bool OnlyHaveQty = false, Int64 OrderId = 0)
        {
            List<string> dtoList = new List<string>();
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();

            if (QtyRequired == false)
            {
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber);
                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, false, string.Empty, null, null).OrderBy(x => x.BinNumber);

                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.BinNumber).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                    foreach (var item in objBinDTOList)
                    {
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.Key = item.BinNumber;
                        objAutoDTO.Value = item.BinNumber;
                        objAutoDTO.ID = item.ID;

                        locations.Add(objAutoDTO);
                    }

                    if (lstDetails != null && lstDetails.Count > 0)
                    {
                        foreach (TransferDetailDTO o in lstDetails)
                        {
                            if (o.ItemGUID.ToString() == ItemGUID)
                                locations.RemoveAll(a => a.ID == o.Bin);
                        }
                    }
                }
            }

            return locations;
        }


        /// <summary>
        /// AddDetailItem
        /// </summary>
        /// <param name="para"></param>
        /// <param name="ItemID"></param>
        /// <param name="ItemGUID"></param>
        /// <param name="pQuentity"></param>
        /// <param name="QuickListID"></param>
        /// <param name="QuickListGuid"></param>
        /// <returns></returns>
        public JsonResult AddItemToDetailTable(string para)
        {
            string message = "";
            string status = "";
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                TransferDetailDTO[] TRFDetails = s.Deserialize<TransferDetailDTO[]>(para);
                TransferDetailDAL objApi = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
                TransferMasterDAL objDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                TransferMasterDTO MasterDTO = objDAL.GetTransferByGuidPlain(TRFDetails[0].TransferGUID);
                ////TransferMasterDTO ReplinishDTO = null;

                //if (MasterDTO != null && MasterDTO.TransferStatus == (int)TransferStatus.Transmitted)
                //{
                //    ReplinishDTO = objDAL.GetRecordByRefTransferID(MasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                //}

                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
                bool IsStagingLocation = false;
                if (MasterDTO.StagingID.GetValueOrDefault(0) > 0)
                    IsStagingLocation = true;
                long SessionUserId = SessionHelper.UserID;
                ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                var lstItemMaster = ItemDAL.GetAllItemsPlainForTransfer(MasterDTO.ReplenishingRoomID, MasterDTO.CompanyID, false, false);
                var roomDateFormat = SessionHelper.RoomDateFormat;
                var roomCulture = SessionHelper.RoomCulture;
                var roomId = SessionHelper.RoomID;
                var companyId = SessionHelper.CompanyID;
                var username = SessionHelper.UserName;
                var roomName = SessionHelper.RoomName;
                var enterpriseDBName = SessionHelper.EnterPriseDBName;
                var userId = SessionHelper.UserID;
                var binMasterDAL = new BinMasterDAL(enterpriseDBName);
                ItemMasterDAL itemmasterobj = new ItemMasterDAL(enterpriseDBName);
                var datetimeNow = DateTimeUtility.DateTimeNow;
                var enterpriseId = SessionHelper.EnterPriceID;

                foreach (TransferDetailDTO item in TRFDetails)
                {
                    if (!string.IsNullOrWhiteSpace(item.RequiredDateString))
                    {
                        item.RequiredDate = DateTime.ParseExact(item.RequiredDateString, roomDateFormat, roomCulture);
                    }

                    item.Room = roomId;
                    item.RoomName = roomName;
                    item.CreatedBy = userId;
                    item.CreatedByName = username;
                    item.UpdatedByName = username;
                    item.LastUpdatedBy = userId;
                    item.CompanyID = companyId;

                    if (!string.IsNullOrEmpty(item.BinName) && item.Bin.GetValueOrDefault(0) <= 0)
                    {
                        item.Bin = binMasterDAL.GetOrInsertBinIDByName(item.ItemGUID, item.BinName, userId, roomId, companyId, IsStagingLocation);
                    }

                    if (item.ID > 0)
                    {
                        item.ReceivedOn = datetimeNow;
                        item.EditedFrom = "Web";
                        objApi.Edit(item, SessionUserId, enterpriseId);

                    }
                    else
                    {
                        if (MasterDTO.RequestType == 0)
                        {
                            objApi.Insert(item, SessionUserId,false,SessionHelper.EnterPriceID);
                        }
                        else
                        {
                            //List<ItemMasterDTO> lstItemMaster = ItemDAL.GetAllItemsWithJoins(MasterDTO.ReplenishingRoomID, MasterDTO.CompanyID, false, false, null).ToList();

                            if (item.RequiredDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue)
                                item.RequiredDate = datetimetoConsider.AddDays(1);

                            var objItem = ItemDAL.GetItemWithoutJoins(null, item.ItemGUID);

                            if (objItem.LeadTimeInDays.GetValueOrDefault(0) > 0)
                            {
                                item.RequiredDate = datetimetoConsider.AddDays(objItem.LeadTimeInDays.GetValueOrDefault(0));
                            }

                            var ItemBinWithMaxQty = ItemDAL.GetMaxQtyBinIdByItemGuid(item.ItemGUID); // added for WI-5662
                            item.Bin = ItemBinWithMaxQty; //objItem.DefaultLocation;
                            var InRoomItem = lstItemMaster.Where(x => x.ItemNumber == objItem.ItemNumber && (x.IsDeleted ?? false) == false).FirstOrDefault();

                            if (InRoomItem != null && InRoomItem.GUID != Guid.Empty)
                            {
                                //var destinationBinId = ItemDAL.GetDefaultBinByItemGuid(InRoomItemGuid);
                                item.DestinationBinId = InRoomItem.DefaultLocation;
                                objApi.Insert(item, SessionUserId,false,SessionHelper.EnterPriceID);
                            }
                        }
                    }

                    
                    itemmasterobj.EditDate(item.ItemGUID, "EditTransferedDate");
                }

                message = ResCommon.RecordsSavedSuccessfully;
                status = "success";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = "Error";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string TransferLineItemsDelete(string ids)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                TransferDetailDAL obj = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionUserId,SessionHelper.EnterPriceID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Transfer NarrowSearch

        public JsonResult GetNarrowSearchData(bool IsDeleted, bool IsArchived, string TransferStatus)
        {
            string MainFilter = "";

            if (Convert.ToString(Session["MainFilter"]).Trim().ToLower() == "true")
            {
                MainFilter = "true";
            }

            CommonDAL objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
            var tmpsupplierIds = new List<long>();
            NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("TransferMaster", SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, TransferStatus, tmpsupplierIds, MainFilter);

            return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearchData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetOrderNarrwSearchHTML
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTransferNarrwSearchHTML()
        {
            return PartialView("_TransferNarrowSearch");
        }

        #endregion

        #region Received



        /// <summary>
        /// Received
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult FullFillTransfer(Int64 ID)
        {
            TransferMasterDAL obj = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            TransferMasterDTO objDTO = obj.GetTransferByIdFull(ID);
            objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
            ViewBag.UDFs = GetUDFDataPageWise("TransferMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ViewBag.TransferStatusList = GetTransferStatusList(objDTO, "");
            RoomDAL roomCtrl = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<RoomDTO> roomDTOList = roomCtrl.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
            ViewBag.ReplenishingRoom = roomDTOList;
            ViewBag.StagginLocations = new List<BinMasterDTO>();

            return PartialView("_FullFillTransfer", objDTO);
        }

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult LoadTransferLineItemsForFullFill(Int64 TransferID)
        {
            TransferMasterDTO objDTO = null;
            objDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferByIdPlain(TransferID);
            objDTO.TransferDetailList = new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetTransferDetailNormal(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds);
            CommonDAL objCommonCTRL = new CommonDAL(SessionHelper.EnterPriseDBName);
            foreach (var item in objDTO.TransferDetailList)
            {

                List<CommonDTO> binDTO = objCommonCTRL.GetLocationListWithQuntity(item.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (binDTO == null || binDTO.Count < 0)
                {
                    binDTO = new List<CommonDTO>();
                    binDTO.Add(new CommonDTO() { ControlID = "", Count = 0, ID = 0, PageName = "", Text = ResCommon.MsgSelectLocation });

                }
                ViewData["Location_" + item.ItemGUID.ToString()] = binDTO;
            }
            return PartialView("_TransferLineItemsForFullFill", objDTO);

        }

        /// <summary>
        /// MoveInMoveOutQty
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="DefaultLocation"></param>
        /// <param name="isMoveIn"></param>
        /// <returns></returns>
        public ActionResult ShowTransferQtyModel(string DetailID)
        {
            TransferDetailDTO objDetailDTO = new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetTransferDetailsByGuidNormal(Guid.Parse(DetailID));
            CommonDAL objCommonCTRL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<CommonDTO> binDTO = objCommonCTRL.GetLocationListWithQuntity(objDetailDTO.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.Location = binDTO;

            if (objDetailDTO.ItemNumber.Length > 50)
                objDetailDTO.ItemNumber = objDetailDTO.ItemNumber.Substring(0, 47) + "...";

            return PartialView("_TransferQtyModel", objDetailDTO);

        }

        /// <summary>
        /// AddItemQuantityInItemWIP
        /// </summary>
        /// <param name="objMoveQty"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MoveQuantityToInTransit(TakenQtyDetail objMoveQty)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                string status = "fail"; 
                TransferInOutQtyDetailDAL InOutCtrl = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
                ResponseMessage RespMsg = InOutCtrl.TransferQuantity(objMoveQty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionUserId,ResourceHelper.CurrentCult.Name,SessionHelper.EnterPriceID);

                if (RespMsg.IsSuccess)
                    status = "ok";

                return Json(new { Message = RespMsg.Message, Status = status }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// AddItemQuantityInItemWIP
        /// </summary>
        /// <param name="objMoveQty"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ApproveQuantityToInTransit(TakenQtyDetail objMoveQty,string InventoryConsuptionMethod)
        {
            try
            {
                TransferMasterDAL TransferDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                TransferDetailDAL transferDetailDAL = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
                TransferDetailDTO transferOutDetailDTO = transferDetailDAL.GetTransferDetailsByGuidPlain(Guid.Parse(objMoveQty.DetailGUID));
                TransferMasterDTO transferInDTO = null;
                TransferDetailDTO transferInDetailDTO = null;
                TransferInOutQtyDetailDAL objQtyDetailDAL = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
                TransferMasterDTO transferOutDTO = TransferDAL.GetTransferByGuidPlain(transferOutDetailDTO.TransferGUID);
                long SessionUserId = SessionHelper.UserID;

                if (!transferOutDTO.RefTransferGUID.HasValue)
                {
                    transferInDTO = TransferDAL.GetTransferByRefTransferGuidPlain(SessionHelper.RoomID, SessionHelper.CompanyID,transferOutDetailDTO.TransferGUID);

                    if (transferInDTO == null)
                    {
                        transferOutDTO.TransferStatus = (int)TransferStatus.Transmitted;
                        transferOutDTO.EditedFrom = "Web";
                        transferOutDTO.Updated = DateTimeUtility.DateTimeNow;
                        transferOutDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        transferOutDTO.LastUpdatedBy = SessionHelper.UserID;
                        TransferDAL.Edit(transferOutDTO);
                    }
                }
                else
                {
                    //transferInDTO = TransferDAL.GetTransferByGuidPlain(transferOutDTO.RefTransferGUID.Value);
                    transferInDTO = new TransferMasterDTO { GUID = transferOutDTO.RefTransferGUID.Value , RoomID = transferOutDTO.ReplenishingRoomID };
                }

                if (transferOutDetailDTO != null && transferOutDetailDTO.ApprovedQuantity.GetValueOrDefault(0) <= 0 && objMoveQty.TotalQty >= 0)
                {
                    transferOutDetailDTO.ApprovedQuantity = objMoveQty.TotalQty;
                    transferOutDetailDTO.EditedFrom = "Web-QtyApproved";
                    transferOutDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    transferOutDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    transferOutDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                    transferDetailDAL.Edit(transferOutDetailDTO, SessionUserId, SessionHelper.EnterPriceID);
                    double ApprovedQty = transferOutDetailDTO.ApprovedQuantity.GetValueOrDefault(0);
                    Int64 TransferInRoomID = 0;
                    Guid TransferInGUID = Guid.Empty;

                    if (transferInDTO != null && transferInDTO.RoomID > 0)
                    {
                        TransferInRoomID = transferInDTO.RoomID;
                        TransferInGUID = transferInDTO.GUID;
                    }
                    else
                    {
                        TransferInRoomID = transferOutDTO.ReplenishingRoomID;
                        TransferInGUID = transferOutDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty);
                    }

                    var replinishRoomItemGuid = objQtyDetailDAL.GetSameItemByIDInReplinishRoom(new Guid(objMoveQty.ItemGUID), TransferInRoomID, SessionHelper.CompanyID);
                    IEnumerable<TransferDetailDTO> lstTrfDetaiIN = transferDetailDAL.GetTransferDetailsByTrfGuidAndItemGuidPlain(TransferInGUID, replinishRoomItemGuid);

                    if (lstTrfDetaiIN != null && lstTrfDetaiIN.Count() > 0)
                    {
                        foreach (var item in lstTrfDetaiIN)
                        {
                            transferInDetailDTO = lstTrfDetaiIN.FirstOrDefault();

                            if (transferInDetailDTO.ApprovedQuantity.GetValueOrDefault(0) <= 0)
                            {
                                if (ApprovedQty - transferInDetailDTO.RequestedQuantity > 0)
                                {
                                    transferInDetailDTO.ApprovedQuantity = transferInDetailDTO.RequestedQuantity;
                                    ApprovedQty = ApprovedQty - transferInDetailDTO.RequestedQuantity;
                                }
                                else
                                    transferInDetailDTO.ApprovedQuantity = ApprovedQty;
                            }

                            transferInDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                            transferInDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            transferInDetailDTO.EditedFrom = "Web-QtyApproved";

                            transferDetailDAL.Edit(transferInDetailDTO, SessionUserId, SessionHelper.EnterPriceID);
                        }
                    }
                }

                var currentCulture = Convert.ToString(ResourceHelper.CurrentCult);
                var msgQuantityTransferred = ResTransfer.QuantityTransferred;
                var msgQuantityNotAvailableToTransfer = ResTransfer.QuantityNotAvailableToTransfer;
                var msgApprovedQuantityIsZero = ResTransfer.ApprovedQuantityIsZero;
                var msgApprovedQuantityAlreadyTransferred = ResTransfer.ApprovedQuantityAlreadyTransferred;
                string status = "fail";
                TransferInOutQtyDetailDAL InOutCtrl = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
                ResponseMessage RespMsg = InOutCtrl.TransferApprovedQuantity(objMoveQty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionUserId,SessionHelper.EnterPriceID,
                    InventoryConsuptionMethod, currentCulture, msgQuantityTransferred,msgQuantityNotAvailableToTransfer,msgApprovedQuantityIsZero,msgApprovedQuantityAlreadyTransferred);

                if (RespMsg.IsSuccess)
                    status = "ok";

                return Json(new { Message = RespMsg.Message, Status = status }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// _CreateOrder
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _ReceiveTransfer()
        {
            return PartialView();
        }

        /// <summary>
        /// Received
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult ReceivedTransfer(Int64 ID)
        {
            TransferMasterDAL obj = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            TransferMasterDTO objDTO = obj.GetTransferByIdFull(ID);
            objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
            ViewBag.UDFs = GetUDFDataPageWise("TransferMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            ViewBag.TransferStatusList = GetTransferStatusList(objDTO, "");
            RoomDAL roomCtrl = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<RoomDTO> roomDTOList = roomCtrl.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
            ViewBag.ReplenishingRoom = roomDTOList;
            ViewBag.StagginLocations = new List<BinMasterDTO>();
            return PartialView("_ReceiveTransfer", objDTO);
        }

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult LoadTransferLineItemsForReceive(string ID)
        {
            TransferMasterDTO objDTO = null;
            long Id = 0;
            long.TryParse(ID, out Id);
            objDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferByIdPlain(Id);
            objDTO.TransferDetailList = new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetTransferDetailByTrfGuidAndSupplierNormal(objDTO.GUID, SessionHelper.UserSupplierIds);
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();

            if (objDTO.StagingID.GetValueOrDefault(0) > 0)
            {
                MaterialStagingDTO objMSDTO = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetRecord(objDTO.StagingID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);

                if (objMSDTO.BinID.GetValueOrDefault(0) > 0)
                {
                    ViewBag.DefaultBinNameInStagingHeader = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objMSDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID).BinNumber;
                }
            }

            return PartialView("_TransferLineItemsForReceive", objDTO);

        }

        /// <summary>
        /// ReceivedItemByLocations
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ordDetailID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReceiveItemQuantity(string ItemID, string DetailID, string ReceiveBinName, bool IsStaging, double QtyToReceive, List<ItemLocationLotSerialDTO> ItemLocationDetails = null)
        {
            long SessionUserId = SessionHelper.UserID;
            TransferInOutQtyDetailDAL objCtrl = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
            Int64 LocationID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByNameItemGuid(Guid.Parse(ItemID), ReceiveBinName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, IsStaging).GetValueOrDefault(0);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            ResponseMessage msg = objCtrl.ReceiveTransferedQuantity(Guid.Parse(ItemID), Guid.Parse(DetailID), LocationID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.UserSupplierIds, QtyToReceive, RoomDateFormat, SessionUserId,SessionHelper.EnterPriceID, ItemLocationDetails: ItemLocationDetails);

            //--------------------------------SET OnTransferQuantity In ItemMaster--------------------------------
            //
            TransferInOutQtyDetailDAL objTransferInOutQtyDetailDAL = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
            objTransferInOutQtyDetailDAL.UpdateOnTransferQuantityByTransferDetailGUID(Guid.Parse(DetailID), SessionHelper.UserID, false, SessionUserId);

            return Json(new { Message = msg.Message, Status = msg.IsSuccess }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// LoadTransferLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult LoadStagingTransferLineItemsForReceive(string ID)
        {
            TransferMasterDTO objDTO = null;
            long Id = 0;
            long.TryParse(ID, out Id);
            objDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferByIdPlain(Id);
            objDTO.TransferDetailList = new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetTransferDetailNormal(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds);
            return PartialView("_TransferLineItemsForReceiveStaging", objDTO);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string RenderRazorViewToString(string viewName, object model)
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

        /// <summary>
        /// ReceivedItemByLocations
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ordDetailID"></param>
        /// <returns></returns>
        public string ItemRetrived(string ItemID, string DetailID)
        {
            TransferInOutQtyDetailDAL detailCTRL = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
            var objModel = detailCTRL.GetTransferInOutQtyDetailsForReceiveByTransferfDtlGUIDNormal(Guid.Parse(DetailID)).OrderBy("ID DESC");
            ViewBag.TransferDetailID = DetailID;
            ViewBag.ItemID = ItemID;
            return RenderRazorViewToString("_ReceivedLineItems", objModel);

        }

        /// <summary>
        /// ReceivedItemByLocations
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ordDetailID"></param>
        /// <returns></returns>
        public string ItemTransferedQty(string ItemID, string DetailID)
        {
            TransferInOutQtyDetailDAL detailCTRL = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
            var objModel = detailCTRL.GetTransferInOutQtyDetailsByTransferfDtlGUIDNormal(Guid.Parse(DetailID)).OrderBy("ID DESC");
            ViewBag.TransferDetailID = DetailID;
            return RenderRazorViewToString("_TransferedLineItems", objModel);

        }

        public string GetTransferInOutItemDetail(string TransferInOutQtyDetailGUID)
        {
            TransferInOutItemDetailDAL transferInOutItemDetailDAL = new TransferInOutItemDetailDAL(SessionHelper.EnterPriseDBName);
            var transferInOutItemDetails = transferInOutItemDetailDAL.GetTransferInOutItemDetailsByTrfQtyDetailGuidNormal(Guid.Parse(TransferInOutQtyDetailGUID)).OrderBy("ID DESC");
            ViewBag.TransferInOutQtyDetailGUID = TransferInOutQtyDetailGUID;
            return RenderRazorViewToString("_TransferInOutItemDetail", transferInOutItemDetails);
        }

        /// <summary>
        /// DeleteRecieveAndUpdateReceivedQty
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ordDetailID"></param>
        /// <param name="deleteIDs"></param>
        /// <returns></returns>
        public string DeleteRecieveAndUpdateReceivedQty(string ItemID, string DetailID, string deleteIDs)
        {
            long SessionUserId = SessionHelper.UserID;
            ItemLocationDetailsDAL itmLoc = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            itmLoc.DeleteRecords(deleteIDs, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, "Replenish >> Delete Receive", SessionUserId);
            IEnumerable<ItemLocationDetailsDTO> objModel = itmLoc.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ItemID), Guid.Parse(DetailID), "ID DESC");

            var aax = objModel.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));

            TransferDetailDAL ordDetailCtrl = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            TransferDetailDTO objOrdDetailDTO = ordDetailCtrl.GetTransferDetailsByGuidPlain(Guid.Parse(DetailID));
            objOrdDetailDTO.ReceivedQuantity = aax;
            ordDetailCtrl.Edit(objOrdDetailDTO, SessionUserId,SessionHelper.EnterPriceID);

            return "ok";
        }

        /// <summary>
        /// ReceivedItemByLocations
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ordDetailID"></param>
        /// <returns></returns>
        public string StagingReceivedLineItems(string DetailID)
        {
            TransferDetailDAL TransferDetailctrl = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            TransferDetailDTO objModel = TransferDetailctrl.GetTransferDetailsByGuidPlain(Guid.Parse(DetailID));
            return RenderRazorViewToString("_ReceivedLineItemsStaging", objModel);
        }

        #endregion

        #region Transfer History
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferMasterListHistory()
        {
            return PartialView("_TransferListHistory");
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferHistoryView(Int64 ID)
        {
            TransferMasterDTO objDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferHistoryByIdFull(ID);
            objDTO.IsRecordNotEditable = true;

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("TransferMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ViewBag.TransferStatusList = GetTransferStatusList(objDTO, "");

            RoomDAL roomCtrl = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<RoomDTO> roomDTOList = roomCtrl.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
            //ViewBag.ReplenishingRoom = GetReplinishingRoom();
            ViewBag.ReplenishingRoom = roomDTOList;
            ViewBag.StagginLocations = new List<BinMasterDTO>();

            return PartialView("_TransferCreateHistory", objDTO);
        }

        /// <summary>
        /// LoadOrderLineItemsHistory
        /// </summary>
        /// <param name="historyID"></param>
        /// <returns></returns>
        public ActionResult LoadTransferLineItemsHistory(Int64 historyID)
        {
            TransferMasterDTO objDTO = null;
            //objDTO = new TransferMasterController().GetHistoryRecord(historyID);
            objDTO = new TransferMasterDAL(SessionHelper.EnterPriseDBName).GetTransferHistoryByIdPlain(historyID);
            //objDTO.TransferDetailList = new TransferDetailController().GetHistoryRecordsListByOrderID(objDTO.ID);
            objDTO.TransferDetailList = new TransferDetailDAL(SessionHelper.EnterPriseDBName).GetTransferDetailHistoryByTrfGuidNormal(objDTO.GUID);
            objDTO.IsRecordNotEditable = true;
            return PartialView("_TransferLineItemsHistory", objDTO);
            //return PartialView("_OrderLineItems_History", obj);
        }

        #endregion
        /// <summary>
        /// This method is used to return data which is required to fill first grid of lot/serial selection popup of transfer.
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TransferItemQuantity(string para)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            List<TransferDetailDTO> transferDetails = s.Deserialize<List<TransferDetailDTO>>(para);
            TransferDetailDAL transerDetailDAL = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            List<TransferDetailDTO> lsttransferDetails = new List<TransferDetailDTO>();

            foreach (TransferDetailDTO transferItem in transferDetails)
            {
                lsttransferDetails.Add(transferItem);
            }

            transferDetails = transerDetailDAL.GeTransferWithDetails(lsttransferDetails, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
            return PartialView("_TransferLotSrSelection", transferDetails);
        }

        /// <summary>
        /// This method is used to return data which is required to fill first grid of lot/serial selection popup of receive transfer.
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TransferItemQuantityForReceive(string para)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            List<TransferDetailDTO> transferDetails = s.Deserialize<List<TransferDetailDTO>>(para);
            TransferDetailDAL transerDetailDAL = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            List<TransferDetailDTO> lsttransferDetails = new List<TransferDetailDTO>();

            foreach (TransferDetailDTO transferItem in transferDetails)
            {
                lsttransferDetails.Add(transferItem);
            }

            transferDetails = transerDetailDAL.GeTransferWithDetails(lsttransferDetails, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);
            return PartialView("_TransferLotSrSelectionForReceive", transferDetails);
        }

        /// <summary>
        /// This method is used to get the items with lot/serial details for transfer and receive transfer.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult TransferLotSrSelection(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Guid ToolGUID = Guid.Empty;
            Guid TransferDetailGUID = Guid.Empty;
            long BinID = 0;
            double PullQuantity = 0;
            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            Guid.TryParse(Convert.ToString(Request["ToolGUID"]), out ToolGUID);
            bool isForReceiveTransfer = Guid.TryParse(Convert.ToString(Request["TransferDetailGUID"]), out TransferDetailGUID); // this will not be Guid.Empty in case of receive transfer            
            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["TransferQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            bool IsStagginLocation = false;
            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ItemMasterDTO oItem = null;
            BinMasterDTO objLocDTO = null;

            if (ItemGUID != Guid.Empty)
            {
                oItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID);
                objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                    IsStagginLocation = true;
            }

            int TotalRecordCount = 0;
            TransferDetailDAL objTransferDetails = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            string[] arrItem;

            if (oItem != null && oItem.ItemType == 4)
            {
                ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                oLotSr.BinID = BinID;
                oLotSr.ID = BinID;
                oLotSr.ItemGUID = ItemGUID;
                oLotSr.LotOrSerailNumber = string.Empty;
                oLotSr.Expiration = string.Empty;
                oLotSr.BinNumber = string.Empty;
                oLotSr.PullQuantity = oItem.DefaultPullQuantity.GetValueOrDefault(0) > PullQuantity ? oItem.DefaultPullQuantity.GetValueOrDefault(0) : PullQuantity;
                oLotSr.LotSerialQuantity = PullQuantity;

                retlstLotSrs.Add(oLotSr);
            }
            else
            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                            || !oItem.DateCodeTracking)
                        {
                            arrItem = new string[2];
                            arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                            arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                        else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0] + "_" + arrItem[1];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[2]));
                            }
                        }
                        else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                        else
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                    }

                    if (TransferDetailGUID != Guid.Empty) // for receive transfer
                    {
                        lstLotSrs = objTransferDetails.GetItemLocationsWithLotSerialsForReceiveTransfer(TransferDetailGUID, ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "0", string.Empty, (IsStagginLocation ? "1" : "0"));
                    }
                    else
                    {
                        lstLotSrs = objTransferDetails.GetItemLocationsWithLotSerialsForTransfer(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "0", string.Empty, (IsStagginLocation ? "1" : "0"));
                    }

                    retlstLotSrs = lstLotSrs.Where(t =>
                        (
                            (
                                arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && !oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
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

                            if (objLocDTO != null && objLocDTO.ID > 0)
                            {
                                oLotSr.BinNumber = objLocDTO.BinNumber;
                            }
                            if (oItem.SerialNumberTracking)
                            {
                                oLotSr.PullQuantity = 1;
                            }
                            oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                            oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                            oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                            retlstLotSrs.Add(oLotSr);
                        }
                        else
                        {
                            retlstLotSrs =
                                retlstLotSrs.Union
                                (
                                    lstLotSrs.Where(t =>
                                  (
                                        (
                                            !arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && !oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.BinNumber)
                                            && !oItem.SerialNumberTracking
                                            && !oItem.LotNumberTracking
                                            && !oItem.DateCodeTracking
                                         )
                                 )).Take(1)
                              ).ToList();
                        }
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

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.BinNumber = objLocDTO.BinNumber;
                        }
                        if (oItem.SerialNumberTracking)
                        {
                            oLotSr.PullQuantity = 1;
                        }
                        oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                        oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                        retlstLotSrs.Add(oLotSr);
                    }
                    else
                    {
                        if (TransferDetailGUID != Guid.Empty) // for receive transfer
                        {
                            retlstLotSrs = objTransferDetails.GetItemLocationsWithLotSerialsForReceiveTransfer(TransferDetailGUID, ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "1", string.Empty, (IsStagginLocation ? "1" : "0"));
                        }
                        else
                        {
                            retlstLotSrs = objTransferDetails.GetItemLocationsWithLotSerialsForTransfer(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "1", string.Empty, (IsStagginLocation ? "1" : "0"));
                        }
                    }

                }

                foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
                {
                    if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        double value = dicSerialLots[item.LotOrSerailNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.Expiration];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.BinNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (item.PullQuantity <= PullQuantity)
                    {
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (PullQuantity >= 0)
                    {
                        item.PullQuantity = PullQuantity;
                        PullQuantity = 0;
                    }
                    else
                    {
                        item.PullQuantity = 0;
                    }
                    if (item.ExpirationDate != null && item.ExpirationDate.HasValue && item.ExpirationDate.Value != DateTime.MinValue)
                    {
                        item.Expiration = FnCommon.ConvertDateByTimeZone(item.ExpirationDate.Value, true, true);
                    }
                    if (item.ReceivedDate != null && item.ReceivedDate.HasValue && item.ReceivedDate.Value != DateTime.MinValue)
                    {
                        item.Received = FnCommon.ConvertDateByTimeZone(item.ReceivedDate.Value, true, true);
                    }
                    if (item.PullQuantity > 0)
                        item.IsSelected = true;
                }
            }

            if (!(ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking)))
            {
                if (TransferDetailGUID != Guid.Empty)
                {
                    retlstLotSrs = retlstLotSrs.ToList();
                }
                else
                {
                    retlstLotSrs = retlstLotSrs.Where(x => x.PullQuantity > 0).ToList();
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

        /// <summary>
        /// This method is used to get the list of lot/serial for receive transfer.
        /// </summary>
        /// <param name="maxRows"></param>
        /// <param name="name_startsWith"></param>
        /// <param name="ItemGuid"></param>
        /// <param name="transferDetailGUID"></param>
        /// <param name="BinID"></param>
        /// <param name="prmSerialLotNos"></param>
        /// <returns></returns>
        public JsonResult GetLotOrSerailNumberListForReceiveTransfer(int maxRows, string name_startsWith, Guid? ItemGuid, Guid? transferDetailGUID, long BinID, string prmSerialLotNos = null)
        {
            bool IsStagginLocation = false;
            TransferDetailDAL objTransferDetails = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO = objTransferDetails.GetItemLocationsWithLotSerialsForReceiveTransfer(transferDetailGUID ?? Guid.Empty, ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, "0", String.Empty, (IsStagginLocation ? "1" : "0"));
            string[] arrSerialLotNos = prmSerialLotNos.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrWhiteSpace(name_startsWith))
            {
                name_startsWith = name_startsWith.Trim();
            }

            var lstLotSr =
                objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith) && !arrSerialLotNos.Contains(x.LotOrSerailNumber)).Select(x => new { x.LotOrSerailNumber }).Distinct();

            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used to validate lot/serial for receive transfer.
        /// </summary>
        /// <param name="ItemGuid"></param>
        /// <param name="SerialOrLotNumber"></param>
        /// <param name="BinID"></param>
        /// <param name="TransferDetailGUID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ValidateSerialLotNumberForReceiveTransfer(Guid? ItemGuid, string SerialOrLotNumber, long BinID, Guid? TransferDetailGUID)
        {
            bool IsStagginLocation = false;

            if (!string.IsNullOrWhiteSpace(SerialOrLotNumber))
            {
                SerialOrLotNumber = SerialOrLotNumber.Trim();
            }

            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            TransferDetailDAL objTransferDetails = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            ItemLocationLotSerialDTO objItemLocationLotSerialDTO = objTransferDetails.GetItemLocationsWithLotSerialsForReceiveTransfer(TransferDetailGUID ?? Guid.Empty, ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, "0", SerialOrLotNumber, (IsStagginLocation ? "1" : "0")).FirstOrDefault();

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

        [HttpPost]
        public JsonResult CloseTransfer(string Ids)
        {
            string message = string.Empty, status = string.Empty;
            TransferMasterDAL transferMasterDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            TransferMasterDTO transferMasterDTO = null;
            int errorCount = 0;

            foreach (var item in Ids.Split(','))
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    long transferId = 0;
                    if (long.TryParse(item.Trim(), out transferId))
                    {
                        try 
                        {
                            transferMasterDTO = transferMasterDAL.GetTransferByIdPlain(transferId);
                            if (transferMasterDTO.TransferStatus == (int)TransferStatus.UnSubmitted)
                            {
                                transferMasterDTO.TransferStatus = (int)TransferStatus.Closed;
                                transferMasterDTO.EditedFrom = "Web";
                                transferMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                transferMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                transferMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                bool isSave = transferMasterDAL.Edit(transferMasterDTO);
                            }

                            TransferMasterDTO replenishTransfer = null;
                            if (transferMasterDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                replenishTransfer = transferMasterDAL.GetTransferByGuidPlain(transferMasterDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty));
                            }                                
                            else
                            {
                                replenishTransfer = transferMasterDAL.GetTransferByRefTransferGuidPlain(transferMasterDTO.ReplenishingRoomID, transferMasterDTO.CompanyID, transferMasterDTO.GUID);
                            }

                            if (replenishTransfer != null && replenishTransfer.ID > 0 )
                            {
                                replenishTransfer.TransferStatus = (int)TransferStatus.Closed;
                                replenishTransfer.EditedFrom = "Web";
                                replenishTransfer.Updated = DateTimeUtility.DateTimeNow;
                                replenishTransfer.LastUpdatedBy = SessionHelper.UserID;
                                replenishTransfer.ReceivedOn = DateTimeUtility.DateTimeNow;
                                bool isSave = transferMasterDAL.Edit(replenishTransfer);
                            }
                        }
                        catch(Exception ex)
                        {
                            CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                            errorCount++;
                        }
                    }
                }
            }
            if (errorCount > 0)
            {
                status = "fail";
                message = "";
            }
            else
            {
                status = "ok";
                message = "";
            }

            return Json(new { Message = message, Status = status });
        }

        public void BlankSession()
        {
            Session["IsInsert"] = "";
        }
    }
}
