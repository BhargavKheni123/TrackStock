using System;

namespace eTurns.DTO
{
    public class InsertABOrderDTO
    {
        public string OrderId {get; set; }
        public DateTime? OrderCreated { get; set; }
        public DateTime? OrderLastUpdated { get; set; }
        public string OrderJson { get; set; }
        public byte[] OrderJsonEncrypted { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
    }
}
