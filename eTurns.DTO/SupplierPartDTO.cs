using System;

namespace eTurns.DTO
{
    public class SupplierPartDTO
    {
        public string ItemNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string SupplierNumber { get; set; }
        public Guid GUID { get; set; }
        public long ID { get; set; }
        public string CostUOM { get; set; }
        public int CostUOMValue { get; set; }
        public string RoomNames { get; set; }
}
}
