var oTable;
var IsRefreshGrid = false;

var IsDeletePopupOpen = false;
var SelectedHistoryRecordID = '';
var AllowDeletePopup = true;
var objColumns = {};
var listName = 'EVMITareRequestList';

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
            { "mDataProp": "RequestTypes", "sClass": "read_only" },
            {
                "mDataProp": "IsTareStarted", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return obj.aData.IsTareStarted ? 'Yes' : 'No';
                }
            },
            {
                "mDataProp": "TareStartTime", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.TareStartTimeStr; }
            },
            {
                "mDataProp": "IsTareCompleted", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return obj.aData.IsTareCompleted ? 'Yes' : 'No';
                }
            },
            {
                "mDataProp": "TareCompletionTime", "sClass": "read_only",
                "fnRender": function (obj, val) { return obj.aData.TareCompletionTimeStr; }
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

    fillTareNarrowSearchDiv();

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


function fillTareNarrowSearchDiv() {
    $('#divNarrowSearch').html('&nbsp');
    $.get(GetTareNarrwSearchHTMLUrl, function (data) {
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

function CollapseTareNarrowSearch() {
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

                BindDropDownList(response.Data.CreatedByList, 'TareCreatedBy', 'TareCreatedByCollapse', resUserCreatedby);
                BindDropDownList(response.Data.UpdatedByList, 'TareUpdatedBy', 'TareUpdatedByCollapse', resUserUpdatedby);
                
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

    $('#TareDateCFrom').blur(function () {
    }).datepicker({
        dateFormat: RoomDateJSFormat, changeMonth: true,
        changeYear: true
    });
    $('#TareDateCTo').blur(function () {
    }).datepicker({
        changeMonth: true,
        changeYear: true, dateFormat: RoomDateJSFormat
    });
    $('#TareDateUFrom').blur(function () {
    }).datepicker({
        changeMonth: true,
        changeYear: true, dateFormat: RoomDateJSFormat
    });
    $('#TareDateUTo').blur(function () {
    }).datepicker({
        changeMonth: true,
        changeYear: true, dateFormat: RoomDateJSFormat
    });

    $('a.downarrow').click(function (e) {
        e.preventDefault();
        $(this).closest('.accordion').find('.dropcontent').slideToggle();
    });

    $('#TareDateCFrom,#TareDateCTo').change(function () {
        var DateCFromValid = true;//Date.isValid($('#TareDateCFrom').val(),format);
        var DateCToValid = true;//Date.isValid($('#TareDateCTo').val(),format);

        try {
            $.datepicker.parseDate(RoomDateJSFormat, $('#TareDateCFrom').val());
            DateCFromValid = true;
        } catch (e) {
            DateCFromValid = false;
        }

        try {
            $.datepicker.parseDate(RoomDateJSFormat, $('#TareDateCTo').val());
            DateCToValid = true;
        } catch (e) {
            DateCToValid = false;
        }

        if (DateCFromValid && DateCToValid) {
            //$("#txtOrderFilter").val('');
            if (!isFromNarrowSearchClear) {
                DoTareNarrowSearch();
            }
        }
        else {
            if (!DateCFromValid) {
                $('#TareDateCFrom').val('');
            }
            if (!DateCToValid) {
                $('#TareDateCTo').val('');
            }
        }
    });

    $('#TareDateUFrom,#TareDateUTo').change(function () {


        var DateUFromValid = true;// Date.isValid($('#TareDateUFrom').val(),format);
        var DateUToValid = true;//Date.isValid($('#TareDateUTo').val(),format);

        try {
            $.datepicker.parseDate(RoomDateJSFormat, $('#TareDateUFrom').val());
            DateUFromValid = true;
        } catch (e) {
            DateUFromValid = false;
        }

        try {
            $.datepicker.parseDate(RoomDateJSFormat, $('#TareDateUTo').val());
            DateUToValid = true;
        } catch (e) {
            DateUToValid = false;
        }
        if (DateUFromValid && DateUToValid) {
            //$("#txtOrderFilter").val('');
            if (!isFromNarrowSearchClear) {
                DoTareNarrowSearch();
            }

        }
        else {
            if (!DateUFromValid)
                $('#TareDateUFrom').val('');
            if (!DateUToValid)
                $('#TareDateUTo').val('');
        }
    });

    $('#ancTareDateCFrom').click(function () {
        $('#TareDateCFrom').focus();
    });
    $('#ancTareDateCTo').click(function () {
        $('#TareDateCTo').focus();
    });
    $('#ancTareDateUFrom').click(function () {
        $('#TareDateUFrom').focus();
    });
    $('#ancTareDateUTo').click(function () {
        $('#TareDateUTo').focus();
    });

    $('#TareDateCreatedClear').click(function () {
        if ($('#TareDateCFrom').val() != '' || $('#DateCTo').val() != '') {
            $('#TareDateCFrom').val('');
            $('#TareDateCTo').val('');
            //NarrowSearchInGrid('');
            if (!isFromNarrowSearchClear) {
                DoTareNarrowSearch();
            }
        }
    });

    $('#TareDateUpdatedClear').click(function () {
        if ($('#TareDateUFrom').val() != '' || $('#DateUTo').val() != '') {
            $('#TareDateUFrom').val('');
            $('#TareDateUTo').val('');
            //NarrowSearchInGrid('');
            if (!isFromNarrowSearchClear) {
                DoTareNarrowSearch();
            }

        }
    });

    $('#ExpandNarrowSearch').click(function (e) {
        ExpandNarrowSearch();
    });
    $('#CollapseNarrowSearch').click(function (e) {
        CollapseNarrowSearch();
    });


    $('#TareNarroSearchClear').click(function () {
        isFromNarrowSearchClear = true;
        $('#TareDateCFrom').val('');
        $('#TareDateCTo').val('');
        $('#TareDateUFrom').val('');
        $('#TareDateUTo').val('');
        $("#TareCreatedBy").multiselect("uncheckAll");
        $("#TareCreatedByCollapse").html('');
        $("#TareUpdatedBy").multiselect("uncheckAll");
        $("#TareUpdatedByCollapse").html('');

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
        DoTareNarrowSearch();
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
            checkAllText: Check,
            uncheckAllText: UnCheck,
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
                case 'TareCreatedBy':
                    CreatedBySelectedValues = $.map($(this).multiselect("getChecked"), function (input) { return input.value; });
                    break;
                case 'TareUpdatedBy':
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
                DoTareNarrowSearch();
            }
        }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

}

function DoTareNarrowSearch() {

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

    if ($('#TareDateCFrom').val() != '' && $('#TareDateCTo').val() != '') {
        narrowSearchValues += ($('#TareDateCFrom').val()) + "," + ($('#TareDateCTo').val()) + "~";
    }
    else {
        narrowSearchValues += "~";
    }

    if ($('#DateUFrom').val() != '' && $('#TareDateUTo').val() != '') {
        narrowSearchValues += ($('#TareDateUFrom').val()) + "," + ($('#TareDateUTo').val()) + "~";
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

