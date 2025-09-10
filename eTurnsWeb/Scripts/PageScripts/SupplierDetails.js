$(document).ready(function () {
    setTimeout(function () {

        //$('body').on('mouseover', "input[type='text'][id='SupplierColor']", function () {
        //    if ($(this).hasClass('colorPicker') === false) {
        //        $(this).colorpicker({ color: $(this).val() });
        //    }
        //});

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



        // $("#SupplierImage").change(function () {
        //     readURL(this);
        // });

        $('body').on('focus', ".datePicker", function () {
            if ($(this).hasClass('hasDatepicker') === false) {
                $(this).datepicker({ dateFormat: RoomDateJSFormat });
            }
        });

        if (SupImageType == 'ImagePath' || SupImageType == '') {
            $("#SupplierImage").show();
            $("#ExternalURL").hide();
            
            if (SupImagePath != undefined && SupImagePath != null && SupImagePath != '') {
                $("#btnDeleteImage").show();
            }
            else {
                $("#btnDeleteImage").hide();
            }
        }
        else {
            $("#SupplierImage").hide();
            $("#ExternalURL").show();
            $("#btnDeleteImage").hide();
        }
        $("#SupplierImage").change(function () { 
            readURL(this);
        });
        $("#btnSave").click(function (event) {
            var errmsg = "";
            
            switch ($("Select#POAutoSequence").val()) {
                case "":
                    //if ($("input[type='radio'][name='ScheduleMode']:checked").length > 0) {
                    //    if ($("input[type='radio'][name='ScheduleMode']:checked").val() > 0) {
                    //        errmsg = errmsg + errmsgBlankOrderNumbering + "\r\t\n";
                    //    }
                    //}
                    
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
                        errmsg = errmsg + errmsgBlankFixedOrderNumberType + "\r\t\n"
                    }
                    if ($.trim($("#POAutoNrReleaseNumber").val()) == "") {
                        errmsg = errmsg + errmsgBlankReleaseNumberOrderNumberType + "\r\t\n"
                    }
                    else {
                        if ($.trim($("#POAutoNrReleaseNumber").val()) == "0") {
                            errmsg = errmsg + errmsgReleaseNumberGreaterZero + "\r\t\n"
                        }
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
            if ($("Select#POAutoSequence").val() != "1") {
                $("#POAutoNrReleaseNumber").val("");
            }
            switch ($("Select#QuoteAutoSequence").val()) {
                case "":
                    //if ($("input[type='radio'][name='ScheduleMode']:checked").length > 0) {
                    //    if ($("input[type='radio'][name='ScheduleMode']:checked").val() > 0) {
                    //        errmsg = errmsg + errmsgBlankOrderNumbering + "\r\t\n";
                    //    }
                    //}

                    break;
                case "0":
                    if ($("input[type='radio'][name='Quote_ScheduleMode']:checked").length > 0) {
                        if ($("input[type='radio'][name='Quote_ScheduleMode']:checked").val() > 0) {
                            errmsg = errmsg + errmsgBlankQuoteNumbering + "\r\t\n";
                        }
                    }

                    break;
                case "1":
                    if ($.trim($("#NextQuoteNo").val()) == "") {
                        errmsg = errmsg + errmsgBlankFixedQuoteNumberType + "\r\t\n"
                    }
                    if ($.trim($("#QuoteAutoNrReleaseNumber").val()) == "") {
                        errmsg = errmsg + errmsgBlankReleaseNumberQuoteNumberType + "\r\t\n"
                    }
                    else {
                        if ($.trim($("#QuoteAutoNrReleaseNumber").val()) == "0") {
                            errmsg = errmsg + errmsgReleaseNumberGreaterZero + "\r\t\n"
                        }
                    }

                    break;
                case "2":
                    if ($("input[type='radio'][name='Quote_ScheduleMode']:checked").length > 0) {
                        if ($("input[type='radio'][name='Quote_ScheduleMode']:checked").val() > 0) {
                            if ($("table#SupplierBlanketPO").find("tbody tr").not(".ng-hide,.norecfound").find("input[type='text'][id='txtBlanketPO']").length < 1 && $("#POAutoSequence").val() == 2) {
                                errmsg = errmsg + errmsgBlanketOrderNumbering + "\r\t\n";
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
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(errmsgBlankFixedPullPurchaseNumberType);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                //alert(errmsgBlankFixedPullPurchaseNumberType);
                return false;
            }
        }



        $("#POAutoSequence").change(function () {
            if ($(this).val() == "") {
                $("#NextOrderNo").val('');
            }
            if ($(this).val() == "1") {
                if ($.trim($("#NextOrderNo").val()) != $.trim(ExistingNextOrderNo)) {
                    $("#POAutoNrReleaseNumber").val("");
                }
                else {
                    $("#POAutoNrReleaseNumber").val(ExistingReleaseNumber);
                }
            }
             
        });
        $("#NextOrderNo").change(function () {
             
            if ($("#POAutoSequence").val() == 1) {
                
                if ($.trim($("#NextOrderNo").val()) != $.trim(ExistingNextOrderNo)) {
                    $("#POAutoNrReleaseNumber").val("");
                }
                else {
                    $("#POAutoNrReleaseNumber").val(ExistingReleaseNumber);
                }
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
            $('#NarroSearchClear').click();
        });
        checkRememberUDFValues($("#hdnPageName").val(), SupplierID);

    }, 300);
    DoNGbootstrap();

    var intNotAllowededCode = charCode.split(',');
    if (intNotAllowededCode != null && intNotAllowededCode.length > 0) {
        specialKeys = new Array();
        for (i = 0; i < intNotAllowededCode.length; i++) {
            specialKeys.push(parseInt(intNotAllowededCode[i])); //Backspace
        }
    }


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
    $('#NarroSearchClear').click();
});

function isNumberKey(evt) {
    var keyCode = [evt.keyCode || evt.which];
    if (keyCode == 8 || keyCode == 13 || keyCode == 9)
        return true;
    if (keyCode > 57 || keyCode < 48)
        return false;
    else
        return true;
}

function ShowImage(currentRadio) {
    var currentId = $(currentRadio).attr("id");

    if (currentId == "ImagePath") {
        $("#SupplierImage").show();
        $("#ExternalURL").hide();
        setImagePath();

        $("#btnDeleteImage").hide();
        if ((SupImagePath != undefined && SupImagePath != null && SupImagePath != '') || ($("input#currentpath").val() != '' && $("input#currentpath").val() != '/Content/images/no-image.jpg')) {
            $("#btnDeleteImage").show();
        }

        // $("img#previewHolder").attr('src', '/Content/images/no-image.jpg');
        // $("#ImageExternalURL").val('');
    }
    else {
        CheckValidURLForImage($("input#ImageExternalURL"));
        //  $("#SupplierImage").val('');
        $("#SupplierImage").hide();
        $("#ExternalURL").show();
        $("#btnDeleteImage").hide();
        //  $("img#previewHolder").attr('src','/Content/images/no-image.jpg');
    }
}

function DeleteSupplierImage(SupplierGUID) {

    $.ajax({
        url: '/Master/DeleteSupplierImage',
        data: { 'SupplierGUID': SupplierGUID },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'ok') {

                $('#previewHolder').attr('src', '/Content/images/no-image.jpg');
                $("input#currentpath").val('/Content/images/no-image.jpg');
                $("input#SupplierImage").val('');

                $("#btnDeleteImage").hide();
                SupImagePath = '';
            }
            else {
                Status = 'Error';
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(MsgErrorInProcess);
                //alert(MsgErrorInProcess);
            }
        }
    });
}

function setImagePath() {
    $('#previewHolder').attr('src', $("input#currentpath").val());
}

function readURL(input) {

   
    if (input.files && input.files[0]) {

        var validExtension = CommonFileExtension.split(',');
        var strValidationMessage = "";
        var fileExt = input.files[0].name;
        fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
        if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
            strValidationMessage = strValidationMessage + input.files[0].name + " " + MsgInvalidFileSelected;
        }
        if (strValidationMessage != "") {
            alert(strValidationMessage + MsgvalidFileList.replace("{0}", validExtension.toString()));
            $('#previewHolder').attr('src', '/Content/images/no-image.jpg');
            $("input#currentpath").val('/Content/images/no-image.jpg');
            $("input#SupplierImage").val('');
            $("#btnDeleteImage").hide();
            return;
        }

        var isError = false;
        var objFile = input.files[0];

        for (var n = 0; n < specialKeys.length; n++) {
            if (objFile.name.toString().lastIndexOf(String.fromCharCode(specialKeys[n])) >= 0) {
                isError = true;
                break;
            }
        }


        if (isError == true) {
            showNotificationDialog();
            //alert("Please select correct file name.");
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgValidFileName);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $("input#currentpath").val('');
            $("input#SupplierImage").val('');
        }
        else{
            var reader = new FileReader();
            reader.onload = function (e) {
                var filePath = $("#SupplierImage").val().split('\\').pop();

                if (filePath.toString().indexOf("&") >= 0 || filePath.toString().indexOf("<") >= 0 || filePath.toString().indexOf(">") >= 0
                    || filePath.toString().indexOf("*") >= 0 || filePath.toString().indexOf(":") >= 0
                    || filePath.toString().indexOf("?") >= 0) {
                    //alert(MsgValidFileName);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgValidFileName);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("input#currentpath").val('');
                }
                else {
                    $('#previewHolder').attr('src', e.target.result);
                    $("input#currentpath").val(e.target.result);
                    $("#btnDeleteImage").show();
                }
            }

            reader.readAsDataURL(input.files[0]);
        }
    }
}

var elm;
function isValidURL(u) {
    if (!elm) {
        elm = document.createElement('input');
        elm.setAttribute('type', 'url');
    }
    elm.value = u;
    return elm.validity.valid;
}


function CheckValidURLForImage(curobj) {
    var strURL = $(curobj).val();
    if (strURL != '' && strURL != null) {
        if (typeof (SupplierMsgInvalidURL) == "undefined") {
             SupplierMsgInvalidURL = 'Invalid URL. please enter valid URL.';
        }
        if (isValidURL(strURL)) {

            var validExtension = CommonFileExtension.split(',');
            var strValidationMessage = "";
            var fileExt = strURL.substring(strURL.lastIndexOf('.'));
            if (fileExt.indexOf("/") <= 0) {
                if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
                    strValidationMessage = strValidationMessage + strURL + " " + MsgInvalidFileSelected;
                }
                if (strValidationMessage != "") {
                    alert(strValidationMessage + MsgvalidFileList.replace("{0}", validExtension.toString()));
                    $('#previewHolder').attr('href', '/Content/images/no-image.jpg');
                    $('#previewHolder').attr('src', '/Content/images/no-image.jpg');
                    $("input#ImageExternalURL").val('');
                    return;
                }
            }

          
            
            $("<img>", {
                src: strURL,
                error: function () {
                    //alert(MsgInvalidURL);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(SupplierMsgInvalidURL);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $(curobj).val("");
                    curobj.focus();
                },
                load: function () {
                    $('#previewHolder').attr('src', strURL);
                }
            });
        } else {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(SupplierMsgInvalidURL);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $(curobj).val("");
            curobj.focus();
        }
     
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
                    $("#spanGlobalMessage").html(MsgEndDateValidation);
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
                        $("#spanGlobalMessage").html(MsgEndDateValidation);
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
                    $("#spanGlobalMessage").html(EndDateShouldGreaterThanStartDateValidation);
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
                        $("#spanGlobalMessage").html(MsgEndDateValidation);
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
                // ShowNewTab('/Master/SupplierCreate?isforbom=' + isForBom, 'frmSupplier');
                SwitchTextTab(0, 'SupplierCreate', 'frmSupplier');
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

            if ($("#SupplierImage").val() != "") {
                ajaxFileUpload(response.SupplierId);
            }
            else {
                SwitchTextTab(0, 'SupplierCreate', 'frmSupplier');
            }

           // SwitchTextTab(0, 'SupplierCreate', 'frmSupplier');
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
                SwitchTextTab(0, 'SupplierCreate', 'frmSupplier');
                //ShowNewTab('/Master/SupplierCreate?isforbom=' + isForBom, 'frmSupplier');
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
                SwitchTextTab(0, 'SupplierCreate', 'frmSupplier');
                //ShowNewTab('/Master/SupplierCreate?isforbom=' + isForBom, 'frmSupplier');
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
