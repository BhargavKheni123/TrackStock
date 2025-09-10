using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class InventoryCountDetailDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //InventoryCountGUID
        [Display(Name = "InventoryCountGUID", ResourceType = typeof(ResInventoryCountDetail))]
        public Guid InventoryCountGUID { get; set; }

        [Display(Name = "ItemNumber", ResourceType = typeof(ResInventoryCountDetail))]
        public string ItemNumber { get; set; }
        public int? InventoryClassification { get; set; }
        //ItemId
        [Display(Name = "ItemGUID", ResourceType = typeof(ResInventoryCountDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid ItemGUID { get; set; }

        //ItemLocationId
        [Display(Name = "ItemLocationId", ResourceType = typeof(ResInventoryCountDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid BinGUID { get; set; }

        //CountQuantity
        [Display(Name = "CountQuantity", ResourceType = typeof(ResInventoryCountDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double CountQuantity { get; set; }

        //CountLineItemDescription
        [Display(Name = "CountLineItemDescription", ResourceType = typeof(ResInventoryCountDetail))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String CountLineItemDescription { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResItemMaster))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ItemDescription { get; set; }

        //CountItemStatus
        [Display(Name = "CountItemStatus", ResourceType = typeof(ResInventoryCountDetail))]
        [StringLength(2, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String CountItemStatus { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResInventoryCountDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResInventoryCountDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResInventoryCountDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResInventoryCountDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResInventoryCountDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Created { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 LastUpdatedBy { get; set; }

        //IsDeleted
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsDeleted { get; set; }

        //IsArchived
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsArchived { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public long CompanyId { get; set; }
        public long RoomId { get; set; }

        public string CountName { get; set; }

        public string CountType { get; set; }
        public Guid CountDetailGUID { get; set; }

        public DateTime CountDate { get; set; }

        public string CountStatus { get; set; }

        public bool IsApplied { get; set; }
        public DateTime? AppliedDate { get; set; }

        public bool IsClosed { get; set; }

        public long BinID { get; set; }
        public string BinNumber { get; set; }

        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }

        public bool IsOnlyFromItemUI { get; set; }

        public double? CountCustomerOwnedQuantity { get; set; }
        public double? CountConsignedQuantity { get; set; }
        public double? ItemQuantity { get; set; }
        public double? CountCustomerOwnedQuantityEntry { get; set; }
        public double? CountConsignedQuantityEntry { get; set; }
        public string CountLineItemDescriptionEntry { get; set; }
        public bool IsStagingLocationCount { get; set; }
        public bool SaveAndApply { get; set; }
        public double? CusOwnedDifference { get; set; }
        public double? ConsignedDifference { get; set; }
        public double TotalDifference { get; set; }
        public string InventoryClassificationCode { get; set; }
        public int ItemType { get; set; }
        public string ItemLotSerialType { get; set; }
        public long HistoryID { get; set; }
        public long ICHistoryId { get; set; }
        public bool Consignment { get; set; }
        public string LIType { get; set; }
        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }

        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }
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
        public System.String ItemUDF1 { get; set; }
        public System.String ItemUDF2 { get; set; }
        public System.String ItemUDF3 { get; set; }
        public System.String ItemUDF4 { get; set; }
        public System.String ItemUDF5 { get; set; }

        public System.String ItemUDF6 { get; set; }
        public System.String ItemUDF7 { get; set; }
        public System.String ItemUDF8 { get; set; }
        public System.String ItemUDF9 { get; set; }
        public System.String ItemUDF10 { get; set; }
        public string WhatWhereAction { get; set; }
        public Guid? ProjectSpendGUID { get; set; }

        [Display(Name = "SupplierPartNo", ResourceType = typeof(ResItemMaster))]
        public string SupplierPartNo { get; set; }

        public bool isCreditApplied { get; set; }

        [Display(Name = "SupplierAccountNumber", ResourceType = typeof(ResRequisitionDetails))]
        public Nullable<System.Guid> SupplierAccountGuid { get; set; }
        public string SupplierAccountNo { get; set; }

        public bool? isLineItemAlreadyApplied { get; set; }

        public string PullOrderNumber { get; set; }
    }

    public class ResInventoryCountDetail
    {
        private static string ResourceFileName = "ResInventoryCountDetail";

        public static string IsStagingLocationCount
        {
            get
            {
                return ResourceRead.GetResourceValue("IsStagingLocationCount", ResourceFileName);
            }
        }
        public static string CountCustomerOwnedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CountCustomerOwnedQuantity", ResourceFileName);
            }
        }

        public static string CountConsignedQuantityEntry
        {
            get
            {
                return ResourceRead.GetResourceValue("CountConsignedQuantityEntry", ResourceFileName);
            }
        }
        public static string CusOwnedDifference
        {
            get
            {
                return ResourceRead.GetResourceValue("CusOwnedDifference", ResourceFileName);
            }
        }

        public static string ConsignedDifference
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedDifference", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceRead.GetResourceValue("Action", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InventoryCountDetail {0} already exist! Try with Another!.
        /// </summary>
        public static string Duplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("Duplicate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to HistoryID.
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeDeleted", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceRead.GetResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InventoryCountDetail.
        /// </summary>
        public static string InventoryCountDetailHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryCountDetailHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InventoryCountDetail.
        /// </summary>
        public static string InventoryCountDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryCountDetail", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View History.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ViewHistory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InventoryCountGUID.
        /// </summary>
        public static string InventoryCountGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryCountGUID", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to ItemId.
        /// </summary>
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemId.
        /// </summary>
        public static string CountColumnHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("CountColumnHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemId.
        /// </summary>
        public static string ItemGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationId.
        /// </summary>
        public static string ItemLocationId
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationId", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountQuantity.
        /// </summary>
        public static string CountQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CountQuantity", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to CountLineItemDescription.
        /// </summary>
        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountLineItemDescription.
        /// </summary>
        public static string CountLineItemDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("CountLineItemDescription", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountItemStatus.
        /// </summary>
        public static string ItemTypeCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemTypeCategory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountItemStatus.
        /// </summary>
        public static string CountItemStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("CountItemStatus", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountItemStatus.
        /// </summary>
        public static string IsApplied
        {
            get
            {
                return ResourceRead.GetResourceValue("IsApplied", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to CountItemStatus.
        /// </summary>
        public static string IsClosed
        {
            get
            {
                return ResourceRead.GetResourceValue("IsClosed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created.
        /// </summary>
        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated.
        /// </summary>
        public static string Updated
        {
            get
            {
                return ResourceRead.GetResourceValue("Updated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdatedBy.
        /// </summary>
        public static string LastUpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDeleted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IsArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GUID.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
            }
        }


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        public static string AppliedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("AppliedDate", ResourceFileName);
            }
        }

        public static string PrevConsignedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PrevConsignedQuantity", ResourceFileName);
            }
        }

        public static string PrevCustOwnedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PrevCustOwnedQuantity", ResourceFileName);
            }
        }

        public static string Apply
        {
            get
            {
                return ResourceRead.GetResourceValue("Apply", ResourceFileName);
            }
        }

        public static string ApplyAll
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyAll", ResourceFileName);
            }
        }

        public static string Add
        {
            get
            {
                return ResourceRead.GetResourceValue("Add", ResourceFileName);
            }
        }

        public static string Delete
        {
            get
            {
                return ResourceRead.GetResourceValue("Delete", ResourceFileName);
            }
        }
        public static string Cancel
        {
            get
            {
                return ResourceRead.GetResourceValue("Cancel", ResourceFileName);
            }
        }

        public static string SupplierAccountNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccountNumber", ResourceFileName);
            }
        }
        public static string AllCountApplied
        {
            get
            {
                return ResourceRead.GetResourceValue("AllCountApplied", ResourceFileName);
            }
        }
        public static string ApplyCountError
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyCountError", ResourceFileName);
            }
        }
        public static string ApplyCountSuccessfullyOn
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyCountSuccessfullyOn", ResourceFileName);
            }
        }
        public static string SomeItemNotPulled
        {
            get
            {
                return ResourceRead.GetResourceValue("SomeItemNotPulled", ResourceFileName);
            }
        }
        public static string SomeItemNotCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("SomeItemNotCredit", ResourceFileName);
            }
        }
        public static string AllCountAppliedonItems
        {
            get
            {
                return ResourceRead.GetResourceValue("AllCountAppliedonItems", ResourceFileName);
            }
        }
        public static string ApplyCountSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ApplyCountSuccessfully", ResourceFileName);
            }
        }
        public static string ReqLotOrSerialEntry
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqLotOrSerialEntry", ResourceFileName);
            }
        }
        public static string ErrortoPerformOperation
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrortoPerformOperation", ResourceFileName);
            }
        }
        public static string MsgGuidExistForLineItem
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgGuidExistForLineItem", ResourceFileName);
            }
        }
        public static string MsgLineItemDetailsHasError
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLineItemDetailsHasError", ResourceFileName);
            }
        }
        public static string MsgLineItemHasError
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLineItemHasError", ResourceFileName);
            }
        }
        public static string MsgItemBinAlreadyAdded
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemBinAlreadyAdded", ResourceFileName);
            }
        }
        public static string MsgDefaultSupplierAccountValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDefaultSupplierAccountValidation", ResourceFileName);
            }
        }
        public static string ReqSerialNumberAndExpiration
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqSerialNumberAndExpiration", ResourceFileName);
            }
        }
        public static string ReqLotNumberAndExpiration
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqLotNumberAndExpiration", ResourceFileName);
            }
        }
        public static string ReqLineItemsToCreateCount
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqLineItemsToCreateCount", ResourceFileName);
            }
        }
        public static string MsgNoLineItemGetValidate
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoLineItemGetValidate", ResourceFileName);
            }
        }
        public static string QLItemAlreadyAddedToCount
        {
            get
            {
                return ResourceRead.GetResourceValue("QLItemAlreadyAddedToCount", ResourceFileName);
            }
        }
        public static string QuantityMissingInRow
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityMissingInRow", ResourceFileName);
            }
        }
        public static string QuantityValidationSerialItem
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityValidationSerialItem", ResourceFileName);
            }
        }
        public static string BinnumberMissingRow
        {
            get
            {
                return ResourceRead.GetResourceValue("BinnumberMissingRow", ResourceFileName);
            }
        }
        public static string LotSerialnumberMissingRow
        {
            get
            {
                return ResourceRead.GetResourceValue("LotSerialnumberMissingRow", ResourceFileName);
            }
        }
        public static string ExpirationDateMissingRow
        {
            get
            {
                return ResourceRead.GetResourceValue("ExpirationDateMissingRow", ResourceFileName);
            }
        }
        


    }

    public class ApplyCountOnLineItemRespDTO
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public bool IsCountClosed { get; set; }
        public InventoryCountDetailDTO CurrentObj { get; set; }
    }
}


