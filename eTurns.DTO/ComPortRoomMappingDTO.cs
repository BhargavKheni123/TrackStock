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
    public class ComPortRoomMappingDTO
    {
        public System.Int64 ID { get; set; }
        public System.Int64 ComPortMasterID { get; set; }
        public System.Int64 RoomID { get; set; }
        public System.Int64 CompanyID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> UpdatedBy { get; set; }
        public Nullable<Int64> IsDeleted { get; set; }
        public int? ShelfID { get; set; }
        public string SelectedComPortMasterIDs { get; set; }

        public long MasterMappingID { get; set; }

        public string ComPortName { get; set; }
        public string HostName { get; set; }
        public string UsrCloudID { get; set; }
        public string PublicIP { get; set; }
        public string PrivateIP { get; set; }
        public int TCPPort { get; set; }
    }


}
