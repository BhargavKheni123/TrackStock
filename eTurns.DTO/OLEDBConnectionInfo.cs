using System;

namespace eTurns.DTO
{
    public class OLEDBConnectionInfo
    {
        public long ID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public Guid GUID { get; set; }
        public string APP { get; set; }
        public string ApplicationIntent { get; set; }
        public string AppDatabase { get; set; }
        public string MarsConn { get; set; }
        public string PacketSize { get; set; }
        public string PWD { get; set; }
        public string Server { get; set; }
        public string Timeout { get; set; }
        public string Trusted_Connection { get; set; }
        public string UID { get; set; }
        public string FailoverPartner { get; set; }
        public string PersistSensitive { get; set; }
        public string ConectionType { get; set; }
        public string MultiSubnetFailover { get; set; }
    }
}
