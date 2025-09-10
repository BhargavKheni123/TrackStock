using System;

namespace eTurns.DTO
{
    public class CountLineItemDetailDTO
    {
        public Nullable<System.Int64> ID { get; set; }
        public Nullable<System.Guid> ItemGUID { get; set; }
        public System.String ItemNumber { get; set; }
        public System.String ItemDescription { get; set; }
        public Nullable<System.Int32> ItemType { get; set; }
        public Nullable<System.Boolean> Consignment { get; set; }
        public System.String Comment { get; set; }
        public Nullable<System.Int64> BinID { get; set; }
        public System.String BinNumber { get; set; }
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }
        public Nullable<System.Double> AvailableQuantity { get; set; }
        public Nullable<System.Double> CountCustomerOwnedQuantity { get; set; }
        public Nullable<System.Double> ConsignedQuantity { get; set; }
        public Nullable<System.Double> CountConsignedQuantity { get; set; }
        public Nullable<System.Boolean> SerialNumberTracking { get; set; }
        public Nullable<System.Boolean> LotNumberTracking { get; set; }
        public System.String LotSerialNumber { get; set; }
        public System.String LotNumber { get; set; }
        public System.String SerialNumber { get; set; }
        public System.String Expiration { get; set; }
        public System.String Received { get; set; }
        public Nullable<System.Boolean> DateCodeTracking { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public Nullable<System.Guid> GUID { get; set; }
        public Nullable<System.Boolean> IsStagingLocationCount { get; set; }
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
        public Nullable<System.Boolean> IsConsigned { get; set; }
        public string InventoryCountDetailDescription { get; set; }
        public Guid? ProjectSpendGUID { get; set; }

        public Nullable<System.Double> ConsignedDifference { get; set; }
        public Nullable<System.Double> CusOwnedDifference { get; set; }

        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }
        public Nullable<System.Guid> SupplierAccountGuid { get; set; }
    }

    public class CountLineItemDetailDTOForAutoComplete
    {
        public System.String LotSerialNumber { get; set; }
        public System.String LotSerialNumberWithoutDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<System.Double> AvailableQuantity { get; set; }
    }
}
