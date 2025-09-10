using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class CustomerMasterDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Customer", ResourceType = typeof(ResCustomer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Customer { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Account", ResourceType = typeof(ResCustomer))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Account { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResCommon))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Address { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResCommon))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string City { get; set; }

        [Display(Name = "State", ResourceType = typeof(ResCommon))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string State { get; set; }

        [Display(Name = "Country", ResourceType = typeof(ResCommon))]
        [StringLength(64, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Country { get; set; }

        [Display(Name = "ZipCode", ResourceType = typeof(ResCommon))]
        [StringLength(24, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "NumberOnly", ErrorMessageResourceType = typeof(ResMessage))]
        public string ZipCode { get; set; }

        [Display(Name = "Contact", ResourceType = typeof(ResCustomer))]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Contact { get; set; }

        [RegularExpression(@"[a-zA-Z0-9.!#$%&'*+-/=?\^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+", ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ResMessage))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(ResCommon))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Email { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(ResCommon))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Phone { get; set; }

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

        [Display(Name = "IsArchived", ResourceType = typeof(ResCustomer))]
        public Nullable<bool> IsArchived { get; set; }

        [Display(Name = "IsDeleted", ResourceType = typeof(ResCustomer))]
        public Nullable<bool> IsDeleted { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64? HistoryID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResCustomer))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResCustomer))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResCustomer))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResCustomer))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResCustomer))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResCustomer))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResCustomer))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResCustomer))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResCustomer))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResCustomer))]
        public string UDF10 { get; set; }

        [Display(Name = "Remarks", ResourceType = typeof(ResCustomer))]
        [StringLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Remarks { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }

        public bool IsOnlyFromItemUI { get; set; }
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
        public string ReceivedOnDateWeb
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

        public int? TotalRecords { get; set; }
    }

    public class ResCustomer
    {
        private static string resourceFile = "ResCustomer";

        /// <summary>
        ///   Looks up a localized string similar to Account.
        /// </summary>
        public static string Account
        {
            get
            {
                return ResourceRead.GetResourceValue("Account", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Customer.
        /// </summary>
        public static string Customer
        {
            get
            {
                return ResourceRead.GetResourceValue("Customer", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Customers.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Customers.
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
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string Remarks
        {
            get
            {
                return ResourceRead.GetResourceValue("Remarks", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max length 255.
        /// </summary>
        public static string ZipCodeMaxLength
        {
            get
            {
                return ResourceRead.GetResourceValue("ZipCodeMaxLength", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Contact.
        /// </summary>
        public static string Contact
        {
            get
            {
                return ResourceRead.GetResourceValue("Contact", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IsArchived", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDeleted", resourceFile);
            }
        }


    }
}
