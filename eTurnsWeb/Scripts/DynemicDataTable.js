var oTableGlobalTABLE;
var sImageUrl = "/Content/images/";
var oTableInnerGrid;
var oTableDynemicDeleteURL = '';
var oAfterDeleteCallbackFunction = '';
var OrderDetailGUID = '';
var RequisitionDetailGuid = '';
var WorkOrderGuid = '';
var toolCheckoutGUID = '';

var paramsMS = [];
var PopUpName = '';
var btnPopUpName = '';
var ToolGUID = '';
function PrepareItemLocationDataTable(TableName, UniqueID, AjaxSourceMehtod, InnerPageName, ColumnObject, IsTSerialAvail, TSearchTerm, fnGetJson) {
    var ObjectTable = TableName + UniqueID;
    oTableGlobalTABLE = $('#' + ObjectTable).dataTable({
        "bJQueryUI": true,
        "bScrollCollapse": true,
        "bAutoWidth": false,
        "sScrollX": "100%",
        "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>',
        //"aaSorting": [[1, "asc"]],
        "aaSorting": [[2, "asc"]],
        "oColReorder": {},
        "sPaginationType": "full_numbers",
        "bProcessing": true,
        "bStateSave": true,
        "bServerSide": true,
        "fnStateSaveParams": function (oSettings, oData) {
            $.ajax({
                "url": "/Master/SaveGridState",
                "type": "POST",
                data: { Data: JSON.stringify(oData), ListName: InnerPageName },
                "async": false,
                cache: false,
                "dataType": "json",
                "success": function (json) {

                    o = json;
                }
            });
        }, "oLanguage": {
            "sLengthMenu": MsgShowRecordsGridBtn,
            "sEmptyTable": MsgNoDataAvailableInTable,
            "sInfo": MsgShowingNoOfEntries,
            "sInfoEmpty": MsgShowingZeroEntries,
            "sZeroRecords": MsgNoDataAvailableInTable
        },
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $("td.RowNo:first", nRow).html(this.fnSettings()._iDisplayStart + iDisplayIndex + 1);
            return nRow;
        },
        "fnStateLoad": function (oSettings) {
            var o;
            $.ajax({
                "url": "/Master/LoadGridState",
                data: { ListName: InnerPageName },
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
        "fnInitComplete": function (oSettings) {

            SetUDFSelected(oSettings);

        },
        "sAjaxSource": AjaxSourceMehtod,
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            aoData.push({ "name": "ItemGUID", "value": UniqueID });
            aoData.push({ "name": "ItemID", "value": UniqueID });

            if (OrderDetailGUID != null && OrderDetailGUID != undefined && OrderDetailGUID != NaN && OrderDetailGUID.toString().length > 0)
                aoData.push({ "name": "OrderDetailGUID", "value": OrderDetailGUID });
            if (ToolGUID != null && ToolGUID != undefined && ToolGUID != NaN && ToolGUID.toString().length > 0)
                aoData.push({ "name": "ToolGUID", "value": UniqueID });
            else if (TableName == "ToolBinWiseSummaryTable" && UniqueID != undefined && UniqueID != null && UniqueID != "")
            {
                aoData.push({ "name": "ToolGUID", "value": UniqueID });
            }
            if (RequisitionDetailGuid != null && RequisitionDetailGuid != undefined && RequisitionDetailGuid != NaN && $.trim(RequisitionDetailGuid).length > 0)
                aoData.push({ "name": "RequisitionDetailGUID", "value": RequisitionDetailGuid });

            if (WorkOrderGuid != null && WorkOrderGuid != undefined && WorkOrderGuid != NaN && $.trim(WorkOrderGuid).length > 0)
                aoData.push({ "name": "WorkOrderGUID", "value": WorkOrderGuid });

            if (toolCheckoutGUID != null && toolCheckoutGUID != undefined && toolCheckoutGUID != NaN && $.trim(toolCheckoutGUID).length > 0)
                aoData.push({ "name": "ToolCheckoutGUID", "value": toolCheckoutGUID });

            if (typeof (paramsMS) != undefined && paramsMS != undefined && paramsMS.length > 0 && paramsMS != null) {
                for (var u = 0; u < paramsMS.length; u++) {
                    aoData.push({ "name": paramsMS[u].name, "value": paramsMS[u].value });
                }

            }
            if (TableName == "ToolChekinCheckoutTable") {
                var sSearchInner = DoNarrowSearchInner();
                aoData.push({ "name": "sSearchInner", "value": sSearchInner });
            }
            if (TableName == "ToolChekinCheckoutHistoryTable") {
                var sSearchInner = DoNarrowSearchInnerForHistory();
                aoData.push({ "name": "sSearchInner", "value": sSearchInner });
            }

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
            if (oSettings.aaSorting.length != 0)
                aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
            else
                aoData.push({ "name": "SortingField", "value": "0" });
            
            aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
            aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });            
            if (TableName.indexOf("ToolBinWiseSummaryTable") != -1
                || TableName.indexOf("ToolChekinCheckoutTable") != -1
                || TableName.indexOf("ToolChekinCheckoutHistoryTable") != -1) {
                aoData.push({ "name": "parentSearch", "value": TSearchTerm });
            }
            else {
                aoData.push({ "name": "parentSearch", "value": '' });
            }

            oSettings.jqXHR = $.ajax({
                "dataType": 'json',
                "type": "POST",
                "url": sSource,
                cache: false,
                "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
                "data": aoData,
                "success": function (json) {

                    
                    if ($.isFunction(fnGetJson)) {
                        json.aaData = fnGetJson(json);
                    }

                    fnCallback(json);
                },
                beforeSend: function () {
                    $('#' + TableName).removeHighlight();
                    $('.dataTables_scroll').css({ "opacity": 0.2 });
                },
                complete: function () {
                    
                    $('.dataTables_scroll').css({ "opacity": 1 });
                    $(".text-boxPriceFormat").priceFormat({
                        prefix: '',
                        thousandsSeparator: '',
                        centsLimit: parseInt($('#hdCostcentsLimit').val(), 10)
                    });
                    if (gblActionName.toLowerCase() != 'requisitionlist' && TableName != 'ToolChekinCheckoutTable') {
                        $(".text-boxQuantityFormat").priceFormat({
                            prefix: '',
                            thousandsSeparator: '',
                            centsLimit: parseInt($('#hdQuantitycentsLimit').val(), 10)
                        });
                    }
                    if (gblActionName.toLowerCase() == 'toollist' && TableName.indexOf("ToolChekinCheckoutTable") != -1 && $("#global_filter").val() != '') {
                        $('#' + ObjectTable).highlight($("#global_filter").val());
                    }

                    if (gblActionName.toLowerCase() == 'toollist' && TableName.indexOf("ToolChekinCheckoutHistoryTable") != -1 && $("#global_filter").val() != '') {
                        $('#' + ObjectTable).highlight($("#global_filter").val());
                    }

                    if (gblActionName.toLowerCase() == 'receivelist') {
                        $('#' + ObjectTable + '_wrapper .ColVis').css({ 'left': '780px' });
                        UpdateReceiveQty(OrderDetailGUID, UniqueID)
                    }
                    if (RequisitionDetailGuid != null && RequisitionDetailGuid != undefined && RequisitionDetailGuid != NaN && $.trim(RequisitionDetailGuid).length > 0) {
                        $('.innerGrid .dataTables_length').attr('style', 'left:0;top:-35px !important');
                        $('.innerGrid .dataTables_paginate').attr('style', 'left: 145px;top:-25px !important');
                        $('div.ColVis').remove();
                    }

                    if (gblActionName.toLowerCase() == 'toollist' && TableName.indexOf("ToolBinWiseSummaryTable") != -1 && $("#global_filter").val() != '') {
                        OpenSerialChildGrid(ObjectTable);
                    }
                }
            })
        },
        "fnDrawCallback": function (settings) {
            if (typeof (DrillDownBinId) != 'undefined') {
                if (DrillDownBinId != null && DrillDownBinId != undefined && DrillDownBinId.trim() != '') {
                    var imgBinOpen = $('img[id="' + DrillDownBinId.toString() + '"');
                    if (imgBinOpen != null && imgBinOpen != undefined && imgBinOpen.length > 0)
                        imgBinOpen[0].click();
                }
                DrillDownBinId = '';
            }
        },
        "aoColumns": ColumnObject
    }).makeEditable();
}


function OpenSerialChildGrid(parentTable) {
    $('#' + parentTable).find("tbody tr").each(function (index, tr) {
            var vIsLocSerialAvail = $(tr).find('input#hdnIsLocSerialAvail').val();
            if (vIsLocSerialAvail == "Yes") {
                    $(tr).find("img.searchExpand").click();
            }
    });
}

function PrepareMyDynatable(TableName, AjaxSourceMehtod, InnerPageName, ColumnObject, paramstopass, ScrollX, _isLOCSerialAvail, _LOCSearchTerm) {

    oTableGlobalTABLE = $('#' + TableName).dataTable({
        "bJQueryUI": true,
        "bScrollCollapse": true,
        "bAutoWidth": false,
        "sScrollX": ScrollX,
        "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>',
        "aaSorting": [[2, "asc"]],
        "oColReorder": {},
        "bAutoWidth": false,
        "sPaginationType": "full_numbers",
        "bProcessing": true,
        "bStateSave": true,
        "bServerSide": true,
        "fnStateSaveParams": function (oSettings, oData) {
            $.ajax({
                "url": "/Master/SaveGridState",
                "type": "POST",
                data: { Data: JSON.stringify(oData), ListName: InnerPageName },
                "async": false,
                cache: false,
                "dataType": "json",
                "success": function (json) {

                    o = json;
                }
            });
        },"oLanguage": {
            "sLengthMenu": MsgShowRecordsGridBtn,
            "sEmptyTable": MsgNoDataAvailableInTable,
            "sInfo": MsgShowingNoOfEntries,
            "sInfoEmpty": MsgShowingZeroEntries,
            "sZeroRecords": MsgNoDataAvailableInTable
        },
        "fnStateLoad": function (oSettings) {
            var o;
            $.ajax({
                "url": "/Master/LoadGridState",
                data: { ListName: InnerPageName },
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
        "fnInitComplete": function (oSettings) {
        },
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.IsDeleted == true && aData.IsArchived == true) {
                nRow.className = "GridDeleatedArchivedRow";
            }
            else if (aData.IsDeleted == true) {
                nRow.className = "GridDeletedRow";
            }
            else if (aData.IsArchived == true) {
                nRow.className = "GridArchivedRow";
            }

            if (aData.IsDeleted || aData.IsArchived) {
                $(nRow).find("input,select").attr("disabled", "disabled");
            }

            $("td.RowNo:first", nRow).html(this.fnSettings()._iDisplayStart + iDisplayIndex + 1);
            return nRow;
        },

        "sAjaxSource": AjaxSourceMehtod,
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {

            if (typeof (paramstopass) != undefined && paramstopass != undefined) {
                if (paramstopass.length > 0 && paramstopass != null) {
                    for (var u = 0; u < paramstopass.length; u++) {
                        aoData.push({ "name": paramstopass[u].name, "value": paramstopass[u].value });
                    }
                }
            }

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
            if (oSettings.aaSorting.length != 0)
                aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
            else
                aoData.push({ "name": "SortingField", "value": "0" });
            
            aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
            aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });
            if (TableName.indexOf("ToolLocationTable") != -1) {
                aoData.push({ "name": "parentLocSearch", "value": _LOCSearchTerm });
            }
            else {
                aoData.push({ "name": "parentLocSearch", "value": '' });
            }

            oSettings.jqXHR = $.ajax({
                "dataType": 'json',
                "type": "POST",
                "url": sSource,
                cache: false,
                "data": aoData,
                "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
                "success": fnCallback,
                beforeSend: function () {
                    $('#' + TableName).removeHighlight();
                    $('.dataTables_scroll').css({ "opacity": 0.2 });
                },
                complete: function () {
                    
                    $('.dataTables_scroll').css({ "opacity": 1 });
                    SetUDFSelected(oSettings);
                    if (TableName.indexOf("ToolLocationTable") != -1 && $("#global_filter").val() != '') {
                        $('#' + TableName).highlight($("#global_filter").val());
                    }

                    $(".text-boxPriceFormat").priceFormat({
                        prefix: '',
                        thousandsSeparator: '',
                        centsLimit: parseInt($('#hdCostcentsLimit').val(), 10)
                    });
                    $(".text-boxQuantityFormat").priceFormat({
                        prefix: '',
                        thousandsSeparator: '',
                        centsLimit: parseInt($('#hdQuantitycentsLimit').val(), 10)
                    });
                }
            })
        },
        "aoColumns": ColumnObject
    }).makeEditable();
}

function fnGetSelected(oTableLocal) {
    if (oTableLocal != undefined)
        return oTableLocal.$('tr.row_selected');
    else
        return oTableLocal;  
}

function DeleteDynemicTableData(oTableLocal, DeleteURL) {
    var anSelectedLocation = fnGetSelected(oTableLocal);
    oTableInnerGrid = oTableLocal;
    oTableDynemicDeleteURL = DeleteURL;
    var stringIDs = "";
    for (var i = 0; i <= anSelectedLocation.length - 1; i++) {
        stringIDs = stringIDs + anSelectedLocation[i].id + ",";
    }
    if (anSelectedLocation.length !== 0) {
        $('#Inner-Grid-basic-modal-content').modal();
    }
}

function DeleteDynemicTableData_New(oTableLocal, DeleteURL, AfterDeleteCallbackFunction) {
    var anSelectedLocation = fnGetSelected(oTableLocal);
    oTableInnerGrid = oTableLocal;
    oTableDynemicDeleteURL = DeleteURL;
    oAfterDeleteCallbackFunction = AfterDeleteCallbackFunction;
    var stringIDs = "";
    for (var i = 0; i <= anSelectedLocation.length - 1; i++) {
        var guidValue = $(anSelectedLocation[i]).find("input[type='hidden'][name='hdnGUID']").val();

        if (typeof (guidValue) != "undefined" && guidValue != null && guidValue.length > 0) {
            stringIDs = stringIDs + + ",";
        }
        else
        {
            var rawData = oTableLocal.fnGetData(anSelectedLocation[i]);
            if (rawData != null && typeof (rawData) != "undefined" && typeof (rawData.GUID) != "undefined" && rawData.GUID != null && rawData.GUID.length > 0) {
                stringIDs += rawData.GUID + ",";
            }
        }       
    }
    if (anSelectedLocation.length !== 0) {
        PopUpName = "PopUp" + oTableLocal[0].id;
        btnPopUpName = "#btn" + "PopUp" + oTableLocal[0].id;
        PrepareDynemicPopup(PopUpName);
        $('#' + PopUpName).modal();
    }
}

$("#Inner-Grid-basic-modal-content").on("click", "#btnModelYesInnerGrid", function () {
    var anSelectedLocation = fnGetSelected(oTableInnerGrid);
    var stringIDs = "";
    if (anSelectedLocation != undefined) {
        for (var i = 0; i <= anSelectedLocation.length - 1; i++) {
            var guidValue = $(anSelectedLocation[i]).find("input[type='hidden'][name='hdnGUID']").val();

            if (typeof (guidValue) != "undefined" && guidValue != null && guidValue.length > 0) {
                stringIDs = stringIDs + + ",";
            }
            else {
                var rawData = oTableInnerGrid.fnGetData(anSelectedLocation[i]);
                if (rawData != null && typeof (rawData) != "undefined" && typeof (rawData.GUID) != "undefined" && rawData.GUID != null && rawData.GUID.length > 0) {
                    stringIDs += rawData.GUID + ",";
                }
            }
        }
    }
    if (anSelectedLocation != undefined && anSelectedLocation.length !== 0) {
        $.ajax({
            'url': oTableDynemicDeleteURL,
            data: { ids: stringIDs },
            success: function (response) {
                if (response == "ok") {
                    for (var i = 0; i <= anSelectedLocation.length - 1; i++) {
                        oTableInnerGrid.fnDeleteRow(anSelectedLocation[i]);
                    }
                    if (anSelectedLocation.length > 0)
                        $("#spanGlobalMessage").html(MsgRecordDeletedSuccessfully.replace("{0}", anSelected.length));

                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                }
            },
            error: function (response) {
            }
        });
        $.modal.impl.close();
    }
});

$('#InnerGridPageNumber').keydown(function (e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code == 13) {
        $(".go").click();
        return false;
    }
});

$("#InnerGridGobtn").click(function () {
    var pval = $('#InnerGridPageNumber').val();
    if (pval == "" || pval.match(/[^0-9]/)) {
        return;
    }
    if (pval == 0)
        return;
    oTableGlobalTABLE.fnPageChange(Number(pval - 1));
    $('#InnerGridPageNumber').val('');
});

function SetUDFSelected(objParams) {

    $("#" + objParams.sInstance).find("tbody").find("tr").each(function () {
        var binId = $(this).find("input[name='hdnBinID']").val();
        $(this).find("#slctBinName").val(binId);
        var $objUdf1 = $(this).find("#UDF1");
        var $spnUDF1 = $(this).find("#spnUDF1");
        var $objUdf2 = $(this).find("#UDF2");
        var $spnUDF2 = $(this).find("#spnUDF2");
        var $objUdf3 = $(this).find("#UDF3");
        var $spnUDF3 = $(this).find("#spnUDF3");
        var $objUdf4 = $(this).find("#UDF4");
        var $spnUDF4 = $(this).find("#spnUDF4");
        var $objUdf5 = $(this).find("#UDF5");
        var $spnUDF5 = $(this).find("#spnUDF5");

        if ($objUdf1 != undefined && $spnUDF1 != undefined) {
            if ($objUdf1.is("select")) {
                $objUdf1.find("option").filter(function () {
                    return this.text == $spnUDF1.text();
                }).attr('selected', true);
            }
            else if ($objUdf1.is("input[type='text']")) {
                $objUdf1.val($spnUDF1.text());
            }
        }
        if ($objUdf2 != undefined && $spnUDF2 != undefined) {
            if ($objUdf2.is("select")) {
                $objUdf2.find("option").filter(function () {
                    return this.text == $spnUDF2.text();
                }).attr('selected', true);
            }
            else if ($objUdf2.is("input[type='text']")) {
                $objUdf2.val($spnUDF2.text());
            }
        }
        if ($objUdf3 != undefined && $spnUDF3 != undefined) {
            if ($objUdf3.is("select")) {
                $objUdf3.find("option").filter(function () {
                    return this.text == $spnUDF3.text();
                }).attr('selected', true);
            }
            else if ($objUdf3.is("input[type='text']")) {
                $objUdf3.val($spnUDF3.text());
            }
        }
        if ($objUdf4 != undefined && $spnUDF4 != undefined) {
            if ($objUdf4.is("select")) {
                $objUdf4.find("option").filter(function () {
                    return this.text == $spnUDF4.text();
                }).attr('selected', true);
            }
            else if ($objUdf4.is("input[type='text']")) {
                $objUdf4.val($spnUDF4.text());
            }
        }

        if ($objUdf5 != undefined && $spnUDF5 != undefined) {
            if ($objUdf5.is("select")) {
                $objUdf5.find("option").filter(function () {
                    return this.text == $spnUDF5.text();
                }).attr('selected', true);
            }
            else if ($objUdf5.is("input[type='text']")) {
                $objUdf5.val($spnUDF5.text());
            }
        }
    });
}

function PrepareDynemicPopup(PopUpName) {
    var s1 = "";
    s1 = "<div class=\"dialog\" id='" + PopUpName + "' style=\"display: none;\">";
    s1 += "<div class=\"inner\">";
    s1 += "<p class=\"text\">";
    s1 += "" + DeleteConfirmRes + "</p>";
    s1 += "<a href=\"JavaScript:void(0)\" onclick=\"CallDeleteFN()\" class=\"yes\">";
    s1 += "<img src=\"/Content/images/yes.png\" alt=\"Yes\" />";
    s1 += "" + YesRes + "</a><a href=\"javascript:void(0)\" class=\"no\"";
    s1 += "onclick=\"closeModal()\">";
    s1 += "<img src=\"/Content/images/no.png\" alt=\"No\" />" + NoRes + "</a>";
    s1 += "</div>";
    s1 += "</div>";

    $("#DivDynemicPopUpContainer").append(s1);
}

function closeModal() {
    $.modal.impl.close();
}

function CallDeleteFN() {
    var anSelectedLocation = fnGetSelected(oTableInnerGrid);
    var stringIDs = "";
    for (var i = 0; i <= anSelectedLocation.length - 1; i++) {
        var guidValue = $(anSelectedLocation[i]).find("input[type='hidden'][name='hdnGUID']").val();

        if (typeof (guidValue) != "undefined" && guidValue != null && guidValue.length > 0) {
            stringIDs = stringIDs + + ",";
        }
        else {
            var rawData = oTableInnerGrid.fnGetData(anSelectedLocation[i]);
            if (rawData != null && typeof (rawData) != "undefined" && typeof (rawData.GUID) != "undefined" && rawData.GUID != null && rawData.GUID.length > 0) {
                stringIDs += rawData.GUID + ",";
            }
        }
    }
    if (anSelectedLocation.length !== 0) {
        $.ajax({
            'url': oTableDynemicDeleteURL,
            data: { ids: stringIDs },
            success: function (response) {
                if (response == "ok") {
                    for (var i = 0; i <= anSelectedLocation.length - 1; i++) {
                        oTableInnerGrid.fnDeleteRow(anSelectedLocation[i]);
                    }
                    if (anSelectedLocation.length > 0) {
                        $("#spanGlobalMessage").removeClass('WarningIcon errorIcon').addClass('succesIcon');
                        $("#spanGlobalMessage").html(MsgRecordDeletedSuccessfully.replace("{0}", anSelectedLocation.length));
                    }

                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    if (oAfterDeleteCallbackFunction != null && oAfterDeleteCallbackFunction != undefined) {
                        if (oAfterDeleteCallbackFunction = 'UpdateQtyDetailOfParentGrid') {
                            UpdateQtyDetailOfParentGrid();
                        }
                    }
                }
                else {
                    $("#spanGlobalMessage").html(response);
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    oTableInnerGrid.fnDraw();
                }
            },
            error: function (response) {
            }
        });
        $.modal.impl.close();
    }
}