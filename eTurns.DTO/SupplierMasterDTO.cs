using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class SupplierMasterDTO
    {
        public Int64 ID { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "SupplierName", ResourceType = typeof(ResSupplierMaster))]
        [AllowHtml]
        public string SupplierName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResSupplierMaster))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Description { get; set; }


        [StringLength(64, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ReceiverID", ResourceType = typeof(ResSupplierMaster))]
        public string ReceiverID { get; set; }

        [StringLength(1027, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Address", ResourceType = typeof(ResSupplierMaster))]
        public string Address { get; set; }

        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "City", ResourceType = typeof(ResSupplierMaster))]
        public string City { get; set; }

        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "State", ResourceType = typeof(ResSupplierMaster))]
        public string State { get; set; }

        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ZipCode", ResourceType = typeof(ResSupplierMaster))]
        //[RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = "NumberOnly", ErrorMessageResourceType = typeof(ResMessage))]
        public string ZipCode { get; set; }

        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Country", ResourceType = typeof(ResSupplierMaster))]
        public string Country { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Contact", ResourceType = typeof(ResSupplierMaster))]
        public string Contact { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Phone", ResourceType = typeof(ResSupplierMaster))]
        public string Phone { get; set; }

        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Fax", ResourceType = typeof(ResSupplierMaster))]
        public string Fax { get; set; }

        //[RegularExpression(@"[a-zA-Z0-9.!#$%&'*+-/=?\^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+", ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ResMessage))]
        //[DataType(DataType.EmailAddress)]
        //[Display(Name = "Email", ResourceType = typeof(ResCommon))]
        //[StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //public string Email { get; set; }
        [MultiEmails("Email", ErrorMessageResourceName = "MultiEmail", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Email", ResourceType = typeof(ResSupplierMaster))]
        public string Email { get; set; }
        [Display(Name = "EmailPOInBody", ResourceType = typeof(ResSupplierMaster))]
        public bool IsEmailPOInBody { get; set; }
        [Display(Name = "EmailPOInPDF", ResourceType = typeof(ResSupplierMaster))]
        public bool IsEmailPOInPDF { get; set; }
        [Display(Name = "EmailPOInCSV", ResourceType = typeof(ResSupplierMaster))]
        public bool IsEmailPOInCSV { get; set; }
        [Display(Name = "EmailPOInX12", ResourceType = typeof(ResSupplierMaster))]
        public bool IsEmailPOInX12 { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]

        public DateTime Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> LastUpdated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public Guid GUID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }
        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResSupplierMaster))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResSupplierMaster))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResSupplierMaster))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResSupplierMaster))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResSupplierMaster))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResSupplierMaster))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResSupplierMaster))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResSupplierMaster))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResSupplierMaster))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResSupplierMaster))]
        public string UDF10 { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "SupplierColor", ResourceType = typeof(ResSupplierMaster))]
        public string SupplierColor { get; set; }

        public Guid? ItemGUID { get; set; }

        //BranchNumber
        [Display(Name = "BranchNumber", ResourceType = typeof(ResSupplierMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String BranchNumber { get; set; }

        //MaximumOrderSize
        [Display(Name = "MaximumOrderSize", ResourceType = typeof(ResSupplierMaster))]
        [RegularExpression(@"\d+", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Decimal> MaximumOrderSize { get; set; }

        [Display(Name = "MaximumOrderSize", ResourceType = typeof(ResSupplierMaster))]
        [RegularExpression(@"\d+", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> MaxOrderSize { get; set; }

        [Display(Name = "DefaultOrderRequiredDays", ResourceType = typeof(ResSupplierMaster))]
        [RegularExpression(@"\d+", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> DefaultOrderRequiredDays { get; set; }

        //IsSendtoVendor
        [Display(Name = "IsSendtoVendor", ResourceType = typeof(ResSupplierMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsSendtoVendor { get; set; }

        //IsVendorReturnAsn
        [Display(Name = "IsVendorReturnAsn", ResourceType = typeof(ResSupplierMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsVendorReturnAsn { get; set; }

        //IsSupplierReceivesKitComponents
        [Display(Name = "IsSupplierReceivesKitComponents", ResourceType = typeof(ResSupplierMaster))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsSupplierReceivesKitComponents { get; set; }

        //POAutoSequence
        [Display(Name = "POAutoSequence", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> POAutoSequence { get; set; }

        //ScheduleType
        [Display(Name = "ScheduleType", ResourceType = typeof(ResSupplierMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ScheduleType { get; set; }

        //Days
        [Display(Name = "Days", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> Days { get; set; }

        //WeekDays
        [Display(Name = "WeekDays", ResourceType = typeof(ResSupplierMaster))]
        [StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String WeekDays { get; set; }

        //MonthDays
        [Display(Name = "MonthDays", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> MonthDays { get; set; }

        //ScheduleTime
        [Display(Name = "ScheduleTime", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.TimeSpan> ScheduleTime { get; set; }

        //IsAutoGenerate
        [Display(Name = "IsAutoGenerate", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<Boolean> IsAutoGenerate { get; set; }

        //IsAutoGenerateSubmit
        [Display(Name = "IsAutoGenerateSubmit", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<Boolean> IsAutoGenerateSubmit { get; set; }

        [Display(Name = "NextOrderNo", ResourceType = typeof(ResRoomMaster))]
        //[RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string NextOrderNo { get; set; }

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

        public int SupplierOrderScheduleMode { get; set; }
        public int SupplierOrderSubmissionMethod { get; set; }
        public DateTime? ScheduleRunTime { get; set; }

        public bool IsOrderScheduleActive { get; set; }
        public bool isForBOM { get; set; }
        public long? RefBomId { get; set; }

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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(LastUpdated, true);
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
        public List<SupplierAccountDetailsDTO> SupplierAccountDetails { get; set; }
        public List<SupplierBlanketPODetailsDTO> SupplierBlanketPODetails { get; set; }
        public int SupplierBlanketDirty { get; set; }
        public int SupplierAccountDirty { get; set; }

        //POAutoSequence
        [Display(Name = "PullPurchaseNumberType", ResourceType = typeof(ResSupplierMaster))]
        public int? PullPurchaseNumberType { get; set; }

        [Display(Name = "LastPullPurchaseNumberUsed", ResourceType = typeof(ResSupplierMaster))]
        public string LastPullPurchaseNumberUsed { get; set; }
        
        [Display(Name = "SupplierImage", ResourceType = typeof(ResSupplierMaster))]
        public string SupplierImage { get; set; }
        
        [Display(Name = "ImageExternalURL", ResourceType = typeof(ResSupplierMaster))]
        public string ImageExternalURL { get; set; }
        public string ImageType { get; set; }
        public Nullable<int> Count { get; set; }

        [Display(Name = "POAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string POAutoNrFixedValue { get; set; }

        [Display(Name = "PullPurchaseNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string PullPurchaseNrFixedValue { get; set; }

        public string AccountNo { get; set; }

        public int TotalRecords { get; set; }

        public string Ent_Com_Room_ID { get; set; }
        public Nullable<bool> IsSelected { get; set; }
        [Display(Name = "IsOrderReleaseNumberEditable", ResourceType = typeof(ResSupplierMaster))]
        public bool IsOrderReleaseNumberEditable { get; set; }

        [Display(Name = "POAutoNrReleaseNumber", ResourceType = typeof(ResSupplierMaster))]
        public string POAutoNrReleaseNumber { get; set; }

        
        [Display(Name = "QuoteAutoSequence", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> QuoteAutoSequence { get; set; }

        [Display(Name = "QuoteAutoNrFixedValue", ResourceType = typeof(ResSupplierMaster))]
        public string QuoteAutoNrFixedValue { get; set; }

        [Display(Name = "NextQuoteNo", ResourceType = typeof(ResSupplierMaster))]
        //[RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string NextQuoteNo { get; set; }

        [Display(Name = "QuoteAutoNrReleaseNumber", ResourceType = typeof(ResSupplierMaster))]
        public string QuoteAutoNrReleaseNumber { get; set; }
    }

    public class ResSupplierMaster
    {
        private static string resourceFile = "ResSupplierMaster";

        /// <summary>
        ///   Looks up a localized string similar to Account#.
        /// </summary>
        public static string Account
        {
            get
            {
                return ResourceRead.GetResourceValue("Account", resourceFile);
            }
        }
        public static string SubmissionMethod
        {
            get
            {
                return ResourceRead.GetResourceValue("SubmissionMethod", resourceFile);
            }
        }

        public static string NextRunDate
        {
            get
            {
                return ResourceRead.GetResourceValue("NextRunDate", resourceFile);
            }
        }

        public static string ScheduleMode
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleMode", resourceFile);
            }
        }

        public static string DailyRecurringType
        {
            get
            {
                return ResourceRead.GetResourceValue("DailyRecurringType", resourceFile);
            }
        }

        public static string DailyRecurringDays
        {
            get
            {
                return ResourceRead.GetResourceValue("DailyRecurringDays", resourceFile);
            }
        }

        public static string WeeklyRecurringWeeks
        {
            get
            {
                return ResourceRead.GetResourceValue("WeeklyRecurringWeeks", resourceFile);
            }
        }
        public static string HourlyRecurringHours
        {
            get
            {
                return ResourceRead.GetResourceValue("HourlyRecurringHours", resourceFile);
            }
        }

        public static string HourlyAtWhatMinute
        {
            get
            {
                return ResourceRead.GetResourceValue("HourlyAtWhatMinute", resourceFile);
            }
        }

        public static string MonthlyRecurringType
        {
            get
            {
                return ResourceRead.GetResourceValue("MonthlyRecurringType", resourceFile);
            }
        }

        public static string MonthlyDateOfMonth
        {
            get
            {
                return ResourceRead.GetResourceValue("MonthlyDateOfMonth", resourceFile);
            }
        }
        public static string MonthlyRecurringMonths
        {
            get
            {
                return ResourceRead.GetResourceValue("MonthlyRecurringMonths", resourceFile);
            }
        }
        public static string MonthlyDayOfMonth
        {
            get
            {
                return ResourceRead.GetResourceValue("MonthlyDayOfMonth", resourceFile);
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
        ///   Looks up a localized string similar to EmailPOInBody.
        /// </summary>
        public static string EmailPOInBody
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEmailPOInBody", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EmailPOInCSV.
        /// </summary>
        public static string EmailPOInCSV
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEmailPOInCSV", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EmailPOInPDF.
        /// </summary>
        public static string EmailPOInPDF
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEmailPOInPDF", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EmailPOInX12.
        /// </summary>
        public static string EmailPOInX12
        {
            get
            {
                return ResourceRead.GetResourceValue("IsEmailPOInX12", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Fax.
        /// </summary>
        public static string Fax
        {
            get
            {
                return ResourceRead.GetResourceValue("Fax", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Supplier.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Suppliers.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReceiverID.
        /// </summary>
        public static string ReceiverID
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiverID", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Supplier.
        /// </summary>
        public static string Supplier
        {
            get
            {
                return ResourceRead.GetResourceValue("Supplier", resourceFile);
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
        ///   Looks up a localized string similar to CategoryColor.
        /// </summary>
        public static string SupplierColor
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierColor", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to BranchNumber.
        /// </summary>
        public static string BranchNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BranchNumber", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaximumOrderSize.
        /// </summary>
        public static string MaximumOrderSize
        {
            get
            {
                return ResourceRead.GetResourceValue("MaximumOrderSize", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsSendtoVendor.
        /// </summary>
        public static string IsSendtoVendor
        {
            get
            {
                return ResourceRead.GetResourceValue("IsSendtoVendor", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsVendorReturnAsn.
        /// </summary>
        public static string IsVendorReturnAsn
        {
            get
            {
                return ResourceRead.GetResourceValue("IsVendorReturnAsn", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsSupplierReceivesKitComponents.
        /// </summary>
        public static string IsSupplierReceivesKitComponents
        {
            get
            {
                return ResourceRead.GetResourceValue("IsSupplierReceivesKitComponents", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to POAutoSequence.
        /// </summary>
        public static string POAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("POAutoSequence", resourceFile);
            }
        }

        public static string RequisitionAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("RequisitionAutoSequence", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ScheduleType.
        /// </summary>
        public static string ScheduleType
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Days.
        /// </summary>
        public static string Days
        {
            get
            {
                return ResourceRead.GetResourceValue("Days", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WeekDays.
        /// </summary>
        public static string WeekDays
        {
            get
            {
                return ResourceRead.GetResourceValue("WeekDays", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MonthDays.
        /// </summary>
        public static string MonthDays
        {
            get
            {
                return ResourceRead.GetResourceValue("MonthDays", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ScheduleTime.
        /// </summary>
        public static string ScheduleTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleTime", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsAutoGenerate.
        /// </summary>
        public static string IsAutoGenerate
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAutoGenerate", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsAutoGenerateSubmit.
        /// </summary>
        public static string Address
        {
            get
            {
                return ResourceRead.GetResourceValue("Address", resourceFile);
            }
        }

        public static string City
        {
            get
            {
                return ResourceRead.GetResourceValue("City", resourceFile);
            }
        }
        public static string State
        {
            get
            {
                return ResourceRead.GetResourceValue("State", resourceFile);
            }
        }
        public static string ZipCode
        {
            get
            {
                return ResourceRead.GetResourceValue("ZipCode", resourceFile);
            }
        }
        public static string Country
        {
            get
            {
                return ResourceRead.GetResourceValue("Country", resourceFile);
            }
        }
        public static string SupplierName
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierName", resourceFile);
            }
        }
        public static string Phone
        {
            get
            {
                return ResourceRead.GetResourceValue("Phone", resourceFile);
            }
        }
        public static string Email
        {
            get
            {
                return ResourceRead.GetResourceValue("Email", resourceFile);
            }
        }
        public static string optBlank
        {
            get
            {
                return ResourceRead.GetResourceValue("optBlank", resourceFile);
            }
        }

        public static string optFixed
        {
            get
            {
                return ResourceRead.GetResourceValue("optFixed", resourceFile);
            }
        }

        public static string optBlanketOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("optBlanketOrder", resourceFile);
            }
        }
        public static string optIncreamentingbyOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("optIncreamentingbyOrder", resourceFile);
            }
        }
        public static string optIncreamentingbyDay
        {
            get
            {
                return ResourceRead.GetResourceValue("optIncreamentingbyDay", resourceFile);
            }
        }
        public static string optDateIncrementing
        {
            get
            {
                return ResourceRead.GetResourceValue("optDateIncrementing", resourceFile);
            }
        }
        public static string optDate
        {
            get
            {
                return ResourceRead.GetResourceValue("optDate", resourceFile);
            }
        }
        public static string errmsgBlanketOrderNumbering
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlanketOrderNumbering", resourceFile);
            }
        }
        public static string errmsgBlanketOrderPurchaseNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlanketOrderPurchaseNumber", resourceFile);
            }
        }

        public static string errmsgBlankOrderNumbering
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankOrderNumbering", resourceFile);
            }
        }
        public static string errmsgBlankQuoteNumbering
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankQuoteNumbering", resourceFile);
            }
        }
        public static string errmsgBlankFixedPOAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankFixedPOAutoSequence", resourceFile);
            }
        }
        public static string errmsgBlankFixedPullPurchaseNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankFixedPullPurchaseNumberType", resourceFile);
            }
        }
        public static string errmsgBlankFixedPurchaseNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankFixedPurchaseNumberType", resourceFile);
            }
        }
        public static string PullPurchaseNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPurchaseNumberType", resourceFile);
            }
        }
        public static string LastPullPurchaseNumberUsed
        {
            get
            {
                return ResourceRead.GetResourceValue("LastPullPurchaseNumberUsed", resourceFile);
            }
        }
        public static string IsBlanketDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsBlanketDeleted", resourceFile);
            }
        }

        public static string PullPurchaseNumberFixed
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPurchaseNumberFixed", resourceFile);
            }
        }
        public static string PullPurchaseNumberBlanketOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPurchaseNumberBlanketOrder", resourceFile);
            }
        }
        public static string PullPurchaseNumberDateIncrementing
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPurchaseNumberDateIncrementing", resourceFile);
            }
        }
        public static string PullPurchaseNumberDate
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPurchaseNumberDate", resourceFile);
            }
        }
        public static string DefaultOrderRequiredDays
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultOrderRequiredDays", resourceFile);
            }
        }
        public static string optFixedIncrementing
        {
            get
            {
                return ResourceRead.GetResourceValue("optFixedIncrementing", resourceFile);
            }
        }
        public static string optDateIncrementingFixed
        {
            get
            {
                return ResourceRead.GetResourceValue("optDateIncrementingFixed", resourceFile);
            }
        }

        public static string SupplierImage
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierImage", resourceFile);
            }
        }

        public static string ImageExternalURL
        {
            get
            {
                return ResourceRead.GetResourceValue("ImageExternalURL", resourceFile);
            }
        }
        public static string TAOAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("TAOAutoSequence", resourceFile);
            }
        }
        public static string IsOrderReleaseNumberEditable
        {
            get
            {
                return ResourceRead.GetResourceValue("IsOrderReleaseNumberEditable", resourceFile);
            }
        }
        public static string POAutoNrReleaseNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("POAutoNrReleaseNumber", resourceFile);
            }
        }
        public static string errmsgBlankReleaseNumberPurchaseNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankReleaseNumberPurchaseNumberType", resourceFile);
            }
        }
        public static string errmsgReleaseNumberGreaterZero
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgReleaseNumberGreaterZero", resourceFile);
            }
        }
		public static string MsgSetOneDefaultSupplier
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSetOneDefaultSupplier", resourceFile);
            }
        }

        public static string MsgEndDateValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEndDateValidation", resourceFile);
            }
        }
        public static string MsgBlanketPOValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgBlanketPOValidation", resourceFile);
            }
        }
        public static string MsgDuplicatePOBlanket
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicatePOBlanket", resourceFile);
            }
        }
        public static string MsgSelectDefaultAccount
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectDefaultAccount", resourceFile);
            }
        }
        public static string MsgAccountNameNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAccountNameNumber", resourceFile);
            }
        }

        public static string MsgPhoneNumberValid
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPhoneNumberValid", resourceFile);
            }
        }

        public static string SelectItemsFromCatalog { get { return ResourceRead.GetResourceValue("SelectItemsFromCatalog", resourceFile); } }
        public static string MsgSupplierNotFound { get { return ResourceRead.GetResourceValue("MsgSupplierNotFound", resourceFile); } }
        public static string MsgSupplierNotAllowBlank { get { return ResourceRead.GetResourceValue("MsgSupplierNotAllowBlank", resourceFile); } }

        public static string MsgSupplierAccountNumberNameValidation { get { return ResourceRead.GetResourceValue("MsgSupplierAccountNumberNameValidation", resourceFile); } }

        public static string SupplierNameRequiredValidation { get { return ResourceRead.GetResourceValue("SupplierNameRequiredValidation", resourceFile); } }
        public static string AddressDetails { get { return ResourceRead.GetResourceValue("AddressDetails", resourceFile); } }
        public static string OtherDetails { get { return ResourceRead.GetResourceValue("OtherDetails", resourceFile); } }
        public static string POSequenceDetail { get { return ResourceRead.GetResourceValue("POSequenceDetail", resourceFile); } }

        public static string AddSupplier { get { return ResourceRead.GetResourceValue("AddSupplier", resourceFile); } }
        public static string SupplierMsgInvalidURL { get { return ResourceRead.GetResourceValue("SupplierMsgInvalidURL", resourceFile); } }
        public static string QuoteAutoSequence { get { return ResourceRead.GetResourceValue("QuoteAutoSequence", resourceFile); } }
        public static string QuoteAutoNrFixedValue { get { return ResourceRead.GetResourceValue("QuoteAutoNrFixedValue", resourceFile); } }
        public static string NextQuoteNo { get { return ResourceRead.GetResourceValue("NextQuoteNo", resourceFile); } }
        public static string QuoteSequenceDetail { get { return ResourceRead.GetResourceValue("QuoteSequenceDetail", resourceFile); } }
        public static string QuoteAutoNrReleaseNumber { get { return ResourceRead.GetResourceValue("QuoteAutoNrReleaseNumber", resourceFile); } }

        public static string optBlanketQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("optBlanketQuote", resourceFile);
            }
        }
        public static string optIncreamentingbyQuote
        {
            get
            {
                return ResourceRead.GetResourceValue("optIncreamentingbyQuote", resourceFile);
            }
        }
        public static string errmsgBlankFixedQuoteNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankFixedQuoteNumberType", resourceFile);
            }
        }

        public static string errmsgBlankReleaseNumberQuoteNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankReleaseNumberQuoteNumberType", resourceFile);
            }
        }

        public static string errmsgBlankFixedOrderNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankFixedOrderNumberType", resourceFile);
            }
        }

        public static string errmsgBlankReleaseNumberOrderNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankReleaseNumberOrderNumberType", resourceFile);
            }
        }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MultiEmailsAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _emailsName;
        public MultiEmailsAttribute(string emailaddress)
        {
            _emailsName = emailaddress;
            this.ErrorMessageResourceType = typeof(ResMessage);
            this.ErrorMessageResourceName = "MultiEmail";
            this.ErrorMessage = ResMessage.MultiEmail;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_emailsName);
            var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null);
            if (value != null)
            {
                RegexUtilities objRegexUtilities = new RegexUtilities();
                string emailadresses = (string)value;
                bool IsValidEmail = false;
                if (emailadresses.Contains(","))
                {
                    string[] arremails = new string[] { };
                    arremails = emailadresses.Split(',');
                    foreach (string semail in arremails)
                    {
                        if (!semail.ToLower().Equals("[Approver]".ToLower())
                            && !semail.ToLower().Equals("[Requester]".ToLower()))
                        {
                            IsValidEmail = objRegexUtilities.IsValidEmail(semail);
                            if (!IsValidEmail)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    IsValidEmail = objRegexUtilities.IsValidEmail(emailadresses);
                }
                if (!IsValidEmail)
                {
                    ValidationResult Ok = new ValidationResult(ResMessage.MultiEmail);
                    return Ok;
                }
            }
            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule>
               GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "multiemails"
            };
        }

    }

    public class RegexUtilities
    {
        bool invalid = false;

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
            }
            catch (Exception)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$", RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}
