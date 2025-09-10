var chkSuggestedOrder = $('#chkSuggestedOrder');
var chkSuggestedTransfer = $('#chkSuggestedTransfer');
var chkIsTax1Parts = $('#chkIsTax1Parts');
var chkIsTax1Labor = $('#chkIsTax1Labor');
var txtTax1name = $('#txtTax1name');
var txtTax1Rate = $('#txtTax1Rate');
var chkIsTax2onTax1 = $('#chkIsTax2onTax1');
var chkIsTax2Parts = $('#chkIsTax2Parts');
var chkIsTax2Labor = $('#chkIsTax2Labor');
var txtTax2name = $('#txttax2name');
var txtTax2Rate = $('#txtTax2Rate');
var PageMode = $('#hdnPageMode').val();
var chkReturnSuggestedOrder = $('#chkReturnSuggestedOrder');

$("form").submit(function (e) {
    var res = false;
    $('#DivLoading').show();
    RemoveLeadingTrailingSpace("frmRoom");
    var result = true;
    if (result == true) {
        $.validator.unobtrusive.parse("#frmRoom");
        res = $(this).valid();
        if ($(this).valid()) {

            rememberUDFValues($("#hdnPageName").val(), RoomId);
        }
        else
        {
            $('#DivLoading').hide();
        }
        e.preventDefault();
        
    }
    else { $('#DivLoading').hide();
        return false;
    }
    if(res)
    {
        $('#DivLoading').show();
    }
    else
    {
        $('#DivLoading').hide();
    }
});


function onSuccess(response) {
   $('#DivLoading').hide();
    if (typeof (response.refressPage) != "undefined" && response.refressPage !== undefined && response.refressPage === true) {
        //window.location.reload();
        window.location.href = GetRoomListURL;
    }
    else {
        IsRefreshGrid = false;
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(response.Message);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        var idValue = $("#hiddenID").val();

        if (response.Status == "fail") {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#RoomName").focus();
        }
        else if (idValue == 0) {
            $("#RoomName").focus();
            //            clearControls('frmRoom');
            if (response.Status == "duplicate")
                $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
            else {
                //                    clearControls('frmRoom');
                setDefaultUDFValues($("#hdnPageName").val(), RoomId);
                //window.location.reload();
                window.location.href = GetRoomListURL;
            }
            FillTopRooms();

        }
        else if (idValue > 0) {
            if (response.Status == "duplicate") {
                $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
                $("#RoomName").focus();
            }
            else {
                //                    clearControls('frmRoom');
                SwitchTextTab(0, 'RoomCreate', 'frmRoom');
                //window.location.reload();
                window.location.href = GetRoomListURL;

            }

        }
    }
}
function onFailure(message) {
   $('#DivLoading').hide();
    $("#spanGlobalMessage").html(message.statusText);
    showNotificationDialog();
    $("#RoomName").focus();
}

jQuery(function () {

    setTimeout(function () {
        if (RoomId == 0 && $("input[type='checkbox'][name='IsRoomActive']").prop("checked")) {
            $("input[type='checkbox'][name='IsRoomActive']").prop("checked", false);
        }
    }, 200);

    $('#txtExample').timepicker({ timeFormat: 'hh:mm tt' });
    $('#txtAutoCreateTransferTime').timepicker({ timeFormat: 'hh:mm tt' });
    if (RoomId == 0) {
        chkSuggestedOrder.prop("checked", "checked");
        //ToggleSuggestedControls(1);
    }
    $("#txtLicenseBilled").datepicker(
        {
            dateFormat: RoomDateJSFormat,
            changeMonth: true,
            changeYear: true,
            minDate: 0,
            maxDate: "+1Y"

        });

//    $(".mainForm").on('focus', "input[type='text'][name='DefaultSupplierName']", function (e) {
//        var objCurtxt = $(this);
//        $(this).autocomplete({
//            source: function (request, response) {
//                $.ajax({
//                    url: GetSupplierURL,
//                    contentType: 'application/json',
//                    dataType: 'json',
//                    data: {
//                        featureClass: "P",
//                        style: "full",
//                        maxRows: 1000,
//                        NameStartWith: request.term,
//                        EntCmpRoom: EntCmpRoom
//                    },
//                    success: function (data) {
//                        response($.map(data, function (item) {
//                            return {
//                                label: item.SupplierName,
//                                value: item.SupplierName,
//                                selval: item.ID
//                            }
//                        }));
//                    },
//                    error: function (err) {

//                    }
//                });
//            },
//            autoFocus: false,
//            minLength: 1,
//            select: function (event, ui) {

//                //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBinName']").val(ui.item.label);
//            },
//            open: function () {
//                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
//            },
//            close: function () {
//                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
//                $(objCurtxt).trigger("change");
//            },
//            change: function (event, ui) {
//                //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBinName']").val($(objCurtxt).val());
//            }
//        });
//    });

    $(".mainForm").on('focus', "input[type='text'][name='Country']", function (e) {
        var objCurtxt = $(this);
        $(this).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: GetCountryURL,
                    contentType: 'application/json',
                    dataType: 'json',
                    data: {
                        featureClass: "P",
                        style: "full",
                        maxRows: 1000,
                        NameStartWith: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.CountryName,
                                value: item.CountryName,
                                selval: item.ID
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

                //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBinName']").val(ui.item.label);
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                $(objCurtxt).trigger("change");
            },
            change: function (event, ui) {
                //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBinName']").val($(objCurtxt).val());
            }
        });
    });

    $('#ancLicenseBilled').click(function () {
        $('#txtLicenseBilled').focus();
    });
    EnableTax1Controls();
    EnableTax2Controls();

    $('#btnCancel').click(function (e) {
        $('#DivLoading').hide();
        SwitchTextTab(0, 'RoomCreate', 'frmRoom');
        if (oTable !== undefined && oTable != null) {
            oTable.fnDraw();
        }
        $('#NarroSearchClear').click();
    });
    checkRememberUDFValues($("#hdnPageName").val(), RoomId);
    //chkSuggestedOrder.click(function (e) {
    //    var Ischecked = chkSuggestedOrder.attr('checked') ? true : false;
    //    if (Ischecked == true) {
    //        ToggleSuggestedControls(1);
    //    }
    //});

    //chkSuggestedTransfer.click(function (e) {
    //    var Ischecked = chkSuggestedTransfer.attr('checked') ? true : false;
    //    if (Ischecked == true) {
    //        ToggleSuggestedControls(2);
    //    }
    //});
    chkIsTax1Labor.click(function (e) {
        EnableTax1Controls();
    });

    chkIsTax1Parts.click(function (e) {
        EnableTax1Controls();
    });

    chkIsTax2Parts.click(function (e) {
        EnableTax2Controls();
    });
});


function ToggleSuggestedControls(Control) {
    if (Control == 1) {
        chkSuggestedOrder.attr("checked", true);
        chkSuggestedTransfer.removeAttr('checked');
    }
    else if (Control == 2) {
        chkSuggestedOrder.removeAttr('checked');
        chkSuggestedTransfer.attr("checked", true);
    }
}




function Validations() {

    var IsTax1Parts = chkIsTax1Parts.attr('checked') ? true : false;
    var IsTax1Labor = chkIsTax1Labor.attr('checked') ? true : false;

    var IsTax2Parts = chkIsTax2Parts.attr('checked') ? true : false;
    var IsTax2Labor = chkIsTax2Labor.attr('checked') ? true : false;


    if ((IsTax1Parts == true || IsTax1Labor == true) && txtTax1name.val() == '') {
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgTaxValidation);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $('#txtTax1name').focus();
        $('#DivLoading').hide();
        return false;
    }
    if ((IsTax1Parts == true || IsTax1Labor == true) && txtTax1Rate.val() == '') {
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgTaxPercentRequired);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $('#txtTax1Rate').focus();
        $('#DivLoading').hide();
        return false;
    }


    if ((IsTax2Parts == true || IsTax2Labor == true) && txtTax2name.val() == '') {
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgTaxTwoRequired);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $('#txtTax2name').focus();
        $('#DivLoading').hide();
        return false;
    }
    if ((IsTax2Parts == true || IsTax2Labor == true) && txtTax2Rate.val() == '') {
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgTaxTwoPercentRequired);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $('#txtTax2Rate').focus();
        $('#DivLoading').hide();
        return false;
    }
    return true;
}

function EnableTax1Controls() {
    var IsTax1Parts = chkIsTax1Parts.attr('checked') ? true : false;
    var IsTax1Labor = chkIsTax1Labor.attr('checked') ? true : false;

    if (IsTax1Parts == true || IsTax1Labor == true) {
        txtTax1name.removeAttr("disabled");
        txtTax1Rate.removeAttr("disabled");
    }
    else {
        txtTax1Rate.attr("disabled", true);
        txtTax1name.attr("disabled", true);
    }
}

function EnableTax2Controls() {
    var IsTax2Parts = chkIsTax2Parts.attr('checked') ? true : false;
    var IsTax2Labor = chkIsTax2Labor.attr('checked') ? true : false;

    if (IsTax2Parts == true || IsTax2Labor == true) {
        txtTax2name.removeAttr("disabled");
        txtTax2Rate.removeAttr("disabled");
        chkIsTax2onTax1.removeAttr("disabled");
    }
    else {
        txtTax2Rate.attr("disabled", true);
        txtTax2name.attr("disabled", true);
        chkIsTax2onTax1.attr("disabled", true);
    }
}

