using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class eVMIRequestDTO
    {
        public long ComPortRoomMappingID { get; set; }
        public int ReqID { get; set; }
        public long? ItemLocationID { get; set; }
        public Int64? TotalQTY { get; set; }
        public double? CalibrateWeight { get; set; }
        public int? SetShelfID { get; set; }
        public long pCalibrateRequestID { get; set; }
        public string cmd { get; set; }
        public int ScaleID { get; set; }
        public int ChannelID { get; set; }
        public string ModelNumber { get; set; }

        public long RoomID { get; set; }
        public long CompanyID { get; set; }
        public long UserID { get; set; }
        public string EnterPriseDBName { get; set; }
        public long EnterPriceID { get; set; }
        public bool? isEVMI { get; set; }

        public Int64 BinId { get; set; }
        public string ItemGuid { get; set; }

        public eVMISiteSettings SensorBinRoomSettings { get; set; }
    }
}
