using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class InventoryCountDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //CountName
        [Display(Name = "CountName", ResourceType = typeof(ResInventoryCount))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String CountName { get; set; }

        //CountItemDescription
        [Display(Name = "CountItemDescription", ResourceType = typeof(ResInventoryCount))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String CountItemDescription { get; set; }

        //CountType
        [Display(Name = "CountType", ResourceType = typeof(ResInventoryCount))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String CountType { get; set; }

        //CountStatus
        [Display(Name = "CountStatus", ResourceType = typeof(ResInventoryCount))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String CountStatus { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResInventoryCount))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResInventoryCount))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResInventoryCount))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResInventoryCount))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResInventoryCount))]
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

        //Year
        [Display(Name = "Year", ResourceType = typeof(ResInventoryCount))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int16 Year { get; set; }

        //CompanyId
        [Display(Name = "CompanyId", ResourceType = typeof(ResInventoryCount))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CompanyId { get; set; }

        //RoomId
        [Display(Name = "RoomId", ResourceType = typeof(ResInventoryCount))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 RoomId { get; set; }

        //CountDate
        [Display(Name = "CountDate", ResourceType = typeof(ResInventoryCount))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime CountDate { get; set; }

        [Display(Name = "CountDateDisplay", ResourceType = typeof(ResInventoryCount))]
        public string CountDateDisplay { get; set; }

        //CountCompletionDate
        [Display(Name = "CountCompletionDate", ResourceType = typeof(ResInventoryCount))]
        public Nullable<System.DateTime> CountCompletionDate { get; set; }

        //IsAutomatedCompletion
        [Display(Name = "IsAutomatedCompletion", ResourceType = typeof(ResInventoryCount))]
        public Nullable<Boolean> IsAutomatedCompletion { get; set; }

        //CompleteCauseCountGUID
        [Display(Name = "CompleteCauseCountGUID", ResourceType = typeof(ResInventoryCount))]
        public Nullable<Guid> CompleteCauseCountGUID { get; set; }

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

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public int TotalItemsWithinCount { get; set; }
        public bool IsClosed { get; set; }
        public bool IsApplied { get; set; }
        public long HistoryID { get; set; }
        public Int32 inventorycount { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public List<InventoryCountDetailDTO> CountLineItemsList { get; set; }


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
        //private string _CountDate;
        //public string strCountDate
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_CountDate))
        //        {
        //            _CountDate = FnCommon.ConvertDateByTimeZone(CountDate, true, true);
        //        }
        //        return _CountDate;
        //    }
        //    set { this._CountDate = value; }
        //}

        public bool IsApplyAllDisable { get; set; }
        public Guid? ProjectSpendGUID { get; set; }
        [Display(Name = "ProjectSpendName", ResourceType = typeof(ResProjectMaster))]
        public string ProjectSpendName { get; set; }

        public int? TotalRecords { get; set; }

        private string _InventoryCountDate;
        public string InventoryCountDate
        {
            get
            {
                if (string.IsNullOrEmpty(_InventoryCountDate))
                {
                    _InventoryCountDate = FnCommon.ConvertDateByTimeZone(CountDate, false, true);
                }
                return _InventoryCountDate;
            }
            set { this._InventoryCountDate = value; }
        }
        [Display(Name = "ReleaseNumber", ResourceType = typeof(ResInventoryCount))]
        public string ReleaseNumber { get; set; }

        [Display(Name = "PullOrderNumber", ResourceType = typeof(ResPullMaster))]
        public string PullOrderNumber { get; set; }
    }

    public class ResInventoryCount
    {
        private static string ResourceFileName = "ResInventoryCount";
        /// <summary>
        ///   Looks up a localized string similar to InventoryCount {0} already exist! Try with Another!.
        /// </summary>
        /// 

        public static string Manual
        {
            get
            {
                return ResourceRead.GetResourceValue("Manual", ResourceFileName);
            }
        }
        public static string Adjustment
        {
            get
            {
                return ResourceRead.GetResourceValue("Adjustment", ResourceFileName);
            }
        }
        public static string TotalItemsWithinCount
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalItemsWithinCount", ResourceFileName);
            }
        }
        public static string Cycle
        {
            get
            {
                return ResourceRead.GetResourceValue("Cycle", ResourceFileName);
            }
        }
        public static string IsClosed
        {
            get
            {
                return ResourceRead.GetResourceValue("IsClosed", ResourceFileName);
            }
        }
        public static string IsApplied
        {
            get
            {
                return ResourceRead.GetResourceValue("IsApplied", ResourceFileName);
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
        ///   Looks up a localized string similar to InventoryCount {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to InventoryCount.
        /// </summary>
        public static string InventoryCountHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryCountHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InventoryCount.
        /// </summary>
        public static string InventoryCount
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryCount", ResourceFileName);
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
        ///   Looks up a localized string similar to GUID.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountName.
        /// </summary>
        public static string CountName
        {
            get
            {
                return ResourceRead.GetResourceValue("CountName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountItemDescription.
        /// </summary>
        public static string CountItemDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("CountItemDescription", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountType.
        /// </summary>
        public static string CountType
        {
            get
            {
                return ResourceRead.GetResourceValue("CountType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountStatus.
        /// </summary>
        public static string CountStatus
        {
            get
            {
                return ResourceRead.GetResourceValue("CountStatus", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName, true);
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
        ///   Looks up a localized string similar to Year.
        /// </summary>
        public static string Year
        {
            get
            {
                return ResourceRead.GetResourceValue("Year", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyId.
        /// </summary>
        public static string CompanyId
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyId", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RoomId.
        /// </summary>
        public static string RoomId
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomId", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountDate.
        /// </summary>
        public static string CountDate
        {
            get
            {
                return ResourceRead.GetResourceValue("CountDate", ResourceFileName);
            }
        }


        public static string CountDateDisplay
        {
            get
            {
                return ResourceRead.GetResourceValue("CountDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CountCompletionDate.
        /// </summary>
        public static string CountCompletionDate
        {
            get
            {
                return ResourceRead.GetResourceValue("CountCompletionDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsAutomatedCompletion.
        /// </summary>
        public static string IsAutomatedCompletion
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAutomatedCompletion", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompleteCauseCountGUID.
        /// </summary>
        public static string CompleteCauseCountGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompleteCauseCountGUID", ResourceFileName);
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

        public static string ModelHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ModelHeader", ResourceFileName);
            }
        }

        public static string ConfirmBeforeAddQuicklist
        {
            get
            {
                return ResourceRead.GetResourceValue("ConfirmBeforeAddQuicklist", ResourceFileName);
            }
        }
        public static string CannotChangeCountType
        {
            get
            {
                return ResourceRead.GetResourceValue("CannotChangeCountType", ResourceFileName);
            }
        }

        public static string ReleaseNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ReleaseNumber", ResourceFileName);
            }
        }

        public static string MsgCloseNotAppliedCount
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCloseNotAppliedCount", ResourceFileName);
            }
        }
        public static string YearShouldHave365Days
        {
            get
            {
                return ResourceRead.GetResourceValue("YearShouldHave365Days", ResourceFileName);
            }
        }
        public static string SameSerialMultipleEntryNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("SameSerialMultipleEntryNotAllowed", ResourceFileName);
            }
        }
        public static string ItemContainsQtyOnSerial
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemContainsQtyOnSerial", ResourceFileName);
            }
        }
        public static string ItemContainsQtyOnSerialInCount
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemContainsQtyOnSerialInCount", ResourceFileName);
            }
        }
        public static string SameLotMultipleEntryNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("SameLotMultipleEntryNotAllowed", ResourceFileName);
            }
        }
        public static string SameLotDifferentExpirationNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("SameLotDifferentExpirationNotAllowed", ResourceFileName);
            }
        }
        public static string CloseSelectedCounts { get { return ResourceRead.GetResourceValue("CloseSelectedCounts", ResourceFileName); } }
        public static string EnterItemNumber { get { return ResourceRead.GetResourceValue("EnterItemNumber", ResourceFileName); } }
        public static string ItemDoesNotExistsInRoom { get { return ResourceRead.GetResourceValue("ItemDoesNotExistsInRoom", ResourceFileName); } }
        public static string LotNumberCantBeBlank { get { return ResourceRead.GetResourceValue("LotNumberCantBeBlank", ResourceFileName); } }
        public static string EnterReceiveDateFormat { get { return ResourceRead.GetResourceValue("EnterReceiveDateFormat", ResourceFileName); } }
        public static string ExpirationDateCantBeEmpty { get { return ResourceRead.GetResourceValue("ExpirationDateCantBeEmpty", ResourceFileName); } }
        public static string EnterExpiryDateFormat { get { return ResourceRead.GetResourceValue("EnterExpiryDateFormat", ResourceFileName); } }
        public static string CountCustOrConsQtyShouldBeGreaterEqualsZero { get { return ResourceRead.GetResourceValue("CountCustOrConsQtyShouldBeGreaterEqualsZero", ResourceFileName); } }
        public static string PullQtyIsntEnoughForCredit { get { return ResourceRead.GetResourceValue("PullQtyIsntEnoughForCredit", ResourceFileName); } }
        public static string ProvideValidCustOrConsQty { get { return ResourceRead.GetResourceValue("ProvideValidCustOrConsQty", ResourceFileName); } }
        public static string CantApplyCountNotEnoughPullForCredit { get { return ResourceRead.GetResourceValue("CantApplyCountNotEnoughPullForCredit", ResourceFileName); } }
        public static string CantApplyCountTotalQtyNotMatchedWithDetailQty { get { return ResourceRead.GetResourceValue("CantApplyCountTotalQtyNotMatchedWithDetailQty", ResourceFileName); } }
        public static string CantApplyCountForSomeItems { get { return ResourceRead.GetResourceValue("CantApplyCountForSomeItems", ResourceFileName); } }
        public static string SaveError { get { return ResourceRead.GetResourceValue("SaveError", ResourceFileName); } }
        public static string SaveSuccess { get { return ResourceRead.GetResourceValue("SaveSuccess", ResourceFileName); } }
        public static string UserNoRightsForInsertCount { get { return ResourceRead.GetResourceValue("UserNoRightsForInsertCount", ResourceFileName); } }
        public static string UserNoRightsForUpdateCount { get { return ResourceRead.GetResourceValue("UserNoRightsForUpdateCount", ResourceFileName); } }
        public static string UserNoRightsForApplyCountLineItem { get { return ResourceRead.GetResourceValue("UserNoRightsForApplyCountLineItem", ResourceFileName); } }
        public static string UserNoRightsForInsertCountforRoom { get { return ResourceRead.GetResourceValue("UserNoRightsForInsertCountforRoom", ResourceFileName); } }
        public static string ReqCountNameorRoomAutoSequenceType { get { return ResourceRead.GetResourceValue("ReqCountNameorRoomAutoSequenceType", ResourceFileName); } }
        public static string ValidateCountTypeChange { get { return ResourceRead.GetResourceValue("ValidateCountTypeChange", ResourceFileName); } }

        public static string ProvideLineItemDetailForTrackingTypeItem { get { return ResourceRead.GetResourceValue("ProvideLineItemDetailForTrackingTypeItem", ResourceFileName); } }
        public static string SerialNoAlreadyExist { get { return ResourceRead.GetResourceValue("SerialNoAlreadyExist", ResourceFileName); } }
        public static string UserNoRightsForApplyCountforRoom { get { return ResourceRead.GetResourceValue("UserNoRightsForApplyCountforRoom", ResourceFileName); } }
        public static string MsgForLabotItemCounted { get { return ResourceRead.GetResourceValue("MsgForLabotItemCounted", ResourceFileName); } }
        public static string MsgBinNumberCanNotEmpty { get { return ResourceRead.GetResourceValue("MsgBinNumberCanNotEmpty", ResourceFileName); } }
        public static string UserNoRightsForupdateCountforRoom { get { return ResourceRead.GetResourceValue("UserNoRightsForupdateCountforRoom", ResourceFileName); } }
        public static string ReqCountNameIDOrGUID { get { return ResourceRead.GetResourceValue("ReqCountNameIDOrGUID", ResourceFileName); } }
        public static string ReqIDOrGUIDForEditCount { get { return ResourceRead.GetResourceValue("ReqIDOrGUIDForEditCount", ResourceFileName); } }
        public static string NoCountFound { get { return ResourceRead.GetResourceValue("NoCountFound", ResourceFileName); } }
        public static string MsgCanNotEditAppliedOrClosedCount { get { return ResourceRead.GetResourceValue("MsgCanNotEditAppliedOrClosedCount", ResourceFileName); } }
        public static string MsgCanNotEditClosedCount { get { return ResourceRead.GetResourceValue("MsgCanNotEditClosedCount", ResourceFileName); } }
        public static string MsgCanNotEditAppliedCount { get { return ResourceRead.GetResourceValue("MsgCanNotEditAppliedCount", ResourceFileName); } }
        public static string ResPrintInventoryCount { get { return ResourceRead.GetResourceValue("ResPrintInventoryCount", ResourceFileName); } }
        public static string UncloseInventoryCount { get { return ResourceRead.GetResourceValue("UncloseInventoryCount", ResourceFileName); } }

    }

    public enum InventoryCountStatus
    {
        Open = 'O',
        Close = 'C'

    }
    public enum InventoryCountType
    {
        Manual = 'M',
        Cycle = 'C',
        Adjustment = 'A'
    }
}


