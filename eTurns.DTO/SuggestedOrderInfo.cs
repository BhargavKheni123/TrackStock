using System;

namespace eTurns.DTO
{
    public class SuggestedOrderInfo
    {
        public double AvailableQuantity { get; set; }
        public double ItemOnHandQuantity { get; set; }
        public double ItemOnTransferInQuantity { get; set; }
        public double ItemOnTransferOutQuantity { get; set; }
        public double ItemOrderedQuantity { get; set; }
        public double ItemRequisitionQuantity { get; set; }
        public double ItemCriticalQuantity { get; set; }
        public double ItemMinimumQuantity { get; set; }
        public double ItemMaximumQuantity { get; set; }
        public double ItemSuggestedOrderQuantity { get; set; }
        public long CompanyId { get; set; }
        public long RoomId { get; set; }
        public double TotalCartQtyForItem { get; set; }
        public double CartQuantity { get; set; }
        public bool? IsItemLevelMinMaxQtyRequired { get; set; }
        public bool ItemIsTranser { get; set; }
        public bool ItemIsPurchase { get; set; }
        public string ReplenishType { get; set; }
        public bool SuggestedOrderRoomFlag { get; set; }
        public bool SuggestedTransferRoomFlag { get; set; }
        public bool IsEnforceDefaultReorderQuantity { get; set; }
        public double DefaultReorderQuantity { get; set; }
        public double moduloval { get; set; }
        public long devideval { get; set; }
        public long BinId { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public string BinNumber { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long SupplierID { get; set; }
        public Boolean IsSupplierReceivesKitComponents { get; set; }
        public Guid ItemGUID { get; set; }
        public int ItemType { get; set; }
        public double QtyOnDemand { get; set; }

        public double ItemTableSO { get; set; }
    }

    public class RPT_SuggestedOrderDTO
    {
        public double? AverageCost { get; set; }
        public double? AverageUsage { get; set; }
        public string BarcodeImage_ItemNumber { get; set; }
        public string BarcodeImage_OrderItemBin { get; set; }
        public string BinName { get; set; }
        public string CartStatus { get; set; }
        public string CategoryName { get; set; }
        public Int64? CompanyID { get; set; }
        public string CompanyInfo { get; set; }
        public string CompanyName { get; set; }
        public string Consignment { get; set; }
        public int? CostDecimalPoint { get; set; }
        public string CostUOM { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public double? CriticalQuantity { get; set; }
        public string CurrentDateTime { get; set; }
        public string DateCodeTracking { get; set; }
        public string DefaultLocationName { get; set; }
        public double? DefaultPullQuantity { get; set; }
        public double? DefaultReorderQuantity { get; set; }
        public double? ExtendedCost { get; set; }
        public string GLAccount { get; set; }
        public Guid GUID { get; set; }
        public Int64 ID { get; set; }
        public int? InventoryClassification { get; set; }
        public string InventoryClassificationName { get; set; }
        public string IsArchived { get; set; }
        public string IsAutoMatedEntry { get; set; }
        public string IsBuildBreak { get; set; }
        public string IsDeleted { get; set; }
        public string IsKitComponent { get; set; }
        public string IsPurchase { get; set; }
        public string IsTransfer { get; set; }
        public string ItemBlanketPO { get; set; }
        public double? ItemCost { get; set; }
        public string ItemDescription { get; set; }
        public Guid? ItemGUID { get; set; }
        public Int64 ItemID { get; set; }
        public double? ItemInTransitquantity { get; set; }
        public string ItemNumber { get; set; }
        public string ItemSupplier { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string ItemUniqueNumber { get; set; }
        public string LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }
        public int? LeadTimeInDays { get; set; }
        public string LongDescription { get; set; }
        public string LotNumberTracking { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerNumber { get; set; }
        public double? Markup { get; set; }
        public double? MaximumQuantity { get; set; }
        public double? MinimumQuantity { get; set; }
        public double? OnHandQuantity { get; set; }
        public double? OnOrderQuantity { get; set; }
        public double? OnReturnQuantity { get; set; }
        public double? OnTransferQuantity { get; set; }
        public double? PricePerTerm { get; set; }
        public double? Quantity { get; set; }
        public int? QuantityDecimalPoint { get; set; }
        public string ReplenishType { get; set; }
        public double? RequisitionedQuantity { get; set; }
        public Int64 RoomID { get; set; }
        public string RoomInfo { get; set; }
        public string RoomName { get; set; }
        public double? SellPrice { get; set; }
        public string SerialNumberTracking { get; set; }
        public double? StagedQuantity { get; set; }
        public double? SuggestedOrderQuantity { get; set; }
        public string SupplierPartNo { get; set; }
        public string Taxable { get; set; }
        public double? Total { get; set; }
        public double? Turns { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string Unit { get; set; }
        public string UNSPSC { get; set; }
        public string UPC { get; set; }
        public string WeightPerPiece { get; set; }

    }
    public class RPT_AssetMasterDTO
    {
        public string Serial { get; set; }
        public Int64 Id { get; set; }
        public string AssetName { get; set; }
        public string Description { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string CategoryName { get; set; }
        public string IsDeleted { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string PurchaseDate { get; set; }
        public string SuggestedMaintenanceDate { get; set; }
        public double DepreciatedValue { get; set; }
        public double PurchasePrice { get; set; }
        public string RoomInfo { get; set; }
        public int? QuantityDecimalPoint { get; set; }
        public int? CostDecimalPoint { get; set; }
        public string CurrentDateTime { get; set; }
        public Guid GUID { get; set; }
    }
    public class RPT_CountMasterDTO
    {
        public string CountName { get; set; }
        public Int64 Id { get; set; }
        public string AverageCost { get; set; }
        public string Markup { get; set; }
        public string SellPrice { get; set; }
        public string MinimumQuantity { get; set; }
        public string MaximumQuantity { get; set; }
        public string OnOrderQuantity { get; set; }
        public string SuggestedOrderQuantity { get; set; }
        public string AverageUsage { get; set; }
        public string Description { get; set; }
        public string IsDeleted { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string UserName { get; set; }
        public string SuggestedMaintenanceDate { get; set; }
        public double DepreciatedValue { get; set; }
        public double PurchasePrice { get; set; }
        public string RoomInfo { get; set; }
        public int? QuantityDecimalPoint { get; set; }
        public int? CostDecimalPoint { get; set; }
        public string CurrentDateTime { get; set; }
        public Guid GUID { get; set; }
        public string SupplierPartNo { get; set; }
        public string SupplierName { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string manufacturernumber { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public string UNSPSC { get; set; }
        public string ItemNumber { get; set; }
        public string ReleaseNumber { get; set; }
    }
    public class RPT_MaterialStagingDTO
    {
        public Int64 Id { get; set; }
        public string StagingName { get; set; }
        public string MaterialStagingDesc { get; set; }
        public double StagingQuantity { get; set; }
        public string StagingLocationName { get; set; }
        public string IsArchived { get; set; }
        public string BinNumber { get; set; }

        public double? Total { get; set; }
        public string IsDeleted { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public Guid GUID { get; set; }
        public string ItemNumber { get; set; }
        public string CategoryName { get; set; }
        public string ItemSupplier { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string UNSPSC { get; set; }
        public string ManufacturerName { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }

        public int? QuantityDecimalPoint { get; set; }
        public int? CostDecimalPoint { get; set; }
        public string CurrentDateTime { get; set; }

    }

    public class RPT_SuggExpDateOrders
    {

        public string ItemNumber { get; set; }
        public Guid ItemGUID { get; set; }
        public string Supplier { get; set; }
        public string SupplierPartNo { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerPartNo { get; set; }
        public string Category { get; set; }
        public string UNSPSC { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }

    }

}
