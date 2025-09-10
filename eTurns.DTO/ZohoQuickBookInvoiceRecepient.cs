using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ZohoQuickBookInvoiceRecepientDetail
    {
        public List<ZohoQuickBookInvoiceRecepient> ZohoQuickBookInvoiceRecepient { get; set; }

    }
    public class ZohoQuickBookInvoiceRecepient
    {
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
    }
}
