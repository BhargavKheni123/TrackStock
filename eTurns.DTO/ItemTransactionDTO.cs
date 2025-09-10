using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class ItemTransactionDTO
    {
        public long TxnID { get; set; }
        public DateTime TxnDate { get; set; }
        public string TxnType { get; set; }
        public double TxnQty { get; set; }
        public double TxnClosingQty { get; set; }
        public string TxnNumber { get; set; }
        public string TxnModuleItemName { get; set; }
        public Guid GUID { get; set; }
        public Guid ItemGUID { get; set; }
        public double TxnValue { get; set; }
        public string TxnstringDate { get; set; }
    }

    public class ItemTransationInfo
    {
        public long ItemId { get; set; }
        public Guid ItemGUID { get; set; }
        public string ItemNumber { get; set; }
        public DashboardParameterDTO RoomAnalyticSettings { get; set; }
        public IEnumerable<ItemTransactionDTO> TxtHistory { get; set; }
        public DateTime? TxnStartDate { get; set; }
        public DateTime? TxnEndDate { get; set; }
        public string AUMeasureMethod { get; set; }
        public double TotalQty { get; set; }
        public double TotalTxnQty { get; set; }
        public double TotalTxnValue { get; set; }
    }
}
