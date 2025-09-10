<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%
    string ChartType = ViewBag.ChartType;
    eTurns.DAL.DashboardDAL objDashboardDAL = new eTurns.DAL.DashboardDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.ProjectSpendItemsDTO> lstReqs = new List<eTurns.DTO.ProjectSpendItemsDTO>();
    string TextChartTitle = string.Empty;
    string TextChartSubTitle = string.Empty;
    string XAxisTitle = eTurns.DTO.ResDashboard.ConsumeChartPSXTitle;
    string YaxisTitle = eTurns.DTO.ResDashboard.ConsumeChartPSYTitle;
    string QuantityFormat = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits))
    {
        QuantityFormat += eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits;
	}
    if (string.IsNullOrWhiteSpace(ChartType))
    {
        ChartType = "Project Amount Exceeds";
    }
    ChartType = ChartType.ToLower();
    switch (ChartType.ToLower())
    {
        case "project amount exceeds":
            lstReqs = objDashboardDAL.GetProjectsForDashboard(eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, 10, false, "ProjectAmount");
            TextChartTitle = eTurns.DTO.ResDashboard.TitleProjectAmountExeeded;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleProjectAmountExeeded;
            break;
        case "item quantity exceeds":
            lstReqs = objDashboardDAL.GetProjectsForDashboard(eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, 10, false, "ItemQuantity");
            TextChartTitle = eTurns.DTO.ResDashboard.TitlePSItemQtyExeeded;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitlePSItemQtyExeeded;
            break;
        case "item amount exceeds":
            lstReqs = objDashboardDAL.GetProjectsForDashboard(eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, 10, false, "ItemAmount");
            TextChartTitle = eTurns.DTO.ResDashboard.TitlePSItemAmountExeeded;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitlePSItemAmountExeeded;
            break;
    }
    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    
    foreach (var key in lstReqs)
    {
        string TTip = string.Empty;
        double YAxixVal = 0;

        switch (ChartType.ToLower())
        {
            case "project amount exceeds":
                YAxixVal = key.ProjectPercent;
                TTip = key.ProjectPercent.ToString(QuantityFormat);
                break;
            case "item quantity exceeds":
                YAxixVal = key.ProjectPercent;
                TTip = key.ProjectPercent.ToString(QuantityFormat);
                break;
            case "item amount exceeds":
                YAxixVal = key.ProjectPercent;
                TTip = key.ProjectPercent.ToString(QuantityFormat);
                break;

        }
        string AxisLabelText = string.Empty;
        if (key.ProjectSpendName.Length > SetItemnumberLength && SetItemnumberLength > 0)
        {
            AxisLabelText = Convert.ToString(key.ProjectSpendName).Substring(0, SetItemnumberLength) + "....";
        }
        else
        {
            AxisLabelText = Convert.ToString(key.ProjectSpendName);
        }
        chartProjectSpend.Series["srsProject"].Points.Add(new DataPoint
        {
            AxisLabel = AxisLabelText,
            YValues = new double[] { (double)(YAxixVal) },
            Color = System.Drawing.Color.FromArgb(201, 77, 32),
            Url = Url.Action("ProjectList", "ProjectSpend") + "?fromdashboard=yes&ProjectID=" + HttpUtility.UrlEncode(key.ID.ToString()),
            ToolTip = Convert.ToString(YAxixVal)
        });
    }

    chartProjectSpend.Titles["ttlProject"].Text = TextChartTitle;
    chartProjectSpend.Titles["SubttlProject"].Text = TextChartSubTitle;
    chartProjectSpend.ChartAreas["chartAreaProject"].AxisX.Title = XAxisTitle;
    chartProjectSpend.ChartAreas["chartAreaProject"].AxisY.Title = YaxisTitle;

    chartProjectSpend.ChartAreas["chartAreaProject"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartProjectSpend);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartProjectSpend.Titles["ttlProject"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartProjectSpend.Titles["SubttlProject"]);
    chartProjectSpend.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartProjectSpend.ChartAreas["chartAreaProject"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartProjectSpend.Series["srsProject"]);
    
%>
<asp:Chart ID="chartProjectSpend" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlProject">
        </asp:Title>
        <asp:Title Name="SubttlProject">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Raised" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaProject" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaProject" XValueType="String" Name="srsProject" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" BorderColor="64, 64, 64, 64" Color="180, 65, 140, 240" YValueType="Double"
            ToolTip="#VALY">
        </asp:Series>
    </Series>
</asp:Chart>
