using System;

namespace eTurns.DTO
{
    public class SolumStore
    {
        public long ID { get; set; }
        public long TokenId { get; set; }
        public Guid GUID { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public string CompanyName { get; set; }
        public string StationCode { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsArchived { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public long CreatedBy { get; set; }
        public long LastUpdatedBy { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public string AccessToken { get; set; }
        public long? UserId { get; set; }

    }
}
