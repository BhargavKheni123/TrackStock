using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace eTurns.DTO
{
    public enum RPT_PSStatus
    {
        All = 1,
        ProjDollarUsedLessDollarLimit = 2,
        ProjDollarUsedGreaterDollarLimit = 3,
        Open = 4,
        Closed = 5,
        ItemQtyUsedLessQtyLimit = 6,
        ItemQtyUsedGreaterQtyLimit = 7,
        ItemDollarUsedLessDollarLimit = 8,
        ItemDollarUsedGreaterDollarLimit = 9
    }

    public class ProjectMasterDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //ProjectSpendName
        [Display(Name = "ProjectSpendName", ResourceType = typeof(ResProjectMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ProjectSpendName { get; set; }

        //Description
        [Display(Name = "Description", ResourceType = typeof(ResProjectMaster))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Description { get; set; }

        //DollarLimitAmount
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "DollarLimitAmount", ResourceType = typeof(ResProjectMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 9999999999, ErrorMessageResourceName = "InvalidDataRange", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Decimal> DollarLimitAmount { get; set; }

        //DollarUsedAmount
        [Display(Name = "DollarUsedAmount", ResourceType = typeof(ResProjectMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Decimal> DollarUsedAmount { get; set; }

        //TrackAllUsageAgainstThis
        [Display(Name = "TrackAllUsageAgainstThis", ResourceType = typeof(ResProjectMaster))]
        public Boolean TrackAllUsageAgainstThis { get; set; }

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

        //IsClosed
        [Display(Name = "IsClosed", ResourceType = typeof(ResProjectMaster))]
        public Boolean IsClosed { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResProjectMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResProjectMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResProjectMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResProjectMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResProjectMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

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

        public List<ProjectSpendItemsDTO> ProjectSpendItems { get; set; }
        public string AppendedBarcodeString { get; set; }
        public System.String WhatWhereAction { get; set; }

        public int PullMasterCount { get; set; }


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
        public Nullable<System.Decimal> RemainsSpend { get; set; }
        public int TotalRecords { get; set; }
    }

    public class ResProjectMaster
    {
        private static string ResourceFileName = "ResProjectMaster";

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
        ///   Looks up a localized string similar to ProjectMaster {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to ProjectMaster.
        /// </summary>
        public static string ProjectMasterHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectMasterHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectMaster.
        /// </summary>
        public static string ProjectMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectMaster", ResourceFileName);
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
        ///   Looks up a localized string similar to ProjectSpendName.
        /// </summary>
        public static string ProjectSpendName
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendName", ResourceFileName);
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

        public static string QuantityLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityLimit", ResourceFileName);
            }
        }

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
        ///   Looks up a localized string similar to TrackAllUsageAgainstThis.
        /// </summary>
        public static string TrackAllUsageAgainstThis
        {
            get
            {
                return ResourceRead.GetResourceValue("TrackAllUsageAgainstThis", ResourceFileName);
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
        ///   Looks up a localized string similar to IsClosed.
        /// </summary>
        public static string IsClosed
        {
            get
            {
                return ResourceRead.GetResourceValue("IsClosed", ResourceFileName);
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

        public static string ModelHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ModelHeader", ResourceFileName);
            }
        }

        public static string OneTrackAllUsageAgainstThisAlreadyThere
        {
            get
            {
                return ResourceRead.GetResourceValue("OneTrackAllUsageAgainstThisAlreadyThere", ResourceFileName);
            }
        }
        public static string RemainsSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("RemainsSpend", ResourceFileName);
            }
        }

        public static string MoreThan100Percentage
        {
            get
            {
                return ResourceRead.GetResourceValue("MoreThan100Percentage", ResourceFileName);
            }
        }

        public static string MoreThan90Percentage
        {
            get
            {
                return ResourceRead.GetResourceValue("MoreThan90Percentage", ResourceFileName);
            }
        }

        public static string MoreThan75Percentage
        {
            get
            {
                return ResourceRead.GetResourceValue("MoreThan75Percentage", ResourceFileName);
            }
        }
        public static string MoreThan50Percentage
        {
            get
            {
                return ResourceRead.GetResourceValue("MoreThan50Percentage", ResourceFileName);
            }
        }

        


        public static string MoreThan10000
        {
            get
            {
                return ResourceRead.GetResourceValue("MoreThan10000", ResourceFileName);
            }
        }
        public static string Between7500To9999
        {
            get
            {
                return ResourceRead.GetResourceValue("Between7500To9999", ResourceFileName);
            }
        }

        public static string Between5000To7499
        {
            get
            {
                return ResourceRead.GetResourceValue("Between5000To7499", ResourceFileName);
            }
        }

        public static string Between2500To4999
        {
            get
            {
                return ResourceRead.GetResourceValue("Between2500To4999", ResourceFileName);
            }
        }

        public static string LessThan2500
        {
            get
            {
                return ResourceRead.GetResourceValue("LessThan2500", ResourceFileName);
            }
        }

        public static string BetweenZeroTo2500
        {
            get
            {
                return ResourceRead.GetResourceValue("BetweenZeroTo2500", ResourceFileName);
            }
        }
        public static string LessThanZero
        {
            get
            {
                return ResourceRead.GetResourceValue("LessThanZero", ResourceFileName);
            }
        }

        public static string ProjectSpendDollarAmountLimitExceed { get { return ResourceRead.GetResourceValue("ProjectSpendDollarAmountLimitExceed", ResourceFileName); } }
        public static string ProjectSpendItemQuantityLimitExceed { get { return ResourceRead.GetResourceValue("ProjectSpendItemQuantityLimitExceed", ResourceFileName); } }
        public static string ProjectSpendItemDollarLimitExceed { get { return ResourceRead.GetResourceValue("ProjectSpendItemDollarLimitExceed", ResourceFileName); } }
    }
}


