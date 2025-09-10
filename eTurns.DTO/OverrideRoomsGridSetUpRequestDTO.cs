using System;

namespace eTurns.DTO
{
    public class OverrideRoomsGridSetUpRequestDTO
    {
        public long ID { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long UserID { get; set; }
        public long RoleID { get; set; }
        public int UserType { get; set; }
        public string UsersUISettingType { get; set; }
        public bool IsStarted { get; set; }
        public Nullable<System.DateTime> TimeStarted { get; set; }
        public bool IsCompleted { get; set; }
        public Nullable<System.DateTime> TimeCompleted { get; set; }
        public bool IsException { get; set; }
        public Nullable<System.DateTime> TimeException { get; set; }
        public string ErrorException { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }        
        public long? CreatedBy { get; set; }
        public long? LastUpdatedBy { get; set; }
    }
}
