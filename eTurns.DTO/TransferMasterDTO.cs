using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace eTurns.DTO
{



    public enum RequestType
    {
        In = 0,
        Out = 1
    }

    public enum TransferStatus
    {
        UnSubmitted = 1,
        Submitted = 2,
        Approved = 3,
        Transmitted = 4,
        FullFillQuantity = 5,
        TransmittedIncomplete = 6,
        TransmittedPastDue = 7,
        TransmittedInCompletePastDue = 8,
        Closed = 9,
        Rejected = 10,

    }
    public class TransferMasterDTO
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "TransferNumber", ResourceType = typeof(ResTransfer))]
        public string TransferNumber { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ReplenishingRoomID", ResourceType = typeof(ResTransfer))]
        public Int64 ReplenishingRoomID { get; set; }
        public string TransferLineItemsIds { get; set; }
        [Display(Name = "RequestingRoom", ResourceType = typeof(ResTransfer))]
        public Int64 RequestingRoomID { get; set; }

        [Display(Name = "Comment", ResourceType = typeof(ResTransfer))]
        [AllowHtml]
        public String Comment { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "RequireDate", ResourceType = typeof(ResTransfer))]
        public DateTime RequireDate { get; set; }
        private string _RequiredDate;
        public bool IsTransferSelected { get; set; }

        [Display(Name = "RequiredDateString", ResourceType = typeof(ResTransfer))]
        public string RequiredDateString
        {
            get
            {
                if (string.IsNullOrEmpty(_RequiredDate))
                {
                    _RequiredDate = Convert.ToString(FnCommon.ConvertDateByTimeZone(RequireDate, true, true).Split(' ')[0]);
                }
                return _RequiredDate;
            }
            set { this._RequiredDate = value; }
        }
        public Nullable<Int64> StagingID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "TransferStatus", ResourceType = typeof(ResTransfer))]
        public int TransferStatus { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "RequestType", ResourceType = typeof(ResTransfer))]
        public int RequestType { get; set; }

        public Nullable<Guid> RefTransferGUID { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public DateTime Updated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Int64 CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Int64 LastUpdatedBy { get; set; }

        [Display(Name = "IsDeleted")]
        public Nullable<bool> IsDeleted { get; set; }

        [Display(Name = "IsArchived")]
        public Nullable<bool> IsArchived { get; set; }


        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Int64 RoomID { get; set; }


        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 CompanyID { get; set; }
        public Guid GUID { get; set; }


        [Display(Name = "UDF1", ResourceType = typeof(ResTransfer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResTransfer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResTransfer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResTransfer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResTransfer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public string UDF5 { get; set; }


        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public bool IsHistory { get; set; }

        [Display(Name = "Rejection Reason")]
        public string RejectionReason { get; set; }

        [Display(Name = "StagingName", ResourceType = typeof(ResMaterialStaging))]
        public string StagingName { get; set; }

        [Display(Name = "ReplenishingRoom", ResourceType = typeof(ResTransfer))]
        public string ReplenishingRoomName { get; set; }

        [Display(Name = "RequestingRoom", ResourceType = typeof(ResTransfer))]
        public string RequestingRoomName { get; set; }

        public string RefTransferNumber { get; set; }

        public string RequestTypeName { get; set; }
        public string TransferStatusName { get; set; }

        public bool IsRecordNotEditable { get; set; }
        public bool IsOnlyStatusUpdate { get; set; }

        public bool TransferIsInReceive { get; set; }
        public bool IsAbleToDelete { get; set; }

        public List<TransferDetailDTO> TransferDetailList { get; set; }
        public string AppendedBarcodeString { get; set; }
        public int TotalRecords { get; set; }
        //public int NoOfItems { get; set; }
        public System.String WhatWhereAction { get; set; }

        public Nullable<Guid> MaterialStagingGUID { get; set; }

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


        public bool IsOnlyFromUI { get; set; }
        //public Nullable<Int64> NoOfLineItems { get; set; }
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
        public string ReceivedOnWebDate
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

        [Display(Name = "NoOfItems", ResourceType = typeof(ResTransfer))]
        public int? NoOfItems { get; set; }

        [Display(Name = "TotalCost", ResourceType = typeof(ResTransfer))]
        public double? TotalCost { get; set; }

        public System.String TransferQuantityString { get; set; }
    }

    public class RPT_Transfer
    {
        public string TransferNumber { get; set; }
        public Guid GUID { get; set; }
        public string TransferStatus { get; set; }
        public int TransferStatusNumber { get; set; }
        public string RequestType { get; set; }
    }

    public class ReturnIntransitItemsResult
    {
        public bool ReturnValue { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class RPT_TransferWithLineItemDTO
    {
        public Int64 ID { get; set; }
        public string TransferNumber { get; set; }
        public string RequestType { get; set; }
        public int TransferStatusNumber { get; set; }
        public string ItemNumber { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public Guid TransferGuid { get; set; }
        public Guid GUID { get; set; }
        public Guid ItemGuid { get; set; }

    }


    public class ResTransfer
    {
        private static string ResourceFileName = "ResTransfer";


        /// <summary>
        ///   Looks up a localized string similar to Transfer.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Add New Item.
        /// </summary>
        public static string AddNewItem
        {
            get
            {
                return ResourceRead.GetResourceValue("AddNewItem", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Approved.
        /// </summary>
        public static string Approved
        {
            get
            {
                return ResourceRead.GetResourceValue("Approved", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change Log.
        /// </summary>
        public static string ChangeLogTab
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeLogTab", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change Transfer.
        /// </summary>
        public static string ChangeTransferTab
        {
            get
            {
                return ResourceRead.GetResourceValue("ChangeTransferTab", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Closed.
        /// </summary>
        public static string Closed
        {
            get
            {
                return ResourceRead.GetResourceValue("Closed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Comment.
        /// </summary>
        public static string Comment
        {
            get
            {
                return ResourceRead.GetResourceValue("Comment", ResourceFileName);
            }
        }
        public static string NoOfItems
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfItems", ResourceFileName);
            }
        }

        public static string NoOfLineItems
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfLineItems", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Fulfill Quantity.
        /// </summary>
        public static string FulFillQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("FulFillQuantity", ResourceFileName);
            }
        }
        public static string RequestedQuantityOHQMSG
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedQuantityOHQMSG", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Fullfill Qty.
        /// </summary>
        public static string FullfillQuantityTab
        {
            get
            {
                return ResourceRead.GetResourceValue("FullfillQuantityTab", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to In.
        /// </summary>
        public static string In
        {
            get
            {
                return ResourceRead.GetResourceValue("In", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InOut.
        /// </summary>
        public static string InOut
        {
            get
            {
                return ResourceRead.GetResourceValue("InOut", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InTransit Quantity.
        /// </summary>
        public static string InTransitQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("InTransitQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemGUID.
        /// </summary>
        public static string ItemGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemID.
        /// </summary>
        public static string ItemID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdated.
        /// </summary>
        public static string LastUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Location.
        /// </summary>
        public static string Location
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Out.
        /// </summary>
        public static string Out
        {
            get
            {
                return ResourceRead.GetResourceValue("Out", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to eTurns: Transfer.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Receivable.
        /// </summary>
        public static string ReceivabledTab
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivabledTab", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Received Quantity.
        /// </summary>
        public static string ReceivedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Receive.
        /// </summary>
        public static string ReceiveTab
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveTab", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RefTransferID.
        /// </summary>
        public static string RefTransferID
        {
            get
            {
                return ResourceRead.GetResourceValue("RefTransferID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RequestType.
        /// </summary>
        public static string RequestType
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Rejected.
        /// </summary>
        public static string Rejected
        {
            get
            {
                return ResourceRead.GetResourceValue("Rejected", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Rejected Reason.
        /// </summary>
        public static string RejectedReason
        {
            get
            {
                return ResourceRead.GetResourceValue("RejectedReason", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Replinish Room.
        /// </summary>
        public static string ReplinishRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplinishRoom", ResourceFileName);
            }
        }
                
        public static string ReplenishingRoomID
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplinishRoom", ResourceFileName);
            }
        }

        public static string ReplinishRoomRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplinishRoomRequired", ResourceFileName);
            }
        }
        public static string TransferNumberRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferNumberRequired", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Requested Quantity.
        /// </summary>
        public static string RequestedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Requested Room.
        /// </summary>
        public static string RequestedRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedRoom", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Required Date.
        /// </summary>
        public static string RequireDate
        {
            get
            {
                return ResourceRead.GetResourceValue("RequireDate", ResourceFileName);
            }
        }

        
        public static string RequiredDateString
        {
            get
            {
                return ResourceRead.GetResourceValue("RequireDate", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Shipped Quantity.
        /// </summary>
        public static string ShippedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ShippedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to StagingID.
        /// </summary>
        public static string StagingID
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Staging Name.
        /// </summary>
        public static string StagingName
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Submitted.
        /// </summary>
        public static string Submitted
        {
            get
            {
                return ResourceRead.GetResourceValue("Submitted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TotatQuantity.
        /// </summary>
        public static string TotalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transferable.
        /// </summary>
        public static string TransferableTab
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferableTab", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer.
        /// </summary>
        public static string TransferBtn
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferBtn", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfered.
        /// </summary>
        public static string Transfered
        {
            get
            {
                return ResourceRead.GetResourceValue("Transfered", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Received.
        /// </summary>
        public static string Received
        {
            get
            {
                return ResourceRead.GetResourceValue("Received", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TransferGUID.
        /// </summary>
        public static string TransferGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TransferID.
        /// </summary>
        public static string TransferID
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Number.
        /// </summary>
        public static string TransferNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Status.
        /// </summary>
        public static string TransferStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferStatus", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transmitted.
        /// </summary>
        public static string Transmitted
        {
            get
            {
                return ResourceRead.GetResourceValue("Transmitted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transmitted.
        /// </summary>
        public static string Transmitted1
        {
            get
            {
                return ResourceRead.GetResourceValue("Transmitted1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Incomplete.
        /// </summary>
        public static string TransmittedIncomplete
        {
            get
            {
                return ResourceRead.GetResourceValue("TransmittedIncomplete", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InComplete Post Due.
        /// </summary>
        public static string TransmittedInCompletePastDue
        {
            get
            {
                return ResourceRead.GetResourceValue("TransmittedInCompletePastDue", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Post Due.
        /// </summary>
        public static string TransmittedPastDue
        {
            get
            {
                return ResourceRead.GetResourceValue("TransmittedPastDue", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName,true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UnSubmitted.
        /// </summary>
        public static string UnSubmitted
        {
            get
            {
                return ResourceRead.GetResourceValue("UnSubmitted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UnSubmitted.
        /// </summary>
        public static string FullFillQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("FullFillQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiveButton.
        /// </summary>
        public static string GetTransferStatusText(string value)
        {
            return ResourceRead.GetResourceValue(value, ResourceFileName);
        }

        public static string RequestingRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestingRoom", ResourceFileName);
            }
        }

        public static string TotalCost
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalCost", ResourceFileName);
            }
        }

        public static string ApprovedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ApprovedQuantity", ResourceFileName);
            }
        }

        public static string TransferQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferQuantity", ResourceFileName);
            }
        }

        public static string QuantityToReceive
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityToReceive", ResourceFileName);
            }
        }
        public static string ReceiveQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveQuantity", ResourceFileName);
            }
        }
        public static string LotNumberTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumberTracking", ResourceFileName);
            }
        }
        public static string SerialNumberTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumberTracking", ResourceFileName);
            }
        }
        public static string DateCodeTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("DateCodeTracking", ResourceFileName);
            }
        }
        public static string Notracking
        {
            get
            {
                return ResourceRead.GetResourceValue("Notracking", ResourceFileName);
            }
        }

        public static string MsgDuplicateSerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicateSerialNumber", ResourceFileName);
            }
        }

        public static string QuantityToTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityToTransfer", ResourceFileName);
            }
        }

        public static string MsgMatchingItemNotFoundToAdd
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMatchingItemNotFoundToAdd", ResourceFileName);
            }
        }

        public static string MsgLotPlusExpirationDateNotMatched
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLotPlusExpirationDateNotMatched", ResourceFileName);
            }
        }

        public static string SerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumber", ResourceFileName);
            }
        }

        public static string LotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumber", ResourceFileName);
            }
        }

        public static string Expiration
        {
            get
            {
                return ResourceRead.GetResourceValue("Expiration", ResourceFileName);
            }
        }

        public static string DestinationLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("DestinationLocation", ResourceFileName);
            }
        }

        public static string MsgSubmittedUnsubmittedRestriction
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSubmittedUnsubmittedRestriction", ResourceFileName);
            }
        }
        public static string ItemsNotInsertedAsNotExistInReplenishRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemsNotInsertedAsNotExistInReplenishRoom", ResourceFileName);
            }
        }
        
        public static string SelectBin
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectBin", ResourceFileName);
            }
        }
        public static string ReqRowToTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRowToTransfer", ResourceFileName);
            }
        }
        public static string ReqQuantityToSave
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqQuantityToSave", ResourceFileName);
            }
        }
        public static string RejectionReason { get  { return ResourceRead.GetResourceValue("RejectionReason", ResourceFileName); } }
        public static string TransferMustHaveMinimumOneLineItem { get  { return ResourceRead.GetResourceValue("TransferMustHaveMinimumOneLineItem", ResourceFileName); } }

        public static string YourTransferHasBeenApproved { get { return ResourceRead.GetResourceValue("YourTransferHasBeenApproved", ResourceFileName); } }
        public static string YourTransferHasAlreadyBeenApproved { get { return ResourceRead.GetResourceValue("YourTransferHasAlreadyBeenApproved", ResourceFileName); } }
        public static string YourTransferIsNotValidToApproval { get { return ResourceRead.GetResourceValue("YourTransferIsNotValidToApproval", ResourceFileName); } }
        public static string TransferHasAlreadyBeenRejected { get { return ResourceRead.GetResourceValue("TransferHasAlreadyBeenRejected", ResourceFileName); } }
        public static string TransferHasRejected { get { return ResourceRead.GetResourceValue("TransferHasRejected", ResourceFileName); } }
        public static string TransferIsNotValidForRejection { get { return ResourceRead.GetResourceValue("TransferIsNotValidForRejection", ResourceFileName); } }
        public static string MsgTransferSequienceNotDefineValidation { get { return ResourceRead.GetResourceValue("MsgTransferSequienceNotDefineValidation", ResourceFileName); } }
        
        public static string NotEnoughQuantity { get { return ResourceRead.GetResourceValue("NotEnoughQuantity", ResourceFileName); } }
        public static string QuantityTransferred { get { return ResourceRead.GetResourceValue("QuantityTransferred", ResourceFileName); } }
        public static string ApprovedQuantityAlreadyTransferred { get { return ResourceRead.GetResourceValue("ApprovedQuantityAlreadyTransferred", ResourceFileName); } }
        public static string ApprovedQuantityIsZero { get { return ResourceRead.GetResourceValue("ApprovedQuantityIsZero", ResourceFileName); } }
        public static string QuantityNotAvailableToTransfer { get { return ResourceRead.GetResourceValue("QuantityNotAvailableToTransfer", ResourceFileName); } }
        public static string NotEnoughQuantityToReceive { get { return ResourceRead.GetResourceValue("NotEnoughQuantityToReceive", ResourceFileName); } }
        public static string ReceivedQtyIsNotMoreThanTransferredQty { get { return ResourceRead.GetResourceValue("ReceivedQtyIsNotMoreThanTransferredQty", ResourceFileName); } }
        public static string QuantityReceived { get { return ResourceRead.GetResourceValue("QuantityReceived", ResourceFileName); } }

        public static string CompanyNameIsnotProper { get { return ResourceRead.GetResourceValue("CompanyNameIsnotProper", ResourceFileName); } }
        public static string ReceivingRoomNameIsNotProper { get { return ResourceRead.GetResourceValue("ReceivingRoomNameIsNotProper", ResourceFileName); } }
        public static string ReplenishingRoomNameIsNotProper { get { return ResourceRead.GetResourceValue("ReplenishingRoomNameIsNotProper", ResourceFileName); } }
        public static string ReceivingAndReplenishingRoomNameMustNotSame { get { return ResourceRead.GetResourceValue("ReceivingAndReplenishingRoomNameMustNotSame", ResourceFileName); } }
        public static string RequestTypeIsNotProper { get { return ResourceRead.GetResourceValue("RequestTypeIsNotProper", ResourceFileName); } }
        public static string TransferStatusIsNotProper { get { return ResourceRead.GetResourceValue("TransferStatusIsNotProper", ResourceFileName); } }
        public static string UserNameIsNotProper { get { return ResourceRead.GetResourceValue("UserNameIsNotProper", ResourceFileName); } }
        public static string UserDoesntHaveRightsToInsertTransfer { get { return ResourceRead.GetResourceValue("UserDoesntHaveRightsToInsertTransfer", ResourceFileName); } }
        public static string UserHasToAcceptLicenceBeforeCreateTransfer { get { return ResourceRead.GetResourceValue("UserHasToAcceptLicenceBeforeCreateTransfer", ResourceFileName); } }
        public static string TransferNoIsNotProper { get { return ResourceRead.GetResourceValue("TransferNoIsNotProper", ResourceFileName); } }
        public static string ToSubmitTransferMustHaveMinimumOneLineItem { get { return ResourceRead.GetResourceValue("ToSubmitTransferMustHaveMinimumOneLineItem", ResourceFileName); } }
        public static string UserHasNoRightToTransferStatus { get { return ResourceRead.GetResourceValue("UserHasNoRightToTransferStatus", ResourceFileName); } }
        public static string UserHasNoRightToInsertOrUpdateTransfer { get { return ResourceRead.GetResourceValue("UserHasNoRightToInsertOrUpdateTransfer", ResourceFileName); } }
        
        public static string RequestedQtyShouldBeGreaterThanZero { get { return ResourceRead.GetResourceValue("RequestedQtyShouldBeGreaterThanZero", ResourceFileName); } }
        public static string LineItemsNotProper { get { return ResourceRead.GetResourceValue("LineItemsNotProper", ResourceFileName); } }
        public static string SomeLineItemsNotProper { get { return ResourceRead.GetResourceValue("SomeLineItemsNotProper", ResourceFileName); } }
        public static string ItemDoesnotExistInOtherRoom { get { return ResourceRead.GetResourceValue("ItemDoesnotExistInOtherRoom", ResourceFileName); } }
        public static string ReplenishRoomItemIsInactive { get { return ResourceRead.GetResourceValue("ReplenishRoomItemIsInactive", ResourceFileName); } }
        public static string ReceivingRoomItemIsInactive { get { return ResourceRead.GetResourceValue("ReceivingRoomItemIsInactive", ResourceFileName); } }
        public static string ReplenishRoomItemIsDeleted { get { return ResourceRead.GetResourceValue("ReplenishRoomItemIsDeleted", ResourceFileName); } }
        public static string ReceivingRoomItemIsDeleted { get { return ResourceRead.GetResourceValue("ReceivingRoomItemIsDeleted", ResourceFileName); } }
        public static string ItemTypeNotMatched { get { return ResourceRead.GetResourceValue("ItemTypeNotMatched", ResourceFileName); } }
        public static string TransferNotExistInCurrentRoom { get { return ResourceRead.GetResourceValue("TransferNotExistInCurrentRoom", ResourceFileName); } }
        public static string InvalidTransferStatus { get { return ResourceRead.GetResourceValue("InvalidTransferStatus", ResourceFileName); } }
        public static string NewItemsNotAllowedInClosedTransfer { get { return ResourceRead.GetResourceValue("NewItemsNotAllowedInClosedTransfer", ResourceFileName); } }
        public static string OtherRoomItemIsLaborItem { get { return ResourceRead.GetResourceValue("OtherRoomItemIsLaborItem", ResourceFileName); } }
        public static string OtherRoomItemIsInactive { get { return ResourceRead.GetResourceValue("OtherRoomItemIsInactive", ResourceFileName); } }
        public static string OtherRoomItemIsDeleted { get { return ResourceRead.GetResourceValue("OtherRoomItemIsDeleted", ResourceFileName); } }
        public static string ItemTypeNotMatchedWithOtherRoom { get { return ResourceRead.GetResourceValue("ItemTypeNotMatchedWithOtherRoom", ResourceFileName); } }
        public static string ItemDoesNotExistInCurrentRoom { get { return ResourceRead.GetResourceValue("ItemDoesNotExistInCurrentRoom", ResourceFileName); } }
        public static string ItemDoesNotExistInCurrentTransfer { get { return ResourceRead.GetResourceValue("ItemDoesNotExistInCurrentTransfer", ResourceFileName); } }
        public static string TransferItemIsNotExistInCurrentRoom { get { return ResourceRead.GetResourceValue("TransferItemIsNotExistInCurrentRoom", ResourceFileName); } }
        public static string CanNotReceiveCloseTransfer { get { return ResourceRead.GetResourceValue("CanNotReceiveCloseTransfer", ResourceFileName); } }
        public static string UserDoesntHaveRightsToReceiveTransfer { get { return ResourceRead.GetResourceValue("UserDoesntHaveRightsToReceiveTransfer", ResourceFileName); } }
        public static string CloseSelectedTransfers { get { return ResourceRead.GetResourceValue("CloseSelectedTransfers", ResourceFileName); } }
        public static string CloseTransferConfirmationMsg { get { return ResourceRead.GetResourceValue("CloseTransferConfirmationMsg", ResourceFileName); } }
        public static string SelectAtleastOneUnsubmittedTransfer { get { return ResourceRead.GetResourceValue("SelectAtleastOneUnsubmittedTransfer", ResourceFileName); } }
        public static string DuplicateTransferItemGuidNotAllowed { get { return ResourceRead.GetResourceValue("DuplicateTransferItemGuidNotAllowed", ResourceFileName); } }
        public static string FulFillQuantityNotExist { get { return ResourceRead.GetResourceValue("FulFillQuantityNotExist", ResourceFileName); } }


    }
}
