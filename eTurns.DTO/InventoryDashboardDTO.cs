using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class InventoryDashboardDTO
    {
        public string Year { get; set; }
        public SelectList YearList
        {
            get
            {
                return new SelectList(Years, "Value", "Text", Year);
            }
        }
        public IEnumerable<SelectListItem> Years
        {
            get
            {
                return new SelectList(Enumerable.Range(DateTime.Now.Year - 10, 11).OrderByDescending(year => year)
                    .Select(year => new SelectListItem
                    {
                        Value = year.ToString(),
                        Text = year.ToString()
                    }
                ), "Value", "Text");
            }
        }

        public int? SelectedMonth { get; set; }

        public SelectList MonthList
        {
            get
            {
                return new SelectList(AllMonths, "Value", "Text", SelectedMonth);
            }
        }

        public IEnumerable<SelectListItem> AllMonths
        {
            get
            {
                CultureInfo info =
                   System.Threading.Thread.CurrentThread.CurrentCulture;

                int index = 1;
                foreach (var monthName in info.DateTimeFormat.MonthNames)
                {
                    yield return new SelectListItem
                    {
                        Value = index.ToString(),
                        Text = monthName,
                    };
                    ++index;
                }
            }
        }

        public global::System.String ItemNumber { get; set; }
        public global::System.String BinNumber { get; set; }
        public global::System.Int64 BinID { get; set; }
        public global::System.Guid GUID { get; set; }
        public global::System.String Description { get; set; }
        public global::System.String Category { get; set; }
        public Nullable<global::System.Decimal> AvailableQty { get; set; }
        public Nullable<global::System.Decimal> InventoryValue { get; set; }
        public Nullable<global::System.Decimal> Turns { get; set; }
        public Nullable<global::System.Decimal> PullTurnsAmt { get; set; }
        public Nullable<global::System.Decimal> OrderTurnsAmt { get; set; }
        public global::System.Double CurrentMin { get; set; }
        public Nullable<global::System.Decimal> CalculatedMin { get; set; }
        public Nullable<global::System.Decimal> MinPercentage { get; set; }
        public Nullable<global::System.Double> AutoCurrentMin { get; set; }
        public Nullable<global::System.Decimal> AutoMinPercentage { get; set; }
        public global::System.Double CurrentMax { get; set; }
        public Nullable<global::System.Decimal> CalculatedMax { get; set; }
        public Nullable<global::System.Decimal> MaxPercentage { get; set; }
        public Nullable<global::System.Decimal> AutoCurrentMax { get; set; }
        public Nullable<global::System.Decimal> AutoMaxPercentage { get; set; }
        public global::System.String MinAnalysis { get; set; }
        public global::System.String MaxAnalysis { get; set; }

    }

}
