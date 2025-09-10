
var _msCreditPull = (function ($) {
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

    self.getItemDataFromRow = function ($tr) {
        var itemData = null;
        if (self.isNewConsumePullObj()) {
            itemData = _NewConsumePull.getItemDataFromRow($tr);
        }
        return itemData;
    };

    self.getspnItemID = function ($tr) {
        var val = 0;
        if (self.isNewConsumePullObj()) {
            var itemData = self.getItemDataFromRow($tr);
            val = itemData.spnItemID;
        }
        else {
            if ($tr.find('#spnItemID').length > 0) {
                val = $tr.find('#spnItemID').text();
            }
        }
        return val;
    };

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
    };

    self.getMaterialStagingGuid = function ($tr) {
        var val = false;
        if (self.isNewConsumePullObj()) {
            var itemData = _NewConsumePull.getItemDataFromRow($tr);
            val = itemData.hdnIsLoadMoreStaging;
        }
        else {
            if ($tr.find('#hdnIsLoadMoreStaging').length > 0) {
                val = $tr.find('#hdnIsLoadMoreStaging').text();
            }
        }
        return val;
    };

    return self;

})(jQuery); //msCreditPull end


var SelectedMSPreCreditObj;
var gblMSPreCreditObjToSave;
var gblMSPreCreditObjToSaveForCredit;
var SelectedMSPreQLCreditObj;
var DeletedMSRowObject = "";
$(document).ready(function () {
    $("#THStagingHeader").hide();
    $("#TDStagingHeader").hide();
    var IsIgnoreCreditRule = false;

    $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {
        IsIgnoreCreditRule = _msCreditPull.getspnIsIgnoreCreditRule($(this)); //$(this).find('#spnIsIgnoreCreditRule').text();
    });
    var ItemGuid = null,
         WOGuid = null,
        ItemType = null,
        Bin = null,
        ProjectName = null,
        Quantity = null,
        UDF1 = null,
        UDF2 = null,
        UDF3 = null,
        UDF4 = null,
        UDF5 = null,
        itemTrack = null,
        PrevPullQty = null,
        ItemNumber = null,
        PullOrderNumber = null,
        MSPullStagingHeader = null;

    if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
        $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {

            var $tr = $(this);

            if (!_msCreditPull.isNewConsumePullObj()) {
                ItemType = $tr.find('#spnOrderItemType').text();
                ItemNumber = $tr.find('#spnItemNumber').text();
                if ($tr.find('#spnItemGUID').length > 0) {
                    ItemGuid = $tr.find('#spnItemGUID').text();
                }
                if ($tr.find('#spnQuickListGUID').length > 0) {
                    qlGuid = $tr.find('#spnQuickListGUID').text();
                }
            }
            else {
                var itemData = _msCreditPull.getItemDataFromRow($tr);
                ItemType = itemData.spnOrderItemType;
                ItemNumber = itemData.spnItemNumber;
                ItemGuid = itemData.spnItemGUID;
                qlGuid = itemData.spnQuickListGUID;
            }

            Quantity = $tr.find('#txtQty').val();

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

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#MSPullStagingHeaderValue').val() != '') {
                    MSPullStagingHeader = $('#MSPullStagingHeaderValue').val();
                }
            }
            else if ($tr.find('#hdnIsLoadMoreStaging') != null) {
                if ($tr.find('#hdnIsLoadMoreStaging').attr("class") == 'selectBox') {
                    MSPullStagingHeader = $tr.find('#hdnIsLoadMoreStaging option:selected').text();
                }
                else {
                    MSPullStagingHeader = $tr.find('#hdnIsLoadMoreStaging').val();
                }
            }

            if ($tr.find('#txtBinNumber').length > 0) {
                bin = $tr.find('#txtBinNumber').val();
            }

            //if (!_msCreditPull.isNewConsumePullObj()) {
            //    if ($(this).find('#spnItemGUID').length > 0) {
            //        ItemGuid = $(this).find('#spnItemGUID').text();
            //    }
            //    if ($(this).find('#spnQuickListGUID').length > 0) {
            //        qlGuid = $(this).find('#spnQuickListGUID').text();
            //    }
            //}
            //else {
            //    var itemData = _msCreditPull.getItemDataFromRow($(this));
            //    ItemGuid = itemData.spnItemID;
            //    qlGuid = itemData.spnQuickListGUID;
            //}
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
            if ($tr.find('#hdnSupplierAccountNumber') != null) {
                vPullSupplierAccountNumber = $tr.find('#hdnSupplierAccountNumber').val();
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
                'PrevPullsToMSCredit': creditarrObj
            };

            gblMSPreCreditObjToSave.push(Creditobj);
        }); // each loop
    }

    var headerTitleMSCredit = "MS Credit";
    if (typeof MSCreditTitle != 'undefined') {
        headerTitleMSCredit = MSCreditTitle;
    }
    $('#divPreMSCreditInfo').dialog({
        autoOpen: false,
        modal: true,
        draggable: true,
        resizable: true,
        width: '50%',
        height: 500,
        title: headerTitleMSCredit,
        open: function () {           
            $('#divPreMSCreditInfo').empty();
            var objToFillPreCredit = $(this).data("MakeMSPreCredit");
            SelectedMSPreCreditObj = SelectedMSPreCreditObj.filter(function (el) {
                return (el.RowID !== objToFillPreCredit.RowID);
            });
            OpenPreMSCreditInfoPage(objToFillPreCredit);
        },
        close: function () {            
            $('#divPreMSCreditInfo').empty();
            ShowHideMSCreditProgressBar(true);
            OpenPreMSCreditInfoDialog();
        }
    });

    //$("#divPreMSCreditInforSerialLot").off('change', "input[type='text'][name^='txtMSStagingHeader']");
    //$("#divPreMSCreditInforSerialLot").on('change', "input[type='text'][name^='txtMSStagingHeader']", function (e)
    //{        
    //    var objCurtxt = $(this);
    //    var CurrentValue = $(objCurtxt).val();
    //    var vItemGUID = $(this).prop("id").split('_')[1];
    //    var BinID = $("#hdnBinIDValue").val();

    //    if ($.trim(CurrentValue) != null && $.trim(CurrentValue) != "")
    //    {
    //        $.ajax({
    //            type: "POST",
    //            url: SaveMaterialStagingUrl,
    //            contentType: 'application/json',
    //            dataType: 'json',
    //            data: "{ BinID: '" + BinID + "' , StagingName: '" + $.trim(CurrentValue) + "'}",
    //            success: function (result) {
    //                if (result.Status == "duplicate")
    //                {
    //                    alert("Staging Header is already available.");
    //                    $(objCurtxt).val("");
    //                    $(objCurtxt).focus();
    //                    return false;
    //                }
    //                else if (result.Status == "Success")
    //                {
    //                    if (result.MaterialStagingGuid != "")
    //                        $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val(result.MaterialStagingGuid);
    //                    if (result.MaterialStagingID != "")
    //                        $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingID").val(result.MaterialStagingID);
    //                }
    //            },
    //            error: function (err) {
    //            }
    //        });
    //    }
    //});

    $("#divPreMSCreditInforSerialLot").off('focus', "input[type='text'][name^='txtStagingHeader']");
    $("#divPreMSCreditInforSerialLot").on('focus', "input[type='text'][name^='txtStagingHeader']", function (e)
    {       
        var objCurtxt = $(this);
        var CurrentValue = $(objCurtxt).val();
      
        if ($(this).hasClass("AutoStagingHeader")) {
            $(this).autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/Inventory/GetAllStagingHeaders',
                        contentType: 'application/json',
                        dataType: 'json',
                        data: { NameStartWith: request.term },
                        success: function (data) {
                            response($.map(data, function (item) {                               
                                return {
                                    label: item.StagingName,
                                    value: item.StagingName,
                                    selval: item.GUID,
                                    selID : item.ID
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
                    
                    $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val(ui.item.selval);
                    $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingID").val(ui.item.selID);
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                    $(this).autocomplete('widget').css('z-index', 9000);
                    $('ul.ui-autocomplete').css('overflow-y', 'auto');
                    $('ul.ui-autocomplete').css('max-height', '300px');
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                  //  $(objCurtxt).trigger("change");
                }
            });
        }
    });

    $("#divPreMSCreditInforSerialLot").off('change', "input.dateTextbox");
    $("#divPreMSCreditInforSerialLot").on('change', "input.dateTextbox", function (e) {
        
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
            var $tr = $(this);
            if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                if (i != row_id && CurrentSelectedLotNumberValue != "") {
                    //var tr = $(this);
                    var SelectedExpirationDate = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                    var SelectedLotNumber = $tr.find('#txtLotOrSerailNumber').val();
                    if (SelectedExpirationDate != "") {
                        if (SelectedExpirationDate == $(objCurtxt).val() && SelectedLotNumber == CurrentSelectedLotNumberValue) {
                            isDuplicateEntry = true;
                        }
                    }
                    if (SelectedLotNumber == CurrentSelectedLotNumberValue) {
                        isDuplicateEntry = true;
                    }
                }
            }
            else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "DateCodeTracking") {
                if (i != row_id) {
                    //var tr = $(this);
                    var SelectedExpirationDate = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                    if (SelectedExpirationDate == $(objCurtxt).val()) {
                        isDuplicateEntry = true;
                    }
                }
            }
        });

        if (isDuplicateEntry == true) {

            if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").html(MsgDuplicateLotNumberExpirationDate);
            }
            else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "DateCodeTracking") {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").html(MsgDuplicateExpirationDate);
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
                        alert(MsgCreditTransactionDone);
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

    $("#divPreMSCreditInforSerialLot").off('focus', "input.dateTextbox");
    $("#divPreMSCreditInforSerialLot").on('focus', "input.dateTextbox", function (e) {

        var objCurtxt = $(this);
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
        var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

        var dtItemPull = "#tblItemPull_" + ids[1].toString();
        var strSerialLotNos = "";

        $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {

            var $tr = $(tr);

            if (index != aPos) {
                var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();

                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                    var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtLotOrSerailNumber != undefined) {
                        strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                    }
                }
                if (hdnDateCodeTracking == "true") {
                    var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                    if (hdnExpiration != undefined) {
                        strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                    }
                }
                else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false") {
                    var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                    if (hdnBinNumber != undefined) {
                        strSerialLotNos = strSerialLotNos + hdnBinNumber + "|#|";
                    }
                }
            }

        });
    });

    $("#divPreMSCreditInforSerialLot").off('change', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#divPreMSCreditInforSerialLot").on('change', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {
        
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
            var $tr = $(this);
            if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                if (i != row_id && $(objCurtxt).val() != "") {
                    //var tr = $(this);
                    var SelectedExpirationDate = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                    var SelectedLotNumber = $tr.find('#' + objCurtxt.prop("id")).val();
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
                    //var tr = $(this);
                    var SerialOrLotNumber = $tr.find('#' + objCurtxt.prop("id")).val();
                    if (SerialOrLotNumber == $(objCurtxt).val()) {
                        isDuplicateEntry = true;
                    }
                    else {
                        var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
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
                alert(DuplicateLotFound);
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
                            alert(MsgCreditTransactionDone);
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

    $("#divPreMSCreditInforSerialLot").off('focus', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#divPreMSCreditInforSerialLot").on('focus', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {

        var objCurtxt = $(this);
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

        var dtItemPull = "#tblItemPull_" + ids[1].toString();
        var strSerialLotNos = "";

        var BinID = $("#divPreMSCreditInforSerialLot").find("#hdnBinID_" + ids[1].toString()).val();

        $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {
            var $tr = $(tr);
            if (index != aPos) {
                var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();

                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                    var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtLotOrSerailNumber != undefined) {
                        strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                    }
                }
                if (hdnDateCodeTracking == "true") {
                    
                    var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                    if (hdnExpiration != undefined) {
                        strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                    }
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

    $("#divPreMSCreditInforSerialLot").off("click", "input[type='button'][name='btnLoadMoreLots']");
    $("#divPreMSCreditInforSerialLot").on("click", "input[type='button'][name='btnLoadMoreLots']", function () {
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
                var $tr = $(tr);
                BinNumber = $tr.find("input[name='hdnBinNumber']").val();
                var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();
                var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();

                
                if (hdnLotNumberTracking == "true") {
                    IsLotNumberTracking = true;
                }
                if (hdnSerialNumberTracking == "true") {
                    IsSerialNumberTracking = true;
                }
                if (hdnDateCodeTracking == "true") {
                    IsDateCodeTracking = true;
                }

                if (IsLotNumberTracking == true) {
                    var txtLotNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtLotNumber == "" || txtLotNumber == null) {
                        showNotificationDialog();
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        $("#spanGlobalMessage").html(EnterLotNumber);
                        isError = true;
                        return false;
                    }
                }
                if (IsSerialNumberTracking == true) {
                    var txtSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtSerailNumber == "" || txtSerailNumber == null) {
                        showNotificationDialog();
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        $("#spanGlobalMessage").html(MsgSerialNumberValidation);
                        isError = true;
                        return false;
                    }
                }
                if (IsDateCodeTracking == true) {
                    var txtExpiration = $.trim($tr.find("input[type='text'][name^='txtExpirationDate']").val());
                    if (txtExpiration == "" || txtExpiration == null) {
                        showNotificationDialog();
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        $("#spanGlobalMessage").html(MsgEnterExpirationDate);
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
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, txtLotOrSerailNumber)) {
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                        }
                    }
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnExpiration)) {
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                        if (hdnExpiration != undefined) {
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else {
                        var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnBinNumber)) {
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                        }
                    }
                }
            }); // loop

            if (isError == false) {
                $("#hdnCurrentLoadedIds_" + vItemGUID).val(strIds);
                var dt = $(dtID).dataTable();
                dt.fnStandingRedraw();
            }
        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(MsgPullCreditQuantity);
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

    $("#divPreMSCreditInforSerialLot").off("click", "input[type='button'][name='btnPullPopup']");
    $("#divPreMSCreditInforSerialLot").on("click", "input[type='button'][name='btnPullPopup']", function () {
        
        var vItemGUID = $(this).prop("id").split('_')[1];

        var dtID = "#tblItemPull_" + vItemGUID;

        var ArrItem = new Array();
        var arrItemDetails;
        var ErrorMessage = ValidateSinglePullForMSCredit(vItemGUID);

        if (ErrorMessage == "") {

            var itemGuid = vItemGUID;
            var SpanQty = $("#divPreMSCreditInforSerialLot").find("#txtPoolQuantity_" + vItemGUID);

            var strpullobj;
            if ($("#divPreMSCreditInforSerialLot").find("#hdnPullMasterDTO_" + vItemGUID).length > 0)
                strpullobj = JSON.parse($("#divPreMSCreditInforSerialLot").find("#hdnPullMasterDTO_" + vItemGUID).val());
            //if ($("input[type='hidden'][name^='hdnPullMasterDTO']").length > 0)
            //    strpullobj = JSON.parse($("input[type='hidden'][name^='hdnPullMasterDTO']").val());

            var BinID = $("#divPreMSCreditInforSerialLot").find("#hdnBinID_" + vItemGUID).val();
            var MainBinNumber = $("#divPreMSCreditInforSerialLot").find("#hdnMainBinNumber_" + vItemGUID).val();

            var objCurtxt = $("#divPreMSCreditInforSerialLot").find("#txtStagingHeader");
            var CurrentValue = $(objCurtxt).val();           
            var BinID = $("#hdnBinIDValue").val();

            if ($.trim(CurrentValue) != null && $.trim(CurrentValue) != "") {
                $.ajax({
                    type: "POST",
                    url: SaveMaterialStagingUrl,
                    contentType: 'application/json',
                    dataType: 'json',
                    data: "{ BinID: '" + BinID + "' , StagingName: '" + $.trim(CurrentValue) + "'}",
                    success: function (result) {                        
                        if (result.Status == "duplicate") {
                            alert(MsgStagingHeaderAvailable);
                            $(objCurtxt).val("");
                            $(objCurtxt).focus();
                            return false;
                        }
                        else if (result.Status == "Success") {
                            if (result.MaterialStagingGuid != "")
                                $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val(result.MaterialStagingGuid);
                            if (result.MaterialStagingID != "")
                                $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingID").val(result.MaterialStagingID);

                            var MaterialStagingGuid = $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val();
                            var MaterialStagingID = $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingID").val();

                            var creditarrObj = new Array();
                            $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {
                                var $tr = $(tr);
                                var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();

                                var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                                var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                                var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();

                                var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                                var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();

                                if (txtPullQty != "") {

                                    var txtLotOrSerailNumber = "";
                                    if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                                        txtLotOrSerailNumber = $tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                                    }

                                    var vSerialNumber = "",
                                        vLotNumber = "",
                                        vExpiration = "";

                                    if (hdnSerialNumberTracking == "true") {
                                        vSerialNumber = txtLotOrSerailNumber;
                                    }
                                    if (hdnLotNumberTracking == "true") {
                                        vLotNumber = txtLotOrSerailNumber;
                                    }
                                    if (hdnDateCodeTracking == "true") {
                                        vExpiration = hdnExpiration;
                                    }
                                }
                                
                                var vPullSupplierAccountNumber = "";
                                if ($tr.find('#hdnSupplierAccountNumber') != null) {
                                    vPullSupplierAccountNumber = $tr.find('#hdnSupplierAccountNumber').val();
                                }
                                
                                creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });
                              
                                                            
                            }); // loop
                            var ItemTracking = "";
                            if (strpullobj.SerialNumberTracking) {
                                ItemTracking = "SERIALTRACK";
                            }
                            else if (strpullobj.LotNumberTracking) {
                                ItemTracking = "LOTTRACK";
                            }
                            else if (strpullobj.DateCodeTracking) {
                                ItemTracking = "DATECODETRACK";
                            }   
                            var Creditobj = {};
                            Creditobj = {
                                'ItemGuid': strpullobj.ItemGUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                                'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                                'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                                'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                                'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                                'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                                'QLGuid': MaterialStagingGuid,
                                'SupplierAccountGuid': strpullobj.SupplierAccountGuid,
                                'PrevPullsToMSCredit': creditarrObj
                            };

                            if (gblMSPreCreditObjToSaveForCredit == undefined || gblMSPreCreditObjToSaveForCredit == null) {
                                gblMSPreCreditObjToSaveForCredit = new Array();
                            }

                            gblMSPreCreditObjToSaveForCredit.push(Creditobj);
                            if (gblMSPreCreditObjToSaveForCredit != null && gblMSPreCreditObjToSaveForCredit.length > 0) {
                                ShowHideMSCreditProgressBar(true);
                                setTimeout(function () {
                                    SaveMSCreditData(gblMSPreCreditObjToSaveForCredit)
                                }, 100);
                            }
                        }
                    },
                    error: function (err) {
                        //alert(err);
                        console.log(err);
                    }
                });
            }
        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(ErrorMessage);
        }
    });

    $("#divPreMSCreditInforSerialLot").off("click", "input[type='button'][name='btnPullAllPopUp']");
    $("#divPreMSCreditInforSerialLot").on("click", "input[type='button'][name='btnPullAllPopUp']", function () {
        var objCurtxt = $("#divPreMSCreditInforSerialLot").find("#txtStagingHeader");
        var CurrentValue = $(objCurtxt).val();
        var BinID = $("#divPreMSCreditInforSerialLot").find("#hdnBinIDValue").val();
        if ($.trim(CurrentValue) != null && $.trim(CurrentValue) != "") {
            $.ajax({
                type: "POST",
                url: SaveMaterialStagingUrl,
                contentType: 'application/json',
                dataType: 'json',
                data: "{ BinID: '" + BinID + "' , StagingName: '" + $.trim(CurrentValue) + "'}",
                success: function (result) {
                    var MaterialStagingGuid = "";
                    if (result.Status == "Success") {

                        gblMSPreCreditObjToSaveForCredit = new Array();

                        var ArrItem = new Array();
                        var arrItemDetails;
                        var ErrorMessage = ValidateAllPullForMSCredit();
                        if (ErrorMessage == "") {
                            $("#divPreMSCreditInforSerialLot").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
                                var $tblHeader = $(tblHeader);
                                //var strpullobj = JSON.parse($tblHeader.find("input[name='hdnPullMasterDTO']").val());
                                var vItemGUID = $tblHeader.prop("id").split('_')[1];
                                var strpullobj = JSON.parse($tblHeader.find("#hdnPullMasterDTO_" + vItemGUID).val());

                                var ArrItem = new Array();
                                var arrItemDetails;

                                var itemGuid = vItemGUID;
                                var SpanQty = $tblHeader.find("#txtPoolQuantity_" + vItemGUID);

                                var creditarrObj = new Array();
                                var ItemTracking = "";

                                var BinID = $tblHeader.find("#hdnBinID_" + vItemGUID).val();
                                var MainBinNumber = $tblHeader.find("#hdnMainBinNumber_" + vItemGUID).val();


                                if (result.MaterialStagingGuid != "") {
                                    $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val(result.MaterialStagingGuid);
                                }
                                if (result.MaterialStagingID != "") {
                                    $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingID").val(result.MaterialStagingID);
                                }

                                MaterialStagingGuid = $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val();
                                var MaterialStagingID = $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingID").val();

                                $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {
                                    var $tr = $(tr);
                                    var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();

                                    var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                                    var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                                    var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();

                                    var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                                    var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();

                                    if (txtPullQty != "") {

                                        var txtLotOrSerailNumber = "";
                                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                                            txtLotOrSerailNumber = $tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                                        }

                                        var vSerialNumber = "",
                                            vLotNumber = "",
                                            vExpiration = "";

                                        if (hdnSerialNumberTracking == "true") {
                                            vSerialNumber = txtLotOrSerailNumber;
                                        }
                                        if (hdnLotNumberTracking == "true") {
                                            vLotNumber = txtLotOrSerailNumber;
                                        }
                                        if (hdnDateCodeTracking == "true") {
                                            vExpiration = hdnExpiration;
                                        }
                                    }


                                    creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });

                                    if (strpullobj.SerialNumberTracking) {
                                        ItemTracking = "SERIALTRACK";
                                    }
                                    else if (strpullobj.LotNumberTracking) {
                                        ItemTracking = "LOTTRACK";
                                    }
                                    else if (strpullobj.DateCodeTracking) {
                                        ItemTracking = "DATECODETRACK";
                                    }
                                });// loop

                                var Creditobj = {};
                                Creditobj = {
                                    'ItemGuid': strpullobj.ItemGUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                                    'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                                    'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                                    'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                                    'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                                    'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                                    'QLGuid': MaterialStagingGuid,
                                    'SupplierAccountGuid': strpullobj.SupplierAccountGuid,
                                    'PrevPullsToMSCredit': creditarrObj
                                };

                                if (gblMSPreCreditObjToSaveForCredit == undefined || gblMSPreCreditObjToSaveForCredit == null)
                                    gblMSPreCreditObjToSaveForCredit = new Array();

                                gblMSPreCreditObjToSaveForCredit.push(Creditobj);
                            });

                            if (gblMSPreCreditObjToSaveForCredit != null && gblMSPreCreditObjToSaveForCredit.length > 0) {
                                ShowHideMSCreditProgressBar(true);
                                setTimeout(function () {
                                    SaveMSCreditData(gblMSPreCreditObjToSaveForCredit)
                                }, 100);
                            }
                        }
                        else {
                            showNotificationDialog();
                            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                            $("#spanGlobalMessage").html(ErrorMessage);
                        }
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });
        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(MsgSelectStagingHeader);
            return false;
        }
    });

    $("#divPreMSCreditInforSerialLot").off("click", "input[type='button'][name='btnPullAllPopUpNew']");
    $("#divPreMSCreditInforSerialLot").on("click", "input[type='button'][name='btnPullAllPopUpNew']", function () {
        var objCurtxt = $("#divPreMSCreditInforSerialLot").find("#txtStagingHeader");
        var CurrentValue = $(objCurtxt).val();
        var BinID = $("#divPreMSCreditInforSerialLot").find("#hdnBinIDValue").val();
        if ($.trim(CurrentValue) != null && $.trim(CurrentValue) != "") {
            $.ajax({
                type: "POST",
                url: SaveMaterialStagingUrl,
                contentType: 'application/json',
                dataType: 'json',
                data: "{ BinID: '" + BinID + "' , StagingName: '" + $.trim(CurrentValue) + "'}",
                success: function (result) {
                    var MaterialStagingGuid = "";
                    if (result.Status == "Success") {
                        gblMSPreCreditObjToSaveForCredit = new Array();
                        var ArrItem = new Array();
                        var arrItemDetails;
                        var ErrorMessage = ValidateAllPullForMSCredit();
                        if (ErrorMessage == "") {
                            $("#divPreMSCreditInforSerialLot").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
                                var $tblHeader = $(tblHeader);
                                var strpullobj = JSON.parse($tblHeader.find("input[name='hdnPullMasterDTO']").val());
                                var vItemGUID = $tblHeader.prop("id").split('_')[1];

                                var ArrItem = new Array();
                                var arrItemDetails;

                                var itemGuid = vItemGUID;
                                var SpanQty = $tblHeader.find("#txtPoolQuantity_" + vItemGUID);

                                var creditarrObj = new Array();
                                var ItemTracking = "";

                                var BinID = $tblHeader.find("#hdnBinID_" + vItemGUID).val();
                                var MainBinNumber = $tblHeader.find("#hdnMainBinNumber_" + vItemGUID).val();


                                if (result.MaterialStagingGuid != "") {
                                    $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val(result.MaterialStagingGuid);
                                }
                                if (result.MaterialStagingID != "") {
                                    $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingID").val(result.MaterialStagingID);
                                }

                                MaterialStagingGuid = $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val();
                                var MaterialStagingID = $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingID").val();

                                $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {
                                    var $tr = $(tr);
                                    var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();

                                    var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                                    var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                                    var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();

                                    var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                                    var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();

                                    if (txtPullQty != "") {

                                        var txtLotOrSerailNumber = "";
                                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                                            txtLotOrSerailNumber = $tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                                        }

                                        var vSerialNumber = "";
                                        var vLotNumber = "";
                                        var vExpiration = "";

                                        if (hdnSerialNumberTracking == "true") {
                                            vSerialNumber = txtLotOrSerailNumber;
                                        }
                                        if (hdnLotNumberTracking == "true") {
                                            vLotNumber = txtLotOrSerailNumber;
                                        }
                                        if (hdnDateCodeTracking == "true") {
                                            vExpiration = hdnExpiration;
                                        }
                                    }


                                    creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });

                                    if (strpullobj.SerialNumberTracking) {
                                        ItemTracking = "SERIALTRACK";
                                    }
                                    else if (strpullobj.LotNumberTracking) {
                                        ItemTracking = "LOTTRACK";
                                    }
                                    else if (strpullobj.DateCodeTracking) {
                                        ItemTracking = "DATECODETRACK";
                                    }
                                }); // loop

                                var Creditobj = {};
                                Creditobj = {
                                    'ItemGuid': strpullobj.ItemGUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                                    'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                                    'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                                    'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                                    'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                                    'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                                    'SupplierAccountGuid': strpullobj.SupplierAccountGuid,
                                    'QLGuid': MaterialStagingGuid, 'PrevPullsToMSCredit': creditarrObj
                                };

                                if (gblMSPreCreditObjToSaveForCredit == undefined || gblMSPreCreditObjToSaveForCredit == null) {
                                    gblMSPreCreditObjToSaveForCredit = new Array();
                                }

                                gblMSPreCreditObjToSaveForCredit.push(Creditobj);
                            });

                            if (gblMSPreCreditObjToSaveForCredit != null && gblMSPreCreditObjToSaveForCredit.length > 0) {
                                ShowHideMSCreditProgressBar(true);
                                setTimeout(function () {
                                    SaveMSCreditData(gblMSPreCreditObjToSaveForCredit)
                                }, 100);
                            }
                        }
                        else {
                            showNotificationDialog();
                            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                            $("#spanGlobalMessage").html(ErrorMessage);
                        }
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });
        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(MsgSelectStagingHeader);
            return false;
        }
    });

    $("#divPreMSCreditInforSerialLot").off("click", "input[type='button'][name='btnCancelPullPopup']");
    $("#divPreMSCreditInforSerialLot").on("click", "input[type='button'][name='btnCancelPullPopup']", function () {
        DeletedMSRowObject = "";
        $("#divPreMSCreditInforSerialLot").empty();
        $('#divPreMSCreditInforSerialLot').dialog('close');
    });

    $("#divPreMSCreditInforSerialLot").off("change", "input[type='text'][name='txtPullQty']");
    $("#divPreMSCreditInforSerialLot").on("change", "input[type='text'][name='txtPullQty']", function () {
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
        $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos).PullQuantity = $(this).val();
    });

    $("#divPreMSCreditInforSerialLot").off("tap click", ".tbl-item-pull tbody tr");
    $("#divPreMSCreditInforSerialLot").on("tap click", ".tbl-item-pull tbody tr", function (e) {
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

    $("#divPreMSCreditInforSerialLot").off('click', "input[type='button'][name='btnDeleteLots']");
    $("#divPreMSCreditInforSerialLot").on('click', "input[type='button'][name='btnDeleteLots']", function (e) {
        
        var vItemGUID = $(this).prop("id").split('_')[1];

        var dtID = "#tblItemPull_" + vItemGUID;

        var TotalRows = $(dtID + ' tbody tr').length;
        var SelectedRows = $(dtID + ' tbody tr.row_selected').length;
        var RemainingRows = TotalRows - SelectedRows;

        if (SelectedRows <= 0) {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(MsgSelectRowToDelete);
        }
        else {
            if (RemainingRows >= 1) {

                $(dtID).find("tbody").find("tr.row_selected").each(function (index, tr) {
                    var $tr = $(tr);
                    var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();
                    var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();

                    if (txtPullQty == "") {
                        txtPullQty = "0";
                    }

                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedMSRowObject, txtLotOrSerailNumber)) {
                            DeletedMSRowObject = DeletedMSRowObject + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                        }
                    }
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedMSRowObject, hdnExpiration)) {
                            DeletedMSRowObject = DeletedMSRowObject + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                        if (hdnExpiration != undefined) {
                            DeletedMSRowObject = DeletedMSRowObject + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else {
                        var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedMSRowObject, hdnBinNumber)) {
                            DeletedMSRowObject = DeletedMSRowObject + hdnBinNumber + "_" + txtPullQty + ",";
                        }
                    }
                    $tr.remove();
                });//loop

                var strIds = "";
                $(dtID).find("tbody").find("tr").each(function (index, tr) {
                    var $tr = $(tr);
                    var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();
                    var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();

                    if (txtPullQty == "") {
                        txtPullQty = "0";
                    }

                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, txtLotOrSerailNumber)) {
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                        }
                    }
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnExpiration)) {
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $tr.find("input[type='text'][name='txtExpirationDate']").val();
                        if (hdnExpiration != undefined) {
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else {
                        var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnBinNumber)) {
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                        }
                    }

                });//loop

                $("#hdnCurrentDeletedLoadedIds_" + vItemGUID).val(DeletedMSRowObject);
                $("#hdnCurrentLoadedIds_" + vItemGUID).val(strIds);
                isDeleteSrLotRow = true;
                var dtThisItem = $(dtID).dataTable();
                dtThisItem.fnStandingRedraw();
            }
            else {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").html(MsgRowShouldExists);
            }
        }
    });
});

function OpenPreMSCreditInfoDialog() {   
    if (SelectedMSPreCreditObj != null && SelectedMSPreCreditObj.length > 0) {
        setTimeout(function () { $('#divPreMSCreditInfo').data({ 'MakeMSPreCredit': SelectedMSPreCreditObj[0] }).dialog('open') }, 50);
    }
    else if (gblMSPreCreditObjToSave != null && gblMSPreCreditObjToSave.length > 0 && SelectedMSPreCreditObj.length <= 0) {
        setTimeout(function () {           
            SaveMSCreditData(gblMSPreCreditObjToSave)
        }, 100);
    }
    else {
        gblMSPreCreditObjToSave = null;
        SelectedMSPreCreditObj = null;
        ShowHideMSCreditProgressBar(false);
    }
}

function OpenPreMSCreditInfoDialogForSerailLot(PullInfo) {
    
    var popupheaderTitle = "MS Credit";
    if (typeof MSCreditTitle != 'undefined') {
        popupheaderTitle = MSCreditTitle;
    }
    $("#divPreMSCreditInforSerialLot").dialog({
        autoOpen: true,
        modal: true,
        draggable: true,
        resizable: true,
        width: '70%',
        height: 500,
        title: popupheaderTitle,
        open: function () {

            setTimeout(function () {
                
                $.ajax({
                    type: "POST",
                    url: "/Pull/PullLotSrSelectionForMSCredit",
                    contentType: 'application/json',
                    dataType: 'html',
                    data: JSON.stringify(PullInfo),
                    success: function (RetData) {
                        
                        //$("#divPreMSCreditInforSerialLot").html("");
                        $("#divPreMSCreditInforSerialLot").html(RetData);
                        //$("#divPreMSCreditInforSerialLot").dialog('open');

                        $('.ShowAllOptions').click(function () {
                            $(this).siblings('.AutoStagingHeader').trigger("focus");
                            $(this).siblings(".AutoStagingHeader").autocomplete("search", " ");
                        });

                        $('#DivLoading').hide();
                    },
                    error: function (response) {
                        $(tr).css('background-color', 'Red');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Red'>" + PullInfo[0].ItemNumber + " :  " + MsgErrorInProcess+"</b><br>"
                    }
                });
            }, 100);
        },
        close: function () {
            DeletedMSRowObject = "";
            $('#divPreMSCreditInforSerialLot').empty();
            ShowHideMSCreditProgressBar(false);
        }
    });
}

function SaveMSCreditData(arr) {
    $.ajax({
        "url": "/Pull/SavePullMSCredit",
        "data": JSON.stringify(arr),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "json",
        "contentType": "application/json",
        success: function (result) {
            if (result.Status != undefined && (result.Status == true || result.Status == 'true')) {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                $("#spanGlobalMessage").html(result.Message);
            } else {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").html(result.Message);
            }
            
            SaveMSCreditEditableUDF(arr);
            ShowHideMSCreditProgressBar(false);
            if (result.Status == true) {
                gblMSPreCreditObjToSave = null;
                gblMSPreCreditObjToSaveForCredit = null;                
                RefreshPullNarrowSearch();//DoNarrowSearchIM(); // Maintain narrow search value - WI-329
                NSForItemModel_ExecuteOnDocReady();
                $.each(result.RetData, function (indx, RetDataItem) {
                    var $divItemRet2 = $('#divItem_' + RetDataItem);
                    $divItemRet2.attr('style', '');
                    $divItemRet2.remove();
                });
                if ($('div[id^="divItem_"]').length <= 0) {
                    $('#divPreMSCreditInforSerialLot').dialog('close');
                    $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                }
            }
        },
        error: function (xhr) {
            ShowHideMSCreditProgressBar(false)
            alert(MsgSaveCreditAjaxError);

        }

    });
}
 
function SaveMSCreditEditableUDF(arr) {
    $.ajax({
        "url": "/Pull/SaveMSCreditEditableUDF",
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

function OpenPreMSCreditInfoPage(obj) {
    
    $.ajax({
        "url": "/Pull/FillPreMSCreditInfo",
        "data": JSON.stringify(obj),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "text",
        "contentType": "application/json",
        success: function (result) {
            $('#divPreMSCreditInfo').html(result);
        },
        error: function (xhr) {
            alert(MsgPreCreditInfoAjaxError);
        }

    });
}

function MsCreditItems(obj) {
   
    
    DeletedMSRowObject = "";
    ShowHideMSCreditProgressBar(true);
    if ($('#NewPullAction').val() == "Pull") {
        ShowHideMSCreditProgressBar(false);
        return false;
    }

    SelectedMSPreCreditObj = new Array();
    gblMSPreCreditObjToSave = new Array();
    gblMSPreCreditObjToSaveForCredit = new Array();
    SelectedMSPreQLCreditObj = new Array();
    var tr = $(obj).parent().parent();
    var $tr = $(tr);
    var itemType;
    var IsIgnoreCreditRule = false;
    
    
    if (!_msCreditPull.isNewConsumePullObj()) {
        itemType = $tr.find('#spnOrderItemType').text();
        if ($tr.find('#spnIsIgnoreCreditRule').length > 0) {
            IsIgnoreCreditRule = $tr.find('#spnIsIgnoreCreditRule').text();
        }
    }
    else {
        var itemData = _msCreditPull.getItemDataFromRow($tr);
        itemType = itemData.spnOrderItemType;
        IsIgnoreCreditRule = itemData.spnIsIgnoreCreditRule;
    }

    var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(tr)[0]);
    var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);

    var creditQty = $tr.find('#txtQty').val();

    if (isNaN(parseFloat(creditQty)) || parseFloat(creditQty) <= 0) {
        showNotificationDialog();
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgQuantityToCreditValid.replace("{0}", aData.ItemNumber));
        ShowHideMSCreditProgressBar(false);
        return false;
    }

    
    
    var itemGuid = null;
    var qlGuid = null;
    var bin = '';
    var itemTrack = '';
    var project = '';
    var itemNumber = aData.ItemNumber;
    var pullOrderNumber = "";
    var MaterialStagingGuid = "";
    var MaterialStagingHeaderName = "";
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
    if ($("#chkUsePullCommonUDF").is(":checked")) {
        if ($('#MSPullStagingHeaderValue').val() != '') {
            MaterialStagingGuid = $('#MSPullStagingHeaderValue').val();           
        }
        if ($('#txtMSPullStagingHeader').val() != '') {
            MaterialStagingHeaderName = $('#txtMSPullStagingHeader').val();
        }
    }
    else if ($tr.find('#hdnIsLoadMoreStaging') != null) {
        if ($tr.find('#hdnIsLoadMoreStaging').attr("class") == 'selectBox') {
            MaterialStagingGuid = $tr.find('#hdnIsLoadMoreStaging option:selected').text();
        }
        else {       
            MaterialStagingGuid = _msCreditPull.getMaterialStagingGuid($tr);

            //MaterialStagingGuid = $tr.find('#hdnIsLoadMoreStaging').val();
            //MaterialStagingGuid = _NewConsumePull.getDataFromRow($tr, 'hdnisloadmorestaging');
        }
    }
    
    if (!$("#chkUsePullCommonUDF").is(":checked")) {
        if ($tr.find('#txtMSStagingHeader') != null) {
            MaterialStagingHeaderName = $tr.find('#txtMSStagingHeader').val();
        }
    }

    if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
        if (aData.SerialNumberTracking != "Yes"
            &&
            aData.LotNumberTracking != "Yes"
            &&
            aData.DateCodeTracking != "Yes"
            ) {
            if (MaterialStagingGuid == "") {
                ShowHideMSCreditProgressBar(false);
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").html(MsgSelectStagingHeader);
                return false;
            }
        }
    }

    if ($tr.find('#txtBinNumber').length > 0) {
        bin = $tr.find('#txtBinNumber').val();
    }
    

    if (!_msCreditPull.isNewConsumePullObj()) {
        if ($tr.find('#spnItemGUID').length > 0) {
            itemGuid = $tr.find('#spnItemGUID').text();
        }
        if ($tr.find('#spnQuickListGUID').length > 0) {
            qlGuid = $tr.find('#spnQuickListGUID').text();
        }
    }
    else {
        var itemData = _msCreditPull.getItemDataFromRow($tr);
        itemGuid = itemData.spnItemGUID;
        qlGuid = itemData.spnQuickListGUID;
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
    else if (parseInt(itemType) == 2) {
        itemTrack = "QUICKLIST"
    }
    
    var vPullSupplierAccountNumber = "";
    if ($tr.find('#hdnSupplierAccountNumber') != null) {
        vPullSupplierAccountNumber = $tr.find('#hdnSupplierAccountNumber').val();
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
    //alert(project);
    var obj = {
        'RowID': 1, 'ItemGuid': itemGuid, 'QLGuid': qlGuid, 'ItemType': itemType,
        'Bin': bin, 'ProjectName': project, 'Quantity': creditQty, 'ItemTracking': itemTrack,
        'UDF1': udf1, 'UDF2': udf2, 'UDF3': udf3, 'UDF4': udf4, 'UDF5': udf5, 'ItemNumber': itemNumber,
        'WOGuid': vWorkOrderDetailGUID, 'PullOrderNumber': pullOrderNumber,
        'SupplierAccountGuid' : vPullSupplierAccountNumber
    };

    if (parseInt(itemType) == 2) {
        SelectedMSPreQLCreditObj.push(obj);
    }
    else {
        SelectedMSPreCreditObj.push(obj);
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
            'QLGuid': MaterialStagingGuid,
            'SupplierAccountGuid': vPullSupplierAccountNumber,
            'MaterialStagingGuid': MaterialStagingGuid,
            'PrevPullsToMSCredit': creditarrObj
        };
        gblMSPreCreditObjToSave.push(Creditobj);
    }

    if (SelectedMSPreQLCreditObj != null && SelectedMSPreQLCreditObj.length > 0) {
        AddQuickListToMSCredit(SelectedMSPreQLCreditObj, IsIgnoreCreditRule, MaterialStagingGuid);
    }
    else if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
        OpenPreMSCreditInfoDialog();
    }
    else if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
        if (itemTrack == "SERIALTRACK"
            || itemTrack == "LOTTRACK"
            || itemTrack == "DATECODETRACK") {
            
            var ItemID = 0;
            //if ($(tr).find('#spnItemID').length > 0) {
            //    ItemID = $(tr).find('#spnItemID').text();
            //}
            ItemID = _msCreditPull.getspnItemID($tr);

            var DefaultPullQuantity = 0;
            //if ($(tr).find('#hdnDPQ').length > 0) {
            //    DefaultPullQuantity = $(tr).find('#hdnDPQ').val() == "" ? 0 : $(tr).find('#hdnDPQ').val();
            //}
            DefaultPullQuantity = _msCreditPull.gethdnDPQ($tr);

            var BinID = null;
            //if ($(tr).find('#BinID').length > 0) {
            //    BinID = $(tr).find('#BinID').val() == "" ? 0 : $(tr).find('#BinID').val();
            //}
            BinID = _msCreditPull.getBinId($tr);

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#ProjectIDCommon') != undefined)
                    vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                else
                    vProjectID = "";
            }
            else {
                //if ($(tr).find('#ProjectID').length > 0) {
                //    vProjectID = $(tr).find('#ProjectID').val() == "" ? "" : $(tr).find('#ProjectID').val();
                //}
                //else {
                //    vProjectID = "";
                //}

                vProjectID = _msCreditPull.GetProjectId($tr);

            }

            
            if (bin == "") {
                $.ajax({
                    url: '/Pull/GetOrInsertBinIDByName',
                    type: 'POST',
                    data: "{ 'ItemGuid': '" + itemGuid + "', 'Name': '[|EmptyStagingBin|]', 'IsStagingLoc': '" + true + "'}",
                    contentType: 'application/json',
                    dataType: 'json',
                    success: function (data) {                        
                        ShowHideMSCreditProgressBar(false);
                        if (data > 0) {
                            BinID = data;
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
                                SupplierAccountGuid: vPullSupplierAccountNumber,
                                MaterialStagingGuid: MaterialStagingGuid,
                                MaterialStagingHeaderName: MaterialStagingHeaderName
                            });

                            OpenPreMSCreditInfoDialogForSerailLot(PullInfo);
                        }
                        ShowHideMSCreditProgressBar(false);
                    },
                    error: function (err) {
                        ShowHideMSCreditProgressBar(false);
                        return data;
                    }
                });               
            }
            else
            {
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
                    SupplierAccountGuid: vPullSupplierAccountNumber,
                    MaterialStagingGuid: MaterialStagingGuid,
                    MaterialStagingHeaderName: MaterialStagingHeaderName
                });

                OpenPreMSCreditInfoDialogForSerailLot(PullInfo);
                ShowHideMSCreditProgressBar(false);
            }
           
        }
        else {
            if (gblMSPreCreditObjToSave != null && gblMSPreCreditObjToSave.length > 0) {
                setTimeout(function () {
                    SaveMSCreditData(gblMSPreCreditObjToSave)
                }, 100);
            }
        }        
    }
}


$('#btnMSCreditAll').on('click', function (e) {
    
    DeletedMSRowObject = "";
    ShowHideMSCreditProgressBar(true);
    if ($('#NewPullAction').val() == "Pull") {
        ShowHideMSCreditProgressBar(false);
        return false;
    }

    SelectedMSPreCreditObj = new Array();
    gblMSPreCreditObjToSave = new Array();
    gblMSPreCreditObjToSaveForCredit = new Array();
    SelectedMSPreQLCreditObj = new Array();
    if ($('#ItemModeDataTable tbody tr.row_selected').length <= 0) {
        ShowHideMSCreditProgressBar(false);
        showNotificationDialog();
        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
        $("#spanGlobalMessage").html(MsgSelectAtleastOneRow);
        return false;
    }
    var IsIgnoreCreditRule = false,
        PullInfo = new Array();
        
    $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {        
        var $tr = $(this);
        var itemType;

        if (!_msCreditPull.isNewConsumePullObj()) {
            itemType = $tr.find('#spnOrderItemType').text();
        }
        else {
            itemType = _msCreditPull.getItemDataFromRow($tr).spnOrderItemType;
        }

        var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(this)[0]);
        var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);

        var creditQty = $tr.find('#txtQty').val();

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
        else if ($tr.find('#txtProjectSpent').length > 0) {
            project = $tr.find('#txtProjectSpent').val();
        }
        if ($tr.find('#txtBinNumber').length > 0) {
            bin = $tr.find('#txtBinNumber').val();
        }
        

        if (!_msCreditPull.isNewConsumePullObj()) {
            if ($tr.find('#spnItemGUID').length > 0) {
                itemGuid = $tr.find('#spnItemGUID').text();
            }
            if ($tr.find('#spnQuickListGUID').length > 0) {
                qlGuid = $tr.find('#spnQuickListGUID').text();
            }
        }
        else {
            var itemData = _msCreditPull.getItemDataFromRow($(this));
            itemGuid = itemData.spnItemGUID;
            qlGuid = itemData.spnQuickListGUID;
        }

        var pullOrderNumber = '';

        //if (typeof $(this).find('input#txtPullOrderNumber') != "undefined") {
        //    pullOrderNumber = $(this).find('input#txtPullOrderNumber').val();
        //}
        if ($tr.find('#txtPullOrderNumber') != null) {
            if ($tr.find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                vPullOrderNumber = $tr.find('#txtPullOrderNumber option:selected').text();
            }
            else {
                vPullOrderNumber = $tr.find('#txtPullOrderNumber').val();
            }
        }
        
        var vPullSupplierAccountNumber = "";
        if ($tr.find('#hdnSupplierAccountNumber') != null) {
            vPullSupplierAccountNumber = $tr.find('#hdnSupplierAccountNumber').val();
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

        var obj = {
            'RowID': i, 'ItemGuid': itemGuid, 'QLGuid': qlGuid, 'ItemType': itemType,
            'Bin': bin, 'ProjectName': project, 'Quantity': creditQty, 'ItemTracking': itemTrack,
            'UDF1': udf1, 'UDF2': udf2, 'UDF3': udf3, 'UDF4': udf4, 'UDF5': udf5, 'ItemNumber': itemNumber,
            'WOGuid': vWorkOrderDetailGUID, 'PullOrderNumber': pullOrderNumber,
            'SupplierAccountGuid': vPullSupplierAccountNumber
        };

        //if ($tr.find('#spnIsIgnoreCreditRule').length > 0) {
        //    IsIgnoreCreditRule = $tr.find('#spnIsIgnoreCreditRule').text();
        //}
        IsIgnoreCreditRule = _msCreditPull.getspnIsIgnoreCreditRule($tr);

        if ($("#chkUsePullCommonUDF").is(":checked")) {
            if ($('#MSPullStagingHeaderValue').val() != '') {
                MaterialStagingGuid = $('#MSPullStagingHeaderValue').val();
            }
            if ($('#txtMSPullStagingHeader').val() != '') {
                MaterialStagingHeaderName = $('#txtMSPullStagingHeader').val();
            }
        }
        else if ($tr.find('#hdnIsLoadMoreStaging') != null) {
            if ($tr.find('#hdnIsLoadMoreStaging').attr("class") == 'selectBox') {
                MaterialStagingGuid = $tr.find('#hdnIsLoadMoreStaging option:selected').text();
            }
            else {
                MaterialStagingGuid = _msCreditPull.getMaterialStagingGuid($tr);

                //MaterialStagingGuid = $tr.find('#hdnIsLoadMoreStaging').val();
                //MaterialStagingGuid = _NewConsumePull.getDataFromRow($tr, 'hdnisloadmorestaging');
            }
        }
        if (!$("#chkUsePullCommonUDF").is(":checked")) {
            if ($tr.find('#txtMSStagingHeader') != null) {
                MaterialStagingHeaderName = $tr.find('#txtMSStagingHeader').val();
            }
        }
        if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
            if (aData.SerialNumberTracking != "Yes"
                    &&
                    aData.LotNumberTracking != "Yes"
                    &&
                    aData.DateCodeTracking != "Yes"
            ) {
                if (MaterialStagingGuid == "") {
                    ShowHideMSCreditProgressBar(false);
                    showNotificationDialog();
                    $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                    $("#spanGlobalMessage").html(MsgSelectStagingHeader);
                    return false;
                }
            }            
        }
      
        if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
            
            if (itemTrack == "SERIALTRACK"
              || itemTrack == "LOTTRACK"
              || itemTrack == "DATECODETRACK") {

                var ItemID = 0;
                //if ($(this).find('#spnItemID').length > 0) {
                //    ItemID = $(this).find('#spnItemID').text();
                //}
                ItemID = _msCreditPull.getspnItemID($tr);

                var DefaultPullQuantity = 0;
                //if ($(this).find('#hdnDPQ').length > 0) {
                //    DefaultPullQuantity = $(this).find('#hdnDPQ').val() == "" ? 0 : $(this).find('#hdnDPQ').val();
                //}
                DefaultPullQuantity = _msCreditPull.gethdnDPQ($tr);

                var BinID = null;
                //if ($(this).find('#BinID').length > 0) {
                //    BinID = $(this).find('#BinID').val() == "" ? 0 : $(this).find('#BinID').val();
                //}
                BinID = _msCreditPull.getBinId($tr);

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

                    vProjectID = _msCreditPull.GetProjectId($tr);

                }
                if (bin == "") {
                    $.ajax({
                        url: '/Pull/GetOrInsertBinIDByName',
                        type: 'POST',
                        data: "{ 'ItemGuid': '" + itemGuid + "', 'Name': '[|EmptyStagingBin|]', 'IsStagingLoc': '" + true + "'}",
                        contentType: 'application/json',
                        dataType: 'json',
                        success: function (data) {  
                            ShowHideMSCreditProgressBar(false);
                            if (data > 0) {
                                BinID = data;
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
                                    SupplierAccountGuid: vPullSupplierAccountNumber,
                                    MaterialStagingGuid: MaterialStagingGuid,
                                    MaterialStagingHeaderName: MaterialStagingHeaderName
                                });
                            }
                        },
                        error: function (err) {
                            ShowHideMSCreditProgressBar(false);
                            return data;
                        }
                    });
                }
                else {
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
                        SupplierAccountGuid: vPullSupplierAccountNumber,
                        MaterialStagingGuid: MaterialStagingGuid,
                        MaterialStagingHeaderName: MaterialStagingHeaderName
                    });
                }
            }
            else
            {
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
                    'QLGuid': MaterialStagingGuid,
                    'SupplierAccountGuid': vPullSupplierAccountNumber,
                    'MaterialStagingGuid': MaterialStagingGuid,
                    'PrevPullsToMSCredit': creditarrObj
                };
                gblMSPreCreditObjToSave.push(Creditobj);
            }
        }
       
        if (itemType == 2) {
            SelectedMSPreQLCreditObj.push(obj);
        }
        else {
            SelectedMSPreCreditObj.push(obj);
        }
    });

    if (SelectedMSPreQLCreditObj != null && SelectedMSPreQLCreditObj.length > 0) {
        AddQuickListToMSCredit(SelectedMSPreQLCreditObj, IsIgnoreCreditRule, MaterialStagingGuid);
    }
    else if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
        OpenPreMSCreditInfoDialog();
    }
    else if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {        
        setTimeout(function () {
            if (PullInfo != null && PullInfo.length > 0) {
                if (gblMSPreCreditObjToSave != null && gblMSPreCreditObjToSave.length > 0) {
                    setTimeout(function () {
                        $.ajax({
                            "url": "/Pull/SavePullMSCredit",
                            "data": JSON.stringify(gblMSPreCreditObjToSave),
                            "type": 'POST',
                            "async": false,
                            "cache": false,
                            "dataType": "json",
                            "contentType": "application/json",
                            success: function (result) {
                                if (result.Status == true) {
                                    SaveMSCreditEditableUDF(gblMSPreCreditObjToSave);
                                    OpenPreMSCreditInfoDialogForSerailLot(PullInfo);
                                    ShowHideMSCreditProgressBar(false);
                                }
                                else {
                                    gblMSPreCreditObjToSave = null;
                                    gblMSPreCreditObjToSaveForCredit = null;
                                    alert(result.Message);
                                    ShowHideMSCreditProgressBar(false);
                                }
                            },
                            error: function (xhr) {
                                gblMSPreCreditObjToSave = null;
                                gblMSPreCreditObjToSaveForCredit = null;
                                ShowHideMSCreditProgressBar(false);
                                alert(MsgSaveCreditAjaxError);
                            }
                        });
                    }, 300);
                }
                else {
                    OpenPreMSCreditInfoDialogForSerailLot(PullInfo);
                    ShowHideMSCreditProgressBar(false);
                }
            }
            else {
                if (gblMSPreCreditObjToSave != null && gblMSPreCreditObjToSave.length > 0) {
                    setTimeout(function () {
                        SaveMSCreditData(gblMSPreCreditObjToSave)
                    }, 300);
                }
            }
        }, 500);
    }
});

function AddQuickListToMSCredit(SelectedMSPreQLCreditObj, IsIgnoreCreditRule, MaterialStagingGuid) {    
    gblMSPreCreditObjToSave = new Array();
    $.ajax({
        "url": "/Pull/GetQLItemForMSCredit",
        "data": JSON.stringify(SelectedMSPreQLCreditObj),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "json",
        "contentType": "application/json",
        success: function (result) {
            if (result.Status && result.QLItems != null && result.QLItems.length > 0) {
                for (var i = 0; i < result.QLItems.length; i++) {
                    SelectedMSPreCreditObj.push(result.QLItems[i]);
                }
            }
            if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false)
            {
                OpenPreMSCreditInfoDialog();
            }
            else if (IsIgnoreCreditRule == "True" || IsIgnoreCreditRule == "true" || IsIgnoreCreditRule == true) {
                var PullInfo = new Array();
                for (var i = 0; i < SelectedMSPreCreditObj.length; i++) {
                    if (SelectedMSPreCreditObj[i].ItemTracking == "SERIALTRACK"
                       || SelectedMSPreCreditObj[i].ItemTracking == "LOTTRACK"
                       || SelectedMSPreCreditObj[i].ItemTracking == "DATECODETRACK") {
                        PullInfo.push({
                            ID: 0,
                            ItemID: 0,
                            ItemNumber: SelectedMSPreCreditObj[i].ItemNumber,
                            ItemType: SelectedMSPreCreditObj[i].ItemType,
                            PoolQuantity: SelectedMSPreCreditObj[i].Quantity,
                            ItemGUID: SelectedMSPreCreditObj[i].ItemGuid,
                            BinID: 0,
                            BinNumber: SelectedMSPreCreditObj[i].Bin,
                            DefaultPullQuantity: SelectedMSPreCreditObj[i].ItemQtyInQL,
                            ProjectSpendGUID: 0,
                            ProjectSpendName: SelectedMSPreCreditObj[i].ProjectName,
                            UDF1: SelectedMSPreCreditObj[i].UDF1,
                            UDF2: SelectedMSPreCreditObj[i].UDF2,
                            UDF3: SelectedMSPreCreditObj[i].UDF3,
                            UDF4: SelectedMSPreCreditObj[i].UDF4,
                            UDF5: SelectedMSPreCreditObj[i].UDF5,
                            WorkOrderDetailGUID: SelectedMSPreCreditObj[i].WOGuid,
                            PullOrderNumber: SelectedMSPreCreditObj[i].PullOrderNumber,
                            SupplierAccountGuid: SelectedMSPreCreditObj[i].SupplierAccountGuid
                        });
                    }
                    else {
                        var creditarrObj = new Array();
                        creditarrObj.push({ 'Serial': null, 'Lot': null, 'ExpireDate': null, 'Qty': SelectedMSPreCreditObj[i].Quantity });
                        var Creditobj = {};
                        Creditobj = {
                            'ItemGuid': SelectedMSPreCreditObj[i].ItemGuid, 'WOGuid': SelectedMSPreCreditObj[i].WOGuid,
                            'ItemType': SelectedMSPreCreditObj[i].ItemType, 'Bin': SelectedMSPreCreditObj[i].Bin,
                            'ProjectName': SelectedMSPreCreditObj[i].ProjectName, 'Quantity': SelectedMSPreCreditObj[i].Quantity,
                            'UDF1': SelectedMSPreCreditObj[i].UDF1, 'UDF2': SelectedMSPreCreditObj[i].UDF2, 'UDF3': SelectedMSPreCreditObj[i].UDF3,
                            'UDF4': SelectedMSPreCreditObj[i].UDF4, 'UDF5': SelectedMSPreCreditObj[i].UDF5,
                            'ItemTracking': SelectedMSPreCreditObj[i].ItemTracking,
                            'PrevPullQty': 0,
                            'ItemNumber': SelectedMSPreCreditObj[i].ItemNumber,
                            'PullOrderNumber': SelectedMSPreCreditObj[i].PullOrderNumber,
                            'QLGuid': MaterialStagingGuid,
                            'SupplierAccountGuid': SelectedMSPreCreditObj[i].SupplierAccountGuid,
                            'PrevPullsToMSCredit': creditarrObj
                        };
                        gblMSPreCreditObjToSave.push(Creditobj);
                    }
                }
                if (PullInfo != null && PullInfo.length > 0) {
                    
                    if (gblMSPreCreditObjToSave != null && gblMSPreCreditObjToSave.length > 0) {
                        $.ajax({
                            "url": "/Pull/SavePullMSCredit",
                            "data": JSON.stringify(gblMSPreCreditObjToSave),
                            "type": 'POST',
                            "async": false,
                            "cache": false,
                            "dataType": "json",
                            "contentType": "application/json",
                            success: function (result) {
                                if (result.Status == true) {
                                    SaveMSCreditEditableUDF(gblMSPreCreditObjToSave);
                                    OpenPreMSCreditInfoDialogForSerailLot(PullInfo);
                                    ShowHideMSCreditProgressBar(false);
                                }
                                else {
                                    gblMSPreCreditObjToSave = null;
                                    gblMSPreCreditObjToSaveForCredit = null;
                                    alert(result.Message);
                                    ShowHideMSCreditProgressBar(false);
                                }
                            },
                            error: function (xhr) {
                                gblMSPreCreditObjToSave = null;
                                gblMSPreCreditObjToSaveForCredit = null;
                                ShowHideMSCreditProgressBar(false);
                                alert(MsgSaveCreditAjaxError);
                            }
                        });
                    }
                    else {
                        OpenPreMSCreditInfoDialogForSerailLot(PullInfo);
                        ShowHideMSCreditProgressBar(false);
                    }
                }
                else {
                    if (gblMSPreCreditObjToSave != null && gblMSPreCreditObjToSave.length > 0) {
                        setTimeout(function () {
                            SaveMSCreditData(gblMSPreCreditObjToSave)
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

function ShowHideMSCreditProgressBar(isShow) {
    if (isShow) {
        $('#DivLoading').show();
        $('#divMSCreditProcessing').modal();
        $('#divMSCreditProcessing').parent().parent().find(".modalCloseImg").css('display', 'none');
    }
    else {
        $('#DivLoading').hide();
        $.modal.impl.close();
    }
}



/////////////// New added for allow More Credit for Serial,lot and date code ////////////////

function ValidateSinglePullForMSCredit(vItemGUID) {
    var IsIgnoreCreditRule = false;
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    var dtID = "#tblItemPull_" + vItemGUID;

    var SpanQty = $("#divPreMSCreditInforSerialLot").find("#txtPoolQuantity_" + vItemGUID);
    var StagingHeader = $("#divPreMSCreditInforSerialLot").find("#txtStagingHeader").val();
    if (StagingHeader == "")
    {
        errormsg = MsgSelectStagingHeader;
        return errormsg;
    }

    var TotalEntered = 0;
    $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {
        var $tr = $(tr);
        var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
        var spnLotSerialQuantity = $tr.find("span[name='spnLotSerialQuantity']").text();
        
        IsIgnoreCreditRule = $tr.find("#hdnIsIgnoreCreditRule").val();

        var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
        var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
        var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();

        if (hdnLotNumberTracking == "true") {
            var txtLotNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
            if (txtLotNumber == "" || txtLotNumber == null) {
                errormsg = "\n" + EnterLotNumber;
                return errormsg;
            }
        }

        if (hdnSerialNumberTracking == "true") {
            var txtSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
            if (txtSerailNumber == "" || txtSerailNumber == null) {
                errormsg = "\n" + MsgSerialNumberValidation;
                return errormsg;
            }
        }

        if (hdnDateCodeTracking == "true") {
            var txtExpiration = $.trim($tr.find("input[type='text'][name^='txtExpirationDate']").val());
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
    });//loop

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

function ValidateAllPullForMSCredit() {
    //
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;

    var IsIgnoreCreditRule = false;    

    $("#divPreMSCreditInforSerialLot").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
        var ID = $(tblHeader).prop("id").split('_')[1];
        var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + ID);

        var StagingHeader = $("#divPreMSCreditInforSerialLot").find("#txtStagingHeader").val();
        if (StagingHeader == "") {
            errormsg = MsgSelectStagingHeader;
            return errormsg;
        }

        var TotalEntered = 0;
        if ($("#tblItemPull_" + ID).length > 0) {
            $("#tblItemPull_" + ID).find("tbody").find("tr").each(function (index, tr) {

                var $tr = $(tr);

                if ($tr.find("input[type='text'][name='txtPullQty']").length > 0) {
                    
                    IsIgnoreCreditRule = $tr.find("#hdnIsIgnoreCreditRule").val();

                    var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
                    var spnLotSerialQuantity = $tr.find("span[name='spnLotSerialQuantity']").text();

                    var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();

                    if (hdnLotNumberTracking == "true") {
                        var txtLotNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotNumber == "" || txtLotNumber == null) {
                            errormsg = "\n" + EnterLotNumber;
                            return errormsg;
                        }
                    }

                    if (hdnSerialNumberTracking == "true") {
                        var txtSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtSerailNumber == "" || txtSerailNumber == null) {
                            errormsg = "\n" + MsgSerialNumberValidation;
                            return errormsg;
                        }
                    }

                    if (hdnDateCodeTracking == "true") {
                        var txtExpiration = $.trim($tr.find("input[type='text'][name^='txtExpirationDate']").val());
                        if (txtExpiration == "" || txtExpiration == null) {
                            errormsg = "\n" + MsgSerialNumberValidation;
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
            }); // loop

            //
            if (isMoreQty == false) {
                var pullQty = parseFloat($(SpanQty).val().toString());
                if (TotalEntered != pullQty) {
                    ////errormsg = errormsg + "\nentered :" + TotalEntered + "\tPull Qty :" + pullQty;
                    errormsg = errormsg + "\n" + MsgEnteredPullQuantityValidation.replace("{0}", TotalEntered).replace("{1}", pullQty);
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

function PreparePullDataTableForMSSerialLot(objPullItemDTO) {
    
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
            var strReturn = "<input type='text' value='" + obj.aData.Expiration + "' id='txtExpirationDate_" + obj.aData.KitDetailGUID + "' name='txtExpirationDate' class='text-boxinner dateTextbox txtExpiration' readonly='readonly'/>";
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

                        $("#divPreMSCreditInforSerialLot input[type='text'][name='txtPullQty']").keypress(function () {
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
    if (objPullItemDTO.MaterialStagingGUID != undefined
        && objPullItemDTO.MaterialStagingGUID != null
        && objPullItemDTO.MaterialStagingGUID != ""
        && objPullItemDTO.MaterialStagingGUID != '00000000-0000-0000-0000-000000000000') {
        if ($("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid") != undefined
            && $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").length > 0) {
            $("#divPreMSCreditInforSerialLot").find("#hdnMaterialStagingGuid").val(objPullItemDTO.MaterialStagingGUID);
        }       
    }
    if ($("#divPreMSCreditInforSerialLot").find("#txtStagingHeader") != undefined
        && $("#divPreMSCreditInforSerialLot").find("#txtStagingHeader").length > 0) {
        $("#divPreMSCreditInforSerialLot").find("#txtStagingHeader").val(objPullItemDTO.MaterialStagingHeaderName);
    }
    
}

/////////////// New added for allow More Credit for Serial,lot and date code ////////////////