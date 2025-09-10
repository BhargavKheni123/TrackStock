using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    [Serializable]
    public class OrderDetailsDTO : ItemViewFields
    {
        public System.Int64 ID { get; set; }
        public Nullable<System.Int64> Bin { get; set; }
        public new Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<System.Int64> Room { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }

        public Guid GUID { get; set; }
        public Nullable<Guid> OrderGUID { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }

        [Display(Name = "RequiredDate", ResourceType = typeof(ResOrder))]
        public Nullable<System.DateTime> RequiredDate { get; set; }

        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "LastUpdated", ResourceType = typeof(ResOrder))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        public Nullable<System.DateTime> LastEDIDate { get; set; }

        [Display(Name = "RequestedQuantity", ResourceType = typeof(ResOrder))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<double> RequestedQuantity { get; set; }

        [Display(Name = "ReceivedQuantity", ResourceType = typeof(ResOrder))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<double> ReceivedQuantity { get; set; }

        [Display(Name = "ApprovedQuantity", ResourceType = typeof(ResOrder))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<double> ApprovedQuantity { get; set; }

        public Nullable<double> InTransitQuantity { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<bool> IsEDIRequired { get; set; }
        public Nullable<bool> IsEDISent { get; set; }

        [Display(Name = "ASNNumber", ResourceType = typeof(ResOrder))]
        public string ASNNumber { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public string BinName { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResItemMaster))]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        public System.String EditedFrom { get; set; }


        public bool IsHistory { get; set; }
        public int TotalRecords { get; set; }

        public DateTime? ChangeOrderDetailCreated { get; set; }
        public DateTime? ChangeOrderDetailLastUpdated { get; set; }
        public Int64? ChangeOrderDetailCreatedBy { get; set; }
        public Int64? ChangeOrderDetailLastUpdatedBy { get; set; }
        public Guid ChangeOrderDetailGUID { get; set; }
        public Int64 ChangeOrderDetailID { get; set; }
        public Guid? ChangeOrderMasterGUID { get; set; }
        public System.String ODPackSlipNumbers { get; set; }
        public bool IsOnlyFromUI { get; set; }

        public bool? IsCloseItem { get; set; }

        private string _createdDate;
        private string _updatedDate;
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

        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedDate))
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(LastUpdated, true);
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
            set
            { this._ReceivedOnWeb = value; }
        }
        [Display(Name = "RequiredDate", ResourceType = typeof(ResOrder))]
        public string RequiredDateStr { get; set; }
        public string CostUOM { get; set; }
        public string OrderUOM { get; set; }

        public string LineNumber { get; set; }
        public string ControlNumber { get; set; }
        public bool IsPackslipMandatoryAtReceive { get; set; }
        public string Comment { get; set; }

        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }

        public double? OrderCost { get; set; }

        public Guid? tempDetailsGUID { get; set; }
        public Nullable<double> OrderLineItemExtendedCost { get; set; }
        public Nullable<double> OrderLineItemExtendedPrice { get; set; }

        public Nullable<double> RequestedQuantityUOM { get; set; }
        public Nullable<double> ReceivedQuantityUOM { get; set; }
        public Nullable<double> ApprovedQuantityUOM { get; set; }
        public Nullable<double> InTransitQuantityUOM { get; set; }

        public double OrderPrice { get; set; }

        public int? OrderType { get; set; }

        //public Nullable<System.Int32> OrderUOMValue { get; set; }
        public bool IsAllowOrderCostuom { get; set; }
        public Nullable<System.Int32> OrderUOMValue_LineItem { get; set; }
        public bool IsAllowOrderCostuom_LineItem { get; set; }
        public int OrderStatus { get; set; }
        public Guid? MaterialStagingGUID { get; set; }
        public long? StagingID { get; set; }

        [Display(Name = "OrderItemCost", ResourceType = typeof(ResOrder))]
        public Nullable<double> ItemCost { get; set; }

        public Nullable<double> ItemCostUOM { get; set; }
        public Nullable<Int32> ItemCostUOMValue { get; set; }
        public Nullable<double> ItemMarkup { get; set; }
        public Nullable<double> ItemSellPrice { get; set; }

        public Guid? QuickListGUID { get; set; }

        public List<FileAttachmentReceiveList> attachmentfileNames { get; set; }
        public Nullable<bool> IsBackOrdered { get; set; }
        public Nullable<int> POItemLineNumber { get; set; }
        public bool hasPOItemNumber { get; set; }
        public bool IsShowNormalItemPopUp { get; set; }

        [Display(Name = "OrderLineException", ResourceType = typeof(ResOrder))]
        public Nullable<bool> OrderLineException { get; set; }

        [Display(Name = "OrderLineExceptionDesc", ResourceType = typeof(ResOrder))]
        public string OrderLineExceptionDesc { get; set; }
    }

    [Serializable]
    public class ItemViewFields
    {

        public Guid ItemViewGUID { get; set; }
        public Int64 ItemID { get; set; }
        public Nullable<Int64> ManufacturerID { get; set; }
        public Nullable<Int64> SupplierID { get; set; }
        public Nullable<Int64> CategoryID { get; set; }
        public Nullable<Int64> GLAccountID { get; set; }
        public Nullable<Int64> UOMID { get; set; }
        public Nullable<Int64> DefaultLocation { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> ItemLastUpdatedBy { get; set; }
        public Nullable<Int64> ItemRoom { get; set; }
        public Nullable<System.Int32> CostUOMValue { get; set; }
        public Nullable<System.Int32> OrderUOMValue { get; set; }
        public bool Trend { get; set; }
        public bool Taxable { get; set; }
        public bool Consignment { get; set; }
        public bool SerialNumberTracking { get; set; }
        public bool LotNumberTracking { get; set; }
        public bool DateCodeTracking { get; set; }

        public Nullable<bool> IsTransfer { get; set; }
        public Nullable<bool> IsPurchase { get; set; }
        public Nullable<bool> ItemIsDeleted { get; set; }
        public Nullable<bool> ItemIsArchived { get; set; }
        public Nullable<bool> IsItemLevelMinMaxQtyRequired { get; set; }
        public Nullable<bool> IsEnforceDefaultReorderQuantity { get; set; }
        public Nullable<bool> IsBuildBreak { get; set; }

        public Nullable<DateTime> ItemCreated { get; set; }
        public Nullable<DateTime> ItemUpdated { get; set; }

        public Nullable<double> StagedQuantity { get; set; }
        public Nullable<double> ItemInTransitQuantity { get; set; }
        public Nullable<double> OnOrderQuantity { get; set; }
        public Nullable<double> OnQuotedQuantity { get; set; }
        public Nullable<double> OnOrderInTransitQuantity { get; set; }
        public Nullable<double> OnReturnQuantity { get; set; }
        public Nullable<double> OnTransferQuantity { get; set; }
        public Nullable<double> SuggestedOrderQuantity { get; set; }
        public Nullable<double> RequisitionedQuantity { get; set; }
        public Nullable<double> AverageUsage { get; set; }
        public Nullable<double> Turns { get; set; }
        public Nullable<double> OnHandQuantity { get; set; }
        public Nullable<double> WeightPerPiece { get; set; }
        public Nullable<double> PackingQuantity { get; set; }
        public Nullable<bool> IsEnforceBinDefaultReorderQuantity { get; set; }
        public double BinDefaultReorderQuantity { get; set; }
        public double DefaultReorderQuantity { get; set; }
        public double DefaultPullQuantity { get; set; }
        public double CriticalQuantity { get; set; }
        public double MinimumQuantity { get; set; }
        public double MaximumQuantity { get; set; }


        public Nullable<int> LeadTimeInDays { get; set; }
        public Nullable<int> InventoryClassification { get; set; }
        public int ItemType { get; set; }

        public Nullable<double> Markup { get; set; }
        public Nullable<double> PricePerTerm { get; set; }
        public Nullable<double> Cost { get; set; }
        public Nullable<double> ExtendedCost { get; set; }
        public Nullable<double> AverageCost { get; set; }
        public Nullable<double> SellPrice { get; set; }

        public string ItemNumber { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string SupplierName { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public string ItemDescription { get; set; }
        public string LongDescription { get; set; }
        public string Category { get; set; }
        public string GLAccount { get; set; }
        public string Unit { get; set; }
        public string Link1 { get; set; }
        public string Link2 { get; set; }
        public string ItemUniqueNumber { get; set; }
        public string ImagePath { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string ItemUDF6 { get; set; }
        public string ItemUDF7 { get; set; }
        public string ItemUDF8 { get; set; }
        public string ItemUDF9 { get; set; }
        public string ItemUDF10 { get; set; }
        public string ItemCreatedByName { get; set; }
        public string ItemUpdatedByName { get; set; }
        public string ItemRoomName { get; set; }
        public string IsLotSerialExpiryCost { get; set; }
        public string DefaultLocationName { get; set; }
        public string ItemBlanketPO { get; set; }

        private string _itemcreatedDate;
        private string _itemupdatedDate;
        public string ItemCreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_itemcreatedDate))
                {
                    _itemcreatedDate = FnCommon.ConvertDateByTimeZone(ItemCreated, true);
                }
                return _itemcreatedDate;
            }
            set { this._itemcreatedDate = value; }
        }

        public string ItemUpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_itemupdatedDate))
                {
                    _itemupdatedDate = FnCommon.ConvertDateByTimeZone(ItemUpdated, true);
                }
                return _itemupdatedDate;
            }
            set { this._itemupdatedDate = value; }
        }
        public bool IsItemActive { get; set; }
        public bool IsItemOrderable { get; set; }
    }

    public class OrderDetailTrackingDTO
    {
        public System.Int64 ID { get; set; }
        public Nullable<System.Int64> Room { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }
        public Guid? OrderDetailID { get; set; }

        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }
        public string PackSlipNumber { get; set; }
        public double? Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }

        private string _createdDate;
        private string _updatedDate;
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

        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedDate))
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(LastUpdated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }
    }
    public class RPT_OrderWithLineItems
    {
        public Int64 ID { get; set; }
        public string OrderNumber { get; set; }
        public string Replenish_Order { get; set; }
        public string StagingName { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }

        public int NoOfLineItems { get; set; }
        public Int64? ChangeOrderRevisionNo { get; set; }
        public double? Total { get; set; }
        public string Comment { get; set; }
        public string Customer { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public Guid GUID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string ShipVia { get; set; }
        public string ShippingVendor { get; set; }
        public string OrderSupplier { get; set; }
        public string Bin { get; set; }
        public string PackSlipNumber { get; set; }
        public double? OrderCost { get; set; }
        public string RequiredDate { get; set; }
        public string OrderDate { get; set; }
        public string LastEDIDate { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string OrderStatus { get; set; }
        public char OrderStatusChar
        {
            get
            {
                return FnCommon.GetOrderStatusChar(OrdStatus);
            }
        }
        public string OrderType { get; set; }
        public string IsDeleted { get; set; }
        public string IsArchived { get; set; }
        public string IsEDIRequired { get; set; }
        public string IsEDISent { get; set; }
        public Int64? RoomID { get; set; }
        public Int64? CompanyID { get; set; }
        public double? RequestedQuantity { get; set; }
        public double? ApprovedQuantity { get; set; }
        public double? ReceivedQuantity { get; set; }
        public double? OrderInTransitQuantity { get; set; }
        public int OrdStatus { get; set; }
        public int OrdType { get; set; }
        public string ItemNumber { get; set; }
        public string ItemUniqueNumber { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public string ItemDescription { get; set; }
        public string LongDescription { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string ItemBlanketPO { get; set; }
        public string ItemSupplier { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public string GLAccount { get; set; }
        public string Unit { get; set; }
        public string DefaultLocationName { get; set; }
        public string InventoryClassificationName { get; set; }
        public string CostUOM { get; set; }
        public int? InventoryClassification { get; set; }
        public int? LeadTimeInDays { get; set; }
        public string ItemTypeName { get; set; }
        public double? ItemCost { get; set; }
        public double? SellPrice { get; set; }
        public double? ExtendedCost { get; set; }
        public double? AverageCost { get; set; }
        public double? PricePerTerm { get; set; }
        public double? OnHandQuantity { get; set; }
        public double? StagedQuantity { get; set; }
        public double? ItemInTransitquantity { get; set; }
        public double? RequisitionedQuantity { get; set; }
        public double? CriticalQuantity { get; set; }
        public double? MinimumQuantity { get; set; }
        public double? MaximumQuantity { get; set; }
        public double? DefaultReorderQuantity { get; set; }
        public double? DefaultPullQuantity { get; set; }
        public string AverageUsage { get; set; }
        public string Turns { get; set; }
        public string Markup { get; set; }
        public string WeightPerPiece { get; set; }
        public string Consignment { get; set; }
        public string IsTransfer { get; set; }
        public string IsPurchase { get; set; }
        public string SerialNumberTracking { get; set; }
        public string LotNumberTracking { get; set; }
        public string DateCodeTracking { get; set; }
        public string IsBuildBreak { get; set; }
        public string Taxable { get; set; }
        public Int64? ItemID { get; set; }
        public Guid? ItemGUID { get; set; }
        public string RoomInfo { get; set; }
        public string CompanyInfo { get; set; }
        public string BarcodeImage_ItemNumber { get; set; }
        public string BarcodeImage_OrderItemBin { get; set; }
        public string BarcodeImage_OrderNumber { get; set; }
        public int? QuantityDecimalPoint { get; set; }
        public int? CostDecimalPoint { get; set; }
        public Int64? CategoryID { get; set; }
        public Int64? SupplierID { get; set; }
        public Int64? ManufacturerID { get; set; }
        public Guid? MaterialStagingGUID { get; set; }
        public string BinUDF1 { get; set; }
        public string BinUDF2 { get; set; }
        public string BinUDF3 { get; set; }
        public string BinUDF4 { get; set; }
        public string BinUDF5 { get; set; }
    }

    public class RPT_OrderSummaryWithLineItems
    {
        public string OrderNumber { get; set; }
        public string Itemnumber { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public Guid? OrderGuid { get; set; }
        public Guid? ItemGuid { get; set; }

        public string OrderUDF1 { get; set; }
        public string OrderUDF2 { get; set; }
        public string OrderUDF3 { get; set; }
        public string OrderUDF4 { get; set; }
        public string OrderUDF5 { get; set; }
        public string OrderSupplier { get; set; }
        public string OrderManufacturer { get; set; }
    }

    public class RPT_OrderItemSummary
    {
        public string ItemNumber { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public Guid ItemGuid { get; set; }
        public Int64? CategoryID { get; set; }
        public Int64? SupplierID { get; set; }
        public Int64? ManufacturerID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPartNo { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerNumber { get; set; }
        public string CategoryName { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string BinUDF1 { get; set; }
        public string BinUDF2 { get; set; }
        public string BinUDF3 { get; set; }
        public string BinUDF4 { get; set; }
        public string BinUDF5 { get; set; }
        public string InvoiceBranch { get; set; }
    }

    public class RPT_ToolAssetOrderWithLineItems
    {
        public Int64 ID { get; set; }
        public string ToolAssetOrderNumber { get; set; }

        public string CreatedByName { get; set; }
        public string LastUpdatedByName { get; set; }



        public double? Total { get; set; }

        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public Guid GUID { get; set; }



        public string BinName { get; set; }

        public string RequiredDate { get; set; }

        public string LastEDIDate { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }


        public string IsDeleted { get; set; }
        public string IsArchived { get; set; }
        public string IsEDIRequired { get; set; }
        public string IsEDISent { get; set; }

        public double? RequestedQuantity { get; set; }
        public double? ApprovedQuantity { get; set; }
        public double? ReceivedQuantity { get; set; }



        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string Description { get; set; }
        public string ToolUDF1 { get; set; }
        public string ToolUDF2 { get; set; }
        public string ToolUDF3 { get; set; }
        public string ToolUDF4 { get; set; }
        public string ToolUDF5 { get; set; }
        public double? Quantity { get; set; }
        public string IsBuildBreak { get; set; }

        public Guid? ToolGUID { get; set; }
        public string RoomInfo { get; set; }
        public string CompanyInfo { get; set; }

    }

    public class ItemToReturnDTO
    {
        public long EnterpriseId { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public long ItemID { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public List<ItemLocationLotSerialDTO> lstItemPullDetails { get; set; }
        public List<PullErrorInfo> ErrorList { get; set; }
        public double TotalCustomerOwnedTobePulled { get; set; }
        public double TotalConsignedTobePulled { get; set; }
        public Guid ItemGUID { get; set; }
        public string ItemNumber { get; set; }
        public long BinID { get; set; }
        public string ReturnDate { get; set; }
        public Int64 OrderDetailID { get; set; }
        public Guid OrderDetailGUID { get; set; }
        public string BinNumber { get; set; }
        public string PackSlipNumber { get; set; }
        public double ReturnedQty { get; set; }
        public double ApprovedQty { get; set; }
        public double QtyToReturn { get; set; }
        public string OrderNumber { get; set; }
        public string ReleaseNumber { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string Supplier { get; set; }
        public Guid OrderGuid { get; set; }
        public bool IsLotTrack { get; set; }
        public bool IsSRTrack { get; set; }
        public bool IsDCTrack { get; set; }
        public string InventoryConsuptionMethod { get; set; }
        //public List<ItemToReturnDetailDTO> ItemWithQtyDetail { get; set; }
        public long CreatedBy { get; set; }
        public long LastUpdatedBy { get; set; }
        public bool CanOverrideProjectLimits { get; set; }
        public bool ValidateProjectSpendLimits { get; set; }
    }

    public class ItemToReturnDetailDTO
    {
        public Int64 BinID { get; set; }
        public double AvailableQty { get; set; }
        public double ConsQty { get; set; }
        public double CustQty { get; set; }
        public double QtyToReturn { get; set; }
        public string SerialNumber { get; set; }
        public string BinName { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime RecievedDate { get; set; }

    }

    public class OrderLineItemDetailDTO
    {
        public Int64 ID { get; set; }
        public Guid? GUID { get; set; }

        public System.String Supplier { get; set; }
        public System.String OrderNumber { get; set; }
        public System.String ReleaseNumber { get; set; }
        public Nullable<DateTime> RequiredDate { get; set; }
        public System.String OrderStatus { get; set; }
        public System.String StagingName { get; set; }
        public System.String OrderComment { get; set; }
        public System.String CustomerName { get; set; }
        public System.String PackSlipNumber { get; set; }
        public System.String ShippingTrackNumber { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResOrder))]
        public System.String OrderUDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResOrder))]
        public System.String OrderUDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResOrder))]
        public System.String OrderUDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResOrder))]
        public System.String OrderUDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResOrder))]
        public System.String OrderUDF5 { get; set; }

        public System.String ShipVia { get; set; }
        public System.String OrderType { get; set; }
        public System.String ShippingVendor { get; set; }
        //public System.String AccountNumber { get; set; }
        public System.String SupplierAccount { get; set; }
        public System.String ItemNumber { get; set; }
        public System.String Bin { get; set; }
        public Nullable<System.Double> RequestedQty { get; set; }
        public Nullable<System.Double> ReceivedQty { get; set; }
        public System.String ASNNumber { get; set; }
        public Nullable<System.Double> ApprovedQty { get; set; }
        public Nullable<System.Double> InTransitQty { get; set; }
        public Nullable<Boolean> IsCloseItem { get; set; }
        public System.String LineNumber { get; set; }
        public System.String ControlNumber { get; set; }
        public System.String ItemComment { get; set; }

        [Display(Name = "OrdDtlUDF1", ResourceType = typeof(ResOrder))]
        public System.String LineItemUDF1 { get; set; }

        [Display(Name = "OrdDtlUDF2", ResourceType = typeof(ResOrder))]
        public System.String LineItemUDF2 { get; set; }

        [Display(Name = "OrdDtlUDF3", ResourceType = typeof(ResOrder))]
        public System.String LineItemUDF3 { get; set; }

        [Display(Name = "OrdDtlUDF4", ResourceType = typeof(ResOrder))]
        public System.String LineItemUDF4 { get; set; }

        [Display(Name = "OrdDtlUDF5", ResourceType = typeof(ResOrder))]
        public System.String LineItemUDF5 { get; set; }

        //
        public Nullable<System.Double> RequestedQtyUOM { get; set; }
        public Nullable<System.Double> ReceivedQtyUOM { get; set; }
        public Nullable<System.Double> ApprovedQtyUOM { get; set; }
        public Nullable<System.Double> InTransitQtyUOM { get; set; }
        public Boolean IsAllowOrderCostuom { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public string SalesOrder { get; set; }

    }

    public class OrderDetailsForSolum
    {
        public Int64 ID { get; set; }
        public Nullable<double> InTransitQuantity { get; set; }
        public bool IsBackOrdered { get; set; }
        public string SupplierPartNo { get; set; }
        public Guid? OrderDetailGUID { get; set; }
        public Guid? OrderGUID { get; set; }
        public Guid? ItemGUID { get; set; }
        public Int64? RoomID { get; set; }
        public Int64? CompanyID { get; set; }
        public Int32? OrderStatus { get; set; }
        public bool? IsReopenedOrder { get; set; }
        public Nullable<double> OnOrderQuantity { get; set; }
        public bool DisplayOrderException { get; set; }


    }
    public class PostProcessOrderDetails
    {
        public Guid? GUID { get; set; }
        public Guid? ItemGUID { get; set; }
        public Int64? RoomID { get; set; }
        public Int64? CompanyID { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsError { get; set; }
    }
}


