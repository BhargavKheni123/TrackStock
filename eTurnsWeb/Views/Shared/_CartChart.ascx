<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%
    string ChartType = ViewBag.ChartType;
    string SelectedSupplier = ViewBag.SelectedSupplier;
    string SelectedCategory = ViewBag.SelectedCategory;
    if (string.IsNullOrWhiteSpace(ChartType))
    {
        ChartType = "purchase";
    }
    eTurns.DAL.CartItemDAL objSuggestedOrderMasterDAL = new eTurns.DAL.CartItemDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.CartChartDTO> lstSuggestedOrderAll = new List<eTurns.DTO.CartChartDTO>();
    List<eTurns.DTO.CartChartDTO> lstSuggestedOrder = new List<eTurns.DTO.CartChartDTO>();
    string TextChartTitle = eTurns.DTO.ResDashboard.TitleSuggestedOrderDue;
    string TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleSuggestedOrderDue;
    string XAxisTitle = eTurns.DTO.ResDashboard.SuggestedOrderChartXTitle;
    string YaxisTitle = eTurns.DTO.ResDashboard.SuggestedOrderChartYTitle;
    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    bool UserConsignmentAllowed = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    IEnumerable<eTurns.DTO.CartChartDTO> SupplierList = null;
    IEnumerable<eTurns.DTO.CartChartDTO> CategoryList = null;
    int TotalCount = 0;
    string QuantityFormat = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits))
    {
        QuantityFormat += eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits;
	}

    switch (ChartType)
    {
        case "purchase":
            TextChartTitle = eTurns.DTO.ResDashboard.TitleSuggestedOrderDue;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleSuggestedOrderDue;
            XAxisTitle = eTurns.DTO.ResDashboard.SuggestedOrderChartXTitle;
            YaxisTitle = eTurns.DTO.ResDashboard.SuggestedOrderChartYTitle;
            lstSuggestedOrderAll = objSuggestedOrderMasterDAL.GetPurchaseCartForDashboardChart(out SupplierList,out CategoryList,0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out TotalCount, string.Empty, "Quantity DESC", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false, "", eTurnsWeb.Helper.SessionHelper.UserSupplierIds, UserConsignmentAllowed, "Purchase",SelectedSupplier,SelectedCategory);
            break;
        case "transfer":
            TextChartTitle = eTurns.DTO.ResDashboard.TitleSuggestedTransferDue;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleSuggestedTransferDue;
            XAxisTitle = eTurns.DTO.ResDashboard.SuggestedTransferChartXTitle;
            YaxisTitle = eTurns.DTO.ResDashboard.SuggestedTransferChartYTitle;
            lstSuggestedOrderAll = objSuggestedOrderMasterDAL.GetCartItemsForDashboard(out SupplierList,out CategoryList,0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out TotalCount, string.Empty, "Quantity ASC", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, "Transfer",eTurnsWeb.Helper.SessionHelper.UserSupplierIds,SelectedSupplier,SelectedCategory);
            break;
    }

    string strSupplierOptions = "";
    string strCategoryOptions = "";

    if (lstSuggestedOrderAll != null && lstSuggestedOrderAll.Count > 0)
    {
        lstSuggestedOrder = lstSuggestedOrderAll;
    }
    else
        lstSuggestedOrder = new List<eTurns.DTO.CartChartDTO>();

    if (SupplierList != null && SupplierList.Any())
    {
        List<string> lstSupplier = SupplierList.Select(x => "<option value='" + x.SupplierId + "'>" + x.SupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
        strSupplierOptions = string.Join("", lstSupplier);
    }

    if (CategoryList != null && CategoryList.Any())
    {
        List<string> lstCategory = CategoryList.Select(x => "<option value='" + x.CategoryId + "'>" + x.CategoryName + "(" + x.TotalRecords + ")" + "</option>").ToList();
        strCategoryOptions = string.Join("", lstCategory);
    }

    double i=1;
    foreach (var key in lstSuggestedOrder)
    {
        string TTip = string.Empty;
        double YAxixVal = 0;
        string AxisLabelText=string.Empty;
        if (key.ItemNumber.Length > SetItemnumberLength && SetItemnumberLength > 0)
        {
            AxisLabelText = Convert.ToString(key.ItemNumber).Substring(0, SetItemnumberLength) + "....";
        }
        else
        {
            AxisLabelText=   Convert.ToString(key.ItemNumber);
        }
        YAxixVal = key.Quantity ;
        TTip = (key.Quantity).ToString(QuantityFormat);
        chartSuggestedOrder.Series["srsSuggestedOrder"].Points.Add(new DataPoint
        {

            AxisLabel =string.Empty,
            YValues = new double[] { (double)(YAxixVal) },
            Color = System.Drawing.Color.FromArgb(201, 77, 32),
            Url = Url.Action("ItemMasterList", "Inventory") + "?fromdashboard=yes&ItemGUID=" + HttpUtility.UrlEncode(key.ItemGuid.ToString()),
            ToolTip = TTip,
        });
        chartSuggestedOrder.ChartAreas["chartAreaSuggestedOrder"].AxisX.CustomLabels.Add(i-0.5, i + 0.5, AxisLabelText, 0, LabelMarkStyle.None);
        i++;
        foreach (CustomLabel label in chartSuggestedOrder.ChartAreas["chartAreaSuggestedOrder"].AxisX.CustomLabels)
        {
            if (label.RowIndex == 0 && label.Text == AxisLabelText)
            {
                label.ToolTip = string.Format("{0}", TTip);
            }
        }
    }

    chartSuggestedOrder.Titles["ttlSuggestedOrder"].Text = TextChartTitle;
    chartSuggestedOrder.Titles["subttlSuggestedOrder"].Text = TextChartSubTitle;
    chartSuggestedOrder.ChartAreas["chartAreaSuggestedOrder"].AxisX.Title = XAxisTitle;
    chartSuggestedOrder.ChartAreas["chartAreaSuggestedOrder"].AxisY.Title = YaxisTitle;
    chartSuggestedOrder.ChartAreas["chartAreaSuggestedOrder"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartSuggestedOrder);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartSuggestedOrder.Titles["ttlSuggestedOrder"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartSuggestedOrder.Titles["subttlSuggestedOrder"]);
    chartSuggestedOrder.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartSuggestedOrder.ChartAreas["chartAreaSuggestedOrder"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartSuggestedOrder.Series["srsSuggestedOrder"]);

%>
<script type="text/javascript">
    $(document).ready(function () {        
        //------------------------------------------------------------------------------------------------
        //
        var SelectedSupplier = '<%= SelectedSupplier %>';
        var SupplierOptions = "<%= strSupplierOptions %>";
        if (SelectedSupplier == null || SelectedSupplier == undefined || SelectedSupplier.trim() == '')
            SelectedSupplier = GlobalCartSupplierValue;

        BindMultiSelect("ddlCartChartSupplier", SupplierOptions, SelectedSupplier, '')
        $("#ddlCartChartSupplier").multiselect().bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            GlobalCartSupplierValue = GetMultiselectSelectedValue('ddlCartChartSupplier');
			ddlCartSupplierCategoryChange(ui, event);
        });

        //------------------------------------------------------------------------------------------------
        //
        var SelectedCategory = '<%= SelectedCategory %>';
        var CategoryOptions = "<%= strCategoryOptions %>";
        if (SelectedCategory == null || SelectedCategory == undefined || SelectedCategory.trim() == '')
            SelectedCategory = GlobalCartCategoryValue;
        
        BindMultiSelect("ddlCartChartCategory", CategoryOptions, SelectedCategory, '')
        $("#ddlCartChartCategory").multiselect().bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            GlobalCartCategoryValue = GetMultiselectSelectedValue('ddlCartChartCategory');
            ddlCartSupplierCategoryChange(ui, event);
        });
    });

    function ddlCartSupplierCategoryChange(chkDropdown, event) {

        var arrDdlCartChartSupplier = $("#ddlCartChartSupplier").multiselect("getChecked").map(function () { return this.value }).get();
        var SelectedSupplier = arrDdlCartChartSupplier.join(",");

        var arrDdlCartChartCategory = $("#ddlCartChartCategory").multiselect("getChecked").map(function () { return this.value }).get();
        var SelectedCategory = arrDdlCartChartCategory.join(",");
        
        if ($('#lnkCartorder').hasClass('liahover')) {
            BindCartOrderStatusGrid(SelectedSupplier, SelectedCategory)
        }
        else if ($('#lnkCartTransfer').hasClass('liahover')) {
            BindCartTransferStatusGrid(SelectedSupplier, SelectedCategory)
        }
    }
</script>
<div id="divCartOrderTab" class="Lnavd" style="float: left; width: 80%; padding-top: 10px;">
	<div style="float:left; width:48%">
		<span style="float:left">Supplier:&nbsp;</span><select id="ddlCartChartSupplier"></select>
	</div>
	<div style="float:left; width:48%">
		<span style="float:left">Category:&nbsp;</span><select id="ddlCartChartCategory"></select>
	</div>
</div>
<asp:Chart ID="chartSuggestedOrder" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlSuggestedOrder">
        </asp:Title>
        <asp:Title Name="subttlSuggestedOrder">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Raised" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaSuggestedOrder" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaSuggestedOrder" XValueType="String" Name="srsSuggestedOrder" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" YValueType="Double" ToolTip="#VALY">
        </asp:Series>
    </Series>
</asp:Chart>
