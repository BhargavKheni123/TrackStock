using System;

namespace eTurns.DTO
{
    [Serializable]
    public class DashboardParameterDTO
    {
        public long EnterpriseId { get; set; }
        public int? AUDayOfUsageToSample { get; set; }


        public int? AUDaysOfDailyUsage { get; set; }
        public byte? AUMeasureMethod { get; set; }
        public long CompanyId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long ID { get; set; }
        public int? MinMaxDayOfAverage { get; set; }
        public int? MinMaxDayOfUsageToSample { get; set; }
        public byte? MinMaxMeasureMethod { get; set; }
        public double? MinMaxMinNumberOfTimesMax { get; set; }
        public int? MinMaxOptValue1 { get; set; }
        public int? MinMaxOptValue2 { get; set; }
        public long RoomId { get; set; }
        public int? TurnsMonthsOfUsageToSample { get; set; }
        public int? TurnsDaysOfUsageToSample { get; set; }
        public byte? TurnsMeasureMethod { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public double? TurnsCalculatedStockRoomTurn { get; set; }
        public double? AUCalculatedDailyUsageOverSample { get; set; }
        public double? MinMaxCalculatedDailyUsageOverSample { get; set; }
        public double? CalculateAUByDay { get; set; }
        public double? AvgDailyPullsOverSample { get; set; }
        public double? MinMaxCalcualtedMax { get; set; }
        public double MinMaxCalcAvgPullByDay { get; set; }
        public int ParameterType { get; set; }
        public int? GraphFromMonth { get; set; }
        public int? GraphToMonth { get; set; }
        public int? GraphFromYear { get; set; }
        public int? GraphToYear { get; set; }
        public bool IsTrendingEnabled { get; set; }
        public byte? PieChartmetricOn { get; set; }
        public bool AutoClassification { get; set; }

        public Int64? MonthlyAverageUsage { get; set; }
        public double AnnualCarryingCostPercent { get; set; }
        public double LargestAnnualCashSavings { get; set; }
    }
}
