using eTurns.DTO.Resources;
using System;

namespace eTurns.DTO
{
    public class DashboardDTO
    {

    }
    public class DashboardWidgeDTO
    {
        public long WidgetID { get; set; }
        public long UserId { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public long EnterpriseId { get; set; }
        public long DashboardType { get; set; }
        public string WidgetOrder { get; set; }
    }
    public class ItemMonthWiseStockOutDTO
    {
        public int? Id { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public double? Turns { get; set; }
        public double? InventoryValue { get; set; }
        public long? Stockouts { get; set; }
    }

    public class DashboardBottomAndTopSpendDTO
    {
        public int RowNum { get; set; }
        public int TotalRecords { get; set; }

        public Guid ItemGUID { get; set; }

        public string ItemNumber { get; set; }
        public string SupplierName { get; set; }

        public double OrderCost { get; set; }

        public long SupplierID { get; set; }

    }

    public class ResDashboard
    {
        private static string resourceFile = typeof(ResDashboard).Name;

        /// <summary>
        ///   Looks up a localized string similar to Rooms.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }


        public static string IYSecaxisTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("IYSecaxisTitle", resourceFile);
            }
        }

        public static string ICYaxisTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ICYaxisTitle", resourceFile);
            }
        }

        public static string MinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMax", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string Authentication
        {
            get
            {
                return ResourceRead.GetResourceValue("Authentication", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string Masters
        {
            get
            {
                return ResourceRead.GetResourceValue("Masters", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string Assets
        {
            get
            {
                return ResourceRead.GetResourceValue("Assets", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string Reports
        {
            get
            {
                return ResourceRead.GetResourceValue("Reports", resourceFile);
            }
        }
        public static string Configuration
        {
            get
            {
                return ResourceRead.GetResourceValue("Configuration", resourceFile);
            }
        }
        public static string Kits
        {
            get
            {
                return ResourceRead.GetResourceValue("Kits", resourceFile);
            }
        }
        public static string Receive
        {
            get
            {
                return ResourceRead.GetResourceValue("Receive", resourceFile);
            }
        }
        public static string Replenish
        {
            get
            {
                return ResourceRead.GetResourceValue("Replenish", resourceFile);
            }
        }
        public static string Consume
        {
            get
            {
                return ResourceRead.GetResourceValue("Consume", resourceFile);
            }
        }
        public static string Inventry
        {
            get
            {
                return ResourceRead.GetResourceValue("Inventry", resourceFile);
            }
        }
        public static string Count
        {
            get
            {
                return ResourceRead.GetResourceValue("Count", resourceFile);
            }
        }
        public static string Cart
        {
            get
            {
                return ResourceRead.GetResourceValue("Cart", resourceFile);
            }
        }
        public static string ToolMaintenanceName
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolMaintenanceName", resourceFile);
            }
        }

        public static string ToolScheduleDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolScheduleDate", resourceFile);
            }
        }
        public static string ToolTrackingMeasurement
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTrackingMeasurement", resourceFile);
            }
        }
        public static string AssetMaintenanceName
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetMaintenanceName", resourceFile);
            }
        }
        public static string AssetScheduleDate
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetScheduleDate", resourceFile);
            }
        }
        public static string AssetTrackingMeasurement
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetTrackingMeasurement", resourceFile);
            }
        }
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", resourceFile);
            }
        }
        public static string ItemDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemDescription", resourceFile);
            }
        }
        public static string ItemOnHandQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemOnHandQuantity", resourceFile);
            }
        }
        public static string ItemCriticalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCriticalQuantity", resourceFile);
            }
        }
        public static string ItemMinimumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemMinimumQuantity", resourceFile);
            }
        }
        public static string ItemMaximumQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemMaximumQuantity", resourceFile);
            }
        }
        public static string ItemSuggestedOrderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemSuggestedOrderQuantity", resourceFile);
            }
        }

        public static string ItemCartQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCartQuantity", resourceFile);
            }
        }
        public static string ItemOnOrderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemOnOrderQuantity", resourceFile);
            }
        }
        public static string ItemOnTransferQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemOnTransferQuantity", resourceFile);
            }
        }
        public static string ItemAverageUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemAverageUsage", resourceFile);
            }
        }
        public static string ItemTurns
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemTurns", resourceFile);
            }
        }
        public static string ItemInventoryClassification
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemInventoryClassification", resourceFile);
            }
        }
        public static string ItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCost", resourceFile);
            }
        }
        public static string ItemStockOut
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemStockOut", resourceFile);
            }
        }
        public static string TransferNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferNumber", resourceFile);
            }
        }
        public static string TransferStagingName
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferStagingName", resourceFile);
            }
        }
        public static string TransferRequireDate
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferRequireDate", resourceFile);
            }
        }
        public static string TransferReplinishRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferReplinishRoom", resourceFile);
            }
        }
        public static string TransferNoOfLineItems
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferNoOfLineItems", resourceFile);
            }
        }
        public static string OrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderNumber", resourceFile);
            }
        }
        public static string OrderStagingName
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderStagingName", resourceFile);
            }
        }
        public static string OrderRequireDate
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderRequireDate", resourceFile);
            }
        }
        public static string OrderSupplier
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderSupplier", resourceFile);
            }
        }
        public static string OrderReleaseNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderReleaseNumber", resourceFile);
            }
        }

        public static string OrderNoOfLineItem
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderNoOfLineItem", resourceFile);
            }
        }
        public static string RequisitionNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionNumber", resourceFile);
            }
        }
        public static string RequisitionDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionDescription", resourceFile);
            }
        }
        public static string RequisitionRequiredDate
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionRequiredDate", resourceFile);
            }
        }
        public static string ProjectSpendName
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendName", resourceFile);
            }
        }
        public static string ProjectSpendDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendDescription", resourceFile);
            }
        }
        public static string ProjectSpendDollarLimitAmount
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendDollarLimitAmount", resourceFile);
            }
        }
        public static string ProjectSpendDollarUsedAmount
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendDollarUsedAmount", resourceFile);
            }
        }
        public static string ProjectSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpend", resourceFile);
            }
        }
        public static string CartItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("CartItemNumber", resourceFile);
            }
        }
        public static string CartSupplierName
        {
            get
            {
                return ResourceRead.GetResourceValue("CartSupplierName", resourceFile);
            }
        }
        public static string CartQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CartQuantity", resourceFile);
            }
        }
        public static string CountID
        {
            get
            {
                return ResourceRead.GetResourceValue("CountID", resourceFile);
            }
        }
        public static string CountName
        {
            get
            {
                return ResourceRead.GetResourceValue("CountName", resourceFile);
            }
        }
        public static string CountType
        {
            get
            {
                return ResourceRead.GetResourceValue("CountType", resourceFile);
            }
        }
        public static string CountStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("CountStatus", resourceFile);
            }
        }
        public static string NoofCount
        {
            get
            {
                return ResourceRead.GetResourceValue("NoofCount", resourceFile);
            }
        }
        public static string TabTool
        {
            get
            {
                return ResourceRead.GetResourceValue("TabTool", resourceFile);
            }
        }
        public static string TabAsset
        {
            get
            {
                return ResourceRead.GetResourceValue("TabAsset", resourceFile);
            }
        }
        public static string TabItem
        {
            get
            {
                return ResourceRead.GetResourceValue("TabItem", resourceFile);
            }
        }
        public static string TabTransferReplenish
        {
            get
            {
                return ResourceRead.GetResourceValue("TabTransferReplenish", resourceFile);
            }
        }
        public static string TabOrderReplenish
        {
            get
            {
                return ResourceRead.GetResourceValue("TabOrderReplenish", resourceFile);
            }
        }
        public static string TabRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("TabRequisition", resourceFile);
            }
        }
        public static string TabProjectSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("TabProjectSpend", resourceFile);
            }
        }
        public static string TabCartOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("TabCartOrder", resourceFile);
            }
        }
        public static string TabCartTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("TabCartTransfer", resourceFile);
            }
        }
        public static string SubTabItemStockOut
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabItemStockOut", resourceFile);
            }
        }
        public static string SubTabItemCritical
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabItemCritical", resourceFile);
            }
        }
        public static string SubTabItemMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabItemMinimum", resourceFile);
            }
        }
        public static string SubTabItemMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabItemMaximum", resourceFile);
            }
        }
        public static string SubTabItemSlowMoving
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabItemSlowMoving", resourceFile);
            }
        }
        public static string SubTabItemFastMoving
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabItemFastMoving", resourceFile);
            }
        }
        public static string SubTabItemInvValue
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabItemInvValue", resourceFile);
            }
        }
        public static string SubTabTransferTobeSubmitted
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabTransferTobeSubmitted", resourceFile);
            }
        }
        public static string SubTabTransferTobeApproved
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabTransferTobeApproved", resourceFile);
            }
        }
        public static string SubTabTransferIncompletereceivePastdue
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabTransferIncompletereceivePastdue", resourceFile);
            }
        }
        public static string SubTabOrderTobeSubmitted
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabOrderTobeSubmitted", resourceFile);
            }
        }
        public static string SubTabOrderTobeApproved
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabOrderTobeApproved", resourceFile);
            }
        }
        public static string SubTabOrderIncompletereceivePastdue
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabOrderIncompletereceivePastdue", resourceFile);
            }
        }

        public static string SubTabOrderBottomSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabOrderBottomSpend", resourceFile);
            }
        }

        public static string SubTabOrderTopSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabOrderTopSpend", resourceFile);
            }
        }


        public static string SubTabRequisitionTobeSubmitted
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabRequisitionTobeSubmitted", resourceFile);
            }
        }
        public static string SubTabRequisitionTobeApproved
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabRequisitionTobeApproved", resourceFile);
            }
        }
        public static string SubTabRequisitionTobePulled
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabRequisitionTobePulled", resourceFile);
            }
        }
        public static string SubTabProjectSpendAmountExceeds
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabProjectSpendAmountExceeds", resourceFile);
            }
        }
        public static string SubTabProjectSpendQuantityExceeds
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabProjectSpendQuantityExceeds", resourceFile);
            }
        }
        public static string SubTabProjectSpendItemAmountExceeds
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTabProjectSpendItemAmountExceeds", resourceFile);
            }
        }
        public static string TitleStockOutChart
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleStockOutChart", resourceFile);
            }
        }
        public static string SubTitleStockOutChart
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleStockOutChart", resourceFile);
            }
        }
        public static string TitleCriticalChart
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleCriticalChart", resourceFile);
            }
        }
        public static string SubTitleCriticalChart
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleCriticalChart", resourceFile);
            }
        }
        public static string TitleMinimumChart
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleMinimumChart", resourceFile);
            }
        }
        public static string SubTitleMinimumChart
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleMinimumChart", resourceFile);
            }
        }
        public static string TitleMaximumChart
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleMaximumChart", resourceFile);
            }
        }
        public static string SubTitleMaximumChart
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleMaximumChart", resourceFile);
            }
        }
        public static string TitleSlowMovingChart
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleSlowMovingChart", resourceFile);
            }
        }
        public static string SubTitleSlowMovingChart
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleSlowMovingChart", resourceFile);
            }
        }
        public static string TitleFastMovingChart
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleFastMovingChart", resourceFile);
            }
        }
        public static string SubTitleFastMovingChart
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleFastMovingChart", resourceFile);
            }
        }
        public static string TitleInventoryValueChart
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleInventoryValueChart", resourceFile);
            }
        }
        public static string SubTitleInventoryValueChart
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleInventoryValueChart", resourceFile);
            }
        }
        public static string TitleChartTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleChartTitle", resourceFile);
            }
        }
        public static string TitleUnsubmittedRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleUnsubmittedRequisition", resourceFile);
            }
        }
        public static string TitleProjectAmountExeeded
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleProjectAmountExeeded", resourceFile);
            }
        }
        public static string SubTitleUnsubmittedRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleUnsubmittedRequisition", resourceFile);
            }
        }
        public static string SubTitleProjectAmountExeeded
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleProjectAmountExeeded", resourceFile);
            }
        }
        public static string TitleUnapprovedRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleUnapprovedRequisition", resourceFile);
            }
        }
        public static string TitlePSItemQtyExeeded
        {
            get
            {
                return ResourceRead.GetResourceValue("TitlePSItemQtyExeeded", resourceFile);
            }
        }

        public static string SubTitlePSItemQtyExeeded
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitlePSItemQtyExeeded", resourceFile);
            }
        }


        public static string TitlePSItemAmountExeeded
        {
            get
            {
                return ResourceRead.GetResourceValue("TitlePSItemAmountExeeded", resourceFile);
            }
        }

        public static string SubTitlePSItemAmountExeeded
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitlePSItemAmountExeeded", resourceFile);
            }
        }



        public static string SubTitleUnapprovedRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleUnapprovedRequisition", resourceFile);
            }
        }
        public static string TitleTobePulledRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleTobePulledRequisition", resourceFile);
            }
        }

        public static string SubTitleTobePulledRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleTobePulledRequisition", resourceFile);
            }
        }
        public static string TitleRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleRequisition", resourceFile);
            }
        }
        public static string TitleProjectAmountExceeds
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleProjectAmountExceeds", resourceFile);
            }
        }
        public static string TitleProjectQuantityExceeds
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleProjectQuantityExceeds", resourceFile);
            }
        }
        public static string TitleItemAmountExceeds
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleItemAmountExceeds", resourceFile);
            }
        }
        public static string TitleToolMaintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleToolMaintenance", resourceFile);
            }
        }
        public static string TitleAssetMaintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleAssetMaintenance", resourceFile);
            }
        }
        public static string TitleUnsubmittedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleUnsubmittedOrder", resourceFile);
            }
        }
        public static string SubTitleUnsubmittedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleUnsubmittedOrder", resourceFile);
            }
        }
        public static string TitleUnapprovedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleUnapprovedOrder", resourceFile);
            }
        }
        public static string SubTitleUnapprovedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleUnapprovedOrder", resourceFile);
            }
        }
        public static string TitleTobeReceivedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleTobeReceivedOrder", resourceFile);
            }
        }
        public static string TitleReceivableItems
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleReceivableItems", resourceFile);
            }
        }
        public static string SubTitleReceivableItems
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleReceivableItems", resourceFile);
            }
        }

        public static string SubTitleTobeReceivedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleTobeReceivedOrder", resourceFile);
            }
        }
        public static string TitleUnsubmittedTransfers
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleUnsubmittedTransfers", resourceFile);
            }
        }

        public static string SubTitleUnsubmittedTransfers
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleUnsubmittedTransfers", resourceFile);
            }
        }

        public static string TitleUnapprovedTransfers
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleUnapprovedTransfers", resourceFile);
            }
        }
        public static string SubTitleUnapprovedTransfers
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleUnapprovedTransfers", resourceFile);
            }
        }

        public static string TitleTobeReceivedTransfers
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleTobeReceivedTransfers", resourceFile);
            }
        }
        public static string SubTitleTobeReceivedTransfers
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleTobeReceivedTransfers", resourceFile);
            }
        }
        public static string TitleInventoryCount
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleInventoryCount", resourceFile);
            }
        }
        public static string TitleCartTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleCartTransfer", resourceFile);
            }
        }
        public static string TitlecartOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("TitlecartOrder", resourceFile);
            }
        }


        public static string ConsumeChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsumeChartXTitle", resourceFile);
            }
        }

        public static string AssetChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetChartXTitle", resourceFile);
            }
        }
        public static string OrderChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderChartXTitle", resourceFile);
            }
        }
        public static string ReceivableItemChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivableItemChartXTitle", resourceFile);
            }
        }
        public static string TransferChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferChartXTitle", resourceFile);
            }
        }
        public static string TransferChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferChartYTitle", resourceFile);
            }
        }

        public static string OrderChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderChartYTitle", resourceFile);
            }
        }
        public static string ReceivableItemChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivableItemChartYTitle", resourceFile);
            }
        }
        public static string ConsumeChartPSXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsumeChartPSXTitle", resourceFile);
            }
        }

        public static string ConsumeChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsumeChartYTitle", resourceFile);
            }
        }
        public static string ConsumeChartPSYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsumeChartPSYTitle", resourceFile);
            }
        }

        public static object Alerts
        {
            get
            {
                return ResourceRead.GetResourceValue("Alerts", resourceFile);
            }
        }

        public static string AssetChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetChartYTitle", resourceFile);
            }
        }



        public static string TitleAssetDue
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleAssetDue", resourceFile);
            }
        }

        public static string SubTitleAssetDue
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleAssetDue", resourceFile);
            }
        }

        public static string ToolChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolChartXTitle", resourceFile);
            }
        }

        public static string ToolChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolChartYTitle", resourceFile);
            }
        }

        public static string TitleToolDue
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleToolDue", resourceFile);
            }
        }

        public static string SubTitleToolDue
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleToolDue", resourceFile);
            }
        }

        public static string InventoryCountChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryCountChartXTitle", resourceFile);
            }
        }

        public static string InventoryCountChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryCountChartYTitle", resourceFile);
            }
        }

        public static string TitleInventoryCountDue
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleInventoryCountDue", resourceFile);
            }
        }

        public static string SubTitleInventoryCountDue
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleInventoryCountDue", resourceFile);
            }
        }

        public static string SuggestedOrderChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedOrderChartXTitle", resourceFile);
            }
        }

        public static string SuggestedOrderChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedOrderChartYTitle", resourceFile);
            }
        }

        public static string TitleSuggestedOrderDue
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleSuggestedOrderDue", resourceFile);
            }
        }

        public static string SubTitleSuggestedOrderDue
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleSuggestedOrderDue", resourceFile);
            }
        }

        public static string SuggestedTransferChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedTransferChartXTitle", resourceFile);
            }
        }

        public static string SuggestedTransferChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedTransferChartYTitle", resourceFile);
            }
        }

        public static string TitleSuggestedTransferDue
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleSuggestedTransferDue", resourceFile);
            }
        }

        public static string SubTitleSuggestedTransferDue
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleSuggestedTransferDue", resourceFile);
            }
        }

        public static string TitleBottomSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleBottomSpend", resourceFile);
            }
        }

        public static string SubTitleBottomSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleBottomSpend", resourceFile);
            }
        }

        public static string BottomTopSpendChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("BottomTopSpendChartXTitle", resourceFile);
            }
        }

        public static string BottomTopSpendChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("BottomTopSpendChartYTitle", resourceFile);
            }
        }
        public static string TopSpendChartXTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("TopSpendChartXTitle", resourceFile);
            }
        }

        public static string TopSpendChartYTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("TopSpendChartYTitle", resourceFile);
            }
        }

        public static string TitleTopSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleTopSpend", resourceFile);
            }
        }

        public static string SubTitleTopSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("SubTitleTopSpend", resourceFile);
            }
        }

        public static string TurnSetting
        {
            get
            {
                return ResourceRead.GetResourceValue("TurnSetting", resourceFile);
            }
        }
        public static string MeasurmentMethod
        {
            get
            {
                return ResourceRead.GetResourceValue("MeasurmentMethod", resourceFile);
            }
        }
        public static string NumberofWholeMonthsofUsagetoSample
        {
            get
            {
                return ResourceRead.GetResourceValue("NumberofWholeMonthsofUsagetoSample", resourceFile);
            }
        }
        public static string Turns
        {
            get
            {
                return ResourceRead.GetResourceValue("Turns", resourceFile);
            }
        }
        public static string AverageUsageSetting
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageUsageSetting", resourceFile);
            }
        }
        public static string DaysofUsagetoSample
        {
            get
            {
                return ResourceRead.GetResourceValue("DaysofUsagetoSample", resourceFile);
            }
        }
        public static string DaysofUsagetoSampleInstock
        {
            get
            {
                return ResourceRead.GetResourceValue("DaysofUsagetoSampleInstock", resourceFile);
            }
        }
        public static string MinimumandMaximumOptimizationSettings
        {
            get
            {
                return ResourceRead.GetResourceValue("MinimumandMaximumOptimizationSettings", resourceFile);
            }
        }
        public static string perDisaboveoptimizationvalue1
        {
            get
            {
                return ResourceRead.GetResourceValue("perDisaboveoptimizationvalue1", resourceFile);
            }
        }
        public static string perDisbetweenoptimizationvalue1andvalue2
        {
            get
            {
                return ResourceRead.GetResourceValue("perDisbetweenoptimizationvalue1andvalue2", resourceFile);
            }
        }
        public static string perDisbelowoptimizationvalue2
        {
            get
            {
                return ResourceRead.GetResourceValue("perDisbelowoptimizationvalue2", resourceFile);
            }
        }
        public static string Minimum
        {
            get
            {
                return ResourceRead.GetResourceValue("Minimum", resourceFile);
            }
        }
        public static string minmaxmessage1
        {
            get
            {
                return ResourceRead.GetResourceValue("minmaxmessage1", resourceFile);
            }
        }
        public static string minmaxmessage2
        {
            get
            {
                return ResourceRead.GetResourceValue("minmaxmessage2", resourceFile);
            }
        }
        public static string MaximumMinimumx
        {
            get
            {
                return ResourceRead.GetResourceValue("MaximumMinimumx", resourceFile);
            }
        }
        public static string OtherSettings
        {
            get
            {
                return ResourceRead.GetResourceValue("OtherSettings", resourceFile);
            }
        }
        public static string PieChartmetric
        {
            get
            {
                return ResourceRead.GetResourceValue("PieChartmetric", resourceFile);
            }
        }

        public static string Item
        {
            get
            {
                return ResourceRead.GetResourceValue("Item", resourceFile);
            }
        }
        public static string Category
        {
            get
            {
                return ResourceRead.GetResourceValue("Category", resourceFile);
            }
        }
        public static string Manufacturer
        {
            get
            {
                return ResourceRead.GetResourceValue("Manufacturer", resourceFile);
            }
        }
        public static string Roomlevelitemusageautocalc
        {
            get
            {
                return ResourceRead.GetResourceValue("Roomlevelitemusageautocalc", resourceFile);
            }
        }
        public static string Autoclassification
        {
            get
            {
                return ResourceRead.GetResourceValue("Autoclassification", resourceFile);
            }
        }
        public static string InventoryValueTurnsStockoutgraph
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryValueTurnsStockoutgraph", resourceFile);
            }
        }
        public static string Fromyear
        {
            get
            {
                return ResourceRead.GetResourceValue("Fromyear", resourceFile);
            }
        }
        public static string Month
        {
            get
            {
                return ResourceRead.GetResourceValue("Month", resourceFile);
            }
        }
        public static string ToYear
        {
            get
            {
                return ResourceRead.GetResourceValue("ToYear", resourceFile);
            }
        }


        //
        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", resourceFile);
            }
        }

        public static string QuantumStartDate
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantumStartDate", resourceFile);
            }
        }

        public static string QuantumEndDate
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantumEndDate", resourceFile);
            }
        }

        public static string AnnualCarryingCostPercent
        {
            get
            {
                return ResourceRead.GetResourceValue("AnnualCarryingCostPercent", resourceFile);
            }
        }
        public static string LargestAnnualCashSavings
        {
            get
            {
                return ResourceRead.GetResourceValue("LargestAnnualCashSavings", resourceFile);
            }
        }


        public static string ItemsCount
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsCount", resourceFile);
            }
        }
        public static string ItemLocationsCount
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationsCount", resourceFile);
            }
        }
        public static string StockOutCount
        {
            get
            {
                return ResourceRead.GetResourceValue("StockOutCount", resourceFile);
            }
        }
        public static string ItemsWithoutCostCount
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsWithoutCostCount", resourceFile);
            }
        }
        public static string ItemsTuningUpdateSettingCount
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsTuningUpdateSettingCount", resourceFile);
            }
        }
        public static string AnnualCarryingCostofInventoryValue
        {
            get
            {
                return ResourceRead.GetResourceValue("AnnualCarryingCostofInventoryValue", resourceFile);
            }
        }
        public static string ReductionOptimizedInvValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ReductionOptimizedInvValue", resourceFile);
            }
        }
        public static string AnnualCashSavingsfromReducedCarryingCost
        {
            get
            {
                return ResourceRead.GetResourceValue("AnnualCashSavingsfromReducedCarryingCost", resourceFile);
            }
        }
        public static string LargestAnnualCashSavingsfromReducedCarryingCost
        {
            get
            {
                return ResourceRead.GetResourceValue("LargestAnnualCashSavingsfromReducedCarryingCost", resourceFile);
            }
        }
        public static string ItemLocationsPeriodOrderUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationsPeriodOrderUsage", resourceFile);
            }
        }
        public static string ItemLocationsPeriodPullUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationsPeriodPullUsage", resourceFile);
            }
        }
        public static string ItemLocationsPeriodPullValueUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationsPeriodPullValueUsage", resourceFile);
            }
        }
        public static string Recalculate
        {
            get
            {
                return ResourceRead.GetResourceValue("Recalculate", resourceFile);
            }
        }

        public static object InventortyCarryingCost
        {
            get
            {
                return ResourceRead.GetResourceValue("InventortyCarryingCost", resourceFile);
            }
        }

        public static object NumberofDaysofUsagetoSample
        {
            get
            {
                return ResourceRead.GetResourceValue("NumberofDaysofUsagetoSample", resourceFile);
            }
        }
        public static string MsgInvalidUpdateMode
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidUpdateMode", resourceFile);
            }
        }
        public static string MsgRecordToPrint
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordToPrint", resourceFile);
            }
        }
        public static string OptimizationParameters { get { return ResourceRead.GetResourceValue("OptimizationParameters", resourceFile); } }
        public static string YaxisTitleStockoutcount { get { return ResourceRead.GetResourceValue("YaxisTitleStockoutcount", resourceFile); } }
        public static string YaxisTitleturns { get { return ResourceRead.GetResourceValue("YaxisTitleturns", resourceFile); } }
        public static string XAxisTitleItemsorKits { get { return ResourceRead.GetResourceValue("XAxisTitleItemsorKits", resourceFile); } }
        public static string MeasurementPullTransferValue { get { return ResourceRead.GetResourceValue("MeasurementPullTransferValue", resourceFile); } }
        public static string MeasurementPullTransfer { get { return ResourceRead.GetResourceValue("MeasurementPullTransfer", resourceFile); } }
        public static string MeasurementOrders { get { return ResourceRead.GetResourceValue("MeasurementOrders", resourceFile); } }
    }
    public class ResInventoryAnalysis
    {
        private static string resourceFile = typeof(ResInventoryAnalysis).Name;

        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        public static object InventortyCarryingCost
        {
            get
            {
                return ResourceRead.GetResourceValue("InventortyCarryingCost", resourceFile);
            }
        }

        public static string AbsValDifCurrCalcMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("AbsValDifCurrCalcMinimum", resourceFile);
            }
        }
        public static string AbsValDifCurrCalcMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("AbsValDifCurrCalcMaximum", resourceFile);
            }
        }
        public static string MinAnalysis
        {
            get
            {
                return ResourceRead.GetResourceValue("MinAnalysis", resourceFile);
            }
        }
        public static string MaxAnalysis
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxAnalysis", resourceFile);
            }
        }
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", resourceFile);
            }
        }
        public static string DefaultReorderQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultReorderQuantity", resourceFile);
            }
        }
        public static string CostUOMValue
        {
            get
            {
                return ResourceRead.GetResourceValue("CostUOMValue", resourceFile);
            }
        }
        public static string DateCreated
        {
            get
            {
                return ResourceRead.GetResourceValue("DateCreated", resourceFile);
            }
        }

        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", resourceFile);
            }
        }
        public static string IsitemLevelMinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("IsitemLevelMinMax", resourceFile);
            }
        }
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", resourceFile);
            }
        }
        public static string InventoryClassification
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryClassification", resourceFile);
            }
        }
        public static string Category
        {
            get
            {
                return ResourceRead.GetResourceValue("Category", resourceFile);
            }
        }
        public static string SupplierName
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierName", resourceFile);
            }
        }

        public static string SupplierPartNo
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierPartNo", resourceFile);
            }
        }

        public static string Manufacturer
        {
            get
            {
                return ResourceRead.GetResourceValue("Manufacturer", resourceFile);
            }
        }

        public static string ManufacturerNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ManufacturerNumber", resourceFile);
            }
        }

        public static string Location
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", resourceFile);
            }
        }
        public static string AvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableQty", resourceFile);
            }
        }
        public static string InventoryValue
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryValue", resourceFile);
            }
        }
        public static string AverageCost
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageCost", resourceFile);
            }
        }
        public static string PeriodPullValueUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("PeriodPullValueUsage", resourceFile);
            }
        }
        public static string AvgDailyPullValueUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("AvgDailyPullValueUsage", resourceFile);
            }
        }
        public static string PeriodPullUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("PeriodPullUsage", resourceFile);
            }
        }
        public static string AvgDailyPullUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("AvgDailyPullUsage", resourceFile);
            }
        }
        public static string PeriodOrdersUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("PeriodOrdersUsage", resourceFile);
            }
        }
        public static string AvgDailyOrdersUsage
        {
            get
            {
                return ResourceRead.GetResourceValue("AvgDailyOrdersUsage", resourceFile);
            }
        }
        public static string PullValueTurns
        {
            get
            {
                return ResourceRead.GetResourceValue("PullValueTurns", resourceFile);
            }
        }
        public static string PullTurns
        {
            get
            {
                return ResourceRead.GetResourceValue("PullTurns", resourceFile);
            }
        }
        public static string OrderTurns
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderTurns", resourceFile);
            }
        }
        public static string Critical
        {
            get
            {
                return ResourceRead.GetResourceValue("Critical", resourceFile);
            }
        }
        public static string CurrentMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("CurrentMinimum", resourceFile);
            }
        }
        public static string CalculatedMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("CalculatedMinimum", resourceFile);
            }
        }

        public static string TrialCalculatedMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("TrialCalculatedMinimum", resourceFile);
            }
        }
        public static string AutocurrentminPercentage
        {
            get
            {
                return ResourceRead.GetResourceValue("AutocurrentminPercentage", resourceFile);
            }
        }
        public static string CurrentMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("CurrentMaximum", resourceFile);
            }
        }
        public static string CalculatedMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("CalculatedMaximum", resourceFile);
            }
        }
        public static string AbsvaluediffcurrcalcMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("AbsvaluediffcurrcalcMaximum", resourceFile);
            }
        }
        public static string TrialCalculatedMaximum
        {
            get
            {
                return ResourceRead.GetResourceValue("TrialCalculatedMaximum", resourceFile);
            }
        }
        public static string AutocurrentmaxPercentage
        {
            get
            {
                return ResourceRead.GetResourceValue("AutocurrentmaxPercentage", resourceFile);
            }
        }
        public static string MaximumPercentage
        {
            get
            {
                return ResourceRead.GetResourceValue("MaximumPercentage", resourceFile);
            }
        }
        public static string MinimumPercentage
        {
            get
            {
                return ResourceRead.GetResourceValue("MinimumPercentage", resourceFile);
            }
        }
        public static string Categorymetric
        {
            get
            {
                return ResourceRead.GetResourceValue("Categorymetric", resourceFile);
            }
        }
        public static string Itemmetric
        {
            get
            {
                return ResourceRead.GetResourceValue("Itemmetric", resourceFile);
            }
        }
        public static string Manufacturermetric
        {
            get
            {
                return ResourceRead.GetResourceValue("Manufacturermetric", resourceFile);
            }
        }
        public static string MaxOptimizationparameters
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxOptimizationparameters", resourceFile);
            }
        }
        public static string MinOptimizationparameters
        {
            get
            {
                return ResourceRead.GetResourceValue("MinOptimizationparameters", resourceFile);
            }
        }
        public static string Aboveoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("Aboveoptimization", resourceFile);
            }
        }
        public static string Betweenoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("Betweenoptimization", resourceFile);
            }
        }
        public static string Belowoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("Belowoptimization", resourceFile);
            }
        }


        public static string AbsvaluediffcurrcalcMinimum
        {
            get
            {
                return ResourceRead.GetResourceValue("AbsvaluediffcurrcalcMinimum", resourceFile);
            }
        }

        public static string Actions
        {
            get
            {
                return ResourceRead.GetResourceValue("Actions", resourceFile);
            }
        }
        public static string QtyUntilOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyUntilOrder", resourceFile);
            }
        }
        public static string NoOfDaysUntilOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfDaysUntilOrder", resourceFile);
            }
        }
        public static string DemandPlanningQtyToOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("DemandPlanningQtyToOrder", resourceFile);
            }
        }
        public static string OptimizedInvValueUsesQOHofAvgCalcdMinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("OptimizedInvValueUsesQOHofAvgCalcdMinMax", resourceFile);
            }
        }
        public static string OptimizedInvValueChange
        {
            get
            {
                return ResourceRead.GetResourceValue("OptimizedInvValueChange", resourceFile);
            }
        }
        public static string TrialCalcInvValueUsesQOHofAvgTrialMinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("TrialCalcInvValueUsesQOHofAvgTrialMinMax", resourceFile);
            }
        }
        public static string TrialInvValueChange
        {
            get
            {
                return ResourceRead.GetResourceValue("TrialInvValueChange", resourceFile);
            }
        }
        public static string MinMaxTopLabelInvValue
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTopLabelInvValue", resourceFile);
            }
        }
        public static string MinMaxTopLabelMTDStockOuts
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTopLabelMTDStockOuts", resourceFile);
            }
        }
        public static string MinMaxTopLabelYTDStockOuts
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTopLabelYTDStockOuts", resourceFile);
            }
        }
        public static string MinMaxTopLabelTurns
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTopLabelTurns", resourceFile);
            }
        }
        public static string MinMaxTopLabelTrialInvValueChange
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTopLabelTrialInvValueChange", resourceFile);
            }
        }
        public static string MinMaxTopLabelTrialCalcInvValueUsesQOHofAvgTrialMinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTopLabelTrialCalcInvValueUsesQOHofAvgTrialMinMax", resourceFile);
            }
        }
        public static string MinMaxTopLabelOptimizedInvValueChange
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTopLabelOptimizedInvValueChange", resourceFile);
            }
        }
        public static string MinMaxTopLabelOptimizedInvValueUsesQOHofAvgCalcdMinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTopLabelOptimizedInvValueUsesQOHofAvgCalcdMinMax", resourceFile);
            }
        }
        public static string DateofOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("DateofOrder", resourceFile);
            }
        }
        public static string LeadTimeInDays
        {
            get
            {
                return ResourceRead.GetResourceValue("LeadTimeInDays", resourceFile);
            }
        }

        public static string MinAboveoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("MinAboveoptimization", resourceFile);
            }
        }
        public static string MinBetweenoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("MinBetweenoptimization", resourceFile);
            }
        }
        public static string MinBelowoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("MinBelowoptimization", resourceFile);
            }
        }

        public static string MaxAboveoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxAboveoptimization", resourceFile);
            }
        }
        public static string MaxBetweenoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxBetweenoptimization", resourceFile);
            }
        }
        public static string MaxBelowoptimization
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxBelowoptimization", resourceFile);
            }
        }
        public static string Removethiscashnowfrominventory
        {
            get
            {
                return ResourceRead.GetResourceValue("Removethiscashnowfrominventory", resourceFile);
            }
        }
        public static string Savethiscasheveryyear
        {
            get
            {
                return ResourceRead.GetResourceValue("Savethiscasheveryyear", resourceFile);
            }
        }

        public static string ItemStats
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemStats", resourceFile);
            }
        }

        public static string TrendingSettingManualAlert
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingSettingManualAlert", resourceFile);
            }
        }

        public static string TitleNegativeCalMinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleNegativeCalMinMax", resourceFile);
            }
        }

        // for Dashboard View Log start ////

        public static string MinMaxAction
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxAction", resourceFile);
            }
        }

        public static string MinMaxActionDate
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxActionDate", resourceFile);
            }
        }

        public static string MinMaxCompanyName
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxCompanyName", resourceFile);
            }
        }

        public static string MinMaxRoomName
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxRoomName", resourceFile);
            }
        }

        public static string MinMaxUserName
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxUserName", resourceFile);
            }
        }

        public static string MinMaxItemnumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxItemnumber", resourceFile);
            }
        }

        public static string MinMaxLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxLocation", resourceFile);
            }
        }
        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", resourceFile);
            }
        }

        // for Dashboard View Log end ////

        public static string RegenerateData
        {
            get
            {
                return ResourceRead.GetResourceValue("RegenerateData", resourceFile);
            }
        }
        public static string ForwardDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ForwardDate", resourceFile);
            }
        }
        public static string ResetMinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("ResetMinMax", resourceFile);
            }
        }
        public static string ApplyMinMaxOnSelect
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyMinMaxOnSelect", resourceFile);
            }
        }
        public static string ApplyMinOnSelect
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyMinOnSelect", resourceFile);
            }
        }
        public static string ApplyMaxOnSelect
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyMaxOnSelect", resourceFile);
            }
        }
        public static string Reset
        {
            get
            {
                return ResourceRead.GetResourceValue("Reset", resourceFile);
            }
        }
        public static string ApplyMinMax
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyMinMax", resourceFile);
            }
        }
        public static string ApplyMin
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyMin", resourceFile);
            }
        }
        public static string ApplyMax
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyMax", resourceFile);
            }
        }
        public static string MinMaxTuning
        {
            get
            {
                return ResourceRead.GetResourceValue("MinMaxTuning", resourceFile);
            }
        }
        public static string AreYouWantRegenerateAllTheData
        {
            get
            {
                return ResourceRead.GetResourceValue("AreYouWantRegenerateAllTheData", resourceFile);
            }
        }
        public static string AreYouWantforwaredPullAndStockOutDates
        {
            get
            {
                return ResourceRead.GetResourceValue("AreYouWantforwaredPullAndStockOutDates", resourceFile);
            }
        }
        public static string AreYouWantResetMinMaxCriticalItems
        {
            get
            {
                return ResourceRead.GetResourceValue("AreYouWantResetMinMaxCriticalItems", resourceFile);
            }
        }

        public static string RangeNotSaved { get { return ResourceRead.GetResourceValue("RangeNotSaved", resourceFile); } }
    }

    public class MinMaxDashboardAuditTrailDTO
    {
        public long ID { get; set; }
        public Guid GUID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public Guid? ItemGuid { get; set; }
        public long? BinID { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
        private string _ActionDate;
        public string ActionDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_ActionDate))
                {
                    _ActionDate = FnCommon.ConvertDateByTimeZone(ActionDate, true);
                }
                return _ActionDate;
            }
            set { this._ActionDate = value; }
        }
        public long UserID { get; set; }

        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string UserName { get; set; }
        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }
    }

}
