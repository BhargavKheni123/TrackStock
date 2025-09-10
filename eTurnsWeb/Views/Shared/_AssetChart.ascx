<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%  
    eTurns.DAL.AssetMasterDAL objAssetMasterDAL = new eTurns.DAL.AssetMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.ToolsMaintenanceDTO> lstAsset = new List<eTurns.DTO.ToolsMaintenanceDTO>();
    string TextChartTitle = string.Empty;
    string TextChartSubTitle = string.Empty;
    string XAxisTitle = eTurns.DTO.ResDashboard.AssetChartXTitle;
    string YaxisTitle = eTurns.DTO.ResDashboard.AssetChartYTitle;
    lstAsset = objAssetMasterDAL.GetAssetMaintenceForDashboardChart(eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph);
    TextChartTitle = eTurns.DTO.ResDashboard.TitleAssetDue;
    TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleAssetDue;
    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    
    foreach (var key in lstAsset)
    {
        string TTip = string.Empty;
        double YAxixVal = 0;
        YAxixVal = key.DaysDiff;
        TTip = key.DaysDiff.ToString(); //(key.AssetName ?? string.Empty).ToString();
        string AxisLabelText = string.Empty;
        if (key.AssetName.Length > SetItemnumberLength && SetItemnumberLength > 0)
        {
            AxisLabelText = Convert.ToString(key.AssetName).Substring(0, SetItemnumberLength) + "....";
        }
        else
        {
            AxisLabelText = Convert.ToString(key.AssetName);
        }
        chartAsset.Series["srsAsset"].Points.Add(new DataPoint
        {
            AxisLabel = AxisLabelText,
            YValues = new double[] { (double)(YAxixVal) },
            Color = System.Drawing.Color.FromArgb(201, 77, 32),
            Url = Url.Action("AssetList", "Assets") + "?fromdashboard=yes&AssetGUID=" + HttpUtility.UrlEncode(key.AssetID.Value.ToString()),
            ToolTip = TTip
        });
    }

    chartAsset.Titles["ttlAsset"].Text = TextChartTitle;
    chartAsset.Titles["subttlAsset"].Text = TextChartSubTitle;
    chartAsset.ChartAreas["chartAreaAsset"].AxisX.Title = XAxisTitle;
    chartAsset.ChartAreas["chartAreaAsset"].AxisY.Title = YaxisTitle;

    chartAsset.ChartAreas["chartAreaAsset"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartAsset);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartAsset.Titles["ttlAsset"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartAsset.Titles["subttlAsset"]);
    chartAsset.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartAsset.ChartAreas["chartAreaAsset"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartAsset.Series["srsAsset"]);
    
%>


<asp:Chart ID="chartAsset" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlAsset">
        </asp:Title>
        <asp:Title Name="subttlAsset">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Raised" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaAsset" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaAsset" XValueType="String" Name="srsAsset" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" YValueType="Double" ToolTip="#VALY">
        </asp:Series>
    </Series>
</asp:Chart>
