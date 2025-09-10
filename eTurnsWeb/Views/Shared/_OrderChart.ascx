<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%  
    string ChartType = ViewBag.ChartType;
    string SelectedSupplier = ViewBag.SelectedSupplier;
    int totalRecs = 0;
    eTurns.DAL.OrderMasterDAL objRequisitionMasterDAL = new eTurns.DAL.OrderMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
    List<eTurns.DTO.OrderMasterDTO> lstOrders = new List<eTurns.DTO.OrderMasterDTO>();
    List<eTurns.DTO.OrderMasterDTO> lstOrdersAll = new List<eTurns.DTO.OrderMasterDTO>();

    List<eTurns.DTO.ReceivableItemDTO> lstReceivable = new List<eTurns.DTO.ReceivableItemDTO>();
    List<eTurns.DTO.ReceivableItemDTO> lstReceivableAll = new List<eTurns.DTO.ReceivableItemDTO>();

    List<eTurns.DTO.DashboardBottomAndTopSpendDTO> lstBTSpend = new List<eTurns.DTO.DashboardBottomAndTopSpendDTO>();
    List<eTurns.DTO.DashboardBottomAndTopSpendDTO> lstBTSpendAll = new List<eTurns.DTO.DashboardBottomAndTopSpendDTO>();

    string TabtoPass = "[^]1";
    string TextChartTitle = eTurns.DTO.ResDashboard.TitleUnsubmittedOrder;
    string TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleUnsubmittedOrder;
    string XAxisTitle = eTurns.DTO.ResDashboard.OrderChartXTitle;
    string YaxisTitle = eTurns.DTO.ResDashboard.OrderChartYTitle;
    IEnumerable<eTurns.DTO.OrderMasterDTO> OrderSupplierList = null;
    IEnumerable<eTurns.DTO.ReceivableItemDTO> ReceiveSupplierList = null;
    string QuantityFormat = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits))
    {
        QuantityFormat += eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits;
	}
    string priceFormte = "N";
    if (!string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits))
    {
        priceFormte += eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits;
	} 

    string SortColumnsName = "NoOfLineItems ASC";
    if (string.IsNullOrWhiteSpace(ChartType))
    {
        ChartType = "unsubmitted";
    }
    ChartType = ChartType.ToLower();
    switch (ChartType.ToLower())
    {
        case "unsubmitted":
            TabtoPass = "[^]1";
            SortColumnsName = "NoOfLineItems DESC";
            lstOrdersAll = objRequisitionMasterDAL.GetOrderMasterPagedDataNormal(out OrderSupplierList,0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out totalRecs, TabtoPass, SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds,SelectedSupplier).ToList();
            TextChartTitle = eTurns.DTO.ResDashboard.TitleUnsubmittedOrder;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleUnsubmittedOrder;
            break;
        case "submitted":
            TabtoPass = "[^]2";
            SortColumnsName = "NoOfLineItems DESC";
            lstOrdersAll = objRequisitionMasterDAL.GetOrderMasterPagedDataNormal(out OrderSupplierList,0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out totalRecs, TabtoPass, SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds,SelectedSupplier).ToList();
            TextChartTitle = eTurns.DTO.ResDashboard.TitleUnapprovedOrder;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleUnapprovedOrder;
            break;
        case "approved":
            TabtoPass = "[^]7";
            SortColumnsName = "InCompleteItemCount DESC";
            lstOrdersAll = objRequisitionMasterDAL.GetOrderMasterPagedDataNormal(out OrderSupplierList,0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out totalRecs, TabtoPass, SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.UserSupplierIds,SelectedSupplier).ToList();
            TextChartTitle = eTurns.DTO.ResDashboard.TitleTobeReceivedOrder;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleTobeReceivedOrder;
            break;
        case "receivable":
            TabtoPass = "";
            SortColumnsName = "QuantityToReceive DESC";
            eTurns.DAL.ReceiveOrderDetailsDAL controller = new eTurns.DAL.ReceiveOrderDetailsDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
            lstReceivableAll = controller.GetReceiveListForDashboardChart(out ReceiveSupplierList, 0, eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph, out totalRecs,SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false, "4,5,6,7", eTurnsWeb.Helper.SessionHelper.UserSupplierIds , SelectedSupplier).ToList();
            //if (lstReceivableAll != null)
            //{
            //    lstReceivableAll = lstReceivableAll.Where(x => !x.IsCloseItem.GetValueOrDefault(false)).ToList();
            //}
            TextChartTitle = eTurns.DTO.ResDashboard.TitleReceivableItems;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleReceivableItems;
            XAxisTitle = eTurns.DTO.ResDashboard.ReceivableItemChartXTitle;
            YaxisTitle = eTurns.DTO.ResDashboard.ReceivableItemChartYTitle;
            break;
        case "bottomspend":
            SortColumnsName = "OrderCost asc";
            TabtoPass = "";
            lstBTSpendAll = objRequisitionMasterDAL.GetDashboardTopAndBottomSpend(0, int.MaxValue, out totalRecs, TabtoPass, SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, 10, "BotttomSpend").ToList();
            TextChartTitle = eTurns.DTO.ResDashboard.TitleBottomSpend;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleBottomSpend;
            XAxisTitle = eTurns.DTO.ResDashboard.BottomTopSpendChartXTitle;
            YaxisTitle = eTurns.DTO.ResDashboard.BottomTopSpendChartYTitle;
            break;
        case "topspend":
            SortColumnsName = "OrderCost DESC";
            TabtoPass = "";
            lstBTSpendAll = objRequisitionMasterDAL.GetDashboardTopAndBottomSpend(0, int.MaxValue, out totalRecs, TabtoPass, SortColumnsName, eTurnsWeb.Helper.SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, 10, "TopSpend").ToList();
            TextChartTitle = eTurns.DTO.ResDashboard.TitleTopSpend;
            TextChartSubTitle = eTurns.DTO.ResDashboard.SubTitleTopSpend;
            XAxisTitle = eTurns.DTO.ResDashboard.TopSpendChartXTitle;
            YaxisTitle = eTurns.DTO.ResDashboard.TopSpendChartYTitle;
            break;
    }
    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
    //int SetItemnumberLength = Convert.ToInt32(Settinfile.Element("ChartLabelCharSize") == null ? "0" : Settinfile.Element("ChartLabelCharSize").Value != null ? Settinfile.Element("ChartLabelCharSize").Value : "0");
    int SetItemnumberLength = Convert.ToInt32(eTurns.DTO.SiteSettingHelper.ChartLabelCharSize == string.Empty ? "0" : eTurns.DTO.SiteSettingHelper.ChartLabelCharSize != string.Empty ? eTurns.DTO.SiteSettingHelper.ChartLabelCharSize : "0");
    
    string strSupplierOptions = "";

    if (ChartType == "receivable")
    {
        if (lstReceivableAll != null && lstReceivableAll.Count > 0)
        {
            lstReceivable = lstReceivableAll;
        }
        else
            lstReceivable = new List<eTurns.DTO.ReceivableItemDTO>();

        //-------------------Prepare DDL Data-------------------
        //

        if (ReceiveSupplierList != null && ReceiveSupplierList.Any())
        {
            List<string> lstSupplier = ReceiveSupplierList.Select(x => "<option value='" + x.OrderSupplierID + "'>" + x.OrderSupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
            strSupplierOptions = string.Join("", lstSupplier);
        }

        foreach (var key in lstReceivable)
        {
            string TTip = string.Empty;
            double YAxixVal = 0;

            YAxixVal = key.ApprovedQuantity - key.ReceivedQuantity;
            TTip = (key.ApprovedQuantity - key.ReceivedQuantity).ToString(QuantityFormat);
            string AxisLabelText = string.Empty;
            if (key.ItemNumber.Length > SetItemnumberLength && SetItemnumberLength > 0)
            {
                AxisLabelText = Convert.ToString(key.ItemNumber).Substring(0, SetItemnumberLength) + "....";
            }
            else
            {
                AxisLabelText = Convert.ToString(key.ItemNumber);
            }
            chartOrder.Series["srsOrder"].Points.Add(new DataPoint
            {
                AxisLabel = AxisLabelText,
                YValues = new double[] { (double)(YAxixVal) },
                Color = System.Drawing.Color.FromArgb(201, 77, 32),
                Url = Url.Action("ReceiveList", "Receive") + "?fromdashboard=yes&incomplete=yes&itemnumber=" + HttpUtility.UrlEncode(key.ItemNumber),
                ToolTip = TTip
            });
        }
    }
    else if (ChartType == "bottomspend")
    {

        lstBTSpend = lstBTSpendAll;
        if (!String.IsNullOrEmpty(SelectedSupplier))
            lstBTSpend = lstBTSpend.Where(x => SelectedSupplier.Split(',').Contains(x.SupplierID.ToString())).ToList();

        if (lstBTSpend != null)
            lstBTSpend = lstBTSpend.Take(eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph).ToList();
        else
            lstBTSpend = new List<eTurns.DTO.DashboardBottomAndTopSpendDTO>();

        //-------------------Prepare DDL Data-------------------
        //
        if (lstBTSpendAll.Count > 0)
        {
            if (lstBTSpendAll.Where(x => x.SupplierName != null && x.SupplierName.Trim() != "").Count() > 0)
            {
                List<string> lstSupplier = lstBTSpendAll.Where(x => x.SupplierName != null && x.SupplierName.Trim() != "").GroupBy(x => new { x.SupplierID, x.SupplierName }).OrderBy(x => x.Key.SupplierName).Select(x => "<option value='" + x.Key.SupplierID + "'>" + x.Key.SupplierName + "(" + x.Count() + ")" + "</option>").ToList();
                strSupplierOptions = String.Join("", lstSupplier);
            }
        }



        if (lstBTSpend != null && lstBTSpend.Count > 0)
        {
            //if (!String.IsNullOrEmpty(SelectedSupplier))
            //    lstBTSpend = lstBTSpend.Where(x => SelectedSupplier.Split(',').Contains(x.SupplierID.ToString())).ToList();

            foreach (var key in lstBTSpend)
            {
                string TTip = string.Empty;
                double YAxixVal = 0;
                YAxixVal = key.OrderCost;
                TTip = eTurnsWeb.Helper.SessionHelper.CurrencySymbol + "  " + (key.OrderCost).ToString(priceFormte,eTurnsWeb.Helper.SessionHelper.RoomCulture);               

                string AxisLabelText = string.Empty;
                if (key.ItemNumber.Length > SetItemnumberLength && SetItemnumberLength > 0)
                {
                    AxisLabelText = Convert.ToString(key.ItemNumber).Substring(0, SetItemnumberLength) + "....";
                }
                else
                {
                    AxisLabelText = Convert.ToString(key.ItemNumber);
                }
                chartOrder.Series["srsOrder"].Points.Add(new DataPoint
                {
                    AxisLabel = AxisLabelText,
                    YValues = new double[] { (double)(YAxixVal) },
                    Color = System.Drawing.Color.FromArgb(201, 77, 32),
                    Url = Url.Action("ItemMasterList", "Inventory") + "?fromdashboard=yes&ItemGUID=" + HttpUtility.UrlEncode(key.ItemGUID.ToString()),
                    ToolTip = TTip
                });
            }
        }



        //------------------------------------------------

    }
    else if (ChartType == "topspend")
    {
        lstBTSpend = lstBTSpendAll;
        if (!String.IsNullOrEmpty(SelectedSupplier))
            lstBTSpend = lstBTSpend.Where(x => SelectedSupplier.Split(',').Contains(x.SupplierID.ToString())).ToList();

        if (lstBTSpend != null)
            lstBTSpend = lstBTSpend.Take(eTurnsWeb.Helper.ChartHelper.MaxItemsInGraph).ToList();
        else
            lstBTSpend = new List<eTurns.DTO.DashboardBottomAndTopSpendDTO>();

        //-------------------Prepare DDL Data-------------------
        //
        if (lstBTSpendAll.Count > 0)
        {
            if (lstBTSpendAll.Where(x => x.SupplierName != null && x.SupplierName.Trim() != "").Count() > 0)
            {
                List<string> lstSupplier = lstBTSpendAll.Where(x => x.SupplierName != null && x.SupplierName.Trim() != "").GroupBy(x => new { x.SupplierID, x.SupplierName }).OrderBy(x => x.Key.SupplierName).Select(x => "<option value='" + x.Key.SupplierID + "'>" + x.Key.SupplierName + "(" + x.Count() + ")" + "</option>").ToList();
                strSupplierOptions = String.Join("", lstSupplier);
            }
        }

        if (lstBTSpend != null && lstBTSpend.Count > 0)
        {
            if (!String.IsNullOrEmpty(SelectedSupplier))
                lstBTSpend = lstBTSpend.Where(x => SelectedSupplier.Split(',').Contains(x.SupplierID.ToString())).ToList();

            foreach (var key in lstBTSpend)
            {
                string TTip = string.Empty;
                double YAxixVal = 0;
                YAxixVal = key.OrderCost;
                TTip = eTurnsWeb.Helper.SessionHelper.CurrencySymbol + "  " + (key.OrderCost).ToString(priceFormte,eTurnsWeb.Helper.SessionHelper.RoomCulture);           
                string AxisLabelText = string.Empty;

                if (key.ItemNumber.Length > SetItemnumberLength && SetItemnumberLength > 0)
                {
                    AxisLabelText = Convert.ToString(key.ItemNumber).Substring(0, SetItemnumberLength) + "....";
                }
                else
                {
                    AxisLabelText = Convert.ToString(key.ItemNumber);
                }
                chartOrder.Series["srsOrder"].Points.Add(new DataPoint
                {
                    AxisLabel = AxisLabelText,
                    YValues = new double[] { (double)(YAxixVal) },
                    Color = System.Drawing.Color.FromArgb(201, 77, 32),
                    Url = Url.Action("ItemMasterList", "Inventory") + "?fromdashboard=yes&ItemGUID=" + HttpUtility.UrlEncode(key.ItemGUID.ToString()),
                    ToolTip = TTip
                });
            }
        }
    }
    else
    {
        if (lstOrdersAll != null && lstOrdersAll.Count > 0)
        {
            lstOrders = lstOrdersAll;
        }
        else
            lstOrders = new List<eTurns.DTO.OrderMasterDTO>();

        //-------------------Prepare DDL Data-------------------
        //

        if (OrderSupplierList != null && OrderSupplierList.Any())
        {
            List<string> lstSupplier = OrderSupplierList.Select(x => "<option value='" + x.Supplier + "'>" + x.SupplierName + "(" + x.TotalRecords + ")" + "</option>").ToList();
            strSupplierOptions = string.Join("", lstSupplier);
        }

        //------------------------------------------------------
        //
        foreach (var key in lstOrders)
        {
            string TTip = string.Empty;
            double YAxixVal = 0;

            switch (ChartType.ToLower())
            {
                case "unsubmitted":
                    YAxixVal = key.NoOfLineItems ?? 0;
                    TTip = (key.NoOfLineItems ?? 0).ToString(QuantityFormat);
                    break;
                case "submitted":
                    YAxixVal = key.NoOfLineItems ?? 0;
                    TTip = (key.NoOfLineItems ?? 0).ToString(QuantityFormat);
                    break;
                case "approved":
                    YAxixVal = key.InCompleteItemCount;
                    TTip = (key.InCompleteItemCount).ToString(QuantityFormat);
                    break;

            }
            string AxisLabelText = string.Empty;
            if (key.OrderNumber.Length > SetItemnumberLength && SetItemnumberLength > 0)
            {
                AxisLabelText = Convert.ToString(key.OrderNumber).Substring(0, SetItemnumberLength) + "....";
            }
            else
            {
                AxisLabelText = Convert.ToString(key.OrderNumber);
            }
            chartOrder.Series["srsOrder"].Points.Add(new DataPoint
            {
                AxisLabel = AxisLabelText,
                YValues = new double[] { (double)(YAxixVal) },
                Color = System.Drawing.Color.FromArgb(201, 77, 32),
                Url = Url.Action("OrderList", "Order") + "?fromdashboard=yes&OrderID=" + HttpUtility.UrlEncode(key.ID.ToString()),
                ToolTip = TTip
            });
        }
    }

    chartOrder.Titles["ttlOrder"].Text = TextChartTitle;
    chartOrder.Titles["subttlOrder"].Text = TextChartSubTitle;
    chartOrder.ChartAreas["chartAreaOrder"].AxisX.Title = XAxisTitle;
    chartOrder.ChartAreas["chartAreaOrder"].AxisY.Title = YaxisTitle;

    chartOrder.ChartAreas["chartAreaOrder"].AxisX.Interval = 1;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartStyle(chartOrder);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartTitleStyle(chartOrder.Titles["ttlOrder"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSubTitleStyle(chartOrder.Titles["subttlOrder"]);
    chartOrder.BorderSkin.SkinStyle = BorderSkinStyle.Raised;
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartAreaStyle(chartOrder.ChartAreas["chartAreaOrder"]);
    eTurnsWeb.Helper.ChartHelper.SeteTurnsChartSeries(chartOrder.Series["srsOrder"]);

%>
<script type="text/javascript">
    $(document).ready(function () {
        //------------------------------------------------------------------------------------------------
        //
        var SelectedSupplier = '<%= SelectedSupplier %>';
        var SupplierOptions = "<%= strSupplierOptions %>";
        if (SelectedSupplier == null || SelectedSupplier == undefined || SelectedSupplier.trim() == '')
            SelectedSupplier = GlogalReplanishSupplierValue;

        BindMultiSelect("ddlReplanishOrderChartSupplier", SupplierOptions, SelectedSupplier, '')
        $("#ddlReplanishOrderChartSupplier").multiselect().bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            GlogalReplanishSupplierValue = GetMultiselectSelectedValue('ddlReplanishOrderChartSupplier');
            ddlReplanishOrderSupplierCategoryChange(ui, event);
        });

        <%--//------------------------------------------------------------------------------------------------
        //
        BindMultiSelect("ddlReplanishOrderChartSupplier", "<%= strSupplierOptions %>", '<%= SelectedSupplier %>', '')
        $("#ddlReplanishOrderChartSupplier").multiselect().bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            ddlReplanishOrderSupplierCategoryChange(ui, event);
        });--%>
    });

    function ddlReplanishOrderSupplierCategoryChange(chkDropdown, event) {
        //debugger;
        var arrDdlCartChartSupplier = $("#ddlReplanishOrderChartSupplier").multiselect("getChecked").map(function () { return this.value }).get();
        var SelectedSupplier = arrDdlCartChartSupplier.join(",");

        if ($('#lnkordernotsubmitted').hasClass('liahover')) {
            BindOrderStatusGrid('lnkordernotsubmitted', 'Unsubmitted', SelectedSupplier)
        }
        else if ($('#lnkordernotapproved').hasClass('liahover')) {
            BindOrderStatusGrid('lnkordernotapproved', 'Submitted', SelectedSupplier)
        }
        else if ($('#lnkordernotreceived').hasClass('liahover')) {
            BindOrderStatusGrid('lnkordernotreceived', 'Approved', SelectedSupplier)
        }
        else if ($('#lnkitemreceivable').hasClass('liahover')) {
            BindOrderStatusGrid('lnkitemreceivable', 'Receivable', SelectedSupplier)
        }
        else if ($('#lnkordbottomspend').hasClass('liahover')) {
            BindOrderStatusGrid('lnkordbottomspend', 'BottomSpend', SelectedSupplier)
        }
        else if ($('#lnkordtopspend').hasClass('liahover')) {
            BindOrderStatusGrid('lnkordtopspend', 'TopSpend', SelectedSupplier)
        }
    }
</script>
<div id="divCartOrderTab" class="Lnavd" style="float: left; width: 80%; padding-top: 10px;">

    <div style="float: left; width: 48%">
        <span style="float: left">Supplier:&nbsp;</span><select id="ddlReplanishOrderChartSupplier"></select>
    </div>

</div>
<asp:Chart ID="chartOrder" runat="server" Height="250" Width="570" BorderlineWidth="1">
    <Titles>
        <asp:Title Name="ttlOrder">
        </asp:Title>
        <asp:Title Name="subttlOrder">
        </asp:Title>
    </Titles>
    <BorderSkin SkinStyle="Raised" />
    <ChartAreas>
        <asp:ChartArea Name="chartAreaOrder" BackColor="Transparent">
            <AxisX>
            </AxisX>
            <AxisY>
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Series>
        <asp:Series ChartArea="chartAreaOrder" XValueType="String" Name="srsOrder" ChartType="Column"
            Font="Trebuchet MS, 8.25pt, style=Bold" CustomProperties="DoughnutRadius=60, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelativePieSize=20,PieLabelStyle=Disabled,PieStartAngle=270"
            MarkerStyle="Circle" BorderColor="64, 64, 64, 64" Color="180, 65, 140, 240" YValueType="Double"
            ToolTip="#VALY">
        </asp:Series>
    </Series>
</asp:Chart>
