using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class AssetMasterDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //AssetName
        [Display(Name = "AssetName", ResourceType = typeof(ResAssetMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AssetName { get; set; }

        //Description
        [Display(Name = "Description", ResourceType = typeof(ResAssetMaster))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Description { get; set; }

        //Make
        [Display(Name = "Make", ResourceType = typeof(ResAssetMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Make { get; set; }

        //Model
        [Display(Name = "Model", ResourceType = typeof(ResAssetMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Model { get; set; }

        //Serial
        [Display(Name = "Serial", ResourceType = typeof(ResAssetMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Serial { get; set; }

        //ToolCategoryID
        [Display(Name = "ToolCategoryID", ResourceType = typeof(ResAssetMaster))]
        public Nullable<System.Int64> ToolCategoryID { get; set; }

        //PurchaseDate
        [Display(Name = "PurchaseDate", ResourceType = typeof(ResAssetMaster))]
        public Nullable<System.DateTime> PurchaseDate { get; set; }

        private string _PurchaseDate;
        public string PurchaseDateString
        {
            get
            {
                if (string.IsNullOrEmpty(_PurchaseDate))
                {
                    _PurchaseDate = FnCommon.ConvertDateByTimeZone(PurchaseDate, true, true);
                }
                return _PurchaseDate;
            }
            set { this._PurchaseDate = value; }
        }

        [Display(Name = "PurchaseDateStr", ResourceType = typeof(ResAssetMaster))]
        public System.String PurchaseDateStr { get; set; }

        //PurchasePrice
        [Display(Name = "PurchasePrice", ResourceType = typeof(ResAssetMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PurchasePrice { get; set; }

        //DepreciatedValue
        [Display(Name = "DepreciatedValue", ResourceType = typeof(ResAssetMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> DepreciatedValue { get; set; }

        //SuggestedMaintenanceDate
        [Display(Name = "SuggestedMaintenanceDate", ResourceType = typeof(ResAssetMaster))]
        public Nullable<System.DateTime> SuggestedMaintenanceDate { get; set; }

        [Display(Name = "SuggestedMaintenanceDate", ResourceType = typeof(ResAssetMaster))]
        public System.String SuggestedMaintenanceDateStr { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //LastUpdatedBy
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }


        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResAssetMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF10 { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "ToolCategory", ResourceType = typeof(ResToolCategory))]
        public string ToolCategory { get; set; }

        [Display(Name = "AssetCategory", ResourceType = typeof(ResAssetCategory))]
        public string AssetCategory { get; set; }

        //AssetCategoryID
        [Display(Name = "AssetCategoryID", ResourceType = typeof(ResAssetMaster))]
        public Nullable<System.Int64> AssetCategoryID { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }
        public bool IsOnlyFromItemUI { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }

        public string AppendedBarcodeString { get; set; }
        private string _Created;

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_Created))
                {
                    _Created = FnCommon.ConvertDateByTimeZone(Created, true);
                }
                return _Created;
            }
            set { this._Created = value; }
        }
        private string _Updated;
        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_Updated))
                {
                    _Updated = FnCommon.ConvertDateByTimeZone(Updated, true);
                }
                return _Updated;
            }
            set { this._Updated = value; }
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
        [Display(Name = "NoOfPastMntsToConsider", ResourceType = typeof(ResAssetMaster))]
        [Range(2, 10, ErrorMessageResourceName = "NoOfPastMntsToConsider", ErrorMessageResourceType = typeof(ResAssetMaster))]
        public int? NoOfPastMntsToConsider { get; set; }

        [Display(Name = "MaintenanceDueNoticeDays", ResourceType = typeof(ResAssetMaster))]
        public int? MaintenanceDueNoticeDays { get; set; }


        [Display(Name = "MaintenanceType", ResourceType = typeof(ResAssetMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int MaintenanceType { get; set; }
        public int DaysDiff { get; set; }

        [Display(Name = "AssetImageExternalURL", ResourceType = typeof(ResAssetMaster))]
        public System.String AssetImageExternalURL { get; set; }
        public string ImageType { get; set; }

        [Display(Name = "ImagePath", ResourceType = typeof(ResItemMaster))]
        public System.String ImagePath { get; set; }
        public System.String MaintenanceName { get; set; }
        public int? TotalRecords { get; set; }

        public DateTime? ScheduleDate  { get;set;}
        public int? TrackngMeasurement { get; set; }
    }

    public class ResAssetMaster
    {
        private static string ResourceFileName = "ResAssetMaster";

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
        public static string MaintenanceType
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceType", ResourceFileName);
            }
        }
        public static string ClickToEdit
        {
            get
            {
                return ResourceRead.GetResourceValue("ClickToEdit", ResourceFileName);
            }
        }


        public static string NoOfPastMntsToConsider
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfPastMntsToConsider", ResourceFileName);
            }
        }

        public static string MaintenanceDueNoticeDays
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceDueNoticeDays", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to AssetMaster {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to AssetMaster.
        /// </summary>
        public static string AssetMasterHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetMasterHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AssetMaster.
        /// </summary>
        public static string AssetMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetMaster", ResourceFileName);
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
        ///   Looks up a localized string similar to AssetName.
        /// </summary>
        public static string AssetName
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Make.
        /// </summary>
        public static string Make
        {
            get
            {
                return ResourceRead.GetResourceValue("Make", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Model.
        /// </summary>
        public static string Model
        {
            get
            {
                return ResourceRead.GetResourceValue("Model", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Serial.
        /// </summary>
        public static string Serial
        {
            get
            {
                return ResourceRead.GetResourceValue("Serial", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to ToolCategory.
        /// </summary>
        public static string ToolCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCategory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ToolCategoryID.
        /// </summary>
        public static string ToolCategoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCategoryID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AssetCategoryID.
        /// </summary>
        public static string AssetCategoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetCategoryID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AssetCategory.
        /// </summary>
        public static string AssetCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetCategory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PurchaseDate.
        /// </summary>
        public static string PurchaseDate
        {
            get
            {
                return ResourceRead.GetResourceValue("PurchaseDate", ResourceFileName);
            }
        }

        
        public static string PurchaseDateStr
        {
            get
            {
                return ResourceRead.GetResourceValue("PurchaseDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PurchasePrice.
        /// </summary>
        public static string PurchasePrice
        {
            get
            {
                return ResourceRead.GetResourceValue("PurchasePrice", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DepreciatedValue.
        /// </summary>
        public static string DepreciatedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("DepreciatedValue", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SuggestedMaintenanceDate.
        /// </summary>
        public static string SuggestedMaintenanceDate
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedMaintenanceDate", ResourceFileName);
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
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
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
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyID", ResourceFileName);
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


        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", ResourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", ResourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", ResourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", ResourceFileName, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", ResourceFileName, true);
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

        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }
        public static string AssetImage
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetImage", ResourceFileName);
            }
        }

        public static string ImagePath
        {
            get
            {
                return ResourceRead.GetResourceValue("ImagePath", ResourceFileName);
            }
        }

        public static string AssetImageExternalURL
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetImageExternalURL", ResourceFileName);
            }
        }
        public static string msgtoviewScheduleList
        {
            get
            {
                return ResourceRead.GetResourceValue("msgtoviewScheduleList", ResourceFileName);
            }
        }
        public static string MsgAssetDoesNotExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAssetDoesNotExist", ResourceFileName);
            }
        }
        public static string MsgItemQtyUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgItemQtyUpdated", ResourceFileName);
            }
        }
        public static string WOTypeWorkorder
        {
            get
            {
                return ResourceRead.GetResourceValue("WOTypeWorkorder", ResourceFileName);
            }
        }

        public static string WOTypeRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("WOTypeRequisition", ResourceFileName);
            }
        }

        public static string WOTypeToolService
        {
            get
            {
                return ResourceRead.GetResourceValue("WOTypeToolService", ResourceFileName);
            }
        }

        public static string WOTypeAssetService
        {
            get
            {
                return ResourceRead.GetResourceValue("WOTypeAssetService", ResourceFileName);
            }
        }
        public static string MsgImageMoved
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgImageMoved", ResourceFileName);
            }
        }
        public static string MsgCheckinCheckoutValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckinCheckoutValidation", ResourceFileName);
            }
        }
        public static string MsgInvalidOperationNoCheckout
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidOperationNoCheckout", ResourceFileName);
            }
        }
        public static string MsgInsertProperQuantityValue
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInsertProperQuantityValue", ResourceFileName);
            }
        }
        public static string MsgCheckInValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckInValidation", ResourceFileName);
            }
        }
        public static string MsgCheckedOutQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckedOutQuantityValidation", ResourceFileName);
            }
        }
        

        public static string TitleNextMaintenanceDue
        {
            get
            {
                return ResourceRead.GetResourceValue("TitleNextMaintenanceDue", ResourceFileName);
            }
        }

    }
}


