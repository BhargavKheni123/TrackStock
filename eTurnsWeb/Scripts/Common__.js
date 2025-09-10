var RefreshTime;
var UserCreatedNarroValues;
var UserUpdatedNarroValues;
var IsEditMode = false;
var IsShowView = false;
var IsShowHistory = false;
var UserUDF1NarrowValues;
var UserUDF2NarrowValues;
var UserUDF3NarrowValues;
var UserUDF4NarrowValues;
var UserUDF5NarrowValues;

var HasInsertPermission = true;
var HasOnlyViewPermission = false;

var PullSupplierNarroValues;
var ManufacturerNarroValues;
var PullCategoryNarroValues;
var StageLocationNarroValues;

var OrderSupplierNarroValues;
var WorkOrderNarroValues;
var OrderNumberNarroValues;
var OrderStatusNarroValues;
var OrderRequiredDateNarroValues;

var HasOrderTab = false;
var HasRequisitionTab = false;
var HasScheduleTab = false;

var SchedulerItemSearchValue = '';
var SchedulerTypeSearchValue = '';

var CostNarroSearchValue = '';
var UserTypeNarroValues = "";
var spendPerSpendLimit = '';
var TotalSpendLimit = '';
var TotalSpendRemaining = '';
var TotalItemSpendLimit = '';

var MatStagLocationsNarroValues = '';
var CartSupplierNarroValues = '';
var CartRTNarroValues = '';
var IsDeleteItemPictureViewRecord = false;

var SSNarroSearchValue = '';

var ItemTypeNarroSearchValue = '';
var FilterStringGlobalUse = '';

var RequisitionTabFilter = '';
var RequisitionCurrentTab = '';

var LocationValue;
var WorkOrderValue;
var ToolCategoryValue;
var AssetCategoryValue;
var ToolMaintanceValue;
var ToolStatusValue;
var ToolCostValue;

var WOTechnicianValues;
var WOCustomerValues;
var WOAssetValues;
var WOToolValues;
var UserRoomNarroValues = '';
var UserRoleNarroValues = '';
var UserCompanyNarroValues = '';
var RoleCompanyNarroValues = '';
var RoleRoomNarroValues = '';
var RoleUserTypeNarroValues = '';
var ICountTypeNarroValues = '';
var ICountStatusNarroValues = '';
var IsColumnResizeFistTimeForControls = true;

var isDirtyForm = false;

var AllowDeletePopupPSInPull = false;

var IsNarrowSearchRefreshRequired = false;

eraseCookieforshift("selectstartindex");
eraseCookieforshift("selectendindex");
eraseCookieforshift("finalselectedarray");



function GetDateInFormat(obj, val) {
    if (val == null || val == '')
        return '';
    var dx = new Date(parseInt(val.substr(6)));
    var dd = dx.getDate();
    var mm = dx.getMonth() + 1;
    var yy = dx.getFullYear();

    if (dd <= 9) {
        dd = "0" + dd;
    }
    if (mm <= 9) {
        mm = "0" + mm;
    }
    return dd + "/" + mm + "/" + yy;
}
function CheckDateBYCompanyConfig(obj) {
    var hdn = $("#hdDateFormat").val();
    var reg = /^([1-9]|0[1-9]|1[0-2])[\/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[\/](1[9][0-9][0-9]|2[0][0-9][0-9]|[0-9][0-9])$/g;
    //var reg1 = /^([1-9]|0[1-9]|1[0-2])[\/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])[\/]([0-9][0-9]|[0-9][0-9])$/g;
    if (hdn == "yy/m/d") {
        reg = /^([0-9][0-9]|1[9][0-9][0-9]|2[0][0-9][0-9])[\/]([1-9]|0[1-9]|1[0-2])[\/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$/g;
        //reg1 = /^([0-9][0-9]|[0-9][0-9])[\/]([1-9]|0[1-9]|1[0-2])[\/]([1-9]|0[1-9]|1[0-9]|2[0-9]|3[0-1])$/g;
    }



    if (reg.test(obj)) {
        return "valid";
    }
    else {
        return "invalid-" + hdn;
    }
}
function GetDateInYYYYMMDDFormat(val) {


    var varHDDateForamt = $("#hdDateFormat").val();

    if (val == null || val == '')
        return '';

    if (varHDDateForamt == "m/d/yy") {

        // val comes in mm/dd/yyyy format    
        var datePart = val.match(/\d+/g),
        year = datePart[2], // get only two digits
        month = datePart[0],
        day = datePart[1];
        return year + '/' + month + '/' + day;
    }
    else if (varHDDateForamt == "d/m/yy") {
        var datePart = val.match(/\d+/g),
        year = datePart[2], // get only two digits
        month = datePart[1],
        day = datePart[0];
        return year + '/' + month + '/' + day;
    }
    else if (varHDDateForamt == "yy/m/d") {
        var datePart = val.match(/\d+/g),
        year = datePart[0], // get only two digits
        month = datePart[1],
        day = datePart[2];
        return year + '/' + month + '/' + day;
    }
    else
        return "";

}



function GetDateInFullFormat(obj) {

    if (obj == null || obj == '')
        return '';
    var todayDate = new Date(parseInt(obj.substr(6)));
    var date = todayDate.getDate();
    var month = todayDate.getMonth() + 1;
    var year = todayDate.getFullYear();
    var hours = todayDate.getHours();
    var minutes = todayDate.getMinutes();
    var seconds = todayDate.getSeconds();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    var hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;
    strTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;

    var varHDDateForamt = $("#hdDateFormat").val();
    var curdat = "";
    if (varHDDateForamt == "m/d/yy") {
        curdat = month + "/" + date + "/" + year + " " + strTime;
    }
    else if (varHDDateForamt == "d/m/yy") {
        curdat = date + "/" + month + "/" + year + " " + strTime;
    }
    else if (varHDDateForamt == "yy/m/d") {
        curdat = year + "/" + month + "/" + date + " " + strTime;
    }
    else
        curdat = month + "/" + date + "/" + year + " " + strTime;

    return curdat;
}

function GetDateInShortFormat(obj) {
    if (obj == null || obj == '')
        return '';
    var todayDate = new Date(parseInt(obj.substr(6)));
    var date = todayDate.getDate();
    var month = todayDate.getMonth() + 1;
    var year = todayDate.getFullYear();
    var hours = todayDate.getHours();
    var minutes = todayDate.getMinutes();
    var seconds = todayDate.getSeconds();
    var varHDDateForamt = $("#hdDateFormat").val();
    var curdat = "";
    if (varHDDateForamt == "m/d/yy") {
        curdat = month + "/" + date + "/" + year
    }
    else if (varHDDateForamt == "d/m/yy") {
        curdat = date + "/" + month + "/" + year
    }
    else if (varHDDateForamt == "yy/m/d") {
        curdat = year + "/" + month + "/" + date
    }
    else
        curdat = month + "/" + date + "/" + year

    return curdat;
}

function AddDaystoDate(obj, Days) { // obj = required date
    var d1 = GetDateInFullFormatCustomizedOnlyDate(obj);
    var RequestedDate = new Date(d1);
    var nextDate = new Date(RequestedDate.getFullYear(), RequestedDate.getMonth(), RequestedDate.getDate() + parseInt(Days));
    return nextDate;
}

function GetTodayDateInFormat() {
    var varHDDateForamt = $("#hdDateFormat").val();
    var todayDate = new Date();
    var date = todayDate.getDate();
    var month = todayDate.getMonth() + 1;
    var year = todayDate.getFullYear();
    var ReqDate = "";
    var curdat = "";
    if (varHDDateForamt == "m/d/yy") {
        curdat = month + "/" + date + "/" + year;
    }
    else if (varHDDateForamt == "d/m/yy") {
        curdat = date + "/" + month + "/" + year;
    }
    else if (varHDDateForamt == "yy/m/d") {
        curdat = year + "/" + month + "/" + date;
    }
    else
        curdat = month + "/" + date + "/" + year;

    var d2 = new Date(curdat);
    return d2;
}

function GetDateInFullFormatCustomized(obj) {
    if (obj == null || obj == '')
        return '';
    var todayDate = new Date(Date.parse(obj)); //new Date(parseInt(obj.substr(10)));
    var date = todayDate.getDate();
    var month = todayDate.getMonth() + 1;
    var year = todayDate.getFullYear();
    var hours = todayDate.getHours();
    var minutes = todayDate.getMinutes();
    var seconds = todayDate.getSeconds();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    var hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;
    strTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;

    var varHDDateForamt = $("#hdDateFormat").val();
    var curdat = "";
    if (varHDDateForamt == "m/d/yy") {
        curdat = month + "/" + date + "/" + year + " " + strTime;
    }
    else if (varHDDateForamt == "d/m/yy") {
        curdat = date + "/" + month + "/" + year + " " + strTime;
    }
    else if (varHDDateForamt == "yy/m/d") {
        curdat = year + "/" + month + "/" + date + " " + strTime;
    }
    else
        curdat = month + "/" + date + "/" + year + " " + strTime;

    return curdat;
}

function GetDateInFullFormatCustomizedOnlyDate(obj) {
    if (obj == null || obj == '')
        return '';
    var todayDate = new Date(Date.parse(obj)); //new Date(parseInt(obj.substr(10)));
    var date = todayDate.getDate();
    var month = todayDate.getMonth() + 1;
    var year = todayDate.getFullYear();
    var hours = todayDate.getHours();
    var minutes = todayDate.getMinutes();
    var seconds = todayDate.getSeconds();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    var hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;
    strTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;

    var varHDDateForamt = $("#hdDateFormat").val();
    var curdat = "";
    if (varHDDateForamt == "m/d/yy") {
        curdat = month + "/" + date + "/" + year;
    }
    else if (varHDDateForamt == "d/m/yy") {
        curdat = date + "/" + month + "/" + year;
    }
    else if (varHDDateForamt == "yy/m/d") {
        curdat = year + "/" + month + "/" + date;
    }
    else
        curdat = month + "/" + date + "/" + year;

    return curdat;
}

function GetDateMMDDYYYYFormat(obj) {
    var todayDate = new Date(parseInt(obj.substr(6)));
    var date = todayDate.getDate();
    var month = todayDate.getMonth() + 1;
    var year = todayDate.getFullYear();
    var hours = todayDate.getHours();
    var minutes = todayDate.getMinutes();
    var seconds = todayDate.getSeconds();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    var hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;
    strTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;
    var curdat = month + "/" + date + "/" + year;  //+ " " + strTime;
    return curdat;
}


function TabItemClicked(action, formName) {
    if (IsEditMode) {
        IsEditMode = false;
        return;
    }
    AllowDeletePopup = false;
    $('#DivLoading').show();
    $(formName).append($('#tab1').load(action, function () { $('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus(); }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}


function TabItemClickedEdit(action, formName) {

    //Passing archived & deleted params while making the request
    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    action += '?IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted;

    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;

    if (action.toLowerCase().indexOf("edit") > 0) {
        if (!HasOnlyViewPermission) {
            $(".tab1").show();
            if (IsArchived || IsDeleted)
                $('.tab1').removeClass("tab1").addClass("tab9");
            else
                $('.tab1').removeClass("tab1").addClass("tab8");
        }
        else {
            $(".tab1").show();
            $('.tab1').removeClass("tab1").addClass("tab9");
        }
    }
    else if (action.toLowerCase().indexOf("view") > 0) {
        $('.tab1').removeClass("tab1").addClass("tab9");
    }

    $("#atab1").click();
    $('#DivLoading').show();
    if (formName == 'frmKitMaster' || formName == 'frmOrder') {
        $(formName).append($('#tab1').load(action, function () { $("#" + formName + " :input:text:visible:first").focus(); }));
    }
    else {
        $(formName).append($('#tab1').load(action, function () { $('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus(); }));
    }
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
    if (action.toLowerCase().indexOf("edit") > 0) {
        if (!HasOnlyViewPermission) {
            $('li.tab8').find('a#atab1').removeAttr("onclick", "");
            $('li.tab8').find('a#atab1').attr("onclick", "TabItemClickedEdit('" + action + "','" + formName + "')");
        }
        else {
            $('li.tab9').find('a#atab1').removeAttr("onclick", "");
            $('li.tab9').find('a#atab1').attr("onclick", "TabItemClickedEdit('" + action + "','" + formName + "')");
        }
    }

}


function TabItemClickedEditMatStag(action, formName) {
    //Passing archived & deleted params while making the request
    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    action += '?IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted;

    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;

    if (action.toLowerCase().indexOf("edit") > 0) {
        if (!HasOnlyViewPermission) {
            $(".tab1").show();
            if (IsArchived || IsDeleted)
                $('.tab1').removeClass("tab1").addClass("tab9");
            else
                $('.tab1').removeClass("tab1").addClass("tab8");
        }
        else {
            $(".tab1").show();
            $('.tab1').removeClass("tab1").addClass("tab9");
        }
    }
    else if (action.toLowerCase().indexOf("view") > 0) {
        $('.tab1').removeClass("tab1").addClass("tab9");
    }

    $("#atab1").click();
    $('#DivLoading').show();
    $('#tab1').html("");
    $(formName).append($('#tab1').load(action, function () {
        $('#DivLoading').hide();
        $("#" + formName + " :input:text:visible:first").focus();



    }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
    if (action.toLowerCase().indexOf("edit") > 0) {
        if (!HasOnlyViewPermission) {
            $('li.tab8').find('a#atab1').removeAttr("onclick", "");
            $('li.tab8').find('a#atab1').attr("onclick", "TabItemClickedEdit('" + action + "','" + formName + "')");
        }
        else {
            $('li.tab9').find('a#atab1').removeAttr("onclick", "");
            $('li.tab9').find('a#atab1').attr("onclick", "TabItemClickedEdit('" + action + "','" + formName + "')");
        }

    }


    //    $(this).find('#txtBinName').autocomplete();
}


function SwitchTabBasedOnPassedTab(tabnameactive) {
    var getDivName = '';
    $('ul.tabs li').each(function () {
        var liObject = $(this).find('a')[0];
        if (liObject.id == tabnameactive) {
            $(this).addClass('active');
            getDivName = tabnameactive.toString().substring(1);
            $("#" + getDivName).show();
        }
        else {
            $(this).removeClass('active');
            getDivName = liObject.id.toString().substring(1);
            $("#" + getDivName).hide();
        }
    });
}



function ChangeEditToNew(action, frmname) {
    $('li.tab8').find('a#atab1').removeAttr("onclick", "");
    $('li.tab8').find('a#atab1').attr("onclick", "TabItemClicked('" + action + "','" + frmname + "')");
    $('li.tab8').removeClass("tab8").addClass("tab1");

    $('li.tab9').find('a#atab1').removeAttr("onclick", "");
    $('li.tab9').find('a#atab1').attr("onclick", "TabItemClicked('" + action + "','" + frmname + "')");
    $('li.tab9').removeClass("tab9").addClass("tab1");

    if (HasInsertPermission == "False") {
        $('li.tab1').hide();
    }
    if (frmname == 'frmUDF') {
        $('li.tab1').hide();
    }
}


$.validator.unobtrusive.parseDynamicContent = function (selector) {
    //use the normal unobstrusive.parse method
    $.validator.unobtrusive.parse(selector);
    //get the relevant form
    var form = $(selector).first().closest('form');
    //get the collections of unobstrusive validators, and jquery validators
    //and compare the two
    var unobtrusiveValidation = form.data('unobtrusiveValidation');
    var validator = form.validate();
}

function SwitchTab(tabid, action, frmName) {
    $('.tab8').removeClass("tab8").addClass("tab1");
    $('.tab9').removeClass("tab9").addClass("tab1");

    if (HasInsertPermission == "False") {
        $('.tab1').hide();
    }

    if (tabid == 0) // go to list, you are on new
    {
        AllowDeletePopup = true;
        $("#atab7").click();
        if (IsRefreshGrid) {
            IsRefreshGrid = false;
            oTable.fnDraw();
        }
    }
    else {// go to new, you are on list

        AllowDeletePopup = false;
        $("#atab1").click();
    }
    $('a#atab1').removeAttr("onclick", "");
    $('a#atab1').attr("onclick", "TabItemClicked('" + action + "','" + frmName + "')");
}


$(document).ready(function () {

    $("#divReorderPopup").dialog({
        autoOpen: false,
        modal: true,
        width: 450,
        title: "Columns",
        draggable: true,
        resizable: true,
        open: function () {
            var Title = $('#divReorderPopup').find('#hdnReOrderTitle').val(); //$(this).data('Title');
            var dataTableName = $('#divReorderPopup').find('#hdnReOrderDataTableName').val(); //$(this).data('dataTableName');
            //var columnObj = $(this).data('objColumns');

            $('#ui-dialog-title-divReorderPopup').text(Title);
            GenerateAndShowGridColumnList(dataTableName);

            $('#divReorderPopup').find('#ulColumnReOrder').sortable({ axis: "y", containment: "parent" });
        },
        close: function () {
            $('#divReorderPopup').find('#hdnReOrderDataTableName').val("");
            $('#divReorderPopup').find('#hdnReOrderTitle').val("");
            $('#divReorderPopup').find('#hdnReOrderListName').val("");
            $('#divReorderPopup').find('#hdnReOrderExecuteFunctionString').val("");
            $('#divReorderPopup li').each(function (index) {
                $(this).remove();
            });
        }
    });

    $('#divLabelPrintPopup').dialog({
        autoOpen: false,
        modal: true,
        width: 380,
        title: "Labels",
        draggable: true,
        resizable: true,
        open: function () {
            $(this).find('#hdnIDs').val($(this).data("IDs"));
            $(this).find('#hdnModuleID').val($(this).data("ModuleID"));
            $(this).find('#hdnSortField').val($(this).data("SortFields"));
        },
        close: function () {
        }
    });


    $(".print").click(function () {
        IsFirstTime = true;
        SelectedRoomIDs = "";
        IsRoomFirstTime = true;
    });

    $("#dvReportParameters").dialog({
        autoOpen: false,
        modal: true,
        width: 500,
        title: "Report",
        title: '@eTurns.DTO.Resources.ResCommon.ReorderColumnPopupHeader',
        draggable: true,
        resizable: true,
        open: function () {
            var dataToPass = $(this).data("data");
            $.ajax({
                type: "POST",
                url: "/Master/ReportParameters",
                contentType: 'application/json',
                dataType: 'json',
                data: dataToPass,
                success: function (retdt) {
                    $("#dvReportParameters").html(retdt);
                },
                error: function (err) {
                    alert("There is some Error");
                }
            });

        },
        close: function () {
            $("#dvReportParameters").html("");
        }
    });

    $("#GlobalReprotBuilder").dialog({
        autoOpen: false,
        modal: true,
        width: 500,
        title: "Reports",
        draggable: false,
        resizable: false,
        open: function () {
            // fill drop down here 
            $('#DivLoading').show();
            GetMultiSelectCompanyDD("ComapnyGlobalReprotBuilder", "ComapnyGlobalReprotBuilderCollapse", "CompanyMaster", "", "Company");
        },
        close: function () {
            if (typeof ($("#ComapnyGlobalReprotBuilder").multiselect("getChecked").length) != undefined && $("#ComapnyGlobalReprotBuilder").multiselect("getChecked").length > 0) {
                $("#ComapnyGlobalReprotBuilder").multiselect("uncheckAll");
            }
            if (typeof ($("#RoomGlobalReprotBuilder").multiselect("getChecked").length) != undefined && $("#RoomGlobalReprotBuilder").multiselect("getChecked").length > 0) {
                $("#RoomGlobalReprotBuilder").multiselect("uncheckAll");
            }
            IsFirstTime = true;
            IsRoomFirstTime = true;
            $("#GlobalReprotBuilder").dialog("close");
        }
    });

    $("#ShowGlobalReprotBuilder").click(function () {
        if ($("#ComapnyGlobalReprotBuilder").multiselect("getChecked").length == 0 || $("#RoomGlobalReprotBuilder").multiselect("getChecked").length == 0) {
            $("#spanGlobalMessage").text("Comapny and Room both are required.");
            showNotificationDialog();
        }
        else if ($("#GlobalReprotBuilder").dialog().data("pgid") == "PullMaster") {

            var StartDate = $("#SDGlobalReprotBuilder")[0].value;
            var EndDate = $("#EDGlobalReprotBuilder")[0].value;
            if (StartDate == "" || EndDate == "") {
                $("#spanGlobalMessage").text("Date range is required.");
                showNotificationDialog();
            }
            else {
                callPrintTransaction("oTable", $("#GlobalReprotBuilder").dialog().data("pgid"), $("#GlobalReprotBuilder").dialog().data("IsSupport"), $("#GlobalReprotBuilder").dialog().data("Ids"), StartDate, EndDate);
                $("#GlobalReprotBuilder").dialog("close");
            }
        }
        else if ($("#GlobalReprotBuilder").dialog().data("pgid") == "ReceiveMasterList") {
            var StartDate = $("#SDGlobalReprotBuilder")[0].value;
            var EndDate = $("#EDGlobalReprotBuilder")[0].value;
            if (StartDate == "" || EndDate == "") {
                $("#spanGlobalMessage").text("Date range is required.");
                showNotificationDialog();
            }
            else {
                callPrintTransaction("oTable", $("#GlobalReprotBuilder").dialog().data("pgid"), $("#GlobalReprotBuilder").dialog().data("IsSupport"), $("#GlobalReprotBuilder").dialog().data("Ids"), StartDate, EndDate);
                $("#GlobalReprotBuilder").dialog("close");
            }
        }
        else if ($("#GlobalReprotBuilder").dialog().data("pgid") === "OrderedItems") {
            var StartDate = $("#SDGlobalReprotBuilder")[0].value;
            var EndDate = $("#EDGlobalReprotBuilder")[0].value;
            if (StartDate == "" || EndDate == "") {
                $("#spanGlobalMessage").text("Date range is required.");
                showNotificationDialog();
            }
            else {
                callPrintTransaction("oTable", $("#GlobalReprotBuilder").dialog().data("pgid"), $("#GlobalReprotBuilder").dialog().data("IsSupport"), $("#GlobalReprotBuilder").dialog().data("Ids"), StartDate, EndDate);
                $("#GlobalReprotBuilder").dialog("close");
            }
        }
        else {
            callPrintTransaction("oTable", $("#GlobalReprotBuilder").dialog().data("pgid"), $("#GlobalReprotBuilder").dialog().data("IsSupport"), $("#GlobalReprotBuilder").dialog().data("Ids"), "", "");
            $("#GlobalReprotBuilder").dialog("close");
        }
    });

    $('ul.tabs').each(function () {
        var $active, $content, $links = $(this).find('a');
        $active = $($links.filter('[href="' + location.hash + '"]')[1] || $links[1]);
        $content = $($active.attr('href'));

        $links.not($active).each(function () {
            $($(this).attr('href')).hide();
        });

        $(this).on('click', 'a', function (e) {
            // Make the old tab inactive.                
            $active.removeClass('active');
            $content.hide();

            $active = $(this);
            $content = $($(this).attr('href'));

            $active.addClass('active');
            $content.show();
            if ($content.selector == "#tab7") {
                AllowDeletePopup = true;
                if (IsRefreshGrid) {
                    IsRefreshGrid = false;
                    oTable.fnDraw();
                }
            }
            ShowHidHistoryTab();
            ShowHideOrderTab();
            e.preventDefault();
        });

    });

    $("#actionSelectAll").click(function () {
        $("#myDataTable").find("tbody tr").removeClass("row_selected").addClass("row_selected");
        //$("#myDataTable").find("tbody tr").toggleClass('row_selected');
        $("#actionDeSelectAll").css('display', '');
        $("#actionSelectAll").css('display', 'none');
        ShowHideCartCreateButton();
    });

    $("#actionDeSelectAll").click(function () {
        $("#myDataTable").find("tbody tr").removeClass("row_selected");
        $("#actionDeSelectAll").css('display', 'none');
        $("#actionSelectAll").css('display', '');
        ShowHideCartCreateButton();
    });

    var lastChecked;
    var starttrvalue = "";
    $("#myDataTable").on("touchstart click", "tbody tr", function (e) {

        //        if (gblActionName === "ReceiveList") {
        //            return;
        //        }
        //        else if (gblActionName == "ItemMasterList") {
        //            return;
        //        }
        //        else
        if ($(e.target).hasClass("control") == true || e.target.nodeName.toLowerCase() == "img" || e.target.type == "checkbox" || e.target.type == "radio" || e.target.type == "text" || e.target.type == "button" || $(e.target).is('a') == true) {
            e.stopPropagation();
        } else {
            if (IsDeleteItemPictureViewRecord)
                $(this).parent().parent().parent().parent().parent().toggleClass('row_selected');
            else {
                //$(this).toggleClass('row_selected');
                if (!lastChecked) {
                    lastChecked = this;
                }

                if (e.shiftKey) {
                    var start = $('#myDataTable tbody tr').index(this);
                    var end = $('#myDataTable tbody tr').index(lastChecked);


                    var stringval1 = readCookieforshift("selectstartindex");
                    if (stringval1 != null) {
                        var endindex = $(this).closest('tr').attr('id');
                        createCookieforshift("selectendindex", endindex, 1);
                        if ($("#hdnPageName").val() !== undefined) {
                            var pagename = '';
                            pagename = $("#hdnPageName").val();
                            GetOnlyIdsForPassPagesForshift(pagename, true);
                        }
                    }

                    for (i = Math.min(start, end); i <= Math.max(start, end); i++) {
                        if (!$('#myDataTable tbody tr').eq(i).hasClass('row_selected')) {
                            $('#myDataTable tbody tr').eq(i).addClass("row_selected");
                        }
                    }
                    if (window.getSelection) {
                        if (window.getSelection().empty) {  // Chrome
                            window.getSelection().empty();
                        } else if (window.getSelection().removeAllRanges) {  // Firefox
                            window.getSelection().removeAllRanges();
                        }
                    } else if (document.selection) {  // IE?
                        document.selection.empty();
                    }
                } else if ((e.metaKey || e.ctrlKey)) {
                    $(this).toggleClass('row_selected');
                } else {
                    $(this).toggleClass('row_selected');


                    if ($(this).hasClass('row_selected')) {
                        (starttrvalue == "") ? starttrvalue = $(this).closest('tr').attr('id') : starttrvalue = starttrvalue + "," + $(this).closest('tr').attr('id');
                        //                        var pageindex = "";
                        //                        $('#myDataTable_paginate span a').each(function () {
                        //                            if ($(this).hasClass('ui-state-disabled')) {
                        //                                pageindex = $(this).text();
                        //                            }
                        //                        });
                        // (pageindex == "") ? starttrvalue = starttrvalue : starttrvalue = starttrvalue + ":" + pageindex;
                        createCookieforshift("selectstartindex", starttrvalue, 1);
                    } else {

                        var stringval = readCookieforshift("selectstartindex");
                        if (stringval != "undefined") {
                            if (stringval != null) {
                                var tmp = stringval.split(',');
                                var index = tmp.indexOf($(this).closest('tr').attr('id'));
                                if (index !== -1) {
                                    tmp.splice(index, 1);
                                    stringval = tmp.join(',');
                                    //eraseCookieforshift("selectstartindex");
                                    //                                var pageindex1 = "";
                                    //                                $('#myDataTable_paginate span a').each(function () {
                                    //                                    if ($(this).hasClass('ui-state-disabled')) {
                                    //                                        pageindex1 = $(this).text();
                                    //                                    }
                                    //                                });
                                    //(pageindex1 == "") ? stringval = stringval : stringval = stringval + ":" + pageindex1;
                                    createCookieforshift("selectstartindex", stringval, 1);
                                }
                            }
                        }

                    }
                }

                lastChecked = this;
            }

            ShowHidHistoryTab();
            ShowHideOrderTab();
            ShowHideChangeLog();
            ShowHideSchedulerTab();
            ShowHideCartCreateButton();
            return false;
        }
    });


    //    $('#myDataTable tbody tr').live('touchstart click', function () {
    //        if (IsDeleteItemPictureViewRecord)
    //            $(this).parent().parent().parent().parent().parent().toggleClass('row_selected');
    //        else
    //            $(this).toggleClass('row_selected');

    //        ShowHidHistoryTab();
    //        ShowHideOrderTab();
    //        ShowHideChangeLog();
    //        return false;
    //    });

    $("table.tbldatach tbody tr").live('touchstart click', function (e) {
        //   debugger;
        if (e.target.type == "checkbox" || e.target.type == "radio" || e.target.type == "text") {
            e.stopPropagation();
        } else {
            $(this).toggleClass('row_selected');
        }
    });
    function ShowHidHistoryTab() {
        // To show or hide the History(change log) tab 
        var anSelectedRows = fnGetSelected(oTable);
        if (anSelectedRows.length == 1)
            $(".tab5").show();
        else {
            if (IsShowHistory) {
                $(".tab5").show();
                IsShowHistory = false;
            }
            else {
                $(".tab5").hide();
            }
        }
    }

    function ShowHideCartCreateButton() {
        var anSelectedRows = fnGetSelected(oTable);
        if ($("#hdnPageName").val() !== undefined) {
            var pagename = '';
            pagename = $("#hdnPageName").val();
        }
        if (pagename == "CartItemList") {
            if (anSelectedRows.length > 0) {
                if ($("#btnCheckout") != undefined) {
                    $("#btnCheckout").removeAttr('disabled', 'disabled');
                    $('#btnCheckout').removeClass('btnGeneraldisabled');
                    $('#btnCheckout').addClass('btnGeneral');
                }
            }
            else {
                if ($("#btnCheckout") != undefined) {
                    $("#btnCheckout").attr('disabled', 'disabled');
                    $('#btnCheckout').removeClass('btnGeneral');
                    $('#btnCheckout').addClass('btnGeneraldisabled');
                }
            }
        }
    }

    function ShowHideChangeLog() {
        // To show or hide the History(change log) tab 
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

    // used for refresh the grid manually...
    //$('#refreshGrid').click(function () {
    $('#refreshGrid').live('click', function () {
        oTable.fnDraw();
        if (gblActionName == 'RequisitionList') {
            SetConsumeRedCount();
        }
        else if (gblActionName = 'CartItemList' || gblActionName == 'OrderList' || gblActionName == 'ReturnOrderList' || gblActionName == 'TransferList' || gblActionName == 'ReceiveList') {
            SetReplenishRedCount();
        }
    });


    $('#deleteRows').click(function () {

        if ($("body").hasClass('DTTT_Print')) {
            return false;
        }

        if (gblActionName.toLowerCase() === 'orderlist' || gblActionName.toLowerCase() == 'TemplateConfigurationList' || gblActionName.toLowerCase() === 'transferlist') {

            return false;
        }
        else if (gblActionName.toLowerCase() == 'requisitionlist') {
            return false;
        }
        else if (gblActionName.toLowerCase() == "quicklist") {
            var anQLSelected = fnGetSelected(oTable);

            var stringIDs = "";
            for (var i = 0; i <= anQLSelected.length - 1; i++) {
                stringIDs = stringIDs + $(anQLSelected).find('#QuickListGUID').text() + ",";
            }

            if (stringIDs.length !== 0) {
                $('#basic-modal-content').modal();
                IsDeletePopupOpen = true;
            }
        }
        else if (gblActionName.toLowerCase() == "materialstaginglist") {
            return false;
        }
        else if (gblActionName.toLowerCase() == "cartitemlist") {
            return false;
        }

        else {
            var anSelected = fnGetSelected(oTable);
            var stringIDs = "";
            for (var i = 0; i <= anSelected.length - 1; i++) {
                if (deleteURL.toLowerCase() == "/pull/deletepullmasterrecords") {
                    var aPos = oTable.fnGetPosition($(anSelected[i])[0]);
                    var aData = oTable.fnGetData(aPos);
                    if (!(!isNaN(parseFloat(aData.ConsignedQuantity)) && parseFloat(aData.ConsignedQuantity) > 0 && aData.Billing == 'Yes')) {
                        stringIDs = stringIDs + aData.ID + ",";
                    }
                }
                else {
                    stringIDs = stringIDs + anSelected[i].id + ",";
                }
            }
            if (stringIDs.length > 0) {
                $('#basic-modal-content').modal();
                IsDeletePopupOpen = true;
            }
        }

    });


    $("#btnModelYes").click(function () {

        var anSelected = fnGetSelected(oTable);
        var stringIDs = "";
        var ReqCount = 0;

        for (var i = 0; i <= anSelected.length - 1; i++) {
            if (gblActionName.toLowerCase() === "quicklist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#QuickListGUID')[0].value + ",";
                ReqCount = ReqCount + 1;
            }
            else if (gblActionName.toLowerCase() === "workorderlist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#WorkOrderGUID')[0].value + ",";
            }
            else if (gblActionName.toLowerCase() === "toollist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#spnToolPKID').text() + ",";
            }
            else if (gblActionName.toLowerCase() == 'requisitionlist') {
                var SpanReqStatus = $(anSelected[i]).find('#spnRequisitionStatus').text();
                if (SpanReqStatus == "Unsubmitted" || SpanReqStatus == "Submittted") {
                    //stringIDs = stringIDs + anSelected[i].id + ",";
                    stringIDs = stringIDs + $(anSelected[i]).find('#RequisitionGUID')[0].value + ",";
                    ReqCount = ReqCount + 1;
                }
            }
            else if (gblActionName.toLowerCase() == "orderlist") {
                var IsDeleteable = $(anSelected[i]).find('#spnIsableToDelete').text();
                if (IsDeleteable == 'true') {
                    var orderID = $(anSelected[i]).find('#spnOrderMasterID').text();
                    stringIDs = stringIDs + orderID + ",";
                    ReqCount = ReqCount + 1;
                }
            }
            else if (gblActionName.toLowerCase() == "transferlist") {
                var IsDeleteable = $(anSelected[i]).find('#spnIsableToDelete').text();
                if (IsDeleteable == 'true') {
                    var masterID = $(anSelected[i]).find('#spnTransferMasterGUID').text();
                    stringIDs = stringIDs + masterID + ",";
                    ReqCount = ReqCount + 1;
                }
            }
            else if (gblActionName.toLowerCase() == "materialstaginglist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#hdnUniqueID').val() + ",";
                ReqCount = ReqCount + 1;
            }
            else if (gblActionName.toLowerCase() == "cartitemlist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#hdnUniqueID').val() + ",";
                ReqCount = ReqCount + 1;
            }
            else if (gblActionName.toLowerCase() == "rolelist") {
                stringIDs = stringIDs + $(anSelected[i]).find("input[name='hdnConcatedId']").val() + ",";
                ReqCount = ReqCount + 1;
            }
            else if (deleteURL.toLowerCase() == "/inventory/deletematerialstagingrecords") {
                stringIDs = stringIDs + $(anSelected[i]).find("input[type='hidden'][name='hdnUniqueID']").val() + ",";
            }
            else if (deleteURL.toLowerCase() == "/pull/deletepullmasterrecords") {
                var aPos = oTable.fnGetPosition($(anSelected[i])[0]);
                var aData = oTable.fnGetData(aPos);
                if (!(!isNaN(parseFloat(aData.ConsignedQuantity)) && parseFloat(aData.ConsignedQuantity) > 0 && aData.Billing == 'Yes')) {
                    stringIDs = stringIDs + $(anSelected[i]).find('#spnPullMasterID').text() + ",";
                    ReqCount = ReqCount + 1;
                }
            }
            else if (deleteURL.toLowerCase() == "/inventory/deleterecords") {
                stringIDs = stringIDs + $(anSelected[i]).find('#ItemGUID')[0].value + ",";
            }
            else if (deleteURL.toLowerCase() == "/bom/deleterecords") {
                stringIDs = stringIDs + $(anSelected[i]).find('#ItemGUID')[0].value + ",";
            }
            else if (deleteURL.toLowerCase() == "/assets/deletemappingrecords") {
                stringIDs = stringIDs + $(anSelected[i]).find('#MappingGUID')[0].value + ",";
            }
            else {
                stringIDs = stringIDs + anSelected[i].id + ",";
            }
        }
        if (anSelected.length !== 0) {
            $.ajax({
                'url': deleteURL,
                data: { ids: stringIDs },
                success: function (response) {
                    //  debugger;
                    if (response.Status == "ok" || response == "ok") {
                        IsNarrowSearchRefreshRequired = true;
                        for (var i = 0; i <= anSelected.length - 1; i++) {
                            if (deleteURL.toLowerCase() == "/pull/deletepullmasterrecords") {
                                var aPos = oTable.fnGetPosition($(anSelected[i])[0]);
                                var aData = oTable.fnGetData(aPos);
                                if (!(!isNaN(parseFloat(aData.ConsignedQuantity)) && parseFloat(aData.ConsignedQuantity) > 0 && aData.Billing == 'Yes')) {
                                    oTable.fnDeleteRow(anSelected[i]);
                                }
                            }
                            else {
                                oTable.fnDeleteRow(anSelected[i]);
                            }
                        }
                        if (anSelected.length > 0) {
                            if (gblActionName == 'PullMasterList' || gblActionName == 'RequisitionList' || gblActionName == 'OrderList' || gblActionName == 'TransferList' || gblActionName == 'MaterialStagingList' || gblActionName == 'CartItemList' || gblActionName == 'QuickList') {
                                $("#spanGlobalMessage").text("Record(" + ReqCount + ") deleted successfully.");
                            }
                            else {
                                $("#spanGlobalMessage").text(response.Message);
                            }

                            ShowHidHistoryTab();
                            ShowHideChangeLog();
                            if (gblActionName == 'RequisitionList') {
                                SetTabswithCount('RequisitionMaster', 'RequisitionStatus');
                                UpdateTopMenuReqCount();
                            }
                            else if (gblActionName == 'ItemMasterList') {
                                CallNarrowfunctions();
                            }
                            else if (gblActionName.toLowerCase() === "quicklist") {
                                fillQLNarrowSearchDiv();
                            }
                            else if (gblActionName.toLowerCase() === "orderlist") {
                                fillOrderNarrowSearchDiv();
                                SetReplenishRedCount();

                            }
                            else if (gblActionName.toLowerCase() === "transferlist") {
                                fillTransferNarrowSearchDiv();
                            }
                            else if (gblActionName.toLowerCase() === "roomlist" || gblActionName.toLowerCase() === "companylist") {
                                window.location.reload();
                            }

                        }
                        showNotificationDialog();
                    }
                    else {

                        if (response == '') {
                            $("#spanGlobalMessage").text("An error has occured while processing your request, please try later.");
                        }
                        else {
                            $("#spanGlobalMessage").text("You can not delete " + response + " field as it is being used by other module.");
                        }

                        ShowHidHistoryTab();
                        ShowHideChangeLog();

                        if (gblActionName == 'ToolList') {
                            oTable.fnDraw();
                        }


                        showNotificationDialog();
                    }
                },
                error: function (response) {
                    // through errror message
                }
            });
            $.modal.impl.close();
            IsDeletePopupOpen = false;
        }
    });

    $('#PageNumber').keydown(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) {
            $(".go").click();
            return false;
        }
    });

    $(".go").click(function () {
        var pval = $('#PageNumber').val();
        if (pval == "" || pval.match(/[^0-9]/)) {
            return;
        }
        if (pval == 0)
            return;
        oTable.fnPageChange(Number(pval - 1));
        $('#PageNumber').val('');
    });

    //    $(".columnsetup").click(function () {
    //        $(".refreshBlock").toggle();
    //    });

    function fnGetSelected(oTableLocal) {
        return oTableLocal.$('tr.row_selected');
    }

    function fnFilterGlobal() {
        //set filter only if more than 2 characters are pressed
        if (typeof $("#global_filter") != 'undefined' && ($("#global_filter").val().length > 2 || $("#global_filter").val().length == 0)) {
            clearNarrowSearchFilter();
            var searchtext = $("#global_filter").val().replace(/'/g, "''");

            // Need to add below code for Requisition Tab Filter 
            if (RequisitionTabFilter != '') {
                if (RequisitionTabFilter.indexOf("[####]") >= 0)
                    searchtext = searchtext + RequisitionTabFilter;
                else
                    searchtext = searchtext + "[####]" + RequisitionTabFilter;
            }
            /////////////////////////////////////////////////////

            $('#myDataTable').dataTable().fnFilter(
                            searchtext,
                            null,
                            null,
                            null
                        );
            $('.tab5').hide();
            //TODO: for Barcode
            //$("#global_filter").select();


        }
        else {
            $('#myDataTable td').removeHighlight();
            $('#myDataTable td').highlight($("#global_filter").val());
        }

        HideOtherTabs();
    }

    var NotAllowedCharCodes = [9, 16, 17, 18, 19, 20, 27, 33, 34, 35, 36, 37, 38, 39, 40];

    //Apply filter
    //$("#global_filter").keyup(fnFilterGlobal);
    var timeoutsc1;
    $('#global_filter').bind('textchange', function () {
        clearTimeout(timeoutsc1);
        var self = this;
        timeoutsc1 = setTimeout(function () {
            fnFilterGlobal();
        }, 200);
    });
    $("#global_filter").keydown(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) {

            var searchtext = $("#global_filter").val().replace(/'/g, "''");

            // Need to add below code for Requisition Tab Filter 
            if (RequisitionTabFilter != '') {
                if (RequisitionTabFilter.indexOf("[####]") >= 0)
                    searchtext = searchtext + RequisitionTabFilter;
                else
                    searchtext = searchtext + "[####]" + RequisitionTabFilter;
            }
            /////////////////////////////////////////////////////

            $('#myDataTable').dataTable().fnFilter(searchtext, null, null, null);

            HideOtherTabs();
            //TODO: for Barcode
            //            if (gblActionName === "ItemMasterList" || gblActionName === "KitList" || gblActionName === "ProjectList" || gblActionName === "QuickList" || gblActionName === "RequisitionList") {

            //                if ($('#myDataTable').dataTable().fnGetData().length === 1) {
            //                    $('#global_filter').val('');
            //                    $('#myDataTable tr').find('#aEditLink').click();
            //                }
            //            }
            //            else if (gblActionName === "MaterialStagingList") {
            //                if ($('#myDataTable').dataTable().fnGetData().length === 1) {
            //                    $('#myDataTable tr').find('.aEditLink').click();
            //                }
            //            }
            return false;
        }
    });

    //TODO: for Barcode
    $("#btnBarcodeAddYes").click(function () {
        var newBarcodeText = '';
        newBarcodeText = $('#global_filter').val();
        var strModule = '';
        if (gblActionName === "ItemMasterList") {
            strModule = 'Item Master';
        }
        else if (gblActionName === "KitList") {
            strModule = 'Kits';
        }
        else if (gblActionName === "OrderList") {
            newBarcodeText = $('#txtOrderFilter').val();
            strModule = 'Orders';
        }
        else if (gblActionName === "ProjectList") {
            strModule = 'Project spent';
        }
        else if (gblActionName === "QuickList") {
            strModule = 'Quick List permission';
        }

        else if (gblActionName === "RequisitionList") {
            strModule = 'Requisitions';
        }
        else if (gblActionName === "MaterialStagingList") {
            strModule = 'Material Staging';
        }
        else if (gblActionName === "QuickList") {
            strModule = 'Quick List permission';
        }
        else if (gblActionName === "AssetList") {
            strModule = 'Assets';
        }
        else if (gblActionName === "ToolList") {
            strModule = 'Tool Master';
        }
        else if (gblActionName === "TransferList") {
            newBarcodeText = $('#txtTransferFilter').val();
            strModule = 'Transfer';
        }
        else if (gblActionName === "WorkOrderList") {
            strModule = 'Work Orders';
        }
        $.modal.impl.close();
        $("#divViewBarcodeModel").data({ "strModule": strModule, "barcodetext": newBarcodeText }).dialog('open');
    });


    //    $("#global_filter").keyup(function (e) {
    //        var code = (e.keyCode ? e.keyCode : e.which);
    //        var index = $.inArray(code, NotAllowedCharCodes);
    //        if (index >= 0) return false;
    //        if (code == 13) {
    //        }
    //        else {
    //            fnFilterGlobal();
    //        }
    //    });

    //    //Keydown event is required to handle ENTER KEY to work in IE
    //    $("#global_filter").keydown(function (e) {
    //        var code = (e.keyCode ? e.keyCode : e.which);
    //        if (code == 13) {
    //            var searchtext = $("#global_filter").val().replace(/'/g, "''");

    //            // Need to add below code for Requisition Tab Filter 
    //            if (RequisitionTabFilter != '') {
    //                if (RequisitionTabFilter.indexOf("[####]") >= 0)
    //                    searchtext = searchtext + RequisitionTabFilter;
    //                else
    //                    searchtext = searchtext + "[####]" + RequisitionTabFilter;
    //            }
    //            /////////////////////////////////////////////////////

    //            $('#myDataTable').dataTable().fnFilter(
    //                            searchtext,
    //                            null,
    //                            null,
    //                            null
    //                        );
    //            return false;
    //        }
    //    });

    //Clear Filter
    $("#clear_global_filter").click(function () {
        if ($("#global_filter").val().length > 0) {
            $("#global_filter").val("");
            $("#global_filter").change();


            //            $("input[type='text'][id='global_filter']").get(0).val("");
            //            //            $("#global_filter").get(0).val("");
            //            $("input[type='text'][id='global_filter']").get(0).trigger("textchange");
            //            //            $('#global_filter').get(0).trigger("textchange");
        }
        return false;
    });

    function funClearFilter() {
        //Check length first        
        if ($("#global_filter").val().length > 0) {
            $("#global_filter").val('');
            $('#myDataTable').dataTable().fnFilter(
                            $("#global_filter").val(),
                            null,
                            null,
                            null
                        );
        }
        $("#global_filter").focus();
        return false;
    }


    $.ajax({
        type: "GET",
        url: "/Master/GlobalUISettingsForGrid",
        data: { ID: 1, SearchType: 'user' },
        success: function (response) {
            RefreshTime = response;
            //setTimeout("RefreshGrid();", RefreshTime); // milliseconds
            setInterval("RefreshGrid();", RefreshTime); // milliseconds
        }
    });

});
function RefreshGrid() {
    var RefreshObject = $('#GridAutoRefresh');
    if (RefreshObject.is(':checked')) {
        //The code is written only for the list/history pages to get refreshed
        if ($("#tab5").hasClass('selected')) {
            oTable.fnDraw();
        }
    }
}


jQuery.fn.dataTableExt.oSort['string-case-asc'] = function (x, y) {
    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
};

jQuery.fn.dataTableExt.oSort['string-case-desc'] = function (x, y) {
    return ((x < y) ? 1 : ((x > y) ? -1 : 0));
};


jQuery.fn.highlight = function (pat) {
    function innerHighlight(node, pat) {
        var skip = 0;
        if (node.nodeType == 3) {
            var pos = node.data.toUpperCase().indexOf(pat);
            if (pos >= 0) {
                var spannode = document.createElement('span');
                spannode.className = 'highlight';
                var middlebit = node.splitText(pos);
                var endbit = middlebit.splitText(pat.length);
                var middleclone = middlebit.cloneNode(true);
                spannode.appendChild(middleclone);
                middlebit.parentNode.replaceChild(spannode, middlebit);
                skip = 1;
            }
        }
        else if (node.nodeType == 1 && node.childNodes && !/(script|style)/i.test(node.tagName)) {
            for (var i = 0; i < node.childNodes.length; ++i) {
                i += innerHighlight(node.childNodes[i], pat);
            }
        }
        return skip;
    }
    return this.each(function () {
        innerHighlight(this, pat.toUpperCase());
    });
};

jQuery.fn.removeHighlight = function () {
    function newNormalize(node) {
        for (var i = 0, children = node.childNodes, nodeCount = children.length; i < nodeCount; i++) {
            var child = children[i];
            if (child.nodeType == 1) {
                newNormalize(child);
                continue;
            }
            if (child.nodeType != 3) { continue; }
            var next = child.nextSibling;
            if (next == null || next.nodeType != 3) { continue; }
            var combined_text = child.nodeValue + next.nodeValue;
            new_node = node.ownerDocument.createTextNode(combined_text);
            node.insertBefore(new_node, child);
            node.removeChild(child);
            node.removeChild(next);
            i--;
            nodeCount--;
        }
    }

    return this.find("span.highlight").each(function () {
        var thisParent = this.parentNode;
        thisParent.replaceChild(this.firstChild, this);
        newNormalize(thisParent);
    }).end();
};


//function callPrint(DataTableName) {
//    //declare the oConfig variable
//    var oConfig = {
//        "sInfo": "<h6>Print view</h6><p>Please use your browser's print function to " +
//		  "print this table. Press escape when finished.",
//        "sMessage": null,
//        "bShowAll": false,
//        "sToolTip": "View print view",
//        "sButtonClass": "DTTT_button_print",
//        "sButtonText": "Print"
//    };

//    //Get the current instance and intialize the print
//    if (typeof (TableTools) != undefined && typeof (TableTools) != 'undefined')
//        TableTools.fnGetInstance(DataTableName).fnPrint(true, oConfig);
//}

function ShowGlobalReprotBuilder(PageNameForReport, IsSupportingInfo) {

    // Here needs to get Selected rows if seleted any else get all Data for print ...
    var anSelected = fnGetSelected(oTable);
    var stringIDs = "";
    if (PageNameForReport.toLowerCase() == "pullmaster") {
        stringIDs = "";
        $("#GlobalReprotBuilder").data("pgid", PageNameForReport).data("IsSupport", IsSupportingInfo).data("Ids", stringIDs).dialog("open");
    }
    if (PageNameForReport.toLowerCase() == "receivemasterlist" || PageNameForReport.toLowerCase() == "ordereditems") {
        stringIDs = "";
        $("#GlobalReprotBuilder").data("pgid", PageNameForReport).data("IsSupport", IsSupportingInfo).data("Ids", stringIDs).dialog("open");
    }
    else if (anSelected.length == 0) {
        GetOnlyIdsForPassPages(PageNameForReport, IsSupportingInfo);
    }
    else {
        for (var i = 0; i <= anSelected.length - 1; i++) {
            stringIDs = stringIDs + anSelected[i].id + ",";
        }
        $("#GlobalReprotBuilder").data("pgid", PageNameForReport).data("IsSupport", IsSupportingInfo).data("Ids", stringIDs).dialog("open");
    }
}

function GetOnlyIdsForPassPages(PageName, IsSupportPage) {

    var o = "";
    var sSource = "";
    var lowername = PageName.toLowerCase();
    switch (lowername) {
        case "requisitionmaster":
            sSource = "RequisitionMasterListAjax";
            break;
        case "workorder":
            sSource = "WOMasterListAjax";
            break;
        case "ordermasterlist":
            sSource = "OrderMasterListAjax";
            break;
        case "binmasterlist":
            sSource = "GetBinList";
            break;
        case "categorymasterlist":
            sSource = "CategoryListAjax";
            break;
        case "customermasterlist":
            sSource = "CustomerMasterListAjax";
            break;
        case "freighttypemasterlist":
            sSource = "DataProviderForFrieghtTypeGrid";
            break;
        case "glaccountmasterlist":
            sSource = "GetGLAccountList";
            break;
        case "locationmasterlist":
            sSource = "GetLocationList";
            break;
        case "manufacturermasterlist":
            sSource = "ManufacturerListAjax";
            break;
        case "measurementtermlist":
            sSource = "GetMeasurementTermList";
            break;
        case "shipviamasterlist":
            sSource = "ShipViaListAjax";
            break;
        case "suppliermasterlist":
            sSource = "GetSupplierList";
            break;
        case "technicianlist":
            sSource = "TechnicianListAjax";
            break;
        case "toolcategorylist":
            sSource = "ToolCategoryListAjax";
            break;
        case "assetcategorylist":
            sSource = "AssetCategoryListAjax";
            break;
        case "unitmasterlist":
            sSource = "GetUnitList";
            break;
        case "itemmasterlist":
            sSource = "ItemMasterListAjax";
            break;
        case "costuommasterlist":
            sSource = "CostUOMListAjax";
            break;
        case "inventoryclassificationmasterlist":
            sSource = "InventoryClassificationListAjax";
            break;
        case "inventorylocationslist":
            sSource = "InventoryLocationsAjax";
            break;
        case "materialstaging":
            sSource = "MaterialStagingListAjax";
            break;
    }


    var StartiDisplayLength = oTable.fnSettings()._iDisplayStart;
    var EndDisplayLength = oTable.fnSettings()._iDisplayLength;

    ///////////////////////////////// Call to get Ids /////////////////////////// START
    oTable.fnSettings().oFeatures.bStateSave = false;
    oTable.fnSettings()._iDisplayStart = 0;
    oTable.fnSettings()._iDisplayLength = 9999999;

    var aoData = oTable._fnAjaxParameters();
    var arrCols = new Array();
    var objCols = oTable.fnSettings().aoColumns;
    for (var i = 0; i <= objCols.length - 1; i++) {
        arrCols.push(objCols[i].mDataProp);
    }
    for (var j = 0; j <= aoData.length - 1; j++) {
        if (aoData[j].name == "sColumns") {
            aoData[j].value = arrCols.join("|");
            break;
        }
    }
    if (oTable.fnSettings().aaSorting.length != 0)
        aoData.push({ "name": "SortingField", "value": oTable.fnSettings().aaSorting[0][3] });
    else
        aoData.push({ "name": "SortingField", "value": "0" });

    aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
    aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });

    $.ajax({
        "dataType": 'json',
        "type": "POST",
        "url": sSource,
        cache: false,
        "async": false,
        "data": aoData,
        "success": function (json) {
            if (json.aaData != '') {
                for (var i = 0; i <= json.aaData.length - 1; i++) {
                    o = o + json.aaData[i].ID + ",";
                }
            }
            $("#GlobalReprotBuilder").data("pgid", PageName).data("IsSupport", IsSupportPage).data("Ids", o).dialog("open");
            //return o;
        },
        error: function (response) {

            //return o;
        }
    })
    ///////////////////////////////// Call to get Ids /////////////////////////// END

    ///////////////////////////////// Set Original settings /////////////////////////// START
    oTable.fnSettings().oFeatures.bStateSave = true;
    oTable.fnSettings()._iDisplayStart = StartiDisplayLength;
    oTable.fnSettings()._iDisplayLength = EndDisplayLength;
    ///////////////////////////////// Set Original settings /////////////////////////// END
}

function callPrint(DataTableName, columnsetupfor, IsSupportingInfo) {

    var lowername = columnsetupfor.toLowerCase();
    if (IsSupportingInfo)
        var URLToOpen = location.protocol + "//" + location.host + "/Reports/" + escape("Supporting Information") + "/";
    else
        var URLToOpen = location.protocol + "//" + location.host + "/Reports/" + escape("Transaction") + "/";

    switch (lowername) {
        case "binmasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "BinLocation");
            break;
        case "categorymasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "Category");
            break;
        case "customermasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "Customers");
            break;
        case "freighttypemasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "FreightTypes");
            break;
        case "glaccountmasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "GLAccounts");
            break;
        case "locationmasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "Locations");
            break;
        case "manufacturermasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "Manufacturers");
            break;
        case "measurementtermlist":
            OpenPopup(columnsetupfor, URLToOpen + "MeasurementTerms");
            break;
        case "shipviamasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "ShipVia");
            break;
        case "suppliermasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "Supplier");
            break;
        case "technicianlist":
            OpenPopup(columnsetupfor, URLToOpen + "Technicians");
            break;
        case "toolcategorylist":
            OpenPopup(columnsetupfor, URLToOpen + "ToolCategory");
            break;
        case "assetcategorylist":
            OpenPopup(columnsetupfor, URLToOpen + "AssetCategory");
            break;
        case "unitmasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "Units");
            break;
        case "itemmasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "Items");
            break;
        case "costuommasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "CostUOM1");
            break;
        case "inventoryclassificationmasterlist":
            OpenPopup(columnsetupfor, URLToOpen + "InventoryClassificationMaster");
            break;
        case "inventorylocationslist":
            OpenPopup(columnsetupfor, URLToOpen + "InventoryLocationsMaster");
            break;

    }
}


function callPrintTransaction(DataTableName, columnsetupfor, IsSupportingInfo, IDs, StartDate, EndDate) {

    var lowername = columnsetupfor.toLowerCase();
    if (IsSupportingInfo)
        var URLToOpen = location.protocol + "//" + location.host + "/Reports/" + escape("Supporting Information") + "/";
    else
        var URLToOpen = location.protocol + "//" + location.host + "/Reports/" + escape("Transaction") + "/";

    var CurrentoTableCols = $(oTable).DataTable.settings[0].aoColumns;
    var CurrentoTableColsList = "";

    if (lowername != "requisitionmaster" && lowername != "ordermasterlist" && lowername != "workorder" && lowername != "pullmaster" && lowername != "receivemasterlist" && lowername != "ordereditems") {
        for (var i = 0; i <= CurrentoTableCols.length - 1; i++) {
            if (CurrentoTableCols[i].bVisible && CurrentoTableCols[i].mDataProp != null) {
                CurrentoTableColsList += CurrentoTableCols[i].mDataProp + ":" + CurrentoTableCols[i].sTitle.trim() + ",";
            }
        }
    }

    var CompanyRprtIds = "";
    var RoomRprtIds = "";
    $("#ComapnyGlobalReprotBuilder").multiselect("getChecked").each(function () {
        CompanyRprtIds += this.value + ",";
    });
    $("#RoomGlobalReprotBuilder").multiselect("getChecked").each(function () {
        RoomRprtIds += this.value + ",";
    });

    $.ajax({
        "dataType": 'json',
        "type": "POST",
        "url": "/Master/AddReportParamsToSesstion",
        cache: false,
        "async": false,
        data: { Ids: IDs, Title: $("#textGlobalReprotBuilder")[0].value, StartDate: StartDate, EndDate: EndDate, DisplayFields: CurrentoTableColsList, CompanyIds: CompanyRprtIds, RoomIds: RoomRprtIds, BarcodeColumn: "" },
        "success": function (json) {
            switch (lowername) {
                case "requisitionmaster":
                    OpenPopup(columnsetupfor, URLToOpen + "Requisition");
                    break;
                case "ordermasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "PurchaseOrder");
                    break;
                case "workorder":
                    OpenPopup(columnsetupfor, URLToOpen + "WorkOrder");
                    break;
                case "pullmaster":
                    OpenPopup(columnsetupfor, URLToOpen + "Pulls");
                    break;
                case "binmasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "BinLocation");
                    break;
                case "categorymasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Category");
                    break;
                case "customermasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Customers");
                    break;
                case "freighttypemasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "FreightTypes");
                    break;
                case "glaccountmasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "GLAccounts");
                    break;
                case "locationmasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Locations");
                    break;
                case "manufacturermasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Manufacturers");
                    break;
                case "measurementtermlist":
                    OpenPopup(columnsetupfor, URLToOpen + "MeasurementTerms");
                    break;
                case "shipviamasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "ShipVia");
                    break;
                case "suppliermasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Supplier");
                    break;
                case "technicianlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Technicians");
                    break;
                case "toolcategorylist":
                    OpenPopup(columnsetupfor, URLToOpen + "ToolCategory");
                    break;
                case "assetcategorylist":
                    OpenPopup(columnsetupfor, URLToOpen + "AssetCategory");
                    break;
                case "unitmasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Units");
                    break;
                case "itemmasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Items");
                    break;
                case "receivemasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "Receives_HeaderReport");
                    break;
                case "ordereditems":
                    OpenPopup(columnsetupfor, URLToOpen + "OrderedItems_HeaderReport");
                    break;
                case "costuommasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "CostUOM1");
                    break;
                case "inventoryclassificationmasterlist":
                    OpenPopup(columnsetupfor, URLToOpen + "InventoryClassificationMaster");
                    break;
                case "inventorylocationslist":
                    OpenPopup(columnsetupfor, URLToOpen + "InventoryLocationsMaster");
                    break;
                case "materialstaging":
                    OpenPopup(columnsetupfor, URLToOpen + "rpt_StagingHeader");
                    break;

            }

        },
        error: function (response) {

        }
    })
}


function callPrintTransactionForAll(DataTableName, Path, CurrentoTableColsList, StartDate, EndDate, BarCodeColumns) {

    var URLToOpen = location.protocol + "//" + location.host + "/Reports" + escape(Path);
    var CompanyRprtIds = "";
    var RoomRprtIds = "";

    $("#ComapnyGlobalReprotBuilderForAll").multiselect("getChecked").each(function () {
        CompanyRprtIds += this.value + ",";
    });
    $("#RoomGlobalReprotBuilderForAll").multiselect("getChecked").each(function () {
        RoomRprtIds += this.value + ",";
    });

    $.ajax({
        "dataType": 'json',
        "type": "POST",
        "url": "/Master/AddReportParamsToSesstion",
        cache: false,
        "async": false,
        data: { Ids: "", Title: $("#textGlobalReprotBuilder")[0].value, StartDate: StartDate, EndDate: EndDate, DisplayFields: CurrentoTableColsList, CompanyIds: CompanyRprtIds, RoomIds: RoomRprtIds, BarcodeColumn: BarCodeColumns },
        "success": function (json) {
            OpenPopup("", URLToOpen);
        },
        error: function (response) {

        }
    })
}


function OpenPopup(columnsetupfor, URlToOpen) {

    IsFirstTime = true;
    IsRoomFirstTime = true;

    mywindow = window.open(URlToOpen, "_blank");
    //    mywindow.moveTo(0, 0);
}
function clearControls(form) {

    $(':input', '#' + form + '')
            .not(':button, :submit, :reset, :hidden')
            .val('')
            .removeAttr('checked')
            .removeAttr('selected')
    ;
}

function disableControls(form) {
    $(':input', '#' + form + '')
        .not('#btnCancel')
        .attr('disabled', 'disabled');
}

function readyonlyControls(form) {
    $(':input', '#' + form + '')
        .not('#btnCancel')
        .attr('readyonly', 'readyonly');
}



function NarrowSearchInGrid(searchstr) {
    FilterStringGlobalUse = searchstr;
    $('#myDataTable').dataTable().fnFilter(searchstr, null, null, null)
}



function GetNarroHTML(tableName, textFieldName, companyID, roomID) {
    $.ajax({
        'url': '/Master/GetDDData',
        data: { TableName: tableName, TextFieldName: textFieldName },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (i, val) {
                s += '<option value="' + val.ID + '">' + val.Text + '</option>';
            });

            $("#UserCreated").append(s);
            $("#UserUpdated").append(s);

            $("#UserCreated").multiselect
            (
                {
                    noneSelectedText: UserCreatedBy, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return CreatedBy + ' ' + numChecked + ' ' + selected;
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                UserCreatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                });
                DoNarrowSearch();
            });

            $("#UserUpdated").multiselect
            (
                {
                    noneSelectedText: UserUpdatedby, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return UpdatedBy + ' ' + numChecked + ' ' + selected;
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                UserUpdatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })
                DoNarrowSearch();
            });
        },
        error: function (response) {
            // through errror message
        }
    });
}

function GetNarrowDDData(tableName, companyID, roomID, _IsArchived, _IsDeleted, _RequisitionCurrentTab) {

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: tableName, TextFieldName: 'CreatedBy', IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: _RequisitionCurrentTab },
        success: function (response) {
            var s = '';

            if (response.DDData != null) {

                if (tableName == "CartItem" || tableName == "CartItemList" || tableName == "MaterialStaging" || tableName == "EnterpriseMaster" || tableName == "CompanyMaster" || tableName == "InventoryCountList" || tableName == "RoleMaster" || tableName == "UserMaster" || tableName == "Room" || tableName == "AssetToolSchedulerList" || tableName == "WorkOrder") {
                    $.each(response.DDData, function (i, val) {
                        var ArrData = i.toString().split('[###]');
                        s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + val + ')' + '</option>';
                    });
                }
                else {
                    $.each(response.DDData, function (i, val) {
                        s += '<option value="' + i + '">' + i + ' (' + val + ')' + '</option>';
                    });

                }
            }

            //Destroy widgets before reapplying the filter
            $("#UserCreated").empty();
            $("#UserCreated").multiselect('destroy');
            $("#UserCreated").multiselectfilter('destroy');

            $("#UserCreated").append(s);
            $("#UserCreated").multiselect
            (
                {
                    noneSelectedText: UserCreatedBy, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return CreatedBy + ' ' + numChecked + ' ' + selected;
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#UserCreatedCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#UserCreatedCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#UserCreatedCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#UserCreatedCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#UserCreatedCollapse").text().indexOf(ui.text) == -1) {
                        $("#UserCreatedCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#UserCreatedCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#UserCreatedCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#UserCreatedCollapse").html(text);
                    }
                    else
                        $("#UserCreatedCollapse").html('');
                }
                UserCreatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#UserCreatedCollapse").text().trim() != '')
                    $("#UserCreatedCollapse").show();
                else
                    $("#UserCreatedCollapse").hide();


                if ($("#UserCreatedCollapse").find('span').length <= 2) {
                    $("#UserCreatedCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#UserCreatedCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message            
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: tableName, TextFieldName: 'LastUpdatedBy', IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: _RequisitionCurrentTab },
        success: function (response) {
            var s = '';
            if (response.DDData != null) {
                if (tableName == "CartItem" || tableName == "CartItemList" || tableName == "MaterialStaging" || tableName == "EnterpriseMaster" || tableName == "CompanyMaster" || tableName == "InventoryCountList" || tableName == "RoleMaster" || tableName == "UserMaster" || tableName == "Room" || tableName == "AssetToolSchedulerList" || tableName == "WorkOrder") {
                    $.each(response.DDData, function (i, val) {
                        var ArrData = i.toString().split('[###]');
                        s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + val + ')' + '</option>';
                    });
                }
                else {
                    $.each(response.DDData, function (i, val) {
                        s += '<option value="' + i + '">' + i + ' (' + val + ')' + '</option>';
                    });
                }

            }

            //Destroy widgets before reapplying the filter
            $("#UserUpdated").empty();
            $("#UserUpdated").multiselect('destroy');
            $("#UserUpdated").multiselectfilter('destroy');

            $("#UserUpdated").append(s);
            $("#UserUpdated").multiselect
            (
                {
                    noneSelectedText: UserUpdatedby, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return UpdatedBy + ' ' + numChecked + ' ' + selected;
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#UserUpdatedCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#UserUpdatedCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#UserUpdatedCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#UserUpdatedCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#UserUpdatedCollapse").text().indexOf(ui.text) == -1) {
                        $("#UserUpdatedCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#UserUpdatedCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#UserUpdatedCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#UserUpdatedCollapse").html(text);
                    }
                    else
                        $("#UserUpdatedCollapse").html('');
                }
                UserUpdatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#UserUpdatedCollapse").text().trim() != '')
                    $("#UserUpdatedCollapse").show();
                else
                    $("#UserUpdatedCollapse").hide();

                if ($("#UserUpdatedCollapse").find('span').length <= 2) {
                    $("#UserUpdatedCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#UserUpdatedCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}

function GetNarroHTMLForUDF(tableName, companyID, roomID, _IsArchived, _IsDeleted, _RequisitionCurrentTab) {
    var UDFObject;

    $("select[name='udflist']").each(function (index) {
        var UDFUniqueID = this.getAttribute('UID');
        $.ajax({
            'url': '/Master/GetUDFDDData',
            data: { TableName: tableName, UDFName: this.id, UDFUniqueID: UDFUniqueID, IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: _RequisitionCurrentTab },
            success: function (response) {
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (UDFVal, ValCount) {
                        s += '<option value="' + UDFVal + '">' + UDFVal + ' (' + ValCount + ')' + '</option>';
                    });
                }
                var UDFColumnNameTemp = response.UDFColName.toString().replace("_dd", "");

                //Destroy widgets before reapplying the filter
                $('[id="' + response.UDFColName + '"]').empty();
                $('[id="' + response.UDFColName + '"]').multiselect('destroy');
                $('[id="' + response.UDFColName + '"]').multiselectfilter('destroy');

                $('[id="' + response.UDFColName + '"]').append(s);

                $('[id="' + response.UDFColName + '"]').multiselect
                (
                    {
                        noneSelectedText: UDFColumnNameTemp, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return UDFColumnNameTemp + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            var CollapseObject = $('#' + UDFUniqueID + 'Collapse')
                            $(CollapseObject).html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($(CollapseObject).text().indexOf(ui.target[i].text) == -1) {
                                    $(CollapseObject).append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $(CollapseObject).show();
                        }
                    }
                )
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    var CollapseObject = $('#' + UDFUniqueID + 'Collapse')
                    if (ui.checked) {
                        if ($(CollapseObject).text().indexOf(ui.text) == -1) {
                            $(CollapseObject).append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $(CollapseObject).html('');
                        }
                        else if (!ui.checked) {
                            var text = $(CollapseObject).html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $(CollapseObject).html(text);
                        }
                        else
                            $(CollapseObject).html('');
                    }
                    if (UDFUniqueID == "UDF1") {
                        UserUDF1NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                    }
                    else if (UDFUniqueID == "UDF2") {
                        UserUDF2NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                    }
                    else if (UDFUniqueID == "UDF3") {
                        UserUDF3NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                    }
                    else if (UDFUniqueID == "UDF4") {
                        UserUDF4NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                    }
                    else {
                        UserUDF5NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                    }

                    if ($(CollapseObject).text().trim() != '')
                        $(CollapseObject).show();
                    else
                        $(CollapseObject).hide();


                    if ($(CollapseObject).find('span').length <= 2) {
                        $(CollapseObject).scrollTop(0).height(50);
                    }
                    else {
                        $(CollapseObject).scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();
                    DoNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {

                // through errror message
            }
        });
    });
}

function GetAssetNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'ToolCategory', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });
            //Destroy widgets before reapplying the filter
            $("#ToolsCategory").empty();
            $("#ToolsCategory").multiselect('destroy');
            $("#ToolsCategory").multiselectfilter('destroy');

            $("#ToolsCategory").append(s);
            $("#ToolsCategory").multiselect(
                        {
                            noneSelectedText: 'ToolsCategory', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'ToolsCategory: ' + numChecked + ' selected';
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ToolsCategoryCollapse").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ToolsCategoryCollapse").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ToolsCategoryCollapse").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ToolsCategoryCollapse").show();
                            }
                        }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ToolsCategoryCollapse").text().indexOf(ui.text) == -1) {
                        $("#ToolsCategoryCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ToolsCategoryCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ToolsCategoryCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ToolsCategoryCollapse").html(text);
                    }
                    else
                        $("#ToolsCategoryCollapse").html('');
                }
                ToolCategoryValue = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ToolsCategoryCollapse").text().trim() != '')
                    $("#ToolsCategoryCollapse").show();
                else
                    $("#ToolsCategoryCollapse").hide();


                if ($("#ToolsCategoryCollapse").find('span').length <= 2) {
                    $("#ToolsCategoryCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ToolsCategoryCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });



}

function GetToolsNarrowSearchData(_IsArchived, _IsDeleted) {
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: 'ToolMaster', TextFieldName: 'ToolCategory', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });
            //Destroy widgets before reapplying the filter
            $("#ToolsCategory").empty();
            $("#ToolsCategory").multiselect('destroy');
            $("#ToolsCategory").multiselectfilter('destroy');

            $("#ToolsCategory").append(s);
            $("#ToolsCategory").multiselect(
                        {
                            noneSelectedText: 'ToolsCategory', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'ToolsCategory: ' + numChecked + ' selected';
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ToolsCategoryCollapse").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ToolsCategoryCollapse").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ToolsCategoryCollapse").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ToolsCategoryCollapse").show();
                            }
                        }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ToolsCategoryCollapse").text().indexOf(ui.text) == -1) {
                        $("#ToolsCategoryCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ToolsCategoryCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ToolsCategoryCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ToolsCategoryCollapse").html(text);
                    }
                    else
                        $("#ToolsCategoryCollapse").html('');
                }
                ToolCategoryValue = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ToolsCategoryCollapse").text().trim() != '')
                    $("#ToolsCategoryCollapse").show();
                else
                    $("#ToolsCategoryCollapse").hide();


                if ($("#ToolsCategoryCollapse").find('span').length <= 2) {
                    $("#ToolsCategoryCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ToolsCategoryCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    $.ajax({
        'url': '/Master/GetDDData',
        data: { TableName: 'TechnicianMaster', TextFieldName: 'Technician' },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (i, val) {
                s += '<option value="' + val.ID + '">' + val.Text + '</option>';
            });
            $("#ToolsTechnician").append(s);
            $("#ToolsTechnician").multiselect(
                        {
                            noneSelectedText: 'Technician', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Technician: ' + numChecked + ' selected';
                            }
                        }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                TechicianValue = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })
            });
        },
        error: function (response) {
            // through errror message
        }
    });


    $.ajax({
        'url': '/Master/GetDDData',
        data: { TableName: 'WorkOrder', TextFieldName: 'WOName' },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (i, val) {
                s += '<option value="' + val.ID + '">' + val.Text + '</option>';
            });
            $("#ToolsWorkOrder").append(s);
            $("#ToolsWorkOrder").multiselect(
                        {
                            noneSelectedText: 'Work Order', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Work Order: ' + numChecked + ' selected';
                            }
                        }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                WorkOrderValue = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })
            });
        },
        error: function (response) {
            // through errror message
        }
    });
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: 'ToolMaster', TextFieldName: 'Location', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#ToolsLocation").empty();
            $("#ToolsLocation").multiselect('destroy');
            $("#ToolsLocation").multiselectfilter('destroy');

            $("#ToolsLocation").append(s);
            $("#ToolsLocation").multiselect(
                        {
                            noneSelectedText: 'Location', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Location: ' + numChecked + ' selected';
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ToolsLocationCollapse").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ToolsLocationCollapse").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ToolsLocationCollapse").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ToolsLocationCollapse").show();
                            }
                        }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ToolsLocationCollapse").text().indexOf(ui.text) == -1) {
                        $("#ToolsLocationCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ToolsLocationCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ToolsLocationCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ToolsLocationCollapse").html(text);
                    }
                    else
                        $("#ToolsLocationCollapse").html('');
                }
                LocationValue = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ToolsLocationCollapse").text().trim() != '')
                    $("#ToolsLocationCollapse").show();
                else
                    $("#ToolsLocationCollapse").hide();


                if ($("#ToolsLocationCollapse").find('span').length <= 2) {
                    $("#ToolsLocationCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ToolsLocationCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    $("#ToolsMaintenance").multiselect(
                        {
                            noneSelectedText: 'Tool Maintenance', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Tool Maintenance: ' + numChecked + ' selected';
                            }
                        }
            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                ToolMaintanceValue = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })
            });
}

/////// /////// /////// GetPullNarrowSearchData/////// /////// /////// /////// ///////

function GetPullNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'SupplierName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#PullSupplier").empty();
            $("#PullSupplier").multiselect('destroy');
            $("#PullSupplier").multiselectfilter('destroy');

            $("#PullSupplier").append(s);
            $("#PullSupplier").multiselect
            (
                {
                    noneSelectedText: Supplier, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return Supplier + ' ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#PullSupplierCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#PullSupplierCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#PullSupplierCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#PullSupplierCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#PullSupplierCollapse").text().indexOf(ui.text) == -1) {
                        $("#PullSupplierCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#PullSupplierCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#PullSupplierCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#PullSupplierCollapse").html(text);
                    }
                    else
                        $("#PullSupplierCollapse").html('');
                }
                PullSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#PullSupplierCollapse").text().trim() != '')
                    $("#PullSupplierCollapse").show();
                else
                    $("#PullSupplierCollapse").hide();


                if ($("#PullSupplierCollapse").find('span').length <= 2) {
                    $("#PullSupplierCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#PullSupplierCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });




    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Manufacturer', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#Manufacturer").empty();
            $("#Manufacturer").multiselect('destroy');
            $("#Manufacturer").multiselectfilter('destroy');

            $("#Manufacturer").append(s);
            $("#Manufacturer").multiselect
            (
                {
                    noneSelectedText: Manufacturer, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return Manufacturer + ' ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ManufacturerCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ManufacturerCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ManufacturerCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ManufacturerCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ManufacturerCollapse").text().indexOf(ui.text) == -1) {
                        $("#ManufacturerCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ManufacturerCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ManufacturerCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ManufacturerCollapse").html(text);
                    }
                    else
                        $("#ManufacturerCollapse").html('');
                }
                ManufacturerNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ManufacturerCollapse").text().trim() != '')
                    $("#ManufacturerCollapse").show();
                else
                    $("#ManufacturerCollapse").hide();


                if ($("#ManufacturerCollapse").find('span').length <= 2) {
                    $("#ManufacturerCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ManufacturerCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Category', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#PullCategory").empty();
            $("#PullCategory").multiselect('destroy');
            $("#PullCategory").multiselectfilter('destroy');

            $("#PullCategory").append(s);
            $("#PullCategory").multiselect
            (
                {
                    noneSelectedText: Category, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return Category + ' ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#PullCategoryCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#PullCategoryCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#PullCategoryCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#PullCategoryCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#PullCategoryCollapse").text().indexOf(ui.text) == -1) {
                        $("#PullCategoryCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#PullCategoryCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#PullCategoryCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#PullCategoryCollapse").html(text);
                    }
                    else
                        $("#PullCategoryCollapse").html('');
                }
                PullCategoryNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#PullCategoryCollapse").text().trim() != '')
                    $("#PullCategoryCollapse").show();
                else
                    $("#PullCategoryCollapse").hide();


                if ($("#PullCategoryCollapse").find('span').length <= 2) {
                    $("#PullCategoryCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#PullCategoryCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}

/////// /////// /////// GetPullNarrowSearchData/////// /////// /////// /////// ///////

/////// /////// /////// GetMatStagNarrowSearchData/////// /////// /////// /////// ///////
function GetMatStagNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'BinName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#MatStagLocations").empty();
            $("#MatStagLocations").multiselect('destroy');
            $("#MatStagLocations").multiselectfilter('destroy');

            $("#MatStagLocations").append(s);
            $("#MatStagLocations").multiselect
            (
                {
                    noneSelectedText: 'Staging Location', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Staging Location ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#MatStagLocationsCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#MatStagLocationsCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#MatStagLocationsCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#MatStagLocationsCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#MatStagLocationsCollapse").text().indexOf(ui.text) == -1) {
                        $("#MatStagLocationsCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#MatStagLocationsCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#MatStagLocationsCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#MatStagLocationsCollapse").html(text);
                    }
                    else
                        $("#MatStagLocationsCollapse").html('');
                }
                MatStagLocationsNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#MatStagLocationsCollapse").text().trim() != '')
                    $("#MatStagLocationsCollapse").show();
                else
                    $("#MatStagLocationsCollapse").hide();


                if ($("#MatStagLocationsCollapse").find('span').length <= 2) {
                    $("#MatStagLocationsCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#MatStagLocationsCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}
/////// /////// /////// GetMatStagNarrowSearchData/////// /////// /////// /////// ///////


/////// /////// /////// GetMatStagNarrowSearchData/////// /////// /////// /////// ///////
function GetCartNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {

    //    $("#CartRT").empty();
    //    $("#CartRT").multiselect('destroy');
    //    $("#CartRT").multiselectfilter('destroy');

    //    $("#CartRT").append(s);

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'ReplenishType', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#CartRT").empty();
            $("#CartRT").multiselect('destroy');
            $("#CartRT").multiselectfilter('destroy');
            $("#CartRT").html(s);
            $("#CartRT").multiselect
            (
                {
                    noneSelectedText: 'Replenish type', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Replenish type ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#CartRTCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#CartRTCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#CartRTCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#CartRTCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#CartRTCollapse").text().indexOf(ui.text) == -1) {
                        $("#CartRTCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#CartRTCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#CartRTCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#CartRTCollapse").html(text);
                    }
                    else
                        $("#CartRTCollapse").html('');
                }
                CartRTNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#CartRTCollapse").text().trim() != '')
                    $("#CartRTCollapse").show();
                else
                    $("#CartRTCollapse").hide();


                if ($("#CartRTCollapse").find('span').length <= 2) {
                    $("#CartRTCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#CartRTCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        }
    });



    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'SupplierName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#CartSupplier").empty();
            $("#CartSupplier").multiselect('destroy');
            $("#CartSupplier").multiselectfilter('destroy');

            $("#CartSupplier").append(s);
            $("#CartSupplier").multiselect
            (
                {
                    noneSelectedText: 'Supplier', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Supplier ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#CartSupplierCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#CartSupplierCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#CartSupplierCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#CartSupplierCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#CartSupplierCollapse").text().indexOf(ui.text) == -1) {
                        $("#CartSupplierCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#CartSupplierCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#CartSupplierCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#CartSupplierCollapse").html(text);
                    }
                    else
                        $("#CartSupplierCollapse").html('');
                }
                CartSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#CartSupplierCollapse").text().trim() != '')
                    $("#CartSupplierCollapse").show();
                else
                    $("#CartSupplierCollapse").hide();


                if ($("#CartSupplierCollapse").find('span').length <= 2) {
                    $("#CartSupplierCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#CartSupplierCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}
/////// /////// /////// GetMatStagNarrowSearchData/////// /////// /////// /////// ///////


function GetICountNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'CountType', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#ICountType").empty();
            $("#ICountType").multiselect('destroy');
            $("#ICountType").multiselectfilter('destroy');
            $("#ICountType").html(s);
            $("#ICountType").multiselect
            (
                {
                    noneSelectedText: 'Count type', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Count type ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ICountTypeCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ICountTypeCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ICountTypeCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ICountTypeCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ICountTypeCollapse").text().indexOf(ui.text) == -1) {
                        $("#ICountTypeCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ICountTypeCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ICountTypeCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ICountTypeCollapse").html(text);
                    }
                    else
                        $("#ICountTypeCollapse").html('');
                }
                ICountTypeNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#ICountTypeCollapse").text().trim() != '')
                    $("#ICountTypeCollapse").show();
                else
                    $("#ICountTypeCollapse").hide();


                if ($("#ICountTypeCollapse").find('span').length <= 2) {
                    $("#ICountTypeCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ICountTypeCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'CountStatus', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#ICountStatus").empty();
            $("#ICountStatus").multiselect('destroy');
            $("#ICountStatus").multiselectfilter('destroy');
            $("#ICountStatus").html(s);

            $("#ICountStatus").multiselect
            (
                {
                    noneSelectedText: 'Count status', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Count status ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ICountStatusCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ICountStatusCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ICountStatusCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ICountStatusCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ICountStatusCollapse").text().indexOf(ui.text) == -1) {
                        $("#ICountStatusCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ICountStatusCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ICountStatusCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ICountStatusCollapse").html(text);
                    }
                    else
                        $("#ICountStatusCollapse").html('');
                }
                ICountStatusNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#ICountStatusCollapse").text().trim() != '')
                    $("#ICountStatusCollapse").show();
                else
                    $("#ICountStatusCollapse").hide();


                if ($("#ICountStatusCollapse").find('span').length <= 2) {
                    $("#ICountStatusCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ICountStatusCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();

        }
    });
}

$(document).ready(function () {
    //SEARCH
    $('#NarroSearchGo').click(function () {
        DoNarrowSearch();
    });

    $('#DateCFrom,#DateCTo').change(function () {

        var DateCFromValid = isDate($('#DateCFrom').val());
        var DateCToValid = isDate($('#DateCTo').val());
        if (DateCFromValid && DateCToValid) {
            $("#global_filter").val('');
            DoNarrowSearch();
        }
        else {
            if (!DateCFromValid)
                $('#DateCFrom').val('');
            if (!DateCToValid)
                $('#DateCTo').val('');
        }
    });

    $('#DateUFrom,#DateUTo').change(function () {
        var DateUFromValid = isDate($('#DateUFrom').val());
        var DateUToValid = isDate($('#DateUTo').val());
        if (DateUFromValid && DateUToValid) {
            $("#global_filter").val('');
            DoNarrowSearch();
        }
        else {
            if (!DateUFromValid)
                $('#DateUFrom').val('');
            if (!DateUToValid)
                $('#DateUTo').val('');

        }
    });


    //CLEAR NARROW SEARCH
    $('#NarroSearchClear').click(function () {

        //        $('#DateCFrom').val('');
        //        $('#DateCTo').val('');
        //        $('#DateUFrom').val('');
        //        $('#DateUTo').val('');
        //        $("#UserCreated").multiselect("uncheckAll");
        //        $("#UserCreatedCollapse").html('');
        //        $("#UserUpdated").multiselect("uncheckAll");
        //        $("select[name='udflist']").each(function (index) {
        //            $(this).multiselect("uncheckAll");
        //        });

        //        // clear pull narrow search extra items 
        //        if ($("#PullSupplier") != undefined) {
        //            $("#PullSupplier").multiselect("uncheckAll");
        //            $("#PullSupplierCollapse").html('');
        //            $("#Manufacturer").multiselect("uncheckAll");
        //            $("#ManufacturerCollapse").html('');
        //            $("#PullCategory").multiselect("uncheckAll");
        //            $("#PullCategoryCollapse").html('');

        //            $("#OrderStatus").multiselect("uncheckAll");
        //            $("#OrderStatusCollapse").html('');
        //            //$("#OrderRequiredDate").multiselect("uncheckAll");
        //            $("#OrderSupplier").multiselect("uncheckAll");
        //            $("#OrderSupplierCollapse").html('');

        //            if ($('#PullCost') != undefined) {
        //                $('#PullCost').val('0_-1');
        //            }

        //            if ($('#StockStatus') != undefined) {
        //                $('#StockStatus').val('0');
        //            }

        //            $("#ItemType").multiselect("uncheckAll");
        //            $("#ItemTypeCollapse").html('');

        //        }
        //        if ($("#ToolsCategory") != undefined) {
        //            $("#ToolsCategory").multiselect("uncheckAll");
        //            $("#ToolsCategoryCollapse").html('');
        //            $("#ToolsWorkOrder").multiselect("uncheckAll");
        //            $("#ToolsWorkOrderCollapse").html('');
        //            $("#ToolsMaintenance").multiselect("uncheckAll");
        //            $("#ToolsMaintenanceCollapse").html('');
        //            $("#ToolsLocation").multiselect("uncheckAll");
        //            $("#ToolsLocationCollapse").html('');
        //            if ($('#ToolsCost') != undefined) {
        //                $('#ToolsCost').val('0_-1');
        //            }
        //        }

        //        if ($("#ReqCustomer") != undefined) {
        //            $("#ReqCustomer").multiselect("uncheckAll");
        //            $("#ReqCustomerCollapse").html('');
        //        }

        if (gblActionName === "KitList" || gblActionName === "RequisitionList") {
            $.ajax({
                url: "/Master/GetMainFilterSessionValue",
                type: "Get",
                dataType: "json",
                "async": false,
                success: function (response) {
                    if (response.value == "true") {
                        ClickFromMenu(false);
                        if (gblActionName === "RequisitionList") {
                            window.location = "/Consume/RequisitionList";
                        }
                        else if (gblActionName === "KitList") {
                            window.location = "/Kit/KitList";
                        }
                        return;
                    }
                },
                error: function (xhr) {
                }
            });
        }
        if ($('#DateCFrom').val() != '') $('#DateCFrom').val('');
        if ($('#DateCTo').val() != '') $('#DateCTo').val('');
        if ($('#DateUFrom').val() != '') $('#DateUFrom').val('');
        if ($('#DateUTo').val() != '') $('#DateUTo').val('');




        if (typeof ($("#UserType").multiselect("getChecked").length) != undefined && $("#UserType").multiselect("getChecked").length > 0) {
            $("#UserType").multiselect("uncheckAll");
            $("#UserTypeCollapse").html('');
        }
        if (typeof ($("#UserRole").multiselect("getChecked").length) != undefined && $("#UserRole").multiselect("getChecked").length > 0) {
            $("#UserRole").multiselect("uncheckAll");
            $("#UserRoleCollapse").html('');
        }

        if (typeof ($("#UserRoom").multiselect("getChecked").length) != undefined && $("#UserRoom").multiselect("getChecked").length > 0) {
            $("#UserRoom").multiselect("uncheckAll");
            $("#UserRoomCollapse").html('');
        }
        if (typeof ($("#UserCompany").multiselect("getChecked").length) != undefined && $("#UserCompany").multiselect("getChecked").length > 0) {
            $("#UserCompany").multiselect("uncheckAll");
            $("#UserCompanyCollapse").html('');
        }
        if (typeof ($("#RoleRoom").multiselect("getChecked").length) != undefined && $("#RoleRoom").multiselect("getChecked").length > 0) {
            $("#RoleRoom").multiselect("uncheckAll");
            $("#RoleRoomCollapse").html('');
        }
        if (typeof ($("#RoleCompany").multiselect("getChecked").length) != undefined && $("#RoleCompany").multiselect("getChecked").length > 0) {
            $("#RoleCompany").multiselect("uncheckAll");
            $("#RoleCompanyCollapse").html('');
        }
        if (typeof ($("#UserCreated").multiselect("getChecked").length) != undefined && $("#UserCreated").multiselect("getChecked").length > 0) {
            $("#UserCreated").multiselect("uncheckAll");
            $("#UserCreatedCollapse").html('');
        }
        if (typeof ($("#ICountStatus").multiselect("getChecked").length) != undefined && $("#ICountStatus").multiselect("getChecked").length > 0) {
            $("#ICountStatus").multiselect("uncheckAll");
            $("#ICountStatusCollapse").html('');
        }
        if (typeof ($("#ICountType").multiselect("getChecked").length) != undefined && $("#ICountType").multiselect("getChecked").length > 0) {
            $("#ICountType").multiselect("uncheckAll");
            $("#ICountTypeCollapse").html('');
        }
        //UserUpdated
        if (typeof ($("#UserUpdated").multiselect("getChecked").length) != undefined && $("#UserUpdated").multiselect("getChecked").length > 0) {
            $("#UserUpdated").multiselect("uncheckAll");
            $("#UserUpdatedCollapse").html('');
        }

        //UDFs
        $("select[name='udflist']").each(function (index) {
            if (typeof ($(this).multiselect("getChecked").length) != undefined && $(this).multiselect("getChecked").length > 0) {
                var UDFUniqueID = this.getAttribute('UID');
                $(this).multiselect("uncheckAll");
                $('#' + UDFUniqueID + 'Collapse').html('');
            }
        });

        //PullSupplier
        if (typeof ($("#PullSupplier").multiselect("getChecked").length) != undefined && $("#PullSupplier").multiselect("getChecked").length > 0) {
            $("#PullSupplier").multiselect("uncheckAll");
            $("#PullSupplierCollapse").html('');
        }

        //Manufacturer
        if (typeof ($("#Manufacturer").multiselect("getChecked").length) != undefined && $("#Manufacturer").multiselect("getChecked").length > 0) {
            $("#Manufacturer").multiselect("uncheckAll");
            $("#ManufacturerCollapse").html('');
        }

        //PullCategory
        if (typeof ($("#PullCategory").multiselect("getChecked").length) != undefined && $("#PullCategory").multiselect("getChecked").length > 0) {
            $("#PullCategory").multiselect("uncheckAll");
            $("#PullCategoryCollapse").html('');
        }

        //OrderStatus
        if (typeof ($("#OrderStatus").multiselect("getChecked").length) != undefined && $("#OrderStatus").multiselect("getChecked").length > 0) {
            $("#OrderStatus").multiselect("uncheckAll");
            $("#OrderStatusCollapse").html('');
        }

        //OrderSupplier
        if (typeof ($("#OrderSupplier").multiselect("getChecked").length) != undefined && $("#OrderSupplier").multiselect("getChecked").length > 0) {
            $("#OrderSupplier").multiselect("uncheckAll");
            $("#OrderSupplierCollapse").html('');
        }

        //OrderSupplier
        if (typeof ($("#OrderSupplier").multiselect("getChecked").length) != undefined && $("#OrderSupplier").multiselect("getChecked").length > 0) {
            $("#OrderSupplier").multiselect("uncheckAll");
            $("#OrderSupplierCollapse").html('');
        }

        //ItemType
        if (typeof ($("#ItemTypeNarroDDL").multiselect("getChecked").length) != undefined && $("#ItemTypeNarroDDL").multiselect("getChecked").length > 0) {
            $("#ItemTypeNarroDDL").multiselect("uncheckAll");
            $("#ItemTypeCollapse").html('');
        }

        //ToolsCategory
        if (typeof ($("#ToolsCategory").multiselect("getChecked").length) != undefined && $("#ToolsCategory").multiselect("getChecked").length > 0) {
            $("#ToolsCategory").multiselect("uncheckAll");
            $("#ToolsCategoryCollapse").html('');
        }

        //ToolsWorkOrder
        if (typeof ($("#ToolsWorkOrder").multiselect("getChecked").length) != undefined && $("#ToolsWorkOrder").multiselect("getChecked").length > 0) {
            $("#ToolsWorkOrder").multiselect("uncheckAll");
            $("#ToolsWorkOrderCollapse").html('');
        }

        //ToolsMaintenance
        if (typeof ($("#ToolsMaintenance").multiselect("getChecked").length) != undefined && $("#ToolsMaintenance").multiselect("getChecked").length > 0) {
            $("#ToolsMaintenance").multiselect("uncheckAll");
            $("#ToolsMaintenanceCollapse").html('');
        }

        //ToolsLocation
        if (typeof ($("#ToolsLocation").multiselect("getChecked").length) != undefined && $("#ToolsLocation").multiselect("getChecked").length > 0) {
            $("#ToolsLocation").multiselect("uncheckAll");
            $("#ToolsLocationCollapse").html('');
        }

        //ReqCustomer
        if (typeof ($("#ReqCustomer").multiselect("getChecked").length) != undefined && $("#ReqCustomer").multiselect("getChecked").length > 0) {
            $("#ReqCustomer").multiselect("uncheckAll");
            $("#ReqCustomerCollapse").html('');
        }

        //ReqWorkOrder
        if (typeof ($("#ReqWorkOrder").multiselect("getChecked").length) != undefined && $("#ReqWorkOrder").multiselect("getChecked").length > 0) {
            $("#ReqWorkOrder").multiselect("uncheckAll");
            $("#ReqWorkOrderCollapse").html('');
        }

        //WOCustomer
        if (typeof ($("#WOCustomer").multiselect("getChecked").length) != undefined && $("#WOCustomer").multiselect("getChecked").length > 0) {
            $("#WOCustomer").multiselect("uncheckAll");
            $("#WOCustomerCollapse").html('');
        }
        if (typeof ($("#WOTechnician").multiselect("getChecked").length) != undefined && $("#WOTechnician").multiselect("getChecked").length > 0) {
            $("#WOTechnician").multiselect("uncheckAll");
            $("#WOTechnicianCollapse").html('');
        }
        if (typeof ($("#WOAsset").multiselect("getChecked").length) != undefined && $("#WOAsset").multiselect("getChecked").length > 0) {
            $("#WOAsset").multiselect("uncheckAll");
            $("#WOAssetCollapse").html('');
        }
        if (typeof ($("#WOTool").multiselect("getChecked").length) != undefined && $("#WOTool").multiselect("getChecked").length > 0) {
            $("#WOTool").multiselect("uncheckAll");
            $("#WOToolCollapse").html('');
        }

        // cart replenish type
        if (typeof ($("#CartRT").multiselect("getChecked").length) != undefined && $("#CartRT").multiselect("getChecked").length > 0) {
            $("#CartRT").multiselect("uncheckAll");
            $("#CartRTCollapse").html('');
        }
        // cart supplier
        if (typeof ($("#CartSupplier").multiselect("getChecked").length) != undefined && $("#CartSupplier").multiselect("getChecked").length > 0) {
            $("#CartSupplier").multiselect("uncheckAll");
            $("#CartSupplierCollapse").html('');
        }
        // Role Master User Type
        if (typeof ($("#RoleUserType").multiselect("getChecked").length) != undefined && $("#RoleUserType").multiselect("getChecked").length > 0) {
            $("#RoleUserType").multiselect("uncheckAll");
            $("#RoleUserTypeCollapse").html('');
        }
        //Staging Bin
        if (typeof ($("#MatStagLocations").multiselect("getChecked").length) != undefined && $("#MatStagLocations").multiselect("getChecked").length > 0) {
            $("#MatStagLocations").multiselect("uncheckAll");
            $("#MatStagLocationsCollapse").html('');
        }
        //Staging Bin
        if (typeof ($("#ddlOrderNumber").multiselect("getChecked").length) != undefined && $("#ddlOrderNumber").multiselect("getChecked").length > 0) {
            $("#ddlOrderNumber").multiselect("uncheckAll");
            $("#ddlOrderNumberCollapse").html('');
        }

        //PullCost
        if ($('#PullCost') != undefined) {
            $('#PullCost').val('0_-1');
        }
        //StockStatus
        if ($('#StockStatus') != undefined) {
            $('#StockStatus').val('0');
        }
        //ToolsCost
        if ($('#ToolsCost') != undefined) {
            $('#ToolsCost').val('0_-1');
        }

        ////////////////////////////////////////////
        if (RequisitionCurrentTab == '')
            NarrowSearchInGrid('');
        else
            DoNarrowSearch();
    });

    //CREATE DATE PICKER
    $('#DateCFrom').blur(function () {
    }).datepicker({ dateFormat: 'm/d/yy' });
    $('#DateCTo').blur(function () {
    }).datepicker({ dateFormat: 'm/d/yy' });
    $('#DateUFrom').blur(function () {
    }).datepicker({ dateFormat: 'm/d/yy' });
    $('#DateUTo').blur(function () {
    }).datepicker({ dateFormat: 'm/d/yy' });

    $('#ancDateCFrom').click(function () {
        $('#DateCFrom').focus();
    });
    $('#ancDateCTo').click(function () {
        $('#DateCTo').focus();
    });
    $('#ancDateUFrom').click(function () {
        $('#DateUFrom').focus();
    });
    $('#ancDateUTo').click(function () {
        $('#DateUTo').focus();
    });

    $('#DateCreatedClear').click(function () {
        if ($('#DateCFrom').val() != '' || $('#DateCTo').val() != '') {
            $('#DateCFrom').val('');
            $('#DateCTo').val('');
            //NarrowSearchInGrid('');
            DoNarrowSearch();
        }
    });
    $('#DateUpdatedClear').click(function () {
        if ($('#DateUFrom').val() != '' || $('#DateUTo').val() != '') {
            $('#DateUFrom').val('');
            $('#DateUTo').val('');
            //NarrowSearchInGrid('');
            DoNarrowSearch();
        }
    });

    $('#IsDeletedRecords').live('click', function () {
        if (gblActionName.toLowerCase() === 'transferlist' || gblActionName.toLowerCase() === 'orderlist' || gblActionName.toLowerCase() === 'quicklist' || gblActionName.toLowerCase() === 'templateconfigurationlist')
            return;
        //NarrowSearchInGrid('');
        DoNarrowSearch();
        if ($('#hdnPageName').val() == 'TransferMaster' || $('#hdnPageName').val() == 'OrderMaster') {
            return;
        }
        CallNarrowfunctions();

        if (PageName == 'MaterialStaging') {
            RefressFilterMS("MaterialStaging");
        }

    });
    $('#IsArchivedRecords').live('click', function () {
        //NarrowSearchInGrid('');
        if (gblActionName.toLowerCase() === 'transferlist' || gblActionName.toLowerCase() === 'orderlist' || gblActionName.toLowerCase() === 'quicklist' || gblActionName.toLowerCase() === 'templateconfigurationlist')
            return;

        DoNarrowSearch();
        CallNarrowfunctions();
        if (PageName == 'MaterialStaging') {
            RefressFilterMS("MaterialStaging");
        }
    });

    $("#SDGlobalReprotBuilder").datepicker({
        dateFormat: $("#hdDateFormat").val(),
        onSelect: function (selected) {
            $("#EDGlobalReprotBuilder").datepicker("option", "minDate", selected)
        }
    });
    $("#EDGlobalReprotBuilder").datepicker({
        dateFormat: $("#hdDateFormat").val(),
        onSelect: function (selected) {
            $("#SDGlobalReprotBuilder").datepicker("option", "maxDate", selected)
        }
    });

});

function DoNarrowSearch() {
    var narrowSearchFields = '';
    var narrowSearchValues = '';
    var narrowSearchItem = '';

    eraseCookieforshift("selectstartindex");
    eraseCookieforshift("selectendindex");
    eraseCookieforshift("finalselectedarray");


    if (UserCreatedNarroValues != undefined && UserCreatedNarroValues.length > 0) {
        //narrowSearchItem += "[###]CreatedBy#" + UserCreatedNarroValues;
        narrowSearchFields += "CreatedBy" + ",";
        narrowSearchValues += UserCreatedNarroValues + "@";
    }
    else {
        narrowSearchFields += "CreatedBy" + ",";
        narrowSearchValues += "@";
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUpdatedNarroValues != undefined && UserUpdatedNarroValues.length > 0) {
        //narrowSearchItem += "[###]UpdatedBy#" + UserUpdatedNarroValues;
        narrowSearchFields += "UpdatedBy" + ",";
        narrowSearchValues += UserUpdatedNarroValues + "@";
    }
    else {
        narrowSearchFields += "UpdatedBy" + ",";
        narrowSearchValues += "@";
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    if ($('#DateCFrom').val() != '' && $('#DateCTo').val() != '') {
        //narrowSearchItem += "[###]DateCreatedFrom#" + GetDateInYYYYMMDDFormat($('#DateCFrom').val()) + "#DateCreatedTo#" + GetDateInYYYYMMDDFormat($('#DateCTo').val());        
        narrowSearchFields += "DateCreatedFrom" + ",";
        narrowSearchValues += GetDateInYYYYMMDDFormat($('#DateCFrom').val()) + "," + GetDateInYYYYMMDDFormat($('#DateCTo').val()) + "@";
    }
    else {
        narrowSearchFields += "DateCreatedFrom" + ",";
        narrowSearchValues += "@";
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////
    if ($('#DateUFrom').val() != '' && $('#DateUTo').val() != '') {
        //narrowSearchItem += "[###]DateUpdatedFrom#" + GetDateInYYYYMMDDFormat($('#DateUFrom').val()) + "#DateUpdatedTo#" + GetDateInYYYYMMDDFormat($('#DateUTo').val());
        narrowSearchFields += "DateUpdatedFrom" + ",";
        narrowSearchValues += GetDateInYYYYMMDDFormat($('#DateUFrom').val()) + "," + GetDateInYYYYMMDDFormat($('#DateUTo').val()) + "@";
    }
    else {
        narrowSearchFields += "DateUpdatedFrom" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF1NarrowValues != undefined && UserUDF1NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF1#" + UserUDF1NarrowValues;
        narrowSearchFields += "UDF1" + ",";
        narrowSearchValues += UserUDF1NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF1" + ",";
        narrowSearchValues += "@";
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF2NarrowValues != undefined && UserUDF2NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF2#" + UserUDF2NarrowValues;
        narrowSearchFields += "UDF2" + ",";
        narrowSearchValues += UserUDF2NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF2" + ",";
        narrowSearchValues += "@";
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF3NarrowValues != undefined && UserUDF3NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF3#" + UserUDF3NarrowValues;
        narrowSearchFields += "UDF3" + ",";
        narrowSearchValues += UserUDF3NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF3" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF4NarrowValues != undefined && UserUDF4NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF4#" + UserUDF4NarrowValues;
        narrowSearchFields += "UDF4" + ",";
        narrowSearchValues += UserUDF4NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF4" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF5NarrowValues != undefined && UserUDF5NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF5#" + UserUDF5NarrowValues;
        narrowSearchFields += "UDF5" + ",";
        narrowSearchValues += UserUDF5NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF5" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////
    if (PullSupplierNarroValues != undefined && PullSupplierNarroValues.length > 0) {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += PullSupplierNarroValues + "@";
    }
    else {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////////////////////////////////////////////////////////////
    if (ManufacturerNarroValues != undefined && ManufacturerNarroValues.length > 0) {
        narrowSearchFields += "ManufacturerID" + ",";
        narrowSearchValues += ManufacturerNarroValues + "@";
    }
    else {
        narrowSearchFields += "ManufacturerID" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////////////////////////////////////////////////////////////
    if (PullCategoryNarroValues != undefined && PullCategoryNarroValues.length > 0) {
        narrowSearchFields += "CategoryID" + ",";
        narrowSearchValues += PullCategoryNarroValues + "@";
    }
    else {
        narrowSearchFields += "CategoryID" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (OrderSupplierNarroValues != undefined && OrderSupplierNarroValues.length > 0) {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += OrderSupplierNarroValues + "@";
    }
    else {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (OrderStatusNarroValues != undefined && OrderStatusNarroValues.length > 0) {
        narrowSearchFields += "OrderStatus" + ",";
        narrowSearchValues += OrderStatusNarroValues + "@";
    }
    else {
        narrowSearchFields += "OrderStatus" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (OrderRequiredDateNarroValues != undefined && OrderRequiredDateNarroValues.length > 0) {
        narrowSearchFields += "OrderRequiredDate" + ",";
        narrowSearchValues += OrderRequiredDateNarroValues + "@";
    }
    else {
        narrowSearchFields += "OrderRequiredDate" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////15//////////////////////////////////////////////////////////

    if (CostNarroSearchValue != undefined && CostNarroSearchValue.length > 0) {
        narrowSearchFields += "Cost" + ",";
        narrowSearchValues += CostNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "Cost" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////16//////////////////////////////////////////////////////////

    if (spendPerSpendLimit != undefined && spendPerSpendLimit.length > 0) {
        narrowSearchFields += "spendPerSpendLimit" + ",";
        narrowSearchValues += spendPerSpendLimit + "@";
    }
    else {
        narrowSearchFields += "spendPerSpendLimit" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////17//////////////////////////////////////////////////////////

    if (TotalSpendLimit != undefined && TotalSpendLimit.length > 0) {
        narrowSearchFields += "TotalSpendLimit" + ",";
        narrowSearchValues += TotalSpendLimit + "@";
    }
    else {
        narrowSearchFields += "TotalSpendLimit" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////18//////////////////////////////////////////////////////////

    if (TotalSpendRemaining != undefined && TotalSpendRemaining.length > 0) {
        narrowSearchFields += "TotalSpendRemaining" + ",";
        narrowSearchValues += TotalSpendRemaining + "@";
    }
    else {
        narrowSearchFields += "TotalSpendRemaining" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////19//////////////////////////////////////////////////////////

    if (TotalItemSpendLimit != undefined && TotalItemSpendLimit.length > 0) {
        narrowSearchFields += "TotalItemSpendLimit" + ",";
        narrowSearchValues += TotalItemSpendLimit + "@";
    }
    else {
        narrowSearchFields += "TotalItemSpendLimit" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////20//////////////////////////////////////////////////////////

    if (MatStagLocationsNarroValues != undefined && MatStagLocationsNarroValues.length > 0) {
        narrowSearchFields += "BinName" + ",";
        narrowSearchValues += MatStagLocationsNarroValues + "@";
    }
    else {
        narrowSearchFields += "BinName" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////21//////////////////////////////////////////////////////////

    if (SSNarroSearchValue != undefined && SSNarroSearchValue.length > 0) {
        narrowSearchFields += "SS" + ",";
        narrowSearchValues += SSNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "SS" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////22//////////////////////////////////////////////////////////

    if (ItemTypeNarroSearchValue != undefined && ItemTypeNarroSearchValue.length > 0) {
        narrowSearchFields += "ItemType" + ",";
        narrowSearchValues += ItemTypeNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "ItemType" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////23//////////////////////////////////////////////////////////

    if (CartSupplierNarroValues != undefined && CartSupplierNarroValues.length > 0) {
        narrowSearchFields += "SupplierName" + ",";
        narrowSearchValues += CartSupplierNarroValues + "@";
    }
    else {
        narrowSearchFields += "SupplierName" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////24//////////////////////////////////////////////////////////

    if (CartRTNarroValues != undefined && CartRTNarroValues.length > 0) {
        narrowSearchFields += "ReplenishType" + ",";
        narrowSearchValues += CartRTNarroValues + "@";
    }
    else {
        narrowSearchFields += "ReplenishType" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////25//////////////////////////////////////////////////////////

    if (RequisitionCurrentTab != undefined && RequisitionCurrentTab.length > 0) {

        narrowSearchFields += "RequisitionStatus" + ",";
        narrowSearchValues += RequisitionCurrentTab + "@";
    }
    else {

        narrowSearchFields += "RequisitionStatus" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////26//////////////////////////////////////////////////////////
    if (OrderNumberNarroValues != undefined && OrderNumberNarroValues.length > 0) {
        narrowSearchFields += "OrderNumber" + ",";
        narrowSearchValues += OrderNumberNarroValues + "@";
    }
    else {
        narrowSearchFields += "OrderNumber" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////27//////////////////////////////////////////////////////////
    if (LocationValue != undefined && LocationValue.length > 0) {
        narrowSearchFields += "LocationID" + ",";
        narrowSearchValues += LocationValue + "@";
    }
    else {
        narrowSearchFields += "LocationID" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////28//////////////////////////////////////////////////////////
    if (WorkOrderValue != undefined && WorkOrderValue.length > 0) {
        narrowSearchFields += "WorkOrderID" + ",";
        narrowSearchValues += WorkOrderValue + "@";
    }
    else {
        narrowSearchFields += "WorkOrderID" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////29//////////////////////////////////////////////////////////
    if (ToolCategoryValue != undefined && ToolCategoryValue.length > 0) {
        narrowSearchFields += "ToolCategoryID" + ",";
        narrowSearchValues += ToolCategoryValue + "@";
    }
    else {
        narrowSearchFields += "ToolCategoryID" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////30//////////////////////////////////////////////////////////
    if (ToolMaintanceValue != undefined && ToolMaintanceValue.length > 0) {
        narrowSearchFields += "ToolMaintanenceID" + ",";
        narrowSearchValues += ToolMaintanceValue + "@";
    }
    else {
        narrowSearchFields += "ToolMaintanenceID" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////31//////////////////////////////////////////////////////////
    if (ToolCostValue != undefined && ToolCostValue.length > 0) {
        narrowSearchFields += "Cost" + ",";
        narrowSearchValues += ToolCostValue + "@";
    }
    else {
        narrowSearchFields += "Cost" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////32//////////////////////////////////////////////////////////
    if (WOAssetValues != undefined && WOAssetValues.length > 0) {
        narrowSearchFields += "AssetID" + ",";
        narrowSearchValues += WOAssetValues + "@";
    }
    else {
        narrowSearchFields += "AssetID" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////33//////////////////////////////////////////////////////////
    if (WOCustomerValues != undefined && WOCustomerValues.length > 0) {
        narrowSearchFields += "CustomerID" + ",";
        narrowSearchValues += WOCustomerValues + "@";
    }
    else {
        narrowSearchFields += "CustomerID" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////34//////////////////////////////////////////////////////////
    if (WOTechnicianValues != undefined && WOTechnicianValues.length > 0) {
        narrowSearchFields += "TechnicianID" + ",";
        narrowSearchValues += WOTechnicianValues + "@";
    }
    else {
        narrowSearchFields += "TechnicianID" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////35//////////////////////////////////////////////////////////
    if (WOToolValues != undefined && WOToolValues.length > 0) {
        narrowSearchFields += "ToolID" + ",";
        narrowSearchValues += WOToolValues + "@";
    }
    else {
        narrowSearchFields += "ToolID" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////36//////////////////////////////////////////////////////////
    if (RoleUserTypeNarroValues != undefined && RoleUserTypeNarroValues.length > 0) {
        narrowSearchFields += "UserType" + ",";
        narrowSearchValues += RoleUserTypeNarroValues + "@";
    }
    else {
        narrowSearchFields += "UserType" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////37//////////////////////////////////////////////////////////
    if (ICountTypeNarroValues != undefined && ICountTypeNarroValues.length > 0) {
        narrowSearchFields += "CountType" + ",";
        narrowSearchValues += ICountTypeNarroValues + "@";
    }
    else {
        narrowSearchFields += "CountType" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////38//////////////////////////////////////////////////////////
    if (ICountStatusNarroValues != undefined && ICountStatusNarroValues.length > 0) {
        narrowSearchFields += "CountStatus" + ",";
        narrowSearchValues += ICountStatusNarroValues + "@";
    }
    else {
        narrowSearchFields += "CountStatus" + ",";
        narrowSearchValues += "@";
    }
    //narrowSearch = 'STARTWITH#' + narrowSearchItem;


    //////////////////////////////////////39//////////////////////////////////////////////////////////
    if (UserTypeNarroValues != undefined && UserTypeNarroValues.length > 0) {
        narrowSearchFields += "UserType" + ",";
        narrowSearchValues += UserTypeNarroValues + "@";
    }
    else {
        narrowSearchFields += "UserType" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////40//////////////////////////////////////////////////////////
    if (UserRoomNarroValues != undefined && UserRoomNarroValues.length > 0) {
        narrowSearchFields += "UserRoom" + ",";
        narrowSearchValues += UserRoomNarroValues + "@";
    }
    else {
        narrowSearchFields += "UserRoom" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////41//////////////////////////////////////////////////////////
    if (UserRoleNarroValues != undefined && UserRoleNarroValues.length > 0) {
        narrowSearchFields += "UserRole" + ",";
        narrowSearchValues += UserRoleNarroValues + "@";
    }
    else {
        narrowSearchFields += "UserRole" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////42//////////////////////////////////////////////////////////
    if (UserCompanyNarroValues != undefined && UserCompanyNarroValues.length > 0) {
        narrowSearchFields += "UserCompany" + ",";
        narrowSearchValues += UserCompanyNarroValues + "@";
    }
    else {
        narrowSearchFields += "UserCompany" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////43//////////////////////////////////////////////////////////
    if (RoleRoomNarroValues != undefined && RoleRoomNarroValues.length > 0) {
        narrowSearchFields += "RoleRoom" + ",";
        narrowSearchValues += RoleRoomNarroValues + "@";
    }
    else {
        narrowSearchFields += "RoleRoom" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////44//////////////////////////////////////////////////////////

    if (RoleCompanyNarroValues != undefined && RoleCompanyNarroValues.length > 0) {
        narrowSearchFields += "RoleCompany" + ",";
        narrowSearchValues += RoleCompanyNarroValues + "@";
    }
    else {
        narrowSearchFields += "RoleCompany" + ",";
        narrowSearchValues += "@";
    }

    ///////////////////////////////////45/////////////////////////////////////////////////////////////
    if (SchedulerItemSearchValue != undefined && SchedulerItemSearchValue.length > 0) {
        narrowSearchFields += "SchedulerItem" + ",";
        narrowSearchValues += SchedulerItemSearchValue + "@";
    }
    else {
        narrowSearchFields += "SchedulerItem" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////46//////////////////////////////////////////////////////////////
    if (SchedulerTypeSearchValue != undefined && SchedulerTypeSearchValue.length > 0) {
        narrowSearchFields += "SchedulerType" + ",";
        narrowSearchValues += SchedulerTypeSearchValue + "@";
    }
    else {
        narrowSearchFields += "SchedulerType" + ",";
        narrowSearchValues += "@";
    }
    if (WorkOrderNarroValues != undefined && WorkOrderNarroValues.length > 0) {

        narrowSearchFields += "WorkOrderName" + ",";
        narrowSearchValues += WorkOrderNarroValues + "@";
    }
    else {
        narrowSearchFields += "WorkOrderName" + ",";
        narrowSearchValues += "@";
    }

    //narrowSearch = 'STARTWITH#' + narrowSearchItem;
    if (narrowSearchValues.replace(/@/g, '') == '')
        NarrowSearchInGrid('');
    else {
        narrowSearch = narrowSearchFields + "[###]" + narrowSearchValues;

        if (narrowSearch.length > 10)
            NarrowSearchInGrid(narrowSearch);
        else if (UserCreatedNarroValues == undefined || UserCreatedNarroValues.length == 0 ||
             UserUpdatedNarroValues == undefined || UserUpdatedNarroValues.length == 0)
            NarrowSearchInGrid(narrowSearch);
    }

}


function UDFfillOptions(_UDFID, _drpID, _UDFColumnName, _Editable) {
    if (_UDFID > 0) {
        $.ajax({
            'url': '/UDF/GetUDFOptionsByUDF',
            data: { UDFID: _UDFID },
            cache: false,
            success: function (response) {
                $("#" + _drpID).empty();
                var s = '';
                s += '<option value="0"></option>';
                $.each(response.DDData, function (i, val) {
                    s += '<option value="' + val.ID + '">' + val.UDFOption + '</option>';
                });
                $("#" + _drpID).append(s);

                var txtvalue = $("#" + _UDFColumnName).val();
                if (txtvalue != null && txtvalue.trim() != '')
                    $('#' + _drpID + ' option:contains("' + txtvalue + '")').attr("selected", "selected");

                UDFtoggleControls(_drpID, _UDFColumnName, _UDFID, _Editable);

            },
            error: function () {
            }
        });
    }
}
function UDFappendScript(_UDFID, _drpID, _UDFColumnName, _Editable) {

    var script = '';
    script += '<script>';
    script += '$(document).ready(function () {';
    script += '$("#' + _drpID + '").change(function () {';
    script += '     UDFtoggleControls("' + _drpID + '","' + _UDFColumnName + '",' + _UDFID + ', ' + _Editable + ')';
    script += '});';
    script += '});';
    script += '<\/\script>';
    return script;
}
function UDFtoggleControls(_drpID, _UDFColumnName, _UDFID, _Editable) {

    $('#' + _UDFColumnName).val($('#' + _drpID + ' option:selected').text());
    var ID = $('#' + _drpID).val();
    if (ID > 0) {
        $("#newButton_" + _UDFID).val("Update");
        $("#delButton_" + _UDFID).css({ "display": "" });
    }
    else {
        $("#newButton_" + _UDFID).val("Add New");
        $("#delButton_" + _UDFID).css({ "display": "none" });
    }
    $("#newOption_" + _UDFID).val($("#" + _drpID + " option:selected").text());
    $("#newButton_" + _UDFID).removeAttr("onclick", "");
    $("#newButton_" + _UDFID).attr("onclick", "UDFcallUpdate(" + ID + ", " + _UDFID + ", '" + _UDFColumnName + "'," + _Editable + ")");
    $("#delButton_" + _UDFID).removeAttr("onclick", "");
    $("#delButton_" + _UDFID).attr("onclick", "UDFcallDelete(" + ID + ", " + _UDFID + ", '" + _UDFColumnName + "'," + _Editable + ")");
}

function UDFcallUpdate(_ID, _UDFID, _UDFColumnName, _Editable) {
    var _UDFOption = $("#newOption_" + _UDFID).val();
    var actionURL;
    var params;

    //Edit Mode - UDF
    if (_UDFID > 0) {

        if (_UDFOption.trim().length > 0) {

            if (_ID > 0) {
                //Edit Mode - UDFOption
                actionURL = '/UDF/EditUDFOption';
                params = { ID: _ID, UDFID: _UDFID, UDFOption: _UDFOption }
            }
            else {
                //Add Mode  - UDFOption
                actionURL = '/UDF/InsertUDFOption';
                params = { ID: _ID, UDFID: _UDFID, UDFOption: _UDFOption }
            }

            $.ajax({
                'url': actionURL,
                data: params,
                success: function (response) {
                    if (response.Response == "duplicate") {
                        $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
                        $("#spanGlobalMessage").text('Option ' + _UDFOption + ' already exist! Try with Another!');
                        showNotificationDialog();
                    }
                    else {

                        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        $("#spanGlobalMessage").text('Option saved successfully');
                        showNotificationDialog();

                        //Refresh the droddown here 
                        $('#' + _UDFColumnName).val(_UDFOption);
                        UDFfillOptions(_UDFID, 'drp_' + _UDFID, _UDFColumnName, _Editable);
                    }
                }
            });
        }
        else {
            alert('Option can not be blank');
        }
    }
    else {
        //Add Mode - UDF
        alert('Please save UDF first');
    }
}

function UDFcallDelete(_ID, UDFID, _UDFColumnName, _Editable) {
    if (_ID > 0) {
        //Delete Mode
        actionURL = '/UDF/DeleteUDFOption';
        params = { ID: _ID }
        $.ajax({
            'url': actionURL,
            data: params,
            success: function (response) {
                UDFfillOptions(UDFID, 'drp_' + UDFID, _UDFColumnName, _Editable);
            }
        });
    }
}
var GetColumnIndex = function (ColumnName) {

    return objColumns[ColumnName];
};

function UpdateColumnOrder(_ListName) {
    $.ajax({
        "url": '/Master/LoadGridState',
        data: { ListName: _ListName },
        cache: false,
        "dataType": "json",
        "success": function (json) {
            if (json.jsonData != '') {
                o = JSON.parse(json.jsonData);
                oTable.fnSettings().oLoadedState = $.extend(true, {}, o);

                //update the order of columns
                var _Order = $('#ColumnSortable').sortable("toArray");
                var __Order = _Order.toString().split(",");
                for (var i = 0; i < __Order.length; i++) {
                    __Order[i] = parseInt(__Order[i], 10);
                }
                oTable.fnSettings().oLoadedState.ColReorder = __Order;

                //update the visibility of columns
                var _abVisCols = [];
                for (i = 0, iLen = oTable.fnSettings().aoColumns.length; i < iLen; i++) {
                    var checked = $("#" + i + "_").is(":checked");
                    _abVisCols.push(checked);
                }
                oTable.fnSettings().oLoadedState.abVisCols = _abVisCols;

                //update the state to the database
                $.ajax({
                    "url": '/Master/SaveGridState',
                    //data: { Data: JSON.stringify(oTable.fnSettings().oLoadedState), ListName: 'ToolList' },
                    data: { Data: JSON.stringify(oTable.fnSettings().oLoadedState), ListName: _ListName },
                    "dataType": "json",
                    cache: false,
                    "success": function (json) {
                        o = json;
                        //refresh the page
                        window.location.reload(true);
                    }
                });


            }
        }
    });


}
function GenerateColumnSortable() {
    //clear the old elements     
    $('#ColumnSortable li').each(function (index) {
        $(this).remove();
    });
    //get current columns order and generate li sortable accordingly
    for (i = 0, iLen = oTable.fnSettings().aoColumns.length; i < iLen; i++) {
        var oColumn = oTable.fnSettings().aoColumns[i];
        var style = '';

        if (oColumn.sClass.indexOf('NotHide') >= 0) {
            //style = ' style="display:none" ';
            style = ' disabled="disabled" ';
        }

        var colindxbyname = GetColumnIndex(oColumn.sTitle.trim());

        if (gblColumnsToHideinPopUp == 'True') {

            var li = document.createElement('li');
            li.id = colindxbyname;
            li.className = 'ui-state-default';
            li.innerHTML = oColumn.sTitle.trim();
            if (oColumn.bVisible || oColumn.sClass.indexOf('NotHide') >= 0)
                li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + GetColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
            else {
                li.innerHTML = '<input type="checkbox" class="checkBox" id="' + GetColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
            }
            $('#ColumnSortable').append(li);
        }
        else {

            if (jQuery.inArray(colindxbyname, ColumnsToHideinPopUp) < 0) {
                var li = document.createElement('li');
                li.id = colindxbyname;
                li.className = 'ui-state-default';
                li.innerHTML = oColumn.sTitle.trim();
                if (oColumn.bVisible || oColumn.sClass.indexOf('NotHide') >= 0) {
                    li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + GetColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
                }
                else {
                    li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" id="' + GetColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
                }

                $('#ColumnSortable').append(li);
            }
            else {
                var li = document.createElement('li');
                li.id = colindxbyname;
                li.className = 'ui-state-default';
                li.innerHTML = oColumn.sTitle.trim();
                li.innerHTML = '<input type="checkbox" disabled="disabled" class="checkBox" id="' + GetColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
                $(li).hide();
                $('#ColumnSortable').append(li);
            }
        }

    }
}


$("#myDataTable").live('contextmenu', function (e) {

    if ($("body").hasClass('DTTT_Print')) {
        e.preventDefault();
        return false;
    }

    //if ($('#tab5')[0].className == "verticalText selected") {
    var x = e.pageX - this.offsetLeft;
    var y = e.pageY - this.offsetTop;
    $('#divContextMenu').css({ 'top': e.pageY, 'left': e.pageX }).fadeIn('slow');
    e.preventDefault();
    // }
});



function TabItemClickedbyContext(action, frmName) {
    $('#tab1').click();
    $('#divContextMenu').fadeOut('slow');
}



function DoNarrowSearchIM() {
    var narrowSearchFields = '';
    var narrowSearchValues = '';
    var narrowSearchItem = '';

    if (UserCreatedNarroValues != undefined && UserCreatedNarroValues.length > 0) {
        //narrowSearchItem += "[###]CreatedBy#" + UserCreatedNarroValues;
        narrowSearchFields += "CreatedBy" + ",";
        narrowSearchValues += UserCreatedNarroValues + "@";
    }
    else {
        narrowSearchFields += "CreatedBy" + ",";
        narrowSearchValues += "@";
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUpdatedNarroValues != undefined && UserUpdatedNarroValues.length > 0) {
        //narrowSearchItem += "[###]UpdatedBy#" + UserUpdatedNarroValues;
        narrowSearchFields += "UpdatedBy" + ",";
        narrowSearchValues += UserUpdatedNarroValues + "@";
    }
    else {
        narrowSearchFields += "UpdatedBy" + ",";
        narrowSearchValues += "@";
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    if ($('#DateCFromIM').val() != '' && $('#DateCToIM').val() != '') {
        //narrowSearchItem += "[###]DateCreatedFrom#" + GetDateInYYYYMMDDFormat($('#DateCFrom').val()) + "#DateCreatedTo#" + GetDateInYYYYMMDDFormat($('#DateCTo').val());        
        narrowSearchFields += "DateCreatedFrom" + ",";
        narrowSearchValues += GetDateInYYYYMMDDFormat($('#DateCFromIM').val()) + "," + GetDateInYYYYMMDDFormat($('#DateCToIM').val()) + "@";
    }
    else {
        narrowSearchFields += "DateCreatedFrom" + ",";
        narrowSearchValues += "@";
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////
    if ($('#DateUFromIM').val() != '' && $('#DateUToIM').val() != '') {
        //narrowSearchItem += "[###]DateUpdatedFrom#" + GetDateInYYYYMMDDFormat($('#DateUFrom').val()) + "#DateUpdatedTo#" + GetDateInYYYYMMDDFormat($('#DateUTo').val());
        narrowSearchFields += "DateUpdatedFrom" + ",";
        narrowSearchValues += GetDateInYYYYMMDDFormat($('#DateUFromIM').val()) + "," + GetDateInYYYYMMDDFormat($('#DateUToIM').val()) + "@";
    }
    else {
        narrowSearchFields += "DateUpdatedFrom" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF1NarrowValues != undefined && UserUDF1NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF1#" + UserUDF1NarrowValues;
        narrowSearchFields += "UDF1" + ",";
        narrowSearchValues += UserUDF1NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF1" + ",";
        narrowSearchValues += "@";
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF2NarrowValues != undefined && UserUDF2NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF2#" + UserUDF2NarrowValues;
        narrowSearchFields += "UDF2" + ",";
        narrowSearchValues += UserUDF2NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF2" + ",";
        narrowSearchValues += "@";
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF3NarrowValues != undefined && UserUDF3NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF3#" + UserUDF3NarrowValues;
        narrowSearchFields += "UDF3" + ",";
        narrowSearchValues += UserUDF3NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF3" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF4NarrowValues != undefined && UserUDF4NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF4#" + UserUDF4NarrowValues;
        narrowSearchFields += "UDF4" + ",";
        narrowSearchValues += UserUDF4NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF4" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF5NarrowValues != undefined && UserUDF5NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF5#" + UserUDF5NarrowValues;
        narrowSearchFields += "UDF5" + ",";
        narrowSearchValues += UserUDF5NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF5" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////////////////////////////////////////////
    if (PullSupplierNarroValues != undefined && PullSupplierNarroValues.length > 0) {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += PullSupplierNarroValues + "@";
    }
    else {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////////////////////////////////////////////////////////////
    if (ManufacturerNarroValues != undefined && ManufacturerNarroValues.length > 0) {
        narrowSearchFields += "ManufacturerID" + ",";
        narrowSearchValues += ManufacturerNarroValues + "@";
    }
    else {
        narrowSearchFields += "ManufacturerID" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////////////////////////////////////////////////////////////
    if (PullCategoryNarroValues != undefined && PullCategoryNarroValues.length > 0) {
        narrowSearchFields += "CategoryID" + ",";
        narrowSearchValues += PullCategoryNarroValues + "@";
    }
    else {
        narrowSearchFields += "CategoryID" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (OrderSupplierNarroValues != undefined && OrderSupplierNarroValues.length > 0) {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += OrderSupplierNarroValues + "@";
    }
    else {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (OrderStatusNarroValues != undefined && OrderStatusNarroValues.length > 0) {
        narrowSearchFields += "OrderStatus" + ",";
        narrowSearchValues += OrderStatusNarroValues + "@";
    }
    else {
        narrowSearchFields += "OrderStatus" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (OrderRequiredDateNarroValues != undefined && OrderRequiredDateNarroValues.length > 0) {
        narrowSearchFields += "OrderRequiredDate" + ",";
        narrowSearchValues += OrderRequiredDateNarroValues + "@";
    }
    else {
        narrowSearchFields += "OrderRequiredDate" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////15//////////////////////////////////////////////////////////

    if (CostNarroSearchValue != undefined && CostNarroSearchValue.length > 0) {
        narrowSearchFields += "Cost" + ",";
        narrowSearchValues += CostNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "Cost" + ",";
        narrowSearchValues += "@";
    }


    //////////////////////////////////////16//////////////////////////////////////////////////////////

    if (spendPerSpendLimit != undefined && spendPerSpendLimit.length > 0) {
        narrowSearchFields += "spendPerSpendLimit" + ",";
        narrowSearchValues += spendPerSpendLimit + "@";
    }
    else {
        narrowSearchFields += "spendPerSpendLimit" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////17//////////////////////////////////////////////////////////

    if (TotalSpendLimit != undefined && TotalSpendLimit.length > 0) {
        narrowSearchFields += "TotalSpendLimit" + ",";
        narrowSearchValues += TotalSpendLimit + "@";
    }
    else {
        narrowSearchFields += "TotalSpendLimit" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////18//////////////////////////////////////////////////////////

    if (TotalSpendRemaining != undefined && TotalSpendRemaining.length > 0) {
        narrowSearchFields += "TotalSpendRemaining" + ",";
        narrowSearchValues += TotalSpendRemaining + "@";
    }
    else {
        narrowSearchFields += "TotalSpendRemaining" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////19//////////////////////////////////////////////////////////

    if (TotalItemSpendLimit != undefined && TotalItemSpendLimit.length > 0) {
        narrowSearchFields += "TotalItemSpendLimit" + ",";
        narrowSearchValues += TotalItemSpendLimit + "@";
    }
    else {
        narrowSearchFields += "TotalItemSpendLimit" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////20//////////////////////////////////////////////////////////

    if (MatStagLocationsNarroValues != undefined && MatStagLocationsNarroValues.length > 0) {
        narrowSearchFields += "BinName" + ",";
        narrowSearchValues += MatStagLocationsNarroValues + "@";
    }
    else {
        narrowSearchFields += "BinName" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////21//////////////////////////////////////////////////////////

    if (SSNarroSearchValue != undefined && SSNarroSearchValue.length > 0) {
        narrowSearchFields += "SS" + ",";
        narrowSearchValues += SSNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "SS" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////22//////////////////////////////////////////////////////////

    if (ItemTypeNarroSearchValue != undefined && ItemTypeNarroSearchValue.length > 0) {
        narrowSearchFields += "ItemType" + ",";
        narrowSearchValues += ItemTypeNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "ItemType" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////23//////////////////////////////////////////////////////////

    if (CartSupplierNarroValues != undefined && CartSupplierNarroValues.length > 0) {
        narrowSearchFields += "SupplierName" + ",";
        narrowSearchValues += CartSupplierNarroValues + "@";
    }
    else {
        narrowSearchFields += "SupplierName" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////24//////////////////////////////////////////////////////////

    if (CartRTNarroValues != undefined && CartRTNarroValues.length > 0) {
        narrowSearchFields += "ReplenishType" + ",";
        narrowSearchValues += CartRTNarroValues + "@";
    }
    else {
        narrowSearchFields += "ReplenishType" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////25//////////////////////////////////////////////////////////

    if (ICLNarroValues != undefined && ICLNarroValues.length > 0) {
        narrowSearchFields += "BinId" + ",";
        narrowSearchValues += ICLNarroValues + "@";
    }
    else {
        narrowSearchFields += "BinId" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////26//////////////////////////////////////////////////////////

    if (StageLocationNarroValues != undefined && StageLocationNarroValues.length > 0) {
        narrowSearchFields += "MSID" + ",";
        narrowSearchValues += StageLocationNarroValues + "@";
        IsStagingLocationOnly = true;
    }
    else {
        narrowSearchFields += "MSID" + ",";
        narrowSearchValues += "@";
    }

    //narrowSearch = 'STARTWITH#' + narrowSearchItem;
    if (narrowSearchValues.replace(/@/g, '') == '')
        NarrowSearchInGridIM('');
    else {
        narrowSearch = narrowSearchFields + "[###]" + narrowSearchValues;

        if (narrowSearch.length > 10)
            NarrowSearchInGridIM(narrowSearch);
        else if (UserCreatedNarroValues == undefined || UserCreatedNarroValues.length == 0 ||
             UserUpdatedNarroValues == undefined || UserUpdatedNarroValues.length == 0)
            NarrowSearchInGridIM(narrowSearch);
    }
}

function NarrowSearchInGridIM(searchstr) {
    FilterStringGlobalUse = searchstr;
    $('#ItemModeDataTable').dataTable().fnFilter(searchstr, null, null, null)
}






(function ($) {


    $.fn.priceFormat = function (options) {

        var defaults =
		{
		    prefix: 'US$ ',
		    suffix: '',
		    centsSeparator: '.',
		    thousandsSeparator: ',',
		    limit: false,
		    centsLimit: 2,
		    clearPrefix: false,
		    clearSufix: false,
		    allowNegative: false,
		    insertPlusSign: false
		};

        var options = $.extend(defaults, options);

        return this.each(function () {

            // pre defined options
            var obj = $(this);
            var is_number = /[0-9]/;

            // load the pluggings settings
            var prefix = options.prefix;
            var suffix = options.suffix;
            var centsSeparator = options.centsSeparator;
            var thousandsSeparator = options.thousandsSeparator;
            var limit = options.limit;
            var centsLimit = options.centsLimit;
            var clearPrefix = options.clearPrefix;
            var clearSuffix = options.clearSuffix;
            var allowNegative = options.allowNegative;
            var insertPlusSign = options.insertPlusSign;

            // If insertPlusSign is on, it automatic turns on allowNegative, to work with Signs
            if (insertPlusSign) allowNegative = true;

            // skip everything that isn't a number
            // and also skip the left zeroes
            function to_numbers(str) {
                var formatted = '';
                for (var i = 0; i < (str.length); i++) {
                    char_ = str.charAt(i);
                    if (formatted.length == 0 && char_ == 0) char_ = false;

                    if (char_ && char_.match(is_number)) {
                        if (limit) {
                            if (formatted.length < limit) formatted = formatted + char_;
                        }
                        else {
                            formatted = formatted + char_;
                        }
                    }
                }

                return formatted;
            }

            // format to fill with zeros to complete cents chars
            function fill_with_zeroes(str) {
                while (str.length < (centsLimit + 1)) str = '0' + str;
                return str;
            }

            // format as price
            function price_format(str) {
                // formatting settings
                var formatted = fill_with_zeroes(to_numbers(str));
                var thousandsFormatted = '';
                var thousandsCount = 0;

                // Checking CentsLimit
                if (centsLimit == 0) {
                    centsSeparator = "";
                    centsVal = "";
                }

                // split integer from cents
                var centsVal = formatted.substr(formatted.length - centsLimit, centsLimit);
                var integerVal = formatted.substr(0, formatted.length - centsLimit);

                // apply cents pontuation
                formatted = (centsLimit == 0) ? integerVal : integerVal + centsSeparator + centsVal;

                // apply thousands pontuation
                if (thousandsSeparator || $.trim(thousandsSeparator) != "") {
                    for (var j = integerVal.length; j > 0; j--) {
                        char_ = integerVal.substr(j - 1, 1);
                        thousandsCount++;
                        if (thousandsCount % 3 == 0) char_ = thousandsSeparator + char_;
                        thousandsFormatted = char_ + thousandsFormatted;
                    }

                    //
                    if (thousandsFormatted.substr(0, 1) == thousandsSeparator) thousandsFormatted = thousandsFormatted.substring(1, thousandsFormatted.length);
                    formatted = (centsLimit == 0) ? thousandsFormatted : thousandsFormatted + centsSeparator + centsVal;
                }

                // if the string contains a dash, it is negative - add it to the begining (except for zero)
                if (allowNegative && (integerVal != 0 || centsVal != 0)) {
                    if (str.indexOf('-') != -1 && str.indexOf('+') < str.indexOf('-')) {
                        formatted = '-' + formatted;
                    }
                    else {
                        if (!insertPlusSign)
                            formatted = '' + formatted;
                        else
                            formatted = '+' + formatted;
                    }
                }

                // apply the prefix
                if (prefix) formatted = prefix + formatted;

                // apply the suffix
                if (suffix) formatted = formatted + suffix;

                return formatted;
            }

            // filter what user type (only numbers and functional keys)
            function key_check(e) {
                var code = (e.keyCode ? e.keyCode : e.which);
                var typed = String.fromCharCode(code);
                var functional = false;
                var str = obj.val();
                var newValue = price_format(str + typed);

                // allow key numbers, 0 to 9
                if ((code >= 48 && code <= 57) || (code >= 96 && code <= 105)) functional = true;

                // check Backspace, Tab, Enter, Delete, and left/right arrows
                if (code == 8) functional = true;
                if (code == 9) functional = true;
                if (code == 13) functional = true;
                if (code == 46) functional = true;
                if (code == 37) functional = true;
                if (code == 39) functional = true;
                // Minus Sign, Plus Sign
                if (allowNegative && (code == 189 || code == 109)) functional = true;
                if (insertPlusSign && (code == 187 || code == 107)) functional = true;

                if (!functional) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (str != newValue) obj.val(newValue);
                }

            }

            // inster formatted price as a value of an input field
            function price_it() {
                var str = obj.val();
                var price = price_format(str);
                if (str != price) obj.val(price);
            }

            // Add prefix on focus
            function add_prefix() {
                var val = obj.val();
                obj.val(prefix + val);
            }

            function add_suffix() {
                var val = obj.val();
                obj.val(val + suffix);
            }

            // Clear prefix on blur if is set to true
            function clear_prefix() {
                if ($.trim(prefix) != '' && clearPrefix) {
                    var array = obj.val().split(prefix);
                    obj.val(array[1]);
                }
            }

            // Clear suffix on blur if is set to true
            function clear_suffix() {
                if ($.trim(suffix) != '' && clearSuffix) {
                    var array = obj.val().split(suffix);
                    obj.val(array[0]);
                }
            }

            // bind the actions
            $(this).bind('keydown.price_format', key_check);
            $(this).bind('keyup.price_format', price_it);
            $(this).bind('focusout.price_format', price_it);

            // Clear Prefix and Add Prefix
            if (clearPrefix) {
                $(this).bind('focusout.price_format', function () {
                    clear_prefix();
                });

                $(this).bind('focusin.price_format', function () {
                    add_prefix();
                });
            }

            // Clear Suffix and Add Suffix
            if (clearSuffix) {
                $(this).bind('focusout.price_format', function () {
                    clear_suffix();
                });

                $(this).bind('focusin.price_format', function () {
                    add_suffix();
                });
            }

            // If value has content
            if ($(this).val().length > 0) {
                price_it();
                clear_prefix();
                clear_suffix();
            }

        });

    };


    $.fn.unpriceFormat = function () {
        return $(this).unbind(".price_format");
    };


    $.fn.unmask = function () {

        var field = $(this).val();
        var result = "";

        for (var f in field) {
            if (!isNaN(field[f]) || field[f] == "-") result += field[f];
        }

        return result;
    };



})(jQuery);


function GetBoolInFormat(obj, val) {
    return val == null ? '' : (val == true ? 'Yes' : 'No');
}





function GetRequisitionNarrowSearchData(_TableName, _IsArchived, _IsDeleted, _RequisitionCurrentTab) {
    /////////   ////////////
    $.ajax({
        'url': '/Master/GetRequisitionRequiredDate',
        data: { TableName: 'OrderRequiredDate', TextFieldName: 'OrderRequiredDateText' },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (i, val) {
                s += '<option value="' + val.ID + '">' + val.Text + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#OrderRequiredDate").empty();
            $("#OrderRequiredDate").multiselect('destroy');
            $("#OrderRequiredDate").multiselectfilter('destroy');

            $("#OrderRequiredDate").append(s);
            $("#OrderRequiredDate").multiselect
            (
                {
                    noneSelectedText: 'Required Date', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Required Date ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                OrderRequiredDateNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });


    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'WorkOrder', IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: _RequisitionCurrentTab },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#ReqWorkOrder").empty();
            $("#ReqWorkOrder").multiselect('destroy');
            $("#ReqWorkOrder").multiselectfilter('destroy');

            $("#ReqWorkOrder").append(s);
            $("#ReqWorkOrder").multiselect
            (
                {
                    noneSelectedText: 'WorkOrder', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'WorkOrder ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ReqWorkOrderCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ReqWorkOrderCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ReqWorkOrderCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ReqWorkOrderCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ReqWorkOrderCollapse").text().indexOf(ui.text) == -1) {
                        $("#ReqWorkOrderCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ReqWorkOrderCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ReqWorkOrderCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ReqWorkOrderCollapse").html(text);
                    }
                    else
                        $("#ReqWorkOrderCollapse").html('');
                }
                WorkOrderNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ReqWorkOrderCollapse").text().trim() != '')
                    $("#ReqWorkOrderCollapse").show();
                else
                    $("#ReqWorkOrderCollapse").hide();


                if ($("#ReqWorkOrderCollapse").find('span').length <= 2) {
                    $("#ReqWorkOrderCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ReqWorkOrderCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });



    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Customer', IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: _RequisitionCurrentTab },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#ReqCustomer").empty();
            $("#ReqCustomer").multiselect('destroy');
            $("#ReqCustomer").multiselectfilter('destroy');

            $("#ReqCustomer").append(s);
            $("#ReqCustomer").multiselect
            (
                {
                    noneSelectedText: 'Customer', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Customer ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ReqCustomerCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ReqCustomerCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ReqCustomerCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ReqCustomerCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ReqCustomerCollapse").text().indexOf(ui.text) == -1) {
                        $("#ReqCustomerCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ReqCustomerCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ReqCustomerCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ReqCustomerCollapse").html(text);
                    }
                    else
                        $("#ReqCustomerCollapse").html('');
                }
                OrderSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ReqCustomerCollapse").text().trim() != '')
                    $("#ReqCustomerCollapse").show();
                else
                    $("#ReqCustomerCollapse").hide();


                if ($("#ReqCustomerCollapse").find('span').length <= 2) {
                    $("#ReqCustomerCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ReqCustomerCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });


}

function GetWONarrowSearchData(_TableName, _IsArchived, _IsDeleted) {
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Customer', IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: null },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#WOCustomer").empty();
            $("#WOCustomer").multiselect('destroy');
            $("#WOCustomer").multiselectfilter('destroy');

            $("#WOCustomer").append(s);
            $("#WOCustomer").multiselect
            (
                {
                    noneSelectedText: 'Customer ', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Customer ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#WOCustomerCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#WOCustomerCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#WOCustomerCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#WOCustomerCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#WOCustomerCollapse").text().indexOf(ui.text) == -1) {
                        $("#WOCustomerCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#WOCustomerCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#WOCustomerCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#WOCustomerCollapse").html(text);
                    }
                    else
                        $("#WOCustomerCollapse").html('');
                }
                WOCustomerValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#WOCustomerCollapse").text().trim() != '')
                    $("#WOCustomerCollapse").show();
                else
                    $("#WOCustomerCollapse").hide();


                if ($("#WOCustomerCollapse").find('span').length <= 2) {
                    $("#WOCustomerCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#WOCustomerCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Technician', IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: null },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#WOTechnician").empty();
            $("#WOTechnician").multiselect('destroy');
            $("#WOTechnician").multiselectfilter('destroy');

            $("#WOTechnician").append(s);
            $("#WOTechnician").multiselect
            (
                {
                    noneSelectedText: 'Technician', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Technician ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#WOTechnicianCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#WOTechnicianCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#WOTechnicianCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#WOTechnicianCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#WOTechnicianCollapse").text().indexOf(ui.text) == -1) {
                        $("#WOTechnicianCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#WOTechnicianCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#WOTechnicianCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#WOTechnicianCollapse").html(text);
                    }
                    else
                        $("#WOTechnicianCollapse").html('');
                }
                WOTechnicianValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#WOTechnicianCollapse").text().trim() != '')
                    $("#WOTechnicianCollapse").show();
                else
                    $("#WOTechnicianCollapse").hide();


                if ($("#WOTechnicianCollapse").find('span').length <= 2) {
                    $("#WOTechnicianCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#WOTechnicianCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Asset', IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: null },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#WOAsset").empty();
            $("#WOAsset").multiselect('destroy');
            $("#WOAsset").multiselectfilter('destroy');

            $("#WOAsset").append(s);
            $("#WOAsset").multiselect
            (
                {
                    noneSelectedText: 'Asset', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Asset ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#WOAssetCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#WOAssetCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#WOAssetCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#WOAssetCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#WOAssetCollapse").text().indexOf(ui.text) == -1) {
                        $("#WOAssetCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#WOAssetCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#WOAssetCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#WOAssetCollapse").html(text);
                    }
                    else
                        $("#WOAssetCollapse").html('');
                }
                WOAssetValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#WOAssetCollapse").text().trim() != '')
                    $("#WOAssetCollapse").show();
                else
                    $("#WOAssetCollapse").hide();


                if ($("#WOAssetCollapse").find('span').length <= 2) {
                    $("#WOAssetCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#WOAssetCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Tool', IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: null },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#WOTool").empty();
            $("#WOTool").multiselect('destroy');
            $("#WOTool").multiselectfilter('destroy');

            $("#WOTool").append(s);
            $("#WOTool").multiselect
            (
                {
                    noneSelectedText: 'Tool', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Tool ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#WOToolCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#WOToolCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#WOToolCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#WOToolCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#WOToolCollapse").text().indexOf(ui.text) == -1) {
                        $("#WOToolCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#WOToolCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#WOToolCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#WOToolCollapse").html(text);
                    }
                    else
                        $("#WOToolCollapse").html('');
                }
                WOToolValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#WOToolCollapse").text().trim() != '')
                    $("#WOToolCollapse").show();
                else
                    $("#WOToolCollapse").hide();


                if ($("#WOToolCollapse").find('span').length <= 2) {
                    $("#WOToolCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#WOToolCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}

function GetOrderNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {
    //    $.ajax({ 'url': '/Master/GetDDData',
    //        data: { TableName: 'SupplierMaster', TextFieldName: 'SupplierName' },
    //        success: function (response) {
    //            var s = '';
    //            $.each(response.DDData, function (i, val) {
    //                s += '<option value="' + val.ID + '">' + val.Text + '</option>';
    //            });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'SupplierName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#OrderSupplier").empty();
            $("#OrderSupplier").multiselect('destroy');
            $("#OrderSupplier").multiselectfilter('destroy');

            $("#OrderSupplier").append(s);
            $("#OrderSupplier").multiselect
            (
                {
                    noneSelectedText: 'Supplier', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Supplier ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#OrderSupplierCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#OrderSupplierCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#OrderSupplierCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#OrderSupplierCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#OrderSupplierCollapse").text().indexOf(ui.text) == -1) {
                        $("#OrderSupplierCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#OrderSupplierCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#OrderSupplierCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#OrderSupplierCollapse").html(text);
                    }
                    else
                        $("#OrderSupplierCollapse").html('');
                }
                OrderSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#OrderSupplierCollapse").text().trim() != '')
                    $("#OrderSupplierCollapse").show();
                else
                    $("#OrderSupplierCollapse").hide();


                if ($("#OrderSupplierCollapse").find('span').length <= 2) {
                    $("#OrderSupplierCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#OrderSupplierCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    /////////////////////
    //    $.ajax({ 'url': '/Master/GetOrderStatus',
    //        data: { TableName: 'OrderStatus', TextFieldName: 'OrderStatusText' },
    //        success: function (response) {
    //            var s = '';
    //            $.each(response.DDData, function (i, val) {
    //                s += '<option value="' + val.ID + '">' + val.Text + '</option>';
    //            });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'OrderStatus', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#OrderStatus").empty();
            $("#OrderStatus").multiselect('destroy');
            $("#OrderStatus").multiselectfilter('destroy');

            $("#OrderStatus").append(s);
            $("#OrderStatus").multiselect
            (
                {
                    noneSelectedText: 'Status', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Status ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#OrderStatusCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#OrderStatusCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#OrderStatusCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#OrderStatusCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#OrderStatusCollapse").text().indexOf(ui.text) == -1) {
                        $("#OrderStatusCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#OrderStatusCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#OrderStatusCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#OrderStatusCollapse").html(text);
                    }
                    else
                        $("#OrderStatusCollapse").html('');
                }
                OrderStatusNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#OrderStatusCollapse").text().trim() != '')
                    $("#OrderStatusCollapse").show();
                else
                    $("#OrderStatusCollapse").hide();


                if ($("#OrderStatusCollapse").find('span').length <= 2) {
                    $("#OrderStatusCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#OrderStatusCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });


    /////////////////////
    $.ajax({
        'url': '/Master/GetOrderRequiredDate',
        data: { TableName: 'OrderRequiredDate', TextFieldName: 'OrderRequiredDateText' },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (i, val) {
                s += '<option value="' + val.ID + '">' + val.Text + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#OrderRequiredDate").empty();
            $("#OrderRequiredDate").multiselect('destroy');
            $("#OrderRequiredDate").multiselectfilter('destroy');

            $("#OrderRequiredDate").append(s);
            $("#OrderRequiredDate").multiselect
            (
                {
                    noneSelectedText: 'Required Date', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Required Date ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        //                        $("#OrderStatusCollapse").html('');
                        //                        for (var i = 0; i <= ui.target.length - 1; i++) {
                        //                            if ($("#OrderStatusCollapse").text().indexOf(ui.target[i].text) == -1) {
                        //                                $("#OrderStatusCollapse").append("<span>" + ui.target[i].text + "</span>");
                        //                            }
                        //                        }
                        //                        $("#OrderStatusCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                //                if (ui.checked) {
                //                    if ($("#OrderStatusCollapse").text().indexOf(ui.text) == -1) {
                //                        $("#OrderStatusCollapse").append("<span>" + ui.text + "</span>");
                //                    }
                //                }
                //                else {
                //                    if (ui.checked == undefined) {
                //                        $("#OrderStatusCollapse").html('');
                //                    }
                //                    else if (!ui.checked) {
                //                        var text = $("#OrderStatusCollapse").html();
                //                        text = text.replace("<span>" + ui.text + "</span>", '');
                //                        $("#OrderStatusCollapse").html(text);
                //                    }
                //                    else
                //                        $("#OrderStatusCollapse").html('');
                //                }
                OrderRequiredDateNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                //                if ($("#OrderStatusCollapse").text().trim() != '')
                //                    $("#OrderStatusCollapse").show();
                //                else
                //                    $("#OrderStatusCollapse").hide();


                //                if ($("#OrderStatusCollapse").find('span').length <= 2) {
                //                    $("#OrderStatusCollapse").scrollTop(0).height(50);
                //                }
                //                else {
                //                    $("#OrderStatusCollapse").scrollTop(0).height(100);
                //                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}


function GetReceiveNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'OrderNumber', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[0] + '">' + ArrData[0] + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#ddlOrderNumber").empty();
            $("#ddlOrderNumber").multiselect('destroy');
            $("#ddlOrderNumber").multiselectfilter('destroy');

            $("#ddlOrderNumber").append(s);
            $("#ddlOrderNumber").multiselect
            (
                {
                    noneSelectedText: 'Order Number', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Order Number ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ddlOrderNumberCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ddlOrderNumberCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ddlOrderNumberCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ddlOrderNumberCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {

                    if ($("#ddlOrderNumberCollapse").text().indexOf(ui.text) == -1) {
                        $("#ddlOrderNumberCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {

                    if (ui.checked == undefined) {
                        $("#ddlOrderNumberCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ddlOrderNumberCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ddlOrderNumberCollapse").html(text);
                    }
                    else
                        $("#ddlOrderNumberCollapse").html('');
                }
                OrderNumberNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ddlOrderNumberCollapse").text().trim() != '')
                    $("#ddlOrderNumberCollapse").show();
                else
                    $("#ddlOrderNumberCollapse").hide();


                if ($("#ddlOrderNumberCollapse").find('span').length <= 2) {
                    $("#ddlOrderNumberCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ddlOrderNumberCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}

function OnlyNumeric(e, obj) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code == 13) {
        return false;
    }
    else if (code > 95 && code < 106) {
    }
    else if (code > 31 && (code < 48 || code > 57)) {
        return false;
    }
}



function changeFontSize(val) {
    $('.userContent').css('font-size', '' + val + '%')
    if (oTable != null && oTable != undefined)
        oTable.fnAdjustColumnSizing();
}


$(document).ready(function () {

    $('#ChangeViewToLarge').live('click', function () {
        changeFontSize(120);
    });

    $('#ChangeViewToMedium').live('click', function () {
        changeFontSize(100);
    });

    $('#ChangeViewToSmall').live('click', function () {
        changeFontSize(90);
    });






    $(document).ajaxComplete(function () {



        $("input[type=text]").click(function () {
            $(this).select();
            return false;
        });

        $("input[type=number]").click(function () {
            $(this).select();
            return false;
        });




        if (IsNarrowSearchRefreshRequired && gblActionName !== "QuickList") {
            IsNarrowSearchRefreshRequired = false;
            if (typeof RefreshNarrowSearchCommonly == 'function') {
                RefreshNarrowSearchCommonly();
            }

        }

        if ($("#Created") != undefined && $("#Created").length > 0 && $("#Updated") != undefined && $("#Updated").length > 0 && $("#Updated").val() != $("#Created").val()) {
            $(".infoBlock:first ul li:nth-child(2)").find('span:first').html(GetDateInFullFormatCustomized($("#Created")[0].value));
            $(".infoBlock:first ul li:nth-child(4)").find('span:first').html(GetDateInFullFormatCustomized($("#Updated")[0].value));
        }
        else if ($("#Created") != undefined && $("#Created").length > 0 && $("#LastUpdated") != undefined && $("#LastUpdated").length > 0) {
            if ($("#hiddenID").val() != "0") {
                $(".infoBlock:first ul li:nth-child(2)").find('span:first').html(GetDateInFullFormatCustomized($("#Created")[0].value));
                $(".infoBlock:first ul li:nth-child(4)").find('span:first').html(GetDateInFullFormatCustomized($("#LastUpdated")[0].value));
            }
        }
        else if ($(".infoBlock:first ul li:nth-child(2)").find('span:first').length > 0 && $(".infoBlock:first ul li:nth-child(4)").find('span:first').length > 0) {
            if ($("#hiddenID").val() != "0") {
                $(".infoBlock:first ul li:nth-child(2)").find('span:first').html(GetDateInFullFormatCustomized($(".infoBlock:first ul li:nth-child(2)").find('span:first').text()));
                $(".infoBlock:first ul li:nth-child(4)").find('span:first').html(GetDateInFullFormatCustomized($(".infoBlock:first ul li:nth-child(4)").find('span:first').text()));
            }
        }

        if (gblActionName.toLowerCase() == 'requisitionlist' && $("#txtRequiredDate") != undefined && $("#txtRequiredDate").length > 0) {
            $("#txtRequiredDate").val(GetDateInFullFormatCustomizedOnlyDate($("#txtRequiredDate").val()));
            if ($(".myDatePicker").length > 0) {
                $(".myDatePicker").each(function () {
                    $(this).val(GetDateInFullFormatCustomizedOnlyDate($(this).val()));
                });
            }
        }
        if (gblActionName.toLowerCase() == 'orderlist' && $("#txtRequiredDate") != undefined && $("#txtRequiredDate").length > 0) {
            $("#txtRequiredDate").val(GetDateInFullFormatCustomizedOnlyDate($("#txtRequiredDate").val()));
            if ($(".myDatePicker").length > 0) {
                $(".myDatePicker").each(function () {
                    $(this).val(GetDateInFullFormatCustomizedOnlyDate($(this).val()));
                });
            }
        }
        if (gblActionName.toLowerCase() == 'transferlist' && $("#txtRequiredDate") != undefined && $("#txtRequiredDate").length > 0) {
            $("#txtRequiredDate").val(GetDateInFullFormatCustomizedOnlyDate($("#txtRequiredDate").val()));
            if ($(".myDatePicker").length > 0) {
                $(".myDatePicker").each(function () {
                    $(this).val(GetDateInFullFormatCustomizedOnlyDate($(this).val()));
                });
            }
        }
    });
});





function ShowHideOrderTab() {
    if (HasOrderTab == true) {
        var anSelectedRows = fnGetSelected(oTable);
        if (anSelectedRows.length == 1)
            $(".tab12").show();
        else
            $(".tab12").hide();
    }
    else {
        $(".tab12").hide();
    }
}

function ShowHideSchedulerTab() {
    if (HasScheduleTab == true) {
        var anSelectedRows = fnGetSelected(oTable);
        if (anSelectedRows.length == 1) {
            $("#tabSchedule").show();
            $("#tabOdometer").show();
            $("#tabMaintenance").show();
            $("#tabScheduleList").show();
            $("#tabOdometerList").show();
        }
        else {
            $("#tabSchedule").hide();
            $("#tabOdometer").hide();
            $("#tabMaintenance").hide();
            $("#tabScheduleList").hide();
            $("#tabOdometerList").hide();
        }
    }
    else {
        $("#tabSchedule").hide();
        $("#tabOdometer").hide();
        $("#tabMaintenance").hide();
        $("#tabScheduleList").hide();
        $("#tabOdometerList").hide();
    }
}
function isDate(txtDate) {

    var currVal = GetDateInYYYYMMDDFormat(txtDate);
    if (currVal == '')
        return false;

    //Declare Regex  
    //var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
    var rxDatePattern = /^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/;
    var dtArray = currVal.match(rxDatePattern); // is format OK?

    if (dtArray == null)
        return false;

    //Checks for mm/dd/yyyy format.
    dtMonth = dtArray[3];
    dtDay = dtArray[5];
    dtYear = dtArray[1];

    if (dtMonth < 1 || dtMonth > 12)
        return false;
    else if (dtDay < 1 || dtDay > 31)
        return false;
    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
        return false;
    else if (dtMonth == 2) {
        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
        if (dtDay > 29 || (dtDay == 29 && !isleap))
            return false;
    }
    return true;
}

function clearNarrowSearchFilter() {


    eraseCookieforshift("selectstartindex");
    eraseCookieforshift("selectendindex");
    eraseCookieforshift("finalselectedarray");


    if (typeof ($("#ICountStatus").multiselect("getChecked").length) != undefined && $("#ICountStatus").multiselect("getChecked").length > 0) {
        $("#ICountStatus").multiselect("uncheckAll");
        $("#ICountStatusCollapse").html('');
    }
    if (typeof ($("#ICountType").multiselect("getChecked").length) != undefined && $("#ICountType").multiselect("getChecked").length > 0) {
        $("#ICountType").multiselect("uncheckAll");
        $("#ICountTypeCollapse").html('');
    }
    if (typeof ($("#UserCreated").multiselect("getChecked").length) != undefined && $("#UserCreated").multiselect("getChecked").length > 0) {
        $("#UserCreated").multiselect("uncheckAll");
        $("#UserCreatedCollapse").html('');
    }
    if (typeof ($("#MatStagLocations").multiselect("getChecked").length) != undefined && $("#MatStagLocations").multiselect("getChecked").length > 0) {
        $("#MatStagLocations").multiselect("uncheckAll");
        $("#MatStagLocationsCollapse").html('');
    }
    if (typeof ($("#UserUpdated").multiselect("getChecked").length) != undefined && $("#UserUpdated").multiselect("getChecked").length > 0) {
        $("#UserUpdated").multiselect("uncheckAll");
        $("#UserUpdatedCollapse").html('');
    }

    $("select[name='udflist']").each(function (index) {
        if (typeof ($(this).multiselect("getChecked").length) != undefined && $(this).multiselect("getChecked").length > 0) {
            var UDFUniqueID = this.getAttribute('UID');
            $(this).multiselect("uncheckAll");
            $('#' + UDFUniqueID + 'Collapse').html('');
        }
    });

    if (typeof ($("#UserRoom").multiselect("getChecked").length) != undefined && $("#UserRoom").multiselect("getChecked").length > 0) {
        $("#UserRoom").multiselect("uncheckAll");
        $("#UserRoomCollapse").html('');
    }
    if (typeof ($("#UserCompany").multiselect("getChecked").length) != undefined && $("#UserCompany").multiselect("getChecked").length > 0) {
        $("#UserCompany").multiselect("uncheckAll");
        $("#UserCompanyCollapse").html('');
    }
    if (typeof ($("#RoleRoom").multiselect("getChecked").length) != undefined && $("#RoleRoom").multiselect("getChecked").length > 0) {
        $("#RoleRoom").multiselect("uncheckAll");
        $("#RoleRoomCollapse").html('');
    }
    if (typeof ($("#RoleCompany").multiselect("getChecked").length) != undefined && $("#RoleCompany").multiselect("getChecked").length > 0) {
        $("#RoleCompany").multiselect("uncheckAll");
        $("#RoleCompanyCollapse").html('');
    }

    if (typeof ($("#UserType").multiselect("getChecked").length) != undefined && $("#UserType").multiselect("getChecked").length > 0) {
        $("#UserType").multiselect("uncheckAll");
        $("#UserTypeCollapse").html('');
    }
    if (typeof ($("#UserRole").multiselect("getChecked").length) != undefined && $("#UserRole").multiselect("getChecked").length > 0) {
        $("#UserRole").multiselect("uncheckAll");
        $("#UserRoleCollapse").html('');
    }


    // Role Master User Type
    if (typeof ($("#RoleUserType").multiselect("getChecked").length) != undefined && $("#RoleUserType").multiselect("getChecked").length > 0) {
        $("#RoleUserType").multiselect("uncheckAll");
        $("#RoleUserTypeCollapse").html('');
    }
    if (typeof ($("#PullSupplier").multiselect("getChecked").length) != undefined && $("#PullSupplier").multiselect("getChecked").length > 0) {
        $("#PullSupplier").multiselect("uncheckAll");
        $("#PullSupplierCollapse").html('');
    }

    if (typeof ($("#Manufacturer").multiselect("getChecked").length) != undefined && $("#Manufacturer").multiselect("getChecked").length > 0) {
        $("#Manufacturer").multiselect("uncheckAll");
        $("#ManufacturerCollapse").html('');
    }

    if (typeof ($("#PullCategory").multiselect("getChecked").length) != undefined && $("#PullCategory").multiselect("getChecked").length > 0) {
        $("#PullCategory").multiselect("uncheckAll");
        $("#PullCategoryCollapse").html('');
    }

    if (typeof ($("#OrderStatus").multiselect("getChecked").length) != undefined && $("#OrderStatus").multiselect("getChecked").length > 0) {
        $("#OrderStatus").multiselect("uncheckAll");
        $("#OrderStatusCollapse").html('');
    }

    if (typeof ($("#OrderRequiredDate").multiselect("getChecked").length) != undefined && $("#OrderRequiredDate").multiselect("getChecked").length > 0) {
        $("#OrderRequiredDate").multiselect("uncheckAll");
    }

    if (typeof ($("#OrderSupplier").multiselect("getChecked").length) != undefined && $("#OrderSupplier").multiselect("getChecked").length > 0) {
        $("#OrderSupplier").multiselect("uncheckAll");
        $("#OrderSupplierCollapse").html('');
    }

    if (typeof ($("#ReqCustomer").multiselect("getChecked").length) != undefined && $("#ReqCustomer").multiselect("getChecked").length > 0) {
        $("#ReqCustomer").multiselect("uncheckAll");
        $("#ReqCustomerCollapse").html('');
    }

    if (typeof ($("#ReqWorkOrder").multiselect("getChecked").length) != undefined && $("#ReqWorkOrder").multiselect("getChecked").length > 0) {
        $("#ReqWorkOrder").multiselect("uncheckAll");
        $("#ReqWorkOrderCollapse").html('');
    }


    if (typeof ($("#WOCustomer").multiselect("getChecked").length) != undefined && $("#WOCustomer").multiselect("getChecked").length > 0) {
        $("#WOCustomer").multiselect("uncheckAll");
        $("#WOCustomerCollapse").html('');
    }
    if (typeof ($("#WOTechnician").multiselect("getChecked").length) != undefined && $("#WOTechnician").multiselect("getChecked").length > 0) {
        $("#WOTechnician").multiselect("uncheckAll");
        $("#WOTechnicianCollapse").html('');
    }
    if (typeof ($("#WOAsset").multiselect("getChecked").length) != undefined && $("#WOAsset").multiselect("getChecked").length > 0) {
        $("#WOAsset").multiselect("uncheckAll");
        $("#WOAssetCollapse").html('');
    }
    if (typeof ($("#WOTool").multiselect("getChecked").length) != undefined && $("#WOTool").multiselect("getChecked").length > 0) {
        $("#WOTool").multiselect("uncheckAll");
        $("#WOToolCollapse").html('');
    }






    if ($('#DateCFrom').val() != '') $('#DateCFrom').val('');
    if ($('#DateCTo').val() != '') $('#DateCTo').val('');
    if ($('#DateUFrom').val() != '') $('#DateUFrom').val('');
    if ($('#DateUTo').val() != '') $('#DateUTo').val('');

    if ($('#PullCost') != undefined) {
        $('#PullCost').val('0_-1');
    }

    if ($('#StockStatus') != undefined) {
        $('#StockStatus').val('0');
    }

    if (typeof ($("#ItemTypeNarroDDL").multiselect("getChecked").length) != undefined && $("#ItemTypeNarroDDL").multiselect("getChecked").length > 0) {
        $("#ItemTypeNarroDDL").multiselect("uncheckAll");
        $("#ItemTypeCollapse").html('');
    }

}



function getCookie(name) {
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var i = 0;
    while (i < clen) {
        var j = i + alen;
        if (document.cookie.substring(i, j) == arg) {
            return getCookieVal(j);
        }
        i = document.cookie.indexOf(" ", i) + 1;
        if (i == 0) break;
    }
    return null;
}

function getCookieVal(offset) {
    var endstr = document.cookie.indexOf(";", offset);
    if (endstr == -1) { endstr = document.cookie.length; }
    return unescape(document.cookie.substring(offset, endstr));
}

function setCookie(name, value, days) {
    if (typeof days != "undefined") { //if set persistent cookie
        var expireDate = new Date();
        expireDate.setDate(expireDate.getDate() + days);
        document.cookie = name + "=" + value + "; path=/; expires=" + expireDate.toGMTString();
    }
    else //else if this is a session only cookie
        document.cookie = name + "=" + value + "; path=/";
}




function rememberUDFValues(pageName, recordID) {
    var objUDFCookies = {};
    if (parseInt(recordID) == 0) {
        $("input[name='rememberlist']").each(function (index) {
            var UDFUniqueID = this.getAttribute('UID');
            var UDFType = this.getAttribute('UTYPE');
            var ID = this.getAttribute('id').toString().replace("rem_", "");
            if ($(this).is(":checked")) {
                if (UDFType == 'input') {
                    objUDFCookies[UDFUniqueID] = $('#' + UDFUniqueID).val() + "";
                }
                else {
                    objUDFCookies[UDFUniqueID] = $('#drp_' + ID + ' option:selected').text() + "" + "";
                }
            }
            else {
                objUDFCookies[UDFUniqueID] = "";
            }
        });
        setCookie(pageName + '_UDFCookie', JSON.stringify(objUDFCookies), 30);
    }
}

function checkRememberUDFValues(pageName, recordID) {
    var objUDFCookies = {};
    if (parseInt(recordID) == 0) {
        objUDFCookies = JSON.parse(getCookie(pageName + '_UDFCookie'));
        $("input[name='rememberlist']").each(function (index) {
            var UDFUniqueID = this.getAttribute('UID');
            var UDFType = this.getAttribute('UTYPE');
            var ID = this.getAttribute('id').toString().replace("rem_", "");
            if (objUDFCookies != null) {
                var UDFCookieValue = objUDFCookies[UDFUniqueID];
                if (UDFCookieValue != null && UDFCookieValue != '') {
                    if (UDFType == 'input') {
                        $('#' + UDFUniqueID).val(UDFCookieValue);
                    }
                    else {
                        $('#' + UDFUniqueID).val(UDFCookieValue);
                        $('#drp_' + ID + ' option:contains("' + UDFCookieValue + '")').attr("selected", "selected");
                    }
                    $(this).attr("checked", "checked");
                }
                else {
                    $(this).removeAttr("checked");
                }
            }
            else {
                $(this).removeAttr("checked");
            }
        });
    }
    else {
        $("input[name='rememberlist']").each(function (index) {
            $(this).attr("style", "display:none");
        });
    }
}

function setDefaultUDFValues(pageName, recordID) {
    $.ajax({
        'url': '/UDF/GetUDFDataPageWiseJSON',
        data: { PageName: pageName },
        success: function (response) {
            //first set the data from UDF
            $.each(response.Response, function (i, obj) {
                //var ID = obj.UDFColumnName.toString().replace("UDF", "");
                var ID = obj.ID;
                if (obj.UDFControlType == 'Textbox')
                    $('#' + obj.UDFColumnName).val(obj.UDFDefaultValue);
                else {
                    $('#' + obj.UDFColumnName).val(obj.UDFDefaultValue);
                    $('#drp_' + ID + ' option:contains("' + obj.UDFDefaultValue + '")').attr("selected", "selected");
                }
            });
            //override the values from cookies
            checkRememberUDFValues(pageName, recordID);
        }
    });
}

function ShowHideRequisitionTab(TabID) {
    if (HasRequisitionTab) {
        LoadListData(TabID);
    }
}
function clearGlobalIfNotInFocus() {
    if ($(document.activeElement).attr('id') != 'global_filter')
        $("#global_filter").val('');
}
function CallShowHideData() {
    var showChar = 100;
    var ellipsestext = "...";
    var moretext = "more";
    var lesstext = "less";
    $('.more').each(function () {
        var content = $(this).html();
        if (content.length > showChar) {
            var c = content.substr(0, showChar);
            var h = content.substr(showChar - 1, content.length - showChar);
            var html = c + '<span class="moreellipses">' + ellipsestext + '&nbsp;</span><span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink" tabindex="-1">' + moretext + '</a></span>';
            $(this).html(html);
        }
    });
    $(".morelink").click(function () {
        if ($(this).hasClass("less")) {
            $(this).removeClass("less");
            $(this).html(moretext);
        } else {
            $(this).addClass("less");
            $(this).html(lesstext);
        }
        $(this).parent().prev().toggle();
        $(this).prev().toggle();
        return false;
    });
}

function GetNarroHTMLForItemType() {


    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: 'ItemMaster', TextFieldName: 'ItemType', IsArchived: false, IsDeleted: false },
        success: function (response) {
            var s = '';
            $("#ItemTypeCollapse").html('');
            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    if (i == "1")
                        s += '<option value="' + i + '"> Item (' + val + ')' + '</option>';
                    if (i == "2" && gblActionName.toLowerCase() != "itemmasterlist")
                        s += '<option value="' + i + '"> Quick List (' + val + ')' + '</option>';
                    if (i == "3")
                        s += '<option value="' + i + '"> Kit (' + val + ')' + '</option>';
                    if (i == "4")
                        s += '<option value="' + i + '"> Labor (' + val + ')' + '</option>';
                });
            }
            //Destroy widgets before reapplying the filter
            $("#ItemTypeNarroDDL").empty();
            $("#ItemTypeNarroDDL").multiselect('destroy');
            $("#ItemTypeNarroDDL").multiselectfilter('destroy');

            $("#ItemTypeNarroDDL").append(s);
            $("#ItemTypeNarroDDL").multiselect
            (
                {
                    noneSelectedText: ItemType, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return ItemType + ' ' + numChecked + ' ' + selected;
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ItemTypeCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ItemTypeCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ItemTypeCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ItemTypeCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ItemTypeCollapse").text().indexOf(ui.text) == -1) {
                        $("#ItemTypeCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ItemTypeCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ItemTypeCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ItemTypeCollapse").html(text);
                    }
                    else
                        $("#ItemTypeCollapse").html('');
                }
                ItemTypeNarroSearchValue = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ItemTypeCollapse").text().trim() != '')
                    $("#ItemTypeCollapse").show();
                else
                    $("#ItemTypeCollapse").hide();


                if ($("#ItemTypeCollapse").find('span').length <= 2) {
                    $("#ItemTypeCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ItemTypeCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

}


function GetNarroHTMLForBOMItemType() {


    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: 'BOMItemMaster', TextFieldName: 'ItemType', IsArchived: false, IsDeleted: false },
        success: function (response) {
            var s = '';
            $("#ItemTypeCollapse").html('');
            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    if (i == "1")
                        s += '<option value="' + i + '"> Item (' + val + ')' + '</option>';
                    //                    if (i == "2" && gblActionName.toLowerCase() != "itemmasterlist")
                    //                        s += '<option value="' + i + '"> Quick List (' + val + ')' + '</option>';
                    //                    if (i == "3")
                    //                        s += '<option value="' + i + '"> Kit (' + val + ')' + '</option>';
                    //                    if (i == "4")
                    //                        s += '<option value="' + i + '"> Labor (' + val + ')' + '</option>';
                });
            }
            //Destroy widgets before reapplying the filter
            $("#ItemTypeNarroDDL").empty();
            $("#ItemTypeNarroDDL").multiselect('destroy');
            $("#ItemTypeNarroDDL").multiselectfilter('destroy');

            $("#ItemTypeNarroDDL").append(s);
            $("#ItemTypeNarroDDL").multiselect
            (
                {
                    noneSelectedText: ItemType, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return ItemType + ' ' + numChecked + ' ' + selected;
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ItemTypeCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ItemTypeCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ItemTypeCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ItemTypeCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#ItemTypeCollapse").text().indexOf(ui.text) == -1) {
                        $("#ItemTypeCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#ItemTypeCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#ItemTypeCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#ItemTypeCollapse").html(text);
                    }
                    else
                        $("#ItemTypeCollapse").html('');
                }
                ItemTypeNarroSearchValue = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                if ($("#ItemTypeCollapse").text().trim() != '')
                    $("#ItemTypeCollapse").show();
                else
                    $("#ItemTypeCollapse").hide();


                if ($("#ItemTypeCollapse").find('span').length <= 2) {
                    $("#ItemTypeCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#ItemTypeCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

}

function onlyNumeric(event) {
    var charCode = (event.which) ? event.which : event.keyCode

    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;

    return true;

}

function HideOtherTabs() {

    // For General 
    if (HasInsertPermission) {
        $('#tab1').find("#spnTabName").text(NewNameRes);
    }
    else if (HasOnlyViewPermission) {
        $('#tab1').find("#spnTabName").text(ViewNameRes);
    }
    TabEnableDisable("#tab1", true);
    $('#tab6').hide();
    if (HasInsertPermission == "False") {
        $('#tab1').hide();
    }
    $("#tabTM").hide();
    $("#tabTS").hide();

    // For Scheduler/Maintance Asset/Tool
    $("#tabSchedule").hide();
    $("#tabScheduleList").hide();
    $("#tabOdometer").hide();
    $("#tabMaintenance").hide();

    // For Order 
    $("#tab11").hide();

}


function GetUserMasterNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'UserType', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#UserType").empty();
            $("#UserType").multiselect('destroy');
            $("#UserType").multiselectfilter('destroy');

            $("#UserType").append(s);
            $("#UserType").multiselect
            (
                {
                    noneSelectedText: 'User type', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'User type ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#UserTypeCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#UserTypeCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#UserTypeCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#UserTypeCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#UserTypeCollapse").text().indexOf(ui.text) == -1) {
                        $("#UserTypeCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#UserTypeCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#UserTypeCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#UserTypeCollapse").html(text);
                    }
                    else
                        $("#UserTypeCollapse").html('');
                }
                UserTypeNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#UserTypeCollapse").text().trim() != '')
                    $("#UserTypeCollapse").show();
                else
                    $("#UserTypeCollapse").hide();


                if ($("#UserTypeCollapse").find('span').length <= 2) {
                    $("#UserTypeCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#UserTypeCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'CompanyName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#UserCompany").empty();
            $("#UserCompany").multiselect('destroy');
            $("#UserCompany").multiselectfilter('destroy');

            $("#UserCompany").append(s);
            $("#UserCompany").multiselect
            (
                {
                    noneSelectedText: 'Company', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Company ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#UserCompanyCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#UserCompanyCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#UserCompanyCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#UserCompanyCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#UserCompanyCollapse").text().indexOf(ui.text) == -1) {
                        $("#UserCompanyCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#UserCompanyCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#UserCompanyCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#UserCompanyCollapse").html(text);
                    }
                    else
                        $("#UserCompanyCollapse").html('');
                }
                UserCompanyNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#UserCompanyCollapse").text().trim() != '')
                    $("#UserCompanyCollapse").show();
                else
                    $("#UserCompanyCollapse").hide();


                if ($("#UserCompanyCollapse").find('span').length <= 2) {
                    $("#UserCompanyCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#UserCompanyCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });


    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'RoomName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#UserRoom").empty();
            $("#UserRoom").multiselect('destroy');
            $("#UserRoom").multiselectfilter('destroy');

            $("#UserRoom").append(s);
            $("#UserRoom").multiselect
            (
                {
                    noneSelectedText: 'Room', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Room ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#UserRoomCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#UserRoomCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#UserRoomCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#UserRoomCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#UserRoomCollapse").text().indexOf(ui.text) == -1) {
                        $("#UserRoomCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#UserRoomCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#UserRoomCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#UserRoomCollapse").html(text);
                    }
                    else
                        $("#UserRoomCollapse").html('');
                }
                UserRoomNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#UserRoomCollapse").text().trim() != '')
                    $("#UserRoomCollapse").show();
                else
                    $("#UserRoomCollapse").hide();


                if ($("#UserRoomCollapse").find('span').length <= 2) {
                    $("#UserRoomCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#UserRoomCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'RoleName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#UserRole").empty();
            $("#UserRole").multiselect('destroy');
            $("#UserRole").multiselectfilter('destroy');

            $("#UserRole").append(s);
            $("#UserRole").multiselect
            (
                {
                    noneSelectedText: 'Role', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'role ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#UserRoleCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#UserRoleCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#UserRoleCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#UserRoleCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#UserRoleCollapse").text().indexOf(ui.text) == -1) {
                        $("#UserRoleCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#UserRoleCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#UserRoleCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#UserRoleCollapse").html(text);
                    }
                    else
                        $("#UserRoleCollapse").html('');
                }
                UserRoleNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#UserRoleCollapse").text().trim() != '')
                    $("#UserRoleCollapse").show();
                else
                    $("#UserRoleCollapse").hide();


                if ($("#UserRoleCollapse").find('span').length <= 2) {
                    $("#UserRoleCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#UserRoleCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });

}

function GetRoleMasterNarrowSearchData(_TableName, _IsArchived, _IsDeleted) {
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'UserType', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#RoleUserType").empty();
            $("#RoleUserType").multiselect('destroy');
            $("#RoleUserType").multiselectfilter('destroy');

            $("#RoleUserType").append(s);
            $("#RoleUserType").multiselect
            (
                {
                    noneSelectedText: 'Role type', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Role type ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#RoleUserTypeCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#RoleUserTypeCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#RoleUserTypeCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#RoleUserTypeCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#RoleUserTypeCollapse").text().indexOf(ui.text) == -1) {
                        $("#RoleUserTypeCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#RoleUserTypeCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#RoleUserTypeCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#RoleUserTypeCollapse").html(text);
                    }
                    else
                        $("#RoleUserTypeCollapse").html('');
                }
                RoleUserTypeNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#RoleUserTypeCollapse").text().trim() != '')
                    $("#RoleUserTypeCollapse").show();
                else
                    $("#RoleUserTypeCollapse").hide();


                if ($("#RoleUserTypeCollapse").find('span').length <= 2) {
                    $("#RoleUserTypeCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#RoleUserTypeCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'CompanyName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#RoleCompany").empty();
            $("#RoleCompany").multiselect('destroy');
            $("#RoleCompany").multiselectfilter('destroy');

            $("#RoleCompany").append(s);
            $("#RoleCompany").multiselect
            (
                {
                    noneSelectedText: 'Company', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Company ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#RoleCompanyCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#RoleCompanyCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#RoleCompanyCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#RoleCompanyCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#RoleCompanyCollapse").text().indexOf(ui.text) == -1) {
                        $("#RoleCompanyCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#RoleCompanyCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#RoleCompanyCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#RoleCompanyCollapse").html(text);
                    }
                    else
                        $("#RoleCompanyCollapse").html('');
                }
                RoleCompanyNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#RoleCompanyCollapse").text().trim() != '')
                    $("#RoleCompanyCollapse").show();
                else
                    $("#RoleCompanyCollapse").hide();


                if ($("#RoleCompanyCollapse").find('span').length <= 2) {
                    $("#RoleCompanyCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#RoleCompanyCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'RoomName', IsArchived: _IsArchived, IsDeleted: _IsDeleted },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#RoleRoom").empty();
            $("#RoleRoom").multiselect('destroy');
            $("#RoleRoom").multiselectfilter('destroy');

            $("#RoleRoom").append(s);
            $("#RoleRoom").multiselect
            (
                {
                    noneSelectedText: 'Room', selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return 'Room ' + numChecked + ' selected';
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#RoleRoomCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#RoleRoomCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#RoleRoomCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#RoleRoomCollapse").show();
                    }
                }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                if (ui.checked) {
                    if ($("#RoleRoomCollapse").text().indexOf(ui.text) == -1) {
                        $("#RoleRoomCollapse").append("<span>" + ui.text + "</span>");
                    }
                }
                else {
                    if (ui.checked == undefined) {
                        $("#RoleRoomCollapse").html('');
                    }
                    else if (!ui.checked) {
                        var text = $("#RoleRoomCollapse").html();
                        text = text.replace("<span>" + ui.text + "</span>", '');
                        $("#RoleRoomCollapse").html(text);
                    }
                    else
                        $("#RoleRoomCollapse").html('');
                }
                RoleRoomNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                    return input.value;
                })

                if ($("#RoleRoomCollapse").text().trim() != '')
                    $("#RoleRoomCollapse").show();
                else
                    $("#RoleRoomCollapse").hide();


                if ($("#RoleRoomCollapse").find('span').length <= 2) {
                    $("#RoleRoomCollapse").scrollTop(0).height(50);
                }
                else {
                    $("#RoleRoomCollapse").scrollTop(0).height(100);
                }
                clearGlobalIfNotInFocus();
                DoNarrowSearch();
            }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}

function SetFixWidthForGrid(RefDataTable) {
    var MaxLength = 0;
    var nNodesRefDataTable = RefDataTable.fnGetNodes();
    for (var i = 0; i < nNodesRefDataTable.length; i++) {
        var tempSelectControls = nNodesRefDataTable[i].getElementsByTagName('select');
        for (var j = 0; j <= tempSelectControls.length - 1; j++) {
            var tempWidth = $(tempSelectControls[j]).width();
            if (tempWidth > MaxLength)
                MaxLength = tempWidth;
        }
    }
    if (MaxLength > 0) {
        if (MaxLength > 175)
            MaxLength = 335;
        for (var i = 0; i < nNodesRefDataTable.length; i++) {
            var tempSelectControls = nNodesRefDataTable[i].getElementsByTagName('select');
            for (var j = 0; j <= tempSelectControls.length - 1; j++) {
                $(nNodesRefDataTable[i]).width(MaxLength);
                $(tempSelectControls[j]).width(MaxLength);
            }
        }
        if (IsColumnResizeFistTimeForControls) {
            $(RefDataTable).dataTable().fnAdjustColumnSizing();
            IsColumnResizeFistTimeForControls = false;
        }
    }
}
function ChangeImgSRC(CG, GridImage, ChartImage) {
    if (CG == 'Grid') {
        $("#" + GridImage).attr("src", "/content/images/list-view.png");
        $("#" + ChartImage).attr("src", "/content/images/chart-view.png");
    }
    else {
        $("#" + GridImage).attr("src", "/content/images/list-view-inactive.png");
        $("#" + ChartImage).attr("src", "/content/images/chart-view-active.png");
    }
}

function compareHistoryRows(oChangeLogTable) {
    try {
        if (oChangeLogTable != undefined) {
            $('#DivLoading').show();
            var nodes = oChangeLogTable.fnGetNodes();
            var nodeLength = nodes.length;
            if (parseInt(nodeLength) > 0) {
                var i;
                for (i = nodeLength - 1; i > 0; i--) {
                    var j;
                    var cellLength = nodes[i].cells.length;
                    for (j = 0; j <= cellLength - 1; j++) {
                        var current = getNodeText(nodes[i].cells[j]);  //innerHTML
                        var next = getNodeText(nodes[i - 1].cells[j]); //innerHTML
                        if (current != next) {
                            $(nodes[i - 1].cells[j]).css({ "background-color": "#fff34d" });
                        }
                    }
                }
            }
            $('#DivLoading').hide();
        }
    }
    catch (e) {
        $('#DivLoading').hide();
    }
}
function getNodeText(elem) {
    if ((elem.textContent) && (typeof (elem.textContent) != "undefined")) {
        return elem.textContent;
    }
    else if ((elem.innerText) && (typeof (elem.innerText) != "undefined")) {
        return elem.innerText;
    } else {
        return elem.innerHTML;
    }
}

function showNotificationDialog() {
    $('div#target').css("z-index", 100000);
    $('div#target').fadeToggle();
    $("div#target").delay(3000).fadeOut(200);
}

// added below line by Niraj , 
// for getting column name in sorting, last parameter added
function SetColumnSortingValues(OTableColumnObjects) {
    var stringSort = "";
    $($(OTableColumnObjects).get().reverse()).each(function () {
        if (this[3] != undefined) {
            if (stringSort.indexOf(this[3]) < 0) {
                stringSort += this[3] + "  " + this[1] + " ,"
            }
        }
    });
    if (stringSort != "") {
        stringSort = stringSort.substring(0, stringSort.length - 2);
        stringSort = stringSort.substring(0, stringSort.lastIndexOf(" "));
    }

    return stringSort;
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

//$('#aBarcodeLabels').click(function () {
function OpenBarcodeLabels(obj, moduleID) {
    if (moduleID > 0) {
        var hrf = $(obj).attr('href');
        var arrCols = new Array();
        var objCols = $('#myDataTable').dataTable().fnSettings().aoColumns;
        for (var i = 0; i <= objCols.length - 1; i++) {
            arrCols.push(objCols[i].mDataProp);
        }

        var sortValue = "";
        for (var i = 0; i <= $('#myDataTable').dataTable().fnSettings().aaSorting.length - 1; i++) {
            if (sortValue.length > 0)
                sortValue += ", "
            sortValue += arrCols[$('#myDataTable').dataTable().fnSettings().aaSorting[i][0]] + ' ' + $('#myDataTable').dataTable().fnSettings().aaSorting[i][1];
        }

        if ($.trim(sortValue).length <= 0) {
            sortValue = "ID Desc";
        }

        var anSelected = fnGetSelected($('#myDataTable').dataTable());

        var stringIDs = "";
        if (anSelected.length > 0) {
            for (var i = 0; i < anSelected.length; i++) {
                if (moduleID == 6) {
                    stringIDs = stringIDs + $(anSelected[i]).find('#spnOrderDetailID').text() + ",";
                }
                else {
                    stringIDs = stringIDs + anSelected[i].id + ",";
                }
            }
        }

        if (stringIDs.length <= 0) {
            stringIDs = '0';
        }

        //// Uncomment below two line for issue WI-155: Printing bar code labels: multiple copies of one item and which label on page
        $('#divLabelPrintPopup').data({ "IDs": stringIDs, "ModuleID": moduleID, "SortFields": sortValue }).dialog('open');
        return false;

        //  hrf = "/labelprinting/LoadLabelsInPDF?ModuleID=" + moduleID + "&IDs=" + stringIDs;

        //  $(obj).attr('href', hrf);

        //  return true;
    }
}



//    var moduleID = "";
//    
//    if (gblActionName.toLowerCase().indexOf("assetlist") >= 0) {
//    moduleID = "1";
//    }
//    else if (gblActionName.toLowerCase().indexOf("itemmasterlist") >= 0) {
//    moduleID = "2";
//    }
//    else if (gblActionName.toLowerCase().indexOf("kitlist") >= 0) {
//    moduleID = "3";
//    }
//    else if (gblActionName.toLowerCase().indexOf("orderlist") >= 0) {
//    moduleID = "4";
//    }
//    else if (gblActionName.toLowerCase().indexOf("quicklist") >= 0) {
//    moduleID = "5";
//    }
//    else if (gblActionName.toLowerCase().indexOf("receivelist") >= 0) {
//    moduleID = "6";
//    }
//    else if (gblActionName.toLowerCase().indexOf("materialstaginglist") >= 0) {
//    moduleID = "7";
//    }
//    else if (gblActionName.toLowerCase().indexOf("toollist") >= 0) {
//    moduleID = "8";
//    }
//    else if (gblActionName.toLowerCase().indexOf("transferlist") >= 0) {
//    moduleID = "9";
//    }


function FormatedCostQtyValues(objValue, objType) {
    // objType = 1 = Cost 2 = Qty 3 = As it is
    if (objType == 1) {

        var s = '', temp,
        num = parseFloat(objValue).toFixed(parseInt($('#hdCostcentsLimit').val(), 10)).toString().split('.'), n = num[0];
        while (n.length > 3) {
            temp = n.substring(n.length - 3);
            s = ',' + temp + s;
            n = n.slice(0, -3);
        }
        if (n) s = n + s;
        if (num[1]) s += '.' + num[1];
        return s;

        //  return objValue.toFixed(parseInt($('#hdCostcentsLimit').val(), 10)).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
    else if (objType == 4) {

        var s = '', temp,
        num = parseFloat(objValue).toFixed(parseInt($('#hdTurnUsageLimit').val(), 10)).toString().split('.'), n = num[0];

        while (n.length > 3) {
            temp = n.substring(n.length - 3);
            s = ',' + temp + s;
            n = n.slice(0, -3);
        }
        if (n) s = n + s;
        if (num[1]) s += '.' + num[1];
        return s;

        //  return objValue.toFixed(parseInt($('#hdCostcentsLimit').val(), 10)).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
    else if (objType == 2)
        return parseFloat(objValue).toFixed(parseInt($('#hdQuantitycentsLimit').val(), 10));  //.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    else
        return objValue;
}

var lastChecked;
$("#myDataTable").on("touchstart click", "tbody tr", function (e) {

    if (e.target.type == "checkbox" || e.target.type == "select" || e.target.type == "radio" || e.target.type == "button") {
        e.stopPropagation();
    } else {
        if (IsDeleteItemPictureViewRecord)
            $(this).parent().parent().parent().parent().parent().toggleClass('row_selected');
        else {

            if (!lastChecked) {
                lastChecked = this;
            }



            if (e.shiftKey) {
                var start = $('#myDataTable tbody tr').index(this);
                var end = $('#myDataTable tbody tr').index(lastChecked);

                for (i = Math.min(start, end); i <= Math.max(start, end); i++) {
                    if (!$('#myDataTable tbody tr').eq(i).hasClass('row_selected')) {
                        $('#myDataTable tbody tr').eq(i).addClass("row_selected");
                    }
                }
                if (window.getSelection) {
                    if (window.getSelection().empty) {  // Chrome
                        window.getSelection().empty();
                    } else if (window.getSelection().removeAllRanges) {  // Firefox
                        window.getSelection().removeAllRanges();
                    }
                } else if (document.selection) {  // IE?
                    document.selection.empty();
                }
            } else if ((e.metaKey || e.ctrlKey)) {
                $(this).toggleClass('row_selected');
            } else {
                $(this).toggleClass('row_selected');
            }

            lastChecked = this;
        }



        return false;
    }
});

function ExportData(ModuleName, Exporttype) {

    var anSelected = fnGetSelected(oTable);
    var stringIDs = "";
    var ReqCount = 0;


    var shiftselectedarray = readCookieforshift("finalselectedarray");
    if (shiftselectedarray != null) {
        stringIDs = shiftselectedarray;
    }

    if (stringIDs == "") {


        for (var i = 0; i <= anSelected.length - 1; i++) {
            if (gblActionName.toLowerCase() === "quicklist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#QuickListGUID')[0].value + ",";
            } else if (gblActionName.toLowerCase() === "workorderlist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#WorkOrderGUID')[0].value + ",";
            } else if (gblActionName.toLowerCase() === "toollist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#spnToolPKID').text() + ",";
            } else if (gblActionName.toLowerCase() == 'requisitionlist') {
                var SpanReqStatus = $(anSelected[i]).find('#spnRequisitionStatus').text();
                if (SpanReqStatus == "Unsubmitted" || SpanReqStatus == "Submittted") {
                    //stringIDs = stringIDs + anSelected[i].id + ",";
                    stringIDs = stringIDs + $(anSelected[i]).find('#RequisitionGUID')[0].value + ",";
                    ReqCount = ReqCount + 1;
                }
            } else if (gblActionName.toLowerCase() == "orderlist") {
                var IsDeleteable = $(anSelected[i]).find('#spnIsableToDelete').text();
                if (IsDeleteable == 'true') {
                    var orderID = $(anSelected[i]).find('#spnOrderMasterID').text();
                    stringIDs = stringIDs + orderID + ",";
                    ReqCount = ReqCount + 1;
                }
            } else if (gblActionName.toLowerCase() == "transferlist") {
                var IsDeleteable = $(anSelected[i]).find('#spnIsableToDelete').text();
                if (IsDeleteable == 'true') {
                    var masterID = $(anSelected[i]).find('#spnTransferMasterGUID').text();
                    stringIDs = stringIDs + masterID + ",";
                    ReqCount = ReqCount + 1;
                }
            } else if (gblActionName.toLowerCase() == "materialstaginglist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#hdnUniqueID').val() + ",";
                ReqCount = ReqCount + 1;
            } else if (gblActionName.toLowerCase() == "cartitemlist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#hdnUniqueID').val() + ",";
                ReqCount = ReqCount + 1;
            } else if (gblActionName.toLowerCase() == "rolelist") {
                stringIDs = stringIDs + $(anSelected[i]).find("input[name='hdnConcatedId']").val() + ",";
                ReqCount = ReqCount + 1;
            } else if (deleteURL.toLowerCase() == "/inventory/deletematerialstagingrecords") {
                stringIDs = stringIDs + $(anSelected[i]).find("input[type='hidden'][name='hdnUniqueID']").val() +
                ",";
            } else if (deleteURL.toLowerCase() == "/pull/deletepullmasterrecords") {
                var aPos = oTable.fnGetPosition($(anSelected[i])[0]);
                var aData = oTable.fnGetData(aPos);
                if (!(!isNaN(parseFloat(aData.ConsignedQuantity)) && parseFloat(aData.ConsignedQuantity) > 0 &&
                aData.Billing == 'Yes')) {
                    stringIDs = stringIDs + $(anSelected[i]).find('#spnPullMasterID').text() + ",";
                    ReqCount = ReqCount + 1;
                }
            } else if (deleteURL.toLowerCase() == "/inventory/deleterecords") {
                stringIDs = stringIDs + $(anSelected[i]).find('#ItemGUID')[0].value + ",";
            } else if (deleteURL.toLowerCase() == "/bom/deleterecords") {
                stringIDs = stringIDs + $(anSelected[i]).find('#ItemGUID')[0].value + ",";
            } else {
                stringIDs = stringIDs + anSelected[i].id + ",";
            }

        }
    }

    if (stringIDs.length > 0) {
        $.ajax({
            "url": '/Export/ExportModuleInfo',
            "data": { ExportModuleName: ModuleName, Ids: stringIDs, Type: Exporttype },
            "dataType": "json",
            "type": "POST",
            "success": function (response) {

                window.open("../../Downloads/" + response, "_self");

            },
            "error": function (response) {

                window.open("../../Downloads/" + response, "_self");

            }
        });
    }
    else {
        alert("Please select record to export.");
    }
}

function createCookieforshift(name, value, days) {
    var expires;
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = escape(name) + "=" + escape(value) + expires + "; path=/";
}
function readCookieforshift(name) {
    var nameEQ = escape(name) + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return unescape(c.substring(nameEQ.length, c.length));
    }
    return null;
}
function eraseCookieforshift(name) {
    createCookieforshift(name, "", -1);
}
function GetOnlyIdsForPassPagesForshift(PageName, IsSupportPage) {
    var sSource = "";
    var lowername = PageName.toLowerCase();
    switch (lowername) {
        case "suppliermaster":
            sSource = "GetSupplierList";
            break;
    }


    var StartiDisplayLength = oTable.fnSettings()._iDisplayStart;
    var EndDisplayLength = oTable.fnSettings()._iDisplayLength;

    ///////////////////////////////// Call to get Ids /////////////////////////// START
    oTable.fnSettings().oFeatures.bStateSave = false;
    oTable.fnSettings()._iDisplayStart = 0;
    oTable.fnSettings()._iDisplayLength = 9999999;

    var aoData = oTable._fnAjaxParameters();
    var arrCols = new Array();
    var objCols = oTable.fnSettings().aoColumns;
    for (var i = 0; i <= objCols.length - 1; i++) {
        arrCols.push(objCols[i].mDataProp);
    }
    for (var j = 0; j <= aoData.length - 1; j++) {
        if (aoData[j].name == "sColumns") {
            aoData[j].value = arrCols.join("|");
            break;
        }
    }
    if (oTable.fnSettings().aaSorting.length != 0)
        aoData.push({ "name": "SortingField", "value": oTable.fnSettings().aaSorting[0][3] });
    else
        aoData.push({ "name": "SortingField", "value": "0" });

    aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
    aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });

    $.ajax({
        "dataType": 'json',
        "type": "POST",
        "url": sSource,
        cache: false,
        "async": false,
        "data": aoData,
        "success": function (json) {
            var start = readCookieforshift("selectstartindex");
            var end = readCookieforshift("selectendindex");
            var array = "";
            if (json.aaData != '') {

                while (start != end) {
                    for (var i = 0; i <= json.aaData.length - 1; i++) {
                        if (json.aaData[i].ID == start) {
                            //array = array + json.aaData[i].ID + ",";
                            (array == "") ? array = json.aaData[i].ID : array = array + "," + json.aaData[i].ID;
                            start = json.aaData[i + 1].ID;
                            break;
                        }
                    }
                }
                //array = array + end;
                (array == "") ? array = end : array = array + "," + end;

                createCookieforshift("finalselectedarray", array, 1);
            }
        },
        error: function (response) {
            //return o;
        }
    });
    ///////////////////////////////// Call to get Ids /////////////////////////// END

    ///////////////////////////////// Set Original settings /////////////////////////// START
    oTable.fnSettings().oFeatures.bStateSave = true;
    oTable.fnSettings()._iDisplayStart = StartiDisplayLength;
    oTable.fnSettings()._iDisplayLength = EndDisplayLength;
    ///////////////////////////////// Set Original settings /////////////////////////// END
}
function OpenMoveMaterialPopupByItemGuid(objData) {
    $('#DivLoading').show();
    $("#divMoveMaterialPopup").empty();
    $.ajax({
        url: 'OpenPopupForMoveMaterial',
        type: 'POST',
        data: JSON.stringify(objData),
        dataType: 'text',
        contentType: 'application/json',
        success: function (result) {
            $("#divMoveMaterialPopup").html(result);
            $('#DivLoading').hide();
        },
        error: function (xhr) {
            $('#DivLoading').hide();
        }
    });
}
if (typeof String.prototype.trim !== 'function') {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    }
}


function ShowLocalReports(objKeyValue, Id) {

    var obj = { paras: objKeyValue, Id: Id.toString() }
    $.ajax({
        url: '/Master/SetReportParaDictionary',
        type: 'Post',
        data: JSON.stringify(obj),
        dataType: 'json',
        contentType: 'application/json',
        success: function (result) {
            window.open(result.ReportURL, "_blank");
            return;
        },
        error: function (xhr) {
        }
    });
}

function SaveLocalReportPDF(objKeyValue, RptID) {
    var obj = { paras: objKeyValue, ReportID: RptID }
    var obj1 = { ReportID: RptID }

    $.ajax({
        url: '/Master/SetPDFReportParaDictionary',
        type: 'Post',
        data: JSON.stringify(obj),
        dataType: 'json',
        contentType: 'application/json',
        success: function (result) {

            if (result.message == "ok") {

                $.ajax({
                    url: '/ReportBuilder/GeneratePDF',
                    type: 'Post',
                    data: JSON.stringify(obj1),
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (result) {

                    },
                    error: function (result) {
                    }
                });
            }

        },
        error: function (result) {
        }
    });
}




function GetGridHeaderColumnsObject(dataTableName, ReOrderTitle, ListName, ExecuteFunctionStringAfterReOrder) {
    var str = '{';
    $('#' + dataTableName + ' thead tr:first th').each(function (i) {
        if (i > 0)
            str += ',';
        str += '"' + $.trim($(this).text()) + '" : ' + i;
    });
    str += '}';
    var columns = $.parseJSON(str);
    $('#divReorderPopup').find('#hdnReOrderDataTableName').val(dataTableName);

    if (ReOrderTitle !== undefined)
        $('#divReorderPopup').find('#hdnReOrderTitle').val(ReOrderTitle);

    if (ListName !== undefined)
        $('#divReorderPopup').find('#hdnReOrderListName').val(ListName);

    if (ExecuteFunctionStringAfterReOrder !== undefined)
        $('#divReorderPopup').find('#hdnReOrderExecuteFunctionString').val(ExecuteFunctionStringAfterReOrder);


    return columns;

}

function GenerateAndShowGridColumnList(dataTableName) {
    var dtTable = $('#' + dataTableName).dataTable();

    for (i = 0, iLen = dtTable.fnSettings().aoColumns.length; i < iLen; i++) {
        var oColumn = dtTable.fnSettings().aoColumns[i];
        var style = '';
        if (oColumn.sClass.indexOf('NotHide') >= 0) {
            //style = ' style="display:none" ';
            style = ' disabled="disabled" ';
        }

        var li = document.createElement('li');
        li.id = objColumns[oColumn.sTitle.trim()];
        li.className = 'ui-state-default';
        li.innerHTML = oColumn.sTitle.trim();
        if (oColumn.bVisible || oColumn.sClass.indexOf('NotHide') >= 0) {
            li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + objColumns[oColumn.sTitle.trim()] + '_" />' + oColumn.sTitle.trim();
        }
        else
            li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" id="' + objColumns[oColumn.sTitle.trim()] + '_" />' + oColumn.sTitle.trim();
        $('#divReorderPopup').find('#ulColumnReOrder').append(li);
    }
}

$('#btnSaveGridColumnOrder').live('click', function () {
    var dataTableName = $('#divReorderPopup').find('#hdnReOrderDataTableName').val();
    var dataTableObject = $('#' + dataTableName).dataTable();
    var listName = $('#divReorderPopup').find('#hdnReOrderListName').val();
    var _Order = $('#divReorderPopup').find('#ulColumnReOrder').sortable("toArray");
    var executeFuncString = $('#divReorderPopup').find('#hdnReOrderExecuteFunctionString').val();
    var __Order = _Order.toString().split(",");

    for (var i = 0; i < __Order.length; i++) {
        __Order[i] = parseFloat(__Order[i]);
    }

    dataTableObject.fnSettings().oLoadedState.ColReorder = __Order;

    var _abVisCols = [];
    for (i = 0, iLen = dataTableObject.fnSettings().aoColumns.length; i < iLen; i++) {
        var checked = $("#" + i + "_").is(":checked");
        _abVisCols.push(checked);
    }

    dataTableObject.fnSettings().oLoadedState.abVisCols = _abVisCols;

    $.ajax({
        "url": '/Master/SaveGridState',
        "data": { Data: JSON.stringify(dataTableObject.fnSettings().oLoadedState), ListName: listName },
        "dataType": "json",
        "cache": false,
        "async": false,
        "success": function (json) {
            o = json;
            $('#divReorderPopup').dialog('close');
            if (executeFuncString.length > 0)
                eval(executeFuncString);
        }
    });

});

function ReportExecutionData(ModuleName) {

    var anSelected = fnGetSelected(oTable);
    var stringIDs = "";
    var ReqCount = 0;


    var shiftselectedarray = readCookieforshift("finalselectedarray");
    if (shiftselectedarray != null) {
        stringIDs = shiftselectedarray;
    }

    if (stringIDs == "") {


        for (var i = 0; i <= anSelected.length - 1; i++) {
            if (gblActionName.toLowerCase() === "workorderlist") {
                stringIDs = stringIDs + $(anSelected[i]).find('#WorkOrderGUID')[0].value + ",";
            } else if (gblActionName.toLowerCase() == "orderlist") {
                var IsDeleteable = $(anSelected[i]).find('#spnIsableToDelete').text();
                if (IsDeleteable == 'true') {
                    var orderID = $(anSelected[i]).find('#spnOrderMasterID').text();
                    stringIDs = stringIDs + orderID + ",";
                    ReqCount = ReqCount + 1;
                }
            }

        }
    }

    if (stringIDs.length > 0) {

        $.ajax({
            "url": '/ReportBuilder/ReportExecutionData',
            "data": { ModuleName: ModuleName, Ids: stringIDs },
            "dataType": "json",
            "type": "POST",
            "success": function (response) {

                window.open(response.ReqURL, "_blank");

            },
            "error": function (response) {

                window.open(response.ReqURL, "_blank");

            }
        });
    }
    else {
        alert("Please select record to Print.");
    }
}
function ReportExecutionSingleRecord(ModuleName, GUId) {

    var stringIDs = GUId;


    if (stringIDs.length > 0) {

        $.ajax({
            "url": '/ReportBuilder/ReportExecutionData',
            "data": { ModuleName: ModuleName, Ids: stringIDs },
            "dataType": "json",
            "type": "POST",
            "success": function (response) {

                window.open(response.ReqURL, "_blank");

            },
            "error": function (response) {

                window.open(response.ReqURL, "_blank");

            }
        });
    }
    else {
        alert("Please select record to Print.");
    }
}
