<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%
    eTurns.DAL.InventoryCountDAL objInventoryCountMasterDAL = new eTurns.DAL.InventoryCountDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.InventoryCountDTO> lstInventoryCount = new List<eTurns.DTO.InventoryCountDTO>();
    string TextChartTitle = string.Empty;
    string TextChartSubTitle = string.Empty;
    string XAxisTitle = eTurns.DTO.ResDashboard.InventoryCountChartXTitle;
    string YaxisTitle = eTurns.DTO.ResDashboard.InventoryCountChartYTitle;
    lstInventoryCount = objInventoryCountMasterDAL.GetInventoryCountForChart(eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, "ID ASC", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID);
    TextChartTitle = eTurns.DTO.ResDashboard.TitleInventoryCountDue;
    TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleInventoryCountDue;
    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    
    foreach (var key in lstInventoryCount)
    {
        string TTip = string.Empty;
        double YAxixVal = 0;
        YAxixVal = key.inventorycount;
        TTip = key.inventorycount.ToString();//(key.CountName ?? string.Empty).ToString();
        string AxisLabelText = string.Empty;
        if (key.CountName.Length > SetItemnumberLength && SetItemnumberLength > 0)
        {
            AxisLabelText = Convert.ToString(key.CountName).Substring(0, SetItemnumberLength) + "....";
        }
        else
        {
            AxisLabelText = Convert.ToString(key.CountName);
        }
        chartInventoryCount.Series["srsInventoryCount"].Points.Add(new DataPoint
        {
            AxisLabel = AxisLabelText,
            YValues = new double[] { (double)(YAxixVal) },
            Color = System.Drawing.Color.FromArgb(201, 77, 32),
            Url = Url.Action("InventoryCountList", "Inventory") + "?fromdashboard=yes&InventoryCountGUID=" + HttpUtility.UrlEncode(key.GUID.ToString()),
            ToolTip = TTip
        });
    }

    chartInventoryCount.Titles["ttlInventoryCount"].Text = TextChartTitle;
    chartInventoryCount.Titles["subttlInventoryCount"].Text = TextChartSubTitle;
    chartInventoryCount.ChartAreas["chartAreaInventoryCount"].AxisX.Title = XAxisTitle;
    chartInventoryCount.ChartAreas["chartAreaInventoryCount"].AxisY.Title = YaxisTitle;
    chartInventoryCount.ChartAreas["chartAreaInventoryCount"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartInventoryCount);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartInventoryCount.Titles["ttlInventoryCount"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartInventoryCount.Titles["subttlInventoryCount"]);
    chartInventoryCount.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartInventoryCount.ChartAreas["chartAreaInventoryCount"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartInventoryCount.Series["srsInventoryCount"]);
    
%>

<asp:Chart ID="chartInventoryCount" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlInventoryCount">
        </asp:Title>
        <asp:Title Name="subttlInventoryCount">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Raised" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaInventoryCount" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaInventoryCount" XValueType="String" Name="srsInventoryCount" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" YValueType="Double" ToolTip="#VALY">
        </asp:Series>
    </Series>
</asp:Chart>
