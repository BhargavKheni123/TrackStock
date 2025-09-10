using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class ToolsSchedulerDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //SchedulerName
        [Display(Name = "SchedulerName", ResourceType = typeof(ResToolsScheduler))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String SchedulerName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResToolsScheduler))]
        [StringLength(8000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Description { get; set; }

        //StartDate
        //[Display(Name = "StartDate", ResourceType = typeof(ResToolsScheduler))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        //public Nullable<System.DateTime> StartDate { get; set; }

        //EndDate
        //[Display(Name = "EndDate", ResourceType = typeof(ResToolsScheduler))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        //public Nullable<System.DateTime> EndDate { get; set; }

        //ScheduleType
        //[Display(Name = "ScheduleType", ResourceType = typeof(ResToolsScheduler))]
        //public int ScheduleType { get; set; }
        [Display(Name = "ScheduleType", ResourceType = typeof(ResToolsScheduler))]
        public int SchedulerType { get; set; }


        public SchedulerDTO ScheduleParams { get; set; }

        public string ScheduleTypeName
        {
            get
            {
                if (SchedulerType == 1)
                {
                    return ResToolsScheduler.TimeBased;
                }
                else if (SchedulerType == 2)
                {
                    return ResToolsScheduler.OperationalHours;
                }
                else if (SchedulerType == 3)
                {
                    return ResToolsScheduler.Mileage;
                }
                else if (SchedulerType == 4)
                {
                    return ResToolsScheduler.CheckOuts;
                }
                else if (SchedulerType == 0)
                {
                    return ResToolsScheduler.None;
                }
                else
                {
                    return ResToolsScheduler.ScheduleType;
                }
            }
            set { }
        }



        ////Days
        //[Display(Name = "Days", ResourceType = typeof(ResToolsScheduler))]
        ////[Range(1, 6, ErrorMessage = "Days should be between 1 to 6.")]
        //public Nullable<System.Int32> Days { get; set; }

        ////WeekDays
        //[Display(Name = "WeekDays", ResourceType = typeof(ResToolsScheduler))]
        //[StringLength(512, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //public System.String WeekDays { get; set; }

        ////MonthDays
        //[Display(Name = "MonthDays", ResourceType = typeof(ResToolsScheduler))]
        //public Nullable<System.Int32> MonthDays { get; set; }

        //OperationalHours
        [Display(Name = "OperationalHours", ResourceType = typeof(ResToolsScheduler))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> OperationalHours { get; set; }

        //Mileage
        [Display(Name = "Mileage", ResourceType = typeof(ResToolsScheduler))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Mileage { get; set; }

        [Display(Name = "RecurringDays", ResourceType = typeof(ResToolsScheduler))]
        public int RecurringDays { get; set; }


        [Display(Name = "CheckOuts", ResourceType = typeof(ResToolsScheduler))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> CheckOuts { get; set; }
        ////ScheduleTime
        //[Display(Name = "ScheduleTime", ResourceType = typeof(ResToolsScheduler))]
        //public string ScheduleTime { get; set; }

        ////ToolID
        //[Display(Name = "ToolID", ResourceType = typeof(ResToolsScheduler))]
        //public Nullable<System.Guid> ToolGUID { get; set; }

        ////ToolName
        //[Display(Name = "ToolName", ResourceType = typeof(ResToolsScheduler))]
        //[StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //public System.String ToolName { get; set; }

        ////AssetID
        //[Display(Name = "AssetID", ResourceType = typeof(ResToolsScheduler))]
        //public Nullable<System.Guid> AssetGUID { get; set; }

        ////AssetName
        //[Display(Name = "AssetName", ResourceType = typeof(ResToolsScheduler))]
        //[StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[AllowHtml]
        //public System.String AssetName { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public System.DateTime Created { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public System.DateTime Updated { get; set; }

        //LastUpdatedBy
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResToolsScheduler))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResToolsScheduler))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResToolsScheduler))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResToolsScheduler))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResToolsScheduler))]
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

        public Int64 HistoryID { get; set; }

        public int MainScheduleType { get; set; }

        [Display(Name = "TimeBasedFrequency", ResourceType = typeof(ResToolsScheduler))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 1000, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public int TimeBasedFrequency { get; set; }

        [Display(Name = "TimeBaseUnit", ResourceType = typeof(ResToolsScheduler))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int TimeBaseUnit { get; set; }

        [Display(Name = "TimeBaseUnit", ResourceType = typeof(ResToolsScheduler))]
        public string TimeBaseUnitName { get; set; }

        [Display(Name = "ScheduleFor", ResourceType = typeof(ResToolsScheduler))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Byte> ScheduleFor { get; set; }

        public string ScheduleForName { get; set; }
        private string _ScheduleFreqInWord;
        public string ScheduleFreqInWord
        {
            get
            {
                if (SchedulerType == (int)MaintenanceScheduleType.TimeBase)
                {
                    _ScheduleFreqInWord = ResSchedulerReportList.Every + " ";
                    if (TimeBasedFrequency > 1)
                    {
                        _ScheduleFreqInWord = _ScheduleFreqInWord + TimeBasedFrequency + " ";
                    }
                    if (TimeBaseUnit == 1)
                    {
                        _ScheduleFreqInWord = _ScheduleFreqInWord + ResCommon.Days;
                    }
                    else if (TimeBaseUnit == 2)
                    {
                        _ScheduleFreqInWord = _ScheduleFreqInWord + ResCommon.Weeks;
                    }
                    else if (TimeBaseUnit == 3)
                    {
                        _ScheduleFreqInWord = _ScheduleFreqInWord + ResCommon.Months;
                    }
                    else if (TimeBaseUnit == 4)
                    {
                        _ScheduleFreqInWord = _ScheduleFreqInWord + ResCommon.Years;
                    }
                    else
                    {
                        _ScheduleFreqInWord = string.Empty;
                    }
                    return _ScheduleFreqInWord;
                }
                else
                {
                    return _ScheduleFreqInWord;
                }

            }
            set { }
        }

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

        public Int16 ScheduleMode { get; set; }

        public IEnumerable<ToolsSchedulerDetailsDTO> lstItems { get; set; }


        public string ItemNumber { get; set; }
        public double? Quantity { get; set; }

        public string SchedulerTypeName { get; set; }

        public bool? IsLineItemDeleted { get; set; }

        public int TotalRecords { get; set; }
    }

    public class ResToolsScheduler
    {
        private static string ResourceFileName = "ResToolsScheduler";


        public static string RecurringDays
        {
            get
            {
                return ResourceRead.GetResourceValue("RecurringDays", ResourceFileName);
            }
        }
        public static string TimeBaseUnit
        {
            get
            {
                return ResourceRead.GetResourceValue("TimeBaseUnit", ResourceFileName);
            }
        }
        public static string TimeBasedFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("TimeBasedFrequency", ResourceFileName);
            }
        }
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }

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
        ///   Looks up a localized string similar to ToolsScheduler {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to ToolsScheduler.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ToolsScheduler.
        /// </summary>
        public static string ToolsScheduler
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolsScheduler", ResourceFileName);
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
        ///   Looks up a localized string similar to SchedulerName.
        /// </summary>
        public static string SchedulerName
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerName", ResourceFileName);
            }
        }
        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SchedulerName.
        /// </summary>
        public static string ScheduleFor
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleFor", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to StartDate.
        /// </summary>
        public static string StartDate
        {
            get
            {
                return ResourceRead.GetResourceValue("StartDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EndDate.
        /// </summary>
        public static string EndDate
        {
            get
            {
                return ResourceRead.GetResourceValue("EndDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ScheduleType.
        /// </summary>
        public static string ScheduleType
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleType", ResourceFileName);
            }
        }
        public static string TimeBased
        {
            get
            {
                return ResourceRead.GetResourceValue("TimeBased", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Days.
        /// </summary>
        public static string Days
        {
            get
            {
                return ResourceRead.GetResourceValue("Days", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WeekDays.
        /// </summary>
        public static string WeekDays
        {
            get
            {
                return ResourceRead.GetResourceValue("WeekDays", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MonthDays.
        /// </summary>
        public static string MonthDays
        {
            get
            {
                return ResourceRead.GetResourceValue("MonthDays", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OperationalHours.
        /// </summary>
        public static string OperationalHours
        {
            get
            {
                return ResourceRead.GetResourceValue("OperationalHours", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mileage.
        /// </summary>
        public static string Mileage
        {
            get
            {
                return ResourceRead.GetResourceValue("Mileage", ResourceFileName);
            }
        }
        public static string CheckOuts
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOuts", ResourceFileName);
            }
        }
        public static string None
        {
            get
            {
                return ResourceRead.GetResourceValue("None", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to ScheduleTime.
        /// </summary>
        public static string ScheduleTime
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleTime", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ToolID.
        /// </summary>
        public static string ToolID
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ToolName.
        /// </summary>
        public static string ToolName
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolName", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AssetID.
        /// </summary>
        public static string AssetID
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AssetName.
        /// </summary>
        public static string AssetName
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetName", ResourceFileName);
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
        public static string NoPullRights
        {
            get
            {
                return ResourceRead.GetResourceValue("NoPullRights", ResourceFileName);
            }
        }
        public static string SchedulerTypeShouldBe
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerTypeShouldBe", ResourceFileName);
            }
        }

        public static string TimeBasedUnitMustBe
        {
            get
            {
                return ResourceRead.GetResourceValue("TimeBasedUnitMustBe", ResourceFileName);
            }
        }
        public static string Scheduler
        {
            get
            {
                return ResourceRead.GetResourceValue("Scheduler", ResourceFileName);
            }
        }
    }
    public enum TimebasedScheduleFreq
    {
        Days = 1,
        Weeks = 2,
        Months = 3,
        Years = 4
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EndDateCheckAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _StartDateFieldName;
        public EndDateCheckAttribute(string StartDateFieldName)
        {
            _StartDateFieldName = StartDateFieldName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_StartDateFieldName);
            var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null);
            if (value != null)
            {
                DateTime EndDate = Convert.ToDateTime(value);
                if (EndDate <= Convert.ToDateTime(otherPropertyValue))
                {
                    ValidationResult Ok = new ValidationResult("End Date must be grater then or equal to Start Date");
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
                ValidationType = "EndDateCheckAttribute"
            };
        }
    }

}


