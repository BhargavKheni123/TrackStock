using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ArchiveScheduleDTO
    {
        public long ScheduleID { get; set; }
        [Display(Name = "Module", ResourceType = typeof(ResCommon))]
        public int ModuleId { get; set; }
        //[Range(0,99, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ArchivalDuration", ResourceType = typeof(ResCommon))]
        public int Duration { get; set; }
        public int DurationType { get; set; }
        //public SchedulerDTO SchedulerParams { get; set; }

        [Display(Name = "ScheduleMode", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 5, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short ScheduleMode { get; set; }

        [Display(Name = "DailyRecurringType", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short DailyRecurringType { get; set; }

        [Display(Name = "DailyRecurringDays", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 365, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short DailyRecurringDays { get; set; }

        [Display(Name = "WeeklyRecurringWeeks", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 52, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short WeeklyRecurringWeeks { get; set; }


        [Display(Name = "HourlyRecurringHours", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 24, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short HourlyRecurringHours { get; set; }


        [Display(Name = "HourlyAtWhatMinute", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 59, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short HourlyAtWhatMinute { get; set; }


        public bool WeeklyOnMonday { get; set; }
        public bool WeeklyOnTuesday { get; set; }
        public bool WeeklyOnWednesday { get; set; }
        public bool WeeklyOnThursday { get; set; }
        public bool WeeklyOnFriday { get; set; }
        public bool WeeklyOnSaturday { get; set; }
        public bool WeeklyOnSunday { get; set; }
        public string WeeklySelectedDays { get; set; }

        [Display(Name = "MonthlyRecurringType", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short MonthlyRecurringType { get; set; }

        [Display(Name = "MonthlyDateOfMonth", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "NumberOnly", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 31, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short MonthlyDateOfMonth { get; set; }

        [Display(Name = "MonthlyRecurringMonths", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 12, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short MonthlyRecurringMonths { get; set; }

        [Display(Name = "MonthlyDayOfMonth", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 7, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short MonthlyDayOfMonth { get; set; }

        [Display(Name = "SubmissionMethod", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short SubmissionMethod { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ScheduleRunTime", ResourceType = typeof(ResSchedulerReportList))]
        public string ScheduleRunTime { get; set; }
        public DateTime ScheduleRunDateTime { get; set; }
        public long SupplierId { get; set; }

        public string SupplierName { get; set; }

        [Display(Name = "IsScheduleActive", ResourceType = typeof(ResSchedulerReportList))]
        public bool IsScheduleActive { get; set; }
        public long RoomId { get; set; }
        public string RoomName { get; set; }
        public short LoadSheduleFor { get; set; }
        public long CompanyId { get; set; }
        public long CreatedBy { get; set; }
        public long LastUpdatedBy { get; set; }
        [Display(Name = "NextRunDate", ResourceType = typeof(ResSupplierMaster))]
        public DateTime? NextRunDate { get; set; }
        public string NextRunDateTime { get; set; }
        public int IsScheduleChanged { get; set; }
        public Nullable<Guid> AssetToolID { get; set; }
        public Nullable<Int64> ReportID { get; set; }
        public string AttachmentTypes { get; set; }
        public string AttachmentReportIDs { get; set; }


        [Display(Name = "EmailAddress", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string EmailAddress { get; set; }

        public string BinNumber { get; set; }

        public EmailTemplateDetailDTO EmailTemplateDetail { get; set; }

        public string ScheduleFreqInWord
        {

            get
            {
                string Returnstr = string.Empty;
                switch (ScheduleMode)
                {
                    case 0:
                        Returnstr = "None";
                        break;
                    case 1:
                        if (DailyRecurringType == 1)
                        {
                            if (DailyRecurringDays == 1)
                            {
                                Returnstr = "Daily - Everyday at " + (ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }
                            else
                            {
                                Returnstr = "Daily - Every " + DailyRecurringDays + " Days at " + (ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }

                        }
                        else
                        {
                            Returnstr = "Daily - Every Weekdays at " + (ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                        }
                        break;
                    case 2:
                        if (WeeklyRecurringWeeks == 1)
                        {
                            Returnstr = "Weekly - Every Week on ";
                        }
                        else
                        {
                            Returnstr = "Weekly - Every " + WeeklyRecurringWeeks + " Week(s) on ";
                        }

                        if (WeeklyOnMonday)
                        {
                            Returnstr += ResSchedulerReportList.Monday + ",";
                        }
                        if (WeeklyOnTuesday)
                        {
                            Returnstr += ResSchedulerReportList.Tuesday + ",";
                        }
                        if (WeeklyOnWednesday)
                        {
                            Returnstr += ResSchedulerReportList.Wednesday + ",";
                        }
                        if (WeeklyOnThursday)
                        {
                            Returnstr += ResSchedulerReportList.Thursday + ",";
                        }
                        if (WeeklyOnFriday)
                        {
                            Returnstr += ResSchedulerReportList.Friday + ",";
                        }
                        if (WeeklyOnSaturday)
                        {
                            Returnstr += ResSchedulerReportList.Saturday + ",";
                        }
                        if (WeeklyOnSunday)
                        {
                            Returnstr += ResSchedulerReportList.Sunday + ",";
                        }
                        Returnstr = Returnstr.TrimEnd(new char[] { ',' });
                        Returnstr = Returnstr + "at " + (ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                        break;
                    case 3:
                        if (MonthlyRecurringType == 1)
                        {
                            if (MonthlyRecurringMonths == 1)
                            {
                                Returnstr = "Monthly - Every month on Day " + MonthlyDateOfMonth + " at " + (ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }
                            else
                            {
                                Returnstr = "Monthly - Every " + MonthlyRecurringMonths + " months on Day " + MonthlyDateOfMonth + " at " + (ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }

                        }
                        else
                        {
                            if (MonthlyRecurringMonths == 1)
                            {
                                Returnstr = "Monthly - Every month on the " + MonthlyDateOfMonth + " " + MonthlyDayOfMonth + " at " + (ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }
                            else
                            {
                                Returnstr = "Monthly - Every  " + MonthlyRecurringMonths + " months on the " + MonthlyDateOfMonth + " " + MonthlyDayOfMonth + "at " + (ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }
                        }
                        break;
                    case 4:
                        if (HourlyRecurringHours == 1)
                        {
                            Returnstr = "Hourly - at Every hour at " + HourlyAtWhatMinute + " Minute";
                        }
                        else
                        {
                            Returnstr = "Hourly - at Every " + HourlyRecurringHours + " hours at " + HourlyAtWhatMinute + " Minute";
                        }

                        break;
                    case 5:
                        Returnstr = "Immidiate";
                        break;

                }
                return Returnstr;
            }
        }

        public string ModuleName
        {
            get
            {
                if (LoadSheduleFor == 1)
                {
                    return "Transfer";
                }
                else if (LoadSheduleFor == 2)
                {
                    return "Order";
                }
                else if (LoadSheduleFor == 3)
                {
                    return "Tracking measurement and frequency";
                }
                else if (LoadSheduleFor == 4)
                {
                    return "Asset & Tool";
                }
                else if (LoadSheduleFor == 5)
                {
                    return "Reports";
                }
                else if (LoadSheduleFor == 6)
                {
                    return "Alerts";
                }
                else if (LoadSheduleFor == 7)
                {
                    return "Consigned Batch Pull";
                }
                else if (LoadSheduleFor == 10)
                {
                    return "Data Archive";
                }
                else
                {
                    return "";
                }

            }
            set { }
        }
        public string ModuleNameResource
        {
            get
            {
                if (LoadSheduleFor == 1)
                {
                    return ResSchedulerReportList.Transfer;
                }
                else if (LoadSheduleFor == 2)
                {
                    return ResSchedulerReportList.Order;
                }
                else if (LoadSheduleFor == 3)
                {
                    return ResSchedulerReportList.TrackingMeasurement;
                }
                else if (LoadSheduleFor == 4)
                {
                    return ResSchedulerReportList.AssetTool;
                }
                else if (LoadSheduleFor == 5)
                {
                    return ResSchedulerReportList.ReportsSchedule;
                }
                else if (LoadSheduleFor == 6)
                {
                    return ResSchedulerReportList.AlertsSchedule;
                }
                else if (LoadSheduleFor == 7)
                {
                    return ResSchedulerReportList.ConsignedBatchPull;

                }
                else
                {
                    return "";
                }

            }
            set { }
        }

        public short? ReportDataSelectionType { get; set; }
        public long? ReportDataSince { get; set; }
        public long? ScheduledBy { get; set; }
        [Display(Name = "ScheduleName", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string ScheduleName { get; set; }
        public int TotalRecords { get; set; }
        public string ReportName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? Created { get; set; }
        public List<EmailTemplateDetailDTO> lstTemplateDtls { get; set; }


        public long EmailTempateId { get; set; }

        public string MailBodyText { get; set; }

        public string MailSubject { get; set; }
        public long[] ReportIds { get; set; }
        public long[] EmailTemplateIds { get; set; }
        public string EmailTemplateDetails { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? Updated { get; set; }
        public bool RecalcSchedule { get; set; }
        public TimeSpan? ScheduleTime { get; set; }
        public long? UserID { get; set; }

        public string Module { get; set; }
        public string DurationTypeName { get; set; }

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

        private string _ScheduleNextRunDate;
        public string ScheduleNextRunDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ScheduleNextRunDate))
                {
                    _ScheduleNextRunDate = FnCommon.ConvertDateByTimeZone(NextRunDate, true);
                }
                return _ScheduleNextRunDate;
            }
            set { this._ScheduleNextRunDate = value; }
        }

        public string EnterpriseName { get; set; }
        public string EnterpriseDBName { get; set; }

        public string CompanyName { get; set; }

        public long ID { get; set; }
        public long EnterpriseId { get; set; }
    }
}
