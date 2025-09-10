<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%
    string ChartType = ViewBag.ChartType;
    //int totalRecs = 0;

    eTurns.DAL.TransferMasterDAL objTransferMasterDAL = new eTurns.DAL.TransferMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.TransferMasterDTO> lstTransfers = new List<eTurns.DTO.TransferMasterDTO>();
    string TextChartTitle = eTurns.DTO.ResDashboard.TitleUnsubmittedTransfers;
    string TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleUnsubmittedTransfers;
    string XAxisTitle = eTurns.DTO.ResDashboard.TransferChartXTitle;
    string YaxisTitle = eTurns.DTO.ResDashboard.TransferChartYTitle;
    string QuantityFormat = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits))
    {
        QuantityFormat += eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits;
	} 

    string SortColumnsName = "NoOfItems ASC";
    if (string.IsNullOrWhiteSpace(ChartType))
    {
        ChartType = "unsubmitted";
    }
    ChartType = ChartType.ToLower();
    string TransferStatusText = "";
    switch (ChartType.ToLower())
    {
        case "unsubmitted":
            TransferStatusText = "Unsubmitted";
            SortColumnsName = "NoOfItems DESC";
            lstTransfers = objTransferMasterDAL.GetTransfersForDashboardChart(0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false, TransferStatusText).ToList();
            TextChartTitle = eTurns.DTO.ResDashboard.TitleUnsubmittedTransfers; ;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleUnsubmittedTransfers;
            break;
        case "tobeapproved":
            TransferStatusText = "ToBeApproved";
            SortColumnsName = "NoOfItems DESC";
            lstTransfers = objTransferMasterDAL.GetTransfersForDashboardChart(0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false, TransferStatusText).ToList();
            TextChartTitle = eTurns.DTO.ResDashboard.TitleUnapprovedTransfers;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleUnapprovedTransfers;
            break;
        case "receive":
            TransferStatusText = "Receive";
            SortColumnsName = "NoOfItems DESC";
            lstTransfers = objTransferMasterDAL.GetTransfersForDashboardChart(0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph,SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false, TransferStatusText).ToList();
            TextChartTitle = eTurns.DTO.ResDashboard.TitleTobeReceivedTransfers;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleTobeReceivedTransfers;
            break;
    }

    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    
    foreach (var key in lstTransfers)
    {
        string TTip = string.Empty;
        double YAxixVal = 0;

        switch (ChartType.ToLower())
        {
            case "unsubmitted":
                YAxixVal = key.NoOfItems ?? 0;
                TTip = key.NoOfItems.GetValueOrDefault(0).ToString(QuantityFormat);
                break;
            case "tobeapproved":
                YAxixVal = key.NoOfItems ?? 0;
                TTip = key.NoOfItems.GetValueOrDefault(0).ToString(QuantityFormat);
                break;
            case "receive":
                YAxixVal = key.NoOfItems ?? 0;
                TTip = key.NoOfItems.GetValueOrDefault(0).ToString(QuantityFormat);
                break;

        }
        string AxisLabelText = string.Empty;
        if (key.TransferNumber.Length > SetItemnumberLength && SetItemnumberLength > 0)
        {
            AxisLabelText = Convert.ToString(key.TransferNumber).Substring(0, SetItemnumberLength) + "....";
        }
        else
        {
            AxisLabelText = Convert.ToString(key.TransferNumber);
        }
        chartTransfer.Series["srsTransfer"].Points.Add(new DataPoint
        {
            AxisLabel = AxisLabelText,
            YValues = new double[] { (double)(YAxixVal) },
            Color = System.Drawing.Color.FromArgb(201, 77, 32),            
            Url = Url.Action("TransferList", "Transfer") + "?fromdashboard=yes&TransferID=" + HttpUtility.UrlEncode(key.ID.ToString())+ "&RequestType=" + HttpUtility.UrlEncode(key.RequestType.ToString()) + "&Status=" + HttpUtility.UrlEncode(key.TransferStatus.ToString()),
            ToolTip = TTip
        });
    }

    chartTransfer.Titles["ttlTransfer"].Text = TextChartTitle;
    chartTransfer.Titles["subttlTransfer"].Text = TextChartSubTitle;
    chartTransfer.ChartAreas["chartAreaTransfer"].AxisX.Title = XAxisTitle;
    chartTransfer.ChartAreas["chartAreaTransfer"].AxisY.Title = YaxisTitle;

    chartTransfer.ChartAreas["chartAreaTransfer"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartTransfer);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartTransfer.Titles["ttlTransfer"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartTransfer.Titles["subttlTransfer"]);
    chartTransfer.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartTransfer.ChartAreas["chartAreaTransfer"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartTransfer.Series["srsTransfer"]);
    
%>
<asp:Chart ID="chartTransfer" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlTransfer">
        </asp:Title>
        <asp:Title Name="subttlTransfer">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Raised" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaTransfer" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaTransfer" XValueType="String" Name="srsTransfer"
            ChartType="Column" Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" BorderColor="64, 64, 64, 64" Color="180, 65, 140, 240" YValueType="Double"
            ToolTip="#VALY">
        </asp:Series>
    </Series>
</asp:Chart>
