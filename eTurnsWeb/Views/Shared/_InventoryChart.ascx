<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%  
    eTurns.DAL.ItemMasterDAL objItemMasterDAL = new eTurns.DAL.ItemMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.ItemMonthWiseStockOutDTO> lstItemMonthWiseStockOutDTO = objItemMasterDAL.GetMonthWiseStockOutList(eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID);
    string TextChartTitle = eTurns.DTO.ResDashboard.TitleInventoryValueChart;
    string TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleInventoryValueChart;
    string XAxisTitle = "Months";
    string YaxisTitle = eTurns.DTO.ResDashboard.ICYaxisTitle;
    string YSecaxisTitle = eTurns.DTO.ResDashboard.IYSecaxisTitle;

    string priceFormte = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits))
    {
        priceFormte += eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits;
    }

    Int32 TurnsAvgDecimalPoints = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.eTurnsRegionInfoProp.TurnsAvgDecimalPoints);

    foreach (eTurns.DTO.ItemMonthWiseStockOutDTO itm in lstItemMonthWiseStockOutDTO)
    {
        if ((itm.Month ?? 0) > 0)
        {


            chartMaxISO.Series["srsISO"].Points.Add(new DataPoint
            {
                AxisLabel = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(itm.Month ?? 0).ToString().Substring(0, 3) + "(" + (itm.Stockouts ?? 0) + ")",
                YValues = new double[] { (double)itm.InventoryValue },
                Color = System.Drawing.Color.FromArgb(201, 77, 32),
                ToolTip = eTurnsWeb.Helper.SessionHelper.CurrencySymbol + "  " + itm.InventoryValue.Value.ToString(priceFormte,eTurnsWeb.Helper.SessionHelper.RoomCulture)
            });

            chartMaxISO.Series["srsISOLine"].Points.Add(new DataPoint
            {
                AxisLabel = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(itm.Month ?? 0).ToString().Substring(0, 3) + "(" + (itm.Stockouts ?? 0) + ")",
                YValues = new double[] { (double)Math.Round(itm.Turns.Value,TurnsAvgDecimalPoints) },
                Color = System.Drawing.Color.FromArgb(70, 4, 251)
            });
        }
    }

    chartMaxISO.Titles["ttlISO"].Text = TextChartTitle;
    chartMaxISO.Titles["subttlISO"].Text = TextChartSubTitle;
    chartMaxISO.ChartAreas["chartAreaISO"].AxisX.Title = XAxisTitle;
    chartMaxISO.ChartAreas["chartAreaISO"].AxisY.Title = YaxisTitle;
    chartMaxISO.ChartAreas["chartAreaISO"].AxisY2.Title = YSecaxisTitle;

    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartMaxISO);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartMaxISO.Titles["ttlISO"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartMaxISO.Titles["subttlISO"]);
    chartMaxISO.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartMaxISO.ChartAreas["chartAreaISO"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartMaxISO.Series["srsISO"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartMaxISO.Series["srsISOLine"]);
    
%>
<asp:Chart ID="chartMaxISO" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlISO">
        </asp:Title>
        <asp:Title Name="subttlISO">
        </asp:Title>
    </Titles>
    <ChartAreas>
        <asp:ChartArea Name="chartAreaISO" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
            <AxisX2 Enabled="True">
            </AxisX2>
            <AxisY2 Enabled="True">
            </AxisY2>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaISO" XValueType="String" Name="srsISO" ChartType="Column"
            CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" YValueType="Double" ToolTip="#VALY">
        </asp:Series>
        <asp:Series ChartArea="chartAreaISO" XValueType="String" Name="srsISOLine" ChartType="Line"
            YValueType="Double" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" ToolTip="#VALY" YAxisType="Secondary">
        </asp:Series>
        <%--<asp:Series ChartArea="chartAreaISO" XValueType="String" Name="srsISO" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" BorderColor="64, 64, 64, 64" Color="180, 65, 140, 240" YValueType="Double"
            ToolTip="#VALY" IsValueShownAsLabel="false">
        </asp:Series>
        <asp:Series ChartArea="chartAreaISO" XValueType="String" Name="srsISOLine" ChartType="Line"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" BorderColor="64, 64, 64, 64" Color="180, 65, 140, 240" YValueType="Double"
            ToolTip="#VALY" IsValueShownAsLabel="false" YAxisType="Secondary">
        </asp:Series>--%>
    </Series>
</asp:Chart>
