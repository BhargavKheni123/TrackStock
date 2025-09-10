var oTableGlobalTABLE;
var sImageUrl = "/Content/images/";
var oTableInnerGrid;
var oTableDeleteURL = '';
var IsDeletePopupOpen1 = false;
var AllowDeletePopup1 = true;

function PrepareDataTable(TableName, UniqueID, AjaxSourceMehtod, InnerPageName, ColumnObject, deleteUrl) {
    oTableDeleteURL = deleteUrl;
    var ObjectTable = TableName + UniqueID;
    oTableGlobalTABLE = $('#' + ObjectTable).dataTable({
        "bJQueryUI": true,
        "bScrollCollapse": true,
        "sScrollX": "100%",
        "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>',
        "aaSorting": [[1, "asc"]],
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
        "sAjaxSource": AjaxSourceMehtod,
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            aoData.push({ "name": "ItemID", "value": UniqueID });

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
            aoData.push({ "name": "ParentID", "value": UniqueID });

            oSettings.jqXHR = $.ajax({
                "dataType": 'json',
                "type": "POST",
                "url": sSource,
                cache: false,
                "data": aoData,
                "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
                "success": fnCallback,
                beforeSend: function () {
                    $('.dataTables_scroll').css({ "opacity": 0.2 });
                },
                complete: function () {
                    $('.dataTables_scroll').css({ "opacity": 1 });
                    $("input#hdnListName").val(InnerPageName);
                }
            })
        },
        "aoColumns": ColumnObject
    });
    // });
    //$('.DTTT_container').css('z-index', '-1');
    $('#' + ObjectTable + ' tbody tr').live('tap click', function () {
        $(this).toggleClass('row_selected');
        return false;
    });






    $(document).keyup(function (e) {
        if (AllowDeletePopup1 == true) {
            var code = (e.keyCode ? e.keyCode : e.which);
            if (code == 46) {
                //closeModal();
                $('#deleteRows1').click();
            }
            if (code == 89 && IsDeletePopupOpen1 == true) {
                $("#btnQLModelYes").click();
                IsDeletePopupOpen1 = false;
            }
            else if (code == 78 && IsDeletePopupOpen1 == true) {
                QLcloseModal();
                IsDeletePopupOpen1 = false;
            }
        }
    });




    $('#PageNumber1').keydown(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) {
            $("#Gobtn1").click();
            return false;
        }
    });

    $("#Gobtn1").click(function () {
        var pval = $('#PageNumber1').val();
        if (pval == "" || pval.match(/[^0-9]/)) {
            return;
        }
        if (pval == 0)
            return;

        oTableGlobalTABLE.fnPageChange(Number(pval - 1));
        $('#PageNumber1').val('');
    });



}


function fnGetSelected1(oTableLocal) {
    return oTableLocal.$('tr.row_selected');
}


function QLcloseModal() {
    $.modal.impl.close();
}

function fnFilterGlobal1() {
    //set filter only if more than 2 characters are pressed
    if (typeof $("#QLItem_filter") != 'undefined' && ($("#QLItem_filter").val().length > 2 || $("#QLItem_filter").val().length == 0)) {
        var searchtext = $("#QLItem_filter").val().replace(/'/g, "''");
        oTableGlobalTABLE.fnFilter(
                            searchtext,
                            null,
                            null,
                            null
                        );
    }
    else {
        $('#' + ObjectTable + ' td').removeHighlight();
        $('#' + ObjectTable + ' td').highlight($("#QLItem_filter").val());
    }
}


$("#QLItem_filter").keyup(function (e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code == 13) {
    }
    else {
        fnFilterGlobal1();
    }
});


$("#QLItem_filter").keydown(function (e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code == 13) {
        var searchtext = $("#QLItem_filter").val().replace(/'/g, "''");
        oTableGlobalTABLE.fnFilter(
                            searchtext,
                            null,
                            null,
                            null
                        );
        return false;
    }
});


$("#clear_QLItem_filter").click(funClearFilter1);

function funClearFilter1() {
    //Check length first        
    if ($("#QLItem_filter").val().length > 0) {
        $("#QLItem_filter").val('');
        oTableGlobalTABLE.fnFilter(
                            $("#QLItem_filter").val(),
                            null,
                            null,
                            null
                        );
    }
    $("#QLItem_filter").focus();
    return false;
}
