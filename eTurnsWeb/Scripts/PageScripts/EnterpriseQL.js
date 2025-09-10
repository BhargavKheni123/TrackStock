
$("#frmEnterpriseQL").submit(function (e) {

    $.validator.unobtrusive.parse("#frmEnterpriseQL");
    e.preventDefault();
});

$(document).ready(function () {
    bindRoomList(EntQLID, selectedRooms);
    
    BindQLDetail(false,true);
});

function onSuccess(response) {
    if (response !== undefined && response != null && response.Message != undefined && response.Message != null) {
        IsRefreshGrid = true;
        showNotificationDialog();
        $("#spanGlobalMessage").html(response.Message);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');

        if (response.Status !== undefined && response.Status != null && response.Status == "ok") {
            SwitchTextTab(0, 'CreateEnterpriseQL', 'frmEnterpriseQL');
        }
        else if (response.Status !== undefined && response.Status != null && response.Status == "fail") {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        }
        else if (response.Status !== undefined && response.Status != null && response.Status == "duplicate") {
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
        }
    }
}

function onFailure(message) {
    //$("#spanGlobalMessage").text(message.NotificationSave);
    //$("#spanGlobalMessage").text(message.statusText);
    //$('div#target').fadeToggle();
    //$("div#target").delay(DelayTime).fadeOut(FadeOutTime);
    //$("#ScheduleName").focus();
}

function onBegin() {

}
$('#btnCancel').click(function (e) {
    //            if (IsRefreshGrid)
    //                $('#NarroSearchClear').click();
    SwitchTextTab(0, 'CreateEnterpriseQL', 'frmEnterpriseQL');
    if (oTable !== undefined && oTable != null) {
        oTable.fnDraw();
    }
    //$('#NarroSearchClear').click();
});

function bindRoomList(objCompanyID, objExistSelected) {

    var objList = '';
    var objSelect = '';

    var objExistselect = objExistSelected.split(',');
    $.ajax({
        type: "POST",
        url: "/EnterpriseQL/GetRoomList",
        data: JSON.stringify({ 'SelectedRoomIds': objExistSelected, 'Id': EntQLID}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (message) {
            objList = message.RoomList;
            objSelect = message.Selected;
        },
        error: function (response) {

        }
    });

    $("#ddlRRoom > option").remove();
    $("#ddlRRoom").multiselect("refresh");
    var s = '';
    for (var i = 0; i < objList.length; i++) {
        s += '<option value=' + objList[i].key + '  >' + objList[i].value + '</option>';
    }

    $("#ddlRRoom").append(s);
    $("#ddlRRoom").multiselect({
        checkAllText: Check,
        uncheckAllText: UnCheck,
        noneSelectedText: lblRoom, selectedList: 5,
        selectedText: function (numChecked, numTotal, checkedItems) {
            return lblRoom + ' ' + numChecked + ' ' + selected;
        }
    }).unbind("multiselectclick multiselectcheckall multiselectuncheckall").bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
        if (ui.checked) {
            $("#ddlRRoom").multiselect("widget").find(":checkbox[value='" + ui.value + "']").attr("checked", "checked");
            $("#ddlRRoom option[value='" + ui.value + "']").attr("selected", 1);
            SetLabelText('lblselectedRoom', 'ddlRRoom');
            var SelectedRoomID = getSelectedID('ddlRRoom');
            $("body").find("input#RoomIds").val(SelectedRoomID);
            BindSupplierPartNoDDL();
        }
        else {
            var SelectedRoomID = getSelectedID('ddlRRoom');
            $("body").find("input#RoomIds").val(SelectedRoomID);
            SetLabelText('lblselectedRoom', 'ddlRRoom');
            BindSupplierPartNoDDL();
        }
    }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });
    $("#ddlRRoom").multiselect("refresh");

    var selectedLtext = '';
    if (objSelect != null && objSelect != "0") {
        for (var i = 0; i < objList.length; i++) {

            if (objList[i].key == objSelect) {
                $("#ddlRRoom").multiselect("widget").find(":checkbox[value='" + objList[i].key + "']").attr("checked", "checked");
                $("#ddlRRoom option[value='" + objList[i].key + "']").attr("selected", 1);
                $("#ddlRRoom").multiselect("refresh");

            }
        }
    }
    for (var i = 0; i < objExistselect.length; i++) {

        for (var j = 0; j < objList.length; j++) {

            if (objList[j].key == objExistselect[i]) {
                $("#ddlRRoom").multiselect("widget").find(":checkbox[value='" + objList[j].key + "']").attr("checked", "checked");
                $("#ddlRRoom option[value='" + objList[j].key + "']").attr("selected", 1);
                $("#ddlRRoom").multiselect("refresh");

            }
        }
    }
    SetLabelText('lblselectedRoom', 'ddlRRoom');
    var SelectedRoomID = getSelectedID('ddlRRoom');
    $("body").find("input#RoomIds").val(SelectedRoomID);
    BindSupplierPartNoDDL();
}
function SetLabelText(objlblID, objddlID) {

    var selectedlist = '';
    $('.ui-multiselect-menu [type="checkbox"][id^="ui-multiselect-' + objddlID + '-option"]').each(function () {
        if ($(this).attr('checked') == 'checked') {
            if (selectedlist.length > 0)
                selectedlist += ',';
            selectedlist += $(this).parent().find('span').text();
        }
    });
    $('#' + objlblID + '').text(selectedlist);

    if ($('#lblselectedRoom').length > 0) {
        var txtLength = $('#lblselectedRoom').text().length;
        if (txtLength > 100) {
            $('.neweditorlabelrm').height("150px");
            $('.neweditorlabelrm').css("overflow", "auto");

        }
        else {
        }
    }

}
function getSelectedID(objID) {
    var selectedlist = '';
    var selectedrecs = $("#" + objID).multiselect("getChecked");
    if (selectedrecs.length > 0) {
        $(selectedrecs).each(function (indx, obj) {
            selectedlist += obj.value + ',';
        });
    }


    //var selectedlist = '';
    //$('.ui-multiselect-menu [type="checkbox"][id^="ui-multiselect-' + objID + '-option"]').each(function () {

    //    if ($(this).attr('checked') == 'checked') {
    //        if (selectedlist.length > 0)
    //            selectedlist += ',';
    //        selectedlist += $(this).val();
    //    }
    //});
    return selectedlist;
}

function DeletetoSeesionBinReplanishSingle(obj) {
    
    var vardelete = false;
    //var vBinID = $(obj).parent().parent().find('#hdnBinID').val();
    var vItemguid = $(obj).parent().parent().find('#hdnItemGUID').val();
    var vhdnID = $(obj).parent().parent().find('#hdnID').val();
    var vhdnGUID = $(obj).parent().parent().find('#hdnGUID').val();
    var supplierPartNoToDelete = $(obj).parent().parent().find('#txtQLDetailNumber').val();//txtQLDetailNumber
    if (SavetoSeesionItemLocationAllNew(vhdnGUID)) {
        if (newGuid != '00000000-0000-0000-0000-000000000000') {
            vhdnGUID = newGuid
        }
        if (vhdnGUID == '00000000-0000-0000-0000-000000000000') {
            //client side remove
            $(obj).parent().parent().remove();
            BindQLDetail(vardelete);
        }
        else {
            if (vItemguid == '') {
                vItemguid = '00000000-0000-0000-0000-000000000000'
            }

            if (vhdnGUID != '') {
                $.ajax({
                    url: DeleteToSeesionQLDetailSingleURL,
                    data: { 'ID': vhdnID, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid },
                    dataType: 'json',
                    type: 'POST',
                    async: false,
                    cache: false,
                    success: function (response) {
                        if (response.status = 'deleted') {
                            //client side remove
                            $(obj).parent().parent().remove();
                            //bind grid
                            $("#ddlSupplierPartNo").multiselect("widget").find(":checkbox[value='" + supplierPartNoToDelete + "']").removeAttr('checked');
                            $("#ddlSupplierPartNo option[value='" + supplierPartNoToDelete + "']").attr("selected", false);
                            $("#ddlSupplierPartNo").multiselect("refresh");
                            $("#ddlSupplierPartNo").multiselect('reload');
                            BindQLDetail(vardelete);                           
                            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                            $("#spanGlobalMessage").html(MsgQuickListDeleted);
                            $('div#target').fadeToggle();
                            //$("div#target").delay(2000).fadeOut(200);
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

                        }
                        else if (response.status = 'error') {
                            alert(MsgErrorInProcess);
                        }
                    }
                });
            }
            else {
                $(obj).parent().parent().remove();
                BindQLDetail(obj);

            }
        }
    }
    return false;
}

function SavetoSeesionItemLocationAllNew(Guid) {
    if ($("#QLDetail").length > 0) {
        var TempSuprows3 = $("#QLDetail").dataTable().fnGetNodes();

        if (TempSuprows3 != null && TempSuprows3.length > 0) {

            for (var i = 0; i < TempSuprows3.length; i++) {
                var QLDetailNumberValue = $(TempSuprows3[i]).find('#txtQLDetailNumber').val();
                if (!(typeof (QLDetailNumberValue) == "undefined" && QLDetailNumberValue == null && QLDetailNumberValue.trim().length < 1)) {

                    if (SavetoSeesionBinReplanishNew(TempSuprows3[i], Guid) == false) {
                        return false;
                    }
                }
            }
        }
    }
    return true;
}

function SavetoSeesionBinReplanishNew(obj1, guid) {

    var vQLDetailId = $(obj1).find('#hdnID').val();
    var vQLDetailNumber = $(obj1).find('#txtQLDetailNumber').val();

    if ((parseFloat($(obj1).find('#txtQuantity').val()) || 0) < 1) {
        $(obj1).find('#txtQuantity').val("1");
    }

    var vQuantity = $(obj1).find('#txtQuantity').val();

    var vItemguid = $(obj1).find('#hdnItemGUID').val();
    if (vItemguid == "") {
        vItemguid = ItemGUID;
    }

    var vhdnID = $(obj1).find('#hdnID').val();
    var vhdnGUID = $(obj1).find('#hdnGUID').val();
    var vhdnSessionSr = $(obj1).find('#hdnSessionSr').val();

    if (vQLDetailId == 0 && vQLDetailNumber == '') {
        return true;
    }

    $.ajax({
        url: SaveToSeesionQLDetailSingleURL,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'QLDetailNumber': vQLDetailNumber, 'Quantity': vQuantity },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'ok') {

                if (guid == vhdnGUID) {
                    newGuid = response.newGUID;
                }
            }
            else {
                alert(MsgErrorInProcess);
            }
        }
    });
    return true;
}

function BindQLDetail(obj, isFromReady) {
    
    $('#divEnterpriseQLDetail').empty();
    $('#DivLoading').show();
    $.get(LoadEnterpriseQuickListDetailURL + '?ItemGUID=' + ItemGUID + '&AddCount=0', function (data) {
        $('#divEnterpriseQLDetail').html(data);
        //if (typeof (isFromReady) != "undefined" && isFromReady != null && isFromReady == true) {
        //    BindSupplierPartNoDDL();
        //}
        $('#DivLoading').hide();

    });
}

function BindSupplierPartNoDDL() {

    var objList = '';
    var objSelect = '';
   
    $.ajax({
        type: "POST",
        url: GetAllSupplierPartNoURL,
        data: JSON.stringify({ 'SupplierPartNo': " ", 'RoomIds': getSelectedID('ddlRRoom'), 'GUID': ItemGUID}),//JSON.stringify({ 'SelectedRoomIds': objExistSelected, 'Id': EntQLID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (Items) {
            
            objList = Items.supplierPartNos;
            objSelect = Items.selected;
        },
        error: function (response) {

        }
    });

    $("#ddlSupplierPartNo > option").remove();
    $("#ddlSupplierPartNo").multiselect("refresh");
    var s = '';
    for (var i = 0; i < objList.length; i++) {
        
        s += '<option value="' + objList[i].SupplierNumber + '" title="' + objList[i].RoomNames + '" >' + objList[i].SupplierPartNo + '</option>';
    }
    $("#ddlSupplierPartNo").append(s);
    $("#ddlSupplierPartNo").multiselect({
        checkAllText: Check,
        uncheckAllText: UnCheck,
        noneSelectedText: lblSupplierPartNo, selectedList: 5,
        selectedText: function (numChecked, numTotal, checkedItems) {
            return lblSupplierPartNo + ' ' + numChecked + ' ' + selected;
        }
    }).unbind("multiselectclick multiselectcheckall multiselectuncheckall").bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
        if (ui.checked) {
            $("#ddlSupplierPartNo").multiselect("widget").find(":checkbox[value='" + ui.value + "']").attr("checked", "checked");
            $("#ddlSupplierPartNo option[value='" + ui.value + "']").attr("selected", 1);
            //SetLabelText('lblselectedRoom', 'ddlSupplierPartNo');
            //var SelectedSPN = getSelectedID('ddlSupplierPartNo');
            //$("body").find("input#RoomIds").val(SelectedRoomID);

        }
        else {
            //var SelectedRoomID = getSelectedID('ddlSupplierPartNo');
            //$("body").find("input#RoomIds").val(SelectedRoomID);
            //SetLabelText('lblselectedRoom', 'ddlSupplierPartNo');
        }
    }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });
    $("#ddlSupplierPartNo").multiselect("refresh");
    for (var i = 0; i < objSelect.length; i++) {

        for (var j = 0; j < objList.length; j++) {

            if (objList[j].SupplierNumber == objSelect[i]) {
                
                $("#ddlSupplierPartNo").multiselect("widget").find(":checkbox[value='" + objList[j].SupplierNumber + "']").attr("checked", "checked");
                $("#ddlSupplierPartNo option[value='" + objList[j].SupplierNumber + "']").attr("selected", 1);
                $("#ddlSupplierPartNo").multiselect("refresh");
            }
        }
    }
}

function AddNewBinReplanish() {
    RemoveLeadingTrailingSpace("divQLDetail");

    if (SavetoSeesionItemLocationAll()) {        
        var selectedSupplierPartNos = getSelectedID('ddlSupplierPartNo');
        $('#divEnterpriseQLDetail').empty();
        $('#DivLoading').show();
        if (typeof (selectedSupplierPartNos) != "undefined" && selectedSupplierPartNos != null && selectedSupplierPartNos.length > 0) {
            var tmpselectedSPN = selectedSupplierPartNos.split(',');
            var selectedSPN = _.uniq(tmpselectedSPN);
            $.get(LoadEnterpriseQLDetailBySupplierPartNoURL + '?ItemGUID=' + ItemGUID + '&AddCount=' + selectedSupplierPartNos.length + '&SuppllierPartNos=' + selectedSPN, function (data) {
                $('#divEnterpriseQLDetail').html(data);
                $('#DivLoading').hide();

            });
        }
        else {
            $.get(LoadEnterpriseQuickListDetailURL + '?ItemGUID=' + ItemGUID + '&AddCount=1', function (data) {
                $('#divEnterpriseQLDetail').html(data);
                $('#DivLoading').hide();

            });
        }
        
    }
    return false;
}

function SavetoSeesionItemLocationAll() {
    if ($("#QLDetail").length > 0) {
        RemoveLeadingTrailingSpace("divQLDetail");
        var TempSuprows3 = $("#QLDetail").dataTable().fnGetNodes();
        var iCountSup = 0;
        var iCountSupDefault = 0;

        if (TempSuprows3 != null && TempSuprows3.length > 0) {

            for (var i = 0; i < TempSuprows3.length; i++) {
                var currentRow = TempSuprows3[i];
                var QLDetailNumberValue = $(currentRow).find('#txtQLDetailNumber').val();
                //if (!($(currentRow).find('#txtQLDetailNumber').val() == '' && $(currentRow).find('#txtQuantity').val() == '')) {

                if (typeof (QLDetailNumberValue) == "undefined" || QLDetailNumberValue == null || QLDetailNumberValue.trim().length < 1) {
                        $(TempSuprows3[i]).css("background-color", "red");
                        $(TempSuprows3[i]).find('#txtQLDetailNumber').css("background-color", "yellow");
                        $(TempSuprows3[i]).find('#txtQLDetailNumber').focus();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(msgQuickListDetailNoRequired);
                        $('div#target').fadeToggle();
                        //$("div#target").delay(2000).fadeOut(200);s
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        return false;
                    }

                    if (SavetoSeesionBinReplanish(TempSuprows3[i]) == false) {
                        return false;
                    }
                //}
            }
        }
    }
    return true;
}

function SavetoSeesionBinReplanish(obj1) {
    var vQLDetailId = $(obj1).find('#hdnID').val();
    var vQLDetailNumber = $(obj1).find('#txtQLDetailNumber').val();

    if ((parseFloat($(obj1).find('#txtQuantity').val()) || 0) < 1) {
        $(obj1).find('#txtQuantity').val("1");
    }
    var vQuantity = $(obj1).find('#txtQuantity').val();
    var vItemguid = $(obj1).find('#hdnItemGUID').val();

    if (vItemguid == "") {
        vItemguid = ItemGUID;
    }

    var vhdnID = $(obj1).find('#hdnID').val();
    var vhdnGUID = $(obj1).find('#hdnGUID').val();
    var vhdnSessionSr = $(obj1).find('#hdnSessionSr').val();

    if (vQLDetailId == 0 && vQLDetailNumber == '') {
        return true;
    }

    if (typeof (vQLDetailNumber) == "undefined" || vQLDetailNumber == null || vQLDetailNumber.trim().length < 1) {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(msgQuickListDetailNoRequired);
        $('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $(obj1).find('#txtQLDetailNumber').focus();
        return false;
    }

    var TempSuprows = $("#QLDetail").dataTable().fnGetNodes();
    var iCountSup = 0;

    if (TempSuprows != null && TempSuprows.length > 0) {

        for (var i = 0; i < TempSuprows.length; i++) {

            if ($(TempSuprows[i]).find('#txtQLDetailNumber').val().trim().toLowerCase() == vQLDetailNumber.trim().toLowerCase()) {

                iCountSup += 1;
                if (iCountSup > 1) {
                    $(TempSuprows[i]).css("background-color", "red");
                    $(TempSuprows[i].cells[1].getElementsByTagName('input')).css("background-color", "yellow");
                    $(TempSuprows[i].cells[1].getElementsByTagName('input').txtQLDetailNumber).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(msgQuickListDetailNoAlreadyAdded);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    return false;
                }
            }
        }
    }

    //if (typeof (vQuantity) == "undefined" || vQuantity == null || (parseFloat(vQuantity) || 0) <= 0) {
    //    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //    $("#spanGlobalMessage").text('Quantity must be greater than Zero.');
    //    $('div#target').fadeToggle();
    //    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

    //    $(obj1).css("background-color", "red");
    //    $(obj1).find('#txtQuantity').css("background-color", "yellow");
    //    $(obj1).find('#txtQuantity').focus();
    //    return false;
    //}

    $.ajax({
        url: SaveToSeesionQLDetailSingleURL,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'QLDetailNumber': vQLDetailNumber, 'Quantity': vQuantity },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'ok') {
            }
            else {
                alert(MsgErrorInProcess);
            }
        }
    });
    return true;
}

function onBegin(xhr) {
    $("#frmEnterpriseQL").find("input[type='hidden']").removeProp("disabled")
    return CheckBeforeSave();
}

function CheckBeforeSave()
{
    if (!SavetoSeesionItemLocationAll()) {
        return false;
    }
    
    var SelectedRoomID = getSelectedID('ddlRRoom');
    //$("body").find("input#RoomIds").val(SelectedRoomID);
    $("body").find("input#RoomIds").val(SelectedRoomID);
    
    //if (SelectedRoomID == null || typeof (SelectedRoomID) == "undefined" || SelectedRoomID.length < 1) {
    //    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //    $("#spanGlobalMessage").text('At least one room must be selected.');
    //    $('div#target').fadeToggle();        
    //    $("div#target").delay(DelayTime).fadeOut(FadeOutTime); //$("div#target").delay(2000).fadeOut(200);
    //    return false;
    //}
    
    return true;
}