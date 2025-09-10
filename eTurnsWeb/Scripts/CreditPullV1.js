var SelectedPreCreditObj;
var gblPreCreditObjToSave;
var SelectedPreQLCreditObj;
$(document).ready(function () {

    $('#divPreCreditInfo').dialog({
        autoOpen: false,
        modal: true,
        draggable: true,
        resizable: true,
        width: '50%',
        height: 500,
        title: "Credit",
        open: function () {
            $('#divPreCreditInfo').empty();
            var objToFillPreCredit = $(this).data("MakePreCredit");
            SelectedPreCreditObj = SelectedPreCreditObj.filter(function (el) {
                return (el.RowID !== objToFillPreCredit.RowID);
            });
            OpenPreCreditInfoPage(objToFillPreCredit);
        },
        close: function () {
            $('#divPreCreditInfo').empty();
            ShowHideCreditProgressBar(true);
            OpenPreCreditInfoDialog();
        }
    });
});

function OpenPreCreditInfoDialog() {
    if (SelectedPreCreditObj != null && SelectedPreCreditObj.length > 0) {
        setTimeout(function () { $('#divPreCreditInfo').data({ 'MakePreCredit': SelectedPreCreditObj[0] }).dialog('open') }, 50);
    }
    else if (gblPreCreditObjToSave != null && gblPreCreditObjToSave.length > 0 && SelectedPreCreditObj.length <= 0) {
        setTimeout(function () {
            SaveCreditData(gblPreCreditObjToSave)
        }, 100);
    }
    else {
        gblPreCreditObjToSave = null;
        SelectedPreCreditObj = null;
        ShowHideCreditProgressBar(false);
    }
}

function SaveCreditData(arr) {
    $.ajax({
        "url": "/Pull/SavePullCredit",
        "data": JSON.stringify(arr),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "json",
        "contentType": "application/json",
        success: function (result) {
            alert(result.Message);
            $('#ItemModeDataTable').dataTable().fnStandingRedraw();
            ShowHideCreditProgressBar(false)
        },
        error: function (xhr) {
            ShowHideCreditProgressBar(false)
            alert('Save Credit: Ajax request has Error');

        }

    });
}

function OpenPreCreditInfoPage(obj) {
    $.ajax({
        "url": "/Pull/FillPreCreditInfo",
        "data": JSON.stringify(obj),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "text",
        "contentType": "application/json",
        success: function (result) {
            $('#divPreCreditInfo').html(result);
        },
        error: function (xhr) {
            alert('Pre Credit Info Page: Ajax request has Error');
        }

    });
}

function CreditItems(obj) {

    ShowHideCreditProgressBar(true);
    if ($('#NewPullAction').val() == "Pull") {
        ShowHideCreditProgressBar(false);
        return false;
    }

    SelectedPreCreditObj = new Array();
    gblPreCreditObjToSave = new Array();
    SelectedPreQLCreditObj = new Array();
    var tr = $(obj).parent().parent();

    var itemType = $(tr).find('#spnOrderItemType').text();
    var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(tr)[0]);
    var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);

    var creditQty = $(tr).find('#txtQty').val();

    if (isNaN(parseFloat(creditQty)) || parseFloat(creditQty) <= 0) {
        alert(aData.ItemNumber + ': Quantity to credit must be greater than zero.');
        ShowHideCreditProgressBar(false);
        return false;
    }

    var itemGuid = null;
    var qlGuid = null;
    var bin = '';
    var itemTrack = '';
    var project = '';
    var itemNumber = aData.ItemNumber;
    var pullOrderNumber = "";
    if ($("#chkUsePullCommonUDF").is(":checked")) {
        if ($('#txtProjectSpentCommon').val() != '') {
            project = $('#txtProjectSpentCommon').val();
        }
    }
    else if ($(tr).find('#txtProjectSpent').length > 0) {
        project = $(tr).find('#txtProjectSpent').val();
    }
    if ($("#chkUsePullCommonUDF").is(":checked")) {
        if ($('#txtPullOrderNumberCommon').val() != '') {
            pullOrderNumber = $('#txtPullOrderNumberCommon').val();
        }
    }
    else if ($(tr).find('#txtPullOrderNumber') != null) {
        if ($(tr).find('#txtPullOrderNumber').attr("class") == 'selectBox') {
            pullOrderNumber = $(tr).find('#txtPullOrderNumber option:selected').text();
        }
        else {
            pullOrderNumber = $(tr).find('#txtPullOrderNumber').val();
        }
    }
    if ($(tr).find('#txtBinNumber').length > 0) {
        bin = $(tr).find('#txtBinNumber').val();
    }
    if ($(tr).find('#spnItemGUID').length > 0) {
        itemGuid = $(tr).find('#spnItemGUID').text();
    }
    if ($(tr).find('#spnQuickListGUID').length > 0) {
        qlGuid = $(tr).find('#spnQuickListGUID').text();
    }
    //var pullOrderNumber = '';

    //if (typeof $(tr).find('input#txtPullOrderNumber') != "undefined") {
    //    pullOrderNumber = $(tr).find('input#txtPullOrderNumber').val();
    //}


    if (aData.SerialNumberTracking == "Yes") {
        itemTrack = "SERIALTRACK"
    }
    else if (aData.LotNumberTracking == "Yes") {
        itemTrack = "LOTTRACK"
    }
    else if (aData.DateCodeTracking == "Yes") {
        itemTrack = "DATECODETRACK"
    }
    else if (itemType == 2) {
        itemTrack = "QUICKLIST"
    }

    var udf1 = GetUDFValue('UDF1', $(tr));
    var udf2 = GetUDFValue('UDF2', $(tr));
    var udf3 = GetUDFValue('UDF3', $(tr));
    var udf4 = GetUDFValue('UDF4', $(tr));
    var udf5 = GetUDFValue('UDF5', $(tr));
    if ($("#chkUsePullCommonUDF").is(":checked")) {
        udf1 = GetUDFValue('UDF1PullCommon', null)
        udf2 = GetUDFValue('UDF2PullCommon', null)
        udf3 = GetUDFValue('UDF3PullCommon', null)
        udf4 = GetUDFValue('UDF4PullCommon', null)
        udf5 = GetUDFValue('UDF5PullCommon', null)
    }
    //alert(project);
    var obj = {
        'RowID': 1, 'ItemGuid': itemGuid, 'QLGuid': qlGuid, 'ItemType': itemType,
        'Bin': bin, 'ProjectName': project, 'Quantity': creditQty, 'ItemTracking': itemTrack,
        'UDF1': udf1, 'UDF2': udf2, 'UDF3': udf3, 'UDF4': udf4, 'UDF5': udf5, 'ItemNumber': itemNumber,
        'WOGuid': vWorkOrderDetailGUID, 'PullOrderNumber': pullOrderNumber
    };

    if (itemType == 2) {
        SelectedPreQLCreditObj.push(obj);
    }
    else {
        SelectedPreCreditObj.push(obj);
    }

    if (SelectedPreQLCreditObj != null && SelectedPreQLCreditObj.length > 0) {
        AddQuickListToCredit(SelectedPreQLCreditObj)
    }
    else {
        OpenPreCreditInfoDialog();
    }
}


$('#btnCreditAll').on('click', function (e) {
    ShowHideCreditProgressBar(true);
    if ($('#NewPullAction').val() == "Pull") {
        ShowHideCreditProgressBar(false);
        return false;
    }

    SelectedPreCreditObj = new Array();
    gblPreCreditObjToSave = new Array();
    SelectedPreQLCreditObj = new Array();
    if ($('#ItemModeDataTable tbody tr.row_selected').length <= 0) {
        ShowHideCreditProgressBar(false);
        alert('Please select atleast one record');
        return false;
    }
    $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {
        var itemType = $(this).find('#spnOrderItemType').text();
        var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(this)[0]);
        var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);

        var creditQty = $(this).find('#txtQty').val();

        if (isNaN(parseFloat(creditQty)) || parseFloat(creditQty) <= 0) {
            alert(aData.ItemNumber + ': Quantity to credit must be greater than zero.');
            return false;
        }

        var itemGuid = null;
        var qlGuid = null;
        var bin = '';
        var itemTrack = '';
        var project = '';
        var itemNumber = aData.ItemNumber;
        if ($("#chkUsePullCommonUDF").is(":checked")) {
            if ($('#txtProjectSpentCommon').val() != '') {
                project = $('#txtProjectSpentCommon').val();
            }
        }
        else if ($(this).find('#txtProjectSpent').length > 0) {
            project = $(this).find('#txtProjectSpent').val();
        }
        if ($(this).find('#txtBinNumber').length > 0) {
            bin = $(this).find('#txtBinNumber').val();
        }
        if ($(this).find('#spnItemGUID').length > 0) {
            itemGuid = $(this).find('#spnItemGUID').text();
        }
        if ($(this).find('#spnQuickListGUID').length > 0) {
            qlGuid = $(this).find('#spnQuickListGUID').text();
        }
        var pullOrderNumber = '';

        //if (typeof $(this).find('input#txtPullOrderNumber') != "undefined") {
        //    pullOrderNumber = $(this).find('input#txtPullOrderNumber').val();
        //}
        if ($(this).find('#txtPullOrderNumber') != null) {
            if ($(this).find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                vPullOrderNumber = $(this).find('#txtPullOrderNumber option:selected').text();
            }
            else {
                vPullOrderNumber = $(this).find('#txtPullOrderNumber').val();
            }
        }

        if (aData.SerialNumberTracking == "Yes") {
            itemTrack = "SERIALTRACK"
        }
        else if (aData.LotNumberTracking == "Yes") {
            itemTrack = "LOTTRACK"
        }
        else if (aData.DateCodeTracking == "Yes") {
            itemTrack = "DATECODETRACK"
        }
        else if (itemType == 2) {
            itemTrack = "QUICKLIST"
        }

        var udf1 = GetUDFValue('UDF1', $(this));
        var udf2 = GetUDFValue('UDF2', $(this));
        var udf3 = GetUDFValue('UDF3', $(this));
        var udf4 = GetUDFValue('UDF4', $(this));
        var udf5 = GetUDFValue('UDF5', $(this));
        if ($("#chkUsePullCommonUDF").is(":checked")) {
            udf1 = GetUDFValue('UDF1PullCommon', null)
            udf2 = GetUDFValue('UDF2PullCommon', null)
            udf3 = GetUDFValue('UDF3PullCommon', null)
            udf4 = GetUDFValue('UDF4PullCommon', null)
            udf5 = GetUDFValue('UDF5PullCommon', null)
        }
        //alert(project);
        var obj = {
            'RowID': i, 'ItemGuid': itemGuid, 'QLGuid': qlGuid, 'ItemType': itemType,
            'Bin': bin, 'ProjectName': project, 'Quantity': creditQty, 'ItemTracking': itemTrack,
            'UDF1': udf1, 'UDF2': udf2, 'UDF3': udf3, 'UDF4': udf4, 'UDF5': udf5, 'ItemNumber': itemNumber,
            'WOGuid': vWorkOrderDetailGUID, 'PullOrderNumber': pullOrderNumber
        };

        if (itemType == 2) {
            SelectedPreQLCreditObj.push(obj);
        }
        else {
            SelectedPreCreditObj.push(obj);
        }
    });

    if (SelectedPreQLCreditObj != null && SelectedPreQLCreditObj.length > 0) {
        AddQuickListToCredit(SelectedPreQLCreditObj)
    }
    else {
        OpenPreCreditInfoDialog();
    }
});

function AddQuickListToCredit(SelectedPreQLCreditObj) {
    $.ajax({
        "url": "/Pull/GetQLItemForCredit",
        "data": JSON.stringify(SelectedPreQLCreditObj),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "json",
        "contentType": "application/json",
        success: function (result) {
            if (result.Status && result.QLItems != null && result.QLItems.length > 0) {
                for (var i = 0; i < result.QLItems.length; i++) {
                    SelectedPreCreditObj.push(result.QLItems[i]);
                }
            }
            OpenPreCreditInfoDialog();
            //$('#divPreCreditInfo').html(result);
        },
        error: function (xhr) {
            alert('Pre Credit Info Page: Ajax request has Error');
        }

    });
}

function GetUDFValue(udfName, tr) {
    var udfValue = '';
    if (tr != null && tr != undefined) {
        if ($(tr).find('#' + udfName) != null) {
            if ($(tr).find('#' + udfName).attr("class") == 'selectBox') {
                udfValue = $(tr).find('#' + udfName + ' option:selected').text();
            }
            else {
                udfValue = $(tr).find('#' + udfName).val();
            }
        }
    }
    else {
        if ($('#' + udfName).attr("class") == 'selectBox') {
            udfValue = $('#' + udfName + ' option:selected').text();
        }
        else {
            udfValue = $('#' + udfName).val();
        }
    }
    return udfValue;
}

function ShowHideCreditProgressBar(isShow) {
    if (isShow) {
        $('#DivLoading').show();
        $('#divCreditProcessing').modal();
        $('#divCreditProcessing').parent().parent().find(".modalCloseImg").css('display', 'none');
    }
    else {
        $('#DivLoading').hide();
        $.modal.impl.close();
    }
}