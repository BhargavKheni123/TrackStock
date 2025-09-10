using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class PullRequestInfo
    {
        public long ID { get; set; }
        public Guid? ItemGUID { get; set; }
        public Guid? BinGUID { get; set; }
        public long? ItemID { get; set; }
        public long? BinID { get; set; }
        public double? PullQuantity { get; set; }
        public Guid? ProjectGUID { get; set; }
        public long? UserId { get; set; }
        public string ItemNumber { get; set; }
        public string ItemType { get; set; }
        public string BinNumber { get; set; }
        public List<ItemLocationDetailsDTO> lstItemLocations { get; set; }
    }
}
