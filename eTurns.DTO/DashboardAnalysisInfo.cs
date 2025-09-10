using System;

namespace eTurns.DTO
{
    public class DashboardAnalysisInfo
    {
        public double CalculatedTurn { get; set; }
        public double CalculatedAverageUsage { get; set; }
        public Guid? ItemGUID { get; set; }
        public double? CalculatedMinimun { get; set; }
        public double? CalculatedMaximun { get; set; }
        public double? PullCost { get; set; }
        public double? PullQuantity { get; set; }
        public double? OrderQuantity { get; set; }
        public double? InventoryValue { get; set; }
        public double? MonthizedPullValueTurns { get; set; }
        public double? MonthizedPullTurns { get; set; }
        public double? MonthizedOrderTurns { get; set; }
        public double CalculatedPullValueAverageUsage { get; set; }
        public double CalculatedPullAverageUsage { get; set; }
        public double CalculatedOrderAverageUsage { get; set; }

    }


}
