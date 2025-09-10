using System;

namespace eTurns.DTO
{
    [Serializable]
    public class RoomReferenceDeleteDTO
    {
        public Int64 Id { get; set; }

        public string RoomIds { get; set; }

        public string EnterpriseDBName { get; set; }

        public Int64 EnterpriseId { get; set; }

        public Int64 CompanyId { get; set; }

        public int ProcessStatus { get; set; }
    }
}
