using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class PreReceivOrderDetailDTO
    {
        public Int64 ID { get; set; }
        public Int64? BinID { get; set; }

        public double? Quantity { get; set; }
        public double? Cost { get; set; }

        public DateTime? ExpirationDate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        [AllowHtml]
        public string LotNumber { get; set; }
        [AllowHtml]
        public string SerialNumber { get; set; }
        [AllowHtml]
        public string PackSlipNumber { get; set; }
        [AllowHtml]
        public string UDF1 { get; set; }
        [AllowHtml]
        public string UDF2 { get; set; }
        [AllowHtml]
        public string UDF3 { get; set; }
        [AllowHtml]
        public string UDF4 { get; set; }
        [AllowHtml]
        public string UDF5 { get; set; }

        public Guid GUID { get; set; }
        public Guid? ItemGUID { get; set; }
        public Guid? OrderDetailGUID { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public Int64? CreatedBy { get; set; }
        public Int64? LastUpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsReceived { get; set; }

        public Int64? CompanyID { get; set; }
        public Int64? Room { get; set; }

        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }

        public Int32 ItemType { get; set; }
        public bool Consignment { get; set; }
        public bool SerialNumberTracking { get; set; }
        public bool LotNumberTracking { get; set; }
        public bool DateCodeTracking { get; set; }
        public string Comment { get; set; }

        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string ItemNumber { get; set; }
        public string BinNumber { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true);
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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true);
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

    }

    public class ToFillPreReceiveDTO
    {
        // Item#, Order# Release# ItemType is only Display Perpose
        public string ItemNumber { get; set; }
        public string OrderNumber { get; set; }
        public string ReleaseNumber { get; set; }
        public string ItemTypeSerialLot { get; set; }
        public bool IsPackSlipMandatory { get; set; }
        public bool SerialNumberTracking { get; set; }
        public bool LotNumberTracking { get; set; }
        public bool DateCodeTracking { get; set; }
        public bool IsModelShow { get; set; }
        public int OrderStatus { get; set; }
        public double? RequestedQty { get; set; }
        public double? ApproveQty { get; set; }
        public double? ReceiveQty { get; set; }

        public string BinNumber { get; set; }
        public string ReceivedDate { get; set; }
        public double? Cost { get; set; }
        public string PackSlipNumber { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }

        public Guid ItemGUID { get; set; }
        public Guid OrderDetailGUID { get; set; }
        public Guid OrderGUID { get; set; }
        public List<ToFillReceiveDetailDTO> MakePreReceiveDetail { get; set; }
        public string Comment { get; set; }
        public double? TotalRecord { get; set; }
        public Nullable<bool> IsReceivedCostChange { get; set; }
        public double? OrderDetailItemCost { get; set; }
        public Nullable<double> InTransitQuantity { get; set; }
        public Nullable<Int32> POLineItemNumber { get; set; }
        public long OrderDetailTrackingID { get; set; }
        
    }

    public class ToFillReceiveDetailDTO
    {
        public double Quantity { get; set; }
        public double QuantityToReceive { get; set; }

        public string ExpirationDate { get; set; }
        public string LotNumber { get; set; }
        public string PackSlipNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Comment { get; set; }
        public long ID { get; set; }

    }

    public class ReceiveErrors
    {
        public Guid OrderDetailGuid { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorMassage { get; set; }
        public string ErrorColor { get; set; }
    }
    public class PreReceivOrderDetailToolDTO
    {
        public Int64 ID { get; set; }
        public Int64? ToolBinID { get; set; }

        public double? Quantity { get; set; }
        public double? Cost { get; set; }
        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }


        public DateTime? ReceivedDate { get; set; }


        [AllowHtml]
        public string PackSlipNumber { get; set; }
        [AllowHtml]
        public string UDF1 { get; set; }
        [AllowHtml]
        public string UDF2 { get; set; }
        [AllowHtml]
        public string UDF3 { get; set; }
        [AllowHtml]
        public string UDF4 { get; set; }
        [AllowHtml]
        public string UDF5 { get; set; }

        public Guid GUID { get; set; }
        public Guid? ToolGUID { get; set; }
        public Guid? ToolAssetOrderDetailGUID { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public Int64? CreatedBy { get; set; }
        public Int64? LastUpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsReceived { get; set; }

        public Int64? CompanyID { get; set; }
        public Int64? Room { get; set; }

        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }

        public Int32 ToolType { get; set; }

        public string Comment { get; set; }

        public string RoomName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string BinNumber { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true);
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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true);
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
        public bool? SerialNumberTracking { get; set; }
    }
    public class ToFillPreReceiveToolDTO
    {
        // Item#, Order# Release# ItemType is only Display Perpose
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string OrderNumber { get; set; }
        public string ReleaseNumber { get; set; }

        public string ToolTypeSerialLot { get; set; }
        public bool SerialNumberTracking { get; set; }
        public bool IsModelShow { get; set; }
        public int OrderStatus { get; set; }
        public double? RequestedQty { get; set; }
        public double? ApproveQty { get; set; }
        public double? ReceiveQty { get; set; }
        public string BinNumber { get; set; }
        public string Location { get; set; }
        public string ReceivedDate { get; set; }
        public double? Cost { get; set; }
        public string PackSlipNumber { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }

        public Guid ToolGUID { get; set; }
        public Guid OrderDetailGUID { get; set; }
        public Guid OrderGUID { get; set; }
        public List<ToFillReceiveDetailToolDTO> MakePreReceiveDetail { get; set; }
        public string Comment { get; set; }

        public string Description { get; set; }

    }
    public class ToFillReceiveDetailToolDTO
    {
        public double Quantity { get; set; }
        public string SerialNumber { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }

    }

}
