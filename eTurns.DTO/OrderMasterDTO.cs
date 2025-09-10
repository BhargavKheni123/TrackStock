using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;



namespace eTurns.DTO
{
    public enum OrderStatus
    {
        UnSubmitted = 1,
        Submitted = 2,
        Approved = 3,
        Transmitted = 4,
        TransmittedIncomplete = 5,
        TransmittedPastDue = 6,
        TransmittedInCompletePastDue = 7,
        Closed = 8,
        //Rejected = 9,
    }

    /// <summary>
    /// OrderType
    /// </summary>
    public enum OrderType
    {
        Order = 1,
        RuturnOrder = 2
    }

    public class AutoOrderNumberGenerate
    {

        public string OrderNumber { get; set; }
        public List<SupplierBlanketPODetailsDTO> BlanketPOs { get; set; }

        public bool IsBlank { get; set; }
        public bool IsBlanketPO { get; set; }
        public int OrderNumberFormateType { get; set; }
        public string OrderGeneratedFrom { get; set; }
        public string ErrorDescription { get; set; }

        public string OrderNumberForSorting { get; set; }
        public long LastUsedTempIncrementNumberRoom { get; set; }
        public long LastUsedTempIncrementNumberSupplier { get; set; }

        public string RequisitionNumberForSorting { get; set; }
        public string RequisitionNumber { get; set; }
        public string RequisitionGeneratedFrom { get; set; }
        public int RequisitionFormateType { get; set; }
        public long LastReqUsedTempIncrementNumberRoom { get; set; }
        public long LastReqUsedTempIncrementNumberSupplier { get; set; }
    }

    public class OrderMasterDTO
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        [Display(Name = "OrderNumber", ResourceType = typeof(ResOrder))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(22, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String OrderNumber { get; set; }

        [Display(Name = "ReleaseNumber", ResourceType = typeof(ResOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ReleaseNumber { get; set; }

        [Display(Name = "ShipVia", ResourceType = typeof(ResOrder))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.Int64? ShipVia { get; set; }

        [Display(Name = "StagingName", ResourceType = typeof(ResMaterialStaging))]
        [AllowHtml]
        public System.Int64? StagingID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Supplier", ResourceType = typeof(ResOrder))]
        //[Range(1, 9223372036854775807)]
        public Nullable<System.Int64> Supplier { get; set; }

        [Display(Name = "Comment", ResourceType = typeof(ResOrder))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Comment { get; set; }

        [Display(Name = "RequiredDate", ResourceType = typeof(ResOrder))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public System.DateTime RequiredDate { get; set; }
        //private string _RequiredDateStr;

        //public System.String RequiredDateStr { get; set; }


        private string _requiredDate;

        [Display(Name = "RequiredDateStr", ResourceType = typeof(ResOrder))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string RequiredDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_requiredDate))
                {
                    _requiredDate = FnCommon.ConvertDateByTimeZone(RequiredDate, false, true);
                }
                return _requiredDate;
            }
            set { this._requiredDate = value; }
        }
        [Display(Name = "SalesOrderNumber", ResourceType = typeof(ResOrder))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string SalesOrderNumber { get; set; }


        [Display(Name = "OrderStatus", ResourceType = typeof(ResOrder))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int32 OrderStatus { get; set; }

        [Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
        public Nullable<System.Int64> CustomerID { get; set; }

        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String PackSlipNumber { get; set; }

        [Display(Name = "ShippingTrackNumber", ResourceType = typeof(ResOrder))]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ShippingTrackNumber { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        public Boolean IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "Reject Reason")]
        public string RejectionReason { get; set; }

        [Display(Name = "Address", ResourceType = typeof(Resources.ResCommon))]
        public string CustomerAddress { get; set; }
        public int? OrderType { get; set; }

        [Display(Name = "OrderCost")]
        public Nullable<System.Double> OrderCost { get; set; }

        public System.Double OrderPrice { get; set; }

        [Display(Name = "NoOfLineItems")]
        public Nullable<int> NoOfLineItems { get; set; }

        [Display(Name = "OrderDate", ResourceType = typeof(ResOrder))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> OrderDate { get; set; }

        [Display(Name = "ChangeOrderRevisionNo", ResourceType = typeof(ResOrder))]
        public Nullable<Int64> ChangeOrderRevisionNo { get; set; }

        [Display(Name = "ShippingVendor", ResourceType = typeof(ResOrder))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "NumberOnly", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<Int64> ShippingVendor { get; set; }

        public string AccountNumber { get; set; }

        public Nullable<Int64> BlanketOrderNumberID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public bool IsHistory { get; set; }


        public string SupplierName { get; set; }

        [Display(Name = "ShipViaName", ResourceType = typeof(ResOrder))]
        public string ShipViaName { get; set; }

        [Display(Name = "StagingName", ResourceType = typeof(ResOrder))]
        public string StagingName { get; set; }

        [Display(Name = "ShippingVendorName", ResourceType = typeof(ResOrder))]
        public string ShippingVendorName { get; set; }

        [Display(Name = "CustomerName", ResourceType = typeof(ResOrder))]
        public string CustomerName { get; set; }
        public string OrderStatusText { get; set; }
        public char OrderStatusChar
        {
            get
            {
                return FnCommon.GetOrderStatusChar(OrderStatus);
            }
        }
        public bool IsRecordNotEditable { get; set; }
        public bool IsOnlyStatusUpdate { get; set; }
        public bool OrderIsInReceive { get; set; }
        public bool IsAbleToDelete { get; set; }
        public List<OrderDetailsDTO> OrderListItem { get; set; }
        public string AppendedBarcodeString { get; set; }
        public int TotalRecords { get; set; }
        public AutoOrderNumberGenerate AutoOrderNumber { get; set; }
        public bool IsBlanketOrder { get; set; }

        public Int64? StagingDefaultLocation { get; set; }
        public bool IsChangeOrderClick { get; set; }
        public System.String WhatWhereAction { get; set; }

        public string Indicator { get; set; }
        public bool IsOrderSelected { get; set; }
        public string OrderLineItemsIds { get; set; }

        public DateTime ChangeOrderCreated { get; set; }
        public DateTime ChangeOrderLastUpdated { get; set; }
        public Int64 ChangeOrderCreatedBy { get; set; }
        public Int64 ChangeOrderLastUpdatedBy { get; set; }
        public Guid ChangeOrderGUID { get; set; }
        public Int64 ChangeOrderID { get; set; }

        [Display(Name = "ChangeOrderCreatedBy", ResourceType = typeof(ResCommon))]
        public string ChangeOrderCreatedByName { get; set; }

        [Display(Name = "ChangeOrderUpdatedBy", ResourceType = typeof(ResCommon))]
        public string ChangeOrderUpdatedByName { get; set; }

        public bool IsMainOrderInChangeOrderHistory { get; set; }

        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResOrder))]
        [AllowHtml]
        public System.String OMPackSlipNumbers { get; set; }

        [Display(Name = "OrderNumber_ForSorting", ResourceType = typeof(ResOrder))]
        [AllowHtml]
        public System.String OrderNumber_ForSorting { get; set; }


        [Display(Name = "CustomerGUID", ResourceType = typeof(ResOrder))]
        public Nullable<System.Guid> CustomerGUID { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

        public int InCompleteItemCount { get; set; }

        public Nullable<Guid> MaterialStagingGUID { get; set; }

        public bool IsOnlyFromUI { get; set; }

        public bool? IsEDIOrder { get; set; }

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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(LastUpdated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }

        [Display(Name = "SupplierAccountDetail", ResourceType = typeof(ResOrder))]
        public Nullable<Guid> SupplierAccountGuid { get; set; }

        private string _changeOrderCreated;
        public string ChangeOrderCreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_changeOrderCreated))
                {
                    _changeOrderCreated = FnCommon.ConvertDateByTimeZone(ChangeOrderCreated, true);
                }
                return _changeOrderCreated;
            }
            set { this._changeOrderCreated = value; }
        }

        private string _changeOrderLastUpdated;

        public string ChangeOrderUpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_changeOrderLastUpdated))
                {
                    _changeOrderLastUpdated = FnCommon.ConvertDateByTimeZone(ChangeOrderLastUpdated, true);
                }
                return _changeOrderLastUpdated;
            }
            set { this._changeOrderLastUpdated = value; }
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

        public string CostUOM { get; set; }

        private string _RequiredDate;

        public string RequiredDateString
        {
            get
            {
                if (string.IsNullOrEmpty(_RequiredDate))
                {
                    _RequiredDate = Convert.ToString(FnCommon.ConvertDateByTimeZone(RequiredDate, false, true).Split(' ')[0]);
                }
                return _RequiredDate;
            }
            set { this._RequiredDate = value; }
        }
        public Nullable<double> OnOrderInTransitQuantity { get; set; }

        public string SupplierAccountNumberName { get; set; }
        public string SupplierAccountNumber { get; set; }
        public string SupplierAccountName { get; set; }
        public string SupplierAccountAddress { get; set; }
        public string SupplierAccountCity { get; set; }
        public string SupplierAccountState { get; set; }
        public string SupplierAccountZipcode { get; set; }
        public string SupplierAccountDetailWithFullAddress { get; set; }

        public System.String OrderLineItemUDF1 { get; set; }
        public System.String OrderLineItemUDF2 { get; set; }
        public System.String OrderLineItemUDF3 { get; set; }
        public System.String OrderLineItemUDF4 { get; set; }
        public System.String OrderLineItemUDF5 { get; set; }

        public string CompanyName { get; set; }
        public int PriseSelectionOption { get; set; }
        public bool IsSupplierApprove { get; set; }
        public Nullable<System.Int64> RequesterID { get; set; }
        public Nullable<System.Int64> ApproverID { get; set; }
        public bool IsOrderReleaseNumberEditable { get; set; }
        public System.String CartQuantityString { get; set; }

        [Display(Name = "SalesOrder", ResourceType = typeof(ResOrder))]
        [AllowHtml]
        public System.String SalesOrder { get; set; }
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public string Status { get; set; }
    }

    public class UnFulFilledOrderMasterDTO
    {

        public System.Int64 ID { get; set; }
        public System.String OrderNumber { get; set; }
        public System.String ReleaseNumber { get; set; }
        public Guid? ItemGUID { get; set; }
        public System.Int64? ShipVia { get; set; }

        public System.Int64? StagingID { get; set; }

        public Nullable<System.Int64> Supplier { get; set; }

        public Guid? OrderGUID { get; set; }
        public System.String Comment { get; set; }

        public Nullable<System.Double> Total { get; set; }
        public System.String RequiredDate { get; set; }
        public string RoomInfo { get; set; }
        public string CompanyInfo { get; set; }
        public string OrderStatus { get; set; }

        public Nullable<System.Int64> CustomerID { get; set; }

        public System.String PackSlipNumber { get; set; }

        public System.String ShippingTrackNumber { get; set; }

        public string Created { get; set; }

        public string LastUpdated { get; set; }

        public string CreatedBy { get; set; }

        public string LastUpdatedBy { get; set; }

        public string Room { get; set; }

        public Boolean IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }


        public Nullable<System.Int64> CompanyID { get; set; }

        public Guid GUID { get; set; }

        public System.String UDF1 { get; set; }

        public System.String UDF2 { get; set; }

        public System.String UDF3 { get; set; }

        public System.String UDF4 { get; set; }

        public System.String UDF5 { get; set; }

        public string RoomName { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public string RejectionReason { get; set; }

        public string CustomerAddress { get; set; }
        public int? OrderType { get; set; }


        public Nullable<System.Double> OrderCost { get; set; }

        public Nullable<int> NoOfLineItems { get; set; }


        public string OrderDate { get; set; }
        public string IsCloseItem { get; set; }

        public Nullable<Int64> ChangeOrderRevisionNo { get; set; }


        public Nullable<Int64> ShippingVendor { get; set; }

        public string AccountNumber { get; set; }

        public Nullable<Int64> BlanketOrderNumberID { get; set; }


        public string Action { get; set; }


        public Int64 HistoryID { get; set; }

        public bool IsHistory { get; set; }
        public string SupplierName { get; set; }
        public string ShipViaName { get; set; }
        public string StagingName { get; set; }
        public string ShippingVendorName { get; set; }
        public string CustomerName { get; set; }
        public string OrderStatusText { get; set; }
        public char OrderStatusChar
        {
            get
            {
                return FnCommon.GetOrderStatusChar(OrderStatus);
            }
        }
        public bool IsRecordNotEditable { get; set; }
        public bool IsOnlyStatusUpdate { get; set; }
        public bool OrderIsInReceive { get; set; }
        public bool IsAbleToDelete { get; set; }
        public List<OrderDetailsDTO> OrderListItem { get; set; }
        public string AppendedBarcodeString { get; set; }
        public int TotalRecords { get; set; }
        public AutoOrderNumberGenerate AutoOrderNumber { get; set; }
        public bool IsBlanketOrder { get; set; }

        public Int64? StagingDefaultLocation { get; set; }
        public bool IsChangeOrderClick { get; set; }
        public System.String WhatWhereAction { get; set; }

        public string Indicator { get; set; }
        public bool IsOrderSelected { get; set; }
        public string OrderLineItemsIds { get; set; }

        public DateTime ChangeOrderCreated { get; set; }
        public DateTime ChangeOrderLastUpdated { get; set; }
        public Int64 ChangeOrderCreatedBy { get; set; }
        public Int64 ChangeOrderLastUpdatedBy { get; set; }
        public Guid ChangeOrderGUID { get; set; }
        public Int64 ChangeOrderID { get; set; }


        public string ChangeOrderCreatedByName { get; set; }


        public string ChangeOrderUpdatedByName { get; set; }

        public bool IsMainOrderInChangeOrderHistory { get; set; }


        public System.String OMPackSlipNumbers { get; set; }


        public System.String OrderNumber_ForSorting { get; set; }



        public Nullable<System.Guid> CustomerGUID { get; set; }


        public Nullable<System.DateTime> ReceivedOn { get; set; }


        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        public System.String AddedFrom { get; set; }


        public System.String EditedFrom { get; set; }

        public int InCompleteItemCount { get; set; }

        public Nullable<Guid> MaterialStagingGUID { get; set; }

        public bool IsOnlyFromUI { get; set; }

        public bool? IsEDIOrder { get; set; }



        public Nullable<Guid> SupplierAccountGuid { get; set; }

        private string _changeOrderCreated;
        public string ChangeOrderCreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_changeOrderCreated))
                {
                    _changeOrderCreated = FnCommon.ConvertDateByTimeZone(ChangeOrderCreated, true);
                }
                return _changeOrderCreated;
            }
            set { this._changeOrderCreated = value; }
        }

        private string _changeOrderLastUpdated;

        public string ChangeOrderUpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_changeOrderLastUpdated))
                {
                    _changeOrderLastUpdated = FnCommon.ConvertDateByTimeZone(ChangeOrderLastUpdated, true);
                }
                return _changeOrderLastUpdated;
            }
            set { this._changeOrderLastUpdated = value; }
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

        public string CostUOM { get; set; }


    }
    public class ResOrder
    {
        private static string resourceFileName = "ResOrder";

        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string Bin
        {
            get
            {
                return ResourceRead.GetResourceValue("Bin", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Please add items for save order.
        /// </summary>
        public static string BlankItemSavedMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("BlankItemSavedMessage", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change Order.
        /// </summary>
        public static string ChangeOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeOrder", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Comment.
        /// </summary>
        public static string Comment
        {
            get
            {
                return ResourceRead.GetResourceValue("Comment", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Customer.
        /// </summary>
        public static string Customer
        {
            get
            {
                return ResourceRead.GetResourceValue("Customer", resourceFileName);
            }
        }

        public static string UncloseOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("UncloseOrder", resourceFileName);
            }
        }

        public static string EditReceipts
        {
            get
            {
                return ResourceRead.GetResourceValue("EditReceipts", resourceFileName);
            }
        }

        public static string EditOrderLineItems
        {
            get
            {
                return ResourceRead.GetResourceValue("EditOrderLineItems", resourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Item added.
        /// </summary>
        public static string ItemAddSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemAddSuccessfully", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Item GUID.
        /// </summary>
        public static string ItemGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemGUID", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Item ID.
        /// </summary>
        public static string ItemID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemID", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Add Items to order.
        /// </summary>
        public static string ItemModelHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemModelHeader", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Item not added.
        /// </summary>
        public static string ItemNotAdded
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNotAdded", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Order.
        /// </summary>
        public static string MenuLinkText
        {
            get
            {
                return ResourceRead.GetResourceValue("MenuLinkText", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Order Approval Request.
        /// </summary>
        public static string OrderApprovalleMailSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderApprovalleMailSubject", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Order GUID.
        /// </summary>
        public static string OrderGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderGUID", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Order ID.
        /// </summary>
        public static string OrderID
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderID", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Order Number.
        /// </summary>
        public static string OrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderNumber", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Order Status.
        /// </summary>
        public static string OrderStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderStatus", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Package Quantity.
        /// </summary>
        public static string PackageQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PackageQuantity", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Pack Slip Number.
        /// </summary>
        public static string PackSlipNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("PackSlipNumber", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Orders.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Orders.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Received Quantity.
        /// </summary>
        public static string ReceivedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedQuantity", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Rejected Reason.
        /// </summary>
        public static string RejectedReason
        {
            get
            {
                return ResourceRead.GetResourceValue("RejectedReason", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Release Number.
        /// </summary>
        public static string ReleaseNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ReleaseNumber", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Requested Quantity.
        /// </summary>
        public static string RequestedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedQuantity", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Requested Quantity.
        /// </summary>
        public static string ApprovedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ApprovedQuantity", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Required Date.
        /// </summary>
        public static string RequiredDate
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredDate", resourceFileName);
            }
        }
                
        public static string RequiredDateStr
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredDate", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Shipping Method.
        /// </summary>
        public static string ShippingMethod
        {
            get
            {
                return ResourceRead.GetResourceValue("ShippingMethod", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Shipping Track Number.
        /// </summary>
        public static string ShippingTrackNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ShippingTrackNumber", resourceFileName);
            }
        }
        public static string SupplierAccountDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccountDetail", resourceFileName);
            }
        }

         
         public static string SupplierAccountGuid
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccountGuid", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Shipping Track Number.
        /// </summary>
        public static string ASNNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ASNNumber", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ShipVia.
        /// </summary>
        public static string ShipVia
        {
            get
            {
                return ResourceRead.GetResourceValue("ShipVia", resourceFileName);
            }
        }

        
        public static string ShipViaName
        {
            get
            {
                return ResourceRead.GetResourceValue("ShipVia", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Staging Name.
        /// </summary>
        public static string StagingName
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingName", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Supplier.
        /// </summary>
        public static string Supplier
        {
            get
            {
                return ResourceRead.GetResourceValue("Supplier", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFileName,true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFileName, true);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string ReceiveQuentity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveQuentity", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveDate.
        /// </summary>
        public static string ReceiveDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveDate", resourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to ReceiveBin.
        /// </summary>
        public static string ReceiveBin
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveBin", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AddNewItemButton.
        /// </summary>
        public static string AddNewItemButton
        {
            get
            {
                return ResourceRead.GetResourceValue("AddNewItemButton", resourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to ReceiveButton.
        /// </summary>
        public static string ReceiveButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveButton", resourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to ReceiveButton.
        /// </summary>
        public static string GetOrderStatusText(string value)
        {
            return ResourceRead.GetResourceValue(value, resourceFileName);
        }


        /// <summary>
        ///   Looks up a localized string similar to ReceiveButton.
        /// </summary>
        public static string ConfirmApprQuantiyMassage
        {
            get
            {
                return ResourceRead.GetResourceValue("ConfirmApprQuantiyMassage", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OrderDate.
        /// </summary>
        public static string OrderDate
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderDate", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ShippingVendor.
        /// </summary>
        public static string ShippingVendor
        {
            get
            {
                return ResourceRead.GetResourceValue("ShippingVendor", resourceFileName);
            }
        }

        
        public static string ShippingVendorName
        {
            get
            {
                return ResourceRead.GetResourceValue("ShippingVendor", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ChangeOrderRevisionNo.
        /// </summary>
        public static string ChangeOrderRevisionNo
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeOrderRevisionNo", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Order Approval Request.
        /// </summary>
        public static string OrderToSupplierMailSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderToSupplierMailSubject", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to ChangeOrderRevisionNo.
        /// </summary>
        public static string Indicator
        {
            get
            {
                return ResourceRead.GetResourceValue("Indicator", resourceFileName);
            }
        }


        public static string NoOfLineItems
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfLineItems", resourceFileName);
            }
        }
        public static string ChangeReturnOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeReturnOrder", resourceFileName);
            }
        }
        public static string ItemBlanketPO
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemBlanketPO", resourceFileName);
            }
        }
        public static string NotApplicable
        {
            get
            {
                return ResourceRead.GetResourceValue("NotApplicable", resourceFileName);
            }
        }
        public static string OrderCost
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderCost", resourceFileName);
            }
        }
        public static string OrderPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderPrice", resourceFileName);
            }
        }
        public static string PageHeaderRO
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeaderRO", resourceFileName);
            }
        }
        public static string PageTitleRO
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitleRO", resourceFileName);
            }
        }
        public static string QuantityToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityToReceive", resourceFileName);
            }
        }
        public static string QuantityToReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityToReturn", resourceFileName);
            }
        }
        public static string RequiredReturnDate
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredReturnDate", resourceFileName);
            }
        }
        public static string ReturnButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnButton", resourceFileName);
            }
        }
        public static string ReturnDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnDate", resourceFileName);
            }
        }
        public static string ReturnedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnedQuantity", resourceFileName);
            }
        }
        public static string ReturnListTab
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnListTab", resourceFileName);
            }
        }
        public static string ReturnOrderCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnOrderCost", resourceFileName);
            }
        }
        public static string ReturnOrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnOrderNumber", resourceFileName);
            }
        }
        public static string ReturnOrderStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnOrderStatus", resourceFileName);
            }
        }
        public static string SendToEDI
        {
            get
            {
                return ResourceRead.GetResourceValue("SendToEDI", resourceFileName);
            }
        }
        public static string ReturnTab
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnTab", resourceFileName);
            }
        }
        public static string RequestedReturnQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedReturnQuantity", resourceFileName);
            }
        }
        public static string ItemModelHeaderRO
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemModelHeaderRO", resourceFileName);
            }
        }

        public static string NoteQuickListNewReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("NoteQuickListNewReceive", resourceFileName);
            }
        }

        public static string MSGReceivedNotEditable
        {
            get
            {
                return ResourceRead.GetResourceValue("MSGReceivedNotEditable", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CustomerGUID.
        /// </summary>
        public static string CustomerGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("Customer", resourceFileName);
            }
        }

        
        public static string CustomerName
        {
            get
            {
                return ResourceRead.GetResourceValue("Customer", resourceFileName);
            }
        }

        public static string ReceivedOnWeb
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOnWeb", resourceFileName);
            }
        }

        public static string ReceivedOn
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOn", resourceFileName);
            }
        }

        public static string EditedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("EditedFrom", resourceFileName);
            }
        }

        public static string AddedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("AddedFrom", resourceFileName);
            }
        }
        public static string Receive
        {
            get
            {
                return ResourceRead.GetResourceValue("Receive", resourceFileName);
            }
        }


        public static string OrdDtlInTransitQty
        {
            get
            {
                return ResourceRead.GetResourceValue("OrdDtlInTransitQty", resourceFileName);
            }
        }


        public static string OrderLineItemExtendedCost
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderLineItemExtendedCost", resourceFileName);
            }
        }



        public static string OrderLineItemExtendedPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("OrdDtlInTransitQty", resourceFileName);
            }
        }

        public static string ExtendedCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ExtendedCost", resourceFileName);
            }
        }
        public static string OrderType
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderType", resourceFileName);
            }
        }
        public static string AccountNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("AccountNumber", resourceFileName);
            }
        }
        public static string SupplierAccount
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccount", resourceFileName);
            }
        }

        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", resourceFileName);
            }
        }

        public static string IsCloseItem
        {
            get
            {
                return ResourceRead.GetResourceValue("IsCloseItem", resourceFileName);
            }
        }
        public static string LineNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("LineNumber", resourceFileName);
            }
        }
        public static string ControlNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ControlNumber", resourceFileName);
            }
        }
        public static string ItemComment
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemComment", resourceFileName);
            }
        }
        public static string OrdDtlUDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("OrdDtlUDF1", resourceFileName);
            }
        }
        public static string OrdDtlUDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("OrdDtlUDF2", resourceFileName);
            }
        }
        public static string OrdDtlUDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("OrdDtlUDF3", resourceFileName);
            }
        }
        public static string OrdDtlUDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("OrdDtlUDF4", resourceFileName);
            }
        }
        public static string OrdDtlUDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("OrdDtlUDF5", resourceFileName);
            }
        }
        public static string OrderUOMUnClose
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderUOMUnClose", resourceFileName);
            }
        }

        public static string ReplinishRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplinishRoom", resourceFileName);
            }
        }

        public static string SupplierApprove
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierApprove", resourceFileName);
            }
        }

        public static string OrderItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderItemCost", resourceFileName);
            }
        }
        public static string OrderItemSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderItemSellPrice", resourceFileName);
            }
        }
        public static string OrderItemMarkup
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderItemMarkup", resourceFileName);
            }
        }
        public static string OrderItemCostUOMValue
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderItemCostUOMValue", resourceFileName);
            }
        }
        public static string MsgValidationSelectOnlyclosedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgValidationSelectOnlyclosedOrder", resourceFileName);
            }
        }
        public static string MsgSelectRowToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRowToReceive", resourceFileName);
            }
        }

        public static string MoreThanThreeWeeks
        {
            get
            {
                return ResourceRead.GetResourceValue("MoreThanThreeWeeks", resourceFileName);
            }
        }
        public static string TwoToThreeWeeks
        {
            get
            {
                return ResourceRead.GetResourceValue("TwoToThreeWeeks", resourceFileName);
            }
        }
        public static string NextWeek
        {
            get
            {
                return ResourceRead.GetResourceValue("NextWeek", resourceFileName);
            }
        }
        public static string ThisWeek
        {
            get
            {
                return ResourceRead.GetResourceValue("ThisWeek", resourceFileName);
            }
        }
        public static string ReturnQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnQuantity", resourceFileName);
            }
        }
        public static string OrderNumberLengthUpto22Char
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderNumberLengthUpto22Char", resourceFileName);
            }
        }
        public static string MaxOrderSizeForSupplier
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxOrderSizeForSupplier", resourceFileName);
            }
        }
        
        public static string ReqClosedOrders
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqClosedOrders", resourceFileName);
            }
        }
        public static string SelectSingleRow
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectSingleRow", resourceFileName);
            }
        }
        public static string ReqOneUnClosedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqOneUnClosedOrder", resourceFileName);
            }
        }
        public static string ErrortoPerformOperation
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrortoPerformOperation", resourceFileName);
            }
        }
        public static string msgSavedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("msgSavedSuccessfully", resourceFileName);
            }
        }
        public static string msgNotSavedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("msgNotSavedSuccessfully", resourceFileName);
            }
        }
        public static string msgRequestedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("msgRequestedQuantity", resourceFileName);
            }
        }
        public static string OrderDetailUDFRequire
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderDetailUDFRequire", resourceFileName);
            }
        }
        public static string CantApproveMoreThanPerOrderItemQtyApprovalLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("CantApproveMoreThanPerOrderItemQtyApprovalLimit", resourceFileName);
            }
        }
        public static string ApprovedQtyNotMatchedWithLocationDefaultReOrderQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ApprovedQtyNotMatchedWwithLocationDefaultReOrderQty", resourceFileName);
            }
        }
        public static string RequestedQtyNotMatchedWithLocationDefaultReOrderQty
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedQtyNotMatchedWithLocationDefaultReOrderQty", resourceFileName);
            }
        }
        public static string RequestedQtyNotMatchedWithDefaultReOrderQty
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedQtyNotMatchedWithDefaultReOrderQty", resourceFileName);
            }
        }
        public static string CantApproveMoreThanRemainingOrderApprovalLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("CantApproveMoreThanRemainingOrderApprovalLimit", resourceFileName);
            }
        }
        public static string SupplierOrderCantApproveMTRemainingOrderApprovalLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierOrderCantApproveMTRemainingOrderApprovalLimit", resourceFileName);
            }
        }
        public static string CantApproveMoreThanPerOrderApprovalLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("CantApproveMoreThanPerOrderApprovalLimit", resourceFileName);
            }
        }
        public static string SupplierOrderCantApproveMTPerOrderApprovalLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierOrderCantApproveMTPerOrderApprovalLimit", resourceFileName);
            }
        }
        public static string OrderMustHaveOneLineItem
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderMustHaveOneLineItem", resourceFileName);
            }
        }
        public static string MailSendSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MailSendSuccessfully", resourceFileName);
            }
        }
        public static string OrderClosedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderClosedSuccessfully", resourceFileName);
            }
        }
        public static string SelectedOrdersClosedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectedOrdersClosedSuccessfully", resourceFileName);
            }
        }

        public static string OrderUnclosedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderUnclosedSuccessfully", resourceFileName);
            }
        }
        public static string NoOrdersFound
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOrdersFound", resourceFileName);
            }
        }
        public static string LineItemClosed
        {
            get
            {
                return ResourceRead.GetResourceValue("LineItemClosed", resourceFileName);
            }
        }
        public static string ItemHasOrderUOMIssue
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemHasOrderUOMIssue", resourceFileName);
            }
        }
        public static string PleaseSelectRecord
        {
            get
            {
                return ResourceRead.GetResourceValue("PleaseSelectRecord", resourceFileName);
            }
        }
        public static string AddedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("AddedSuccessfully", resourceFileName);
            }
        }
        public static string ItemNotAddedMaxQtyReached
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNotAddedMaxQtyReached", resourceFileName);
            }
        }
        public static string ApproveQuantityShouldbelessthanMaxQuantityLimitInSBPO
        {
            get
            {
                return ResourceRead.GetResourceValue("ApproveQuantityShouldbelessthanMaxQuantityLimitInSBPO", resourceFileName);
            }
        }
        public static string OrderLineItemExtendedCostShouldbelessthanMaxLimitInSBPO
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderLineItemExtendedCostShouldbelessthanMaxLimitInSBPO", resourceFileName);
            }
        }
        public static string NotAddedItemMaxQtyReached
        {
            get
            {
                return ResourceRead.GetResourceValue("NotAddedItemMaxQtyReached", resourceFileName);
            }
        }
        public static string ItemNotAddedBinMaxQtyReached
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNotAddedBinMaxQtyReached", resourceFileName);
            }
        }
        public static string NotAddedBinMaxQtyReached
        {
            get
            {
                return ResourceRead.GetResourceValue("NotAddedBinMaxQtyReached", resourceFileName);
            }
        }
        public static string ItemCantUnclosedItemMaxQtyReached
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCantUnclosedItemMaxQtyReached", resourceFileName);
            }
        }
        public static string ItemCantUnclosedBinMaxQtyReached
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCantUnclosedBinMaxQtyReached", resourceFileName);
            }
        }

        public static string NotInsert
        {
            get
            {
                return ResourceRead.GetResourceValue("NotInsert", resourceFileName);
            }
        }
        public static string ItemsAddedToOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsAddedToOrder", resourceFileName);
            }
        }
        public static string NotAddedItemsExistInOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("NotAddedItemsExistInOrder", resourceFileName);
            }
        }
        public static string ItemsAddedAndExistInOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsAddedAndExistInOrder", resourceFileName);
            }
        }
        public static string SelectOrderLineItemToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectOrderLineItemToReceive", resourceFileName);
            }
        }
        public static string ItemReceivedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemReceivedSuccessfully", resourceFileName);
            }
        }
        public static string SomeItemsNotReceivedDueToReason
        {
            get
            {
                return ResourceRead.GetResourceValue("SomeItemsNotReceivedDueToReason", resourceFileName);
            }
        }
        public static string OrderReceivedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderReceivedSuccessfully", resourceFileName);
            }
        }
        public static string SelectOrderLineItemToReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectOrderLineItemToReturn", resourceFileName);
            }
        }
        public static string SelectBinToReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectBinToReturn", resourceFileName);
            }
        }
        public static string ReturnedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnedSuccessfully", resourceFileName);
            }
        }
        public static string SomeItemsNotReturnedDueToReason
        {
            get
            {
                return ResourceRead.GetResourceValue("SomeItemsNotReturnedDueToReason", resourceFileName);
            }
        }
        public static string OrderReturnedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderReturnedSuccessfully", resourceFileName);
            }
        }

        public static string RecordNotDeletedDueToError
        {
            get
            {
                return ResourceRead.GetResourceValue("RecordNotDeletedDueToError", resourceFileName);
            }
        }
        public static string ItemSerialClickInlineReceiveButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemSerialClickInlineReceiveButton", resourceFileName);
            }
        }

        public static string ClosedItemCantReceiveClickInlineUncloseButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ClosedItemCantReceiveClickInlineUncloseButton", resourceFileName);
            }
        }
        public static string StagingBinHaveNotSufficientQtyToReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingBinHaveNotSufficientQtyToReturn", resourceFileName);
            }
        }
        public static string BinHaveNotSufficientQtyToReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("BinHaveNotSufficientQtyToReturn", resourceFileName);
            }
        }
        public static string ComponentOfKit
        {
            get
            {
                return ResourceRead.GetResourceValue("ComponentOfKit", resourceFileName);
            }
        }
        public static string SeeAttachedFilesForOrderDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("SeeAttachedFilesForOrderDetail", resourceFileName);
            }
        }
        public static string SeeAttachedFilesForReturnOrderDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("SeeAttachedFilesForReturnOrderDetail", resourceFileName);
            }
        }
        public static string ReturnOrderApprovalMailSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnOrderApprovalMailSubject", resourceFileName);
            }
        }
        public static string ReturnOrderToSupplierMailSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnOrderToSupplierMailSubject", resourceFileName);
            }
        }
        public static string OrderNumberDuplicateInList
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderNumberDuplicateInList", resourceFileName);
            }
        }
        public static string ReturnOrderNumberDuplicateInList
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnOrderNumberDuplicateInList", resourceFileName);
            }
        }
        public static string OrderNumberAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderNumberAlreadyExist", resourceFileName);
            }
        }
        public static string ReturnOrderNumberAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnOrderNumberAlreadyExist", resourceFileName);
            }
        }
        public static string OrderNo
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderNo", resourceFileName);
            }
        }
        public static string MsgItemNotAddedToOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemNotAddedToOrder", resourceFileName);
            }
        }
        public static string OrdersWillBeGenerated
        {
            get
            {
                return ResourceRead.GetResourceValue("OrdersWillBeGenerated", resourceFileName);
            }
        }
        public static string SalesOrderNumber { get { return ResourceRead.GetResourceValue("SalesOrderNumber", resourceFileName); } }
        public static string OrderCostMail { get { return ResourceRead.GetResourceValue("OrderCostMail", resourceFileName); } }
        public static string CloseSelectedOrders { get { return ResourceRead.GetResourceValue("CloseSelectedOrders", resourceFileName); } }
        public static string OrderDetailUDFSetup { get { return ResourceRead.GetResourceValue("OrderDetailUDFSetup", resourceFileName); } }
        public static string OrderPriceMail { get { return ResourceRead.GetResourceValue("OrderPriceMail", resourceFileName); } }
        
        public static string YourOrderHasBeenApproved { get { return ResourceRead.GetResourceValue("YourOrderHasBeenApproved", resourceFileName); } }
        public static string YourReturnOrderHasBeenApproved { get { return ResourceRead.GetResourceValue("YourReturnOrderHasBeenApproved", resourceFileName); } }
        public static string YourOrderHasAlreadyBeenApproved { get { return ResourceRead.GetResourceValue("YourOrderHasAlreadyBeenApproved", resourceFileName); } }
        public static string YourReturnOrderHasAlreadyBeenApproved { get { return ResourceRead.GetResourceValue("YourReturnOrderHasAlreadyBeenApproved", resourceFileName); } }
        public static string YourOrderIsNotValidToApproval { get { return ResourceRead.GetResourceValue("YourOrderIsNotValidToApproval", resourceFileName); } }
        public static string YourReturnOrderIsNotValidToApproval { get { return ResourceRead.GetResourceValue("YourReturnOrderIsNotValidToApproval", resourceFileName); } }
        public static string NoItemForThisOrder { get { return ResourceRead.GetResourceValue("NoItemForThisOrder", resourceFileName); } }
        public static string ReturnOrderHasAlreadyBeenRejected { get { return ResourceRead.GetResourceValue("ReturnOrderHasAlreadyBeenRejected", resourceFileName); } }
        public static string OrderHasAlreadyBeenRejected { get { return ResourceRead.GetResourceValue("OrderHasAlreadyBeenRejected", resourceFileName); } }
        public static string OrderHasRejected { get { return ResourceRead.GetResourceValue("OrderHasRejected", resourceFileName); } }
        public static string ReturnOrderHasRejected { get { return ResourceRead.GetResourceValue("ReturnOrderHasRejected", resourceFileName); } }
        public static string OrderIsNotValidToRejection { get { return ResourceRead.GetResourceValue("OrderIsNotValidToRejection", resourceFileName); } }
        public static string ReturnOrderIsNotValidToRejection { get { return ResourceRead.GetResourceValue("ReturnOrderIsNotValidToRejection", resourceFileName); } }
        public static string MsgFixedOrderNumberValidation { get { return ResourceRead.GetResourceValue("MsgFixedOrderNumberValidation", resourceFileName); } }
        public static string MsgBlanketNotDefineValidation { get { return ResourceRead.GetResourceValue("MsgBlanketNotDefineValidation", resourceFileName); } }
        public static string MsgPOSequanceNotDefineValidation { get { return ResourceRead.GetResourceValue("MsgPOSequanceNotDefineValidation", resourceFileName); } }
        public static string MsgWOSequanceNotDefineValidation { get { return ResourceRead.GetResourceValue("MsgWOSequanceNotDefineValidation", resourceFileName); } }
        
        public static string NotEnoughQtyOnHandQtyIsLessThanReturnQty { get { return ResourceRead.GetResourceValue("NotEnoughQtyOnHandQtyIsLessThanReturnQty", resourceFileName); } }
        public static string QuantityReturnedSuccessfully { get { return ResourceRead.GetResourceValue("QuantityReturnedSuccessfully", resourceFileName); } }

        public static string OrderNumberValidation { get { return ResourceRead.GetResourceValue("OrderNumberValidation", resourceFileName); } }

        public static string DuplicateOrderNumberValidation { get { return ResourceRead.GetResourceValue("DuplicateOrderNumberValidation", resourceFileName); } }
        public static string RequiredDateValidation { get { return ResourceRead.GetResourceValue("RequiredDateValidation", resourceFileName); } }

        public static string ValidOrderStatusValidation { get { return ResourceRead.GetResourceValue("ValidOrderStatusValidation", resourceFileName); } }
        public static string OrderStatusNotBlank { get { return ResourceRead.GetResourceValue("OrderStatusNotBlank", resourceFileName); } }
        public static string ValidOrderTypeValidation { get { return ResourceRead.GetResourceValue("ValidOrderTypeValidation", resourceFileName); } }
        public static string OrderTypeBlankValidation { get { return ResourceRead.GetResourceValue("OrderTypeBlankValidation", resourceFileName); } }
        public static string MsgRequestedQtyValidation { get { return ResourceRead.GetResourceValue("OrderTypeBlankValidation", resourceFileName); } }
        public static string MsgItemNotExistValidation { get { return ResourceRead.GetResourceValue("MsgItemNotExistValidation", resourceFileName); } }
        public static string MsgApprovedQuantityValidation { get { return ResourceRead.GetResourceValue("MsgApprovedQuantityValidation", resourceFileName); } }
        public static string MsgDefaultReorderQuantity { get { return ResourceRead.GetResourceValue("MsgDefaultReorderQuantity", resourceFileName); } }
        public static string MsgOrderNumberDuplicate { get { return ResourceRead.GetResourceValue("MsgOrderNumberDuplicate", resourceFileName); } }
        public static string MsgOrderNotAvailableForDateRange { get { return ResourceRead.GetResourceValue("MsgOrderNotAvailableForDateRange", resourceFileName); } }
        public static string ReqDateRange { get { return ResourceRead.GetResourceValue("ReqDateRange", resourceFileName); } }
        public static string MsgProvideCorrectInformation { get { return ResourceRead.GetResourceValue("MsgProvideCorrectInformation", resourceFileName); } }
        public static string MsgNoRightsForNewReceive { get { return ResourceRead.GetResourceValue("MsgNoRightsForNewReceive", resourceFileName); } }
        public static string MsgItemNotFound { get { return ResourceRead.GetResourceValue("MsgItemNotFound", resourceFileName); } }
        public static string MsgLaborItemCanNotReceived { get { return ResourceRead.GetResourceValue("MsgLaborItemCanNotReceived", resourceFileName); } }
        public static string MsgInActiveItemCanNotReceived { get { return ResourceRead.GetResourceValue("MsgInActiveItemCanNotReceived", resourceFileName); } }
        public static string QuantityIsRequired { get { return ResourceRead.GetResourceValue("QuantityIsRequired", resourceFileName); } }
        public static string QuickListDoesNotExist { get { return ResourceRead.GetResourceValue("QuickListDoesNotExist", resourceFileName); } }
        public static string ItemDoesNotExistOrInActive { get { return ResourceRead.GetResourceValue("ItemDoesNotExistOrInActive", resourceFileName); } }
        public static string LaborItemNotAllowedInOrder { get { return ResourceRead.GetResourceValue("LaborItemNotAllowedInOrder", resourceFileName); } }
        public static string InActiveItemsNotAllowedInOrder { get { return ResourceRead.GetResourceValue("InActiveItemsNotAllowedInOrder", resourceFileName); } }
        public static string UserDoesNotHaveRightToOrderConsignedItem { get { return ResourceRead.GetResourceValue("UserDoesNotHaveRightToOrderConsignedItem", resourceFileName); } }
        public static string UserDoesNotHaveRights { get { return ResourceRead.GetResourceValue("UserDoesNotHaveRights", resourceFileName); } }
        public static string LocationQtyNotMatchedWithDefaultReOrderQty { get { return ResourceRead.GetResourceValue("LocationQtyNotMatchedWithDefaultReOrderQty", resourceFileName); } }
        public static string QuantityNotMatchedWithDefaultReOrderQty { get { return ResourceRead.GetResourceValue("QuantityNotMatchedWithDefaultReOrderQty", resourceFileName); } }
        public static string OrderLineItemsContainsDuplicateSerials { get { return ResourceRead.GetResourceValue("OrderLineItemsContainsDuplicateSerials", resourceFileName); } }
        public static string SerialIsDuplicateForItem { get { return ResourceRead.GetResourceValue("SerialIsDuplicateForItem", resourceFileName); } }
        public static string ProvideSerialForItem { get { return ResourceRead.GetResourceValue("ProvideSerialForItem", resourceFileName); } }
        public static string SerialDuplicateForItem { get { return ResourceRead.GetResourceValue("SerialDuplicateForItem", resourceFileName); } }
        public static string ProvideSrNoOfItemMultipleOfOrderUOM { get { return ResourceRead.GetResourceValue("ProvideSrNoOfItemMultipleOfOrderUOM", resourceFileName); } }
        public static string ProvideLotNoOrQtyForItem { get { return ResourceRead.GetResourceValue("ProvideLotNoOrQtyForItem", resourceFileName); } }
        public static string ProvideLotNoWithValidQtyForItem { get { return ResourceRead.GetResourceValue("ProvideLotNoWithValidQtyForItem", resourceFileName); } }
        public static string ProvideLotNoForItem { get { return ResourceRead.GetResourceValue("ProvideLotNoForItem", resourceFileName); } }
        public static string UserHasNoRightToInsertOrder { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertOrder", resourceFileName); } }
        public static string UserHasNoRightToInsertReceive { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertReceive", resourceFileName); } }
        public static string UserHasNoRightsToInsertOrEditOrder { get { return ResourceRead.GetResourceValue("UserHasNoRightsToInsertOrEditOrder", resourceFileName); } }
        public static string UserHasNoRightToEditOrder { get { return ResourceRead.GetResourceValue("UserHasNoRightToEditOrder", resourceFileName); } }
        public static string UserHasToAcceptLicenceBeforeCreateOrder { get { return ResourceRead.GetResourceValue("UserHasToAcceptLicenceBeforeCreateOrder", resourceFileName); } }
        
        public static string UserDoesNotHaveRightToCreateOrder { get { return ResourceRead.GetResourceValue("UserDoesNotHaveRightToCreateOrder", resourceFileName); } }
        public static string OrderContainsDuplicateLineItems { get { return ResourceRead.GetResourceValue("OrderContainsDuplicateLineItems", resourceFileName); } }
        public static string OrderLineItemContainsDuplicateSerial { get { return ResourceRead.GetResourceValue("OrderLineItemContainsDuplicateSerial", resourceFileName); } }
        public static string OrderMustHaveMinimumOneSuccessLineItem { get { return ResourceRead.GetResourceValue("OrderMustHaveMinimumOneSuccessLineItem", resourceFileName); } }

        public static string MsgEnterValidQtyForSerialNumber { get { return ResourceRead.GetResourceValue("MsgEnterValidQtyForSerialNumber", resourceFileName); } }
        public static string MsgEnterValidQtyForLotNumber { get { return ResourceRead.GetResourceValue("MsgEnterValidQtyForLotNumber", resourceFileName); } }
        public static string MsgDuplicateSerialNumberForItem { get { return ResourceRead.GetResourceValue("MsgDuplicateSerialNumberForItem", resourceFileName); } }
        public static string ErrorInReceiveOrder { get { return ResourceRead.GetResourceValue("ErrorInReceiveOrder", resourceFileName); } }
        public static string MsgOrderCreatedWithError { get { return ResourceRead.GetResourceValue("MsgOrderCreatedWithError", resourceFileName); } }
        public static string MsgOrderXMLFileGenerateSuccessfully { get { return ResourceRead.GetResourceValue("MsgOrderXMLFileGenerateSuccessfully", resourceFileName); } }
        public static string MsgOrderInvalidXMLFileFormat { get { return ResourceRead.GetResourceValue("MsgOrderInvalidXMLFileFormat", resourceFileName); } }

        public static string OrderNotAllowToApproveForSupplier { get { return ResourceRead.GetResourceValue("OrderNotAllowToApproveForSupplier",resourceFileName); } }
        public static string RequiredQtyMustBeGreaterThanZero { get { return ResourceRead.GetResourceValue("RequiredQtyMustBeGreaterThanZero",resourceFileName); } }
        public static string ApprovedOrderMustHaveApprovedQtyGreaterThanZero { get { return ResourceRead.GetResourceValue("ApprovedOrderMustHaveApprovedQtyGreaterThanZero",resourceFileName); } }
        public static string ApprovedQtyMustBeLessOrEqualRequestedQty { get { return ResourceRead.GetResourceValue("ApprovedQtyMustBeLessOrEqualRequestedQty",resourceFileName); } }
        public static string ItemDoesNotAllowToAddInTransmittedOrder { get { return ResourceRead.GetResourceValue("ItemDoesNotAllowToAddInTransmittedOrder",resourceFileName); } }
        public static string ApprovedQtyIsNotMatchedWithDefaultReorderQty { get { return ResourceRead.GetResourceValue("ApprovedQtyIsNotMatchedWithDefaultReorderQty",resourceFileName); } }
        public static string LocationRequiredQtyNotMatchedWithDefaultReorderQty { get { return ResourceRead.GetResourceValue("LocationRequiredQtyNotMatchedWithDefaultReorderQty",resourceFileName); } }
        public static string RequiredQtyNotMatchedWithDefaultReorderQty { get { return ResourceRead.GetResourceValue("RequiredQtyNotMatchedWithDefaultReorderQty",resourceFileName); } }
        public static string UserHasNoRightToOrderStatus { get { return ResourceRead.GetResourceValue("UserHasNoRightToOrderStatus",resourceFileName); } }
        public static string ProvideHigherOrderStatusThanCurrent { get { return ResourceRead.GetResourceValue("ProvideHigherOrderStatusThanCurrent",resourceFileName); } }
        public static string MsgObjectisNotProper { get { return ResourceRead.GetResourceValue("MsgObjectisNotProper", resourceFileName); } }
        public static string OrderNumberRequired { get { return ResourceRead.GetResourceValue("OrderNumberRequired", resourceFileName); } }
        
        public static string OrderNumberIsInvalid{ get { return ResourceRead.GetResourceValue("OrderNumberIsInvalid", resourceFileName); } }
        public static string AddFromCatalogButton { get { return ResourceRead.GetResourceValue("AddFromCatalogButton", resourceFileName); } }
        public static string ReceiveAll { get { return ResourceRead.GetResourceValue("ReceiveAll", resourceFileName); } }
        public static string ClearAll { get { return ResourceRead.GetResourceValue("ClearAll", resourceFileName); } }
        public static string SelectAll { get { return ResourceRead.GetResourceValue("SelectAll", resourceFileName); } }
        public static string CloseOrder { get { return ResourceRead.GetResourceValue("CloseOrder", resourceFileName); } }
        public static string ReturnALL { get { return ResourceRead.GetResourceValue("ReturnALL", resourceFileName); } }
        public static string CloseReturnOrder { get { return ResourceRead.GetResourceValue("CloseReturnOrder", resourceFileName); } }
        public static string CloseLineItem { get { return ResourceRead.GetResourceValue("CloseLineItem", resourceFileName); } }
        public static string UncloseItem { get { return ResourceRead.GetResourceValue("UncloseItem", resourceFileName); } }
        public static string ViewReceived { get { return ResourceRead.GetResourceValue("ViewReceived", resourceFileName); } }
        public static string SaveBin { get { return ResourceRead.GetResourceValue("SaveBin", resourceFileName); } }
        public static string ConfirmCloseOrder { get { return ResourceRead.GetResourceValue("ConfirmCloseOrder", resourceFileName); } }
        public static string ConfirmCloseOrderItem { get { return ResourceRead.GetResourceValue("ConfirmCloseOrderItem", resourceFileName); } }
        public static string ConfirmCloseselectedOrderItem { get { return ResourceRead.GetResourceValue("ConfirmCloseselectedOrderItem", resourceFileName); } }
        public static string ConfirmUnCloseOrderItem { get { return ResourceRead.GetResourceValue("ConfirmUnCloseOrderItem", resourceFileName); } }
        public static string TitleOrderListHistory { get { return ResourceRead.GetResourceValue("TitleOrderListHistory", resourceFileName); } }
        public static string PrintPurchaseOrder { get { return ResourceRead.GetResourceValue("PrintPurchaseOrder", resourceFileName); } }
        public static string TotalSellPrice { get { return ResourceRead.GetResourceValue("TotalSellPrice", resourceFileName); } }
        public static string TotalCost { get { return ResourceRead.GetResourceValue("TotalCost", resourceFileName); } }
        public static string MsgApprovedQtygreaterthanonHandQty { get { return ResourceRead.GetResourceValue("MsgApprovedQtygreaterthanonHandQty", resourceFileName); } }
        public static string ReturnedDateValidation { get { return ResourceRead.GetResourceValue("ReturnedDateValidation", resourceFileName); } }
        public static string MsgRequestedReturnedQtyValidation { get { return ResourceRead.GetResourceValue("MsgRequestedReturnedQtyValidation", resourceFileName); } }
        public static string MsgItemNotAllowed { get { return ResourceRead.GetResourceValue("MsgItemNotAllowed", resourceFileName); } }
        public static string ItemnotOrderable { get { return ResourceRead.GetResourceValue("ItemnotOrderable", resourceFileName); } }
        public static string OrderFilesLabel { get { return ResourceRead.GetResourceValue("OrderFilesLabel", resourceFileName); } }
        public static string ReceivedDetail { get { return ResourceRead.GetResourceValue("ReceivedDetail", resourceFileName); } }
        public static string ReturnOrderCostRequired { get { return ResourceRead.GetResourceValue("ReturnOrderCostRequired", resourceFileName); } }
        public static string OrderImageError { get { return ResourceRead.GetResourceValue("OrderImageError", resourceFileName); } }
        public static string NoItemLineAvailable { get { return ResourceRead.GetResourceValue("NoItemLineAvailable", resourceFileName); } }
        public static string ISBackOrdered { get { return ResourceRead.GetResourceValue("ISBackOrdered", resourceFileName); } }
        public static string BackOrderedExpandCollapse { get { return ResourceRead.GetResourceValue("BackOrderedExpandCollapse", resourceFileName); } }
        public static string SalesOrder { get { return ResourceRead.GetResourceValue("SalesOrder", resourceFileName); } }
        public static string POItemLineNumber { get { return ResourceRead.GetResourceValue("POItemLineNumber", resourceFileName); } }
        public static string OrderLineException { get { return ResourceRead.GetResourceValue("OrderLineException", resourceFileName); } }
        public static string OrderLineExceptionDesc { get { return ResourceRead.GetResourceValue("OrderLineExceptionDesc", resourceFileName); } }

    }

    public class RPT_OrderMasterDTO
    {

        public long ID { get; set; }
        public string OrderNumber { get; set; }
        public string InvoiceBranch { get; set; }
        public Guid OrderGUID { get; set; }
        public string ReleaseNumber { get; set; }
        public string SupplierName { get; set; }
        public string StagingName { get; set; }
        public string Comment { get; set; }
        public string RequiredDate { get; set; }
        public string OrderStatus { get; set; }
        public string Customer { get; set; }
        public string PackSlipNumber { get; set; }
        public string ShippingTrackNumber { get; set; }
        //public string Created { get; set; }
        //public DateTime Date { get; set; }
        //public DateTime Time { get; set; }
        //public string LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public string IsDeleted { get; set; }
        public string IsArchived { get; set; }
        public Guid GUID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string ShipVia { get; set; }
        public int OrderType { get; set; }
        public double? OrderCost { get; set; }
        public int NoOfLineItems { get; set; }
        public DateTime OrderDate { get; set; }
        public long? ChangeOrderRevisionNo { get; set; }
        public string ShippingVendor { get; set; }
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
        public long Supplier { get; set; }
        public string BarcodeImage_OrderNumber { get; set; }
        public string BarcodeImage_PackSlipNumber { get; set; }
        public string BarcodeImage_ShippingTrackNumber { get; set; }
        public string ContactName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        public string CurrentDateTime { get; set; }
        public string Supplier_StreetAddress { get; set; }
        public string Supplier_City { get; set; }
        public string Supplier_State { get; set; }
        public string Supplier_Zipcode { get; set; }
        public string Supplier_Contact { get; set; }
        public string Supplier_Email { get; set; }
        public string Supplier_Country { get; set; }
        public string Supplier_Phone { get; set; }
        public string Supplier_Fax { get; set; }
        public string SupplierWithFullAddress { get; set; }
        public string SupplierAccountNumberName { get; set; }
        public string SupplierAccountNumber { get; set; }
        public string SupplierAccountName { get; set; }
        public string SupplierAccountAddress { get; set; }
        public string SupplierAccountCity { get; set; }
        public string SupplierAccountState { get; set; }
        public string SupplierAccountZipcode { get; set; }
        public string SupplierAccountDetailWithFullAddress { get; set; }
        public string OrderId { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }

    }


    public  class OrderImageDetail
    {
        public long ID { get; set; }
        public Nullable<System.Guid> OrderGuid { get; set; }
        public string ImageName { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<long> LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<System.DateTime> ReceivedOn { get; set; }
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public Nullable<long> RoomId { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public string WhatWhereAction { get; set; }
    }
}


