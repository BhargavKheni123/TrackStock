using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class ItemAvgCostQtyInfo
    {        
        public Guid ItemGUID { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyId { get; set; }
        public long RoomId { get; set; }
        public double AverageExtendedCost { get; set; }
        public double AverageOnHandQuantity { get; set; }

    }
}
