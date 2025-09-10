<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="eTurns.DTO" %>
<%
    chartMaxChart.Series["srsMax"].Points.Add(new DataPoint
    {
        AxisLabel = @ResInventoryAnalysis.MaxAboveoptimization.Replace("[!MaximumOptimizationXPer!]", Convert.ToString(ViewBag.MinMaxOptValue1)).Replace("[!MaximumOptimizationYPer!]", Convert.ToString(ViewBag.MinMaxOptValue2)),
        YValues = new double[] { (double)(ViewBag.MaxRedCounts) },
        Color = System.Drawing.Color.Red
    });
    chartMaxChart.Series["srsMax"].Points.Add(new DataPoint
    {
        AxisLabel = @ResInventoryAnalysis.MaxBetweenoptimization.Replace("[!MaximumOptimizationXPer!]", Convert.ToString(ViewBag.MinMaxOptValue1)).Replace("[!MaximumOptimizationYPer!]", Convert.ToString(ViewBag.MinMaxOptValue2)),
        YValues = new double[] { (double)(ViewBag.MaxYelloCounts) },
        Color = System.Drawing.Color.Yellow
    });
    chartMaxChart.Series["srsMax"].Points.Add(new DataPoint
    {
        AxisLabel = @ResInventoryAnalysis.MaxBelowoptimization.Replace("[!MaximumOptimizationXPer!]", Convert.ToString(ViewBag.MinMaxOptValue1)).Replace("[!MaximumOptimizationYPer!]", Convert.ToString(ViewBag.MinMaxOptValue2)),
        YValues = new double[] { (double)(ViewBag.MaxGreenCounts) },
        Color = System.Drawing.Color.Green
    });
    chartMaxChart.Titles["ttlMax"].Text = @ResInventoryAnalysis.MaxOptimizationparameters;
%>
<asp:Chart ID="chartMaxChart" runat="server" Height="300" Width="360" BackColor="211, 223, 240"
    BorderlineDashStyle="Solid" BackSecondaryColor="White" BackGradientStyle="TopBottom"
    BorderlineWidth="1" Palette="BrightPastel" BorderlineColor="26, 59, 105" AntiAliasing="All"
    TextAntiAliasingQuality="Normal">
    <Titles>
        <asp:Title Name="ttlMax" Text="" ShadowColor="32, 0, 0, 0"
            Font="Trebuchet MS, 14pt, style=Bold" ShadowOffset="3" ForeColor="26, 59, 105">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Emboss" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaMax" BackColor="Transparent">
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
        <asp:Series ChartArea="chartAreaMax" XValueType="String" Name="srsMax" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" BorderColor="64, 64, 64, 64" Color="180, 65, 140, 240" YValueType="Double"
            ToolTip="#VALY" IsValueShownAsLabel="false">
        </asp:Series>
    </Series>
</asp:Chart>
