$(document).ready(function () {
    setTimeout(function () {

        if ($("input#OnHandQuantity").val() != '') {
            $("input#OnHandQuantity").val(FormatedCostQtyValues(parseFloat($("input#OnHandQuantity").val()), 2));
        }

        if ($("input#SuggestedOrderQuantity").val() != '') {

            $("input#SuggestedOrderQuantity").val(FormatedCostQtyValues(parseFloat($("input#SuggestedOrderQuantity").val()), 2));
        }
        $("#divitemlocationbinreplanish").on({
            mousedown: function () {

                if (!$(this).hasClass("ui-autocomplete-input")) {
                    SetAutoCompleteOpenOnFocus($(this), Master_GetAllLocationOfRoom, null, " ");
                }
            }

        }, "input[type='text'][id='txtLocation']");

        $("#divitemlocationbinreplanish").on({ click: function () { $(this).siblings('#txtLocation').trigger("focus"); } }, ".ShowAllOptionsBin");

        $("div#divItemManufacturer").on({
            mousedown: function () {
                var txtman = $(this);
                $(txtman).autocomplete("search", "");
            },
            mouseenter: function () {
                var txtman = $(this);

                if (!$(txtman).hasClass('ui-autocomplete-input')) {
                    $(txtman).autocomplete({
                        source: function (request, response) {
                            var urlstring;
                            if (isBOMItem == 'True') {
                                urlstring = "/BOM/GetManufacturer";
                            }
                            else {
                                urlstring = "/Master/GetManufacturer";
                            }
                            $.ajax({
                                url: urlstring,
                                contentType: 'application/json',
                                dataType: 'json',
                                data: {
                                    maxRows: 1000,
                                    name_startsWith: request.term
                                },
                                success: function (data) {
                                    response($.map(data, function (item) {
                                        return {
                                            label: item.Manufacturer,
                                            value: item.Manufacturer,
                                            selval: item.ID
                                        }
                                    }));
                                },
                                error: function (err) {
                                    alert(err);
                                }
                            });
                        },
                        autoFocus: false,
                        select: function (event, ui) {
                            $(this).next().val(ui.item.selval);
                        },
                        minLength: 0,
                        open: function () {
                            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");

                            $(this).autocomplete('widget').css('z-index', 100);
                        },
                        close: function () {
                            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                        }
                    });
                }
            },
            focus: function (ev) {
                //var txtsup = $(this);
                //if (ev.hasOwnProperty('originalEvent')) {
                //    $(txtsup).autocomplete("search", "");
                //}
            }
        }, "#txtManufacturerName");

        $("div#divItemManufacturer").on("click", ".ShowAllOptionsMan", function () {
            $(this).siblings("#txtManufacturerName").autocomplete("search", "");
            //$(this).siblings('#txtSupplierName').trigger("focus");
        });

        $("div#divItemSupplier").on({
            mousedown: function () {
                var txtsup = $(this);
                $(txtsup).autocomplete("search", "");
            },
            mouseenter: function () {
                var txtSupplier = $(this);

                if (!$(txtSupplier).hasClass('ui-autocomplete-input')) {
                    $(txtSupplier).autocomplete({
                        source: function (request, response) {
                            var urlstring;
                            if (isBOMItem == 'True') {
                                urlstring = "/BOM/GetSupplier";
                            }
                            else {
                                urlstring = "/Master/GetSupplier";
                            }
                            $.ajax({
                                url: urlstring,
                                contentType: 'application/json',
                                dataType: 'json',
                                data: {
                                    maxRows: 1000,
                                    name_startsWith: request.term
                                },
                                success: function (data) {
                                    response($.map(data, function (item) {
                                        return {
                                            label: item.SupplierName,
                                            value: item.SupplierName,
                                            selval: item.ID
                                        }
                                    }));
                                },
                                error: function (err) {
                                    alert(err);
                                }
                            });
                        },
                        autoFocus: false,
                        select: function (event, ui) {
                            $(this).next().val(ui.item.selval);
                        },
                        minLength: 0,
                        open: function () {
                            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                            $(this).autocomplete('widget').css('z-index', 100);
                        },
                        close: function () {
                            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                        }
                    });
                }
            },
            focus: function (ev) {
                //var txtsup = $(this);
                //if (ev.hasOwnProperty('originalEvent')) {
                //    $(txtsup).autocomplete("search", "");
                //}
            }
        }, "#txtSupplierName");

        $("div#divItemSupplier").on("click", ".ShowAllOptionsSup", function () {
            $(this).siblings("#txtSupplierName").autocomplete("search", "");
            //$(this).siblings('#txtSupplierName').trigger("focus");
        });



        $('form').areYouSure();
        $.validator.unobtrusive.parse("#frmItemMaster");
        if (ItemImageType == 'ImagePath' || ItemImageType == '') {
            $("#ItemImage").show();
            $("#ExternalURL").hide();
        }
        else {
            $("#ItemImage").hide();
            $("#ExternalURL").show();
        }
        $("#ItemImage").change(function () {
            readURL(this);
        });
        $('#chkIsAutoInventoryClassification').click(AutoInventoryClassification);
        var Itemfrmvalidator = $("#frmItemMaster").validate();
        $(".userHead").on("blur", "input[data-val='true']", function () {
            Itemfrmvalidator.element("#" + $(this).attr("id"));
        });
        jQuery.validator.addMethod('criticlequantitycheck', function (value, element, params) {
            if (parseFloat(element.form.CriticalQuantity.value == '' ? 0 : element.form.CriticalQuantity.value, 10) > parseFloat(element.form.MinimumQuantity.value == '' ? 0 : element.form.MinimumQuantity.value, 10))
                return false;
            else
                return true;
        }, '');
        jQuery.validator.unobtrusive.adapters.add('criticlequantitycheck', {}, function (options) {
            if ($('#liCriticalQuantity').css("display") == 'none') {
                options.rules['criticlequantitycheck'] = true;
                options.messages['criticlequantitycheck'] = options.message;
            }
            else {
                options.rules['criticlequantitycheck'] = false;
            }
        });
        jQuery.validator.addMethod('minimumquantitycheck', function (value, element, params) {
            if (parseFloat(element.form.MinimumQuantity.value == '' ? 0 : element.form.MinimumQuantity.value, 10) > parseFloat(element.form.MaximumQuantity.value == '' ? 0 : element.form.MaximumQuantity.value, 10))
                return false;
            else
                return true;
        }, '');
        jQuery.validator.unobtrusive.adapters.add('minimumquantitycheck', {}, function (options) {
            //if ($('#IsItemLevelMinMaxQtyRequired').attr('checked') == 'checked') {
            if ($('#liMinimumQuantity').css("display") != 'none') {
                options.rules['minimumquantitycheck'] = true;
                options.messages['minimumquantitycheck'] = options.message;
            }
            else {
                options.rules['minimumquantitycheck'] = false;
            }
        });
        if ($('#hiddenDefaultLocationName').val() == "") {
            $('#hiddenDefaultLocationName').val("abc");
        }
        $('#btnCancel').click(function (e) {
            SwitchTextTab(0, 'ItemCreate', 'frmItemMaster');
            if (oTable !== undefined && oTable != null) {
                oTable.fnStandingRedraw();
            }
        });

        if (IsItemLevelMinMaxQtyRequired == 'True') {
            $('#liCriticalQuantity').show();
            $('#liMinimumQuantity').show();
            $('#liMaximumQuantity').show();
            $('#liDefaultLocation').hide();
            //BindBinReplanish(true);
        }
        else {
            if (ItemType != 4) {
                $('#liCriticalQuantity').hide();
                $('#liMinimumQuantity').hide();
                $('#liMaximumQuantity').hide();
                $('#liDefaultLocation').hide();
                //BindBinReplanish(false);
            }
        }
        $("#btnSave").click(function () {
            if ($("input[type='radio'][name='ItemTraking']:checked").length > 0) {
                $("input[type='hidden'][id='hdnItmTracking']").remove();
            }

        });
        var check;
        $("input[type='radio'][name='ItemTraking']").hover(function () {
            check = $(this).is(':checked');
        });
        $("input[type='radio'][name='ItemTraking']").click(function () {
            check = !check;
            $(this).prop("checked", check);
        });
        checkRememberUDFValues($("#hdnPageName").val(), ItemID);

        //BindItemManufacture();
        //BindItemSupplier();

        $("#divKitDetails").on("change", "input[type='text'][id='txtQuantityPerKit']", function () {
            var curobj = $(this);
            var itmguid = $(curobj).parent().parent().find("input[type='hidden'][id='hdnItemGUID']").val();
            //            var aPos = oTableItemKit.fnGetPosition($(curobj).parent()[0]);
            //            var aData = oTableItemKit.fnGetData(aPos[0]);

            $.ajax({
                type: "POST",
                url: Inventory_SaveKitQty,
                contentType: 'application/json',
                dataType: 'json',
                data: "{QuantityPerKit:'" + $(curobj).val() + "',ItemGUID:'" + itmguid + "'}",
                success: function (retdt) {

                },
                error: function (err) {
                    //
                    alert("There is some Error");
                }
            });
        });

        AddMarkupAndCostToSellPrice('cost');

        AddMarkupAndCostToSellPrice('sell');


        AutoInventoryClassification();
        //BindBinReplanish();
        //   alert($("#Markup"));

        $('.ShowAllOptionsCat').click(function () {
            $(this).siblings("#CategoryName").autocomplete("search", "");
            $(this).siblings('#CategoryName').trigger("focus");
        });

        if (ItemID > 0) {
            DisableSerialLotExpiry();

            $('#dlItemType').attr('disabled', true);
            $('#dlItemType').val(ItemType);

            if (ItemType == 4) {
                EnableDisableLabour(true);
            }

            if (ItemType == 3) {
                BindKitDetails();
            }

            if (ItemType == 4) {
                //  EnableDisableLabour(true);
                //
                $('#liCriticalQuantity').find('#CriticalQuantity').addClass('disableBack');
                $('#liMinimumQuantity').find('#MinimumQuantity').addClass('disableBack');
                $('#liMaximumQuantity').find('#MaximumQuantity').addClass('disableBack');
                $('#liDefaultPullQuantity').find('#DefaultPullQuantity').addClass('disableBack');
                $('#liDefaultReorderQuantity').find('#DefaultReorderQuantity').addClass('disableBack');
                $('#liCriticalQuantity').find('#CriticalQuantity')[0].value = '';
                $('#liMinimumQuantity').find('#MinimumQuantity')[0].value = '';
                $('#liMaximumQuantity').find('#MaximumQuantity')[0].value = '';
                $('#liDefaultLocation').hide();
                //  BindBinReplanish(true);
            }
            else {
                // EnableDisableLabour(false);
                $('#liCriticalQuantity').find('#CriticalQuantity').removeClass('disableBack');
                $('#liMinimumQuantity').find('#MinimumQuantity').removeClass('disableBack');
                $('#liMaximumQuantity').find('#MaximumQuantity').removeClass('disableBack');
                $('#liDefaultPullQuantity').find('#DefaultPullQuantity').removeClass('disableBack');
                $('#liDefaultReorderQuantity').find('#DefaultReorderQuantity').removeClass('disableBack');
                // $('#liCriticalQuantity').find('#CriticalQuantity')[0].value = '0.0';
                // $('#liMinimumQuantity').find('#MinimumQuantity')[0].value = '0.0';
                // $('#liMaximumQuantity').find('#MaximumQuantity')[0].value = '0.0';
                $('#liDefaultLocation').show();
                // BindBinReplanish(false);
            }

            if ($('#IsItemLevelMinMaxQtyRequired').attr('type') != 'hidden') {
                $('#IsItemLevelMinMaxQtyRequired').attr('disabled', true);
            }
        }
        else {
            $('#chkSerialNumberTracking').click(ToggleSerialLotExpiry);
            $('#chkLotNumberTracking').click(ToggleSerialLotExpiry);

        }

        ///// disable last cost or average cost and markup and sell price based on Valuing Inventory Method /////




        // NDCostChanges : next If else commented

        //        if (MethodOfValuingInventory == 3) {   // Average Cost
        //            $("#Cost").addClass('disableBack');
        //            $("#AverageCost").addClass('disableBack');
        //        }
        //        else if (MethodOfValuingInventory == 4) {  /// Last Cost
        //            $("#Cost").addClass('disableBack');
        //            $("#AverageCost").addClass('disableBack');
        //            $("#Markup").val("0.0");
        //            $("#Markup").addClass('disableBack');
        //            $("#SellPrice").val("0.0");
        //            $("#SellPrice").addClass('disableBack');
        //        }

        //////////////////////////////
        SetCosignmentFields($('#Consignment').prop("checked"));

        $('#Consignment').change(function () {
            SetCosignmentFields($('#Consignment').prop("checked"));
        });

        $('#dlItemType').change(function (e) {
            //  alert('hi');

            if ($(this).val() == '4') {

                EnableDisableLabour(true);
                //
                $('#liCriticalQuantity').find('#CriticalQuantity').addClass('disableBack');
                $('#liMinimumQuantity').find('#MinimumQuantity').addClass('disableBack');
                $('#liMaximumQuantity').find('#MaximumQuantity').addClass('disableBack');
                $('#liDefaultPullQuantity').find('#DefaultPullQuantity').addClass('disableBack');
                $('#liDefaultReorderQuantity').find('#DefaultReorderQuantity').addClass('disableBack');
                $('#liCriticalQuantity').find('#CriticalQuantity')[0].value = '';
                $('#liMinimumQuantity').find('#MinimumQuantity')[0].value = '';
                $('#liMaximumQuantity').find('#MaximumQuantity')[0].value = '';
            }
            else {


                EnableDisableLabour(false);
                $('#liCriticalQuantity').find('#CriticalQuantity').removeClass('disableBack');
                $('#liMinimumQuantity').find('#MinimumQuantity').removeClass('disableBack');
                $('#liMaximumQuantity').find('#MaximumQuantity').removeClass('disableBack');
                $('#liDefaultPullQuantity').find('#DefaultPullQuantity').removeClass('disableBack');
                $('#liDefaultReorderQuantity').find('#DefaultReorderQuantity').removeClass('disableBack');
                $('#liCriticalQuantity').find('#CriticalQuantity')[0].value = FormatedCostQtyValues(0, 2);
                $('#liMinimumQuantity').find('#MinimumQuantity')[0].value = FormatedCostQtyValues(0, 2);
                $('#liMaximumQuantity').find('#MaximumQuantity')[0].value = FormatedCostQtyValues(0, 2);

            }

            BindKitDetails();
            ToggleSerialLotExpiry();
            AutoInventoryClassification();

            if (IsItemLevelMinMaxQtyRequired == 'True') {
                DisableLocationTextbox('True');
            }
            else {
                if (ItemType != 4) {
                    DisableLocationTextbox('False');
                }
            }


            //    $("#divItemManufacturer").find("input,select").prop("disabled", true);
            //    $("#divItemSupplier").find("input,select").prop("disabled", true);
            //    $("#divItemSupplier").find("input,select").prop("disabled", true);

        });





        $('#IsItemLevelMinMaxQtyRequired').change(function (e) {

            var checkboxcheck;
            if ($(this).attr('checked') == 'checked') {
                $('#liCriticalQuantity').show();
                $('#liMinimumQuantity').show();
                $('#liMaximumQuantity').show();
                $('#liDefaultLocation').hide();
                $('#divBinReplanish').empty();
                //BindBinReplanish(true);
                checkboxcheck = true;
            }
            else {
                $('#liCriticalQuantity').hide();
                $('#liMinimumQuantity').hide();
                $('#liMaximumQuantity').hide();
                $('#liDefaultLocation').hide();
                //BindBinReplanish(false);
                checkboxcheck = false;
            }
        });



        if (ViewBag_LockReplenishmentType == 'True') {
            if ($('#dummy') == undefined) {
                $('#IsItemLevelMinMaxQtyRequired').attr('disabled', true);
            }
        }
        if (ViewBag_LockConsignment == 'True') {
            $('#Consignment').attr('disabled', true);
        }
        $(".text-boxPriceFormat").priceFormat({
            prefix: '',
            thousandsSeparator: '',
            centsLimit: parseInt($('#hdCostcentsLimit').val(), 10)
        });
        $(".text-boxQuantityFormat").priceFormat({
            prefix: '',
            thousandsSeparator: '',
            centsLimit: parseInt($('#hdQuantitycentsLimit').val(), 10)
        });
        $(".text-boxQuantityFormatSR").priceFormat({
            prefix: '',
            thousandsSeparator: '',
            centsLimit: 0
        });
        $(".text-boxPriceFixedFormat").priceFormat({
            prefix: '',
            thousandsSeparator: '',
            centsLimit: 1
        });

        //$(".text-boxWeightPerPieceFormat").priceFormat({
        //    prefix: '',
        //    thousandsSeparator: '',
        //    centsLimit: parseInt($('#hdWeightPerPieceLimit').val(), 10)
        //});

        $("#Cost").blur(function () { setDecimal(this); return AddMarkupAndCostToSellPrice('cost'); });
        $("input[type='text'][id='Markup']").blur(function () { setDecimal(this); return AddMarkupAndCostToSellPrice('markup'); });
        $("#SellPrice").blur(function () { setDecimal(this); return AddMarkupAndCostToSellPrice('sell'); });

        //        $("#Cost").val(function () { return FormatedCostQtyValues(parseFloat(this.value), 1); });
        //        $("#SellPrice").val(function () { return FormatedCostQtyValues(parseFloat(this.value), 1); });
        //        $("#ExtendedCost").val(function () { return FormatedCostQtyValues(parseFloat(this.value), 1); });
        //        $("#AverageCost").val(function () { return FormatedCostQtyValues(parseFloat(this.value), 1); });
        $("#ItemModelPS").dialog({
            autoOpen: false,
            modal: true,
            draggable: true,
            resizable: true,
            width: '82%',
            height: 710,
            title: MsgAddKitComponentToKit,
            open: function () {
                $('#DivLoading').show();
                $("#ItemModelPS").load($(this).data("url"));
            },
            close: function () {
                BindKitDetails();
            }
        });


        $("#SellPrice").on("focus", function () {
            $("#hdnOldSellPrise").val($(this).val());
        });

        $("#Cost").on("focus", function () {
            $("#hdnOldCost").val($(this).val());
        });

        $("#txtUnit").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Master/GetUnits',
                    contentType: 'application/json',
                    dataType: 'json',
                    data: {
                        maxRows: 1000,
                        name_startsWith: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.Unit,
                                value: item.Unit,
                                selval: item.ID
                            }
                        }));
                    },
                    error: function (err) {
                        //
                        alert(err);
                    }
                });
            },
            autoFocus: false,
            minLength: 0,
            select: function (event, ui) {
                $("#UOMID").val(ui.item.selval);
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");

                $(this).autocomplete('widget').css('z-index', 100);
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });

        $("#CategoryName").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Master/GetCategory',
                    contentType: 'application/json',
                    dataType: 'json',
                    data: {
                        maxRows: 1000,
                        name_startsWith: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.Category,
                                value: item.Category,
                                selval: item.ID
                            }
                        }));
                    }
                });
            },
            autoFocus: false,
            minLength: 0,
            select: function (event, ui) {
                $("#CategoryID").val(ui.item.selval);
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");

                $(this).autocomplete('widget').css('z-index', 100);
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });

        $("#NewMasterPopUP").dialog({
            autoOpen: false,
            show: "blind",
            hide: "explode",
            height: 600,
            width: 800,
            modal: true,
            open: function () {
                $(this).parent().find("span.ui-dialog-title").html("Add New " + $(this).data("data"));
            },
            close: function () {
                $('#DivLoading').show();
                RefreshDropdownList($(this).data("data"), $(this).data("IDVal"));
                $(this).data("data", null);
                $(this).data("IDVal", null);
                $(this).parent().find("span.ui-dialog-title").html('');
            }
        });

    }, 300);
    DoNGbootstrap();
});
// DOC.READY END

$("form").submit(function (e) {
    RemoveLeadingTrailingSpace("frmItemMaster");
    if ($(this).valid()) {
        rememberUDFValues($("#hdnPageName").val(), ItemID);
    }
    e.preventDefault();
});


function ShowImage(currentRadio) {
    var currentId = $(currentRadio).attr("id");

    if (currentId == "ImagePath") {
        $("#ItemImage").show();
        $("#ExternalURL").hide();
        setImagePath();

        // $("img#previewHolder").attr('src', '/Content/images/no-image.jpg');
        // $("#ImageExternalURL").val('');
    }
    else {
        CheckValidURLForImage($("input#ItemImageExternalURL"));
        //  $("#SupplierImage").val('');
        $("#ItemImage").hide();
        $("#ExternalURL").show();
        //  $("img#previewHolder").attr('src','/Content/images/no-image.jpg');
    }
}

function setImagePath() {
    $('#previewHolder').attr('src', $("input#currentpath").val());
}

function OpenFile(id, medianame, urlid) {
    var h = 280;
    var w = 450;
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    return window.open('/Inventory/MediaUploadPartial?id=' + id + '&ItemNumber=' + medianame + '&urlid=' + urlid, 'MediaUpload', 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h + '); //, top=' + top + ', left=' + left);
    return false;
}

function UpdateFileName(filename, filepath) {

    $("#ItemImageBox").parent().html("<img id='ItemImageBox' width='120px' height='120px' src=" + filepath + " />")
    $("#txtImagePath").val(filename);
}

function CheckBeforeSave() {
    //
    var itemt = 0;
    if (parseInt(ItemID) == 0) {
        itemt = $("#dlItemType").val();
    }
    else {
        itemt = $("#ItemType").val();
    }

    if (!($('#IsTransfer').attr("checked") || $('#IsPurchase').attr("checked"))) {
        //            alert('Transfer AND/OR Purchase is Required');
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").text('Transfer AND/OR Purchase is Required');
        $('div#target').fadeToggle();
        $("div#target").delay(2000).fadeOut(200);
        return false;
    }

    if (parseInt(itemt) != 4) {

        var Temprows1 = $("#ItemSupplier").dataTable().fnGetNodes();
        var iCountManuDefault = 0;
        var iCountSupPartNo = 0;
        if (Temprows1 != null && Temprows1.length > 0) {
            for (var i = 0; i < Temprows1.length; i++) {
                if (!(Temprows1[i].cells[0].getElementsByTagName('input')[0].value == '' && Temprows1[i].cells[1].getElementsByTagName('input')[0].value == '')) {
                    if (Temprows1[i].cells[3].getElementsByTagName('input')[0].checked) {
                        iCountManuDefault += 1;
                    }
                    if (Temprows1[i].cells[1].getElementsByTagName('input')[0].value == '') {
                        iCountSupPartNo += 1;

                    }
                }
            }
        }

        if (!(iCountManuDefault == 1)) {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").text(OnedefaultSupplierisRequired);
            $('div#target').fadeToggle();
            $("div#target").delay(2000).fadeOut(200);
            return false;
        }

        if (iCountSupPartNo > 0) {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").text('Supplier Number required.');
            $('div#target').fadeToggle();
            $("div#target").delay(2000).fadeOut(200);
            return false;
        }



        var TempLocationrows1 = $("#ItemLocationLevelQuanity").dataTable().fnGetNodes();
        var iCountLocationDefault = 0;
        var iCountLocationPartNo = 0;
        if (TempLocationrows1 != null && TempLocationrows1.length > 0) {
            for (var i = 0; i < TempLocationrows1.length; i++) {
                if (!(TempLocationrows1[i].cells[1].getElementsByTagName('input')[0].value.trim() == '')) {
                    if (TempLocationrows1[i].cells[0].getElementsByTagName('input')[0].checked) {
                        iCountLocationDefault += 1;
                    }
                }
            }
        }
        else {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").text('Atleaset one default location required.');
            $('div#target').fadeToggle();
            $("div#target").delay(2000).fadeOut(200);
            return false;
        }

        if (!(iCountLocationDefault == 1)) {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").text('Please select one default location.');
            $('div#target').fadeToggle();
            $("div#target").delay(2000).fadeOut(200);
            return false;
        }

        // kit
        if (parseInt(itemt) == 3) {
            // //  atleast one item required.
            var Temprowskit = $("#ItemKitComponent").dataTable().fnGetNodes();
            var iCountKitDetails = 0;
            if (Temprowskit != null && Temprowskit.length > 0) {
                for (var i = 0; i < Temprowskit.length; i++) {
                    iCountKitDetails = iCountKitDetails + 1;
                }
            }
            if (!(iCountKitDetails > 0)) {
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").text('Atleast one kit component required.');
                $('div#target').fadeToggle();
                $("div#target").delay(2000).fadeOut(200);
                return false;
            }
        }

        if (!SavetoSeesionItemSupplierAllSave()) {
            return false;
        }

        if (!SavetoSeesionItemManufactureAllSave()) {
            return false;
        }

        if (!SavetoSeesionItemLocationAll()) {
            return false;
        }

        if (!SavetoSeesionItemKitComponentAll()) {
            return false;
        }
    }
    else if (parseInt(itemt) == 4) {

        if (!SavetoSeesionItemManufactureAllSave()) {
            return false;
        }
    }
    return true;
}

function onBegin() {
    //
    $("#frmItemMaster").find("input[type='hidden']").removeProp("disabled")
    return CheckBeforeSave();
}

function onSuccess(response) {

    //        console.log($('#frmItemMaster').validate().errorList);
    //   $("input").addAttr("disabled");
    $('div#target').fadeToggle();
    $("div#target").delay(2000).fadeOut(200);
    $("#spanGlobalMessage").html(response.Message);
    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    var idValue = $("#dvhdns").find("input[type='hidden'][id='hiddenID']").val();
    if (response.Status == "fail") {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#ItemNumber").val("");
        $("#ItemNumber").focus();
        IsRefreshGrid = true;
    }
    else if (response.Status == "Fa") {
        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
        $("#ItemNumber").focus();
        IsRefreshGrid = true;
    }
    else if (idValue == 0) {
        IsRefreshGrid = true;
        $("#ItemNumber").val("");
        $("#ItemNumber").focus();
        //clearControls('frmItemMaster');
        if (response.Status == "duplicate")
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
        else {
            if ($("#Link2").val() != "") {
                ajaxFileUpload(response.ItemID);
            }
            if ($("#ItemImage").val() != "") {
                ajaxFileUpload1(response.ItemID);
            }

            clearControls('frmItemMaster');
            setDefaultUDFValues($("#hdnPageName").val(), ItemID);

            var _IsArchived12 = false;
            var _IsDeleted12 = false;
            if (typeof ($('#IsArchivedRecords')) != undefined)
                _IsArchived12 = $('#IsArchivedRecords').is(':checked');
            if (typeof ($('#IsDeletedRecords')) != undefined)
                _IsDeleted12 = $('#IsDeletedRecords').is(':checked');
            GetPullNarrowSearchData('ItemMaster', _IsArchived12, _IsDeleted12);
            CallNarrowfunctions();

            $('#chkLotNumberTracking').attr("disabled", false);
            $('#chkSerialNumberTracking').attr("disabled", false);
            $('#chkDateCodeTracking').attr("disabled", false);
            if (response.DestinationModule != null && response.DestinationModule != "") {
                if (response.DestinationModule == "CartItemMaster") {
                    $("#divSupplierCatalogItems").dialog("close");
                    SwitchTextTab(0, '', '');
                    if (oTable !== undefined && oTable != null) {
                        oTable.fnDraw();
                    }
                    //                        if ($("#Cart_ItemModel_filter") != undefined) {
                    //                            $("#Cart_ItemModel_filter").val(response.ItemDTO.UPC);
                    //                            fnFilterGlobalCIM();
                    //                        }

                }
                if (response.DestinationModule == "OrderMaster") {
                    $("#divSupplierCatalogItems").dialog("close");
                    CallThisFunctionFromModel('success');
                }
            }
            ShowNewTab('ItemCreate', 'frmItemMaster');
        }
    }
    else if (idValue > 0) {
        if (response.Status == "duplicate") {
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
            $("#ItemNumber").val("");
            $("#ItemNumber").focus();
        }
        else {
            if (oTable !== undefined && oTable != null) {
                oTable.fnStandingRedraw();
            }
            // IsRefreshGrid = true;
            var _IsArchived12 = false;
            var _IsDeleted12 = false;
            if (typeof ($('#IsArchivedRecords')) != undefined)
                _IsArchived12 = $('#IsArchivedRecords').is(':checked');
            if (typeof ($('#IsDeletedRecords')) != undefined)
                _IsDeleted12 = $('#IsDeletedRecords').is(':checked');
            GetPullNarrowSearchData('ItemMaster', _IsArchived12, _IsDeleted12);
            CallNarrowfunctions();
            if ($("#Link2").val() != "") {
                ajaxFileUpload(response.ItemID);
            }
            if ($("#ItemImage").val() != "") {
                ajaxFileUpload1(response.ItemID);
            }
            var QueryStringParam2 = ItemGUID;
            // alert(QueryStringParam2);
            ShowEditTabGUID("ItemEdit?ItemGUID=" + QueryStringParam2, "frmItem");

            //clearControls('frmItemMaster');
            //SwitchTextTab(0, 'ItemCreate', 'frmItemMaster');

        }
    }
}

function onFailure(message) {
    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    $("#spanGlobalMessage").html(message.statusText);
    $('div#target').fadeToggle();
    $("div#target").delay(2000).fadeOut(200);
    $("#ItemNumber").focus();
}

function ajaxFileUpload1(retid) {
    //starting setting some animation when the ajax starts and completes
    $("#loading")
.ajaxStart(function () {
    $(this).show();
})
.ajaxComplete(function () {
    $(this).hide();
});

    $.ajaxFileUpload
(
    {
        url: '/api/fileupload/PostItemFile/' + retid,
        secureuri: false,
        type: "POST",
        fileElementId: 'ItemImage',
        dataType: 'json',
        success: function (data, status) {
            //  window.location.reload();
        },
        error: function (data, status, e) {
            //    window.location.reload();
        }
    }
)
    return false;
}

function ajaxFileUpload(retid) {
    //starting setting some animation when the ajax starts and completes
    $("#loading")
.ajaxStart(function () {
    $(this).show();
})
.ajaxComplete(function () {
    $(this).hide();
});

    $.ajaxFileUpload
(
    {
        url: '/api/fileupload/ItemLink2/' + retid,
        secureuri: false,
        type: "POST",
        fileElementId: 'Link2',
        dataType: 'json',
        success: function (data, status) {
            //  window.location.reload();
        },
        error: function (data, status, e) {
            // window.location.reload();
        }
    }
)
    return false;
}

function ToggleSerialLotExpiry() {
    var isSerialNumberTracking = $('#chkSerialNumberTracking').attr('checked') ? true : false;
    var isLotNumberTracking = $('#chkLotNumberTracking').attr('checked') ? true : false;
    var isDateCodeTracking = $('#chkDateCodeTracking').attr('checked') ? true : false;

    if (isSerialNumberTracking)
        $('#chkLotNumberTracking').attr("disabled", true);
    else
        $('#chkLotNumberTracking').attr("disabled", false);

    if (isLotNumberTracking)
        $('#chkSerialNumberTracking').attr("disabled", true);
    else
        $('#chkSerialNumberTracking').attr("disabled", false);
}

function AutoInventoryClassification() {
    var IsAutoInventoryClassification = $('#chkIsAutoInventoryClassification').attr('checked') ? true : false;
    if (IsAutoInventoryClassification)
        $('#drpInventoryClassification').attr("disabled", true);
    else
        $('#drpInventoryClassification').attr("disabled", false);
}

function DisableSerialLotExpiry() {
    $('#chkLotNumberTracking').attr("disabled", true);
    $('#chkSerialNumberTracking').attr("disabled", true);
    $('#chkDateCodeTracking').attr("disabled", true);
}

function fnGetSelected(oTableLocal) {
    var aReturn = new Array();
    var aTrs = oTableLocal.fnGetNodes();
    for (var i = 0; i < aTrs.length; i++) {
        if ($(aTrs[i]).hasClass('row_selected')) {
            aReturn.push(aTrs[i]);
        }
    }
    return aReturn;
}

function onlyNumeric(event) {
    var charCode = (event.which) ? event.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function SetNames() {
    //$("#CategoryName").val($("#CategoryID option:selected").text());
    //$("#Unit").val($("#UOMID option:selected").text());
    $("#GLAccount").val($("#GLAccountID option:selected").text());
}

function SetNamesUOM() {
    $("#Unit").val($("#UOMID option:selected").text());
    // alert($("#UOMID").val());
}

function readURL(input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var filePath = $("#currentpath").val().split('\\').pop();

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

function DisableLocationTextboxForBOM(obj) {
    //

    if (ItemID != 0 && IsBOMItem != null && IsBOMItem == false && RefBomId != null && RefBomId > 0) {

        $('#ItemLocationLevelQuanity tbody tr').each(function () {

            if (obj == true || obj == 'True') {
                //$(this).find('#txtCriticalQuantity').val();
                $(this).find('#txtCriticalQuantity').addClass("disableBack");
                $(this).find('#txtMinimumQuantity').addClass("disableBack");
                $(this).find('#txtMaximumQuantity').addClass("disableBack");
                $(this).find('#txteVMISensorPort').addClass("disableBack");
                $(this).find('#txteVMISensorID').addClass("disableBack");
                $(this).find('#IsDefault').addClass("disableBack");


                $(this).find('#txtCriticalQuantity').attr("disabled", "disabled");
                $(this).find('#txtMinimumQuantity').attr("disabled", "disabled");
                $(this).find('#txtMaximumQuantity').attr("disabled", "disabled");
                $(this).find('#txteVMISensorPort').attr("disabled", "disabled");
                $(this).find('#txteVMISensorID').attr("disabled", "disabled");
                $(this).find('#IsDefault').attr("disabled", "disabled");

            }

        });


        $('#ItemManufacturer tbody tr').each(function () {

            if (obj == true || obj == 'True') {
                //$(this).find('#txtCriticalQuantity').val();
                $(this).find('#txtManufacturer').addClass("disableBack");
                $(this).find('#txtManufacturerNumber').addClass("disableBack");
                $(this).find('#IsDefault').addClass("disableBack");

                $(this).find('#txtManufacturer').attr('readonly', true);
                $(this).find('#txtManufacturerNumber').attr('readonly', true);
                $(this).find('#IsDefault').attr('readonly', true);

                $(this).find('#txtManufacturer').attr("disabled", "disabled");
                $(this).find('#txtManufacturerNumber').attr("disabled", "disabled");
                $(this).find('#IsDefault').attr("disabled", "disabled");

            }

        });


        $('#ItemSupplier tbody tr').each(function () {

            if (obj == true || obj == 'True') {
                //$(this).find('#txtCriticalQuantity').val();
                $(this).find('#txtSupplier').addClass("disableBack");
                $(this).find('#txtSupplierNumber').addClass("disableBack");
                $(this).find('#IsDefault').addClass("disableBack");

                $(this).find('#txtSupplier').attr('readonly', true);
                $(this).find('#txtSupplierNumber').attr('readonly', true);
                $(this).find('#IsDefault').attr('readonly', true);

                $(this).find('#txtSupplier').attr("disabled", "disabled");
                $(this).find('#txtSupplierNumber').attr("disabled", "disabled");
                $(this).find('#IsDefault').attr("disabled", "disabled");

            }

        });
    }
}

function EnableDisableLabour(IsDisable) {
    $('#ulQuantity').find('*').attr('disabled', IsDisable);
    $('#ulTracking').find('*').attr('disabled', IsDisable);

    if (ViewBag_LockReplenishmentType == 'True') {
        $('#IsItemLevelMinMaxQtyRequired').attr('disabled', true);
    }
    if (ViewBag_LockReplenishmentType == 'True') {
        if ($('#dummy') == undefined) {
            $('#IsItemLevelMinMaxQtyRequired').attr('disabled', true);
        }
        else {
            $('#dummy').attr('disabled', true);
        }
    }
    if (IsDisable) {
        $('#ulSupplierDetails').hide();
        $('#liLocationListInline').hide();
    }
    else {
        $('#ulSupplierDetails').show();
        $('#liLocationListInline').show();
    }
}

function BindKitDetails() {
    if ($('#dlItemType').val() == 3) {
        $('#ulkitdetails').show();
        $('#divKitDetails').empty();
        $('#DivLoading').show();
        $.get(Inventory_LoadKitComponentofItem + '?ItemGUID=' + ItemGUID + '&AddCount=0', function (data) {
            $('#divKitDetails').html(data);
            $('#DivLoading').hide();
        });
    }
    else {
        $('#ulkitdetails').hide();
    }
}

function AddNewKitComponent() {
    var strUrl = Inventory_LoadItemKitModel;
    strUrl = strUrl + '?Parentid=1';
    strUrl = strUrl + '&ParentGuid=' + ItemGUID.ToString();
    $('#ItemModelPS').data("url", strUrl).dialog('open');
    return false;
}

function SavetoSeesionItemKitComponentAll(obj) {
    //
    if ($("#ItemKitComponent").length > 0) {
        var TempSuprows4 = $("#ItemKitComponent").dataTable().fnGetNodes();
        if (TempSuprows4 != null && TempSuprows4.length > 0) {
            for (var i = 0; i < TempSuprows4.length; i++) {
                if (!SavetoSeesionItemKitComponent(TempSuprows4[i])) {
                    //                    return false;
                }
            }
        }
    }
    return true;
}

function SavetoSeesionItemKitComponent(obj) {
    //
    var vtxtQuantityPerKit = $(obj).parent().parent().find('#txtQuantityPerKit').val();
    var vitemQuantityPerKit = $(obj).parent().parent().find('#item_QuantityPerKit').val();
    //  alert(vitemQuantityPerKit);
    var vItemguid = $(obj).parent().parent().find('#hdnItemGUID').val();
    var vKitGUID = $(obj).parent().parent().find('#hdnKitGUID').val();
    var vhdnID = $(obj).parent().parent().find('#hdnID').val();
    var vhdnGUID = $(obj).parent().parent().find('#hdnGUID').val();
    var vhdnSessionSr = $(obj).parent().parent().find('#hdnSessionSr').val();

    //var TempLrows = $("#ItemKitComponent").dataTable().fnGetNodes();
    $.ajax({
        url: Inventory_SavetoSeesionItemKitComponent,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'KitGUID': vKitGUID, 'QuantityPerKit': vtxtQuantityPerKit },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'ok') {
                BindKitDetails();
                //  $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                //$("#spanGlobalMessage").text('Kit Component Added.');
                // $('div#target').fadeToggle();
                //  $("div#target").delay(2000).fadeOut(200);
            }
            else {
                alert("Error");
            }
        }
    });
    return false;
}

function DeletetoSeesionItemKitComponentSingle(obj) {
    var vtxtQuantityPerKit = $(obj).parent().parent().find('#txtQuantityPerKit').val();
    var vItemguid = $(obj).parent().parent().find('#hdnItemGUID').val();
    var vKitGUID = $(obj).parent().parent().find('#hdnKitGUID').val();
    var vhdnID = $(obj).parent().parent().find('#hdnID').val();
    var vhdnGUID = $(obj).parent().parent().find('#hdnGUID').val();
    var vhdnSessionSr = $(obj).parent().parent().find('#hdnSessionSr').val();

    //        if (vhdnGUID == '00000000-0000-0000-0000-000000000000') {
    //            //client side remove
    //            $(obj).parent().parent().remove();
    //            BindKitDetails();
    //        }
    //        else {

    $.ajax({
        url: Inventory_DeletetoSeesionItemKitComponentSingle,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'KitGUID': vKitGUID, 'QuantityPerKit': vtxtQuantityPerKit },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'reference') {
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").text('Kit Component cannot be deleted, quantity available in it.');
                $('div#target').fadeToggle();
                $("div#target").delay(2000).fadeOut(200);
            }
            else if (response.status == 'deleted') {
                BindKitDetails();
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").text('Record Deleted Successfully.');
                $('div#target').fadeToggle();
                $("div#target").delay(2000).fadeOut(200);

            }
        }
    });
    //}
    return false;
}

function BindBinReplanish(obj) {
    $('#divBinReplanish').empty();
    $('#DivLoading').show();
    $.get(Inventory_LoadLocationsofItem + '?ItemGUID=' + ItemGUID + '&AddCount=0', function (data) {
        $('#divBinReplanish').html(data);
        DisableLocationTextbox(obj);
        // alert('hi');
        // DisableLocationTextboxForBOM('True');
        $('#DivLoading').hide();
    });
}

function AddNewBinReplanish() {

    RemoveLeadingTrailingSpace("divitemlocationbinreplanish");
    var checkboxchk = false;
    if ($('#IsItemLevelMinMaxQtyRequired').attr('checked') == 'checked') {
        checkboxchk = true;
    }
    else if ($('#IsItemLevelMinMaxQtyRequired').val() == "True") {
        checkboxchk = true;
    }
    //

    if (SavetoSeesionItemLocationAll()) {
        $('#divBinReplanish').empty();
        $('#DivLoading').show();
        $.get(Inventory_LoadLocationsofItem + '?ItemGUID=' + ItemGUID + '&AddCount=1', function (data) {
            $('#divBinReplanish').html(data);
            DisableLocationTextbox(checkboxchk);
            $('#DivLoading').hide();
        });
    }
    return false;
}

function DisableLocationTextbox(obj) {
    // alert(obj);
    $('#ItemLocationLevelQuanity tbody tr').each(function () {
        // alert($(this).find('#txtCriticalQuantity').val());
        if (obj == true || obj == 'True') {
            //$(this).find('#txtCriticalQuantity').val();
            $(this).find('#txtCriticalQuantity').addClass("disableBack");
            $(this).find('#txtMinimumQuantity').addClass("disableBack");
            $(this).find('#txtMaximumQuantity').addClass("disableBack");

            $(this).find('#txtCriticalQuantity').val('N/A');
            $(this).find('#txtMinimumQuantity').val('N/A');
            $(this).find('#txtMaximumQuantity').val('N/A');

            $(this).find('#txtCriticalQuantity').attr('readonly', true);
            $(this).find('#txtMinimumQuantity').attr('readonly', true);
            $(this).find('#txtMaximumQuantity').attr('readonly', true);

            $(this).find('#txtCriticalQuantity').attr("disabled", "disabled");
            $(this).find('#txtMinimumQuantity').attr("disabled", "disabled");
            $(this).find('#txtMaximumQuantity').attr("disabled", "disabled");

        }
        else {

            $(this).find('#txtCriticalQuantity').removeClass("disableBack");
            $(this).find('#txtMinimumQuantity').removeClass("disableBack");
            $(this).find('#txtMaximumQuantity').removeClass("disableBack");

            //  alert($(this).find('#txtCriticalQuantity').val());
            if ($(this).find('#txtCriticalQuantity').val() <= 0)
                $(this).find('#txtCriticalQuantity').val(FormatedCostQtyValues(0, 2));
            if ($(this).find('#txtMinimumQuantity').val() <= 0)
                $(this).find('#txtMinimumQuantity').val(FormatedCostQtyValues(0, 2));
            if ($(this).find('#txtMaximumQuantity').val() <= 0)
                $(this).find('#txtMaximumQuantity').val(FormatedCostQtyValues(0, 2));

            $(this).find('#txtCriticalQuantity').attr('readonly', false);
            $(this).find('#txtMinimumQuantity').attr('readonly', false);
            $(this).find('#txtMaximumQuantity').attr('readonly', false);

            $(this).find('#txtCriticalQuantity').removeAttr("disabled");
            $(this).find('#txtMinimumQuantity').removeAttr("disabled");
            $(this).find('#txtMaximumQuantity').removeAttr("disabled");
        }
    });


    return true;
}

function SavetoSeesionBinReplanish(obj1) {
    //
    // var vBinID = $(obj).parent().parent().find('#dlLocation')[0].value == '' ? 0 : $(obj).parent().parent().find('#dlLocation')[0].value;
    var vBinID = $(obj1).find('#SubBinID').val();
    var vtxtBinLocation = $(obj1).find('#txtLocation').val();
    var vCritical = $(obj1).find('#txtCriticalQuantity').val() == 'N/A' ? 0.0 : $(obj1).find('#txtCriticalQuantity').val();
    var vMinimum = $(obj1).find('#txtMinimumQuantity').val() == 'N/A' ? 0.0 : $(obj1).find('#txtMinimumQuantity').val();
    var vMaximum = $(obj1).find('#txtMaximumQuantity').val() == 'N/A' ? 0.0 : $(obj1).find('#txtMaximumQuantity').val();
    var vItemguid = $(obj1).find('#hdnItemGUID').val();
    if (vItemguid == "") {
        vItemguid = ItemGUID;
    }
    var vhdnID = $(obj1).find('#hdnID').val();
    var vhdnGUID = $(obj1).find('#hdnGUID').val();
    var vhdnSessionSr = $(obj1).find('#hdnSessionSr').val();
    var vlocIsDefault = $(obj1).find('#IsDefault').is(':checked');
    var veVMISensorPort = $(obj1).find('#txteVMISensorPort').val();
    var veVMISensorID = $(obj1).find('#txteVMISensorID').val() == '' ? 0.0 : $(obj1).find('#txteVMISensorID').val();


    if (vBinID == 0 && vtxtBinLocation == '') {

        return true;
    }

    if (vtxtBinLocation == '') {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").text('Inventory Location is required.');
        $('div#target').fadeToggle();
        $("div#target").delay(2000).fadeOut(200);
        $(obj1).find('#txtLocation').focus();
        return false;
    }

    var TempSuprows = $("#ItemLocationLevelQuanity").dataTable().fnGetNodes();
    var iCountSup = 0;
    if (TempSuprows != null && TempSuprows.length > 0) {
        for (var i = 0; i < TempSuprows.length; i++) {
            if (TempSuprows[i].cells[1].getElementsByTagName('input').txtLocation.value == vtxtBinLocation) {
                iCountSup += 1;
                if (iCountSup > 1) {
                    $(TempSuprows[i]).css("background-color", "red");
                    $(TempSuprows[i].cells[1].getElementsByTagName('input')).css("background-color", "yellow");
                    $(TempSuprows[i].cells[1].getElementsByTagName('input').txtLocation).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").text('This Inventory Location is already added.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    return false;
                }
            }
        }
    }

    if (vtxtBinLocation == '') {
        alert("Please select Inventory location.");
        $(obj1).find('#txtLocation').focus();
        return false;
    }
    if ($(obj1).find('#txtCriticalQuantity').val() != 'N/A') {

        if (vCritical == '') {
            alert("Please select Critical Quantity.");
            $(obj1).find('#txtCriticalQuantity').focus();
            return false;
        }

        if (vMinimum == '') {
            alert("Please select Minimum Quantity.");
            $(obj1).find('#txtMinimumQuantity').focus();
            return false;
        }

        if (vMaximum == '') {
            alert("Please select Maximum Quantity.");
            $(obj1).find('#txtMaximumQuantity').focus();
            return false;
        }

        if (parseFloat(vCritical) > parseFloat(vMinimum)) {
            alert('Critical quantity must be less then Minimum quantity');
            $(obj1).find('#txtCriticalQuantity').focus();
            return false;
        }

        if (parseFloat(vMinimum) > parseFloat(vMaximum)) {
            alert('Minimum quantity must be less then Maximum quantity');
            $(obj1).find('#txtMaximumQuantity').focus();
            return false;
        }
    }

    //

    $.ajax({
        url: Inventory_SavetoSeesionBinReplanishSingle,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'BinID': vBinID, 'BinLocation': vtxtBinLocation, 'CriticalQuanity': vCritical, 'MinimumQuantity': vMinimum, 'MaximumQuantity': vMaximum, 'IsDefault': vlocIsDefault, 'eVMISensorPort': veVMISensorPort, 'eVMISensorID': veVMISensorID },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            //  alert(response);
            if (response.status == 'ok') {
                // BindBinReplanish();
                // $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                // $("#spanGlobalMessage").text('Inventory Location Added.');
                // $('div#target').fadeToggle();
                //  $("div#target").delay(2000).fadeOut(200);
            }
            else {
                alert("Error");
            }
        }
    });
    return true;
}

function SavetoSeesionBinReplanishNew(obj1, guid) {
    //
    // var vBinID = $(obj).parent().parent().find('#dlLocation')[0].value == '' ? 0 : $(obj).parent().parent().find('#dlLocation')[0].value;
    var vBinID = $(obj1).find('#SubBinID').val();
    var vtxtBinLocation = $(obj1).find('#txtLocation').val();
    var vCritical = $(obj1).find('#txtCriticalQuantity').val() == 'N/A' ? 0.0 : $(obj1).find('#txtCriticalQuantity').val();
    var vMinimum = $(obj1).find('#txtMinimumQuantity').val() == 'N/A' ? 0.0 : $(obj1).find('#txtMinimumQuantity').val();
    var vMaximum = $(obj1).find('#txtMaximumQuantity').val() == 'N/A' ? 0.0 : $(obj1).find('#txtMaximumQuantity').val();
    var vItemguid = $(obj1).find('#hdnItemGUID').val();
    if (vItemguid == "") {
        vItemguid = ItemGUID;
    }
    var vhdnID = $(obj1).find('#hdnID').val();
    var vhdnGUID = $(obj1).find('#hdnGUID').val();
    var vhdnSessionSr = $(obj1).find('#hdnSessionSr').val();
    var vlocIsDefault = $(obj1).find('#IsDefault').is(':checked');
    var veVMISensorPort = $(obj1).find('#txteVMISensorPort').val();
    var veVMISensorID = $(obj1).find('#txteVMISensorID').val() == '' ? 0.0 : $(obj1).find('#txteVMISensorID').val();


    if (vBinID == 0 && vtxtBinLocation == '') {

        return true;
    }

    if (vtxtBinLocation == '') {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").text('Inventory Location is required.');
        $('div#target').fadeToggle();
        $("div#target").delay(2000).fadeOut(200);
        $(obj1).find('#txtLocation').focus();
        return false;
    }

    var TempSuprows = $("#ItemLocationLevelQuanity").dataTable().fnGetNodes();
    var iCountSup = 0;
    if (TempSuprows != null && TempSuprows.length > 0) {
        for (var i = 0; i < TempSuprows.length; i++) {
            if (TempSuprows[i].cells[1].getElementsByTagName('input').txtLocation.value == vtxtBinLocation) {
                iCountSup += 1;
                if (iCountSup > 1) {
                    $(TempSuprows[i]).css("background-color", "red");
                    $(TempSuprows[i].cells[1].getElementsByTagName('input')).css("background-color", "yellow");
                    $(TempSuprows[i].cells[1].getElementsByTagName('input').txtLocation).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").text('This Inventory Location is already added.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    return false;
                }
            }
        }
    }

    if (vtxtBinLocation == '') {
        alert("Please select Inventory location.");
        $(obj1).find('#txtLocation').focus();
        return false;
    }
    if ($(obj1).find('#txtCriticalQuantity').val() != 'N/A') {

        if (vCritical == '') {
            alert("Please select Critical Quantity.");
            $(obj1).find('#txtCriticalQuantity').focus();
            return false;
        }

        if (vMinimum == '') {
            alert("Please select Minimum Quantity.");
            $(obj1).find('#txtMinimumQuantity').focus();
            return false;
        }

        if (vMaximum == '') {
            alert("Please select Maximum Quantity.");
            $(obj1).find('#txtMaximumQuantity').focus();
            return false;
        }

        if (parseFloat(vCritical) > parseFloat(vMinimum)) {
            alert('Critical quantity must be less then Minimum quantity');
            $(obj1).find('#txtCriticalQuantity').focus();
            return false;
        }

        if (parseFloat(vMinimum) > parseFloat(vMaximum)) {
            alert('Minimum quantity must be less then Maximum quantity');
            $(obj1).find('#txtMaximumQuantity').focus();
            return false;
        }
    }

    //

    $.ajax({
        url: Inventory_SavetoSeesionBinReplanishSingleNew,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'BinID': vBinID, 'BinLocation': vtxtBinLocation, 'CriticalQuanity': vCritical, 'MinimumQuantity': vMinimum, 'MaximumQuantity': vMaximum, 'IsDefault': vlocIsDefault, 'eVMISensorPort': veVMISensorPort, 'eVMISensorID': veVMISensorID },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            //  alert(response);
            if (response.status == 'ok') {

                if (guid == vhdnGUID) {
                    newGuid = response.newGUID;
                }
                // BindBinReplanish();
                // $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                // $("#spanGlobalMessage").text('Inventory Location Added.');
                // $('div#target').fadeToggle();
                //  $("div#target").delay(2000).fadeOut(200);
            }
            else {
                alert("Error");
            }
        }
    });
    return true;
}

function SavetoSeesionItemLocationAll() {
    //
    if ($("#ItemLocationLevelQuanity").length > 0) {
        var TempSuprows3 = $("#ItemLocationLevelQuanity").dataTable().fnGetNodes();
        var iCountSup = 0;
        var iCountSupDefault = 0;

        if (TempSuprows3 != null && TempSuprows3.length > 0) {
            //
            // alert(TempSuprows3.length);
            for (var i = 0; i < TempSuprows3.length; i++) {

                if (!(TempSuprows3[i].cells[1].getElementsByTagName('input').txtLocation.value == '' && TempSuprows3[i].cells[2].getElementsByTagName('input').txtCriticalQuantity.value == '' && TempSuprows3[i].cells[3].getElementsByTagName('input').txtMinimumQuantity.value == '' && TempSuprows3[i].cells[4].getElementsByTagName('input').txtMaximumQuantity.value == '')) {

                    if (TempSuprows3[i].cells[1].getElementsByTagName('input').txtLocation.value == '') {
                        $(TempSuprows3[i]).css("background-color", "red");
                        $(TempSuprows3[i].cells[1].getElementsByTagName('input')[0]).css("background-color", "yellow");
                        $(TempSuprows3[i].cells[1].getElementsByTagName('input')[0]).focus();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").text('Inventory Location is required.');
                        $('div#target').fadeToggle();
                        $("div#target").delay(2000).fadeOut(200);
                        return false;
                    }
                    //  alert(TempSuprows3[i]);
                    //
                    if (SavetoSeesionBinReplanish(TempSuprows3[i]) == false) {
                        // alert('herr');
                        // alert('Hi');
                        return false;
                    }
                }
            }
        }
    }
    return true;
}

function SavetoSeesionItemLocationAllNew(Guid) {
    //
    if ($("#ItemLocationLevelQuanity").length > 0) {
        var TempSuprows3 = $("#ItemLocationLevelQuanity").dataTable().fnGetNodes();
        var iCountSup = 0;
        var iCountSupDefault = 0;

        if (TempSuprows3 != null && TempSuprows3.length > 0) {
            //

            for (var i = 0; i < TempSuprows3.length; i++) {

                if (!(TempSuprows3[i].cells[1].getElementsByTagName('input').txtLocation.value == '' && TempSuprows3[i].cells[2].getElementsByTagName('input').txtCriticalQuantity.value == '' && TempSuprows3[i].cells[3].getElementsByTagName('input').txtMinimumQuantity.value == '' && TempSuprows3[i].cells[4].getElementsByTagName('input').txtMaximumQuantity.value == '')) {

                    if (TempSuprows3[i].cells[1].getElementsByTagName('input').txtLocation.value == '') {
                        $(TempSuprows3[i]).css("background-color", "red");
                        $(TempSuprows3[i].cells[1].getElementsByTagName('input')[0]).css("background-color", "yellow");
                        $(TempSuprows3[i].cells[1].getElementsByTagName('input')[0]).focus();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").text('Inventory Location is required.');
                        $('div#target').fadeToggle();
                        $("div#target").delay(2000).fadeOut(200);
                        return false;
                    }

                    //
                    if (SavetoSeesionBinReplanishNew(TempSuprows3[i], Guid) == false) {
                        // alert('herr');
                        // alert('Hi');
                        return false;
                    }
                }
            }
        }
    }
    return true;
}

function DeletetoSeesionBinReplanishSingle(obj) {
    var vardelete = false;

    if ($('#IsItemLevelMinMaxQtyRequired').attr('checked') == 'checked') {
        vardelete = true;
    }
    else if ($('#IsItemLevelMinMaxQtyRequired').val() == "True") {
        vardelete = true;
    }


    var vBinID = $(obj).parent().parent().find('#hdnBinID').val();
    // var vBinID = $(obj).parent().parent().find('#dlLocation')[0].value == '' ? 0 : $(obj).parent().parent().find('#dlLocation')[0].value;
    var vCritical = $(obj).parent().parent().find('#txtCriticalQuantity').val() == 'N/A' ? 0.0 : $(obj).find('#txtCriticalQuantity').val();
    var vMinimum = $(obj).parent().parent().find('#txtMinimumQuantity').val() == 'N/A' ? 0.0 : $(obj).find('#txtMinimumQuantity').val();
    var vMaximum = $(obj).parent().parent().find('#txtMaximumQuantity').val() == 'N/A' ? 0.0 : $(obj).find('#txtMaximumQuantity').val();
    var vItemguid = $(obj).parent().parent().find('#hdnItemGUID').val();
    var vhdnID = $(obj).parent().parent().find('#hdnID').val();
    var vhdnGUID = $(obj).parent().parent().find('#hdnGUID').val();

    if (SavetoSeesionItemLocationAllNew(vhdnGUID)) {
        // alert('dd');
        //
        if (newGuid != '00000000-0000-0000-0000-000000000000') {
            vhdnGUID = newGuid
        }
        if (vhdnGUID == '00000000-0000-0000-0000-000000000000') {
            //client side remove
            $(obj).parent().parent().remove();
            BindBinReplanish(vardelete);
        }
        else {
            if (vItemguid == '') {
                vItemguid = '00000000-0000-0000-0000-000000000000'
            }
            $.ajax({
                url: Inventory_CheckifanyCartEntryExist,
                data: { 'ITEMGUID': vItemguid, 'BinID': vBinID },
                dataType: 'json',
                type: 'POST',
                async: false,
                cache: false,
                success: function (response) {
                    //                    if (response.status == 'reference') {
                    //                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                    //                        $("#spanGlobalMessage").text('Suggested Order is exist within this Inventory location. So, not able to delete it.');
                    //                        $('div#target').fadeToggle();
                    //                        $("div#target").delay(2000).fadeOut(200);
                    //                    }
                    if (response.status == 'referencecount') {
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        $("#spanGlobalMessage").text('Inventory location is having quantity. So, not able to delete it.');
                        $('div#target').fadeToggle();
                        $("div#target").delay(2000).fadeOut(200);
                    }
                    else if (response.status == 'ok') {
                        ////Delete///
                        //  alert(response.status);
                        $.ajax({
                            url: Inventory_DeletetoSeesionBinReplanishSingle,
                            data: { 'ID': vhdnID, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'BinID': vBinID },
                            dataType: 'json',
                            type: 'POST',
                            async: false,
                            cache: false,
                            success: function (response) {
                                if (response.status = 'deleted') {
                                    //client side remove
                                    $(obj).parent().parent().remove();
                                    //bind grid
                                    BindBinReplanish(vardelete);

                                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                                    $("#spanGlobalMessage").text('Inventory Location deleted.');
                                    $('div#target').fadeToggle();
                                    $("div#target").delay(2000).fadeOut(200);

                                }
                                else if (response.status = 'error') {
                                    alert('Opps...Error....!');
                                }
                            }
                        });
                        ///Delete///
                    }
                }
            });

        }
    }
    return false;
}

function BindItemManufacture() {
    $('#divItemManufacturer').empty();
    $('#DivLoading').show();
    $.get(Inventory_LoadManufaturerofItem + '?ItemGUID=' + ItemGUID + '&AddCount=0', function (data) {
        $('#divItemManufacturer').html(data);
        // DisableLocationTextboxForBOM('True');
        $('#DivLoading').hide();
    });

}

function AddNewItemManufacture() {
    if (SavetoSeesionItemManufactureAll()) {

        $('#divItemManufacturer').empty();
        $('#DivLoading').show();
        $.get(Inventory_LoadManufaturerofItem + '?ItemGUID=' + ItemGUID + '&AddCount=1', function (data) {
            $('#divItemManufacturer').html(data);
            $('#DivLoading').hide();
        });

    }
    return false;
}

function SavetoSeesionItemManufacture(obj) {

    //var vdlManufacturerID = $(obj).parent().parent().find('#dlManufacturer')[0].value == '' ? 0 : $(obj).parent().parent().find('#dlManufacturer')[0].value;
    var vdlManufacturerID = $(obj).find('#SubManufacturerID').val();
    var vtxtManufacturer = $(obj).find('#txtManufacturer').val();
    var vtxtManufacturerNumber = $(obj).find('#txtManufacturerNumber').val();
    var vIsDefault = $(obj).find('#IsDefault').is(':checked');
    var vItemguid = $(obj).find('#hdnItemGUID').val();
    var vhdnID = $(obj).find('#hdnID').val();
    var vhdnGUID = $(obj).find('#hdnGUID').val();
    var vhdnSessionSr = $(obj).find('#hdnSessionSr').val();
    if (vdlManufacturerID == 0 && vtxtManufacturer == '' && vtxtManufacturerNumber == '') {
        return true;
    }
    if (vtxtManufacturer == '')
        vdlManufacturerID = 0;
    //        if (vtxtManufacturer == '') {
    //            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //            $("#spanGlobalMessage").text('Manufacturer is required.');
    //            $('div#target').fadeToggle();
    //            $("div#target").delay(2000).fadeOut(200);
    //            $(obj).find('#txtManufacturer').focus();
    //            return false;
    //        }
    //        if (vtxtManufacturerNumber == '') {
    //            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //            $("#spanGlobalMessage").text('Manufacturer Number is required.');
    //            $('div#target').fadeToggle();
    //            $("div#target").delay(2000).fadeOut(200);
    //            $(obj).find('#txtManufacturerNumber').focus();
    //            return false;
    //        }


    var TempSuprows = $("#ItemManufacturer").dataTable().fnGetNodes();
    var iCountSup = 0;
    var iCountSupDefault = 0;
    var iCountBlanketPO = 0;
    var iCountNumber = 0;

    var IsBlankAlreadyAdded = false;
    if (TempSuprows != null && TempSuprows.length > 0) {
        for (var i = 0; i < TempSuprows.length; i++) {
            if (TempSuprows[i].cells[0].getElementsByTagName('input').txtManufacturer.value == vtxtManufacturer) {
                iCountSup += 1;
                if (iCountSup > 1) {
                    $(TempSuprows[i]).css("background-color", "red");
                    $(TempSuprows[i].cells[0].getElementsByTagName('input').txtManufacturer).css("background-color", "yellow");
                    $(TempSuprows[i].cells[0].getElementsByTagName('input').txtManufacturer).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").text('This Manufacturer is already added.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    return false;
                }
            }
            if (vtxtManufacturerNumber == '') {
                IsBlankAlreadyAdded = true;
            }
            //                if (TempSuprows[i].cells[1].getElementsByTagName('input').txtManufacturerNumber.value == vtxtManufacturerNumber) {
            //                    iCountNumber += 1;
            //                    if (iCountNumber > 1) {
            //                        $(TempSuprows[i]).css("background-color", "red");
            //                        $(TempSuprows[i].cells[1].getElementsByTagName('input').txtManufacturerNumber).css("background-color", "yellow");
            //                        $(TempSuprows[i].cells[1].getElementsByTagName('input').txtManufacturerNumber).focus();
            //                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            //                        $("#spanGlobalMessage").text('This Manufacturer number is already added.');
            //                        $('div#target').fadeToggle();
            //                        $("div#target").delay(2000).fadeOut(200);
            //                        return false;
            //                    }
            //                }

            if (TempSuprows[i].cells[2].getElementsByTagName('input')[0].checked) {

                iCountSupDefault += 1;

                if (iCountSupDefault > 1) {
                    $(TempSuprows[i]).css("background-color", "red");
                    $(TempSuprows[i].cells[2].getElementsByTagName('input')[0]).css("background-color", "yellow");
                    $(TempSuprows[i].cells[2].getElementsByTagName('input')[0]).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").text('Default Manufacturer is already added.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    return false;
                }
            }
        }

    }
    //        if (CheckManuPartNumberAtRoomLeval(vtxtManufacturerNumber, vhdnID)) {
    $.ajax({
        url: Inventory_SavetoSeesionItemManufacture,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'ManufacturerID': vdlManufacturerID, 'ManufactureName': vtxtManufacturer, 'ManufacturerNumber': vtxtManufacturerNumber, 'IsDefault': vIsDefault },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'ok') {
                //BindItemManufacture();

                if (vIsDefault) {
                    $("#ManufacturerID").val(vdlManufacturerID);
                    $("#ManufacturerName").val($(obj).parent().parent().find('#txtManufacturer')[0].text);
                    $("#ManufacturerNumber").val($(obj).parent().parent().find('#txtManufacturerNumber').val());
                }

                //                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                //                    $("#spanGlobalMessage").text('Manufacturer Added.');
                //                    $('div#target').fadeToggle();
                //                    $("div#target").delay(2000).fadeOut(200);
            }
            else if (response.status == 'duplicate') {
                $(obj).parent().parent().find('#ManufacturerName').focus();
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").text('Manufacturer is already added.');
                $('div#target').fadeToggle();
                $("div#target").delay(2000).fadeOut(200);
            }

        }
    });
    //        }
    //        else {
    //            $(obj).parent().parent().find('#ManufacturerNumber').focus();
    //            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //            $("#spanGlobalMessage").text('Manufacturer number is already used in another item.');
    //            $('div#target').fadeToggle();
    //            $("div#target").delay(2000).fadeOut(200);
    //            return false;
    //        }
    return true;
}

function SavetoSeesionItemManufactureAll(obj) {
    //
    if ($("#ItemManufacturer").length > 0) {
        var TempSuprows2 = $("#ItemManufacturer").dataTable().fnGetNodes();
        var iCountSup = 0;
        var iCountSupDefault = 0;
        if (TempSuprows2 != null && TempSuprows2.length > 0) {
            for (var i = 0; i < TempSuprows2.length; i++) {
                //if (!(TempSuprows2[i].cells[0].getElementsByTagName('input').txtManufacturer.value == '' && TempSuprows2[i].cells[1].getElementsByTagName('input').txtManufacturerNumber.value == '')) {

                //                    if (TempSuprows2[i].cells[0].getElementsByTagName('input').txtManufacturer.value == '') {
                //                        $(TempSuprows2[i]).css("background-color", "red");
                //                        $(TempSuprows2[i].cells[0].getElementsByTagName('input')[0]).css("background-color", "yellow");
                //                        $(TempSuprows2[i].cells[0].getElementsByTagName('input')[0]).focus();
                //                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                //                        $("#spanGlobalMessage").text('Manufacturer is required.');
                //                        $('div#target').fadeToggle();
                //                        $("div#target").delay(2000).fadeOut(200);
                //                        return false;
                //                    }

                //                    if (TempSuprows2[i].cells[1].getElementsByTagName('input').txtManufacturerNumber.value == '') {
                //                        $(TempSuprows2[i]).css("background-color", "red");
                //                        $(TempSuprows2[i].cells[1].getElementsByTagName('input')[0]).css("background-color", "yellow");
                //                        $(TempSuprows2[i].cells[1].getElementsByTagName('input')[0]).focus();
                //                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                //                        $("#spanGlobalMessage").text('Manufacturer Number is required.');
                //                        $('div#target').fadeToggle();
                //                        $("div#target").delay(2000).fadeOut(200);
                //                        return false;
                //                    }

                if (TempSuprows2[i].cells[2].getElementsByTagName('input')[0].checked) {
                    iCountSupDefault += 1;

                    if (iCountSupDefault > 1) {
                        $(TempSuprows2[i]).css("background-color", "red");
                        $(TempSuprows2[i].cells[2].getElementsByTagName('input')[0]).css("background-color", "yellow");
                        $(TempSuprows2[i].cells[2].getElementsByTagName('input')[0]).focus();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").text('Default Manufacturer is already added.');
                        $('div#target').fadeToggle();
                        $("div#target").delay(2000).fadeOut(200);
                        return false;
                    }
                }

                if (!SavetoSeesionItemManufacture(TempSuprows2[i])) {
                    return false;
                }
                // }
            }
        }
    }
    return true;
}

function SavetoSeesionItemManufactureAllSave(obj) {
    //

    if ($("#ItemManufacturer").length > 0) {

        var TempSuprows2 = $("#ItemManufacturer").dataTable().fnGetNodes();
        var iCountSup = 0;
        var iCountSupDefault = 0;
        if (TempSuprows2 != null && TempSuprows2.length > 0) {
            for (var i = 0; i < TempSuprows2.length; i++) {
                if ((TempSuprows2[i].cells[0].getElementsByTagName('input').txtManufacturer.value != '') || (TempSuprows2[i].cells[1].getElementsByTagName('input').txtManufacturerNumber.value != '')) {
                    //if (!(TempSuprows2[i].cells[0].getElementsByTagName('input').txtManufacturer.value == '' && TempSuprows2[i].cells[1].getElementsByTagName('input').txtManufacturerNumber.value == '')) {

                    //                        if (TempSuprows2[i].cells[0].getElementsByTagName('input').txtManufacturer.value == '') {
                    //                            $(TempSuprows2[i]).css("background-color", "red");
                    //                            $(TempSuprows2[i].cells[0].getElementsByTagName('input')[0]).css("background-color", "yellow");
                    //                            $(TempSuprows2[i].cells[0].getElementsByTagName('input')[0]).focus();
                    //                            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    //                            $("#spanGlobalMessage").text('Manufacturer is required.');
                    //                            $('div#target').fadeToggle();
                    //                            $("div#target").delay(2000).fadeOut(200);
                    //                            return false;
                    //                        }

                    //                        if (TempSuprows2[i].cells[1].getElementsByTagName('input').txtManufacturerNumber.value == '') {
                    //                            $(TempSuprows2[i]).css("background-color", "red");
                    //                            $(TempSuprows2[i].cells[1].getElementsByTagName('input')[0]).css("background-color", "yellow");
                    //                            $(TempSuprows2[i].cells[1].getElementsByTagName('input')[0]).focus();
                    //                            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    //                            $("#spanGlobalMessage").text('Manufacturer Number is required.');

                    //                            $('div#target').fadeToggle();
                    //                            $("div#target").delay(2000).fadeOut(200);
                    //                            return false;
                    //                        }

                    if (TempSuprows2[i].cells[2].getElementsByTagName('input')[0].checked) {
                        iCountSupDefault += 1;

                        if (iCountSupDefault > 1) {
                            $(TempSuprows2[i]).css("background-color", "red");
                            $(TempSuprows2[i].cells[2].getElementsByTagName('input')[0]).css("background-color", "yellow");
                            $(TempSuprows2[i].cells[2].getElementsByTagName('input')[0]).focus();
                            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                            $("#spanGlobalMessage").text('Default Manufacturer is already added.');
                            $('div#target').fadeToggle();
                            $("div#target").delay(2000).fadeOut(200);
                            return false;
                        }
                    }

                    if (!SavetoSeesionItemManufacture(TempSuprows2[i])) {
                        return false;
                    }
                }
            }
        }
    }
    return true;
}

function DeletetoSeesionItemManufactureSingle(obj) {

    //
    //  var vdlManufacturerID = $(obj).parent().parent().find('#dlManufacturer')[0].value == '' ? 0 : $(obj).parent().parent().find('#dlManufacturer')[0].value;
    var vdlManufacturerID = $(obj).parent().parent().find('#hdnManufacturerID').val();
    var vtxtManufacturer = $(obj).parent().parent().find('#txtManufacturer').val();
    var vtxtManufacturerNumber = $(obj).parent().parent().find('#txtManufacturerNumber').val();
    var vIsDefault = $(obj).parent().parent().find('#IsDefault').is(':checked');
    var vItemguid = $(obj).parent().parent().find('#hdnItemGUID').val();
    var vhdnID = $(obj).parent().parent().find('#hdnID').val();
    var vhdnGUID = $(obj).parent().parent().find('#hdnGUID').val();
    var vhdnSessionSr = $(obj).parent().parent().find('#hdnSessionSr').val();

    if (vhdnGUID == '00000000-0000-0000-0000-000000000000') {
        //client side remove
        $(obj).parent().parent().remove();
        BindItemManufacture();
    }
    else {

        ////Delete///
        $.ajax({
            url: Inventory_DeletetoSeesionItemManufactureSingle,
            data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'ManufacturerID': vdlManufacturerID, 'ManufacturerNumber': vtxtManufacturerNumber, 'IsDefault': vIsDefault },
            dataType: 'json',
            type: 'POST',
            async: false,
            cache: false,
            success: function (response) {
                if (response.status = 'deleted') {
                    //client side remove
                    $(obj).parent().parent().remove();
                    //bind grid
                    BindItemManufacture();

                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    $("#spanGlobalMessage").text('Manufacturer deleted.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);

                }
                else if (response.status = 'error') {
                    alert('Opps...Error....!');
                }
            }
        });
        ///Delete///


    }
    return false;
}

function checkInventoryclassification(curobj) {
    //

    var vdlID = $(curobj).val();



    ////Delete///
    $.ajax({
        url: Inventory_checkInventoryclassification,
        data: { 'ID': vdlID },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status != '0') {
                $('#InventoryClassification').val(response.status);
                // alert('h1');
            }
            else if (response.status == '0') {
                $('#InventoryClassification').val(0);
                //alert('h2');
            }
            else if (response.status = 'error') {
                // alert('h3');
            }
        }
    });
    ///Delete///

    return false;
}

function ValidateManunumber() {

}

function BindItemSupplier() {
    $('#divItemSupplier').empty();
    $('#DivLoading').show();
    $.get(Inventory_LoadSupplierofItem + '?ItemGUID=' + ItemGUID + '&AddCount=0', function (data) {
        $('#divItemSupplier').html(data);
        // DisableLocationTextboxForBOM('True');
        $('#DivLoading').hide();
    });
}

function AddNewItemSupplier() {
    if (SavetoSeesionItemSupplierAll()) {

        $('#divItemSupplier').empty();
        $('#DivLoading').show();
        $.get(Inventory_LoadSupplierofItem + '?ItemGUID=' + ItemGUID + '&AddCount=1', function (data) {
            $('#divItemSupplier').html(data);
            $('#DivLoading').hide();
        });
    }

    return false;
}

function SavetoSeesionItemSupplierAll() {
    //
    if ($("#ItemSupplier").length > 0) {
        var TempSuprowsAdd = $("#ItemSupplier").dataTable().fnGetNodes();
        var iCountSupD = 0;
        if (TempSuprowsAdd != null && TempSuprowsAdd.length > 0) {
            for (var k = 0; k < TempSuprowsAdd.length; k++) {
                // if (!(TempSuprowsAdd[k].cells[0].getElementsByTagName('input').txtSupplier.value == '' && TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber.value == '')) {
                if (TempSuprowsAdd[k].cells[0].getElementsByTagName('input').txtSupplier.value == '') {
                    $(TempSuprowsAdd[k]).css("background-color", "red");
                    $(TempSuprowsAdd[k].cells[0].getElementsByTagName('input').txtSupplier).css("background-color", "yellow");
                    $(TempSuprowsAdd[k].cells[0].getElementsByTagName('input').txtSupplier).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").text('Supplier is required.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    return false;
                }

                if (TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber.value == '') {
                    $(TempSuprowsAdd[k]).css("background-color", "red");
                    $(TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber).css("background-color", "yellow");
                    $(TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").text('Supplier Number is required.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    return false;
                }

                if (TempSuprowsAdd[k].cells[3].getElementsByTagName('input')[0].checked) {
                    iCountSupD += 1;
                }

                if (!SavetoSeesionItemSupplier(TempSuprowsAdd[k])) {
                    return false;
                }
                //}
            }
        }
    }
    return true;
}

function SavetoSeesionItemSupplierAllSave() {
    //
    if ($("#ItemSupplier").length > 0) {
        var TempSuprowsAdd = $("#ItemSupplier").dataTable().fnGetNodes();
        var iCountSupD = 0;
        if (TempSuprowsAdd != null && TempSuprowsAdd.length > 0) {
            for (var k = 0; k < TempSuprowsAdd.length; k++) {
                if (!(TempSuprowsAdd[k].cells[0].getElementsByTagName('input').txtSupplier.value == '' && TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber.value == '')) {
                    if (TempSuprowsAdd[k].cells[0].getElementsByTagName('input').txtSupplier.value == '') {
                        $(TempSuprowsAdd[k]).css("background-color", "red");
                        $(TempSuprowsAdd[k].cells[0].getElementsByTagName('input').txtSupplier).css("background-color", "yellow");
                        $(TempSuprowsAdd[k].cells[0].getElementsByTagName('input').txtSupplier).focus();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").text('Supplier is required.');
                        $('div#target').fadeToggle();
                        $("div#target").delay(2000).fadeOut(200);
                        return false;
                    }

                    if (TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber.value == '') {
                        $(TempSuprowsAdd[k]).css("background-color", "red");
                        $(TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber).css("background-color", "yellow");
                        $(TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber).focus();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").text('Supplier Number is required.');
                        $('div#target').fadeToggle();
                        $("div#target").delay(2000).fadeOut(200);
                        return false;
                    }



                    if (TempSuprowsAdd[k].cells[3].getElementsByTagName('input')[0].checked) {
                        iCountSupD += 1;
                    }

                    if (!SavetoSeesionItemSupplier(TempSuprowsAdd[k])) {
                        return false;
                    }
                }
            }
        }
    }
    return true;
}

function SavetoSeesionItemSupplier(obj) {
    //
    var vdlSupplierID = $(obj).find('#SubSupplierID').val();
    var vtxtSupplier = $(obj).find('#txtSupplier').val();
    var vtxtSupplierNumber = $(obj).find('#txtSupplierNumber').val();
    var vsupIsDefault = $(obj).find('#IsDefault').is(':checked');
    var vItemguid = $(obj).find('#hdnItemGUID').val();
    var vsuphdnID = $(obj).find('#hdnID').val();
    var vsuphdnGUID = $(obj).find('#hdnGUID').val();
    var vsuphdnSessionSr = $(obj).find('#hdnSessionSr').val();
    var vBlanketPOIDCount = $(obj).find("#BlanketPOID").val() == '' ? 0 : $(obj).find("#BlanketPOID").val();
    var vhdExpiry = $(obj).find('#hdnExpiry').val();


    if (vdlSupplierID == 0 && vtxtSupplier == '' && vtxtSupplierNumber == '') {
        return true;
    }

    if (vtxtSupplier == '') {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").text('Supplier is required.');
        $('div#target').fadeToggle();
        $("div#target").delay(2000).fadeOut(200);
        $(obj).find('#txtSupplier').focus();
        return false;
    }

    if (vtxtSupplierNumber == '') {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").text('Supplier Number is required.');
        $('div#target').fadeToggle();
        $("div#target").delay(2000).fadeOut(200);
        $(obj).find('#txtSupplierNumber').focus();
        return false;
    }

    var suppresult = true;

    if (vBlanketPOIDCount > 0) {
        if (vhdExpiry > 0 && vBlanketPOIDCount == vhdExpiry) {

            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
            $("#spanGlobalMessage").text('Supplier Blanket PO is Expired, Please select another Blanket PO.');
            $('div#target').fadeToggle();
            $("div#target").delay(2000).fadeOut(200);
            $(obj).find("#BlanketPOID").focus();
            suppresult = false;
        }
        else {

            suppresult = true;
        }
    }


    if (suppresult == false) {

        return false;


    }
    var TempSuprows = $("#ItemSupplier").dataTable().fnGetNodes();
    var iCountSup = 0;
    var iCountSupDefault = 0;
    var iCountBlanketPO = 0;
    if (TempSuprows != null && TempSuprows.length > 0) {
        for (var i = 0; i < TempSuprows.length; i++) {
            if (TempSuprows[i].cells[0].getElementsByTagName('input').txtSupplier.value == vtxtSupplier) {
                iCountSup += 1;
                if (iCountSup > 1) {
                    $(TempSuprows[i]).css("background-color", "red");
                    $(TempSuprows[i].cells[0].getElementsByTagName('input').txtSupplier).css("background-color", "yellow");
                    $(TempSuprows[i].cells[0].getElementsByTagName('input').txtSupplier).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").text('This Supplier is already added.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    return false;
                }
            }
            if (TempSuprows[i].cells[3].getElementsByTagName('input')[0].checked) {
                iCountSupDefault += 1;

                if (iCountSupDefault > 1) {
                    $(TempSuprows[i]).css("background-color", "red");
                    $(TempSuprows[i].cells[3].getElementsByTagName('input')[0]).css("background-color", "yellow");
                    $(TempSuprows[i].cells[3].getElementsByTagName('input')[0]).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").text('Default Supplier is already added.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);
                    return false;
                }
            }
        }
    }

    //        if (CheckSuppPartNumberAtRoomLeval(vtxtSupplierNumber, vsuphdnID)) {
    $.ajax({
        url: Inventory_SavetoSeesionItemSupplier,
        data: { 'ID': vsuphdnID, 'SessionSr': vsuphdnSessionSr, 'GUID': vsuphdnGUID, 'ITEMGUID': vItemguid, 'SupplierID': vdlSupplierID, 'SupplierName': vtxtSupplier, 'SupplierNumber': vtxtSupplierNumber, 'IsDefault': vsupIsDefault, 'BlanketPOID': vBlanketPOIDCount },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'ok') {
                if (vsupIsDefault == true) {
                    $("#SupplierID").val(vdlSupplierID);
                    $("#SupplierName").val(vtxtSupplier);
                    $("#SupplierPartNo").val(vtxtSupplierNumber);
                }
                //BindItemSupplier();
                //$("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                //$("#spanGlobalMessage").text('Supplier Added.');
                //$('div#target').fadeToggle();
                //$("div#target").delay(2000).fadeOut(200);
            }
            else if (response.status == 'duplicate') {
                $(obj).find('#txtSupplier').focus();
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").text('This Supplier is already added.');
                $('div#target').fadeToggle();
                $("div#target").delay(2000).fadeOut(200);
            }
        },
        error: function (err) {
            alert(err);
        }
    });
    //        }
    //        else {
    //            $(obj).find('#txtSupplierNumber').focus();
    //            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //            $("#spanGlobalMessage").text('This Supplier number is already assigned to another item.');
    //            $('div#target').fadeToggle();
    //            $("div#target").delay(2000).fadeOut(200);
    //            return false;
    //        }
    return true;
}

function DeletetoSeesionItemSupplierSingle(obj) {

    var vdlSupplierID = $(obj).parent().parent().find('#SubSupplierID').val();
    var vtxtSupplier = $(obj).parent().parent().find('#txtSupplier').val();
    var vtxtSupplierNumber = $(obj).parent().parent().find('#txtSupplierNumber').val();
    var vsupIsDefault = $(obj).parent().parent().find('#IsDefault').is(':checked');
    var vItemguid = $(obj).parent().parent().find('#hdnItemGUID').val();
    var vsuphdnID = $(obj).parent().parent().find('#hdnID').val();
    var vsuphdnGUID = $(obj).parent().parent().find('#hdnGUID').val();
    var vsuphdnSessionSr = $(obj).parent().parent().find('#hdnSessionSr').val();
    var vBlanketPOIDCount = $(obj).parent().parent().find("#BlanketPOID").val() == '' ? 0 : $(obj).parent().parent().find("#BlanketPOID").val();


    if (vsuphdnGUID == '00000000-0000-0000-0000-000000000000') {
        //client side remove
        $(obj).parent().parent().remove();
        BindItemSupplier();
    }
    else {

        ////Delete///
        $.ajax({
            url: Inventory_DeletetoSeesionItemSupplierSingle,
            data: { 'ID': vsuphdnID, 'SessionSr': vsuphdnSessionSr, 'GUID': vsuphdnGUID, 'ITEMGUID': vItemguid, 'SupplierID': vdlSupplierID, 'SupplierNumber': vtxtSupplierNumber, 'IsDefault': vsupIsDefault, 'BlanketPOID': vBlanketPOIDCount },
            dataType: 'json',
            type: 'POST',
            async: false,
            cache: false,
            success: function (response) {
                if (response.status = 'deleted') {
                    //client side remove
                    $(obj).parent().parent().remove();
                    //bind grid
                    BindItemSupplier();

                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    $("#spanGlobalMessage").text('Supplier deleted.');
                    $('div#target').fadeToggle();
                    $("div#target").delay(2000).fadeOut(200);

                }
                else if (response.status = 'error') {
                    alert('Opps...Error....!');
                }
            }
        });
        ///Delete///


    }
    return false;
}

function SetCosignmentFields(val) {

    if (val == true) {
        $("#Cost").removeClass('disableBack');
        $("#Cost").removeProp("readonly");
    }
    else {
        if (MethodOfValuingInventory == 3) {   // Average Cost
            $("#Cost").addClass('disableBack');
            $("#Cost").prop("readonly", "readonly");
        }
        else if (MethodOfValuingInventory == 4) {  /// Last Cost
            $("#Cost").removeClass('disableBack');
            $("#Cost").removeProp("readonly");
        }
    }
}

function setDecimal(curobj) {
    var vdlID = $(curobj).val();
    var te = $(curobj).val().split(".");
    if (te[0] == '') {
        $(curobj).val("0" + $(curobj).val());
        vdlID = $(curobj).val();
    }
}

function AddMarkupAndCostToSellPrice(ActionType) {

    if (ActionType == 'cost') {
        if ($("#Cost").val() != $("#hdnOldCost").val()) {
            var TempSellPrice = parseFloat($("#SellPrice").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
            var Value1Cost = parseFloat($("#Cost").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
            var Value2MarkUp = parseFloat($("input[type='text'][id='Markup']").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));

            if (Value1Cost > 0 && Value2MarkUp > 0) {
                if (Value2MarkUp > 0) {
                    var TempCostValue = parseFloat($("#Cost").val()); // parseFloat(Value1Cost).toFixed(parseInt($('#hdCostcentsLimit').val(), 10))
                    var MarkUPValue = parseFloat(((parseFloat($("#Cost").val()) * parseFloat($("input[type='text'][id='Markup']").val())) / parseFloat(100)));
                    var FinalPirce = parseFloat(TempCostValue) + parseFloat(MarkUPValue);
                    $("#SellPrice").val(parseFloat(FinalPirce));
                }
            }
            else if (Value1Cost > 0) {
                var tempValue = parseFloat($("#Cost").val());
                $("#SellPrice").val(tempValue);
            }
            else {
                $("#SellPrice").val(parseFloat(0));

                $("input[type='text'][id='Markup']").val(parseFloat(0));
            }
        }
        //            var TempSellPrice = parseFloat($("#SellPrice").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
        //            var Value1Cost = parseFloat($("#Cost").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
        //            var Value2MarkUp = parseFloat($("input[type='text'][id='Markup']").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));

        //            if (Value1Cost > 0 && Value2MarkUp > 0) {
        //                if (Value2MarkUp > 0) {
        //                    var TempCostValue = parseFloat($("#Cost").val()); // parseFloat(Value1Cost).toFixed(parseInt($('#hdCostcentsLimit').val(), 10))
        //                    var MarkUPValue = parseFloat(((parseFloat($("#Cost").val()) * parseFloat($("input[type='text'][id='Markup']").val())) / parseFloat(100)));
        //                    var FinalPirce = parseFloat(TempCostValue) + parseFloat(MarkUPValue);
        //                    $("#SellPrice").val(parseFloat(FinalPirce));
        //                }
        //            }
        //            else if (Value1Cost > 0) {
        //                var tempValue = parseFloat($("#Cost").val());
        //                $("#SellPrice").val(tempValue);
        //            }
        //            else {
        //                $("#SellPrice").val(parseFloat(0));
        //                $("input[type='text'][id='Markup']").val(parseFloat(0));
        //            }
    }
    else if (ActionType == 'markup') {
        if ($("input[type='text'][id='Markup']").val() != "" && parseFloat($("input[type='text'][id='Markup']").val()) > 0) {


            var Value1Cost = parseFloat($("#Cost").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
            var Value2MarkUp = parseFloat($("input[type='text'][id='Markup']").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
            var TempSellPrice = parseFloat($("#SellPrice").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
            if (Value1Cost > 0) {
                var TempCostValue = parseFloat($("#Cost").val());
                var MarkUPValue = parseFloat(((parseFloat($("#Cost").val()) * parseFloat($("input[type='text'][id='Markup']").val())) / parseFloat(100)));
                var FinalPirce = parseFloat(TempCostValue) + parseFloat(MarkUPValue);

                $("#SellPrice").val(parseFloat(FinalPirce));
            }
            else if (Value1Cost > 0 && TempSellPrice > 0) {
                var TempValue = parseFloat($("#SellPrice").val()) - parseFloat($("#Cost").val());
                var TempmarkUp = parseFloat(TempValue) / parseFloat($("#Cost").val());
                if (isNaN(TempmarkUp))
                    TempmarkUp = 0;
                var FinalPrice = parseFloat(TempmarkUp) * parseFloat(100.00);

                $("input[type='text'][id='Markup']").val(parseFloat(FinalPrice));
            }
            else if (Value1Cost > 0) {
                var tempValue = parseFloat($("#Cost").val());

                $("#SellPrice").val(tempValue);
            }
            else {
                $("#SellPrice").val(parseFloat(0));

                $("input[type='text'][id='Markup']").val(parseFloat(0));
            }
        }
    }
    else if (ActionType == 'sell') {

        //if ($("#SellPrice").val() != $("#hdnOldSellPrise").val()) {
        var TempCostPrice = parseFloat($("#Cost").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
        var TempSellPrice = parseFloat($("#SellPrice").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
        var Value2MarkUp = parseFloat($("input[type='text'][id='Markup']").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));

        if (parseFloat(TempSellPrice) <= 0 && parseFloat(TempCostPrice) > 0 && parseFloat(Value2MarkUp) > 0) {
            var TempCostValue = parseFloat($("#Cost").val());
            var MarkUPValue = parseFloat(((parseFloat($("#Cost").val()) * parseFloat($("input[type='text'][id='Markup']").val())) / parseFloat(100)));
            var FinalPirce = parseFloat(TempCostValue) + parseFloat(MarkUPValue);
            $("#SellPrice").val(parseFloat(FinalPirce));
        }
        else if (parseFloat(TempCostPrice) > 0 && parseFloat(TempSellPrice) > 0) {

            var TempValue = parseFloat($("#SellPrice").val()) - parseFloat($("#Cost").val());
            var TempmarkUp = parseFloat(TempValue) / parseFloat($("#Cost").val());
            var FinalPrice = parseFloat(TempmarkUp) * parseFloat(100.00);

            $("input[type='text'][id='Markup']").val(FormatedCostQtyValues(parseFloat(FinalPrice), 1));
        }
        //}

        //            var TempCostPrice = parseFloat($("#Cost").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
        //            var TempSellPrice = parseFloat($("#SellPrice").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));
        //            var Value2MarkUp = parseFloat($("input[type='text'][id='Markup']").val()).toFixed(parseInt($('#hdCostcentsLimit').val(), 10));

        //            if (parseFloat(TempSellPrice) <= 0 && parseFloat(TempCostPrice) > 0 && parseFloat(Value2MarkUp) > 0) {
        //                var TempCostValue = parseFloat($("#Cost").val());
        //                var MarkUPValue = parseFloat(((parseFloat($("#Cost").val()) * parseFloat($("input[type='text'][id='Markup']").val())) / parseFloat(100)));
        //                var FinalPirce = parseFloat(TempCostValue) + parseFloat(MarkUPValue);
        //                $("#SellPrice").val(parseFloat(FinalPirce));
        //            }
        //            else if (parseFloat(TempCostPrice) > 0 && parseFloat(TempSellPrice) > 0) {

        //                var TempValue = parseFloat($("#SellPrice").val()) - parseFloat($("#Cost").val());
        //                var TempmarkUp = parseFloat(TempValue) / parseFloat($("#Cost").val());
        //                var FinalPrice = parseFloat(TempmarkUp) * parseFloat(100.00);
        //                $("input[type='text'][id='Markup']").val(parseFloat(FinalPrice));
        //            }
    }

    return false;
}

function is_valid_url(curobj) {
    var url = $(curobj).val();
    if (url != '' && url != null) {
        var IsValidURL = /^(http(s)?:\/\/)?(www\.)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/.test(url);
        if (IsValidURL == false) {
            alert('Invalid URL, kindly enter valid URL');
            $(curobj).val("");
            $(curobj).focus();
        }
    }
    return false;
}

function AddNewFromPopup(PopupFor) {
    $('#DivLoading').show();
    var _URL = ''
    if (PopupFor == 'Manufacturer') {
        _URL = menufURL;
    }
    else if (PopupFor == 'Supplier') {
        _URL = supURL;
    }
    else if (PopupFor == 'Unit') {
        _URL = Master_UnitCreate;
    }
    else if (PopupFor == 'Category') {
        _URL = Master_CategoryCreate;
    }
    else if (PopupFor == 'Location') {
        _URL = Master_BinCreate;
    }
    else if (PopupFor == 'CostUOM') {
        _URL = Master_CostUOMCreate;
    }
    else if (PopupFor == 'UOMUnit') {
        _URL = Master_UnitCreate;
    }
    else if (PopupFor == 'InventoryClassification') {
        _URL = Master_InventoryClassificationCreate;
    }
    else {
        return false;
    }

    if (PopupFor != 'InventoryClassification') {
        $('#NewMasterPopUP').load(_URL, function () {
            $('#NewMasterPopUP').data("data", PopupFor).dialog('open');
            $('#DivLoading').hide();
        });
    }
    else {
        $('#NewMasterPopUP').load(_URL, function () {
            $('#NewMasterPopUP').data("data", PopupFor).dialog('open');
            $('#DivLoading').hide();
        });
    }
}

function RefreshDropdownList(PopupFor, IDVal) {
    if (IDVal != undefined) {
        var _ControlID = '';
        if (PopupFor == 'Manufacturer') {
            _ControlID = "dlManufacturer";

            BindItemManufacture();
        }
        else if (PopupFor == 'Supplier') {
            _ControlID = "dlSupplier";

            BindItemSupplier();
        }
        else if (PopupFor == 'Unit') {
            _ControlID = "UOMID";
        }
        else if (PopupFor == 'Category') {
            _ControlID = "CategoryID";
        }
        else if (PopupFor == 'Location') {
            _ControlID = "DefaultLocation";
        }
        else if (PopupFor == 'CostUOM') {
            _ControlID = "CostUOMID";
        }
        else if (PopupFor == 'UOMUnit') {
            _ControlID = "UOMID";
        }
        else if (PopupFor == 'InventoryClassification') {
            _ControlID = "InventoryClassification";
        }

        var arrdata = IDVal.split("~");
        var listData = $('select[id="' + _ControlID + '"]');
        $(listData).each(function () {
            $(this).append($("<option />").val(arrdata[0]).text(arrdata[1]));
        });
    }
    $('#DivLoading').hide();
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