var oTableReorderId = '';
var oTableReorderColumnSetupFor = '';
var DashboardLiId = '';
var DashboardActiveTabs = [];
var openpopupwarn = false;
var timer = new Timer();
//timer.start({
//    callback: function (timer) {
//        $('#callbackExample .values').html(
//            'Hello, I am a callback and I am counting time: ' + timer.getTimeValues().toString(['hours', 'minutes', 'seconds', 'secondTenths'])
//        );
//    }
//});

timer.start();
timer.addEventListener('secondsUpdated', function (e) {
    if (typeof moment === 'undefined') {
        return;
    }
    if (timer.getTimeValues().seconds % 5 == 0) {
        var currentselection = GetCurrentselctedRoom();
        var currentselectionLatest = $.cookie('EPCMRM');
        if (currentselection != "0_0_0" && currentselection != currentselectionLatest) {
            setTimeout(function () {
                var RedirectURL = window.location.href;
                if (window.location.pathname.toLowerCase() != "/master/evmisetup"
                    && RedirectURL.indexOf("Import") <= 0
                    && RedirectURL.indexOf("ExecuteStoredProcedure") <= 0
                    && window.location.pathname.toLowerCase() != "/udf/udflist") {
                    if (window.location.href.indexOf("?") > -1) {
                        if (window.location.href.split('?').length > 0) {
                            if (window.location.href.split('?')[0] != "") {
                                window.location.href = window.location.href.split('?')[0];
                            }
                        }
                    }
                    else {
                        window.location.href = RedirectURL;
                        window.location.reload(true);
                        //var tmpHashKey = window.location.hash;
                        //if (tmpHashKey !== undefined && tmpHashKey != null && tmpHashKey.length > 0) {
                        //    window.location.reload(true);
                        //}
                    }
                }
                else {
                    if (window.location.pathname.toLowerCase() == "/master/evmisetup") {
                        window.location.href = window.location.pathname;
                    }
                    else {
                        if (RedirectURL.indexOf("Import") >= 0 || RedirectURL.indexOf("ExecuteStoredProcedure") > 0) {
                            window.location.href = RedirectURL;
                        }
                        else {
                            window.location.reload(true);
                        }
                    }
                }
            }, 1000);

        }
        var lasttimeexecution = moment($.cookie('lastajaxcallstarttime'), "YYYY-MM-DD HH:mm:ss Z");
        var idleminutes = moment.utc().diff(lasttimeexecution, 'minutes');
        if (idleminutes >= (IdleSessionTimout - IdleSessionTimoutwarning) && openpopupwarn == false) {
            openpopupwarn = true;
        }
        if (idleminutes >= IdleSessionTimout) {
            window.location.href = urlLogoutUser;
        }
    }
});
//timer.addEventListener('minutesUpdated', function (e) {
//    $('div#IdleTimer').html(timer.getTimeValues().toString());
//    $('#ConfirmReportDeleteModel').modal();
//});
function OpenReorderPopupByDataTableName(DataTableName, ColumnSetupFor) {

    DashboardActiveTabs = [];
    var LiResizable = $('#' + DataTableName).closest('.ui-resizable');
    DashboardLiId = $(LiResizable)[0].id;

    if (DataTableName == 'myInventoryDataTable')
        objColumns = objColumnsInventoryDataTable;
    else if (DataTableName == 'myOrderDataTable')
        objColumns = objColumnsMyOrderDataTable;
    else if (DataTableName == 'myReceiveDataTable')
        objColumns = objColumnsMyReceiveDataTable;
    else if (DataTableName == 'myTransferDataTable')
        objColumns = objColumnsMyTransferDataTable;
    else if (DataTableName == 'myRequisitionDataTable')
        objColumns = objColumnsMyRequisitionDataTable;
    else if (DataTableName == 'myProjectDataTable')
        objColumns = objColumnsMyProjectDataTable;
    else if (DataTableName == 'myAssetsMaintenanceDataTable')
        objColumns = objColumnsMyAssetsMaintenanceDataTable;
    else if (DataTableName == 'myToolsMaintenanceDataTable')
        objColumns = objColumnsMyToolsMaintenanceDataTable;
    else if (DataTableName == 'myCountDataTable')
        objColumns = objColumnsMyCountDataTable;
    else if (DataTableName == 'myCartDataTable')
        objColumns = objColumnsMyCartDataTable;

    $(LiResizable).find('.liahover').each(function () {
        DashboardActiveTabs.push($(this)[0].id);
    });

    oTableReorderId = DataTableName;
    oTableReorderColumnSetupFor = ColumnSetupFor;
    try {
        $("#ColumnSortableModal").dialog("destroy");
        $("#ColumnSortableModal").dialog({
            autoOpen: false,
            modal: true,
            width: 500,
            //title: "ReOrder Columns",
            title: strReorderColumnPopupHeader,
            draggable: true,
            resizable: true,
            create: function (event, ui) {
                $("#ColumnSortableModal").dialog("open");
            },
            open: function () {
                GenerateColumnSortable();
                $("#ColumnSortable").sortable({ axis: "y", containment: "parent" });
                $("#ColumnSortableModal")[0].style.height = '391px';
            },
            close: function () {

            }
        });
    }
    catch (ex) {
        alert(ex.message);
    }
}

function SelectAllByDataTableNam(DataTableName) {
    $("#" + DataTableName).find("tbody tr").removeClass("row_selected").addClass("row_selected");
    $('#' + DataTableName + '_SelectAll').css('display', 'none');
    $('#' + DataTableName + '_DeSelectAll').css('display', '');
}

function DeselectAllByDataTableNam(DataTableName) {
    $("#" + DataTableName).find("tbody tr").removeClass("row_selected");
    $('#' + DataTableName + '_SelectAll').css('display', '');
    $('#' + DataTableName + '_DeSelectAll').css('display', 'none');
}

$(document).ready(function () {

    if (window.location.href.indexOf("staging") < 0 && window.location.href.indexOf("amazonaws") < 0 && window.location.href.indexOf("14.") < 0 && window.location.href.indexOf("54.88.163") < 0 && window.location.href.indexOf("192.") < 0 && window.location.href.indexOf("localhost") < 0 && window.location.href.indexOf("demo") < 0 && window.location.protocol != 'https:') {
        location.href = location.href.replace("http://", "https://");
    }
    if ($("input#global_filter").length > 0) {
        $("input#global_filter").focus();
        setTimeout(function () {
            $("input#global_filter").focus();
        }, 600);
    }
    if ($("input.searchinput").length > 0) {
        $("input.searchinput").focus();
        setTimeout(function () {
            $("input.searchinput").focus();
        }, 600);

    }

    if (self == top) {
        var theBody = document.getElementsByTagName('body')[0];
        theBody.style.display = "block";

    }
    else {
        top.location = self.location;
    }

    //$.ajaxSetup({ cache: false });
    FillTopEnterprise();
    FillTopCompanies();
    FillTopRooms();
    FillCompanyLanguages();
    SetReplenishRedCount();
    SetConsumeRedCount();
    //if (varIsItemListView == 'True') {
    //    SetItemCountInMenu(); // will be set via ajax call
    //}
    var intervaltemp = setInterval("KeepSessionAlive()", interval);
    SetPageMode();
    SetRoomCockie();
    $('div.DivLoadingProcessing').css('height', $(document).height().toString() + 'px');
    //$("#ColumnOrderSetup").click(function () {
    //    $("#ColumnSortableModal").dialog("open");
    //    return false;
    //});
    var useractivelastaction;
    //$(document).on("mousemove keydown DOMMouseScroll mousewheel mousedown", function () {
    $(document).on("mousemove keydown DOMMouseScroll mousewheel ", function (e) {
        clearTimeout(useractivelastaction);
        useractivelastaction = setTimeout(function () {
            $.cookie('lastajaxcallstarttime', moment.utc().format("YYYY-MM-DD HH:mm:ss Z"), { path: '/', SameSite: 'Strict'  });
        }, 250);
    });

    $("a#ColumnOrderSetup").off("click");
    $(document).on("click", "a#ColumnOrderSetup", function () {
        $("#ColumnSortableModal").dialog("open");
    });

    $("a#MinMaxTableColumnOrderSetup").off("click");
    $(document).on("click", "a#MinMaxTableColumnOrderSetup", function () {
        $("#MinMaxColumnSortableModal").dialog("open");
    });

    $("a#TurnsTableColumnOrderSetup").off("click");
    $(document).on("click", "a#TurnsTableColumnOrderSetup", function () {
        $("#TurnsColumnSortableModal").dialog("open");
    });

    var previouslanguagevalue = "";
    $("#ddlCompanyLanguage").on('focus', function () {
        // Store the current value on focus and on change
        previouslanguagevalue = this.value;
    });
    $("#ddlCompanyLanguage").change(function () {
        //check for any input field value change and message show to user before navigate
        if (!dirtyCheck()) {
            this.value = previouslanguagevalue;
            return false;
        }
        else {
            previouslanguagevalue = this.value;
            removeDirtyclass();
        }
        //End navigate
        var _selected = $("#ddlCompanyLanguage").val();
        $.ajax({
            type: "POST"
            , url: urlCompanyLanguageChanged
            , data: "{'SelectedCulture':'" + _selected + "'}"
            , contentType: "application/json;"
            , dataType: "json"
            , success: function (msg) {
                window.location.reload(true);
            }
        });

    });
    //$("#btnPollAll").click(function () {
    //    $('#DivLoading').show();
    //    $.ajax({
    //        url: urlSetPollAllTrue,
    //        type: 'Get',
    //        data: {},
    //        dataType: 'json',
    //        async: false,
    //        success: function (response) {
    //            if (response.Status = "ok") {
    //                $('#DivLoading').hide();
    //                showNotificationDialog();
    //                $("#spanGlobalMessage").text(response.Message);
    //                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    //            }
    //            else if (response.Status == "fail") {
    //                $('#DivLoading').hide();
    //                showNotificationDialog();
    //                $("#spanGlobalMessage").text(response.Message);
    //                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');

    //            }

    //        },
    //        error: function (xhr) {
    //            $('#DivLoading').hide();
    //            showNotificationDialog();
    //            $("#spanGlobalMessage").text("Unknown error");
    //            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');

    //        }
    //    });


    //});

    var isMouseMoved = false;
    setInterval(function () {
        if (isMouseMoved) {
            $("#divContextMenu").hide();
        }
    }, 2500);

    $("#divContextMenu").mouseenter(function () {
        isMouseMoved = false;
    });

    $(document).mousedown(function (e) {
        if (e.button == 2) {
            isMouseMoved = false;
        }
    });

    $(document).mousemove(function (e) {
        var container = $("#divContextMenu");
        if (container.is(":visible") && (!container.is(e.target)) && container.has(e.target).length === 0) {
            isMouseMoved = true;
        } else {
            isMouseMoved = false;
        }
    });


    $("#ColumnOrderSetup_Context").click(function () {

        $("#ColumnSortableModal").dialog("open");
        return false;
    });

    $("#MinMaxTableColumnOrderSetup_Context").click(function () {
        $("#MinMaxColumnSortableModal").dialog("open");
        return false;
    });

    $("#TurnsTableColumnOrderSetup_Context").click(function () {
        $("#TurnsColumnSortableModal").dialog("open");
        return false;
    });

    $("#ancLogout").click(function () {
        logout();
    });
    //$('div').on('dblclick', function (e) {

    //    // do stuff

    //    if (window.getSelection) {
    //        window.getSelection().removeAllRanges();
    //    } else if (document.selection) {
    //        document.selection.empty();
    //    }
    //});
    //            $('#sessionExpireWarning').knockoutSessionTimeout();


    $("ul.sub_subMenu li a").click(function () {

        setTimeout(function () {

            switch (window.location.hash.toLowerCase()) {
                case "#new": case "#newus":
                    $("#tab1").click();
                    break;
                case "#list": case "#hist":

                    $("#tab5").click();
                    break;
                case "#recpastmnts":
                    $("#tab28").click();
                    break;
                case "#mntsdue":
                    $("#tab27").click();
                    break;
                case "#listorders":
                    $("#tabOrders").click();
                    break;
                case "#listtransfers":
                    $("#tabTransfers").click();
                    break;
                case "#listsuggestedreturn":
                    $("#tabReturnList").click();
                    break;
                default:
                    if (!(window.location.href.toLowerCase().indexOf("newpull") > 0)) {
                        $("#tab5").click();
                    }
                    break;

            }

            if (window.location.hash.toLowerCase() == "#new") {
                $("#tab1").click();
            }
            else if (window.location.hash.toLowerCase() == "#list") {
                $("#tab5").click();
            }

            if (window.location.hash.toLowerCase() == "#unsubmitted") {
                $("#tab2").click();
            }
            else if (window.location.hash.toLowerCase() == "#approve" || window.location.hash.toLowerCase() == "#submitted") {
                $("#tab3").click();
            }
            else if (window.location.hash.toLowerCase() == "#approved" || window.location.hash.toLowerCase() == "#changeorder") {
                $("#tab4").click();
            }
            else if (window.location.hash.toLowerCase() == "#receive") {
                $("#tab7").click();
            }
            else if (window.location.hash.toLowerCase() == "#listorders") {
                $("#tabOrders").click();
            }
            else if (window.location.hash.toLowerCase() == "#listtransfers") {
                $("#tabTransfers").click();
            }
            else if (window.location.hash.toLowerCase() == "#listsuggestedreturn") {
                $("#tabReturnList").click();
            }









        }, 150);
    });

    $("#ddlEnterprise").change(function () {
        //check for any input field value change and message show to user before navigate
        if (!dirtyCheck()) {
            return false;
        }
        else {
            removeDirtyclass();
        }
        //End navigate
        var _selected = $("#ddlEnterprise").val();
        var _selectedtext = escape($("#ddlEnterprise option:selected").text());
        var RedirectEntURL = window.location.href;
        $.ajax({
            type: "POST"
            , url: urlSetEnterprise
            , data: "{'EnterpriseID':'" + _selected + "','EnterpriseText' : '" + _selectedtext + "'}"
            , contentType: "application/json;"
            , dataType: "json"
            , success: function (msg) {
                //                               window.location.href = window.location.href;
                if (window.location.pathname.toLowerCase() == "/master/evmisetup") {
                    window.location.href = window.location.pathname;
                } else {
                    if (RedirectEntURL.indexOf("Import") >= 0 || RedirectEntURL.indexOf("ExecuteStoredProcedure") > 0) {
                        //window.location.reload();
                        window.location.href = RedirectEntURL;
                    }
                    else {
                        if (window.location.pathname.toLowerCase() != "/udf/udflist" && window.location.href.indexOf("?") > -1) {
                            if (window.location.href.split('?').length > 0) {
                                if (window.location.href.split('?')[0] != "") {
                                    window.location.href = window.location.href.split('?')[0];
                                }
                            }
                        }
                        else {
                            window.location.reload();
                        }
                    }
                }
            }
        });
    });

    var previouscompanyvalue = "";
    $("#ddlCompany").focus(function () {
        // Store the current value on focus and on change
        previouscompanyvalue = this.value;
    }).change(function () {
        //check for any input field value change and message show to user before navigate
        if (!dirtyCheck()) {
            this.value = previouscompanyvalue;
            return false;
        }
        else {
            previouscompanyvalue = this.value;
            removeDirtyclass();
        }

        //End navigate
        var _selected = $("#ddlCompany").val();
        var _selectedtext = escape($("#ddlCompany option:selected").text());
        //  alert(_selectedtext);
        var RedirectCompanyURL = window.location.href;
        $.ajax({
            type: "POST"
            , url: urlSetCompany
            , data: "{'CompanyID':'" + _selected + "','CompanyName' : '" + _selectedtext + "'}"
            , contentType: "application/json;"
            , dataType: "json"
            , success: function (msg) {
                //                               window.location.href = window.location.href; 

                if (window.location.pathname.toLowerCase() == "/master/evmisetup") {
                    window.location.href = window.location.pathname;
                } else {
                    if (RedirectCompanyURL.indexOf("Import") >= 0 || RedirectCompanyURL.indexOf("ExecuteStoredProcedure") > 0) {
                        //window.location.reload();
                        window.location.href = RedirectCompanyURL;
                    }
                    else {
                        if (window.location.pathname.toLowerCase() != "/udf/udflist" && window.location.href.indexOf("?") > -1) {
                            if (window.location.href.split('?').length > 0) {
                                if (window.location.href.split('?')[0] != "") {
                                    window.location.href = window.location.href.split('?')[0];
                                }
                            }
                        }
                        else {
                            window.location.reload();
                        }
                    }
                }
            }
        });
    });

    $("#ddlStockroom").change(function () {
        //check for any input field value change and message show to user before navigate
        if (!dirtyCheck()) {
            return false;            
        }
        else {
            removeDirtyclass();
        }
        //End navigate
        var _selected = $("#ddlStockroom").val();
        var _selectedtext = escape($("#ddlStockroom option:selected").text());
        //  alert(_selectedtext);
        var RedirectRoomURL = window.location.href;
        $.ajax({
            type: "POST"
            , url: urlSetStockRoom
            , data: "{'RoomID':'" + _selected + "','RoomText' : '" + _selectedtext + "'}"
            , contentType: "application/json;"
            , dataType: "json"
            , success: function (msg) {
                //                               window.location.href = window.location.href;

                if (window.location.pathname.toLowerCase() == "/master/evmisetup") {
                    window.location.href = window.location.pathname;
                } else {
                    if (RedirectRoomURL.indexOf("Import") >= 0 || RedirectRoomURL.indexOf("ExecuteStoredProcedure") > 0) {
                        //window.location.reload();
                        window.location.href = RedirectRoomURL;
                    }
                    else {
                        if (window.location.pathname.toLowerCase() != "/udf/udflist" && window.location.href.indexOf("?") > -1) {
                            if (window.location.href.split('?').length > 0) {
                                if (window.location.href.split('?')[0] != "") {
                                    window.location.href = window.location.href.split('?')[0];
                                }
                            }
                        }
                        else {
                            window.location.reload();
                        }
                    }
                }
            }
        });
    });

    $("#divViewBarcodeModel").dialog({
        autoOpen: false, modal: true, draggable: true, resizable: true, width: '72%', height: 620, title: 'Add New Barcode',
        open: function () {
            $('#DivLoading').show();
            var strModuleName = $(this).data("strModule");
            var strbarcodeText = $(this).data("barcodetext");

            $.get(urlAddNewBarcodeFromModel, { 'moduleName': strModuleName, 'newBarcodeText': strbarcodeText }, function (data) {
                $("#divViewBarcodeModel").html(data);
                $('#DivLoading').hide();
            });
        },
        close: function () {
            $("#divViewBarcodeModel").empty();
            if (gblActionName.toLowerCase() === "orderlist") {
                $('#txtOrderFilter').focus();
            }
            else if (gblActionName.toLowerCase() === "transferlist") {
                $('#txtTransferFilter').focus();
            }
            else {
                $("#global_filter").focus();
            }
            //$('#myDataTable').dataTable().fnDraw();
        }
    });



    $("#ColumnSortableModal").dialog({
        autoOpen: false,
        modal: true,
        width: 500,
        //title: "ReOrder Columns",
        title: strReorderColumnPopupHeader,
        draggable: true,
        resizable: true,
        open: function () {

            GenerateColumnSortable();
            $("#ColumnSortable").sortable({ axis: "y", containment: "parent" });
        },
        close: function () {

        }
    });

    $("#MinMaxColumnSortableModal").dialog("destroy");
    $("#MinMaxColumnSortableModal").dialog({
        autoOpen: false,
        modal: true,
        width: 500,
        //title: "ReOrder Columns",
        title: strReorderColumnPopupHeader,
        draggable: true,
        resizable: true,
        open: function () {
            GenerateColumnSortableForMinMaxDashboard();
            $("#ColumnSortable").sortable({ axis: "y", containment: "parent" });
        },
        close: function () {
        }
    });

    $("#TurnsColumnSortableModal").dialog("destroy");
    $("#TurnsColumnSortableModal").dialog({
        autoOpen: false,
        modal: true,
        width: 500,
        //title: "ReOrder Columns",
        title: strReorderColumnPopupHeader,
        draggable: true,
        resizable: true,
        open: function () {
            GenerateColumnSortableForTurnsDashboard();
            $("#TurnsColumnSortable").sortable({ axis: "y", containment: "parent" });
        },
        close: function () {
        }
    });

    //$(window).load(function () {
    //    $('#global_filter').val('');
    //});


    /*Top Navitaion*/
    //$('#num').transform({ "rotate": '270deg' });

    $('#DivLoading').hide();
    //.hide()  // hide it initially


    $('#close').click(function () {
        $("#target").hide();
        return false;
    });

    $('#admin').click(function () {
        $(".slidingDiv").slideToggle();
        //setTimeout(function () { $(".slidingDiv").fadeOut(); }, 5000);
    });


    //Start - Grid delete functionality on Keypress

    $(document).keyup(function (e) {

        /*START - HANDLING OF GLOBAL DELETE KEYPRESS BUG*/
        if (e.target.type == 'text' && e.target.localName == 'input')
            return false;
        /*END - HANDLING OF GLOBAL DELETE KEYPRESS BUG*/

        if (gblActionName.toLowerCase() === "kitlist") {
            var code = (e.keyCode ? e.keyCode : e.which);
            if (code == 89 && $('#cnfBarcodeAddmdl').css('display') != "none") {
                $("#btnBarcodeAddYes").click();
            }
            else if (code == 78 && $('#cnfBarcodeAddmdl').css('display') != "none") {
                closeBarcodeYesNoModal();
            }
            return false;
        }
        if (typeof (AllowDeletePopup) != "undefined") {


            if (AllowDeletePopup == true) {

                var code = (e.keyCode ? e.keyCode : e.which);
                if (code == 46) {
                    $('#deleteRows').click();
                }
                if (code == 89 && IsDeletePopupOpen == true) {
                    $("#btnModelYes").click();
                    IsDeletePopupOpen = false;
                }
                else if (code == 78 && IsDeletePopupOpen == true) {
                    closeModal();
                    IsDeletePopupOpen = false;
                }
                else if (code == 89 && $('#cnfBarcodeAddmdl').css('display') != "none") {
                    $("#btnBarcodeAddYes").click();

                }
                else if (code == 78 && $('#cnfBarcodeAddmdl').css('display') != "none") {
                    closeBarcodeYesNoModal();
                }

            }
        }

        if (AllowDeletePopupPSInPull == true) {
            var Vcode = (e.keyCode ? e.keyCode : e.which);
            if (Vcode == 89 && AllowDeletePopupPSInPull == true) {
                $("#btnModelYesPSLimit").click();
                AllowDeletePopupPSInPull = false;
            }
            else if (Vcode == 78 && AllowDeletePopupPSInPull == true) {
                closeModalPSLimit();
                AllowDeletePopupPSInPull = false;
            }
        }


    });

    //End - Grid delete functionality on Keypress


});

function closeModal() {
    $.modal.impl.close();
}
function closeModalSaveBeforeLeavePage() {
    $.modal.impl.close();
    $('.verticalText').click();
}

function closeBarcodeYesNoModal() {
    if (gblActionName.toLowerCase() === "orderlist") {
        $('#txtOrderFilter').focus();
        $('#txtOrderFilter').select();

    }
    else if (gblActionName.toLowerCase() === "transferlist") {
        $('#txtTransferFilter').focus();
        $('#txtTransferFilter').select();
    }
    else if (typeof $("#global_filter") != 'undefined') {
        $('#global_filter').focus();
        $('#global_filter').select();
    }
    $.modal.impl.close();
}
function FillTopEnterprise() {
    var request = $.ajax({
        url: urlGetEnterprises,
        type: "POST",
        contentType: "application/json",
        dataType: "json"
    });
    request.done(function (retdata) {
        $("#ddlEnterprise").html("");
        if (retdata != null) {
            if (retdata.length > 0) {
                $("#ddlEnterprise").html(retdata);
            }
            else {
                $("#ddlEnterprise").html("<option value=''>No enterprises</option>");
            }
        }
        else {
            $("#ddlEnterprise").html("<option value=''>No enterprises</option>");
        }
    });

    request.fail(function (jqXHR, textStatus) {

    });

}
function FillTopRooms() {
    var request = $.ajax({
        url: urlGetRoomList,
        type: "POST",
        contentType: "application/json",
        dataType: "json"
    });
    request.done(function (retdata) {
        $("#ddlStockroom").html("");
        if (retdata != null) {
            if (retdata.length > 0) {
                $("#ddlStockroom").html(retdata);
            }
            else {
                $("#ddlStockroom").html("<option value=''>No Rooms</option>");
            }
        } else {
            $("#ddlStockroom").html("<option value=''>No Rooms</option>");
        }
        if ($("#ddlStockroom").find("option:selected").length > 0) {
            if ($("#ddlStockroom").find("option:selected").attr("style") != undefined) {
                if ($("#ddlStockroom").find("option:selected").attr("style").toString().indexOf("lightgrey") != -1) {
                    $("#ddlStockroom").css("background-color", "lightgrey");
                    $("#ddlStockroom").find("option").each(function (inx, option) {
                        if ($(option).attr("style") != undefined) {
                            if ($(option).attr("style").toString().indexOf("lightgrey") == -1) {
                                $(option).css("background-color", "white");
                            }
                        }
                        else {
                            $(option).css("background-color", "white");
                        }
                    });
                }
                else {
                    $("#ddlStockroom").css("background-color", "white");
                }
            }
        }
        else {
            $("#ddlStockroom").css("background-color", "white");
        }
    });

    request.fail(function (jqXHR, textStatus) {

    });

}

function FillTopCompanies() {
    var request = $.ajax({
        url: urlGetCompanies,
        type: "POST",
        contentType: "application/json",
        dataType: "json"
    });
    request.done(function (retdata) {
        $("#ddlCompany").html("");
        if (retdata != null) {
            if (retdata.length > 0) {
                $("#ddlCompany").html(retdata);
            }
            else {
                $("#ddlCompany").html("<option value=''>No Companies</option>");
            }
        }
        else {
            $("#ddlCompany").html("<option value=''>No Companies</option>");
        }
        if ($("#ddlCompany").find("option:selected").length > 0) {
            if ($("#ddlCompany").find("option:selected").attr("style") != undefined) {
                if ($("#ddlCompany").find("option:selected").attr("style").toString().indexOf("lightgrey") != -1) {
                    $("#ddlCompany").css("background-color", "lightgrey");
                    $("#ddlCompany").find("option").each(function (inx, option) {
                        if ($(option).attr("style") != undefined) {
                            if ($(option).attr("style").toString().indexOf("lightgrey") == -1) {
                                $(option).css("background-color", "white");
                            }
                        }
                        else {
                            $(option).css("background-color", "white");
                        }
                    });
                }
                else {
                    $("#ddlCompany").css("background-color", "white");
                }
            }
        }
        else {
            $("#ddlCompany").css("background-color", "white");
        }
    });

    request.fail(function (jqXHR, textStatus) {

    });

}

function FillCompanyLanguages() {
    $.ajax({
        url: urlGetCompanyLanguage
        , type: 'GET'
        , contentType: "application/json"
        , success: function (result) {
            var items = "";
            $.each(result, function (i, result) {
                items += "<option value='" + result.Value + "'>" + result.Text + "</option>";
            });
            $("#ddlCompanyLanguage").html(items);
            $("#ddlCompanyLanguage").val(strCurrentCult);
        }
        , error: function (msg) {
        }
    });

}

function logout() {
    $.ajax({
        type: "POST",
        url: urlLogoutUser,
        data: "{'CompanyID': '1'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (message) {
            window.location = '/Master/UserLogin';
        },
        error: function (response) {
            // through errror message
        }
    });
}
function KeepSessionAlive() {
    if (IsLoginActiveRequired) {
        $.ajax({
            type: "POST"
            , url: urlLoginSessionAlive
            , headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() }
            , contentType: "application/json;"
            , dataType: "json"
            , success: function (msg) {
                CounterInMin += 1;
                if (CounterInMin > 30)
                    IsLoginActiveRequired = false;
            }
        });
    }
}
function SetPageMode() {


    if (window.location.hash.toLowerCase() == "#new") {
        $("#tab1").click();
    }



    //            if (window.location.hash.toLowerCase() == "#listing") {
    //                $("#tab5").click();
    //            }
}
function ClickFromMenu(value) {
    $('#DivLoading').show();
    $.ajax({
        url: urlClickFromMenuNotification,
        data: { 'IsTrue': value },
        type: 'Post',
        dataType: 'json',
        "async": false,
        cache: false,
        success: function (response) {
            $('#DivLoading').hide();
            return;
        },
        error: function (xhr) {
            $('#DivLoading').hide();
            return;
        }

    });
}
function SetRoomCockie() {
    //var eid = $("#divEnterPrise").find("#ddlEnterprise").val();
    //var cid = $("#divEnterPrise").find("#ddlCompany").val();
    //var rid = $("#divEnterPrise").find("#ddlStockroom").val();
    $.cookie('EPCMRM', EID + "_" + CID + "_" + RID, { path: '/', SameSite: 'Strict'  });
}
function GetCurrentselctedRoom() {
    var eid = parseInt($("#divEnterPrise").find("#ddlEnterprise").val());
    var cid = parseInt($("#divEnterPrise").find("#ddlCompany").val());
    var rid = parseInt($("#divEnterPrise").find("#ddlStockroom").val());
    if (isNaN(eid)) {
        eid = 0;
    }
    if (isNaN(cid)) {
        cid = 0;
    }
    if (isNaN(rid)) {
        rid = 0;
    }

    return eid + "_" + cid + "_" + rid;
}
//        function FillRedCount() {
//            $.ajax({
//                url: '@Url.Content("~/Order/GetTabCountRedCircle")'
//                 , type: "GET"
//                 , data: {}
//                 , contentType: "application/json"
//                 , dataType: "json"
//                 , "async": true
//                 , success: function (result) {


//                     if ($("#spnTotReplinishCnt") !== undefined)
//                         $("#spnTotReplinishCnt").text(result.TotalReplanish);

//                     if ($("#amnuOrderLink") !== undefined) {
//                         $("#amnuOrderLink").text(result.OrderNotification);
//                     }

//                     if ($("#amnuReceiveLink") !== undefined) {
//                         $("#amnuReceiveLink").text(result.ReceiveNotification);
//                     }

//                     if ($("#amnuCartLink") !== undefined) {
//                         $("#amnuCartLink").text(result.CartNotification);
//                     }
//                     if ($("#amnuTransferLink") !== undefined) {
//                         $("#amnuTransferLink").text(result.TransferNotification);
//                     }

//                     if (isNaN(parseInt(result.TotalReplanish)) == false && parseInt(result.TotalReplanish) > 0) {
//                         $("#spnTotReplinishCnt").css('display', 'block');
//                     } else {
//                         $("#spnTotReplinishCnt").css('display', 'none');
//                     }


//                 }
//                 , error: function (msg) {
//                     alert('error');
//                 }
//            });
//        }

function SetConsumeRedCount() {
    if ($("#spnTotConsumeCnt").length <= 0) {
        return false;
    }
    $.ajax({
        url: urlGetConsumeRedCount,
        type: "GET",
        data: {},
        contentType: "application/json",
        dataType: "json",
        "async": true,
        "cache": false,
        success: function (result) {
            if (result.Status = "ok") {
                if (result.ModuleType == "Consume") {
                    if (!isNaN(parseInt(result.ConsumeMenuButtonCount)) && parseInt(result.ConsumeMenuButtonCount) > 0) {
                        $("#spnTotConsumeCnt").css('display', 'block');
                        $("#spnTotConsumeCnt").text(result.ConsumeMenuButtonCount);
                    }
                    else {
                        $("#spnTotConsumeCnt").text('');
                        $("#spnTotConsumeCnt").css('display', 'none');
                    }
                    if ($("#amnuRequisitionLink") !== undefined) {
                        if (!isNaN(parseInt(result.RequisitionMenuLinkCount)) && parseInt(result.RequisitionMenuLinkCount) > 0) {
                            $("#amnuRequisitionLink").html(strResLayoutRequisitions + '(' + result.RequisitionMenuLinkCount + ')');
                        }
                    }

                    SetRequisitionRedCount(result.RequisitionRedCount)

                }

            }

        },
        error: function (msg) {
            // alert('error');
        }
    });
}

function SetRequisitionRedCount(obj) {
    if (gblControllerName == 'Consume') {
        $("#divUnsubmitted").css('display', 'none');
        $("#divUnsubmitted").html('');
        $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divSubmittted").css('display', 'none');
        $("#divSubmittted").html('');
        $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divApproved").css('display', 'none');
        $("#divApproved").html('');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');


    }
    if ($("#amnuReqUnsubmittedLink") !== undefined) {
        $("#amnuReqUnsubmittedLink").html(strResCommonUnsubmitted);
    }
    if ($("#amnuReqSubmittedLink") !== undefined) {
        $("#amnuReqSubmittedLink").html(strResCommonApproved);
    }
    if ($("#amnuReqApprovedLink") !== undefined) {
        $("#amnuReqApprovedLink").html(strResCommonPull);
    }

    for (var i = 0; i < obj.length; i++) {
        var OrderLinkCnt = 0;
        if (obj[i].Status == 'UnSubmitted') {
            if (gblControllerName == 'Consume') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divUnsubmitted").html(obj[i].RecCircleCount);

                    $("#divUnsubmitted").css('display', 'block');
                    $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }

            if ($("#amnuReqUnsubmittedLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuReqUnsubmittedLink").html(strResCommonUnsubmitted + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'ToBeApproved') {
            if (gblControllerName == 'Consume') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divSubmittted").html(obj[i].RecCircleCount);
                    $("#divSubmittted").css('display', 'block');
                    $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuReqSubmittedLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuReqSubmittedLink").html(strResCommonApproved + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'Approved') {
            if (gblControllerName == 'Consume') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divApproved").html(obj[i].RecCircleCount);
                    $("#divApproved").css('display', 'block');
                    $("#divApproved").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuReqApprovedLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuReqApprovedLink").html(strResCommonPull + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
    }
}


function SetReplenishRedCount() {

    if ($("#spnTotReplinishCnt").length <= 0) {
        return false;
    }

    $("#divSuggestedOrders").parent().find("#spnTabName").css('padding-right', '25px');
    $("#divSuggestedTransfers").parent().find("#spnTabName").css('padding-right', '25px');
    $("#divSuggestedReturn").parent().find("#spnTabName").css('padding-right', '25px');

    $.ajax({
        url: urlGetReplinshRedCount,
        type: "GET",
        data: {},
        contentType: "application/json",
        dataType: "json",
        "async": true,
        "cache": false,
        success: function (result) {
            if (result.Status = "ok") {
                if (result.ModuleType == "Replenish") {
                    //if ($("#spnTotReplinishCnt") !== undefined)
                    if (!isNaN(parseInt(result.ReplenishMenuButtonCount)) && parseInt(result.ReplenishMenuButtonCount) > 0) {
                        $("#spnTotReplinishCnt").css('display', 'block');
                        $("#spnTotReplinishCnt").text(result.ReplenishMenuButtonCount);
                    }
                    else {
                        $("#spnTotReplinishCnt").text('');
                        $("#spnTotReplinishCnt").css('display', 'none');
                    }

                    if ($("#amnuCartLink") !== undefined) {
                        if (!isNaN(parseInt(result.CartMenuLinkCount)) && parseInt(result.CartMenuLinkCount) > 0) {
                            $("#amnuCartLink").html(strResLayoutCart + '(' + result.CartMenuLinkCount + ')');

                            if (!isNaN(parseInt(result.CartOrderMenuLinkCount)) && parseInt(result.CartOrderMenuLinkCount) > 0) {
                                $("#amnuCartLinkOrders").html(strResLayoutCartOrders + '(' + result.CartOrderMenuLinkCount + ')');
                            }
                            //else {
                            //    $("#amnuCartLinkOrders").text(strResLayoutCartOrders);
                            //}
                            if (!isNaN(parseInt(result.CartTransMenuLinkCount)) && parseInt(result.CartTransMenuLinkCount) > 0) {
                                $("#amnuCartLinkTransfers").html(strResLayoutCartTransfers + '(' + result.CartTransMenuLinkCount + ')');
                            }
                            //else {
                            //    $("#amnuCartLinkTransfers").text(strResLayoutCartTransfers);
                            //}
                            if (!isNaN(parseInt(result.CartSuggestedReturnOrderMenuLinkCount)) && parseInt(result.CartSuggestedReturnOrderMenuLinkCount) > 0) {
                                $("#amnuCartLinkSuggestedReturn").html(strResLayoutCartSuggestedReturn + '(' + result.CartSuggestedReturnOrderMenuLinkCount + ')');
                            }
                            else {
                                $("#amnuCartLinkSuggestedReturn").html(strResLayoutCartSuggestedReturn);
                            }

                            if (typeof (gblActionName) != undefined) {
                                if (gblActionName != undefined) {
                                    if (gblActionName.toLowerCase() == "cartitemlist") {
                                        $("div#divUnSubmittted").html(result.CartMenuLinkCount);
                                        $("div#divUnSubmittted").show();
                                        $("#tab5").find("span#spnTabName").css("padding-right", "25px");

                                        if (!isNaN(parseInt(result.CartOrderMenuLinkCount)) && parseInt(result.CartOrderMenuLinkCount) > 0) {
                                            $("div#divSuggestedOrders").html(result.CartOrderMenuLinkCount);
                                            $("div#divSuggestedOrders").show();
                                        }
                                        if (!isNaN(parseInt(result.CartTransMenuLinkCount)) && parseInt(result.CartTransMenuLinkCount) > 0) {
                                            $("div#divSuggestedTransfers").html(result.CartTransMenuLinkCount);
                                            $("div#divSuggestedTransfers").show();
                                        }
                                        if (!isNaN(parseInt(result.CartSuggestedReturnOrderMenuLinkCount)) && parseInt(result.CartSuggestedReturnOrderMenuLinkCount) > 0) {
                                            $("div#divSuggestedReturn").html(result.CartSuggestedReturnOrderMenuLinkCount);
                                            $("div#divSuggestedReturn").show();
                                        }
                                    }
                                    if (gblActionName.toLowerCase() == "dashboard") {
                                        $("#spndashcartcount").text(result.CartMenuLinkCount);
                                    }

                                }
                            }
                        }
                        else {
                            $("div#divUnSubmittted").html("");
                            $("div#divUnSubmittted").hide();

                            $("div#divSuggestedOrders").html("");
                            $("div#divSuggestedOrders").hide();
                            $("div#divSuggestedTransfers").html("");
                            $("div#divSuggestedTransfers").hide();
                            $("div#divSuggestedReturn").html("");
                            $("div#divSuggestedReturn").hide();

                            $("#tab5").find("span#spnTabName").css("padding-right", "5px");
                            $("#amnuCartLink").html(strResLayoutCart);
                            $("#amnuCartLinkOrders").html(strResLayoutCartOrders);
                            $("#amnuCartLinkTransfers").html(strResLayoutCartTransfers);
                            $("#amnuCartLinkSuggestedReturn").html(strResLayoutCartSuggestedReturn);
                        }
                    }
                    if ($("#amnuOrderLink") !== undefined) {
                        if (!isNaN(parseInt(result.OrderMenuLinkCount)) && parseInt(result.OrderMenuLinkCount) > 0) {
                            $("#amnuOrderLink").html(strResLayoutOrders + '(' + result.OrderMenuLinkCount + ')');
                        }
                        //else {
                        //    $("#amnuOrderLink").text(strResLayoutOrders);
                        //}
                        SetOrderRedCount(result.OrderRedCount);

                        if ($("#amnuReceiveLink") === undefined) {
                            SetReceiveRedCount(result.ReceiveRedCount);
                        }
                        if ($("#amnuReceiveToolLink") === undefined) {
                            SetReceiveToolRedCount(result.ToolReceiveRedCount);
                        }
                    }
                    if ($("#amnuToolAssetOrderLink") !== undefined) {
                        if (!isNaN(parseInt(result.ToolOrderMenuLinkCount)) && parseInt(result.ToolOrderMenuLinkCount) > 0) {
                            $("#amnuToolAssetOrderLink").html(strResLayoutOrders + '(' + result.ToolOrderMenuLinkCount + ')');
                        }
                        //else {
                        //    $("#amnuToolAssetOrderLink").text(strResLayoutOrders);
                        //}
                        SetToolOrderRedCount(result.ToolOrderRedCount);

                        //if ($("#amnuReceiveLink") === undefined) {
                        //    SetReceiveRedCount(result.ReceiveRedCount);
                        //}
                        //if ($("#amnuReceiveToolLink") === undefined) {
                        //    SetReceiveToolRedCount(result.ToolReceiveRedCount);
                        //}
                    }

                    if ($("#amnuReturnOrderLink") !== undefined) {
                        if (!isNaN(parseInt(result.OrderMenuLinkCount)) && parseInt(result.ReturnOrderMenuLinkCount) > 0) {
                            $("#amnuReturnOrderLink").html(strResLayoutReturnOrders + '(' + result.ReturnOrderMenuLinkCount + ')');
                        }
                        //else {
                        //    $("#amnuReturnOrderLink").text(strResLayoutReturnOrders);
                        //}
                        SetReturnOrderRedCount(result.ReturnOrderRedCount);
                    }


                    if ($("#amnuReceiveLink") !== undefined) {
                        if (!isNaN(parseInt(result.ReceiveMenuLinkCount)) && parseInt(result.ReceiveMenuLinkCount) > 0) {
                            $("#amnuReceiveLink").html(strResLayoutReceive + '(' + result.ReceiveMenuLinkCount + ')');
                        }
                        SetReceiveRedCount(result.ReceiveRedCount);
                    }

                    if ($("#amnuReceiveToolLink") !== undefined) {
                        if (!isNaN(parseInt(result.ToolReceiveMenuLinkCount)) && parseInt(result.ToolReceiveMenuLinkCount) > 0) {
                            $("#amnuReceiveToolLink").html(strResLayoutReceive + '(' + result.ToolReceiveMenuLinkCount + ')');
                        }
                        //else {
                        //    $("#amnuReceiveToolLink").text(strResLayoutReceive);
                        //}
                        SetReceiveToolRedCount(result.ToolReceiveRedCount);
                    }

                    if ($("#amnuTransferLink") !== undefined) {
                        if (!isNaN(parseInt(result.TransferMenuLinkCount)) && parseInt(result.TransferMenuLinkCount) > 0) {
                            $("#amnuTransferLink").html(strResLayoutTransfer + '(' + result.TransferMenuLinkCount + ')');
                        }
                        //else {
                        //    $("#amnuTransferLink").text(strResLayoutTransfer);
                        //}
                        SetTransferRedCount(result.TransferRedCount);
                    }
                    if ($("#amnuQuoteLink") !== undefined) {
                        
                        if (!isNaN(parseInt(result.QuoteMenuLinkCount)) && parseInt(result.QuoteMenuLinkCount) > 0) {
                            $("#amnuQuoteLink").html(strResLayoutQuote + '(' + result.QuoteMenuLinkCount + ')');
                        }
                        //else {
                        //    $("#amnuQuoteLink").text(strResLayoutQuote);
                        //}
                        SetQuoteRedCount(result.QuoteRedCount);
                    }
                    if ($("#amnuQuoteToOrder") !== undefined) {

                        //if (!isNaN(parseInt(result.QuoteMenuLinkCount)) && parseInt(result.QuoteMenuLinkCount) > 0) {
                        //    $("#amnuQuoteToOrder").text(strResLayoutQuote + '(' + result.QuoteMenuLinkCount + ')');
                        //}
                        //else {
                        $("#amnuQuoteToOrder").html(strResLayoutQuoteToOrder);
                        //}
                        //SetQuoteRedCount(result.QuoteRedCount);
                    }
                }

            }

        },
        error: function (msg) {
            // alert('error');
        }
    });
}


function SetOrderRedCount(obj) {

    if (gblActionName.toLowerCase() == 'orderlist') {
        $("#divUnsubmitted").css('display', 'none');
        $("#divSubmittted").css('display', 'none');
        $("#divChangeOrder").css('display', 'none');
        $("#divApproved").css('display', 'none');

        $("#divUnsubmitted").html('');
        $("#divSubmittted").html('');
        $("#divChangeOrder").html('');
        $("#divApproved").html('');

        $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '5px');
        $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '5px');
        $("#divChangeOrder").parent().find("#spnTabName").css('padding-right', '5px');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');

    }
    if ($("#amnuOrderUnsubmittedLink") !== undefined) {
        $("#amnuOrderUnsubmittedLink").html(strResCommonUnsubmitted);
    }
    if ($("#amnuOrderApproveLink") !== undefined) {
        $("#amnuOrderApproveLink").html(strResCommonApproved);
    }

    if ($("#amnuChangeOrderLink") !== undefined) {
        $("#amnuChangeOrderLink").html(strResOrderChangeOrder);
    }

    if ($("#amnuOrderReceiveLink") !== undefined) {
        $("#amnuOrderReceiveLink").html(strResCommonReceivable);
    }
    var vChOrdRedCount = 0;
    for (var i = 0; i < obj.length; i++) {
        var OrderLinkCnt = 0;

        if (obj[i].Status == 'UnSubmitted') {
            if (gblActionName.toLowerCase() == 'orderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divUnsubmitted").html(obj[i].RecCircleCount);
                    $("#divUnsubmitted").css('display', 'block');
                    $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }

            if ($("#amnuOrderUnsubmittedLink") !== undefined) {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuOrderUnsubmittedLink").html(strResCommonUnsubmitted + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'ToBeApproved') {
            if (gblActionName.toLowerCase() == 'orderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divSubmittted").html(obj[i].RecCircleCount);
                    $("#divSubmittted").css('display', 'block');
                    $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '25px');
                }

            }
            if ($("#amnuOrderApproveLink") !== undefined) {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuOrderApproveLink").html(strResCommonApproved + '(' + obj[i].RecCircleCount + ')');
                }

            }
        }
        else if (obj[i].Status == 'Transmitted' || obj[i].Status == 'TransmittedInCompletePastDue') {
            if (gblActionName.toLowerCase() == 'orderlist') {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    vChOrdRedCount = vChOrdRedCount + parseInt(obj[i].RecCircleCount);
                    $("#divChangeOrder").html(vChOrdRedCount);
                    $("#divChangeOrder").css('display', 'block');
                    $("#divChangeOrder").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuChangeOrderLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuChangeOrderLink").html(strResOrderChangeOrder + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }


    }
}

function SetToolOrderRedCount(obj) {

    if (gblActionName.toLowerCase() == 'toolassetorderlist') {
        $("#divUnsubmitted").css('display', 'none');
        $("#divSubmittted").css('display', 'none');
        $("#divChangeOrder").css('display', 'none');
        $("#divApproved").css('display', 'none');

        $("#divUnsubmitted").html('');
        $("#divSubmittted").html('');
        $("#divChangeOrder").html('');
        $("#divApproved").html('');

        $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '5px');
        $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '5px');
        $("#divChangeOrder").parent().find("#spnTabName").css('padding-right', '5px');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');

    }
    if ($("#amnuToolOrderUnsubmittedLink") !== undefined) {
        $("#amnuToolOrderUnsubmittedLink").html(strResCommonUnsubmitted);
    }
    if ($("#amnuToolOrderApproveLink") !== undefined) {
        $("#amnuToolOrderApproveLink").html(strResCommonApproved);
    }

    if ($("#amnuChangeToolOrderLink") !== undefined) {
        $("#amnuChangeToolOrderLink").html(strResOrderChangeOrder);
    }

    if ($("#amnuToolOrderReceiveLink") !== undefined) {
        $("#amnuToolOrderReceiveLink").html(strResCommonReceivable);
    }
    var vChOrdRedCount = 0;
    for (var i = 0; i < obj.length; i++) {
        var OrderLinkCnt = 0;

        if (obj[i].Status == 'UnSubmitted') {
            if (gblActionName.toLowerCase() == 'toolassetorderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divUnsubmitted").html(obj[i].RecCircleCount);
                    $("#divUnsubmitted").css('display', 'block');
                    $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }

            if ($("#amnuToolOrderUnsubmittedLink") !== undefined) {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuToolOrderUnsubmittedLink").html(strResCommonUnsubmitted + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'ToBeApproved') {
            if (gblActionName.toLowerCase() == 'toolassetorderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divSubmittted").html(obj[i].RecCircleCount);
                    $("#divSubmittted").css('display', 'block');
                    $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '25px');
                }

            }
            if ($("#amnuToolOrderApproveLink") !== undefined) {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuToolOrderApproveLink").html(strResCommonApproved + '(' + obj[i].RecCircleCount + ')');
                }

            }
        }
        else if (obj[i].Status == 'Transmitted' || obj[i].Status == 'TransmittedInCompletePastDue') {
            if (gblActionName.toLowerCase() == 'toolassetorderlist') {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    vChOrdRedCount = vChOrdRedCount + parseInt(obj[i].RecCircleCount);
                    $("#divChangeOrder").html(vChOrdRedCount);
                    $("#divChangeOrder").css('display', 'block');
                    $("#divChangeOrder").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuChangeToolOrderLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuChangeToolOrderLink").html(strResOrderChangeOrder + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }


    }
}

function SetReceiveRedCount(obj) {

    if (gblActionName.toLowerCase() == 'orderlist') {
        $("#divApproved").css('display', 'none');
        $("#divApproved").html('');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');
    }

    if (gblControllerName == 'Receive') {
        $("#divIncompleteItems").css('display', 'none');
        $("#divIncompleteItems").html('');
        $("#divIncompleteItems").parent().find("#spnTabName").css('padding-right', '5px');
    }
    var strResIncomplete = "Incomplete";
    if (typeof strResLayoutIncomplete != 'undefined') {
        strResIncomplete = strResLayoutIncomplete;
    }
    if ($("#amnuReceiveIncomplete") !== undefined) {
        $("#amnuReceiveIncomplete").html(strResIncomplete);
    }


    for (var i = 0; i < obj.length; i++) {
        var OrderLinkCnt = 0;
        if (obj[i].Status == 'InComplete') {
            if (gblControllerName == 'Receive') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divIncompleteItems").html(obj[i].RecCircleCount);
                    $("#divIncompleteItems").css('display', 'block');
                    $("#divIncompleteItems").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }

            if ($("#amnuReceiveIncomplete") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuReceiveIncomplete").html(strResIncomplete + '(' + obj[i].RecCircleCount + ')');
                }
            }
            if ($("#amnuChangeOrderLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuOrderReceiveLink").html(strResLayoutReceive + '(' + obj[i].RecCircleCount + ')');
                }
            }
            if (gblActionName.toLowerCase() == 'orderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divApproved").html(obj[i].RecCircleCount);
                    $("#divApproved").css('display', 'block');
                    $("#divApproved").parent().find("#spnTabName").css('padding-right', '25px');
                }


            }
        }

    }


}

function SetTransferRedCount(obj) {
    if (gblControllerName == 'Transfer') {
        $("#divUnsubmitted").css('display', 'none');
        $("#divUnsubmitted").html('');
        $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divRequested").css('display', 'none');
        $("#divRequested").html('');
        $("#divRequested").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divSubmittted").css('display', 'none');
        $("#divSubmittted").html('');
        $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divApproved").css('display', 'none');
        $("#divApproved").html('');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divReceivable").css('display', 'none');
        $("#divReceivable").html('');
        $("#divReceivable").parent().find("#spnTabName").css('padding-right', '5px');
    }

    if ($("#amnuTransferUnsubmittedLink") !== undefined) {
        $("#amnuTransferUnsubmittedLink").html(strResCommonUnsubmitted);
    }
    if ($("#amnuTransferRequestedLink") !== undefined) {
        $("#amnuTransferRequestedLink").html(strResCommonRequested);
    }
    if ($("#amnuTransferApproveLink") !== undefined) {
        $("#amnuTransferApproveLink").html(strResCommonApproved);
    }
    if ($("#amnuTransferReceiveLink") !== undefined) {
        $("#amnuTransferReceiveLink").html(strResCommonReceivable);
    }

    for (var i = 0; i < obj.length; i++) {
        var OrderLinkCnt = 0;
        if (obj[i].Status == 'UnSubmitted') {
            if (gblControllerName == 'Transfer') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divUnsubmitted").html(obj[i].RecCircleCount);
                    $("#divUnsubmitted").css('display', 'block');
                    $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuTransferUnsubmittedLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuTransferUnsubmittedLink").html(strResCommonUnsubmitted + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        if (obj[i].Status == 'Requested') {
            if (gblControllerName == 'Transfer') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divRequested").html(obj[i].RecCircleCount);
                    $("#divRequested").css('display', 'block');
                    $("#divRequested").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuTransferRequestedLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuTransferRequestedLink").html(strResCommonRequested + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'ToBeApproved') {
            if (gblControllerName == 'Transfer') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divSubmittted").html(obj[i].RecCircleCount);
                    $("#divSubmittted").css('display', 'block');
                    $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuTransferApproveLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuTransferApproveLink").html(strResCommonApproved + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'Receive') {
            if (gblControllerName == 'Transfer') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divReceivable").html(obj[i].RecCircleCount);
                    $("#divReceivable").css('display', 'block');
                    $("#divReceivable").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuTransferReceiveLink") !== undefined) {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuTransferReceiveLink").html(strResCommonReceivable + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
    }
}

function SetQuoteRedCount(obj) {
    if (gblActionName.toLowerCase() == 'quotelist') {
        $("#divUnsubmitted").css('display', 'none');
        $("#divUnsubmitted").html('');
        $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divRequested").css('display', 'none');
        $("#divRequested").html('');
        $("#divRequested").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divSubmittted").css('display', 'none');
        $("#divSubmittted").html('');
        $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divApproved").css('display', 'none');
        $("#divApproved").html('');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divReceivable").css('display', 'none');
        $("#divReceivable").html('');
        $("#divReceivable").parent().find("#spnTabName").css('padding-right', '5px');
    }

    if ($("#amnuQuoteUnsubmittedLink") !== undefined) {
        $("#amnuQuoteUnsubmittedLink").html(strResCommonUnsubmitted);
    }
    if ($("#amnuQuoteRequestedLink") !== undefined) {
        $("#amnuQuoteRequestedLink").html(strResCommonRequested);
    }
    if ($("#amnuQuoteApproveLink") !== undefined) {
        $("#amnuQuoteApproveLink").html(strResCommonApproved);
    }
    
    if ($("#amnuChangeQuoteLink") !== undefined) {
        $("#amnuChangeQuoteLink").html(strResOrderChangeQuote);
    }
     
    var vChQuoteRedCount = 0;
    for (var i = 0; i < obj.length; i++) {
        if (obj[i].Status == 'UnSubmitted') {
            if (gblActionName.toLowerCase() == 'quotelist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divUnsubmitted").html(obj[i].RecCircleCount);
                    $("#divUnsubmitted").css('display', 'block');
                    $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuQuoteUnsubmittedLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuQuoteUnsubmittedLink").html(strResCommonUnsubmitted + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        if (obj[i].Status == 'Requested') {
            if (gblActionName.toLowerCase() == 'quotelist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divRequested").html(obj[i].RecCircleCount);
                    $("#divRequested").css('display', 'block');
                    $("#divRequested").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuQuoteRequestedLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuQuoteRequestedLink").html(strResCommonRequested + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'ToBeApproved') {
            if (gblActionName.toLowerCase() == 'quotelist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divSubmittted").html(obj[i].RecCircleCount);
                    $("#divSubmittted").css('display', 'block');
                    $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '25px');
                }

            }
            if (typeof($("#amnuQuoteApproveLink")) !== "undefined") {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuQuoteApproveLink").html(strResCommonApproved + '(' + obj[i].RecCircleCount + ')');
                }

            }
        }
        else if (obj[i].Status == 'Transmitted' || obj[i].Status == 'TransmittedInCompletePastDue') {
            if (gblActionName.toLowerCase() == 'quotelist') {

                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    vChQuoteRedCount = vChQuoteRedCount + parseInt(obj[i].RecCircleCount);
                    $("#divApproved").html(vChQuoteRedCount);
                    $("#divApproved").css('display', 'block');
                    $("#divApproved").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if (typeof ($("#amnuChangeQuoteLink")) !== "undefined") {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuChangeQuoteLink").html(strResOrderChangeQuote + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
         
    }
}
function SetReturnOrderRedCount(obj) {

    if (gblActionName.toLowerCase() == 'returnorderlist') {
        $("#divUnsubmitted").css('display', 'none');
        $("#divUnsubmitted").html('');
        $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divSubmittted").css('display', 'none');
        $("#divSubmittted").html('');
        $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divApproved").css('display', 'none');
        $("#divApproved").html('');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');

        $("#divChangeOrder").css('display', 'none');
        $("#divChangeOrder").html('');
        $("#divChangeOrder").parent().find("#spnTabName").css('padding-right', '5px');

    }
    if ($("#amnuReturnOrderUnsubmittedLink") !== undefined) {
        $("#amnuReturnOrderUnsubmittedLink").html(strResCommonUnsubmitted);
    }
    if ($("#amnuReturnOrderApproveLink") !== undefined) {
        $("#amnuReturnOrderApproveLink").html(strResCommonApproved);
    }
    if ($("#amnuReturnOrderReceiveLink") !== undefined) {
        $("#amnuReturnOrderReceiveLink").html(strResOrderReturnListTab);
    }
    if ($("#amnuChangeReturnOrderLink") !== undefined) {
        $("#amnuChangeReturnOrderLink").html(strResOrderChangeReturnOrder);
    }
    for (var i = 0; i < obj.length; i++) {
        var OrderLinkCnt = 0;
        if (obj[i].Status == 'UnSubmitted') {
            if (gblActionName.toLowerCase() == 'returnorderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divUnsubmitted").html(obj[i].RecCircleCount);
                    $("#divUnsubmitted").css('display', 'block');
                    $("#divUnsubmitted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuReturnOrderUnsubmittedLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuReturnOrderUnsubmittedLink").html(strResCommonUnsubmitted + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'ToBeApproved') {
            if (gblActionName.toLowerCase() == 'returnorderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divSubmittted").html(obj[i].RecCircleCount);
                    $("#divSubmittted").css('display', 'block');
                    $("#divSubmittted").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuReturnOrderApproveLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuReturnOrderApproveLink").html(strResCommonApproved + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        else if (obj[i].Status == 'InReturn') {
            if (gblActionName.toLowerCase() == 'returnorderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divApproved").html(obj[i].RecCircleCount);
                    $("#divApproved").css('display', 'block');
                    $("#divApproved").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }
            if ($("#amnuReturnOrderReceiveLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuReturnOrderReceiveLink").html(strResOrderReturnListTab + '(' + obj[i].RecCircleCount + ')');
                }
            }
        }
        //else if (obj[i].Status == 'Transmitted') {
        //    if (gblActionName == 'ReturnOrderList') {
        //        if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
        //     $("#divChangeOrder").html(obj[i].RecCircleCount);
        //     $("#divChangeOrder").css('display', 'block');
        //     $("#divChangeOrder").parent().find("#spnTabName").css('padding-right', '25px');
        //}
        //     }
        //     if ($("#amnuChangeReturnOrderLink") !== undefined) {
        //         if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
        //             $("#amnuChangeReturnOrderLink").html(strResOrderChangeReturnOrder + '(' + obj[i].RecCircleCount + ')');
        //         }
        //     }
        // }
    }

}

function SetItemCountInMenu() {
    if (HasItemModuleRights == 'True') {


        $.ajax({
            url: '/Master/GetTotalItemMasterNumber',
            timeout: 0,
            type: "GET",
            success: function (response) {

                if (response != null && response.result != null && response.result.length > 0) {
                    var iCount = parseInt(response.result);
                    if (!isNaN(iCount) && iCount > 0) {
                        $("#lnkItemList").html(ItemRes + "(" + iCount + ")");
                    }
                    else {
                        $("#lnkItemList").html(ItemRes);
                    }
                }
                else {
                    $("#lnkItemList").html(ItemRes);
                }
            },
            error: function (response) {
                $("#lnkItemList").html(ItemRes);
            }
        });
    }
}

function SetReceiveToolRedCount(obj) {
    if (gblActionName.toLowerCase() == 'toolassetorderlist') {
        $("#divApproved").css('display', 'none');
        $("#divApproved").html('');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');
    }

    if (gblControllerName == 'ReceiveToolAsset') {
        $("#divIncompleteItems").css('display', 'none');
        $("#divIncompleteItems").html('');
        $("#divIncompleteItems").parent().find("#spnTabName").css('padding-right', '5px');
    }

    if ($("#amnuReceiveToolIncomplete") !== undefined) {
        $("#amnuReceiveToolIncomplete").html('Incomplete');
    }

    for (var i = 0; i < obj.length; i++) {
        var OrderLinkCnt = 0;
        if (obj[i].Status == 'InComplete') {
            if (gblControllerName == 'ReceiveToolAsset') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divIncompleteItems").html(obj[i].RecCircleCount);
                    $("#divIncompleteItems").css('display', 'block');
                    $("#divIncompleteItems").parent().find("#spnTabName").css('padding-right', '25px');
                }
            }

            if ($("#amnuReceiveToolIncomplete") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuReceiveToolIncomplete").html('Incomplete' + '(' + obj[i].RecCircleCount + ')');
                }
            }
            if ($("#amnuChangeToolOrderLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuToolOrderReceiveLink").html(strResLayoutReceive + '(' + obj[i].RecCircleCount + ')');
                }
            }
            if (gblActionName.toLowerCase() == 'toolassetorderlist') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divApproved").html(obj[i].RecCircleCount);
                    $("#divApproved").css('display', 'block');
                    $("#divApproved").parent().find("#spnTabName").css('padding-right', '25px');
                }


            }
        }

    }
}