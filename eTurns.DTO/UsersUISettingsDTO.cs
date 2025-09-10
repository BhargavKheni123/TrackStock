using System;

namespace eTurns.DTO
{
    public class UsersUISettingsDTO
    {
        public Int64 ID { get; set; }
        public Nullable<Int64> UserID { get; set; }
        public string ListName { get; set; }
        public string JSONDATA { get; set; }

        public Nullable<Int64> EnterpriseID { get; set; }

        public Nullable<Int64> CompanyID { get; set; }

        public Nullable<Int64> RoomID { get; set; }
    }
}
