using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ReceiveToolAssetController : eTurnsControllerBase
    {
        #region New Methods

        private Int64 RoomID = SessionHelper.RoomID;
        private Int64 CompanyID = SessionHelper.CompanyID;
        private Int64 UserID = SessionHelper.UserID;
        private Int64 EnterpriseId = SessionHelper.EnterPriceID;

        /// <summary>
        /// Order List
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiveList()
        {
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            return View();
        }

        /// <summary>
        /// New: OrderMasterListAjax = ReceiveListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ReceiveListAjax(JQueryDataTableParamModel param)
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
            string OrderStatusIn = Request["OrderStatusIn"].ToString();

            bool IsRequestFromDashboard = false;
            string _SelectedSupplier = "";
            string strSupplierOptions = "";


            bool IsArchived = false;
            bool IsDeleted = false;


            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ToolName desc";
            }
            else
                sortColumnName = "ToolName desc";

            string searchQuery = string.Empty;

            //if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower() == "ordernumber")
            //    sortColumnName = "OrderNumber_ForSorting";

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower().Contains("ordernumber"))
                sortColumnName = sortColumnName.Replace("ToolAssetOrderNumber", "OrderNumber_ForSorting");

            int TotalRecordCount = 0;

            ReceiveToolAssetOrderDetailsDAL controller = new ReceiveToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ReceivableToolDTO> DataFromDBAll = new List<ReceivableToolDTO>();
            List<ReceivableToolDTO> DataFromDB = new List<ReceivableToolDTO>();

            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            if (IsRequestFromDashboard == true)
            {

                DataFromDBAll = controller.GetALLReceiveListByPaging(0, int.MaxValue, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, OrderStatusIn, RoomDateFormat, CurrentTimeZone).ToList();
                if (DataFromDBAll != null && DataFromDBAll.Count > 0)
                {
                    DataFromDB = DataFromDBAll;


                    TotalRecordCount = DataFromDB.Count();
                    if (DataFromDB != null && DataFromDB.Count > 0)
                        DataFromDB = DataFromDB.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                    DataFromDB = new List<ReceivableToolDTO>();

                //-------------------Prepare DDL Data-------------------
                //                
                if (DataFromDBAll.Count > 0)
                {

                }
            }
            else
            {

                DataFromDB = controller.GetALLReceiveListByPaging(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, OrderStatusIn, RoomDateFormat, CurrentTimeZone).ToList();
            }

            if (DataFromDB != null)
            {
                DataFromDB.ToList().ForEach(t => t.strReqDtlDate = FnCommon.ConvertDateByTimeZone(t.OrderDetailRequiredDate, false, true));
                //if (OrderStatusIn == "4,5,6,7")
                //{
                //    DataFromDB = DataFromDB.Where(x => !x.IsCloseItem.GetValueOrDefault(false)).ToList();
                //}
            }
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB,
                SupplierOptions = strSupplierOptions,
                SelectedSupplier = _SelectedSupplier,
                SearchTerm = param.sSearch,
                StartIndex = param.iDisplayStart
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// New: ReceiveItem  =RetrivedItems(string ItemGUID, string ItemType)
        /// </summary>
        /// <returns></returns>

        public ActionResult ReceiveTool(Guid ToolAssetOrderDetailGUID)
        {
            ReceivableToolDTO objDTO = new ReceiveToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetALLReceiveList(SessionHelper.RoomID, SessionHelper.CompanyID, null, null, null, ToolAssetOrderDetailGUID).FirstOrDefault();
            LocationMasterDAL objCommon = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<LocationMasterDTO> objBinDTO = null;
            if (objDTO != null)
            {
                objBinDTO = objCommon.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            }


            ViewBag.BinLocations = objBinDTO;

            if (!string.IsNullOrEmpty(objDTO.ReceiveBinName) && objBinDTO.Count() > 0)
            {
                if (objDTO.ReceiveBinID.GetValueOrDefault(0) > 0)
                {
                    if (objBinDTO.ToList().FindIndex(x => x.ID == objDTO.ReceiveBinID.GetValueOrDefault(0)) >= 0)
                        objDTO.ReceiveBinName = objBinDTO.Where(x => x.ID == objDTO.ReceiveBinID.GetValueOrDefault(0)).FirstOrDefault().Location;
                }
                else if (objDTO.ToolDefaultLocation > 0)
                {
                    if (objBinDTO.ToList().FindIndex(x => x.ID == objDTO.ToolDefaultLocation) >= 0)
                        objDTO.ReceiveBinName = objBinDTO.Where(x => x.ID == objDTO.ToolDefaultLocation).FirstOrDefault().Location;
                }
            }
            objDTO.IsOnlyFromUI = true;
            return PartialView("_ReceiveTool", objDTO);
        }
        /// <summary>
        /// ReceivedItemDetail
        /// </summary>
        /// <returns></returns>



        public ActionResult ReceivedToolAssetDetail(ReceivableToolDTO objDTO)
        {
            ReceivedToolAssetOrderTransferDetailDAL objROTDDAL = null;
            ToolAssetOrderMasterDAL objOrderMasterDAL = null;
            ToolAssetOrderMasterDTO objOrderDTO = null;
            ToolAssetOrderDetailsDAL objOrderDetailDAL = null;
            ToolAssetOrderDetailsDTO objOrderDetailDTO = null;
            try
            {
                objROTDDAL = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                objDTO.ReceivedToolDetail = objROTDDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.ToolGUID, objDTO.ToolAssetOrderDetailGUID, "ID Desc");

                objOrderMasterDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                objOrderDTO = objOrderMasterDAL.GetRecord(objDTO.ToolAssetOrderGUID, RoomID, CompanyID);
                objDTO.OrderStatus = objOrderDTO.OrderStatus;

                objOrderDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                objOrderDetailDTO = objOrderDetailDAL.GetRecord(objDTO.ToolAssetOrderDetailGUID, RoomID, CompanyID);
                objDTO.IsCloseTool = objOrderDetailDTO.IsCloseTool.GetValueOrDefault(false);

                return PartialView("_ReceivedToolAssetDetail", objDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objROTDDAL = null;
                objOrderMasterDAL = null;
                objOrderDTO = null;
            }
        }


        /// <summary>
        /// ReceivedItemDetail
        /// </summary>
        /// <returns></returns>



        public ActionResult ReceivedToolsByToolAssetOrderDetail(Guid dataGUID)
        {
            ReceivedToolAssetOrderTransferDetailDAL objROTDDAL = null;
            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> lstReceivedItems;
            try
            {
                objROTDDAL = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                lstReceivedItems = objROTDDAL.GetAllRecordByOrderDetailGuid(SessionHelper.RoomID, SessionHelper.CompanyID, dataGUID, "ID Desc");
                ViewBag.dataGuid = dataGUID;
                ViewBag.dataUrl = "ReceivedItemsByOrderDetail";
                return PartialView("_ReceivedToolDetail", lstReceivedItems);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objROTDDAL = null;
            }
        }

        /// <summary>
        /// LoadAllItems
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadAllTools()
        {
            return PartialView("ReceiveToolWithoutOrder");
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult NewReceiveToolsListAjax(QuickListJQueryDataTableParamModel param)
        {
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
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

            bool IsArchived = false;
            bool IsDeleted = false;


            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ToolName desc";
            }
            else
                sortColumnName = "ToolName desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedToolsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "", "", RoomDateFormat, CurrentTimeZone, "", "1,2");

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// ReceiveNewItemWithoutOrderInner
        /// </summary>
        /// <param name="ItemGuid"></param>
        /// <returns></returns>
        public ActionResult ReceiveNewToolWithoutOrderInnerGrid(string ToolGuid, string ToolType)
        {
            object objItem = null;
            string returnView = "";
            ToolMasterDAL itemDAL = null;

            try
            {

                {
                    itemDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    //objItem = itemDAL.GetRecord(ItemGuid, RoomID, CompanyID);
                    objItem = itemDAL.GetToolByGUIDPlain(Guid.Parse(ToolGuid));

                    returnView = "ReceiveNewToolWithoutOrdNotSerial";
                    if (((ToolMasterDTO)objItem).SerialNumberTracking)
                        returnView = "ReceiveNewToolWithoutOrdSerial";
                    return PartialView(returnView, (ToolMasterDTO)objItem);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                itemDAL = null;
            }

        }

        /// <summary>
        /// SaveNewReceiveWithoutOrder
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveNewReceiveWithoutOrder(ReceivableToolDTO objDTO)
        {
            ReceiveToolAssetOrderDetailsDAL objRecOrdDetDAL = new ReceiveToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ToolAssetOrderDetailsDTO> objOrdDetailDTO = null;
            ToolAssetQuantityDetailDAL objItemLocationDetailsDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            string rcvDateValid = string.Empty;
            DateTime dtRcvTemp;

            DateTime.TryParseExact(objDTO.OrderRequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtRcvTemp);
            ToolAssetOrderDetailsDAL objOrderDetailsDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            if (dtRcvTemp <= DateTime.MinValue)
            {
                rcvDateValid = ResReceiveOrderDetails.EnterValidReceivedDate;
            }

            if (!string.IsNullOrEmpty(rcvDateValid))
            {
                return Json(new { Message = rcvDateValid, Status = "Error" }, JsonRequestBehavior.AllowGet);
            }

            objDTO.OrderRequiredDate = objDTO.OrderRequiredDateStr != null ? DateTime.ParseExact(objDTO.OrderRequiredDateStr, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult) : Convert.ToDateTime(objDTO.OrderRequiredDateStr);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedToolAssetOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            RoomDTO roomDTO = null;
            
            foreach (var i in DataFromDB)
            {
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF1))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF2))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF3))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF4))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF5))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }

                    if (!string.IsNullOrEmpty(udfRequier))
                        break;
                
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                return Json(new { Message = udfRequier, Status = "UDFError" }, JsonRequestBehavior.AllowGet);
            }
            if (objDTO != null && objDTO.ToolAssetOrderNumber != null)
            {
                if (objDTO.ToolAssetOrderNumber.Length > 22)
                {
                    return Json(new { Message = ResOrder.OrderNumberLengthUpto22Char, Status = "Error" }, JsonRequestBehavior.AllowGet);
                }

                //----------------------Check For Order Number Duplication----------------------
                //
                string strOK = string.Empty;
                //   roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string columnList = "ID,RoomName,IsAllowOrderDuplicate";
                roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                if (roomDTO.IsAllowOrderDuplicate != true)
                {
                    ToolAssetOrderMasterDAL objOrderMasterDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                    if (objDTO.ToolAssetOrderGUID != null && objDTO.ToolAssetOrderGUID != Guid.Empty)
                    {
                        if (objOrderMasterDAL.IsOrderNumberDuplicate(objDTO.ToolAssetOrderNumber, objDTO.ToolAssetOrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID))
                        {
                            strOK = "duplicate";
                        }
                    }
                    else
                    {
                        if (objOrderMasterDAL.IsOrderNumberDuplicate(objDTO.ToolAssetOrderNumber, 0, SessionHelper.RoomID, SessionHelper.CompanyID))
                        {
                            strOK = "duplicate";
                        }
                    }
                }

                if (strOK == "duplicate")
                {
                    return Json(new { Message = string.Format(ResMessage.DuplicateMessage, ResOrder.OrderNumber, objDTO.ToolAssetOrderNumber), Status = "Error" }, JsonRequestBehavior.AllowGet);
                }
                //
                //-------------------------------------------------------------------------------
            }

            ToolMasterDTO objItemDTO = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(objDTO.ToolGUID);


            InventoryController objInvCtrl = new InventoryController();

            ToolLocationDetailsDTO objbinDTO = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetToolLocation(objDTO.ToolGUID, objDTO.ReceiveBinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ReceiveToolAsset>>DirectReceiveTool");


            if (objDTO.ToolAssetOrderGUID == Guid.Empty && objDTO.ToolAssetOrderDetailGUID == Guid.Empty)
            {
                objDTO.ReceiveBinID = objbinDTO.ID;
                objOrdDetailDTO = objRecOrdDetDAL.CreateDirectReceiveToolAssetOrder(objDTO.ToolAssetOrderNumber, objDTO.ToolGUID, objDTO.RequestedQuantity, objDTO.ReceiveBinID.GetValueOrDefault(0), objDTO.OrderRequiredDate, objDTO.ReceivedQuantity, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objDTO.OrderNumber_ForSorting, "Web", DateTimeUtility.DateTimeNow, "Web", DateTimeUtility.DateTimeNow, string.Empty);

            }
            else
            {
                objOrdDetailDTO = objOrderDetailsDAL.GetOrderedRecord(objDTO.ToolAssetOrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            ReceivedToolAssetOrderTransferDetailDTO obj = new ReceivedToolAssetOrderTransferDetailDTO()
            {
                ToolAssetOrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID,
                ToolBinID = objbinDTO.ID,
                SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.ReceivedToolDetail.FirstOrDefault().SerialNumber)) ? objDTO.ReceivedToolDetail.FirstOrDefault().SerialNumber.Trim() : string.Empty,
                Quantity = objDTO.ReceivedQuantity,
                Action = string.Empty,
                Location = objDTO.ReceiveBinName,
                CompanyID = SessionHelper.CompanyID,
                Cost = objDTO.ReceivedToolDetail.FirstOrDefault().Cost,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                HistoryID = 0,
                ID = 0,
                IsArchived = false,
                GUID = Guid.Empty,
                IsDeleted = false,
                ToolGUID = objDTO.ToolGUID,
                ToolName = objItemDTO.ToolName,
                KitDetailGUID = null,
                LastUpdatedBy = SessionHelper.UserID,
                mode = string.Empty,
                Received = objDTO.ReceivedToolDetail.FirstOrDefault().Received,
                ReceivedDate = dtRcvTemp,
                Room = SessionHelper.RoomID,
                RoomName = SessionHelper.RoomName,
                SerialNumberTracking = objItemDTO.SerialNumberTracking,
                UDF1 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF1,
                UDF2 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF2,
                UDF3 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF3,
                UDF4 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF4,
                UDF5 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF5,
                IsEDISent = false,
                Updated = DateTimeUtility.DateTimeNow,
                UpdatedByName = SessionHelper.UserName,
                AddedFrom = "Web",
                EditedFrom = "Web",
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                Description = objDTO.Description
            };


            ReceivedToolAssetOrderTransferDetailDAL rotd = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);

            obj.ToolAssetOrderStatus = Convert.ToInt32((OrderStatus.TransmittedInCompletePastDue));
            rotd.InsertReceive(obj);


            ToolAssetOrderDetailsDAL objOrderDetail = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);

            objOrderDetail.UpdateOrderStatusByReceiveNew(objOrdDetailDTO.FirstOrDefault().ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);
            objOrdDetailDTO = objOrderDetail.GetOrderedRecord(objOrdDetailDTO.FirstOrDefault().ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);


            return Json(new { Massage = "ok", Success = "ok", OrderGUID = objOrdDetailDTO.FirstOrDefault().ToolAssetOrderGUID, OrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID, ReceivedQty = objOrdDetailDTO.FirstOrDefault().ReceivedQuantity }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// GetOrderNumber
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrderNumber(Int64 ToolID)
        {

            AutoOrderNumberGenerate objAutoNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextToolAssetOrderNumber(SessionHelper.RoomID, SessionHelper.CompanyID, 0,SessionHelper.EnterPriceID);
            string orderNumberForSort = "";
            string ToolAssetOrderNumber = objAutoNumber.OrderNumber;
            if (ToolAssetOrderNumber != null && (!string.IsNullOrEmpty(ToolAssetOrderNumber)))
            {
                ToolAssetOrderNumber = ToolAssetOrderNumber.Length > 22 ? ToolAssetOrderNumber.Substring(0, 22) : ToolAssetOrderNumber;
            }

            if (string.IsNullOrWhiteSpace(ToolAssetOrderNumber))
                ToolAssetOrderNumber = "";

            orderNumberForSort = objAutoNumber.OrderNumberForSorting;

            if (string.IsNullOrEmpty(orderNumberForSort))
            {
                orderNumberForSort = ToolAssetOrderNumber;
            }

            return Json(new { Message = "OK", Status = "OK", OrderNumber = ToolAssetOrderNumber, OrderNumberForSort = orderNumberForSort }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrderNumberBySupplier(Int64 SupplierID)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;

            try
            {
                objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                objAutoNumber = objAutoSeqDAL.GetNextOrderNumber(RoomID, CompanyID, SupplierID, EnterpriseId);

                string orderNumber = objAutoNumber.OrderNumber;
                string OrderNumber_ForSorting = objAutoNumber.OrderNumberForSorting;

                return Json(new { Message = "OK", Status = true, OrderNumber = orderNumber, OrderNumberForSort = OrderNumber_ForSorting }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.ToString(), Status = false, OrderNumber = "", OrderNumberForSort = "" }, JsonRequestBehavior.AllowGet);

            }
            finally
            {
                objAutoNumber = null;
                objAutoSeqDAL = null;
            }
        }

        /// <summary>
        /// AddNewReceiveBin
        /// </summary>
        /// <param name="NewBinNumber"></param>
        /// <param name="IsStaging"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddNewReceiveBin(string NewBinNumber, bool IsStaging)
        {

            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (objCommonDAL.DuplicateCheck(NewBinNumber, "add", 0, "BinMaster", "BinNumber") == "ok")
            {
                BinMasterDTO objBinDTO = new BinMasterDTO()
                {
                    BinNumber = NewBinNumber,
                    CompanyID = SessionHelper.CompanyID,
                    Room = SessionHelper.RoomID,
                    CreatedBy = SessionHelper.UserID,
                    LastUpdatedBy = SessionHelper.UserID,
                    Created = DateTimeUtility.DateTimeNow,
                    LastUpdated = DateTimeUtility.DateTimeNow,
                    IsStagingLocation = IsStaging,
                    AddedFrom = "Web",
                    EditedFrom = "Web",
                    ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    ReceivedOn = DateTimeUtility.DateTimeNow,
                };
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                objBinDTO = objBinDAL.InsertBin(objBinDTO);
                return Json(new { Message = "OK", Status = "OK", BinDTO = objBinDTO }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = ResReceiveOrderDetails.BinNoAlreadyExist, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// SaveNewReceiveWithoutOrder
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveReceiveWithOrder(ReceivableToolDTO objDTO)
        {
            ReceiveToolAssetOrderDetailsDAL objRecOrdDetDAL = new ReceiveToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            InventoryController objInvCtrl = new InventoryController();
            ToolAssetOrderDetailsDAL objOrdDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            JsonResult objResult;
            IEnumerable<ToolAssetOrderDetailsDTO> objOrdDetailDTO;
            objDTO.OrderRequiredDate = objDTO.OrderRequiredDateStr != null ? DateTime.ParseExact(objDTO.OrderRequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : Convert.ToDateTime(objDTO.OrderRequiredDateStr);
            objOrdDetailDTO = objOrdDetailDAL.GetOrderedRecord(objDTO.ToolAssetOrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.GUID == objDTO.ToolAssetOrderDetailGUID);
            List<ToolAssetQuantityDetailDTO> lstLocationWiseDTO = null;
            if (objOrdDetailDTO == null)
                objOrdDetailDTO = objOrdDetailDAL.GetOrderedRecord(objDTO.ToolAssetOrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ToolGUID == objDTO.ToolGUID);

            ToolMasterDTO objItemDTO = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(objDTO.ToolGUID);
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedToolAssetOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;

            foreach (var i in DataFromDB)
            {
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF1))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF2))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF3))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF4))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objDTO.ReceivedToolDetail.FirstOrDefault().UDF5))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }

                    if (!string.IsNullOrEmpty(udfRequier))
                        break;
                
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                return Json(new { Message = udfRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

            ToolLocationDetailsDTO objTLDDTO = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetToolLocation(objDTO.ToolGUID, objDTO.ReceiveBinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ReceiveToolAsset>>DirectReceiveTool");

            ToolAssetQuantityDetailDTO obj = new ToolAssetQuantityDetailDTO()
            {
                ToolAssetOrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID,
                ToolBinID = objTLDDTO.ID,
                Quantity = objDTO.ReceivedQuantity,
                Action = string.Empty,
                Location = objDTO.ReceiveBinName,
                CompanyID = SessionHelper.CompanyID,
                Cost = objDTO.ReceivedToolDetail.FirstOrDefault().Cost,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                DateCodeTracking = objItemDTO.DateCodeTracking,
                ID = 0,
                IsArchived = false,
                GUID = Guid.Empty,
                IsDeleted = false,
                ToolGUID = objDTO.ToolGUID,
                ToolName = objItemDTO.ToolName,
                UpdatedBy = SessionHelper.UserID,
                LotNumberTracking = objItemDTO.LotNumberTracking,
                ReceivedDate = objDTO.ReceivedToolDetail.FirstOrDefault().ReceivedDate,
                RoomID = SessionHelper.RoomID,
                RoomName = SessionHelper.RoomName,
                SerialNumberTracking = objItemDTO.SerialNumberTracking,
                Updated = DateTimeUtility.DateTimeNow,
                UpdatedByName = SessionHelper.UserName,
                UDF1 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF1,
                UDF2 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF2,
                UDF3 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF3,
                UDF4 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF4,
                UDF5 = objDTO.ReceivedToolDetail.FirstOrDefault().UDF5,
                Description = objDTO.Description

            };


            //if (objItemDTO.SerialNumberTracking)
            //{
            //    objResult = objInvCtrl.ItemLocationDetailSaveNewReceiveOrderForSerial(obj);
            //}
            //else
            //{
            lstLocationWiseDTO = new List<ToolAssetQuantityDetailDTO>();
            lstLocationWiseDTO.Add(obj);

            ToolAssetQuantityDetailDAL itmLocDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);

            itmLocDAL.InsertToolAssetQuantityDetailsFromRecieve(lstLocationWiseDTO);

                objOrdDetailDAL.UpdateOrderStatusByReceive(objDTO.ToolAssetOrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);
                objResult = Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK" }, JsonRequestBehavior.AllowGet);

            //}




            objOrdDetailDTO = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderedRecord(objOrdDetailDTO.FirstOrDefault().ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ToolGUID == objDTO.ToolGUID);
            ToolAssetOrderMasterDAL objOrdDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
            ToolAssetOrderMasterDTO objOrdDTO = objOrdDAL.GetRecord(objOrdDetailDTO.FirstOrDefault().ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
            string OrdStatusText = objOrdDTO.OrderStatusText;
            return Json(new { Massage = "ok", Success = "ok", OrderGUID = objOrdDetailDTO.FirstOrDefault().ToolAssetOrderGUID, OrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID, ReceivedQty = objOrdDetailDTO.FirstOrDefault().ReceivedQuantity, OrderStatusText = OrdStatusText }, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetToolLocations(Guid ToolAssetOrderGuid, Guid ToolGuid, string NameStartWith, bool? IsLoadMoreLocations = null)
        {
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            ToolAssetOrderMasterDTO objOrdDTO = null;
            if (ToolAssetOrderGuid != Guid.Empty)
            {
                objOrdDTO = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(ToolAssetOrderGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            IEnumerable<LocationMasterDTO> objBinDTOList;
            if (IsLoadMoreLocations.HasValue)
            {
                objBinDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByToolLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGuid.ToString()).Where(x => !string.IsNullOrWhiteSpace(x.Location)).OrderBy(x => x.Location);
                //objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid,0,null,false).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber);// !x.IsStagingLocation && 
            }
            else
            {
                objBinDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, NameStartWith.ToLower()).Where(x => !string.IsNullOrWhiteSpace(x.Location)).OrderBy(x => x.Location);
            }

            if (objBinDTOList != null && objBinDTOList.Count() > 0)
            {
                if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    objBinDTOList = objBinDTOList.Where(x => x.Location.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                }

                foreach (var item in objBinDTOList)
                {
                    DTOForAutoComplete obj = new DTOForAutoComplete()
                    {
                        Key = item.Location,
                        Value = item.Location,
                        ID = item.ID,
                        GUID = item.GUID,
                    };
                    returnKeyValList.Add(obj);
                }
            }

            if (IsLoadMoreLocations.HasValue)
            {
                if (IsLoadMoreLocations.Value)
                {
                    IEnumerable<LocationMasterDTO> oAllBinDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, NameStartWith.ToLower().Trim()).Where(x => !string.IsNullOrWhiteSpace(x.Location)).OrderBy(x => x.Location);
                    if (oAllBinDTOList != null && oAllBinDTOList.Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                        {
                            oAllBinDTOList = oAllBinDTOList.Where(x => x.Location.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                        }

                        foreach (var item in oAllBinDTOList)
                        {
                            if (!returnKeyValList.Any(x => x.Key == item.Location))
                            {
                                DTOForAutoComplete obj = new DTOForAutoComplete()
                                {
                                    Key = item.Location,
                                    Value = item.Location,
                                    ID = item.ID,
                                    GUID = item.GUID,
                                };
                                returnKeyValList.Add(obj);
                            }
                        }
                    }
                }
                else
                {
                    DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                    objAutoDTO.Key = ResBin.MoreLocations;
                    objAutoDTO.Value = ResBin.MoreLocations;
                    objAutoDTO.ID = 0;
                    objAutoDTO.GUID = Guid.Empty;
                    returnKeyValList.Add(objAutoDTO);
                }
            }


            return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// OrderMasterListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>



        #region Edit Receive
        /// <summary>
        /// Open Edit Receive Dailog
        /// </summary>
        /// <param name="ReceivedGUID"></param>
        /// <returns></returns>




        public ActionResult OpenDialogForEditReceiptTool(Guid ToolGuid, Guid ToolAssetOrderDetailGuid)
        {
            ReceivedToolAssetOrderTransferDetailDAL objRecdOrdTrnDtlDAL = null;
            List<ReceivedToolAssetOrderTransferDetailDTO> objRecdOrdTrnDtlDTO = null;
            try
            {
                objRecdOrdTrnDtlDAL = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                objRecdOrdTrnDtlDTO = objRecdOrdTrnDtlDAL.GetAllRecords(RoomID, CompanyID, ToolGuid, ToolAssetOrderDetailGuid, "ID").ToList();
                ViewBag.ToolGuid = ToolGuid;
                ViewBag.ToolAssetOrderDetailGuid = ToolAssetOrderDetailGuid;
                ToolAssetOrderDetailsDTO objOrderDTO = new eTurns.DAL.ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(ToolAssetOrderDetailGuid, RoomID, CompanyID);
                ViewBag.ToolAssetOrderGuid = objOrderDTO.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty);

                return PartialView("_EditReceiptTool", objRecdOrdTrnDtlDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRecdOrdTrnDtlDAL = null;
                objRecdOrdTrnDtlDTO = null;
            }

        }


        [HttpPost]
        public JsonResult EditReceiptRecordsTools(List<ReceivedToolAssetOrderTransferDetailDTO> reciepts)
        {
            CommonDAL objCommonDAL = null;
            ReceivedToolAssetOrderTransferDetailDAL objROTDDAL = null;
            ToolAssetOrderDetailsDAL objOrderDetailDAL = null;
            ToolAssetOrderMasterDAL objOrderDAL = null;
            ToolAssetOrderDetailsDTO objOrderDetailDTO = null;
            ToolAssetOrderMasterDTO objOrderDTO = null;
            ToolLocationDetailsDAL objItemLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            bool isSuccess = true;
            string returnMsg = ResCommon.MsgUpdatedSuccessfully;
            try
            {
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                objROTDDAL = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                objOrderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                objOrderDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                string strDuplicateSerials = string.Empty;

                foreach (var item in reciepts)
                {

                    if (string.IsNullOrWhiteSpace(item.Location))
                    {
                        strDuplicateSerials = strDuplicateSerials + "<br/>" + ResReceiveOrderDetails.BinCantEmptySelectIt;
                    }
                    objOrderDetailDTO = objOrderDetailDAL.GetRecord(item.ToolAssetOrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                    objOrderDTO = objOrderDAL.GetRecord(objOrderDetailDTO.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);



                    if (string.IsNullOrEmpty(strDuplicateSerials))
                    {
                        ToolLocationDetailsDTO objBinMasterDTO = objItemLocationDetailsDAL.GetToolLocation(item.ToolGUID ?? Guid.Empty, item.Location, RoomID, CompanyID, UserID, "ReceiveController>>EditReceiptRecordsTools");
                        item.ToolBinID = objBinMasterDTO.ID;
                        item.CompanyID = CompanyID;
                        item.Room = RoomID;
                        item.LastUpdatedBy = UserID;
                        item.Updated = DateTimeUtility.DateTimeNow;


                        item.EditedFrom = "Web";
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;

                    }
                    if (!string.IsNullOrWhiteSpace(item.strReceivedDate))
                    {

                        //item.ReceivedDate = DateTime.ParseExact(item.strReceivedDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
                        item.ReceivedDate = DateTime.ParseExact(item.strReceivedDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                    }

                }

                if (!string.IsNullOrEmpty(strDuplicateSerials))
                {
                    return Json(new { Status = false, Message = strDuplicateSerials }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    objROTDDAL.Edit(reciepts);
                }

                return Json(new { Status = isSuccess, Message = returnMsg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Status = false, Message = ResCommon.ErrorColon + " " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objCommonDAL = null;
                objROTDDAL = null;
            }

        }
        #endregion


        /// <summary>
        /// Pre Receive Info For Serial Item only
        /// </summary>
        /// <param name="OrderDetailGuid"></param>
        /// <param name="ItemGuid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FillPreReceiveInfoForSerialItem(ToFillPreReceiveToolDTO MakePreReceive)
        {
            PreReceivOrderDetailToolAssetDAL PreReciveDAL = null;
            List<PreReceivOrderDetailToolDTO> preReceveInfo = null;
            List<ToFillReceiveDetailToolDTO> lstToFillRcvDtl = null;
            ToFillReceiveDetailToolDTO detailDTO = null;
            try
            {

                lstToFillRcvDtl = new List<ToFillReceiveDetailToolDTO>();
                PreReciveDAL = new PreReceivOrderDetailToolAssetDAL(SessionHelper.EnterPriseDBName);
                preReceveInfo = PreReciveDAL.GetAllRecordsByOrderDetailTool(MakePreReceive.OrderDetailGUID, MakePreReceive.ToolGUID).ToList();
                ViewBag.ItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, MakePreReceive.ToolGUID);
                if (preReceveInfo != null && preReceveInfo.Count > 0)
                {
                    foreach (var item in preReceveInfo)
                    {
                        detailDTO = GetToFillReceiveDetailToolList(item, MakePreReceive);
                        lstToFillRcvDtl.Add(detailDTO);
                    }
                }

                double qty = MakePreReceive.MakePreReceiveDetail[0].Quantity;
                if (qty > lstToFillRcvDtl.Count)
                {
                    qty = qty - lstToFillRcvDtl.Count;

                    for (int i = 0; i < int.Parse(qty.ToString()); i++)
                    {
                        detailDTO = GetToFillReceiveDetailToolList(new PreReceivOrderDetailToolDTO(), MakePreReceive);
                        lstToFillRcvDtl.Add(detailDTO);
                    }

                }
                else
                {
                    //detailDTO = GetToFillReceiveDetailList(new PreReceivOrderDetailDTO(), MakePreReceive);
                    //lstToFillRcvDtl.Add(detailDTO);
                    qty = 0;
                    if (MakePreReceive.ApproveQty.GetValueOrDefault(0) > 0)
                    {
                        qty = MakePreReceive.ApproveQty.GetValueOrDefault(0) - MakePreReceive.ReceiveQty.GetValueOrDefault(0);
                    }
                    else
                    {
                        qty = MakePreReceive.RequestedQty.GetValueOrDefault(0) - MakePreReceive.ReceiveQty.GetValueOrDefault(0);
                    }
                    if (qty > 0)
                    {
                        for (int i = 0; i < qty; i++)
                        {
                            detailDTO = GetToFillReceiveDetailToolList(new PreReceivOrderDetailToolDTO(), MakePreReceive);
                            lstToFillRcvDtl.Add(detailDTO);
                        }
                    }
                    else
                    {
                        detailDTO = GetToFillReceiveDetailToolList(new PreReceivOrderDetailToolDTO(), MakePreReceive);
                        lstToFillRcvDtl.Add(detailDTO);
                    }
                }


                MakePreReceive.IsModelShow = true;
                MakePreReceive.MakePreReceiveDetail = lstToFillRcvDtl;
                return PartialView("PreRecieveInfoForSerialItem", MakePreReceive);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                PreReciveDAL = null;
                preReceveInfo = null;
                lstToFillRcvDtl = null;
                detailDTO = null;
            }
        }





        private ToFillReceiveDetailToolDTO GetToFillReceiveDetailToolList(PreReceivOrderDetailToolDTO item, ToFillPreReceiveToolDTO MakePreReceive)
        {
            ToFillReceiveDetailToolDTO toFillRcvDtl = new ToFillReceiveDetailToolDTO();



            toFillRcvDtl.Quantity = item.Quantity.GetValueOrDefault(0);


            return toFillRcvDtl;

        }



        private ReceivedToolAssetOrderTransferDetailDTO GetROTDTOOLDTO(ToolAssetOrderMasterDTO orderDTO, ToolAssetOrderDetailsDTO orderDetailDTO, ToFillReceiveDetailToolDTO innerItem, ToFillPreReceiveToolDTO item)
        {
            ToolLocationDetailsDAL objToolLocationDetailsDAL = null;
            objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            double? QTY = null;
            QTY = innerItem.Quantity;
            DateTime recDate;

            if (!string.IsNullOrEmpty(item.ReceivedDate))
            {
                //recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
                recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            }
            else
                recDate = DateTimeUtility.DateTimeNow;

            ToolLocationDetailsDTO objToolAssetQuantityDetail = objToolLocationDetailsDAL.GetToolLocation(item.ToolGUID, item.BinNumber, RoomID, CompanyID, UserID, "ReceiveController>>GetROTDTOOLDTO");
            ReceivedToolAssetOrderTransferDetailDTO rotd = new ReceivedToolAssetOrderTransferDetailDTO()
            {
                ToolBinID = objToolAssetQuantityDetail.ID,
                CompanyID = CompanyID,
                Room = RoomID,
                Cost = item.Cost,
                ToolGUID = item.ToolGUID,
                SerialNumber = innerItem.SerialNumber,
                ToolAssetOrderDetailGUID = item.OrderDetailGUID,
                ReceivedDate = recDate,
                Action = string.Empty,
                AddedFrom = "Web",
                Location = item.BinNumber ?? string.Empty,
                Quantity = QTY ?? 0,
                GUID = Guid.NewGuid(),
                ToolLocationGUID = objToolAssetQuantityDetail.LocationGUID,
                EditedFrom = "Web",
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                UDF1 = item.UDF1,
                UDF2 = item.UDF2,
                UDF3 = item.UDF3,
                UDF4 = item.UDF4,
                UDF5 = item.UDF5,
                ControlNumber = string.Empty,
                PackSlipNumber = item.PackSlipNumber,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                IsEDISent = false,
                Description = innerItem.Description
            };

            return rotd;
        }




        private string ValidateReceiveUDF(ToFillPreReceiveToolDTO objDTO)
        {
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedToolAssetOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            foreach (var i in DataFromDB)
            {
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objDTO.UDF1))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objDTO.UDF2))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objDTO.UDF3))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objDTO.UDF4))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objDTO.UDF5))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }

                    if (!string.IsNullOrEmpty(udfRequier))
                        break;
                
            }

            return udfRequier;
        }


        public ActionResult FillPreReceiveInformationTool(ToFillPreReceiveToolDTO MakePreReceive)
        {

            PreReceivOrderDetailToolAssetDAL PreReciveDAL = null;
            List<PreReceivOrderDetailToolDTO> preReceveInfo = null;
            List<ToFillReceiveDetailToolDTO> lstToFillRcvDtl = null;
            ToFillReceiveDetailToolDTO detailDTO = null;
            try
            {

                lstToFillRcvDtl = new List<ToFillReceiveDetailToolDTO>();
                PreReciveDAL = new PreReceivOrderDetailToolAssetDAL(SessionHelper.EnterPriseDBName);
                preReceveInfo = PreReciveDAL.GetAllRecordsByOrderDetailTool(MakePreReceive.OrderDetailGUID, MakePreReceive.ToolGUID).ToList();
                if (preReceveInfo != null && preReceveInfo.Count > 0)
                {

                    foreach (var item in preReceveInfo)
                    {
                        detailDTO = GetToFillReceiveDetailToolList(item, MakePreReceive);
                        if (detailDTO.Quantity > 0)
                            lstToFillRcvDtl.Add(detailDTO);
                    }

                    if (lstToFillRcvDtl.Count > 0)
                        MakePreReceive.IsModelShow = true;

                    //MakePreReceive.MakePreReceiveDetail = lstToFillRcvDtl;
                }

                detailDTO = GetToFillReceiveDetailToolList(new PreReceivOrderDetailToolDTO(), MakePreReceive);
                if (!MakePreReceive.IsModelShow && lstToFillRcvDtl.Count <= 0)
                    preReceveInfo = new List<PreReceivOrderDetailToolDTO>();
                if (preReceveInfo.Count <= 0 && MakePreReceive.MakePreReceiveDetail != null && MakePreReceive.MakePreReceiveDetail.Count > 0)
                {
                    detailDTO.Quantity = MakePreReceive.MakePreReceiveDetail[0].Quantity;
                }
                if (!MakePreReceive.IsModelShow && (MakePreReceive.SerialNumberTracking))
                {
                    MakePreReceive.IsModelShow = true;
                }


                lstToFillRcvDtl.Add(detailDTO);
                MakePreReceive.MakePreReceiveDetail = lstToFillRcvDtl;
                return PartialView("PreRecieveInfoTool", MakePreReceive);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                PreReciveDAL = null;
                preReceveInfo = null;
                lstToFillRcvDtl = null;
                detailDTO = null;
            }
        }
        [HttpPost]
        public ActionResult SaveReceiveInformationTool(List<ToFillPreReceiveToolDTO> SavePreReceiveData)
        {
            ToolAssetOrderMasterDAL orderDAL = null;
            ToolAssetOrderDetailsDAL orderDetailDAL = null;
            ToolAssetOrderMasterDTO orderDTO = null;
            ToolAssetOrderDetailsDTO orderDetailDTO = null;
            //List<ItemLocationDetailsDTO> lstROTD = null;
            List<ReceivedToolAssetOrderTransferDetailDTO> lstROTD = null;

            List<ReceiveErrors> listReceiveErrors = null;
            ReceiveErrors receiveError = null;
            List<Guid> listOrderGuids = null;
            int ordStatus = 0;
            listReceiveErrors = new List<ReceiveErrors>();
            try
            {

                if (SavePreReceiveData == null || SavePreReceiveData.Count <= 0)
                {
                    receiveError = new ReceiveErrors()
                    {
                        ErrorColor = string.Empty,
                        ErrorMassage = ResReceiveOrderDetails.MsgSelectDataToReceive,
                        ErrorTitle = "",
                        OrderDetailGuid = Guid.Empty,
                    };
                    listReceiveErrors.Add(receiveError);

                    return Json(new { Status = false, Message = "Error", Errors = listReceiveErrors }, JsonRequestBehavior.AllowGet);
                }


                // lstROTD = new List<ItemLocationDetailsDTO>();
                lstROTD = new List<ReceivedToolAssetOrderTransferDetailDTO>();

                orderDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                orderDetailDAL = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                listOrderGuids = new List<Guid>();


                foreach (var item in SavePreReceiveData)
                {
                    if (item.Location == null || item.Location == "null")
                    {
                        item.Location = string.Empty;

                    }
                    orderDetailDTO = orderDetailDAL.GetRecord(item.OrderDetailGUID, RoomID, CompanyID);
                    orderDTO = orderDAL.GetRecord(orderDetailDTO.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                    if (orderDetailDTO.IsCloseTool.GetValueOrDefault(false))
                    {
                        receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                        if (receiveError != null)
                        {
                            receiveError.ErrorMassage += ResReceiveOrderDetails.MsgOrderLineItemClosed;
                        }
                        else
                        {
                            receiveError = new ReceiveErrors()
                            {
                                ErrorColor = "Red",
                                ErrorMassage = ResReceiveOrderDetails.MsgOrderLineItemClosed,
                                ErrorTitle = orderDTO.ToolAssetOrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ToolName,
                                OrderDetailGuid = orderDetailDTO.GUID,
                            };
                            listReceiveErrors.Add(receiveError);
                        }
                        continue;
                    }
                    if (orderDTO.OrderStatus == (int)ToolAssetOrderStatus.Closed)
                    {
                        receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                        if (receiveError != null)
                        {
                            receiveError.ErrorMassage += (" " + ResReceiveOrderDetails.MsgOrderClosed);
                        }
                        else
                        {
                            receiveError = new ReceiveErrors()
                            {
                                ErrorColor = "Red",
                                ErrorMassage = ResReceiveOrderDetails.MsgOrderClosed,
                                ErrorTitle = orderDTO.ToolAssetOrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ToolName,
                                OrderDetailGuid = orderDetailDTO.GUID,
                            };
                            listReceiveErrors.Add(receiveError);
                        }
                        continue;
                    }
                    //if (string.IsNullOrEmpty(item.Location))
                    //{
                    //    receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                    //    if (receiveError != null)
                    //    {
                    //        receiveError.ErrorMassage += " Location is mandatory.";
                    //    }
                    //    else
                    //    {
                    //        receiveError = new ReceiveErrors()
                    //        {
                    //            ErrorColor = "Red",
                    //            ErrorMassage = "Location is mandatory.",
                    //            ErrorTitle = orderDTO.ToolAssetOrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.Serial,
                    //            OrderDetailGuid = orderDetailDTO.GUID,
                    //        };
                    //        listReceiveErrors.Add(receiveError);
                    //    }
                    //    continue;
                    //}

                    //if (  string.IsNullOrEmpty(item.PackSlipNumber))
                    //{
                    //    receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                    //    if (receiveError != null)
                    //    {
                    //        receiveError.ErrorMassage += " Packslip is mandatory.";
                    //    }
                    //    else
                    //    {
                    //        receiveError = new ReceiveErrors()
                    //        {
                    //            ErrorColor = "Red",
                    //            ErrorMassage = "Packslip is mandatory.",
                    //            ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                    //            OrderDetailGuid = orderDetailDTO.GUID,
                    //        };
                    //        listReceiveErrors.Add(receiveError);
                    //    }
                    //    continue;
                    //}

                    string udfError = ValidateReceiveUDF(item);
                    if (!string.IsNullOrEmpty(udfError))
                    {
                        receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                        if (receiveError != null)
                        {
                            receiveError.ErrorMassage += " " + udfError;
                        }
                        else
                        {
                            receiveError = new ReceiveErrors()
                            {
                                ErrorColor = "Red",
                                ErrorMassage = udfError,
                                ErrorTitle = orderDTO.ToolAssetOrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ToolName,
                                OrderDetailGuid = orderDetailDTO.GUID,
                            };
                            listReceiveErrors.Add(receiveError);
                        }
                        continue;
                    }


                    if (!(item.MakePreReceiveDetail != null && item.MakePreReceiveDetail.Count > 0))
                    {
                        receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                        if (receiveError != null)
                        {
                            receiveError.ErrorMassage += (" " + ResReceiveOrderDetails.MsgEnterDataToReceive);
                        }
                        else
                        {
                            receiveError = new ReceiveErrors()
                            {
                                ErrorColor = "Red",
                                ErrorMassage = ResReceiveOrderDetails.MsgEnterDataToReceive,
                                ErrorTitle = orderDTO.ToolAssetOrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ToolName,
                                OrderDetailGuid = orderDetailDTO.GUID,
                            };

                            listReceiveErrors.Add(receiveError);
                        }
                        continue;
                    }
                    else
                    {
                        foreach (var innerItem in item.MakePreReceiveDetail)
                        {
                            if (innerItem.Quantity <= 0)
                            {
                                receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                                if (receiveError != null)
                                {
                                    receiveError.ErrorMassage += (" " + ResReceiveOrderDetails.MsgEnterQtyToReceive);
                                }
                                else
                                {
                                    receiveError = new ReceiveErrors()
                                    {
                                        ErrorColor = "Red",
                                        ErrorMassage = ResReceiveOrderDetails.MsgEnterQtyToReceive,
                                        ErrorTitle = orderDTO.ToolAssetOrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ToolName,
                                        OrderDetailGuid = orderDetailDTO.GUID,
                                    };

                                    listReceiveErrors.Add(receiveError);
                                }
                                continue;
                            }



                            if (listReceiveErrors.FindIndex(x => x.OrderDetailGuid == item.OrderDetailGUID) < 0)
                            {
                                ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                                //Guid? UsedToolGUId = objToolDAL.GetUsedToolGuidinQuantity(SessionHelper.RoomID, SessionHelper.CompanyID, orderDetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty), string.Empty);
                                Guid? UsedToolGUId = orderDetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty);
                                Guid? ACtualOrderDetailsGUID = null;
                                Guid? ACtualReceiveToolGUID = null;
                                if (UsedToolGUId != null && UsedToolGUId.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    ACtualOrderDetailsGUID = orderDetailDTO.ToolGUID;
                                    ACtualReceiveToolGUID = item.ToolGUID;
                                    orderDetailDTO.ToolGUID = UsedToolGUId;
                                    item.ToolGUID = UsedToolGUId.GetValueOrDefault(Guid.Empty);
                                }
                                ToolLocationDetailsDTO ildd = GetReceiveOrderTransferDetailDTO(orderDTO, orderDetailDTO, innerItem, item);
                                if (UsedToolGUId != null && UsedToolGUId.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    orderDetailDTO.ToolGUID = ACtualOrderDetailsGUID;
                                }

                                ReceivedToolAssetOrderTransferDetailDTO rotd = GetROTDTOOLDTO(orderDTO, orderDetailDTO, innerItem, item);
                                if (UsedToolGUId != null && UsedToolGUId.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    orderDetailDTO.ToolGUID = ACtualOrderDetailsGUID;
                                    item.ToolGUID = ACtualReceiveToolGUID ?? Guid.Empty;
                                }
                                rotd.PackSlipNumber = item.PackSlipNumber;
                                lstROTD.Add(rotd);



                                if (listOrderGuids.FindIndex(x => x == orderDTO.GUID) < 0)
                                    listOrderGuids.Add(orderDTO.GUID);
                            }
                        }
                    }

                    orderDetailDTO.Comment = item.Comment;

                    orderDetailDAL.UpdateLineComment(orderDetailDTO, SessionHelper.UserID);
                }


                if (lstROTD != null && lstROTD.Count > 0)
                {
                    //ItemLocationDetailsDAL itmLocDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    //itmLocDAL.InsertItemLocationDetailsFromRecieve(lstROTD);
                    ReceivedToolAssetOrderTransferDetailDAL itmLocDAL = new ReceivedToolAssetOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                    foreach (var item in lstROTD)
                    {
                        itmLocDAL.InsertReceive(item);
                    }
                }



                if (listOrderGuids != null && listOrderGuids.Count > 0)
                {
                    foreach (var ordGuid in listOrderGuids)
                    {
                        ordStatus = (int)orderDetailDAL.UpdateOrderStatusByReceiveNew(ordGuid, RoomID, CompanyID, SessionHelper.UserID, true);
                    }
                    //if (listOrderGuids.Count == 1)
                    //{
                    //    orderDTO = orderDAL.GetRecord(listOrderGuids[0], RoomID, CompanyID);
                    //    ordStatus = orderDTO.OrderStatus;
                    //}
                }

                if (listReceiveErrors != null && listReceiveErrors.Count > 0)
                {
                    return Json(new { Status = false, Message = "", Errors = listReceiveErrors, OrderStatus = ordStatus }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Status = true, Message = "", Errors = listReceiveErrors, OrderStatus = ordStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                receiveError = new ReceiveErrors()
                {
                    ErrorColor = "Red",
                    ErrorMassage = ex.Message,
                    ErrorTitle = "Exception",
                    OrderDetailGuid = Guid.Empty,
                };
                listReceiveErrors.Add(receiveError);
                return Json(new { Status = false, Message = "Exception", Errors = listReceiveErrors, OrderStatus = ordStatus }, JsonRequestBehavior.AllowGet);
            }
        }
        private ToolLocationDetailsDTO GetReceiveOrderTransferDetailDTO(ToolAssetOrderMasterDTO orderDTO, ToolAssetOrderDetailsDTO orderDetailDTO, ToFillReceiveDetailToolDTO innerItem, ToFillPreReceiveToolDTO item)
        {
            LocationMasterDAL binDAL = null;
            binDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);

            double? Qty = null;



            Qty = innerItem.Quantity;


            DateTime recDate;

            if (!string.IsNullOrEmpty(item.ReceivedDate))
            {
                //recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
                recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            }
            else
                recDate = DateTimeUtility.DateTimeNow;


            ToolLocationDetailsDTO rotd = new ToolLocationDetailsDTO()
            {
                LocationID = binDAL.GetToolLocation(item.ToolGUID, item.BinNumber ?? string.Empty, RoomID, CompanyID, UserID, "ReceiveController>>GetReceiveOrderTransferDetailsDTO").ID,
                CompanyID = CompanyID,
                RoomID = RoomID,
                Cost = item.Cost,
                ToolGuid = item.ToolGUID,

                //ReceivedDate = recDate,
                Action = string.Empty,
                AddedFrom = "Web",
                ToolLocationName = item.BinNumber,



                EditedFrom = "Web",
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                Createdon = DateTimeUtility.DateTimeNow,
                LastUpdatedOn = DateTimeUtility.DateTimeNow,
                //InitialQuantity = innerItem.Quantity,
                //InitialQuantityWeb = innerItem.Quantity,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
            };


            return rotd;

        }
        #endregion
        public string DuplicateCheckSrNumberTool(string SrNumber, int ID, Guid? ToolGUID)
        {
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            return objCDal.CheckDuplicateToolSerialNumbers(SrNumber, ID, SessionHelper.RoomID, SessionHelper.CompanyID, ToolGUID ?? Guid.Empty);

        }
        #region Old Methods


        /// <summary>
        /// OrderMasterListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //public ActionResult ReceiveMasterListAjax(JQueryDataTableParamModel param)
        //{

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
        //    bool IsArchived = false;// bool.Parse(Request["IsArchived"].ToString());
        //    bool IsDeleted = false;// bool.Parse(Request["IsDeleted"].ToString());
        //    // set the default column sorting here, if first time then required to set 
        //    if (sortColumnName == "0" || sortColumnName == "undefined")
        //        //sortColumnName = "ItemID";
        //        sortColumnName = "ItemNumber";

        //    if (sortDirection == "asc")
        //        sortColumnName = sortColumnName + " asc";
        //    else
        //        sortColumnName = sortColumnName + " desc";

        //    string searchQuery = string.Empty;

        //    int TotalRecordCount = 0;

        //    //sortColumnName = "ItemID asc";

        //    //ReceiveMasterController controller = new ReceiveMasterController();
        //    ReceiveOrderDetailsDAL controller = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
        //    IEnumerable<ReceiveOrderLineItemDetailsDTO> DataFromDB = controller.GetPagedPendigLineItemsRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

        //    var result = from u in DataFromDB
        //                 select new ReceiveOrderLineItemDetailsDTO
        //                 {
        //                     ID = u.ID,
        //                     ReceivedQuantity = u.ReceivedQuantity,
        //                     RequestedQuantity = u.RequestedQuantity,
        //                     ItemNumber = u.ItemNumber,
        //                     ItemGUID = u.ItemGUID,
        //                     Description = u.Description,
        //                     OrderID = u.OrderID,
        //                     RequiredDate = u.RequiredDate,
        //                     Room = u.Room,
        //                     CurrentReceivedDate = DateTime.Now,
        //                     ItemType = u.ItemType,
        //                 };
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
        //        aaData = result
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// GetOrderDetails
        /// </summary>
        /// <param name="OrderDetailID"></param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult GetOrderDetails(string OrderDetailID)
        //{
        //    string status = "";
        //    try
        //    {
        //        //ReceiveMasterController objReceiveMaster = new ReceiveMasterController();
        //        ReceiveOrderDetailsDAL objReceiveMaster = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
        //        //ReceiveOrderLineItemDetailsDTO objData = objReceiveMaster.GetLineItemsOrderDetails(Int64.Parse(OrderDetailID.ToString()));
        //        ReceiveOrderLineItemDetailsDTO objData = objReceiveMaster.GetLineItemsOrderDetails(Guid.Parse(OrderDetailID));
        //        status = "success";
        //        return Json(new { Status = status, Data = objData }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        status = "fail";
        //        return Json(new { Status = status }, JsonRequestBehavior.AllowGet);

        //    }

        //}

        /// <summary>
        /// GetReceivedLineItemsDetails
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //public JsonResult GetReceivedLineItemsDetails(JQueryDataTableParamModel param)
        //{
        //    int TotalRecordCount = 0;
        //    Guid OrderDetaiGUID = Guid.Parse(Request["DetailId"]);
        //    //ReceiveOrderDetailsController OrderDetailController = new ReceiveOrderDetailsController();
        //    ReceiveOrderDetailsDAL OrderDetailController = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
        //    IEnumerable<ReceiveOrderDetailsDTO> DataFromDB = OrderDetailController.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID);

        //    var result = from x in DataFromDB
        //                 where x.OrderDetailGUID == OrderDetaiGUID && x.IsArchived == false && x.IsDeleted == false
        //                 orderby x.ID descending
        //                 select new ReceiveOrderDetailsDTO
        //                 {
        //                     ReceiveBin = x.ReceiveBin,
        //                     ReceiveDate = x.ReceiveDate,
        //                     ReceiveQuantity = x.ReceiveQuantity,
        //                     UpdatedByName = x.UpdatedByName,
        //                     LastUpdated = x.LastUpdated,
        //                     Created = x.Created,
        //                     CreatedByName = x.CreatedByName,
        //                     RoomName = x.RoomName,
        //                     ID = x.ID

        //                 };
        //    TotalRecordCount = result.Count();
        //    return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = result }, JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        //public ActionResult LoadReceiveOrderDetailsModel(string OrderDetailID)
        //{
        //    OrderDetailsDTO obj = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(Guid.Parse(OrderDetailID), SessionHelper.RoomID, SessionHelper.CompanyID);

        //    //LocationMasterController objLocationCntrl = new LocationMasterController();
        //    LocationMasterDAL objLocationCntrl = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
        //    List<LocationMasterDTO> lstLocation = objLocationCntrl.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
        //    // lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = "-- Select Location --" });
        //    ViewBag.LocationList = lstLocation;

        //    return PartialView("ItemReceivedInfo", obj);
        //}

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        //public ActionResult GetOrderDetails(ReceivedOrderListJQueryDataTableParamModel param)
        //{
        //    //string status = "";
        //    //try
        //    //{
        //    //    ReceiveMasterController objReceiveMaster = new ReceiveMasterController();
        //    //    OrderDetailsDTO objData = objReceiveMaster.GetLineItemsOrderDetails(Int64.Parse(OrderDetailID.ToString()));
        //    //    status = "success";
        //    //    return Json(new { Status = status, Data = objData }, JsonRequestBehavior.AllowGet);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    status = "fail";
        //    //    return Json(new { Status = status }, JsonRequestBehavior.AllowGet);

        //    //}

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
        //    bool IsArchived = false;// bool.Parse(Request["IsArchived"].ToString());
        //    bool IsDeleted = false;// bool.Parse(Request["IsDeleted"].ToString());
        //    // set the default column sorting here, if first time then required to set 
        //    if (sortColumnName == "0" || sortColumnName == "undefined")
        //        sortColumnName = "ID";

        //    if (sortDirection == "asc")
        //        sortColumnName = sortColumnName + " asc";
        //    else
        //        sortColumnName = sortColumnName + " desc";

        //    string searchQuery = string.Empty;

        //    int TotalRecordCount = 0;


        //    ReceiveMasterController controller = new ReceiveMasterController();
        //    IEnumerable<OrderDetailsDTO> DataFromDB = null;//controller.GetLineItemsOrderDetails(Int64.Parse(param.OrderDetailID.ToString()));
        //    //IEnumerable<OrderDetailsDTO> DataFromDB = controller.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

        //    var result = from u in DataFromDB
        //                 select new OrderDetailsDTO
        //                 {
        //                     ID = u.ID,
        //                     
        //                     ReceivedQuantity = u.ReceivedQuantity,
        //                     RequestedQuantity = u.RequestedQuantity,
        //                     ItemNumber = u.ItemNumber,
        //                     Description = u.Description,
        //                     OrderID = u.OrderID,
        //                     RequiredDate = u.RequiredDate,
        //                     Room = u.Room,
        //                 };
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
        //        aaData = result
        //    },
        //                JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// RetrivedItems
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <param name="ItemType"></param>
        /// <returns></returns>
        //public ActionResult LoadReceiveListItemsModel(string TypeID)
        //{
        //    OrderDetailsDTO obj = null;
        //    return PartialView("ReceiveListItems", obj);
        //}

        /// <summary>
        /// RetrivedItems
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <param name="ItemType"></param>
        /// <returns></returns>
        //public string RetrivedItems(string ItemGUID, string ItemType)
        //{

        //    //ViewBag.ItemID = ItemID;
        //    //ViewBag.ItemType = ItemType;

        //    //using (ReceiveMasterController objReceiveMaster = new ReceiveMasterController())
        //    //{
        //    //    ViewBag.ReceivedOrder = objReceiveMaster.GetLineItemsOrderRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemID, 0).ToList();
        //    //}
        //    ModelForReceivedItems obj = new ModelForReceivedItems();
        //    //obj.ItemID = ItemID;
        //    obj.ItemGUID = ItemGUID;
        //    obj.ItemType = ItemType;
        //    //using (ReceiveMasterController objReceiveMaster = new ReceiveMasterController())
        //    ReceiveOrderDetailsDAL objReceiveMaster = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
        //    obj.ReceivedOrderList = objReceiveMaster.GetLineItemsOrderRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID, 0).ToList();

        //    return RenderRazorViewToString("_ReceiveItems", obj);
        //}

        /// <summary>
        /// RenderRazorViewToString
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        //public string RenderRazorViewToString(string viewName, object model)
        //{
        //    ViewData.Model = model;
        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
        //        var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}

        //public ActionResult LoadReceiveOrders(Int64 ItemID)



        /// <summary>
        /// GetOrderDetailsFromStatus
        /// </summary>
        /// <param name="OrderStatus"></param>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult GetOrderDetailsFromStatus(string OrderStatus, string ItemID)
        //{
        //    string status = "";
        //    try
        //    {
        //        //ReceiveMasterController objReceiveMaster = new ReceiveMasterController();
        //        ReceiveOrderDetailsDAL objReceiveMaster = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
        //        List<ReceiveOrderLineItemDetailsDTO> objData = objReceiveMaster.GetStatuswiseOrderRecords(SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ItemID), Int64.Parse(OrderStatus)).ToList();
        //        status = "success";
        //        return Json(new { Status = status, Data = objData }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        status = "fail";
        //        return Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        //    }
        //}




        /// <summary>
        /// Received Item Detail Inner Grid Without staging
        /// </summary>
        /// <param name="OrderDetailID"></param>
        /// <returns></returns>
        //public PartialViewResult GetReceivedDetailInnerGrid(string OrderDetailID)
        //{
        //    OrderDetailsDTO OrdDetailDTO = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(Guid.Parse(OrderDetailID), SessionHelper.RoomID, SessionHelper.CompanyID);
        //    return PartialView("_ReceivedItemsDetail", OrdDetailDTO);
        //}

        #endregion

    }



}
