using System;

namespace eTurns.DTO
{
    public class SiteListColumnDetailDTO
    {
        public Int64 ID { get; set; }
        public Int64 ListId { get; set; }
        public string ColumnName { get; set; }
        public string ActualColumnName { get; set; }
        public bool Visibility { get; set; }
        public int? LastOrder { get; set; }
        public int? OrderNumber { get; set; }
        public double ColumnSize { get; set; }
        public string ResourceFileName { get; set; }
        public string SortColumnName { get; set; }
        public bool IsVisibilityEditable { get; set; }
    }
}
