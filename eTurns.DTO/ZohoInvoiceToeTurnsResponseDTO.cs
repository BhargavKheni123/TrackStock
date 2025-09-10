using System;

namespace eTurns.DTO
{
    public class ZohoInvoiceToeTurnsResponseDTO
    {
        public long ID { get; set; }
        public bool IsStarted { get; set; }
        public Nullable<System.DateTime> TimeStarted { get; set; }
        public bool IsCompleted { get; set; }
        public Nullable<System.DateTime> TimeCompleted { get; set; }
        public bool IsException { get; set; }
        public Nullable<System.DateTime> TimeException { get; set; }
        public string ErrorException { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public string RespData { get; set; }
        public string QBInvoiceId { get; set; }

        public ZohoInvoiceToeTurnsResponseInfo InvoiceInfo { get; set; }
        
        public ZohoInvoice Invoice { get; set; }
        public bool isValid { get; set; }
    }

    public class ZohoInvoiceToeTurnsResponseInfo
    {
        
        public DateTime created_time { get; set; }
        public string event_id { get; set; }
        public string event_type { get; set; }
        public ZohoInvoiceData data { get; set; }
        public string event_time_formatted { get; set; }
        public string event_source { get; set; }
        public string event_time { get; set; }
    }

    public class ZohoInvoiceData
    {
        public ZohoInvoice invoice { get; set; }
    }
}
