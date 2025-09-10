using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class NewReceiveOrderImportDTO
    {
        public long ID { get; set; }
        public string ImportBulkID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? UserID { get; set; }
        public string EnterpriseName { get; set; }
        public Guid? OrderGUID { get; set; }
        public Guid? ItemGUID { get; set; }
        public string ItemNumber { get; set; }

        public string OrderNumber { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public NewReceiveOrderItemImportDTO[] ReceiveOrderItem { get; set; } 
        public string ErrorMessage { get; set; }
        public string StatusMessage { get; set; }
        public string PackslipNumber { get; set; }
        public string ShippingTrackNumber { get; set; }
        public string UserName { get; set; }
        public DateTime? ImportDate { get; set; }
        public string ImportedFrom { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? ProcessDate { get; set; }
        public bool? IsSuccess { get; set; }
        public string StrImportDate { get; set; }
        public string StrProcessDate { get; set; }
    }
    
    public class NewReceiveOrderItemImportDTO
    {
        public long ID { get; set; }
        public string ImportBulkID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? UserID { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string UserName { get; set; }
        public Guid? OrderDetailGUID { get; set; }
        public Guid? OrderGUID { get; set; }
        public Guid? ItemGUID { get; set; }

        public string ItemNumber { get; set; }
        public string ReceiveBinName { get; set; }
        public double? Cost { get; set; }
        public double Quantity { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string EditedFrom { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusMessage { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public ReceivedOrderTransferDetailImportDTO[] ReceivedItemTransferDetail { get; set; }
        public DateTime? ImportDate { get; set; }
        public string ImportedFrom { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? ProcessDate { get; set; }
        public bool? IsSuccess { get; set; }
        public string StrImportDate { get; set; }
        public string StrProcessDate { get; set; }
    }

    public class ReceivedOrderTransferDetailImportDTO
    {
        public long ID { get; set; }
        public string ImportBulkID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? UserID { get; set; }
        public Guid? OrderDetailGUID { get; set; }
        public Guid? ItemGUID { get; set; }
        public string ItemNumber { get; set; }
        public string ReceiveBinName { get; set; }
        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public double? Cost { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusMessage { get; set; }
        public double? Quantity { get; set; }
        public DateTime? ImportDate { get; set; }
        public string ImportedFrom { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? ProcessDate { get; set; }
        public bool? IsSuccess { get; set; }
        public string StrImportDate { get; set; }
        public string StrProcessDate { get; set; }
    }
}
