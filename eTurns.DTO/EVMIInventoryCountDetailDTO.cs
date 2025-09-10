using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class EVMIInvCountDetailDTO
    {
        public Guid ItemGUID { get; set; }
        public Guid BinGUID { get; set; }
        public double NewQuantity { get; set; }


        public long BinID { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }

        public long CreatedBy { get; set; }
        public bool IsConsignment { get; set; }

        public double ItemCost { get; set; }
        public double ItemPrice { get; set; }
        public Guid? CountGUID { get; set; }
    }

    public class SaveEVMIInvCountDetailDTO
    {
        public long? CountDetailID { get; set; }
        public Guid? CountDetailGUID { get; set; }
        public Guid? InventoryCountGUID { get; set; }
        public string Status { get; set; }
    }
}
