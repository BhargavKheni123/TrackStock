using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    //22-Nov-2012

    /// <summary>
    /// UDFDTO
    /// </summary>
    public class UDFDTO
    {
        //Config PK
        public Int64 ID { get; set; }
        public Int64 EnterpriseId { get; set; }
        //Company ID
        public Int64 CompanyID { get; set; }

        //Company ID
        public Nullable<Int64> Room { get; set; }

        //UDF TableName   
        [Display(Name = "TableName", ResourceType = typeof(ResUDFSetup))]
        public string UDFTableName { get; set; }

        //UDF ColumnName
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage)), Display(Name = "ColumnName", ResourceType = typeof(ResUDFSetup))]
        public string UDFColumnName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage)), Display(Name = "UDFPDAColumnName", ResourceType = typeof(ResUDFSetup))]
        public string UDFPDAColumnName { get; set; }


        //UDF ControlType
        [Display(Name = "ControlType", ResourceType = typeof(ResUDFSetup))]
        public string UDFControlType { get; set; }

        //UDF Options - This will be used for dropdown choices        
        [Display(Name = "DropdownOptions", ResourceType = typeof(ResUDFSetup))]
        public string UDFOptionsCSV { get; set; }

        //UDF DefaultValues
        [Display(Name = "DefaultTextboxValue", ResourceType = typeof(ResUDFSetup))]
        public string UDFDefaultValue { get; set; }

        //UDF Required - Constraint
        [Display(Name = "IsRequired", ResourceType = typeof(ResUDFSetup))]
        public Nullable<Boolean> UDFIsRequired { get; set; }

        //UDF Searchable
        [Display(Name = "IsInNarrowSearch", ResourceType = typeof(ResUDFSetup))]
        public Nullable<Boolean> UDFIsSearchable { get; set; }


        /* COMMON MODEL FIELDS */

        //Created On Date
        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public DateTime Created { get; set; }

        //Update On Date
        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        //Created By ID
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        //Created By ID
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        //Created By Name
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        //Updated By Name
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        //GUID
        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        //IsArchived
        [Display(Name = "IsArchived", ResourceType = typeof(ResCommon))]
        public bool IsArchived { get; set; }

        //IsDeleted
        [Display(Name = "IsDeleted", ResourceType = typeof(ResCommon))]
        public bool IsDeleted { get; set; }



        //UDF ColumnName

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage)), Display(Name = "UDF")]
        public string UDFDisplayColumnName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage)), Display(Name = "UDFPDADisplayColumnName")]
        public string UDFPDADisplayColumnName { get; set; }

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
        public string ValueString { get; set; }
        public bool showPDAField { get; set; }
        public bool? OtherFromeTurns { get; set; }
        public bool? SetUpForEnterpriseLevel { get; set; }
        [Display(Name = "IsEncryption", ResourceType = typeof(ResUDFSetup))]
        public bool? IsEncryption { get; set; }

        public bool ShowEncryptionCheckBox { get; set; }
        public bool? IsQuickListUDF { get; set; }

        [Range(0,255, ErrorMessageResourceName = "MsgUDFMaxLengthRange", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "UDFMaxLength", ResourceType = typeof(ResUDFSetup))]
        public int? UDFMaxLength { get; set; }

        public string CurrentCult { get; set; }

        public int? TotalRecords { get; set; }
    }

    /// <summary>
    /// UDFChoicesDTO
    /// </summary>
    public class UDFOptionsDTO
    {
        //Config PK
        public Int64 ID { get; set; }
        public bool? IsEncryption { get; set; }
        //Company ID
        public Int64 UDFID { get; set; }

        //Company ID
        public Int64 CompanyID { get; set; }

        //UDF TableName
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage)), Display(Name = "UDFOption", ResourceType = typeof(ResUDFSetup))]
        public string UDFOption { get; set; }


        /* COMMON MODEL FIELDS */

        //Created On Date
        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        //Update On Date
        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        //Created By ID
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        //Created By ID
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        //Created By Name
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        //Updated By Name
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        //GUID
        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        //IsDeleted
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }

        public Int64? Room { get; set; }

    }

    public class ResUDFSetup
    {

        private static string resourceFile = "ResUDFSetup";


        /// <summary>
        ///   Looks up a localized string similar to ColumnName.
        /// </summary>
        public static string ColumnName
        {
            get
            {
                return ResourceRead.GetResourceValue("ColumnName", resourceFile);
            }
        }
        public static string IsEncryption
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEncryption", resourceFile);
            }
        }

        public static string UDFPDAColumnName
        {
            get
            {
                return ResourceRead.GetResourceValue("UDFPDAColumnName", resourceFile);
            }
        }


        public static string ColumnDisplayName
        {
            get
            {
                return ResourceRead.GetResourceValue("ColumnDisplayName", resourceFile);
            }
        }

        public static string ColumnDisplayNamePDA
        {
            get
            {
                return ResourceRead.GetResourceValue("ColumnDisplayNamePDA", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Control Type.
        /// </summary>
        public static string ControlType
        {
            get
            {
                return ResourceRead.GetResourceValue("ControlType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Default Textbox Value.
        /// </summary>
        public static string DefaultTextboxValue
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultTextboxValue", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Default Value.
        /// </summary>
        public static string DefaultValue
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultValue", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dropdown Options.
        /// </summary>
        public static string DropdownOptions
        {
            get
            {
                return ResourceRead.GetResourceValue("DropdownOptions", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include in Narrow Search.
        /// </summary>
        public static string IsInNarrowSearch
        {
            get
            {
                return ResourceRead.GetResourceValue("IsInNarrowSearch", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Required.
        /// </summary>
        public static string IsRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("IsRequired", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Location.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Location.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TableName.
        /// </summary>
        public static string TableName
        {
            get
            {
                return ResourceRead.GetResourceValue("TableName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TableName.
        /// </summary>
        public static string UDFOption
        {
            get
            {
                return ResourceRead.GetResourceValue("UDFOption", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to TableName.
        /// </summary>
        public static string EnterHere
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterHere", resourceFile);
            }
        }

        public static string UDFMaxLength
        {
            get
            {
                return ResourceRead.GetResourceValue("UDFMaxLength", resourceFile);
            }
        }


        public static string MaxLengthMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxLengthMessage", resourceFile);
            }
        }
        public static string InvalidUDFColumnName
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidUDFColumnName", resourceFile);
            }
        }
        public static string InvalidUDFTableName
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidUDFTableName", resourceFile);
            }
        }

        public static string MsgUDFOptionTryAnother
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUDFOptionTryAnother", resourceFile);
            }
        }

        public static string MsgUDFOptionSaved
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUDFOptionSaved", resourceFile);
            }
        }
        public static string MsgUDFOptionCanNotBlank
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUDFOptionCanNotBlank", resourceFile);
            }
        }
        public static string MsgUDFSaveFirst
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUDFSaveFirst", resourceFile);
            }
        }
        public static string UDFOptionValueLengthCantMoreThanUDFMaxLength { get { return ResourceRead.GetResourceValue("UDFOptionValueLengthCantMoreThanUDFMaxLength", resourceFile); } }
        public static string UDFColumnNameRequired { get { return ResourceRead.GetResourceValue("UDFColumnNameRequired", resourceFile); } }
        public static string UDFDisplayColumnNameRequired { get { return ResourceRead.GetResourceValue("UDFDisplayColumnNameRequired", resourceFile); } }
        public static string UDFPDADisplayColumnNameRequired { get { return ResourceRead.GetResourceValue("UDFPDADisplayColumnNameRequired", resourceFile); } }
        public static string KeyPairRemoved { get { return ResourceRead.GetResourceValue("KeyPairRemoved", resourceFile); } }
        public static string NoEntryFoundForKey { get { return ResourceRead.GetResourceValue("NoEntryFoundForKey", resourceFile); } }        
        public static string ReqValueBeforeSave
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqValueBeforeSave", resourceFile);
            }
        }
        public static string ReqDropDonwOption
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqDropDonwOption", resourceFile);
            }
        }
        public static string AllowedMaxCharacterForUDF { get { return ResourceRead.GetResourceValue("AllowedMaxCharacterForUDF", resourceFile); } }
        public static string MsgUDFRoomSet { get { return ResourceRead.GetResourceValue("MsgUDFRoomSet", resourceFile); } }
        public static string MsgCorrectModuleName { get { return ResourceRead.GetResourceValue("MsgCorrectModuleName", resourceFile); } }
        public static string MsgProvideUDFTableName { get { return ResourceRead.GetResourceValue("MsgProvideUDFTableName", resourceFile); } }
        public static string InsertUDFOptionRights { get { return ResourceRead.GetResourceValue("InsertUDFOptionRights", resourceFile); } }
        public static string MsgUDFDeleted { get { return ResourceRead.GetResourceValue("MsgUDFDeleted", resourceFile); } }
        public static string MsgDropDownEditableValidation { get { return ResourceRead.GetResourceValue("MsgDropDownEditableValidation", resourceFile); } }

        public static string ErrorUpdatingUDF { get { return ResourceRead.GetResourceValue("ErrorUpdatingUDF", resourceFile); } }
        public static string UDFOptionAddedSuccess { get { return ResourceRead.GetResourceValue("UDFOptionAddedSuccess", resourceFile); } }
        public static string UDFOptionAlreadyExists { get { return ResourceRead.GetResourceValue("UDFOptionAlreadyExists", resourceFile); } }
        public static string NewUDFOptionCreated { get { return ResourceRead.GetResourceValue("NewUDFOptionCreated", resourceFile); } }
        public static string PageTitleUDFSetUp { get { return ResourceRead.GetResourceValue("PageTitleUDFSetUp", resourceFile); } }
        public static string PageHeaderUDFSetUp { get { return ResourceRead.GetResourceValue("PageHeaderUDFSetUp", resourceFile); } }
        public static string PageHeaderUDFSetting { get { return ResourceRead.GetResourceValue("PageHeaderUDFSetting", resourceFile); } }
        public static string IsDefault { get { return ResourceRead.GetResourceValue("IsDefault", resourceFile); } }
        public static string BackToPage { get { return ResourceRead.GetResourceValue("BackToPage", resourceFile); } }


    }
    public class UDFOptionsCheckDTO
    {
        public Int64 UDFID { get; set; }
        public string UDFOption { get; set; }
        public string UDFColumnName { get; set; }
    }
    public class ExportUDFDTO
    {
        public string ModuleName { get; set; }
        public string UDFColumnName { get; set; }
        public string UDFName { get; set; }
        public string ControlType { get; set; }
        public string UDFDefaultValue { get; set; }
        public string OptionName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsDeleted { get; set; }
        public bool IncludeInNarrowSearch { get; set; }
        public Nullable<bool> IsEncryption { get; set; }

        public Nullable<int> UDFMaxLength { get; set; }
    }

    public class UDFPrefixResult
    {
        public Int32 Id { get; set; }
        public string ReportResourceFile { get; set; }
        public string Prefix { get; set; }
    }

    public class UDFBulkItem
    {
        public long UDFID { get; set; }
        public string UDFOption { get; set; }
        public string UDFColumnName { get; set; }
    }
}
