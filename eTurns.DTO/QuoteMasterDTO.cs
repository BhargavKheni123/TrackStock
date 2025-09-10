using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public enum QuoteStatus
    {
        UnSubmitted = 1,
        Submitted = 2,
        Approved = 3,
        Transmitted = 4,
        TransmittedIncomplete = 5,
        TransmittedPastDue = 6,
        TransmittedInCompletePastDue = 7,
        Closed = 8
    }

    public class AutoQuoteNumberGenerate
    {
        public string QuoteNumber { get; set; }
        public bool IsBlank { get; set; }
        public bool IsBlanketPO { get; set; }
        public int QuoteNumberFormateType { get; set; }
        public string QuoteGeneratedFrom { get; set; }
        public string ErrorDescription { get; set; }
        public string QuoteNumberForSorting { get; set; }
        public long LastUsedTempIncrementNumberRoom { get; set; }

        public long LastUsedTempIncrementNumberSupplier { get; set; }

        public List<SupplierBlanketPODetailsDTO> BlanketPOs { get; set; }

    }

    public class QuoteMasterDTO
    {
        #region [Properties for Quote Master module]

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public long ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        [Display(Name = "QuoteNumber", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(22, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string QuoteNumber { get; set; }

        [Display(Name = "QuoteStatus", ResourceType = typeof(ResQuoteMaster))]
        public int QuoteStatus { get; set; }


        [Display(Name = "ReleaseNumber", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string ReleaseNumber { get; set; }

        [Display(Name = "Comment", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string Comment { get; set; }

        [Display(Name = "RequiredDate", ResourceType = typeof(ResQuoteMaster))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime RequiredDate { get; set; }

        [Display(Name = "QuoteDate", ResourceType = typeof(ResQuoteMaster))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? QuoteDate { get; set; }


        [Display(Name = "QuoteCost", ResourceType = typeof(ResQuoteMaster))]
        public double? QuoteCost { get; set; }

        [Display(Name = "QuotePrice", ResourceType = typeof(ResQuoteMaster))]
        public double? QuotePrice { get; set; }

        [Display(Name = "NoOfLineItems", ResourceType = typeof(ResQuoteMaster))]
        public int? NoOfLineItems { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public long Room { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public long CompanyID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF5 { get; set; }

        [Display(Name = "AddedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string EditedFrom { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public long CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public long LastUpdatedBy { get; set; }


        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public DateTime LastUpdated { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResItemMaster))]
        public DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResItemMaster))]
        public DateTime ReceivedOnWeb { get; set; }

        public long? RequesterID { get; set; }
        public long? ApproverID { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsEDIQuote { get; set; }

        [Display(Name = "QuoteSupplierNamesCSV", ResourceType = typeof(ResQuoteMaster))]
        public string QuoteSupplierNamesCSV { get; set; }

        [Display(Name = "QuoteSupplierIdsCSV", ResourceType = typeof(ResQuoteMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string QuoteSupplierIdsCSV { get; set; }
        public string QuoteNumber_ForSorting { get; set; }
        public string WhatWhereAction { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        private string _requiredDate;

        [Display(Name = "RequiredDateStr", ResourceType = typeof(ResQuoteMaster))]
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

        #endregion

        #region [Quote Master Properties Added with probable use in future]
        [Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
        public long? CustomerID { get; set; }

        [Display(Name = "CustomerGUID", ResourceType = typeof(ResQuoteMaster))]
        public Guid? CustomerGUID { get; set; }

        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string PackSlipNumber { get; set; }

        [Display(Name = "ShippingTrackNumber", ResourceType = typeof(ResQuoteMaster))]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string ShippingTrackNumber { get; set; }

        [Display(Name = "ShipVia", ResourceType = typeof(ResQuoteMaster))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public long? ShipVia { get; set; }

        [Display(Name = "Address", ResourceType = typeof(Resources.ResCommon))]
        public string CustomerAddress { get; set; }

        [Display(Name = "Supplier", ResourceType = typeof(ResQuoteMaster))]
        public long? Supplier { get; set; }

        [Display(Name = "ChangeQuoteRevisionNo", ResourceType = typeof(ResQuoteMaster))]
        public long? ChangeQuoteRevisionNo { get; set; }

        [Display(Name = "ShippingVendor", ResourceType = typeof(ResQuoteMaster))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "NumberOnly", ErrorMessageResourceType = typeof(ResMessage))]
        public long? ShippingVendor { get; set; }
        public string AccountNumber { get; set; }
        public string RejectionReason { get; set; }
        [Display(Name = "SupplierAccountDetail", ResourceType = typeof(ResQuoteMaster))]
        public Guid? SupplierAccountGuid { get; set; }
        #endregion

        #region [Un used Or Supportive properties]      
        public bool? IsArchived { get; set; }
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }
        public int? QuoteType { get; set; }
        public long? BlanketQuoteNumberID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }
        public bool IsHistory { get; set; }
        public string SupplierName { get; set; }

        [Display(Name = "ShipViaName", ResourceType = typeof(ResQuoteMaster))]
        public string ShipViaName { get; set; }

        [Display(Name = "StagingName", ResourceType = typeof(ResQuoteMaster))]
        public string StagingName { get; set; }

        [Display(Name = "ShippingVendorName", ResourceType = typeof(ResQuoteMaster))]
        public string ShippingVendorName { get; set; }

        [Display(Name = "CustomerName", ResourceType = typeof(ResQuoteMaster))]
        public string CustomerName { get; set; }
        public string QuoteStatusText { get; set; }
        public char QuoteStatusChar
        {
            get
            {
                return FnCommon.GetQuoteStatusChar(QuoteStatus);
            }
        }
        public bool IsRecordNotEditable { get; set; }
        public bool IsOnlyStatusUpdate { get; set; }
        public bool QuoteIsInReceive { get; set; }
        public bool IsAbleToDelete { get; set; }
        public List<QuoteDetailDTO> QuoteListItem { get; set; }
        public string AppendedBarcodeString { get; set; }
        public int TotalRecords { get; set; }
        public bool IsBlanketQuote { get; set; }
        public Int64? StagingDefaultLocation { get; set; }
        public bool IsChangeQuoteClick { get; set; }
        public string Indicator { get; set; }
        public bool IsQuoteSelected { get; set; }
        public string QuoteLineItemsIds { get; set; }
        public DateTime ChangeQuoteCreated { get; set; }
        public DateTime ChangeQuoteLastUpdated { get; set; }
        public Int64 ChangeQuoteCreatedBy { get; set; }
        public Int64 ChangeQuoteLastUpdatedBy { get; set; }
        public Guid ChangeQuoteGUID { get; set; }
        public Int64 ChangeQuoteID { get; set; }

        [Display(Name = "ChangeQuoteCreatedBy", ResourceType = typeof(ResCommon))]
        public string ChangeQuoteCreatedByName { get; set; }

        [Display(Name = "ChangeQuoteUpdatedBy", ResourceType = typeof(ResCommon))]
        public string ChangeQuoteUpdatedByName { get; set; }
        public bool IsMainQuoteInChangeQuoteHistory { get; set; }

        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResQuoteMaster))]
        [AllowHtml]
        public string OMPackSlipNumbers { get; set; }
        public int InCompleteItemCount { get; set; }
        public Guid? MaterialStagingGUID { get; set; }
        public bool IsOnlyFromUI { get; set; }

        private string _changeQuoteCreated;
        public string ChangeQuoteCreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_changeQuoteCreated))
                {
                    _changeQuoteCreated = FnCommon.ConvertDateByTimeZone(ChangeQuoteCreated, true);
                }
                return _changeQuoteCreated;
            }
            set { this._changeQuoteCreated = value; }
        }

        private string _changeQuoteLastUpdated;

        public string ChangeQuoteUpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_changeQuoteLastUpdated))
                {
                    _changeQuoteLastUpdated = FnCommon.ConvertDateByTimeZone(ChangeQuoteLastUpdated, true);
                }
                return _changeQuoteLastUpdated;
            }
            set { this._changeQuoteLastUpdated = value; }
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
        public double? OnQuoteInTransitQuantity { get; set; }
        public string SupplierAccountNumberName { get; set; }
        public string SupplierAccountNumber { get; set; }
        public string SupplierAccountName { get; set; }
        public string SupplierAccountAddress { get; set; }
        public string SupplierAccountCity { get; set; }
        public string SupplierAccountState { get; set; }
        public string SupplierAccountZipcode { get; set; }
        public string SupplierAccountDetailWithFullAddress { get; set; }
        public string QuoteLineItemUDF1 { get; set; }
        public string QuoteLineItemUDF2 { get; set; }
        public string QuoteLineItemUDF3 { get; set; }
        public string QuoteLineItemUDF4 { get; set; }
        public string QuoteLineItemUDF5 { get; set; }
        public string CompanyName { get; set; }
        public int PriseSelectionOption { get; set; }
        public bool IsSupplierApprove { get; set; }
        public bool IsQuoteReleaseNumberEditable { get; set; }
        public string QuoteQuantityString { get; set; }
        public string SelectedSuppliers { get; set; }
        #endregion
    }

    public class ResQuoteMaster
    {
        private static string resourceFileName = "ResQuoteMaster";


        public static string Bin
        {
            get
            {
                return ResourceRead.GetResourceValue("Bin", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Please add items for save Quote.
        /// </summary>
        public static string BlankItemSavedMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("BlankItemSavedMessage", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change Quote.
        /// </summary>
        public static string ChangeQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeQuote", resourceFileName);
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

        public static string UncloseQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("UncloseQuote", resourceFileName);
            }
        }

        public static string EditReceipts
        {
            get
            {
                return ResourceRead.GetResourceValue("EditReceipts", resourceFileName);
            }
        }

        public static string EditQuoteLineItems
        {
            get
            {
                return ResourceRead.GetResourceValue("EditQuoteLineItems", resourceFileName);
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
        ///   Looks up a localized string similar to Add Items to Quote.
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
        ///   Looks up a localized string similar to Quote.
        /// </summary>
        public static string MenuLinkText
        {
            get
            {
                return ResourceRead.GetResourceValue("MenuLinkText", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quote Approval Request.
        /// </summary>
        public static string QuoteApprovalleMailSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteApprovalleMailSubject", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quote GUID.
        /// </summary>
        public static string QuoteGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteGUID", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quote ID.
        /// </summary>
        public static string QuoteID
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteID", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quote Number.
        /// </summary>
        public static string QuoteNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteNumber", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quote Status.
        /// </summary>
        public static string QuoteStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteStatus", resourceFileName);
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
        ///   Looks up a localized string similar to Quotes.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Quotes.
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
                return ResourceRead.GetResourceValue("UDF1", resourceFileName, true);
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
        public static string GetQuoteStatusText(string value)
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
        ///   Looks up a localized string similar to QuoteDate.
        /// </summary>
        public static string QuoteDate
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteDate", resourceFileName);
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
        ///   Looks up a localized string similar to ChangeQuoteRevisionNo.
        /// </summary>
        public static string ChangeQuoteRevisionNo
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeQuoteRevisionNo", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Quote Approval Request.
        /// </summary>
        public static string QuoteToSupplierMailSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteToSupplierMailSubject", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to ChangeQuoteRevisionNo.
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
        public static string ChangeReturnQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeReturnQuote", resourceFileName);
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
        public static string QuoteCost
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteCost", resourceFileName);
            }
        }
        public static string QuotePrice
        {
            get
            {
                return ResourceRead.GetResourceValue("QuotePrice", resourceFileName);
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
        public static string ReturnQuoteCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnQuoteCost", resourceFileName);
            }
        }
        public static string ReturnQuoteNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnQuoteNumber", resourceFileName);
            }
        }
        public static string ReturnQuoteStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("ReturnQuoteStatus", resourceFileName);
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


        public static string QuoteLineItemExtendedCost
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteLineItemExtendedCost", resourceFileName);
            }
        }



        public static string QuoteLineItemExtendedPrice
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
        public static string QuoteType
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteType", resourceFileName);
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
        public static string QuoteUOMUnClose
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteUOMUnClose", resourceFileName);
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

        public static string QuoteItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteItemCost", resourceFileName);
            }
        }
        public static string QuoteItemSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteItemSellPrice", resourceFileName);
            }
        }
        public static string QuoteItemMarkup
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteItemMarkup", resourceFileName);
            }
        }
        public static string QuoteItemCostUOMValue
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteItemCostUOMValue", resourceFileName);
            }
        }

        public static string msgQuoteItemSelect
        {
            get
            {
                return ResourceRead.GetResourceValue("msgQuoteItemSelect", resourceFileName);
            }
        }
        public static string alertQuoteCreated//Quote(s) will be created.
        {
            get
            {
                return ResourceRead.GetResourceValue("alertQuoteCreated", resourceFileName);
            }
        }

        public static string CreateOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("CreateOrder", resourceFileName);
            }
        }
        public static string QuoteRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteRequired", resourceFileName);
            }
        }

        public static string QuoteItemRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteItemRequired", resourceFileName);
            }
        }

        public static string SelectAtleaseOneQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectAtleaseOneQuote", resourceFileName);
            }
        }
        public static string QuoteSupplierNamesCSV
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteSupplierNamesCSV", resourceFileName);
            }
        }
        public static string QuoteSupplierIdsCSV
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteSupplierIdsCSV", resourceFileName);
            }
        }
        public static string CloseQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("CloseQuote", resourceFileName);
            }
        }
        public static string CloseQuoteLineItem
        {
            get
            {
                return ResourceRead.GetResourceValue("CloseQuoteLineItem", resourceFileName);
            }
        }

        public static string QuoteLegendNotOrderable
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteLegendNotOrderable", resourceFileName);
            }
        }

        public static string OrderedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderedQuantity", resourceFileName);
            }
        }

        public static string ItemsAddedToQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsAddedToQuote", resourceFileName);
            }
        }
        public static string NotAddedItemsExistInQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("NotAddedItemsExistInQuote", resourceFileName);
            }
        }
        public static string ItemsAddedAndExistInQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsAddedAndExistInQuote", resourceFileName);
            }
        }
        public static string SeeAttachedFilesForQuoteDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("SeeAttachedFilesForQuoteDetail", resourceFileName);
            }
        }
        public static string QuoteNumberLengthUpto22Char
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteNumberLengthUpto22Char", resourceFileName);
            }
        }
        public static string RequiredFor
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredFor", resourceFileName);
            }
        }
        public static string QuoteMustHaveOneLineItem
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteMustHaveOneLineItem", resourceFileName);
            }
        }
        public static string SelectedQuotesClosedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectedQuotesClosedSuccessfully", resourceFileName);
            }
        }
        public static string ValidationFailedFor
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidationFailedFor", resourceFileName);
            }
        }
        public static string QuoteNumberDuplicateInList
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteNumberDuplicateInList", resourceFileName);
            }
        }
        public static string QuoteNumberAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteNumberAlreadyExist", resourceFileName);
            }
        }
        public static string BelowGivenItemsRejected
        {
            get
            {
                return ResourceRead.GetResourceValue("BelowGivenItemsRejected", resourceFileName);
            }
        }
        public static string BelowGivenSuppliersRejected
        {
            get
            {
                return ResourceRead.GetResourceValue("BelowGivenSuppliersRejected", resourceFileName);
            }
        }  
        public static string NotAddedSupplierMaxOrderSizeReached
        {
            get
            {
                return ResourceRead.GetResourceValue("NotAddedSupplierMaxOrderSizeReached", resourceFileName);
            }
        }        

        public static string MsgSelectClosedQuotes
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectClosedQuotes", resourceFileName);
            }
        }
        public static string ReqOrderToCreate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqOrderToCreate", resourceFileName);
            }
        }
        public static string ReqOneSupplier
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqOneSupplier", resourceFileName);
            }
        }
        public static string msgRequestedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("msgRequestedQuantity", resourceFileName);
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
        public static string MsgItemNotAddedToQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemNotAddedToQuote", resourceFileName);
            }
        }
        public static string Quotes
        {
            get
            {
                return ResourceRead.GetResourceValue("Quotes", resourceFileName);
            }
        }
        public static string CloseSelectedQuotes { get { return ResourceRead.GetResourceValue("CloseSelectedQuotes", resourceFileName); } }
        public static string QuoteDetailUDFSetup { get { return ResourceRead.GetResourceValue("QuoteDetailUDFSetup", resourceFileName); } }
        public static string MsgQuoteNumberValidation { get { return ResourceRead.GetResourceValue("MsgQuoteNumberValidation", resourceFileName); } }
        public static string MsgDuplicateQuoteNumber { get { return ResourceRead.GetResourceValue("MsgDuplicateQuoteNumber", resourceFileName); } }
        public static string MsgValidQuoteStatus { get { return ResourceRead.GetResourceValue("MsgValidQuoteStatus", resourceFileName); } }
        public static string MsgQuoteStatusBlank { get { return ResourceRead.GetResourceValue("MsgQuoteStatusBlank", resourceFileName); } }
        public static string MsgQuoteNumberDuplicate { get { return ResourceRead.GetResourceValue("MsgQuoteNumberDuplicate", resourceFileName); } }
        public static string PrintQuote { get { return ResourceRead.GetResourceValue("PrintQuote", resourceFileName); } }
        public static string SupplierFilter { get { return ResourceRead.GetResourceValue("SupplierFilter", resourceFileName); } }
        public static string NooflineItems { get { return ResourceRead.GetResourceValue("NooflineItems", resourceFileName); } }
        public static string TotalSellPrice { get { return ResourceRead.GetResourceValue("TotalSellPrice", resourceFileName); } }
        public static string TotalCost { get { return ResourceRead.GetResourceValue("TotalCost", resourceFileName); } }
        public static string TitleCreateNewItem { get { return ResourceRead.GetResourceValue("TitleCreateNewItem", resourceFileName); } }
        public static string QuantityNotMatchedWithDefaultReOrderQty { get { return ResourceRead.GetResourceValue("QuantityNotMatchedWithDefaultReOrderQty", resourceFileName); } }
        public static string cnfrmNewlyAddedItems { get { return ResourceRead.GetResourceValue("cnfrmNewlyAddedItems", resourceFileName); } }
        public static string QuoteUnclosedSuccessfully { get { return ResourceRead.GetResourceValue("QuoteUnclosedSuccessfully", resourceFileName); } }
        public static string MsgQuoteSequanceNotDefineValidation { get { return ResourceRead.GetResourceValue("MsgQuoteSequanceNotDefineValidation", resourceFileName); } }
        public static string MsgFixedQuoteNumberValidation { get { return ResourceRead.GetResourceValue("MsgFixedQuoteNumberValidation", resourceFileName); } }
        public static string MsgBlanketNotDefineValidation { get { return ResourceRead.GetResourceValue("MsgBlanketNotDefineValidation", resourceFileName); } }


    }
    public class RPT_QuoteMasterDTO
    {

        public long ID { get; set; }
        public string QuoteNumber { get; set; }
        public string ReleaseNumber { get; set; }
        public string SupplierName { get; set; }
        public string Comment { get; set; }
        public string RequiredDate { get; set; }
        public string Quotetatus { get; set; }
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
        public double? QuoteCost { get; set; }
        public int NoOfLineItems { get; set; }
        public DateTime QuoteDate { get; set; }
        public long? ChangeQuoteRevisionNo { get; set; }
        public string ShippingVendor { get; set; }
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
        public long Supplier { get; set; }
        public string BarcodeImage_QuoteNumber { get; set; }
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
        public string QuoteId { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }

    }
}
