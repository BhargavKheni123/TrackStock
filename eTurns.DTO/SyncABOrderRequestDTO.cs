using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class SyncABOrderRequestDTO
    {
        public long ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte Mode { get; set; }
        public bool IsStarted { get; set; }
        public Nullable<System.DateTime> TimeStarted { get; set; }
        
        public bool IsCompleted { get; set; }
        public Nullable<System.DateTime> TimeCompleted { get; set; }
        public bool IsException { get; set; }
        public Nullable<System.DateTime> TimeException { get; set; }
        public string ErrorException { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long? CreatedBy { get; set; }
        public long? LastUpdatedBy { get; set; }
    }
}
