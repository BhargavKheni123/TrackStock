using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class ItemMasterMonthEndDTO
    {
        public int? CompanyID { get; set; }
        public int? ExtendedCost { get; set; }
        public Guid GUID { get; set; }
        public bool IsItemLevelMinMaxQtyRequired { get; set; }
        public double MaximumQuantity { get; set; }
        public double MinimumQuantity { get; set; }
        public int? OnHandQuantity { get; set; }
        public int? Room { get; set; }
    }
}
