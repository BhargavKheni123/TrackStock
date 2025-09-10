using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class NotificationDTO
    {

        public long ID { get; set; }
        public long RoomScheduleID { get; set; }
        public string SupplierIds { get; set; }

        [Display(Name = "AttachmentTypes", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string AttachmentTypes { get; set; }
        public string AttachmentReportIDs { get; set; }
        public int? TotalRecords { get; set; }
        public long? EmailTemplateID { get; set; }
        public long? ReportID { get; set; }
        public SchedulerDTO SchedulerParams { get; set; }
        public EmailTemplateDTO EmailTemplateDetail { get; set; }
        public ReportBuilderDTO ReportMasterDTO { get; set; }
        public ReportBuilderDTO AttachedReportMasterDTO { get; set; }
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string CreatedByDate { get; set; }
        public string UpdatedByDate { get; set; }
        public IEnumerable<EmailTemplateDetailDTO> lstEmailTemplateDtls { get; set; }
        public string EmailTemplateDetails { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? NextRunDate { get; set; }
        public int ScheduleFor { get; set; }
        public short? ScheduleMode { get; set; }

        public string TemplateName { get; set; }
        public string ReportName { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResSchedulerReportList))]
        public bool IsActive { get; set; }

        [Display(Name = "ScheduleName", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string ScheduleName { get; set; }

        [Display(Name = "EmailTemplates", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string EmailTemplates { get; set; }

        [Display(Name = "NotificationName", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string NotificationName { get; set; }

        [Display(Name = "Suppliers", ResourceType = typeof(ResSchedulerReportList))]
        public string Suppliers { get; set; }

        [Display(Name = "CultureCode", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string CultureCode { get; set; }


        [Display(Name = "EmailSubject", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string EmailSubject { get; set; }

        [Display(Name = "Reports", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string Reports { get; set; }

        [MultiEmails("EmailAddress", ErrorMessageResourceName = "MultiEmail", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(4096, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string EmailAddress { get; set; }

        public eTurnsRegionInfo objeTurnsRegionInfo { get; set; }

        public TimeSpan? ScheduleTime { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }

        public short? ReportDataSelectionType { get; set; }

        public long? ReportDataSince { get; set; }

        public string ScheduleRunTime { get; set; }

        public DateTime ScheduleRunDateTime { get; set; }
        [Display(Name = "NotificationMode", ResourceType = typeof(ResSchedulerReportList))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int NotificationMode { get; set; }

        [Display(Name = "FTPName", ResourceType = typeof(ResSchedulerReportList))]
        public string FTPName { get; set; }

        public FTPMasterDTO FtpDetails { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public long? FTPId { get; set; }
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
        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "SendEmptyEmail", ResourceType = typeof(ResSchedulerReportList))]
        public bool SendEmptyEmail { get; set; }
        [Display(Name = "HideHeader", ResourceType = typeof(ResSchedulerReportList))]
        public bool HideHeader { get; set; }
        [Display(Name = "ShowSignature", ResourceType = typeof(ResSchedulerReportList))]
        public bool ShowSignature { get; set; }


        public List<KeyValDTO> SortFields { get; set; }
        public List<KeyValDTO> SortOrders { get; set; }
        public string SortSequence { get; set; }

        [Display(Name = "SortFieldFirstValue", ResourceType = typeof(ResSchedulerReportList))]
        public string SortFieldFirstValue { get; set; }
        [Display(Name = "SortFieldSecondValue", ResourceType = typeof(ResSchedulerReportList))]
        public string SortFieldSecondValue { get; set; }
        [Display(Name = "SortFieldThirdValue", ResourceType = typeof(ResSchedulerReportList))]
        public string SortFieldThirdValue { get; set; }
        [Display(Name = "SortFieldFourthValue", ResourceType = typeof(ResSchedulerReportList))]
        public string SortFieldFourthValue { get; set; }
        [Display(Name = "SortFieldFifthValue", ResourceType = typeof(ResSchedulerReportList))]
        public string SortFieldFifthValue { get; set; }

        [Display(Name = "EmailTemplateToken", ResourceType = typeof(ResSchedulerReportList))]
        public string EmailTemplateToken { get; set; }

        public string SortFieldFirstOrder { get; set; }
        public string SortFieldSecondOrder { get; set; }
        public string SortFieldThirdOrder { get; set; }
        public string SortFieldFourthOrder { get; set; }
        public string SortFieldFifthOrder { get; set; }

        public string XMLValue { get; set; }
        public string RoomIds { get; set; }

        public string Status { get; set; }
        public string CompanyIds { get; set; }
        public string Range { get; set; }

        public string WOStatus { get; set; }
        public string OrderStatus { get; set; }

        public string ItemStatus { get; set; }

        public string QtyType { get; set; }

        public string FilterQOH { get; set; }

        public string ReportRange { get; set; }

        public string ReportRangeData { get; set; }

        [Display(Name = "SelectAllRangeData", ResourceType = typeof(ResSchedulerReportList))]
        public bool SelectAllRangeData { get; set; }

        public long? MonthlyAverageUsage { get; set; }

        public bool OnlyExpirationItems { get; set; }

        public string ActionCodes { get; set; }
        public string ActionCode { get; set; }
        public bool OnlyExpiredItems { get; set; }
        public string DaysUntilItemExpires { get; set; }
        public string DaysToApproveOrder { get; set; }
        public string ProjectExpirationDate { get; set; }

        public bool OnlyAvailableTools { get; set; }
        public string CountAppliedFilter { get; set; }

        public string UsageType { get; set; }

        public DateTime? FromdDate { get; set; }

        private string _fromDate;

        public string FromdDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_fromDate))
                {
                    _fromDate = FnCommon.ConvertDateByTimeZone(FromdDate, false, true);
                }
                return _fromDate;
            }
            set { this._fromDate = value; }
        }

        public string ParentReportName { get; set; }

        public ReportAlertScheduleRunHistoryDTO ReportAlertScheduleInfo { get; set; }

        public string CartType { get; set; }

        [Display(Name = "IsIncludeStockouttool", ResourceType = typeof(ResSchedulerReportList))]
        public bool IsIncludeStockouttool { get; set; }

        public string ResourceKey { get; set; }

        private string _reportResourceName;
        public string ReportResourceName
        {
            get
            {
                if (!string.IsNullOrEmpty(ResourceKey))
                {
                    _reportResourceName = ResourceHelper.GetReportNameByResource(ResourceKey);
                }
                else
                {
                    _reportResourceName = ReportName;
                }
                return _reportResourceName;
            }
            set { this._reportResourceName = value; }
        }

        public string ResourceKeyName { get; set; }
        private string _alertResourceName;
        public string AlertResourceName
        {
            get
            {
                if (!string.IsNullOrEmpty(ResourceKeyName))
                {
                    _alertResourceName = ResourceHelper.GetAlertNameByResource(ResourceKeyName);
                }
                else
                {
                    _alertResourceName = TemplateName;
                }
                return _alertResourceName;
            }
            set { this._alertResourceName = value; }
        }

        public bool IsSupplierRequired { get; set;}
        public bool IsDateRangeRequired { get; set; }
        public bool IsAllowedZeroPullUsage { get; set; }

        public string MoveType { get; set; }
        public bool ExcludeZeroOrdQty { get; set; }

        [Display(Name = "DisplayAllCheckedOutTools", ResourceType = typeof(ResSchedulerReportList))]
        public bool AllCheckedOutTools { get; set; }
    }

    public class NotificationMasterDTO
    {
        public long ID { get; set; }
        public long NotificationID { get; set; }
        public long RoomScheduleID { get; set; }
        public long? EmailTemplateID { get; set; }
        public long? ReportID { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public long EnterpriseId { get; set; }
        public long? ReportDataSince { get; set; }
        public long? FTPId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool? SendEmptyEmail { get; set; }
        public bool HideHeader { get; set; }
        public bool ShowSignature { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? NextRunDate { get; set; }
        public DateTime? ScheduleRunTime { get; set; }
        public int ScheduleFor { get; set; }
        public int NotificationMode { get; set; }
        public string EmailAddress { get; set; }
        public string WhatWhereAction { get; set; }
        public string SortSequence { get; set; }
        public string XMLValue { get; set; }
        public string RoomIDs { get; set; }
        public string CompanyIDs { get; set; }
        public string Range { get; set; }
        public string Status { get; set; }
        public short? ReportDataSelectionType { get; set; }
        public TimeSpan? ScheduleTime { get; set; }
        public string SupplierIds { get; set; }
        public string AttachmentTypes { get; set; }
        public string AttachmentReportIDs { get; set; }
        public string EnterpriseName { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public string EnterpriseDBName { get; set; }
    }
}
