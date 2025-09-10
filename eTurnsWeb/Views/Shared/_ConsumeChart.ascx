<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%    
    string ChartType = ViewBag.ChartType;
    System.Data.DataTable ReqStatuses = ViewBag.ReqStatuses;
    System.Data.DataTable ReqTypes = ViewBag.ReqTypes;
    int totalRecs = 0;
    eTurns.DAL.RequisitionMasterDAL objRequisitionMasterDAL = new eTurns.DAL.RequisitionMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.RequisitionMasterDTO> lstReqs = new List<eTurns.DTO.RequisitionMasterDTO>();
    string ReqStatus = "Unsubmitted";
    string ReqType = "Requisition";
    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize  == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    string TextChartTitle = string.Empty;
    string TextChartSubTitle = string.Empty;
    string XAxisTitle = eTurns.DTO.ResDashboard.ConsumeChartXTitle;
    string YaxisTitle = eTurns.DTO.ResDashboard.ConsumeChartYTitle;
    string QuantityFormat = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits))
    {
        QuantityFormat += eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits;
	}

    if (string.IsNullOrWhiteSpace(ChartType))
    {
        ChartType = "unsubmitted";
    }

    ChartType = ChartType.ToLower();
    switch (ChartType.ToLower())
    {
        case "unsubmitted":
            ReqStatus = "Unsubmitted";
            lstReqs = objRequisitionMasterDAL.GetRequisitionsForConsumeChart(0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out totalRecs, string.Empty, "NumberofItemsrequisitioned DESC", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, ReqStatus, ReqType, eTurnsWeb.Helper.SessionHelper.UserID, ReqStatuses, ReqTypes, true);
            TextChartTitle = eTurns.DTO.ResDashboard.TitleUnsubmittedRequisition;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleUnsubmittedRequisition;
            break;
        case "submitted":
            ReqStatus = "Submitted";
            lstReqs = objRequisitionMasterDAL.GetRequisitionsForConsumeChart(0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out totalRecs, string.Empty, "NumberofItemsrequisitioned DESC", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, ReqStatus, ReqType, eTurnsWeb.Helper.SessionHelper.UserID, ReqStatuses, ReqTypes, true);
            TextChartTitle = eTurns.DTO.ResDashboard.TitleUnapprovedRequisition;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleUnapprovedRequisition;
            break;
        case "approved":
            ReqStatus = "Approved";
            lstReqs = objRequisitionMasterDAL.GetRequisitionsForConsumeChart(0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out totalRecs, string.Empty, "InCompletePullCount DESC", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, ReqStatus, ReqType, eTurnsWeb.Helper.SessionHelper.UserID, ReqStatuses, ReqTypes, true);
            TextChartTitle = eTurns.DTO.ResDashboard.TitleTobePulledRequisition;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleTobePulledRequisition;
            break;
    }

    foreach (var key in lstReqs)
    {
        string TTip = string.Empty;
        double YAxixVal = 0;

        switch (ChartType.ToLower())
        {
            case "unsubmitted":
                YAxixVal = key.NumberofItemsrequisitioned ?? 0;
                TTip = (key.NumberofItemsrequisitioned ?? 0).ToString(QuantityFormat);;
                break;
            case "submitted":
                YAxixVal = key.NumberofItemsrequisitioned ?? 0;
                TTip = (key.NumberofItemsrequisitioned ?? 0).ToString(QuantityFormat);
                break;
            case "approved":
                YAxixVal = key.InCompletePullCount ?? 0;
                TTip = (key.InCompletePullCount ?? 0).ToString(QuantityFormat);
                break;

        }

        string AxisLabelText = string.Empty;

        if (key.RequisitionNumber.Length > SetItemnumberLength && SetItemnumberLength > 0)
        {
            AxisLabelText = Convert.ToString(key.RequisitionNumber).Substring(0, SetItemnumberLength) + "....";
        }
        else
        {
            AxisLabelText = Convert.ToString(key.RequisitionNumber);
        }

        chartConsume.Series["srsConsume"].Points.Add(new DataPoint
        {
            AxisLabel = AxisLabelText,
            YValues = new double[] { (double)(YAxixVal) },
            Color = System.Drawing.Color.FromArgb(201, 77, 32),
            Url = Url.Action("RequisitionList", "Consume") + "?fromdashboard=yes&RequisitionGUID=" + HttpUtility.UrlEncode(key.GUID.ToString()),
            ToolTip = TTip
        });
    }

    chartConsume.Titles["ttlConsume"].Text = TextChartTitle;
    chartConsume.Titles["subttlConsume"].Text = TextChartSubTitle;
    chartConsume.ChartAreas["chartAreaConsume"].AxisX.Title = XAxisTitle;
    chartConsume.ChartAreas["chartAreaConsume"].AxisY.Title = YaxisTitle;
    chartConsume.ChartAreas["chartAreaConsume"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartConsume);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartConsume.Titles["ttlConsume"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartConsume.Titles["subttlConsume"]);
    chartConsume.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartConsume.ChartAreas["chartAreaConsume"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartConsume.Series["srsConsume"]);

%>
<asp:Chart ID="chartConsume" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlConsume">
        </asp:Title>
        <asp:Title Name="subttlConsume">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Raised" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaConsume" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaConsume" XValueType="String" Name="srsConsume" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" YValueType="Double" ToolTip="#VALY">
        </asp:Series>
    </Series>
</asp:Chart>
