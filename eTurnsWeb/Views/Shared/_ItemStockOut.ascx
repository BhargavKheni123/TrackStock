<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<form runat="server">    
<%  
    string TTip = string.Empty;
    int MaxItemsInGraph = ViewBag.MaxItemsInGraph;
    string SelectedSupplier = ViewBag.SelectedSupplier;
    string SelectedCategory = ViewBag.SelectedCategory;
    float YpointVal = 0;
    string Criteria = Convert.ToString(ViewBag.Criteria);
    int TotalCount = 0;
    string TextChartTitle = string.Empty;
    string TextChartSubTitle = string.Empty;
    eTurns.DAL.DashboardDAL objDashboardDAL = new eTurns.DAL.DashboardDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    IList<eTurns.DTO.DashboardItem> peoples = null;
    IList<eTurns.DTO.DashboardItem> AllPeoples = null;    
    string XAxisTitle = "Item number";
    string YaxisTitle = string.Empty;
    IEnumerable<eTurns.DTO.DashboardItem> SupplierList = null;
    IEnumerable<eTurns.DTO.DashboardItem> CategoryList = null;
    string QuantityFormat = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits))
    {
        QuantityFormat += eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits;
	} 

    switch (Criteria.ToLower())
    {
        case "stock out":
            AllPeoples = objDashboardDAL.GetPagedItemsForDashboardInventoryChart(out SupplierList, out CategoryList,0, MaxItemsInGraph, out TotalCount, string.Empty, "StockOutCount Desc", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, "stock out", false, SelectedSupplier, SelectedCategory).ToList();
            TextChartTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.TitleStockOutChart);
            TextChartSubTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.SubTitleStockOutChart);
            YaxisTitle = eTurns.DTO.ResDashboard.YaxisTitleStockoutcount;
            break;
        case "critical":
            AllPeoples = objDashboardDAL.GetPagedItemsForDashboardInventoryChart(out SupplierList, out CategoryList,0, MaxItemsInGraph, out TotalCount, string.Empty, "Turns Desc", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, "critical", false, SelectedSupplier, SelectedCategory).ToList();
            TextChartTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.TitleCriticalChart);
            TextChartSubTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.SubTitleCriticalChart);
            XAxisTitle = eTurns.DTO.ResDashboard.XAxisTitleItemsorKits;
            YaxisTitle = eTurns.DTO.ResDashboard.YaxisTitleturns;
            break;
        case "minimum":
            AllPeoples = objDashboardDAL.GetPagedItemsForDashboardInventoryChart(out SupplierList, out CategoryList,0, MaxItemsInGraph, out TotalCount, string.Empty, "Turns Desc", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, "minimum", false, SelectedSupplier, SelectedCategory).ToList();
            TextChartTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.TitleMinimumChart);
            TextChartSubTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.SubTitleMinimumChart);
            XAxisTitle = eTurns.DTO.ResDashboard.XAxisTitleItemsorKits;
            YaxisTitle = eTurns.DTO.ResDashboard.YaxisTitleturns;
            break;
        case "maximum":
            AllPeoples = objDashboardDAL.GetPagedItemsForDashboardInventoryChart(out SupplierList, out CategoryList,0, MaxItemsInGraph, out TotalCount, string.Empty, "Turns Desc", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, "maximum", false, SelectedSupplier, SelectedCategory).ToList();
            TextChartTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.TitleMaximumChart);
            TextChartSubTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.SubTitleMaximumChart);
            XAxisTitle = eTurns.DTO.ResDashboard.XAxisTitleItemsorKits;
            YaxisTitle = eTurns.DTO.ResDashboard.YaxisTitleturns;
            break;
        case "slow moving":
            AllPeoples = objDashboardDAL.GetPagedItemsForDashboardInventoryChart(out SupplierList, out CategoryList,0, MaxItemsInGraph, out TotalCount, string.Empty, "Turns Desc", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, "slow moving", false, SelectedSupplier, SelectedCategory).ToList();
            TextChartTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.TitleSlowMovingChart);
            TextChartSubTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.SubTitleSlowMovingChart);
            YaxisTitle = eTurns.DTO.ResDashboard.YaxisTitleturns;
            break;
        case "fast moving":
            AllPeoples = objDashboardDAL.GetPagedItemsForDashboardInventoryChart(out SupplierList, out CategoryList,0, MaxItemsInGraph, out TotalCount, string.Empty, "Turns Desc", eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds, true, "fast moving", false, SelectedSupplier, SelectedCategory).ToList();
            TextChartTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.TitleFastMovingChart);
            TextChartSubTitle = HttpUtility.HtmlDecode(eTurns.DTO.ResDashboard.SubTitleFastMovingChart);
            YaxisTitle = eTurns.DTO.ResDashboard.YaxisTitleturns;
            break;
    }

    string strSupplierOptions = "";
    string strCategoryOptions = "";
    if (AllPeoples != null && AllPeoples.Count > 0)
    {
        peoples = AllPeoples;        
    }
    else
        peoples = new List<eTurns.DTO.DashboardItem>();

    if (SupplierList != null && SupplierList.Any())
    {
        List<string> lstSupplier = SupplierList.Select(x => "<option value='" + x.SupplierID + "'>" + x.SupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
        strSupplierOptions = string.Join("", lstSupplier);
    }

    if (CategoryList != null && CategoryList.Any())
    {
        List<string> lstCategory = CategoryList.Select(x => "<option value='" + x.CategoryID + "'>" + x.CategoryName + "(" + x.TotalRecords + ")" + "</option>").ToList();
        strCategoryOptions = string.Join("", lstCategory);
    }

    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    
    int intNumAvgDecPoints = 2;
    string numberFormte = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints))
    {
        intNumAvgDecPoints = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints);
        numberFormte += eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints;
    }
    else
    {
        numberFormte = "N2";
    }
           
    foreach (var key in peoples)
    {
        switch (Criteria)
        {
            case "Stock Out":
                TTip = (key.StockOutCount ?? 0).ToString(QuantityFormat);
                YpointVal = (float)(key.StockOutCount ?? 0);
                break;
            case "Critical":
                TTip = "Critical:" + key.CriticalQuantity.ToString(QuantityFormat) + "\nAvailable:" + key.OnHandQuantity.GetValueOrDefault(0).ToString(QuantityFormat) + "\nTurns:" + key.Turns.GetValueOrDefault(0).ToString(QuantityFormat);
                YpointVal = (float)(key.Turns ?? 0);
                break;
            case "Minimum":
                TTip = "Minimum:" + key.MinimumQuantity.ToString(QuantityFormat) + "\nAvailable:" + key.OnHandQuantity.GetValueOrDefault(0).ToString(QuantityFormat) + "\nTurns:" + key.Turns.GetValueOrDefault(0).ToString(QuantityFormat);
                YpointVal = (float)(key.Turns ?? 0);
                break;
            case "Maximum":
                TTip = "Maximum:" + key.MaximumQuantity.ToString(QuantityFormat) + "\nAvailable:" + key.OnHandQuantity.GetValueOrDefault(0).ToString(QuantityFormat) + "\nTurns:" + key.Turns.GetValueOrDefault(0).ToString(QuantityFormat);
                YpointVal = (float)(key.Turns ?? 0);
                break;
            case "Slow Moving":
                float dbTurns = 0;
                if (key.Turns != null && key.Turns > 0)
                {
                    dbTurns = (float)Math.Round((float) key.Turns, intNumAvgDecPoints);
                }
                TTip = "Slow Moving Paramter:" + key.SlowMovingValue.ToString(QuantityFormat) + "\nTurn:" + dbTurns.ToString(QuantityFormat);
                YpointVal = (float)dbTurns;
                break;
            case "Fast Moving":
                float dbTurnsFM = 0;
                if (key.Turns != null && key.Turns > 0)
                {
                    dbTurns = (float)Math.Round((float) key.Turns, intNumAvgDecPoints);
                }
                TTip = "Fast Moving Paramter:" + key.FastMovingValue.ToString(QuantityFormat) + "\nTurn:" + dbTurnsFM.ToString(QuantityFormat);
                YpointVal = (float)(key.Turns ?? 0);
                break;
            default:
                TTip = (key.StockOutCount ?? 0).ToString();
                YpointVal = (float)(key.StockOutCount ?? 0);
                break;
        }
        string AxisLabelText = string.Empty;
        if (Criteria == "Critical" || Criteria == "Minimum" || Criteria == "Maximum" || Criteria == "Stock Out")
        {
            if (key.ItemWithBin.Length > SetItemnumberLength && SetItemnumberLength > 0)
            {
                AxisLabelText = Convert.ToString(key.ItemWithBin).Substring(0, SetItemnumberLength) + "....";
            }
            else
            {
                AxisLabelText = Convert.ToString(key.ItemWithBin);
            }
        }
        else
        {
            if (key.ItemNumber.Length > SetItemnumberLength && SetItemnumberLength > 0)
            {
                AxisLabelText = Convert.ToString(key.ItemNumber).Substring(0, SetItemnumberLength) + "....";
            }
            else
            {
                AxisLabelText = Convert.ToString(key.ItemNumber);
            }
        }
        chartMaxISO.Series["srsISO"].Points.Add(new DataPoint
        {
            AxisLabel = AxisLabelText,
            YValues = new double[] { YpointVal },
            Color = System.Drawing.Color.FromArgb(201, 77, 32),
            Url = Url.Action("ItemMasterList", "Inventory") + "?fromdashboard=yes&ItemGUID=" + HttpUtility.UrlEncode(key.GUID.ToString()),
            ToolTip = TTip
        });
    }

    chartMaxISO.Titles["ttlISO"].Text = TextChartTitle;
    chartMaxISO.Titles["subttlISO"].Text = TextChartSubTitle;
    chartMaxISO.ChartAreas["chartAreaISO"].AxisX.Title = XAxisTitle;
    chartMaxISO.ChartAreas["chartAreaISO"].AxisY.Title = YaxisTitle;
    chartMaxISO.ChartAreas["chartAreaISO"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartMaxISO);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartMaxISO.Titles["ttlISO"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartMaxISO.Titles["subttlISO"]);
    chartMaxISO.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartMaxISO.ChartAreas["chartAreaISO"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartMaxISO.Series["srsISO"]);

    //EXPORT DASHBOARD GRAPH START
    //chartMaxISO.SaveImage(Server.MapPath("~/Downloads/") + "InventoryChartImage_" + DateTime.Now.ToString("yyyyMMddHHmmsss") + ".png");
    //imgItemStockOutImage.ImageUrl = "~/Downloads/InventoryChartImage_20171025190541.png";
    //EXPORT DASHBOARD GRAPH END

    //imgItemStockOutImage.Style.Add("display", "none");
    // 
    //chartMaxISO.ChartAreas["chartAreaISO"].AxisX.LabelStyle.Angle = -60;

    //Chart propery SET START
    //chartMaxISO.SeteTurnsChartStyle
    //chartMaxISO.Height = eTurnsWeb.Helper.ChartHelper.eTurnsChartWidth;
    //chartMaxISO.Height = eTurnsWeb.Helper.ChartHelper.eTurnsChartHeight;
    //chartMaxISO.BackColor = eTurnsWeb.Helper.ChartHelper.eTurnsChartBackColor;
    //chartMaxISO.BorderlineDashStyle = eTurnsWeb.Helper.ChartHelper.eturnsChartBorderlineDashStyle;
    //chartMaxISO.BackSecondaryColor = eTurnsWeb.Helper.ChartHelper.eturnsChartBackSecondaryColor;

    //Chart propery SET END

%>
<script type="text/javascript">
    $(document).ready(function () {        
        //------------------------------------------------------------------------------------------------
        //
        var SelectedSupplier = '<%= SelectedSupplier %>';
        var SupplierOptions = "<%= strSupplierOptions %>";
        if (SelectedSupplier == null || SelectedSupplier == undefined || SelectedSupplier.trim() == '')
            SelectedSupplier = GlogalInventorySupplierValue;
        //if ($('#lnkstockout').hasClass('liahover')) {
        //    if (SelectedSupplier == null || SelectedSupplier == undefined || SelectedSupplier.trim() == '')
        //        SelectedSupplier = GlogalInventorySupplierValue;
        //}

        BindMultiSelect("ddlInventoryChartSupplier", SupplierOptions, SelectedSupplier, '')
        $("#ddlInventoryChartSupplier").multiselect().bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            GlogalInventorySupplierValue = GetMultiselectSelectedValue('ddlInventoryChartSupplier');
            //if ($('#lnkstockout').hasClass('liahover')) {
            //    GlogalInventorySupplierValue = GetMultiselectSelectedValue('ddlInventoryChartSupplier');
            //}
            ddlInventorySupplierCategoryChange(ui, event);
        });

        //------------------------------------------------------------------------------------------------
        //
        var SelectedCategory = '<%= SelectedCategory %>';
        var CategoryOptions = "<%= strCategoryOptions %>";
        if (SelectedCategory == null || SelectedCategory == undefined || SelectedCategory.trim() == '')
            SelectedCategory = GlogalInventoryCategoryValue;
        //if ($('#lnkstockout').hasClass('liahover')) {
        //    if (SelectedCategory == null || SelectedCategory == undefined || SelectedCategory.trim() == '')
        //        SelectedCategory = GlogalInventoryCategoryValue;
        //}
        
        BindMultiSelect("ddlInventoryChartCategory", CategoryOptions, SelectedCategory, '')
        $("#ddlInventoryChartCategory").multiselect().bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            GlogalInventoryCategoryValue = GetMultiselectSelectedValue('ddlInventoryChartCategory');
            //if ($('#lnkstockout').hasClass('liahover')) {
            //    GlogalInventoryCategoryValue = GetMultiselectSelectedValue('ddlInventoryChartCategory');
            //}
            ddlInventorySupplierCategoryChange(ui, event);
        });
    });

    function ddlInventorySupplierCategoryChange(chkDropdown, event) {

        var arrDdlCartChartSupplier = $("#ddlInventoryChartSupplier").multiselect("getChecked").map(function () { return this.value }).get();
        var SelectedSupplier = arrDdlCartChartSupplier.join(",");

        var arrDdlCartChartCategory = $("#ddlInventoryChartCategory").multiselect("getChecked").map(function () { return this.value }).get();
        var SelectedCategory = arrDdlCartChartCategory.join(",");

        if ($('#lnkstockout').hasClass('liahover')) {
            BindPartialInventoryStatusGrid('lnkstockout', 'Stock Out', SelectedSupplier, SelectedCategory)
        }
        else if ($('#lnkcritical').hasClass('liahover')) {
            BindPartialInventoryStatusGrid('lnkcritical', 'Critical', SelectedSupplier, SelectedCategory)
        }
        else if ($('#lnkimnimum').hasClass('liahover')) {
            BindPartialInventoryStatusGrid('lnkimnimum', 'Minimum', SelectedSupplier, SelectedCategory)
        }
        else if ($('#lnkMaximum').hasClass('liahover')) {
            BindPartialInventoryStatusGrid('lnkMaximum', 'Maximum', SelectedSupplier, SelectedCategory)
        }
        else if ($('#lnkslowmoving').hasClass('liahover')) {
            BindPartialInventoryStatusGrid('lnkslowmoving', 'Slow Moving', SelectedSupplier, SelectedCategory)
        }
        else if ($('#lnkfastmoving').hasClass('liahover')) {
            BindPartialInventoryStatusGrid('lnkfastmoving', 'Fast Moving', SelectedSupplier, SelectedCategory)
        }
    }
</script>
<div id="divCartOrderTab" class="Lnavd" style="float: left; width: 80%; padding-top: 10px;">
    <div style="float:left; width:48%">
        <span style="float:left">Supplier:&nbsp;</span><select id="ddlInventoryChartSupplier"></select>
    </div>
    <div style="float:left; width:48%">
        <span style="float:left">Category:&nbsp;</span><select id="ddlInventoryChartCategory"></select>
    </div>
    <%--EXPORT DASHBOARD GRAPH START
    <a id="ExportPDF" href="#" onclick="ExportDashboardGraph('divItemStockOutImage')">Export PDF</a>
        EXPORT DASHBOARD GRAPH END--%>
</div>
<div id="divInventoryGraph">
    <asp:Chart ID="chartMaxISO" runat="server">
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
            </asp:ChartArea>
        </ChartAreas>
        <Series>
            <asp:Series ChartArea="chartAreaISO" XValueType="String" Name="srsISO" ChartType="Column"
                CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
                MarkerStyle="Circle" YValueType="Double" ToolTip="#VALY">
            </asp:Series>
        </Series>
    </asp:Chart>
    <div id="divItemStockOutImage" style="display:none">
        <asp:Image runat="server" ID="imgItemStockOutImage" ClientIDMode="Static" />
    </div>
</div>
</form>