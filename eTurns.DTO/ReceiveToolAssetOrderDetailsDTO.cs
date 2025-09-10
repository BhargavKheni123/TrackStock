using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class ReceiveToolAssetOderLineItemsDTO : ToolAssetOrderDetailsDTO
    {
        public Nullable<System.Int64> ReceiveBin { get; set; }

        public Nullable<System.DateTime> ReceiveDate { get; set; }

        public Nullable<System.Double> ReceiveQuantity { get; set; }
    }


    public class ReceivableToolDTO
    {
        public string ToolAssetOrderNumber { get; set; }
        public Guid ToolAssetOrderGUID { get; set; }
        public int OrderStatus { get; set; }
        public string OrderReleaseNumber { get; set; }

        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string ToolDescription { get; set; }
        public Int64 ToolType { get; set; }
        public Guid ToolGUID { get; set; }
        public Guid ToolAssetOrderDetailGUID { get; set; }
        public Int64? ReceiveBinID { get; set; }
        public Guid? ReceiveLocationGUID { get; set; }
        public string ReceiveBinName { get; set; }

        public string OrderStatusText { get; set; }

        public char OrderStatusChar { get { return FnCommon.GetOrderStatusChar(OrderStatus); } }
        public DateTime OrderRequiredDate { get; set; }
        public DateTime OrderDetailRequiredDate { get; set; }
        public double RequestedQuantity { get; set; }
        public double ApprovedQuantity { get; set; }
        public double ReceivedQuantity { get; set; }
        public double? InTransitQuantity { get; set; }
        public int TotalRecords { get; set; }
        public Int64 OrderCreatedByID { get; set; }
        public Int64 OrderLastUpdatedByID { get; set; }
        public string OrderCreatedByName { get; set; }
        public string OrderUpdatedByName { get; set; }
        public DateTime OrderLastUpdated { get; set; }
        public DateTime OrderCreated { get; set; }
        public double ToolCost { get; set; }
        public Int64 ToolID { get; set; }
        public Int64 ToolAssetOrderID { get; set; }
        public Int64 ToolAssetOrderDetailID { get; set; }
        public string ToolUDF1 { get; set; }
        public string ToolUDF2 { get; set; }
        public string ToolUDF3 { get; set; }
        public string ToolUDF4 { get; set; }
        public string ToolUDF5 { get; set; }

        public string ToolUDF6 { get; set; }
        public string ToolUDF7 { get; set; }
        public string ToolUDF8 { get; set; }
        public string ToolUDF9 { get; set; }
        public string ToolUDF10 { get; set; }

        public double PackingQuantity { get; set; }

        public string ODPackSlipNumbers { get; set; }
        public string OrderNumber_ForSorting { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> ReceivedToolDetail { get; set; }

        public bool IsOnlyFromUI { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public DateTime ReceivedOn { get; set; }
        public string OrderRequiredDateStr { get; set; }
        public bool? IsEDIOrder { get; set; }
        public bool? IsCloseTool { get; set; }


        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(OrderCreated, true);
                }
                return _createdDate;
            }
            set { this._createdDate = value; }
        }

        private string _updatedDate;
        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedDate))
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(OrderLastUpdated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }
        private string _ReceivedOn;
        public string ReceivedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOn))
                {
                    _ReceivedOn = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOn;
            }
            set { this._ReceivedOn = value; }
        }

        private string _ReceivedOnWeb;
        public string ReceivedOnDateWeb
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWeb))
                {
                    _ReceivedOnWeb = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWeb;
            }
            set { this._ReceivedOnWeb = value; }
        }

        private string _ReqDate;
        public string strReqDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReqDate))
                {
                    _ReqDate = FnCommon.ConvertDateByTimeZone(OrderRequiredDate, true, true);
                }
                return _ReqDate;
            }
            set { this._ReqDate = value; }
        }

        private string _ReqDtlDate;
        public string strReqDtlDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReqDtlDate))
                {
                    _ReqDtlDate = FnCommon.ConvertDateByTimeZone(OrderDetailRequiredDate, true, true);
                }
                return _ReqDtlDate;
            }
            set { this._ReqDtlDate = value; }
        }

        public Nullable<System.Double> ToolQuantity { get; set; }
        public Int64 ToolDefaultLocation { get; set; }

        public string ToolDefaultLocationName { get; set; }
        public bool SerialNumberTracking { get; set; }

        public string Description { get; set; }
    }


    public class ResReceiveToolAssetOrderDetails
    {
        private static string ResourceFileName = "ResReceiveToolAssetOrderDetails";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceRead.GetResourceValue("Action", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveOrderDetails {0} already exist! Try with Another!.
        /// </summary>
        public static string Duplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("Duplicate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to HistoryID.
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeDeleted", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceRead.GetResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveOrderDetails.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveOrderDetails.
        /// </summary>
        public static string ReceiveToolAssetOrderDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveToolAssetOrderDetails", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View History.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ViewHistory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OrderDetailID.
        /// </summary>
        public static string OrderDetailID
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderDetailID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OrderDetailGUID.
        /// </summary>
        public static string OrderDetailGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderDetailGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveBin.
        /// </summary>
        public static string ReceiveBin
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveBin", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveDate.
        /// </summary>
        public static string ReceiveDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveDate", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to RequestedQuantity.
        /// </summary>
        public static string RequestedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveQuantity.
        /// </summary>
        public static string ReceiveQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created.
        /// </summary>
        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdated.
        /// </summary>
        public static string LastUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdatedBy.
        /// </summary>
        public static string LastUpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDeleted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IsArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GUID.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
            }
        }


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string ItemID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OrderID.
        /// </summary>
        public static string Order
        {
            get
            {
                return ResourceRead.GetResourceValue("Order", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequiredDate.
        /// </summary>
        public static string RequiredDate
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredDate", ResourceFileName);
            }
        }


        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName);
            }
        }
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName);
            }
        }
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName);
            }
        }
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName);
            }
        }
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName);
            }
        }

        //
        public static string ReceivedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedDate", ResourceFileName);
            }
        }


    }

    public class RPT_ReceiveToolDTO
    {
        public Int64 ToolID { get; set; }
        public string ToolName { get; set; }
        public string OrderNumber { get; set; }

        public double? ToolCost { get; set; }
        public string ReceiveBin { get; set; }
        public double? RequestedQuantity { get; set; }
        public double? ApprovedQuantity { get; set; }
        public double? ReceivedQuantity { get; set; }
        public double? Total { get; set; }
        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string ReceivedDate { get; set; }
        public double? RecievedCost { get; set; }
        public string ReceivedUDF1 { get; set; }
        public string ReceivedUDF2 { get; set; }
        public string ReceivedUDF3 { get; set; }
        public string ReceivedUDF4 { get; set; }
        public string ReceivedUDF5 { get; set; }
        public string LastUpdatedBy { get; set; }
        public int? CostDecimalPoint { get; set; }
        public int? QuantityDecimalPoint { get; set; }
        public string OrderStatus { get; set; }
        public char OrderStatusChar
        {
            get
            {
                return FnCommon.GetToolAssetOrderStatusChar(OrderStatus);
            }
        }
        public string OrderType { get; set; }
        public string RequiredDate { get; set; }

        public string LastUpdatedOn { get; set; }
        public string OrderNumber_ForSorting { get; set; }
        public Guid ToolGuid { get; set; }
        public string RoomInfo { get; set; }
        public string ToolDescription { get; set; }
        public string ToolCategory { get; set; }
        public string UOM { get; set; }
        public double? OnHandQuantity { get; set; }
        public double? StagedQuantity { get; set; }
        public double? OnOrderQuantity { get; set; }
        public double? CriticalQuantity { get; set; }
        public double? MinimumQuantity { get; set; }
        public double? MaximumQuantity { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public double? SellPrice { get; set; }
        public double? ExtendedCost { get; set; }
        public double? AverageCost { get; set; }


    }

    public class RPT_ReceivableToolDTO
    {
        public Guid OrderDetailGuid { get; set; }
        public Guid ToolGUID { get; set; }
        public Guid GUID { get; set; }
        public string ToolName { get; set; }

        public string OrderNumber { get; set; }


        public string BinName { get; set; }

        public double? ReceivedQuantity { get; set; }
        public double? ApprovedQuantity { get; set; }

    }

}
