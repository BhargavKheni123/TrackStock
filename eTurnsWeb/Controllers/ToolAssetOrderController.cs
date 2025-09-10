using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ToolAssetOrderController : eTurnsControllerBase
    {
        bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrderSubmit);
        bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrderApproval);
        Int64 RoomID = SessionHelper.RoomID;
        Int64 CompanyID = SessionHelper.CompanyID;
        Int64 EnterpriseId = SessionHelper.EnterPriceID;

        #region Tool Asset Order Master
        public ToolAssetOrderController()
        {

        }

        /// <summary>
        /// Order List
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolAssetOrderList()
        {
            SessionHelper.RomoveSessionByKey(GetSessionKey(0));
            SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
            Session["ToolAssetOrderDetail"] = null;
            Session["ToolORDType"] = "1,2";
            return View();
        }



        private ToolAssetOrderType GetToolAssetOrderType
        {
            get
            {
                string strOrdType = "1";
                if (Request != null && Request.UrlReferrer != null && Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("ReturnOrderList"))
                {
                    strOrdType = "2";
                }
                else if (Request != null && Request.UrlReferrer != null && Request.Url.ToString().Contains("ReturnOrderList"))
                {
                    strOrdType = "2";
                }
                ToolAssetOrderType OrdType = (ToolAssetOrderType)Enum.Parse(typeof(ToolAssetOrderType), strOrdType, true);
                return OrdType;
            }
        }

        /// <summary>
        /// OrderMasterListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ToolAssetOrderMasterListAjax(JQueryDataTableParamModel param)
        {
            SessionHelper.RomoveSessionByKey(GetSessionKey(0));
            SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
            ToolAssetOrderMasterDAL controller = null;
            List<ToolAssetOrderMasterDTO> DataFromDB = null;
            try
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
                // set the default column sorting here, if first time then required to set 
                //if (sortColumnName == "0" || sortColumnName == "undefined")
                //    sortColumnName = "ID desc";

                if (!string.IsNullOrEmpty(sortColumnName))
                {
                    if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                        sortColumnName = "ID desc";
                }
                else
                    sortColumnName = "ID desc";

                if (sortColumnName.ToLower().Contains("orderstatustext"))
                    sortColumnName = sortColumnName.Replace("OrderStatusText", "OrderStatusName");

                //if (sortColumnName.ToLower().Contains("orderstatuschar"))
                //    sortColumnName = sortColumnName.Replace("OrderStatusChar", "OrderStatusName");

                if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower().Contains("toolassetordernumber"))
                    sortColumnName = sortColumnName.Replace("ToolAssetOrderNumber", "OrderNumber_ForSorting");

                string searchQuery = string.Empty;

                int TotalRecordCount = 0;
                string MainFilter = "";
                if (Convert.ToString(Session["MainFilter"]).Trim().ToLower() == "true")
                {
                    MainFilter = "true";
                }

                controller = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                DataFromDB = controller.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, GetToolAssetOrderType, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, MainFilter).ToList();

                DataFromDB.ForEach(x =>
                {
                    x.IsAbleToDelete = IsRecordDeleteable(x);

                    x.RequiredDateStr = x.RequiredDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);

                    x.OrderStatusText = ResToolAssetOrder.GetOrderStatusText(((eTurns.DTO.ToolAssetOrderStatus)x.OrderStatus).ToString());
                });

                SessionHelper.RomoveSessionByKey(GetSessionKey(0));
                return Json(new
                {

                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = DataFromDB
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                controller = null;
                DataFromDB = null;
            }
        }

        /// <summary>
        /// _CreateOrder
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _CreateToolAssetOrder()
        {


            return PartialView();
        }



        /// <summary>
        /// Get Release No
        /// </summary>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        public JsonResult GetReleaseNumber(string ToolAssetOrderNumber)
        {
            ToolAssetOrderMasterDAL objOrderDAL = null;
            IEnumerable<ToolAssetOrderMasterDTO> objOrderList = null;
            try
            {
                int ReleaseNo = 1;
                if (!string.IsNullOrWhiteSpace(ToolAssetOrderNumber))
                {
                    objOrderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                    objOrderList = objOrderDAL.GetAllRecords(RoomID, CompanyID, false, false, GetToolAssetOrderType);
                    //ReleaseNo = objOrderList.Where(x => x.ToolAssetOrderNumber == OrderNumber).Count() + 1;
                    if (objOrderList != null && objOrderList.Count() > 0)
                        ReleaseNo = objOrderList.Max(x => int.Parse(x.ReleaseNumber)) + 1;
                }
                return Json(new { ReleaseNumber = ReleaseNo, Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objOrderDAL = null;
                objOrderList = null;
            }

        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolAssetOrderCreate()
        {

            ToolAssetOrderMasterDAL objToolAssetOrderMasterDAL = null;
            IEnumerable<ToolAssetOrderMasterDTO> objToolAssetOrderMasterList = null;
            ToolAssetOrderMasterDTO objDTO = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            AutoOrderNumberGenerate objAutoNumber = null;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
            try
            {
                SessionHelper.RomoveSessionByKey(GetSessionKey(0));
                SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
                SessionHelper.RomoveSessionByKey("AddItemToOrder_" + RoomID + "_" + CompanyID);
                SessionHelper.RomoveSessionByKey("AddItemToReturnOrder_" + RoomID + "_" + CompanyID);
                int DefaultOrderRequiredDays = 0;

                //string ToolAssetorderNumber = string.Empty;
                objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                objAutoNumber = objAutoSeqDAL.GetNextToolAssetOrderNumber(RoomID, CompanyID, 0, EnterpriseId);

                string ToolAssetorderNumber = objAutoNumber.OrderNumber;
                if (ToolAssetorderNumber != null && (!string.IsNullOrEmpty(ToolAssetorderNumber)))
                {
                    ToolAssetorderNumber = ToolAssetorderNumber.Length > 22 ? ToolAssetorderNumber.Substring(0, 22) : ToolAssetorderNumber;
                }
                int ReleaseNo = 1;

                if (!string.IsNullOrWhiteSpace(ToolAssetorderNumber))
                {
                    objToolAssetOrderMasterDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                    objToolAssetOrderMasterList = objToolAssetOrderMasterDAL.GetAllRecords(RoomID, CompanyID, false, false, GetToolAssetOrderType).Where(x => x.ToolAssetOrderNumber == ToolAssetorderNumber && !string.IsNullOrEmpty(x.ReleaseNumber));
                    //ReleaseNo = objOrderList.Where(x => x.ToolAssetOrderNumber == orderNumber).Count() + 1;
                    if (objToolAssetOrderMasterList != null && objToolAssetOrderMasterList.Count() > 0)
                        ReleaseNo = objToolAssetOrderMasterList.Max(x => int.Parse(x.ReleaseNumber)) + 1;
                }



                objDTO = new ToolAssetOrderMasterDTO()
                {
                    RequiredDate = datetimetoConsider.AddDays(DefaultOrderRequiredDays),
                    ToolAssetOrderNumber = ToolAssetorderNumber,
                    OrderStatus = (int)ToolAssetOrderStatus.UnSubmitted,
                    ReleaseNumber = ReleaseNo.ToString(),
                    LastUpdated = DateTimeUtility.DateTimeNow,
                    Created = DateTimeUtility.DateTimeNow,

                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    LastUpdatedBy = SessionHelper.UserID,
                    CompanyID = CompanyID,
                    RoomID = RoomID,
                    RoomName = SessionHelper.RoomName,
                    UpdatedByName = SessionHelper.UserName,
                    OrderDate = datetimetoConsider,

                    OrderType = (int)GetToolAssetOrderType,

                    IsOnlyFromUI = true,
                    IsEDIOrder = false,
                };



                objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);

                ViewBag.UDFs = GetUDFDataPageWise("ToolAssetOrder");
                foreach (var i in ViewBag.UDFs)
                {
                    string _UDFColumnName = (string)i.UDFColumnName;
                    ViewData[_UDFColumnName] = i.UDFDefaultValue;
                }

                ViewBag.OrderStatusList = GetOrderStatusList(objDTO, "create");



                return PartialView("_CreateToolAssetOrder", objDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objToolAssetOrderMasterDAL = null;
                objToolAssetOrderMasterList = null;
                objDTO = null;


            }

        }





        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderEdit(Int64 ID)
        {
            ToolAssetOrderMasterDAL obj = null;
            ToolAssetOrderMasterDTO objDTO = null;
            List<SelectListItem> lstOrderStatus = null;


            try
            {

                SessionHelper.RomoveSessionByKey(GetSessionKey(ID));
                SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(ID));
                SessionHelper.RomoveSessionByKey("AddItemToOrder_" + RoomID + "_" + CompanyID);
                SessionHelper.RomoveSessionByKey("AddItemToReturnOrder_" + RoomID + "_" + CompanyID);

                obj = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                objDTO = obj.GetRecord(ID, RoomID, CompanyID);
                objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);

                objDTO.IsOnlyFromUI = true;
                bool isOrderHeaderEdit = true;
                if (objDTO.OrderStatus >= (int)ToolAssetOrderStatus.Submitted)
                    isOrderHeaderEdit = false;

                ViewBag.UDFs = GetUDFDataPageWise("ToolAssetOrder", isOrderHeaderEdit);
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;

                if (objDTO.IsDeleted || objDTO.IsArchived.GetValueOrDefault(false))
                {
                    //ViewBag.OrderStatusList = GetOrderStatusList(objDTO, "");
                    objDTO.IsRecordNotEditable = true;
                    objDTO.IsOnlyStatusUpdate = false;
                    objDTO.IsAbleToDelete = true;
                }

                if (objDTO.OrderStatus >= (int)ToolAssetOrderStatus.Approved || objDTO.IsRecordNotEditable)
                {
                    lstOrderStatus = GetOrderStatusList(objDTO, "");
                    //if (objDTO.IsRecordNotEditable)
                    //{
                    //    lstOrderStatus = lstOrderStatus.Where(x => int.Parse(x.Value) <= objDTO.OrderStatus).ToList();
                    //}
                }
                else
                    lstOrderStatus = GetOrderStatusList(objDTO, "edit");

                //if (IsChangeOrder && Convert.ToString(Request["IsChangeOrder"]) == "true")
                //{
                //    Int64 NewChangeOrderRevisionNo = 0;
                //    Int64.TryParse(Convert.ToString(Request["ChangeOrderRevisionNo"]), out NewChangeOrderRevisionNo);
                //    if (objDTO.ChangeOrderRevisionNo.GetValueOrDefault(0) < NewChangeOrderRevisionNo)
                //    {
                //        objDTO.IsRecordNotEditable = false;
                //        objDTO.ChangeOrderRevisionNo = NewChangeOrderRevisionNo;
                //        objDTO.IsChangeOrderClick = true;
                //        lstOrderStatus = lstOrderStatus.Where(x => x.Value == "1").ToList();
                //    }
                //}


                bool IsableToUnApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowAnOrderToBeUnapprove);

                ViewBag.OrderStatusList = lstOrderStatus;



                objDTO.RequiredDateStr = objDTO.RequiredDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);




                return PartialView("_CreateToolAssetOrder", objDTO);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                obj = null;
                objDTO = null;
                lstOrderStatus = null;

            }

        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveOrder(ToolAssetOrderMasterDTO objDTO)
        {
            ToolAssetOrderMasterDAL obj = null;
            CommonDAL objCDAL = null;

            IEnumerable<ToolAssetOrderMasterDTO> objOrderList = null;
            List<ToolAssetOrderDetailsDTO> lstOrdDetailDTO = null;
            objDTO.EditedOnAction = "Tool Asset Order Save from web from order page.";
            try
            {
                string message = "";
                string status = "";

                obj = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                if (string.IsNullOrEmpty(objDTO.ToolAssetOrderNumber))
                {
                    message = string.Format(ResMessage.Required, ResToolAssetOrder.ToolAssetOrderNumber);
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                if (!string.IsNullOrEmpty(objDTO.ToolAssetOrderNumber) && objDTO.ToolAssetOrderNumber != null)
                {
                    if (objDTO.ToolAssetOrderNumber.Length > 22)
                    {
                        message = ResOrder.OrderNumberLengthUpto22Char;
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                }

                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, "ToolAssetOrder", SessionHelper.RoomID);

                CommonDAL commonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string OrdersUDFRequier = string.Empty;
                commonDAL.CheckUDFIsRequired("ToolAssetOrder", objDTO.UDF1, objDTO.UDF2, objDTO.UDF3, objDTO.UDF4, objDTO.UDF5, out OrdersUDFRequier, CompanyID, RoomID,EnterpriseId,SessionHelper.UserID);

                if (!string.IsNullOrEmpty(OrdersUDFRequier))
                {
                    return Json(new { Message = OrdersUDFRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }

                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.RoomID = RoomID;

                // objDTO.RequiredDate = DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                objDTO.RequiredDate = DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);


                //objDTO.ToolAssetOrderNumber = objDTO.ToolAssetOrderNumber.Replace("'", "");
                //if (!string.IsNullOrEmpty(objDTO.ToolAssetOrderNumber_ForSorting))
                //    objDTO.ToolAssetOrderNumber_ForSorting = objDTO.ToolAssetOrderNumber_ForSorting.Replace("'", "");

                //----------------------Check For Order Number Duplication----------------------
                //
                string strOK = string.Empty;
                // RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);

                string columnList = "ID,RoomName,IsAllowOrderDuplicate,POAutoSequence";
                RoomDTO roomDTO = commonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                if (roomDTO.IsAllowOrderDuplicate != true)
                {
                    if (obj.IsOrderNumberDuplicate(objDTO.ToolAssetOrderNumber, objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID))
                    {
                        strOK = "duplicate";
                    }
                }

                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolAssetOrder.ToolAssetOrderNumber, objDTO.ToolAssetOrderNumber);
                    status = "duplicate";
                }
                else
                {
                    if (objDTO.ID == 0)
                    {


                        if (roomDTO.POAutoSequence.GetValueOrDefault(0) == 0)
                        {
                            objOrderList = obj.GetAllRecords(RoomID, CompanyID, false, false, ToolAssetOrderType.Order);
                            objDTO.ReleaseNumber = Convert.ToString(objOrderList.Where(x => x.ToolAssetOrderNumber == objDTO.ToolAssetOrderNumber).Count() + 1);
                        }

                        objDTO.GUID = Guid.NewGuid();
                        objDTO.WhatWhereAction = "Order";

                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO = obj.InsertOrder(objDTO);
                        long ReturnVal = objDTO.ID;
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
                    else
                    {
                        string OrderDetailsUDFRequier = string.Empty;
                        lstOrdDetailDTO = GetLineItemsFromSession(objDTO.ID);

                        double? UserApprovalLimit = null;
                        double UserUsedLimit = 0;
                        double OrderCost = 0;
                        double? ItemApprovedQuantity = null;
                        int? PriseSelectionOption = 0;
                        eTurns.DAL.RoomDAL onjRoomDAL = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName);
                        RoomModuleSettingsDTO objRoomModuleSettingsDTO = onjRoomDAL.GetRoomModuleSettings(eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Orders);
                        if (objRoomModuleSettingsDTO != null)
                            PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;

                        if (PriseSelectionOption != 1 && PriseSelectionOption != 2)
                            PriseSelectionOption = 1;

                        DollerApprovalLimitDTO objDollarLimt = null;
                        UserMasterDAL userDAL = new UserMasterDAL(SessionHelper.EnterPriseDBName);
                        if (IsApprove)
                        {
                            objDollarLimt = userDAL.GetOrderLimitByUserId(SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                        }
                        if (objDollarLimt != null)
                        {
                            UserApprovalLimit = objDollarLimt.DollerLimit > 0 ? objDollarLimt.DollerLimit : null;
                            UserUsedLimit = objDollarLimt.UsedLimit;
                            ItemApprovedQuantity = objDollarLimt.ItemApprovedQuantity > 0 ? objDollarLimt.ItemApprovedQuantity : null;
                        }

                        foreach (ToolAssetOrderDetailsDTO objOrderDetail in lstOrdDetailDTO)
                        {
                            string ordDetailUDFReq = string.Empty;
                            commonDAL.CheckUDFIsRequired("ToolAssetOrderDetails", objOrderDetail.UDF1, objOrderDetail.UDF2, objOrderDetail.UDF3, objOrderDetail.UDF4, objOrderDetail.UDF5, out ordDetailUDFReq, CompanyID, RoomID,SessionHelper.EnterPriceID,SessionHelper.UserID);
                            if (!string.IsNullOrEmpty(ordDetailUDFReq))
                                OrderDetailsUDFRequier = OrderDetailsUDFRequier + string.Format(ResQuoteMaster.RequiredFor, ordDetailUDFReq, objOrderDetail.ToolName); 

                            //                            if (!string.IsNullOrEmpty(OrderDetailsUDFRequier))
                            //                               break;
                            //if (string.IsNullOrEmpty(objOrderDetail.ToolLocationName))
                            //{
                            //    OrderDetailsUDFRequier = OrderDetailsUDFRequier + ResToolAssetOrder.Bin+ " for " + objOrderDetail.ToolName + ".";
                            //}
                            if (objOrderDetail.ToolLocationName == null)
                            {
                                objOrderDetail.ToolLocationName = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(OrderDetailsUDFRequier))
                                break;
                            ToolMasterDAL objItemMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                            ToolMasterDTO objItemMasterDTO = new ToolMasterDTO();
                            objItemMasterDTO = objItemMasterDAL.GetToolByGUIDPlain(objOrderDetail.ToolGUID ?? Guid.Empty);

                            if (objOrderDetail != null && objOrderDetail.ApprovedQuantity != null
                                && objOrderDetail.ApprovedQuantity > 0)
                            {
                                if (objDollarLimt != null && objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && IsApprove && objDTO.OrderStatus == (int)ToolAssetOrderStatus.Approved
                                   && ItemApprovedQuantity > 0 && objOrderDetail.ApprovedQuantity > (ItemApprovedQuantity))
                                {
                                    return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanPerOrderItemQtyApprovalLimit, objOrderDetail.ApprovedQuantity, ItemApprovedQuantity), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                }
                            }


                            if (objItemMasterDTO != null && objOrderDetail.ApprovedQuantity != null
                                        && objOrderDetail.ApprovedQuantity > 0)
                            {

                                OrderCost += (objItemMasterDTO.Cost.GetValueOrDefault(0) * objOrderDetail.ApprovedQuantity.GetValueOrDefault(0));
                            }
                        }

                        if (!string.IsNullOrEmpty(OrderDetailsUDFRequier))
                            return Json(new { Message = OrderDetailsUDFRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);

                        //if (SessionHelper.RoleID > 0 && objDollarLimt != null && objDTO.OrderType == 1)
                        //{

                        //    if (objDollarLimt.OrderLimitType == OrderLimitType.All && IsApprove && objDTO.OrderStatus == (int)ToolAssetOrderStatus.Approved
                        //         && (UserApprovalLimit - UserUsedLimit) > 0 && OrderCost > (UserApprovalLimit - UserUsedLimit))
                        //        return Json(new { Message = "Can not approve ($" + OrderCost + ") more than remaining order approval limit($" + (UserApprovalLimit - UserUsedLimit) + ").", Status = "fail" }, JsonRequestBehavior.AllowGet);
                        //    else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && IsApprove && objDTO.OrderStatus == (int)ToolAssetOrderStatus.Approved
                        //             && UserApprovalLimit > 0 && OrderCost > (UserApprovalLimit))
                        //        return Json(new { Message = "Can not approve ($" + OrderCost + ") more than per order approval limit($" + (UserApprovalLimit) + ").", Status = "fail" }, JsonRequestBehavior.AllowGet);
                        //}

                        ToolAssetOrderMasterDTO tempOrd = new ToolAssetOrderMasterDTO();
                        tempOrd = obj.GetRecord((Int64)objDTO.ID, (Int64)objDTO.RoomID, (Int64)objDTO.CompanyID);
                        if (tempOrd != null)
                        {
                            objDTO.AddedFrom = tempOrd.AddedFrom;
                            objDTO.ReceivedOnWeb = tempOrd.ReceivedOnWeb;

                            if (tempOrd.OrderStatus == (int)ToolAssetOrderStatus.UnSubmitted && objDTO.OrderStatus == (int)ToolAssetOrderStatus.Closed)
                                TempData["IsOrderClosedFromUnSubmitted"] = true;

                        }

                        if (objDTO.OrderStatus >= (int)ToolAssetOrderStatus.Submitted && objDTO.OrderStatus != (int)ToolAssetOrderStatus.Closed && (lstOrdDetailDTO == null || lstOrdDetailDTO.Count <= 0))
                        {
                            message = string.Format(ResOrder.OrderMustHaveOneLineItem);
                            status = "fail";
                        }
                        else
                        {
                            objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;

                            bool ReturnVal = obj.Edit(objDTO);
                            if (ReturnVal)
                            {
                                if (SessionHelper.RoleID > 0
                                    && objDollarLimt != null
                                    && objDollarLimt.OrderLimitType == OrderLimitType.All
                                    && IsApprove
                                    && objDTO.OrderStatus == (int)ToolAssetOrderStatus.Approved
                                    && OrderCost <= (UserApprovalLimit - UserUsedLimit))
                                {
                                    userDAL.SaveDollerUsedLimt(OrderCost, SessionHelper.UserID, CompanyID, RoomID);
                                }

                                // As per client Mail :Short term To Dos, committed completion dates, and specific fixes needed (point :6  Eliminate “Transmit” on Order History)
                                if (objDTO.OrderStatus == (int)ToolAssetOrderStatus.Approved)
                                {
                                    objDTO.EditedFrom = "Web";
                                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    UpdateOrderToTransmited(objDTO.GUID.ToString(), "Web", "SaveOrder.ToTransmitFromApproved");
                                }
                                message = ResMessage.SaveMessage;
                                status = "ok";
                            }
                            else
                            {
                                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                                status = "fail";
                            }
                        }
                    }


                    if (objDTO.OrderStatus == (int)ToolAssetOrderStatus.UnSubmitted)
                    {
                        //SendMailOrderUnSubmitted
                        //SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                        //SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetRecord(objDTO.Supplier.GetValueOrDefault(0), RoomID, CompanyID, false);
                        SendMailOrderUnSubmitted(objDTO);
                    }

                }

                Session["IsInsert"] = "True";
                return Json(new { Message = message, Status = status, ID = objDTO.ID }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                obj = null;
                objCDAL = null;
                objOrderList = null;
                lstOrdDetailDTO = null;
            }
        }

        /// <summary>
        /// Send Mail After Save;
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult SetOrderMail(Int64 OrderID)
        {
            ToolAssetOrderMasterDTO objOrderDTO = null;
            ToolAssetOrderMasterDAL objOrdDAL = null;

            try
            {
                objOrdDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                objOrderDTO = objOrdDAL.GetRecord(OrderID, RoomID, CompanyID);

                if (objOrderDTO.OrderStatus == (int)ToolAssetOrderStatus.Submitted)
                {
                    SendMailToApprovalAuthority(objOrderDTO);
                }
                //else if (objOrderDTO.OrderStatus == (int)ToolAssetOrderStatus.UnSubmitted)
                //{
                //    //SendMailOrderUnSubmitted
                //    objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                //    lstSuppliers = new List<SupplierMasterDTO>();
                //    objSupplierDTO = objSupplierDAL.GetRecord(objOrderDTO.Supplier.GetValueOrDefault(0), RoomID, CompanyID, false);
                //    SendMailOrderUnSubmitted(objSupplierDTO, objOrderDTO);

                //}
                else if (objOrderDTO.OrderStatus == (int)ToolAssetOrderStatus.Approved || objOrderDTO.OrderStatus == (int)ToolAssetOrderStatus.Transmitted)
                {
                    SendMailForApprovedOrReject(objOrderDTO, "approved");
                }

                return Json(new { Message = ResOrder.MailSendSuccessfully, Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.ToString(), Status = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objOrderDTO = null;
                objOrdDAL = null;

            }

        }
        /// <summary>
        /// UpdateOrderToTransmited
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult UpdateOrderToTransmited(string OrderGUID, string editedFrom, string whateWhereAction)
        {
            ToolAssetOrderMasterDAL obj = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            ToolAssetOrderMasterDTO objDTO = new ToolAssetOrderMasterDTO() { GUID = Guid.Parse(OrderGUID), RoomID = RoomID, CompanyID = CompanyID, EditedFrom = editedFrom, WhatWhereAction = whateWhereAction };
            objDTO = obj.DB_TransmitOrder(objDTO);
            return Json(new { Message = ResMessage.SaveMessage, Status = "ok", ID = objDTO.ID }, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// UpdateOrderToClose
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult UpdateOrderToClose(Int64 OrderID)
        {
            ToolAssetOrderMasterDAL obj = null;
            ToolAssetOrderDetailsDAL objDetailDAL = null;
            List<ToolAssetOrderDetailsDTO> lstOrdDetailDTO = null;
            ToolAssetOrderMasterDTO objDTO = null;
            try
            {
                obj = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                objDTO = obj.GetRecord(OrderID, RoomID, CompanyID);
                objDTO.OrderStatus = (int)ToolAssetOrderStatus.Closed;
                objDTO.IsOnlyFromUI = true;
                objDTO.EditedFrom = "Web";
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                bool isSave = obj.Edit(objDTO);
                if (isSave)
                {
                    objDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    lstOrdDetailDTO = objDetailDAL.GetOrderedRecord(objDTO.GUID, RoomID, CompanyID);
                    foreach (var item in lstOrdDetailDTO)
                    {
                        item.EditedFrom = "Web";
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDetailDAL.Edit(item);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                obj = null;
                objDetailDAL = null;
                lstOrdDetailDTO = null;
                objDTO = null;
            }

            return Json(new { Message = ResOrder.OrderClosedSuccessfully, Status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void UpdateOrderComment(string Comment, Int64 OrderID)
        {
            try
            {
                ToolAssetOrderMasterDAL ordDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                ordDAL.UpdateOrderComment(Comment, OrderID, SessionHelper.UserID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult UpdateOrderCommentNew(string Comment, Int64 OrderID, ToolAssetOrderDetailsDTO[] arrDetails)
        {
            string message = "";
            string status = "";

            try
            {
                ToolAssetOrderMasterDAL ordDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                ordDAL.UpdateOrderComment(Comment, OrderID, SessionHelper.UserID);

                if (OrderID > 0 && arrDetails != null)
                {
                    ToolAssetOrderDetailsDAL objDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    foreach (ToolAssetOrderDetailsDTO item in arrDetails)
                    {
                        objDAL.UpdateLineComment(item, SessionHelper.UserID);
                    }
                }
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = "Error";
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateSomeFieldsInOrder(ToolAssetOrderMasterDTO order)
        {
            string message = "fail";
            bool status = false;
            if (order != null && order.ID > 0)
            {
                ToolAssetOrderMasterDAL ordDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                ordDAL.UpdateOrderComment(order.Comment, order.PackSlipNumber, order.ID, SessionHelper.UserID);

                if (order.ID > 0 && order.OrderListItem != null)
                {
                    ToolAssetOrderDetailsDAL objDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    foreach (ToolAssetOrderDetailsDTO item in order.OrderListItem)
                    {
                        objDAL.UpdateLineComment(item, SessionHelper.UserID);
                    }
                }
                message = "Success";
                status = true;
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTabCountRedCircle()
        {
            return Json(new
            {
                UnsubmittedCount = 0,
                SubmittedCount = 0,
                ChanegeOrderCount = 0,
                ApproveCount = 0,
                CloseCount = 0,
                CartNotification = ResLayout.Cart,
                OrderNotification = ResLayout.Orders,
                ReceiveNotification = ResLayout.Receive,
                TransferNotification = ResLayout.Transfer,
                TotalReplanish = 0
            }, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// BlankSession
        /// </summary>
        public void BlankSession()
        {
            Session["IsInsert"] = "";
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteOrderMasterRecords(string ids)
        {
            try
            {
                //OrderMasterController obj = new OrderMasterController();
                ToolAssetOrderMasterDAL obj = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, RoomID, CompanyID);
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }





        /// <summary>
        /// IsRecordNotEditable
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool IsRecordNotEditable(ToolAssetOrderMasterDTO objDTO)
        {
            bool isNotEditable = false;

            bool IsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            bool IsUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
            bool IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
            //if (objDTO.OrderType == (int)ToolAssetOrderType.RuturnOrder)
            //{
            //    IsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            //    IsUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
            //    IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);

            //}

            if (!(IsInsert || IsUpdate || IsDelete || IsSubmit || IsApprove))//|| IsChangeOrder
            {
                isNotEditable = true;
                return isNotEditable;
            }

            if (objDTO.ID <= 0 && !IsInsert)
            {
                isNotEditable = true;
                return isNotEditable;
            }

            //if (IsChangeOrder && !objDTO.OrderIsInReceive && objDTO.OrderStatus == (int)ToolAssetOrderStatus.Transmitted)
            //{
            //    isNotEditable = false;
            //}
            else if (IsUpdate || IsSubmit || IsApprove || Convert.ToString(Session["IsInsert"]) == "True" || IsInsert)
            {
                if (IsApprove || IsSubmit)
                    objDTO.IsOnlyStatusUpdate = true;
                else
                    objDTO.IsOnlyStatusUpdate = false;


                if (objDTO.OrderStatus < (int)ToolAssetOrderStatus.Submitted)
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
                else if (objDTO.OrderStatus >= (int)ToolAssetOrderStatus.Submitted)
                {
                    if (objDTO.OrderStatus == (int)ToolAssetOrderStatus.Submitted)
                    {
                        if (IsSubmit && !IsApprove)
                            isNotEditable = false;
                        else if (!IsApprove)
                            isNotEditable = true;
                        else if (IsApprove && !IsUpdate)
                        {
                            objDTO.IsOnlyStatusUpdate = true;
                            isNotEditable = true;
                        }
                    }
                    else if (objDTO.OrderStatus > (int)ToolAssetOrderStatus.Submitted)
                    {
                        isNotEditable = true;
                        //if (objDTO.OrderStatus == (int)ToolAssetOrderStatus.Transmitted && IsChangeOrder)
                        //    isNotEditable = false;
                        objDTO.IsOnlyStatusUpdate = false;
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
        public bool IsRecordDeleteable(ToolAssetOrderMasterDTO objDTO)
        {
            bool IsDeleteable = false;
            bool IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
            //if (objDTO.OrderType == (int)ToolAssetOrderType.RuturnOrder)
            //{
            //    IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
            //}
            if ((objDTO.OrderStatus < (int)ToolAssetOrderStatus.Approved || objDTO.OrderStatus == (int)ToolAssetOrderStatus.Closed) && IsDelete)
            {
                IsDeleteable = true;
            }

            return IsDeleteable;
        }

        /// <summary>
        /// ChangeOrder
        /// </summary>
        /// <param name="OrderGuid"></param>
        /// <param name="OrdType"></param>
        /// <returns></returns>
        public PartialViewResult GetChangeOrders(Guid OrderGuid, int OrdType)
        {
            List<ToolAssetOrderMasterDTO> objChangeOrders = null;
            ToolAssetOrderMasterDAL objOrderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            objChangeOrders = objOrderDAL.DB_GetChangeOrderData(CompanyID, RoomID, false, false, null, OrderGuid, (OrderType)OrdType, null, null).ToList();
            ToolAssetOrderMasterDTO objCurrentOrder = objOrderDAL.GetRecord(OrderGuid, RoomID, CompanyID);
            objCurrentOrder.IsMainOrderInChangeOrderHistory = true;
            objCurrentOrder.ChangeOrderLastUpdated = objCurrentOrder.LastUpdated.GetValueOrDefault(DateTime.MinValue);
            objCurrentOrder.ChangeOrderCreated = objCurrentOrder.Created.GetValueOrDefault(DateTime.MinValue);


            objChangeOrders.Insert(0, objCurrentOrder);
            ViewBag.OrderGuid = OrderGuid.ToString();
            return PartialView("_ChangeOrderList", objChangeOrders);
        }

        /// <summary>
        /// ChangeOrder
        /// </summary>
        /// <param name="OrderGuid"></param>
        /// <param name="OrdType"></param>
        /// <returns></returns>
        public PartialViewResult GetChangeOrderDetail(Guid ChangeOrderMasterGuid)
        {
            List<ToolAssetOrderDetailsDTO> objOrderDetailDTO = null;
            ToolAssetOrderDetailsDAL objDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            objOrderDetailDTO = objDAL.DB_GetChangeOrderDetailData(RoomID, CompanyID, false, false, null, null, ChangeOrderMasterGuid).ToList();

            ToolAssetOrderMasterDAL objOrdDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            ToolAssetOrderMasterDTO objOrderDTO = objOrdDAL.DB_GetChangeOrderData(CompanyID, RoomID, false, false, null, null, null, ChangeOrderMasterGuid, null).FirstOrDefault();
            objOrderDTO.OrderListItem = objOrderDetailDTO;
            //return RenderRazorViewToString("_ChangeOrderDetailList", objOrderDTO);
            return PartialView("_ChangeOrderDetailList", objOrderDTO);
        }

        /// <summary>
        /// ChangeOrder
        /// </summary>
        /// <param name="OrderGuid"></param>
        /// <param name="OrdType"></param>
        /// <returns></returns>
        public PartialViewResult GetCurrentOrderDetailInChangeOrderHistory(Guid ChangeOrderMasterGuid)
        {
            List<ToolAssetOrderDetailsDTO> objOrderDetailDTO = null;
            ToolAssetOrderDetailsDAL objDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            objOrderDetailDTO = objDAL.GetOrderedRecord(ChangeOrderMasterGuid, RoomID, CompanyID).ToList();

            ToolAssetOrderMasterDAL objOrdDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            ToolAssetOrderMasterDTO objOrderDTO = objOrdDAL.GetRecord(ChangeOrderMasterGuid, RoomID, CompanyID);
            objOrderDTO.OrderListItem = objOrderDetailDTO;
            return PartialView("_ChangeOrderDetailList", objOrderDTO);
        }

        /// <summary>
        /// TempUpdateOrderNumberForSorting
        /// </summary>
        /// <returns></returns>
        public JsonResult TempUpdateOrderNumberForSorting()
        {
            //ToolAssetOrderMasterDAL objOrderMasterDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            //objOrderMasterDAL.TempFillOrderNumberSorting();
            return Json(new { Message = "OK", Success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UncloseOrder(string OrderGUID)
        {
            string message = string.Empty;
            string status = string.Empty;
            ToolAssetOrderMasterDAL obj = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            ToolAssetOrderMasterDTO objDTO = new ToolAssetOrderMasterDTO();
            objDTO = obj.GetRecord(Guid.Parse(OrderGUID), SessionHelper.RoomID, SessionHelper.CompanyID);


            if (objDTO != null)
            {
                objDTO.OrderStatus = (int)ToolAssetOrderStatus.UnSubmitted;
                objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                try
                {
                    objDTO.WhatWhereAction = "ToolAssetOrder>>UncloseOrder";
                    bool IsUpdate = false;
                    objDTO.IsOnlyFromUI = true;
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    IsUpdate = obj.Edit(objDTO);
                    if (IsUpdate)
                    {
                        message = ResMessage.SaveMessage;
                        status = "ok";
                    }
                    else
                    {
                        message = ResMessage.SaveErrorMsg;
                        status = "fail";
                    }
                }
                catch (Exception)
                {
                    message = ResMessage.SaveErrorMsg;
                    status = "fail";
                }
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method called but plugin when a row is closed
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult CloseOrderDetailLineItems(string ids, string CallFrom)
        {
            ToolAssetOrderDetailsDAL orderDetailDAL = null;
            ToolAssetOrderDetailsDTO orderDetail = null;
            ToolAssetOrderMasterDTO order = null;
            ToolAssetOrderMasterDAL orderDAL = null;
            int orderStatus = 0;
            try
            {

                orderDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                orderDetailDAL.CloseOrderDetailItem(ids, SessionHelper.UserID, RoomID, CompanyID);

                if (!string.IsNullOrEmpty(CallFrom) && CallFrom.ToLower() == "order")
                {
                    string[] orderIDs = ids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (orderIDs != null && orderIDs.Length > 0)
                    {
                        orderDetail = orderDetailDAL.GetRecord(Int64.Parse(orderIDs[0]), RoomID, CompanyID);
                        orderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                        order = orderDAL.GetRecord(orderDetail.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                        orderStatus = order.OrderStatus;
                    }
                }

                return Json(new { Message = ResOrder.LineItemClosed, Status = "ok", OrderStatus = orderStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                orderDetailDAL = null;

            }
        }

        /// <summary>
        /// Method called but plugin when a row is closed
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult UnCloseOrderLineItems(string ids, string CallFrom)
        {
            ToolAssetOrderDetailsDAL orderDetailDAL = null;
            ToolAssetOrderDetailsDTO orderDetail = null;
            ToolAssetOrderMasterDTO order = null;
            ToolAssetOrderMasterDAL orderDAL = null;
            int orderStatus = 0;
            try
            {

                orderDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                orderDetailDAL.UnCloseOrderDetailItem(ids, SessionHelper.UserID, RoomID, CompanyID);

                if (!string.IsNullOrEmpty(CallFrom) && CallFrom.ToLower() == "order")
                {
                    string[] orderIDs = ids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (orderIDs != null && orderIDs.Length > 0)
                    {
                        orderDetail = orderDetailDAL.GetRecord(Int64.Parse(orderIDs[0]), RoomID, CompanyID);
                        orderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                        order = orderDAL.GetRecord(orderDetail.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                        orderStatus = order.OrderStatus;
                    }
                }

                return Json(new { Message = ResOrder.LineItemClosed, Status = "ok", OrderStatus = orderStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                orderDetailDAL = null;

            }
        }

        /// <summary>
        /// ToUnclosedOrder
        /// </summary>
        /// <param name="OrderGuid"></param>
        /// <returns></returns>
        public JsonResult ToUnclosedOrder(Guid OrderGuid)
        {
            //OrderDetailsDAL orderDetailDAL = null;
            ToolAssetOrderMasterDAL obj = null;
            ToolAssetOrderMasterDTO objDTO = null;
            try
            {
                //orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                //int ordStatus = (int)orderDetailDAL.UpdateOrderStatusByReceiveNew(OrderGuid, RoomID, CompanyID, SessionHelper.UserID, true);
                //if (ordStatus == (int)ToolAssetOrderStatus.Closed)
                //{
                //    UpdateOrderToTransmited(OrderGuid.ToString(), "Web", "ToUnclosedOrder");
                //}

                obj = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                objDTO = new ToolAssetOrderMasterDTO() { GUID = OrderGuid, RoomID = RoomID, CompanyID = CompanyID, EditedFrom = "Web", WhatWhereAction = "ToUnclosedToolAssetOrder" };
                objDTO = obj.DB_TransmitOrder(objDTO);
                return Json(new { Message = "Ok", Status = true, OrderStatus = objDTO.OrderStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                obj = null;
            }
        }
        #endregion

        #region Tool Asset Order Detail

        private string GetSessionKey(Int64 OrderID = 0)
        {
            string strKey = "OrderLineItem_" + SessionHelper.EnterPriceID + "_" + CompanyID + "_" + RoomID;
            return strKey;
        }

        private string GetSessionKeyForDeletedRecord(Int64 OrderID = 0)
        {
            string strKey = "DeletedOrderLineItem_" + SessionHelper.EnterPriceID + "_" + CompanyID + "_" + RoomID;
            return strKey;
        }

        private List<ToolAssetOrderDetailsDTO> GetDeletedLineItemsFromSession(Int64 OrderID)
        {

            List<ToolAssetOrderDetailsDTO> lstDetailDTO = new List<ToolAssetOrderDetailsDTO>();

            List<ToolAssetOrderDetailsDTO> lstDetails = (List<ToolAssetOrderDetailsDTO>)SessionHelper.Get(GetSessionKeyForDeletedRecord(OrderID));
            if (lstDetails != null && lstDetails.Count > 0)
            {
                lstDetailDTO = lstDetails;
            }

            return lstDetailDTO;
        }

        private List<ToolAssetOrderDetailsDTO> GetLineItemsFromSession(Int64 OrderID)
        {

            List<ToolAssetOrderDetailsDTO> lstDetailDTO = new List<ToolAssetOrderDetailsDTO>();

            List<ToolAssetOrderDetailsDTO> lstDetails = (List<ToolAssetOrderDetailsDTO>)SessionHelper.Get(GetSessionKey(OrderID));
            if (lstDetails != null && lstDetails.Count > 0)
            {
                lstDetailDTO = lstDetails;
            }

            return lstDetailDTO;
        }



        private List<ToolAssetOrderDetailsDTO> PreparedOrderLiteItemWithProperData(ToolAssetOrderMasterDTO objOrd)
        {
            ToolMasterDTO objItemMaster = null;
            ToolMasterDAL objItemDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolAssetOrderDetailsDTO> lstDetails = (List<ToolAssetOrderDetailsDTO>)SessionHelper.Get(GetSessionKey(objOrd.ID));
            if (lstDetails != null && lstDetails.Count > 0)
            {
                foreach (ToolAssetOrderDetailsDTO item in lstDetails)
                {
                    objItemMaster = null;
                    objItemMaster = objItemDAL.GetToolByGUIDPlain(item.ToolGUID.GetValueOrDefault(Guid.NewGuid()));
                    if (objItemMaster != null)
                    {
                        //objBinMaster = objBinDAL.GetBinByLocationNameItemGuid(objOrd.Room.GetValueOrDefault(0), objOrd.CompanyID.GetValueOrDefault(0), false, false, item.BinName, objItemMaster.GUID).FirstOrDefault();
                        item.ToolDescription = objItemMaster.Description;

                    }

                    //if (objBinMaster != null)
                    //{
                    //    item.Bin = objBinMaster.ID;
                    //    item.BinName = objItemMaster.BinNumber;
                    //}

                    item.RequiredDate = objOrd.RequiredDate;

                }


            }

            return lstDetails;
        }

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult LoadOrderLineItems(Int64 orderID)
        {
            ToolAssetOrderMasterDTO objDTO = null;
            if (orderID <= 0)
                objDTO = new ToolAssetOrderMasterDTO() { ID = orderID, OrderListItem = new List<ToolAssetOrderDetailsDTO>() };
            else
            {
                objDTO = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(orderID, RoomID, CompanyID);
                objDTO.OrderListItem = GetLineItemsFromSession(orderID);
                if (objDTO.OrderListItem == null || objDTO.OrderListItem.Count <= 0)
                {
                    objDTO.OrderListItem = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderedRecord(objDTO.GUID, RoomID, CompanyID, objDTO.IsArchived.GetValueOrDefault(false), objDTO.IsDeleted);
                    SessionHelper.Add(GetSessionKey(orderID), objDTO.OrderListItem);
                    Session["ToolAssetOrderDetail"] = objDTO.OrderListItem;
                }

                objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
                if (objDTO.IsDeleted || objDTO.IsArchived.GetValueOrDefault(false))
                {
                    objDTO.IsRecordNotEditable = true;
                    objDTO.IsOnlyStatusUpdate = false;
                    objDTO.IsAbleToDelete = true;
                }

                //if (objDTO.StagingID.GetValueOrDefault(0) > 0)
                //{
                //    MaterialStagingDTO objMSDTO = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetRecord(objDTO.StagingID.GetValueOrDefault(0), RoomID, CompanyID);
                //    objDTO.StagingDefaultLocation = objMSDTO.BinID;
                //    objDTO.StagingName = objMSDTO.StagingName;
                //    objMSDTO = null;
                //} 


                if (objDTO.OrderListItem != null && objDTO.OrderListItem.Count > 0)
                {
                    objDTO.NoOfLineItems = objDTO.OrderListItem.Count;
                    double? OrderCost = objDTO.OrderCost;
                    objDTO.OrderCost = objDTO.OrderListItem.Sum(x => double.Parse((x.ApprovedQuantity.GetValueOrDefault(0) == 0 ? x.RequestedQuantity.GetValueOrDefault(0) : x.ApprovedQuantity.GetValueOrDefault(0)).ToString()) * ((x.ToolCost.GetValueOrDefault(0))));
                    if (OrderCost != objDTO.OrderCost)
                        new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName).Edit(objDTO);
                }

            }
            //objDTO.OrderListItem.ForEach(ol => ol.BinName = Convert.ToString(ol.BinName) == "[|EmptyStagingBin|]" ? string.Empty : ol.BinName);
            //objDTO.OrderListItem.ForEach(ol => ol.RequiredDateStr = ol.RequiredDate != null ? ol.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat) : string.Empty);
            objDTO.OrderListItem.ForEach(x =>
            {
                x.RequiredDateStr = x.RequiredDate != null ? x.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
                x.ToolLocationName = Convert.ToString(x.ToolLocationName) == "[|EmptyStagingBin|]" ? string.Empty : x.ToolLocationName;
                //x.ExtendedCost = double.Parse((x.ApprovedQuantity.GetValueOrDefault(0) == 0 ? x.RequestedQuantity.GetValueOrDefault(0) : x.ApprovedQuantity.GetValueOrDefault(0)).ToString()) * ((x.Cost.GetValueOrDefault(0)) / ((x.CostUOMValue ?? 0) == 0 ? 1 : (x.CostUOMValue ?? 1)));
            });
            return PartialView("_OrderLineItems", objDTO);
        }

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult LoadItemMasterModel(Int64 ParentId, string ParentGuid, string ToolType)
        {
            ToolAssetOrderMasterDTO objDTO = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(ParentId, RoomID, CompanyID);

            string modelHeader = eTurns.DTO.ResToolAssetOrder.ItemModelHeader;
            //if (objDTO.OrderType == (int)ToolAssetOrderType.RuturnOrder)
            //{
            //    modelHeader = "Add Item(s) to Return Order.";
            //}
            Session["ToolORDType"] = ToolType;
            ItemModelPerameter obj = null;
            if (GetToolAssetOrderType == ToolAssetOrderType.Order)
            { 
                obj = new ItemModelPerameter()
                {
                    AjaxURLAddItemToSession = "~/ToolAssetOrder/AddToolToDetailTableKit/",
                    PerentID = Convert.ToString(ParentId),
                    PerentGUID = ParentGuid,
                    ModelHeader = ResToolAssetOrder.AddToolToOrder, 
                    AjaxURLAddMultipleItemToSession = "~/ToolAssetOrder/AddToolToDetailTableKit/",
                    AjaxURLToFillItemGrid = "~/ToolAssetOrder/GetItemsModelMethod/",
                    CallingFromPageName = "ToolAssetORD",

                    OrdRequeredDate = objDTO.RequiredDate.ToString("MM/dd/yyyy"),
                    OrderStatus = objDTO.OrderStatus,
                };
            }
            //else if (GetToolAssetOrderType == ToolAssetOrderType.RuturnOrder)
            //{
            //    obj = new ItemModelPerameter()
            //    {
            //        AjaxURLAddItemToSession = "~/ToolAssetOrder/AddItemsToOrder/", // Not Used
            //        PerentID = ParentId.ToString(),
            //        PerentGUID = objDTO.GUID.ToString(),
            //        ModelHeader = modelHeader,
            //        AjaxURLAddMultipleItemToSession = "~/ToolAssetOrder/AddItemsToOrder/",
            //        AjaxURLToFillItemGrid = "~/ToolAssetOrder/GetItemsModelMethod/",
            //        CallingFromPageName = "RETORD",
            //        OrdStagingID = objDTO.StagingID.ToString(),
            //        OrdRequeredDate = objDTO.RequiredDate.ToString("MM/dd/yyyy"),
            //        OrderStatus = objDTO.OrderStatus,
            //    };
            //}



            return PartialView("ToolMasterModel", obj);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetItemsModelMethod(QuickListJQueryDataTableParamModel param)
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

            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            if (Request["sSearch_0"] != null && !string.IsNullOrEmpty(Request["sSearch_0"]))
            {
                param.sSearch = Request["sSearch_0"].Trim(',');
            }

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            Int64 ORDID = 0;
            Int64.TryParse(Request["ParentID"], out ORDID);
            ToolAssetOrderMasterDTO objMasterDTO = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(ORDID, RoomID, CompanyID);
            List<ToolAssetOrderDetailsDTO> lstItems = GetLineItemsFromSession(ORDID);
            string ToolIDs = "";

            if (lstItems != null && lstItems.Count > 0)
            {
                foreach (var item in lstItems)
                {
                    if (!string.IsNullOrEmpty(ToolIDs))
                        ToolIDs += ",";

                    ToolIDs += item.ToolGUID.ToString();
                }
            }

            ToolMasterDAL obj = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);
            string ToolType = Request["ToolType"].ToString();
            Session["ToolORDType"] = ToolType;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedToolsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "", "", RoomDateFormat, CurrentTimeZone, ToolIDs, Type: ToolType, CalledPage: "FromOrderPage");
            DataFromDB = DataFromDB.Where(t => t.IsGroupOfItems != 0).ToList();
            var jsonResult = Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;

        }
        public JsonResult AddToolToDetailTableKit(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            ToolAssetOrderDetailsDTO[] ToolDetails = s.Deserialize<ToolAssetOrderDetailsDTO[]>(para);
            Int64 ToolAssetOrderId = 0;

            if (ToolDetails != null)
            {
                ToolAssetOrderId = ToolDetails.FirstOrDefault().ToolID ?? 0;
            }

            int sessionsr = 0;
            ToolAssetOrderMasterDAL objToolAssetOrderMasterDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            double? RequestQty = 0;

            foreach (ToolAssetOrderDetailsDTO item in ToolDetails)
            {
                RequestQty = item.RequestedQuantity;
                item.Room = SessionHelper.RoomID;
                item.Serial = item.Serial == "null" ? string.Empty : item.Serial;
                item.ToolAssetOrderGUID = item.ToolGUID;
                item.ToolGUID = item.ToolItemGUID;
                item.RoomName = SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;
                item.LastUpdated = DateTimeUtility.DateTimeNow;
                item.RequestedQuantity = item.QuantityPerKit;
                item.AddedFrom = "Web";
                item.EditedFrom = "Web";
                if (!(item.GUID != null && item.GUID != Guid.Empty))
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    ToolMasterDTO ObjToolDTO = objToolDAL.GetToolByGUIDPlain(item.ToolGUID.GetValueOrDefault(Guid.Empty));
                    if (ObjToolDTO != null) //&& ObjToolDTO.Type != 2
                    {
                        item.ToolGUID = ObjToolDTO.GUID;
                        item.ToolQuantity = ObjToolDTO.Quantity;
                        List<ToolAssetOrderDetailsDTO> lstToolDetail = null;
                        if (Session["ToolAssetOrderDetail"] != null)
                        {
                            lstToolDetail = (List<ToolAssetOrderDetailsDTO>)Session["ToolAssetOrderDetail"];
                            item.SessionSr = lstToolDetail.Count + 1;
                        }
                        else
                        {

                            item.SessionSr = sessionsr + 1;
                            lstToolDetail = new List<ToolAssetOrderDetailsDTO>();
                        }
                        lstToolDetail.Add(item);
                        Session["ToolAssetOrderDetail"] = lstToolDetail;
                        SessionHelper.Add(GetSessionKey(ToolAssetOrderId), lstToolDetail);

                    }

                }
            }

            message = ResToolAssetOrder.ToolAddedToOrderSuccessfully;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public List<DTOForAutoComplete> GetBinsOfItemByOrderId(string StagingName, string NameStartWith, string ItemGUID, bool QtyRequired = false, bool OnlyHaveQty = false, Int64 OrderId = 0)
        {
            List<string> dtoList = new List<string>();
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();

            if (QtyRequired == false)
            {
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber);
                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, false, string.Empty, null, null).OrderBy(x => x.BinNumber);
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,false).OrderBy(x => x.BinNumber);//.Where(x => !x.IsStagingLocation)
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
                        if (!string.IsNullOrEmpty(item.BinNumber) && item.BinNumber.Trim().Length > 0)
                            locations.Add(objAutoDTO);
                    }

                    /*
                    List<OrderDetailsDTO> lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get("OrderLineItem_" + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID);
                    if (lstDetails != null && lstDetails.Count > 0)
                    {
                        foreach (OrderDetailsDTO o in lstDetails)
                        {
                            if (o.ItemGUID.GetValueOrDefault(Guid.Empty).ToString() == ItemGUID)
                                locations.RemoveAll(a => a.ID == o.Bin);
                        }
                    }
                    */
                }
            }

            return locations;
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult OrderLineItemsDelete(ToolAssetOrderDetailsDTO[] objDeletedItems, Int64 OrderID)
        {
            string message = "";
            string status = "";
            string orderItems = "0";
            string ordercost = "0";
            try
            {
                //---------------------------------------------
                //


                //---------------------------------------------
                //
                if (objDeletedItems != null && objDeletedItems.Length > 0)
                {
                    List<ToolAssetOrderDetailsDTO> lstDetailDTO = GetLineItemsFromSession(OrderID);
                    List<ToolAssetOrderDetailsDTO> lstDeletedItems = GetDeletedLineItemsFromSession(OrderID);
                    foreach (var item in objDeletedItems)
                    {
                        if (item.ID > 0)
                        {
                            lstDeletedItems.Add(item);
                        }
                        if (item.ID > 0)
                        {
                            lstDetailDTO.RemoveAll(x => x.ID == item.ID && x.ToolGUID == item.ToolGUID);
                        }
                        else if (item.tempDetailsGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            lstDetailDTO.RemoveAll(x => x.tempDetailsGUID == item.tempDetailsGUID && x.ToolGUID == item.ToolGUID);
                        }
                    }

                    SessionHelper.Add(GetSessionKeyForDeletedRecord(OrderID), lstDeletedItems);
                    SessionHelper.Add(GetSessionKey(OrderID), lstDetailDTO);
                    Session["ToolAssetOrderDetail"] = lstDetailDTO;
                    message = string.Format(ResCommon.RecordDeletedSuccessfully, objDeletedItems.Length);
                    status = "ok";
                    if (lstDetailDTO != null && lstDetailDTO.Count > 0)
                    {
                        orderItems = lstDetailDTO.Count.ToString();
                        double ordCost = 0;

                        foreach (var item in lstDetailDTO)
                        {
                            if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            {

                                ordCost += (double)item.ApprovedQuantity.GetValueOrDefault(0) * (double)item.ToolCost.GetValueOrDefault(0);

                            }
                            else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordCost += (double)item.RequestedQuantity.GetValueOrDefault(0) * (double)item.ToolCost.GetValueOrDefault(0);
                            }
                        }

                        RegionSettingDAL objRegDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                        eTurnsRegionInfo objRegionInfo = objRegDAL.GetRegionSettingsById(RoomID, CompanyID, SessionHelper.UserID);
                        if (objRegionInfo != null)
                        {
                            ordercost = ordCost.ToString("N" + objRegionInfo.CurrencyDecimalDigits.ToString(), SessionHelper.RoomCulture);
                        }
                        else
                            ordercost = ordCost.ToString("N" + "0", SessionHelper.RoomCulture);
                    }

                }
                else
                {
                    message = ResOrder.PleaseSelectRecord;
                    status = "fail";
                }
                //obj.DeleteRecords(ids, SessionHelper.UserID, RoomID, CompanyID);

                return Json(new { Message = message, Status = status, OrderItems = orderItems, Ordercost = ordercost }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }

        /// <summary>
        /// AddUpdateDeleteItemToOrder
        /// </summary>
        /// <param name="arrDetails"></param>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult AddUpdateDeleteOrderItemsToOrder(ToolAssetOrderDetailsDTO[] arrDetails, Int64 OrderID)
        {
            string message = "";
            string status = "";
            bool IsOrderClosedFromUnSubmitted = false;
            try
            {
                if (TempData["IsOrderClosedFromUnSubmitted"] != null)
                {
                    IsOrderClosedFromUnSubmitted = Convert.ToBoolean(TempData["IsOrderClosedFromUnSubmitted"]);
                }

                LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                List<ToolAssetOrderDetailsDTO> lstDeletedItems = GetDeletedLineItemsFromSession(0);
                string strDeletedIDs = string.Join(",", lstDeletedItems.Select(x => x.ID));
                if (!string.IsNullOrEmpty(strDeletedIDs))
                {
                    ToolAssetOrderDetailsDAL objDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    objDetailDAL.DeleteRecords(strDeletedIDs, SessionHelper.UserID, RoomID, CompanyID);
                }

                ToolAssetOrderDetailsDAL objDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);

                if (OrderID > 0)
                {
                    ToolAssetOrderMasterDTO objOrder = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(OrderID, RoomID, CompanyID);

                }
                if (arrDetails != null)
                {
                    LocationMasterDTO objBinMasterDTO;
                    ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    ToolLocationDetailsDTO objToolLocationDetailsDTO = null;
                    foreach (ToolAssetOrderDetailsDTO item in arrDetails)
                    {
                        if (IsOrderClosedFromUnSubmitted == true)
                            item.ApprovedQuantity = 0;

                        objBinMasterDTO = new LocationMasterDTO();
                        item.Room = RoomID;
                        item.RequiredDate = item.RequiredDateStr != null ? Convert.ToDateTime(DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture)) : Convert.ToDateTime(item.RequiredDateStr);
                        item.CompanyID = CompanyID;

                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(item.ToolGUID.GetValueOrDefault(Guid.Empty), item.ToolLocationName ?? string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolAssetOrderController>>AddUpdateDeleteOrderItemsToOrder");
                        if (objToolLocationDetailsDTO != null)
                        {
                            item.ToolBinID = objToolLocationDetailsDTO.ID;
                            //item.ToolLocationGuid = objBinMasterDTO.GUID;
                            //item.ToolLocationName = objBinMasterDTO.Location;
                        }
                        //item.Bin = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, SessionHelper.UserID, RoomID, CompanyID, IsStagingLoc);

                        item.LastUpdatedBy = SessionHelper.UserID;
                        if (item.ID > 0)
                        {

                            item.EditedFrom = "Web";
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                            item.LastUpdated = DateTimeUtility.DateTimeNow;
                            objDAL.Edit(item);
                        }
                        else
                        {
                            item.CreatedBy = SessionHelper.UserID;
                            if (item.RequiredDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue)
                                item.RequiredDate = DateTimeUtility.DateTimeNow;
                            item.AddedFrom = "Web";
                            item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            item.EditedFrom = "Web";
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;

                            objDAL.Insert(item);
                        }
                    }

                }

                SessionHelper.RomoveSessionByKey(GetSessionKey(0));
                SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
                Session["ToolAssetOrderDetail"] = null;
                message = ResCommon.RecordsSavedSuccessfully;
                status = "ok";

                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = "Error";
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }

        [HttpPost]
        public JsonResult SaveLatestValueInSession(ToolAssetOrderDetailsDTO[] arrDetails, Int64 OrderID)
        {
            try
            {
                List<ToolAssetOrderDetailsDTO> objSessionOrderList = (List<ToolAssetOrderDetailsDTO>)SessionHelper.Get(GetSessionKey(OrderID));

                List<ToolAssetOrderDetailsDTO> objOrderList = arrDetails.ToList();
                foreach (var item in objOrderList)
                {
                    ToolAssetOrderDetailsDTO detailFromSession = objSessionOrderList.FirstOrDefault(x => x.ToolGUID == item.ToolGUID);
                    if (detailFromSession != null)
                    {
                        item.ToolName = detailFromSession.ToolName;
                        item.ToolCost = detailFromSession.ToolCost;
                        item.RequestedQuantity = detailFromSession.RequestedQuantity;

                        item.ToolQuantity = detailFromSession.ToolQuantity;

                        item.ToolDescription = detailFromSession.ToolDescription;
                        item.ApprovedQuantity = detailFromSession.ApprovedQuantity;

                        item.ODPackSlipNumbers = detailFromSession.ODPackSlipNumbers;
                    }
                }

                SessionHelper.Add(GetSessionKey(OrderID), objOrderList);
            }
            catch
            {

            }
            return Json(new { Message = ResOrder.AddedSuccessfully, Success = true }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Add New Item to Order
        /// </summary>
        /// <param name="objNewItems"></param>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult AddItemsToOrder(ToolAssetOrderDetailsDTO[] objNewItems, Int64 OrderID)
        {
            string message = ResOrder.NotInsert;
            string status = "fail";
            ToolAssetOrderMasterDTO objOrderDTO = null;
            ToolAssetOrderMasterDAL objOrderDAL = null;


            LocationMasterDAL objBinDAL = null;
            ToolMasterDTO objItemDTO = null;
            ToolMasterDAL objItemDAL = null;

            ToolAssetOrderDetailsDTO objNewDetailDTO = null;

            List<ToolAssetOrderDetailsDTO> lstReturnsForSameItemWithBin = null;
            List<DTOForAutoComplete> lstAddedItemsBin = null;
            try
            {
                objOrderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                List<ToolAssetOrderDetailsDTO> lstDetails = GetLineItemsFromSession(OrderID);
                objOrderDTO = objOrderDAL.GetRecord(OrderID, RoomID, CompanyID);
                string binName = string.Empty;
                Guid? binID = null;


                objBinDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                objItemDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);



                if (objNewItems != null && objNewItems.Length > 0)
                {
                    lstReturnsForSameItemWithBin = new List<ToolAssetOrderDetailsDTO>();
                    lstAddedItemsBin = new List<DTOForAutoComplete>();
                    foreach (var item in objNewItems)
                    {
                        binID = item.ToolLocationGuid;
                        binName = item.ToolLocationName;
                        if (string.IsNullOrEmpty(binName))
                            binName = item.ToolLocationName;

                        objItemDTO = objItemDAL.GetToolByGUIDPlain(item.ToolGUID.GetValueOrDefault(Guid.Empty));
                        List<ToolLocationDetailsDTO> lstItemBins = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetToolLocationsByToolGUID(RoomID, CompanyID, false, false, objItemDTO.GUID).OrderBy(x => x.ToolLocationName).ToList();
                        if (lstItemBins != null && lstItemBins.Where(x => x.LocationGUID == binID).Count() > 0 && String.IsNullOrEmpty(binName))
                        {
                            item.ToolLocationName = lstItemBins.Where(x => x.LocationGUID == binID).ToList()[0].ToolLocationName;
                            binName = item.ToolLocationName;
                        }


                        DTOForAutoComplete objAdd = new DTOForAutoComplete()
                        {
                            ItemGuid = objItemDTO.GUID,
                            Key = objItemDTO.ToolName,
                            ID = 0,
                            Value = "",
                        };

                        if (lstItemBins.Count() > 0)
                        {
                            ToolLocationDetailsDTO binDTO = lstItemBins.FirstOrDefault();
                            if (binDTO == null)
                            {
                                binDTO = lstItemBins.FirstOrDefault();
                            }

                            objAdd.ItemGuid = binDTO.LocationGUID;
                            objAdd.Value = binDTO.ToolLocationName;
                        }

                        lstAddedItemsBin.Add(objAdd);
                        lstDetails.Add(objNewDetailDTO);
                    }

                    SessionHelper.Add(GetSessionKey(OrderID), lstDetails);
                    if (lstReturnsForSameItemWithBin.Count <= 0)
                        message = string.Format(ResToolAssetOrder.ToolsAreAddedToOrder, objNewItems.Length);
                    else if (objNewItems.Length - lstReturnsForSameItemWithBin.Count > 0) 
                   message = string.Format(ResOrder.ItemsAddedAndExistInOrder, (objNewItems.Length - lstReturnsForSameItemWithBin.Count), lstReturnsForSameItemWithBin.Count);
                    else
                        message = ResOrder.NotAddedItemsExistInOrder;

                    status = "ok";
                    return Json(new { Message = message, Status = status, AlreadyExistsItems = lstReturnsForSameItemWithBin, AddedBins = lstAddedItemsBin }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = "Error";
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
            finally
            {
                objOrderDTO = null;
                objOrderDAL = null;

                //objBINDTO = null;
                objBinDAL = null;
                objItemDTO = null;
                objItemDAL = null;

                objNewDetailDTO = null;

            }
        }

        private ToolAssetOrderDetailsDTO UpdateOrderDetailWithFullInfo(ToolAssetOrderDetailsDTO item, ToolMasterDTO objItemDTO)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
            Guid? temp_DetailGUID = null;
            if (item.tempDetailsGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
            {
                temp_DetailGUID = Guid.NewGuid();
            }
            else
            {
                temp_DetailGUID = item.tempDetailsGUID;
            }

            ToolAssetOrderDetailsDTO objNewDetailDTO = new ToolAssetOrderDetailsDTO()
            {
                ID = 0,
                RequestedQuantity = item.RequestedQuantity,
                ApprovedQuantity = item.ApprovedQuantity.GetValueOrDefault(0),
                ToolAssetOrderGUID = item.ToolAssetOrderGUID,
                ToolGUID = item.ToolGUID,
                ToolName = objItemDTO.ToolName,
                ToolLocationGuid = item.ToolLocationGuid,

                ToolLocationName = item.ToolLocationName,
                RequiredDate = item.RequiredDate,
                ReceivedQuantity = item.ReceivedQuantity,
                Room = RoomID,
                CompanyID = CompanyID,

                ToolCost = objItemDTO.Cost,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                IsDeleted = false,
                IsEDIRequired = true,
                IsEDISent = false,
                IsArchived = false,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                AddedFrom = "Web",
                EditedFrom = "Web",
                ASNNumber = string.Empty,
                LastEDIDate = null,

                CreatedByName = SessionHelper.UserName,

                Action = string.Empty,

                GUID = Guid.Empty,
                HistoryID = 0,
                ImagePath = objItemDTO.ImagePath,
                InTransitQuantity = 0,

                IsBuildBreak = objItemDTO.IsBuildBreak,

                IsHistory = false,


                LastUpdated = DateTimeUtility.DateTimeNow,
                LastUpdatedBy = SessionHelper.UserID,

                RoomName = SessionHelper.RoomName,

                TotalRecords = 0,

                UDF1 = item.UDF1,
                UDF2 = item.UDF2,
                UDF3 = item.UDF3,
                UDF4 = item.UDF4,
                UDF5 = item.UDF5,
                Comment = item.Comment,
                tempDetailsGUID = temp_DetailGUID,


            };
            return objNewDetailDTO;
        }

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        //public ActionResult GetOrderLineItem(string OrderDTO)
        //{
        //    // Int64 sID = 0;
        //    //Int64.TryParse(supplierID, out sID);

        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    OrderMasterDTO objDTO = js.Deserialize<OrderMasterDTO>(OrderDTO);
        //    objDTO.OrderListItem = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderedRecord(objDTO.GUID, RoomID, CompanyID);
        //    List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
        //    if (objDTO.StagingID.GetValueOrDefault(0) <= 0)
        //        lstBins = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(RoomID, CompanyID, false, false).Where(x => x.IsStagingLocation == false).ToList();

        //    lstBins.Insert(0, new BinMasterDTO());
        //    ViewBag.BinList = lstBins;

        //    return PartialView("_OrderLineItems", objDTO);
        //}

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
        ///public JsonResult AddDetailItem(string para, Int64 ItemID, string ItemGUID, double pQuentity, Int64 QuickListID, string QuickListGuid)
        //public JsonResult AddItemToDetailTable(string para)
        //{
        //    string message = "";
        //    string status = "";
        //    try
        //    {

        //        List<OrderDetailsDTO> lstDeletedItems = GetDeletedLineItemsFromSession(0);
        //        string strDeletedIDs = string.Join(",", lstDeletedItems.Select(x => x.ID));
        //        if (!string.IsNullOrEmpty(strDeletedIDs))
        //        {
        //            OrderDetailsDAL objDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
        //            objDetailDAL.DeleteRecords(strDeletedIDs, SessionHelper.UserID, RoomID, CompanyID);
        //        }

        //        JavaScriptSerializer s = new JavaScriptSerializer();
        //        OrderDetailsDTO[] QLDetails = s.Deserialize<OrderDetailsDTO[]>(para);
        //        OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

        //        foreach (OrderDetailsDTO item in QLDetails)
        //        {
        //            item.Room = RoomID;
        //            item.RoomName = SessionHelper.RoomName;
        //            item.CreatedBy = SessionHelper.UserID;
        //            item.CreatedByName = SessionHelper.UserName;
        //            item.UpdatedByName = SessionHelper.UserName;
        //            item.LastUpdatedBy = SessionHelper.UserID;
        //            item.CompanyID = CompanyID;

        //            if (item.ID > 0)
        //            {
        //                objDAL.Edit(item);
        //            }
        //            else
        //            {
        //                if (item.RequiredDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue)
        //                    item.RequiredDate = DateTime.Now;

        //                List<OrderDetailsDTO> tempDTO = objDAL.GetCachedData(RoomID, CompanyID).Where(x => x.OrderGUID == item.OrderGUID && x.ItemGUID == item.ItemGUID && x.IsDeleted == false && x.IsArchived == false).ToList();

        //                try
        //                {
        //                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
        //                    item.Bin = objItemMasterDAL.GetRecord(item.ItemGUID.ToString(), RoomID, CompanyID).DefaultLocation;
        //                }
        //                catch { }
        //                if (tempDTO == null || tempDTO.Count == 0)
        //                    objDAL.Insert(item);
        //            }
        //        }

        //        SessionHelper.RomoveSessionByKey(GetSessionKey(0));
        //        SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));

        //        message = ResMessage.SaveMessage.Replace("Record", "Records");
        //        status = "ok";

        //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        message = "Error";
        //        status = "fail";
        //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //        //throw;
        //    }
        //}
        public JsonResult SaveLineItemBin(List<ToolAssetOrderDetailsDTO> lstBins, Guid OrderGuid)
        {
            foreach (var item in lstBins)
            {
                ToolAssetOrderMasterDAL objOrderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                ToolAssetOrderMasterDTO objOrderDTO = objOrderDAL.GetRecord(OrderGuid, RoomID, CompanyID);
                item.ToolBinID = null;
                if (!string.IsNullOrEmpty(item.ToolLocationName))
                {
                    ToolLocationDetailsDAL objBinDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    item.ToolBinID = objBinDAL.GetToolLocation(item.ToolGUID.GetValueOrDefault(Guid.Empty), item.ToolLocationName, RoomID, CompanyID, SessionHelper.UserID, "ToolAssetOrderController>>SaveLineItemBin").ID;
                }

                item.LastUpdatedBy = SessionHelper.UserID;

                ToolAssetOrderDetailsDAL objOrderDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                objOrderDetailDAL.UpdateLineItemBin(item);
            }
            return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Received

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult LoadOrderLineItemsForReceive(string orderID)
        {
            ToolAssetOrderMasterDTO objDTO = null;
            objDTO = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(Int64.Parse(orderID), RoomID, CompanyID);
            if (objDTO.OrderType.GetValueOrDefault(0) == 0)
                objDTO.OrderType = 1;

            objDTO.OrderListItem = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderedRecord(objDTO.GUID, RoomID, CompanyID, objDTO.IsArchived.GetValueOrDefault(false), objDTO.IsDeleted);

            //List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            //lstBins = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(RoomID, CompanyID, false, false).Where(x => x.IsStagingLocation == (objDTO.StagingID.GetValueOrDefault(0) > 0)).ToList();

            //lstBins.Insert(0, null);
            //ViewBag.BinList = lstBins;



            return PartialView("_ReceiveOrderLineItem", objDTO);

        }

        /// <summary>
        /// Receive Order Line Items 
        /// Currently this method not work for serial item.
        /// </summary>
        /// <param name="NewReceiveDetail"></param>
        /// <param name="OrderID"></param>
        /// <param name="IsStaging"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult ReceiveOrderedLineItems(BasicDetailForNewReceive[] NewReceiveDetail, Int64 OrderID, bool IsStaging)
        {
            ToolAssetOrderMasterDAL objOrdDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            //if (GetToolAssetOrderType == ToolAssetOrderType.RuturnOrder)
            //{
            //    return ReturnOrderedLineItems(NewReceiveDetail, OrderID, IsStaging);
            //}
            ToolAssetOrderMasterDTO OrdDTO = objOrdDAL.GetRecord(OrderID, RoomID, CompanyID);
            string responseMessage = string.Empty;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            KeyValDTO keyValDTO = null;
            string responseStatus = "false";
            if (NewReceiveDetail == null || NewReceiveDetail.Length < 0)
            {
                responseMessage = "<b style='color: Red;'>" + ResOrder.SelectOrderLineItemToReceive + "</b><br />";
            }
            else
            {

                int Index = 0;
                ToolAssetOrderDetailsDAL objDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                ToolAssetOrderDetailsDTO OrdDetailDTO = null;
                // List<ItemLocationDetailsDTO> lstLocationWiseDTO = null;
                //ItemLocationDetailsDTO itemLocDTO = null;

                List<ReceivedToolAssetOrderTransferDetailDTO> lstReceiveDTO = new List<ReceivedToolAssetOrderTransferDetailDTO>();
                ReceivedToolAssetOrderTransferDetailDTO receiveDTO = null;

                InventoryController invCtrlObj = new InventoryController();
                foreach (var item in NewReceiveDetail)
                {
                    Index += 1;

                    OrdDetailDTO = objDAL.GetRecord(item.OrderDetailID, RoomID, CompanyID);
                    string errorMsg = ValidateItemForReceive(item, OrdDetailDTO, Index, OrdDTO);
                    keyValDTO = new KeyValDTO();
                    keyValDTO.key = OrdDetailDTO.GUID.ToString();
                    if (errorMsg == "ok")
                    {
                        //itemLocDTO = GetItemLocationDetail(item, OrdDetailDTO);

                        //if (lstLocationWiseDTO == null)
                        //    lstLocationWiseDTO = new List<ItemLocationDetailsDTO>();

                        //lstLocationWiseDTO.Add(itemLocDTO);

                        receiveDTO = GetROTDDTO(item, OrdDetailDTO);
                        lstReceiveDTO.Add(receiveDTO);


                        keyValDTO.value = "Green"; 
                        responseMessage += "<b style='color: Green;'>" + string.Format(ResOrder.ItemReceivedSuccessfully, Index, OrdDetailDTO.ToolName) + "</br>";
                    }
                    else
                    {
                        responseMessage += errorMsg;
                        keyValDTO.value = "Olive";
                    }
                    lstKeyValDTO.Add(keyValDTO);
                }
                string OrdStatusText = string.Empty;
                //if (lstLocationWiseDTO != null && lstLocationWiseDTO.Count > 0)
                //{
                //    ItemLocationDetailsDAL itmLocDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                //    itmLocDAL.InsertItemLocationDetailsFromRecieve(lstLocationWiseDTO);

                //    objDAL.UpdateOrderStatusByReceive(OrdDTO.GUID, RoomID, CompanyID, SessionHelper.UserID, true);
                //    OrdDTO = objOrdDAL.GetRecord(OrderID, RoomID, CompanyID);
                //    responseStatus = "OK";
                //    responseMessage = ResMessage.SaveMessage.Replace("Record", "Records");
                //}

                if (lstReceiveDTO != null && lstReceiveDTO.Count > 0)
                {
                    ReceivedToolAssetOrderTransferDetailDAL objROTDDAL = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                    foreach (var item in lstReceiveDTO)
                    {
                        objROTDDAL.InsertReceive(item);
                    }
                    objDAL.UpdateOrderStatusByReceive(OrdDTO.GUID, RoomID, CompanyID, SessionHelper.UserID, true);
                    OrdDTO = objOrdDAL.GetRecord(OrderID, RoomID, CompanyID);
                    responseStatus = "OK";
                    responseMessage = ResCommon.RecordsSavedSuccessfully;
                }

                if (lstKeyValDTO.Count > 0)
                {
                    if (lstKeyValDTO.Where(x => x.value != "Green").Count() > 0)
                    {
                        responseMessage = "<b>" + ResOrder.SomeItemsNotReceivedDueToReason + "</b><br /><br />" + responseMessage;
                        if (lstKeyValDTO.Where(x => x.value == "Green").Count() > 0)
                        {
                            responseStatus = "true";
                        }
                    }
                    else
                    {
                        responseMessage = "<b>" + ResOrder.OrderReceivedSuccessfully + "</b><br /><br />" + responseMessage;
                        responseStatus = "true";
                    }
                }
            }


            return Json(new { Status = responseStatus, Message = responseMessage, RowColors = lstKeyValDTO.ToArray(), OrderStatus = OrdDTO.OrderStatus }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// ReceivedOrderDetail
        /// </summary>
        /// <returns></returns>
        public string ReceivedOrderDetail(string ToolGUID, string ordDetailGUID)
        {

            ViewBag.ToolGUID = ToolGUID;

            Guid? OrderDetailGUID = Guid.Parse(ordDetailGUID);

            ToolMasterDAL objItemAPI = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetToolByGUIDPlain(Guid.Parse(ToolGUID));
            ViewBag.ItemID = Objitem.ID;

            ReceivedToolAssetOrderTransferDetailDAL objAPI = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetAllRecords(RoomID, CompanyID, Guid.Parse(ToolGUID), OrderDetailGUID, "ID DESC");

            ToolAssetOrderDetailsDTO objOrderDetailDTO = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

            ViewBag.IsCloseItem = objOrderDetailDTO.IsCloseTool.GetValueOrDefault(false);

            ViewBag.ToolName = Objitem.ToolName;


            ViewBag.ItemGUID_ItemType = ToolGUID + "#" + Objitem.Type;
            ViewBag.OrderDetailGUID = OrderDetailGUID.GetValueOrDefault(Guid.Empty);
            return RenderRazorViewToString("_ReceivedOrderDetail", objModel);
        }




        /// <summary>
        /// ReceivedItemLocationsListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ReceivedItemLocationsListAjax(JQueryDataTableParamModel param)
        {

            string ToolGUID = string.Empty;//Request["ItemGUID"].ToString();
            Guid? OrderDetailGUID = null;
            if (!string.IsNullOrEmpty(Request["ToolAssetOrderDetailGUID"]) && Request["ToolAssetOrderDetailGUID"].Trim().Length > 0)
            {
                OrderDetailGUID = Guid.Parse(Request["ToolAssetOrderDetailGUID"]);
                if (OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    ToolAssetOrderDetailsDTO objOrdDtlDTO = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                    ToolGUID = objOrdDtlDTO.ToolGUID.GetValueOrDefault(Guid.Empty).ToString();
                }
            }


            ///////////// requried when paging needs in this method /////////////////
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            ViewBag.ToolGUID = ToolGUID;
            ToolMasterDAL objItemAPI = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetToolByGUIDPlain(Guid.Parse(ToolGUID));

            ViewBag.ToolID = Objitem.ID;



            ReceivedToolAssetOrderTransferDetailDAL objAPI = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);

            int TotalRecordCount = 0;

            var objModel = objAPI.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, Guid.Parse(ToolGUID), OrderDetailGUID);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = objModel
            },
                        JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// DeleteRecieveAndUpdateReceivedQty
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ordDetailID"></param>
        /// <param name="deleteIDs"></param>
        /// <returns></returns>
        public string DeleteRecieveAndUpdateReceivedQty(string ToolGUID, string ordDetailGUID, string deleteIDs)
        {
            ReceivedToolAssetOrderTransferDetailDAL objRecdOrdTrnDtlDAL = null;
            ReceivedToolAssetOrderTransferDetailDTO objRecdOrdTrnDtlDTO = null;

            ToolAssetOrderDetailsDAL ordDetailCtrl = null;
            ToolMasterDAL ItemDAL = null;

            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> objModel = null;
            ToolAssetOrderDetailsDTO objOrdDetailDTO = null;

            string returString = "fail";
            try
            {
                string[] rotdGUID = deleteIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (rotdGUID != null && rotdGUID.Length > 0)
                {
                    ItemDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);

                    ordDetailCtrl = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    objRecdOrdTrnDtlDAL = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                    foreach (string item in rotdGUID)
                    {
                        objRecdOrdTrnDtlDTO = objRecdOrdTrnDtlDAL.GetRecord(Guid.Parse(item), RoomID, CompanyID);

                        bool isChanged = false;


                        if (!isChanged)
                        {
                            objRecdOrdTrnDtlDAL.DeleteRecords(item, SessionHelper.UserID, RoomID, CompanyID);
                            objModel = objRecdOrdTrnDtlDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(ToolGUID), Guid.Parse(ordDetailGUID), "ID DESC");
                            var aax = objModel.Sum(x => (x.Quantity.GetValueOrDefault(0)));

                            objOrdDetailDTO = ordDetailCtrl.GetRecord(Guid.Parse(ordDetailGUID), RoomID, CompanyID);
                            objOrdDetailDTO.ReceivedQuantity = aax;
                            ordDetailCtrl.Edit(objOrdDetailDTO);
                            ordDetailCtrl.UpdateOrderStatusByReceive(objOrdDetailDTO.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), objOrdDetailDTO.Room.GetValueOrDefault(0), objOrdDetailDTO.CompanyID.GetValueOrDefault(0), objOrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0));
                            returString = "ok";
                        }
                    }
                }
                return returString;
            }
            catch (Exception)
            {
                return "fail";
            }
            finally
            {
                objRecdOrdTrnDtlDAL = null;
                objRecdOrdTrnDtlDTO = null;

                ordDetailCtrl = null;
                ItemDAL = null;

                objModel = null;
                objOrdDetailDTO = null;
            }

            //return "fail";
        }

        public ActionResult OpenPopupToReturnItem(List<ItemToReturnDTO> returnInfo)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,InventoryConsuptionMethod";
            RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            foreach (var item in returnInfo)
            {
                item.InventoryConsuptionMethod = roomDTO.InventoryConsuptionMethod;
            }
            return PartialView("_ItemToReturn", returnInfo);
        }

        #endregion

        #region Private General Methods

        #region Private methods for Receive

        /// <summary>
        /// Validate Receive item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ErrorSrNo"></param>
        /// <returns></returns>
        private string ValidateItemForReceive(BasicDetailForNewReceive item, ToolAssetOrderDetailsDTO OrdDetailDTO, int ErrorSrNo, ToolAssetOrderMasterDTO OrderMasterDTO)
        {
            string returnString = "";



            if (OrdDetailDTO.IsCloseTool.GetValueOrDefault(false)) 
            {
                returnString += "<b style='color: Olive'>" + string.Format(ResOrder.ClosedItemCantReceiveClickInlineUncloseButton, ErrorSrNo, OrdDetailDTO.ToolName) + " </b> <br />";
                return returnString;
            }

            bool IsLotErrorMsg = false;
            bool IsExprieErrorMsg = false;
            bool ISQtyErrorMsg = false;
            bool IsBinErrorMsg = false;
            bool IsUDFRequired = false;
            bool IsPackSlipRequiredErrorMsg = false;

            double qty = 0;
            double.TryParse(item.Quantity, out qty);
            if (qty <= 0)
                ISQtyErrorMsg = true;

            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedOrderTransferDetail", RoomID, CompanyID);
            string udfRequier = string.Empty;

            foreach (var i in DataFromDB)
            {
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(item.UDF1))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier += string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        IsUDFRequired = true;
                    }
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(item.UDF2))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier += string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        IsUDFRequired = true;
                    }
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(item.UDF3))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier += string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        IsUDFRequired = true;
                    }
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(item.UDF4))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier += string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        IsUDFRequired = true;
                    }
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(item.UDF5))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier += string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        IsUDFRequired = true;
                    }

                    if (IsUDFRequired)
                        break;
                
            }


            if (!IsLotErrorMsg && !IsExprieErrorMsg && !ISQtyErrorMsg && !IsBinErrorMsg && !IsUDFRequired && !IsPackSlipRequiredErrorMsg)
            {
                returnString = "ok";
            }
            else
            {
                returnString = "<b style='color: Olive;'>" + ErrorSrNo + ". " + OrdDetailDTO.ToolName + ":";
                List<string> arrErrors = new List<string>();


                if (ISQtyErrorMsg)
                    arrErrors.Add(ResReceiveToolAssetOrderDetails.ReceiveQuantity);

                if (IsUDFRequired)
                    arrErrors.Add(udfRequier);



                returnString += " ";
                for (int i = 0; i < arrErrors.Count; i++)
                {
                    if (arrErrors.Count > 1 && (i + 1) == arrErrors.Count)
                        returnString += " " + ResCommon.And + " ";
                    else if (i > 0)
                        returnString += ResCommon.Comma + " ";

                    returnString += arrErrors[i];
                }

                returnString += " ";
                if (arrErrors.Count > 1)
                    returnString += ResCommon.Are;
                else
                    returnString += ResCommon.Is;

                returnString += " " + ResCommon.Mandatory + "  </b> <br />";
            }

            return returnString;
        }

        /// <summary>
        /// Validate Receive item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ErrorSrNo"></param>
        /// <returns></returns>
        private string ValidateItemForReturn(BasicDetailForNewReceive item, OrderDetailsDTO OrdDetailDTO, int ErrorSrNo, Int64 BinID)
        {
            string returnString = "ok";

            bool ISQtyErrorMsg = false;
            bool IsBinErrorMsg = false;
            bool IsPackSlipRequiredErrorMsg = false;

            //if (SessionHelper.CompanyConfig != null && SessionHelper.CompanyConfig.IsPackSlipRequired.GetValueOrDefault(false))
            //{
            //    if (string.IsNullOrEmpty(item.PackSlipNumber) || string.IsNullOrWhiteSpace(item.PackSlipNumber))
            //        IsPackSlipRequiredErrorMsg = true;
            //}


            ItemMasterDTO objItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, OrdDetailDTO.ItemGUID);
            if (objItemDTO != null && objItemDTO.IsPackslipMandatoryAtReceive && (string.IsNullOrEmpty(item.PackSlipNumber) || string.IsNullOrWhiteSpace(item.PackSlipNumber)))
                IsPackSlipRequiredErrorMsg = true;


            double qty = 0;
            double.TryParse(item.Quantity, out qty);
            if (qty <= 0)
                ISQtyErrorMsg = true;

            if (string.IsNullOrEmpty(item.BinName))
                IsBinErrorMsg = true;


            if (!ISQtyErrorMsg && !IsBinErrorMsg && !IsPackSlipRequiredErrorMsg)
            {
                OrderMasterDTO objOrdDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByGuidPlain(OrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));

                if (objOrdDTO.StagingID.GetValueOrDefault(0) > 0)
                {
                    MaterialStagingDTO msDTO = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetRecord(objOrdDTO.StagingID.GetValueOrDefault(0), RoomID, CompanyID);
                    MaterialStagingDetailDTO msDtlDTO = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName).GetMaterialStagingDetailbyItemGUIDANDStagingBINID(msDTO.GUID, BinID, OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                    if (msDtlDTO == null || msDtlDTO.Quantity < double.Parse(item.Quantity))
                    {
                        returnString = "<b style='color: Olive;'>" + ErrorSrNo + ". " + OrdDetailDTO.ItemNumber + ":";
                        returnString += (" " + string.Format(ResOrder.StagingBinHaveNotSufficientQtyToReturn, item.BinName));
                    }
                }
                else
                {

                    ItemLocationQTYDAL objLocQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                    ItemLocationQTYDTO objItemLocQtyDTO = objLocQtyDAL.GetRecordByBinItem(OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), BinID, RoomID, CompanyID);
                    if (objItemLocQtyDTO == null || objItemLocQtyDTO.Quantity < double.Parse(item.Quantity))
                    {
                        returnString = "<b style='color: Olive;'>" + ErrorSrNo + ". " + OrdDetailDTO.ItemNumber + ":";
                        returnString += (" " + string.Format(ResOrder.BinHaveNotSufficientQtyToReturn, item.BinName));
                    }
                }
            }
            else
            {
                returnString = "<b style='color: Olive;'>" + ErrorSrNo + ". " + OrdDetailDTO.ItemNumber + ":";
                List<string> arrErrors = new List<string>();

                if (ISQtyErrorMsg)
                    arrErrors.Add(ResOrder.QuantityToReturn);

                if (IsBinErrorMsg)
                    arrErrors.Add(ResOrder.Bin);

                if (IsPackSlipRequiredErrorMsg)
                    arrErrors.Add(ResOrder.PackSlipNumber);

                returnString += " ";
                for (int i = 0; i < arrErrors.Count; i++)
                {
                    if (arrErrors.Count > 1 && (i + 1) == arrErrors.Count)
                        returnString += " " + ResCommon.And + " ";
                    else if (i > 0)
                        returnString += ResCommon.Comma + " ";

                    returnString += arrErrors[i];
                }
                returnString += " ";
                if (arrErrors.Count > 1)
                    returnString += ResCommon.Are;
                else
                    returnString += ResCommon.Is;

                returnString += " " + ResCommon.Mandatory + "  </b> <br />";
            }

            return returnString;
        }

        /// <summary>
        /// GetItemLocationDetail DTO
        /// </summary>
        /// <param name="OrdDetailDTO"></param>
        /// <returns></returns>
        //private ItemLocationDetailsDTO GetItemLocationDetail(BasicDetailForNewReceive item, OrderDetailsDTO OrdDetailDTO)
        //{
        //    ItemLocationDetailsDTO itemLocDTO = new ItemLocationDetailsDTO();
        //    ItemLocationDetailsDAL ItemLocationDetailsDAL = new eTurns.DAL.ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
        //    //itemLocDTO.BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByName(OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, SessionHelper.UserID, RoomID, CompanyID);
        //    BinMasterDTO objBinMasterDTO = ItemLocationDetailsDAL.GetItemBin(OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, RoomID, CompanyID, SessionHelper.UserID, false);
        //    itemLocDTO.BinID = objBinMasterDTO.ID;
        //    itemLocDTO.BinNumber = objBinMasterDTO.BinNumber;
        //    double dcost = 0;
        //    double.TryParse(item.Cost, out dcost);
        //    itemLocDTO.Cost = dcost;
        //    itemLocDTO.OrderDetailGUID = OrdDetailDTO.GUID;
        //    itemLocDTO.Received = item.ReceiveDate;
        //    itemLocDTO.ReceivedDate = item.ReceiveDate != null ? Convert.ToDateTime(DateTime.ParseExact(item.ReceiveDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)) : Convert.ToDateTime(item.ReceiveDate);//DateTime.Parse(item.ReceiveDate);

        //    if (OrdDetailDTO.Consignment)
        //        itemLocDTO.ConsignedQuantity = double.Parse(item.Quantity);
        //    else
        //        itemLocDTO.CustomerOwnedQuantity = double.Parse(item.Quantity);

        //    if (OrdDetailDTO.LotNumberTracking)
        //    {
        //        itemLocDTO.LotNumber = item.LotNumber;
        //        itemLocDTO.LotNumberTracking = OrdDetailDTO.LotNumberTracking;
        //    }
        //    if (OrdDetailDTO.DateCodeTracking)
        //    {
        //        itemLocDTO.DateCodeTracking = OrdDetailDTO.DateCodeTracking;
        //        itemLocDTO.Expiration = item.ExpirationDate;
        //        itemLocDTO.ExpirationDate = item.ExpirationDate != null ? DateTime.ParseExact(item.ExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult) : Convert.ToDateTime(item.ExpirationDate);//DateTime.Parse(item.ExpirationDate);
        //    }

        //    itemLocDTO.ItemGUID = OrdDetailDTO.ItemGUID;
        //    itemLocDTO.ItemType = OrdDetailDTO.ItemType;

        //    if (OrdDetailDTO.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
        //    {
        //        if (objBinMasterDTO != null)
        //        {
        //            itemLocDTO.CriticalQuantity = objBinMasterDTO.CriticalQuantity;
        //            itemLocDTO.MinimumQuantity = objBinMasterDTO.MinimumQuantity;
        //            itemLocDTO.MaximumQuantity = objBinMasterDTO.MaximumQuantity;
        //        }
        //    }

        //    itemLocDTO.CreatedBy = SessionHelper.UserID;
        //    itemLocDTO.LastUpdatedBy = SessionHelper.UserID;
        //    itemLocDTO.Room = RoomID;
        //    itemLocDTO.CompanyID = CompanyID;
        //    itemLocDTO.PackSlipNumber = item.PackSlipNumber;

        //    itemLocDTO.UDF1 = item.UDF1;
        //    itemLocDTO.UDF2 = item.UDF2;
        //    itemLocDTO.UDF3 = item.UDF3;
        //    itemLocDTO.UDF4 = item.UDF4;
        //    itemLocDTO.UDF5 = item.UDF5;

        //    return itemLocDTO;
        //}

        /// <summary>
        /// GetItemLocationDetail DTO
        /// </summary>
        /// <param name="OrdDetailDTO"></param>
        /// <returns></returns>
        private ReceivedToolAssetOrderTransferDetailDTO GetROTDDTO(BasicDetailForNewReceive item, ToolAssetOrderDetailsDTO OrdDetailDTO)
        {
            ReceivedToolAssetOrderTransferDetailDTO pbjROTDDTO = new ReceivedToolAssetOrderTransferDetailDTO();
            ToolLocationDetailsDAL ItemLocationDetailsDAL = new eTurns.DAL.ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            //itemLocDTO.BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByName(OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, SessionHelper.UserID, RoomID, CompanyID);


            double dcost = 0;
            double.TryParse(item.Cost, out dcost);
            pbjROTDDTO.Cost = dcost;
            pbjROTDDTO.OrderDetailGUID = OrdDetailDTO.GUID;
            pbjROTDDTO.Received = item.ReceiveDate;
            pbjROTDDTO.ReceivedDate = item.ReceiveDate != null ? Convert.ToDateTime(DateTime.ParseExact(item.ReceiveDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)) : Convert.ToDateTime(item.ReceiveDate);//DateTime.Parse(item.ReceiveDate);

            //if (OrdDetailDTO.Consignment)
            //    pbjROTDDTO.ConsignedQuantity = double.Parse(item.Quantity);
            //else
            //    pbjROTDDTO.CustomerOwnedQuantity = double.Parse(item.Quantity);


            pbjROTDDTO.ToolGUID = OrdDetailDTO.ToolGUID;
            pbjROTDDTO.Type = Convert.ToInt32(OrdDetailDTO.ToolType);



            pbjROTDDTO.CreatedBy = SessionHelper.UserID;
            pbjROTDDTO.LastUpdatedBy = SessionHelper.UserID;
            pbjROTDDTO.Room = RoomID;
            pbjROTDDTO.CompanyID = CompanyID;
            pbjROTDDTO.PackSlipNumber = item.PackSlipNumber;

            pbjROTDDTO.UDF1 = item.UDF1;
            pbjROTDDTO.UDF2 = item.UDF2;
            pbjROTDDTO.UDF3 = item.UDF3;
            pbjROTDDTO.UDF4 = item.UDF4;
            pbjROTDDTO.UDF5 = item.UDF5;

            return pbjROTDDTO;
        }


        /// <summary>
        /// MaterialStagingPullDetailDTO DTO
        /// </summary>
        /// <param name="OrdDetailDTO"></param>
        /// <returns></returns>
        //private MaterialStagingPullDetailDTO GetMaterialStagingPullDetail(BasicDetailForNewReceive item, OrderDetailsDTO OrdDetailDTO)
        //{
        //    MaterialStagingPullDetailDTO itemLocDTO = new MaterialStagingPullDetailDTO();
        //    ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
        //    BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBin(OrdDetailDTO.ItemGUID ?? Guid.Empty, item.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);
        //    itemLocDTO.BinID = objBinMasterDTO.ID;
        //    //itemLocDTO.BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByName(OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, SessionHelper.UserID, RoomID, CompanyID, true);

        //    double dcost = 0;
        //    double.TryParse(item.Cost, out dcost);
        //    itemLocDTO.ItemCost = dcost;
        //    itemLocDTO.OrderDetailGUID = OrdDetailDTO.GUID;
        //    itemLocDTO.Received = item.ReceiveDate;

        //    if (OrdDetailDTO.Consignment)
        //        itemLocDTO.ConsignedQuantity = double.Parse(item.Quantity);
        //    else
        //        itemLocDTO.CustomerOwnedQuantity = double.Parse(item.Quantity);

        //    if (OrdDetailDTO.LotNumberTracking)
        //    {
        //        itemLocDTO.LotNumber = item.LotNumber;
        //        itemLocDTO.LotNumberTracking = OrdDetailDTO.LotNumberTracking;
        //    }
        //    if (OrdDetailDTO.DateCodeTracking)
        //    {
        //        itemLocDTO.DateCodeTracking = OrdDetailDTO.DateCodeTracking;
        //        itemLocDTO.Expiration = item.ExpirationDate;
        //    }

        //    itemLocDTO.ItemGUID = OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty);

        //    itemLocDTO.CreatedBy = SessionHelper.UserID;
        //    itemLocDTO.LastUpdatedBy = SessionHelper.UserID;
        //    itemLocDTO.Room = RoomID;
        //    itemLocDTO.CompanyID = CompanyID;
        //    itemLocDTO.PackSlipNumber = item.PackSlipNumber;

        //    itemLocDTO.UDF1 = item.UDF1;
        //    itemLocDTO.UDF2 = item.UDF2;
        //    itemLocDTO.UDF3 = item.UDF3;
        //    itemLocDTO.UDF4 = item.UDF4;
        //    itemLocDTO.UDF5 = item.UDF5;

        //    return itemLocDTO;
        //}


        #endregion

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
        /// GetOrderStatusList
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        private List<SelectListItem> GetOrderStatusList(ToolAssetOrderMasterDTO objDTO, string Mode)
        {
            int CurrentStatus = objDTO.OrderStatus;

            List<SelectListItem> returnList = new List<SelectListItem>();

            if (Mode.ToLower() == "create")
            {
                returnList.Add(new SelectListItem() { Text = ResToolAssetOrder.GetOrderStatusText(OrderStatus.UnSubmitted.ToString()), Value = ((int)ToolAssetOrderStatus.UnSubmitted).ToString() });
            }
            else if (Mode.ToLower() == "edit")
            {
                foreach (var item in Enum.GetValues(typeof(ToolAssetOrderStatus)))
                {
                    string itemText = item.ToString();
                    int itemValue = (int)(Enum.Parse(typeof(ToolAssetOrderStatus), itemText));
                    if (itemValue < (int)ToolAssetOrderStatus.Transmitted && itemValue >= CurrentStatus)
                    {
                        if (returnList.FindIndex(x => int.Parse(x.Value) == itemValue) < 0)
                            returnList.Add(new SelectListItem() { Text = ResToolAssetOrder.GetOrderStatusText(item.ToString()), Value = itemValue.ToString() });
                    }
                    //else if (CurrentStatus == (int)ToolAssetOrderStatus.Transmitted && IsChangeOrder && !objDTO.OrderIsInReceive)
                    //{
                    //    if (returnList.FindIndex(x => int.Parse(x.Value) == (int)ToolAssetOrderStatus.UnSubmitted) < 0)
                    //    {
                    //        returnList.Add(new SelectListItem() { Text = ResToolAssetOrder.GetOrderStatusText(ToolAssetOrderStatus.UnSubmitted.ToString()), Value = ((int)ToolAssetOrderStatus.UnSubmitted).ToString() });
                    //        string strOrderNumber = objDTO.ToolAssetOrderNumber;
                    //        string[] ordRevNumber = strOrderNumber.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    //        if (ordRevNumber.Length == 1)
                    //        {
                    //            strOrderNumber = strOrderNumber + "_R1";

                    //        }
                    //        else if (ordRevNumber.Length > 1)
                    //        {
                    //            int RevNo = int.Parse(ordRevNumber[ordRevNumber.Length - 1].Replace("R", ""));
                    //            if (ordRevNumber.Length > 2)
                    //            {
                    //                for (int i = 0; i < ordRevNumber.Length - 2; i++)
                    //                {
                    //                    if (i > 0)
                    //                        strOrderNumber += "_";
                    //                    strOrderNumber += ordRevNumber[i];
                    //                }
                    //                strOrderNumber += "_R" + (RevNo + 1);
                    //            }
                    //            else
                    //            {
                    //                strOrderNumber = ordRevNumber[0] + "_R" + (RevNo + 1);
                    //            }


                    //        }
                    //        objDTO.ToolAssetOrderNumber = strOrderNumber;
                    //        break;
                    //    }
                    //}
                }

                if (objDTO.OrderStatus != (int)ToolAssetOrderStatus.Approved && returnList.FindIndex(x => x.Value == ((int)ToolAssetOrderStatus.Approved).ToString()) >= 0 && !IsApprove)
                {
                    returnList.RemoveAt(returnList.FindIndex(x => x.Value == ((int)ToolAssetOrderStatus.Approved).ToString()));
                }

                if (objDTO.OrderStatus != (int)ToolAssetOrderStatus.Submitted && returnList.FindIndex(x => x.Value == ((int)ToolAssetOrderStatus.Submitted).ToString()) >= 0 && !IsSubmit)
                {
                    returnList.RemoveAt(returnList.FindIndex(x => x.Value == ((int)ToolAssetOrderStatus.Submitted).ToString()));
                }


                if (returnList.FindIndex(x => x.Value == ((int)ToolAssetOrderStatus.Approved).ToString()) >= 0)
                {

                    returnList.Add(new SelectListItem() { Text = ResToolAssetOrder.GetOrderStatusText(ToolAssetOrderStatus.Closed.ToString()), Value = ((int)ToolAssetOrderStatus.Closed).ToString() });
                }


            }
            else
            {
                foreach (var item in Enum.GetValues(typeof(ToolAssetOrderStatus)))
                {
                    string itemText = item.ToString();
                    int itemValue = (int)(Enum.Parse(typeof(ToolAssetOrderStatus), itemText));
                    //if (CurrentStatus <= itemValue && itemValue <= CurrentStatus + 1)
                    returnList.Add(new SelectListItem() { Text = ResToolAssetOrder.GetOrderStatusText(item.ToString()), Value = itemValue.ToString() });
                }
            }

            if (Mode.ToLower() != "create" && !(returnList.FindIndex(x => x.Value == ((int)ToolAssetOrderStatus.Closed).ToString()) >= 0))
            {
                returnList.Add(new SelectListItem() { Text = ResToolAssetOrder.GetOrderStatusText(ToolAssetOrderStatus.Closed.ToString()), Value = ((int)ToolAssetOrderStatus.Closed).ToString() });
            }

            return returnList;
        }



        /// <summary>
        /// GetUDFDataPageWise
        /// </summary>
        /// <param name="PageName"></param>
        /// <returns></returns>
        private object GetUDFDataPageWise(string PageName, bool IsOrderHeaderCanEdit = true)
        {
            //UDFApiController objUDFApiController = new UDFApiController();
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(PageName, RoomID, CompanyID);

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
                             UDFIsRequired = (IsOrderHeaderCanEdit ? c.UDFIsRequired : false),
                             UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
                             Created = c.Created,
                             Updated = c.Updated,
                             //UpdatedByName = c.UpdatedByName,
                             //CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                         };
            //if (result != null && result.Count() > 0 && !IsOrderHeaderCanEdit)
            //{
            //    if (result.Where(x => x.UDFIsRequired == true).ToList().Count > 0)
            //        result.ToList().ForEach(t => { t.UDFIsRequired = false; });
            //}
            return result;
        }
        /// <summary>
        /// GetMailBody
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetMailBody(ToolAssetOrderMasterDTO obj)
        {
            string mailBody = "";


            string udfRequier = string.Empty;
            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
            List<UDFDTO> DataFromDB = objUDFDAL.GetNonDeletedUDFsByUDFTableNamePlain("ToolAssetOrder", RoomID, CompanyID);


            string OrdNumber = ResToolAssetOrder.ToolAssetOrderNumber;
            string ReqDateCap = ResToolAssetOrder.RequiredDate;
            string OrdStatus = ResToolAssetOrder.OrderStatus;
            string OrdReqQty = ResToolAssetOrder.RequestedQuantity;

            //if (obj.OrderType == (int)ToolAssetOrderType.RuturnOrder)
            //{
            //    OrdNumber = "Return " + ResToolAssetOrder.ToolAssetOrderNumber;
            //    ReqDateCap = "Return Date";
            //    OrdStatus = "Return " + OrdStatus;
            //    OrdReqQty = "Return Quanity";
            //}


            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + OrdNumber + @": </label>
                                    <label style=""font-weight: bold;"">
                                        " + obj.ToolAssetOrderNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResToolAssetOrder.Comment + @": </label>
                                    <label>
                                        " + obj.Comment + @"</label>
                                </td>
                            </tr>
                            ";

            mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        " + ResToolAssetOrder.OrderCost + @": </label>
                                    <label>
                                        " + obj.OrderCost.GetValueOrDefault(0).ToString("N" + SessionHelper.CurrencyDecimalDigits) + @"</label>
                                </td>
                            </tr>";
            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                for (int i = 0; i < DataFromDB.Count; i++)
                {
                    mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        &nbsp; </label>
                                    <label>
                                        &nbsp;</label>
                                </td>
                            </tr>";
                }
            }

            mailBody = mailBody + @"</table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + obj.RequiredDate.ToString(SessionHelper.DateTimeFormat) + @"</label>
                                </td>
                            </tr>
                           
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(ToolAssetOrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
                                </td>
                            </tr>";


            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                for (int i = 0; i < DataFromDB.Count; i++)
                {
                    if (DataFromDB[i].UDFColumnName == "UDF" + (i + 1))
                    {

                        string val = string.Empty;// ResourceUtils.GetResource(UDFTableResourceFileName, item.UDFColumnName, true, (OtherFromeTurns), ForEnterPriseSetup);
                        val = ResourceUtils.GetResource("ResToolAssetOrder", DataFromDB[i].UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            DataFromDB[i].UDFDisplayColumnName = val;
                        else
                            DataFromDB[i].UDFDisplayColumnName = DataFromDB[i].UDFColumnName;

                        System.Reflection.PropertyInfo info = obj.GetType().GetProperty("UDF" + (i + 1));
                        string udfValue = (string)info.GetValue(obj, null);
                        if (string.IsNullOrEmpty(udfValue))
                            udfValue = "";

                        mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        " + DataFromDB[i].UDFDisplayColumnName + @": </label>
                                    <label>
                                        " + udfValue + @"</label>
                                </td>
                            </tr>";
                    }
                }
            }



            mailBody = mailBody + @"</table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResToolMaster.ToolName + @"
                                    </th>
                                    
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResToolMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OrdReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
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
            if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
            {
                ToolAssetOrderDetailsDAL objOrdDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);

                obj.OrderListItem = objOrdDetailDAL.GetOrderedRecord(obj.GUID, RoomID, CompanyID, obj.IsArchived.GetValueOrDefault(false), obj.IsDeleted);
            }

            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {

                foreach (var item in obj.OrderListItem)
                {

                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();


                    if (obj.OrderStatus == (int)ToolAssetOrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);


                    trs += @"<tr>
                        <td>
                            " + item.ToolName + @"
                        </td>
                        
                        <td>
                            " + item.ToolDescription + @"
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
        /// <summary>
        /// GetMailBody
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string GetMailBodySupplier(ToolAssetOrderMasterDTO obj, string DateTimeFormat = "MM/dd/yyyy")
        {
            string mailBody = "";

            string OrdNumber = ResToolAssetOrder.ToolAssetOrderNumber;
            string ReqDateCap = ResToolAssetOrder.RequiredDate;
            string OrdStatus = ResToolAssetOrder.OrderStatus;
            string OrdReqQty = ResToolAssetOrder.RequestedQuantity;


            string strReqDate = obj.RequiredDate.ToString(DateTimeFormat);//obj.RequiredDate.ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);


            //if (obj.OrderType == (int)ToolAssetOrderType.RuturnOrder)
            //{
            //    OrdNumber = "Return " + ResToolAssetOrder.ToolAssetOrderNumber;
            //    ReqDateCap = "Return Date";
            //    OrdStatus = "Return " + OrdStatus;
            //    OrdReqQty = "Return Quanity";
            //}




            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                        <tr>
                            <td style=""width: 48%"">
                                <table style=""margin-left: 0px; width: 99%;"">
                                <tr>
                                    <td>
                                        <label style=""font-weight: bold;"">
                                            " + OrdNumber + @": </label>
                                        <label style=""font-weight: bold;"">
                                            " + obj.ToolAssetOrderNumber + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResToolAssetOrder.Comment + @": </label>
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
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + strReqDate + @"</label>
                                </td>
                            </tr>
                            
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(ToolAssetOrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
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
                                        " + ResToolMaster.ToolName + @"
                                    </th>
                                    
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResToolMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OrdReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
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
            if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
            {
                ToolAssetOrderDetailsDAL objOrdDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.OrderListItem = objOrdDetailDAL.GetOrderedRecord(obj.GUID, obj.RoomID.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));

            }


            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {
                List<ToolAssetOrderDetailsDTO> objOrderItemList = new List<ToolAssetOrderDetailsDTO>();
                foreach (var item in obj.OrderListItem)
                {
                    objOrderItemList.Add(item);
                    if (item.ToolType == 2)
                    {
                        IEnumerable<ToolDetailDTO> objKitDeailList = new ToolDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(item.ToolGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false, true);

                        foreach (var KitCompitem in objKitDeailList)
                        {
                            ToolAssetOrderDetailsDTO objOrderDetailDTO = new ToolAssetOrderDetailsDTO()
                            {
                                ToolName = KitCompitem.ToolName + "&nbsp;" + " <I>(comp. of Tool Kit: " + item.ToolName + ")</I>",
                                ApprovedQuantity = KitCompitem.QuantityPerKit.GetValueOrDefault(0) * item.ApprovedQuantity.GetValueOrDefault(),
                                RequiredDate = item.RequiredDate,
                            };
                            objOrderItemList.Add(objOrderDetailDTO);
                        }
                    }
                }
                obj.OrderListItem.Clear();
                obj.OrderListItem = objOrderItemList;
            }
            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {

                foreach (var item in obj.OrderListItem)
                {

                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    //binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.Bin)),null,null).FirstOrDefault().BinNumber;

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();


                    if (obj.OrderStatus == (int)ToolAssetOrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        //ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(DateTimeFormat);


                    trs += @"<tr>
                        <td>
                            " + item.ToolName + @"
                        </td>
                        
                        <td>
                            " + item.ToolDescription + @"
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



        private string GetMailBodySupplierForUnSubmitted(ToolAssetOrderMasterDTO obj, string DateTimeFormat = "MM/dd/yyyy")
        {
            string mailBody = "";

            string OrdNumber = ResToolAssetOrder.ToolAssetOrderNumber;
            string ReqDateCap = ResToolAssetOrder.RequiredDate;
            string OrdStatus = ResToolAssetOrder.OrderStatus;
            string OrdReqQty = ResToolAssetOrder.RequestedQuantity;


            //obj.RequiredDate.ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture)
            string strRequiredDate = obj.RequiredDate.ToString(DateTimeFormat);


            //if (obj.OrderType == (int)ToolAssetOrderType.RuturnOrder)
            //{
            //    OrdNumber = "Return " + ResToolAssetOrder.ToolAssetOrderNumber;
            //    ReqDateCap = "Return Date";
            //    OrdStatus = "Return " + OrdStatus;
            //    OrdReqQty = "Return Quanity";
            //}




            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                        <tr>
                            <td style=""width: 48%"">
                                <table style=""margin-left: 0px; width: 99%;"">
                                <tr>
                                    <td>
                                        <label style=""font-weight: bold;"">
                                            " + OrdNumber + @": </label>
                                        <label style=""font-weight: bold;"">
                                            " + obj.ToolAssetOrderNumber + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResToolAssetOrder.Comment + @": </label>
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
                                       " + ReqDateCap + @": </label>
                                    <label>
                                        " + strRequiredDate + @"</label>
                                </td>
                            </tr>
                            
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(ToolAssetOrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
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
                                        " + ResToolMaster.ToolName + @"
                                    </th>
                                    
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResToolMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OrdReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
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
            if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
            {
                ToolAssetOrderDetailsDAL objOrdDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.OrderListItem = objOrdDetailDAL.GetOrderedRecord(obj.GUID, obj.RoomID.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                if (obj.OrderStatus == (int)ToolAssetOrderStatus.UnSubmitted)
                {
                    if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
                    {
                        obj.OrderListItem = PreparedOrderLiteItemWithProperData(obj); //GetLineItemsFromSession(obj.ID);
                    }
                }
            }


            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {
                List<ToolAssetOrderDetailsDTO> objOrderItemList = new List<ToolAssetOrderDetailsDTO>();
                foreach (var item in obj.OrderListItem)
                {
                    objOrderItemList.Add(item);
                    if (item.ToolType == 2)
                    {
                        IEnumerable<ToolDetailDTO> objKitDeailList = new ToolDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(item.ToolGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false, true);

                        foreach (var KitCompitem in objKitDeailList)
                        {
                            ToolAssetOrderDetailsDTO objOrderDetailDTO = new ToolAssetOrderDetailsDTO()
                            {
                                ToolName = KitCompitem.ToolName + "&nbsp;" + " <I>(comp. of Tool Kit: " + item.ToolName + ")</I>",

                                ApprovedQuantity = KitCompitem.QuantityPerKit.GetValueOrDefault(0) * item.ApprovedQuantity.GetValueOrDefault(),
                                RequiredDate = item.RequiredDate,
                            };
                            objOrderItemList.Add(objOrderDetailDTO);
                        }
                    }
                }
                obj.OrderListItem.Clear();
                obj.OrderListItem = objOrderItemList;
            }
            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {

                foreach (var item in obj.OrderListItem)
                {

                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    string desc = "&nbsp;";
                    // if (item.Bin != null && item.Bin > 0 && string.IsNullOrEmpty(item.BinName))
                    //binname = new BinMasterController().GetRecord(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID, false, false).BinNumber;
                    //      binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID, false, false).BinNumber;
                    //binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.Bin)),null,null).FirstOrDefault().BinNumber;

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();


                    if (obj.OrderStatus == (int)ToolAssetOrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        // ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);
                        ReqDate = strRequiredDate;

                    if (!string.IsNullOrEmpty(item.ToolDescription))
                    {
                        desc = item.ToolDescription;
                    }

                    trs += @"<tr>
                        <td>
                            " + item.ToolName + @"
                        </td>
                        
                        <td>
                            " + desc + @"
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
        /// <summary>
        /// Send Mail to Approval authority for Approve Order
        /// </summary>
        /// <param name="strToEmailAddress"></param>
        /// <param name="objOrder"></param>
        public void SendMailToApprovalAuthority(ToolAssetOrderMasterDTO objOrder)
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
                if ((objOrder.OrderType ?? 0) == (int)ToolAssetOrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ToolAssetOrderApproval, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                    lstNotifications = lstNotifications.ToList();
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
                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.ToolAssetOrderNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                                        string DateTimeFormat = "MM/dd/yyyy";
                                        DateTime TZDateTimeNow = DateTime.UtcNow;
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {
                                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }
                                    }
                                }
                                MessageBody.Replace("@@ORDERNO@@", objOrder.ToolAssetOrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                MessageBody.Replace("@@TABLE@@", GetMailBody(objOrder));
                                MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                //MessageBody.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));

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
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);


                                if (!string.IsNullOrWhiteSpace(strToAddress))
                                {
                                    List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    if (EmailAddrs != null && EmailAddrs.Count > 0)
                                    {
                                        foreach (var emailitem in EmailAddrs)
                                        {
                                            string strdata = objOrder.ID + "^" + objOrder.RoomID + "^" + objOrder.CompanyID + "^" + (objOrder.LastUpdatedBy ?? objOrder.CreatedBy) + "^" + SessionHelper.EnterPriceID.ToString() + "^" + objOrder.LastUpdatedBy.GetValueOrDefault(0) + "^" + emailitem; ;
                                            string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
                                            string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");


                                            List<eMailAttachmentDTO> objeMailAttchListNew = new List<eMailAttachmentDTO>();
                                            foreach (var item in lstAttachments)
                                            {
                                                objeMailAttchListNew.Add(item);
                                            }

                                            MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/ToolAssetOrderStatus?eKey=" + approvalURLData + "'>'" + ResCommon.Approve + "'</a> &nbsp;&nbsp;<a href='" + replacePart + "/EmailLink/ToolAssetOrderStatus?eKey=" + rejectURLData + "'>'" + ResCommon.Reject + "'</a>");
                                            objAlertMail.CreateAlertMail(objeMailAttchListNew, StrSubject, MessageBody.ToString(), emailitem, t, objEnterpriseDTO);

                                        }
                                    }
                                }

                            }

                        });
                    }

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

        /// <summary>
        /// SendMailForApprovedOrReject
        /// </summary>
        /// <param name="objOrder"></param>
        /// <param name="AprvRejString"></param>
        public void SendMailForApprovedOrReject(ToolAssetOrderMasterDTO objOrder, string AprvRejString)
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            eTurnsWeb.Helper.AlertMail objAlertMail = new Helper.AlertMail();
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            NotificationDAL objNotificationDAL = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            try
            {
                EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
                if ((objOrder.OrderType ?? 0) == (int)ToolAssetOrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ToolAssetOrderApproveReject, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

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
                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.ToolAssetOrderNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                                        string DateTimeFormat = "MM/dd/yyyy";
                                        DateTime TZDateTimeNow = DateTime.UtcNow;
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {
                                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }
                                    }
                                }
                                MessageBody.Replace("@@ORDERNO@@", objOrder.ToolAssetOrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                //MessageBody.Replace("@@TABLE@@", new OrderController().GetMailBody(objOrder));
                                MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                MessageBody.Replace("@@APRVREJ@@", AprvRejString);

                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                            }
                        });
                    }
                }

            }
            finally
            {
                MessageBody = null;
                objNotificationDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }


        /// <summary>
        /// Send Mail Order UnSubmitted
        /// </summary>
        /// <param name="ToSupplier"></param>
        /// <param name="objOrder"></param>
        public void SendMailOrderUnSubmitted(ToolAssetOrderMasterDTO objOrder)
        {
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            //eTurnsUtility objUtils = null;
            eMailMasterDAL objEmailDAL = null;
            //IEnumerable<EmailUserConfigurationDTO> extUsers = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            //string[] splitEmails = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            //eMailAttachmentDTO objeMailAttch = null;
            //ArrayList arrAttchament = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            try
            {
                #region [Order]
                if ((objOrder.OrderType ?? 0) == (int)ToolAssetOrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ToolAssetOrderUnSubmitted, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                    //lstNotifications = lstNotifications.Where(t => t.SupplierIds == Convert.ToString(ToSuppliers.ID)).ToList();
                    lstNotifications = lstNotifications.ToList();
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
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                            }
                            string strToAddress = t.EmailAddress;
                            string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
                            eTurnsRegionInfo objeTurnsRegionInfoNew = null;
                            if (objEnterpriseDTO != null)
                            {
                                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
                            }
                            string DateTimeFormat = "MM/dd/yyyy";
                            DateTime TZDateTimeNow = DateTime.UtcNow;
                            if (objeTurnsRegionInfoNew != null)
                            {
                                DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                                TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                            }
                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                                //string strCCAddress = "";
                                StringBuilder MessageBody = new StringBuilder();
                                objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
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
                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.ToolAssetOrderNumber);
                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }
                                    if (objEnterpriseDTO != null)
                                    {
                                        DateTimeFormat = "MM/dd/yyyy";
                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }
                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {

                                            StrSubject = StrSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                                        }

                                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                                        StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                                    }
                                }
                                MessageBody.Replace("@@ORDERNO@@", objOrder.ToolAssetOrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);

                                string stratatTABLEatatTag = GetMailBodySupplierForUnSubmitted(objOrder, DateTimeFormat);

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


                                string strReplText = ResOrder.SeeAttachedFilesForOrderDetail;

                                MessageBody.Replace("@@TABLE@@", strReplText);


                                objeMailAttchList = new List<eMailAttachmentDTO>();

                                MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                                if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                                {
                                    MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                                }
                                MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                                {
                                    MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                }
                                MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));
                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                                //objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, CompanyID, RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier");
                            }
                        });
                    }

                }
                #endregion
            }
            catch
            {
            }
            finally
            {
                //objUtils = null;
                objEmailDAL = null;
                //extUsers = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                //splitEmails = null;
                objeMailAttchList = null;
                //objeMailAttch = null;
                //arrAttchament = null;
            }
        }
        /// <summary>
        /// GetExternalUserEmailAddress
        /// </summary>
        /// <param name="eMailTemplateName"></param>
        /// <returns></returns>
        public IEnumerable<EmailUserConfigurationDTO> GetExternalUserEmailAddress(string eMailTemplateName)
        {
            EmailUserConfigurationDAL objExternalUser = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<EmailUserConfigurationDTO> lstExternalUser = objExternalUser.GetAllExternalUserRecords(eMailTemplateName, true, RoomID, CompanyID);
            return lstExternalUser;
        }
        /// <summary>
        /// GetPDFStreamToAttachInMail
        /// </summary>
        /// <param name="strHTML"></param>
        /// <returns></returns>
        private byte[] GetPDFStreamToAttachInMail(string strHTML, ToolAssetOrderMasterDTO objOrderDTO, NotificationDTO objNotificationDTO)
        {
            ReportMasterDAL objReportMasterDAL = null;
            ReportBuilderDTO objRPTDTO = null;
            ReportBuilderController objRPTCTRL = null;
            MasterController objMSTCTRL = null;
            KeyValDTO objKeyVal = null;
            List<KeyValDTO> objKeyValList = null;
            JavaScriptSerializer objJSSerial = null;
            byte[] fileBytes = null;
            JsonResult objJSON = null;
            MemoryStream fs = null;
            StringReader sr = null;
            iTextSharp.text.html.simpleparser.HTMLWorker hw = null;
            iTextSharp.text.Document doc = null;

            try
            {

                objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objReportMasterDAL.GetReportList().FirstOrDefault(x => x.IsBaseReport && x.ModuleName == "Order" && x.ReportType == 2 && x.ReportName == "Order");
                if (objRPTDTO != null)
                {
                    objKeyValList = new List<KeyValDTO>();

                    objKeyVal = new KeyValDTO() { key = "DataGuids", value = objOrderDTO.GUID.ToString() };
                    objKeyValList.Add(objKeyVal);
                    objKeyVal = new KeyValDTO() { key = "CompanyIDs", value = CompanyID.ToString() };
                    objKeyValList.Add(objKeyVal);
                    objKeyVal = new KeyValDTO() { key = "RoomIDs", value = RoomID.ToString() };
                    objKeyValList.Add(objKeyVal);

                    objMSTCTRL = new MasterController();
                    objMSTCTRL.SetPDFReportParaDictionary(objKeyValList, objRPTDTO.ID.ToString(), null);

                    objRPTCTRL = new ReportBuilderController();
                    objJSON = objRPTCTRL.GenerateBytesFromReport(objRPTDTO.ID, "PDF");
                    objJSON.MaxJsonLength = int.MaxValue;

                    objJSSerial = new JavaScriptSerializer();
                    var json = objJSSerial.Deserialize<Dictionary<string, object>>(objJSSerial.Serialize(objJSON.Data));

                    if (Convert.ToString(json["Message"]) == "ok")
                    {
                        fileBytes = System.IO.File.ReadAllBytes(Convert.ToString(json["FilePath"]));
                        return fileBytes;
                    }
                }

                strHTML = strHTML.Replace("/Content/", Server.MapPath("/Content/"));
                fs = new MemoryStream();
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36f, 36f, 36f, 36f);
                iTextSharp.text.pdf.PdfWriter pdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                doc.Open();
                hw = new iTextSharp.text.html.simpleparser.HTMLWorker(doc);
                sr = new StringReader(strHTML);
                hw.Parse(sr);
                pdfWriter.CloseStream = false;
                doc.Close();
                pdfWriter.Dispose();
                fs.Position = 0;
                return fs.ToArray();


            }
            finally
            {
                if (hw != null)
                {
                    hw.Dispose();
                    hw = null;
                }

                if (sr != null)
                {
                    sr.Dispose();
                    sr = null;
                }

                if (doc != null)
                {
                    doc.Dispose();
                    doc = null;
                }
                if (fs != null)
                {
                    fs.Dispose();
                    fs = null;
                }

                objReportMasterDAL = null;
                objRPTDTO = null;
                objRPTCTRL = null;
                objMSTCTRL = null;
                objKeyVal = null;
                objKeyValList = null;
                objJSSerial = null;
                fileBytes = null;
                objJSON = null;
            }

        }

        /// <summary>
        /// Get CVS File Data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private byte[] GetCVSStringTOAttachINMail(ToolAssetOrderMasterDTO obj)
        {

            ReportMasterDAL objReportMasterDAL = null;
            ReportBuilderDTO objRPTDTO = null;
            ReportBuilderController objRPTCTRL = null;
            MasterController objMSTCTRL = null;
            KeyValDTO objKeyVal = null;
            List<KeyValDTO> objKeyValList = null;
            JavaScriptSerializer objJSSerial = null;
            MemoryStream stream = null;
            JsonResult objJSON = null;

            string stringCsvData = "Order#, Require Date,Comment,ToolName,Quanity,Tool Required Date";
            byte[] data = null;
            ToolAssetOrderDetailsDAL objOrdDetailDAL = null;
            try
            {

                objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objReportMasterDAL.GetReportList().FirstOrDefault(x => x.IsBaseReport && x.ModuleName == "ToolAssetOrder" && x.ReportType == 2 && x.ReportName == "ToolAssetOrder");
                if (objRPTDTO != null)
                {
                    objKeyValList = new List<KeyValDTO>();

                    objKeyVal = new KeyValDTO() { key = "DataGuids", value = obj.GUID.ToString() };
                    objKeyValList.Add(objKeyVal);
                    objKeyVal = new KeyValDTO() { key = "CompanyIDs", value = CompanyID.ToString() };
                    objKeyValList.Add(objKeyVal);
                    objKeyVal = new KeyValDTO() { key = "RoomIDs", value = RoomID.ToString() };
                    objKeyValList.Add(objKeyVal);

                    objMSTCTRL = new MasterController();
                    objMSTCTRL.SetPDFReportParaDictionary(objKeyValList, objRPTDTO.ID.ToString(), null);

                    objRPTCTRL = new ReportBuilderController();
                    objJSON = objRPTCTRL.GenerateBytesFromReport(objRPTDTO.ID, "Excel");

                    objJSSerial = new JavaScriptSerializer();
                    var json = objJSSerial.Deserialize<Dictionary<string, object>>(objJSSerial.Serialize(objJSON.Data));

                    if (Convert.ToString(json["Message"]) == "ok")
                    {
                        string sheetName = Convert.ToString(json["SheetName"]);
                        sheetName = sheetName.Substring(0, 31);
                        string ReportExportPathCSV = System.Web.HttpContext.Current.Server.MapPath(@"/Downloads/") + objRPTDTO.ReportName + DateTimeUtility.DateTimeNow.ToString("MMddyyyy_HHmmss") + ".csv";
                        convertExcelToCSV(Convert.ToString(json["FilePath"]), sheetName, ReportExportPathCSV);
                        data = System.IO.File.ReadAllBytes(ReportExportPathCSV);
                        return data;
                    }


                }
                else
                {

                    if (obj != null)
                    {
                        if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
                        {
                            objOrdDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                            obj.OrderListItem = objOrdDetailDAL.GetOrderedRecord(obj.GUID, obj.RoomID.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                        }
                        if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
                        {
                            foreach (var item in obj.OrderListItem)
                            {
                                stringCsvData += "\r\n";
                                stringCsvData += string.Format("\"{0}\" ,\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\""
                                                            , obj.ToolAssetOrderNumber.Replace(",", " ")

                                                            , obj.RequiredDate.ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture)
                                                            , obj.Comment == null ? "" : obj.Comment.Replace(",", " ")
                                                            , item.ToolName.Replace(",", " ")
                                                            , item.ApprovedQuantity.GetValueOrDefault(0).ToString("N2")
                                                            , item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture));
                            }


                        }
                    }

                    data = new ASCIIEncoding().GetBytes(stringCsvData);
                    return data;
                }

                return null;
            }
            finally
            {
                data = null;
                objOrdDetailDAL = null;

                if (stream != null)
                    stream.Dispose();

                stream = null;


                objReportMasterDAL = null;
                objRPTDTO = null;
                objRPTCTRL = null;
                objMSTCTRL = null;
                objKeyVal = null;
                objKeyValList = null;
                objJSSerial = null;

            }
        }

        /// <summary>
        /// Convert Excel To CSV
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="worksheetName"></param>
        /// <param name="targetFile"></param>
        static void convertExcelToCSV(string sourceFile, string worksheetName, string targetFile)
        {
            OleDbConnection conn = null;
            StreamWriter wrtr = null;
            OleDbCommand cmd = null;
            OleDbDataAdapter da = null;
            try
            {
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sourceFile + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                conn = new OleDbConnection(strConn);
                conn.Open();
                //cmd = new OleDbCommand("SELECT * FROM " + worksheetName, conn);
                cmd = new OleDbCommand("SELECT * FROM [" + worksheetName + "$]", conn);
                cmd.CommandType = CommandType.Text;
                wrtr = new StreamWriter(targetFile);
                da = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int x = 0; x < dt.Rows.Count; x++)
                    {
                        string rowString = "";
                        for (int y = 0; y < dt.Columns.Count; y++)
                        {
                            rowString += "\"" + Convert.ToString(dt.Rows[x][y]) + "\",";
                        }
                        wrtr.WriteLine(rowString);
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];

            //    objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
            //    objEmailDAL.eMailToSend(strBCCAddress, "", " Error IN CSV function at line " + strErrorLine, ex.ToString(), SessionHelper.EnterPriceID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, null, "Web => Order => OrderToSupplier =>  convertExcelToCSV => Error During Create CSV ");
            //    throw ex;
            //}
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
                da.Dispose();
                wrtr.Close();
                wrtr.Dispose();
            }
        }
        public void SendemailSet()
        {

            ToolAssetOrderMasterDTO objOrder = new ToolAssetOrderMasterDTO();
            ToolAssetOrderMasterDAL objOrderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SessionHelper.RoomID = 212;
            SessionHelper.CompanyID = 201540119;
            objOrder = objOrderDAL.GetRecord(Guid.Parse(""), Convert.ToInt64(SessionHelper.RoomID), Convert.ToInt64(SessionHelper.CompanyID));

            //SendMailToSupplier(objSupplierMasterDTO, objOrder);
        }
        #endregion




        #region Order NarrowSearch

        public JsonResult GetNarrowSearchData(bool IsDeleted, bool IsArchived, string OrderStatus)
        {
            //CommonController objCommonCtrl = new CommonController();
            string MainFilter = "";
            if (Convert.ToString(Session["MainFilter"]).Trim().ToLower() == "true")
            {
                MainFilter = "true";
            }

            CommonDAL objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);

            NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("ToolAssetOrderMaster", CompanyID, RoomID, IsArchived, IsDeleted, OrderStatus, SessionHelper.UserSupplierIds, MainFilter, ToolAssetOrderTypeOrdType: GetToolAssetOrderType);
            return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearchData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetOrderNarrwSearchHTML
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrderNarrwSearchHTML()
        {
            //CommonController objCommonCtrl = new CommonController();
            //NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("OrderMaster", CompanyID, RoomID, IsArchived, IsDeleted, OrderStatus);

            //return PartialView("_OrderNarrowSearch", objNarrowSearchData);
            return PartialView("_OrderNarrowSearch");

        }

        #endregion


    }



}
