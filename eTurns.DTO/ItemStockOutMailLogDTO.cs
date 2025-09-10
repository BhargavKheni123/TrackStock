using System;

namespace eTurns.DTO
{
    public class ItemStockOutMailLogDTO
    {


        public System.Int64 ItemId { get; set; }
        public string ItemNumber { get; set; }
        public Guid? ItemGUID { get; set; }
        public System.Int64 RoomId { get; set; }
        public System.Int64 CompanyId { get; set; }
        public Nullable<System.Double> OnHandQuantity { get; set; }
        public DateTime StockoutDate { get; set; }
        public System.Int64 ID { get; set; }
        public System.Int64 UserID { get; set; }
        public System.Boolean IsMailSendComplete { get; set; }
        public System.Int64 BinID { get; set; }
        public string CompanyName { get; set; }
        public string RoomName { get; set; }
        public Nullable<System.Double> MinimumQuantity { get; set; }
        public Nullable<System.Double> MaximumQuantity { get; set; }
    }
}
