using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;


namespace eTurnsWeb.Controllers
{
    public class InventoryChartController : eTurnsControllerBase
    {
        #region [Global parameters]
        int MaxItemsInGraph = Convert.ToInt32(ConfigurationManager.AppSettings["MaxItemsInGraph"]);
        #endregion
        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component
        public FileResult CreateChart(SeriesChartType chartType)
        {
            //SeriesChartType chartType = SeriesChartType.Column;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            IList<ItemMasterDTO> peoples = null;
            string SearchCteriaCombined = string.Empty;

            if (Session["InventorySearchCriteria"] != null)
                SearchCteriaCombined = Session["InventorySearchCriteria"].ToString();

            string TextSeriesName, TextLegendName, TextChartArea, TextChartTitle = string.Empty;

            Dictionary<string, double> dicCriticl = new Dictionary<string, double>();
            Dictionary<string, double> dicInveValueTurns = new Dictionary<string, double>();

            string Criteria = string.Empty;
            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());
                Criteria = CriteriaFields[4];

                FromDate = FromDate.AddHours(23);
                ToDate = ToDate.AddHours(23);

                if (Criteria == "Stock Out")
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetStockOutData(SessionHelper.RoomID, SessionHelper.CompanyID, FromDate, ToDate).ToList();
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double StockOutCount = 0;
                        StockOutCount = Convert.ToDouble(item.LeadTimeInDays);
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, StockOutCount);
                    }
                    TextLegendName = "Stock Out";
                    TextChartArea = "Stock Out";
                    TextChartTitle = "Stock Out Chart";
                    TextSeriesName = "Stock Out=> X-Axis:Item Number Y-Axis:Number of Stock Out";
                }
                else if (Criteria == "Critical")
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria).ToList();

                    foreach (ItemMasterDTO item in peoples)
                    {
                        double itemCrtclQTY = 0;
                        if (item.CriticalQuantity != 0)
                        {
                            itemCrtclQTY = 100 - ((100.00 * item.OnHandQuantity.GetValueOrDefault(0)) / item.CriticalQuantity ?? 0);
                        }
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, itemCrtclQTY);

                    }
                    TextLegendName = "Critical";
                    TextChartArea = "Critical";
                    TextChartTitle = "Critical Chart";
                    TextSeriesName = "Critical=> X-Axis:Item Number Y-Axis:% of Critical";
                }
                else if (Criteria == "Minimum")
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria).Where(x => x.MinimumQuantity > 0).OrderBy(x => x.MinimumQuantity).Take(10).ToList();
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double itemCrtclQTY = 0;
                        if (item.MinimumQuantity != 0)
                        {
                            itemCrtclQTY = item.MinimumQuantity.GetValueOrDefault(0);//100 - ((100.00 * item.OnHandQuantity.GetValueOrDefault(0)) / item.MinimumQuantity);
                        }
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                    }
                    TextLegendName = "Minimum";
                    TextChartArea = "Minimum";
                    TextChartTitle = "Minimum Chart";
                    TextSeriesName = "Minimum=> X-Axis:Item Number Y-Axis:% of Minimum";
                }
                else if (Criteria == "Maximum")
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria).Where(x => x.MaximumQuantity > 0).OrderBy(x => x.MaximumQuantity).Take(10).ToList();
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double itemCrtclQTY = 0;
                        if (item.MaximumQuantity.GetValueOrDefault(0) != 0)
                        {
                            itemCrtclQTY = item.MaximumQuantity.GetValueOrDefault(0);//(100.00 * item.OnHandQuantity.GetValueOrDefault(0)) / item.MaximumQuantity;
                        }
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                    }
                    TextLegendName = "Maximum";
                    TextChartArea = "Maximum";
                    TextChartTitle = "Maximum Chart";
                    TextSeriesName = "Maximum=> X-Axis:Item Number Y-Axis:% of Maximum";
                }
                else if (Criteria == "Slow Moving")
                {
                    peoples = new List<ItemMasterDTO>(); //objItemDAL.GetMovingData(SessionHelper.RoomID, SessionHelper.CompanyID, FromDate, ToDate, true);
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double StockOutCount = 0;
                        StockOutCount = Convert.ToDouble(item.Turns);
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, StockOutCount);
                    }
                    TextLegendName = "Slow Moving";
                    TextChartArea = "Slow Moving";
                    TextChartTitle = "Slow Moving Chart";
                    TextSeriesName = "Slow Moving=> X-Axis:Item Number Y-Axis:Item Turns Value";
                }
                else if (Criteria == "Fast Moving")
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetMovingData(SessionHelper.RoomID, SessionHelper.CompanyID, FromDate, ToDate, false);
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double StockOutCount = 0;
                        StockOutCount = Convert.ToDouble(item.Turns);
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, StockOutCount);
                    }
                    TextLegendName = "Fast Moving";
                    TextChartArea = "Fast Moving";
                    TextChartTitle = "Fast Moving Chart";
                    TextSeriesName = "Fast Moving=> X-Axis:Item Number Y-Axis:Item Turns Value";
                }
                else if (Criteria == "InvValue")
                {

                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetStockOutDataForInvValue(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();


                    foreach (ItemMasterDTO item in peoples)
                    {
                        List<InventoryDashboardDTO> InveData = new List<InventoryDashboardDTO>();//objItemDAL.GetInveValueData(SessionHelper.RoomID, SessionHelper.CompanyID, item.MonthValue, DateTimeUtility.DateTimeNow.Year);

                        Int32 StockOutCount = 0;
                        //ItemMasterDTO ObjItemDTO = peoples.Where(x => x.ItemNumber == item.ItemNumber).SingleOrDefault();
                        //if (ObjItemDTO != null)
                        StockOutCount = item.StockOutCount.GetValueOrDefault(0);

                        //string ItemNumber = item.ItemNumber + " ( " + InvMonthValue + " )( " + StockOutCount + " )";
                        string ItemNumber = GetMonthName(item.MonthValue) + " ( " + StockOutCount + " )";
                        if (!dicCriticl.ContainsKey(ItemNumber))
                            dicCriticl.Add(ItemNumber, Convert.ToDouble(InveData.Sum(x => x.InventoryValue.GetValueOrDefault(0))));

                        if (!dicInveValueTurns.ContainsKey(ItemNumber))
                            dicInveValueTurns.Add(ItemNumber, Convert.ToDouble(InveData.Sum(x => x.Turns.GetValueOrDefault(0))));
                    }
                    TextLegendName = "Inventory Value";
                    TextChartArea = "Inventory Value";
                    TextChartTitle = "Inventory Value Chart";
                    TextSeriesName = "Inventory Value=> X-Axis:Month ( Stock Out Count ) Y-Axis:Inventory Value Z-Axis Turns Value";
                }
                else
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria).Take(10).ToList();
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double itemCrtclQTY = item.OnHandQuantity.GetValueOrDefault(0);
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                    }
                    TextLegendName = "Legend Name";
                    TextChartArea = "ChartArea";
                    TextChartTitle = "Chart Title";
                    TextSeriesName = "Series Name";
                }
            }
            else
            {
                peoples = objItemDAL.GetAllItemsWithoutJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null).Where(x => x.Cost.GetValueOrDefault(0) > 0).Take(5).ToList();
                foreach (ItemMasterDTO item in peoples)
                {
                    double itemCrtclQTY = item.OnHandQuantity.GetValueOrDefault(0);
                    if (!dicCriticl.ContainsKey(item.ItemNumber))
                        dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                }
                TextLegendName = "Legend Name";
                TextChartArea = "ChartArea";
                TextChartTitle = "Chart Title";
                TextSeriesName = "Series Name";
            }
            Chart chart = new Chart();
            chart.Width = 570;
            chart.Height = 250;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;

            chart.Titles.Add(CreateTitle(TextChartTitle));
            chart.Legends.Add(CreateLegend(TextLegendName));
            chart.Series.Add(CreateSeries(dicCriticl, chartType, TextSeriesName, TextChartArea));

            if (Criteria == "InvValue")
            {
                TextLegendName = "Inventory Value1";
                TextChartArea = "Inventory Value";
                TextChartTitle = "Inventory Value Chart1";
                TextSeriesName = "Turns Value";
                chart.Series.Add(CreateSeries2(dicInveValueTurns, SeriesChartType.Line, TextSeriesName, TextChartArea));
            }

            chart.ChartAreas.Add(CreateChartArea(TextChartArea));
            chart.ChartAreas[0].AxisX.IsMarginVisible = false;

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
        //public ActionResult LoadChart()
        //{
        //    return View("InventoryChart");
        //}
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart(string ChartType, string SelectedSupplier, string SelectedCategory)
        {
            if (string.IsNullOrWhiteSpace(ChartType))
            {
                ChartType = "invvalue";
            }
            ViewBag.MaxItemsInGraph = MaxItemsInGraph;
            ViewBag.Criteria = ChartType;
            ViewBag.SelectedSupplier = SelectedSupplier;
            ViewBag.SelectedCategory = SelectedCategory;
            if (ChartType == "InvValue")
            {
                return View("_InventoryChart");
            }
            else
            {
                return View("_ItemStockOut");
            }
        }

        [NonAction]
        public Title CreateTitle(string TitleText)
        {
            Title title = new Title();
            title.Text = TitleText;
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Calibri", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }
        [NonAction]
        public Legend CreateLegend(string LegendText)
        {
            Legend legend = new Legend();
            legend.Name = LegendText;
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Calibri"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }
        [NonAction]
        public Series CreateSeries(Dictionary<string, double> results, SeriesChartType chartType, string SeriesName, string ChartAreaText)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = SeriesName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            var resultsA = results.OrderByDescending(x => x.Value).Take(10).ToList();

            foreach (KeyValuePair<string, double> result in resultsA)
            {
                point = new DataPoint();
                point.AxisLabel = result.Key;
                point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = ChartAreaText;

            return seriesDetail;
        }
        [NonAction]
        public Series CreateSeries2(Dictionary<string, double> results, SeriesChartType chartType, string SeriesName, string ChartAreaText)
        {
            Series seriesDetail2 = new Series();
            seriesDetail2.Name = SeriesName;
            seriesDetail2.IsValueShownAsLabel = false;
            seriesDetail2.Color = Color.FromArgb(99, 198, 45);
            seriesDetail2.ChartType = chartType;
            seriesDetail2.BorderWidth = 2;
            seriesDetail2["DrawingStyle"] = "Cylinder";
            seriesDetail2["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            var resultsA = results.OrderByDescending(x => x.Value).Take(10).ToList();

            foreach (KeyValuePair<string, double> result in resultsA)
            {
                point = new DataPoint();
                //point.AxisLabel = result.Key;
                point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                point.Label = result.Value.ToString("00.00", CultureInfo.InvariantCulture);
                point.LabelForeColor = Color.Maroon;
                seriesDetail2.Points.Add(point);
            }
            //seriesDetail2.ChartArea = ChartAreaText;
            //seriesDetail2.XAxisType = AxisType.Secondary;
            seriesDetail2.YAxisType = AxisType.Secondary;
            return seriesDetail2;
        }
        [NonAction]
        public ChartArea CreateChartArea(string ChartAreaText)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = ChartAreaText;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            return chartArea;
        }
        [NonAction]
        public ChartArea CreateChartArea2(string ChartAreaText)
        {
            ChartArea chartArea2 = new ChartArea();
            chartArea2.Name = ChartAreaText;
            chartArea2.BackColor = Color.Transparent;
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisY.IsLabelAutoFit = false;
            chartArea2.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea2.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea2.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea2.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea2.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea2.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea2.AxisX.Interval = 1;
            return chartArea2;
        }
        [NonAction]
        public string GetMonthName(int MonthValue)
        {
            switch (MonthValue)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
                default:
                    return "";
            }
        }

        #endregion
    }
    public class ItemChartController : eTurnsControllerBase
    {
        #region Chart Component

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart()
        {
            return View("ItemChart");
        }
        public FileResult CreateChart(SeriesChartType chartType)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            IList<ItemMasterDTO> peoples = null;
            string SearchCteriaCombined = string.Empty;
            if (Session["InventorySearchCriteria"] != null)
                SearchCteriaCombined = Session["InventorySearchCriteria"].ToString();

            Dictionary<string, double> dicCriticl = new Dictionary<string, double>();

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());
                string Criteria = CriteriaFields[4];

                if (Criteria == "Critical")
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria).ToList();

                    foreach (ItemMasterDTO item in peoples)
                    {
                        double itemCrtclQTY = 100 - ((100.00 * item.OnHandQuantity.GetValueOrDefault(0)) / item.CriticalQuantity ?? 0);
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                    }
                }
                else if (Criteria == "Minimum")
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria).OrderBy(x => x.MinimumQuantity).Take(10).ToList();
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double itemCrtclQTY = 100 - ((100.00 * item.OnHandQuantity.GetValueOrDefault(0)) / item.MinimumQuantity.GetValueOrDefault(0));
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                    }
                }
                else if (Criteria == "Maximum")
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria).OrderBy(x => x.MaximumQuantity).Take(10).ToList();
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double itemCrtclQTY = (100.00 * item.OnHandQuantity.GetValueOrDefault(0)) / item.MaximumQuantity.GetValueOrDefault(0);
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                    }
                }
                else
                {
                    peoples = new List<ItemMasterDTO>();//objItemDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, FromDate, ToDate, Criteria).Take(10).ToList();
                    foreach (ItemMasterDTO item in peoples)
                    {
                        double itemCrtclQTY = item.OnHandQuantity.GetValueOrDefault(0);
                        if (!dicCriticl.ContainsKey(item.ItemNumber))
                            dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                    }
                }
            }
            else
            {
                peoples = objItemDAL.GetAllItemsWithoutJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null).Where(x => x.Cost.GetValueOrDefault(0) > 0).Take(5).ToList();
                foreach (ItemMasterDTO item in peoples)
                {
                    double itemCrtclQTY = item.OnHandQuantity.GetValueOrDefault(0);
                    if (!dicCriticl.ContainsKey(item.ItemNumber))
                        dicCriticl.Add(item.ItemNumber, itemCrtclQTY);
                }
            }
            Chart chart = new Chart();
            chart.Width = 400;
            chart.Height = 250;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(CreateTitle());
            chart.Legends.Add(CreateLegend());
            chart.Series.Add(CreateSeries(dicCriticl, chartType));
            chart.ChartAreas.Add(CreateChartArea());

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
        [NonAction]
        public Title CreateTitle()
        {
            Title title = new Title();
            title.Text = "Current Stock(on hand qty)";
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Calibri", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }
        [NonAction]
        public Legend CreateLegend()
        {
            Legend legend = new Legend();
            legend.Name = "Current Stock(on hand qty)";
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Calibri"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }
        [NonAction]
        public Series CreateSeries(Dictionary<string, double> results, SeriesChartType chartType)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = "Current Stock(on hand qty)";
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            var resultsA = results.OrderByDescending(x => x.Value).Take(10).ToList();

            foreach (KeyValuePair<string, double> result in resultsA)
            {
                point = new DataPoint();
                point.AxisLabel = result.Key;
                point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = "Current Stock(on hand qty)";

            return seriesDetail;
        }
        [NonAction]
        public ChartArea CreateChartArea()
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = "Current Stock(on hand qty)";
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            // chartArea.Position.Width = 98;
            // chartArea.Position.Height = 70;
            // chartArea.Position.Y = 15;
            // chartArea.Position.X = 0;
            //chartArea.Area3DStyle.Enable3D = true;
            /*chartArea.Area3DStyle.Rotation = 10;
            chartArea.Area3DStyle.Perspective = 10;
            chartArea.Area3DStyle.Inclination = 15;
            chartArea.Area3DStyle.IsRightAngleAxes=false;
            chartArea.Area3DStyle.WallWidth=0;
            chartArea.Area3DStyle.IsClustered=false;*/
            return chartArea;
        }
        #endregion
    }
    public class ConsumeChartController : eTurnsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component
        public FileResult CreateChart(SeriesChartType chartType)
        {
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            IList<RequisitionMasterDTO> lstRequisition = null;
            string SearchCteriaCombined = string.Empty;
            if (Session["ConsumeSearchCriteria"] != null)
                SearchCteriaCombined = Session["ConsumeSearchCriteria"].ToString();
            Dictionary<string, double> dicConsumes = new Dictionary<string, double>();

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());
                string Criteria = CriteriaFields[4];

                FromDate = FromDate.AddHours(23);
                ToDate = ToDate.AddHours(23);

                //lstRequisition = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted).ToList();
                //lstRequisition = lstRequisition.Where(t => t.Created.Value.Date >= FromDate.Date && t.Created.Value.Date <= ToDate.Date && t.RequisitionStatus == Criteria).OrderByDescending(t => t.NumberofItemsrequisitioned.GetValueOrDefault(0)).ToList();
                lstRequisition = obj.GetRequisitionByDateAndStatusDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, FromDate.Date, ToDate.Date, Criteria).OrderByDescending(t => t.NumberofItemsrequisitioned.GetValueOrDefault(0)).ToList();
                if (Criteria == "Unsubmitted")
                {
                    TextChartTitle = "Unsubmitted Requisition";
                    TextSeriesName = "Requisition=> X-Axis:Requisition Number Y-Axis:# of items";
                }
                else if (Criteria == "Submitted")
                {
                    TextChartTitle = "Unapproved Requisition";
                    TextSeriesName = "Requisition=> X-Axis:Requisition Number Y-Axis:# of items";
                }
                else if (Criteria == "Approved")
                {
                    TextChartTitle = "To be Pulled Requisition";
                    TextSeriesName = "Requisition=> X-Axis:Requisition Number Y-Axis:# of items";
                }
                else
                {
                    TextChartTitle = "Requisition";
                    TextSeriesName = "Requisition";
                }

                foreach (RequisitionMasterDTO item in lstRequisition)
                {
                    //TimeSpan? difference = ToDate.Date - item.Created;
                    //double TotalDays = difference.Value.TotalDays;
                    if (!dicConsumes.ContainsKey(item.RequisitionNumber))
                        dicConsumes.Add(item.RequisitionNumber, item.NumberofItemsrequisitioned.GetValueOrDefault(0));
                }
            }
            else
            {
                //lstRequisition = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Take(10).ToList();
                lstRequisition = obj.GetRequisitionByDateAndStatusDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, DateTime.Now.AddYears(-5).Date, DateTime.Now.AddYears(5).Date, null).OrderByDescending(t => t.NumberofItemsrequisitioned.GetValueOrDefault(0)).ToList();
                TextChartTitle = "Requisition";
                TextSeriesName = "Requisition";
            }
            Chart chart = new Chart();
            chart.Width = 570;
            chart.Height = 250;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(CreateTitle(TextChartTitle));
            chart.Legends.Add(CreateLegend(TextChartTitle));
            chart.Series.Add(CreateSeries(dicConsumes, chartType, TextSeriesName, TextChartTitle));
            chart.ChartAreas.Add(CreateChartArea(TextChartTitle));

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
        //public ActionResult LoadChart()
        //{
        // return View("ConsumeChart");
        //}
        public ActionResult LoadChartold()
        {

            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            IList<RequisitionMasterDTO> lstRequisition = null;
            string SearchCteriaCombined = string.Empty;
            if (Session["ConsumeSearchCriteria"] != null)
                SearchCteriaCombined = Session["ConsumeSearchCriteria"].ToString();
            Dictionary<string, double> dicConsumes = new Dictionary<string, double>();

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());
                string Criteria = CriteriaFields[4];

                FromDate = FromDate.AddHours(23);
                ToDate = ToDate.AddHours(23);

                //lstRequisition = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted).ToList();
                //lstRequisition = lstRequisition.Where(t => t.Created.Value.Date >= FromDate.Date && t.Created.Value.Date <= ToDate.Date && t.RequisitionStatus == Criteria).OrderByDescending(t => t.NumberofItemsrequisitioned.GetValueOrDefault(0)).ToList();
                lstRequisition = obj.GetRequisitionByDateAndStatusDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, FromDate.Date, ToDate.Date, Criteria).OrderByDescending(t => t.NumberofItemsrequisitioned.GetValueOrDefault(0)).ToList();

                if (Criteria == "Unsubmitted")
                {
                    TextChartTitle = ResDashboard.TitleUnsubmittedRequisition;// "Unsubmitted Requisition";

                }
                else if (Criteria == "Submitted")
                {
                    TextChartTitle = ResDashboard.TitleUnapprovedRequisition;// "Unapproved Requisition";

                }
                else if (Criteria == "Approved")
                {
                    TextChartTitle = ResDashboard.TitleTobePulledRequisition;// "To be Pulled Requisition";

                }
                else
                {
                    TextChartTitle = ResDashboard.TitleRequisition;// "Requisition";


                }

                foreach (RequisitionMasterDTO item in lstRequisition)
                {
                    //TimeSpan? difference = ToDate.Date - item.Created;
                    //double TotalDays = difference.Value.TotalDays;
                    if (item.NumberofItemsrequisitioned.GetValueOrDefault(0) > 0)
                    {
                        if (!dicConsumes.ContainsKey(item.RequisitionNumber))
                            dicConsumes.Add(item.RequisitionNumber, item.NumberofItemsrequisitioned.GetValueOrDefault(0));
                    }

                }
            }
            ViewBag.ConsumeChartTitle = TextChartTitle;
            ViewBag.Consumes = dicConsumes.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            return PartialView("_ConsumeChart");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart(string ChartType)
        {

            ViewBag.ChartType = ChartType;
            ViewBag.ReqStatuses = ChartHelper.GetReqStatus();
            ViewBag.ReqTypes = ChartHelper.GetReqTypes();
            //RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<RequisitionMasterDTO> lstRequisition = null;
            //string SearchCteriaCombined = string.Empty;

            //Dictionary<string, double> dicConsumes = new Dictionary<string, double>();

            //string TextSeriesName = string.Empty;
            //string TextChartTitle = string.Empty;
            //if (string.IsNullOrWhiteSpace(ChartType))
            //{
            //    ChartType = "Unsubmitted";
            //}

            //bool IsArchived = false;
            //bool IsDeleted = false;

            //lstRequisition = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted).ToList();
            //lstRequisition = lstRequisition.Where(t => t.RequisitionStatus == ChartType).OrderBy(t => t.NumberofItemsrequisitioned.GetValueOrDefault(0)).ToList();
            //if (ChartType == "Unsubmitted")
            //{
            //    TextChartTitle = ResDashboard.TitleUnsubmittedRequisition;// "Unsubmitted Requisition";

            //}
            //else if (ChartType == "Submitted")
            //{
            //    TextChartTitle = ResDashboard.TitleUnapprovedRequisition;// "Unapproved Requisition";

            //}
            //else if (ChartType == "Approved")
            //{
            //    TextChartTitle = ResDashboard.TitleTobePulledRequisition;// "To be Pulled Requisition";

            //}
            //else
            //{
            //    TextChartTitle = ResDashboard.TitleRequisition;// "Requisition";
            //}

            //foreach (RequisitionMasterDTO item in lstRequisition)
            //{
            //    if (item.NumberofItemsrequisitioned.GetValueOrDefault(0) > 0)
            //    {
            //        if (!dicConsumes.ContainsKey(item.RequisitionNumber))
            //            dicConsumes.Add(item.RequisitionNumber, item.NumberofItemsrequisitioned.GetValueOrDefault(0));
            //    }
            //}
            //ViewBag.ConsumeChartTitle = TextChartTitle;
            //ViewBag.Consumes = dicConsumes.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            return PartialView("_ConsumeChart");
        }
        [NonAction]
        public Title CreateTitle(string TitleText)
        {
            Title title = new Title();
            title.Text = TitleText;
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Calibri", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }
        [NonAction]
        public Legend CreateLegend(string TitleText)
        {
            Legend legend = new Legend();
            legend.Name = TitleText;
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Calibri"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }
        [NonAction]
        public Series CreateSeries(Dictionary<string, double> results, SeriesChartType chartType, string SeriesName, string ChartAreaText)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = SeriesName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            var resultsA = results.OrderByDescending(x => x.Value).Take(10).ToList();

            foreach (KeyValuePair<string, double> result in resultsA)
            {
                point = new DataPoint();
                point.AxisLabel = result.Key;
                point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = ChartAreaText;

            return seriesDetail;
        }
        [NonAction]
        public ChartArea CreateChartArea(string TitleText)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = TitleText;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;

            return chartArea;
        }
        #endregion
    }

    public class ProjectChartController : eTurnsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component
        public FileResult CreateChart(SeriesChartType chartType)
        {
            ProjectMasterDAL objProject = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            IList<ProjectMasterDTO> lstProject = null;

            Dictionary<string, double> dicProject = new Dictionary<string, double>();

            string SearchCteriaCombined = string.Empty;
            if (Session["ProjectSearchCriteria"] != null)
                SearchCteriaCombined = Session["ProjectSearchCriteria"].ToString();

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());
                string Status = CriteriaFields[4];
                string StatusValue = CriteriaFields[5];

                lstProject = objProject.GetProjectMaster(SessionHelper.RoomID, SessionHelper.CompanyID, FromDate, ToDate, Status, int.Parse(StatusValue)).ToList();

                if (Status == "Project Amount Exceeds")
                {
                    foreach (ProjectMasterDTO item in lstProject)
                    {
                        double itemCrtclQTY = (100 * double.Parse(item.DollarUsedAmount.Value.ToString())) / double.Parse(item.DollarLimitAmount.Value.ToString());
                        if (!dicProject.ContainsKey(item.ProjectSpendName))
                            dicProject.Add(item.ProjectSpendName, itemCrtclQTY);
                    }
                }
                else if (Status == "Item Quantity Exceeds")
                {
                    foreach (ProjectMasterDTO item in lstProject)
                    {
                        foreach (ProjectSpendItemsDTO subitem in item.ProjectSpendItems)
                        {
                            double itemCrtclQTY = (100 * double.Parse(subitem.QuantityUsed.Value.ToString())) / double.Parse(subitem.QuantityLimit.Value.ToString());
                            if (!dicProject.ContainsKey(item.ProjectSpendName))
                                dicProject.Add(item.ProjectSpendName, itemCrtclQTY);
                        }
                    }
                }
                else// if (Status == "Item Amount Exceeds")
                {
                    foreach (ProjectMasterDTO item in lstProject)
                    {
                        foreach (ProjectSpendItemsDTO subitem in item.ProjectSpendItems)
                        {
                            double itemCrtclQTY = (100 * double.Parse(subitem.DollarUsedAmount.Value.ToString())) / double.Parse(subitem.DollarLimitAmount.Value.ToString());
                            if (!dicProject.ContainsKey(item.ProjectSpendName))
                                dicProject.Add(item.ProjectSpendName, itemCrtclQTY);
                        }
                    }
                }
            }

            Chart chart = new Chart();
            chart.Width = 570;
            chart.Height = 250;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(CreateTitle());
            chart.Legends.Add(CreateLegend());
            chart.Series.Add(CreateSeries(dicProject, chartType));
            chart.ChartAreas.Add(CreateChartArea());

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
        //public ActionResult LoadChart()
        //{
        //    return View("ProjectChart");
        //}
        public ActionResult LoadChartOld()
        {
            ProjectMasterDAL objProject = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            IList<ProjectMasterDTO> lstProject = null;

            Dictionary<string, double> dicProject = new Dictionary<string, double>();
            string ChartTitle = string.Empty;
            string SearchCteriaCombined = string.Empty;
            if (Session["ProjectSearchCriteria"] != null)
                SearchCteriaCombined = Session["ProjectSearchCriteria"].ToString();

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());
                string Status = CriteriaFields[4];
                string StatusValue = CriteriaFields[5];

                lstProject = objProject.GetProjectMaster(SessionHelper.RoomID, SessionHelper.CompanyID, FromDate, ToDate, Status, int.Parse(StatusValue)).ToList();

                if (Status == "Project Amount Exceeds")
                {
                    foreach (ProjectMasterDTO item in lstProject)
                    {
                        double itemCrtclQTY = (100 * double.Parse(item.DollarUsedAmount.Value.ToString())) / double.Parse(item.DollarLimitAmount.Value.ToString());
                        if (itemCrtclQTY > 0)
                        {
                            if (!dicProject.ContainsKey(item.ProjectSpendName))
                                dicProject.Add(item.ProjectSpendName, itemCrtclQTY);
                        }

                    }
                    ChartTitle = ResDashboard.TitleProjectAmountExceeds;
                }
                else if (Status == "Item Quantity Exceeds")
                {
                    foreach (ProjectMasterDTO item in lstProject)
                    {
                        foreach (ProjectSpendItemsDTO subitem in item.ProjectSpendItems)
                        {
                            double itemCrtclQTY = (100 * double.Parse(subitem.QuantityUsed.Value.ToString())) / double.Parse(subitem.QuantityLimit.Value.ToString());
                            if (itemCrtclQTY > 0)
                            {
                                if (!dicProject.ContainsKey(item.ProjectSpendName))
                                    dicProject.Add(item.ProjectSpendName, itemCrtclQTY);
                            }

                        }
                    }
                    ChartTitle = ResDashboard.TitleProjectQuantityExceeds;
                }
                else// if (Status == "Item Amount Exceeds")
                {
                    foreach (ProjectMasterDTO item in lstProject)
                    {
                        foreach (ProjectSpendItemsDTO subitem in item.ProjectSpendItems)
                        {
                            double itemCrtclQTY = (100 * double.Parse(subitem.DollarUsedAmount.Value.ToString())) / double.Parse(subitem.DollarLimitAmount.Value.ToString());
                            if (itemCrtclQTY > 0)
                            {
                                if (!dicProject.ContainsKey(item.ProjectSpendName))
                                    dicProject.Add(item.ProjectSpendName, itemCrtclQTY);
                            }

                        }
                    }
                    ChartTitle = ResDashboard.TitleItemAmountExceeds;
                }
            }
            ViewBag.ProjectTitle = ChartTitle;
            ViewBag.ProjectdicProject = dicProject.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            return View("_ProjectSpendChart");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart(string ChartType, int? StatusValue)
        {
            ViewBag.ChartType = ChartType;
            return View("_ProjectSpendChart");
        }

        [NonAction]
        public Title CreateTitle()
        {
            Title title = new Title();
            title.Text = "Project Spend Usage";
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Calibri", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }
        [NonAction]
        public Legend CreateLegend()
        {
            Legend legend = new Legend();
            legend.Name = "Project Spend Usage";
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Calibri"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }
        [NonAction]
        public Series CreateSeries(Dictionary<string, double> dicProject, SeriesChartType chartType)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = "Project Spend Usage";
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;


            var resultsA = dicProject.OrderByDescending(x => x.Value).Take(10).ToList();

            foreach (KeyValuePair<string, double> result in resultsA)
            {
                point = new DataPoint();
                point.AxisLabel = result.Key;
                point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = "Project Spend Usage";

            return seriesDetail;
        }
        [NonAction]
        public ChartArea CreateChartArea()
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = "Project Spend Usage";
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;

            return chartArea;
        }
        #endregion
    }

    public class ToolChartController : eTurnsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component
        public FileResult CreateChart(SeriesChartType chartType)
        {
            ToolsMaintenanceDAL objTools = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            IList<ToolsMaintenanceDTO> lstTools = null;
            string SearchCteriaCombined = string.Empty;
            if (Session["ToolsSearchCriteria"] != null)
                SearchCteriaCombined = Session["ToolsSearchCriteria"].ToString();
            Dictionary<string, double> dicTools = new Dictionary<string, double>();

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;
            TextChartTitle = "Tool Maintenance Due";
            TextSeriesName = "Tool Maintenance=> X-Axis:Tool/Maintenance Name Y-Axis:Next 10 Tools";

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                Int32 Days = int.Parse(CriteriaFields[2]);

                lstTools = objTools.ToolsAssetMaintenceDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, Days).Where(x => x.ToolGUID.HasValue).Take(10).ToList();
                foreach (ToolsMaintenanceDTO item in lstTools)
                {
                    if (item.ScheduleDate.HasValue)
                    {
                        TimeSpan? difference = item.ScheduleDate - DateTimeUtility.DateTimeNow.Date;
                        double TotalDays = difference.Value.TotalDays;
                        /*Comment for dll optimization if requied then uncomment*/
                        //string ToolName = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.ToolGUID.Value, null).ToolName;
                        /*Comment for dll optimization if requied then uncomment*/
                        string ToolName = "";
                        string DisplayName = string.Empty;
                        if (!string.IsNullOrEmpty(ToolName))
                            DisplayName = "( " + ToolName + " )" + item.MaintenanceName;
                        else
                            DisplayName = item.MaintenanceName;
                        if (!dicTools.ContainsKey(DisplayName))
                            dicTools.Add(DisplayName, Days);
                    }
                }
            }
            else
            {
                lstTools = objTools.ToolsAssetMaintenceDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, 0).Where(x => x.ToolGUID.HasValue).Take(10).ToList();
            }
            Chart chart = new Chart();
            chart.Width = 570;
            chart.Height = 250;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(CreateTitle(TextChartTitle));
            chart.Legends.Add(CreateLegend(TextChartTitle));
            chart.Series.Add(CreateSeries(dicTools, chartType, TextSeriesName, TextChartTitle));
            chart.ChartAreas.Add(CreateChartArea(TextChartTitle));

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
        //public ActionResult LoadChart()
        //{
        //    return View("ToolChart");
        //}

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChartOld()
        {
            ToolsMaintenanceDAL objTools = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            IList<ToolsMaintenanceDTO> lstTools = null;
            string SearchCteriaCombined = string.Empty;
            if (Session["ToolsSearchCriteria"] != null)
                SearchCteriaCombined = Session["ToolsSearchCriteria"].ToString();
            Dictionary<string, double> dicTools = new Dictionary<string, double>();

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;
            TextChartTitle = ResDashboard.TitleToolMaintenance;// "Tool Maintenance Due";
            TextSeriesName = "Tool Maintenance=> X-Axis:Tool/Maintenance Name Y-Axis:Next 10 Tools";

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                Int32 Days = int.Parse(CriteriaFields[2]);

                lstTools = objTools.ToolsAssetMaintenceDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, Days).Where(x => x.ToolGUID.HasValue).Take(10).ToList();
                foreach (ToolsMaintenanceDTO item in lstTools)
                {
                    if (item.ScheduleDate.HasValue)
                    {
                        TimeSpan? difference = item.ScheduleDate - DateTimeUtility.DateTimeNow.Date;
                        double TotalDays = difference.Value.TotalDays;
                        /*Comment for dll optimization if requied then uncomment*/
                        //string ToolName = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.ToolGUID.Value, null).ToolName;
                        /*Comment for dll optimization if requied then uncomment*/
                        string ToolName = "";
                        string DisplayName = string.Empty;
                        if (!string.IsNullOrEmpty(ToolName))
                            DisplayName = "( " + ToolName + " )" + item.MaintenanceName;
                        else
                            DisplayName = item.MaintenanceName;
                        if (!dicTools.ContainsKey(DisplayName))
                            dicTools.Add(DisplayName, Days);
                    }
                }
            }
            else
            {
                lstTools = objTools.ToolsAssetMaintenceDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, 0).Where(x => x.ToolGUID.HasValue).Take(10).ToList();
            }
            ViewBag.TooldicTools = dicTools.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            ViewBag.ToolTitle = TextChartTitle;
            return View("_ToolChart");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart()
        {
            return View("_ToolChart");
        }
        [NonAction]
        public Title CreateTitle(string TitleText)
        {
            Title title = new Title();
            title.Text = TitleText;
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Calibri", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }
        [NonAction]
        public Legend CreateLegend(string TitleText)
        {
            Legend legend = new Legend();
            legend.Name = TitleText;
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Calibri"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }
        [NonAction]
        public Series CreateSeries(Dictionary<string, double> lstTools, SeriesChartType chartType, string SeriesName, string ChartAreaText)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = SeriesName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            var resultsA = lstTools.OrderBy(x => x.Value).Take(10).ToList();

            foreach (KeyValuePair<string, double> result in resultsA)
            {
                point = new DataPoint();
                point.AxisLabel = result.Key;
                point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = ChartAreaText;

            return seriesDetail;
        }
        [NonAction]
        public ChartArea CreateChartArea(string TitleText)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = TitleText;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;

            return chartArea;
        }
        #endregion
    }

    public class AssetChartController : eTurnsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component
        public FileResult CreateChart(SeriesChartType chartType)
        {
            ToolsMaintenanceDAL objTools = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            IList<ToolsMaintenanceDTO> lstTools = null;
            string SearchCteriaCombined = string.Empty;
            if (Session["AssetSearchCriteria"] != null)
                SearchCteriaCombined = Session["AssetSearchCriteria"].ToString();
            Dictionary<string, double> dicTools = new Dictionary<string, double>();

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;
            TextChartTitle = "Asset Maintenance Due";
            TextSeriesName = "Asset Maintenance=> X-Axis:Asset/Maintenance Name Y-Axis:Next 10 Assets";

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                Int32 Days = int.Parse(CriteriaFields[2]);

                lstTools = objTools.ToolsAssetMaintenceDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, Days).Where(x => x.AssetGUID.HasValue).Take(10).ToList();
                foreach (ToolsMaintenanceDTO item in lstTools)
                {
                    if (item.ScheduleDate.HasValue)
                    {
                        TimeSpan? difference = item.ScheduleDate - DateTimeUtility.DateTimeNow.Date;
                        double TotalDays = difference.Value.TotalDays;
                        string AssetName = new AssetMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(item.AssetGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).AssetName;
                        string DisplayName = string.Empty;
                        if (!string.IsNullOrEmpty(AssetName))
                            DisplayName = "( " + AssetName + " )" + item.MaintenanceName;
                        else
                            DisplayName = item.MaintenanceName;
                        if (!dicTools.ContainsKey(DisplayName))
                            dicTools.Add(DisplayName, Days);
                    }
                }
            }
            else
            {
                lstTools = objTools.ToolsAssetMaintenceDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, 0).Where(x => x.ToolGUID.HasValue).Take(10).ToList();
            }
            Chart chart = new Chart();
            chart.Width = 570;
            chart.Height = 250;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(CreateTitle(TextChartTitle));
            chart.Legends.Add(CreateLegend(TextChartTitle));
            chart.Series.Add(CreateSeries(dicTools, chartType, TextSeriesName, TextChartTitle));
            chart.ChartAreas.Add(CreateChartArea(TextChartTitle));

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
        //public ActionResult LoadChart()
        //{
        //    return View("AssetChart");
        //}
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChartOld()
        {
            ToolsMaintenanceDAL objTools = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            IList<ToolsMaintenanceDTO> lstTools = null;
            string SearchCteriaCombined = string.Empty;
            if (Session["AssetSearchCriteria"] != null)
                SearchCteriaCombined = Session["AssetSearchCriteria"].ToString();
            Dictionary<string, double> dicTools = new Dictionary<string, double>();

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;
            TextChartTitle = ResDashboard.TitleAssetMaintenance;// "Asset Maintenance Due";
            TextSeriesName = "Asset Maintenance=> X-Axis:Asset/Maintenance Name Y-Axis:Next 10 Assets";

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                Int32 Days = int.Parse(CriteriaFields[2]);

                lstTools = objTools.ToolsAssetMaintenceDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, Days).Where(x => x.AssetGUID.HasValue).Take(10).ToList();
                foreach (ToolsMaintenanceDTO item in lstTools)
                {
                    if (item.ScheduleDate.HasValue)
                    {
                        TimeSpan? difference = item.ScheduleDate - DateTimeUtility.DateTimeNow.Date;
                        double TotalDays = difference.Value.TotalDays;
                        string AssetName = new AssetMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(item.AssetGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).AssetName;
                        string DisplayName = string.Empty;
                        if (!string.IsNullOrEmpty(AssetName))
                            DisplayName = "( " + AssetName + " )" + item.MaintenanceName;
                        else
                            DisplayName = item.MaintenanceName;
                        if (!dicTools.ContainsKey(DisplayName))
                            dicTools.Add(DisplayName, Days);
                    }
                }
            }
            else
            {
                lstTools = objTools.ToolsAssetMaintenceDashboard(SessionHelper.RoomID, SessionHelper.CompanyID, 0).Where(x => x.ToolGUID.HasValue).Take(10).ToList();
            }
            ViewBag.AssetdicTools = dicTools.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            ViewBag.AssetTitle = TextChartTitle;
            return View("_AssetChart");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart()
        {
            return View("_AssetChart");
        }

        [NonAction]
        public Title CreateTitle(string TitleText)
        {
            Title title = new Title();
            title.Text = TitleText;
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Calibri", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }
        [NonAction]
        public Legend CreateLegend(string TitleText)
        {
            Legend legend = new Legend();
            legend.Name = TitleText;
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Calibri"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }
        [NonAction]
        public Series CreateSeries(Dictionary<string, double> lstTools, SeriesChartType chartType, string SeriesName, string ChartAreaText)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = SeriesName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            var resultsA = lstTools.OrderBy(x => x.Value).Take(10).ToList();

            foreach (KeyValuePair<string, double> result in resultsA)
            {
                point = new DataPoint();
                point.AxisLabel = result.Key;
                point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = ChartAreaText;

            return seriesDetail;
        }
        [NonAction]
        public ChartArea CreateChartArea(string TitleText)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = TitleText;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;

            return chartArea;
        }
        #endregion
    }

    public class OrderChartController : eTurnsControllerBase
    {

        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component
        public FileResult CreateChart(SeriesChartType chartType)
        {
            OrderMasterDAL controller = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            IList<OrderMasterDTO> lstOrder = null;
            Dictionary<string, double> dicOrders = new Dictionary<string, double>();
            string SearchCteriaCombined = string.Empty;
            if (Session["OrderSearchCriteria"] != null)
                SearchCteriaCombined = Session["OrderSearchCriteria"].ToString();
            string OrderStatusKey = string.Empty;

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());
                OrderStatusKey = CriteriaFields[4].ToString();
                //Int32 OrderStatusValue = 0;

                FromDate = FromDate.AddHours(23);
                ToDate = ToDate.AddHours(23);

                if (OrderStatusKey == "Unsubmitted")
                {
                    //OrderStatusValue = (int)OrderStatus.UnSubmitted;
                    TextChartTitle = "Unsubmitted Order";
                    TextSeriesName = "Order=> X-Axis:Order # Y-Axis:# of items";
                }
                else if (OrderStatusKey == "Submitted")
                {
                    //OrderStatusValue = (int)OrderStatus.Submitted;
                    TextChartTitle = "Unapproved Order";
                    TextSeriesName = "Order=> X-Axis:Order # Y-Axis:# of items";
                }
                else if (OrderStatusKey == "Approved")
                {
                    //OrderStatusValue = (int)OrderStatus.TransmittedPastDue;
                    TextChartTitle = "To be Received Order";
                    TextSeriesName = "Order=> X-Axis:Order # Y-Axis:# of items";
                }

                //lstOrder = controller.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, OrderType.Order).ToList();
                //lstOrder = lstOrder.Where(t => t.Created.Value.Date >= FromDate.Date && t.Created.Value.Date <= ToDate.Date && t.OrderStatus == OrderStatusValue && t.NoOfLineItems.GetValueOrDefault(0) > 0).ToList();
                foreach (OrderMasterDTO item in lstOrder)
                {
                    TimeSpan? difference = item.RequiredDate - DateTimeUtility.DateTimeNow.Date;
                    double TotalDays = difference.Value.TotalDays;
                    if (!dicOrders.ContainsKey(item.OrderNumber))
                        dicOrders.Add(item.OrderNumber, item.NoOfLineItems.GetValueOrDefault(0));
                }
            }

            Chart chart = new Chart();
            chart.Width = 570;
            chart.Height = 250;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(CreateTitle(TextChartTitle));
            chart.Legends.Add(CreateLegend(TextChartTitle));
            chart.Series.Add(CreateSeries(dicOrders, chartType, OrderStatusKey, TextSeriesName, TextChartTitle));
            chart.ChartAreas.Add(CreateChartArea(TextChartTitle));

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
        //public ActionResult LoadChart()
        //{
        //    return View("OrderChart");
        //}

        public ActionResult LoadChartOld(string ChartType)
        {
            OrderMasterDAL controller = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            IList<OrderMasterDTO> lstOrder = null;
            Dictionary<string, double> dicOrders = new Dictionary<string, double>();
            string SearchCteriaCombined = string.Empty;
            if (Session["OrderSearchCriteria"] != null)
                SearchCteriaCombined = Session["OrderSearchCriteria"].ToString();
            string OrderStatusKey = string.Empty;

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());
                OrderStatusKey = CriteriaFields[4].ToString();
                //Int32 OrderStatusValue = 0;

                FromDate = FromDate.AddHours(23);
                ToDate = ToDate.AddHours(23);

                if (OrderStatusKey == "Unsubmitted")
                {
                    //OrderStatusValue = (int)OrderStatus.UnSubmitted;
                    TextChartTitle = ResDashboard.TitleUnsubmittedOrder;// "Unsubmitted Order";
                    TextSeriesName = "Order=> X-Axis:Order # Y-Axis:# of items";
                }
                else if (OrderStatusKey == "Submitted")
                {
                    // OrderStatusValue = (int)OrderStatus.Submitted;
                    TextChartTitle = ResDashboard.TitleUnapprovedOrder;// "Unapproved Order";
                    TextSeriesName = "Order=> X-Axis:Order # Y-Axis:# of items";
                }
                else if (OrderStatusKey == "Approved")
                {
                    //OrderStatusValue = (int)OrderStatus.TransmittedPastDue;
                    TextChartTitle = ResDashboard.TitleTobeReceivedOrder;// "To be Received Order";
                    TextSeriesName = "Order=> X-Axis:Order # Y-Axis:# of items";
                }

                //lstOrder = controller.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, OrderType.Order).ToList();
                //lstOrder = lstOrder.Where(t => t.Created.Value.Date >= FromDate.Date && t.Created.Value.Date <= ToDate.Date && t.OrderStatus == OrderStatusValue && t.NoOfLineItems.GetValueOrDefault(0) > 0).ToList();
                foreach (OrderMasterDTO item in lstOrder)
                {
                    TimeSpan? difference = item.RequiredDate - DateTimeUtility.DateTimeNow.Date;
                    double TotalDays = difference.Value.TotalDays;
                    if (!dicOrders.ContainsKey(item.OrderNumber))
                        dicOrders.Add(item.OrderNumber, item.NoOfLineItems.GetValueOrDefault(0));
                }
            }
            ViewBag.OrderdicOrders = dicOrders.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            ViewBag.ChartTitle = TextChartTitle;
            return View("_OrderChart");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart(string ChartType, string SelectedSupplier)
        {
            ViewBag.ChartType = ChartType;
            ViewBag.SelectedSupplier = SelectedSupplier ?? "";
            return View("_OrderChart");
        }

        [NonAction]
        public Title CreateTitle(string TitleText)
        {
            Title title = new Title();
            title.Text = TitleText;
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Calibri", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }
        [NonAction]
        public Legend CreateLegend(string TitleText)
        {
            Legend legend = new Legend();
            legend.Name = TitleText;
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Calibri"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }
        [NonAction]
        public Series CreateSeries(Dictionary<string, double> lstOrders, SeriesChartType chartType, string OrderStatusKey, string SeriesName, string ChartAreaText)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = SeriesName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;
            if (OrderStatusKey == "Approved")
            {
                var resultsA = lstOrders.OrderByDescending(x => x.Value).Take(10).ToList();
                foreach (KeyValuePair<string, double> result in resultsA)
                {
                    point = new DataPoint();
                    point.AxisLabel = result.Key;
                    point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                    seriesDetail.Points.Add(point);
                }
            }
            else
            {
                var resultsA = lstOrders.OrderBy(x => x.Value).Take(10).ToList();
                foreach (KeyValuePair<string, double> result in resultsA)
                {
                    point = new DataPoint();
                    point.AxisLabel = result.Key;
                    point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                    seriesDetail.Points.Add(point);
                }
            }

            seriesDetail.ChartArea = ChartAreaText;
            return seriesDetail;
        }
        [NonAction]
        public ChartArea CreateChartArea(string TitleText)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = TitleText;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;

            return chartArea;
        }
        #endregion
    }

    public class TransferChartController : eTurnsControllerBase
    {
        int MaxItemsInGraph = Convert.ToInt32(ConfigurationManager.AppSettings["MaxItemsInGraph"]);
        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component
        public FileResult CreateChart(SeriesChartType chartType)
        {
            TransferMasterDAL controller = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            IList<TransferMasterDTO> lstTransfer = null;
            Dictionary<string, double> dicTransfer = new Dictionary<string, double>();
            string SearchCteriaCombined = string.Empty;
            if (Session["TransferSearchCriteria"] != null)
                SearchCteriaCombined = Session["TransferSearchCriteria"].ToString();
            string TransferStatusKey = string.Empty;

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());

                FromDate = FromDate.AddHours(23);
                ToDate = ToDate.AddHours(23);

                TransferStatusKey = CriteriaFields[4].ToString();
                Int32 TransferStatusValue = 0;
                if (TransferStatusKey == "Unsubmitted")
                {
                    TransferStatusValue = (int)TransferStatus.UnSubmitted;
                    TextChartTitle = "Unsubmitted Transfers";
                    TextSeriesName = "Transfer=> X-Axis:Transfer # Y-Axis:# of items";
                }
                else if (TransferStatusKey == "Submitted")
                {
                    TransferStatusValue = (int)TransferStatus.Submitted;
                    TextChartTitle = "Unapproved Transfers";
                    TextSeriesName = "Transfer=> X-Axis:Transfer # Y-Axis:# of items";
                }
                else if (TransferStatusKey == "Approved")
                {
                    TransferStatusValue = (int)TransferStatus.TransmittedPastDue;
                    TextChartTitle = "To be Received Transfers";
                    TextSeriesName = "Transfer=> X-Axis:Transfer # Y-Axis:# of items";
                }

                //lstTransfer = controller.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted).ToList();
                //lstTransfer = lstTransfer.Where(t => t.Created.Date >= FromDate.Date && t.Created.Date <= ToDate.Date && t.TransferStatus == TransferStatusValue && t.NoOfItems > 0).OrderByDescending(t => t.NoOfItems).ToList();
                foreach (TransferMasterDTO item in lstTransfer)
                {
                    TimeSpan? difference = item.RequireDate - DateTimeUtility.DateTimeNow.Date;
                    double TotalDays = difference.Value.TotalDays;
                    if (!dicTransfer.ContainsKey(item.TransferNumber))
                        dicTransfer.Add(item.TransferNumber, item.NoOfItems ?? 0);
                }
            }
            else
            {
                //lstTransfer = controller.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Take(10).ToList();
            }
            Chart chart = new Chart();
            chart.Width = 570;
            chart.Height = 250;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(CreateTitle(TextChartTitle));
            chart.Legends.Add(CreateLegend(TextChartTitle));
            chart.Series.Add(CreateSeries(dicTransfer, chartType, TransferStatusKey, TextSeriesName, TextChartTitle));
            chart.ChartAreas.Add(CreateChartArea(TextChartTitle));

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }
        //public ActionResult LoadChart()
        //{
        //    return View("TransferChart");
        //}

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart(string ChartType)
        {
            string TextChartTitle = string.Empty;

            if (!string.IsNullOrEmpty(ChartType))
            {
                if (ChartType == "Unsubmitted")
                {
                    TextChartTitle = ResDashboard.TitleUnsubmittedTransfers;// "Unsubmitted Transfers";
                }
                else if (ChartType == "ToBeApproved")
                {
                    TextChartTitle = ResDashboard.TitleUnapprovedTransfers;// "Unapproved Transfers";
                }
                else if (ChartType == "Receive")
                {
                    TextChartTitle = ResDashboard.TitleTobeReceivedTransfers;// "To be Received Transfers";
                }
            }

            ViewBag.ChartType = ChartType;
            ViewBag.TransferTextChartTitle = TextChartTitle;
            return View("_TransferChart");
        }

        public ActionResult LoadChartOld(string ChartType)
        {
            TransferMasterDAL controller = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            IList<TransferMasterDTO> lstTransfer = null;
            Dictionary<string, double> dicTransfer = new Dictionary<string, double>();
            string SearchCteriaCombined = string.Empty;
            if (Session["TransferSearchCriteria"] != null)
                SearchCteriaCombined = Session["TransferSearchCriteria"].ToString();
            string TransferStatusKey = string.Empty;

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;

            if (SearchCteriaCombined.Length > 0)
            {
                string[] CriteriaFields = SearchCteriaCombined.Split('#');

                bool IsArchived = bool.Parse(CriteriaFields[0].ToString());
                bool IsDeleted = bool.Parse(CriteriaFields[1].ToString());
                DateTime FromDate = DateTime.Parse(CriteriaFields[2].ToString());
                DateTime ToDate = DateTime.Parse(CriteriaFields[3].ToString());

                FromDate = FromDate.AddHours(23);
                ToDate = ToDate.AddHours(23);

                TransferStatusKey = CriteriaFields[4].ToString();
                Int32 TransferStatusValue = 0;
                if (TransferStatusKey == "Unsubmitted")
                {
                    TransferStatusValue = (int)TransferStatus.UnSubmitted;
                    TextChartTitle = ResDashboard.TitleUnsubmittedTransfers;// "Unsubmitted Transfers";
                    TextSeriesName = "Transfer=> X-Axis:Transfer # Y-Axis:# of items";
                }
                else if (TransferStatusKey == "Submitted")
                {
                    TransferStatusValue = (int)TransferStatus.Submitted;
                    TextChartTitle = ResDashboard.TitleUnapprovedTransfers;// "Unapproved Transfers";
                    TextSeriesName = "Transfer=> X-Axis:Transfer # Y-Axis:# of items";
                }
                else if (TransferStatusKey == "Approved")
                {
                    TransferStatusValue = (int)TransferStatus.TransmittedPastDue;
                    TextChartTitle = ResDashboard.TitleTobeReceivedTransfers;// "To be Received Transfers";
                    TextSeriesName = "Transfer=> X-Axis:Transfer # Y-Axis:# of items";
                }

                //lstTransfer = controller.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted).ToList();
                //lstTransfer = lstTransfer.Where(t => t.Created.Date >= FromDate.Date && t.Created.Date <= ToDate.Date && t.TransferStatus == TransferStatusValue && t.NoOfItems > 0).OrderByDescending(t => t.NoOfItems).ToList();
                foreach (TransferMasterDTO item in lstTransfer)
                {
                    TimeSpan? difference = item.RequireDate - DateTimeUtility.DateTimeNow.Date;
                    double TotalDays = difference.Value.TotalDays;
                    if (!dicTransfer.ContainsKey(item.TransferNumber))
                        dicTransfer.Add(item.TransferNumber, item.NoOfItems ?? 0);
                }
            }

            ViewBag.TransferTextChartTitle = TextChartTitle;
            ViewBag.TransferdicTransfer = dicTransfer.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            return View("_TransferChart");
        }
        [NonAction]
        public Title CreateTitle(string TitleText)
        {
            Title title = new Title();
            title.Text = TitleText;
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Calibri", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }
        [NonAction]
        public Legend CreateLegend(string TitleText)
        {
            Legend legend = new Legend();
            legend.Name = TitleText;
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Calibri"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }
        [NonAction]
        public Series CreateSeries(Dictionary<string, double> lstOrders, SeriesChartType chartType, string OrderStatusKey, string SeriesName, string ChartAreaText)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = SeriesName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            if (OrderStatusKey == "Approved")
            {
                var resultsA = lstOrders.Take(10).ToList();
                foreach (KeyValuePair<string, double> result in resultsA)
                {
                    point = new DataPoint();
                    point.AxisLabel = result.Key;
                    point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                    seriesDetail.Points.Add(point);
                }
            }
            else
            {
                var resultsA = lstOrders.Take(10).ToList();
                foreach (KeyValuePair<string, double> result in resultsA)
                {
                    point = new DataPoint();
                    point.AxisLabel = result.Key;
                    point.YValues = new double[] { double.Parse(result.Value.ToString()) };
                    seriesDetail.Points.Add(point);
                }
            }
            seriesDetail.ChartArea = ChartAreaText;

            return seriesDetail;
        }
        [NonAction]
        public ChartArea CreateChartArea(string TitleText)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = TitleText;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;

            return chartArea;
        }
        #endregion
    }
    public class InventoryCountChartController : eTurnsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadChart()
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            IList<CommonDTO> lstCommonDTO = null;
            Dictionary<string, double> dicTransfer = new Dictionary<string, double>();
            string SearchCteriaCombined = string.Empty;
            if (Session["TransferSearchCriteria"] != null)
                SearchCteriaCombined = Session["TransferSearchCriteria"].ToString();
            string TransferStatusKey = string.Empty;

            string TextSeriesName = string.Empty;
            string TextChartTitle = string.Empty;
            TextChartTitle = ResDashboard.TitleInventoryCount;// "Inventory Count";
            //lstCommonDTO = objInventoryCountDAL.GetInventoryListCountByRoomAndCompanyNormal(SessionHelper.CompanyID, SessionHelper.RoomID).ToList();
            ViewBag.ChartTitle = TextChartTitle;
            //ViewBag.lstCommonDTO = lstCommonDTO.OrderByDescending(x => x.Count).Take(10).ToList();
            return View("_InventoryCountChart");
        }

        #endregion
    }
    public class CartChartController : eTurnsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
        #region Chart Component
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        //public ActionResult LoadTransferChart()
        //{
        //    //string SearchCteriaCombined = string.Empty;
        //    //if (Session["TransferSearchCriteria"] != null)
        //    //    SearchCteriaCombined = Session["TransferSearchCriteria"].ToString();
        //    //string TransferStatusKey = string.Empty;

        //    //string TextSeriesName = string.Empty;
        //    //string TextChartTitle = string.Empty;
        //    //CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
        //    //List<CartChartDTO> lstCartChartDTO = new List<CartChartDTO>();
        //    //lstCartChartDTO = objCartItemDAL.GetCartItemTransferList(SessionHelper.CompanyID, SessionHelper.RoomID);
        //    //TextChartTitle = ResDashboard.TitleCartTransfer;// "Cart Transfer";
        //    //ViewBag.CartTextChartTitle = TextChartTitle;
        //    //ViewBag.lstCartChartDTO = lstCartChartDTO.OrderByDescending(x => x.Quantity).Take(10).ToList();
        //    ViewBag.ChartType = "transfer";
        //    return View("_CartChart");
        //}

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        //public ActionResult LoadOrderChart()
        //{


        //    //string SearchCteriaCombined = string.Empty;
        //    //if (Session["TransferSearchCriteria"] != null)
        //    //    SearchCteriaCombined = Session["TransferSearchCriteria"].ToString();
        //    //string TransferStatusKey = string.Empty;

        //    //string TextSeriesName = string.Empty;
        //    //string TextChartTitle = string.Empty;
        //    //CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
        //    //List<CartChartDTO> lstCartChartDTO = new List<CartChartDTO>();
        //    //lstCartChartDTO = objCartItemDAL.GetCartItemOrderList(SessionHelper.CompanyID, SessionHelper.RoomID);
        //    //TextChartTitle = ResDashboard.TitlecartOrder;// "cart Order";
        //    //ViewBag.CartTextChartTitle = TextChartTitle;
        //    //ViewBag.lstCartChartDTO = lstCartChartDTO.OrderByDescending(x => x.Quantity).Take(10).ToList();
        //    ViewBag.ChartType = "purchase";
        //    return View("_CartChart");
        //}

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult LoadCartChart(string ChartType, string SelectedSupplier, string SelectedCategory)
        {
            if (string.IsNullOrEmpty(SelectedSupplier))
                SelectedSupplier = "";

            if (string.IsNullOrEmpty(SelectedCategory))
                SelectedCategory = "";

            if (string.IsNullOrWhiteSpace(ChartType))
            {
                ChartType = "purchase";
            }

            ViewBag.ChartType = ChartType.ToLower();
            ViewBag.SelectedSupplier = SelectedSupplier;
            ViewBag.SelectedCategory = SelectedCategory;
            return View("_CartChart");
        }

        #endregion
    }
}
