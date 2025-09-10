using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.BAL;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Data;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ReceiveController : eTurnsControllerBase
    {
        #region New Methods

        private Int64 RoomID = SessionHelper.RoomID;
        private Int64 CompanyID = SessionHelper.CompanyID;
        private Int64 UserID = SessionHelper.UserID;
        private Int64 EnterpriseId = SessionHelper.EnterPriceID;
        private List<long> SupplierFilterIds = SessionHelper.UserSupplierIds;
        char[] commaTrim = { ',' };

        /// <summary>
        /// Order List
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiveList()
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<ItemMasterDTO> DataFromDB = obj.GetPagedRecordsForModel(0, 10000, out TotalRecordCount, "", "Id Desc", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, "", "", "", SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, isQuickListRequired: true);

            Session["ItemMasterList"] = DataFromDB;
            return View();
        }

        /// <summary>
        /// New: OrderMasterListAjax = ReceiveListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ReceiveListAjax(JQueryDataTableParamModel param)
        {
            OrderDetailsDAL orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

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
            string selectedSupplier = "";
            string strSupplierOptions = "";
            if (Request["SelectedSupplier"] != null)
            {
                selectedSupplier = Request["SelectedSupplier"].ToString();
                IsRequestFromDashboard = true;
            }

            bool IsArchived = false;
            if (Request["IsArchived"] != null)
                IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = false;
            if (Request["IsDeleted"] != null)
                IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            //if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
            //    sortColumnName = "ItemNumber Asc";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ItemNumber desc";
            }
            else
                sortColumnName = "ItemNumber desc";

            string searchQuery = string.Empty;

            //if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower() == "ordernumber")
            //    sortColumnName = "OrderNumber_ForSorting";

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower().Contains("ordernumber"))
                sortColumnName = sortColumnName.Replace("OrderNumber", "OrderNumber_ForSorting");

            int TotalRecordCount = 0;

            ReceiveOrderDetailsDAL controller = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ReceivableItemDTO> DataFromDBAll = new List<ReceivableItemDTO>();
            List<ReceivableItemDTO> DataFromDB = new List<ReceivableItemDTO>();
            List<ReceivableItemDTO> SupplierList = new List<ReceivableItemDTO>();

            if (IsRequestFromDashboard == true)
            {
                DataFromDBAll = controller.GetALLReceiveListByPagingForDashboard(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, OrderStatusIn, selectedSupplier, out SupplierList, SessionHelper.UserSupplierIds).ToList();
                if (DataFromDBAll != null && DataFromDBAll.Count > 0)
                {
                    DataFromDB = DataFromDBAll;
                }
                else
                    DataFromDB = new List<ReceivableItemDTO>();

                //-------------------Prepare DDL Data-------------------
                //                
                if (SupplierList.Count > 0 && SupplierList.Any())
                {
                    List<string> lstSupplier = SupplierList.Select(x => "<option value='" + x.OrderSupplierID + "'>" + x.OrderSupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
                    strSupplierOptions = string.Join("", lstSupplier);
                }
            }
            else
            {
                string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                DataFromDB = controller.GetALLReceiveListByPaging(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, OrderStatusIn, SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone).ToList();
            }
            List<PreReceivOrderDetailDTO> orderDetailTrackingList = orderDetailDAL.GetOrderDetailTrackingList(SessionHelper.RoomID, SessionHelper.CompanyID);

            if (DataFromDB != null)
            {
                //DataFromDB.ToList().ForEach(t => t.strReqDtlDate = FnCommon.ConvertDateByTimeZone(t.OrderDetailRequiredDate, false, true));
                DataFromDB.ForEach(x =>
                {
                    x.strReqDtlDate = FnCommon.ConvertDateByTimeZone(x.OrderDetailRequiredDate, false, true);
                    if (x.OrderUOMValue == null || x.OrderUOMValue <= 0)
                    {
                        x.OrderUOMValue = 1;
                    }

                    if (x.IsAllowOrderCostuom == true)
                    {
                        x.RequestedQuantity = x.RequestedQuantity / (double)x.OrderUOMValue;
                        x.ApprovedQuantity = x.ApprovedQuantity / (double)x.OrderUOMValue;
                        x.ReceivedQuantity = x.ReceivedQuantity / (double)x.OrderUOMValue;
                    }
                    List<PreReceivOrderDetailDTO> orderDetailTrackingListByID = orderDetailTrackingList.Where(z => z.OrderDetailGUID == x.OrderDetailGUID).Select(q => q).ToList();
                    if (orderDetailTrackingListByID.Count == 1)
                    {
                        x.PackSlipNumber = orderDetailTrackingListByID[0].PackSlipNumber;
                    }

                    if (SiteSettingHelper.IsBorderStateURL == "1" && SiteSettingHelper.EnableOktaLoginForSpecialUrls.Contains(eTurnsWeb.Helper.SessionHelper.CurrentDomainURL))
                    {
                        x.isOktaEnable = true;
                    }
                });
            }
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB,
                SupplierOptions = strSupplierOptions,
                SelectedSupplier = selectedSupplier,
                SearchTerm = param.sSearch,
                StartIndex = param.iDisplayStart
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// New: ReceiveItem  =RetrivedItems(string ItemGUID, string ItemType)
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiveItem(Guid OrderDetailGUID)
        {
            ReceivableItemDTO objDTO = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetALLReceiveList(SessionHelper.RoomID, SessionHelper.CompanyID, null, null, null, OrderDetailGUID).FirstOrDefault();
            BinMasterDAL objCommon = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDTO objBinMasterDTO = null;
            if (objDTO != null)
            {
                if (string.IsNullOrEmpty(objDTO.ReceiveBinName))
                {
                    if (objDTO.ReceiveBinID.GetValueOrDefault(0) > 0)
                    {
                        objBinMasterDTO = objCommon.GetBinMasterByBinID(objDTO.ReceiveBinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objBinMasterDTO != null)
                        {
                            objDTO.ReceiveBinName = objBinMasterDTO.BinNumber;
                        }
                    }
                    else if (objDTO.ItemDefaultLocation > 0)
                    {
                        objBinMasterDTO = objCommon.GetBinMasterByBinID(objDTO.ItemDefaultLocation, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objBinMasterDTO != null)
                        {
                            objDTO.ReceiveBinName = objBinMasterDTO.BinNumber;
                        }
                    }
                }
            }
            //IEnumerable<BinMasterDTO> objBinDTO = null;
            //if (objDTO != null && objDTO.StagingID <= 0)
            //{
            //    objBinDTO = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false);
            //    // objBinDTO = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, false);//.Where(x => x.IsStagingLocation == false);
            //}
            //else if (objDTO != null)
            //{
            //    objBinDTO = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == true && x.ID == objDTO.StagingID);
            //    //    objBinDTO = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, objDTO.StagingID, null, true);//.Where(x => x.IsStagingLocation == true && x.ID == objDTO.StagingID);
            //}

            //ViewBag.BinLocations = objBinDTO;

            //if (!string.IsNullOrEmpty(objDTO.ReceiveBinName))
            //{
            //    if (objDTO.ReceiveBinID.GetValueOrDefault(0) > 0)
            //    {
            //        if (objBinDTO.ToList().FindIndex(x => x.ID == objDTO.ReceiveBinID.GetValueOrDefault(0)) >= 0)
            //            objDTO.ReceiveBinName = objBinDTO.Where(x => x.ID == objDTO.ReceiveBinID.GetValueOrDefault(0)).FirstOrDefault().BinNumber;
            //    }
            //    else if (objDTO.ItemDefaultLocation > 0)
            //    {
            //        if (objBinDTO.ToList().FindIndex(x => x.ID == objDTO.ItemDefaultLocation) >= 0)
            //            objDTO.ReceiveBinName = objBinDTO.Where(x => x.ID == objDTO.ItemDefaultLocation).FirstOrDefault().BinNumber;
            //    }
            //}

            if (objDTO != null)
            {
                objDTO.IsOnlyFromUI = true;
                if (objDTO.IsAllowOrderCostuom)
                    objDTO.RequestedQuantity = objDTO.RequestedQuantity / (int)objDTO.OrderUOMValue;
                if (objDTO.IsAllowOrderCostuom)
                    objDTO.ApprovedQuantity = objDTO.ApprovedQuantity / (int)objDTO.OrderUOMValue;
                if (objDTO.IsAllowOrderCostuom)
                    objDTO.ReceivedQuantity = objDTO.ReceivedQuantity / (int)objDTO.OrderUOMValue;
            }
            return PartialView("_ReceiveItem", (objDTO == null ? new ReceivableItemDTO() : objDTO));
        }

        /// <summary>
        /// ReceivedItemDetail
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceivedItemDetail(ReceivableItemDTO objDTO)
        {
            ReceivedOrderTransferDetailDAL objROTDDAL = null;
            OrderMasterDAL objOrderMasterDAL = null;
            OrderMasterDTO objOrderDTO = null;
            OrderDetailsDAL objOrderDetailDAL = null;
            OrderDetailsDTO objOrderDetailDTO = null;
            try
            {
                objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                objDTO.ReceivedItemDetail = objROTDDAL.GetROTDByOrderDetailGUIDFull(objDTO.OrderDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID).OrderByDescending(x => x.ID).ToList();
                if (objDTO.ReceivedItemDetail != null && objDTO.ReceivedItemDetail.Count() > 0)
                {
                    objDTO.ReceivedItemDetail.ToList().ForEach(x =>
                    {
                        if (x.OrderUOMValue <= 0)
                            x.OrderUOMValue = 1;

                        if (x.SerialNumberTracking == false && x.IsAllowOrderCostuom)
                        {
                            if (x.CustomerOwnedQuantity != null && x.CustomerOwnedQuantity.GetValueOrDefault(0) >= 1)
                                x.CustomerOwnedQuantity = x.CustomerOwnedQuantity / x.OrderUOMValue;

                            if (x.ConsignedQuantity != null && x.ConsignedQuantity.GetValueOrDefault(0) >= 1)
                                x.ConsignedQuantity = x.ConsignedQuantity / x.OrderUOMValue;
                        }
                    });
                }
                objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                objOrderDTO = objOrderMasterDAL.GetOrderByGuidPlain(objDTO.OrderGUID);
                objDTO.OrderStatus = objOrderDTO.OrderStatus;

                objOrderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                objOrderDetailDTO = objOrderDetailDAL.GetOrderDetailByGuidPlain(objDTO.OrderDetailGUID, RoomID, CompanyID);
                objDTO.IsCloseItem = (objOrderDetailDTO != null ? objOrderDetailDTO.IsCloseItem.GetValueOrDefault(false) : false);

                return PartialView("_ReceivedItemDetail", objDTO);
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
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
        public ActionResult ReceivedItemsByOrder(Guid dataGUID)
        {
            ReceivedOrderTransferDetailDAL objROTDDAL = null;
            IEnumerable<ReceivedOrderTransferDetailDTO> lstReceivedItems;
            try
            {
                objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                lstReceivedItems = objROTDDAL.GetROTDByOrderGUIDFull(dataGUID, SessionHelper.RoomID, SessionHelper.CompanyID).OrderByDescending(x => x.ID).ToList();

                lstReceivedItems.ToList().ForEach(x =>
                {
                    if (x.OrderUOMValue <= 0)
                        x.OrderUOMValue = 1;

                    if (x.SerialNumberTracking == false && x.IsAllowOrderCostuom && x.OrderType == (int)OrderType.Order)
                    {
                        if (x.ConsignedQuantity != null && x.ConsignedQuantity > 0)
                            x.ConsignedQuantity = x.ConsignedQuantity / x.OrderUOMValue;

                        if (x.CustomerOwnedQuantity != null && x.CustomerOwnedQuantity > 0)
                            x.CustomerOwnedQuantity = x.CustomerOwnedQuantity / x.OrderUOMValue;
                    }
                });

                ViewBag.dataGuid = dataGUID;
                ViewBag.dataUrl = "ReceivedItemsByOrder";
                return PartialView("_ReceivedDetail", lstReceivedItems);

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
        /// ReceivedItemDetail
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceivedItemsByOrderDetail(Guid dataGUID)
        {
            ReceivedOrderTransferDetailDAL objROTDDAL = null;
            IEnumerable<ReceivedOrderTransferDetailDTO> lstReceivedItems;

            try
            {
                objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                lstReceivedItems = objROTDDAL.GetROTDByOrderDetailGUIDFull(dataGUID, SessionHelper.RoomID, SessionHelper.CompanyID).OrderByDescending(x => x.ID).ToList();
                lstReceivedItems.ToList().ForEach(x =>
                {
                    if (x.OrderUOMValue <= 0)
                        x.OrderUOMValue = 1;

                    if (x.SerialNumberTracking == false && x.IsAllowOrderCostuom && x.OrderType == (int)OrderType.Order)
                    {
                        if (x.ConsignedQuantity != null && x.ConsignedQuantity > 0)
                            x.ConsignedQuantity = x.ConsignedQuantity / x.OrderUOMValue;

                        if (x.CustomerOwnedQuantity != null && x.CustomerOwnedQuantity > 0)
                            x.CustomerOwnedQuantity = x.CustomerOwnedQuantity / x.OrderUOMValue;
                    }
                });
                ViewBag.dataGuid = dataGUID;
                ViewBag.dataUrl = "ReceivedItemsByOrderDetail";
                return PartialView("_ReceivedDetail", lstReceivedItems);

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
        public ActionResult LoadAllItems()
        {
            return PartialView("ReceiveItemWithoutOrder");
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult NewReceiveItemsListAjax(QuickListJQueryDataTableParamModel param)
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

            bool IsArchived = false;
            bool IsDeleted = false;

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
            //    sortColumnName = "ItemNumber Asc";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ItemNumber desc";
            }
            else
                sortColumnName = "ItemNumber desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "", "", "", SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, isQuickListRequired: true);

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
        public ActionResult ReceiveNewItemWithoutOrderInnerGrid(string ItemGuid, string ItemType)
        {
            object objItem = null;
            string returnView = "";
            ItemMasterDAL itemDAL = null;
            QuickListDAL qlDAL = null;
            SupplierMasterDAL objSupDAL = null;
            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
            AutoOrderNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            try
            {
                if (!string.IsNullOrEmpty(ItemType) && ItemType.ToLower().Contains("quick list"))
                {
                    qlDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                    returnView = "ReceiveNewQuickList";
                    objItem = qlDAL.GetRecord(ItemGuid, RoomID, CompanyID, false, false);
                    ((QuickListMasterDTO)objItem).QuickListDetailList = qlDAL.GetQuickListItemsRecords(RoomID, CompanyID, ItemGuid, false, false, SupplierFilterIds);
                    List<QuickListDetailDTO> QuickListDetailListRequirePackslip = qlDAL.GetQuickListItemsRequirePackslip(RoomID, CompanyID, ItemGuid, false, false);
                    ViewBag.IsPackSlipNumberMandatory = false;

                    if (QuickListDetailListRequirePackslip != null && QuickListDetailListRequirePackslip.Count > 0)
                    {
                        ViewBag.IsPackSlipNumberMandatory = true;
                    }
                   
                    if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                    {
                        var strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
                        var suppliers = objSupDAL.GetSupplierByIDsPlain(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);

                        if (suppliers != null && suppliers.Any())
                        {
                            lstSupplier.AddRange(suppliers);
                        }
                        if (lstSupplier != null && lstSupplier.Any() && lstSupplier.Count() > 0)
                        {
                            lstSupplier = lstSupplier.OrderBy(x => x.SupplierName).ToList();
                        }
                    }
                    else
                    {
                        lstSupplier = objSupDAL.GetSupplierByRoomPlain(RoomID, CompanyID, false).OrderBy(x => x.SupplierName).ToList();
                    }

                    Int64 DefualtRoomSupplier = 0;
                    // objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,DefaultSupplierID";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                    DefualtRoomSupplier = objRoomDTO.DefaultSupplierID.GetValueOrDefault(0);

                    if (DefualtRoomSupplier > 0)
                    {
                        lstSupplier = lstSupplier.Where(x => x.ID == DefualtRoomSupplier).ToList();
                    }
                    else
                    {
                        lstSupplier.Insert(0, null);
                    }

                    ViewBag.SupplierList = lstSupplier;

                    objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                    objAutoNumber = objAutoSeqDAL.GetNextOrderNumber(RoomID, CompanyID, DefualtRoomSupplier, EnterpriseId);
                    ViewBag.OrderNumber = objAutoNumber.OrderNumber;
                    ViewBag.OrderNumberForSort = objAutoNumber.OrderNumberForSorting;

                    return PartialView(returnView, (QuickListMasterDTO)objItem);
                }
                else
                {
                    itemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    Int64 DefualtItemSupplier = 0;
                    //objItem = itemDAL.GetRecord(ItemGuid, RoomID, CompanyID);
                    objItem = itemDAL.GetItemWithMasterTableJoins(null, Guid.Parse(ItemGuid), RoomID, CompanyID);
                    objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                    objAutoNumber = objAutoSeqDAL.GetNextOrderNumber(RoomID, CompanyID, ((ItemMasterDTO)objItem).SupplierID.GetValueOrDefault(0), EnterpriseId);
                    ViewBag.AutoNumber = objAutoNumber;
                    ViewBag.OrderNumber = objAutoNumber.OrderNumber;
                    ViewBag.BlanketPOId = objAutoNumber.BlanketPOs != null && objAutoNumber.BlanketPOs.Any() && objAutoNumber.BlanketPOs.Count > 0 ? objAutoNumber.BlanketPOs[0].ID : 0;
                    ViewBag.OrderNumberForSort = string.IsNullOrEmpty(objAutoNumber.OrderNumberForSorting) ? objAutoNumber.OrderNumber : objAutoNumber.OrderNumberForSorting;
                    
                    ViewBag.SupplierList = lstSupplier;
                    lstSupplier = objSupDAL.GetSupplierByRoomPlain(RoomID, CompanyID, false).OrderBy(x => x.SupplierName).ToList();

                    // Ensure a valid default selection
                    DefualtItemSupplier = ((ItemMasterDTO)objItem).SupplierID.GetValueOrDefault(0);

                    lstSupplier.Insert(0, null);
                    ViewBag.SupplierList = lstSupplier;

                    returnView = "ReceiveNewItemWithoutOrdNotSerial";
                    if (((ItemMasterDTO)objItem).SerialNumberTracking)
                        returnView = "ReceiveNewItemWithoutOrdSerial";
                    return PartialView(returnView, (ItemMasterDTO)objItem);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                itemDAL = null;
                qlDAL = null;
                objSupDAL = null;
                lstSupplier = null;
                objAutoNumber = null;
                objAutoSeqDAL = null;
            }

        }

        /// <summary>
        /// SaveNewReceiveWithoutOrder
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveNewReceiveWithoutOrder(ReceivableItemDTO objDTO)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            ReceiveOrderDetailsDAL objRecOrdDetDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<OrderDetailsDTO> objOrdDetailDTO = null;
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);

            string rcvDateValid = string.Empty;
            DateTime dtRcvTemp;


            //DateTime.TryParseExact(objDTO.OrderRequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dtRcvTemp);
            DateTime.TryParseExact(objDTO.OrderRequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtRcvTemp);


            DateTime currentDateAsPerRoom = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
            DateTime newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, objDTO.OrderRequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, SessionHelper.CurrentTimeZone, SessionHelper.RoomTimeFormat);


            OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            if (dtRcvTemp <= DateTime.MinValue)
            {
                rcvDateValid = ResReceiveOrderDetails.EnterValidReceivedDate;
            }

            if (!string.IsNullOrEmpty(rcvDateValid))
            {
                return Json(new { Message = rcvDateValid, Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            #region WI-7318	AB Integration | Sync Item cost when an item Added to Order line item.
            List<string> ASINs = new List<string>();
            Dictionary<List<ItemMasterDTO>, string> lstNonOrderableItems = new Dictionary<List<ItemMasterDTO>, string>();

            if (SessionHelper.AllowABIntegration)
            {
                ItemMasterDTO objABItemDTO = new ItemMasterDTO();
                objABItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithMasterTableJoins(null, objDTO.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                if (objABItemDTO != null && !string.IsNullOrWhiteSpace(objABItemDTO.SupplierPartNo))
                {
                    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                    Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objABItemDTO.SupplierPartNo, objABItemDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    if (ABItemMappingID > 0)
                    {
                        ASINs.Add(objABItemDTO.SupplierPartNo);
                    }
                }
                if (ASINs != null && ASINs.Count > 0)
                {
                    lstNonOrderableItems = eTurns.ABAPIBAL.Helper.ABAPIHelper.ItemSyncToRoom(ASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                }
            }
            #endregion

            ItemMasterDTO objItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objDTO.ItemGUID);

            if (SessionHelper.AllowABIntegration && lstNonOrderableItems != null && lstNonOrderableItems.Count > 0
                            && lstNonOrderableItems.Values.Contains("success")
                             && lstNonOrderableItems.Keys.Count > 0
                             && lstNonOrderableItems.Keys.SelectMany(c => c).ToList().Count > 0
                             && objItemDTO != null)
            {
                List<ItemMasterDTO> lstReturnsItems = new List<ItemMasterDTO>();
                lstReturnsItems = lstNonOrderableItems.Keys.SelectMany(c => c).ToList();
                if (lstReturnsItems != null && lstReturnsItems.Count > 0
                    && lstReturnsItems.Where(x => x.GUID == objItemDTO.GUID).Count() > 0)
                {
                    return Json(new { Message = string.Format(ResOrder.ItemnotOrderable, objItemDTO.ItemNumber), Status = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }

            objDTO.OrderRequiredDate = newReceiveTempDate;//objDTO.OrderRequiredDateStr != null ? DateTime.ParseExact(objDTO.OrderRequiredDateStr, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult) : Convert.ToDateTime(objDTO.OrderRequiredDateStr);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            RoomDTO roomDTO = null;
            long SessionUserId = SessionHelper.UserID;

            foreach (var i in DataFromDB)
            {
                if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF1))
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                    if (!string.IsNullOrEmpty(val))
                        i.UDFDisplayColumnName = val;
                    else
                        i.UDFDisplayColumnName = i.UDFColumnName;
                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }
                else if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF2))
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                    if (!string.IsNullOrEmpty(val))
                        i.UDFDisplayColumnName = val;
                    else
                        i.UDFDisplayColumnName = i.UDFColumnName;
                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }
                else if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF3))
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                    if (!string.IsNullOrEmpty(val))
                        i.UDFDisplayColumnName = val;
                    else
                        i.UDFDisplayColumnName = i.UDFColumnName;
                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }
                else if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF4))
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                    if (!string.IsNullOrEmpty(val))
                        i.UDFDisplayColumnName = val;
                    else
                        i.UDFDisplayColumnName = i.UDFColumnName;
                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }
                else if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF5))
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
            if (objDTO != null && objDTO.OrderNumber != null)
            {
                if (objDTO.OrderNumber.Length > 22)
                {
                    return Json(new { Message = ResOrder.OrderNumberLengthUpto22Char, Status = "Error" }, JsonRequestBehavior.AllowGet);
                }

                //----------------------Check For Order Number Duplication----------------------
                //
                string strOK = string.Empty;
                // roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string columnList = "ID,RoomName,IsAllowOrderDuplicate";
                roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                if (roomDTO.IsAllowOrderDuplicate != true)
                {
                    OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                    if (objDTO.OrderGUID != null && objDTO.OrderGUID != Guid.Empty)
                    {
                        if (objOrderMasterDAL.IsOrderNumberDuplicateByGuid(objDTO.OrderNumber, objDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID))
                        {
                            strOK = "duplicate";
                        }
                    }
                    else
                    {
                        if (objOrderMasterDAL.IsOrderNumberDuplicateById(objDTO.OrderNumber, 0, SessionHelper.RoomID, SessionHelper.CompanyID))
                        {
                            strOK = "duplicate";
                        }
                    }
                }

                if (strOK == "duplicate")
                {
                    return Json(new { Message = string.Format(ResMessage.DuplicateMessage, ResOrder.OrderNumber, objDTO.OrderNumber), Status = "Error" }, JsonRequestBehavior.AllowGet);
                }
                //
                //-------------------------------------------------------------------------------
            }

            if (objDTO.DateCodeTracking)
            {
                CommonDAL cmnDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DateTime dtExpDate;
                string expValid = string.Empty;
                foreach (var item in objDTO.ReceivedItemDetail)
                {
                    //DateTime.TryParseExact(item.Expiration, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out dtExpDate);
                    DateTime.TryParseExact(item.Expiration, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtExpDate);

                    if (objItemDTO != null && objItemDTO.LotNumberTracking)
                    {
                        if (string.IsNullOrWhiteSpace(item.LotNumber) || dtExpDate <= DateTime.MinValue)
                        {
                            expValid = ResReceiveOrderDetails.EnterLotAndValidExpirationDate;
                            break;
                        }
                        else
                        {
                            string Expiration = dtExpDate.ToString("MM/dd/yyyy");
                            string msg = cmnDAL.CheckDuplicateLotAndExpiration(item.LotNumber, Expiration, dtExpDate, 0, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.ItemGUID, SessionHelper.UserID, SessionHelper.EnterPriceID);
                            if (string.IsNullOrWhiteSpace(msg) || (msg ?? string.Empty).ToLower() == "ok")
                            {
                                item.ExpirationDate = dtExpDate;
                            }
                            else
                            {
                                expValid = msg;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (dtExpDate <= DateTime.MinValue)
                        {
                            expValid = ResPullMaster.EnterValidExpirationDate;
                            break;
                        }
                        else
                        {
                            item.ExpirationDate = dtExpDate;
                        }
                    }

                }
                if (!string.IsNullOrEmpty(expValid))
                {
                    return Json(new { Message = expValid, Status = "Error" }, JsonRequestBehavior.AllowGet);
                }

            }


            InventoryController objInvCtrl = new InventoryController();
            BinMasterDTO objbinDTO = objItemLocationDetailsDAL.GetItemBinPlain(objDTO.ItemGUID, objDTO.ReceiveBinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);

            //#region Validating Receive For Item/Bin Max (WI-4561)

            //if (roomDTO == null)
            //{
            //    roomDTO = new RoomDAL(SessionHelper.EnterPriseDBName).GetRecord(RoomID, CompanyID, false, false);
            //}

            //if (roomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder)
            //{
            //    if (objItemDTO.IsItemLevelMinMaxQtyRequired.HasValue && objItemDTO.IsItemLevelMinMaxQtyRequired.Value)
            //    {
            //        if (objItemDTO.MaximumQuantity.HasValue && objItemDTO.MaximumQuantity.Value > 0 && (((objItemDTO.OnOrderQuantity.HasValue ? objItemDTO.OnOrderQuantity.Value : 0) + objDTO.RequestedQuantity) > objItemDTO.MaximumQuantity.Value))
            //        {
            //            return Json(new
            //            {
            //                Message = "Item: " + objItemDTO.ItemNumber + " cannot received , item Maximum Quantity reached.",
            //                Status = "Error"
            //            }, JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //    else
            //    {
            //        BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //        List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemDTO.GUID, RoomID, CompanyID).OrderBy(x => x.BinNumber).ToList();
            //        var maxQtyAtBinLevel = lstItemBins.Where(e => e.BinNumber.Equals(objbinDTO.BinNumber)).FirstOrDefault();
            //        var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : objbinDTO.ID;
            //        var onOrderQtyAtBin = objOrderDetailsDAL.GetOrderdQtyOfItemBinWise(RoomID, CompanyID, objItemDTO.GUID, tmpBinId);

            //        if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0 && ((onOrderQtyAtBin + objDTO.RequestedQuantity) > maxQtyAtBinLevel.MaximumQuantity.Value))
            //        {
            //            return Json(new
            //            {
            //                Message = "Item: " + objItemDTO.ItemNumber + " cannot received , Bin Maximum Quantity reached.",
            //                Status = "Error"
            //            }, JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //}

            //#endregion

            if (objDTO.OrderGUID == Guid.Empty && objDTO.OrderDetailGUID == Guid.Empty)
            {
                //objDTO.ReceiveBinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByName(objDTO.ItemGUID, objDTO.ReceiveBinName, SessionHelper.UserID, RoomID, CompanyID);
                BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(objDTO.ItemGUID, objDTO.ReceiveBinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                objDTO.ReceiveBinID = objBinMasterDTO.ID;
                ItemMasterDTO ImDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidNormal(objDTO.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (ImDTO != null)
                {
                    if (ImDTO.OrderUOMValue == null || ImDTO.OrderUOMValue <= 0)
                        ImDTO.OrderUOMValue = 1;
                }
                if (ImDTO.SerialNumberTracking == false && ImDTO.IsAllowOrderCostuom)
                {
                    //objDTO.RequestedQuantity = objDTO.RequestedQuantity * (double)ImDTO.OrderUOMValue;
                    //objDTO.ReceivedQuantity = objDTO.ReceivedQuantity * (double)ImDTO.OrderUOMValue;
                }
                else if (ImDTO.SerialNumberTracking == true && ImDTO.IsAllowOrderCostuom)
                {
                    objDTO.RequestedQuantity = objDTO.RequestedQuantity * (double)ImDTO.OrderUOMValue;
                    objDTO.SerialNumberTracking = true;
                }

                objOrdDetailDTO = objRecOrdDetDAL.CreateDirectReceiveOrder(objDTO.OrderNumber, objDTO.ItemGUID, objDTO.RequestedQuantity, objDTO.ReceiveBinID.GetValueOrDefault(0), objDTO.OrderRequiredDate, objDTO.ReceivedQuantity, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objDTO.OrderNumber_ForSorting, "Web", DateTimeUtility.DateTimeNow, "Web", DateTimeUtility.DateTimeNow, objDTO.PackSlipNumber, objDTO.ShippingTrackNumber, SessionUserId, objDTO.ItemSupplierID);
                foreach (OrderDetailsDTO OrdDtl in objOrdDetailDTO)
                {
                    ItemMasterDTO ItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidNormal(OrdDtl.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (ItemDTO != null) // && ItemDTO.SerialNumberTracking == false
                    {
                        if (ItemDTO.OrderUOMValue == null || ItemDTO.OrderUOMValue <= 0)
                            ItemDTO.OrderUOMValue = 1;

                        if (OrdDtl.RequestedQuantity != null && OrdDtl.RequestedQuantity.GetValueOrDefault(0) > 0 && ItemDTO.IsAllowOrderCostuom)
                            OrdDtl.RequestedQuantityUOM = OrdDtl.RequestedQuantity / ItemDTO.OrderUOMValue;
                        else
                            OrdDtl.RequestedQuantityUOM = OrdDtl.RequestedQuantity;

                        if (OrdDtl.ReceivedQuantity != null && OrdDtl.ReceivedQuantity.GetValueOrDefault(0) > 0 && ItemDTO.IsAllowOrderCostuom)
                            OrdDtl.ReceivedQuantityUOM = OrdDtl.ReceivedQuantity / ItemDTO.OrderUOMValue;
                        else
                            OrdDtl.ReceivedQuantityUOM = OrdDtl.ReceivedQuantity;

                        if (OrdDtl.ApprovedQuantity != null && OrdDtl.ApprovedQuantity.GetValueOrDefault(0) > 0 && ItemDTO.IsAllowOrderCostuom)
                            OrdDtl.ApprovedQuantityUOM = OrdDtl.ApprovedQuantity / ItemDTO.OrderUOMValue;
                        else
                            OrdDtl.ApprovedQuantityUOM = OrdDtl.ApprovedQuantity;

                        //objOrderDetailsDAL.Edit(OrdDtl);

                        if (objDTO.ReceivedItemDetail != null)
                        {
                            ReceivedOrderTransferDetailDTO RecOrdDtl = objDTO.ReceivedItemDetail.Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemDTO.GUID).FirstOrDefault();
                            if (RecOrdDtl != null)
                            {
                                if (SessionHelper.AllowABIntegration && ASINs != null && ASINs.Count > 0
                                    && objItemDTO != null
                                    && ASINs.Contains(objItemDTO.SupplierPartNo))
                                {
                                    RecOrdDtl.Cost = objItemDTO.Cost.GetValueOrDefault(0);
                                }
                                OrdDtl.ItemCost = RecOrdDtl.Cost;
                            }
                        }

                        #region WI-6215 and Other Relevant order cost related jira

                        if (ItemDTO.CostUOMValue == null || ItemDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                        {
                            ItemDTO.CostUOMValue = 1;
                        }
                        //if (OrdDtl.ItemCostUOMValue == null
                        //    || OrdDtl.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                        //{
                        if (ItemDTO != null)
                        {
                            if (ItemDTO.CostUOMValue == null
                                || ItemDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                                OrdDtl.ItemCostUOMValue = 1;
                            else
                            {
                                OrdDtl.ItemCostUOMValue = ItemDTO.CostUOMValue.GetValueOrDefault(1);
                            }
                        }
                        else
                        {
                            OrdDtl.ItemCostUOMValue = 1;
                        }
                        //}
                        //if (OrdDtl.ItemCostUOMValue.GetValueOrDefault(0) == 0)
                        //    OrdDtl.ItemCostUOMValue = ItemDTO.CostUOMValue.GetValueOrDefault(1);
                        if (OrdDtl.ItemMarkup.GetValueOrDefault(0) == 0)
                            OrdDtl.ItemMarkup = ItemDTO.Markup.GetValueOrDefault(0);

                        if (OrdDtl.ItemCost.GetValueOrDefault(0) > 0)
                        {
                            if (OrdDtl.ItemMarkup > 0)
                            {
                                OrdDtl.ItemSellPrice = OrdDtl.ItemCost + ((OrdDtl.ItemCost * OrdDtl.ItemMarkup) / 100);
                            }
                            else
                            {
                                OrdDtl.ItemSellPrice = OrdDtl.ItemCost;
                            }
                        }
                        else
                        {
                            OrdDtl.ItemSellPrice = 0;
                        }

                        if (ItemDTO != null)
                        {
                            if (OrdDtl.ItemCostUOMValue == null
                                || OrdDtl.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                OrdDtl.ItemCostUOMValue = 1;
                            }
                            OrdDtl.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (OrdDtl.RequestedQuantity.GetValueOrDefault(0) * (OrdDtl.ItemCost.GetValueOrDefault(0) / OrdDtl.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                        : (OrdDtl.ApprovedQuantity.GetValueOrDefault(0) * (OrdDtl.ItemCost.GetValueOrDefault(0) / OrdDtl.ItemCostUOMValue.GetValueOrDefault(1))))));

                            OrdDtl.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (OrdDtl.RequestedQuantity.GetValueOrDefault(0) * (OrdDtl.ItemSellPrice.GetValueOrDefault(0) / OrdDtl.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                        : (OrdDtl.ApprovedQuantity.GetValueOrDefault(0) * (OrdDtl.ItemSellPrice.GetValueOrDefault(0) / OrdDtl.ItemCostUOMValue.GetValueOrDefault(1))))));

                        }

                        #endregion

                        objOrderDetailsDAL.UpdateOrderDetail(OrdDtl);
                    }
                }
            }
            else
            {
                objOrdDetailDTO = objOrderDetailsDAL.GetOrderDetailByOrderGUIDPlain(objDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            ReceivedOrderTransferDetailDTO obj = new ReceivedOrderTransferDetailDTO()
            {
                OrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID,
                //BinID = objDTO.ReceiveBinID,
                BinID = objbinDTO.ID,
                //BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByNameItemGuid(objDTO.ItemGUID, objDTO.ReceiveBinName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false),
                ConsignedQuantity = objItemDTO.Consignment ? objDTO.ReceivedQuantity : 0,
                CustomerOwnedQuantity = objItemDTO.Consignment ? 0 : objDTO.ReceivedQuantity,
                LotNumber = (!string.IsNullOrWhiteSpace(objDTO.ReceivedItemDetail.FirstOrDefault().LotNumber)) ? objDTO.ReceivedItemDetail.FirstOrDefault().LotNumber.Trim() : string.Empty,
                SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.ReceivedItemDetail.FirstOrDefault().SerialNumber)) ? objDTO.ReceivedItemDetail.FirstOrDefault().SerialNumber.Trim() : string.Empty,
                //ExpirationDate = objDTO.ReceivedItemDetail.FirstOrDefault().ExpirationDateStr != null ? DateTime.ParseExact(objDTO.ReceivedItemDetail.FirstOrDefault().ExpirationDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult) : objDTO.ReceivedItemDetail.FirstOrDefault().ExpirationDate,
                ExpirationDate = objDTO.ReceivedItemDetail.FirstOrDefault().ExpirationDate,
                Action = string.Empty,
                BinNumber = objbinDTO.BinNumber,
                CompanyID = SessionHelper.CompanyID,
                Cost = objDTO.ReceivedItemDetail.FirstOrDefault().Cost,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                CriticalQuantity = 0,
                DateCodeTracking = objItemDTO.DateCodeTracking,
                Expiration = objDTO.ReceivedItemDetail.FirstOrDefault().Expiration,
                HistoryID = 0,
                ID = 0,
                IsArchived = false,
                GUID = Guid.Empty,
                IsCreditPull = true,
                IsDeleted = false,
                ItemGUID = objDTO.ItemGUID,
                ItemNumber = objItemDTO.ItemNumber,
                ItemType = objItemDTO.ItemType,
                KitDetailGUID = null,
                LastUpdatedBy = SessionHelper.UserID,
                LotNumberTracking = objItemDTO.LotNumberTracking,
                MaximumQuantity = null,
                MeasurementID = null,
                MinimumQuantity = null,
                mode = string.Empty,
                Received = objDTO.ReceivedItemDetail.FirstOrDefault().Received,
                ReceivedDate = newReceiveTempDate,//dtRcvTemp,
                Room = SessionHelper.RoomID,
                RoomName = SessionHelper.RoomName,
                SerialNumberTracking = objItemDTO.SerialNumberTracking,
                //SuggestedOrderQuantity = null,
                TransferDetailGUID = null,
                UDF1 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF1,
                UDF2 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF2,
                UDF3 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF3,
                UDF4 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF4,
                UDF5 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF5,
                IsEDISent = false,
                Updated = DateTimeUtility.DateTimeNow,
                UpdatedByName = SessionHelper.UserName,
                PackSlipNumber = objDTO.PackSlipNumber,
                AddedFrom = "Web",
                EditedFrom = "Web",
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                IsOnlyFromUI = objDTO.IsOnlyFromUI
            };

            ReceivedOrderTransferDetailDAL rotd = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
            obj.OrderStatus = Convert.ToInt32((OrderStatus.TransmittedInCompletePastDue));

            OrderUOMMasterDAL objOrderUOMDAL = new OrderUOMMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            objItemMasterDTO = objItemMasterDAL.GetItemByGuidNormal(obj.ItemGUID.GetValueOrDefault(Guid.Empty), this.RoomID, this.CompanyID);
            if (objItemMasterDTO.SerialNumberTracking == false && objItemMasterDTO.IsAllowOrderCostuom)
            {
                if (obj.CustomerOwnedQuantity != null && obj.CustomerOwnedQuantity >= 0)
                    obj.CustomerOwnedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, obj.CustomerOwnedQuantity);
                if (obj.ConsignedQuantity != null && obj.ConsignedQuantity >= 0)
                    obj.ConsignedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, obj.ConsignedQuantity);
            }

            rotd.InsertReceive(obj, SessionUserId, objDTO.ReceivedItemDetail.FirstOrDefault().IsReceivedCostChange.GetValueOrDefault(false), SessionHelper.EnterPriceID);
            OrderDetailsDAL objOrderDetail = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            objOrderDetail.UpdateOrderStatusByReceiveNew(objOrdDetailDTO.FirstOrDefault().OrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);
            objOrdDetailDTO = objOrderDetail.GetOrderDetailByOrderGUIDPlain(objOrdDetailDTO.FirstOrDefault().OrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objOrdDetailDTO != null
                && objOrdDetailDTO.FirstOrDefault().ID > 0)
            {
                List<OrderDetailsDTO> lstOrderDetails = new List<OrderDetailsDTO>();
                lstOrderDetails.Add(objOrdDetailDTO.FirstOrDefault());
                OrderMasterDAL orderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                DataTable dtOrdDetails = objOrderDetail.GetOrderDetailTableFromList(lstOrderDetails);
                orderMasterDAL.Ord_UpdateItemCostBasedonOrderDetailCost(SessionHelper.UserID, "Web-DirectOrderApprove", SessionHelper.RoomID, SessionHelper.CompanyID, dtOrdDetails);
            }
            QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
            //objQBItemDAL.InsertQuickBookItem(objDTO.ItemGUID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, "Update", false, SessionHelper.UserID, "Web", null, "New Receive");

            if (true)
            {
                try
                {
                    string receiveGUIDs = "<DataGuids>" + Convert.ToString(obj.GUID) + "</DataGuids>";
                    string eventName = "ORECC";
                    string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                    NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    if (lstNotification != null && lstNotification.Count > 0)
                    {
                        objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, receiveGUIDs);
                    }
                }
                catch (Exception ex)
                {

                    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                }
            }

            return Json(new { Massage = "ok", Success = "ok", OrderGUID = objOrdDetailDTO.FirstOrDefault().OrderGUID, OrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID, ReceivedQty = objOrdDetailDTO.FirstOrDefault().ReceivedQuantity, RequestedQty = objOrdDetailDTO.FirstOrDefault().RequestedQuantity }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// GetOrderNumber
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrderNumber(Int64 ItemID, Int64 SupplierID)
        {
            AutoOrderNumberGenerate objAutoNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextOrderNumber(SessionHelper.RoomID, SessionHelper.CompanyID, SupplierID, EnterpriseId);
            string orderNumber = "";
            string orderNumberForSort = "";

            //if (objAutoNumber.IsBlanketPO)
            //{
            //    ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //    ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(ItemID, null);
            //    ItemSupplierDetailsDAL objISDDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
            //    Guid ItemGuid = Guid.Empty;
            //    Guid.TryParse(objItemDTO.GUID.ToString(), out ItemGuid);
            //    ItemSupplierDetailsDTO objISDDTO = objISDDAL.GetSupplierBySupplierIDByMasterSupplierID(RoomID, CompanyID, ItemGuid, objItemDTO.SupplierID, SupplierID);
            //    if (objISDDTO != null && objISDDTO.BlanketPOID.GetValueOrDefault(0) > 0)
            //    {
            //        if (objAutoNumber.BlanketPOs.Where(x => x.ID == objISDDTO.BlanketPOID.GetValueOrDefault(0) && x.StartDate.GetValueOrDefault(DateTime.MinValue) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTime.MinValue) >= DateTimeUtility.DateTimeNow).Any())
            //            orderNumber = objAutoNumber.BlanketPOs.Where(x => x.ID == objISDDTO.BlanketPOID.GetValueOrDefault(0) && x.StartDate.GetValueOrDefault(DateTime.MinValue) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTime.MinValue) >= DateTimeUtility.DateTimeNow).FirstOrDefault().BlanketPO;
            //    }

            //    if (string.IsNullOrEmpty(orderNumber) || string.IsNullOrWhiteSpace(orderNumber))
            //    {
            //        //orderNumber = objAutoNumber.BlanketPOs.Where(x => x.StartDate.GetValueOrDefault(DateTime.MinValue) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTime.MinValue) >= DateTimeUtility.DateTimeNow).FirstOrDefault().BlanketPO;
            //        //to resolve Object ref error when data not found in start and end date range
            //        SupplierBlanketPODetailsDTO objSuppBlanketPODTO = objAutoNumber.BlanketPOs.Where(x => x.StartDate.GetValueOrDefault(DateTime.MinValue) <= DateTimeUtility.DateTimeNow && x.Enddate.GetValueOrDefault(DateTime.MinValue) >= DateTimeUtility.DateTimeNow).FirstOrDefault();
            //        if (objSuppBlanketPODTO != null)
            //        {
            //            orderNumber = objSuppBlanketPODTO.BlanketPO;
            //        }
            //    }
            //}
            //else
            //{
            orderNumber = objAutoNumber.OrderNumber;
            //}

            if (string.IsNullOrWhiteSpace(orderNumber))
                orderNumber = "";

            orderNumberForSort = objAutoNumber.OrderNumberForSorting;

            if (string.IsNullOrEmpty(orderNumberForSort))
            {
                orderNumberForSort = orderNumber;
            }


            return Json(new { Message = "OK", Status = "OK", OrderNumber = orderNumber, OrderNumberForSort = orderNumberForSort }, JsonRequestBehavior.AllowGet);
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
        public JsonResult SaveReceiveWithOrder(ReceivableItemDTO objDTO)
        {
            ReceiveOrderDetailsDAL objRecOrdDetDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
            InventoryController objInvCtrl = new InventoryController();
            OrderDetailsDAL objOrdDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            JsonResult objResult;
            IEnumerable<OrderDetailsDTO> objOrdDetailDTO;
            var tmpsupplierIds = new List<long>();
            objDTO.OrderRequiredDate = objDTO.OrderRequiredDateStr != null ? DateTime.ParseExact(objDTO.OrderRequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : Convert.ToDateTime(objDTO.OrderRequiredDateStr);
            objOrdDetailDTO = objOrdDetailDAL.GetOrderDetailByOrderGUIDPlain(objDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.GUID == objDTO.OrderDetailGUID);
            List<ItemLocationDetailsDTO> lstLocationWiseDTO = null;
            if (objOrdDetailDTO == null)
                objOrdDetailDTO = objOrdDetailDAL.GetOrderDetailByOrderGUIDPlain(objDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID);

            ItemMasterDTO objItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objDTO.ItemGUID);

            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            foreach (var i in DataFromDB)
            {
                if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF1))
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                    if (!string.IsNullOrEmpty(val))
                        i.UDFDisplayColumnName = val;
                    else
                        i.UDFDisplayColumnName = i.UDFColumnName;
                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }
                else if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF2))
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                    if (!string.IsNullOrEmpty(val))
                        i.UDFDisplayColumnName = val;
                    else
                        i.UDFDisplayColumnName = i.UDFColumnName;
                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }
                else if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF3))
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                    if (!string.IsNullOrEmpty(val))
                        i.UDFDisplayColumnName = val;
                    else
                        i.UDFDisplayColumnName = i.UDFColumnName;
                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }
                else if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF4))
                {
                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                    if (!string.IsNullOrEmpty(val))
                        i.UDFDisplayColumnName = val;
                    else
                        i.UDFDisplayColumnName = i.UDFColumnName;
                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }
                else if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(objDTO.ReceivedItemDetail.FirstOrDefault().UDF5))
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

            // if (SessionHelper.CompanyConfig != null && SessionHelper.CompanyConfig.IsPackSlipRequired.GetValueOrDefault(false))
            if (objDTO.IsPackSlipNumberMandatory)
            {
                if (string.IsNullOrEmpty(objDTO.PackSlipNumber) || string.IsNullOrWhiteSpace(objDTO.PackSlipNumber))
                {
                    if (!string.IsNullOrEmpty(udfRequier))
                    {
                        udfRequier = udfRequier.Replace(".", " " + ResCommon.And + ResReceiveOrderDetails.MsgPackslipMandatory);
                    }
                    else
                    {
                        udfRequier += ResReceiveOrderDetails.MsgPackslipMandatory;
                    }
                }
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                return Json(new { Message = udfRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }


            if (objDTO.StagingID <= 0)
            {
                ItemLocationDetailsDTO obj = new ItemLocationDetailsDTO()
                {
                    OrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID,
                    BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByNameItemGuid(objDTO.ItemGUID, objDTO.ReceiveBinName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false),
                    ConsignedQuantity = objItemDTO.Consignment ? objDTO.ReceivedQuantity : 0,
                    CustomerOwnedQuantity = objItemDTO.Consignment ? 0 : objDTO.ReceivedQuantity,
                    LotNumber = (!string.IsNullOrWhiteSpace(objDTO.ReceivedItemDetail.FirstOrDefault().LotNumber)) ? objDTO.ReceivedItemDetail.FirstOrDefault().LotNumber.Trim() : string.Empty,
                    SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.ReceivedItemDetail.FirstOrDefault().SerialNumber)) ? objDTO.ReceivedItemDetail.FirstOrDefault().SerialNumber.Trim() : string.Empty,
                    ExpirationDate = objDTO.ReceivedItemDetail.FirstOrDefault().ExpirationDate,
                    Action = string.Empty,
                    BinNumber = objDTO.ReceiveBinName,
                    CompanyID = SessionHelper.CompanyID,
                    Cost = objDTO.ReceivedItemDetail.FirstOrDefault().Cost,
                    Created = DateTimeUtility.DateTimeNow,
                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    CriticalQuantity = 0,
                    DateCodeTracking = objItemDTO.DateCodeTracking,
                    eVMISensorID = string.Empty,
                    eVMISensorPort = null,
                    Expiration = objDTO.ReceivedItemDetail.FirstOrDefault().Expiration,
                    HistoryID = 0,
                    ID = 0,
                    IsArchived = false,
                    GUID = Guid.Empty,
                    IsCreditPull = true,
                    IsDeleted = false,
                    ItemGUID = objDTO.ItemGUID,
                    ItemNumber = objItemDTO.ItemNumber,
                    ItemType = objItemDTO.ItemType,
                    KitDetailGUID = null,
                    LastUpdatedBy = SessionHelper.UserID,
                    LotNumberTracking = objItemDTO.LotNumberTracking,
                    MaximumQuantity = null,
                    MeasurementID = null,
                    MinimumQuantity = null,
                    mode = string.Empty,
                    Received = objDTO.ReceivedItemDetail.FirstOrDefault().Received,
                    ReceivedDate = objDTO.ReceivedItemDetail.FirstOrDefault().ReceivedDate,
                    Room = SessionHelper.RoomID,
                    RoomName = SessionHelper.RoomName,
                    SerialNumberTracking = objItemDTO.SerialNumberTracking,
                    SuggestedOrderQuantity = null,
                    TransferDetailGUID = null,
                    UDF1 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF1,
                    UDF2 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF2,
                    UDF3 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF3,
                    UDF4 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF4,
                    UDF5 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF5,
                    UDF6 = string.Empty,
                    UDF7 = string.Empty,
                    UDF8 = string.Empty,
                    UDF9 = string.Empty,
                    UDF10 = string.Empty,
                    Updated = DateTimeUtility.DateTimeNow,
                    UpdatedByName = SessionHelper.UserName,
                    PackSlipNumber = objDTO.PackSlipNumber,
                };


                if (objItemDTO.SerialNumberTracking)
                {
                    objResult = objInvCtrl.ItemLocationDetailSaveNewReceiveOrderForSerial(obj);
                }
                else
                {
                    lstLocationWiseDTO = new List<ItemLocationDetailsDTO>();
                    lstLocationWiseDTO.Add(obj);
                    long SessionUserId = SessionHelper.UserID;
                    //objResult = objInvCtrl.ItemLocationDetailSaveNewReceiveOrderForNotSerial(obj);
                    ItemLocationDetailsDAL itmLocDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    itmLocDAL.InsertItemLocationDetailsFromRecieve(lstLocationWiseDTO, SessionUserId, SessionHelper.EnterPriceID);

                    objOrdDetailDAL.UpdateOrderStatusByReceive(objDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);
                    objResult = Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK" }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                MaterialStagingDAL objDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                MaterialStagingDTO objStagingDTO = objDAL.GetRecord(objDTO.StagingID, SessionHelper.RoomID, SessionHelper.CompanyID);

                List<MaterialStagingPullDetailDTO> lstMSPULL = new List<MaterialStagingPullDetailDTO>();
                MaterialStagingPullDetailDTO obj = new MaterialStagingPullDetailDTO()
                {
                    OrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID,
                    BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByNameItemGuid(objDTO.ItemGUID, objDTO.ReceiveBinName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, true),
                    ConsignedQuantity = objItemDTO.Consignment ? objDTO.ReceivedQuantity : 0,
                    CustomerOwnedQuantity = objItemDTO.Consignment ? 0 : objDTO.ReceivedQuantity,
                    LotNumber = (!string.IsNullOrWhiteSpace(objDTO.ReceivedItemDetail.FirstOrDefault().LotNumber)) ? objDTO.ReceivedItemDetail.FirstOrDefault().LotNumber.Trim() : string.Empty,
                    SerialNumber = (!string.IsNullOrWhiteSpace(objDTO.ReceivedItemDetail.FirstOrDefault().SerialNumber)) ? objDTO.ReceivedItemDetail.FirstOrDefault().SerialNumber.Trim() : string.Empty,
                    MaterialStagingGUID = objStagingDTO.GUID,
                    BinNumber = objDTO.ReceiveBinName,
                    CompanyID = SessionHelper.CompanyID,
                    ItemCost = objDTO.ReceivedItemDetail.FirstOrDefault().Cost,
                    Created = DateTimeUtility.DateTimeNow,
                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    DateCodeTracking = objItemDTO.DateCodeTracking,
                    Expiration = objDTO.ReceivedItemDetail.FirstOrDefault().Expiration,
                    ID = 0,
                    IsArchived = false,
                    GUID = Guid.Empty,
                    IsDeleted = false,
                    ItemGUID = objDTO.ItemGUID,
                    ItemNumber = objItemDTO.ItemNumber,
                    LastUpdatedBy = SessionHelper.UserID,
                    LotNumberTracking = objItemDTO.LotNumberTracking,
                    Received = objDTO.ReceivedItemDetail.FirstOrDefault().Received,
                    Room = SessionHelper.RoomID,
                    RoomName = SessionHelper.RoomName,
                    SerialNumberTracking = objItemDTO.SerialNumberTracking,
                    Updated = DateTimeUtility.DateTimeNow,
                    UpdatedByName = SessionHelper.UserName,
                    UDF1 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF1,
                    UDF2 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF2,
                    UDF3 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF3,
                    UDF4 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF4,
                    UDF5 = objDTO.ReceivedItemDetail.FirstOrDefault().UDF5,
                    PackSlipNumber = objDTO.PackSlipNumber,
                };

                lstMSPULL.Add(obj);
                objResult = objInvCtrl.ItemLocationDetailsSaveForMSCredit(lstMSPULL);
            }
            objOrdDetailDTO = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailByOrderGUIDPlain(objOrdDetailDTO.FirstOrDefault().OrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ItemGUID == objDTO.ItemGUID);
            OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objOrdDTO = objOrdDAL.GetOrderByGuidPlain(objOrdDetailDTO.FirstOrDefault().OrderGUID.GetValueOrDefault(Guid.Empty));
            string OrdStatusText = objOrdDTO.OrderStatusText;
            return Json(new { Massage = "ok", Success = "ok", OrderGUID = objOrdDetailDTO.FirstOrDefault().OrderGUID, OrderDetailGUID = objOrdDetailDTO.FirstOrDefault().GUID, ReceivedQty = objOrdDetailDTO.FirstOrDefault().ReceivedQuantity, OrderStatusText = OrdStatusText }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// GetItemLocations
        /// </summary>
        /// <param name="OrderGuid"></param>
        /// <param name="ItemGuid"></param>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetItemLocations(Guid OrderGuid, Guid ItemGuid, string NameStartWith, bool? IsLoadMoreLocations = null)
        {
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            OrderMasterDTO objOrdDTO = null;
            if (OrderGuid != Guid.Empty)
            {
                objOrdDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByGuidPlain(OrderGuid);
            }
            if (objOrdDTO != null && objOrdDTO.StagingID.GetValueOrDefault(0) > 0)
            {
                MaterialStagingDAL objMSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                MaterialStagingDTO MsDTO = objMSDAL.GetRecord(objOrdDTO.StagingID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (MsDTO != null)
                {
                    if (MsDTO.BinID.GetValueOrDefault(0) <= 0)
                    {
                        MaterialStagingDetailDAL objMSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<MaterialStagingDetailDTO> MsDetailDTOList = objMSDetailDAL.GetHistoryRecordbyMaterialStagingID(MsDTO.GUID);

                        foreach (var item in MsDetailDTOList)
                        {
                            if (returnKeyValList.FindIndex(x => x.Value == item.StagingBinName) < 0)
                            {
                                DTOForAutoComplete obj = new DTOForAutoComplete()
                                {
                                    Key = item.StagingBinName,
                                    Value = item.StagingBinName,
                                    ID = item.ID,
                                    GUID = item.GUID,
                                };
                                returnKeyValList.Add(obj);
                            }
                        }

                    }
                    else
                    {
                        BinMasterDTO objBinDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(MsDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                        //BinMasterDTO objBinDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, MsDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                        DTOForAutoComplete obj = new DTOForAutoComplete()
                        {
                            Key = objBinDTO.BinNumber,
                            Value = objBinDTO.BinNumber,
                            ID = objBinDTO.ID,
                            GUID = objBinDTO.GUID,
                        };
                        returnKeyValList.Add(obj);
                    }
                }

                if (IsLoadMoreLocations.HasValue)
                {
                    if (IsLoadMoreLocations.Value)
                    {
                        //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), true).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber);
                        IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), true, string.Empty, false, null).OrderBy(x => x.BinNumber);
                        if (objBinDTOList != null && objBinDTOList.Count() > 0)
                        {
                            foreach (var item in objBinDTOList)
                            {
                                if (!returnKeyValList.Any(x => x.Key == item.BinNumber))
                                {
                                    DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                                    {
                                        GUID = Guid.Empty,
                                        ID = 0,
                                        Key = item.BinNumber,
                                        Value = item.BinNumber,
                                        OtherInfo1 = string.Empty,
                                    };
                                    returnKeyValList.Add(objAutoDTO);
                                }
                            }
                        }
                    }
                    else
                    {
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.GUID = Guid.Empty;
                        objAutoDTO.ID = 0;
                        objAutoDTO.Key = ResBin.MoreLocations;
                        objAutoDTO.Value = ResBin.MoreLocations;
                        returnKeyValList.Add(objAutoDTO);
                    }
                }

                if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    returnKeyValList = returnKeyValList.Where(x => x.Key.StartsWith(NameStartWith.ToLower())).ToList();
                }
            }
            else
            {
                IEnumerable<BinMasterDTO> objBinDTOList;
                if (IsLoadMoreLocations.HasValue)
                {
                    //objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid.ToString()).Where(x => !x.IsStagingLocation && !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber);
                    objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid.ToString(), false, string.Empty, false, null).OrderBy(x => x.BinNumber);
                    //objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid,0,null,false).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber);// !x.IsStagingLocation && 
                }
                else
                {
                    //objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber);
                    objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false, string.Empty, false, null).OrderBy(x => x.BinNumber);
                }

                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }

                    foreach (var item in objBinDTOList)
                    {
                        DTOForAutoComplete obj = new DTOForAutoComplete()
                        {
                            Key = item.BinNumber,
                            Value = item.BinNumber,
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
                        //IEnumerable<BinMasterDTO> oAllBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber);
                        IEnumerable<BinMasterDTO> oAllBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false, string.Empty, false, null).OrderBy(x => x.BinNumber);
                        if (oAllBinDTOList != null && oAllBinDTOList.Count() > 0)
                        {
                            if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                            {
                                oAllBinDTOList = oAllBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                            }

                            foreach (var item in oAllBinDTOList)
                            {
                                if (!returnKeyValList.Any(x => x.Key == item.BinNumber))
                                {
                                    DTOForAutoComplete obj = new DTOForAutoComplete()
                                    {
                                        Key = item.BinNumber,
                                        Value = item.BinNumber,
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
            }

            return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LoadReceiveOrders
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <returns></returns>
        public ActionResult LoadReceiveOrders(string ItemGUID)
        {
            //return Content("Amit Prajapati" + ItemID.ToString());
            ViewBag.ItemGUID = ItemGUID;
            List<SelectListItem> returnList = new List<SelectListItem>();
            returnList.Add(new SelectListItem() { Text = ResRoleMaster.All, Value = "0" });
            returnList.Add(new SelectListItem() { Text = ResReceiveOrderDetails.OpenOrders, Value = "1" });
            returnList.Add(new SelectListItem() { Text = ResReceiveOrderDetails.CloseOrders, Value = "2" });
            ViewBag.OrderStatus = returnList;
            OrderMasterDTO odt = new OrderMasterDTO();

            return PartialView("ReceiveOrderStatus");
        }

        /// <summary>
        /// OrderMasterListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ItemwiseOrderedMasterListAjax(JQueryDataTableParamModel param)
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

            int OrderStatus = int.Parse(Request["OrderStatus"].ToString());
            //Int64 ItemID = Int64.Parse(Request["ItemID"].ToString());
            Guid ItemGUID = Guid.Parse(Request["ItemGUID"].ToString());

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            //OrderMasterController controller = new OrderMasterController();
            OrderMasterDAL controller = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<OrderMasterDTO> DataFromDB = controller.GetOrderedPagedRecordsByItemGuidNormal(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID, OrderStatus, OrderType.Order);

            var result = from c in DataFromDB
                         select new OrderMasterDTO
                         {
                             ID = c.ID,
                             OrderNumber = c.OrderNumber,
                             ReleaseNumber = c.ReleaseNumber,
                             Comment = c.Comment,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             LastUpdated = c.LastUpdated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             CompanyID = c.CompanyID,
                             CreatedBy = c.CreatedBy,
                             RequiredDate = c.RequiredDate,
                             CustomerID = c.CustomerID,
                             CustomerName = c.CustomerName,
                             GUID = c.GUID,
                             LastUpdatedBy = c.LastUpdatedBy,
                             OrderStatus = c.OrderStatus,
                             OrderStatusText = c.OrderStatusText,
                             PackSlipNumber = c.PackSlipNumber,
                             Room = c.Room,
                             ShippingTrackNumber = c.ShippingTrackNumber,
                             ShipVia = c.ShipVia,
                             StagingName = c.StagingName,
                             StagingID = c.StagingID,
                             Supplier = c.Supplier,
                             RejectionReason = c.RejectionReason,
                             IsArchived = c.IsArchived,
                             IsDeleted = c.IsDeleted,
                             ShipViaName = c.ShipViaName,
                             SupplierName = c.SupplierName,
                             CustomerGUID = c.CustomerGUID,
                             AddedFrom = c.AddedFrom,
                             EditedFrom = c.EditedFrom,
                             ReceivedOn = c.ReceivedOn,
                             ReceivedOnWeb = c.ReceivedOnWeb
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB //result
            },
                        JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Quick List Receive

        [HttpPost]
        public JsonResult ReceiveQuickListItem(QLOrderHeader orderHeader)
        {
            JsonResult jsonResult = null;
            try
            {
                if (orderHeader.OrderNumber == null || orderHeader.OrderNumber.Trim() == "")
                {
                    return Json(new { Message = string.Format(ResCommon.MsgMissing, ResOrder.OrderNumber), Success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //----------------------Check For Order Number Duplication----------------------
                    //
                    string strOK = string.Empty;
                    //  RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,IsAllowOrderDuplicate";
                    RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                    if (roomDTO.IsAllowOrderDuplicate != true)
                    {
                        OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                        if (objOrderMasterDAL.IsOrderNumberDuplicateById(orderHeader.OrderNumber, 0, SessionHelper.RoomID, SessionHelper.CompanyID))
                        {
                            strOK = "duplicate";
                        }
                    }

                    if (strOK == "duplicate")
                    {
                        return Json(new { Message = string.Format(ResMessage.DuplicateMessage, ResOrder.OrderNumber, orderHeader.OrderNumber), Status = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //
                    //-------------------------------------------------------------------------------
                }

                if (orderHeader.OrderItemDetail != null && orderHeader.OrderItemDetail.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(orderHeader.RequiredDateStr))
                    {
                        orderHeader.RequiredDate = DateTime.ParseExact(orderHeader.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                    }
                    orderHeader = InsertOrderHeaderForQL(orderHeader);
                    if (orderHeader != null && orderHeader.OrderGuid != Guid.Empty)
                    {
                        orderHeader = InsertOrderItemsForQL(orderHeader);
                        jsonResult = ReceiveQLItems(orderHeader);
                    }

                    return Json(new { Message = ResReceiveOrderDetails.MsgReceivedSuccessfully, Success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Message = ResReceiveOrderDetails.NoItemToCreateOrder, Success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Success = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                jsonResult = null;
            }
        }

        /// <summary>
        /// Insert OrderHeader For QL
        /// </summary>
        /// <param name="orderHeader"></param>
        /// <returns></returns>
        private QLOrderHeader InsertOrderHeaderForQL(QLOrderHeader orderHeader)
        {
            OrderMasterDAL objOrderDAL = null;
            OrderMasterDTO objOrderDTO = null;
            try
            {
                long SessionUserId = SessionHelper.UserID;

                objOrderDTO = new OrderMasterDTO();

                objOrderDTO.OrderNumber = orderHeader.OrderNumber;
                objOrderDTO.OrderNumber_ForSorting = orderHeader.OrderNumber_ForSorting;
                objOrderDTO.Supplier = orderHeader.Supplier;
                objOrderDTO.Comment = orderHeader.Comment;
                objOrderDTO.RequiredDate = orderHeader.RequiredDate;
                objOrderDTO.OrderStatus = orderHeader.OrderStatus;
                objOrderDTO.ReleaseNumber = "1";
                objOrderDTO.WhatWhereAction = "Direct Quick list Receive";
                objOrderDTO.OrderDate = DateTimeUtility.DateTimeNow;
                objOrderDTO.Room = RoomID;
                objOrderDTO.CompanyID = CompanyID;
                objOrderDTO.CreatedBy = UserID;
                objOrderDTO.LastUpdatedBy = UserID;
                objOrderDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objOrderDTO.Created = DateTimeUtility.DateTimeNow;
                objOrderDTO.OrderType = (int)OrderType.Order;
                objOrderDTO.AddedFrom = "Web";
                objOrderDTO.EditedFrom = "Web";
                objOrderDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objOrderDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objOrderDTO.PackSlipNumber = orderHeader.PackSlipNumber;

                objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                if (objOrderDTO.OrderStatus == (int)OrderStatus.Submitted)
                {
                    objOrderDTO.RequesterID = UserID;
                }
                else if (objOrderDTO.OrderStatus == (int)OrderStatus.Approved)
                {
                    objOrderDTO.ApproverID = UserID;
                }
                objOrderDTO = objOrderDAL.InsertOrder(objOrderDTO, SessionUserId);
                orderHeader.OrderGuid = objOrderDTO.GUID;

                return orderHeader;
            }
            finally
            {
                objOrderDAL = null;
                objOrderDTO = null;
            }
        }

        /// <summary>
        /// Inser Order Items For QL
        /// </summary>
        /// <param name="OrderItemDetail"></param>
        /// <param name="OrderGuid"></param>
        /// <returns></returns>
        private QLOrderHeader InsertOrderItemsForQL(QLOrderHeader orderHeader)
        {
            OrderDetailsDAL objOrderDetailDAL = null;
            CommonDAL objCommonDAL = null;
            OrderDetailsDTO objOrderDetail = null;
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            try
            {
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                objOrderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                long SessionUserId = SessionHelper.UserID;
                BinMasterDTO objBin = new BinMasterDTO();
                foreach (var item in orderHeader.OrderItemDetail)
                {
                    ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                    objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidNormal(item.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //  item.ReceivedDate =   DateTime.ParseExact(item.ReceivedDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult); 
                    objBin = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID, item.BinName, RoomID, CompanyID, UserID, false);
                    objOrderDetail = new OrderDetailsDTO();
                    objOrderDetail.ID = 0;
                    objOrderDetail.RequestedQuantity = item.RequestedQuantity;
                    //objOrderDetail.Bin = objCommonDAL.GetOrInsertBinIDByName(item.ItemGUID, item.BinName, UserID, RoomID, CompanyID);
                    objOrderDetail.Bin = objBin.ID;
                    objOrderDetail.BinName = objBin.BinNumber;
                    objOrderDetail.ApprovedQuantity = item.ApprovedQuantity;
                    objOrderDetail.CompanyID = CompanyID;
                    objOrderDetail.Room = RoomID;
                    objOrderDetail.Created = DateTimeUtility.DateTimeNow;
                    objOrderDetail.LastUpdated = DateTimeUtility.DateTimeNow;
                    objOrderDetail.CreatedBy = UserID;
                    objOrderDetail.LastUpdatedBy = UserID;
                    objOrderDetail.OrderGUID = orderHeader.OrderGuid;
                    objOrderDetail.ItemGUID = item.ItemGUID;
                    objOrderDetail.AddedFrom = "Web";
                    objOrderDetail.EditedFrom = "Web";
                    objOrderDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objOrderDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    #region WI-6215 and Other Relevant order cost related jira
                    objOrderDetail.ItemCost = item.Cost;
                    if (objItemMasterDTO != null && objItemMasterDTO.ID > 0)
                    {
                        objOrderDetail.ItemCostUOMValue = objItemMasterDTO.CostUOMValue.GetValueOrDefault(1);
                        objOrderDetail.ItemMarkup = objItemMasterDTO.CostUOMValue.GetValueOrDefault(1);
                    }
                    if (objOrderDetail.ItemCost.GetValueOrDefault(0) > 0)
                    {
                        if (objOrderDetail.ItemMarkup > 0)
                        {
                            objOrderDetail.ItemSellPrice = objOrderDetail.ItemCost + ((objOrderDetail.ItemCost * objOrderDetail.ItemMarkup) / 100);
                        }
                        else
                        {
                            objOrderDetail.ItemSellPrice = objOrderDetail.ItemCost;
                        }
                    }
                    else
                    {
                        objOrderDetail.ItemSellPrice = 0;
                    }
                    if (objOrderDetail.ItemCostUOMValue == null
                       || objOrderDetail.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                    {
                        objOrderDetail.ItemCostUOMValue = 1;
                    }

                    objOrderDetail.OrderLineItemExtendedCost = double.Parse(Convert.ToString((orderHeader.OrderStatus <= 2 ? (objOrderDetail.RequestedQuantity.GetValueOrDefault(0) * (objOrderDetail.ItemCost.GetValueOrDefault(0) / objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                               : (objOrderDetail.ApprovedQuantity.GetValueOrDefault(0) * (objOrderDetail.ItemCost.GetValueOrDefault(0) / objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1))))));

                    objOrderDetail.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((orderHeader.OrderStatus <= 2 ? (objOrderDetail.RequestedQuantity.GetValueOrDefault(0) * (objOrderDetail.ItemSellPrice.GetValueOrDefault(0) / objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                : (objOrderDetail.ApprovedQuantity.GetValueOrDefault(0) * (objOrderDetail.ItemSellPrice.GetValueOrDefault(0) / objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1))))));
                    #endregion
                    objOrderDetail = objOrderDetailDAL.Insert(objOrderDetail, SessionUserId, SessionHelper.EnterPriceID);

                    item.OrderDetailGuid = objOrderDetail.GUID;
                    item.OrderGUID = orderHeader.OrderGuid;
                    item.BinID = objOrderDetail.Bin.GetValueOrDefault(0);

                }

                return orderHeader;
            }
            finally
            {
                objOrderDetailDAL = null;
                objCommonDAL = null;
                objOrderDetail = null;
            }
        }

        /// <summary>
        /// Receive QL Items
        /// </summary>
        /// <param name="orderHeader"></param>
        /// <returns></returns>
        private JsonResult ReceiveQLItems(QLOrderHeader orderHeader)
        {
            //List<ItemLocationDetailsDTO> objData = null;
            //ItemLocationDetailsDTO objItemLoc = null;
            //InventoryController objInvCtrl = null;
            List<ReceivedOrderTransferDetailDTO> objData = null;
            ReceivedOrderTransferDetailDTO objItemLoc = null;

            ReceivedOrderTransferDetailDAL objInvCtrl = null;

            BinMasterDAL objBinDAL = null;
            BinMasterDTO objBinDTO = null;
            UDFDAL objUDFApiController = null;
            List<UDFDTO> DataFromDB = null;
            var enterpriseId = SessionHelper.EnterPriceID;

            try
            {
                objData = new List<ReceivedOrderTransferDetailDTO>();
                objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);

                string udfRequier = string.Empty;

                foreach (var item in orderHeader.OrderItemDetail)
                {
                    if (!item.ReceivedOrderDetail.LotNumberTracking && !item.ReceivedOrderDetail.SerialNumberTracking && !item.ReceivedOrderDetail.DateCodeTracking)
                    {
                        if ((item.ReceivedOrderDetail.ConsignedQuantity.GetValueOrDefault(0) + item.ReceivedOrderDetail.CustomerOwnedQuantity.GetValueOrDefault(0)) > 0)
                        {
                            foreach (var i in DataFromDB)
                            {
                                if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(item.ReceivedOrderDetail.UDF1))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }
                                else if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(item.ReceivedOrderDetail.UDF2))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }
                                else if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(item.ReceivedOrderDetail.UDF3))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }
                                else if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(item.ReceivedOrderDetail.UDF4))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }
                                else if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(item.ReceivedOrderDetail.UDF5))
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
                                break;

                            objBinDTO = objBinDAL.GetBinByID(item.BinID, RoomID, CompanyID);
                            //objBinDTO = objBinDAL.GetItemLocation(RoomID, CompanyID, false, false,Guid.Empty, item.BinID, null,null).FirstOrDefault();
                            int eVMISensorPortInt = 0;
                            int.TryParse(objBinDTO.eVMISensorPort, out eVMISensorPortInt);
                            int? eVMISensorPortIntNull = null;
                            if (eVMISensorPortInt > 0)
                                eVMISensorPortIntNull = eVMISensorPortInt;

                            string streVMISensorID = string.Empty;
                            if (objBinDTO.eVMISensorID.GetValueOrDefault(0) > 0)
                                streVMISensorID = objBinDTO.eVMISensorID.GetValueOrDefault(0).ToString();

                            objItemLoc = new ReceivedOrderTransferDetailDTO();

                            DateTime currentDateAsPerRoom = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
                            DateTime newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, item.ReceivedOrderDetail.ReceivedDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, SessionHelper.CurrentTimeZone, SessionHelper.RoomTimeFormat);

                            objItemLoc.BinID = item.BinID;
                            objItemLoc.BinNumber = item.BinName;
                            objItemLoc.ItemGUID = item.ItemGUID;
                            objItemLoc.OrderDetailGUID = item.OrderDetailGuid;
                            objItemLoc.CustomerOwnedQuantity = item.ReceivedOrderDetail.CustomerOwnedQuantity;
                            objItemLoc.ConsignedQuantity = item.ReceivedOrderDetail.ConsignedQuantity;
                            objItemLoc.Cost = item.ReceivedOrderDetail.Cost;
                            objItemLoc.Received = newReceiveTempDate.ToString(SessionHelper.RoomDateFormat);
                            objItemLoc.ReceivedDate = newReceiveTempDate;//DateTime.ParseExact(item.ReceivedOrderDetail.ReceivedDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                            objItemLoc.PackSlipNumber = item.ReceivedOrderDetail.PackSlipNumber;
                            objItemLoc.UDF1 = item.ReceivedOrderDetail.UDF1;
                            objItemLoc.UDF2 = item.ReceivedOrderDetail.UDF2;
                            objItemLoc.UDF3 = item.ReceivedOrderDetail.UDF3;
                            objItemLoc.UDF4 = item.ReceivedOrderDetail.UDF4;
                            objItemLoc.UDF5 = item.ReceivedOrderDetail.UDF5;
                            objItemLoc.Updated = DateTimeUtility.DateTimeNow; objItemLoc.Created = DateTimeUtility.DateTimeNow;
                            objItemLoc.CreatedBy = UserID; objItemLoc.LastUpdatedBy = UserID;
                            objItemLoc.Room = RoomID; objItemLoc.CompanyID = CompanyID;
                            objItemLoc.ItemType = item.ReceivedOrderDetail.ItemType;
                            objItemLoc.LotNumberTracking = item.ReceivedOrderDetail.LotNumberTracking;
                            objItemLoc.DateCodeTracking = item.ReceivedOrderDetail.DateCodeTracking;
                            objItemLoc.SerialNumberTracking = item.ReceivedOrderDetail.SerialNumberTracking;
                            objItemLoc.eVMISensorID = streVMISensorID;
                            //objItemLoc.eVMISensorIDdbl = objBinDTO.eVMISensorID;
                            // objItemLoc.eVMISensorPort = eVMISensorPortIntNull;
                            //objItemLoc.eVMISensorPortstr = objBinDTO.eVMISensorPort;
                            objItemLoc.MinimumQuantity = objBinDTO.MinimumQuantity;
                            objItemLoc.MaximumQuantity = objBinDTO.MaximumQuantity;
                            objItemLoc.CriticalQuantity = objBinDTO.CriticalQuantity;
                            //objItemLoc.SuggestedOrderQuantity = objBinDTO.SuggestedOrderQuantity;
                            objItemLoc.IsDeleted = false; objItemLoc.IsArchived = false;
                            objItemLoc.EditedFrom = "Web";
                            objItemLoc.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objItemLoc.GUID = Guid.Empty; objItemLoc.ID = 0; objItemLoc.HistoryID = 0;

                            objData.Add(objItemLoc);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(udfRequier))
                {
                    return Json(new { Message = udfRequier, Status = "UDFError" }, JsonRequestBehavior.AllowGet);
                }

                if (objData != null && objData.Count > 0)
                {
                    //objInvCtrl = new InventoryController();
                    //objJsonR = objInvCtrl.ItemLocationDetailsSaveOrder(objData);
                    List<Guid> lstOrderGuid = new List<Guid>();
                    objInvCtrl = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);



                    string receiveGUIDs = string.Empty;
                    long SessionUserId = SessionHelper.UserID;
                    foreach (var item in objData)
                    {
                        item.OrderStatus = orderHeader.OrderStatus;
                        objInvCtrl.InsertReceive(item, SessionUserId, true, enterpriseId);
                        receiveGUIDs += "," + Convert.ToString(item.GUID);

                    }
                    OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetOrderDetailByGuidPlain(objData.FirstOrDefault().OrderDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    OrderStatus ordStatus = ordDetailCtrl.UpdateOrderStatusByReceiveNew(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);

                    try
                    {
                        string dataGUIDs = "<DataGuids>" + receiveGUIDs + "</DataGuids>";
                        string eventName = "ORECC";
                        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                        NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                        List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        if (lstNotification != null && lstNotification.Count > 0)
                        {
                            objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                        }
                    }
                    catch (Exception ex)
                    {

                        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                    }

                }
                //return objJsonR;

                return Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK", OrderStatusText = "" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objData = null;
                objItemLoc = null;
                objInvCtrl = null;
                objBinDAL = null;
                objBinDTO = null;
            }
        }

        #endregion


        #region Edit Receive
        /// <summary>
        /// Open Edit Receive Dailog
        /// </summary>
        /// <param name="ReceivedGUID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OpenEditReceiveDialog(Guid ReceivedGUID)
        {
            ReceivedOrderTransferDetailDAL objRecdOrdTrnDtlDAL = null;
            ReceivedOrderTransferDetailDTO objRecdOrdTrnDtlDTO = null;
            try
            {
                objRecdOrdTrnDtlDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                objRecdOrdTrnDtlDTO = objRecdOrdTrnDtlDAL.GetROTDByGuidFull(ReceivedGUID, RoomID, CompanyID);

                //OrderDetailsDTO objOrderDTO = new eTurns.DAL.OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailByGuidPlain(objRecdOrdTrnDtlDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                ItemLocationDetailsDTO objItemLocationDetail = new eTurns.DAL.ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(objRecdOrdTrnDtlDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                bool isChanged = false;
                if (objItemLocationDetail == null)
                {
                    objItemLocationDetail = new ItemLocationDetailsDTO()
                    {
                        BinID = objRecdOrdTrnDtlDTO.BinID,
                        BinNumber = objRecdOrdTrnDtlDTO.BinNumber,
                        CompanyID = objRecdOrdTrnDtlDTO.CompanyID,
                        ConsignedQuantity = objRecdOrdTrnDtlDTO.ConsignedQuantity,
                        CustomerOwnedQuantity = objRecdOrdTrnDtlDTO.CustomerOwnedQuantity,
                        ItemGUID = objRecdOrdTrnDtlDTO.ItemGUID,
                        ItemNumber = objRecdOrdTrnDtlDTO.ItemNumber,
                    };
                    isChanged = true;
                }
                else if (objItemLocationDetail.CustomerOwnedQuantity.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.CustomerOwnedQuantity.GetValueOrDefault(0))
                {
                    isChanged = true;
                }
                else if (objItemLocationDetail.ConsignedQuantity.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.ConsignedQuantity.GetValueOrDefault(0))
                {
                    isChanged = true;
                }
                else if (objItemLocationDetail.Cost.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.Cost.GetValueOrDefault(0))
                {
                    isChanged = true;
                }
                else if (objItemLocationDetail.DateCodeTracking && objItemLocationDetail.ExpirationDate != objRecdOrdTrnDtlDTO.ExpirationDate)
                {
                    isChanged = true;
                }
                else if (objItemLocationDetail.ReceivedDate != objRecdOrdTrnDtlDTO.ReceivedDate)
                {
                    isChanged = true;
                }
                else if (objItemLocationDetail.SerialNumberTracking && objItemLocationDetail.SerialNumber != objRecdOrdTrnDtlDTO.SerialNumber)
                {
                    isChanged = true;
                }
                else if (objItemLocationDetail.LotNumberTracking && objItemLocationDetail.LotNumber != objRecdOrdTrnDtlDTO.LotNumber)
                {
                    isChanged = true;
                }
                ViewBag.IsChanged = isChanged;
                ViewBag.OrderGuid = objRecdOrdTrnDtlDTO.OrderGuid;

                objRecdOrdTrnDtlDTO.IsOnlyFromUI = true;
                return PartialView("_EditReceivedItem", objRecdOrdTrnDtlDTO);
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

        /// <summary>
        /// Open Edit Receive Dailog
        /// </summary>
        /// <param name="ReceivedGUID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OpenDialogForEditReceipt(Guid ItemGuid, Guid OrderDetailGuid)
        {
            ReceivedOrderTransferDetailDAL objRecdOrdTrnDtlDAL = null;
            List<ReceivedOrderTransferDetailDTO> objRecdOrdTrnDtlDTO = null;
            List<ReceivedOrderTransferDetailDTO> objFilteredRecdOrdTrnDtlDTO = new List<ReceivedOrderTransferDetailDTO>();
            OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            try
            {
                objRecdOrdTrnDtlDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                objRecdOrdTrnDtlDTO = objRecdOrdTrnDtlDAL.GetROTDByOrderDetailGUIDFull(OrderDetailGuid, RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
                ViewBag.ItemGuid = ItemGuid;
                ViewBag.OrderDetailGuid = OrderDetailGuid;
                OrderDetailsDTO objOrderDetailDTO = new eTurns.DAL.OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailByGuidFull(OrderDetailGuid, RoomID, CompanyID);
                ViewBag.OrderGuid = objOrderDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty);
                ItemMasterDTO objItemMasterDTO = new eTurns.DAL.ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidPlain(ItemGuid, RoomID, CompanyID);

                if (objRecdOrdTrnDtlDTO != null && objRecdOrdTrnDtlDTO.Count > 0)
                {
                    for (int i = 0; i < objRecdOrdTrnDtlDTO.Count; i++)
                    {
                        if (objRecdOrdTrnDtlDTO[i].ItemLocationDetailGUID != null)
                        {
                            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                            ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetRecordsByItemLocationDetailGuid(objRecdOrdTrnDtlDTO[i].ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.InitialQuantity > 0)
                            {
                                if (objOrderDetailDTO.OrderUOMValue == null || objOrderDetailDTO.OrderUOMValue <= 0)
                                    objOrderDetailDTO.OrderUOMValue = 1;

                                if (objItemMasterDTO != null && objItemMasterDTO.SerialNumberTracking == false && objItemMasterDTO.IsAllowOrderCostuom && objOrderDetailDTO.OrderType == (int)OrderType.Order)
                                {
                                    if (objRecdOrdTrnDtlDTO[i].ConsignedQuantity != null && objRecdOrdTrnDtlDTO[i].ConsignedQuantity >= 0)
                                        objRecdOrdTrnDtlDTO[i].ConsignedQuantity = objRecdOrdTrnDtlDTO[i].ConsignedQuantity / objOrderDetailDTO.OrderUOMValue;

                                    if (objRecdOrdTrnDtlDTO[i].CustomerOwnedQuantity != null && objRecdOrdTrnDtlDTO[i].CustomerOwnedQuantity >= 0)
                                        objRecdOrdTrnDtlDTO[i].CustomerOwnedQuantity = objRecdOrdTrnDtlDTO[i].CustomerOwnedQuantity / objOrderDetailDTO.OrderUOMValue;
                                }
                                objFilteredRecdOrdTrnDtlDTO.Add(objRecdOrdTrnDtlDTO[i]);
                            }
                        }
                    }
                }
                objRecdOrdTrnDtlDTO = new List<ReceivedOrderTransferDetailDTO>();
                objRecdOrdTrnDtlDTO.AddRange(objFilteredRecdOrdTrnDtlDTO);

                return PartialView("_EditReceipt", objRecdOrdTrnDtlDTO);
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



        /// <summary>
        /// Edit Received Record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditReceivedRecord(ReceivedOrderTransferDetailDTO objDTO)
        {
            CommonDAL objCommonDAL = null;
            ReceivedOrderTransferDetailDAL objROTDDAL = null;
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            bool isSuccess = true;
            string returnMsg = ResCommon.MsgUpdatedSuccessfully;
            long SessionUserId = SessionHelper.UserID;
            try
            {
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                if (!string.IsNullOrEmpty(objDTO.SerialNumber) && objDTO.SerialNumberTracking)
                {

                    string returnSerialMessage = objCommonDAL.CheckDuplicateSerialNumbers(objDTO.SerialNumber.Trim(), objDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, objDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                    if (returnSerialMessage == "duplicate")
                    {
                        return Json(new { Status = false, Message = string.Format(ResReceiveOrderDetails.IsDuplicate, objDTO.SerialNumber) }, JsonRequestBehavior.AllowGet);
                    }
                }
                BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(objDTO.ItemGUID ?? Guid.Empty, objDTO.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                objDTO.BinID = objBinMasterDTO.ID;
                //objDTO.BinID = objCommonDAL.GetOrInsertBinIDByName(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinNumber, UserID, RoomID, CompanyID);
                objDTO.CompanyID = CompanyID;
                objDTO.Room = RoomID;
                objDTO.LastUpdatedBy = UserID;
                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                if (objDTO.IsOnlyFromUI)
                {
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                List<ReceivedOrderTransferDetailDTO> objListRecOrder = new List<ReceivedOrderTransferDetailDTO>();
                objListRecOrder.Add(objDTO);
                objROTDDAL.Edit(objListRecOrder, SessionUserId, SessionHelper.EnterPriceID);

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

        /// <summary>
        /// Edit Receipt Records
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditReceiptRecords(List<ReceivedOrderTransferDetailDTO> reciepts)
        {
            int OrdStatus = 0;
            CommonDAL objCommonDAL = null;
            ReceivedOrderTransferDetailDAL objROTDDAL = null;
            OrderDetailsDAL objOrderDetailDAL = null;
            OrderMasterDAL objOrderDAL = null;
            OrderDetailsDTO objOrderDetailDTO = null;
            OrderMasterDTO objOrderDTO = null;
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL cmnDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            bool isSuccess = true;
            string returnMsg = ResCommon.MsgUpdatedSuccessfully;
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            try
            {
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                objOrderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

                string strDuplicateSerials = string.Empty;

                DateTime currentDateAsPerRoom = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);

                foreach (var item in reciepts)
                {


                    if (!string.IsNullOrEmpty(item.SerialNumber) && item.SerialNumberTracking)
                    {
                        string returnSerialMessage = objCommonDAL.CheckDuplicateSerialNumbers(item.SerialNumber.Trim(), item.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, item.ItemGUID.GetValueOrDefault(Guid.Empty));
                        if (returnSerialMessage == "duplicate")
                        {
                            strDuplicateSerials = strDuplicateSerials + "<br/>" + string.Format(ResReceiveOrderDetails.IsDuplicate, item.SerialNumber);
                        }

                    }
                    else if (string.IsNullOrWhiteSpace(item.BinNumber))
                    {
                        strDuplicateSerials = strDuplicateSerials + "<br/>" + ResReceiveOrderDetails.BinCantEmptySelectIt;
                    }

                    if (item.LotNumberTracking && item.DateCodeTracking
                        && !string.IsNullOrWhiteSpace(item.LotNumber)
                        && !string.IsNullOrWhiteSpace(item.strExpirationDate))
                    {
                        DateTime dtExpDate;
                        //string _expDate = (item.strExpirationDate.GetValueOrDefault(DateTime.MinValue)).ToString(SessionHelper.RoomDateFormat);
                        DateTime.TryParseExact(item.strExpirationDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtExpDate);
                        string Expiration = dtExpDate.ToString("MM/dd/yyyy");
                        string msg = cmnDAL.CheckDuplicateLotAndExpiration(item.LotNumber, Expiration, dtExpDate, 0, RoomID, CompanyID, item.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.UserID, SessionHelper.EnterPriceID);
                        if (string.IsNullOrWhiteSpace(msg) || (msg ?? string.Empty).ToLower() == "ok")
                        {

                        }
                        else
                        {
                            strDuplicateSerials = strDuplicateSerials + "<br/> " + msg;
                        }
                    }


                    objOrderDetailDTO = objOrderDetailDAL.GetOrderDetailByGuidNormal(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                    // objOrderDTO = objOrderDAL.GetOrderByGuidPlain(objOrderDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));
                    OrdStatus = objOrderDetailDTO.OrderStatus;
                    bool IsStagelocation = false;
                    if (objOrderDetailDTO.StagingID.GetValueOrDefault(0) > 0 || objOrderDetailDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        IsStagelocation = true;
                    }

                    if (string.IsNullOrEmpty(strDuplicateSerials))
                    {
                        BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID ?? Guid.Empty, item.BinNumber, RoomID, CompanyID, UserID, IsStagelocation);
                        item.BinID = objBinMasterDTO.ID;
                        item.CompanyID = CompanyID;
                        item.Room = RoomID;
                        item.LastUpdatedBy = UserID;
                        item.Updated = DateTimeUtility.DateTimeNow;

                        if (item.IsOnlyFromUI)
                        {
                            if (objOrderDetailDTO.OrderType == (int)OrderType.RuturnOrder)
                            {
                                item.EditedFrom = "Web>>EditReceipt>>ReturnOrder";
                            }
                            else
                            {
                                item.EditedFrom = "Web>>EditReceipt>>Order";
                            }

                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(item.strReceivedDate))
                    {



                        DateTime _newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, item.strReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, SessionHelper.CurrentTimeZone, SessionHelper.RoomTimeFormat);


                        //item.ReceivedDate = DateTime.ParseExact(item.strReceivedDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
                        item.ReceivedDate = _newReceiveTempDate; //DateTime.ParseExact(item.strReceivedDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                    }
                    if (!string.IsNullOrEmpty(item.strExpirationDate) && item.DateCodeTracking)
                    {
                        //item.ExpirationDate = DateTime.ParseExact(item.strExpirationDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
                        item.ExpirationDate = DateTime.ParseExact(item.strExpirationDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                    }
                }

                if (!string.IsNullOrEmpty(strDuplicateSerials))
                {
                    return Json(new { Status = false, Message = strDuplicateSerials, OrderStatus = OrdStatus }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    objROTDDAL.Edit(reciepts, SessionUserId, enterpriseId);
                    //if(reciepts != null && reciepts.Count > 0)
                    //{
                    //    QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                    //    objQBItemDAL.InsertQuickBookItem(reciepts[0].ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, "Update", false, SessionHelper.UserID, "Web", null, reciepts[0].EditedFrom ?? "EditReceipt >> Order");
                    //}

                    try
                    {
                        string receiveGUIDs = string.Join(",", reciepts.Select(x => x.GUID));
                        string dataGUIDs = "<DataGuids>" + receiveGUIDs + "</DataGuids>";
                        string eventName = "ORECE";
                        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                        NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                        List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        if (lstNotification != null && lstNotification.Count > 0)
                        {
                            objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                        }
                    }
                    catch (Exception ex)
                    {

                        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                    }

                }
                objOrderDTO = objOrderDAL.GetOrderByGuidPlain(objOrderDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));
                OrdStatus = objOrderDTO.OrderStatus;

                return Json(new { Status = isSuccess, Message = returnMsg, OrderStatus = OrdStatus }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Status = false, Message = ResCommon.ErrorColon + " " + ex.Message, OrderStatus = OrdStatus }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objCommonDAL = null;
                objROTDDAL = null;
            }

        }


        /// <summary>
        /// Pre Receive Info
        /// </summary>
        /// <param name="OrderDetailGuid"></param>
        /// <param name="ItemGuid"></param>
        /// <returns></returns>
        public ActionResult FillPreReceiveInformation(ToFillPreReceiveDTO MakePreReceive)
        {
            OrderDetailsDAL orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

            PreReceivOrderDetailDAL PreReciveDAL = null;
            List<PreReceivOrderDetailDTO> preReceveInfo = null;
            List<ToFillReceiveDetailDTO> lstToFillRcvDtl = null;
            ToFillReceiveDetailDTO detailDTO = null;
            try
            {

                lstToFillRcvDtl = new List<ToFillReceiveDetailDTO>();
                PreReciveDAL = new PreReceivOrderDetailDAL(SessionHelper.EnterPriseDBName);
                preReceveInfo = PreReciveDAL.GetAllRecordsByOrderDetailItem(MakePreReceive.OrderDetailGUID, MakePreReceive.ItemGUID).ToList();
                ViewBag.ItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, MakePreReceive.ItemGUID);
                if (preReceveInfo != null && preReceveInfo.Count > 0)
                {

                    foreach (var item in preReceveInfo)
                    {
                        detailDTO = GetToFillReceiveDetailList(item, MakePreReceive);
                        if (detailDTO.Quantity > 0)
                            lstToFillRcvDtl.Add(detailDTO);
                    }

                    if (lstToFillRcvDtl.Count > 0)
                        MakePreReceive.IsModelShow = true;

                    //MakePreReceive.MakePreReceiveDetail = lstToFillRcvDtl;
                }

                detailDTO = GetToFillReceiveDetailList(new PreReceivOrderDetailDTO(), MakePreReceive);
                if (!MakePreReceive.IsModelShow && lstToFillRcvDtl.Count <= 0)
                    preReceveInfo = new List<PreReceivOrderDetailDTO>();

                if (!MakePreReceive.IsModelShow && (MakePreReceive.LotNumberTracking || MakePreReceive.SerialNumberTracking || MakePreReceive.DateCodeTracking))
                {
                    MakePreReceive.IsModelShow = true;
                }
                else if (preReceveInfo.Count <= 0 && MakePreReceive.MakePreReceiveDetail != null && MakePreReceive.MakePreReceiveDetail.Count > 0)
                {
                    detailDTO.Quantity = MakePreReceive.MakePreReceiveDetail[0].Quantity;
                }

                lstToFillRcvDtl.Add(detailDTO);

                List<PreReceivOrderDetailDTO> orderDetailTrackingList = orderDetailDAL.GetOrderDetailTrackingByGuidPlain(MakePreReceive.OrderDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (orderDetailTrackingList.Count > 1)
                {
                    MakePreReceive.IsModelShow = true;
                    for (int i = 0; i < orderDetailTrackingList.Count; i++)
                    {
                        double QuantityToReceive = (MakePreReceive.MakePreReceiveDetail[0].QuantityToReceive <= 0 ? MakePreReceive.MakePreReceiveDetail[0].Quantity : MakePreReceive.MakePreReceiveDetail[0].QuantityToReceive);
                        if (i < QuantityToReceive)
                        {
                            if (i < lstToFillRcvDtl.Count)
                            {
                                lstToFillRcvDtl[i].PackSlipNumber = orderDetailTrackingList[i].PackSlipNumber;
                                lstToFillRcvDtl[i].Quantity = orderDetailTrackingList[i].Quantity.HasValue ? orderDetailTrackingList[i].Quantity.Value : lstToFillRcvDtl[i].Quantity;
                                lstToFillRcvDtl[i].ID = orderDetailTrackingList[i].ID;
                            }
                            else
                            {
                                ToFillReceiveDetailDTO lstOfItemData = new ToFillReceiveDetailDTO(); // Replace YourItemType with the actual type of items in lstToFillRcvDtl
                                lstOfItemData.PackSlipNumber = orderDetailTrackingList[i].PackSlipNumber;
                                lstOfItemData.Quantity = orderDetailTrackingList[i].Quantity.HasValue ? orderDetailTrackingList[i].Quantity.Value : 0; // You may need to adjust this line depending on the type of Quantity
                                lstOfItemData.ID = orderDetailTrackingList[i].ID;

                                lstToFillRcvDtl.Add(lstOfItemData);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    MakePreReceive.MakePreReceiveDetail = lstToFillRcvDtl;
                    return PartialView("PreRecieveInfoForNormalItem", MakePreReceive);
                }
                else
                {
                    MakePreReceive.MakePreReceiveDetail = lstToFillRcvDtl;
                    return PartialView("PreRecieveInfo", MakePreReceive);
                }
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


        /// <summary>
        /// Pre Receive Info For Serial Item only
        /// </summary>
        /// <param name="OrderDetailGuid"></param>
        /// <param name="ItemGuid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FillPreReceiveInfoForSerialItem(ToFillPreReceiveDTO MakePreReceive)
        {
            OrderDetailsDAL orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();

            PreReceivOrderDetailDAL PreReciveDAL = null;
            List<PreReceivOrderDetailDTO> preReceveInfo = null;
            List<ToFillReceiveDetailDTO> lstToFillRcvDtl = null;
            ToFillReceiveDetailDTO detailDTO = null;
            try
            {
                ItemMasterDTO objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidNormal(MakePreReceive.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objItemMasterDTO.OrderUOMValue == null || objItemMasterDTO.OrderUOMValue <= 0)
                    objItemMasterDTO.OrderUOMValue = 1;

                if (objItemMasterDTO.IsAllowOrderCostuom)
                {
                    MakePreReceive.MakePreReceiveDetail[0].Quantity = MakePreReceive.MakePreReceiveDetail[0].Quantity * objItemMasterDTO.OrderUOMValue.GetValueOrDefault(0);
                }
                lstToFillRcvDtl = new List<ToFillReceiveDetailDTO>();
                PreReciveDAL = new PreReceivOrderDetailDAL(SessionHelper.EnterPriseDBName);
                preReceveInfo = PreReciveDAL.GetAllRecordsByOrderDetailItem(MakePreReceive.OrderDetailGUID, MakePreReceive.ItemGUID).ToList();
                ViewBag.ItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, MakePreReceive.ItemGUID);
                if (preReceveInfo != null && preReceveInfo.Count > 0)
                {
                    foreach (var item in preReceveInfo)
                    {
                        detailDTO = GetToFillReceiveDetailList(item, MakePreReceive);
                        lstToFillRcvDtl.Add(detailDTO);
                    }
                }

                double qty = MakePreReceive.MakePreReceiveDetail[0].Quantity;
                //ItemMasterDTO objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemsWithJoinsByGUIDs(Convert.ToString(MakePreReceive.ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID).FirstOrDefault();
                //if (objItemMasterDTO.OrderUOMValue == null || objItemMasterDTO.OrderUOMValue <= 0)
                //    objItemMasterDTO.OrderUOMValue = 1;

                //qty = qty * objItemMasterDTO.OrderUOMValue.GetValueOrDefault(0);
                if (qty >= lstToFillRcvDtl.Count && preReceveInfo != null && preReceveInfo.Count > 0)
                {
                    qty = qty - lstToFillRcvDtl.Count;

                    for (int i = 0; i < int.Parse(qty.ToString()); i++)
                    {
                        detailDTO = GetToFillReceiveDetailList(new PreReceivOrderDetailDTO(), MakePreReceive);
                        lstToFillRcvDtl.Add(detailDTO);
                    }

                }
                else if (qty > lstToFillRcvDtl.Count && (preReceveInfo == null || preReceveInfo.Count <= 0))
                {
                    qty = qty - lstToFillRcvDtl.Count;

                    for (int i = 0; i < int.Parse(qty.ToString()); i++)
                    {
                        detailDTO = GetToFillReceiveDetailList(new PreReceivOrderDetailDTO(), MakePreReceive);
                        lstToFillRcvDtl.Add(detailDTO);
                    }

                }
                else
                {
                    //detailDTO = GetToFillReceiveDetailList(new PreReceivOrderDetailDTO(), MakePreReceive);
                    //lstToFillRcvDtl.Add(detailDTO);
                    qty = 0;
                    if (SiteSettingHelper.IsBorderStateURL == "1" && SiteSettingHelper.EnableOktaLoginForSpecialUrls.Contains(eTurnsWeb.Helper.SessionHelper.CurrentDomainURL))
                    {
                        qty = (double)(MakePreReceive.InTransitQuantity ?? 0);
                    }
                    else
                    {
                        if (MakePreReceive.InTransitQuantity > 0)
                        {
                            qty = (double)MakePreReceive.InTransitQuantity;
                        }
                        else
                        {
                            if (MakePreReceive.ApproveQty.GetValueOrDefault(0) > 0)
                            {
                                qty = MakePreReceive.ApproveQty.GetValueOrDefault(0) - MakePreReceive.ReceiveQty.GetValueOrDefault(0);
                            }
                            else
                            {
                                qty = MakePreReceive.RequestedQty.GetValueOrDefault(0) - MakePreReceive.ReceiveQty.GetValueOrDefault(0);
                            }
                        }

                    }

                    if (qty > 0)
                    {
                        if (objItemMasterDTO.IsAllowOrderCostuom)
                        {
                            qty = qty * objItemMasterDTO.OrderUOMValue.GetValueOrDefault(0);
                            if (lstToFillRcvDtl != null && lstToFillRcvDtl.Count > 0 && preReceveInfo != null && preReceveInfo.Count > 0)
                                qty = qty - lstToFillRcvDtl.Count;
                        }
                        for (int i = 0; i < qty; i++)
                        {
                            detailDTO = GetToFillReceiveDetailList(new PreReceivOrderDetailDTO(), MakePreReceive);
                            lstToFillRcvDtl.Add(detailDTO);
                        }
                    }
                    else
                    {
                        detailDTO = GetToFillReceiveDetailList(new PreReceivOrderDetailDTO(), MakePreReceive);
                        lstToFillRcvDtl.Add(detailDTO);
                    }
                }

                List<PreReceivOrderDetailDTO> orderDetailTrackingList = orderDetailDAL.GetOrderDetailTrackingByGuidPlain(MakePreReceive.OrderDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                if (orderDetailTrackingList != null && orderDetailTrackingList.Count > 0)
                {
                    for (int i = 0; i < orderDetailTrackingList.Count; i++)
                    {
                        if (i < lstToFillRcvDtl.Count) // Ensure both lists have elements at index i
                        {
                            lstToFillRcvDtl[i].SerialNumber = orderDetailTrackingList[i].SerialNumber;
                            lstToFillRcvDtl[i].LotNumber = orderDetailTrackingList[i].LotNumber;
                            lstToFillRcvDtl[i].PackSlipNumber = orderDetailTrackingList[i].PackSlipNumber;
                            lstToFillRcvDtl[i].ExpirationDate = (orderDetailTrackingList[i].ExpirationDate.HasValue ? FnCommon.ConvertDateByTimeZone(orderDetailTrackingList[i].ExpirationDate, false, true) : null);
                            lstToFillRcvDtl[i].Quantity = orderDetailTrackingList[i].Quantity.HasValue ? orderDetailTrackingList[i].Quantity.Value : lstToFillRcvDtl[i].Quantity;
                            lstToFillRcvDtl[i].ID = orderDetailTrackingList[i].ID;
                        }
                        else
                        {
                            break; // Exit loop if lstToFillRcvDtl is exhausted
                        }
                    }
                }
                MakePreReceive.IsModelShow = true;
                MakePreReceive.MakePreReceiveDetail = lstToFillRcvDtl;

                if (MakePreReceive.OrderDetailGUID != null
                    && MakePreReceive.OrderDetailGUID != Guid.Empty)
                {

                    orderDetailsDTO = orderDetailDAL.GetOrderDetailByGuidPlain(MakePreReceive.OrderDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (orderDetailsDTO != null && orderDetailsDTO.ID > 0)
                    {
                        MakePreReceive.OrderDetailItemCost = orderDetailsDTO.ItemCost.GetValueOrDefault(0);
                    }
                }
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



        /// <summary>
        /// Pre Receive Info
        /// </summary>
        /// <param name="OrderDetailGuid"></param>
        /// <param name="ItemGuid"></param>
        /// <returns></returns>
        public ActionResult FillPreReceiveInfoForLotExpItems(ToFillPreReceiveDTO MakePreReceive)
        {
            OrderDetailsDAL orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            PreReceivOrderDetailDAL PreReciveDAL = null;
            List<PreReceivOrderDetailDTO> preReceveInfo = null;
            List<ToFillReceiveDetailDTO> lstToFillRcvDtl = null;
            ToFillReceiveDetailDTO detailDTO = null;
            try
            {

                lstToFillRcvDtl = new List<ToFillReceiveDetailDTO>();
                PreReciveDAL = new PreReceivOrderDetailDAL(SessionHelper.EnterPriseDBName);
                preReceveInfo = PreReciveDAL.GetAllRecordsByOrderDetailItem(MakePreReceive.OrderDetailGUID, MakePreReceive.ItemGUID).ToList();
                ViewBag.ItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, MakePreReceive.ItemGUID);
                if (preReceveInfo != null && preReceveInfo.Count > 0)
                {
                    foreach (var item in preReceveInfo)
                    {
                        detailDTO = GetToFillReceiveDetailList(item, MakePreReceive);
                        lstToFillRcvDtl.Add(detailDTO);
                    }
                }

                detailDTO = GetToFillReceiveDetailList(new PreReceivOrderDetailDTO(), MakePreReceive);
                if (preReceveInfo.Count <= 0 && MakePreReceive.MakePreReceiveDetail != null && MakePreReceive.MakePreReceiveDetail.Count > 0)
                {
                    detailDTO.Quantity = MakePreReceive.MakePreReceiveDetail[0].Quantity;
                }
                MakePreReceive.IsModelShow = true;
                lstToFillRcvDtl.Add(detailDTO);

                List<PreReceivOrderDetailDTO> orderDetailTrackingList = orderDetailDAL.GetOrderDetailTrackingByGuidPlain(MakePreReceive.OrderDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                for (int i = 0; i < orderDetailTrackingList.Count; i++)
                {
                    double QuantityToReceive = (MakePreReceive.MakePreReceiveDetail[0].QuantityToReceive <= 0 ? MakePreReceive.MakePreReceiveDetail[0].Quantity : MakePreReceive.MakePreReceiveDetail[0].QuantityToReceive);
                    if (i < QuantityToReceive)
                    {
                        if (i < lstToFillRcvDtl.Count)
                        {
                            lstToFillRcvDtl[i].LotNumber = orderDetailTrackingList[i].LotNumber;
                            lstToFillRcvDtl[i].PackSlipNumber = orderDetailTrackingList[i].PackSlipNumber;
                            lstToFillRcvDtl[i].ExpirationDate = (orderDetailTrackingList[i].ExpirationDate.HasValue ? FnCommon.ConvertDateByTimeZone(orderDetailTrackingList[i].ExpirationDate, false, true) : null);
                            lstToFillRcvDtl[i].Quantity = orderDetailTrackingList[i].Quantity.HasValue ? orderDetailTrackingList[i].Quantity.Value : lstToFillRcvDtl[i].Quantity;
                            lstToFillRcvDtl[i].ID = orderDetailTrackingList[i].ID;
                        }
                        else
                        {
                            ToFillReceiveDetailDTO lstOfLotData = new ToFillReceiveDetailDTO(); // Replace YourItemType with the actual type of items in lstToFillRcvDtl
                            lstOfLotData.LotNumber = orderDetailTrackingList[i].LotNumber;
                            lstOfLotData.PackSlipNumber = orderDetailTrackingList[i].PackSlipNumber;
                            lstOfLotData.ExpirationDate = (orderDetailTrackingList[i].ExpirationDate.HasValue ? FnCommon.ConvertDateByTimeZone(orderDetailTrackingList[i].ExpirationDate, false, true) : null);
                            lstOfLotData.Quantity = orderDetailTrackingList[i].Quantity.HasValue ? orderDetailTrackingList[i].Quantity.Value : 0; // You may need to adjust this line depending on the type of Quantity
                            lstOfLotData.ID = orderDetailTrackingList[i].ID;

                            lstToFillRcvDtl.Add(lstOfLotData);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                MakePreReceive.MakePreReceiveDetail = lstToFillRcvDtl;
                if (MakePreReceive.OrderDetailGUID != null
                    && MakePreReceive.OrderDetailGUID != Guid.Empty)
                {
                    OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();
                    orderDetailsDTO = orderDetailDAL.GetOrderDetailByGuidPlain(MakePreReceive.OrderDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (orderDetailsDTO != null && orderDetailsDTO.ID > 0)
                    {
                        MakePreReceive.OrderDetailItemCost = orderDetailsDTO.ItemCost.GetValueOrDefault(0);
                    }
                }
                return PartialView("PreRecieveInfoForLotExpItem", MakePreReceive);
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


        private ToFillReceiveDetailDTO GetToFillReceiveDetailList(PreReceivOrderDetailDTO item, ToFillPreReceiveDTO MakePreReceive)
        {
            ToFillReceiveDetailDTO toFillRcvDtl = new ToFillReceiveDetailDTO();

            if (item.ExpirationDate.HasValue)
                toFillRcvDtl.ExpirationDate = FnCommon.ConvertDateByTimeZone(item.ExpirationDate, false, true);

            toFillRcvDtl.LotNumber = item.LotNumber;
            toFillRcvDtl.SerialNumber = item.SerialNumber;
            toFillRcvDtl.Quantity = item.Quantity.GetValueOrDefault(0);
            if (MakePreReceive.SerialNumberTracking)
            {
                toFillRcvDtl.Quantity = 1;
            }

            return toFillRcvDtl;

        }

        [HttpPost]
        public ActionResult SaveReceiveInformation(List<ToFillPreReceiveDTO> SavePreReceiveData)
        {
            //OrderMasterDAL orderDAL = null;
            //OrderDetailsDAL orderDetailDAL = null;
            //OrderMasterDTO orderDTO = null;
            //OrderDetailsDTO orderDetailDTO = null;
            ////List<ItemLocationDetailsDTO> lstROTD = null;
            //List<ReceivedOrderTransferDetailDTO> lstROTD = null;
            //List<MaterialStagingPullDetailDTO> lstMSPD = null;
            //List<ReceiveErrors> listReceiveErrors = null;
            //ReceiveErrors receiveError = null;
            //List<Guid> listOrderGuids = null;
            //int ordStatus = 0;
            //InventoryController objInvCtrl = null;
            //listReceiveErrors = new List<ReceiveErrors>();
            //ItemMasterDAL ItemDAL = null;
            //try
            //{

            //    if (SavePreReceiveData == null || SavePreReceiveData.Count <= 0)
            //    {
            //        receiveError = new ReceiveErrors()
            //        {
            //            ErrorColor = string.Empty,
            //            ErrorMassage = "Please select data to Receive",
            //            ErrorTitle = "",
            //            OrderDetailGuid = Guid.Empty,
            //        };
            //        listReceiveErrors.Add(receiveError);

            //        return Json(new { Status = false, Message = "Error", Errors = listReceiveErrors }, JsonRequestBehavior.AllowGet);
            //    }


            //    // lstROTD = new List<ItemLocationDetailsDTO>();
            //    lstROTD = new List<ReceivedOrderTransferDetailDTO>();
            //    lstMSPD = new List<MaterialStagingPullDetailDTO>();
            //    orderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            //    orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            //    ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //    listOrderGuids = new List<Guid>();
            //    List<ToFillPreReceiveDTO> lstOrdRecSerial = null;

            //    if (SavePreReceiveData != null && SavePreReceiveData.Count > 0)
            //    {
            //        lstOrdRecSerial = (from t in SavePreReceiveData
            //                           select new ToFillPreReceiveDTO
            //                            {
            //                                ItemGUID = t.ItemGUID
            //                            }).OrderBy(x => x.ItemNumber).GroupBy(x => new { x.ItemGUID }).Select(x => new ToFillPreReceiveDTO
            //                                                   {
            //                                                       ItemGUID = x.Key.ItemGUID,
            //                                                       TotalRecord = x.Count()
            //                                                   }).ToList();
            //    }
            //    foreach (var item in SavePreReceiveData)
            //    {
            //       orderDetailDTO = orderDetailDAL.GetOrderDetailByGuidFull(item.OrderDetailGUID, RoomID, CompanyID);
            //        orderDTO = orderDAL.GetOrderByGuidPlain(orderDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));

            //        if (orderDetailDTO.IsCloseItem.GetValueOrDefault(false))
            //        {
            //            receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
            //            if (receiveError != null)
            //            {
            //                receiveError.ErrorMassage += "Order line item is closed.";
            //            }
            //            else
            //            {
            //                receiveError = new ReceiveErrors()
            //                {
            //                    ErrorColor = "Red",
            //                    ErrorMassage = "Order line item is closed.",
            //                    ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                    OrderDetailGuid = orderDetailDTO.GUID,
            //                };
            //                listReceiveErrors.Add(receiveError);
            //            }
            //            continue;
            //        }
            //        if (orderDTO.OrderStatus == (int)OrderStatus.Closed)
            //        {
            //            receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
            //            if (receiveError != null)
            //            {
            //                receiveError.ErrorMassage += " Order is closed.";
            //            }
            //            else
            //            {
            //                receiveError = new ReceiveErrors()
            //                {
            //                    ErrorColor = "Red",
            //                    ErrorMassage = "Order is closed.",
            //                    ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                    OrderDetailGuid = orderDetailDTO.GUID,
            //                };
            //                listReceiveErrors.Add(receiveError);
            //            }
            //            continue;
            //        }
            //        if (string.IsNullOrEmpty(item.BinNumber))
            //        {
            //            receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
            //            if (receiveError != null)
            //            {
            //                receiveError.ErrorMassage += " BinNumber is mandatory.";
            //            }
            //            else
            //            {
            //                receiveError = new ReceiveErrors()
            //                {
            //                    ErrorColor = "Red",
            //                    ErrorMassage = "BinNumber is mandatory.",
            //                    ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                    OrderDetailGuid = orderDetailDTO.GUID,
            //                };
            //                listReceiveErrors.Add(receiveError);
            //            }
            //            continue;
            //        }

            //        if (orderDetailDTO.IsPackslipMandatoryAtReceive && string.IsNullOrEmpty(item.PackSlipNumber))
            //        {
            //            receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
            //            if (receiveError != null)
            //            {
            //                receiveError.ErrorMassage += " Packslip is mandatory.";
            //            }
            //            else
            //            {
            //                receiveError = new ReceiveErrors()
            //                {
            //                    ErrorColor = "Red",
            //                    ErrorMassage = "Packslip is mandatory.",
            //                    ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                    OrderDetailGuid = orderDetailDTO.GUID,
            //                };
            //                listReceiveErrors.Add(receiveError);
            //            }
            //            continue;
            //        }

            //        string udfError = ValidateReceiveUDF(item);
            //        if (!string.IsNullOrEmpty(udfError))
            //        {
            //            receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
            //            if (receiveError != null)
            //            {
            //                receiveError.ErrorMassage += " " + udfError;
            //            }
            //            else
            //            {
            //                receiveError = new ReceiveErrors()
            //                {
            //                    ErrorColor = "Red",
            //                    ErrorMassage = udfError,
            //                    ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                    OrderDetailGuid = orderDetailDTO.GUID,
            //                };
            //                listReceiveErrors.Add(receiveError);
            //            }
            //            continue;
            //        }


            //        if (!(item.MakePreReceiveDetail != null && item.MakePreReceiveDetail.Count > 0))
            //        {
            //            receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
            //            if (receiveError != null)
            //            {
            //                receiveError.ErrorMassage += " Please enter data to receive.";
            //            }
            //            else
            //            {
            //                receiveError = new ReceiveErrors()
            //                {
            //                    ErrorColor = "Red",
            //                    ErrorMassage = "Please enter data to receive.",
            //                    ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                    OrderDetailGuid = orderDetailDTO.GUID,
            //                };

            //                listReceiveErrors.Add(receiveError);
            //            }
            //            continue;
            //        }
            //        else
            //        {
            //            foreach (var innerItem in item.MakePreReceiveDetail)
            //            {
            //                if (innerItem.Quantity <= 0)
            //                {
            //                    receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
            //                    if (receiveError != null)
            //                    {
            //                        receiveError.ErrorMassage += " Please enter qauntity to receive.";
            //                    }
            //                    else
            //                    {
            //                        receiveError = new ReceiveErrors()
            //                        {
            //                            ErrorColor = "Red",
            //                            ErrorMassage = "Please enter qauntity to receive.",
            //                            ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                            OrderDetailGuid = orderDetailDTO.GUID,
            //                        };

            //                        listReceiveErrors.Add(receiveError);
            //                    }
            //                    continue;
            //                }

            //                if (orderDetailDTO.SerialNumberTracking)
            //                {
            //                    listReceiveErrors = ValidateSerials(listReceiveErrors, orderDTO, orderDetailDTO, innerItem, item);
            //                    //if (lstROTD != null && lstROTD.Select(x => x.SerialNumber.Trim()).Contains(innerItem.SerialNumber.Trim())
            //                    if (lstROTD != null && lstROTD.Where(x=> x.SerialNumber.Trim() == innerItem.SerialNumber.Trim() && x.ItemGUID.GetValueOrDefault(Guid.Empty) == item.ItemGUID).Count() > 0)
            //                    {
            //                        receiveError = new ReceiveErrors()
            //                        {
            //                            ErrorColor = "Red",
            //                            ErrorMassage = "duplicate serial# found.",
            //                            ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                            OrderDetailGuid = orderDetailDTO.GUID,
            //                        };

            //                        listReceiveErrors.Add(receiveError);
            //                    }

            //                    if (lstMSPD != null && lstMSPD.Select(x => x.SerialNumber).Contains(innerItem.SerialNumber))
            //                    {
            //                        receiveError = new ReceiveErrors()
            //                        {
            //                            ErrorColor = "Red",
            //                            ErrorMassage = "duplicate serial# found.",
            //                            ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                            OrderDetailGuid = orderDetailDTO.GUID,
            //                        };

            //                        listReceiveErrors.Add(receiveError);
            //                    }

            //                    ItemMasterDTO IMDTO = ItemDAL.GetItemByGuidNormal(item.ItemGUID, RoomID, CompanyID);
            //                    if (IMDTO != null && (IMDTO.OrderUOMValue == null || IMDTO.OrderUOMValue <= 0))
            //                        IMDTO.OrderUOMValue = 1;

            //                    ToFillPreReceiveDTO objToFillPreReceiveDTO = lstOrdRecSerial.Where(x => x.ItemGUID == item.ItemGUID).FirstOrDefault();
            //                    if (objToFillPreReceiveDTO != null && IMDTO.IsAllowOrderCostuom)
            //                    {
            //                        double TotReceived = orderDetailDTO.ReceivedQuantity.GetValueOrDefault(0);
            //                        double TotToFillPreReceive = objToFillPreReceiveDTO.TotalRecord.GetValueOrDefault(0);
            //                        //if ((objToFillPreReceiveDTO.TotalRecord % IMDTO.OrderUOMValue) != 0)
            //                        if (((TotReceived + TotToFillPreReceive) % IMDTO.OrderUOMValue) != 0)
            //                        {
            //                            receiveError = new ReceiveErrors()
            //                            {
            //                                ErrorColor = "Red",
            //                                ErrorMassage = "qty must receive equal to orderuom.",
            //                                ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                                OrderDetailGuid = orderDetailDTO.GUID,
            //                            };

            //                            listReceiveErrors.Add(receiveError);
            //                        }
            //                        //else if ((TotReceived + TotToFillPreReceive) > orderDetailDTO.ApprovedQuantity.GetValueOrDefault(0))
            //                        //{
            //                        //    receiveError = new ReceiveErrors()
            //                        //    {
            //                        //        ErrorColor = "Red",
            //                        //        ErrorMassage = "qty not receive more then approved qty.",
            //                        //        ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
            //                        //        OrderDetailGuid = orderDetailDTO.GUID,
            //                        //    };

            //                        //    listReceiveErrors.Add(receiveError);
            //                        //}

            //                    }
            //                }

            //                else if (orderDetailDTO.LotNumberTracking && orderDetailDTO.DateCodeTracking)
            //                    listReceiveErrors = ValidateLotAndDateCodes(listReceiveErrors, orderDTO, orderDetailDTO, innerItem, item);

            //                else if (orderDetailDTO.LotNumberTracking)
            //                    listReceiveErrors = ValidateLots(listReceiveErrors, orderDTO, orderDetailDTO, innerItem, item);

            //                if (orderDetailDTO.DateCodeTracking && !orderDetailDTO.LotNumberTracking)
            //                    listReceiveErrors = ValidateExpiration(listReceiveErrors, orderDTO, orderDetailDTO, innerItem, item);

            //                if (listReceiveErrors.FindIndex(x => x.OrderDetailGuid == item.OrderDetailGUID) < 0)
            //                {
            //                    if (!orderDTO.MaterialStagingGUID.HasValue)
            //                    {
            //                        //ItemLocationDetailsDTO rotd = GetReceiveOrderTransferDetailDTO(orderDTO, orderDetailDTO, innerItem, item);
            //                        ReceivedOrderTransferDetailDTO rotd = GetROTDDTO(orderDTO, orderDetailDTO, innerItem, item);
            //                        rotd.PackSlipNumber = item.PackSlipNumber;
            //                        lstROTD.Add(rotd);
            //                    }
            //                    else
            //                    {
            //                        ItemLocationDetailsDTO ildd = GetReceiveOrderTransferDetailDTO(orderDTO, orderDetailDTO, innerItem, item);
            //                        MaterialStagingPullDetailDTO mspdd = GetMaterialStagingPullDetails(orderDTO, ildd);
            //                        ildd.PackSlipNumber = item.PackSlipNumber;
            //                        mspdd.PackSlipNumber = item.PackSlipNumber;
            //                        lstMSPD.Add(mspdd);
            //                    }

            //                    if (listOrderGuids.FindIndex(x => x == orderDTO.GUID) < 0)
            //                        listOrderGuids.Add(orderDTO.GUID);
            //                }
            //            }
            //        }

            //        orderDetailDTO.Comment = item.Comment;

            //        orderDetailDAL.UpdateLineComment(orderDetailDTO, SessionHelper.UserID);
            //    }

            //    long SessionUserId = SessionHelper.UserID;
            //    if (lstROTD != null && lstROTD.Count > 0)
            //    {
            //        //ItemLocationDetailsDAL itmLocDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            //        //itmLocDAL.InsertItemLocationDetailsFromRecieve(lstROTD);
            //        string receiveGUIDs = string.Empty;
            //        ReceivedOrderTransferDetailDAL itmLocDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
            //        foreach (var item in lstROTD)
            //        {
            //            item.OrderStatus = orderDTO.OrderStatus;

            //            OrderUOMMasterDAL objOrderUOMDAL = new OrderUOMMasterDAL(SessionHelper.EnterPriseDBName);
            //            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            //            objItemMasterDTO = objItemMasterDAL.GetItemByGuidNormal(item.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID); 
            //            if (objItemMasterDTO.SerialNumberTracking == false && objItemMasterDTO.IsAllowOrderCostuom)
            //            {
            //                if (item.CustomerOwnedQuantity != null && item.CustomerOwnedQuantity >= 0)
            //                    item.CustomerOwnedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.CustomerOwnedQuantity);
            //                if (item.ConsignedQuantity != null && item.ConsignedQuantity >= 0)
            //                    item.ConsignedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.ConsignedQuantity);
            //            }
            //            itmLocDAL.InsertReceive(item, SessionUserId);
            //            receiveGUIDs += "," + Convert.ToString(item.GUID);
            //        }

            //        try
            //        {

            //            string dataGUIDs = "<DataGuids>" + receiveGUIDs.Trim(commaTrim) + "</DataGuids>";
            //            string eventName = "ORECC";
            //            string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
            //            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            //            List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            //            if (lstNotification != null && lstNotification.Count > 0)
            //            {
            //                objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
            //            }
            //        }
            //        catch (Exception ex)
            //        {

            //            CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
            //        }
            //    }

            //    if (lstMSPD != null && lstMSPD.Count > 0)
            //    {
            //        objInvCtrl = new InventoryController();
            //        foreach (var item in lstMSPD)
            //        {
            //            List<MaterialStagingPullDetailDTO> mspdDTO = new List<MaterialStagingPullDetailDTO>();
            //            mspdDTO.Add(item);
            //            objInvCtrl.ItemLocationDetailsSaveForMSCredit(mspdDTO);
            //        }

            //    }

            //    if (listOrderGuids != null && listOrderGuids.Count > 0)
            //    {
            //        foreach (var ordGuid in listOrderGuids)
            //        {
            //            ordStatus = (int)orderDetailDAL.UpdateOrderStatusByReceiveNew(ordGuid, RoomID, CompanyID, SessionHelper.UserID, true);
            //        }
            //        //if (listOrderGuids.Count == 1)
            //        //{
            //        //    orderDTO = orderDAL.GetRecord(listOrderGuids[0], RoomID, CompanyID);
            //        //    ordStatus = orderDTO.OrderStatus;
            //        //}
            //    }

            //    return Json(new { Status = true, Message = "", Errors = listReceiveErrors, OrderStatus = ordStatus }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
            //    receiveError = new ReceiveErrors()
            //    {
            //        ErrorColor = "Red",
            //        ErrorMassage = ex.Message,
            //        ErrorTitle = "Exception",
            //        OrderDetailGuid = Guid.Empty,
            //    };
            //    listReceiveErrors.Add(receiveError);
            //    return Json(new { Status = false, Message = "Exception", Errors = listReceiveErrors, OrderStatus = ordStatus }, JsonRequestBehavior.AllowGet);
            //}


            using (ReceiveBAL receiveBAL = new ReceiveBAL())
            {
                var saveInfo = receiveBAL.SaveReceiveInformation(SavePreReceiveData);
                return Json(new
                {
                    Status = saveInfo.Status,
                    Message = saveInfo.Message,
                    Errors = saveInfo.Errors,
                    OrderStatus = saveInfo.OrderStatus,
                    OrderDetailReceived = saveInfo.OrderDetailReceivedGuids
                }, JsonRequestBehavior.AllowGet);
            }

        }

        //private ItemLocationDetailsDTO GetReceiveOrderTransferDetailDTO(OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        //{
        //    BinMasterDAL binDAL = null;
        //    binDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

        //    double? ConsignQty = null;
        //    double? CustQty = null;

        //    if (orderDetailDTO.Consignment)
        //        ConsignQty = innerItem.Quantity;
        //    else
        //        CustQty = innerItem.Quantity;


        //    DateTime? ExpDate = null;
        //    //DateTime recDate;
        //    if (!string.IsNullOrEmpty(innerItem.ExpirationDate))
        //    {
        //        //ExpDate = DateTime.ParseExact(innerItem.ExpirationDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
        //        ExpDate = DateTime.ParseExact(innerItem.ExpirationDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
        //    }

        //    //if (!string.IsNullOrEmpty(item.ReceivedDate))
        //    //{
        //    //    //recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
        //    //    recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
        //    //}
        //    //else
        //    //    recDate = DateTimeUtility.DateTimeNow;

        //    DateTime currentDateAsPerRoom = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
        //    DateTime newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, item.ReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, SessionHelper.CurrentTimeZone,SessionHelper.RoomTimeFormat);



        //    ItemLocationDetailsDTO rotd = new ItemLocationDetailsDTO()
        //    {
        //        BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID, item.BinNumber, UserID, RoomID, CompanyID, (orderDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)),
        //        CompanyID = CompanyID,
        //        Room = RoomID,
        //        Cost = item.Cost,
        //        ItemGUID = item.ItemGUID,
        //        OrderDetailGUID = item.OrderDetailGUID,
        //        ReceivedDate = newReceiveTempDate, //recDate,
        //        Action = string.Empty,
        //        AddedFrom = "Web",
        //        BinNumber = item.BinNumber,
        //        ConsignedQuantity = ConsignQty,
        //        CustomerOwnedQuantity = CustQty,
        //        ExpirationDate = ExpDate,
        //        SerialNumber = innerItem.SerialNumber,
        //        LotNumber = innerItem.LotNumber,
        //        EditedFrom = "Web",
        //        CreatedBy = UserID,
        //        LastUpdatedBy = UserID,
        //        Created = DateTimeUtility.DateTimeNow,
        //        Updated = DateTimeUtility.DateTimeNow,
        //        UDF1 = item.UDF1,
        //        UDF2 = item.UDF2,
        //        UDF3 = item.UDF3,
        //        UDF4 = item.UDF4,
        //        UDF5 = item.UDF5,
        //        InitialQuantity = innerItem.Quantity,
        //        InitialQuantityWeb = innerItem.Quantity,
        //        ReceivedOn = DateTimeUtility.DateTimeNow,
        //        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
        //    };


        //    return rotd;

        //}

        //private ReceivedOrderTransferDetailDTO GetROTDDTO(OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        //{
        //    BinMasterDAL binDAL = null;
        //    binDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

        //    double? ConsignQty = null;
        //    double? CustQty = null;

        //    if (orderDetailDTO.Consignment)
        //        ConsignQty = innerItem.Quantity;
        //    else
        //        CustQty = innerItem.Quantity;


        //    DateTime? ExpDate = null;
        //    //DateTime recDate;
        //    if (!string.IsNullOrEmpty(innerItem.ExpirationDate))
        //    {
        //        //ExpDate = DateTime.ParseExact(innerItem.ExpirationDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
        //        ExpDate = DateTime.ParseExact(innerItem.ExpirationDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
        //    }

        //    //if (!string.IsNullOrEmpty(item.ReceivedDate))
        //    //{
        //    //    //recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
        //    //    recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
        //    //}
        //    //else
        //    //    recDate = DateTimeUtility.DateTimeNow;

        //    DateTime currentDateAsPerRoom = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
        //    DateTime newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, item.ReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, SessionHelper.CurrentTimeZone,SessionHelper.RoomTimeFormat);

        //    ReceivedOrderTransferDetailDTO rotd = new ReceivedOrderTransferDetailDTO()
        //    {
        //        BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID, item.BinNumber, UserID, RoomID, CompanyID, (orderDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)),
        //        CompanyID = CompanyID,
        //        Room = RoomID,
        //        Cost = item.Cost,
        //        ItemGUID = item.ItemGUID,
        //        OrderDetailGUID = item.OrderDetailGUID,
        //        ReceivedDate = newReceiveTempDate,//recDate,
        //        Action = string.Empty,
        //        AddedFrom = "Web",
        //        BinNumber = item.BinNumber,
        //        ConsignedQuantity = ConsignQty,
        //        CustomerOwnedQuantity = CustQty,
        //        ExpirationDate = ExpDate,
        //        SerialNumber = (!string.IsNullOrWhiteSpace(innerItem.SerialNumber)) ? innerItem.SerialNumber.Trim() : string.Empty,
        //        LotNumber = (!string.IsNullOrWhiteSpace(innerItem.LotNumber)) ? innerItem.LotNumber.Trim() : string.Empty,
        //        EditedFrom = "Web",
        //        CreatedBy = UserID,
        //        LastUpdatedBy = UserID,
        //        Created = DateTimeUtility.DateTimeNow,
        //        Updated = DateTimeUtility.DateTimeNow,
        //        UDF1 = item.UDF1,
        //        UDF2 = item.UDF2,
        //        UDF3 = item.UDF3,
        //        UDF4 = item.UDF4,
        //        UDF5 = item.UDF5,
        //        ControlNumber = string.Empty,
        //        PackSlipNumber = item.PackSlipNumber,
        //        ReceivedOn = DateTimeUtility.DateTimeNow,
        //        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
        //        IsOnlyFromUI = true,
        //        IsEDISent = false,

        //    };


        //    return rotd;

        //}

        //private MaterialStagingPullDetailDTO GetMaterialStagingPullDetails(OrderMasterDTO orderDTO, ItemLocationDetailsDTO objDTO)
        //{
        //    MaterialStagingPullDetailDTO obj = new MaterialStagingPullDetailDTO()
        //    {
        //        OrderDetailGUID = objDTO.OrderDetailGUID,
        //        BinID = objDTO.BinID,
        //        ConsignedQuantity = objDTO.ConsignedQuantity,
        //        CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity,
        //        LotNumber = objDTO.LotNumber,
        //        SerialNumber = objDTO.SerialNumber,
        //        MaterialStagingGUID = orderDTO.MaterialStagingGUID,
        //        BinNumber = objDTO.BinNumber,
        //        CompanyID = SessionHelper.CompanyID,
        //        ItemCost = objDTO.Cost,
        //        Created = DateTimeUtility.DateTimeNow,
        //        CreatedBy = SessionHelper.UserID,
        //        CreatedByName = SessionHelper.UserName,
        //        Expiration = objDTO.ExpirationDate.HasValue ? objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.RoomDateFormat) : "",
        //        ID = 0,
        //        IsArchived = false,
        //        GUID = Guid.Empty,
        //        IsDeleted = false,
        //        ItemGUID = objDTO.ItemGUID.GetValueOrDefault(Guid.Empty),
        //        ItemNumber = objDTO.ItemNumber,
        //        LastUpdatedBy = SessionHelper.UserID,
        //        Received = objDTO.ReceivedDate.HasValue ? objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.RoomDateFormat) : "",
        //        Room = SessionHelper.RoomID,
        //        RoomName = SessionHelper.RoomName,
        //        Updated = DateTimeUtility.DateTimeNow,
        //        UpdatedByName = SessionHelper.UserName,
        //        UDF1 = objDTO.UDF1,
        //        UDF2 = objDTO.UDF2,
        //        UDF3 = objDTO.UDF3,
        //        UDF4 = objDTO.UDF4,
        //        UDF5 = objDTO.UDF5,
        //        PackSlipNumber = objDTO.PackSlipNumber,

        //    };
        //    return obj;
        //}

        //private List<ReceiveErrors> ValidateSerials(List<ReceiveErrors> listReceiveErrors, OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        //{
        //    CommonDAL cmnDAL = null;
        //    cmnDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //    ReceiveErrors receiveError = null;

        //    if (string.IsNullOrEmpty(innerItem.SerialNumber))
        //    {
        //        receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
        //        if (receiveError != null)
        //        {
        //            receiveError.ErrorMassage += " Please enter serial# to receive.";
        //        }
        //        else
        //        {
        //            receiveError = new ReceiveErrors()
        //            {
        //                ErrorColor = "Red",
        //                ErrorMassage = "Please enter seral# to receive.",
        //                ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
        //                OrderDetailGuid = orderDetailDTO.GUID,
        //            };

        //            listReceiveErrors.Add(receiveError);
        //        }
        //    }
        //    else
        //    {
        //        string dupSerial = cmnDAL.CheckDuplicateSerialNumbers(innerItem.SerialNumber, 0, RoomID, CompanyID, item.ItemGUID);
        //        if (dupSerial == "duplicate" || (item.MakePreReceiveDetail.Count(x => x.SerialNumber == innerItem.SerialNumber) > 1))
        //        {
        //            receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
        //            if (receiveError != null)
        //            {
        //                receiveError.ErrorMassage += " duplicate serial# found.";
        //            }
        //            else
        //            {
        //                receiveError = new ReceiveErrors()
        //                {
        //                    ErrorColor = "Red",
        //                    ErrorMassage = "duplicate serial# found.",
        //                    ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
        //                    OrderDetailGuid = orderDetailDTO.GUID,
        //                };

        //                listReceiveErrors.Add(receiveError);
        //            }
        //        }

        //    }

        //    return listReceiveErrors;
        //}


        //ValidateLotAndDateCodes
        //private List<ReceiveErrors> ValidateLotAndDateCodes(List<ReceiveErrors> listReceiveErrors, OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        //{
        //    CommonDAL cmnDAL = null;
        //    cmnDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //    ReceiveErrors receiveError = null;

        //    if (string.IsNullOrEmpty(innerItem.LotNumber) || string.IsNullOrWhiteSpace(innerItem.ExpirationDate))
        //    {
        //        receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
        //        if (receiveError != null)
        //        {
        //            receiveError.ErrorMassage += " Please enter lot# and expiration date to receive.";
        //        }
        //        else
        //        {
        //            receiveError = new ReceiveErrors()
        //            {
        //                ErrorColor = "Red",
        //                ErrorMassage = "Please enter lot# and expiration date to receive.",
        //                ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
        //                OrderDetailGuid = orderDetailDTO.GUID,
        //            };

        //            listReceiveErrors.Add(receiveError);
        //        }
        //    }
        //    else
        //    {

        //        DateTime ExpDate = DateTime.MinValue;
        //        DateTime.TryParseExact(innerItem.ExpirationDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, DateTimeStyles.None, out ExpDate);
        //        string Expiration = ExpDate.ToString("MM/dd/yyyy");
        //        if (ExpDate != DateTime.MinValue)
        //        {
        //            string msg = cmnDAL.CheckDuplicateLotAndExpiration(innerItem.LotNumber, Expiration, ExpDate, 0, SessionHelper.RoomID, SessionHelper.CompanyID, orderDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
        //            if (string.IsNullOrWhiteSpace(msg) || (msg ?? string.Empty).ToLower() == "ok")
        //            {

        //            }
        //            else
        //            {
        //                receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
        //                if (receiveError != null)
        //                {
        //                    receiveError.ErrorMassage += " " + msg;
        //                }
        //                else
        //                {
        //                    receiveError = new ReceiveErrors()
        //                    {
        //                        ErrorColor = "Red",
        //                        ErrorMassage = msg,
        //                        ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
        //                        OrderDetailGuid = orderDetailDTO.GUID,
        //                    };

        //                    listReceiveErrors.Add(receiveError);
        //                }
        //            }
        //        }

        //    }

        //    return listReceiveErrors;
        //}


        //private List<ReceiveErrors> ValidateLots(List<ReceiveErrors> listReceiveErrors, OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        //{
        //    CommonDAL cmnDAL = null;
        //    cmnDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //    ReceiveErrors receiveError = null;

        //    if (string.IsNullOrEmpty(innerItem.LotNumber))
        //    {
        //        receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
        //        if (receiveError != null)
        //        {
        //            receiveError.ErrorMassage += " Please enter lot# to receive.";
        //        }
        //        else
        //        {
        //            receiveError = new ReceiveErrors()
        //            {
        //                ErrorColor = "Red",
        //                ErrorMassage = "Please enter lot# to receive.",
        //                ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
        //                OrderDetailGuid = orderDetailDTO.GUID,
        //            };

        //            listReceiveErrors.Add(receiveError);
        //        }
        //    }

        //    return listReceiveErrors;
        //}

        //private List<ReceiveErrors> ValidateExpiration(List<ReceiveErrors> listReceiveErrors, OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        //{
        //    CommonDAL cmnDAL = null;
        //    cmnDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //    ReceiveErrors receiveError = null;
        //    if (orderDetailDTO.DateCodeTracking)
        //    {
        //        if (string.IsNullOrEmpty(innerItem.ExpirationDate))
        //        {
        //            receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
        //            if (receiveError != null)
        //            {
        //                receiveError.ErrorMassage += " Please enter expiration date to receive.";
        //            }
        //            else
        //            {
        //                receiveError = new ReceiveErrors()
        //                {
        //                    ErrorColor = "Red",
        //                    ErrorMassage = "Please enter expiration date to receive.",
        //                    ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
        //                    OrderDetailGuid = orderDetailDTO.GUID,
        //                };

        //                listReceiveErrors.Add(receiveError);
        //            }
        //        }

        //    }
        //    return listReceiveErrors;
        //}


        //private string ValidateReceiveUDF(ToFillPreReceiveDTO objDTO)
        //{
        //    UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //    IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, "ReceivedOrderTransferDetail", SessionHelper.RoomID);
        //    string udfRequier = string.Empty;
        //    foreach (var i in DataFromDB)
        //    {
        //        if (!i.IsDeleted && i.UDFControlType != null)
        //        {
        //            if (i.UDFColumnName == "UDF1" && i.UDFIsRequired.GetValueOrDefault(false) && string.IsNullOrEmpty(objDTO.UDF1))
        //            {
        //                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
        //                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
        //                if (!string.IsNullOrEmpty(val))
        //                    i.UDFDisplayColumnName = val;
        //                else
        //                    i.UDFDisplayColumnName = i.UDFColumnName;
        //                udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
        //            }
        //            else if (i.UDFColumnName == "UDF2" && i.UDFIsRequired.GetValueOrDefault(false) && string.IsNullOrEmpty(objDTO.UDF2))
        //            {
        //                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
        //                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
        //                if (!string.IsNullOrEmpty(val))
        //                    i.UDFDisplayColumnName = val;
        //                else
        //                    i.UDFDisplayColumnName = i.UDFColumnName;
        //                udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
        //            }
        //            else if (i.UDFColumnName == "UDF3" && i.UDFIsRequired.GetValueOrDefault(false) && string.IsNullOrEmpty(objDTO.UDF3))
        //            {
        //                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
        //                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
        //                if (!string.IsNullOrEmpty(val))
        //                    i.UDFDisplayColumnName = val;
        //                else
        //                    i.UDFDisplayColumnName = i.UDFColumnName;
        //                udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
        //            }
        //            else if (i.UDFColumnName == "UDF4" && i.UDFIsRequired.GetValueOrDefault(false) && string.IsNullOrEmpty(objDTO.UDF4))
        //            {
        //                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
        //                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
        //                if (!string.IsNullOrEmpty(val))
        //                    i.UDFDisplayColumnName = val;
        //                else
        //                    i.UDFDisplayColumnName = i.UDFColumnName;
        //                udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
        //            }
        //            else if (i.UDFColumnName == "UDF5" && i.UDFIsRequired.GetValueOrDefault(false) && string.IsNullOrEmpty(objDTO.UDF5))
        //            {
        //                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
        //                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
        //                if (!string.IsNullOrEmpty(val))
        //                    i.UDFDisplayColumnName = val;
        //                else
        //                    i.UDFDisplayColumnName = i.UDFColumnName;
        //                udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
        //            }

        //            if (!string.IsNullOrEmpty(udfRequier))
        //                break;
        //        }
        //    }

        //    return udfRequier;
        //}

        public JsonResult DeleteReceivedItems(List<ReceivedOrderTransferDetailDTO> ROTDGuids)
        {
            ReceivedOrderTransferDetailDAL objRecdOrdTrnDtlDAL = null;
            ReceivedOrderTransferDetailDTO objRecdOrdTrnDtlDTO = null;
            string returString = "fail";
            try
            {
                if (ROTDGuids != null)
                {
                    long SessionUserId = SessionHelper.UserID;
                    objRecdOrdTrnDtlDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                    foreach (var item in ROTDGuids)
                    {
                        objRecdOrdTrnDtlDTO = objRecdOrdTrnDtlDAL.GetROTDByGuidPlain(item.GUID, RoomID, CompanyID);
                        ItemLocationDetailsDTO objItemLocationDetail = new eTurns.DAL.ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(objRecdOrdTrnDtlDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                        bool isChanged = false;
                        if (objItemLocationDetail == null)
                        {
                            isChanged = true;
                        }
                        else if (objItemLocationDetail.CustomerOwnedQuantity.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.CustomerOwnedQuantity.GetValueOrDefault(0))
                        {
                            isChanged = true;
                        }
                        else if (objItemLocationDetail.ConsignedQuantity.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.ConsignedQuantity.GetValueOrDefault(0))
                        {
                            isChanged = true;
                        }
                        else if (objItemLocationDetail.Cost.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.Cost.GetValueOrDefault(0))
                        {
                            isChanged = true;
                        }
                        else if (objItemLocationDetail.DateCodeTracking && objItemLocationDetail.Expiration != objRecdOrdTrnDtlDTO.Expiration)
                        {
                            isChanged = true;
                        }
                        else if (objItemLocationDetail.ReceivedDate != objRecdOrdTrnDtlDTO.ReceivedDate)
                        {
                            isChanged = true;
                        }
                        else if (objItemLocationDetail.SerialNumberTracking && objItemLocationDetail.SerialNumber.Trim() != objRecdOrdTrnDtlDTO.SerialNumber.Trim())
                        {
                            isChanged = true;
                        }
                        else if (objItemLocationDetail.LotNumberTracking && objItemLocationDetail.LotNumber.Trim() != objRecdOrdTrnDtlDTO.LotNumber.Trim())
                        {
                            isChanged = true;
                        }

                        if (!isChanged)
                        {
                            ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                            objRecdOrdTrnDtlDAL.DeleteRecords(item.GUID.ToString(), SessionHelper.UserID, RoomID, CompanyID, SessionUserId);
                            List<ReceivedOrderTransferDetailDTO> objModel = objRecdOrdTrnDtlDAL.GetROTDByOrderDetailGUIDPlain(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
                            var aax = objModel.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                            OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                            OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetOrderDetailByGuidPlain(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            objOrdDetailDTO.ReceivedQuantity = aax;
                            ItemMasterDTO ImDTO = ItemDAL.GetItemByGuidNormal(objOrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            if (ImDTO.OrderUOMValue == null || ImDTO.OrderUOMValue <= 0)
                                ImDTO.OrderUOMValue = 1;
                            if (ImDTO.IsAllowOrderCostuom)
                                objOrdDetailDTO.ReceivedQuantityUOM = objOrdDetailDTO.ReceivedQuantity / ImDTO.OrderUOMValue;
                            else
                                objOrdDetailDTO.ReceivedQuantityUOM = objOrdDetailDTO.ReceivedQuantity;
                            ordDetailCtrl.Edit(objOrdDetailDTO, SessionUserId, EnterpriseId);
                            returString = "ok";
                        }

                    }
                }
                if (returString == "ok")
                {
                    OrderDetailsDAL ordDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<Guid> ordGuids = ROTDGuids.Select(x => x.OrderGuid).Distinct();
                    foreach (var item in ordGuids)
                    {
                        ordDetailDAL.UpdateOrderStatusByReceive(item, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    }
                }

                return Json(new { Status = true, Message = returString }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Status = false, Message = ResCommon.Fail + ": " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteReturnedItems(List<ReceivedOrderTransferDetailDTO> ROTDGuids)
        {
            OrderDetailsDAL OrdDetailDAL = null;
            OrderDetailsDTO OrdDetailDTO = null;
            OrderMasterDAL OrderDAL = null;
            OrderMasterDTO OrderDTO = null;
            ReceivedOrderTransferDetailDAL RcvedOrdDtlDAL = null;

            try
            {
                long SessionUserId = SessionHelper.UserID;
                RcvedOrdDtlDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                OrdDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                OrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in ROTDGuids)
                {
                    OrdDetailDTO = OrdDetailDAL.GetOrderDetailByGuidPlain(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                    Guid OrderGuid = OrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty);
                    OrderDTO = OrderDAL.GetOrderByGuidPlain(item.OrderGuid);
                    bool isStagingOrder = OrderDTO.StagingID.GetValueOrDefault(0) > 0;
                    Guid[] GUIDsToDelete = new Guid[1] { item.GUID };
                    RcvedOrdDtlDAL.DeleteReturnedRecords(GUIDsToDelete, isStagingOrder, SessionHelper.UserID, RoomID, CompanyID, SessionUserId, EnterpriseId);
                    IEnumerable<ReceivedOrderTransferDetailDTO> objModel = RcvedOrdDtlDAL.GetROTDByOrderDetailGUIDPlain(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
                    var aax = objModel.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                    OrdDetailDTO.ReceivedQuantity = aax;
                    OrdDetailDAL.Edit(OrdDetailDTO, SessionUserId, EnterpriseId);
                }

                IEnumerable<Guid> ordGuids = ROTDGuids.Select(x => x.OrderGuid).Distinct();
                foreach (var item in ordGuids)
                {
                    OrdDetailDAL.UpdateOrderStatusByReceive(OrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), OrdDetailDTO.Room.GetValueOrDefault(0), OrdDetailDTO.CompanyID.GetValueOrDefault(0), OrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0));
                }


                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = ResOrder.RecordNotDeletedDueToError, Status = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                OrdDetailDAL = null;
                OrdDetailDTO = null;
                OrderDAL = null;
                OrderDTO = null;
                RcvedOrdDtlDAL = null;
            }


        }


        public JsonResult DeleteFileAttachment(string FileGuid)
        {
            ReceiveFileDetailsDAL receiveFileDetailsDAL = new ReceiveFileDetailsDAL(SessionHelper.EnterPriseDBName);
            if (receiveFileDetailsDAL.DeleteReceiveFileByGuid(FileGuid, SessionHelper.UserID))
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Message = ResMessage.FailToDelete, Status = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadReceiveDocument(List<Guid> lstGuids)
        {
            List<string> retData = new List<string>();
            ReceiveFileDetailsDAL receiveFileDetailsDAL = new ReceiveFileDetailsDAL(SessionHelper.EnterPriseDBName);
            OrderDetailsDAL orderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            foreach (var reqguid in lstGuids)
            {
                List<ReceiveFileDetailDTO> objOrderImageDetail = new List<ReceiveFileDetailDTO>();
                objOrderImageDetail = receiveFileDetailsDAL.GetReceiveFileByGuidPlain(reqguid).ToList();
                string receiveFilePath = string.IsNullOrEmpty(SiteSettingHelper.ReceiveFilePaths) ? "~/Uploads/ReceiveFile/" : SiteSettingHelper.ReceiveFilePaths;  //Settinfile.Element("WorkOrderFilePaths").Value;
                receiveFilePath = receiveFilePath.Replace("~", string.Empty);
                Guid reqGUID = Guid.Empty;
                OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();
                string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath, "");
                baseURL = SessionHelper.CurrentDomainURL;
                string ReceiveImagePath = baseURL + receiveFilePath + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/" + orderDetailsDTO.ID;
                if (Guid.TryParse(reqguid.ToString(), out reqGUID))
                {
                    orderDetailsDTO = orderDetailsDAL.GetOrderDetailByGuidPlain(reqGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if(orderDetailsDTO == null)
                    {
                       orderDetailsDTO = orderDetailsDAL.GetOrderDetailByGuidPlainArchieved(reqGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                    ReceiveImagePath = baseURL + receiveFilePath + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/" + orderDetailsDTO.ID;
                }

                foreach (ReceiveFileDetailDTO file in objOrderImageDetail)
                {
                    retData.Add(ReceiveImagePath + "/" + file.FileName);
                }
            }

            //return File(data, "application/csv", dtoModuleDetail.ResourceModuleName + "_" + dtoModuleDetail.PageName + "_MobileRes_" + DateTimeUtility.DateTimeNow.ToString("yyyyMMddHHmmss") + ".csv");
            return Json(new { Message = "Sucess", Status = true, ReturnFiles = retData }, JsonRequestBehavior.AllowGet);
        }


        #endregion

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

    /// <summary>
    /// ModelForReceivedItems
    /// </summary>
    //public class ModelForReceivedItems
    //{

    //    //public Int64 ItemID { get; set; }
    //    public string ItemGUID { get; set; }
    //    public string ItemType { get; set; }
    //    public List<ReceiveOrderLineItemDetailsDTO> ReceivedOrderList { get; set; }
    //}

}
