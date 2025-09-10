using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ProductSearchQueryReqInfo
    {
        public string keywords { get; set; }
        public string PageNumber { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long UserID { get; set; }
        public string PrimeEligible { get; set; }
        public string EligibleForFreeShipping { get; set; }
        public string DeliveryDay { get; set; }
        public string Category { get; set; }
        public string Availability { get; set; }
        public string SubCategory { get; set; }
        public int pageSize { get; set; }
        public string EnterPriseDBName { get; set; }
    }
}
