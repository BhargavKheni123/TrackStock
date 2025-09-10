using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class ProjectSpendItemsDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }


        //ProjectGUID
        [Display(Name = "ProjectGUID", ResourceType = typeof(ResProjectSpendItems))]
        public Nullable<Guid> ProjectGUID { get; set; }


        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResProjectSpendItems))]
        public Nullable<Guid> ItemGUID { get; set; }

        //QuantityLimit
        [Display(Name = "QuantityLimit", ResourceType = typeof(ResProjectSpendItems))]
        public Nullable<System.Double> QuantityLimit { get; set; }

        //QuantityUsed
        [Display(Name = "QuantityUsed", ResourceType = typeof(ResProjectSpendItems))]
        public Nullable<System.Double> QuantityUsed { get; set; }

        //DollarLimitAmount
        [Display(Name = "DollarLimitAmount", ResourceType = typeof(ResProjectSpendItems))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 9999999999, ErrorMessageResourceName = "InvalidDataRange", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Decimal> DollarLimitAmount { get; set; }

        //DollarUsedAmount
        [Display(Name = "DollarUsedAmount", ResourceType = typeof(ResProjectSpendItems))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Decimal> DollarUsedAmount { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResProjectSpendItems))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResProjectSpendItems))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResProjectSpendItems))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResProjectSpendItems))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResProjectSpendItems))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        public Nullable<System.DateTime> Updated { get; set; }

        //LastUpdated
        [Display(Name = "UpdatedOn", ResourceType = typeof(ResProjectSpendItems))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

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

        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        public string ItemNumber { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResItemMaster))]
        public string Description { get; set; }


        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }

        public Nullable<double> ItemCost { get; set; }

        public string ProjectSpendName { get; set; }

        public long RownumberPS { get; set; }
        public double ProjectPercent { get; set; }

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


        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

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

        public Nullable<Boolean> TrackAllUsageAgainstThis { get; set; }
        public Nullable<Boolean> IsClosed { get; set; }
        public Nullable<System.Decimal> RemainsSpend { get; set; }

        public int TotalRecords { get; set; }
    }

    //public class ProjectSpendItems : ItemMasterDTO
    //{
    //    //public int StageQuentity { get; set; }
    //    public string Supplier { get; set; }
    //    public string Category { get; set; }
    //    public Int64 ProjectSpendItemID { get; set; }
    //    public Guid? ProjectSpendItemGuiD { get; set; }
    //    public Int64 ProjectSpendMasterID { get; set; }
    //    public Guid ProjectSpendGuiD { get; set; }
    //    public Nullable<System.Int32> QuantityLimit { get; set; }
    //    public Nullable<System.Int32> QuantityUsed { get; set; }
    //    public Nullable<System.Decimal> DollarLimitAmount { get; set; }
    //    public Nullable<System.Decimal> DollarUsedAmount { get; set; }


    //}


    public class ResProjectSpendItems
    {
        private static string ResourceFileName = "ResProjectSpendItems";

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
        ///   Looks up a localized string similar to ProjectSpendItems {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to ProjectSpendItems.
        /// </summary>
        public static string ProjectSpendItemsHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendItemsHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectSpendItems.
        /// </summary>
        public static string ProjectSpendItems
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendItems", ResourceFileName);
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
        ///   Looks up a localized string similar to ProjectID.
        /// </summary>
        public static string ProjectID
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectGUID.
        /// </summary>
        public static string ProjectGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectGUID", ResourceFileName);
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
        ///   Looks up a localized string similar to QuantityLimit.
        /// </summary>
        public static string QuantityLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityLimit", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityUsed.
        /// </summary>
        public static string QuantityUsed
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityUsed", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DollarLimitAmount.
        /// </summary>
        public static string DollarLimitAmount
        {
            get
            {
                return ResourceRead.GetResourceValue("DollarLimitAmount", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DollarUsedAmount.
        /// </summary>
        public static string DollarUsedAmount
        {
            get
            {
                return ResourceRead.GetResourceValue("DollarUsedAmount", ResourceFileName);
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
        public static string NumberValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("NumberValidation", ResourceFileName);
            }
        }
        public static string AddItemProjectSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("AddItemProjectSpend", ResourceFileName);
            }
        }
        public static string SpendLimitText
        {
            get
            {
                return ResourceRead.GetResourceValue("SpendLimitText", ResourceFileName);
            }
        }
        public static string TotalSpendlimit
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalSpendlimit", ResourceFileName);
            }
        }
        public static string TotalSpendremaining
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalSpendremaining", ResourceFileName);
            }
        }
        public static string TotalItemSpendlimit
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalItemSpendlimit", ResourceFileName);
            }
        }
    }
}


