using System;

namespace eTurns.DTO
{
    public class ToolAssetCountLineItemDetailDTO
    {
        public Nullable<System.Int64> ID { get; set; }
        public Nullable<System.Guid> ToolGUID { get; set; }
        public System.String ToolName { get; set; }
        public System.String ToolDescription { get; set; }
        //public Nullable<System.Int64> ToolType { get; set; }        
        public System.String Comment { get; set; }
        public Nullable<System.Int64> ToolBinID { get; set; }
        public System.String Location { get; set; }
        public Nullable<System.Double> Quantity { get; set; }
        public Nullable<System.Double> AvailableQuantity { get; set; }
        public Nullable<System.Double> CountQuantity { get; set; }
        public Nullable<System.Boolean> SerialNumberTracking { get; set; }
        public Nullable<System.Boolean> LotNumberTracking { get; set; }
        public System.String LotSerialNumber { get; set; }
        public System.String SerialNumber { get; set; }
        public System.String Received { get; set; }
        public Nullable<System.Boolean> DateCodeTracking { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public Nullable<System.Guid> GUID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<System.Boolean> IsDeleted { get; set; }
        public Nullable<System.Boolean> IsArchived { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }
        public Nullable<System.Int64> RoomID { get; set; }
        public Nullable<System.Guid> CountGUID { get; set; }
        public Nullable<System.Guid> CountDetailGUID { get; set; }
        public Nullable<System.DateTime> ReceivedOn { get; set; }
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public Nullable<System.Boolean> IsApplied { get; set; }
        public Nullable<System.Boolean> IsValidObject { get; set; }
        public Nullable<System.Boolean> IsAdd { get; set; }
        public Nullable<System.Boolean> IsUpdate { get; set; }
        public string InventoryCountDetailDescription { get; set; }
        public Guid? ProjectSpendGUID { get; set; }

        public Nullable<System.Double> QuantityDifference { get; set; }
    }
}
