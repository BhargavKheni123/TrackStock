var gblArrPreReceiveObj;
var gblArrReceiveInfoToSave;

// Events
$(document).ready(function () {
    $('#divPreRecieveInfo').dialog({
        autoOpen: false,
        //modal: true,
        //draggable: true,
        resizable: true,
        width: '75%',
        height: 560,
        create: function (event, ui) {

        },
        title: "Pre Recive Info",
        open: function () {
            $('#divPreRecieveInfo').empty();
            var objToFillPreReceive = $(this).data("MakePreReceive");
            
            gblArrPreReceiveObj = gblArrPreReceiveObj.filter(function (el) {
                return (el.OrderDetailGUID !== objToFillPreReceive.OrderDetailGUID);
            });
            OpenPreReceiveInfoPage(objToFillPreReceive);
        },
        close: function () {
            //closeOrdReceiveInfoModel();
            closeOrdReceiveInfoModel();
            $('#divPreRecieveInfo').empty();
            OpenPreReceiveInfoDialog();
        }
    });
});


$('#btnReceiveAllNew').on('click', function () {
    $(this).attr('disabled', 'disabled');
    $('#DivLoading').show();

    gblArrPreReceiveObj = new Array();
    gblArrReceiveInfoToSave = new Array();

    // var ItemSelected = fnGetSelected(oTable);
    
    var ItemSelected;;
    if (gblActionName.toLowerCase() == "receivelist") {
        ItemSelected = fnGetSelected(oTable);
    }
    else if (gblActionName.toLowerCase() == "orderlist" || gblActionName.toLowerCase() == "toolassetorderlist") {
        ItemSelected = fnGetSelected($('#RecieveOrderLineItem' + OrdID).dataTable());
    }
    else if (gblActionName.toLowerCase() == "toolassetorderlist") {
        ItemSelected = fnGetSelected($('#RecieveOrderLineItem' + OrdID).dataTable());
    }


    if (!isNaN(ItemSelected.length) && ItemSelected.length > 0) {
        $('#OrdReceiveProcessing').modal();
        $('#OrdReceiveProcessing').parent().parent().find(".modalCloseImg").css('display', 'none');
        for (var i = 0; i < ItemSelected.length; i++) {
            var isClosedItem = false;
            if (gblActionName.toLowerCase() == "receivelist" && $.trim($(ItemSelected[i]).find('#spnIsCloseLineItem').text().toLowerCase()) == 'true') {
                isClosedItem = true;
            }
            else if ((gblActionName.toLowerCase() == "orderlist" || gblActionName.toLowerCase() == "toolassetorderlist") && $.trim($(ItemSelected[i]).find('#hdnIsCloseItem').val()) == 'True') {
                isClosedItem = true;
            }
            else if (gblActionName.toLowerCase() == "toolassetorderlist" && $.trim($(ItemSelected[i]).find('#hdnIsCloseItem').val()) == 'True') {
                isClosedItem = true;
            }
            if (!isClosedItem) {
                var tr = ItemSelected[i];
                var rowPosition;
                var aData;
                var toFillRecieveInfo;
                if (gblActionName.toLowerCase() == "receivelist") {
                    rowPosition = oTable.fnGetPosition(tr);
                    aData = oTable.fnGetData(rowPosition);
                    toFillRecieveInfo = GetToFillPreReceive(tr, aData);
                }
                else if (gblActionName.toLowerCase() == "orderlist" || gblActionName.toLowerCase() == "toolassetorderlist") {
                    rowPosition = $('#RecieveOrderLineItem' + OrdID).dataTable().fnGetPosition(tr);
                    aData = $('#RecieveOrderLineItem' + OrdID).dataTable().fnGetData(rowPosition);
                    toFillRecieveInfo = GetToFillPreReceiveFroOrder(tr, aData);
                }
                else if (gblActionName.toLowerCase() == "toolassetorderlist") {
                    rowPosition = $('#RecieveOrderLineItem' + OrdID).dataTable().fnGetPosition(tr);
                    aData = $('#RecieveOrderLineItem' + OrdID).dataTable().fnGetData(rowPosition);
                    toFillRecieveInfo = GetToFillPreReceiveFroToolAssetOrder(tr, aData);
                }
                gblArrPreReceiveObj.push(toFillRecieveInfo);
            }
            else {
                $(ItemSelected[i]).removeClass('row_selected');
            }
        }
        OpenPreReceiveInfoDialog();
    }
    else {
        $(this).removeAttr('disabled');
        $('#DivLoading').hide();
        alert(MsgSelectRowToReceive);
    }

});

// due to jira WI-4108 off click removes bcoz plus icon not working in order header


//$('#btnAddNewRowPreRcv').die('click').live('click', function (e) {
//$(document).off('click').on('click', 'input#btnAddNewRowPreRcv', function (e) {
$(document).on('click', 'input#btnAddNewRowPreRcv', function (e) {
    var lsttr = $('#tblPreReceive tbody tr:last');
    $('#tblPreReceive tbody tr:last').after(lsttr[0].outerHTML);
    $(lsttr).find('#btnAddNewRowPreRcv').parent().html('');
});

//$(document).off('click').on('click', 'input#btnAddRowLotExpPreRcv', function (e) {
$(document).on('click', 'input#btnAddRowLotExpPreRcv', function (e) {

    var lsttr = $('#tblPreReceiveHeader tbody tr:last');

    $('#tblPreReceiveHeader tbody tr:last').after(lsttr[0].outerHTML);
    $(lsttr).find('#btnAddRowLotExpPreRcv').parent().html('');
    UDFfillEditableOptionsForGrid();

    var lsttrNew = $('#tblPreReceiveHeader tbody tr:last');
    var ind = parseInt($(this).parent().parent().parent().children().index($(this).parent().parent())) + 2;
    var txtdtid = $(lsttrNew).find('input.txtReceivedDate').attr('id') + ind;
    var txtexpdtid = $(lsttrNew).find('input.txtExpiration').attr('id') + ind;

    $(lsttrNew).find('input.txtReceivedDate').attr('id', txtdtid);
    $(lsttrNew).find('input.txtReceivedDate').attr('name', txtdtid);

    $(lsttrNew).find('input.txtExpiration').attr('id', txtexpdtid);
    $(lsttrNew).find('input.txtExpiration').attr('name', txtexpdtid);

    RegisterDatePicker();

});

function RegisterDatePicker() {
    $('#tblPreReceiveHeader tbody tr').each(function (i) {
        $(this).find('input.dateTextbox').removeClass('hasDatepicker');
        $(this).find('input.dateTextbox').datepicker({
            dateFormat: RoomDateJSFormat, showButtonPanel: true,
            clearText: 'Clear', onClose: function () { this.focus(); }
        });
    });
}

//$('#divPreRecieveInfo').on('click', "input.dateTextbox", function (e) {
//    $(this).datepicker({
//        dateFormat: RoomDateJSFormat, showButtonPanel: true,
//        clearText: 'Clear', onClose: function () { this.focus(); }
//    });
//});

// Functions
function OpenPreReceiveInfoDialog() {
    $('#DivLoading').show();
    $('#OrdReceiveProcessing').modal();
     
    $('#OrdReceiveProcessing').parent().parent().find(".modalCloseImg").css('display', 'none');
    if (gblArrPreReceiveObj != null && gblArrPreReceiveObj.length > 0) {
        setTimeout(function () { $('#divPreRecieveInfo').data({ 'MakePreReceive': gblArrPreReceiveObj[0] }).dialog('open') }, 100);
    }
    else if (gblArrReceiveInfoToSave != null && gblArrReceiveInfoToSave.length > 0 && gblArrPreReceiveObj.length <= 0) {
        setTimeout(function () { SaveReceiveData(gblArrReceiveInfoToSave) }, 100);
    }
    else {
        gblArrPreReceiveObj = null;
        gblArrReceiveInfoToSave = null;
        $('#btnReceiveAllNew').removeAttr('disabled');
        $('#DivLoading').hide();
        closeOrdReceiveInfoModel();
    }
}

function MakeReceiveArray(obj) {
    var newReceiveArray = new Array();
    var mainArray = new Array();
    for (var i = 0; i < obj.length; i++) {
        if (newReceiveArray.length < 250) {
            newReceiveArray.push(obj[i]);
        }
        else {
            mainArray.push(newReceiveArray);
            newReceiveArray = null;
            newReceiveArray = new Array();
            newReceiveArray.push(obj[i]);
        }
    }

    if (newReceiveArray.length > 0) {
        mainArray.push(newReceiveArray);
    }

    SaveLargeReceiveData(mainArray)
}

function SaveLargeReceiveData(objReceiveArray) {
    var returnResponse = new Array();
    var errorMsgOuter = '';
    for (var k = 0; k < objReceiveArray.length; k++) {
        
        $.ajax({
            "url": "/ReceiveToolAsset/SaveReceiveInformationTool",
            "data": JSON.stringify(objReceiveArray[k]),
            "type": 'POST',
            "async": false,
            "cache": false,
            "dataType": "json",
            "contentType": "application/json",
            "success": function (result) {

                var errorMsg = '';
                var ItemSelected;
                if (gblActionName.toLowerCase() == "receivelist") {
                    ItemSelected = fnGetSelected(oTable);
                }
                else if (gblActionName.toLowerCase() == "orderlist" || gblActionName.toLowerCase() == "toolassetorderlist" ) {
                    ItemSelected = fnGetSelected($('#RecieveOrderLineItem' + OrdID).dataTable());
                    if (!isNaN(parseInt(result.OrderStatus)) && parseInt(result.OrderStatus) > 0) {
                        $('#ddlOrderStatus').val(result.OrderStatus);
                    }
                    SetReplenishRedCount();
                }
                if (!isNaN(ItemSelected.length) && ItemSelected.length > 0) {
                    for (var i = 0; i < ItemSelected.length; i++) {
                        var tr = ItemSelected[i];
                        var orddtlGuid = $(tr).find('#spnOrderDetailGUID').text();
                        var objArrElement = $.grep(objReceiveArray[k], function (el) {
                            return ($.trim(el.OrderDetailGUID.toString()) === $.trim(orddtlGuid.toString()));
                        });
                        if (objArrElement !== undefined && objArrElement != null && objArrElement.length > 0 && ($.trim(objArrElement[0].OrderDetailGUID.toString()) === $.trim(orddtlGuid.toString()))) {
                            $(tr).removeClass('row_selected');
                            if (result.Errors != null && result.Errors.length > 0) {
                                var objReceiveErrorResponse = $.grep(result.Errors, function (el) {
                                    return ($.trim(el.OrderDetailGuid.toString()) === $.trim(orddtlGuid.toString()));
                                });

                                if (objReceiveErrorResponse !== undefined && objReceiveErrorResponse != null && objReceiveErrorResponse.length > 0 && objReceiveErrorResponse[0].ErrorMassage.length > 0) {
                                    $(tr).css('background-color', 'Red');
                                    errorMsgOuter = errorMsgOuter + "<br/>"
                                    errorMsgOuter = errorMsgOuter + "<span style='color:red'><b>" + objReceiveErrorResponse[0].ErrorTitle + '</b>: ' + objReceiveErrorResponse[0].ErrorMassage + "</span>"
                                }
                                else {
                                    objReceiveErrorResponse = $.grep(result.Errors, function (el) {
                                        return ($.trim(el.OrderDetailGuid.toString()) === "00000000-0000-0000-0000-000000000000" && el.ErrorMassage.length > 0);
                                    });

                                    if (objReceiveErrorResponse !== undefined && objReceiveErrorResponse != null && objReceiveErrorResponse.length > 0 && objReceiveErrorResponse[0].ErrorMassage.length > 0) {
                                        $(tr).css('background-color', 'Red');
                                        errorMsgOuter = errorMsgOuter + "<br/>"
                                        errorMsgOuter = errorMsgOuter + "<span style='color:red'><b>" + objReceiveErrorResponse[0].ErrorTitle + '</b>: ' + objReceiveErrorResponse[0].ErrorMassage + "</span>"
                                    }
                                    else {
                                        $(tr).css('background-color', 'Green');
                                    }
                                }
                            }
                            else {
                                $(tr).css('background-color', 'Green');
                            }
                        }
                    }
                }
            },
            "error": function (xhr) {
                errorMsgOuter = errorMsgOuter + "<br />" + MsgErrorInProcess;
            }
        });
    }


    if (errorMsgOuter.length > 0) {
        errorMsgOuter = "<b>" + MsgErrorWhileReceving+" </b><br/>" + errorMsgOuter;
        $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html(errorMsgOuter);
    }
    else {
        $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html("<b>" + MsgReceivedSuccessfully +".</b>");
    }

    gblArrReceiveInfoToSave = null;
    closeOrdReceiveInfoModel();
    $('#btnReceiveAllNew').removeAttr('disabled');
    $('#DivLoading').hide();
    $('#OrdReceivedInfoDialog').modal();

}

function SaveReceiveData(obj) {
    if (obj.length > 250) {
        MakeReceiveArray(obj);
    }
    else {
        
        $.ajax({
            "url": "/ReceiveToolAsset/SaveReceiveInformationTool",
            "data": JSON.stringify(obj),
            "type": 'POST',
            "async": false,
            "cache": false,
            "dataType": "json",
            "contentType": "application/json",
            success: function (result) {
                $('#btnReceiveAllNew').removeAttr('disabled');
                var errorMsg = '';
                var ItemSelected;;
                if (gblActionName.toLowerCase() == "receivelist") {
                    ItemSelected = fnGetSelected(oTable);
                }
                else if (gblActionName.toLowerCase() == "orderlist" || gblActionName.toLowerCase() =="toolassetorderlist") {
                    ItemSelected = fnGetSelected($('#RecieveOrderLineItem' + OrdID).dataTable());

                    if (!isNaN(parseInt(result.OrderStatus)) && parseInt(result.OrderStatus) > 0) {
                        $('#ddlOrderStatus').val(result.OrderStatus);
                    }
                    SetReplenishRedCount();
                }
                $('#DivLoading').hide();
                if (!isNaN(ItemSelected.length) && ItemSelected.length > 0) {
                    for (var i = 0; i < ItemSelected.length; i++) {
                        var tr = ItemSelected[i];
                        $(tr).removeClass('row_selected');
                        var orddtlGuid = $(tr).find('#spnOrderDetailGUID').text();

                        if (result.Errors != null && result.Errors.length > 0) {
                            var obj = $.grep(result.Errors, function (el) {
                                return ($.trim(el.OrderDetailGuid.toString()) === $.trim(orddtlGuid.toString()));
                            });

                            if (obj !== undefined && obj != null && obj.length > 0 && obj[0].ErrorMassage.length > 0) {
                                $(tr).css('background-color', 'Red');
                                errorMsg = errorMsg + "<br/>"
                                errorMsg = errorMsg + "<span style='color:red'><b>" + obj[0].ErrorTitle + '</b>: ' + obj[0].ErrorMassage + "</span>"
                            }
                            else {
                                var obj = $.grep(result.Errors, function (el) {
                                    return ($.trim(el.OrderDetailGuid.toString()) === "00000000-0000-0000-0000-000000000000" && el.ErrorMassage.length > 0);
                                });

                                if (obj !== undefined && obj != null && obj.length > 0 && obj[0].ErrorMassage.length > 0) {
                                    $(tr).css('background-color', 'Red');
                                    errorMsg = errorMsg + "<br/>"
                                    errorMsg = errorMsg + "<span style='color:red'><b>" + obj[0].ErrorTitle + '</b>: ' + obj[0].ErrorMassage + "</span>"
                                }
                                else {
                                    $(tr).css('background-color', 'Green');
                                }
                            }
                        }
                        else {
                            $(tr).css('background-color', 'Green');
                        }
                    }
                }
                gblArrReceiveInfoToSave = null;

                closeOrdReceiveInfoModel();
                if (errorMsg.length > 0) {
                    errorMsg = "<b>" + MsgErrorWhileReceving +" </b><br/>" + errorMsg;
                    $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html(errorMsg);
                }
                else {
                    $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html("<b>" + MsgReceivedSuccessfully+"</b>");
                }

                $('#OrdReceivedInfoDialog').modal();
            },
            error: function (xhr) {
                $('#btnReceiveAllNew').removeAttr('disabled');
                $('#DivLoading').hide();
                closeOrdReceiveInfoModel();
                alert(MsgSaveReceiveInfoAjaxError);
                $('#DivLoading').hide();
            }

        });
    }
}

function OpenPreReceiveInfoPage(objToFillPreReceive) {
     
    var ajaxurl = '/ReceiveToolAsset/FillPreReceiveInformationTool';

    if (objToFillPreReceive.SerialNumberTracking) {
        ajaxurl = '/ReceiveToolAsset/FillPreReceiveInfoForSerialItem';
        closeOrdReceiveInfoModel();
    }
    else {
        $('div.ui-dialog[aria-labelledby="ui-dialog-title-divPreRecieveInfo"]').css('width', '15px');
        $('div.ui-dialog[aria-labelledby="ui-dialog-title-divPreRecieveInfo"]').css('height', '15px');
    }

    $.ajax({
        "url": ajaxurl,
        "data": JSON.stringify(objToFillPreReceive),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "text",
        "contentType": "application/json",
        success: function (result) {
            $('#divPreRecieveInfo').html(result);

        },
        error: function (xhr) {
            alert(MsgErrorInProcess);
            $('#DivLoading').hide();
        },
        complete: function () {
            $('.ui-dialog').css('top', '50px !important');
        }
    });

}

function GetToFillPreReceive(tr, aData) {
    
    var arrObj = new Array();
    var RecvQty = 0;
    if ($(tr).find("input[type='text'][id^='txtQtyToRecv']").length > 0) {
        RecvQty = $(tr).find("input[type='text'][id^='txtQtyToRecv']").val();
    }

    var toFillReceiveDetail = { "Quantity": RecvQty };
    arrObj.push(toFillReceiveDetail);

    var RecvItemNumber = aData.ToolName;
    var RecvCost = aData.ToolCost;
    var RecvOrderNumber = aData.OrderNumber;
    var RecvReleaseNumber = aData.OrderReleaseNumber;
    var RecvIsPackslipMandatory = aData.IsPackSlipNumberMandatory;
    var RecvItemTypeSerialLot = '';
    var RecvSerialNumberTracking = false;
    var RecvLotNumberTracking = false;
    var RecvDateCodeTracking = false;
    if (aData.SerialNumberTracking == 'Yes') {
        RecvItemTypeSerialLot = "Serial#";
        RecvSerialNumberTracking = true;
    }
    else if (aData.LotNumberTracking == 'Yes') {
        RecvItemTypeSerialLot = "Lot#";
        RecvLotNumberTracking = true;
    }

  
    var RecvRequestedQty = aData.RequestedQuantity;
    var RecvApprovedQty = aData.ApprovedQuantity;
    var RecvReceivedQty = aData.ReceivedQuantity;
    var RecvComment = aData.Comment;

    var RecvItemGUID = aData.ToolGUID;
    var RecvOrderDetailGUID = aData.ToolAssetOrderDetailGUID;
    var RecvOrderGUID = aData.ToolAssetOrderGUID;
    var RecvOrderStatus = aData.OrderStatus;
    var RecvUDF1 = getRecUDFSelectedVal(tr, "UDF1");
    var RecvUDF2 = getRecUDFSelectedVal(tr, "UDF2");
    var RecvUDF3 = getRecUDFSelectedVal(tr, "UDF3");
    var RecvUDF4 = getRecUDFSelectedVal(tr, "UDF4");
    var RecvUDF5 = getRecUDFSelectedVal(tr, "UDF5");
    var RecvBin = '';
    if ($(tr).find("#txtBinNumber").length > 0) {
        RecvBin = $(tr).find("#txtBinNumber").val();
    }
    else {
        RecvBin = aData.ReceiveBinName;
    }
    
    if ($(tr).find("#txtToolCost").length > 0) {
        RecvCost = $(tr).find("#txtToolCost").val();
    }

    //var RecvPackSlip = $(tr).find("span[id^='spnPackSlipNumber_']").text();
    var RecvPackSlip = '';
    if ($(tr).find("#txtPackslipNo").length > 0) {
        RecvPackSlip = $(tr).find("input[id^='txtPackslipNo']").val();
    }
    else {
        RecvPackSlip = $(tr).find("span[id^='spnPackSlipNumber_']").text();
    }

    var RecvDate = getCurrentDateByRoomFormat();//getCurrentDate();
    if ($(tr).find("input[type='text'][id^='txtReceiveDate']").length > 0) {
        RecvDate = $(tr).find($(tr).find("input[type='text'][id^='txtReceiveDate']")).val();
    }

    var toFillPreReceive = {
        "ToolName": RecvItemNumber, "OrderNumber": RecvOrderNumber,
        "ReleaseNumber": RecvReleaseNumber,
        "SerialNumberTracking": RecvSerialNumberTracking,
        "IsModelShow": false, "Cost": RecvCost,
        "Location": RecvBin,
        "BinNumber": RecvBin, "ReceivedDate": RecvDate, "PackSlipNumber": RecvPackSlip,
        "Comment": RecvComment,
        "UDF1": RecvUDF1, "UDF2": RecvUDF2, "UDF3": RecvUDF3, "UDF4": RecvUDF4, "UDF5": RecvUDF5,
        "ToolGUID": RecvItemGUID, "OrderDetailGUID": RecvOrderDetailGUID, "OrderGUID": RecvOrderGUID,
        "OrderStatus": RecvOrderStatus, "RequestedQty": RecvRequestedQty, "ApproveQty": RecvApprovedQty,
        "ReceiveQty": RecvReceivedQty, "MakePreReceiveDetail": arrObj
    };
    return toFillPreReceive;
}

function getRecUDFSelectedVal(tr, UdfName) {
    
    var returnUDFVal
    if ($(tr).find("#" + UdfName).length > 0) {
        if ($(tr).find("#" + UdfName)[0].nodeName === "SELECT")
            returnUDFVal = $(tr).find("#" + UdfName + " option:selected").text();
        else
            returnUDFVal = $(tr).find("#" + UdfName).val();
    }
    return returnUDFVal;
}

$('#divPreRecieveInfo').on('click', ".ShowAllOptions", function (e) {
    $(this).siblings('.PreRcvBinAuto').trigger("focus");
    $(this).siblings(".PreRcvBinAuto").autocomplete("search", " ");
});


$('#divPreRecieveInfo').on('focus', "input.PreRcvBinAuto", function (e) {
    var ajaxURL = '/Master/GetLocationsOfToolByOrderId';
    var tolGuid = $(this).parent().parent().parent().find('#hdnTolGuid').val();
    var includeQty = false;
    var tr = $(this).parent().parent().parent();


    var hdnIsLoadMoreLocations = $(tr).find("#hdnIsLoadMoreLocations").val();

    $(this).autocomplete({
        source: function (request, response) {
            $.ajax({
                url: ajaxURL,
                type: 'POST',
                data: JSON.stringify({ 'NameStartWith': request.term, 'ToolGUID': tolGuid, 'QtyRequired': includeQty, 'OnlyHaveQty': includeQty,'OrderId':'0', 'IsLoadMoreLocations': hdnIsLoadMoreLocations }),
                contentType: 'application/json',
                dataType: 'json',
                success: function (data) {
                    response($.map(data, function (Items) {
                        return {
                            label: Items.Value,
                            value: Items.Key
                        }
                    }));
                },
                error: function (err) {
                }
            });
        },
        autoFocus: false,
        minLength: 1,
        select: function (event, ui) {
            if (ui.item.value == MoreLocation) {
                $(tr).find("#hdnIsLoadMoreLocations").val("true");
                $(this).trigger("focus");
                $(this).autocomplete("search", " ");
                return false;
            }
            else {
                $(this).val(ui.item.value);
            }
        },
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            $('ul.ui-autocomplete').css('overflow-y', 'auto');
            $('ul.ui-autocomplete').css('max-height', '300px');
            $('ul.ui-autocomplete').css('z-index', '99999');
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        },
        change: function (event, ui) {
        }
    });
});
//$('#global_filter').keyup(function (evt) {
//    clearGlobaleSearchInput('global_filter', 'myDataTable', evt);
//});