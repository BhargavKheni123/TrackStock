using System.Collections.Generic;

namespace eTurns.DTO
{
    public static class RedCircleStatic
    {

        static RedCircleStatic()
        {
            SignalGroups = new List<DTO.ECR>();
        }
        public static List<ECR> SignalGroups { get; set; }
    }

    public class ECR
    {
        public long EID { get; set; }
        public long CID { get; set; }
        public long RID { get; set; }
        public bool IsProcessed { get; set; }
    }
}
