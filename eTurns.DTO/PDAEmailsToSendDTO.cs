using System;

namespace eTurns.DTO
{
    public class PDAEmailsToSendDTO
    {
        public Guid GUID { get; set; }

        public long CompanyId { get; set; }

        public long RoomId { get; set; }

        public Guid RecordGUID { get; set; }

        public string RecordFromTable { get; set; }

        public string EmailTemplateType { get; set; }

        public bool IsSent { get; set; }

        public long CreatedByUserId { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
    }
}
