$(document).ready(function () {
    setTimeout(function () {
        $('body').on('mouseover', "input[type='text'][id='SupplierColor']", function () {
            if ($(this).hasClass('colorPicker') === false) {
                $(this).colorpicker({ color: $(this).val() });
            }
        });

        $('form').areYouSure();



        jQuery.validator.addMethod("multiemails", function (value, element) {
            if (this.optional(element)) // return true on optional element
                return true;
            var emails = value.split(/[,]+/); // split element by , and ;
            valid = true;
            for (var i in emails) {
                value = emails[i];
                valid = valid && jQuery.validator.methods.email.call(this, $.trim(value), element);
            }
            return valid;
        }, jQuery.validator.messages.multiemails);

        jQuery.validator.unobtrusive.adapters.add('multiemails', {}, function (options) {
            options.rules['multiemails'] = true;
            options.messages['multiemails'] = options.message;
        });



        $("#SupplierImage").change(function () {
            readURL(this);
        });

        $('body').on('focus', ".datePicker", function () {
            if ($(this).hasClass('hasDatepicker') === false) {
                $(this).datepicker({ dateFormat: RoomDateJSFormat });
            }
        });

        if (SupImageType == 'ImagePath' || SupImageType == '') {
            $("#SupplierImage").show();
            $("#ExternalURL").hide();
        }
        else {
            $("#SupplierImage").hide();
            $("#ExternalURL").show();
        }

        $("#btnSave").click(function (event) {
            var errmsg = "";

            switch ($("Select#POAutoSequence").val()) {
                case "":
                    if ($("input[type='radio'][name='ScheduleMode']:checked").length > 0) {
                        if ($("input[type='radio'][name='ScheduleMode']:checked").val() > 0) {
                            errmsg = errmsg + errmsgBlankOrderNumbering + "\r\t\n";
                        }
                    }
                    break;
                case "0":
                    if ($("input[type='radio'][name='ScheduleMode']:checked").length > 0) {
                        if ($("input[type='radio'][name='ScheduleMode']:checked").val() > 0) {
                            errmsg = errmsg + errmsgBlankOrderNumbering + "\r\t\n";
                        }
                    }
                    break;
                case "1":
                    if ($.trim($("#NextOrderNo").val()) == "") {
                        errmsg = errmsg + errmsgBlankFixedPurchaseNumberType + "\r\t\n"
                    }
                    break;
                case "2":
                    if ($("input[type='radio'][name='ScheduleMode']:checked").length > 0) {
                        if ($("input[type='radio'][name='ScheduleMode']:checked").val() > 0) {
                            if ($("table#SupplierBlanketPO").find("tbody tr").not(".ng-hide,.norecfound").find("input[type='text'][id='txtBlanketPO']").length < 1 && $("#POAutoSequence").val() == 2) {
                                errmsg = errmsg + errmsgBlanketOrderNumbering + "\r\t\n";
                            }
                        }
                    }
                    break;              
            }
            switch ($("select#PullPurchaseNumberType").val()) {

                case "0":
                    //if ($("input[type='radio'][name='Pull_ScheduleMode']:checked").length > 0) {
                    //    if ($("input[type='radio'][name='Pull_ScheduleMode']:checked").val() != 6) {
                    //        errmsg = errmsg + errmsgBlankOrderNumbering + "\r\t\n";
                    //    }
                    //}
                    break;
                case "1":
                    if ($.trim($("#LastPullPurchaseNumberUsed").val()) == "") {
                        errmsg = errmsg + errmsgBlankFixedPullPurchaseNumberType + "\r\t\n"
                    }
                    break;
                case "2":
                    if ($("input[type='radio'][name='Pull_ScheduleMode']:checked").length > 0) {
                        if ($("input[type='radio'][name='Pull_ScheduleMode']:checked").val() != 6) {
                            if ($("table#SupplierBlanketPO").find("tbody tr").not(".ng-hide,.norecfound").find("input[type='text'][id='txtBlanketPO']").length < 1 && $("#PullPurchaseNumberType").val() == 2) {
                                errmsg = errmsg + errmsgBlanketOrderPurchaseNumber + "\r\t\n";
                                
                            }
                        }
                    }
                    break;             
            }
            if (errmsg != "") {
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(errmsg);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                event.preventDefault();
            }


        });





        if ($("#POAutoSequence").val() == "") {
            $("#NextOrderNo").val('');
        }



        if ($("#PullPurchaseNumberType").val() == "1") {
            if ($("#LastPullPurchaseNumberUsed").val().trim() == '') {
                alert(errmsgBlankFixedPullPurchaseNumberType);
                return false;
            }
        }



        $("#POAutoSequence").change(function () {
            if ($(this).val() == "") {
                $("#NextOrderNo").val('');
            }
        });



        $('#btnSupCancel').click(function (e) {
            //  if (IsRefreshGrid)
            //  $('#NarroSearchClear').click();
            if ($('#NewMasterPopUP') != undefined && $('#NewMasterPopUP').length > 0)
                $('#NewMasterPopUP').dialog('close');
            else {
                SwitchTextTab(0, 'SupplierCreate', 'frmSupplier');
                if (oTable !== undefined && oTable != null) {
                    oTable.fnDraw();
                }
            }
        });
        checkRememberUDFValues($("#hdnPageName").val(), SupplierID);




    }, 300);
    DoNGbootstrap();
});

$("#frmSupplier").submit(function (e) {


    $.validator.unobtrusive.parse("#frmSupplier");
    var Supfrmvalidator = $("#frmSupplier").validate();

    $(".userHead").on("blur", "input[data-val='true'],select[data-val='true'],textarea[data-val='true']", function () {
        Supfrmvalidator.element("#" + $(this).attr("id"));
    });

    if ($(this).valid()) {
        rememberUDFValues($("#hdnPageName").val(), SupplierID);
    }
    e.preventDefault();

});

function ShowImage(currentRadio) {
    var currentId = $(currentRadio).attr("id");

    if (currentId == "ImagePath") {
        $("#SupplierImage").show();
        $("#ExternalURL").hide();
        setImagePath();

        // $("img#previewHolder").attr('src', '/Content/images/no-image.jpg');
        // $("#ImageExternalURL").val('');
    }
    else {
        CheckValidURLForImage($("input#ImageExternalURL"));
        //  $("#SupplierImage").val('');
        $("#SupplierImage").hide();
        $("#ExternalURL").show();
        //  $("img#previewHolder").attr('src','/Content/images/no-image.jpg');
    }
}

function setImagePath() {
    $('#previewHolder').attr('src', $("input#currentpath").val());
}

function readURL(input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var filePath = $("#SupplierImage").val().split('\\').pop();

            if (filePath.toString().indexOf("&") >= 0 || filePath.toString().indexOf("<") >= 0 || filePath.toString().indexOf(">") >= 0
                || filePath.toString().indexOf("*") >= 0 || filePath.toString().indexOf(":") >= 0
                || filePath.toString().indexOf("?") >= 0) {
                alert("Please select correct file name.");
                $("input#currentpath").val('');
            }
            else {
                $('#previewHolder').attr('src', e.target.result);
                $("input#currentpath").val(e.target.result);
            }
        }

        reader.readAsDataURL(input.files[0]);
    }
}

function CheckValidURLForImage(curobj) {
    var strURL = $(curobj).val();

    if (strURL != '' && strURL != null) {
        $("<img>", {
            src: strURL,
            error: function () {
                alert('Invalid URL. please enter valid URL');
                $(curobj).val("");
                curobj.focus();
            },
            load: function () {
                $('#previewHolder').attr('src', strURL);
            }
        });
    }
    else {
        $('#previewHolder').attr('src', '/Content/images/no-image.jpg');
    }
    return false;
}

function onBegin() {
    if (!CheckDateSupplierBlanketPO()) {
        return false;
    }
}

function CheckDateSupplierBlanketPO() {
    var TempSupplierBlanketPO = $("#SupplierBlanketPO").find("tbody tr").not(".ng-hide,.norecfound");
    var iCountSupD = 0;
    if (TempSupplierBlanketPO != null && TempSupplierBlanketPO.length > 0) {

        for (var k = 0; k < TempSupplierBlanketPO.length; k++) {
            //End Date Should be greater than System Date
            if (TempSupplierBlanketPO[k].cells[2].childNodes[1].value != '') {
                var myDate = new Date();
                var prettyDate = (myDate.getMonth() + 1) + '/' + myDate.getDate() + '/' + myDate.getFullYear();

                var ToDate = TempSupplierBlanketPO[k].cells[2].childNodes[1].value;

                // var begD = $.datepicker.parseDate(RoomDateJSFormat, prettyDate);
                begD = myDate;

                var endD = $.datepicker.parseDate(RoomDateJSFormat, ToDate);


                if (endD < begD) {

                    TempSupplierBlanketPO[k].cells[2].childNodes[1].focus();
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    $("#spanGlobalMessage").html('End date should be greater then today date.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    return false;
                }
                else if (begD.toString() == endD.toString()) {

                    var dteString = begD.getFullYear() + "/" + (begD.getMonth() + 1) + "/" + begD.getDate();
                    var begT = new Date(dteString + " " + begD);
                    var endT = new Date(dteString + " " + endD);

                    if (begT > endT) {
                        TempSupplierBlanketPO[k].cells[2].childNodes[1].value = "";
                        TempSupplierBlanketPO[k].cells[2].childNodes[1].focus();
                        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        $("#spanGlobalMessage").html('End date should be greater then today date.');
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        return false;
                    }
                }
            }


            if ((TempSupplierBlanketPO[k].cells[1].childNodes[1].value != '') && (TempSupplierBlanketPO[k].cells[2].childNodes[1].value != '')) {
                var startdate = TempSupplierBlanketPO[k].cells[1].childNodes[1].value;
                var enddate = TempSupplierBlanketPO[k].cells[2].childNodes[1].value;

                var begD = $.datepicker.parseDate(RoomDateJSFormat, startdate);
                var endD = $.datepicker.parseDate(RoomDateJSFormat, enddate);

                if (begD > endD) {

                    TempSupplierBlanketPO[k].cells[2].childNodes[1].value = "";
                    TempSupplierBlanketPO[k].cells[2].childNodes[1].focus();

                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    $("#spanGlobalMessage").html('End date should be greater than start date.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    return false;
                }
                else if (begD.toString() == endD.toString()) {

                    var dteString = begD.getFullYear() + "/" + (begD.getMonth() + 1) + "/" + begD.getDate();
                    var begT = new Date(dteString + " " + begD);
                    var endT = new Date(dteString + " " + endD);

                    if (begT > endT) {
                        TempSupplierBlanketPO[k].cells[2].childNodes[1].value = "";
                        TempSupplierBlanketPO[k].cells[2].childNodes[1].focus();
                        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        $("#spanGlobalMessage").html('End date should be greater than start date.');
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        return false;
                    }
                }

            }
        }
    }
    return true;
}

function onSupSuccess(response) {
    IsRefreshGrid = true;
    //$('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    showNotificationDialog();
    $("#spanGlobalMessage").html(response.Message);
    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    var idSupValue = $("#hiddenSupID").val();

    if (response.Status == "fail") {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        //$("#SupplierNameNew").val("");
        $("#SupplierNameNew").focus();
    }
    else if (idSupValue == 0) {
        $("#SupplierNameNew").focus();
        if (response.Status == "duplicate")
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
        else {
            if ($('#NewMasterPopUP') != undefined && $('#NewMasterPopUP').length > 0) {
                $('#NewMasterPopUP').data("IDVal", response.NewIDForPopUp + '~' + $("#SupplierNameNew").val()).dialog('close');
            }
            else {
                //                    clearControls('frmSupplier');
                setDefaultUDFValues($("#hdnPageName").val(), SupplierID);
                //ShowNewTab('SupplierCreate', 'frmSupplier');
            }
            if ($("#SupplierImage").val() != "") {
                ajaxFileUpload(response.SupplierId);
            }
            else {
                ShowNewTab('/Master/SupplierCreate?isforbom=' + isForBom, 'frmSupplier');
            }
        }
    }
    else if (idSupValue > 0) {
        if (response.Status == "duplicate") {
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
            //  $("#SupplierNameNew").val("");
            $("#SupplierNameNew").focus();
        }
        else {
            //                clearControls('frmSupplier');

            SwitchTextTab(0, 'SupplierCreate', 'frmSupplier');
        }

        if ($("#SupplierImage").val() != "") {
            ajaxFileUpload(response.SupplierId);
        }
        else {
            SwitchTextTab(0, 'SupplierCreate', 'frmSupplier');
        }
    }
}

function ajaxFileUpload(retid) {
    $.ajaxFileUpload
    (
        {
            url: '/api/fileupload/PostSupplierFile/' + retid,
            secureuri: false,
            type: "POST",
            fileElementId: 'SupplierImage',
            dataType: 'json',
            success: function (data, status) {
                ShowNewTab('/Master/SupplierCreate?isforbom=' + isForBom, 'frmSupplier');
                //window.location.reload();
                //			        if (mode == "Add") {
                //			            location.href = location.href;
                //			        }
                //			        if (typeof (data.error) != 'undefined') {
                //			            if (data.error != '') {
                //			                alert(data.error);
                //			            } else {
                //			                alert(data.msg);
                //			            }
                //			        }
            },
            error: function (data, status, e) {
                ShowNewTab('/Master/SupplierCreate?isforbom=' + isForBom, 'frmSupplier');
                //window.location.reload();
            }
        }
    )

    return false;

}

function onFailure(message) {

    $("#spanGlobalMessage").html(message.statusText);
    //$('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    showNotificationDialog();
    $("#SupplierNameNew").focus();
}

function DoNGbootstrap() {
    var scope = angular.element($("#CtabNew")).scope();
    if (scope != undefined) {
        var injector = $('[ng-app]').injector();
        var $compile = injector.get('$compile');
        $('#CtabNew').html($compile($('#CtabNew').html())(scope));
        scope.$apply();

        //$('#liSupplierBlanketPODetails').html($compile($('li#liSupplierBlanketPODetails').html())(scope));
        //$('#liSupplierAccountDetails').html($compile($('li#liSupplierAccountDetails').html())(scope));
    }
}
