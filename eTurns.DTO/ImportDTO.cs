using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class ImportDTO
    {
        public class ImportStatus_DTO
        {
            public Int64 ID { get; set; }
            public string ImportType { get; set; }
            public string ImportBulkID { get; set; }
            public Int32 TotalRecords { get; set; }
            public Nullable<DateTime> ImportCreatedDate { get; set; }
            public bool IsAllItemInserted { get; set; }
            public bool IsImportStarted { get; set; }
            public bool IsImportCompleted { get; set; }
            public Nullable<DateTime> ImportComplitionDate { get; set; }
            public Int32 SuccessRecords { get; set; }

        }

        public class Import_PULL_DTO
        {
            public Int64 ID { get; set; }
            public string ImportBulkID { get; set; }
            public Nullable<Int64> EnterpriseID { get; set; }
            public Nullable<Int64> CompanyID { get; set; }
            public Nullable<Int64> RoomID { get; set; }
            public Nullable<Int64> UserID { get; set; }
            public Nullable<Int64> ItemID { get; set; }
            public Nullable<Int64> ProjectMasterID { get; set; }
            public Nullable<Int64> WorkOrderID { get; set; }
            public string EnterpriseName { get; set; }
            public string CompanyName { get; set; }
            public string RoomName { get; set; }
            public string UserName { get; set; }
            public string ItemNumber { get; set; }
            public double PullQuantity { get; set; }
            public string PullBin { get; set; }
            public string ProjectSpend { get; set; }
            public string WorkOrder { get; set; }
            public string UDF1 { get; set; }
            public string UDF2 { get; set; }
            public string UDF3 { get; set; }
            public string UDF4 { get; set; }
            public string UDF5 { get; set; }
            public Nullable<bool> IsCredit { get; set; }
            public Nullable<DateTime> ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public Nullable<bool> IsProcessed { get; set; }
            public Nullable<DateTime> ProcessDate { get; set; }
            public Nullable<bool> IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string strImportDate { get; set; }
            public string strProcessDate { get; set; }
            public Guid? ImportGUID { get; set; }
            public string EditedFrom { get; set; }

            public string PullOrderNumber { get; set; }

            public Guid? WOGUID { get; set; }

            public Guid? PullGUID { get; set; }

            public List<Import_PULLDetails_DTO> PullDetails { get; set; }

            public string PullGUIDs { get; set; }

            public string EnterpriseDBName { get; set; }
            public Guid? SupplierAccountGUID { get; set; }
            public double? SellPrice { get; set; }
        }

        #region for add Serial number into Insert Pull service

        public class Import_PULLDetails_DTO
        {

            public Int64 ID { get; set; }
            public string ImportBulkID { get; set; }
            public Nullable<Int64> EnterpriseID { get; set; }
            public Nullable<Int64> CompanyID { get; set; }
            public Nullable<Int64> RoomID { get; set; }
            public Nullable<Int64> UserID { get; set; }
            public Nullable<bool> IsCredit { get; set; }
            public Guid? PullGUID { get; set; }
            public Guid PullDetailGUID { get; set; }

            public string LotNumber { get; set; }

            public string SerialNumber { get; set; }

            public DateTime? ExpirationDate { get; set; }

            public string DateCode { get; set; }

            public double? Quantity { get; set; }

            public double? Cost { get; set; }

            public string EditedFrom { get; set; }

            public string ErrorMessage { get; set; }

            public string StatusMessage { get; set; }
        }

        #endregion


        public class Import_WorkOrderHeader_DTO
        {
            public Int64 ID { get; set; }
            public string ImportBulkID { get; set; }
            public Nullable<Int64> EnterpriseID { get; set; }
            public Nullable<Int64> CompanyID { get; set; }
            public Nullable<Int64> RoomID { get; set; }
            public Nullable<Int64> UserID { get; set; }

            public string EnterpriseName { get; set; }
            public string CompanyName { get; set; }
            public string RoomName { get; set; }
            public string UserName { get; set; }

            public string Description { get; set; }

            public string Asset { get; set; }

            public string Customer { get; set; }

            public Nullable<Double> Odometer_OperationHours { get; set; }

            public string SupplierName { get; set; }


            public string TechnicianCode { get; set; }

            public string TechnicianName { get; set; }

            public string Tool { get; set; }

            public Nullable<Guid> WOGUID { get; set; }

            public string WorkOrderStatus { get; set; }

            public string WorkOrderType { get; set; }

            public string WorkorderName { get; set; }

            public string UDF1 { get; set; }
            public string UDF2 { get; set; }
            public string UDF3 { get; set; }
            public string UDF4 { get; set; }
            public string UDF5 { get; set; }

            public Nullable<DateTime> ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public Nullable<bool> IsProcessed { get; set; }
            public Nullable<DateTime> ProcessDate { get; set; }
            public Nullable<bool> IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string strImportDate { get; set; }
            public string strProcessDate { get; set; }
            public Guid? ImportGUID { get; set; }
            public string EditedFrom { get; set; }

            public int? CreatedFrom { get; set; }

            public byte[] SignatureContent { get; set; }

            public List<Import_PULL_DTO> WOPullItems { get; set; }

            public List<Import_WorkOrderImageDetail_DTO> WOImageDetail { get; set; }

            public string SupplierAccountNumber { get; set; }
        }



        public class Import_WorkOrderImageDetail_DTO
        {
            public Int64 ID { get; set; }
            public string ImportBulkID { get; set; }
            public Nullable<Int64> WOImportHeaderID { get; set; }

            public Nullable<Guid> WOImgGUID { get; set; }

            public string WOImageName { get; set; }

            public byte[] WOImageContent { get; set; }

            public Nullable<bool> IsDeleted { get; set; }

            public Nullable<DateTime> ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public Nullable<bool> IsProcessed { get; set; }
            public Nullable<DateTime> ProcessDate { get; set; }
            public Nullable<bool> IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string strImportDate { get; set; }
            public string strProcessDate { get; set; }
            public Guid? ImportGUID { get; set; }
            public string EditedFrom { get; set; }


        }

        public class Import_PreReceivOrderDetail_DTO
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
            public double? Quantity { get; set; }
            public double? Cost { get; set; }

            public DateTime? ExpirationDate { get; set; }

            public string LotNumber { get; set; }

            public string SerialNumber { get; set; }

            public Guid GUID { get; set; }

            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }

            public string EditedFrom { get; set; }
            public string StrExpirationDate { get; set; }

            public string ReceiveBinName { get; set; }

            public string PackSlipNumber { get; set; }

            public List<Import_ReceiveFileDetail_DTO> ReceiveOrderImageDetails { get; set; }
        }

        public class Import_OrderDetail_DTO
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

            public string BinName { get; set; }
            public string OrderNumber { get; set; }
            public string ASNNumber { get; set; }
            public double? RequiredQty { get; set; }
            public double? ApprovedQuantity { get; set; }

            public string ControlNumber { get; set; }

            public double? InTransitQuantity { get; set; }

            public bool? IsEDISent { get; set; }
            public int? ItemType { get; set; }
            public string LineNumber { get; set; }
            public string EditedFrom { get; set; }

            public string UDF1 { get; set; }

            public string UDF2 { get; set; }

            public string UDF3 { get; set; }

            public string UDF4 { get; set; }

            public string UDF5 { get; set; }
            public List<Import_PreReceivOrderDetail_DTO> OrderReceiveDetails { get; set; }
            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            public double? Cost { get; set; }
            public double? Price { get; set; }
            public string CostUOM { get; set; }
            public double? PerItemCost { get; set; }
            public double? OrderCost { get; set; }
            public double? Quantity { get; set; }
            public DateTime? ReceiveDate { get; set; }
            public string StrReceiveDate { get; set; }
            //public string ReceiveBinName { get; set; }
            public bool? SerialNumberTracking { get; set; }
            public bool? LotNumberTracking { get; set; }
            public bool? DateCodeTracking { get; set; }

            public bool? IsBackOrdered { get; set; }
            public double? BackOrderedQuantity { get; set; }
            public DateTime? ExpectedDate { get; set; }
            public Nullable<System.Int32> POItemLineNumber { get; set; }
            public List<Import_OrderDetail_Tracking_DTO> OrderDetailTracking { get; set; }

        }

        public class Import_OrderDetail_Tracking_DTO
        {
            public string SerialNumber { get; set; }
            public string LotNumber { get; set; }
            public string PackSlipNumber { get; set; }
            public double? Quantity { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public Guid? OrderDetailGUID { get; set; }
            public string ImportBulkID { get; set; }
            public string EditedFrom { get; set; }
            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
        }
        public class Import_OrderMaster_DTO
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

            public Guid? OrderGUID { get; set; }
            public Guid? ItemGUID { get; set; }
            public string ItemNumber { get; set; }
            public string OrderNumber { get; set; }
            public string ReleaseNumber { get; set; }
            public DateTime? RequiredDate { get; set; }
            public string OrderStatus { get; set; }
            public string Comment { get; set; }
            public string PackSlipNumber { get; set; }
            public string ShippingTrackNumber { get; set; }
            public string Customer { get; set; }
            public string ShipViaName { get; set; }
            public string CustomerAddress { get; set; }
            public string ShippingVendorName { get; set; }
            public string SupplierAccountName { get; set; }
            public string SupplierName { get; set; }
            public string SupplierAccountNumber { get; set; }
            public bool? IsOrderExist { get; set; }
            public bool? IsEDIOrder { get; set; }
            public int? OrderType { get; set; }
            public string VersionNumber { get; set; }
            public int? CreatedFrom { get; set; }
            public string EditedFrom { get; set; }

            public string UDF1 { get; set; }

            public string UDF2 { get; set; }

            public string UDF3 { get; set; }

            public string UDF4 { get; set; }

            public string UDF5 { get; set; }

            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            public string StrRequiredDate { get; set; }
            public Guid? ImportGUID { get; set; }

            public List<Import_OrderDetail_DTO> OrderDetails { get; set; }
            public List<Import_PreReceivOrderDetail_DTO> OrderReceiveDetails { get; set; }
            public List<Import_OrderImageDetail_DTO> OrderImageDetails { get; set; }

            private int? _OrderStatusValue;
            public int? OrderStatusValue
            {
                get
                {
                    //if (!_OrderStatusValue.HasValue)
                    //{
                    OrderStatus choice;
                    if (Enum.TryParse(OrderStatus, out choice))
                    {
                        _OrderStatusValue = (int)choice;
                    }
                    else { _OrderStatusValue = null; }

                    return _OrderStatusValue;
                    //}
                    //return _OrderStatusValue;
                }
                set { this._OrderStatusValue = value; }
            }
            public double? OrderCost { get; set; }
            public string SalesOrder { get; set; }          
        }

        public class ImportPreReceivOrderDetail
        {
            public Guid GUID { get; set; }
            public Guid? OrderDetailGUID { get; set; }
            public Guid? ItemGUID { get; set; }
            public string ItemNumber { get; set; }
            public double? Quantity { get; set; }
            public string LotNumber { get; set; }
            public string SerialNumber { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public double? Cost { get; set; }
            public string EditedFrom { get; set; }
            public string ReceiveBinName { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string PackSlipNumber { get; set; }

        }
        public class OrderDetailTrackingTbl
        {
            public Guid? ID { get; set; }
            public Guid? OrderDetailGUID { get; set; }
            public double? Quantity { get; set; }
            public string LotNumber { get; set; }
            public string SerialNumber { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public string EditedFrom { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string PackSlipNumber { get; set; }

        }
        public class OrderDetailsTbl
        {
            public Guid? OrderDetailGUID { get; set; }
            public Guid? OrderGUID { get; set; }
            public Guid? ItemGUID { get; set; }
            public string ItemNumber { get; set; }
            public string OrderNumber { get; set; }
            public string ASNNumber { get; set; }
            public double? RequiredQty { get; set; }
            public double? ApprovedQuantity { get; set; }
            public string BinName { get; set; }
            public string ControlNumber { get; set; }
            public double? InTransitQuantity { get; set; }
            public bool? IsEDISent { get; set; }
            public int? ItemType { get; set; }
            public string LineNumber { get; set; }
            public string EditedFrom { get; set; }
            public double? OrderCost { get; set; }
            public string UDF1 { get; set; }

            public string UDF2 { get; set; }

            public string UDF3 { get; set; }

            public string UDF4 { get; set; }

            public string UDF5 { get; set; }
            public double? Quantity { get; set; }
            public DateTime? ReceiveDate { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public bool? IsBackOrdered { get; set; }
            public double? BackOrderedQuantity { get; set; }
            public DateTime? ExpectedDate { get; set; }
            public Nullable<System.Int32> POItemLineNumber { get; set; }
        }

        #region InsertRequisition


        public class RequisitionImportDTO
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
            public string RequisitionNumber { get; set; }
            public Guid? ReqGUID { get; set; }
            public string WorkorderName { get; set; }
            public DateTime? RequiredDate { get; set; }
            public string RequisitionStatus { get; set; }
            public string Customer { get; set; }
            public string ProjectSpendName { get; set; }
            public string RequisitionType { get; set; }
            public string Description { get; set; }
            public string BillingAccount { get; set; }
            public string UDF1 { get; set; }
            public string UDF2 { get; set; }
            public string UDF3 { get; set; }
            public string UDF4 { get; set; }
            public string UDF5 { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusCode { get; set; }
            public List<RequisitionDetailsImportDTO> RequisitionItem { get; set; }
            public List<RequisitionDetailsForToolImportDTO> RequisitionTool { get; set; }
            public Guid? WorkorderGUID { get; set; }
            public string SupplierName { get; set; }
            public string EditedFrom { get; set; }
            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StatusMessage { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            public string StrRequiredDate { get; set; }
            public Guid? ImportGUID { get; set; }
            public int? CreatedFrom { get; set; }
            public List<RequisitionPULLDetailsImportDTO> PullDetails { get; set; }
            public List<ImportDTO.Import_OrderImageDetail_DTO> RequisitionFiles { get; set; }


        }


        public class RequisitionDetailsImportDTO
        {
            public long ID { get; set; }
            public string ImportBulkID { get; set; }
            public long? EnterpriseID { get; set; }
            public long? CompanyID { get; set; }
            public long? RoomID { get; set; }
            public long? UserID { get; set; }
            public string ItemNumber { get; set; }
            public Guid? ItemGUID { get; set; }
            public string RequisitionNumber { get; set; }
            public Guid RequisitionGuid { get; set; }
            public string BinNumber { get; set; }
            public double? QuantityRequisitioned { get; set; }
            public double? QuantityApproved { get; set; }
            public double? QuantityToPull { get; set; }
            public string RoomName { get; set; }
            public string CompanyName { get; set; }
            public string UserName { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusCode { get; set; }
            public string PullUDF1 { get; set; }
            public string PullUDF2 { get; set; }
            public string PullUDF3 { get; set; }
            public string PullUDF4 { get; set; }
            public string PullUDF5 { get; set; }
            public string EditedFrom { get; set; }
            public string ProjectSpend { get; set; }
            public Guid? PullGUID { get; set; }
            public string SupplierAccountNumber { get; set; }
            public Guid? SupplierAccountGUID { get; set; }
            public List<RequisitionPULLDetailsImportDTO> PullDetails { get; set; }
            public Guid? RequisitionDetailGuid { get; set; }
            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StatusMessage { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            public long? RequisitionDetailsID { get; set; }
        }

        #region for add Serial number into Insert Pull service


        public class RequisitionPULLDetailsImportDTO
        {
            public long ID { get; set; }
            public string ImportBulkID { get; set; }
            public long? EnterpriseID { get; set; }
            public long? CompanyID { get; set; }
            public long? RoomID { get; set; }
            public long? UserID { get; set; }
            public Guid? RequisitionDetailGuid { get; set; }
            public Guid? RequisitionPullGuid { get; set; }
            public string LotNumber { get; set; }
            public string SerialNumber { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public string DateCode { get; set; }
            public double? Quantity { get; set; }
            public double? Cost { get; set; }
            public string EditedFrom { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
        }

        #endregion

        public class RequisitionDetailsForToolImportDTO
        {
            public long ID { get; set; }
            public string ImportBulkID { get; set; }
            public long? EnterpriseID { get; set; }
            public long? CompanyID { get; set; }
            public long? RoomID { get; set; }
            public long? UserID { get; set; }
            public string RequisitionNumber { get; set; }
            public Guid RequisitionGuid { get; set; }
            public Guid? RequisitionToolDetailGuid { get; set; }
            public Guid? ToolGuid { get; set; }
            public string ToolName { get; set; }
            public string Serial { get; set; }
            public double? QuantityRequisitioned { get; set; }
            public double? QuantityApproved { get; set; }
            public double? QuantityToPull { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusCode { get; set; }
            public string ToolCheckOutUDF1 { get; set; }
            public string ToolCheckOutUDF2 { get; set; }
            public string ToolCheckOutUDF3 { get; set; }
            public string ToolCheckOutUDF4 { get; set; }
            public string ToolCheckOutUDF5 { get; set; }
            public string TechnicianName { get; set; }
            public string TechnicianCode { get; set; }
            public string ToolCategory { get; set; }
            public long? ToolCategoryID { get; set; }
            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StatusMessage { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
        }

        public class RequisitionToolDetailsTbl
        {
            public string RequisitionNumber { get; set; }
            public Guid RequisitionGuid { get; set; }
            public Guid? RequisitionToolDetailGuid { get; set; }
            public Guid? ToolGuid { get; set; }
            public string ToolName { get; set; }
            public string Serial { get; set; }
            public double? QuantityRequisitioned { get; set; }
            public double? QuantityApproved { get; set; }
            public double? QuantityToPull { get; set; }
            public string TechnicianName { get; set; }
            public string TechnicianCode { get; set; }
            public string ToolCategory { get; set; }
            public string ToolCheckOutUDF1 { get; set; }
            public string ToolCheckOutUDF2 { get; set; }
            public string ToolCheckOutUDF3 { get; set; }
            public string ToolCheckOutUDF4 { get; set; }
            public string ToolCheckOutUDF5 { get; set; }
        }

        public class RequisitionPullDetailsTbl
        {
            public Guid? RequisitionDetailGuid { get; set; }
            public Guid? RequisitionPullGuid { get; set; }
            public string LotNumber { get; set; }
            public string SerialNumber { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public string DateCode { get; set; }
            public double? Quantity { get; set; }
            public double? Cost { get; set; }
            public string EditedFrom { get; set; }
        }

        public class RequisitionDetailsTbl
        {
            public string RequisitionNumber { get; set; }
            public Guid RequisitionGuid { get; set; }
            public Guid? RequisitionDetailGuid { get; set; }
            public Guid? ItemGUID { get; set; }
            public string ItemNumber { get; set; }
            public string BinNumber { get; set; }
            public double? QuantityRequisitioned { get; set; }
            public double? QuantityApproved { get; set; }
            public double? QuantityToPull { get; set; }
            public string ProjectSpend { get; set; }
            public Guid? PullGUID { get; set; }
            public string SupplierAccountNumber { get; set; }
            public string EditedFrom { get; set; }
            public string PullUDF1 { get; set; }
            public string PullUDF2 { get; set; }
            public string PullUDF3 { get; set; }
            public string PullUDF4 { get; set; }
            public string PullUDF5 { get; set; }

        }

        #endregion

        #region Edit Item Import DTOs


        public class EDIItemImportDTO
        {
            public long ID { get; set; }
            public string ImportBulkID { get; set; }
            public long? EnterpriseID { get; set; }
            public long? CompanyID { get; set; }
            public long? RoomID { get; set; }
            public long? UserID { get; set; }
            public string EnterpriseName { get; set; }

            public string ItemNumber { get; set; }
            public double? DefaultReorderQuantity { get; set; }
            public double? DefaultPullQuantity { get; set; }
            //public string Unit { get; set; }
            public string CompanyName { get; set; }
            public string RoomName { get; set; }
            public double? Cost { get; set; }
            public double? Markup { get; set; }
            public double? CriticalQuantity { get; set; }
            public double? MinimumQuantity { get; set; }
            public double? MaximumQuantity { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public bool? IsTransfer { get; set; }
            public bool? IsPurchase { get; set; }
            public string Description { get; set; }
            public bool? Consignment { get; set; }
            public string UOM { get; set; }
            public string CostUOM { get; set; }
            public Guid? Guid { get; set; }
            public bool? IsActive { get; set; }
            public string EditedFrom { get; set; }
            public string UDF1 { get; set; }
            public string UDF2 { get; set; }
            public string UDF3 { get; set; }
            public string UDF4 { get; set; }
            public string UDF5 { get; set; }
            public string UDF6 { get; set; }
            public string UDF7 { get; set; }
            public string UDF8 { get; set; }
            public string UDF9 { get; set; }
            public string UDF10 { get; set; }
            public byte[] ImageContent { get; set; }
            public string ImageName { get; set; }
            public string ImageType { get; set; }
            public string ItemImageExternalURL { get; set; }
            public string UserName { get; set; }
            public string UPC { get; set; }
            public string UNSPSC { get; set; }
            public string LongDescription { get; set; }
            public string Category { get; set; }
            public string GLAccount { get; set; }
            public double? SellPrice { get; set; }
            public int? LeadTimeInDays { get; set; }
            public string ItemDocExternalURL { get; set; }
            public string TrendingSetting { get; set; }
            public bool? Taxable { get; set; }
            public bool? IsAutoInventoryClassification { get; set; }
            public string InventoryClassification { get; set; }
            public bool? IsEnforceDefaultReorderQuantity { get; set; }
            public bool? PullQtyScanOverride { get; set; }
            public bool? IsPackslipMandatoryAtReceive { get; set; }
            public byte[] ItemLink2ImageContent { get; set; }
            public string ItemLink2ImageName { get; set; }
            public string ItemLink2ImageType { get; set; }
            public string ItemLink2ImageExternalURL { get; set; }
            public double? WeightPerPiece { get; set; }
            public List<KitDetailsImportDTO> KitDetails { get; set; }
            public List<ItemManufacturerDetailsImportDTO> ItemManufacturerDetails { get; set; }
            public List<ItemSupplierDetailsImportDTO> ItemSupplierDetails { get; set; }
            public List<InventoryLocationDetailsImportDTO> InventoryLocationDetails { get; set; }

            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            public Guid? ImportGUID { get; set; }
            public string EnhancedDescription { get; set; }
            public Nullable<System.Int32> POItemLineNumber { get; set; }
        }

        public class ItemSupplierDetailsImportDTO
        {
            public long ID { get; set; }
            public string ImportBulkID { get; set; }
            public long? EnterpriseID { get; set; }
            public long? CompanyID { get; set; }
            public long? RoomID { get; set; }
            public long? UserID { get; set; }

            public Guid? ItemGuid { get; set; }
            public string SupplierName { get; set; }
            public string SupplierNumber { get; set; }
            public string BlanketPO { get; set; }
            public bool IsDefault { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }

            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            //public Guid? ImportGUID { get; set; }
            public long? SupplierID { get; set; }
            public long? BlanketPOID { get; set; }
        }


        public class ItemManufacturerDetailsImportDTO
        {
            public long ID { get; set; }
            public string ImportBulkID { get; set; }
            public long? EnterpriseID { get; set; }
            public long? CompanyID { get; set; }
            public long? RoomID { get; set; }
            public long? UserID { get; set; }
            public Guid? ItemGuid { get; set; }

            public string ManufacturerName { get; set; }
            public string ManufacturerNumber { get; set; }
            public bool IsDefault { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }

            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            //public Guid? ImportGUID { get; set; }
            public long? ManufacturerID { get; set; }
        }


        public class InventoryLocationDetailsImportDTO
        {
            public long ID { get; set; }
            public string ImportBulkID { get; set; }
            public long? EnterpriseID { get; set; }
            public long? CompanyID { get; set; }
            public long? RoomID { get; set; }
            public long? UserID { get; set; }
            public Guid? ItemGuid { get; set; }

            public string BinNumber { get; set; }
            public double? CriticalQuantity { get; set; }
            public double? MinimumQuantity { get; set; }
            public double? MaximumQuantity { get; set; }
            public bool? IsDefault { get; set; }
            public double? ConsignedQuantity { get; set; }
            public double? CustomerOwnedQuantity { get; set; }
            public double? eVMISensorID { get; set; }
            public bool? IsEnforceDefaultReorderQuantity { get; set; }
            public bool? IsEnforceDefaultPullQuantity { get; set; }
            public double? DefaultReorderQuantity { get; set; }
            public double? DefaultPullQuantity { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }

            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            //public Guid? ImportGUID { get; set; }
        }


        public class KitDetailsImportDTO
        {
            public long ID { get; set; }
            public string ImportBulkID { get; set; }
            public long? EnterpriseID { get; set; }
            public long? CompanyID { get; set; }
            public long? RoomID { get; set; }
            public long? UserID { get; set; }
            public Guid? ItemGuid { get; set; }

            public string ItemType { get; set; }
            public string ItemNumber { get; set; }
            public string Description { get; set; }
            public double? QuantityPerKit { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }

            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            //public Guid? ImportGUID { get; set; }
        }

        public class ItemSupplierDetailsTbl
        {
            public string SupplierName { get; set; }
            public string SupplierNumber { get; set; }
            public string BlanketPO { get; set; }
            public bool IsDefault { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
        }

        public class ItemManufacturerDetailsTbl
        {
            public string ManufacturerName { get; set; }
            public string ManufacturerNumber { get; set; }
            public bool IsDefault { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }

        }

        public class InventoryLocationDetailsTbl
        {
            public string BinNumber { get; set; }
            public double? CriticalQuantity { get; set; }
            public double? MinimumQuantity { get; set; }
            public double? MaximumQuantity { get; set; }
            public bool? IsDefault { get; set; }
            public double? ConsignedQuantity { get; set; }
            public double? CustomerOwnedQuantity { get; set; }
            public double? eVMISensorID { get; set; }
            public bool? IsEnforceDefaultReorderQuantity { get; set; }
            public bool? IsEnforceDefaultPullQuantity { get; set; }
            public double? DefaultReorderQuantity { get; set; }
            public double? DefaultPullQuantity { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }

        }

        public class KitDetailsTbl
        {
            public string ItemType { get; set; }
            public string ItemNumber { get; set; }
            public string Description { get; set; }
            public double? QuantityPerKit { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
        }

        #endregion

        public class Import_OrderImageDetail_DTO
        {
            public Int64 ID { get; set; }
            public string ImportBulkID { get; set; }
            public Nullable<Int64> OImportHeaderID { get; set; }

            public Nullable<Guid> OImgGUID { get; set; }

            public string OImageName { get; set; }

            public byte[] OImageContent { get; set; }

            public Nullable<bool> IsDeleted { get; set; }

            public Nullable<DateTime> ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public Nullable<bool> IsProcessed { get; set; }
            public Nullable<DateTime> ProcessDate { get; set; }
            public Nullable<bool> IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string strImportDate { get; set; }
            public string strProcessDate { get; set; }
            public Guid? ImportGUID { get; set; }
            public string EditedFrom { get; set; }


        }

        public class RequisitionFileDTO
        {
            public Guid? GUID { get; set; }
            public string OImageName { get; set; }
            public byte[] OImageContent { get; set; }
            public string ImgStatusCode { get; set; }
            public string ImgErrorMessage { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class Import_RequisitionFile_DTO
        {
            public Int64 ID { get; set; }
            public string ImportBulkID { get; set; }
            public Nullable<Int64> ImportHeaderID { get; set; }

            public Nullable<Guid> ImgGUID { get; set; }

            public string ImageName { get; set; }

            public byte[] ImageContent { get; set; }

            public Nullable<bool> IsDeleted { get; set; }

            public Nullable<DateTime> ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public Nullable<bool> IsProcessed { get; set; }
            public Nullable<DateTime> ProcessDate { get; set; }
            public Nullable<bool> IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string strImportDate { get; set; }
            public string strProcessDate { get; set; }
            public Guid? ImportGUID { get; set; }
            public string EditedFrom { get; set; }


        }


        public class Import_ReceiveFileDetail_DTO
        {
            public Int64 ID { get; set; }
            public string ImportBulkID { get; set; }

            public string OrderDetailsGUID { get; set; }

            public Nullable<Int64> ImportHeaderID { get; set; }

            public Nullable<Guid> GUID { get; set; }

            public string FileName { get; set; }

            public byte[] FileContent { get; set; }

            public Nullable<bool> IsDeleted { get; set; }

            public Nullable<DateTime> ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public Nullable<bool> IsProcessed { get; set; }
            public Nullable<DateTime> ProcessDate { get; set; }
            public Nullable<bool> IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string strImportDate { get; set; }
            public string strProcessDate { get; set; }
            public Guid? ImportGUID { get; set; }
            public string EditedFrom { get; set; }
        }


        public class Import_QuoteDetail_DTO
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
            public Guid? QuoteDetailGUID { get; set; }
            public Guid? QuoteGuid { get; set; }
            public Guid? ItemGUID { get; set; }
            public string ItemNumber { get; set; }

            public string BinName { get; set; }
            public string QuoteNumber { get; set; }
            public string ASNNumber { get; set; }
            public double? RequiredQty { get; set; }
            public double? ApprovedQuantity { get; set; }

            public string ControlNumber { get; set; }

            public double? InTransitQuantity { get; set; }

            public bool? IsEDISent { get; set; }
            public int? ItemType { get; set; }
            public string LineNumber { get; set; }
            public string EditedFrom { get; set; }

            public string UDF1 { get; set; }

            public string UDF2 { get; set; }

            public string UDF3 { get; set; }

            public string UDF4 { get; set; }

            public string UDF5 { get; set; }
          //  public List<Import_PreReceivOrderDetail_DTO> OrderReceiveDetails { get; set; }
            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            public double? Cost { get; set; }
            public double? Price { get; set; }
            public string CostUOM { get; set; }
            public double? PerItemCost { get; set; }
            public double? QuoteCost { get; set; }
            public double? Quantity { get; set; }
            public DateTime? ReceiveDate { get; set; }
            public string StrReceiveDate { get; set; }
            //public string ReceiveBinName { get; set; }
            public bool? SerialNumberTracking { get; set; }
            public bool? LotNumberTracking { get; set; }
            public bool? DateCodeTracking { get; set; }

            public bool? IsBackQuoted { get; set; }
            public double? BackQuotedQuantity { get; set; }
            public DateTime? ExpectedDate { get; set; }
            public Nullable<System.Int32> POItemLineNumber { get; set; }

        }

        public class Import_QuoteMaster_DTO
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

            public Guid? QuoteGuid { get; set; }
            public Guid? ItemGUID { get; set; }
            public string ItemNumber { get; set; }
            public string QuoteNumber { get; set; }
            public string ReleaseNumber { get; set; }
            public DateTime? RequiredDate { get; set; }
            public string QuoteStatus { get; set; }
            public string Comment { get; set; }
            public string PackSlipNumber { get; set; }
            public string ShippingTrackNumber { get; set; }
            public string Customer { get; set; }
            public string ShipViaName { get; set; }
            public string CustomerAddress { get; set; }
            public string ShippingVendorName { get; set; }
            public string QuoteSupplierIdsCSV { get; set; }
            public string SupplierAccountNumber { get; set; }
            public bool? IsQuoteExist { get; set; }
            public bool? IsEDIQuote { get; set; }
            public int? OrderType { get; set; }
            public string VersionNumber { get; set; }
            public int? CreatedFrom { get; set; }
            public string EditedFrom { get; set; }

            public string UDF1 { get; set; }

            public string UDF2 { get; set; }

            public string UDF3 { get; set; }

            public string UDF4 { get; set; }

            public string UDF5 { get; set; }

            public DateTime? ImportDate { get; set; }
            public string ImportedFrom { get; set; }
            public bool? IsProcessed { get; set; }
            public DateTime? ProcessDate { get; set; }
            public bool? IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string StatusMessage { get; set; }
            public string StrImportDate { get; set; }
            public string StrProcessDate { get; set; }
            public string StrRequiredDate { get; set; }
            public Guid? ImportGUID { get; set; }

            public List<Import_QuoteDetail_DTO> QuoteDetails { get; set; }

            private int? _QuoteStatusValue;
            public int? QuoteStatusValue
            {
                get
                {
                    //if (!_OrderStatusValue.HasValue)
                    //{
                    QuoteStatus choice;
                    if (Enum.TryParse(QuoteStatus, out choice))
                    {
                        _QuoteStatusValue = (int)choice;
                    }
                    else { _QuoteStatusValue = null; }

                    return _QuoteStatusValue;
                    //}
                    //return _OrderStatusValue;
                }
                set { this._QuoteStatusValue = value; }
            }
            public double? OrderCost { get; set; }
            public string SalesOrder { get; set; }
        }

        public class AutoSotForImport_DTO
        {
            public Guid NewItemGUID { get; set; }        
            public long UserID { get; set; }             
            public long SessionID { get; set; }             
            public string EnterpriseDBName { get; set; }
            public bool IsStarted { get; set; }         
            public bool IsCompleted { get; set; }        
            public bool IsError { get; set; }            
            public bool ISQtyToMeedDemandUpdateRequired { get; set; }            
            public string Error { get; set; }           
        }
    }
}


