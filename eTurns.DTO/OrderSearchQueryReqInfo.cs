namespace eTurns.DTO
{
    public class OrderSearchQueryReqInfo
    {
        public string startDate {get; set; }
        public string endDate { get; set; }
        public string nextPageToken { get; set; }
        public string purchaseOrderNumber { get; set; }
        public bool includeLineItems { get; set; }
        public bool includeShipments { get; set; }
        public bool includeCharges { get; set; }

        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long UserID { get; set; }
        public string EnterPriseDBName { get; set; }
    }

    public class OrderByIdQueryReqInfo
    {
        public string OrderId { get; set; }
        public bool includeLineItems { get; set; }
        public bool includeShipments { get; set; }
        public bool includeCharges { get; set; }

        public long CompanyID { get; set; }
        public long RoomID { get; set; }
        public long UserID { get; set; }
        public string EnterPriseDBName { get; set; }
    }
}
