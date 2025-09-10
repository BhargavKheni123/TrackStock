using System;

namespace eTurns.DTO
{
    public class DailyScheduleRunHistoryDTO
    {
        public long ID { get; set; }
        public Guid RecGUID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? ScheduleID { get; set; }
        public long? MasterScheduleID { get; set; }
        public bool IsTurnsCalcStarted { get; set; }
        public bool IsOrderNightlyCalcStarted { get; set; }
        public bool IsDailyItemLocStarted { get; set; }
        public DateTime? TurnsCalcStartedTime { get; set; }
        public DateTime? OrderNightlyCalcStartedTime { get; set; }
        public DateTime? DailyItemLocStartedTime { get; set; }
        public bool IsTurnsCalcCompleted { get; set; }
        public bool IsOrderNightlyCalcCompleted { get; set; }
        public bool IsDailyItemLocCompleted { get; set; }
        public DateTime? TurnsCalcCompletedTime { get; set; }
        public DateTime? OrderNightlyCalcCompletedTime { get; set; }
        public DateTime? DailyItemLocCompletedTime { get; set; }
        public DateTime? NextRunDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public string TurnsCalcException { get; set; }
        public string OrderNightlyException { get; set; }
        public string DailyItemLocException { get; set; }

        public DateTime? TurnsCalcErrorLogingTime { get; set; }
        public DateTime? OrderNightlyCalcErrorLogingTime { get; set; }
        public DateTime? DailyItemLocLogingTime { get; set; }
        public string ScheduleDbName { get; set; }


    }
    public class OrderScheduleRunHistoryDTO
    {
        public long ID { get; set; }
        public Guid RecGUID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? ScheduleID { get; set; }
        public long? MasterScheduleID { get; set; }
        public DateTime? NextRunDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ScheduleDbName { get; set; }
        public string GeneralScheduleException { get; set; }
        public bool IsGeneralScheduleCompleted { get; set; }
        public DateTime? GeneralScheduleExceptionTime { get; set; }
        public DateTime? GeneralScheduleCompletedTime { get; set; }
        public DateTime? GeneralScheduleStartedTime { get; set; }
        public short? ScheduleFor { get; set; }
        public bool IsGeneralScheduleStarted { get; set; }

        public int Attempt { get; set; }
        public int LastAttemptDate { get; set; }
    }

    public class ReportAlertScheduleRunHistoryDTO
    {
        public Guid RecGUID { get; set; }
        public int? ScheduleFor { get; set; }
        public int? AlertReportScheduleAttempt { get; set; }
        public int? CycleCountScheduleAttempt { get; set; }
        public DateTime? NextRunDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? AlertReportScheduleStartedTime { get; set; }
        public DateTime? AlertReportScheduleCompletedTime { get; set; }
        public DateTime? AlertReportScheduleExceptionTime { get; set; }
        public DateTime? AlertReportScheduleAttemptTime { get; set; }
        public DateTime? CycleCountScheduleStartedTime { get; set; }
        public DateTime? CycleCountScheduleCompletedTime { get; set; }
        public DateTime? CycleCountScheduleExceptionTime { get; set; }
        public DateTime? CycleCountScheduleAttemptTime { get; set; }
        public bool IsAlertReportScheduleStarted { get; set; }
        public bool IsAlertReportScheduleCompleted { get; set; }
        public bool IsCycleCountScheduleStarted { get; set; }
        public bool IsCycleCountScheduleCompleted { get; set; }
        public long ID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? ScheduleID { get; set; }
        public long? NotificationID { get; set; }
        public long? MasterScheduleID { get; set; }
        public long? MasterNotificationID { get; set; }
        public long? CycleCountSettingID { get; set; }
        public long? MasterCycleCountSettingID { get; set; }
        public string AlertReportScheduleException { get; set; }
        public string CycleCountScheduleException { get; set; }
        public string DataGuids { get; set; }

        public string ExternalFilter { get; set; }
        public bool ExecuitAsPastSchedule { get; set; }
    }


    public class eVMIScheduleRunHistoryDTO
    {
        public long ID { get; set; }
        public Guid RecGUID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? eVMISetupID { get; set; }
        public long? MastereVMISetupID { get; set; }
        public DateTime? NextPollDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ScheduleDbName { get; set; }
        public string GeneralScheduleException { get; set; }
        public bool IsGeneralScheduleCompleted { get; set; }
        public DateTime? GeneralScheduleExceptionTime { get; set; }
        public DateTime? GeneralScheduleCompletedTime { get; set; }
        public DateTime? GeneralScheduleStartedTime { get; set; }
        public short? ScheduleFor { get; set; }
        public bool IsGeneralScheduleStarted { get; set; }

        public int Attempt { get; set; }
        public int LastAttemptDate { get; set; }
    }

}
