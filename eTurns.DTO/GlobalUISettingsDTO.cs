using System;

namespace eTurns.DTO
{
    public class GlobalUISettingsDTO
    {
        public Int64 ID { get; set; }
        public Nullable<Int64> CompanyID { get; set; }
        public Nullable<Int64> UserID { get; set; }
        public Nullable<Int32> GridRefreshTimeInSecond { get; set; }
    }
}
