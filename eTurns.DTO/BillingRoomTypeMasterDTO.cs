using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class BillingRoomTypeMasterDTO
    {
        public int ID { get; set; }
        public string BillingRoomTypeName { get; set; }
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }

        public long EnterpriseID { get; set; }
    }
}
