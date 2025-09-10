<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="System.Web.UI.DataVisualization.Charting" %>
<%@ Import Namespace="eTurns.DAL" %>
<%@ Import Namespace="eTurns.DTO" %>
<%@ Import Namespace="eTurnsWeb.Helper" %>
<% 
    
    List<ItemMasterDTO> lstItemMaster = new List<ItemMasterDTO>();
    DashboardDAL obdashboard = new DashboardDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    byte ItemOrCategory = Convert.ToByte(ViewBag.PieChartmetricOn);
    lstItemMaster = obdashboard.GetCategoryWiseCost(SessionHelper.CompanyID, SessionHelper.RoomID, ItemOrCategory, 10);
    Int32 CurrencyDecimalDigits = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits);
    string priceFormte = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits))
    {
        priceFormte += eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits;
    }

    if (lstItemMaster != null && lstItemMaster.Count > 0)
    {
        ItemMasterDTO objOthers = lstItemMaster.Where(t => t.CategoryName == "[!restofothers!]").FirstOrDefault();
        if (objOthers != null)
        {
            lstItemMaster = lstItemMaster.Where(t => (t.Cost ?? 0) > 0 && t.CategoryName != "[!restofothers!]").OrderByDescending(t => t.Cost).ToList();
            objOthers.CategoryName = "Others";
            if (objOthers.Cost > 0)
            {
                lstItemMaster.Add(objOthers);
            }
        }
        else
        {
            lstItemMaster = lstItemMaster.Where(t => (t.Cost ?? 0) > 0).OrderByDescending(t => t.Cost).ToList();
        }
        //""
        foreach (var itm in lstItemMaster)
        {
            chartItemCategoryChart.Series["srsItemCategory"].Points.Add(new DataPoint
            {
                AxisLabel = itm.CategoryName,
                YValues = new double[] { (double)Math.Round((itm.Cost ?? 0),CurrencyDecimalDigits) }
            });
        }
    }
    if (Convert.ToString(ViewBag.PieChartmetricOn) == "1")
    {
        chartItemCategoryChart.Titles["ttlChart"].Text = @ResInventoryAnalysis.Itemmetric;
    }
    else if (Convert.ToString(ViewBag.PieChartmetricOn) == "2")
    {
        chartItemCategoryChart.Titles["ttlChart"].Text = @ResInventoryAnalysis.Categorymetric;
    }
    else //if (Convert.ToString(ViewBag.PieChartmetricOn) == "3") for Manufacturer
    {
        chartItemCategoryChart.Titles["ttlChart"].Text = @ResInventoryAnalysis.Manufacturermetric;
    }
    chartItemCategoryChart.Series[0].LegendText = "#VALX-" + eTurnsWeb.Helper.SessionHelper.CurrencySymbol +"#VALY{"+ priceFormte +"}" + " (" + "#PERCENT" +")" ;
    chartItemCategoryChart.Series[0].ToolTip = eTurnsWeb.Helper.SessionHelper.CurrencySymbol +"#VALY{" + priceFormte + "}" + " (" + "#PERCENT" +")" ;
    
%>
<asp:Chart ID="chartItemCategoryChart" runat="server" Height="300" Width="450" BackColor="211, 223, 240"
    BorderlineDashStyle="Solid" BackSecondaryColor="White" BackGradientStyle="TopBottom"
    BorderlineWidth="1" Palette="BrightPastel" BorderlineColor="26, 59, 105" AntiAliasing="All"
    TextAntiAliasingQuality="Normal">
    <Titles>
        <asp:Title Name="ttlChart" Text="Category metric" ShadowColor="32, 0, 0, 0" Font="Trebuchet MS, 14pt, style=Bold"
            ShadowOffset="3" ForeColor="26, 59, 105">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Emboss" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaItemCategory" BackColor="Transparent">
            <AxisX IsLabelAutoFit="false" LineColor="64, 64, 64, 64">
                <MajorGrid LineColor="64, 64, 64, 64" />
                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
            </AxisX>
            <AxisY IsLabelAutoFit="false">
                <MajorGrid LineColor="64, 64, 64, 64" />
                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaItemCategory" XValueType="String" Name="srsItemCategory"
            ChartType="Pie" Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" BorderColor="64, 64, 64, 64" Color="180, 65, 140, 240" YValueType="Double"
            LegendText="#VALX-#VALY(#PERCENT)" ToolTip="#VALY(#PERCENT)" IsValueShownAsLabel="false">
        </asp:Series>
    </Series>
    <Legends>
        <asp:Legend Name="lgndItemCategory" BackColor="Transparent" LegendStyle="Table" Docking="Right">
            <Position Auto="true" />
        </asp:Legend>
    </Legends>
</asp:Chart>
