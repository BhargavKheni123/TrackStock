using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DTO
{
    [Serializable]
    public class ComPortMasterDTO
    {
        public System.Int64 ID { get; set; }
        public System.String ComPortName { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> UpdatedBy { get; set; }
        public Nullable<Boolean> IsDeleted { get; set; }
        public System.String HostName { get; set; }
        public System.String UsrCloudID { get; set; }
        public System.String PublicIP { get; set; }
        public System.String PrivateIP { get; set; }

        public Nullable<bool> IsSelected { get; set; }
        public int TCPPort { get; set; }
    }
}
