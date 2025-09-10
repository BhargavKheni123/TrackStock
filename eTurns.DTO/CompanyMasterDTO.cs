using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace eTurns.DTO
{
    [Serializable]
    public class CompanyMasterDTO
    {
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public System.Int64 ID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CompanyName", ResourceType = typeof(ResCompany))]
        [StringLength(255)]
        public System.String Name { get; set; }

        [Display(Name = "CompanyNumber", ResourceType = typeof(ResCompany))]
        [StringLength(255)]
        public System.String CompanyNumber { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResCommon))]
        [StringLength(1024)]
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
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Country { get; set; }

        [Display(Name = "ContactPhone", ResourceType = typeof(ResCompany))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ContactPhone { get; set; }

        [Display(Name = "ContactEmail", ResourceType = typeof(ResCompany))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ContactEmail { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public Nullable<Boolean> IsDeleted { get; set; }

        public Nullable<bool> IsArchived { get; set; }

        public Guid GUID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResCompany))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResCompany))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResCompany))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResCompany))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResCompany))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResCompany))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResCompany))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResCompany))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResCompany))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResCompany))]
        public string UDF10 { get; set; }

        public long EnterPriseId { get; set; }
        public string EnterPriseName { get; set; }

        [Display(Name = "CompanyLogo", ResourceType = typeof(ResCompany))]
        public string CompanyLogo { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResCompany))]
        public bool IsActive { get; set; }

        //public CompanyConfigDTO objCompanyConfig { get; set; }
        public bool IsStatusChanged { get; set; }
        [Display(Name = "IncludeCommonBOM", ResourceType = typeof(ResCompany))]
        public bool IsIncludeCommonBOM { get; set; }

        private string _createdDate;

        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate) && Created != null && HttpContext.Current != null)
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true, true);// Created.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public Int32 NoOfRooms { get; set; }
        public Int32 NoOfActiveRooms { get; set; }
        public int? TotalRecords { get; set; }
        public System.Int64 CompanyID { get; set; }
    }
    public class RoleCompanyMasterDTO
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long EnterPriseId { get; set; }
        public string EnterPriseName { get; set; }
    }
    public class ResCompany
    {
        private static string resourceFile = "ResCompany";

        /// <summary>
        ///   Looks up a localized string similar to Company name.
        /// </summary>
        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Company name.
        /// </summary>
        public static string CompanyName
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyName", resourceFile);
            }
        }
        public static string CompanyNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyNumber", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Contact Email.
        /// </summary>
        public static string ContactEmail
        {
            get
            {
                return ResourceRead.GetResourceValue("ContactEmail", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Contact Phone.
        /// </summary>
        public static string ContactPhone
        {
            get
            {
                return ResourceRead.GetResourceValue("ContactPhone", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Companies.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Companies.
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

        /// <summary>
        ///   Looks up a localized string similar to Company logo.
        /// </summary>
        public static string CompanyLogo
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyLogo", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IncludeCommonBOM.
        /// </summary>
        public static string IncludeCommonBOM
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeCommonBOM", resourceFile);
            }
        }

        public static string Enterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("Enterprise", resourceFile);
            }
        }

        public static string NoOfRooms
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfRooms", resourceFile);
            }
        }

        public static string NoOfActiveRooms
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfActiveRooms", resourceFile);
            }
        }
		
		public static string MsgCompanyUndeleteSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCompanyUndeleteSuccessfully", resourceFile);
            }
        }

        public static string MsgCompanyExistForUndelete
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCompanyExistForUndelete", resourceFile);
            }
        }
        public static string MsgProvideCorrectCompany
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProvideCorrectCompany", resourceFile);
            }
        }
        public static string MsgCompanyNameRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCompanyNameRequired", resourceFile);
            }
        }
        public static string MsgCompanyNotExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCompanyNotExist", resourceFile);
            }
        }
        public static string MsgCompanyDoesNotExistinEnterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCompanyDoesNotExistinEnterprise", resourceFile);
            }
        }
    }
}


