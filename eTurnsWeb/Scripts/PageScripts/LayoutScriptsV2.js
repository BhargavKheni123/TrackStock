var oTableReorderId = '';
var oTableReorderColumnSetupFor = '';
var DashboardLiId = '';
var DashboardActiveTabs = [];

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
    catch(ex)
    {
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

    $.ajaxSetup({ cache: false });
    FillTopEnterprise();
    FillTopCompanies();
    FillTopRooms();
    FillCompanyLanguages();
    SetReplenishRedCount();
    SetConsumeRedCount();
    SetItemCountInMenu();
    var intervaltemp = setInterval("KeepSessionAlive()", interval);
    SetPageMode();
    $('div.DivLoadingProcessing').css('height', $(document).height().toString() + 'px');
    //$("#ColumnOrderSetup").click(function () {
    //    //debugger;

    //    $("#ColumnSortableModal").dialog("open");
    //    return false;
    //});
    $("a#ColumnOrderSetup").off("click");
    $(document).on("click", "a#ColumnOrderSetup", function () {
        $("#ColumnSortableModal").dialog("open");
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
                               window.location = window.location;
                           }
        });

    });
    $("#btnPollAll").click(function () {
        $('#DivLoading').show();
        $.ajax({
            url: urlSetPollAllTrue,
            type: 'Get',
            data: {},
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response.Status = "ok") {
                    $('#DivLoading').hide();
                    showNotificationDialog();
                    $("#spanGlobalMessage").text(response.Message);
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                }
                else if (response.Status == "fail") {
                    $('#DivLoading').hide();
                    showNotificationDialog();
                    $("#spanGlobalMessage").text(response.Message);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');

                }

            },
            error: function (xhr) {
                $('#DivLoading').hide();
                showNotificationDialog();
                $("#spanGlobalMessage").text("Unknown error");
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');

            }
        });


    });



    $("#ColumnOrderSetup_Context").click(function () {

        $("#ColumnSortableModal").dialog("open");
        return false;
    });

    $("#ancLogout").click(function () {
        logout();
    });
    $('div').on('dblclick', function (e) {

        // do stuff

        if (window.getSelection) {
            window.getSelection().removeAllRanges();
        } else if (document.selection) {
            document.selection.empty();
        }
    });
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
           
            if (window.location.hash.toLowerCase() == "#unsubmitted" ) {
                $("#tab2").click();
            }
            else if (window.location.hash.toLowerCase() == "#approve" || window.location.hash.toLowerCase() == "#submittted") {
                $("#tab3").click();
            }
            else if (window.location.hash.toLowerCase() == "#approved" || window.location.hash.toLowerCase() == "#changeorder") {
                $("#tab4").click();
            }
            else if (window.location.hash.toLowerCase() == "#receive" ) {
                $("#tab7").click();
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
                                   else
                                   {
                                       window.location.reload();
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
                                       window.location.reload();
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
                                       window.location.reload();
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
            if (gblActionName === "OrderList") {
                $('#txtOrderFilter').focus();
            }
            else if (gblActionName === "TransferList") {
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
    $(window).load(function () {
        $('#global_filter').val('');
    });


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

        if (gblActionName === "KitList") {
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
    if (gblActionName === "OrderList") {
        $('#txtOrderFilter').focus();
        $('#txtOrderFilter').select();

    }
    else if (gblActionName === "TransferList") {
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
                            $("#amnuRequisitionLink").text(strResLayoutRequisitions + '(' + result.RequisitionMenuLinkCount + ')');
                        }
                        else {
                            $("#amnuRequisitionLink").text(strResLayoutRequisitions);
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
                            $("#amnuCartLink").text(strResLayoutCart + '(' + result.CartMenuLinkCount + ')');
                           
                            if (!isNaN(parseInt(result.CartOrderMenuLinkCount)) && parseInt(result.CartOrderMenuLinkCount) > 0) {
                                $("#amnuCartLinkOrders").text(strResLayoutCartOrders + '(' + result.CartOrderMenuLinkCount + ')');
                            }
                            else
                            {
                                $("#amnuCartLinkOrders").text(strResLayoutCartOrders);
                            }
                            if (!isNaN(parseInt(result.CartTransMenuLinkCount)) && parseInt(result.CartTransMenuLinkCount) > 0) {
                                $("#amnuCartLinkTransfers").text(strResLayoutCartTransfers + '(' + result.CartTransMenuLinkCount + ')');
                            }
                            else
                            {
                                $("#amnuCartLinkTransfers").text(strResLayoutCartTransfers );
                            }

                            if (typeof (gblActionName) != undefined) {
                                if (gblActionName != undefined) {
                                    if (gblActionName.toLowerCase() == "cartitemlist") {
                                        $("div#divUnSubmittted").html(result.CartMenuLinkCount);
                                        $("div#divUnSubmittted").show();
                                        $("#tab5").find("span#spnTabName").css("padding-right", "25px");

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
                            $("#tab5").find("span#spnTabName").css("padding-right", "5px");
                            $("#amnuCartLink").text(strResLayoutCart);
                            $("#amnuCartLinkOrders").text(strResLayoutCartOrders);
                            $("#amnuCartLinkTransfers").text(strResLayoutCartTransfers);
                        }
                    }

                    if ($("#amnuOrderLink") !== undefined) {
                        if (!isNaN(parseInt(result.OrderMenuLinkCount)) && parseInt(result.OrderMenuLinkCount) > 0) {
                            $("#amnuOrderLink").text(strResLayoutOrders + '(' + result.OrderMenuLinkCount + ')');
                        }
                        else {
                            $("#amnuOrderLink").text(strResLayoutOrders);
                        }
                        SetOrderRedCount(result.OrderRedCount);
                        
                        if ($("#amnuReceiveLink") === undefined) {
                            SetReceiveRedCount(result.ReceiveRedCount);
                        }
                    }

                    if ($("#amnuReturnOrderLink") !== undefined) {
                        if (!isNaN(parseInt(result.OrderMenuLinkCount)) && parseInt(result.ReturnOrderMenuLinkCount) > 0) {
                            $("#amnuReturnOrderLink").text(strResLayoutReturnOrders + '(' + result.ReturnOrderMenuLinkCount + ')');
                        }
                        else {
                            $("#amnuReturnOrderLink").text(strResLayoutReturnOrders);
                        }
                        SetReturnOrderRedCount(result.ReturnOrderRedCount);
                    }


                    if ($("#amnuReceiveLink") !== undefined) {
                        if (!isNaN(parseInt(result.ReceiveMenuLinkCount)) && parseInt(result.ReceiveMenuLinkCount) > 0) {
                            $("#amnuReceiveLink").text(strResLayoutReceive + '(' + result.ReceiveMenuLinkCount + ')');
                        }
                        else {
                            $("#amnuReceiveLink").text(strResLayoutReceive);
                        }
                        SetReceiveRedCount(result.ReceiveRedCount);
                    }

                    if ($("#amnuTransferLink") !== undefined) {
                        if (!isNaN(parseInt(result.TransferMenuLinkCount)) && parseInt(result.TransferMenuLinkCount) > 0) {
                            $("#amnuTransferLink").text(strResLayoutTransfer + '(' + result.TransferMenuLinkCount + ')');
                        }
                        else {
                            $("#amnuTransferLink").text(strResLayoutTransfer);
                        }
                        SetTransferRedCount(result.TransferRedCount);
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
    if (gblActionName == 'OrderList') {
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
    
    for (var i = 0; i < obj.length; i++) {
        var OrderLinkCnt = 0;
        if (obj[i].Status == 'UnSubmitted') {
            if (gblActionName == 'OrderList') {
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
            if (gblActionName == 'OrderList') {
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
        else if (obj[i].Status == 'Transmitted') {
            if (gblActionName == 'OrderList') {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#divChangeOrder").html(obj[i].RecCircleCount);
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

function SetReceiveRedCount(obj) {

    if (gblActionName == 'OrderList') {
        $("#divApproved").css('display', 'none');
        $("#divApproved").html('');
        $("#divApproved").parent().find("#spnTabName").css('padding-right', '5px');
    }
    
    if (gblControllerName == 'Receive') {
        $("#divIncompleteItems").css('display', 'none');
        $("#divIncompleteItems").html('');
        $("#divIncompleteItems").parent().find("#spnTabName").css('padding-right', '5px');
    }

    if ($("#amnuReceiveIncomplete") !== undefined) {
        $("#amnuReceiveIncomplete").html('Incomplete');
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
                    $("#amnuReceiveIncomplete").html('Incomplete' + '(' + obj[i].RecCircleCount + ')');
                }
            }
            if ($("#amnuChangeOrderLink") !== undefined) {
                if (!isNaN(parseInt(obj[i].RecCircleCount)) && parseInt(obj[i].RecCircleCount) > 0) {
                    $("#amnuOrderReceiveLink").html(strResLayoutReceive + '(' + obj[i].RecCircleCount + ')');
                }
            }
            if (gblActionName == 'OrderList') {
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

function SetReturnOrderRedCount(obj) {

    if (gblActionName == 'ReturnOrderList') {
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
            if (gblActionName == 'ReturnOrderList') {
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
            if (gblActionName == 'ReturnOrderList') {
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
            if (gblActionName == 'ReturnOrderList') {
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
	 // $("#lnkItemList").html(ItemRes);
     $.ajax({
        url: '/Master/GetItemCountFromRoom',
        timeout: 0,
        type: "GET",
        success: function (response) {
            if (response.result.length > 0) {
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