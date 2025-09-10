using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class eVmiReqRespDTO
    {
        public string Status { get; set; }
        public double WeightPerPiece { get; set; }
        public int ShelfID { get; set; }
        public long? calibrateRequestID { get; set; }
        public double calWeight { get; set; }
        public string Version { get; set; }
        public string SerialNo { get; set; }
        public string Model { get; set; }
    }
}
