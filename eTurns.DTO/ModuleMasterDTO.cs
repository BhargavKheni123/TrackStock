using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ModuleMasterDTO
    {
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string ModuleName { get; set; }

        [Display(Name = "ParentModule", ResourceType = typeof(ResModuleMaster))]
        public Nullable<Int64> ParentID { get; set; }

        [Display(Name = "ModuleValue", ResourceType = typeof(ResModuleMaster))]
        public string Value { get; set; }


        [Display(Name = "ModulePriority", ResourceType = typeof(ResModuleMaster))]
        public int Priority { get; set; }

        [Display(Name = "ModuleDisplayText", ResourceType = typeof(ResModuleMaster))]
        public string DisplayName { get; set; }

        [Display(Name = "ModuleType", ResourceType = typeof(ResModuleMaster))]
        public bool IsModule { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public DateTime? Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public DateTime? Updated { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> Room { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public bool IsDeleted { get; set; }

        [Display(Name = "GroupId", ResourceType = typeof(ResModuleMaster))]
        public int? GroupId { get; set; }

        public string resourcekey { get; set; }
        public string ModuleNameFromResource
        {
            get
            {
                if (!string.IsNullOrEmpty(resourcekey))
                    return ResourceRead.GetResourceValue(resourcekey, "ResModuleName");
                else
                    return ModuleName;
            }
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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }
        public int? TotalRecords { get; set; }

    }

    public class ResModuleMaster
    {
        private static string resourceFile = "ResModuleMaster";

        /// <summary>
        ///   Looks up a localized string similar to GroupId.
        /// </summary>
        public static string GroupId
        {
            get
            {
                return ResourceRead.GetResourceValue("GroupId", resourceFile);
            }
        }
        public static string TemplateName
        {
            get
            {
                return ResourceRead.GetResourceValue("TemplateName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Module Display Text.
        /// </summary>
        public static string ModuleDisplayText
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleDisplayText", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Module Name.
        /// </summary>
        public static string ModuleName
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Module priority.
        /// </summary>
        public static string ModulePriority
        {
            get
            {
                return ResourceRead.GetResourceValue("ModulePriority", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Module type.
        /// </summary>
        public static string ModuleType
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Module Value.
        /// </summary>
        public static string ModuleValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleValue", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Modules.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Modules.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Parent Module.
        /// </summary>
        public static string ParentModule
        {
            get
            {
                return ResourceRead.GetResourceValue("ParentModule", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile);
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
                return ResourceRead.GetResourceValue("UDF2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile);
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
		 public static string SupportTablePermissions
        {
            get
            {
                return ResourceRead.GetResourceValue("SupportTablePermissions", resourceFile);
            }
        }

        public static string ModulePermissions
        {
            get
            {
                return ResourceRead.GetResourceValue("ModulePermissions", resourceFile);
            }
        }

        public static string AdminPermissions
        {
            get
            {
                return ResourceRead.GetResourceValue("AdminPermissions", resourceFile);
            }
        }

        public static string DefaultSettings
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultSettings", resourceFile);
            }
        }

        public static string SpecialPermissions
        {
            get
            {
                return ResourceRead.GetResourceValue("SpecialPermissions", resourceFile);
            }
        }
    }

    public class ParentModuleMasterDTO
    {
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Display(Name = "ModuleName", ResourceType = typeof(ResCommon))]
        public string ModuleName { get; set; }

        [Display(Name = "ParentModuleName", ResourceType = typeof(ResCommon))]
        public string ParentModuleName { get; set; }
    }

    public class ModuleNotificationDTO
    {
        [Display(Name = "NotificationID", ResourceType = typeof(ResCommon))]
        public Int64 NotificationID { get; set; }

        [Display(Name = "ModuleId", ResourceType = typeof(ResCommon))]
        public Int64 ModuleId { get; set; }

        [Display(Name = "UserID", ResourceType = typeof(ResCommon))]
        public Int64 UserID { get; set; }

        [Display(Name = "RoomID", ResourceType = typeof(ResCommon))]
        public Int64 RoomID { get; set; }

        [Display(Name = "Notification", ResourceType = typeof(ResCommon))]
        public int Notification { get; set; }

    }
}
