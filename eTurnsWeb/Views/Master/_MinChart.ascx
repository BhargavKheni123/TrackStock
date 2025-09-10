<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="System.Web.UI.DataVisualization.Charting" %>
<%@ Import Namespace="eTurns.DAL" %>
<%@ Import Namespace="eTurns.DTO" %>
<%@ Import Namespace="eTurnsWeb.Helper" %>
<%
    chartMinChart.Series["srsMin"].Points.Add(new DataPoint
    {
        AxisLabel = @ResInventoryAnalysis.MinAboveoptimization.Replace("[!MinimumOptimizationXPer!]", Convert.ToString(ViewBag.MinMaxOptValue1)).Replace("[!MinimumOptimizationYPer!]", Convert.ToString(ViewBag.MinMaxOptValue2)),
        YValues = new double[] { (double)(ViewBag.MINRedCounts) },
        Color = System.Drawing.Color.Red
    });
    chartMinChart.Series["srsMin"].Points.Add(new DataPoint
    {
        AxisLabel = @ResInventoryAnalysis.MinBetweenoptimization.Replace("[!MinimumOptimizationXPer!]", Convert.ToString(ViewBag.MinMaxOptValue1)).Replace("[!MinimumOptimizationYPer!]", Convert.ToString(ViewBag.MinMaxOptValue2)),
        YValues = new double[] { (double)(ViewBag.MINYelloCounts) },
        Color = System.Drawing.Color.Yellow
    });
    chartMinChart.Series["srsMin"].Points.Add(new DataPoint
    {
        AxisLabel = @ResInventoryAnalysis.MinBelowoptimization.Replace("[!MinimumOptimizationXPer!]", Convert.ToString(ViewBag.MinMaxOptValue1)).Replace("[!MinimumOptimizationYPer!]", Convert.ToString(ViewBag.MinMaxOptValue2)),
        YValues = new double[] { (double)(ViewBag.MINGreenCounts) },
        Color = System.Drawing.Color.Green
    });
    chartMinChart.Titles["ttlMin"].Text = @ResInventoryAnalysis.MinOptimizationparameters;
%>
<asp:Chart ID="chartMinChart" runat="server" Height="300" Width="360" BackColor="211, 223, 240"
    BorderlineDashStyle="Solid" BackSecondaryColor="White" BackGradientStyle="TopBottom"
    BorderlineWidth="1" Palette="BrightPastel" BorderlineColor="26, 59, 105" AntiAliasing="All"
    TextAntiAliasingQuality="Normal">
    <Titles>
        <asp:Title Name="ttlMin" Text="Min Optimization parameters" ShadowColor="32, 0, 0, 0"
            Font="Trebuchet MS, 14pt, style=Bold" ShadowOffset="3" ForeColor="26, 59, 105">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Emboss" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaMin" BackColor="Transparent">
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
        <asp:Series ChartArea="chartAreaMin" XValueType="String" Name="srsMin" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" BorderColor="64, 64, 64, 64" Color="180, 65, 140, 240" YValueType="Double"
            ToolTip="#VALY" IsValueShownAsLabel="false">
        </asp:Series>
    </Series>
</asp:Chart>
