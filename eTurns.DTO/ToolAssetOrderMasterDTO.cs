using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;



namespace eTurns.DTO
{
    public enum ToolAssetOrderStatus
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
    public enum ToolAssetOrderType
    {
        Order = 1,
        //RuturnOrder = 2
    }

    public class AutoToolAssetOrderNumberGenerate
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

    public class ToolAssetOrderMasterDTO
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        [Display(Name = "ToolAssetOrderNumber", ResourceType = typeof(ResToolAssetOrder))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(22, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ToolAssetOrderNumber { get; set; }

        [Display(Name = "ReleaseNumber", ResourceType = typeof(ResToolAssetOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ReleaseNumber { get; set; }



        [Display(Name = "Comment", ResourceType = typeof(ResToolAssetOrder))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Comment { get; set; }

        [Display(Name = "RequiredDate", ResourceType = typeof(ResToolAssetOrder))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public System.DateTime RequiredDate { get; set; }
        //private string _RequiredDateStr;

        //public System.String RequiredDateStr { get; set; }


        private string _requiredDate;

        [Display(Name = "RequiredDate", ResourceType = typeof(ResToolAssetOrder))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
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



        [Display(Name = "OrderStatus", ResourceType = typeof(ResToolAssetOrder))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int32 OrderStatus { get; set; }


        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResToolAssetOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String PackSlipNumber { get; set; }


        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> RoomID { get; set; }

        public Boolean IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResToolAssetOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResToolAssetOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResToolAssetOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResToolAssetOrder))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResToolAssetOrder))]
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

        public int? OrderType { get; set; }

        [Display(Name = "OrderCost")]
        public Nullable<System.Double> OrderCost { get; set; }
        [Display(Name = "NoOfLineItems")]
        public Nullable<int> NoOfLineItems { get; set; }

        [Display(Name = "OrderDate", ResourceType = typeof(ResToolAssetOrder))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> OrderDate { get; set; }

        [Display(Name = "ChangeOrderRevisionNo", ResourceType = typeof(ResToolAssetOrder))]
        public Nullable<Int64> ChangeOrderRevisionNo { get; set; }


        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public bool IsHistory { get; set; }


        public string OrderStatusText { get; set; }
        public char OrderStatusChar
        {
            get
            {
                return FnCommon.GetToolAssetOrderStatusChar(OrderStatus);
            }
        }
        public bool IsRecordNotEditable { get; set; }
        public bool IsOnlyStatusUpdate { get; set; }
        public bool OrderIsInReceive { get; set; }
        public bool IsAbleToDelete { get; set; }
        public List<ToolAssetOrderDetailsDTO> OrderListItem { get; set; }
        public string AppendedBarcodeString { get; set; }
        public int TotalRecords { get; set; }
        public AutoOrderNumberGenerate AutoOrderNumber { get; set; }
        public bool IsBlanketOrder { get; set; }


        public bool IsChangeOrderClick { get; set; }
        public System.String WhatWhereAction { get; set; }
        public System.String EditedOnAction { get; set; }

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

        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResToolAssetOrder))]
        [AllowHtml]
        public System.String OMPackSlipNumbers { get; set; }

        [Display(Name = "OrderNumber_ForSorting", ResourceType = typeof(ResToolAssetOrder))]
        [AllowHtml]
        public System.String OrderNumber_ForSorting { get; set; }



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

        //public string CostUOM { get; set; }

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



        public System.String OrderLineItemUDF1 { get; set; }
        public System.String OrderLineItemUDF2 { get; set; }
        public System.String OrderLineItemUDF3 { get; set; }
        public System.String OrderLineItemUDF4 { get; set; }
        public System.String OrderLineItemUDF5 { get; set; }

        public string CompanyName { get; set; }

    }


    public class ResToolAssetOrder
    {
        private static string resourceFileName = "ResToolAssetOrder";

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
        public static string ToolAssetOrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolAssetOrderNumber", resourceFileName);
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

        public static string AddToolToOrder { get { return ResourceRead.GetResourceValue("AddToolToOrder", resourceFileName); } }
        public static string ToolAddedToOrderSuccessfully { get { return ResourceRead.GetResourceValue("ToolAddedToOrderSuccessfully", resourceFileName); } }
        public static string ToolsAreAddedToOrder { get { return ResourceRead.GetResourceValue("ToolsAreAddedToOrder", resourceFileName); } }
    }

}


