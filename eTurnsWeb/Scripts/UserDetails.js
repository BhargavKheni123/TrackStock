var hdnSelectedRoomReplanishmentValue = $("#hdnSelectedRoomReplanishmentValue");
var hdnSelectedRoomAccessValue = $("#hdnSelectedRoomAccessValue");
var hdnRoleID = $("#hiddenID")

var SelectedModuleList = new Array();
var SelectedNonModuleList = new Array();
var SelectedDefaultSettings = new Array();
var SelectedRooms = new Array();

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
        $("#spanGlobalMessage").text('Room Access is required.');
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
                SelectedRoomReplanishment += sep1 + this.value;
        }
    });
    $("#hdnSelectedRoomReplanishmentValue").val('');
    $("#hdnSelectedRoomReplanishmentValue").val(SelectedRoomReplanishment);

    SetSelectedModule_NonModulePermissions();
    if ($.trim(hdnSelectedModuleList.val()) != "" || $.trim(hdnSelectedNonModuleList.val()) != "" || $.trim(hdnSelectedDefaultSettings.val()) != "") {


        $.ajax({
            type: "POST",
            url: url_SaveToUserPermissionsToSession,
            data: "{'RoomID': '" + hdnCurrentSelectedRoom + "' ,'RoleID':'" + hdnRoleID.val() + "','SelectedModuleList':'" + hdnSelectedModuleList.val() + "','SelectedNonModuleList':'" + hdnSelectedNonModuleList.val() + "','SelectedDefaultSettings':'" + hdnSelectedDefaultSettings.val() + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (message) {
            },
            error: function (response) {
                // through errror message
            }
        });

    }
    else {
        alert("Room can not be assigned without permissions");
        return false;
    }
    //------------

    $.validator.unobtrusive.parse("#frmUser");
    if ($(this).valid()) {
    }
    e.preventDefault();
});


function onSuccess(response) {
    IsRefreshGrid = true;
    //$('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    showNotificationDialog();
    $("#spanGlobalMessage").text(response.Message);
    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    var idValue = $("#hiddenID").val();

    if (response.Status == "fail") {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        clearControls('frmUser');
        ClearAllRolesDropdown();

        $('input:checkbox').removeAttr('checked');
        // $("#txtDescription").val("");

        $("#txtUserName").val("");
        $("#txtUserName").focus();
    }
    else if (idValue == 0) {
        //clearControls('frmRole');
        $("#txtUserName").val("");
        $("#txtUserName").focus();
        if (response.Status == "duplicate")
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
        else {
            clearControls('frmUser');
            ClearAllRolesDropdown();
        }
    }
    else if (idValue > 0) {

        if (response.Status == "duplicate") {
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
            $("#txtUserName").val("");
            $("#txtUserName").focus();
        }
        else {
            clearControls('frmUser');
            ClearAllRolesDropdown();
            SwitchTextTab(0, 'UserCreate', 'frmUser');
        }
    }
}

function Login_OnBegin() {
    var passrej = new RegExp("((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})");
    var currentPass = $("#Password").val();
    if (currentPass.match(passrej)) {
       
        return true;
    }
    else {
        $('div#target').fadeToggle();
        $("div#target").delay(2000).fadeOut(200);
        $("#spanGlobalMessage").text('@ResUserMaster.errPasswordRuleBreak');
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('succesIcon');
        return false;
    }
}
function onFailure(message) {
    $("#spanGlobalMessage").text(message.statusText);
    //$('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    showNotificationDialog();
    $("#txtUserName").focus();
}

$(document).ready(function () {

    SetAccessLevels($("#drpUserType").val());
    if ($("#hdnDisableControl").val() != '') {
        if ($("#hdnDisableControl").val().toLowerCase() == "true") {
            $(':input', '#frmUser')
                            .not('#btnCancel')
                            .attr('disabled', 'disabled');
        }


    }
    if ($("#hdnSelectedModuleList").val() != '') {
        SelectedModuleList = $("#hdnSelectedModuleList").val().split(',');
    }

    if ($("#hdnSelectedNonModuleList").val() != '') {
        SelectedNonModuleList = $("#hdnSelectedNonModuleList").val().split(',');
    }
    if ($("#hdnSelectedDefaultSettings").val() != '') {
        SelectedDefaultSettings = $("#hdnSelectedDefaultSettings").val().split(',');
    }

    $("#drpUserType").change(function () {
        SetAccessLevels($(this).val());
    });
    $('#btnCancel').click(function (e) {
        //            if (IsRefreshGrid)
        //                $('#NarroSearchClear').click();
        SwitchTextTab(0, 'UserCreate', 'frmUser');
        if (oTable !== undefined && oTable != null) {
            oTable.fnDraw();
        }
    });


    $('#ddlRole').change(function (e) {
        RoleSelection();
    });


    $('#ddlSelectedRooms').change(function (e) {
        RoomChanged();
    });

    $("#ddlDefaultPermissionRooms").multiselect(
                                {
                                    noneSelectedText: 'Room  ', selectedList: 5,
                                    selectedText: function (numChecked, numTotal, checkedItems) { return 'Room : ' + numChecked + ' selected'; }
                                }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                                    $.map($(this).multiselect("getChecked"), function (input) { return input.value; })
                                });


    EnableDisableRoomsControls();

});

$(function () {
    $("#tabs").tabs();
});
//-----------------------------------------------
function GetSelectedRoleID() {
    var SelectedRoleID = 0; // $("#ddlSelectedRooms").val();
    $("#ddlRole > option").each(function () {
        if (this.selected == true) {
            SelectedRoleID = this.value;
        }
    });
    $("#hdnRoleID").val(SelectedRoleID);
    return SelectedRoleID;
}

function GetSelectedRoomAccessID() {
    var SelectedRoomID = 0; // $("#ddlSelectedRooms").val();
    
    $("#ddlSelectedRooms > option").each(function () {
        if (this.selected == true) {
            SelectedRoomID = this.value;
        }
    });
    return SelectedRoomID;
}
function GetUserID() {
    var UserID = 0;
    if ($("#hiddenID").val() != '') {
        UserID = $("#hiddenID").val();
    }
    return UserID;
}


function ClearAllRolesDropdown() {

    $("#ddlDefaultPermissionRooms > option").remove();
    $("#ddlDefaultPermissionRooms").multiselect("refresh");


    $("#EnterpriseData > option").remove();
    $("#EnterpriseData").multiselect("refresh");

    $("#CompanyData > option").remove();
    $("#CompanyData").multiselect("refresh");

    $("#RoomData > option").remove();
    $("#RoomData").multiselect("refresh");

    $("#RoomReplanishment > option").remove();
    $("#RoomReplanishment").multiselect("refresh");

    $("#ddlSelectedRooms > option").remove();
    $("#ddlSelectedRooms").multiselect("refresh");



    $("#CreateRolePermissionDIV input:checkbox").each(function () {
        this.checked = false;
    });

    $("#CreateRolePermissionDIV input:text").each(function () {
        this.value = '';
    });

}

function RoleSelection() {

    ClearAllRolesDropdown();
    var SelectedRoleID = GetSelectedRoleID();
    var UserType = $("#drpUserType").val();
    var SelecteUserID = GetUserID();
    if (SelectedRoleID > 0) {
        $.ajax({
            url: url_GetRoleDetailsInfo,
            type: 'POST',
            data: { RoleID: SelectedRoleID, UserType: UserType, UserId: SelecteUserID },
            success: function (response) {

                var s = '';
                var selectedEps = "";
                $.each(response.EnterPriseList, function (i, val) {
                    if (val.IsSelected == true) {
                        s += '<option selected="selected" value="' + val.EnterPriseId + '">' + val.EnterPriseName + '</option>';
                    }
                    else {
                        s += '<option value="' + val.EnterPriseId + '">' + val.EnterPriseName + '</option>';
                    }

                    if (selectedEps == "") {
                        selectedEps = val.EnterPriseId + "_" + val.EnterPriseName;
                    }
                    else {
                        selectedEps += sep1 + val.EnterPriseId + "_" + val.EnterPriseName;
                    }
                });
                $("#hdnSelectedEnterpriseAccessValue").val(selectedEps);
                $("#EnterpriseData").html(s);
                $("#EnterpriseData").multiselect("refresh");

                s = '';
                selectedEps = "";
                $.each(response.CompanyList, function (i, val) {
                    if (UserType == 1) {
                        if (val.IsSelected == true) {
                            s += '<option selected="selected" value="' + val.EnterPriseId_CompanyId + '">' + val.CompanyName + '(' + val.EnterPriseName + ')</option>';
                        }
                        else {
                            s += '<option value="' + val.EnterPriseId_CompanyId + '">' + val.CompanyName + '(' + val.EnterPriseName + ')</option>';
                        }
                    }
                    else {
                        if (val.IsSelected == true) {
                            s += '<option selected="selected" value="' + val.EnterPriseId_CompanyId + '">' + val.CompanyName + '</option>';
                        }
                        else {
                            s += '<option value="' + val.EnterPriseId_CompanyId + '">' + val.CompanyName + '</option>';
                        }
                    }
                    if (selectedEps == "") {
                        selectedEps = val.EnterPriseId_CompanyId + "_" + val.CompanyName;
                    }
                    else {
                        selectedEps += sep1 + val.EnterPriseId_CompanyId + "_" + val.CompanyName;
                    }
                });
                $("#hdnSelectedCompanyAccessValue").val(selectedEps);
                $("#CompanyData").html(s);
                $("#CompanyData").multiselect("refresh");

                selectedEps = '';
                s = '';
                $.each(response.RoomList, function (i, val) {
                    if (UserType == 1 || UserType == 2) {
                        if (val.IsSelected == true) {
                            s += '<option selected="selected" value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '(' + val.CompanyName + ')</option>';
                        }
                        else {
                            s += '<option value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '(' + val.CompanyName + ')</option>';
                        }
                    }
                    else {
                        if (val.IsSelected == true) {
                            s += '<option selected="selected" value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '</option>';
                        }
                        else {
                            s += '<option value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '</option>';
                        }
                    }
                    if (selectedEps == '') {
                        selectedEps = val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                    }
                    else {
                        selectedEps += sep1 + val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                    }
                });
                hdnSelectedRoomAccessValue.val(selectedEps);
                $("#RoomData").html(s);
                $("#RoomData").multiselect("refresh");

                selectedEps = '';
                s = '';

                if (response.ReplenishList != null) {
                    $.each(response.ReplenishList, function (i, val) {
                        if (UserType == 1 || UserType == 2) {
                            s += '<option value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '(' + val.CompanyName + ')</option>';
                        } else {
                            s += '<option value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '</option>';
                        }
                        if (selectedEps == '') {
                            selectedEps = val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                        }
                        else {
                            selectedEps += sep1 + val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                        }
                    });
                }

                $("#RoomReplanishment").html(s);
                $("#RoomReplanishment").multiselect("refresh");
                $("#hdnSelectedRoomReplanishmentValue").val(selectedEps);
                //                    EditModeSetRoomReplanishmentData();
                //New change

                //New change

                ResetRoomAccessSelection();

            },
            error: function (response) {
                // through errror message
            }
        });
        //            EditModeSetRoomReplanishmentData();
    }
}

function RoomChanged() {
    var SelectedRoomID = GetSelectedRoomAccessID();
    var SelectedRoleID = GetSelectedRoleID();

    SetDefaultPermissionRooms();

    SetSelectedModule_NonModulePermissions();

    $.ajax({
        type: "POST",
        url: url_SaveToUserPermissionsToSession,
        data: "{'RoomID': '" + hdnCurrentSelectedRoom + "' ,'RoleID':'" + SelectedRoleID + "','SelectedModuleList':'" + hdnSelectedModuleList.val() + "','SelectedNonModuleList':'" + hdnSelectedNonModuleList.val() + "','SelectedDefaultSettings':'" + hdnSelectedDefaultSettings.val() + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (message) {
        },
        error: function (response) {
            // through errror message
        }
    });
    ClearRoomAccessHidden();
    var SelecteUserID = GetUserID();
    //    $("#CreateRolePermissionDIV").load(url_UserRolePermissionCreate,
    //    { RoomID: SelectedRoomID, RoleID: SelectedRoleID, UserID: SelecteUserID, UserType: $("#drpUserType").val() }, function () {
    //        hdnCurrentSelectedRoom = SelectedRoomID;
    //        SetSelectedModule_NonModulePermissions();
    //    });
    LoadPermissionDiv(SelectedRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());
    EnableDisableRoomsControls();
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
            var txtcnt = "";
            if (this.id == "99") {
                txtcnt = this.id + '#' + this.value;
            }
            else {
                txtcnt = this.id + '#' + this.value;
            }
            SelectedDefaultSettings.push(txtcnt);
        }
    });
    $("#CreateRolePermissionDIV select").each(function (indx, slctobj) {
        if ($(this).val() != '') {
            var txtcnt = "";
            if ($(this).attr("id") == "99") {
                txtcnt = $(this).attr("id") + '#' + $(this).val();
                SelectedDefaultSettings.push(txtcnt);
            }
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
    $("#hdnSelectedEnterpriseAccessValue").val(SelectedEnterprises.join(sep1));

    if (event.type == 'multiselectcheckall' || event.type == 'multiselectuncheckall') {
        if (event.type == 'multiselectcheckall') {
            $("#CompanyData").multiselect("widget").find(":checkbox").each(function () {
                $(this).attr("checked", "checked");
            });
            $("#CompanyData option").each(function () {
                $(this).attr("selected", 1);
            });

            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {
                $(this).attr("checked", "checked");
            });
            $("#RoomData option").each(function () {
                $(this).attr("selected", 1);
            });

        }
        else {
            $("#CompanyData").multiselect("widget").find(":checkbox").each(function () {
                $(this).removeAttr("checked");
            });
            $("#CompanyData option").each(function () {
                $(this).removeAttr("selected");
            });

            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {
                $(this).removeAttr("checked");
            });
            $("#RoomData option").each(function () {
                $(this).removeAttr("selected");
            });
        }
    }
    else {
        var valuesToselect = chkDropdown.value;
        if (chkDropdown.checked) {
            $("#CompanyData").multiselect("widget").find(":checkbox").each(function () {
                var eid = $(this).attr("value").split("_")[0];
                if (eid == valuesToselect) {
                    $(this).attr("checked", "checked");
                }
            });
            $("#CompanyData option").each(function () {
                var eid = $(this).attr("value").split("_")[0];
                if (eid == valuesToselect) {
                    $(this).attr("selected", 1);
                }
            });
            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {
                var eid = $(this).attr("value").split("_")[0];
                if (eid == valuesToselect) {
                    $(this).attr("checked", "checked");
                }

            });
            $("#RoomData option").each(function () {
                var eid = $(this).attr("value").split("_")[0];
                if (eid == valuesToselect) {
                    $(this).attr("selected", 1);
                }
            });
        }
        else {
            $("#CompanyData").multiselect("widget").find(":checkbox").each(function () {
                var eid = $(this).attr("value").split("_")[0];
                if (eid == valuesToselect) {
                    $(this).removeAttr("checked");
                }
            });
            $("#CompanyData option").each(function () {
                var eid = $(this).attr("value").split("_")[0];
                if (eid == valuesToselect) {
                    $(this).removeAttr("selected");
                }
            });
            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {
                var eid = $(this).attr("value").split("_")[0];
                if (eid == valuesToselect) {
                    $(this).removeAttr("checked");
                }

            });
            $("#RoomData option").each(function () {
                var eid = $(this).attr("value").split("_")[0];
                if (eid == valuesToselect) {
                    $(this).removeAttr("selected");
                }
            });
        }
    }
    SetSelectBox();
}

function SetSelectBox() {
    $("#ddlSelectedRooms").html("");
    SelectedRooms = [];
    var checkedList = $("#RoomData").multiselect("getChecked");
    $(checkedList).each(function (indx, obj) {
        var ss = '<option value="' + obj.value + '">' + obj.title + '</option>';
        if (indx == 0) {
            ss = '<option value="' + obj.value + '" selected="selected">' + obj.title + '</option>'
        }
        $("#ddlSelectedRooms").append(ss);
        SelectedRooms.push(obj.value);
    });
    var SelectedRoomID = 0
    $.ajax({
        type: "POST",
        url: url_AddRemoveUserRoomsToSession,
        data: "{'RoomIDs': '" + SelectedRooms.join(sep1) + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (message) {
            if (SelectedRooms.toString() != '') {
                SelectedRoomID = SelectedRooms[0];
            }
            var SelectedRoleID = GetSelectedRoleID();
            var SelecteUserID = GetUserID();
            //    $("#CreateRolePermissionDIV").load(url_UserRolePermissionCreate,
            //    { RoomID: SelectedRoomID, RoleID: SelectedRoleID, UserID: SelecteUserID, UserType: $("#drpUserType").val() }, function () {
            //        hdnCurrentSelectedRoom = SelectedRoomID;
            //        SetSelectedModule_NonModulePermissions();
            //    });
            LoadPermissionDiv(SelectedRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());
            SetDefaultPermissionRooms();
            EnableDisableRoomsControls();
        },
        error: function (response) {
            // through errror message
        }
    });
    
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
    $("#hdnSelectedCompanyAccessValue").val(SelectedCompanies.join(sep1));

    if (event.type == 'multiselectcheckall' || event.type == 'multiselectuncheckall') {
        if (event.type == 'multiselectcheckall') {
            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {
                $(this).attr("checked", "checked");
            });
            $("#RoomData option").each(function () {
                $(this).attr("selected", 1);
            });
        }
        else {
            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {
                $(this).removeAttr("checked");
            });
            $("#RoomData option").each(function () {
                $(this).removeAttr("selected");
            });
        }
    }
    else {
        var valuesToselect = chkDropdown.value;
        if (chkDropdown.checked) {
            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {

                var cid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
                if (cid == valuesToselect) {
                    $(this).attr("checked", "checked");
                }

            });
            $("#RoomData option").each(function () {

                var cid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
                if (cid == valuesToselect) {
                    $(this).attr("selected", 1);
                }
            });
        }
        else {

            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {

                var cid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
                if (cid == valuesToselect) {
                    $(this).removeAttr("checked");
                }

            });
            $("#RoomData option").each(function () {

                var cid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
                if (cid == valuesToselect) {
                    $(this).removeAttr("selected");
                }
            });
        }
    }
    SetSelectBox();

}
function Checkclick(chkDropdown, event) {
    SetSelectBox();
}

function EditModeSetRoomReplanishmentData() {
    $("#RoomReplanishment > option").each(function () {
        this.selected = false;
    });
    $("#RoomReplanishment").multiselect("refresh");

    if ($("#hdnSelectedRoomReplanishmentValue").val() != '') {
        var Rnreplenish = $("#hdnSelectedRoomReplanishmentValue").val().split(sep1);
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
function ResetRoomAccessSelection() {
    var checkedList1 = $("#RoomData").multiselect("getChecked");

    $("#ddlSelectedRooms > option").remove();
    if (checkedList1.length > 0) {

        var SelectedRoleID = GetSelectedRoleID();
        var SelecteUserID = GetUserID();
        var FirstRoomID = 0;
        var Rn = hdnSelectedRoomAccessValue.val().split(sep1);
        for (var i = 0; i < checkedList1.length; i++) {
            //            checkedList1[0].title
            //            checkedList1[0].value
            //            var Rnames = Rn[i].split("_");
            if (i == 0) {
                FirstRoomID = checkedList1[i].value;
            }
            //            var optionname = "";
            //            var thirducindx = nth_occurrence(Rn[i], '_', 3);
            //            if (thirducindx != -1) {
            //                optionname = Rn[i].substring((thirducindx + 1), Rn[i].length);
            //            }
            var ss = '<option value="' + checkedList1[i].value + '">' + checkedList1[i].title + '</option>';
            $("#ddlSelectedRooms").append(ss);
            //            $("#RoomData").multiselect("widget").find(":checkbox[value='" + Rnames[0] + "_" + Rnames[1] + "_" + Rnames[2] + "']").attr("checked", "checked");
            //            $("#RoomData option[value='" + Rnames[0] + "_" + Rnames[1] + "_" + Rnames[2] + "']").attr("selected", 1);
            //            $("#RoomData").multiselect("refresh");
        }

        Rn = $("#hdnSelectedEnterpriseAccessValue").val().split(sep1);
        //        for (var i = 0; i < Rn.length; i++) {
        //            var Rnames = Rn[i].split("_");
        //            $("#EnterpriseData").multiselect("widget").find(":checkbox[value='" + Rnames[0] + "']").attr("checked", "checked");
        //            $("#EnterpriseData option[value='" + Rnames[0] + "']").attr("selected", 1);
        //            $("#EnterpriseData").multiselect("refresh");
        //        }
        Rn = $("#hdnSelectedCompanyAccessValue").val().split(sep1);
        //        for (var i = 0; i < Rn.length; i++) {
        //            var Rnames = Rn[i].split("_");
        //            $("#CompanyData").multiselect("widget").find(":checkbox[value='" + Rnames[0] + "_" + Rnames[1] + "']").attr("checked", "checked");
        //            $("#CompanyData option[value='" + Rnames[0] + "_" + Rnames[1] + "']").attr("selected", 1);
        //            $("#CompanyData").multiselect("refresh");
        //        }

        Rn = $("#hdnSelectedRoomReplanishmentValue").val().split(sep1);
        //        for (var i = 0; i < Rn.length; i++) {
        //            var Rnames = Rn[i].split("_");
        //            $("#RoomReplanishment").multiselect("widget").find(":checkbox[value='" + Rnames[0] + "_" + Rnames[1] + "_" + Rnames[2] + "']").attr("checked", "checked");
        //            $("#RoomReplanishment option[value='" + Rnames[0] + "_" + Rnames[1] + "_" + Rnames[2] + "']").attr("selected", 1);
        //            $("#RoomReplanishment").multiselect("refresh");
        //        }
        if (FirstRoomID != "") {
            $("#ddlSelectedRooms").multiselect("widget").find(":checkbox[value='" + FirstRoomID + "']").attr("checked", "checked");
            $("#ddlSelectedRooms option[value='" + FirstRoomID + "']").attr("selected", 1);
            $("#ddlSelectedRooms").multiselect("refresh");

            SetDefaultPermissionRooms();
            LoadPermissionDiv(FirstRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());

        }
    }
}
function LoadPermissionDiv(FirstRoomID, SelectedRoleID, SelecteUserID, UserType) {
    $("#CreateRolePermissionDIV").load(url_UserRolePermissionCreate, { RoomID: FirstRoomID, RoleID: SelectedRoleID, UserID: SelecteUserID, UserType: UserType }, function () {
        if (UserType == 1) {
            $("tr[id='41']").show();
            $("tr[id='39']").show();
        }
        else if (UserType == 2) {
            $("tr[id='41']").hide();
            $("tr[id='39']").show();
        }
        else {
            $("tr[id='41']").hide();
            $("tr[id='39']").hide();
        }
        hdnCurrentSelectedRoom = FirstRoomID;
        SetSelectedModule_NonModulePermissions();
        EnableDisableRoomsControls();
    });
}
function EditModeSetData() {

    if ($("#hdnRoleID").val() != '') {
        $("#ddlRole > option").each(function () {
            if (this.value == $("#hdnRoleID").val()) {
                this.selected = true;
            }
        });
    }
    //var SelectedRoleID = GetSelectedRoleID();
    RoleSelection();

    //        ResetRoomAccessSelection();
    //        EnableDisableRoomsControls();
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
    
    var SelectedRoomID = GetSelectedRoomAccessID();

    var CopyToRoomIDs = '';
    $("#ddlDefaultPermissionRooms > option").each(function () {
        if (this.selected == true) {
            if (CopyToRoomIDs == '')
                CopyToRoomIDs = this.value;
            else
                CopyToRoomIDs += sep1 + this.value;
        }
    });

    SetSelectedModule_NonModulePermissions();

    if (CopyToRoomIDs != '') {
        $.ajax({
            type: "POST",
            url: url_SaveToUserPermissionsToSession,
            data: "{'RoomID': '" + hdnCurrentSelectedRoom + "' ,'RoleID':'" + hdnRoleID.val() + "','SelectedModuleList':'" + hdnSelectedModuleList.val() + "','SelectedNonModuleList':'" + hdnSelectedNonModuleList.val() + "','SelectedDefaultSettings':'" + hdnSelectedDefaultSettings.val() + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (message) {
                
                
                $.ajax({
                    type: "POST",
                    url: url_CopyUserPermissionsToRooms,
                    data: "{'ParentRoomID': '" + SelectedRoomID + "' ,'CopyToRoomIDs':'" + CopyToRoomIDs + "','RoleID':'" + hdnRoleID.val() + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (message) {
                        
                        //$('div#target').fadeToggle();
                        //$("div#target").delay(2000).fadeOut(200);
                        showNotificationDialog();
                        $("#spanGlobalMessage").text('Permission Copied.');
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
        $("#spanGlobalMessage").text('Select Rooms to copy permission');
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('WarningIcon');
        return false;
    }
}

function RoomValidation() {
    var RoomAvailable = false;
    var SelectedRooms = '';
    $("#ddlSelectedRooms > option").each(function () {
        if (SelectedRooms == '') {
            SelectedRooms = this.value + "_" + this.text;
        }
        else {
            SelectedRooms += sep1 + this.value + "_" + this.text;
        }
        RoomAvailable = true;
    });

    hdnSelectedRoomAccessValue.val(SelectedRooms);
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
function SetAccessLevels(userType) {

    if (userType == "1") {
        $("#liEnterprises").css("display", "");
        $("#liCompanies").css("display", "");
        $("tr[id='41']").show();
        $("tr[id='39']").show();
        LoadEnterprises();
    }
    if (userType == "2") {
        $("#liEnterprises").css("display", "none");
        $("#liCompanies").css("display", "");
        $("tr[id='41']").hide();
        $("tr[id='39']").show();
        var arrnewEnterprise = new Array();
        arrnewEnterprise.push(UserEnterpriseID);
        LoadCompanies(arrnewEnterprise);
    }
    if (userType == "3") {
        $("#liEnterprises").css("display", "none");
        $("#liCompanies").css("display", "none");
        $("tr[id='41']").hide();
        $("tr[id='39']").hide();

        var arrnewCompany = new Array();
        arrnewCompany.push(EnterPriceID_CompanyId);
        LoadRooms(arrnewCompany);
        LoadReplenishRoomSelectBox();
    }
    LoadRolesByUserType(userType);
}
function LoadEnterprises() {
    $("#EnterpriseData").multiselect({
        noneSelectedText: 'Enterprise Access', selectedList: 5,
        selectedText: function (numChecked, numTotal, checkedItems) {
            return 'Enterprise Access';
        }
    }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
        CheckEnterpriseClick(ui, event);
    });
    LoadCompanies("")
}

function LoadRolesByUserType(userType) {
    $("#ddlRole > option").remove();
    $.ajax({
        url: url_getRoleListByUserType,
        type: "POST",
        data: { UserType: userType, CompanyId: UserCompanyId },
        success: function (response) {
            var s = '';
            $.each(response, function (i, val) {
                s += '<option value="' + val.ID + '"  >' + val.RoleName + '</option>';
            });
            $("#ddlRole").append(s);
            EditModeSetData();
        },
        error: function (response) {
            // through errror message
        }
    });
}

function LoadCompanies(enterpriseids) {

    var styrdata = JSON.stringify(enterpriseids);
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
    LoadRooms("");
    LoadReplenishRoomSelectBox();
}

function LoadRooms(companyids) {
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

            });
}
function FillSuppliers() {
    $.ajax({
        type: "POST",
        url: '@Url.Action("GetSuppliersByRoom", "Master")',
        contentType: 'application/json',
        dataType: 'json',
        data: "{RoomId:'" + parseInt('@Model.RoomID') + "' , CompanyId: '" + parseInt('@Model.CompanyId') + "'}",
        success: function (retdt) {
            $("select[id='99']").html("");
            $(retdt).each(function (index, obj) {
                $("select[id='99']").append("<option value='" + obj.ID + "'>" + obj.SupplierName + "</option>");
            });
        },
        error: function (err) {
            alert("There is some Error");
        }
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
            this.checked = Chked.checked;
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
    //  alert(TabNO);
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
                }
            }
        }
    });

    if (hasDelete == true || hasUpdate == true || hasInsert == true || hasShowDeleted == true || hasShowArchived == true || hasShowUDF == true) {
        Viewbtn.disabled = true;
    }
    else {
        Viewbtn.disabled = false;
        Viewbtn.checked = false;
    }
}