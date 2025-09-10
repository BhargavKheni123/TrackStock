var vCheckOutStatus = "";
var vCheckedOutQTY = 0;
var vCheckedOutMQT = 0;
var vCheckInCheckOutID = 0;
var IsCheckoutM = false;
var vQuantity = 0;
var vAvalQuantity = 0;
var vToolID = "";
var vUDF1 = '';
var vUDF2 = '';
var vUDF3 = '';
var vUDF4 = '';
var vUDF5 = '';
var TechnicianGuid = '';
var TechnicianName = '';
var vUDF1ToolCommon = ''; var vUDF2ToolCommon = ''; var vUDF3ToolCommon = ''; var vUDF4ToolCommon = ''; var vUDF5ToolCommon = '';
var vTechnician = '';
var vTechnicianName = '';
var ErrorMessage = '';
var SuccessMessage = '';
var ToolName = '';
var arrItems = new Array();
var selectedToolForWrittenOff = new Array();
var serialNumberTrackingForWrittenOff = false;
var vBinID = 0;
var isDeleteSrLotRow = false;
var LenAfterRebind = 0;
var IsLoadMoreLotsClicked = false;
var ToolSerialForWrittenOff = '';
var IsClearGlobalFilter;
//var IsGroupOfItems;

function PerformTCICO(obj, ActionType) {
    $(obj).attr('disabled', "disabled");
    //  $('#DivLoading').show();
     

    var currentButton = $(obj);
    //$(obj).prop('disabled', true);

    vCheckOutStatus = $(obj).parent().find('#spnCheckOutStatus').text() == "" ? "" : $(obj).parent().find('#spnCheckOutStatus').text();
    vCheckedOutQTY = $(obj).parent().find('#spnCheckedOutQTY').text() == "" ? 0 : $(obj).parent().find('#spnCheckedOutQTY').text();
    vCheckedOutMQT = $(obj).parent().find('#spnCheckedOutMQTY').text() == "" ? 0 : $(obj).parent().find('#spnCheckedOutMQTY').text();
    vCheckInCheckOutID = $(obj).parent().find('#spnCheckInCheckOutID').text() == "" ? 0 : $(obj).parent().find('#spnCheckInCheckOutID').text();
    vToolID = $(obj).parent().find('#spnToolID').text() == "" ? 0 : $(obj).parent().find('#spnToolID').text();
    //TechnicianGuid = $(obj).parents("tr").find('input#TechnicianGUID').val() == "" ? '00000000-0000-0000-0000-000000000000' : $(obj).parents("tr").find('input#TechnicianGUID').val();
    TechnicianName = $(obj).parents("tr").find('input#txtTechnician').val();
    IsCheckoutM = $(obj).parents("tr").find('#chkMaintance').attr('checked') ? true : false;

    vQuantity = $(obj).parents("tr").find('#txtQty').val() == "" ? 0 : $(obj).parent().parent().find('#txtQty').val();
    vAvalQuantity = $(obj).parents("tr").find('#spnQuantity').text() == "" ? 0 : $(obj).parent().find('#spnQuantity').text();

    vCheckedOutQTY = parseInt(vCheckedOutQTY, 10);
    vCheckedOutMQT = parseInt(vCheckedOutMQT, 10);
    if ($(obj).parents("tr").find('#BinID').length > 0)
    vBinID = $(obj).parents("tr").find('#BinID')[0].value == '' ? 0 : $(obj).parents("tr").find('#BinID')[0].value;

    if (isNaN(vCheckedOutMQT)) {
        vCheckedOutMQT = 0;
    }
    if (isNaN(vCheckedOutQTY)) {
        vCheckedOutQTY = 0;
    }
    vCheckInCheckOutID = parseInt(vCheckInCheckOutID, 10);
    //vToolID = parseInt(vToolID, 10);
    vQuantity = parseInt(vQuantity, 10);


    if (vQuantity == 0) {
        //alert('Kindly insert proper quantity value to perform the operation.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgInsertProperQuantityValue);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        $(obj).removeAttr('disabled');
        return false;
    }

    if ($("#txtUseThisTechnician") != null) {
        //vTechnician = $("body").find("input#UseThisTechnicianGUID").val();
        vTechnicianName = $("body").find("input#txtUseThisTechnician").val();
    }
    if ($("#chkUseToolCommonUDF").is(":checked")) {
        //TechnicianGuid = vTechnician;
        TechnicianName = vTechnicianName;
    }

    //if (TechnicianGuid == '00000000-0000-0000-0000-000000000000' || TechnicianGuid == '') {
    //    //alert('Kindly insert proper quantity value to perform the operation.');
    //    //$('div#target').fadeToggle();
    //    //$("div#target").delay(2000).fadeOut(200);
    //    showNotificationDialog();
    //    $("#spanGlobalMessage").text('Kindly select Technician from TechnicianList.');
    //    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    //    $(obj).removeAttr('disabled');
    //    return false;
    //}
    if (TechnicianName == null || TechnicianName == undefined || TechnicianName.trim() == '' || TechnicianName.trim() == TechnicianText) {
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgKindlyFillTechnician);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        $(obj).removeAttr('disabled');
        return false;
    }
    else if (TechnicianName.indexOf('-') >= 0) {
        if (TechnicianName.split('-')[0].trim() == '') {
            showNotificationDialog();
            $("#spanGlobalMessage").html(MsgInvalidTechnician);
            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
            $(obj).removeAttr('disabled');
            ErrorMessage = ErrorMessage + (ErrorMessage != '' ? ' ' : '') + MsgInvalidTechnician;
            return false;
        }
        if (TechnicianName.indexOf(" --- ") >= 0) {
            var technicianNameArray = TechnicianName.split(" --- ");
            var technicianName = technicianNameArray[0];
            var technicianCode = technicianNameArray[1];

            if ((technicianName !== undefined && technicianName != null && technicianName != '' && technicianName.indexOf("---") >= 0)
                || (technicianCode !== undefined && technicianCode != null && technicianCode != '' && technicianCode.indexOf("---") >= 0)) {

                showNotificationDialog();
                $("#spanGlobalMessage").html(msgInvalidTechnician);
                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                $(obj).removeAttr('disabled');
                ErrorMessage = ErrorMessage + (ErrorMessage != '' ? ' ' : '') + msgInvalidTechnician;
                return false;
            }
        }
        else {
            if ((TechnicianName !== undefined && TechnicianName != null && TechnicianName != '' && TechnicianName.indexOf("---") >= 0)) {

                showNotificationDialog();
                $("#spanGlobalMessage").html(msgInvalidTechnician);
                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                $(obj).removeAttr('disabled');
                ErrorMessage = ErrorMessage + (ErrorMessage != '' ? ' ' : '') + msgInvalidTechnician;
                return false;
            }
        }
    }
    
    if ($(obj).parent().parent().find('#UDF1') != null) {
        if ($(obj).parent().parent().find('#UDF1').attr("class") == 'selectBox') {
            vUDF1 = $(obj).parent().parent().find('#UDF1 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF1 = $(obj).parent().parent().find('#UDF1').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF2') != null) {
        if ($(obj).parent().parent().find('#UDF2').attr("class") == 'selectBox') {
            vUDF2 = $(obj).parent().parent().find('#UDF2 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF2 = $(obj).parent().parent().find('#UDF2').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF3') != null) {
        if ($(obj).parent().parent().find('#UDF3').attr("class") == 'selectBox') {
            vUDF3 = $(obj).parent().parent().find('#UDF3 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF3 = $(obj).parent().parent().find('#UDF3').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF4') != null) {
        if ($(obj).parent().parent().find('#UDF4').attr("class") == 'selectBox') {
            vUDF4 = $(obj).parent().parent().find('#UDF4 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF4 = $(obj).parent().parent().find('#UDF4').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF5') != null) {
        if ($(obj).parent().parent().find('#UDF5').attr("class") == 'selectBox') {
            vUDF5 = $(obj).parent().parent().find('#UDF5 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF5 = $(obj).parent().parent().find('#UDF5').val();
        }
    }
    ;
    if ($("#UDF1ToolCommon") != null) {
        if ($("#UDF1ToolCommon").attr("class") == 'selectBox') {
            vUDF1ToolCommon = $("#UDF1ToolCommon option:selected").text();
        }
        else {
            vUDF1ToolCommon = $("#UDF1ToolCommon").val();
        }
    }

    if ($("#UDF2ToolCommon") != null) {
        if ($("#UDF2ToolCommon").attr("class") == 'selectBox') {
            vUDF2ToolCommon = $("#UDF2ToolCommon option:selected").text();
        }
        else {
            vUDF2ToolCommon = $("#UDF2ToolCommon").val();
        }
    }

    if ($("#UDF3ToolCommon") != null) {
        if ($("#UDF3ToolCommon").attr("class") == 'selectBox') {
            vUDF3ToolCommon = $("#UDF3ToolCommon option:selected").text();
        }
        else {
            vUDF3ToolCommon = $("#UDF3ToolCommon").val();
        }
    }

    if ($("#UDF4ToolCommon") != null) {
        if ($("#UDF4ToolCommon").attr("class") == 'selectBox') {
            vUDF4ToolCommon = $("#UDF4ToolCommon option:selected").text();
        }
        else {
            vUDF4ToolCommon = $("#UDF4ToolCommon").val();
        }
    }

    if ($("#UDF5ToolCommon") != null) {
        if ($("#UDF5ToolCommon").attr("class") == 'selectBox') {
            vUDF5ToolCommon = $("#UDF5ToolCommon option:selected").text();
        }
        else {
            vUDF5ToolCommon = $("#UDF5ToolCommon").val();
        }
    }
    
    if ($("#chkUseToolCommonUDF").is(":checked")) {
        vUDF1 = vUDF1ToolCommon;
        vUDF2 = vUDF2ToolCommon;
        vUDF3 = vUDF3ToolCommon;
        vUDF4 = vUDF4ToolCommon;
        vUDF5 = vUDF5ToolCommon;

    }

    if (ActionType == 1) // 1 = check in , 2 = check out (Action Type)
    {
        return CheckInOperation(obj);
    }
    else {
        return CheckOutOperation(obj);
    }
}
function PerformTCICONew(obj, ActionType) {
    var currentRecordMessge = '';
 
        vCheckOutStatus = $(obj).find('#spnCheckOutStatus').text() == "" ? "" : $(obj).find('#spnCheckOutStatus').text();
        vCheckedOutQTY = $(obj).find('#spnCheckedOutQTY').text() == "" ? 0 : $(obj).find('#spnCheckedOutQTY').text();
        vCheckedOutMQT = $(obj).find('#spnCheckedOutMQTY').text() == "" ? 0 : $(obj).find('#spnCheckedOutMQTY').text();
        vCheckInCheckOutID = $(obj).find('#spnCheckInCheckOutID').text() == "" ? 0 : $(obj).find('#spnCheckInCheckOutID').text();
        vToolID = $(obj).find('#spnToolID').text() == "" ? 0 : $(obj).find('#spnToolID').text();
        TechnicianName = $(obj).find('input#txtTechnician').val();
        IsCheckoutM = $(obj).find('#chkMaintance').attr('checked') ? true : false;
        ToolName = $(obj).find("a#ToolName").text();
        vQuantity = ($(obj).find('#txtQty').val() == "" || $(obj).find('#txtQty').val() == undefined) ? 0 : $(obj).find('#txtQty').val();
        vAvalQuantity = $(obj).find('#spnQuantity').text() == "" ? 0 : $(obj).find('#spnQuantity').text();
    

    vCheckedOutQTY = parseInt(vCheckedOutQTY, 10);
    vCheckedOutMQT = parseInt(vCheckedOutMQT, 10);
    vAvalQuantity = parseInt(vAvalQuantity, 10);
    if (isNaN(vCheckedOutMQT)) {
        vCheckedOutMQT = 0;
    }
    if (isNaN(vAvalQuantity)) {
        vAvalQuantity = 0;
    }
    if (isNaN(vCheckedOutQTY)) {
        vCheckedOutQTY = 0;
    }
    vCheckInCheckOutID = parseInt(vCheckInCheckOutID, 10);
    //vToolID = parseInt(vToolID, 10);
    vQuantity = parseInt(vQuantity, 10);
    if (vCheckedOutQTY == vAvalQuantity) {
        return false;
    }

    if (vQuantity == 0) {
        //alert('Kindly insert proper quantity value to perform the operation.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        currentRecordMessge += MsgInsertProperQuantityValue;
        ErrorMessage += MsgInsertProperQuantityValue;
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    if ($("#txtUseThisTechnician") != null) {
        //vTechnician = $("body").find("input#UseThisTechnicianGUID").val();
        vTechnicianName = $("body").find("input#txtUseThisTechnician").val();
    }
    if ($("#chkUseToolCommonUDF").is(":checked")) {
        //TechnicianGuid = vTechnician;
        TechnicianName = vTechnicianName;
    }
    //if (TechnicianGuid == '00000000-0000-0000-0000-000000000000' || TechnicianGuid == '') {
    //    //alert('Kindly insert proper quantity value to perform the operation.');
    //    //$('div#target').fadeToggle();
    //    //$("div#target").delay(2000).fadeOut(200);
    //    //showNotificationDialog();
    //    ErrorMessage += 'Kindly select Technician from TechnicianList.';
    //    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    //    return false;
    //}
    if (TechnicianName == null || TechnicianName == undefined || TechnicianName.trim() == '' || TechnicianName.trim() == TechnicianText) {
        // showNotificationDialog();
        $("#spanGlobalMessage").html(TechnicianRequired + ToolName + '.');
        ErrorMessage += TechnicianRequired + ToolName + '.';
        currentRecordMessge += TechnicianRequired + ToolName + '.';
        $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
        $(obj).removeAttr('disabled');
        return false;
    }
    else if (TechnicianName.indexOf('-') >= 0) {
        if (TechnicianName.split('-')[0].trim() == '') {
            //  showNotificationDialog();
            $("#spanGlobalMessage").html(MsgInvalidTechnician);
            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
            $(obj).removeAttr('disabled');
            ErrorMessage = ErrorMessage + (ErrorMessage != '' ? ' ' : '') + 'Invalid techincian.';
            currentRecordMessge = currentRecordMessge + (currentRecordMessge != '' ? ' ' : '') + 'Invalid techincian.';
            return false;
        }

        if (TechnicianName.indexOf(" --- ") >= 0) {
            var technicianNameArray = TechnicianName.split(" --- ");
            var technicianName = technicianNameArray[0];
            var technicianCode = technicianNameArray[1];

            if ((technicianName !== undefined && technicianName != null && technicianName != '' && technicianName.indexOf("---") >= 0)
                || (technicianCode !== undefined && technicianCode != null && technicianCode != '' && technicianCode.indexOf("---") >= 0))
            {
                $("#spanGlobalMessage").html(msgInvalidTechnician + ToolName + '.');
                ErrorMessage += msgInvalidTechnician + ToolName + '.';
                currentRecordMessge += msgInvalidTechnician + ToolName + '.';
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                $(obj).removeAttr('disabled');
                return false;
            }
        }
        else
        {
            if ((TechnicianName !== undefined && TechnicianName != null && TechnicianName != '' && TechnicianName.indexOf("---") >= 0))
            {
                $("#spanGlobalMessage").html(msgInvalidTechnician + ToolName + '.');
                ErrorMessage += msgInvalidTechnician + ToolName + '.';
                currentRecordMessge += msgInvalidTechnician + ToolName + '.';
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                $(obj).removeAttr('disabled');
                return false;
            }
        }
    }

    if ($(obj).parent().parent().find('#UDF1') != null) {
        if ($(obj).parent().parent().find('#UDF1').attr("class") == 'selectBox') {
            vUDF1 = $(obj).parent().parent().find('#UDF1 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF1 = $(obj).parent().parent().find('#UDF1').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF2') != null) {
        if ($(obj).parent().parent().find('#UDF2').attr("class") == 'selectBox') {
            vUDF2 = $(obj).parent().parent().find('#UDF2 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF2 = $(obj).parent().parent().find('#UDF2').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF3') != null) {
        if ($(obj).parent().parent().find('#UDF3').attr("class") == 'selectBox') {
            vUDF3 = $(obj).parent().parent().find('#UDF3 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF3 = $(obj).parent().parent().find('#UDF3').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF4') != null) {
        if ($(obj).parent().parent().find('#UDF4').attr("class") == 'selectBox') {
            vUDF4 = $(obj).parent().parent().find('#UDF4 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF4 = $(obj).parent().parent().find('#UDF4').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF5') != null) {
        if ($(obj).parent().parent().find('#UDF5').attr("class") == 'selectBox') {
            vUDF4 = $(obj).parent().parent().find('#UDF5 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF5 = $(obj).parent().parent().find('#UDF5').val();
        }
    }
    if ($("#UDF1ToolCommon") != null) {
        if ($("#UDF1ToolCommon").attr("class") == 'selectBox') {
            vUDF1ToolCommon = $("#UDF1ToolCommon option:selected").text();
        }
        else {
            vUDF1ToolCommon = $("#UDF1ToolCommon").val();
        }
    }

    if ($("#UDF2ToolCommon") != null) {
        if ($("#UDF2ToolCommon").attr("class") == 'selectBox') {
            vUDF2ToolCommon = $("#UDF2ToolCommon option:selected").text();
        }
        else {
            vUDF2ToolCommon = $("#UDF2ToolCommon").val();
        }
    }

    if ($("#UDF3ToolCommon") != null) {
        if ($("#UDF3ToolCommon").attr("class") == 'selectBox') {
            vUDF3ToolCommon = $("#UDF3ToolCommon option:selected").text();
        }
        else {
            vUDF3ToolCommon = $("#UDF3ToolCommon").val();
        }
    }

    if ($("#UDF4ToolCommon") != null) {
        if ($("#UDF4ToolCommon").attr("class") == 'selectBox') {
            vUDF4ToolCommon = $("#UDF4ToolCommon option:selected").text();
        }
        else {
            vUDF4ToolCommon = $("#UDF4ToolCommon").val();
        }
    }

    if ($("#UDF5ToolCommon") != null) {
        if ($("#UDF5ToolCommon").attr("class") == 'selectBox') {
            vUDF5ToolCommon = $("#UDF5ToolCommon option:selected").text();
        }
        else {
            vUDF5ToolCommon = $("#UDF5ToolCommon").val();
        }
    }
    if ($("#chkUseToolCommonUDF").is(":checked")) {
        vUDF1 = vUDF1ToolCommon;
        vUDF2 = vUDF2ToolCommon;
        vUDF3 = vUDF3ToolCommon;
        vUDF4 = vUDF4ToolCommon;
        vUDF5 = vUDF5ToolCommon;
    }

    //if (ActionType == 1) // 1 = check in , 2 = check out (Action Type)
    //{
    return CheckOutOperationNew(currentRecordMessge);
    //}
    //else {
    //    return CheckOutOperationNew();
    //}
}
function CheckInOperation(obj) {
    if (IsCheckoutM && vQuantity > vCheckedOutMQT) {
        //alert('Check in quantity must be less then Check out quantity. i.e. ' + vCheckedOutMQT);
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgCheckinCheckoutValidation.replace("{0}",vCheckedOutMQT));
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    if (!IsCheckoutM && vQuantity > vCheckedOutQTY) {
        //alert('Check in quantity must be less then Check out quantity. i.e. ' + vCheckedOutQTY);
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgCheckinCheckoutValidation.replace("{0}", vCheckedOutMQT));
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    if (IsCheckoutM && vCheckedOutMQT == 0) {
        //alert('Invalid Operation!!! No check out performed.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgInvalidOperationNoCheckout);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    if (!IsCheckoutM && vCheckedOutQTY == 0) {
        //alert('Invalid Operation!!! No check out performed.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgInvalidOperationNoCheckout);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    setTimeout(function () { $(obj).removeAttr('disabled'); }, 10000);
    // $(obj).removeAttr('disabled');
    return true;
}
function CheckOutOperation(obj) {

    if (vAvalQuantity == 0) {
        //alert('There is no Quantity available for Check Out');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgNoQuantityForCheckout);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        setTimeout(function () { $(obj).removeAttr('disabled'); }, 10000);
        return false;
    }
    if (vQuantity > (vAvalQuantity - vCheckedOutQTY - vCheckedOutMQT)) {
        //alert('You can check out only Available Quantity i.e. ' + (vAvalQuantity - vCheckedOutQTY - vCheckedOutMQT));
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgCheckoutOnlyAvailableQuantity.replace("{0}", (vAvalQuantity - vCheckedOutQTY - vCheckedOutMQT)));
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        setTimeout(function () { $(obj).removeAttr('disabled'); }, 10000);
        return false;
    }

    $.ajax({
        "url": ChecInOutURL,
        data: { ActionType: 'co', Quantity: vQuantity, IsForMaintance: IsCheckoutM, ToolGUID: vToolID, AQty: vAvalQuantity, CQty: vCheckedOutQTY, CMQty: vCheckedOutMQT, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, CheckInCheckOutGUID: "", IsOnlyFromUI: true, TechnicianName: TechnicianName ,"SerialNumber":null,"ToolBinID" : vBinID},
        //UDF data was not save properly wi-973
        //            data: { ActionType: 'co', Quantity: vQuantity, IsForMaintance: IsCheckoutM, ToolGUID: vToolID, AQty: vAvalQuantity, CQty: vCheckedOutQTY, CMQty: vCheckedOutMQT, UDF1: escape(vUDF1), UDF2: escape(vUDF2), UDF3: escape(vUDF3), UDF4: escape(vUDF4), UDF5: escape(vUDF5), CheckInCheckOutGUID: "" },
        "async": false,
        "cache": false,
        "dataType": "text",
        "success": function (json) {
            if (json == "ok") {
                //$('div#target').fadeToggle();
                //$("div#target").delay(2000).fadeOut(200);
                showNotificationDialog();
                $("#spanGlobalMessage").html(MsgRecordSucessfullyUpdated);
                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                IsClearGlobalFilter = false;
                ResetNarrowSearchTechnicianList();
                //GetToolsNarrowSearchData(false, false);
                //CallNarrowfunctions();
            }
            else {
                showNotificationDialog();
                $("#spanGlobalMessage").html(json);
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                return false;
            }

            //$(obj).prop('disabled', false);

            $('#DivLoading').hide();
        },
        error: function (response) {
        },
        complete: function () {

            setTimeout(function () { $(obj).removeAttr('disabled'); }, 10000);
        }
    });

    return true;
}
function CheckOutOperationNew(CurrentRecord) {

    if (vAvalQuantity == 0) {
        //alert('There is no Quantity available for Check Out');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        //showNotificationDialog();
        ErrorMessage += MsgNoQuantityForCheckout;
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        //  return false;
    }
    if (vQuantity > (vAvalQuantity - vCheckedOutQTY - vCheckedOutMQT)) {
        //alert('You can check out only Available Quantity i.e. ' + (vAvalQuantity - vCheckedOutQTY - vCheckedOutMQT));
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        //showNotificationDialog();
        ErrorMessage += MsgCheckoutOnlyAvailableQuantity.replace("{0}", (vAvalQuantity - vCheckedOutQTY - vCheckedOutMQT));
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        //return false;
    }

    if (ErrorMessage == '' || CurrentRecord == '') {
        var data = {
            ActionType: 'co', Quantity: vQuantity,
            IsForMaintance: IsCheckoutM, ToolGUID: vToolID, AQty: vAvalQuantity,
            CQty: vCheckedOutQTY, CMQty: vCheckedOutMQT, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4,
            UDF5: vUDF5, CheckInCheckOutGUID: "", IsOnlyFromUI: true, TechnicianName: TechnicianName, "ToolName": ToolName
        };
        //alert(JSON.stringify(data));
        arrItems.push(data);
         
        return true;
    }

}

function GetSelectedToolDataForWrittenOff(obj, rowData) {
    var currentRecordMessge = '';
    vCheckedOutQTY = $(obj).parent().find('#spnCheckedOutQTY').text() == "" ? 0 : $(obj).parent().find('#spnCheckedOutQTY').text();
    vCheckedOutMQT = $(obj).parent().find('#spnCheckedOutMQTY').text() == "" ? 0 : $(obj).parent().find('#spnCheckedOutMQTY').text();
    vToolID = $(obj).parent().find('#spnToolID').text() == "" ? 0 : $(obj).parent().find('#spnToolID').text();
    ToolName = $(obj).parents("tr").find("a#ToolName").text();
    vQuantity = $(obj).parents("tr").find('#txtQty').val() == "" ? 0 : $(obj).parents("tr").find('#txtQty').val();
    vAvalQuantity = $(obj).parents("tr").find('#spnQuantity').text() == "" ? 0 : $(obj).parents("tr").find('#spnQuantity').text();
    var tmpSerialNumberTracking = $(obj).parents("tr").find("#spnToolSerialNumberTracking").text();
    serialNumberTrackingForWrittenOff = (tmpSerialNumberTracking !== undefined && tmpSerialNumberTracking != null && tmpSerialNumberTracking.toLowerCase() == "true") ? true : false;
    ToolSerialForWrittenOff = ( rowData != null && serialNumberTrackingForWrittenOff == false ) ? rowData.Serial : "";
    // IsGroupOfItems = (rowData != null && rowData.IsGroupOfItems.toLowerCase() == "yes");

    vCheckedOutQTY = parseInt(vCheckedOutQTY, 10);
    vCheckedOutMQT = parseInt(vCheckedOutMQT, 10);
    if (isNaN(vCheckedOutMQT)) {
        vCheckedOutMQT = 0;
    }
    if (isNaN(vCheckedOutQTY)) {
        vCheckedOutQTY = 0;
    }
    vQuantity = parseInt(vQuantity, 10);

    if (vQuantity == 0) {
        currentRecordMessge += MsgInsertProperQuantityValue;
        ErrorMessage += MsgInsertProperQuantityValue;
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }

    return ValidateWrittenOffAll(currentRecordMessge);
}

function ValidateWrittenOffAll(CurrentRecord) {
    if (vAvalQuantity == 0) {
        if (ErrorMessage != null && ErrorMessage.length > 0) {
            ErrorMessage += "<br>" + msgQtyNotAvailableToWrittenOff;
        }
        else {
            ErrorMessage += msgQtyNotAvailableToWrittenOff;
        }        
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    }
    if (vQuantity > (parseInt(vAvalQuantity) - (vCheckedOutQTY + vCheckedOutMQT))) {
        if (ErrorMessage != null && ErrorMessage.length > 0) {
            ErrorMessage += "<br>" + msgWrittenOffOnlyAvailableQty + (vAvalQuantity - (vCheckedOutQTY + vCheckedOutMQT));
        }
        else {
            ErrorMessage += msgWrittenOffOnlyAvailableQty + (vAvalQuantity - (vCheckedOutQTY + vCheckedOutMQT));
        }        
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    }
    //if ((serialNumberTrackingForWrittenOff || IsGroupOfItems) && (vQuantity > (parseInt(vAvalQuantity) - (vCheckedOutQTY + vCheckedOutMQT) - (1))))
    //{
    //    if (ErrorMessage != null && ErrorMessage.length > 0) {
    //        ErrorMessage += "<br>" + msgAvailableQtyCantBeLessThanOne;
    //    }
    //    else {
    //        ErrorMessage += msgAvailableQtyCantBeLessThanOne;
    //    }            
    //    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    //}

    if (ErrorMessage == '' && CurrentRecord == '') {
        var data = {
            Quantity: vQuantity,
            GUID: vToolID,
            ToolName: ToolName,
            Serial: ToolSerialForWrittenOff,
            SerialNumberTracking: serialNumberTrackingForWrittenOff
        };

        selectedToolForWrittenOff.push(data);

        return true;
    }    
    return false;
}

$(document).ready(function () {
    $('body').on('click', "input#btnToolCheckoutAllNewFlow", function (event) {
        $(this).prop('disabled', true);
        SuccessMessage = '';
        ErrorMessage = '';
        arrItems = new Array();

        if ($('#myDataTable tbody tr.row_selected').length <= 0) {
            alert(MsgSelectRecordCheckout);
            $(this).prop('disabled', false);
        }
        else {
            var isSerialLot = false;
            $('#myDataTable tbody tr.row_selected').each(function (i, tr) {
                var aPos = $('#myDataTable').dataTable().fnGetPosition($(tr)[0]);
                var aData = $('#myDataTable').dataTable().fnGetData(aPos);
                
                if ($(this).attr('class').indexOf('row_selected') != -1) {
                    var TechnicianName = $(this).find('input#txtTechnician').val();
                    var vTechnicianName = '';
                    if ($("#txtUseThisTechnician") != null) {
                        //vTechnician = $("body").find("input#UseThisTechnicianGUID").val();
                        vTechnicianName = $("body").find("input#txtUseThisTechnician").val();
                    }
                    if ($("#chkUseToolCommonUDF").is(":checked")) {
                        //TechnicianGuid = vTechnician;
                        TechnicianName = vTechnicianName;
                    }
                    if (TechnicianName == null || TechnicianName == undefined || TechnicianName.trim() == '' || TechnicianName.trim() == TechnicianText) {
                        //showNotificationDialog();
                        //$("#spanGlobalMessage").html(MsgKindlyFillTechnician);
                        ErrorMessage = MsgKindlyFillTechnician;
                        //$("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        $(this).removeAttr('disabled');
                        return false;
                    }

                    if ((aData.SerialNumberTracking == false)) {
                        $(this).find("btnCheckOut").prop('disabled', true);
                        if (PerformTCICONew($(this), 2) == false) {
                            return;
                        }
                    }
                    else {
                        
                        if (aData.SerialNumberTracking == true) {
                             
                            isSerialLot = true;
                        }
                    }
                }
            });
               
            if (ErrorMessage != '' && arrItems.length == 0 ) {
                $('#myDataTable tbody tr.row_selected').each(function (i) {
                    if ($(this).attr('class').indexOf('row_selected') != -1) {
                        $(this).prop('disabled', false);
                    }
                });
              
                $("#btnToolCheckoutAllNewFlow").prop('disabled', false);
                showNotificationDialog();
                $("#spanGlobalMessage").html(ErrorMessage);
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
            }
            else {
                if (isSerialLot == true) {
                    OpenCheckOutPopup($(this));
                }
                else {
                    if (arrItems.length > 0) {
                        $.ajax({
                            "url": CheckOutAll,
                            data: { "arrItems": JSON.stringify(arrItems) },
                            "async": false,
                            "cache": false,
                            dataType: "text",
                            "success": function (json) {
                                $("#btnToolCheckoutAllNewFlow").prop('disabled', false);
                                //if (json == "ok") {

                                showNotificationDialog();
                                //SuccessMessage=" Checkout Successfully Updated." ;
                                if (ErrorMessage == '') {
                                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                                }
                                else {
                                    $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                                }
                                $("#spanGlobalMessage").html(json + ErrorMessage);
                                IsClearGlobalFilter = false;
                                ResetNarrowSearchTechnicianList();

                                //}
                                //else {

                                //    ErrorMessage = json;
                                //    showNotificationDialog();
                                //    $("#spanGlobalMessage").html(ErrorMessage);
                                //    $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                                //    ResetNarrowSearchTechnicianList();
                                //}
                            },
                            error: function (response) {
                                $("#btnToolCheckoutAllNewFlow").prop('disabled', false);
                            }
                        });
                    } else {
                        // No records to process.
                        showNotificationDialog();
                        $("#spanGlobalMessage").html(SelectProperRecordToCheckout);
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                    }
                }
            }
            $("#btnToolCheckoutAllNewFlow").prop('disabled', false);

        }
    });
    $('body').on('click', "input#btnToolCheckInAllBottom", function (event) {
        var TechnicianGuid = ToolTechnicianValue[0];

        var selectedRecordsCount = (parseInt($('#myDataTable tbody tr.row_selected').length) || 0);
        if (selectedRecordsCount < 1) {
            alert(MsgSelectToolToCheckIn);
            return false;

        }
        arrItems = new Array();

        $('#myDataTable tbody tr.row_selected').each(function (i,tr) {
            var currentRaw = $(this);
            if (currentRaw.attr('class').indexOf('row_selected') != -1) {
                var tmpToolGuid = currentRaw.find('#hdnGUID');
                var vToolID = tmpToolGuid.val(); //currentRaw.find('#hdnGUID').val() == "" ? "" : currentRaw.find('#hdnGUID').val();
                if (typeof (tmpToolGuid) === 'undefined' || tmpToolGuid === undefined || typeof (vToolID) === 'undefined' || vToolID === undefined) {
                    alert(MsgSelectToolMainGridCheckIn);
                    return false;
                }
                var CheckInQty = currentRaw.find('#txtCheckInQty').val() == "" ? "0" : currentRaw.find('#txtCheckInQty').val();
                var ToolName = currentRaw.find("a#ToolName").text();

                
                if (typeof (CheckInQty) === 'undefined' || CheckInQty === undefined || CheckInQty === null || CheckInQty === '' || isNaN(CheckInQty) || ( parseInt(CheckInQty) || 0 ) <= 0) {
                    alert(MsgEnterQuantityToCheckIn);
                    return false;
                }
                else {
                    var data = {
                        ActionType: 'ci', Quantity: CheckInQty,CQty:0,CMQty:0,
                        ToolGUID: vToolID, IsForMaintance: false, AQty: 0
                        , UDF1: '', UDF2: '', UDF3: '', UDF4: '', UDF5:''
                        , CheckInCheckOutGUID: "62BC6D67-13FC-45EF-9888-2C48EF948F45", IsOnlyFromUI: true, TechnicianGuid: ToolTechnicianValue[0], "ToolName": ToolName
                        , TechnicianName:''
                    };
                    //alert(JSON.stringify(data));
                    arrItems.push(data);
                }
            }
        });


        if (arrItems.length > 0 && arrItems.length === selectedRecordsCount) {
            $.ajax({
                "url": CheckInAllNew,
                data: { "arrItems": JSON.stringify(arrItems) },
                "async": false,
                "cache": false,
                dataType: "text",
                "success": function (json) {

                    //if (json == "ok") {

                    showNotificationDialog();
                    //SuccessMessage=" Checkout Successfully Updated." ;
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    $("#spanGlobalMessage").html(json);
                    IsClearGlobalFilter = false;
                    ResetNarrowSearchTechnicianList();
                    $("input#btnToolCheckInAllBottom").hide();
                    ToolTechnicianValue = '';
                }, complete: function () {
                    $("#btnToolCheckInAllNewFlow").prop('disabled', false);
                }
            });
        }
    });

    $('body').on('click', "input#btnToolWrittenOffAllNewFlow", function (event) {
        $(this).prop('disabled', true);
        SuccessMessage = '';
        ErrorMessage = '';
        selectedToolForWrittenOff = new Array();
        var tmpErrorMessage = '';
        var selectedRecords = 0;

        $('#myDataTable tbody tr.row_selected').each(function (i, tr) {
            var aPos = $('#myDataTable').dataTable().fnGetPosition($(tr)[0]);
            var aData = $('#myDataTable').dataTable().fnGetData(aPos);

            //if ($(this).attr('class').indexOf('row_selected') != -1) {                    
            if (aData != null && aData.Type == 1)
                {
                    selectedRecords++;
                }
            //}
        });
       
        if (selectedRecords < 1)
        {
            alert(msgSelectToolToWrittenOff);
            $(this).prop('disabled', false);
        }
        else {
            var isSerialLot = false;
            $('#myDataTable tbody tr.row_selected').each(function (i, tr) {
                var aPos = $('#myDataTable').dataTable().fnGetPosition($(tr)[0]);
                var aData = $('#myDataTable').dataTable().fnGetData(aPos);
                if (aData != null && aData.Type == 1) {
                    if ($(this).attr('class').indexOf('row_selected') != -1) {
                        if (GetSelectedToolDataForWrittenOff($(this).find("input#btnCheckOut"), aData) == false) {
                            var ToolName = $(this).find("a#ToolName").text();
                            if (tmpErrorMessage != '' && tmpErrorMessage.length > 0) {
                                tmpErrorMessage += "<br>" + ToolName + ": " + ErrorMessage;
                            }
                            else {
                                tmpErrorMessage += ToolName + ": " + ErrorMessage;
                            }
                            //tmpErrorMessage += ErrorMessage;
                            ErrorMessage = '';
                            return;
                        }
                    }
                }                
            });
            ErrorMessage = tmpErrorMessage;
            if (ErrorMessage != '' && selectedToolForWrittenOff.length > 0)
            {
                $("#btnToolWrittenOffAllNewFlow").prop('disabled', false);
                showNotificationDialog();
                $("#spanGlobalMessage").html(ErrorMessage);
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
            }
            if (ErrorMessage != '' && selectedToolForWrittenOff.length == 0) {
                $('#myDataTable tbody tr.row_selected').each(function (i) {
                    if ($(this).attr('class').indexOf('row_selected') != -1) {
                        $(this).prop('disabled', false);
                    }
                });

                $("#btnToolWrittenOffAllNewFlow").prop('disabled', false);
                showNotificationDialog();
                $("#spanGlobalMessage").html(ErrorMessage);
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
            }
            else {
                $.ajax({
                        type: "POST",
                        url: urlWrittenOffToolQuantity,
                        contentType: 'application/json',
                        dataType: 'html',
                        data: JSON.stringify(selectedToolForWrittenOff),
                        async:false,                        
                        success: function (RetData) {
                            $("#btnToolWrittenOffAllNewFlow").prop('disabled', false);
                            $("#DivPullSelection").html("");
                            $("#DivPullSelection").html(RetData);
                            $("#DivPullSelection").dialog('open');
                            $('span#ui-dialog-title-DivPullSelection').text("Written Off Tool");
                            $('#DivLoading').hide();
                        },
                        error: function (err) {
                            console.log(err);
                        }
                    });
            }
            $("#btnToolWrittenOffAllNewFlow").prop('disabled', false);
        }
    });

    $('body').on('click', "input#btnReassignTechnician", function (event) {
        $('div#DivLoading').show();
        var selectedRecords = 0;
        var ToolCheckInCheckOutGuids = "";
        var aaritems = new Array();
        var vUDF1 = '', vUDF2 = '', vUDF3 = '', vUDF4 = '', vUDF5 = '';

        $('.toolcheckouttables tbody tr.row_selected').each(function (i, tr) {
            selectedRecords++;
            var CheckInCheckOutGuid = $(tr).find("#spnCheckInCheckOutID");

            if (CheckInCheckOutGuid !== undefined && CheckInCheckOutGuid != null && CheckInCheckOutGuid != ""
                && CheckInCheckOutGuid.text() != undefined && CheckInCheckOutGuid.text() != null && CheckInCheckOutGuid.text() != "")
            {
                ToolCheckInCheckOutGuids += CheckInCheckOutGuid.text() + ",";

                //if ($(tr).find('#UDF1') != null) {
                //    if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
                //        vUDF1 = $(tr).find('#UDF1 option:selected').text().replace(/'/g, "''");
                //    }
                //    else {
                //        vUDF1 = $(tr).find('#UDF1').val();
                //    }
                //}
                //if ($(tr).find('#UDF2') != null) {
                //    if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
                //        vUDF2 = $(tr).find('#UDF2 option:selected').text().replace(/'/g, "''");
                //    }
                //    else {
                //        vUDF2 = $(tr).find('#UDF2').val();
                //    }
                //}
                //if ($(tr).find('#UDF3') != null) {
                //    if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
                //        vUDF3 = $(tr).find('#UDF3 option:selected').text().replace(/'/g, "''");
                //    }
                //    else {
                //        vUDF3 = $(tr).find('#UDF3').val();
                //    }
                //}
                //if ($(tr).find('#UDF4') != null) {
                //    if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
                //        vUDF4 = $(tr).find('#UDF4 option:selected').text().replace(/'/g, "''");
                //    }
                //    else {
                //        vUDF4 = $(tr).find('#UDF4').val();
                //    }
                //}
                //if ($(tr).find('#UDF5') != null) {
                //    if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
                //        vUDF5 = $(tr).find('#UDF5 option:selected').text().replace(/'/g, "''");
                //    }
                //    else {
                //        vUDF5 = $(tr).find('#UDF5').val();
                //    }
                //}


                //if (vUDF1 == null || vUDF1 == undefined) {
                //    vUDF1 = "";
                //}
                //if (vUDF2 == null || vUDF2 == undefined) {
                //    vUDF2 = "";
                //}
                //if (vUDF3 == null || vUDF3 == undefined) {
                //    vUDF3 = "";
                //}
                //if (vUDF4 == null || vUDF4 == undefined) {
                //    vUDF4 = "";
                //}
                //if (vUDF5 == null || vUDF5 == undefined) {
                //    vUDF5 = "";
                //}
                  
                //aaritems.push({
                //    ToolCheckInOutGuid: CheckInCheckOutGuid.text(), UDF1: escape(vUDF1), UDF2: escape(vUDF2), UDF3: escape(vUDF3), UDF4: escape(vUDF4), UDF5: escape(vUDF5),
                //});
            }
            
        });

        if (selectedRecords < 1) {
            $("#spanGlobalMessage").html(msgSelectCheckOutTool);
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            showNotificationDialog();
            $('#DivLoading').hide();
            return false;
        }
        else {
            var TechnicianName = "";

            if ($("#chkUseToolCommonUDF").is(":checked")) {

                if ($("#txtUseThisTechnician") != null && $("#txtUseThisTechnician") !== undefined) {
                    TechnicianName = $("body").find("input#txtUseThisTechnician").val();
                }
            }

            if (TechnicianName == null || TechnicianName == undefined || TechnicianName.trim() == '' || TechnicianName.trim() == TechnicianText) {
                $("#spanGlobalMessage").html(msgCheckUseThis);
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                showNotificationDialog();
                $('#DivLoading').hide();
                return false;
            }
            else if (TechnicianName.indexOf('-') >= 0) {
                if (TechnicianName.split('-')[0].trim() == '') {
                    $("#spanGlobalMessage").html(msgInvalidTechnicianForReassign);
                    $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
                    showNotificationDialog();
                    $('#DivLoading').hide();
                    return false;
                }

                if (TechnicianName.indexOf(" --- ") >= 0) {
                    var technicianNameArray = TechnicianName.split(" --- ");
                    var technicianName = technicianNameArray[0];
                    var technicianCode = technicianNameArray[1];

                    if ((technicianName !== undefined && technicianName != null && technicianName != '' && technicianName.indexOf("---") >= 0)
                        || (technicianCode !== undefined && technicianCode != null && technicianCode != '' && technicianCode.indexOf("---") >= 0)) {
                        $("#spanGlobalMessage").html(msgInvalidTechnician);
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        showNotificationDialog();
                        $('#DivLoading').hide();
                        return false;
                    }
                }
                else {
                    if ((TechnicianName !== undefined && TechnicianName != null && TechnicianName != '' && TechnicianName.indexOf("---") >= 0)) {
                        $("#spanGlobalMessage").html(msgInvalidTechnician);
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        showNotificationDialog();
                        $('#DivLoading').hide();
                        return false;
                    }
                }
            }

            if ($("#UDF1ToolCommon") != null) {
                if ($("#UDF1ToolCommon").attr("class") == 'selectBox') {
                    vUDF1 = $("#UDF1ToolCommon option:selected").text();
                }
                else {
                    vUDF1 = $("#UDF1ToolCommon").val();
                }
            }

            if ($("#UDF2ToolCommon") != null) {
                if ($("#UDF2ToolCommon").attr("class") == 'selectBox') {
                    vUDF2 = $("#UDF2ToolCommon option:selected").text();
                }
                else {
                    vUDF2 = $("#UDF2ToolCommon").val();
                }
            }

            if ($("#UDF3ToolCommon") != null) {
                if ($("#UDF3ToolCommon").attr("class") == 'selectBox') {
                    vUDF3 = $("#UDF3ToolCommon option:selected").text();
                }
                else {
                    vUDF3 = $("#UDF3ToolCommon").val();
                }
            }

            if ($("#UDF4ToolCommon") != null) {
                if ($("#UDF4ToolCommon").attr("class") == 'selectBox') {
                    vUDF4 = $("#UDF4ToolCommon option:selected").text();
                }
                else {
                    vUDF4 = $("#UDF4ToolCommon").val();
                }
            }

            if ($("#UDF5ToolCommon") != null) {
                if ($("#UDF5ToolCommon").attr("class") == 'selectBox') {
                    vUDF5 = $("#UDF5ToolCommon option:selected").text();
                }
                else {
                    vUDF5 = $("#UDF5ToolCommon").val();
                }
            }
            //if ($("#chkUseToolCommonUDF").is(":checked")) {
            //    vUDF1 = vUDF1ToolCommon;
            //    vUDF2 = vUDF2ToolCommon;
            //    vUDF3 = vUDF3ToolCommon;
            //    vUDF4 = vUDF4ToolCommon;
            //    vUDF5 = vUDF5ToolCommon;
            //}
            //var params = { ReassignCheckOutTechnician: aaritems, TechnicianName: TechnicianName };
            var params = { ToolCheckInCheckOutGuids: ToolCheckInCheckOutGuids, TechnicianName: TechnicianName, UDF1: escape(vUDF1), UDF2: escape(vUDF2), UDF3: escape(vUDF3), UDF4: escape(vUDF4), UDF5: escape(vUDF5) };
            $('#DivLoading').show();

            $.ajax({
                type: "POST",
                url: urlReassignTechnician,
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify(params),
                success: function (response) {
                    if (response.Status) {
                        oTable.fnStandingRedraw();
                    }
                    $('#DivLoading').hide();
                    $.modal.impl.close();
                    showNotificationDialog();
                    $("#spanGlobalMessage").html(response.Message);
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon succesIcon').addClass(response.NotificationClass);
                },
                error: function (err) {
                    showNotificationDialog();
                    $("#spanGlobalMessage").html(msgFailToReassign);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $('#DivLoading').hide();
                    $.modal.impl.close();
                }
            });
        }
    });
});


function PrepareWrittenOffDataTable(objPullItemDTO) {    
    var columnarrIL = new Array();
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            
            if (objPullItemDTO.ViewRight == "ViewOverwrite") {
                var strReturn = "<span style='position:relative'>";
                strReturn = strReturn + "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumber' name='txtLotOrSerailNumber' class='text-boxinner95 AutoSerialLot' />";
                strReturn = strReturn + '<a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsSL" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
                return strReturn;
            }
            else if (objPullItemDTO.ViewRight == "NoRight") {
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
            var strReturn = "<span name='spnLotSerialQuantity' id='spnLotSerialQuantity_" + obj.aData.ID + "_" + obj.aData.LotOrSerailNumber + "'>" + obj.aData.LotSerialQuantity + "</span>";
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {

            var strReturn = "<input type='hidden' name='hdnRowUniqueId' value='" + obj.aData.ID + "_" + obj.aData.GUID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnSerialNumberTracking' value='" + obj.aData.SerialNumberTracking + "' />";
            
            strReturn = strReturn + "<input type='hidden' name='hdnToolGUID' value='" + objPullItemDTO.ToolGUID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnToolKitDetailGUID' value='" + objPullItemDTO.ToolKitDetailGUID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnToolKitGUID' value='" + objPullItemDTO.ToolKitGUID + "' />";

            if (objPullItemDTO.SerialNumberTracking == BoolTrueString) {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtPullQty_" + obj.aData.ID + "_" + obj.aData.LotOrSerailNumber + "' name='txtPullQty' class='text-boxinner pull-quantity' style='width:100px;' readonly='readonly' />";
            }
            else {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtPullQty_" + obj.aData.ID + "_" + obj.aData.LotOrSerailNumber + "' name='txtPullQty' class='text-boxinner pull-quantity numericinput' style='width:100px;' />";
            }
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strWOobj = JSON.parse($("#hdnWrittenOffCategories").val());
            var strReturn = "<select class='selectBox writtenOffInput' id='ddlCategory_" + obj.aData.ID + "_" + obj.aData.LotOrSerailNumber + "' name='ddlCategory' >";
            for (i = 0; i < strWOobj.length; i++) {

                strReturn += "<option value= " + strWOobj[i].ID + " >" + strWOobj[i].Text + " </option>";
            }
            strReturn += "</select>";
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            return "<textarea class='text-box writtenOffInput' cols='200' id='WrittenOffDescription_" + obj.aData.ID + "_" + obj.aData.LotOrSerailNumber + "' name='WrittenOffDescription' maxlength='1024' rows='2' style='background:#fff;border: 1px solid #dddddd;width:95%;border-radius: 5px;padding: 5px;'></textarea>";
        }
    });
    columnarrIL.push({ mDataProp: "Received", sClass: "read_only" });

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
        "sAjaxSource": WrittenOffSerialSelectionUrl,
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.IsConsignedLotSerial == true) {
                nRow.className = "even trconsigned";
            }
        },
        "fnInitComplete": function (oSettings) {
            var strAllSelected = "";

            $("#hdnSelectedId_" + objPullItemDTO.objPullItemDTO).val();
            //if (objPullItemDTO.LotNumberTracking != BoolTrueString && objPullItemDTO.SerialNumberTracking != BoolTrueString) {
            if (objPullItemDTO.SerialNumberTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(0, false);
            }           
        },
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            aoData.push({ "name": "ToolGUID", "value": objPullItemDTO.ToolGUID });
            //aoData.push({ "name": "BinID", "value": objPullItemDTO.BinID });
            if (objPullItemDTO.ToolGUID != '00000000-0000-0000-0000-000000000000' && objPullItemDTO.ToolGUID != '')
                aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ToolGUID).val(), 2) });

            aoData.push({ "name": "CurrentLoaded", "value": $("#hdnCurrentLoadedIds_" + objPullItemDTO.ToolGUID).val() });
            aoData.push({ "name": "ViewRight", "value": objPullItemDTO.ViewRight });
            aoData.push({ "name": "IsDeleteRowMode", "value": isDeleteSrLotRow });
            aoData.push({ "name": "SerialNumberTracking", "value": objPullItemDTO.SerialNumberTracking });
            aoData.push({ "name": "ToolKitDetailGUID", "value": objPullItemDTO.ToolKitDetailGUID });
            aoData.push({ "name": "ToolKitGUID", "value": objPullItemDTO.ToolKitGUID });

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

                    if (objPullItemDTO.ViewRight == "ViewOnly") {
                        $("input[type='text'][name='txtLotOrSerailNumberViewOnly']").keypress(function () {
                            return false;
                        });

                        $("#DivPullSelection input[type='text'][name='txtPullQty']").keypress(function () {
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

function OpenCheckOutPopup(btnObj) {
    var errorMsg = '';
    $('#DivLoading').show();
    var PullInfo = new Array();
    
    if ($(btnObj).prop("id") == "btnToolCheckoutAllNewFlow") {
        $('#myDataTable').find("tbody").find("tr.row_selected").each(function (index, tr) {
            var aPos = $('#myDataTable').dataTable().fnGetPosition($(tr)[0]);
            var aData = $('#myDataTable').dataTable().fnGetData(aPos);
            errorMsg = '';
            var txxt = $(tr).find('#txtQty');
            var vBinID;
            
            var vProjectSpendName = '';
            var toolType = $(tr).find('#spnToolType').html();
            var txtQty = txxt.val();
            
            var ToolTypeTracking = $(tr).find('#spnToolSerialNumberTracking').text();
            var vsIsForMaintenance = $(tr).find('#chkMaintance').is(":checked");
            if ($(tr).find('#BinID').length > 0)
            vBinID = $(tr).find('#BinID')[0].value == '' ? 0 : $(tr).find('#BinID')[0].value;

            if (ToolTypeTracking == BoolTrueString || ToolTypeTracking == true | ToolTypeTracking == "true") {

                if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                    errorMsg = MsgInsertQuantityCheckOut;
                    //alert(MsgInsertQuantityCheckOut);
                    
                    return;
                }

                if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                    errorMsg = MsgSelectLocationCheckout;
                    //alert(MsgSelectLocationCheckout);
                   
                    return;
                }
            }
            //else {

            //    if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
            //        $(tr).css('background-color', 'Olive');
            //        IsGlobalErrorMsgShow = true;
            //        errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": Labour Item Required Hours to Pull.</b><br/>"
            //    }
            //    vBinID = 0;
            //    vProjectID = '';
            //    vProjectSpendName = '';
            //}
            
            var TechnicianName = $(tr).find('input#txtTechnician').val();
            var vTechnician = "";

            if ($("#chkUseToolCommonUDF").is(":checked")) {
                if ($("#txtUseThisTechnician") != null) {
                    TechnicianName = $("body").find("input#txtUseThisTechnician").val();
                }
            }
            else {
                if ($(tr).find('#txtTechnician') != null) {
                    TechnicianName = $(tr).find('#txtTechnician').val();
                }
            }

            //if (TechnicianName == null || TechnicianName == undefined || TechnicianName.trim() == '' || TechnicianName.trim() == TechnicianText) {
            //    errorMsg = 'Kindly fill technician.';
            //}

            if (TechnicianName.indexOf(" --- ") >= 0) {
                var technicianNameArray = TechnicianName.split(" --- ");
                var technicianName = technicianNameArray[0];
                var technicianCode = technicianNameArray[1];

                if ((technicianName !== undefined && technicianName != null && technicianName != '' && technicianName.indexOf("---") >= 0)
                    || (technicianCode !== undefined && technicianCode != null && technicianCode != '' && technicianCode.indexOf("---") >= 0)) {

                    errorMsg = msgInvalidTechnician;
                }
            }
            else {
                if ((TechnicianName !== undefined && TechnicianName != null && TechnicianName != '' && TechnicianName.indexOf("---") >= 0)) {
                    errorMsg = msgInvalidTechnician;
                }
            }

            if (errorMsg.length <= 0) {
                var vItemID = $(tr).parent().find('#spnToolID').text() == "" ? 0 : $(tr).parent().find('#spnToolID').text();
                var vToolGUID = $(tr).find('#spnItemGUID').text();
                var vspnOn_HandQuantity = $(tr).find('#spnQuantity').text() == "" ? 0 : $(tr).find('#spnQuantity').text();
                var vPullCreditText = "Check Out"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
                var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                var vUDF1ToolCOCommon = ''; var vUDF2ToolCOCommon = ''; var vUDF3ToolCOCommon = ''; var vUDF4ToolCOCommon = ''; var vUDF5ToolCOCommon = '';
                
                if ($("#chkUseToolCommonUDF").is(":checked")) {
                    if ($("#UDF1ToolCommon") != null) {
                        if ($("#UDF1ToolCommon").attr("class") == 'selectBox') {
                            vUDF1ToolCOCommon = $("#UDF1ToolCommon option:selected").text();
                        }
                        else {
                            vUDF1ToolCOCommon = $("#UDF1ToolCommon").val();
                        }
                    }

                    if ($("#UDF2ToolCommon") != null) {
                        if ($("#UDF2ToolCommon").attr("class") == 'selectBox') {
                            vUDF2ToolCOCommon = $("#UDF2ToolCommon option:selected").text();
                        }
                        else {
                            vUDF2ToolCOCommon = $("#UDF2ToolCommon").val();
                        }
                    }

                    if ($("#UDF3ToolCommon") != null) {
                        if ($("#UDF3ToolCommon").attr("class") == 'selectBox') {
                            vUDF3ToolCOCommon = $("#UDF3ToolCommon option:selected").text();
                        }
                        else {
                            vUDF3ToolCOCommon = $("#UDF3ToolCommon").val();
                        }
                    }

                    if ($("#UDF4ToolCommon") != null) {
                        if ($("#UDF4ToolCommon").attr("class") == 'selectBox') {
                            vUDF4ToolCOCommon = $("#UDF4ToolCommon option:selected").text();
                        }
                        else {
                            vUDF4ToolCOCommon = $("#UDF4ToolCommon").val();
                        }
                    }

                    if ($("#UDF5ToolCommon") != null) {
                        if ($("#UDF5ToolCommon").attr("class") == 'selectBox') {
                            vUDF5ToolCOCommon = $("#UDF5ToolCommon option:selected").text();
                        }
                        else {
                            vUDF5ToolCOCommon = $("#UDF5ToolCommon").val();
                        }
                    }
                }

                if ($(tr).find('#UDF1') != null) {
                    if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
                        vUDF1 = $(tr).find('#UDF1 option:selected').text();
                    }
                    else {
                        vUDF1 = $(tr).find('#UDF1').val();
                    }
                }

                if ($(tr).find('#UDF2') != null) {
                    if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
                        vUDF2 = $(tr).find('#UDF2 option:selected').text();
                    }
                    else {
                        vUDF2 = $(tr).find('#UDF2').val();
                    }
                }

                if ($(tr).find('#UDF3') != null) {
                    if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
                        vUDF3 = $(tr).find('#UDF3 option:selected').text();
                    }
                    else {
                        vUDF3 = $(tr).find('#UDF3').val();
                    }
                }

                if ($(tr).find('#UDF4') != null) {
                    if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
                        vUDF4 = $(tr).find('#UDF4 option:selected').text();
                    }
                    else {
                        vUDF4 = $(tr).find('#UDF4').val();
                    }
                }

                if ($(tr).find('#UDF5') != null) {
                    if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
                        vUDF5 = $(tr).find('#UDF5 option:selected').text();
                    }
                    else {
                        vUDF5 = $(tr).find('#UDF5').val();
                    }
                }

                if ($("#chkUseToolCommonUDF").is(":checked")) {
                    vUDF1 = vUDF1ToolCOCommon;
                    vUDF2 = vUDF2ToolCOCommon;
                    vUDF3 = vUDF3ToolCOCommon;
                    vUDF4 = vUDF4ToolCOCommon;
                    vUDF5 = vUDF5ToolCOCommon;
                }
                
                //PullInfo.push({ ID: index, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vToolGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity, ProjectSpendGUID: vProjectID, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: vWorkOrderDetailGUID, PullOrderNumber: vPullOrderNumber });
                PullInfo.push({ ID: 0, ToolID: vItemID, PullQuantity: txtQty, ToolGUID: vToolGUID, BinID: vBinID, ToolCheckoutUDF1: vUDF1, ToolCheckoutUDF2: vUDF2, ToolCheckoutUDF3: vUDF3, ToolCheckoutUDF4: vUDF4, ToolCheckoutUDF5: vUDF5, Technician: TechnicianName, IsForMaintenance: vsIsForMaintenance });
            }
            else {
                
                showNotificationDialog();
                $("#spanGlobalMessage").html(errorMsg);
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
              
                return false;
            }
        });
    }
    else if ($(btnObj).prop("id") == "btnCheckOut") {

        var tr = $(btnObj).parent().parent()[0];
        var aPos = $('#myDataTable').dataTable().fnGetPosition($(tr)[0]);
        var aData = $('#myDataTable').dataTable().fnGetData(aPos);

        var txxt = $(tr).find('#txtQty');
        var vBinID;
        
        var vProjectSpendName = '';
        var toolType = $(tr).find('#spnToolType').text();
        var txtQty = txxt.val();
        var ToolTypeTracking = $(tr).find('#spnToolSerialNumberTracking').text();
        var TechnicianName = $(tr).find('input#txtTechnician').val();
        var vTechnicianName = '';
        if ($("#txtUseThisTechnician") != null) {
            //vTechnician = $("body").find("input#UseThisTechnicianGUID").val();
            vTechnicianName = $("body").find("input#txtUseThisTechnician").val();
        }
        if ($("#chkUseToolCommonUDF").is(":checked")) {
            //TechnicianGuid = vTechnician;
            TechnicianName = vTechnicianName;
        }
        if (ToolTypeTracking == BoolTrueString || ToolTypeTracking == true || ToolTypeTracking == "true") {

            vBinID = $(tr).find('#BinID')[0].value == '' ? 0 : $(tr).find('#BinID')[0].value;

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $(tr).css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg = MsgQtyCheckOutMandatory;
            }

            if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                $(tr).css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg = InventoryLocationMandatory;
            }
            if (TechnicianName == null || TechnicianName == undefined || TechnicianName.trim() == '' || TechnicianName.trim() == TechnicianText) {
                $(tr).css('background-color', 'Olive');
                errorMsg = MsgKindlyFillTechnician;
                    //alert("Please select Location to check out");
                    IsGlobalErrorMsgShow = true;
            }
            if (TechnicianName.indexOf(" --- ") >= 0) {
                var technicianNameArray = TechnicianName.split(" --- ");
                var technicianName = technicianNameArray[0];
                var technicianCode = technicianNameArray[1];

                if ((technicianName !== undefined && technicianName != null && technicianName != '' && technicianName.indexOf("---") >= 0)
                    || (technicianCode !== undefined && technicianCode != null && technicianCode != '' && technicianCode.indexOf("---") >= 0)) {

                    $(tr).css('background-color', 'Olive');
                    errorMsg = msgInvalidTechnician;
                    IsGlobalErrorMsgShow = true;
                }
            }
            else {
                if ((TechnicianName !== undefined && TechnicianName != null && TechnicianName != '' && TechnicianName.indexOf("---") >= 0)) {
                    $(tr).css('background-color', 'Olive');
                    errorMsg = msgInvalidTechnician;
                    IsGlobalErrorMsgShow = true;
                }
            }
        }

        if (errorMsg.length <= 0) {
            var vItemID = $(tr).find('#spnToolID').text() == "" ? 0 : $(tr).parent().find('#spnToolID').text();
            var vItemGUID = $(tr).find('#spnItemGUID').text();
            var vspnOn_HandQuantity = $(tr).find('#spnQuantity').text() == "" ? 0 : $(tr).find('#spnQuantity').text();
            var vPullCreditText = "Check Out"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
            var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
            var vUDF1ToolCOCommon = ''; var vUDF2ToolCOCommon = ''; var vUDF3ToolCOCommon = ''; var vUDF4ToolCOCommon = ''; var vUDF5ToolCOCommon = '';
            var vTechnician = "";
            var vsIsForMaintenance = $(tr).find('#chkMaintance').is(":checked");
            if ($("#chkUseToolCommonUDF").is(":checked")) {
                if ($('#txtUseThisTechnician') != null) {
                     
                    TechnicianName = $('#txtUseThisTechnician').val();
                    
                }
            }
            else {
                if ($(tr).find('#txtTechnician') != null) {
                    TechnicianName = $(tr).find('#txtTechnician').val();
                }
            }

            if ($("#chkUseToolCommonUDF").is(":checked")) {
                if ($("#UDF1ToolCommon") != null) {
                    if ($("#UDF1ToolCommon").attr("class") == 'selectBox') {
                        vUDF1ToolCommon = $("#UDF1ToolCommon option:selected").text();
                    }
                    else {
                        vUDF1ToolCommon = $("#UDF1ToolCommon").val();
                    }
                }

                if ($("#UDF2ToolCommon") != null) {
                    if ($("#UDF2ToolCommon").attr("class") == 'selectBox') {
                        vUDF2ToolCommon = $("#UDF2ToolCommon option:selected").text();
                    }
                    else {
                        vUDF2ToolCommon = $("#UDF2ToolCommon").val();
                    }
                }

                if ($("#UDF3ToolCommon") != null) {
                    if ($("#UDF3ToolCommon").attr("class") == 'selectBox') {
                        vUDF3ToolCommon = $("#UDF3ToolCommon option:selected").text();
                    }
                    else {
                        vUDF3ToolCommon = $("#UDF3ToolCommon").val();
                    }
                }

                if ($("#UDF4ToolCommon") != null) {
                    if ($("#UDF4ToolCommon").attr("class") == 'selectBox') {
                        vUDF4ToolCommon = $("#UDF4ToolCommon option:selected").text();
                    }
                    else {
                        vUDF4ToolCommon = $("#UDF4ToolCommon").val();
                    }
                }

                if ($("#UDF5ToolCommon") != null) {
                    if ($("#UDF5ToolCommon").attr("class") == 'selectBox') {
                        vUDF5ToolCommon = $("#UDF5ToolCommon option:selected").text();
                    }
                    else {
                        vUDF5ToolCommon = $("#UDF5ToolCommon").val();
                    }
                }
            }

            if ($(tr).find('#UDF1') != null) {
                if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
                    vUDF1 = $(tr).find('#UDF1 option:selected').text();
                }
                else {
                    vUDF1 = $(tr).find('#UDF1').val();
                }
            }

            if ($(tr).find('#UDF2') != null) {
                if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
                    vUDF2 = $(tr).find('#UDF2 option:selected').text();
                }
                else {
                    vUDF2 = $(tr).find('#UDF2').val();
                }
            }

            if ($(tr).find('#UDF3') != null) {
                if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
                    vUDF3 = $(tr).find('#UDF3 option:selected').text();
                }
                else {
                    vUDF3 = $(tr).find('#UDF3').val();
                }
            }

            if ($(tr).find('#UDF4') != null) {
                if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
                    vUDF4 = $(tr).find('#UDF4 option:selected').text();
                }
                else {
                    vUDF4 = $(tr).find('#UDF4').val();
                }
            }

            if ($(tr).find('#UDF5') != null) {
                if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
                    vUDF5 = $(tr).find('#UDF5 option:selected').text();
                }
                else {
                    vUDF5 = $(tr).find('#UDF5').val();
                }
            }

            if ($("#chkUseToolCommonUDF").is(":checked")) {
                vUDF1 = vUDF1ToolCommon;
                vUDF2 = vUDF2ToolCommon;
                vUDF3 = vUDF3ToolCommon;
                vUDF4 = vUDF4ToolCommon;
                vUDF5 = vUDF5ToolCommon;
            }

            PullInfo.push({ ID: 0, ToolID: vItemID, PullQuantity: txtQty, ToolGUID: vItemGUID, BinID: vBinID, ToolCheckoutUDF1: vUDF1, ToolCheckoutUDF2: vUDF2, ToolCheckoutUDF3: vUDF3, ToolCheckoutUDF4: vUDF4, ToolCheckoutUDF5: vUDF5, Technician: TechnicianName, IsForMaintenance: vsIsForMaintenance });
        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").html(errorMsg);
            $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
            $(btnObj).removeAttr('disabled');
            $('#DivLoading').hide();
            return false;
        }
    }

    if (errorMsg.length <= 0) {
         
        $.ajax({
            type: "POST",
            url: PullToolQuantityUrl,
            contentType: 'application/json',
            dataType: 'html',
            data: JSON.stringify(PullInfo),
            success: function (RetData) {
                $("#DivPullSelection").html("");
                $("#DivPullSelection").html(RetData);
                $("#DivPullSelection").dialog('open');
                $('#DivLoading').hide();
            },
            error: function (err) {
                //alert(err);
                console.log(err);
            }
        });
    }
    else {
        $('#DivLoading').hide();
        $('#DivLoading').hide();
        $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
        $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
        $('#dlgCommonErrorMsg').find("#pErrMessage").html(errorMsg);
        $('#dlgCommonErrorMsg').modal({
            escClose: false,
            close: false
        });
    }
    return false;
}

function closeModalPSLimit() {
    $.modal.impl.close();
}

function PrepareToolPullDataTable(objPullItemDTO) {
    
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
                //strReturn = strReturn + '<a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsSL" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
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
    // changed by hetal dave to remove [|EmptyStagingBin|]
    //columnarrIL.push({ mDataProp: "BinNumber", sClass: "read_only" });    
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strReturn = "<span name='spnBinNumber' id='spnBinNumber_" + obj.aData.ID + "'>" + (obj.aData.Location == "[|EmptyStagingBin|]" ? "" : obj.aData.Location) + "</span>";
            return strReturn;
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

            var RequisitionDetailGUID = '';
            if (obj.aData.RequisitionDetailGUID != null && obj.aData.RequisitionDetailGUID != undefined)
                RequisitionDetailGUID = obj.aData.RequisitionDetailGUID;

            var strReturn = "<input type='hidden' name='hdnRowUniqueId' value='" + obj.aData.ID + "_" + obj.aData.ToolGUID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnLotNumberTracking' value='" + obj.aData.LotNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnSerialNumberTracking' value='" + obj.aData.SerialNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnDateCodeTracking' value='" + obj.aData.DateCodeTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpiration' value='" + obj.aData.Expiration + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpirationDate' value='" + obj.aData.strExpirationDate + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnBinNumber' value='" + obj.aData.Location + "' />";

            if (objPullItemDTO.SerialNumberTracking == BoolTrueString) {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtPullQty_" + obj.aData.ID + "' name='txtPullQty' class='text-boxinner pull-quantity' readonly='readonly' />";
            }
            else {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtPullQty_" + obj.aData.ID + "' name='txtPullQty' class='text-boxinner pull-quantity' />";
            }
            return strReturn;
        }
    });
    columnarrIL.push({ mDataProp: "Received", sClass: "read_only" });
    columnarrIL.push({ mDataProp: "Expiration", sClass: "read_only" });

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
        "sAjaxSource": PullToolLotSrSelectionUrl,
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.IsConsignedLotSerial == true) {
                nRow.className = "even trconsigned";
            }
        },
        "fnInitComplete": function (oSettings) {
            var strAllSelected = "";

            $("#hdnSelectedId_" + objPullItemDTO.ToolGUID ).val();
            if (objPullItemDTO.LotNumberTracking != BoolTrueString && objPullItemDTO.SerialNumberTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(0, false);
            }
            if (objPullItemDTO.DateCodeTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(5, false);
            }
        },
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {

           
            aoData.push({ "name": "ToolGUID", "value": objPullItemDTO.ToolGUID });
            aoData.push({ "name": "BinID", "value": objPullItemDTO.BinID });
            aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPullQuantity_" + objPullItemDTO.ToolGUID+ "_" +objPullItemDTO.RequisitionDetailGUID).val(), 2)
        });
           // aoData.push({ "name": "InventoryConsuptionMethod", "value": objPullItemDTO.InventoryConsuptionMethod });
            aoData.push({ "name": "CurrentLoaded", "value": $("#hdnCurrentLoadedIds_" + objPullItemDTO.ToolGUID+ "_" +objPullItemDTO.RequisitionDetailGUID).val() });
            aoData.push({ "name": "ViewRight", "value": objPullItemDTO.ViewRight });
            aoData.push({ "name": "IsDeleteRowMode", "value":false  });//isDeleteSrLotRow
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

                    if (objPullItemDTO.ViewRight == "ViewOnly") {
                        $("input[type='text'][name='txtLotOrSerailNumberViewOnly']").keypress(function () {
                            return false;
                        });

                        $("#DivPullSelection input[type='text'][name='txtPullQty']").keypress(function () {
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
$(document).ready(function () {
$("#DivPullSelection").off("click", "input[type='button'][name='btnToolPopup']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnToolPopup']", function () {
         //RequisitionDetailsGUID
        var vToolGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
        if (vRequisitionDetailGUID == undefined || (typeof vRequisitionDetailGUID) == (typeof undefined))
        {
            vRequisitionDetailGUID='';
        }
        
        var dtID = "#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID;

        var ArrItem = new Array();
        var arrItemDetails;
        var ErrorMessage = ValidateSinglePull(vToolGUID, vRequisitionDetailGUID);

        if (ErrorMessage == "") {

            arrItemDetails = new Array();
            var ID = vToolGUID;
            var SpanQty = $("#DivPullSelection").find("#txtPullQuantity_" + vToolGUID + "_" + vRequisitionDetailGUID);

            var dt = $("#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID).dataTable();
            var currentData = dt.fnGetData();

            var strpullobj = JSON.parse($("#hdnToolAssetPullMasterDTO_" + vToolGUID + "_" + vRequisitionDetailGUID).val());

            

            $("#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                var hdnIsForMaintenance = $(tr).find("input[name='hdnIsForMaintenance']").val();
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

                    var obj = {
                        "LotOrSerailNumber": txtLotOrSerailNumber, "Location": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                                    , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                    , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": $.trim(vLotNumber)
                                    , "ToolGUID": strpullobj.ToolGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                    };

                    arrItemDetails.push(obj);
                }
            });

            var pullQty = parseFloat($(SpanQty).val().toString());
            
            var PullItem = {
                ID: 1,
                ToolGUID: strpullobj.ToolGUID,
                TechnicianGUID: strpullobj.TechnicianGUID,
                Technician: strpullobj.Technician,
                ToolID: strpullobj.ToolID,
                ToolName: strpullobj.ToolName,
                BinID: strpullobj.BinID,
                Location: strpullobj.Location,
                PullQuantity: pullQty,
                ToolCheckoutUDF1: strpullobj.ToolCheckoutUDF1,
                ToolCheckoutUDF2: strpullobj.ToolCheckoutUDF2,
                ToolCheckoutUDF3: strpullobj.ToolCheckoutUDF3,
                ToolCheckoutUDF4: strpullobj.ToolCheckoutUDF4,
                ToolCheckoutUDF5: strpullobj.ToolCheckoutUDF5,
                IsMaintenance: strpullobj.IsForMaintenance,
                lstToolPullDetails: arrItemDetails,
                PullOrderNumber: strpullobj.PullOrderNumber,
                WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID,
                RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID
            };

            ArrItem.push(PullItem);

            if (ArrItem.length > 0) {
                PullMultipleToolNew(ArrItem);
            }
        }
        else {
            alert(ErrorMessage);
        }

    });
    $("#DivPullSelection").off('focus', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#DivPullSelection").on('focus', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {

        var objCurtxt = $(this);
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        
        var aPos = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

        var dtItemPull = "#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString();
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
                else if (hdnDateCodeTracking == "true") {
                    var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                    if (hdnExpiration != undefined)
                        strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                }
                else {
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
                        url: '/Tool/GetLotOrSerailNumberList',
                        contentType: 'application/json',
                        dataType: 'json',
                        data: {
                            maxRows: 1000,
                            name_startsWith: request.term,
                            ToolGuid: ids[1].toString(),
                            BinID: aData.BinID,
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
                    //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBin']").val(ui.item.selval);
                    //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBinName']").val(ui.item.label);
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
    $("#DivPullSelection").off('change', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#DivPullSelection").on('change', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {

        var objCurtxt = $(this);
        var oldValue = $(objCurtxt).val();
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');

        var aPos = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

        var dtThisItem = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable();
        var currentTR = $(objCurtxt).parent().parent().parent()[0];
        var row_id = dtThisItem.fnGetPosition(currentTR);

        if ($.trim(oldValue) == '')
            return;

        var isDuplicateEntry = false;
        var OtherPullQuantity = 0;
        $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString() + " tbody tr").each(function (i) {
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
        });

        if (isDuplicateEntry == true) {

            if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "LotNumberTracking")
                alert(MsgDuplicateLotNumber);
            else if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "SerialNumberTracking")
                alert(MsgDuplicateSerialNumberValidation);
            else
                alert(MsgDuplicateNumber);

            $(objCurtxt).val("");
            $(objCurtxt).focus();
        }
        else {
            $.ajax({
                type: "POST",
                url: ValidateSerialLotNumberUrl,
                contentType: 'application/json',
                dataType: 'json',
                data: "{ ToolGuid: '" + ids[1].toString() + "', SerialOrLotNumber: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + aData.BinID + "' }",
                success: function (RetData) {
                    if (RetData.ID > 0) {
                        IsCheckViewRight = false;

                        var spnPoolQuantity = parseFloat($("#txtPullQuantity_" + ids[1].toString() + "_" + ids[2].toString()).val());
                        if ((spnPoolQuantity - OtherPullQuantity) > 0) {
                            if ((spnPoolQuantity - OtherPullQuantity) < RetData.PullQuantity)
                                RetData.PullQuantity = spnPoolQuantity - OtherPullQuantity;
                        }
                        else {
                            RetData.PullQuantity = 0;
                        }

                        dtThisItem.fnUpdate(RetData, row_id, undefined, false, false);
                        IsCheckViewRight = true;

                        $('.ShowAllOptionsSL').click(function () {
                            $(this).siblings('.AutoSerialLot').trigger("focus");
                            $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                        });

                        if (RetData.IsConsignedLotSerial) {
                            $(currentTR).addClass("trconsigned");
                        }
                        else {
                            $(currentTR).removeClass("trconsigned");
                        }
                    }
                    else {
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
    $("#DivPullSelection1").off('focus', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#DivPullSelection1").on('focus', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {

        var objCurtxt = $(this);
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');

        var aPos = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

        var dtItemPull = "#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString();
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
                else if (hdnDateCodeTracking == "true") {
                    var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                    if (hdnExpiration != undefined)
                        strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                }
                else {
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
                        url: '/Tool/GetLotOrSerailNumberList',
                        contentType: 'application/json',
                        dataType: 'json',
                        data: {
                            maxRows: 1000,
                            name_startsWith: request.term,
                            ToolGuid: ids[1].toString(),
                            BinID: aData.BinID,
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
                    //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBin']").val(ui.item.selval);
                    //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBinName']").val(ui.item.label);
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
    $("#DivPullSelection1").off('change', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#DivPullSelection1").on('change', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {

        var objCurtxt = $(this);
        var oldValue = $(objCurtxt).val();
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');

        var aPos = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

        var dtThisItem = $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable();
        var currentTR = $(objCurtxt).parent().parent().parent()[0];
        var row_id = dtThisItem.fnGetPosition(currentTR);

        if ($.trim(oldValue) == '')
            return;

        var isDuplicateEntry = false;
        var OtherPullQuantity = 0;
        $("#tblToolPull_" + ids[1].toString() + "_" + ids[2].toString() + " tbody tr").each(function (i) {
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
        });

        if (isDuplicateEntry == true) {

            if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "LotNumberTracking")
                alert(MsgDuplicateLotNumber);
            else if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "SerialNumberTracking")
                alert(MsgDuplicateSerialNumberValidation);
            else
                alert(MsgDuplicateNumber);

            $(objCurtxt).val("");
            $(objCurtxt).focus();
        }
        else {
            $.ajax({
                type: "POST",
                url: ValidateSerialLotNumberUrl,
                contentType: 'application/json',
                dataType: 'json',
                data: "{ ToolGuid: '" + ids[1].toString() + "', SerialOrLotNumber: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + aData.BinID + "' }",
                success: function (RetData) {
                     
                    if (RetData.ID > 0) {
                        IsCheckViewRight = false;

                        var spnPoolQuantity = parseFloat($("#txtPullQuantity_" + ids[1].toString() + "_" + ids[2].toString()).val());
                        if ((spnPoolQuantity - OtherPullQuantity) > 0) {
                            if ((spnPoolQuantity - OtherPullQuantity) < RetData.PullQuantity)
                                RetData.PullQuantity = spnPoolQuantity - OtherPullQuantity;
                        }
                        else {
                            RetData.PullQuantity = 0;
                        }

                        dtThisItem.fnUpdate(RetData, row_id, undefined, false, false);
                        IsCheckViewRight = true;

                        $('.ShowAllOptionsSL').click(function () {
                            $(this).siblings('.AutoSerialLot').trigger("focus");
                            $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                        });

                        if (RetData.IsConsignedLotSerial) {
                            $(currentTR).addClass("trconsigned");
                        }
                        else {
                            $(currentTR).removeClass("trconsigned");
                        }
                    }
                    else {
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
    $("#DivPullSelection").off("click", "input[type='button'][name='btnToolCheckOutAllPopUp']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnToolCheckOutAllPopUp']", function () {
        ToolCheckOutAllNewFlow();
    });
    $("#DivPullSelection1").off("click", "input[type='button'][name='btnToolCheckOutAllPopUp']");
    $("#DivPullSelection1").on("click", "input[type='button'][name='btnToolCheckOutAllPopUp']", function () {
        ToolCheckOutAllNewFlow1();
    });
    $("#DivPullSelection").off("click", "input[type='button'][name='btnCancelToolCheckOutPopup']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnCancelToolCheckOutPopup']", function () {
        $("#DivPullSelection").empty();
        $('#DivPullSelection').dialog('close');
        //$('#ItemModeDataTable').dataTable().fnStandingRedraw();
    });
    $("#DivPullSelection1").off("click", "input[type='button'][name='btnCancelToolCheckOutPopup']");
    $("#DivPullSelection1").on("click", "input[type='button'][name='btnCancelToolCheckOutPopup']", function () {
        $("#DivPullSelection1").empty();
        $('#DivPullSelection1').dialog('close');
        //$('#ItemModeDataTable').dataTable().fnStandingRedraw();
    });
    $("#DivPullSelection").off('click', "input[type='button'][name='btnDeleteLots']");
    $("#DivPullSelection").on('click', "input[type='button'][name='btnDeleteLots']", function (e) {

        var vToolGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
        if (vRequisitionDetailGUID == null || vRequisitionDetailGUID == "null") {
            vRequisitionDetailGUID = "";
        }
        var dtID = "#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID;

        var TotalRows = $(dtID + ' tbody tr').length;
        var SelectedRows = $(dtID + ' tbody tr.row_selected').length;
        var RemainingRows = TotalRows - SelectedRows;

        if (SelectedRows <= 0) {
            alert(MsgSelectRowToDelete);
        }
        else {
            if (RemainingRows >= 1) {
                $(dtID).find("tbody").find("tr.row_selected").each(function (index, tr) {
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
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber))
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                    }
                        //else if (hdnDateCodeTracking == "true") {
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $(tr).find("input[name='hdnExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnExpiration))
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[name='hdnExpirationDate']").val();
                        if (hdnExpiration != undefined)
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber))
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                    }

                });

                $("#hdnCurrentLoadedIds_" + vToolGUID + "_" + vRequisitionDetailGUID).val(strIds);
                isDeleteSrLotRow = true;
                var dtThisItem = $(dtID).dataTable();
                dtThisItem.fnStandingRedraw();
            }
            else {
                alert(MsgRowShouldExists);
               
            }

        }
    });
    $("#DivPullSelection").off("click", "input[type='button'][name='btnLoadMoreLots']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnLoadMoreLots']", function () {
        var vToolGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
        if (vRequisitionDetailGUID == null || vRequisitionDetailGUID == "null") {
            vRequisitionDetailGUID = "";
        }
        var dtID = "#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID;
        var strIds = "";

        var MaxQuantity = $("#txtPullQuantity_" + vToolGUID + "_" + vRequisitionDetailGUID)[0].value;
        var TotalQuantity = 0;
        $("#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID).find("[id*='txtPullQty_']").each(function () {
            TotalQuantity = TotalQuantity + parseInt($(this)[0].value);
        });
       
        if (MaxQuantity > TotalQuantity) {
            IsLoadMoreLotsClicked = true;
            $(dtID).find("tbody").find("tr").each(function (index, tr) {

                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                if (txtPullQty != undefined) {
                    if (txtPullQty == "") {
                        txtPullQty = "0";
                    }
                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber))
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                    }
                        //else if (hdnDateCodeTracking == "true") {
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $(tr).find("input[name='hdnExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnExpiration))
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[name='hdnExpirationDate']").val();
                        if (hdnExpiration != undefined)
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber))
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                    }
                }
            });

            $("#hdnCurrentLoadedIds_" + vToolGUID + "_" + vRequisitionDetailGUID).val(strIds);

            var dt = $(dtID).dataTable();
            dt.fnStandingRedraw();
        }
        else {
            alert(MsgPullCreditQuantity);
            
        }
    });
    $("#DivPullSelection1").off("click", "input[type='button'][name='btnAddToolLineItem']");
    $("#DivPullSelection1").on("click", "input[type='button'][name='btnAddToolLineItem']", function () {
        
        //RequisitionDetailsGUID
        var vToolGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
        if (vRequisitionDetailGUID == undefined || (typeof vRequisitionDetailGUID) == (typeof undefined)) {
            vRequisitionDetailGUID = '';
        }

        var dtID = "#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID;

        var ArrItem = new Array();
        var arrItemDetails;
        var ErrorMessage = ValidateSinglePull1(vToolGUID, vRequisitionDetailGUID);
      
        if (ErrorMessage == "") {

            arrItemDetails = new Array();
            var ID = vToolGUID;
            var SpanQty = $("#DivPullSelection1").find("#txtPullQuantity_" + vToolGUID + "_" + vRequisitionDetailGUID);

            var dt = $("#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID).dataTable();
            var currentData = dt.fnGetData();

            var strpullobj = JSON.parse($("#hdnToolAssetPullMasterDTO_" + vToolGUID + "_" + vRequisitionDetailGUID).val());



            $("#tblToolPull_" + vToolGUID + "_" + vRequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                var hdnIsForMaintenance = $(tr).find("input[name='hdnIsForMaintenance']").val();
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

                    var obj = {
                        "LotOrSerailNumber": txtLotOrSerailNumber, "Location": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                        , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                        , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": $.trim(vLotNumber)
                        , "ToolGUID": strpullobj.ToolGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                    };

                    arrItemDetails.push(obj);
                }
            });

            var pullQty = parseFloat($(SpanQty).val().toString());
             
            var PullItem = {
                ID: 1,
                ToolGUID: strpullobj.ToolGUID,
                TechnicianGUID: strpullobj.TechnicianGUID,
                Technician: strpullobj.Technician,
                ToolID: strpullobj.ToolID,
                PullGUID: $("div#CtabNew").find("input#GUID").val(),
                ToolName: strpullobj.ToolName,
                BinID: strpullobj.BinID,
                Location: strpullobj.Location,
                PullQuantity: pullQty,
                ToolCheckoutUDF1: strpullobj.ToolCheckoutUDF1,
                ToolCheckoutUDF2: strpullobj.ToolCheckoutUDF2,
                ToolCheckoutUDF3: strpullobj.ToolCheckoutUDF3,
                ToolCheckoutUDF4: strpullobj.ToolCheckoutUDF4,
                ToolCheckoutUDF5: strpullobj.ToolCheckoutUDF5,
                IsMaintenance: strpullobj.IsForMaintenance,
                lstToolPullDetails: arrItemDetails,
                PullOrderNumber: strpullobj.PullOrderNumber,
                WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID,
                RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID
            };

            ArrItem.push(PullItem);

            if (ArrItem.length > 0) {
                //PullMultipleToolNew(ArrItem);
                ToolKitLineItemNew(ArrItem);
            }
        }
        else {
            alert(ErrorMessage);
        }

    });
});
function ToolCheckOutAllNewFlow() {
    var ArrItem = new Array();
    var arrItemDetails;
    //alert("In new flow");
    var ErrorMessage = ValidateAllCheckOut();
     
    if (ErrorMessage == "") {
        $("#DivPullSelection").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
             
            var strpullobj = JSON.parse($(tblHeader).find("input[name='hdnToolAssetPullMasterDTO']").val());
            arrItemDetails = new Array();
            var ID = $(tblHeader).prop("id").split('_')[1];
            var vToolGUID = strpullobj.ToolGUID;
            var RequisitionDetailGUID = $(tblHeader).prop("id").split('_')[2];
            var SpanQty = 0;
            if (RequisitionDetailGUID == undefined || (typeof RequisitionDetailGUID) == (typeof undefined)) {
                RequisitionDetailGUID = '';
            }
            if (ID != '00000000-0000-0000-0000-000000000000')
                SpanQty = $(tblHeader).find("#txtPullQuantity_" + ID + "_" + RequisitionDetailGUID);
            else
                SpanQty = $(tblHeader).find("#txtPullQuantity_" + vToolGUID + "_" + RequisitionDetailGUID);

            var dt = null;
            if ($("#tblToolPull_" + ID + "_" + RequisitionDetailGUID).length > 0)
                dt = $("#tblToolPull_" + ID + "_" + RequisitionDetailGUID).dataTable();

            //if (strpullobj.RequisitionDetailGUID != null && strpullobj.RequisitionDetailGUID != undefined && strpullobj.RequisitionDetailGUID != '' && strpullobj.RequisitionDetailGUID != '00000000-0000-0000-0000-000000000000') {
            //    strpullobj.WorkOrderDetailGUID = null;
            //}
            //else if (strpullobj.WorkOrderDetailGUID != null && strpullobj.WorkOrderDetailGUID != undefined && strpullobj.WorkOrderDetailGUID != '' && strpullobj.WorkOrderDetailGUID != '00000000-0000-0000-0000-000000000000') {
            //    strpullobj.RequisitionDetailGUID = null;
            //}
            //else {
            //    strpullobj.WorkOrderDetailGUID = null;
            //    strpullobj.RequisitionDetailGUID = null;
            //}
            
            if ($("#tblToolPull_" + ID + "_" + RequisitionDetailGUID).length > 0) {
                var currentData = dt.fnGetData();
                $("#tblToolPull_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                    var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                    var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                    var hdnIsForMaintenance = $(tr).find("input[name='hdnIsForMaintenance']").val();

                    if (txtPullQty != "") {
                        var txtLotOrSerailNumber = "";
                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                            var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                        var vSerialNumber = "";
                        var vLotNumber = "";
                        var vExpiration = "";

                        if (hdnSerialNumberTracking == "true")
                            vSerialNumber = $.trim(txtLotOrSerailNumber);
                        if (hdnLotNumberTracking == "true")
                            vLotNumber = $.trim(txtLotOrSerailNumber);
                        if (hdnDateCodeTracking == "true")
                            vExpiration = hdnExpiration;

                        var obj = {
                            "LotOrSerailNumber": $.trim(txtLotOrSerailNumber), "Location": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                                        , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                        , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": vLotNumber
                                        , "ToolGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                        };

                        arrItemDetails.push(obj);
                    }
                });
            }
            
            var pullQty = parseFloat($(SpanQty).val());

            var PullItem = {
                ID: indx,   ToolGUID: strpullobj.ToolGUID,  
                BinID: strpullobj.BinID, Location: strpullobj.BinNumber,
                PullQuantity: pullQty,  
                lstToolPullDetails: arrItemDetails,
                WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID, RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID,
                Technician: strpullobj.Technician, TechnicianGUID: strpullobj.TechnicianGUID, ToolCheckoutUDF1: strpullobj.ToolCheckoutUDF1,
                ToolCheckoutUDF2: strpullobj.ToolCheckoutUDF2, ToolCheckoutUDF3: strpullobj.ToolCheckoutUDF3,
                ToolCheckoutUDF4: strpullobj.ToolCheckoutUDF4, ToolCheckoutUDF5: strpullobj.ToolCheckoutUDF5,
                IsMaintenance: strpullobj.IsForMaintenance
            };
            ArrItem.push(PullItem);
        });

        if (ArrItem.length > 0) {
            PullMultipleToolNew(ArrItem);
        }
    }
    else {
        alert(ErrorMessage);
    }
}
function ToolCheckOutAllNewFlow1() {
    var ArrItem = new Array();
    var arrItemDetails;
    //alert("In new flow");
    var ErrorMessage = ValidateAllCheckOut();

    if (ErrorMessage == "") {
        $("#DivPullSelection1").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {

            var strpullobj = JSON.parse($(tblHeader).find("input[name='hdnToolAssetPullMasterDTO']").val());
            arrItemDetails = new Array();
            var ID = $(tblHeader).prop("id").split('_')[1];
            var vToolGUID = strpullobj.ToolGUID;
            var RequisitionDetailGUID = $(tblHeader).prop("id").split('_')[2];
            var SpanQty = 0;
            if (RequisitionDetailGUID == undefined || (typeof RequisitionDetailGUID) == (typeof undefined)) {
                RequisitionDetailGUID = '';
            }
            if (ID != '00000000-0000-0000-0000-000000000000')
                SpanQty = $(tblHeader).find("#txtPullQuantity_" + ID + "_" + RequisitionDetailGUID);
            else
                SpanQty = $(tblHeader).find("#txtPullQuantity_" + vToolGUID + "_" + RequisitionDetailGUID);

            var dt = null;
            if ($("#tblToolPull_" + ID + "_" + RequisitionDetailGUID).length > 0)
                dt = $("#tblToolPull_" + ID + "_" + RequisitionDetailGUID).dataTable();

            

            if ($("#tblToolPull_" + ID + "_" + RequisitionDetailGUID).length > 0) {
                var currentData = dt.fnGetData();
                $("#tblToolPull_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                    var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                    var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                    var hdnIsForMaintenance = $(tr).find("input[name='hdnIsForMaintenance']").val();

                    if (txtPullQty != "") {
                        var txtLotOrSerailNumber = "";
                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                            var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                        var vSerialNumber = "";
                        var vLotNumber = "";
                        var vExpiration = "";

                        if (hdnSerialNumberTracking == "true")
                            vSerialNumber = $.trim(txtLotOrSerailNumber);
                        if (hdnLotNumberTracking == "true")
                            vLotNumber = $.trim(txtLotOrSerailNumber);
                        if (hdnDateCodeTracking == "true")
                            vExpiration = hdnExpiration;

                        var obj = {
                            "LotOrSerailNumber": $.trim(txtLotOrSerailNumber), "Location": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                            , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                            , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": vLotNumber
                            , "ToolGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                        };

                        arrItemDetails.push(obj);
                    }
                });
            }

            var pullQty = parseFloat($(SpanQty).val());

            var PullItem = {
                ID: indx, ToolGUID: strpullobj.ToolGUID,
                BinID: strpullobj.BinID, Location: strpullobj.BinNumber,
                PullGUID: $("div#CtabNew").find("input#GUID").val(),
                PullQuantity: pullQty,
                lstToolPullDetails: arrItemDetails,
                WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID, RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID,
                Technician: strpullobj.Technician, TechnicianGUID: strpullobj.TechnicianGUID, ToolCheckoutUDF1: strpullobj.ToolCheckoutUDF1,
                ToolCheckoutUDF2: strpullobj.ToolCheckoutUDF2, ToolCheckoutUDF3: strpullobj.ToolCheckoutUDF3,
                ToolCheckoutUDF4: strpullobj.ToolCheckoutUDF4, ToolCheckoutUDF5: strpullobj.ToolCheckoutUDF5,
                IsMaintenance: strpullobj.IsForMaintenance
            };
            ArrItem.push(PullItem);
        });

        if (ArrItem.length > 0) {
            ToolKitLineItemNew(ArrItem);
        }
    }
    else {
        alert(ErrorMessage);
    }
}
function ValidateAllCheckOut() {
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    $("#DivPullSelection").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
        var ID = $(tblHeader).prop("id").split('_')[1];
        var RequisitionDetailGUID = $(tblHeader).prop("id").split('_')[2];
        var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + ID + "_" + RequisitionDetailGUID);

        var TotalEntered = 0;
        if ($("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).length > 0) {
            $("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                if ($(tr).find("input[type='text'][name='txtPullQty']").length > 0) {
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                    var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

                    if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
                        errormsg = "\n" + MsgPullMoreQuantityValidation;
                        isMoreQty = true;
                        return errormsg;
                    }

                    TotalEntered = TotalEntered + parseFloat(txtPullQty);
                }
            });

            if (isMoreQty == false) {
                var pullQty = parseFloat($(SpanQty).val().toString());
                if (TotalEntered != pullQty) {
                    ////errormsg = errormsg + "\nentered :" + TotalEntered + "\tPull Qty :" + pullQty;
                    errormsg = errormsg + "\n" + MsgQTYCheckOutQTYValidation.replace("{0}", TotalEntered).replace("{1}", pullQty);
                }
            }
            else {
                errormsg = MsgCheckoutMoreQuantityValidation;
            }
        }
    });

    return errormsg;
}
function PullMultipleToolNew(ArrItem) {
    $('#DivLoading').show();
    $.ajax({
        type: "POST",
        url: PullSerialsAndLotsNewUrl,
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(ArrItem),
        success: function (RetData) {
           
            var errorMessage = "";
            var _RequisitionDetailGUID = "";
            $.each(RetData, function (indx, RetDataItem) {
                
                if (RetDataItem.ErrorMessage != null && RetDataItem.ErrorMessage != undefined && RetDataItem.ErrorMessage.trim() > '') {
                    errorMessage += RetDataItem.ErrorMessage + "<br />";
                }
                else if (RetDataItem.ErrorList.length > 0) {
                    $.each(RetDataItem.ErrorList, function (indx, ErrorListItem) {
                        errorMessage += ErrorListItem.ErrorMessage + "<br />";
                    });
                }
                else {
                    if (RetDataItem.RequisitionDetailsGUID != null && RetDataItem.RequisitionDetailsGUID != undefined && RetDataItem.RequisitionDetailsGUID.trim() != '') {
                        $('#divItem_' + RetDataItem.ToolGUID + '_' + RetDataItem.RequisitionDetailsGUID).attr('style', '');
                        $('#divItem_' + RetDataItem.ToolGUID + '_' + RetDataItem.RequisitionDetailsGUID).remove();
                    }
                    else {
                        $('#divItem_' + RetDataItem.ToolGUID + '_').attr('style', '');
                        $('#divItem_' + RetDataItem.ToolGUID + '_').remove();
                        $('#divItem_' + RetDataItem.ToolGUID ).attr('style', '');
                        $('#divItem_' + RetDataItem.ToolGUID ).remove();
                    }
                }

                _RequisitionDetailGUID = RetDataItem.RequisitionDetailsGUID;
            });

            $('#DivLoading').hide();
            if (errorMessage != "") {
                $.modal.impl.close();
                errorMessage = "<b>" + MsgNoPullReasons +"</b><br /><br />" + errorMessage;
                $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                //$('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
                $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html(errorMessage);
                $('#dlgCommonErrorMsgPopup').modal();
                $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                $('#simplemodal-overlay').css("z-index", "1103");
                $('#simplemodal-container').css("z-index", "1104");
            }
            else {
                var IsFromToolMaster = false;

                if ($("table[id^='RequisitionItemsTable']").length > 0) {
                    IsFromToolMaster = false;
                }
                else if ($("input[type='hidden'][name^='hdnToolAssetPullMasterDTO_']").length > 0) {
                    IsFromToolMaster = true;
                }
                else {
                    IsFromToolMaster = false;
                }

                if (_RequisitionDetailGUID != null && _RequisitionDetailGUID != undefined && _RequisitionDetailGUID.trim() != '') {
                    $.ajax({
                        "url": CloseRequisitionIfPullCompletedUrl,
                        data: { RequisitionDetailGUID: _RequisitionDetailGUID },
                        "async": false,
                        cache: false,
                        "dataType": "json",
                        success: function (response) {
                            if (IsFromPullMaster == true) {
                                $.modal.impl.close();
                                $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                                $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("<b>" + MsgPullDoneSuccess +"</b><br /><br />");
                                $('#dlgCommonErrorMsgPopup').modal();
                                $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                                $('#simplemodal-overlay').css("z-index", "1103");
                                $('#simplemodal-container').css("z-index", "1104");
                                if ($('div[id^="divItem_"]').length <= 0) {
                                    $('#DivPullSelection').dialog('close');
                                    if ($('#ItemModeDataTable').length > 0)
                                        $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                                }
                            }
                            else {
                                $("#spanGlobalMessage").html(MsgAllPulldone);
                                $('div#target').fadeToggle();
                                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                                if ($('div[id^="divItem_"]').length <= 0) {
                                    $('#DivPullSelection').dialog('close');
                                    if ($('#ItemModeDataTable').length > 0)
                                        $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                                }
                            }
                        }
                    });
                }
                else {
                    if (IsFromToolMaster == true) {
                        $.modal.impl.close();
                        $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                        $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("<b>" + MsgCheckoutDoneSuccess +"</b><br /><br />");
                        $('#dlgCommonErrorMsgPopup').modal();
                        $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                        $('#simplemodal-overlay').css("z-index", "1103");
                        $('#simplemodal-container').css("z-index", "1104");
                        if ($('div[id^="divTool_"]').length <= 0) {
                            $('#DivPullSelection').dialog('close');
                            $('#myDataTable').dataTable().fnStandingRedraw();
                        }
                    }
                    else {
                        $("#spanGlobalMessage").html(MsgAllCheckoutDoneSucess);
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        if ($('div[id^="divItem_"]').length <= 0) {
                            $('#DivPullSelection').dialog('close');
                            $('#myDataTable').dataTable().fnStandingRedraw();
                        }
                    }
                }
            }
        },
        error: function (err) {
            $('#DivLoading').hide();
            $.modal.impl.close();
            $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
            //$('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
            $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html(MsgRescommonError);
            $('#dlgCommonErrorMsgPopup').modal();
            $('#dlgCommonErrorMsgPopup').css("z-index", "1004");
            $('#simplemodal-overlay').css("z-index", "1003");
            $('#simplemodal-container').css("z-index", "1004");
        },
        complete: function () {
        }
    });
}
function ToolKitLineItemNew(ArrItem) {
    $('#DivLoading').show();
    $.ajax({
        type: "POST",
        url: AddSerialToolKitLineItem,
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(ArrItem),
        success: function (RetData) {
            
            var errorMessage = "";
            var _RequisitionDetailGUID = "";
            $.each(RetData, function (indx, RetDataItem) {

                if (RetDataItem.ErrorMessage != null && RetDataItem.ErrorMessage != undefined && RetDataItem.ErrorMessage.trim() > '') {
                    errorMessage += RetDataItem.ErrorMessage + "<br />";
                }
                else if (RetDataItem.ErrorList.length > 0) {
                    $.each(RetDataItem.ErrorList, function (indx, ErrorListItem) {
                        errorMessage += ErrorListItem.ErrorMessage + "<br />";
                    });
                }
                else {
                    if (RetDataItem.RequisitionDetailsGUID != null && RetDataItem.RequisitionDetailsGUID != undefined && RetDataItem.RequisitionDetailsGUID.trim() != '') {
                        $('#divItem_' + RetDataItem.ToolGUID + '_' + RetDataItem.RequisitionDetailsGUID).attr('style', '');
                        $('#divItem_' + RetDataItem.ToolGUID + '_' + RetDataItem.RequisitionDetailsGUID).remove();
                    }
                    else {
                        $('#divItem_' + RetDataItem.ToolGUID + '_').attr('style', '');
                        $('#divItem_' + RetDataItem.ToolGUID + '_').remove();
                        $('#divItem_' + RetDataItem.ToolGUID).attr('style', '');
                        $('#divItem_' + RetDataItem.ToolGUID).remove();
                    }
                }

                _RequisitionDetailGUID = RetDataItem.RequisitionDetailsGUID;
            });

            $('#DivLoading').hide();
            if (errorMessage != "") {
                $.modal.impl.close();
                errorMessage = "<b>" + MsgNoPullReasons +"</b><br /><br />" + errorMessage;
                $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                //$('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
                $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html(errorMessage);
                $('#dlgCommonErrorMsgPopup').modal();
                $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                $('#simplemodal-overlay').css("z-index", "1103");
                $('#simplemodal-container').css("z-index", "1104");
            }
            else {
                var IsFromToolMaster = false;

                if ($("table[id^='RequisitionItemsTable']").length > 0) {
                    IsFromToolMaster = false;
                }
                else if ($("input[type='hidden'][name^='hdnToolAssetPullMasterDTO_']").length > 0) {
                    IsFromToolMaster = true;
                }
                else {
                    IsFromToolMaster = false;
                }

                if (_RequisitionDetailGUID != null && _RequisitionDetailGUID != undefined && _RequisitionDetailGUID.trim() != '') {
                    $.ajax({
                        "url": CloseRequisitionIfPullCompletedUrl,
                        data: { RequisitionDetailGUID: _RequisitionDetailGUID },
                        "async": false,
                        cache: false,
                        "dataType": "json",
                        success: function (response) {

                            if (IsFromPullMaster == true) {
                                $.modal.impl.close();
                                $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                                $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("<b>" + MsgItemAddedSuccess +"</b><br /><br />");
                                $('#dlgCommonErrorMsgPopup').modal();
                                $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                                $('#simplemodal-overlay').css("z-index", "1103");
                                $('#simplemodal-container').css("z-index", "1104");
                                if ($('div[id^="divItem_"]').length <= 0) {
                                    $('#DivPullSelection1').dialog('close');
                                    if ($('#ItemModeDataTable').length > 0)
                                        $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                                }
                            }
                            else {
                                $("#spanGlobalMessage").html(MsgAllItemAdded);
                                $('div#target').fadeToggle();
                                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                                if ($('div[id^="divItem_"]').length <= 0) {
                                    $('#DivPullSelection1').dialog('close');
                                    if ($('#ItemModeDataTable').length > 0)
                                        $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                                }
                            }
                        }
                    });
                }
                else {
                    
                    if (IsFromToolMaster == true) {
                        $.modal.impl.close();
                        $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                        $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("<b>" + MsgItemAddedSuccess +"</b><br /><br />");
                        $('#dlgCommonErrorMsgPopup').modal();
                        $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                        $('#simplemodal-overlay').css("z-index", "1103");
                        $('#simplemodal-container').css("z-index", "1104");
                        if ($('div[id^="divTool_"]').length <= 0) {
                            $('#DivPullSelection1').dialog('close');
                            $('#ToolModeDataTablePopup').dataTable().fnStandingRedraw();
                        }
                    }
                    else {
                        $("#spanGlobalMessage").html(MsgAllItemAdded);
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        if ($('div[id^="divItem_"]').length <= 0) {
                            $('#DivPullSelection1').dialog('close');
                            $('#ToolModeDataTablePopup').dataTable().fnStandingRedraw();
                        }
                    }
                }
            }
        },
        error: function (err) {
            $('#DivLoading').hide();
            $.modal.impl.close();
            $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
            //$('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
            $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html(MsgRescommonError);
            $('#dlgCommonErrorMsgPopup').modal();
            $('#dlgCommonErrorMsgPopup').css("z-index", "1004");
            $('#simplemodal-overlay').css("z-index", "1003");
            $('#simplemodal-container').css("z-index", "1004");
        },
        complete: function () {
        }
    });
}
function ValidateSinglePull(vToolGUID, RequisitionDetailGUID) {

    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    var dtID = "#tblToolPull_" + vToolGUID + "_" + RequisitionDetailGUID;
    
    var SpanQty = $("#DivPullSelection").find("#txtPullQuantity_" + vToolGUID + "_" + RequisitionDetailGUID);

    var TotalEntered = 0;
    $("#tblToolPull_" + vToolGUID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
        var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
        var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

        if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
            errormsg = "\n" + MsgPullMoreQuantityValidation;
            isMoreQty = true;
            return errormsg;
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
        errormsg = MsgPullMoreQuantityValidation;
    }

    return errormsg;
}
function ValidateSinglePull1(vToolGUID, RequisitionDetailGUID) {

    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    var dtID = "#tblToolPull_" + vToolGUID + "_" + RequisitionDetailGUID;

    var SpanQty = $("#DivPullSelection1").find("#txtPullQuantity_" + vToolGUID + "_" + RequisitionDetailGUID);

    var TotalEntered = 0;
    $("#tblToolPull_" + vToolGUID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
        var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
        var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

        if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
            errormsg = "\n" + MsgPullMoreQuantityValidation;
            isMoreQty = true;
            return errormsg;
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
        errormsg = MsgPullMoreQuantityValidation;
    }

    return errormsg;
}
function IsLotSerialExistsInCurrentLoaded(strIds, SerialLot) {
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

function GetMasterGridColumns() {
    objColumns = masterGridColumns;
}