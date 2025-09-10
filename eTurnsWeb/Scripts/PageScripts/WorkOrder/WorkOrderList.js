var oTable;
var IsRefreshGrid = false;
var deleteURL = "/WorkOrder/DeleteWOMasterRecords";
var sImageUrl = "/Content/images/";
var anOpen = [];
var IsDeletePopupOpen = false;
var AllowDeletePopup = true;
var SelectedHistoryRecordID = 0;
var HistorySelected;
var WorkUniqueID = '';
var objColumns = {};
var objDummyColums = {};
var OrderListFlag = false;

var _WorkOrderList = (function ($) {

    var self = {};

    self.urls = {
        SaveGridStateUrl: null,
        LoadGridStateUrl: null,
        WOMasterListAjaxUrl: null,
        UpdateDataUrl: null,
        WODetailsUrl: null,
        BlankSessionUrl: null
        
    }

    self.initUrls = function (SaveGridStateUrl, LoadGridStateUrl, WOMasterListAjaxUrl
        , UpdateDataUrl, WODetailsUrl, BlankSessionUrl) {
        self.urls.SaveGridStateUrl = SaveGridStateUrl;
        self.urls.LoadGridStateUrl = LoadGridStateUrl;
        self.urls.WOMasterListAjaxUrl = WOMasterListAjaxUrl;
        self.urls.UpdateDataUrl = UpdateDataUrl;
        self.urls.WODetailsUrl = WODetailsUrl;
        self.urls.BlankSessionUrl = BlankSessionUrl;
    };

    self.init = function () {
        self.initEvents();
       
    };

    self.initEvents = function () {
        $(document).ready(function () {
            self.initDataTable();
            //HIDE PRINT CONTAINER
            $('.DTTT_container').css('z-index', '-1');

            if (isCostWOL == 'False') {

                HideColumnUsingClassName("myDataTable");
            }

            //fromPull
            var QueryStringParam = _Common.getParameterByName('fromPull');
            if (QueryStringParam != '' && QueryStringParam == 'yes') {
                $("#tab1").click();
            }

            var QueryStringParam3 = _Common.getParameterByName('fromdashboard');
            var QueryStringParam4 = _Common.getParameterByName('WorkOrderGUID');
            if (QueryStringParam4 != '' && QueryStringParam3 == 'yes') {
                //ShowEditTab("RequisitionEdit/" + QueryStringParam2 ,"frmRequisitionMaster");
                ShowEditTabGUID("WOEdit?WorkOrderGUID=" + QueryStringParam4, "frmWOMaster");
            }

            $('#deleteRows').click(function () {

                /* IF PRINT PREVIEW DONT SHOW CONTEXT MENU */
                if ($("body").hasClass('DTTT_Print')) {
                    return false;
                }
                /* IF PRINT PREVIEW DONT SHOW CONTEXT MENU */

                //                var anSelectedReq = fnGetSelected(oTable);
                //                var stringIDs = "";
                //                for (var i = 0; i <= anSelectedReq.length - 1; i++) {
                //                    var SpanReqStatus = $(anSelectedReq[i]).find('#spnWOStatus').text();
                //                    if(SpanReqStatus == "Open")
                //                        stringIDs = stringIDs + anSelectedReq[i].id + ",";
                //                }
                //                if (stringIDs.length > 0) {
                //                    $('#basic-modal-content').modal();
                //                    IsDeletePopupOpen = true;
                //                }
                //                else
                //                {
                //                    $("#spanGlobalMessage").text("Closed record(s) can not deleted.");
                //                    $('div#target').fadeToggle();
                //                    $("div#target").delay(2000).fadeOut(200);
                //                }
            });

            /* HISTORY related data deleated and archived START */
            $('#ViewHistory').live('click', function () {
                HistorySelected = _WorkOrderList.fnGetSelected(oTable);
                if (HistorySelected != undefined && HistorySelected.length == 1) {
                    $("#atab5").click();
                }
                else {
                    $('#tab5').html('');
                    $("#spanGlobalMessage").html(msgSelectForViewHistory);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    return false;
                }
            });
            /* HISTORY related data deleated and archived END */
            $('#actionCloseWorkOrder').die('click');
            $('#actionCloseWorkOrder').click(function () {
                var anSelected = fnGetSelected(oTable);
                var cntIDs = 0;
                for (var i = 0; i <= anSelected.length - 1; i++) {
                    var aData = oTable.fnGetData(anSelected[i]);
                    if (typeof (aData.WOStatus) != "undefined" && aData.WOStatus != null && aData.WOStatus != "Close") {
                        cntIDs = cntIDs + 1;
                    }
                }
                if (cntIDs > 0) {
                    $('#WorkOrderListCloseConfirm').modal();
                }
                else {
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon').text(msgSelectOpenWorkOrderToClose);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                }
            });

            $('#bntCloseWorkOrderListConfirmYes').die('click');
            $('#bntCloseWorkOrderListConfirmYes').click(function () {

                $('#DivLoading').show();
                var anSelected = fnGetSelected(oTable);
                var stringIDs = "";
                var guids = "";
                var cntIDs = 0;
                if (anSelected.length !== 0) {
                    for (var i = 0; i <= anSelected.length - 1; i++) {
                        var aData = oTable.fnGetData(anSelected[i]);
                        if (typeof (aData.WOStatus) != "undefined" && aData.WOStatus != null && aData.WOStatus != "Close") {
                            stringIDs = stringIDs + aData.ID + ",";
                            cntIDs = cntIDs + 1;

                            if (typeof (aData.GUID) != undefined && aData.GUID != null && aData.GUID.length > 0)
                            {
                                guids = guids + aData.GUID + ",";
                            }
                        }                        
                    }

                    if (cntIDs > 0) {
                        $.ajax({
                            url: urlCloseWorkOrders,
                            type: "POST",
                            data: { ids: stringIDs, Guids: guids},
                            success: function (responce) {
                                if (responce.Status == "ok") {
                                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon').text(RecordClosedSuccessfully.replace("{0}", cntIDs));
                                    $('div#target').fadeToggle();
                                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                    oTable.fnDraw();
                                }
                                else {
                                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon').text(MsgErrorInProcess);
                                    $('div#target').fadeToggle();
                                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                }
                                closeWorkOrderDialog();
                            },
                            error: function (err) {
                                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon').text(responce.Message);
                                $('div#target').fadeToggle();
                                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                            }
                        });
                    }
                    else {
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon').text(msgSelectOpenWorkOrderToClose);
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        closeWorkOrderDialog();
                        $('#DivLoading').hide();
                    }
                }
            });
        }); // ready
    };

    self.initDataTable = function () {
        objColumns = GetGridHeaderColumnsObject('myDataTable');
        objDummyColums = objColumns;
        LoadTabs();
        var gaiSelected = [];

        var arrAOColumnsWO = [
            { mDataProp: null, sClass: "read_only center NotHide RowNo", "bSortable": false, sDefaultContent: '' },
            {
                "mDataProp": null,
                "bSortable": false,
                "sClass": "read_only control center",
                "sDefaultContent": '<img src="' + sImageUrl + 'drildown_open.jpg' + '">'
            },
            { "mDataProp": "ID", "sClass": "read_only" },
            {
                "mDataProp": "WOName",
                "sClass": "read_only",
                "sDefaultContent": '',
                "bSortable": true,
                "bSearchable": false,
                "fnRender": function (obj, val) {
                    return "<a id='aEditLink' onclick='BlankSession();  return ShowEditTabGUID(&quot;WOEdit?WorkOrderGUID=" + obj.aData.GUID.toString() + "&quot;,&quot;frmWOMaster&quot;)' href='JavaScript:void(0);'>" + obj.aData.WOName + "</a> " + " <span id='spnWOStatus' style='display:none'>" + obj.aData.WOStatus + "</span>" + " <input type='hidden' id='WorkOrderGUID' value='" + obj.aData.GUID.toString() + "' />";
                }
            },
            { "mDataProp": "ReleaseNumber", "sClass": "read_only" },
            {
                "mDataProp": "RequisitionNumber", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    if (obj.aData.RequisitionNumber != null && obj.aData.RequisitionNumber != NaN)
                        return obj.aData.RequisitionNumber.toString().substring(1, obj.aData.RequisitionNumber.length);
                    else
                        return "";
                }
            },
            { "mDataProp": "WOStatus", "sClass": "read_only" },
            { "mDataProp": "Technician", "sClass": "read_only" },
            { "mDataProp": "Customer", "sClass": "read_only" },
            { "mDataProp": "AssetName", "sClass": "read_only" },
            { "mDataProp": "ToolName", "sClass": "read_only" },
            { "mDataProp": "Odometer_OperationHours", "sClass": "read_only" },
            { "mDataProp": "Description", "sClass": "read_only" },
            { "mDataProp": "UsedItems", "sClass": "read_only numericalign" },
            {
                "mDataProp": "UsedItemsCost", "sClass": "read_only numericalign isCost",
                "fnRender": function (obj, val) {
                    if (obj.aData.PriseSelectionOption != null && obj.aData.PriseSelectionOption == 1) {
                        if (obj.aData.UsedItemsCost != null && obj.aData.UsedItemsCost != NaN)
                            return FormatedCostQtyValues(obj.aData.UsedItemsCost, 1);
                        else
                            return FormatedCostQtyValues(0, 1);
                    }
                    else if (obj.aData.PriseSelectionOption != null && obj.aData.PriseSelectionOption == 2) {
                        {
                            if (obj.aData.UsedItemsSellPrice != null && obj.aData.UsedItemsSellPrice != NaN)
                                return FormatedCostQtyValues(obj.aData.UsedItemsSellPrice, 1);
                            else
                                return FormatedCostQtyValues(0, 1);
                        }
                    }
                    else {
                        return FormatedCostQtyValues(0, 1);
                    }
                }
            },
            { "mDataProp": "RoomName", "sClass": "read_only" },
            { "mDataProp": "Created", "sClass": "read_only", "fnRender": function (obj, val) { return obj.aData.CreatedDate; } },
            { "mDataProp": "Updated", "sClass": "read_only", "fnRender": function (obj, val) { return obj.aData.UpdatedDate; } },
            //                            { "mDataProp": "CreatedDate", "sClass": "read_only" },
            //                            { "mDataProp": "UpdatedDate", "sClass": "read_only" },
            { "mDataProp": "CreatedByName", "sClass": "read_only" },
            { "mDataProp": "UpdatedByName", "sClass": "read_only" },
            {
                "mDataProp": "SignatureName", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    if (obj.aData.IsSignatureCapture && val.length > 0) {
                        return '<img src="/Uploads/WorkOrderSignature/' + obj.aData.CompanyID + '/' + val + '" alt="WOSignature" height="50px"  />'
                    }
                    else {
                        return "";
                    }
                }
            },

            { "mDataProp": "AddedFrom", "sClass": "read_only" },
            { "mDataProp": "EditedFrom", "sClass": "read_only" },
            {
                "mDataProp": "ReceivedOnWeb", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    //return GetDateInFullFormat(val);
                    return obj.aData.ReceivedOnWebDate;

                }
            },
            {
                "mDataProp": "ReceivedOn", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    // return GetDateInFullFormat(val);
                    return obj.aData.ReceivedOnDate;
                }
            },
            {
                "mDataProp": "SupplierName", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    // return GetDateInFullFormat(val);
                    return obj.aData.SupplierName;
                }
            },
            {
                "mDataProp": "SupplierAccountNumberName", "sClass": "read_only", "bSortable": false,
                "fnRender": function (obj, val) {
                    // return GetDateInFullFormat(val);
                    return obj.aData.SupplierAccountNumberName;
                }
            },
            {
                "mDataProp": "ProjectSpendName", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    if (obj.aData.ProjectSpendName != null && obj.aData.ProjectSpendName != NaN)
                        return obj.aData.ProjectSpendName.toString().substring(1, obj.aData.ProjectSpendName.length);
                    else
                        return "";
                }
            }

        ];

        $.each(arrWorkOrder_Col_WO, function (index, val) {
            arrAOColumnsWO.push(val);
        });


        oTable = $('#myDataTable').dataTable({
            "bJQueryUI": true,
            "bScrollCollapse": true,
            "sScrollX": "150%",
            "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>T',
            "oColVis": {},
            "aaSorting": [[2, "asc"]],
            "oColReorder": {},
            "sPaginationType": "full_numbers",
            "bProcessing": true,
            "bStateSave": true,
            "oLanguage": oLanguageWO,
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                if (aData.IsDeleted == true && aData.sArchived == true)
                    $(nRow).css('background-color', '#B9BCBF');
                //nRow.className = "GridDeleatedArchivedRow";
                else if (aData.IsDeleted == true)
                    $(nRow).css('background-color', '#FFCCCC');
                //   nRow.className = "GridDeletedRow";
                else if (aData.IsArchived == true)
                    $(nRow).css('background-color', '#CCFFCC');
                //   nRow.className = "GridArchivedRow";
                $("td.RowNo:first", nRow).html(this.fnSettings()._iDisplayStart + iDisplayIndex + 1);
                return nRow;
            },
            "fnStateSaveParams": function (oSettings, oData) {
                if (oData.oSearch != null)
                    oData.oSearch.sSearch = "";
                //if (PostCount > 1) {
                $.ajax({
                    "url": _WorkOrderList.urls.SaveGridStateUrl,
                    "type": "POST",
                    data: { Data: JSON.stringify(oData), ListName: 'WorkOrder' },
                    "async": false,
                    cache: false,
                    "dataType": "json",
                    "success": function (json) {
                        o = json;
                    }
                });
                //}
            },
            "fnStateLoad": function (oSettings) {
                var o;
                $.ajax({
                    "url": _WorkOrderList.urls.LoadGridStateUrl,
                    "type": "POST",
                    data: { ListName: 'WorkOrder' },
                    "async": false,
                    cache: false,
                    "dataType": "json",
                    "success": function (json) {
                        if (json.jsonData != '')
                            o = JSON.parse(json.jsonData);
                    }
                });
                return o;
            },
            "bServerSide": true,
            "sAjaxSource": _WorkOrderList.urls.WOMasterListAjaxUrl,
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                //PostCount = PostCount + 1;
                var arrCols = new Array();
                var objCols = this.fnSettings().aoColumns;
                for (var i = 0; i <= objCols.length - 1; i++) {
                    arrCols.push(objCols[i].mDataProp);
                }
                for (var j = 0; j <= aoData.length - 1; j++) {
                    if (aoData[j].name == "sColumns") {
                        aoData[j].value = arrCols.join("|");
                        break;
                    }
                }
                if (oSettings.aaSorting.length != 0) {
                    //aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
                    var sortValue = ""
                    for (var i = 0; i <= oSettings.aaSorting.length - 1; i++) {
                        if (sortValue.length > 0)
                            sortValue += ", "
                        sortValue += arrCols[oSettings.aaSorting[i][0]] + ' ' + oSettings.aaSorting[i][1];

                    }
                    aoData.push({ "name": "SortingField", "value": sortValue });
                }
                else
                    aoData.push({ "name": "SortingField", "value": "0" });

                aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
                aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });


                oSettings.jqXHR = $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "cache": false,
                    "data": aoData,
                    "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
                    "success": fnCallback,
                    beforeSend: function () {
                        $('#myDataTable').removeHighlight();
                        $('.dataTables_scroll').css({ "opacity": 0.2 });
                    },
                    complete: function () {
                        $('.dataTables_scroll').css({ "opacity": 1 });
                        if ($("#global_filter").val() != '') {
                            $('#myDataTable').highlight($("#global_filter").val());
                        }
                        CallShowHideData();
                    }
                })
            },
            "fnInitComplete": function () {
                $('.ColVis').detach().appendTo(".setting-arrow");
            },
            "aoColumns": arrAOColumnsWO
        }).makeEditable({
            sUpdateURL: self.urls.UpdateDataUrl
        });

        /*Functions used for nasted data binding START*/
        $("#myDataTable").on("click", "td.control", function (event) {

            if ($(this).find('img').length <= 0)
                return;

            var nTr = this.parentNode;


            var i = $.inArray(nTr, anOpen);

            if (i === -1) {
                $('img', this).attr('src', sImageUrl + "drildown_close.jpg");
                oTable.fnOpen(nTr, self.fnFormatDetails(oTable, nTr), '');
                anOpen.push(nTr);
            }
            else {
                $('img', this).attr('src', sImageUrl + "drildown_open.jpg");
                oTable.fnClose(nTr);
                anOpen.splice(i, 1);
            }
        });

        $('#myDataTable').on('tap click', 'a[id^="aEditLink"]', function () {
            var tr = $(this).parent().parent();
            $("#myDataTable").find("tbody tr").removeClass("row_selected");
            $(tr).addClass('row_selected');

        });

    };

    self.fnFormatDetails = function (oTable, nTr) {
        var oData = oTable.fnGetData(nTr);
        var sOut = '';
        $('#DivLoading').show();
        $.ajax({
            "url": _WorkOrderList.urls.WODetailsUrl,
            data: { WorkOrderGUID: oData.GUID },
            "async": false,
            cache: false,
            "dataType": "text",
            "success": function (json) {
                sOut = json;
                $('#DivLoading').hide();
            },
            error: function (response) {
                //
            }
        });

        return sOut;
    }

    self.fnGetSelected =  function (oTableLocal) {
        return oTableLocal.$('tr.row_selected');
    }


    self.HistoryTabClick = function () {
        if ($('#IsDeletedRecords').is(':checked')) {
            $('#undeleteRows').css('display', '');
            $('#deleteRows').css('display', 'none');
        }
        else {
            $('#undeleteRows').css('display', 'none');
            $('#deleteRows').css('display', '');
        }
        GetHistoryData();
    }

    /// Private functions


    function GetHistoryData() {
        HistorySelected = _WorkOrderList.fnGetSelected(oTable);
        if (HistorySelected != undefined && HistorySelected.length == 1) {
            var WOGUID = $(HistorySelected).find('#WorkOrderGUID')[0].value;
            SelectedHistoryRecordID = WOGUID;
            $('#CtabCL').html('');
            $('#DivLoading').show();
            $("#CTab").hide();
            $("#CtabCL").show();
            $('#CtabCL').load('/Master/WOMHistory', function () { $('#DivLoading').hide(); });
        }
        else {
            $('#CtabCL').html('');
            $("#spanGlobalMessage").html(msgSelectForViewHistory);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }
    }


    return self;

})(jQuery); // _CreateWOItem end




function BlankSession(ID) {
    $.get(_WorkOrderList.urls.BlankSessionUrl, function (data) { });

}

function PrintAttachedDocs(lnk) {
    var rowSelected = _WorkOrderList.fnGetSelected(oTable);
    var arrIds = new Array();
    if (rowSelected.length > 0) {
        for (var i = 0; i < rowSelected.length; i++) {
            var WOGUID = $(rowSelected[i]).find('#WorkOrderGUID')[0].value;
            arrIds.push(WOGUID);
        }
    }

    if (arrIds.length) {
        $.ajax({
            url: 'DownloadWorkOrderDocument',
            type: 'Post',
            data: JSON.stringify(arrIds),
            dataType: 'json',
            async: false,
            contentType: 'application/json',
            success: function (result) {
                if (result.Status) {
                    var isURLHttps = false;
                    var CurrentURL = window.location.href;
                    if (CurrentURL.indexOf("localhost:") < 0
                        && CurrentURL.toLowerCase().indexOf("demo") < 0)
                    {
                        if (CurrentURL.indexOf("https:") >= 0) {
                            isURLHttps = true;
                        }
                    }

                    if (result.ReturnFiles.length > 0) {
                        for (var i = 0; i < result.ReturnFiles.length; i++) {
                            if (isURLHttps == false) {
                                if (result.ReturnFiles[i].indexOf("https:") >= 0) {
                                    result.ReturnFiles[i] = result.ReturnFiles[i].replace("https:", "http:");
                                }
                            }
                            if (isURLHttps == true) {
                                if (result.ReturnFiles[i].indexOf("http:") >= 0) {
                                    result.ReturnFiles[i] = result.ReturnFiles[i].replace("http:", "https:");
                                }
                            }
                            window.open(result.ReturnFiles[i]);
                        }
                    } else {
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        $("#spanGlobalMessage").html('@ResCommon.NoDocsToDownload');
                        showNotificationDialog();
                    }
                }
            },
            error: function (xhr) {
                alert(MsgResErrorInProcess);
            }
        });
    }
    else {
        alert(MsgSelectRow);

    }
}

function closeWorkOrderDialog() {
    $.modal.impl.close();
}