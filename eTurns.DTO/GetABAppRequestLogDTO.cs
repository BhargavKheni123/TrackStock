using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace eTurns.DTO
{
    public class GetABAppRequestLogDTO
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string amazon_callback_uri { get; set; }
        public string amazon_state { get; set; }
        public string applicationId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public long EnterpriseID { get; set; }
        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public string eTurns_state { get; set; }
        public string auth_code { get; set; }

    }
}
