using eTurns.DTO.Resources;
using eTurns.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace eTurns.DTO
{
    [Serializable]
    public class UserMasterDTO
    {

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 CompanyID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "UserName", ResourceType = typeof(ResUserMaster))]
        [RegularExpression(@"[a-zA-Z0-9 ]+", ErrorMessage = "Special Charactor is not allowed.")]
        public string UserName { get; set; }


        [PasswordRequired("ConfirmPassword", "ID", ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(ResUserMaster))]
        [RegularExpression(@"^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$", ErrorMessageResourceName = "errPasswordRuleBreak", ErrorMessageResourceType = typeof(ResUserMaster))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ResUserMaster))]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(ResCommon))]
        public string Phone { get; set; }

        [Display(Name = "RoleID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoleID { get; set; }

        [Display(Name = "RoleName", ResourceType = typeof(ResRoleMaster))]
        public string RoleName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"[a-zA-Z0-9.!#$%&'*+-/=?\^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+", ErrorMessage = "Please enter correct email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(ResCommon))]
        public string Email { get; set; }

        //public int SyncMinutes { get; set; }
        //public DateTime SyncTime { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public DateTime Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        public string EnterpriseName { get; set; }
        //public string LoginID { get; set; }
        //public RoleMasterDTO RomeWiseRol { get; set; }

        public UserWiseRoomsAccessDetailsDTO UserwiseRoomsAccessDetail { get; set; }

        public List<UserRoleModuleDetailsDTO> PermissionList { get; set; }

        public List<UserWiseRoomsAccessDetailsDTO> UserWiseAllRoomsAccessDetails { get; set; }

        public List<UserRoomReplanishmentDetailsDTO> ReplenishingRooms { get; set; }


        public List<UserRoleModuleDetailsDTO> ModuleMasterList { get; set; }

        public List<UserRoleModuleDetailsDTO> OtherModuleList { get; set; }

        public List<UserRoleModuleDetailsDTO> NonModuleList { get; set; }


        public string SelectedModuleIDs { get; set; }
        public string SelectedNonModuleIDs { get; set; }
        public string SelectedDefaultSettings { get; set; }

        public string SelectedRoomAccessValue { get; set; }
        public string SelectedRoomReplanishmentValue { get; set; }

        public string EnterpriseDbName { get; set; }
        public string EnterPriseDomainURL { get; set; }
        public long EnterpriseId { get; set; }
        public int UserType { get; set; }
        public string SelectedEnterpriseAccessValue { get; set; }
        public string SelectedCompanyAccessValue { get; set; }
        public string UserTypeName { get; set; }
        public IEnumerable<RolePermissionInfo> lstUserPermissions { get; set; }
        public string EnterPriseId_CompanyId_RoomId { get; set; }
        public string[] arrEnterPriseId_CompanyId_RoomId { get; set; }
        public string UserRoomAccess { get; set; }
        public bool IsLicenceAccepted { get; set; }
        public string CompanyName { get; set; }
        public string PasswordResetLink { get; set; }
        public Guid? PasswordResetRequestID { get; set; }
        public List<UserAccessDTO> lstAccess { get; set; }
        public bool IsLocked { get; set; }

        public bool EnforceRolePermission { get; set; }
        public bool HasChangedFirstPassword { get; set; }
        public int TotalRecords { get; set; }
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

        [Display(Name = "RedirectPage", ResourceType = typeof(Resources.ResCommon))]
        public string RedirectURL { get; set; }

        public Int64 EnterPriseSuperAdminID { get; set; }

        [Display(Name = "FirstLicenceAccept", ResourceType = typeof(Resources.ResCommon))]
        public DateTime? FirstLicenceAccept { get; set; }

        private string _firstLicenceAccept;
        public string FirstLicenceAcceptStr
        {
            get
            {
                if (string.IsNullOrEmpty(_firstLicenceAccept))
                {
                    _firstLicenceAccept = FnCommon.ConvertDateByTimeZone(FirstLicenceAccept, true);
                }
                return _firstLicenceAccept;
            }
            set { this._firstLicenceAccept = value; }
        }
        [Display(Name = "LastLicenceAccept", ResourceType = typeof(Resources.ResCommon))]
        public DateTime? LastLicenceAccept { get; set; }

        private string _lastLicenceAccept;
        public string LastLicenceAcceptStr
        {
            get
            {
                if (string.IsNullOrEmpty(_lastLicenceAccept))
                {
                    _lastLicenceAccept = FnCommon.ConvertDateByTimeZone(LastLicenceAccept, true);
                }
                return _lastLicenceAccept;
            }
            set { this._lastLicenceAccept = value; }
        }
        [Display(Name = "Acceptcount", ResourceType = typeof(Resources.ResCommon))]
        public int Acceptcount { get; set; }
        public int DaysRemains { get; set; }

        public DateTime? LastSyncDateTime { get; set; }
        public string PDABuildVersion { get; set; }

        private string _LastSyncDateTimeStr;
        public string LastSyncDateTimeStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LastSyncDateTimeStr))
                {
                    _LastSyncDateTimeStr = FnCommon.ConvertDateByTimeZone(LastSyncDateTime, true);
                }
                return _LastSyncDateTimeStr;
            }
            set { this._LastSyncDateTimeStr = value; }
        }
        public bool NewEulaAccept { get; set; }
        public bool IseTurnsAdmin { get; set; }

        public Int64 UserSettingID { get; set; }

        [Display(Name = "IsRequistionReportDisplay", ResourceType = typeof(Resources.ResCommon))]
        public bool? IsRequistionReportDisplay { get; set; }

        [Display(Name = "SearchPattern", ResourceType = typeof(ResUserMaster))]
        public Int32 SearchPattern { get; set; }


        [Display(Name = "IsNeverExpirePwd", ResourceType = typeof(Resources.ResCommon))]
        public bool IsNeverExpirePwd { get; set; }

        public ChangePasswordDTO objChangePassword { get; set; }

        [Display(Name = "ShowDateTime", ResourceType = typeof(Resources.ResCommon))]
        public bool ShowDateTime { get; set; }

        [Display(Name = "IsAutoLogin", ResourceType = typeof(Resources.ResCommon))]
        public bool IsAutoLogin { get; set; }
        public DateTime? LastLogin { get; set; }
        private string _LastLoginStr;
        public string LastLoginStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LastLoginStr))
                {
                    _LastLoginStr = FnCommon.ConvertDateByTimeZone(LastLogin, true);
                }
                return _LastLoginStr;
            }
            set { this._LastLoginStr = value; }
        }
        [Display(Name = "Prefix", ResourceType = typeof(ResUserMaster))]
        public string Prefix { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(ResUserMaster))]
        public string FullName { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(ResUserMaster))]
        public string FirstName { get; set; }
        [Display(Name = "MiddleName", ResourceType = typeof(ResUserMaster))]
        public string MiddleName { get; set; }
        [Display(Name = "LastName", ResourceType = typeof(ResUserMaster))]
        public string LastName { get; set; }
        [Display(Name = "Gender", ResourceType = typeof(ResUserMaster))]
        public string Gender { get; set; }
        [Display(Name = "Phone2", ResourceType = typeof(ResUserMaster))]
        public string Phone2 { get; set; }
        [Display(Name = "EmployeeNumber", ResourceType = typeof(ResUserMaster))]
        public string EmployeeNumber { get; set; }
        [Display(Name = "CostCenter", ResourceType = typeof(ResUserMaster))]
        public string CostCenter { get; set; }
        [Display(Name = "JobTitle", ResourceType = typeof(ResUserMaster))]
        public string JobTitle { get; set; }
        [Display(Name = "Address", ResourceType = typeof(ResUserMaster))]
        public string Address { get; set; }
        [Display(Name = "Country", ResourceType = typeof(ResUserMaster))]
        public string Country { get; set; }
        [Display(Name = "State", ResourceType = typeof(ResUserMaster))]
        public string State { get; set; }
        [Display(Name = "City", ResourceType = typeof(ResUserMaster))]
        public string City { get; set; }
        [Display(Name = "PostalCode", ResourceType = typeof(ResUserMaster))]
        public string PostalCode { get; set; }
        [Display(Name = "UDF1", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF1 { get; set; }
        [Display(Name = "UDF2", ResourceType = typeof(ResUserMasterUDF))]

        public string UDF2 { get; set; }
        [Display(Name = "UDF3", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF3 { get; set; }
        [Display(Name = "UDF4", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF4 { get; set; }
        [Display(Name = "UDF5", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF5 { get; set; }
        [Display(Name = "UDF6", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF6 { get; set; }
        [Display(Name = "UDF7", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF7 { get; set; }
        [Display(Name = "UDF8", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF8 { get; set; }
        [Display(Name = "UDF9", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF9 { get; set; }
        [Display(Name = "UDF10", ResourceType = typeof(ResUserMasterUDF))]
        public string UDF10 { get; set; }
        public bool IsNgNLFAllowed { get; set; }
    }
    public class UserSchedulerDTO
    {
        public long ModuleID { get; set; }
        public long UserID { get; set; }
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
        public long EnterpriseID { get; set; }
        public long RoleID { get; set; }
        public string ModuleValue { get; set; }
        public long ScheduleID { get; set; }
        public string ActionToDo { get; set; }

        public int Frequency { get; set; }
        public int TimeBaseUnit { get; set; }
        public float OrderLimit { get; set; }
        public float OrderUsedLimit { get; set; }
        public int LimitType { get; set; }

    }
    public class UserMasterLoginDTO
    {
        public Int64 ID { get; set; }
        public string UserName { get; set; }
        public string EnterpriseName { get; set; }
    }
    public class RoleDTO
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class LoginModel
    {

        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; }
        public int propnumber { get; set; }

        public string ReturnUrl { get; set; }
        public string CurrentLoggedInUserName { get; set; }
        //string Email{get;set;}; string Password, bool? IsRemember

        public string amazon_callback_uri { get; set; }
        public string amazon_state { get; set; }
        public string applicationId { get; set; }
        public string eTurns_state { get; set; }
    }
    [Serializable]
    public class UserWiseRoomsAccessDetailsDTO
    {
        [Display(Name = "RoleID", ResourceType = typeof(ResRoleMaster))]
        public Int64? RoleID { get; set; }

        [Display(Name = "RoomID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoomID { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResRoleMaster))]
        public string RoomName { get; set; }

        public List<UserRoleModuleDetailsDTO> PermissionList { get; set; }

        public List<UserRoleModuleDetailsDTO> ModuleMasterList { get; set; }

        public List<UserRoleModuleDetailsDTO> OtherModuleList { get; set; }

        public List<UserRoleModuleDetailsDTO> NonModuleList { get; set; }

        public List<UserRoleModuleDetailsDTO> OtherDefaultSettings { get; set; }

        public long EnterpriseId { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string EnterpriseName { get; set; }
        public bool IsRoomActive { get; set; }
        public bool IsCompanyActive { get; set; }
        public bool IsEnterpriseActive { get; set; }
        public int UserType { get; set; }

        public string SupplierIDs { get; set; }
    }

    public class UserRoomReplanishmentDetailsDTO
    {
        [Display(Name = "ID")]
        public Int64 ID { get; set; }

        [Display(Name = "GUID")]
        public Guid GUID { get; set; }

        [Display(Name = "UserID", ResourceType = typeof(ResUserMaster))]
        public Int64 UserID { get; set; }

        [Display(Name = "RoomID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoomID { get; set; }

        [Display(Name = "RoleID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoleID { get; set; }

        public long CompanyId { get; set; }

        public long EnterpriseId { get; set; }

        public bool IsRoomAccess { get; set; }
    }
    [Serializable]
    public class UserRoleModuleDetailsDTO
    {
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64? ID { get; set; }

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Display(Name = "RoleID", ResourceType = typeof(ResRoleMaster))]
        public Int64 RoleID { get; set; }

        [Display(Name = "UserID", ResourceType = typeof(ResUserMaster))]
        public Int64 UserID { get; set; }

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

        [Display(Name = "ParentID", ResourceType = typeof(ResRoleMaster))]
        public Int64? ParentID { get; set; }

        [Display(Name = "ModuleURL", ResourceType = typeof(ResRoleMaster))]
        public string ModuleURL { get; set; }

        [Display(Name = "ImageName", ResourceType = typeof(ResRoleMaster))]
        public string ImageName { get; set; }

        [Display(Name = "ShowDeleted", ResourceType = typeof(ResRoleMaster))]
        public bool ShowDeleted { get; set; }

        [Display(Name = "ShowArchived", ResourceType = typeof(ResRoleMaster))]
        public bool ShowArchived { get; set; }

        [Display(Name = "ShowUDF", ResourceType = typeof(ResRoleMaster))]
        public bool ShowUDF { get; set; }

        [Display(Name = "ShowChangeLog", ResourceType = typeof(ResRoleMaster))]
        public bool ShowChangeLog { get; set; }


        public long CompanyId { get; set; }
        public long EnteriseId { get; set; }
        public int? DisplayOrderNumber { get; set; }
        public bool IsRoomActive { get; set; }
        public string resourcekey { get; set; }
        public string DisplayOrderName { get; set; }

        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public bool IsCompanyActive { get; set; }
        public bool IsEnterpriseActive { get; set; }

        public string RoleName { get; set; }
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

        [Display(Name = "PermissionChanges", ResourceType = typeof(ResCommon))]
        public string PermissionChanges { get; set; }

        public bool IsRecent { get; set; }

        public DateTime HistoryDate { get; set; }

        public string HistoryDateDisplay { get; set; }

        public long UserRoleChangeLogID { get; set; }

        public int TotalRecords { get; set; }
    }

    public class ResUserMaster
    {

        private static string resourceFile = "ResUserMaster";


        /// <summary>MsgCharacterLength
        ///   Looks up a localized string similar to Confirm Password.
        /// </summary>
        public static string errPasswordRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("resPasswordRequered", resourceFile);
            }
        }
        public static string Prefix
        {
            get
            {
                return ResourceRead.GetResourceValue("Prefix", resourceFile);
            }
        }
        public static string FullName
        {
            get
            {
                return ResourceRead.GetResourceValue("FullName", resourceFile);
            }
        }

        public static string FirstName
        {
            get
            {
                return ResourceRead.GetResourceValue("FirstName", resourceFile);
            }
        }
        public static string MiddleName
        {
            get
            {
                return ResourceRead.GetResourceValue("MiddleName", resourceFile);
            }
        }
        public static string LastName
        {
            get
            {
                return ResourceRead.GetResourceValue("LastName", resourceFile);
            }
        }
        public static string Gender
        {
            get
            {
                return ResourceRead.GetResourceValue("Gender", resourceFile);
            }
        }
        public static string Phone2
        {
            get
            {
                return ResourceRead.GetResourceValue("Phone2", resourceFile);
            }
        }
        public static string EmployeeNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("EmployeeNumber", resourceFile);
            }
        }
        public static string CostCenter
        {
            get
            {
                return ResourceRead.GetResourceValue("CostCenter", resourceFile);
            }
        }
        public static string JobTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("JobTitle", resourceFile);
            }
        }
        public static string Address
        {
            get
            {
                return ResourceRead.GetResourceValue("Address", resourceFile);
            }
        }
        public static string Country
        {
            get
            {
                return ResourceRead.GetResourceValue("Country", resourceFile);
            }
        }
        public static string State
        {
            get
            {
                return ResourceRead.GetResourceValue("State", resourceFile);
            }
        }
        public static string City
        {
            get
            {
                return ResourceRead.GetResourceValue("City", resourceFile);
            }
        }
        public static string PostalCode
        {
            get
            {
                return ResourceRead.GetResourceValue("PostalCode", resourceFile);
            }
        }

        public static string errPasswordmismatch
        {
            get
            {
                return ResourceRead.GetResourceValue("errPasswordmismatch", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Confirm Password.
        /// </summary>
        public static string errPasswordRuleBreak
        {
            get
            {
                return ResourceRead.GetResourceValue("errPasswordRuleBreak", resourceFile);
            }
        }


        public static string lblPasswordRules
        {
            get
            {
                return ResourceRead.GetResourceValue("lblPasswordRules", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Confirm Password.
        /// </summary>
        public static string ConfirmPassword
        {
            get
            {
                return ResourceRead.GetResourceValue("ConfirmPassword", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Password.
        /// </summary>
        public static string Password
        {
            get
            {
                return ResourceRead.GetResourceValue("Password", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Role.
        /// </summary>
        public static string Role
        {
            get
            {
                return ResourceRead.GetResourceValue("Role", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UserID.
        /// </summary>
        public static string UserID
        {
            get
            {
                return ResourceRead.GetResourceValue("UserID", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User Name.
        /// </summary>
        public static string EmailAddress
        {
            get
            {
                return ResourceRead.GetResourceValue("Email", resourceFile);
            }
        }

        public static string IsDefault
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDefault", resourceFile);
            }
        }

        public static string MarkDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("MarkDeleted", resourceFile);
            }
        }

        public static string IsEPSuperAdmin
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEPSuperAdmin", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to User Name.
        /// </summary>
        public static string PhoneNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("PhoneNumber", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User Name.
        /// </summary>
        public static string UserName
        {
            get
            {
                return ResourceRead.GetResourceValue("UserName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User Details.
        /// </summary>
        public static string UserDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("UserDetails", resourceFile);
            }
        }

        public static string UserTypeName
        {
            get
            {
                return ResourceRead.GetResourceValue("UserTypeName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tool Categories.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }
        public static string EnforceRolePermission
        {
            get
            {
                return ResourceRead.GetResourceValue("EnforceRolePermission", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Tool Categories.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }
        public static string CurrentPassword
        {
            get
            {
                return ResourceRead.GetResourceValue("CurrentPassword", resourceFile);
            }
        }

        public static string LastSyncDateTime
        {
            get
            {
                return ResourceRead.GetResourceValue("LastSyncDateTime", resourceFile);
            }
        }

        public static string PDABuildVersion
        {
            get
            {
                return ResourceRead.GetResourceValue("PDABuildVersion", resourceFile);
            }
        }

        public static string IseTurnsAdmin
        {
            get
            {
                return ResourceRead.GetResourceValue("IseTurnsAdmin", resourceFile);
            }
        }

        public static string LastLogin
        {
            get
            {
                return ResourceRead.GetResourceValue("LastLogin", resourceFile);
            }
        }
        public static string SearchPattern
        {
            get
            {
                return ResourceRead.GetResourceValue("SearchPattern", resourceFile);
            }
        }
        public static string IncrementalSearch
        {
            get
            {
                return ResourceRead.GetResourceValue("IncrementalSearch", resourceFile);
            }
        }
        public static string EnterKeySearch
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterKeySearch", resourceFile);
            }
        }
        public static string ReqRoleAccess
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRoleAccess", resourceFile);
            }
        }
		public static string MsgHigherUserType
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgHigherUserType", resourceFile);
            }
        }

        public static string MsgRoleUserMismatch
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoleUserMismatch", resourceFile);
            }
        }

        public static string MsgOneRoomMustAssign
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgOneRoomMustAssign", resourceFile);
            }
        }

        public static string MsgUserUndeleteSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUserUndeleteSuccessfully", resourceFile);
            }
        }

        public static string MsgUserExistForUndelete
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUserExistForUndelete", resourceFile);
            }
        }

        public static string MsgCantDeleteAllEnterpriseUser
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCantDeleteAllEnterpriseUser", resourceFile);
            }
        }

        public static string MsgUserSettingSavedSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUserSettingSavedSuccess", resourceFile);
            }
        }
        public static string MsgRoomWithoutPermission
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoomWithoutPermission", resourceFile);
            }
        }
        public static string MsgUserNameAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUserNameAlreadyExist", resourceFile);
            }
        }        
        public static string MsgNoSystemUserFound
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoSystemUserFound", resourceFile);
            }
        }
        public static string MsgNoEnterpriseUserFound
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoEnterpriseUserFound", resourceFile);
            }
        }
        public static string ReqUserName
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqUserName", resourceFile);
            }
        }

        public static string MsgCharacterLength
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCharacterLength", resourceFile);
            }
        }
        public static string MsgAtleastOneNumberRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAtleastOneNumberRequired", resourceFile);
            }
        }
        public static string MsgAtleastOneCapitalRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAtleastOneCapitalRequired", resourceFile);
            }
        }
        public static string MsgAtleasetOneLetterRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAtleasetOneLetterRequired", resourceFile);
            }
        }
        public static string MsgAtleastOneSpecialLetterRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAtleastOneSpecialLetterRequired", resourceFile);
            }
        }
        public static string MsgWrongUserName
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgWrongUserName", resourceFile);
            }
        }
        public static string MsgUserRoomRightsValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUserRoomRightsValidation", resourceFile);
            }
        }
        public static string MsgUserProjectSpendRightsValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUserProjectSpendRightsValidation", resourceFile);
            }
        }
        public static string MsgUserProjectSpendRightsUpdateValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUserProjectSpendRightsUpdateValidation", resourceFile);
            }
        }

        public static string UserDoesNotExist { get { return ResourceRead.GetResourceValue("UserDoesNotExist", resourceFile); } }
        public static string ReqUserID { get { return ResourceRead.GetResourceValue("ReqUserID", resourceFile); } }
        public static string ReqCorrectUserID { get { return ResourceRead.GetResourceValue("ReqCorrectUserID", resourceFile); } }
        public static string MsgUserNoRightsForCompany { get { return ResourceRead.GetResourceValue("MsgUserNoRightsForCompany", resourceFile); } }
        public static string MsgUserNoRightsForAnyRoom { get { return ResourceRead.GetResourceValue("MsgUserNoRightsForAnyRoom", resourceFile); } }
        public static string MsgUserNotAcceptedlicence { get { return ResourceRead.GetResourceValue("MsgUserNotAcceptedlicence", resourceFile); } }
        public static string MsgCorrectUserName { get { return ResourceRead.GetResourceValue("MsgCorrectUserName", resourceFile); } }
        public static string MsgCorrectEnterpriseUserName { get { return ResourceRead.GetResourceValue("MsgCorrectEnterpriseUserName", resourceFile); } }
        public static string MsgProvidedUserNotExist { get { return ResourceRead.GetResourceValue("MsgProvidedUserNotExist", resourceFile); } }
        public static string MsgEnterOldPassword { get { return ResourceRead.GetResourceValue("MsgEnterOldPassword", resourceFile); } }
        public static string MsgEnterNewPassword { get { return ResourceRead.GetResourceValue("MsgEnterNewPassword", resourceFile); } }
        public static string MsgEnterConfirmPassword { get { return ResourceRead.GetResourceValue("MsgEnterConfirmPassword", resourceFile); } }
        public static string MsgPasswordConfirmValidation { get { return ResourceRead.GetResourceValue("MsgPasswordConfirmValidation", resourceFile); } }
        public static string MsgPasswordConfirmSameValidation { get { return ResourceRead.GetResourceValue("MsgPasswordConfirmSameValidation", resourceFile); } }
        public static string TitlePasswordRequirements { get { return ResourceRead.GetResourceValue("TitlePasswordRequirements", resourceFile); } }
        public static string TitleConfirmPasswordRequirements { get { return ResourceRead.GetResourceValue("TitleConfirmPasswordRequirements", resourceFile); } }
        public static string AtLeast { get { return ResourceRead.GetResourceValue("AtLeast", resourceFile); } }
        public static string BeAtLeast { get { return ResourceRead.GetResourceValue("BeAtLeast", resourceFile); } }
        public static string PasswordOneLetter { get { return ResourceRead.GetResourceValue("PasswordOneLetter", resourceFile); } }
        public static string PasswordOneCapitalLetter { get { return ResourceRead.GetResourceValue("PasswordOneCapitalLetter", resourceFile); } }
        public static string PasswordOneNumber { get { return ResourceRead.GetResourceValue("PasswordOneNumber", resourceFile); } }
        public static string PasswordOneSpecialLetter { get { return ResourceRead.GetResourceValue("PasswordOneSpecialLetter", resourceFile); } }
        public static string PasswordEightCharacters { get { return ResourceRead.GetResourceValue("PasswordEightCharacters", resourceFile); } }
        public static string Copy { get { return ResourceRead.GetResourceValue("Copy", resourceFile); } }
        public static string UDFDetails { get { return ResourceRead.GetResourceValue("UDFDetails", resourceFile); } }

        public static string AccessTrackstockSite
        {
            get
            {
                return ResourceRead.GetResourceValue("AccessTrackstockSite", resourceFile);
            }
        }
    }
    public class ResUserMasterUDF
    {

        private static string resourceFile = "ResUserMasterUDF";

        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile);
            }
        }
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile);
            }
        }
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile);
            }
        }
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile);
            }
        }
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile);
            }
        }
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", resourceFile);
            }
        }
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", resourceFile);
            }
        }
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", resourceFile);
            }
        }
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", resourceFile);
            }
        }
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", resourceFile);
            }
        }
    }


    public class UserMasterAPIDTO
    {
        public long ID { get; set; }
        public long UserName { get; set; }

        public long EnterpriseID { get; set; }

        public int ModuleID { get; set; }
        public int ModuleName { get; set; }

        public long CompanyID { get; set; }

        public long RoomID { get; set; }
    }

    public class RPT_UserMasterDTO
    {

        public string UserName { get; set; }
        public string UserType { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string CostCenter { get; set; }
        public string CustomerNumber { get; set; }
        public string Email { get; set; }
        public Int64 UserID { get; set; }
        public Int64 CompanyID { get; set; }
        public Int64 RoomID { get; set; }
        public Int64 EnterpriseID { get; set; }
        public Guid? UserGUID { get; set; }
    }

    public class UserListEnableNLF
    {
        public int[] SelectedIds { get; set; }
    }

    public class DefaultRedirectURLs
    {
        public Int32 ID { get; set; }
        public string SapphireURLs { get; set; }
        public string TrackstockURLs { get; set; }
    }
}

