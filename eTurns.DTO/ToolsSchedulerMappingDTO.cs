using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ToolsSchedulerMappingDTO
    {
        public long ID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "SchedulerFor", ResourceType = typeof(ResToolsSchedulerMapping))]
        public Nullable<byte> SchedulerFor { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "AssetToolGUID", ResourceType = typeof(ResToolsSchedulerMapping))]
        public Nullable<Guid> AssetToolGUID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ToolSchedulerGuid", ResourceType = typeof(ResToolsSchedulerMapping))]
        public Nullable<Guid> ToolSchedulerGuid { get; set; }

        public Nullable<Guid> ToolGUID { get; set; }
        public Nullable<Guid> AssetGUID { get; set; }
        [Display(Name = "SchedulerType", ResourceType = typeof(ResToolsSchedulerMapping))]
        public Nullable<byte> SchedulerType { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<DateTime> Updated { get; set; }
        public Nullable<long> LastUpdatedBy { get; set; }
        public long Room { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Guid GUID { get; set; }
        public long CompanyID { get; set; }
        [Display(Name = "UDF1", ResourceType = typeof(ResToolsSchedulerMapping))]
        public string UDF1 { get; set; }
        [Display(Name = "UDF2", ResourceType = typeof(ResToolsSchedulerMapping))]
        public string UDF2 { get; set; }
        [Display(Name = "UDF3", ResourceType = typeof(ResToolsSchedulerMapping))]
        public string UDF3 { get; set; }
        [Display(Name = "UDF4", ResourceType = typeof(ResToolsSchedulerMapping))]
        public string UDF4 { get; set; }
        [Display(Name = "UDF5", ResourceType = typeof(ResToolsSchedulerMapping))]
        public string UDF5 { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        [Display(Name = "SchedulerForName", ResourceType = typeof(ResToolsSchedulerMapping))]
        public string SchedulerForName { get; set; }
        [Display(Name = "SchedulerType", ResourceType = typeof(ResToolsSchedulerMapping))]
        public string SchedulerTypeName { get; set; }
        [Display(Name = "Itemname", ResourceType = typeof(ResToolsSchedulerMapping))]
        public string Itemname { get; set; }
        public string AssetName { get; set; }
        public string ToolName { get; set; }

        [Display(Name = "SchedulerName", ResourceType = typeof(ResToolsScheduler))]
        public string SchedulerName { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }

        [Display(Name = "MaintenanceName", ResourceType = typeof(ResToolsMaintenance))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String MaintenanceName { get; set; }


        [Display(Name = "TrackingMeasurement", ResourceType = typeof(ResToolsMaintenance))]
        public int? TrackingMeasurement { get; set; }
        public string Serial { get; set; }
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

        public AssetMasterDTO AssetInfo { get; set; }
        public ToolMasterDTO ToolInfo { get; set; }
        public ToolsSchedulerDTO ToolScheduleInfo { get; set; }


    }
    public class ResToolsSchedulerMapping
    {
        private static string ResourceFileName = "ResToolsSchedulerMapping";
        public static string SchedulerFor
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerFor", ResourceFileName);
            }
        }
        public static string SchedulerType
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerType", ResourceFileName);
            }
        }
        public static string Itemname
        {
            get
            {
                return ResourceRead.GetResourceValue("Itemname", ResourceFileName);
            }
        }

        
        public static string AssetToolGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("Itemname", ResourceFileName);
            }
        }

        public static string SchedulerName
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerName", ResourceFileName);
            }
        }
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

        public static string errToolMappingNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("errToolMappingNotAllowed", ResourceFileName);
            }
        }
        public static string errToolMappingNotAllowedAsset
        {
            get
            {
                return ResourceRead.GetResourceValue("errToolMappingNotAllowedAsset", ResourceFileName);
            }
        }
        public static string Id
        {
            get
            {
                return ResourceRead.GetResourceValue("Id", ResourceFileName);
            }
        }
        public static string SchedulerForName
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerForName", ResourceFileName);
            }
        }

        public static string ToolSchedulerGuid
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerForName", ResourceFileName);
            }
        }

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
        public static string SchedulerMappingExist
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerMappingExist", ResourceFileName);
            }
        }
        public static string ErrorAssetName
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorAssetName", ResourceFileName);
            }
        }
        public static string ErrorToolName
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorToolName", ResourceFileName);
            }
        }
    }
    public class clslist
    {
        public string Text { get; set; }
        public string Guid { get; set; }
    }
}
