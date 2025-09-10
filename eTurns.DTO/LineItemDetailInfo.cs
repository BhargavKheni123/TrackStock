namespace eTurns.DTO
{
    public class EDILineItemDetailInfo
    {
        public string EnterpriseName { get; set; }
        public string SupplierAccountNumber { get; set; }
        public string ItemNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public long RoomID { get; set; }
        public long CompanyID { get; set; }
        public string CostUOM { get; set; }
        public string ItemCost { get; set; }
        //public string ItemDescription { get; set; }
    }
}
