using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class SupplierAccountDetailsDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //SupplierID
        [Display(Name = "SupplierID", ResourceType = typeof(ResSupplierAccountDetails))]
        public Nullable<System.Int64> SupplierID { get; set; }

        //AccountNo
        [Display(Name = "AccountNo", ResourceType = typeof(ResSupplierAccountDetails))]
        [StringLength(64, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String AccountNo { get; set; }

        //IsDefault
        [Display(Name = "IsDefault", ResourceType = typeof(ResSupplierAccountDetails))]
        public Nullable<Boolean> IsDefault { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "Updated", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsDeleted
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsDeleted { get; set; }

        //IsArchived
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResSupplierAccountDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResSupplierAccountDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResSupplierAccountDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResSupplierAccountDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResSupplierAccountDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }
        public string AccountNumnerMerge { get; set; }
        public int SessionSr { get; set; }

        //AccountName
        [Display(Name = "AccountName", ResourceType = typeof(ResSupplierAccountDetails))]
        [StringLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String AccountName { get; set; }


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

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Country", ResourceType = typeof(ResCommon))]
        public string Country { get; set; }
        public Nullable<System.Int32> DefaultOrderRequiredDays { get; set; }

        public Nullable<long> ShipViaID { get; set; }

        [Display(Name = "ShipVia", ResourceType = typeof(ResShipVia))]
        public string ShipVia { get; set; }

        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ShipToID", ResourceType = typeof(ResSupplierAccountDetails))]
        public string ShipToID { get; set; }

        public bool IsViewable { get; set; }
        public bool IsInsertable { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeleteable { get; set; }

    }

    public class ResSupplierAccountDetails
    {
        private static string ResourceFileName = "ResSupplierAccountDetails";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceRead.GetResourceValue("Action", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SupplierAccountDetails {0} already exist! Try with Another!.
        /// </summary>
        public static string Duplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("Duplicate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to HistoryID.
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeDeleted", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceRead.GetResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SupplierAccountDetails.
        /// </summary>
        public static string SupplierAccountDetailsHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccountDetailsHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SupplierAccountDetails.
        /// </summary>
        public static string SupplierAccountDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccountDetails", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View History.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ViewHistory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SupplierID.
        /// </summary>
        public static string SupplierID
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AccountNo.
        /// </summary>
        public static string AccountNo
        {
            get
            {
                return ResourceRead.GetResourceValue("AccountNo", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AccountNo.
        /// </summary>
        public static string AccountName
        {
            get
            {
                return ResourceRead.GetResourceValue("AccountName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AccountNo.
        /// </summary>
        public static string AccountAddress
        {
            get
            {
                return ResourceRead.GetResourceValue("AccountAddress", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AccountNo.
        /// </summary>
        public static string AccountCity
        {
            get
            {
                return ResourceRead.GetResourceValue("AccountCity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AccountNo.
        /// </summary>
        public static string AccountState
        {
            get
            {
                return ResourceRead.GetResourceValue("AccountState", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AccountNo.
        /// </summary>
        public static string AccountZip
        {
            get
            {
                return ResourceRead.GetResourceValue("AccountZip", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AccountNo.
        /// </summary>
        public static string AccountIsDefault
        {
            get
            {
                return ResourceRead.GetResourceValue("AccountIsDefault", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to IsDefault.
        /// </summary>
        public static string IsDefault
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDefault", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created.
        /// </summary>
        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated.
        /// </summary>
        public static string Updated
        {
            get
            {
                return ResourceRead.GetResourceValue("Updated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdatedBy.
        /// </summary>
        public static string LastUpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDeleted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IsArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GUID.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
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

        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        public static string ShipToID
        {
            get
            {
                return ResourceRead.GetResourceValue("ShipToID", ResourceFileName);
            }
        }

        public static string SupplierAccountNameCantBlank { get { return ResourceRead.GetResourceValue("SupplierAccountNameCantBlank", ResourceFileName); } }
        public static string SupplierAccountNumberCantBlank { get { return ResourceRead.GetResourceValue("SupplierAccountNumberCantBlank", ResourceFileName); } }
        public static string NotAllowedToInsertSupplierAccountDetail { get { return ResourceRead.GetResourceValue("NotAllowedToInsertSupplierAccountDetail", ResourceFileName); } }
        public static string NotAllowedToUpdateSupplierAccountDetail { get { return ResourceRead.GetResourceValue("NotAllowedToUpdateSupplierAccountDetail", ResourceFileName); } }
        public static string AddAccount { get { return ResourceRead.GetResourceValue("AddAccount", ResourceFileName); } }
    }
}