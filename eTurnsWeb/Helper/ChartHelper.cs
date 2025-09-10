using eTurns.DAL;
using eTurns.DTO;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml.XPath;
namespace eTurnsWeb.Helper
{
    public class ChartValueHelper
    {
        public static DateTime CacheExpiryTime
        {
            get
            {
                return DateTimeUtility.DateTimeNow.AddSeconds(104400);
            }
        }
        public static XElement GetChartPNode(string Nodepath)
        {
            XDocument loResource = null;
            XElement loRoot = null;
            try
            {
                string SessinKey = "ChartParaXml";

                if (HttpContext.Current.Cache.Get(SessinKey) != null)
                    loResource = (XDocument)HttpContext.Current.Cache.Get(SessinKey);

                if (loResource == null)
                {
                    loResource = new XDocument();
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/eTurnsChartSettings.xml")))
                    {
                        loResource = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/eTurnsChartSettings.xml"));
                        CacheDependency cacheDep = new CacheDependency(HttpContext.Current.Server.MapPath("~/Content/eTurnsChartSettings.xml"));
                        HttpContext.Current.Cache.Add(SessinKey, loResource, cacheDep, CacheExpiryTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                }
                loRoot = loResource.XPathSelectElement(Nodepath);
                return loRoot;
            }
            catch (Exception)
            {
                //throw ex;
                return loRoot;
                // Add exception log code here;
            }

            finally
            {
                loResource = null;
                loRoot = null;
            }
        }
    }

    public class eTurnsChartProperties
    {
        public static T ParseEnum<T>(string value)
        {
            value = value.Trim();
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static Color eTurnsChartBackColor
        {
            get
            {
                System.Drawing.Color tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(26, 59, 105);
                int A, R, G, B;
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eTurnsChartBackColor");
                if (eTurnsChartBackColor != null)
                {
                    int.TryParse(eTurnsChartBackColor.Element("A").Value, out A);
                    int.TryParse(eTurnsChartBackColor.Element("R").Value, out R);
                    int.TryParse(eTurnsChartBackColor.Element("G").Value, out G);
                    int.TryParse(eTurnsChartBackColor.Element("B").Value, out B);
                    tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(A, R, G, B);
                }
                return tmpeTurnsChartBackColor;
            }
        }

        public static ChartDashStyle eturnsChartBorderlineDashStyle
        {
            get
            {
                ChartDashStyle tmChartDashStyle = ChartDashStyle.Solid;
                XElement eturnsChartBorderlineDashStyle = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eturnsChartBorderlineDashStyle");
                if (eturnsChartBorderlineDashStyle != null)
                {
                    return ParseEnum<ChartDashStyle>(Convert.ToString(eturnsChartBorderlineDashStyle.Elements("ChartDashStyle").First().Value).Trim());
                }
                return tmChartDashStyle;
            }
        }
        public static Color eturnsChartBackSecondaryColor
        {
            get
            {
                System.Drawing.Color tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                int A, R, G, B;
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eturnsChartBackSecondaryColor");
                if (eTurnsChartBackColor != null)
                {
                    int.TryParse(eTurnsChartBackColor.Element("A").Value, out A);
                    int.TryParse(eTurnsChartBackColor.Element("R").Value, out R);
                    int.TryParse(eTurnsChartBackColor.Element("G").Value, out G);
                    int.TryParse(eTurnsChartBackColor.Element("B").Value, out B);
                    tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(A, R, G, B);
                }
                return tmpeTurnsChartBackColor;
            }
        }
        public static GradientStyle eturnsChartBackGradientStyle
        {
            get
            {
                GradientStyle tmChartDashStyle = GradientStyle.TopBottom;
                XElement eturnsChartBorderlineDashStyle = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eturnsChartBackGradientStyle");
                if (eturnsChartBorderlineDashStyle != null)
                {
                    return ParseEnum<GradientStyle>(Convert.ToString(eturnsChartBorderlineDashStyle.Elements("GradientStyle").First().Value).Trim());
                }
                return tmChartDashStyle;
            }
        }
        public static ChartColorPalette eturnsChartColorPalette
        {
            get
            {
                ChartColorPalette tmChartDashStyle = ChartColorPalette.BrightPastel;
                XElement eturnsChartBorderlineDashStyle = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eturnsChartColorPalette");
                if (eturnsChartBorderlineDashStyle != null)
                {
                    return ParseEnum<ChartColorPalette>(Convert.ToString(eturnsChartBorderlineDashStyle.Elements("ChartColorPalette").First().Value).Trim());
                }
                return tmChartDashStyle;
            }
        }
        public static Color eTurnsChartBorderlineColor
        {
            get
            {
                System.Drawing.Color tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(26, 59, 105);
                int A, R, G, B;
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eTurnsChartBorderlineColor");
                if (eTurnsChartBackColor != null)
                {
                    int.TryParse(eTurnsChartBackColor.Element("A").Value, out A);
                    int.TryParse(eTurnsChartBackColor.Element("R").Value, out R);
                    int.TryParse(eTurnsChartBackColor.Element("G").Value, out G);
                    int.TryParse(eTurnsChartBackColor.Element("B").Value, out B);
                    tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(A, R, G, B);
                }
                return tmpeTurnsChartBackColor;
            }
        }
        public static AntiAliasingStyles eTurnsChartAntiAliasing
        {
            get
            {
                AntiAliasingStyles tmChartDashStyle = AntiAliasingStyles.All;
                XElement eturnsChartBorderlineDashStyle = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eTurnsChartAntiAliasing");
                if (eturnsChartBorderlineDashStyle != null)
                {
                    return ParseEnum<AntiAliasingStyles>(Convert.ToString(eturnsChartBorderlineDashStyle.Elements("AntiAliasingStyles").First().Value).Trim());
                }
                return tmChartDashStyle;
            }
        }
        public static TextAntiAliasingQuality eTurnsChartTextAntiAliasingQuality
        {
            get
            {
                TextAntiAliasingQuality tmChartDashStyle = TextAntiAliasingQuality.Normal;
                XElement eturnsChartBorderlineDashStyle = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eTurnsChartTextAntiAliasingQuality");
                if (eturnsChartBorderlineDashStyle != null)
                {
                    return ParseEnum<TextAntiAliasingQuality>(Convert.ToString(eturnsChartBorderlineDashStyle.Elements("TextAntiAliasingQuality").First().Value).Trim());
                }
                return tmChartDashStyle;
            }
        }
        public static Color eTurnsChartBorderColor
        {
            get
            {
                System.Drawing.Color tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(26, 59, 105);
                int A, R, G, B;
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eTurnsChartBorderColor");
                if (eTurnsChartBackColor != null)
                {
                    int.TryParse(eTurnsChartBackColor.Element("A").Value, out A);
                    int.TryParse(eTurnsChartBackColor.Element("R").Value, out R);
                    int.TryParse(eTurnsChartBackColor.Element("G").Value, out G);
                    int.TryParse(eTurnsChartBackColor.Element("B").Value, out B);
                    tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(A, R, G, B);
                }
                return tmpeTurnsChartBackColor;
            }
        }


        public static Color eTurnsChartTitleShadowColor
        {
            get
            {
                System.Drawing.Color tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(32, 0, 0, 0);
                int A, R, G, B;
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartTitleProperty/eTurnsChartTitleShadowColor");
                if (eTurnsChartBackColor != null)
                {
                    int.TryParse(eTurnsChartBackColor.Element("A").Value, out A);
                    int.TryParse(eTurnsChartBackColor.Element("R").Value, out R);
                    int.TryParse(eTurnsChartBackColor.Element("G").Value, out G);
                    int.TryParse(eTurnsChartBackColor.Element("B").Value, out B);
                    tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(A, R, G, B);
                }
                return tmpeTurnsChartBackColor;
            }
        }
        public static Font eTurnsChartTitleFont
        {
            get
            {

                Font tmpfnt = new Font("Trebuchet MS", 14.0f, FontStyle.Bold);
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartTitleProperty/eTurnsChartTitleFont");
                if (eTurnsChartBackColor != null)
                {
                    tmpfnt = new Font(eTurnsChartBackColor.Element("FontFamily").Value, float.Parse(eTurnsChartBackColor.Element("FontempSize").Value), ParseEnum<FontStyle>(eTurnsChartBackColor.Element("FontStyle").Value));
                }
                return tmpfnt;
            }
        }
        public static int eTurnsChartTitleShadowOffset
        {
            get
            {
                //
                int tmpint = 3;
                XElement xele = ChartValueHelper.GetChartPNode("eturnsChart/chartTitleProperty/eTurnsChartTitleShadowOffset");
                if (xele != null)
                {
                    tmpint = int.Parse(xele.Value);
                }
                return tmpint;
            }
        }
        public static Color eTurnsChartTitleForeColor
        {
            get
            {
                System.Drawing.Color tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(26, 59, 105);
                int A, R, G, B;
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartProperty/eTurnsChartTitleForeColor");
                if (eTurnsChartBackColor != null)
                {
                    int.TryParse(eTurnsChartBackColor.Element("A").Value, out A);
                    int.TryParse(eTurnsChartBackColor.Element("R").Value, out R);
                    int.TryParse(eTurnsChartBackColor.Element("G").Value, out G);
                    int.TryParse(eTurnsChartBackColor.Element("B").Value, out B);
                    tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(A, R, G, B);
                }
                return tmpeTurnsChartBackColor;
            }
        }



        public static Color eTurnsChartSubTitleShadowColor
        {
            get
            {
                System.Drawing.Color tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(32, 0, 0, 0);
                int A, R, G, B;
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartSubTitleProperty/eTurnschartSubTitleShadowColor");
                if (eTurnsChartBackColor != null)
                {
                    int.TryParse(eTurnsChartBackColor.Element("A").Value, out A);
                    int.TryParse(eTurnsChartBackColor.Element("R").Value, out R);
                    int.TryParse(eTurnsChartBackColor.Element("G").Value, out G);
                    int.TryParse(eTurnsChartBackColor.Element("B").Value, out B);
                    tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(A, R, G, B);
                }
                return tmpeTurnsChartBackColor;
            }
        }
        public static Font eTurnsChartSubTitleFont
        {
            get
            {

                Font tmpfnt = new Font("Trebuchet MS", 14.0f, FontStyle.Bold);
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartSubTitleProperty/eTurnschartSubTitleFont");
                if (eTurnsChartBackColor != null)
                {
                    tmpfnt = new Font(eTurnsChartBackColor.Element("FontFamily").Value, float.Parse(eTurnsChartBackColor.Element("FontempSize").Value), ParseEnum<FontStyle>(eTurnsChartBackColor.Element("FontStyle").Value));
                }
                return tmpfnt;
            }
        }
        public static int eTurnsChartSubTitleShadowOffset
        {
            get
            {
                //
                int tmpint = 3;
                XElement xele = ChartValueHelper.GetChartPNode("eturnsChart/chartSubTitleProperty/eTurnschartSubTitleShadowOffset");
                if (xele != null)
                {
                    tmpint = int.Parse(xele.Value);
                }
                return tmpint;
            }
        }
        public static Color eTurnsChartSubTitleForeColor
        {
            get
            {
                System.Drawing.Color tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(26, 59, 105);
                int A, R, G, B;
                XElement eTurnsChartBackColor = ChartValueHelper.GetChartPNode("eturnsChart/chartSubTitleProperty/eTurnschartSubTitleForeColor");
                if (eTurnsChartBackColor != null)
                {
                    int.TryParse(eTurnsChartBackColor.Element("A").Value, out A);
                    int.TryParse(eTurnsChartBackColor.Element("R").Value, out R);
                    int.TryParse(eTurnsChartBackColor.Element("G").Value, out G);
                    int.TryParse(eTurnsChartBackColor.Element("B").Value, out B);
                    tmpeTurnsChartBackColor = System.Drawing.Color.FromArgb(A, R, G, B);
                }
                return tmpeTurnsChartBackColor;
            }
        }


        public static Color eTurnsAxisMajorGridlinecolor = Color.FromArgb(211, 211, 211);
        public static Color eTurnsAxisLinecolor = Color.FromArgb(167, 186, 197);
        public static Color eTurnsAxislabelForeColor = Color.FromArgb(12, 12, 12);
        public static Color eTurnsSeriesBorderColor = Color.FromArgb(180, 26, 59, 105);
        public static Color eTurnsRoundMarkerBorderColor = Color.FromArgb(255, 0, 120, 255);
        public static Color eTurnsRoundMarkerColor = Color.WhiteSmoke;
        public static Unit eTurnsChartWidth = new Unit(570);
        public static Unit eTurnsChartHeight = new Unit(250);

        public static BorderSkinStyle eturnsChartBorderSkinStyle = BorderSkinStyle.Emboss;
        public static bool eturnsChartAreaAxisXIsLabelAutoFit = false;
        public static Color eturnsChartAreaAxisXLineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        public static Color eturnsChartAreaAxisXMajorGridLineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        public static Font eturnsChartAreaAxisXLabelStyleFont = new Font("Arial,Helvetica,sans-serif", 8.25f, FontStyle.Bold);
        public static Font eTurnsChartSeriesFont = new Font("Trebuchet MS", 8.25f, FontStyle.Bold);
        public static Color eTurnsChartSeriesBorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
        public static Color eTurnsChartSeriesColor = System.Drawing.Color.FromArgb(180, 65, 140, 240);
        public static bool eTurnsChartIsValueShownAsLabel = false;
    }


    public class ChartHelper
    {

        public static int MaxItemsInGraph = Convert.ToInt32(ConfigurationManager.AppSettings["MaxItemsInGraph"]);

        public static Title CreateTitle(string name)
        {
            Title title = new Title();
            title.Text = name;
            //title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            title.ShadowOffset = 0;
            title.ForeColor = Color.FromArgb(26, 59, 105);
            return title;

        }

        public static Legend CreateLegend()
        {
            Legend legend = new Legend();
            legend.Name = ResDashboard.OptimizationParameters; 
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Trebuchet MS"), 9);
            legend.LegendStyle = LegendStyle.Row;
            return legend;
        }

        public static Series CreateSeries(SeriesChartType chartType, string LabelName, double Xaxval, double Yaxval, Color seriescolor, string areaname)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = LabelName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = seriescolor;
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            seriesDetail.ChartArea = areaname;
            DataPoint point;
            point = new DataPoint();
            point.AxisLabel = LabelName;
            point.XValue = Xaxval;
            point.YValues = new double[] { Yaxval };
            seriesDetail.Points.Add(point);
            return seriesDetail;
        }

        public static Series CreatepieSeries(SeriesChartType chartType, string LabelName, double Xaxval, double Yaxval, Color seriescolor, string areaname)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = LabelName;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = seriescolor;
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            seriesDetail.ChartArea = areaname;
            return seriesDetail;
        }

        public static ChartArea CreateChartArea(string areaName)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = areaName;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.Interval = 1;
            return chartArea;
        }

        public static void SetChartDefaultStyle(Chart eTurnsChart)
        {
            eTurnsChart.Height = eTurnsChartProperties.eTurnsChartWidth;
            eTurnsChart.Height = eTurnsChartProperties.eTurnsChartHeight;
            eTurnsChart.BackColor = eTurnsChartProperties.eTurnsChartBackColor;
            eTurnsChart.BorderlineDashStyle = eTurnsChartProperties.eturnsChartBorderlineDashStyle;
            eTurnsChart.BackSecondaryColor = eTurnsChartProperties.eturnsChartBackSecondaryColor;
            //eTurnsChart.BackGradientStyle = eTurnsChartProperties.eturnsChartBackGradientStyle;
            eTurnsChart.Palette = eTurnsChartProperties.eturnsChartColorPalette;
            eTurnsChart.BorderlineColor = eTurnsChartProperties.eTurnsChartBorderlineColor;
        }

        public static DataTable GetReqStatus()
        {
            DataTable ReqStatus = new DataTable();
            DataColumn Dt = new DataColumn("StatusName", typeof(string));
            Dt.MaxLength = 255;
            ReqStatus.Columns.Add(Dt);
            Dt = new DataColumn("StatusValue", typeof(string));
            Dt.MaxLength = 255;
            ReqStatus.Columns.Add(Dt);

            ReqStatus.Rows.Add("Unsubmitted", ResRequisitionMaster.Unsubmitted);
            ReqStatus.Rows.Add("Submitted", ResRequisitionMaster.Submitted);
            ReqStatus.Rows.Add("Approved", ResRequisitionMaster.Approved);
            ReqStatus.Rows.Add("Closed", ResRequisitionMaster.Closed);
            return ReqStatus;
        }

        public static DataTable GetReqTypes()
        {
            DataTable ReqType = new DataTable();
            DataColumn Dt = new DataColumn("StatusName", typeof(string));
            Dt.MaxLength = 255;
            ReqType.Columns.Add(Dt);
            Dt = new DataColumn("StatusValue", typeof(string));
            Dt.MaxLength = 255;
            ReqType.Columns.Add(Dt);

            ReqType.Rows.Add("Asset Service", ResRequisitionMaster.AssetService);
            ReqType.Rows.Add("Requisition", ResRequisitionMaster.Requisition);
            ReqType.Rows.Add("Tool Service", ResRequisitionMaster.ToolService);
            return ReqType;
        }

        #region [Chart Styling]

        public static void SeteTurnsChartStyle(Chart eTurnsChart)
        {
            eTurnsChart.Width = eTurnsChartProperties.eTurnsChartWidth;
            eTurnsChart.Height = eTurnsChartProperties.eTurnsChartHeight;
            eTurnsChart.BackColor = eTurnsChartProperties.eTurnsChartBackColor;
            eTurnsChart.BorderlineDashStyle = eTurnsChartProperties.eturnsChartBorderlineDashStyle;
            eTurnsChart.BackSecondaryColor = eTurnsChartProperties.eturnsChartBackSecondaryColor;
            //eTurnsChart.BackGradientStyle = eTurnsChartProperties.eturnsChartBackGradientStyle;
            eTurnsChart.Palette = eTurnsChartProperties.eturnsChartColorPalette;
            eTurnsChart.BorderlineColor = eTurnsChartProperties.eTurnsChartBorderlineColor;
            eTurnsChart.AntiAliasing = eTurnsChartProperties.eTurnsChartAntiAliasing;
            eTurnsChart.TextAntiAliasingQuality = eTurnsChartProperties.eTurnsChartTextAntiAliasingQuality;
        }

        public static void SeteTurnsChartTitleStyle(Title ChartTitle)
        {
            ChartTitle.ShadowColor = eTurnsChartProperties.eTurnsChartTitleShadowColor;
            ChartTitle.Font = eTurnsChartProperties.eTurnsChartTitleFont;
            ChartTitle.ShadowOffset = 0;
            ChartTitle.ForeColor = eTurnsChartProperties.eTurnsChartTitleForeColor;
        }

        public static void SeteTurnsChartSubTitleStyle(Title ChartTitle)
        {
            ChartTitle.ShadowColor = eTurnsChartProperties.eTurnsChartSubTitleShadowColor;
            ChartTitle.Font = eTurnsChartProperties.eTurnsChartSubTitleFont;
            ChartTitle.ShadowOffset = 0;
            ChartTitle.ForeColor = eTurnsChartProperties.eTurnsChartSubTitleForeColor;
        }

        public static void SeteTurnsChartAreaStyle(ChartArea eturnsChartArea)
        {
            if (eturnsChartArea.AxisX != null)
            {
                eturnsChartArea.AxisX.IsLabelAutoFit = eTurnsChartProperties.eturnsChartAreaAxisXIsLabelAutoFit;
                eturnsChartArea.AxisX.LineColor = eTurnsChartProperties.eturnsChartAreaAxisXLineColor;
                eturnsChartArea.AxisX.MajorGrid.LineColor = eTurnsChartProperties.eturnsChartAreaAxisXMajorGridLineColor;
                eturnsChartArea.AxisX.LabelStyle.Font = eTurnsChartProperties.eturnsChartAreaAxisXLabelStyleFont;
            }
            if (eturnsChartArea.AxisY != null)
            {
                eturnsChartArea.AxisY.IsLabelAutoFit = eTurnsChartProperties.eturnsChartAreaAxisXIsLabelAutoFit;
                eturnsChartArea.AxisY.LineColor = eTurnsChartProperties.eturnsChartAreaAxisXLineColor;
                eturnsChartArea.AxisY.MajorGrid.LineColor = eTurnsChartProperties.eturnsChartAreaAxisXMajorGridLineColor;
                eturnsChartArea.AxisY.LabelStyle.Font = eTurnsChartProperties.eturnsChartAreaAxisXLabelStyleFont;
            }
            if (eturnsChartArea.AxisX2 != null)
            {
                eturnsChartArea.AxisX2.IsLabelAutoFit = eTurnsChartProperties.eturnsChartAreaAxisXIsLabelAutoFit;
                eturnsChartArea.AxisX2.LineColor = eTurnsChartProperties.eturnsChartAreaAxisXLineColor;
                eturnsChartArea.AxisX2.MajorGrid.LineColor = eTurnsChartProperties.eturnsChartAreaAxisXMajorGridLineColor;
                eturnsChartArea.AxisX2.LabelStyle.Font = eTurnsChartProperties.eturnsChartAreaAxisXLabelStyleFont;
            }
            if (eturnsChartArea.AxisX2 != null)
            {
                eturnsChartArea.AxisY2.IsLabelAutoFit = eTurnsChartProperties.eturnsChartAreaAxisXIsLabelAutoFit;
                eturnsChartArea.AxisY2.LineColor = eTurnsChartProperties.eturnsChartAreaAxisXLineColor;
                eturnsChartArea.AxisY2.MajorGrid.LineColor = eTurnsChartProperties.eturnsChartAreaAxisXMajorGridLineColor;
                eturnsChartArea.AxisY2.LabelStyle.Font = eTurnsChartProperties.eturnsChartAreaAxisXLabelStyleFont;
            }
        }

        public static void SeteTurnsChartSeries(Series ChartSeries)
        {
            ChartSeries.Font = eTurnsChartProperties.eTurnsChartSeriesFont;
            ChartSeries.BorderColor = eTurnsChartProperties.eTurnsChartSeriesBorderColor;
            ChartSeries.Color = eTurnsChartProperties.eTurnsChartSeriesColor;
            ChartSeries.IsValueShownAsLabel = eTurnsChartProperties.eTurnsChartIsValueShownAsLabel;
        }

        #endregion
    }

}