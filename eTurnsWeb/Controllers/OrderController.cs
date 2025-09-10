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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using eTurns.DTO.Utils;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using HtmlAgilityPack;
using eTurns.ABAPIBAL.Helper;
using System.Web;
using System.Security.Claims;
namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class OrderController : eTurnsControllerBase
    {
        bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderSubmit);
        bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderApproval);
        bool IsChangeOrder = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ChangeOrder);
        bool AllowConsignedItemToOrder = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);
        Int64 RoomID = SessionHelper.RoomID;
        Int64 CompanyID = SessionHelper.CompanyID;
        Int64 EnterpriseId = SessionHelper.EnterPriceID;

        #region Order Master
        public OrderController()
        {

        }

        /// <summary>
        /// Order List
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderList()
        {
            SessionHelper.RomoveSessionByKey(GetSessionKey(0));
            SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
            return View();
        }

        private string GetColumnHeaderResourceValue(string ColumnName, OrderMasterDTO obj)
        {
            switch (ColumnName)
            {
                case "@~itemnumber~@":
                    return ResItemMaster.ItemNumber;
                case "@~bin~@":
                    return ResOrder.Bin;
                case "@~description~@":
                    return ResItemMaster.Description;
                case "@~requestedquantity~@":
                    if (obj.OrderType == (int)OrderType.RuturnOrder)
                    {
                        return ResOrder.ReturnQuantity;
                    }
                    return ResOrder.RequestedQuantity;
                case "@~requireddate~@":
                    if (obj.OrderType == (int)OrderType.RuturnOrder)
                    {
                        return ResOrder.ReturnDate;
                    }
                    return ResOrder.RequiredDate;
                case "@~supplierpartno~@":
                    return ResItemMaster.SupplierPartNo;
                default:
                    return string.Empty;
            }

        }
        /// <summary>
        /// Order List
        /// </summary>
        /// <returns></returns>
        public ActionResult ReturnOrderList()
        {
            SessionHelper.RomoveSessionByKey(GetSessionKey(0));
            SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
            return View("OrderList");
        }

        private OrderType GetOrderType
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
                OrderType OrdType = (OrderType)Enum.Parse(typeof(OrderType), strOrdType, true);
                return OrdType;
            }
        }

        /// <summary>
        /// OrderMasterListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult OrderMasterListAjax(JQueryDataTableParamModel param)
        {
            SessionHelper.RomoveSessionByKey(GetSessionKey(0));
            SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
            OrderMasterDAL controller = null;
            List<OrderMasterDTO> DataFromDB = null;

            try
            {
                //int PageIndex = int.Parse(param.sEcho);
                //int PageSize = param.iDisplayLength;
                //var sortDirection = Request["sSortDir_0"];
                //var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
                //var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
                //var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                //string sortColumnName = string.Empty;
                //string sDirection = string.Empty;
                //int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
                string sortColumnName = Request["SortingField"].ToString();
                bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
                bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

                if (!string.IsNullOrEmpty(sortColumnName))
                {
                    if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                        sortColumnName = "ID desc";
                }
                else
                    sortColumnName = "ID desc";

                if (sortColumnName.ToLower().Contains("orderstatustext"))
                    sortColumnName = sortColumnName.Replace("OrderStatusText", "OrderStatusName");

                if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower().Contains("ordernumber"))
                    sortColumnName = sortColumnName.Replace("OrderNumber", "OrderNumber_ForSorting");

                //string searchQuery = string.Empty;
                int TotalRecordCount = 0;
                //string MainFilter = "";

                //if (Convert.ToString(Session["MainFilter"]).Trim().ToLower() == "true")
                //{
                //    MainFilter = "true";
                //}

                controller = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                DataFromDB = controller.GetPagedOrderMasterDataFull(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, GetOrderType, Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.UserSupplierIds, CurrentTimeZone).ToList();

                DataFromDB.ForEach(x =>
                {
                    x.IsAbleToDelete = IsRecordDeleteable(x);

                    x.RequiredDateStr = x.RequiredDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);

                    x.OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)x.OrderStatus).ToString());
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
        public PartialViewResult _CreateOrder()
        {
            return PartialView();
        }

        public ActionResult DownloadOrdersDocument(List<Guid> lstGuids)
        {
            List<string> retData = new List<string>();
            foreach (var reqguid in lstGuids)
            {
                OrderImageDetailDAL orderImageDetail = new OrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                OrderMasterDAL orderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                var lstimagedetail = orderImageDetail.GetorderImagesByGuidPlain(reqguid);
                string orderFilePath = SiteSettingHelper.OrderFilePaths;  //Settinfile.Element("WorkOrderFilePaths").Value;
                orderFilePath = orderFilePath.Replace("~", string.Empty);
                Guid reqGUID = Guid.Empty;
                OrderMasterDTO orderMasterDTO = new OrderMasterDTO();
                string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath, "");
                baseURL = SessionHelper.CurrentDomainURL;
                string OrderImagePath = baseURL + orderFilePath + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/" + orderMasterDTO.ID;
                if (Guid.TryParse(reqguid.ToString(), out reqGUID))
                {
                    orderMasterDTO = orderMasterDAL.GetOrderByGuidPlain(reqGUID);
                    OrderImagePath = baseURL + orderFilePath + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/" + orderMasterDTO.ID;
                }

                foreach (OrderImageDetail img in lstimagedetail)
                {
                    retData.Add(OrderImagePath + "/" + img.ImageName);
                }
            }

            //return File(data, "application/csv", dtoModuleDetail.ResourceModuleName + "_" + dtoModuleDetail.PageName + "_MobileRes_" + DateTimeUtility.DateTimeNow.ToString("yyyyMMddHHmmss") + ".csv");
            return Json(new { Message = "Sucess", Status = true, ReturnFiles = retData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrderNumber(Int64 SupplierID, Int64 tOrderStatus)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            RoomDAL objRoomDAL = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            OrderMasterDAL objOrderDAL = null;
            OrderMasterDTO objDTO = null;
            SupplierMasterDTO defaultSupplierDTO = null;
            SupplierMasterDAL objSupDAL = null;
            try
            {
                objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

                Int64 DefualtRoomSupplier = SupplierID;
                if (DefualtRoomSupplier == 0)
                {
                    objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    //   RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,DefaultSupplierID";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                    if (objRoomDTO != null)
                        DefualtRoomSupplier = objRoomDTO.DefaultSupplierID.GetValueOrDefault(0);
                    else
                        DefualtRoomSupplier = 0;
                }
                defaultSupplierDTO = objSupDAL.GetSupplierByIDPlain(DefualtRoomSupplier);
                objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                objAutoNumber = objAutoSeqDAL.GetNextOrderNumber(RoomID, CompanyID, DefualtRoomSupplier, EnterpriseId);
                string orderNumber = string.Empty;

                if (objAutoNumber != null && objAutoNumber.OrderNumber != null)
                    orderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;

                int ReleaseNo = 1;

                if (!string.IsNullOrWhiteSpace(orderNumber))
                {
                    objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                    var maximumReleaseNumberByOrderNo = objOrderDAL.GetMaximumReleaseNoByOrderNumber(RoomID, CompanyID, orderNumber, GetOrderType);
                    if (defaultSupplierDTO != null)
                    {
                        if (defaultSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 1)
                        {
                            if (!string.IsNullOrWhiteSpace(defaultSupplierDTO.POAutoNrReleaseNumber))
                            {
                                ReleaseNo = Convert.ToInt32(defaultSupplierDTO.POAutoNrReleaseNumber) + 1;
                            }
                            else
                            {
                                ReleaseNo = 1;
                            }
                        }
                        else
                        {
                            if (maximumReleaseNumberByOrderNo > 0)
                                ReleaseNo = maximumReleaseNumberByOrderNo + 1;
                        }
                    }
                    else
                    {
                        if (maximumReleaseNumberByOrderNo > 0)
                            ReleaseNo = maximumReleaseNumberByOrderNo + 1;
                    }
                }

                int intOrderType = (int)GetOrderType;
                objDTO = new OrderMasterDTO()
                {
                    OrderNumber = orderNumber,

                    ReleaseNumber = ReleaseNo.ToString(),
                    LastUpdated = DateTimeUtility.DateTimeNow,
                    AutoOrderNumber = objAutoNumber,
                    OrderNumber_ForSorting = objAutoNumber.OrderNumberForSorting,
                    IsBlanketOrder = objAutoNumber.IsBlanketPO,
                    OrderStatus = Convert.ToInt32(tOrderStatus),
                    OrderType = intOrderType,
                };

                return PartialView("_OrderNumberLI", objDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAutoNumber = null;
                objRoomDAL = null;
                objAutoSeqDAL = null;
                objOrderDAL = null;
                objDTO = null;
            }
        }

        /// <summary>
        /// Get Release No
        /// </summary>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        public JsonResult GetReleaseNumber(string OrderNumber)
        {
            OrderMasterDAL objOrderDAL = null;
            try
            {
                int ReleaseNo = 1;
                if (!string.IsNullOrWhiteSpace(OrderNumber))
                {
                    objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                    var maximumReleaseNumberByOrderNo = objOrderDAL.GetMaximumReleaseNoByOrderNumber(RoomID, CompanyID, OrderNumber, GetOrderType);

                    if (maximumReleaseNumberByOrderNo > 0)
                        ReleaseNo = maximumReleaseNumberByOrderNo + 1;
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
            }

        }

        public JsonResult GetOrderReleaseNumberForCart(string OrderNumber, string OrderType)
        {
            OrderMasterDAL objOrderDAL = null;
            try
            {
                int ReleaseNo = 1;
                if (!string.IsNullOrWhiteSpace(OrderNumber))
                {
                    objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                    var orderType = (OrderType)Enum.Parse(typeof(OrderType), OrderType, true);
                    var maximumReleaseNumberByOrderNo = objOrderDAL.GetMaximumReleaseNoByOrderNumber(RoomID, CompanyID, OrderNumber, orderType);

                    if (maximumReleaseNumberByOrderNo > 0)
                        ReleaseNo = maximumReleaseNumberByOrderNo + 1;
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
            }

        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderCreate()
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            RoomDAL objRoomDAL = null;
            OrderMasterDAL objOrderDAL = null;
            OrderMasterDTO objDTO = null;
            IEnumerable<SupplierBlanketPODetailsDTO> objSuppBlnkPOList = null;
            List<CustomerMasterDTO> lstCust = null;
            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
            SupplierMasterDTO defaultSupplierDTO = null;
            CustomerMasterDAL objCustDAL = null;
            SupplierMasterDAL objSupDAL = null;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

            try
            {
                SessionHelper.RomoveSessionByKey(GetSessionKey(0));
                SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
                SessionHelper.RomoveSessionByKey("AddItemToOrder_" + RoomID + "_" + CompanyID);
                SessionHelper.RomoveSessionByKey("AddItemToReturnOrder_" + RoomID + "_" + CompanyID);
                int DefaultOrderRequiredDays = 0;
                Int64 DefualtRoomSupplier = 0;
                string orderNumber = string.Empty;
                if (DefualtRoomSupplier == 0)
                {
                    objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,DefaultSupplierID,NextOrderNo";
                    RoomDTO objRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                    DefualtRoomSupplier = objRoom.DefaultSupplierID.GetValueOrDefault(0);
                    if (SessionHelper.UserSuppliers.Count < 1 || (SessionHelper.UserSuppliers.Count > 1 && SessionHelper.UserSuppliers.Contains(DefualtRoomSupplier)))
                    {
                        defaultSupplierDTO = objSupDAL.GetSupplierByIDPlain(DefualtRoomSupplier);
                    }
                    else
                    {
                        orderNumber = (string.IsNullOrEmpty(objRoom.NextOrderNo)) ? string.Empty : objRoom.NextOrderNo;

                    }
                }

                if (orderNumber == string.Empty)
                {
                    objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                    objAutoNumber = objAutoSeqDAL.GetNextOrderNumber(RoomID, CompanyID, DefualtRoomSupplier, EnterpriseId);
                    orderNumber = objAutoNumber.OrderNumber;
                }

                if (orderNumber != null && (!string.IsNullOrEmpty(orderNumber)))
                {
                    orderNumber = orderNumber.Length > 22 ? orderNumber.Substring(0, 22) : orderNumber;
                }
                int ReleaseNo = 1;

                if (!string.IsNullOrWhiteSpace(orderNumber))
                {
                    objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                    var maximumReleaseNumberByOrderNo = objOrderDAL.GetMaximumReleaseNoByOrderNumber(RoomID, CompanyID, orderNumber, GetOrderType);
                    if (defaultSupplierDTO != null)
                    {
                        if (defaultSupplierDTO.POAutoSequence.GetValueOrDefault(0) == 1)
                        {
                            if (!string.IsNullOrWhiteSpace(defaultSupplierDTO.POAutoNrReleaseNumber))
                            {
                                ReleaseNo = Convert.ToInt32(defaultSupplierDTO.POAutoNrReleaseNumber) + 1;
                            }
                            else
                            {
                                ReleaseNo = 1;
                            }
                        }
                        else
                        {
                            if (maximumReleaseNumberByOrderNo > 0)
                                ReleaseNo = maximumReleaseNumberByOrderNo + 1;
                        }
                    }
                    else
                    {
                        if (maximumReleaseNumberByOrderNo > 0)
                            ReleaseNo = maximumReleaseNumberByOrderNo + 1;
                    }
                }

                if (defaultSupplierDTO != null)
                {
                    DefaultOrderRequiredDays = defaultSupplierDTO.DefaultOrderRequiredDays.GetValueOrDefault(0);
                }

                objDTO = new OrderMasterDTO()
                {
                    RequiredDate = datetimetoConsider.AddDays(DefaultOrderRequiredDays),
                    OrderNumber = orderNumber,
                    OrderStatus = (int)OrderStatus.UnSubmitted,
                    ReleaseNumber = ReleaseNo.ToString(),
                    LastUpdated = DateTimeUtility.DateTimeNow,
                    Created = DateTimeUtility.DateTimeNow,
                    Supplier = DefualtRoomSupplier,
                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    LastUpdatedBy = SessionHelper.UserID,
                    CompanyID = CompanyID,
                    Room = RoomID,
                    RoomName = SessionHelper.RoomName,
                    UpdatedByName = SessionHelper.UserName,
                    OrderDate = datetimetoConsider,
                    AutoOrderNumber = objAutoNumber,
                    IsBlanketOrder = objAutoNumber != null ? objAutoNumber.IsBlanketPO : false,
                    OrderType = (int)GetOrderType,
                    OrderNumber_ForSorting = objAutoNumber != null ? objAutoNumber.OrderNumberForSorting : orderNumber.ToString().PadLeft(11, '0'),
                    IsOnlyFromUI = true,
                    IsEDIOrder = false,
                    IsOrderReleaseNumberEditable = defaultSupplierDTO != null ? defaultSupplierDTO.IsOrderReleaseNumberEditable : false
                };

                //Set as Default First Active Blanket po 
                if (objDTO.IsBlanketOrder)
                {
                    objSuppBlnkPOList = (from x in objAutoNumber.BlanketPOs
                                         where x != null
                                         && Convert.ToDateTime(x.StartDate.GetValueOrDefault(DateTime.MinValue).ToShortDateString()) <= Convert.ToDateTime(DateTimeUtility.DateTimeNow.ToShortDateString())
                                         && Convert.ToDateTime(x.Enddate.GetValueOrDefault(DateTime.MinValue).ToShortDateString()) >= Convert.ToDateTime(DateTimeUtility.DateTimeNow.ToShortDateString())
                                         select x);

                    if (objSuppBlnkPOList != null && objSuppBlnkPOList.Count() > 0)
                    {
                        objDTO.BlanketOrderNumberID = objSuppBlnkPOList.FirstOrDefault().ID;
                    }
                }

                objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
                ViewBag.UDFs = GetUDFDataPageWise("OrderMaster");

                foreach (var i in ViewBag.UDFs)
                {
                    string _UDFColumnName = (string)i.UDFColumnName;
                    ViewData[_UDFColumnName] = i.UDFDefaultValue;
                }

                objCustDAL = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                ViewBag.OrderStatusList = GetOrderStatusList(objDTO, "create");
                lstCust = objCustDAL.GetCustomersByRoomID(RoomID, CompanyID).OrderBy(x => x.Customer).ToList();
                lstCust.Insert(0, null);
                objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

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

                lstSupplier.Insert(0, null);
                ViewBag.SupplierList = lstSupplier;
                objDTO.SupplierAccountGuid = Guid.Empty;
                SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
                ViewBag.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(Convert.ToInt64(objDTO.Supplier), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                List<ShipViaDTO> lstShipVia = new ShipViaDAL(SessionHelper.EnterPriseDBName).GetShipViaByRoomIDPlain(RoomID, CompanyID).OrderBy(x => x.ShipVia).ToList();
                lstShipVia.Insert(0, new ShipViaDTO());
                //List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetAllRecords(RoomID, CompanyID, false, false).OrderBy(t => t.StagingName).ToList();
                List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetMaterialStaging(RoomID, CompanyID, false, false, string.Empty, null).OrderBy(t => t.StagingName).ToList();
                lstStaging.Insert(0, new MaterialStagingDTO());
                VenderMasterDAL objVendorDAL = new VenderMasterDAL(SessionHelper.EnterPriseDBName);
                List<VenderMasterDTO> lstVendor = objVendorDAL.GetVenderByRoomIDPlain(RoomID, CompanyID).OrderBy(x => x.Vender).ToList();
                lstVendor.Insert(0, null);
                ViewBag.StagingList = lstStaging;
                ViewBag.ShipViaList = lstShipVia;
                ViewBag.CustomerList = lstCust;
                ViewBag.VendorList = lstVendor;

                return PartialView("_CreateOrder", objDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAutoNumber = null;
                objAutoSeqDAL = null;
                objRoomDAL = null;
                objOrderDAL = null;
                objDTO = null;
                objSuppBlnkPOList = null;
                lstCust = null;
                lstSupplier = null;
                objCustDAL = null;
                objSupDAL = null;
            }

        }
        public ActionResult ShowData(Int64 SupplierID)
        {

            List<SupplierAccountDetailsDTO> lstSupplierAccountDetails = new List<SupplierAccountDetailsDTO>();
            SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);

            lstSupplierAccountDetails = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(SupplierID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            return Json(lstSupplierAccountDetails, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSupplierDefaultOrderRequiredDays(Int64 SupplierID)
        {
            CultureInfo RoomCulture = new CultureInfo("en-US");
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByIDPlain(SupplierID);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
            DateTime? _OrderRequiredDate = datetimetoConsider;
            if (objSupplierMasterDTO != null)
            {
                // _OrderRequiredDate = DateTime.UtcNow.AddDays(objSupplierMasterDTO.DefaultOrderRequiredDays.GetValueOrDefault(0));
                _OrderRequiredDate = datetimetoConsider.AddDays(objSupplierMasterDTO.DefaultOrderRequiredDays.GetValueOrDefault(0));
            }


            var ObjReturn = new
            {
                OrderRequiredDate = _OrderRequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture),
                IsOrderReleaseNumberEditable = objSupplierMasterDTO.IsOrderReleaseNumberEditable
            };

            return Json(ObjReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierApproveRight(Int64 SupplierID)
        {
            bool blFlag = CheckSupplierAproveRight(SupplierID);

            var ObjReturn = new { IsSupplierRight = blFlag };

            return Json(ObjReturn, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetDefaultAccount(Int64 SupplierID)
        {
            List<SupplierAccountDetailsDTO> lstSupplierAccountDetails = new List<SupplierAccountDetailsDTO>();
            SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
            lstSupplierAccountDetails = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(SupplierID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            if (lstSupplierAccountDetails != null && lstSupplierAccountDetails.Count() > 0 && lstSupplierAccountDetails.Where(s => s.IsDefault == true).Any())
            {
                return Json(new { SuppAccGuid = lstSupplierAccountDetails.Where(s => s.IsDefault == true).FirstOrDefault().GUID.ToString(), Success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SuppAccGuid = "", Success = true }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderEdit(Int64 ID, string callfor = "", bool IsArchivedOrder = false)
        {
            OrderMasterDAL obj = null;
            OrderMasterDTO objDTO = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            List<SelectListItem> lstOrderStatus = null;
            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
            SupplierMasterDAL objSupDAL = null;
            MaterialStagingDTO objMSDTO = null;
            MaterialStagingDAL objMSDAL = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(callfor))
                {
                    if ((callfor ?? string.Empty).ToLower().Contains("editorderitem"))
                    {
                        ViewBag.CallFor = "editOrderItem";
                    }
                    else if ((callfor ?? string.Empty).ToLower().Contains("uncloseorder"))
                    {
                        ViewBag.CallFor = "UnCloseOrder";
                    }
                    else
                    {
                        ViewBag.CallFor = "";
                    }
                }
                else
                {
                    ViewBag.CallFor = "";
                }

                SessionHelper.RomoveSessionByKey(GetSessionKey(ID));
                SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(ID));
                SessionHelper.RomoveSessionByKey("AddItemToOrder_" + RoomID + "_" + CompanyID);
                SessionHelper.RomoveSessionByKey("AddItemToReturnOrder_" + RoomID + "_" + CompanyID);
                bool IsArchived = IsArchivedOrder;
                bool IsDeleted = false;
                if (Request["IsArchived"] != null && !string.IsNullOrEmpty(Request["IsArchived"].ToString()))
                    IsArchived = bool.Parse(Request["IsArchived"].ToString());
                if (Request["IsDeleted"] != null && !string.IsNullOrEmpty(Request["IsDeleted"].ToString()))
                    IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

                obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                objDTO = IsArchived ? obj.GetArchivedOrderByIdFull(ID) : obj.GetOrderByIdFull(ID);
                objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
                objDTO.IsArchived = IsArchived;
                if (objDTO.BlanketOrderNumberID > 0)
                {
                    objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                    objDTO.AutoOrderNumber = objAutoSeqDAL.GetNextOrderNumber(RoomID, CompanyID, objDTO.Supplier.GetValueOrDefault(0), EnterpriseId);
                    objDTO.IsBlanketOrder = objDTO.AutoOrderNumber.IsBlanketPO;
                }

                objDTO.IsOnlyFromUI = true;
                bool isOrderHeaderEdit = true;

                if (objDTO.OrderStatus >= (int)OrderStatus.Submitted)
                    isOrderHeaderEdit = false;

                ViewBag.UDFs = GetUDFDataPageWise("OrderMaster", isOrderHeaderEdit);
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;

                if (objDTO.IsDeleted || objDTO.IsArchived.GetValueOrDefault(false))
                {
                    objDTO.IsRecordNotEditable = true;
                    objDTO.IsOnlyStatusUpdate = false;
                    objDTO.IsAbleToDelete = true;
                }

                if (objDTO.OrderStatus >= (int)OrderStatus.Approved || objDTO.IsRecordNotEditable)
                {
                    lstOrderStatus = GetOrderStatusList(objDTO, "");
                }
                else
                    lstOrderStatus = GetOrderStatusList(objDTO, "edit");

                if (IsChangeOrder && Convert.ToString(Request["IsChangeOrder"]) == "true")
                {
                    Int64 NewChangeOrderRevisionNo = 0;
                    Int64.TryParse(Convert.ToString(Request["ChangeOrderRevisionNo"]), out NewChangeOrderRevisionNo);
                    if (objDTO.ChangeOrderRevisionNo.GetValueOrDefault(0) < NewChangeOrderRevisionNo)
                    {
                        objDTO.IsRecordNotEditable = false;
                        objDTO.ChangeOrderRevisionNo = NewChangeOrderRevisionNo;
                        objDTO.IsChangeOrderClick = true;
                        lstOrderStatus = lstOrderStatus.Where(x => x.Value == "1").ToList();
                    }
                }

                objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

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

                bool IsableToUnApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowAnOrderToBeUnapprove);
                SupplierMasterDTO oSupplier = objSupDAL.GetSupplierByIDPlain(objDTO.Supplier.GetValueOrDefault(0));

                if (oSupplier != null && oSupplier.IsSendtoVendor && IsableToUnApprove && objDTO.OrderStatus == (int)OrderStatus.Approved)
                {
                    objDTO.IsOnlyStatusUpdate = true;
                    lstOrderStatus = lstOrderStatus.Where(x => x.Value == "1" || x.Value == "3" || x.Value == "8").ToList();
                }

                lstSupplier.Insert(0, null);
                ViewBag.OrderStatusList = lstOrderStatus;
                ViewBag.SupplierList = lstSupplier;

                if (objDTO.MaterialStagingGUID != null)
                {
                    objMSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                    objMSDTO = objMSDAL.GetRecord(objDTO.MaterialStagingGUID.Value, RoomID, CompanyID);
                    objDTO.StagingDefaultLocation = objMSDTO.BinID;
                    objDTO.StagingName = objMSDTO.StagingName;
                }

                objDTO.RequiredDateStr = objDTO.RequiredDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                List<ShipViaDTO> lstShipVia = new ShipViaDAL(SessionHelper.EnterPriseDBName).GetShipViaByRoomIDPlain(RoomID, CompanyID).OrderBy(x => x.ShipVia).ToList();
                lstShipVia.Insert(0, new ShipViaDTO());
                //List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetAllRecords(RoomID, CompanyID, false, false).OrderBy(t => t.StagingName).ToList();
                List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetMaterialStaging(RoomID, CompanyID, false, false, string.Empty, null).OrderBy(t => t.StagingName).ToList();
                lstStaging.Insert(0, new MaterialStagingDTO());
                CustomerMasterDAL objCustDAL = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                List<CustomerMasterDTO> lstCust = objCustDAL.GetCustomersByRoomID(RoomID, CompanyID).OrderBy(x => x.Customer).ToList();
                lstCust.Insert(0, null);
                VenderMasterDAL objVendorDAL = new VenderMasterDAL(SessionHelper.EnterPriseDBName);
                List<VenderMasterDTO> lstVendor = objVendorDAL.GetVenderByRoomIDPlain(RoomID, CompanyID).OrderBy(x => x.Vender).ToList();
                lstVendor.Insert(0, null);
                ViewBag.StagingList = lstStaging;
                ViewBag.ShipViaList = lstShipVia;
                ViewBag.CustomerList = lstCust;
                ViewBag.VendorList = lstVendor;
                SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
                ViewBag.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(objDTO.Supplier.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);

                if (objDTO.Supplier.GetValueOrDefault(0) > 0)
                {
                    objDTO.IsSupplierApprove = CheckSupplierAproveRight(objDTO.Supplier.GetValueOrDefault(0));
                }

                return PartialView("_CreateOrder", objDTO);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                obj = null;
                objDTO = null;
                objAutoSeqDAL = null;
                lstOrderStatus = null;
                lstSupplier = null;
                objSupDAL = null;
                objMSDTO = null;
                objMSDAL = null;
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
        public JsonResult SaveOrder(OrderMasterDTO objDTO)
        {

            // ignore validation in case of closed order and edit line item
            if (Request.Form["hdnIsEditOrderLineItems"] != "true")
            {
                var ignoreProperty = new List<string>() { };
                var valResult = DTOCommonUtils.ValidateDTO<OrderMasterDTO>(objDTO, ControllerContext, ignoreProperty);

                if (valResult.HasErrors())
                {
                    return Json(new { Message = ResMessage.InvalidModel, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }

            //if (!ModelState.IsValid)
            //{
            //    return Json(new { Message = ResMessage.InvalidModel, Status = "fail" }, JsonRequestBehavior.AllowGet);
            //}


            OrderMasterDAL obj = null;
            CommonDAL objCDAL = null;
            List<OrderDetailsDTO> LineItemsList = null;
            SupplierMasterDAL objSupDAL = null;
            List<OrderDetailsDTO> lstOrdDetailDTO = null;

            //try
            //{
            objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objSuppMast = new SupplierMasterDTO();
            objSuppMast = objSupDAL.GetSupplierByIDPlain(objDTO.Supplier.GetValueOrDefault(0));
            long SessionUserId = SessionHelper.UserID;
            string message = "";
            string status = "";

            obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            bool isUpdateOrderStatus = false;
            int? orderStatus = 0;

            if (string.IsNullOrEmpty(objDTO.OrderNumber))
            {
                message = string.Format(ResMessage.Required, ResOrder.OrderNumber);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(objDTO.OrderNumber) && objDTO.OrderNumber != null)
            {
                if (objDTO.OrderNumber.Length > 22)
                {
                    message = ResOrder.OrderNumberLengthUpto22Char;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
            }

            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, "OrderMaster", SessionHelper.RoomID);
            CommonDAL commonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string OrdersUDFRequier = string.Empty;
            commonDAL.CheckUDFIsRequired("OrderMaster", objDTO.UDF1, objDTO.UDF2, objDTO.UDF3, objDTO.UDF4, objDTO.UDF5, out OrdersUDFRequier, CompanyID, RoomID, EnterpriseId, SessionHelper.UserID);

            if (!string.IsNullOrEmpty(OrdersUDFRequier))
            {
                return Json(new { Message = OrdersUDFRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = RoomID;
            objDTO.RequiredDate = DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            objDTO.StagingID = objCDAL.GetOrInsertMaterialStagingIDByName(objDTO.StagingName, SessionHelper.UserID, RoomID, CompanyID);
            objDTO.MaterialStagingGUID = objCDAL.GetOrInsertMaterialStagingGUIDByName(objDTO.StagingName, SessionHelper.UserID, RoomID, CompanyID);
            objDTO.ShipVia = objCDAL.GetOrInsertShipVaiIDByName(objDTO.ShipViaName, SessionHelper.UserID, RoomID, CompanyID);
            objDTO.CustomerID = null;

            if (!string.IsNullOrWhiteSpace(objDTO.CustomerName))
            {
                bool allowCustomerInsert = false;
                allowCustomerInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                if (allowCustomerInsert)
                {
                    CustomerMasterDTO objCustomerMasterDTO = objCDAL.GetOrInsertCustomerGUIDByName(objDTO.CustomerName, SessionHelper.UserID, RoomID, CompanyID);

                    if (objCustomerMasterDTO != null)
                    {
                        objDTO.CustomerGUID = objCustomerMasterDTO.GUID;
                    }
                }
                else
                {
                    CustomerMasterDAL objDAL = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                    CustomerMasterDTO objCustomerMasterDTO = objDAL.GetCustomerByName(objDTO.CustomerName, RoomID, CompanyID);
                    if (objCustomerMasterDTO == null || objCustomerMasterDTO.GUID == null && objCustomerMasterDTO.GUID == Guid.Empty)
                    {
                        message = ResMessage.NoRightsToInsertCustomer;
                        status = "fail";
                        return Json(new { Message = message.ToString(), Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        objDTO.CustomerGUID = objCustomerMasterDTO.GUID;
                    }
                }
            }
            objDTO.ShippingVendor = objCDAL.GetOrInsertVendorIDByName(objDTO.ShippingVendorName, SessionHelper.UserID, RoomID, CompanyID);

            if (string.IsNullOrEmpty(objDTO.OrderNumber_ForSorting))
            {
                objDTO.OrderNumber_ForSorting = objDTO.OrderNumber;
            }


            int MaxOrderSizeOfSupplier = objSupDAL.GetSupplierMaxOrderSize(objDTO.Supplier.GetValueOrDefault(0), RoomID, CompanyID, objSuppMast);

            if (MaxOrderSizeOfSupplier > 0)
            {
                LineItemsList = GetLineItemsFromSession(objDTO.ID);
                if (LineItemsList != null && LineItemsList.Count > 0)
                {
                    if (MaxOrderSizeOfSupplier < LineItemsList.Count)
                    {
                        message = string.Format(ResOrder.MaxOrderSizeForSupplier, MaxOrderSizeOfSupplier);
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            //----------------------Check For Order Number Duplication----------------------
            //
            string strOK = string.Empty;
            //RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);

            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,IsAllowOrderDuplicate,POAutoSequence,PreventMaxOrderQty";
            RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

            if (roomDTO.IsAllowOrderDuplicate != true)
            {
                if (obj.IsOrderNumberDuplicateById(objDTO.OrderNumber, objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID))
                {
                    strOK = "duplicate";
                }
            }

            if (objSuppMast.IsOrderReleaseNumberEditable && obj.IsOrderReleaseDuplicate(objDTO.OrderNumber, objDTO.ReleaseNumber, objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID))
            {
                strOK = "duplicate";
            }

            if (strOK == "duplicate")
            {
                message = string.Format(ResMessage.DuplicateMessage, ResOrder.OrderNumber, objDTO.OrderNumber);
                status = "duplicate";
            }
            else
            {
                if (objDTO.ID == 0)
                {
                    //objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    //objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);

                    if (roomDTO.POAutoSequence.GetValueOrDefault(0) == 0)
                    {
                        var orderCount = obj.GetCountOfOrderByOrderNumber(RoomID, CompanyID, objDTO.OrderNumber);
                        objDTO.ReleaseNumber = Convert.ToString(orderCount + 1);
                    }

                    objDTO.GUID = Guid.NewGuid();
                    objDTO.WhatWhereAction = "Order";
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    if (objDTO.OrderStatus == (int)OrderStatus.Submitted)
                    {
                        objDTO.RequesterID = SessionHelper.UserID;
                    }
                    else if (objDTO.OrderStatus == (int)OrderStatus.Approved)
                    {
                        objDTO.ApproverID = SessionHelper.UserID;
                    }

                    objDTO = obj.InsertOrder(objDTO, SessionUserId);
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
                    double OrderPrice = 0;
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

                    if (lstOrdDetailDTO != null && lstOrdDetailDTO.Count > 0)
                    {

                        eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
                        List<UDFDTO> UDFDataFromDB = objUDFDAL.GetRequiredUDFsByUDFTableNamePlain("OrderDetails", SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                        List<Guid> lstItemGUID = lstOrdDetailDTO.Where(d => d.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).Select(x => x.ItemGUID.GetValueOrDefault(Guid.Empty)).ToList();
                        string strItemGUIDs = string.Join(",", lstItemGUID.ToArray());
                        List<ItemMasterDTO> lstOfOrderLineItem = new List<ItemMasterDTO>();
                        lstOfOrderLineItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidsNormal(strItemGUIDs, SessionHelper.RoomID, SessionHelper.CompanyID); //orderuomvalue and costuomvalue
                        BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        List<BinMasterDTO> lstOfOrderLineItemBin = new List<BinMasterDTO>();
                        string strBinNumbers = string.Join(",", lstOrdDetailDTO.Where(x => (x.BinName ?? string.Empty) != string.Empty).Select(b => b.BinName).Distinct());
                        lstOfOrderLineItemBin = objBinMasterDAL.GetAllBinMastersByBinList(strBinNumbers, SessionHelper.RoomID, SessionHelper.CompanyID);

                        foreach (OrderDetailsDTO objOrderDetail in lstOrdDetailDTO)
                        {
                            string ordDetailUDFReq = string.Empty;
                            commonDAL.CheckUDFIsRequiredLight(UDFDataFromDB, objOrderDetail.UDF1, objOrderDetail.UDF2, objOrderDetail.UDF3, objOrderDetail.UDF4, objOrderDetail.UDF5, out ordDetailUDFReq, CompanyID, RoomID, EnterpriseId, SessionHelper.UserID);

                            if (!string.IsNullOrEmpty(ordDetailUDFReq))
                                OrderDetailsUDFRequier += string.Format(ResOrder.OrderDetailUDFRequire, ordDetailUDFReq, objOrderDetail.ItemNumber);

                            if (!string.IsNullOrEmpty(OrderDetailsUDFRequier))
                                break;

                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            if (lstOfOrderLineItem != null && lstOfOrderLineItem.Any())
                            {
                                objItemMasterDTO = lstOfOrderLineItem.Where(x => x.GUID == (objOrderDetail.ItemGUID ?? Guid.Empty)).FirstOrDefault();
                            }

                            if (objItemMasterDTO != null)
                            {
                                if (objItemMasterDTO.OrderUOMValue == null || objItemMasterDTO.OrderUOMValue <= 0)
                                    objItemMasterDTO.OrderUOMValue = 1;
                            }

                            if (objOrderDetail != null && objOrderDetail.ApprovedQuantity != null
                                && objOrderDetail.ApprovedQuantity > 0)
                            {
                                if (objDollarLimt != null && objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && IsApprove && objDTO.OrderStatus == (int)OrderStatus.Approved
                                   && ItemApprovedQuantity > 0 && objOrderDetail.ApprovedQuantity > (ItemApprovedQuantity))
                                {
                                    return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanPerOrderItemQtyApprovalLimit, objOrderDetail.ApprovedQuantity, ItemApprovedQuantity), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                }
                            }

                            if (objDTO.OrderType.GetValueOrDefault(1) == (int)OrderType.Order && objItemMasterDTO != null)
                            {
                                BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                                objBinMasterDTO = lstOfOrderLineItemBin.Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty && x.ItemGUID == objItemMasterDTO.GUID && (x.BinNumber ?? string.Empty).ToLower() == (objOrderDetail.BinName ?? string.Empty).ToLower()).FirstOrDefault();
                                if (objBinMasterDTO == null || objBinMasterDTO.ID <= 0)
                                {
                                    objBinMasterDTO = objBinMasterDAL.GetItemBinPlain(objItemMasterDTO.GUID, objOrderDetail.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                                }

                                double Modulo = 0;
                                string strMsg = ResOrderDetails.SaveErrorMsgApprovedQTY;

                                if (objBinMasterDTO != null && objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                {
                                    objBinMasterDTO.DefaultReorderQuantity = (objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0) <= 0 ? 1 : objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0));
                                }

                                if (objItemMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                {
                                    if (objBinMasterDTO != null && objBinMasterDTO.DefaultReorderQuantity > 0)
                                    {
                                        objItemMasterDTO.DefaultReorderQuantity = objBinMasterDTO.DefaultReorderQuantity;
                                    }
                                    else
                                    {
                                        objItemMasterDTO.DefaultReorderQuantity = (objItemMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0) <= 0 ? 1 : objItemMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0));
                                    }
                                }

                                if (objOrderDetail != null && objOrderDetail.ApprovedQuantity != null && objOrderDetail.ApprovedQuantity > 0)
                                {
                                    if (objBinMasterDTO != null && objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                    {
                                        double binDefault = objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0);
                                        if (objItemMasterDTO.IsAllowOrderCostuom)
                                        {
                                            Modulo = ((objOrderDetail.ApprovedQuantity ?? 0) * (objItemMasterDTO.OrderUOMValue ?? 0)) % (objBinMasterDTO.DefaultReorderQuantity ?? 1);
                                            binDefault = objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0) / objItemMasterDTO.OrderUOMValue.GetValueOrDefault(0);
                                        }
                                        else
                                            Modulo = (objOrderDetail.ApprovedQuantity ?? 0) % (objBinMasterDTO.DefaultReorderQuantity ?? 1);
                                        if (Modulo != 0)
                                        {
                                            return Json(new { Message = string.Format(ResOrder.ApprovedQtyNotMatchedWithLocationDefaultReOrderQty, objBinMasterDTO.BinNumber, binDefault, objItemMasterDTO.ItemNumber), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else if (objItemMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                    {
                                        if (objBinMasterDTO != null && objBinMasterDTO.DefaultReorderQuantity > 0 && objBinMasterDTO.DefaultReorderQuantity > objItemMasterDTO.DefaultReorderQuantity)
                                        {
                                            objItemMasterDTO.DefaultReorderQuantity = objBinMasterDTO.DefaultReorderQuantity;
                                        }
                                        if (objItemMasterDTO.IsAllowOrderCostuom)
                                            Modulo = ((objOrderDetail.ApprovedQuantity ?? 0) * (objItemMasterDTO.OrderUOMValue ?? 0)) % (objItemMasterDTO.DefaultReorderQuantity ?? 1);
                                        else
                                            Modulo = (objOrderDetail.ApprovedQuantity ?? 0) % (objItemMasterDTO.DefaultReorderQuantity ?? 1);
                                        if (Modulo != 0)
                                        {
                                            // return Json(new { Message = "Approved Quantity is not matched with Item Default Reorder Quantity. Please update Approved Quantity in multiple of " + objItemMasterDTO.DefaultReorderQuantity + " for Item#" + objItemMasterDTO.ItemNumber, Status = "fail" }, JsonRequestBehavior.AllowGet);
                                            return Json(new { Message = strMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }


                                }
                                else if (objOrderDetail != null && objOrderDetail.RequestedQuantity != null)
                                {

                                    if (objBinMasterDTO != null && objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                    {
                                        double binDefault = objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0);
                                        if (objItemMasterDTO.IsAllowOrderCostuom)
                                        {
                                            Modulo = ((objOrderDetail.RequestedQuantity ?? 0) * (objItemMasterDTO.OrderUOMValue ?? 0)) % (objBinMasterDTO.DefaultReorderQuantity ?? 1);
                                            binDefault = objBinMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0) / objItemMasterDTO.OrderUOMValue.GetValueOrDefault(0);
                                        }
                                        else
                                            Modulo = (objOrderDetail.RequestedQuantity ?? 0) % (objBinMasterDTO.DefaultReorderQuantity ?? 1);
                                        if (Modulo != 0)
                                            return Json(new { Message = string.Format(ResOrder.RequestedQtyNotMatchedWithLocationDefaultReOrderQty, objBinMasterDTO.BinNumber, binDefault, objItemMasterDTO.ItemNumber), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                    }
                                    else if (objItemMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                                    {
                                        if (objBinMasterDTO != null && objBinMasterDTO.DefaultReorderQuantity > 0 && objBinMasterDTO.DefaultReorderQuantity > objItemMasterDTO.DefaultReorderQuantity)
                                        {
                                            objItemMasterDTO.DefaultReorderQuantity = objBinMasterDTO.DefaultReorderQuantity;
                                        }
                                        if (objItemMasterDTO.IsAllowOrderCostuom)
                                            Modulo = ((objOrderDetail.RequestedQuantity ?? 0) * (objItemMasterDTO.OrderUOMValue ?? 0)) % (objItemMasterDTO.DefaultReorderQuantity ?? 1);
                                        else
                                            Modulo = (objOrderDetail.RequestedQuantity ?? 0) % (objItemMasterDTO.DefaultReorderQuantity ?? 1);
                                        if (Modulo != 0)
                                            return Json(new { Message = string.Format(ResOrder.RequestedQtyNotMatchedWithDefaultReOrderQty, objItemMasterDTO.DefaultReorderQuantity, objItemMasterDTO.ItemNumber), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                    }


                                }
                            }

                            CostUOMMasterDTO costUOM = new CostUOMMasterDTO();
                            if (objItemMasterDTO == null || objItemMasterDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }
                            else
                            {
                                costUOM.CostUOMValue = objItemMasterDTO.CostUOMValue;
                            }

                            #region WI-6215 and Other Relevant order cost related jira

                            if (objDTO.OrderStatus == (int)OrderStatus.UnSubmitted)
                                if (objDTO.OrderType == (int)OrderType.Order
                                    && objDTO.OrderStatus == (int)OrderStatus.UnSubmitted)
                                {
                                    if (objOrderDetail.ItemCostUOMValue == null
                                        || objOrderDetail.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                                    {
                                        if (objItemMasterDTO != null)
                                        {
                                            if (objItemMasterDTO.CostUOMValue == null
                                                || objItemMasterDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
                                                objOrderDetail.ItemCostUOMValue = 1;
                                            else
                                            {
                                                objOrderDetail.ItemCostUOMValue = objItemMasterDTO.CostUOMValue.GetValueOrDefault(1);
                                            }
                                        }
                                        else
                                        {
                                            objOrderDetail.ItemCostUOMValue = 1;
                                        }
                                        //objOrderDetail.ItemCostUOMValue = objItemMasterDTO.CostUOMValue.GetValueOrDefault(1);
                                    }
                                    if (objOrderDetail.ItemMarkup.GetValueOrDefault(0) == 0)
                                        if (objItemMasterDTO != null)
                                            objOrderDetail.ItemMarkup = objItemMasterDTO.Markup.GetValueOrDefault(0);
                                        else
                                            objOrderDetail.ItemMarkup = 0;
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

                            #endregion
                            if (objOrderDetail.ItemCostUOMValue == null
                                || objOrderDetail.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                objOrderDetail.ItemCostUOMValue = 1;
                            }
                            if (objItemMasterDTO != null && objOrderDetail != null)
                            {
                                objOrderDetail.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (objOrderDetail.RequestedQuantity.GetValueOrDefault(0) * (objOrderDetail.ItemCost.GetValueOrDefault(0) / objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                            : (objOrderDetail.ApprovedQuantity.GetValueOrDefault(0) * (objOrderDetail.ItemCost.GetValueOrDefault(0) / objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1))))));

                                objOrderDetail.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (objOrderDetail.RequestedQuantity.GetValueOrDefault(0) * (objOrderDetail.ItemSellPrice.GetValueOrDefault(0) / objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                            : (objOrderDetail.ApprovedQuantity.GetValueOrDefault(0) * (objOrderDetail.ItemSellPrice.GetValueOrDefault(0) / objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1))))));

                            }

                            if (objItemMasterDTO != null && objOrderDetail.ApprovedQuantity != null
                                        && objOrderDetail.ApprovedQuantity > 0)
                            {

                                if (PriseSelectionOption == 1)
                                {
                                    OrderPrice += (objOrderDetail.ItemSellPrice.GetValueOrDefault(0) * objOrderDetail.ApprovedQuantity.GetValueOrDefault(0))
                                          / (objOrderDetail.ItemCostUOMValue.GetValueOrDefault(0) > 0 ? objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1) : 1);
                                }
                                else
                                {
                                    OrderCost += (objOrderDetail.ItemCost.GetValueOrDefault(0) * objOrderDetail.ApprovedQuantity.GetValueOrDefault(0))
                                           / (objOrderDetail.ItemCostUOMValue.GetValueOrDefault(0) > 0 ? objOrderDetail.ItemCostUOMValue.GetValueOrDefault(1) : 1);
                                }
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(OrderDetailsUDFRequier))
                        return Json(new { Message = OrderDetailsUDFRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);

                    if (SessionHelper.RoleID > 0 && objDollarLimt != null && objDTO.OrderType == 1)
                    {
                        if (PriseSelectionOption == 1)
                        {
                            if (objDollarLimt.OrderLimitType == OrderLimitType.All && IsApprove && objDTO.OrderStatus == (int)OrderStatus.Approved
                                   && (UserApprovalLimit - UserUsedLimit) > 0 && OrderPrice > (UserApprovalLimit - UserUsedLimit))
                                return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanRemainingOrderApprovalLimit, OrderPrice, (UserApprovalLimit - UserUsedLimit)), Status = "fail" }, JsonRequestBehavior.AllowGet);
                            else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && IsApprove && objDTO.OrderStatus == (int)OrderStatus.Approved
                                     && UserApprovalLimit > 0 && OrderPrice > (UserApprovalLimit))
                                return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanPerOrderApprovalLimit, OrderPrice, UserApprovalLimit), Status = "fail" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (objDollarLimt.OrderLimitType == OrderLimitType.All && IsApprove && objDTO.OrderStatus == (int)OrderStatus.Approved
                                 && (UserApprovalLimit - UserUsedLimit) > 0 && OrderCost > (UserApprovalLimit - UserUsedLimit))
                                return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanRemainingOrderApprovalLimit, OrderCost, (UserApprovalLimit - UserUsedLimit)), Status = "fail" }, JsonRequestBehavior.AllowGet);
                            else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && IsApprove && objDTO.OrderStatus == (int)OrderStatus.Approved
                                     && UserApprovalLimit > 0 && OrderCost > (UserApprovalLimit))
                                return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanPerOrderApprovalLimit, OrderCost, UserApprovalLimit), Status = "fail" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    OrderMasterDTO tempOrd = new OrderMasterDTO();
                    tempOrd = obj.GetOrderByIdPlain(objDTO.ID);
                    if (tempOrd != null)
                    {
                        objDTO.AddedFrom = tempOrd.AddedFrom;
                        objDTO.ReceivedOnWeb = tempOrd.ReceivedOnWeb;

                        if (tempOrd.OrderStatus == (int)OrderStatus.UnSubmitted && objDTO.OrderStatus == (int)OrderStatus.Closed)
                            TempData["IsOrderClosedFromUnSubmitted"] = true;

                    }
                    //
                    if (objDTO.OrderStatus == (int)OrderStatus.Approved)
                    {
                        if (objDTO.Supplier != null && objDTO.Supplier.GetValueOrDefault(0) > 0)
                        {
                            UserAccessDTO objUserAccess = null;
                            if (SessionHelper.UserType == 1)
                            {
                                eTurnsMaster.DAL.UserMasterDAL objUserdal = new eTurnsMaster.DAL.UserMasterDAL();
                                objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }
                            else
                            {
                                eTurns.DAL.UserMasterDAL objUserdal = new UserMasterDAL(SessionHelper.EnterPriseDBName);
                                objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }

                            if (objUserAccess != null && !string.IsNullOrWhiteSpace(objUserAccess.SupplierIDs))
                            {
                                List<string> strSupplier = objUserAccess.SupplierIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (strSupplier != null && strSupplier.Count > 0)
                                {
                                    if (strSupplier.Contains(objDTO.Supplier.ToString()) == false)
                                    {
                                        //message = "order not allow to approve for this supplier";
                                        message = ResOrder.SupplierApprove;
                                        status = "fail";
                                        return Json(new { Message = message.ToString(), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }

                        }
                    }

                    //

                    if (objDTO.OrderStatus >= (int)OrderStatus.Submitted && objDTO.OrderStatus != (int)OrderStatus.Closed && (lstOrdDetailDTO == null || lstOrdDetailDTO.Count <= 0))
                    {
                        message = string.Format(ResOrder.OrderMustHaveOneLineItem);
                        status = "fail";
                    }
                    else
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        bool NeedtoUpdateReleaseno = false;
                        if (objDTO.OrderNumber != tempOrd.OrderNumber)
                        {
                            if (objSuppMast.POAutoSequence.GetValueOrDefault(0) == 1)
                            {
                                NeedtoUpdateReleaseno = true;
                                if (!string.IsNullOrWhiteSpace(objSuppMast.POAutoNrReleaseNumber) && objSuppMast.NextOrderNo == objDTO.OrderNumber)
                                {
                                    objDTO.ReleaseNumber = Convert.ToString(Convert.ToInt64(objSuppMast.POAutoNrReleaseNumber) + 1);
                                }
                            }
                        }

                        var orderType = (objDTO.OrderType == null || (!objDTO.OrderType.HasValue)) ? (int)OrderType.Order : objDTO.OrderType.Value;
                        // objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                        // objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);

                        if (orderType == (int)OrderType.Order && roomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder && objDTO.OrderStatus != (int)OrderStatus.Closed)
                        {
                            string validationMsg = string.Empty;
                            var isOrderLineItemsAreValid = ValidateOrderLineItemsForMaxOrderQty(objDTO.ID, out validationMsg, objDTO.OrderStatus);

                            if (!isOrderLineItemsAreValid)
                            {
                                return Json(new { Message = validationMsg, Status = "fail", ID = objDTO.ID }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        //WI-8417
                        if (orderType == (int)OrderType.Order && objDTO.OrderStatus >= (int)OrderStatus.Approved && objDTO.OrderStatus != (int)OrderStatus.Closed)
                        {
                            string validationMsgForPO = string.Empty;
                            var isValidOrderLineItems = commonDAL.ValidateOrderItemOnSupplierBlanketPO(objDTO.GUID, lstOrdDetailDTO, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, SessionHelper.UserID, out validationMsgForPO);

                            if (!isValidOrderLineItems)
                            {
                                return Json(new { Message = validationMsgForPO, Status = "fail", ID = objDTO.ID }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        if (tempOrd != null && tempOrd.RequesterID.GetValueOrDefault(0) > 0)
                        {
                            objDTO.RequesterID = tempOrd.RequesterID;
                        }
                        if (tempOrd != null && tempOrd.ApproverID.GetValueOrDefault(0) > 0)
                        {
                            objDTO.ApproverID = tempOrd.ApproverID;
                        }
                        if (tempOrd.OrderStatus == (int)OrderStatus.UnSubmitted
                            && objDTO.OrderStatus == (int)OrderStatus.Submitted)
                        {
                            objDTO.RequesterID = SessionHelper.UserID;
                        }
                        else if (tempOrd.OrderStatus == (int)OrderStatus.Submitted
                            && objDTO.OrderStatus == (int)OrderStatus.Approved)
                        {
                            objDTO.ApproverID = SessionHelper.UserID;
                        }
                        else if (tempOrd.OrderStatus == (int)OrderStatus.UnSubmitted
                            && objDTO.OrderStatus == (int)OrderStatus.Approved)
                        {
                            objDTO.RequesterID = SessionHelper.UserID;
                            objDTO.ApproverID = SessionHelper.UserID;
                        }

                        int orderStatusSupposedToBe = objDTO.OrderStatus;

                        if (tempOrd != null && (tempOrd.OrderStatus == (int)OrderStatus.UnSubmitted || tempOrd.OrderStatus == (int)OrderStatus.Submitted)
                            && objDTO.OrderStatus == (int)OrderStatus.Approved)
                        {
                            isUpdateOrderStatus = true;
                            orderStatus = (int)OrderStatus.Approved;
                            objDTO.OrderStatus = tempOrd.OrderStatus;
                        }

                        bool ReturnVal = obj.Edit(objDTO);

                        if (ReturnVal)
                        {
                            if (orderStatusSupposedToBe <= (int)OrderStatus.Approved)
                            {
                                AutoSequenceDAL objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
                                objAutoSeqDAL.UpdateNextOrderNumber(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Supplier.GetValueOrDefault(0), objDTO.OrderNumber, SessionUserId, objDTO.ReleaseNumber);
                            }
                            if (PriseSelectionOption == 1)
                            {
                                if (SessionHelper.RoleID > 0
                                       && objDollarLimt != null
                                       && objDollarLimt.OrderLimitType == OrderLimitType.All
                                       && IsApprove
                                       && orderStatusSupposedToBe == (int)OrderStatus.Approved
                                       && OrderPrice <= (UserApprovalLimit - UserUsedLimit))
                                {
                                    userDAL.SaveDollerUsedLimt(OrderPrice, SessionHelper.UserID, CompanyID, RoomID);
                                }
                            }
                            else
                            {
                                if (SessionHelper.RoleID > 0
                                    && objDollarLimt != null
                                    && objDollarLimt.OrderLimitType == OrderLimitType.All
                                    && IsApprove
                                    && orderStatusSupposedToBe == (int)OrderStatus.Approved
                                    && OrderCost <= (UserApprovalLimit - UserUsedLimit))
                                {
                                    userDAL.SaveDollerUsedLimt(OrderCost, SessionHelper.UserID, CompanyID, RoomID);
                                }
                            }

                            // As per client Mail :Short term To Dos, committed completion dates, and specific fixes needed (point :6  Eliminate “Transmit” on Order History)
                            if (orderStatusSupposedToBe == (int)OrderStatus.Approved && objSuppMast != null && !objSuppMast.IsSendtoVendor)
                            {
                                if (isUpdateOrderStatus)
                                {
                                    orderStatus = (int)OrderStatus.Transmitted;
                                }
                                //objDTO.EditedFrom = "Web";
                                //objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                //UpdateOrderToTransmited(objDTO.GUID.ToString(), "Web", "SaveOrder.ToTransmitFromApproved");
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

                //if (objDTO.OrderStatus == (int)OrderStatus.UnSubmitted && lstOrdDetailDTO != null && lstOrdDetailDTO.Count > 0)
                //{
                //    //SendMailOrderUnSubmitted
                //    SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                //    SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(objDTO.Supplier.GetValueOrDefault(0));
                //    SendMailOrderUnSubmitted(objSupplierDTO, objDTO);
                //}

            }

            Session["IsInsert"] = "True";
            return Json(new { Message = message, Status = status, ID = objDTO.ID, IsUpdateOrderStatus = isUpdateOrderStatus, OrderStatus = orderStatus, OrderGuid = objDTO.GUID }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    obj = null;
            //    objCDAL = null;
            //    LineItemsList = null;
            //    objSupDAL = null;
            //    lstOrdDetailDTO = null;
            //}
        }
        /// <summary>
        /// Send Mail After Save for Unsubmitted Order;
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetOrderMailUnsubmitted(Int64 OrderID)
        {
            try
            {
                OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                OrderMasterDTO objOrdDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdFull(OrderID);
                if (objOrdDTO != null && objOrdDTO.OrderStatus == (int)OrderStatus.UnSubmitted && objOrdDTO.NoOfLineItems != null &&
                    objOrdDTO.NoOfLineItems.GetValueOrDefault(0) > 0)
                {
                    List<OrderDetailsDTO> lstOrdDtlDTO = objDAL.GetDeletedOrUnDeletedOrderDetailByOrderGUIDPlain(objOrdDTO.GUID, objOrdDTO.Room.GetValueOrDefault(0), objOrdDTO.CompanyID.GetValueOrDefault(0), false);
                    if (lstOrdDtlDTO != null && lstOrdDtlDTO.Count > 0)
                    {
                        //SendMailOrderUnSubmitted
                        SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                        SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(objOrdDTO.Supplier.GetValueOrDefault(0));
                        SendMailOrderUnSubmitted(objSupplierDTO, objOrdDTO);
                    }
                }
                return Json(new { Message = ResOrder.MailSendSuccessfully, Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.ToString(), Status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Send Mail After Save;
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetOrderMail(Int64 OrderID, bool IsUpdateOrderStatus, int? OrderStatusToUpdate)
        {
            OrderMasterDTO objOrderDTO = null;
            OrderMasterDAL objOrdDAL = null;
            List<SupplierMasterDTO> lstSuppliers = null;
            SupplierMasterDTO objSupplierDTO = null;
            SupplierMasterDAL objSupplierDAL = null;
            try
            {
                objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

                if (IsUpdateOrderStatus && OrderStatusToUpdate.HasValue && OrderStatusToUpdate.Value >= 1)
                {
                    objOrdDAL.UpdateOrderStatus(null, OrderID, OrderStatusToUpdate.GetValueOrDefault(1));
                }

                objOrderDTO = objOrdDAL.GetOrderByIdNormal(OrderID);

                if (objOrderDTO.OrderStatus == (int)OrderStatus.Submitted)
                {
                    SendMailToApprovalAuthority(objOrderDTO);
                }
                else if (objOrderDTO.OrderStatus == (int)OrderStatus.Approved || objOrderDTO.OrderStatus == (int)OrderStatus.Transmitted)
                {
                    eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                    eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                    UserMasterDTO ReqUser = new UserMasterDTO();

                    string OrdRequesterEmailAddress = "";
                    string OrdApproverEmailAddress = "";
                    if (objOrderDTO.RequesterID.GetValueOrDefault(0) > 0)
                    {
                        ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objOrderDTO.RequesterID.GetValueOrDefault(0));
                        if (ReqUser == null)
                        {
                            ReqUser = userMasterDAL.GetUserByIdPlain(objOrderDTO.RequesterID.GetValueOrDefault(0));
                        }
                        if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                        {
                            OrdRequesterEmailAddress = ReqUser.Email;
                        }
                    }
                    if (objOrderDTO.ApproverID.GetValueOrDefault(0) > 0)
                    {
                        ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objOrderDTO.ApproverID.GetValueOrDefault(0));
                        if (ReqUser == null)
                        {
                            ReqUser = userMasterDAL.GetUserByIdPlain(objOrderDTO.ApproverID.GetValueOrDefault(0));
                        }
                        if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                        {
                            OrdApproverEmailAddress = ReqUser.Email;
                        }
                    }

                    objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    lstSuppliers = new List<SupplierMasterDTO>();
                    objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(objOrderDTO.Supplier.GetValueOrDefault(0));
                    SendMailToSupplier(objSupplierDTO, objOrderDTO,true);
                    SendMailForApprovedOrReject(objOrderDTO, "approved", OrdRequesterEmailAddress, OrdApproverEmailAddress);
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
                lstSuppliers = null;
                objSupplierDTO = null;
                objSupplierDAL = null;
            }

        }

        /// <summary>
        /// UpdateOrderToTransmited
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult UpdateOrderToTransmited(string OrderGUID, string editedFrom, string whateWhereAction)
        {
            OrderMasterDAL obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objDTO = new OrderMasterDTO() { GUID = Guid.Parse(OrderGUID), Room = RoomID, CompanyID = CompanyID, EditedFrom = editedFrom, WhatWhereAction = whateWhereAction };
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
            OrderMasterDAL obj = null;
            OrderDetailsDAL objDetailDAL = null;
            List<OrderDetailsDTO> lstOrdDetailDTO = null;
            OrderMasterDTO objDTO = null;
            try
            {
                long SessionUserId = SessionHelper.UserID;
                obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                objDTO = obj.GetOrderByIdPlain(OrderID);
                objDTO.OrderStatus = (int)OrderStatus.Closed;
                objDTO.IsOnlyFromUI = true;
                objDTO.EditedFrom = "Web";
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                bool isSave = obj.Edit(objDTO);
                if (isSave)
                {
                    objDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    lstOrdDetailDTO = objDetailDAL.GetOrderDetailByOrderGUIDPlain(objDTO.GUID, RoomID, CompanyID);
                    foreach (var item in lstOrdDetailDTO)
                    {
                        item.EditedFrom = "Web";
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDetailDAL.Edit(item, SessionUserId, EnterpriseId);
                    }

                    //Logic for Scheduler immediate 
                    if (OrderID > 0 && objDTO != null && objDTO.OrderStatus == (int)OrderStatus.Closed)
                    {
                        OrderMasterDTO objOrdDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdFull(OrderID);
                        if (objOrdDTO != null && objOrdDTO.OrderStatus == (int)OrderStatus.Closed && objOrdDTO.NoOfLineItems != null &&
                            objOrdDTO.NoOfLineItems.GetValueOrDefault(0) > 0)
                        {
                            List<OrderDetailsDTO> lstOrdDtlDTO = objDetailDAL.GetDeletedOrUnDeletedOrderDetailByOrderGUIDPlain(objOrdDTO.GUID, objOrdDTO.Room.GetValueOrDefault(0), objOrdDTO.CompanyID.GetValueOrDefault(0), false);
                            if (lstOrdDtlDTO != null && lstOrdDtlDTO.Count > 0)
                            {
                                //SendMailOrderUnSubmitted
                                SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                                SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(objOrdDTO.Supplier.GetValueOrDefault(0));
                                SendMailOrderClosed(objSupplierDTO, objOrdDTO);
                            }
                        }
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
        public JsonResult UpdateSelectedOrdersToClose(string ids)
        {

            string message = string.Empty, status = string.Empty;


            OrderMasterDAL obj = null;
            OrderDetailsDAL objDetailDAL = null;
            List<OrderDetailsDTO> lstOrdDetailDTO = null;
            OrderMasterDTO objDTO = null;
            int errorCount = 0;
            long SessionUserId = SessionHelper.UserID;
            foreach (var item in ids.Split(','))
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    long OrderID = 0;
                    if (long.TryParse(item.Trim(), out OrderID))
                    {
                        try
                        {
                            obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                            objDTO = obj.GetOrderByIdPlain(OrderID);
                            if (objDTO.OrderStatus != (int)OrderStatus.Closed)
                            {
                                objDTO.OrderStatus = (int)OrderStatus.Closed;
                                objDTO.IsOnlyFromUI = true;
                                objDTO.EditedFrom = "Web";
                                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                bool isSave = obj.Edit(objDTO);
                                if (isSave)
                                {
                                    objDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                                    lstOrdDetailDTO = objDetailDAL.GetOrderDetailByOrderGUIDPlain(objDTO.GUID, RoomID, CompanyID);
                                    foreach (var itemdetail in lstOrdDetailDTO)
                                    {
                                        itemdetail.EditedFrom = "Web";
                                        itemdetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objDetailDAL.Edit(itemdetail, SessionUserId, EnterpriseId);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
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
                message = ResOrder.SelectedOrdersClosedSuccessfully;
            }

            return Json(new { Message = message, Status = status });
        }

        [HttpPost]
        public JsonResult UpdateSomeFieldsInOrder(OrderMasterDTO order)
        {
            string message = "fail";
            bool status = false;

            if (order != null && order.ID > 0)
            {
                OrderMasterDAL ordDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                ordDAL.UpdateOrderComment(order.Comment, order.PackSlipNumber, order.ShippingTrackNumber, order.ID, SessionHelper.UserID);

                if (order.ID > 0 && order.OrderListItem != null)
                {
                    OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

                    foreach (OrderDetailsDTO item in order.OrderListItem)
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
                long SessionUserId = SessionHelper.UserID;
                OrderMasterDAL obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteOrdersByOrderIds(ids, SessionHelper.UserID, RoomID, CompanyID, SessionUserId, SessionHelper.EnterPriceID);
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        /// Get Customer Address by CustoGUID
        public JsonResult GetCustomerAddessByID(Guid CustomerGUID)
        {
            CustomerMasterDAL custMastCtrl = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            CustomerMasterDTO CustDTO = custMastCtrl.GetCustomerByGUID(CustomerGUID);
            string strAddress = string.Empty;

            if (!string.IsNullOrEmpty(CustDTO.Address))
                strAddress = CustDTO.Address;
            if (!string.IsNullOrEmpty(CustDTO.City))
                strAddress += (ResCommon.Comma + " " + CustDTO.City);

            if (!string.IsNullOrEmpty(CustDTO.State))
                strAddress += (ResCommon.Comma + " " + CustDTO.State);

            if (!string.IsNullOrEmpty(CustDTO.Country))
                strAddress += (ResCommon.Comma + " " + CustDTO.Country);

            if (!string.IsNullOrEmpty(CustDTO.ZipCode))
                strAddress += (ResCommon.Comma + " " + CustDTO.ZipCode);

            if (!string.IsNullOrEmpty(CustDTO.Email))
                strAddress += (ResCommon.Comma + " " + CustDTO.Email);

            if (!string.IsNullOrEmpty(CustDTO.Contact))
                strAddress += (ResCommon.Comma + " " + CustDTO.Contact);

            if (!string.IsNullOrEmpty(CustDTO.Phone))
                strAddress += (ResCommon.Comma + " " + CustDTO.Phone);


            return Json(new { Address = strAddress }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// IsRecordNotEditable
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool IsRecordNotEditable(OrderMasterDTO objDTO)
        {
            bool isNotEditable = false;

            bool IsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            bool IsUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
            bool IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
            if (objDTO.OrderType == (int)OrderType.RuturnOrder)
            {
                IsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                IsUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
                IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);

            }

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

            if (IsChangeOrder && !objDTO.OrderIsInReceive && objDTO.OrderStatus == (int)OrderStatus.Transmitted)
            {
                isNotEditable = false;
            }
            else if (IsUpdate || IsSubmit || IsApprove || Convert.ToString(Session["IsInsert"]) == "True" || IsInsert)
            {
                if (IsApprove || IsSubmit)
                    objDTO.IsOnlyStatusUpdate = true;
                else
                    objDTO.IsOnlyStatusUpdate = false;


                if (objDTO.OrderStatus < (int)OrderStatus.Submitted)
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
                else if (objDTO.OrderStatus >= (int)OrderStatus.Submitted)
                {
                    if (objDTO.OrderStatus == (int)OrderStatus.Submitted)
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
                    else if (objDTO.OrderStatus > (int)OrderStatus.Submitted)
                    {
                        isNotEditable = true;
                        if (objDTO.OrderStatus == (int)OrderStatus.Transmitted && IsChangeOrder)
                            isNotEditable = false;
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
        public bool IsRecordDeleteable(OrderMasterDTO objDTO)
        {
            bool IsDeleteable = false;
            bool IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
            if (objDTO.OrderType == (int)OrderType.RuturnOrder)
            {
                IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
            }
            if ((objDTO.OrderStatus < (int)OrderStatus.Approved || objDTO.OrderStatus == (int)OrderStatus.Closed) && IsDelete)
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
            List<OrderMasterDTO> objChangeOrders = null;
            OrderMasterDAL objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            objChangeOrders = objOrderDAL.GetChangeOrderDataNormal(CompanyID, RoomID, OrderGuid, SessionHelper.UserSupplierIds, (OrderType)OrdType).ToList();
            OrderMasterDTO objCurrentOrder = objOrderDAL.GetOrderByGuidNormalForChangeOrder(OrderGuid);
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
            List<OrderDetailsDTO> objOrderDetailDTO = null;
            OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            objOrderDetailDTO = objDAL.GetChangeOrderDetailData(RoomID, CompanyID, SessionHelper.UserSupplierIds, ChangeOrderMasterGuid).ToList();
            OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objOrderDTO = objOrdDAL.GetChangeOrderDataPlain(CompanyID, RoomID, SessionHelper.UserSupplierIds, ChangeOrderMasterGuid);
            objOrderDTO.OrderListItem = objOrderDetailDTO;
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
            List<OrderDetailsDTO> objOrderDetailDTO = null;
            OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            objOrderDetailDTO = objDAL.GetOrderDetailByOrderGUIDFull(ChangeOrderMasterGuid, RoomID, CompanyID).ToList();

            OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objOrderDTO = objOrdDAL.GetOrderByGuidPlain(ChangeOrderMasterGuid);
            objOrderDTO.OrderListItem = objOrderDetailDTO;
            return PartialView("_ChangeOrderDetailList", objOrderDTO);
        }

        /// <summary>
        /// TempUpdateOrderNumberForSorting
        /// </summary>
        /// <returns></returns>
        //public JsonResult TempUpdateOrderNumberForSorting()
        //{
        //    OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
        //    objOrderMasterDAL.TempFillOrderNumberSorting();
        //    return Json(new { Message = "OK", Success = true }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult UncloseOrder(string OrderGUID)
        {
            string message = string.Empty;
            string status = string.Empty;
            OrderMasterDAL obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objDTO = new OrderMasterDTO();
            objDTO = obj.GetOrderByGuidPlain(Guid.Parse(OrderGUID));

            if (objDTO != null)
            {
                objDTO.OrderStatus = (int)OrderStatus.UnSubmitted;
                objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objDTO.LastUpdatedBy = SessionHelper.UserID;

                try
                {
                    objDTO.WhatWhereAction = "Order";
                    bool IsUpdate = false;
                    objDTO.IsOnlyFromUI = true;
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.RequesterID = null;
                    objDTO.ApproverID = null;
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
        /// WI-1273: Modify the unclose process of an order
        /// </summary>
        /// <param name="OrederGuid"></param>
        /// <param name="EditType"></param>
        /// <returns></returns>
        public JsonResult NewUnCloseOrderToEdit(List<Guid> OrederGuid, string EditType)
        {
            OrderMasterDAL orderDal = null;
            OrderMasterDTO orderDTO = null;
            List<OrderDetailsDTO> orderlineItems = null;
            OrderDetailsDAL orderDetailDal = null;
            List<ReceivedOrderTransferDetailDTO> receivedDetail = null;
            ReceivedOrderTransferDetailDAL receivedOrderDAL = null;
            ItemLocationDetailsDAL itemLocationDetailDAL = null;
            string message = "Error";
            string status = "fail";
            var orderLineItemsAreInvalid = false;
            var validationMessageForOrderLineItems = string.Empty;
            var enterpriseId = SessionHelper.EnterPriceID;

            try
            {
                long SessionUserId = SessionHelper.UserID;
                if (OrederGuid != null && OrederGuid.Count > 0)
                {
                    orderDal = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                    orderDetailDal = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    if (EditType == "EditLineItems")
                    {
                        receivedOrderDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                        itemLocationDetailDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    }

                    foreach (var orderGuid in OrederGuid)
                    {
                        orderDTO = orderDal.GetOrderByGuidPlain(orderGuid);
                        if (!orderDTO.IsEDIOrder.GetValueOrDefault(false))
                        {
                            orderlineItems = orderDetailDal.GetOrderDetailByOrderGUIDPlain(orderGuid, RoomID, CompanyID);
                            if (EditType == "EditReceipt")
                            {
                                if (orderDTO.RequiredDate <= DateTime.Now)
                                    orderDTO.OrderStatus = (int)OrderStatus.TransmittedIncomplete;
                                else
                                    orderDTO.OrderStatus = (int)OrderStatus.TransmittedInCompletePastDue;

                                orderDal.Edit(orderDTO);
                                if (orderlineItems != null && orderlineItems.Count > 0)
                                {
                                    foreach (var lineItem in orderlineItems)
                                    {
                                        lineItem.EditedFrom = "Web";
                                        lineItem.LastUpdatedBy = SessionHelper.UserID;
                                        lineItem.LastUpdated = DateTimeUtility.DateTimeNow;
                                        orderDetailDal.Edit(lineItem, SessionUserId, EnterpriseId);
                                    }
                                }
                            }
                            else if (EditType == "EditLineItems")
                            {
                                bool isOrderLineItemsValid = false;
                                if (orderlineItems != null && orderlineItems.Count > 0)
                                {
                                    var validationMessage = string.Empty;
                                    isOrderLineItemsValid = ValidateOrderLineItemsForUncloseOrder(orderGuid, out validationMessage);
                                    validationMessageForOrderLineItems += validationMessage;

                                    if (isOrderLineItemsValid)
                                    {
                                        foreach (var lineItem in orderlineItems)
                                        {
                                            receivedDetail = receivedOrderDAL.GetROTDByOrderDetailGUIDFull(lineItem.GUID, RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
                                            if (receivedDetail != null && receivedDetail.Count > 0)
                                            {
                                                foreach (var receivedItem in receivedDetail)
                                                {
                                                    receivedItem.CustomerOwnedQuantity = 0;
                                                    receivedItem.ConsignedQuantity = 0;
                                                    if (orderDTO.OrderType == (int)OrderType.Order)
                                                        receivedItem.EditedFrom = "Web>>UncloseOrder";
                                                    else if (orderDTO.OrderType == (int)OrderType.RuturnOrder)
                                                        receivedItem.EditedFrom = "Web>>UncloseReturnOrder";
                                                    else
                                                        receivedItem.EditedFrom = "Web";
                                                    receivedItem.IsEDISent = false;
                                                    receivedItem.LastUpdatedBy = SessionHelper.UserID;
                                                    receivedItem.Updated = DateTimeUtility.DateTimeNow;
                                                }

                                                receivedOrderDAL.Edit(receivedDetail, SessionUserId, enterpriseId);
                                            }

                                            lineItem.ReceivedQuantity = 0;
                                            lineItem.IsEDISent = false;
                                            lineItem.EditedFrom = "Web";
                                            lineItem.LastUpdatedBy = SessionHelper.UserID;
                                            lineItem.LastUpdated = DateTimeUtility.DateTimeNow;
                                            lineItem.IsCloseItem = false;

                                            orderDetailDal.Edit(lineItem, SessionUserId, EnterpriseId);
                                        }
                                    }
                                    else
                                    {
                                        orderLineItemsAreInvalid = true;
                                    }
                                }
                                if (isOrderLineItemsValid)
                                {
                                    orderDTO.OrderStatus = (int)OrderStatus.UnSubmitted;
                                    orderDal.Edit(orderDTO);
                                }
                            }
                        }
                    }
                    if (EditType == "EditLineItems" && orderLineItemsAreInvalid)
                    {
                        message = validationMessageForOrderLineItems;
                        status = "fail";
                    }
                    else
                    {
                        message = ResOrder.OrderUnclosedSuccessfully;
                        status = "ok";
                    }
                }
                else
                {
                    message = ResOrder.NoOrdersFound;
                    status = "fail";
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = ex.Message;
                status = "fail";
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
            OrderDetailsDAL orderDetailDAL = null;
            OrderDetailsDTO orderDetail = null;
            OrderMasterDTO order = null;
            OrderMasterDAL orderDAL = null;
            int orderStatus = 0;

            try
            {
                orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                long SessionUserId = SessionHelper.UserID;
                orderDetailDAL.CloseOrderDetailItem(ids, SessionHelper.UserID, RoomID, CompanyID, SessionUserId, EnterpriseId);

                if (!string.IsNullOrEmpty(CallFrom) && CallFrom.ToLower() == "order")
                {
                    string[] orderIDs = ids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (orderIDs != null && orderIDs.Length > 0)
                    {
                        orderDetail = orderDetailDAL.GetOrderDetailByIDPlain(Int64.Parse(orderIDs[0]), RoomID, CompanyID);
                        orderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                        order = orderDAL.GetOrderByGuidPlain(orderDetail.OrderGUID.GetValueOrDefault(Guid.Empty));
                        orderStatus = order.OrderStatus;
                    
                    //Logic for Scheduler immediate 
                    if (orderIDs != null && orderIDs.Length > 0 && order != null && order.OrderStatus == (int)OrderStatus.Closed)
                    {
                        OrderMasterDTO objOrdDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdFull(order.ID);
                        if (objOrdDTO != null && objOrdDTO.OrderStatus == (int)OrderStatus.Closed && objOrdDTO.NoOfLineItems != null &&
                            objOrdDTO.NoOfLineItems.GetValueOrDefault(0) > 0)
                        {
                            List<OrderDetailsDTO> lstOrdDtlDTO = orderDetailDAL.GetDeletedOrUnDeletedOrderDetailByOrderGUIDPlain(objOrdDTO.GUID, objOrdDTO.Room.GetValueOrDefault(0), objOrdDTO.CompanyID.GetValueOrDefault(0), false);
                            if (lstOrdDtlDTO != null && lstOrdDtlDTO.Count > 0)
                            {
                                //SendMailOrderUnSubmitted
                                SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                                SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(objOrdDTO.Supplier.GetValueOrDefault(0));
                                SendMailOrderClosed(objSupplierDTO, objOrdDTO);
                            }
                        }
                    }
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
            OrderDetailsDAL orderDetailDAL = null;
            OrderDetailsDTO orderDetail = null;
            int orderStatus = 0;

            try
            {
                orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                long SessionUserId = SessionHelper.UserID;
                string Newids = string.Empty;
                string ItemNameError = string.Empty;
                string ItemDeletedError = string.Empty;
                bool IsValidateOrderUOM = ValidateUOMForUncloseOrderLineItems(ids, out Newids, out ItemNameError, out ItemDeletedError);

                if (ItemNameError != string.Empty)
                    ItemNameError = string.Format(ResOrder.ItemHasOrderUOMIssue, ItemNameError);

                if (ItemDeletedError != string.Empty)
                {
                    ItemDeletedError = string.Format(ResItemMaster.ItemHasDeleted, ItemDeletedError);
                    ItemNameError = ItemNameError != string.Empty ? ItemNameError + " \n " + ItemDeletedError : ItemDeletedError;
                }

                //orderDetailDAL.UnCloseOrderDetailItem(ids, SessionHelper.UserID, RoomID, CompanyID);
                orderDetailDAL.UnCloseOrderDetailItem(Newids, SessionHelper.UserID, RoomID, CompanyID, SessionUserId, SessionHelper.EnterPriceID);

                if (!string.IsNullOrEmpty(CallFrom) && CallFrom.ToLower() == "order")
                {
                    string[] orderIDs = ids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (orderIDs != null && orderIDs.Length > 0)
                    {
                        orderDetail = orderDetailDAL.GetOrderDetailByIDNormal(Int64.Parse(orderIDs[0]), RoomID, CompanyID);
                        orderStatus = orderDetail.OrderStatus;
                    }
                }

                return Json(new { Message = ResOrder.LineItemClosed, Status = "ok", OrderStatus = orderStatus, ItemError = ItemNameError }, JsonRequestBehavior.AllowGet);
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
            OrderDetailsDAL orderDetailDAL = null;
            OrderMasterDAL obj = null;
            ReceivedOrderTransferDetailDAL objROTDDAL = null;
            OrderMasterDTO objDTO = null;
            ItemMasterDAL ItemDAL = null;

            try
            {
                obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                objDTO = new OrderMasterDTO() { GUID = OrderGuid, Room = RoomID, CompanyID = CompanyID, EditedFrom = "Web", WhatWhereAction = "ToUnclosedOrder" };
                var validationMsg = string.Empty;
                var isOrderValidForItemMax = ValidateOrderLineItemsForUncloseOrder(OrderGuid, out validationMsg);

                if (isOrderValidForItemMax)
                {
                    bool IsValidateOrderUOM = ValidateUOMForUncloseOrder(OrderGuid);
                    if (IsValidateOrderUOM)
                    {
                        objDTO = obj.DB_TransmitOrder(objDTO);

                        OrderDetailsDAL objDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        List<OrderDetailsDTO> lstOrdDetailDTO = objDetailDAL.GetOrderDetailByOrderGUIDPlain(objDTO.GUID, RoomID, CompanyID);
                        foreach (var item in lstOrdDetailDTO)
                        {
                            item.EditedFrom = "Web";
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objDetailDAL.Edit(item, SessionHelper.UserID, EnterpriseId);
                        }
                    }
                    else
                    {
                        validationMsg = ResOrder.OrderUOMUnClose;
                        return Json(new { Message = validationMsg, Status = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Message = validationMsg, Status = false }, JsonRequestBehavior.AllowGet);
                }
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

        #region Order Detail

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

        private List<OrderDetailsDTO> GetDeletedLineItemsFromSession(Int64 OrderID)
        {
            List<OrderDetailsDTO> lstDetailDTO = new List<OrderDetailsDTO>();
            List<OrderDetailsDTO> lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get(GetSessionKeyForDeletedRecord(OrderID));

            if (lstDetails != null && lstDetails.Count > 0)
            {
                lstDetailDTO = lstDetails;
            }

            return lstDetailDTO;
        }

        private List<OrderDetailsDTO> GetLineItemsFromSession(Int64 OrderID)
        {
            List<OrderDetailsDTO> lstDetailDTO = new List<OrderDetailsDTO>();
            List<OrderDetailsDTO> lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get(GetSessionKey(OrderID));

            if (lstDetails != null && lstDetails.Count > 0)
            {
                lstDetailDTO = lstDetails;
            }

            return lstDetailDTO;
        }

        private List<OrderDetailsDTO> PreparedOrderLiteItemWithProperData(OrderMasterDTO objOrd)
        {
            ItemMasterDTO objItemMaster = null;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<OrderDetailsDTO> lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get(GetSessionKey(objOrd.ID));

            if (lstDetails != null && lstDetails.Count > 0)
            {
                foreach (OrderDetailsDTO item in lstDetails)
                {
                    objItemMaster = null;
                    objItemMaster = objItemDAL.GetRecordByItemNumber(item.ItemNumber, objOrd.Room.GetValueOrDefault(0), objOrd.CompanyID.GetValueOrDefault(0));

                    if (objItemMaster != null)
                    {
                        item.ItemDescription = objItemMaster.Description;
                        item.SupplierPartNo = objItemMaster.SupplierPartNo;
                        item.IsAllowOrderCostuom = objItemMaster.IsAllowOrderCostuom;
                        item.OrderUOMValue = objItemMaster.OrderUOMValue;
                    }
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
        public ActionResult LoadOrderLineItems(Int64 orderID, bool IsShowDeleted = false, bool IsArchivedOrder = false, bool IsAfterDelete = false)
        {
            OrderMasterDTO objDTO = null;

            if (orderID <= 0)
                objDTO = new OrderMasterDTO() { ID = orderID, OrderListItem = new List<OrderDetailsDTO>() };
            else
            {
                objDTO = IsArchivedOrder ? new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetArchivedOrderByIdFull(orderID) : new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdNormal(orderID);
                var orderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

                if (IsShowDeleted)
                {
                    objDTO.OrderListItem = orderDetailsDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objDTO.GUID, RoomID, CompanyID, true, SessionHelper.UserSupplierIds);
                    #region Attachment Files
                    ReceiveOrderDetailsDAL receiveOrderDetailsDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    var allFiles = receiveOrderDetailsDAL.getReceiveFileByRoom(CompanyID, RoomID);
                    if (allFiles != null && allFiles.Count > 0)
                    {
                        FileAttachmentReceiveList objFileReceive = null;
                        foreach (var listItem in objDTO.OrderListItem)
                        {
                            var ordFileDetails = allFiles.FindAll(x => x.OrderDetailsGUID == listItem.GUID).ToList();
                            List<FileAttachmentReceiveList> filenames = new List<FileAttachmentReceiveList>();
                            foreach (var item in ordFileDetails)
                            {
                                objFileReceive = new FileAttachmentReceiveList();
                                objFileReceive.FileGUID = item.GUID;
                                objFileReceive.FileName = item.FileName;
                                filenames.Add(objFileReceive);
                                objFileReceive = null;
                            }
                            listItem.attachmentfileNames = filenames;
                        }
                    }
                    #endregion
                }
                else
                    objDTO.OrderListItem = GetLineItemsFromSession(orderID);

                if (objDTO.OrderListItem == null || objDTO.OrderListItem.Count <= 0 && IsAfterDelete != true)
                {
                    if (IsShowDeleted)
                        objDTO.OrderListItem = orderDetailsDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objDTO.GUID, RoomID, CompanyID, true, SessionHelper.UserSupplierIds);
                    else
                    {
                        objDTO.OrderListItem =
                            IsArchivedOrder ? orderDetailsDAL.GetArchivedOrderDetailByOrderGUIDFullWithSupplierFilter(objDTO.GUID, RoomID, CompanyID, SessionHelper.UserSupplierIds)
                            : orderDetailsDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objDTO.GUID, RoomID, CompanyID, false, SessionHelper.UserSupplierIds);

                        #region Attachment Files
                        ReceiveOrderDetailsDAL receiveOrderDetailsDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        var allFiles = receiveOrderDetailsDAL.getReceiveFileByRoom(CompanyID, RoomID);
                        if (allFiles != null && allFiles.Count > 0)
                        {
                            FileAttachmentReceiveList objFileReceive = null;
                            foreach (var listItem in objDTO.OrderListItem)
                            {
                                var ordFileDetails = allFiles.FindAll(x => x.OrderDetailsGUID == listItem.GUID).ToList();
                                List<FileAttachmentReceiveList> filenames = new List<FileAttachmentReceiveList>();
                                foreach (var item in ordFileDetails)
                                {
                                    objFileReceive = new FileAttachmentReceiveList();
                                    objFileReceive.FileGUID = item.GUID;
                                    objFileReceive.FileName = item.FileName;
                                    filenames.Add(objFileReceive);
                                    objFileReceive = null;
                                }
                                listItem.attachmentfileNames = filenames;
                            }
                        } 
                        #endregion

                        SessionHelper.Add(GetSessionKey(orderID), objDTO.OrderListItem);
                    }
                }
                
                    if (objDTO.OrderListItem != null && objDTO.OrderListItem.Count > 0)
                    {
                        objDTO.OrderListItem = objDTO.OrderListItem.OrderBy(x => x.ItemNumber).ToList();
                        if (objDTO.OrderStatus < (int)OrderStatus.Submitted && objDTO.WhatWhereAction!= "QuoteToOrder")
                        {
                            objDTO.OrderListItem.ToList().Where(x => x.hasPOItemNumber == false).ToList().ForEach(o => o.POItemLineNumber = null);
                        }
                        var LineItemsWithoutPOLineNumber = objDTO.OrderListItem.Where(x => x.POItemLineNumber == null || x.POItemLineNumber < 0).ToList();
                        var LineItemsWithPOLineNumber = objDTO.OrderListItem.Where(x => x.POItemLineNumber != null && x.POItemLineNumber > 0).ToList();
                        if (LineItemsWithPOLineNumber.Count > 0 && LineItemsWithoutPOLineNumber.Count > 0)
                        {
                            List<OrderDetailsDTO> UpdatedLineItems = new List<OrderDetailsDTO>();
                            int ItemLineCounter = 1, j = 0;
                            for (int i = 0; i < objDTO.OrderListItem.Count; i++)
                            {
                                var list = LineItemsWithPOLineNumber.Where(x => x.POItemLineNumber == ItemLineCounter).ToList();
                                if (list != null && list.Count > 0)
                                {
                                    UpdatedLineItems.AddRange(list);
                                    i = i + list.Count - 1;
                                }
                                else
                                {
                                    if (j < LineItemsWithoutPOLineNumber.Count)
                                    {
                                        UpdatedLineItems.Add(LineItemsWithoutPOLineNumber[j]);
                                        j++;
                                    }
                                    else
                                    {
                                        UpdatedLineItems.AddRange(LineItemsWithPOLineNumber.Where(x => x.POItemLineNumber > ItemLineCounter).ToList());
                                        break;
                                    }

                                }
                                ItemLineCounter++;
                            }
                            objDTO.OrderListItem = UpdatedLineItems;
                        }


                    }
                

                objDTO.IsRecordNotEditable = IsRecordNotEditable(objDTO);
                if (objDTO.IsDeleted || objDTO.IsArchived.GetValueOrDefault(false) || IsShowDeleted)
                {
                    objDTO.IsRecordNotEditable = true;
                    objDTO.IsOnlyStatusUpdate = false;
                    objDTO.IsAbleToDelete = true;
                }

                if (objDTO.MaterialStagingGUID != null)
                {
                    MaterialStagingDTO objMSDTO = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetRecord(objDTO.MaterialStagingGUID.Value, RoomID, CompanyID);
                    objDTO.StagingDefaultLocation = objMSDTO.BinID;
                    objDTO.StagingName = objMSDTO.StagingName;
                    objMSDTO = null;
                }

                if (IsShowDeleted == false)
                {
                    objDTO.NoOfLineItems = objDTO.OrderListItem.Count;
                    if (objDTO.OrderListItem != null && objDTO.OrderListItem.Count > 0)
                    {
                        if (objDTO.OrderStatus < (int)OrderStatus.Closed)
                        {
                            double? OrderCost = objDTO.OrderCost;
                            objDTO.OrderCost = objDTO.OrderListItem.Sum(x => x.IsCloseItem.GetValueOrDefault(false) ? x.OrderLineItemExtendedCost : double.Parse((x.ApprovedQuantity.GetValueOrDefault(0) == 0 ? x.RequestedQuantity.GetValueOrDefault(0) : x.ApprovedQuantity.GetValueOrDefault(0)).ToString()) * ((x.ItemCost.GetValueOrDefault(0)) / ((x.ItemCostUOMValue ?? 0) == 0 ? 1 : (x.ItemCostUOMValue ?? 1))));
                            double? OrderPrice = objDTO.OrderPrice;
                            objDTO.OrderPrice = objDTO.OrderListItem.Sum(x => x.IsCloseItem.GetValueOrDefault(false) ? x.OrderLineItemExtendedPrice : double.Parse((x.ApprovedQuantity.GetValueOrDefault(0) == 0 ? x.RequestedQuantity.GetValueOrDefault(0) : x.ApprovedQuantity.GetValueOrDefault(0)).ToString()) * ((x.ItemSellPrice.GetValueOrDefault(0)) / ((x.ItemCostUOMValue ?? 0) == 0 ? 1 : (x.ItemCostUOMValue ?? 1)))) ?? 0;

                            if (OrderCost != objDTO.OrderCost || OrderPrice != objDTO.OrderPrice)
                                new OrderMasterDAL(SessionHelper.EnterPriseDBName).Edit(objDTO);
                        }
                    }
                }
            }

            ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsCalOrderCost = false;
            objDTO.OrderListItem.ForEach(x =>
            {
                double TempReqQTY = 0;
                ItemMasterDTO Imdto = ItemDAL.GetItemByGuidNormalNew(x.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                if (Imdto != null)
                {
                    if (Imdto.OrderUOMValue == null || Imdto.OrderUOMValue <= 0)
                        Imdto.OrderUOMValue = 1;

                    x.OrderUOMValue_LineItem = Imdto.OrderUOMValue;
                    x.IsAllowOrderCostuom_LineItem = Imdto.IsAllowOrderCostuom;
                }

                x.RequiredDateStr = x.RequiredDate != null ? x.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
                x.BinName = Convert.ToString(x.BinName) == "[|EmptyStagingBin|]" ? string.Empty : x.BinName;

                if ((x.RequestedQuantityUOM == null || x.RequestedQuantityUOM <= 0))
                {
                    if (Imdto != null && Imdto.IsAllowOrderCostuom)
                        TempReqQTY = Convert.ToInt64(x.RequestedQuantity * Imdto.OrderUOMValue);
                    else
                        TempReqQTY = Convert.ToInt64(x.RequestedQuantity);
                    IsCalOrderCost = true;
                }

                if (x.CostUOMValue == null || x.CostUOMValue <= 0)
                {
                    x.CostUOMValue = 1;
                }

                if (objDTO.OrderStatus < (int)OrderStatus.Closed && !x.IsCloseItem.GetValueOrDefault(false))
                {
                    if (x.ItemCostUOMValue == null
                     || x.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                    {
                        x.ItemCostUOMValue = 1;
                    }

                    x.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (x.RequestedQuantity.GetValueOrDefault(0) * (x.ItemCost.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                     : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.ItemCost.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1))))));

                    x.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (x.RequestedQuantity.GetValueOrDefault(0) * (x.ItemSellPrice.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                     : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.ItemSellPrice.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1))))));

                    if (x.RequestedQuantityUOM == null || x.RequestedQuantityUOM <= 0)
                    {
                        x.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (TempReqQTY * (x.ItemCost.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                     : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.ItemCost.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1))))));

                        x.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (TempReqQTY * (x.ItemSellPrice.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                         : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.ItemSellPrice.GetValueOrDefault(0) / x.ItemCostUOMValue.GetValueOrDefault(1))))));
                    }
                }

                if (x.RequestedQuantityUOM != null && x.RequestedQuantityUOM > 0)
                    x.RequestedQuantity = x.RequestedQuantityUOM;

                if (x.ApprovedQuantityUOM != null && x.ApprovedQuantityUOM > 0)
                    x.ApprovedQuantity = x.ApprovedQuantityUOM;

                if (x.ReceivedQuantityUOM != null && x.ReceivedQuantityUOM > 0)
                    x.ReceivedQuantity = x.ReceivedQuantityUOM;

                if (x.InTransitQuantityUOM != null && x.InTransitQuantityUOM > 0)
                    x.InTransitQuantity = x.InTransitQuantityUOM;
                else if (x.InTransitQuantityUOM != null && x.InTransitQuantityUOM < 0 && x.InTransitQuantityUOM != null)
                    x.InTransitQuantity = x.InTransitQuantityUOM;

                if (x.BinName != null && x.BinName != string.Empty)
                {
                    var lstOfOrderLineItemBin = objBinMasterDAL.GetAllBinMastersByBinList(x.BinName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (lstOfOrderLineItemBin != null && lstOfOrderLineItemBin.Where(xl => xl.ID == x.Bin).Count() > 0 && (lstOfOrderLineItemBin.Where(xl => xl.ID == x.Bin).FirstOrDefault().IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) || x.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false)))
                    {
                        x.DefaultReorderQuantity = lstOfOrderLineItemBin.Where(xl => xl.ID == x.Bin).FirstOrDefault().DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? lstOfOrderLineItemBin.Where(xl => xl.ID == x.Bin).FirstOrDefault().DefaultReorderQuantity.GetValueOrDefault(0) : x.DefaultReorderQuantity;
                    }
                }


            });

            if (IsShowDeleted == false && IsCalOrderCost && orderID > 0 && objDTO.OrderStatus < (int)OrderStatus.Closed)
            {
                objDTO.OrderCost = objDTO.OrderListItem.Sum(x => x.OrderLineItemExtendedCost);
                objDTO.OrderPrice = objDTO.OrderListItem.Sum(x => x.OrderLineItemExtendedPrice) ?? 0;
            }

            ViewBag.IsShowDeleted = IsShowDeleted;

            return PartialView("_OrderLineItems", objDTO);
        }

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult LoadItemMasterModel(Int64 ParentId)
        {
            OrderMasterDTO objDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdPlain(ParentId);

            string modelHeader = eTurns.DTO.ResOrder.ItemModelHeader;

            if (objDTO.OrderType == (int)OrderType.RuturnOrder)
            {
                modelHeader = ResOrder.ItemModelHeaderRO;
            }

            ItemModelPerameter obj = null;

            if (GetOrderType == OrderType.Order)
            {
                obj = new ItemModelPerameter()
                {
                    AjaxURLAddItemToSession = "~/Order/AddItemsToOrder/", // Not Used
                    PerentID = ParentId.ToString(),
                    PerentGUID = objDTO.GUID.ToString(),
                    ModelHeader = modelHeader,
                    AjaxURLAddMultipleItemToSession = "~/Order/AddItemsToOrder/",
                    AjaxURLToFillItemGrid = "~/Order/GetItemsModelMethod/",
                    CallingFromPageName = "ORD",
                    OrdStagingID = objDTO.StagingID.ToString(),
                    OrdRequeredDate = objDTO.RequiredDate.ToString("MM/dd/yyyy"),
                    OrderStatus = objDTO.OrderStatus,
                };
            }
            else if (GetOrderType == OrderType.RuturnOrder)
            {
                obj = new ItemModelPerameter()
                {
                    AjaxURLAddItemToSession = "~/Order/AddItemsToOrder/", // Not Used
                    PerentID = ParentId.ToString(),
                    PerentGUID = objDTO.GUID.ToString(),
                    ModelHeader = modelHeader,
                    AjaxURLAddMultipleItemToSession = "~/Order/AddItemsToOrder/",
                    AjaxURLToFillItemGrid = "~/Order/GetItemsModelMethod/",
                    CallingFromPageName = "RETORD",
                    OrdStagingID = objDTO.StagingID.ToString(),
                    OrdRequeredDate = objDTO.RequiredDate.ToString("MM/dd/yyyy"),
                    OrderStatus = objDTO.OrderStatus,
                };
            }

            return PartialView("ItemMasterModel", obj);
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
            //make changes to resolve an issue of Sort (WI-431)

            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower().Contains("itemudf"))
                sortColumnName = sortColumnName.ToLower().Replace("item", "");
            if (!string.IsNullOrEmpty(sortColumnName) && (sortColumnName.ToLower().Equals("extendedcost desc") || sortColumnName.ToLower().Equals("extendedcost asc")))
                sortColumnName = sortColumnName.ToLower().Replace("extendedcost", "ISNULL(ExtendedCost,0)");
            if (!string.IsNullOrEmpty(sortColumnName) && (sortColumnName.ToLower().Equals("islotserialexpirycost desc") || sortColumnName.ToLower().Equals("islotserialexpirycost asc")))
                sortColumnName = sortColumnName.ToLower().Replace("islotserialexpirycost", "ISNULL(IsLotSerialExpiryCost,0)");
            //if (!string.IsNullOrEmpty(sortColumnName) && (sortColumnName.ToLower().Equals("costuomname desc") || sortColumnName.ToLower().Equals("costuomname asc")))
            //    sortColumnName = sortColumnName.ToLower().Replace("costuomname", "ISNULL(CostUOMName,0)");
            if (!string.IsNullOrEmpty(sortColumnName) && (sortColumnName.ToLower().Equals("cost desc") || sortColumnName.ToLower().Equals("cost asc")))
                sortColumnName = sortColumnName.ToLower().Replace("cost", "ISNULL(Cost,0)");
            if ((sortColumnName.Equals("null desc") || sortColumnName.Equals("null asc")))
                sortColumnName = string.Empty;
            if (Request["sSearch_0"] != null && !string.IsNullOrEmpty(Request["sSearch_0"]))
            {
                param.sSearch = Request["sSearch_0"].Trim(',');
            }

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            Int64 ORDID = 0;
            Int64.TryParse(Request["ParentID"], out ORDID);
            OrderMasterDTO objMasterDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdPlain(ORDID);
            List<OrderDetailsDTO> lstItems = GetLineItemsFromSession(ORDID);
            string ExclueBinMasterGUIDs = "";
            string ItemsIDs = "";
            string modelPopupFor = "order";
            if (GetOrderType == OrderType.RuturnOrder && objMasterDTO.StagingID.GetValueOrDefault(0) > 0)
                modelPopupFor = "stagingreturnorder";
            else if (GetOrderType == OrderType.RuturnOrder)
            {
                ItemsIDs = String.Join(",", lstItems.Select(x => (x.ItemGUID == null ? "" : x.ItemGUID.Value.ToString())));
                modelPopupFor = "returnorder";
            }

            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetPagedItemsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, AllowConsignedItemToOrder, AllowConsignedItemToOrder, true, SessionHelper.UserID, modelPopupFor, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, ItemsIDs, null, ExclueBinMasterGUIDs, objMasterDTO.Supplier).ToList();
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            foreach (var item in DataFromDB)
            {

                List<DTOForAutoComplete> locations = GetBinsOfItemByOrderId(string.Empty, string.Empty, item.GUID.ToString(), false, false, ORDID);
                item.BinAutoComplete = new List<BinAutoComplete>();

                foreach (var binlist in locations)
                {
                    BinAutoComplete bin = new BinAutoComplete();
                    bin.ID = binlist.ID;
                    bin.BinNumber = binlist.Key;
                    item.BinAutoComplete.Add(bin);
                }

                if (item.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                {
                    item.DefaultReorderQuantity = (item.DefaultReorderQuantity.GetValueOrDefault(0) <= 0 ? 1 : item.DefaultReorderQuantity.GetValueOrDefault(0));
                    item.ItemDefaultReorderQuantity = (item.ItemDefaultReorderQuantity.GetValueOrDefault(0) <= 0 ? 1 : item.ItemDefaultReorderQuantity.GetValueOrDefault(0));
                }

                if (modelPopupFor == "order" && item.IsAllowOrderCostuom)
                {
                    item.DefaultReorderQuantity = item.DefaultReorderQuantity / item.OrderUOMValue;
                    item.ItemDefaultReorderQuantity = item.DefaultReorderQuantity;
                }

                if (item.DefaultLocationName != null && item.DefaultLocationName != string.Empty)
                {
                    var lstOfOrderLineItemBin = objBinMasterDAL.GetAllBinMastersByBinList(item.DefaultLocationName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (lstOfOrderLineItemBin != null && lstOfOrderLineItemBin.Where(xl => xl.ID == item.DefaultLocation).Count() > 0 && lstOfOrderLineItemBin.Where(xl => xl.ID == item.DefaultLocation).FirstOrDefault().IsEnforceDefaultReorderQuantity.GetValueOrDefault(false))
                    {
                        item.IsBinEnforceDefaultReorderQuantity = lstOfOrderLineItemBin.Where(xl => xl.ID == item.DefaultLocation).FirstOrDefault().IsEnforceDefaultReorderQuantity.GetValueOrDefault(false);
                    }
                }

            }

            var jsonResult = Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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
            ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

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
                        ItemMasterDTO IMDTO = ItemDAL.GetItemByGuidNormal(item.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.Key = item.BinNumber;
                        objAutoDTO.Value = item.BinNumber;
                        objAutoDTO.ID = item.ID;

                        if (item.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true && IMDTO.IsAllowOrderCostuom)
                        {
                            objAutoDTO.OtherInfo2 = Convert.ToString(item.DefaultReorderQuantity.GetValueOrDefault(0) / IMDTO.OrderUOMValue);
                        }

                        if (!string.IsNullOrEmpty(item.BinNumber) && item.BinNumber.Trim().Length > 0)
                            locations.Add(objAutoDTO);
                    }
                }
            }

            return locations;
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult OrderLineItemsDelete(OrderDetailsDTO[] objDeletedItems, Int64 OrderID)
        {
            string message = "";
            string status = "";
            string orderItems = "0";
            string ordercost = "0";
            string orderprice = "0";
            try
            {
                //---------------------------------------------
                //
                int? PriseSelectionOption = 0;
                eTurns.DAL.RoomDAL onjRoomDAL = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName);
                RoomModuleSettingsDTO objRoomModuleSettingsDTO = onjRoomDAL.GetRoomModuleSettings(eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Orders);
                if (objRoomModuleSettingsDTO != null)
                {
                    PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;
                }

                if (PriseSelectionOption != 1 && PriseSelectionOption != 2)
                {
                    PriseSelectionOption = 1;
                }

                //---------------------------------------------
                //
                if (objDeletedItems != null && objDeletedItems.Length > 0)
                {
                    List<OrderDetailsDTO> lstDetailDTO = GetLineItemsFromSession(OrderID);
                    List<OrderDetailsDTO> lstDeletedItems = GetDeletedLineItemsFromSession(OrderID);

                    foreach (var item in objDeletedItems)
                    {
                        if (item.ID > 0)
                        {
                            lstDeletedItems.Add(item);
                        }
                        if (item.ID > 0)
                        {
                            lstDetailDTO.RemoveAll(x => x.ID == item.ID && x.ItemGUID == item.ItemGUID);
                        }
                        else if (item.tempDetailsGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            lstDetailDTO.RemoveAll(x => x.tempDetailsGUID == item.tempDetailsGUID && x.ItemGUID == item.ItemGUID);
                        }
                    }

                    SessionHelper.Add(GetSessionKeyForDeletedRecord(OrderID), lstDeletedItems);
                    SessionHelper.Add(GetSessionKey(OrderID), lstDetailDTO);
                    message = string.Format(ResCommon.RecordDeletedSuccessfully, objDeletedItems.Length);
                    status = "ok";

                    if (lstDetailDTO != null && lstDetailDTO.Count > 0)
                    {
                        orderItems = lstDetailDTO.Count.ToString();
                        double ordCost = 0;
                        double ordPrice = 0;

                        foreach (var item in lstDetailDTO)
                        {
                            Int32 ordItemcostUOMValue = 1;
                            if (item.ItemCostUOMValue != null
                                && item.ItemCostUOMValue.GetValueOrDefault(0) > 0)
                            {
                                ordItemcostUOMValue = item.ItemCostUOMValue.GetValueOrDefault(1);
                            }
                            if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordCost += ((double)item.ApprovedQuantity.GetValueOrDefault(0) * (double)item.Cost.GetValueOrDefault(0))
                                            / (ordItemcostUOMValue);
                            }
                            else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordCost += ((double)item.RequestedQuantity.GetValueOrDefault(0) * (double)item.Cost.GetValueOrDefault(0))
                                            / (ordItemcostUOMValue);
                            }

                            if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordPrice += ((double)item.ApprovedQuantity.GetValueOrDefault(0) * (double)item.SellPrice.GetValueOrDefault(0))
                                             / (ordItemcostUOMValue);
                            }
                            else if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ordPrice += ((double)item.RequestedQuantity.GetValueOrDefault(0) * (double)item.SellPrice.GetValueOrDefault(0))
                                             / (ordItemcostUOMValue);
                            }
                        }

                        RegionSettingDAL objRegDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                        eTurnsRegionInfo objRegionInfo = objRegDAL.GetRegionSettingsById(RoomID, CompanyID, SessionHelper.UserID);

                        if (objRegionInfo != null)
                        {
                            ordercost = ordCost.ToString("N" + objRegionInfo.CurrencyDecimalDigits.ToString(), SessionHelper.RoomCulture);
                            orderprice = ordPrice.ToString("N" + objRegionInfo.CurrencyDecimalDigits.ToString(), SessionHelper.RoomCulture);
                        }
                        else
                        {
                            ordercost = ordCost.ToString("N" + "0", SessionHelper.RoomCulture);
                            orderprice = ordPrice.ToString("N" + "0", SessionHelper.RoomCulture);
                        }
                    }
                }
                else
                {
                    message = ResOrder.PleaseSelectRecord;
                    status = "fail";
                }

                return Json(new { Message = message, Status = status, OrderItems = orderItems, Ordercost = ordercost, OrdPrice = orderprice }, JsonRequestBehavior.AllowGet);
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
        public JsonResult AddUpdateDeleteOrderItemsToOrder(OrderDetailsDTO[] arrDetails, long OrderID, bool IsUpdateOrderStatus, int? OrderStatusToUpdate)
        {
            string message = "";
            string status = "";
            bool IsOrderClosedFromUnSubmitted = false;
            bool isOrderApproved = false;
            try
            {
                List<OrderDetailsDTO> lstordDetailsForItemCostUpdate = new List<OrderDetailsDTO>();

                if (TempData["IsOrderClosedFromUnSubmitted"] != null)
                {
                    IsOrderClosedFromUnSubmitted = Convert.ToBoolean(TempData["IsOrderClosedFromUnSubmitted"]);
                }

                Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
                Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
                int rejectedDueToPreventMaxValidation = 0;

                long SessionUserId = SessionHelper.UserID;

                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                List<OrderDetailsDTO> lstDeletedItems = GetDeletedLineItemsFromSession(0);
                string strDeletedIDs = string.Join(",", lstDeletedItems.Select(x => x.ID));

                if (!string.IsNullOrEmpty(strDeletedIDs))
                {
                    OrderDetailsDAL objDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    objDetailDAL.DeleteRecords(strDeletedIDs, SessionHelper.UserID, RoomID, CompanyID, SessionUserId, SessionHelper.EnterPriceID);
                }

                OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

                bool IsStagingLoc = false;
                OrderMasterDTO objOrder = new OrderMasterDTO();

                if (OrderID > 0)
                {
                    objOrder = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdPlain(OrderID);
                    if (objOrder != null && objOrder.MaterialStagingGUID != null)
                        IsStagingLoc = true;
                }

                if (arrDetails != null && arrDetails.Length > 0)
                {
                    BinMasterDTO objBinMasterDTO;
                    List<Guid> lstItemGUID = arrDetails.Where(d => d.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).Select(x => x.ItemGUID.GetValueOrDefault(Guid.Empty)).ToList();
                    string strItemGUIDs = string.Join(",", lstItemGUID.ToArray());
                    List<ItemMasterDTO> lstOfOrderLineItem = new List<ItemMasterDTO>();
                    lstOfOrderLineItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidsNormal(strItemGUIDs, SessionHelper.RoomID, SessionHelper.CompanyID);
                    BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BinMasterDTO> lstOfOrderLineItemBin = new List<BinMasterDTO>();
                    string strBinNumbers = string.Join(",", arrDetails.Where(x => (x.BinName ?? string.Empty) != string.Empty).Select(b => b.BinName).Distinct());
                    lstOfOrderLineItemBin = objBinMasterDAL.GetAllBinMastersByBinList(strBinNumbers, SessionHelper.RoomID, SessionHelper.CompanyID);
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                    if (OrderID > 0
                        && objOrder != null
                        && ((objOrder.OrderStatus == (int)OrderStatus.Approved || objOrder.OrderStatus == (int)OrderStatus.Transmitted)
                        || (IsUpdateOrderStatus && OrderStatusToUpdate.HasValue && (OrderStatusToUpdate.Value == (int)OrderStatus.Approved || OrderStatusToUpdate.Value == (int)OrderStatus.Transmitted)))
                        )
                    {
                        if (objOrder.OrderType == (int)OrderType.Order)
                            isOrderApproved = true;
                    }

                    foreach (OrderDetailsDTO item in arrDetails)
                    {
                        if (IsOrderClosedFromUnSubmitted == true)
                            item.ApprovedQuantity = 0;

                        objBinMasterDTO = new BinMasterDTO();
                        item.Room = RoomID;
                        item.RequiredDate = item.RequiredDateStr != null ? Convert.ToDateTime(DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture)) : Convert.ToDateTime(item.RequiredDateStr);
                        item.CompanyID = CompanyID;
                        objBinMasterDTO = lstOfOrderLineItemBin.Where(x => (x.BinNumber ?? string.Empty).ToLower() == (item.BinName ?? string.Empty).ToLower() && x.ItemGUID == item.ItemGUID).FirstOrDefault();

                        if (objBinMasterDTO == null || objBinMasterDTO.ID <= 0)
                        {

                            objBinMasterDTO = objBinMasterDAL.GetItemBinPlain(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, IsStagingLoc);
                        }

                        if (objBinMasterDTO != null)
                        {
                            item.Bin = objBinMasterDTO.ID;
                        }

                        item.LastUpdatedBy = SessionHelper.UserID;

                        ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                        objItemMasterDTO = lstOfOrderLineItem.Where(x => x.GUID == (item.ItemGUID ?? Guid.Empty)).FirstOrDefault();

                        if (objOrder != null && objItemMasterDTO != null && item != null)
                        {
                            CostUOMMasterDTO costUOM = new CostUOMMasterDTO();

                            if (objItemMasterDTO.CostUOMValue <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }
                            else
                            {
                                costUOM.CostUOMValue = objItemMasterDTO.CostUOMValue;
                            }

                            if (costUOM.CostUOMValue == null || costUOM.CostUOMValue <= 0)
                            {
                                costUOM.CostUOMValue = 1;
                            }

                            if (objItemMasterDTO.CostUOMID != null)
                                item.ItemCostUOM = objItemMasterDTO.CostUOMID;


                            if (item.ItemCost.GetValueOrDefault(0) > 0)
                            {
                                if (item.ItemMarkup > 0)
                                {
                                    item.ItemSellPrice = item.ItemCost + ((item.ItemCost * item.ItemMarkup) / 100);
                                }
                                else
                                {
                                    item.ItemSellPrice = item.ItemCost;
                                }
                            }
                            else
                            {
                                item.ItemSellPrice = 0;
                            }

                            //item.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objOrder.OrderStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1)))
                            //                                            : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))))));

                            //item.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objOrder.OrderStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemSellPrice.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1)))
                            //                                            : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemSellPrice.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))))));

                            OrderUOMMasterDAL objOrderUOMDAL = new OrderUOMMasterDAL(SessionHelper.EnterPriseDBName);

                            if (item.RequestedQuantity != null && item.RequestedQuantity >= 0)
                            {
                                item.RequestedQuantityUOM = item.RequestedQuantity;
                                if (objOrder.OrderType == (int)OrderType.Order)
                                    item.RequestedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.RequestedQuantity);    //item.RequestedQuantity * costUOM.CostUOMValue;
                            }

                            if (item.ApprovedQuantity != null && item.ApprovedQuantity >= 0)
                            {
                                item.ApprovedQuantityUOM = item.ApprovedQuantity;
                                if (objOrder.OrderType == (int)OrderType.Order)
                                    item.ApprovedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.ApprovedQuantity); //item.ApprovedQuantity * costUOM.CostUOMValue;
                            }

                            if (item.ReceivedQuantity != null && item.ReceivedQuantity >= 0)
                            {
                                item.ReceivedQuantityUOM = item.ReceivedQuantity;
                                if (objOrder.OrderType == (int)OrderType.Order)
                                    item.ReceivedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.ReceivedQuantity); //item.ReceivedQuantity * costUOM.CostUOMValue;
                            }

                            if (item.InTransitQuantity != null && item.InTransitQuantity >= 0)
                            {
                                item.InTransitQuantityUOM = item.InTransitQuantity;
                                if (objOrder.OrderType == (int)OrderType.Order)
                                    item.InTransitQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.InTransitQuantity); //item.InTransitQuantity * costUOM.CostUOMValue;
                            }

                            if (item.ItemCostUOMValue == null
                             || item.ItemCostUOMValue.GetValueOrDefault(0) <= 0)
                            {
                                item.ItemCostUOMValue = 1;
                            }
                            item.OrderLineException = item.OrderLineException;
                            item.OrderLineExceptionDesc = item.OrderLineExceptionDesc;

                            item.OrderLineItemExtendedCost = double.Parse(Convert.ToString(((objOrder.OrderStatus <= 2 && !IsUpdateOrderStatus) ? item.ApprovedQuantity.GetValueOrDefault(0) == 0 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))) : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                      : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))))));

                            item.OrderLineItemExtendedPrice = double.Parse(Convert.ToString(((objOrder.OrderStatus <= 2 && !IsUpdateOrderStatus) ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemSellPrice.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1)))
                                                                        : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemSellPrice.GetValueOrDefault(0) / item.ItemCostUOMValue.GetValueOrDefault(1))))));

                            if ((objOrder.OrderStatus <= (int)OrderStatus.Transmitted || item.ID < 1) && objOrder.Supplier.GetValueOrDefault(0) > 0 && item.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                var supplierForOrderLineItem = objItemMasterDAL.GetItemSupplierForOrder(item.ItemGUID.GetValueOrDefault(Guid.Empty), objOrder.Supplier.GetValueOrDefault(0));

                                if (supplierForOrderLineItem != null && supplierForOrderLineItem.SupplierID.GetValueOrDefault(0) > 0)
                                {
                                    item.SupplierID = supplierForOrderLineItem.SupplierID;
                                    item.SupplierPartNo = supplierForOrderLineItem.SupplierPartNo;
                                }
                            }
                        }

                        if (item.ID > 0)
                        {
                            item.EditedFrom = "Web";
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                            item.LastUpdated = DateTimeUtility.DateTimeNow;
                            if (item.RequiredDate.GetValueOrDefault(DateTime.MinValue) <= DateTime.MinValue)
                                item.RequiredDate = DateTimeUtility.DateTimeNow;
                            objDAL.Edit(item, SessionUserId, EnterpriseId);
                            if (isOrderApproved)
                            {
                                lstordDetailsForItemCostUpdate.Add(item);
                                //WI-8417 JKP
                                try
                                {
                                    objDAL.UpdateOrderUsedTotalValueBPO(item.ItemGUID.Value, item.ApprovedQuantity.GetValueOrDefault(0), item.OrderLineItemExtendedCost.GetValueOrDefault(0), "order");
                                }
                                catch (Exception ex) { }
                            }
                            if ((objOrder != null
                                && ((objOrder.OrderStatus == (int)OrderStatus.Approved || objOrder.OrderStatus == (int)OrderStatus.Transmitted)
                                || (IsUpdateOrderStatus && OrderStatusToUpdate.HasValue && (OrderStatusToUpdate.Value == (int)OrderStatus.Approved || OrderStatusToUpdate.Value == (int)OrderStatus.Transmitted)))
                                ) && (objOrder.OrderType == (int)OrderType.RuturnOrder))
                            {
                                //WI-8417 JKP
                                try
                                {
                                    objDAL.UpdateOrderUsedTotalValueBPO(item.ItemGUID.Value, item.ApprovedQuantity.GetValueOrDefault(0), item.OrderLineItemExtendedCost.GetValueOrDefault(0), "returnorder");
                                }
                                catch (Exception ex) { }
                            }
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

                            OrderDetailsDTO insertedOrderDetail = new OrderDetailsDTO();
                            insertedOrderDetail = objDAL.Insert(item, SessionUserId, SessionHelper.EnterPriceID);
                            if (isOrderApproved && insertedOrderDetail != null
                                && insertedOrderDetail.ID > 0)
                            {
                                lstordDetailsForItemCostUpdate.Add(insertedOrderDetail);
                                //WI-8417
                                try
                                {
                                    objDAL.UpdateOrderUsedTotalValueBPO(item.ItemGUID.Value, item.ApprovedQuantity.GetValueOrDefault(0), item.OrderLineItemExtendedCost.GetValueOrDefault(0), "order");
                                }
                                catch (Exception ex) { }
                            }
                            if ((objOrder != null
                               && ((objOrder.OrderStatus == (int)OrderStatus.Approved || objOrder.OrderStatus == (int)OrderStatus.Transmitted)
                               || (IsUpdateOrderStatus && OrderStatusToUpdate.HasValue && (OrderStatusToUpdate.Value == (int)OrderStatus.Approved || OrderStatusToUpdate.Value == (int)OrderStatus.Transmitted)))
                               ) && (objOrder.OrderType == (int)OrderType.RuturnOrder))
                            {
                                //WI-8417 JKP
                                try
                                {
                                    objDAL.UpdateOrderUsedTotalValueBPO(item.ItemGUID.Value, item.ApprovedQuantity.GetValueOrDefault(0), item.OrderLineItemExtendedCost.GetValueOrDefault(0), "returnorder");
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }

                }
                if (isOrderApproved
                    && lstordDetailsForItemCostUpdate != null
                    && lstordDetailsForItemCostUpdate.Count > 0)
                {
                    OrderMasterDAL orderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

                    DataTable dtOrdDetails = objDAL.GetOrderDetailTableFromList(lstordDetailsForItemCostUpdate);
                    orderMasterDAL.Ord_UpdateItemCostBasedonOrderDetailCost(SessionHelper.UserID, "Web-OrderApprove", SessionHelper.RoomID, SessionHelper.CompanyID, dtOrdDetails);
                    //WI-8417 

                }

                if (OrderID > 0 && objOrder != null && (objOrder.OrderStatus == (int)OrderStatus.UnSubmitted && !IsUpdateOrderStatus))
                {
                    OrderMasterDTO objOrdDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdFull(OrderID);
                    if (objOrdDTO != null && objOrdDTO.OrderStatus == (int)OrderStatus.UnSubmitted && objOrdDTO.NoOfLineItems != null &&
                        objOrdDTO.NoOfLineItems.GetValueOrDefault(0) > 0)
                    {
                        List<OrderDetailsDTO> lstOrdDtlDTO = objDAL.GetDeletedOrUnDeletedOrderDetailByOrderGUIDPlain(objOrdDTO.GUID, objOrdDTO.Room.GetValueOrDefault(0), objOrdDTO.CompanyID.GetValueOrDefault(0), false);
                        if (lstOrdDtlDTO != null && lstOrdDtlDTO.Count > 0)
                        {
                            //SendMailOrderUnSubmitted
                            SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                            SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(objOrdDTO.Supplier.GetValueOrDefault(0));
                            SendMailOrderUnSubmitted(objSupplierDTO, objOrdDTO);
                        }
                    }
                }
                if (OrderID > 0 && objOrder != null && objOrder.OrderStatus == (int)OrderStatus.Closed)
                {
                    OrderMasterDTO objOrdDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdFull(OrderID);
                    if (objOrdDTO != null && objOrdDTO.OrderStatus == (int)OrderStatus.Closed && objOrdDTO.NoOfLineItems != null &&
                        objOrdDTO.NoOfLineItems.GetValueOrDefault(0) > 0)
                    {
                        List<OrderDetailsDTO> lstOrdDtlDTO = objDAL.GetDeletedOrUnDeletedOrderDetailByOrderGUIDPlain(objOrdDTO.GUID, objOrdDTO.Room.GetValueOrDefault(0), objOrdDTO.CompanyID.GetValueOrDefault(0), false);
                        if (lstOrdDtlDTO != null && lstOrdDtlDTO.Count > 0)
                        {
                            //SendMailOrderUnSubmitted
                            SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                            SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(objOrdDTO.Supplier.GetValueOrDefault(0));
                            SendMailOrderClosed(objSupplierDTO, objOrdDTO);
                        }
                    }
                }

                if (rejectedDueToPreventMaxValidation <= 0)
                {
                    SessionHelper.RomoveSessionByKey(GetSessionKey(0));
                    SessionHelper.RomoveSessionByKey(GetSessionKeyForDeletedRecord(0));
                }

                if (rejectedDueToPreventMaxValidation <= 0)
                {
                    message = ResCommon.RecordsSavedSuccessfully;
                    status = "ok";
                }
                else if (arrDetails.Length - (rejectedDueToPreventMaxValidation) > 0)
                {
                    if (objOrder.OrderStatus == (int)OrderStatus.Closed)
                    {
                        message = ResCommon.RecordsSavedSuccessfully;
                        status = "ok";
                    }
                    else
                    {
                        var msg = (arrDetails.Length - rejectedDueToPreventMaxValidation) + " " + ResCommon.RecordsSavedSuccessfully;
                        message = (message == ""
                            ? msg
                            : message + msg);
                        status = "Warning";
                    }
                }
                else
                {
                    if (objOrder.OrderStatus == (int)OrderStatus.Closed && rejectedDueToPreventMaxValidation > 0)
                    {
                        status = "ok";
                    }
                    else
                    {
                        status = "fail";
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
            }
        }

        [HttpPost]
        public JsonResult SaveLatestValueInSession(OrderDetailsDTO[] arrDetails, Int64 OrderID)
        {
            try
            {
                List<OrderDetailsDTO> objSessionOrderList = (List<OrderDetailsDTO>)SessionHelper.Get(GetSessionKey(OrderID));

                List<OrderDetailsDTO> objOrderList = arrDetails.ToList();
                if (objSessionOrderList != null && objSessionOrderList.Count > 0)
                {
                    foreach (var item in objOrderList)
                    {
                        OrderDetailsDTO detailFromSession = objSessionOrderList.FirstOrDefault(x => x.ItemGUID == item.ItemGUID);
                        if (detailFromSession != null)
                        {
                            item.ItemNumber = detailFromSession.ItemNumber;
                            item.Cost = detailFromSession.Cost;
                            item.SellPrice = detailFromSession.SellPrice;
                            item.OnHandQuantity = detailFromSession.OnHandQuantity;
                            item.StagedQuantity = detailFromSession.StagedQuantity;
                            item.ItemDescription = detailFromSession.ItemDescription;
                            item.Unit = detailFromSession.Unit;
                            item.GLAccount = detailFromSession.GLAccount;
                            item.Markup = detailFromSession.Markup;
                            item.SupplierID = detailFromSession.SupplierID;
                            item.SupplierName = detailFromSession.SupplierName;
                            item.SupplierPartNo = detailFromSession.SupplierPartNo;
                            item.OnOrderQuantity = detailFromSession.OnOrderQuantity;
                            item.OnReturnQuantity = detailFromSession.OnReturnQuantity;
                            item.OnOrderInTransitQuantity = detailFromSession.OnOrderInTransitQuantity;
                            item.MinimumQuantity = detailFromSession.MinimumQuantity;
                            item.MaximumQuantity = detailFromSession.MaximumQuantity;
                            item.DefaultReorderQuantity = detailFromSession.DefaultReorderQuantity;
                            item.DefaultReorderQuantity = detailFromSession.DefaultReorderQuantity;
                            item.ManufacturerNumber = detailFromSession.ManufacturerNumber;
                            item.Manufacturer = detailFromSession.Manufacturer;
                            item.ManufacturerID = detailFromSession.ManufacturerID;
                            item.SuggestedOrderQuantity = detailFromSession.SuggestedOrderQuantity;
                            item.ODPackSlipNumbers = detailFromSession.ODPackSlipNumbers;
                            item.ItemCost = detailFromSession.ItemCost;
                            item.ItemCostUOM = detailFromSession.ItemCostUOM;
                            item.ItemCostUOMValue = detailFromSession.ItemCostUOMValue;
                            item.ItemMarkup = detailFromSession.ItemMarkup;
                            item.ItemSellPrice = detailFromSession.ItemSellPrice;
                        }
                    }
                    SessionHelper.Add(GetSessionKey(OrderID), objOrderList);
                }
            }
            catch
            {
            }

            return Json(new { Message = ResOrder.AddedSuccessfully, Success = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used to validate order line items for Item/Bin max order qty(WI-4561) on save order.
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="message"></param>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        private bool ValidateOrderLineItemsForMaxOrderQty(Int64 OrderID, out string message, int orderStatus)
        {
            List<OrderDetailsDTO> lstDetails = GetLineItemsFromSession(OrderID);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objOrder = new OrderMasterDTO();
            OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            int rejectedDueToPreventMaxValidation = 0;
            message = string.Empty;
            Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
            Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
            Dictionary<Guid, double> itemLevelOrderQtyForEdit = new Dictionary<Guid, double>();
            Dictionary<string, double> binLevelOrderQtyForEdit = new Dictionary<string, double>();

            if (OrderID > 0)
            {
                objOrder = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdPlain(OrderID);
                objOrder.OrderListItem = objDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objOrder.GUID, RoomID, CompanyID, false, SessionHelper.UserSupplierIds);
            }

            var isOrderStatusSubmit = (orderStatus >= (int)OrderStatus.Approved ? true : false);
            var isOrderStatusHigherThanSubmit = (orderStatus > (int)OrderStatus.Approved ? true : false);

            if (lstDetails != null && lstDetails.Count > 0)
            {
                List<ItemMasterDTO> ordDetailItemMaster = new List<ItemMasterDTO>();
                List<Guid> lstItemGUID = lstDetails.Where(d => d.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).Select(x => x.ItemGUID.GetValueOrDefault(Guid.Empty)).ToList();
                string strItemGUIDs = string.Join(",", lstItemGUID.ToArray());
                ordDetailItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidsPlain(strItemGUIDs, SessionHelper.RoomID, SessionHelper.CompanyID);

                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                List<BinMasterDTO> lstOfOrderLineItemBin = new List<BinMasterDTO>();
                string strBinNumbers = string.Join(",", lstDetails.Where(x => (x.BinName ?? string.Empty) != string.Empty).Select(b => b.BinName).Distinct());
                lstOfOrderLineItemBin = objBinMasterDAL.GetAllBinMastersByBinList(strBinNumbers, SessionHelper.RoomID, SessionHelper.CompanyID);

                foreach (var item in lstDetails.Where(e => e.IsDeleted.GetValueOrDefault(false).Equals(false)))
                {
                    ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                    objItemMasterDTO = ordDetailItemMaster.Where(x => x.GUID == (item.ItemGUID ?? Guid.Empty)).FirstOrDefault();

                    if (objItemMasterDTO != null && objItemMasterDTO.IsItemLevelMinMaxQtyRequired.HasValue && objItemMasterDTO.IsItemLevelMinMaxQtyRequired.Value)
                    {
                        var tmpItemOnOrderQty = objItemMasterDTO.OnOrderQuantity.GetValueOrDefault(0);
                        if (OrderID > 0 && objOrder != null && objOrder.OrderListItem != null && objOrder.OrderListItem.Any())
                        {
                            var lineItemExistingQty = objOrder.OrderListItem.Where(e => e.ItemNumber.Equals(objItemMasterDTO.ItemNumber) && item.ID > 0 && e.ID == item.ID).FirstOrDefault();

                            if (lineItemExistingQty != null && (isOrderStatusHigherThanSubmit ? (lineItemExistingQty.ApprovedQuantity.GetValueOrDefault(0)) : (lineItemExistingQty.RequestedQuantity.GetValueOrDefault(0))) > 0)
                            {
                                var existingLineItems = objOrder.OrderListItem.Where(e => e.ItemNumber.Equals(objItemMasterDTO.ItemNumber)).ToList();

                                if (existingLineItems != null && existingLineItems.Any())
                                {
                                    var totalOfExistingQty = (existingLineItems.Sum(e => isOrderStatusHigherThanSubmit ? e.ApprovedQuantity.GetValueOrDefault(0) : e.RequestedQuantity.GetValueOrDefault(0)));
                                    tmpItemOnOrderQty = tmpItemOnOrderQty - (totalOfExistingQty);
                                    var latestExistingItemsExceptCurrentItem = lstDetails.Where(e => e.ItemNumber.Equals(objItemMasterDTO.ItemNumber) && e.ID > 0 && e.ID != item.ID).ToList();
                                    if (latestExistingItemsExceptCurrentItem != null && latestExistingItemsExceptCurrentItem.Any())
                                    {
                                        var totalOflatestExistingItems =
                                            isOrderStatusSubmit
                                            ? (latestExistingItemsExceptCurrentItem.Sum(e => e.ApprovedQuantity.GetValueOrDefault(0)))
                                            : (latestExistingItemsExceptCurrentItem.Sum(e => e.RequestedQuantity.GetValueOrDefault(0)));
                                        tmpItemOnOrderQty = tmpItemOnOrderQty + (totalOflatestExistingItems);
                                    }
                                }

                                if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 && (tmpItemOnOrderQty + (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0))) > objItemMasterDTO.MaximumQuantity.Value)
                                {
                                    var msg = "\n" + string.Format(ResOrder.ItemNotAddedMaxQtyReached, objItemMasterDTO.ItemNumber);
                                    message = (message == "" ? msg : message + msg);
                                    rejectedDueToPreventMaxValidation++;
                                    continue;
                                }
                            }
                            else
                            {
                                if (!itemLevelOrderQtyForEdit.ContainsKey(objItemMasterDTO.GUID))
                                {
                                    itemLevelOrderQtyForEdit[objItemMasterDTO.GUID] = (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0));
                                }
                                else
                                {
                                    itemLevelOrderQtyForEdit[objItemMasterDTO.GUID] = itemLevelOrderQtyForEdit[objItemMasterDTO.GUID] + ((isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0)));
                                }

                                var existingLineItems = objOrder.OrderListItem.Where(e => e.ItemNumber.Equals(objItemMasterDTO.ItemNumber)).ToList();

                                if (existingLineItems != null && existingLineItems.Any())
                                {
                                    var totalOfExistingQty = (existingLineItems.Sum(e => isOrderStatusHigherThanSubmit ? e.ApprovedQuantity.GetValueOrDefault(0) : e.RequestedQuantity.GetValueOrDefault(0)));
                                    tmpItemOnOrderQty = tmpItemOnOrderQty - (totalOfExistingQty);
                                    var latestExistingItems = lstDetails.Where(e => e.ItemGUID.Equals(objItemMasterDTO.GUID) && e.ID > 0).ToList();

                                    if (latestExistingItems != null && latestExistingItems.Any())
                                    {
                                        var totalOflatestExistingItems =
                                            isOrderStatusSubmit
                                            ? (latestExistingItems.Sum(e => e.ApprovedQuantity.GetValueOrDefault(0)))
                                            : (latestExistingItems.Sum(e => e.RequestedQuantity.GetValueOrDefault(0)));
                                        tmpItemOnOrderQty = tmpItemOnOrderQty + (totalOflatestExistingItems);
                                    }
                                }

                                if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 && (itemLevelOrderQtyForEdit[objItemMasterDTO.GUID] + tmpItemOnOrderQty) > objItemMasterDTO.MaximumQuantity.Value)
                                {
                                    itemLevelOrderQtyForEdit[objItemMasterDTO.GUID] = itemLevelOrderQtyForEdit[objItemMasterDTO.GUID] - (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0));
                                    var msg = "\n " + string.Format(ResOrder.ItemNotAddedMaxQtyReached, objItemMasterDTO.ItemNumber);
                                    message = (message == "" ? msg : message + msg);
                                    rejectedDueToPreventMaxValidation++;
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            double itemOrderQtySoFar = 0;

                            if (itemsOrderQtyForItemMinMax.ContainsKey(objItemMasterDTO.GUID))
                            {
                                itemOrderQtySoFar += itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID];
                            }

                            if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 && (objItemMasterDTO.OnOrderQuantity.GetValueOrDefault(0) + itemOrderQtySoFar + (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0))) > objItemMasterDTO.MaximumQuantity.Value)
                            {
                                var msg = "\n " + string.Format(ResOrder.ItemNotAddedMaxQtyReached, objItemMasterDTO.ItemNumber);
                                message = (message == "" ? msg : message + msg);
                                rejectedDueToPreventMaxValidation++;
                                continue;
                            }
                            else
                            {
                                itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID] = (
                                        itemsOrderQtyForItemMinMax.ContainsKey(objItemMasterDTO.GUID)
                                        ? (itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID] + (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0)))
                                        : (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0))
                                        );
                            }
                        }
                    }
                    else
                    {
                        var maxQtyAtBinLevel = lstOfOrderLineItemBin.Where(e => e.BinNumber.Equals(item.BinName) && e.ItemGUID.GetValueOrDefault(Guid.Empty) == objItemMasterDTO.GUID).FirstOrDefault();
                        var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : item.Bin.GetValueOrDefault(0);
                        var onOrderQtyAtBin = objDAL.GetOrderdQtyOfItemBinWise(RoomID, CompanyID, objItemMasterDTO.GUID, tmpBinId);
                        var tmponOrderQtyAtBin = onOrderQtyAtBin;

                        if (OrderID > 0 && objOrder != null && objOrder.OrderListItem.Any())
                        {
                            var lineItemExistingQty = objOrder.OrderListItem.Where(e => e.ItemNumber.Equals(objItemMasterDTO.ItemNumber) && item.ID > 0 && e.ID == item.ID && e.Bin.GetValueOrDefault(0).Equals(tmpBinId)).FirstOrDefault();

                            if (lineItemExistingQty != null && (isOrderStatusHigherThanSubmit ? (lineItemExistingQty.ApprovedQuantity.GetValueOrDefault(0)) : (lineItemExistingQty.RequestedQuantity.GetValueOrDefault(0))) > 0)
                            {
                                var existingLineItems = objOrder.OrderListItem.Where(e => e.ItemNumber.Equals(objItemMasterDTO.ItemNumber) && e.BinName.Equals(item.BinName)).ToList();

                                if (existingLineItems != null && existingLineItems.Any())
                                {
                                    var totalOfExistingQty = (existingLineItems.Sum(e => (isOrderStatusHigherThanSubmit ? e.ApprovedQuantity.GetValueOrDefault(0) : e.RequestedQuantity.GetValueOrDefault(0))));
                                    tmponOrderQtyAtBin = tmponOrderQtyAtBin - (totalOfExistingQty);
                                    var latestExistingItemsExceptCurrentItem = lstDetails.Where(e => e.ItemGUID.Equals(objItemMasterDTO.GUID) && e.BinName.Equals(item.BinName) && e.ID > 0 && e.ID != item.ID).ToList();

                                    if (latestExistingItemsExceptCurrentItem != null && latestExistingItemsExceptCurrentItem.Any())
                                    {
                                        var totalOflatestExistingItems =
                                            isOrderStatusSubmit
                                            ? (latestExistingItemsExceptCurrentItem.Sum(e => e.ApprovedQuantity.GetValueOrDefault(0)))
                                            : (latestExistingItemsExceptCurrentItem.Sum(e => e.RequestedQuantity.GetValueOrDefault(0)));

                                        tmponOrderQtyAtBin = tmponOrderQtyAtBin + (totalOflatestExistingItems);
                                    }
                                }

                                if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0 && (tmponOrderQtyAtBin + (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0))) > maxQtyAtBinLevel.MaximumQuantity.Value)
                                {
                                    var msg = "\n " + string.Format(ResOrder.ItemNotAddedBinMaxQtyReached, objItemMasterDTO.ItemNumber);
                                    message = (message == "" ? msg : message + msg);
                                    rejectedDueToPreventMaxValidation++;
                                    continue;
                                }
                            }
                            else
                            {
                                if (!binLevelOrderQtyForEdit.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName))
                                {
                                    binLevelOrderQtyForEdit[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] = (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0));
                                }
                                else
                                {
                                    binLevelOrderQtyForEdit[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] = binLevelOrderQtyForEdit[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] + (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0));
                                }

                                var existingLineItems = objOrder.OrderListItem.Where(e => e.ItemNumber.Equals(objItemMasterDTO.ItemNumber) && e.Bin.GetValueOrDefault(0).Equals(tmpBinId)).ToList();

                                if (existingLineItems != null && existingLineItems.Any())
                                {
                                    var totalOfExistingQty = (existingLineItems.Sum(e => (isOrderStatusHigherThanSubmit ? e.ApprovedQuantity.GetValueOrDefault(0) : e.RequestedQuantity.GetValueOrDefault(0))));
                                    tmponOrderQtyAtBin = tmponOrderQtyAtBin - (totalOfExistingQty);
                                    var latestExistingItems = lstDetails.Where(e => e.ItemGUID.Equals(objItemMasterDTO.GUID) && e.BinName.Equals(item.BinName) && e.ID > 0).ToList();

                                    if (latestExistingItems != null && latestExistingItems.Any())
                                    {
                                        var totalOflatestExistingItems =
                                            isOrderStatusSubmit
                                            ? (latestExistingItems.Sum(e => e.ApprovedQuantity.GetValueOrDefault(0)))
                                            : (latestExistingItems.Sum(e => e.RequestedQuantity.GetValueOrDefault(0)));

                                        tmponOrderQtyAtBin = tmponOrderQtyAtBin + (totalOflatestExistingItems);
                                    }
                                }

                                if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0 && (binLevelOrderQtyForEdit[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] + tmponOrderQtyAtBin) > maxQtyAtBinLevel.MaximumQuantity.Value)
                                {
                                    binLevelOrderQtyForEdit[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] = binLevelOrderQtyForEdit[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] - (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0));
                                    var msg = "\n" + string.Format(ResOrder.ItemNotAddedBinMaxQtyReached, objItemMasterDTO.ItemNumber);
                                    message = (message == "" ? msg : message + msg);
                                    rejectedDueToPreventMaxValidation++;
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            double itemOrderQtySoFar = 0;

                            if (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName))
                            {
                                itemOrderQtySoFar += itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName];
                            }

                            if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0
                                && (tmponOrderQtyAtBin + itemOrderQtySoFar + (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0))) > maxQtyAtBinLevel.MaximumQuantity.Value)
                            {
                                var msg = "\n " + string.Format(ResOrder.ItemNotAddedBinMaxQtyReached, objItemMasterDTO.ItemNumber);
                                message = (message == "" ? msg : message + msg);
                                rejectedDueToPreventMaxValidation++;
                                continue;
                            }
                            else
                            {
                                itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] =
                                    (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName)
                                    ? (itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] + (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0)))
                                    : (isOrderStatusSubmit ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0))
                                    );
                            }
                        }
                    }
                }
            }

            if (rejectedDueToPreventMaxValidation > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method is used to validate order line items for Item/Bin max order qty(WI-4561) on unclose order.
        /// </summary>
        /// <param name="OrderGuid"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ValidateOrderLineItemsForUncloseOrder(Guid OrderGuid, out string message)
        {
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objOrder = new OrderMasterDTO();
            OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            int rejectedDueToPreventMaxValidation = 0;
            int rejectedDueToDeletedItem = 0;
            message = string.Empty;
            Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
            Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
            bool isPreviousOrderStatusApprovedOrHigher = false;

            if (OrderGuid != Guid.Empty)
            {
                objOrder = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByGuidPlain(OrderGuid);

                if (objOrder != null && objOrder.ID > 0)
                {
                    //var objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    // var objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,PreventMaxOrderQty";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                    objOrder.OrderListItem = objDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objOrder.GUID, RoomID, CompanyID, false, SessionHelper.UserSupplierIds);
                    foreach (var item in objOrder.OrderListItem)
                    {
                        ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                        objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((item.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objItemMasterDTO == null)
                        {
                            ItemMasterDTO objItem = objItemMasterDAL.GetDeleteUndeleteRecordByItemGUID(item.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (objItem != null && objItem.IsDeleted == true)
                            {
                                var msg = "\n" + string.Format(ResItemMaster.ItemHasDeleted, item.ItemNumber);
                                message = (message == "" ? msg : message + msg);
                                rejectedDueToDeletedItem++;
                            }
                            continue;
                        }
                    }
                    if (rejectedDueToDeletedItem > 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (!(objOrder.OrderType == (int)OrderType.Order && objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder))
                        {
                            return true;
                        }
                    }
                    int orderPreviousStatus = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderPreviousStatus(objOrder.ID).GetValueOrDefault((int)OrderStatus.UnSubmitted);
                    isPreviousOrderStatusApprovedOrHigher = (orderPreviousStatus >= (int)OrderStatus.Approved);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            foreach (var item in objOrder.OrderListItem)
            {
                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((item.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objItemMasterDTO == null)
                {
                    ItemMasterDTO objItem = objItemMasterDAL.GetDeleteUndeleteRecordByItemGUID(item.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objItem != null && objItem.IsDeleted == true)
                    {
                        var msg = "\n" + string.Format(ResItemMaster.ItemHasDeleted, item.ItemNumber);
                        message = (message == "" ? msg : message + msg);
                        rejectedDueToDeletedItem++;
                    }
                    continue;
                }
                if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired.HasValue && objItemMasterDTO.IsItemLevelMinMaxQtyRequired.Value)
                {
                    double itemOrderQtySoFar = 0;

                    if (itemsOrderQtyForItemMinMax.ContainsKey(objItemMasterDTO.GUID))
                    {
                        itemOrderQtySoFar += itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID];
                    }

                    if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 &&
                        (objItemMasterDTO.OnOrderQuantity.GetValueOrDefault(0) + itemOrderQtySoFar + (isPreviousOrderStatusApprovedOrHigher
                        ? item.ApprovedQuantity.GetValueOrDefault(0)
                        : item.RequestedQuantity.GetValueOrDefault(0))) > objItemMasterDTO.MaximumQuantity.Value)
                    {
                        var msg = "\n " + string.Format(ResOrder.ItemCantUnclosedItemMaxQtyReached, objItemMasterDTO.ItemNumber);
                        message = (message == "" ? msg : message + msg);
                        rejectedDueToPreventMaxValidation++;
                        continue;
                    }
                    else
                    {
                        itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID] = (
                                itemsOrderQtyForItemMinMax.ContainsKey(objItemMasterDTO.GUID)
                                ? (itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID] + (isPreviousOrderStatusApprovedOrHigher ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0)))
                                : (isPreviousOrderStatusApprovedOrHigher ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0))
                                );
                    }
                }
                else
                {
                    List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemMasterDTO.GUID, RoomID, CompanyID).OrderBy(x => x.BinNumber).ToList();
                    var maxQtyAtBinLevel = lstItemBins.Where(e => e.BinNumber.Equals(item.BinName)).FirstOrDefault();
                    var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : item.Bin.GetValueOrDefault(0);
                    var onOrderQtyAtBin = objDAL.GetOrderdQtyOfItemBinWise(RoomID, CompanyID, objItemMasterDTO.GUID, tmpBinId);
                    var tmponOrderQtyAtBin = onOrderQtyAtBin;
                    double itemOrderQtySoFar = 0;

                    if (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName))
                    {
                        itemOrderQtySoFar += itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName];
                    }

                    if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0
                        && (tmponOrderQtyAtBin + itemOrderQtySoFar + (isPreviousOrderStatusApprovedOrHigher
                        ? item.ApprovedQuantity.GetValueOrDefault(0)
                        : item.RequestedQuantity.GetValueOrDefault(0))) > maxQtyAtBinLevel.MaximumQuantity.Value)
                    {
                        var msg = "\n" + string.Format(ResOrder.ItemCantUnclosedBinMaxQtyReached, objItemMasterDTO.ItemNumber);
                        message = (message == "" ? msg : message + msg);
                        rejectedDueToPreventMaxValidation++;
                        continue;
                    }
                    else
                    {
                        itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] =
                            (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName)
                            ? (itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + item.BinName] + (isPreviousOrderStatusApprovedOrHigher ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0)))
                            : (isPreviousOrderStatusApprovedOrHigher ? item.ApprovedQuantity.GetValueOrDefault(0) : item.RequestedQuantity.GetValueOrDefault(0))
                            );
                    }
                }
            }

            if (rejectedDueToPreventMaxValidation > 0 || rejectedDueToDeletedItem > 0)
            {
                return false;
            }
            return true;
        }

        private bool ValidateUOMForUncloseOrder(Guid OrderGuid)
        {
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objOrder = new OrderMasterDTO();
            OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            bool IsSuccess = true;

            if (OrderGuid != Guid.Empty)
            {
                objOrder = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByGuidPlain(OrderGuid);

                if (objOrder != null && objOrder.ID > 0)
                {
                    if (!(objOrder.OrderType == (int)OrderType.Order))
                    {
                        return true;
                    }

                    objOrder.OrderListItem = objDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objOrder.GUID, RoomID, CompanyID, false, SessionHelper.UserSupplierIds);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            double Modulo = 0;

            if (objOrder != null && objOrder.OrderListItem != null && objOrder.OrderListItem.Any() && objOrder.OrderListItem.Count() > 0)
            {
                foreach (var OrderItem in objOrder.OrderListItem)
                {
                    if (OrderItem.IsCloseItem.GetValueOrDefault(false) == false)
                    {
                        ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                        objItemMasterDTO = objItemMasterDAL.GetRecordByItemGUID((OrderItem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objItemMasterDTO.IsAllowOrderCostuom && (OrderItem.RequestedQuantity != null || OrderItem.ApprovedQuantity != null || OrderItem.ReceivedQuantity != null))
                        {
                            if (OrderItem.RequestedQuantity != null && OrderItem.RequestedQuantity > 0)
                            {
                                Modulo = (OrderItem.RequestedQuantity ?? 0) % (objItemMasterDTO.OrderUOMValue ?? 1);
                                //if (Modulo == 0 && (OrderItem.RequestedQuantity / objItemMasterDTO.OrderUOMValue) != OrderItem.RequestedQuantityUOM)
                                //    Modulo = 1;
                            }

                            if (Modulo == 0 && OrderItem.ApprovedQuantity != null && OrderItem.ApprovedQuantity > 0)
                                Modulo = (OrderItem.ApprovedQuantity ?? 0) % (objItemMasterDTO.OrderUOMValue ?? 1);

                            if (Modulo == 0 && OrderItem.ReceivedQuantity != null && OrderItem.ReceivedQuantity > 0)
                                Modulo = (OrderItem.ReceivedQuantity ?? 0) % (objItemMasterDTO.OrderUOMValue ?? 1);

                            if (Modulo != 0)
                            {
                                // Error
                                IsSuccess = false;
                                break;
                            }
                        }
                        else if (objItemMasterDTO.IsAllowOrderCostuom == false)
                        {
                            if ((OrderItem.RequestedQuantity ?? 0) != (OrderItem.RequestedQuantityUOM ?? 0))
                                Modulo = 1;

                            if (Modulo == 0 && ((OrderItem.ApprovedQuantity ?? 0) != (OrderItem.ApprovedQuantityUOM ?? 0)))
                                Modulo = 1;

                            if (Modulo == 0 && ((OrderItem.ReceivedQuantity ?? 0) != (OrderItem.ReceivedQuantityUOM ?? 0)))
                                Modulo = 1;

                            if (Modulo != 0)
                            {
                                IsSuccess = false;
                                break;
                            }
                        }
                    }
                }
            }

            return IsSuccess;
        }

        private bool ValidateUOMForUncloseOrderLineItems(string IDs, out string NewIDs, out string ItemNameError, out string ItemDeletedError)
        {
            string[] strArrOrderDetailIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            NewIDs = string.Empty;
            ItemNameError = string.Empty;
            ItemDeletedError = string.Empty;
            bool IsSuccess = true;

            OrderDetailsDTO objOrdDetailDTO = objDAL.GetOrderDetailByIDNormal(Convert.ToInt64(strArrOrderDetailIDs[0]), SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objOrdDetailDTO != null && objOrdDetailDTO.ID > 0)
            {
                if (!(objOrdDetailDTO.OrderType == (int)OrderType.Order))
                {
                    NewIDs = IDs;
                    return true;
                }
            }

            foreach (string strOrderDetailsID in strArrOrderDetailIDs)
            {
                double Modulo = 0;
                OrderDetailsDTO objOrdDtlDTO = objDAL.GetOrderDetailByIDPlain(Convert.ToInt64(strOrderDetailsID), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objOrdDtlDTO != null)
                {
                    ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                    objItemMasterDTO = objItemMasterDAL.GetRecordByItemGUID((objOrdDtlDTO.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objItemMasterDTO == null)
                    {
                        IsSuccess = false; // Error
                        ItemMasterDTO objItem = objItemMasterDAL.GetDeleteUndeleteRecordByItemGUID(objOrdDtlDTO.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objItem != null && objItem.IsDeleted == true)
                        {
                            ItemDeletedError = ItemDeletedError + objOrdDetailDTO.ItemNumber + ",";
                        }
                        continue;
                    }
                    if (objItemMasterDTO.IsAllowOrderCostuom && (objOrdDtlDTO.RequestedQuantity != null || objOrdDtlDTO.ApprovedQuantity != null || objOrdDtlDTO.ReceivedQuantity != null))
                    {
                        if (objOrdDtlDTO.RequestedQuantity != null && objOrdDtlDTO.RequestedQuantity > 0)
                        {
                            Modulo = (objOrdDtlDTO.RequestedQuantity ?? 0) % (objItemMasterDTO.OrderUOMValue ?? 1);

                        }

                        if (Modulo == 0 && objOrdDtlDTO.ApprovedQuantity != null && objOrdDtlDTO.ApprovedQuantity > 0)
                            Modulo = (objOrdDtlDTO.ApprovedQuantity ?? 0) % (objItemMasterDTO.OrderUOMValue ?? 1);

                        if (Modulo == 0 && objOrdDtlDTO.ReceivedQuantity != null && objOrdDtlDTO.ReceivedQuantity > 0)
                            Modulo = (objOrdDtlDTO.ReceivedQuantity ?? 0) % (objItemMasterDTO.OrderUOMValue ?? 1);

                        if (Modulo != 0)
                        {
                            IsSuccess = false; // Error
                            ItemNameError = ItemNameError + objItemMasterDTO.ItemNumber + ",";
                        }
                        else
                        {
                            NewIDs = NewIDs + strOrderDetailsID + ",";
                        }
                    }
                    else if (objItemMasterDTO.IsAllowOrderCostuom == false)
                    {
                        if ((objOrdDtlDTO.RequestedQuantity ?? 0) != (objOrdDtlDTO.RequestedQuantityUOM ?? 0))
                            Modulo = 1;

                        if (Modulo == 0 && ((objOrdDtlDTO.ApprovedQuantity ?? 0) != (objOrdDtlDTO.ApprovedQuantityUOM ?? 0)))
                            Modulo = 1;

                        if (Modulo == 0 && ((objOrdDtlDTO.ReceivedQuantity ?? 0) != (objOrdDtlDTO.ReceivedQuantityUOM ?? 0)))
                            Modulo = 1;

                        if (Modulo != 0)
                        {
                            IsSuccess = false; // Error
                            ItemNameError = ItemNameError + objItemMasterDTO.ItemNumber + ",";
                        }
                        else
                        {
                            NewIDs = NewIDs + strOrderDetailsID + ",";
                        }
                    }
                }
            }
            return IsSuccess;
        }

        /// <summary>
        /// Add New Item to Order
        /// </summary>
        /// <param name="objNewItems"></param>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult AddItemsToOrder(OrderDetailsDTO[] objNewItems, Int64 OrderID)
        {
            string message = ResOrder.NotInsert;
            string status = "fail";
            OrderMasterDTO objOrderDTO = null;
            OrderMasterDAL objOrderDAL = null;
            MaterialStagingDTO objMSDTO = null;
            BinMasterDAL objBinDAL = null;
            ItemMasterDTO objItemDTO = null;
            ItemMasterDAL objItemDAL = null;
            SupplierBlanketPODetailsDTO objSupplierBlkPO = null;
            OrderDetailsDTO objNewDetailDTO = null;
            SupplierBlanketPODetailsDAL objSupBlnaPODAL = null;
            List<OrderDetailsDTO> lstReturnsForSameItemWithBin = null;
            List<DTOForAutoComplete> lstAddedItemsBin = null;
            Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
            Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
            int rejectedDueToPreventMaxValidation = 0;
            OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

            try
            {
                objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                List<OrderDetailsDTO> lstDetails = GetLineItemsFromSession(OrderID);
                objOrderDTO = objOrderDAL.GetOrderByIdPlain(OrderID);
                string binName = string.Empty;
                Int64? binID = null;

                objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                if (objOrderDTO.MaterialStagingGUID != null)
                {
                    objMSDTO = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetRecord(objOrderDTO.MaterialStagingGUID.Value, RoomID, CompanyID);
                    if (!string.IsNullOrEmpty(objMSDTO.StagingLocationName))
                    {
                        binName = objMSDTO.StagingLocationName;
                    }
                }


                if (objNewItems != null && objNewItems.Length > 0)
                {
                    #region WI-7318	AB Integration | Sync Item cost when an item Added to Order line item.
                    List<string> ASINs = new List<string>();
                    Dictionary<List<ItemMasterDTO>, string> lstNonOrderableItems = new Dictionary<List<ItemMasterDTO>, string>();
                    if (SessionHelper.AllowABIntegration)
                    {
                        foreach (var ABitem in objNewItems)
                        {
                            ItemMasterDTO objABItemDTO = new ItemMasterDTO();
                            objABItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, ABitem.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                            if (objABItemDTO != null && !string.IsNullOrWhiteSpace(objABItemDTO.SupplierPartNo))
                            {
                                ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                                Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objABItemDTO.SupplierPartNo, objABItemDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID);
                                if (ABItemMappingID > 0)
                                {
                                    ASINs.Add(objABItemDTO.SupplierPartNo);
                                }
                            }
                        }
                        if (ASINs != null && ASINs.Count > 0)
                        {
                            lstNonOrderableItems = ABAPIHelper.ItemSyncToRoom(ASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                        }
                        if (lstNonOrderableItems != null && lstNonOrderableItems.Count > 0
                            && lstNonOrderableItems.Values.Contains("success")
                             && lstNonOrderableItems.Keys.Count > 0
                             && lstNonOrderableItems.Keys.SelectMany(c => c).ToList().Count > 0)
                        {
                            List<ItemMasterDTO> lstReturnsItems = new List<ItemMasterDTO>();
                            lstReturnsItems = lstNonOrderableItems.Keys.SelectMany(c => c).ToList();
                            objNewItems = objNewItems.Where(x => !lstReturnsItems.Select(y => y.GUID).Contains(x.ItemGUID.GetValueOrDefault(Guid.Empty))).ToArray();
                        }
                    }
                    #endregion

                    lstReturnsForSameItemWithBin = new List<OrderDetailsDTO>();
                    lstAddedItemsBin = new List<DTOForAutoComplete>();


                    foreach (var item in objNewItems)
                    {
                        binID = item.Bin;
                        binName = item.BinName;
                        if (string.IsNullOrEmpty(binName))
                            binName = item.BinName;

                        objItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                        if (SessionHelper.AllowABIntegration && ASINs != null && ASINs.Count > 0
                            && objItemDTO != null
                            && ASINs.Contains(objItemDTO.SupplierPartNo)
                            && item.ItemCost.GetValueOrDefault(0) != objItemDTO.Cost.GetValueOrDefault(0))
                        {
                            item.ItemCost = objItemDTO.Cost.GetValueOrDefault(0);
                        }
                        if (objItemDTO.IsAllowOrderCostuom)
                        {
                            if (objItemDTO.OrderUOMValue == null || objItemDTO.OrderUOMValue <= 0)
                                objItemDTO.OrderUOMValue = 1;
                            objItemDTO.DefaultReorderQuantity = objItemDTO.DefaultReorderQuantity / objItemDTO.OrderUOMValue;
                        }
                        List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemDTO.GUID, RoomID, CompanyID).OrderBy(x => x.BinNumber).ToList();

                        if (lstItemBins != null && lstItemBins.Where(x => x.ID == binID).Count() > 0 && String.IsNullOrEmpty(binName))
                        {
                            item.BinName = lstItemBins.Where(x => x.ID == binID).ToList()[0].BinNumber;
                            binName = item.BinName;
                        }

                        if (binID == null && string.IsNullOrEmpty(binName))
                        {
                            binID = objItemDTO.DefaultLocation;
                            binName = objItemDTO.DefaultLocationName;
                        }
                        else if (binID.GetValueOrDefault(0) <= 0 && string.IsNullOrEmpty(binName))
                        {
                            binID = objItemDTO.DefaultLocation;
                            binName = objItemDTO.DefaultLocationName;
                        }
                        else if (binID.GetValueOrDefault(0) <= 0 && !string.IsNullOrEmpty(binName))
                        {
                            binID = objBinDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), binName, SessionHelper.UserID, RoomID, CompanyID, objOrderDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty);
                        }

                        item.Bin = binID;
                        item.BinName = binName;
                        objSupBlnaPODAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
                        //WI-8470
                        objSupplierBlkPO = objSupBlnaPODAL.GetBlanketPOByItemSupplierID(objOrderDTO.Supplier.GetValueOrDefault(0), RoomID, CompanyID, objItemDTO.GUID).FirstOrDefault();

                        if (objItemDTO.CostUOMValue == null || objItemDTO.CostUOMValue <= 0)
                        {
                            objItemDTO.CostUOMValue = 1;
                        }

                        if (item.Bin.HasValue && lstItemBins != null)
                        {
                            var itemBin = lstItemBins.Where(x => x.ID == item.Bin.GetValueOrDefault(0)).FirstOrDefault();

                            if (itemBin != null && itemBin.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true && itemBin.DefaultReorderQuantity.GetValueOrDefault(0) > 0)
                            {
                                objItemDTO.DefaultReorderQuantity = (!objItemDTO.IsAllowOrderCostuom)
                                    ? itemBin.DefaultReorderQuantity.GetValueOrDefault(0)
                                    : (itemBin.DefaultReorderQuantity.GetValueOrDefault(0) / objItemDTO.OrderUOMValue.GetValueOrDefault(0));
                            }
                        }

                        objNewDetailDTO = UpdateOrderDetailWithFullInfo(item, objItemDTO, objSupplierBlkPO, objOrderDTO.OrderStatus);

                        DTOForAutoComplete objAdd = new DTOForAutoComplete()
                        {
                            ItemGuid = objItemDTO.GUID,
                            Key = objItemDTO.ItemNumber,
                            ID = 0,
                            Value = "",
                            OtherInfo2 = objItemDTO.DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? objItemDTO.DefaultReorderQuantity.GetValueOrDefault(0).ToString() : ""
                        };

                        if (lstItemBins.Count() > 0)
                        {
                            BinMasterDTO binDTO = lstItemBins.Where(x => (x.IsDefault ?? false) == true).FirstOrDefault();
                            if (binDTO == null)
                            {
                                binDTO = lstItemBins.FirstOrDefault();
                            }

                            objAdd.ID = binDTO.ID;
                            objAdd.Value = binDTO.BinNumber;
                            objAdd.OtherInfo2 = binDTO.DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? binDTO.DefaultReorderQuantity.GetValueOrDefault(0).ToString() : "";

                            if (!string.IsNullOrEmpty(objAdd.OtherInfo2) && Convert.ToInt64(objAdd.OtherInfo2) > 0 && binDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true && objItemDTO.IsAllowOrderCostuom)
                            {
                                objAdd.OtherInfo2 = Convert.ToString(binDTO.DefaultReorderQuantity.GetValueOrDefault(0) / objItemDTO.OrderUOMValue);
                            }
                        }

                        lstAddedItemsBin.Add(objAdd);
                        if (!string.IsNullOrEmpty(objAdd.OtherInfo2))
                        {
                            if (Convert.ToDouble(objAdd.OtherInfo2) > objNewDetailDTO.DefaultReorderQuantity)
                            {
                                objNewDetailDTO.DefaultReorderQuantity = Convert.ToDouble(objAdd.OtherInfo2);
                            }
                        }
                        objNewDetailDTO.hasPOItemNumber = (objNewDetailDTO.POItemLineNumber != null && objNewDetailDTO.POItemLineNumber > 0) ? true : false;
                        lstDetails.Add(objNewDetailDTO);
                    }

                    SessionHelper.Add(GetSessionKey(OrderID), lstDetails);
                    if (lstReturnsForSameItemWithBin.Count <= 0 && rejectedDueToPreventMaxValidation <= 0)
                    {
                        message = (message == ResOrder.NotInsert
                            ? string.Format(ResOrder.ItemsAddedToOrder, objNewItems.Length)
                            : message + string.Format(ResOrder.ItemsAddedToOrder, objNewItems.Length));
                    }
                    else if (objNewItems.Length - (lstReturnsForSameItemWithBin.Count + rejectedDueToPreventMaxValidation) > 0)
                    {
                        var msg = string.Format(ResOrder.ItemsAddedAndExistInOrder, (objNewItems.Length - lstReturnsForSameItemWithBin.Count + rejectedDueToPreventMaxValidation), lstReturnsForSameItemWithBin.Count);
                        message = (message == ResOrder.NotInsert
                            ? msg
                            : message + msg);
                    }
                    else
                    {
                        message = (message == ResOrder.NotInsert
                            ? ResOrder.NotAddedItemsExistInOrder
                            : message + ResOrder.NotAddedItemsExistInOrder);
                    }
                    if (SessionHelper.AllowABIntegration)
                    {
                        if (lstNonOrderableItems != null && lstNonOrderableItems.Count > 0)
                        {
                            foreach (ItemMasterDTO item in lstNonOrderableItems.Keys.SelectMany(c => c).ToList())
                            {
                                message = (string.IsNullOrEmpty(message)
                                            ? string.Format(ResOrder.ItemnotOrderable, item.ItemNumber)
                                            : message + string.Format(ResOrder.ItemnotOrderable, item.ItemNumber));
                            }
                        }
                    }
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
            }
            finally
            {
                objOrderDTO = null;
                objOrderDAL = null;
                objMSDTO = null;
                objBinDAL = null;
                objItemDTO = null;
                objItemDAL = null;
                objSupplierBlkPO = null;
                objNewDetailDTO = null;
                objSupBlnaPODAL = null;
            }
        }
    
        private OrderDetailsDTO UpdateOrderDetailWithFullInfo(OrderDetailsDTO item, ItemMasterDTO objItemDTO, SupplierBlanketPODetailsDTO objSupplierBlkPO, int intOrderStatus)
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

            if (objItemDTO != null && item.QuickListGUID != null && item.QuickListGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                item.ItemCost = objItemDTO.Cost.GetValueOrDefault(0);
            }

            OrderDetailsDTO objNewDetailDTO = new OrderDetailsDTO()
            {
                ID = 0,
                RequestedQuantity = item.RequestedQuantity,
                ApprovedQuantity = item.ApprovedQuantity.GetValueOrDefault(0),
                OrderGUID = item.OrderGUID,
                ItemGUID = item.ItemGUID,
                ItemNumber = objItemDTO.ItemNumber,
                Bin = item.Bin,
                CostUOMValue = objItemDTO.CostUOMValue,
                BinName = item.BinName,
                RequiredDate = objItemDTO.LeadTimeInDays.GetValueOrDefault(0) > 0 ? datetimetoConsider.AddDays(objItemDTO.LeadTimeInDays.GetValueOrDefault(0)) : item.RequiredDate,
                ReceivedQuantity = item.ReceivedQuantity,
                Room = RoomID,
                CompanyID = CompanyID,
                Consignment = objItemDTO.Consignment,
                Cost = objItemDTO.Cost,
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
                CostUOM = objItemDTO.CostUOMName,
                CategoryID = objItemDTO.CategoryID,
                Category = objItemDTO.CategoryName,
                CriticalQuantity = objItemDTO.CriticalQuantity.GetValueOrDefault(0),
                AverageCost = objItemDTO.AverageCost,
                AverageUsage = objItemDTO.AverageUsage,
                Action = string.Empty,
                DateCodeTracking = objItemDTO.DateCodeTracking,
                DefaultLocation = objItemDTO.DefaultLocation,
                DefaultLocationName = objItemDTO.DefaultLocationName,
                DefaultPullQuantity = objItemDTO.DefaultPullQuantity.GetValueOrDefault(0),
                DefaultReorderQuantity = objItemDTO.DefaultReorderQuantity.GetValueOrDefault(0),
                ExtendedCost = objItemDTO.ExtendedCost,
                GLAccount = objItemDTO.GLAccount,
                GLAccountID = objItemDTO.GLAccountID,
                GUID = Guid.Empty,
                HistoryID = 0,
                ImagePath = objItemDTO.ImagePath,
                InTransitQuantity = 0,
                InventoryClassification = objItemDTO.InventoryClassification,
                IsBuildBreak = objItemDTO.IsBuildBreak,
                IsEnforceDefaultReorderQuantity = objItemDTO.IsEnforceDefaultReorderQuantity,
                IsHistory = false,
                IsItemLevelMinMaxQtyRequired = objItemDTO.IsItemLevelMinMaxQtyRequired,
                IsLotSerialExpiryCost = objItemDTO.IsLotSerialExpiryCost,
                IsPurchase = objItemDTO.IsPurchase,
                IsTransfer = objItemDTO.IsTransfer,
                ItemCreated = objItemDTO.Created,
                ItemCreatedByName = objItemDTO.CreatedByName,
                ItemDescription = objItemDTO.Description,
                ItemID = objItemDTO.ID,
                ItemInTransitQuantity = objItemDTO.InTransitquantity,
                ItemIsArchived = objItemDTO.IsArchived,
                ItemIsDeleted = objItemDTO.IsDeleted,
                ItemLastUpdatedBy = objItemDTO.LastUpdatedBy,
                ItemRoom = objItemDTO.Room,
                ItemRoomName = objItemDTO.RoomName,
                ItemType = objItemDTO.ItemType,
                ItemUDF1 = objItemDTO.UDF1,
                ItemUDF2 = objItemDTO.UDF2,
                ItemUDF3 = objItemDTO.UDF3,
                ItemUDF4 = objItemDTO.UDF4,
                ItemUDF5 = objItemDTO.UDF5,
                ItemUDF6 = objItemDTO.UDF6,
                ItemUDF7 = objItemDTO.UDF7,
                ItemUDF8 = objItemDTO.UDF8,
                ItemUDF9 = objItemDTO.UDF9,
                ItemUDF10 = objItemDTO.UDF10,
                ItemUniqueNumber = objItemDTO.ItemUniqueNumber,
                ItemUpdated = objItemDTO.Updated,
                ItemUpdatedByName = objItemDTO.UpdatedByName,
                ItemViewGUID = objItemDTO.GUID,
                LastUpdated = DateTimeUtility.DateTimeNow,
                LastUpdatedBy = SessionHelper.UserID,
                LeadTimeInDays = objItemDTO.LeadTimeInDays,
                Link1 = objItemDTO.Link1,
                Link2 = objItemDTO.Link2,
                LongDescription = objItemDTO.LongDescription,
                LotNumberTracking = objItemDTO.LotNumberTracking,
                Manufacturer = objItemDTO.ManufacturerName,
                ManufacturerID = objItemDTO.ManufacturerID,
                ManufacturerNumber = objItemDTO.ManufacturerNumber,
                Markup = objItemDTO.Markup,
                MaximumQuantity = objItemDTO.MaximumQuantity.GetValueOrDefault(0),
                MinimumQuantity = objItemDTO.MinimumQuantity.GetValueOrDefault(0),
                OnHandQuantity = objItemDTO.OnHandQuantity,
                OnOrderQuantity = objItemDTO.OnOrderQuantity,
                OnTransferQuantity = objItemDTO.OnTransferQuantity,
                PackingQuantity = objItemDTO.PackingQuantity,
                PricePerTerm = objItemDTO.PricePerTerm,
                RequisitionedQuantity = objItemDTO.RequisitionedQuantity,
                RoomName = SessionHelper.RoomName,
                SellPrice = objItemDTO.SellPrice,
                SerialNumberTracking = objItemDTO.SerialNumberTracking,
                StagedQuantity = objItemDTO.StagedQuantity,
                SuggestedOrderQuantity = objItemDTO.SuggestedOrderQuantity,
                SupplierID = item.SupplierID.GetValueOrDefault(0) > 0 ? item.SupplierID.GetValueOrDefault(0) : objItemDTO.SupplierID,
                SupplierName = !string.IsNullOrEmpty(item.SupplierName) && !string.IsNullOrWhiteSpace(item.SupplierName) ? item.SupplierName : objItemDTO.SupplierName,
                SupplierPartNo = !string.IsNullOrEmpty(item.SupplierPartNo) && !string.IsNullOrWhiteSpace(item.SupplierPartNo) ? item.SupplierPartNo : objItemDTO.SupplierPartNo,
                Taxable = objItemDTO.Taxable,
                TotalRecords = 0,
                Trend = objItemDTO.Trend,
                Turns = objItemDTO.Turns,
                Unit = objItemDTO.Unit,
                UNSPSC = objItemDTO.UNSPSC,
                UOMID = objItemDTO.UOMID,
                UPC = objItemDTO.UPC,
                UpdatedByName = SessionHelper.UserName,
                WeightPerPiece = objItemDTO.WeightPerPiece,
                ItemBlanketPO = objSupplierBlkPO != null ? objSupplierBlkPO.BlanketPO : "",
                UDF1 = item.UDF1,
                UDF2 = item.UDF2,
                UDF3 = item.UDF3,
                UDF4 = item.UDF4,
                UDF5 = item.UDF5,
                Comment = item.Comment,
                tempDetailsGUID = temp_DetailGUID,
                OnOrderInTransitQuantity = objItemDTO.OnOrderInTransitQuantity,
                OrderLineItemExtendedCost = double.Parse(Convert.ToString((intOrderStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / objItemDTO.CostUOMValue.GetValueOrDefault(1)))
                                                                 : (item.ApprovedQuantity.GetValueOrDefault(0) * (item.ItemCost.GetValueOrDefault(0) / objItemDTO.CostUOMValue.GetValueOrDefault(1)))))),

                OrderLineItemExtendedPrice = double.Parse(Convert.ToString((intOrderStatus <= 2 ? (item.RequestedQuantity.GetValueOrDefault(0) * ((item.ItemCost.GetValueOrDefault(0) + ((item.ItemCost.GetValueOrDefault(0) * objItemDTO.Markup.GetValueOrDefault(0)) / 100)) / objItemDTO.CostUOMValue.GetValueOrDefault(1)))
                                                                 : (item.ApprovedQuantity.GetValueOrDefault(0) * ((item.ItemCost.GetValueOrDefault(0) + ((item.ItemCost.GetValueOrDefault(0) * objItemDTO.Markup.GetValueOrDefault(0)) / 100)) / objItemDTO.CostUOMValue.GetValueOrDefault(1)))))),
                ItemCost = item.ItemCost,
                ItemCostUOM = objItemDTO.CostUOMID,
                ItemCostUOMValue = objItemDTO.CostUOMValue.GetValueOrDefault(1),
                ItemSellPrice = item.ItemCost.GetValueOrDefault(0) + ((item.ItemCost.GetValueOrDefault(0) * objItemDTO.Markup.GetValueOrDefault(0)) / 100),
                ItemMarkup = objItemDTO.Markup.GetValueOrDefault(0),
                POItemLineNumber = objItemDTO.POItemLineNumber
            };

            return objNewDetailDTO;
        }

        public JsonResult SaveLineItemBin(List<OrderDetailsDTO> lstBins, Guid OrderGuid)
        {
            foreach (var item in lstBins)
            {
                OrderMasterDAL objOrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                OrderMasterDTO objOrderDTO = objOrderDAL.GetOrderByGuidPlain(OrderGuid);
                item.Bin = null;

                if (!string.IsNullOrEmpty(item.BinName))
                {
                    BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    item.Bin = objBinDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, SessionHelper.UserID, RoomID, CompanyID, objOrderDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty);
                }

                item.LastUpdatedBy = SessionHelper.UserID;
                OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
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
        public ActionResult LoadOrderLineItemsForReceive(string orderID, bool IsShowDeleted = false, bool IsArchivedOrder = false)
        {
            OrderDetailsDAL orderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);

            ViewBag.IsShowDeleted = IsShowDeleted;
            OrderMasterDTO objDTO = null;
            ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            objDTO = IsArchivedOrder ? new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetArchivedOrderByIdFull(long.Parse(orderID)) : new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdPlain(long.Parse(orderID));
            objDTO.IsArchived = IsArchivedOrder;

            var orderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            if (objDTO.OrderType.GetValueOrDefault(0) == 0)
                objDTO.OrderType = 1;

            if (IsShowDeleted)
                objDTO.OrderListItem = orderDetailsDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objDTO.GUID, RoomID, CompanyID, true, SessionHelper.UserSupplierIds);
            else
            {
                objDTO.OrderListItem =
                            IsArchivedOrder ? orderDetailsDAL.GetArchivedOrderDetailByOrderGUIDFullWithSupplierFilter(objDTO.GUID, RoomID, CompanyID, SessionHelper.UserSupplierIds)
                            : orderDetailsDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objDTO.GUID, RoomID, CompanyID, false, SessionHelper.UserSupplierIds);
            }

            if (objDTO.OrderListItem != null && objDTO.OrderListItem.Count > 0)
            {
                ReceiveOrderDetailsDAL receiveOrderDetailsDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in objDTO.OrderListItem)
                {
                    item.attachmentfileNames = receiveOrderDetailsDAL.getReceiveFileAttachement(item.GUID);
                }
            }
            List<PreReceivOrderDetailDTO> orderDetailTrackingList = orderDetailDAL.GetOrderDetailTrackingList(SessionHelper.RoomID, SessionHelper.CompanyID);
            objDTO.OrderListItem.ForEach(x =>
            {
                x.RequiredDateStr = x.RequiredDate != null ? x.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
                x.BinName = Convert.ToString(x.BinName) == "[|EmptyStagingBin|]" ? string.Empty : x.BinName;

                if (x.CostUOMValue == null || x.CostUOMValue <= 0)
                {
                    x.CostUOMValue = 1;
                }

                if (objDTO.OrderStatus < (int)OrderStatus.Closed && !x.IsCloseItem.GetValueOrDefault(false))
                {
                    x.OrderLineItemExtendedCost = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (x.RequestedQuantity.GetValueOrDefault(0) * (x.Cost.GetValueOrDefault(0) / x.CostUOMValue.GetValueOrDefault(1)))
                                                                     : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.Cost.GetValueOrDefault(0) / x.CostUOMValue.GetValueOrDefault(1))))));

                    x.OrderLineItemExtendedPrice = double.Parse(Convert.ToString((objDTO.OrderStatus <= 2 ? (x.RequestedQuantity.GetValueOrDefault(0) * (x.SellPrice.GetValueOrDefault(0) / x.CostUOMValue.GetValueOrDefault(1)))
                                                                     : (x.ApprovedQuantity.GetValueOrDefault(0) * (x.SellPrice.GetValueOrDefault(0) / x.CostUOMValue.GetValueOrDefault(1))))));
                }

                if (objDTO.OrderType == (int)OrderType.Order)
                {
                    ItemMasterDTO ItemDTO = ItemDAL.GetItemByGuidNormal(x.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                    if (ItemDTO != null && (ItemDTO.OrderUOMValue == null || ItemDTO.OrderUOMValue <= 0))
                        ItemDTO.OrderUOMValue = 1;

                    if (x.RequestedQuantityUOM != null && x.RequestedQuantityUOM > 0)
                        x.RequestedQuantity = x.RequestedQuantityUOM;
                    else if (x.RequestedQuantity != null && x.RequestedQuantity > 0 && ItemDTO != null && ItemDTO.IsAllowOrderCostuom)
                        x.RequestedQuantity = x.RequestedQuantity / ItemDTO.OrderUOMValue;

                    if (x.ApprovedQuantityUOM != null && x.ApprovedQuantityUOM > 0)
                        x.ApprovedQuantity = x.ApprovedQuantityUOM;
                    else if (x.ApprovedQuantity != null && x.ApprovedQuantity > 0 && ItemDTO != null && ItemDTO.IsAllowOrderCostuom)
                        x.ApprovedQuantity = x.ApprovedQuantity / ItemDTO.OrderUOMValue;

                    if (x.ReceivedQuantityUOM != null && x.ReceivedQuantityUOM > 0)
                        x.ReceivedQuantity = Math.Floor((double)x.ReceivedQuantityUOM);
                    else if (x.ReceivedQuantity != null && x.ReceivedQuantity > 0 && ItemDTO != null && ItemDTO.IsAllowOrderCostuom)
                        x.ReceivedQuantity = Math.Floor((double)x.ReceivedQuantity / (double)ItemDTO.OrderUOMValue);

                    if (x.InTransitQuantityUOM != null && x.InTransitQuantityUOM > 0)
                        x.InTransitQuantity = Math.Floor((double)x.InTransitQuantityUOM);
                    else if (x.InTransitQuantity != null && x.InTransitQuantity > 0 && ItemDTO != null && ItemDTO.IsAllowOrderCostuom)
                        x.InTransitQuantity = Math.Floor((double)x.InTransitQuantity / (double)ItemDTO.OrderUOMValue);
                }

                List<PreReceivOrderDetailDTO> orderDetailTrackingListByID = orderDetailTrackingList.Where(z => z.OrderDetailGUID == x.GUID).Select(q => q).ToList();
                if (orderDetailTrackingListByID != null && orderDetailTrackingListByID.Count == 1)
                {
                    objDTO.PackSlipNumber = orderDetailTrackingListByID[0].PackSlipNumber;
                    x.IsShowNormalItemPopUp = false;
                }
                else
                {
                    x.IsShowNormalItemPopUp = true;
                }
            });

            if (objDTO.MaterialStagingGUID != null)
            {
                MaterialStagingDTO objMSDTO = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetRecord(objDTO.MaterialStagingGUID.Value, RoomID, CompanyID);
                objDTO.StagingDefaultLocation = objMSDTO.BinID;
                objDTO.StagingName = objMSDTO.StagingName;
                objMSDTO = null;
            }

            if (objDTO.OrderStatus == Convert.ToInt32(OrderStatus.Closed))
            {
                return OrderEdit(objDTO.ID, IsArchivedOrder: IsArchivedOrder);
            }
            else
            {
                return PartialView("_ReceiveOrderLineItem", objDTO);
            }

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
            OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

            if (GetOrderType == OrderType.RuturnOrder)
            {
                return ReturnOrderedLineItems(NewReceiveDetail, OrderID, IsStaging);
            }

            OrderMasterDTO OrdDTO = objOrdDAL.GetOrderByIdPlain(OrderID);
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
                OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                OrderDetailsDTO OrdDetailDTO = null;
                List<ReceivedOrderTransferDetailDTO> lstReceiveDTO = new List<ReceivedOrderTransferDetailDTO>();
                ReceivedOrderTransferDetailDTO receiveDTO = null;
                List<MaterialStagingPullDetailDTO> lstMSLocationWiseDTO = null;
                MaterialStagingPullDetailDTO itemMSLocDTO = null;
                InventoryController invCtrlObj = new InventoryController();
                JsonResult jsResult = null;
                var enterpriseId = SessionHelper.EnterPriceID;

                foreach (var item in NewReceiveDetail)
                {
                    Index += 1;
                    OrdDetailDTO = objDAL.GetOrderDetailByIDFull(item.OrderDetailID, RoomID, CompanyID);
                    string errorMsg = ValidateItemForReceive(item, OrdDetailDTO, Index, OrdDTO);
                    keyValDTO = new KeyValDTO();
                    keyValDTO.key = OrdDetailDTO.GUID.ToString();

                    if (errorMsg == "ok")
                    {
                        if (IsStaging)
                        {
                            itemMSLocDTO = GetMaterialStagingPullDetail(item, OrdDetailDTO);

                            lstMSLocationWiseDTO = new List<MaterialStagingPullDetailDTO>();
                            lstMSLocationWiseDTO.Add(itemMSLocDTO);
                            jsResult = invCtrlObj.ItemLocationDetailsSaveForMSCredit(lstMSLocationWiseDTO);
                        }
                        else
                        {
                            receiveDTO = GetROTDDTO(item, OrdDetailDTO);
                            lstReceiveDTO.Add(receiveDTO);
                        }

                        keyValDTO.value = "Green";
                        responseMessage += "<b style='color: Green;'>" + string.Format(ResOrder.ItemReceivedSuccessfully, Index, OrdDetailDTO.ItemNumber) + "</br>";
                    }
                    else
                    {
                        responseMessage += errorMsg;
                        keyValDTO.value = "Olive";
                    }
                    lstKeyValDTO.Add(keyValDTO);
                }
                string OrdStatusText = string.Empty;
                long SessionUserId = SessionHelper.UserID;
                if (lstReceiveDTO != null && lstReceiveDTO.Count > 0)
                {
                    ReceivedOrderTransferDetailDAL objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                    foreach (var item in lstReceiveDTO)
                    {
                        item.OrderStatus = OrdDTO.OrderStatus;
                        objROTDDAL.InsertReceive(item, SessionUserId, item.IsReceivedCostChange.GetValueOrDefault(false), enterpriseId);
                    }
                    objDAL.UpdateOrderStatusByReceive(OrdDTO.GUID, RoomID, CompanyID, SessionHelper.UserID, true);
                    OrdDTO = objOrdDAL.GetOrderByIdPlain(OrderID);
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
        /// Receive Order Line Items 
        /// Currently this method not work for serial item.
        /// </summary>
        /// <param name="NewReceiveDetail"></param>
        /// <param name="OrderID"></param>
        /// <param name="IsStaging"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult ReturnOrderedLineItems(BasicDetailForNewReceive[] NewReceiveDetail, Int64 OrderID, bool IsStaging)
        {
            string responseMessage = string.Empty;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            KeyValDTO keyValDTO = null;
            string responseStatus = "false";

            if (NewReceiveDetail == null || NewReceiveDetail.Length < 0)
            {
                responseMessage = "<b style='color: Red;'>" + ResOrder.SelectOrderLineItemToReturn + "</b><br />";
            }
            else
            {
                int Index = 0;
                OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                OrderDetailsDTO OrdDetailDTO = null;
                CommonDAL objComonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                long SessionUserId = SessionHelper.UserID;
                foreach (var item in NewReceiveDetail)
                {
                    try
                    {
                        Index += 1;
                        keyValDTO = new KeyValDTO();
                        OrdDetailDTO = objDAL.GetOrderDetailByIDNormal(item.OrderDetailID, RoomID, CompanyID);
                        keyValDTO.key = OrdDetailDTO.GUID.ToString();

                        if (string.IsNullOrEmpty(item.BinName))
                        {
                            responseMessage += "<b style='color: Olive;'>" + string.Format(ResOrder.SelectBinToReturn, Index, OrdDetailDTO.ItemNumber) + "</br>";
                            keyValDTO.value = "Olive";
                            continue;
                        }

                        BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(OrdDetailDTO.ItemGUID ?? Guid.Empty, item.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                        Int64? BinId = objBinMasterDTO.ID;
                        string errorMsg = ValidateItemForReturn(item, OrdDetailDTO, Index, BinId.GetValueOrDefault(0));

                        if (errorMsg == "ok")
                        {
                            OrderReturnQuantityDetail objRetunDTO = new OrderReturnQuantityDetail()
                            {
                                ReturnQuantity = double.Parse(item.Quantity),
                                OrderDetailGUID = OrdDetailDTO.GUID,
                                ItemGUID = OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty),
                                LocationID = BinId.GetValueOrDefault(0),
                                LocationName = item.BinName,
                                IsStaging = IsStaging,
                            };

                            ResponseMessage RespMsg = null;

                            if (!IsStaging)
                            {
                                RespMsg = objDAL.OrderReturnQuantity(objRetunDTO, RoomID, CompanyID, SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, item.IsOnlyFromUI);
                            }
                            else
                            {
                                OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                                OrderMasterDTO objOrdDTO = objOrdDAL.GetOrderByGuidPlain(OrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));
                                RespMsg = objDAL.StagingOrderReturnQuantity(objRetunDTO, RoomID, CompanyID, SessionHelper.UserID, objOrdDTO.StagingID.GetValueOrDefault(0), SessionUserId, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                            }

                            if (RespMsg.IsSuccess)
                            {
                                keyValDTO.value = "Green";
                                responseMessage += "<b style='color: Green;'>" + string.Format(ResOrder.ReturnedSuccessfully, Index, OrdDetailDTO.ItemNumber) + "</br>";


                                //QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                                //objQBItemDAL.InsertQuickBookItem(objRetunDTO.ItemGUID, SessionHelper.EnterPriceID, CompanyID, RoomID, "Update", false, SessionUserId, "Web", null, "Return Order");

                            }
                            else
                            {
                                responseMessage += RespMsg.Message;
                                keyValDTO.value = "Olive";
                            }
                        }
                        else
                        {
                            responseMessage += errorMsg;
                            keyValDTO.value = "Olive";
                        }
                    }
                    catch (Exception ex)
                    {
                        responseMessage += ex.Message;
                        keyValDTO.value = "Red";
                    }
                    finally
                    {
                    }
                    lstKeyValDTO.Add(keyValDTO);
                }
            }

            if (lstKeyValDTO.Count > 0)
            {
                if (lstKeyValDTO.Where(x => x.value != "Green").Count() > 0)
                {
                    responseMessage = "<b>" + ResOrder.SomeItemsNotReturnedDueToReason + "</b><br /><br />" + responseMessage;
                    if (lstKeyValDTO.Where(x => x.value == "Green").Count() > 0)
                    {
                        responseStatus = "true";
                    }
                }
                else
                {
                    responseMessage = "<b>" + ResOrder.OrderReturnedSuccessfully + "</b><br /><br />" + responseMessage;
                    responseStatus = "true";
                }
            }

            OrderMasterDTO OrdDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByIdPlain(OrderID);
            return Json(new { Status = responseStatus, Message = responseMessage, RowColors = lstKeyValDTO.ToArray(), OrderStatus = OrdDTO.OrderStatus }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// ReceivedOrderDetail
        /// </summary>
        /// <returns></returns>
        public string ReceivedOrderDetail(string ItemGUID, string ordDetailGUID)
        {
            ViewBag.ItemGUID = ItemGUID;
            Guid? OrderDetailGUID = Guid.Parse(ordDetailGUID);
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, Guid.Parse(ItemGUID));
            ViewBag.ItemID = Objitem.ID;
            ReceivedOrderTransferDetailDAL objAPI = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetROTDByOrderDetailGUIDFull(OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
            OrderDetailsDTO objOrderDetailDTO = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailByGuidPlain(OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
            ViewBag.IsCloseItem = objOrderDetailDTO.IsCloseItem.GetValueOrDefault(false);
            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            ViewBag.ItemGUID_ItemType = ItemGUID + "#" + Objitem.ItemType;
            ViewBag.OrderDetailGUID = OrderDetailGUID.GetValueOrDefault(Guid.Empty);
            return RenderRazorViewToString("_ReceivedOrderDetail", objModel);
        }

        /// <summary>
        /// ReceivedOrderDetail
        /// </summary>
        /// <returns></returns>
        public string ReceivedDetailOfOrder(Guid OrderGUID)
        {
            ReceivedOrderTransferDetailDAL objAPI = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetROTDByOrderGUIDFull(OrderGUID, RoomID, CompanyID).ToList().OrderByDescending(x => x.ItemNumber).ToList();
            return RenderRazorViewToString("_ReceivedOrderDetail", objModel);
        }


        /// <summary>
        /// ReceivedItemLocationsListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ReceivedItemLocationsListAjax(JQueryDataTableParamModel param)
        {

            string ItemGUID = string.Empty;//Request["ItemGUID"].ToString();
            Guid? OrderDetailGUID = null;

            if (!string.IsNullOrEmpty(Request["OrderDetailGUID"]) && Request["OrderDetailGUID"].Trim().Length > 0)
            {
                OrderDetailGUID = Guid.Parse(Request["OrderDetailGUID"]);

                if (OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    OrderDetailsDTO objOrdDtlDTO = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailByGuidPlain(OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                    ItemGUID = objOrdDtlDTO.ItemGUID.GetValueOrDefault(Guid.Empty).ToString();
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

            ViewBag.ItemGUID = ItemGUID;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, Guid.Parse(ItemGUID));
            ViewBag.ItemID = Objitem.ID;

            if (Objitem == null || Objitem.IsItemLevelMinMaxQtyRequired == null)
            {
                ViewBag.IsItemLevelMinMaxQtyRequired = false;
            }
            else
            {
                ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            }

            ReceivedOrderTransferDetailDAL objAPI = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            List<ReceivedOrderTransferDetailDTO> lstRecOrdTrnsDtl = objAPI.GetPagedReceivedOrderTransferDetailRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, Guid.Parse(ItemGUID), OrderDetailGUID).ToList();
            ItemMasterDTO objItemDTO = objItemAPI.GetItemByGuidNormal(Guid.Parse(ItemGUID), RoomID, CompanyID);

            if (objItemDTO != null && objItemDTO.SerialNumberTracking == false)
            {
                foreach (ReceivedOrderTransferDetailDTO item in lstRecOrdTrnsDtl)
                {
                    if (objItemDTO.OrderUOMValue == null || objItemDTO.OrderUOMValue <= 0)
                        objItemDTO.OrderUOMValue = 1;
                    if (item.IsAllowOrderCostuom && item.OrderType == (int)OrderType.Order)
                    {
                        if (item.CustomerOwnedQuantity != null && item.CustomerOwnedQuantity >= 0)
                            item.CustomerOwnedQuantity = item.CustomerOwnedQuantity / objItemDTO.OrderUOMValue;

                        if (item.ConsignedQuantity != null && item.ConsignedQuantity >= 0)
                            item.ConsignedQuantity = item.ConsignedQuantity / objItemDTO.OrderUOMValue;
                    }
                }
            }
            var objModel = lstRecOrdTrnsDtl;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
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
        public string DeleteRecieveAndUpdateReceivedQty(string ItemGUID, string ordDetailGUID, string deleteIDs)
        {
            ReceivedOrderTransferDetailDAL objRecdOrdTrnDtlDAL = null;
            ReceivedOrderTransferDetailDTO objRecdOrdTrnDtlDTO = null;
            ItemLocationDetailsDTO objItemLocationDetail = null;
            MaterialStagingPullDetailDTO objMSDetailDTO = null;
            OrderDetailsDAL ordDetailCtrl = null;
            ItemMasterDAL ItemDAL = null;
            MaterialStagingPullDetailDAL mspdDAL = null;
            ItemLocationDetailsDAL ilDAL = null;
            IEnumerable<ReceivedOrderTransferDetailDTO> objModel = null;
            OrderDetailsDTO objOrdDetailDTO = null;

            string returString = "fail";
            try
            {
                string[] rotdGUID = deleteIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                long SessionUserId = SessionHelper.UserID;
                if (rotdGUID != null && rotdGUID.Length > 0)
                {
                    ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    mspdDAL = new eTurns.DAL.MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
                    ilDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    objRecdOrdTrnDtlDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                    foreach (string item in rotdGUID)
                    {
                        objRecdOrdTrnDtlDTO = objRecdOrdTrnDtlDAL.GetROTDByGuidPlain(Guid.Parse(item), RoomID, CompanyID);
                        objItemLocationDetail = ilDAL.GetRecord(objRecdOrdTrnDtlDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                        bool isChanged = false;
                        if (objItemLocationDetail != null)
                        {
                            if (objItemLocationDetail.CustomerOwnedQuantity.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.CustomerOwnedQuantity.GetValueOrDefault(0))
                                isChanged = true;
                            else if (objItemLocationDetail.ConsignedQuantity.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.ConsignedQuantity.GetValueOrDefault(0))
                                isChanged = true;
                            else if (objItemLocationDetail.Cost.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.Cost.GetValueOrDefault(0))
                                isChanged = true;
                            else if (objItemLocationDetail.DateCodeTracking && objItemLocationDetail.ExpirationDate.GetValueOrDefault(DateTime.MinValue) != objRecdOrdTrnDtlDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue))
                                isChanged = true;
                            else if (objItemLocationDetail.ReceivedDate != objRecdOrdTrnDtlDTO.ReceivedDate)
                                isChanged = true;
                            else if (objItemLocationDetail.SerialNumberTracking && objItemLocationDetail.SerialNumber != objRecdOrdTrnDtlDTO.SerialNumber)
                                isChanged = true;
                            else if (objItemLocationDetail.LotNumberTracking && objItemLocationDetail.LotNumber != objRecdOrdTrnDtlDTO.LotNumber)
                                isChanged = true;
                        }
                        else
                        {
                            objMSDetailDTO = mspdDAL.GetRecord(objRecdOrdTrnDtlDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                            if (objMSDetailDTO == null)
                                isChanged = true;
                            else if (objMSDetailDTO.CustomerOwnedQuantity.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.CustomerOwnedQuantity.GetValueOrDefault(0))
                                isChanged = true;
                            else if (objMSDetailDTO.ConsignedQuantity.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.ConsignedQuantity.GetValueOrDefault(0))
                                isChanged = true;
                            else if (objMSDetailDTO.ItemCost.GetValueOrDefault(0) != objRecdOrdTrnDtlDTO.Cost.GetValueOrDefault(0))
                                isChanged = true;
                            else if (objMSDetailDTO.SerialNumberTracking && objItemLocationDetail.SerialNumber != objRecdOrdTrnDtlDTO.SerialNumber)
                                isChanged = true;
                            else if (objMSDetailDTO.LotNumberTracking && objItemLocationDetail.LotNumber != objRecdOrdTrnDtlDTO.LotNumber)
                                isChanged = true;

                        }

                        if (!isChanged)
                        {
                            objRecdOrdTrnDtlDAL.DeleteRecords(item, SessionHelper.UserID, RoomID, CompanyID, SessionUserId);
                            objModel = objRecdOrdTrnDtlDAL.GetROTDByOrderDetailGUIDPlain(Guid.Parse(ordDetailGUID), RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
                            var aax = objModel.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));

                            objOrdDetailDTO = ordDetailCtrl.GetOrderDetailByGuidPlain(Guid.Parse(ordDetailGUID), RoomID, CompanyID);
                            objOrdDetailDTO.ReceivedQuantity = aax;
                            ItemMasterDTO ImDTO = ItemDAL.GetItemByGuidNormal(objOrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            if (ImDTO.OrderUOMValue == null || ImDTO.OrderUOMValue <= 0)
                                ImDTO.OrderUOMValue = 1;
                            if (ImDTO.IsAllowOrderCostuom)
                                objOrdDetailDTO.ReceivedQuantityUOM = objOrdDetailDTO.ReceivedQuantity / ImDTO.OrderUOMValue;
                            else
                                objOrdDetailDTO.ReceivedQuantityUOM = objOrdDetailDTO.ReceivedQuantity;
                            ordDetailCtrl.Edit(objOrdDetailDTO, SessionUserId, EnterpriseId);
                            ordDetailCtrl.UpdateOrderStatusByReceive(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), objOrdDetailDTO.Room.GetValueOrDefault(0), objOrdDetailDTO.CompanyID.GetValueOrDefault(0), objOrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0));
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
                objItemLocationDetail = null;
                objMSDetailDTO = null;
                ordDetailCtrl = null;
                ItemDAL = null;
                mspdDAL = null;
                ilDAL = null;
                objModel = null;
                objOrdDetailDTO = null;
            }
        }

        /// <summary>
        /// Delete Received Or Returned Entry
        /// </summary>
        /// <param name="OrderDetailGuid"></param>
        /// <param name="GUIDsToDelete"></param>
        /// <returns></returns>
        public JsonResult DeleteReceivedOrReturnedEntry(Guid OrderDetailGuid, Guid[] GUIDsToDelete)
        {
            OrderDetailsDAL OrdDetailDAL = null;
            OrderDetailsDTO OrdDetailDTO = null;
            OrderMasterDAL OrderDAL = null;

            ReceivedOrderTransferDetailDAL RcvedOrdDtlDAL = null;

            try
            {
                long SessionUserId = SessionHelper.UserID;
                RcvedOrdDtlDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                OrdDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                OrderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

                OrdDetailDTO = OrdDetailDAL.GetOrderDetailByGuidNormal(OrderDetailGuid, RoomID, CompanyID);

                bool isStagingOrder = OrdDetailDTO.StagingID.GetValueOrDefault(0) > 0;

                if (OrdDetailDTO.OrderType == (int)OrderType.Order)
                {
                    RcvedOrdDtlDAL.DeleteRecievedRecords(GUIDsToDelete, isStagingOrder, SessionHelper.UserID, RoomID, CompanyID, SessionUserId);
                }
                else
                {
                    RcvedOrdDtlDAL.DeleteReturnedRecords(GUIDsToDelete, isStagingOrder, SessionHelper.UserID, RoomID, CompanyID, SessionUserId, EnterpriseId);
                }

                IEnumerable<ReceivedOrderTransferDetailDTO> objModel = RcvedOrdDtlDAL.GetROTDByOrderDetailGUIDPlain(OrderDetailGuid, RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
                var aax = objModel.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                OrdDetailDTO.ReceivedQuantity = aax;
                OrdDetailDAL.Edit(OrdDetailDTO, SessionUserId, EnterpriseId);
                OrdDetailDAL.UpdateOrderStatusByReceive(OrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), OrdDetailDTO.Room.GetValueOrDefault(0), OrdDetailDTO.CompanyID.GetValueOrDefault(0), OrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0));

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
                RcvedOrdDtlDAL = null;
            }
        }

        /// <summary>
        /// GetReceivedQuantity Call From Receive Module
        /// </summary>
        /// <param name="OrderDetailGUID"></param>
        /// <returns></returns>
        public JsonResult GetReceivedQuantity(string OrderDetailGUID)
        {
            OrderDetailsDAL odctrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            OrderDetailsDTO objDTO = odctrl.GetOrderDetailByGuidPlain(Guid.Parse(OrderDetailGUID), RoomID, CompanyID);
            return Json(new { Status = "ok", Message = "Success", ReturnDTO = objDTO }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenPopupToReturnItem(List<ItemToReturnDTO> returnInfo)
        {
            // RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            // RoomDTO roomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,InventoryConsuptionMethod";
            RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            foreach (var item in returnInfo)
            {
                item.InventoryConsuptionMethod = roomDTO.InventoryConsuptionMethod;
            }
            return PartialView("_ItemToReturn", returnInfo);
        }

        [HttpPost]
        public JsonResult ReturnSerialsAndLots(List<ItemToReturnDTO> objItemPullInfo)
        {
            List<ItemToReturnDTO> oReturn = new List<ItemToReturnDTO>();
            OrderDetailsDAL objPullMasterDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemPullInfo> oReturnError = new List<ItemPullInfo>();
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            foreach (ItemToReturnDTO item in objItemPullInfo)
            {
                if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                {
                    item.lstItemPullDetails = item.lstItemPullDetails.Where(x => x.PullQuantity > 0).ToList();

                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                    {
                        ItemToReturnDTO oItemPullInfo = item;
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        oItemPullInfo.CanOverrideProjectLimits = true;
                        oItemPullInfo.ValidateProjectSpendLimits = false;
                        oItemPullInfo.ErrorList = new List<PullErrorInfo>();
                        oItemPullInfo = ValidateLotAndSerial(oItemPullInfo);

                        if (oItemPullInfo.ErrorList.Count == 0)
                        {
                            oItemPullInfo.EnterpriseId = enterpriseId;
                            oItemPullInfo = objPullMasterDAL.ReturntemQuantity(oItemPullInfo, SessionUserId, enterpriseId);

                            //QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                            //objQBItemDAL.InsertQuickBookItem(oItemPullInfo.ItemGUID, SessionHelper.EnterPriceID, CompanyID, RoomID, "Update", false, SessionUserId, "Web", null, "Return Order");
                        }

                        oReturn.Add(oItemPullInfo);
                    }
                }
            }

            return Json(oReturn);
        }

        private ItemToReturnDTO ValidateLotAndSerial(ItemToReturnDTO objItemPullInfo)
        {
            ItemMasterDTO objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);
            double TotalPulled = 0, Diff = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            double AvailQty = 0;
            ItemLocationQTYDTO oLocQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);

            if (oLocQty != null)
            {
                AvailQty = (oLocQty.CustomerOwnedQuantity ?? 0) + (oLocQty.ConsignedQuantity ?? 0);
            }

            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstAvailableQty = new List<ItemLocationLotSerialDTO>();

            if (AvailQty >= objItemPullInfo.QtyToReturn)
            {
                if (!objItem.LotNumberTracking && !objItem.SerialNumberTracking)
                {
                    lstAvailableQty = objPullMasterDAL.GetItemLocationsLotSerials(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID, objItemPullInfo.QtyToReturn, true);
                    lstAvailableQty.ForEach(il =>
                    {
                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (il.Cost ?? 0));
                        Diff = (objItemPullInfo.QtyToReturn - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0);
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.QtyToReturn - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.QtyToReturn - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (ConsignedTaken + CustownedTaken) * (il.Cost.GetValueOrDefault(0));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));
                    });

                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                }
                else
                {
                    if (objItem.LotNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, t.LotNumber, string.Empty, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { LotNumber = il.LotNumber.Trim() } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber.Trim(),
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {
                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (il.Cost ?? 0));
                                Diff = (objItemPullInfo.QtyToReturn - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0);
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.QtyToReturn - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.QtyToReturn - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (il.Cost ?? 0));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                            });

                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                        }
                    }

                    if (objItem.SerialNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { SerialNumber = il.SerialNumber.Trim() } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber.Trim(),
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (il.Cost ?? 0));
                                Diff = (objItemPullInfo.QtyToReturn - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.QtyToReturn - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.QtyToReturn - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (il.Cost ?? 0));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                            });

                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                        }
                    }
                }
            }
            else
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "1", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgQuantityNotAvailable });
            }
            return objItemPullInfo;
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
        private string ValidateItemForReceive(BasicDetailForNewReceive item, OrderDetailsDTO OrdDetailDTO, int ErrorSrNo, OrderMasterDTO OrderMasterDTO)
        {
            string returnString = "";

            if (OrdDetailDTO.SerialNumberTracking)
            {
                returnString += "<b style='color: Olive'>" + string.Format(ResOrder.ItemSerialClickInlineReceiveButton, ErrorSrNo, OrdDetailDTO.ItemNumber) + " </b> <br />";
                return returnString;
            }

            if (OrdDetailDTO.IsCloseItem.GetValueOrDefault(false))
            {
                returnString += "<b style='color: Olive'>" + ErrorSrNo + ")" + string.Format(ResOrder.ClosedItemCantReceiveClickInlineUncloseButton, ErrorSrNo, OrdDetailDTO.ItemNumber) + " </b> <br />";
                return returnString;
            }

            bool IsLotErrorMsg = false;
            bool IsExprieErrorMsg = false;
            bool ISQtyErrorMsg = false;
            bool IsBinErrorMsg = false;
            bool IsUDFRequired = false;
            bool IsPackSlipRequiredErrorMsg = false;
            ItemMasterDTO objItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, OrdDetailDTO.ItemGUID);

            if (objItemDTO != null && objItemDTO.IsPackslipMandatoryAtReceive && (string.IsNullOrEmpty(item.PackSlipNumber) || string.IsNullOrWhiteSpace(item.PackSlipNumber)))
                IsPackSlipRequiredErrorMsg = true;

            if (OrdDetailDTO.LotNumberTracking && string.IsNullOrEmpty(item.LotNumber))
                IsLotErrorMsg = true;

            if (OrdDetailDTO.DateCodeTracking)
            {
                DateTime dtExp = DateTime.MinValue;
                DateTime.TryParse(Convert.ToString(DateTime.ParseExact(item.ExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)), out dtExp);
                if (dtExp <= DateTime.MinValue)
                    IsExprieErrorMsg = true;
            }

            double qty = 0;
            double.TryParse(item.Quantity, out qty);

            if (qty <= 0)
                ISQtyErrorMsg = true;

            if (string.IsNullOrEmpty(item.BinName) && OrderMasterDTO.StagingID == null)
                IsBinErrorMsg = true;

            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedOrderTransferDetail", RoomID, CompanyID);
            string udfRequier = string.Empty;

            foreach (var i in DataFromDB)
            {
                if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(item.UDF1))
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
                else if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(item.UDF2))
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
                else if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(item.UDF3))
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
                else if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(item.UDF4))
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
                else if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(item.UDF5))
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
                returnString = "<b style='color: Olive;'>" + ErrorSrNo + ". " + OrdDetailDTO.ItemNumber + ":";
                List<string> arrErrors = new List<string>();
                if (IsLotErrorMsg)
                    arrErrors.Add(ResItemLocationDetails.LotNumber);

                if (IsExprieErrorMsg)
                    arrErrors.Add(ResItemLocationDetails.ExpirationDate);

                if (ISQtyErrorMsg)
                    arrErrors.Add(ResReceiveToolAssetOrderDetails.ReceiveQuantity);

                if (IsBinErrorMsg)
                    arrErrors.Add(ResOrder.ReceiveBin);

                if (IsUDFRequired)
                    arrErrors.Add(udfRequier);

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
            // ItemMasterDTO objItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, OrdDetailDTO.ItemGUID);

            if (OrdDetailDTO.IsPackslipMandatoryAtReceive && (string.IsNullOrEmpty(item.PackSlipNumber) || string.IsNullOrWhiteSpace(item.PackSlipNumber)))
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
        private ItemLocationDetailsDTO GetItemLocationDetail(BasicDetailForNewReceive item, OrderDetailsDTO OrdDetailDTO)
        {
            ItemLocationDetailsDTO itemLocDTO = new ItemLocationDetailsDTO();
            BinMasterDAL ItemLocationDetailsDAL = new eTurns.DAL.BinMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDTO objBinMasterDTO = ItemLocationDetailsDAL.GetItemBinPlain(OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, RoomID, CompanyID, SessionHelper.UserID, false);
            itemLocDTO.BinID = objBinMasterDTO.ID;
            itemLocDTO.BinNumber = objBinMasterDTO.BinNumber;
            double dcost = 0;
            double.TryParse(item.Cost, out dcost);
            itemLocDTO.Cost = dcost;
            itemLocDTO.OrderDetailGUID = OrdDetailDTO.GUID;
            itemLocDTO.Received = item.ReceiveDate;
            itemLocDTO.ReceivedDate = item.ReceiveDate != null ? Convert.ToDateTime(DateTime.ParseExact(item.ReceiveDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)) : Convert.ToDateTime(item.ReceiveDate);//DateTime.Parse(item.ReceiveDate);

            if (OrdDetailDTO.Consignment)
                itemLocDTO.ConsignedQuantity = double.Parse(item.Quantity);
            else
                itemLocDTO.CustomerOwnedQuantity = double.Parse(item.Quantity);

            if (OrdDetailDTO.LotNumberTracking)
            {
                itemLocDTO.LotNumber = item.LotNumber;
                itemLocDTO.LotNumberTracking = OrdDetailDTO.LotNumberTracking;
            }
            if (OrdDetailDTO.DateCodeTracking)
            {
                itemLocDTO.DateCodeTracking = OrdDetailDTO.DateCodeTracking;
                itemLocDTO.Expiration = item.ExpirationDate;
                itemLocDTO.ExpirationDate = item.ExpirationDate != null ? DateTime.ParseExact(item.ExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult) : Convert.ToDateTime(item.ExpirationDate);//DateTime.Parse(item.ExpirationDate);
            }

            itemLocDTO.ItemGUID = OrdDetailDTO.ItemGUID;
            itemLocDTO.ItemType = OrdDetailDTO.ItemType;

            if (OrdDetailDTO.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
            {
                if (objBinMasterDTO != null)
                {
                    itemLocDTO.CriticalQuantity = objBinMasterDTO.CriticalQuantity;
                    itemLocDTO.MinimumQuantity = objBinMasterDTO.MinimumQuantity;
                    itemLocDTO.MaximumQuantity = objBinMasterDTO.MaximumQuantity;
                }
            }

            itemLocDTO.CreatedBy = SessionHelper.UserID;
            itemLocDTO.LastUpdatedBy = SessionHelper.UserID;
            itemLocDTO.Room = RoomID;
            itemLocDTO.CompanyID = CompanyID;
            itemLocDTO.PackSlipNumber = item.PackSlipNumber;
            itemLocDTO.UDF1 = item.UDF1;
            itemLocDTO.UDF2 = item.UDF2;
            itemLocDTO.UDF3 = item.UDF3;
            itemLocDTO.UDF4 = item.UDF4;
            itemLocDTO.UDF5 = item.UDF5;

            return itemLocDTO;
        }

        /// <summary>
        /// GetItemLocationDetail DTO
        /// </summary>
        /// <param name="OrdDetailDTO"></param>
        /// <returns></returns>
        private ReceivedOrderTransferDetailDTO GetROTDDTO(BasicDetailForNewReceive item, OrderDetailsDTO OrdDetailDTO)
        {
            ReceivedOrderTransferDetailDTO pbjROTDDTO = new ReceivedOrderTransferDetailDTO();
            BinMasterDAL ItemLocationDetailsDAL = new eTurns.DAL.BinMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDTO objBinMasterDTO = ItemLocationDetailsDAL.GetItemBinPlain(OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinName, RoomID, CompanyID, SessionHelper.UserID, false);
            pbjROTDDTO.BinID = objBinMasterDTO.ID;
            pbjROTDDTO.BinNumber = objBinMasterDTO.BinNumber;
            double dcost = 0;
            double.TryParse(item.Cost, out dcost);
            if (OrdDetailDTO != null
                && dcost != OrdDetailDTO.ItemCost.GetValueOrDefault(0))
            {
                pbjROTDDTO.IsReceivedCostChange = true;
            }
            else
            {
                pbjROTDDTO.IsReceivedCostChange = false;
            }

            DateTime currentDateAsPerRoom = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
            DateTime newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, item.ReceiveDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, SessionHelper.CurrentTimeZone, SessionHelper.RoomTimeFormat);
            pbjROTDDTO.Cost = dcost;
            pbjROTDDTO.OrderDetailGUID = OrdDetailDTO.GUID;
            pbjROTDDTO.Received = newReceiveTempDate.ToString(SessionHelper.RoomDateFormat); //item.ReceiveDate;
            pbjROTDDTO.ReceivedDate = newReceiveTempDate;//item.ReceiveDate != null ? Convert.ToDateTime(DateTime.ParseExact(item.ReceiveDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)) : Convert.ToDateTime(item.ReceiveDate);//DateTime.Parse(item.ReceiveDate);

            if (OrdDetailDTO.Consignment)
                pbjROTDDTO.ConsignedQuantity = double.Parse(item.Quantity);
            else
                pbjROTDDTO.CustomerOwnedQuantity = double.Parse(item.Quantity);

            if (OrdDetailDTO.LotNumberTracking)
            {
                pbjROTDDTO.LotNumber = item.LotNumber;
                pbjROTDDTO.LotNumberTracking = OrdDetailDTO.LotNumberTracking;
            }

            if (OrdDetailDTO.DateCodeTracking)
            {
                pbjROTDDTO.DateCodeTracking = OrdDetailDTO.DateCodeTracking;
                pbjROTDDTO.Expiration = item.ExpirationDate;
                pbjROTDDTO.ExpirationDate = item.ExpirationDate != null ? DateTime.ParseExact(item.ExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult) : Convert.ToDateTime(item.ExpirationDate);//DateTime.Parse(item.ExpirationDate);
            }

            pbjROTDDTO.ItemGUID = OrdDetailDTO.ItemGUID;
            pbjROTDDTO.ItemType = OrdDetailDTO.ItemType;

            if (OrdDetailDTO.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
            {
                if (objBinMasterDTO != null)
                {
                    pbjROTDDTO.CriticalQuantity = objBinMasterDTO.CriticalQuantity;
                    pbjROTDDTO.MinimumQuantity = objBinMasterDTO.MinimumQuantity;
                    pbjROTDDTO.MaximumQuantity = objBinMasterDTO.MaximumQuantity;
                }
            }

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
        private MaterialStagingPullDetailDTO GetMaterialStagingPullDetail(BasicDetailForNewReceive item, OrderDetailsDTO OrdDetailDTO)
        {
            MaterialStagingPullDetailDTO itemLocDTO = new MaterialStagingPullDetailDTO();
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBin(OrdDetailDTO.ItemGUID ?? Guid.Empty, item.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);
            itemLocDTO.BinID = objBinMasterDTO.ID;
            DateTime currentDateAsPerRoom = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
            DateTime newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, item.ReceiveDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, SessionHelper.CurrentTimeZone, SessionHelper.RoomTimeFormat);
            double dcost = 0;
            double.TryParse(item.Cost, out dcost);
            itemLocDTO.ItemCost = dcost;
            itemLocDTO.OrderDetailGUID = OrdDetailDTO.GUID;
            itemLocDTO.Received = newReceiveTempDate.ToString(SessionHelper.RoomDateFormat); //item.ReceiveDate;
            itemLocDTO.ReceivedDate = newReceiveTempDate;

            if (OrdDetailDTO.Consignment)
                itemLocDTO.ConsignedQuantity = double.Parse(item.Quantity);
            else
                itemLocDTO.CustomerOwnedQuantity = double.Parse(item.Quantity);

            if (OrdDetailDTO.LotNumberTracking)
            {
                itemLocDTO.LotNumber = item.LotNumber;
                itemLocDTO.LotNumberTracking = OrdDetailDTO.LotNumberTracking;
            }

            if (OrdDetailDTO.DateCodeTracking)
            {
                itemLocDTO.DateCodeTracking = OrdDetailDTO.DateCodeTracking;
                itemLocDTO.Expiration = item.ExpirationDate;
            }

            itemLocDTO.ItemGUID = OrdDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty);
            itemLocDTO.CreatedBy = SessionHelper.UserID;
            itemLocDTO.LastUpdatedBy = SessionHelper.UserID;
            itemLocDTO.Room = RoomID;
            itemLocDTO.CompanyID = CompanyID;
            itemLocDTO.PackSlipNumber = item.PackSlipNumber;
            itemLocDTO.UDF1 = item.UDF1;
            itemLocDTO.UDF2 = item.UDF2;
            itemLocDTO.UDF3 = item.UDF3;
            itemLocDTO.UDF4 = item.UDF4;
            itemLocDTO.UDF5 = item.UDF5;

            return itemLocDTO;
        }


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
        private List<SelectListItem> GetOrderStatusList(OrderMasterDTO objDTO, string Mode)
        {
            int CurrentStatus = objDTO.OrderStatus;
            List<SelectListItem> returnList = new List<SelectListItem>();

            if (Mode.ToLower() == "create")
            {
                returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.UnSubmitted.ToString()), Value = ((int)OrderStatus.UnSubmitted).ToString() });
            }
            else if (Mode.ToLower() == "edit")
            {
                foreach (var item in Enum.GetValues(typeof(OrderStatus)))
                {
                    string itemText = item.ToString();
                    int itemValue = (int)(Enum.Parse(typeof(OrderStatus), itemText));
                    if (itemValue < (int)OrderStatus.Transmitted && itemValue >= CurrentStatus)
                    {
                        if (returnList.FindIndex(x => int.Parse(x.Value) == itemValue) < 0)
                            returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(item.ToString()), Value = itemValue.ToString() });
                    }
                    else if (CurrentStatus == (int)OrderStatus.Transmitted && IsChangeOrder && !objDTO.OrderIsInReceive)
                    {
                        if (returnList.FindIndex(x => int.Parse(x.Value) == (int)OrderStatus.UnSubmitted) < 0)
                        {
                            returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.UnSubmitted.ToString()), Value = ((int)OrderStatus.UnSubmitted).ToString() });
                            string strOrderNumber = objDTO.OrderNumber;
                            string[] ordRevNumber = strOrderNumber.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                            if (ordRevNumber.Length == 1)
                            {
                                strOrderNumber = strOrderNumber + "_R1";

                            }
                            else if (ordRevNumber.Length > 1)
                            {
                                int RevNo = int.Parse(ordRevNumber[ordRevNumber.Length - 1].Replace("R", ""));
                                if (ordRevNumber.Length > 2)
                                {
                                    for (int i = 0; i < ordRevNumber.Length - 2; i++)
                                    {
                                        if (i > 0)
                                            strOrderNumber += "_";
                                        strOrderNumber += ordRevNumber[i];
                                    }
                                    strOrderNumber += "_R" + (RevNo + 1);
                                }
                                else
                                {
                                    strOrderNumber = ordRevNumber[0] + "_R" + (RevNo + 1);
                                }
                            }
                            objDTO.OrderNumber = strOrderNumber;
                            break;
                        }
                    }
                }

                if (objDTO.OrderStatus != (int)OrderStatus.Approved && returnList.FindIndex(x => x.Value == ((int)OrderStatus.Approved).ToString()) >= 0 && !IsApprove)
                {
                    returnList.RemoveAt(returnList.FindIndex(x => x.Value == ((int)OrderStatus.Approved).ToString()));
                }

                if (objDTO.OrderStatus != (int)OrderStatus.Submitted && returnList.FindIndex(x => x.Value == ((int)OrderStatus.Submitted).ToString()) >= 0 && !IsSubmit)
                {
                    returnList.RemoveAt(returnList.FindIndex(x => x.Value == ((int)OrderStatus.Submitted).ToString()));
                }

                if (returnList.FindIndex(x => x.Value == ((int)OrderStatus.Approved).ToString()) >= 0)
                {
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Closed.ToString()), Value = ((int)OrderStatus.Closed).ToString() });
                }
            }
            else
            {
                foreach (var item in Enum.GetValues(typeof(OrderStatus)))
                {
                    string itemText = item.ToString();
                    int itemValue = (int)(Enum.Parse(typeof(OrderStatus), itemText));
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(item.ToString()), Value = itemValue.ToString() });
                }
            }

            if (Mode.ToLower() != "create" && !(returnList.FindIndex(x => x.Value == ((int)OrderStatus.Closed).ToString()) >= 0))
            {
                returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Closed.ToString()), Value = ((int)OrderStatus.Closed).ToString() });
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
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                             UDFMaxLength = c.UDFMaxLength
                         };

            return result;
        }

        /// <summary>
        /// GetMailBody
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetMailBody(OrderMasterDTO obj)
        {
            string mailBody = "";
            int PriseSelectionOption = 1;
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            RoomModuleSettingsDTO objRoomModuleSettingsDTO = new RoomModuleSettingsDTO();
            objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(CompanyID, RoomID, (long)SessionHelper.ModuleList.Orders);

            if (objRoomModuleSettingsDTO != null && objRoomModuleSettingsDTO.ID > 0)
            {
                PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption.GetValueOrDefault(0);
            }

            string OrderCostTitle = ResOrder.OrderCost;
            OrderCostTitle = (PriseSelectionOption == 1 ? ResOrder.OrderPrice : ResOrder.OrderCost);
            string udfRequier = string.Empty;
            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
            List<UDFDTO> DataFromDB = objUDFDAL.GetNonDeletedUDFsByUDFTableNamePlain("OrderMaster", RoomID, CompanyID);
            string suppliername = "";
            string OrdNumber = ResOrder.OrderNumber;
            string ReqDateCap = ResOrder.RequiredDate;
            string OrdStatus = ResOrder.OrderStatus;
            string OrdReqQty = ResOrder.RequestedQuantity;

            if (obj.OrderType == (int)OrderType.RuturnOrder)
            {
                OrdNumber = ResOrder.ReturnOrderNumber;
                ReqDateCap = ResOrder.ReturnDate;
                OrdStatus = ResOrder.ReturnOrderStatus;
                OrdReqQty = ResOrder.ReturnQuantity;
            }

            if (obj.Supplier != null && obj.Supplier > 0)
            {
                SupplierMasterDTO objSupplierDTO = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(obj.Supplier)));
                if (objSupplierDTO != null && objSupplierDTO.ID > 0)
                    suppliername = objSupplierDTO.SupplierName;
            }
            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + OrdNumber + @": </label>
                                    <label style=""font-weight: bold;"">
                                        " + obj.OrderNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResOrder.Comment + @": </label>
                                    <label>
                                        " + obj.Comment + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResOrder.ShippingMethod + @": </label>
                                    <label>
                                        " + obj.ShipViaName + @"</label>
                                </td>
                            </tr>";

            mailBody = mailBody + @"
                            <tr>
                                <td>
                                    <label>
                                        " + OrderCostTitle + @": </label>
                                    <label>
                                        " + (Convert.ToString(PriseSelectionOption) == "1" ? obj.OrderPrice.ToString("N" + SessionHelper.CurrencyDecimalDigits) : obj.OrderCost.GetValueOrDefault(0).ToString("N" + SessionHelper.CurrencyDecimalDigits)) + @"</label>
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
                                        " + ResOrder.Supplier + @": </label>
                                    <label>
                                        " +
                                          suppliername
                                          + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(OrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
                                </td>
                            </tr>";


            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                for (int i = 0; i < DataFromDB.Count; i++)
                {
                    if (DataFromDB[i].UDFColumnName == "UDF" + (i + 1))
                    {
                        string val = string.Empty;
                        val = ResourceUtils.GetResource("ResOrder", DataFromDB[i].UDFColumnName, true);

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
                                        " + ResItemMaster.ItemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResOrder.Bin + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.Description + @"
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
                OrderDetailsDAL objOrdDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.OrderListItem = objOrdDetailDAL.GetOrderDetailByOrderGUIDFull(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
            }

            if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {

                foreach (var item in obj.OrderListItem)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.Bin != null && item.Bin > 0)
                        binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID).BinNumber;

                    int OrderUOM = 1;
                    if (item.OrderUOMValue != null && item.OrderUOMValue >= 1)
                        OrderUOM = Convert.ToInt32(item.OrderUOMValue);

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();


                    if (obj.OrderStatus == (int)OrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(ReqQty) && ReqQty != "&nbsp;" && Convert.ToInt32(ReqQty) >= 0) //  && item.SerialNumberTracking == false
                    {
                        int intReqQty = Convert.ToInt32(ReqQty) / OrderUOM;
                        if (intReqQty <= 0)
                            intReqQty = 0;

                        ReqQty = intReqQty.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);


                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                        <td>
                            " + binname + @"
                        </td>
                        <td>
                            " + item.ItemDescription + @"
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
        private string GetMailBodySupplier(OrderMasterDTO obj, string DateTimeFormat = "MM/dd/yyyy")
        {
            string mailBody = "";
            string suppliername = "";
            string SupplierPartNo = string.Empty;
            string OrdNumber = ResOrder.OrderNumber;
            string ReqDateCap = ResOrder.RequiredDate;
            string OrdStatus = ResOrder.OrderStatus;
            string OrdReqQty = ResOrder.RequestedQuantity;
            SupplierMasterDTO objSupplierMasterDTP = null;

            string strReqDate = obj.RequiredDate.ToString(DateTimeFormat);//obj.RequiredDate.ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);


            if (obj.OrderType == (int)OrderType.RuturnOrder)
            {
                OrdNumber = ResOrder.ReturnOrderNumber;
                ReqDateCap = ResOrder.ReturnDate;
                OrdStatus = ResOrder.ReturnOrderStatus;
                OrdReqQty = ResOrder.ReturnQuantity;
            }

            if (obj.Supplier != null && obj.Supplier > 0)
            {
                objSupplierMasterDTP = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(obj.Supplier)));
                if (objSupplierMasterDTP != null && objSupplierMasterDTP.ID > 0)
                    suppliername = objSupplierMasterDTP.SupplierName;
            }

            bool IsignoreUOMFields = false;
            try
            {
                string[] lstrestrictroom = SiteSettingHelper.MailOrderFieldRoomRestriction.Split(',');
                string CombinationIdToCheck = SessionHelper.EnterPriceID + "_" + obj.CompanyID.ToString() + "_" + obj.Room.ToString();
                foreach (var item in lstrestrictroom)
                {
                    if (item.Contains(CombinationIdToCheck))
                    {
                        IsignoreUOMFields = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                        <tr>
                            <td style=""width: 48%"">
                                <table style=""margin-left: 0px; width: 99%;"">
                                <tr>
                                    <td>
                                        <label style=""font-weight: bold;"">
                                            " + OrdNumber + @": </label>
                                        <label style=""font-weight: bold;"">
                                            " + obj.OrderNumber + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResOrder.Comment + @": </label>
                                        <label>
                                            " + obj.Comment + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResOrder.ShippingMethod + @": </label>
                                        <label>
                                            " + obj.ShipViaName + @"</label>
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
                                        " + ResOrder.Supplier + @": </label>
                                    <label>
                                        " +
                                          suppliername
                                          + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(OrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
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
                                        " + ResItemMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResOrder.Bin + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OrdReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
                                    </th>
                                     " + appendExtraFieldsHeader(IsignoreUOMFields) + @"
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
                OrderDetailsDAL objOrdDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.OrderListItem = objOrdDetailDAL.GetOrderDetailByOrderGUIDFull(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));

            }

            if (objSupplierMasterDTP.IsSupplierReceivesKitComponents && obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {
                List<OrderDetailsDTO> objOrderItemList = new List<OrderDetailsDTO>();

                foreach (var item in obj.OrderListItem)
                {
                    objOrderItemList.Add(item);

                    if (item.ItemType == 3)
                    {
                        IEnumerable<KitDetailDTO> objKitDeailList = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false, true);

                        foreach (var KitCompitem in objKitDeailList)
                        {
                            OrderDetailsDTO objOrderDetailDTO = new OrderDetailsDTO()
                            {
                                ItemNumber = KitCompitem.ItemNumber + "&nbsp;" + " <I>" + string.Format(ResOrder.ComponentOfKit, item.ItemNumber) + "</I>",
                                BinName = item.BinName,
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
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.Bin != null && item.Bin > 0)
                        binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID).BinNumber;

                    int OrderUOM = 1;

                    if (item.OrderUOMValue != null && item.OrderUOMValue >= 1)
                        OrderUOM = Convert.ToInt32(item.OrderUOMValue);

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();

                    if (obj.OrderStatus == (int)OrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(ReqQty) && ReqQty != "&nbsp;" && Convert.ToInt32(ReqQty) >= 0) //  && item.SerialNumberTracking == false
                    {
                        int intReqQty = 0;
                        if (item.IsAllowOrderCostuom)
                            intReqQty = Convert.ToInt32(ReqQty) / OrderUOM;
                        else
                            intReqQty = Convert.ToInt32(ReqQty);
                        if (intReqQty <= 0)
                            intReqQty = 0;

                        ReqQty = intReqQty.ToString();
                    }

                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(DateTimeFormat);

                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                         <td>
                            " + item.ItemDescription + @"
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
                         " + appendExtraFieldValues(IsignoreUOMFields, item) + @"
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

        private string appendExtraFieldsHeader(bool IsignoreUOMFields)
        {
            if (!IsignoreUOMFields)
            {
                string extraFields = @"
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.SupplierPartNo + @"
                                    </th>
                                     <th  style=""width: 10%; text-align: left;"">
                                       " + ResItemMaster.UOM + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResCostUOMMaster.CostUOM + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResCostUOMMaster.CostUOMValue + @"
                                    </th>";
                return extraFields;
            }
            else
            {
                return string.Empty;
            }
        }
        private string appendExtraFieldValues(bool IsignoreUOMFields, OrderDetailsDTO item)
        {
            if (!IsignoreUOMFields)
            {
                return @"
                        <td>
                            " + item.SupplierPartNo + @"
                        </td>
                        <td>
                            " + item.Unit + @"
                        </td>
                         <td>
                            " + item.CostUOM + @"
                        </td>
                         <td>
                            " + item.CostUOMValue + @"
                        </td> ";
            }
            else
                return string.Empty;
        }

        private void GetMailBodySupplierDynamicContent(StringBuilder MailBody, OrderMasterDTO obj, string DateTimeFormat = "MM/dd/yyyy")
        {
            string suppliername = "";
            SupplierMasterDTO objSupplierMasterDTP = null;

            try
            {
                if (obj.Supplier != null && obj.Supplier > 0)
                {
                    objSupplierMasterDTP = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(obj.Supplier)));
                    if (objSupplierMasterDTP != null && objSupplierMasterDTP.ID > 0)
                        suppliername = objSupplierMasterDTP.SupplierName;
                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(MailBody.ToString());
                var tableNodes = doc.DocumentNode.SelectNodes("//table");

                if (tableNodes != null && tableNodes.Count() > 0)
                {
                    string[] orderLineItemHeaders = new string[6];
                    orderLineItemHeaders[0] = "@~ITEMNUMBER~@";
                    orderLineItemHeaders[1] = "@~BIN~@";
                    orderLineItemHeaders[2] = "@~DESCRIPTION~@";
                    orderLineItemHeaders[3] = "@~REQUESTEDQUANTITY~@";
                    orderLineItemHeaders[4] = "@~REQUIREDDATE~@";
                    orderLineItemHeaders[5] = "@~SUPPLIERPARTNO~@";
                    Dictionary<int, string> orderLineItemsHeader = new Dictionary<int, string>();
                    bool isCorrectTableFound = false;
                    int startingIndexToReplace = 0;
                    int lengthToReplace = 0;

                    if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
                    {
                        OrderDetailsDAL objOrdDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        obj.OrderListItem = objOrdDetailDAL.GetOrderDetailByOrderGUIDFull(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                    }

                    if (objSupplierMasterDTP.IsSupplierReceivesKitComponents && obj.OrderListItem != null && obj.OrderListItem.Count > 0)
                    {
                        List<OrderDetailsDTO> objOrderItemList = new List<OrderDetailsDTO>();

                        foreach (var item in obj.OrderListItem)
                        {
                            objOrderItemList.Add(item);

                            if (item.ItemType == 3)
                            {
                                IEnumerable<KitDetailDTO> objKitDeailList = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false, true);

                                foreach (var KitCompitem in objKitDeailList)
                                {
                                    OrderDetailsDTO objOrderDetailDTO = new OrderDetailsDTO()
                                    {
                                        ItemNumber = KitCompitem.ItemNumber + "&nbsp;" + " <I>" + string.Format(ResOrder.ComponentOfKit, item.ItemNumber) + "</I>",
                                        BinName = item.BinName,
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

                    foreach (var node in tableNodes)
                    {
                        var rows = node.SelectNodes(node.XPath + "//tr"); //node.SelectNodes(node.FirstChild.XPath + "//tr");//node.SelectNodes("//tr");
                        var headerRows = node.SelectNodes(node.XPath + "//th"); //node.SelectNodes(node.FirstChild.XPath + "//th");

                        if ((rows != null && rows.Count() > 0 && rows.Count() < 2) || (headerRows != null && headerRows.Count() > 0 && headerRows.Count() < 2))
                        {
                            foreach (var raw in rows)
                            {
                                var tds = raw.SelectNodes(raw.XPath + "//td");
                                if (tds != null && tds.Count() > 0 && tds.Count() <= 6)
                                {
                                    bool isCorrectTable = true;
                                    foreach (var td in tds)
                                    {
                                        if (string.IsNullOrEmpty(td.InnerText) || string.IsNullOrWhiteSpace(td.InnerText) ||
                                                !orderLineItemHeaders.Contains(td.InnerText.Trim()))
                                        {
                                            isCorrectTable = false;
                                            break;
                                        }
                                    }

                                    if (isCorrectTable)
                                    {
                                        isCorrectTableFound = true;
                                        for (int cnt = 0; cnt < tds.Count(); cnt++)
                                        {
                                            orderLineItemsHeader.Add(cnt, tds[cnt].InnerText.Trim());
                                        }
                                        break;
                                    }
                                }
                            }

                            if (isCorrectTableFound)
                            {
                                startingIndexToReplace = node.InnerStartIndex;//node.OuterStartIndex;//node.InnerStartIndex;
                                lengthToReplace = node.InnerLength;//node.OuterLength; //node.InnerLength;
                                                                   //startingTag = node.Name;
                                break;
                            }
                        }
                    }

                    if (lengthToReplace > 0)
                    {
                        var stringToReplace = MailBody.ToString().Substring(startingIndexToReplace, lengthToReplace);

                        if (orderLineItemsHeader != null && orderLineItemsHeader.Any() && orderLineItemsHeader.Count() > 0)
                        {
                            var orderLineItemBodyHtml = @"<thead><tr role=""row"">";
                            foreach (var key in orderLineItemsHeader.OrderBy(e => e.Key))
                            {
                                var tmpColName = GetColumnHeaderResourceValue(key.Value.ToLower(), obj);
                                orderLineItemBodyHtml += @"<th  style=""width: 10%; text-align: left;"">" + tmpColName + "</th>";
                            }
                            orderLineItemBodyHtml += "</tr></thead>";

                            if (obj.OrderListItem != null && obj.OrderListItem.Any() && obj.OrderListItem.Count > 0)
                            {
                                orderLineItemBodyHtml += "<tbody>";
                                foreach (var item in obj.OrderListItem)
                                {
                                    string binname = "&nbsp;";
                                    string ReqQty = "&nbsp;";
                                    string ReqDate = "&nbsp;";
                                    if (item.Bin != null && item.Bin > 0)
                                        binname = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(item.Bin)), RoomID, CompanyID).BinNumber;

                                    int OrderUOM = 1;

                                    if (item.OrderUOMValue != null && item.OrderUOMValue >= 1)
                                        OrderUOM = Convert.ToInt32(item.OrderUOMValue);

                                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                                        ReqQty = item.RequestedQuantity.ToString();

                                    if (obj.OrderStatus == (int)OrderStatus.Approved)
                                    {
                                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                                            ReqQty = item.ApprovedQuantity.ToString();
                                    }

                                    if (!string.IsNullOrWhiteSpace(ReqQty) && ReqQty != "&nbsp;" && Convert.ToInt32(ReqQty) >= 0) //  && item.SerialNumberTracking == false
                                    {
                                        int intReqQty = 0;
                                        if (item.IsAllowOrderCostuom)
                                            intReqQty = Convert.ToInt32(ReqQty) / OrderUOM;
                                        else
                                            intReqQty = Convert.ToInt32(ReqQty);
                                        if (intReqQty <= 0)
                                            intReqQty = 0;

                                        ReqQty = intReqQty.ToString();
                                    }

                                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(DateTimeFormat);

                                    orderLineItemBodyHtml += "<tr>";
                                    foreach (var key in orderLineItemsHeader.OrderBy(e => e.Key))
                                    {
                                        switch (key.Value.ToLower())
                                        {
                                            case "@~itemnumber~@":
                                                orderLineItemBodyHtml += "<td>" + item.ItemNumber + "</td>";
                                                break;
                                            case "@~bin~@":
                                                orderLineItemBodyHtml += "<td>" + binname + "</td>";
                                                break;
                                            case "@~description~@":
                                                orderLineItemBodyHtml += "<td>" + item.ItemDescription + "</td>";
                                                break;
                                            case "@~requestedquantity~@":
                                                orderLineItemBodyHtml += "<td>" + ReqQty + "</td>";
                                                break;
                                            case "@~requireddate~@":
                                                orderLineItemBodyHtml += "<td>" + ReqDate + "</td>";
                                                break;
                                            case "@~supplierpartno~@":
                                                orderLineItemBodyHtml += "<td>" + item.SupplierPartNo + "</td>";
                                                break;
                                            default:
                                                orderLineItemBodyHtml += "<td>&nbsp;</td>";
                                                break;
                                        }
                                    }
                                    orderLineItemBodyHtml += "</tr>";
                                }
                                orderLineItemBodyHtml += "</tbody>";
                            }
                            else
                            {
                                orderLineItemBodyHtml += "<tbody><tr><td colspan='" + orderLineItemsHeader.Count() + "'  style='text-align:center'>There is no item for this order</td><tr></tbody>";
                            }
                            MailBody = MailBody.Replace(stringToReplace, orderLineItemBodyHtml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
            }

        }

        private string GetMailBodySupplierForUnSubmitted(OrderMasterDTO obj, string DateTimeFormat = "MM/dd/yyyy")
        {
            string mailBody = "";
            string suppliername = "";
            string SupplierPartNo = string.Empty;
            string OrdNumber = ResOrder.OrderNumber;
            string ReqDateCap = ResOrder.RequiredDate;
            string OrdStatus = ResOrder.OrderStatus;
            string OrdReqQty = ResOrder.RequestedQuantity;
            SupplierMasterDTO objSupplierMasterDTP = null;
            string strRequiredDate = obj.RequiredDate.ToString(DateTimeFormat);

            if (obj.OrderType == (int)OrderType.RuturnOrder)
            {
                OrdNumber = ResOrder.ReturnOrderNumber;
                ReqDateCap = ResOrder.ReturnDate;
                OrdStatus = ResOrder.ReturnOrderStatus;
                OrdReqQty = ResOrder.ReturnQuantity;
            }

            if (obj.Supplier != null && obj.Supplier > 0)
            {
                objSupplierMasterDTP = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(Int64.Parse(Convert.ToString(obj.Supplier)));
                if (objSupplierMasterDTP != null && objSupplierMasterDTP.ID > 0)
                    suppliername = objSupplierMasterDTP.SupplierName;
            }

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                        <tr>
                            <td style=""width: 48%"">
                                <table style=""margin-left: 0px; width: 99%;"">
                                <tr>
                                    <td>
                                        <label style=""font-weight: bold;"">
                                            " + OrdNumber + @": </label>
                                        <label style=""font-weight: bold;"">
                                            " + obj.OrderNumber + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResOrder.Comment + @": </label>
                                        <label>
                                            " + obj.Comment + @"</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>
                                            " + ResOrder.ShippingMethod + @": </label>
                                        <label>
                                            " + obj.ShipViaName + @"</label>
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
                                        " + ResOrder.Supplier + @": </label>
                                    <label>
                                        " +
                                          suppliername
                                          + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + OrdStatus + @": </label>
                                    <label>
                                        " + Enum.Parse(typeof(OrderStatus), obj.OrderStatus.ToString()).ToString() + @"</label>
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
                                        " + ResItemMaster.Description + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + OrdReqQty + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ReqDateCap + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.SupplierPartNo + @"
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
                OrderDetailsDAL objOrdDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.OrderListItem = PreparedOrderLiteItemWithProperData(obj); //GetLineItemsFromSession(obj.ID);
                if (obj.OrderStatus == (int)OrderStatus.UnSubmitted)
                {
                    if (obj.OrderListItem == null || obj.OrderListItem.Count <= 0)
                    {
                        obj.OrderListItem = objOrdDetailDAL.GetOrderDetailByOrderGUIDFull(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));

                    }
                }
            }

            if (objSupplierMasterDTP.IsSupplierReceivesKitComponents && obj.OrderListItem != null && obj.OrderListItem.Count > 0)
            {
                List<OrderDetailsDTO> objOrderItemList = new List<OrderDetailsDTO>();

                foreach (var item in obj.OrderListItem)
                {
                    objOrderItemList.Add(item);

                    if (item.ItemType == 3)
                    {
                        IEnumerable<KitDetailDTO> objKitDeailList = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(item.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID, false, false, true);

                        foreach (var KitCompitem in objKitDeailList)
                        {
                            OrderDetailsDTO objOrderDetailDTO = new OrderDetailsDTO()
                            {
                                ItemNumber = KitCompitem.ItemNumber + "&nbsp;" + " <I>" + string.Format(ResOrder.ComponentOfKit, item.ItemNumber) + "</I>",
                                BinName = item.BinName,
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
                    int OrderUOM = 1;

                    if (item.OrderUOMValue != null && item.OrderUOMValue >= 1)
                        OrderUOM = Convert.ToInt32(item.OrderUOMValue);

                    if (item.RequestedQuantity.GetValueOrDefault(0) > 0)
                        ReqQty = item.RequestedQuantity.ToString();

                    if (obj.OrderStatus == (int)OrderStatus.Approved)
                    {
                        if (item.ApprovedQuantity.GetValueOrDefault(0) > 0)
                            ReqQty = item.ApprovedQuantity.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(ReqQty) && ReqQty != "&nbsp;" && Convert.ToInt32(ReqQty) >= 0) //  && item.SerialNumberTracking == false
                    {
                        int intReqQty = 0;
                        if (item.IsAllowOrderCostuom)
                            intReqQty = Convert.ToInt32(ReqQty) / OrderUOM;
                        else
                            intReqQty = Convert.ToInt32(ReqQty);
                        if (intReqQty <= 0)
                            intReqQty = 0;

                        ReqQty = intReqQty.ToString();
                    }
                    if (item.RequiredDate != null && item.RequiredDate.HasValue)
                        ReqDate = strRequiredDate;

                    if (!string.IsNullOrEmpty(item.ItemDescription))
                    {
                        desc = item.ItemDescription;
                    }

                    trs += @"<tr>
                        <td>
                            " + item.ItemNumber + @"
                        </td>
                        <td>
                            " + item.BinName + @"
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
                         <td>
                            " + item.SupplierPartNo + @"
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
        public void SendMailToApprovalAuthority(OrderMasterDTO objOrder)
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
                if ((objOrder.OrderType ?? 0) == (int)OrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.OrderApproval, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                    lstNotifications = lstNotifications.Where(t => (t.SupplierIds ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(Convert.ToString(objOrder.Supplier ?? 0))).ToList();
                    //lstNotifications = lstNotifications.Where(n => n.SupplierIds == Convert.ToString(objOrder.Supplier ?? 0)).ToList();

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
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
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
                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                MessageBody.Replace("@@TABLE@@", GetMailBody(objOrder));
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
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);

                                if (!string.IsNullOrWhiteSpace(strToAddress))
                                {
                                    List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                                    if (EmailAddrs != null && EmailAddrs.Count > 0)
                                    {
                                        foreach (var emailitem in EmailAddrs)
                                        {
                                            string strdata = objOrder.ID + "^" + objOrder.Room + "^" + objOrder.CompanyID + "^" + (objOrder.LastUpdatedBy ?? objOrder.CreatedBy) + "^" + SessionHelper.EnterPriceID.ToString() + "^" + objOrder.LastUpdatedBy.GetValueOrDefault(0) + "^" + emailitem; ;
                                            string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
                                            string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");
                                            List<eMailAttachmentDTO> objeMailAttchListNew = new List<eMailAttachmentDTO>();

                                            if (lstAttachments != null)
                                            {
                                                foreach (var item in lstAttachments)
                                                {
                                                    objeMailAttchListNew.Add(item);
                                                }
                                            }

                                            MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/OrderStatus?eKey=" + approvalURLData + "'>'" + ResCommon.Approve + "'</a> &nbsp;&nbsp;<a href='" + replacePart + "/EmailLink/OrderStatus?eKey=" + rejectURLData + "'>'" + ResCommon.Reject + "'</a>");
                                            objAlertMail.CreateAlertMail(objeMailAttchListNew, StrSubject, MessageBody.ToString(), emailitem, t, objEnterpriseDTO);
                                        }
                                    }
                                }
                            }
                        });
                    }
                }

                if ((objOrder.OrderType ?? 0) == (int)OrderType.RuturnOrder)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ReturnOrderApproval, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                    lstNotifications = lstNotifications.Where(n => n.SupplierIds.Contains(Convert.ToString(objOrder.Supplier ?? 0))).ToList();

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
                            string StrSubject = t.EmailTemplateDetail.MailSubject;

                            string strToAddress = t.EmailAddress;
                            if (!string.IsNullOrEmpty(strToAddress))
                            {
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
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
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
                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                MessageBody.Replace("@@TABLE@@", GetMailBody(objOrder));
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
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                lstAttachments = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);

                                if (!string.IsNullOrWhiteSpace(strToAddress))
                                {
                                    List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    if (EmailAddrs != null && EmailAddrs.Count > 0)
                                    {
                                        foreach (var emailitem in EmailAddrs)
                                        {
                                            string strdata = objOrder.ID + "^" + objOrder.Room + "^" + objOrder.CompanyID + "^" + (objOrder.LastUpdatedBy ?? objOrder.CreatedBy) + "^" + SessionHelper.EnterPriceID.ToString() + "^" + objOrder.LastUpdatedBy.GetValueOrDefault(0) + "^" + emailitem;
                                            string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
                                            string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");
                                            MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/OrderStatus?eKey=" + approvalURLData + "'>'" + ResCommon.Approve + "'</a> &nbsp;&nbsp;<a href='" + replacePart + "/EmailLink/OrderStatus?eKey=" + rejectURLData + "'>'" + ResCommon.Reject + "'</a>");

                                            objAlertMail.CreateAlertMail(lstAttachments, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
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
                MessageBody = null;
                objNotificationDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }

        /// <summary>
        /// Send Mail to Supplier
        /// </summary>
        /// <param name="ToSupplier"></param>
        /// <param name="objOrder"></param>
        public void SendMailToSupplier(SupplierMasterDTO ToSuppliers, OrderMasterDTO objOrder,bool IsCallNewFuction=false)
        {
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            eMailMasterDAL objEmailDAL = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();

            try
            {
                if ((objOrder.OrderType ?? 0) == (int)OrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplateQuoteandOrder((long)MailTemplate.OrderToSupplier, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                    lstNotifications = lstNotifications.Where(t => (t.SupplierIds ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(Convert.ToString(ToSuppliers.ID))).ToList();
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
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);

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

                                string SupplierName = objOrder.SupplierName;

                                if (string.IsNullOrEmpty(objOrder.SupplierName) && string.IsNullOrWhiteSpace(objOrder.SupplierName)
                                    && objOrder.Supplier.GetValueOrDefault(0) > 0)
                                {
                                    var orderSupplier = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(objOrder.Supplier.GetValueOrDefault(0));
                                    if (orderSupplier != null && orderSupplier.ID > 0)
                                    {
                                        SupplierName = orderSupplier.SupplierName;
                                        objOrder.SupplierName = SupplierName;
                                    }
                                }

                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                MessageBody.Replace("@@Supplier@@", SupplierName);
                                MessageBody.Replace("@@COMMENT@@", objOrder.Comment);
                                MessageBody.Replace("@@SHIPPINGMETHOD@@", objOrder.ShipViaName);
                                MessageBody.Replace("@@REQUIREDDATE@@", objOrder.RequiredDate.ToString(DateTimeFormat));
                                MessageBody.Replace("@@ORDERSTATUS@@", Enum.Parse(typeof(OrderStatus), objOrder.OrderStatus.ToString()).ToString());
                                string stratatTABLEatatTag = GetMailBodySupplier(objOrder, DateTimeFormat);
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

                                if (ToSuppliers.IsEmailPOInBody)
                                {
                                    if (MessageBody != null && !string.IsNullOrEmpty(MessageBody.ToString()) && !string.IsNullOrWhiteSpace(MessageBody.ToString()))
                                    {
                                        GetMailBodySupplierDynamicContent(MessageBody, objOrder, DateTimeFormat);
                                    }
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else if (!ToSuppliers.IsEmailPOInCSV && !ToSuppliers.IsEmailPOInPDF)
                                {
                                    if (MessageBody != null && !string.IsNullOrEmpty(MessageBody.ToString()) && !string.IsNullOrWhiteSpace(MessageBody.ToString()))
                                    {
                                        GetMailBodySupplierDynamicContent(MessageBody, objOrder, DateTimeFormat);
                                    }
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else
                                {
                                    string strReplText = ResOrder.SeeAttachedFilesForOrderDetail;
                                    if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                                        strReplText = ResOrder.SeeAttachedFilesForReturnOrderDetail;

                                    MessageBody.Replace("@@TABLE@@", strReplText);
                                }

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
                                if (IsCallNewFuction)
                                    objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlertQuoteandOrder(t, objEnterpriseDTO, Params);
                               else
                                    objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                t.RoomID = RoomID; 
                                objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                            }
                        });
                    }
                }

                if ((objOrder.OrderType ?? 0) == (int)OrderType.RuturnOrder)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ReturnOrderApproval, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

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
                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
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

                                string DateTimeFormat = "MM/dd/yyyy";
                                DateTime TZDateTimeNow = DateTime.UtcNow;

                                if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                                {
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);

                                    if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                                    {
                                        StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                                    }

                                    if (objEnterpriseDTO != null)
                                    {
                                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                        eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                                        DateTimeFormat = "MM/dd/yyyy";

                                        if (objeTurnsRegionInfoNew != null)
                                        {
                                            DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                                            TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                                        }

                                        if (StrSubject != null && StrSubject.ToLower().Contains("@@date@@"))
                                        {
                                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                            StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);

                                            if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                                            {
                                                StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                                            }

                                            if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                                            {
                                                StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                                            }
                                            StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                                        }
                                    }
                                }

                                if (MessageBody != null && objOrder != null)
                                {
                                    string SupplierName = objOrder.SupplierName;

                                    if (string.IsNullOrEmpty(objOrder.SupplierName) && string.IsNullOrWhiteSpace(objOrder.SupplierName)
                                        && objOrder.Supplier.GetValueOrDefault(0) > 0)
                                    {
                                        var orderSupplier = new SupplierMasterDAL(SessionHelper.EnterPriseDBName).GetSupplierByIDPlain(objOrder.Supplier.GetValueOrDefault(0));
                                        if (orderSupplier != null && orderSupplier.ID > 0)
                                        {
                                            SupplierName = orderSupplier.SupplierName;
                                            objOrder.SupplierName = SupplierName;
                                        }
                                    }

                                    MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                    MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                    MessageBody.Replace("@@Supplier@@", SupplierName);
                                    MessageBody.Replace("@@COMMENT@@", objOrder.Comment);
                                    MessageBody.Replace("@@SHIPPINGMETHOD@@", objOrder.ShipViaName);
                                    MessageBody.Replace("@@REQUIREDDATE@@", objOrder.RequiredDate.ToString(DateTimeFormat));
                                    MessageBody.Replace("@@ORDERSTATUS@@", Enum.Parse(typeof(OrderStatus), objOrder.OrderStatus.ToString()).ToString());
                                }
                                string stratatTABLEatatTag = GetMailBodySupplier(objOrder, DateTimeFormat);

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

                                if (ToSuppliers.IsEmailPOInBody)
                                {
                                    if (MessageBody != null && !string.IsNullOrEmpty(MessageBody.ToString()) && !string.IsNullOrWhiteSpace(MessageBody.ToString()))
                                    {
                                        GetMailBodySupplierDynamicContent(MessageBody, objOrder, DateTimeFormat);
                                    }
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else if (!ToSuppliers.IsEmailPOInCSV && !ToSuppliers.IsEmailPOInPDF)
                                {
                                    if (MessageBody != null && !string.IsNullOrEmpty(MessageBody.ToString()) && !string.IsNullOrWhiteSpace(MessageBody.ToString()))
                                    {
                                        GetMailBodySupplierDynamicContent(MessageBody, objOrder, DateTimeFormat);
                                    }
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else
                                {
                                    string strReplText = ResOrder.SeeAttachedFilesForOrderDetail;
                                    if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                                        strReplText = ResOrder.SeeAttachedFilesForReturnOrderDetail;

                                    MessageBody.Replace("@@TABLE@@", strReplText);
                                }

                                objeMailAttchList = new List<eMailAttachmentDTO>();

                                if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                                {
                                    MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                                }
                                MessageBody = MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                                if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                                {
                                    MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                }
                                eTurnsRegionInfo objeTurnsRegionInfo = null;
                                if (objEnterpriseDTO != null)
                                {
                                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                                    objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                                }

                                if (objeTurnsRegionInfo != null)
                                {
                                    DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                                    TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                                }
                                if (MessageBody != null)
                                {
                                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                                    MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                                    MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                                    MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                                    MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));
                                }

                                Dictionary<string, string> Params = new Dictionary<string, string>();
                                Params.Add("DataGuids", objOrder.GUID.ToString());
                                objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);
                                objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);
                            }
                        });
                    }
                }
            }
            finally
            {
                objEmailDAL = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objeMailAttchList = null;
            }
        }

        /// <summary>
        /// SendMailForApprovedOrReject
        /// </summary>
        /// <param name="objOrder"></param>
        /// <param name="AprvRejString"></param>
        public void SendMailForApprovedOrReject(OrderMasterDTO objOrder, string AprvRejString, string OrdRequesterEmailAddress, string OrdApproverEmailAddress)
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
                if ((objOrder.OrderType ?? 0) == (int)OrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.OrderApproveReject, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

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
                                if (strToAddress.Contains("[Requester]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(OrdRequesterEmailAddress))
                                        strToAddress = strToAddress.Replace("[Requester]", OrdRequesterEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Requester]", "");
                                }
                                if (strToAddress.Contains("[Approver]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(OrdApproverEmailAddress))
                                        strToAddress = strToAddress.Replace("[Approver]", OrdApproverEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Approver]", "");
                                }
                                List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                EmailAddrs = EmailAddrs.Distinct().ToList();
                                strToAddress = string.Join(",", EmailAddrs);

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
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
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

                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
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

                if ((objOrder.OrderType ?? 0) == (int)OrderType.RuturnOrder)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.ReturnOrderApproveReject, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);

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
                            string StrSubject = t.EmailTemplateDetail.MailSubject;
                            string strToAddress = t.EmailAddress;

                            if (!string.IsNullOrEmpty(strToAddress))
                            {
                                if (strToAddress.Contains("[Requester]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(OrdRequesterEmailAddress))
                                        strToAddress = strToAddress.Replace("[Requester]", OrdRequesterEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Requester]", "");
                                }
                                if (strToAddress.Contains("[Approver]"))
                                {
                                    if (!string.IsNullOrWhiteSpace(OrdApproverEmailAddress))
                                        strToAddress = strToAddress.Replace("[Approver]", OrdApproverEmailAddress);
                                    else
                                        strToAddress = strToAddress.Replace("[Approver]", "");
                                }
                                List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                EmailAddrs = EmailAddrs.Distinct().ToList();
                                strToAddress = string.Join(",", EmailAddrs);

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
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
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
                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
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
        public void SendMailOrderUnSubmitted(SupplierMasterDTO ToSuppliers, OrderMasterDTO objOrder)
        {
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            eMailMasterDAL objEmailDAL = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();

            try
            {
                #region [Order]
                if ((objOrder.OrderType ?? 0) == (int)OrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.OrderUnSubmitted, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                    lstNotifications = lstNotifications.Where(t => (t.SupplierIds ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(Convert.ToString(ToSuppliers.ID))).ToList();

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
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);

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

                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                MessageBody.Replace("@@Supplier@@", objOrder.SupplierName);
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

                                if (ToSuppliers.IsEmailPOInBody)
                                {
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else if (!ToSuppliers.IsEmailPOInCSV && !ToSuppliers.IsEmailPOInPDF)
                                {
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else
                                {
                                    string strReplText = ResOrder.SeeAttachedFilesForOrderDetail;
                                    if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                                        strReplText = ResOrder.SeeAttachedFilesForReturnOrderDetail;

                                    MessageBody.Replace("@@TABLE@@", strReplText);
                                }

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
                            }
                        });
                    }

                }
                #endregion
            }
            finally
            {
                objEmailDAL = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objeMailAttchList = null;
            }
        }
        /// <summary>
        /// Send Mail Order UnSubmitted
        /// </summary>
        /// <param name="ToSupplier"></param>
        /// <param name="objOrder"></param>
        public void SendMailOrderClosed(SupplierMasterDTO ToSuppliers, OrderMasterDTO objOrder)
        {
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            eMailMasterDAL objEmailDAL = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();

            try
            {
                #region [Order]
                if ((objOrder.OrderType ?? 0) == (int)OrderType.Order)
                {
                    objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    lstNotifications = objNotificationDAL.GetAllSchedulesByReportName("Orders Closed", SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                    lstNotifications = lstNotifications.Where(t => (t.SupplierIds ?? String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(Convert.ToString(ToSuppliers.ID))).ToList();

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
                                    StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);

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

                                MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                                MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                                MessageBody.Replace("@@Supplier@@", objOrder.SupplierName);
                                // string stratatTABLEatatTag = GetMailBodySupplierForUnSubmitted(objOrder, DateTimeFormat);
                                string stratatTABLEatatTag = "";
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

                                if (ToSuppliers.IsEmailPOInBody)
                                {
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else if (!ToSuppliers.IsEmailPOInCSV && !ToSuppliers.IsEmailPOInPDF)
                                {
                                    MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                                }
                                else
                                {
                                    string strReplText = ResOrder.SeeAttachedFilesForOrderDetail;
                                    if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                                        strReplText = ResOrder.SeeAttachedFilesForReturnOrderDetail;

                                    MessageBody.Replace("@@TABLE@@", strReplText);
                                }

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
                            }
                        });
                    }

                }
                #endregion
            }
            finally
            {
                objEmailDAL = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objeMailAttchList = null;
            }
        }

        /// <summary>
        /// Send Mail to Approval authority for Approve Order
        /// </summary>
        /// <param name="strToEmailAddress"></param>
        /// <param name="objOrder"></param>
        public void SendMailToApprovalAuthorityOLD(List<UserMasterDTO> arrUsers, OrderMasterDTO objOrder)
        {
            eTurnsUtility objUtils = null;
            StringBuilder MessageBody = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            eMailMasterDAL objEmailDAL = null;

            try
            {
                string StrSubject = ResOrder.OrderApprovalleMailSubject;
                string strTemplateName = "OrderApproval";

                if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                {
                    StrSubject = ResOrder.ReturnOrderApprovalMailSubject;
                    strTemplateName = "ReturnOrderApproval";
                }

                string strToAddress = CommonUtility.GetEmailToAddress(arrUsers, strTemplateName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                string DateTimeFormat = "MM/dd/yyyy";
                DateTime TZDateTimeNow = DateTime.UtcNow;
                if (objeTurnsRegionInfoNew != null)
                {
                    DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                    TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                }

                if (!string.IsNullOrEmpty(strToAddress))
                {
                    string strCCAddress = "";
                    MessageBody = new StringBuilder();
                    objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                    objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate(strTemplateName, ResourceHelper.CurrentCult.ToString(), RoomID, CompanyID);
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
                        StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                        if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                        {
                            StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                        }

                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);

                        if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                        {
                            StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                        }

                        if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                        {
                            StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                        }

                        StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                    }

                    MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                    MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                    MessageBody.Replace("@@TABLE@@", GetMailBody(objOrder));
                    MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                    MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    string urlPart = Request.Url.ToString();
                    string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                    string strdata = objOrder.ID + "^" + objOrder.Room + "^" + objOrder.CompanyID + "^" + arrUsers[0].ID + "^" + SessionHelper.EnterPriceID.ToString() + "^" + objOrder.LastUpdatedBy.GetValueOrDefault(0);
                    string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
                    string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");
                    MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/OrderStatus?eKey=" + approvalURLData + "'>'" + ResCommon.Approve + "'</a> &nbsp;&nbsp;<a href='" + replacePart + "/EmailLink/OrderStatus?eKey=" + rejectURLData + "'>'" + ResCommon.Reject + "'</a>");
                    objUtils = new eTurnsUtility();
                    objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                    objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);

                    if (MessageBody != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);

                        if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                        {
                            MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                        }

                        if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                        {
                            MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                        }

                        MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                    }
                    objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, CompanyID, RoomID, SessionHelper.UserID, null, "Web => Order => OrderToAoApproval");
                }
            }
            finally
            {
                objUtils = null;
                objEmailDAL = null;
                MessageBody = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }

        /// <summary>
        /// Send Mail to Supplier
        /// </summary>
        /// <param name="ToSupplier"></param>
        /// <param name="objOrder"></param>
        public void SendMailToSupplierOLD(SupplierMasterDTO ToSuppliers, OrderMasterDTO objOrder)
        {
            eMailMasterDAL objEmailDAL = null;
            IEnumerable<EmailUserConfigurationDTO> extUsers = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            string[] splitEmails = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            eMailAttachmentDTO objeMailAttch = null;

            try
            {
                string StrSubject = ResOrder.OrderToSupplierMailSubject;// "Order Approval Request";;// "Order Approval Request";
                string strToAddress = ConfigurationManager.AppSettings["OverrideToEmail"];
                string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
                string strTemplateName = "OrderToSupplier";

                if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                {
                    StrSubject = ResOrder.ReturnOrderToSupplierMailSubject;
                    strTemplateName = "ReturnOrderToSupplier";
                }

                if (!string.IsNullOrEmpty(strToAddress) && !string.IsNullOrWhiteSpace(strToAddress))
                {
                    string strEmails = string.Empty;
                    string[] strArrToAddress = strToAddress.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var itemToAddr in strArrToAddress)
                    {
                        if (!string.IsNullOrEmpty(ToSuppliers.Email))
                        {
                            splitEmails = ToSuppliers.Email.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in splitEmails)
                            {
                                if (!string.IsNullOrEmpty(strEmails))
                                    strEmails += ",";

                                strEmails += @"""" + item + @"""" + @"<" + itemToAddr + @">";
                            }
                        }

                        //Add External User for send mail
                        extUsers = GetExternalUserEmailAddress(strTemplateName);
                        if (extUsers != null && extUsers.Count() > 0)
                        {
                            foreach (var item in extUsers)
                            {
                                if (!string.IsNullOrEmpty(strEmails))
                                    strEmails += ",";

                                strEmails += @"""" + item.Email + @"""" + @"<" + itemToAddr + @">";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(strEmails))
                        strToAddress = strEmails;
                }
                else
                {
                    strToAddress = string.Empty;
                    splitEmails = ToSuppliers.Email.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in splitEmails)
                    {
                        if (!string.IsNullOrEmpty(strToAddress))
                            strToAddress += ",";

                        strToAddress += item;
                    }

                    //Add External User for send mail
                    extUsers = GetExternalUserEmailAddress(strTemplateName);
                    if (extUsers != null && extUsers.Count() > 0)
                    {
                        foreach (var item in extUsers)
                        {
                            if (!string.IsNullOrEmpty(strToAddress))
                                strToAddress += ",";

                            strToAddress += item.Email;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(strToAddress))
                {
                    objEmailDAL = new eTurns.DAL.eMailMasterDAL(SessionHelper.EnterPriseDBName);
                    string strCCAddress = "";
                    StringBuilder MessageBody = new StringBuilder();
                    objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                    objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate(strTemplateName, ResourceHelper.CurrentCult.ToString(), RoomID, CompanyID);

                    if (objEmailTemplateDetailDTO != null)
                    {
                        MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                        StrSubject = objEmailTemplateDetailDTO.MailSubject;
                    }
                    else
                    {
                        return;
                    }
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                    eTurnsRegionInfo objeTurnsRegionInfoNew = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                    string DateTimeFormat = "MM/dd/yyyy";
                    DateTime TZDateTimeNow = DateTime.UtcNow;

                    if (objeTurnsRegionInfoNew != null)
                    {
                        DateTimeFormat = objeTurnsRegionInfoNew.ShortDatePattern;// + " " + objeTurnsRegionInfoNew.ShortTimePattern;
                        TZDateTimeNow = objeTurnsRegionInfoNew.TZDateTimeNow ?? DateTime.UtcNow;
                    }

                    if (StrSubject != null && !string.IsNullOrWhiteSpace(StrSubject))
                    {
                        StrSubject = StrSubject.Replace("@@ORDERNO@@", objOrder.OrderNumber);

                        if (StrSubject != null && StrSubject.ToLower().Contains("@@releaseno@@"))
                        {
                            StrSubject = StrSubject.Replace("@@RELEASENO@@", objOrder.ReleaseNumber).Replace("@@releaseno@@", objOrder.ReleaseNumber).Replace("@@Releaseno@@", objOrder.ReleaseNumber).Replace("@@ReleaseNo@@", objOrder.ReleaseNumber);
                        }
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);

                        if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                        {
                            StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                        {
                            StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                        }

                        StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

                    }
                    MessageBody.Replace("@@ORDERNO@@", objOrder.OrderNumber);
                    MessageBody.Replace("@@RELEASENO@@", objOrder.ReleaseNumber);
                    MessageBody.Replace("@@Supplier@@", objOrder.SupplierName);
                    string stratatTABLEatatTag = GetMailBodySupplier(objOrder, DateTimeFormat);
                    string urlPart = Request.Url.ToString();
                    string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];

                    if (ToSuppliers.IsEmailPOInBody)
                    {
                        MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                    }
                    else if (!ToSuppliers.IsEmailPOInCSV && !ToSuppliers.IsEmailPOInPDF)
                    {
                        MessageBody.Replace("@@TABLE@@", stratatTABLEatatTag);
                    }
                    else
                    {
                        string strReplText = ResOrder.SeeAttachedFilesForOrderDetail;
                        if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                            strReplText = ResOrder.SeeAttachedFilesForReturnOrderDetail;

                        MessageBody.Replace("@@TABLE@@", strReplText);
                    }

                    objeMailAttchList = new List<eMailAttachmentDTO>();

                    if (ToSuppliers.IsEmailPOInPDF)
                    {
                        try
                        {
                            objeMailAttch = new eMailAttachmentDTO();
                            string fileName = "PDF_Order_" + objOrder.OrderNumber + ".pdf";
                            if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                            {
                                fileName = "PDF_ReturnOrder_" + objOrder.OrderNumber + ".pdf";
                            }
                            objeMailAttch.FileData = GetPDFStreamToAttachInMail(stratatTABLEatatTag, objOrder, null);
                            objeMailAttch.eMailToSendID = 0;
                            objeMailAttch.MimeType = "application/pdf";
                            objeMailAttch.AttachedFileName = fileName;
                            objeMailAttchList.Add(objeMailAttch);
                        }
                        catch (Exception ex)
                        {
                            objEmailDAL.eMailToSend(strBCCAddress, "", " Error IN PDF " + StrSubject, MessageBody.ToString() + "<br/>" + ex.ToString(), SessionHelper.EnterPriceID, CompanyID, RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier => Error During Create PDF ");
                            MessageBody.AppendLine(" ");
                            MessageBody.AppendLine(" Note: PDF not attached due to some error.");
                        }
                    }

                    if (ToSuppliers.IsEmailPOInCSV)
                    {
                        try
                        {
                            objeMailAttch = new eMailAttachmentDTO();
                            string fileName = "CSV_Order_" + objOrder.OrderNumber + ".csv";
                            if (objOrder.OrderType.GetValueOrDefault(0) == (int)OrderType.RuturnOrder)
                            {
                                fileName = "CSV_ReturnOrder_" + objOrder.OrderNumber + ".csv";
                            }

                            objeMailAttch.FileData = GetCVSStringTOAttachINMail(objOrder);
                            objeMailAttch.eMailToSendID = 0;
                            objeMailAttch.MimeType = "application/csv";
                            objeMailAttch.AttachedFileName = fileName;
                            objeMailAttchList.Add(objeMailAttch);
                        }
                        catch (Exception ex)
                        {
                            objEmailDAL.eMailToSend(strBCCAddress, "", " Error IN CSV " + StrSubject, MessageBody.ToString() + "<br />" + ex.ToString(), SessionHelper.EnterPriceID, CompanyID, RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier => Error During Create CSV ");
                            MessageBody.AppendLine(" ");
                            MessageBody.AppendLine(" Note: csv not attached due to some error.");
                            MessageBody.AppendLine(" ");
                        }
                        finally
                        {
                        }
                    }

                    MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);

                    if (MessageBody != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);

                        if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                        {
                            MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                        }

                        if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                        {
                            MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                        }

                        MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));
                    }

                    objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, CompanyID, RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier");
                }
            }
            finally
            {
                objEmailDAL = null;
                extUsers = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                splitEmails = null;
                objeMailAttchList = null;
                objeMailAttch = null;
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
        private byte[] GetPDFStreamToAttachInMail(string strHTML, OrderMasterDTO objOrderDTO, NotificationDTO objNotificationDTO)
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
        private byte[] GetCVSStringTOAttachINMail(OrderMasterDTO obj)
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
            string stringCsvData = "Order#,Supplier,Require Date,Comment,Item#,Quanity,Item Required Date";
            byte[] data = null;
            OrderDetailsDAL objOrdDetailDAL = null;

            try
            {

                objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objReportMasterDAL.GetReportList().FirstOrDefault(x => x.IsBaseReport && x.ModuleName == "Order" && x.ReportType == 2 && x.ReportName == "Order");

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
                            objOrdDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                            obj.OrderListItem = objOrdDetailDAL.GetOrderDetailByOrderGUIDFull(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                        }
                        if (obj.OrderListItem != null && obj.OrderListItem.Count > 0)
                        {
                            foreach (var item in obj.OrderListItem)
                            {
                                stringCsvData += "\r\n";
                                stringCsvData += string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\""
                                                            , obj.OrderNumber.Replace(",", " ")
                                                            , obj.SupplierName.Replace(",", " ")
                                                            , obj.RequiredDate.ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture)
                                                            , obj.Comment == null ? "" : obj.Comment.Replace(",", " ")
                                                            , item.ItemNumber.Replace(",", " ")
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


        #endregion

        #region Order History
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderMasterListHistory()
        {
            return PartialView("_OrderListHistory");
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderHistoryView(Int64 ID)
        {
            OrderMasterDAL obj = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objDTO = obj.GetOrderHistoryByHistoryIDFull(ID);
            objDTO.IsRecordNotEditable = true;
            ViewBag.UDFs = GetUDFDataPageWise("OrderMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            ViewBag.OrderStatusList = GetOrderStatusList(objDTO, "");
            List<CustomerMasterDTO> lstCust = new CustomerMasterDAL(SessionHelper.EnterPriseDBName).GetCustomersByRoomID(RoomID, CompanyID).OrderBy(x => x.Customer).ToList();
            lstCust.Insert(0, new CustomerMasterDTO());
            //List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetAllRecords(RoomID, CompanyID, false, false).OrderBy(t => t.StagingName).ToList();
            List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetMaterialStaging(RoomID, CompanyID, false, false, string.Empty, null).OrderBy(t => t.StagingName).ToList();
            lstStaging.Insert(0, new MaterialStagingDTO());
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();

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

            lstSupplier.Insert(0, new SupplierMasterDTO());
            List<ShipViaDTO> lstShipVia = new ShipViaDAL(SessionHelper.EnterPriseDBName).GetShipViaByRoomIDPlain(RoomID, CompanyID).OrderBy(x => x.ShipVia).ToList();
            lstShipVia.Insert(0, new ShipViaDTO());
            ViewBag.CustomerList = lstCust;
            ViewBag.StagingList = lstStaging;
            ViewBag.SupplierList = lstSupplier;
            ViewBag.ShipViaList = lstShipVia;

            return PartialView("_CreateOrder_History", objDTO);
        }

        /// <summary>
        /// LoadOrderLineItemsHistory
        /// </summary>
        /// <param name="historyID"></param>
        /// <returns></returns>
        public ActionResult LoadOrderLineItemsHistory(Int64 historyID)
        {
            OrderMasterDTO objDTO = null;
            ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            objDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderHistoryByHistoryIDNormal(historyID);
            objDTO.OrderListItem = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailHistoryByOrderGUIDNormal(objDTO.GUID);

            objDTO.OrderListItem.ForEach(x =>
            {
                ItemMasterDTO ItemDTO = ItemDAL.GetItemByGuidNormal(x.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                if (ItemDTO != null && (ItemDTO.OrderUOMValue == null || ItemDTO.OrderUOMValue <= 0))
                    ItemDTO.OrderUOMValue = 1;

                if (x.SerialNumberTracking == false && ItemDTO.IsAllowOrderCostuom)
                {
                    if (x.RequestedQuantity != null && x.RequestedQuantity > 0)
                        x.RequestedQuantity = x.RequestedQuantity / ItemDTO.OrderUOMValue;

                    if (x.ApprovedQuantity != null && x.ApprovedQuantity > 0)
                        x.ApprovedQuantity = x.ApprovedQuantity / ItemDTO.OrderUOMValue;

                    if (x.ReceivedQuantity != null && x.ReceivedQuantity > 0)
                        x.ReceivedQuantity = x.ReceivedQuantity / ItemDTO.OrderUOMValue;

                    if (x.ReceivedQuantity != null && x.ReceivedQuantity % 1 != 0)
                        x.ReceivedQuantity = 0;

                    if (x.InTransitQuantity != null && x.InTransitQuantity > 0)
                        x.InTransitQuantity = x.InTransitQuantity / ItemDTO.OrderUOMValue;

                    if (x.InTransitQuantity != null && x.InTransitQuantity % 1 != 0)
                        x.InTransitQuantity = 0;
                }
            });

            objDTO.IsRecordNotEditable = true;
            return PartialView("_OrderLineItems_History", objDTO);

        }

        #endregion

        #region Order NarrowSearch

        public JsonResult GetNarrowSearchData(bool IsDeleted, bool IsArchived, string OrderStatus)
        {
            string MainFilter = "";

            if (Convert.ToString(Session["MainFilter"]).Trim().ToLower() == "true")
            {
                MainFilter = "true";
            }

            CommonDAL objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
            NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("OrderMaster", CompanyID, RoomID, IsArchived, IsDeleted, OrderStatus, SessionHelper.UserSupplierIds, MainFilter, GetOrderType);

            return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearchData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetOrderNarrwSearchHTML
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrderNarrwSearchHTML(string tabName)
        {
            CommonDTO commonDTO = new CommonDTO
            {
                ListName = string.IsNullOrEmpty(tabName) ? "OrderMaster" : "OrderMaster" + tabName,
                PageName = "OrderMaster"
            };
            return PartialView("_OrderNarrowSearch", commonDTO);
        }

        #endregion

        #region For Edit Required Date and comment for Close Order -- 4661 Add a back ordered checkbox to order details records in the order page

        [HttpPost]
        public JsonResult UpdateReqDateandCommenttoOrderLineItems(OrderDetailsDTO[] arrDetails, Int64 OrderID, bool isCommentUpdate, bool isReqDateUpdate)
        {
            try
            {
                List<OrderDetailsDTO> objOrderList = arrDetails.ToList();

                foreach (var item in objOrderList)
                {
                    OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
                    objOrderDetailsDTO.ID = item.ID;
                    objOrderDetailsDTO.ItemGUID = item.ItemGUID;
                    objOrderDetailsDTO.OrderGUID = item.OrderGUID;
                    objOrderDetailsDTO.Comment = item.Comment;
                    if (!string.IsNullOrEmpty(item.RequiredDateStr))
                    {
                        objOrderDetailsDTO.RequiredDate = !string.IsNullOrEmpty(item.RequiredDateStr) ? Convert.ToDateTime(DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture)) : Convert.ToDateTime(item.RequiredDateStr);
                    }
                    OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    objOrderDetailDAL.UpdateReqDateandCommenttoOrderLineItems(objOrderDetailsDTO, SessionHelper.UserID, isCommentUpdate, isReqDateUpdate);

                    List<OrderDetailsDTO> objSessionOrderList = (List<OrderDetailsDTO>)SessionHelper.Get(GetSessionKey(OrderID));

                    if (objSessionOrderList != null && objSessionOrderList.Count > 0)
                    {
                        OrderDetailsDTO detailFromSession = objSessionOrderList.FirstOrDefault(x => x.ItemGUID == item.ItemGUID && x.ID == item.ID);
                        if (detailFromSession != null)
                        {
                            if (isCommentUpdate)
                            {
                                detailFromSession.Comment = item.Comment;
                            }
                            if (isReqDateUpdate)
                            {
                                detailFromSession.RequiredDateStr = item.RequiredDateStr;
                                detailFromSession.RequiredDate = item.RequiredDateStr != null
                                    ? Convert.ToDateTime(DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture))
                                    : Convert.ToDateTime(item.RequiredDateStr);
                            }
                        }
                    }
                }
                return Json(new { Status = "ok", Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Status = "No", Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
            }
        }

        #endregion

        public bool CheckSupplierAproveRight(long SupplierID)
        {
            bool SupplierAprove = true;
            UserAccessDTO objUserAccess = null;
            if (SessionHelper.UserType == 1)
            {
                eTurnsMaster.DAL.UserMasterDAL objUserdal = new eTurnsMaster.DAL.UserMasterDAL();
                objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
            }
            else
            {
                eTurns.DAL.UserMasterDAL objUserdal = new UserMasterDAL(SessionHelper.EnterPriseDBName);
                objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
            }

            if (objUserAccess != null && !string.IsNullOrWhiteSpace(objUserAccess.SupplierIDs))
            {
                List<string> strSupplier = objUserAccess.SupplierIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (strSupplier != null && strSupplier.Count > 0)
                {
                    if (strSupplier.Contains(SupplierID.ToString()) == false)
                    {
                        SupplierAprove = false;
                    }
                }
            }

            return SupplierAprove;
        }

        #region expand inner grid for return order

        public ActionResult ReturnItem(Guid OrderDetailGUID)
        {
            ReceivableItemDTO objDTO = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetReturnItemsDetailsByOrderDetailGuid(OrderDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
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
            return PartialView("_ReturnItem", (objDTO == null ? new ReceivableItemDTO() : objDTO));
        }

        public string ReturnOrderDetail(string ItemGUID, string ordDetailGUID)
        {
            ViewBag.ItemGUID = ItemGUID;
            Guid? OrderDetailGUID = Guid.Parse(ordDetailGUID);
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, Guid.Parse(ItemGUID));
            ViewBag.ItemID = Objitem.ID;
            ReceivedOrderTransferDetailDAL objAPI = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetROTDByOrderDetailGUIDFull(OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
            OrderDetailsDTO objOrderDetailDTO = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailByGuidPlain(OrderDetailGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
            ViewBag.IsCloseItem = objOrderDetailDTO.IsCloseItem.GetValueOrDefault(false);
            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            ViewBag.ItemGUID_ItemType = ItemGUID + "#" + Objitem.ItemType;
            ViewBag.OrderDetailGUID = OrderDetailGUID.GetValueOrDefault(Guid.Empty);
            return RenderRazorViewToString("_ReturnOrderDetail", objModel);
        }

        public ActionResult ReturnItemDetail(ReceivableItemDTO objDTO)
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

                return PartialView("_ReturnedItemDetail", objDTO);
            }
            finally
            {
                objROTDDAL = null;
                objOrderMasterDAL = null;
                objOrderDTO = null;
            }
        }


        public ActionResult GetOrderFiles(Guid OrderGuid)
        {
            OrderImageDetailDAL orderImageDetailDAL = new OrderImageDetailDAL(SessionHelper.EnterPriseDBName);
            List<OrderImageDetail> listWorkOrderImageDetail = orderImageDetailDAL.GetorderImagesByGuidPlain(OrderGuid).ToList();
            Dictionary<string, Guid> retData = new Dictionary<string, Guid>();

            if (listWorkOrderImageDetail != null && listWorkOrderImageDetail.Any())
            {
                foreach (OrderImageDetail woimg in listWorkOrderImageDetail.Where(e => !string.IsNullOrEmpty(e.ImageName) && !string.IsNullOrWhiteSpace(e.ImageName)))
                {
                    if (!retData.ContainsKey(woimg.ImageName))
                    {
                        retData.Add(woimg.ImageName, woimg.Guid.GetValueOrDefault());
                    }
                }
            }

            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);

        }

        public int DeleteExistingFiles(string FileId, Guid OrderGuid)
        {
            try
            {
                OrderImageDetailDAL objOrderImageDetailDAL = new OrderImageDetailDAL(SessionHelper.EnterPriseDBName);
                objOrderImageDetailDAL.DeleteRecords(FileId, SessionHelper.UserID, OrderGuid);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        public string BackOrderedDetails(string OrderDetailsGUID)
        {
            OrderDetailsDAL orderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            var BackOrderedDetails = orderDetailsDAL.GetReturnItemsDetailsByOrderDetailGuid(OrderDetailsGUID);
            ViewBag.OrderDetailGUID = OrderDetailsGUID;
            return RenderRazorViewToString("_BackOrderDetails", BackOrderedDetails);
        }
    }

    public class BasicDetailForNewReceive
    {
        public string LotNumber { get; set; }
        public string ExpirationDate { get; set; }

        public string Quantity { get; set; }
        public Int64 OrderDetailID { get; set; }
        public string ReceiveDate { get; set; }
        public string Cost { get; set; }
        public string BinName { get; set; }
        public string SerialNumber { get; set; }
        public string PackSlipNumber { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public bool IsOnlyFromUI { get; set; }
    }

}
