using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class PullSchedulerDTO
    {

        public long Pull_ScheduleID { get; set; }

        [Display(Name = "ScheduleMode", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 6, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_ScheduleMode { get; set; }

        [Display(Name = "DailyRecurringType", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_DailyRecurringType { get; set; }

        [Display(Name = "DailyRecurringDays", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 365, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_DailyRecurringDays { get; set; }

        [Display(Name = "WeeklyRecurringWeeks", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 52, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_WeeklyRecurringWeeks { get; set; }


        [Display(Name = "HourlyRecurringHours", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 24, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_HourlyRecurringHours { get; set; }


        [Display(Name = "HourlyAtWhatMinute", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 59, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_HourlyAtWhatMinute { get; set; }


        public bool Pull_WeeklyOnMonday { get; set; }
        public bool Pull_WeeklyOnTuesday { get; set; }
        public bool Pull_WeeklyOnWednesday { get; set; }
        public bool Pull_WeeklyOnThursday { get; set; }
        public bool Pull_WeeklyOnFriday { get; set; }
        public bool Pull_WeeklyOnSaturday { get; set; }
        public bool Pull_WeeklyOnSunday { get; set; }
        public string Pull_WeeklySelectedDays { get; set; }

        [Display(Name = "MonthlyRecurringType", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_MonthlyRecurringType { get; set; }

        [Display(Name = "MonthlyDateOfMonth", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "NumberOnly", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 31, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_MonthlyDateOfMonth { get; set; }

        [Display(Name = "MonthlyRecurringMonths", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 12, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_MonthlyRecurringMonths { get; set; }

        [Display(Name = "MonthlyDayOfMonth", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 7, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_MonthlyDayOfMonth { get; set; }

        [Display(Name = "SubmissionMethod", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Pull_SubmissionMethod { get; set; }
        [Display(Name = "PullScheduleRunTime", ResourceType = typeof(ResSchedulerReportList))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Pull_ScheduleRunTime { get; set; }
        public DateTime Pull_ScheduleRunDateTime { get; set; }
        public long Pull_SupplierId { get; set; }

        public string Pull_SupplierName { get; set; }

        [Display(Name = "Pull_IsScheduleActive", ResourceType = typeof(ResSchedulerReportList))]
        public bool Pull_IsScheduleActive { get; set; }
        public long Pull_RoomId { get; set; }
        public string Pull_RoomName { get; set; }
        public short Pull_LoadSheduleFor { get; set; }
        public long Pull_CompanyId { get; set; }
        public long Pull_CreatedBy { get; set; }
        public long Pull_LastUpdatedBy { get; set; }
        [Display(Name = "NextRunDate", ResourceType = typeof(ResSupplierMaster))]
        public DateTime? Pull_NextRunDate { get; set; }
        public string Pull_NextRunDateTime { get; set; }
        public int Pull_IsScheduleChanged { get; set; }
        public Nullable<Guid> Pull_AssetToolID { get; set; }
        public Nullable<Int64> Pull_ReportID { get; set; }
        public string Pull_AttachmentTypes { get; set; }
        public string Pull_AttachmentReportIDs { get; set; }


        [Display(Name = "EmailAddress", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Pull_EmailAddress { get; set; }

        public string Pull_BinNumber { get; set; }



        public EmailTemplateDetailDTO Pull_EmailTemplateDetail { get; set; }
        public string Pull_ModuleName
        {
            get
            {
                if (Pull_LoadSheduleFor == 1)
                {
                    return "Transfer";
                }
                else if (Pull_LoadSheduleFor == 2)
                {
                    return "Order";
                }
                else if (Pull_LoadSheduleFor == 3)
                {
                    return "Tracking measurement and frequency";
                }
                else if (Pull_LoadSheduleFor == 4)
                {
                    return "Asset & Tool";
                }
                else if (Pull_LoadSheduleFor == 5)
                {
                    return "Reports";
                }
                else if (Pull_LoadSheduleFor == 6)
                {
                    return "Alerts";
                }
                else if (Pull_LoadSheduleFor == 7)
                {
                    return "Pull Batch";
                }
                else
                {
                    return "";
                }




            }
            set { }
        }
        public string Pull_ModuleNameResource
        {
            get
            {
                if (Pull_LoadSheduleFor == 1)
                {
                    return ResSchedulerReportList.Transfer;
                }
                else if (Pull_LoadSheduleFor == 2)
                {
                    return ResSchedulerReportList.Order;
                }
                else if (Pull_LoadSheduleFor == 3)
                {
                    return ResSchedulerReportList.TrackingMeasurement;
                }
                else if (Pull_LoadSheduleFor == 4)
                {
                    return ResSchedulerReportList.AssetTool;
                }
                else if (Pull_LoadSheduleFor == 5)
                {
                    return ResSchedulerReportList.ReportsSchedule;
                }
                else if (Pull_LoadSheduleFor == 6)
                {
                    return ResSchedulerReportList.AlertsSchedule;
                }
                else if (Pull_LoadSheduleFor == 7)
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
        public short? Pull_ReportDataSelectionType { get; set; }
        public long? Pull_ReportDataSince { get; set; }
        public long? Pull_ScheduledBy { get; set; }
        [Display(Name = "ScheduleName", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Pull_ScheduleName { get; set; }
        public int Pull_TotalRecords { get; set; }
        public string Pull_ReportName { get; set; }
        public string Pull_CreatedByName { get; set; }
        public string Pull_UpdatedByName { get; set; }
        public DateTime? Pull_Created { get; set; }
        public List<EmailTemplateDetailDTO> Pull_lstTemplateDtls { get; set; }


        public long Pull_EmailTempateId { get; set; }

        public string Pull_MailBodyText { get; set; }

        public string Pull_MailSubject { get; set; }
        public long[] Pull_ReportIds { get; set; }
        public long[] Pull_EmailTemplateIds { get; set; }
        public string Pull_EmailTemplateDetails { get; set; }

        public bool Pull_IsDeleted { get; set; }

        public DateTime? Pull_Updated { get; set; }
        public bool Pull_RecalcSchedule { get; set; }
        public TimeSpan? Pull_ScheduleTime { get; set; }

    }

    public enum ScheduleParamFor
    {
        Transfer = 1,
        Order = 2,
        TrackingMeasurementAndFrequency = 3,
        AssetTool = 4,
        Reports = 5,
        Alerts = 6,
        ConsignedBatchPull = 7,
        DailyMidNightCalculations = 8,
        UserSchedule = 9



    }
    public class SchedulerDTO
    {

        public long ScheduleID { get; set; }

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
                else if (LoadSheduleFor == 12)
                {
                    //   return ResSchedulerReportList.ConsignedBatchPull;
                    return "Quote";

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
    }

    public class QuoteSchedulerDTO
    {

        public long Quote_ScheduleID { get; set; }

        [Display(Name = "ScheduleMode", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 5, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_ScheduleMode { get; set; }

        [Display(Name = "DailyRecurringType", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_DailyRecurringType { get; set; }

        [Display(Name = "DailyRecurringDays", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 365, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_DailyRecurringDays { get; set; }

        [Display(Name = "WeeklyRecurringWeeks", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 52, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_WeeklyRecurringWeeks { get; set; }


        [Display(Name = "HourlyRecurringHours", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 24, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_HourlyRecurringHours { get; set; }


        [Display(Name = "HourlyAtWhatMinute", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, 59, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_HourlyAtWhatMinute { get; set; }


        public bool Quote_WeeklyOnMonday { get; set; }
        public bool Quote_WeeklyOnTuesday { get; set; }
        public bool Quote_WeeklyOnWednesday { get; set; }
        public bool Quote_WeeklyOnThursday { get; set; }
        public bool Quote_WeeklyOnFriday { get; set; }
        public bool Quote_WeeklyOnSaturday { get; set; }
        public bool Quote_WeeklyOnSunday { get; set; }
        public string Quote_WeeklySelectedDays { get; set; }

        [Display(Name = "MonthlyRecurringType", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_MonthlyRecurringType { get; set; }

        [Display(Name = "MonthlyDateOfMonth", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceName = "NumberOnly", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 31, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_MonthlyDateOfMonth { get; set; }

        [Display(Name = "MonthlyRecurringMonths", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 12, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_MonthlyRecurringMonths { get; set; }

        [Display(Name = "MonthlyDayOfMonth", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 7, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_MonthlyDayOfMonth { get; set; }

        [Display(Name = "SubmissionMethod", ResourceType = typeof(ResSupplierMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 2, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public short Quote_SubmissionMethod { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ScheduleRunTime", ResourceType = typeof(ResSchedulerReportList))]
        public string Quote_ScheduleRunTime { get; set; }
        public DateTime Quote_ScheduleRunDateTime { get; set; }
        public long Quote_SupplierId { get; set; }

        public string Quote_SupplierName { get; set; }

        [Display(Name = "IsScheduleActive", ResourceType = typeof(ResSchedulerReportList))]
        public bool Quote_IsScheduleActive { get; set; }
        public long Quote_RoomId { get; set; }
        public string Quote_RoomName { get; set; }
        public short Quote_LoadSheduleFor { get; set; }
        public long Quote_CompanyId { get; set; }
        public long Quote_CreatedBy { get; set; }
        public long Quote_LastUpdatedBy { get; set; }
        [Display(Name = "NextRunDate", ResourceType = typeof(ResSupplierMaster))]
        public DateTime? Quote_NextRunDate { get; set; }
        public string Quote_NextRunDateTime { get; set; }
        public int Quote_IsScheduleChanged { get; set; }
        public Nullable<Guid> Quote_AssetToolID { get; set; }
        public Nullable<Int64> Quote_ReportID { get; set; }
        public string Quote_AttachmentTypes { get; set; }
        public string Quote_AttachmentReportIDs { get; set; }


        [Display(Name = "EmailAddress", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Quote_EmailAddress { get; set; }

        public string Quote_BinNumber { get; set; }



        public EmailTemplateDetailDTO Quote_EmailTemplateDetail { get; set; }

        public string Quote_ScheduleFreqInWord
        {

            get
            {
                string Returnstr = string.Empty;
                switch (Quote_ScheduleMode)
                {
                    case 0:
                        Returnstr = "None";
                        break;
                    case 1:
                        if (Quote_DailyRecurringType == 1)
                        {
                            if (Quote_DailyRecurringDays == 1)
                            {
                                Returnstr = "Daily - Everyday at " + (Quote_ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }
                            else
                            {
                                Returnstr = "Daily - Every " + Quote_DailyRecurringDays + " Days at " + (Quote_ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }

                        }
                        else
                        {
                            Returnstr = "Daily - Every Weekdays at " + (Quote_ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                        }
                        break;
                    case 2:
                        if (Quote_WeeklyRecurringWeeks == 1)
                        {
                            Returnstr = "Weekly - Every Week on ";
                        }
                        else
                        {
                            Returnstr = "Weekly - Every " + Quote_WeeklyRecurringWeeks + " Week(s) on ";
                        }

                        if (Quote_WeeklyOnMonday)
                        {
                            Returnstr += ResSchedulerReportList.Monday + ",";
                        }
                        if (Quote_WeeklyOnTuesday)
                        {
                            Returnstr += ResSchedulerReportList.Tuesday + ",";
                        }
                        if (Quote_WeeklyOnWednesday)
                        {
                            Returnstr += ResSchedulerReportList.Wednesday + ",";
                        }
                        if (Quote_WeeklyOnThursday)
                        {
                            Returnstr += ResSchedulerReportList.Thursday + ",";
                        }
                        if (Quote_WeeklyOnFriday)
                        {
                            Returnstr += ResSchedulerReportList.Friday + ",";
                        }
                        if (Quote_WeeklyOnSaturday)
                        {
                            Returnstr += ResSchedulerReportList.Saturday + ",";
                        }
                        if (Quote_WeeklyOnSunday)
                        {
                            Returnstr += ResSchedulerReportList.Sunday + ",";
                        }
                        Returnstr = Returnstr.TrimEnd(new char[] { ',' });
                        Returnstr = Returnstr + "at " + (Quote_ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                        break;
                    case 3:
                        if (Quote_MonthlyRecurringType == 1)
                        {
                            if (Quote_MonthlyRecurringMonths == 1)
                            {
                                Returnstr = "Monthly - Every month on Day " + Quote_MonthlyDateOfMonth + " at " + (Quote_ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }
                            else
                            {
                                Returnstr = "Monthly - Every " + Quote_MonthlyRecurringMonths + " months on Day " + Quote_MonthlyDateOfMonth + " at " + (Quote_ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }

                        }
                        else
                        {
                            if (Quote_MonthlyRecurringMonths == 1)
                            {
                                Returnstr = "Monthly - Every month on the " + Quote_MonthlyDateOfMonth + " " + Quote_MonthlyDayOfMonth + " at " + (Quote_ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }
                            else
                            {
                                Returnstr = "Monthly - Every  " + Quote_MonthlyRecurringMonths + " months on the " + Quote_MonthlyDateOfMonth + " " + Quote_MonthlyDayOfMonth + "at " + (Quote_ScheduleTime ?? new TimeSpan(0, 1, 0)).ToString(@"hh\:mm");
                            }
                        }
                        break;
                    case 4:
                        if (Quote_HourlyRecurringHours == 1)
                        {
                            Returnstr = "Hourly - at Every hour at " + Quote_HourlyAtWhatMinute + " Minute";
                        }
                        else
                        {
                            Returnstr = "Hourly - at Every " + Quote_HourlyRecurringHours + " hours at " + Quote_HourlyAtWhatMinute + " Minute";
                        }

                        break;
                    case 5:
                        Returnstr = "Immidiate";
                        break;

                }
                return Returnstr;
            }
        }

        public string Quote_ModuleName
        {
            get
            {
                if (Quote_LoadSheduleFor == 1)
                {
                    return "Transfer";
                }
                else if (Quote_LoadSheduleFor == 2)
                {
                    return "Order";
                }
                else if (Quote_LoadSheduleFor == 3)
                {
                    return "Tracking measurement and frequency";
                }
                else if (Quote_LoadSheduleFor == 4)
                {
                    return "Asset & Tool";
                }
                else if (Quote_LoadSheduleFor == 5)
                {
                    return "Reports";
                }
                else if (Quote_LoadSheduleFor == 6)
                {
                    return "Alerts";
                }
                else if (Quote_LoadSheduleFor == 7)
                {
                    return "Consigned Batch Pull";
                }
                else
                {
                    return "";
                }

            }
            set { }
        }
        public string Quote_ModuleNameResource
        {
            get
            {
                if (Quote_LoadSheduleFor == 1)
                {
                    return ResSchedulerReportList.Transfer;
                }
                else if (Quote_LoadSheduleFor == 2)
                {
                    return ResSchedulerReportList.Order;
                }
                else if (Quote_LoadSheduleFor == 3)
                {
                    return ResSchedulerReportList.TrackingMeasurement;
                }
                else if (Quote_LoadSheduleFor == 4)
                {
                    return ResSchedulerReportList.AssetTool;
                }
                else if (Quote_LoadSheduleFor == 5)
                {
                    return ResSchedulerReportList.ReportsSchedule;
                }
                else if (Quote_LoadSheduleFor == 6)
                {
                    return ResSchedulerReportList.AlertsSchedule;
                }
                else if (Quote_LoadSheduleFor == 7)
                {
                    return ResSchedulerReportList.ConsignedBatchPull;

                }
                else if (Quote_LoadSheduleFor == 12)
                {
                    //   return ResSchedulerReportList.ConsignedBatchPull;
                    return "Quote";

                }
                else
                {
                    return "";
                }

            }
            set { }
        }

        public short? Quote_ReportDataSelectionType { get; set; }
        public long? Quote_ReportDataSince { get; set; }
        public long? Quote_ScheduledBy { get; set; }
        [Display(Name = "ScheduleName", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Quote_ScheduleName { get; set; }
        public int Quote_TotalRecords { get; set; }
        public string Quote_ReportName { get; set; }
        public string Quote_CreatedByName { get; set; }
        public string Quote_UpdatedByName { get; set; }
        public DateTime? Quote_Created { get; set; }
        public List<EmailTemplateDetailDTO> Quote_lstTemplateDtls { get; set; }


        public long Quote_EmailTempateId { get; set; }

        public string Quote_MailBodyText { get; set; }

        public string Quote_MailSubject { get; set; }
        public long[] Quote_ReportIds { get; set; }
        public long[] Quote_EmailTemplateIds { get; set; }
        public string Quote_EmailTemplateDetails { get; set; }

        public bool Quote_IsDeleted { get; set; }

        public DateTime? Quote_Updated { get; set; }
        public bool Quote_RecalcSchedule { get; set; }
        public TimeSpan? Quote_ScheduleTime { get; set; }
        public long? Quote_UserID { get; set; }
    }
    public class RoomScheduleDetailDTO
    {
        public long ID { get; set; }
        public long ScheduleID { get; set; }
        public DateTime ExecuitionDate { get; set; }
        public long SupplierId { get; set; }
        public string SupplierName { get; set; }
        public bool IsScheduleActive { get; set; }
        public long RoomId { get; set; }
        public string RoomName { get; set; }
        public short LoadSheduleFor { get; set; }
        public long CompanyId { get; set; }
        public short SubmissionMethod { get; set; }
        public DateTime? NextRunDate { get; set; }
        public string TimeZoneName { get; set; }
        public long? UserID { get; set; }
    }

    public class RoomScheduleDetailMasterDTO
    {
        public Guid? AssetToolID { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? Created { get; set; }
        public long? CreatedBy { get; set; }
        public short DailyRecurringDays { get; set; }
        public short DailyRecurringType { get; set; }
        public long EnterpriseId { get; set; }
        public short? HourlyAtWhatMinute { get; set; }
        public short? HourlyRecurringHours { get; set; }
        public long ID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsScheduleActive { get; set; }
        public long? LastUpdatedBy { get; set; }
        public short MonthlyDateOfMonth { get; set; }
        public short MonthlyDayOfMonth { get; set; }
        public short MonthlyRecurringMonths { get; set; }
        public short MonthlyRecurringType { get; set; }
        public DateTime? NextRunDate { get; set; }
        public short? ReportDataSelectionType { get; set; }
        public long? ReportDataSince { get; set; }
        public long? ReportID { get; set; }
        public long? RoomId { get; set; }
        public long? ScheduledBy { get; set; }
        public short ScheduleFor { get; set; }
        public long ScheduleID { get; set; }
        public short ScheduleMode { get; set; }
        public string ScheduleName { get; set; }
        public DateTime ScheduleRunTime { get; set; }
        public TimeSpan? ScheduleTime { get; set; }
        public short SubmissionMethod { get; set; }
        public long? SupplierId { get; set; }
        public DateTime? Updated { get; set; }
        public bool WeeklyOnFriday { get; set; }
        public bool WeeklyOnMonday { get; set; }
        public bool WeeklyOnSaturday { get; set; }
        public bool WeeklyOnSunday { get; set; }
        public bool WeeklyOnThursday { get; set; }
        public bool WeeklyOnTuesday { get; set; }
        public bool WeeklyOnWednesday { get; set; }
        public short WeeklyRecurringWeeks { get; set; }
        public string WhatWhereAction { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string EnterpriseDBName { get; set; }
    }
    public class ResSchedulerReportList
    {

        private static string resourceFile = "ResSchedulerReportList";

        /// <summary>
        ///   Looks up a localized string similar to Category.
        /// </summary>
        public static string ScheduleID
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleID", resourceFile);
            }
        }
        public static string HideHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("HideHeader", resourceFile);
            }
        }
        public static string ShowSignature
        {
            get
            {
                return ResourceRead.GetResourceValue("ShowSignature", resourceFile);
            }
        }
        public static string EmailTemplateToken
        {
            get
            {
                return ResourceRead.GetResourceValue("EmailTemplateToken", resourceFile);
            }
        }


        public static string NotificationMode
        {
            get
            {
                return ResourceRead.GetResourceValue("NotificationMode", resourceFile);
            }
        }

        public static string FTPName
        {
            get
            {
                return ResourceRead.GetResourceValue("FTPName", resourceFile);
            }
        }

        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", resourceFile);
            }
        }
        public static string AttachmentTypes
        {
            get
            {
                return ResourceRead.GetResourceValue("AttachmentTypes", resourceFile);
            }
        }

        public static string ScheduleName
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleName", resourceFile);
            }
        }

        public static string ScheduleMode
        {
            get
            {
                return ResourceRead.GetResourceValue("ScheduleMode", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Comment.
        /// </summary>
        public static string ReportName
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportName", resourceFile);
            }
        }
        public static string DataSelectionType
        {
            get
            {
                return ResourceRead.GetResourceValue("DataSelectionType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemNumber.
        /// </summary>
        public static string NextRunDate
        {
            get
            {
                return ResourceRead.GetResourceValue("NextRunDate", resourceFile);
            }
        }
        public static string DatasinceDays
        {
            get
            {
                return ResourceRead.GetResourceValue("DatasinceDays", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to List Type.
        /// </summary>
        public static string CreatedByName
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedByName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Add Item to Quicklist.
        /// </summary>
        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to # of Items.
        /// </summary>
        public static string TotalRecords
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalRecords", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuickList.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: QuickList.
        /// </summary>
        public static string PageTitle
        {
            get
            {

                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        public static string None
        {
            get
            {

                return ResourceRead.GetResourceValue("None", resourceFile);
            }
        }
        public static string Manual
        {
            get
            {

                return ResourceRead.GetResourceValue("Manual", resourceFile);
            }
        }

        public static string Immediate
        {
            get
            {

                return ResourceRead.GetResourceValue("Immediate", resourceFile);
            }
        }

        public static string Hourly
        {
            get
            {

                return ResourceRead.GetResourceValue("Hourly", resourceFile);
            }
        }

        public static string Daily
        {
            get
            {

                return ResourceRead.GetResourceValue("Daily", resourceFile);
            }
        }

        public static string Weekly
        {
            get
            {

                return ResourceRead.GetResourceValue("Weekly", resourceFile);
            }
        }

        public static string Monthly
        {
            get
            {

                return ResourceRead.GetResourceValue("Monthly", resourceFile);
            }
        }
        public static string Every
        {
            get
            {

                return ResourceRead.GetResourceValue("Every", resourceFile);
            }
        }
        public static string EveryWeekDays
        {
            get
            {

                return ResourceRead.GetResourceValue("EveryWeekDays", resourceFile);
            }
        }
        public static string HoursAtEvery
        {
            get
            {

                return ResourceRead.GetResourceValue("HoursAtEvery", resourceFile);
            }
        }
        public static string Recurevery
        {
            get
            {

                return ResourceRead.GetResourceValue("Recurevery", resourceFile);
            }
        }
        public static string WeeksOn
        {
            get
            {

                return ResourceRead.GetResourceValue("WeeksOn", resourceFile);
            }
        }

        public static string Monday
        {
            get
            {

                return ResourceRead.GetResourceValue("Monday", resourceFile);
            }
        }
        public static string Tuesday
        {
            get
            {

                return ResourceRead.GetResourceValue("Tuesday", resourceFile);
            }
        }
        public static string Wednesday
        {
            get
            {

                return ResourceRead.GetResourceValue("Wednesday", resourceFile);
            }
        }
        public static string Thursday
        {
            get
            {

                return ResourceRead.GetResourceValue("Thursday", resourceFile);
            }
        }
        public static string Friday
        {
            get
            {

                return ResourceRead.GetResourceValue("Friday", resourceFile);
            }
        }
        public static string Saturday
        {
            get
            {

                return ResourceRead.GetResourceValue("Saturday", resourceFile);
            }
        }
        public static string Sunday
        {
            get
            {

                return ResourceRead.GetResourceValue("Sunday", resourceFile);
            }
        }
        public static string Months
        {
            get
            {

                return ResourceRead.GetResourceValue("Months", resourceFile);
            }
        }
        public static string OfEvery
        {
            get
            {

                return ResourceRead.GetResourceValue("OfEvery", resourceFile);
            }
        }
        public static string AutoGenerate
        {
            get
            {

                return ResourceRead.GetResourceValue("AutoGenerate", resourceFile);
            }
        }
        public static string AutoGenerateSubmit
        {
            get
            {

                return ResourceRead.GetResourceValue("AutoGenerateSubmit", resourceFile);
            }
        }
        public static string DataselectFromLastReport
        {
            get
            {

                return ResourceRead.GetResourceValue("DataselectFromLastReport", resourceFile);
            }
        }
        public static string DataselectSince
        {
            get
            {

                return ResourceRead.GetResourceValue("DataselectSince", resourceFile);
            }
        }
        public static string First
        {
            get
            {

                return ResourceRead.GetResourceValue("First", resourceFile);
            }
        }
        public static string Second
        {
            get
            {

                return ResourceRead.GetResourceValue("Second", resourceFile);
            }
        }
        public static string Third
        {
            get
            {

                return ResourceRead.GetResourceValue("Third", resourceFile);
            }
        }
        public static string Fourth
        {
            get
            {

                return ResourceRead.GetResourceValue("Fourth", resourceFile);
            }
        }
        public static string Fifth
        {
            get
            {

                return ResourceRead.GetResourceValue("Fifth", resourceFile);
            }
        }
        public static string Minutes
        {
            get
            {

                return ResourceRead.GetResourceValue("Minutes", resourceFile);
            }
        }
        public static string Days
        {
            get
            {

                return ResourceRead.GetResourceValue("Days", resourceFile);
            }
        }

        public static string The
        {
            get
            {

                return ResourceRead.GetResourceValue("The", resourceFile);
            }
        }

        public static string EmailAddress
        {
            get
            {

                return ResourceRead.GetResourceValue("EmailAddress", resourceFile);
            }
        }

        public static string PickReport
        {
            get
            {

                return ResourceRead.GetResourceValue("PickReport", resourceFile);
            }
        }


        public static string EmailSubject
        {
            get
            {

                return ResourceRead.GetResourceValue("EmailSubject", resourceFile);
            }
        }
        public static string SendEmptyEmail
        {
            get
            {

                return ResourceRead.GetResourceValue("SendEmptyEmail", resourceFile);
            }
        }

        public static string CultureCode
        {
            get
            {

                return ResourceRead.GetResourceValue("CultureCode", resourceFile);
            }
        }
        public static string TemplateName
        {
            get
            {

                return ResourceRead.GetResourceValue("TemplateName", resourceFile);
            }
        }

        public static string EmailTemplates
        {
            get
            {

                return ResourceRead.GetResourceValue("EmailTemplates", resourceFile);
            }
        }
        public static string Reports
        {
            get
            {

                return ResourceRead.GetResourceValue("Reports", resourceFile);
            }
        }
        public static string PDF
        {
            get
            {

                return ResourceRead.GetResourceValue("PDF", resourceFile);
            }
        }

        public static string Excel
        {
            get
            {

                return ResourceRead.GetResourceValue("Excel", resourceFile);
            }
        }
        public static string Suppliers
        {
            get
            {

                return ResourceRead.GetResourceValue("Suppliers", resourceFile);
            }
        }

        public static string SuppliersMessage
        {
            get
            {

                return ResourceRead.GetResourceValue("SuppliersMessage", resourceFile);
            }
        }

        public static string SuppliersMessageAllNone
        {
            get
            {

                return ResourceRead.GetResourceValue("SuppliersMessageAllNone", resourceFile);
            }
        }

        public static string ActionMessageAllNone
        {
            get
            {

                return ResourceRead.GetResourceValue("ActionMessageAllNone", resourceFile);
            }
        }

        public static string Transfer
        {
            get
            {

                return ResourceRead.GetResourceValue("Transfer", resourceFile);
            }
        }

        public static string Order
        {
            get
            {

                return ResourceRead.GetResourceValue("Order", resourceFile);
            }
        }

        public static string TrackingMeasurement
        {
            get
            {

                return ResourceRead.GetResourceValue("TrackingMeasurement", resourceFile);
            }
        }

        public static string AssetTool
        {
            get
            {

                return ResourceRead.GetResourceValue("AssetTool", resourceFile);
            }
        }

        public static string ReportsSchedule
        {
            get
            {

                return ResourceRead.GetResourceValue("ReportsSchedule", resourceFile);
            }
        }

        public static string AlertsSchedule
        {
            get
            {

                return ResourceRead.GetResourceValue("AlertsSchedule", resourceFile);
            }
        }

        public static string ConsignedBatchPull
        {
            get
            {

                return ResourceRead.GetResourceValue("ConsignedBatchPull", resourceFile);
            }
        }

        public static string ScheduleDetails
        {
            get
            {

                return ResourceRead.GetResourceValue("ScheduleDetails", resourceFile);
            }
        }
        public static string CopyNotification
        {
            get
            {
                return ResourceRead.GetResourceValue("CopyNotification", resourceFile);
            }
        }

        public static string NotificationName
        {
            get
            {
                return ResourceRead.GetResourceValue("NotificationName", resourceFile);
            }
        }

        public static string DataselectFromFirstOfMonth
        {
            get
            {

                return ResourceRead.GetResourceValue("DataselectFromFirstOfMonth", resourceFile);
            }
        }

        //
        public static string SelectAllRangeData
        {
            get
            {

                return ResourceRead.GetResourceValue("SelectAllRangeData", resourceFile);
            }
        }

        //
        public static string IsScheduleActive
        {
            get
            {

                return ResourceRead.GetResourceValue("IsScheduleActive", resourceFile);
            }
        }

        public static string Pull_IsScheduleActive
        {
            get
            {

                return ResourceRead.GetResourceValue("Pull_IsScheduleActive", resourceFile);
            }
        }

        public static string IsIncludeStockouttool
        {
            get
            {
                return ResourceRead.GetResourceValue("IsIncludeStockouttool", resourceFile);
            }
        }
        public static string ApproverRequesterEmaillNote
        {
            get
            {
                return ResourceRead.GetResourceValue("ApproverRequesterEmaillNote", resourceFile);
            }
        }

        public static string Copy
        {
            get
            {
                return ResourceRead.GetResourceValue("Copy", resourceFile);
            }
        }
        public static string MoveType
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveType", resourceFile);
            }
        }
        public static string ReqSupllierToScheduleReport
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqSupllierToScheduleReport", resourceFile);
            }
        }
        public static string ReqRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRoom", resourceFile);
            }
        }
        public static string ReqFromDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqFromDate", resourceFile);
            }
        }
        public static string ReqToEmailAddress
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqToEmailAddress", resourceFile);
            }
        }
        public static string ReqValidToEmailAddress
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqValidToEmailAddress", resourceFile);
            }
        }
        public static string ReqSubject
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqSubject", resourceFile);
            }
        }
        public static string ReqBody
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqBody", resourceFile);
            }
        }
        public static string ReqAttachmentTypes
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqAttachmentTypes", resourceFile);
            }
        }
        public static string Reportrunsuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("Reportrunsuccessfully", resourceFile);
            }
        }
        public static string RunReportUsedIn
        {
            get
            {
                return ResourceRead.GetResourceValue("RunReportUsedIn", resourceFile);
            }
        }
        public static string ReqRangeValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRangeValue", resourceFile);
            }
        }
        public static string ReqCompany
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqCompany", resourceFile);
            }
        }
        public static string MsgEnterValidCCEmail
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterValidCCEmail", resourceFile);
            }
        }

        public static string AutoSettings { get { return ResourceRead.GetResourceValue("AutoSettings", resourceFile); } }
        public static string ScheduleRunTime { get { return ResourceRead.GetResourceValue("ScheduleRunTime", resourceFile); } }
        public static string PullScheduleRunTime { get { return ResourceRead.GetResourceValue("PullScheduleRunTime", resourceFile); } }
        public static string Zero { get { return ResourceRead.GetResourceValue("Zero", resourceFile); } }
        public static string One { get { return ResourceRead.GetResourceValue("One", resourceFile); } }
        public static string Two { get { return ResourceRead.GetResourceValue("Two", resourceFile); } }
        public static string Three { get { return ResourceRead.GetResourceValue("Three", resourceFile); } }
        public static string Four { get { return ResourceRead.GetResourceValue("Four", resourceFile); } }
        public static string Five { get { return ResourceRead.GetResourceValue("Five", resourceFile); } }
        public static string Six { get { return ResourceRead.GetResourceValue("Six", resourceFile); } }
        public static string Seven { get { return ResourceRead.GetResourceValue("Seven", resourceFile); } }
        public static string Eight { get { return ResourceRead.GetResourceValue("Eight", resourceFile); } }
        public static string Nine { get { return ResourceRead.GetResourceValue("Nine", resourceFile); } }
        public static string Ten { get { return ResourceRead.GetResourceValue("Ten", resourceFile); } }
        public static string Eleven { get { return ResourceRead.GetResourceValue("Eleven", resourceFile); } }
        public static string Twelve { get { return ResourceRead.GetResourceValue("Twelve", resourceFile); } }
        public static string Thirteen { get { return ResourceRead.GetResourceValue("Thirteen", resourceFile); } }
        public static string Fourteen { get { return ResourceRead.GetResourceValue("Fourteen", resourceFile); } }
        public static string Fifteen { get { return ResourceRead.GetResourceValue("Fifteen", resourceFile); } }
        public static string Sixteen { get { return ResourceRead.GetResourceValue("Sixteen", resourceFile); } }
        public static string Seventeen { get { return ResourceRead.GetResourceValue("Seventeen", resourceFile); } }
        public static string Eighteen { get { return ResourceRead.GetResourceValue("Eighteen", resourceFile); } }
        public static string Nineteen { get { return ResourceRead.GetResourceValue("Nineteen", resourceFile); } }
        public static string Twenty { get { return ResourceRead.GetResourceValue("Twenty", resourceFile); } }
        public static string TwentyOne { get { return ResourceRead.GetResourceValue("TwentyOne", resourceFile); } }
        public static string TwentyTwo { get { return ResourceRead.GetResourceValue("TwentyTwo", resourceFile); } }
        public static string TwentyThree { get { return ResourceRead.GetResourceValue("TwentyThree", resourceFile); } }
        public static string TwentyFour { get { return ResourceRead.GetResourceValue("TwentyFour", resourceFile); } }
        public static string TwentyFive { get { return ResourceRead.GetResourceValue("TwentyFive", resourceFile); } }
        public static string TwentySix { get { return ResourceRead.GetResourceValue("TwentySix", resourceFile); } }
        public static string TwentySeven { get { return ResourceRead.GetResourceValue("TwentySeven", resourceFile); } }
        public static string TwentyEight { get { return ResourceRead.GetResourceValue("TwentyEight", resourceFile); } }
        public static string TwentyNine { get { return ResourceRead.GetResourceValue("TwentyNine", resourceFile); } }
        public static string Thirty { get { return ResourceRead.GetResourceValue("Thirty", resourceFile); } }
        public static string ThirtyOne { get { return ResourceRead.GetResourceValue("ThirtyOne", resourceFile); } }
        public static string ThirtyTwo { get { return ResourceRead.GetResourceValue("ThirtyTwo", resourceFile); } }
        public static string ThirtyThree { get { return ResourceRead.GetResourceValue("ThirtyThree", resourceFile); } }
        public static string ThirtyFour { get { return ResourceRead.GetResourceValue("ThirtyFour", resourceFile); } }
        public static string ThirtyFive { get { return ResourceRead.GetResourceValue("ThirtyFive", resourceFile); } }
        public static string ThirtySix { get { return ResourceRead.GetResourceValue("ThirtySix", resourceFile); } }
        public static string ThirtySeven { get { return ResourceRead.GetResourceValue("ThirtySeven", resourceFile); } }
        public static string ThirtyEight { get { return ResourceRead.GetResourceValue("ThirtyEight", resourceFile); } }
        public static string ThirtyNine { get { return ResourceRead.GetResourceValue("ThirtyNine", resourceFile); } }
        public static string Forty { get { return ResourceRead.GetResourceValue("Forty", resourceFile); } }
        public static string FortyOne { get { return ResourceRead.GetResourceValue("FortyOne", resourceFile); } }
        public static string FortyTwo { get { return ResourceRead.GetResourceValue("FortyTwo", resourceFile); } }
        public static string FortyThree { get { return ResourceRead.GetResourceValue("FortyThree", resourceFile); } }
        public static string FortyFour { get { return ResourceRead.GetResourceValue("FortyFour", resourceFile); } }
        public static string FortyFive { get { return ResourceRead.GetResourceValue("FortyFive", resourceFile); } }
        public static string FortySix { get { return ResourceRead.GetResourceValue("FortySix", resourceFile); } }
        public static string FortySeven { get { return ResourceRead.GetResourceValue("FortySeven", resourceFile); } }
        public static string FortyEight { get { return ResourceRead.GetResourceValue("FortyEight", resourceFile); } }
        public static string FortyNine { get { return ResourceRead.GetResourceValue("FortyNine", resourceFile); } }
        public static string Fifty { get { return ResourceRead.GetResourceValue("Fifty", resourceFile); } }
        public static string FiftyOne { get { return ResourceRead.GetResourceValue("FiftyOne", resourceFile); } }
        public static string FiftyTwo { get { return ResourceRead.GetResourceValue("FiftyTwo", resourceFile); } }
        public static string FiftyThree { get { return ResourceRead.GetResourceValue("FiftyThree", resourceFile); } }
        public static string FiftyFour { get { return ResourceRead.GetResourceValue("FiftyFour", resourceFile); } }
        public static string FiftyFive { get { return ResourceRead.GetResourceValue("FiftyFive", resourceFile); } }
        public static string FiftySix { get { return ResourceRead.GetResourceValue("FiftySix", resourceFile); } }
        public static string FiftySeven { get { return ResourceRead.GetResourceValue("FiftySeven", resourceFile); } }
        public static string FiftyEight { get { return ResourceRead.GetResourceValue("FiftyEight", resourceFile); } }
        public static string FiftyNine { get { return ResourceRead.GetResourceValue("FiftyNine", resourceFile); } }
        public static string Now { get { return ResourceRead.GetResourceValue("Now", resourceFile); } }
        public static string Done { get { return ResourceRead.GetResourceValue("Done", resourceFile); } }
        public static string ChooseTime { get { return ResourceRead.GetResourceValue("ChooseTime", resourceFile); } }
        public static string Time { get { return ResourceRead.GetResourceValue("Time", resourceFile); } }
        public static string Hour { get { return ResourceRead.GetResourceValue("Hour", resourceFile); } }
        public static string Minute { get { return ResourceRead.GetResourceValue("Minute", resourceFile); } }
        public static string Seconds { get { return ResourceRead.GetResourceValue("Seconds", resourceFile); } }
        public static string Millisecond { get { return ResourceRead.GetResourceValue("Millisecond", resourceFile); } }
        public static string DisplayAllCheckedOutTools { get { return ResourceRead.GetResourceValue("DisplayAllCheckedOutTools", resourceFile); } }
        public static string SelectedCompany { get { return ResourceRead.GetResourceValue("SelectedCompany", resourceFile); } }
        public static string SelectedRoom { get { return ResourceRead.GetResourceValue("SelectedRoom", resourceFile); } }
        public static string ImmediateActions { get { return ResourceRead.GetResourceValue("ImmediateActions", resourceFile); } }
        public static string FilterQOH { get { return ResourceRead.GetResourceValue("FilterQOH", resourceFile); } }
        public static string SelectedQOHFilters { get { return ResourceRead.GetResourceValue("SelectedQOHFilters", resourceFile); } }
        public static string MonthlyUsages { get { return ResourceRead.GetResourceValue("MonthlyUsages", resourceFile); } }
        public static string OnlyExpirationItems { get { return ResourceRead.GetResourceValue("OnlyExpirationItems", resourceFile); } }
        public static string ItemStatus { get { return ResourceRead.GetResourceValue("ItemStatus", resourceFile); } }
        public static string Active { get { return ResourceRead.GetResourceValue("Active", resourceFile); } }
        public static string InActive { get { return ResourceRead.GetResourceValue("InActive", resourceFile); } }
        public static string Range { get { return ResourceRead.GetResourceValue("Range", resourceFile); } }
        public static string InStock { get { return ResourceRead.GetResourceValue("InStock", resourceFile); } }
        public static string WorkOrderStatus { get { return ResourceRead.GetResourceValue("WorkOrderStatus", resourceFile); } }
        public static string AppliedFilter { get { return ResourceRead.GetResourceValue("AppliedFilter", resourceFile); } }
        public static string AppliedOnly { get { return ResourceRead.GetResourceValue("AppliedOnly", resourceFile); } }
        public static string NotAppliedOnly { get { return ResourceRead.GetResourceValue("NotAppliedOnly", resourceFile); } }
        public static string QuantityType { get { return ResourceRead.GetResourceValue("QuantityType", resourceFile); } }
        public static string UsageType { get { return ResourceRead.GetResourceValue("UsageType", resourceFile); } }
        public static string CombineAllLocation { get { return ResourceRead.GetResourceValue("CombineAllLocation", resourceFile); } }
        public static string SeparateByLocation { get { return ResourceRead.GetResourceValue("SeparateByLocation", resourceFile); } }
        public static string AllowIncludeItemsWithZeroPullUsage { get { return ResourceRead.GetResourceValue("AllowIncludeItemsWithZeroPullUsage", resourceFile); } }
        public static string OnlyAvailableTools { get { return ResourceRead.GetResourceValue("OnlyAvailableTools", resourceFile); } }
        public static string OnlyExpiredItems { get { return ResourceRead.GetResourceValue("OnlyExpiredItems", resourceFile); } }
        public static string DaysUntilItemExpires { get { return ResourceRead.GetResourceValue("DaysUntilItemExpires", resourceFile); } }
        public static string DaysToApproveOrder { get { return ResourceRead.GetResourceValue("DaysToApproveOrder", resourceFile); } }
        public static string ProjectExpirationDate { get { return ResourceRead.GetResourceValue("ProjectExpirationDate", resourceFile); } }
        public static string CartType { get { return ResourceRead.GetResourceValue("CartType", resourceFile); } }
        public static string SortFieldFirstValue { get { return ResourceRead.GetResourceValue("SortFieldFirstValuev", resourceFile); } }
        public static string SortFieldSecondValue { get { return ResourceRead.GetResourceValue("SortFieldSecondValue", resourceFile); } }
        public static string SortFieldThirdValue { get { return ResourceRead.GetResourceValue("SortFieldThirdValue", resourceFile); } }
        public static string SortFieldFourthValue { get { return ResourceRead.GetResourceValue("SortFieldFourthValue", resourceFile); } }
        public static string SortFieldFifthValue { get { return ResourceRead.GetResourceValue("SortFieldFifthValue", resourceFile); } }
        public static string AddToken { get { return ResourceRead.GetResourceValue("AddToken", resourceFile); } }
        public static string Run { get { return ResourceRead.GetResourceValue("Run", resourceFile); } }



    }

}
