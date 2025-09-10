using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class SSOUsersMapDTO
    {
        public long ID { get; set; }
        public string SSOClient { get; set; }
        public string SSOType { get; set; }
        public string SSOUserName { get; set; }
        public string SSOUserEmail { get; set; }
        public string SSOUserFirstName { get; set; }
        public string SSOUserLastName { get; set; }
        public Guid eTurnsUserGUID { get; set; }
        public string eTurnsUserName { get; set; }
    }
}
