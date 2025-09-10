using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class UsersUISettingsQueueDTO
    {
        public long ID { get; set; }
        public string OperationCode { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long UserID { get; set; }
        public string JSONDATA { get; set; }
        public string ListName { get; set; }
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
    }

    public enum UsersUISettingType
    {
        WI = 1,
        NLF = 2,
        Angular = 3
    }
}
