using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class CategoryMasterDTO
    {
        [Display(Name = "GUID")]
        public Guid GUID { get; set; }

        [Display(Name = "CategoryID")]
        public long ID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Category", ResourceType = typeof(ResCategoryMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Category { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]

        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> Room { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<bool> IsArchived { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }


        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }


        [Display(Name = "UDF1", ResourceType = typeof(ResCategoryMaster))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResCategoryMaster))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResCategoryMaster))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResCategoryMaster))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResCategoryMaster))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResCategoryMaster))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResCategoryMaster))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResCategoryMaster))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResCategoryMaster))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResCategoryMaster))]
        public string UDF10 { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CategoryColor", ResourceType = typeof(ResCategoryMaster))]
        public string CategoryColor { get; set; }
        public bool isForBOM { get; set; }
        public long? RefBomId { get; set; }
        public string CreatedDate { get; set; }
        public double ServerOffset { get; set; }
        public string UpdatedDate { get; set; }

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
        public Nullable<int> Count { get; set; }
        public int? TotalRecords { get; set; }
    }

    /// <summary>
    /// Resource of Category Masters
    /// </summary>
    public class ResCategoryMaster
    {
        private static string resourceFile = "ResCategoryMaster";

        /// <summary>
        ///   Looks up a localized string similar to CategoryID.
        /// </summary>
        public static string CategoryID
        {

            get
            {
                return ResourceRead.GetResourceValue("CategoryID", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Categories.
        /// </summary>
        public static string Category
        {
            get
            {
                return ResourceRead.GetResourceValue("Category", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Categories.
        /// </summary>
        public static string CategoryHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("CategoryHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Categories.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile,true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CategoryColor.
        /// </summary>
        public static string CategoryColor
        {
            get
            {
                return ResourceRead.GetResourceValue("CategoryColor", resourceFile);
            }
        }

        public static string SelectCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectCategory", resourceFile);
            }
        }
    }
}
