using System;

namespace eTurns.DTO
{
    public class RPT_DicrepencyItem
    {

        public string BinNumber { get; set; }
        public string BlanketPO { get; set; }
        public DateTime? created { get; set; }
        public string CriticalQuantity { get; set; }
        public string ExtendedCost { get; set; }
        public bool? IsAddedFromPDA { get; set; }
        public string ItemBinOnHand { get; set; }
        public string itemCost { get; set; }
        public string ItemNumber { get; set; }
        public string LotNumber { get; set; }
        public string ManufacturerName { get; set; }
        public string UserName { get; set; }
        public string SupplierName { get; set; }
        public string RoomInfo { get; set; }
        public string PackSlipNumber { get; set; }
        public string MaximumQuantity { get; set; }
        public string PoolQuantity { get; set; }
        public string SerialNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string OnOrderQuantity { get; set; }
        public string MinimumQuantity { get; set; }
        public string ManufacturerNumber { get; set; }
        public string OnHandQuantity { get; set; }
        public string PullCredit { get; set; }
        public Guid? ItemGUID { get; set; }
        public string ItemOnhandQty { get; set; }
    }
}
