using System;

namespace eTurns.DTO
{
    public class ArchiveScheduleRunHistoryDTO
    {
        public Guid RecGUID { get; set; }
        public int? ScheduleFor { get; set; }
        public DateTime? NextRunDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? ArchiveScheduleStartedTime { get; set; }
        public DateTime? ArchiveScheduleCompletedTime { get; set; }
        public string ArchiveScheduleException { get; set; }
        public DateTime? ArchiveScheduleExceptionTime { get; set; }
        public int? ArchiveScheduleAttempt { get; set; }
        public DateTime? ArchiveScheduleAttemptTime { get; set; }
        public bool IsArchiveScheduleStarted { get; set; }
        public bool IsArchiveScheduleCompleted { get; set; }
        public long ID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? ScheduleID { get; set; }
        public long? MasterScheduleID { get; set; }
        public string DataGuids { get; set; }
        public string ExternalFilter { get; set; }
    }
}
