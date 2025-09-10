
var _creditPull = (function ($) {

    var self = {};

    self.isNewConsumePullObj = function () {
        return typeof _NewConsumePull !== 'undefined' && _NewConsumePull !== null;
    };

    self.getBinId = function ($tr) {
        var vBinID = 0;

        if (self.isNewConsumePullObj()) {
            vBinID = _NewConsumePull.getDataFromRow($tr, 'BinID');
        }
        else {
            if ($tr.find('#BinID').length > 0) {
                vBinID = $tr.find('#BinID').val() == "" ? 0 : $tr.find('#BinID').val();
            }
        }

        return vBinID;
    }

    self.GetProjectId = function ($tre) {
        var vProjectID = "";
        if (self.isNewConsumePullObj()) {
            vProjectID = _NewConsumePull.GetProjectId($tre);
        }
        else {
            if ($tre.find('#ProjectID')[0] == undefined) {
                vProjectID = "";
            }
            else {
                vProjectID = $tre.find('#ProjectID')[0].value == "" ? "" : $tre.find('#ProjectID')[0].value;
            }
        }
        return vProjectID;
    };

    self.gethdnDPQ = function ($tr) {
        var val = 0;
        if (self.isNewConsumePullObj()) {
            val = _NewConsumePull.getDataFromRow($tr, 'hdnDPQ');

        }
        else {
            if ($tr.find('#hdnDPQ').length > 0) {
                val = $tr.find('#hdnDPQ').val() == "" ? 0 : $tr.find('#hdnDPQ').val();
                return val;
            }
        }
        return val;
    };

    self.getItemDataFromRow = function ($tr) {
        var itemData = null;
        if (self.isNewConsumePullObj()) {
            itemData = _NewConsumePull.getItemDataFromRow($tr);
        }
        return itemData;
    }

    self.getspnOrderItemType = function ($tr) {
        var val;
        if (self.isNewConsumePullObj()) {
            var itemData = _NewConsumePull.getItemDataFromRow($tr);
            val = itemData.spnOrderItemType;
        }
        else {
            val = $tr.find('#spnOrderItemType').text();
        }
        return val;
    }

    self.getspnIsIgnoreCreditRule = function ($tr) {
        var val = false;
        if (self.isNewConsumePullObj()) {
            var itemData = _NewConsumePull.getItemDataFromRow($tr);
            val = itemData.spnIsIgnoreCreditRule;
        }
        else {
            if ($tr.find('#spnIsIgnoreCreditRule').length > 0) {
                val = $tr.find('#spnIsIgnoreCreditRule').text();
            }
        }
        return val;
    }

    return self;

})(jQuery); //_creditPull end

var SelectedPreCreditObj;
var gblPreCreditObjToSave;
var gblPreCreditObjToSaveForCredit;
var SelectedPreQLCreditObj;
var DeletedRowObject ="";

$(document).ready(function () {

    var IsIgnoreCreditRule = false;

    $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {
        IsIgnoreCreditRule = _creditPull.getspnIsIgnoreCreditRule($(this));//$(this).find('#spnIsIgnoreCreditRule').text();
    });
    var ItemGuid = null;
    var WOGuid = null;
    var ItemType = null;
    var Bin = null;
    var ProjectName = null;
    var Quantity = null;
    var UDF1 = null;
    var UDF2 = null;
    var UDF3 = null;
    var UDF4 = null;
    var UDF5 = null;
    var itemTrack = null;
    var PrevPullQty = null;
    var ItemNumber = null;
    var PullOrderNumber = null;   
  
    if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
        $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {


            if (!_creditPull.isNewConsumePullObj()) {
            ItemType = $(this).find('#spnOrderItemType').text();
            ItemNumber = $(this).find('#spnItemNumber').text();
            }
            else {
                var itemData = _creditPull.getItemDataFromRow($(this));
                ItemType = itemData.spnOrderItemType;
                ItemNumber = itemData.spnItemNumber;
            }

            Quantity = $(this).find('#txtQty').val();

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#txtProjectSpentCommon').val() != '') {
                    project = $('#txtProjectSpentCommon').val();
                }
            }
            else if ($(this).find('#txtProjectSpent').length > 0) {
                project = $(this).find('#txtProjectSpent').val();
            }
            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#txtPullOrderNumberCommon').val() != '') {
                    pullOrderNumber = $('#txtPullOrderNumberCommon').val();
                }
            }
            else if ($(this).find('#txtPullOrderNumber') != null) {
                if ($(this).find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                    pullOrderNumber = $(this).find('#txtPullOrderNumber option:selected').text();
                }
                else {
                    pullOrderNumber = $(this).find('#txtPullOrderNumber').val();
                }
            }
            if ($(this).find('#txtBinNumber').length > 0) {
                bin = $(this).find('#txtBinNumber').val();
            }
            if (!_creditPull.isNewConsumePullObj()) {
                if ($(this).find('#spnItemGUID').length > 0) {
                    ItemGuid = $(this).find('#spnItemGUID').text();
                }
                if ($(this).find('#spnQuickListGUID').length > 0) {
                    qlGuid = $(this).find('#spnQuickListGUID').text();
                }
            }
            else {
                var itemData = _creditPull.getItemDataFromRow($(this));
                itemGuid = itemData.spnItemGUID;
                qlGuid = itemData.spnQuickListGUID;
            }
            //if (aData.SerialNumberTracking == "Yes") {
            //    itemTrack = "SERIALTRACK"
            //}
            //else if (aData.LotNumberTracking == "Yes") {
            //    itemTrack = "LOTTRACK"
            //}
            //else if (aData.DateCodeTracking == "Yes") {
            //    itemTrack = "DATECODETRACK"
            //}
            //else if (itemType == 2) {
            //    itemTrack = "QUICKLIST"
            //}

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
            
            var vPullSupplierAccountNumber = "";
            if ($(this).find('#hdnSupplierAccountNumber') != null) {
                vPullSupplierAccountNumber = $(this).find('#hdnSupplierAccountNumber').val();
            }

            var creditarrObj = new Array();
            creditarrObj.push({ 'Serial': null, 'Lot': null, 'ExpireDate': null, 'Qty': Quantity });
            var Creditobj = {};
            Creditobj = {
                'ItemGuid': ItemGuid, 'WOGuid': WOGuid,
                'ItemType': ItemType, 'Bin': bin,
                'ProjectName': project, 'Quantity': Quantity,
                'UDF1': udf1, 'UDF2': udf2, 'UDF3': udf3,
                'UDF4': udf4, 'UDF5': udf5, 'ItemTracking': itemTrack,
                'PrevPullQty': 0, 'ItemNumber': ItemNumber, 'PullOrderNumber': pullOrderNumber,
                'SupplierAccountGuid': vPullSupplierAccountNumber,
                'PrevPullsToCredit': creditarrObj
            };

            gblPreCreditObjToSave.push(Creditobj);
        });
    }

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

    $("#divPreCreditInforSerialLot").off('change', "input.dateTextbox");
    $("#divPreCreditInforSerialLot").on('change', "input.dateTextbox", function (e) {
        

        
        var objCurtxt = $(this);
        var oldValue = $(objCurtxt).val();        

        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
        var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

        var dtThisItem = $("#tblItemPull_" + ids[1].toString()).dataTable();
        var currentTR = $(objCurtxt).parent().parent()[0];
        var row_id = dtThisItem.fnGetPosition(currentTR);

        var CurrentSelectedLotNumberValue = "";
        var CurrentSelectedLotNumber = $(currentTR).find('#txtLotOrSerailNumber');
        
        if ($.trim(oldValue) == '')
            return;

        if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
            if (CurrentSelectedLotNumber != undefined) {
                CurrentSelectedLotNumberValue = $(CurrentSelectedLotNumber).val();
            }
        }

        $(currentTR).find("input[type='text'][name='txtExpirationDate']").val($(objCurtxt).val());

        var CurrentSelectedExpirationDate = $(currentTR).find("input[type='text'][name='txtExpirationDate']").val();

        
        var BinID = 0;
        if ($("#hdnBinIDValue").length > 0) {
            BinID = $("#hdnBinIDValue").val();
        }

        var isDuplicateEntry = false;
        $("#tblItemPull_" + ids[1].toString() + " tbody tr").each(function (i) {
            
            if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                if (i != row_id && CurrentSelectedLotNumberValue != "") {
                    var tr = $(this);
                    var SelectedExpirationDate = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                    var SelectedLotNumber = $(tr).find('#txtLotOrSerailNumber').val();
                    if (SelectedExpirationDate == $(objCurtxt).val() && SelectedLotNumber == CurrentSelectedLotNumberValue) {
                        isDuplicateEntry = true;
                    }
                    if (SelectedLotNumber == CurrentSelectedLotNumberValue) {
                        isDuplicateEntry = true;
                    }
                }
            }
            else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "DateCodeTracking") {
                if (i != row_id) {
                    var tr = $(this);
                    var SelectedExpirationDate = $(tr).find("input[type='text'][name='txtExpirationDate']").val() ;
                    if (SelectedExpirationDate == $(objCurtxt).val()) {
                        isDuplicateEntry = true;
                    }
                }
            }
        });

        if (isDuplicateEntry == true) {
            if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK")
                alert(MsgDuplicateLotNumberExpirationDate);
            else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "DateCodeTracking") {
                alert(MsgDuplicateExpirationDate);
            }

            $(objCurtxt).val("");
            $(objCurtxt).focus();
        }
        else {
            if (CurrentSelectedLotNumberValue != "" && $("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK")
                
            $.ajax({
                type: "POST",
                url: ValidateLotDateCodeForCreditUrl,
                contentType: 'application/json',
                dataType: 'json',
                data: "{ ItemGuid: '" + ids[1].toString() + "', LotNumber: '" + CurrentSelectedLotNumberValue + "', ExpirationDate: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + BinID + "' }",                
                success: function (result) {
                    if (result.IsSerailAvailableForCredit == "false" || result.IsSerailAvailableForCredit == " False" || result.IsSerailAvailableForCredit == false) {
                        alert(MsgLotNumberExpDateValidation);
                        $(objCurtxt).val("");
                        $(objCurtxt).focus();
                    }
                },
                error: function (err) {
                    //alert(err);
                    console.log(err);
                }
            });
        }
    });

    $("#divPreCreditInforSerialLot").off('focus', "input.dateTextbox");
    $("#divPreCreditInforSerialLot").on('focus', "input.dateTextbox", function (e) {
      
        var objCurtxt = $(this);
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
        var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);
               
        var dtItemPull = "#tblItemPull_" + ids[1].toString();
        var strSerialLotNos = "";

        $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {

            if (index != aPos) {
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                    var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtLotOrSerailNumber != undefined)
                        strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                }
                if (hdnDateCodeTracking == "true") {                   
                    var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                    if (hdnExpiration != undefined)
                        strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                }
                else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false") {
                    var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                    if (hdnBinNumber != undefined)
                        strSerialLotNos = strSerialLotNos + hdnBinNumber + "|#|";
                }
            }

        });       
    });

    $("#divPreCreditInforSerialLot").off('change', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#divPreCreditInforSerialLot").on('change', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {
        
        var objCurtxt = $(this);
        var oldValue = $(objCurtxt).val();
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

        var dtThisItem = $("#tblItemPull_" + ids[1].toString()).dataTable();
        var currentTR = $(objCurtxt).parent().parent().parent()[0];
        var row_id = dtThisItem.fnGetPosition(currentTR);

        var CurrentSelectedExpirationDate = $(currentTR).find("input[type='text'][name='txtExpirationDate']").val();

        if ($.trim(oldValue) == '')
            return;

        
        var BinID = 0;
        if ($("#hdnBinIDValue").length > 0) {
            BinID = $("#hdnBinIDValue").val();
        }

        var isDuplicateEntry = false;
        var OtherPullQuantity = 0;
               
        $("#tblItemPull_" + ids[1].toString() + " tbody tr").each(function (i) {
            
            if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                if (i != row_id && $(objCurtxt).val() != "") {
                    var tr = $(this);                    
                    var SelectedExpirationDate = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                    var SelectedLotNumber = $(tr).find('#' + objCurtxt.prop("id")).val();
                    if (SelectedExpirationDate != "") {
                        if (SelectedExpirationDate == CurrentSelectedExpirationDate && SelectedLotNumber == $(objCurtxt).val()) {
                            isDuplicateEntry = true;
                        }
                    }
                    if (SelectedLotNumber == $(objCurtxt).val()) {
                        isDuplicateEntry = true;
                    }
                }
            }
            else {
                if (i != row_id) {
                    var tr = $(this);
                    var SerialOrLotNumber = $(tr).find('#' + objCurtxt.prop("id")).val();
                    if (SerialOrLotNumber == $(objCurtxt).val()) {
                        isDuplicateEntry = true;
                    }
                    else {
                        var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                        OtherPullQuantity = OtherPullQuantity + parseFloat(txtPullQty);
                    }
                }
            }
        });

        if (isDuplicateEntry == true) {           
            
            if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK")
                alert(MsgDuplicateLotNumberExpirationDate);
            else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "SERIALDATECODETRACK")
                alert(DuplicateSerialFound);
            else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LotNumberTracking")
                alert(MsgDuplicateLotNumber);
            else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "SerialNumberTracking")
                alert(DuplicateSerialFound);
            else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "DateCodeTracking") {
                alert(MsgDuplicateExpirationDate);
            }

            $(objCurtxt).val("");
            $(objCurtxt).focus();
        }
        else {
            if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "SerialNumberTracking" ||
                $("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "SERIALDATECODETRACK") {
                
                $.ajax({
                    type: "POST",
                    url: ValidateSerialNumberForCreditUrl,
                    contentType: 'application/json',
                    dataType: 'json',
                    data: "{ ItemGuid: '" + ids[1].toString() + "', SerialNumber: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + BinID + "' }",
                    success: function (result) {
                        if (result.IsSerailAvailableForCredit == "false" || result.IsSerailAvailableForCredit == " False" || result.IsSerailAvailableForCredit == false) {
                            alert(MsgCreditTransactionForSerialNumber);
                            $(objCurtxt).val("");
                            $(objCurtxt).focus();
                        }
                    },
                    error: function (err) {
                        //alert(err);
                        console.log(err);
                    }
                });
            }
            else if (CurrentSelectedExpirationDate != "" && $("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                
                $.ajax({
                    type: "POST",
                    url: ValidateLotDateCodeForCreditUrl,
                    contentType: 'application/json',
                    dataType: 'json',
                    data: "{ ItemGuid: '" + ids[1].toString() + "', LotNumber: '" + $.trim($(objCurtxt).val()) + "', ExpirationDate: '" + CurrentSelectedExpirationDate + "',BinID: '" + BinID + "' }",
                    success: function (result) {
                        if (result.IsSerailAvailableForCredit == "false" || result.IsSerailAvailableForCredit == " False" || result.IsSerailAvailableForCredit == false) {
                            alert(MsgLotNumberExpDateValidation);
                            $(objCurtxt).val("");
                            $(objCurtxt).focus();
                        }
                    },
                    error: function (err) {
                        //alert(err);
                        console.log(err);
                    }
                });
            }
        }
    });

    $("#divPreCreditInforSerialLot").off('focus', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#divPreCreditInforSerialLot").on('focus', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {

        var objCurtxt = $(this);
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

        var dtItemPull = "#tblItemPull_" + ids[1].toString();
        var strSerialLotNos = "";

        var BinID = $("#divPreCreditInforSerialLot").find("#hdnBinID_" + ids[1].toString()).val();

        $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {

            if (index != aPos) {
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                    var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtLotOrSerailNumber != undefined)
                        strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                }
                if (hdnDateCodeTracking == "true") {                    
                    var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                    if (hdnExpiration != undefined)
                        strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                }
                else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false") {
                    var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                    if (hdnBinNumber != undefined)
                        strSerialLotNos = strSerialLotNos + hdnBinNumber + "|#|";
                }
            }

        });

        if ($(this).hasClass("AutoSerialLot")) {
            $(this).autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/Pull/GetLotOrSerailNumberListForCredit',
                        contentType: 'application/json',
                        dataType: 'json',
                        data: {
                            maxRows: 1000,
                            name_startsWith: request.term,
                            ItemGuid: ids[1].toString(),
                            BinID: BinID,
                            prmSerialLotNos: strSerialLotNos
                        },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.LotOrSerailNumber,
                                    value: item.LotOrSerailNumber,
                                    selval: item.LotOrSerailNumber
                                }
                            }));
                        },
                        error: function (err) {

                        }
                    });
                },
                autoFocus: false,
                minLength: 0,
                select: function (event, ui) {
                   
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                    $(this).autocomplete('widget').css('z-index', 9000);
                    $('ul.ui-autocomplete').css('overflow-y', 'auto');
                    $('ul.ui-autocomplete').css('max-height', '300px');
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                    $(objCurtxt).trigger("change");
                }
            });
        }
    });

    $("#divPreCreditInforSerialLot").off("click", "input[type='button'][name='btnLoadMoreLots']");
    $("#divPreCreditInforSerialLot").on("click", "input[type='button'][name='btnLoadMoreLots']", function () {
        var vItemGUID = $(this).prop("id").split('_')[1];        
        var dtID = "#tblItemPull_" + vItemGUID;
        var strIds = "";

        var BinNumber = "";
        var IsLotNumberTracking = false;
        var IsSerialNumberTracking = false;
        var IsDateCodeTracking = false;

        var MaxQuantity = $("#txtPoolQuantity_" + vItemGUID)[0].value;
        var BinID = $("#hdnBinID_" + vItemGUID)[0].value;
        var CurrentDate = $("#hdnCurrentDate_" + vItemGUID)[0].value;
        
        var TotalQuantity = 0;
        $("#tblItemPull_" + vItemGUID).find("[id*='txtPullQty_']").each(function () {
            TotalQuantity = TotalQuantity + parseInt($(this)[0].value);
        });

        var isError = false;

        if (MaxQuantity > TotalQuantity) {
            IsLoadMoreLotsClicked = true;
            $(dtID).find("tbody").find("tr").each(function (index, tr) {

                BinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                
                if (hdnLotNumberTracking == "true")
                    IsLotNumberTracking = true;
                if (hdnSerialNumberTracking == "true")
                    IsSerialNumberTracking = true;
                if (hdnDateCodeTracking == "true")
                    IsDateCodeTracking = true;

                if (IsLotNumberTracking == true) {
                    var txtLotNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtLotNumber == "" || txtLotNumber == null) {
                        alert(EnterLotNumber);
                        isError = true;
                        return false;
                    }
                }
                if (IsSerialNumberTracking == true) {
                    var txtSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtSerailNumber == "" || txtSerailNumber == null) {
                        alert(MsgSerialNumberValidation);
                        isError = true;
                        return false;
                    }
                }
                if (IsDateCodeTracking == true) {
                    var txtExpiration = $.trim($(tr).find("input[type='text'][name^='txtExpirationDate']").val());
                    if (txtExpiration == "" || txtExpiration == null) {
                        alert(MsgEnterExpirationDate);
                        isError = true;
                        return false;
                    }
                }
                
                if (txtPullQty != undefined) {
                    if (txtPullQty == "") {
                        txtPullQty = "0";
                    }
                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, txtLotOrSerailNumber))
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                    }
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnExpiration))
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                        if (hdnExpiration != undefined)
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnBinNumber))
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                    }
                }
            });

            if (isError == false) {
                $("#hdnCurrentLoadedIds_" + vItemGUID).val(strIds);
                var dt = $(dtID).dataTable();
                dt.fnStandingRedraw();
            }
        }
        else {
            alert(MsgPullCreditQuantity);
        }
    });

    function IsLotSerialExistsInCurrentLoadedForCredit(strIds, SerialLot) {
        if (SerialLot.trim() == '')
            return true;

        if (strIds.trim() == '')
            return false

        var ArrIds = strIds.split(',');
        var i = 0;
        for (i = 0; i < ArrIds.length; i++) {
            if (ArrIds[i].split('_')[0].toLowerCase() == SerialLot.toLowerCase()) {
                return true;
            }
        }

        return false;
    }

    $("#divPreCreditInforSerialLot").off("click", "input[type='button'][name='btnPullPopup']");
    $("#divPreCreditInforSerialLot").on("click", "input[type='button'][name='btnPullPopup']", function () {
        
        var vItemGUID = $(this).prop("id").split('_')[1];
        
        var dtID = "#tblItemPull_" + vItemGUID;

        var ArrItem = new Array();
        var arrItemDetails;
        var ErrorMessage = ValidateSinglePullForCredit(vItemGUID);

        if (ErrorMessage == "") {

            var itemGuid = vItemGUID;
            var SpanQty = $("#divPreCreditInforSerialLot").find("#txtPoolQuantity_" + vItemGUID);
            var BinID = $("#divPreCreditInforSerialLot").find("#hdnBinID_" + vItemGUID).val();
            var MainBinNumber = $("#divPreCreditInforSerialLot").find("#hdnMainBinNumber_" + vItemGUID).val();

            var strpullobj;   
                              
            if ($("#divPreCreditInforSerialLot").find("#hdnPullMasterDTO_" + vItemGUID).length > 0)
                strpullobj = JSON.parse($("#divPreCreditInforSerialLot").find("#hdnPullMasterDTO_" + vItemGUID).val());

            var creditarrObj = new Array();
            $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {

                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                
                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();

                if (txtPullQty != "") {

                    var txtLotOrSerailNumber = "";
                    if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                        var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                    var vSerialNumber = "";
                    var vLotNumber = "";
                    var vExpiration = "";

                    if (hdnSerialNumberTracking == "true")
                        vSerialNumber = txtLotOrSerailNumber;
                    if (hdnLotNumberTracking == "true")
                        vLotNumber = txtLotOrSerailNumber;
                    if (hdnDateCodeTracking == "true")
                        vExpiration = hdnExpiration;                   
                }


               
                creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });
            });
            var Creditobj = {};

            var ItemTracking = "";
            if (strpullobj.SerialNumberTracking)
                ItemTracking = "SERIALTRACK";
            else if (strpullobj.LotNumberTracking)
                ItemTracking = "LOTTRACK";
            else if (strpullobj.DateCodeTracking)
                ItemTracking = "DATECODETRACK";

            Creditobj = {
                'ItemGuid': strpullobj.ItemGUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                'SupplierAccountGuid': strpullobj.SupplierAccountGuid,
                'PrevPullsToCredit': creditarrObj
            };
           
            if (gblPreCreditObjToSaveForCredit == null) {
                gblPreCreditObjToSaveForCredit = new Array();
            }
            gblPreCreditObjToSaveForCredit.push(Creditobj);

            if (gblPreCreditObjToSaveForCredit != null && gblPreCreditObjToSaveForCredit.length > 0) {
                ShowHideCreditProgressBar(true);
                setTimeout(function () {
                    SaveCreditData(gblPreCreditObjToSaveForCredit)
                }, 100);
            }
        }
        else {
            alert(ErrorMessage);
        }
    });

    $("#divPreCreditInforSerialLot").off("click", "input[type='button'][name='btnPullAllPopUp']");
    $("#divPreCreditInforSerialLot").on("click", "input[type='button'][name='btnPullAllPopUp']", function () {        
        var ArrItem = new Array();
        gblPreCreditObjToSaveForCredit = new Array();
        var arrItemDetails;       
        var ErrorMessage = ValidateAllPullForCredit();

        if (ErrorMessage == "") {
            $("#divPreCreditInforSerialLot").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
                var vItemGUID = $(tblHeader).prop("id").split('_')[1];
                var strpullobj = JSON.parse($(tblHeader).find("#hdnPullMasterDTO_" + vItemGUID).val());
                

                var ArrItem = new Array();
                var arrItemDetails;
                var itemGuid = vItemGUID;
                var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + vItemGUID);

                var BinID = $(tblHeader).find("#hdnBinID_" + vItemGUID).val();
                var MainBinNumber = $(tblHeader).find("#hdnMainBinNumber_" + vItemGUID).val();

                var creditarrObj = new Array();
                var ItemTracking = "";

                $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {

                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                    var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                    var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();

                    if (txtPullQty != "") {

                        var txtLotOrSerailNumber = "";
                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                            var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                        var vSerialNumber = "";
                        var vLotNumber = "";
                        var vExpiration = "";

                        if (hdnSerialNumberTracking == "true")
                            vSerialNumber = txtLotOrSerailNumber;
                        if (hdnLotNumberTracking == "true")
                            vLotNumber = txtLotOrSerailNumber;
                        if (hdnDateCodeTracking == "true")
                            vExpiration = hdnExpiration;
                    }

                    creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });

                    if (strpullobj.SerialNumberTracking)
                        ItemTracking = "SERIALTRACK";
                    else if (strpullobj.LotNumberTracking)
                        ItemTracking = "LOTTRACK";
                    else if (strpullobj.DateCodeTracking)
                        ItemTracking = "DATECODETRACK";
                });

                var Creditobj = {};
                Creditobj = {
                    'ItemGuid': strpullobj.ItemGUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                    'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                    'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                    'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                    'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                    'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                    'SupplierAccountGuid' : strpullobj.SupplierAccountGuid,
                    'PrevPullsToCredit': creditarrObj
                };
                if (gblPreCreditObjToSaveForCredit == null) {
                    gblPreCreditObjToSaveForCredit = new Array();
                }
                gblPreCreditObjToSaveForCredit.push(Creditobj);
            });

            if (gblPreCreditObjToSaveForCredit != null && gblPreCreditObjToSaveForCredit.length > 0) {
                ShowHideCreditProgressBar(true);
                setTimeout(function () {
                    SaveCreditData(gblPreCreditObjToSaveForCredit)
                }, 100);
            }
        }
        else {
            alert(ErrorMessage);
        }
    });

    $("#divPreCreditInforSerialLot").off("click", "input[type='button'][name='btnPullAllPopUpNew']");
    $("#divPreCreditInforSerialLot").on("click", "input[type='button'][name='btnPullAllPopUpNew']", function () {        
        var ArrItem = new Array();
        var arrItemDetails;
        var ErrorMessage = ValidateAllPullForCredit();
        gblPreCreditObjToSaveForCredit = new Array();
        if (ErrorMessage == "") {
            $("#divPreCreditInforSerialLot").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
                var strpullobj = JSON.parse($(tblHeader).find("input[name='hdnPullMasterDTO']").val());
                var vItemGUID = $(tblHeader).prop("id").split('_')[1];

                var ArrItem = new Array();
                var arrItemDetails;
                
                var itemGuid = vItemGUID;
                var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + vItemGUID);
                var BinID = $(tblHeader).find("#hdnBinID_" + vItemGUID).val();
                var MainBinNumber = $(tblHeader).find("#hdnMainBinNumber_" + vItemGUID).val();

                var creditarrObj = new Array();
                var ItemTracking = "";

                $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {

                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                    var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                    var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();

                    if (txtPullQty != "") {

                        var txtLotOrSerailNumber = "";
                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                            var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                        var vSerialNumber = "";
                        var vLotNumber = "";
                        var vExpiration = "";

                        if (hdnSerialNumberTracking == "true")
                            vSerialNumber = txtLotOrSerailNumber;
                        if (hdnLotNumberTracking == "true")
                            vLotNumber = txtLotOrSerailNumber;
                        if (hdnDateCodeTracking == "true")
                            vExpiration = hdnExpiration;
                    }


                    creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });

                    if (strpullobj.SerialNumberTracking)
                        ItemTracking = "SERIALTRACK";
                    else if (strpullobj.LotNumberTracking)
                        ItemTracking = "LOTTRACK";
                    else if (strpullobj.DateCodeTracking)
                        ItemTracking = "DATECODETRACK";
                });

                var Creditobj = {};
                Creditobj = {
                    'ItemGuid': strpullobj.ItemGUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                    'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                    'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                    'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                    'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                    'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                    'SupplierAccountGuid' : strpullobj.SupplierAccountGuid,
                    'PrevPullsToCredit': creditarrObj
                };
                if (gblPreCreditObjToSaveForCredit == null) {
                    gblPreCreditObjToSaveForCredit = new Array();
                }
                gblPreCreditObjToSaveForCredit.push(Creditobj);
            });

            if (gblPreCreditObjToSaveForCredit != null && gblPreCreditObjToSaveForCredit.length > 0) {
                ShowHideCreditProgressBar(true);
                setTimeout(function () {
                    SaveCreditData(gblPreCreditObjToSaveForCredit)
                }, 100);
            }
        }
        else {
            alert(ErrorMessage);
        }
    });

    $("#divPreCreditInforSerialLot").off("click", "input[type='button'][name='btnCancelPullPopup']");
    $("#divPreCreditInforSerialLot").on("click", "input[type='button'][name='btnCancelPullPopup']", function () {
        DeletedRowObject = "";
        $("#divPreCreditInforSerialLot").empty();
        $('#divPreCreditInforSerialLot').dialog('close');
    });

    $("#divPreCreditInforSerialLot").off("change", "input[type='text'][name='txtPullQty']");
    $("#divPreCreditInforSerialLot").on("change", "input[type='text'][name='txtPullQty']", function () {
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
        $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos).PullQuantity = $(this).val();
    });

    $("#divPreCreditInforSerialLot").off("tap click", ".tbl-item-pull tbody tr");
    $("#divPreCreditInforSerialLot").on("tap click", ".tbl-item-pull tbody tr", function (e) {
        if (e.target.type == "checkbox" || e.target.type == "radio" || e.target.type == "text") {
            e.stopPropagation();
        }
        else if (e.currentTarget.getElementsByTagName("input").btnLoad != undefined) {
            e.stopPropagation();
        }
        else {
            if ((e.metaKey || e.ctrlKey)) {
                $(this).toggleClass('row_selected');
            } else {
                $(this).toggleClass('row_selected');
            }
        }
        return false;
    });

    $("#divPreCreditInforSerialLot").off('click', "input[type='button'][name='btnDeleteLots']");
    $("#divPreCreditInforSerialLot").on('click', "input[type='button'][name='btnDeleteLots']", function (e) {
        
        var vItemGUID = $(this).prop("id").split('_')[1];        

        var dtID = "#tblItemPull_" + vItemGUID;

        var TotalRows = $(dtID + ' tbody tr').length;
        var SelectedRows = $(dtID + ' tbody tr.row_selected').length;
        var RemainingRows = TotalRows - SelectedRows;

        if (SelectedRows <= 0) {
            alert(MsgSelectRowToDelete);
        }
        else {
            if (RemainingRows >= 1) {                

                $(dtID).find("tbody").find("tr.row_selected").each(function (index, tr) {
                    
                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                    if (txtPullQty == "")
                        txtPullQty = "0";

                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedRowObject, txtLotOrSerailNumber))
                            DeletedRowObject = DeletedRowObject + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                    }
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedRowObject, hdnExpiration))
                            DeletedRowObject = DeletedRowObject + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                        if (hdnExpiration != undefined)
                            DeletedRowObject = DeletedRowObject + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedRowObject, hdnBinNumber))
                            DeletedRowObject = DeletedRowObject + hdnBinNumber + "_" + txtPullQty + ",";
                    }
                    $(tr).remove();
                });

                var strIds = "";
                $(dtID).find("tbody").find("tr").each(function (index, tr) {
                    
                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                    if (txtPullQty == "")
                        txtPullQty = "0";

                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, txtLotOrSerailNumber))
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                    }
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnExpiration))
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                        if (hdnExpiration != undefined)
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnBinNumber))
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                    }

                });

                $("#hdnCurrentDeletedLoadedIds_" + vItemGUID).val(DeletedRowObject);
                $("#hdnCurrentLoadedIds_" + vItemGUID).val(strIds);
                isDeleteSrLotRow = true;
                var dtThisItem = $(dtID).dataTable();
                dtThisItem.fnStandingRedraw();
            }
            else {
                alert(MsgRowShouldExists);
            }
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

function OpenPreCreditInfoDialogForSerailLot(PullInfo) {     

    $("#divPreCreditInforSerialLot").dialog({
        autoOpen: true,
        modal: true,
        draggable: true,
        resizable: true,
        width: '70%',
        height: 500,
        title: "Credit",
        open: function () {

            setTimeout(function () {
            
            $.ajax({
                type: "POST",
                url: "/Pull/PullLotSrSelectionForCredit",
                contentType: 'application/json',
                dataType: 'html',
                data: JSON.stringify(PullInfo),
                success: function (RetData) {
                    
                    //$("#divPreCreditInforSerialLot").html("");
                    $("#divPreCreditInforSerialLot").html(RetData);
                    //$("#divPreCreditInforSerialLot").dialog('open');
                    $('#DivLoading').hide();
                },
                error: function (response) {
                    $(tr).css('background-color', 'Red');
                    IsGlobalErrorMsgShow = true;
                    errorMsg += "<b style='color:Red'>" + PullInfo[0].ItemNumber + " :  " + MsgErrorInProcess +"</b><br>"
                }
            });
            }, 100);
        },
        close: function () {
            DeletedRowObject = "";
            $('#divPreCreditInforSerialLot').empty();
            ShowHideCreditProgressBar(false);
        }
    });
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
            if (result.Status == "Fail" || result.Status == false) {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").html(result.Message);
            }
            gblPreCreditObjToSave = null;
            gblPreCreditObjToSaveForCredit = null;
            ShowHideCreditProgressBar(false);
            if (result.Status == true) {
                SaveCreditEditableUDF(arr);                
                RefreshPullNarrowSearch();//DoNarrowSearchIM(); // Maintain narrow search value - WI-329
                NSForItemModel_ExecuteOnDocReady(); 
                $.each(result.RetData, function (indx, RetDataItem) {                    
                    var $divItemRet2 = $('#divItem_' + RetDataItem);
                    $divItemRet2.attr('style', '');
                    $divItemRet2.remove();
                });                
                if ($('div[id^="divItem_"]').length <= 0) {
                    $('#divPreCreditInforSerialLot').dialog('close');  
                    $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                }
            }
        },
        error: function (xhr) {
            ShowHideCreditProgressBar(false);
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgSaveCreditAjaxError);
        }

    });
}

function SaveCreditEditableUDF(arr) {
    
    $.ajax({
        "url": "/Pull/SaveCreditEditableUDF",
        "data": JSON.stringify(arr),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "json",
        "contentType": "application/json",
        success: function (result) {
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
            alert(MsgPreCreditInfoAjaxError);
        }

    });
}

function CreditItems(obj) {
    DeletedRowObject = "";
    ShowHideCreditProgressBar(true);
    if ($('#NewPullAction').val() == "Pull") {
        ShowHideCreditProgressBar(false);
        return false;
    }

    SelectedPreCreditObj = new Array();
    gblPreCreditObjToSave = new Array();
    gblPreCreditObjToSaveForCredit = new Array();
    SelectedPreQLCreditObj = new Array();
    var $objCredit = $(obj);

    var tr = $(obj).parent().parent();

    var itemType = _creditPull.getspnOrderItemType($(tr)); //$(tr).find('#spnOrderItemType').text();
    var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(tr)[0]);
    var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);

    var creditQty = $(tr).find('#txtQty').val();

    if (isNaN(parseFloat(creditQty)) || parseFloat(creditQty) <= 0) {
        showNotificationDialog();
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgQuantityToCreditValid.replace("{0}", aData.ItemNumber));
        ShowHideCreditProgressBar(false);
        return false;
    }

    var itemGuid = null,
        qlGuid = null,
        bin = '',
        itemTrack = '',
        project = '',
        itemNumber = aData.ItemNumber,
        pullOrderNumber = "";

    var $tr = $(tr);

    if ($("#chkUsePullCommonUDF").is(":checked")) {
        if ($('#txtProjectSpentCommon').val() != '') {
            project = $('#txtProjectSpentCommon').val();
        }
    }
    else if ($tr.find('#txtProjectSpent').length > 0) {
        project = $tr.find('#txtProjectSpent').val();
    }
    if ($("#chkUsePullCommonUDF").is(":checked")) {
        if ($('#txtPullOrderNumberCommon').val() != '') {
            pullOrderNumber = $('#txtPullOrderNumberCommon').val();
        }
    }
    else if ($tr.find('#txtPullOrderNumber') != null) {
        if ($tr.find('#txtPullOrderNumber').attr("class") == 'selectBox') {
            pullOrderNumber = $tr.find('#txtPullOrderNumber option:selected').text();
        }
        else {
            pullOrderNumber = $tr.find('#txtPullOrderNumber').val();
        }
    }
    if ($tr.find('#txtBinNumber').length > 0) {
        bin = $tr.find('#txtBinNumber').val();
    }

    var IsIgnoreCreditRule = false;
    
    if (!_creditPull.isNewConsumePullObj()) {
        if ($tr.find('#spnItemGUID').length > 0) {
            itemGuid = $tr.find('#spnItemGUID').text();
        }
        if ($tr.find('#spnQuickListGUID').length > 0) {
            qlGuid = $tr.find('#spnQuickListGUID').text();
        }
        if ($tr.find('#spnIsIgnoreCreditRule').length > 0) {
            IsIgnoreCreditRule = $tr.find('#spnIsIgnoreCreditRule').text();
        }
    }
    else {
        var itemData = _creditPull.getItemDataFromRow($tr);
        itemGuid = itemData.spnItemGUID;
        qlGuid = itemData.spnQuickListGUID;
        IsIgnoreCreditRule = itemData.spnIsIgnoreCreditRule;
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

    var udf1 = GetUDFValue('UDF1', $tr);
    var udf2 = GetUDFValue('UDF2', $tr);
    var udf3 = GetUDFValue('UDF3', $tr);
    var udf4 = GetUDFValue('UDF4', $tr);
    var udf5 = GetUDFValue('UDF5', $tr);
    if ($("#chkUsePullCommonUDF").is(":checked")) {
        udf1 = GetUDFValue('UDF1PullCommon', null)
        udf2 = GetUDFValue('UDF2PullCommon', null)
        udf3 = GetUDFValue('UDF3PullCommon', null)
        udf4 = GetUDFValue('UDF4PullCommon', null)
        udf5 = GetUDFValue('UDF5PullCommon', null)
    }
    
    var vPullSupplierAccountNumber = "";
    if ($(tr).find('#hdnSupplierAccountNumber') != null) {
        vPullSupplierAccountNumber = $(tr).find('#hdnSupplierAccountNumber').val();
    }

    //alert(project);
    var obj1 = {
        'RowID': 1, 'ItemGuid': itemGuid, 'QLGuid': qlGuid, 'ItemType': itemType,
        'Bin': bin, 'ProjectName': project, 'Quantity': creditQty, 'ItemTracking': itemTrack,
        'UDF1': udf1, 'UDF2': udf2, 'UDF3': udf3, 'UDF4': udf4, 'UDF5': udf5, 'ItemNumber': itemNumber,
        'WOGuid': vWorkOrderDetailGUID, 'PullOrderNumber': pullOrderNumber,
        'SupplierAccountGuid': vPullSupplierAccountNumber
    };

    if (itemType == 2) {
        SelectedPreQLCreditObj.push(obj1);
    }
    else {
        SelectedPreCreditObj.push(obj1);
    }

    
    if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
        var creditarrObj = new Array();
        creditarrObj.push({ 'Serial': null, 'Lot': null, 'ExpireDate': null, 'Qty': creditQty });
        var Creditobj = {};
        Creditobj = {
            'ItemGuid': itemGuid, 'WOGuid': vWorkOrderDetailGUID,
            'ItemType': itemType, 'Bin': bin,
            'ProjectName': project, 'Quantity': creditQty,
            'UDF1': udf1, 'UDF2': udf2, 'UDF3': udf3,
            'UDF4': udf4, 'UDF5': udf5, 'ItemTracking': itemTrack,
            'PrevPullQty': 0, 'ItemNumber': itemNumber, 'PullOrderNumber': pullOrderNumber,
            'SupplierAccountGuid': vPullSupplierAccountNumber,
            'PrevPullsToCredit': creditarrObj            
        };
        gblPreCreditObjToSave.push(Creditobj);
    } 

    if (SelectedPreQLCreditObj != null && SelectedPreQLCreditObj.length > 0) {
        AddQuickListToCredit(SelectedPreQLCreditObj, IsIgnoreCreditRule);
    }
    else if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false)
    {
        OpenPreCreditInfoDialog();
    }
    else if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
        if (itemTrack == "SERIALTRACK"
            || itemTrack == "LOTTRACK"
            || itemTrack == "DATECODETRACK") {
            
            var ItemID = 0;

            if (!_creditPull.isNewConsumePullObj()) {
                if ($tr.find('#spnItemID').length > 0) {
                    ItemID = $tr.find('#spnItemID').text();
                }
            }
            else {
                ItemID = _creditPull.getItemDataFromRow($tr).spnItemID;
            }

            var DefaultPullQuantity = 0;
            //if ($(tr).find('#hdnDPQ').length > 0) {
            //    DefaultPullQuantity = $(tr).find('#hdnDPQ').val() == "" ? 0 : $(tr).find('#hdnDPQ').val();
            //}
            DefaultPullQuantity = _creditPull.gethdnDPQ($tr);

            var BinID = null;
            //if ($(tr).find('#BinID').length > 0) {
            //    BinID = $(tr).find('#BinID').val() == "" ? 0 : $(tr).find('#BinID').val();
            //}
            BinID = _creditPull.getBinId($tr);

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#ProjectIDCommon') != undefined) {
                    vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
            }
            else {
                    vProjectID = "";
            }
            }
            else {
                //if ($(tr).find('#ProjectID').length > 0) {
                //    vProjectID = $(tr).find('#ProjectID').val() == "" ? "" : $(tr).find('#ProjectID').val();
                //}
                //else {
                //    vProjectID = "";
                //}
                vProjectID = _creditPull.GetProjectId($tr);
            }

            var PullInfo = new Array();
            PullInfo.push({
                ID: 0,
                ItemID: ItemID,
                ItemNumber: itemNumber,
                ItemType: itemType,
                PoolQuantity: creditQty,
                ItemGUID: itemGuid,
                BinID: BinID,
                BinNumber: bin,
                DefaultPullQuantity: DefaultPullQuantity,
                ProjectSpendGUID: vProjectID,
                ProjectSpendName: project,
                UDF1: udf1, UDF2: udf2, UDF3: udf3, UDF4: udf4, UDF5: udf5,
                WorkOrderDetailGUID: vWorkOrderDetailGUID,
                PullOrderNumber: pullOrderNumber,
                SupplierAccountGuid: vPullSupplierAccountNumber
            });

            OpenPreCreditInfoDialogForSerailLot(PullInfo);
            ShowHideCreditProgressBar(false);
        }
        else {
            if (gblPreCreditObjToSave != null && gblPreCreditObjToSave.length > 0) {
                setTimeout(function () {
                    SaveCreditData(gblPreCreditObjToSave)
                }, 100);
            }
        }
    }
}

$('#btnCreditAll').on('click', function (e) {
    DeletedRowObject = "";
    ShowHideCreditProgressBar(true);
    if ($('#NewPullAction').val() == "Pull") {
        ShowHideCreditProgressBar(false);
        return false;
    }

    SelectedPreCreditObj = new Array();
    gblPreCreditObjToSave = new Array();
    gblPreCreditObjToSaveForCredit = new Array();
    SelectedPreQLCreditObj = new Array();
    if ($('#ItemModeDataTable tbody tr.row_selected').length <= 0) {
        ShowHideCreditProgressBar(false);
        showNotificationDialog();
        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
        $("#spanGlobalMessage").html(MsgSelectAtleastOneRow);
        return false;
    }
    var IsIgnoreCreditRule = false;
    var PullInfo = new Array();
    $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {
        var itemType = _creditPull.getspnOrderItemType($(this)); //$(this).find('#spnOrderItemType').text();
        var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(this)[0]);
        var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);

        var creditQty = $(this).find('#txtQty').val();

        if (isNaN(parseFloat(creditQty)) || parseFloat(creditQty) <= 0) {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgQuantityToCreditValid.replace("{0}", aData.ItemNumber));
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


        if (!_creditPull.isNewConsumePullObj()) {
            if ($(this).find('#spnItemGUID').length > 0) {
                itemGuid = $(this).find('#spnItemGUID').text();
            }
            if ($(this).find('#spnQuickListGUID').length > 0) {
                qlGuid = $(this).find('#spnQuickListGUID').text();
            }
        }
        else {
            var itemData = _creditPull.getItemDataFromRow($(this));
            itemGuid = itemData.spnItemGUID;
            qlGuid = itemData.spnQuickListGUID;
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


        var vPullSupplierAccountNumber = "";
        if ($(this).find('#hdnSupplierAccountNumber') != null) {
            vPullSupplierAccountNumber = $(this).find('#hdnSupplierAccountNumber').val();
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
            'WOGuid': vWorkOrderDetailGUID, 'PullOrderNumber': pullOrderNumber, 'SupplierAccountGuid': vPullSupplierAccountNumber
        };
        //if ($(this).find('#spnIsIgnoreCreditRule').length > 0) {
        //    IsIgnoreCreditRule = $(this).find('#spnIsIgnoreCreditRule').text();
        //}
        IsIgnoreCreditRule = _creditPull.getspnIsIgnoreCreditRule($(this));

        if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {

            if (itemTrack == "SERIALTRACK"
               || itemTrack == "LOTTRACK"
               || itemTrack == "DATECODETRACK") {

                var ItemID = 0;
                if (!_creditPull.isNewConsumePullObj()) {
                    if ($(this).find('#spnItemID').length > 0) {
                        ItemID = $(this).find('#spnItemID').text();
                    }
                }
                else {
                    ItemID = _creditPull.getItemDataFromRow($(this)).spnItemID;
                }

                var DefaultPullQuantity = 0;
                //if ($(this).find('#hdnDPQ').length > 0) {
                //    DefaultPullQuantity = $(this).find('#hdnDPQ').val() == "" ? 0 : $(this).find('#hdnDPQ').val();
                //}
                DefaultPullQuantity = _creditPull.gethdnDPQ($(this));

                var BinID = null;
                //if ($(this).find('#BinID').length > 0) {
                //    BinID = $(this).find('#BinID').val() == "" ? 0 : $(this).find('#BinID').val();
                //}
                BinID = _creditPull.getBinId($(this));

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#ProjectIDCommon') != undefined)
                        vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                    else
                        vProjectID = "";
                }
                else {
                    //if ($(this).find('#ProjectID').length > 0) {
                    //    vProjectID = $(this).find('#ProjectID').val() == "" ? "" : $(this).find('#ProjectID').val();
                    //}
                    //else {
                    //    vProjectID = "";
                    //}
                    vProjectID = _creditPull.GetProjectId($(this));
                }
                PullInfo.push({
                    ID: 0,
                    ItemID: ItemID,
                    ItemNumber: itemNumber,
                    ItemType: itemType,
                    PoolQuantity: creditQty,
                    ItemGUID: itemGuid,
                    BinID: BinID,
                    BinNumber: bin,
                    DefaultPullQuantity: DefaultPullQuantity,
                    ProjectSpendGUID: vProjectID,
                    ProjectSpendName: project,
                    UDF1: udf1, UDF2: udf2, UDF3: udf3, UDF4: udf4, UDF5: udf5,
                    WorkOrderDetailGUID: vWorkOrderDetailGUID,
                    PullOrderNumber: pullOrderNumber,
                    SupplierAccountGuid: vPullSupplierAccountNumber
                });
            }
            else {
                var creditarrObj = new Array();
                creditarrObj.push({ 'Serial': null, 'Lot': null, 'ExpireDate': null, 'Qty': creditQty });
                var Creditobj = {};
                Creditobj = {
                    'ItemGuid': itemGuid, 'WOGuid': vWorkOrderDetailGUID,
                    'ItemType': itemType, 'Bin': bin,
                    'ProjectName': project, 'Quantity': creditQty,
                    'UDF1': udf1, 'UDF2': udf2, 'UDF3': udf3,
                    'UDF4': udf4, 'UDF5': udf5, 'ItemTracking': itemTrack,
                    'PrevPullQty': 0, 'ItemNumber': itemNumber, 'PullOrderNumber': pullOrderNumber,
                    'SupplierAccountGuid': vPullSupplierAccountNumber,
                    'PrevPullsToCredit': creditarrObj
                };
                gblPreCreditObjToSave.push(Creditobj);
            }
        }

        if (itemType == 2) {
            SelectedPreQLCreditObj.push(obj);
        }
        else {
            SelectedPreCreditObj.push(obj);
        }
    });

    if (SelectedPreQLCreditObj != null && SelectedPreQLCreditObj.length > 0) {
        AddQuickListToCredit(SelectedPreQLCreditObj, IsIgnoreCreditRule);
    }
    else if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
        OpenPreCreditInfoDialog();
    }
    else if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
        if (PullInfo != null && PullInfo.length > 0) {

            if (gblPreCreditObjToSave != null && gblPreCreditObjToSave.length > 0) {
                $.ajax({
                    "url": "/Pull/SavePullCredit",
                    "data": JSON.stringify(gblPreCreditObjToSave),
                    "type": 'POST',
                    "async": false,
                    "cache": false,
                    "dataType": "json",
                    "contentType": "application/json",
                    success: function (result) {
                        if (result.Status == true) {
                            SaveCreditEditableUDF(gblPreCreditObjToSave);
                            OpenPreCreditInfoDialogForSerailLot(PullInfo);
                            ShowHideCreditProgressBar(false);
                        }
                        else
                        {
                            gblPreCreditObjToSave = null;
                            gblPreCreditObjToSaveForCredit = null;
                            alert(result.Message);
                            ShowHideCreditProgressBar(false);
                        }

                        //alert(result.Message);
                        //ShowHideCreditProgressBar(false);
                        //if (result.Status == true) {
                        //    gblPreCreditObjToSave = null;
                        //    gblPreCreditObjToSaveForCredit = null;
                        //    $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                        //    $('#divPreCreditInforSerialLot').dialog('close');
                        //}
                    },
                    error: function (xhr) {
                        gblPreCreditObjToSave = null;
                        gblPreCreditObjToSaveForCredit = null;
                        ShowHideCreditProgressBar(false);
                        alert(MsgSaveCreditAjaxError);
                    }
                });
            }
            else
            {
                OpenPreCreditInfoDialogForSerailLot(PullInfo);
                ShowHideCreditProgressBar(false);
            }
        }
        else {
            if (gblPreCreditObjToSave != null && gblPreCreditObjToSave.length > 0) {
                setTimeout(function () {
                    SaveCreditData(gblPreCreditObjToSave)
                }, 100);
            }
        }
    }
});

function AddQuickListToCredit(SelectedPreQLCreditObj, IsIgnoreCreditRule) {    
    gblPreCreditObjToSave = new Array();   
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
            if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false)
            {
                OpenPreCreditInfoDialog();
            }
            else if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
                var PullInfo = new Array();
                for (var i = 0; i < SelectedPreCreditObj.length; i++) {
                    if (SelectedPreCreditObj[i].ItemTracking == "SERIALTRACK"
                       || SelectedPreCreditObj[i].ItemTracking == "LOTTRACK"
                       || SelectedPreCreditObj[i].ItemTracking == "DATECODETRACK") {                        
                        PullInfo.push({
                            ID: 0,
                            ItemID: 0,
                            ItemNumber: SelectedPreCreditObj[i].ItemNumber,
                            ItemType: SelectedPreCreditObj[i].ItemType,
                            PoolQuantity: SelectedPreCreditObj[i].Quantity,
                            ItemGUID: SelectedPreCreditObj[i].ItemGuid,
                            BinID: 0,
                            BinNumber: SelectedPreCreditObj[i].Bin,
                            DefaultPullQuantity: SelectedPreCreditObj[i].ItemQtyInQL,
                            ProjectSpendGUID: 0,
                            ProjectSpendName: SelectedPreCreditObj[i].ProjectName,
                            UDF1: SelectedPreCreditObj[i].UDF1,
                            UDF2: SelectedPreCreditObj[i].UDF2,
                            UDF3: SelectedPreCreditObj[i].UDF3,
                            UDF4: SelectedPreCreditObj[i].UDF4,
                            UDF5: SelectedPreCreditObj[i].UDF5,
                            WorkOrderDetailGUID: SelectedPreCreditObj[i].WOGuid,
                            PullOrderNumber: SelectedPreCreditObj[i].PullOrderNumber,
                            SupplierAccountGuid: SelectedPreCreditObj[i].SupplierAccountGuid
                        });

                        //OpenPreCreditInfoDialogForSerailLot(PullInfo);
                        //ShowHideCreditProgressBar(false);
                    }
                    else {
                        var creditarrObj = new Array();
                        creditarrObj.push({ 'Serial': null, 'Lot': null, 'ExpireDate': null, 'Qty': SelectedPreCreditObj[i].Quantity });
                        var Creditobj = {};
                        Creditobj = {
                            'ItemGuid': SelectedPreCreditObj[i].ItemGuid, 'WOGuid': SelectedPreCreditObj[i].WOGuid,
                            'ItemType': SelectedPreCreditObj[i].ItemType, 'Bin': SelectedPreCreditObj[i].Bin,
                            'ProjectName': SelectedPreCreditObj[i].ProjectName, 'Quantity': SelectedPreCreditObj[i].Quantity,
                            'UDF1': SelectedPreCreditObj[i].UDF1, 'UDF2': SelectedPreCreditObj[i].UDF2, 'UDF3': SelectedPreCreditObj[i].UDF3,
                            'UDF4': SelectedPreCreditObj[i].UDF4, 'UDF5': SelectedPreCreditObj[i].UDF5,
                            'ItemTracking': SelectedPreCreditObj[i].ItemTracking,
                            'PrevPullQty': 0,
                            'ItemNumber': SelectedPreCreditObj[i].ItemNumber,
                            'PullOrderNumber': SelectedPreCreditObj[i].PullOrderNumber,
                            'SupplierAccountGuid' : SelectedPreCreditObj[i].SupplierAccountGuid,
                            'PrevPullsToCredit': creditarrObj
                        };
                        gblPreCreditObjToSave.push(Creditobj);
                    }
                }
                if (PullInfo != null && PullInfo.length > 0) {

                    if (gblPreCreditObjToSave != null && gblPreCreditObjToSave.length > 0) {
                        $.ajax({
                            "url": "/Pull/SavePullCredit",
                            "data": JSON.stringify(gblPreCreditObjToSave),
                            "type": 'POST',
                            "async": false,
                            "cache": false,
                            "dataType": "json",
                            "contentType": "application/json",
                            success: function (result) {
                                if (result.Status == true) {
                                    SaveCreditEditableUDF(gblPreCreditObjToSave);
                                    OpenPreCreditInfoDialogForSerailLot(PullInfo);
                                    ShowHideCreditProgressBar(false);
                                }
                                else {
                                    gblPreCreditObjToSave = null;
                                    gblPreCreditObjToSaveForCredit = null;
                                    alert(result.Message);
                                    ShowHideCreditProgressBar(false);
                                }
                            },
                            error: function (xhr) {
                                gblPreCreditObjToSave = null;
                                gblPreCreditObjToSaveForCredit = null;
                                ShowHideCreditProgressBar(false);
                                alert(MsgSaveCreditAjaxError);
                            }
                        });
                    }
                    else {
                        OpenPreCreditInfoDialogForSerailLot(PullInfo);
                        ShowHideCreditProgressBar(false);
                    }
                }
                else {
                    if (gblPreCreditObjToSave != null && gblPreCreditObjToSave.length > 0) {
                        setTimeout(function () {
                            SaveCreditData(gblPreCreditObjToSave)
                        }, 100);
                    }
                }
            }           
        },
        error: function (xhr) {
            alert(MsgPreCreditInfoAjaxError);
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

/////////////// New added for allow More Credit for Serial,lot and date code ////////////////

function ValidateSinglePullForCredit(vItemGUID) {
    var IsIgnoreCreditRule = false;  
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    var dtID = "#tblItemPull_" + vItemGUID;

    var SpanQty = $("#divPreCreditInforSerialLot").find("#txtPoolQuantity_" + vItemGUID);

    var TotalEntered = 0;
    $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {
        var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
        var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();
        
        IsIgnoreCreditRule = $(tr).find("#hdnIsIgnoreCreditRule").val();
       
        var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
        var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
        var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

        if (hdnLotNumberTracking == "true") {
            var txtLotNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
            if (txtLotNumber == "" || txtLotNumber == null)
            {
                errormsg = "\n" + MsgEnterLotNumber;
                return errormsg;
            }
        }

        if (hdnSerialNumberTracking == "true")
        {
            var txtSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
            if (txtSerailNumber == "" || txtSerailNumber == null) {
                errormsg = "\n" + MsgSerialNumberValidation;
                return errormsg;
            }
        }

        if (hdnDateCodeTracking == "true") {
            var txtExpiration = $.trim($(tr).find("input[type='text'][name^='txtExpirationDate']").val());
            if (txtExpiration == "" || txtExpiration == null) {
                errormsg = "\n" + MsgEnterExpirationDate;
                return errormsg;
            }
        }

        if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
            if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
                errormsg = "\n" + MsgPullMoreQuantityValidation;
                isMoreQty = true;
                return errormsg;
            }
        }

        TotalEntered = TotalEntered + parseFloat(txtPullQty);
    });

    if (isMoreQty == false) {
        var pullQty = parseFloat($(SpanQty).val().toString());        
        if (TotalEntered != pullQty) {
            errormsg = errormsg + "\n" + MsgEnteredPullQuantityValidation.replace("{0}", TotalEntered).replace("{1}", pullQty);
        }
    }
    else {
        if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
            errormsg = MsgPullMoreQuantityValidation;
        }
    }

    return errormsg;
}

function ValidateAllPullForCredit() {
    //
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;

    var IsIgnoreCreditRule = false;    

    $("#divPreCreditInforSerialLot").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
        var ID = $(tblHeader).prop("id").split('_')[1];        
        var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + ID);       

        var TotalEntered = 0;
        if ($("#tblItemPull_" + ID).length > 0) {
            $("#tblItemPull_" + ID).find("tbody").find("tr").each(function (index, tr) {
                if ($(tr).find("input[type='text'][name='txtPullQty']").length > 0) {

                    
                    IsIgnoreCreditRule = $(tr).find("#hdnIsIgnoreCreditRule").val();

                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                    var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                    if (hdnLotNumberTracking == "true") {
                        var txtLotNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotNumber == "" || txtLotNumber == null) {
                            errormsg = "\n" + MsgEnterLotNumber;
                            return errormsg;
                        }
                    }

                    if (hdnSerialNumberTracking == "true") {
                        var txtSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtSerailNumber == "" || txtSerailNumber == null) {
                            errormsg = "\n" + MsgSerialNumberValidation;
                            return errormsg;
                        }
                    }

                    if (hdnDateCodeTracking == "true") {
                        var txtExpiration = $.trim($(tr).find("input[type='text'][name^='txtExpirationDate']").val());
                        if (txtExpiration == "" || txtExpiration == null) {
                            errormsg = "\n" + MsgEnterExpirationDate;
                            return errormsg;
                        }
                    }

                    if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
                        if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
                            errormsg = "\n" + MsgPullMoreQuantityValidation;
                            isMoreQty = true;
                            return errormsg;
                        }
                    }

                    TotalEntered = TotalEntered + parseFloat(txtPullQty);
                }
            });

            //
            if (isMoreQty == false) {
                var pullQty = parseFloat($(SpanQty).val().toString());
                if (TotalEntered != pullQty) {
                    ////errormsg = errormsg + "\nentered :" + TotalEntered + "\tPull Qty :" + pullQty;
                    errormsg = errormsg + "\n"+ MsgEnteredPullQuantityValidation.replace("{0}", TotalEntered).replace("{1}", pullQty);
                }
            }
            else {
                if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
                    errormsg = MsgPullMoreQuantityValidation;
                }
            }
        }
    });

    return errormsg;
}

function PreparePullDataTableForSerialLot(objPullItemDTO) {
    
    var columnarrIL = new Array();
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            if (objPullItemDTO.ViewRight == "ViewOverwrite") {
                var strReturn = "<span style='position:relative'>";
                strReturn = strReturn + "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumber' name='txtLotOrSerailNumber' class='text-boxinner95 AutoSerialLot' />";
                strReturn = strReturn + '<a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsSL" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
                return strReturn;
            }
            else if (objPullItemDTO.ViewRight == "NoRight") // && IsCheckViewRight == false)
            {
                var strReturn = "<span style='position:relative'>";
                strReturn = strReturn + "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberNoRight' name='txtLotOrSerailNumberNoRight' class='text-boxinner' />";
                return strReturn;
            }
            else if (objPullItemDTO.ViewRight == "ViewOnly") {
                var strReturn = "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberViewOnly' name='txtLotOrSerailNumberViewOnly' class='text-boxinner' />";
                return strReturn;
            }
            else {
                var strReturn = "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberViewOnly' name='txtLotOrSerailNumberViewOnly' class='text-boxinner' />";
                return strReturn;
            }
        }
    });   
    //columnarrIL.push({
    //    mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
    //        var strReturn = "<span name='spnBinNumber' id='spnBinNumber_" + obj.aData.ID + "'>" + (obj.aData.BinNumber == "[|EmptyStagingBin|]" ? "" : obj.aData.BinNumber) + "</span>";
    //        return strReturn;
    //    }
    //});
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strReturn = "<span name='spnLotSerialQuantity' id='spnLotSerialQuantity_" + obj.aData.ID + "'>" + obj.aData.LotSerialQuantity + "</span>";
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            
            var strReturn = "<input type='hidden' name='hdnRowUniqueId' value='" + obj.aData.ID + "_" + obj.aData.ItemGUID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnLotNumberTracking' value='" + obj.aData.LotNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnSerialNumberTracking' value='" + obj.aData.SerialNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnDateCodeTracking' value='" + obj.aData.DateCodeTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpiration' value='" + obj.aData.Expiration + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpirationDate' value='" + obj.aData.strExpirationDate + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnBinNumber' id='hdnBinNumber' value='" + obj.aData.BinNumber + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnBinIDValue' id='hdnBinIDValue' value='" + obj.aData.BinID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnIsIgnoreCreditRule' id='hdnIsIgnoreCreditRule' value='" + objPullItemDTO.IsIgnoreCreditRule + "' />";

            if (objPullItemDTO.SerialNumberTracking == BoolTrueString) {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtPullQty_" + obj.aData.ID + "' name='txtPullQty' class='text-boxinner pull-quantity' readonly='readonly' />";
            }
            else {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtPullQty_" + obj.aData.ID + "' name='txtPullQty' class='text-boxinner pull-quantity numericinput' />";
            }
            return strReturn;
        }
    });
    columnarrIL.push({ mDataProp: "Received", sClass: "read_only" });    
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strReturn = "<input type='text' value='" + obj.aData.Expiration + "' id='txtExpirationDate_" + obj.aData.KitDetailGUID + "' name='txtExpirationDate' class='text-boxinner dateTextbox txtExpiration' readonly='readonly' />";
            return strReturn;
        }
    });

    //alert(JSON.stringify(columnarrIL));

    var Curtable = $('#' + objPullItemDTO.tableID).dataTable({
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": false,
        "bInfo": false,
        "bAutoWidth": false,
        "sScrollX": "100%",
        "bRetrieve": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": columnarrIL,        
        "sAjaxSource": PullLotSrSelectionForCreditPopupUrl,
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {          
            if (aData.IsConsignedLotSerial == true) {
                nRow.className = "even trconsigned";
            }
        },
        "fnInitComplete": function (oSettings) {            
            var strAllSelected = "";

            $("#hdnSelectedId_" + objPullItemDTO.ItemGUID).val();
            if (objPullItemDTO.LotNumberTracking != BoolTrueString && objPullItemDTO.SerialNumberTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(0, false);
            }
            if (objPullItemDTO.DateCodeTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(4, false);
            }
        },
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            
            aoData.push({ "name": "ItemGUID", "value": objPullItemDTO.ItemGUID });
            aoData.push({ "name": "BinID", "value": objPullItemDTO.BinID });
            aoData.push({ "name": "BinNumber", "value": objPullItemDTO.BinNumber });
            if (objPullItemDTO.ItemGUID != '00000000-0000-0000-0000-000000000000' && objPullItemDTO.ItemGUID != '')
                aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ItemGUID).val(), 2) });
            else
                aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ToolGUID).val(), 2) });
            aoData.push({ "name": "InventoryConsuptionMethod", "value": objPullItemDTO.InventoryConsuptionMethod });
            aoData.push({ "name": "CurrentLoaded", "value": $("#hdnCurrentLoadedIds_" + objPullItemDTO.ItemGUID).val() });
            aoData.push({ "name": "CurrentDeletedLoaded", "value": $("#hdnCurrentDeletedLoadedIds_" + objPullItemDTO.ItemGUID).val() });
            aoData.push({ "name": "ViewRight", "value": objPullItemDTO.ViewRight });
            aoData.push({ "name": "IsDeleteRowMode", "value": isDeleteSrLotRow });
            oSettings.jqXHR = $.ajax({
                dataType: 'json',
                type: "POST",
                url: sSource,
                cache: false,
                data: aoData,
                headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
                success: fnCallback,
                beforeSend: function () {
                    LenBeforeRebind = $('#' + objPullItemDTO.tableID).find("tbody").find("tr").length;
                    $('.dataTables_scroll').css({ "opacity": 0.2 });
                },
                complete: function () {
                    $('.dataTables_scroll').css({ "opacity": 1 });
                    isDeleteSrLotRow = false;
                    $('.ShowAllOptionsSL').click(function () {
                        $(this).siblings('.AutoSerialLot').trigger("focus");
                        $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                    });
                    

                    $('#' + objPullItemDTO.tableID).each(function (i) {
                        
                        $(this).find('input.dateTextbox').removeClass('hasDatepicker');
                        var currentDatePicker = $(this);
                        $(this).find('input.dateTextbox').datepicker({
                            dateFormat: RoomDateJSFormat, showButtonPanel: true,
                            clearText: 'Clear', onClose: function () { this.focus(); }
                        });
                    });                    

                    if (objPullItemDTO.ViewRight == "ViewOnly") {
                        $("input[type='text'][name='txtLotOrSerailNumberViewOnly']").keypress(function () {
                            return false;
                        });

                        $("#divPreCreditInforSerialLot input[type='text'][name='txtPullQty']").keypress(function () {
                            return false;
                        });
                    }

                    LenAfterRebind = $('#' + objPullItemDTO.tableID).find("tbody").find("tr").length;
                    if (LenBeforeRebind == LenAfterRebind && IsLoadMoreLotsClicked == true) {
                        alert(MsgNoLocationToAdd);
                    }
                    IsLoadMoreLotsClicked = false;
                }
            });
        }
    });
}

/////////////// New added for allow More Credit for Serial,lot and date code ////////////////