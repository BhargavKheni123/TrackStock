
var _ReceiveItem = (function ($) {
    var self = {};

    var IsLoadMoreLocations = false;

    self.urls = {
        itemLocationDetailsSaveOrderURL: null, itemLocationDetailsSaveForMSCreditURL: null
        , saveReceiveInformationURL: null, getItemLocationsURL :null
    };
    self.roomDateJSFormat = null;
    self.model = {
        stagingID: null, orderDetailID: null,
        ItemGUID: null,
        OrderGUID: null,
        OrderStatus: null
    };

    self.init = function (roomDateJSFormat, orderDetailID, stagingID, OrderGUID, ItemGUID, OrderStatus,
        itemLocationDetailsSaveOrderURL, itemLocationDetailsSaveForMSCreditURL
        , saveReceiveInformationURL, getItemLocationsURL
        , CurrencyDecimalDigits
    ) {

        self.model.orderDetailID = orderDetailID;
        self.model.stagingID = stagingID;
        self.model.OrderGUID = OrderGUID;
        self.model.OrderStatus = OrderStatus;
        self.model.ItemGUID = ItemGUID;

        //urls
        self.urls.itemLocationDetailsSaveOrderURL = itemLocationDetailsSaveOrderURL;
        self.urls.itemLocationDetailsSaveForMSCreditURL = itemLocationDetailsSaveForMSCreditURL;
        self.urls.saveReceiveInformationURL = saveReceiveInformationURL;
        self.urls.getItemLocationsURL = getItemLocationsURL;

        self.roomDateJSFormat = roomDateJSFormat;
        self.CurrencyDecimalDigits = CurrencyDecimalDigits;
    }

    self.initEvents = function () {
        $(document).ready(function () {

            UDFfillEditableOptionsForGrid();

            $('.ShowAllOptions').click(function () {
                $(this).siblings('.ReciveInAuto').trigger("focus");
                $(this).siblings(".ReciveInAuto").autocomplete("search", " ");
            });

            $('form').areYouSure();
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

            $("[name*=" + "Received" + "]").blur(function () {
            }).datepicker({
                dateFormat: _ReceiveItem.roomDateJSFormat, showButtonPanel: true,
                changeMonth: true,
                changeYear: true,
                clearText: 'Clear', onClose: function () { this.focus(); }
            });

            $("[name*=" + "Expiration" + "]").blur(function () {
            }).datepicker({
                dateFormat: _ReceiveItem.roomDateJSFormat, showButtonPanel: true,
                changeMonth: true,
                changeYear: true,
                clearText: 'Clear', onClose: function () { this.focus(); }
            });

            fillReceivedItemGrid(_ReceiveItem.model.orderDetailID);

            $('#NewReceiveEntry' + _ReceiveItem.model.orderDetailID).on({
                mouseover: function () {
                    var itmGuid = _ReceiveItem.model.ItemGUID;
                    var orderGuid = _ReceiveItem.model.OrderGUID;
                    if (gblControllerName.toLowerCase() == "order") {
                        getItemInventoryStagingLocation1($(this), "", itmGuid, orderGuid);
                    }
                    else {
                        getItemInventoryStagingLocation($(this), "", itmGuid, orderGuid);
                    }

                },
                focus: function () {
                    var itmGuid = _ReceiveItem.model.ItemGUID;
                    var orderGuid = _ReceiveItem.model.OrderGUID;
                    getItemInventoryStagingLocation($(this), "", itmGuid, orderGuid);
                },
                change: function () {
                    $(this).parent().find("input[id='txtReceiveBinNumber__" + _ReceiveItem.model.OrderDetailID + "']").val($(this).val());
                }
            }, "#slctReceiveBinNumber_" + _ReceiveItem.model.OrderDetailID);

            $('#NewReceiveEntry' + _ReceiveItem.model.OrderDetailID).on('focus', "input.ReciveInAuto", function (e) {

                var ajaxURL = _ReceiveItem.urls.getItemLocationsURL; //'/Receive/GetItemLocations';
                var tr = $(this).parent().parent().parent();
                var itmGuid = _ReceiveItem.model.ItemGUID;
                var orderGuid = _ReceiveItem.model.OrderGUID;

                $(this).autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            url: ajaxURL,
                            type: 'POST',
                            data: JSON.stringify({ 'OrderGuid': orderGuid, 'ItemGuid': itmGuid, 'NameStartWith': request.term, 'IsLoadMoreLocations': IsLoadMoreLocations }),
                            contentType: 'application/json',
                            dataType: 'json',
                            success: function (data) {
                                response($.map(data, function (Items) {
                                    return {
                                        label: Items.Value,
                                        value: Items.Key
                                    }
                                }));

                            },
                            error: function (err) {
                                $('#DivLoading').hide();
                            }
                        });
                    },
                    autoFocus: false,
                    minLength: 1,
                    select: function (event, ui) {
                        if (ui.item.value == MoreLocation) {
                            IsLoadMoreLocations = true;
                            $(this).trigger("focus");
                            $(this).autocomplete("search", " ");
                            return false;
                        }
                        else {
                            $(this).val(ui.item.value);
                        }
                    },
                    open: function () {
                        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                        $('ul.ui-autocomplete').css('overflow-y', 'auto');
                        $('ul.ui-autocomplete').css('max-height', '300px');
                    },
                    close: function () {
                        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                    },
                    change: function (event, ui) {
                    }
                });
            });
        });

    }

    self.NewReceiveStage = function (ordDetailID, obj) {
        $(obj).attr('disabled', 'disabled');
        $('#DivLoading').show();

        var tr = $(obj).parent().parent();
        var data = self.getHiddenFieldsData(tr, ordDetailID)

        var consqty = null;
        var custqty = null;
        var expirationDate = null;
        var qty = 0;
        var lotNumber = "";
        var srNumber = "";
        var isError = false;
        var ErrorMassage = "";
        var receivecost = 0;
        var arrLocDetail = new Array();

        var isSerial = data.SerialNumberTracking; //$(tr).find("#hdnIsSerial_" + ordDetailID).val();
        var consign = data.ItemConsignment; //$(tr).find("#hdnIsConsign_" + ordDetailID).val();
        var isLot = data.LotNumberTracking; //$(tr).find("#hdnIsLot_" + ordDetailID).val();
        var isDateCode = data.DateCodeTracking; //$(tr).find("#hdnIsDateCode_" + ordDetailID).val();
        var ordDetailGUID = data.OrderDetailGUID; //$(tr).find("#hdnOrderDetailGUID_" + ordDetailID).val();
        var itemGUID = data.ItemGUID; //$(tr).find("#hdnItemGuid_" + ordDetailID).val();
        var binName = data.ReceiveBinName; //$(tr).find('#txtReceiveBinNumber_' + ordDetailID).val();
        var packSlipNumber = data.PackSlipNumber; //$(tr).find('#txtPackslip_' + ordDetailID).val();
        var comment = data.comment; //$(tr).find('#Comment_' + ordDetailID).val();

        var recvDate = data.recvDate; //$(tr).find("#txtReceived_" + ordDetailID).val();
        var itemType = data.ItemType; //$(tr).find("#hdnItemType_" + ordDetailID).val();
        var ReqstedQty = data.RequestedQuantity; //parseFloat($(tr).find("#hdnRequestedQty_" + ordDetailID).val());
        var ApprovedQty = data.ApprovedQuantity; //parseFloat($(tr).find("#hdnApprovedQty_" + ordDetailID).val());
        var ReceivedQty = data.ReceivedQuantity; //parseFloat($(tr).find("#hdnReceivedQty_" + ordDetailID).val());
        var UDF1 = data.UDF1; //getUDFValues('UDF1', ordDetailID);
        var UDF2 = data.UDF2; //getUDFValues('UDF2', ordDetailID);
        var UDF3 = data.UDF3; //getUDFValues('UDF3', ordDetailID);
        var UDF4 = data.UDF4; //getUDFValues('UDF4', ordDetailID);
        var UDF5 = data.UDF5; //getUDFValues('UDF5', ordDetailID);
        var isPackSlipNumberMandatory = data.IsPackSlipNumberMandatory; //$(tr).find("#hdnIsPackSlipNumberMandatory_" + ordDetailID).val();
        var stagID = data.StagingID; //$(tr).find("#hdnStagingID_" + ordDetailID).val();

        if ($(tr).find("#txtCostBox_" + ordDetailID).length > 0) {
            receivecost = $(tr).find("#txtCostBox_" + ordDetailID).val();
        }
        else if ($(tr).find("#hdnCostBox_" + ordDetailID).length > 0) {
            receivecost = $(tr).find("#hdnCostBox_" + ordDetailID).val();
        }

        if ($.trim($(tr).find('#txtReceiveBinNumber_' + ordDetailID).val()).length <= 0 && parseInt(stagID) <= 0) {
            isError = true;
            ErrorMassage += "<b style='color: Red;'>" + MsgBinNumberValidation +"</b><br/>"
        }
        if (isSerial == "True") {
            srNumber = $(tr).find("#txtsrnumber_" + ordDetailID).val();
            if (srNumber.length <= 0) {
                isError = true;
                ErrorMassage += "<b style='color: Red;'>" + MsgSerialNumberValidation +"</b><br/>"
            }
            else {
                var txtSRNumber = $(tr).find("#txtsrnumber_" + ordDetailID);
                if (txtSRNumber.css('background-color') === "rgb(247, 187, 196)") {
                    $("#spanGlobalMessage").html(MsgDuplicateSerialNumberValidation);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    showNotificationDialog();
                    $("#DivLoading").hide();
                    $("#txtsrnumber_" + ordDetailID).focus();
                    return false;
                }
                else
                    qty = 1;
            }
        }
        else {
            qty = $(tr).find("#txtReceiveQuantity_" + ordDetailID).val();
            if (isNaN(qty) || qty <= 0) {
                isError = true;
                ErrorMassage += "<b style='color: Red;'>" + MsgEnterQuantityReceive + "</b><br/>"
            }
        }


        if (isPackSlipNumberMandatory.toUpperCase() == 'TRUE' && packSlipNumber.length <= 0) {
            isError = true;
            ErrorMassage += "<b style='color: Red;'> " + MsgEnterPayslipNumber +"</b><br/>"
        }

        if (isLot == "True") {
            lotNumber = $(tr).find("#txtlotnumber_" + ordDetailID).val();
            if (lotNumber.length <= 0) {
                isError = true;
                ErrorMassage += "<b style='color: Red;'>" + MsgEnterLotNumber +"</b><br/>"
            }
        }
        if (isDateCode == "True") {
            expirationDate = $(tr).find("#txtExpiration_" + ordDetailID).val();
            if (expirationDate.length <= 0) {
                isError = true;
                ErrorMassage += "<b style='color: Red;'>" + MsgExpireDateValidation +"</b><br/>"
            }
        }

        if (consign == "True") {
            consqty = qty;
        }
        else {
            custqty = qty;
        }

        if (isError) {
            ErrorMassage = '<b>' + MsgReceiveReasons +'</b><br />' + ErrorMassage;
            $('#OrdReceivedErrorDialog').find("#OrdReceivErrorMSG").html(ErrorMassage);
            $('#DivLoading').hide();
            $('#OrdReceivedErrorDialog').modal();
            $('#btnReceive_' + ordDetailID).removeAttr('disabled');
            return false;
        }
        else {
            var isTrue = false;
            //if (confirm(msg)) {

            if (isNaN(parseFloat(ApprovedQty)) || parseFloat(ApprovedQty) < 0) {
                ApprovedQty = 0;
            }

            if (isNaN(parseFloat(ReceivedQty)) || parseFloat(ReceivedQty) < 0) {
                ReceivedQty = 0;
            }

            if (isNaN(parseFloat(qty)) || parseFloat(qty) < 0) {
                qty = 0;
            }

            if (parseFloat(ApprovedQty) < (parseFloat(qty) + parseFloat(ReceivedQty))) {
                var msg = MsgExceedApprovedQuantity;
                if (confirm(msg)) {
                    isTrue = true;
                }
            }
            else
                isTrue = true;

            if (isTrue) {
                var obj = {
                    "BinNumber": binName, "CustomerOwnedQuantity": custqty, "ConsignedQuantity": consqty,
                    "LotNumber": lotNumber, "SerialNumber": srNumber, "ExpirationDate": expirationDate,
                    "ReceivedDate": recvDate, "Expiration": expirationDate, "Received": recvDate,
                    "Cost": receivecost, "ItemGUID": itemGUID, "ItemType": itemType,
                    "SerialNumberTracking": isSerial, "LotNumberTracking": isLot, "DateCodeTracking": isDateCode,
                    "OrderDetailGUID": ordDetailGUID, "ItemCost": receivecost, "PackSlipNumber": packSlipNumber,
                    "UDF1": UDF1, "UDF2": UDF2, "UDF3": UDF3, "UDF4": UDF4, "UDF5": UDF5, "IsOnlyFromUI": true
                };



                arrLocDetail.push(obj);
                var url = _ReceiveItem.urls.itemLocationDetailsSaveOrderURL;
                if (!isNaN(stagID) && parseInt(stagID) > 0) {
                    url = _ReceiveItem.urls.itemLocationDetailsSaveForMSCreditURL;
                }
                else {

                }
                setTimeout(function () {
                    $.ajax({
                        "url": url,
                        "data": JSON.stringify(arrLocDetail),
                        "type": 'POST',
                        "async": false,
                        "cache": false,
                        "dataType": "json",
                        "contentType": "application/json",
                        "success": function (response) {
                            if (response.Status == "OK") {
                                SetReplenishRedCount();
                                UDFInsertNewForGrid($('#NewReceiveEntry' + ordDetailID));
                                ImageIDToOpen = "#imgPlusMinus_" + ordDetailID;
                                $('#myDataTable').dataTable().fnStandingRedraw();
                                $('#DivLoading').hide();

                            }
                            else if (response.Status === "UDFError") {
                                $('#DivLoading').hide();
                                alert(response.Message);
                            }
                            else {
                                $('#DivLoading').hide();
                                alert(MsgErrorNotReceived);
                            }
                            return false;

                        },
                        "error": function (error) {
                            $('#btnReceive_' + ordDetailID).removeAttr('disabled');
                            $('#DivLoading').hide();
                            $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html("<b style='color: Red;'>" + MsgErrorInAjaxrequest +"</b>");
                            $('#OrdReceivedInfoDialog').modal();
                            return false;
                        },
                        "completed": function (obj) {
                            $('#btnReceive_' + ordDetailID).removeAttr('disabled');
                            $('#myDataTable').find("#imgPlusMinus_" + ordDetailID).click();
                            $('#DivLoading').hide();
                        }

                    })
                }, 50);
            }
            else {
                $('#btnReceive_' + ordDetailID).removeAttr('disabled');
                $('#DivLoading').hide();
                return false;
            }

        }
    };

    self.getHiddenFieldsData = function (tr, ordDetailID) {

        var obj = {};
        
        obj.ItemConsignment = $(tr).find("#hdnIsConsign_" + ordDetailID).val();
        obj.SerialNumberTracking = $(tr).find("#hdnIsSerial_" + ordDetailID).val();
        obj.LotNumberTracking = $(tr).find("#hdnIsLot_" + ordDetailID).val();
        obj.DateCodeTracking = $(tr).find("#hdnIsDateCode_" + ordDetailID).val();
        obj.OrderDetailGUID = $(tr).find("#hdnOrderDetailGUID_" + ordDetailID).val();
        obj.ItemGUID = $(tr).find("#hdnItemGuid_" + ordDetailID).val();
        obj.ItemType = $(tr).find("#hdnItemType_" + ordDetailID).val();
        obj.RequestedQuantity = parseFloat($(tr).find("#hdnRequestedQty_" + ordDetailID).val());
        obj.StagingID = $(tr).find("#hdnStagingID_" + ordDetailID).val();
        obj.ApprovedQuantity = parseFloat($(tr).find("#hdnApprovedQty_" + ordDetailID).val());
        obj.ReceivedQuantity = parseFloat($(tr).find("#hdnReceivedQty_" + ordDetailID).val());
        obj.OrderGUID = $(tr).find("#hdnOrderGuid_" + ordDetailID).val();
        obj.IsPackSlipNumberMandatory = $(tr).find("#hdnIsPackSlipNumberMandatory_" + ordDetailID).val();
        obj.ItemNumber = $(tr).find("#hdnItemNumber_" + ordDetailID).val();
        obj.OrderReleaseNumber = $(tr).find("#hdnOrderReleaseNumber_" + ordDetailID).val();
        

        obj.ReceiveBinName = $(tr).find('#txtReceiveBinNumber_' + ordDetailID).val();
        obj.PackSlipNumber = $(tr).find('#txtPackslip_' + ordDetailID).val();
        obj.comment = $(tr).find('#Comment_' + ordDetailID).val();
        obj.recvDate = $(tr).find("#txtReceived_" + ordDetailID).val();
        obj.UDF1 = getUDFValues('UDF1', ordDetailID);
        obj.UDF2 = getUDFValues('UDF2', ordDetailID);
        obj.UDF3 = getUDFValues('UDF3', ordDetailID);
        obj.UDF4 = getUDFValues('UDF4', ordDetailID);
        obj.UDF5 = getUDFValues('UDF5', ordDetailID);

        return obj;
    };

    self.ReceiveNew = function(ordDetailID, obj) {

        $(obj).attr('disabled', 'disabled');
        $('#DivLoading').show();
        var isError = false;
        var ErrorMassage = "";

        var tr = $(obj).parent().parent();
        var data = self.getHiddenFieldsData(tr, ordDetailID);
        
        var RecvQty = 0;
        var RecvItemNumber = data.ItemNumber; //'@Model.ItemNumber'; // @*'@Model.ItemNumber';*@
        var RecvCost = data.ItemCost; //'@Model.ItemCost';
        var RecvOrderNumber = data.OrderNumber; //'@Model.OrderNumber';
        var RecvReleaseNumber = data.OrderReleaseNumber; //'@Model.OrderReleaseNumber';
        var RecvIsPackslipMandatory = data.IsPackSlipNumberMandatory //'@Model.IsPackSlipNumberMandatory';
        var RecvItemTypeSerialLot = '';
        var RecvSerialNumberTracking = data.SerialNumberTracking; //'@Model.SerialNumberTracking';
        var RecvLotNumberTracking = data.LotNumberTracking; //'@Model.LotNumberTracking';
        var RecvDateCodeTracking = data.DateCodeTracking; //'@Model.DateCodeTracking';
        var RecvRequestedQty = data.RequestedQuantity; //'@Model.RequestedQuantity';
        var RecvApprovedQty = data.ApprovedQuantity;// '@Model.ApprovedQuantity';
        var RecvReceivedQty = data.ReceivedQuantity; //'@Model.ReceivedQuantity';
        var RecvItemGUID = data.ItemGUID; //'@Model.ItemGUID';
        var RecvOrderDetailGUID = data.OrderDetailGUID; //'@Model.OrderDetailGUID';
        var RecvOrderGUID = data.OrderGUID; //'@Model.OrderGUID';
        var RecvOrderStatus = data.OrderStatus; //'@Model.OrderStatus';
        var RecvDate = getCurrentDate();
        var RecvPackSlip = data.PackSlipNumber; //$(tr).find('#txtPackslip_' + ordDetailID).val();
        var RecvComment = data.comment; //$(tr).find('#Comment_' + ordDetailID).val();
        var RecvBin = data.ReceiveBinName; //$(tr).find('#txtReceiveBinNumber_' + ordDetailID).val();

        var RecvUDF1 = data.UDF1; //getUDFValues('UDF1', ordDetailID);
        var RecvUDF2 = data.UDF2; //getUDFValues('UDF2', ordDetailID);
        var RecvUDF3 = data.UDF3; //getUDFValues('UDF3', ordDetailID);
        var RecvUDF4 = data.UDF4; //getUDFValues('UDF4', ordDetailID);
        var RecvUDF5 = data.UDF5; //getUDFValues('UDF5', ordDetailID);
        var ExpDate = '';
        var Lot = '';
        var Serial = '';

        /// WI-6215 Related Changes Start ///
        var CurrentReceiveCost = 0;
        var OldReceiveCost = 0;
        var IsReceivedCostChange = false;
        if ($(tr).find("#txtCostBox_" + ordDetailID).length > 0) {
            CurrentReceiveCost = $(tr).find("#txtCostBox_" + ordDetailID).val();
        }
        if ($(tr).find("#hdnCostBox_" + ordDetailID).length > 0) {
            OldReceiveCost = $(tr).find("#hdnCostBox_" + ordDetailID).val();
        }
        var CurrencyDecimalDigits = _ReceiveItem.CurrencyDecimalDigits;
        if (CurrencyDecimalDigits == null || CurrencyDecimalDigits == "") {
            CurrencyDecimalDigits = 0;
        }
        if (parseFloat(CurrentReceiveCost).toFixed(CurrencyDecimalDigits)
            != parseFloat(OldReceiveCost).toFixed(CurrencyDecimalDigits)) {
            IsReceivedCostChange = true;
        }        
        /// WI-6215 Related Changes END ///

        if ($(tr).find("#txtCostBox_" + ordDetailID).length > 0) {
            RecvCost = $(tr).find("#txtCostBox_" + ordDetailID).val();
        }
        else if ($(tr).find("#hdnCostBox_" + ordDetailID).length > 0) {
            RecvCost = $(tr).find("#hdnCostBox_" + ordDetailID).val();
        }

        if ($(tr).find("#txtReceived_" + ordDetailID).length > 0) {
            RecvDate = $(tr).find("#txtReceived_" + ordDetailID).val();
        }

        if ($(tr).find("#txtReceiveQuantity_" + ordDetailID).length > 0 && RecvQty == 0)
            RecvQty = $(tr).find("#txtReceiveQuantity_" + ordDetailID).val();

        if (RecvSerialNumberTracking == 'True') {
            RecvItemTypeSerialLot = "Serial#";
            RecvSerialNumberTracking = true;
            RecvQty = 1;
            Serial = $(tr).find("#txtsrnumber_" + ordDetailID).val();
        }
        else if (RecvLotNumberTracking == 'True') {
            RecvItemTypeSerialLot = "Lot#";
            RecvLotNumberTracking = true;

            Lot = $(tr).find("#txtlotnumber_" + ordDetailID).val();
        }
        if (RecvDateCodeTracking == 'True') {
            if (RecvItemTypeSerialLot.length > 0)
                RecvItemTypeSerialLot = RecvItemTypeSerialLot + " " + lblAndExpiration;
            else
                RecvItemTypeSerialLot = RecvItemTypeSerialLot + " " + lblExpirationDate;

            RecvDateCodeTracking = true;
            ExpDate = $(tr).find("#txtExpiration_" + ordDetailID).val();
        }
        if (RecvItemTypeSerialLot.length > 0) {
            RecvItemTypeSerialLot = RecvItemTypeSerialLot + " " + lblTrackingItem;
        }

        if (RecvBin.length <= 0) {
            isError = true;
            ErrorMassage += "<b style='color: Red;'>" + MsgBinNumberValidation +"</b><br/>"
        }

        if ((RecvSerialNumberTracking == 'True' || RecvSerialNumberTracking == true) && Serial.length <= 0) {
            isError = true;
            ErrorMassage += "<b style='color: Red;'>" + MsgSerialNumberValidation +"</b><br/>"
        }
        else if ((RecvLotNumberTracking == "True" || RecvLotNumberTracking == true) && Lot.length <= 0) {
            isError = true;
            ErrorMassage += "<b style='color: Red;'>" + MsgEnterLotNumber +"</b><br/>"
        }
        else if (isNaN(RecvQty) || RecvQty <= 0) {
            isError = true;
            ErrorMassage += "<b style='color: Red;'>" + MsgEnterQuantityReceive +"</b><br/>"
        }

        if ((RecvDateCodeTracking == "True" || RecvDateCodeTracking == true) && ExpDate.length < 0) {
            isError = true;
            ErrorMassage += "<b style='color: Red;'>>" + MsgExpireDateValidation+ "</b><br/>"
        }

        if (RecvIsPackslipMandatory.toUpperCase() == 'TRUE' && RecvPackSlip.length <= 0) {
            isError = true;
            ErrorMassage += "<b style='color: Red;'>" + MsgEnterPayslipNumber +"</b><br/>"
        }

        if (isError) {
            ErrorMassage = '<b>' + MsgReceiveReasons +'</b><br />' + ErrorMassage;
            $('#OrdReceivedErrorDialog').find("#OrdReceivErrorMSG").html(ErrorMassage);
            $('#DivLoading').hide();
            $('#OrdReceivedErrorDialog').modal();
            $('#btnReceive_' + ordDetailID).removeAttr('disabled');
            return false;
        }
        else {
            var isTrue = false;

            if (parseFloat(RecvApprovedQty) < (parseFloat(RecvQty) + parseFloat(RecvReceivedQty))) {
                var msg = MsgExceedApprovedQuantity;
                if (confirm(msg)) {
                    isTrue = true;
                }
            }
            else
                isTrue = true;

            if (isTrue) {
                var arrMakePreRecieveDetail = new Array();
                var arrFillPreReciveInfo = new Array();

                var makePreReceiveDetail = { "Quantity": RecvQty, "ExpirationDate": ExpDate, "LotNumber": Lot, "SerialNumber": Serial };
                arrMakePreRecieveDetail.push(makePreReceiveDetail);
                var toFillPreReceive = {
                    "ItemNumber": RecvItemNumber, "OrderNumber": RecvOrderNumber,
                    "ReleaseNumber": RecvReleaseNumber,
                    "ItemTypeSerialLot": RecvItemTypeSerialLot, "IsPackSlipMandatory": RecvIsPackslipMandatory,
                    "SerialNumberTracking": RecvSerialNumberTracking, "LotNumberTracking": RecvLotNumberTracking,
                    "DateCodeTracking": RecvDateCodeTracking, "IsModelShow": false, "Cost": RecvCost,
                    "BinNumber": RecvBin, "ReceivedDate": RecvDate, "PackSlipNumber": RecvPackSlip,
                    "Comment": RecvComment,
                    "UDF1": RecvUDF1, "UDF2": RecvUDF2, "UDF3": RecvUDF3, "UDF4": RecvUDF4, "UDF5": RecvUDF5,
                    "ItemGUID": RecvItemGUID, "OrderDetailGUID": RecvOrderDetailGUID, "OrderGUID": RecvOrderGUID,
                    "OrderStatus": RecvOrderStatus, "RequestedQty": RecvRequestedQty, "ApproveQty": RecvApprovedQty,
                    "ReceiveQty": RecvReceivedQty, "MakePreReceiveDetail": arrMakePreRecieveDetail
                    , "IsReceivedCostChange": IsReceivedCostChange
                };                
                arrFillPreReciveInfo.push(toFillPreReceive);
                $('#DivLoading').show();

                $.ajax({
                    "url": _ReceiveItem.urls.saveReceiveInformationURL,
                    "data": JSON.stringify(arrFillPreReciveInfo),
                    "type": 'POST',
                    "async": false,
                    "cache": false,
                    "dataType": "json",
                    "contentType": "application/json",
                    "success": function (response) {
                        if (response.Status) {
                            SetReplenishRedCount();
                            UDFInsertNewForGrid($('#NewReceiveEntry' + ordDetailID));
                            ImageIDToOpen = "#imgPlusMinus_" + ordDetailID;
                            $('#myDataTable').dataTable().fnStandingRedraw();
                            $("#spanGlobalMessage").removeClass('WarningIcon errorIcon').addClass('succesIcon').text(MsgReceivedSuccessfully);
                            $('#DivLoading').hide();
                            showNotificationDialog();

                        }
                        else if (response.Errors.length > 0) {
                            var err = '';
                            for (var i = 0; i < response.Errors.length; i++) {
                                err = err + '' + response.Errors[i].ErrorMassage;
                            }
                            $('#OrdReceivedErrorDialog').find("#OrdReceivErrorMSG").html(err);
                            $('#OrdReceivedErrorDialog').modal();
                        }
                        else {
                            $('#DivLoading').hide();
                            alert(MsgErrorNotReceived);
                        }
                        $('#DivLoading').hide();
                        $('#btnReceive_' + ordDetailID).removeAttr('disabled');
                        return false;
                    },
                    "error": function (error) {
                        $('#btnReceive_' + ordDetailID).removeAttr('disabled');
                        $('#DivLoading').hide();
                        $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html("<b style='color: Red;'>" + MsgErrorInAjaxrequest+ "</b>");
                        $('#OrdReceivedInfoDialog').modal();
                        return false;
                    },
                    "completed": function (obj) {
                        $('#btnReceive_' + ordDetailID).removeAttr('disabled');
                        $('#myDataTable').find("#imgPlusMinus_" + ordDetailID).click();
                        $('#DivLoading').hide();
                    }
                })
            }
            else {
                $('#btnReceive_' + ordDetailID).removeAttr('disabled');
                $('#DivLoading').hide();
                return false;
            }

        }
    }

    // private functions

    //function getUDFValues(udfName, orddtlID) {
    //    var UdfVal = '';
    //    if ($('#NewReceiveEntry' + orddtlID).find('#' + udfName).length > 0) {
    //        if ($('#NewReceiveEntry' + orddtlID).find("#" + udfName)[0].nodeName === "SELECT")
    //            UdfVal = $('#NewReceiveEntry' + orddtlID).find("#" + +udfName + " option:selected").text();
    //        else
    //            UdfVal = $('#NewReceiveEntry' + orddtlID).find('#' + udfName).val();
    //    }
    //    return UdfVal;
    //}

    function getUDFValues(udfName, orddtlID) {
        var UdfVal = '';
        if ($('#NewReceiveEntry' + orddtlID).find('#' + udfName).length > 0) {
            if ($('#NewReceiveEntry' + orddtlID).find("#" + udfName)[0].nodeName === "SELECT")
                UdfVal = $('#NewReceiveEntry' + orddtlID).find("#" + udfName + " option:selected").text();
            else
                UdfVal = $('#NewReceiveEntry' + orddtlID).find('#' + udfName).val();
        }
        return UdfVal;
    }

    return self;

})(jQuery);