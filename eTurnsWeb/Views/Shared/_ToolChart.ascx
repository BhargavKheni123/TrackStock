<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%  
    eTurns.DAL.ToolMasterDAL objToolMasterDAL = new eTurns.DAL.ToolMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.ToolMasterDTO> lstTool = new List<eTurns.DTO.ToolMasterDTO>();
    string TextChartTitle = string.Empty;
    string TextChartSubTitle = string.Empty;
    string XAxisTitle = eTurns.DTO.ResDashboard.ToolChartXTitle;
    string YaxisTitle = eTurns.DTO.ResDashboard.ToolChartYTitle;
    lstTool = objToolMasterDAL.GetPagedToolMaintainanceDue(eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph);
    TextChartTitle = eTurns.DTO.ResDashboard.TitleToolDue;
    TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleToolDue;
    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    

    foreach (var key in lstTool)
    {
        string TTip = string.Empty;
        double YAxixVal = 0;
        YAxixVal = key.DaysDiff;
        TTip = key.DaysDiff.ToString();//(key.ToolName ?? string.Empty).ToString();
        string AxisLabelText = string.Empty;
        if (key.ToolName.Length > SetItemnumberLength && SetItemnumberLength > 0)
        {
            AxisLabelText = Convert.ToString(key.ToolName).Substring(0, SetItemnumberLength) + "....";
        }
        else
        {
            AxisLabelText = Convert.ToString(key.ToolName);
        }
        chartTool.Series["srsTool"].Points.Add(new DataPoint
        {
            AxisLabel = AxisLabelText,
            YValues = new double[] { (double)(YAxixVal) },
            Color = System.Drawing.Color.FromArgb(201, 77, 32),
            Url = (eTurnsWeb.Helper.SessionHelper.AllowToolOrdering ? Url.Action("ToolList", "Tool") :Url.Action("ToolList", "Assets"))  + "?fromdashboard=yes&ToolGUID=" + HttpUtility.UrlEncode(key.ID.ToString()),
            ToolTip = TTip
        });
    }

    chartTool.Titles["ttlTool"].Text = TextChartTitle;
    chartTool.Titles["subttlTool"].Text = TextChartSubTitle;
    chartTool.ChartAreas["chartAreaTool"].AxisX.Title = XAxisTitle;
    chartTool.ChartAreas["chartAreaTool"].AxisY.Title = YaxisTitle;

    chartTool.ChartAreas["chartAreaTool"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartTool);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartTool.Titles["ttlTool"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartTool.Titles["subttlTool"]);
    chartTool.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartTool.ChartAreas["chartAreaTool"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartTool.Series["srsTool"]);
    
%>


<asp:Chart ID="chartTool" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlTool">
        </asp:Title>
        <asp:Title Name="subttlTool">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Raised" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaTool" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaTool" XValueType="String" Name="srsTool" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" YValueType="Double" ToolTip="#VALY">
        </asp:Series>
    </Series>
</asp:Chart>
