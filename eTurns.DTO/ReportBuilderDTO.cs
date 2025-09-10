using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class ReportBuilderDTO
    {
        public string ReportName { get; set; }
        public long ID { get; set; }
        public string ReportFileName { get; set; }
        public string SubReportFileName { get; set; }
        public int ReportType { get; set; }
        public string SortColumns { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsBaseReport { get; set; }
        public long? PrivateUserID { get; set; }
        public long CompanyID { get; set; }
        public long? RoomID { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public string MasterReportResFile { get; set; }
        public string SubReportResFile { get; set; }
        public bool IsIncludeDateRange { get; set; }
        public bool IsIncludeTotal { get; set; }
        public bool IsIncludeSubTotal { get; set; }
        public bool IsIncludeGrandTotal { get; set; }
        public bool IsIncludeGroup { get; set; }
        public string GroupName { get; set; }
        public int? Days { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? ParentID { get; set; }
        public string ChildReportName { get; set; }
        public string ToEmailAddress { get; set; }
        public string ModuleName { get; set; }
        public bool? ISEnterpriseReport { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public bool? SetAsDefaultPrintReport { get; set; }

        public bool? IsIncludeTax1 { get; set; }
        public bool? IsIncludeTax2 { get; set; }
        public bool? IsNotEditable { get; set; }

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

        public Nullable<Boolean> HideHeader { get; set; }
        public Nullable<Boolean> ShowSignature { get; set; }

        public string ParentReportName { get; set; }
        public string ReportAppIntent { get; set; }

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

        public bool IsSupplierRequired { get; set;}
        public bool IsDateRangeRequired { get; set; }        
        public string CombineReportID { get; set; }
    }

    public class ReportPerameters
    {
        public bool HasStartDate { get; set; }
        public bool HasEndDate { get; set; }
        public bool HasRoomIds { get; set; }
        public bool HasCompanyIds { get; set; }

        public List<KeyValDTO> RoomList { get; set; }
        public List<KeyValDTO> CompanyList { get; set; }

        public List<KeyValDTO> SortFields { get; set; }
        public List<KeyValDTO> SortOrders { get; set; }

        public string SortFieldFirstValue { get; set; }
        public string SortFieldSecondValue { get; set; }
        public string SortFieldThirdValue { get; set; }
        public string SortFieldFourthValue { get; set; }
        public string SortFieldFifthValue { get; set; }

        public string SortFieldFirstOrder { get; set; }
        public string SortFieldSecondOrder { get; set; }
        public string SortFieldThirdOrder { get; set; }
        public string SortFieldFourthOrder { get; set; }
        public string SortFieldFifthOrder { get; set; }
        public string ModuleName { get; set; }
        public string ReportFileName { get; set; }
        public Int64 Id { get; set; }
        public bool IsUserCanDelete { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDateStr
        {
            get;
            set;
        }

        public string EndDateStr
        {
            get;
            set;
        }
        public long? MonthlyAverageUsage { get; set; }

        public string ParentReportName { get; set; }

        public int? AUDayOfUsageToSample { get; set; }
        public byte? AUMeasureMethod { get; set; }
        public int? MinMaxDayOfAverage { get; set; }
        public double? MinMaxMinNumberOfTimesMax { get; set; }
        public string QuantumStartDate { get; set; }
        public string QuantumEndDate { get; set; }
        public int? MinMaxDayOfUsageToSample { get; set; }
        public byte? MinMaxMeasureMethod { get; set; }
        public int DecimalPointFromConfig { get; set; }
    }
    public class ReportGroupMasterDTO
    {
        public Int64 Id { get; set; }
        public Int64 ReportID { get; set; }
        public string FieldName { get; set; }

        public string FieldColumnID { get; set; }

        public Int64 GroupOrder { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
    }
    public class ReportFilterParam
    {
        public string FieldName { get; set; }
        public List<KeyValCheckDTO> FieldFilterName { get; set; }
        public string FieldDisplayName { get; set; }
    }
    public class KeyValSelectDTO
    {
        public string key { get; set; }
        public string value { get; set; }
        public bool IsSelect { get; set; }
    }
    public class ReportScheduleDTO
    {
        public long ID { get; set; }
        public long ScheduleID { get; set; }
        public DateTime ExecuitionDate { get; set; }
        public bool IsScheduleActive { get; set; }
        public long RoomId { get; set; }
        public short LoadSheduleFor { get; set; }
        public long CompanyId { get; set; }
        public long ReportId { get; set; }
        public DateTime? NextRundate { get; set; }
        public short? ReportDataSelectionType { get; set; }
        public long? ReportDataSince { get; set; }
    }
    public class ReportMailLogDTO
    {
        public long Id { get; set; }
        public long ReportID { get; set; }
        public long ScheduleID { get; set; }
        public int? AttachmentCount { get; set; }
        public string SendEmailAddress { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long NotificationID { get; set; }
        public DateTime SendDate { get; set; }
    }
    public class AlertReportDTO
    {
        public string ID { get; set; }
        public int ScheduleFor { get; set; }
        public string AlertReportName { get; set; }

    }

    public class ReportMasterDTO
    {
        [Display(Name = "CompanyID", ResourceType = typeof(ResReportMaster))]
        public long CompanyID { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResReportMaster))]
        public long CreatedBy { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResReportMaster))]
        public DateTime CreatedOn { get; set; }


        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(CreatedOn, true);
                }
                return _createdDate;
            }
            set { this._createdDate = value; }
        }


        [Display(Name = "Days", ResourceType = typeof(ResReportMaster))]
        public int? Days { get; set; }

        [Display(Name = "FromDate", ResourceType = typeof(ResReportMaster))]
        public DateTime? FromDate { get; set; }

        [Display(Name = "GroupName", ResourceType = typeof(ResReportMaster))]
        public string GroupName { get; set; }

        [Display(Name = "HideHeader", ResourceType = typeof(ResReportMaster))]
        public bool? HideHeader { get; set; }

        [Display(Name = "ID", ResourceType = typeof(ResReportMaster))]
        public long ID { get; set; }

        [Display(Name = "IsArchived", ResourceType = typeof(ResReportMaster))]
        public bool? IsArchived { get; set; }

        [Display(Name = "IsBaseReport", ResourceType = typeof(ResReportMaster))]
        public bool IsBaseReport { get; set; }

        [Display(Name = "IsDefaultReport", ResourceType = typeof(ResReportMaster))]
        public bool? IsDefaultReport { get; set; }

        [Display(Name = "IsDeleted", ResourceType = typeof(ResReportMaster))]
        public bool? IsDeleted { get; set; }

        [Display(Name = "ISEnterpriseReport", ResourceType = typeof(ResReportMaster))]
        public bool? ISEnterpriseReport { get; set; }

        [Display(Name = "IsIncludeDateRange", ResourceType = typeof(ResReportMaster))]
        public bool IsIncludeDateRange { get; set; }

        [Display(Name = "IsIncludeGrandTotal", ResourceType = typeof(ResReportMaster))]
        public bool IsIncludeGrandTotal { get; set; }

        [Display(Name = "IsIncludeGroup", ResourceType = typeof(ResReportMaster))]
        public bool IsIncludeGroup { get; set; }

        [Display(Name = "IsIncludeSubTotal", ResourceType = typeof(ResReportMaster))]
        public bool IsIncludeSubTotal { get; set; }

        [Display(Name = "IsIncludeTax1", ResourceType = typeof(ResReportMaster))]
        public bool? IsIncludeTax1 { get; set; }

        [Display(Name = "IsIncludeTax2", ResourceType = typeof(ResReportMaster))]
        public bool? IsIncludeTax2 { get; set; }

        [Display(Name = "IsIncludeTotal", ResourceType = typeof(ResReportMaster))]
        public bool IsIncludeTotal { get; set; }

        [Display(Name = "IsNotEditable", ResourceType = typeof(ResReportMaster))]
        public bool? IsNotEditable { get; set; }

        [Display(Name = "IsPrivate", ResourceType = typeof(ResReportMaster))]
        public bool IsPrivate { get; set; }

        [Display(Name = "MasterReportResFile", ResourceType = typeof(ResReportMaster))]
        public string MasterReportResFile { get; set; }

        [Display(Name = "ModuleName", ResourceType = typeof(ResReportMaster))]
        public string ModuleName { get; set; }

        [Display(Name = "ParentID", ResourceType = typeof(ResReportMaster))]
        public long? ParentID { get; set; }

        [Display(Name = "PrivateUserID", ResourceType = typeof(ResReportMaster))]
        public long? PrivateUserID { get; set; }

        [Display(Name = "ReportFileName", ResourceType = typeof(ResReportMaster))]
        public string ReportFileName { get; set; }

        [Display(Name = "ReportName", ResourceType = typeof(ResReportMaster))]
        public string ReportName { get; set; }

        [Display(Name = "ReportType", ResourceType = typeof(ResReportMaster))]
        public int ReportType { get; set; }

        [Display(Name = "RoomID", ResourceType = typeof(ResReportMaster))]
        public long? RoomID { get; set; }

        [Display(Name = "SetAsDefaultPrintReport", ResourceType = typeof(ResReportMaster))]
        public bool? SetAsDefaultPrintReport { get; set; }

        [Display(Name = "ShowSignature", ResourceType = typeof(ResReportMaster))]
        public bool? ShowSignature { get; set; }

        [Display(Name = "SortColumns", ResourceType = typeof(ResReportMaster))]
        public string SortColumns { get; set; }

        [Display(Name = "SubReportFileName", ResourceType = typeof(ResReportMaster))]
        public string SubReportFileName { get; set; }

        [Display(Name = "SubReportResFile", ResourceType = typeof(ResReportMaster))]
        public string SubReportResFile { get; set; }

        [Display(Name = "ToDate", ResourceType = typeof(ResReportMaster))]
        public DateTime? ToDate { get; set; }

        [Display(Name = "ToEmailAddress", ResourceType = typeof(ResReportMaster))]
        public string ToEmailAddress { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResReportMaster))]
        public long UpdatedBy { get; set; }

        [Display(Name = "UpdatedON", ResourceType = typeof(ResReportMaster))]
        public DateTime UpdatedON { get; set; }

        private string _updatedDate;
        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedDate))
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(UpdatedON, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }

        [Display(Name = "CreatedByName", ResourceType = typeof(ResReportMaster))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedByName", ResourceType = typeof(ResReportMaster))]
        public string UpdatedByName { get; set; }

        [Display(Name = "CompanyName", ResourceType = typeof(ResReportMaster))]
        public string CompanyName { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResReportMaster))]
        public string RoomName { get; set; }
        public long TotalRecords { get; set; }

        public long RowNum { get; set; }

        [Display(Name = "AllowAttachmentSelection", ResourceType = typeof(ResReportMaster))]
        public bool AllowAttachmentSelection { get; set; }

        [Display(Name = "AllowDataSelectFirstOfMonth", ResourceType = typeof(ResReportMaster))]
        public bool AllowDataSelectFirstOfMonth { get; set; }

        [Display(Name = "AllowDataSelectSinceLastReportFilter", ResourceType = typeof(ResReportMaster))]
        public bool AllowDataSelectSinceLastReportFilter { get; set; }

        [Display(Name = "AllowDataSinceFilter", ResourceType = typeof(ResReportMaster))]
        public bool AllowDataSinceFilter { get; set; }

        [Display(Name = "AllowedAttahmentReports", ResourceType = typeof(ResReportMaster))]
        public string AllowedAttahmentReports { get; set; }

        [Display(Name = "AllowScheduleDaily", ResourceType = typeof(ResReportMaster))]
        public bool AllowScheduleDaily { get; set; }

        [Display(Name = "AllowedIMMActions", ResourceType = typeof(ResReportMaster))]
        public string AllowedIMMActions { get; set; }

        [Display(Name = "AllowExcelAttachment", ResourceType = typeof(ResReportMaster))]
        public bool AllowExcelAttachment { get; set; }

        [Display(Name = "AllowPDFAttachment", ResourceType = typeof(ResReportMaster))]
        public bool AllowPDFAttachment { get; set; }

        [Display(Name = "AllowScheduleHourly", ResourceType = typeof(ResReportMaster))]
        public bool AllowScheduleHourly { get; set; }

        [Display(Name = "AllowScheduleIMM", ResourceType = typeof(ResReportMaster))]
        public bool AllowScheduleIMM { get; set; }

        [Display(Name = "AllowScheduleMonthly", ResourceType = typeof(ResReportMaster))]
        public bool AllowScheduleMonthly { get; set; }

        [Display(Name = "AllowScheduleWeekly", ResourceType = typeof(ResReportMaster))]
        public bool AllowScheduleWeekly { get; set; }

        [Display(Name = "AllowSupplierFilter", ResourceType = typeof(ResReportMaster))]
        public bool AllowSupplierFilter { get; set; }

        [Display(Name = "EmailTemplateID", ResourceType = typeof(ResReportMaster))]
        public long EmailTemplateID { get; set; }

        [Display(Name = "AlertConfigID", ResourceType = typeof(ResReportMaster))]
        public long AlertConfigID { get; set; }

        [Display(Name = "EmailTemplateName", ResourceType = typeof(ResReportMaster))]
        public string EmailTemplateName { get; set; }

        [Display(Name = "ItemType", ResourceType = typeof(ResReportMaster))]
        public string ItemType { get; set; }


        [Display(Name = "AllowRangeDataSelect", ResourceType = typeof(ResReportMaster))]
        public bool AllowRangeDataSelect { get; set; }

        public string ParentReportName { get; set; }

        public string AllowedAttahmentReportsWithChild { get; set; }
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
                    _alertResourceName = EmailTemplateName;
                }
                return _alertResourceName;
            }
            set { this._alertResourceName = value; }
        }
    
        [Display(Name = "IsSupplierRequired", ResourceType = typeof(ResReportMaster))]
        public bool IsSupplierRequired { get; set; }

        [Display(Name = "IsDateRangeRequired", ResourceType = typeof(ResReportMaster))]
        public bool IsDateRangeRequired { get; set; }
    }

    public class TransactionEventMasterDTO
    {
        public long ID { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
    }

    public class ReportAlertConfigDTO
    {
        public bool AllowAttachmentSelection { get; set; }
        public bool AllowDataSelectSinceLastReportFilter { get; set; }

        public bool AllowDataSelectFirstOfMonth { get; set; }

        public bool AllowDataSinceFilter { get; set; }
        public string AllowedAttahmentReports { get; set; }
        public string AllowedIMMActions { get; set; }
        public bool AllowExcelAttachment { get; set; }
        public bool AllowPDFAttachment { get; set; }
        public bool AllowScheduleHourly { get; set; }
        public bool AllowScheduleIMM { get; set; }
        public bool AllowScheduleMonthly { get; set; }
        public bool AllowScheduleWeekly { get; set; }
        public bool AllowSupplierFilter { get; set; }
        public long EmailTemplateID { get; set; }
        public long ID { get; set; }
        public long ReportID { get; set; }

        public bool AllowRangeDataSelect { get; set; }
    }

    public class ResReportMaster
    {

        private static string resourceFile = "ResReportMaster";
        public static string ID { get { return ResourceRead.GetResourceValue("ID", resourceFile); } }

        public static string PageTitle { get { return ResourceRead.GetResourceValue("PageTitle", resourceFile); } }

        public static string EditReport { get { return ResourceRead.GetResourceValue("EditReport", resourceFile); } }
        public static string ReportName { get { return ResourceRead.GetResourceValue("ReportName", resourceFile); } }
        public static string ReportFileName { get { return ResourceRead.GetResourceValue("ReportFileName", resourceFile); } }
        public static string ReportType { get { return ResourceRead.GetResourceValue("ReportType", resourceFile); } }
        public static string CompanyID { get { return ResourceRead.GetResourceValue("CompanyID", resourceFile); } }
        public static string RoomID { get { return ResourceRead.GetResourceValue("RoomID", resourceFile); } }
        public static string SortColumns { get { return ResourceRead.GetResourceValue("SortColumns", resourceFile); } }
        public static string CreatedBy { get { return ResourceRead.GetResourceValue("CreatedBy", resourceFile); } }
        public static string UpdatedBy { get { return ResourceRead.GetResourceValue("UpdatedBy", resourceFile); } }
        public static string CreatedOn { get { return ResourceRead.GetResourceValue("CreatedOn", resourceFile); } }
        public static string UpdatedON { get { return ResourceRead.GetResourceValue("UpdatedON", resourceFile); } }
        public static string SubReportFileName { get { return ResourceRead.GetResourceValue("SubReportFileName", resourceFile); } }
        public static string IsPrivate { get { return ResourceRead.GetResourceValue("IsPrivate", resourceFile); } }
        public static string PrivateUserID { get { return ResourceRead.GetResourceValue("PrivateUserID", resourceFile); } }
        public static string IsBaseReport { get { return ResourceRead.GetResourceValue("IsBaseReport", resourceFile); } }
        public static string MasterReportResFile { get { return ResourceRead.GetResourceValue("MasterReportResFile", resourceFile); } }
        public static string SubReportResFile { get { return ResourceRead.GetResourceValue("SubReportResFile", resourceFile); } }
        public static string IsIncludeDateRange { get { return ResourceRead.GetResourceValue("IsIncludeDateRange", resourceFile); } }
        public static string IsIncludeTotal { get { return ResourceRead.GetResourceValue("IsIncludeTotal", resourceFile); } }
        public static string IsIncludeSubTotal { get { return ResourceRead.GetResourceValue("IsIncludeSubTotal", resourceFile); } }
        public static string IsIncludeGrandTotal { get { return ResourceRead.GetResourceValue("IsIncludeGrandTotal", resourceFile); } }
        public static string IsIncludeGroup { get { return ResourceRead.GetResourceValue("IsIncludeGroup", resourceFile); } }
        public static string GroupName { get { return ResourceRead.GetResourceValue("GroupName", resourceFile); } }
        public static string Days { get { return ResourceRead.GetResourceValue("Days", resourceFile); } }
        public static string FromDate { get { return ResourceRead.GetResourceValue("FromDate", resourceFile); } }
        public static string ToDate { get { return ResourceRead.GetResourceValue("ToDate", resourceFile); } }
        public static string ParentID { get { return ResourceRead.GetResourceValue("ParentID", resourceFile); } }
        public static string ToEmailAddress { get { return ResourceRead.GetResourceValue("ToEmailAddress", resourceFile); } }
        public static string ModuleName { get { return ResourceRead.GetResourceValue("ModuleName", resourceFile); } }
        public static string ISEnterpriseReport { get { return ResourceRead.GetResourceValue("ISEnterpriseReport", resourceFile); } }
        public static string IsDeleted { get { return ResourceRead.GetResourceValue("IsDeleted", resourceFile); } }
        public static string IsArchived { get { return ResourceRead.GetResourceValue("IsArchived", resourceFile); } }
        public static string SetAsDefaultPrintReport { get { return ResourceRead.GetResourceValue("SetAsDefaultPrintReport", resourceFile); } }
        public static string IsIncludeTax1 { get { return ResourceRead.GetResourceValue("IsIncludeTax1", resourceFile); } }
        public static string IsIncludeTax2 { get { return ResourceRead.GetResourceValue("IsIncludeTax2", resourceFile); } }
        public static string IsNotEditable { get { return ResourceRead.GetResourceValue("IsNotEditable", resourceFile); } }
        public static string IsDefaultReport { get { return ResourceRead.GetResourceValue("IsDefaultReport", resourceFile); } }
        public static string HideHeader { get { return ResourceRead.GetResourceValue("HideHeader", resourceFile); } }
        public static string ShowSignature { get { return ResourceRead.GetResourceValue("ShowSignature", resourceFile); } }

        public static string RoomName
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomName", resourceFile);
            }
        }

        public static string CompanyName
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyName", resourceFile);
            }
        }

        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        public static string PageHeaderForDefaultReportSetup
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeaderForDefaultReportSetup", resourceFile);
            }
        }

        public static string CreatedByName { get { return ResourceRead.GetResourceValue("CreatedByName", resourceFile); } }

        public static string UpdatedByName { get { return ResourceRead.GetResourceValue("UpdatedByName", resourceFile); } }

        public static string AlertConfigID { get { return ResourceRead.GetResourceValue("AlertConfigID", resourceFile); } }

        public static string EmailTemplateID { get { return ResourceRead.GetResourceValue("EmailTemplateID", resourceFile); } }
        public static string AllowScheduleIMM { get { return ResourceRead.GetResourceValue("AllowScheduleIMM", resourceFile); } }
        public static string AllowScheduleHourly { get { return ResourceRead.GetResourceValue("AllowScheduleHourly", resourceFile); } }
        public static string AllowScheduleWeekly { get { return ResourceRead.GetResourceValue("AllowScheduleWeekly", resourceFile); } }
        public static string AllowScheduleMonthly { get { return ResourceRead.GetResourceValue("AllowScheduleMonthly", resourceFile); } }
        public static string AllowedIMMActions { get { return ResourceRead.GetResourceValue("AllowedIMMActions", resourceFile); } }
        public static string AllowDataSelectSinceLastReportFilter { get { return ResourceRead.GetResourceValue("AllowDataSelectSinceLastReportFilter", resourceFile); } }

        public static string AllowDataSelectFirstOfMonth { get { return ResourceRead.GetResourceValue("AllowDataSelectFirstOfMonth", resourceFile); } }


        public static string AllowRangeDataSelect { get { return ResourceRead.GetResourceValue("AllowRangeDataSelect", resourceFile); } }

        public static string AllowDataSinceFilter { get { return ResourceRead.GetResourceValue("AllowDataSinceFilter", resourceFile); } }
        public static string AllowSupplierFilter { get { return ResourceRead.GetResourceValue("AllowSupplierFilter", resourceFile); } }
        public static string AllowPDFAttachment { get { return ResourceRead.GetResourceValue("AllowPDFAttachment", resourceFile); } }
        public static string AllowExcelAttachment { get { return ResourceRead.GetResourceValue("AllowExcelAttachment", resourceFile); } }
        public static string AllowAttachmentSelection { get { return ResourceRead.GetResourceValue("AllowAttachmentSelection", resourceFile); } }
        public static string AllowedAttahmentReports { get { return ResourceRead.GetResourceValue("AllowedAttahmentReports", resourceFile); } }
        //AllowScheduleDaily
        public static string AllowScheduleDaily { get { return ResourceRead.GetResourceValue("AllowScheduleDaily", resourceFile); } }
        public static string EmailTemplateName { get { return ResourceRead.GetResourceValue("EmailTemplateName", resourceFile); } }
        public static string ItemType { get { return ResourceRead.GetResourceValue("ItemType", resourceFile); } }

        //EntReportWorning
        public static string EntReportWorning { get { return ResourceRead.GetResourceValue("EntReportWorning", resourceFile); } }

        public static string ReportSchedule { get { return ResourceRead.GetResourceValue("ReportSchedule", resourceFile); } }
        public static string IsSupplierRequired { get { return ResourceRead.GetResourceValue("IsSupplierRequired", resourceFile); } }
        public static string IsDateRangeRequired { get { return ResourceRead.GetResourceValue("IsDateRangeRequired", resourceFile); } }
        public static string ParentReportName { get { return ResourceRead.GetResourceValue("ParentReportName", resourceFile);} }
        public static string ReportCopySuccessfully { get { return ResourceRead.GetResourceValue("ReportCopySuccessfully", resourceFile); } }
        public static string ErrorWhileCopyReport { get { return ResourceRead.GetResourceValue("ErrorWhileCopyReport", resourceFile); } }
        public static string ReqReportName { get { return ResourceRead.GetResourceValue("ReqReportName", resourceFile); } }
        public static string ReqReportToCopy { get { return ResourceRead.GetResourceValue("ReqReportToCopy", resourceFile); } }
        public static string ReqCompanyAndRoom { get { return ResourceRead.GetResourceValue("ReqCompanyAndRoom", resourceFile); } }
        public static string ReqDateRange { get { return ResourceRead.GetResourceValue("ReqDateRange", resourceFile); } }
        public static string NoProperDataToDelete { get { return ResourceRead.GetResourceValue("NoProperDataToDelete", resourceFile); } }
        public static string ReportNameUsedByOtherUser { get { return ResourceRead.GetResourceValue("ReportNameUsedByOtherUser", resourceFile); } }
        public static string ReportFileDoesNotExist { get { return ResourceRead.GetResourceValue("ReportFileDoesNotExist", resourceFile); } }
        public static string SubReportFileDoesNotExist { get { return ResourceRead.GetResourceValue("SubReportFileDoesNotExist", resourceFile); } }
        public static string ProjectUsedLessThanLimit { get { return ResourceRead.GetResourceValue("ProjectUsedLessThanLimit", resourceFile); } }
        public static string ProjectUsedMoreThanLimit { get { return ResourceRead.GetResourceValue("ProjectUsedMoreThanLimit", resourceFile); } }
        public static string ItemQtyUsedLessThanLimit { get { return ResourceRead.GetResourceValue("ItemQtyUsedLessThanLimit", resourceFile); } }
        public static string ItemQtyUsedMoreThanLimit { get { return ResourceRead.GetResourceValue("ItemQtyUsedMoreThanLimit", resourceFile); } }
        public static string ItemUsedLessThanLimit { get { return ResourceRead.GetResourceValue("ItemUsedLessThanLimit", resourceFile); } }
        public static string ItemUsedMoreThanLimit { get { return ResourceRead.GetResourceValue("ItemUsedMoreThanLimit", resourceFile); } }      
        public static string YouAreNotAbleToUpdate { get { return ResourceRead.GetResourceValue("YouAreNotAbleToUpdate", resourceFile); } }      
        public static string UpdateAllReportsFieldsError { get { return ResourceRead.GetResourceValue("UpdateAllReportsFieldsError", resourceFile); } }      
        public static string NoRecordsFoundToGeneratePDF { get { return ResourceRead.GetResourceValue("NoRecordsFoundToGeneratePDF", resourceFile); } }      
        public static string NoRecordsFoundToGenerateFile { get { return ResourceRead.GetResourceValue("NoRecordsFoundToGenerateFile", resourceFile); } }      
        public static string SelectDefaultReportForPrint { get { return ResourceRead.GetResourceValue("SelectDefaultReportForPrint", resourceFile); } }      

        public static string ReqMinimunOneColumn { get { return ResourceRead.GetResourceValue("ReqMinimunOneColumn", resourceFile); } }
        public static string MsgSelectDifferentStartandEndDate { get { return ResourceRead.GetResourceValue("MsgSelectDifferentStartandEndDate", resourceFile); } }
        public static string MsgSelectOnyOneItemToViewReport { get { return ResourceRead.GetResourceValue("MsgSelectOnyOneItemToViewReport", resourceFile); } }
        public static string MsgDoNotSelectTodayDateasStartDate { get { return ResourceRead.GetResourceValue("MsgDoNotSelectTodayDateasStartDate", resourceFile); } }
        public static string MsgDoNotSelectTodayDateasEndDate { get { return ResourceRead.GetResourceValue("MsgDoNotSelectTodayDateasEndDate", resourceFile); } }
        public static string ReqItemToViewReport { get { return ResourceRead.GetResourceValue("ReqItemToViewReport", resourceFile); } }
        public static string ReqStartandEndDate { get { return ResourceRead.GetResourceValue("ReqStartandEndDate", resourceFile); } }
        public static string MsgMax12MonthDurationCanSelect { get { return ResourceRead.GetResourceValue("MsgMax12MonthDurationCanSelect", resourceFile); } }
        public static string MsgSelectOnyOneItemToSendReport { get { return ResourceRead.GetResourceValue("MsgSelectOnyOneItemToSendReport", resourceFile); } }
        public static string ExcludeZeroQuantityOrder { get { return ResourceRead.GetResourceValue("ExcludeZeroQuantityOrder", resourceFile); } }
        public static string ExportToExcelxls { get { return ResourceRead.GetResourceValue("ExportToExcelxls", resourceFile); } }
        public static string ExportToExcelxlsx { get { return ResourceRead.GetResourceValue("ExportToExcelxlsx", resourceFile); } }
        public static string ExportToWordDoc { get { return ResourceRead.GetResourceValue("ExportToWordDoc", resourceFile); } }
        public static string ExportToWordDocx { get { return ResourceRead.GetResourceValue("ExportToWordDocx", resourceFile); } }
        public static string ExportToPDF { get { return ResourceRead.GetResourceValue("ExportToPDF", resourceFile); } }
        public static string ExportToIMAGE { get { return ResourceRead.GetResourceValue("ExportToIMAGE", resourceFile); } }
        public static string FirstSortField { get { return ResourceRead.GetResourceValue("FirstSortField", resourceFile); } }
        public static string SecondSortField { get { return ResourceRead.GetResourceValue("SecondSortField", resourceFile); } }
        public static string ThirdSortField { get { return ResourceRead.GetResourceValue("ThirdSortField", resourceFile); } }
        public static string FourthSortField { get { return ResourceRead.GetResourceValue("FourthSortField", resourceFile); } }
        public static string FifthSortField { get { return ResourceRead.GetResourceValue("FifthSortField", resourceFile); } }
        public static string SetAsDefaultPrint { get { return ResourceRead.GetResourceValue("SetAsDefaultPrint", resourceFile); } }
        public static string SetAsDefaultPrintForAllRoom { get { return ResourceRead.GetResourceValue("SetAsDefaultPrintForAllRoom", resourceFile); } }
        public static string MakeAsPrivate { get { return ResourceRead.GetResourceValue("MakeAsPrivate", resourceFile); } }
        public static string MakeasEnterpriseReport { get { return ResourceRead.GetResourceValue("MakeasEnterpriseReport", resourceFile); } }
        public static string IncludeDateRange { get { return ResourceRead.GetResourceValue("IncludeDateRange", resourceFile); } }
        public static string IncludeTotal { get { return ResourceRead.GetResourceValue("IncludeTotal", resourceFile); } }
        public static string IncludeSubTotal { get { return ResourceRead.GetResourceValue("IncludeSubTotal", resourceFile); } }
        public static string IncludeGrandTotal { get { return ResourceRead.GetResourceValue("IncludeGrandTotal", resourceFile); } }
        public static string IncludeRoomDetail { get { return ResourceRead.GetResourceValue("IncludeRoomDetail", resourceFile); } }
        public static string SelectedCompany { get { return ResourceRead.GetResourceValue("SelectedCompany", resourceFile); } }
        public static string Company { get { return ResourceRead.GetResourceValue("Company", resourceFile); } }
        public static string Room { get { return ResourceRead.GetResourceValue("Room", resourceFile); } }
        public static string SelectedRoom { get { return ResourceRead.GetResourceValue("SelectedRoom", resourceFile); } }
        public static string HideReportHeader { get { return ResourceRead.GetResourceValue("HideReportHeader", resourceFile); } }
        public static string ShowSignatureOnLastPage { get { return ResourceRead.GetResourceValue("ShowSignatureOnLastPage", resourceFile); } }
        public static string Includenormalpull { get { return ResourceRead.GetResourceValue("Includenormalpull", resourceFile); } }
        public static string ViewReport { get { return ResourceRead.GetResourceValue("ViewReport", resourceFile); } }
        public static string SendReport { get { return ResourceRead.GetResourceValue("SendReport", resourceFile); } }
        public static string DeleteSelectedReport { get { return ResourceRead.GetResourceValue("DeleteSelectedReport", resourceFile); } }
        public static string EndDate { get { return ResourceRead.GetResourceValue("EndDate", resourceFile); } }
        public static string Datecreatedend { get { return ResourceRead.GetResourceValue("Datecreatedend", resourceFile); } }
        public static string StartDate { get { return ResourceRead.GetResourceValue("StartDate", resourceFile); } }
        public static string Datecreatedstart { get { return ResourceRead.GetResourceValue("Datecreatedstart", resourceFile); } }
        public static string DateCreatedEarlierThan { get { return ResourceRead.GetResourceValue("DateCreatedEarlierThan", resourceFile); } }
        public static string DateActiveLaterThan { get { return ResourceRead.GetResourceValue("DateActiveLaterThan", resourceFile); } }
        public static string SelectDate { get { return ResourceRead.GetResourceValue("SelectDate", resourceFile); } }
        public static string DateFilterOn { get { return ResourceRead.GetResourceValue("DateFilterOn", resourceFile); } }
        public static string Range { get { return ResourceRead.GetResourceValue("Range", resourceFile); } }
        public static string LabelListofReports { get { return ResourceRead.GetResourceValue("LabelListofReports", resourceFile); } }
        public static string ItemStatus { get { return ResourceRead.GetResourceValue("ItemStatus", resourceFile); } }
        public static string ItemTypes { get { return ResourceRead.GetResourceValue("ItemTypes", resourceFile); } }
        public static string CartType { get { return ResourceRead.GetResourceValue("CartType", resourceFile); } }
        public static string QuantityType { get { return ResourceRead.GetResourceValue("QuantityType", resourceFile); } }
        public static string UsageType { get { return ResourceRead.GetResourceValue("UsageType", resourceFile); } }
        public static string AllowItemZeroPullUsageLabel { get { return ResourceRead.GetResourceValue("AllowItemZeroPullUsageLabel", resourceFile); } }
        public static string AppliedFilter { get { return ResourceRead.GetResourceValue("AppliedFilter", resourceFile); } }
        public static string OnlyAvailableTools { get { return ResourceRead.GetResourceValue("OnlyAvailableTools", resourceFile); } }
        public static string Italic { get { return ResourceRead.GetResourceValue("Italic", resourceFile); } }
        public static string Bold { get { return ResourceRead.GetResourceValue("Bold", resourceFile); } }
        public static string Normal { get { return ResourceRead.GetResourceValue("Normal", resourceFile); } }
        public static string left { get { return ResourceRead.GetResourceValue("left", resourceFile); } }
        public static string Middle { get { return ResourceRead.GetResourceValue("Middle", resourceFile); } }
        public static string Right { get { return ResourceRead.GetResourceValue("Right", resourceFile); } }
        public static string Select { get { return ResourceRead.GetResourceValue("Select", resourceFile); } }
        public static string Portrait { get { return ResourceRead.GetResourceValue("Portrait", resourceFile); } }
        public static string Landscape { get { return ResourceRead.GetResourceValue("Landscape", resourceFile); } }
        public static string Include { get { return ResourceRead.GetResourceValue("Include", resourceFile); } }
        public static string GroupBy { get { return ResourceRead.GetResourceValue("GroupBy", resourceFile); } }
        public static string SaveAs { get { return ResourceRead.GetResourceValue("SaveAs", resourceFile); } }
        public static string Delete { get { return ResourceRead.GetResourceValue("Delete", resourceFile); } }
        public static string Asc { get { return ResourceRead.GetResourceValue("Asc", resourceFile); } }
        public static string Desc { get { return ResourceRead.GetResourceValue("Desc", resourceFile); } }
        public static string CurrentWidthHeader { get { return ResourceRead.GetResourceValue("CurrentWidthHeader", resourceFile); } }
        public static string Detail { get { return ResourceRead.GetResourceValue("Detail", resourceFile); } }
        public static string MaxWidth { get { return ResourceRead.GetResourceValue("MaxWidth", resourceFile); } }
        public static string CurrentWidth { get { return ResourceRead.GetResourceValue("CurrentWidth", resourceFile); } }
        public static string Types { get { return ResourceRead.GetResourceValue("Types", resourceFile); } }
        public static string Status { get { return ResourceRead.GetResourceValue("Status", resourceFile); } }
        public static string WorkOrderStatus { get { return ResourceRead.GetResourceValue("WorkOrderStatus", resourceFile); } }
        public static string DisplayAllCheckedOutTools { get { return ResourceRead.GetResourceValue("DisplayAllCheckedOutTools", resourceFile); } }
        public static string SelectOne { get { return ResourceRead.GetResourceValue("SelectOne", resourceFile); } }
        public static string SetAsDefaultPrintForAllEntConfirm { get { return ResourceRead.GetResourceValue("SetAsDefaultPrintForAllEntConfirm", resourceFile); } }
        public static string Event { get { return ResourceRead.GetResourceValue("Event", resourceFile); } }
        public static string Report { get { return ResourceRead.GetResourceValue("Report", resourceFile); } }
        public static string ParentReport { get { return ResourceRead.GetResourceValue("ParentReport", resourceFile); } }

    }

    public class ModuleWiseReportListForDefaultPrintDTO
    {
        public long ModuleID { get; set; }
        public string ModuleName { get; set; }
        public long MasterReportID { get; set; }
        public string MasterReportName { get; set; }
        public string ChildReportName { get; set; }
        public long ChildReportID { get; set; }
        public bool isBaseReport { get; set; }
        public bool? ISEnterpriseReport { get; set; }
        public long ChildParentID { get; set; }
        public string Parents { get; set; }

        public string ResModuleName { get; set; }

        public string ResourceKey { get; set; }

        public List<ModuleWiseMasterReportList> lstModuleWiseMasterReport;
    }

    public class ModuleWiseMasterReportList
    {
        public long MasterReportID { get; set; }
        public string MasterReportName { get; set; }
        public string Parents { get; set; }

        public List<ModuleWiseChildReportList> lstModuleWiseChildReport;
    }

    public class ModuleWiseChildReportList
    {
        public string ChildReportName { get; set; }
        public long ChildReportID { get; set; }
        public long MasterReportID { get; set; }
        public bool? ISEnterpriseReport { get; set; }
    }

    public class ModuleWiseReportForDefaultPrintDTO
    {
        public long ID { get; set; }
        public long ModuleID { get; set; }
        public long DefaultPrintReportID { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public System.Int64 CreatedBy { get; set; }
        public System.Int64 LastUpdatedBy { get; set; }
        public long CompanyID { get; set; }
        public long? RoomID { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public Nullable<Boolean> IsArchived { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }  
        public bool SetAsDefaultPrintReportForAllRoom { get; set; }
    }
}
