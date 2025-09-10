var Status = 'ok';
$(document).ready(function () {
    window.location.hash = '';
    setTimeout(function () {
        AllowOhQty();
        if ($("input#OnHandQuantity").val() != '') {
            $("input#OnHandQuantity").val(FormatedCostQtyValues(parseFloat($("input#OnHandQuantity").val()), 2));
        }
        if ($("input#ExtendedCost").val() != '') {

            $("input#ExtendedCost").val(FormatedCostQtyValues(parseFloat($("input#ExtendedCost").val()), 1));
        }
        if ($("input#AverageUsage").val() != '') {
            $("input#AverageUsage").val(FormatedCostQtyValues(parseFloat($("input#AverageUsage").val()), 4));
        }
        if ($("input#AverageCost").val() != '') {
            $("input#AverageCost").val(FormatedCostQtyValues(parseFloat($("input#AverageCost").val()), 1));
        }

        if ($("input#PerItemCost").val() != '') {
            $("input#PerItemCost").val(FormatedCostQtyValues(parseFloat($("input#PerItemCost").val()), 1));
        }

        if ($("input#SuggestedOrderQuantity").val() != '') {

            $("input#SuggestedOrderQuantity").val(FormatedCostQtyValues(parseFloat($("input#SuggestedOrderQuantity").val()), 2));
        }
        if ($("input#SuggestedTransferQuantity").val() != '') {

            $("input#SuggestedTransferQuantity").val(FormatedCostQtyValues(parseFloat($("input#SuggestedTransferQuantity").val()), 2));
        }
        if ($("input#OnOrderQuantity").val() != '') {

            $("input#OnOrderQuantity").val(FormatedCostQtyValues(parseFloat($("input#OnOrderQuantity").val()), 2));
        }

        if ($("input#OnOrderInTransitQuantity").val() != '') {

            $("input#OnOrderInTransitQuantity").val(FormatedCostQtyValues(parseFloat($("input#OnOrderInTransitQuantity").val()), 2));
        }
        if ($("input#OnReturnQuantity").val() != '') {

            $("input#OnReturnQuantity").val(FormatedCostQtyValues(parseFloat($("input#OnReturnQuantity").val()), 2));
        }
        if ($("input#OnTransferQuantity").val() != '') {

            $("input#OnTransferQuantity").val(FormatedCostQtyValues(parseFloat($("input#OnTransferQuantity").val()), 2));
        }
        if ($("input#RequisitionedQuantity").val() != '') {

            $("input#RequisitionedQuantity").val(FormatedCostQtyValues(parseFloat($("input#RequisitionedQuantity").val()), 2));
        }
        if ($("input#StagedQuantity").val() != '') {

            $("input#StagedQuantity").val(FormatedCostQtyValues(parseFloat($("input#StagedQuantity").val()), 2));
        }
        if ($("input#InTransitquantity").val() != '') {

            $("input#InTransitquantity").val(FormatedCostQtyValues(parseFloat($("input#InTransitquantity").val()), 2));
        }
        if ($("input#QtyToMeetDemand").val() != '') {

            $("input#QtyToMeetDemand").val(FormatedCostQtyValues(parseFloat($("input#QtyToMeetDemand").val()), 2));
        }
        if ($("input#OutTransferQuantity").val() != '') {

            $("input#OutTransferQuantity").val(FormatedCostQtyValues(parseFloat($("input#OutTransferQuantity").val()), 2));
        }
        if ($("input#Turns").val() != '') {

            $("input#Turns").val(FormatedCostQtyValues(parseFloat($("input#Turns").val()), 4));
        }
        if ($("input#CriticalQuantity").val() != '') {
            $("input#CriticalQuantity").val(FormatedCostQtyValues(parseFloat($("input#CriticalQuantity").val()), 2));
        }
        if ($("input#MinimumQuantity").val() != '') {
            $("input#MinimumQuantity").val(FormatedCostQtyValues(parseFloat($("input#MinimumQuantity").val()), 2));
        }
        if ($("input#MaximumQuantity").val() != '') {
            $("input#MaximumQuantity").val(FormatedCostQtyValues(parseFloat($("input#MaximumQuantity").val()), 2));
        }


        $("#divitemlocationbinreplanish").on({
            focus: function () {
                //var Currentinput = $(this);
                //setTimeout(function () {
                //    if (!($(Currentinput).hasClass("ui-autocomplete-input"))) {
                //        SetAutoCompleteOpenOnFocus($(Currentinput), Master_GetAllLocationOfRoom, null, " ");
                //    }
                //}, 100);


                if (!$(this).hasClass("ui-autocomplete-input")) {
                    SetAutoCompleteOpenOnFocus($(this), Master_GetAllLocationOfRoom, null, " ");
                }
            }

        }, "input[type='text'][id='txtLocation']");

        $("#divitemlocationbinreplanish").on({ click: function () { $(this).siblings('#txtLocation').trigger("focus"); } }, ".ShowAllOptionsBin");


        $("input#rdoSerialTracking").click(function () {
            AllowOhQty();
        });
        $("input#rdoLotTracking").click(function () {

            AllowOhQty();
        });
        $("input#chkDateCodeTracking").change(function () {
            AllowOhQty();
        });

        $("div#divItemSupplier").on({
            click: function () {
                $(this).siblings(".autocompleteSup").autocomplete("search", "");
                $(this).siblings('.autocompleteSup').trigger("focus");
            }
        }, ".ShowAllOptionsSup");

        $('.ShowAllOptionsSup').click(function () {
            $(this).siblings(".autocompleteSup").autocomplete("search", "");
            $(this).siblings('.autocompleteSup').trigger("focus");
        });

        $('form').areYouSure();
        $.validator.unobtrusive.parse("#frmItemMaster");
        if (ItemImageType == 'ImagePath' || ItemImageType == '') {
            //$("#ItemImage").show();
            $("#CustomItemImage").show();
            $("#ExternalURL").hide();
            if (ItemImagePath != undefined && ItemImagePath != null && ItemImagePath != '') {
                $("#btnDeleteImage").show();
            }
            else {
                $("#btnDeleteImage").hide();
            }
        }
        else {
            $("#ItemImage").hide();
            $("#CustomItemImage").hide();
            $("#ExternalURL").show();
            $("#btnDeleteImage").hide();
        }
        if (ItemLink2ImageType == 'InternalLink' || ItemImageType == '') {
            // $("#Link2").show();
            $("#CustomLink2").show();
            $("#CustomLink2").show();
            $("#lnkExternalURL").hide();

            if (ItemImageLink != undefined && ItemImageLink != null && ItemImageLink != '') {
                $("#btnDeleteLink").show();
            }
            else {
                $("#btnDeleteLink").hide();
            }
        }
        else {
            $("#Link2").hide();
            $("#CustomLink2").hide();
            $("#lnkExternalURL").show();
            $("#btnDeleteLink").hide();
        }
        $("#ItemImage").change(function () {
            readURL(this);
        });
        $("#Link2").change(function () {
            readURLLink2(this);
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

        // $('#btnCancel').on('click', function (e) {
        $('#frmItemMaster').on('click', 'input#btnCancel', function (e) {
            if (gblActionName.toLowerCase() == "orderlist" || gblActionName.toLowerCase() == "returnorderlist") {
                //OpenItemPopup();
                $("#divAddNewItemDailog").dialog("close");
                return;
            }

            SwitchTextTab(0, 'ItemCreate', 'frmItemMaster');
            if (oTable !== undefined && oTable != null) {
                oTable.fnStandingRedraw();
            }
            //$('#NarroSearchClear').click();
        });

        if (IsItemLevelMinMaxQtyRequired == 'True') {
            $('#liCriticalQuantity').show();
            $('#liMinimumQuantity').show();
            $('#liMaximumQuantity').show();
            $('#liDefaultLocation').hide();
            BindBinReplanish(true);
        }
        else {
            if (ItemType != 4) {
                $('#liCriticalQuantity').hide();
                $('#liMinimumQuantity').hide();
                $('#liMaximumQuantity').hide();
                $('#liDefaultLocation').hide();
                BindBinReplanish(false);
            }
        }
        //$("#btnSave").click(function () {
        $('#frmItemMaster').on('click', 'input#btnSave', function (e) {

            if ($("input[type='radio'][name='ItemTraking']:checked").length > 0) {
                $("input[type='hidden'][id='hdnItmTracking']").remove();
            }

            if ($("input#ItemImageExternalURL").val() != '') {

                if (!CheckValidURLForImage($("input#ItemImageExternalURL"))) {
                    //$("input#ItemImageExternalURL").val('');
                }
                // if (!CheckValidURLForLink2($("input#ItemLink2ExternalURL"))) {
                //$("input#ItemImageExternalURL").val('');
                //  }

            }
            if ($("input#ItemLink2ExternalURL").val() != '') {

                if (!CheckValidURLForLink2($("input#ItemLink2ExternalURL"))) {
                    //$("input#ItemLink2ExternalURL").val('');
                    //e.preventDefault();
                    //return false;
                }
                // if (!CheckValidURLForLink2($("input#ItemLink2ExternalURL"))) {
                //$("input#ItemImageExternalURL").val('');
                //  }

            }
            if ($("input#ExtendedCost").val() != '') {

                $("input#ExtendedCost").val(FormatedCostQtyValues(parseFloat($("input#ExtendedCost").val()), 1));
            }
            if ($("input#AverageCost").val() != '') {
                $("input#AverageCost").val(FormatedCostQtyValues(parseFloat($("input#AverageCost").val()), 1));
            }

            if ($("input#PerItemCost").val() != '') {
                $("input#PerItemCost").val(FormatedCostQtyValues(parseFloat($("input#PerItemCost").val()), 1));
            }
            //$('#NarroSearchClear').click();
            var cost1 = $("#Cost").val();
            var price1 = $("#SellPrice").val();
            var markup1 = $("input[type='text'][id='Markup']").val();
            //if (MethodOfValuingInventory == 3) {
            //    var isConsigned = $('#Consignment').prop("checked");
            //    var onHandQty = $("input#OnHandQuantity").val();
            //    if (!isConsigned && (onHandQty == null || onHandQty <= 0)) {
            //        markup1 = 0;
            //        $("input[type='text'][id='Markup']").val(markup1);
            //        $("input[type='hidden'][id='Markup']").val($("input[type='text'][id='Markup']").val());
            //    }
            //}
        });
        var check;
        $("input[type='radio'][name='ItemTraking']").hover(function () {
            check = $(this).is(':checked');
        });
        $("input[type='radio'][name='ItemTraking']").click(function () {
            check = !check;
            $(this).prop("checked", check);
        });
        checkRememberUDFValues("ItemMaster", ItemID);//checkRememberUDFValues($("#hdnPageName").val(), ItemID); //

        BindItemManufacture();
        BindItemSupplier();



        $("#divKitDetails").on("change", "input[type='text'][id='txtQuantityPerKit']", function () {
            var curobj = $(this);
            var itmguid = $(curobj).parent().parent().find("input[type='hidden'][id='hdnItemGUID']").val();
            var inputqty = $(curobj).val();
            if (!isNaN(inputqty) && inputqty != '' && parseInt(inputqty) > 0) {
                $.ajax({
                    type: "POST",
                    url: Inventory_SaveKitQty,
                    contentType: 'application/json',
                    dataType: 'json',
                    data: "{ItemGUID:'" + ItemGUID + "',KitItemGuid:'" + itmguid + "',QuantityPerKit:'" + $(curobj).val() + "'}",
                    success: function (retdt) {

                    },
                    error: function (err) {
                        //
                        alert(MsgErrorInProcess);
                    }
                });
            } else {
                $(curobj).val("");
            }
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

                BindKitDetails(); //consol 1
            }

            if (ItemType == 4) {
                //  EnableDisableLabour(true);
                //
                $('#liCriticalQuantity').find('#CriticalQuantity').addClass('disableBack');
                $('#liMinimumQuantity').find('#MinimumQuantity').addClass('disableBack');
                $('#liMaximumQuantity').find('#MaximumQuantity').addClass('disableBack');
                // $('#liDefaultPullQuantity').find('#DefaultPullQuantity').addClass('disableBack');
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
                //   $('#liDefaultPullQuantity').find('#DefaultPullQuantity').removeClass('disableBack');
                $('#liDefaultReorderQuantity').find('#DefaultReorderQuantity').removeClass('disableBack');
                // $('#liCriticalQuantity').find('#CriticalQuantity')[0].value = '0.0';
                // $('#liMinimumQuantity').find('#MinimumQuantity')[0].value = '0.0';
                // $('#liMaximumQuantity').find('#MaximumQuantity')[0].value = '0.0';
                $('#liDefaultLocation').show();
                // BindBinReplanish(false);
            }

            //if ($('#IsItemLevelMinMaxQtyRequired').attr('type') != 'hidden') {
            //    $('#IsItemLevelMinMaxQtyRequired').attr('disabled', true);
            //}
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

        $(':hidden[name=Consignment]').val($('#Consignment').is(":checked"));
        $('#Consignment').change(function () {
            SetCosignmentFields($('#Consignment').prop("checked"));
            $(':hidden[name=Consignment]').val($('#Consignment').is(":checked"));
        });



        $('#dlItemType').change(function (e) {
            //  alert('hi');

            if ($(this).val() == '4') {

                EnableDisableLabour(true);
                //
                $('#liCriticalQuantity').find('#CriticalQuantity').addClass('disableBack');
                $('#liMinimumQuantity').find('#MinimumQuantity').addClass('disableBack');
                $('#liMaximumQuantity').find('#MaximumQuantity').addClass('disableBack');
                $('#OnHandQuantity').addClass('disableBack');
                //$('#liDefaultPullQuantity').find('#DefaultPullQuantity').addClass('disableBack');
                $('#liDefaultReorderQuantity').find('#DefaultReorderQuantity').addClass('disableBack');
                $('#liCriticalQuantity').find('#CriticalQuantity')[0].value = '';
                $('#liMinimumQuantity').find('#MinimumQuantity')[0].value = '';
                $('#liMaximumQuantity').find('#MaximumQuantity')[0].value = '';
                var cost1 = $("#Cost").val();
                var price1 = $("#SellPrice").val();
                if (cost1 == "" || isNaN(cost1)) {
                    cost1 = 0;
                }
                if (price1 == "" || isNaN(price1)) {
                    price1 = 0;
                }
                if (cost1 == 0 && price1 == 0) {
                    var RoomGlobMarkupLabor = $("#hdnRoomGlobMarkupLabor").val();
                    if (RoomGlobMarkupLabor != null && RoomGlobMarkupLabor != undefined && RoomGlobMarkupLabor != "" && !isNaN(RoomGlobMarkupLabor)) {
                        $("input[type='text'][id='Markup']").val(RoomGlobMarkupLabor);
                    }
                    else {
                        $("input[type='text'][id='Markup']").val("0");
                    }
                }
            }
            else {


                EnableDisableLabour(false);
                $('#liCriticalQuantity').find('#CriticalQuantity').removeClass('disableBack');
                $('#liMinimumQuantity').find('#MinimumQuantity').removeClass('disableBack');
                $('#liMaximumQuantity').find('#MaximumQuantity').removeClass('disableBack');
                $('#OnHandQuantity').removeClass('disableBack');
                //$('#liDefaultPullQuantity').find('#DefaultPullQuantity').removeClass('disableBack');
                $('#liDefaultReorderQuantity').find('#DefaultReorderQuantity').removeClass('disableBack');
                $('#liCriticalQuantity').find('#CriticalQuantity')[0].value = FormatedCostQtyValues(0, 2);
                $('#liMinimumQuantity').find('#MinimumQuantity')[0].value = FormatedCostQtyValues(0, 2);
                $('#liMaximumQuantity').find('#MaximumQuantity')[0].value = FormatedCostQtyValues(0, 2);

                var cost1 = $("#Cost").val();
                var price1 = $("#SellPrice").val();
                if (cost1 == "" || isNaN(cost1)) {
                    cost1 = 0;
                }
                if (price1 == "" || isNaN(price1)) {
                    price1 = 0;
                }
                if (cost1 == 0 && price1 == 0) {
                    var RoomGlobMarkupParts = $("#hdnRoomGlobMarkupParts").val();
                    if (RoomGlobMarkupParts != null && RoomGlobMarkupParts != undefined && RoomGlobMarkupParts != "" && !isNaN(RoomGlobMarkupParts)) {
                        $("input[type='text'][id='Markup']").val(RoomGlobMarkupParts);
                    }
                    else {
                        $("input[type='text'][id='Markup']").val("0");
                    }
                }
            }

            BindKitDetails(); //console 2
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
            if (ItemType != 4) {
                AllowOhQty();
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
                BindBinReplanish(true);
                checkboxcheck = true;
            }
            else {
                $('#liCriticalQuantity').hide();
                $('#liMinimumQuantity').hide();
                $('#liMaximumQuantity').hide();
                $('#liDefaultLocation').hide();
                BindBinReplanish(false);
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
        if (ViewBag_LockIsAllowOrderCostuom == 'True') {
            $('#IsAllowOrderCostuom').attr('disabled', true);
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

        $("#Cost").change(function () {
            //checkInventoryclassification(this);
            setDecimal(this);
            return AddMarkupAndCostToSellPrice('cost');
        });
        $("input[type='text'][id='Markup']").change(function () {
            setDecimal(this);
            return AddMarkupAndCostToSellPrice('markup');


        });
        $("#SellPrice").change(function () {
            setDecimal(this);
            return AddMarkupAndCostToSellPrice('sell');
        });

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
                $('[aria-labelledby="ui-dialog-title-ItemModelPS"]').focus();
            },
            close: function () {
                BindKitDetails(); //console 3
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

    $('#OnHandQuantity').change(function () {
        if ($('#OnHandQuantity').val() == null || $('#OnHandQuantity').val() == undefined
            || $('#OnHandQuantity').val().trim() == '' || parseFloat($('#OnHandQuantity').val().trim()) <= 0) {
            $("#Cost").addClass('disableBack');
            $("#Cost").prop("readonly", "readonly");

            $("#SellPrice").addClass('disableBack');
            $("#SellPrice").prop("readonly", "readonly");

            $("#Cost").val('0');
            $("#Markup").val('0');
            $("#SellPrice").val('0');
        }
        else {
            //$('#IsBuildBreak').attr('checked', false);
            $("#Cost").removeClass('disableBack');
            $("#Cost").removeProp("readonly");

            $("#SellPrice").removeClass('disableBack');
            $("#SellPrice").removeProp("readonly");
        }

        if (ItemID == 0) {
            var onHandQty = $("input#OnHandQuantity").val();
            if (onHandQty !== undefined && onHandQty != null && $.isNumeric(onHandQty.trim())) {
                $('[data-calc="OHQ"]').val("");
                var defaultBin = $('[data-id="txtLoc"]:checked=checked');
                var isConsigned = $('#Consignment').prop("checked");

                if (isConsigned == true) {
                    $(defaultBin).parent().parent().find('input[id="txtConsignedQuantity"]').val(parseFloat(onHandQty.trim())); //$(defaultBin).closest('tr').find('#txtConsignedQuantity').val(parseFloat(onHandQty.trim()));
                }
                else {
                    $(defaultBin).parent().parent().find('input[id="txtCustomerOwnedQuantity"]').val(parseFloat(onHandQty.trim()));
                }
            }
        }
    });

    $('#IsBuildBreak').click(function () {
        if ($('#IsBuildBreak').is(":checked")) {
            //if (ItemID > 0) 
            {
                $('#OnHandQuantity').val('');
                $("input#OnHandQuantity").val("");
                $("input#OnHandQuantity").prop("readonly", "readonly").addClass("disableBack");
            }
        }
        else {
            $("input#OnHandQuantity").removeProp("readonly").removeClass("disableBack");
        }
        AllowOhQty();
    });

    var intNotAllowededCode = charCode.split(',');
    if (intNotAllowededCode != null && intNotAllowededCode.length > 0) {
        specialKeys = new Array();
        for (i = 0; i < intNotAllowededCode.length; i++) {
            specialKeys.push(parseInt(intNotAllowededCode[i])); //Backspace
        }
    }
    if (ItemID > 0) {
        disableCustomerConsignedQtyOnAddLocation();
    }

    if ($("#IsAllowOrderCostuom").attr('checked') == 'checked') {
        $("#IsEnforceDefaultReorderQuantity").attr('checked', 'checked');
        $("#IsEnforceDefaultReorderQuantity").attr('disabled', 'disabled');
    }

    $("#CostUOMID").change(function (e) {
        var vCostUOMData = $("#CostUOMData").val();
        var prevRQ = $("#DefaultReorderQuantity").val();
        var vCUOMVal = $('#CostUOMID option:selected').html();
        if ($("#IsAllowOrderCostuom").attr('checked') == 'checked') {
            $("#IsEnforceDefaultReorderQuantity").attr('checked', 'checked');
            var vCostUOMVal = GetCostUOMValue(vCostUOMData, vCUOMVal);


            if (prevRQ.length > 0) {
                if (parseInt(vCostUOMVal) > parseInt(prevRQ)) {
                    $("#DefaultReorderQuantity").val(vCostUOMVal);
                    SetCostUOMvalueForBinGridCostUOMChange();
                }
                else {
                    var actValue = vCostUOMVal;
                    if ((prevRQ % vCostUOMVal) == 0) {
                        actValue = prevRQ;
                    }
                    else {
                        //actValue = parseInt(prevRQ) + (parseInt(vCostUOMVal) - (parseInt(prevRQ) % parseInt(vCostUOMVal)));
                    }
                    $("#DefaultReorderQuantity").val(actValue);
                    SetCostUOMvalueForBinGridCostUOMChange();
                }
            }
            else {
                $("#DefaultReorderQuantity").val(vCostUOMVal);
                SetCostUOMvalueForBinGridCostUOMChange();
            }

            //if (vCostUOMVal == null || vCostUOMVal == undefined || isNaN(vCostUOMVal)) {
            //    $("#DefaultReorderQuantity").val(1);
            //} else {
            //    $("#DefaultReorderQuantity").val(vCostUOMVal);
            //}

        }

    });

    $("#IsAllowOrderCostuom").change(function (e) {
        var vCostUOMData = $("#CostUOMData").val();
        var prevRQ = $("#DefaultReorderQuantity").val();
        if ($(this).attr('checked') == 'checked') {
            var vCUOMVal = $('#CostUOMID option:selected').html();
            var vCostUOMVal = GetCostUOMValue(vCostUOMData, vCUOMVal);

            if (prevRQ.length > 0) {
                if (parseInt(vCostUOMVal) > parseInt(prevRQ)) {
                    $("#DefaultReorderQuantity").val(vCostUOMVal);
                }
                else {
                    var actValue = vCostUOMVal;
                    if ((prevRQ % vCostUOMVal) == 0) {
                        actValue = prevRQ;

                    }
                    else {
                        //actValue = parseInt(prevRQ) + (parseInt(vCostUOMVal) - (parseInt(prevRQ) % parseInt(vCostUOMVal)));
                    }
                    $("#DefaultReorderQuantity").val(actValue);
                }
            }
            else {
                $("#DefaultReorderQuantity").val(vCostUOMVal);
            }


            //if (vCostUOMVal == null || vCostUOMVal == undefined || isNaN(vCostUOMVal)) {
            //    $("#DefaultReorderQuantity").val(1);
            //}
            //else {
            //    $("#DefaultReorderQuantity").val(vCostUOMVal);
            //}
            if (IsBOMItem === 'False') {
                $("#IsEnforceDefaultReorderQuantity").attr('checked', 'checked');
                $("#IsEnforceDefaultReorderQuantity").attr('disabled', 'disabled');
            }

            // Code for Bin Grid
            var DefQTY = $("#DefaultReorderQuantity").val();
            $('#ItemLocationLevelQuanity tbody tr').each(function () {

                $(this).find("#chkIsEnforceDefaultReorderQuantity").attr('checked', 'checked');
                $(this).find("#chkIsEnforceDefaultReorderQuantity").attr('disabled', 'disabled');

                $(this).find('#txtDefaultReorderQuantity').val(DefQTY);
                //$(this).find('#txtDefaultReorderQuantity').attr("disabled", "disabled");
            });
        }
        else {
            if (IsBOMItem === 'False') {
                $("#IsEnforceDefaultReorderQuantity").removeAttr('disabled');
            }
            $('#ItemLocationLevelQuanity tbody tr').each(function () {
                $(this).find("#chkIsEnforceDefaultReorderQuantity").removeAttr('disabled');
                $(this).find('#txtDefaultReorderQuantity').removeAttr("disabled");
            });
        }
    });

    $("input#DefaultReorderQuantity").change(function (e) {
        if ($("#IsAllowOrderCostuom").attr('checked') == 'checked') {
            var vCostUOMData = $("#CostUOMData").val();
            var currRQ = $(this).val();
            var vCUOMVal = $('#CostUOMID option:selected').html();

            $("#IsEnforceDefaultReorderQuantity").attr('checked', 'checked');
            var vCostUOMVal = GetCostUOMValue(vCostUOMData, vCUOMVal);

            if (currRQ.length > 0) {
                if (parseInt(vCostUOMVal) > parseInt(currRQ)) {
                    $(this).val(vCostUOMVal);
                }
                else {

                    var actValue = vCostUOMVal;
                    if ((currRQ % vCostUOMVal) == 0) {
                        actValue = currRQ;
                    }
                    else {
                        //actValue = parseInt(currRQ) + (parseInt(vCostUOMVal) - (parseInt(currRQ) % parseInt(vCostUOMVal)));
                    }
                    $(this).val(actValue);
                }
            }
            else {
                $(this).val(vCostUOMVal);
            }
        }
    });

    if (IsRoomELabelEnable) {
        $("#ulSolumnLabelDetails").show();
        BindSolumnLabels();
    } else {
        $("#ulSolumnLabelDetails").hide();
    }
  
});
// DOC.READY END

function GetWeightPerPiece() {

    var _vtotalQTY = $('#totalQTY').val().trim();
    if (_vtotalQTY != undefined && _vtotalQTY != null && _vtotalQTY != '') {
        var itemGuid = ItemGUID;

        $.ajax({
            'url': Inventory_SaveItemWeightPerPieceRequest,
            'data': { 'ItemGuid': ItemGUID, 'TotalQty': _vtotalQTY },
            'type': 'Post',
            'async': false,
            'cache': false,
            'dataType': 'json',
            'success': function (response) {
                if (response.Status === "ok") {
                    $('#DivLoading').hide();
                    $('#totalQTY').val('');
                    showNotificationDialog();
                    $("#spanGlobalMessage").html("Success");
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    if (parseFloat(response.itemWeightPerPiece) > 0) {
                        $("#WeightPerPiece").val(response.itemWeightPerPiece);
                    }
                }
                else {
                    $('#DivLoading').hide();
                    showNotificationDialog();
                    $("#spanGlobalMessage").html(response.Status);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                }
            },
            'error': function (xhr) {
                $('#DivLoading').hide();
            }

        });

    }
    else {
        _notification.showError(MsgEnterWeightPieceTotal);
    }
    return false;
}

function GetCostUOMValue(cstData, cstText) {
    if (cstText != null && cstText != '') {
        var obj = JSON.parse(cstData);
        for (var i = 0; i < obj.length; i++) {
            if (obj[i].Text == cstText) {
                return obj[i].Value;
            }
        }
        return obj[cstText];
    }
    else {
        return 1;
    }
}


$("#frmItemMaster").submit(function (e) {
    //$("#btnSave").attr('disabled', 'disabled');
    RemoveLeadingTrailingSpace("frmItemMaster");
    if ($(this).valid()) {
        rememberUDFValues("ItemMaster", ItemID);//rememberUDFValues($("#hdnPageName").val(), ItemID); //"ItemMaster"
    }
    e.preventDefault();
});


function ShowImage(currentRadio) {
    var currentId = $(currentRadio).attr("id");

    if (currentId == "ImagePath") {
        // $("#ItemImage").show();
        $("#CustomItemImage").show();
        $("#ExternalURL").hide();
        setImagePath();

        $("#btnDeleteImage").hide();
        if ((ItemImagePath != undefined && ItemImagePath != null && ItemImagePath != '') || ($("input#currentpath").val() != '' && $("input#currentpath").val() != '/Content/images/no-image.jpg')) {
            $("#btnDeleteImage").show();
        }

        // $("img#previewHolder").attr('src', '/Content/images/no-image.jpg');
        // $("#ImageExternalURL").val('');
    }
    else {
        CheckValidURLForImage($("input#ItemImageExternalURL"));
        //  $("#SupplierImage").val('');
        $("#ItemImage").hide();
        $("#CustomItemImage").hide();
        $("#ExternalURL").show();
        $("#btnDeleteImage").hide();
        //  $("img#previewHolder").attr('src','/Content/images/no-image.jpg');
    }
}
function DeleteItemImage(ItemGUID) {

    $.ajax({
        url: Inventory_RemoveItemImage,
        data: { 'ItemGUID': ItemGUID },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'ok') {

                $('#previewHolder').attr('src', '/Content/images/no-image.jpg');
                $("input#currentpath").val('/Content/images/no-image.jpg');

                $("input#ItemImage").val('');
                $("#btnDeleteImage").hide();
                ItemImagePath = '';
                var nofilechoosentext = "No file chosen";
                if (typeof textNofilechosen != 'undefined') {
                    nofilechoosentext = textNofilechosen;
                }
                $("#lblnofilechoosen").html(nofilechoosentext);
            }
            else {
                Status = 'Error';
                alert(MsgErrorInProcess);
            }
        }
    });
}

function ShowLink2Image(currentRadio) {
    var currentId = $(currentRadio).attr("id");

    if (currentId == "InternalLink") {
        //$("#Link2").show();
        $("#CustomLink2").show();
        $("#lnkExternalURL").hide();
        setImagePathLink2();

        $("#btnDeleteLink").hide();
        if ((ItemImageLink != undefined && ItemImageLink != null && ItemImageLink != '')
            || ($("input#currentpathLink2").val() != '' && $("input#currentpathLink2").val() != '/Content/images/no-image.jpg')
            || ($("input#Link2").val() != '' && $("input#Link2").val() != '/Content/images/no-image.jpg')) {
            $("#btnDeleteLink").show();
        }

        // $("img#previewHolder").attr('src', '/Content/images/no-image.jpg');
        // $("#ImageExternalURL").val('');
    }
    else {
        CheckValidURLForLink2($("input#ItemLink2ExternalURL"));
        //  $("#SupplierImage").val('');
        $("#Link2").hide();
        $("#CustomLink2").hide();
        $("#lnkExternalURL").show();
        $("#btnDeleteLink").hide();
        //  $("img#previewHolder").attr('src','/Content/images/no-image.jpg');

        //$('#previewHolderLink2').css('display', '');
        setImagePathLink2External();
    }
}

function DeleteItemLink(ItemGUID) {

    $.ajax({
        url: Inventory_RemoveItemLink,
        data: { 'ItemGUID': ItemGUID },
        dataType: 'json',
        type: 'POST',
        async: false,
        cache: false,
        success: function (response) {
            if (response.status == 'ok') {

                $('#previewHolderLink2').attr('href', '/Content/images/no-image.jpg');
                $("input#currentpathLink2").val('/Content/images/no-image.jpg');
                //$('#previewHolderLink2').css('display', 'none');

                $("input#Link2").val('');
                $("#btnDeleteLink").hide();
                ItemImageLink = '';
                var nofilechoosentext = "No file chosen";
                if (typeof textNofilechosen != 'undefined') {
                    nofilechoosentext = textNofilechosen;
                }
                $("#lblnofileLink2").html(nofilechoosentext);
            }
            else {
                Status = 'Error';
                alert(MsgErrorInProcess);
            }
        }
    });
}

function setImagePath() {
    $('#previewHolder').attr('src', $("input#currentpath").val());
}
function setImagePathLink2() {
    $('#previewHolderLink2').attr('href', $("input#currentpathLink2").val());
}
function setImagePathLink2External() {
    var Path = $("input#currentpathLink2External").val()
    if (Path == '') {
        $("input#currentpathLink2External").val('/Content/images/no-image.jpg');
    }
    $('#previewHolderLink2').attr('href', $("input#currentpathLink2External").val());
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
        $("#spanGlobalMessage").html(MsgTransferORPurchaseRequired);
        $('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
            $("#spanGlobalMessage").html(OnedefaultSupplierisRequired);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }

        if (iCountSupPartNo > 0) {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(RequiredSupplierNumber);
            $('div#target').fadeToggle();
            // $("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }



        var TempLocationrows1 = $("#ItemLocationLevelQuanity").dataTable().fnGetNodes();
        var iCountLocationDefault = 0;
        var iCountLocationPartNo = 0;
        if (TempLocationrows1 != null && TempLocationrows1.length > 0) {
            for (var i = 0; i < TempLocationrows1.length; i++) {
                if (!($(TempLocationrows1[i]).find('#txtLocation').val() == '')) {
                    if ($(TempLocationrows1[i]).find("#IsDefault").is(":checked")) {
                        iCountLocationDefault += 1;
                    }
                }
            }
        }
        else {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgAtleastOneLocationRequired);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }

        if (!(iCountLocationDefault == 1)) {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgPleaseSelectDefaultLocation);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                $("#spanGlobalMessage").html(MsgAtleastOneKitRequired);
                $('div#target').fadeToggle();
                //  $("div#target").delay(2000).fadeOut(200);
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                return false;
            } else {
                for (var i = 0; i < Temprowskit.length; i++) {
                    if ($(Temprowskit[i]).find("#txtQuantityPerKit") != undefined && $(Temprowskit[i]).find("#txtQuantityPerKit").val() == '') {
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(QuantityPerKitRequired);
                        $('div#target').fadeToggle();
                        //  $("div#target").delay(2000).fadeOut(200);
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        return false;
                    }
                }

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
    if ($("input#ExtendedCost").val() != '') {

        $("input#ExtendedCost").val(FormatedCostQtyValues(parseFloat($("input#ExtendedCost").val()), 1));
    }
    if ($("input#AverageCost").val() != '') {
        $("input#AverageCost").val(FormatedCostQtyValues(parseFloat($("input#AverageCost").val()), 1));
    }

    if ($("input#PerItemCost").val() != '') {
        $("input#PerItemCost").val(FormatedCostQtyValues(parseFloat($("input#PerItemCost").val()), 1));
    }

    var AllValilLabel = true;
    $('#SolumLabelList tbody tr').each(function () {
        if ($(this).find("#txtLabelCode").length > 0 && $(this).find("#txtLabelCode").attr('ActiveError') == 'true') {
            AllValilLabel = false;
        }
    });
    if (!AllValilLabel) {
        return false;
    } 

    return true;
}

function onBegin(xhr) {
    $("#btnSave").attr('disabled', 'disabled');

    $("#frmItemMaster").find("input[type='hidden']").removeProp("disabled")
    //xhr.setRequestHeader("__RequestVerificationToken", $("input[name='__RequestVerificationToken'][type='hidden']").val())
    var isValid = CheckBeforeSave();

    if (!isValid) {
        $("#btnSave").removeAttr('disabled');
    }

    return isValid;
}

function onSuccess(response) {
    //   $("input").addAttr("disabled");
    //var $spanGlobalMessage = $("#spanGlobalMessage");
    var msg = '';
    //$('div#target').fadeToggle();
    //$("div#target").delay(DelayTime).fadeOut(FadeOutTime);
    msg = response.Message; //$spanGlobalMessage.text(response.Message);
    if (typeof response.ErrorMessage != 'undefined' && response.ErrorMessage != '') {
        msg = response.ErrorMessage;  //$spanGlobalMessage.text(response.ErrorMessage);
    }
    //$spanGlobalMessage.removeClass('errorIcon WarningIcon').addClass('succesIcon');
    $("#btnSave").removeAttr('disabled');
    var idValue = $("#dvhdns").find("input[type='hidden'][id='hiddenID']").val();
    if (response.Status == "fail") {

        //$spanGlobalMessage.removeClass('succesIcon WarningIcon').addClass('errorIcon');
        _notification.showError(msg);
        if (response.calcDefaultReorderQuantity != null) {
            $("#DefaultReorderQuantity").val(response.calcDefaultReorderQuantity);
            $("#IsEnforceDefaultReorderQuantity").attr("checked", "checked");
            $("#DefaultReorderQuantity").focus();
        }
        else {
            $("#ItemNumber").val("");
            $("#ItemNumber").focus();
            IsRefreshGrid = true;
        }
    }
    else if (response.Status == "Fa") {
        //$spanGlobalMessage.removeClass('succesIcon errorIcon').addClass('WarningIcon');
        _notification.showWarning(msg);
        $("#ItemNumber").focus();
        IsRefreshGrid = true;
    }
    else if (idValue == 0) {

        IsRefreshGrid = true;
        $("#ItemNumber").val("");
        $("#ItemNumber").focus();
        //clearControls('frmItemMaster');
        if (response.Status == "duplicate") {
            //$spanGlobalMessage.removeClass('errorIcon succesIcon').addClass('WarningIcon');
            _notification.showWarning(msg);
        }
        else {
            _notification.showSuccess(msg);
            SetItemCountInMenu();
            if ($("#Link2").val() != "") {
                ajaxFileUpload(response.ItemID);
            }
            _NarrowSearchSave.processSearchReload();
            if ($("#ItemImage").val() != "") {
                if (ajaxFileUpload1(response, idValue)) {

                }
                else {
                    $('#DivLoading').show();
                    CallHistoryDialog(response);
                    CallGridFunction(response, idValue);
                }
            }
            else {
                $('#DivLoading').show();
                CallHistoryDialog(response);
                CallGridFunction(response, idValue);
            }
            //ShowNewTab('ItemCreate', 'frmItemMaster');
        }
    }
    else if (idValue > 0) {
        if (response.Status == "duplicate") {
            _notification.showWarning(msg); //$spanGlobalMessage.removeClass('errorIcon succesIcon').addClass('WarningIcon');
            $("#ItemNumber").val("");
            $("#ItemNumber").focus();
        }
        else {
            _notification.showSuccess(msg);
            SetItemCountInMenu();
            _NarrowSearchSave.processSearchReload();
            if ($("#Link2").val() != "") {
                ajaxFileUpload(response.ItemID);
            }
            if ($("#ItemImage").val() != "") {
                if (ajaxFileUpload1(response, idValue)) {

                }
                else {
                    $('#DivLoading').show();
                    CallHistoryDialog(response);
                    CallGridFunction(response, idValue);
                }
            }
            else {
                $('#DivLoading').show();
                CallHistoryDialog(response);
                CallGridFunction(response, idValue);
            }
            //if (oTable !== undefined && oTable != null) {
            //    oTable.fnStandingRedraw();
            //}
            // IsRefreshGrid = true;
            //var _IsArchived12 = false;
            //var _IsDeleted12 = false;
            //if (typeof ($('#IsArchivedRecords')) != undefined)
            //    _IsArchived12 = $('#IsArchivedRecords').is(':checked');
            //if (typeof ($('#IsDeletedRecords')) != undefined)
            //    _IsDeleted12 = $('#IsDeletedRecords').is(':checked');
            //GetPullNarrowSearchData('ItemMaster', _IsArchived12, _IsDeleted12);
            //CallNarrowfunctions();

            //var QueryStringParam2 = ItemGUID;

            //// alert(QueryStringParam2);
            ////ShowEditTabGUID("ItemEdit?ItemGUID=" + QueryStringParam2, "frmItem");
            //$("#tab5").click();

            //clearControls('frmItemMaster');
            //SwitchTextTab(0, 'ItemCreate', 'frmItemMaster');

        }
    }
}


function CallHistoryDialog(itemResponse) {
    $.ajax({
        url: BinChangeHistoryURL,
        type: 'GET',
        data: { 'ItemGUID': itemResponse.ItemDTO.GUID },
        timeout: 0,
        success: function (responseHistory) {

            var historyString = "";
            if (responseHistory.errorMessage == "") {
                if (responseHistory.isRecordAvail == true) {
                    $.each(responseHistory.historyData, function (i, val) {
                        historyString += WarnNewBinCreatedRes.replace("{0}", val.ItemNumber).replace("{1}", "<b>" + val.BinNumber + "</b>");
                    });

                    $('#HistoryInfoDialog').find("#HistoryMSG").html(historyString);
                    closeHistoryInfoModel();
                    $('#HistoryInfoDialog').modal();
                }
            }
            else {
                $('#HistoryInfoDialog').find("#HistoryMSG").html(MsgBinHistoryErrorPartial + responseHistory.errorMessage);
                closeHistoryInfoModel();
                $('#HistoryInfoDialog').modal();
            }
        }
    });
}

function closeHistoryInfoModel() {
    $.modal.impl.close();
}

function closeFromOk() {
    $.modal.impl.close();
}

function CallGridFunction(response, Id) {
    if (Id == 0) {
        clearControls('frmItemMaster');
        setDefaultUDFValues($("#hdnPageName").val(), ItemID);
        if (response.DestinationModule == "OrderItemPopup") {
            preSearchItemText = response.ItemDTO.ItemNumber;
            //OpenItemPopup();
            $("#divAddNewItemDailog").dialog("close");
            return;
        }
    }
    var _IsArchived12 = false;
    var _IsDeleted12 = false;
    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived12 = $('#IsArchivedRecords').is(':checked');
    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted12 = $('#IsDeletedRecords').is(':checked');

    var page = $("#hdnPageName").val();
    if (page == '' || page == null)
        page = 'ItemMaster';
    //GetPullNarrowSearchData('ItemMaster', _IsArchived12, _IsDeleted12);
    GetPullNarrowSearchData(page, _IsArchived12, _IsDeleted12);
    CallNarrowfunctions();
    if (Id == 0) {
        $('#chkLotNumberTracking').attr("disabled", false);
        $('#chkSerialNumberTracking').attr("disabled", false);
        $('#chkDateCodeTracking').attr("disabled", false);
        if (response.DestinationModule != null && response.DestinationModule != "") {
            if (response.DestinationModule == "CartItemMaster") {
                $("#divSupplierCatalogItems").dialog("close");
                SwitchTextTab(0, '', '');
                //if (oTable !== undefined && oTable != null) {
                //    oTable.fnDraw();
                //}
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
    }
    if (oTable !== undefined && oTable != null) {
        oTable.fnDraw();
    }
    $('#DivLoading').hide();
    $("#tab5").click();
}
function onFailure(message) {
    $("#btnSave").removeAttr('disabled');
    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    $("#spanGlobalMessage").html(message.statusText);
    $('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
    $("#ItemNumber").focus();
}

function ajaxFileUpload1(Response, idValue) {
    //starting setting some animation when the ajax starts and completes
    $("#loading")
        .ajaxStart(function () {
            $(this).show();
        })
        .ajaxComplete(function () {
            $(this).hide();
        });
    $('#DivLoading').show();
    $.ajaxFileUpload
        (
            {
                url: '/api/fileupload/PostItemFile/' + Response.ItemID,
                secureuri: false,
                type: "POST",
                fileElementId: 'ItemImage',
                dataType: 'json',
                async: false,
                success: function (data, status) {

                    CallGridFunction(Response, idValue);
                    //  window.location.reload();
                    return true;
                },
                error: function (data, status, e) {

                    CallGridFunction(Response, idValue);
                    //    window.location.reload();
                    return false;
                }
            }
        )
    //return false;
    return true;
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
                async: false,
                success: function (data, status) {
                    //  window.location.reload();
                    return false;
                },
                error: function (data, status, e) {
                    return false;
                }
            }
        )
    // return false;
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
    if (IsAutoInventoryClassification || IsBOMItem == 'True')
        $('#drpInventoryClassification').attr("disabled", true);
    else
        $('#drpInventoryClassification').attr("disabled", false);
}

function DisableSerialLotExpiry() {
    $('#chkLotNumberTracking').attr("disabled", true);
    $('#chkSerialNumberTracking').attr("disabled", true);
    //$('#chkDateCodeTracking').attr("disabled", true);
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
    if (charCode > 31 && (charCode < 48 || charCode > 57 || code == 86))
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
            $("input#ItemImage").val('');
            $("#btnDeleteImage").hide();
            EnterpriseImagePath = '';
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
            //alert("Please select correct file name.");
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgSelectCorrectFileName);
            $('div#target').fadeToggle();
            //$("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

            $("input#currentpath").val('');
            $("input#ItemImage").val('');
        }
        else {
            var reader = new FileReader();
            reader.onload = function (e) {

                var filePath = $("#currentpath").val().split('\\').pop();
                $("#lblnofilechoosen").html(objFile.name);
                if (filePath.toString().indexOf("&") >= 0
                    || filePath.toString().indexOf("<") >= 0
                    || filePath.toString().indexOf(">") >= 0
                    || filePath.toString().indexOf("*") >= 0
                    || filePath.toString().indexOf(":") >= 0
                    || filePath.toString().indexOf("?") >= 0
                    || filePath.toString().indexOf("%") >= 0) {
                    //alert("Please select correct file name.");
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSelectCorrectFileName);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(5000).fadeOut(200);
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
function readURLLink2(input) {

    if (input.files && input.files[0]) {

        var isError = false;
        var objFile = input.files[0];

        for (var n = 0; n < specialKeys.length; n++) {
            if (objFile.name.toString().lastIndexOf(String.fromCharCode(specialKeys[n])) >= 0) {
                isError = true;
                break;
            }
        }

        if (isError == true) {
            //alert("Please select correct file name.");
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgSelectCorrectFileName);
            $('div#target').fadeToggle();
            //$("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

            $("input#currentpathLink2").val('');
            $("input#Link2").val('');
        }
        else {

            var validExtension = Link2AllowedFileExtension.split(',');
            var strValidationMessage = "";
            var fileExt = input.files[0].name;
            fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
            if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
                strValidationMessage = strValidationMessage + input.files[0].name + " " + MsgInvalidFileSelected;
            }
            if (strValidationMessage != "") {
                alert(strValidationMessage + MsgvalidFileList.replace("{0}", validExtension.toString()));
                $('#previewHolderLink2').attr('src', '/Content/images/no-image.jpg');
                $("input#currentpathLink2").val('/Content/images/no-image.jpg');
                $("input#Link2").val('');
                $("#btnDeleteLink").hide();
                EnterpriseImagePath = '';
                return;
            }

            var reader = new FileReader();
            reader.onload = function (e) {
                var filePath = $("#currentpathLink2").val().split('\\').pop();
                $("#lblnofileLink2").html(objFile.name);
                if (filePath.toString().indexOf("&") >= 0 || filePath.toString().indexOf("<") >= 0 || filePath.toString().indexOf(">") >= 0
                    || filePath.toString().indexOf("*") >= 0 || filePath.toString().indexOf(":") >= 0
                    || filePath.toString().indexOf("?") >= 0) {
                    //alert("Please select correct file name.");
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSelectCorrectFileName);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(5000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

                    //$("input#currentpathLink2").val('');
                }
                else {
                    $("#previewHolderLink2").attr('href', e.target.result);
                    // $('#previewHolderLink2').attr('src', e.target.result);
                    // $("input#currentpathLink2").val(e.target.result);
                    $("#btnDeleteLink").show();
                }
            }

            reader.readAsDataURL(input.files[0]);
        }
    }
}

function CheckValidURLForImage(curobj) {
    var strURL = $(curobj).val();
    if (strURL != '' && strURL != null) {
        var validExtension = CommonFileExtension.split(',');
        var strValidationMessage = "";
        var fileExt = strURL.substring(strURL.lastIndexOf('.'));
        if (fileExt.indexOf("/") <= 0) {
            if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
                var result = fileExt.substring(fileExt.indexOf('?'));
                fileExt = fileExt.replace(result, '');
            }
            if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
                strValidationMessage = strValidationMessage + strURL + " " + MsgInvalidFileSelected;
            }
            if (strValidationMessage != "") {
                alert(strValidationMessage + MsgvalidFileList.replace("{0}", validExtension.toString()));
                $('#previewHolder').attr('href', '/Content/images/no-image.jpg');
                $('#previewHolder').attr('src', '/Content/images/no-image.jpg');
                $("input#ItemImageExternalURL").val('');
                return;
            }
        }

        $("<img>", {
            src: strURL,
            error: function () {
                //alert('Invalid URL. please enter valid URL');
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(MsgInvalidURL);
                $('div#target').fadeToggle();
                //$("div#target").delay(5000).fadeOut(200);
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

                $(curobj).val("");
                $("input#ItemImageExternalURL").val('');

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

function CheckValidURLForLink2(curobj) {
    var strURL = $(curobj).val();

    if (strURL != '' && strURL != null) {

        var validExtension = CommonFileExtension.split(',');
        var strValidationMessage = "";
        var fileExt = strURL.substring(strURL.lastIndexOf('.'));

        if (fileExt.indexOf("/") <= 0) {
            if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
                var result = fileExt.substring(fileExt.indexOf('?'));
                fileExt = fileExt.replace(result, '');
            }
            if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
                strValidationMessage = strValidationMessage + strURL + " " + MsgInvalidFileSelected;
            }
            if (strValidationMessage != "") {
                alert(strValidationMessage + MsgvalidFileList.replace("{0}", validExtension.toString()));
                $('#previewHolderLink2').attr('href', '/Content/images/no-image.jpg');
                $("input#ItemLink2ExternalURL").val('');
                return;
            }
        }

        $("<img>", {
            src: strURL,
            error: function () {
                //alert('Invalid URL. please enter valid URL');
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(MsgInvalidURlForLink);
                $('div#target').fadeToggle();
                //$("div#target").delay(5000).fadeOut(200);
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

                $(curobj).val("");
                $("input#ItemLink2ExternalURL").val('');
                $('#previewHolderLink2').attr('href', '/Content/images/no-image.jpg');

                curobj.focus();

            },
            load: function () {

                $('#previewHolderLink2').attr('href', strURL);

            }
        });
    }
    else {
        $('#previewHolderLink2').attr('src', '/Content/images/no-image.jpg');
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
                //$(this).find('#txtCustomerOwnedQuantity').addClass("disableBack");
                //$(this).find('#txtConsignedQuantity').addClass("disableBack");
                $(this).find('#txteVMISensorPort').addClass("disableBack");
                $(this).find('#txteVMISensorID').addClass("disableBack");
                $(this).find('#IsDefault').addClass("disableBack");


                $(this).find('#txtCriticalQuantity').attr("disabled", "disabled");
                $(this).find('#txtMinimumQuantity').attr("disabled", "disabled");
                $(this).find('#txtMaximumQuantity').attr("disabled", "disabled");
                //$(this).find('#txtCustomerOwnedQuantity').attr("disabled", "disabled");
                //$(this).find('#txtConsignedQuantity').attr("disabled", "disabled");
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
    $('#ulQuantity').find('li#liDefaultPullQuantity').removeAttr('disabled');
    $('#ulQuantity').find('li#liDefaultPullQuantity').find("input#DefaultPullQuantity").removeAttr('disabled');

    if (ViewBag_LockReplenishmentType == 'True') {
        if ($('#IsItemLevelMinMaxQtyRequired').attr('type') != 'hidden')
            $('#IsItemLevelMinMaxQtyRequired').attr('disabled', true);
        else
            $('#IsItemLevelMinMaxQtyRequired').removeAttr('disabled');
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
    if ($('#ItemModelPS').dialog('isOpen')) {
        $('#ItemModelPS').dialog('close');
        //$("[id^='ItemModelPS']").dialog('close');
    }

    var strUrl = Inventory_LoadItemKitModel;
    strUrl = strUrl + '?Parentid=1';
    strUrl = strUrl + '&ParentGuid=' + ItemGUID;
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
            if (Status != 'Error') {
                BindKitDetails(); //console 4
            }
        }
    }
    return true;
}

function SavetoSeesionItemKitComponent(obj) {
    //
    //var vtxtQuantityPerKit = $(obj).parent().parent().find('#txtQuantityPerKit').val();
    //var vitemQuantityPerKit = $(obj).parent().parent().find('#item_QuantityPerKit').val();
    ////  alert(vitemQuantityPerKit);
    //var vItemguid = $(obj).parent().parent().find('#hdnItemGUID').val();
    //var vKitGUID = $(obj).parent().parent().find('#hdnKitGUID').val();
    //var vhdnID = $(obj).parent().parent().find('#hdnID').val();
    //var vhdnGUID = $(obj).parent().parent().find('#hdnGUID').val();
    //var vhdnSessionSr = $(obj).parent().parent().find('#hdnSessionSr').val();

    var vtxtQuantityPerKit = $(obj).find('#txtQuantityPerKit').val();
    var vitemQuantityPerKit = $(obj).find('#item_QuantityPerKit').val();
    var vItemguid = $(obj).find('#hdnItemGUID').val();
    var vKitGUID = $(obj).find('#hdnKitGUID').val();
    var vhdnID = $(obj).find('#hdnID').val();
    var vhdnGUID = $(obj).find('#hdnGUID').val();
    var vhdnSessionSr = $(obj).find('#hdnSessionSr').val();
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
                //  $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                //$("#spanGlobalMessage").text('Kit Component Added.');
                // $('div#target').fadeToggle();
                //  $("div#target").delay(2000).fadeOut(200);
            }
            else {
                Status = 'Error';
                alert(MsgErrorInProcess);
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
    if (vtxtQuantityPerKit == "" || vtxtQuantityPerKit == "''") {
        vtxtQuantityPerKit = 0;
    }

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
                $("#spanGlobalMessage").html(MsgKitDeleteValidation);
                $('div#target').fadeToggle();
                //$("div#target").delay(2000).fadeOut(200);
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            }
            else if (response.status == 'deleted') {

                BindKitDetails();//console 5
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").html(MsgRecordDeletedSuccessfully.replace("{0}", ""));
                $('div#target').fadeToggle();
                //$("div#target").delay(2000).fadeOut(200);
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

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
        DisableLocationTextbox(obj, false);
        disableCustomerConsignedQtyOnAddLocation();
        updateOnHandQuantity();
        SetCostUOMvalueForBinGrid();
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
            DisableLocationTextbox(checkboxchk, false);
            disableCustomerConsignedQtyOnAddLocation();
            SetCostUOMvalueForBinGrid();
            $('#DivLoading').hide();

        });
    }
    return false;
}

function DisableLocationTextbox(obj, isCustConsDisableNotRequire) {

    var anSelectedLocations;
    if (typeof (oTableProjectItems) != "undefined" && oTableProjectItems != null)
        anSelectedLocations = oTableProjectItems.$('tr');
    else
        anSelectedLocations = null;

    if (anSelectedLocations != null && typeof (anSelectedLocations) != "undefined" && anSelectedLocations.length != 0) {

        for (var i = 0; i <= anSelectedLocations.length - 1; i++) {
            if (obj == true || obj == 'True') {
                $(anSelectedLocations[i]).find('#txtCriticalQuantity,#txtMinimumQuantity,#txtMaximumQuantity').addClass("disableBack").val('N/A').attr('readonly', true).attr("disabled", "disabled");
            }
            else {
                $(anSelectedLocations[i]).find('#txtCriticalQuantity,#txtMinimumQuantity,#txtMaximumQuantity').removeClass("disableBack").attr('readonly', false).removeAttr("disabled");

                if ($(anSelectedLocations[i]).find('#txtCriticalQuantity').val() <= 0)
                    $(anSelectedLocations[i]).find('#txtCriticalQuantity').val(FormatedCostQtyValues(0, 2));
                if ($(anSelectedLocations[i]).find('#txtMinimumQuantity').val() <= 0)
                    $(anSelectedLocations[i]).find('#txtMinimumQuantity').val(FormatedCostQtyValues(0, 2));
                if ($(anSelectedLocations[i]).find('#txtMaximumQuantity').val() <= 0)
                    $(anSelectedLocations[i]).find('#txtMaximumQuantity').val(FormatedCostQtyValues(0, 2));
            }
        }
    }
    else {
        $('#ItemLocationLevelQuanity tbody tr').each(function () {
            if (obj == true || obj == 'True') {
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
    }

    return true;
}

function SavetoSeesionBinReplanish(obj1) {
    //
    // var vBinID = $(obj).parent().parent().find('#dlLocation')[0].value == '' ? 0 : $(obj).parent().parent().find('#dlLocation')[0].value;
    var vBinID = $(obj1).find('#SubBinID').val();
    var vtxtBinLocation = $(obj1).find('#txtLocation').val();
    var vCritical = ($(obj1).find('#txtCriticalQuantity').val() == 'N/A' || $(obj1).find('#txtCriticalQuantity').val() == '') ? 0.0 : $(obj1).find('#txtCriticalQuantity').val();
    var vMinimum = ($(obj1).find('#txtMinimumQuantity').val() == 'N/A' || $(obj1).find('#txtMinimumQuantity').val() == '') ? 0.0 : $(obj1).find('#txtMinimumQuantity').val();
    var vMaximum = ($(obj1).find('#txtMaximumQuantity').val() == 'N/A' || $(obj1).find('#txtMaximumQuantity').val() == '') ? 0.0 : $(obj1).find('#txtMaximumQuantity').val();

    var vCustOwnedQty = ($(obj1).find('#txtCustomerOwnedQuantity').val() == 'N/A' || $(obj1).find('#txtCustomerOwnedQuantity').val() == '') ? 0.0 : $(obj1).find('#txtCustomerOwnedQuantity').val();
    var vConsQty = ($(obj1).find('#txtConsignedQuantity').val() == 'N/A' || $(obj1).find('#txtConsignedQuantity').val() == '') ? 0.0 : $(obj1).find('#txtConsignedQuantity').val();

    var vItemguid = $(obj1).find('#hdnItemGUID').val();
    if (vItemguid == "") {
        vItemguid = ItemGUID;
    }
    var vhdnID = $(obj1).find('#hdnID').val();
    var vhdnGUID = $(obj1).find('#hdnGUID').val();
    var vhdnSessionSr = $(obj1).find('#hdnSessionSr').val();
    var vlocIsDefault = $(obj1).find('#IsDefault').is(':checked');
    var veVMISensorPort = $(obj1).find('#dlComPort').val();
    var veVMISensorID = $(obj1).find('#txteVMISensorID').val() == '' ? 0.0 : $(obj1).find('#txteVMISensorID').val();

    var vlocIsEDPQ = $(obj1).find('#chkIsEnforceDefaultPullQuantity').is(':checked');
    var vlocIsEDRQ = $(obj1).find('#chkIsEnforceDefaultReorderQuantity').is(':checked');
    var vDPQty = ($(obj1).find('#txtDefaultPullQuantity').val() == 'N/A' || $(obj1).find('#txtDefaultPullQuantity').val() == '') ? 0.0 : $(obj1).find('#txtDefaultPullQuantity').val();
    var vDRQty = ($(obj1).find('#txtDefaultReorderQuantity').val() == 'N/A' || $(obj1).find('#txtDefaultReorderQuantity').val() == '') ? 0.0 : $(obj1).find('#txtDefaultReorderQuantity').val();

    if ($(obj1).find('#UDF1Bin') != null) {
        var UDF1Bin = $(obj1).find('#UDF1Bin');
        if ($(UDF1Bin).attr("class") == 'selectBox' || $(UDF1Bin).attr("class") == 'selectBox valid') {
            vUDF1Bin = $(UDF1Bin).children("option:selected").text();
        }
        else {
            vUDF1Bin = $(UDF1Bin).val();
        }
        var vBinUDF1Required = $(UDF1Bin).attr("udfrequired");
        if (vBinUDF1Required == "true" && vUDF1Bin == "") {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(ReqBinUDF1);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $(UDF1Bin).focus();
            return false;
        }
    }

    if ($(obj1).find('#UDF2Bin') != null) {
        var UDF2Bin = $(obj1).find('#UDF2Bin');
        if ($(UDF2Bin).attr("class") == 'selectBox' || $(UDF2Bin).attr("class") == 'selectBox valid') {
            vUDF2Bin = $(UDF2Bin).children("option:selected").text();
        }
        else {
            vUDF2Bin = $(UDF2Bin).val();
        }
        var vBinUDF2Required = $(UDF2Bin).attr("udfrequired");
        if (vBinUDF2Required == "true" && vUDF2Bin == "") {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(ReqBinUDF2);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $(UDF2Bin).focus();
            return false;
        }
    }
    if ($(obj1).find('#UDF3Bin') != null) {
        var UDF3Bin = $(obj1).find('#UDF3Bin');
        if ($(UDF3Bin).attr("class") == 'selectBox' || $(UDF3Bin).attr("class") == 'selectBox valid') {
            vUDF3Bin = $(UDF3Bin).children("option:selected").text();
        }
        else {
            vUDF3Bin = $(UDF3Bin).val();
        }
        var vBinUDF3Required = $(UDF3Bin).attr("udfrequired");
        if (vBinUDF3Required == "true" && vUDF3Bin == "") {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(ReqBinUDF3);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $(UDF3Bin).focus();
            return false;
        }
    }
    if ($(obj1).find('#UDF4Bin') != null) {
        var UDF4Bin = $(obj1).find('#UDF4Bin');
        if ($(UDF4Bin).attr("class") == 'selectBox' || $(UDF4Bin).attr("class") == 'selectBox valid') {
            vUDF4Bin = $(UDF4Bin).children("option:selected").text();
        }
        else {
            vUDF4Bin = $(UDF4Bin).val();
        }
        var vBinUDF4Required = $(UDF4Bin).attr("udfrequired");
        if (vBinUDF4Required == "true" && vUDF4Bin == "") {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(ReqBinUDF4);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $(UDF4Bin).focus();
            return false;
        }
    }
    if ($(obj1).find('#UDF5Bin') != null) {
        var UDF5Bin = $(obj1).find('#UDF5Bin');
        if ($(UDF5Bin).attr("class") == 'selectBox' || $(UDF5Bin).attr("class") == 'selectBox valid') {
            vUDF5Bin = $(UDF5Bin).children("option:selected").text();
        }
        else {
            vUDF5Bin = $(UDF5Bin).val();
        }
        var vBinUDF5Required = $(UDF5Bin).attr("udfrequired");
        if (vBinUDF5Required == "true" && vUDF5Bin == "") {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(ReqBinUDF5);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $(UDF5Bin).focus();
            return false;
        }
    }

    if (vBinID == 0 && vtxtBinLocation == '') {

        return true;
    }




    if (vtxtBinLocation == '') {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgReqInventoryLocation);
        $('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $(obj1).find('#txtLocation').focus();
        return false;
    }

    var TempSuprows = $("#ItemLocationLevelQuanity").dataTable().fnGetNodes();
    var iCountSup = 0;
    if (TempSuprows != null && TempSuprows.length > 0) {
        for (var i = 0; i < TempSuprows.length; i++) {
            if ($(TempSuprows[i]).find('#txtLocation').val() == vtxtBinLocation) {
                iCountSup += 1;
                if (iCountSup > 1) {
                    $(TempSuprows[i]).css("background-color", "red");
                    $(TempSuprows[i].cells[1].getElementsByTagName('input')).css("background-color", "yellow");
                    $(TempSuprows[i].cells[1].getElementsByTagName('input').txtLocation).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgInventoryLocationAlreadyAdded);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    return false;
                }
            }
        }
    }


    if (vlocIsEDPQ == true && parseFloat(vDPQty) <= 0) {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgPullQuantityValidation);
        $('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

        $(obj1).css("background-color", "red");
        $(obj1).find('#txtDefaultPullQuantity').css("background-color", "yellow");
        $(obj1).find('#txtDefaultPullQuantity').focus();
        return false;
    }

    if (vlocIsEDRQ == true && parseFloat(vDRQty) <= 0) {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgReorderQuantityValidation);
        $('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

        $(obj1).css("background-color", "red");
        $(obj1).find('#txtDefaultReorderQuantity').css("background-color", "yellow");
        $(obj1).find('#txtDefaultReorderQuantity').focus();
        return false;
    }

    if (vtxtBinLocation == '') {
        //alert("Please select Inventory location.");
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgSelectInventoryLocation);
        $('div#target').fadeToggle();
        //$("div#target").delay(5000).fadeOut(200);
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

        $(obj1).find('#txtLocation').focus();
        return false;
    }
    if ($(obj1).find('#txtCriticalQuantity').val() != 'N/A') {
        if (vCritical == '' && vCritical != '0.0') {
            //alert("Please select Critical Quantity.");
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgSelectCriticalQuantity);
            $('div#target').fadeToggle();
            //$("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

            $(obj1).find('#txtCriticalQuantity').focus();
            return false;
        }

        if (vMinimum == '' && vMinimum != '0.0') {
            //alert("Please select Minimum Quantity.");
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgSelectMinimumQuantity);
            $('div#target').fadeToggle();
            //$("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

            $(obj1).find('#txtMinimumQuantity').focus();
            return false;
        }

        if (vMaximum == '' && vMaximum != '0.0') {
            //alert("Please select Maximum Quantity.");
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgSelectMaximumQuantity);
            $('div#target').fadeToggle();
            //$("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

            $(obj1).find('#txtMaximumQuantity').focus();
            return false;
        }

        if (parseFloat(vCritical) > parseFloat(vMinimum)) {
            //alert('Critical quantity must be less then Minimum quantity');
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgCriticalMinimumQuantityValidation);
            $('div#target').fadeToggle();
            // $("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

            $(obj1).find('#txtCriticalQuantity').focus();
            return false;
        }

        if (parseFloat(vMinimum) > parseFloat(vMaximum)) {
            //alert('Minimum quantity must be less then Maximum quantity');
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgMinimumMaximumQuantityValidation);
            $('div#target').fadeToggle();
            //$("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

            $(obj1).find('#txtMaximumQuantity').focus();
            return false;
        }
    }
    //
    $.ajax({
        url: Inventory_SavetoSeesionBinReplanishSingle,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'BinID': vBinID, 'BinLocation': vtxtBinLocation, 'CriticalQuanity': vCritical, 'MinimumQuantity': vMinimum, 'MaximumQuantity': vMaximum, 'IsDefault': vlocIsDefault, 'eVMISensorPort': veVMISensorPort, 'eVMISensorID': veVMISensorID, 'customerOwnedQuantity': vCustOwnedQty, 'consignedQuantity': vConsQty, 'isEnforceDefaultPullQuantity': vlocIsEDPQ, 'defaultPullQuantity': vDPQty, 'isEnforceDefaultReorderQuantity': vlocIsEDRQ, 'defaultReorderQuantity': vDRQty, 'UDF1Bin': vUDF1Bin, 'UDF2Bin': vUDF2Bin, 'UDF3Bin': vUDF3Bin, 'UDF4Bin': vUDF4Bin, 'UDF5Bin': vUDF5Bin },
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
                alert(MsgErrorInProcess);
            }
        }
    });
    return true;
}

function SavetoSeesionBinReplanishNew(obj1, guid) {
    // var vBinID = $(obj).parent().parent().find('#dlLocation')[0].value == '' ? 0 : $(obj).parent().parent().find('#dlLocation')[0].value;
    var vBinID = $(obj1).find('#SubBinID').val();
    var vtxtBinLocation = $(obj1).find('#txtLocation').val();
    var vCritical = ($(obj1).find('#txtCriticalQuantity').val() == 'N/A' || $(obj1).find('#txtCriticalQuantity').val() == '') ? 0.0 : $(obj1).find('#txtCriticalQuantity').val();
    var vMinimum = ($(obj1).find('#txtMinimumQuantity').val() == 'N/A' || $(obj1).find('#txtMinimumQuantity').val() == '') ? 0.0 : $(obj1).find('#txtMinimumQuantity').val();
    var vMaximum = ($(obj1).find('#txtMaximumQuantity').val() == 'N/A' || $(obj1).find('#txtMaximumQuantity').val() == '') ? 0.0 : $(obj1).find('#txtMaximumQuantity').val();
    var vCustOwnedQty = ($(obj1).find('#txtCustomerOwnedQuantity').val() == 'N/A' || $(obj1).find('#txtCustomerOwnedQuantity').val() == '') ? 0.0 : $(obj1).find('#txtCustomerOwnedQuantity').val();
    var vConsQty = ($(obj1).find('#txtConsignedQuantity').val() == 'N/A' || $(obj1).find('#txtConsignedQuantity').val() == '') ? 0.0 : $(obj1).find('#txtConsignedQuantity').val();

    var vItemguid = $(obj1).find('#hdnItemGUID').val();
    if (vItemguid == "") {
        vItemguid = ItemGUID;
    }
    var vhdnID = $(obj1).find('#hdnID').val();
    var vhdnGUID = $(obj1).find('#hdnGUID').val();
    var vhdnSessionSr = $(obj1).find('#hdnSessionSr').val();
    var vlocIsDefault = $(obj1).find('#IsDefault').is(':checked');
    var veVMISensorPort = $(obj1).find('#dlComPort').val();
    var veVMISensorID = $(obj1).find('#txteVMISensorID').val() == '' ? 0.0 : $(obj1).find('#txteVMISensorID').val();

    var vlocIsEDPQ = $(obj1).find('#chkIsEnforceDefaultPullQuantity').is(':checked');
    var vlocIsEDRQ = $(obj1).find('#chkIsEnforceDefaultReorderQuantity').is(':checked');
    var vDPQty = ($(obj1).find('#txtDefaultPullQuantity').val() == 'N/A' || $(obj1).find('#txtDefaultPullQuantity').val() == '') ? 0.0 : $(obj1).find('#txtDefaultPullQuantity').val();
    var vDRQty = ($(obj1).find('#txtDefaultReorderQuantity').val() == 'N/A' || $(obj1).find('#txtDefaultReorderQuantity').val() == '') ? 0.0 : $(obj1).find('#txtDefaultReorderQuantity').val();


    if (vBinID == 0 && vtxtBinLocation == '') {

        return true;
    }

    //if (vtxtBinLocation == '') {
    //    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //    $("#spanGlobalMessage").text('Inventory Location is required.');
    //    $('div#target').fadeToggle();
    //    $("div#target").delay(2000).fadeOut(200);
    //    $(obj1).find('#txtLocation').focus();
    //    return false;
    //}
    var TempSuprows = $("#ItemLocationLevelQuanity").dataTable().fnGetNodes();
    var iCountSup = 0;
    //if (TempSuprows != null && TempSuprows.length > 0) {
    //    for (var i = 0; i < TempSuprows.length; i++) {
    //        if (TempSuprows[i].cells[1].getElementsByTagName('input').txtLocation.value == vtxtBinLocation) {
    //            iCountSup += 1;
    //            if (iCountSup > 1) {
    //                $(TempSuprows[i]).css("background-color", "red");
    //                $(TempSuprows[i].cells[1].getElementsByTagName('input')).css("background-color", "yellow");
    //                $(TempSuprows[i].cells[1].getElementsByTagName('input').txtLocation).focus();
    //                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //                $("#spanGlobalMessage").text('This Inventory Location is already added.');
    //                $('div#target').fadeToggle();
    //                $("div#target").delay(2000).fadeOut(200);
    //                return false;
    //            }
    //        }
    //    }
    //}

    //if (vtxtBinLocation == '') {
    //    //alert("Please select Inventory location.");
    //    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //    $("#spanGlobalMessage").text('Please select Inventory location.');
    //    $('div#target').fadeToggle();
    //    $("div#target").delay(5000).fadeOut(200);

    //    $(obj1).find('#txtLocation').focus();
    //    return false;
    //}
    //if ($(obj1).find('#txtCriticalQuantity').val() != 'N/A') {
    //    if (vCritical == '') {
    //        //alert("Please select Critical Quantity.");
    //        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //        $("#spanGlobalMessage").text('Please select Critical Quantity.');
    //        $('div#target').fadeToggle();
    //        $("div#target").delay(5000).fadeOut(200);

    //        $(obj1).find('#txtCriticalQuantity').focus();
    //        return false;
    //    }

    //    if (vMinimum == '') {
    //        //alert("Please select Minimum Quantity.");
    //        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //        $("#spanGlobalMessage").text('Please select Minimum Quantity.');
    //        $('div#target').fadeToggle();
    //        $("div#target").delay(5000).fadeOut(200);

    //        $(obj1).find('#txtMinimumQuantity').focus();
    //        return false;
    //    }

    //    if (vMaximum == '') {
    //        //alert("Please select Maximum Quantity.");
    //        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //        $("#spanGlobalMessage").text('Please select Maximum Quantity.');
    //        $('div#target').fadeToggle();
    //        $("div#target").delay(5000).fadeOut(200);

    //        $(obj1).find('#txtMaximumQuantity').focus();
    //        return false;
    //    }

    //    if (parseFloat(vCritical) > parseFloat(vMinimum)) {
    //        //alert('Critical quantity must be less then Minimum quantity');
    //        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //        $("#spanGlobalMessage").text('Critical quantity must be less then Minimum quantity');
    //        $('div#target').fadeToggle();
    //        $("div#target").delay(5000).fadeOut(200);

    //        $(obj1).find('#txtCriticalQuantity').focus();
    //        return false;
    //    }

    //    if (parseFloat(vMinimum) > parseFloat(vMaximum)) {
    //        //alert('Minimum quantity must be less then Maximum quantity');
    //        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
    //        $("#spanGlobalMessage").text('Minimum quantity must be less then Maximum quantity');
    //        $('div#target').fadeToggle();
    //        $("div#target").delay(5000).fadeOut(200);

    //        $(obj1).find('#txtMaximumQuantity').focus();
    //        return false;
    //    }
    //}

    //

    $.ajax({
        url: Inventory_SavetoSeesionBinReplanishSingleNew,
        data: { 'ID': vhdnID, 'SessionSr': vhdnSessionSr, 'GUID': vhdnGUID, 'ITEMGUID': vItemguid, 'BinID': vBinID, 'BinLocation': vtxtBinLocation, 'CriticalQuanity': vCritical, 'MinimumQuantity': vMinimum, 'MaximumQuantity': vMaximum, 'IsDefault': vlocIsDefault, 'eVMISensorPort': veVMISensorPort, 'eVMISensorID': veVMISensorID, 'customerOwnedQuantity': vCustOwnedQty, 'consignedQuantity': vConsQty, 'isEnforceDefaultPullQuantity': vlocIsEDPQ, 'defaultPullQuantity': vDPQty, 'isEnforceDefaultReorderQuantity': vlocIsEDRQ, 'defaultReorderQuantity': vDRQty },
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
                alert(MsgErrorInProcess);
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
                var currentRow = TempSuprows3[i];
                if (!($(currentRow).find('#txtLocation').val() == '' && $(currentRow).find('#txtCriticalQuantity').val() == '' && $(currentRow).find('#txtMinimumQuantity').val() == '' && $(currentRow).find('#txtMaximumQuantity').val() == '')) {

                    if ($(currentRow).find('#txtLocation').val() == '') {
                        $(TempSuprows3[i]).css("background-color", "red");
                        $(TempSuprows3[i]).find('#txtLocation').css("background-color", "yellow");
                        $(TempSuprows3[i]).find('#txtLocation').focus();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(MsgReqInventoryLocation);
                        $('div#target').fadeToggle();
                        //$("div#target").delay(2000).fadeOut(200);
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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

                if (!($(TempSuprows3[i]).find('#txtLocation').val() == '' && $(TempSuprows3[i]).find('#txtCriticalQuantity').val() == '' && $(TempSuprows3[i]).find('#txtMinimumQuantity').val() == '' && $(TempSuprows3[i]).find('#txtMaximumQuantity').val() == '')) {

                    //if (TempSuprows3[i].cells[1].getElementsByTagName('input').txtLocation.value == '') {
                    //    $(TempSuprows3[i]).css("background-color", "red");
                    //    $(TempSuprows3[i].cells[1].getElementsByTagName('input')[0]).css("background-color", "yellow");
                    //    $(TempSuprows3[i].cells[1].getElementsByTagName('input')[0]).focus();
                    //    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    //    $("#spanGlobalMessage").text('Inventory Location is required.');
                    //    $('div#target').fadeToggle();
                    //    $("div#target").delay(2000).fadeOut(200);
                    //    return false;
                    //}

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
                        $("#spanGlobalMessage").html(MsgLocationQuantityValidation.replace("{0}", response.ErrorMessage));
                        $('div#target').fadeToggle();
                        //$("div#target").delay(2000).fadeOut(200);
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    }
                    else if (response.status == 'ok') {
                        ////Delete///
                        //  alert(response.status);
                        if (vhdnGUID != '') {
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
                                        $("#spanGlobalMessage").html(MsgInventoryLocationDeleted);
                                        $('div#target').fadeToggle();
                                        //$("div#target").delay(2000).fadeOut(200);
                                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

                                    }
                                    else if (response.status = 'error') {
                                        alert(MsgErrorInProcess);
                                    }
                                }
                            });
                        }
                        else {
                            $(obj).parent().parent().remove();
                            BindBinReplanish(obj);

                        }
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
                    $("#spanGlobalMessage").html(MsgManufactureExist);
                    $('div#target').fadeToggle();
                    // $("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                    $("#spanGlobalMessage").html(MsgDefaultManufacturerExist);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                $("#spanGlobalMessage").html(MsgManufactureExist);
                $('div#target').fadeToggle();
                //$("div#target").delay(2000).fadeOut(200);
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                        $("#spanGlobalMessage").html(MsgManufactureExist);
                        $('div#target').fadeToggle();
                        //$("div#target").delay(2000).fadeOut(200);
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                            $("#spanGlobalMessage").html(MsgDefaultManufacturerExist);
                            $('div#target').fadeToggle();
                            // $("div#target").delay(2000).fadeOut(200);
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                    $("#spanGlobalMessage").html(MsgManufacturerdeleted);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

                }
                else if (response.status = 'error') {
                    alert(MsgErrorInProcess);
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
                    $("#spanGlobalMessage").html(MsgSupplierRequired);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    return false;
                }

                if (TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber.value == '') {
                    $(TempSuprowsAdd[k]).css("background-color", "red");
                    $(TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber).css("background-color", "yellow");
                    $(TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber).focus();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSupplierNumberRequired);
                    $('div#target').fadeToggle();
                    // $("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                        $("#spanGlobalMessage").html(MsgSupplierRequired);
                        $('div#target').fadeToggle();
                        //$("div#target").delay(2000).fadeOut(200);
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        return false;
                    }

                    if (TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber.value == '') {
                        $(TempSuprowsAdd[k]).css("background-color", "red");
                        $(TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber).css("background-color", "yellow");
                        $(TempSuprowsAdd[k].cells[1].getElementsByTagName('input').txtSupplierNumber).focus();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(MsgSupplierNumberRequired);
                        $('div#target').fadeToggle();
                        //$("div#target").delay(2000).fadeOut(200);
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
        $("#spanGlobalMessage").html(MsgSupplierRequired);
        $('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $(obj).find('#txtSupplier').focus();
        return false;
    }

    if (vtxtSupplierNumber == '') {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgSupplierNumberRequired);
        $('div#target').fadeToggle();
        // $("div#target").delay(2000).fadeOut(200);
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $(obj).find('#txtSupplierNumber').focus();
        return false;
    }

    var suppresult = true;

    if (vBlanketPOIDCount > 0) {
        if (vhdExpiry > 0 && vBlanketPOIDCount == vhdExpiry) {

            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
            $("#spanGlobalMessage").html(MsgBlanketPOExpired);
            $('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                    $("#spanGlobalMessage").html(MsgSupplierExist);
                    $('div#target').fadeToggle();
                    // $("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                    $("#spanGlobalMessage").html(MsgDefaultSupplierExist);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                $("#spanGlobalMessage").html(MsgSupplierExist);
                $('div#target').fadeToggle();
                // $("div#target").delay(2000).fadeOut(200);
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
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
                    $("#spanGlobalMessage").html(MsgSupplierDeleted);
                    $('div#target').fadeToggle();
                    // $("div#target").delay(2000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

                }
                else if (response.status = 'error') {
                    alert(MsgErrorInProcess);
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
        disableCustomerConsignedQtyOnAddLocation();
    }
    else {
        if (MethodOfValuingInventory == 3) {   // Average Cost
            //$("#Cost").addClass('disableBack');
            //$("#Cost").prop("readonly", "readonly");

            if ($('#hiddenID').val() == null || $('#hiddenID').val() == undefined || $('#hiddenID').val().trim() == '' || parseInt($('#hiddenID').val()) <= 0) {
                //$("#SellPrice").addClass('disableBack');
                //$("#SellPrice").prop("readonly", "readonly");
            }
            else {
                if ($('#Markup').val() == null || $('#Markup').val() == undefined || $('#Markup').val().trim() == '' || parseInt($('#Markup').val()) <= 0) {
                    //$("#SellPrice").addClass('disableBack');
                    //$("#SellPrice").prop("readonly", "readonly");
                }
                else {
                    $("#SellPrice").removeClass('disableBack');
                    $("#SellPrice").removeProp("readonly");
                }
            }
        }
        else if (MethodOfValuingInventory == 4) {  /// Last Cost
            $("#Cost").removeClass('disableBack');
            $("#Cost").removeProp("readonly");
        }
        if (ItemID == 0) {
            $('#ItemLocationLevelQuanity tbody tr').each(function () {
                var ConsignedQty = $(this).find('#txtCustomerOwnedQuantity').val();
                var CustomerownedQty = $(this).find('#txtConsignedQuantity').val();
                if (CustomerownedQty == '' || CustomerownedQty == undefined || CustomerownedQty == null || CustomerownedQty == 'N/A') {
                    CustomerownedQty = '0';
                }
                if (ConsignedQty == '' || ConsignedQty == undefined || ConsignedQty == null || ConsignedQty == 'N/A') {
                    ConsignedQty = '0';
                }
                var total = parseFloat(ConsignedQty) + parseFloat(CustomerownedQty);
                $(this).find('#txtCustomerOwnedQuantity').val(total);
            });
            $(".consignedQtyInput").val("");
        }
        disableCustomerConsignedQtyOnAddLocation();
    }
    updateOnHandQuantity();
}

function setDecimal(curobj) {
    var vdlID = $(curobj).val();
    if (vdlID != "") {
        var te = $(curobj).val().split(".");
        if (te[0] == '') {
            $(curobj).val("0" + $(curobj).val());
            vdlID = $(curobj).val();
        }
    }
}

function isMinusZero(value) {
    return 1 / value === -Infinity;
}

function AddMarkupAndCostToSellPrice(ActionType) {

    var cost1 = $("#Cost").val();
    var price1 = $("#SellPrice").val();
    var markup1 = $("input[type='text'][id='Markup']").val();
    if (cost1 == "" || isNaN(cost1)) {
        cost1 = 0;
    }
    if (price1 == "" || isNaN(price1)) {
        price1 = 0;
    }
    if (markup1 == "" || isNaN(markup1)) {
        markup1 = 0;
    }

    if (cost1 == 0 && price1 > 0) {
        if (ActionType == "sell") {
            cost1 = price1;
        }
        if (ActionType == "cost") {
            cost1 = 0;
            price1 = 0;
            markup1 = 0;
        }
    }

    if (markup1 == 0 && ActionType == "markup" && $.trim($("input[type='text'][id='Markup']").val()) != "") {
        price1 = cost1;
    }
    //if (markup1 == 0 && ActionType == "markup" && $.trim($("input[type='text'][id='Markup']").val()) == "") {
    //    price1 = cost1;
    //}

    if (cost1 != 0 && price1 != 0 && markup1 != 0) {
        if (ActionType == "markup") {
            price1 = parseFloat(cost1) + parseFloat(((cost1 * markup1) / 100));
        }
        else {
            markup1 = parseFloat(((price1 * 100) / cost1)) - 100;
        }

    }
    else if (cost1 != 0 && price1 == 0 && markup1 == 0) {
        price1 = cost1;
    }
    else if (cost1 != 0 && price1 != 0 && markup1 == 0) {
        markup1 = parseFloat(((price1 * 100) / cost1)) - 100;
    }
    else if (cost1 != 0 && price1 == 0 && markup1 != 0) {
        price1 = parseFloat(cost1) + parseFloat(((cost1 * markup1) / 100));
    }
    else if (cost1 == 0 && price1 != 0 && markup1 != 0) {
        cost1 = parseFloat(price1) - parseFloat(((price1 * markup1) / 100));
    }
    else if (cost1 == 0 && price1 == 0 && markup1 == 0) {

    }
    else if (cost1 == 0 && price1 != 0 && markup1 == 0) {
        cost1 = price1;
    }
    else if (cost1 == 0 && price1 == 0 && markup1 != 0) {
        var isSetMarkupZero = true;
        var RoomGlobMarkupLabor = $("#hdnRoomGlobMarkupLabor").val();
        var RoomGlobMarkupParts = $("#hdnRoomGlobMarkupParts").val();
        if (ItemType == 4) {
            if (RoomGlobMarkupLabor > 0) {
                isSetMarkupZero = false;
            }
        }
        else if (RoomGlobMarkupParts > 0) {
            isSetMarkupZero = false;
        }
        if (isSetMarkupZero == true) {
            var hdMarkup = $("input[type='hidden'][id='Markup']").val();
            if (hdMarkup == "" || hdMarkup <= 0) {
                markup1 = 0;
            }
        }
        //if (MethodOfValuingInventory == 3) {
        //    var isConsigned = $('#Consignment').prop("checked");
        //    var onHandQty = $("input#OnHandQuantity").val();
        //    if (!isConsigned && (onHandQty == null || onHandQty <= 0)) {
        //        markup1 = 0;
        //    }
        //}
    }


    markup1 = FormatedCostQtyValues(markup1, 1);
    var isMZero = isMinusZero(markup1);
    if (isMZero) {
        markup1 = FormatedCostQtyValues((markup1 * -1), 1);
    }
    //price1 = FormatedCostQtyValues(price1, 1);
    //cost1 = FormatedCostQtyValues(cost1, 1);

    $("#Cost").val(cost1);
    $("#SellPrice").val(price1);
    $("input[type='text'][id='Markup']").val(markup1);

    if (ActionType != "markup") {
        $("input[type='hidden'][id='Markup']").val($("input[type='text'][id='Markup']").val());
    }
    else {
        $("input[type='hidden'][id='Markup']").val($("input[type='text'][id='Markup']").val());
    }
}
function AddMarkupAndCostToSellPrice1(ActionType) {

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
        else if (parseFloat(TempSellPrice) > 0) {

        }

        $("input[type='hidden'][id='Markup']").val($("input[type='text'][id='Markup']").val());
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
            //alert('Invalid URL, kindly enter valid URL');
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgInvalidURL);
            $('div#target').fadeToggle();
            //$("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

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
    else if (PopupFor == 'OrderUOM') {
        _URL = Master_OrderUOMCreate;
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
        else if (PopupFor == 'OrderUOM') {
            _ControlID = "OrderUOMID";
        }
        else if (PopupFor == 'UOMUnit') {
            _ControlID = "UOMID";
        }
        else if (PopupFor == 'InventoryClassification') {
            _ControlID = "drpInventoryClassification";
        }

        var arrdata = IDVal.split("~");
        var listData = $('select[id="' + _ControlID + '"]');

        if (_ControlID == "UOMID") {
            if ($("#UOMID option[value='" + arrdata[0] + "']").length <= 0) {
                $(listData).each(function () {
                    $(this).append($("<option />").val(arrdata[0]).text(arrdata[1]));
                });
            }
        }
        else {
            $(listData).each(function () {

                $(this).append($("<option />").val(arrdata[0]).text(arrdata[1]));
            });
        }

        if (PopupFor == "CostUOM") {
            var ocstData = $("#CostUOMData").val();
            var obj = JSON.parse(ocstData);
            var newCount = obj.length + 1;
            var newObjStr = '},{"ID":' + arrdata[0] + ',"Text":"' + arrdata[1] + '","Value":"' + arrdata[2] + '","Count":' + newCount + ',"PageName":null,"ControlID":null,"ItemModelCallFromPageName":null,"IDsufix":null,"ParentID":0,"ToolModelCallFromPageName":null}]';
            var newAppendStr = ocstData.replace("}]", newObjStr);
            $("#CostUOMData").val(newAppendStr);
            //alert(newAppendStr);
        }
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

function AllowOhQty() {

    setTimeout(function () {

        var isConsigned = $('#Consignment').prop("checked");

        if (ItemID == 0) {
            var srtrck = $("input#rdoSerialTracking").prop("checked");
            var lottrck = $("input#rdoLotTracking").prop("checked");
            var datecodetrck = $("input#chkDateCodeTracking").prop("checked");
            var isBuildBreak = $('#IsBuildBreak');
            var tmpItemType = $("#dlItemType").val();
            if (!srtrck && !lottrck && !datecodetrck && ((tmpItemType == 3 && typeof (isBuildBreak) !== "undefined" && isBuildBreak.is(":checked") === false) || (tmpItemType != 3 && tmpItemType != 4))) {
                $("input#OnHandQuantity").removeProp("readonly").removeClass("disableBack");
                $('[data-calc="OHQ"]').removeProp("disabled").removeProp("readonly").removeClass("disableBack");
                if (!isConsigned) {
                    $(".consignedQtyInput").val("").addClass('disableBack').prop("readonly", "readonly").prop("disabled", "disabled");
                }
            }
            else {
                $("input#OnHandQuantity").val("");
                $("input#OnHandQuantity").prop("readonly", "readonly").addClass("disableBack");
                $('[data-calc="OHQ"]').val("");
                $('[data-calc="OHQ"]').prop("readonly", "readonly").prop("disabled", "disabled").addClass("disableBack");
            }
        }
        else {
            $("input#OnHandQuantity").prop("readonly", "readonly").addClass("disableBack");
            $('[data-calc="OHQ"]').prop("readonly", "readonly").prop("disabled", "disabled").addClass("disableBack");
        }
    }, 100);
}

function updateOnHandQuantity() {
    if (ItemID == 0) {
        if (itemIsNotOfTypeLotSerialDatecode()) {
            $("input#OnHandQuantity").removeProp("readonly").removeClass("disableBack");
            var totalOnHandQty = 0;
            var anSelectedLocations;
            if (typeof (oTableProjectItems) != "undefined" && oTableProjectItems != null)
                anSelectedLocations = oTableProjectItems.$('tr');
            else
                anSelectedLocations = null;

            if (anSelectedLocations != null && typeof (anSelectedLocations) != "undefined" && anSelectedLocations.length != 0) {

                for (var i = 0; i <= anSelectedLocations.length - 1; i++) {
                    $(anSelectedLocations[i]).find('[data-calc="OHQ"]').each(function () {
                        if ($.isNumeric(this.value)) {
                            totalOnHandQty += parseFloat(this.value);
                        }
                    });
                }
            }
            else {
                $('[data-calc="OHQ"]').each(function () {
                    if ($.isNumeric(this.value)) {
                        totalOnHandQty += parseFloat(this.value);
                    }
                });
            }

            $("input#OnHandQuantity").val(FormatedCostQtyValues(parseFloat(totalOnHandQty)));
        }
        else {
            //$("input#OnHandQuantity").val("");
            //$("input#OnHandQuantity").prop("readonly", "readonly").addClass("disableBack");
        }
    }
    else {
        var anSelectedLocations;
        if (typeof (oTableProjectItems) != "undefined" && oTableProjectItems != null)
            anSelectedLocations = oTableProjectItems.$('tr');
        else
            anSelectedLocations = null;

        if (anSelectedLocations != null && typeof (anSelectedLocations) != "undefined" && anSelectedLocations.length != 0) {
            var totalOnHandQty = 0;

            for (var i = 0; i <= anSelectedLocations.length - 1; i++) {
                $(anSelectedLocations[i]).find('[data-calc="OHQ"]').each(function () {
                    if ($.isNumeric(this.value)) {
                        totalOnHandQty += parseFloat(this.value);
                    }
                });
            }
            $("input#OnHandQuantity").val(FormatedCostQtyValues(parseFloat(totalOnHandQty)));
            $("input#OnHandQuantity").prop("readonly", "readonly").addClass("disableBack");
        }
        else {
            var totalOnHandQty = 0;
            $('[data-calc="OHQ"]').each(function () {
                if ($.isNumeric(this.value)) {
                    totalOnHandQty += parseFloat(this.value);
                }
            });

            $("input#OnHandQuantity").val(FormatedCostQtyValues(parseFloat(totalOnHandQty)));
            $("input#OnHandQuantity").prop("readonly", "readonly").addClass("disableBack");
        }
    }
}

function disableCustomerConsignedQtyOnAddLocation() {
    var isConsigned = $('#Consignment').prop("checked");
    if (ItemID > 0) {
        var anSelectedLocations;
        if (typeof (oTableProjectItems) != "undefined" && oTableProjectItems != null)
            anSelectedLocations = oTableProjectItems.$('tr');
        else
            anSelectedLocations = null;

        if (anSelectedLocations != null && typeof (anSelectedLocations) != "undefined" && anSelectedLocations.length != 0) {
            for (var i = 0; i <= anSelectedLocations.length - 1; i++) {
                $(anSelectedLocations[i]).find('[data-calc="OHQ"]').addClass("disableBack").prop('readonly', "readonly").prop("disabled", "disabled");
            }
        }
        else {
            $('[data-calc="OHQ"]').addClass("disableBack").prop('readonly', "readonly").prop("disabled", "disabled");
        }

        if (itemIsNotOfTypeLotSerialDatecode()) {

            if (anSelectedLocations != null && typeof (anSelectedLocations) != "undefined" && anSelectedLocations.length != 0) {
                for (var i = 0; i <= anSelectedLocations.length - 1; i++) {
                    $(anSelectedLocations[i]).find('[data-calc="OHQ"]').each(function () {
                        var binId = $(this).data("binid");
                        if ((!$.isNumeric(binId) || ($.isNumeric(binId) && binId < 1))) {
                            $(this).removeProp("disabled").removeProp("readonly").removeClass("disableBack");
                            if (!isConsigned && $(this).attr("id") == "txtConsignedQuantity") {
                                $(this).val("");
                            }
                        }
                    });

                    if (!isConsigned) {
                        $(anSelectedLocations[i]).find(".consignedQtyInput").addClass('disableBack').prop("readonly", "readonly").prop("disabled", "disabled");
                    }
                }
            }
            else {
                $('[data-calc="OHQ"]').each(function () {
                    var binId = $(this).data("binid");
                    if ((!$.isNumeric(binId) || ($.isNumeric(binId) && binId < 1))) {
                        $(this).removeProp("disabled").removeProp("readonly").removeClass("disableBack");
                        if (!isConsigned && $(this).attr("id") == "txtConsignedQuantity") {
                            $(this).val("");
                        }
                    }
                });

                if (!isConsigned) {
                    $(".consignedQtyInput").addClass('disableBack').prop("readonly", "readonly").prop("disabled", "disabled");
                }
            }
        }
    }
    else {
        var anSelectedLocations;
        if (typeof (oTableProjectItems) != "undefined" && oTableProjectItems != null)
            anSelectedLocations = oTableProjectItems.$('tr');
        else
            anSelectedLocations = null;

        if (!isConsigned) {
            if (anSelectedLocations != null && typeof (anSelectedLocations) != "undefined" && anSelectedLocations.length != 0) {
                for (var i = 0; i <= anSelectedLocations.length - 1; i++) {
                    $(anSelectedLocations[i]).find(".consignedQtyInput").val("").addClass('disableBack').prop("readonly", "readonly").prop("disabled", "disabled");
                }
            }
            else {
                $(".consignedQtyInput").val("").addClass('disableBack').prop("readonly", "readonly").prop("disabled", "disabled");
            }
        }
        else {
            if (itemIsNotOfTypeLotSerialDatecode()) {
                if (anSelectedLocations != null && typeof (anSelectedLocations) != "undefined" && anSelectedLocations.length != 0) {
                    for (var i = 0; i <= anSelectedLocations.length - 1; i++) {
                        $(anSelectedLocations[i]).find(".consignedQtyInput").removeProp("disabled").removeProp("readonly").removeClass("disableBack");
                    }
                }
                else {
                    $(".consignedQtyInput").removeProp("disabled").removeProp("readonly").removeClass("disableBack");
                }
            }
        }

        var isBuildBreak = $('#IsBuildBreak');

        if ((!itemIsNotOfTypeLotSerialDatecode()) || (isBuildBreak !== undefined && isBuildBreak.is(":checked"))) {
            if (anSelectedLocations != null && typeof (anSelectedLocations) != "undefined" && anSelectedLocations.length != 0) {
                for (var i = 0; i <= anSelectedLocations.length - 1; i++) {
                    $(anSelectedLocations[i]).find('[data-calc="OHQ"]').val("").addClass("disableBack").prop('readonly', "readonly").prop("disabled", "disabled");
                }
            }
            else {
                $('[data-calc="OHQ"]').val("").addClass("disableBack").prop('readonly', "readonly").prop("disabled", "disabled");
            }

        }
    }
}

function SetCostUOMvalueForBinGrid() {
    if ($("#IsAllowOrderCostuom").attr('checked') == 'checked') {
        var DefQTY = $("#DefaultReorderQuantity").val();
        $('#ItemLocationLevelQuanity tbody tr').each(function () {

            if ($(this).find("#chkIsEnforceDefaultReorderQuantity").attr('checked') != 'checked') {

                $(this).find("#chkIsEnforceDefaultReorderQuantity").attr('checked', 'checked');
                $(this).find("#chkIsEnforceDefaultReorderQuantity").attr('disabled', 'disabled');
                $(this).find('#txtDefaultReorderQuantity').val(DefQTY);
            }
            else {
                $(this).find("#chkIsEnforceDefaultReorderQuantity").attr('disabled', 'disabled');
            }
        });
    }
}

function SetCostUOMvalueForBinGridCostUOMChange() {
    if ($("#IsAllowOrderCostuom").attr('checked') == 'checked') {
        var DefQTY = $("#DefaultReorderQuantity").val();
        $('#ItemLocationLevelQuanity tbody tr').each(function () {

            if ($(this).find("#chkIsEnforceDefaultReorderQuantity").attr('checked') == 'checked') {

                //$(this).find("#chkIsEnforceDefaultReorderQuantity").attr('checked', 'checked');
                $(this).find("#chkIsEnforceDefaultReorderQuantity").attr('disabled', 'disabled');
                $(this).find('#txtDefaultReorderQuantity').val(DefQTY);
            }
        });
    }
}

function itemIsNotOfTypeLotSerialDatecode() {
    if (ItemID > 0) {
        if (isSerialNumberTracking.toLowerCase() == "false" && isLotNumberTracking.toLowerCase() == "false" && isDateCodeTracking.toLowerCase() == "false") {
            return true;
        }
        return false;
    }
    else {
        var srtrck = $("input#rdoSerialTracking").prop("checked");
        var lottrck = $("input#rdoLotTracking").prop("checked");
        var datecodetrck = $("input#chkDateCodeTracking").prop("checked");
        if (!srtrck && !lottrck && !datecodetrck) {
            return true;
        }
        return false;
    }
}

function updateDefaultLocation(obj) {
    setTimeout(function () {
        if (ItemID == 0 && itemIsNotOfTypeLotSerialDatecode()) {
            var onHandQty = $("input#OnHandQuantity").val();
            if (onHandQty !== undefined && onHandQty != null && $.isNumeric(onHandQty.trim())) {
                $('[data-calc="OHQ"]').val("");
                var defaultBin = $('[data-id="txtLoc"]:checked=checked');
                var isConsigned = $('#Consignment').prop("checked");

                if (isConsigned == true) {
                    $(defaultBin).parent().parent().find('input[id="txtConsignedQuantity"]').val(parseFloat(onHandQty.trim()));
                }
                else {
                    $(defaultBin).parent().parent().find('input[id="txtCustomerOwnedQuantity"]').val(parseFloat(onHandQty.trim()));
                }
            }
        }
    }, 200);
}

function BindSolumnLabels() {
    $('#divSolumnLabelDetails').empty();
    $('#DivLoading').show();
    $.get(Inventory_LoadSolumnLabels + '?SupplierPartNumber=' + SupplierPartNumber + '&strExistingLabelsList=' + '&AddCount=0&IsBOMItem=' + vIsBOMItem, function (data) {
        $('#divSolumnLabelDetails').html(data);
        // DisableLocationTextboxForBOM('True');
        $('#DivLoading').hide();
        SetLabelsValues();
    });
}


function AddNewSolumnLabel() {
        var ExistingSelectedLabels = getExistingSelectedLabels();
        $('#divSolumnLabelDetails').empty();
        $('#DivLoading').show();
        $.get(Inventory_LoadSolumnLabels + '?SupplierPartNumber=' + SupplierPartNumber + '&strExistingLabelsList=' + ExistingSelectedLabels + '&AddCount=1&IsBOMItem=' + vIsBOMItem , function (data) {
            $('#divSolumnLabelDetails').html(data);
            // DisableLocationTextboxForBOM('True');
            $('#DivLoading').hide();
        });
}

function getExistingSelectedLabels() {
    var SelectedSolumLabels = '';
    $('#SolumLabelList tbody tr').each(function () {
        if ($(this).find("#txtLabelCode").length > 0) {
            SelectedSolumLabels += $(this).find("#txtLabelCode").val() + ",";
        } else if ($(this).find("#txtLabelCode").length > 0) {
            //display in red
        }
    });
    return SelectedSolumLabels;
}

function SetLabelsValues() {
    if ($("#SolumMappedLabels").length > 0) {
        var Assignlabels = getExistingSelectedLabels();
        $("#SolumMappedLabels").val(Assignlabels);
        var ExisitngLabelArray = $("#hdnExistingLabels").val().split(',');
        for (var i = 0; i < ExisitngLabelArray.length; i++) {
            if (Assignlabels.indexOf(ExisitngLabelArray[i]) < 0) {
                var ExistingValue = $("#SolumUnMappedLabels").val();
                ExistingValue += ExisitngLabelArray[i] + ",";
                $("#SolumUnMappedLabels").val(ExistingValue);
            }
        }
    }
    return true;
}


function LabelsValidation(e) {
    var currentElement = $(e);
    if (currentElement.val().length > 0) {
    var Assignlabels = getExistingSelectedLabels();
    if (chkOccurrences(Assignlabels, currentElement.val()) > 1) {
        alert('Duplicate labels selected for item.');
        currentElement.css("border-color", 'red');
        currentElement.attr("ActiveError", 'true');
        $("#btnAddmoreLabel").attr("disabled", "disabled");
        currentElement.next().hide();
        return false;
    } else {
        //$(e).css("border", "1px");
        currentElement.css("border-color", '');
        currentElement.removeAttr("ActiveError");
        $("#btnAddmoreLabel").removeAttr("disabled");
    }

        $('#DivLoading').show();
        $.ajax({
            'url': '/Inventory/VerifyLabelCheckSUM',
            'data': { 'LabelCode': currentElement.val() },
            'type': 'Post',
            'async': false,
            'cache': false,
            'dataType': 'json',
            'success': function (response) {
                if (response.Status === "ok") {
                    if (response.returnCode == "405") {
                        currentElement.css("border-color", 'red');
                        currentElement.attr("ActiveError", 'true');
                        currentElement.next().show();
                        $("#btnAddmoreLabel").attr("disabled", "disabled");
                    } else {
                        currentElement.css("border-color", '');
                        currentElement.next().hide();
                        currentElement.removeAttr("ActiveError");
                        $("#btnAddmoreLabel").removeAttr("disabled");
                    }
                }
                $('#DivLoading').hide();
            },
            'error': function (xhr) {
                $('#DivLoading').hide();
                console.log('error');
                return true;
            }

        });
    }
    SetLabelsValues();
    return true

}

function DeleteFromListSingle(element) {
    if ($($(element).parent().parent()).find("#txtLabelCode").length > 0) {
        var ExistingValue = $("#SolumUnMappedLabels").val();
        var NewUnMappedValue = $($(element).parent().parent()).find("#txtLabelCode").val();
        if ($("#hdnNonExistingLabels").length > 0) {
            if ($("#hdnNonExistingLabels").val().indexOf(NewUnMappedValue) < 0) {
                ExistingValue += NewUnMappedValue + ",";
                $("#SolumUnMappedLabels").val(ExistingValue);
            }
        }
        $($(element).parent().parent()).remove();
        SetLabelsValues();
    } 
}

function chkOccurrences(string, substring) {
    var n = 0;
    var pos = 0;
    while (true) {
        pos = string.indexOf(substring, pos);
        if (pos != -1) { n++; pos += substring.length; }
        else { break; }
    }
    return (n);
}
