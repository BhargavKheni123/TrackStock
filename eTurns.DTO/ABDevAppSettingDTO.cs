using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ABDevAppSettingDTO
    {
        public long ID { get; set; }
        public string ABAppName { get; set; }
        public string ABAppID { get; set; }
        public string LWAClientID { get; set; }
        public string LWAClientSecret { get; set; }
        public string DeveloperID { get; set; }
        public string ABIAMAccessKey { get; set; }
        public string ABIAMAccessKeySecret { get; set; }
        public string RedirectURL { get; set; }
        public string BtoBConnectURL { get; set; }
        public string TokenRequestURL { get; set; }
    }
}
