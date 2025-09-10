
var _CreateRole = (function ($) {
    var self = {};

    var dlgRoleUsersId = 'dlgRoleUsers';
    var ddlRoomDataId = 'RoomData';

    self.roomAccessOnLoad = null;

    self.init = function () {
        $(document).ready(function () {
            _dialogWrapper.init(dlgRoleUsersId, 'Users found in role', 500);
            self.roomAccessOnLoad = $("#hdnSelectedRoomAccessValue").val();
        });        
    }


    self.setIsECRAccessUpdated = function (val) {
        $("#hdnIsECRAccessUpdated").val(val ? 'True' : 'False');
    }

    self.getIsECRAccessUpdated = function (val) {
        return $("#hdnIsECRAccessUpdated").val();
    }

    self.getRoleId = function () {
        if ($("#hiddenID").length) {
            return $("#hiddenID").val();
        }
        else {
            return 0;
        }
    }

    self.getUserType = function () {
        return $("#drpUserType").val();
    }

    self.isRoomSelectedOnLoad = function (roomId) {
        var split = self.roomAccessOnLoad.split(sep1);
        for (var i = 0; i < split.length; i++) {
            var split2 = split[i].split('_');

            if (parseFloat(split2[2]) == parseFloat(roomId)) {
                return true;
            }
        }

        return false;
    }

    self.getUnSelectedRooms = function () {
        // get unselected rooms after page load
        var roomsOnLoad = self.roomAccessOnLoad.split(sep1);
        var selectedRooms = _multiSelectWrapper.getChecked(ddlRoomDataId);
        var unSelectedRooms = [];
        

        for (var i = 0; i < roomsOnLoad.length; i++) {
            var split2 = roomsOnLoad[i].split('_');
            var entId = split2[0];
            var roomOnPageLoad = split2[2];
            var isRoomFound = false;
            for (var j = 0; j < selectedRooms.length; j++) {
                var split3 = selectedRooms[j].value.split('_');
                
                var room = split3[2];

                if (parseFloat(roomOnPageLoad) == parseFloat(room)) {
                    isRoomFound = true;
                    break;
                }
            }

            if (isRoomFound == false) {
                unSelectedRooms.push(roomsOnLoad[i]);
            }

        }

        return unSelectedRooms;
    }

    self.getRoleAssignmentToUser = function () {
        $('#divRoleEditProcessing').show();
        var roleId = parseFloat(self.getRoleId());
        var $dvRoleUsers = $("#dvRoleUsers");
        $dvRoleUsers.html("");

        if (roleId > 0) {

            var UnSelectedRooms = self.getUnSelectedRooms();

            if (UnSelectedRooms.length == 0) {
                // no room un selected after page load
                self.submitForm();
                return false;
            }

            var roomIdCSV = '';
            
            $.each(UnSelectedRooms, function (ids, room) {
                roomIdCSV = roomIdCSV + room + ","
            })

            $('#DivLoading').show();

            // get users in role
            _AjaxUtil.getJson('/Master/GetRoleUsers'
                , { userType: self.getUserType(), roleId: roleId, roomIdCSV: roomIdCSV }
                , function (res) {

                    $('#DivLoading').hide();
                    var affectedUsers = [];

                    // get users without enforce role
                    $.each(res.roleUsers, function (idx, obj) {
                        if (obj.EnforceRolePermission == false) {
                            affectedUsers.push(obj);
                        }
                    });

                    if (affectedUsers.length) {

                        // prepae table for users list
                        _multiSelectWrapper.close(ddlRoomDataId);

                        var tblUsers = $("<table id='tblRoleUsers' class='display dataTable'></table>");

                        tblUsers.append("<tr><td width='15%'><b>#</b></td><td><b>User Name</b></td></tr>");

                        for (var i = 0; i < affectedUsers.length; i++) {
                            //var name = "<a href='#' onclick=_CreateRole.redirectToUser("
                            //    + affectedUsers[i].ID + "," + affectedUsers[i].UserType + ","
                            //    + affectedUsers[i].RoleId + "," + affectedUsers[i].EnterpriseId + ")>"
                            //    + affectedUsers[i].UserName + "</a>";
                            var name = "<a target='_blank' href='/Master/UserList?mode=e&id=" + affectedUsers[i].ID
                                + "&rid=" + affectedUsers[i].RoleId
                                + "&eid=" + affectedUsers[i].EnterpriseId
                                + "&typ=" + affectedUsers[i].UserType
                                + "')>" + affectedUsers[i].UserName + "</a>";
                            tblUsers.append("<tr><td>" + (i + 1) + "</td><td>" + name + "</td></tr>");
                        }

                        //for (var i = 0; i < 100; i++) {
                        //    var name = "<a href='/Master/UserList'>" + 1 + "</a>";
                        //    tblUsers.append("<tr><td>" + (i + 1) + "</td><td>" + name + "</td></tr>");
                        //}

                        $dvRoleUsers.append(tblUsers);

                        // show popup with users
                        _dialogWrapper.open(dlgRoleUsersId);

                    }
                    else {
                        self.submitForm();
                    }

                }, function (err) {
                    $('#DivLoading').hide();
                },
                true,
                false
            ); // ajax
        }
        else {
            self.submitForm();
        }

        $('#divRoleEditProcessing').hide();
        return false;
    }

    self.submitForm = function () {

        // un-tick disabled checkboxes for modules
        //$("#Roomtab3 input:checkbox:disabled[IsModuleDisable=true]").prop('checked',false);
        $("input:checkbox:disabled[IsModuleDisable=true]").prop('checked', false);

        $("#frmRole").submit();
    }

    self.selectedBillRoomID = '';

    self.getEntCompRoom = function (selId) {
        var sp = selId.split('_');
        return { EntId: sp[0], CompId: sp[1], RoomId: sp[2] };
    }

    self.getRoomEnabledModules = function () {
        $("#divRoleSelectProcessing").show();
        var selId = $("#ddlSelectedRooms").val();

        if (_utils.isNullUndefined(selId)) {
            return;
        }

        var chkListAll = $("#Roomtab3 input:checkbox");

        if (chkListAll.length == 0) {
            return;
        }

        //if (self.selectedBillRoomID == selId) {
        //    return;
        //}

        self.selectedBillRoomID = selId;

        var selECR = self.getEntCompRoom(selId);

        _Common.showHideLoader(true);

        _AjaxUtil.getJson('/BillingTypeModules/GetBillingRoomModules'
            , { roomId: selECR.RoomId, compId: selECR.CompId, entId: selECR.EntId }
            , function (res) {
                _Common.showHideLoader(false);
                var list = res.list;

                if (typeof list !== 'undefined' && list != null && list.length) {
                    
                    var len = list.length;
                    for (var i = 0; i < len; i++) {
                        var obj = list[i];
                        if (obj.ModuleID == AllUDFSetUpModuleID) {
                            if (obj.IsModuleEnabled == false) {
                                $("input:checkbox._ShowUDFAll").removeAttr("checked");
                                $("input:checkbox._ShowUDFAll").attr({ 'disabled': 'disabled', 'IsModuleDisable': true });
                                $("input:checkbox._ShowUDFAll").parent("td").css('background-color', '#FF6347');
                                var RoomTab2SelectAllUDF = $("#Roomtab2 th input[type='checkbox'][id='AllShowUDF$Tab2']");
                                if (RoomTab2SelectAllUDF.length > 0) {
                                    RoomTab2SelectAllUDF.removeAttr("checked");
                                    RoomTab2SelectAllUDF.attr({ 'disabled': 'disabled', 'IsModuleDisable': true });
                                    RoomTab2SelectAllUDF.parent("th").css('background-color', '#FF6347');
                                }
                                var RoomTab3SelectAllUDF = $("#Roomtab3 th input[type='checkbox'][id='AllShowUDF$Tab3']");
                                if (RoomTab3SelectAllUDF.length > 0) {
                                    RoomTab3SelectAllUDF.removeAttr("checked");
                                    RoomTab3SelectAllUDF.attr({ 'disabled': 'disabled', 'IsModuleDisable': true });
                                    RoomTab3SelectAllUDF.parent("th").css('background-color', '#FF6347');
                                }
                            }

                        } else {

                            //var chkList = $("#Roomtab3 input:checkbox[id^='" + obj.ModuleID + "_']");
                            var chkList = $("input:checkbox[id^='" + obj.ModuleID + "_']");

                            if (chkList.length == 0) {
                                continue;
                            }

                            if (obj.IsModuleEnabled == false) {
                                var moduleId = obj.ModuleID;
                                chkList.attr({ 'disabled': 'disabled', 'IsModuleDisable': true });
                                chkList.parent("td").css('background-color', '#FF6347');
                            }
                            else {
                                chkList.removeAttr('disabled');
                                chkList.parent("td").css('background-color', '');
                            }
                        }
                    }

                    
                }
                $("#divRoleSelectProcessing").hide();
            }, function (error) {
                _Common.showHideLoader(false);
                $("#divRoleSelectProcessing").hide();
            }, true, false);
    }


    self.EnableDisableRoomsControls = function (isGetRoomModules) {
        var SelectedRoomscnt = $("#ddlSelectedRooms > option").length;

        if (SelectedRoomscnt == 0) {
            $('#CreateRolePermissionDIV :input').attr('disabled', true);
            if (isGetRoomModules) {
                self.getRoomEnabledModules();
            }
        }
        else {
            $('#CreateRolePermissionDIV :input').removeAttr('disabled');
            self.getRoomEnabledModules();
        }
    }

    self.TemplateSelection = function (EnterpriseID, CompanyID, RoomID, RoleID, templateID) {

        $.ajax({
            type: "POST",
            url: url_SetTemplatePermissionToSession,
            data: "{'EnterpriseID': '" + EnterpriseID + "' ,'CompanyID': '" + CompanyID + "' ,'RoomID': '" + RoomID + "' ,'RoleID':'" + RoleID + "','templateID':'" + templateID + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (message) {
                ClearRoomAccessHidden();

                $("#CreateRolePermissionDIV").load(url_RolePermissionCreate, { RoomID: $('#ddlSelectedRooms').val(), RoleID: hdnRoleID.val(), UserType: $("#drpUserType").val() }, function () {
                    hdnCurrentSelectedRoom = $('#ddlSelectedRooms').val();
                    SetSelectedModule_NonModulePermissions();
                    if ($("#drpUserType").val() == "1") {
                        $("tr[id='module_41']").show();
                        $("tr[id='module_39']").show();
                    }
                    else if ($("#drpUserType").val() == "2") {
                        $("tr[id='module_41']").hide();
                        $("tr[id='module_39']").show();
                    }
                    else {
                        $("tr[id='module_41']").hide();
                        $("tr[id='module_39']").hide();
                    }
                });
                _CreateRole.EnableDisableRoomsControls();
            },
            error: function (response) {
                // through errror message
            }
        });

    }


    // initialise object
    self.init();

    return self;

})(jQuery);

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

var ProperUsername = true;
var show = true;
var pswd = '';
var Cpswd = '';

$("form").submit(function (e) {
    $.validator.unobtrusive.parse("#frmRole");
    if ($(this).valid()) {
        rememberUDFValues($("#hdnPageName").val(), '@Model.ID');
    }
    e.preventDefault();
});

$(document).ready(function () {
    $('form').areYouSure();
    checkRememberUDFValues($("#hdnPageName").val(), '@Model.ID');
    show = false;
    $('input#txtPassword').keyup(function () {

        pswd = $(this).val();
        if (pswd.length < 8) {
            $('#length').removeClass('valid').addClass('invalid');
        } else {
            $('#length').removeClass('invalid').addClass('valid');
        }
        if (pswd.match(/[A-z]/)) {
            $('#letter').removeClass('invalid').addClass('valid');
        } else {
            $('#letter').removeClass('valid').addClass('invalid');
        }

        //validate capital letter
        if (pswd.match(/[A-Z]/)) {
            $('#capital').removeClass('invalid').addClass('valid');
        } else {
            $('#capital').removeClass('valid').addClass('invalid');
        }
        if (pswd.match(/\W/g)) {
            $('#special').removeClass('invalid').addClass('valid');
        } else {
            $('#special').removeClass('valid').addClass('invalid');
        }

        //validate number
        if (pswd.match(/\d/)) {
            $('#number').removeClass('invalid').addClass('valid');
        } else {
            $('#number').removeClass('valid').addClass('invalid');
        }
        $('#pswd_info').show();
    }).focus(function () {
        $('#pswd_info').show();
    }).blur(function () {
        $('#pswd_info').hide();
    });
    $('input#txtConfirmPassword').keyup(function () {

        Cpswd = $(this).val();
        if (Cpswd.length < 8) {
            $('#Clength').removeClass('valid').addClass('invalid');
        } else {
            $('#Clength').removeClass('invalid').addClass('valid');
        }
        if (Cpswd.match(/[A-z]/)) {
            $('#Cletter').removeClass('invalid').addClass('valid');
        } else {
            $('#Cletter').removeClass('valid').addClass('invalid');
        }

        //validate capital letter
        if (Cpswd.match(/[A-Z]/)) {
            $('#Ccapital').removeClass('invalid').addClass('valid');
        } else {
            $('#Ccapital').removeClass('valid').addClass('invalid');
        }
        if (Cpswd.match(/\W/g)) {
            $('#Cspecial').removeClass('invalid').addClass('valid');
        } else {
            $('#Cspecial').removeClass('valid').addClass('invalid');
        }

        //validate number
        if (Cpswd.match(/\d/)) {
            $('#Cnumber').removeClass('invalid').addClass('valid');
        } else {
            $('#Cnumber').removeClass('valid').addClass('invalid');
        }
        $('#Cpswd_info').show();
    }).focus(function () {
        $('#Cpswd_info').show();
    }).blur(function () {
        $('#Cpswd_info').hide();
    });

});


$("input#txtUserName").focusout(function () {
    return ValidateUserName();
});

function ValidateUserName() {
    var currentUser = $.trim($("input#txtUserName").val());
    var UserId = $("input#hiddenID").val();
    $.ajax({
        url: "/master/ValidateUserName",
        type: "POST",
        data: { "UserName": currentUser, "UserID": UserId },
        success: function (res) {
            if (res != "ok") {
                if (res == "duplicate") {
                    $("span.DuplicateUserName").html(MsgUserNameAlreadyExist.replace("{0}", currentUser));
                    $("span.DuplicateUserName").show();
                    $("input#txtUserName").val('');
                    ProperUsername = false;
                    return false;
                }
                else {
                    $("span.DuplicateUserName").html(res);
                    $("span.DuplicateUserName").show();
                    $("input#txtUserName").val('');
                    ProperUsername = false;
                    return false;
                }
            }
            else {
                $("span.DuplicateUserName").hide();
                $("span.DuplicateUserName").html('');
                ProperUsername = true;
                return true;
            }
        },
        error: function (xhr) {
            console.log(xhr.status);
        },
        complete: function () {

        }
    });
}
$("input#txtPassword").focus(function (e) {
    $('#pswd_info').show();
});
$("input#txtPassword").focusout(function (e) {
    $('#pswd_info').hide();
});

$("input#txtPassword").click(function (e) {
    $('#pswd_info').show();
});
//confirmpassword
$("input#txtConfirmPassword").focus(function (e) {
    $('#Cpswd_info').show();
});
$("input#txtConfirmPassword").focusout(function (e) {
    $('#Cpswd_info').hide();
});

$("input#txtConfirmPassword").click(function (e) {
    $('#Cpswd_info').show();
});



var sep1 = "[!,!]";

function ShowHideEturnsAdmin() {
    if ($('#drpUserType').val() == '1') {
        $('#liIseTurnsAdmin').show();
    }
    else {
        $('#liIseTurnsAdmin').hide();
    }
}

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
                SelectedRoomReplanishment += sep1 + this.value;
        }
    });
    $("#hdnSelectedRoomReplanishmentValue").val('');
    $("#hdnSelectedRoomReplanishmentValue").val(SelectedRoomReplanishment);

    SetSelectedModule_NonModulePermissions();

    if (hdnSelectedModuleList.val() == ""
       && hdnSelectedNonModuleList.val() == "") {
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgRoomAccessRequired);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        return false;
    }

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
    $('#NarroSearchClear').click();
});



$(document).ready(function () {
    $("#divEditRoleLoading").show();
    var IsDeleted = $("#hdnIsDeleted").val();
    var TrueString = $("#hdTrueString").val();
    if (IsDeleted == TrueString) {
        disableControls('frmRole');
    }

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

    $("#ddlPermissionTemplateName").change(function () {

        if ($("#ddlSelectedRooms").val() != "" && $("#ddlSelectedRooms").val() != null) {


            var epid, compid, roomid;
            epid = $("#ddlSelectedRooms").val().split("_")[0];
            compid = $("#ddlSelectedRooms").val().split("_")[1];
            roomid = $("#ddlSelectedRooms").val().split("_")[2];

            _CreateRole.TemplateSelection(epid, compid, roomid, hdnRoleID.val(), $(this).val());
        }
    });

    $('#btnCancel').click(function (e) {
        //            if (IsRefreshGrid)
        //                $('#NarroSearchClear').click();
        SwitchTextTab(0, 'RoleCreate', 'frmRole');
        if (oTable !== undefined && oTable != null) {
            oTable.fnDraw();
        }
        $('#NarroSearchClear').click();
    });

    $('#ddlSelectedRooms').change(function (e) {
        RoomChanged();
    });

    $("#ddlDefaultPermissionRooms").multiselect(
                                {
                                    checkAllText: Check,
                                    uncheckAllText: UnCheck,
                                    noneSelectedText: RoomRes + ' ', selectedList: 5,
                                    selectedText: function (numChecked, numTotal, checkedItems) {
                                        return RoomRes + ' : ' + numChecked + ' ' + selected;
                                    }
                                }
                    ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                    });
    _CreateRole.EnableDisableRoomsControls(true);
    
    $("#divEditRoleLoading").hide();
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

            $("#CreateRolePermissionDIV").load(url_RolePermissionCreate, { RoomID: SelectedRoomID, RoleID: hdnRoleID.val(), UserType: $("#drpUserType").val() }, function () {
                if ($("#drpUserType").val() == "1") {
                    $("tr[id='module_41']").show();
                    $("tr[id='module_39']").show();
                }
                else if ($("#drpUserType").val() == "2") {
                    $("tr[id='module_41']").hide();
                    $("tr[id='module_39']").show();
                }
                else {
                    $("tr[id='module_41']").hide();
                    $("tr[id='module_39']").hide();
                }
                hdnCurrentSelectedRoom = SelectedRoomID;
                SetSelectedModule_NonModulePermissions();
                _CreateRole.EnableDisableRoomsControls();
            });
            
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
    else if (response.Status == "fail-roleaccess") {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#txtRoleName").focus();
    }
    else if (idValue == 0) {
        //clearControls('frmRole');
        //$("#txtRoleName").val("");
        $("#txtRoleName").focus();
        if (response.Status == "duplicate")
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
        else {
            // alert('in idvalue=0')
            //ClearRoleRelatedSession();
            clearControls('frmRole');
            ClearAllRolesDropdown();
            CallNarrowfunctions();
            SwitchTextTab(0, 'RoleCreate', 'frmRole');
        }
    }
    else if (idValue > 0) {

        if (response.Status == "duplicate") {
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
           // $("#txtRoleName").val("");
            $("#txtRoleName").focus();
        }
        else {
            // alert('in idvalue>0')
            //ClearRoleRelatedSession();
            clearControls('frmRole');
            ClearAllRolesDropdown();
           
            CallNarrowfunctions();
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
    $('#divRoleSelectProcessing').show();
    hdnSelectedModuleList.val('');
    hdnSelectedNonModuleList.val('');
    hdnSelectedDefaultSettings.val('');

    SelectedNonModuleList = [];
    SelectedModuleList = [];
    SelectedDefaultSettings = [];

    $("#CreateRolePermissionDIV input:checkbox").each(function () {
        var ControlID = this.id.toLowerCase();
        if ($('#drpUserType').val() == "1") {
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
        }
        else if ($('#drpUserType').val() == "2") {
            
            if (ValidateCheckBox(ControlID) == true) {
                if (ControlID.toLowerCase().indexOf("41_") < 0) {
                    if (this.checked == true) {
                        if (ControlID.toLowerCase().indexOf("_ischecked") != -1) {
                            SelectedNonModuleList.push(ControlID);
                        }
                        else {
                            SelectedModuleList.push(ControlID);
                        }
                    }
                }
            }
        }
        else if ($('#drpUserType').val() == "3") {
            if (ValidateCheckBox(ControlID) == true) {
                if (ControlID.toLowerCase().indexOf("41_") < 0 && ControlID.toLowerCase().indexOf("39_") < 0) {
                    if (this.checked == true) {
                        if (ControlID.toLowerCase().indexOf("_ischecked") != -1) {
                            SelectedNonModuleList.push(ControlID);
                        }
                        else {
                            SelectedModuleList.push(ControlID);
                        }
                    }
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
    $('#divRoleSelectProcessing').hide();
}

function ClearRoomAccessHidden() {
    hdnSelectedModuleList.val('');
    hdnSelectedNonModuleList.val('');
    SelectedModuleList = [];
    SelectedNonModuleList = [];
}

function CheckEnterpriseClick(chkDropdown, event) {

    //    var arrItems = new Array();
    //if (event.type == 'multiselectcheckall') {
    //    $("#EnterpriseData > option").each(function () {
    //        if (this.selected == true)
    //            arrItems.push(this.value);
    //    });

    //    if (chkDropdown.checked == true) {
    //        arrItems.push(chkDropdown.value);
    //    }
    //    else {
    //        var selected = chkDropdown.value;
    //        arrItems = jQuery.grep(arrItems, function (value) {
    //            return value != selected;
    //        });
    //    }
    var arrItems = $("#EnterpriseData").multiselect("getChecked").map(function () { return this.value }).get();
    //    if (chkDropdown.selected == true) {
    //        arrItems.push(chkDropdown.value);
    //    }

    SelectedEnterprises = arrItems;

    $("#hdnSelectedEnterpriseAccessValue").val(SelectedEnterprises.join(sep1));
    _CreateRole.setIsECRAccessUpdated(true);
    LoadCompanies(arrItems);

    $("#ddlDefaultPermissionRooms > option").remove();
    ddlDefaultPermissionRooms.multiselect("refresh");
}

function CheckCompanyClick(chkDropdown, event) {

    //    var arrItems = new Array();
    //if (event.type == 'multiselectcheckall') {
    //    $("#CompanyData > option").each(function () {
    //        if (this.selected == true)
    //            arrItems.push(this.value);
    //    });

    //    if (chkDropdown.checked == true) {
    //        arrItems.push(chkDropdown.value);
    //    }
    //    else {
    //        var selected = chkDropdown.value;
    //        arrItems = jQuery.grep(arrItems, function (value) {
    //            return value != selected;
    //        });
    //    }
    //    if (chkDropdown.selected == true) {
    //        arrItems.push(chkDropdown.value);
    //    }
    var arrItems = $("#CompanyData").multiselect("getChecked").map(function () { return this.value }).get();
    SelectedCompanies = arrItems;
    $("#hdnSelectedCompanyAccessValue").val(SelectedCompanies.join(sep1));
    _CreateRole.setIsECRAccessUpdated(true);
    LoadRooms(arrItems);
    $("#ddlDefaultPermissionRooms > option").remove();
    ddlDefaultPermissionRooms.multiselect("refresh");
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
    $("#hdnSelectedRoomAccessValue").val(arrnewrooms.join(sep1));
    _CreateRole.setIsECRAccessUpdated(true);
    
    SelectedRooms = [];
    $("#ddlSelectedRooms > option").each(function () {
        SelectedRooms.push(this.value);
    });

    AddRemoveRoomsToSession(SelectedRooms);

}

function EditModeSetData() {
    //$("#divRoleEditProcessing").show();
    $("#ddlSelectedRooms").html("");
    if (hdnSelectedRoomAccessValue.val() != '') {
        var FirstRoomID = 0;
        var Rn = hdnSelectedRoomAccessValue.val().split(sep1);
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
    if (SelectedRooms.length > 0) {
        AddRemoveRoomsToSession(SelectedRooms);
    }
    var tmpRoleId = 0;
    if ($("#hiddenID").length) {
        tmpRoleId = (parseInt($("#hiddenID").val()) || 0);
    }
    
    if (tmpRoleId < 1) {
        $("#divRoleProcessing,#divRoleSelectProcessing,#divRoleEditProcessing").hide();
    }
    //$("#divEditRoleLoading").hide();
    //$("#divRoleEditProcessing").hide();
    //        EnableDisableRoomsControls();
}

function AddRemoveRoomsToSession(SelectedRooms) {
    var SelectedRoomID = 0
    $.ajax({
        type: "POST",
        url: url_AddRemoveRoomsToSession,
        data: "{'RoomIDs': '" + SelectedRooms.join(sep1) + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (message) {
            if (SelectedRooms.join(sep1) != '') {
                SelectedRoomID = SelectedRooms[0];
            }
            $("#CreateRolePermissionDIV").load(url_RolePermissionCreate, { RoomID: SelectedRoomID, RoleID: hdnRoleID.val(), UserType: $("#drpUserType").val() }, function () {
                if ($("#drpUserType").val() == "1") {
                    $("tr[id='module_41']").show();
                    $("tr[id='module_39']").show();
                }
                else if ($("#drpUserType").val() == "2") {
                    $("tr[id='module_41']").hide();
                    $("tr[id='module_39']").show();
                }
                else {
                    $("tr[id='module_41']").hide();
                    $("tr[id='module_39']").hide();
                }
                hdnCurrentSelectedRoom = SelectedRoomID;
                SetSelectedModule_NonModulePermissions();
                _CreateRole.EnableDisableRoomsControls();
            });
            SetDefaultPermissionRooms();
            
        },
        error: function (response) {
            // through errror message

        }
    });
}
function EditModeSetRoomReplanishmentData() {

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
                CopyToRoomIDs += sep1 + this.value;
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
                        //showNotificationDialog();
                        //$("#spanGlobalMessage").text('Permission Copied.');
                        //$("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('succesIcon');
                        _notification.showSuccess(MsgPermissionCopied);
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
        $("#spanGlobalMessage").html(MsgSelectRoomsCopyPermission);
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
    else if (Chked.toLowerCase().indexOf("allshowdeleted") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allshowarchived") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allshowudf") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allshowchangelog") != -1) {
        result = false;
    }
    return result;

}

function ClearAllRolesDropdown() {
    $.ajax({
        "dataType": 'json',
        "type": "POST",
        "url": '/Master/ClearSelectedRoleListSession',
        "success": function (retdata) {


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
    });

}

$("#drpUserType").change(function () {
    SetAccessLevels($(this).val());
    clearAlloption();
});

function clearAlloption()
{
    $("#hdnSelectedEnterpriseAccessValue").val('');
    $("#hdnSelectedCompanyAccessValue").val('');
    $("#hdnSelectedRoomAccessValue").val('');

    $("#EnterpriseData > option").remove();
    $("#EnterpriseData").multiselect("refresh");

    $("#CompanyData > option").remove();
    $("#CompanyData").multiselect("refresh");

    $("#RoomData > option").remove();
    $("#RoomData").multiselect("refresh");

    $("#ddlSelectedRooms > option").remove();
    $("#ddlSelectedRooms").multiselect("refresh");

    $("#ddlDefaultPermissionRooms > option").remove();
    $("#ddlDefaultPermissionRooms").multiselect("refresh");

    $("#Roomtab2 input:checkbox").each(function () {
        this.checked = false;
    });
    $("#Roomtab3 input:checkbox").each(function () {
        this.checked = false;
    });

    $("#Roomtab4 input:checkbox").each(function () {
        this.checked = false;
    });

    SelectedRooms = [];
    AddRemoveRoomsToSession(SelectedRooms);   
}

function SetAccessLevels(userType) {
    var ihiddenID = $("#hiddenID").val();
    if (userType == "1") {
        $("#liEnterprises").css("display", "");
        $("#liCompanies").css("display", "");
        $("tr[id='module_41']").show();
        $("tr[id='module_39']").show();
        if (ihiddenID == "0") {
            $("#hdnSelectedEnterpriseAccessValue").val('');
            $("#hdnSelectedCompanyAccessValue").val('');
            $("#hdnSelectedRoomAccessValue").val('');
        }
        LoadEnterprises();

    }
    if (userType == "2") {
        $("#liEnterprises").css("display", "none");
        $("#liCompanies").css("display", "");
        $("tr[id='module_41']").hide();
        $("tr[id='module_39']").show();

        //$("#EnterpriseData").multiselect("uncheckAll");
        //$("#CompanyData").multiselect("uncheckAll");
        //$("#RoomData").multiselect("uncheckAll");

        if (ihiddenID == "0") {
            $("#hdnSelectedEnterpriseAccessValue").val('');
            $("#hdnSelectedCompanyAccessValue").val('');
            $("#hdnSelectedRoomAccessValue").val('');
        }
       
        if ($("#hdnRoleWiseEnterpriseId") != undefined && $("#hdnRoleWiseEnterpriseId").val() != "" && $("#hdnRoleWiseEnterpriseId").val() != "0")
        {
            RoleEnterpriseID = $("#hdnRoleWiseEnterpriseId").val();
        }

        var arrnewEnterprise = new Array();
        arrnewEnterprise.push(RoleEnterpriseID);
        LoadCompanies(arrnewEnterprise);

    }
    if (userType == "3") {

        $("#liEnterprises").css("display", "none");
        $("#liCompanies").css("display", "none");
        $("tr[id='module_41']").hide();
        $("tr[id='module_39']").hide();

        //$("#EnterpriseData").multiselect("uncheckAll");
        //$("#CompanyData").multiselect("uncheckAll");
       // $("#RoomData").multiselect("uncheckAll");
        
        if (ihiddenID == "0") {
            $("#hdnSelectedEnterpriseAccessValue").val('');
            $("#hdnSelectedCompanyAccessValue").val('');
            $("#hdnSelectedRoomAccessValue").val('');
        }

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
                var eids = $("#hdnSelectedEnterpriseAccessValue").val().split(sep1);
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
    //alert(enterpriseids);
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
                var eids = $("#hdnSelectedCompanyAccessValue").val().split(sep1);
                for (var e = 0; e < eids.length; e++) {
                    $("#CompanyData").multiselect("widget").find(":checkbox[value='" + eids[e] + "']").attr("checked", "checked");
                    $("#CompanyData option[value='" + eids[e] + "']").attr("selected", 1);                    
                }
                $("#CompanyData").multiselect("refresh");
            }
            var arrnewCompany = $("#CompanyData").multiselect("getChecked").map(function () { return this.value; }).get();
            $("#hdnSelectedCompanyAccessValue").val(arrnewCompany.join(sep1));
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
            //$("#RoomData").multiselect("refresh");
            $("#RoomReplanishment > option").remove();
            //$("#RoomReplanishment").multiselect("refresh");
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
                var eids = $("#hdnSelectedRoomAccessValue").val().split(sep1);
                for (var e = 0; e < eids.length; e++) {
                    var optionname = "";
                    var thirducindx = nth_occurrence(eids[e], '_', 3);
                    if (thirducindx != -1) {
                        optionname = eids[e].substring(0, thirducindx);
                    }
                    $("#RoomData").multiselect("widget").find(":checkbox[value='" + optionname + "']").attr("checked", "checked");
                    $("#RoomData option[value='" + optionname + "']").attr("selected", 1);
                    
                }
                $("#RoomData").multiselect("refresh");
            }
            var arrnewrooms = $("#RoomData").multiselect("getChecked").map(function () { return this.value + "_" + this.title; }).get();
            $("#hdnSelectedRoomAccessValue").val(arrnewrooms.join(sep1));

            if ($("#hdnSelectedRoomReplanishmentValue").val() != '') {
                var eids = $("#hdnSelectedRoomReplanishmentValue").val().split(sep1);
                for (var e = 0; e < eids.length; e++) {
                    $("#RoomReplanishment").multiselect("widget").find(":checkbox[value='" + eids[e] + "']").attr("checked", "checked");
                    $("#RoomReplanishment option[value='" + eids[e] + "']").attr("selected", 1);
                    
                }
                $("#RoomReplanishment").multiselect("refresh");
            }
            var arrreplerooms = $("#RoomReplanishment").multiselect("getChecked").map(function () { return this.value; }).get();
            $("#hdnSelectedRoomReplanishmentValue").val(arrreplerooms.join(sep1));

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
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: EnterPriceAccess, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return EnterPriceAccess;
                            }
                        }

            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                CheckEnterpriseClick(ui, event);
            }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });
}

function LoadComanySelectBox() {

    $("#CompanyData").multiselect(
                        {
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: CompanyAccess, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return CompanyAccess;
                            }
                        }

            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                CheckCompanyClick(ui, event);
            }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });
}

function LoadRoomSelectBox() {
    $("#RoomData").multiselect(
                        {
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: RoomAccess, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return RoomAccess;
                            }
                        }

            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                Checkclick(ui, event);
            }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });
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
    $('#divRoleProcessing').show();
    if (TabNO == '1') {
        $("#Roomtab2 input:checkbox:enabled").each(function () {
            this.checked = Chked.checked;
        });
    }
    else if (TabNO == '2') {
        $("#Roomtab3 input:checkbox:enabled").each(function () {
            this.checked = Chked.checked;
        });
    }
    else if (TabNO == '3') {
        $("#Roomtab4 input:checkbox:enabled").each(function () {
            var vModuleID = $(this).prop("id").split('_')[0]
            if ((vModuleID != "116" && vModuleID != "118" && vModuleID != "121") || Chked.checked == false)
                this.checked = Chked.checked;
            else
                this.checked = false;
        });
    }
    $('#divRoleProcessing').hide();
}

function SelectRowColumnChk(Chked, TabNO) {
       

    if (Chked.id.toLowerCase().indexOf("rowall") != -1) {
        // all checkbox in a row
        var chkModuleID = Chked.id.split("_");
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox:enabled").not(":disabled");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox:enabled");
        }
        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID[0] == chkModuleID[0]) {
                this.checked = Chked.checked;
            }
        });
    }

    var fnTickColumnCheckbox = function (permission) {
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox:enabled");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox:enabled");
        }

        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID.length > 1) {
                if (currentModuleID[1].toLowerCase() == permission) {
                    this.checked = Chked.checked;
                }
            }
        });

    }

    // ----- Select All View permission --------
    if (Chked.id.toLowerCase().indexOf("allview") != -1) {
        fnTickColumnCheckbox('view');
    }
    // ----- Select All Insert permission --------
    else if (Chked.id.toLowerCase().indexOf("allinsert") != -1) {
        fnTickColumnCheckbox('insert');
    }
    // ----- Select All Delete permission --------
    else if (Chked.id.toLowerCase().indexOf("alldelete") != -1) {
        fnTickColumnCheckbox('delete');
    }
    // ----- Select All Update permission --------
    else if (Chked.id.toLowerCase().indexOf("allupdate") != -1) {
        fnTickColumnCheckbox('update');        
    }
    else if (Chked.id.toLowerCase().indexOf("allshowdeleted") != -1) {
        fnTickColumnCheckbox('showdeleted');
    }
    else if (Chked.id.toLowerCase().indexOf("allshowarchived") != -1) {
        fnTickColumnCheckbox('showarchived');
    }
    else if (Chked.id.toLowerCase().indexOf("allshowarchived") != -1) {
        fnTickColumnCheckbox('showarchived');
    }
    else if (Chked.id.toLowerCase().indexOf("allshowudf") != -1) {
        fnTickColumnCheckbox('showudf');
    }
    else if (Chked.id.toLowerCase().indexOf("allshowchangelog") != -1) {
        fnTickColumnCheckbox('showchangelog');
    }

    // ----- Select All Show Deleted,Archived and UDF permission --------
    //SelectRowColumnChkAll(Chked, TabNO, "allshowdeleted", "showdeleted");
    //SelectRowColumnChkAll(Chked, TabNO, "allshowarchived", "showarchived");
    //SelectRowColumnChkAll(Chked, TabNO, "allshowudf", "showudf");
    
    //SelectRowColumnChkAll(Chked, TabNO, "allshowchangelog", "showchangelog");
    // ----- Select All Show Deleted,Archived and UDF permission --------

}

//function SelectRowColumnChkAll(Chked, TabNO, Allchkname, chkname) {
//    if (Chked.id.toLowerCase().indexOf(Allchkname) != -1) {
//        var Tabs = '';
//        if (TabNO == '1') {
//            var Tabs = $("#Roomtab2 input:checkbox");
//        }
//        else if (TabNO == '2') {
//            var Tabs = $("#Roomtab3 input:checkbox");
//        }

//        Tabs.each(function () {
//            var currentModuleID = this.id.split("_");
//            if (currentModuleID.length > 1) {
//                if (currentModuleID[1].toLowerCase() == chkname) {
//                    this.checked = Chked.checked;
//                }
//            }
//        });
//    }
//}



//function CheckViewChkBox(Chked, TabNO) {
//    var Tabs = '';
//    if (TabNO == '1') {
//        var Tabs = $("#Roomtab2 input:checkbox:enabled");
//    }
//    else if (TabNO == '2') {
//        var Tabs = $("#Roomtab3 input:checkbox:enabled");
//    }

//    Tabs.each(function () {
//        var currentModuleID = this.id.split("_");
//        if (currentModuleID.length > 1) {
//            if (currentModuleID[1].toLowerCase() == 'view') {
//                this.checked = Chked.checked;
//            }
//        }
//    });
//}

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



function ClearRoleRelatedSession() {
    setTimeout(function () {
        $.ajax({
            "dataType": 'json',
            "type": "POST",
            "url": clearSessionURL,
            "success": function (retdata) {

            }
        })

    }, 3000);
}

