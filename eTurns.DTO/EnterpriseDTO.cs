using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace eTurns.DTO
{
    [Serializable]
    public class EnterpriseDTO
    {
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public System.Int64 ID { get; set; }
        public Guid GUID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Name", ResourceType = typeof(ResEnterprise))]
        public System.String Name { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResCommon))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Address { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResCommon))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String City { get; set; }

        [Display(Name = "State", ResourceType = typeof(ResCommon))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String State { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(ResCommon))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String PostalCode { get; set; }

        [Display(Name = "Country", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Country { get; set; }

        [Display(Name = "ContactPhone", ResourceType = typeof(ResEnterprise))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ContactPhone { get; set; }


        [RegularExpression(@"[a-zA-Z0-9.!#$%&'*+-/=?\^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+", ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.EmailAddress)]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ContactEmail", ResourceType = typeof(ResEnterprise))]
        public System.String ContactEmail { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }


        [Display(Name = "UDF1", ResourceType = typeof(ResEnterprise))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResEnterprise))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResEnterprise))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResEnterprise))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResEnterprise))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResEnterprise))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResEnterprise))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResEnterprise))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResEnterprise))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResEnterprise))]
        public string UDF10 { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime Created { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }


        public Boolean IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public string EnterpriseDBName { get; set; }

        [RegularExpression(@"[a-zA-Z0-9.!#$%&'*+-/=?\^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+", ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.EmailAddress)]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "EnterpriseUserEmail", ResourceType = typeof(ResEnterprise))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string EnterpriseUserEmail { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "UserName", ResourceType = typeof(ResEnterprise))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$", ErrorMessageResourceName = "errPasswordRuleBreak", ErrorMessageResourceType = typeof(ResUserMaster))]
        [StringLength(20, ErrorMessageResourceName = "PasswordLengthMessage", MinimumLength = 6, ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "EnterpriseUserPassword", ResourceType = typeof(ResEnterprise))]
        public string EnterpriseUserPassword { get; set; }
        //public string EnterpriseDBConnectionString { get; set; }
        public string EnterpriseSqlServerName { get; set; }
        public string EnterpriseSqlServerUserName { get; set; }
        public string EnterpriseSqlServerPassword { get; set; }
        [Display(Name = "EnterpriseLogo", ResourceType = typeof(ResEnterprise))]
        public string EnterpriseLogo { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResEnterprise))]
        public bool IsActive { get; set; }

        public long EnterpriseUserID { get; set; }
        public List<ResourceLanguageDTO> lstResourceAll { get; set; }
        public string[] strResourceSelected { get; set; }
        public string IsPasswordChanged { get; set; }
        public string IsEmailChanged { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate) && HttpContext.Current != null)
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true, true);// Created.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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
                if (string.IsNullOrEmpty(_updatedDate) && Updated != null && HttpContext.Current != null)
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true, true);// Updated.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }
        public Nullable<Int64> EnterpriseSuperAdmin { get; set; }

        public List<EnterpriseSuperAdmin> lstSuperAdmins { get; set; }

        public string EnterpriseSuperAdmins { get; set; }
        public string EnterpriseSuperAdminDirty { get; set; }

        public string EnterPriseDomainURL { get; set; }
        public string ArchiveDbName { get; set; }
        public string ChangeLogDbName { get; set; }
        public string HistoryDBName { get; set; }
        public int? TotalRecords { get; set; }
        public System.DateTime HistoryDate { get; set; }

        [Display(Name = "AllowABIntegration", ResourceType = typeof(ResRoomMaster))]
        public bool AllowABIntegration { get; set; }
        public bool IsCreatedFromNLF { get; set; }
    }
    [Serializable]
    public class RoleEnterpriseDTO
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class EnterpriseSuperAdmin
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long RoleID { get; set; }
        public int UserType { get; set; }
        public string UserTypeName { get; set; }
        public string RoleName { get; set; }
        public bool IsDefault { get; set; }
        public bool MarkDeleted { get; set; }
        public bool IsEPSuperAdmin { get; set; }
    }

    public class ResEnterprise
    {

        private static string resourceFile = "ResEnterprise";

        public static string EnterpriseLogo
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("EnterpriseLogo", resourceFile);
            }
        }
        public static string EnterpriseUserEmail
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("EnterpriseUserEmail", resourceFile);
            }
        }
        public static string hdrEnterpriseUserDetails
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("hdrEnterpriseUserDetails", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Contact Email.
        /// </summary>
        public static string ContactEmail
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ContactEmail", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Contact Phone.
        /// </summary>
        public static string ContactPhone
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ContactPhone", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cost/Bin.
        /// </summary>
        public static string CostBin
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("CostBin", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Discount Price1.
        /// </summary>
        public static string DiscountPrice1
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("DiscountPrice1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Discount Price2.
        /// </summary>
        public static string DiscountPrice2
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("DiscountPrice2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Discount Price3.
        /// </summary>
        public static string DiscountPrice3
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("DiscountPrice3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enterprise name.
        /// </summary>
        public static string EnterpriseName
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("EnterpriseName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include License Fees.
        /// </summary>
        public static string IncludeLicenseFees
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("IncludeLicenseFees", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaxBinsPerBasePrice.
        /// </summary>
        public static string MaxBinsPerBasePrice
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxBinsPerBasePrice", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 1.
        /// </summary>
        public static string MaxLicenseTier1
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxLicenseTier1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 2.
        /// </summary>
        public static string MaxLicenseTier2
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxLicenseTier2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 3.
        /// </summary>
        public static string MaxLicenseTier3
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxLicenseTier3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 4.
        /// </summary>
        public static string MaxLicenseTier4
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxLicenseTier4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 5.
        /// </summary>
        public static string MaxLicenseTier5
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxLicenseTier5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 6.
        /// </summary>
        public static string MaxLicenseTier6
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxLicenseTier6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 1.
        /// </summary>
        public static string MaxSubscriptionTier1
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxSubscriptionTier1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 2.
        /// </summary>
        public static string MaxSubscriptionTier2
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxSubscriptionTier2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 3.
        /// </summary>
        public static string MaxSubscriptionTier3
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxSubscriptionTier3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 4.
        /// </summary>
        public static string MaxSubscriptionTier4
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxSubscriptionTier4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 5.
        /// </summary>
        public static string MaxSubscriptionTier5
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxSubscriptionTier5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 6.
        /// </summary>
        public static string MaxSubscriptionTier6
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MaxSubscriptionTier6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enterprises.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Enterprises.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 1.
        /// </summary>
        public static string PriceLicenseTier1
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceLicenseTier1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 2.
        /// </summary>
        public static string PriceLicenseTier2
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceLicenseTier2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 3.
        /// </summary>
        public static string PriceLicenseTier3
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceLicenseTier3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 4.
        /// </summary>
        public static string PriceLicenseTier4
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceLicenseTier4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 5.
        /// </summary>
        public static string PriceLicenseTier5
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceLicenseTier5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 6.
        /// </summary>
        public static string PriceLicenseTier6
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceLicenseTier6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 1.
        /// </summary>
        public static string PriceSubscriptionTier1
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceSubscriptionTier1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 2.
        /// </summary>
        public static string PriceSubscriptionTier2
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceSubscriptionTier2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 3.
        /// </summary>
        public static string PriceSubscriptionTier3
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceSubscriptionTier3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 4.
        /// </summary>
        public static string PriceSubscriptionTier4
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceSubscriptionTier4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 5.
        /// </summary>
        public static string PriceSubscriptionTier5
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceSubscriptionTier5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 6.
        /// </summary>
        public static string PriceSubscriptionTier6
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PriceSubscriptionTier6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Software Base Price.
        /// </summary>
        public static string SoftwareBasePrice
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("SoftwareBasePrice", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF10", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF7", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF8", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UDF9", resourceFile);
            }
        }

        public static object EnterpriseLanguages
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("EnterpriseLanguages", resourceFile);
            }
        }
        public static object ReqSingleRecordtoViewUDF
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ReqSingleRecordtoViewUDF", resourceFile);
            }
        }
        public static object msgSelectForViewHistory
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("msgSelectForViewHistory", resourceFile);
            }
        }
        public static object MsgProvideCorrectEnterpriseName
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgProvideCorrectEnterpriseName", resourceFile);
            }
        }
        public static object MsgProvideCorrectData
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgProvideCorrectData", resourceFile);
            }
        }
        public static string Name
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("Name", resourceFile);
            }
        }
        public static string IsActive
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("IsActive", resourceFile);
            }
        }
        public static string UserName
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UserName", resourceFile);
            }
        }
        public static string EnterpriseUserPassword
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("EnterpriseUserPassword", resourceFile);
            }
        }
        public static string TitlePasswordRequirements { get { return ResourceRead.GetEnterPriseResourceValue("TitlePasswordRequirements", resourceFile); } }
        public static string AtLeast { get { return ResourceRead.GetEnterPriseResourceValue("AtLeast", resourceFile); } }
        public static string BeAtLeast { get { return ResourceRead.GetEnterPriseResourceValue("BeAtLeast", resourceFile); } }
        public static string PasswordOneLetter { get { return ResourceRead.GetEnterPriseResourceValue("PasswordOneLetter", resourceFile); } }
        public static string PasswordOneCapitalLetter { get { return ResourceRead.GetEnterPriseResourceValue("PasswordOneCapitalLetter", resourceFile); } }
        public static string PasswordOneNumber { get { return ResourceRead.GetEnterPriseResourceValue("PasswordOneNumber", resourceFile); } }
        public static string PasswordOneSpecialLetter { get { return ResourceRead.GetEnterPriseResourceValue("PasswordOneSpecialLetter", resourceFile); } }
        public static string PasswordEightCharacters { get { return ResourceRead.GetEnterPriseResourceValue("PasswordEightCharacters", resourceFile); } }
        public static string TitleLoginUserDetails { get { return ResourceRead.GetEnterPriseResourceValue("TitleLoginUserDetails", resourceFile); } }
        public static string ABIntegrationOffConfirmationMsg { get { return ResourceRead.GetEnterPriseResourceValue("ABIntegrationOffConfirmationMsg", resourceFile); } }

    }

    public class ResLoginForms
    {

        private static string resourceFile = "ResLoginForms";

        public static string errPasswordRequired
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("resPasswordRequered", resourceFile);
            }
        }

        public static string errPasswordmismatch
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("errPasswordmismatch", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Confirm Password.
        /// </summary>
        public static string errPasswordRuleBreak
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("errPasswordRuleBreak", resourceFile);
            }
        }


        public static string lblPasswordRules
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("lblPasswordRules", resourceFile);
            }
        }
        public static string PageHeaderForgotPassword
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PageHeaderForgotPassword", resourceFile);
            }
        }
        public static string PageHeaderForgotPasswordTop
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PageHeaderForgotPasswordTop", resourceFile);
            }
        }
        public static string PageTitleForgotPassword
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PageTitleForgotPassword", resourceFile);
            }
        }

        public static string ForgotPasswordUserName
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ForgotPasswordUserName", resourceFile);
            }
        }
        public static string ForgotPasswordMessage
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ForgotPasswordMessage", resourceFile);
            }
        }
        public static string ForgotPasswordInvalidUserMessage
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ForgotPasswordInvalidUserMessage", resourceFile);
            }
        }

        public static string UserName
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("UserName", resourceFile);
            }
        }

        public static string CaptchaImage
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("CaptchaImage", resourceFile);
            }
        }

        public static string RefressCaptchaImage
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("RefressCaptchaImage", resourceFile);
            }
        }

        public static string IncorrectCapcha
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("IncorrectCapcha", resourceFile);
            }
        }

        public static string ResetPassMainHeader
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ResetPassMainHeader", resourceFile);
            }
        }

        public static string ResetPassHeader
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ResetPassHeader", resourceFile);
            }
        }
        public static string ResetPassTitle
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ResetPassTitle", resourceFile);
            }
        }

        public static string lnkNotFoundText
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("lnkNotFoundText", resourceFile);
            }
        }

        public static string LinkExpired
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("LinkExpired", resourceFile);
            }
        }
        public static string Password
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("Password", resourceFile);
            }
        }
        public static string PasswordLengthMessage
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PasswordLengthMessage", resourceFile);
            }
        }
        public static string ConfirmPassword
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("ConfirmPassword", resourceFile);
            }
        }


        public static string PasswordResetSuccess
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("PasswordResetSuccess", resourceFile);
            }
        }
        public static string repeatpwd
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("repeatpwd", resourceFile);
            }
        }

        public static string NotAllowedToAccess
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("NotAllowedToAccess", resourceFile);
            }
        }
		public static string MsgInvalidLink
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgInvalidLink", resourceFile);
            }
        }

        public static string MsgLinkExpiredGenerateNew
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgLinkExpiredGenerateNew", resourceFile);
            }
        }
        public static string MsgLinkExpired
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgLinkExpired", resourceFile);
            }
        }
        public static string MSgLinkUsed
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MSgLinkUsed", resourceFile);
            }
        }
        public static string MsgRepeatPassword
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgRepeatPassword", resourceFile);
            }
        }
        public static string MsgPasswordResetSuccessfully
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgPasswordResetSuccessfully", resourceFile);
            }
        }

        public static string MsgInvalidCaptcha
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgInvalidCaptcha", resourceFile);
            }
        }
        public static string MsgResetPwdMailBody
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgResetPwdMailBody", resourceFile);
            }
        }
        public static string MsgResetPwdMailSubject
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgResetPwdMailSubject", resourceFile);
            }
        }
        public static string MsgServerError
        {
            get
            {
                return ResourceRead.GetEnterPriseResourceValue("MsgServerError", resourceFile);
            }
        }
    }

    public class EntpCompanyRoom
    {
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public string CompanyIDs { get; set; }
        public long RoomID { get; set; }
        public string RoomIDs { get; set; }
    }
}


