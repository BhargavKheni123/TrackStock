using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
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

        [Display(Name = "SoftwareBasePrice", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> SoftwareBasePrice { get; set; }

        [Display(Name = "MaxBinsPerBasePrice", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> MaxBinsPerBasePrice { get; set; }

        [Display(Name = "CostBin", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> CostPerBin { get; set; }

        [Display(Name = "DiscountPrice1", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> DiscountPrice1 { get; set; }

        [Display(Name = "DiscountPrice2", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> DiscountPrice2 { get; set; }

        [Display(Name = "DiscountPrice3", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> DiscountPrice3 { get; set; }

        [Display(Name = "MaxSubscriptionTier1", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxSubscriptionTier1 { get; set; }

        [Display(Name = "MaxSubscriptionTier2", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxSubscriptionTier2 { get; set; }

        [Display(Name = "MaxSubscriptionTier3", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxSubscriptionTier3 { get; set; }

        [Display(Name = "MaxSubscriptionTier4", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxSubscriptionTier4 { get; set; }

        [Display(Name = "MaxSubscriptionTier5", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxSubscriptionTier5 { get; set; }

        [Display(Name = "MaxSubscriptionTier6", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxSubscriptionTier6 { get; set; }

        [Display(Name = "PriceSubscriptionTier1", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceSubscriptionTier1 { get; set; }

        [Display(Name = "PriceSubscriptionTier2", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceSubscriptionTier2 { get; set; }

        [Display(Name = "PriceSubscriptionTier3", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceSubscriptionTier3 { get; set; }

        [Display(Name = "PriceSubscriptionTier4", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceSubscriptionTier4 { get; set; }

        [Display(Name = "PriceSubscriptionTier5", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceSubscriptionTier5 { get; set; }

        [Display(Name = "PriceSubscriptionTier6", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceSubscriptionTier6 { get; set; }

        [Display(Name = "IncludeLicenseFees", ResourceType = typeof(ResEnterprise))]
        public bool IncludeLicenseFees { get; set; }

        [Display(Name = "MaxLicenseTier1", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxLicenseTier1 { get; set; }

        [Display(Name = "MaxLicenseTier2", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxLicenseTier2 { get; set; }

        [Display(Name = "MaxLicenseTier3", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxLicenseTier3 { get; set; }

        [Display(Name = "MaxLicenseTier4", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxLicenseTier4 { get; set; }

        [Display(Name = "MaxLicenseTier5", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxLicenseTier5 { get; set; }

        [Display(Name = "MaxLicenseTier6", ResourceType = typeof(ResEnterprise))]
        [DataType(DataType.Text)]
        public Nullable<System.Int32> MaxLicenseTier6 { get; set; }

        [Display(Name = "PriceLicenseTier1", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceLicenseTier1 { get; set; }

        [Display(Name = "PriceLicenseTier2", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceLicenseTier2 { get; set; }

        [Display(Name = "PriceLicenseTier3", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceLicenseTier3 { get; set; }

        [Display(Name = "PriceLicenseTier4", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceLicenseTier4 { get; set; }

        [Display(Name = "PriceLicenseTier5", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceLicenseTier5 { get; set; }

        [Display(Name = "PriceLicenseTier6", ResourceType = typeof(ResEnterprise))]
        public Nullable<System.Double> PriceLicenseTier6 { get; set; }


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
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$", ErrorMessageResourceName = "errPasswordRuleBreak", ErrorMessageResourceType = typeof(ResUserMaster))]
        [StringLength(20, ErrorMessageResourceName = "PasswordLengthMessage", MinimumLength = 6, ErrorMessageResourceType = typeof(ResMessage))]

        public string EnterpriseUserPassword { get; set; }
        public string EnterpriseDBConnectionString { get; set; }
        public string EnterpriseSqlServerName { get; set; }
        public string EnterpriseSqlServerUserName { get; set; }
        public string EnterpriseSqlServerPassword { get; set; }
        public string EnterpriseLogo { get; set; }
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
                    _createdDate = Created.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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
                    _updatedDate = Updated.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }
        public Nullable<Int64> EnterpriseSuperAdmin { get; set; }
    }
    public class ResEnterprise
    {

        private static string resourceFile = "ResEnterprise";

        public static string EnterpriseLogo
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("EnterpriseLogo", resourceFile);
            }
        }
        public static string EnterpriseUserEmail
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("EnterpriseUserEmail", resourceFile);
            }
        }
        public static string hdrEnterpriseUserDetails
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("hdrEnterpriseUserDetails", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Contact Email.
        /// </summary>
        public static string ContactEmail
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ContactEmail", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Contact Phone.
        /// </summary>
        public static string ContactPhone
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ContactPhone", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cost/Bin.
        /// </summary>
        public static string CostBin
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("CostBin", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Discount Price1.
        /// </summary>
        public static string DiscountPrice1
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("DiscountPrice1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Discount Price2.
        /// </summary>
        public static string DiscountPrice2
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("DiscountPrice2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Discount Price3.
        /// </summary>
        public static string DiscountPrice3
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("DiscountPrice3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enterprise name.
        /// </summary>
        public static string EnterpriseName
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("EnterpriseName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include License Fees.
        /// </summary>
        public static string IncludeLicenseFees
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("IncludeLicenseFees", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaxBinsPerBasePrice.
        /// </summary>
        public static string MaxBinsPerBasePrice
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxBinsPerBasePrice", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 1.
        /// </summary>
        public static string MaxLicenseTier1
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxLicenseTier1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 2.
        /// </summary>
        public static string MaxLicenseTier2
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxLicenseTier2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 3.
        /// </summary>
        public static string MaxLicenseTier3
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxLicenseTier3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 4.
        /// </summary>
        public static string MaxLicenseTier4
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxLicenseTier4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 5.
        /// </summary>
        public static string MaxLicenseTier5
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxLicenseTier5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max License Tier 6.
        /// </summary>
        public static string MaxLicenseTier6
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxLicenseTier6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 1.
        /// </summary>
        public static string MaxSubscriptionTier1
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxSubscriptionTier1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 2.
        /// </summary>
        public static string MaxSubscriptionTier2
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxSubscriptionTier2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 3.
        /// </summary>
        public static string MaxSubscriptionTier3
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxSubscriptionTier3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 4.
        /// </summary>
        public static string MaxSubscriptionTier4
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxSubscriptionTier4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 5.
        /// </summary>
        public static string MaxSubscriptionTier5
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxSubscriptionTier5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Subscription Tier 6.
        /// </summary>
        public static string MaxSubscriptionTier6
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("MaxSubscriptionTier6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enterprises.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Enterprises.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 1.
        /// </summary>
        public static string PriceLicenseTier1
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceLicenseTier1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 2.
        /// </summary>
        public static string PriceLicenseTier2
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceLicenseTier2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 3.
        /// </summary>
        public static string PriceLicenseTier3
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceLicenseTier3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 4.
        /// </summary>
        public static string PriceLicenseTier4
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceLicenseTier4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 5.
        /// </summary>
        public static string PriceLicenseTier5
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceLicenseTier5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price License Tier 6.
        /// </summary>
        public static string PriceLicenseTier6
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceLicenseTier6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 1.
        /// </summary>
        public static string PriceSubscriptionTier1
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceSubscriptionTier1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 2.
        /// </summary>
        public static string PriceSubscriptionTier2
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceSubscriptionTier2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 3.
        /// </summary>
        public static string PriceSubscriptionTier3
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceSubscriptionTier3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 4.
        /// </summary>
        public static string PriceSubscriptionTier4
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceSubscriptionTier4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 5.
        /// </summary>
        public static string PriceSubscriptionTier5
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceSubscriptionTier5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Price Subscription Tier 6.
        /// </summary>
        public static string PriceSubscriptionTier6
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PriceSubscriptionTier6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Software Base Price.
        /// </summary>
        public static string SoftwareBasePrice
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("SoftwareBasePrice", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF10", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF7", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF8", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UDF9", resourceFile);
            }
        }

        public static object EnterpriseLanguages
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("EnterpriseLanguages", resourceFile);
            }
        }
    }

    public class ResLoginForms
    {

        private static string resourceFile = "ResLoginForms";

        public static string errPasswordRequired
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("resPasswordRequered", resourceFile);
            }
        }

        public static string errPasswordmismatch
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("errPasswordmismatch", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Confirm Password.
        /// </summary>
        public static string errPasswordRuleBreak
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("errPasswordRuleBreak", resourceFile);
            }
        }


        public static string lblPasswordRules
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("lblPasswordRules", resourceFile);
            }
        }
        public static string PageHeaderForgotPassword
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PageHeaderForgotPassword", resourceFile);
            }
        }

        public static string PageTitleForgotPassword
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PageTitleForgotPassword", resourceFile);
            }
        }

        public static string ForgotPasswordUserName
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ForgotPasswordUserName", resourceFile);
            }
        }
        public static string ForgotPasswordMessage
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ForgotPasswordMessage", resourceFile);
            }
        }
        public static string ForgotPasswordInvalidUserMessage
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ForgotPasswordInvalidUserMessage", resourceFile);
            }
        }

        public static string UserName
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("UserName", resourceFile);
            }
        }

        public static string CaptchaImage
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("CaptchaImage", resourceFile);
            }
        }

        public static string RefressCaptchaImage
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("RefressCaptchaImage", resourceFile);
            }
        }

        public static string IncorrectCapcha
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("IncorrectCapcha", resourceFile);
            }
        }

        public static string ResetPassMainHeader
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ResetPassMainHeader", resourceFile);
            }
        }

        public static string ResetPassHeader
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ResetPassHeader", resourceFile);
            }
        }
        public static string ResetPassTitle
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ResetPassTitle", resourceFile);
            }
        }

        public static string lnkNotFoundText
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("lnkNotFoundText", resourceFile);
            }
        }

        public static string LinkExpired
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("LinkExpired", resourceFile);
            }
        }
        public static string Password
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("Password", resourceFile);
            }
        }
        public static string PasswordLengthMessage
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PasswordLengthMessage", resourceFile);
            }
        }
        public static string ConfirmPassword
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("ConfirmPassword", resourceFile);
            }
        }
        

        public static string PasswordResetSuccess
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("PasswordResetSuccess", resourceFile);
            }
        }
        public static string repeatpwd
        {
            get
            {
                return ResourceHelper.GetEnterPriseResourceValue("repeatpwd", resourceFile);
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


