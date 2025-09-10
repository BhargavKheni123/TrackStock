using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
//using System.Net.Mail;

namespace eTurnsWeb.Controllers
{
    public partial class ReportBuilderController : eTurnsControllerBase
    {
        private string _ReportAppIntent { get; set; }
        public string ReportAppIntent
        {
            get { return _ReportAppIntent; }
            set
            {
                _ReportAppIntent = value;
            }
        }
        public JsonResult GetModuleItem(string CompanyID, string RoomID, string startDate, string endDate, string StrStatusType, string ModuleName, string StrItemType, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string QuantityType, string ExpiredItem, string ItemExpiredDays, string ItemApproveDays, string ProjectExpirationDate, string CountAppliedFilter, int FilterDateOn = 0, string ItemIsActive = null, string DateCreatedEarlier = null, string DateActiveLater = null, string UsageType = "Consolidate", string AUDayOfUsageToSample = null, string AUMeasureMethod = null, string MinMaxDayOfAverage = null, string MinMaxMinNumberOfTimesMax = null, string StrWOStatusType = null, string SelectedCartType = null, string Includestockedouttools = null, string Days = null, string ItemTypeFilter = null, bool IsAllowZeroPullUsage = false, bool ExcludeZeroOrdQty = false, bool AllCheckedOutTools = false)
        {
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            #region [common paramters]
            string[] sepForRoleRoom = new string[] { "," };
            long[] arrCompanyid = new long[] { };
            long[] arrRoomid = new long[] { };
            string[] arrstrStatusType = new string[] { };
            string[] arrstrWOStatusType = new string[] { };
            string[] arrReqItemType = new string[] { };
            string[] arrItemIsActive = new string[] { };

            if (!string.IsNullOrWhiteSpace(CompanyID))
            {
                arrCompanyid = CompanyID.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries).ToIntArray();
            }
            if (!string.IsNullOrWhiteSpace(RoomID))
            {
                arrRoomid = RoomID.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries).ToIntArray();
            }
            if (!string.IsNullOrWhiteSpace(StrStatusType))
            {
                arrstrStatusType = StrStatusType.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
            }

            if (!string.IsNullOrWhiteSpace(StrWOStatusType))
            {
                arrstrWOStatusType = StrWOStatusType.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
            }

            if (!string.IsNullOrWhiteSpace(StrItemType))
            {
                arrReqItemType = StrItemType.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
            }
            if (!string.IsNullOrWhiteSpace(ItemIsActive))
            {
                arrItemIsActive = ItemIsActive.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
            }

            bool applydatefilter = false;
            if (!string.IsNullOrEmpty(startDate) && Convert.ToDateTime(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).ToString()) != DateTime.MinValue && !string.IsNullOrEmpty(endDate) && Convert.ToDateTime(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).ToString()) != DateTime.MinValue && Convert.ToDateTime(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).ToString()) <= Convert.ToDateTime(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).ToString()))
            {
                applydatefilter = true;
            }
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            _ReportAppIntent = "ReadWrite";
            ReportBuilderDTO objReport = objReportMasterDAL.GetReportDetail(ReportID);
            if (objReport != null && (objReport.ReportAppIntent == "ReadWrite" || objReport.ReportAppIntent == "ReadOnly"))
            {
                _ReportAppIntent = objReport.ReportAppIntent;
            }
            #endregion
            if (arrRoomid.Length > 0)
            {
                switch (ModuleName)
                {
                    case "Consume_Pull":
                        ReportMasterDAL objPullReportDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oPullReportDTO = objPullReportDAL.GetParentReportMasterByReportID(ReportID);
                        if (oPullReportDTO != null)
                        {
                            if (oPullReportDTO.ReportName.ToLower() == "pull summary" || oPullReportDTO.ReportName.ToLower() == "pull summary by quarter")
                            {
                                lstKeyValDTO = getforPullSummarybyquarterReport(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, FilterDateOn, UsageType, ReportModuleName: "Consume_Pull", arrItemIsActive: arrItemIsActive, IsAllowZeroPullUsage: IsAllowZeroPullUsage);
                            }
                            else if (oPullReportDTO.ReportName.ToLower() == "pull"
                                    || oPullReportDTO.ReportName.ToLower() == "pull incomplete"
                                    || oPullReportDTO.ReportName.ToLower() == "pull item summary"
                                    || oPullReportDTO.ReportName.ToLower() == "pull completed"
                                    || oPullReportDTO.ReportName.ToLower() == "pull summary by consignedpo"
                                    || oPullReportDTO.ReportName.ToLower() == "pull no header"
                                    || oPullReportDTO.ReportName.ToLower() == "credit pull"
                                    || oPullReportDTO.ReportName.ToLower() == "cumulative pull"
                                    || oPullReportDTO.ReportName.ToLower() == "total pulled")
                            {
                                lstKeyValDTO = getItemsforPullMainReport(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, FilterDateOn, UsageType, ReportModuleName: "Consume_Pull");
                            }
                            else
                            {
                                lstKeyValDTO = getItemsforPull(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, FilterDateOn, UsageType, ReportModuleName: "Consume_Pull");
                            }
                        }
                        else
                        {
                            lstKeyValDTO = getItemsforPull(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, FilterDateOn, UsageType, ReportModuleName: "Consume_Pull");
                        }
                        break;
                    case "Not Consume_Pull":
                        lstKeyValDTO = getItemsforPull(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, FilterDateOn, "Consolidate", true, ReportModuleName: "Not Consume_Pull");
                        break;
                    case "Item-AuditTrail":
                        lstKeyValDTO = getItemsAuditTrail(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "AuditTrail":
                        lstKeyValDTO = getAuditTrailData(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "ATTSummary":
                        lstKeyValDTO = GetAuditTrailTransactionData(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange);
                        break;
                    case "Item-Instock":
                        lstKeyValDTO = getItemsforInstock(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, Starttime, Endtime);
                        break;
                    case "InStockByActivity":
                        lstKeyValDTO = getItemsforInstockByActivity(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive);
                        break;
                    case "InStockByBin":
                        ReportMasterDAL objReportDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oReportDTO = objReportDAL.GetParentReportMasterByReportID(ReportID);
                        if (oReportDTO != null)
                        {
                            if (oReportDTO.ReportName.ToLower().Equals("item serial lot datcode"))
                            {
                                lstKeyValDTO = getItemsforInStockForSerialLotDateCode(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive, ItemTypeFilter);
                            }
                            else
                            {
                                lstKeyValDTO = getItemsforInstockReport(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, QuantityType: QuantityType, arrItemIsActive: arrItemIsActive);
                            }
                        }
                        else
                            lstKeyValDTO = getItemsforInstockByBin(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive);
                        break;
                    case "InStockByBinMargin":
                        lstKeyValDTO = getItemsforInstockByBin(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive);
                        break;
                    case "InStockWithQOH":
                        lstKeyValDTO = getItemsforInstockWithQOH(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, Starttime, Endtime, arrItemIsActive);
                        break;
                    case "Range-Receive":
                        ReportBuilderDTO oReportBuilderDTO = objReportMasterDAL.GetParentReportMasterByReportID(ReportID);
                        if (oReportBuilderDTO != null)
                        {
                            if (oReportBuilderDTO.ReportName.ToLower().Equals("return item candidates"))
                            {
                                lstKeyValDTO = getReturnItemCandidatesListForRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime, Days);
                            }
                            else if (oReportBuilderDTO.ReportName.ToLower().Equals("received items more than approved"))
                            {
                                lstKeyValDTO = GetReceiveMoreThanApprovedItemForRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                            }
                            else if (oReportBuilderDTO.ReportName.ToLower().Equals("item received receivable"))
                            {
                                lstKeyValDTO = GetItemReceivedReceivableRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                            }
                            else
                            {
                                lstKeyValDTO = GetReceiveLineItemForRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                            }
                        }
                        else
                        {
                            lstKeyValDTO = GetReceiveLineItemForRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        }
                        break;
                    case "Receive":
                        lstKeyValDTO = getReceiveListForRange(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportRange, Starttime, Endtime);
                        break;
                    case "Consume_Requisition":
                        lstKeyValDTO = getRequisitionList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime, arrstrWOStatusType);
                        break;
                    case "Range-Consume_Requisition":
                        lstKeyValDTO = getRequisitionWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, ReportID, ReportRange, Starttime, Endtime, objReport, arrstrWOStatusType);
                        break;
                    case "ReqItemSummary":
                        lstKeyValDTO = getRequisitionItemSummary(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "WOPullSummary":
                        lstKeyValDTO = getItemsforWorkorderPullSummary(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType);
                        break;
                    case "WorkorderList":
                        lstKeyValDTO = GetWorkorderList(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "WorkOrder":
                        ReportMasterDAL objReportMasterWOrderDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oReportBuilderWOrderDTO = objReportMasterWOrderDAL.GetParentReportMasterByReportID(ReportID);
                        if (oReportBuilderWOrderDTO != null)
                        {
                            if (oReportBuilderWOrderDTO.ReportName.ToLower().Equals("work order"))
                            {
                                lstKeyValDTO = GetWorkOrdersWithRange(arrCompanyid, arrRoomid, ReportRange, startDate, endDate, arrstrStatusType, Starttime, Endtime, ReportID, oReportBuilderWOrderDTO, QuantityType, StrItemType);
                            }
                            else
                            {
                                lstKeyValDTO = GetWorkOrdersWithRanage(arrCompanyid, arrRoomid, ReportRange, startDate, endDate, arrstrStatusType, Starttime, Endtime, ReportID, QuantityType, StrItemType);
                            }
                            //else if (oReportBuilderWOrderDTO.ReportName.ToLower().Equals("work order with grouped pulls"))
                            //{
                            //    lstKeyValDTO = GetWorkOrderWithGroupedPullsWithRanage(arrCompanyid, arrRoomid, ReportRange, startDate, endDate, arrstrStatusType, Starttime, Endtime, QuantityType, StrItemType);
                            //}                            
                        }
                        else
                        {
                            lstKeyValDTO = getWorkOrderList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, Starttime, Endtime, QuantityType);
                        }
                        break;
                    case "Order":
                        lstKeyValDTO = getOrderList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Replenish_Order":
                        ReportMasterDAL objReportMasterOrderDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oReportBuilderOrderDTO = objReportMasterOrderDAL.GetParentReportMasterByReportID(ReportID);
                        if (oReportBuilderOrderDTO != null)
                        {
                            if (oReportBuilderOrderDTO.ReportName.ToLower().Equals("order summary lineitem"))
                            {
                                lstKeyValDTO = GetOrderSummaryWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                            }
                            else if (oReportBuilderOrderDTO.ReportName.ToLower().Equals("order item summary"))
                            {
                                lstKeyValDTO = getOrderItemSummaryList(arrCompanyid, arrRoomid, ReportID, ReportRange, startDate, endDate, Starttime, Endtime, FilterDateOn, ExcludeZeroOrdQty);
                            }
                            else
                            {
                                lstKeyValDTO = GetOrderWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                            }
                        }
                        else
                        {
                            lstKeyValDTO = GetOrderWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        }

                        break;
                    case "ReturnOrder":
                        lstKeyValDTO = getReturnOrderList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "ClosedOrder":
                        lstKeyValDTO = getClosedOrder(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "UnfulFilledOrders":
                        lstKeyValDTO = GetUnfulFilledOrdersList(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Item":
                        lstKeyValDTO = getItemsforItemList(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, Starttime, Endtime);
                        break;
                    case "Cart":
                        lstKeyValDTO = getSuggestedOrders(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, SelectedCartType);
                        break;
                    case "Transfer":
                        lstKeyValDTO = GetTransferData(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                        break;
                    case "TransferdItems":
                        lstKeyValDTO = GetTransferWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime, arrReqItemType);
                        break;
                    case "CheckOutTool":
                        lstKeyValDTO = GetCheckoutTools(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, AllCheckedOutTools);
                        break;
                    case "ToolInOutHistory":
                        lstKeyValDTO = GetToolsCheckInoutHistory(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Tool":
                        lstKeyValDTO = GetTools(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "InventoryDailyHistory":
                        lstKeyValDTO = GetInventoryDailyHistoryData(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange);
                        break;
                    case "InventoryReconciliation":
                        lstKeyValDTO = GetInventoryReconciliationData(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange);
                        break;
                    case "InventoryDailyHistoryWithDateRange":
                        lstKeyValDTO = GetInventoryDailyHistoryDataWithDateRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange);
                        break;





                    case "Item-Discrepency":
                        lstKeyValDTO = getItemsDescripencyReport(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, false, Starttime, Endtime);
                        break;

                    case "ProjectSpend":
                        lstKeyValDTO = getProjectSpendList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, Starttime, Endtime);
                        break;
                    case "eVMI":
                        lstKeyValDTO = GeteVMIUsageReport(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "eVMIPollH":
                        lstKeyValDTO = GeteVMIPollHistory(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Assetmaster":
                        lstKeyValDTO = getAssetMasters(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "CountMaster":
                        ReportMasterDAL objReportMasterCountDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oReportBuilderCountDTO = objReportMasterCountDAL.GetParentReportMasterByReportID(ReportID);
                        if (oReportBuilderCountDTO != null)
                        {
                            if (oReportBuilderCountDTO.ReportName.ToLower().Equals("inventory count - customer owned"))
                            {
                                lstKeyValDTO = GetCountMasterForCustAndConsigned(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime, CountAppliedFilter, false, true);
                            }
                            else if (oReportBuilderCountDTO.ReportName.ToLower().Equals("inventory count - consigned"))
                            {
                                lstKeyValDTO = GetCountMasterForCustAndConsigned(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime, CountAppliedFilter, true, false);
                            }
                            else
                            {
                                lstKeyValDTO = GetCountMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime, CountAppliedFilter);
                            }
                        }
                        else
                        {
                            lstKeyValDTO = GetCountMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime, CountAppliedFilter);
                        }
                        break;
                    case "Staging":
                        lstKeyValDTO = GetMaterialStagingMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Room":
                        lstKeyValDTO = GetRoomMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime,arrReqItemType);
                        break;
                    case "User_List":
                        lstKeyValDTO = GetUserMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Consume_PullSummary":
                        lstKeyValDTO = getItemsforPullSummary(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType);
                        break;
                    case "CumulativePull":
                        lstKeyValDTO = getItemsforPull(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, ReportModuleName: "CumulativePull");
                        break;

                    case "SuggOrderOfExpDate":
                        lstKeyValDTO = GetItemsForExpSuggOrders(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;

                    case "Kit":
                        ReportMasterDAL objReportMasterKitDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oReportBuilderKitDTO = objReportMasterKitDAL.GetParentReportMasterByReportID(ReportID);
                        if (oReportBuilderKitDTO != null)
                        {
                            if (oReportBuilderKitDTO.ReportName.ToLower().Equals("kit serial"))
                            {
                                lstKeyValDTO = getKitSerialList(arrCompanyid, arrRoomid, ReportRange, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                            }
                            else if (oReportBuilderKitDTO.ReportName.ToLower().Equals("kit summary"))
                            {
                                lstKeyValDTO = getKitSummaryList(arrCompanyid, arrRoomid, ReportRange, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                            }
                            else if (oReportBuilderKitDTO.ReportName.ToLower().Equals("kit detail"))
                            {
                                lstKeyValDTO = getKitDetailList(arrCompanyid, arrRoomid, ReportRange, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                            }
                            else
                            {
                                lstKeyValDTO = getKitList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                            }
                        }
                        else
                        {
                            lstKeyValDTO = getKitList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                        }
                        break;



                    case "eVMI_ManualCount":
                        lstKeyValDTO = GeteVMIUsageManualCountReport(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "ItemList":
                        lstKeyValDTO = GetItemsList(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime);
                        break;
                    case "ItemsWithSuppliers":
                        lstKeyValDTO = GetItemsWithSuppliers(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime);
                        break;

                    case "ExpiringItems":
                        lstKeyValDTO = getItemsforExpiringItem(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, Starttime, Endtime, ExpiredItem, ItemExpiredDays, ItemApproveDays, ProjectExpirationDate);
                        break;



                    case "ToolMaintananceCost":
                        lstKeyValDTO = GetToolMaiantananceCost(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;

                    case "MaintenanceDue":
                        lstKeyValDTO = GetMaiantananceDue(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "SerialItems":
                        lstKeyValDTO = getItemsforSerialItems(arrCompanyid, arrRoomid, "", "", ReportID, "");
                        break;
                    //case "Maintenance":
                    //    lstKeyValDTO = GetMaintenance(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                    //    break;
                    case "ToolAssetOrder":
                        lstKeyValDTO = getToolAssetOrderList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Tool_ToolAssetOrder":
                        lstKeyValDTO = GetToolAssetOrderWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Stock Out Item":
                        lstKeyValDTO = getItemsforOutstockItem(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive, DateCreatedEarlier, DateActiveLater);
                        break;
                    case "Inventory Stock Out":
                        lstKeyValDTO = getItemsforInventoryStockOut(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive, DateCreatedEarlier, DateActiveLater);
                        break;
                    case "WrittenOffTools":
                        lstKeyValDTO = GetWrittenOffTools(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "ToolAuditTrail":
                        lstKeyValDTO = GetToolAuditTrailData(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "ToolAuditTrailTransaction":
                        lstKeyValDTO = GetToolAuditTrailTransactionData(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "PreciseDemandPlanning":
                        lstKeyValDTO = getItemsforPreciseDemandPlanning(arrCompanyid, arrRoomid, ReportID, ReportRange, QOHFilter, OnlyExirationItems, arrItemIsActive, AUDayOfUsageToSample, AUMeasureMethod, MinMaxDayOfAverage, MinMaxMinNumberOfTimesMax);
                        break;
                    case "PreciseDemandPlanningByItem":
                        lstKeyValDTO = getItemsforPreciseDemandPlanningByItem(arrCompanyid, arrRoomid, ReportID, ReportRange, QOHFilter, OnlyExirationItems, arrItemIsActive, AUDayOfUsageToSample, AUMeasureMethod, MinMaxDayOfAverage, MinMaxMinNumberOfTimesMax);
                        break;

                    case "Supplier":
                        lstKeyValDTO = GetSupplierForReport(arrCompanyid, arrRoomid, startDate, endDate, ReportID, Starttime, Endtime);
                        break;
                    case "ToolInStock":
                        lstKeyValDTO = getToolsforInstock(arrCompanyid, arrRoomid, ReportID, ReportRange, Includestockedouttools);
                        break;
                    case "Quote":
                        lstKeyValDTO = getQuoteList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "MoveMaterial":
                        lstKeyValDTO = getMoveBinTransactionsList(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime, MoveType: StrItemType, arrItemIsActive: arrItemIsActive);
                        break;
                        //
                        //case "Maintenance":
                        //    lstKeyValDTO = GetMaintenance(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        //    break;

                }
            }
            else
            {
                if (arrCompanyid.Length > 0)
                {
                    switch (ModuleName)
                    {
                        case "Company":
                            lstKeyValDTO = GetCompanyMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                            break;
                        default:
                            return Json(new { Status = true, KeyValList = lstKeyValDTO }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (ModuleName == "EnterpriseList")
                    {
                        lstKeyValDTO = GetEnterpriseList(startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                    }
                }

            }
            return Json(new { Status = true, KeyValList = lstKeyValDTO }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetModuleItemForSchedule(string CompanyID, string RoomID, string startDate, string endDate, string StrStatusType, string ModuleName, string StrItemType, Int64 _ReportID, Int64 NotificationID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string QuantityType, string ExpiredItem, string ItemExpiredDays, string ItemApproveDays, string ProjectExpirationDate, string CountAppliedFilter, int FilterDateOn = 0, string UsageType = "Consolidate", string ItemIsActive = null, string SelectedCartType = null, string IsIncludeStockouttool = null, string Days = null, bool IsAllowZeroPullUsage = false, string MoveType = null, bool ExcludeZeroOrdQty = false, bool AllCheckedOutTools = false)
        {
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();

            long ReportID = 0;
            // Int64 ReportMasterID = 0;
            if (NotificationID > 0)
            {
                NotificationDTO oNotificationDTO = new NotificationDAL(SessionHelper.EnterPriseDBName).GetNotifiactionByID(NotificationID);
                if (oNotificationDTO != null && oNotificationDTO.ReportID.HasValue)
                    ReportID = oNotificationDTO.ReportID.Value;
                else
                    ReportID = _ReportID;
            }
            else
            {
                ReportID = _ReportID;
            }


            #region [common paramters]
            string[] sepForRoleRoom = new string[] { "," };
            long[] arrCompanyid = new long[] { };
            long[] arrRoomid = new long[] { };
            string[] arrstrStatusType = new string[] { };
            string[] arrReqItemType = new string[] { };
            string[] arrItemIsActive = new string[] { };

            if (!string.IsNullOrWhiteSpace(CompanyID))
            {
                arrCompanyid = CompanyID.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries).ToIntArray();
            }
            if (!string.IsNullOrWhiteSpace(RoomID))
            {
                arrRoomid = RoomID.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries).ToIntArray();
            }
            if (!string.IsNullOrWhiteSpace(StrStatusType))
            {
                arrstrStatusType = StrStatusType.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
            }

            if (!string.IsNullOrWhiteSpace(StrItemType))
            {
                arrReqItemType = StrItemType.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
            }

            if (!string.IsNullOrWhiteSpace(ItemIsActive))
            {
                arrItemIsActive = ItemIsActive.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
            }


            bool applydatefilter = false;
            if (!string.IsNullOrEmpty(startDate) && Convert.ToDateTime(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).ToString()) != DateTime.MinValue && !string.IsNullOrEmpty(endDate) && Convert.ToDateTime(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).ToString()) != DateTime.MinValue && Convert.ToDateTime(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).ToString()) <= Convert.ToDateTime(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).ToString()))
            {
                applydatefilter = true;
            }
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            _ReportAppIntent = "ReadWrite";
            ReportBuilderDTO objReport = objReportMasterDAL.GetReportDetail(ReportID);
            if (objReport != null && (objReport.ReportAppIntent == "ReadWrite" || objReport.ReportAppIntent == "ReadOnly"))
            {
                _ReportAppIntent = objReport.ReportAppIntent;
            }

            #endregion
            if (arrRoomid.Length > 0)
            {
                switch (ModuleName)
                {
                    case "WorkOrder":
                        lstKeyValDTO = getWorkOrderList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, Starttime, Endtime);
                        break;
                    case "Order":
                        lstKeyValDTO = getOrderList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "ReturnOrder":
                        lstKeyValDTO = getReturnOrderList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Receive":
                        lstKeyValDTO = getReceiveList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, Starttime, Endtime);
                        break;
                    case "Consume_Requisition":
                        lstKeyValDTO = getRequisitionList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                        break;
                    case "Item-Instock":
                        lstKeyValDTO = getItemsforInstock(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, Starttime, Endtime);
                        break;
                    case "InStockByActivity":
                        lstKeyValDTO = getItemsforInstockByActivityForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive);
                        break;
                    case "InStockByBin":
                        ReportMasterDAL objReportDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oReportDTO = objReportDAL.GetParentReportMasterByReportID(ReportID);
                        if (oReportDTO != null)
                        {
                            if (oReportDTO.ReportName.ToLower().Equals("item serial lot datcode"))
                            {
                                lstKeyValDTO = getItemsforInstockByBinForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive);
                            }
                            else
                            {
                                lstKeyValDTO = getforInStockByBinReportForSchedule(arrCompanyid, arrRoomid, "", "", applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter, OnlyExirationItems, "", arrItemIsActive);
                            }
                        }
                        break;
                    case "InStockByBinMargin":
                        lstKeyValDTO = getItemsforInstockByBinForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime, arrItemIsActive);
                        break;
                    case "InStockWithQOH":
                        lstKeyValDTO = getItemsforInstockWithQOHForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, Starttime, Endtime, arrItemIsActive);
                        break;
                    case "Item":
                        lstKeyValDTO = getItemsforItemList(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, Starttime, Endtime);
                        break;
                    case "Consume_Pull":
                        ReportMasterDAL objPullReportDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oPullReportDTO = objPullReportDAL.GetParentReportMasterByReportID(ReportID);
                        if (oPullReportDTO != null)
                        {
                            if (oPullReportDTO.ReportName.ToLower() == "pull summary" || oPullReportDTO.ReportName.ToLower() == "pull summary by quarter")
                            {
                                lstKeyValDTO = getforPullsummaryReportForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, FilterDateOn, UsageType, ReportModuleName: "Consume_Pull", arrItemIsActive: arrItemIsActive, IsAllowZeroPullUsage: IsAllowZeroPullUsage);
                            }
                            else
                            {
                                lstKeyValDTO = getItemsforPullForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, oPullReportDTO.ReportName, FilterDateOn, UsageType, ReportModuleName: "Consume_Pull");
                            }
                        }
                        else
                        {
                            lstKeyValDTO = getItemsforPullForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, string.Empty, FilterDateOn, UsageType, ReportModuleName: "Consume_Pull");
                        }
                        break;
                    case "Not Consume_Pull":
                        lstKeyValDTO = getItemsforPull(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, FilterDateOn, "Consolidate", true, ReportModuleName: "Not Consume_Pull");
                        break;
                    case "Replenish_Order":
                        ReportMasterDAL objPReportOrderMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO OrderReportBuilderDTO = objPReportOrderMasterDAL.GetParentReportMasterByReportID(ReportID);
                        string MainReportName = "";
                        if (OrderReportBuilderDTO != null)
                        {
                            MainReportName = OrderReportBuilderDTO.ReportName.ToLower();
                        }
                        if (!string.IsNullOrWhiteSpace(MainReportName) && MainReportName.Equals("order item summary"))
                        {
                            lstKeyValDTO = getOrderItemSummaryList(arrCompanyid, arrRoomid, ReportID, ReportRange, startDate, endDate, Starttime, Endtime, FilterDateOn, ExcludeZeroOrdQty);
                        }
                        else
                        {
                            lstKeyValDTO = GetOrderWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        }
                        break;
                    case "Range-Receive":
                        ReportMasterDAL objPReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO oPReportBuilderDTO = objPReportMasterDAL.GetParentReportMasterByReportID(ReportID);
                        if (oPReportBuilderDTO != null)
                        {
                            if (oPReportBuilderDTO.ReportName.ToLower().Equals("return item candidates"))
                            {
                                lstKeyValDTO = getReturnItemCandidatesListForRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime, Days);
                            }
                            else if (oPReportBuilderDTO.ReportName.ToLower().Equals("received items more than approved"))
                            {
                                lstKeyValDTO = GetReceiveMoreThanApprovedItemForRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                            }
                            else
                            {
                                lstKeyValDTO = GetReceiveLineItemForRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                            }
                        }
                        else
                        {
                            lstKeyValDTO = GetReceiveLineItemForRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        }
                        break;
                    case "Range-Consume_Requisition":
                        lstKeyValDTO = getRequisitionWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, ReportID, ReportRange, Starttime, Endtime, objReport);
                        break;
                    case "Item-AuditTrail":
                        lstKeyValDTO = getItemsAuditTrail(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "AuditTrail":
                        lstKeyValDTO = getAuditTrailData(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Item-Discrepency":
                        lstKeyValDTO = getItemsDescripencyReport(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, false, Starttime, Endtime);
                        break;
                    case "Cart":
                        lstKeyValDTO = getSuggestedOrders(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, SelectedCartType);
                        break;
                    case "ProjectSpend":
                        lstKeyValDTO = getProjectSpendList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, Starttime, Endtime);
                        break;
                    case "eVMI":
                        lstKeyValDTO = GeteVMIUsageReport(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "eVMIPollH":
                        lstKeyValDTO = GeteVMIPollHistory(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Assetmaster":
                        lstKeyValDTO = getAssetMasters(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "CountMaster":
                        lstKeyValDTO = GetCountMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime, CountAppliedFilter);
                        break;
                    case "Staging":
                        lstKeyValDTO = GetMaterialStagingMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Room":
                        lstKeyValDTO = GetRoomMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime,arrReqItemType);
                        break;
                    case "Consume_PullSummary":
                        lstKeyValDTO = getItemsforPullSummary(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType);
                        break;
                    case "CumulativePull":
                        lstKeyValDTO = getItemsforPull(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType, ReportModuleName: "CumulativePull");
                        break;
                    case "WOPullSummary":
                        lstKeyValDTO = getItemsforWorkorderPullSummary(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QuantityType);
                        break;
                    case "SuggOrderOfExpDate":
                        lstKeyValDTO = GetItemsForExpSuggOrders(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "CheckOutTool":
                        lstKeyValDTO = GetCheckoutTools(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, AllCheckedOutTools);
                        break;
                    case "ToolInOutHistory":
                        lstKeyValDTO = GetToolsCheckInoutHistory(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "Tool":
                        lstKeyValDTO = GetTools(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "Kit":
                        lstKeyValDTO = getKitList(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                        break;
                    case "Transfer":
                        lstKeyValDTO = GetTransferData(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, Starttime, Endtime);
                        break;
                    case "WorkorderList":
                        lstKeyValDTO = getItemsforWorkOrderListForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, ReportModuleName: "WorkorderList");
                        //lstKeyValDTO = GetWorkorderList(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "UnfulFilledOrders":
                        lstKeyValDTO = GetUnfulFilledOrdersList(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "eVMI_ManualCount":
                        lstKeyValDTO = GeteVMIUsageManualCountReport(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "ItemList":
                        lstKeyValDTO = GetItemsList(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime);
                        break;
                    case "ItemsWithSuppliers":
                        lstKeyValDTO = GetItemsWithSuppliers(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, OnlyExirationItems, Starttime, Endtime);
                        break;
                    case "TransferdItems":
                        lstKeyValDTO = GetTransferWithLineItems(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime, arrReqItemType);
                        break;
                    case "ExpiringItems":
                        lstKeyValDTO = getItemsforExpiringItem(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, QOHFilter, Starttime, Endtime, ExpiredItem, ItemExpiredDays, ItemApproveDays, ProjectExpirationDate);
                        break;
                    case "ReqItemSummary":
                        lstKeyValDTO = getRequisitionItemSummary(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, arrReqItemType, applydatefilter, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "InventoryDailyHistory":
                        lstKeyValDTO = GetInventoryDailyHistoryData(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange);
                        break;
                    case "InventoryReconciliation":
                        lstKeyValDTO = GetInventoryReconciliationData(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange);
                        break;
                    case "ClosedOrder":
                        lstKeyValDTO = getClosedOrder(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                        break;
                    case "ToolMaintananceCost":
                        lstKeyValDTO = GetToolMaiantananceCost(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "ATTSummary":
                        lstKeyValDTO = GetAuditTrailTransactionData(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange);
                        break;
                    case "MaintenanceDue":
                        lstKeyValDTO = GetMaiantananceDue(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "SerialItems":
                        lstKeyValDTO = getItemsforSerialItems(arrCompanyid, arrRoomid, "", "", ReportID, "");
                        break;
                    case "WrittenOffTools":
                        lstKeyValDTO = GetWrittenOffTools(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "ToolAuditTrail":
                        lstKeyValDTO = GetToolAuditTrailData(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "ToolAuditTrailTransaction":
                        lstKeyValDTO = GetToolAuditTrailTransactionData(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, QOHFilter);
                        break;
                    case "InventoryDailyHistoryWithDateRange":
                        lstKeyValDTO = GetInventoryDailyHistoryDataWithDateRange(arrCompanyid, arrRoomid, startDate, endDate, ReportID, ReportRange);
                        break;
                    case "Supplier":
                        lstKeyValDTO = GetSupplierForReport(arrCompanyid, arrRoomid, startDate, endDate, ReportID, Starttime, Endtime);
                        break;
                    case "ToolInStock":
                        lstKeyValDTO = getToolsforInstock(arrCompanyid, arrRoomid, ReportID, null, IsIncludeStockouttool);
                        break;
                    case "MoveMaterial":
                        lstKeyValDTO = getMoveBinTransactionsListForSchedule(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, Starttime, Endtime, MoveType: MoveType, arrItemIsActive: arrItemIsActive);
                        break;
                }
            }
            else
            {
                if (arrCompanyid.Length > 0)
                {
                    switch (ModuleName)
                    {
                        case "Company":
                            lstKeyValDTO = GetCompanyMaster(arrCompanyid, arrRoomid, startDate, endDate, arrstrStatusType, ReportID, ReportRange, Starttime, Endtime);
                            break;
                        default:
                            return Json(new { Status = true, KeyValList = lstKeyValDTO }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (ModuleName == "EnterpriseList")
                    {
                        lstKeyValDTO = GetEnterpriseList(startDate, endDate, ReportID, ReportRange, Starttime, Endtime);
                    }
                }

            }
            return Json(new { Status = true, KeyValList = lstKeyValDTO }, JsonRequestBehavior.AllowGet);
        }

        public List<KeyValDTO> GetWorkOrdersWithRanage(long[] arrCompanyid, long[] arrRoomid, string ReportRange, string startDate, string endDate, string[] arrstrStatusType, string Starttime, string Endtime, Int64 ReportID, string QuantityType = "", string WOType = "")
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_WorkOrder> DBWOData = null;
            try
            {
                string fieldName = "WOName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;


                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.GetWorkorderRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, QuantityType, WOType, true, _isRunWithReportConnection: isRunWithReportConnection);

                string _FieldColumnID = string.Empty;
                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                if (objRPTDTO != null)
                {
                    long ReportIDToCheck = ReportID;
                    if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                    {
                        ReportIDToCheck = objRPTDTO.ParentID.GetValueOrDefault(0) > 0 ? objRPTDTO.ParentID.GetValueOrDefault(0) : ReportID;
                    }
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportIDToCheck);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportIDToCheck && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                    else
                    {
                        _FieldColumnID = "GUID";
                    }

                }

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (fieldName == "WOName")
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }
                    else
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                          //  lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        }
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> GetWorkOrdersWithRange(long[] arrCompanyid, long[] arrRoomid, string ReportRange, string startDate, string endDate, string[] arrstrStatusType, string Starttime, string Endtime, Int64 ReportID, ReportBuilderDTO objRPTDTO, string QuantityType = "", string WOType = "")
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            IEnumerable<RPT_WorkOrder> DBWOData = null;
            try
            {
                string fieldName = "WOName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                //ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                //ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                //objRPTDTO = objDAL.GetReportDetail(ReportID);
                //string ReportPath = string.Empty;
                //string Reportname = objRPTDTO.ReportName;
                //string MasterReportname = objRPTDTO.ReportFileName;
                //string SubReportname = objRPTDTO.SubReportFileName;
                //string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                //if (objRPTDTO.ParentID > 0)
                //{
                //    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                //    {
                //        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                //    }
                //    else
                //    {
                //        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                //    }
                //}
                //else
                //{
                //    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                //}

                //XDocument doc = XDocument.Load(ReportPath);
                //string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.GetWorkorderRangeData("RPT_GetWorkOrders", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, QuantityType, WOType, true, _isRunWithReportConnection: isRunWithReportConnection);

                //lsttempKeyValDTO = (from p in DBWOData
                //                    select new KeyValDTO
                //                    {
                //                        key = Convert.ToString(p.GUID),
                //                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                    }).ToList();

                //foreach (var itemOrd in lsttempKeyValDTO)
                //{
                //    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                //    {
                //        lstKeyValDTO.Add(itemOrd);
                //    }
                //    else
                //    {
                //        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                //    }
                //}
                //lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                //return lstKeyValDTO;

                bool AppendRoomName = false;

                if (arrRoomid != null && arrRoomid.Length > 1)
                {
                    AppendRoomName = true;
                }

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;

                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(ParentID);

                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();

                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(objRPTDTO.ID);

                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == objRPTDTO.ID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                        }
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }
        public List<KeyValDTO> GetWorkOrderWithGroupedPullsWithRanage(long[] arrCompanyid, long[] arrRoomid, string ReportRange, string startDate, string endDate, string[] arrstrStatusType, string Starttime, string Endtime, string QuantityType = "", string WOType = "")
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_WorkOrder> DBWOData = null;
            try
            {
                string fieldName = "WOName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.GetWorkorderRangeData("RPT_GetWorkOrderWithSubTotal", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, QuantityType, WOType, true, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }


        private List<KeyValDTO> getWorkOrderList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string Starttime, string Endtime, string QuantityType = "")
        {
            //bool applydatefilter = false;
            //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) != DateTime.MinValue && !(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) != DateTime.MinValue && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) <= Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)))
            //{
            //    applydatefilter = true;
            //}
            WorkOrderDAL objWorkOrderDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
            //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

            //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
            //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
            if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
            {
                if (!string.IsNullOrWhiteSpace(Starttime))
                {
                    string[] Hours_Minutes = Starttime.Split(':');
                    int TotalSeconds = 0;
                    if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                    {
                        TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                        TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                    }
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                }
                else
                {
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                }
            }

            if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
            {
                if (!string.IsNullOrWhiteSpace(Endtime))
                {
                    string[] Hours_Minutes = Endtime.Split(':');
                    int TotalSeconds = 0;
                    if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                    {
                        TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                        TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                    }
                    TotalSeconds += 59;
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                }
                else
                {
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                }
            }

            List<WorkOrderDTO> DBWOData = objWorkOrderDAL.GetAllWorkOrderReport(startDate, endDate, QuantityType);



            lstKeyValDTO = (from p in DBWOData
                            where arrRoomid.Contains((p.Room ?? 0))
                            && arrstrStatusType.Contains(p.WOStatus)
                            // && (applydatefilter ? (Convert.ToDateTime(p.Created.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy HH:mm:ss")) >= Convert.ToDateTime(Convert.ToDateTime(startDate).ToString("MM/dd/yyyy HH:mm:ss"))
                            //&& Convert.ToDateTime(p.Created.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy HH:mm:ss")) <= Convert.ToDateTime(Convert.ToDateTime(endDate).ToString("MM/dd/yyyy HH:mm:ss"))) : true)
                            select new KeyValDTO
                            {
                                key = Convert.ToString(p.GUID),
                                value = p.WOName
                            }).ToList();
            if (lstKeyValDTO != null)
            {
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            return lstKeyValDTO;

        }
        private List<KeyValDTO> getOrderList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            OrderMasterDAL objOrderMasterDAL = null;
            IEnumerable<RPT_OrderMasterDTO> DBWOData = null;
            CommonDAL objCommonDAL = null;
            //eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                if (string.IsNullOrWhiteSpace(ReportRange))
                {
                    ReportRange = "OrderNumber";
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetOrderRangeData("RPT_GetOrders", ReportRange, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetOrderRangeData("RPT_GetOrders", ReportRange, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_OrderMasterDTO>("RPT_GetOrders", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_OrderMasterDTO>("RPT_GetOrders", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                    }

                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_OrderMasterDTO>("RPT_GetOrders", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);

                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (ReportRange ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (ReportRange ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(ReportRange) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(ReportRange).GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(ReportRange).GetValue(p, null)),
                                        }).ToList();
                }
                lstKeyValDTO = (from i in lsttempKeyValDTO
                                group i by i.value into g
                                select new KeyValDTO { value = g.Key, key = string.Join(",", g.Select(kvp => Convert.ToString(kvp.key)).Distinct()) }).ToList();

                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            finally
            {
                lstKeyValDTO = null;
                objOrderMasterDAL = null;
                DBWOData = null;
            }


        }

        private List<KeyValDTO> getQuoteList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            QuoteMasterDAL objQuoteMasterDAL = null;
            IEnumerable<RPT_QuoteMasterDTO> DBWOData = null;
            CommonDAL objCommonDAL = null;
            //eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                objQuoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetQuoteRangeData("RPT_GetQuotes", "QuoteNumber", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetQuoteRangeData("RPT_GetQuotes", "QuoteNumber", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_QuoteMasterDTO>("RPT_GetQuotes", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_QuoteMasterDTO>("RPT_GetQuotes", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                    }

                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_QuoteMasterDTO>("RPT_GetQuotes", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);


                lstKeyValDTO = new List<KeyValDTO>();
                lsttempKeyValDTO = new List<KeyValDTO>();

                foreach (var item in arrRoomid)
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        where (p.RoomID) == item
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = p.QuoteNumber
                                        }).ToList();
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }

                }

                return lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            finally
            {
                lstKeyValDTO = null;
                objQuoteMasterDAL = null;
                DBWOData = null;
            }


        }

        private List<KeyValDTO> getMoveBinTransactionsList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string MoveType = null, string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            IEnumerable<RPT_MoveMaterialDTO> DBWOData = null;
            CommonDAL objCommonDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                DBWOData = objReportMasterDAL.GetMoveBinTransactionsRangeData("RPT_MoveBinTransactions", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, MoveType, arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection);

                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                            else
                            {
                                if (lstKeyValDTO.Where(x => x.key.ToLower().Contains(itemOrd.key.ToLower())).Count() == 0)
                                {
                                    lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                                }
                            }
                        }
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }
                return lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();

            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }

        private List<KeyValDTO> getClosedOrder(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            OrderMasterDAL objOrderMasterDAL = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                bool applydatefilter = false;
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) != DateTime.MinValue && !(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) != DateTime.MinValue && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) <= Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)))
                {
                    applydatefilter = true;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserSupplierDetails(SessionHelper.UserID);
                lstKeyValDTO = new List<KeyValDTO>();
                lsttempKeyValDTO = new List<KeyValDTO>();

                foreach (var item in arrRoomid)
                {
                    if (lstUserRoleModuleDetailsDTO != null && lstUserRoleModuleDetailsDTO.Any() && (lstUserRoleModuleDetailsDTO.Where(x => x.RoomId == item).Count() > 0))
                    {
                        var suppliers = lstUserRoleModuleDetailsDTO.Where(x => x.RoomId == item).FirstOrDefault().ModuleValue;
                        var userSupplierIds = new List<long>();

                        if (!string.IsNullOrEmpty(suppliers) && suppliers.Length > 0)
                        {
                            var tmpSupplierIds = suppliers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            if (tmpSupplierIds != null && tmpSupplierIds.Any())
                            {
                                foreach (var supplier in tmpSupplierIds)
                                {
                                    long tempid = 0;

                                    if (long.TryParse(supplier, out tempid))
                                    {
                                        userSupplierIds.Add(tempid);
                                    }
                                }
                            }
                        }

                        lsttempKeyValDTO = objOrderMasterDAL.GetClosedOrdersForReportByRoomAndSupplier(item, userSupplierIds, applydatefilter, startDate, endDate).ToList();

                        if (lsttempKeyValDTO != null && lsttempKeyValDTO.Any())
                        {
                            lstKeyValDTO.AddRange(lsttempKeyValDTO);
                        }
                    }
                    else
                    {
                        lsttempKeyValDTO = objOrderMasterDAL.GetClosedOrdersForReportByRoom(item, applydatefilter, startDate, endDate).ToList();

                        if (lsttempKeyValDTO != null && lsttempKeyValDTO.Any())
                        {
                            lstKeyValDTO.AddRange(lsttempKeyValDTO);
                        }
                    }
                }
                if (lstKeyValDTO != null)
                {
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }
                return lstKeyValDTO;
            }
            finally
            {
                lstKeyValDTO = null;
                objOrderMasterDAL = null;
            }
        }

        private List<KeyValDTO> getReturnOrderList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            OrderMasterDAL objOrderMasterDAL = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                bool applydatefilter = false;
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) != DateTime.MinValue && !(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) != DateTime.MinValue && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) <= Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)))
                {
                    applydatefilter = true;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }


                objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserSupplierDetails(SessionHelper.UserID);
                lstKeyValDTO = new List<KeyValDTO>();
                lsttempKeyValDTO = new List<KeyValDTO>();

                foreach (var item in arrRoomid)
                {
                    if (lstUserRoleModuleDetailsDTO != null && lstUserRoleModuleDetailsDTO.Any() && (lstUserRoleModuleDetailsDTO.Where(x => x.RoomId == item).Count() > 0))
                    {
                        var suppliers = lstUserRoleModuleDetailsDTO.Where(x => x.RoomId == item).FirstOrDefault().ModuleValue;
                        var userSupplierIds = new List<long>();

                        if (!string.IsNullOrEmpty(suppliers) && suppliers.Length > 0)
                        {
                            var tmpSupplierIds = suppliers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            if (tmpSupplierIds != null && tmpSupplierIds.Any())
                            {
                                foreach (var supplier in tmpSupplierIds)
                                {
                                    long tempid = 0;

                                    if (long.TryParse(supplier, out tempid))
                                    {
                                        userSupplierIds.Add(tempid);
                                    }
                                }
                            }
                        }

                        lsttempKeyValDTO = objOrderMasterDAL.GetReturnOrdersForReportByRoomAndSupplier(item, string.Join(",", arrstrStatusType), userSupplierIds, applydatefilter, startDate, endDate).ToList();

                        if (lsttempKeyValDTO != null && lsttempKeyValDTO.Any())
                        {
                            lstKeyValDTO.AddRange(lsttempKeyValDTO);
                        }

                    }
                    else
                    {
                        lsttempKeyValDTO = objOrderMasterDAL.GetReturnOrdersForReportByRoom(item, string.Join(",", arrstrStatusType), applydatefilter, startDate, endDate).ToList();

                        if (lsttempKeyValDTO != null && lsttempKeyValDTO.Any())
                        {
                            lstKeyValDTO.AddRange(lsttempKeyValDTO);
                        }
                    }
                }

                return lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            finally
            {
                lstKeyValDTO = null;
                objOrderMasterDAL = null;
            }


        }

        private List<KeyValDTO> getReceiveList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_ReceivableItemDTO> DBWOData = null;

            try
            {
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceivableItemDTO>("RPT_GetReceivableItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    where p.ApprovedQuantity > p.ReceivedQuantity
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.OrderDetailGuid),
                                        value = Convert.ToString(p.GetType().GetProperty("ItemNumber").GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }

        private List<KeyValDTO> getReceiveListForRange(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_ReceivableItemDTO> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);


                //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetReceiveRangeData("RPT_GetReceivableItems", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetReceiveRangeData("RPT_GetReceivableItems", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceivableItemDTO>("RPT_GetReceivableItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);


                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceivableItemDTO>("RPT_GetReceivableItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);


                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceivableItemDTO>("RPT_GetReceivableItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);






                lsttempKeyValDTO = (from p in DBWOData
                                    where p.ApprovedQuantity > p.ReceivedQuantity
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.OrderDetailGuid),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }

        private List<KeyValDTO> getReturnItemCandidatesListForRange(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string Days)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_ReceiveDTO> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);


                //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}
                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetReturnItemCandidatesRangeData("RPT_GetReturnItemCandidates", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, Days: Days);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetReturnItemCandidatesRangeData("RPT_GetReturnItemCandidates", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, Days: Days);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceiveDTO>("RPT_GetReturnItemCandidates", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, Days: Days);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceiveDTO>("RPT_GetReturnItemCandidates", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, Days: Days);

                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceiveDTO>("RPT_GetReturnItemCandidates", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, Days: Days);





                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGuid),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getRequisitionList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string[] arrReqItemType, bool applydatefilter, string Starttime, string Endtime, string[] arrstrWOStatusType = null)
        {
            CommonDAL objCommonDAL = null;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            IEnumerable<RPT_Requistions> DBWOData = null;
            try
            {
                RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);


                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetConsumeRequisitionRangeData("RPT_GetRequisitions", "RequisitionNumber", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, arrstrWOStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetConsumeRequisitionRangeData("RPT_GetRequisitions", "RequisitionNumber", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, arrstrWOStatusType, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Requistions>("RPT_GetRequisitions", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, OrderStatus: arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection, WOStatus: arrstrWOStatusType);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Requistions>("RPT_GetRequisitions", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, OrderStatus: arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection, WOStatus: arrstrWOStatusType);

                        }
                    }

                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Requistions>("RPT_GetRequisitions", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, OrderStatus: arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection, WOStatus: arrstrWOStatusType);






                if (DBWOData != null && DBWOData.Count() > 0)
                {
                    if (arrReqItemType != null && arrReqItemType.Length > 0)
                    {
                        DBWOData = DBWOData.Where(x => arrReqItemType.Contains(x.RequisitionType));
                    }
                }

                lstKeyValDTO = (from p in DBWOData
                                select new KeyValDTO
                                {
                                    key = Convert.ToString(p.GUID),
                                    value = p.RequisitionNumber
                                }).ToList();

            }
            catch
            {

            }
            if (lstKeyValDTO != null)
            {
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            return lstKeyValDTO;

        }

        public List<KeyValDTO> getItemsforInstock(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ItemInStokeDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemInStokeDTO>("RPT_InStock", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsforInstockForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ItemInStokeDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemInStokeDTO>("RPT_InStock", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsforExpiringItem(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string Starttime, string Endtime, string ExpiredItem, string ItemExpiredDays, string ItemApproveDays, string ProjectExpirationDate)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ItemExpiringDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }
                if (!(string.IsNullOrEmpty(ProjectExpirationDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(ProjectExpirationDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    ProjectExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ProjectExpirationDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                }


                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemExpiringDTO>("RPT_ExpiredItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, @OnlyExpirationItems: ExpiredItem, @DaysToApproveOrder: ItemExpiredDays, @DaysUntilItemExpires: ItemApproveDays, @ProjectExpirationDate: ProjectExpirationDate, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> getItemsforInstockByBin(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && fieldName.ToLower() == "enterpriseql")
                {
                    lstKeyValDTO = new List<KeyValDTO>();
                    var lstEnterpriseQL = GetEnterpriseQLForReportRangeData(arrRoomid, isRunWithReportConnection);

                    if (lstEnterpriseQL != null && lstEnterpriseQL.Any() && lstEnterpriseQL.Count() > 0)
                    {
                        lstKeyValDTO.AddRange(lstEnterpriseQL);
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    return lstKeyValDTO;
                }
                else
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                        }
                    }

                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                        }
                    }

                    lstKeyValDTO = new List<KeyValDTO>();
                    objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                    string _NewRangeDataFill = "";
                    _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;

                    if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                    {
                        if (_NewRangeDataFill.ToLower() == "all")
                        {
                            DBWOData = objReportMasterDAL.GetInStockByBinRangeData("RPT_InStockByBin", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, QuantityType: "", _isRunWithReportConnection: isRunWithReportConnection);
                        }
                        else
                        {
                            List<string> entList = _NewRangeDataFill.Split(',').ToList();
                            if (entList != null && entList.Count > 0)
                            {
                                string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(isEntAvail))
                                {
                                    DBWOData = objReportMasterDAL.GetInStockByBinRangeData("RPT_InStockByBin", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, QuantityType: "", _isRunWithReportConnection: isRunWithReportConnection);

                                }
                                else
                                {
                                    DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByBin", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                                }
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByBin", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                            }
                        }

                    }
                    else
                        DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByBin", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);

                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ItemGUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();

                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        }
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    return lstKeyValDTO;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> GetEnterpriseQLForReportRangeData(long[] arrRoomid, bool isRunWithReportConnection)
        {
            EnterpriseQuickListDAL enterpriseQuickListDAL = new EnterpriseQuickListDAL(SessionHelper.EnterPriseDBName);
            var enterpriseQLData = enterpriseQuickListDAL.GetEnterpriseQLForReportRangeData(arrRoomid, isRunWithReportConnection);
            List<KeyValDTO> lstEnterpriseQL = new List<KeyValDTO>();

            if (enterpriseQLData != null && enterpriseQLData.Any() && enterpriseQLData.Count() > 0)
            {
                lstEnterpriseQL = (from p in enterpriseQLData
                                   select new KeyValDTO
                                   {
                                       key = p.EnterpriseQLItemGuids,
                                       value = p.QLName,
                                   }).ToList();
            }

            return lstEnterpriseQL;
        }

        public List<KeyValDTO> getItemsforInStockForSerialLotDateCode(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string[] arrItemIsActive = null, string ItemTypeFilter = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InStockForSerialLotDateCodeDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = "";
                _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetInStockForSerialLotDateCodeRangeData("RPT_InStockForSerialLotDateCode", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection, ItemTypeFilter: ItemTypeFilter);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetInStockForSerialLotDateCodeRangeData("RPT_InStockForSerialLotDateCode", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection, ItemTypeFilter: ItemTypeFilter);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InStockForSerialLotDateCodeDTO>("RPT_InStockForSerialLotDateCode", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, ItemTypeFilter: ItemTypeFilter);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InStockForSerialLotDateCodeDTO>("RPT_InStockForSerialLotDateCode", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, ItemTypeFilter: ItemTypeFilter);
                        }
                    }

                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InStockForSerialLotDateCodeDTO>("RPT_InStockForSerialLotDateCode", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, ItemTypeFilter: ItemTypeFilter);


                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();



                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getItemsforOutstockItem(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string[] arrItemIsActive = null, string DateCreatedEarlier = null, string DateActiveLater = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    //if (!string.IsNullOrWhiteSpace(Starttime))
                    //{
                    //    string[] Hours_Minutes = Starttime.Split(':');
                    //    int TotalSeconds = 0;
                    //    if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                    //    {
                    //        TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                    //        TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                    //    }
                    //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    //}
                    //else
                    //{
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    //}
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    //if (!string.IsNullOrWhiteSpace(Endtime))
                    //{
                    //    string[] Hours_Minutes = Endtime.Split(':');
                    //    int TotalSeconds = 0;
                    //    if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                    //    {
                    //        TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                    //        TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                    //    }
                    //    TotalSeconds += 59;
                    //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    //}
                    //else
                    //{
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                    //}
                }

                if (!(string.IsNullOrEmpty(DateCreatedEarlier)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(DateCreatedEarlier), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    //if (!string.IsNullOrWhiteSpace(Endtime))
                    //{
                    //    string[] Hours_Minutes = Endtime.Split(':');
                    //    int TotalSeconds = 0;
                    //    if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                    //    {
                    //        TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                    //        TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                    //    }
                    //    TotalSeconds += 59;
                    //    DateCreatedEarlier = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(DateCreatedEarlier, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    //}
                    //else
                    //{
                    DateCreatedEarlier = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(DateCreatedEarlier, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                    // }
                }

                if (!(string.IsNullOrEmpty(DateActiveLater)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(DateActiveLater), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    //if (!string.IsNullOrWhiteSpace(Endtime))
                    //{
                    //    string[] Hours_Minutes = Endtime.Split(':');
                    //    int TotalSeconds = 0;
                    //    if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                    //    {
                    //        TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                    //        TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                    //    }
                    //    TotalSeconds += 59;
                    //    DateActiveLater = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(DateActiveLater, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    //}
                    //else
                    //{
                    DateActiveLater = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(DateActiveLater, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                    //}
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_OutStockItem", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, DateCreatedEarlier: DateCreatedEarlier, DateActiveLater: DateActiveLater);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getItemsforInventoryStockOut(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string[] arrItemIsActive = null, string DateCreatedEarlier = null, string DateActiveLater = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                }

                if (!(string.IsNullOrEmpty(DateCreatedEarlier)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(DateCreatedEarlier), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    DateCreatedEarlier = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(DateCreatedEarlier, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                }

                if (!(string.IsNullOrEmpty(DateActiveLater)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(DateActiveLater), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    DateActiveLater = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(DateActiveLater, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InventoryStockOut", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, DateCreatedEarlier: DateCreatedEarlier, DateActiveLater: DateActiveLater);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getItemsforInstockByBinForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;


            bool AppendRoomName = false;
            if (arrRoomid != null && arrRoomid.Length > 1)
            {
                AppendRoomName = true;
            }


            //lstSuppliers.ForEach(t =>
            //{
            //    t.SupplierName = ((AppendRoomName) ? (t.SupplierName + "-(" + t.RoomName + ")") : (t.SupplierName));
            //});



            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }


                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByBin", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.ItemRoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                        }).ToList();
                }

                /*
                switch (fieldName)
                {
                    case "ItemNumber":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.ItemGUID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.ItemRoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "SupplierPartNo":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.SupplierID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.ItemRoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "Supplier":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.SupplierID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.ItemRoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "Category":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.CategoryID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.ItemRoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "Bin":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.BinID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.ItemRoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;

                }
                */

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {

                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        //else
                        //{
                        //    lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        //}
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsforInstockByActivity(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && fieldName.ToLower() == "enterpriseql")
                {
                    lstKeyValDTO = new List<KeyValDTO>();
                    var lstEnterpriseQL = GetEnterpriseQLForReportWiseRangeData(arrRoomid, isRunWithReportConnection);

                    if (lstEnterpriseQL != null && lstEnterpriseQL.Any() && lstEnterpriseQL.Count() > 0)
                    {
                        lstKeyValDTO.AddRange(lstEnterpriseQL);
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    return lstKeyValDTO;
                }
                else
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                        }
                    }

                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                        }
                    }

                    lstKeyValDTO = new List<KeyValDTO>();
                    objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                    string _NewRangeDataFill = "";
                    _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;


                    if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                    {
                        if (_NewRangeDataFill.ToLower() == "all")
                        {
                            DBWOData = objReportMasterDAL.GetInStockByActivityRangeData("RPT_InStockByActivity", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                        else
                        {
                            List<string> entList = _NewRangeDataFill.Split(',').ToList();
                            if (entList != null && entList.Count > 0)
                            {
                                string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(isEntAvail))
                                {
                                    DBWOData = objReportMasterDAL.GetInStockByActivityRangeData("RPT_InStockByActivity", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection);

                                }
                                else
                                {
                                    DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByActivity", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                                }
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByActivity", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                            }
                        }

                    }
                    else
                        DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByActivity", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);

                    ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                    ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                    objRPTDTO = objDAL.GetReportDetail(ReportID);
                    List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                    string _FieldColumnID = string.Empty;
                    if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                    {
                        Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                        lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                        if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                        {
                            var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                            if (objField != null)
                            {
                                _FieldColumnID = objField.FieldColumnID;
                            }
                        }
                    }
                    else
                    {
                        lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                        if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                        {
                            var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                            if (objField != null)
                            {
                                _FieldColumnID = objField.FieldColumnID;
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                    {
                        if (fieldName.ToLower() == "supplierpartno")
                        {
                            _FieldColumnID = "SupplierPartNo";
                        }
                        else if (fieldName.ToLower() == "manufacturerpartno")
                        {
                            _FieldColumnID = "ManufacturerPartNo";
                        }
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                                value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            }).ToList();
                    }

                    //lsttempKeyValDTO = (from p in DBWOData
                    //                    select new KeyValDTO
                    //                    {
                    //                        key = Convert.ToString(p.ItemGUID),
                    //                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                    //                    }).ToList();

                    //foreach (var itemOrd in lsttempKeyValDTO)
                    //{
                    //    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    //    {
                    //        lstKeyValDTO.Add(itemOrd);
                    //    }
                    //    else
                    //    {
                    //        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    //    }
                    //}

                    //lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                    {
                        foreach (var itemOrd in lsttempKeyValDTO)
                        {
                            if (!string.IsNullOrWhiteSpace(itemOrd.value))
                            {
                                if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                                {
                                    lstKeyValDTO.Add(itemOrd);
                                }
                                else
                                {
                                    if (lstKeyValDTO.Where(x => x.key.ToLower().Contains(itemOrd.key.ToLower())).Count() == 0)
                                    {
                                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                                    }
                                }
                            }
                        }
                        lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    }
                    return lstKeyValDTO;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsforInstockByActivityForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }


                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                //DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByActivity", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                string _NewRangeDataFill = "";
                _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;


                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetInStockByActivityRangeData("RPT_InStockByActivity", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetInStockByActivityRangeData("RPT_InStockByActivity", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByActivity", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByActivity", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                        }
                    }

                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByActivity", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);


                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    if (fieldName.ToLower() == "supplierpartno")
                    {
                        _FieldColumnID = "SupplierPartNo";
                    }
                    else if (fieldName.ToLower() == "manufacturerpartno")
                    {
                        _FieldColumnID = "ManufacturerPartNo";
                    }

                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }


                /*
                switch (fieldName)
                {
                    case "ItemNumber":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.ItemGUID),
                                                value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            }).ToList();
                        break;
                    case "SupplierPartNo":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.SupplierID),
                                                value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            }).ToList();
                        break;
                }
                */

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {

                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        //else
                        //{
                        //    lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        //}
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsforInstockWithQOH(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string Starttime, string Endtime, string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && fieldName.ToLower() == "enterpriseql")
                {
                    lstKeyValDTO = new List<KeyValDTO>();
                    var lstEnterpriseQL = GetEnterpriseQLForReportRangeData(arrRoomid, isRunWithReportConnection);

                    if (lstEnterpriseQL != null && lstEnterpriseQL.Any() && lstEnterpriseQL.Count() > 0)
                    {
                        lstKeyValDTO.AddRange(lstEnterpriseQL);
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    return lstKeyValDTO;
                }
                else
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                        }
                    }

                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                        }
                    }

                    lstKeyValDTO = new List<KeyValDTO>();
                    objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                    string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;

                    if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                    {
                        if (_NewRangeDataFill.ToLower() == "all")
                        {
                            DBWOData = objReportMasterDAL.GetInStockWithQOHRangeData("RPT_InStockByBinWithQty", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, arrItemIsActive: arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                        else
                        {
                            List<string> entList = _NewRangeDataFill.Split(',').ToList();
                            if (entList != null && entList.Count > 0)
                            {
                                string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(isEntAvail))
                                {
                                    DBWOData = objReportMasterDAL.GetInStockWithQOHRangeData("RPT_InStockByBinWithQty", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, arrItemIsActive: arrItemIsActive, _isRunWithReportConnection: isRunWithReportConnection);

                                }
                                else
                                {
                                    DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByBinWithQty", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                                }
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByBinWithQty", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);
                            }
                        }

                    }
                    else
                        DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByBinWithQty", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive);

                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ItemGUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();

                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        }
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    return lstKeyValDTO;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsforInstockWithQOHForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string Starttime, string Endtime, string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }


                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_InStockByBinWithQty", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, _isRunWithReportConnection: isRunWithReportConnection);


                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {

                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }


                /*
                switch (fieldName)
                {
                    case "ItemNumber":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.ItemGUID),
                                                value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            }).ToList();
                        break;
                    case "SupplierPartNo":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.SupplierID),
                                                value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            }).ToList();
                        break;
                    case "Supplier":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.SupplierID),
                                                value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            }).ToList();
                        break;
                    case "Bin":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.BinID),
                                                value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            }).ToList();
                        break;

                }
                */

                //lsttempKeyValDTO = (from p in DBWOData
                //                    select new KeyValDTO
                //                    {
                //                        key = Convert.ToString(p.ItemGUID),
                //                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                    }).ToList();


                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        //else
                        //{
                        //    lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        //}
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }


        public List<KeyValDTO> getItemsforItemList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            ItemMasterDAL objMasterDAL = null;
            IEnumerable<ItemMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserSupplierDetails(SessionHelper.UserID);
                lstKeyValDTO = new List<KeyValDTO>();
                objMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                DBWOData = objMasterDAL.GetAllItemsRecordsForViewReport(arrCompanyid, arrRoomid, applydatefilter, (startDate), (endDate));
                foreach (var item in arrRoomid)
                {
                    if (lstUserRoleModuleDetailsDTO != null && lstUserRoleModuleDetailsDTO.Any() && (lstUserRoleModuleDetailsDTO.Where(x => x.RoomId == item).Count() > 0))
                    {
                        var suppliers = lstUserRoleModuleDetailsDTO.Where(x => x.RoomId == item).FirstOrDefault().ModuleValue;
                        var userSupplierIds = new List<long>();

                        if (!string.IsNullOrEmpty(suppliers) && suppliers.Length > 0)
                        {
                            var tmpSupplierIds = suppliers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            if (tmpSupplierIds != null && tmpSupplierIds.Any())
                            {
                                foreach (var supplier in tmpSupplierIds)
                                {
                                    long tempid = 0;

                                    if (long.TryParse(supplier, out tempid))
                                    {
                                        userSupplierIds.Add(tempid);
                                    }
                                }
                            }
                        }

                        lsttempKeyValDTO = (from p in DBWOData
                                            where userSupplierIds.Contains((p.SupplierID ?? 0)) && (p.Room ?? 0) == item
                                            orderby p.ItemNumber
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.GUID),
                                                value = p.ItemNumber
                                            }).ToList();

                        foreach (var itemOrd in lsttempKeyValDTO)
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                    }
                    else
                    {
                        lsttempKeyValDTO = (from p in DBWOData
                                            where (p.Room ?? 0) == item
                                            orderby p.ItemNumber
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.GUID),
                                                value = p.ItemNumber
                                            }).ToList();
                        foreach (var itemOrd in lsttempKeyValDTO)
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                    }
                }
                if (lstKeyValDTO != null)
                {
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objMasterDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> getItemsforPull(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QuantityType, int FilterDateOn = 0, string UsageType = "Consolidate", bool isremoceaddsec = false, string ReportModuleName = "")
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            //PullMasterDAL objMasterDAL = null;
            CommonDAL objCommonDAL = null;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<RPT_PullMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (isremoceaddsec)
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                        }
                    }
                }

                if (isremoceaddsec)
                {
                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                        }
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;

                //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    var userSupplierIds = string.Join(",",SessionHelper.UserSupplierIds);

                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetConsumePullRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, userSupplierIds, true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetConsumePullRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, userSupplierIds, true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, reportModuleName: ReportModuleName);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, reportModuleName: ReportModuleName);
                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, reportModuleName: ReportModuleName);



                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                lstKeyValDTO = (from i in lsttempKeyValDTO
                                group i by i.value into g
                                select new KeyValDTO { value = g.Key, key = string.Join(",", g.Select(kvp => Convert.ToString(kvp.key))) }).ToList();

                //foreach (var itemOrd in lsttempKeyValDTO)
                //{
                //    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                //    {
                //        lstKeyValDTO.Add(itemOrd);
                //    }
                //    else
                //    {
                //        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                //    }
                //}
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                //objMasterDAL = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }
        public List<KeyValDTO> getItemsforPullMainReport(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QuantityType, int FilterDateOn = 0, string UsageType = "Consolidate", bool isremoceaddsec = false, string ReportModuleName = "")
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            //PullMasterDAL objMasterDAL = null;
            CommonDAL objCommonDAL = null;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<RPT_PullMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                if (isremoceaddsec)
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                        }
                    }
                }

                if (isremoceaddsec)
                {
                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                        }
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                string _NewRangeDataFill = "";
                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    var userSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);

                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetConsumePullRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, userSupplierIds, true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetConsumePullRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, userSupplierIds, true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, reportModuleName: ReportModuleName);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, reportModuleName: ReportModuleName);
                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, reportModuleName: ReportModuleName);



                //lsttempKeyValDTO = (from p in DBWOData
                //                    select new KeyValDTO
                //                    {
                //                        key = Convert.ToString(p.GUID),
                //                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                    }).ToList();

                //foreach (var itemOrd in lsttempKeyValDTO)
                //{
                //    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                //    {
                //        lstKeyValDTO.Add(itemOrd);
                //    }
                //    else
                //    {
                //        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                //    }
                //}
                //lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                bool AppendRoomName = false;
                if (arrRoomid != null && arrRoomid.Length > 1)
                {
                    AppendRoomName = true;
                }
                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                        }
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                //objMasterDAL = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsforPullForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QuantityType, string ParentReportName, int FilterDateOn = 0, string UsageType = "Consolidate", string ReportModuleName = "")
        {
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> lsttempKeyValDTO = new List<KeyValDTO>();
            //PullMasterDAL objMasterDAL = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_PullMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;



            bool AppendRoomName = false;
            if (arrRoomid != null && arrRoomid.Length > 1)
            {
                AppendRoomName = true;
            }


            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                
                if (!string.IsNullOrEmpty(ParentReportName) && !string.IsNullOrWhiteSpace(ParentReportName) &&  ParentReportName.ToLower() == "total pulled")
                {
                    spName = spName + "_RangeData";
                }
                else
                {
                    spName = spName + "_ForSchedule";
                }
                
                if (!string.IsNullOrEmpty(ParentReportName) && !string.IsNullOrWhiteSpace(ParentReportName) && ParentReportName.ToLower() == "total pulled")
                {
                    string supplierIds = string.Join(",", SessionHelper.UserSupplierIds);
                    DBWOData = objCommonDAL.GetScheduleFilterDataForTotalPulledReport<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, fieldName, SessionHelper.UserID, supplierIds,QuantityType,FilterDateOn,isRunWithReportConnection);
                }
                else
                {
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, reportModuleName: ReportModuleName);
                }

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                        }).ToList();
                }

                /*
                switch (fieldName)
                {
                    case "ItemNumber":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.ItemGUID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "SupplierPartNo":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.SupplierID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "SupplierName":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.SupplierID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();

                        break;
                    case "ManufacturerName":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.ManufacturerID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "ManufacturerNumber":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.ManufacturerID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "CategoryName":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.CategoryID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "PullBin":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.PullBinID),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;
                    case "ConsignedPO":
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = Convert.ToString(p.ConsignedPO),
                                                value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            }).ToList();
                        break;

                }
                */

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                        }
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                //objMasterDAL = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsforPullSummary(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QuantityType)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            //PullMasterDAL objMasterDAL = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_PullMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>("RPT_GetPullSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                //objMasterDAL = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }
        public List<KeyValDTO> getItemsforWorkorderPullSummary(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QuantityType)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            //PullMasterDAL objMasterDAL = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_PullMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetWOPullSummaryRangeData("RPT_GetWorkOrderPullSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, QuantityType, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetWOPullSummaryRangeData("RPT_GetWorkOrderPullSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, QuantityType, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>("RPT_GetWorkOrderPullSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>("RPT_GetWorkOrderPullSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, _isRunWithReportConnection: isRunWithReportConnection);

                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>("RPT_GetWorkOrderPullSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, _isRunWithReportConnection: isRunWithReportConnection);






                //if (!string.IsNullOrEmpty(ReportRange) && (ReportRange.ToLower() == "workorder" || ReportRange.ToLower().Contains("workorderudf")))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.WorkOrderDetailguid),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).ToList();
                //}
                //else
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.ItemGUID),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).ToList();
                //}
                //foreach (var itemOrd in lsttempKeyValDTO)
                //{
                //    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                //    {
                //        lstKeyValDTO.Add(itemOrd);
                //    }
                //    else
                //    {
                //        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                //    }
                //}
                //lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                bool AppendRoomName = false;
                if (arrRoomid != null && arrRoomid.Length > 1)
                {
                    AppendRoomName = true;
                }
                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                        }
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                //objMasterDAL = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> GetOrderWithLineItems(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_OrderWithLineItems> DBWOData = null;

            try
            {
                string fieldName = "OrderNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);


                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                bool isOld = false;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetReplenishOrderRangeData("RPT_GetOrdersWithLineItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, statusType, true, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetReplenishOrderRangeData("RPT_GetOrdersWithLineItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, statusType, true, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                isOld = true;
                            }
                        }
                        else
                        {
                            isOld = true;
                        }
                    }
                }
                else
                {
                    isOld = true;
                }

                if (isOld)
                {
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_OrderWithLineItems>("RPT_GetOrdersWithLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                    List<int> arrOrderStatus = new List<int>();
                    if (statusType != null && statusType.Length > 0)
                    {
                        for (int i = 0; i < statusType.Length; i++)
                        {
                            arrOrderStatus.Add(int.Parse(statusType[i]));
                        }
                    }

                    if (arrOrderStatus.Count > 0)
                    {
                        DBWOData = DBWOData.Where(x => arrOrderStatus.Contains(x.OrdStatus));
                    }
                }

                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                lstKeyValDTO = (from i in lsttempKeyValDTO
                                group i by i.value into g
                                select new KeyValDTO { value = g.Key, key = string.Join(",", g.Select(kvp => Convert.ToString(kvp.key)).Distinct()) }).ToList();
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetOrderSummaryWithLineItems(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_OrderSummaryWithLineItems> DBWOData = null;

            try
            {
                string fieldName = "OrderNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.GetReplenishOrderSummaryRangeData("RPT_GetOrderSummaryLineItem", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, statusType, true, _isRunWithReportConnection: isRunWithReportConnection);

                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!string.IsNullOrEmpty(itemOrd.value))
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                    }
                    //else
                    //{
                    //    lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    //}
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getOrderItemSummaryList(long[] arrCompanyid, long[] arrRoomid, Int64 ReportID, string fieldName, string startDate, string endDate, string Starttime, string Endtime, int FilterDateOn = 0, bool ExcludeZeroOrdQty = false)
        {
            CommonDAL objCommonDAL = null;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> lsttempKeyValDTO = null;
            IEnumerable<RPT_OrderItemSummary> DBWOData = null;
            try
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    fieldName = "ItemNumber";
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.OrderItemSummaryRangeData("RPT_GetOrderItemSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, FilterDateOn, true, _isRunWithReportConnection: isRunWithReportConnection, ExcludeZeroOrdQty: ExcludeZeroOrdQty);

                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ItemGuid),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }
                lstKeyValDTO = (from i in lsttempKeyValDTO
                                group i by i.value into g
                                select new KeyValDTO { value = g.Key, key = string.Join(",", g.Select(kvp => Convert.ToString(kvp.key)).Distinct()) }).ToList();

                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;


                //lsttempKeyValDTO = (from p in DBWOData
                //                select new KeyValDTO
                //                {
                //                    key = Convert.ToString(p.ItemGuid),
                //                    value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                }).ToList();

                //foreach (var itemOrd in lstKeyValDTO)
                //{
                //    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                //    {
                //        lstKeyValDTO.Add(itemOrd);
                //    }
                //    else
                //    {
                //        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                //    }
                //}
                //lstKeyValDTO = lstKeyValDTO.Distinct().ToList();
                //lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                //return lstKeyValDTO;

            }
            catch (Exception ex)
            {

            }

            if (lstKeyValDTO != null)
            {
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            return lstKeyValDTO;

        }

        private List<KeyValDTO> getToolAssetOrderList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            ToolAssetOrderMasterDAL objOrderMasterDAL = null;
            IEnumerable<ToolAssetOrderMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                bool applydatefilter = false;
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) != DateTime.MinValue && !(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) != DateTime.MinValue && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) <= Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)))
                {
                    applydatefilter = true;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                objOrderMasterDAL = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objOrderMasterDAL.GetAllReportRecords().Where(x => x.OrderType == (int)ToolAssetOrderType.Order); ;
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                lstKeyValDTO = new List<KeyValDTO>();
                lsttempKeyValDTO = new List<KeyValDTO>();
                foreach (var item in arrRoomid)
                {


                    lsttempKeyValDTO = (from p in DBWOData
                                        where (p.RoomID ?? 0) == item && arrstrStatusType.Contains(p.OrderStatus.ToString()) && (applydatefilter ? (Convert.ToDateTime(p.Created.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy")) >= Convert.ToDateTime(Convert.ToDateTime(startDate).ToString("MM/dd/yyyy")) && Convert.ToDateTime(p.Created.GetValueOrDefault(DateTime.MinValue).ToString("MM/dd/yyyy")) <= Convert.ToDateTime(Convert.ToDateTime(endDate).ToString("MM/dd/yyyy"))) : true)
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = p.ToolAssetOrderNumber
                                        }).ToList();
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }

                }
                if (lstKeyValDTO != null)
                {
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }
                return lstKeyValDTO;
            }
            finally
            {
                lstKeyValDTO = null;
                objOrderMasterDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetToolAssetOrderWithLineItems(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_ToolAssetOrderWithLineItems> DBWOData = null;

            try
            {
                string fieldName = "ToolAssetOrderNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ToolAssetOrderWithLineItems>("RPT_GetToolAssetOrdersWithLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID);
                List<int> arrOrderStatus = new List<int>();
                if (statusType != null && statusType.Length > 0)
                {
                    for (int i = 0; i < statusType.Length; i++)
                    {
                        arrOrderStatus.Add(int.Parse(statusType[i]));
                    }
                }




                if (!string.IsNullOrEmpty(fieldName) && fieldName == "ItemNumber")
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ToolGUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }
        public List<KeyValDTO> GetMaterialStagingMaster(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_MaterialStagingDTO> DBWOData = null;

            try
            {
                string fieldName = "StagingName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_MaterialStagingDTO>("RPT_GetMaterialStagingMaster", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                List<int> arrOrderStatus = new List<int>();
                if (statusType != null && statusType.Length > 0)
                {
                    for (int i = 0; i < statusType.Length; i++)
                    {
                        arrOrderStatus.Add(int.Parse(statusType[i]));
                    }
                }


                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }
        public List<KeyValDTO> GetCompanyMaster(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_CompanyMasterDTO> DBWOData = null;

            try
            {
                string fieldName = "Name";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }


                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_CompanyMasterDTO>("RPT_GetCompanyList", arrCompanyid, null, startDate, endDate, SessionHelper.UserID, IsRoomIdFilter: false, _isRunWithReportConnection: isRunWithReportConnection);
                List<int> arrOrderStatus = new List<int>();
                if (statusType != null && statusType.Length > 0)
                {
                    for (int i = 0; i < statusType.Length; i++)
                    {
                        arrOrderStatus.Add(int.Parse(statusType[i]));
                    }
                }

                try
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }catch(Exception)
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty("Name").GetValue(p, null)),
                                        }).ToList();
                }
                

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }
        public List<KeyValDTO> GetRoomMaster(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime,string[] RoomTypes )
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_RoomMasterDTO> DBWOData = null;

            try
            {
                string fieldName = "RoomName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_RoomMasterDTO>("RPT_GetRoomList", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                List<int> arrOrderStatus = new List<int>();
                if (statusType != null && statusType.Length > 0)
                {
                    for (int i = 0; i < statusType.Length; i++)
                    {
                        arrOrderStatus.Add(int.Parse(statusType[i]));
                    }
                }

                if (DBWOData != null && DBWOData.Count() > 0)
                {
                    if (RoomTypes != null && RoomTypes.Length > 0)
                    {
                        DBWOData = DBWOData.Where(x => RoomTypes.Contains(Convert.ToString(x.BillingRoomType)));
                    }
                }
                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetUserMaster(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {

            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_UserMasterDTO> DBWOData = null;

            try
            {
                string fieldName = "UserName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL("eTurnsMaster");
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_UserMasterDTO>("RPT_GetEnterpriseUsers", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                List<int> arrOrderStatus = new List<int>();
                if (statusType != null && statusType.Length > 0)
                {
                    for (int i = 0; i < statusType.Length; i++)
                    {
                        arrOrderStatus.Add(int.Parse(statusType[i]));
                    }
                }


                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.UserGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetCountMaster(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string CountAppliedFilter)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_CountMasterDTO> DBWOData = null;

            try
            {
                string fieldName = "CountName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_CountMasterDTO>("RPT_GetCountMaster", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, CountAppliedFilter: CountAppliedFilter, _isRunWithReportConnection: isRunWithReportConnection);
                List<int> arrOrderStatus = new List<int>();
                if (statusType != null && statusType.Length > 0)
                {
                    for (int i = 0; i < statusType.Length; i++)
                    {
                        arrOrderStatus.Add(int.Parse(statusType[i]));
                    }
                }

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetCountMasterForCustAndConsigned(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string CountAppliedFilter, bool IsConsignedReport, bool isCustomerOwnedReport)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_CountMasterDTO> DBWOData = null;

            try
            {
                string fieldName = "CountName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.GetCountMasterForCustAndConsignedRangeData("RPT_GetInventoryCountCustAndConsigned", fieldName, arrCompanyid, arrRoomid, startDate, endDate, CountAppliedFilter, SessionHelper.UserID, true, isRunWithReportConnection, IsConsignedReport, isCustomerOwnedReport);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }

        }


        public List<KeyValDTO> GetReceiveLineItemForRange(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_ReceiveDTO> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);


                //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetRangeReceiveRangeData("RPT_Get_Receive_OrderItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetRangeReceiveRangeData("RPT_Get_Receive_OrderItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceiveDTO>("RPT_Get_Receive_OrderItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceiveDTO>("RPT_Get_Receive_OrderItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ReceiveDTO>("RPT_Get_Receive_OrderItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                lstKeyValDTO = getKeyValueList(ReportID, fieldName, DBWOData);
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetItemReceivedReceivableRange(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lstReceivedKeyValDTO = null;
            List<KeyValDTO> lstReceivableKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_ReceivableItemDTO> DBReceivableData = null;
            IEnumerable<RPT_ReceiveDTO> DBReceivedData = null;
            IEnumerable<RPT_ReceiveDTO> DBWOData = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string[] arrstrStatusType = null;
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                DBReceivableData = objReportMasterDAL.GetReceiveRangeData("RPT_GetReceivableItems", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                DBReceivedData = objReportMasterDAL.GetRangeReceiveRangeData("RPT_Get_Receive_OrderItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                lstReceivableKeyValDTO = (from p in DBReceivableData
                                          select new KeyValDTO
                                          {
                                              key = Convert.ToString(p.ItemGUID),
                                              value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                          }).ToList();

                lstReceivedKeyValDTO = (from p in DBReceivedData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ItemGuid),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();

                foreach (var itemOrd in lstReceivableKeyValDTO.Union(lstReceivedKeyValDTO).Distinct())
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }
        public List<KeyValDTO> GetReceiveMoreThanApprovedItemForRange(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_ReceiveDTO> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.GetRangeReceiveRangeData("RPT_GetReceivedItemsMoreThanApproved", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGuid),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getRequisitionWithLineItems(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string[] arrReqItemType, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, ReportBuilderDTO objRPTDTO, string[] arrstrWOStatusType = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_RequistionWithItemDTO> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                lstKeyValDTO = new List<KeyValDTO>();

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                //ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetRangeConsumeRequisitionRangeData("RPT_GetRequisitionWithLineItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, arrstrWOStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetRangeConsumeRequisitionRangeData("RPT_GetRequisitionWithLineItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, arrstrWOStatusType, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_RequistionWithItemDTO>("RPT_GetRequisitionWithLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection, WOStatus: arrstrWOStatusType);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_RequistionWithItemDTO>("RPT_GetRequisitionWithLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection, WOStatus: arrstrWOStatusType);
                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_RequistionWithItemDTO>("RPT_GetRequisitionWithLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, _isRunWithReportConnection: isRunWithReportConnection, WOStatus: arrstrWOStatusType);

                if (arrReqItemType != null && arrReqItemType.Length > 0)
                {
                    DBWOData = DBWOData.Where(x => arrReqItemType.Contains(x.RequisitionType));
                }

                bool AppendRoomName = false;

                if (arrRoomid != null && arrRoomid.Length > 1)
                {
                    AppendRoomName = true;
                }

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;

                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(ParentID);

                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();

                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(ReportID);

                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                        }
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                //objMasterDAL = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        /// <summary>
        /// Get Audit Trail Transaction Data
        /// </summary>
        /// <param name="arrCompanyid"></param>
        /// <param name="arrRoomid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="applydatefilter"></param>
        /// <param name="ReportID"></param>
        /// <param name="ReportRange"></param>
        /// <returns></returns>
        public List<KeyValDTO> getItemsAuditTrail(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ItemAuditTrailDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {

                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);



                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetItemAuditTrailRangeData("RPT_GetItemAuditTrail_Trans", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetItemAuditTrailRangeData("RPT_GetItemAuditTrail_Trans", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemAuditTrailDTO>("RPT_GetItemAuditTrail_Trans", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemAuditTrailDTO>("RPT_GetItemAuditTrail_Trans", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemAuditTrailDTO>("RPT_GetItemAuditTrail_Trans", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);


                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGuid.GetValueOrDefault(Guid.Empty)),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();



                foreach (var itemOrd in lsttempKeyValDTO)
                {


                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key.Contains(itemOrd.key))
                        {
                            lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        }
                    }


                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        /// <summary>
        /// Get Audit Trails Data
        /// </summary>
        /// <param name="arrCompanyid"></param>
        /// <param name="arrRoomid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="applydatefilter"></param>
        /// <param name="ReportID"></param>
        /// <param name="ReportRange"></param>
        /// <returns></returns>
        public List<KeyValDTO> getAuditTrailData(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ItemAuditTrailDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }


                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);


                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetAuditTrailRangeData("RPT_GetAuditTrail_Data", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetAuditTrailRangeData("RPT_GetAuditTrail_Data", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemAuditTrailDTO>("RPT_GetAuditTrail_Data", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemAuditTrailDTO>("RPT_GetAuditTrail_Data", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ItemAuditTrailDTO>("RPT_GetAuditTrail_Data", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);




                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGuid.GetValueOrDefault(Guid.Empty)),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    //if (fieldName == "ItemNumber")
                    //{
                    //    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    //    {
                    //        lstKeyValDTO.Add(itemOrd);
                    //    }
                    //    else
                    //    {
                    //        if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key.Contains(itemOrd.key))
                    //        {
                    //            lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key += "," + itemOrd.key;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        //lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key.Contains(itemOrd.key))
                        {
                            lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        }
                    }
                    //}
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> getItemsDescripencyReport(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, bool IsFromAlert, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_DicrepencyItem> DBWOData = null;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                DBWOData = objReportMasterDAL.GetDecripencyReportData(arrCompanyid, arrRoomid, startDate, endDate, applydatefilter, ReportID, ReportRange, IsFromAlert);
                lstKeyValDTO = (from itm in DBWOData
                                group itm by new { itm.ItemGUID, itm.ItemNumber } into groupedp
                                select new KeyValDTO
                                {
                                    key = groupedp.Key.ItemGUID.ToString(),
                                    value = groupedp.Key.ItemNumber

                                }).ToList();

                if (lstKeyValDTO != null)
                {
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> getSuggestedOrders(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string SelectedCartType = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_SuggestedOrderDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetCartSuggestedOrderRangeData("RPT_GetCartItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, SelectedCartType: SelectedCartType);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetCartSuggestedOrderRangeData("RPT_GetCartItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, SelectedCartType: SelectedCartType);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_SuggestedOrderDTO>("RPT_GetCartItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, SelectedCartType: SelectedCartType);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_SuggestedOrderDTO>("RPT_GetCartItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, SelectedCartType: SelectedCartType);
                        }
                    }
                }
                else
                {
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_SuggestedOrderDTO>("RPT_GetCartItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, SelectedCartType: SelectedCartType);
                }




                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }
        public List<KeyValDTO> getAssetMasters(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_AssetMasterDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "AssetName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;

                if (spName.ToLower().Equals("rpt_getassetmaster"))
                {
                    objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_AssetMasterDTO>("RPT_GetAssetMaster", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                    lstKeyValDTO = new List<KeyValDTO>();
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList<KeyValDTO>();

                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        }
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    return lstKeyValDTO;
                }
                else
                {
                    IEnumerable<ToolsMaintenanceDTO> DBWODataTools = null;
                    objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                    ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                    //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                    string _NewRangeDataFill = "";
                    //if (Settinfile.Element("NewRangeDataFill") != null)
                    //{
                    //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                    //}

                    if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                    {
                        _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                    }

                    if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                    {
                        if (_NewRangeDataFill.ToLower() == "all")
                        {
                            DBWODataTools = objReportMasterDAL.GetAssetMaintenanceRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                        else
                        {
                            List<string> entList = _NewRangeDataFill.Split(',').ToList();
                            if (entList != null && entList.Count > 0)
                            {
                                string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(isEntAvail))
                                {
                                    DBWODataTools = objReportMasterDAL.GetAssetMaintenanceRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, _isRunWithReportConnection: isRunWithReportConnection);
                                }
                                else
                                {
                                    DBWODataTools = objCommonDAL.GetDataForReportFilterList<ToolsMaintenanceDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, null, FilterDateOn: 0, _isRunWithReportConnection: isRunWithReportConnection);
                                }
                            }
                            else
                            {
                                DBWODataTools = objCommonDAL.GetDataForReportFilterList<ToolsMaintenanceDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, null, FilterDateOn: 0, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                        }
                    }
                    else
                        DBWODataTools = objCommonDAL.GetDataForReportFilterList<ToolsMaintenanceDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, null, FilterDateOn: 0, _isRunWithReportConnection: isRunWithReportConnection);


                    lstKeyValDTO = new List<KeyValDTO>();
                    lsttempKeyValDTO = (from p in DBWODataTools
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList<KeyValDTO>();

                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        //else
                        //{
                        //    lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                        //}
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    return lstKeyValDTO;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        private List<KeyValDTO> getProjectSpendList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string Starttime, string Endtime)
        {
            ProjectMasterDAL objDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();


            //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
            //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

            //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
            //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
            if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
            {
                if (!string.IsNullOrWhiteSpace(Starttime))
                {
                    string[] Hours_Minutes = Starttime.Split(':');
                    int TotalSeconds = 0;
                    if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                    {
                        TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                        TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                    }
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                }
                else
                {
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                }
            }

            if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
            {
                if (!string.IsNullOrWhiteSpace(Endtime))
                {
                    string[] Hours_Minutes = Endtime.Split(':');
                    int TotalSeconds = 0;
                    if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                    {
                        TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                        TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                    }
                    TotalSeconds += 59;
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                }
                else
                {
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                }
            }

            IEnumerable<ProjectMasterDTO> DBData = objDAL.GetProjectsForReport(arrRoomid, arrCompanyid, arrstrStatusType, startDate, endDate);
            lstKeyValDTO = (from p in DBData
                            select new KeyValDTO
                            {
                                key = Convert.ToString(p.GUID),
                                value = p.ProjectSpendName
                            }).ToList();
            if (lstKeyValDTO != null)
            {
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            return lstKeyValDTO;

        }

        public List<KeyValDTO> GeteVMIUsageReport(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_eMVIHistory> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_eMVIHistory>("RPT_GeteVMIUsage", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                //lstKeyValDTO = lsttempKeyValDTO.Select(x => x.value).Distinct().ToList();
                lstKeyValDTO = lsttempKeyValDTO.GroupBy(x => x.value).Select(x => x.First()).ToList();

                lsttempKeyValDTO = (from lst1 in lsttempKeyValDTO
                                    where !lstKeyValDTO.Any(
                                             x => x.key == lst1.key)
                                    select lst1).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GeteVMIUsageManualCountReport(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_eMVIHistory> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_eMVIHistory>("RPT_GeteVMIUsage_ManualCount", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GeteVMIPollHistory(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_eMVIHistory> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }


                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_eMVIHistory>("RPT_GeteVMIPollHistory", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetItemsForExpSuggOrders(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_SuggExpDateOrders> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_SuggExpDateOrders>("RPT_ExpirationSuggestedOrders", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetToolsCheckInoutHistory(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_Tools> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ToolName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetToolInOutHistoryRangeData("RPT_GetCheckOutToolsHistory", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetToolInOutHistoryRangeData("RPT_GetCheckOutToolsHistory", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetCheckOutToolsHistory", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetCheckOutToolsHistory", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                    }
                }
                else
                {
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetCheckOutToolsHistory", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                }





                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ModuleItemGuid),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> GetCheckoutTools(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, bool AllCheckedOutTools = false)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ToolsCheckOut> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ToolName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetCheckOutToolRangeData("RPT_GetCheckOutTools", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, AllCheckedOutTools: AllCheckedOutTools);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetCheckOutToolRangeData("RPT_GetCheckOutTools", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection, AllCheckedOutTools: AllCheckedOutTools);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ToolsCheckOut>("RPT_GetCheckOutTools", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ToolsCheckOut>("RPT_GetCheckOutTools", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                        }
                    }
                }
                else
                {
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ToolsCheckOut>("RPT_GetCheckOutTools", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                }







                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetTools(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string stQtyFilter)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_Tools> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ToolName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetToolsRangeData("RPT_GetTools", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetToolsRangeData("RPT_GetTools", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetTools", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, QOHFilters: stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);


                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetTools", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, QOHFilters: stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);


                        }
                    }
                }
                else
                {
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetTools", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, QOHFilters: stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);


                }






                if (fieldName.Equals("ToolName"))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null) + " - " + p.GetType().GetProperty("Serial").GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetWrittenOffTools(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string stQtyFilter)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_Tools> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ToolName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                string _FieldColumnID = string.Empty;
                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                var objRPTDTO = objDAL.GetReportDetail(ReportID);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetWrittenOffTools", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, QOHFilters: stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }



                if (fieldName.Equals("ToolName"))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null) + " - " + p.GetType().GetProperty("Serial").GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        //lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> GetToolAuditTrailData(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string stQtyFilter)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ToolAuditTrailDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ToolName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ToolAuditTrailDTO>("RPT_GetToolAuditTrail_Data", arrCompanyid, arrRoomid, startDate, endDate, null, QOHFilters: stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);

                if (fieldName.Equals("ToolName"))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ToolGuid),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null) + " - " + p.GetType().GetProperty("SerialNumber").GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ToolGuid),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (fieldName.Equals("ToolName"))
                    {
                        if (!lstKeyValDTO.Exists(x => x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                    }
                    else
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> GetToolAuditTrailTransactionData(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string stQtyFilter)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ToolAuditTrailDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ToolName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ToolAuditTrailDTO>("RPT_GetToolAuditTrail_Trans", arrCompanyid, arrRoomid, startDate, endDate, null, QOHFilters: stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);

                if (fieldName.Equals("ToolName"))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ToolGuid),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null) + " - " + p.GetType().GetProperty("SerialNumber").GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.ToolGuid),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (fieldName.Equals("ToolName"))
                    {
                        if (!lstKeyValDTO.Exists(x => x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                    }
                    else
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getKitList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string[] arrReqItemType, bool applydatefilter, string Starttime, string Endtime)
        {
            CommonDAL objCommonDAL = null;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            IEnumerable<RPT_KitHeader> DBWOData = null;
            try
            {
                RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_KitHeader>("RPT_GetKitHeader", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);


                lstKeyValDTO = (from p in DBWOData
                                select new KeyValDTO
                                {
                                    key = Convert.ToString(p.KitGuid),
                                    value = p.KitPartNumber
                                }).ToList();

            }
            catch
            {

            }

            if (lstKeyValDTO != null)
            {
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            return lstKeyValDTO;

        }

        public List<KeyValDTO> getKitSerialList(long[] arrCompanyid, long[] arrRoomid, string fieldName, string startDate, string endDate, string[] arrstrStatusType, string[] arrReqItemType, bool applydatefilter, string Starttime, string Endtime)
        {
            CommonDAL objCommonDAL = null;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> tmplstKeyValDTO = null;
            IEnumerable<RPT_KitSerialHeader> DBWOData = null;
            try
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    fieldName = "KitPartNumber";
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.GetKitSerialRangeData("RPT_GetKitSerialHeader", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, _isRunWithReportConnection: isRunWithReportConnection);

                tmplstKeyValDTO = (from p in DBWOData
                                   select new KeyValDTO
                                   {
                                       key = Convert.ToString(p.strKitGuid),
                                       value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                   }).ToList();

                foreach (var itemOrd in tmplstKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[tmplstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

                //lstKeyValDTO = (from p in DBWOData
                //                select new KeyValDTO
                //                {
                //                    key = Convert.ToString(p.KitGuid),
                //                    value = p.KitPartNumber
                //                }).ToList();

            }
            catch (Exception ex)
            {

            }

            if (lstKeyValDTO != null)
            {
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            return lstKeyValDTO;

        }

        public List<KeyValDTO> getKitSummaryList(long[] arrCompanyid, long[] arrRoomid, string fieldName, string startDate, string endDate, string[] arrstrStatusType, string[] arrReqItemType, bool applydatefilter, string Starttime, string Endtime)
        {
            CommonDAL objCommonDAL = null;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> tmplstKeyValDTO = null;
            IEnumerable<RPT_KitHeader> DBWOData = null;
            try
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    fieldName = "KitPartNumber";
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.KitSummaryRangeData("RPT_KitSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, _isRunWithReportConnection: isRunWithReportConnection);

                tmplstKeyValDTO = (from p in DBWOData
                                   select new KeyValDTO
                                   {
                                       key = Convert.ToString(p.KitGuid),
                                       value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                   }).ToList();

                foreach (var itemOrd in tmplstKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {

            }

            if (lstKeyValDTO != null)
            {
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            return lstKeyValDTO;

        }

        public List<KeyValDTO> getKitDetailList(long[] arrCompanyid, long[] arrRoomid, string fieldName, string startDate, string endDate, string[] arrstrStatusType, string[] arrReqItemType, bool applydatefilter, string Starttime, string Endtime)
        {
            CommonDAL objCommonDAL = null;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> tmplstKeyValDTO = null;
            IEnumerable<RPT_KitHeader> DBWOData = null;
            try
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    fieldName = "KitPartNumber";
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objReportMasterDAL.KitDetailRangeData("RPT_KitDetail", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, _isRunWithReportConnection: isRunWithReportConnection);

                tmplstKeyValDTO = (from p in DBWOData
                                   select new KeyValDTO
                                   {
                                       key = Convert.ToString(p.KitGuid),
                                       value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                   }).ToList();

                foreach (var itemOrd in tmplstKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {

            }

            if (lstKeyValDTO != null)
            {
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            }
            return lstKeyValDTO;

        }


        public List<KeyValDTO> GetTransferData(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string[] arrReqItemType, bool applydatefilter, string Starttime, string Endtime)
        {
            CommonDAL objCommonDAL = null;
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            IEnumerable<RPT_Transfer> DBWOData = null;
            try
            {
                TransferMasterDAL objRequisitionMasterDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                bool isOLD = false;

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetTransferRangeData("RPT_GetTransfer", "TransferNumber", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, arrReqItemType, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetTransferRangeData("RPT_GetTransfer", "TransferNumber", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, arrstrStatusType, arrReqItemType, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                isOLD = true;
                            }
                        }
                        else
                        {
                            isOLD = true;
                        }
                    }
                }
                else
                    isOLD = true;


                if (isOLD)
                {
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Transfer>("RPT_GetTransfer", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                    if (DBWOData != null && DBWOData.Count() > 0)
                    {
                        if (arrstrStatusType != null && arrstrStatusType.Length > 0)
                        {
                            DBWOData = DBWOData.Where(p => arrstrStatusType.Any(val => p.TransferStatusNumber.ToString().Contains(val)));
                        }

                        if (arrReqItemType != null && arrReqItemType.Length > 0)
                        {
                            DBWOData = DBWOData.Where(x => arrReqItemType.Contains(x.RequestType));
                        }
                    }
                }

                lstKeyValDTO = (from p in DBWOData
                                select new KeyValDTO
                                {
                                    key = Convert.ToString(p.GUID),
                                    value = p.TransferNumber
                                }).ToList();

            }
            catch
            {

            }
            lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
            return lstKeyValDTO;

        }
        public List<KeyValDTO> GetUnfulFilledOrdersList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<UnFulFilledOrderMasterDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {

                string fieldName = "OrderNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetUnFulFilledOrderRangeData("RPT_GetUnFulFilledOrderLineItems", "OrderNumber", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetUnFulFilledOrderRangeData("RPT_GetUnFulFilledOrderLineItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<UnFulFilledOrderMasterDTO>("RPT_GetUnFulFilledOrderLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<UnFulFilledOrderMasterDTO>("RPT_GetUnFulFilledOrderLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                    }

                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<UnFulFilledOrderMasterDTO>("RPT_GetUnFulFilledOrderLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);





                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.OrderGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();



                foreach (var itemOrd in lsttempKeyValDTO)
                {

                    if (fieldName == "OrderNumber")
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }
                    else
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }

                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetWorkorderList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_WorkOrder> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {

                string fieldName = "WOName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetWorkorderListRangeData("RPT_GetWorkOrdersList", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetWorkorderListRangeData("RPT_GetWorkOrdersList", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_WorkOrder>("RPT_GetWorkOrdersList", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_WorkOrder>("RPT_GetWorkOrdersList", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_WorkOrder>("RPT_GetWorkOrdersList", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);



                string _FieldColumnID = string.Empty;
                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                var objRPTDTO = objDAL.GetReportDetail(ReportID);

                if (objRPTDTO != null)
                {
                    long ReportIDToCheck = ReportID;
                    if(objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                    {
                        ReportIDToCheck = objRPTDTO.ParentID.GetValueOrDefault(0) > 0 ? objRPTDTO.ParentID.GetValueOrDefault(0) : ReportID;
                    }
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportIDToCheck);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportIDToCheck && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }

                }

                //if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                //{
                //    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                //    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                //    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                //    {
                //        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                //        if (objField != null)
                //        {
                //            _FieldColumnID = objField.FieldColumnID;
                //        }
                //    }
                //}
                //else
                //{
                //    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                //    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                //    {
                //        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                //        if (objField != null)
                //        {
                //            _FieldColumnID = objField.FieldColumnID;
                //        }
                //    }
                //}


                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();



                foreach (var itemOrd in lsttempKeyValDTO)
                {

                    if (fieldName == "WOName")
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }
                    else
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }

                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetEnterpriseList(string startDate, string endDate, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<EnterpriseDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {

                string fieldName = "Name";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                //string sqlConnectionString = ConfigurationManager.ConnectionStrings["eTurnsMasterDbConnection"].ConnectionString.ToString();
                string sqlConnectionString = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);


                DBWOData = objCommonDAL.GetDataForReportFilterList<EnterpriseDTO>(DbConnectionHelper.GetETurnsMasterDBName() + ".dbo.RPT_GetEnterprises", null, null, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty("Name").GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {

                    if (fieldName == "Name")
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }
                    else
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (!lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key.Contains(itemOrd.key))
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }

                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetItemsList(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_temList> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_temList>("RPT_ItemList", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection);

                lstKeyValDTO = getKeyValueList(ReportID, fieldName, DBWOData);
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetItemsWithSuppliers(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_temList> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_temList>("RPT_GetItemsWithSuppliers", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection);

                lstKeyValDTO = getKeyValueList(ReportID, fieldName, DBWOData);
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetTransferWithLineItems(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string[] RequestType)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_TransferWithLineItemDTO> DBWOData = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                //if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                //if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                //    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetTransferdItemsRangeData("RPT_GetTransferWithLineItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, statusType, RequestType, _isRunWithReportConnection: isRunWithReportConnection);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetTransferdItemsRangeData("RPT_GetTransferWithLineItems", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, statusType, RequestType, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_TransferWithLineItemDTO>("RPT_GetTransferWithLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, "", statusType, RequestType, _isRunWithReportConnection: isRunWithReportConnection);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_TransferWithLineItemDTO>("RPT_GetTransferWithLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, "", statusType, RequestType, _isRunWithReportConnection: isRunWithReportConnection);
                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_TransferWithLineItemDTO>("RPT_GetTransferWithLineItems", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, "", statusType, RequestType, _isRunWithReportConnection: isRunWithReportConnection);

                List<int> arrOrderStatus = new List<int>();
                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> getRequisitionItemSummary(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] arrstrStatusType, string[] arrReqItemType, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_RequistionWithItemDTO> DBWOData = null;

            try
            {
                string fieldName = "RequisitionNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }


                lstKeyValDTO = new List<KeyValDTO>();

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                DBWOData = objReportMasterDAL.GetReqItemSummaryRangeData("RPT_GetRequisitionLineItemSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                //string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}
                //bool isnewRangeData = false;
                //if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                //{
                //    if (_NewRangeDataFill.ToLower() == "all")
                //    {
                //        DBWOData = objReportMasterDAL.GetReqItemSummaryRangeData("RPT_GetRequisitionLineItemSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                //        isnewRangeData = true;
                //    }
                //    else
                //    {
                //        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                //        if (entList != null && entList.Count > 0)
                //        {
                //            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                //            if (!string.IsNullOrWhiteSpace(isEntAvail))
                //            {
                //                DBWOData = objReportMasterDAL.GetReqItemSummaryRangeData("RPT_GetRequisitionLineItemSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);
                //                isnewRangeData = true;
                //            }
                //            else
                //            {
                //                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_RequistionWithItemDTO>("RPT_GetRequisitionLineItemSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);


                //            }
                //        }
                //        else
                //        {
                //            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_RequistionWithItemDTO>("RPT_GetRequisitionLineItemSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);


                //        }
                //    }

                //}
                //else
                //    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_RequistionWithItemDTO>("RPT_GetRequisitionLineItemSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);


                if (arrReqItemType != null && arrReqItemType.Length > 0)
                {
                    DBWOData = DBWOData.Where(x => arrReqItemType.Contains(x.RequisitionType));
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                //if (arrstrStatusType != null && arrstrStatusType.Length > 0)
                //{
                //    DBWOData = DBWOData.Where(x => arrstrStatusType.Contains(x.RequisitionStatus));
                //}

                //lsttempKeyValDTO = (from p in DBWOData
                //                    select new KeyValDTO
                //                    {
                //                        key = Convert.ToString(p.ItemGuid),
                //                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                    }).ToList();

                //if (isnewRangeData)
                //{
                //if (fieldName.ToLower().Equals("requisitionnumber".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseRequisitionGuid),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ItemNumber".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseItemGuid),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ItemSupplier".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseSupplierID),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).ToList();
                //}
                //else if (fieldName.ToLower().Equals("SupplierPartNo".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseSupplierPartNo),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("CategoryName".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseCategoryID),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ManufacturerName".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseManufacturerID),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ManufacturerNumber".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseManufacturerNumber),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("UNSPSC".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseUNSPSC),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ItemUDF1".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseItemUDF1),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ItemUDF2".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseItemUDF2),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ItemUDF3".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseItemUDF3),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ItemUDF4".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseItemUDF4),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else if (fieldName.ToLower().Equals("ItemUDF5".ToLower()))
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.RangeWiseItemUDF5),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}
                //else
                //{
                //    lsttempKeyValDTO = (from p in DBWOData
                //                        select new KeyValDTO
                //                        {
                //                            key = Convert.ToString(p.ItemGuid),
                //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                //                        }).Distinct().ToList();
                //}

                if (lsttempKeyValDTO == null)
                {
                    lsttempKeyValDTO = new List<KeyValDTO>();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                //objMasterDAL = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> GetInventoryDailyHistoryData(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_InventoryDailyHistory> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                    fieldName = ReportRange;

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);


                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);


                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);


                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetInventoryDailyHistoryRangeData("RPT_DailyInverntoryHistory", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetInventoryDailyHistoryRangeData("RPT_DailyInverntoryHistory", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_DailyInverntoryHistory", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_DailyInverntoryHistory", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_DailyInverntoryHistory", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);


                lstKeyValDTO = getKeyValueList(ReportID, fieldName, DBWOData);
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetInventoryReconciliationData(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_InventoryDailyHistory> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                    fieldName = ReportRange;

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);


                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);


                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);


                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetInventoryReconciliationRangeData("RPT_InventoryReconciliation", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetInventoryReconciliationRangeData("RPT_InventoryReconciliation", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_InventoryReconciliation", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);


                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_InventoryReconciliation", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);


                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_InventoryReconciliation", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);




                lstKeyValDTO = getKeyValueList(ReportID, fieldName, DBWOData);
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetToolMaiantananceCost(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string stQtyFilter)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_ToolMaintanance> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ToolName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ToolMaintanance>("RPT_GetToolMaintanceWithCost", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, QOHFilters: stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);
                if (fieldName.Equals("ToolName"))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null) + " - " + p.GetType().GetProperty("Serial").GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetMaiantananceDue(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string stQtyFilter)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_MaintananceDue> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "Maintenance";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_MaintananceDue>("RPT_GetMaintanceDue", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, QOHFilters: stQtyFilter, _isRunWithReportConnection: isRunWithReportConnection);
                DBWOData = DBWOData.Where(x => x.MaintenanceStatus.ToLower() == "open").ToList();
                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = !string.IsNullOrEmpty(p.AssetName)
                                                    ? Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + " - Asset"
                                                    : Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + " - Tool",
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }


        public List<KeyValDTO> GetAuditTrailTransactionData(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_InventoryDailyHistory> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                    fieldName = ReportRange;

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);


                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetATTSummaryRangeData("RPT_AuditTrailTransactionSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetATTSummaryRangeData("RPT_AuditTrailTransactionSummary", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_AuditTrailTransactionSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_AuditTrailTransactionSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                        }
                    }

                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_AuditTrailTransactionSummary", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);


                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                lstKeyValDTO = (from i in lsttempKeyValDTO
                                group i by i.value into g
                                select new KeyValDTO { value = g.Key, key = string.Join(",", g.Select(kvp => Convert.ToString(kvp.key)).Distinct()) }).ToList();

                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> getItemsforSerialItems(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_SerialNumberList> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_SerialNumberList>("RPT_SerialNumberList", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).GroupBy(n => new { n.key, n.value }).Select(g => g.FirstOrDefault()).ToList();


                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> getItemsforPreciseDemandPlanning(long[] arrCompanyid, long[] arrRoomid, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string[] arrItemIsActive = null, string AUDayOfUsageToSample = null, string AUMeasureMethod = null, string MinMaxDayOfAverage = null, string MinMaxMinNumberOfTimesMax = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                int DecimalPointFromConfig = 0;
                //if (Settinfile.Element("decimalPointFromConfig") != null)
                //{
                //    DecimalPointFromConfig = Convert.ToInt32(Settinfile.Element("decimalPointFromConfig").Value);
                //}
                if (SiteSettingHelper.decimalPointFromConfig != string.Empty)
                {
                    DecimalPointFromConfig = Convert.ToInt32(SiteSettingHelper.decimalPointFromConfig);
                }


                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_PreciseDemandPlanning", arrCompanyid, arrRoomid, null, null, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, AUDayOfUsageToSample: AUDayOfUsageToSample, AUMeasureMethod: AUMeasureMethod, MinMaxDayOfAverage: MinMaxDayOfAverage, MinMaxMinNumberOfTimesMax: MinMaxMinNumberOfTimesMax, DecimalPointFromConfig: DecimalPointFromConfig);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getItemsforPreciseDemandPlanningByItem(long[] arrCompanyid, long[] arrRoomid, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string[] arrItemIsActive = null, string MinMaxDayOfUsageToSample = null, string MinMaxMeasureMethod = null, string MinMaxDayOfAverage = null, string MinMaxMinNumberOfTimesMax = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                int DecimalPointFromConfig = 0;

                //if (Settinfile.Element("decimalPointFromConfig") != null)
                //{
                //    DecimalPointFromConfig = Convert.ToInt32(Settinfile.Element("decimalPointFromConfig").Value);
                //}
                if (SiteSettingHelper.decimalPointFromConfig != string.Empty)
                {
                    DecimalPointFromConfig = Convert.ToInt32(SiteSettingHelper.decimalPointFromConfig);
                }

                DBWOData = objCommonDAL.GetDataForReportFilterList<RRT_InstockByBinDTO>("RPT_PreciseDemandPlanningByItem", arrCompanyid, arrRoomid, null, null, SessionHelper.UserID, null, QOHFilter, OnlyExirationItems, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, AUDayOfUsageToSample: MinMaxDayOfUsageToSample, AUMeasureMethod: MinMaxMeasureMethod, MinMaxDayOfAverage: MinMaxDayOfAverage, MinMaxMinNumberOfTimesMax: MinMaxMinNumberOfTimesMax, DecimalPointFromConfig: DecimalPointFromConfig);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemNumber),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> GetInventoryDailyHistoryDataWithDateRange(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string ReportRange)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_InventoryDailyHistory> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                    fieldName = ReportRange;

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                    startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
                    endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);


                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);


                string _NewRangeDataFill = SiteSettingHelper.NewRangeDataFill;
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetInventoryDailyHistoryDataWithDateRangeRangeData("RPT_DailyInverntoryHistoryWithDateRange", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetInventoryDailyHistoryDataWithDateRangeRangeData("RPT_DailyInverntoryHistoryWithDateRange", fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_DailyInverntoryHistoryWithDateRange", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_DailyInverntoryHistoryWithDateRange", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_DailyInverntoryHistoryWithDateRange", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);




                //DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_DailyInverntoryHistoryWithDateRange", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.ItemGUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                lsttempKeyValDTO = (from i in lsttempKeyValDTO
                                    group i by i.value into g
                                    select new KeyValDTO { value = g.Key, key = string.Join(",", g.Select(kvp => Convert.ToString(kvp.key)).Distinct()) }).ToList();

                //foreach (var itemOrd in lsttempKeyValDTO)
                //{
                //    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                //        lstKeyValDTO.Add(itemOrd);
                //    else
                //        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                //}

                lstKeyValDTO = lsttempKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        public List<KeyValDTO> GetSupplierForReport(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, Int64 ReportID, string Starttime, string Endtime)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_InventoryDailyHistory> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            try
            {
                string fieldName = "SupplierName";
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }

                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_InventoryDailyHistory>("RPT_SupplierList", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, _isRunWithReportConnection: isRunWithReportConnection);

                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        lstKeyValDTO.Add(itemOrd);
                    else
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                }

                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }


        }

        #region WI-5426 -Tool Instock Report

        public List<KeyValDTO> getToolsforInstock(long[] arrCompanyid, long[] arrRoomid, Int64 ReportID, string ReportRange, string Includestockedouttools)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RPT_Tools> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            try
            {
                string fieldName = "ToolName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                lstKeyValDTO = new List<KeyValDTO>();
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                DBWOData = objReportMasterDAL.getToolsforInstockRangeData("RPT_GetToolInstock", fieldName, arrCompanyid, arrRoomid, Includestockedouttools, _isRunWithReportConnection: isRunWithReportConnection);

                //string _NewRangeDataFill = "";
                //if (Settinfile.Element("NewRangeDataFill") != null)
                //{
                //    _NewRangeDataFill = Convert.ToString(Settinfile.Element("NewRangeDataFill").Value);
                //}

                //if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                //{
                //    if (_NewRangeDataFill.ToLower() == "all")
                //    {
                //        DBWOData = objReportMasterDAL.getToolsforInstockRangeData("RPT_GetToolInstock", fieldName, arrCompanyid, arrRoomid, Includestockedouttools, _isRunWithReportConnection: isRunWithReportConnection);
                //    }
                //    else
                //    {
                //        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                //        if (entList != null && entList.Count > 0)
                //        {
                //            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                //            if (!string.IsNullOrWhiteSpace(isEntAvail))
                //            {
                //                DBWOData = objReportMasterDAL.getToolsforInstockRangeData("RPT_GetToolInstock", fieldName, arrCompanyid, arrRoomid, Includestockedouttools, _isRunWithReportConnection: isRunWithReportConnection);
                //            }
                //            else
                //            {
                //                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetToolInstock", arrCompanyid, arrRoomid, null, null, SessionHelper.UserID, null, null, null, _isRunWithReportConnection: isRunWithReportConnection, Includestockedouttools: Includestockedouttools);
                //            }
                //        }
                //        else
                //        {
                //            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetToolInstock", arrCompanyid, arrRoomid, null, null, SessionHelper.UserID, null, null, null, _isRunWithReportConnection: isRunWithReportConnection, Includestockedouttools: Includestockedouttools);
                //        }
                //    }

                //}
                //else
                //    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_Tools>("RPT_GetToolInstock", arrCompanyid, arrRoomid, null, null, SessionHelper.UserID, null, null, null, _isRunWithReportConnection: isRunWithReportConnection, Includestockedouttools: Includestockedouttools);


                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }
                else
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = Convert.ToString(p.GUID),
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                        }).ToList();
                }

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                DBWOData = null;
            }
        }

        #endregion

        public List<KeyValDTO> getItemsforWorkOrderListForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string ReportModuleName = "")
        {
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> lsttempKeyValDTO = new List<KeyValDTO>();
            CommonDAL objCommonDAL = null;
            List<RPT_WorkOrder> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            bool AppendRoomName = false;
            if (arrRoomid != null && arrRoomid.Length > 1)
            {
                AppendRoomName = true;
            }
            try
            {
                string fieldName = "WOName";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                // spName = spName + "_ForSchedule";
                DBWOData = objDAL.GetWorkorderListRangeDataForSchedule(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, _isRunWithReportConnection: isRunWithReportConnection);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            //value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),

                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                        }
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        #region 6453 - Pull Summary Report - allow to select INactive Items
        public List<KeyValDTO> getforPullSummaryReport(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter,
            Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QuantityType, int FilterDateOn = 0,
            string UsageType = "Consolidate", bool isremoceaddsec = false, string ReportModuleName = "", string[] arrItemIsActive = null
            , bool IsAllowZeroPullUsage = false)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<RPT_PullMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                if (isremoceaddsec)
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                        }
                    }
                }

                if (isremoceaddsec)
                {
                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                        }
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;

                string _NewRangeDataFill = "";
                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetConsumePullSummaryRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, arrItemIsActive: arrItemIsActive, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetConsumePullSummaryRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, arrItemIsActive: arrItemIsActive, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, _usageType: UsageType, reportModuleName: ReportModuleName, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, _usageType: UsageType, reportModuleName: ReportModuleName, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);
                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, _usageType: UsageType, reportModuleName: ReportModuleName, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);



                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = Convert.ToString(p.GUID),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();

                foreach (var itemOrd in lsttempKeyValDTO)
                {
                    if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                    {
                        lstKeyValDTO.Add(itemOrd);
                    }
                    else
                    {
                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                    }
                }
                lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getforPullSummarybyquarterReport(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter,
                    Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QuantityType, int FilterDateOn = 0,
                    string UsageType = "Consolidate", bool isremoceaddsec = false, string ReportModuleName = "", string[] arrItemIsActive = null
                    , bool IsAllowZeroPullUsage = false)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<RPT_PullMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }
                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                if (isremoceaddsec)
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                        }
                    }
                }

                if (isremoceaddsec)
                {
                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString();
                        }
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                        }
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;

                string _NewRangeDataFill = "";
                if (SiteSettingHelper.NewRangeDataFill != string.Empty)
                {
                    _NewRangeDataFill = Convert.ToString(SiteSettingHelper.NewRangeDataFill);
                }

                if (!string.IsNullOrWhiteSpace(_NewRangeDataFill ?? string.Empty))
                {
                    if (_NewRangeDataFill.ToLower() == "all")
                    {
                        DBWOData = objReportMasterDAL.GetConsumePullSummaryRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, arrItemIsActive: arrItemIsActive, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);
                    }
                    else
                    {
                        List<string> entList = _NewRangeDataFill.Split(',').ToList();
                        if (entList != null && entList.Count > 0)
                        {
                            string isEntAvail = entList.Where(x => x == SessionHelper.EnterPriceID.ToString()).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(isEntAvail))
                            {
                                DBWOData = objReportMasterDAL.GetConsumePullSummaryRangeData(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, _usageType: UsageType, arrItemIsActive: arrItemIsActive, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);

                            }
                            else
                            {
                                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, _usageType: UsageType, reportModuleName: ReportModuleName, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);
                            }
                        }
                        else
                        {
                            DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, _usageType: UsageType, reportModuleName: ReportModuleName, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);
                        }
                    }
                }
                else
                    DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, _usageType: UsageType, reportModuleName: ReportModuleName, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);


                bool AppendRoomName = false;
                if (arrRoomid != null && arrRoomid.Length > 1)
                {
                    AppendRoomName = true;
                }
                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                        }
                    }

                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }
        public List<KeyValDTO> getforPullsummaryReportForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate,
            bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QuantityType, int FilterDateOn = 0,
            string UsageType = "Consolidate", string ReportModuleName = "", string[] arrItemIsActive = null, bool IsAllowZeroPullUsage = false)
        {
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> lsttempKeyValDTO = new List<KeyValDTO>();
            //PullMasterDAL objMasterDAL = null;
            CommonDAL objCommonDAL = null;
            IEnumerable<RPT_PullMasterDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;

            bool AppendRoomName = false;
            if (arrRoomid != null && arrRoomid.Length > 1)
            {
                AppendRoomName = true;
            }

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                spName = spName + "_ForSchedule";
                DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_PullMasterDTO>(spName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, null, "", "", true, QuantityType, FilterDateOn: FilterDateOn, _isRunWithReportConnection: isRunWithReportConnection, arrItemIsActive: arrItemIsActive, _usageType: UsageType, reportModuleName: ReportModuleName, IsAllowedZeroPullUsage: IsAllowZeroPullUsage);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (spName.ToLower().Equals("RPT_GetPullSummaryByQuarter".ToLower()) || spName.ToLower().Equals("RPT_GetPullSummaryByQuarter_ForSchedule".ToLower()))
                {
                    DBWOData.ToList().ForEach(x => x.ItemGUID = x.GUID);
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)))),//
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!string.IsNullOrWhiteSpace(itemOrd.value))
                        {
                            if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower() && x.key.ToLower() == itemOrd.key.ToLower()))
                            {
                                lstKeyValDTO.Add(itemOrd);
                            }
                        }
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }
                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        #endregion

        #region #6582. Instock time out related changes
        public List<KeyValDTO> getforInStockByBinReportForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string QOHFilter, string OnlyExirationItems, string ReportModuleName = "", string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> lsttempKeyValDTO = new List<KeyValDTO>();
            CommonDAL objCommonDAL = null;
            List<RRT_InstockByBinDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            bool AppendRoomName = false;
            if (arrRoomid != null && arrRoomid.Length > 1)
            {
                AppendRoomName = true;
            }
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                DBWOData = objDAL.GetInStockByBinRangeDataForSchedule(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, null, QOHFilter, OnlyExirationItems, SessionHelper.UserID, arrItemIsActive, true, _isRunWithReportConnection: isRunWithReportConnection);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {
                    if (fieldName.ToLower() == "supplierpartno")
                    {
                        _FieldColumnID = "SupplierPartNo";
                    }
                    else if (fieldName.ToLower() == "manufacturerpartno")
                    {
                        _FieldColumnID = "ManufacturerPartNo";
                    }

                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            //value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.ItemRoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null))))
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (lstKeyValDTO.Where(x => x.key.ToLower().Contains(itemOrd.key.ToLower())).Count() == 0)
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        public List<KeyValDTO> getItemsforInstockReport(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string QOHFilter, string OnlyExirationItems, string Starttime, string Endtime, string QuantityType = "", string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = null;
            IEnumerable<RRT_InstockByBinDTO> DBWOData = null;
            List<KeyValDTO> lsttempKeyValDTO = null;
            CommonDAL objCommonDAL = null;

            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && fieldName.ToLower() == "enterpriseql")
                {
                    lstKeyValDTO = new List<KeyValDTO>();
                    var lstEnterpriseQL = GetEnterpriseQLForReportWiseRangeData(arrRoomid, isRunWithReportConnection);

                    if (lstEnterpriseQL != null && lstEnterpriseQL.Any() && lstEnterpriseQL.Count() > 0)
                    {
                        lstKeyValDTO.AddRange(lstEnterpriseQL);
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    return lstKeyValDTO;
                }
                else
                {
                    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Starttime))
                        {
                            string[] Hours_Minutes = Starttime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                        }
                    }

                    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Endtime))
                        {
                            string[] Hours_Minutes = Endtime.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                        }
                        else
                        {
                            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                        }
                    }

                    lstKeyValDTO = new List<KeyValDTO>();
                    objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                    DBWOData = objReportMasterDAL.GetInStockByBinRangeData("RPT_InStockByBinReport", fieldName, arrCompanyid, arrRoomid, SessionHelper.UserID, QOHFilter, OnlyExirationItems, arrItemIsActive: arrItemIsActive, QuantityType: QuantityType, _isRunWithReportConnection: isRunWithReportConnection);

                    ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                    ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                    objRPTDTO = objDAL.GetReportDetail(ReportID);
                    List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                    string _FieldColumnID = string.Empty;
                    if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                    {
                        Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                        lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                        if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                        {
                            var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                            if (objField != null)
                            {
                                _FieldColumnID = objField.FieldColumnID;
                            }
                        }
                    }
                    else
                    {
                        lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                        if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                        {
                            var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                            if (objField != null)
                            {
                                _FieldColumnID = objField.FieldColumnID;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                    {
                        if (fieldName.ToLower() == "supplierpartno")
                        {
                            _FieldColumnID = "SupplierPartNo";
                        }
                        else if (fieldName.ToLower() == "manufacturerpartno")
                        {
                            _FieldColumnID = "ManufacturerPartNo";
                        }
                        lsttempKeyValDTO = (from p in DBWOData
                                            select new KeyValDTO
                                            {
                                                key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                                value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                            }).ToList();
                    }

                    if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                    {
                        foreach (var itemOrd in lsttempKeyValDTO)
                        {
                            if (!string.IsNullOrWhiteSpace(itemOrd.value))
                            {
                                if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                                {
                                    lstKeyValDTO.Add(itemOrd);
                                }
                                else
                                {
                                    if (lstKeyValDTO.Where(x => x.key.ToLower().Contains(itemOrd.key.ToLower())).Count() == 0)
                                    {
                                        lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                                    }
                                }
                            }
                        }
                        lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                    }

                    return lstKeyValDTO;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }


        public List<KeyValDTO> GetEnterpriseQLForReportWiseRangeData(long[] arrRoomid, bool isRunWithReportConnection)
        {
            EnterpriseQuickListDAL enterpriseQuickListDAL = new EnterpriseQuickListDAL(SessionHelper.EnterPriseDBName);
            var enterpriseQLData = enterpriseQuickListDAL.GetEnterpriseQLForReportWiseRangeData(arrRoomid, isRunWithReportConnection);
            List<KeyValDTO> lstEnterpriseQL = new List<KeyValDTO>();

            if (enterpriseQLData != null && enterpriseQLData.Any() && enterpriseQLData.Count() > 0)
            {
                lstEnterpriseQL = (from p in enterpriseQLData
                                   select new KeyValDTO
                                   {
                                       key = p.EnterpriseQLItemGuids,
                                       value = p.QLName,
                                   }).ToList();
            }

            return lstEnterpriseQL;
        }

        #endregion

        public List<KeyValDTO> getMoveBinTransactionsListForSchedule(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, bool applydatefilter, Int64 ReportID, string ReportRange, string Starttime, string Endtime, string MoveType = "", string ReportModuleName = "", string[] arrItemIsActive = null)
        {
            List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> lsttempKeyValDTO = new List<KeyValDTO>();
            CommonDAL objCommonDAL = null;
            List<RPT_MoveMaterialDTO> DBWOData = null;
            eTurns.DAL.UserMasterDAL objinterUserDAL = null;
            bool AppendRoomName = false;
            if (arrRoomid != null && arrRoomid.Length > 1)
            {
                AppendRoomName = true;
            }
            try
            {
                string fieldName = "ItemNumber";
                if (!string.IsNullOrEmpty(ReportRange))
                {
                    fieldName = ReportRange;
                }

                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstKeyValDTO = new List<KeyValDTO>();
                objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Starttime))
                    {
                        string[] Hours_Minutes = Starttime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
                    }
                }

                if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomCulture)) > DateTime.MinValue)
                {
                    if (!string.IsNullOrWhiteSpace(Endtime))
                    {
                        string[] Hours_Minutes = Endtime.Split(':');
                        int TotalSeconds = 0;
                        if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                        {
                            TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                            TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                        }
                        TotalSeconds += 59;
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                    }
                    else
                    {
                        endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
                    }
                }
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);
                string ReportPath = string.Empty;
                string Reportname = objRPTDTO.ReportName;
                string MasterReportname = objRPTDTO.ReportFileName;
                string SubReportname = objRPTDTO.SubReportFileName;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objRPTDTO.ParentID > 0)
                {
                    if (objRPTDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname;
                    }
                    else
                    {
                        ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname;
                    }
                }
                else
                {
                    ReportPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname;
                }

                XDocument doc = XDocument.Load(ReportPath);
                string spName = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                DBWOData = objDAL.GetMoveBinTransactionsRangeDataForSchedule(spName, fieldName, arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID, MoveType, arrItemIsActive, true, _isRunWithReportConnection: isRunWithReportConnection);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
                {

                    lsttempKeyValDTO = (from p in DBWOData
                                        select new KeyValDTO
                                        {
                                            key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                            value = ((AppendRoomName) ? (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)) + "-(" + p.RoomName + ")") : (Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null))))
                                        }).ToList();
                }

                if (lsttempKeyValDTO != null && lsttempKeyValDTO.Count > 0)
                {
                    foreach (var itemOrd in lsttempKeyValDTO)
                    {
                        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
                        {
                            lstKeyValDTO.Add(itemOrd);
                        }
                        else
                        {
                            if (lstKeyValDTO.Where(x => x.key.ToLower().Contains(itemOrd.key.ToLower())).Count() == 0)
                            {
                                lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
                            }
                        }
                    }
                    lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
                }

                return lstKeyValDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lstKeyValDTO = null;
                objCommonDAL = null;
                DBWOData = null;
            }
        }

        private List<KeyValDTO> getKeyValueList<T>(long ReportID, string fieldName, IEnumerable<T> DBWOData)
        {
            List<KeyValDTO> lsttempKeyValDTO = null;
            ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
            ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            objRPTDTO = objDAL.GetReportDetail(ReportID);

            List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
            string _FieldColumnID = string.Empty;
            if (objRPTDTO != null && objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
            {
                Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                {
                    var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                    if (objField != null)
                    {
                        _FieldColumnID = objField.FieldColumnID;
                    }
                }
            }
            else
            {
                lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                {
                    var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (fieldName ?? string.Empty).ToLower()).FirstOrDefault();
                    if (objField != null)
                    {
                        _FieldColumnID = objField.FieldColumnID;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(_FieldColumnID))
            {
                lsttempKeyValDTO = (from p in DBWOData
                                    select new KeyValDTO
                                    {
                                        key = ((p.GetType().GetProperty(_FieldColumnID).GetValue(p, null)) ?? string.Empty).ToString(),
                                        value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
                                    }).ToList();
            }

            lsttempKeyValDTO = (from i in lsttempKeyValDTO
                                group i by i.value into g
                                select new KeyValDTO { value = g.Key, key = string.Join(",", g.Select(kvp => Convert.ToString(kvp.key)).Distinct()) }).ToList();
            return lsttempKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
        }
    }
}
