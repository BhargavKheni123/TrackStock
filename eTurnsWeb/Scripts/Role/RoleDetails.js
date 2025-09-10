var hdnSelectedRoomReplanishmentValue = $("#hdnSelectedRoomReplanishmentValue");
var hdnSelectedRoomAccessValue = $("#hdnSelectedRoomAccessValue");
var hdnRoleID = $("#hiddenID");
var SelectedModuleList = new Array();
var SelectedNonModuleList = new Array();
var SelectedDefaultSettings = new Array();
var SelectedRooms = new Array();
var SelectedCompanies = new Array();
var SelectedEnterprises = new Array();
var hdnSelectedModuleList = $("#hdnSelectedModuleList");
var hdnSelectedNonModuleList = $("#hdnSelectedNonModuleList");
var hdnSelectedDefaultSettings = $("#hdnSelectedDefaultSettings");
var ddlDefaultPermissionRooms = $("#ddlDefaultPermissionRooms");
var hdnCurrentSelectedRoom = 0;
//--------- ----------
$("form").submit(function (e) {

    if (RoomValidation() == false) {
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgRoomAccessRequired);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        return false;
    }

    //-----Set control when direct click without room selection ------\
    var SelectedRoomReplanishment = '';
    $("#RoomReplanishment > option").each(function () {
        if (this.selected == true) {
            if (SelectedRoomReplanishment == '')
                SelectedRoomReplanishment = this.value;
            else
                SelectedRoomReplanishment += ',' + this.value;
        }
    });
    $("#hdnSelectedRoomReplanishmentValue").val('');
    $("#hdnSelectedRoomReplanishmentValue").val(SelectedRoomReplanishment);

    SetSelectedModule_NonModulePermissions();

    $.ajax({
        type: "POST",
        url: url_SaveToRolePermissionsToSession,
        data: "{'RoomID': '" + hdnCurrentSelectedRoom + "' ,'RoleID':'" + hdnRoleID.val() + "','SelectedModuleList':'" + hdnSelectedModuleList.val() + "','SelectedNonModuleList':'" + hdnSelectedNonModuleList.val() + "','SelectedDefaultSettings':'" + hdnSelectedDefaultSettings.val() + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (message) {

        },
        error: function (response) {
            // through errror message

        }
    });

    //------------

    $.validator.unobtrusive.parse("#frmRole");
    if ($(this).valid()) {
    }
    e.preventDefault();
});



$(document).ready(function () {
    LoadEnterpriseSelectBox();
    LoadComanySelectBox();
    LoadRoomSelectBox();
    LoadReplenishRoomSelectBox();
    SetAccessLevels($("#drpUserType").val());
    if ($("#hdnSelectedModuleList").val() != '') {
        SelectedModuleList = $("#hdnSelectedModuleList").val().split(',');
    }

    if ($("#hdnSelectedNonModuleList").val() != '') {
        SelectedNonModuleList = $("#hdnSelectedNonModuleList").val().split(',');
    }
    if ($("#hdnSelectedDefaultSettings").val() != '') {
        SelectedDefaultSettings = $("#hdnSelectedDefaultSettings").val().split(',');
    }


    $('#btnCancel').click(function (e) {
        //            if (IsRefreshGrid)
        //                $('#NarroSearchClear').click();
        SwitchTextTab(0, 'RoleCreate', 'frmRole');
        if (oTable !== undefined && oTable != null) {
            oTable.fnDraw();
        }
    });

    $('#ddlSelectedRooms').change(function (e) {
        RoomChanged();
    });

    $("#ddlDefaultPermissionRooms").multiselect(
                                {
                                    noneSelectedText: 'Room  ', selectedList: 5,
                                    selectedText: function (numChecked, numTotal, checkedItems) {
                                        return 'Room : ' + numChecked + ' ' + selected;
                                    }
                                }
                    ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                    });
    EnableDisableRoomsControls();

});

$(function () {
    $("#tabs").tabs();
});
//-----------------------------------------------


function RoomChanged() {
    var SelectedRoomID = 0; // $("#ddlSelectedRooms").val();
    $("#ddlSelectedRooms > option").each(function () {
        if (this.selected == true) {
            SelectedRoomID = this.value;
        }
    });
    SetDefaultPermissionRooms();

    SetSelectedModule_NonModulePermissions();

    $.ajax({
        type: "POST",
        url: url_SaveToRolePermissionsToSession,
        data: "{'RoomID': '" + hdnCurrentSelectedRoom + "' ,'RoleID':'" + hdnRoleID.val() + "','SelectedModuleList':'" + hdnSelectedModuleList.val() + "','SelectedNonModuleList':'" + hdnSelectedNonModuleList.val() + "','SelectedDefaultSettings':'" + hdnSelectedDefaultSettings.val() + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (message) {
            ClearRoomAccessHidden();

            $("#CreateRolePermissionDIV").load("/Master/RolePermissionCreate", { RoomID: SelectedRoomID, RoleID: hdnRoleID.val(), UserType: $("#drpUserType").val() }, function () {
                hdnCurrentSelectedRoom = SelectedRoomID;
                SetSelectedModule_NonModulePermissions();
            });
            EnableDisableRoomsControls();
        },
        error: function (response) {
            // through errror message
        }
    });

}
function onSuccess(response) {
    IsRefreshGrid = true;
    //$('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    showNotificationDialog();
    $("#spanGlobalMessage").html(response.Message);
    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    var idValue = $("#hiddenID").val();

    if (response.Status == "fail") {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        clearControls('frmRole');

        $('input:checkbox').removeAttr('checked');
        $("#txtDescription").val("");

        $("#txtRoleName").val("");
        $("#txtRoleName").focus();
    }
    else if (idValue == 0) {
        clearControls('frmRole');
        $("#txtRoleName").val("");
        $("#txtRoleName").focus();
        if (response.Status == "duplicate")
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
        else {
            clearControls('frmRole');
            ClearAllRolesDropdown();
        }
    }
    else if (idValue > 0) {

        if (response.Status == "duplicate") {
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
            $("#txtRoleName").val("");
            $("#txtRoleName").focus();
        }
        else {
            clearControls('frmRole');
            ClearAllRolesDropdown();
            SwitchTextTab(0, 'RoleCreate', 'frmRole');
        }
    }
}

function onFailure(message) {
    $("#spanGlobalMessage").html(message.statusText);
    //$('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    showNotificationDialog();
    $("#txtRoleName").focus();
}
function SetSelectedModule_NonModulePermissions() {
    hdnSelectedModuleList.val('');
    hdnSelectedNonModuleList.val('');
    hdnSelectedDefaultSettings.val('');

    SelectedNonModuleList = [];
    SelectedModuleList = [];
    SelectedDefaultSettings = [];

    $("#CreateRolePermissionDIV input:checkbox").each(function () {
        var ControlID = this.id.toLowerCase();
        if (ValidateCheckBox(ControlID) == true) {
            if (this.checked == true) {
                if (ControlID.toLowerCase().indexOf("_ischecked") != -1) {
                    SelectedNonModuleList.push(ControlID);
                }
                else {
                    SelectedModuleList.push(ControlID);
                }
            }
        }
    });

    $("#CreateRolePermissionDIV input:text").each(function () {
        if (this.value != '') {
            var txtcnt = this.id + '#' + this.value;
            SelectedDefaultSettings.push(txtcnt);
        }
    });

    if (SelectedModuleList.toString() != '') {
        hdnSelectedModuleList.val(SelectedModuleList.toString());
    }
    if (SelectedNonModuleList.toString() != '') {
        $("#hdnSelectedNonModuleList").val(SelectedNonModuleList.toString());
    }
    if (SelectedDefaultSettings.toString() != '') {
        $("#hdnSelectedDefaultSettings").val(SelectedDefaultSettings.toString());
    }
}

function ClearRoomAccessHidden() {
    hdnSelectedModuleList.val('');
    hdnSelectedNonModuleList.val('');
    SelectedModuleList = [];
    SelectedNonModuleList = [];
}

function CheckEnterpriseClick(chkDropdown, event) {

    var arrItems = new Array();
    //if (event.type == 'multiselectcheckall') {
    $("#EnterpriseData > option").each(function () {
        if (this.selected == true)
            arrItems.push(this.value);
    });

    if (chkDropdown.checked == true) {
        arrItems.push(chkDropdown.value);
    }
    else {
        var selected = chkDropdown.value;
        arrItems = jQuery.grep(arrItems, function (value) {
            return value != selected;
        });
    }
    if (chkDropdown.selected == true) {
        arrItems.push(chkDropdown.value);
    }
    SelectedEnterprises = arrItems;

    $("#hdnSelectedEnterpriseAccessValue").val(SelectedEnterprises.join(","));

    LoadCompanies(arrItems);
}

function CheckCompanyClick(chkDropdown, event) {

    var arrItems = new Array();
    //if (event.type == 'multiselectcheckall') {
    $("#CompanyData > option").each(function () {
        if (this.selected == true)
            arrItems.push(this.value);
    });

    if (chkDropdown.checked == true) {
        arrItems.push(chkDropdown.value);
    }
    else {
        var selected = chkDropdown.value;
        arrItems = jQuery.grep(arrItems, function (value) {
            return value != selected;
        });
    }
    if (chkDropdown.selected == true) {
        arrItems.push(chkDropdown.value);
    }
    SelectedCompanies = arrItems;
    $("#hdnSelectedCompanyAccessValue").val(SelectedCompanies.join(","));
    LoadRooms(arrItems);
    //        LoadReplenishRooms(arrItems);
}

function Checkclick(chkDropdown, event) {

    var found = false;
    var ElementID;

    if (event.type == 'multiselectcheckall' || event.type == 'multiselectuncheckall') {
        $("#ddlDefaultPermissionRooms > option").remove();
        if (event.type == 'multiselectcheckall') {
            $("#ddlSelectedRooms > option").remove();
            $("#RoomData > option").each(function () {
                var ss = '<option value="' + this.value + '">' + this.text + '</option>';
                $("#ddlSelectedRooms").append(ss);
            });
        }
        else if (event.type == 'multiselectuncheckall') {
            $("#ddlSelectedRooms > option").remove();
        }
    } else {

        $("#ddlSelectedRooms > option").each(function () {
            if (this.value == chkDropdown.value) {
                found = true;
                ElementID = chkDropdown.value;
                return;
            }
        });

        if (chkDropdown.checked == true) {
            if (found == false) {
                var ss = '<option value="' + chkDropdown.value + '">' + chkDropdown.text + '</option>';
                $("#ddlSelectedRooms").append(ss);
            }
        }
        else {
            $('#ddlSelectedRooms option[value="' + ElementID + '"]').remove();
        }
    }
    var arrnewrooms = $("#RoomData").multiselect("getChecked").map(function () { return this.value + "_" + this.title; }).get();
    $("#hdnSelectedRoomAccessValue").val(arrnewrooms.join(","));

    SelectedRooms = [];
    $("#ddlSelectedRooms > option").each(function () {
        SelectedRooms.push(this.value);
    });

    AddRemoveRoomsToSession(SelectedRooms);

}

function EditModeSetData() {
    $("#ddlSelectedRooms").html("");
    if (hdnSelectedRoomAccessValue.val() != '') {
        var FirstRoomID = 0;
        var Rn = hdnSelectedRoomAccessValue.val().split(",");
        for (var i = 0; i < Rn.length; i++) {
            var Rnames = Rn[i].split("_");
            if (i == 0) {
                FirstRoomID = Rnames[0] + '_' + Rnames[1] + '_' + Rnames[2];
            }

            var optionname = "";
            var thirducindx = nth_occurrence(Rn[i], '_', 3);
            if (thirducindx != -1) {
                optionname = Rn[i].substring((thirducindx + 1), Rn[i].length);
            }
            var ss = '<option value="' + Rnames[0] + '_' + Rnames[1] + '_' + Rnames[2] + '">' + optionname + '</option>';
            $("#ddlSelectedRooms").append(ss);

            //            $("#RoomData").multiselect("widget").find(":checkbox[value='" + Rnames[0] + '_' + Rnames[1] + '_' + Rnames[2] + "']").attr("checked", "checked");
            //            $("#RoomData option[value='" + Rnames[0] + '_' + Rnames[1] + '_' + Rnames[2] + "']").attr("selected", 1);
            //            $("#RoomData").multiselect("refresh");
        }
    }
    SelectedRooms = [];
    $("#ddlSelectedRooms > option").each(function () {
        SelectedRooms.push(this.value);
    });

    AddRemoveRoomsToSession(SelectedRooms);
    //        EnableDisableRoomsControls();
}

function AddRemoveRoomsToSession(SelectedRooms) {
    var SelectedRoomID = 0
    $.ajax({
        type: "POST",
        url: url_AddRemoveRoomsToSession,
        data: "{'RoomIDs': '" + SelectedRooms.toString() + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (message) {
            if (SelectedRooms.toString() != '') {
                SelectedRoomID = SelectedRooms[0];
            }
            $("#CreateRolePermissionDIV").load("/Master/RolePermissionCreate", { RoomID: SelectedRoomID, RoleID: hdnRoleID.val(), UserType: $("#drpUserType").val() }, function () {
                hdnCurrentSelectedRoom = SelectedRoomID;
                SetSelectedModule_NonModulePermissions();
            });
            SetDefaultPermissionRooms();
            EnableDisableRoomsControls();
        },
        error: function (response) {
            // through errror message
        }
    });
}
function EditModeSetRoomReplanishmentData() {

    if ($("#hdnSelectedRoomReplanishmentValue").val() != '') {

        var Rnreplenish = $("#hdnSelectedRoomReplanishmentValue").val().split(",");
        for (var i = 0; i < Rnreplenish.length; i++) {
            $("#RoomReplanishment").multiselect("widget").find(":checkbox[value='" + Rnreplenish[i] + "']").attr("checked", "checked");
            $("#RoomReplanishment option[value='" + Rnreplenish[i] + "']").attr("selected", 1);
            $("#RoomReplanishment").multiselect("refresh");
        }
    }
}


function nth_occurrence(string, char, nth) {
    var first_index = string.indexOf(char);
    var length_up_to_first_index = first_index + 1;

    if (nth == 1) {
        return first_index;
    } else {
        var string_after_first_occurrence = string.slice(length_up_to_first_index);
        var next_occurrence = nth_occurrence(string_after_first_occurrence, char, nth - 1);

        if (next_occurrence === -1) {
            return -1;
        } else {
            return length_up_to_first_index + next_occurrence;
        }
    }
}

function SetDefaultPermissionRooms() {
    $("#ddlDefaultPermissionRooms > option").remove();

    $("#ddlSelectedRooms > option").each(function () {
        if (this.selected == false) {
            var ss = '<option value="' + this.value + '">' + this.text + '</option>';
            ddlDefaultPermissionRooms.append(ss);
        }
    });
    ddlDefaultPermissionRooms.multiselect("refresh");
}

function CopyPermission() {
    var SelectedRoomID = 0; // $("#ddlSelectedRooms").val();
    $("#ddlSelectedRooms > option").each(function () {
        if (this.selected == true) {
            SelectedRoomID = this.value;
        }
    });

    var CopyToRoomIDs = '';
    $("#ddlDefaultPermissionRooms > option").each(function () {
        if (this.selected == true) {
            if (CopyToRoomIDs == '')
                CopyToRoomIDs = this.value;
            else
                CopyToRoomIDs += ',' + this.value;
        }
    });

    SetSelectedModule_NonModulePermissions();

    if (CopyToRoomIDs != '') {
        $.ajax({
            type: "POST",
            url: url_SaveToRolePermissionsToSession,
            data: "{'RoomID': '" + hdnCurrentSelectedRoom + "' ,'RoleID':'" + hdnRoleID.val() + "','SelectedModuleList':'" + hdnSelectedModuleList.val() + "','SelectedNonModuleList':'" + hdnSelectedNonModuleList.val() + "','SelectedDefaultSettings':'" + hdnSelectedDefaultSettings.val() + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (message) {
                $.ajax({
                    type: "POST",
                    url: url_CopyPermissionsToRooms,
                    data: "{'ParentRoomID': '" + SelectedRoomID + "' ,'CopyToRoomIDs':'" + CopyToRoomIDs + "','RoleID':'" + hdnRoleID.val() + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (message) {
                        //$('div#target').fadeToggle();
                        //$("div#target").delay(2000).fadeOut(200);
                        showNotificationDialog();
                        $("#spanGlobalMessage").html(MsgPermissionCopied);
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('succesIcon');
                        return false;

                    },
                    error: function (response) {
                        // through errror message
                    }
                });
            },
            error: function (response) {
                // through errror message
            }
        });
        //    ClearRoomAccessHidden();


    }
    else {
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgRoomToCopyPermission);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('WarningIcon');
        return false;
    }
}

function RoomValidation() {
    var RoomAvailable = false;
    $("#ddlSelectedRooms > option").each(function () {
        if (this.selected == true) {
            RoomAvailable = true;
        }
    });
    return RoomAvailable;
}

function EnableDisableRoomsControls() {
    var SelectedRoomscnt = $("#ddlSelectedRooms > option").length;

    if (SelectedRoomscnt == 0) {
        $('#CreateRolePermissionDIV :input').attr('disabled', true);
    }
    else {
        $('#CreateRolePermissionDIV :input').removeAttr('disabled');
    }
}

function ValidateCheckBox(Chked) {
    var result = true;
    if (Chked.toLowerCase().indexOf("rowall") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allview") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allinsert") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("alldelete") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allupdate") != -1) {
        result = false;
    }
    return result;

}

function ClearAllRolesDropdown() {

    $("#ddlDefaultPermissionRooms > option").remove();
    $("#ddlDefaultPermissionRooms").multiselect("refresh");

    $("#RoomData > option").remove();
    $("#RoomData").multiselect("refresh");

    $("#ddlSelectedRooms > option").remove();
    $("#ddlSelectedRooms").multiselect("refresh");

    $("#RoomReplanishment").multiselect("uncheckAll");
    $("#RoomReplanishment").multiselect("refresh");

    $("#CreateRolePermissionDIV input:checkbox").each(function () {
        this.checked = false;
    });

    $("#CreateRolePermissionDIV input:text").each(function () {
        this.value = '';
    });

}

$("#drpUserType").change(function () {
    SetAccessLevels($(this).val());
});

function SetAccessLevels(userType) {

    if (userType == "1") {
        $("#liEnterprises").css("display", "");
        $("#liCompanies").css("display", "");
        $("tr[id='module_41']").show();
        LoadEnterprises();

    }
    if (userType == "2") {
        $("#liEnterprises").css("display", "none");
        $("#liCompanies").css("display", "");
        $("tr[id='module_41']").hide();
        var arrnewEnterprise = new Array();
        arrnewEnterprise.push(RoleEnterpriseID);
        LoadCompanies(arrnewEnterprise);

    }
    if (userType == "3") {

        $("#liEnterprises").css("display", "none");
        $("#liCompanies").css("display", "none");
        $("tr[id='module_41']").hide();
        var arrnewCompany = new Array();
        arrnewCompany.push(ep_compid);
        LoadRooms(arrnewCompany);
        //            LoadReplenishRooms(arrnewCompany);
    }
}

function LoadEnterprises() {

    $.ajax({
        url: url_GetEnterpriseData,
        type: 'POST',
        data: {},
        success: function (response) {

            $("#EnterpriseData > option").remove();
            $("#EnterpriseData").multiselect("refresh");
            var s = '';
            $.each(response, function (i, val) {
                s += '<option value="' + val.ID + '"  >' + val.Name + '</option>';
            });
            $("#EnterpriseData").append(s);
            $("#EnterpriseData").multiselect("refresh");
            //                $("#EnterpriseData").multiselect({
            //                    noneSelectedText: 'Enterprise Access', selectedList: 5,
            //                    selectedText: function (numChecked, numTotal, checkedItems) {
            //                        return 'Enterprise Access';
            //                    }
            //                }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            //                    CheckEnterpriseClick(ui, event);
            //                });
            var arrnewEnterprise = new Array();
            if ($("#hdnSelectedEnterpriseAccessValue").val() != '') {
                var eids = $("#hdnSelectedEnterpriseAccessValue").val().split(",");
                for (var e = 0; e < eids.length; e++) {
                    $("#EnterpriseData").multiselect("widget").find(":checkbox[value='" + eids[e] + "']").attr("checked", "checked");
                    $("#EnterpriseData option[value='" + eids[e] + "']").attr("selected", 1);
                    $("#EnterpriseData").multiselect("refresh");
                    arrnewEnterprise.push(eids[e]);
                }
            }
            LoadCompanies(arrnewEnterprise);
        },
        error: function (response) {
            // through errror message
        }
    });

}

function LoadCompanies(enterpriseids) {

    var styrdata = JSON.stringify(enterpriseids);
    $.ajax({
        url: url_GetCompanyData,
        type: 'POST',
        data: { EnterpriseIds: styrdata, RoleType: $("#drpUserType").val() },
        success: function (response) {

            $("#CompanyData > option").remove();
            $("#CompanyData").multiselect("refresh");
            var s = '';
            if ($("#drpUserType").val() == 1) {
                $.each(response, function (i, val) {
                    s += '<option value="' + val.EnterPriseId + '_' + val.ID + '"  >' + val.Name + '(' + val.EnterPriseName + ')</option>';
                });
            }
            else {
                $.each(response, function (i, val) {
                    s += '<option value="' + val.EnterPriseId + '_' + val.ID + '"  >' + val.Name + '</option>';
                });
            }

            $("#CompanyData").append(s);
            //                $("#CompanyData").multiselect(
            //                        { noneSelectedText: 'Company Access', selectedList: 5,
            //                            selectedText: function (numChecked, numTotal, checkedItems) {
            //                                return 'Company Access';
            //                            }
            //                        }

            //            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            //                CheckCompanyClick(ui, event);
            //            });
            $("#CompanyData").multiselect("refresh");

            if ($("#hdnSelectedCompanyAccessValue").val() != '') {
                var eids = $("#hdnSelectedCompanyAccessValue").val().split(",");
                for (var e = 0; e < eids.length; e++) {
                    $("#CompanyData").multiselect("widget").find(":checkbox[value='" + eids[e] + "']").attr("checked", "checked");
                    $("#CompanyData option[value='" + eids[e] + "']").attr("selected", 1);
                    $("#CompanyData").multiselect("refresh");
                }
            }
            var arrnewCompany = $("#CompanyData").multiselect("getChecked").map(function () { return this.value; }).get();
            $("#hdnSelectedCompanyAccessValue").val(arrnewCompany.join(","));
            LoadRooms(arrnewCompany);
            //                LoadReplenishRooms(arrnewCompany);
        },
        error: function (response) {
            // through errror message
        }
    });
}

function LoadRooms(companyids) {

    var styrdata = JSON.stringify(companyids);
    $.ajax({
        url: url_GetRoomData,
        type: 'POST',
        data: { companyids: styrdata },
        success: function (response) {

            $("#RoomData > option").remove();
            $("#RoomData").multiselect("refresh");
            $("#RoomReplanishment > option").remove();
            $("#RoomReplanishment").multiselect("refresh");
            var s = '';
            if ($("#drpUserType").val() == 1 || $("#drpUserType").val() == 2) {
                $.each(response.DDData, function (i, val) {
                    s += '<option value="' + val.EnterpriseId + '_' + val.CompanyID + '_' + val.ID + '"  >' + val.RoomName + '(' + val.CompanyName + ')</option>';
                });
            }
            else {
                $.each(response.DDData, function (i, val) {
                    s += '<option value="' + val.EnterpriseId + '_' + val.CompanyID + '_' + val.ID + '"  >' + val.RoomName + '</option>';
                });
            }

            $("#RoomData").append(s);
            $("#RoomData").multiselect("refresh");
            $("#RoomReplanishment").append(s);
            $("#RoomReplanishment").multiselect("refresh");

            if ($("#hdnSelectedRoomAccessValue").val() != '') {
                var eids = $("#hdnSelectedRoomAccessValue").val().split(",");
                for (var e = 0; e < eids.length; e++) {
                    var optionname = "";
                    var thirducindx = nth_occurrence(eids[e], '_', 3);
                    if (thirducindx != -1) {
                        optionname = eids[e].substring(0, thirducindx);
                    }
                    $("#RoomData").multiselect("widget").find(":checkbox[value='" + optionname + "']").attr("checked", "checked");
                    $("#RoomData option[value='" + optionname + "']").attr("selected", 1);
                    $("#RoomData").multiselect("refresh");
                }
            }
            var arrnewrooms = $("#RoomData").multiselect("getChecked").map(function () { return this.value + "_" + this.title; }).get();
            $("#hdnSelectedRoomAccessValue").val(arrnewrooms.join(","));

            if ($("#hdnSelectedRoomReplanishmentValue").val() != '') {
                var eids = $("#hdnSelectedRoomReplanishmentValue").val().split(",");
                for (var e = 0; e < eids.length; e++) {
                    $("#RoomReplanishment").multiselect("widget").find(":checkbox[value='" + eids[e] + "']").attr("checked", "checked");
                    $("#RoomReplanishment option[value='" + eids[e] + "']").attr("selected", 1);
                    $("#RoomReplanishment").multiselect("refresh");
                }
            }
            var arrreplerooms = $("#RoomReplanishment").multiselect("getChecked").map(function () { return this.value; }).get();
            $("#hdnSelectedRoomReplanishmentValue").val(arrreplerooms.join(","));

            EditModeSetData();
        },
        error: function (response) {
            // through errror message
        }
    });
}
function LoadReplenishRooms(companyids) {
    var styrdata = JSON.stringify(companyids);
    $.ajax({
        url: url_GetRoomData,
        type: 'POST',
        data: { companyids: styrdata },
        success: function (response) {
            $("#RoomReplanishment > option").remove();
            $("#RoomReplanishment").multiselect("refresh");
            var s = '';
            if ($("#drpUserType").val() == 1 || $("#drpUserType").val() == 2) {
                $.each(response.DDData, function (i, val) {
                    s += '<option value="' + val.EnterpriseId + '_' + val.CompanyID + '_' + val.ID + '"  >' + val.RoomName + '(' + val.CompanyName + ')</option>';
                });
            }
            else {
                $.each(response.DDData, function (i, val) {
                    s += '<option value="' + val.EnterpriseId + '_' + val.CompanyID + '_' + val.ID + '"  >' + val.RoomName + '</option>';
                });
            }

            $("#RoomReplanishment").append(s);
            $("#RoomReplanishment").multiselect("refresh");
        },
        error: function (response) {
            // through errror message
        }
    });
}

function LoadEnterpriseSelectBox() {
    $("#EnterpriseData").multiselect(
                        {
                            noneSelectedText: 'Enterprise Access', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Enterprise Access';
                            }
                        }

            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                CheckEnterpriseClick(ui, event);
            });
}

function LoadComanySelectBox() {

    $("#CompanyData").multiselect(
                        {
                            noneSelectedText: 'Company Access', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Company Access';
                            }
                        }

            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                CheckCompanyClick(ui, event);
            });
}

function LoadRoomSelectBox() {
    $("#RoomData").multiselect(
                        {
                            noneSelectedText: 'Room Access', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Room Access';
                            }
                        }

            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                Checkclick(ui, event);
            });
}
function LoadReplenishRoomSelectBox() {
    $("#RoomReplanishment").multiselect(
                        {
                            noneSelectedText: 'Room Replenishment', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Room Access';
                            }
                        }

            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                //                Checkclick(ui, event);
            });
}

function numberOnly() {
    if ((event.keyCode < 48) || (event.keyCode > 57))
        return false;
}

function SelectAllTabChk(Chked, TabNO) {
    if (TabNO == '1') {
        $("#Roomtab2 input:checkbox").each(function () {
            this.checked = Chked.checked;
        });
    }
    else if (TabNO == '2') {
        $("#Roomtab3 input:checkbox").each(function () {
            this.checked = Chked.checked;
        });
    }
    else if (TabNO == '3') {
        $("#Roomtab4 input:checkbox").each(function () {
            var vModuleID = $(this).prop("id").split('_')[0]
            if ((vModuleID != "116" && vModuleID != "118" && vModuleID != "121") || Chked.checked == false)
                this.checked = Chked.checked;
            else
                this.checked = false;
        });
    }
}

function SelectRowColumnChk(Chked, TabNO) {

    if (Chked.id.toLowerCase().indexOf("rowall") != -1) {
        var chkModuleID = Chked.id.split("_");
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox");
        }
        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID[0] == chkModuleID[0]) {
                this.checked = Chked.checked;
            }
        });
    }

    // ----- Select All View permission --------
    if (Chked.id.toLowerCase().indexOf("allview") != -1) {
        CheckViewChkBox(Chked, TabNO);
        //            var Tabs = '';
        //            if (TabNO == '1') {
        //                var Tabs = $("#Roomtab2 input:checkbox");
        //            }
        //            else if (TabNO == '2') {
        //                var Tabs = $("#Roomtab3 input:checkbox");
        //            }

        //            Tabs.each(function () {
        //                var currentModuleID = this.id.split("_");
        //                if (currentModuleID.length > 1) {
        //                    if (currentModuleID[1].toLowerCase() == 'view') {
        //                        this.checked = Chked.checked;
        //                    }
        //                }
        //            });
    }

    // ----- Select All Insert permission --------
    if (Chked.id.toLowerCase().indexOf("allinsert") != -1) {
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox");
        }

        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID.length > 1) {
                if (currentModuleID[1].toLowerCase() == 'insert') {
                    this.checked = Chked.checked;
                }
            }
        });
    }

    // ----- Select All Delete permission --------
    if (Chked.id.toLowerCase().indexOf("alldelete") != -1) {
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox");
        }

        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID.length > 1) {
                if (currentModuleID[1].toLowerCase() == 'delete') {
                    this.checked = Chked.checked;
                }
            }
        });
    }
    // ----- Select All Update permission --------

    if (Chked.id.toLowerCase().indexOf("allupdate") != -1) {
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox");
        }

        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID.length > 1) {
                if (currentModuleID[1].toLowerCase() == 'update') {
                    this.checked = Chked.checked;
                }
            }
        });
    }

    // ----- Select All Show Deleted,Archived and UDF permission --------
    SelectRowColumnChkAll(Chked, TabNO, "allshowdeleted", "showdeleted");
    SelectRowColumnChkAll(Chked, TabNO, "allshowarchived", "showarchived");
    SelectRowColumnChkAll(Chked, TabNO, "allshowudf", "showudf");

    SelectRowColumnChkAll(Chked, TabNO, "allshowchangelog", "showchangelog");
    // ----- Select All Show Deleted,Archived and UDF permission --------

}

function SelectRowColumnChkAll(Chked, TabNO, Allchkname, chkname) {
    if (Chked.id.toLowerCase().indexOf(Allchkname) != -1) {
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox");
        }

        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID.length > 1) {
                if (currentModuleID[1].toLowerCase() == chkname) {
                    this.checked = Chked.checked;
                }
            }
        });
    }
}



function CheckViewChkBox(Chked, TabNO) {
    var Tabs = '';
    if (TabNO == '1') {
        var Tabs = $("#Roomtab2 input:checkbox");
    }
    else if (TabNO == '2') {
        var Tabs = $("#Roomtab3 input:checkbox");
    }

    Tabs.each(function () {
        var currentModuleID = this.id.split("_");
        if (currentModuleID.length > 1) {
            if (currentModuleID[1].toLowerCase() == 'view') {
                this.checked = Chked.checked;
            }
        }
    });
}

function SelectViewPermission(Chked, TabNO) {
    var SelectedModuleID = Chked.id.split("_");

    var hasView = false;
    var hasDelete = false;
    var hasUpdate = false;
    var hasInsert = false;

    var hasShowDeleted = false;
    var hasShowArchived = false;
    var hasShowUDF = false;
    var hadShowChangeLog = false;
    var Viewbtn = null;

    var Tabs = '';
    if (TabNO == '1') {
        var Tabs = $("#Roomtab2 input:checkbox");
    }
    else if (TabNO == '2') {
        var Tabs = $("#Roomtab3 input:checkbox");
    }

    Tabs.each(function () {
        var currentModuleID = this.id.split("_");
        if (currentModuleID.length > 1) {
            if (currentModuleID[0] == SelectedModuleID[0]) {

                if (Chked.checked == true && currentModuleID[1].toLowerCase() == 'view') {
                    Viewbtn = this;
                    this.checked = true;
                    this.disabled = true;
                }
                else {

                    if (currentModuleID[1].toLowerCase() == 'view') {
                        Viewbtn = this;
                    }

                    if (currentModuleID[1].toLowerCase() == 'delete') {
                        if (this.checked == true) {
                            hasDelete = true;
                        }
                    }

                    if (currentModuleID[1].toLowerCase() == 'update') {
                        if (this.checked == true) {
                            hasUpdate = true;
                        }
                    }

                    if (currentModuleID[1].toLowerCase() == 'insert') {
                        if (this.checked == true) {
                            hasInsert = true;
                        }
                    }

                    if (currentModuleID[1].toLowerCase() == 'showdeleted') {
                        if (this.checked == true) {
                            hasShowDeleted = true;
                        }
                    }
                    if (currentModuleID[1].toLowerCase() == 'showarchived') {
                        if (this.checked == true) {
                            hasShowArchived = true;
                        }
                    }
                    if (currentModuleID[1].toLowerCase() == 'showudf') {
                        if (this.checked == true) {
                            hasShowUDF = true;
                        }
                    }
                    if (currentModuleID[1].toLowerCase() == 'showchangelog') {
                        if (this.checked == true) {
                            hadShowChangeLog = true;
                        }
                    }
                }
            }
        }
    });

    if (hasDelete == true || hasUpdate == true || hasInsert == true || hasShowDeleted == true || hasShowArchived == true || hasShowUDF == true || hadShowChangeLog == true) {
        Viewbtn.disabled = true;
    }
    else {
        Viewbtn.disabled = false;
        Viewbtn.checked = false;
    }
}