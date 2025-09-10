var oTable;
var IsRefreshGrid = false;

var IsDeletePopupOpen = false;
var SelectedHistoryRecordID = '';
var AllowDeletePopup = true;
var objColumns = {};
var listName = 'EVMICalibrateRequestList';

var isTabClick = false;
var _IsArchived = false;
var _IsDeleted = false;


function callbackhistory() {
    window.location.hash = '#list';

}

function SetTabView() {
    var tabname = window.location.hash.toLowerCase();

    switch (tabname) {
        case "#list":
            $("#tab5").click();
            break
    }
}

$(document).ready(function () {
    objColumns = GetGridHeaderColumnsObject('myDataTable');
    LoadTabs();
    var gaiSelected = [];
    oTable = $('#myDataTable').dataTable({
        "bJQueryUI": true,
        "bScrollCollapse": true,
        "sScrollX": "200%",
        "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>T',
        "oColVis": {},
        "aaSorting": [[1, "desc"]],
        "oColReorder": {},
        "sPaginationType": "full_numbers",
        "bProcessing": true,
        "bStateSave": true,
        "oLanguage": oLanguage,
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

            if ($(aData.IsDeleted).text() == 'Yes' && $(aData.IsArchived).text() == 'Yes') {
                $(nRow).css('background-color', '#B9BCBF');
            }
            else if ($(aData.IsDeleted).text() == 'Yes') {
                $(nRow).css('background-color', '#FFCCCC');
            }
            else if ($(aData.IsArchived).text() == 'Yes') {
                $(nRow).css('background-color', '#CCFFCC');
            }
            $("td.RowNo:first", nRow).html(this.fnSettings()._iDisplayStart + iDisplayIndex + 1);
            return nRow;
        },
        "fnStateSaveParams": function (oSettings, oData) {
            oData.oSearch.sSearch = "";
            $.ajax({
                "url": saveGridStateUrl,
                "type": "POST",
                data: { Data: JSON.stringify(oData), ListName: listName },
                "async": false,
                cache: false,
                "dataType": "json",
                "success": function (json) {
                    if (json.jsonData != '') {
                        o = json;
                    }
                }
            });
        },
        "fnStateLoad": function (oSettings) {
            var o;
            $.ajax({
                "url": loadGridStateUrl,
                "type": "POST",
                data: { ListName: listName },
                "async": false,
                cache: false,
                "dataType": "json",
                "success": function (json) {
                    if (json.jsonData != '') {
                        o = JSON.parse(json.jsonData);
                    }
                }
            });

            return o;
        },
        "bServerSide": true,
        "sAjaxSource": ajaxUrl,
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
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
                var sortValue = "";
                for (var i = 0; i <= oSettings.aaSorting.length - 1; i++) {
                    if (sortValue.length > 0) {
                        sortValue += ", "
                    }
                    sortValue += arrCols[oSettings.aaSorting[i][0]] + ' ' + oSettings.aaSorting[i][1];

                }
                aoData.push({ "name": "SortingField", "value": sortValue });
            }
            else {
                aoData.push({ "name": "SortingField", "value": "0" });
            }

            aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
            aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });


            oSettings.jqXHR = $.ajax({
                "dataType": 'json',
                "type": "POST",
                cache: false,
                "url": sSource,
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
                    if ($("#global_filter").val().length > 0) {
                        if ($('#myDataTable').dataTable().fnGetData().length <= 0) {
                        }
                    }

                }
            })
        },
        "fnInitComplete": function () {
            $('.ColVis').detach().appendTo(".setting-arrow");
        },
        "aoColumns": [
            { "mDataProp": null, "sClass": "read_only center NotHide RowNo", "sWidth": '100px', "bSortable": false, "sDefaultContent": '' },
            { "mDataProp": "ID", "sClass": "read_only" },
            { "mDataProp": "ItemNumber", "sClass": "read_only" },
            { "mDataProp": "BinNumber", "sClass": "read_only" },
            { "mDataProp": "ScaleID", "sClass": "read_only" },
            { "mDataProp": "ChannelID", "sClass": "read_only" },
            { "mDataProp": "ComPortName", "sClass": "read_only" },
            { "mDataProp": "RequestTypeName", "sClass": "read_only" },
            { "mDataProp": "CalibrationWeight", "sClass": "read_only" },
            {
                "mDataProp": "IsStep1Started", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return obj.aData.IsStep1Started ? 'Yes' : 'No';
                }
            },
            {
                "mDataProp": "Step1StartTime", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.Step1StartTimeStr; }
            },
            {
                "mDataProp": "IsStep1Completed", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return obj.aData.IsStep1Completed ? 'Yes' : 'No';
                }
            },
            {
                "mDataProp": "Step1CompletionTime", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.Step1CompletionTimeStr; }
            },
            {
                "mDataProp": "IsStep2Started", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return obj.aData.IsStep2Started ? 'Yes' : 'No';
                }
            },
            {
                "mDataProp": "Step2StartTime", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.Step2StartTimeStr; }
            },
            {
                "mDataProp": "IsStep2Completed", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return obj.aData.IsStep2Completed ? 'Yes' : 'No';
                }
            },
            {
                "mDataProp": "Step2CompletionTime", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.Step2CompletionTimeStr; }
            },
            {
                "mDataProp": "IsStep3Started", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return obj.aData.IsStep3Started ? 'Yes' : 'No';
                }
            },
            {
                "mDataProp": "Step3StartTime", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.Step3StartTimeStr; }
            },
            {
                "mDataProp": "IsStep3Completed", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return obj.aData.IsStep3Completed ? 'Yes' : 'No';
                }
            },
            {
                "mDataProp": "Step3CompletionTime", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.Step3CompletionTimeStr; }
            },
            { "mDataProp": "ErrorDescription", "sClass": "read_only" },
            { "mDataProp": "CreatedByName", "sClass": "read_only" },
            { "mDataProp": "UpdatedByName", "sClass": "read_only" },
            {
                "mDataProp": "Created", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.CreatedStr; }
            },
            {
                "mDataProp": "Updated", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    return obj.aData.UpdatedStr;
                }
            }
        ]
    })


    //This is Most important
    jQuery("#btnDiv").click(function (e) {
        var offset = $(this).offset();
        var leftpx = (parseInt(e.clientX) + parseInt($(this).css("width").toString().replace("px", "")) + parseInt(15)) + 'px';
        var toppx = (parseInt(e.clientY) + parseInt(5)) + 'px';
        jQuery('#myDataTable_wrapper div.ColVis .ColVis_Button').click();
        jQuery('.ColVis_collection').css("left", leftpx);
        jQuery('.ColVis_collection').css("top", toppx);
        e.preventDefault();

    });

    //HIDE PRINT CONTAINER
    $('.DTTT_container').css('z-index', '-1');

    $('#myDataTable').on('tap click', 'a[id^="aEditLink"]', function () {
        var tr = $(this).parent().parent();
        $("#myDataTable").find("tbody tr").removeClass("row_selected");
        $(tr).addClass('row_selected');

    });

    fillCalibrateNarrowSearchDiv();

});


/* HISTORY related data deleated and archived START */
function fnGetSelected(oTableLocal) {
    return oTableLocal.$('tr.row_selected');
}
function HistoryTabClick() {
    GetHistoryData();
}

function GetHistoryData() {

}

var CreatedBySelectedValues = '';
var UpdatedBySelectedValues = '';


function fillCalibrateNarrowSearchDiv() {
    $('#divNarrowSearch').html('&nbsp');
    $.get(GetCalibrateNarrwSearchHTMLUrl, function (data) {
        $('#divNarrowSearch').html(data);
        BindNarrowSearchEvents();

    });
}


//function clearSearchFilterIfNotInFocus() {
//    if ($(document.activeElement).attr('id') != 'txtOrderFilter')
//        $("#txtOrderFilter").val('');
//}

function ExpandNarrowSearch() {
    var w = $('.IteamBlock').css("width");
    $('.IteamBlock').show();
    $('.IteamBlock').stop().animate({
        width: "99.5%"
    }, 0, function () {
        $('.userContent').css({ "width": "80.5%", "margin": "0" });
        $('#myDataTable_length').css({ "left": "0px" });
        $('#myDataTable_paginate').css({ "left": "145px" });
        $('.leftopenContent').css({ "display": "none" });
        setCookie('NarrowSearchState', 'Collapsed');
    });
    $('#divNarrowSearch').css('width', '18%');
}

function CollapseCalibrateNarrowSearch() {
    $('.IteamBlock').stop().animate({
        width: "0%"
    }, 0, function () {
        $('.IteamBlock').hide();
        $('.userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
        var Left = $('.viewBlock').css("width");
        $('#myDataTable_length').css({ "left": Left });
        var LeftW = 145 + parseInt(Left);
        $('#myDataTable_paginate').css({ "left": LeftW + 'px' });
        oTable.fnAdjustColumnSizing();
        $('.leftopenContent').css({ "display": "" });
        setCookie('NarrowSearchState', 'Expanded');
    });
    $('#divNarrowSearch').css('width', '0%');
}

function FillNarrowSearchData() {
    $.ajax({
        url: GetNarrowSearchDataUrl,
        data: { 'IsDeleted': _IsDeleted, 'IsArchived': _IsArchived },
        dataType: 'json',
        success: function (response) {
            if (response.Success && response.Data != null) {

                BindDropDownList(response.Data.CreatedByList, 'CalibrateCreatedBy', 'CalibrateCreatedByCollapse', resUserCreatedby);
                BindDropDownList(response.Data.UpdatedByList, 'CalibrateUpdatedBy', 'CalibrateUpdatedByCollapse', resUserUpdatedby);

            }
        },
        error: function (xhr) {
            //alert(xhr);
            console.log(xhr);
        }
    });
}


function BindNarrowSearchEvents() {

    var NarrowSearchState = getCookie('NarrowSearchState');
    if (NarrowSearchState == 'Expanded') {
        CollapseNarrowSearch();
    }
    else {
        ExpandNarrowSearch();
    }

    $('.IteamBlock').css('width', '99.5%');

    FillNarrowSearchData();

    $('#CalibrateDateCFrom').blur(function () {
    }).datepicker({
        dateFormat: RoomDateJSFormat, changeMonth: true,
        changeYear: true
    });
    $('#CalibrateDateCTo').blur(function () {
    }).datepicker({
        changeMonth: true,
        changeYear: true, dateFormat: RoomDateJSFormat
    });
    $('#CalibrateDateUFrom').blur(function () {
    }).datepicker({
        changeMonth: true,
        changeYear: true, dateFormat: RoomDateJSFormat
    });
    $('#CalibrateDateUTo').blur(function () {
    }).datepicker({
        changeMonth: true,
        changeYear: true, dateFormat: RoomDateJSFormat
    });

    $('a.downarrow').click(function (e) {
        e.preventDefault();
        $(this).closest('.accordion').find('.dropcontent').slideToggle();
    });

    $('#CalibrateDateCFrom,#CalibrateDateCTo').change(function () {
        var DateCFromValid = true;//Date.isValid($('#CalibrateDateCFrom').val(),format);
        var DateCToValid = true;//Date.isValid($('#CalibrateDateCTo').val(),format);

        try {
            $.datepicker.parseDate(RoomDateJSFormat, $('#CalibrateDateCFrom').val());
            DateCFromValid = true;
        } catch (e) {
            DateCFromValid = false;
        }

        try {
            $.datepicker.parseDate(RoomDateJSFormat, $('#CalibrateDateCTo').val());
            DateCToValid = true;
        } catch (e) {
            DateCToValid = false;
        }

        if (DateCFromValid && DateCToValid) {
            //$("#txtOrderFilter").val('');
            if (!isFromNarrowSearchClear) {
                DoCalibrateNarrowSearch();
            }
        }
        else {
            if (!DateCFromValid) {
                $('#CalibrateDateCFrom').val('');
            }
            if (!DateCToValid) {
                $('#CalibrateDateCTo').val('');
            }
        }
    });

    $('#CalibrateDateUFrom,#CalibrateDateUTo').change(function () {


        var DateUFromValid = true;// Date.isValid($('#CalibrateDateUFrom').val(),format);
        var DateUToValid = true;//Date.isValid($('#CalibrateDateUTo').val(),format);

        try {
            $.datepicker.parseDate(RoomDateJSFormat, $('#CalibrateDateUFrom').val());
            DateUFromValid = true;
        } catch (e) {
            DateUFromValid = false;
        }

        try {
            $.datepicker.parseDate(RoomDateJSFormat, $('#CalibrateDateUTo').val());
            DateUToValid = true;
        } catch (e) {
            DateUToValid = false;
        }
        if (DateUFromValid && DateUToValid) {
            //$("#txtOrderFilter").val('');
            if (!isFromNarrowSearchClear) {
                DoCalibrateNarrowSearch();
            }

        }
        else {
            if (!DateUFromValid)
                $('#CalibrateDateUFrom').val('');
            if (!DateUToValid)
                $('#CalibrateDateUTo').val('');
        }
    });

    $('#ancCalibrateDateCFrom').click(function () {
        $('#CalibrateDateCFrom').focus();
    });
    $('#ancCalibrateDateCTo').click(function () {
        $('#CalibrateDateCTo').focus();
    });
    $('#ancCalibrateDateUFrom').click(function () {
        $('#CalibrateDateUFrom').focus();
    });
    $('#ancCalibrateDateUTo').click(function () {
        $('#CalibrateDateUTo').focus();
    });

    $('#CalibrateDateCreatedClear').click(function () {
        if ($('#CalibrateDateCFrom').val() != '' || $('#DateCTo').val() != '') {
            $('#CalibrateDateCFrom').val('');
            $('#CalibrateDateCTo').val('');
            //NarrowSearchInGrid('');
            if (!isFromNarrowSearchClear) {
                DoCalibrateNarrowSearch();
            }
        }
    });

    $('#CalibrateDateUpdatedClear').click(function () {
        if ($('#CalibrateDateUFrom').val() != '' || $('#DateUTo').val() != '') {
            $('#CalibrateDateUFrom').val('');
            $('#CalibrateDateUTo').val('');
            //NarrowSearchInGrid('');
            if (!isFromNarrowSearchClear) {
                DoCalibrateNarrowSearch();
            }

        }
    });

    $('#ExpandNarrowSearch').click(function (e) {
        ExpandNarrowSearch();
    });
    $('#CollapseNarrowSearch').click(function (e) {
        CollapseNarrowSearch();
    });


    $('#CalibrateNarroSearchClear').click(function () {
        isFromNarrowSearchClear = true;
        $('#CalibrateDateCFrom').val('');
        $('#CalibrateDateCTo').val('');
        $('#CalibrateDateUFrom').val('');
        $('#CalibrateDateUTo').val('');
        $("#CalibrateCreatedBy").multiselect("uncheckAll");
        $("#CalibrateCreatedByCollapse").html('');
        $("#CalibrateUpdatedBy").multiselect("uncheckAll");
        $("#CalibrateUpdatedByCollapse").html('');

        //$("#OrderStatus").multiselect("uncheckAll");
        //$("#OrderStatusCollapse").html('');
        //$("#OrderRequiredDate").multiselect("uncheckAll");
        //$("#OrderRequiredDateCollapse").html('');
        //$("#OrderSupplier").multiselect("uncheckAll");
        //$("#OrderSupplierCollapse").html('');
        if ($('#global_filter').val() != '') $('#global_filter').val('');
        //$("select[name='UDFS']").each(function (index) {
        //    $(this).multiselect("uncheckAll");
        //});

        //$("div[name='CUDFS']").each(function (index) {
        //    $(this).html('');
        //});
        $('#myDataTable tbody tr').removeClass('row_selected');
        ShowHideChangeLog();
        isFromNarrowSearchClear = false;
        $('input[type="search"]').val('').trigger('keyup');
        DoCalibrateNarrowSearch();
    });

}

function GetOptions(arrData, isUDF) {
    var options;
    if (isUDF) {
        $.each(arrData, function (i) {
            options += '<option value="' + arrData[i].Text + '">' + arrData[i].Text + ' (' + arrData[i].Count + ')' + '</option>';
        });
    }
    else {
        $.each(arrData, function (i) {
            options += '<option value="' + arrData[i].ID + '">' + arrData[i].Text + ' (' + arrData[i].Count + ')' + '</option>';
        });

    }
    return options;
}

function BindDropDownList(arrData, ddlName, ddlCollapseName, displayName) {
    $("#" + ddlName).empty();
    $("#" + ddlName).multiselect('destroy');
    $("#" + ddlName).multiselectfilter('destroy');
    $("#" + ddlName).append(GetOptions(arrData, (ddlName.indexOf('QLUDF') != -1)));
    $("#" + ddlName).multiselect(
        {
            noneSelectedText: displayName, selectedList: 5,
            selectedText: function (numChecked, numTotal, checkedItems) {
                return displayName + ' ' + numChecked + ' ' + selected;
            }
        },
        {
            checkAll: function (ui) {
                $("#" + ddlCollapseName).html('');
                for (var i = 0; i <= ui.target.length - 1; i++) {
                    if ($("#" + ddlCollapseName).text().indexOf(ui.target[i].text) == -1) {
                        $("#" + ddlCollapseName).append("<span>" + ui.target[i].text + "</span>");
                    }
                }
                $("#" + ddlCollapseName).show();
            }
        }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            if (ui.checked) {
                if ($("#" + ddlCollapseName).text().indexOf(ui.text) == -1) {
                    $("#" + ddlCollapseName).append("<span>" + ui.text + "</span>");
                }
            }
            else {
                if (ui.checked == undefined) {
                    $("#" + ddlCollapseName).html('');
                }
                else if (!ui.checked) {
                    var text = $("#" + ddlCollapseName).html();
                    text = text.replace("<span>" + ui.text + "</span>", '');
                    $("#" + ddlCollapseName).html(text);
                }
                else {
                    $("#" + ddlCollapseName).html('');
                }
            }
            switch ($(this).attr('id')) {
                case 'CalibrateCreatedBy':
                    CreatedBySelectedValues = $.map($(this).multiselect("getChecked"), function (input) { return input.value; });
                    break;
                case 'CalibrateUpdatedBy':
                    UpdatedBySelectedValues = $.map($(this).multiselect("getChecked"), function (input) { return input.value; });
                    break;
            }

            if ($("#" + ddlCollapseName).text().trim() != '') {
                $("#" + ddlCollapseName).show();
            }
            else {
                $("#" + ddlCollapseName).hide();
            }

            if ($("#" + ddlCollapseName).find('span').length <= 2) {
                $("#" + ddlCollapseName).scrollTop(0).height(50);
            }
            else {
                $("#" + ddlCollapseName).scrollTop(0).height(100);
            }
            if (!isFromNarrowSearchClear) {
                DoCalibrateNarrowSearch();
            }
        }).multiselectfilter();

}

function DoCalibrateNarrowSearch() {

    //   clearSearchFilterIfNotInFocus();

    var narrowSearchValues = '';

    if (CreatedBySelectedValues == undefined || CreatedBySelectedValues.length <= 0) {
        CreatedBySelectedValues = '';
    }

    if (UpdatedBySelectedValues == undefined || UpdatedBySelectedValues.length <= 0) {
        UpdatedBySelectedValues = '';
    }


    narrowSearchValues += CreatedBySelectedValues + "~";
    narrowSearchValues += UpdatedBySelectedValues + "~";

    if ($('#CalibrateDateCFrom').val() != '' && $('#CalibrateDateCTo').val() != '') {
        narrowSearchValues += ($('#CalibrateDateCFrom').val()) + "," + ($('#CalibrateDateCTo').val()) + "~";
    }
    else {
        narrowSearchValues += "~";
    }

    if ($('#DateUFrom').val() != '' && $('#CalibrateDateUTo').val() != '') {
        narrowSearchValues += ($('#CalibrateDateUFrom').val()) + "," + ($('#CalibrateDateUTo').val()) + "~";
    }
    else {
        narrowSearchValues += "~";
    }


    //  if (narrowSearchValues.replace(/~/g, '') == '') {
    //      narrowSearch = '';
    //   }
    //   else {
    var searchtext = $("#global_filter").val().replace(/'/g, "''");
    narrowSearch = "[###]" + narrowSearchValues + "[###]" + searchtext;
    //   }

    //fnFilterOrderList(narrowSearch);
    $('#myDataTable').dataTable().fnFilter(narrowSearch, null, null, null)
}

