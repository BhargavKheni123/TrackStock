using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class CycleCountSettingDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int32 ID { get; set; }



        //AClassFrequency
        [Display(Name = "AClassFrequency", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int16 AClassFrequency { get; set; }

        //BClassFrequency
        [Display(Name = "BClassFrequency", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int16 BClassFrequency { get; set; }

        //CClassFrequency
        [Display(Name = "CClassFrequency", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int16 CClassFrequency { get; set; }

        //DClassFrequency
        [Display(Name = "DClassFrequency", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int16 DClassFrequency { get; set; }

        //EClassFrequency
        [Display(Name = "EClassFrequency", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int16 EClassFrequency { get; set; }

        [Display(Name = "FClassFrequency", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int16 FClassFrequency { get; set; }

        //YearStartDate
        [Display(Name = "YearStartDate", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime YearStartDate { get; set; }


        public int YearStartMonth { get; set; }

        public int YearEndMonth { get; set; }

        public int YearStartDay { get; set; }

        public int YearEndDay { get; set; }

        [Display(Name = "YearStartDateStr", ResourceType = typeof(ResCycleCountSetting))]
        public System.String YearStartDateStr { get; set; }

        [Display(Name = "YearEndDateStr", ResourceType = typeof(ResCycleCountSetting))]
        public System.String YearEndDateStr { get; set; }

        //YearStartDate
        [Display(Name = "YearEndDate", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime YearEndDate { get; set; }

        //EmailAddressesPreCountNotification
        [Display(Name = "EmailAddressesPreCountNotification", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EmailAddressesPreCountNotification { get; set; }

        //EmailAddressesDailyCycle
        [Display(Name = "EmailAddressesDailyCycle", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EmailAddressesDailyCycle { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Created { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 LastUpdatedBy { get; set; }

        //IsDeleted
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsDeleted { get; set; }

        //IsArchived
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Boolean IsArchived { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //CompanyId
        [Display(Name = "CompanyId", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CompanyId { get; set; }

        //RoomId
        [Display(Name = "RoomId", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 RoomId { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

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

        private string _NextRunDatestr;
        public string NextRunDatestr
        {
            get
            {
                if (string.IsNullOrEmpty(_NextRunDatestr))
                {
                    _NextRunDatestr = FnCommon.ConvertDateByTimeZone(NextRunDate, true);
                }
                return _NextRunDatestr;
            }
            set { this._NextRunDatestr = value; }
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

        public List<InventoryClassificationMasterDTO> lstClassifications { get; set; }
        public List<CycleCountSetUpDTO> lstCycleCountSetup { get; set; }
        [Range(1, 200, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int RecurrringDays { get; set; }

        [Range(1, 200, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int CycleCountsPerCycle { get; set; }


        [Display(Name = "CycleCountTime", ResourceType = typeof(ResCycleCountSetting))]
        public TimeSpan CycleCountTime { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string CycleCountTimestr { get; set; }

        [Display(Name = "NextRunDate", ResourceType = typeof(ResCycleCountSetting))]
        public DateTime? NextRunDate { get; set; }

        [Display(Name = "CountFrequencyType", ResourceType = typeof(ResCycleCountSetting))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public int CountFrequencyType { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResCycleCountSetting))]
        public bool IsActive { get; set; }
        [Display(Name = "RecurringType", ResourceType = typeof(ResCycleCountSetting))]
        public int RecurringType { get; set; }

        [Display(Name = "MissedItemsEmailTime", ResourceType = typeof(ResCycleCountSetting))]
        public TimeSpan MissedItemsEmailTime { get; set; }


        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string MissedItemsEmailTimestr { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 10, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "MissedItemEmailPriorHours", ResourceType = typeof(ResCycleCountSetting))]
        public int MissedItemEmailPriorHours { get; set; }

        [Display(Name = "CountType", ResourceType = typeof(ResInventoryCount))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String CountType { get; set; }
    }


    public class CycleCountSettingMasterDTO
    {

        public int ID { get; set; }
        public int CycleCountSettingID { get; set; }
        public short AClassFrequency { get; set; }
        public short BClassFrequency { get; set; }
        public short CClassFrequency { get; set; }
        public short DClassFrequency { get; set; }
        public short EClassFrequency { get; set; }
        public DateTime YearStartDate { get; set; }
        public string EmailAddressesPreCountNotification { get; set; }
        public string EmailAddressesDailyCycle { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public long CreatedBy { get; set; }
        public long LastUpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }
        public Guid GUID { get; set; }
        public long EnterpriseId { get; set; }
        public long CompanyId { get; set; }
        public long RoomId { get; set; }
        public bool IsActive { get; set; }
        public short FClassFrequency { get; set; }
        public DateTime? YearEndDate { get; set; }
        public int RecurrringDays { get; set; }
        public TimeSpan CycleCountTime { get; set; }
        public DateTime? NextRunDate { get; set; }
        public int CountFrequencyType { get; set; }
        public int CycleCountsPerCycle { get; set; }
        public int RecurringType { get; set; }
        public TimeSpan MissedItemsEmailTime { get; set; }
        public int MissedItemEmailPriorHours { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string EnterpriseDBName { get; set; }
    }
    public class CycleCountSetUpDTO
    {
        public string AddedFrom { get; set; }
        public Guid ClassificationGUID { get; set; }
        public long? CompanyID { get; set; }
        public DateTime? Created { get; set; }
        public long? CreatedBy { get; set; }

        [Range(0, 365, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CycleCountFrequency", ResourceType = typeof(ResCycleCountSetting))]
        public int? CycleCountFrequency { get; set; }
        public string EditedFrom { get; set; }
        public Guid GUID { get; set; }
        public long ID { get; set; }
        public bool? IsArchived { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastUpdated { get; set; }
        public long? LastUpdatedBy { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ReceivedOnWeb { get; set; }
        public long? Room { get; set; }
        public string InventoryClassification { get; set; }

        public int ICID { get; set; }
        public Guid ICGUID { get; set; }

        public int? CycleCountSettingId { get; set; }

        [Display(Name = "TimeBaseRecurfrequency", ResourceType = typeof(ResCycleCountSetting))]
        [Range(1, 1000, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public int? TimeBaseRecurfrequency { get; set; }

        [Display(Name = "TimeBaseUnit", ResourceType = typeof(ResCycleCountSetting))]

        public int? TimeBaseUnit { get; set; }

        public int RecurringType { get; set; }

    }

    public class ResCycleCountSetting
    {
        private static string ResourceFileName = "ResCycleCountSetting";
        public static string MissedItemEmailPriorHours
        {
            get
            {
                return ResourceRead.GetResourceValue("MissedItemEmailPriorHours", ResourceFileName);
            }
        }


        public static string MissedItemsEmailTime
        {
            get
            {
                return ResourceRead.GetResourceValue("MissedItemsEmailTime", ResourceFileName);
            }
        }

        public static string CycleCountTime
        {
            get
            {
                return ResourceRead.GetResourceValue("CycleCountTime", ResourceFileName);
            }
        }

        public static string CycleBaseRecurring
        {
            get
            {
                return ResourceRead.GetResourceValue("CycleBaseRecurring", ResourceFileName);
            }
        }

        public static string NoOfTimesClassCount
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfTimesClassCount", ResourceFileName);
            }
        }
                
        public static string CycleCountFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfTimesClassCount", ResourceFileName);
            }
        }


        public static string ClassTimeBaseRecurring
        {
            get
            {
                return ResourceRead.GetResourceValue("ClassTimeBaseRecurring", ResourceFileName);
            }
        }

        public static string RecurringType
        {
            get
            {
                return ResourceRead.GetResourceValue("RecurringType", ResourceFileName);
            }
        }




        public static string TimeBasedFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("TimeBasedFrequency", ResourceFileName);
            }
        }

        
        public static string TimeBaseRecurfrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("ClassTimeBaseRecurring", ResourceFileName);
            }
        }


        public static string TimeBaseUnit
        {
            get
            {
                return ResourceRead.GetResourceValue("TimeBaseUnit", ResourceFileName);
            }
        }



        public static string TimeBaseRecurring
        {
            get
            {
                return ResourceRead.GetResourceValue("TimeBaseRecurring", ResourceFileName);
            }
        }

        public static string NextRunDate
        {
            get
            {
                return ResourceRead.GetResourceValue("NextRunDate", ResourceFileName);
            }
        }

        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", ResourceFileName);
            }
        }

        public static string CountFrequencyType
        {
            get
            {
                return ResourceRead.GetResourceValue("CountFrequencyType", ResourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        /// 
        public static string SettingTabName
        {
            get
            {
                return ResourceRead.GetResourceValue("SettingTabName", ResourceFileName);
            }
        }
        public static string SettingHeaderName
        {
            get
            {
                return ResourceRead.GetResourceValue("SettingHeaderName", ResourceFileName);
            }
        }
        public static string Action
        {
            get
            {
                return ResourceRead.GetResourceValue("Action", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CycleCountSetting {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to CycleCountSetting.
        /// </summary>
        public static string CycleCountSettingHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("CycleCountSettingHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CycleCountSetting.
        /// </summary>
        public static string CycleCountSetting
        {
            get
            {
                return ResourceRead.GetResourceValue("CycleCountSetting", ResourceFileName);
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
        ///   Looks up a localized string similar to AClassFrequency.
        /// </summary>
        public static string AClassFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("AClassFrequency", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BClassFrequency.
        /// </summary>
        public static string BClassFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("BClassFrequency", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CClassFrequency.
        /// </summary>
        public static string CClassFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("CClassFrequency", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DClassFrequency.
        /// </summary>
        public static string DClassFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("DClassFrequency", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EClassFrequency.
        /// </summary>
        public static string EClassFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("EClassFrequency", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EClassFrequency.
        /// </summary>
        public static string FClassFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("FClassFrequency", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to YearStartDate.
        /// </summary>
        public static string YearStartDate
        {
            get
            {
                return ResourceRead.GetResourceValue("YearStartDate", ResourceFileName);
            }
        }

        
        public static string YearStartDateStr
        {
            get
            {
                return ResourceRead.GetResourceValue("YearStartDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to YearStartDate.
        /// </summary>
        public static string YearEndDate
        {
            get
            {
                return ResourceRead.GetResourceValue("YearEndDate", ResourceFileName);
            }
        }

        public static string YearEndDateStr
        {
            get
            {
                return ResourceRead.GetResourceValue("YearEndDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EmailAddressesPreCountNotification.
        /// </summary>
        public static string EmailAddressesPreCountNotification
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailAddressesPreCountNotification", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EmailAddressesDailyCycle.
        /// </summary>
        public static string EmailAddressesDailyCycle
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailAddressesDailyCycle", ResourceFileName);
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
        ///   Looks up a localized string similar to CompanyId.
        /// </summary>
        public static string CompanyId
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyId", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RoomId.
        /// </summary>
        public static string RoomId
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomId", ResourceFileName);
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
    }
}


