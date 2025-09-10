using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    [Serializable]
    public class RoleMasterDTO
    {

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage)), Display(Name = "RoleName", ResourceType = typeof(ResRoleMaster))]
        [StringLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string RoleName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResRoleMaster))]
        public string Description { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public DateTime? Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public DateTime? Updated { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Int64 Room { get; set; }

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

        [Display(Name = "IsArchived", ResourceType = typeof(ResCommon))]
        public bool IsArchived { get; set; }

        [Display(Name = "IsDeleted", ResourceType = typeof(ResCommon))]
        public bool IsDeleted { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResRoleMaster))]
        public bool IsActive { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        public RoleWiseRoomsAccessDetailsDTO RoleRoomsAccessDetail { get; set; }

        public List<RoleWiseRoomsAccessDetailsDTO> RoleWiseRoomsAccessDetails { get; set; }

        public List<RoleRoomReplanishmentDetailsDTO> ReplenishingRooms { get; set; }

        public List<RoleModuleDetailsDTO> PermissionList { get; set; }

        public List<RoleModuleDetailsDTO> ModuleMasterList { get; set; }

        public List<RoleModuleDetailsDTO> OtherModuleList { get; set; }

        public List<RoleModuleDetailsDTO> NonModuleList { get; set; }

        public string SelectedModuleIDs { get; set; }
        public string SelectedNonModuleIDs { get; set; }
        public string SelectedDefaultSettings { get; set; }
        public string SelectedEnterpriseAccessValue { get; set; }
        public string SelectedCompanyAccessValue { get; set; }
        public string SelectedRoomAccessValue { get; set; }
        public string SelectedRoomReplanishmentValue { get; set; }

        [Display(Name = "UserType", ResourceType = typeof(ResRoleMaster))]
        public int UserType { get; set; }
        public string UserTypeName { get; set; }
        public long EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public bool ActionExecuited { get; set; }


        public string RoleRoomAccess { get; set; }
        public string RoleRoomAccessName { get; set; }

        public List<RolePermissionInfo> lstRolePermissions { get; set; }
        public bool CanBeDeleted { get; set; }
        public List<UserAccessDTO> lstAccess { get; set; }


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
        public Nullable<Int64> UserID { get; set; }
        public int TotalRecords { get; set; }

        /// <summary>
        /// Is Enterprise or Company or Room Access changed
        /// </summary>
        public bool IsECRAccessUpdated { get; set; }
    }

    public class RoleRoomReplanishmentDetailsDTO
    {
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Display(Name = "RoomID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoomID { get; set; }

        [Display(Name = "RoleID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoleID { get; set; }

        [Display(Name = "IsRoomAccess", ResourceType = typeof(ResRoleMaster))]
        public bool IsRoomAccess { get; set; }

        [Display(Name = "Room ID")]
        public string RoomName { get; set; }

        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long EnterpriseId { get; set; }

    }
    [Serializable]
    public class RoleWiseRoomsAccessDetailsDTO
    {

        [Display(Name = "RoleID", ResourceType = typeof(ResRoleMaster))]
        public Int64? RoleID { get; set; }

        [Display(Name = "RoomID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoomID { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResRoleMaster))]
        public string RoomName { get; set; }
        public Int64 CompanyID { get; set; }
        public Int64 EnterpriseID { get; set; }

        public string CompanyName { get; set; }
        public string EnterPriseName { get; set; }
        public List<RoleModuleDetailsDTO> PermissionList { get; set; }

        public List<RoleModuleDetailsDTO> ModuleMasterList { get; set; }

        public List<RoleModuleDetailsDTO> OtherModuleList { get; set; }

        public List<RoleModuleDetailsDTO> NonModuleList { get; set; }

        public List<RoleModuleDetailsDTO> OtherDefaultSettings { get; set; }
    }
    [Serializable]
    public class RoleModuleDetailsDTO
    {

        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64? ID { get; set; }

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Display(Name = "RoleID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoleID { get; set; }

        [Display(Name = "RoomID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoomId { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResRoleMaster))]
        public string RoomName { get; set; }

        [Display(Name = "ModuleID", ResourceType = typeof(ResRoleMaster))]
        public Int64 ModuleID { get; set; }

        [Display(Name = "ModuleName", ResourceType = typeof(ResRoleMaster))]
        public string ModuleName { get; set; }

        [Display(Name = "Insert", ResourceType = typeof(ResRoleMaster))]
        public bool IsInsert { get; set; }

        [Display(Name = "Update", ResourceType = typeof(ResRoleMaster))]
        public bool IsUpdate { get; set; }

        [Display(Name = "Delete", ResourceType = typeof(ResRoleMaster))]
        public bool IsDelete { get; set; }

        [Display(Name = "View", ResourceType = typeof(ResRoleMaster))]
        public bool IsView { get; set; }

        [Display(Name = "IsModule", ResourceType = typeof(ResRoleMaster))]
        public bool? IsModule { get; set; }

        [Display(Name = "GroupId", ResourceType = typeof(ResRoleMaster))]
        public int? GroupId { get; set; }

        [Display(Name = "IsChecked", ResourceType = typeof(ResRoleMaster))]
        public bool IsChecked { get; set; }

        [Display(Name = "CreatedRoom", ResourceType = typeof(ResRoleMaster))]
        public Int64? CreatedRoom { get; set; }

        [Display(Name = "ModuleValue", ResourceType = typeof(ResRoleMaster))]
        public string ModuleValue { get; set; }

        [Display(Name = "ShowDeleted", ResourceType = typeof(ResRoleMaster))]
        public bool ShowDeleted { get; set; }

        [Display(Name = "ShowArchived", ResourceType = typeof(ResRoleMaster))]
        public bool ShowArchived { get; set; }

        [Display(Name = "ShowUDF", ResourceType = typeof(ResRoleMaster))]
        public bool ShowUDF { get; set; }
        [Display(Name = "ShowChangeLog", ResourceType = typeof(ResRoleMaster))]
        public bool ShowChangeLog { get; set; }
        public Int64 CompanyID { get; set; }
        public string CompanyName { get; set; }
        public Int64 EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public int? DisplayOrderNumber { get; set; }
        public string DisplayOrderName { get; set; }
        public string resourcekey { get; set; }
        public string ToolTipResourceKey { get; set; }
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
        public string ModuleToolTipFromResource
        {
            get
            {
                if (!string.IsNullOrEmpty(ToolTipResourceKey))
                    return ResourceRead.GetResourceValue(ToolTipResourceKey, "ResModuleName");
                else
                    return ModuleName;
            }
        }

    }

    public enum RoleOrderStatus
    {
        Barcode = 1010,
        Configuration = 1011,
        Consignment = 1012,
        Item = 1013,
        Order = 1014,
        ProjectSpend = 1015,
        Requisition = 1016,
        SuperAdmins = 1017,
        Support = 1018,
        Transfer = 1019,
        User = 1020,
        General = 1021,
        Pull = 1022,
        Cart = 1000,
        Tools = 1023,
        Dashboard = 1024,
        ToolAssetOrder = 1025,
        Quote = 1026,
        Reporting = 1027
        //ToolWrittenOff = 1026
    }

    public class ResRoleMaster
    {
        private static string resourceFile = "ResRoleMaster";//typeof(ResRoleMaster).Name;

        /// <summary>
        ///   Looks up a localized string similar to All.
        /// </summary>
        public static string All
        {
            get
            {
                return ResourceRead.GetResourceValue("All", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Copy Permission To.
        /// </summary>
        public static string CopyPermissionTo
        {
            get
            {
                return ResourceRead.GetResourceValue("CopyPermissionTo", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreatedRoom.
        /// </summary>
        public static string CreatedRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedRoom", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Delete.
        /// </summary>
        public static string Delete
        {
            get
            {
                return ResourceRead.GetResourceValue("Delete", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", resourceFile);
            }
        }

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

        /// <summary>
        ///   Looks up a localized string similar to Insert.
        /// </summary>
        public static string Insert
        {
            get
            {
                return ResourceRead.GetResourceValue("Insert", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsActive.
        /// </summary>
        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsChecked.
        /// </summary>
        public static string IsChecked
        {
            get
            {
                return ResourceRead.GetResourceValue("IsChecked", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsModule.
        /// </summary>
        public static string IsModule
        {
            get
            {
                return ResourceRead.GetResourceValue("IsModule", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ModuleID.
        /// </summary>
        public static string ModuleID
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleID", resourceFile);
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
        ///   Looks up a localized string similar to Roles.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Roles.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Permissions.
        /// </summary>
        public static string Permissions
        {
            get
            {
                return ResourceRead.GetResourceValue("Permissions", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RoleID.
        /// </summary>
        public static string RoleID
        {
            get
            {
                return ResourceRead.GetResourceValue("RoleID", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Role Name.
        /// </summary>
        public static string RoleName
        {
            get
            {
                return ResourceRead.GetResourceValue("RoleName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room Name.
        /// </summary>
        public static string RoomName
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomName", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Room Access.
        /// </summary>
        public static string RoomAccess
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomAccess", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Room Access.
        /// </summary>
        public static string CompanyAccess
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyAccess", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Room Access.
        /// </summary>
        public static string EnterpriseAccess
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterpriseAccess", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room Details.
        /// </summary>
        public static string RoomDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomDetails", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room ID.
        /// </summary>
        public static string RoomID
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomID", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room Replenishment.
        /// </summary>
        public static string RoomReplenishment
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomReplenishment", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Selected Rooms.
        /// </summary>
        public static string SelectedRooms
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectedRooms", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Support tables permissions.
        /// </summary>
        public static string SupportTablesPermissions
        {
            get
            {
                return ResourceRead.GetResourceValue("SupportTablesPermissions", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Update.
        /// </summary>
        public static string Update
        {
            get
            {
                return ResourceRead.GetResourceValue("Update", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View.
        /// </summary>
        public static string View
        {
            get
            {
                return ResourceRead.GetResourceValue("View", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to View.
        /// </summary>
        public static string ModulePermissions
        {
            get
            {
                return ResourceRead.GetResourceValue("ModulePermissions", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to AdminPermissions.
        /// </summary>
        public static string AdminPermissions
        {
            get
            {
                return ResourceRead.GetResourceValue("AdminPermissions", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to DefaultSettings.
        /// </summary>
        public static string DefaultSettings
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultSettings", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View.
        /// </summary>
        public static string ShowDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowDeleted", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to View.
        /// </summary>
        public static string ShowArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowArchived", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to View.
        /// </summary>
        public static string ShowUDF
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowUDF", resourceFile);
            }
        }
        public static string ShowChangeLog
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowChangeLog", resourceFile);
            }
        }

        public static string UserType
        {
            get
            {
                return ResourceRead.GetResourceValue("UserType", resourceFile);
            }
        }

        public static string hdrRoleDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("hdrRoleDetail", resourceFile);
            }
        }
        public static string hdrRoomDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("hdrRoomDetail", resourceFile);
            }
        }
        public static string lblRoomReplenishment
        {
            get
            {
                return ResourceRead.GetResourceValue("lblRoomReplenishment", resourceFile);
            }
        }
        public static string lblEnterprises
        {
            get
            {
                return ResourceRead.GetResourceValue("lblEnterprises", resourceFile);
            }
        }
        public static string lblCompanies
        {
            get
            {
                return ResourceRead.GetResourceValue("lblCompanies", resourceFile);
            }
        }
        public static string lblRooms
        {
            get
            {
                return ResourceRead.GetResourceValue("lblRooms", resourceFile);
            }
        }
        public static string lblRoomsSupplier
        {
            get
            {
                return ResourceRead.GetResourceValue("lblRoomsSupplier", resourceFile);
            }
        }
        public static string lblSelectedRooms
        {
            get
            {
                return ResourceRead.GetResourceValue("lblSelectedRooms", resourceFile);
            }
        }
        public static string lblCopyPermissionTo
        {
            get
            {
                return ResourceRead.GetResourceValue("lblCopyPermissionTo", resourceFile);
            }
        }

        public static string ReqRole
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRole", resourceFile);
            }
        }

        public static string ReqRoomAccess
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRoomAccess", resourceFile);
            }
        }

		public static string MsgRoleNotSavedForHigherUser
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoleNotSavedForHigherUser", resourceFile);
            }
        }
        public static string MsgSelectRoomsCopyPermission
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRoomsCopyPermission", resourceFile);
            }
        }
        public static string MsgPermissionCopied
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPermissionCopied", resourceFile);
            }
        }
        public static string MsgRoomToCopyPermission
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoomToCopyPermission", resourceFile);
            }
        }
        public static string MsgUserNameAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUserNameAlreadyExist", resourceFile);
            }
        }
        public static string Copy
        {
            get
            {
                return ResourceRead.GetResourceValue("Copy", resourceFile);
            }
        }


    }
}
