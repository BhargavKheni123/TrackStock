var IsEditMode = false;
var IsShowView = false;
var IsShowHistory = false;
var HasInsertPermission = true;
var HasOnlyViewPermission = false;

//window.addEventListener('popstate', function (event) {
    //alert("location: " + document.location + ", state: " + JSON.stringify(event.state));
    //    alert(window.location.href);
    //    window.location.href = window.location.href;
//});

function TabClass(id, name, callback, isVisible, isEnable, isNotification, redCircleID) {
    this.Id = id;
    this.Name = name;
    this.CallBack = callback;
    this.IsVisible = isVisible;
    this.IsEnable = isEnable;
    this.IsNotification = isNotification;
    this.RedCircleID = redCircleID;
}
var tabrawhtml = "<li><div class='verticalText' id='[ID]' style='display:[DISPLAY];' callback=[CALLBACK] IsEnable=[ISENABLE]  ><div class='tabrightbg'></div><div class='vertical-text-inner tabmidbg'>[NOTIFICATION]<span id='spnTabName' style='[PADDING]'>[TABNAME]</span></div><div class='tableftbg'></div></div></li>";
function LoadTabs() {
    for (var i = 0; i < TabsArry.length; i++) {
        //        if (TabsArry[i].IsNotification != "" && TabsArry[i].IsNotification != "0" && typeof TabsArry[i].IsNotification != "undefined") {
        //            if (typeof TabsArry[i].IsNotification != "undefined") {
        //                $('.tabs').append(tabrawhtml.replace('[ID]', TabsArry[i].Id).replace('[TABNAME]', TabsArry[i].Name).replace('[DISPLAY]', (TabsArry[i].IsVisible ? 'block' : 'none')).replace('[CALLBACK]', TabsArry[i].CallBack).replace('[ISENABLE]', TabsArry[i].IsEnable).replace('[NOTIFICATION]', "<div style='display:block;' id='" + TabsArry[i].RedCircleID + "' class='notification'>" + TabsArry[i].IsNotification + "</div>"));
        //            }
        //            else {
        //                $('.tabs').append(tabrawhtml.replace('[ID]', TabsArry[i].Id).replace('[TABNAME]', TabsArry[i].Name).replace('[DISPLAY]', (TabsArry[i].IsVisible ? 'block' : 'none')).replace('[CALLBACK]', TabsArry[i].CallBack).replace('[ISENABLE]', TabsArry[i].IsEnable).replace('[NOTIFICATION]', "<div style='display:block;' id='" + TabsArry[i].RedCircleID + "' class='notification'>" + TabsArry[i].IsNotification + "</div>"));
        //            }
        //        }
        //        else {
        //            $('.tabs').append(tabrawhtml.replace('[ID]', TabsArry[i].Id).replace('[TABNAME]', TabsArry[i].Name).replace('[DISPLAY]', (TabsArry[i].IsVisible ? 'block' : 'none')).replace('[CALLBACK]', TabsArry[i].CallBack).replace('[ISENABLE]', TabsArry[i].IsEnable).replace('[NOTIFICATION]', "<div style='display:none;' id='" + TabsArry[i].RedCircleID + "' class='notification'></div>").replace('[PADDING]', "padding-right:5px !important"));
        //        }

        if (!isNaN(parseInt(TabsArry[i].IsNotification)) && parseInt(TabsArry[i].IsNotification) > 0) {
            $('.tabs').append(tabrawhtml.replace('[ID]', TabsArry[i].Id).replace('[TABNAME]', TabsArry[i].Name).replace('[DISPLAY]', (TabsArry[i].IsVisible ? 'block' : 'none')).replace('[CALLBACK]', TabsArry[i].CallBack).replace('[ISENABLE]', TabsArry[i].IsEnable).replace('[NOTIFICATION]', "<div style='display:block;' id='" + TabsArry[i].RedCircleID + "' class='notification'>" + TabsArry[i].IsNotification + "</div>").replace('[PADDING]', "padding-right:25px;padding-left:5px"));
        }
        else {
            $('.tabs').append(tabrawhtml.replace('[ID]', TabsArry[i].Id).replace('[TABNAME]', TabsArry[i].Name).replace('[DISPLAY]', (TabsArry[i].IsVisible ? 'block' : 'none')).replace('[CALLBACK]', TabsArry[i].CallBack).replace('[ISENABLE]', TabsArry[i].IsEnable).replace('[NOTIFICATION]', "<div style='display:none;' id='" + TabsArry[i].RedCircleID + "' class='notification'></div>").replace('[PADDING]', "padding-right:5px"));
        }

    }
    SetDefaults();
    TabHover();
}
function TabHover() {
    $(".tabs > li > div").each(function (i) {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('unselected')
        }
        else {
            $(this).addClass('unselected');
        }
        $(".unselected").hover(
           function () { $(this).addClass('selected') },
           function () { $(this).removeClass('selected') }
    );
        
        i++;
    });

    //$(".verticalText").hover(
    $(".verticalText").off('hover').on('hover', function () {
        if (!$(this).hasClass('unselected')) {
            $(this).addClass('selected')
        }
    });
}


function SetDefaults() {

    $('.tabContener').hide();
    $('#tab5').addClass('selected');
    $('#Ctab').show();

    $('.verticalText').click(function () {

        //check for any input field value change and message show to user before navigate
        if (!dirtyCheck()) {
            return false;
        }
        else {
            removeDirtyclass();
        }
        //End navigate
        //        if (isDirtyForm) {
        //            if (confirm(SaveConfirmationMSG)) {
        //                //isDirtyForm = false;
        //                return false;
        //            }
        //            isDirtyForm = false;
        //        }

        if ($(this).attr('IsEnable') == 'true') {

            $('.verticalText').removeClass('selected');
            $('.tabContener').hide();
            var selectedid = $(this).attr('id');
            $(this).addClass('selected');
            TabHover();
            TabEnableDisable("#tab1", true);
            TabEnableDisable("#tab21", true);
            TabEnableDisable("#tab22", true);
            TabEnableDisable("#tab23", true);
            TabEnableDisable("#tab24", true);
            TabEnableDisable("#tab25", true);
            TabEnableDisable("#tab26", true);
            TabEnableDisable("#tab27", true);
            TabEnableDisable("#tab28", true);
            TabEnableDisable("#tab29", true);
            TabEnableDisable("#tab30", true);
            TabEnableDisable("#tab31", true);
            TabEnableDisable("#tabTM", true);
            TabEnableDisable("#tabTS", true);
            TabEnableDisable("#tab11", true);
            TabEnableDisable("#tabSchedule", true);
            TabEnableDisable("#tabOdometer", true);
            TabEnableDisable("#tabOdometerList", true);
            TabEnableDisable("#tabMaintenance", true);
            TabEnableDisable("#tabScheduleList", true);
            TabEnableDisable("#tabToolHistory", true);
            TabEnableDisable("#tabBuildBreak", true);
            TabEnableDisable("#tabWrittenOffTool", true);
            TabEnableDisable("#tabReturnList", true);
            

            if (selectedid == "tab1") {
                $('#CtabNew').show();
                TabEnableDisable("#tab1", false);
            }
            else if (selectedid == "tab6") {
                $('#CtabCL').show();
                TabEnableDisable("#tab1", true);
                if (HasInsertPermission) {
                    $('#tab1').find("#spnTabName").html(NewNameRes);
                }
                else if (HasOnlyViewPermission) {
                    $('#tab1').find("#spnTabName").html(ViewNameRes);
                }
                if (HasInsertPermission == "False") {
                    $('#tab1').hide();
                }
                AllowDeletePopup = false;
            }
            else if (selectedid == "tab21") { // For Move Out
                $('#CtabNew').show();
                TabEnableDisable("#tab21", false);
            }
            else if (selectedid == "tab22") { // For Move Out
                $('#CtabNew').show();
                TabEnableDisable("#tab22", false);
            }
            else if (selectedid == "tab24") { // For Move Out
                $('#CtabNew').show();
                TabEnableDisable("#tab24", false);
            }
            else if (selectedid == "tab25") { // For Move Out
                $('#CtabNew').show();
                TabEnableDisable("#tab25", false);
            }
            else if (selectedid == "tab26") { // For Move Out
                $('#CtabNew').show();
                TabEnableDisable("#tab26", false);
            }
            else if (selectedid == "tab23") { // For Move Out
                $('#CtabNew').show();
                TabEnableDisable("#tab23", false);
            }
            else if (selectedid == "tab11") { // For Order Receive
                $('#CtabNew').show();
                TabEnableDisable("#tab11", false);
            }
            else if (selectedid == "tabSchedule") {
                $('#CtabSCH').show();
                TabEnableDisable("#tabSchedule", false);
            }
            else if (selectedid == "tabOdometer") {
                $('#CtabOdometer').show();
                TabEnableDisable("#tabOdometer", false);
            }
            else if (selectedid == "tabOdometerList") {
                $('#CtabOdometerList').show();
                TabEnableDisable("#tabOdometerList", false);
            }
            else if (selectedid == "tabMaintenance") {
                $('#CtabMaintenance').show();
                TabEnableDisable("#tabMaintenance", false);
            }
            else if (selectedid == "tabScheduleList") {
                $('#CtabSchedulerList').show();
                TabEnableDisable("#tabScheduleList", false);
            }
            else if (selectedid == "CtabModuleConfig") {
                $('#CtabModuleConfiguration').show();
                TabEnableDisable("#CtabModuleConfiguration", false);
            }
            else if (selectedid == "tab27") { // For Move Out                
                MaintenanceEdit = false;
                MaintenanceView = false;
                if (HasInsertPermission) {
                    $('#tab1').find("#spnTabName").html(NewNameRes);
                }                
                if (HasInsertPermission == "False") {
                    $('#tab1').hide();
                }
                AllowDeletePopup = false;
                TabEnableDisable("#tab27", false);
            }
            else if (selectedid == "tab28") { // For Move Out
                $('#CtabNew').show();
                MaintenanceEdit = false;
                MaintenanceView = false;
                if (HasInsertPermission) {
                    $('#tab1').find("#spnTabName").html(NewNameRes);
                }
               
                if (HasInsertPermission == "False") {
                    $('#tab1').hide();
                }
                AllowDeletePopup = false;
                TabEnableDisable("#tab28", false);
            }
            else if (selectedid == "tab30") { // For Move Out
                $('#CtabReqWo').show();
                TabEnableDisable("#tab30", false);
            }
            else if (selectedid == "tab31") { // For Move Out
                $('#CtabReqWo').show();
                TabEnableDisable("#tab31", false);
            }
            else if (selectedid == "tabMMHistory") { // For Move History
                TabEnableDisable("#tab1", true);
                if (HasInsertPermission == "False") {
                    $('#tab1').hide();
                }
            }
            else if (selectedid == "tabToolHistory") { // For Tool History
                $('#CtabToolHistoryList').show();
                TabEnableDisable("#tabToolHistory", false);
            }
            else if (selectedid == "tabBuildBreak") {
                $('#CtabCL').show();
                TabEnableDisable("#tabBuildBreak", false);
            }
            else if (selectedid == "tabWrittenOffTool") { // For Written Off Tool List
                $('#CtabWrittenOffTool').show();
                TabEnableDisable("#tabWrittenOffTool", false);
            }
            else if (selectedid == "tabReturnList") { // For Written Off Tool List
                $('#CtabReturnList').show();
                TabEnableDisable("#tabReturnList", false);
            }
            else {
                if (HasInsertPermission) {
                    $('#tab1').find("#spnTabName").html(NewNameRes);
                }
                else if (HasOnlyViewPermission) {
                    $('#tab1').find("#spnTabName").html(ViewNameRes);
                }
                TabEnableDisable("#tab1", true);
                $('#Ctab').show();
                $('#tab6').hide();
                if (HasInsertPermission == "False") {
                    $('#tab1').hide();
                }
                $("#tabScheduleList").hide();
                $("#tabMaintenance").hide();
                $("#tabOdometer").hide();
                $("#tabOdometerList").hide();
                $("#tabSchedule").hide();
                $("#tabBuildBreak").hide();
                
                if (selectedid == "tab5") {
                    AllowDeletePopup = true;
                    if (IsRefreshGrid) {
                        IsRefreshGrid = false;
                        IsEditMode = false;
                        if (typeof (oTable) != "undefined" && oTable != null) {
                            if (oTable != null) {
                                oTable.fnDraw();
                            }
                        }

                    }
                    if (window.location.href.indexOf("#new") > -1) {
                        //oTable.fnStandingRedraw();
                        if (typeof (oTable) != "undefined" && oTable != null) {
                            if (oTable != null) {
                                oTable.fnAdjustColumnSizing();
                            }
                        }
                    }
                }
            }

            ShowHidHistoryTabNew();
            eval($(this).attr('callback'));
        }
    });

}

function ShowHidHistoryTabNew() {
    // To show or hide the History(change log) tab
    if (typeof (oTable) == "undefined" || oTable === null || oTable === undefined)
        return;

    var anSelectedRows = fnGetSelected(oTable);
    if (anSelectedRows.length == 1) {
        $("#tab6").show();
        $("#tab23").show();
    }
    else {
        if (IsShowHistory) {
            $("#tab6").show();
            $("#tab23").show();
            IsShowHistory = false;
        }
        else {
            $("#tab6").hide();
            $("#tab23").hide();
        }
    }

}

function ShowNewTab(action, formName) {
    if (IsEditMode) {
        IsEditMode = false;
        return;
    }

    AllowDeletePopup = false;
    $('#DivLoading').show();
    $(formName).append($('#CtabNew').load(action, function () {

        $('#DivLoading').hide();
        //$("#" + formName + " :input:text:visible:first").focus();
        setTimeout(function () {
            $("#" + formName).find('input, textarea, select')
            .not('input[type=hidden],input[type=radio],input[type=button],input[type=submit],input[type=reset],input[type=image],button,:disabled')
            .filter(':enabled:visible:first')
            .focus();
        },1500);
    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');

    //    if (window.location.hash.toLowerCase().indexOf("new") < 0) {
    //        history.pushState(null, null, window.location.pathname + "#new");
    //    }

}
$.validator.unobtrusive.parseDynamicContent = function (selector) {
    $.validator.unobtrusive.parse(selector);
    var form = $(selector).first().closest('form');
    var unobtrusiveValidation = form.data('unobtrusiveValidation');
    var validator = form.validate();
}
function ShowEditTab(action, formName) {
    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    action += '?IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted;
    if (formName == 'frmCostUOM') {
        var isForBom = $('#hdnisForBom').val();
        action += '&isForBom=' + isForBom;
    }
    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;

    if (!HasOnlyViewPermission) {
        if (IsArchived || IsDeleted)
            $('#tab1').find("#spnTabName").html(ViewNameRes);
        else
            $('#tab1').find("#spnTabName").html(EditNameRes);
    }
    else {
        $('#tab1').find("#spnTabName").html(ViewNameRes);
    }
    $('#DivLoading').show();
    $('#tab1').show();
    $('#tab1').click();
    $(formName).append($('#CtabNew').load(action, function () {
        $('#DivLoading').hide();
        $("#" + formName + " :input:text:visible:first").focus();

        //var scope = angular.element($("#CtabNew")).scope();
        //if (scope != undefined) {
        //    var injector = $('[ng-app]').injector();
        //    var $compile = injector.get('$compile');
        //    //$compile($('#CtabNew').html())(scope);
        //    $('#liSupplierBlanketPODetails').html($compile($('li#liSupplierBlanketPODetails').html())(scope));

        //}

    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}
function ShowEditTabNotification(action, formName, rowobj) {

    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    action += '?IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted + '&forcopy=' + rowobj.CompanyName;
    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;

    //    if (!HasOnlyViewPermission) {
    //        if (IsArchived || IsDeleted)
    //            $('#tab24').find("#spnTabName").text(ViewNameRes);
    //        else
    //            $('#tab24').find("#spnTabName").text(EditNameRes);
    //    }
    //    else {
    //        $('#tab24').find("#spnTabName").text(ViewNameRes);
    //    }
    $('#DivLoading').show();
    if (rowobj.CompanyName == "true") {
        if (rowobj.ScheduleFor == 55) {
            $('#tab25').show();
            $('#tab25').find("#spnTabName").html(ReportNew);
            $('#tab24').find("#spnTabName").html(AlertNew);
            $('#tab25').click();
        }
        else if (rowobj.ScheduleFor == 66) {
            $('#tab24').show();
            $('#tab24').find("#spnTabName").html(AlertNew);
            $('#tab25').find("#spnTabName").html(ReportNew);
            $('#tab24').click();
        }
        else {
            $('#tab24').hide();
            $('#tab25').hide();
            $('#tab26,#CtabNew').show();
            $('#tab26').find("#spnTabName").html(ReportAlertNew);
            $('#tab26').click();
            $("#tab5").removeClass("selected").addClass("unselected");
            $("#tab26").removeClass("unselected").addClass("selected");
        }
    }
    else {
        if (rowobj.ScheduleFor == 55) {
            $('#tab25').show();
            $('#tab25').find("#spnTabName").html(ReportEdit);
            $('#tab24').find("#spnTabName").html(AlertNew);
            $('#tab25').click();
        }
        else if (rowobj.ScheduleFor == 66) {
            $('#tab24').show();
            $('#tab24').find("#spnTabName").html(AlertEdit);
            $('#tab25').find("#spnTabName").html(ReportNew);
            $('#tab24').click();
        }
        else {
            $('#tab24').hide();
            $('#tab25').hide();
            $('#tab26,#CtabNew').show();
            $('#tab26').find("#spnTabName").html(ReportAlertEdit);
            $('#tab26').click();
            $("#tab5").removeClass("selected").addClass("unselected");
            $("#tab26").removeClass("unselected").addClass("selected");
        }
    }

    $(formName).append($('#CtabNew').load(action, function () {
        //$('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus();
        $('#DivLoading').hide();
        setTimeout(function () {
            $("#" + formName).find('input, textarea, select')
            .not('input[type=hidden],input[type=radio],input[type=button],input[type=submit],input[type=reset],input[type=image],button,:disabled')
            .filter(':enabled:visible:first')
            .focus();
        }, 1500);
    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}
function ShowEditTabWithParams(action, formName, params, IsClick) {

    if (typeof (IsClick) === 'undefined') IsClick = true; // change for WI-593
    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    action += '?IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted;
    if (params.length > 0) {
        for (var i = 0; i < params.length; i++) {
            action += "&" + params[i].name + "=" + params[i].value;
        }
    }
    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;

    if (!HasOnlyViewPermission) {
        if (IsArchived || IsDeleted)
            $('#tab1').find("#spnTabName").html(ViewNameRes);
        else
            $('#tab1').find("#spnTabName").html(EditNameRes);
    }
    else {
        $('#tab1').find("#spnTabName").html(ViewNameRes);
    }
    $('#DivLoading').show();
    $('#tab1').show();
    if (IsClick == true)
        $('#tab1').click();
    $(formName).append($('#CtabNew').load(action, function () {
        //$('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus();
        $('#DivLoading').hide();
        setTimeout(function () {
            $("#" + formName).find('input, textarea, select')
            .not('input[type=hidden],input[type=radio],input[type=button],input[type=submit],input[type=reset],input[type=image],button,:disabled')
            .filter(':enabled:visible:first')
            .focus();
        }, 1500);
    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}


function ShowEditTabGUID(action, formName, IsClick) {

    if (typeof (IsClick) === 'undefined') IsClick = true; // change for WI-593
    var IsArchived = false;
    var IsDeleted = false;

    if ($('#IsArchivedRecords').length > 0)
        IsArchived = $('#IsArchivedRecords').is(':checked');

    if ($('#IsDeletedRecords').length > 0)
        IsDeleted = $('#IsDeletedRecords').is(':checked');

    action += '&IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted;
    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;

    if (!HasOnlyViewPermission) {
        if (IsArchived || IsDeleted)
            $('#tab1').find("#spnTabName").html(ViewNameRes);
        else
            $('#tab1').find("#spnTabName").html(EditNameRes);
    }
    else {
        $('#tab1').find("#spnTabName").html(ViewNameRes);
    }
    $('#DivLoading').show();
    $('#tab1').show();
    // change for WI-712
    if (IsClick == true)
        $('#tab1').click();
    $(formName).append($('#CtabNew').load(action, function () {
        $('#DivLoading').hide();
        setTimeout(function () {            
            $("#" + formName).find('input, textarea, select')
       .not('input[type=hidden],input[type=radio],input[type=button],input[type=submit],input[type=reset],input[type=image],button,:disabled')
       .filter(':enabled:visible:first')
       .focus();
        }, 1000);

        //$("#" + formName + " :input:text:visible:first").focus();
    }));

    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}

function DisplayNewTabCommon(actionname, formname) {
    setTimeout(function () {
        ShowNewTab(actionname, formname);
        if ($('#tab1').find("#spnTabName") !== undefined) {
            $('#tab1').find("#spnTabName").html("New");
        }
    }, 1000);
}
function ShowEditTabGUIDTRUEOnly(action, formName) {

    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    action += '&IsArchived=' + false + '&IsDeleted=' + false;
    IsEditMode = true;
    IsShowHistory = true;
    //IsRefreshGrid = true;
    AllowDeletePopup = false;

    if (!HasOnlyViewPermission) {
        if (IsArchived || IsDeleted)
            $('#tab1').find("#spnTabName").html(ViewNameRes);
        else
            $('#tab1').find("#spnTabName").html(EditNameRes);
    }
    else {
        $('#tab1').find("#spnTabName").html(ViewNameRes);
    }
    $('#tab1').show();
    //$('#tab1').click();  
    $(formName).append($('#CtabNew').load(action, function () {

        //$('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus();
        $('#DivLoading').hide();
        setTimeout(function () {
            $("#" + formName).find('input, textarea, select')
            .not('input[type=hidden],input[type=radio],input[type=button],input[type=submit],input[type=reset],input[type=image],button,:disabled')
            .filter(':enabled:visible:first')
            .focus();
        }, 1500);

    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}

function ShowViewTab(action, formName) {
    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    var IsHistory = true;
    action += '?IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted + '&IsHistory=' + IsHistory;
    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;
    $('#tab1').find("#spnTabName").html(ViewNameRes);
    $('#DivLoading').show();
    $('#tab1').show();
    $('#tab1').click();
    $(formName).append($('#CtabNew').load(action, function () {
        //$('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus();
        $('#DivLoading').hide();
        setTimeout(function () {
            $("#" + formName).find('input, textarea, select')
            .not('input[type=hidden],input[type=radio],input[type=button],input[type=submit],input[type=reset],input[type=image],button,:disabled')
            .filter(':enabled:visible:first')
            .focus();
        }, 1500);
    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}

function ShowViewTabForHistory(action, formName) {
    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    var IsChangeLog = true;
    action += '?IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted + '&IsChangeLog=' + IsChangeLog;
    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;
    $('#tab1').find("#spnTabName").html(ViewNameRes);
    $('#DivLoading').show();
    $('#tab1').show();
    $('#tab1').click();
    $(formName).append($('#CtabNew').load(action, function () {
        //$('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus();
        $('#DivLoading').hide();
        setTimeout(function () {
            $("#" + formName).find('input, textarea, select')
            .not('input[type=hidden],input[type=radio],input[type=button],input[type=submit],input[type=reset],input[type=image],button,:disabled')
            .filter(':enabled:visible:first')
            .focus();
        }, 1500);
    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}

function ShowViewTabGUID(action, formName) {
    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    var IsHistory = true;
    action += '?IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted + '&IsHistory=' + IsHistory;
    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;
    $('#tab1').find("#spnTabName").html(ViewNameRes);
    $('#DivLoading').show();
    $('#tab1').show();
    $('#tab1').click();
    $(formName).append($('#CtabNew').load(action, function () {

        //$('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus();
        $('#DivLoading').hide();
        setTimeout(function () {
            $("#" + formName).find('input, textarea, select')
            .not('input[type=hidden],input[type=radio],input[type=button],input[type=submit],input[type=reset],input[type=image],button,:disabled')
            .filter(':enabled:visible:first')
            .focus();
        }, 1500);
    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}

function SwitchTextTab(tabid, action, frmName) {
    //ClearGridStateChange();
    isDirtyForm = false;
    //check for any input field value change and message show to user before navigate
    if (!dirtyCheck()) {
        return false;
    }
    else {
        removeDirtyclass();
    }
    //End navigate
    if (HasInsertPermission == "False") {
        $('#tab1').hide();
    }
    $('#tab1').find("#spnTabName").html(NewNameRes);
    if (tabid == 0) // go to list, you are on new
    {
        AllowDeletePopup = true;
        IsNarrowSearchRefreshRequired = true;
        IsEditMode = false;
        $("#tab5").click();
        $('#CtabNew').hide();
        if (IsRefreshGrid) {
            IsRefreshGrid = false;
            oTable.fnDraw();
        }
        $('#tab6').hide();
    }
    else {// go to new, you are on list        
        AllowDeletePopup = false;
        $("#tab1").click();
    }
}
function TabEnableDisable(TabName, Status) {
    $(TabName).attr("IsEnable", Status);
}


function SetTabswithCount(tablename, fieldname) {
    $.ajax({
        url: '/Master/GetTabStatusWithCount',
        data: { TableName: tablename, StatusFieldName: fieldname },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {

            //Remove old status
            for (var i = 0; i < TabsArry.length; i++) {
                if (typeof TabsArry[i].IsNotification != "undefined") {
                    if (typeof $('#' + TabsArry[i].RedCircleID) != 'undefined') {
                        $('#' + TabsArry[i].RedCircleID).hide();
                    }
                }
            }

            //Add new Status Count
            for (var i = 0; i <= response.Result.length - 1; i++) {
                if (response.Result[i].Count > 0) {
                    if (typeof $('#div' + response.Result[i].Text)[0] != "undefined") {
                        $('#div' + response.Result[i].Text).show();
                        $('#div' + response.Result[i].Text)[0].innerHTML = response.Result[i].Count;
                    }
                }
                else {
                    if (typeof $('#div' + response.Result[i].Text)[0] != 'undefined') {
                        $('#div' + response.Result[i].Text).hide();
                    }
                }
            }
        },
        error: function (error) {
            //alert(error);
            console.log(error);
        }
    });
}

function fnGetSelected(oTableLocal) {
    return oTableLocal.$('tr.row_selected');
}

function LoadPartialView(divID, action) {
    $("#" + divID).load(action, function () { $('#DivLoading').hide(); $("#" + divID + " :input:text:visible:first").focus(); })
    $("#" + divID).show();
}

